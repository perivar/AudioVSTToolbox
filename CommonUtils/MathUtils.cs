using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils
{
	/// <summary>
	/// Description of MathUtils.
	/// </summary>
	public static class MathUtils
	{
		// For use in calculating log base 10. A log times this is a log base 10.
		private static double LOG10SCALE = 1 / Math.Log(10);
		
		// handy static methods
		public static double log10(double val)
		{
			return Math.Log(val) * LOG10SCALE;
		}
		public static double exp10(double val)
		{
			return Math.Exp(val / LOG10SCALE);
		}
		public static float flog10(double val)
		{
			return (float)log10(val);
		}
		public static float fexp10(double val)
		{
			return (float)exp10(val);
		}
		
		public static float[] ReSampleToArbitrary(float[] input, int size)
		{
			float[] returnArray = new float[size];
			int length = input.Length;
			float phaseInc = (float) length / size;
			float phase = 0.0F;
			float phaseMant = 0.0F;
			
			for (int i = 0; i < size; i++)
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
		
		#region ConvertRangeAndMaintainRation
		public static float[] ConvertRangeAndMainainRatio(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			float[] newValueArray = new float[oldValueArray.Length];
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float newValue = (((oldValueArray[x] - oldMin) * newRange) / oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}
		
		public static float[] ConvertRangeAndMainainRatioLog(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			float[] newValueArray = new float[oldValueArray.Length];
			
			// TODO: Addition of Epsilon prevents log from returning minus infinity if value is zero
			float newRange = (newMax - newMin);
			float log_oldMin = flog10(Math.Abs(oldMin) + float.Epsilon);
			float log_oldMax = flog10(oldMax + float.Epsilon);
			float oldRange = (oldMax - oldMin);
			float log_oldRange = (log_oldMax - log_oldMin);
			float data_per_log_unit = newRange / log_oldRange;
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float log_oldValue = flog10(oldValueArray[x] + float.Epsilon);
				float newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}
		
		public static double[] ConvertRangeAndMainainRatio(double[] oldValueArray, double oldMin, double oldMax, double newMin, double newMax) {
			double[] newValueArray = new double[oldValueArray.Length];
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
		
		#region ConvertAndMaintainRatio
		public static double ConvertAndMainainRatio(double oldValue, double oldMin, double oldMax, double newMin, double newMax) {
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			double newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}

		public static float ConvertAndMainainRatio(float oldValue, float oldMin, float oldMax, float newMin, float newMax) {
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}

		public static double ConvertAndMainainRatioLog(double oldValue, double oldMin, double oldMax, double newMin, double newMax) {
			// Addition of Epsilon prevents log from returning minus infinity if value is zero
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			double log_oldMin = flog10(Math.Abs(oldMin) + double.Epsilon);
			double log_oldMax = flog10(oldMax + double.Epsilon);
			double log_oldRange = (log_oldMax - log_oldMin);
			//double data_per_log_unit = newRange / log_oldRange;
			double log_oldValue = flog10(oldValue + double.Epsilon);
			double newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
			return newValue;
		}

		public static float ConvertAndMainainRatioLog(float oldValue, float oldMin, float oldMax, float newMin, float newMax) {
			// Addition of Epsilon prevents log from returning minus infinity if value is zero
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float log_oldMin = flog10(Math.Abs(oldMin) + float.Epsilon);
			float log_oldMax = flog10(oldMax + float.Epsilon);
			float log_oldRange = (log_oldMax - log_oldMin);
			//float data_per_log_unit = newRange / log_oldRange;
			float log_oldValue = flog10(oldValue + float.Epsilon);
			float newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
			return newValue;
		}
		#endregion
		
		public static double RoundToNearest(double number, double nearest) {
			double rounded = Math.Round(number * (1 / nearest), MidpointRounding.AwayFromZero) / (1 / nearest);
			return rounded;
		}
		
		public static int RoundToNearestInteger(int number, int nearest) {
			int rounded = (int) Math.Round( (double) number / nearest, MidpointRounding.AwayFromZero) * nearest;
			return rounded;
		}

		public static double RoundDown(double number, int decimalPlaces)
		{
			return Math.Floor(number * Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces);
		}
		
		#region ComputeMinAndMax
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
		
		public static float[] GetSineWave(float frequency, float amplitude, float sampleRate, int offset, int sampleCount, int sample = 0) {
			float[] buffer = new float[sampleCount+offset];
			for (int n = 0; n < sampleCount; n++)
			{
				buffer[n+offset] = (float)(amplitude * Math.Sin((2 * Math.PI * sample * frequency) / sampleRate));
				sample++;
				if (sample >= sampleRate) sample = 0;
			}
			return buffer;
		}
		
		// look at this http://jvalentino2.tripod.com/dft/index.html
		public static float ConvertAmplitudeToDB(float amplitude, float MinDb, float MaxDb) {
			// db = 20 * log10( fft[index] );
			// Addition of smallestNumber prevents log from returning minus infinity if mag is zero
			float smallestNumber = float.Epsilon;
			float db = 20 * (float) Math.Log10( (float) (amplitude + smallestNumber) );
			
			if (db < MinDb) db = MinDb;
			if (db > MaxDb) db = MaxDb;
			
			return db;
		}
		
		public static float ConvertFloatToDB(float amplitude) {
			// 20 log10(mag) => 20/ln(10) ln(mag)
			// javascript: var result = Math.log(x) * (20.0 / Math.LN10);
			// http://www.plugindeveloper.com/05/decibel-calculator-online
			// Addition of smallestNumber prevents log from returning minus infinity if mag is zero
			float smallestNumber = float.Epsilon;
			double result = Math.Log(amplitude + smallestNumber) * (20.0 / Math.Log(10));
			return (float) result;
		}

		public static float ConvertDBToFloat(float dB) {
			// javascript: var result = Math.exp((x) * (Math.LN10 / 20.0));
			// http://www.plugindeveloper.com/05/decibel-calculator-online
			double result = Math.Exp(( dB) * (Math.Log(10) / 20.0));
			return (float) result;
		}
		
		/// <summary>
		/// Return the frequency in Hz for each index in FFT
		/// </summary>
		/// <param name="i">index</param>
		/// <param name="spectrumLength">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
		/// <param name="sampleRate">sample rate (e.g. 44100 Hz)</param>
		/// <param name="nFFT">size of FFT (e.g. 1024)</param>
		/// <returns>Frequency in Hz</returns>
		public static float ConvertIndexToHz(int i, int spectrumLength, double sampleRate, double nFFT) {
			double nyquistFreq = sampleRate / 2;
			double firstFrequency = nyquistFreq / spectrumLength;
			double frequency = firstFrequency *  i ;
			return (float) frequency;
		}

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
		/// <param name = "spectrumLength">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
		/// <returns>Frequency in Hz</returns>
		public static double IndexToFreq(int i, double sampleRate, int spectrumLength) {
			return (double) i * (sampleRate / spectrumLength);
		}
		
		/// <summary>
		/// Gets the index in the spectrum vector from according to the starting frequency specified as the parameter
		/// </summary>
		/// <param name = "freq">Frequency to be found in the spectrum vector [E.g. 300Hz]</param>
		/// <param name = "sampleRate">Frequency rate at which the signal was processed [E.g. 5512Hz]</param>
		/// <param name = "spectrumLength">Length of the spectrum [2048 elements generated by WDFT from which only 1024 are with the actual data]</param>
		/// <returns>Index of the frequency in the spectrum array</returns>
		/// <remarks>
		///   The Bandwidth of the spectrum runs from 0 until SampleRate / 2 [E.g. 5512 / 2]
		///   Important to remember:
		///   N points in time domain correspond to N/2 + 1 points in frequency domain
		///   E.g. 300 Hz applies to 112'th element in the array
		/// </remarks>
		public static int FreqToIndex(double freq, double sampleRate, int spectrumLength)
		{
			double fraction = freq / (sampleRate / 2); /*N sampled points in time correspond to [0, N/2] frequency range */
			int i = (int) Math.Round((spectrumLength / 2 + 1) * fraction); /*DFT N points defines [N/2 + 1] frequency points*/
			return i;
		}
		
		/// <summary>
		/// Return the frequency in Hz for each index in FFT
		/// </summary>
		/// <param name="i">index in the FFT spectrum</param>
		/// <param name="sampleRate">sample rate (e.g. 44100 Hz)</param>
		/// <param name="nFFT">size of FFT (e.g. 1024)</param>
		/// <returns>Frequency in Hz</returns>
		public static double Index2Freq(int i, double sampleRate, int nFFT) {
			return (double) i * (sampleRate / nFFT / 2.0);
		}
		
		/// <summary>
		/// Gets the index in the FFT spectrum according to the frequency specified as the parameter
		/// </summary>
		/// <param name="freq">Frequency in Hz</param>
		/// <param name="sampleRate">sample rate (e.g. 44100 Hz)</param>
		/// <param name="nFFT">size of FFT (e.g. 1024)</param>
		/// <returns></returns>
		public static int Freq2Index(double freq, double sampleRate, int nFFT) {
			return (int) (freq / (sampleRate / nFFT / 2.0));
		}
		
		public static double ConvertToTime(double sampleRate, int numberOfSamples) {
			double time = numberOfSamples / sampleRate;
			return time;
		}
		
		#region FloatAndDoubleConversions
		public static double[] FloatToDouble(float[] floatArray) {
			double[] doubleArray = Array.ConvertAll(floatArray, x => (double)x);
			return doubleArray;
		}

		public static double[][] FloatToDouble(float[][] jaggedFloatArray) {
			// http://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array
			double[][] jaggedDoubleArray = jaggedFloatArray.Select(i => i.Select(j => (double)j).ToArray()).ToArray();
			return jaggedDoubleArray;
		}
		
		public static float[] DoubleToFloat(double[] doubleArray) {
			float[] floatArray = Array.ConvertAll(doubleArray, x => (float)x);
			return floatArray;
		}
		
		public static float[][] DoubleToFloat(double[][] jaggedDoubleArray) {
			// http://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array
			float[][] jaggedFloatArray = jaggedDoubleArray.Select(i => i.Select(j => (float)j).ToArray()).ToArray();
			return jaggedFloatArray;
		}
		#endregion
		
		#region NumberFormatting
		/* Return a nicer number
		 * 0,1 --> 0,1
		 * 0,2 --> 0,25
		 * 0,7 --> 1
		 * 1 --> 1
		 * 2 --> 2,5
		 * 9 --> 10
		 * 25 --> 50
		 * 58 --> 100
		 * 99 --> 100
		 * 158 --> 250
		 * 267 --> 500
		 * 832 --> 1000
		 */
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
		/// <param name="num"></param>
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
			//var within = 1;
			var withins = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.Where( p => p.distance <= within )
				.Select( p => p.n );
			
			return withins;
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
		
		#region Normalize
		/// <summary>
		/// Calculate the root mean square (RMS) of an array
		/// </summary>
		/// <param name="x">array of ints</param>
		/// <returns>RMS</returns>
		public static double RootMeanSquare(int[] x)
		{
			double sum = 0;
			for (int i = 0; i < x.Length; i++)
			{
				sum += (x[i]*x[i]);
			}
			return Math.Sqrt(sum / x.Length);
		}
		
		/// <summary>
		/// Calculate the root mean square (RMS) of an array
		/// </summary>
		/// <param name="x">array of floats</param>
		/// <returns>RMS</returns>
		public static double RootMeanSquare(float[] x)
		{
			double sum = 0;
			for (int i = 0; i < x.Length; i++)
			{
				sum += (x[i]*x[i]);
			}
			return Math.Sqrt(sum / x.Length);
		}
		
		// normalize power (volume) of a wave file.
		// minimum and maximum rms to normalize from.
		private const float MINRMS = 0.1f;
		private const float MAXRMS = 3;

		/// <summary>
		///   Normalizing the input power (volume)
		/// </summary>
		/// <param name = "samples">Samples of a song to be normalized</param>
		public static void NormalizeInPlace(float[] samples)
		{
			int nsamples = samples.Length;
			float rms = (float) RootMeanSquare(samples);

			// we don't want to normalize by the real RMS, because excessive clipping will occur
			rms = rms * 10;
			
			if (rms < MINRMS)
				rms = MINRMS;
			if (rms > MAXRMS)
				rms = MAXRMS;

			for (int i = 0; i < nsamples; i++) {
				samples[i] /= rms;
				samples[i] = Math.Min(samples[i], 1);
				samples[i] = Math.Max(samples[i], -1);
			}
		}
		
		/** sqrt(a^2 + b^2) without under/overflow. **/
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
		
		#endregion
		
		#region MinMaxAbs
		public static float[] Abs(float[] floatArray) {

			if (floatArray == null) return null;
			
			// use LINQ
			float[] absArray = floatArray.Select(i => Math.Abs(i)).ToArray();
			
			// use old method
			/*
			float[] absArray = new float[floatArray.Length];
			for (int i = 0; i < floatArray.Length; i++) {
				float absValue = Math.Abs(floatArray[i]);
				absArray[i] = absValue;
			}
			 */
			return absArray;
		}

		public static double[] Abs(double[] doubleArray) {

			if (doubleArray == null) return null;
			
			// use LINQ
			double[] absArray = doubleArray.Select(i => Math.Abs(i)).ToArray();
			
			// use old method
			/*
			float[] absArray = new float[doubleArray.Length];
			for (int i = 0; i < doubleArray.Length; i++) {
				float absValue = Math.Abs(doubleArray[i]);
				absArray[i] = absValue;
			}
			 */
			return absArray;
		}
		
		public static double Max(double[][] data) {
			// Find maximum number when all numbers are made positive.
			double max = data.Max((b) => b.Max((v) => Math.Abs(v)));
			return max;
		}

		public static double Min(double[][] data) {
			// Find maximum number when all numbers are made positive.
			double min = data.Min((b) => b.Min((v) => Math.Abs(v)));
			return min;
		}
		#endregion

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
		
	}
}
