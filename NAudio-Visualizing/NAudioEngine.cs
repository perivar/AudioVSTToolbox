// (c) Copyright Jacob Johnston.
// Modified by perivar@nerseth.com
// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;

using NAudio.Wave;

using CommonUtils.Audio;
using CommonUtils.MathLib.FFT;

namespace NAudio_Visualizing
{
	class NAudioEngine : INotifyPropertyChanged, IDisposable, IWaveformPlayer, ISpectrumPlayer
	{
		#region Enums
		public enum StereoProcessingType {
			CHANNEL_STEREO_LEFT = 1,
			CHANNEL_STEREO_RIGHT = 2,
			CHANNEL_MONOMIX = 3
		}
		#endregion
		
		#region Fields
		private static NAudioEngine instance;
		private readonly Timer positionTimer = new Timer();
		private readonly BackgroundWorker waveformGenerateWorker = new BackgroundWorker();
		private readonly int fftDataSize = (int)FFTDataSize.FFT8192;
		private bool disposed;
		private bool canPlay;
		private bool canPause;
		private bool canStop;
		private bool isPlaying;
		private bool inChannelTimerUpdate;
		private double channelLength;
		private double channelPosition;
		private bool inChannelSet;
		private WaveOut waveOutDevice;
		private WaveStream activeStream;
		private WaveChannel32 inputStream;
		private SampleAggregator sampleAggregator;
		private SampleAggregator waveformAggregator;
		private string pendingWaveformPath;
		private float[] waveformData;
		private TagLib.File fileTag;
		private TimeSpan repeatStart;
		private TimeSpan repeatStop;
		private bool inRepeatSet;
		private StereoProcessingType stereoProcessing = StereoProcessingType.CHANNEL_MONOMIX;
		#endregion

		#region Constants
		private const int repeatThreshold = 200;
		#endregion

		#region Singleton Pattern
		public static NAudioEngine Instance
		{
			get
			{
				if (instance == null)
					instance = new NAudioEngine();
				return instance;
			}
		}
		#endregion

		#region Constructor
		private NAudioEngine()
		{
			positionTimer.Interval = 50;
			positionTimer.Tick += positionTimer_Tick;

			waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
			waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
			waveformGenerateWorker.WorkerSupportsCancellation = true;
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!disposed)
			{
				if(disposing)
				{
					StopAndCloseStream();
				}

				disposed = true;
			}
		}
		#endregion

		#region ISpectrumPlayer
		public bool GetFFTData(float[] fftDataBuffer)
		{
			sampleAggregator.GetFFTResults(fftDataBuffer);
			return isPlaying;
		}

		public int GetFFTFrequencyIndex(int frequency)
		{
			double sampleRate;
			if (ActiveStream != null) {
				sampleRate = ActiveStream.WaveFormat.SampleRate;
			} else {
				sampleRate = 44100; // Assume a default 44.1 kHz sample rate.
			}
			
			return CommonUtils.MathUtils.FreqToIndex(frequency, sampleRate, fftDataSize);
		}
		#endregion

		#region IWaveformPlayer
		public TimeSpan SelectionBegin
		{
			get { return repeatStart; }
			set
			{
				if (!inRepeatSet)
				{
					inRepeatSet = true;
					TimeSpan oldValue = repeatStart;
					repeatStart = value;
					if (oldValue != repeatStart)
						NotifyPropertyChanged("SelectionBegin");
					inRepeatSet = false;
				}
			}
		}

		public TimeSpan SelectionEnd
		{
			get { return repeatStop; }
			set
			{
				if (!inChannelSet)
				{
					inRepeatSet = true;
					TimeSpan oldValue = repeatStop;
					repeatStop = value;
					if (oldValue != repeatStop)
						NotifyPropertyChanged("SelectionEnd");
					inRepeatSet = false;
				}
			}
		}

		public float[] WaveformData
		{
			get { return waveformData; }
			protected set
			{
				float[] oldValue = waveformData;
				waveformData = value;
				if (oldValue != waveformData)
					NotifyPropertyChanged("WaveformData");
			}
		}

		public double ChannelLength
		{
			get { return channelLength; }
			protected set
			{
				double oldValue = channelLength;
				channelLength = value;
				if (oldValue != channelLength)
					NotifyPropertyChanged("ChannelLength");
			}
		}

