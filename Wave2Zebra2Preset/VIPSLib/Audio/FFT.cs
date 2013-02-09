using System;

/**
 * Class for computing a windowed fast Fourier transform.
 *  Implements some of the window functions for the STFT from
 *  Harris (1978), Proc. IEEE, 66, 1, 51-83.
 */
namespace VIPSLib.Audio
{
	public class FFT
	{
		#region Define
		/** used in {@link FFT#fft(double[], double[], int)} to specify
		 *  a forward Fourier transform */
		public const int FFT_FORWARD = -1;
		/** used in {@link FFT#fft(double[], double[], int)} to specify
		 * an inverse Fourier transform */
		public const int FFT_REVERSE = 1;
		/** used to specify a magnitude Fourier transform */
		public const int FFT_MAGNITUDE = 2;
		/** used to specify a magnitude phase Fourier transform */
		public const int FFT_MAGNITUDE_PHASE = 3;
		/** used to specify a normalized power Fourier transform */
		public const int FFT_NORMALIZED_POWER = 4;
		/** used to specify a power Fourier transform */
		public const int FFT_POWER = 5;
		/** used to specify a power phase Fourier transform */
		public const int FFT_POWER_PHASE = 6;
		/** used to specify a inline power phase Fourier transform */
		public const int FFT_INLINE_POWER_PHASE = 7;

		/** used to specify a rectangular window function */
		public const int WND_NONE = -1;
		/** used to specify a rectangular window function */
		public const int WND_RECT = 0;
		/** used to specify a Hamming window function */
		public const int WND_HAMMING = 1;
		/** used to specify a 61-dB 3-sample Blackman-Harris window function */
		public const int WND_BH3 = 2;
		/** used to specify a 74-dB 4-sample Blackman-Harris window function */
		public const int WND_BH4 = 3;
		/** used to specify a minimum 3-sample Blackman-Harris window function */
		public const int WND_BH3MIN = 4;
		/** used to specify a minimum 4-sample Blackman-Harris window function */
		public const int WND_BH4MIN = 5;
		/** used to specify a Gaussian window function */
		public const int WND_GAUSS = 6;
		/** used to specify a Hanning window function */
		public const int WND_HANNING = 7;
		/** used to specify a Hanning window function */
		public const int WND_USER_DEFINED = 8;
		#endregion

		private double[] windowFunction;
		private double windowFunctionSum;
		private int windowFunctionType;
		private int transformationType;
		private int windowSize;
		private const double PI2 = 2 * Math.PI;
		
		public FFT(int transformationType, int windowSize)
		{
			Initialize(transformationType, windowSize, WND_NONE, windowSize);
		}

		public FFT(int transformationType, int windowSize, int windowFunctionType)
		{
			Initialize(transformationType, windowSize, windowFunctionType, windowSize);
		}

		public FFT(int transformationType, int windowSize, int windowFunctionType, int support)
		{
			Initialize(transformationType, windowSize, windowFunctionType, support);
		}

		public FFT(int transformationType, int windowSize, double[] windowFunction)
		{
			Initialize(transformationType, windowSize, WND_NONE, windowSize);

			if(windowFunction.Length != windowSize)
			{
				throw new Exception("size of window function match window size");
			}
			else
			{
				this.windowFunction = windowFunction;
				this.windowFunctionType = WND_USER_DEFINED;
				CalculateWindowFunctionSum();
			}

		}
		
		private void Initialize(int transformationType, int windowSize, int windowFunctionType, int support)
		{
			//check and set fft type
			this.transformationType = transformationType;
			if(transformationType < -1 || transformationType > 7)
			{
				transformationType = FFT_FORWARD;
				throw new Exception("unknown fft type");
			}

			//check and set windowSize
			this.windowSize = windowSize;
			if (windowSize != (1 << ((int)Math.Round(Math.Log(windowSize) / Math.Log(2), 0))))
				throw new Exception("fft data must be power of 2");

			//create window function buffer and set window function
			this.windowFunction = new double[windowSize];
			SetWindowFunction(windowFunctionType, support);
		}
		
