using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using System.Windows.Forms;

using System.Threading;
using System.Timers;

namespace com.badlogic.audio.visualization
{
	/**
	 * A simple class that allows to plot float[] arrays
	 * to a window. The first function to plot that
	 * is given to this class will set the minimum and
	 * maximum height values.
	 * 
	 * @author mzechner
	 * @author perivar@nerseth.com
	 */
	public class Plot : Form
	{
		/** the frame **/
		private Form form;
		
		/** the image **/
		private Bitmap image;
		
		/** the last scaling factor to normalize samples **/
		private float scalingFactor = 1;

		/** wheter the plot was cleared, if true we have to recalculate the scaling factor **/
		private bool cleared = true;

		/** current marker position and color **/
		private int markerPosition = -1;
		private Color markerColor = Color.White;
		
		public Plot()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// Plot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(512, 512);
			this.DoubleBuffered = true;
			this.Name = "Plot";
			this.Text = "Plot";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Plot_Paint);
			this.ResumeLayout(false);
		}
		
		/**
		 * Creates a new Plot with the given title and dimensions.
		 * 
		 * @param title The title.
		 * @param width The width of the plot in pixels.
		 * @param height The height of the plot in pixels.
		 */
		public Plot(string title, int width, int height) : this()
		{
			// initialize image
			image = new Bitmap(width, height, PixelFormat.Format32bppRgb);

			form = this;
			form.Text = title;
			form.Size = new Size(width, height);
			form.ClientSize = form.Size;
			form.AutoScrollMinSize = image.Size;
			
			System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
			myTimer.Tick += new EventHandler(TimerEventProcessor);
			myTimer.Interval = 100;
			myTimer.Start();
		}
		
		public void Plot_Paint(object sender, PaintEventArgs e)
		{
			Redraw();
		}
		
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// do nothing
		}
		
		// for System.Windows.Forms.Timer
		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs) {
			//markerPosition += 10; // testing marker animation
			Redraw();
		}
		
		private void Redraw() {
			lock (image) {
				Graphics g = form.CreateGraphics();
				g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
				g.SmoothingMode = SmoothingMode.HighQuality;
				
				g.DrawImageUnscaled(image, 0, 0);
				
				// Plot the marker
				if (markerPosition >= 0) {
					g.DrawLine(new Pen(markerColor), markerPosition, 0, markerPosition, image.Height);
				}
				g.Dispose();
			}
		}
		
		public void Clear( )
		{
			lock (image) {
				Graphics g = Graphics.FromImage(image);
				g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
				g.Dispose();
				cleared = true;
			}
		}

		public void plot(float[] samples, float samplesPerPixel, Color color)
		{
			lock (image) {
				Graphics g;
				if( image.Width <  samples.Length / samplesPerPixel )
				{
					image = new Bitmap((int)(samples.Length / samplesPerPixel), form.Height, PixelFormat.Format32bppRgb);
					form.AutoScrollMinSize = image.Size;
					g = Graphics.FromImage(image);
					g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
					g.Dispose();
				}
				
				if(cleared)
				{
					float min = 0;
					float max = 0;
					for(int i = 0; i < samples.Length; i++)
					{
						min = Math.Min(samples[i], min);
						max = Math.Max(samples[i], max);
					}
					scalingFactor = max - min;
					cleared = false;
				}

				g = Graphics.FromImage(image);
				float lastValue = (samples[0] / scalingFactor) * image.Height / 3 + image.Height / 2;
				int lastx1 = -1;
				int lastx2 = -1;
				for(int i = 1; i < samples.Length; i++)
				{
					float @value = (samples[i] / scalingFactor) * image.Height / 3 + image.Height / 2;
					
					int x1 = (int)((i-1) / samplesPerPixel);
					int y1 = image.Height - (int)lastValue;
					int x2 = (int)(i / samplesPerPixel);
					int y2 = image.Height - (int)@value;
					
					//if (x1 == lastx1) continue;
					
					//g.DrawLine(new Pen(color), (int)((i-1) / samplesPerPixel), image.Height - (int)lastValue, (int)(i / samplesPerPixel), image.Height - (int)@value);
					g.DrawLine(new Pen(color), x1, y1, x2, y2);
					lastValue = @value;
					lastx1 = x1;
					lastx2 = x2;
				}
				g.Dispose();
			}
		}

		public void plot(List<float> samples, float samplesPerPixel, Color color)
		{
			lock (image) {
				Graphics g;
				if(image.Width < samples.Count / samplesPerPixel)
				{
					image = new Bitmap((int)(samples.Count / samplesPerPixel), form.Height, PixelFormat.Format32bppRgb);
					form.AutoScrollMinSize = image.Size;
					g = Graphics.FromImage(image);
					g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
					g.Dispose();
				}

				if(cleared)
				{
					float min = 0;
					float max = 0;
					for(int i = 0; i < samples.Count; i++)
					{
						min = Math.Min(samples[i], min);
						max = Math.Max(samples[i], max);
					}
					scalingFactor = max - min;
					cleared = false;
				}

				g = Graphics.FromImage(image);
				float lastValue = (samples[0] / scalingFactor) * image.Height / 3 + image.Height / 2;
				for(int i = 1; i < samples.Count; i++)
				{
					float @value = (samples[i] / scalingFactor) * image.Height / 3 + image.Height / 2;
					g.DrawLine(new Pen(color), (int)((i-1) / samplesPerPixel), image.Height - (int)lastValue, (int)(i / samplesPerPixel), image.Height - (int)@value);
					lastValue = @value;
				}
				g.Dispose();
			}
		}

		public void plot(float[] samples, float samplesPerPixel, float offset, bool useLastScale, Color color)
		{
			lock (image) {
				Graphics g;
				if(image.Width < samples.Length / samplesPerPixel)
				{
					image = new Bitmap((int)(samples.Length / samplesPerPixel), form.Height, PixelFormat.Format32bppRgb);
					form.AutoScrollMinSize = image.Size;
					g = Graphics.FromImage(image);
					g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
					g.Dispose();
				}

				if(!useLastScale)
				{
					float min = 0;
					float max = 0;
					for(int i = 0; i < samples.Length; i++)
					{
						min = Math.Min(samples[i], min);
						max = Math.Max(samples[i], max);
					}
					scalingFactor = max - min;
				}

				g = Graphics.FromImage(image);
				float lastValue = (samples[0] / scalingFactor) * image.Height / 3 + image.Height / 2 - offset * image.Height / 3;
				for(int i = 1; i < samples.Length; i++)
				{
					float @value = (samples[i] / scalingFactor) * image.Height / 3 + image.Height / 2 - offset * image.Height / 3;
					g.DrawLine(new Pen(color), (int)((i-1) / samplesPerPixel), image.Height - (int)lastValue, (int)(i / samplesPerPixel), image.Height - (int)@value);
					lastValue = @value;
				}
				g.Dispose();
			}
		}

		public void plot(List<float> samples, float samplesPerPixel, float offset, bool useLastScale, Color color)
		{
			lock (image) {
				Graphics g;
				if(image.Width < samples.Count / samplesPerPixel)
				{
					image = new Bitmap((int)(samples.Count / samplesPerPixel), form.Height, PixelFormat.Format32bppRgb);
					form.AutoScrollMinSize = image.Size;
					g = Graphics.FromImage(image);
					g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
					g.Dispose();
				}

				if(!useLastScale)
				{
					float min = 0;
					float max = 0;
					for(int i = 0; i < samples.Count; i++)
					{
						min = Math.Min(samples[i], min);
						max = Math.Max(samples[i], max);
					}
					scalingFactor = max - min;
				}

				g = Graphics.FromImage(image);
				float lastValue = (samples[0] / scalingFactor) * image.Height / 3 + image.Height / 2 - offset * image.Height / 3;
				for(int i = 1; i < samples.Count; i++)
				{
					float @value = (samples[i] / scalingFactor) * image.Height / 3 + image.Height / 2 - offset * image.Height / 3;
					g.DrawLine(new Pen(color), (int)((i-1) / samplesPerPixel), image.Height - (int)lastValue, (int)(i / samplesPerPixel), image.Height - (int)@value);
					lastValue = @value;
				}
				g.Dispose();
			}
		}

		public void SetMarker(int x, Color color)
		{
			this.markerPosition = x;
			this.markerColor = color;
		}
	}
}