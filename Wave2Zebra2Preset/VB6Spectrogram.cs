//*****************************************************************************
//*
//* Copyright (c) 2002, Wilhelm Kurz.  All Rights Reserved.
//* wkurz@foni.net
//*
//* This file is provided for demonstration and educational uses only.
//* Permission to use, copy, modify and distribute this file for
//* any purpose and without fee is hereby granted.
//*
//* Sample application for DynaPlot3
//*****************************************************************************
using System;
using System.Drawing;
using System.Drawing.Imaging;

using System.Collections.Generic;

using CommonUtils;

namespace Wave2Zebra2Preset
{
	/// <summary>
	/// Description of VB6Spectrogram.
	/// </summary>
	public class VB6Spectrogram
	{
		private const int RangedB = 100; // used for color calculations, maps from -RangedB to 0 dB
		private const int RangePaletteIndex = 256;
		private static double Log10 = Math.Log(10); //2.30258509299405;

		//public long[] LevelPalette = new long[RangePaletteIndex];
		public Dictionary<long, Color> LevelPaletteDictionary = new Dictionary<long, Color>();
		
		// computes the color palette for the pseudocolor display
		// creates a bitmap with the palette and loads it into chart #3
		public void ComputeColorPalette()
		{
			long col = 0;
			int imageHeight = 200;
			byte[,] Legendpixelmatrix = new byte[imageHeight, RangePaletteIndex];
			
			for (col = 0; col < RangePaletteIndex; col++)
			{
				//LevelPalette[col] = PaletteValue(col, RangePaletteIndex);
				LevelPaletteDictionary.Add(col, PaletteValueColor(col, RangePaletteIndex));
				for (int i = 0; i < imageHeight; i++) {
					Legendpixelmatrix[i, col] = (byte) col;
				}
			}
			
			SaveBitmap("VB6", "Colorpalette", Legendpixelmatrix, 8, LevelPaletteDictionary);
		}

		// loads a wav-file and displays the recorded signal in chart #1
		// computes the spectrogram of the signal
		// converts the pixelarray to a bmp-file
		// saves this file to disk and loads it into chart #2
		//
		// fftOverlapPercentage is a number between 0 and 100
		// fftWindowsSize aka NFFT (width of FFT window)
		public float[][] Compute(float[] data, double sampleRate, int fftWindowsSize, float fftOverlapPercentage)
		{
			long col = 0;
			int c = 0;
			
			long NumSamples = data.Length;

			fftOverlapPercentage = fftOverlapPercentage / 100;
			
			// calculate the number of samples one column will make up using the actual overlap
			long ColSampleWidth = (long)(fftWindowsSize * (1 - fftOverlapPercentage));
			double fftOverlapSamples = fftWindowsSize * fftOverlapPercentage;
			// calculate the number of columns when each column which has the specified sample width
			long NumCols = NumSamples / ColSampleWidth;

			System.Console.Out.WriteLine(String.Format("Samples: {0}, Sample Rate {1}, Seconds: {2}.", NumSamples, sampleRate, NumSamples/sampleRate));
			System.Console.Out.WriteLine(String.Format("NFFT (fftWindowsSize): {0}, Overlap percentage: {1}%, Overlap samples (NOverlap): {2:n2}.", fftWindowsSize, fftOverlapPercentage*100, fftOverlapSamples ));
			System.Console.Out.WriteLine(String.Format("Dividing the samples into {0} columns with width {1}.", NumCols, ColSampleWidth));

			System.Console.Out.WriteLine(String.Format("Width: {0}.", NumCols));
			System.Console.Out.WriteLine(String.Format("Height: {0}.", fftWindowsSize/2));
			
			double[] real = new double[fftWindowsSize];
			double[] imag = new double[fftWindowsSize];
			float[] magnitude = new float[fftWindowsSize / 2];
			byte[,] Pixelmatrix = new byte[fftWindowsSize / 2, NumCols];
			float[][] frames = new float[NumCols][];

			long sampleIndex = 0;
			for (col = 0; col < NumCols; col++)
			{
				// read a segment of the audio file
				for (c = 0; c < fftWindowsSize; c++)
				{
					sampleIndex = col * ColSampleWidth + c;
					// make sure we don't step beyond the end of the recording
					if (sampleIndex < NumSamples) {
						real[c] = data[sampleIndex] * VB6Fourier.Hanning(fftWindowsSize, c);
						imag[c] = 0; // clear the phase
					} else {
						//System.Console.Out.WriteLine(String.Format("Outside boundries: col: {0} c: {1}", col, c));
					}
				}

				// transform to the frequency domain
				VB6Fourier.FourierTransform(real, imag, fftWindowsSize, true);

				// and compute the magnitude spectrum
				VB6Fourier.MagnitudeSpectrum(real, imag, fftWindowsSize, VB6Fourier.W0Hanning, out magnitude);

				// set up one column of the spectrogram
				for (c = 0; c < fftWindowsSize / 2; c++)
				{
					Pixelmatrix[c, col] = (byte) MapToPixelIndex(magnitude[c], RangedB, RangePaletteIndex);
				}

				frames[col] = magnitude;
			}

			// the sampleRate / 2 (nyquistFreq) has a total of fftWindowsSize / 2 unique frequencies
			double nyquistFreq = sampleRate / 2;
			float[] F = new float[fftWindowsSize/2];
			for (int i = 1; i < fftWindowsSize/2 + 1; i++) {
				F[i-1] = (float) ((double)i / fftWindowsSize * sampleRate); // in hz
			}
			
			// the total time NumSamples / sampleRate * 1000 (ms) must be divided by numColumns
			double TimeslotWidth = (fftWindowsSize / (double) sampleRate) * 1000;
			double TimeslotIncrement = (ColSampleWidth / (double) sampleRate) * 1000;
			double timeIncrement = (NumSamples/sampleRate*1000) / NumCols;
			double[] T = new double[NumCols];
			for (int i = 1; i < NumCols + 1; i++) {
				T[i-1] = (double) i * timeIncrement; // in milliseconds
			}
			
			//Export.exportCSV(@"c:\VB-magnitude-freq.csv", frames[0], F);
			System.Console.Out.WriteLine(String.Format("TimeslotWidth: {0}, TimeslotIncrement: {1}.", TimeslotWidth, TimeslotIncrement));

			// save the Pixelmatrix as a bitmap file to disk
			SaveBitmap ("VB6", String.Format("C:\\Spectrogram-{0}x{1}", NumCols, fftWindowsSize / 2), Pixelmatrix, 8, LevelPaletteDictionary);
			
			return frames;
		}