		public void Transform(double[] re, double[] im)
		{
			//check for correct size of the real part data array
			if(re.Length < windowSize)
				throw new Exception("data array smaller than fft window size");

			//apply the window function to the real part
			ApplyWindowFunction(re);

			//perform the transformation
			switch(transformationType)
			{
				case FFT_FORWARD:
					//check for correct size of the imaginary part data array
					if(im.Length < windowSize)
						throw new Exception("data array smaller than fft window size");
					else
						CalFFT(re, im, FFT_FORWARD);
					break;
				case FFT_INLINE_POWER_PHASE:
					if(im.Length < windowSize)
						throw new Exception("data array smaller than fft window size");
					else
						PowerPhaseIFFT(re, im);
					break;
				case FFT_MAGNITUDE:
					MagnitudeFFT(re);
					break;
				case FFT_MAGNITUDE_PHASE:
					if(im.Length < windowSize)
						throw new Exception("data array smaller than fft window size");
					else
						MagnitudePhaseFFT(re, im);
					break;
				case FFT_NORMALIZED_POWER:
					NormalizedPowerFFT(re);
					break;
				case FFT_POWER:
					PowerFFT(re);
					break;
				case FFT_POWER_PHASE:
					if(im.Length < windowSize)
						throw new Exception("data array smaller than fft window size");
					else
						PowerPhaseFFT(re, im);
					break;
				case FFT_REVERSE:
					if(im.Length < windowSize)
						throw new Exception("data array smaller than fft window size");
					else
						CalFFT(re, im, FFT_REVERSE);
					break;
			}
		}

		/** The FFT method. Calculation is inline, for complex data stored
		 *  in 2 separate arrays. Length of input data must be a power of two.
		 *  @param re        the real part of the complex input and output data
		 *  @param im        the imaginary part of the complex input and output data
		 *  @param direction the direction of the Fourier transform (FORWARD or
		 *  REVERSE)
		 *  @throws Exception if the length of the input data is
		 *  not a power of 2
		 */
		private void CalFFT(double[] re, double[] im, int direction)
		{
			int n = re.Length;
			int bits = (int)Math.Round(Math.Log(n) / Math.Log(2), 0);

			if (n != (1 << bits))
				throw new Exception("fft data must be power of 2");

			int localN;
			int j = 0;
			for (int i = 0; i < n-1; i++)
			{
				if (i < j)
				{
					double temp = re[j];
					re[j] = re[i];
					re[i] = temp;
					temp = im[j];
					im[j] = im[i];
					im[i] = temp;
				}

				int k = n / 2;

				while ((k >= 1) &&  (k - 1 < j))
				{
					j = j - k;
					k = k / 2;
				}

				j = j + k;
			}

			for(int m = 1; m <= bits; m++)
			{
				localN = 1 << m;
				double Wjk_r = 1;
				double Wjk_i = 0;
				double theta = PI2 / localN;
				double Wj_r = Math.Cos(theta);
				double Wj_i = direction * Math.Sin(theta);
				int nby2 = localN / 2;
				for (j = 0; j < nby2; j++)
				{
					for (int k = j; k < n; k += localN)
					{
						int id = k + nby2;
						double tempr = Wjk_r * re[id] - Wjk_i * im[id];
						double tempi = Wjk_r * im[id] + Wjk_i * re[id];
						re[id] = re[k] - tempr;
						im[id] = im[k] - tempi;
						re[k] += tempr;
						im[k] += tempi;
					}
					double wtemp = Wjk_r;
					Wjk_r = Wj_r * Wjk_r  - Wj_i * Wjk_i;
					Wjk_i = Wj_r * Wjk_i  + Wj_i * wtemp;
				}
			}
		}

		/** Computes the power spectrum of a real sequence (in place).
		 *  @param re the real input and output data; length must be a power of 2
		 */
		private void PowerFFT(double[] re)
		{
			double[] im = new double[re.Length];

			CalFFT(re, im, FFT_FORWARD);

			for (int i = 0; i < re.Length; i++)
				re[i] = re[i] * re[i] + im[i] * im[i];
		}

