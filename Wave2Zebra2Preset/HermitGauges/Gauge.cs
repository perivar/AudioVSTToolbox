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

namespace Wave2Zebra2Preset.HermitGauges
{
	///
	/// <summary> * A graphical display which shows some data in a region
	/// * within a view. The data may come from an <seealso cref="Instrument"/> or
	/// * some other source.
	/// * 
	/// * <h2>Configuration</h2>
	/// * 
	/// * <p>Your gauge will be notified of its geometry by a call to
	/// * <seealso cref="#setGeometry(Rectangle)"/>. This is where you should note your position
	/// * and size and perform any internal layout you need to do.
	/// * 
	/// * <p>Note that if you are running in an app which handles screen
	/// * configuration changes, <seealso cref="#setGeometry(Rectangle)"/> will be called any
	/// * time the screen changes size or shape (e.g. on an orientation change).
	/// * You should be prepared to handle these subsequent calls by re-creating
	/// * data structures, re-doing layout, etc., as required.
	/// * 
	/// * <h2>Data Updating</h2>
	/// * 
	/// * <p>It is assumed that your gauge has some kind of data source, but
	/// * how this works is up to you.
	/// * 
	/// * <h2>Drawing Sequence -- User</h2>
	/// * 
	/// * <p>A gauge may have a background which is rendered separately from
	/// * its content, for performance reasons. Hence, a Gauge user must request
	/// * the background to be drawn, and then the gauge content to be drawn. If
	/// * the caller is going to cache the background, the background need be
	/// * requested only when the geometry changes.
	/// * 
	/// * <p>There are two options. In the non-caching case, the caller may
	/// * simply call <seealso cref="#draw(Canvas, long, boolean)"/>, passing true
	/// * as the last argument. This asks the Gauge to draw its background and
	/// * its content.
	/// * 
	/// * <p>In the caching case, the caller should call
	/// * <seealso cref="#drawBackground(Canvas)"/> to ask the gauge to draw its
	/// * background into the given graphics. Since the gauge will use the same
	/// * co-ordinates that it uses to draw to the screen, the graphics will need
	/// * to be the size of the screen (or you can translate the co-ordinates).
	/// * Then, to draw the gauge, the caller should render the stored background
	/// * and then call <seealso cref="#draw(Canvas, long, boolean)"/>.
	/// * 
	/// * <h2>Drawing Sequence -- Implementor</h2>
	/// * 
	/// * <p>From the Gauge implementor's point of view, there are two routines
	/// * to implement: <seealso cref="#drawBackgroundBody(Canvas, Graphics)"/> (optional), and
	/// * <seealso cref="#drawBody(Canvas, Graphics, long)"/>.
	/// * 
	/// * <p>If your implementation of <seealso cref="#drawBody(Canvas, Graphics, long)"/>
	/// * draws a complete, opaque rendition of the gauge, that's all you need;
	/// * there's no need to provide an implementation of drawBackgroundBody().
	/// * But if your gauge has a separate, persistent background appearance,
	/// * you may reap a performance benefit by separating out its drawing.
	/// * Do this by implementing <seealso cref="#drawBackgroundBody(Canvas, Graphics)"/>.
	/// * This routine should draw the gauge background at the gauge's
	/// * configured position in the specified Canvas.
	/// * 
	/// * <p>A facility is provided for caching background images. To use this,
	/// * call <seealso cref="#cacheBackground()"/> once your layout is set up -- for example
	/// * at the end of <seealso cref="#setGeometry(Rectangle)"/>. At that point, your background
	/// * will be fetched (by calling your implementation of drawBackgroundBody())
	/// * and stored; then when someone asks us to draw our background, the request
	/// * will be satisfied using the stored bitmap, without calling your
	/// * drawBackgroundBody() again. </summary>
	/// 
	public class Gauge
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Set up this view.
		///	 *  </summary>
		///
		public Gauge()
		{
		}


		///
		/// <summary> * Set up this view.
		/// *  </summary>
		/// * <param name="parent">   Parent surface. </param>
		/// * <param name="options">  Options for this . A bitwise OR of
		/// *                          GAUGE_XXX constants. </param>
		///
		public Gauge(int options)
		{
			
			gaugeOptions = options;
			
			initializePen(drawPen);
		}


		///
		///	 <summary> * Set up this view.
		///	 *  </summary>
		/// * @param	grid			Colour for drawing a data scale / grid.
		/// * @param	plot			Colour for drawing data plots. </param>
		///
		public Gauge(int grid, int plot)
		{
		}


