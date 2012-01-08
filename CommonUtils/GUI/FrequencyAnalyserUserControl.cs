using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using CommonUtils;
using CommonUtils.FFT;

namespace CommonUtils.GUI
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
			set { foundMaxFrequency = value; }
		}
		
		public float FoundMaxDecibel
		{
			get { return foundMaxDecibel; }
			set { foundMaxDecibel = value; }
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

			float[] spectrumData = AudioAnalyzer.CreateSpectrumAnalysisLomont(audioData, sampleRate, fftWindowsSize, fftOverlap);
			
			//bmp = AudioAnalyzer.PrepareAndDrawSpectrumAnalysis(spectrumData, sampleRate, fftWindowsSize, fftOverlap, new Size(this.Width, this.Height), showMinFrequency, showMaxFrequency);

			float[] m_mag;
			float[] m_freq;
			AudioAnalyzer.PrepareSpectrumAnalysis(spectrumData, sampleRate, fftWindowsSize, fftOverlap,
			                                      out m_mag, out m_freq, out foundMaxFrequency, out foundMaxDecibel);
			
			bmp = AudioAnalyzer.DrawSpectrumAnalysis(ref m_mag, ref m_freq, new Size(this.Width, this.Height),
			                                   showMinFrequency, showMaxFrequency, foundMaxDecibel, foundMaxFrequency);
			
			// force redraw
			this.Invalidate();
		}
	}
}
