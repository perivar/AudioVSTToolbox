using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
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
		private Bitmap offlineBitmap;

		double progressPercent = 0.0;

		private double startLoopRegion = -1;
		private double endLoopRegion = -1;

		private int leftSample = -1;
		private int rightSample = -1;
		private int startPosition = 0;
		private float samplesPerPixel = 128;
		#endregion
		
		public void Zoom(int leftSample, int rightSample)
		{
			startPosition = leftSample;
			samplesPerPixel = (float) (rightSample - leftSample) / (float) this.Width;

			UpdateWaveform();
		}
		
		public void FitToScreen()
		{
			if (soundPlayer != null && soundPlayer.WaveformData != null && soundPlayer.WaveformData.Length > 1)
			{
				int totalNumberOfSamples = soundPlayer.WaveformData.Length;
				samplesPerPixel = (float) totalNumberOfSamples / (float) this.Width;

				startPosition = 0;
				leftSample = 0;
				rightSample = totalNumberOfSamples;
			}

			UpdateWaveform();
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
			using (Pen markerPen = new Pen(Color.Black, 1))
			{
				double xLocation = progressPercent * this.Width;
				e.Graphics.DrawLine(markerPen, (float) xLocation, 0, (float) xLocation, Height);
			}

			// draw loop region
			using (Pen loopPen = new Pen(Color.Chocolate, 2))
			{
				if (loopRegion.Height > 0 && loopRegion.Width > 0)  {
					e.Graphics.DrawRectangle(loopPen, loopRegionStartXPosition, 0, loopRegion.Width, loopRegion.Height);
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
					UpdateLoopRegion();
					break;
				case "SelectionEnd":
					endLoopRegion = soundPlayer.SelectionEnd.TotalSeconds;
					UpdateLoopRegion();
					break;
				case "WaveformData":
					FitToScreen();
					//UpdateWaveform();
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
				if (this.offlineBitmap == null) this.offlineBitmap = new Bitmap( this.Width, this.Height, PixelFormat.Format32bppArgb );
				int height = offlineBitmap.Height;
				int width = offlineBitmap.Width;
				
				Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
				Color fillColor = ColorTranslator.FromHtml("#F9C998");
				using (Pen linePen = new Pen(sampleColor, 1))
				{
					using (Graphics g = Graphics.FromImage(offlineBitmap))
					{
						g.Clear(fillColor);

						int totalNumberOfSamples = soundPlayer.WaveformData.Length;

						// crop to the zoom area
						float[] data;
						if (rightSample != 0) {
							data = new float[rightSample-leftSample];
							Array.Copy(soundPlayer.WaveformData, leftSample, data, 0, rightSample-leftSample);
						} else {
							data = soundPlayer.WaveformData;
							samplesPerPixel = (float) totalNumberOfSamples / (float) width;
						}
						
						if (samplesPerPixel >= 1) {
							// the number of samples are greater than the available drawing space (i.e. greater than the number of pixles in the X-Axis)
							for (int iPixel = 0; iPixel < offlineBitmap.Width; iPixel++)
							{
								// determine start and end points within waveform (for this single pixel on the X axis)
								int start 	= (int)((float)(iPixel) 		* samplesPerPixel);
								int end 	= (int)((float)(iPixel + 1) 	* samplesPerPixel);
								
								float min = float.MaxValue;
								float max = float.MinValue;
								for (int i = start; i < end; i++)
								{
									float val = data[i];
									min = val < min ? val : min;
									max = val > max ? val : max;
								}
								int yMax = height - (int)((max + 1) * .5 * height);
								int yMin = height - (int)((min + 1) * .5 * height);
								g.DrawLine(linePen, iPixel, yMax, iPixel, yMin);
							}
						} else {
							// the number of samples are less than the available drawing space
							// (i.e. less than the number of pixles in the X-Axis)
							float x, y = 0;
							int samples = data.Length;
							if (samples > 1) {
								// at least two samples
								float mult_x = (float) width / (data.Length - 1);
								List<Point> ps = new List<Point>();
								for (int i = 0; i < data.Length; i++)
								{
									x = (i * mult_x);
									y = height - (int)((data[i] + 1) * 0.5 * height);
									Point p = new Point((int)x, (int)y);
									ps.Add(p);
								}

								if (ps.Count > 0)
								{
									g.DrawLines(linePen, ps.ToArray());
								}
							} else {
								// we have only one sample, draw a flat line
								g.DrawLine(linePen, 0, 0.5f * height, width, 0.5f * height);
							}
						}
					}
				}

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
		private bool allowLoopRegions = true;
		private bool isMouseDown = false;
		private Rectangle loopRegion = new Rectangle();
		private int loopRegionStartXPosition = 0;
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

			if (isMouseDown)
			{
				if (Math.Abs(currentPoint.X - mouseDownPoint.X) > mouseMoveTolerance)
				{
					if (mouseDownPoint.X < currentPoint.X)
					{
						if (allowLoopRegions) {
							startLoopRegion = ((double)mouseDownPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
							endLoopRegion = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
						}
					}
					else
					{
						if (allowLoopRegions) {
							startLoopRegion = ((double)currentPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
							endLoopRegion = ((double)mouseDownPoint.X / (double)this.Width) * soundPlayer.ChannelLength;
						}
					}

					if (isZooming) {
						leftSample = Math.Max((int)(startPosition + samplesPerPixel * Math.Min(mouseDownPoint.X, currentPoint.X)), 0);
						rightSample = Math.Min((int)(startPosition + samplesPerPixel * Math.Max(mouseDownPoint.X, currentPoint.X)), soundPlayer.WaveformData.Length);
					}
				}
				else
				{
					startLoopRegion = -1;
					endLoopRegion = -1;
				}

				if (allowLoopRegions) {
					UpdateLoopRegion();
				}
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
				if (PointInLoopRegion(mouseDownPoint))
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
				UpdateLoopRegion();
				//base.OnMouseUp(e);
			}
			if (isZooming) {
				//Zoom(startZoomRegion, endZoomRegion);
				Zoom(leftSample, rightSample);
			}
		}
		
		private bool PointInLoopRegion(Point point)
		{
			if (soundPlayer.ChannelLength == 0)
				return false;

			double loopLeft = (soundPlayer.SelectionBegin.TotalSeconds / soundPlayer.ChannelLength) * this.Width;
			double loopRight = (soundPlayer.SelectionEnd.TotalSeconds / soundPlayer.ChannelLength) * this.Width;

			return (point.X >= loopLeft && point.X < loopRight);
		}
		
		private void UpdateLoopRegion()
		{
			if (soundPlayer == null)
				return;

			double startLoopPercent = startLoopRegion / soundPlayer.ChannelLength;
			double startLoopXLocation = startLoopPercent * this.Width;
			double endLoopPercent = endLoopRegion / soundPlayer.ChannelLength;
			double endLoopXLocation = endLoopPercent * this.Width;

			if (soundPlayer.ChannelLength == 0 ||
			    endLoopXLocation <= startLoopXLocation)
			{
				loopRegion.Width = 0;
				loopRegion.Height = 0;
				return;
			}
			
			loopRegionStartXPosition = (int) startLoopXLocation;
			loopRegion.Width = (int) (endLoopXLocation - startLoopXLocation);
			loopRegion.Height = this.Height;
			
			// force redraw
			this.Invalidate();
		}
	}
}