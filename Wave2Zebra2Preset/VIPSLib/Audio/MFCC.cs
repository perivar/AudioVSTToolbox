using VIPSLib.Maths;

using System;

/**
 * <b>Mel Frequency Cepstrum Coefficients - MFCCs</b>
 *
 * <p>Description: </p>
 * Computes the MFCC representation of a pcm signal. The signal is cut into
 * short overlapping frames, and for each frame, a feature vector is is computed,
 * which consists of Mel Frequency Cepstrum Coefficients.<br>
 * The cepstrum is the inverse Fourier transform of the log-spectrum. We call
 * mel-cepstrum the cepstrum computed after a non-linear frequency wrapping onto
 * a perceptual frequency scale, the Mel-frequency scale. Since it is a inverse
 * Fourier transform, the resulting coefficients are called Mel frequency
 * cepstrum coefficients (MFCC). Only the first few coefficients are used to
 * represent a frame. The number of coefficients is a an important parameter.
 * Therefore MFCCs provide a low-dimensional, smoothed version of the log
 * spectrum, and thus are a good and compact representation of the spectral shape.
 * They are widely used as features for speech recognition, and have also proved
 * useful in music instrument recognition [1].<br>
 *<br>
 * [1] Aucouturier, Pachet "Improving Trimbre Similarity: How high's the sky?",
 *     in Journal of Negative Results in Speech and Audio Sciences, 1(1), 2004.
 *
 *
 * @author Klaus Seyerlehner
 * @version 1.0
 * Ported to C# by royalgarter-lifetime-projects
 */
namespace VIPSLib.Audio
{
	public class MFCC
	{
		//general fields
		protected int windowSize;
		protected int hopSize;
		protected float sampleRate;
		protected double baseFreq;

		//fields concerning the mel filter banks
		protected double minFreq;
		protected double maxFreq;
		protected int numberFilters;

		//fields concerning the MFCCs settings
		protected int numberCoefficients;
		protected bool useFirstCoefficient;

		//implementation details
		private double[] inputData;
		private double[] buffer;
		private Matrix dctMatrix;
		private Matrix melFilterBanks;
		private FFT normalizedPowerFFT;

		/**
		 * Creates a new MFCC object with default window size of 512 for the given
		 * sample rate. The overleap of the windows is fixed at 50 percent. The number
		 * of coefficients is set to 20 and the first coefficient is in use. The
		 * 40 mel-filters are place in the range from 20 to 16000 Hz.
		 *
		 * @param sampleRate float sampes per second, must be greater than zero; not
		 *                         wohle-numbered values get rounded
		 * @throws Exception raised if mehtod contract is violated
		 */
		public MFCC(float sampleRate)
		{
			Initialize(sampleRate, 512, 20, true, 20.0, 16000.0, 40);
		}

		/**
		 * Creates a new MFCC object. 40 mel-filters are place in the range from 20 to
		 * 16000 Hz.
		 *
		 * @param sampleRate float sampes per second, must be greater than zero; not
		 *                         wohle-numbered values get rounded
		 * @param windowSize int size of window; must be 2^n and at least 32
		 * @param numberCoefficients int must be grate or equal to 1 and smaller than
		 *                               the number of filters
		 * @param useFirstCoefficient boolean indicates whether the first coefficient
		 *                                    of the dct process should be used in the
		 *                                    mfcc feature vector or not
		 * @throws Exception raised if mehtod contract is violated
		 */
		public MFCC(float sampleRate, int windowSize, int numberCoefficients, bool useFirstCoefficient)
		{
			Initialize(sampleRate, windowSize, numberCoefficients, useFirstCoefficient, 20.0, 16000.0, 40);
		}

		/**
		 * Creates a new MFCC object. 40 mel-filters are place in the range from 20 to
		 * 16000 Hz.
		 *
		 * @param sampleRate float sampes per second, must be greater than zero; not
		 *                         wohle-numbered values get rounded
		 * @param windowSize int size of window; must be 2^n and at least 32
		 * @param numberCoefficients int must be grate or equal to 1 and smaller than
		 *                               the number of filters
		 * @param useFirstCoefficient boolean indicates whether the first coefficient
		 *                                    of the dct process should be used in the
		 *                                    mfcc feature vector or not
		 * @param minFreq double start of the interval to place the mel-filters in
		 * @param maxFreq double end of the interval to place the mel-filters in
		 * @param numberFilters int number of mel-filters to place in the interval
		 * @throws Exception raised if mehtod contract is violated
		 */
		public MFCC(float sampleRate, int windowSize, int numberCoefficients, bool useFirstCoefficient, double minFreq, double maxFreq, int numberFilters)
		{
			Initialize(sampleRate, windowSize, numberCoefficients, useFirstCoefficient, minFreq, maxFreq, numberFilters);
		}
		
