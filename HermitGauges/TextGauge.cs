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

namespace Wave2ZebraSynth.HermitGauges
{
	///
	/// <summary> * A <seealso cref="Gauge"/> which displays data in textual form, generally as
	/// * a grid of numeric values. </summary>
	/// 
	public class TextGauge : Gauge
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		/// <summary> * Set up this view, and configure the text fields to be displayed in
		/// * this element. This is equivalent to calling setTextFields()
		/// * after the basic constructor.
		/// * 
		/// * We support display of a single field, or a rectangular table
		/// * of fields. The caller must call
		/// * <seealso cref="#setTextFields(String[] template, int rows)"/> to set the
		/// * table format.
		/// *  </summary>
		/// * <param name="parent">   Parent surface. </param>
		///
		public TextGauge() : base()
		{
			textSize = BaseTextSize;
		}


		///
		/// <summary> * Set up this view, and configure the text fields to be displayed in
		/// * this element. This is equivalent to calling setTextFields()
		/// * after the basic constructor.
		/// * 
		/// * We support display of a single field, or a rectangular table
		/// * of fields. The fields are specified by passing in sample text
		/// * values to be measured; we then allocate the space automatically.
		/// *  </summary>
		/// * <param name="parent">   Parent surface. </param>
		/// * <param name="template"> Strings representing the columns to display.
		/// *                      Each one should be a sample piece of text
		/// *                      which will be measured to determine the
		/// *                      required space for each column. Must be provided. </param>
		/// * <param name="rows"> Number of rows of text to display. </param>
		///
		public TextGauge(string[] template, int rows) : base()
		{
			textSize = BaseTextSize;

			// Set up the text fields.
			setTextFields(template, rows);
		}


		///
		///	 <summary> * Set up the pen for this element. This is called during
		///	 * initialisation. Subclasses can override this to do class-specific
		///	 * one-time initialisation.
		///	 *  </summary>
		///	 * <param name="pen">			The pen to initialise. </param>
		///
		protected internal override void initializePen(Pen pen)
		{
			float scaleX = TextScaleX;
			if (scaleX != 1f)
			{
				//pen.TextScaleX = scaleX;
			}
			//pen.Font = TextFont;
			//pen.AntiAlias = true;
		}


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
				
