using System;
using System.Drawing;
using System.Drawing.Imaging;
using Lomont;

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

		
		public static Bitmap PrepareAndDrawSpectrumAnalysis(float[][] spectrogramData, double sampleRate, int fftWindowsSize, int fftOverlap, Color foreColor, Color backColor, Size imageSize) {
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
				m_mag[i] = ConvertAmplitudeToDB((float) spectrogramData[0][i], -120.0f, 18.0f);
				m_freq[i] = ConvertIndexToHz (i, spectrogramLength, sampleRate, fftWindowsSize);
			}
			return DrawSpectrumAnalysis(ref m_mag, ref m_freq, foreColor, backColor, imageSize);
		}

		/**
		 * Draw a graph of the spectrum
		 *
		 * Released under the MIT License
		 *
		 * Copyright (c) 2010 Gerald T. Beauregard
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
		public static Bitmap DrawSpectrumAnalysis(ref float[] mag, ref float[] freq, Color foreColor, Color backColor, Size imageSize)
		{
			// Basic constants
			float MIN_FREQ = 0;                 // Minimum frequency (Hz) on horizontal axis.
			float MAX_FREQ = 20000; //4000      // Maximum frequency (Hz) on horizontal axis.
			float FREQ_STEP = 2000; //500;      // Interval between ticks (Hz) on horizontal axis.
			float MAX_DB = -0.0f;           	// Maximum dB magnitude on vertical axis.
			float MIN_DB = -100.0f; //-60       // Minimum dB magnitude on vertical axis.
			float DB_STEP = 20;                	// Interval between ticks (dB) on vertical axis.
			int TOP = 0;                     	// Top of graph
			int LEFT = 0;                    	// Left edge of graph
			int HEIGHT = imageSize.Height;      // Height of graph
			int WIDTH = imageSize.Width;        // Width of graph
			int TICK_LEN = 10;                	// Length of tick in pixels
			String LABEL_X = "Frequency (Hz)"; 	// Label for X axis
			String LABEL_Y = "dB";             	// Label for Y axis
			
			// Derived constants
			int BOTTOM = HEIGHT-TOP;                   				// Bottom of graph
			float DBTOPIXEL = (float) HEIGHT/(MAX_DB-MIN_DB);    	// Pixels/tick
			float FREQTOPIXEL = (float) WIDTH/(MAX_FREQ-MIN_FREQ);	// Pixels/Hz
			
			try {
				Bitmap png = new Bitmap( WIDTH, HEIGHT, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				
				int numPoints = mag.Length;
				if ( mag.Length != freq.Length )
					System.Diagnostics.Debug.WriteLine( "mag.length != freq.length" );
				
				// Draw a rectangular box marking the boundaries of the graph
				Pen linePen = new Pen(backColor, 0.5f);
				Pen textPen = new Pen(foreColor, 1);
				Pen samplePen = new Pen(foreColor, 1);
				
				// Create rectangle.
				Rectangle rect = new Rectangle(LEFT, TOP, WIDTH, HEIGHT);
				g.FillRectangle(Brushes.White, rect);
				g.DrawRectangle(linePen, rect);
				
				// Tick marks on the vertical axis
				float y = 0;
				float x = 0;
				bool m_tickTextAdded = false;
				for ( float dBTick = MIN_DB; dBTick <= MAX_DB; dBTick += DB_STEP )
				{
					y = BOTTOM - DBTOPIXEL*(dBTick-MIN_DB);
					g.DrawLine(linePen, LEFT-TICK_LEN/2, y, LEFT+WIDTH, y);
					if ( m_tickTextAdded == false )
					{
						// Numbers on the tick marks
						Font drawFont = new Font("Arial", 8);
						SolidBrush drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + dBTick, drawFont, drawBrush, LEFT+10, y - drawFont.GetHeight(g)/2);
					}
				}
				
				// Label for vertical axis
				if ( m_tickTextAdded == false )
				{
					Font drawFont = new Font("Arial", 10);
					SolidBrush drawBrush = new SolidBrush(textPen.Color);
					g.DrawString(LABEL_Y, drawFont, drawBrush, LEFT, TOP + HEIGHT/2 - drawFont.GetHeight(g)/2);
				}
				
				// Tick marks on the horizontal axis
				for ( float f = MIN_FREQ; f <= MAX_FREQ; f += FREQ_STEP )
				{
					x = LEFT + FREQTOPIXEL*(f-MIN_FREQ);
					g.DrawLine(linePen, x, BOTTOM + TICK_LEN/2, x, TOP);
					if ( m_tickTextAdded == false )
					{
						// Numbers on the tick marks
						Font drawFont = new Font("Arial", 8);
						SolidBrush drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + f, drawFont, drawBrush, x, BOTTOM-30);
					}
				}
				
				// Label for horizontal axis
				if ( m_tickTextAdded == false )
				{
					Font drawFont = new Font("Arial", 10);
					SolidBrush drawBrush = new SolidBrush(textPen.Color);
					g.DrawString(LABEL_X, drawFont, drawBrush, LEFT+WIDTH/2, BOTTOM-50);
				}
				
				m_tickTextAdded = true;
				
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
		
		// look at this http://jvalentino2.tripod.com/dft/index.html
		public static float ConvertAmplitudeToDB(float amplitude, float MinDb, float MaxDb) {
			// db = 20 * log10( fft[index] );
			float smallestNumber = float.Epsilon;
			float db = 20 * (float) Math.Log10( (float) (amplitude + smallestNumber) );
			
			if (db < MinDb) db = MinDb;
			if (db > MaxDb) db = MaxDb;
			
			return db;
		}
		
		public static float ConvertIndexToHz(int i, int numberOfSamples, double sampleRate, double fftWindowsSize) {
			// either
			// freq = index * samplerate / fftsize;
			// frequency = ( i + 1 ) * (float) sampleRate / fftWindowsSize;
			
			// or
			// ( i + 1 ) * ((sampleRate / 2) / numberOfSamples)
			double nyquistFreq = sampleRate / 2;
			double firstFrequency = nyquistFreq / numberOfSamples;
			double frequency = firstFrequency * ( i + 1 );
			return (float) frequency;
		}
	}
}
