/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 02.08.2011
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Collections.Generic;

namespace Wave2ZebraSynth
{
	/// <summary>
	/// Description of VB6Spectrogram.
	/// </summary>
	public class VB6Spectrogram
	{
		private const int RangedB = 100;
		private const int RangePaletteIndex = 255;
		private static double Log10 = Math.Log(10); //2.30258509299405;

		//public long[] LevelPalette = new long[RangePaletteIndex];
		public Dictionary<long, Color> LevelPalette2 = new Dictionary<long, Color>();
		
		// computes the color palette for the pseudocolor display
		// creates a bitmap with the palette and loads it into chart #3
		public void ComputeColorPalette()
		{
			long col = 0;
			for (col = 0; col < RangePaletteIndex; col++)
			{
				//LevelPalette[col] = PaletteValue(col, RangePaletteIndex);
				LevelPalette2.Add(col, PaletteValue2(col, RangePaletteIndex));
			}
		}

		// loads a wav-file and displays the recorded signal in chart #1
		// computes the spectrogram of the signal
		// converts the pixelarray to a bmp-file
		// saves this file to disk and loads it into chart #2
		public static double[] Compute(float[] data, int numChannels, double sampleRate, int fftWindowsSize, float fftOverlapPercentage)
		{
			long NumSamples = 0;
			long NumCols = 0;			
			long ColIncrement = 0;
					
			long col;
			int c;
			if (true)
			{
				NumSamples = data.Length;

				//DynaPlot1.Axes.XAxis.From = 0;
				//DynaPlot1.Axes.XAxis.To = NumSamples / (double)SampleRate;

				// Add the new recorded signal
				for (c = 0; c < numChannels; c++)
				{
					//DynaPlot1.DataCurves.AddParametricCpp "Channel " + (c + 1), 0, 1 / SampleRate, Y(0, c), NumSamples;
				}

				ColIncrement = (long)(fftWindowsSize * (1 - fftOverlapPercentage));
				NumCols = NumSamples / ColIncrement;
				
				double[] real = new double[fftWindowsSize];
				double[] imag = new double[fftWindowsSize];
				double[] magnitude = new double[(int)(fftWindowsSize / 2.0)];
				byte[,] Pixelmatrix = new byte[fftWindowsSize / 2, NumCols];

				// make sure we don't step beyond the end of the recording
				while (NumCols * ColIncrement + fftWindowsSize > NumSamples)
				{
					NumCols = NumCols - 1;
					long sampleIndex = 0;
					for (col = 0; col < NumCols; col++)
					{
						// read a segment of the recorded signal
						for (c = 0; c < fftWindowsSize-1; c++)
						{
							imag[c] = 0;
							try {
								sampleIndex = col * ColIncrement + c;
								//real[c] = Y[col * ColIncrement + c, 0] * VB6Fourier.Hanning(fftWindowsSize, c);
								real[c] = data[sampleIndex] * VB6Fourier.Hanning(fftWindowsSize, c);
							} catch (System.IndexOutOfRangeException e) {
								// err
							}
						}

						// transform to the frequency domain
						VB6Fourier.FourierTransform(real, imag, fftWindowsSize, true);

						// and compute the magnitude spectrum
						VB6Fourier.MagnitudeSpectrum(real, imag, fftWindowsSize, VB6Fourier.W0Hanning, magnitude);

						// set up one column of the spectrogram
						for (c = 0; c < fftWindowsSize / 2.0; c++)
						{
							Pixelmatrix[c, col] = (byte) MapToPixelindex(magnitude[c], RangedB, RangePaletteIndex);
						}
					}

					// save the Pixelmatrix as a bitmap file to disk
					//SaveBitmap BitmapFilename, Pixelmatrix, 8, LevelPalette;

					//DynaPlot2.Axes.XAxis.From = DynaPlot1.Axes.XAxis.From;
					//DynaPlot2.Axes.XAxis.To = DynaPlot1.Axes.XAxis.To;

					//DynaPlot2.Axes.YAxis.From = 0;
					//DynaPlot2.Axes.YAxis.To = SampleRate / 2.0;

					// load the bitmap
					//TimeslotWidth = fftWindowsSize / (double)SampleRate;
					//TimeslotIncrement = ColIncrement / (double)SampleRate;
					//DynaPlot2.DataBitmaps.Add "Channel 1", BitmapFilename, TimeslotWidth / 2, 0, 1 / TimeslotIncrement, TimeslotWidth;
					
				}
				// double[] magnitude
				// byte[,] Pixelmatrix
				return magnitude;
			}
		}

		// Maps magnitudes in the range [-RangedB .. 0] dB to palette index values in the range [0 .. Rangeindex-1]
		// and computes and returns the index value which corresponds to passed-in magnitude Mag
		public static double MapToPixelindex(double Mag, double RangedB, long Rangeindex)
		{
			double tempMapToPixelindex = 0;
			double LevelIndB = 0;
			if (Mag == 0)
			{
				tempMapToPixelindex = 0;
			}
			else
			{
				LevelIndB = 20 * Math.Log(Mag) / Log10;
				if (LevelIndB < -RangedB)
				{
					tempMapToPixelindex = 0;
				}
				else
				{
					tempMapToPixelindex = Rangeindex * (LevelIndB + RangedB) / RangedB;
				}
			}
			return tempMapToPixelindex;
		}

		// The inverse function of the above
		// Maps magnitudes in the range [-RangedB .. 0] dB to palette index values in the range [0 .. Rangeindex-1]
		// Computes and returns the Level in dB which coresponds to palette index Index
		private double PixelindexToLevel(long Index, double RangedB, long Rangeindex)
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
					Level = PixelindexToLevel(i, RangedB, RangePaletteIndex);
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
			long col = AColor.ColorToLong(c);
			return col;
			//return (long)(R + G * 0X100 + B * 0X10000);
		}

		// return pseudo color value for a value x in range [0...range-1]
		// colors go from black - blue - green - red - violet - blue
		// this is just one of many possible palettes
		public static Color PaletteValue2(long x, long range)
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
			long V = 0;
			V = (x * 0XFF / range) & 0XFF;

			return V * 0X10101;
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
	}
}
