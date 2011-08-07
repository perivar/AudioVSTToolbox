///
/// <summary> * org.hermit.android.instrument: graphical instruments for Android.
/// * <br>Copyright 2009 Ian Cameron Smith
/// * 
/// * <p>These classes provide input and display functions for creating on-screen
/// * instruments of various kinds in Android apps.
/// *
/// * <p>This program is free software; you can redistribute it and/or modify
/// * it under the terms of the GNU General Public License version 2
/// * as published by the Free Software Foundation (see COPYING).
/// * 
/// * <p>This program is distributed in the hope that it will be useful,
/// * but WITHOUT ANY WARRANTY; without even the implied warranty of
/// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
/// * GNU General Public License for more details. </summary>
/// 

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;  

namespace Wave2ZebraSynth.HermitGauges
{
	///
	/// <summary> * A graphical display which displays the audio spectrum from an
	/// * <seealso cref="AudioAnalyser"/> instrument. This class cannot be instantiated
	/// * directly; get an instance by calling
	/// * <seealso cref="AudioAnalyser#getSpectrumGauge()"/>. </summary>
	/// 
	public class SpectrumGauge : Gauge
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Create a SpectrumGauge. This constructor is package-local, as
		///	 * public users get these from an <seealso cref="AudioAnalyser"/> instrument.
		///	 * 
		///	 * @param	parent		Parent surface. </summary>
		/// * <param name="rate"> The input sample rate, in samples/sec. </param>
		///
		internal SpectrumGauge(int rate) : base()
		{
			nyquistFreq = rate / 2;
		}


		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		/// <summary> * Set the sample rate for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The desired rate, in samples/sec. </param>
		///
		public virtual int SampleRate
		{
			set
			{
				nyquistFreq = value / 2;
				
				// If we have a size, then we have a background. Re-draw it
				// to show the new frequency scale.
				if (haveBounds())
				{
					drawBg(bgCanvas, DrawPen);
				}
			}
		}


		///
		/// <summary> * Set the size for the label text.
		/// *  </summary>
		/// * <param name="size"> Label text size for the gauge. </param>
		///
		public virtual float LabelSize
		{
			set
			{
				labelSize = value;
			}
			get
			{
				return labelSize;
			}
		}


		///
		/// <summary> * Get the size for the label text.
		/// *  </summary>
		/// * <returns>  Label text size for the gauge. </returns>
		///


		// ******************************************************************** //
		// Geometry.
		// ******************************************************************** //

		///
		/// <summary> * This is called during layout when the size of this element has
		/// * changed. This is where we first discover our size, so set
		/// * our geometry to match.
		/// * 
		///	 * @param	bounds		The bounding rect of this element within
		///	 * 						its parent View. </summary>
		///
		public override Rectangle Geometry
		{
			set
			{
				base.Geometry = value;
				
				dispX = value.Left;
				dispY = value.Top;
				dispWidth = value.Width;
				dispHeight = value.Height;
				
				// Do some layout within the meter.
				int mw = dispWidth;
				int mh = dispHeight;
				if (labelSize == 0f)
				{
					labelSize = mw / 24f;
				}
				
				spectLabY = mh - 4;
				spectGraphMargin = labelSize;
				spectGraphX = spectGraphMargin;
				spectGraphY = 0;
				spectGraphWidth = mw - spectGraphMargin * 2;
				spectGraphHeight = mh - labelSize - 6;
				
				// Create the bitmap for the spectrum display,
				// and the Canvas for drawing into it.
				specBitmap = new Bitmap( dispWidth, dispHeight, PixelFormat.Format32bppArgb );
				specCanvas = Graphics.FromImage(specBitmap);
				
				// Create the bitmap for the background,
				// and the Canvas for drawing into it.
				bgBitmap = new Bitmap( dispWidth, dispHeight, PixelFormat.Format32bppArgb );
				bgCanvas = Graphics.FromImage(bgBitmap);
				
				drawBg(bgCanvas, DrawPen);
			}
		}


		// ******************************************************************** //
		// Background Drawing.
		// ******************************************************************** //

