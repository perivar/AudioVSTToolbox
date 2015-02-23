using System;
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
	/// AudioAnalyzer Class. Contains methods for generating waveforms, spectrum graphs and spectrograms
	/// perivar@nerseth.com
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
		
		#region CreateLogSpectrogram
		/// <summary>
		/// Get logarithmically spaced indices
		/// </summary>
		/// <param name = "sampleRate">Signal's sample rate</param>
		/// <param name = "minFreq">Min frequency</param>
		/// <param name = "maxFreq">Max frequency</param>
		/// <param name = "logBins">Number of logarithmically spaced bins</param>
		/// <param name = "fftSize">FFT Size</param>
		/// <param name = "logBase">Log base of the logarithm to be spaced</param>
		/// <param name = "indexes">output parameter to return the indexes</param>
		/// <param name = "frequencies">output parameter to return the frequencies</param>
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
		/// <param name = "logarithmicBase">Logarithmic base</param>
		/// <param name = "indexes">output parameter to return the indices</param>
		/// <param name = "frequencies">output parameter to return the frequencies</param>
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
			var sumFreq = new float[logBins]; /*32*/
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
		
		/// <summary>
		/// Generate a spectrogram array spaced logarithmically
		/// </summary>
		/// <param name="samples">audio data</param>
		/// <param name="fftWindowsSize">fft window size</param>
		/// <param name="fftOverlap">overlap</param>
		/// <param name="logBins">number of log bins along the frequency axis</param>
		/// <param name="logFrequenciesIndex">array of log frequency indexes</param>
		/// <param name="logFrequencies">array of log frequencies</param>
		/// <returns>log spectrogram jagged array</returns>
		public static float[][] CreateLogSpectrogramLomont(float[] samples, int fftWindowsSize, int fftOverlap, int logBins, int[] logFrequenciesIndex, float[] logFrequencies)
		{
			var fft = new LomontFFT();
			
			int numberOfSamples = samples.Length;

			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			
			// width of the segment - e.g. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			var frames = new float[numberOfSegments][];
			
			// even - Re, odd - Img
			var complexSignal = new double[2*fftWindowsSize];
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
		#endregion

		#region CreateSpectrogram
		/// <summary>
		/// Generate a spectrogram array spaced linearily
		/// </summary>
		/// <param name="samples">audio data</param>
		/// <param name="fftWindowsSize">fft window size</param>
		/// <param name="fftOverlap">overlap in number of samples (normaly half of the fft window size) [low number = high overlap]</param>
		/// <returns>spectrogram jagged array</returns>
		public static float[][] CreateSpectrogramLomont(float[] samples, int fftWindowsSize, int fftOverlap)
		{
			var fft = new LomontFFT();
			
			int numberOfSamples = samples.Length;

			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			
			// width of the segment - e.g. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			var frames = new float[numberOfSegments][];
			
			// even - Re, odd - Img
			var complexSignal = new double[2*fftWindowsSize];
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
				var band = new float[fftWindowsSize/2];
				for (int j = 0; j < fftWindowsSize/2; j++)
				{
					double re = complexSignal[2*j];
					double img = complexSignal[2*j + 1];
					
					band[j] = (float) Math.Sqrt(re*re + img*img) * 4;
				}
				frames[i] = band;
			}
			return frames;
		}
		#endregion
		
		#region CreateSpectrumAnalysis
		/// <summary>
		/// Generate a spectrum graph array spaced linearily
		/// </summary>
		/// <param name="samples">audio data</param>
		/// <param name="fftWindowsSize">fft window size</param>
		/// <returns>spectrum graph array</returns>
		public static float[] CreateSpectrumAnalysisLomont(float[] samples, int fftWindowsSize)
		{
			var fft = new LomontFFT();

			int numberOfSamples = samples.Length;
			
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);

			// even - Re, odd - Img
			var complexSignal = new double[2*fftWindowsSize];

			// apply Hanning Window
			// e.g. take 371 ms each 11.6 ms (2048 samples each 64 samples)
			for (int j = 0; (j < fftWindowsSize) && (samples.Length > j); j++)
			{
				// Weight by Hann Window
				complexSignal[2*j] = (double) (windowArray[j] * samples[j]);
				
				// need to clear out as fft modifies buffer (phase)
				complexSignal[2*j + 1] = 0;
			}

			// FFT transform for gathering the spectrum
			fft.FFT(complexSignal, true);

			var band = new float[fftWindowsSize/2];
			double lengthSqrt = Math.Sqrt(fftWindowsSize);
			for (int j = 0; j < fftWindowsSize/2; j++)
			{
				double re = complexSignal[2*j] * lengthSqrt;
				double img = complexSignal[2*j + 1] * lengthSqrt;
				
				// do the Abs calculation and add with Math.Sqrt(audio_data.Length);
				// i.e. the magnitude spectrum
				band[j] = (float) (Math.Sqrt(re*re + img*img) * lengthSqrt);
			}
			return band;
		}
		#endregion
		
		#region DrawingMethods
		/// <summary>
		/// Utility method to return a spectrum graph image based on audio data
		/// </summary>
		/// <param name="audioData">The audio data (mono)</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="sampleRate">sample rate</param>
		/// <param name="fftWindowsSize">fft windows size</param>
		/// <param name="minFrequency">minimum frequency to show</param>
		/// <param name="maxFrequency">maximum frequency to show</param>
		/// <returns>Spectrum graph image</returns>
		public static Bitmap GetSpectrumImage(float[] audioData, int width, int height, double sampleRate, int fftWindowsSize, float minFrequency, float maxFrequency) {

			float[] mag;
			float[] freq;
			float foundMaxFreq, foundMaxDecibel;
			float[] spectrumData = AudioAnalyzer.CreateSpectrumAnalysisLomont(audioData, fftWindowsSize);
			AudioAnalyzer.PrepareSpectrumAnalysis(spectrumData, sampleRate, fftWindowsSize, out mag, out freq, out foundMaxFreq, out foundMaxDecibel);
			Bitmap spectrum = AudioAnalyzer.GetSpectrumImage(ref mag, ref freq, new Size(width, height), minFrequency, maxFrequency, foundMaxDecibel, foundMaxFreq);
			return spectrum;
		}

		/// <summary>
		/// Prepare the spectrum graph by extracting the amplitude as decibel and the frequencies as herz
		/// </summary>
		/// <param name="spectrumData">spectrum data</param>
		/// <param name="sampleRate">sample rate</param>
		/// <param name="fftWindowsSize">fft windows size</param>
		/// <param name="m_mag">output the magnitude array as decibel</param>
		/// <param name="m_freq">output the frequency array as herz</param>
		/// <param name="foundMaxFrequency">output the max frequency found</param>
		/// <param name="foundMaxDecibel">output the max frequency decibel found</param>
		public static void PrepareSpectrumAnalysis(float[] spectrumData, double sampleRate, int fftWindowsSize,
		                                           out float[] m_mag, out float[] m_freq,
		                                           out float foundMaxFrequency, out float foundMaxDecibel) {

			int spectrumDataLength = spectrumData.Length; // 1024 - half the fftWindowsSize (2048)

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
				m_freq[i] = (float) MathUtils.IndexToFreq(i, sampleRate, fftWindowsSize);
			}
			
			// store the max findings
			foundMaxDecibel = MathUtils.AmplitudeToDecibel(spectrumData[maxIndex]);
			foundMaxFrequency = (float) MathUtils.IndexToFreq(maxIndex, sampleRate, fftWindowsSize);
		}
		
		/// <summary>
		/// Get a spectrum of the signal specified at the input
		/// </summary>
		/// <param name="mag">array of magnitude values as decibel</param>
		/// <param name="freq">array of frequency values as herz</param>
		/// <param name="imageSize">Size of image</param>
		/// <param name="minFrequency">minimum frequency to show</param>
		/// <param name="maxFrequency">maximum frequency to show</param>
		/// <param name="foundMaxDecibel">if specified output max decibel text</param>
		/// <param name="foundMaxFrequency">if specified output max frequency text</param>
		/// <returns>Spectral image of the signal</returns>
		/// <remarks>This code is based on the code by Gerald T. Beauregard
		/// which was released under the MIT License. (Copyright (c) 2010 Gerald T. Beauregard)
		/// The code were ported to C# and heavily modifified by Per Ivar Nerseth, 2012
		/// </remarks>
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

			int TOP_MARGIN = 5;                     	// Top of graph
			int LEFT_MARGIN = 5;                    	// Left edge of graph
			int HEIGHT = imageSize.Height-2*TOP_MARGIN;	// Height of graph
			int WIDTH = imageSize.Width-2*LEFT_MARGIN;     // Width of graph
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
			int BOTTOM = TOTAL_HEIGHT-TOP_MARGIN;                   		// Bottom of graph
			float DBTOPIXEL = (float) HEIGHT/(MAX_DB-MIN_DB);    	// Pixels/tick
			float FREQTOPIXEL = (float) WIDTH/(MAX_FREQ-MIN_FREQ);	// Pixels/Hz
			
			try {
				var png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				var eg = new ExtendedGraphics(g);
				
				int numPoints = mag.Length;
				if ( mag.Length != freq.Length )
					System.Diagnostics.Debug.WriteLine( "mag.length != freq.length" );
				
				var linePen = new Pen(lineColor, 0.5f);
				var middleLinePen = new Pen(middleLineColor, 0.5f);
				var textPen = new Pen(textColor, 1);
				var samplePen = new Pen(sampleColor, 1);

				// Draw a rectangular box marking the boundaries of the graph
				// Create outer rectangle.
				var rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
				Brush fillBrushOuter = new SolidBrush(fillOuterColor);
				g.FillRectangle(fillBrushOuter, rectOuter);
				
				// Create rectangle.
				var rect = new Rectangle(LEFT_MARGIN, TOP_MARGIN, WIDTH, HEIGHT);
				Brush fillBrush = new SolidBrush(fillColor);
				if (drawRoundedRectangles) {
					eg.FillRoundRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height, 10);
					eg.DrawRoundRectangle(linePen, rect.X, rect.Y, rect.Width, rect.Height, 10);
				} else {
					g.FillRectangle(fillBrush, rect);
					g.DrawRectangle(linePen, rect);
				}
				
				// Label for horizontal axis
				var drawLabelFont = new Font("Arial", 8);
				var drawLabelBrush = new SolidBrush(textPen.Color);
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
					if (y < BOTTOM && y > TOP_MARGIN+1) {
						g.DrawLine(linePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
					}

					// draw horozontal middle line (between the main lines)
					yMiddle = y-(DBTOPIXEL*DB_STEP)/2;
					if (yMiddle > 0) {
						g.DrawLine(middleLinePen, LEFT_MARGIN, yMiddle, LEFT_MARGIN+WIDTH, yMiddle);
					}

					if ( dBTick != MAX_DB )
					{
						// Numbers on the tick marks
						var drawFont = new Font("Arial", 8);
						var drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + dBTick + " dB", drawFont, drawBrush, LEFT_MARGIN+5, y - drawFont.GetHeight(g) -2);
					}
				}
				
				if (drawLabels) {
					// Label for vertical axis
					g.DrawString(LABEL_Y, drawLabelFont, drawLabelBrush, 1, TOP_MARGIN + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
				}
				
				// Tick marks on the horizontal axis
				for ( float f = MIN_FREQ; f <= MAX_FREQ; f += FREQ_STEP )
				{
					// draw vertical main line
					x = LEFT_MARGIN + FREQTOPIXEL*(f-MIN_FREQ);
					if (x > LEFT_MARGIN  && x < WIDTH) {
						g.DrawLine(linePen, x, BOTTOM, x, TOP_MARGIN);
					}

					// draw vertical middle line (between the main lines)
					xMiddle = x + FREQTOPIXEL*FREQ_STEP/2;
					if (xMiddle < TOTAL_WIDTH) {
						g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP_MARGIN);
					}

					if ( f != MIN_FREQ && f != MAX_FREQ )
					{
						// Numbers on the tick marks
						var drawFont = new Font("Arial", 8);
						var drawBrush = new SolidBrush(textPen.Color);
						g.DrawString("" + f + " Hz", drawFont, drawBrush, x, TOP_MARGIN +2);
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
				float oldY = TOP_MARGIN;
				bool firstPoint = true;
				for ( ; i < numPoints && freq[i] <= MAX_FREQ; i++ )
				{
					// Compute horizontal position
					x = LEFT_MARGIN + FREQTOPIXEL*(freq[i]-MIN_FREQ);
					
					// Compute vertical position of point
					// and clip at top/bottom.
					y = BOTTOM - DBTOPIXEL*(mag[i]-MIN_DB);
					
					if ( y < TOP_MARGIN )
						y = TOP_MARGIN;
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
		/// Utility method to return spectrogram image using audio data
		/// </summary>
		/// <param name="audioData">The audio data (mono)</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="sampleRate">sample rate</param>
		/// <param name="fftWindowsSize">fft windows size</param>
		/// <param name="fftOverlap">fft overlap</param>
		/// <param name="colorPalette">color palette to use, typ ColorUtils.ColorPaletteType</param>
		/// <param name="doLogScale">whether to use log scale on the freq axis</param>
		/// <returns>Spectrogram image</returns>
		public static Bitmap GetSpectrogramImage(float[] audioData, int width, int height, double sampleRate, int fftWindowsSize, int fftOverlap, ColorUtils.ColorPaletteType colorPalette, bool doLogScale)
		{
			float[][] spectrogram;
			double minFrequency = 27.5;
			double maxFrequency = sampleRate / 2;
			int logBins = height - 2*40; // the margins used
			var logFrequenciesIndex = new int[1];
			var logFrequencies = new float[1];

			// find the time
			int numberOfSamples = audioData.Length;
			double seconds = numberOfSamples / sampleRate;

			if (!doLogScale) {
				spectrogram = CreateSpectrogramLomont(audioData, fftWindowsSize, fftOverlap);
			} else {
				// calculate the log frequency index table
				GetLogFrequenciesIndex(sampleRate, minFrequency, maxFrequency, logBins, fftWindowsSize, LogBase, out logFrequenciesIndex, out logFrequencies);
				spectrogram = CreateLogSpectrogramLomont(audioData, fftWindowsSize, fftOverlap, logBins, logFrequenciesIndex, logFrequencies);
			}
			
			return GetSpectrogramImage(spectrogram, width, height, seconds*1000, sampleRate, colorPalette, doLogScale, logFrequenciesIndex, logFrequencies);
		}

		/// <summary>
		///   Get a spectrogram of the signal specified at the input
		/// </summary>
		/// <param name="spectrogram">Signal</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <returns>Spectral image of the signal</returns>
		/// <remarks>
		///   X axis - time
		///   Y axis - frequency
		///   Color - magnitude level of corresponding band value of the signal
		/// </remarks>
		/// <remarks>This is a copy of the method with the same name from
		/// Soundfingerprinting.SoundTools.Imaging.cs in
		/// https://code.google.com/p/soundfingerprinting/
		/// </remarks>
		public static Bitmap GetSpectrogramImage(float[][] spectrogram, int width, int height)
		{
			#if SAFE
			if (width < 0)
				throw new ArgumentException("width should be bigger than 0");
			if (height < 0)
				throw new ArgumentException("height should be bigger than 0");
			#endif
			var image = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(image);
			
			// Fill Back color
			using (Brush brush = new SolidBrush(Color.Black))
			{
				graphics.FillRectangle(brush, new Rectangle(0, 0, width, height));
			}
			
			int bands = spectrogram[0].Length;
			double max = spectrogram.Max((b) => b.Max((v) => Math.Abs(v)));
			double deltaX = (double) (width - 1)/spectrogram.Length; /*By how much the image will move to the left*/
			double deltaY = (double) (height - 1)/(bands + 1); /*By how much the image will move upward*/
			int prevX = 0;
			for (int i = 0, n = spectrogram.Length; i < n; i++)
			{
				double x = i*deltaX;
				if ((int) x == prevX) continue;
				for (int j = 0, m = spectrogram[0].Length; j < m; j++)
				{
					Color color = ColorUtils.ValueToBlackWhiteColor(spectrogram[i][j], max/10);
					image.SetPixel((int) x, height - (int) (deltaY*j) - 1, color);
				}
				prevX = (int) x;
			}
			return image;
		}
		
		/// <summary>
		/// Get a spectrogram of the signal specified at the input
		/// </summary>
		/// <param name="spectrogram">Signal</param>
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
		public static Bitmap GetSpectrogramImage(float[][] spectrogram, int width, int height, double milliseconds, double sampleRate, ColorUtils.ColorPaletteType colorPalette, bool doLogScale, int[] logFrequenciesIndex, float[] logFrequencies)
		{
			#if SAFE
			if (width < 0)
				throw new ArgumentException("width should be bigger than 0");
			if (height < 0)
				throw new ArgumentException("height should be bigger than 0");
			#endif

			bool drawLabels = true;
			float minDb = -90.0f; // -80.0f also works good
			float maxDb = 10.0f; // with the current color palettes 10.0f works well
			
			// Basic constants
			int TOTAL_HEIGHT = height;    // Height of graph
			int TOTAL_WIDTH = width;      // Width of graph

			int TOP_MARGIN = 40;                    // Top of graph
			int LEFT_MARGIN = 60;                   // Left edge of graph
			int HEIGHT = height-2*TOP_MARGIN;		// Height of graph
			int WIDTH = width-2*LEFT_MARGIN;     	// Width of graph
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
			int BOTTOM = TOTAL_HEIGHT-TOP_MARGIN;                   			// Bottom of graph
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
			
			var fullImage = new Bitmap(TOTAL_WIDTH, TOTAL_HEIGHT);
			Graphics g = Graphics.FromImage(fullImage);
			
			var linePen = new Pen(lineColor, 0.5f);
			var middleLinePen = new Pen(middleLineColor, 0.5f);
			var labelPen = new Pen(labelColor, 1);
			var tickPen = new Pen(tickColor, 1);

			// Draw a rectangular box marking the boundaries of the graph
			var rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
			Brush fillBrushOuter = new SolidBrush(fillOuterColor);
			g.FillRectangle(fillBrushOuter, rectOuter);
			
			// Create rectangle.
			var rect = new Rectangle(LEFT_MARGIN, TOP_MARGIN, WIDTH, HEIGHT);
			Brush fillBrush = new SolidBrush(fillColor);
			g.FillRectangle(fillBrush, rect);
			g.DrawRectangle(linePen, rect);
			
			// Label for horizontal axis
			var drawLabelFont = new Font("Arial", 8);
			var drawLabelBrush = new SolidBrush(labelPen.Color);
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
					if (y < BOTTOM && y > TOP_MARGIN+1) {
						g.DrawLine(linePen, LEFT_MARGIN-2, y, LEFT_MARGIN+WIDTH+2, y);
					}

					// draw horozontal middle line (between the main lines)
					yMiddle = y-(FREQTOPIXEL*FREQ_STEP)/2;
					if (yMiddle > TOP_MARGIN && yMiddle < HEIGHT+TOP_MARGIN) {
						g.DrawLine(middleLinePen, LEFT_MARGIN, yMiddle, LEFT_MARGIN+WIDTH, yMiddle);
					}

					if ( freqTick != MAX_FREQ )
					{
						// Numbers on the tick marks
						var drawFont = new Font("Arial", 8);
						var drawBrush = new SolidBrush(tickPen.Color);
						
						// left
						g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, LEFT_MARGIN - 33, y - drawFont.GetHeight(g)/2);

						// right
						g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, WIDTH + LEFT_MARGIN + 4, y - drawFont.GetHeight(g)/2);
					}
				}
			} else {
				// LOG SCALE
				for (int i = 0; i < logFrequencies.Length; i+=20)
				{
					float freqTick = logFrequencies[i];
					y = BOTTOM - i;

					// draw horozontal main line
					if (y < BOTTOM && y > TOP_MARGIN+1) {
						g.DrawLine(linePen, LEFT_MARGIN-2, y, LEFT_MARGIN+WIDTH+2, y);
					}
					
					// Numbers on the tick marks
					var drawFont = new Font("Arial", 8);
					var drawBrush = new SolidBrush(tickPen.Color);
					
					// left
					g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, LEFT_MARGIN - 33, y - drawFont.GetHeight(g)/2);

					// right
					g.DrawString(MathUtils.FormatNumber((int) freqTick), drawFont, drawBrush, WIDTH + LEFT_MARGIN + 4, y - drawFont.GetHeight(g)/2);
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
				x = LEFT_MARGIN + TIMETOPIXEL*(timeTick-MIN_TIME);
				if (x > LEFT_MARGIN  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM+2, x, TOP_MARGIN-2);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < WIDTH+LEFT_MARGIN) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP_MARGIN);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// Numbers on the tick marks
					var drawFont = new Font("Arial", 8);
					var drawBrush = new SolidBrush(tickPen.Color);
					SizeF drawTimeTickTextSize = g.MeasureString("" + timeTick, drawFont);

					// top
					g.DrawString("" + timeTick, drawFont, drawBrush, x-(drawTimeTickTextSize.Width/2), TOP_MARGIN - 15);

					// bottom
					g.DrawString("" + timeTick, drawFont, drawBrush, x-(drawTimeTickTextSize.Width/2), BOTTOM + 2);
				}
			}
			
			// draw spectrogram
			var spectrogramImage = new Bitmap(WIDTH, HEIGHT);
			
			// calculate min and max
			double max = spectrogram.Max((b) => b.Max((v) => Math.Abs(v)));
			double min = spectrogram.Min((b) => b.Min((v) => Math.Abs(v)));

			int numberOfSamplesX = spectrogram.Length; 	// time
			int numberOfSamplesY = spectrogram[0].Length; 	// hz
			
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
					float amplitude = spectrogram[i][j];
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
					spectrogramImage.SetPixel((int) xCoord + 1, HEIGHT - (int) (deltaY*j) - 1, colorbw);
				}
				prevX = (int) xCoord;
			}
			
			if (colorPalette != ColorUtils.ColorPaletteType.BLACK_AND_WHITE) {
				spectrogramImage = ColorUtils.Colorize(spectrogramImage, 255, colorPalette);
			}
			
			// add the spectrogram to the full image
			g.DrawImage(spectrogramImage, LEFT_MARGIN, TOP_MARGIN);
			
			return fullImage;
		}

		/// <summary>
		/// Draw a waveform
		/// </summary>
		/// <param name="audioData">The audio data (mono)</param>
		/// <param name="imageSize">Size of the image</param>
		/// <param name="amplitude">Amplitude (1 is default)</param>
		/// <param name="sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name="drawRaw">Whether to draw only the raw image (no margins)</param>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, double sampleRate, bool drawRaw=false) {
			return DrawWaveform(audioData, imageSize, amplitude, 0, 0, sampleRate, drawRaw);
		}

		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name="audioData">The audio data (mono)</param>
		/// <param name="imageSize">Size of the image</param>
		/// <param name="amplitude">Amplitude (1 is default)</param>
		/// <param name="startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name="endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name="sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name="drawRaw">Whether to draw only the raw image (no margins)</param>
		/// <returns>A bitmap of the waveform</returns>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, double sampleRate, bool drawRaw=false) {
			DrawingProperties prop = DrawingProperties.Blue;
			prop.DrawRaw = true;
			
			return DrawWaveform(audioData, imageSize, amplitude, startZoomSamplePosition, endZoomSamplePosition, sampleRate, prop);
		}

		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name="audioData">The audio data (mono)</param>
		/// <param name="imageSize">Size of the image</param>
		/// <param name="amplitude">Amplitude (1 is default)</param>
		/// <param name="startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name="endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name="sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name="properties">DrawingProperties properties, like colors and margins</param>
		/// <seealso cref="https://github.com/aalin/canvas_waveform"></seealso>
		/// <seealso cref="http://www.hisschemoller.com/2010/mp3-wave-display/"></seealso>
		/// <seealso cref="http://www.marinbezhanov.com/web-development/14/actionscript-3-sound-extract-demystified-or-how-to-draw-a-waveform-in-flash/"></seealso>
		/// <seealso cref="http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form"></seealso>
		/// <returns>A bitmap of the waveform</returns>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, double sampleRate, DrawingProperties properties) {

			#region Define Basic Variables and Properties
			int TOTAL_HEIGHT = imageSize.Height;    // Height of graph
			int TOTAL_WIDTH = imageSize.Width;      // Width of graph

			int TOP_MARGIN = properties.Margin;               	// Top of graph
			int LEFT_MARGIN = properties.Margin;                 	// Left edge of graph
			if (properties.DrawRaw) {
				TOP_MARGIN = 0;                     		// Top of graph
				LEFT_MARGIN = 0;                    		// Left edge of graph
			}
			int HEIGHT = imageSize.Height-2*TOP_MARGIN;		// Height of graph
			int WIDTH = imageSize.Width-2*LEFT_MARGIN;     	// Width of graph
			
			// make sure amplitude doesn't exceed a sensible treshold
			if (amplitude > 5000) {
				amplitude = 5000;
			}
			float MIN_AMPLITUDE = -1.0f / amplitude;
			float MAX_AMPLITUDE = 1.0f / amplitude;
			float AMPLITUDE_STEP = MAX_AMPLITUDE / 2;
			
			// Derived constants
			int CENTER = TOTAL_HEIGHT / 2;
			int RIGHT = WIDTH;
			int BOTTOM = TOTAL_HEIGHT-TOP_MARGIN; // Bottom of graph
			#endregion
			
			int totalNumberOfSamples = 0;
			float[] data = null;
			float samplesPerPixel = 0;
			
			#region Setup data array taking zoom into account
			if (audioData != null && audioData.Length > 0) {

				totalNumberOfSamples = audioData.Length;
				
				// make sure the zoom start and zoom end is correct
				if (startZoomSamplePosition < 0)
					startZoomSamplePosition = 0;
				if (endZoomSamplePosition > audioData.Length || endZoomSamplePosition < 0)
					endZoomSamplePosition = audioData.Length;
				
				if (endZoomSamplePosition != 0) {
					data = new float[endZoomSamplePosition-startZoomSamplePosition];
					Array.Copy(audioData, startZoomSamplePosition, data, 0, endZoomSamplePosition-startZoomSamplePosition);
					samplesPerPixel = (float) (endZoomSamplePosition - startZoomSamplePosition) / (float) WIDTH;
				} else {
					data = audioData;
					samplesPerPixel = (float) totalNumberOfSamples / (float) WIDTH;
				}
			}
			#endregion
			
			#region Calculate time variables
			double totalDurationMs = totalNumberOfSamples / sampleRate * 1000;
			
			float MAX_TIME = (float) (endZoomSamplePosition / sampleRate * 1000);
			float MIN_TIME = 0.0f;
			if (startZoomSamplePosition > 0) {
				MIN_TIME = (float) (startZoomSamplePosition / sampleRate * 1000);
			}
			
			float TIME_STEP = (float) MathUtils.GetNicerNumber((MAX_TIME-MIN_TIME) / 5);
			float AMPLITUDETOPIXEL = (float) HEIGHT/(MAX_AMPLITUDE-MIN_AMPLITUDE); 	// Pixels/tick
			float TIMETOPIXEL = (float) WIDTH/(MAX_TIME-MIN_TIME); 					// Pixels/second
			#endregion
			
			// Set up for drawing
			var png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality; // Set pixeloffsetmode to high quality to make sure we can draw small filled circles
			var eg = new ExtendedGraphics(g);
			
			#region Define Pens and Brushes
			var centreLinePen = new Pen(properties.CenterLineColor, 1.0f);
			var linePen = new Pen(properties.LineColor, 0.5f);
			var middleLinePen = new Pen(properties.MiddleLineColor, 0.5f);
			var textPen = new Pen(properties.TextColor, 1.0f);
			var samplePen = new Pen(properties.SampleColor, 1.0f);
			var infoBoxPen = new Pen(properties.DebugBoxTextColor, 1.0f);

			Brush sampleDotBrush = new SolidBrush(properties.SampleColor);
			Brush fillBrushOuter = new SolidBrush(properties.FillOuterColor);
			Brush fillBrush = new SolidBrush(properties.FillColor);
			Brush drawLabelBrush = new SolidBrush(textPen.Color);
			Brush drawBrush = new SolidBrush(textPen.Color);
			#endregion

			#region Draw a Rectangular Box marking the boundaries of the graph
			
			// Create outer rectangle.
			var rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
			g.FillRectangle(fillBrushOuter, rectOuter);
			
			// Create rectangle.
			var rect = new Rectangle(LEFT_MARGIN, TOP_MARGIN, WIDTH, HEIGHT);
			if (properties.DrawRoundedRectangles) {
				eg.FillRoundRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height, 10);
				eg.DrawRoundRectangle(linePen, rect.X, rect.Y, rect.Width, rect.Height, 10);
			} else {
				g.FillRectangle(fillBrush, rect);
				g.DrawRectangle(linePen, rect);
			}
			#endregion
			
			#region Draw Grid with Labels and Ticks
			
			// Label for horizontal axis
			var drawLabelFont = new Font("Arial", 8);
			if (properties.DrawLabels) {
				SizeF drawLabelTextSize = g.MeasureString(properties.LabelXaxis, drawLabelFont);
				g.DrawString(properties.LabelXaxis, drawLabelFont, drawLabelBrush, (TOTAL_WIDTH/2) - (drawLabelTextSize.Width/2), TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -3);
			}

			float y = 0;
			float yMiddle = 0;
			float x = 0;
			float xMiddle = 0;
			for ( float amplitudeTick = MIN_AMPLITUDE; amplitudeTick <= MAX_AMPLITUDE; amplitudeTick += AMPLITUDE_STEP )
			{
				// draw horizontal main line
				y = BOTTOM - AMPLITUDETOPIXEL*(amplitudeTick-MIN_AMPLITUDE);
				if (y < BOTTOM && y > TOP_MARGIN+1) {
					g.DrawLine(linePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
				}

				// draw horizontal middle line (between the main lines)
				yMiddle = y-(AMPLITUDETOPIXEL*AMPLITUDE_STEP)/2;
				if (yMiddle > 0) {
					g.DrawLine(middleLinePen, LEFT_MARGIN, yMiddle, LEFT_MARGIN+WIDTH, yMiddle);
				}

				if ( amplitudeTick != MAX_AMPLITUDE ) {
					// Numbers on the tick marks
					var drawFont = new Font("Arial", 7);
					g.DrawString(amplitudeTick.ToString("0.000000"), drawFont, drawBrush, LEFT_MARGIN+5, y - drawFont.GetHeight(g) -2);
				}
			}
			
			if (properties.DrawLabels) {
				// Label for vertical axis
				g.DrawString(properties.LabelYaxis, drawLabelFont, drawLabelBrush, 1, TOP_MARGIN + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
			}
			
			// Tick marks on the horizontal axis
			for ( double timeTick = MIN_TIME; timeTick <= MAX_TIME; timeTick += TIME_STEP )
			{
				// draw vertical main line
				x = (float) (LEFT_MARGIN + TIMETOPIXEL*(timeTick-MIN_TIME));
				if (x > LEFT_MARGIN  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM, x, TOP_MARGIN);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < TOTAL_WIDTH) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP_MARGIN);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// Numbers on the tick marks
					var drawFont = new Font("Arial", 7);
					TimeSpan time = TimeSpan.FromMilliseconds(timeTick);
					g.DrawString(time.ToString(@"hh\:mm\:ss\.FFFFFFF"), drawFont, drawBrush, x, TOP_MARGIN +2);
				}
			}

			if (properties.DisplayTime) {
				string displayTimeString = String.Format("Duration: {0} samples @ {1:0.0000} ms", totalNumberOfSamples, totalDurationMs);
				SizeF displayTimeStringTextSize = g.MeasureString(displayTimeString, drawLabelFont);
				g.DrawString(displayTimeString, drawLabelFont, drawLabelBrush, TOTAL_WIDTH - displayTimeStringTextSize.Width - 10, TOTAL_HEIGHT - drawLabelFont.GetHeight(g) - 10);
			}
			
			// draw centre line
			g.DrawLine(centreLinePen, LEFT_MARGIN, CENTER, WIDTH, CENTER);
			#endregion
			
			#region Draw waveform
			if (data != null && data.Length > 0) {
				if (samplesPerPixel >= 1) {
					// the number of samples are greater than the available drawing space
					// (i.e. greater than the number of pixles in the X-Axis)

					#region Draw When More Samples than Width
					int yMax = 0;
					int yMin = 0;
					for (int xAxis = 0; xAxis < WIDTH; xAxis++)
					{
						// determine start and end points within WAV (for this single pixel on the X axis)
						int start 	= (int)((float)(xAxis) 		* samplesPerPixel);
						int end 	= (int)((float)(xAxis + 1) 	* samplesPerPixel);
						
						// reset the min and max values
						yMax = 0;
						yMin = 0;
						
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
						
						yMax = TOP_MARGIN + HEIGHT - (int)((max * amplitude + 1) * 0.5 * HEIGHT);
						yMin = TOP_MARGIN + HEIGHT - (int)((min * amplitude + 1) * 0.5 * HEIGHT);

						// limit within the drawing space
						if (yMax < 0) yMax = 0;
						if (yMin > HEIGHT) yMin = HEIGHT;
						
						// make sure that we always draw something
						if (yMin == yMax) {
							yMin += 1;
						}

						// draw waveform
						// use yMax and yMin
						// original from example: http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
						// basically don't care about previous x or y, but draw vertical lines
						// from y min to y max value
						g.DrawLine(samplePen, xAxis + LEFT_MARGIN, yMin, xAxis + LEFT_MARGIN, yMax);
					}
					#endregion
					
				} else {
					// the number of samples are less than the available drawing space
					// (i.e. less than the number of pixles in the X-Axis)
					
					#region Draw When Less Samples than Width
					int samples = data.Length;
					if (samples > 1) {
						
						// at least two samples
						float mult_x = (float) WIDTH / (endZoomSamplePosition-startZoomSamplePosition - 1);

						var ps = new List<Point>();
						for (int i = 0; i < data.Length; i++) {
							x = (i * mult_x) + LEFT_MARGIN;
							y = TOP_MARGIN + HEIGHT - (int)((data[i] * amplitude + 1) * 0.5 * HEIGHT);
							var p = new Point((int)x, (int)y);
							ps.Add(p);
						}

						if (ps.Count > 0) {
							g.DrawLines(samplePen, ps.ToArray());

							// draw small dots for each sample
							// make sure we have at least space for a dot with 5 pixels width
							if ( ps.Count < (float) (WIDTH / 5)) {
								foreach(Point p in ps) {
									g.FillEllipse(sampleDotBrush, p.X-2, p.Y-2, 4, 4);
								}
							}
						}
						
					} else {
						// we have only one sample, draw a flat line
						g.DrawLine(linePen, 0, 0.5f * HEIGHT, WIDTH, 0.5f * HEIGHT);
					}
					#endregion
				}
			}
			#endregion
			
			#region Draw right upper debug box
			if (properties.DisplayDebugBox) {
				var drawInfoBoxFont = new Font("Arial", 8);
				var drawInfoBoxBrush = new SolidBrush(infoBoxPen.Color);
				
				string infoBoxLine1Text = String.Format("SamplesPerPixel Orig: {0:0.000} => New: {1:0.000}", (float) totalNumberOfSamples / WIDTH, samplesPerPixel);
				string infoBoxLine2Text = String.Format("Time (Min->Max): {0} -> {1}", MIN_TIME, MAX_TIME);
				string infoBoxLine3Text = String.Format("Timestep: {0}, TimeToPixel: {1}", TIME_STEP, TIMETOPIXEL);

				// get box width
				const int infoBoxMargin = 5;
				var textLineSizes = new List<float>();
				textLineSizes.Add(g.MeasureString(infoBoxLine1Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine2Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine3Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(150.0f); // info box minimum width
				
				float infoBoxLineTextWidth = 0.0f;
				float minWidth = 0.0f;
				MathUtils.ComputeMinAndMax(textLineSizes.ToArray(), out minWidth, out infoBoxLineTextWidth);

				int infoBoxWidth = (int) infoBoxLineTextWidth;
				
				float infoBoxLineTextHeight = drawInfoBoxFont.GetHeight(g);
				int infoBoxHeight = (int) (infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*4);
				
				var rectInfoBox = new Rectangle(WIDTH - infoBoxWidth - 20, 30, infoBoxWidth, infoBoxHeight);
				Brush fillBrushInfoBox = new SolidBrush(properties.DebugBoxBgColor);
				g.FillRectangle(fillBrushInfoBox, rectInfoBox);
				g.DrawRectangle(linePen, rectInfoBox);
				
				g.DrawString(infoBoxLine1Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin);
				g.DrawString(infoBoxLine2Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin));
				g.DrawString(infoBoxLine3Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*2);
			}
			#endregion
			
			#region Clean Up variables
			centreLinePen.Dispose();
			linePen.Dispose();
			middleLinePen.Dispose();
			textPen.Dispose();
			samplePen.Dispose();
			infoBoxPen.Dispose();

			sampleDotBrush.Dispose();
			fillBrushOuter.Dispose();
			fillBrush.Dispose();
			drawLabelBrush.Dispose();
			drawBrush.Dispose();
			#endregion
			
			return png;
		}
		
		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name = "audioData">The audio data (interleaved 32-bit floating-point sample data)</param>
		/// <param name = "imageSize">Size of the image</param>
		/// <param name = "amplitude">Amplitude (1 is default)</param>
		/// <param name = "startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name = "endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name = "sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name = "channels">Number of channels</param>
		/// <returns>A bitmap of the waveform</returns>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, double sampleRate, int channels) {
			DrawingProperties prop = DrawingProperties.Blue;
			prop.DrawRaw = true;
			prop.DisplayTime = true;
			prop.TimeLineUnit = TimelineUnit.Time;
			
			return DrawWaveform(audioData, imageSize, amplitude, startZoomSamplePosition, endZoomSamplePosition, sampleRate, channels, prop);
		}
		
		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name = "audioData">The audio data (interleaved 32-bit floating-point sample data)</param>
		/// <param name = "imageSize">Size of the image</param>
		/// <param name = "amplitude">Amplitude (1 is default)</param>
		/// <param name = "startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name = "endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name = "sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name = "channels">Number of channels</param>
		/// <param name = "properties">DrawingProperties properties, like colors and margins</param>
		/// <returns>A bitmap of the waveform</returns>
		/// <remarks>
		/// Audio data should be structured in an array where each sucessive index
		/// alternates between left or right channel data, starting with left. Index 0
		/// should be the first left level, index 1 should be the first right level, index
		/// 2 should be the second left level, etc.
		/// </remarks>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, double sampleRate, int channels, DrawingProperties properties) {
			
			#region Define Basic Variables and Properties
			int TOTAL_HEIGHT = imageSize.Height;    		// Height of graph
			int TOTAL_WIDTH = imageSize.Width;      		// Width of graph

			int TOP_MARGIN = properties.Margin;				// Top of graph
			int LEFT_MARGIN = properties.Margin;			// Left edge of graph
			if (properties.DrawRaw) {
				TOP_MARGIN = 15;                     		// Top of graph
				LEFT_MARGIN = 30;                    		// Left edge of graph
			}
			int HEIGHT = imageSize.Height -2*TOP_MARGIN;	// Height of graph
			int WIDTH = imageSize.Width -2*LEFT_MARGIN;     // Width of graph
			
			// make sure amplitude doesn't exceed a sensible treshold
			if (amplitude > 5000) {
				amplitude = 5000;
			}
			float MIN_AMPLITUDE = -1.0f / amplitude;
			float MAX_AMPLITUDE = 1.0f / amplitude;
			float AMPLITUDE_STEP = MAX_AMPLITUDE / 4;
			
			// Derived constants
			//int CENTER = TOTAL_HEIGHT / 2;
			int RIGHT = WIDTH;
			int BOTTOM = TOTAL_HEIGHT-TOP_MARGIN; // Bottom of graph
			#endregion
			
			int totalNumberOfSamples = 0;
			int maxChannelNumberOfSamples = 0;
			float[] data = null;
			float samplesPerPixel = 0;
			
			#region Setup data array taking zoom into account
			if (audioData != null && audioData.Length > 0) {

				maxChannelNumberOfSamples = (int) ((float) audioData.Length / (float) channels);
				totalNumberOfSamples = audioData.Length;
				
				// make sure the zoom start and zoom end is correct
				if (startZoomSamplePosition < 0) {
					startZoomSamplePosition = 0;
				}
				if (endZoomSamplePosition > maxChannelNumberOfSamples || endZoomSamplePosition < 0) {
					endZoomSamplePosition = maxChannelNumberOfSamples;
				}
				if (endZoomSamplePosition != 0) {
					data = new float[(endZoomSamplePosition-startZoomSamplePosition)*channels];
					Array.Copy(audioData, startZoomSamplePosition, data, 0, (endZoomSamplePosition-startZoomSamplePosition)*channels);
					samplesPerPixel = (float) (endZoomSamplePosition - startZoomSamplePosition) / (float) WIDTH;
				} else {
					data = audioData;
					samplesPerPixel = (float) maxChannelNumberOfSamples / (float) WIDTH;
				}
			}
			#endregion
			
			#region Calculate time variables
			double totalDurationMs = maxChannelNumberOfSamples / sampleRate * 1000;
			
			float MAX_TIME = (float) (endZoomSamplePosition / sampleRate * 1000);
			float MIN_TIME = 0.0f;
			if (startZoomSamplePosition > 0) {
				MIN_TIME = (float) (startZoomSamplePosition / sampleRate * 1000);
			}
			
			float TIME_STEP = (float) MathUtils.GetNicerNumber((MAX_TIME-MIN_TIME) / 10);
			float AMPLITUDETOPIXEL = (float) (HEIGHT/(channels))/(MAX_AMPLITUDE-MIN_AMPLITUDE);	// Pixels/tick
			float TIMETOPIXEL = (float) WIDTH/(MAX_TIME-MIN_TIME); 								// Pixels/second
			#endregion
			
			// Set up for drawing
			var png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality; // Set pixeloffsetmode to high quality to make sure we can draw small filled circles
			var eg = new ExtendedGraphics(g);
			
			#region Define Pens and Brushes
			var separationLinePen = new Pen(properties.SeparationLineColor, 1.0f);
			var centreLinePen = new Pen(properties.CenterLineColor, 1.0f);
			var linePen = new Pen(properties.LineColor, 0.5f);
			var middleLinePen = new Pen(properties.MiddleLineColor, 0.5f);
			var textPen = new Pen(properties.TextColor, 1.0f);
			var samplePen = new Pen(properties.SampleColor, 1.0f);
			var infoBoxPen = new Pen(properties.DebugBoxTextColor, 1.0f);

			Brush sampleDotBrush = new SolidBrush(properties.SampleColor);
			Brush fillBrushOuter = new SolidBrush(properties.FillOuterColor);
			Brush fillBrush = new SolidBrush(properties.FillColor);
			Brush drawLabelBrush = new SolidBrush(textPen.Color);
			Brush drawBrush = new SolidBrush(textPen.Color);
			#endregion

			#region Draw a Rectangular Box marking the boundaries of the graph
			
			// Create outer rectangle.
			var rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
			g.FillRectangle(fillBrushOuter, rectOuter);
			
			// Create rectangle.
			var rect = new Rectangle(LEFT_MARGIN, TOP_MARGIN, WIDTH, HEIGHT);
			if (properties.DrawRoundedRectangles) {
				eg.FillRoundRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height, 10);
				eg.DrawRoundRectangle(linePen, rect.X, rect.Y, rect.Width, rect.Height, 10);
			} else {
				g.FillRectangle(fillBrush, rect);
				g.DrawRectangle(linePen, rect);
			}
			
			#endregion
			
			#region Draw Grid with Labels and Ticks
			
			var drawLabelFont = new Font("Arial", 8);
			if (properties.DrawLabels) {
				// Label for horizontal axis
				SizeF drawLabelTextSize = g.MeasureString(properties.LabelXaxis, drawLabelFont);
				g.DrawString(properties.LabelXaxis, drawLabelFont, drawLabelBrush, (TOTAL_WIDTH/2) - (drawLabelTextSize.Width/2), TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -3);
				
				// Label for vertical axis
				g.DrawString(properties.LabelYaxis, drawLabelFont, drawLabelBrush, 1, TOP_MARGIN + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
			}
			
			// Tick marks on the horizontal axis
			float x = 0;
			float y = 0;
			float yMiddle = 0;
			float xMiddle = 0;
			for ( double timeTick = MIN_TIME; timeTick <= MAX_TIME; timeTick += TIME_STEP )
			{
				// draw vertical main line
				x = (float) (LEFT_MARGIN + TIMETOPIXEL*(timeTick-MIN_TIME));
				if (x > LEFT_MARGIN  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM, x, TOP_MARGIN);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < TOTAL_WIDTH) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP_MARGIN);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// draw numbers on the tick marks
					var drawFont = new Font("Arial", 7);
					string displayTimeTickString = "";
					TimeSpan time;

					switch (properties.TimeLineUnit) {
						case TimelineUnit.Time:
							time = TimeSpan.FromMilliseconds(timeTick);
							displayTimeTickString = time.ToString(@"hh\:mm\:ss\.fff");
							break;
						case TimelineUnit.Samples:
							int samples = (int)(timeTick * sampleRate / 1000);
							displayTimeTickString = samples.ToString("0");
							break;
						case TimelineUnit.Seconds:
							time = TimeSpan.FromMilliseconds(timeTick);
							displayTimeTickString = time.ToString(@"s\.fff");
							break;
					}
					SizeF displayTimeTickStringSize = g.MeasureString(displayTimeTickString, drawFont);
					g.DrawString(displayTimeTickString, drawFont, drawBrush, x - displayTimeTickStringSize.Width/2, 2);
				}
			}
			#endregion

			if (properties.DisplayTime) {
				string displayTimeString = String.Format("Duration: {0} samples @ {1:0.0000} ms", maxChannelNumberOfSamples, totalDurationMs);
				SizeF displayTimeStringTextSize = g.MeasureString(displayTimeString, drawLabelFont);
				g.DrawString(displayTimeString, drawLabelFont, drawLabelBrush, TOTAL_WIDTH - displayTimeStringTextSize.Width - LEFT_MARGIN, TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -2);
			}
			
			#region Draw Waveform
			for (int c = 0; c < channels; c++) {
				
				x = 0;
				y = 0;
				yMiddle = 0;
				xMiddle = 0;
				var drawFont = new Font("Arial", 7);
				for ( float amplitudeTick = MIN_AMPLITUDE; amplitudeTick <= MAX_AMPLITUDE; amplitudeTick += AMPLITUDE_STEP )
				{
					// calcuate y steps from the top and downwards
					y = TOP_MARGIN + AMPLITUDETOPIXEL*(amplitudeTick-MIN_AMPLITUDE) + (c*HEIGHT/channels);
					
					// draw numbers on the tick marks
					if (amplitudeTick != MAX_AMPLITUDE && amplitudeTick != MIN_AMPLITUDE && amplitudeTick != 0) {
						// draw horizontal main line
						g.DrawLine(linePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
						g.DrawString(MathUtils.AmplitudeToDecibel(Math.Abs(amplitudeTick)).ToString("0.0"), drawFont, drawBrush, 4, y - drawFont.GetHeight(g)/2);
					} else if (amplitudeTick == 0) {
						// draw horizontal main line (the center of the waveform)
						g.DrawLine(centreLinePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
						g.DrawString("-dB", drawFont, drawBrush, 4, y - drawFont.GetHeight(g)/2);
					} else if (amplitudeTick == MAX_AMPLITUDE) {
						// draw separation line
						if (properties.DrawSeparationLine) {
							if (channels > 1 && c < channels-1) {
								g.DrawLine(separationLinePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
							}
						}
					}
				}
				
				if (data != null && data.Length > 0) {
					if (samplesPerPixel >= 1) {
						// the number of samples are greater than the available drawing space
						// (i.e. greater than the number of pixles in the X-Axis)

						#region Draw When More Samples than Width
						int yMax = 0;
						int yMin = 0;
						for (int xAxis = 0; xAxis < WIDTH; xAxis++)
						{
							// determine start and end points within WAV (for this single pixel on the X axis)
							int start 	= (int)((float)(xAxis) 		* samplesPerPixel);
							int end 	= (int)((float)(xAxis + 1) 	* samplesPerPixel);
							
							// reset the min and max values
							yMax = 0;
							yMin = 0;
							
							// determine the min and max values within this specific range
							float min = float.MaxValue;
							float max = float.MinValue;
							for (int i = start; i < end; i++) // removed the i <= end
							{
								if (i < data.Length) {
									float val = data[channels*i+c];
									min = val < min ? val : min;
									max = val > max ? val : max;
								}
							}
							
							// start drawing the topmost wave
							yMax = Transform(max, HEIGHT/channels, c, amplitude) + TOP_MARGIN;
							yMin = Transform(min, HEIGHT/channels, c, amplitude) + TOP_MARGIN;
							
							// make sure that we always draw something
							if (yMin == yMax) {
								yMin += 1;
							}

							// draw waveform
							// use yMax and yMin
							// original from example: http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
							// basically don't care about previous x or y, but draw vertical lines
							// from y min to y max value
							g.DrawLine(samplePen, xAxis + LEFT_MARGIN, yMin, xAxis + LEFT_MARGIN, yMax);
						}
						#endregion
						
					} else {
						// the number of samples are less than the available drawing space
						// (i.e. less than the number of pixles in the X-Axis)
						
						#region Draw When Less Samples than Width
						int samples = (int) ((float) data.Length / (float) channels);
						if (samples > 1) {
							
							// at least two samples
							float mult_x = (float) WIDTH / (endZoomSamplePosition-startZoomSamplePosition - 1);

							var ps = new List<Point>();
							for (int i = 0; i < data.Length; i++) {
								x = (i * mult_x) + LEFT_MARGIN;
								//y = TOP_MARGIN + HEIGHT - (int)((data[channels*i+c] * amplitude + 1) * 0.5 * HEIGHT);
								y = Transform(data[channels*i+c], HEIGHT/channels, c, amplitude) + TOP_MARGIN;
								var p = new Point((int)x, (int)y);
								ps.Add(p);
							}

							if (ps.Count > 0) {
								g.DrawLines(samplePen, ps.ToArray());

								// draw small dots for each sample
								// make sure we have at least space for a dot with 5 pixels width
								if ( ps.Count < (float) (WIDTH / 5)) {
									foreach(Point p in ps) {
										g.FillEllipse(sampleDotBrush, p.X-2, p.Y-2, 4, 4);
									}
								}
							}
							
						} else {
							// we have only one sample, draw a flat line
							g.DrawLine(linePen, 0, 0.5f * HEIGHT, WIDTH, 0.5f * HEIGHT);
						}
						#endregion
					}
				}
			}
			#endregion
			
			#region Draw right upper debug box
			if (properties.DisplayDebugBox) {
				var drawInfoBoxFont = new Font("Arial", 8);
				var drawInfoBoxBrush = new SolidBrush(infoBoxPen.Color);
				
				string infoBoxLine1Text = String.Format("SamplesPerPixel Orig: {0:0.000} => New: {1:0.000}", (float) totalNumberOfSamples / WIDTH, samplesPerPixel);
				string infoBoxLine2Text = String.Format("Time (Min->Max): {0} -> {1}", MIN_TIME, MAX_TIME);
				string infoBoxLine3Text = String.Format("Timestep: {0}, TimeToPixel: {1}", TIME_STEP, TIMETOPIXEL);

				// get box width
				const int infoBoxMargin = 5;
				var textLineSizes = new List<float>();
				textLineSizes.Add(g.MeasureString(infoBoxLine1Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine2Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine3Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(150.0f); // info box minimum width
				
				float infoBoxLineTextWidth = 0.0f;
				float minWidth = 0.0f;
				MathUtils.ComputeMinAndMax(textLineSizes.ToArray(), out minWidth, out infoBoxLineTextWidth);

				int infoBoxWidth = (int) infoBoxLineTextWidth;
				
				float infoBoxLineTextHeight = drawInfoBoxFont.GetHeight(g);
				int infoBoxHeight = (int) (infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*4);
				
				var rectInfoBox = new Rectangle(WIDTH - infoBoxWidth - 20, 30, infoBoxWidth, infoBoxHeight);
				Brush fillBrushInfoBox = new SolidBrush(properties.DebugBoxBgColor);
				g.FillRectangle(fillBrushInfoBox, rectInfoBox);
				g.DrawRectangle(linePen, rectInfoBox);
				
				g.DrawString(infoBoxLine1Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin);
				g.DrawString(infoBoxLine2Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin));
				g.DrawString(infoBoxLine3Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*2);
			}
			#endregion
			
			#region Clean Up variables
			separationLinePen.Dispose();
			centreLinePen.Dispose();
			linePen.Dispose();
			middleLinePen.Dispose();
			textPen.Dispose();
			samplePen.Dispose();
			infoBoxPen.Dispose();

			sampleDotBrush.Dispose();
			fillBrushOuter.Dispose();
			fillBrush.Dispose();
			drawLabelBrush.Dispose();
			drawBrush.Dispose();
			#endregion
			
			return png;
		}
		
		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name = "audioData">The audio data (interleaved 32-bit floating-point sample data)</param>
		/// <param name = "imageSize">Size of the image</param>
		/// <param name = "amplitude">Amplitude (1 is default)</param>
		/// <param name = "startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name = "endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name = "startSelectSamplePosition">What sample to start selection at</param>
		/// <param name = "endSelectSamplePosition">What sample to end selection at</param>
		/// <param name = "samplePosition">What sample position the cursor/playhead is at</param>
		/// <param name = "sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name = "channels">Number of channels</param>
		/// <returns>A bitmap of the waveform</returns>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, int startSelectSamplePosition, int endSelectSamplePosition, int samplePosition, double sampleRate, int channels) {
			DrawingProperties prop = DrawingProperties.Blue;
			prop.DrawRaw = true;
			prop.DisplayTime = true;
			prop.TimeLineUnit = TimelineUnit.Time;
			
			return DrawWaveform(audioData, imageSize, amplitude, startZoomSamplePosition, endZoomSamplePosition, startSelectSamplePosition, endSelectSamplePosition, samplePosition, sampleRate, channels, prop);
		}
		
		/// <summary>
		/// Draw a waveform using start and end zoom sample position
		/// </summary>
		/// <param name = "audioData">The audio data (interleaved 32-bit floating-point sample data)</param>
		/// <param name = "imageSize">Size of the image</param>
		/// <param name = "amplitude">Amplitude (1 is default)</param>
		/// <param name = "startZoomSamplePosition">First Sample to Zoom in on</param>
		/// <param name = "endZoomSamplePosition">Last Sample to Zoom in on</param>
		/// <param name = "startSelectSamplePosition">What sample to start selection at</param>
		/// <param name = "endSelectSamplePosition">What sample to end selection at</param>
		/// <param name = "samplePosition">What sample position the cursor/playhead is at</param>
		/// <param name = "sampleRate">Samplerate of the audio data (to calculate time)</param>
		/// <param name = "channels">Number of channels</param>
		/// <param name = "properties">DrawingProperties properties, like colors and margins</param>
		/// <returns>A bitmap of the waveform</returns>
		/// <remarks>
		/// Audio data should be structured in an array where each sucessive index
		/// alternates between left or right channel data, starting with left. Index 0
		/// should be the first left level, index 1 should be the first right level, index
		/// 2 should be the second left level, etc.
		/// </remarks>
		public static Bitmap DrawWaveform(float[] audioData, Size imageSize, int amplitude, int startZoomSamplePosition, int endZoomSamplePosition, int startSelectSamplePosition, int endSelectSamplePosition, int samplePosition, double sampleRate, int channels, DrawingProperties properties) {
			
			#region Define Basic Variables and Properties
			int TOTAL_HEIGHT = imageSize.Height;    		// Height of graph
			int TOTAL_WIDTH = imageSize.Width;      		// Width of graph

			int TOP_MARGIN = properties.Margin;				// Top of graph
			int LEFT_MARGIN = properties.Margin;			// Left edge of graph
			if (properties.DrawRaw) {
				TOP_MARGIN = 15;                     		// Top of graph
				LEFT_MARGIN = 30;                    		// Left edge of graph
			}
			int HEIGHT = imageSize.Height -2*TOP_MARGIN;	// Height of graph
			int WIDTH = imageSize.Width -2*LEFT_MARGIN;     // Width of graph
			
			// make sure amplitude doesn't exceed a sensible treshold
			if (amplitude > 5000) {
				amplitude = 5000;
			}
			float MIN_AMPLITUDE = -1.0f / amplitude;
			float MAX_AMPLITUDE = 1.0f / amplitude;
			float AMPLITUDE_STEP = MAX_AMPLITUDE / 4;
			
			// Derived constants
			//int CENTER = TOTAL_HEIGHT / 2;
			int RIGHT = WIDTH;
			int BOTTOM = TOTAL_HEIGHT-TOP_MARGIN; // Bottom of graph
			#endregion
			
			int totalNumberOfSamples = 0;
			int maxChannelNumberOfSamples = 0;
			float[] data = null;
			float samplesPerPixel = 0;
			
			#region Setup data array taking zoom into account
			if (audioData != null && audioData.Length > 0) {

				maxChannelNumberOfSamples = (int) ((float) audioData.Length / (float) channels);
				totalNumberOfSamples = audioData.Length;
				
				// make sure the zoom start and zoom end is correct
				if (startZoomSamplePosition < 0) {
					startZoomSamplePosition = 0;
				}
				if (endZoomSamplePosition > maxChannelNumberOfSamples || endZoomSamplePosition <= 0) {
					endZoomSamplePosition = maxChannelNumberOfSamples;
				}
				if (endZoomSamplePosition != 0) {
					data = new float[(endZoomSamplePosition-startZoomSamplePosition)*channels];
					Array.Copy(audioData, startZoomSamplePosition, data, 0, (endZoomSamplePosition-startZoomSamplePosition)*channels);
					samplesPerPixel = (float) (endZoomSamplePosition - startZoomSamplePosition) / (float) WIDTH;
				} else {
					data = audioData;
					samplesPerPixel = (float) maxChannelNumberOfSamples / (float) WIDTH;
				}
			}
			#endregion
			
			#region Calculate time variables
			double totalDurationMs = maxChannelNumberOfSamples / sampleRate * 1000;
			
			float MAX_TIME = (float) (endZoomSamplePosition / sampleRate * 1000);
			float MIN_TIME = 0.0f;
			if (startZoomSamplePosition > 0) {
				MIN_TIME = (float) (startZoomSamplePosition / sampleRate * 1000);
			}
			
			float TIME_STEP = (float) MathUtils.GetNicerNumber((MAX_TIME-MIN_TIME) / 10);
			float AMPLITUDETOPIXEL = (float) (HEIGHT/(channels))/(MAX_AMPLITUDE-MIN_AMPLITUDE);	// Pixels/tick
			float TIMETOPIXEL = (float) WIDTH/(MAX_TIME-MIN_TIME); 								// Pixels/second
			#endregion
			
			// Set up for drawing
			var png = new Bitmap( TOTAL_WIDTH, TOTAL_HEIGHT, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality; // Set pixeloffsetmode to high quality to make sure we can draw small filled circles
			var eg = new ExtendedGraphics(g);
			
			#region Define Pens and Brushes
			var separationLinePen = new Pen(properties.SeparationLineColor, 1.0f);
			var centreLinePen = new Pen(properties.CenterLineColor, 1.0f);
			var linePen = new Pen(properties.LineColor, 0.5f);
			var middleLinePen = new Pen(properties.MiddleLineColor, 0.5f);
			var textPen = new Pen(properties.TextColor, 1.0f);
			var samplePen = new Pen(properties.SampleColor, 1.0f);
			var infoBoxPen = new Pen(properties.DebugBoxTextColor, 1.0f);

			Brush sampleDotBrush = new SolidBrush(properties.SampleColor);
			Brush fillBrushOuter = new SolidBrush(properties.FillOuterColor);
			Brush fillBrush = new SolidBrush(properties.FillColor);
			Brush drawLabelBrush = new SolidBrush(textPen.Color);
			Brush drawBrush = new SolidBrush(textPen.Color);
			#endregion

			#region Draw a Rectangular Box marking the boundaries of the graph
			
			// Create outer rectangle.
			var rectOuter = new Rectangle(0, 0, TOTAL_WIDTH, TOTAL_HEIGHT);
			g.FillRectangle(fillBrushOuter, rectOuter);
			
			// Create rectangle.
			var rect = new Rectangle(LEFT_MARGIN, TOP_MARGIN, WIDTH, HEIGHT);
			if (properties.DrawRoundedRectangles) {
				eg.FillRoundRectangle(fillBrush, rect.X, rect.Y, rect.Width, rect.Height, 10);
				eg.DrawRoundRectangle(linePen, rect.X, rect.Y, rect.Width, rect.Height, 10);
			} else {
				g.FillRectangle(fillBrush, rect);
				g.DrawRectangle(linePen, rect);
			}
			
			#endregion
			
			#region Draw Grid with Labels and Ticks
			
			var drawLabelFont = new Font("Arial", 8);
			if (properties.DrawLabels) {
				// Label for horizontal axis
				SizeF drawLabelTextSize = g.MeasureString(properties.LabelXaxis, drawLabelFont);
				g.DrawString(properties.LabelXaxis, drawLabelFont, drawLabelBrush, (TOTAL_WIDTH/2) - (drawLabelTextSize.Width/2), TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -3);
				
				// Label for vertical axis
				g.DrawString(properties.LabelYaxis, drawLabelFont, drawLabelBrush, 1, TOP_MARGIN + HEIGHT/2 - drawLabelFont.GetHeight(g)/2);
			}
			
			// Tick marks on the horizontal axis
			float x = 0;
			float y = 0;
			float yMiddle = 0;
			float xMiddle = 0;
			for ( double timeTick = MIN_TIME; timeTick <= MAX_TIME; timeTick += TIME_STEP )
			{
				// draw vertical main line
				x = (float) (LEFT_MARGIN + TIMETOPIXEL*(timeTick-MIN_TIME));
				if (x > LEFT_MARGIN  && x < WIDTH) {
					g.DrawLine(linePen, x, BOTTOM, x, TOP_MARGIN);
				}

				// draw vertical middle line (between the main lines)
				xMiddle = x + TIMETOPIXEL*TIME_STEP/2;
				if (xMiddle < TOTAL_WIDTH) {
					g.DrawLine(middleLinePen, xMiddle, BOTTOM, xMiddle, TOP_MARGIN);
				}

				if ( timeTick != MIN_TIME && timeTick != MAX_TIME )
				{
					// draw numbers on the tick marks
					var drawFont = new Font("Arial", 7);
					string displayTimeTickString = "";
					TimeSpan time;

					switch (properties.TimeLineUnit) {
						case TimelineUnit.Time:
							time = TimeSpan.FromMilliseconds(timeTick);
							displayTimeTickString = time.ToString(@"hh\:mm\:ss\.fff");
							break;
						case TimelineUnit.Samples:
							int samples = (int)(timeTick * sampleRate / 1000);
							displayTimeTickString = samples.ToString("0");
							break;
						case TimelineUnit.Seconds:
							time = TimeSpan.FromMilliseconds(timeTick);
							displayTimeTickString = time.ToString(@"s\.fff");
							break;
					}
					SizeF displayTimeTickStringSize = g.MeasureString(displayTimeTickString, drawFont);
					g.DrawString(displayTimeTickString, drawFont, drawBrush, x - displayTimeTickStringSize.Width/2, 2);
				}
			}
			#endregion

			if (properties.DisplayTime) {
				string displayTimeString = String.Format("Duration: {0} samples @ {1:0.0000} ms", maxChannelNumberOfSamples, totalDurationMs);
				SizeF displayTimeStringTextSize = g.MeasureString(displayTimeString, drawLabelFont);
				g.DrawString(displayTimeString, drawLabelFont, drawLabelBrush, TOTAL_WIDTH - displayTimeStringTextSize.Width - LEFT_MARGIN, TOTAL_HEIGHT - drawLabelFont.GetHeight(g) -2);
			}
			
			#region Draw Waveform
			for (int c = 0; c < channels; c++) {
				x = 0;
				y = 0;
				yMiddle = 0;
				xMiddle = 0;
				var drawFont = new Font("Arial", 7);
				for ( float amplitudeTick = MIN_AMPLITUDE; amplitudeTick <= MAX_AMPLITUDE; amplitudeTick += AMPLITUDE_STEP )
				{
					// calcuate y steps from the top and downwards
					y = TOP_MARGIN + AMPLITUDETOPIXEL*(amplitudeTick-MIN_AMPLITUDE) + (c*HEIGHT/channels);
					
					// draw numbers on the tick marks
					if (amplitudeTick != MAX_AMPLITUDE && amplitudeTick != MIN_AMPLITUDE && amplitudeTick != 0) {
						// draw horizontal main line
						g.DrawLine(linePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
						g.DrawString(MathUtils.AmplitudeToDecibel(Math.Abs(amplitudeTick)).ToString("0.0"), drawFont, drawBrush, 4, y - drawFont.GetHeight(g)/2);
					} else if (amplitudeTick == 0) {
						// draw horizontal main line (the center of the waveform)
						g.DrawLine(centreLinePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
						g.DrawString("-dB", drawFont, drawBrush, 4, y - drawFont.GetHeight(g)/2);
					} else if (amplitudeTick == MAX_AMPLITUDE) {
						// draw separation line
						if (properties.DrawSeparationLine) {
							if (channels > 1 && c < channels-1) {
								g.DrawLine(separationLinePen, LEFT_MARGIN, y, LEFT_MARGIN+WIDTH, y);
							}
						}
					}
				}
				
				if (data != null && data.Length > 0) {
					if (samplesPerPixel >= 1) {
						// the number of samples are greater than the available drawing space
						// (i.e. greater than the number of pixles in the X-Axis)

						#region Draw When More Samples than Width
						int yMax = 0;
						int yMin = 0;
						for (int xAxis = 0; xAxis < WIDTH; xAxis++)
						{
							// determine start and end points within WAV (for this single pixel on the X axis)
							int start 	= (int)((float)(xAxis) 		* samplesPerPixel);
							int end 	= (int)((float)(xAxis + 1) 	* samplesPerPixel);
							
							// reset the min and max values
							yMax = 0;
							yMin = 0;
							
							// determine the min and max values within this specific range (sample bucket)
							float min = float.MaxValue;
							float max = float.MinValue;
							for (int i = start; i < end; i++)
							{
								if (i < data.Length) {
									float val = data[channels*i+c];
									min = val < min ? val : min;
									max = val > max ? val : max;
								}
							}
							
							// start drawing the topmost wave
							yMax = Transform(max, HEIGHT/channels, c, amplitude) + TOP_MARGIN;
							yMin = Transform(min, HEIGHT/channels, c, amplitude) + TOP_MARGIN;
							
							// make sure that we always draw something
							if (yMin == yMax) {
								yMin += 1;
							}

							// draw waveform
							// use yMax and yMin
							// original from example: http://stackoverflow.com/questions/1215326/open-source-c-sharp-code-to-present-wave-form
							// basically don't care about previous x or y, but draw vertical lines
							// from y min to y max value
							
							// if selection or position change colors
							// check also public virtual void OnDraw(CDC pDC) in WaveShopView.cs
							if (samplePosition >= start && samplePosition < end) {
								// we are drawing the "right now" playhead position/cursor
								
								// draw vertical line full height
								g.DrawLine(Pens.Blue, xAxis + LEFT_MARGIN, TOP_MARGIN + HEIGHT/channels + (c*HEIGHT/channels), xAxis + LEFT_MARGIN, TOP_MARGIN + (c*HEIGHT/channels));
							} else if (start >= startSelectSamplePosition && end <= endSelectSamplePosition) {
								
								// draw vertical line full height with background selection color
								g.DrawLine(Pens.Black, xAxis + LEFT_MARGIN, TOP_MARGIN + HEIGHT/channels + (c*HEIGHT/channels), xAxis + LEFT_MARGIN, TOP_MARGIN + (c*HEIGHT/channels));
								
								// and normal wave in selected color
								g.DrawLine(Pens.Blue, xAxis + LEFT_MARGIN, yMin, xAxis + LEFT_MARGIN, yMax);
							} else {
								// draw normal wave in normal color
								g.DrawLine(samplePen, xAxis + LEFT_MARGIN, yMin, xAxis + LEFT_MARGIN, yMax);
							}
						}
						#endregion
						
					} else {
						// the number of samples are less than the available drawing space
						// (i.e. less than the number of pixles in the X-Axis)
						
						#region Draw When Less Samples than Width
						int samples = (int) ((float) data.Length / (float) channels);
						if (samples > 1) {
							
							// at least two samples
							float mult_x = (float) WIDTH / (endZoomSamplePosition-startZoomSamplePosition - 1);

							var ps = new List<Point>();
							for (int i = 0; i < data.Length; i++) {
								x = (i * mult_x) + LEFT_MARGIN;
								//y = TOP_MARGIN + HEIGHT - (int)((data[channels*i+c] * amplitude + 1) * 0.5 * HEIGHT);
								y = Transform(data[channels*i+c], HEIGHT/channels, c, amplitude) + TOP_MARGIN;
								var p = new Point((int)x, (int)y);
								ps.Add(p);
							}

							if (ps.Count > 0) {
								g.DrawLines(samplePen, ps.ToArray());

								// draw small dots for each sample
								// make sure we have at least space for a dot with 5 pixels width
								if ( ps.Count < (float) (WIDTH / 5)) {
									foreach(Point p in ps) {
										g.FillEllipse(sampleDotBrush, p.X-2, p.Y-2, 4, 4);
									}
								}
							}
							
						} else {
							// we have only one sample, draw a flat line
							g.DrawLine(linePen, 0, 0.5f * HEIGHT, WIDTH, 0.5f * HEIGHT);
						}
						#endregion
					}
				}
			}
			#endregion
			
			// invert selection range
			
			
			#region Draw right upper debug box
			if (properties.DisplayDebugBox) {
				var drawInfoBoxFont = new Font("Arial", 8);
				var drawInfoBoxBrush = new SolidBrush(infoBoxPen.Color);
				
				string infoBoxLine1Text = String.Format("SamplesPerPixel Orig: {0:0.000} => New: {1:0.000}", (float) totalNumberOfSamples / WIDTH, samplesPerPixel);
				string infoBoxLine2Text = String.Format("Time (Min->Max): {0} -> {1}", MIN_TIME, MAX_TIME);
				string infoBoxLine3Text = String.Format("Timestep: {0}, TimeToPixel: {1}", TIME_STEP, TIMETOPIXEL);

				// get box width
				const int infoBoxMargin = 5;
				var textLineSizes = new List<float>();
				textLineSizes.Add(g.MeasureString(infoBoxLine1Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine2Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(g.MeasureString(infoBoxLine3Text, drawInfoBoxFont).Width + infoBoxMargin*2);
				textLineSizes.Add(150.0f); // info box minimum width
				
				float infoBoxLineTextWidth = 0.0f;
				float minWidth = 0.0f;
				MathUtils.ComputeMinAndMax(textLineSizes.ToArray(), out minWidth, out infoBoxLineTextWidth);

				int infoBoxWidth = (int) infoBoxLineTextWidth;
				
				float infoBoxLineTextHeight = drawInfoBoxFont.GetHeight(g);
				int infoBoxHeight = (int) (infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*4);
				
				var rectInfoBox = new Rectangle(WIDTH - infoBoxWidth - 20, 30, infoBoxWidth, infoBoxHeight);
				Brush fillBrushInfoBox = new SolidBrush(properties.DebugBoxBgColor);
				g.FillRectangle(fillBrushInfoBox, rectInfoBox);
				g.DrawRectangle(linePen, rectInfoBox);
				
				g.DrawString(infoBoxLine1Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin);
				g.DrawString(infoBoxLine2Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin));
				g.DrawString(infoBoxLine3Text, drawInfoBoxFont, drawInfoBoxBrush, WIDTH - infoBoxWidth - 20 + infoBoxMargin, 30 + infoBoxMargin + (infoBoxLineTextHeight + infoBoxMargin)*2);
			}
			#endregion
			
			#region Clean Up variables
			separationLinePen.Dispose();
			centreLinePen.Dispose();
			linePen.Dispose();
			middleLinePen.Dispose();
			textPen.Dispose();
			samplePen.Dispose();
			infoBoxPen.Dispose();

			sampleDotBrush.Dispose();
			fillBrushOuter.Dispose();
			fillBrush.Dispose();
			drawLabelBrush.Dispose();
			drawBrush.Dispose();
			#endregion
			
			return png;
		}
		
		/// <summary>
		/// Transforms a value in the range of -1.0 to +1.0
		/// </summary>
		/// <param name="peak"></param>
		/// <param name="channelHeight"></param>
		/// <param name="channelIndex"></param>
		/// <param name="zoom"></param>
		/// <returns></returns>
		private static int Transform(float peak, int channelHeight, int channelIndex, double zoom)
		{
			int top = channelIndex * channelHeight;
			int bottom = top + channelHeight;
			var i = (int)((0.5d + 0.5d * -peak * zoom) * channelHeight + channelHeight * channelIndex);
			return i < top ? top : i > bottom ? bottom : i;
		}

		/// <summary>
		/// Draw a waveform of the signal
		/// </summary>
		/// <param name = "data">Data to be drawn</param>
		/// <param name = "width">Width of the image</param>
		/// <param name = "height">Height of the image</param>
		/// <returns>Bitmap</returns>
		/// <remarks>This is a copy of the method GetSignalImage from
		/// Soundfingerprinting.SoundTools.Imaging.cs in
		/// https://code.google.com/p/soundfingerprinting/
		/// </remarks>
		public static Bitmap DrawWaveform(float[] data, int width, int height)
		{
			#if SAFE
			// create new image
			if (data == null)
				throw new ArgumentException("Bitmap data was not supplied");
			if (width < 0)
				throw new ArgumentException("width should be bigger than 0");
			if (height < 0)
				throw new ArgumentException("height should be bigger than 0");
			#endif
			
			var image = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(image);
			/*Fill Back color*/
			using (Brush brush = new SolidBrush(Color.Black))
			{
				graphics.FillRectangle(brush, new Rectangle(0, 0, width, height));
			}
			const int gridline = 50; /*Every 50 pixels draw gridline*/
			/*Draw gridlines*/
			using (var pen = new Pen(Color.Red, 1))
			{
				/*Draw horizontal gridlines*/
				for (int i = 1; i < height/gridline; i++)
				{
					graphics.DrawLine(pen, 0, i*gridline, width, i*gridline);
				}

				/*Draw vertical gridlines*/
				for (int i = 1; i < width/gridline; i++)
				{
					graphics.DrawLine(pen, i*gridline, 0, i*gridline, height);
				}
			}
			int center = height/2;
			/*Draw lines*/
			using (var pen = new Pen(Color.MediumSpringGreen, 1))
			{
				/*Find delta X, by which the lines will be drawn*/
				double deltaX = (double) width/data.Length;
				double normalizeFactor = data.Max((a) => Math.Abs(a))/((double) height/2);
				for (int i = 0, n = data.Length; i < n; i++)
				{
					graphics.DrawLine(pen, (float) (i*deltaX), center, (float) (i*deltaX), (float) (center - (data[i]/normalizeFactor)));
				}
			}
			using (var pen = new Pen(Color.DarkGreen, 1))
			{
				/*Draw center line*/
				graphics.DrawLine(pen, 0, center, width, center);
			}
			return image;
		}
		#endregion
	}
	
	/// <summary>
	/// Defines in what unit a timeline is represented.
	/// </summary>
	public enum TimelineUnit
	{
		Samples,
		Time,
		Seconds
	}
	
	public class DrawingProperties {
		
		/// <summary>
		/// For a waveform view this is the centre middle line (0 dB)
		/// </summary>
		public Color CenterLineColor  { get; set; }
		
		/// <summary>
		/// This is the line between the waveforms when showing more than one (e.g. when showing stereo)
		/// </summary>
		public Color SeparationLineColor { get; set; }
		
		public Color LineColor  { get; set; }
		public Color MiddleLineColor  { get; set; }
		public Color TextColor  { get; set; }
		public Color SampleColor { get; set; }
		public Color FillOuterColor { get; set; }
		public Color FillColor { get; set; }
		public Color DebugBoxTextColor { get; set; }
		public Color DebugBoxBgColor { get; set; }
		
		public string LabelXaxis { get; set; }
		public string LabelYaxis { get; set; }
		
		/// <summary>
		/// Define the margin around the waveform image
		/// </summary>
		public int Margin { get; set; }
		
		/// <summary>
		/// Defines in what unit a timeline is represented.
		/// </summary>
		public TimelineUnit TimeLineUnit { get; set; }
		
		bool drawRaw;
		public bool DrawRaw {
			get {
				return drawRaw;
			}
			set {
				drawRaw = value;
				if (drawRaw) {
					DrawLabels = false;
					DrawRoundedRectangles = false;
					DisplayDebugBox = false;
					DisplayTime = false;
				} else {
					DrawLabels = false;
					DrawRoundedRectangles = true;
					DisplayDebugBox = false;
					DisplayTime = true;
				}
			}
		}
		
		/// <summary>
		/// Determine whether to draw labels on the axis
		/// </summary>
		public bool DrawLabels { get; set; }
		
		/// <summary>
		/// Determine whether to draw rounded rectangles
		/// </summary>
		public bool DrawRoundedRectangles { get; set; }
		
		/// <summary>
		/// Determine whether to draw a line between the waveforms if more than one
		/// </summary>
		public bool DrawSeparationLine { get; set; }
		
		/// <summary>
		/// Display Debug Box?
		/// </summary>
		public bool DisplayDebugBox { get; set; }
		
		/// <summary>
		/// Determine whether to display a separate time info line
		/// </summary>
		public bool DisplayTime { get; set; }
		
		public DrawingProperties() {
			
			// Set some default values, setting DrawRaw also sets the default values for the other bool parameters
			DrawRaw = false;
			
			// always separate the waveforms
			DrawSeparationLine = true;
			
			Margin = 5; // Use 5 pixels margins when not drawing raw
			
			TimeLineUnit = TimelineUnit.Time;
			
			LabelXaxis = "Time"; 					// Label for X axis
			LabelYaxis = "Amplitude";             	// Label for Y axis
		}
		
		public static DrawingProperties Orange {
			get {
				var prop = new DrawingProperties();

				prop.CenterLineColor = ColorTranslator.FromHtml("#C7834C");
				prop.SeparationLineColor = ColorTranslator.FromHtml("#000000");
				prop.LineColor = ColorTranslator.FromHtml("#C7834C");
				prop.MiddleLineColor = ColorTranslator.FromHtml("#EFAB74");
				prop.TextColor = ColorTranslator.FromHtml("#A9652E");
				prop.SampleColor = ColorTranslator.FromHtml("#4C2F1A");
				prop.FillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
				prop.FillColor = ColorTranslator.FromHtml("#F9C998");
				prop.DebugBoxTextColor = ColorTranslator.FromHtml("#4C2F1A");
				prop.DebugBoxBgColor = ColorTranslator.FromHtml("#F7DECA");
				
				return prop;
			}
		}

		public static DrawingProperties Blue {
			get {
				var prop = new DrawingProperties();

				prop.CenterLineColor = ColorTranslator.FromHtml("#000032");
				prop.SeparationLineColor = ColorTranslator.FromHtml("#000000");
				prop.LineColor = ColorTranslator.FromHtml("#C0C0C0");
				prop.MiddleLineColor = ColorTranslator.FromHtml("#E2E2E2");
				prop.TextColor = ColorTranslator.FromHtml("#000000");
				prop.SampleColor = ColorTranslator.FromHtml("#000064");
				prop.FillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
				prop.FillColor = ColorTranslator.FromHtml("#FFFFFF");
				prop.DebugBoxTextColor = ColorTranslator.FromHtml("#4C2F1A");
				prop.DebugBoxBgColor = ColorTranslator.FromHtml("#F7DECA");
				
				return prop;
			}
		}

	}
}