		// Maps magnitudes in the range [-RangedB .. 0] dB to palette index values in the range [0 .. Rangeindex-1]
		// and computes and returns the index value which corresponds to passed-in magnitude Mag
		public static double MapToPixelIndex(double Mag, double RangedB, long Rangeindex)
		{
			double tempMapToPixelIndex = 0;
			double LevelIndB = 0;
			if (Mag == 0)
			{
				tempMapToPixelIndex = 0;
			}
			else
			{
				LevelIndB = 20 * Math.Log(Mag) / Log10;
				if (LevelIndB < -RangedB) {
					tempMapToPixelIndex = 0;
				} else if (LevelIndB > 0) {
					tempMapToPixelIndex  = Rangeindex;
				} else {
					tempMapToPixelIndex = Rangeindex * (LevelIndB + RangedB) / RangedB;
				}
			}
			return tempMapToPixelIndex;
		}

		// The inverse function of the above
		// Maps magnitudes in the range [-RangedB .. 0] dB to palette index values in the range [0 .. Rangeindex-1]
		// Computes and returns the Level in dB which coresponds to palette index Index
		private double PixelIndexToLevel(long Index, double RangedB, long Rangeindex)
		{
			return -RangedB + Index / (double)Rangeindex * RangedB;
		}
		
		// find the index in the pseudocolor palette which corresponds to the color of the pixel the
		// cursor is currently on and compute the level in dB
		// output this level together with the point's coordinates
		/*
		private void GetCursorText(long Cidx, long Curve, long Idx, double x, double Y, long Color, string Text)
		{
			double Level = 0;
			long i = 0;

			// brute force search, not the best of solutions, but it works
			for (i = 0; i < RangePaletteIndex; i++)
			{
				if (LevelPalette[i] == Color)
				{
					Level = PixelIndexToLevel(i, RangedB, RangePaletteIndex);
					Text = Text + "  z: " + Level.ToString("00.0 dB");
					break;
				}
			}
		}
		 */
		
		// return pseudo color value for a value x in range [0...range-1]
		// colors go from black - blue - green - red - violet - blue
		// this is just one of many possible palettes
		public static long PaletteValue(long x, long range)
		{
			double R = 0;
			double G = 0;
			double B = 0;
			double r4 = 0;
			double U = 0;

			r4 = range / 4.0;
			U = 255;

			if (x < r4)
			{
				B = x / r4;
				G = 0;
				R = 0;
			}
			else if (x < 2 * r4)
			{
				B = (1 - (x - r4) / r4);
				G = 1 - B;
				R = 0;
			}
			else if (x < 3 * r4)
			{
				B = 0;
				G = (2 - (x - r4) / r4);
				R = 1 - G;
			}
			else
			{
				B = (x - 3 * r4) / r4;
				G = 0;
				R = 1 - B;
			}

			R = (int)(Math.Sqrt(R) * U) & 0XFF;
			G = (int)(Math.Sqrt(G) * U) & 0XFF;
			B = (int)(Math.Sqrt(B) * U) & 0XFF;

			Color c = Color.FromArgb((int)R, (int)G, (int)B);
			long col = ColorUtils.ColorToLong(c);
			return col;
		}

