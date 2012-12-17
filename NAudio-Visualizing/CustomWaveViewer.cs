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

		int progressSample = 0;

		private int startLoopSamplePosition = -1;
		private int endLoopSamplePosition = -1;

		private int startZoomSamplePosition = -1;
		private int endZoomSamplePosition = -1;
		private int previousStartZoomSamplePosition = -1;

		private float samplesPerPixel = 128;
		
		private const int mouseMoveTolerance = 3;
		private bool isMouseDown = false;
		private bool isZooming = false;
		private Point mouseDownPoint;
		private Point currentPoint;

		private Rectangle selectRegion = new Rectangle();
		private int startSelectXPosition = -1;
		private int endSelectXPosition = -1;

		#endregion
		
		public void Zoom(int startZoomSamplePosition, int endZoomSamplePosition)
		{
			previousStartZoomSamplePosition = startZoomSamplePosition;
			samplesPerPixel = (float) (endZoomSamplePosition - startZoomSamplePosition) / (float) this.Width;

			// remove select region after zooming
			startSelectXPosition = -1;
			endSelectXPosition = -1;
			selectRegion.Width = 0;
			selectRegion.Height = 0;

			UpdateWaveform();
		}
		
		public void FitToScreen()
		{
			if (soundPlayer != null && soundPlayer.WaveformData != null && soundPlayer.WaveformData.Length > 1)
			{
				int totalNumberOfSamples = soundPlayer.WaveformData.Length;
				samplesPerPixel = (float) totalNumberOfSamples / (float) this.Width;

				previousStartZoomSamplePosition = 0;
				startZoomSamplePosition = 0;
				endZoomSamplePosition = totalNumberOfSamples;
			}

			// remove select region after zooming
			startSelectXPosition = -1;
			endSelectXPosition = -1;
			selectRegion.Width = 0;
			selectRegion.Height = 0;
			
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
				// what samples are we showing?
				if (progressSample >= startZoomSamplePosition && progressSample <= endZoomSamplePosition) {
					double xLocation = (progressSample - startZoomSamplePosition) / samplesPerPixel;
					e.Graphics.DrawLine(markerPen, (float) xLocation, 0, (float) xLocation, Height);
				}
			}
			
			// draw select region
			using (Pen loopPen = new Pen(Color.Red, 2))
			{
				if (selectRegion.Height > 0 && selectRegion.Width > 0)  {
					e.Graphics.DrawRectangle(loopPen, startSelectXPosition, 0, selectRegion.Width, selectRegion.Height);
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
					startLoopSamplePosition = SecondsToSamplePosition(soundPlayer.SelectionBegin.TotalSeconds, soundPlayer.ChannelLength, soundPlayer.WaveformData.Length);
					//UpdateLoopRegion();
					break;
				case "SelectionEnd":
					endLoopSamplePosition = SecondsToSamplePosition(soundPlayer.SelectionEnd.TotalSeconds, soundPlayer.ChannelLength, soundPlayer.WaveformData.Length);
					//UpdateLoopRegion();
					break;
				case "WaveformData":
					FitToScreen();
					break;
				case "ChannelPosition":
					UpdateProgressIndicator();
					break;
				case "ChannelLength":
					startLoopSamplePosition = -1;
					endLoopSamplePosition = -1;
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
						if (endZoomSamplePosition != 0) {
							data = new float[endZoomSamplePosition-startZoomSamplePosition];
							Array.Copy(soundPlayer.WaveformData, startZoomSamplePosition, data, 0, endZoomSamplePosition-startZoomSamplePosition);
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
			if (soundPlayer != null && soundPlayer.ChannelLength != 0
			    && soundPlayer.WaveformData != null && soundPlayer.WaveformData.Length != 0)
			{
				progressSample = SecondsToSamplePosition(soundPlayer.ChannelPosition, soundPlayer.ChannelLength, soundPlayer.WaveformData.Length);
				
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
		
		void CustomWaveViewerMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				isMouseDown = true;
				mouseDownPoint = e.Location;
				
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
					// Control key is being pressed
					isZooming = true;
				} else {
					isZooming = false;
				}
			}
			else if (e.Button == MouseButtons.Right) {
				FitToScreen();
			}
		}
		
		void CustomWaveViewerMouseMove(object sender, MouseEventArgs e)
		{
			currentPoint = e.Location;

			if (soundPlayer.WaveformData == null) return;
			
			if (isMouseDown)
			{
				if (Math.Abs(currentPoint.X - mouseDownPoint.X) > mouseMoveTolerance)
				{
					startSelectXPosition = Math.Min(mouseDownPoint.X, currentPoint.X);
					endSelectXPosition = Math.Max(mouseDownPoint.X, currentPoint.X);
				}
				else
				{
					startSelectXPosition = -1;
					endSelectXPosition = -1;
				}
				
				UpdateSelectRegion();
			}
		}
		
		void CustomWaveViewerMouseUp(object sender, MouseEventArgs e)
		{
			if (!isMouseDown || soundPlayer.WaveformData == null)
				return;
			
			isMouseDown = false;

			if (isZooming) {
				startZoomSamplePosition = Math.Max((int)(previousStartZoomSamplePosition + samplesPerPixel * startSelectXPosition), 0);
				endZoomSamplePosition = Math.Min((int)(previousStartZoomSamplePosition + samplesPerPixel * endSelectXPosition), soundPlayer.WaveformData.Length);
				
				Zoom(startZoomSamplePosition, endZoomSamplePosition);
				return;
			}

			bool doUpdateLoopRegion = false;

			if (Math.Abs(currentPoint.X - mouseDownPoint.X) < mouseMoveTolerance)
			{
				// if we did not select a new loop range but just clicked
				int curSamplePosition = (int)(previousStartZoomSamplePosition + samplesPerPixel * mouseDownPoint.X);

				if (PointInLoopRegion(curSamplePosition))
				{
					soundPlayer.ChannelPosition = SamplePositionToSeconds(curSamplePosition, soundPlayer.WaveformData.Length, soundPlayer.ChannelLength);
				}
				else
				{
					soundPlayer.SelectionBegin = TimeSpan.Zero;
					soundPlayer.SelectionEnd = TimeSpan.Zero;
					soundPlayer.ChannelPosition = SamplePositionToSeconds(curSamplePosition, soundPlayer.WaveformData.Length, soundPlayer.ChannelLength);
					
					startLoopSamplePosition = -1;
					endLoopSamplePosition = -1;
					
					doUpdateLoopRegion = true;
				}
			} else {
				startLoopSamplePosition = Math.Max((int)(previousStartZoomSamplePosition + samplesPerPixel * startSelectXPosition), 0);
				endLoopSamplePosition = Math.Min((int)(previousStartZoomSamplePosition + samplesPerPixel * endSelectXPosition), soundPlayer.WaveformData.Length);

				soundPlayer.SelectionBegin = TimeSpan.FromSeconds(SamplePositionToSeconds(startLoopSamplePosition, soundPlayer.WaveformData.Length, soundPlayer.ChannelLength));
				soundPlayer.SelectionEnd = TimeSpan.FromSeconds(SamplePositionToSeconds(endLoopSamplePosition, soundPlayer.WaveformData.Length, soundPlayer.ChannelLength));
				soundPlayer.ChannelPosition = SamplePositionToSeconds(startLoopSamplePosition, soundPlayer.WaveformData.Length, soundPlayer.ChannelLength);
			}
			
			if (doUpdateLoopRegion) {
				startSelectXPosition = 0;
				endSelectXPosition = 0;
				UpdateSelectRegion();
			}
		}
		
		private static double SamplePositionToSeconds(int samplePosition, int totalSamples, double totalDurationSeconds) {
			double positionPercent = (double) samplePosition / (double) totalSamples;
			double position = (totalDurationSeconds * positionPercent);
			return Math.Min(totalDurationSeconds, Math.Max(0, position));
		}

		private static int SecondsToSamplePosition(double channelPositionSeconds, double totalDurationSeconds, int totalSamples) {
			double progressPercent = channelPositionSeconds / totalDurationSeconds;
			int position = (int) (totalSamples * progressPercent);
			return Math.Min(totalSamples, Math.Max(0, position));
		}
		
		private bool PointInLoopRegion(int curSamplePosition) {
			if (soundPlayer.ChannelLength == 0)
				return false;

			double loopStartSamples = (soundPlayer.SelectionBegin.TotalSeconds / soundPlayer.ChannelLength) * soundPlayer.WaveformData.Length;
			double loopEndSamples = (soundPlayer.SelectionEnd.TotalSeconds / soundPlayer.ChannelLength) * soundPlayer.WaveformData.Length;
			
			return (curSamplePosition >= loopStartSamples && curSamplePosition < loopEndSamples);
		}
		
		private void UpdateSelectRegion()
		{
			selectRegion.Width = endSelectXPosition - startSelectXPosition;
			selectRegion.Height = this.Height;
			
			// force redraw
			this.Invalidate();
		}
	}
}