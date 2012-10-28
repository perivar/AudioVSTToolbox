using System;
using Lomont;

namespace CommonUtils.FFT
{
	/// <summary>
	/// FFTUtils is a class utilising the Lomont FFT class for FFT transforms 
	/// as well as having utility classes for converting from real arrays to complex arrays 
	/// as used by the exocortex FFT and Lomont FFT
	/// http://www.exocortex.org/dsp/
	/// http://www.lomont.org
	/// 
	/// perivar@nerseth.com
	/// </summary>
	public static class FFTUtils
	{
		/* This method duplicates exactly the function
		 * abs(fft(input)) in MATLAB
		 * The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		 * Input is e.g. an audio signal
		 * Returns a an array with frequency information.
		 * */
		public static double[] AbsFFT(double[] input) {
			double[] fftArray = FFT(input);
			return Abs(fftArray);
		}
		
		/* This method duplicates exactly the function
		 * abs(fft(input)) in MATLAB
		 * The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		 * Input is e.g. an audio signal
		 * Returns a an array with frequency information.
		 * */
		public static float[] AbsFFT(float[] floatArray) {
			double[] doubleArray = MathUtils.FloatToDouble(floatArray);
			double[] doubleArray2 = AbsFFT(doubleArray);
			return MathUtils.DoubleToFloat(doubleArray2);
		}
		
		/* This method duplicates exactly the function
		 * fft(input) in MATLAB
		 * Input is e.g. an audio signal
		 * Return a complex return arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * */
		public static double[] FFT(float[] floatArray) {
			double[] doubleArray = MathUtils.FloatToDouble(floatArray);
			return FFT(doubleArray);
		}
		
		/* This method duplicates exactly the function
		 * fft(input) in MATLAB
		 * Input is e.g. an audio signal
		 * Return a complex return arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * */
		public static double[] FFT(double[] input) {
			Lomont.LomontFFT fft = new Lomont.LomontFFT();
			double[] complexSignal = DoubleToComplexDouble(input);
			fft.FFT(complexSignal, true);
			return complexSignal;
		}

		/* This method duplicates exactly the function
		 * ifft(input) in MATLAB
		 * Requires a complex input number to be able to exactly
		 * transform back to an orginal signal
		 * i.e. x = ifft(fft(x))
		 * Parameter: inputComplex
		 * If true, the input array is a complex arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * If false, the array contains only real values
		 * Parameter: returnComplex
		 * If true, return a complex return arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * If false, return only the positive real value
		 * */
		public static float[] IFFT(float[] floatArray, bool inputComplex=true, bool returnComplex=true) {
			double[] doubleArray = MathUtils.FloatToDouble(floatArray);
			double[] doubleArray2 = IFFT(doubleArray, inputComplex, returnComplex);
			return MathUtils.DoubleToFloat(doubleArray2);
		}

		/* This method duplicates exactly the function
		 * ifft(input) in MATLAB
		 * Requires a complex input number to be able to exactly
		 * transform back to an orginal signal
		 * i.e. x = ifft(fft(x))
		 * Parameter: inputComplex
		 * If true, the input array is a complex arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * If false, the array contains only real values
		 * Parameter: returnComplex
		 * If true, return a complex return arrray.
		 * i.e. the array alternates between a real and an imaginary value
		 * If false, return only the positive real value
		 * */
		public static double[] IFFT(double[] input, bool inputComplex=true, bool returnComplex=true) {
			Lomont.LomontFFT fft = new Lomont.LomontFFT();
			
			double[] complexSignal;
			if (inputComplex) {
				complexSignal = input;
			} else {
				complexSignal = DoubleToComplexDouble(input);
			}
			
			fft.FFT(complexSignal, false);

			if (returnComplex) {
				return complexSignal;
			} else {
				return Real(complexSignal);
			}
		}
		
		/* This method duplicates the function
		 * abs(input) in MATLAB for a complex signal array
		 * i.e. the array alternates between a real and an imaginary value
		 * The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		 * */
		public static double[] Abs(double[] complexSignal, double scale=1) {
			int N = complexSignal.Length / 2;
			
			double[] abs = new double[N];
			for (int j = 0; j < N; j++) {
				double re = complexSignal[2*j];
				double img = complexSignal[2*j + 1];
				abs[j] = Math.Sqrt(re*re + img*img) * scale;
			}
			return abs;
		}
		
		/* This method duplicates the function
		 * abs(input) in MATLAB for a complex signal array
		 * i.e. the array alternates between a real and an imaginary value
		 * The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		 * */
		public static float[] Abs(float[] complexSignal, float scale=1) {
			int N = complexSignal.Length / 2;
			
			float[] abs = new float[N];
			for (int j = 0; j < N; j++) {
				float re = complexSignal[2*j];
				float img = complexSignal[2*j + 1];
				abs[j] = (float) Math.Sqrt(re*re + img*img) * scale;
			}
			return abs;
		}
		
		/* This method duplicates exactly the function
		 * real(input) in MATLAB
		 * Requires a complex input number array
		 * */
		public static double[] Real(double[] complexSignal, double scale=1) {
			int N = complexSignal.Length / 2;
			
			double[] returnArray = new double[N];
			for (int j = 0; j < N; j++) {
				double re = complexSignal[2*j] * scale;
				double img = complexSignal[2*j + 1] * scale;
				returnArray[j] = re;
			}
			return returnArray;
		}
		