		///
		/// <summary> * Do the subclass-specific parts of drawing the background
		/// * for this element. Subclasses should override
		/// * this if they have significant background content which they would
		/// * like to draw once only. Whatever is drawn here will be saved in
		/// * a bitmap, which will be rendered to the screen before the
		/// * dynamic content is drawn.
		/// * 
		/// * <p>Obviously, if implementing this method, don't clear the screen when
		/// * drawing the dynamic part.
		/// *  </summary>
		/// * <param name="graphics"> Canvas to draw into. </param>
		/// * <param name="pen">  The Graphics which was set up in initializePen(). </param>
		///
		private void drawBg(Graphics graphics, Pen pen)
		{
			SolidBrush drawBrush = new SolidBrush(AColor.UIntToColor(0xff000000));
			graphics.FillRectangle(drawBrush, Bounds);
			pen.Color = AColor.UIntToColor(0xffffff00);
			pen.DashStyle = DashStyle.Dash;

			// Draw the grid.
			float sx = 0 + spectGraphX;
			float sy = 0 + spectGraphY;
			float ex = sx + spectGraphWidth - 1;
			float ey = sy + spectGraphHeight - 1;
			float bw = spectGraphWidth - 1;
			float bh = spectGraphHeight - 1;
			graphics.DrawRectangle(pen, sx, sy, ex, ey);
			for (int i = 1; i < 10; ++i)
			{
				float x = (float) i * (float) bw / 10f;
				graphics.DrawLine(pen, sx + x, sy, sx + x, sy + bh);
			}
			for (int i = 1; i < RANGE_BELS; ++i)
			{
				float y = (float) i * (float) bh / RANGE_BELS;
				graphics.DrawLine(pen, sx, sy + y, sx + bw, sy + y);
			}

			// Draw the labels below the grid.
			float ly = 0 + spectLabY;
			//pen.TextSize = labelSize;
			int step = measureText(graphics, "8.8k") > bw / 10f - 1 ? 2 : 1;
			for (int i = 0; i <= 10; i += step)
			{
				int f = nyquistFreq * i / 10;
				string text = f >= 10000 ? "" + (f / 1000) + "k" : f >= 1000 ? "" + (f / 1000) + "." + (f / 100 % 10) + "k" : "" + f;
				float tw = measureText(graphics, text);
				float lx = sx + i * (float) bw / 10f + 1 - (tw / 2);
				graphics.DrawString(text, TextFont, TextBrush, lx, ly);
			}
		}


		// ******************************************************************** //
		// Data Updates.
		// ******************************************************************** //

		///
		///	 <summary> * New data from the instrument has arrived. This method is called
		///	 * on the thread of the instrument.
		///	 *  </summary>
		/// * <param name="data"> An array of floats defining the signal power
		/// *                      at each frequency in the spectrum. </param>
		///
		internal void Update(float[] data)
		{
			Graphics graphics = specCanvas;
			Pen pen = DrawPen;

			// Now actually do the drawing.
			lock (this)
			{
				graphics.DrawImage((Image)bgBitmap, 0, 0);
				if (logFreqScale)
				{
					logGraph(data, graphics, pen);
				}
				else
				{
					linearGraph(data, graphics, pen);
				}
			}
		}


		///
		/// <summary> * Draw a linear spectrum graph.
		/// *  </summary>
		/// * <param name="data"> An array of floats defining the signal power
		/// *                      at each frequency in the spectrum. </param>
		/// * <param name="graphics">  Canvas to draw into. </param>
		/// * <param name="pen"> Graphics to draw with. </param>
		///
		private void logGraph(float[] data, Graphics graphics, Pen pen)
		{
			//pen.Style = Graphics.Style.FILL;
			paintColor[1] = 1f;
			paintColor[2] = 1f;
			int len = data.Length;
			float bw = (float)(spectGraphWidth - 2) / (float) len;
			float bh = spectGraphHeight - 2;
			float be = spectGraphY + spectGraphHeight - 1;

			// Determine the first and last frequencies we have.
			float lf = nyquistFreq / len;
			float rf = nyquistFreq;

			// Now, how many octaves is that. Round down. Calculate pixels/oct.
			int octaves = (int) Math.Floor(log2(rf / lf)) - 2;
			float octWidth = (float)(spectGraphWidth - 2) / (float) octaves;

			// Calculate the base frequency for the graph, which isn't lf.
			float bf = rf / (float) Math.Pow(2, octaves);

			// Element 0 isn't a frequency bucket; skip it.
			for (int i = 1; i < len; ++i)
			{
				// Cycle the hue angle from 0° to 300°; i.e. red to purple.
				paintColor[0] = (float) i / (float) len * 300f;
				pen.Color = AColor.HSVToColor(paintColor);

				// What frequency bucket are we in.
				float f = lf * i;

				// For freq f, calculate x.
				float x = spectGraphX + (float)(log2(f) - log2(bf)) * octWidth;

				// Draw the bar.
				float y = be - (float)(Math.Log10(data[i]) / RANGE_BELS + 1f) * bh;
				if (y > be)
				{
					y = be;
				}
				else if (y < spectGraphY)
				{
					y = spectGraphY;
				}
				if (bw <= 1.0f)
				{
					graphics.DrawLine(pen, x, y, x, be);
				}
				else
				{
					graphics.DrawRectangle(pen, x, y, x + bw, be);
				}
			}
		}


