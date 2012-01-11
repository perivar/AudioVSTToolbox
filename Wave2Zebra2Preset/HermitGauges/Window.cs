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

namespace Wave2Zebra2Preset.HermitGauges
{

	///
	/// <summary> * A windowing function for a discrete signal. This is used to
	/// * pre-process a signal prior to FFT, in order to improve the frequency
	/// * response, essentially by eliminating the discontinuities at the ends
	/// * of a block of samples. </summary>
	/// 
	public sealed class Window
	{

		// ******************************************************************** //
		// Public Constants.
		// ******************************************************************** //

		///
		/// <summary> * Definitions of the available window functions. </summary>
		///
		public enum Function
		{
			/// <summary> A simple rectangular window function. This is equivalent to </summary>
			/// * doing no windowing.
			RECTANGULAR,

			/// <summary> The Blackman-Harris window function. </summary>
			BLACKMAN_HARRIS,

			/// <summary> The Gauss window function. </summary>
			GAUSS,

			/// <summary> The Weedon-Gauss window function. </summary>
			WEEDON_GAUSS,
		}


		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		/// <summary> * Create a window function for a given sample size. This preallocates
		/// * resources appropriate to that block size.
		/// *  </summary>
		/// * <param name="size"> The number of samples in a block that we will
		/// *                      be asked to transform. </param>
		///
		public Window(int size) : this(size, DEFAULT_FUNC)
		{
		}

		///
		/// <summary> * Create a window function for a given sample size. This preallocates
		/// * resources appropriate to that block size.
		/// *  </summary>
		/// * <param name="size"> The number of samples in a block that we will
		/// *                      be asked to transform. </param>
		/// * <param name="function"> The window function to use. Function.RECTANGULAR
		/// *                      effectively means no transformation. </param>
		///
		public Window(int size, Function function)
		{
			blockSize = size;

			// Create the window function as an array, so we do the
			// calculations once only. For RECTANGULAR, leave the kernel as
			// null, signalling no transformation.
			kernel = function == Function.RECTANGULAR ? null : new double[size];

			switch (function)
			{
				case Function.RECTANGULAR:
					// Nothing to do.
					break;
				case Function.BLACKMAN_HARRIS:
					makeBlackmanHarris(kernel, size);
					break;
				case Function.GAUSS:
					makeGauss(kernel, size);
					break;
				case Function.WEEDON_GAUSS:
					makeWeedonGauss(kernel, size);
					break;
			}
		}

		// ******************************************************************** //
		// Window Functions.
		// ******************************************************************** //

		private void makeBlackmanHarris(double[] buf, int len)
		{
			double n = (double)(len - 1);
			for (int i = 0; i < len; ++i)
			{
				double f = Math.PI * (double) i / n;
				buf[i] = BH_A0 - BH_A1 * Math.Cos(2.0 * f) + BH_A2 * Math.Cos(4.0 * f) - BH_A3 * Math.Cos(6.0 * f);
			}
		}

		private void makeGauss(double[] buf, int len)
		{
			double k = (double)(len - 1) / 2;

			for (int i = 0; i < len; ++i)
			{
				double d = (i - k) / (0.4 * k);
				buf[i] = Math.Exp(-0.5 * d * d);
			}
		}


		private void makeWeedonGauss(double[] buf, int len)
		{
			double k = (-250.0 * 0.4605) / (double)(len * len);
			double d = (double) len / 2.0;

			for (int i = 0; i < len; ++i)
			{
				double n = (double) i - d;
				buf[i] = Math.Exp(n * n * k);
			}
		}


		// ******************************************************************** //
		// Data Transformation.
		// ******************************************************************** //

		///
		/// <summary> * Apply the window function to a given data block. The data in
		/// * the provided buffer will be multiplied by the window function.
		/// *  </summary>
		/// * <param name="input">  The input data buffer. This data will be
		/// *                      transformed in-place by the window function. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid data size. </exception>
		///
		public void transform(double[] input)
		{
			transform(input, 0, input.Length);
		}

		///
		/// <summary> * Apply the window function to a given data block. The data in
		/// * the provided buffer will be multiplied by the window function.
		/// *  </summary>
		/// * <param name="input">  The input data buffer. This data will be
		/// *                      transformed in-place by the window function. </param>
		/// * <param name="off">  Offset in the buffer at which the data to
		/// *                      be transformed starts. </param>
		/// * <param name="count">  Number of samples in the data to be
		/// *                      transformed. Must be the same as the size
		/// *                      parameter that was given to the constructor. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid data size. </exception>
		///
		public void transform(double[] input, int off, int count)
		{
			if (count != blockSize)
			{
				throw new System.ArgumentException("bad input count in Window:" + " constructed for " + blockSize + "; given " + input.Length);
			}
			if (kernel != null)
			{
				for (int i = 0; i < blockSize; i++)
				{
					input[off + i] *= kernel[i];
				}
			}
		}


		// ******************************************************************** //
		// Private Constants.
		// ******************************************************************** //

		// Default window function.
		private const Function DEFAULT_FUNC = Function.BLACKMAN_HARRIS;

		// Blackman-Harris coefficients. These sum to 1.0.
		private const double BH_A0 = 0.35875;
		private const double BH_A1 = 0.48829;
		private const double BH_A2 = 0.14128;
		private const double BH_A3 = 0.01168;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// The size of an input data block.
		private readonly int blockSize;

		// The window function, as a pre-computed array of multiplication factors.
		// If null, do no transformation -- this is a unity rectangular window.
		private readonly double[] kernel;
	}
}