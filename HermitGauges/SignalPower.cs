///
/// <summary> * dsp: various digital signal processing algorithms
/// * <br>Copyright 2009 Ian Cameron Smith
/// *
/// * <p>This program is free software; you can redistribute it and/or modify
/// * it under the terms of the GNU General Public License version 2
/// * as published by the Free Software Foundation (see COPYING).
/// * 
/// * <p>This program is distributed in the hope that it will be useful,
/// * but WITHOUT ANY WARRANTY; without even the implied warranty of
/// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
/// * GNU General Public License for more details. </summary>
/// 

using System;

namespace Wave2ZebraSynth.HermitGauges
{

	///
	/// <summary> * A power metering algorithm. </summary>
	/// 
	public sealed class SignalPower
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		/// <summary> * Only static methods are provided in this class. </summary>
		/// 
		private SignalPower()
		{
		}


		// ******************************************************************** //
		// Algorithm.
		// ******************************************************************** //

		///
		/// <summary> * Calculate the bias and range of the given input signal.
		/// *  </summary>
		/// * <param name="sdata">  Buffer containing the input samples to process. </param>
		/// * <param name="off">  Offset in sdata of the data of interest. </param>
		/// * <param name="samples"> Number of data samples to process. </param>
		/// * <param name="out">  A float array in which the results will be placed
		/// *                      Must have space for two entries, which will be
		/// *                      set to:
		/// *                      <ul>
		/// *                      <li>The bias, i.e. the offset of the average
		/// *                      signal value from zero.
		/// *                      <li>The range, i.e. the absolute value of the largest
		/// *                      departure from the bias level.
		/// *                      </ul> </param>
		/// * <exception cref="NullPointerException"> Null output array reference. </exception>
		/// * <exception cref="ArrayIndexOutOfBoundsException">  Output array too small. </exception>
		/// 
		public static void BiasAndRange(short[] sdata, int off, int samples, float[] @out)
		{
			// Find the max and min signal values, and calculate the bias.
			short min = 32767;
			short max = -32768;
			int total = 0;
			for (int i = off; i < off + samples; ++i)
			{
				short val = sdata[i];
				total += val;
				if (val < min)
				{
					min = val;
				}
				if (val > max)
				{
					max = val;
				}
			}
			float bias = (float) total / (float) samples;
			float bmin = min + bias;
			float bmax = max - bias;
			float range = Math.Abs(bmax - bmin) / 2f;

			@out[0] = bias;
			@out[1] = range;
		}


		///
		/// <summary> * Calculate the power of the given input signal.
		/// *  </summary>
		/// * <param name="sdata">  Buffer containing the input samples to process. </param>
		/// * <param name="off">  Offset in sdata of the data of interest. </param>
		/// * <param name="samples"> Number of data samples to process. </param>
		/// * <returns>  The calculated power in dB relative to the maximum
		/// *                      input level; hence 0dB represents maximum power,
		/// *                      and minimum power is about -95dB. Particular
		/// *                      cases of interest:
		/// *                      <ul>
		/// *                      <li>A non-clipping full-range sine wave input is
		/// *                          about -2.41dB.
		/// *                      <li>Saturated input (heavily clipped) approaches
		/// *                          0dB.
		/// *                      <li>A low-frequency fully saturated input can
		/// *                          get above 0dB, but this would be pretty
		/// *                          artificial.
		/// *                      <li>A really tiny signal, which only occasionally
		/// *                          deviates from zero, can get below -100dB.
		/// *                      <li>A completely zero input will produce an
		/// *                          output of -Infinity.
		/// *                      </ul>
		/// *                      <b>You must be prepared to handle this infinite
		/// *                      result and results greater than zero,</b> although
		/// *                      clipping them off would be quite acceptable in
		/// *                      most cases. </returns>
		/// 
		public static double CalculatePowerDb(short[] sdata, int off, int samples)
		{
			// Calculate the sum of the values, and the sum of the squared values.
			// We need longs to avoid running out of bits.
			double sum = 0;
			double sqsum = 0;
			for (int i = 0; i < samples; i++)
			{
				long v = sdata[off + i];
				sum += v;
				sqsum += v * v;
			}

			// sqsum is the sum of all (signal+bias)², so
			//     sqsum = sum(signal²) + samples * bias²
			// hence
			//     sum(signal²) = sqsum - samples * bias²
			// Bias is simply the average value, i.e.
			//     bias = sum / samples
			// Since power = sum(signal²) / samples, we have
			//     power = (sqsum - samples * sum² / samples²) / samples
			// so
			//     power = (sqsum - sum² / samples) / samples
			double power = (sqsum - sum * sum / samples) / samples;

			// Scale to the range 0 - 1.
			power /= MAX_16_BIT * MAX_16_BIT;

			// Convert to dB, with 0 being max power. Add a fudge factor to make
			// a "real" fully saturated input come to 0 dB.
			return Math.Log10(power) * 10f + FUDGE;
		}


		// ******************************************************************** //
		// Constants.
		// ******************************************************************** //

		// Maximum signal amplitude for 16-bit data.
		private const float MAX_16_BIT = 32768;

		// This fudge factor is added to the output to make a realistically
		// fully-saturated signal come to 0dB. Without it, the signal would
		// have to be solid samples of -32768 to read zero, which is not
		// realistic. This really is a fudge, because the best value depends
		// on the input frequency and sampling rate. We optimise here for
		// a 1kHz signal at 16,000 samples/sec.
		private const float FUDGE = 0.6f;

	}

}