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
			float smallestNumber = float.Epsilon;
			float db = 20 * (float) Math.Log10( (float) (amplitude + smallestNumber) );
			
			if (db < MinDb) db = MinDb;
			if (db > MaxDb) db = MaxDb;
			
			return db;
		}
		
		public static float ConvertIndexToHz(int i, int numberOfSamples, double sampleRate, double fftWindowsSize) {
			double nyquistFreq = sampleRate / 2;
			double firstFrequency = nyquistFreq / numberOfSamples;
			double frequency = firstFrequency *  i ;
			return (float) frequency;
		}		
	}
}