		///
		/// <summary> * Set up this view.
		/// *  </summary>
		/// * <param name="options">  Options for this . A bitwise OR of
		/// *                          GAUGE_XXX constants. </param>
		/// * <param name="grid"> Colour for drawing a data scale / grid. </param>
		/// * <param name="plot"> Colour for drawing data plots. </param>
		///
		public Gauge(int options, uint grid, uint plot)
		{
			
			gaugeOptions = options;
			gridColour = grid;
			plotColour = plot;

			initializePen(drawPen);
		}


		///
		///	 <summary> * Set up the pen for this element. This is called during
		///	 * initialization. Subclasses can override this to do class-specific
		///	 * one-time initialization.
		///	 *  </summary>
		///	 * <param name="pen">			The pen to initialize. </param>
		///
		protected internal virtual void initializePen(Pen pen)
		{
		}


		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		/// <summary> * Check whether the given option flag is set on this surface.
		/// *  </summary>
		/// * <param name="option"> The option flag to test; one of GAUGE_XXX. </param>
		/// * <returns>  true iff the option is set. </returns>
		///
		public virtual bool optionSet(int option)
		{
			return (gaugeOptions & option) != 0;
		}


		// ******************************************************************** //
		// Global Layout Parameters.
		// ******************************************************************** //

		public Pen DrawPen
		{
			set
			{
				drawPen = value;
			}
			get
			{
				return drawPen;
			}
		}

		
		public Brush TextBrush
		{
			set
			{
				drawBrush = value;
			}
			get
			{
				return drawBrush;
			}
		}

		public Graphics Canvas
		{
			set
			{
				backgroundCanvas = value;
			}
			get
			{
				return backgroundCanvas;
			}
		}
		
		///
		/// <summary> * Set the default font for all text.
		/// *  </summary>
		/// * <param name="face"> The default font for all text. </param>
		///
		public static Font TextFont
		{
			set
			{
				baseTextFace = value;
			}
			get
			{
				return baseTextFace;
			}
		}


		///
		/// <summary> * Get the default font for all text.
		/// *  </summary>
		/// * <returns>  The default font for all text. </returns>
		///


		///
		/// <summary> * Set the base size for text.
		/// *  </summary>
		/// * <param name="size"> Base text size for the app. </param>
		///
		public static float BaseTextSize
		{
			set
			{
				baseTextSize = value;
				headTextSize = baseTextSize * 1.3f;
				miniTextSize = baseTextSize * 0.9f;
				tinyTextSize = baseTextSize * 0.8f;
			}
			get
			{
				return baseTextSize;
			}
		}


		///
		/// <summary> * Get the base size for text.
		/// *  </summary>
		/// * <returns>  Base text size for the app. </returns>
		///


		///
		/// <summary> * Set the size for header text.
		/// *  </summary>
		/// * <param name="size"> Header text size for the app. </param>
		///
		public static float HeadTextSize
		{
			set
			{
				headTextSize = value;
			}
			get
			{
				return headTextSize;
			}
		}


		///
		/// <summary> * Get the size for header text.
		/// *  </summary>
		/// * <returns>  Header text size for the app. </returns>
		///


		///
		/// <summary> * Set the size for mini text.
		/// *  </summary>
		/// * <param name="size"> Mini text size for the app. </param>
		///
		public static float MiniTextSize
		{
			set
			{
				miniTextSize = value;
			}
			get
			{
				return miniTextSize;
			}
		}


		///
		/// <summary> * Get the size for mini text.
		/// *  </summary>
		/// * <returns>  Mini text size for the app. </returns>
		///


		///
		/// <summary> * Set the size for tiny text.
		/// *  </summary>
		/// * <param name="size"> Tiny text size for the app. </param>
		///
		public static float TinyTextSize
		{
			set
			{
				tinyTextSize = value;
			}
			get
			{
				return tinyTextSize;
			}
		}


		///
		/// <summary> * Get the size for tiny text based on this screen's size.
		/// *  </summary>
		/// * <returns>  Tiny text size for the app. </returns>
		///


		///
		/// <summary> * Set the horizontal scaling of the font; this can be used to
		/// * produce a tall, thin font.
		/// *  </summary>
		/// * <param name="scale">  Horizontal scaling of the font. </param>
		///
		public static float TextScaleX
		{
			set
			{
				textScaleX = value;
			}
			get
			{
				return textScaleX;
			}
		}


		///
		/// <summary> * Get the base size for text based on this screen's size.
		/// *  </summary>
		/// * <returns>  Horizontal scaling of the font. </returns>
		///


