using System;
using System.Drawing;
using System.Windows.Forms;

using CommonUtils;

using CommonUtils.FFT;

namespace Wav2Zebra2Osc
{
	/// <summary>
	/// Wave Cell Slot
	/// </summary>
	public partial class WaveDisplayUserControl : UserControl
	{
		const long serialVersionUID = 1L;
		
		float[] waveData;
		float[] dftData;
		float[] harmonicsData;
		
		MainForm parentForm;
		DrawingProperties drawingProperties;

		public WaveDisplayUserControl(MainForm parentForm)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Ensure the paint methods are called if resized
			ResizeRedraw = true;
			
			//
			// Constructor code after the InitializeComponent() call.
			//
			this.parentForm = parentForm;
			
			// initialize the data arrays
			this.waveData = new float[128];
			this.harmonicsData = new float[128];
			this.dftData = new float[128];

			// define the drawing properties for the waveform
			drawingProperties = DrawingProperties.Blue;
			drawingProperties.Margin = 0;
			drawingProperties.NumberOfHorizontalLines = 2;
			drawingProperties.DrawRoundedRectangles = false;
			drawingProperties.DrawHorizontalTickMarks = false;
			drawingProperties.DrawVerticalTickMarks = false;
			drawingProperties.DrawLabels = false;
			drawingProperties.DisplayTime = false;
			drawingProperties.TimeLineUnit = TimelineUnit.Samples;
		}
		
		#region Set and Get Properties
		public virtual string FileName {
			set;
			get;
		}
		
		public virtual float[] WaveData
		{
			set
			{
				Array.Copy(value, 0, this.waveData, 0, 128);
			}
			get
			{
				return this.waveData;
			}
		}
		
		public virtual float[] DftData
		{
			set
			{
				Array.Copy(value, 0, this.dftData, 0, 128);
			}
			get
			{
				return this.dftData;
			}
		}
		
		public virtual float[] HarmonicsData
		{
			set
			{
				Array.Copy(value, 0, this.harmonicsData, 0, 128);
			}
			get
			{
				return this.harmonicsData;
			}
		}

		public virtual bool Selected {
			set;
			get;
		}
		
		public virtual bool Loaded {
			set;
			get;
		}
		#endregion
		
		#region Clear the arrays
		public virtual void ClearWaveData()
		{
			Array.Clear(this.waveData, 0, this.waveData.Length);
			this.Loaded = false;
		}
		
		public virtual void ClearDftData()
		{
			Array.Clear(this.dftData, 0, this.dftData.Length);
		}
		
		public virtual void ClearHarmonics()
		{
			Array.Clear(this.harmonicsData, 0, this.harmonicsData.Length);
			this.Loaded = false;
		}
		#endregion
		
		private void WaveDisplayUserControl_Paint(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			
			int height = Height;
			int width = Width;
			
			// set white background
			//g.Clear(Color.White);

			// if selected, highlight
			// TODO: fix highligting selected
			if (this.Selected)
			{
				//g.Clear(Color.Black);
			}

			float[] interpolatedData;
			if (this.parentForm.DoShowRAWWaves)
			{
				interpolatedData = this.waveData;
			}
			else
			{
				interpolatedData = this.dftData;
				//interpolatedData = this.harmonicsData;
			}
			
			// get the waveform
			Bitmap _offlineBitmap = AudioAnalyzer.DrawWaveform(interpolatedData,
			                                                   new Size(this.Width, this.Height),
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
			
			var drawFileNameFont = new Font("Arial", 9);
			string fileName = System.IO.Path.GetFileNameWithoutExtension(this.FileName);
			SizeF drawFileNameTextSize = g.MeasureString(fileName, drawFileNameFont);
			g.DrawString(fileName, drawFileNameFont, Brushes.Black, Width - drawFileNameTextSize.Width - 2, 1);
		}

		void WaveDisplayUserControlDoubleClick(object sender, System.EventArgs e)
		{
			this.parentForm.LoadCell();
		}
		
		void WaveDisplayUserControlClick(object sender, System.EventArgs e)
		{
			// deselect all other wave displays
			for (int i = 0; i < 16; i++)
			{
				if (this != this.parentForm.waveDisplays[i])
				{
					this.parentForm.waveDisplays[i].Selected = false;
					this.parentForm.waveDisplays[i].Refresh();
				}
			}
			// and select this one
			this.Selected = true;
			this.Refresh();
		}
	}
}
