using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using CommonUtils.MathLib.FFT;

namespace Wav2Zebra2Osc
{
	/// <summary>
	/// Wave Cell Slot
	/// </summary>
	public partial class WaveDisplayUserControl : UserControl
	{
		const long serialVersionUID = 1L;
		
		float[] waveData;
		float[] morphedData;
		
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
			this.morphedData = new float[128];

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
		
		public virtual float[] MorphedData
		{
			set
			{
				Array.Copy(value, 0, this.morphedData, 0, 128);
			}
			get
			{
				return this.morphedData;
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
		
		public virtual void ClearMorphedData()
		{
			Array.Clear(this.morphedData, 0, this.morphedData.Length);
		}
		#endregion
		
		private void WaveDisplayUserControl_Paint(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			
			int height = Height;
			int width = Width;

			// if selected, highlight
			if (this.Selected)
			{
				drawingProperties.FillColor = Color.Gray;
			} else {
				drawingProperties.FillColor = Color.White;
			}

			float[] waveData;
			if (this.parentForm.DoShowRAWWaves)
			{
				waveData = this.waveData;
			}
			else
			{
				waveData = this.morphedData;
			}
			
			// get the waveform
			Bitmap _offlineBitmap = AudioAnalyzer.DrawWaveform(waveData,
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
			string fileName = Path.GetFileNameWithoutExtension(this.FileName);
			SizeF drawFileNameTextSize = g.MeasureString(fileName, drawFileNameFont);
			g.DrawString(fileName, drawFileNameFont, Brushes.Black, Width - drawFileNameTextSize.Width - 2, 1);
		}

		void WaveDisplayUserControlDoubleClick(object sender, System.EventArgs e)
		{
			this.parentForm.MakeUserLoadCell();
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
			this.parentForm.LoadSelectedCellIntoAudioSystem();
			this.Refresh();
		}
		
		#region Key Events
		/// <summary>Keys which can generate OnKeyDown event.</summary>
		private static readonly Keys[] InputKeys = new []
		{ Keys.Left, Keys.Up, Keys.Right, Keys.Down, Keys.Oemcomma, Keys.Home, Keys.OemPeriod, Keys.End, Keys.Decimal };

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			if(Array.IndexOf<Keys>(InputKeys, e.KeyCode) != -1)
			{
				e.IsInputKey = true;
			}
			base.OnPreviewKeyDown(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			if (e.KeyCode == Keys.Up) {
				this.parentForm.MoveUp();
			} else if (e.KeyCode == Keys.Down) {
				this.parentForm.MoveDown();
			} else if (e.KeyCode == Keys.Right) {
				this.parentForm.MoveRight();
			} else if (e.KeyCode == Keys.Left) {
				this.parentForm.MoveLeft();
			}
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("Filename: '{0}', Loaded: {1}, Selected: {2}", FileName, Loaded, Selected);
		}
		
	}
}
