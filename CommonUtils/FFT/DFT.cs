using System;
using Lomont;

namespace CommonUtils.FFT
{
	public class DFT
	{
		public const double PI = Math.PI; // 3.141593F;
		public const double TWO_PI = 2 * Math.PI; //6.283186F;

		/*
		 * Mimic the original DFT function that was a part of
		 * Wav2Zebra2Java. NB! SLOW VERSION
		 * Does a 2 x abs() / N before returning the value
		 * */
		public static float[] DFTSlow(float[] floatArray) {
			double[] doubleArray = Array.ConvertAll(floatArray, x => (double)x);
			double[] returnDoubleArray = DFTSlow(doubleArray);
			float[] returnfloatArray = Array.ConvertAll(returnDoubleArray, x => (float)x);
			return returnfloatArray;
		}
		
		/*
		 * Mimic the original DFT function that was a part of
		 * Wav2Zebra2Java. NB! SLOW VERSION
		 * Does a 2 x abs() / N before returning the value
		 * */
		public static double[] DFTSlow(double[] input)
		{
			int N = input.Length;
			double[] mag = new double[N];
			double[] c = new double[N]; // real (cosinus)
			double[] s = new double[N]; // imaginary (sinus)

			for(int i=0; i < N; i++) {
				for(int j=0; j < N; j++) {
					c[i] += input[j] * Math.Cos(i*j*TWO_PI/N);
					s[i] -= input[j] * Math.Sin(i*j*TWO_PI/N);
				}
				
				// normalize data
				c[i] /= N;
				s[i] /= N;

				// get power spectral density
				mag[i] = 2 * Math.Sqrt(c[i] * c[i] + s[i] * s[i]);
			}
			
			// rotate left (i.e. shift the first element so it becomes the last
			double temp = mag[0];
			Array.Copy(mag, 1, mag, 0, mag.Length - 1);
			mag[mag.Length-1] = temp;
			return mag;
		}
		
		/*
		 * Mimic the original DFT function that was a part of
		 * Wav2Zebra2Java.
		 * Does a 2 x abs() / N before returning the value
		 * */
		public static float[] DFTLomont(float[] input) {
			int N = input.Length;
			
			Lomont.LomontFFT fft = new Lomont.LomontFFT();
			
			// even - Re, odd - Img
			double[] complexSignal = new double[2 * N];
			for (int j = 0; j < N; j++) {
				complexSignal[2*j] = (double) input[j];
				complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer (phase)
			}
			
			fft.FFT(complexSignal, true);

			// get power spectral density
			float[] mag = new float[N];
			for (int j = 0; j < N; j++) {
				double re = complexSignal[2*j];
				double img = complexSignal[2*j + 1];
				mag[j] = (float) (2 * Math.Sqrt(re*re + img*img) / N);
			}

			// rotate left (i.e. shift the first element so it becomes the last
			float temp = mag[0];
			Array.Copy(mag, 1, mag, 0, mag.Length - 1);
			mag[mag.Length-1] = temp;
			return mag;
		}
		
	}
}