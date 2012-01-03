using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lomont;

using System.Drawing.Extended;

namespace CommonUtils.FFT
{
	/// <summary>
	/// Description of FFTUtils.
	/// </summary>
	public static class FFTUtils
	{

		public static float[][] CreateSpectrogram(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			
			LomontFFT fft = new LomontFFT();
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap; 	// width of the segment - i.e. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			float[][] frames = new float[numberOfSegments][];
			double[] complexSignal = new double[2*fftWindowsSize]; 		// even - Re, odd - Img
			for (int i = 0; i < numberOfSegments; i++)
			{
				// apply Hanning Window
				for (int j = 0; j < fftWindowsSize; j++)
				{
					// add window with (4.0 / (fftWindowsSize - 1)
					complexSignal[2*j] = (double) ((4.0/(fftWindowsSize - 1)) * windowArray[j] * samples[i * fftOverlap + j]); 	// Weight by Hann Window
					//complexSignal[2*j] = (double) (windowArray[j] * samples[i * fftOverlap + j]); 	// Weight by Hann Window
					complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer (phase)
				}

				// FFT transform for gathering the spectrum
				fft.FFT(complexSignal, true);
				float[] band = new float[fftWindowsSize/2];
				for (int j = 0; j < fftWindowsSize/2; j++)
				{
					double re = complexSignal[2*j];
					double img = complexSignal[2*j + 1];
					band[j] = (float) Math.Sqrt(re*re + img*img);
				}
				frames[i] = band;
			}
			
			return frames;
		}

		
		public static Bitmap PrepareAndDrawSpectrumAnalysis(float[][] spectrogramData, double sampleRate, int fftWindowsSize, int fftOverlap, Size imageSize) {
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
			 * 20 log10 (mag) => 20/ ln(10) ln(mag)
			 */
			
			int numberOfSegments = spectrogramData.Length; // i.e. 78 images which containt a spectrum which is half the fftWindowsSize (2048)
			int spectrogramLength = spectrogramData[0].Length; // 1024 - half the fftWindowsSize (2048)
			double numberOfSamples = (fftOverlap * numberOfSegments) + fftWindowsSize;
			double seconds = numberOfSamples / sampleRate;
			
			float[] m_mag = new float[spectrogramLength];
			float[] m_freq = new float[spectrogramLength];
			for (int i = 0; i < spectrogramLength; i++)
			{
				m_mag[i] = MathUtils.ConvertAmplitudeToDB((float) spectrogramData[0][i], -120.0f, 18.0f);
				m_freq[i] = MathUtils.ConvertIndexToHz (i, spectrogramLength, sampleRate, fftWindowsSize);
			}
			return DrawSpectrumAnalysis(ref m_mag, ref m_freq, imageSize);
		}

