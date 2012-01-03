using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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
		int fftWindowsSize = 256;
		
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
		
		public static Bitmap DrawNormalizedAudio(ref float[] data, Color foreColor, Color backColor, Size imageSize)
		{
			// http://code.google.com/p/vocalize-ufo/source/browse/trunk/Vocalize+UFO/src/SpectrumAnalyzer.as?spec=svn27&r=27
			Bitmap bmp = new Bitmap(imageSize.Width, imageSize.Height);

			int BORDER_WIDTH = 2;
			int width = bmp.Width - (2 * BORDER_WIDTH);
			int height = bmp.Height - (2 * BORDER_WIDTH);

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(backColor);
				Pen pen = new Pen(foreColor);
				int size = data.Length;
				for (int iPixel = 0; iPixel < width; iPixel++)
				{
					// determine start and end points within WAV
					int start = (int)((float)iPixel * ((float)size / (float)width));
					int end = (int)((float)(iPixel + 1) * ((float)size / (float)width));
					float min = float.MaxValue;
					float max = float.MinValue;
					for (int i = start; i < end; i++)
					{
						float val = data[i];
						min = val < min ? val : min;
						max = val > max ? val : max;
					}
					int yMax = BORDER_WIDTH + height - (int)((max + 1) * .5 * height);
					int yMin = BORDER_WIDTH + height - (int)((min + 1) * .5 * height);
					g.DrawLine(pen, iPixel + BORDER_WIDTH, yMax,
					           iPixel + BORDER_WIDTH, yMin);
				}
			}

			return bmp;
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
			float[][] spectrogramData = FFTUtils.CreateSpectrogram(audioData, sampleRate, fftWindowsSize, fftOverlap);
			bmp = FFTUtils.PrepareAndDrawSpectrumAnalysis(spectrogramData, sampleRate, fftWindowsSize, fftOverlap, ForeColor, BackColor, new Size(this.Width, this.Height));
			
			this.Invalidate();
		}
	}
}