		/* This method duplicates exactly the function
		 * real(input) in MATLAB
		 * Requires a complex input number array
		 * */
		public static float[] Real(float[] complexSignal, float scale=1) {
			int N = complexSignal.Length / 2;
			
			float[] returnArray = new float[N];
			for (int j = 0; j < N; j++) {
				float re = complexSignal[2*j] * scale;
				float img = complexSignal[2*j + 1] * scale;
				returnArray[j] = re;
			}
			return returnArray;
		}
		
		/* This method duplicates exactly the function
		 * imag(input) in MATLAB
		 * Requires a complex input number array
		 * */
		public static double[] Imag(double[] complexSignal, double scale=1) {
			int N = complexSignal.Length / 2;
			
			double[] returnArray = new double[N];
			for (int j = 0; j < N; j++) {
				double re = complexSignal[2*j] * scale;
				double img = complexSignal[2*j + 1] * scale;
				returnArray[j] = img;
			}
			return returnArray;
		}
		
		/* This method duplicates exactly the function
		 * imag(input) in MATLAB
		 * Requires a complex input number array
		 * */
		public static float[] Imag(float[] complexSignal, float scale=1) {
			int N = complexSignal.Length / 2;
			
			float[] returnArray = new float[N];
			for (int j = 0; j < N; j++) {
				float re = complexSignal[2*j] * scale;
				float img = complexSignal[2*j + 1] * scale;
				returnArray[j] = img;
			}
			return returnArray;
		}
		
		public static double[] DoubleToComplexDouble(double[] input) {
			// LomontFFT and ExocortexDSP requires a complex signal to work
			// i.e. the array alternates between a real and an imaginary value
			// even - Re, odd - Img
			int N = input.Length;
			double[] complexSignal = new double[2 * N];
			for (int j = 0; j < N; j++) {
				complexSignal[2*j] = (double) input[j];
				complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer (phase)
			}
			return complexSignal;
		}
		
		public static float[] FloatToComplexFloat(float[] input) {
			// LomontFFT and ExocortexDSP requires a complex signal to work
			// i.e. the array alternates between a real and an imaginary value
			// even - Re, odd - Img
			int N = input.Length;
			float[] complexSignal =  new float[2 * N];
			for (int j = 0; j < N; j++)
			{
				complexSignal[2*j] = (float) input[j];
				complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer (phase)
			}
			return complexSignal;
		}

		public static Complex[] DoubleToComplex(double[] input) {
			// LomontFFT and ExocortexDSP requires a complex signal to work
			// i.e. the array alternates between a real and an imaginary value
			// even - Re, odd - Img
			int N = input.Length;
			Complex[] complexSignal = new Complex[N];
			for (int j = 0; j < N; j++) {
				complexSignal[j] = new Complex(input[j], 0);
			}
			return complexSignal;
		}
		
		public static ComplexF[] FloatToComplex(float[] input) {
			// LomontFFT and ExocortexDSP requires a complex signal to work
			// i.e. the array alternates between a real and an imaginary value
			// even - Re, odd - Img
			int N = input.Length;
			ComplexF[] complexSignal =  new ComplexF[N];
			for (int j = 0; j < N; j++)
			{
				complexSignal[j] = new ComplexF(input[j], 0);
			}
			return complexSignal;
		}
		
		/// <summary>
		/// Convert a Half Complex Format to a Complex Format
		/// This method converts a half complex format to it's real components
		/// Requires a half complex input number array
		/// r0, r1, r2, ..., rn/2, i(n+1)/2-1, ..., i2, i1
		/// Here, rk is the real part of the kth output, and ikis the imaginary part. (Division by 2 is rounded down.)
		/// For a halfcomplex array hc[n], the kth component thus has its real part in hc[k] and its imaginary part in hc[n-k],
		/// with the exception of k == 0 or n/2 (the latter only if n is even)—in these two cases, the imaginary part is zero due to symmetries of the real-input DFT, and is not stored.
		/// Thus, the r2hc transform of n real values is a halfcomplex array of length n, and vice versa for hc2r.
		/// </summary>
		/// <param name="halfcomplex_coefficient"></param>
		/// <returns></returns>
		public static double[] HC2C(double[] halfcomplex_coefficient) {
			
			int n = halfcomplex_coefficient.Length;
			double[] complex_coefficient = new double[2 * n];
			
			complex_coefficient[0] = halfcomplex_coefficient[0];
			complex_coefficient[1] = 0.0;
			int i = 0;
			
			for (i = 1; i < n - i; i++)
			{
				double hc_real = halfcomplex_coefficient[i];
				double hc_imag = halfcomplex_coefficient[n-i];

				complex_coefficient[2*i] 		= hc_real;
				complex_coefficient[2*i + 1] 	= hc_imag;
				
				int endPos = 2*(n-i+1);
				complex_coefficient[endPos-2]	= hc_real;
				complex_coefficient[endPos-1] 	= -hc_imag;
			}
			
			if (i == n - i)
			{
				complex_coefficient[n] = halfcomplex_coefficient[i];
				complex_coefficient[n+1] = 0.0;
			}
			
			return complex_coefficient;
		}
		
	}
}