		/**
		 * Draw a graph of the spectrum
		 *
		 * Released under the MIT License
		 *
		 * Copyright (c) 2010 Gerald T. Beauregard
		 * Ported to C# and heavily modifified by Per Ivar Nerseth, 2012
		 *
		 * Permission is hereby granted, free of charge, to any person obtaining a copy
		 * of this software and associated documentation files (the "Software"), to
		 * deal in the Software without restriction, including without limitation the
		 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
		 * sell copies of the Software, and to permit persons to whom the Software is
		 * furnished to do so, subject to the following conditions:
		 *
		 * The above copyright notice and this permission notice shall be included in
		 * all copies or substantial portions of the Software.
		 */
		public static Bitmap DrawSpectrumAnalysis(ref float[] mag, ref float[] freq, Size imageSize)
		{
			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			float MIN_FREQ = 0;					// Minimum frequency (Hz) on horizontal axis.
			float MAX_FREQ = 20000;				// Maximum frequency (Hz) on horizontal axis.
			float FREQ_STEP = 2000;				// Interval between ticks (Hz) on horizontal axis.
			float MAX_DB = -0.0f;				// Maximum dB magnitude on vertical axis.
			float MIN_DB = -100.0f; //-60       // Minimum dB magnitude on vertical axis.
			float DB_STEP = 20;                	// Interval between ticks (dB) on vertical axis.

			int TOP = 4;                     	// Top of graph
			int LEFT = 4;                    	// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph
			string LABEL_X = "Frequency (Hz)"; 	// Label for X axis
			string LABEL_Y = "dB";             	// Label for Y axis
			bool drawLabels = false;
			bool drawRoundedRectangles = true;
			
			// Colors
			Color lineColor = ColorTranslator.FromHtml("#C7834C");
			Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
			Color textColor = ColorTranslator.FromHtml("#A9652E");
			Color sampleColor = ColorTranslator.FromHtml("#A9652E");
			Color fillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
			Color fillColor = ColorTranslator.FromHtml("#F9C998");
			
			// Derived constants
			int BOTTOM = TOTAL_HEIGHT-TOP;                   		// Bottom of graph
			float DBTOPIXEL = (float) HEIGHT/(MAX_DB-MIN_DB);    	// Pixels/tick
			float FREQTOPIXEL = (float) WIDTH/(MAX_FREQ-MIN_FREQ);	// Pixels/Hz
			
			try {
				Bitmap png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				ExtendedGraphics eg = new ExtendedGraphics(g);
				
				int numPoints = mag.Length;
				if ( mag.Length != freq.Length )
					System.Diagnostics.Debug.WriteLine( "mag.length != freq.length" );
				
				Pen linePen = new Pen(lineColor, 0.5f);
				Pen middleLinePen = new Pen(middleLineColor, 0.5f);
				Pen textPen = new Pen(textColor, 1);
				Pen samplePen = new Pen(sampleColor, 1);

				// Draw a rectangular box marking the boundaries of the graph
				// Create outer rectangle.
				Rectangle rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
				Brush fillBrushOuter = new SolidBrush(fillOuterColor);
				g.FillRectangle(fillBrushOuter, rectOuter);
				
				// Create rectangle.
				Rectangle rect = new Rectangle(LEFT, TOP, WIDTH, HEIGHT);
				Brush fillBrush = new SolidBrush(fillColor);
				if (drawRoundedRectangles) {
					eg.FillRoundRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height, 10);
					eg.DrawRoundRectangle(linePen, rect.X, rect.Y, rect.Width, rect.Height, 10);
				} else {
					g.FillRectangle(fillBrush, rect);
					g.DrawRectangle(linePen, rect);
				}
				
				// Label for horizontal axis
				Font drawLabelFont = new Font("Arial", 8);
				SolidBrush drawLabelBrush = new SolidBrush(textPen.Color);
				if (drawLabels) {
					SizeF drawLabelTextSize = g.MeasureString(LABEL_X, drawLabelFont);
					g.DrawString(LABEL_X, drawLabelFont, drawLabelBrush, (TOTAL_WIDTH/2) - (drawLabelTextSize.Width/2), TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -3);
				}
				
				float y = 0;
				float yMiddle = 0;
				float x = 0;
				float xMiddle = 0;
				for ( float dBTick = MIN_DB; dBTick <= MAX_DB; dBTick += DB_STEP )
				{
					// draw horozontal main line
					y = BOTTOM - DBTOPIXEL*(dBTick-MIN_DB);
					if (y < BOTTOM && y > TOP+1) {
						g.DrawLine(linePen, LEFT, y, LEFT+WIDTH, y);
					}

					// draw horozontal middle line (between the main lines)
					yMiddle = y-(DBTOPIXEL*DB_STEP)/2;
					if (yMiddle > 0) {
						g.DrawLine(middleLinePen, LEFT, yMiddle, LEFT+WIDTH, yMiddle);
					}

					if ( dBTick != MAX_DB )
					{
						// Numbers on the tick marks
						Font drawFont = new Font("Arial", 8);
						SolidBrush drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + dBTick + " dB", drawFont, drawBrush, LEFT+5, y - drawFont.GetHeight(g) -2);
					}
				}
				
				if (drawLabels) {
					// Label for vertical axis
					g.DrawString(LABEL_Y, drawLabelFont, drawLabelBrush, 1, TOP + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
				}
				
				// Tick marks on the horizontal axis
				for ( float f = MIN_FREQ; f <= MAX_FREQ; f += FREQ_STEP )
				{
					// draw vertical main line
					x = LEFT + FREQTOPIXEL*(f-MIN_FREQ);
					if (x > LEFT  && x < WIDTH) {
						g.DrawLine(linePen, x, BOTTOM, x, TOP);
					}

					// draw vertical middle line (between the main lines)
					xMiddle = x + FREQTOPIXEL*FREQ_STEP/2;
					if (xMiddle < TOTAL_WIDTH) {
						g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP);
					}

					if ( f != MIN_FREQ && f != MAX_FREQ )
					{
						// Numbers on the tick marks
						Font drawFont = new Font("Arial", 8);
						SolidBrush drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + f + " Hz", drawFont, drawBrush, x, TOP +2);
					}
				}
				

				// The line in the graph
				int i = 0;
				
				// Ignore points that are too far to the left
				for ( i = 0; i < numPoints && freq[i] < MIN_FREQ; i++ )
				{
				}
				
				// For all remaining points within range of x-axis
				float oldX = 0;
				float oldY = TOP;
				bool firstPoint = true;
				for ( ; i < numPoints && freq[i] <= MAX_FREQ; i++ )
				{
					// Compute horizontal position
					x = LEFT + FREQTOPIXEL*(freq[i]-MIN_FREQ);
					
					// Compute vertical position of point
					// and clip at top/bottom.
					y = BOTTOM - DBTOPIXEL*(mag[i]-MIN_DB);
					
					if ( y < TOP )
						y = TOP;
					else if ( y > BOTTOM )
						y = BOTTOM;
					
					// If it's the first point
					if ( firstPoint )
					{
						// Move to the point
						oldX = x;
						oldY = y;
						firstPoint = false;
					}
					else
					{
						// Otherwise, draw line from the previous point
						g.DrawLine(samplePen, oldX, oldY, x, y);
						oldX = x;
						oldY = y;
					}
				}
				
				return png;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return null;
			}
		}
	}
}