		public double ChannelPosition
		{
			get { return channelPosition; }
			set
			{
				if (!inChannelSet)
				{
					inChannelSet = true; // Avoid recursion
					double oldValue = channelPosition;
					double position = Math.Max(0, Math.Min(value, ChannelLength));
					if (!inChannelTimerUpdate && ActiveStream != null)
						ActiveStream.Position = (long)((position / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
					channelPosition = position;
					if (oldValue != channelPosition)
						NotifyPropertyChanged("ChannelPosition");
					inChannelSet = false;
				}
			}
		}
		#endregion

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion

		#region Waveform Generation
		private class WaveformGenerationParams
		{
			public WaveformGenerationParams(string path)
			{
				Path = path;
			}

			public string Path { get; protected set; }
		}

		private void GenerateWaveformData(string path)
		{
			if (waveformGenerateWorker.IsBusy)
			{
				pendingWaveformPath = path;
				waveformGenerateWorker.CancelAsync();
				return;
			}

			if (!waveformGenerateWorker.IsBusy)
				waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(path));
		}

		private void waveformGenerateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (!waveformGenerateWorker.IsBusy)
					waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(pendingWaveformPath));
			}
		}

		private void waveformGenerateWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var waveformParams = e.Argument as WaveformGenerationParams;
			
			ISampleProvider sampleProvider = new AudioFileReader(waveformParams.Path);
			var fileWaveStream = (WaveStream) sampleProvider;
			var waveformInputStream = new WaveChannel32(fileWaveStream);
			waveformInputStream.PadWithZeroes = false;
			waveformInputStream.Sample += waveStream_Sample;
			
			int frameLength = fftDataSize;
			int frameCount = (int)((double)waveformInputStream.Length / (double)frameLength);
			int waveformLength = frameCount * 2;
			var samples = new float[frameLength];
			var floatList = new List<float>();
			while(sampleProvider.Read(samples, 0, samples.Length) > 0) {
				if (waveformInputStream.WaveFormat.Channels == 1) {
					floatList.AddRange(samples);
				} else if (waveformInputStream.WaveFormat.Channels == 2) {
					switch(stereoProcessing) {
						case StereoProcessingType.CHANNEL_STEREO_LEFT:
							for (int i = 0; i < samples.Length; i+=2) {
								float left = samples[i];
								float right = samples[i+1];
								floatList.Add(left);
							}
							break;
						case StereoProcessingType.CHANNEL_STEREO_RIGHT:
							for (int i = 0; i < samples.Length; i+=2) {
								float left = samples[i];
								float right = samples[i+1];
								floatList.Add(right);
							}
							break;
						case StereoProcessingType.CHANNEL_MONOMIX:
						default:
							for (int i = 0; i < samples.Length; i+=2) {
								float left = samples[i];
								float right = samples[i+1];
								// Make stored channel data stereo by averaging left and right values.
								floatList.Add(( (left + right) / 2.0f));
							}
							break;
					}
				}

				if (waveformGenerateWorker.CancellationPending) {
					e.Cancel = true;
					break;
				}
			}

			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			                                               {
			                                               	WaveformData = floatList.ToArray();
			                                               }));

			waveformInputStream.Close();
			waveformInputStream.Dispose();
			waveformInputStream = null;
		}
		#endregion

		#region Private Utility Methods
		private void StopAndCloseStream()
		{
			if (waveOutDevice != null) {
				waveOutDevice.Stop();
			}
			if (activeStream != null) {
				activeStream.Close();
				activeStream = null;
				//inputStream.Close();
				//inputStream = null;
			}
			if (waveOutDevice != null) {
				waveOutDevice.Dispose();
				waveOutDevice = null;
			}
		}
		#endregion

		#region Public Methods
		public void Stop()
		{
			if (waveOutDevice != null) {
				waveOutDevice.Stop();
			}
			IsPlaying = false;
			CanStop = false;
			CanPlay = true;
			CanPause = false;
		}

		public void Pause()
		{
			if (IsPlaying && CanPause) {
				waveOutDevice.Pause();
				IsPlaying = false;
				CanPlay = true;
				CanPause = false;
			}
		}

		public void Play()
		{
			if (CanPlay) {
				waveOutDevice.Play();
				IsPlaying = true;
				CanPause = true;
				CanPlay = false;
				CanStop = true;
			}
		}

		public void OpenFile(string path)
		{
			Stop();

			if (ActiveStream != null) {
				SelectionBegin = TimeSpan.Zero;
				SelectionEnd = TimeSpan.Zero;
				ChannelPosition = 0;
			}
			
			StopAndCloseStream();

			if (File.Exists(path)) {
				try {
					waveOutDevice = new WaveOut()
					{
						DesiredLatency = 100
					};
					
					ActiveStream = (WaveStream) new AudioFileReader(path);
					inputStream = new WaveChannel32(ActiveStream);
					sampleAggregator = new SampleAggregator(fftDataSize);
					inputStream.Sample += inputStream_Sample;
					waveOutDevice.Init(inputStream);
					ChannelLength = inputStream.TotalTime.TotalSeconds;
					FileTag = TagLib.File.Create(path);
					GenerateWaveformData(path);
					CanPlay = true;
				}
				catch
				{
					ActiveStream = null;
					CanPlay = false;
				}
			}
		}
		#endregion

		#region Public Properties
		public TagLib.File FileTag
		{
			get { return fileTag; }
			set
			{
				TagLib.File oldValue = fileTag;
				fileTag = value;
				if (oldValue != fileTag)
					NotifyPropertyChanged("FileTag");
			}
		}

		public WaveStream ActiveStream
		{
			get { return activeStream; }
			protected set
			{
				WaveStream oldValue = activeStream;
				activeStream = value;
				if (oldValue != activeStream)
					NotifyPropertyChanged("ActiveStream");
			}
		}

		public bool CanPlay
		{
			get { return canPlay; }
			protected set
			{
				bool oldValue = canPlay;
				canPlay = value;
				if (oldValue != canPlay)
					NotifyPropertyChanged("CanPlay");
			}
		}

		public bool CanPause
		{
			get { return canPause; }
			protected set
			{
				bool oldValue = canPause;
				canPause = value;
				if (oldValue != canPause)
					NotifyPropertyChanged("CanPause");
			}
		}

		public bool CanStop
		{
			get { return canStop; }
			protected set
			{
				bool oldValue = canStop;
				canStop = value;
				if (oldValue != canStop)
					NotifyPropertyChanged("CanStop");
			}
		}

		public bool IsPlaying
		{
			get { return isPlaying; }
			protected set
			{
				bool oldValue = isPlaying;
				isPlaying = value;
				if (oldValue != isPlaying)
					NotifyPropertyChanged("IsPlaying");
				
				positionTimer.Enabled = value;
			}
		}
		
		public int SampleRate {
			get {
				if (ActiveStream != null) {
					return ActiveStream.WaveFormat.SampleRate;
				} else {
					// Assume a default 44.1 kHz sample rate.
					return 44100;
				}
			}
		}
		
		public int FftDataSize {
			get {
				return fftDataSize;
			}
		}
		
		public StereoProcessingType StereoProcessing {
			set {
				stereoProcessing = value;
			}
			get {
				return stereoProcessing;
			}
		}
		#endregion

		#region Event Handlers
		private void inputStream_Sample(object sender, SampleEventArgs e)
		{
			sampleAggregator.Add(e.Left, e.Right);
			long repeatStartPosition = (long)((SelectionBegin.TotalSeconds / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
			long repeatStopPosition = (long)((SelectionEnd.TotalSeconds / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
			if (((SelectionEnd - SelectionBegin) >= TimeSpan.FromMilliseconds(repeatThreshold)) && ActiveStream.Position >= repeatStopPosition) {
				sampleAggregator.Clear();
				ActiveStream.Position = repeatStartPosition;
			}
		}

		void waveStream_Sample(object sender, SampleEventArgs e)
		{
			waveformAggregator.Add(e.Left, e.Right);
		}

		void positionTimer_Tick(object sender, EventArgs e)
		{
			inChannelTimerUpdate = true;
			ChannelPosition = ((double)ActiveStream.Position / (double)ActiveStream.Length) * ActiveStream.TotalTime.TotalSeconds;
			inChannelTimerUpdate = false;
		}
		#endregion
	}
}