		// return pseudo color value for a value x in range [0...range-1]
		// colors go from black - blue - green - red - violet - blue
		// this is just one of many possible palettes
		public static Color PaletteValueColor(long x, long range)
		{
			double R = 0;
			double G = 0;
			double B = 0;
			
			double r4 = 0;
			double U = 0;

			r4 = range / 4.0;
			U = 255;

			if (x < r4)
			{
				B = x / r4;
				G = 0;
				R = 0;
			}
			else if (x < 2 * r4)
			{
				B = (1 - (x - r4) / r4);
				G = 1 - B;
				R = 0;
			}
			else if (x < 3 * r4)
			{
				B = 0;
				G = (2 - (x - r4) / r4);
				R = 1 - G;
			}
			else
			{
				B = (x - 3 * r4) / r4;
				G = 0;
				R = 1 - B;
			}

			R = (int)(Math.Sqrt(R) * U) & 0XFF;
			G = (int)(Math.Sqrt(G) * U) & 0XFF;
			B = (int)(Math.Sqrt(B) * U) & 0XFF;

			Color c = Color.FromArgb((int)R, (int)G, (int)B);
			return c;
		}
		
		// return pseudo color value for a value x in range [0...range-1]
		// colors go from black - white
		// this is just one of many possible palettes
		public static long GreyPaletteValue(long x, long range)
		{
			long V = (x * 0XFF / range) & 0XFF;
			return V * 0X10101;
		}

		// return pseudo color value for a value x in range [0...range-1]
		// colors go from black - white
		// this is just one of many possible palettes
		public static Color GreyPaletteValueColor(long x, long range)
		{
			long V = (x * 0XFF / range) & 0XFF;
			int v = (int)(V * 0X10101);
			return ColorUtils.IntToColor(v);
		}

		private static long SwapColors(long col)
		{
			long Red = 0;
			long Green = 0;
			long Blue = 0;

			Red = (col / 0X10000) & 0XFF; // integer division!
			Green = (col / 0X100) & 0XFF;
			Blue = col & 0XFF;

			return Red + Green * 0X100 + Blue * 0X10000;
		}
		
		public static void SaveBitmap(string prefix, string filename, byte[,] Pixelmatrix, int BitsPerPixel, Dictionary<long, Color> PaletteDictionary)
		{
			int width = 0;
			int height = 0;
			int row = 0;
			int col = 0;
			long palettesize = 0;
			long NumQuadsPerRow = 0;
			
			height = Pixelmatrix.GetUpperBound(0) + 1;
			width = Pixelmatrix.GetUpperBound(1) + 1;
			palettesize = PaletteDictionary.Count;
			
			if (palettesize == 1)
			{
				palettesize = 0;
			}

			switch (BitsPerPixel)
			{
				case 24:
					NumQuadsPerRow = 3 * width / 4;
					if (4 * NumQuadsPerRow / 3 < width)
					{
						NumQuadsPerRow = NumQuadsPerRow + 1;
					}

					break;
				case 8:
					NumQuadsPerRow = width / 4;
					if (width % 4 > 0)
					{
						NumQuadsPerRow = NumQuadsPerRow + 1;
					}

					break;
				case 4:
					NumQuadsPerRow = width / 8;
					if (width % 8 > 0)
					{
						NumQuadsPerRow = NumQuadsPerRow + 1;
					}

					break;
				case 1:
					NumQuadsPerRow = width / 32;
					if (width % 32 > 0)
					{
						NumQuadsPerRow = NumQuadsPerRow + 1;
					}
					break;
			}
			
			try {
				String filenameToSave = String.Format("C:\\{0}-{1}.png", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Console.Out.WriteLine("Writing " + filenameToSave);

				Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				byte[] OneRow = new byte[(int)(4 * NumQuadsPerRow - 1) + 1];
				
				for(row = 0; row <= height - 1; row += 1)
				{
					for(col = 0; col <= width - 1; col += 1)
					{
						OneRow[col] = Convert.ToByte(Convert.ToInt32(Pixelmatrix[row, col]) & 0xFF);
						Color c = PaletteDictionary[OneRow[col]];
						png.SetPixel(col, height-row-1, c);
					}
				}
				
				png.Save(filenameToSave);
			} catch (Exception ex) {
				System.Console.Out.WriteLine(ex);
			}
			
		}
	}
}