				// Position our text based on our actual geometry. If setTextFields()
				// hasn't been called this does nothing.
				positionText();
			}
		}


		///
		/// <summary> * Set the margins around the displayed text. This the total space
		/// * between the edges of the element and the outside bounds of the text.
		/// * 
		///	 * @param	left		The left margin.
		///	 * @param	top			The top margin.
		///	 * @param	right		The right margin.
		///	 * @param	bottom		The bottom margin. </summary>
		///
		public virtual void setMargins(int left, int top, int right, int bottom)
		{
			marginLeft = left;
			marginTop = top;
			marginRight = right;
			marginBottom = bottom;

			// Position our text based on these new margins. If setTextFields()
			// hasn't been called this does nothing.
			positionText();
		}


		///
		///	 <summary> * Set up the text fields to be displayed in this element.
		///	 * If this is never called, there will be no text.
		///	 * 
		///	 * We support display of a single field, or a rectangular table
		///	 * of fields. The fields are specified by passing in sample text
		///	 * values to be measured; we then allocate the space automatically.
		///	 * 
		///	 * This must be called before setText() can be called.
		///	 * 
		///	 * @param	template	Strings representing the columns to display.
		///	 * 						Each one should be a sample piece of text
		///	 * 						which will be measured to determine the
		///	 * 						required space for each column. </summary>
		/// * <param name="rows"> Number of rows of text to display. </param>
		///
		public virtual void setTextFields(string[] template, int rows)
		{
			fieldTemplate = template;
			numRows = rows;

			// Make the field buffers based on the template.
			char[][][] buffers = new char[numRows][][];
			for (int r = 0; r < numRows; ++r)
			{
				int cols = template.Length;
				buffers[r] = new char[cols][];
				for (int c = 0; c < cols; ++c)
				{
					int l = template[c].Length;
					char[] buf = new char[l];
					for (int i = 0; i < l; ++i)
					{
						buf[i] = ' ';
					}
					buffers[r][c] = buf;
				}
			}
			fieldBuffers = buffers;

			// Position our text based on the template. If setGeometry()
			// hasn't been called yet, then the positions will not be final,
			// but getTextWidth() and getTextHeight() will return sensible
			// values.
			positionText();
		}


		///
		///	 <summary> * Get the text buffers for the field values. The caller can change
		///	 * a field's content by writing to the appropriate member of the
		///	 * array, as in "buffer[row][col][0] = 'X';".
		///	 *  </summary>
		///	 * <returns> Text buffers for the field values. </returns>
		///
		public virtual char[][][] Buffer
		{
			get
			{
				return fieldBuffers;
			}
		}


		///
		///	 <summary> * Get the minimum width needed to fit all the text.
		///	 * 
		///	 * @return			The minimum width needed to fit all the text.
		///	 * 					Returns zero if setTextFields() hasn't been called. </summary>
		///
		public override int PreferredWidth
		{
			get
			{
				return textWidth;
			}
		}


		///
		///	 <summary> * Get the minimum height needed to fit all the text.
		///	 * 
		///	 * @return			The minimum height needed to fit all the text.
		///	 * 					Returns zero if setTextFields() hasn't been called. </summary>
		///
		public override int PreferredHeight
		{
			get
			{
				return textHeight;
			}
		}


		///
		///	 <summary> * Position the text based on the current template and geometry.
		///	 * If If setTextFields() hasn't been called this does nothing.
		///	 * If setGeometry() hasn't been called yet, then the positions will
		///	 * not be final, but getTextWidth() and getTextHeight() will return
		///	 * sensible values. </summary>
		///
		private void positionText()
		{
			if (fieldTemplate == null)
			{
				return;
			}

			int nf = fieldTemplate.Length;
			colsX = new int[nf];
			rowsY = new int[numRows];

			Rectangle bounds = Bounds;
			Pen pen = DrawPen;
			//pen.TextSize = textSize;

			// Create the bitmap for the background,
			// and the Canvas for drawing into it.
			Bitmap bitmap = new Bitmap( bounds.Width, bounds.Height, PixelFormat.Format32bppArgb );
			Graphics canvas = Graphics.FromImage(bitmap);			
			
			// Assign all the column positions based on minimum width.
			int x = bounds.Left;
			for (int i = 0; i < nf; ++i)
			{
				int len = (int) Math.Ceiling(measureText(canvas, fieldTemplate[i]));
				int lp = i > 0 ? textPadLeft : marginLeft;
				int rp = i < nf - 1 ? textPadRight : marginRight;
				colsX[i] = x + lp;
				x += len + lp + rp;
			}
			textWidth = x - bounds.Left;

			// If we have excess width, distribute it into the inter-column gaps.
			// Don't adjust textWidth because it is the minimum.
			if (nf > 1)
			{
				int excess = (bounds.Right - x) / (nf - 1);
				if (excess > 0)
				{
					for (int i = 1; i < nf; ++i)
					{
						colsX[i] += excess * i;
					}
				}
			}

			// Assign all the row positions based on minimum height.
			//Graphics.FontMetricsInt fm = pen.FontMetricsInt;
			FontFamily fm = TextFont.FontFamily;
			int y = bounds.Top;
			for (int i = 0; i < numRows; ++i)
			{
				int tp = i > 0 ? textPadTop : marginTop;
				int bp = i < numRows - 1 ? textPadBottom : marginBottom;
				rowsY[i] = y + tp - fm.GetCellAscent(FontStyle.Regular) - 2;
				y += -fm.GetCellAscent(FontStyle.Regular) - 2 + fm.GetCellDescent(FontStyle.Regular) + tp + bp;
			}
			textHeight = y - bounds.Top;
		}


		// ******************************************************************** //
		// Appearance.
		// ******************************************************************** //

		///
		///	 <summary> * Set the text colour of this element.
		///	 * 
		///	 * @param	col			The new text colour, in ARGB format. </summary>
		///
		public virtual uint TextColor
		{
			set
			{
				PlotColor = value;
			}
			get
			{
				return PlotColor;
			}
		}


		///
		///	 <summary> * Get the text colour of this element.
		///	 * 
		///	 * @return				The text colour, in ARGB format. </summary>
		///


		///
		///	 <summary> * Set the text size of this element.
		///	 * 
		///	 * @param	size		The new text size. </summary>
		///
		public virtual float TextSize
		{
			set
			{
				textSize = value;
				
				// Position our text based on the new value. If setTextFields()
				// hasn't been called this does nothing.
				positionText();
			}
			get
			{
				return textSize;
			}
		}


		///
		///	 <summary> * Get the text size of this element.
		///	 * 
		///	 * @return				The text size. </summary>
		///


		// ******************************************************************** //
		// View Drawing.
		// ******************************************************************** //

		///
		///	 <summary> * This method is called to ask the element to draw itself.
		///	 * 
		///	 * @param	graphics		Canvas to draw into.
		///	 * @param	pen		The Graphics which was set up in initializePen(). </summary>
		/// * <param name="now">  Nominal system time in ms. of this update. </param>
		///
		protected internal override void drawBody(Graphics graphics, Pen pen, long now)
		{
			// Set up the display style.
			pen.Color = AColor.UIntToColor(PlotColor);
			//pen.TextSize = textSize;

			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final char[][][] tv = fieldBuffers;
			char[][][] tv = fieldBuffers;

			// If we have any text to show, draw it.
			if (tv != null)
			{
				for (int row = 0; row < rowsY.Length && row < tv.Length; ++row)
				{
					char[][] fields = tv[row];
					int y = rowsY[row];
					for (int col = 0; col < colsX.Length && col < fields.Length; ++col)
					{
						char[] field = fields[col];
						int x = colsX[col];
						graphics.DrawString(field.ToString(), TextFont, TextBrush, x, y);
					}
				}
			}
		}


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @SuppressWarnings("unused") private static final String TAG = "instrument";
		private const string TAG = "instrument";


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Template for the text fields we're displaying.
		private string[] fieldTemplate = null;
		private int numRows = 0;

		// Buffers where the values of the fields will be stored.
		private char[][][] fieldBuffers;

		// Horizontal positions of the text columns, and vertical positions
		// of the rows. These are the actual text base positions. These
		// will be null if we have no defined text fields.
		private int[] colsX = null;
		private int[] rowsY = null;

		// The width and height we would need to display all the text at minimum,
		// including padding and margins. Set after a call to setTextFields().
		private int textWidth = 0;
		private int textHeight = 0;

		// Current text size.
		private float textSize;

		// Margins. These are applied around the outside of the text.
		private int marginLeft = 0;
		private int marginRight = 0;
		private int marginTop = 0;
		private int marginBottom = 0;

		// Text padding. This is applied between all pairs of text fields.
		private int textPadLeft = 2;
		private int textPadRight = 2;
		private int textPadTop = 0;
		private int textPadBottom = 0;

	}


}