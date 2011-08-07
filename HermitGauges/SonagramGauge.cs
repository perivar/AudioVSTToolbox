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
	/// <summary> * A graphical display which displays the audio sonarum from an
	/// * <seealso cref="AudioAnalyser"/> instrument. This class cannot be instantiated
	/// * directly; get an instance by calling
	/// * <seealso cref="AudioAnalyser#getSpectrumGauge()"/>. </summary>
	/// 
	public class SonagramGauge : Gauge
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
		internal SonagramGauge(int rate, int inputBlockSize) : base()
		{
			blockSize=inputBlockSize;
			SampleRate = rate;

			//Colors for Sonagram
			for (int i=0;i<50;i++)
			{
				paintColors[i]= Color.FromArgb(0, i, i*5).ToArgb();
			}
			for (int i=50;i<100;i++)
			{
				paintColors[i]= Color.FromArgb(0, i, (100-i)*5).ToArgb();
			}
			for (int i=100;i<150;i++)
			{
				paintColors[i]= Color.FromArgb((i-100)*3,(i-50)*2,0).ToArgb();
			}
			for (int i=150;i<=250;i++)
			{
				paintColors[i]= Color.FromArgb(i, 550-i*2, 0).ToArgb();
			}
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
				period=(float)(1f / value)*blockSize*2;
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
				
				sonaGraphX = 0;
				sonaGraphY = 3;
				sonaGraphWidth = mw - labelSize * 2;
				sonaGraphHeight = mh - labelSize - 6;
				
				// Create the bitmap for the sonagram display,
				// and the Canvas for drawing into it.
				finalBitmap = new Bitmap( dispWidth, dispHeight, PixelFormat.Format32bppArgb );
				finalCanvas = Graphics.FromImage(finalBitmap);
				
				// Create the bitmap for the sonagram display,
				// and the Canvas for drawing into it.
				sonaBitmap = new Bitmap( (int) sonaGraphWidth, (int) sonaGraphHeight);
				sonaCanvas = Graphics.FromImage(sonaBitmap);
				
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
		/// <summary> * Do the subclass-sonaific parts of drawing the background
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
			float lx;
			float ly;
			SolidBrush drawBrush = new SolidBrush(AColor.UIntToColor(0xff000000));
			graphics.FillRectangle(drawBrush, Bounds);

			pen.Color = AColor.UIntToColor(0xffffff00);
			pen.DashStyle = DashStyle.Dash;

			// Draw the grid.
			float sx = sonaGraphX;
			float sy = sonaGraphY;
			float bw = sonaGraphWidth - 1;
			float bh = sonaGraphHeight - 1;

			// Draw freq.
			lx = sx + bw + 1;
			for (int i = 0; i <= 10; i += 1)
			{
				int f = nyquistFreq * i / 10;
				string text = f >= 10000 ? "" + (f / 1000) + "k" : f >= 1000 ? "" + (f / 1000) + "." + (f / 100 % 10) + "k" : "" + f;
				ly = sy + bh - i * (float) bh / 10f + 1;
				graphics.DrawString(text, TextFont, TextBrush, lx + 7, ly + labelSize/3);			
				graphics.DrawLine(pen, lx, ly, lx+3, ly);
			}

			// Draw time.
			ly = sy + bh + labelSize + 1;
			float totaltime =(float)Math.Floor(bw*period);
			for (int i = 0; i <= 9; i += 1)
			{
				float time = totaltime * i / 10;
				string text = "" + time + "s";
				float tw = measureText(graphics, text);
				lx = sx + i * (float) bw / 10f + 1;
				graphics.DrawString(text, TextFont, TextBrush, lx - (tw / 2), ly);
				graphics.DrawLine(pen, lx, sy + bh-1, lx, sy + bh + 2);
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
		/// *                      at each frequency in the sonagram. </param>
		///
		internal void Update(float[] data)
		{
			Graphics graphics = finalCanvas;
			Pen pen = DrawPen;

			// Now actually do the drawing.
			lock (this)
			{
				//Background
				graphics.DrawImage((Image)bgBitmap, 0, 0);
				
				//Scroll
				sonaCanvas.DrawImage((Image)sonaBitmap, 1, 0);

				//Add Current Data
				linearGraph(data, sonaCanvas, pen);
				graphics.DrawImage((Image)sonaBitmap, sonaGraphX, sonaGraphY);
			}
		}

		///
		///	 <summary> * Draw a linear sonagram graph.
		///	 *  </summary>
		/// * <param name="data"> An array of floats defining the signal power
		/// *                      at each frequency in the sonagram. </param>
		///	 * <param name="graphics">  Canvas to draw into. </param>
		///	 * <param name="pen"> Graphics to draw with. </param>
		///
		private void linearGraph(float[] data, Graphics graphics, Pen pen)
		{
			//pen.Style = Graphics.Style.FILL;
			int len = data.Length;
			float bh = (float) sonaGraphHeight / (float) len;

			// Element 0 isn't a frequency bucket; skip it.
			for (int i = 1; i < len; ++i)
			{
				// Draw the new line.
				float y = sonaGraphHeight- i * bh + 1;

				// Cycle the hue angle from 0° to 300°; i.e. red to purple.
				float v = (float)(Math.Log10(data[i]) / RANGE_BELS + 2f);
				int colorIndex =(int)(v*maxColors);
				if (colorIndex<0)
				{
					colorIndex=0;
				}
				if (colorIndex>maxColors)
				{
					colorIndex=maxColors;
				}
				pen.Color = Color.FromArgb(paintColors[colorIndex]);

				if (bh <= 1.0f)
				{
					graphics.DrawEllipse(pen, 0, y, 1, 1);
				}
				else
				{
					graphics.DrawLine(pen, 0, y, 0, y - bh);
				}
			}
		}


		// ******************************************************************** //
		// View Drawing.
		// ******************************************************************** //

		///
		///	 <summary> * Do the subclass-sonaific parts of drawing for this element.
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
				graphics.DrawImage((Image)finalBitmap, dispX, dispY);
			}
		}


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const string TAG = "instrument";

		// Vertical range of the graph in bels.
		private const float RANGE_BELS = 2f;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// The Nyquist frequency -- the highest frequency
		// represented in the sonarum data we will be plotting.
		private int nyquistFreq = 0;
		private int blockSize = 0;
		private float period = 0;

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
		private float sonaGraphX = 0;
		private float sonaGraphY = 0;
		private float sonaGraphWidth = 0;
		private float sonaGraphHeight = 0;

		// Bitmap in which we draw the gauge background,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap bgBitmap = null;
		private Graphics bgCanvas = null;

		// Bitmap in which we draw the audio sonagram display,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap sonaBitmap = null;
		private Graphics sonaCanvas = null;

		// Bitmap in which we draw the audio sonagram display,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap finalBitmap = null;
		private Graphics finalCanvas = null;


		// Buffer for calculating the draw colour from H,S,V values.
		private readonly int[] paintColors = new int[251];
		private readonly int maxColors =250;

	}

}