		private void Initialize(float sampleRate, int windowSize, int numberCoefficients, bool useFirstCoefficient, double minFreq, double maxFreq, int numberFilters)
		{
			//check for correct window size
			if(windowSize < 32)
			{
				throw new Exception("window size must be at least 32");
			}
			else
			{
				int i = 32;
				while(i < windowSize && i < Int32.MaxValue)
					i = i << 1;

				if(i != windowSize)
					throw new Exception("window size must be 2^n");
			}

			//check sample rate
			sampleRate = (float)Math.Round(sampleRate);
			if(sampleRate < 1)
				throw new Exception("sample rate must be at least 1");

			//check numberFilters
			if(numberFilters < 2 || numberFilters > (windowSize/2) + 1)
				throw new Exception("number filters must be at least 2 and smaller than the nyquist frequency");

			//check numberCoefficients
			if(numberCoefficients < 1 || numberCoefficients >= numberFilters)
				throw new Exception("the number of coefficients must be greater or equal to 1 and samller than the number of filters");

			//check minFreq/maxFreq
			if(minFreq <= 0 || minFreq > maxFreq || maxFreq > 88200.0f)
				throw new Exception("the min. frequency must be greater 0 smaller than the max. frequency, which must be smaller than 88200.0");;

			this.sampleRate = sampleRate;
			this.windowSize = windowSize;
			this.hopSize = windowSize/2; //50% Overleap
			this.baseFreq = sampleRate/windowSize;

			this.numberCoefficients = numberCoefficients;
			this.useFirstCoefficient = useFirstCoefficient;

			this.minFreq = minFreq;
			this.maxFreq = maxFreq;
			this.numberFilters = numberFilters;

			//create buffers
			inputData = new double[windowSize];
			buffer = new double[windowSize];

			//store filter weights and DCT matrix due to performance reason
			melFilterBanks = GetMelFilterBanks();
			dctMatrix = GetDCTMatrix();

			//create power fft object
			normalizedPowerFFT = new FFT(FFT.FFT_NORMALIZED_POWER, windowSize, FFT.WND_HANNING);
		}

		/**
		 * Returns the boundaries (start, center, end) of a given number of triangular
		 * mel filters at linear scale. Mel-filters are triangular filters on the
		 * linear scale with an integral (area) of 1. However they are placed
		 * equidistantly on the mel scale, which is non-linear rather logarithmic.
		 * The minimum linear frequency and the maximum linear frequency define the
		 * mel-scaled interval to equidistantly place the filters.
		 * Since mel-filters overlap, an array is used to efficiently store the
		 * boundaries of a filter. For example you can get the boundaries of the k-th
		 * filter by accessing the returned array as follows:
		 *
		 * leftBoundary = boundaries[k-1];
		 * center = boundaries[k];
		 * rightBoundary = boundaries[k+1];
		 *
		 * @param minFreq double frequency used for the left boundary of the first
		 *                       filter
		 * @param maxFreq double frequency used for the right boundary of the last
		 *                       filter
		 * @param numberFilters int number of filters to place within the interval
		 *                          [minFreq, maxFreq]
		 * @return double[] array countaining the boundaries
		 */
		private double[] GetMelFilterBankBoundaries(double minFreq, double maxFreq, int numberFilters)
		{
			//create return array
			double[] centers = new double[numberFilters + 2];
			double maxFreqMel, minFreqMel, deltaFreqMel, nextCenterMel;

			//compute mel min./max. frequency
			maxFreqMel = LinToMelFreq(maxFreq);
			minFreqMel = LinToMelFreq(minFreq);
			deltaFreqMel = (maxFreqMel - minFreqMel)/(numberFilters + 1);

			//create (numberFilters + 2) equidistant points for the triangles
			nextCenterMel = minFreqMel;
			for(int i = 0; i < centers.Length; i++)
			{
				//transform the points back to linear scale
				centers[i] = MelToLinFreq(nextCenterMel);
				nextCenterMel += deltaFreqMel;
			}

			//ajust boundaries to exactly fit the given min./max. frequency
			centers[0] = minFreq;
			centers[numberFilters + 1] = maxFreq;

			return centers;
		}

