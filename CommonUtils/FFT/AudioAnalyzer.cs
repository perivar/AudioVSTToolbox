using System;
using System.Drawing;
using System.Drawing.Imaging;

using System.Drawing.Extended;

using Lomont;

namespace CommonUtils.FFT
{
	/// <summary>
	/// Description of FFTUtils.
	/// </summary>
	public static class AudioAnalyzer
	{
		public static float[][] CreateSpectrogramLomont(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			LomontFFT fft = new LomontFFT();
			
			// find the time
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;

			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			
			// width of the segment - i.e. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			float[][] frames = new float[numberOfSegments][];
			
			// even - Re, odd - Img
			double[] complexSignal = new double[2*fftWindowsSize];
			for (int i = 0; i < numberOfSegments; i++)
			{
				// apply Hanning Window
				// e.g. take 371 ms each 11.6 ms (2048 samples each 64 samples)
				for (int j = 0; j < fftWindowsSize; j++)
				{
					// Weight by Hann Window
					complexSignal[2*j] = (double) (windowArray[j] * samples[i * fftOverlap + j]);
					
					// need to clear out as fft modifies buffer (phase)
					complexSignal[2*j + 1] = 0;
				}

				// FFT transform for gathering the spectrum
				fft.FFT(complexSignal, true);

				// get the ABS of the complex signal
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
		
		public static float[][] CreateSpectrogramExocortex(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			
			// find the time
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;
			
			// width of the segment - i.e. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			float[][] frames = new float[numberOfSegments][];
			
			// even - Re, odd - Img
			float[] complexSignal = new float[2*fftWindowsSize];
			for (int i = 0; i < numberOfSegments; i++)
			{
				// apply Hanning Window
				// e.g. take 371 ms each 11.6 ms (2048 samples each 64 samples)
				for (int j = 0; j < fftWindowsSize; j++)
				{
					// Weight by Hann Window
					complexSignal[2*j] = (float) (windowArray[j] * samples[i * fftOverlap + j]);
					
					// need to clear out as fft modifies buffer (phase)
					complexSignal[2*j + 1] = 0;
				}

				// FFT transform for gathering the spectrum
				Fourier.FFT(complexSignal, fftWindowsSize, FourierDirection.Forward);
				
				// get the ABS of the complex signal
				float[] band = new float[fftWindowsSize/2];
				for (int j = 0; j < fftWindowsSize/2; j++)
				{
					double re = complexSignal[2*j];
					double img = complexSignal[2*j + 1];
					
					// do the Abs calculation and multiply by 1/N (2/N cause of the using half the window size)
					band[j] = (float) Math.Sqrt(re*re + img*img) * 2/fftWindowsSize;
				}
				frames[i] = band;
			}
			
			return frames;
		}
		
		public static float[] CreateSpectrumAnalysisLomont(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			LomontFFT fft = new LomontFFT();

			// find the time
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;
			
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);

			// even - Re, odd - Img
			double[] complexSignal = new double[2*fftWindowsSize];

			// apply Hanning Window
			// e.g. take 371 ms each 11.6 ms (2048 samples each 64 samples)
			for (int j = 0; j < fftWindowsSize; j++)
			{
				// Weight by Hann Window
				complexSignal[2*j] = (double) (windowArray[j] * samples[fftOverlap + j]);
				
				// need to clear out as fft modifies buffer (phase)
				complexSignal[2*j + 1] = 0;
			}

			// FFT transform for gathering the spectrum
			fft.FFT(complexSignal, true);

			float[] band = new float[fftWindowsSize/2];
			for (int j = 0; j < fftWindowsSize/2; j++)
			{
				double re = complexSignal[2*j] / (float) Math.Sqrt(fftWindowsSize);
				double img = complexSignal[2*j + 1] / (float) Math.Sqrt(fftWindowsSize);
				
				// do the Abs calculation and add with Math.Sqrt(audio_data.Length);
				band[j] = (float) Math.Sqrt(re*re + img*img) * 4;
			}
			return band;
		}
		
		public static float[] CreateSpectrumAnalysisExocortex(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			// find the time
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;
			
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);

			// even - Re, odd - Img
			float[] complexSignal = new float[2*fftWindowsSize];

			// apply Hanning Window
			// e.g. take 371 ms each 11.6 ms (2048 samples each 64 samples)
			for (int j = 0; j < fftWindowsSize; j++)
			{
				// Weight by Hann Window
				complexSignal[2*j] = (float) (windowArray[j] * samples[fftOverlap + j]);
				
				// need to clear out as fft modifies buffer (phase)
				complexSignal[2*j + 1] = 0;
			}

			// FFT transform for gathering the spectrum
			Fourier.FFT(complexSignal, fftWindowsSize, FourierDirection.Forward);

			float[] band = new float[fftWindowsSize/2];
			for (int j = 0; j < fftWindowsSize/2; j++)
			{
				double re = complexSignal[2*j];
				double img = complexSignal[2*j + 1];
				
				// do the Abs calculation and multiply by 1/N (2/N cause of the using half the window size)
				band[j] = (float) Math.Sqrt(re*re + img*img) * 4/fftWindowsSize;
			}
			return band;
		}

