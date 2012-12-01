using System;
using System.Collections.Generic;

namespace com.badlogic.audio.analysis
{
	// Calculates a threshold function based on the spectral flux.
	// @author mzechner
	public class ThresholdFunction
	{
		/// the history size
		private readonly int historySize;

		/// the average multiplier
		private readonly float multiplier;

		// Consturctor, sets the history size in number of spectra
		// to take into account to calculate the average spectral flux
		// at a specific position. Also sets the multiplier to
		// multiply the average with.
		// 
		// @param historySize The history size.
		// @param multiplier The average multiplier.
		public ThresholdFunction(int historySize, float multiplier)
		{
			this.historySize = historySize;
			this.multiplier = multiplier;
		}

		// Returns the threshold function for a given
		// spectral flux function.
		// @return The threshold function.
		public virtual List<float> calculate(List<float> spectralFlux)
		{
			List<float> thresholds = new List<float>(spectralFlux.Count);

			for(int i = 0; i < spectralFlux.Count; i++)
			{
				float sum = 0;
				int Start = Math.Max(0, i - historySize / 2);
				int end = Math.Min(spectralFlux.Count-1, i + historySize / 2);
				for(int j = Start; j <= end; j++)
					sum += spectralFlux[j];
				sum /= (end-Start);
				sum *= multiplier;
				thresholds.Add(sum);
			}

			return thresholds;
		}
	}
}