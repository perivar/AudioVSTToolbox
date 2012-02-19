using System;
using Lomont;

namespace CommonUtils.FFT
{
	/// <summary>
	/// Description of FFTUtils.
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
		 * i.e. The input data is a complex array, where in[i][0] is the real part and in[i][1] is the imaginary part.
		 * The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		 * */
		public static double[] Abs(double[][] complexSignal, double scale=1) {
			int N = complexSignal.Length;
			
			double[] abs = new double[N];
			for (int i = 0; i < N; i++) {
				double re = complexSignal[i][0] * scale;	// Real part
				double img = complexSignal[i][1] * scale; 	// Imaginary part
				abs[i] = Math.Sqrt(re*re + img*img) * scale;
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
		 * i.e. The input data is a complex array, where in[i][0] is the real part and in[i][1] is the imaginary part.
		 * */
		public static double[] Real(double[][] complexSignal, double scale=1) {
			int N = complexSignal.Length / 2;
			
			double[] returnArray = new double[N];
			for (int i = 0; i < N; i++) {
				double re = complexSignal[i][0] * scale;	// Real part
				double img = complexSignal[i][1] * scale; 	// Imaginary part
				returnArray[i] = re;
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
		 * i.e. The input data is a complex array, where in[i][0] is the real part and in[i][1] is the imaginary part.
		 * */
		public static double[] Imag(double[][] complexSignal, double scale=1) {
			int N = complexSignal.Length / 2;
			
			double[] returnArray = new double[N];
			for (int i = 0; i < N; i++) {
				double re = complexSignal[i][0] * scale;	// Real part
				double img = complexSignal[i][1] * scale; 	// Imaginary part
				returnArray[i] = img;
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

		public static double[][] DoubleToComplexDoubleMatrix(double[] input) {
			// FFTW needs a complex signal to work:
			// The input/output data is a complex array,
			// where in[i][0] is the real part and in[i][1] is the imaginary part.
			
			int N = input.Length;
			double[][] complexSignal = new double[N][];

			// Populate data for fftw input
			for (int i = 0; i < N; i++) {
				complexSignal[i] = new double[2];
				complexSignal[i][0] = (double) input[i];  	// Real part
				complexSignal[i][1] = 0;          			// Imaginary part
			}
			return complexSignal;
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
	}
}

