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
		private const long serialVersionUID = 1L;
		private float[] waveData;
		private float[] dftData;
		private float[] harmonicsData;
		private float[] emptyData;
		
		private MainForm parentForm;

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
			
			this.waveData = new float[128];
			this.harmonicsData = new float[128];
			this.emptyData = new float[128];
			this.dftData = new float[128];
			for (int i = 0; i < 128; i++)
			{
				this.waveData[i] = 0.0F;
				this.harmonicsData[i] = 0.0F;
				this.emptyData[i] = 0.0F;
				this.dftData[i] = 0.0F;
			}
		}
		
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
				int length = value.Length;
				for (int i = 0; i < length; i++)
				{
					this.harmonicsData[i] = value[i];
				}
			}
			get
			{
				return this.harmonicsData;
			}
		}
		
		public virtual void ClearWaveData()
		{
			Array.Copy(this.emptyData, 0, this.waveData, 0, 128);
			this.Loaded = false;
		}
		
		public virtual void ClearDftData()
		{
			Array.Copy(this.emptyData, 0, this.dftData, 0, 128);
		}
		
		public virtual void ClearHarmonics()
		{
			Array.Copy(this.emptyData, 0, this.harmonicsData, 0, 128);
			this.Loaded = false;
		}
		
		public virtual bool Selected {
			set;
			get;
		}
		
		public virtual bool Loaded {
			set;
			get;
		}

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
			}
			
			DrawingProperties properties = DrawingProperties.Blue;
			properties.Margin = 0;
			properties.NumberOfHorizontalLines = 2;
			properties.DrawRoundedRectangles = false;
			properties.DrawHorizontalTickMarks = false;
			properties.DrawVerticalTickMarks = false;
			properties.DrawLabels = false;
			properties.DisplayTime = false;
			properties.TimeLineUnit = TimelineUnit.Samples;
			
			Bitmap _offlineBitmap = AudioAnalyzer.DrawWaveform(interpolatedData,
			                                                   new Size(this.Width, this.Height),
			                                                   1,
			                                                   -1, -1,
			                                                   -1, -1,
			                                                   -1,
			                                                   44100,
			                                                   1,
			                                                   properties);

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
			for (int i = 0; i < 16; i++)
			{
				if (this != this.parentForm.waveDisplays[i])
				{
					this.parentForm.waveDisplays[i].Selected = false;
					this.parentForm.waveDisplays[i].Refresh();
				}
			}
			this.Selected = true;
			this.Refresh();
		}
	}
}
