/*
 * Copied from Audacity - FFT
 */
using System;

namespace CommonUtils.FFT
{
	/// <summary>
	/// Description of AudacityFFT.
	/// </summary>
	public class FFTWindowFunctions
	{
		public const int RECTANGULAR = 0;
		public const int BARTLETT = 1;
		public const int HAMMING = 2;
		public const int HANNING = 3;
		public const int BLACKMAN = 4;
		public const int BLACKMAN_HARRIS = 5;
		public const int WELCH = 6;
		public const int GAUSSIAN_2_5 = 7;
		public const int GAUSSIAN_3_5 = 8;
		public const int GAUSSIAN_4_5 = 9;
		
		/// <summary>
		///   Gets the corresponding window function
		/// </summary>
		/// <param name = "fftWindowsSize">Length of the window</param>
		/// <returns>Window function</returns>
		public static double[] GetWindowFunction(int whichFunction, int fftWindowsSize)
		{
			double[] array = new double[fftWindowsSize];
			
			// initialize to 1's
			for (int i = 0; i < fftWindowsSize; i++) {
				array[i] = 1;
			}
			ApplyWindowFunction(whichFunction, fftWindowsSize, array);
			return array;
		}
		
		/*
		 * Applies a windowing function to the data in place
		 *
		 * 0: Rectangular (no window)
		 * 1: Bartlett    (triangular)
		 * 2: Hamming
		 * 3: Hanning
		 * 4: Blackman
		 * 5: Blackman-Harris
		 * 6: Welch
		 * 7: Gaussian(a=2.5)
		 * 8: Gaussian(a=3.5)
		 * 9: Gaussian(a=4.5)
		 */
		/// <summary>
		///   Applies the corresponding window function
		/// </summary>
		/// <param name = "fftWindowsSize">Length of the window</param>
		/// <returns>Window function</returns>
		public static void ApplyWindowFunction(int whichFunction, int fftWindowsSize, double[] dataArray)
		{
			int i;
			double A;

			switch(whichFunction)
			{
				case 0:
					// Rectangular:
					// i.e. do nothing
					break;
				case 1:
					// Bartlett (triangular) window
					for (i = 0; i < fftWindowsSize / 2; i++)
					{
						dataArray[i] *= (i / (fftWindowsSize / 2));
						dataArray[i + (fftWindowsSize / 2)] *= (1.0f - (i / (fftWindowsSize / 2)));
					}
					break;
				case 2:
					// Hamming
					for (i = 0; i < fftWindowsSize; i++)
						dataArray[i] *= (0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (fftWindowsSize - 1)));
					break;
				case 3:
					// Hanning
					for (i = 0; i < fftWindowsSize; i++)
						dataArray[i] *= (0.50 - 0.50 * Math.Cos(2 * Math.PI * i / (fftWindowsSize - 1)));
					break;
				case 4:
					// Blackman
					for (i = 0; i < fftWindowsSize; i++)
					{
						dataArray[i] *= (0.42 - 0.5 * Math.Cos (2 * Math.PI * i / (fftWindowsSize - 1)) + 0.08 * Math.Cos (4 * Math.PI * i / (fftWindowsSize - 1)));
					}
					break;
				case 5:
					// Blackman-Harris
					for (i = 0; i < fftWindowsSize; i++)
					{
						dataArray[i] *= (0.35875 - 0.48829 * Math.Cos(2 * Math.PI * i /(fftWindowsSize-1)) + 0.14128 * Math.Cos(4 * Math.PI * i/(fftWindowsSize-1)) - 0.01168 * Math.Cos(6 * Math.PI * i/(fftWindowsSize-1)));
					}
					break;
				case 6:
					// Welch
					for (i = 0; i < fftWindowsSize; i++)
					{
						dataArray[i] *= 4 *i/fftWindowsSize*(1-(i/fftWindowsSize));
					}
					break;
				case 7:
					// Gaussian (a=2.5)
					// Precalculate some values, and simplify the fmla to try and reduce overhead
					A = -2 *2.5 *2.5;
					for (i = 0; i < fftWindowsSize; i++)
					{
						// full
						// in[i] *= exp(-0.5*(A*((i-fftWindowsSize/2)/fftWindowsSize/2))*(A*((i-fftWindowsSize/2)/fftWindowsSize/2)));
						// reduced
						dataArray[i] *= Math.Exp(A*(0.25 + ((i/fftWindowsSize)*(i/fftWindowsSize)) - (i/fftWindowsSize)));
					}
					break;
				case 8:
					// Gaussian (a=3.5)
					A = -2 *3.5 *3.5;
					for (i = 0; i < fftWindowsSize; i++)
					{
						// reduced
						dataArray[i] *= Math.Exp(A*(0.25 + ((i/fftWindowsSize)*(i/fftWindowsSize)) - (i/fftWindowsSize)));
					}
					break;
				case 9:
					// Gaussian (a=4.5)
					A = -2 *4.5 *4.5;
					for (i = 0; i < fftWindowsSize; i++)
					{
						// reduced
						dataArray[i] *= Math.Exp(A*(0.25 + ((i/fftWindowsSize)*(i/fftWindowsSize)) - (i/fftWindowsSize)));
					}
					break;
				default:
					//fprintf(stderr,"FFT::WindowFunc - Invalid window function: %d\n",whichFunction);
					break;
			}
		}

		/*
		 * Returns the name of the windowing function (for UI display)
		 */
		public static String FFTWindowFunctionName(int whichFunction)
		{
			switch (whichFunction)
			{
				default:
				case 0:
					return "Rectangular";
				case 1:
					return "Bartlett";
				case 2:
					return "Hamming";
				case 3:
					return "Hanning";
				case 4:
					return "Blackman";
				case 5:
					return "Blackman-Harris";
				case 6:
					return "Welch";
				case 7:
					return "Gaussian(a=2.5)";
				case 8:
					return "Gaussian(a=3.5)";
				case 9:
					return "Gaussian(a=4.5)";
			}
		}

		/*
		 * Returns the number of windowing functions supported
		 */
		public static int NumberOfWindowFunctions()
		{
			return 10;
		}
		
	}
}