		///
		/// <summary> * Set the sidebar width.
		/// *  </summary>
		/// * <param name="width">  The sidebar width. </param>
		///
		public static int SidebarWidth
		{
			set
			{
				viewSidebar = value;
			}
			get
			{
				return viewSidebar;
			}
		}


		///
		/// <summary> * Get the sidebar width.
		/// *  </summary>
		/// * <returns>  The sidebar width. </returns>
		///


		///
		/// <summary> * Set the amount of padding between major elements in a view.
		/// *  </summary>
		/// * <param name="pad"> The amount of padding between major elements in a view. </param>
		///
		public static int InterPadding
		{
			set
			{
				interPadding = value;
			}
			get
			{
				return interPadding;
			}
		}


		///
		/// <summary> * Get the amount of padding between major elements in a view.
		/// *  </summary>
		/// * <returns>   The amount of padding between major elements in a view. </returns>
		///


		///
		/// <summary> * Set the amount of padding within atoms within an element. Specifically
		/// * the small gaps in side bars.
		/// *  </summary>
		/// * <param name="gap"> The amount of padding within atoms within an element </param>
		///
		public static int InnerGap
		{
			set
			{
				innerGap = value;
			}
			get
			{
				return innerGap;
			}
		}


		///
		/// <summary> * Get the amount of padding within atoms within an element. Specifically
		/// * the small gaps in side bars.
		/// *  </summary>
		/// * <returns>   The amount of padding within atoms within an element </returns>
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
		public virtual Rectangle Geometry
		{
			set
			{
				elemBounds = value;
				
				// Any cached background we may have is now invalid.
				backgroundBitmap = null;
				backgroundCanvas = null;
			}
		}


		///
		///	 <summary> * Fetch and cache an image of the background now, then use that
		///	 * to draw the background on future draw requests. The cached image
		///	 * is invalidated the next time the geometry changes.
		///	 * 
		///	 * <p>Implementations should call this method once their layout is
		///	 * set -- for example at the end of <seealso cref="#setGeometry(Rectangle)"/> --
		///	 * if they have a significant static background that they wish
		///	 * to have cached. </summary>
		///
		protected internal virtual void cacheBackground()
		{
			// Create the bitmap for the background,
			// and the Canvas for drawing into it.
			backgroundBitmap = new Bitmap( elemBounds.Width, elemBounds.Height, PixelFormat.Format32bppArgb );
			backgroundCanvas = Graphics.FromImage(backgroundBitmap);
			
			// Because the element is going to draw its BG at its proper
			// location, and this bitmap is local to the element, we need
			// to translate the drawing co-ordinates.
			drawBackground(backgroundCanvas, -elemBounds.Left, -elemBounds.Top);
		}


		///
		///	 <summary> * Get the minimum preferred width for this atom.
		///	 * 
		///	 * @return			The minimum preferred width for this atom.
		///	 * 					Returns zero if we don't know yet. </summary>
		///
		public virtual int PreferredWidth
		{
			get
			{
				return 0;
			}
		}


		///
		///	 <summary> * Get the minimum preferred height for this atom.
		///	 * 
		///	 * @return			The minimum preferred height for this atom.
		///	 * 					Returns zero if we don't know yet. </summary>
		///
		public virtual int PreferredHeight
		{
			get
			{
				return 0;
			}
		}


		///
		/// <summary> * Determine whether we have the bounding rect of this Element.
		/// *  </summary>
		/// * <returns>  True if our geometry has been set up. </returns>
		///
		public bool haveBounds()
		{
			return Width > 0 && Height > 0;
		}


		///
		///	 <summary> * Get the bounding rect of this Element.
		///	 * 
		///	 * @return				The bounding rect of this element within
		///	 * 						its parent View. This will be 0, 0, 0, 0 if
		///	 * 						setGeometry() has not been called yet. </summary>
		///
		public Rectangle Bounds
		{
			get
			{
				return elemBounds;
			}
		}


		///
		///	 <summary> * Get the width of this element -- i.e. the current configured width.
		///	 * 
		///	 * @return				The width of this element within
		///	 * 						its parent View. This will be 0 if
		///	 * 						setGeometry() has not been called yet. </summary>
		///
		public int Width
		{
			get
			{
				return elemBounds.Right - elemBounds.Left;
			}
		}


		///
		///	 <summary> * Get the height of this element -- i.e. the current configured height.
		///	 * 
		///	 * @return				The height of this element within
		///	 * 						its parent View. This will be 0 if
		///	 * 						setGeometry() has not been called yet. </summary>
		///
		public int Height
		{
			get
			{
				return elemBounds.Bottom - elemBounds.Top;
			}
		}


