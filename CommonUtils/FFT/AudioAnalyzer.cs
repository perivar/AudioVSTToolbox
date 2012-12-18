﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing.Extended;
using Lomont;
using CommonUtils;

namespace CommonUtils.FFT
{
	/// <summary>
	/// Description of FFTUtils.
	/// </summary>
	public static class AudioAnalyzer
	{
		public static double LogBase = Math.E;
		
		/*
		http://stackoverflow.com/questions/7735036/naudio-frequency-band-intensity
		In the case of the frequency axis you will probably want to group
		your bins into bands, which might each be an octave
		(2:1 frequency range), or more commonly for higher resolution,
		third octave.
		So if you just want 10 "bars" then you might use the following
		octave bands:
		
		   25 -    50 Hz
		   50 -   100 Hz
		  100 -   200 Hz
		  200 -   400 Hz
		  400 -   800 Hz
		  800 -  1600 Hz
		 1600 -  3200 Hz
		 3200 -  6400 Hz
		 6400 - 12800 Hz
		12800 - 20000 Hz
		 */
		
		/// <summary>
		/// Get logarithmically spaced indices
		/// </summary>
		/// <param name = "sampleRate">Signal's sample rate</param>
		/// <param name = "minFreq">Min frequency</param>
		/// <param name = "maxFreq">Max frequency</param>
		/// <param name = "logBins">Number of logarithmically spaced bins</param>
		/// <param name = "fftSize">FFT Size</param>
		/// <param name = "logBase">Log base of the logarithm to be spaced</param>
		/// <returns>Gets an array of indexes</returns>
		public static void GetLogFrequenciesIndex(double sampleRate, double minFreq, double maxFreq, int logBins, int fftSize, double logBase, out int[] indexes, out float[] frequencies)
		{
			GenerateLogFrequencies(sampleRate, minFreq, maxFreq, logBins, fftSize, logBase, out indexes, out frequencies);
		}
		
		/// <summary>
		/// Get logarithmically spaced indices
		/// </summary>
		/// <param name = "sampleRate">Signal's sample rate</param>
		/// <param name = "minFreq">Min frequency</param>
		/// <param name = "maxFreq">Max frequency</param>
		/// <param name = "logBins">Number of logarithmically spaced bins</param>
		/// <param name = "fftSize">FFT Size</param>
		/// <param name = "logarithmicBase">Logarithm base</param>
		private static void GenerateLogFrequencies(double sampleRate, double minFreq, double maxFreq, int logBins, int fftSize, double logarithmicBase, out int[] indexes, out float[] frequencies)
		{
			double logMin = Math.Log(minFreq, logarithmicBase);
			double logMax = Math.Log(maxFreq, logarithmicBase);
			double delta = (logMax - logMin)/ logBins;

			indexes = new int[logBins + 1];
			frequencies = new float[logBins + 1];
			double accDelta = 0;
			for (int i = 0; i <= logBins; ++i)
			{
				float freq = (float) Math.Pow(logarithmicBase, logMin + accDelta);
				frequencies[i] = freq;

				accDelta += delta; // accDelta = delta * i;
				indexes[i] = MathUtils.FreqToIndex(freq, sampleRate, fftSize);
			}
		}
		
		/// <summary>
		/// Logarithmic spacing of a frequency in a linear domain
		/// </summary>
		/// <param name="complexSignal">Spectrum to space</param>
		/// <param name="logBins">number of log bins</param>
		/// <param name="logFrequenciesIndex">array of logarithmically spaced indexes</param>
		/// <returns>Logarithmically spaced signal</returns>
		private static float[] ExtractLogBins(double[] complexSignal, int logBins, int[] logFrequenciesIndex)
		{
			#if SAFE
			if (complexSignal == null)
				throw new ArgumentNullException("complexSignal");
			if (MinFrequency >= MaxFrequency)
				throw new ArgumentException("Minimal frequency cannot be bigger or equal to Maximum frequency");
			if (SampleRate <= 0)
				throw new ArgumentException("sampleRate cannot be less or equal to zero");
			#endif

			float[] sumFreq = new float[logBins]; /*32*/
			for (int i = 0; i < logBins; i++)
			{
				int lowBound = logFrequenciesIndex[i];
				int hiBound = logFrequenciesIndex[i + 1];

				if (hiBound*2 < complexSignal.Length) {
					for (int j = lowBound; j < hiBound; j++)
					{
						double re = complexSignal[2*j];
						double img = complexSignal[2*j + 1];
						sumFreq[i] += (float) (Math.Sqrt(re*re + img*img));
					}
				}
				sumFreq[i] = sumFreq[i]/(hiBound - lowBound);
			}
			return sumFreq;
		}
		
