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
		
		public void FitToScreen()
		{
			/*
			if (waveStream == null) return;

			int samples = (int)(waveStream.Length / bytesPerSample);
			startPosition = 0;
			SamplesPerPixel = samples / this.Width;
			 */
		}

		public void Zoom(double startLoopRegion, double endLoopRegion)
		{
			/*
			startPosition = leftSample * bytesPerSample;
			SamplesPerPixel = (rightSample - leftSample) / this.Width;
			 */
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

			// draw marker
			using (Pen linePen = new Pen(Color.Black, 1))
			{
				double xLocation = progressPercent * this.Width;
				e.Graphics.DrawLine(linePen, (float) xLocation, 0, (float) xLocation, Height);
			}

			// draw repeat region
			using (Pen linePen = new Pen(Color.Chocolate, 2))
			{
				if (repeatRegion.Height > 0 && repeatRegion.Width > 0)  {
					e.Graphics.DrawRectangle(linePen, repeatRegionStartXPosition, 0, repeatRegion.Width, repeatRegion.Height);
				}
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
					UpdateRepeatRegion();
					break;
				case "SelectionEnd":
					endLoopRegion = soundPlayer.SelectionEnd.TotalSeconds;
					UpdateRepeatRegion();
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
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomWaveViewerMouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CustomWaveViewerMouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomWaveViewerMouseUp);
			this.ResumeLayout(false);
		}
		#endregion
		
		private const int mouseMoveTolerance = 3;
		private bool AllowRepeatRegions = true;
		private bool isMouseDown = false;
		private Rectangle repeatRegion = new Rectangle();
		private int repeatRegionStartXPosition = 0;
		private bool isZooming = false;
		private Point mouseDownPoint;
		private Point currentPoint;
		
		void CustomWaveViewerMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				isMouseDown = true;
				mouseDownPoint = e.Location;
				
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
					// Control is being pressed
					isZooming = true;
				} else {
					isZooming = false;
				}
			}
			else if (e.Button == MouseButtons.Right) {
				FitToScreen();
			}
			
			//base.OnMouseDown(e);
		}
		
		void CustomWaveViewerMouseMove(object sender, MouseEventArgs e)
		{
			currentPoint = e.Location;

			if (isMouseDown && AllowRepeatRegions)
			{
				if (Math.Abs(currentPoint.X - mouseDownPoint.X) > mouseMoveTolerance)
				{
					if (mouseDownPoint.X < currentPoint.X)
					{
						startLoopRegion = ((double)mouseDownPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
						endLoopRegion = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
					}
					else
					{
						startLoopRegion = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
						endLoopRegion = ((double)mouseDownPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
					}
				}
				else
				{
					startLoopRegion = -1;
					endLoopRegion = -1;
				}

				UpdateRepeatRegion();
				
				//base.OnMouseMove(e);
			}
		}
		
		void CustomWaveViewerMouseUp(object sender, MouseEventArgs e)
		{
			if (!isMouseDown)
				return;

			bool updateRepeatRegion = false;
			isMouseDown = false;
			if (Math.Abs(currentPoint.X - mouseDownPoint.X) < mouseMoveTolerance)
			{
				if (PointInRepeatRegion(mouseDownPoint))
				{
					double position = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
					soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
				}
				else
				{
					soundPlayer.SelectionBegin = TimeSpan.Zero;
					soundPlayer.SelectionEnd = TimeSpan.Zero;
					double position = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
					soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
					startLoopRegion = -1;
					endLoopRegion = -1;
					updateRepeatRegion = true;
				}
			}
			else
			{
				soundPlayer.SelectionBegin = TimeSpan.FromSeconds(startLoopRegion);
				soundPlayer.SelectionEnd = TimeSpan.FromSeconds(endLoopRegion);
				double position = startLoopRegion;
				soundPlayer.ChannelPosition = Math.Min(soundPlayer.ChannelLength, Math.Max(0, position));
				updateRepeatRegion = true;
			}

			if (updateRepeatRegion) {
				UpdateRepeatRegion();
				//base.OnMouseUp(e);
			}
			if (isZooming) {
				Zoom(startLoopRegion, endLoopRegion);
			}
		}
		
		private bool PointInRepeatRegion(Point point)
		{
			if (soundPlayer.ChannelLength == 0)
				return false;

			double regionLeft = (soundPlayer.SelectionBegin.TotalSeconds / soundPlayer.ChannelLength) * this.Width;
			double regionRight = (soundPlayer.SelectionEnd.TotalSeconds / soundPlayer.ChannelLength) * this.Width;

			return (point.X >= regionLeft && point.X < regionRight);
		}
		
		private void UpdateRepeatRegion()
		{
			if (soundPlayer == null)
				return;

			double startPercent = startLoopRegion / soundPlayer.ChannelLength;
			double startXLocation = startPercent * this.Width;
			double endPercent = endLoopRegion / soundPlayer.ChannelLength;
			double endXLocation = endPercent * this.Width;

			if (soundPlayer.ChannelLength == 0 ||
			    endXLocation <= startXLocation)
			{
				repeatRegion.Width = 0;
				repeatRegion.Height = 0;
				return;
			}
			
			repeatRegionStartXPosition = (int) startXLocation;
			repeatRegion.Width = (int) (endXLocation - startXLocation);
			repeatRegion.Height = this.Height;
			
			// force redraw
			this.Invalidate();
		}
	}
}