		// ******************************************************************** //
		// Appearance.
		// ******************************************************************** //

		///
		///	 <summary> * Set the background colour of this element.
		///	 * 
		///	 * @param	col			The new background colour, in ARGB format. </summary>
		///
		public virtual uint BackgroundColor
		{
			set
			{
				colBg = value;
			}
			get
			{
				return colBg;
			}
		}


		///
		///	 <summary> * Get the background colour of this element.
		///	 * 
		///	 * @return				The background colour, in ARGB format. </summary>
		///


		///
		///	 <summary> * Set the plot colours of this element.
		///	 * 
		/// * @param	grid      Colour for drawing a data scale / grid.
		/// * @param	plot      Colour for drawing data plots. </summary>
		///
		public virtual void setDataColors(uint grid, uint plot)
		{
			gridColour = grid;
			plotColour = plot;
		}


		///
		/// <summary> * Set the data scale / grid colour of this element.
		/// *  </summary>
		/// * <param name="grid"> Colour for drawing a data scale / grid. </param>
		///
		public virtual uint GridColor
		{
			set
			{
				gridColour = value;
			}
			get
			{
				return gridColour;
			}
		}


		///
		/// <summary> * Set the data plot colour of this element.
		/// *  </summary>
		/// * <param name="plot"> Colour for drawing a data plot. </param>
		///
		public virtual uint PlotColor
		{
			set
			{
				plotColour = value;
			}
			get
			{
				return plotColour;
			}
		}


		///
		/// <summary> * Get the data scale / grid colour of this element.
		/// *  </summary>
		/// * <returns> Colour for drawing a data scale / grid. </returns>
		///


		///
		/// <summary> * Get the data plot colour of this element.
		/// *  </summary>
		/// * <returns>   Colour for drawing data plots. </returns>
		///


		// ******************************************************************** //
		// Error Handling.
		// ******************************************************************** //

		///
		/// <summary> * An error has occurred. Notify the user somehow.
		/// * 
		/// * <p>Subclasses can override this to do something neat.
		/// *  </summary>
		/// * <param name="error">  ERR_XXX code describing the error. </param>
		///
		public virtual void error(int error)
		{

		}


		// ******************************************************************** //
		// View Drawing.
		// ******************************************************************** //


		///
		///	 <summary> * Get this element's Pen.
		///	 * 
		///	 * @return				The Pen which was set up in initializePen(). </summary>
		///
		protected internal virtual Pen Pen
		{
			get
			{
				return drawPen;
			}
		}


		///
		/// <summary> * This method is called to ask the element to draw its static
		/// * content; i.e. the background / chrome.
		/// *  </summary>
		/// * <param name="graphics"> Canvas to draw into. </param>
		///
		public virtual void drawBackground(Graphics graphics)
		{
			if (backgroundBitmap != null)
			{
				graphics.DrawImage((Image)backgroundBitmap, elemBounds.Left, elemBounds.Top);
			}
			else
			{
				drawBackground(graphics, 0, 0);
			}
		}


		///
		/// <summary> * This internal method is used to get the gauge implementation to
		/// * render its background at a specific location.
		/// *  </summary>
		/// * <param name="graphics"> Canvas to draw into. </param>
		/// * <param name="dx">   X co-ordinate translation to apply. </param>
		/// * <param name="dy">   Y co-ordinate translation to apply. </param>
		///
		private void drawBackground(Graphics graphics, int dx, int dy)
		{
			// Clip to our part of the graphics.
			//graphics.save();
			//graphics.translate(dx, dy);
			//graphics.clipRect(Bounds);

			drawBackgroundBody(graphics, drawPen);

			//graphics.restore();
		}


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
		protected internal virtual void drawBackgroundBody(Graphics graphics, Pen pen)
		{
			// If not overridden, we shouldn't need anything, as the overall
			// background is cleared each time.
		}


		///
		/// <summary> * This method is called to ask the element to draw its dynamic content.
		/// *  </summary>
		/// * <param name="graphics"> Canvas to draw into. </param>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		/// * <param name="bg">   Iff true, tell the gauge to draw its background
		/// *                      first. This is cheaper than calling
		/// *                      <seealso cref="#drawBackground(Canvas)"/> before
		/// *                      this method. </param>
		///
		public virtual void draw(Graphics graphics, long now, bool bg)
		{
			drawStart(graphics, drawPen, now);
			if (bg)
			{
				if (backgroundBitmap != null)
				{
					graphics.DrawImage((Image)backgroundBitmap, elemBounds.Left, elemBounds.Top);
				}
				else
				{
					drawBackgroundBody(graphics, drawPen);
				}
			}
			drawBody(graphics, drawPen, now);
			drawFinish(graphics, drawPen, now);
		}


