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

using CommonUtils;

namespace Wave2Zebra2Preset.HermitGauges
{
	///
	/// <summary> * A graphical display which displays the audio waveform from an
	/// * <seealso cref="AudioAnalyser"/> instrument. This class cannot be instantiated
	/// * directly; get an instance by calling
	/// * <seealso cref="AudioAnalyser#getWaveformGauge()"/>. </summary>
	/// 
	public class WaveformGauge : Gauge
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		internal WaveformGauge()
		{
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
				
				dispX = value.Left;
				dispY = value.Top;
				dispWidth = value.Width;
				dispHeight = value.Height;
				
				// Create the bitmap for the audio waveform display,
				// and the Canvas for drawing into it.
				waveBitmap = new Bitmap( dispWidth, dispHeight, PixelFormat.Format32bppArgb );
				waveCanvas = Graphics.FromImage(waveBitmap);
			}
		}


		// ******************************************************************** //
		// Data Updates.
		// ******************************************************************** //

		///
		///	 <summary> * New data from the instrument has arrived. This method is called
		///	 * on the thread of the instrument.
		///	 *  </summary>
		/// * <param name="buffer"> Audio data that was just read. </param>
		/// * <param name="off">  Offset in data of the input data. </param>
		/// * <param name="len">  Length of the input data. </param>
		/// * <param name="bias"> Bias of the signal -- i.e. the offset of the
		/// *                      average signal value from zero. </param>
		/// * <param name="range">  The range of the signal -- i.e. the absolute
		/// *                      value of the largest departure from the bias level. </param>
		///
		internal void Update(short[] buffer, int off, int len, float bias, float range)
		{
			Graphics graphics = waveCanvas;
			Pen pen = DrawPen;

			// Calculate a scaling factor. We want a degree of AGC, but not
			// so much that the waveform is always the same height. Note we have
			// to take bias into account, otherwise we could scale the signal
			// off the screen.
			float scale = (float) Math.Pow(1f / (range / 6500f), 0.7) / 16384 * dispHeight;
			if (scale < 0.001f || float.IsInfinity(scale))
			{
				scale = 0.001f;
			}
			else if (scale > 1000f)
			{
				scale = 1000f;
			}
			float margin = dispWidth / 24;
			float gwidth = dispWidth - margin * 2;
			float baseY = dispHeight / 2f;
			float uw = gwidth / (float) len;

			// Now actually do the drawing.
			lock (this)
			{
				graphics.Clear(ColorUtils.UIntToColor(0xff000000));

				// Draw the axes.
				pen.Color = ColorUtils.UIntToColor(0xffffff00);
				pen.DashStyle = DashStyle.Dash;
				graphics.DrawLine(pen, margin, 0, margin, dispHeight - 1);

				// Draw the waveform. Drawing vertical lines up/down to the
				// waveform creates a "filled" effect, and is *much* faster
				// than drawing the waveform itself with diagonal lines.
				pen.Color = ColorUtils.UIntToColor(0xffffff00);
				pen.DashStyle = DashStyle.Dash;
				for (int i = 0; i < len; ++i)
				{
					float x = margin + i * uw;
					float y = baseY - (buffer[off + i] - bias) * scale;
					graphics.DrawLine(pen, x, baseY, x, y);
				}
				
				// TODO : Remove temp images
				waveBitmap.Save(@"c:\WaveformGauge-waveBitmap.png");
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
				graphics.DrawImage((Image)waveBitmap, dispX, dispY);
			}
		}


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const string TAG = "instrument";

		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Display position and size within the parent view.
		private int dispX = 0;
		private int dispY = 0;
		private int dispWidth = 0;
		private int dispHeight = 0;

		// Bitmap in which we draw the audio waveform display,
		// and the Canvas and Graphics for drawing into it.
		private Bitmap waveBitmap = null;
		private Graphics waveCanvas = null;

	}

}