using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;

using System.Windows.Forms;

using CommonUtils.FFT;
using CommonUtils.Audio;

using NAudio.Wave;

namespace CommonUtils.GUI
{
	/// <summary>
	/// Control for viewing waveforms
	/// </summary>
	public class CustomWaveViewer : UserControl
	{
		#region Fields
		private IWaveformPlayer soundPlayer;
		private double startLoopRegion = -1;
		private double endLoopRegion = -1;
		#endregion
		
		private Bitmap offlineBitmap;
		double progressPercent = 0.0;
		//private int samplesPerPixel = 128;
		
		private int waveDisplayResolution = 0;
		private int waveDisplayAmplitude = 1;
		private int waveDisplayStartPosition = 0;
		private double sampleRate = 44100;
		
		public Color PenColor { get; set; }
		public float PenWidth { get; set; }

		public void FitToScreen()
		{
			/*
			if (waveStream == null) return;

			int samples = (int)(waveStream.Length / bytesPerSample);
			startPosition = 0;
			SamplesPerPixel = samples / this.Width;
			 */
		}

		public void Zoom(int leftSample, int rightSample)
		{
			/*
			startPosition = leftSample * bytesPerSample;
			SamplesPerPixel = (rightSample - leftSample) / this.Width;
			 */
		}

		#region MouseSelect
		private Point mousePos, startPos;
		private bool mouseDrag = false;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				startPos = e.Location;
				mousePos = new Point(-1, -1);
				mouseDrag = true;
				DrawVerticalLine(e.X);
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mouseDrag)
			{
				DrawVerticalLine(e.X);
				if (mousePos.X != -1) DrawVerticalLine(mousePos.X);
				mousePos = e.Location;
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (mouseDrag && e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				mouseDrag = false;
				DrawVerticalLine(startPos.X);

				if (mousePos.X == -1) return;
				DrawVerticalLine(mousePos.X);

				int leftX = Math.Min(startPos.X, mousePos.X);
				int rightX = Math.Max(startPos.X, mousePos.X);
				
				soundPlayer.SelectionBegin = TimeSpan.Zero;
				soundPlayer.SelectionEnd = TimeSpan.Zero;
				double position = (double)rightX / (double)this.Width * soundPlayer.ChannelLength;
				soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
				startLoopRegion = -1;
				endLoopRegion = -1;
				
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right) FitToScreen();

			base.OnMouseUp(e);
		}
		#endregion
		
		private void DrawVerticalLine(int x)
		{
			ControlPaint.DrawReversibleLine(PointToScreen(new Point(x, 0)), PointToScreen(new Point(x, Height)), Color.Black);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			FitToScreen();
		}

		/// <summary>
		/// Creates a new WaveViewer control
		/// </summary>
		public CustomWaveViewer()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			              ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			this.DoubleBuffered = true;

			this.PenColor = Color.DodgerBlue;
			this.PenWidth = 1;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		/// <summary>
		/// <see cref="Control.OnPaint"/>
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			if (offlineBitmap != null) {
				e.Graphics.DrawImage(offlineBitmap, 0, 0);
			}

			using (Pen linePen = new Pen(Color.Black, PenWidth))
			{
				double xLocation = progressPercent * this.Width;
				e.Graphics.DrawLine(linePen, (float) xLocation, 0, (float) xLocation, Height);
			}
			
			// Calling the base class OnPaint
			base.OnPaint(e);
		}

		#region Public Methods
		/// <summary>
		/// Register a sound player from which the waveform timeline
		/// can get the necessary playback data.
		/// </summary>
		/// <param name="soundPlayer">A sound player that provides waveform data through the IWaveformPlayer interface methods.</param>
		public void RegisterSoundPlayer(IWaveformPlayer soundPlayer)
		{
			this.soundPlayer = soundPlayer;
			soundPlayer.PropertyChanged += soundPlayer_PropertyChanged;
		}
		#endregion

		private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "SelectionBegin":
					startLoopRegion = soundPlayer.SelectionBegin.TotalSeconds;
					//UpdateRepeatRegion();
					break;
				case "SelectionEnd":
					endLoopRegion = soundPlayer.SelectionEnd.TotalSeconds;
					//UpdateRepeatRegion();
					break;
				case "WaveformData":
					UpdateWaveform();
					break;
				case "ChannelPosition":
					UpdateProgressIndicator();
					break;
				case "ChannelLength":
					startLoopRegion = -1;
					endLoopRegion = -1;
					//UpdateAllRegions();
					break;
			}
		}
		
		private void UpdateWaveform()
		{
			if (soundPlayer == null || soundPlayer.WaveformData == null)
				return;

			if (soundPlayer.WaveformData != null && soundPlayer.WaveformData.Length > 1)
			{
				
				int width = this.Width;
				int height = this.Height;
				this.offlineBitmap = AudioAnalyzer.DrawWaveform(soundPlayer.WaveformData,
				                                                new Size(width, height),
				                                                waveDisplayResolution,
				                                                waveDisplayAmplitude,
				                                                waveDisplayStartPosition,
				                                                sampleRate, true);
				
				/*
				using (Pen linePen = new Pen(PenColor, PenWidth))
				{
					int width = this.Width;
					int height = this.Height;
					if (this.offlineBitmap == null) this.offlineBitmap = new Bitmap( width, height, PixelFormat.Format32bppArgb );
					Graphics g = Graphics.FromImage(offlineBitmap);
					
					samplesPerPixel = (int) (soundPlayer.WaveformData.Length / width);
					
					float x = 0;
					for (x = 0; x < width; x++)
					{
						float low = 0;
						float high = 0;
						if (soundPlayer.WaveformData.Length >= (int)(x*samplesPerPixel)) {
							float sample = soundPlayer.WaveformData[(int)(x*samplesPerPixel)];
							if (sample < low) low = sample;
							if (sample > high) high = sample;
						}

						float lowPercent = (low);
						float highPercent = (high);

						if (lowPercent > 0 || highPercent > 0) {
							g.DrawLine(linePen, x, this.Height * lowPercent, x, this.Height * highPercent);
						}
				}
			}
				 */


				// force redraw
				this.Invalidate();
			}
		}

		private void UpdateProgressIndicator()
		{
			if (soundPlayer != null && soundPlayer.ChannelLength != 0)
			{
				progressPercent = soundPlayer.ChannelPosition / soundPlayer.ChannelLength;

				// force redraw
				this.Invalidate();
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// CustomWaveViewer
			// 
			this.Name = "CustomWaveViewer";
			this.ResumeLayout(false);
		}
		#endregion
	}
}