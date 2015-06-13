using System;
using System.Collections; // BitArray
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils
{
	/// <summary>
	/// Description of MathUtils.
	/// </summary>
	public static class MathUtils
	{
		// use in calculating log base 10. A log times this is a log base 10.
		private static double LOG10SCALE = 1 / Math.Log(10);
		
		// handy static methods
		public static double Log10(double val)
		{
			return Math.Log10(val);
		}
		public static double Exp10(double val)
		{
			return Math.Exp(val / LOG10SCALE);
		}
		public static float Log10Float(double val)
		{
			return (float)Log10(val);
		}
		public static float Exp10Float(double val)
		{
			return (float)Exp10(val);
		}
		
		#region PowerOfTwo
		/// <summary>
		/// Check if a given number is power of two
		/// </summary>
		/// <param name="x">the number of check</param>
		/// <returns>true or false</returns>
		public static bool IsPowerOfTwo(int x)
		{
			return (x != 0) && ((x & (x - 1)) == 0);
		}
		
		/// <summary>
		/// Return next power of two
		/// </summary>
		/// <param name="x">value</param>
		/// <returns>next power of two</returns>
		public static int NextPowerOfTwo(int x)
		{
			x--; // comment out to always take the next biggest power of two, even if x is already a power of two
			x |= (x >> 1);
			x |= (x >> 2);
			x |= (x >> 4);
			x |= (x >> 8);
			x |= (x >> 16);
			return (x+1);
		}

		/// <summary>
		/// Return next power of two
		/// </summary>
		/// <param name="x">value</param>
		/// <returns>next power of two</returns>
		public static int NextPow2(int x)
		{
			x = Math.Abs(x);
			if (x != 0)
				x--;

			int i = 0;
			while (x != 0)
			{
				x = x >> 1;
				i++;
			}

			return i;
		}
		
		/// <summary>
		/// Return previous power of two
		/// </summary>
		/// <param name="x">value</param>
		/// <returns>previous power of two</returns>
		public static int PreviousPowerOfTwo(int x) {
			if (x == 0) {
				return 0;
			}
			// x--; Uncomment this, if you want a strictly less than 'x' result.
			x |= (x >> 1);
			x |= (x >> 2);
			x |= (x >> 4);
			x |= (x >> 8);
			x |= (x >> 16);
			return x - (x >> 1);
		}
		#endregion
		
		#region Prime Number Methods
		/// <summary>
		/// This function is designed to look for the smallest integer that satisfies the
		/// following conditions:
		/// (i) it is greater than or equal to the argument 'number';
		/// (ii) it is a product only of the numbers 2, 3, 5 and 7.
		/// </summary>
		/// <param name="number">number to check for low prime factorisation</param>
		/// <param name="validSmallPrimes">an array of valid small primes to check for, e.g. int[] p = { 2, 3 }</param>
		/// <returns>the next low prime factor that is equal or higher than the argument</returns>
		/// <description>
		/// For faster FFT processing you can use buildingblocks that are multiples off small primes
		/// For example, the standard FFTW distribution works most efficiently for arrays
		/// whose size can be factored into small primes (2, 3, 5, and 7).
		/// I.e. the fftw transform works most efficiently on arrays which have dimensions
		/// which are products of small primes.
		///	</description>
		public static int NextLowPrimeFactorization(int number, int[] validSmallPrimes = null)
		{
			while (IsLowPrimeFactors(number, validSmallPrimes) != 1) {
				number++;
			}
			return number;
		}

		/// <summary>
		/// Check if the passed number is a multiple of small primes
		/// i.e. a product only of the numbers 2, 3, 5 and 7.
		/// </summary>
		/// <param name="number">number to check</param>
		/// <param name="validSmallPrimes">an array of valid small primes to check for, e.g. int[] p = { 2, 3 }</param>
		/// <returns>returns 1 if x is only made of these small primes</returns>
		public static int IsLowPrimeFactors(int number, int[] validSmallPrimes = null)
		{
			if (number < 2) return 0;
			
			if (validSmallPrimes == null) {
				validSmallPrimes = new int[] { 2, 3, 5, 7 }; // the small primes to check for
			}

			for (int i = 0; i < validSmallPrimes.Length; i++) {
				while (number % validSmallPrimes[i] == 0) {
					number /= validSmallPrimes[i];
				}
			}
			return number;
		}
		
		/// <summary>
		/// Calculate the prime factorization for a given number
		/// </summary>
		/// <param name="numbers">the number to factor</param>
		/// <returns>list of prime factors</returns>
		/// <see cref="http://www.vogella.com/tutorials/JavaAlgorithmsPrimeFactorization/article.html"/>
		public static List<int> PrimeFactors(int numbers) {
			int n = numbers;
			var factors = new List<int>();
			for (int i = 2; i <= n / i; i++) {
				while (n % i == 0) {
					factors.Add(i);
					n /= i;
				}
			}
			if (n > 1) {
				factors.Add(n);
			}
			return factors;
		}
		
		/// <summary>
		/// Check if number is a prime number
		/// </summary>
		/// <param name="number">number</param>
		/// <returns>true if number is a prime</returns>
		/// <see cref="http://stackoverflow.com/questions/17579091/faster-way-to-check-if-a-number-is-a-prime"/>
		/// <seealso cref="http://www.dotnetperls.com/prime"/>
		public static bool IsPrime(int number)
		{
			if (number < 2) return false;
			if (number % 2 == 0) return (number == 2);
			int root = (int)Math.Sqrt((double)number);
			for (int i = 3; i <= root; i += 2)
			{
				if (number % i == 0) return false;
			}
			return true;
		}
		
		/// <summary>
		/// Return a list of all primes up to and including the passed bound
		/// using the Sieve of Erathostenes method
		/// </summary>
		/// <param name="bound">highest (inclusive) possible number to return</param>
		/// <returns>a list of all primes up to the bound number</returns>
		/// <description>Finding prime numbers up to int.MaxValue takes about 45 seconds (and uses +/- 130Mb of memory).
		/// Using 1 million as bound will be lightning fast, though.
		/// For larger numbers, you need more advanced techniques.
		/// A popular one is the Miller-Rabin primality test.
		/// </description>
		public static IEnumerable<int> Primes(int bound)
		{
			if (bound < 2) yield break;
			//The first prime number is 2
			yield return 2;

			var composite = new BitArray((bound - 1) / 2);
			int limit = ((int)(Math.Sqrt(bound)) - 1) / 2;
			for (int i = 0; i < limit; i++) {
				if (composite[i]) continue;
				//The first number not crossed out is prime
				int prime = 2 * i + 3;
				yield return prime;
				//cross out all multiples of this prime, starting at the prime squared
				for (int j = (prime * prime - 2) >> 1; j < composite.Count; j += prime) {
					composite[j] = true;
				}
			}
			//The remaining numbers not crossed out are also prime
			for (int i = limit; i < composite.Count; i++) {
				if (!composite[i]) yield return 2 * i + 3;
			}
		}
		#endregion
		
		#region Normalize
		// normalize power (volume) of an audio file.
		// minimum and maximum rms to normalize from.
		// these values has been detected empirically
		private const float MINRMS = 0.1f;
		private const float MAXRMS = 3;

		/// <summary>
		/// Normalizing the input power (volume) to -1 to 1
		/// </summary>
		/// <param name="samples">Signal to be Normalized</param>
		public static void NormalizeInPlace(float[] samples)
		{
			double squares = samples.AsParallel().Aggregate<float, double>(0, (current, t) => current + (t * t));

			// we don't want to normalize by the real RMS, because excessive clipping will occur
			float rms = (float)Math.Sqrt(squares / samples.Length) * 10;
			
			if (rms < MINRMS)
				rms = MINRMS;
			if (rms > MAXRMS)
				rms = MAXRMS;

			for (int i = 0; i < samples.Length; i++) {
				samples[i] /= rms;
				samples[i] = Math.Min(samples[i], 1);
				samples[i] = Math.Max(samples[i], -1);
			}
		}
		
		/// <summary>
		/// Normalize the input signal to -1 to 1
		/// </summary>
		/// <param name="data">Signal to be Normalized</param>
		public static void Normalize(ref double[][] data)
		{
			// Find maximum number when all numbers are made positive.
			double max = data.Max((b) => b.Max((v) => Math.Abs(v)));
			
			if (max == 0.0f)
				return;

			// divide by max and return
			data = data.Select(i => i.Select(j => j/max).ToArray()).ToArray();
			
			// to normalize only positive numbers add Abs
			//data = data.Select(i => i.Select(j => Math.Abs(j)/max).ToArray()).ToArray();
		}

		/// <summary>
		/// Normalize the input signal to -1 to 1
		/// </summary>
		/// <param name="data">Signal to be Normalized</param>
		public static void Normalize(ref double[] data)
		{
			// Find maximum number when all numbers are made positive.
			double max = data.Max((b) => Math.Abs(b));
			
			if (max == 0.0f)
				return;

			// divide by max and return
			data = data.Select(i => i/max).ToArray();
		}
		
		/// <summary>
		/// Normalize the input signal to -1 to 1
		/// </summary>
		/// <param name="data">Signal to be Normalized</param>
		public static void Normalize(ref byte[] bytes) {
			
			// Find maximum number when all numbers are made positive.
			byte max = bytes.Max();
			
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] /= max;     	 // scale bytes to 0..1
				bytes[i] *= 2;            // scale bytes to 0..2
				bytes[i]--;               // scale bytes to -1..1
			}
		}
		
		/// <summary>
		/// To normalize a matrix such that all values fall in the range [0, 1]
		/// </summary>
		/// <param name="data">Signal to be Normalized</param>
		public static double[][] Normalize(double[][] data)
		{
			double min = data.Min(b => b.Min());
			double max = data.Max(b => b.Max());
			double[][] inorm = data.Select(i => i.Select(x => (x - min) / (max - min)).ToArray()).ToArray();
			return inorm;
		}
		#endregion
		
		#region Morph
		/// <summary>
		/// Morph between the arrays specified by the from and to index
		/// </summary>
		/// <param name="data">data array</param>
		/// <param name="fromIndex">first array to morph from</param>
		/// <param name="toIndex">last array to morph from</param>
		public static void Morph(ref float[][] data, int fromIndex, int toIndex)
		{
			float[] harmFrom = data[fromIndex];
			float[] harmTo = data[toIndex];
			
			int numArrays = data.Length; // 16
			int numSamples = data[0].Length; // 128
			
			var tempHarm = new float[numSamples];
			
			int steps = toIndex - fromIndex - 1;
			float stepSize = 1.0F / (steps + 1.0F);
			float stepHelper = 0.0F;
			for (int i = 0; i < steps; i++)
			{
				for (int j = 0; j < numSamples; j++)
				{
					stepHelper = stepSize * (i + 1);
					tempHarm[j] = (harmFrom[j] * (1.0F - stepHelper) + harmTo[j] * stepHelper);
				}
				int sanityCheck = fromIndex + i + 1;
				Array.Copy(tempHarm, 0, data[sanityCheck], 0, numSamples);
			}
		}
		#endregion
		
		#region Resample
		/// <summary>
		/// Resample a signal to a specific length
		/// </summary>
		/// <param name="input"></param>
		/// <param name="destLength"></param>
		/// <returns></returns>
		/// <remarks>This method is originally a part of Wav2Zebra2Java developed by
		/// jupiter8 from Gothenburg Sweden</remarks>
		/// <seealso cref="https://sites.google.com/site/wav2zebra2/"/>
		public static float[] ResampleToArbitrary(float[] input, int destLength)
		{
			var returnArray = new float[destLength];
			int length = input.Length;
			float phaseInc = (float) length / destLength;
			float phase = 0.0F;
			float phaseMant = 0.0F;
			
			for (int i = 0; i < destLength; i++)
			{
				int intPhase = (int) phase;
				int intPhasePlusOne = intPhase + 1;
				if (intPhasePlusOne >= length)
				{
					intPhasePlusOne -= length;
				}
				phaseMant = (float) phase - intPhase;
				returnArray[i] = (input[intPhase] * (1.0F - phaseMant) + input[intPhasePlusOne] * phaseMant);
				phase += phaseInc;
			}
			return returnArray;
		}
		
		/// <summary>
		/// Resample a signal to a specific sample rate
		/// </summary>
		/// <param name="input">original signal array</param>
		/// <param name="fromSampleRate">original sample rate</param>
		/// <param name="toSampleRate">new sample rate</param>
		/// <param name="quality">quality is half the window width</param>
		/// <returns>the resampled signal</returns>
		public static float[] Resample(float[] input, int fromSampleRate, int toSampleRate, int quality = 10) {

			int destLength = (int) ((double) input.Length * (double) toSampleRate / (double) fromSampleRate);
			return Resample(input, destLength, quality);
		}
		
		/// <summary>
		/// Resample a signal to a specific length
		/// </summary>
		/// <param name="input">original signal array</param>
		/// <param name="destLength">length of resampled signal</param>
		/// <param name="quality">quality is half the window width</param>
		/// <returns>the resampled signal</returns>
		/// <see cref="http://stackoverflow.com/questions/7995470/how-can-i-resample-wav-file"/>
		public static float[] Resample(float[] input, int destLength, int quality = 10) {
			
			int srcLength = input.Length;
			double dx = (double) srcLength / (double) destLength;

			var samples = new float[destLength];
			
			// fmax : Nyqist half of destination sampleRate
			// fmax / fsr = 0.5;
			const double fmaxDivSR = 0.5;
			const double r_g = 2 * fmaxDivSR;

			// quality is half the window width
			int wndWidth2 = quality;
			int wndWidth = quality * 2;

			double x = 0;
			int i, j = 0;
			double r_y;
			int tau;
			double r_w;
			double r_a;
			double r_snc;
			
			for (i=0; i < destLength; ++i)
			{
				r_y = 0.0;
				for (tau=-wndWidth2; tau < wndWidth2; ++tau)
				{
					// input sample index
					j = (int)(x+tau);

					// Hann Window. Scale and calculate sinc
					r_w = 0.5 - 0.5 * Math.Cos(2 * Math.PI * (0.5 + (j - x) / wndWidth));
					r_a = 2 * Math.PI * (j-x) * fmaxDivSR;
					r_snc = 1.0;
					if (r_a != 0)
						r_snc = Math.Sin(r_a)/r_a;

					if ((j >= 0) && (j < srcLength))
					{
						r_y += r_g * r_w * r_snc * input[j];
					}
				}
				samples[i] = (float) r_y;
				x += dx;
			}
			
			return samples;
		}
		#endregion
		
		#region ConvertRangeAndMaintainRatio
		
		/// <summary>
		/// Modify an array with new min and max values and return the modified array
		/// </summary>
		/// <param name="oldValueArray">original array</param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified array</returns>
		public static float[] ConvertRangeAndMainainRatio(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			var newValueArray = new float[oldValueArray.Length];
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float newValue = (((oldValueArray[x] - oldMin) * newRange) / oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}
		
		/// <summary>
		/// Modify an array with new min and max values and return the modified array using LOG values
		/// </summary>
		/// <param name="oldValueArray">original array</param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified array</returns>
		public static float[] ConvertRangeAndMainainRatioLog(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			var newValueArray = new float[oldValueArray.Length];
			
			// TODO: Addition of Epsilon prevents log from returning minus infinity if value is zero
			float newRange = (newMax - newMin);
			float log_oldMin = Log10Float(Math.Abs(oldMin) + float.Epsilon);
			float log_oldMax = Log10Float(oldMax + float.Epsilon);
			float oldRange = (oldMax - oldMin);
			float log_oldRange = (log_oldMax - log_oldMin);
			float data_per_log_unit = newRange / log_oldRange;
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float log_oldValue = Log10Float(oldValueArray[x] + float.Epsilon);
				float newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}
		
		/// <summary>
		/// Modify an array with new min and max values and return the modified array
		/// </summary>
		/// <param name="oldValueArray">original array</param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified array</returns>
		public static double[] ConvertRangeAndMainainRatio(double[] oldValueArray, double oldMin, double oldMax, double newMin, double newMax) {
			var newValueArray = new double[oldValueArray.Length];
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				double newValue = (((oldValueArray[x] - oldMin) * newRange) / oldRange) + newMin;
				newValueArray[x] = newValue;
			}
			
			return newValueArray;
		}
		#endregion
		
		#region ConvertAndMaintainRatio and Scale methods
		/// <summary>
		/// Change a value with new min and max values
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified value</returns>
		public static double ConvertAndMainainRatio(double oldValue, double oldMin, double oldMax, double newMin, double newMax) {
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			double newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}

		/// <summary>
		/// Change a value with new min and max values
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified value</returns>
		public static float ConvertAndMainainRatio(float oldValue, float oldMin, float oldMax, float newMin, float newMax) {
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}
		
		/// <summary>
		/// Change a value with new min and max values using LOG scale
		/// Does not seem to work if oldMin is a minus value and oldValue also is minus
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified value</returns>
		public static double ConvertAndMainainRatioLog(double oldValue, double oldMin, double oldMax, double newMin, double newMax) {
			// // TODO: Does not seem to work if oldMin is a minus value and oldValue also is minus
			
			// Addition of Epsilon prevents log from returning minus infinity if value is zero
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			double log_oldMin = Log10(Math.Abs(oldMin) + double.Epsilon);
			double log_oldMax = Log10(oldMax + double.Epsilon);
			double log_oldRange = (log_oldMax - log_oldMin);
			//double data_per_log_unit = newRange / log_oldRange;
			double log_oldValue = Log10(Math.Abs(oldValue) + double.Epsilon);
			double newValue = (((log_oldValue - log_oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}

		/// <summary>
		/// Change a value with new min and max values using LOG scale
		/// </summary>
		/// <param name="oldValue"></param>
		/// <param name="oldMin">original minimum value</param>
		/// <param name="oldMax">original maxiumum value</param>
		/// <param name="newMin">new minimum value</param>
		/// <param name="newMax">new maxiumum value</param>
		/// <returns>the modified value</returns>
		public static float ConvertAndMainainRatioLog(float oldValue, float oldMin, float oldMax, float newMin, float newMax) {
			// Addition of Epsilon prevents log from returning minus infinity if value is zero
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float log_oldMin = Log10Float(Math.Abs(oldMin) + float.Epsilon);
			float log_oldMax = Log10Float(oldMax + float.Epsilon);
			float log_oldRange = (log_oldMax - log_oldMin);
			//float data_per_log_unit = newRange / log_oldRange;
			float log_oldValue = Log10Float(oldValue + float.Epsilon);
			float newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
			return newValue;
		}
		
		/// <summary>
		/// Scale data from one format to another (similar to ConvertRangeAndMainainRatio)
		/// </summary>
		/// <example>
		/// double [-1,1] to int [0-255]
		/// int[] integers = doubles.Select(x => x.Scale(-1,1,0,255)).ToArray();
		/// 
		/// int [0-255] to double [-1,1]
		/// double[] doubles = integers.Select(x => ((double)x).Scale(0,255,-1,1)).ToArray();
		/// 
		/// double min = jaggedArray.Min(b => b.Min());
		/// double max = jaggedArray.Max(b => b.Max());
		/// double[][] doubles = jaggedArray.Select(i => i.Select(j => j.Scale(min, max, 0, 255)).ToArray()).ToArray();
		/// </example>
		/// <see cref="http://stackoverflow.com/questions/5383937/array-data-normalization">Array Data Normalization</see>
		/// <param name="elementToScale">double value</param>
		/// <param name="rangeMin">original min value</param>
		/// <param name="rangeMax">original max value</param>
		/// <param name="scaledRangeMin">new min value</param>
		/// <param name="scaledRangeMax">new max value</param>
		/// <returns></returns>
		public static double Scale(this double elementToScale,
		                           double rangeMin, double rangeMax,
		                           double scaledRangeMin, double scaledRangeMax)
		{
			var scaled = scaledRangeMin + ((elementToScale - rangeMin) * (scaledRangeMax - scaledRangeMin) / (rangeMax - rangeMin));
			return scaled;
		}
		#endregion
		
		#region Jagged Array init using Linq
		
		/// <summary>
		/// Initialize a jagged array in many dimensions
		/// </summary>
		/// <example>
		/// double[][] my2DArray = CreateJaggedArray<double[][]>(noOfRows, noOfCols);
		/// int[][][] my3DArray = CreateJaggedArray<int[][][]>(1, 2, 3);
		/// </example>
		/// <param name="lengths">array that contain the length of each dimension</param>
		/// <returns>a initialized multidimensional jagged array</returns>
		public static T CreateJaggedArray<T>(params int[] lengths)
		{
			return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths);
		}

		/// <summary>
		/// Method that initialize a jagged array
		/// </summary>
		/// <param name="type">unit of measure</param>
		/// <param name="index">index</param>
		/// <param name="lengths">array that contain the length of each dimension</param>
		/// <returns>an object</returns>
		private static object InitializeJaggedArray(Type type, int index, int[] lengths)
		{
			Array array = Array.CreateInstance(type, lengths[index]);
			Type elementType = type.GetElementType();

			if (elementType != null)
			{
				for (int i = 0; i < lengths[index]; i++)
				{
					array.SetValue(
						InitializeJaggedArray(elementType, index + 1, lengths), i);
				}
			}

			return array;
		}
		#endregion
		
		#region Round
		/// <summary>
		/// Round a number to nearest double value
		/// </summary>
		/// <param name="number">original value</param>
		/// <param name="nearest">value to get near to</param>
		/// <returns>rounded number</returns>
		public static double RoundToNearest(double number, double nearest) {
			// use AwayFromZero since default rounding is "round to even", which would make 1.5 => 1 
			double rounded = Math.Round(number * (1 / nearest), MidpointRounding.AwayFromZero) / (1 / nearest);
			return rounded;
		}
		
		/// <summary>
		/// Round a number to nearest integer value
		/// </summary>
		/// <param name="number">original value</param>
		/// <param name="nearest">value to get near to</param>
		/// <returns>rounded number</returns>
		public static int RoundToNearestInteger(int number, int nearest) {
			// use AwayFromZero since default rounding is "round to even", which would make 1.5 => 1 
			int rounded = (int) Math.Round( (double) number / nearest, MidpointRounding.AwayFromZero) * nearest;
			return rounded;
		}

		/// <summary>
		/// Round a number down with specific number of decimal places
		/// </summary>
		/// <param name="number">original value</param>
		/// <param name="decimalPlaces">number of decimal places</param>
		/// <returns>rounded number</returns>
		public static double RoundDown(double number, int decimalPlaces)
		{
			return Math.Floor(number * Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces);
		}
		
		/// <summary>
		/// Returns the largest integer less than or equal to the specified double-precision floating-point number.
		/// </summary>
		/// <param name="number">A double-precision floating-point number.</param>
		/// <returns>The largest integer less than or equal to 'number'.
		/// If d is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned.</returns>
		public static double RoundDown(double number) {
			return Math.Floor(number);
		}

		/// <summary>
		/// Returns the smallest integral value that is greater than or equal to the specified double-precision floating-point number.
		/// </summary>
		/// <param name="number">A double-precision floating-point number.</param>
		/// <returns>The smallest integral value that is greater than or equal to 'number'.
		/// If a is equal to NaN, NegativeInfinity, or PositiveInfinity, that value is returned.
		/// Note that this method returns a Double instead of an integral type.</returns>
		public static double RoundUp(double number) {
			return Math.Ceiling(number);
		}
		#endregion
		
		#region ComputeMinAndMax
		/// <summary>
		/// Find min and max values from array
		/// </summary>
		/// <param name="data">array</param>
		/// <param name="min">outputted min value</param>
		/// <param name="max">outputted max value</param>
		public static void ComputeMinAndMax(double[,] data, out double min, out double max) {
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.GetLength(0); x++)
			{
				for(int y = 0; y < data.GetLength(1); y++)
				{
					if (data[x,y] > maxVal)
						maxVal = data[x,y];
					if (data[x,y] < minVal)
						minVal = data[x,y];
				}
			}
			min = minVal;
			max = maxVal;
		}

		/// <summary>
		/// Find min and max values from array
		/// </summary>
		/// <param name="data">array</param>
		/// <param name="min">outputted min value</param>
		/// <param name="max">outputted max value</param>
		public static void ComputeMinAndMax(double[] data, out double min, out double max) {
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				if (data[x] > maxVal)
					maxVal = data[x];
				if (data[x] < minVal)
					minVal = data[x];
			}
			min = minVal;
			max = maxVal;
		}
		
		/// <summary>
		/// Find min and max values from array
		/// </summary>
		/// <param name="data">array</param>
		/// <param name="min">outputted min value</param>
		/// <param name="max">outputted max value</param>
		public static void ComputeMinAndMax(double[][] data, out double min, out double max) {
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal)
						maxVal = data[x][y];
					if (data[x][y] < minVal)
						minVal = data[x][y];
				}
			}
			min = minVal;
			max = maxVal;
		}

		/// <summary>
		/// Find min and max values from array
		/// </summary>
		/// <param name="data">array</param>
		/// <param name="min">outputted min value</param>
		/// <param name="max">outputted max value</param>
		public static void ComputeMinAndMax(float[] data, out float min, out float max) {
			// prepare the data:
			float maxVal = float.MinValue;
			float minVal = float.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				if (data[x] > maxVal)
					maxVal = data[x];
				if (data[x] < minVal)
					minVal = data[x];
			}
			min = minVal;
			max = maxVal;
		}
		
		/// <summary>
		/// Find min and max values from array
		/// </summary>
		/// <param name="data">array</param>
		/// <param name="min">outputted min value</param>
		/// <param name="max">outputted max value</param>
		public static void ComputeMinAndMax(float[][] data, out float min, out float max) {
			// prepare the data:
			float maxVal = float.MinValue;
			float minVal = float.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal)
						maxVal = data[x][y];
					if (data[x][y] < minVal)
						minVal = data[x][y];
				}
			}
			min = minVal;
			max = maxVal;
		}
		#endregion
		
		#region ConvertDecibel
		
		/// <summary>
		/// Convert amplitude in the range between 0, 1 to decibel
		/// </summary>
		/// <param name="amplitude">amplitude</param>
		/// <param name="minDb">minimum dB allowed</param>
		/// <param name="maxDb">maximum dB allowed</param>
		/// <seealso cref="http://jvalentino2.tripod.com/dft/index.html"></seealso>
		/// <returns>decibel value</returns>
		public static float AmplitudeToDecibel(float amplitude, float minDb, float maxDb) {
			float db = AmplitudeToDecibel(amplitude);
			
			if (db < minDb) db = minDb;
			if (db > maxDb) db = maxDb;
			
			return db;
		}
		
		/// <summary>
		/// Convert amplitude in the range between 0, 1 to decibel
		/// </summary>
		/// <param name="amplitude">amplitude</param>
		/// <remarks>
		/// As for decibels, their relation to amplitude is:
		/// db 			~ 20 * log10 (amplitude),
		/// amplitude 	~ 10 ^ (dB/20)
		/// </remarks>
		/// <seealso cref="http://www.plugindeveloper.com/05/decibel-calculator-online"></seealso>
		/// <returns>decibel value</returns>
		public static float AmplitudeToDecibel(float amplitude) {
			// 20 log10 (mag) => 20/ln(10) ln(mag)
			// javascript: var result = Math.log(x) * (20.0 / Math.LN10);
			// float result = Math.Log(x) * (20.0 / Math.Log(10));
			
			// Addition of smallestNumber prevents log from returning minus infinity if mag is zero
			const float smallestNumber = float.Epsilon;
			float db = 20 * (float) Math.Log10( (float) (amplitude + smallestNumber) );
			return (float) db;
		}

		/// <summary>
		/// Convert decibel to amplitude in the range between 0, 1
		/// </summary>
		/// <param name="dB">value in decibel</param>
		/// <remarks>
		/// As for decibels, their relation to amplitude is:
		/// db 			~ 20 * log10 (amplitude),
		/// amplitude 	~ 10 ^ (dB/20)
		/// </remarks>
		/// <seealso cref="http://www.plugindeveloper.com/05/decibel-calculator-online"></seealso>
		/// <returns>amplitude in the range between 0, 1</returns>
		public static float DecibelToAmplitude(float dB) {
			// javascript: var result = Math.exp((x) * (Math.LN10 / 20.0));
			//double result = Math.Exp(( dB) * (Math.Log(10) / 20.0));
			
			double result = Exp10(dB / 20.0);
			return (float) result;
		}
		#endregion

		#region Time, Index and Freq Conversion
		/// <summary>
		/// Return the frequency in Hz for each index in FFT
		///
		///	The first bin in the FFT is DC (0 Hz), the second bin is Fs / N, where Fs is the sample rate and N is the size of the FFT.
		/// The next bin is 2 * Fs / N. To express this in general terms, the nth bin is n * Fs / N.
		///
		///	So if your sample rate, Fs is say 44.1 kHz and your FFT size, N is 1024, then the FFT output bins are at:
		///
		///	  0:   0 * 44100 / 1024 =     0.0 Hz
		///	  1:   1 * 44100 / 1024 =    43.1 Hz
		///	  2:   2 * 44100 / 1024 =    86.1 Hz
		///	  3:   3 * 44100 / 1024 =   129.2 Hz
		///	  4: ...
		///	  5: ...
		///		 ...
		///	511: 511 * 44100 / 1024 = 22006.9 Hz
		/// 512: 512 * 44100 / 1024 = 22050.0 Hz, the nyquist limit
		///
		/// Note that for a real input signal (imaginary parts all zero) the second half of the FFT (bins from N / 2 + 1 to N - 1)
		/// contain no useful additional information (they have complex conjugate symmetry with the first N / 2 - 1 bins).
		/// The last useful bin (for practical aplications) is at N / 2 - 1, which corresponds to 22006.9 Hz in the above example.
		/// The bin at N / 2 represents energy at the Nyquist frequency, i.e. Fs / 2 ( = 22050 Hz in this example),
		/// but this is in general not of any practical use, since anti-aliasing filters will typically attenuate any signals at and above Fs / 2.
		/// </summary>
		/// <param name="i">index in the FFT spectrum</param>
		/// <param name = "sampleRate">Frequency rate at which the signal was processed [E.g. 5512Hz]</param>
		/// <param name = "fftDataSize">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
		/// <returns>Frequency in Hz</returns>
		/// <remarks>
		/// Real frequencies are mapped to k as follows:
		/// 
		/// F = k*Fs/N  for k = 0 ... N/2-1 ((N-1)/2 for odd N)
		/// 
		/// or
		/// 
		/// k = F*N/Fs  for F = 0Hz ... Fs/2-Fs/N
		/// where F is the frequency in Hz, N is the FFT size, and Fs is the sampling frequency (Hz).
		/// 
		/// Some things to note:
		/// * k is an integer, so not all frequencies will map to an integer k. Find the closest k
		/// * If you need more frequency resolution, increase N.
		/// * Signals sampled at Fs are only able to accurately represent frequencies up to, but not including Fs/2 (Nyquist rate). This is why I showed that the mapping from k to Hz is only good for half the output samples. I will not go into what the second half represents (it will actually be a mirror image of the first half for a real input signal)
		/// * The output of the DFT/FFT is complex. You most likely want to take the magnitude of this.
		/// * If you need to compute even a few DFT outputs, it may be better to just use the FFT function available and get all the output samples instead of calculating just the output samples you need using the DFT. The reason is that most FFT algorithms are heavily optimized so even though you may be theoretically doing less work, it may take longer than the FFT. You would probably just have to benchmark this to see which approach is better.
		/// </remarks>
		/// <seealso cref="http://stackoverflow.com/questions/2921674/determining-the-magnitude-of-a-certain-frequency-on-the-iphone"></seealso>
		public static double IndexToFreq(int i, double sampleRate, int fftDataSize) {
			return (double) i * (sampleRate / fftDataSize);
		}
		
		/// <summary>
		/// Gets the index in the spectrum vector according to the frequency specified as the parameter
		/// </summary>
		/// <param name = "frequency">Frequency to be found in the spectrum vector [E.g. 300 Hz]</param>
		/// <param name = "sampleRate">Frequency rate at which the signal was processed [E.g. 44100 Hz]</param>
		/// <param name = "fftDataSize">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
		/// <returns>Index of the frequency in the spectrum array</returns>
		/// <see cref="IndexToFreq">See the converse method IndexToFreq for more details</see>
		public static int FreqToIndex(double frequency, double sampleRate, int fftDataSize)
		{
			//return (int) (frequency * fftDataSize / sampleRate);
			// added rounding - taken from https://github.com/wo80/AcoustID.NET/blob/master/AcoustID/Util/Helper.cs
			return (int) Math.Round(frequency * fftDataSize / sampleRate);
		}
		
		/// <summary>
		/// Convert samplerate and number of samples to seconds
		/// </summary>
		/// <param name="sampleRate">samplerate</param>
		/// <param name="numberOfSamples">number of samples</param>
		/// <returns>time in seconds</returns>
		public static double ConvertToTime(double sampleRate, int numberOfSamples) {
			return numberOfSamples / sampleRate;
		}
		#endregion
		
		#region Float, Int And Double Conversions
		/// <summary>
		/// Convert float array to int array
		/// </summary>
		/// <param name="floatArray">original array</param>
		/// <returns>converted array</returns>
		public static int[] FloatToInt(float[] floatArray) {
			int[] intArray = Array.ConvertAll(floatArray, x => (int)(float)x);
			return intArray;
		}

		/// <summary>
		/// Convert double array to int array
		/// </summary>
		/// <param name="doubleArray">original array</param>
		/// <returns>converted array</returns>
		public static int[] DoubleToInt(double[] doubleArray) {
			// http://stackoverflow.com/questions/2103944/json-to-c-sharp-convert-an-arraylist-of-doubles-to-an-array-of-ints
			int[] intArray = Array.ConvertAll(doubleArray, x => (int)(double)x);
			// Note the cast is framed as (int)(double).
			// This first unboxes the boxed double and then casts to an int.
			return intArray;
		}

		/// <summary>
		/// Convert int array to float array
		/// </summary>
		/// <param name="intArray">original array</param>
		/// <returns>converted array</returns>
		public static float[] IntToFloat(int[] intArray) {
			float[] floatArray = Array.ConvertAll(intArray, x => (float)x);
			return floatArray;
		}

		/// <summary>
		/// Convert int array to double array
		/// </summary>
		/// <param name="intArray">original array</param>
		/// <returns>converted array</returns>
		public static double[] IntToDouble(int[] intArray) {
			double[] doubleArray = Array.ConvertAll(intArray, x => (double)x);
			return doubleArray;
		}
		
		/// <summary>
		/// Convert float array to double array
		/// </summary>
		/// <param name="floatArray">original array</param>
		/// <returns>converted array</returns>
		public static double[] FloatToDouble(float[] floatArray) {
			double[] doubleArray = Array.ConvertAll(floatArray, x => (double)x);
			return doubleArray;
		}

		/// <summary>
		/// Convert float array to double array
		/// </summary>
		/// <param name="jaggedFloatArray">original array</param>
		/// <returns>converted array</returns>
		public static double[][] FloatToDouble(float[][] jaggedFloatArray) {
			// http://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array
			double[][] jaggedDoubleArray = jaggedFloatArray.Select(i => i.Select(j => (double)j).ToArray()).ToArray();
			return jaggedDoubleArray;
		}
		
		/// <summary>
		/// Convert double array to float array
		/// </summary>
		/// <param name="doubleArray">original array</param>
		/// <returns>converted array</returns>
		public static float[] DoubleToFloat(double[] doubleArray) {
			float[] floatArray = Array.ConvertAll(doubleArray, x => (float)x);
			return floatArray;
		}
		
		/// <summary>
		/// Convert double array to float array
		/// </summary>
		/// <param name="jaggedDoubleArray">original array</param>
		/// <returns>converted array</returns>
		public static float[][] DoubleToFloat(double[][] jaggedDoubleArray) {
			// http://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array
			float[][] jaggedFloatArray = jaggedDoubleArray.Select(i => i.Select(j => (float)j).ToArray()).ToArray();
			return jaggedFloatArray;
		}
		#endregion
		
		#region NumberFormatting
		
		/// <summary>
		/// Return a nicer number
		/// 0,1 --> 0,1
		/// 0,2 --> 0,25
		/// 0,7 --> 1
		/// 1 --> 1
		/// 2 --> 2,5
		/// 9 --> 10
		/// 25 --> 50
		/// 58 --> 100
		/// 99 --> 100
		/// 158 --> 250
		/// 267 --> 500
		/// 832 --> 1000
		/// </summary>
		/// <param name="val">value to format</param>
		/// <returns>formatted value</returns>
		public static double GetNicerNumber(double val)
		{
			// get the first larger power of 10
			var nice = Math.Pow(10, Math.Ceiling(Math.Log10(val)));

			// scale the power to a "nice enough" value
			if (val < 0.25 * nice)
				nice = 0.25 * nice;
			else if (val < 0.5 * nice)
				nice = 0.5 * nice;

			return nice;
		}
		
		/// <summary>
		/// Format numbers rounded to thousands with K (and M)
		/// 1 => 1
		/// 23 => 23
		/// 136 => 136
		/// 6968 => 6,968
		/// 23067 => 23.1K
		/// 133031 => 133K
		/// </summary>
		/// <param name="num">number to format</param>
		/// <returns></returns>
		public static string FormatNumber(int num) {
			if (num >= 100000000)
				return (num / 1000000D).ToString("#,0M");

			if (num >= 10000000)
				return (num / 1000000D).ToString("0.#") + "M";

			if (num >= 100000)
				return (num / 1000D).ToString("#,0K");

			if (num >= 10000)
				return (num / 1000D).ToString("0.#") + "K";

			return num.ToString("#,0");
		}
		#endregion
		
		#region FindClosest
		/// <summary>
		/// Find the closest number in a list of numbers
		/// Use like this:
		/// List<int> list = new List<int> { 2, 5, 7, 10 };
		/// int target = 6;
		/// int closest = FindClosest(list, target);
		/// </summary>
		/// <param name="numbers"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int FindClosest(IEnumerable<int> numbers, int target) {
			// http://stackoverflow.com/questions/5953552/how-to-get-the-closest-number-from-a-listint-with-linq
			int closest = numbers.Aggregate((x,y) => Math.Abs(x-target) < Math.Abs(y-target) ? x : y);
			return closest;
		}

		/// <summary>
		/// Find the closest number in a list of numbers
		/// Use like this:
		/// List<int> list = new List<int> { 2, 5, 7, 10 };
		/// int target = 6;
		/// int closest = FindClosest(list, target);
		/// </summary>
		/// <param name="numbers"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static uint FindClosest(IEnumerable<uint> numbers, uint target) {
			// http://stackoverflow.com/questions/5953552/how-to-get-the-closest-number-from-a-listint-with-linq
			uint closest = numbers.Aggregate((x,y) => Math.Abs(x-target) < Math.Abs(y-target) ? x : y);
			return closest;
		}
		
		/// <summary>
		/// Find the closest number in a list of numbers
		/// Use like this:
		/// List<float> list = new List<float> { 10f, 20f, 22f, 30f };
		/// float target = 21f;
		/// float closest = FindClosest(list, target);
		/// </summary>
		/// <param name="numbers"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static float FindClosest(IEnumerable<float> numbers, float target) {
			// http://stackoverflow.com/questions/3723321/linq-to-get-closest-value
			
			//gets single number which is closest
			var closest = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.First().n;
			
			return closest;
		}

		/// <summary>
		/// Find the x closest numbers in a list of numbers
		/// Use like this:
		/// List<float> list = new List<float> { 10f, 20f, 22f, 30f };
		/// float target = 21f;
		/// int take = 2;
		/// float closest = FindClosest(list, target, take);
		/// </summary>
		/// <param name="numbers"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static IEnumerable<float> FindClosest(IEnumerable<float> numbers, float target, int take) {
			// http://stackoverflow.com/questions/3723321/linq-to-get-closest-value
			
			//get x closest
			var closests = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.Select( p => p.n )
				.Take( take );

			return closests;
		}
		#endregion
		
		#region Calculus
		/// <summary>
		/// Perform a hypot calculation
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <remarks>
		/// sqrt(a^2 + b^2) without under/overflow
		/// </remarks>
		/// <returns></returns>
		public static double Hypot(double a, double b)
		{
			double r;
			if (Math.Abs(a) > Math.Abs(b)) {
				r = b/a;
				r = Math.Abs(a)*Math.Sqrt(1+r*r);
			} else if (b != 0) {
				r = a/b;
				r = Math.Abs(b)*Math.Sqrt(1+r*r);
			} else {
				r = 0.0;
			}
			return r;
		}
		
		/// <summary>
		/// Multiply signal with factor
		/// </summary>
		/// <param name="data">Signal to be processed</param>
		/// <param name="factor">factor to multiply with</param>
		public static void Multiply(ref double[] data, double factor)
		{
			// multiply by factor and return
			data = data.Select(i => i * factor).ToArray();
		}

		/// <summary>
		/// Multiply signal with factor
		/// </summary>
		/// <param name="data">Signal to be processed</param>
		/// <param name="factor">factor to multiply with</param>
		public static void Multiply(ref float[] data, float factor)
		{
			// multiply by factor and return
			data = data.Select(i => i * factor).ToArray();
		}

		/// <summary>
		/// Divide signal with factor
		/// </summary>
		/// <param name="data">Signal to be processed</param>
		/// <param name="factor">factor to divide with</param>
		public static void Divide(ref double[] data, double factor)
		{
			// divide by factor and return
			data = data.Select(i => i / factor).ToArray();
		}

		/// <summary>
		/// Divide signal with factor
		/// </summary>
		/// <param name="data">Signal to be processed</param>
		/// <param name="factor">factor to divide with</param>
		public static void Divide(ref float[] data, float factor)
		{
			// divide by factor and return
			data = data.Select(i => i / factor).ToArray();
		}
		
		/// <summary>
		/// Convert radian to degrees
		/// </summary>
		/// <param name="angle">angle in radian</param>
		/// <returns>degrees</returns>
		public static double RadianToDegree(double angle)
		{
			return angle * (180.0 / Math.PI);
		}
		
		/// <summary>
		/// Convert degrees to radian
		/// </summary>
		/// <param name="angle">angle in degrees</param>
		/// <returns>radian</returns>
		public static double DegreeToRadian(double angle)
		{
			return Math.PI * angle / 180.0;
		}
		#endregion
		
		#region MinMaxAbs
		
		/// <summary>
		/// Perform a Math.Abs on a full array
		/// </summary>
		/// <param name="floatArray">array of floats</param>
		/// <returns>array after Math.Abs</returns>
		public static float[] Abs(float[] floatArray) {

			if (floatArray == null) return null;
			
			// use LINQ
			float[] absArray = floatArray.Select(i => Math.Abs(i)).ToArray();
			
			return absArray;
		}

		/// <summary>
		/// Perform a Math.Abs on a full array
		/// </summary>
		/// <param name="doubleArray">array of doubles</param>
		/// <returns>array after Math.Abs</returns>
		public static double[] Abs(double[] doubleArray) {

			if (doubleArray == null) return null;
			
			// use LINQ
			double[] absArray = doubleArray.Select(i => Math.Abs(i)).ToArray();
			
			return absArray;
		}
		
		/// <summary>
		/// Find maximum number when all numbers are made positive.
		/// </summary>
		/// <param name="data">data</param>
		/// <returns>max</returns>
		public static double Max(double[][] data) {
			double max = data.Max((b) => b.Max((v) => Math.Abs(v)));
			return max;
		}

		/// <summary>
		/// Find minimum number when all numbers are made positive.
		/// </summary>
		/// <param name="data">data</param>
		/// <returns>min</returns>
		public static double Min(double[][] data) {
			double min = data.Min((b) => b.Min((v) => Math.Abs(v)));
			return min;
		}
		#endregion
		
		#region Extension method (IndexesWhere and 2D Row and Column accessors and 2D Array Deep Copy)
		/// <summary>
		/// Extension method to return indexes for linq queries
		/// </summary>
		/// <param name="source">array</param>
		/// <param name="predicate">query [e.g. t => t.StartsWith("t")]</param>
		/// <example>
		/// string[] s = {"zero", "one", "two", "three", "four", "five"};
		/// var x = s.IndexesWhere(t => t.StartsWith("t"));
		/// </example>
		/// <returns>Enumeration of indices</returns>
		public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			int index=0;
			foreach (T element in source)
			{
				if (predicate(element))
				{
					yield return index;
				}
				index++;
			}
		}

		/// <summary>
		/// Extension method to return rows for linq queries
		/// </summary>
		/// <param name="array">array</param>
		/// <param name="row">row</param>
		/// <returns>Enumeration</returns>
		public static IEnumerable<T> Row<T>(this T[,] array, int row)
		{
			for (int i = 0; i < array.GetLength(1); i++)
			{
				yield return array[row, i];
			}
		}

		/// <summary>
		/// Extension method to return columns for linq queries
		/// </summary>
		/// <param name="array">array</param>
		/// <param name="column">column</param>
		/// <returns>Enumeration</returns>
		public static IEnumerable<T> Column<T>(this T[,] array, int column)
		{
			for (int i = 0; i < array.GetLength(0); i++)
			{
				yield return array[i, column];
			}
		}

		/// <summary>
		/// Extension method to return rows for linq queries
		/// </summary>
		/// <param name="array">array</param>
		/// <param name="row">row</param>
		/// <returns>Enumeration</returns>
		public static IEnumerable<T> Row<T>(this T[][] array, int row)
		{
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[row][i];
			}
		}

		/// <summary>
		/// Extension method to return columns for linq queries
		/// </summary>
		/// <param name="array">array</param>
		/// <param name="column">column</param>
		/// <returns>Enumeration</returns>
		public static IEnumerable<T> Column<T>(this T[][] array, int column)
		{
			var col = array.Select(row => row[column]);
			return col;
		}
		
		/// <summary>
		/// Extension method to copy an array to another array
		/// </summary>
		/// <param name="array">array</param>
		/// <returns>a copy of the array</returns>
		public static T[][] DeepCopy<T>(this T[][] array)
		{
			return array.Select(a => a.ToArray()).ToArray();
		}
		#endregion
		
		/// <summary>
		/// Return Median of a int array.
		/// NB! The array need to be sorted first
		/// </summary>
		/// <param name="pNumbers"></param>
		/// <returns></returns>
		public static double GetMedian(int[] pNumbers)  {

			int size = pNumbers.Length;

			int mid = size /2;

			double median = (size % 2 != 0) ? (double)pNumbers[mid] :
				((double)pNumbers[mid] + (double)pNumbers[mid-1]) / 2;

			return median;

		}
		
		/// <summary>
		/// Find all numbers that are within x of target
		/// Use like this:
		/// List<float> list = new List<float> { 10f, 20f, 22f, 30f };
		/// float target = 21f;
		/// float within = 1;
		/// var result = FindWithinTarget(list, target, within);
		/// </summary>
		/// <param name="numbers"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static IEnumerable<float> FindWithinTarget(IEnumerable<float> numbers, float target, float within) {
			// http://stackoverflow.com/questions/3723321/linq-to-get-closest-value
			
			//gets any that are within x of target
			var withins = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.Where( p => p.distance <= within )
				.Select( p => p.n );
			
			return withins;
		}
		
		/// <summary>
		/// The goal of pre-emphasis is to compensate the high-frequency part
		/// that was suppressed during the sound production mechanism of humans.
		/// Moreover, it can also amplify the importance of high-frequency formants.
		/// It's not neccesary for only music, but important for speech
		/// </summary>
		/// <param name="samples">Audio data to preemphase</param>
		/// <param name="preEmphasisAlpha">Pre-Emphasis Alpha (Set to 0 if no pre-emphasis should be performed)</param>
		/// <returns>processed audio</returns>
		public static float[] PreEmphase(float[] samples, float preEmphasisAlpha){
			var emphasedSamples = new float[samples.Length];
			for (int i = 1; i < samples.Length; i++){
				emphasedSamples[i] = (float) samples[i] - preEmphasisAlpha * samples[i - 1];
			}
			return emphasedSamples;
		}
		
		/// <summary>
		/// The goal of pre-emphasis is to compensate the high-frequency part
		/// that was suppressed during the sound production mechanism of humans.
		/// Moreover, it can also amplify the importance of high-frequency formants.
		/// It's not neccesary for only music, but important for speech.
		/// This method uses a fixed pre emphasis alpha factor of 0.95
		/// </summary>
		/// <param name="samples">Audio data to preemphase</param>
		/// <returns>processed audio</returns>
		public static float[] PreEmphase(float[] samples) {
			const float PREEMPHASISALPHA = 0.95f;
			return PreEmphase(samples, PREEMPHASISALPHA);
		}
		
		/// <summary>
		/// Flatten Jagged Array (i.e. convert from double[][] to double[])
		/// </summary>
		/// <param name="data">jagged array</param>
		/// <returns>flattened array</returns>
		public static double[] Flatten(double[][] data) {
			return data.SelectMany((b) => (b)).ToArray();
		}
		
		/// <summary>
		/// Linear Interpolation
		/// </summary>
		/// <param name="y0">first number</param>
		/// <param name="y1">second number</param>
		/// <param name="fraction">fraction in the range [0,1}</param>
		/// <remarks>
		/// The standard way to blend between two colors (C1 and C2)
		/// is just to linearly interpolate each red, green, and blue component
		/// according to the following formulas:
		/// R' = R1 + f * (R2 - R1)
		/// G' = G1 + f * (G2 - G1)
		/// B' = B1 + f * (B2 - B1)
		/// Where f is a fraction the range [0,1].
		/// When f is 0, our result is 100% C1,
		/// and when f is 1, our result is 100% C2.
		/// </remarks>
		/// <returns>Interpolated double</returns>
		public static double Interpolate(double y0, double y1, double fraction)
		{
			return y0 + (y1 - y0) * fraction;
		}
		
		#region Limit methods
		/// <summary>
		/// Limit integer between min and max
		/// </summary>
		/// <param name="value">int value</param>
		/// <param name="min">minimum value</param>
		/// <param name="max">maximum value</param>
		public static int LimitInt(int value, int min, int max)
		{
			if (value < min)
				value = min;
			if (value > max)
				value = max;
			
			return value;
		}

		/// <summary>
		/// Limit float between min and max
		/// </summary>
		/// <param name="value">float value</param>
		/// <param name="min">minimum value</param>
		/// <param name="max">maximum value</param>
		public static float LimitFloat(float value, float min, float max)
		{
			if (value < min)
				value = min;
			if (value > max)
				value = max;
			
			return value;
		}
		#endregion
		
		/// <summary>
		/// Pad array with zeros
		/// </summary>
		/// <param name="data">data array to be padded</param>
		/// <param name="length">new length</param>
		/// <returns>padded array</returns>
		public static double[] PadZeros(double[] data, int length) {
			var returnArray = new double[length];
			
			if (length < data.Length) {
				// shorten?
				Array.Copy(data, 0, returnArray, 0, length);
			} else {
				Array.Copy(data, 0, returnArray, 0, data.Length);
			}
			return returnArray;
		}
		
		/// <summary>
		/// Calculate averages on data using start and end index
		/// </summary>
		/// <param name="data">float array with data</param>
		/// <param name="startIndex">start index</param>
		/// <param name="endIndex">end index</param>
		/// <param name="posAvg">output positive average</param>
		/// <param name="negAvg">output negative average</param>
		public static void Averages(float[] data, int startIndex, int endIndex, out float posAvg, out float negAvg)
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
		
		/// <summary>
		/// Return smalles non negative number
		/// (almost like Math.Min just doesn't return negative numbers)
		/// </summary>
		/// <param name="a">The first of two integers to compare.</param>
		/// <param name="b">The first of two integers to compare.</param>
		/// <returns>Parameter val1 or val2, whichever is smaller and non-negative.</returns>
		public static int GetSmallestNonNegative(int a, int b)
		{
			if (a >= 0 && b >= 0) {
				return Math.Min(a,b);
			} else if (a >= 0 && b < 0) {
				return a;
			} else if (a < 0 && b >= 0) {
				return b;
			} else {
				return 0;
			}
		}
		
		/// <summary>
		/// Return smalles non negative number
		/// (almost like Math.Min just doesn't return negative numbers)
		/// </summary>
		/// <param name="a">The first of two integers to compare.</param>
		/// <param name="b">The first of two integers to compare.</param>
		/// <returns>Parameter val1 or val2, whichever is smaller and non-negative.</returns>
		public static int MinNonNegative(int a, int b) {
			return GetSmallestNonNegative(a, b);
		}
		
		#region Distribute items in columns evenly
		/// <summary>
		/// Return an even distribution of items spread out over a number of columns.
		/// When having less items than available columns this method prefers edges,
		/// i.e. 3 items spread out on 7 columns will return array [1,0,0,1,0,0,1]
		/// </summary>
		/// <param name="items">number of items to spread</param>
		/// <param name="columns">number of columns to spread into</param>
		/// <returns>an array of length 'columns' of the evenly distributed items</returns>
		public static int[] GetEvenDistributionPreferEdges(int items, int columns)
		{
			var result = new int[columns];

			if (items == 0) {
				// do nothing
			} else if (items == 1) {
				result[0] = 1;
			} else if (items > 1 && items <= columns) {
				// http://stackoverflow.com/questions/2451298/evenly-select-n-elems-from-array
				
				// set step size
				double step = (double) (columns-1)/(items-1);

				for (int i=0; i < items; i++) {
					// push each element of a position which is a
					// multiple of step to R
					int index = (int) Math.Round(step*i);
					result[index] = 1;
				}
			} else {
				// use another algorithm
				result = GetEvenDistribution(items, columns);
			}
			
			return result;
		}
		
		// http://stackoverflow.com/questions/23161009/how-to-evenly-distribute-array-members
		
		/// <summary>
		/// Return an even distribution of items spread out over a number of columns.
		/// </summary>
		/// <param name="items">number of items to spread</param>
		/// <param name="columns">number of columns to spread into</param>
		/// <returns>an array of length 'columns' of the evenly distributed items</returns>
		public static int[] GetEvenDistribution(int items, int columns) {
			var result = new int[columns];

			double itemsPerColumns = (double) (items) / (columns);
			
			int index = 0;
			int numItemsInColumn = 0;
			double currentItems = 0;
			int oldItems = 0;
			while (index < columns) {
				currentItems += itemsPerColumns;
				numItemsInColumn = (int) Math.Round(currentItems - oldItems);
				
				oldItems += numItemsInColumn;
				result[index++] = numItemsInColumn;
			}
			
			return result;
		}
		
		/// <summary>
		/// This seem to do the same as GetEvenDistribution()
		/// </summary>
		/// <param name="items">number of items to spread</param>
		/// <param name="columns">number of columns to spread into</param>
		/// <returns>an array of length 'columns' of the evenly distributed items</returns>
		public static int[] GetEvenDistribution2(int items, int columns) {
			var result = new int[columns];
			for (int t = 0; t < columns; t++) {
				int numItemsInColumn = ((t+1)*items+columns/2)/columns - (t*items+columns/2)/columns;
				result[t] = numItemsInColumn;
			}
			return result;
		}
		
		#endregion

		#region Find indices matching a search criteria
		
		/// <summary>
		/// Return all indices that match a given value
		/// </summary>
		/// <param name="array">array to search</param>
		/// <param name="searchFor">value to search for</param>
		/// <returns>an array of indices found</returns>
		public static int[] IndexOf(int[] array, int searchFor) {
			var indices = array.Select((value, index) => new { value, index })
				.Where(x => x.value == searchFor)
				.Select(x => x.index)
				.ToArray();
			
			return indices;
		}

		/// <summary>
		/// Return all indices that match a given value
		/// </summary>
		/// <param name="array">array to search</param>
		/// <param name="searchFor">value to search for</param>
		/// <returns>an array of indices found</returns>
		public static int[] IndexOf(float[] array, float searchFor) {
			var indices = array.Select((value, index) => new { value, index })
				.Where(x => x.value == searchFor)
				.Select(x => x.index)
				.ToArray();
			
			return indices;
		}

		/// <summary>
		/// Return all indices that match a given value
		/// </summary>
		/// <param name="array">array to search</param>
		/// <param name="searchFor">value to search for</param>
		/// <returns>an array of indices found</returns>
		public static int[] IndexOf(double[] array, double searchFor) {
			var indices = array.Select((value, index) => new { value, index })
				.Where(x => x.value == searchFor)
				.Select(x => x.index)
				.ToArray();
			
			return indices;
		}

		/// <summary>
		/// Return all indices that match a given value
		/// </summary>
		/// <param name="array">array to search</param>
		/// <param name="searchFor">value to search for</param>
		/// <returns>an array of indices found</returns>
		public static int[] IndexOf(string[] array, string searchFor) {
			var indices = array.Select((value, index) => new { value, index })
				.Where(x => x.value == searchFor)
				.Select(x => x.index)
				.ToArray();
			
			return indices;
		}
		#endregion
		
	}
}