		/** Computes the magnitude spectrum of a real sequence (in place).
		 *  @param re the real input and output data; length must be a power of 2
		 */
		private void MagnitudeFFT(double[] re)
		{
			double[] im = new double[re.Length];

			CalFFT(re, im, FFT_FORWARD);

			for (int i = 0; i < re.Length; i++)
				re[i] = Math.Sqrt(re[i] * re[i] + im[i] * im[i]);
		}

		/** Computes the power spectrum of a real sequence (in place).
		 *  @param re the real input and output data; length must be a power of 2
		 */
		private void NormalizedPowerFFT(double[] re)
		{
			double[] im = new double[re.Length];
			double r, i;

			CalFFT(re, im, FFT_FORWARD);

			for (int j = 0; j < re.Length; j++)
			{
				r = re[j] / windowFunctionSum * 2;
				i = im[j] / windowFunctionSum * 2;
				re[j] = r * r + i * i;
			}
		}

		/** Converts a real power sequence from to magnitude representation,
		 *  by computing the square root of each value.
		 *  @param re the real input (power) and output (magnitude) data; length
		 *  must be a power of 2
		 */
		private void ToMagnitude(double[] re)
		{
			for (int i = 0; i < re.Length; i++)
				re[i] = Math.Sqrt(re[i]);
		}

		/** Computes a complex (or real if im[] == {0,...}) FFT and converts
		 *  the results to polar coordinates (power and phase). Both arrays
		 *  must be the same length, which is a power of 2.
		 *  @param re the real part of the input data and the power of the output
		 *  data
		 *  @param im the imaginary part of the input data and the phase of the
		 *  output data
		 */
		private void PowerPhaseFFT(double[] re, double[] im)
		{
			CalFFT(re, im, FFT_FORWARD);

			for (int i = 0; i < re.Length; i++)
			{
				double pow = re[i] * re[i] + im[i] * im[i];
				im[i] = Math.Atan2(im[i], re[i]);
				re[i] = pow;
			}
		}

		/** Inline computation of the inverse FFT given spectral input data
		 *  in polar coordinates (power and phase).
		 *  Both arrays must be the same length, which is a power of 2.
		 *  @param pow the power of the spectral input data (and real part of the
		 *  output data)
		 *  @param ph the phase of the spectral input data (and the imaginary part
		 *  of the output data)
		 */
		private void PowerPhaseIFFT(double[] pow, double[] ph)
		{
			ToMagnitude(pow);

			for (int i = 0; i < pow.Length; i++)
			{
				double re = pow[i] * Math.Cos(ph[i]);
				ph[i] = pow[i] * Math.Sin(ph[i]);
				pow[i] = re;
			}

			CalFFT(pow, ph, FFT_REVERSE);
		}

		/** Computes a complex (or real if im[] == {0,...}) FFT and converts
		 *  the results to polar coordinates (magnitude and phase). Both arrays
		 *  must be the same length, which is a power of 2.
		 *  @param re the real part of the input data and the magnitude of the
		 *  output data
		 *  @param im the imaginary part of the input data and the phase of the
		 *  output data
		 */
		private void MagnitudePhaseFFT(double[] re, double[] im)
		{
			PowerPhaseFFT(re, im);
			ToMagnitude(re);
		}

