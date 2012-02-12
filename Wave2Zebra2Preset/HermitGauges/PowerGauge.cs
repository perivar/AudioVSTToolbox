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
	/// <summary> * A graphical display which displays the signal power in dB from an
	/// * <seealso cref="AudioAnalyser"/> instrument. This class cannot be instantiated
	/// * directly; get an instance by calling
	/// * <seealso cref="AudioAnalyser#getPowerGauge()"/>. </summary>
	/// 
	public class PowerGauge : Gauge
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Create a PowerGauge. This constructor is package-local, as
		///	 * public users get these from an <seealso cref="AudioAnalyser"/> instrument.
		///	 * 
		///	 * @param	parent			Parent surface. </summary>
		///
		internal PowerGauge() : base()
		{

			meterPeaks = new float[METER_PEAKS];
			meterPeakTimes = new long[METER_PEAKS];

			// Create and initialize the history buffer.
			powerHistory = new float[METER_AVERAGE_COUNT];
			for (int i = 0; i < METER_AVERAGE_COUNT; ++i)
			{
				powerHistory[i] = -100.0f;
			}
			averagePower = -100.0f;

			// Create the buffers where the text labels are formatted.
			dbBuffer = "-100.0dB";
			pkBuffer = "-100.0dB peak";
		}


		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		///	 <summary> * Set the overall thickness of the bar.
		///	 *  </summary>
		///	 * <param name="width">  Overall width in pixels of the bar. </param>
		///
		public virtual int BarWidth
		{
			set
			{
				barWidth = value;
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
				
				// Create the bitmap for the background,
				// and the Canvas for drawing into it.
				Bitmap bitmap = new Bitmap( value.Width, value.Height, PixelFormat.Format32bppArgb );
				Graphics canvas = Graphics.FromImage(bitmap);
				
				Pen pen = DrawPen;
				
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
				
				// The bar and labels.
				meterBarTop = 0;
				meterBarGap = barWidth / 4;
				meterLabY = meterBarTop + barWidth + labelSize;
				
				// Horizontal margins.
				meterBarMargin = labelSize;
				
				// Text readouts. First try putting them side by side.
				float th = mh - meterLabY;
				float subWidth = (mw - meterBarMargin * 2) / 2;
				
				meterTextSize = findTextSize(canvas, subWidth, th, "-100.0dB", pen);
				//pen.TextSize = meterTextSize;
				meterTextX = meterBarMargin;
				//meterTextY = mh - pen.descent();
				
				meterSubTextSize = findTextSize(canvas, subWidth, th, "-100.0dB peak", pen);
				//pen.TextSize = meterSubTextSize;
				meterSubTextX = meterTextX + subWidth;
				//meterSubTextY = mh - pen.descent();
				
				// If we have tons of empty space, stack the text readouts vertically.
				if (meterTextSize <= th / 2)
				{
					float Split = th * 1f / 3f;
					meterTextSize = (th - Split) * 0.9f;
					//pen.TextSize = meterTextSize;
					
					float tw = MeasureString(canvas, "-100.0dB");
					meterTextX = (mw - tw) / 2f;
					//meterTextY = mh - Split - pen.descent();
					
					meterSubTextSize = (th - meterTextSize) * 0.9f;
					//pen.TextSize = meterSubTextSize;
					float pw = MeasureString(canvas, "-100.0dB peak");
					meterSubTextX = (mw - pw) / 2f;
					//meterSubTextY = mh - pen.descent();
				}
				
				// Cache our background image.
				cacheBackground();
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
		protected internal override void drawBackgroundBody(Graphics graphics, Pen pen)
		{
			SolidBrush drawBrush = new SolidBrush(ColorUtils.UIntToColor(0xff000000));
			graphics.FillRectangle(drawBrush, Bounds);
			pen.DashStyle = DashStyle.Dash;
			pen.Color = ColorUtils.UIntToColor(0xffffff00);

			// Draw the grid.
			float mx = dispX + meterBarMargin;
			float mw = dispWidth - meterBarMargin * 2;
			float by = dispY + meterBarTop;
			float bw = mw - 1;
			float bh = barWidth - 1;
			float gw = bw / 10f;
			graphics.DrawRectangle(pen, mx, by, mx + bw, by + bh);
			for (int i = 1; i < 10; ++i)
			{
				float x = (float) i * (float) bw / 10f;
				graphics.DrawLine(pen, mx + x, by, mx + x, by + bh);
			}

			// Draw the labels below the grid.
			float ly = dispY + meterLabY;
			float ls = labelSize;
			//pen.TextSize = ls;
			int step = (int)(MeasureString(graphics, "-99")) > bw / 10f - 1 ? 2 : 1;
			for (int i = 0; i <= 10; i += step)
			{
				string text = "" + (i * 10 - 100);
				float tw = MeasureString(graphics, text);
				float lx = mx + i * gw + 1 - (tw / 2);
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
		/// * <param name="power">  The current instantaneous signal power level
		/// *                      in dB, from -Inf to 0+. Typical range is
		/// *                      -100dB to 0dB, 0dB representing max. input power. </param>
		///
		internal void Update(double power)
		{
			lock (this)
			{
				// Save the current level. Clip it to a reasonable range.
				if (power < -100.0)
				{
					power = -100.0;
				}
				else if (power > 0.0)
				{
					power = 0.0;
				}
				currentPower = (float) power;

				// Get the previous power value, and add the new value into the
				// history buffer. Re-calculate the rolling average power value.
				if (++historyIndex >= powerHistory.Length)
				{
					historyIndex = 0;
				}
				prevPower = powerHistory[historyIndex];
				powerHistory[historyIndex] = (float) power;
				averagePower -= prevPower / METER_AVERAGE_COUNT;
				averagePower += (float) power / METER_AVERAGE_COUNT;
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
			lock (this)
			{
				// Re-calculate the peak markers.
				calculatePeaks(now, currentPower, prevPower);

				pen.Color = ColorUtils.UIntToColor(0xffffff00);
				pen.DashStyle = DashStyle.Dash;

				// Position parameters.
				float mx = dispX + meterBarMargin;
				float mw = dispWidth - meterBarMargin * 2;
				float by = dispY + meterBarTop;
				float bh = barWidth;
				float gap = meterBarGap;
				float bw = mw - 2f;

				// Draw the average bar.
				float pa = (averagePower / 100f + 1f) * bw;
				//pen.Style = Graphics.Style.FILL;
				pen.Color = ColorUtils.UIntToColor(METER_AVERAGE_COL);

				graphics.DrawRectangle(pen, mx + 1, by + 1, mx + pa + 1, by + bh - 1);

				// Draw the power bar.
				float p = (currentPower / 100f + 1f) * bw;
				//pen.Style = Graphics.Style.FILL;
				pen.Color = ColorUtils.UIntToColor(METER_POWER_COL);
				graphics.DrawRectangle(pen, mx + 1, by + gap, mx + p + 1, by + bh - gap);

				// Now, draw in the peaks.
				//pen.Style = Graphics.Style.FILL;
				for (int i = 0; i < METER_PEAKS; ++i)
				{
					if (meterPeakTimes[i] != 0)
					{
						// Fade the peak according to its age.
						long age = now - meterPeakTimes[i];
						float fac = 1f - ((float) age / (float) METER_PEAK_TIME);
						int alpha = (int)(fac * 255f);
						pen.Color = ColorUtils.UIntToColor((uint)(METER_PEAK_COL | (alpha << 24)));
						// Draw it in.
						float pp = (meterPeaks[i] / 100f + 1f) * bw;
						graphics.DrawRectangle(pen, mx + pp - 1, by + gap, mx + pp + 3, by + bh - gap);
					}
				}

				// Draw the text below the meter.
				float tx = dispX + meterTextX;
				float ty = dispY + meterTextY;
				dbBuffer = String.Format("{0:0.#}", averagePower);
				pen.DashStyle = DashStyle.Dash;
				pen.Color = ColorUtils.UIntToColor(0xff00ffff);
				//pen.TextSize = meterTextSize;
				graphics.DrawString(dbBuffer, TextFont, TextBrush, tx, ty);

				float px = dispX + meterSubTextX;
				float py = dispY + meterSubTextY;
				pkBuffer = String.Format("{0:0.#}", meterPeakMax);
				//pen.TextSize = meterSubTextSize;
				graphics.DrawString(pkBuffer, TextFont, TextBrush, px, py);
			}
		}


		///
		/// <summary> * Re-calculate the positions of the peak markers in the VU meter. </summary>
		///
		private void calculatePeaks(long now, float power, float prev)
		{
			// First, delete any that have been passed or have timed out.
			for (int i = 0; i < METER_PEAKS; ++i)
			{
				if (meterPeakTimes[i] != 0 && (meterPeaks[i] < power || now - meterPeakTimes[i] > METER_PEAK_TIME))
				{
					meterPeakTimes[i] = 0;
				}
			}

			// If the meter has gone up, set a new peak, if there's an empty
			// slot. If there isn't, don't bother, because we would be kicking
			// out a higher peak, which we don't want.
			if (power > prev)
			{
				bool done = false;

				// First, check for a slightly-higher existing peak. If there
				// is one, just bump its time.
				for (int i = 0; i < METER_PEAKS; ++i)
				{
					if (meterPeakTimes[i] != 0 && meterPeaks[i] - power < 2.5)
					{
						meterPeakTimes[i] = now;
						done = true;
						break;
					}
				}

				if (!done)
				{
					// Now scan for an empty slot.
					for (int i = 0; i < METER_PEAKS; ++i)
					{
						if (meterPeakTimes[i] == 0)
						{
							meterPeaks[i] = power;
							meterPeakTimes[i] = now;
							break;
						}
					}
				}
			}

			// Find the highest peak value.
			meterPeakMax = -100f;
			for (int i = 0; i < METER_PEAKS; ++i)
			{
				if (meterPeakTimes[i] != 0 && meterPeaks[i] > meterPeakMax)
				{
					meterPeakMax = meterPeaks[i];
				}
			}
		}


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const string TAG = "instrument";

		// Number of peaks we will track in the VU meter.
		private const int METER_PEAKS = 4;

		// Time in ms over which peaks in the VU meter fade out.
		private const int METER_PEAK_TIME = 4000;

		// Number of updates over which we average the VU meter to get
		// a rolling average. 32 is about 2 seconds.
		private const int METER_AVERAGE_COUNT = 32;

		// Colours for the meter power bar and average bar and peak marks.
		// In METER_PEAK_COL, alpha is set dynamically in the code.
		private const uint METER_POWER_COL = 0xff0000ff;
		private const uint METER_AVERAGE_COL = 0xa0ff9000;
		private const uint METER_PEAK_COL = 0x00ff0000;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Configured meter bar thickness.
		private int barWidth = 32;

		// Display position and size within the parent view.
		private int dispX = 0;
		private int dispY = 0;
		private int dispWidth = 0;
		private int dispHeight = 0;

		// Label text size for the gauge. Zero if not set yet.
		private float labelSize = 0f;

		// Layout parameters for the VU meter. Position and size for the
		// bar itself; position and size for the bar labels; position
		// and size for the main readout text.
		private float meterBarTop = 0;
		private float meterBarGap = 0;
		private float meterLabY = 0;
		private float meterTextX = 0;
		private float meterTextY = 0;
		private float meterTextSize = 0;
		private float meterSubTextX = 0;
		private float meterSubTextY = 0;
		private float meterSubTextSize = 0;
		private float meterBarMargin = 0;

		// Current and previous power levels.
		private float currentPower = 0f;
		private float prevPower = 0f;

		// Buffered old meter levels, used to calculate the rolling average.
		// Index of the most recent value.
		private float[] powerHistory = null;
		private int historyIndex = 0;

		// Rolling average power value,  calculated from the history buffer.
		private float averagePower = -100.0f;

		// Peak markers in the VU meter, and the times for each one. A zero
		// time indicates a peak not set.
		private float[] meterPeaks = null;
		private long[] meterPeakTimes = null;
		private float meterPeakMax = 0f;

		// Buffer for displayed average and peak dB value texts.
		private String dbBuffer = null;
		private String pkBuffer = null;
	}
}