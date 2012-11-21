using System;

namespace com.badlogic.audio.analysis
{
	/// <summary>
	/// Some signal metric functions like energy, power etc.
	/// @author mzechner
	/// </summary>
	public class Signals
	{
		public static float Mean(float[] signal)
		{
			float mean = 0;
			for(int i = 0; i < signal.Length; i++)
				mean+=signal[i];
			mean /= signal.Length;
			return mean;
		}

		public static float Energy(float[] signal)
		{
			float totalEnergy = 0;
			for(int i = 0; i < signal.Length; i++)
				totalEnergy += (signal[i] * signal[i]);
			return totalEnergy;
		}

		public static float Power(float[] signal)
		{
			return Energy(signal) / signal.Length;
		}

		public static float Norm(float[] signal)
		{
			return (float)Math.Sqrt(Energy(signal));
		}

		public static float Minimum(float[] signal)
		{
			float min = float.PositiveInfinity;
			for(int i = 0; i < signal.Length; i++)
				min = Math.Min(min, signal[i]);
			return min;
		}

		public static float Maximum(float[] signal)
		{
			float max = float.NegativeInfinity;
			for(int i = 0; i < signal.Length; i++)
				max = Math.Max(max, signal[i]);
			return max;
		}

		public static void Scale(float[] signal, float scale)
		{
			for(int i = 0; i < signal.Length; i++)
				signal[i] *= scale;
		}
	}
}