		/**
		 * This method creats a matrix containing <code>numberFilters</code>
		 * mel-filters. Each filter is represented by one row of this matrix. Thus all
		 * the filters can be applied at once by a simple matrix multiplication.
		 *
		 * @return Matrix a matrix containing the filter banks
		 */
		private Matrix GetMelFilterBanks()
		{
			//get boundaries of the different filters
			double[] boundaries = GetMelFilterBankBoundaries(minFreq, maxFreq, numberFilters);

			//ignore filters outside of spectrum
			for(int i = 1; i < boundaries.Length-1; i++)
			{
				if(boundaries[i] > sampleRate/2 )
				{
					numberFilters = i-1;
					break;
				}
			}

			//create the filter bank matrix
			double[][] matrix = new double[numberFilters][];

			//fill each row of the filter bank matrix with one triangular mel filter
			for(int i = 1; i <= numberFilters; i++)
			{
				double[] filter = new double[(windowSize/2)+1];

				//for each frequency of the fft
				for(int j = 0; j < filter.Length; j++)
				{
					//compute the filter weight of the current triangular mel filter
					double freq = baseFreq * j;
					filter[j] = GetMelFilterWeight(i, freq, boundaries);
				}

				//add the computed mel filter to the filter bank
				matrix[i-1] = filter;
			}

			//return the filter bank
			return new Matrix(matrix, numberFilters, (windowSize/2)+1);
		}

		/**
		 * Returns the filter weight of a given mel filter at a given freqency.
		 * Mel-filters are triangular filters on the linear scale with an integral
		 * (area) of 1. However they are placed equidistantly on the mel scale, which
		 * is non-linear rather logarithmic.
		 * Consequently there are lots of high, thin filters at start of the linear
		 * scale and rather few and flat filters at the end of the linear scale.
		 * Since the start-, center- and end-points of the triangular mel-filters on
		 * the linear scale are known, the weigths are computed using linear
		 * interpolation.
		 *
		 * @param filterBank int the number of the mel-filter, used to exract the
		 *                       boundaries of the filter from the array
		 * @param freq double    the frequency, at which the filter weight should be
		 *                       returned
		 * @param boundaries double[] an array containing all the boundaries
		 * @return double the filter weight
		 */
		private double GetMelFilterWeight(int filterBank, double freq, double[] boundaries)
		{
			//for most frequencies the filter weight is 0
			double result = 0;

			//compute start- , center- and endpoint as well as the height of the filter
			double start = boundaries[filterBank - 1];
			double center = boundaries[filterBank];
			double end = boundaries[filterBank + 1];
			double height = 2.0d/(end - start);

			//is the frequency within the triangular part of the filter
			if(freq >= start && freq <= end)
			{
				//depending on frequencys position within the triangle
				if(freq < center)
				{
					//...use a ascending linear function
					result = (freq - start) * (height/(center - start));
				}
				else
				{
					//..use a descending linear function
					result = height + ((freq - center) * (-height/(end - center)));
				}
			}

			return result;
		}

		/**
		 * Compute mel frequency from linear frequency.
		 *
		 * @param inputFreq the input frequency in linear scale
		 * @return the frequency in a mel scale
		 */
		private double LinToMelFreq(double inputFreq)
		{
			return (2595.0 * (Math.Log(1.0 + inputFreq / 700.0) / Math.Log(10.0)));
		}

		/**
		 * Compute linear frequency from mel frequency.
		 *
		 * @param inputFreq the input frequency in mel scale
		 * @return the frequency in a linear scale
		 */
		private double MelToLinFreq(double inputFreq)
		{
			return (700.0 * (Math.Pow(10.0, (inputFreq / 2595.0)) - 1.0));
		}

