using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using CommonUtils;
using CommonUtils.FFT;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of FrequencyAnalyserUserControl.
	/// </summary>
	public partial class FrequencyAnalyserUserControl : UserControl
	{
		Bitmap bmp;
		
		double sampleRate = 44100;
		int fftWindowsSize = 2048;
		
		float showMinFrequency = 0;
		float showMaxFrequency = 20000;
		
		float foundMaxFrequency = 0;
		float foundMaxDecibel = 0;
		
		// overlap must be an integer smaller than the window size
		// half the windows size is quite normal, sometimes 80% is best?!
		int fftOverlap = 1;

		public FrequencyAnalyserUserControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			              ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
		}

		public float FoundMaxFrequency
		{
			get { return foundMaxFrequency; }
		}
		
		public float FoundMaxDecibel
		{
			get { return foundMaxDecibel; }
		}
		
		public float MinimumFrequency
		{
			get { return showMinFrequency; }
			set { showMinFrequency = value; }
		}

		public float MaximumFrequency
		{
			get { return showMaxFrequency; }
			set { showMaxFrequency = value; }
		}
		
		public double SampleRate
		{
			get { return sampleRate; }
			set { sampleRate = value; }
		}
		
		public int FFTWindowsSize
		{
			get { return fftWindowsSize; }
			set { fftWindowsSize = value; }
		}

		public int FFTOverlap
		{
			get { return fftOverlap; }
			set { fftOverlap = value; }
		}
		
		/// <summary>
		/// <see cref="Control.OnPaint"/>
		/// </summary>
		protected override void OnPaint(PaintEventArgs pe)
		{
			if (bmp != null) {
				pe.Graphics.DrawImage(bmp, 0, 0);
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
		
		private float[] audioData;

		/// <summary>
		/// sets graph data
		/// </summary>
		public void SetAudioData(float[] audioData)
		{
			this.audioData = audioData;
			float[][] spectrogramData = AudioAnalyzer.CreateSpectrogramLomont(audioData, sampleRate, fftWindowsSize, fftOverlap, true);

			//bmp = AudioAnalyzer.PrepareAndDrawSpectrumAnalysis(spectrogramData, sampleRate, fftWindowsSize, fftOverlap, new Size(this.Width, this.Height), showMinFrequency, showMaxFrequency);
			
			int numberOfSegments = spectrogramData.Length; // i.e. 78 images which containt a spectrum which is half the fftWindowsSize (2048)
			int spectrogramLength = spectrogramData[0].Length; // 1024 - half the fftWindowsSize (2048)
			double numberOfSamples = (fftOverlap * numberOfSegments) + fftWindowsSize;
			double seconds = numberOfSamples / sampleRate;
			
			// prepare the data:
			float[] m_mag = new float[spectrogramLength];
			float[] m_freq = new float[spectrogramLength];

			float maxVal = float.MinValue;
			int maxIndex = 0;
			float minVal = float.MaxValue;
			int minIndex = 0;
			for (int i = 0; i < spectrogramLength; i++)
			{
				if (spectrogramData[0][i] > maxVal) {
					maxVal = spectrogramData[0][i];
					maxIndex = i;
				}
				if (spectrogramData[0][i] < minVal) {
					minVal = spectrogramData[0][i];
					minIndex = i;
				}

				m_mag[i] = MathUtils.ConvertAmplitudeToDB((float) spectrogramData[0][i], -120.0f, 18.0f);
				m_freq[i] = MathUtils.ConvertIndexToHz (i, spectrogramLength, sampleRate, fftWindowsSize);
			}			
			this.foundMaxDecibel = MathUtils.ConvertAmplitudeToDB((float) spectrogramData[0][maxIndex], -120.0f, 18.0f);
			this.foundMaxFrequency = MathUtils.ConvertIndexToHz (maxIndex, spectrogramLength, sampleRate, fftWindowsSize);

			bmp = AudioAnalyzer.DrawSpectrumAnalysis(ref m_mag, ref m_freq, new Size(this.Width, this.Height), showMinFrequency, showMaxFrequency, this.foundMaxDecibel, this.foundMaxFrequency);
			
			// force redraw
			this.Invalidate();
		}
	}
}