		public static float[][] CreateLogSpectrogramLomont(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap, int logBins, int[] logFrequenciesIndex, float[] logFrequencies)
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
				for (int j = 0; j < fftWindowsSize; j++)
				{
					// Weight by Hann Window
					complexSignal[2*j] = (double) (windowArray[j] * samples[i * fftOverlap + j]);
					
					// need to clear out as fft modifies buffer (phase)
					complexSignal[2*j + 1] = 0;
				}

				// FFT transform for gathering the spectrum
				fft.FFT(complexSignal, true);

				frames[i] = ExtractLogBins(complexSignal, logBins, logFrequenciesIndex);
			}
			return frames;
		}

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

					// get the magnitude spectrum
					//normalize img/re part
					//re /= (float) fftWindowsSize/2;
					//img /= (float) fftWindowsSize/2;
					//band[j] = (float) Math.Sqrt(re*re + img*img);
					
					band[j] = (float) Math.Sqrt(re*re + img*img) * 4;
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
			
			// width of the segment - i.e. split the file into X time slots (numberOfSegments) and do analysis on each slot
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			float[][] frames = new float[numberOfSegments][];
			
			// even - Re, odd - Img
			float[] complexSignal = new float[2*fftWindowsSize];
			for (int i = 0; i < numberOfSegments; i++)
			{
				// apply Hanning Window
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
					// i.e. the magnitude spectrum
					//re /= (float) fftWindowsSize/2;
					//img /= (float) fftWindowsSize/2;
					//band[j] = (float) Math.Sqrt(re*re + img*img);
					//band[j] = (float) Math.Sqrt(re*re + img*img) * 2/fftWindowsSize;
					
					band[j] = (float) Math.Sqrt(re*re + img*img) * 4/fftWindowsSize;
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
			for (int j = 0; (j < fftWindowsSize) && (samples.Length > fftOverlap+j); j++)
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
				// i.e. the magnitude spectrum
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
				// i.e. the magnitude spectrum
				band[j] = (float) Math.Sqrt(re*re + img*img) * 4/fftWindowsSize;
			}
			return band;
		}
		
		/// <summary>
		/// Utility method to return a spectrum image based on audio data
		/// </summary>
		/// <param name="audioData"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="sampleRate"></param>
		/// <param name="fftWindowsSize"></param>
		/// <param name="fftOverlap"></param>
		/// <returns></returns>
		public static Bitmap GetSpectrumImage(float[] audioData, int width, int height, double sampleRate, int fftWindowsSize, int fftOverlap, float minFrequency, float maxFrequency) {

			float[] mag;
			float[] freq;
			float foundMaxFreq, foundMaxDecibel;
			float[] spectrumData = AudioAnalyzer.CreateSpectrumAnalysisLomont(audioData, sampleRate, fftWindowsSize, fftOverlap);
			AudioAnalyzer.PrepareSpectrumAnalysis(spectrumData, sampleRate, fftWindowsSize, fftOverlap, out mag, out freq, out foundMaxFreq, out foundMaxDecibel);
			Bitmap spectrum = AudioAnalyzer.GetSpectrumImage(ref mag, ref freq, new Size(1200, 600), minFrequency, maxFrequency, foundMaxDecibel, foundMaxFreq);
			return spectrum;
		}

		public static void PrepareSpectrumAnalysis(float[] spectrumData, double sampleRate, int fftWindowsSize, int fftOverlap,
		                                           out float[] m_mag, out float[] m_freq,
		                                           out float foundMaxFrequency, out float foundMaxDecibel) {

			int spectrumDataLength = spectrumData.Length; // 1024 - half the fftWindowsSize (2048)
			//int numberOfSamples = fftOverlap + fftWindowsSize;
			//double seconds = MathUtils.ConvertToTime(sampleRate, numberOfSamples);

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

				m_mag[i] = MathUtils.AmplitudeToDecibel(spectrumData[i]);
				m_freq[i] = MathUtils.IndexToFreq(i, spectrumDataLength, sampleRate, fftWindowsSize);
			}
			
			// store the max findings
			foundMaxDecibel = MathUtils.AmplitudeToDecibel(spectrumData[maxIndex]);
			foundMaxFrequency = MathUtils.IndexToFreq(maxIndex, spectrumDataLength, sampleRate, fftWindowsSize);
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
		public static Bitmap GetSpectrumImage(ref float[] mag, ref float[] freq,
		                                      Size imageSize,
		                                      float minFrequency = 0, float maxFrequency = 20000,
		                                      float foundMaxDecibel = -1, float foundMaxFrequency = -1)
		{
			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			float MIN_FREQ = minFrequency;  	// Minimum frequency (Hz) on horizontal axis.
			float MAX_FREQ = maxFrequency;		// Maximum frequency (Hz) on horizontal axis.
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
					int note = 0;
					int cents = 0;
					MidiUtils.PitchToMidiNote(foundMaxFrequency, out note, out cents);
					string noteName = MidiUtils.GetNoteName(note, false, true);
					
					//string foundMax = String.Format("Max found: {0}dB @ {1} hz", foundMaxDecibel, foundMaxFrequency);
					string foundMax = String.Format("Max found: {0}dB @ {1} hz ({2} - Note: {3} {4:+#;-#;0} cents)", foundMaxDecibel, foundMaxFrequency, noteName, note, cents);
					
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
		
		
		/// <summary>
		/// Utility method to return spectrogramimage using audio data
		/// </summary>
		/// <param name="audioData"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="sampleRate"></param>
		/// <param name="fftWindowsSize"></param>
		/// <param name="fftOverlap"></param>
		/// <param name="colorPalette"></param>
		/// <param name="doLogScale"></param>
		/// <returns></returns>
		public static Bitmap GetSpectrogramImage(float[] audioData, int width, int height, double sampleRate, int fftWindowsSize, int fftOverlap, ColorUtils.ColorPaletteType colorPalette, bool doLogScale)
		{
			float[][] spectrogram;
			double minFrequency = 27.5;
			double maxFrequency = sampleRate / 2;
			int logBins = height - 2*40; // the margins used
			int[] logFrequenciesIndex = new int[1];
			float[] logFrequencies = new float[1];

			// find the time
			int numberOfSamples = audioData.Length;
			double seconds = numberOfSamples / sampleRate;

			if (!doLogScale) {
				spectrogram = CreateSpectrogramLomont(audioData, sampleRate, fftWindowsSize, fftOverlap);
			} else {
				// calculate the log frequency index table
				GetLogFrequenciesIndex(sampleRate, minFrequency, maxFrequency, logBins, fftWindowsSize, LogBase, out logFrequenciesIndex, out logFrequencies);
				spectrogram = CreateLogSpectrogramLomont(audioData, sampleRate, fftWindowsSize, fftOverlap, logBins, logFrequenciesIndex, logFrequencies);
			}
			
			return GetSpectrogramImage(spectrogram, width, height, seconds*1000, sampleRate, colorPalette, doLogScale, logFrequenciesIndex, logFrequencies);
		}

		/// <summary>
		///   Get a spectrogram of the signal specified at the input
		/// </summary>
		/// <param name = "data">Signal</param>
		/// <param name = "width">Width of the image</param>
		/// <param name = "height">Height of the image</param>
		/// <returns>Spectral image of the signal</returns>
		/// <remarks>
		///   X axis - time
		///   Y axis - frequency
		///   Color - magnitude level of corresponding band value of the signal
		/// </remarks>
		public static Bitmap GetSpectrogramImage(float[][] spectrum, int width, int height)
		{
			#if SAFE
			if (width < 0)
				throw new ArgumentException("width should be bigger than 0");
			if (height < 0)
				throw new ArgumentException("height should be bigger than 0");
			#endif
			Bitmap image = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(image);
			/*Fill Back color*/
			using (Brush brush = new SolidBrush(Color.Black))
			{
				graphics.FillRectangle(brush, new Rectangle(0, 0, width, height));
			}
			int bands = spectrum[0].Length;
			double max = spectrum.Max((b) => b.Max((v) => Math.Abs(v)));
			double deltaX = (double) (width - 1)/spectrum.Length; /*By how much the image will move to the left*/
			double deltaY = (double) (height - 1)/(bands + 1); /*By how much the image will move upward*/
			int prevX = 0;
			for (int i = 0, n = spectrum.Length; i < n; i++)
			{
				double x = i*deltaX;
				if ((int) x == prevX) continue;
				for (int j = 0, m = spectrum[0].Length; j < m; j++)
				{
					Color color = ColorUtils.ValueToBlackWhiteColor(spectrum[i][j], max/10);
					image.SetPixel((int) x, height - (int) (deltaY*j) - 1, color);
				}
				prevX = (int) x;
			}
			return image;
		}
		
		/// <summary>
		/// Get a spectrogram of the signal specified at the input
		/// </summary>
		/// <param name="spectrum">Signal</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="milliseconds">Time in ms</param>
		/// <param name="sampleRate">Sample rate in hz</param>
		/// <param name="colorPalette">Specify to color palette</param>
		/// <param name="doLogScale">log scale or not?</param>
		/// <param name="logFrequenciesIndex">log frequency index array</param>
		/// <param name="logFrequencies">log frequency array</param>
		/// <remarks>
		///   X axis - time
		///   Y axis - frequency
		///   Color - magnitude level of corresponding band value of the signal
		/// <returns>Spectral image of the signal</returns>
		public static Bitmap GetSpectrogramImage(float[][] spectrum, int width, int height, double milliseconds, double sampleRate, ColorUtils.ColorPaletteType colorPalette, bool doLogScale, int[] logFrequenciesIndex, float[] logFrequencies)
		{
			if (width < 0)
				throw new ArgumentException("width should be bigger than 0");
			if (height < 0)
				throw new ArgumentException("height should be bigger than 0");

			bool drawLabels = true;
			float minDb = -90.0f; // -80.0f also works good
			float maxDb = 10.0f; // with the current color palettes 10.0f works well
			
			// Basic constants
			int TOTAL_HEIGHT = height;    // Height of graph
			int TOTAL_WIDTH = width;      // Width of graph

			int TOP = 40;                    // Top of graph
			int LEFT = 60;                   // Left edge of graph
			int HEIGHT = height-2*TOP;		// Height of graph
			int WIDTH = width-2*LEFT;     	// Width of graph
			string LABEL_X = "Time (ms)"; 		// Label for X axis
			string LABEL_Y = "Frequency (Hz)";  // Label for Y axis
			
			float MAX_FREQ = (float) sampleRate / 2;	// Maximum frequency (Hz) on vertical axis.
			float MIN_FREQ = 27.5f;        	// Minimum frequency (Hz) on vertical axis.
			float FREQ_STEP = 1000;        	// Interval between ticks (dB) on vertical axis.

			// if the max frequency gets lower than ... lower the frequency step
			if (MAX_FREQ < 20000) {
				FREQ_STEP = (float) MathUtils.GetNicerNumber(MAX_FREQ / 20);
			}
			
			// Derived constants
			int BOTTOM = TOTAL_HEIGHT-TOP;                   			// Bottom of graph
			float FREQTOPIXEL = (float) HEIGHT/(MAX_FREQ-MIN_FREQ);    	// Pixels/Hz
			
			float MIN_TIME = 0.0f;
			float MAX_TIME = (float) milliseconds;
			if (MAX_TIME == 0) MAX_TIME = 1000;

			// Interval between ticks (time) on horizontal axis.
			float TIME_STEP = (float) MathUtils.GetNicerNumber(MAX_TIME / 20);
			float TIMETOPIXEL = (float) WIDTH/(MAX_TIME-MIN_TIME); 	// Pixels/second
			
			// Colors
			// black, gray, white style
			Color lineColor = ColorTranslator.FromHtml("#BFBFBF");
			Color middleLineColor = ColorTranslator.FromHtml("#BFBFBF");
			Color labelColor = ColorTranslator.FromHtml("#FFFFFF");
			Color tickColor = ColorTranslator.FromHtml("#BFBFBF");
			Color fillOuterColor = ColorTranslator.FromHtml("#000000");
			Color fillColor = ColorTranslator.FromHtml("#000000");
			
			Bitmap fullImage = new Bitmap(TOTAL_WIDTH, TOTAL_HEIGHT);
			Graphics g = Graphics.FromImage(fullImage);
			
			Pen linePen = new Pen(lineColor, 0.5f);
			Pen middleLinePen = new Pen(middleLineColor, 0.5f);
			Pen labelPen = new Pen(labelColor, 1);
			Pen tickPen = new Pen(tickColor, 1);

			// Draw a rectangular box marking the boundaries of the graph
			Rectangle rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
			Brush fillBrushOuter = new SolidBrush(fillOuterColor);
			g.FillRectangle(fillBrushOuter, rectOuter);
			
			// Create rectangle.
			Rectangle rect = new Rectangle(LEFT, TOP, WIDTH, HEIGHT);
			Brush fillBrush = new SolidBrush(fillColor);
			g.FillRectangle(fillBrush, rect);
			g.DrawRectangle(linePen, rect);
			
			// Label for horizontal axis
			Font drawLabelFont = new Font("Arial", 8);
			SolidBrush drawLabelBrush = new SolidBrush(labelPen.Color);
			if (drawLabels) {
				SizeF drawLabelTextSize = g.MeasureString(LABEL_X, drawLabelFont);
				g.DrawString(LABEL_X, drawLabelFont, drawLabelBrush, (TOTAL_WIDTH/2) - (drawLabelTextSize.Width/2), TOTAL_HEIGHT - drawLabelFont.GetHeight(g) - 5);
			}
			
			float y = 0;
			float yMiddle = 0;
			float x = 0;
			float xMiddle = 0;

			if (!doLogScale) {
				// LINEAR SCALE
				
				// Tick marks on the vertical axis
				for ( float freqTick = MIN_FREQ; freqTick <= MAX_FREQ; freqTick += FREQ_STEP )
				{
					// draw horozontal main line
					y = BOTTOM - FREQTOPIXEL*(freqTick-MIN_FREQ);
					if (y < BOTTOM && y > TOP+1) {
						g.DrawLine(linePen, LEFT-2, y, LEFT+WIDTH+2, y);
					}

					// draw horozontal middle line (between the main lines)
					yMiddle = y-(FREQTOPIXEL*FREQ_STEP)/2;
					if (yMiddle > TOP && yMiddle < HEIGHT+TOP) {
						g.DrawLine(middleLinePen, LEFT, yMiddle, LEFT+WIDTH, yMiddle);
					}

					if ( freqTick != MAX_FREQ )
					{
						// Numbers on the tick marks
						Font drawFont = new Font("Arial", 8);
						SolidBrush drawBrush = new SolidBrush(tickPen.Color);
						
						// left
						g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, LEFT - 33, y - drawFont.GetHeight(g)/2);

						// right
						g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, WIDTH + LEFT + 4, y - drawFont.GetHeight(g)/2);
					}
				}
			} else {
				// LOG SCALE
				for (int i = 0; i < logFrequencies.Length; i+=20)
				{
					float freqTick = logFrequencies[i];
					y = BOTTOM - i;

					// draw horozontal main line
					if (y < BOTTOM && y > TOP+1) {
						g.DrawLine(linePen, LEFT-2, y, LEFT+WIDTH+2, y);
					}
					
					// Numbers on the tick marks
					Font drawFont = new Font("Arial", 8);
					SolidBrush drawBrush = new SolidBrush(tickPen.Color);
					
					// left
					g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, LEFT - 33, y - drawFont.GetHeight(g)/2);

					// right
					g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, WIDTH + LEFT + 4, y - drawFont.GetHeight(g)/2);
				}
			}
			
			
			if (drawLabels) {
				// Label for vertical axis
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				g.TranslateTransform(g.VisibleClipBounds.Width, 0);
				g.RotateTransform(270);
				g.DrawString(LABEL_Y, drawLabelFont, drawLabelBrush, -(TOTAL_HEIGHT/2), -TOTAL_WIDTH + 5, format);
				g.ResetTransform();
			}
			
			// Tick marks on the horizontal axis
			for ( float timeTick = MIN_TIME; timeTick <= MAX_TIME; timeTick += TIME_STEP )
			{
				// draw vertical main line
				x = LEFT + TIMETOPIXEL*(timeTick-MIN_TIME);
				if (x > LEFT  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM+2, x, TOP-2);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < WIDTH+LEFT) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// Numbers on the tick marks
					Font drawFont = new Font("Arial", 8);
					SolidBrush drawBrush = new SolidBrush(tickPen.Color);
					SizeF drawTimeTickTextSize = g.MeasureString("" + timeTick, drawFont);

					// top
					g.DrawString("" + timeTick, drawFont, drawBrush, x-(drawTimeTickTextSize.Width/2), TOP - 15);

					// bottom
					g.DrawString("" + timeTick, drawFont, drawBrush, x-(drawTimeTickTextSize.Width/2), BOTTOM + 2);
				}
			}
			
			// draw spectrogram
			Bitmap spectrogram = new Bitmap(WIDTH, HEIGHT);
			
			// calculate min and max
			double max = spectrum.Max((b) => b.Max((v) => Math.Abs(v)));
			double min = spectrum.Min((b) => b.Min((v) => Math.Abs(v)));

			int numberOfSamplesX = spectrum.Length; 	// time
			int numberOfSamplesY = spectrum[0].Length; 	// hz
			
			double deltaX = (double) (WIDTH - 1)/(numberOfSamplesX); 	// By how much the image will move to the left
			double deltaY = (double) (HEIGHT- 1)/(numberOfSamplesY); 	// By how much the image will move upward
			
			int prevX = 0;
			Color prevColor = Color.Black;
			for (int i = 0; i < numberOfSamplesX; i++)
			{
				double xCoord = i*deltaX;
				if ((int) xCoord == prevX) continue;
				for (int j = 0; j < numberOfSamplesY; j++)
				{
					float amplitude = spectrum[i][j];
					Color colorbw = Color.Black;
					if (amplitude > 0) {
						float dB = MathUtils.AmplitudeToDecibel(amplitude, minDb, maxDb);
						int colorval = (int) MathUtils.ConvertAndMainainRatio(dB, minDb, maxDb, 0, 255); // 255 is full brightness, and good for REW colors (for SOX 220 is good, and for PHOTOSOUNDER 245 seems good)
						colorbw = Color.FromArgb(colorval, colorval, colorval);
						//colorbw = ValueToBlackWhiteColor(amplitude, max*0.010);
						prevColor = colorbw;
					} else {
						colorbw = prevColor;
					}
					spectrogram.SetPixel((int) xCoord + 1, HEIGHT - (int) (deltaY*j) - 1, colorbw);
				}
				prevX = (int) xCoord;
			}
			
			if (colorPalette != ColorUtils.ColorPaletteType.BLACK_AND_WHITE) {
				spectrogram = ColorUtils.Colorize(spectrogram, 255, colorPalette);
			}
			
			// add the spectrogram to the full image
			g.DrawImage(spectrogram, LEFT, TOP);
			
			return fullImage;
		}

		// More waveform links:
		// https://github.com/aalin/canvas_waveform
		// http://www.hisschemoller.com/2010/mp3-wave-display/
		// http://www.marinbezhanov.com/web-development/14/actionscript-3-sound-extract-demystified-or-how-to-draw-a-waveform-in-flash/

		// http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
		// TODO: startPosition is NOT YET SUPPORTED
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int resolution, int amplitude, int startPosition, double sampleRate, bool drawRaw=false) {

			// Basic constants
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			int TOP = 5;                     		// Top of graph
			int LEFT = 5;                    		// Left edge of graph
			if (drawRaw) {
				TOP = 0;                     		// Top of graph
				LEFT = 0;                    		// Left edge of graph
			}
			int HEIGHT = imageSize.Height-2*TOP;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT;     // Width of graph
			
			float MIN_AMPLITUDE = -1.0f;
			float MAX_AMPLITUDE = 1.0f;
			float AMPLITUDE_STEP = 0.25f;
			
			string LABEL_X = "Time (ms)"; 				// Label for X axis
			string LABEL_Y = "Amplitude";             	// Label for Y axis
			
			bool drawLabels = false;
			bool drawRoundedRectangles = true;
			bool displayInformationBox = true;
			bool displayTime = true;
			bool useAverages = false; // averages draws a "filled" waveform

			if (drawRaw) {
				drawLabels = false;
				drawRoundedRectangles = false;
				displayInformationBox = false;
				displayTime = false;
			}
			
			// Colors
			Color lineColor = ColorTranslator.FromHtml("#C7834C");
			Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
			Color textColor = ColorTranslator.FromHtml("#A9652E");
			Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
			Color fillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
			Color fillColor = ColorTranslator.FromHtml("#F9C998");
			Color textInfoBoxColor = ColorTranslator.FromHtml("#4C2F1A");
			Color textInfoBoxBgColor = ColorTranslator.FromHtml("#F7DECA");
			
			// Derived constants
			int CENTER = TOTAL_HEIGHT / 2;
			int RIGHT = WIDTH;
			int BOTTOM = TOTAL_HEIGHT-TOP;                   		// Bottom of graph

			int numberOfSamples = 0;
			float[] data = null;
			if (audioData != null && audioData.Length > 0) {
				
				// shift the start position
				//if (startPosition > 0 ) {
				//	data = new float[audioData.Length - startPosition];
				//	Array.Copy(audioData, startPosition, data, 0, audioData.Length - startPosition);
				//} else {
				data = audioData;
				//}
				
				numberOfSamples = data.Length;
			}
			double milliseconds = numberOfSamples / sampleRate * 1000;
			
			//int amplitude = 1; // 1 = normal
			//resolution = 1; //low resolution (2+) means to zoom into the waveform

			float sampleToPixel = (float) numberOfSamples / (float) WIDTH;
			if (resolution != 0 && resolution < sampleToPixel) {
				sampleToPixel = resolution;
			}

			//float MAX_TIME = (float) milliseconds;
			float MAX_TIME = (float) (sampleToPixel * WIDTH / sampleRate * 1000);
			if (MAX_TIME == 0) MAX_TIME = 100;

			float TIME_STEP = (float) MathUtils.GetNicerNumber(MAX_TIME / 10);
			
			float MIN_TIME = 0.0f;
			//if (startPosition > 0) {
			//	MIN_TIME = (float) (startPosition / sampleRate * 1000);
			//}
			
			float AMPLITUDETOPIXEL = (float) HEIGHT/(MAX_AMPLITUDE-MIN_AMPLITUDE); 	// Pixels/tick
			float TIMETOPIXEL = (float) WIDTH/(MAX_TIME-MIN_TIME); 					// Pixels/second
			
			// Set up for drawing
			Bitmap png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			ExtendedGraphics eg = new ExtendedGraphics(g);
			
			Pen linePen = new Pen(lineColor, 0.5f);
			Pen middleLinePen = new Pen(middleLineColor, 0.5f);
			Pen textPen = new Pen(textColor, 1.0f);
			Pen samplePen = new Pen(sampleColor, 1.0f);
			Pen infoBoxPen = new Pen(textInfoBoxColor, 1.0f);

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
			for ( float amplitudeTick = MIN_AMPLITUDE; amplitudeTick <= MAX_AMPLITUDE; amplitudeTick += AMPLITUDE_STEP )
			{
				// draw horozontal main line
				y = BOTTOM - AMPLITUDETOPIXEL*(amplitudeTick-MIN_AMPLITUDE);
				if (y < BOTTOM && y > TOP+1) {
					g.DrawLine(linePen, LEFT, y, LEFT+WIDTH, y);
				}

				// draw horozontal middle line (between the main lines)
				yMiddle = y-(AMPLITUDETOPIXEL*AMPLITUDE_STEP)/2;
				if (yMiddle > 0) {
					g.DrawLine(middleLinePen, LEFT, yMiddle, LEFT+WIDTH, yMiddle);
				}

				if ( amplitudeTick != MAX_AMPLITUDE )
				{
					// Numbers on the tick marks
					Font drawFont = new Font("Arial", 8);
					SolidBrush drawBrush = new SolidBrush(textPen.Color);
					g.DrawString("" + amplitudeTick, drawFont, drawBrush, LEFT+5, y - drawFont.GetHeight(g) -2);
				}
			}
			
			if (drawLabels) {
				// Label for vertical axis
				g.DrawString(LABEL_Y, drawLabelFont, drawLabelBrush, 1, TOP + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
			}
			
			// Tick marks on the horizontal axis
			for ( float timeTick = MIN_TIME; timeTick <= MAX_TIME; timeTick += TIME_STEP )
			{
				// draw vertical main line
				x = LEFT + TIMETOPIXEL*(timeTick-MIN_TIME);
				if (x > LEFT  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM, x, TOP);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < TOTAL_WIDTH) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// Numbers on the tick marks
					Font drawFont = new Font("Arial", 8);
					SolidBrush drawBrush = new SolidBrush(textPen.Color);
					g.DrawString("" + timeTick + " ms", drawFont, drawBrush, x, TOP +2);
				}
			}

			if (displayTime) {
				string displayTimeString = String.Format("Duration: {0} samples @ {1:0.0000} ms", numberOfSamples, milliseconds);
				SizeF displayTimeStringTextSize = g.MeasureString(displayTimeString, drawLabelFont);
				g.DrawString(displayTimeString, drawLabelFont, drawLabelBrush, TOTAL_WIDTH - displayTimeStringTextSize.Width - 10, TOTAL_HEIGHT - drawLabelFont.GetHeight(g) - 10);
			}
			
			// draw middle line
			g.DrawLine(middleLinePen, LEFT, CENTER, WIDTH, CENTER);
			
			
			// Draw waveform
			if (data != null && data.Length > 0) {
				if (sampleToPixel >= 1) {
					// the number of samples are greater than the available drawing space (i.e. greater than the number of pixles in the X-Axis)

					float xPrev = 0;
					float yPrev = 0;
					float yAxis = 0;

					int yMax = 0;
					int yMin = 0;
					int yMaxPrev = 0;
					int yMinPrev = 0;
					bool firstPoint = true;
					for (int xAxis = 0; xAxis < WIDTH; xAxis++)
					{
						// determine start and end points within WAV (for this single pixel on the X axis)
						int start 	= (int)((float)(xAxis) 		* sampleToPixel);
						int end 	= (int)((float)(xAxis + 1) 	* sampleToPixel);
						
						// reset the min and max values
						yMax = 0;
						yMin = 0;
						if (useAverages) {
							// determine the average min and max values within this specific range
							float posAvg = 0, negAvg = 0;
							averages(data, start, end, out posAvg, out negAvg);

							yMax = TOP + HEIGHT - (int)((posAvg * amplitude + 1) * 0.5 * HEIGHT);
							yMin = TOP + HEIGHT - (int)((negAvg * amplitude + 1) * 0.5 * HEIGHT);
							
							g.DrawLine(samplePen, xAxis + LEFT, yMin, xAxis + LEFT, yMax);
						} else {
							// determine the min and max values within this specific range
							float min = float.MaxValue;
							float max = float.MinValue;
							for (int i = start; i <= end; i++)
							{
								if (i < data.Length) {
									float val = data[i];
									min = val < min ? val : min;
									max = val > max ? val : max;
								}
							}
							
							yMax = TOP + HEIGHT - (int)((max * amplitude + 1) * 0.5 * HEIGHT);
							yMin = TOP + HEIGHT - (int)((min * amplitude + 1) * 0.5 * HEIGHT);

							// make sure that we draw something
							if (yMin == yMax) {
								yMin += 1;
							}
							yAxis = yMax;

							// draw waveform
							// If it's the first point
							if ( firstPoint )
							{
								// Move to the point
								xPrev = xAxis;
								yPrev = yAxis;
								
								yMaxPrev = yMax;
								yMinPrev = yMin;
								
								firstPoint = false;
							}
							else
							{
								if (TIMETOPIXEL > 10) {
									// For smaller resolution, Draw line from the previous point
									g.DrawLine(samplePen, xPrev + LEFT, yPrev, xAxis + LEFT, yAxis);
									
									// Try to smooth the lines by using the previous max value
									//g.DrawLine(samplePen, xPrev + LEFT, yMaxPrev, xAxis + LEFT, yMin);
								} else {
									// use yMax and yMin
									// original from example: http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
									// basically don't care about previous x or y, but draw vertical lines
									// from y min to y max value
									g.DrawLine(samplePen, xAxis + LEFT, yMin, xAxis + LEFT, yMax);
								}

								// store values to next iteration
								xPrev = xAxis;
								yPrev = yAxis;
								
								yMaxPrev = yMax;
								yMinPrev = yMin;
							}
						}
					}
				} else {
					// the number of samples are lower than the available drawing space (i.e. less than the number of pixles in the X-Axis)
					float mult_x = (float) WIDTH / (numberOfSamples - 1);
					
					List<Point> ps = new List<Point>();
					for (int i = 0; i < data.Length; i++)
					{
						x = (i * mult_x) + LEFT;
						y = TOP + HEIGHT - (int)((data[i] * amplitude + 1) * 0.5 * HEIGHT);
						Point p = new Point((int)x, (int)y);
						ps.Add(p);
					}

					if (ps.Count > 0)
					{
						g.DrawLines(samplePen, ps.ToArray());
					}
				}
			}
			
			// draw right upper box
			if (displayInformationBox) {
				Font drawInfoBoxFont = new Font("Arial", 8);
				SolidBrush drawInfoBoxBrush = new SolidBrush(infoBoxPen.Color);
				
				string infoBoxLine1Text = String.Format("Resolution: {0}", resolution);
				string infoBoxLine2Text = String.Format("SampleToPixel Orig: {0:0.000} => New: {1:0.000}", (float) numberOfSamples / WIDTH, sampleToPixel);
				string infoBoxLine3Text = String.Format("Time (Min->Max): {0} -> {1}", MIN_TIME, MAX_TIME);
				string infoBoxLine4Text = String.Format("Timestep: {0}, TimeToPixel: {1}", TIME_STEP, TIMETOPIXEL);

				// get box width
				int infoBoxMargin = 5;
				List<float> textLineSizes = new List<float>();
				textLineSizes.Add(g.MeasureString(infoBoxLine1Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine2Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine3Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine4Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(150.0f); // info box minimum width
				
				float infoBoxLineTextWidth = 0.0f;
				float minWidth = 0.0f;
				MathUtils.ComputeMinAndMax(textLineSizes.ToArray(), out minWidth, out infoBoxLineTextWidth);

				int infoBoxWidth = (int) infoBoxLineTextWidth;
				
				float infoBoxLineTextHeight = drawInfoBoxFont.GetHeight(g);
				int infoBoxHeight = (int) (infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*4);
				
				Rectangle rectInfoBox = new Rectangle(WIDTH - infoBoxWidth - 20, 30, infoBoxWidth, infoBoxHeight);
				Brush fillBrushInfoBox = new SolidBrush(textInfoBoxBgColor);
				g.FillRectangle(fillBrushInfoBox, rectInfoBox);
				g.DrawRectangle(linePen, rectInfoBox);
				
				g.DrawString(infoBoxLine1Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin);
				g.DrawString(infoBoxLine2Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin));
				g.DrawString(infoBoxLine3Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*2);
				g.DrawString(infoBoxLine4Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*3);
			}
			
			return png;
		}

		private static void averages(float[] data, int startIndex, int endIndex, out float posAvg, out float negAvg)
		{
			posAvg = 0.0f;
			negAvg = 0.0f;

			int posCount = 0, negCount = 0;

			for (int i = startIndex; i < endIndex; i++)
			{
				if (data[i] > 0)
				{
					posCount++;
					posAvg += data[i];
				}
				else
				{
					negCount++;
					negAvg += data[i];
				}
			}

			if (posCount != 0) {
				posAvg /= posCount;
			}
			if (negCount != 0) {
				negAvg /= negCount;
			}
		}

	}
}