		/**
		 * Generates the DCT matrix for the known number of filters (input vector) and
		 * for the known number of used coefficients (output vector). Therfore the
		 * DCT matrix has the dimensions (numberCoefficients x numberFilters).
		 * If useFirstCoefficient is set to false the matrix dimensions are
		 * (numberCoefficients-1 x numberFilters). This matrix is a submatrix of the
		 * full matrix. Only the frist row is missing.
		 *
		 * @return Matrix the appropriate DCT matrix
		 */
		private Matrix GetDCTMatrix()
		{
			//compute constants
			double k = Math.PI/numberFilters;
			double w1 = 1.0/(Math.Sqrt(numberFilters));//1.0/(Math.Sqrt(numberFilters/2));
			double w2 = Math.Sqrt(2.0/numberFilters);//Math.Sqrt(2.0/numberFilters)*(Math.Sqrt(2.0)/2.0);

			//create new matrix
			Matrix matrix = new Matrix(numberCoefficients, numberFilters);

			//generate dct matrix
			for(int i = 0; i < numberCoefficients; i++)
			{
				for(int j = 0; j < numberFilters; j++)
				{
					if(i == 0)
						matrix.Set(i, j, w1 * Math.Cos(k*i*(j + 0.5d)));
					else
						matrix.Set(i, j, w2 * Math.Cos(k*i*(j + 0.5d)));
				}
			}

			//ajust index if we are using first coefficient
			if(!useFirstCoefficient)
				matrix = matrix.GetMatrix(1, numberCoefficients-1, 0, numberFilters-1);

			return matrix;
		}
		
		/**
		 * Performs the transformation of the input data to MFCCs.
		 * This is done by splitting the given data into windows and processing
		 * each of these windows with processWindow().
		 *
		 * @param input double[] input data is an array of samples, must be a multiple
		 *                       of the hop size, must not be a null value
		 * @return double[][] an array of arrays contains a double array of Sone value
		 *                    for each window
		 * @throws IOException if there are any problems regarding the inputstream
		 * @throws Exception raised if mehtod contract is violated
		 */
		public double[][] Process(double[] input)
		{
			//check for null
			if(input == null)
				throw new Exception("input data must not be a null value");

			//check for correct array length
			if ((input.Length % hopSize) != 0)
			{
				int lenNew = input.Length / hopSize * hopSize;
				Array.Resize<double>(ref input, lenNew);
				//throw new Exception("Input data must be multiple of hop size (windowSize/2).");
			}

			//create return array with appropriate size
			int len = (input.Length/hopSize)-1;
			double[][] mfcc = new double[len][];
			for (int i = 0; i < len; i++) {
				mfcc[i] = new double[numberCoefficients];
			}

			//process each window of this audio segment
			for(int i = 0, pos = 0; pos < input.Length - hopSize; i++, pos+=hopSize) {
				mfcc[i] = ProcessWindow(input, pos);
			}

			return mfcc;
		}

		/**
		 * Returns the window size.
		 *
		 * @return int the window size in samples
		 */
		public int GetWindowSize()
		{
			return windowSize;
		}

		/**
		 * Transforms one window of MFCCs. The following steps are
		 * performed: <br>
		 * <br>
		 * (1) normalized power fft with hanning window function<br>
		 * (2) convert to Mel scale by applying a mel filter bank<br>
		 * (3) convertion to db<br>
		 * (4) finally a DCT is performed to get the mfcc<br>
		 *<br>
		 * This process is mathematical identical with the process described in [1].
		 *
		 * @param window double[] data to be converted, must contain enough data for
		 *                        one window
		 * @param start int start index of the window data
		 * @return double[] the window representation in Sone
		 * @throws Exception raised if mehtod contract is violated
		 */
		public double[] ProcessWindow(double[] window, int start)
		{
			//number of unique coefficients, and the rest are symmetrically redundant
			int fftSize = (windowSize / 2) + 1;

			//check start
			if(start < 0)
				throw new Exception("start must be a positve value");

			//check window size
			if(window == null || window.Length - start < windowSize)
				throw new Exception("the given data array must not be a null value and must contain data for one window");

			//just copy to buffer
			for (int j = 0; j < windowSize; j++)
				buffer[j] = window[j + start];

			//perform power fft
			normalizedPowerFFT.Transform(buffer, null);

			//use all coefficient up to the nequist frequency (ceil((fftSize+1)/2))
			Matrix x = new Matrix(buffer, windowSize);
			x = x.GetMatrix(0, fftSize-1, 0, 0); //fftSize-1 is the index of the nyquist frequency

			//apply mel filter banks
			x = melFilterBanks.Times(x);

			//to db
			double log10 = 10 * (1 / Math.Log(10)); // log for base 10 and scale by factor 10
			x.ThrunkAtLowerBoundary(1);
			x.LogEquals();
			x.TimesEquals(log10);

			//compute DCT
			x = dctMatrix.Times(x);

			return x.GetColumnPackedCopy();
		}
	}
}
