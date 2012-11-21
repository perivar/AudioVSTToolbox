using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace com.badlogic.audio.visualization
{
	/**
	 * A simple class that allows to plot float[] arrays
	 * to a swing window. The first function to plot that
	 * is given to this class will set the minimum and
	 * maximum height values. I'm not that good with Swing
	 * so i might have done a couple of stupid things in here :)
	 * 
	 * @author mzechner
	 */
	public class Plot
	{
		/** the frame **/
		Size frame = new Size(600, 400);
		
		/** the image **/
		private Bitmap image;
		
		/** the last scaling factor to normalize samples **/
		private float scalingFactor = 1;

		/** wheter the plot was cleared, if true we have to recalculate the scaling factor **/
		private bool cleared = true;

		/** current marker position and color **/
		private int markerPosition = 0;
		private Color markerColor = Color.White;
		
		/**
		 * Creates a new Plot with the given title and dimensions.
		 * 
		 * @param title The title.
		 * @param width The width of the plot in pixels.
		 * @param height The height of the plot in pixels.
		 */
		public Plot(string title, int width, int height)
		{
			image = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(Brushes.Black, 0, 0, width, height);

			// Plot the marker
			g.DrawLine(new Pen(markerColor), markerPosition, 0, markerPosition, image.Height);

			g.Dispose();
		}
		
		public void clear( )
		{
			Graphics g = Graphics.FromImage(image);
			g.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);
			g.Dispose();
			cleared = true;
		}

		public void plot(float[] samples, float samplesPerPixel, Color color)
		{
			Graphics g;
			if( image.Width <  samples.Length / samplesPerPixel )
			{
				image = new Bitmap((int)(samples.Length / samplesPerPixel), frame.Height, PixelFormat.Format32bppRgb);
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
			for(int i = 1; i < samples.Length; i++)
			{
				float @value = (samples[i] / scalingFactor) * image.Height / 3 + image.Height / 2;
				g.DrawLine(new Pen(color), (int)((i-1) / samplesPerPixel), image.Height - (int)lastValue, (int)(i / samplesPerPixel), image.Height - (int)@value);
				lastValue = @value;
			}
			g.Dispose();
		}

		public void plot(List<float> samples, float samplesPerPixel, Color color)
		{
			Graphics g;
			if(image.Width < samples.Count / samplesPerPixel)
			{
				image = new Bitmap((int)(samples.Count / samplesPerPixel), frame.Height, PixelFormat.Format32bppRgb);
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

		public void plot(float[] samples, float samplesPerPixel, float offset, bool useLastScale, Color color)
		{
			Graphics g;
			if(image.Width < samples.Length / samplesPerPixel)
			{
				image = new Bitmap((int)(samples.Length / samplesPerPixel), frame.Height, PixelFormat.Format32bppRgb);
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

		public void plot(List<float> samples, float samplesPerPixel, float offset, bool useLastScale, Color color)
		{
			Graphics g;
			if(image.Width < samples.Count / samplesPerPixel)
			{
				image = new Bitmap((int)(samples.Count / samplesPerPixel), frame.Height, PixelFormat.Format32bppRgb);
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

		public void SetMarker(int x, Color color)
		{
			this.markerPosition = x;
			this.markerColor = color;
		}
	}
}