		public static void PrepareSpectrumAnalysis(float[] spectrumData, double sampleRate, int fftWindowsSize, int fftOverlap,
		                                           out float[] m_mag, out float[] m_freq,
		                                           out float foundMaxFrequency, out float foundMaxDecibel) {
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
			 * 20 log10 (mag) => 20/ ln(10) ln(mag)
			 */
			int spectrumDataLength = spectrumData.Length; // 1024 - half the fftWindowsSize (2048)
			double numberOfSamples = fftOverlap + fftWindowsSize;
			double seconds = numberOfSamples / sampleRate;

			// prepare the data:
			m_mag = new float[spectrumDataLength];
			m_freq = new float[spectrumDataLength];
			foundMaxFrequency = -1;
			foundMaxDecibel = -1;

			// prepare to store min and max values
			float maxVal = float.MinValue;
			int maxIndex = 0;
			float minVal = float.MaxValue;
			int minIndex = 0;
			for (int i = 0; i < spectrumDataLength; i++)
			{
				if (spectrumData[i] > maxVal) {
					maxVal = spectrumData[i];
					maxIndex = i;
				}
				if (spectrumData[i] < minVal) {
					minVal = spectrumData[i];
					minIndex = i;
				}

				//m_mag[i] = MathUtils.ConvertAmplitudeToDB((float) spectrumData[i], -120.0f, 18.0f);
				m_mag[i] = MathUtils.ConvertFloatToDB(spectrumData[i]);
				m_freq[i] = MathUtils.ConvertIndexToHz (i, spectrumDataLength, sampleRate, fftWindowsSize);
			}
			
			// store the max findings
			//foundMaxDecibel = MathUtils.ConvertAmplitudeToDB((float) spectrumData[maxIndex], -120.0f, 18.0f);
			foundMaxDecibel = MathUtils.ConvertFloatToDB(spectrumData[maxIndex]);
			foundMaxFrequency = MathUtils.ConvertIndexToHz (maxIndex, spectrumDataLength, sampleRate, fftWindowsSize);
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
		public static Bitmap DrawSpectrumAnalysis(ref float[] mag, ref float[] freq,
		                                          Size imageSize,
		                                          float showMinFrequency = 0, float showMaxFrequency = 20000,
		                                          float foundMaxDecibel = -1, float foundMaxFrequency = -1)
		{
			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			float MIN_FREQ = showMinFrequency;  // Minimum frequency (Hz) on horizontal axis.
			float MAX_FREQ = showMaxFrequency;	// Maximum frequency (Hz) on horizontal axis.
			float FREQ_STEP = 2000;				// Interval between ticks (Hz) on horizontal axis.
			float MAX_DB = -0.0f;				// Maximum dB magnitude on vertical axis.
			float MIN_DB = -100.0f; //-60       // Minimum dB magnitude on vertical axis.
			float DB_STEP = 20;                	// Interval between ticks (dB) on vertical axis.

			int TOP = 5;                     	// Top of graph
			int LEFT = 5;                    	// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph
			string LABEL_X = "Frequency (Hz)"; 	// Label for X axis
			string LABEL_Y = "dB";             	// Label for Y axis
			bool drawLabels = false;
			bool drawRoundedRectangles = true;
			
			// if the max frequency gets lower than ... lower the frequency step
			if (MAX_FREQ < 20000) {
				FREQ_STEP = (float) MathUtils.GetNicerNumber(MAX_FREQ / 10);
			}
			
			// Colors
			Color lineColor = ColorTranslator.FromHtml("#C7834C");
			Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
			Color textColor = ColorTranslator.FromHtml("#A9652E");
			//Color sampleColor = ColorTranslator.FromHtml("#A9652E");
			Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
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
				
				if (foundMaxDecibel != -1 && foundMaxFrequency != -1) {
					string foundMax = String.Format("Max found: {0}dB @ {1} hz", foundMaxDecibel, foundMaxFrequency);
					SizeF foundMaxLabelTextSize = g.MeasureString(foundMax, drawLabelFont);
					g.DrawString(foundMax, drawLabelFont, drawLabelBrush, TOTAL_WIDTH - foundMaxLabelTextSize.Width - 10, TOTAL_HEIGHT - drawLabelFont.GetHeight(g) - 10);
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
		
		public static Bitmap DrawWaveform(float[] data, Size imageSize, int resolution) {

			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			int TOP = 5;                     		// Top of graph
			int LEFT = 5;                    		// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph

			string LABEL_X = "X axis"; 				// Label for X axis
			string LABEL_Y = "Y axis";             	// Label for Y axis
			bool drawRoundedRectangles = true;
			
			// Colors
			Color lineColor = ColorTranslator.FromHtml("#C7834C");
			Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
			Color textColor = ColorTranslator.FromHtml("#A9652E");
			//Color sampleColor = ColorTranslator.FromHtml("#A9652E");
			Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
			Color fillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
			Color fillColor = ColorTranslator.FromHtml("#F9C998");
			
			// Set up for drawing
			Bitmap png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			ExtendedGraphics eg = new ExtendedGraphics(g);
			
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
			
			// Determine channnel boundries
			int CENTER = TOTAL_HEIGHT / 2;

			g.DrawLine(middleLinePen, LEFT, CENTER, WIDTH, CENTER);

			int RIGHT = WIDTH;
			int BOTTOM = HEIGHT;

			int numberOfSamples = data.Length;
			//int resolution = 44; //low resolution (1) means to zoom into the waveform
			int amplitude = 2; // 1 = normal
			
			// Draw channel
			//double yScale = 0.5 * (BOTTOM - TOP) / 32768;  // a 16 bit sample has values from -32768 to 32767
			double yScale = 0.5 * (BOTTOM - TOP) * amplitude;  // a float sample has values from -1 to 1
			int xPrev = 0, yPrev = 0;
			for (int xAxis = 0; xAxis < RIGHT; xAxis++)
			{
				//int sampleToPixel = (int) data.Length / (RIGHT - LEFT); 
				int sampleToPixel = resolution;
				int sampleIndex = sampleToPixel * xAxis;
				float sample = data[sampleIndex];
				int yAxis = (int)(CENTER + (sample * yScale));
				if (xAxis == 0)
				{
					xPrev = 0;
					yPrev = yAxis;
				}
				else
				{
					g.DrawLine(samplePen, xPrev+LEFT, yPrev, xAxis+LEFT, yAxis);
					xPrev = xAxis;
					yPrev = yAxis;
				}
			}
			
			return png;
		}
		
		public static Bitmap DrawWaveform2(float[] data, Size imageSize, int resolution) {

			// Basic constants
			bool sampleBitMono = false;
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			int TOP = 5;                     		// Top of graph
			int LEFT = 5;                    		// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph

			string LABEL_X = "X axis"; 				// Label for X axis
			string LABEL_Y = "Y axis";             	// Label for Y axis

			// Set up for drawing
			Bitmap png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			
			Pen pen = new Pen(Color.LightGreen, 1);
			
			float X_Slot = (float) (0.8 * WIDTH / 10);
			float Y_Slot = (float) (0.8 * HEIGHT / 10);
			float X = 0;
			float Y = 0;
			float X_0 = (float) (WIDTH * 0.1);
			float X_1 = (float) (WIDTH * 0.9);
			float Y_0 = (float) (HEIGHT * 0.1);
			float Y_1 = (float) (HEIGHT * 0.9);
			float X_Unit = 1;
			float Y_Unit = 0;
			
			if (sampleBitMono) {
				Y_Unit = (float) (0.8 * HEIGHT/ 256);
				for (int i = 0; i < data.Length; i++) {
					data[i] = data[i] + 128;
				}
			} else {
				Y_Unit = (float) (0.8 * HEIGHT/ 65536);
				for (int i = 0; i < data.Length; i++) {
					data[i] = data[i] + 32768;
				}
			}
			
			g.Clear(Color.LightGray);
			g.DrawLine(Pens.Black, X_0, Y_0, X_0, Y_1);
			g.DrawLine(Pens.Black, X_0, Y_1, X_1, Y_1);
			
			for (int i = 1; i < 10; i++) {
				g.DrawLine(Pens.DarkGray, X_0, Y_0 + (i * Y_Slot), X_1, Y_0 + (i * Y_Slot));
				g.DrawLine(Pens.DarkGray, X_0 + (i * X_Slot), Y_0, X_0 + (i * X_Slot), Y_1);
			}

			pen.Width = 2.0F;
			g.DrawLine(pen, X_0, Y_0 + (5 * Y_Slot), X_1, Y_0 + (5 * Y_Slot));
			g.DrawLine(pen, X_0 + (5 * X_Slot), Y_0, X_0 + (5 * X_Slot), Y_1);
			
			X_Unit = (float) (0.8 * WIDTH / data.Length);
			
			PointF[] pointArray = new PointF[data.Length];
			for (int i = 0; i < data.Length; i++) {
				X = X_0 + (i * X_Unit);
				Y = Y_1 - (data[i] * Y_Unit);
				pointArray[i] = new PointF(X, Y);
			}
			
			g.DrawLines(Pens.DarkBlue, pointArray);

			return png;
		}
		
		/*
		 * Converted from a Java Class forom jMusic API version 1.4, February 2003.
		 * 
		 * Copyright (C) 2000 Andrew Sorensen & Andrew Brown
		 * 
		 * This program is free software; you can redistribute it and/or modify
		 * it under the terms of the GNU General Public License as published by
		 * the Free Software Foundation; either version 2 of the License, or any
		 * later version.
		 */
		public static Bitmap DrawWaveform3(float[] data, Size imageSize, int resolution) {

			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			int TOP = 5;                     		// Top of graph
			int LEFT = 5;                    		// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph

			string LABEL_X = "X axis"; 				// Label for X axis
			string LABEL_Y = "Y axis";             	// Label for Y axis
			
			// Colors
			Color lineColor = ColorTranslator.FromHtml("#C7834C");
			Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
			Color textColor = ColorTranslator.FromHtml("#A9652E");
			//Color sampleColor = ColorTranslator.FromHtml("#A9652E");
			Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
			Color fillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
			Color fillColor = ColorTranslator.FromHtml("#F9C998");
			
			Pen linePen = new Pen(lineColor, 0.5f);
			Pen middleLinePen = new Pen(middleLineColor, 0.5f);
			Pen textPen = new Pen(textColor, 1);
			Pen samplePen = new Pen(sampleColor, 1);
			
			// Set up for drawing
			Bitmap bmp = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(bmp);
			
			int numberOfSamples = data.Length;
			//int resolution = 5; //125 // low resolution (2+) means to zoom into the waveform
			int amplitude = 2; // 1 = normal

			float max = 0.0f;
			float min = 0.0f;

			float drawMax, drawMin, currData;

			int h2 = HEIGHT/2 - 1;
			int position = 0;
			int sampleStart = 0;
			
			// Draw a rectangular box marking the boundaries of the graph
			Rectangle rect = new Rectangle(0, 0, WIDTH, HEIGHT);
			g.DrawRectangle(linePen, rect);
			
			// mid line
			g.DrawLine(middleLinePen, 0, h2, WIDTH, h2);

			// draw wave
			int pixCount = Math.Min(data.Length - resolution, WIDTH * resolution);
			if (resolution == 1) {
				for (int i = sampleStart; i < sampleStart + pixCount; i += resolution) {
					currData = data[i];
					int x1 = position;
					int y1 = (int)(h2 - currData * h2 * amplitude);
					int x2 = position + 1;
					int y2 = (int)(h2 - currData * h2 * amplitude);
					g.DrawLine(linePen, x1, y1, x2, y2);
					position++;
				}
			} else {
				for (int i = sampleStart; i < sampleStart + pixCount; i += resolution) {
					if( i < numberOfSamples ) {
						currData = data[i];
						
						// max and min
						max = 0.0f;
						min = 0.0f;
						for( int j=0; j< resolution; j++) {
							if (data[i+j] > max) max = data[i+j];
							if (data[i+j] < min) min = data[i+j];
						}
						
						// highest and lowest curve values
						// disabled - does not look good?!
						/*
						if (resolution > 8) {
							drawMax = Math.Max(currData, data[i+resolution]);
							drawMin = Math.Min(currData, data[i+resolution]);
							
							if (max > 0.0f) {
								g.DrawLine(samplePen, position, (int)(h2 - drawMax * h2 * amplitude), position, (int)(h2 - max * h2 * amplitude));
							}
							
							if (min < 0.0f) {
								g.DrawLine(samplePen, position, (int)(h2 - drawMin * h2 * amplitude), position, (int)(h2 - min * h2 * amplitude));
							}
						}
						 */
						
						// draw wave
						int x1 = position++;
						int y1 = (int)(h2 - currData * h2 * amplitude);
						int x2 = position;
						int y2 = (int)(h2 - data[i+resolution] * h2 * amplitude);
						g.DrawLine(samplePen, x1, y1, x2, y2);
					}
				}
			}
			return bmp;
		}
		
	}
}
