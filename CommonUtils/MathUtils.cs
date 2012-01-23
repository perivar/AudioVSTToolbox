using System;

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
		
		public static float ConvertAndMainainRatio(float oldValue, float oldMin, float oldMax, float newMin, float newMax) {
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
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

		public static double RoundToNearest(double number, double nearest) {
			double rounded = Math.Round(number * (1 / nearest), MidpointRounding.AwayFromZero) / (1 / nearest);
			return rounded;
		}
		
		public static int RoundToNearestInteger(int number, int nearest) {
			int rounded = (int) Math.Round( (double) number / nearest) * nearest;
			return rounded;
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
		
		public static float ConvertIndexToHz(int i, int numberOfSamples, double sampleRate, double fftWindowsSize) {
			double nyquistFreq = sampleRate / 2;
			double firstFrequency = nyquistFreq / numberOfSamples;
			double frequency = firstFrequency *  i ;
			return (float) frequency;
		}
		
		public static double[] FloatToDouble(float[] floatArray) {
			double[] doubleArray = Array.ConvertAll(floatArray, x => (double)x);
			return doubleArray;
		}

		public static float[] DoubleToFloat(double[] doubleArray) {
			float[] floatArray = Array.ConvertAll(doubleArray, x => (float)x);
			return floatArray;
		}
		
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
		
		public static float[] Abs(float[] floatArray) {
			if (floatArray == null) return null;
			
			float[] absArray = new float[floatArray.Length];
			for (int i = 0; i < floatArray.Length; i++) {
				float absValue = Math.Abs(floatArray[i]);
				absArray[i] = absValue;
			}
			return absArray;
		}
		
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
	}
}
