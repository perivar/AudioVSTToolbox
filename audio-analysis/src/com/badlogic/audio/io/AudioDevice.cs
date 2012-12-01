using System;
using System.Threading;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using AudioDevice = com.badlogic.audio.io.AudioDevice;

namespace com.badlogic.audio.io
{
	/**
	 * Class that allows directly passing PCM float mono
	 * data to the sound card for playback. The sampling
	 * rate of the PCM data must be 44100Hz.
	 * @author mzechner
	 * @author perivar@nerseth.com
	 */
	public class AudioDevice
	{
		/// the buffer size in samples
		private const int BUFFER_SIZE = 1024;

		// the sound line we write our samples to
		private IWavePlayer waveOut;

		// buffer for BUFFER_SIZE 32-bit samples
		private byte[] buffer = new byte[BUFFER_SIZE*4];

		// PlayBuffer
		private BufferedWaveProvider PlayBuffer;
		
		// fileWaveStream
		private WaveStream fileWaveStream;
		private SampleChannel sampleChannel;
		
		public WaveStream WaveStream { get { return fileWaveStream; } }
		public SampleChannel SampleChannel { get { return sampleChannel; } }
		public IWavePlayer WavePlayer { get { return waveOut; } }
		
		public TimeSpan Elapsed {
			get {
				return (waveOut.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : fileWaveStream.CurrentTime;
			}
		}
		
		/**
		 * Constructor, initializes the audio system for
		 * 44100Hz 32-bit float stereo output.
		 */
		public AudioDevice()
		{
			// BufferedWaveProvider
			PlayBuffer = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
			PlayBuffer.BufferDuration = TimeSpan.FromMinutes(10);

			// waveOut
			/*
			 * This is the default and recommended approach if you are creating a WaveOut object
			 * from the GUI thread of a Windows Forms or WPF application.
			 * Whenever WaveOut wants more data it posts a message that is handled by the
			 * Windows message pump of an invisible new window.
			 * You get this callback model by default when you call the empty WaveOut constructor.
			 * However, it will not work on a background thread, since there is no message pump.
			 * */
			waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()); // seems to be the best way
			//waveOut = new WaveOut();
			//waveOut.Volume = 0.5f;
			waveOut.Init(PlayBuffer);
			waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(_waveOutDevice_PlaybackStopped);
			waveOut.Play();
		}
		
		public AudioDevice(string fileName) : this() {
			ISampleProvider sampleProvider = new AudioFileReader(fileName);
			this.fileWaveStream = (WaveStream) sampleProvider;
			
			// add event
			SampleToWaveProvider waveProvider = new SampleToWaveProvider(sampleProvider);
			this.sampleChannel = new SampleChannel(waveProvider, true);
			sampleChannel.PreVolumeMeter += OnPreVolumeMeter;
			
			// play
			//IWavePlayer waveOut = new WaveOut();
			//waveOut.Init(waveProvider);
			//waveOut.Play();
		}
		
		private void OnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
		{
			// we know it is stereo
			//waveformPainter1.AddMax(e.MaxSampleValues[0]);
			//waveformPainter2.AddMax(e.MaxSampleValues[1]);
			float[] max = e.MaxSampleValues;
		}
		
		/**
		 * Writes the given samples to the audio device. The samples
		 * have to be sampled at 44100Hz stereo and have to be in
		 * the range [-1,1].
		 * @param samples The samples.
		 */
		public virtual void WriteSamples(float[] samples)
		{
			FillBuffer(samples);
			PlayBuffer.AddSamples(buffer, 0, buffer.Length);
		}

		/**
		 * Writes the given samples to the audio device.
		 * @param samples The samples.
		 */
		public virtual void WriteSamples(byte[] buffer)
		{
			PlayBuffer.AddSamples(buffer, 0, buffer.Length);
		}
		
		public void Dispose() {
			waveOut.Stop();
			waveOut = null;
		}
		
		private void FillBuffer(float[] samples)
		{
			for(int i = 0, j = 0; i < samples.Length; i++, j+=4)
			{
				byte[] array = BitConverter.GetBytes(samples[i]);
				buffer[j] = array[0];
				buffer[j+1] = array[1];
				buffer[j+2] = array[2];
				buffer[j+3] = array[3];
			}
		}
		
		public static void Main(string[] argv)
		{
			//ISampleProvider reader = new AudioFileReader(@"C:\Users\perivar.nerseth\Music\Sleep Away16.wav");
			//ISampleProvider reader = new AudioFileReader("samples/sample.wav");

			ISampleProvider reader = new AudioFileReader(@"C:\Users\perivar.nerseth\Music\Sleep Away32f.wav");
			AudioDevice device = new AudioDevice();
			
			float[] samples = new float[1024];
			while(reader.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
			}
			
			System.Threading.Thread.Sleep(10000);
			device.Dispose();

			/*
			WaveFileReader wfr = new WaveFileReader("samples/sample.wav");
			WaveOut audioOutput = new WaveOut();
			WaveChannel32 wc = new WaveChannel32(wfr);
			wc.PadWithZeroes = false;
			audioOutput.Init(wc);
			audioOutput.PlaybackStopped += new EventHandler<StoppedEventArgs>(_waveOutDevice_PlaybackStopped);
			audioOutput.Play();
			
			while (audioOutput.PlaybackState != PlaybackState.Stopped) {
				System.Threading.Thread.Sleep(100);
			}
			 */
		}
		
		private static void _waveOutDevice_PlaybackStopped (object sender, EventArgs e) {
			System.Console.Out.WriteLine("Stopped!");
		}
		
		public static WaveStream CreateIeeeFloatWaveStream(string fileName)
		{
			WaveStream readerStream = new WaveFileReader(fileName);
			if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
				readerStream = new BlockAlignReductionStream(readerStream);
			}
			return readerStream;
		}
	}
}