		private double log2(double x)
		{
			return Math.Log(x) / LOG2;
		}


		///
		///	 <summary> * Draw a linear spectrum graph.
		///	 *  </summary>
		/// * <param name="data"> An array of floats defining the signal power
		/// *                      at each frequency in the spectrum. </param>
		///	 * <param name="graphics">  Canvas to draw into. </param>
		///	 * <param name="pen"> Graphics to draw with. </param>
		///
		private void linearGraph(float[] data, Graphics graphics, Pen pen)
		{
			//pen.Style = Graphics.Style.FILL;
			paintColor[1] = 1f;
			paintColor[2] = 1f;
			int len = data.Length;
			float bw = (float)(spectGraphWidth - 2) / (float) len;
			float bh = spectGraphHeight - 2;
			float be = spectGraphY + spectGraphHeight - 1;

			// Element 0 isn't a frequency bucket; skip it.
			for (int i = 1; i < len; ++i)
			{
				// Cycle the hue angle from 0° to 300°; i.e. red to purple.
				paintColor[0] = (float) i / (float) len * 300f;
				pen.Color = AColor.HSVToColor(paintColor);

				// Draw the bar.
				float x = spectGraphX + i * bw + 1;
				float y = be - (float)(Math.Log10(data[i]) / RANGE_BELS + 1f) * bh;
				if (y > be)
				{
					y = be;
				}
				else if (y < spectGraphY)
				{
					y = spectGraphY;
				}
				if (bw <= 1.0f)
				{
					graphics.DrawLine(pen, x, y, x, be);
				}
				else
				{
					graphics.DrawRectangle(pen, x, y, x + bw, be);
				}
			}
		}


		// ******************************************************************** //
		// View Drawing.
		// ******************************************************************** //

		///
		///	 <summary> * Do the subclass-specific parts of drawing for this element.
		///	 * This method is called on the thread of the containing SuraceView.
		///	 * 
		///	 * <p>Subclasses should override this to do their drawing.
		///	 * 
		///	 * @param	graphics		Canvas to draw into.
		///	 * @param	pen		The Graphics which was set up in initializePen(). </summary>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		///
		protected internal override sealed void drawBody(Graphics graphics, Pen pen, long now)
		{
			// Since drawBody may be called more often than we get audio
			// data, it makes sense to just draw the buffered image here.
			lock (this)
			{
				graphics.DrawImage((Image)specBitmap, dispX, dispY);
			}
		}


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings("unused") private static final String TAG = "instrument";
		private const string TAG = "instrument";

		// Log of 2.
		private static double LOG2 = Math.Log(2);

		// Vertical range of the graph in bels.
		private const float RANGE_BELS = 6f;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// The Nyquist frequency -- the highest frequency
		// represented in the spectrum data we will be plotting.
		private int nyquistFreq = 0;

		// If true, draw a logarithmic frequency scale. Otherwise linear.
		private bool logFreqScale = false;

		// Display position and size within the parent view.
		private int dispX = 0;
		private int dispY = 0;
		private int dispWidth = 0;
		private int dispHeight = 0;

		// Label text size for the gauge. Zero means not set yet.
		private float labelSize = 0f;

		// Layout parameters for the VU meter. Position and size for the
		// bar itself; position and size for the bar labels; position
		// and size for the main readout text.
		private float spectGraphX = 0;
		private float spectGraphY = 0;
		private float spectGraphWidth = 0;
		private float spectGraphHeight = 0;
		private float spectLabY = 0;
		private float spectGraphMargin = 0;

		// Bitmap in which we draw the gauge background,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap bgBitmap = null;
		private Graphics bgCanvas = null;

		// Bitmap in which we draw the audio spectrum display,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap specBitmap = null;
		private Graphics specCanvas = null;

		// Buffer for calculating the draw colour from H,S,V values.
		// HSV stands for hue, saturation, and value, and is also often called HSB (B for brightness).
		private float[] paintColor = { 0, 1, 1 };

	}


}