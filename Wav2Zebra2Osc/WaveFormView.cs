using System;
using System.Drawing;
using System.Windows.Forms;

using CommonUtils.MathLib.FFT;

namespace Wav2Zebra2Osc
{
	/// <summary>
	/// Description of WaveFormView.
	/// </summary>
	public partial class WaveFormView : Form
	{
		float[] waveData;
		
		DrawingProperties drawingProperties;
		
		public WaveFormView(float[] waveData)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Ensure the paint methods are called if resized
			ResizeRedraw = true;
			
			this.waveData = waveData;
			
			// define the drawing properties for the waveform
			drawingProperties = DrawingProperties.Blue;
			drawingProperties.Margin = 15;
			drawingProperties.NumberOfHorizontalLines = 4;
			drawingProperties.DrawRoundedRectangles = false;
			drawingProperties.DrawHorizontalTickMarks = false;
			drawingProperties.DrawVerticalTickMarks = true;
			drawingProperties.DrawLabels = false;
			drawingProperties.DisplayTime = true;
			drawingProperties.TimeLineUnit = TimelineUnit.Samples;
		}
		
		void PictureBox1Paint(object sender, PaintEventArgs e)
		{
			int width = pictureBox1.Size.Width;
			int height = pictureBox1.Size.Height;
			
			Graphics g = e.Graphics;

			// get the waveform
			Bitmap _offlineBitmap = AudioAnalyzer.DrawWaveform(waveData,
			                                                   new Size(width, height),
			                                                   1,
			                                                   -1, -1,
			                                                   -1, -1,
			                                                   -1,
			                                                   44100,
			                                                   1,
			                                                   drawingProperties);

			if (_offlineBitmap != null) {

				// draw the offline bitmap
				g.DrawImage(_offlineBitmap, 0, 0);
			}
		}
		
		void PictureBox1Resize(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}
	}
}