		///
		///	 <summary> * Do initial parts of drawing for this element.
		///	 * 
		///	 * @param	graphics		Canvas to draw into.
		///	 * @param	pen		The Graphics which was set up in initializePen(). </summary>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		///
		protected internal virtual void drawStart(Graphics graphics, Pen pen, long now)
		{
			// Clip to our part of the graphics.
			//graphics.save();
			//graphics.clipRect(Bounds);
		}


		///
		///	 <summary> * Do the subclass-specific parts of drawing for this element.
		///	 * 
		///	 * Subclasses should override this to do their drawing.
		///	 * 
		///	 * @param	graphics		Canvas to draw into.
		///	 * @param	pen		The Graphics which was set up in initializePen(). </summary>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		///
		protected internal virtual void drawBody(Graphics graphics, Pen pen, long now)
		{
			// If not overridden, just fill with BG colour.
			SolidBrush drawBrush = new SolidBrush(AColor.UIntToColor(colBg));
			graphics.FillRectangle(drawBrush, Bounds);
		}


		///
		///	 <summary> * Wrap up drawing of this element.
		///	 * 
		///	 * @param	graphics		Canvas to draw into.
		///	 * @param	pen		The Graphics which was set up in initializePen(). </summary>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		///
		protected internal virtual void drawFinish(Graphics graphics, Pen pen, long now)
		{
			//graphics.restore();
		}

		/// helper function to measure number of pixels a specific string will take
		protected float MeasureString(Graphics graphics, string template)
		{
			return graphics.MeasureString(template, TextFont).Width;
		}

		protected System.Drawing.SizeF MeasureString(string text, System.Drawing.Font font)
		{    
    		return System.Drawing.Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, font);
		}		

		/// convert uint on format 0x1000001E to actual uint
		public static uint Hex2Dec(String hexNumber) {
			int val = System.Int32.Parse(hexNumber, System.Globalization.NumberStyles.AllowHexSpecifier);		
			//return Convert.ToUInt32(hexNumber);
			return (uint)val;
		}

		/// convert decimal back to hex format like 0x1000001E 
		public static String Dec2Hex(uint dec) {
			return String.Format("{0:X8}", dec);
		}
		
		protected float findTextSize(Graphics graphics, float w, float h, string template, Pen pen)
		{
			float size = h;
			do
			{
				//pen.TextSize = size;
				int sw = (int) graphics.MeasureString(template, TextFont).Width;
				if (sw <= w)
				{
					break;
				}
				--size;
			} while (size > 12);

			return size;
		}
		
		
		// ******************************************************************** //
		// Private Constants.
		// ******************************************************************** //

		// The minimum happy text size.
		private const float MIN_TEXT = 22f;


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const string TAG = "instrument";

		// The default font for all text.
		private static Font baseTextFace = new Font("Arial", 8);
		
		// The base size for all text, based on screen size.
		private static float baseTextSize = MIN_TEXT;

		// Various other text sizes.
		private static float headTextSize = MIN_TEXT * 1.3f;
		private static float miniTextSize = MIN_TEXT * 0.9f;
		private static float tinyTextSize = MIN_TEXT * 0.8f;

		// The horizontal scaling of the font; this can be used to
		// produce a tall, thin font.
		private static float textScaleX = 1f;

		// The thickness of a side bar in a view element.
		private static int viewSidebar;

		// The amount of padding between major elements in a view.
		private static int interPadding;

		// The amount of padding within atoms within an element. Specifically
		// the small gaps in side bars.
		private static int innerGap;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Option flags for this instance. A bitwise OR of GAUGE_XXX constants.
		private int gaugeOptions = 0;

		// The pen we use for drawing.
		private Pen drawPen = new Pen(Color.Black);
		
		// the brush for drawing text
		private Brush drawBrush = Brushes.Black;

		// The bounding rect of this element within its parent View.
		private Rectangle elemBounds = new Rectangle(0, 0, 0, 0);

		// Background colour.
		private uint colBg = 0xff000000;

		// Colour of the graph grid and plot.
		private uint gridColour = 0xff00ff00;
		private uint plotColour = 0xffff0000;

		// Bitmap in which we draw the gauge background, if we're caching it,
		// and the Canvas for drawing into it.
		private Bitmap backgroundBitmap = null;
		private Graphics backgroundCanvas = null;

	}

}