		/** Fill an array with the values of a standard Hamming window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void Hamming(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double scale = 1.0 / (double)size / 0.54;
			double factor = PI2 / (double)size;

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = scale * (25.0/46.0 - 21.0/46.0 * Math.Cos(factor * i));
		}

		/** Fill an array with the values of a standard Hanning window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void Hanning(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double factor = PI2 / (size - 1.0d);

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = 0.5 * (1 - Math.Cos(factor * i));
		}

		/** Fill an array with the values of a minimum 4-sample Blackman-Harris
		 *  window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void BlackmanHarris4sMin(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double scale = 1.0 / (double)size / 0.36;

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = scale * ( 0.35875 -
				                             0.48829 * Math.Cos(PI2 * i / size) +
				                             0.14128 * Math.Cos(2 * PI2 * i / size) -
				                             0.01168 * Math.Cos(3 * PI2 * i / size));
		}

		/** Fill an array with the values of a 74-dB 4-sample Blackman-Harris
		 *  window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void BlackmanHarris4s(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double scale = 1.0 / (double)size / 0.4;

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = scale * ( 0.40217 -
				                             0.49703 * Math.Cos(PI2 * i / size) +
				                             0.09392 * Math.Cos(2 * PI2 * i / size) -
				                             0.00183 * Math.Cos(3 * PI2 * i / size));
		}

		/** Fill an array with the values of a minimum 3-sample Blackman-Harris
		 *  window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void BlackmanHarris3sMin(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double scale = 1.0 / (double) size / 0.42;

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = scale * ( 0.42323 -
				                             0.49755 * Math.Cos(PI2 * i / size) +
				                             0.07922 * Math.Cos(2 * PI2 * i / size));
		}

		/** Fill an array with the values of a 61-dB 3-sample Blackman-Harris
		 *  window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void BlackmanHarris3s(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double scale = 1.0 / (double) size / 0.45;

			for (int i = 0; start < stop; start++, i++)
				windowFunction[i] = scale * ( 0.44959 -
				                             0.49364 * Math.Cos(PI2 * i / size) +
				                             0.05677 * Math.Cos(2 * PI2 * i / size));
		}

		/** Fill an array with the values of a Gaussian window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void Gauss(int size)
		{ // ?? between 61/3 and 74/4 BHW
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;
			double delta = 5.0 / size;
			double x = (1 - size) / 2.0 * delta;
			double c = -Math.PI * Math.Exp(1.0) / 10.0;
			double sum = 0;

			for (int i = start; i < stop; i++)
			{
				windowFunction[i] = Math.Exp(c * x * x);
				x += delta;
				sum += windowFunction[i];
			}

			for (int i = start; i < stop; i++)
				windowFunction[i] /= sum;
		}

		/** Fill an array with the values of a rectangular window function
		 *  @param data the array to be filled
		 *  @param size the number of non zero values; if the array is larger than
		 *  this, it is zero-padded symmetrically at both ends
		 */
		private void Rectangle(int size)
		{
			int start = (windowFunction.Length - size) / 2;
			int stop = (windowFunction.Length + size) / 2;

			for (int i = start; i < stop; i++)
				windowFunction[i] = 1.0 / (double) size;
		}

		/**
		 * This method allows to change the window function to one of the predefined
		 * window function types.
		 *
		 * @param windowFunctionType int the type of the window function
		 * @param support int
		 */
		public void SetWindowFunction(int windowFunctionType, int support)
		{
			if (support > windowSize)
				support = windowSize;

			switch (windowFunctionType)
			{
					case WND_NONE: 										break;
					case WND_RECT:		Rectangle(support);				break;
					case WND_HAMMING:	Hamming(support);				break;
					case WND_HANNING:   Hanning(support);				break;
					case WND_BH3:		BlackmanHarris3s(support);		break;
					case WND_BH4:		BlackmanHarris4s(support);		break;
					case WND_BH3MIN:	BlackmanHarris3sMin(support);	break;
					case WND_BH4MIN:	BlackmanHarris4sMin(support);	break;
					case WND_GAUSS:		Gauss(support);					break;
				default:
					windowFunctionType = WND_NONE;
					throw new Exception("unknown window function specified");
			}
			this.windowFunctionType = windowFunctionType;
			CalculateWindowFunctionSum();
		}

		public int GetTransformationType()
		{
			return transformationType;
		}

		public int GetWindowFunctionType()
		{
			return windowFunctionType;
		}

		/** Applies a window function to an array of data, storing the result in
		 *  the data array.
		 *  Performs a dot product of the data and window arrays.
		 *  @param data   the array of input data, also used for output
		 *  @param window the values of the window function to be applied to data
		 */
		private void ApplyWindowFunction(double[] data)
		{
			if(windowFunctionType != WND_NONE)
			{
				for (int i = 0; i < data.Length; i++)
					data[i] *= windowFunction[i];
			}
		}

		private void CalculateWindowFunctionSum()
		{
			windowFunctionSum = 0;
			for(int i = 0; i < windowFunction.Length; i++)
				windowFunctionSum += windowFunction[i];
		}
	}
}
