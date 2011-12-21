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
using Lomont;

namespace Wave2ZebraSynth.HermitGauges
{
	
	///
	/// <summary> * Implementation of the Cooleyâ€"Tukey FFT algorithm by Tsan-Kuang Lee,
	/// * for real-valued data and results:
	/// * http://www.ling.upenn.edu/~tklee/Projects/dsp/
	/// * 
	/// * <p>His copyright statement: "Do whatever you want with the code.
	/// * Feedbacks and improvement welcome."
	/// * 
	/// * <p>Usage: create an FFTTransformer with a specified block size, to
	/// * pre-allocate the necessary resources. Then, for each block that
	/// * you want to transform:
	/// * <ul>
	/// * <li>Call <seealso cref="#SetInput(float[], int, int)"/> to
	/// *     supply the input data. The execution of this method is the only
	/// *     time your input buffer will be accessed; the data is converted
	/// *     to complex and copied to a different buffer.
	/// * <li>Call <seealso cref="#transform()"/> to actually do the FFT. This is the
	/// *     time-consuming part.
	/// * <li>Call <seealso cref="#getResults(float[])"/> to get the results into
	/// *     your output buffer.
	/// * </ul>
	/// * <p>The flow is broken up like this to allow you to make best use of
	/// * locks. For example, if the input buffer is also accessed by a thread
	/// * which reads from the audio, you only need to lock out that thread during
	/// * <seealso cref="#SetInput(float[], int, int)"/>, not the entire FFT process. </summary>
	/// 
	public sealed class FFTTransformer
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		/// <summary> * Create an FFT transformer for a given sample size. This preallocates
		/// * resources appropriate to that block size.
		/// *  </summary>
		/// * <param name="size"> The number of samples in a block that we will
		/// *                      be asked to transform. Must be a power of 2. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid parameter. </exception>
		///
		public FFTTransformer(int size) : this(size, (Window) null)
		{
		}


		///
		/// <summary> * Create an FFT transformer for a given sample size. This preallocates
		/// * resources appropriate to that block size. A specified window
		/// * function will be applied to all input data.
		/// *  </summary>
		/// * <param name="size"> The number of samples in a block that we will
		/// *                      be asked to transform. Must be a power of 2. </param>
		/// * <param name="winfunc"> Window function to apply to all input data. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid parameter. </exception>
		///
		public FFTTransformer(int size, Window.Function winfunc) : this(size, new Window(size, winfunc))
		{
		}


		///
		/// <summary> * Create an FFT transformer for a given sample size. This preallocates
		/// * resources appropriate to that block size. A specified window
		/// * function will be applied to all input data.
		/// *  </summary>
		/// * <param name="size"> The number of samples in a block that we will
		/// *                      be asked to transform. Must be a power of 2. </param>
		/// * <param name="window"> Window function to apply to all input data.
		/// *                      Its block size must be the same as the size
		/// *                      parameter. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid parameter. </exception>
		///
		public FFTTransformer(int size, Window window)
		{
			if (!IsPowerOf2(size))
			{
				throw new System.ArgumentException("size for FFT must" + " be a power of 2 (was " + size + ")");
			}

			windowFunc = window;
			transformer = new LomontFFT();
			blockSize = size;

			// Allocate working data arrays.
			xre = new double[blockSize];
		}


		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		/// <summary> * Set a new windowing function for this analyser.
		/// *  </summary>
		/// * <param name="func"> The desired windowing function. </param>
		///
		public Window.Function WindowFunc
		{
			set
			{
				windowFunc = new Window(blockSize, value);
			}
		}


		// ******************************************************************** //
		// Data Setup.
		// ******************************************************************** //

		///
		/// <summary> * Set up a new data block for the FFT algorithm. The data in
		/// * the provided buffer will be copied out, and that buffer
		/// * will not be referenced again. This is separated
		/// * out from the main computation to allow for more efficient use
		/// * of locks.
		/// *  </summary>
		/// * <param name="input">  The input data buffer. </param>
		/// * <param name="off">  Offset in the buffer at which the data to
		/// *                      be transformed starts. </param>
		/// * <param name="count">  Number of samples in the data to be
		/// *                      transformed. Must be the same as the size
		/// *                      parameter that was given to the constructor. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid data size. </exception>
		///
		public void SetInput(float[] input, int off, int count)
		{
			if (count != blockSize)
			{
				throw new System.ArgumentException("bad input count in FFT:" + " constructed for " + blockSize + "; given " + input.Length);
			}

			// Copy and transform the samples into our internal data buffer.
			for (int i = 0; i < blockSize; i++)
			{
				xre[i] = input[off + i];
			}
		}


		///
		/// <summary> * Set up a new data block for the FFT algorithm. The data in
		/// * the provided buffer will be copied out, and that buffer
		/// * will not be referenced again. This is separated
		/// * out from the main computation to allow for more efficient use
		/// * of locks.
		/// *  </summary>
		/// * <param name="input">  The input data buffer. </param>
		/// * <param name="off">  Offset in the buffer at which the data to
		/// *                      be transformed starts. </param>
		/// * <param name="count">  Number of samples in the data to be
		/// *                      transformed. Must be the same as the size
		/// *                      parameter that was given to the constructor. </param>
		/// * <exception cref="IllegalArgumentException"> Invalid data size. </exception>
		///
		public void SetInput(short[] input, int off, int count)
		{
			if (count != blockSize)
			{
				throw new System.ArgumentException("bad input count in FFT:" + " constructed for " + blockSize + "; given " + input.Length);
			}

			// Copy and transform the samples into our internal data buffer.
			for (int i = 0; i < blockSize; i++)
			{
				xre[i] = (double) input[off + i] / 32768.0;
			}
		}


		// ******************************************************************** //
		// Transform.
		// ******************************************************************** //

		///
		/// <summary> * Transform the data provided in the last call to SetInput. </summary>
		///
		public void transform()
		{
			// If we have a window function, apply it now.
			if (windowFunc != null)
			{
				windowFunc.transform(xre);
			}
			
			// Do the FFT.
			transformer.FFT(xre, true);
		}


		// ******************************************************************** //
		// Results.
		// ******************************************************************** //

		///
		/// <summary> * Get the real results of the last transformation.
		/// *  </summary>
		/// * <param name="buffer">  Buffer in which the real part of the results
		/// *                  will be placed. This buffer must be half the
		/// *                  length of the input block. If transform() has
		/// *                  not been called, the results will be garbage. </param>
		/// * <returns>   The parameter buffer. </returns>
		/// * <exception cref="IllegalArgumentException"> Invalid buffer size. </exception>
		///
		public float[] getResults(float[] buffer)
		{
			if (buffer.Length != blockSize / 2)
			{
				throw new System.ArgumentException("bad output buffer size in FFT:" + " must be " + (blockSize / 2) + "; given " + buffer.Length);
			}

			float scale = blockSize * FUDGE;
			for (int i = 0; i < blockSize / 2; i++)
			{
				double r = xre[i * 2];
				double im = i == 0 ? 0.0 : xre[i * 2 - 1];
				buffer[i] = (float)(Math.Sqrt(r * r + im * im)) / scale;
			}
			return buffer;
		}


		///
		/// <summary> * Get the rolling average real results of the last n transformations.
		/// *  </summary>
		/// * <param name="average"> Buffer in which the averaged real part of the
		/// *                      results will be maintained. This buffer must be
		/// *                      half the length of the input block. It is
		/// *                      important that this buffer is kept intact and
		/// *                      undisturbed between calls, as the average
		/// *                      calculation for each value depends on the
		/// *                      previous average. </param>
		/// * <param name="histories">   Buffer in which the historical values of the
		/// *                      results will be kept. This must be a rectangular
		/// *                      array, the first dimension being the same as
		/// *                      average. The second dimension determines the
		/// *                      length of the history, and hence the time over
		/// *                      which values are averaged. It is
		/// *                      important that this buffer is kept intact and
		/// *                      undisturbed between calls. </param>
		/// * <param name="index">  Current history index. The caller needs to pass
		/// *                      in zero initially, and save the return value
		/// *                      of this method to pass in as index next time. </param>
		/// * <returns>  The updated index value. Pass this in as
		/// *                      the index parameter next time around. </returns>
		/// * <exception cref="IllegalArgumentException"> Invalid buffer size. </exception>
		///
		public int getResults(float[] average, float[][] histories, int index)
		{
			if (average.Length != blockSize / 2)
			{
				throw new System.ArgumentException("bad history buffer size in FFT:" + " must be " + (blockSize / 2) + "; given " + average.Length);
			}
			if (histories.Length != blockSize / 2)
			{
				throw new System.ArgumentException("bad average buffer size in FFT:" + " must be " + (blockSize / 2) + "; given " + histories.Length);
			}

			// Update the index.
			int historyLen = histories[0].Length;
			if (++index >= historyLen)
			{
				index = 0;
			}

			// Now do the rolling average of each value.
			float scale = blockSize * FUDGE;
			for (int i = 0; i < blockSize / 2; i++)
			{
				double r = xre[i * 2];
				double im = i == 0 ? 0.0 : xre[i * 2 - 1];
				float val = (float)(Math.Sqrt(r * r + im * im)) / scale;

				float[] hist = histories[i];
				float prev = hist[index];
				hist[index] = val;
				average[i] = average[i] - prev / historyLen + val / historyLen;
			}

			return index;
		}


		// ******************************************************************** //
		// Results Analysis.
		// ******************************************************************** //

		///
		/// <summary> * Given the results of an FFT, identify prominent frequencies
		/// * in the spectrum.
		/// * 
		/// * <p><b>Note:</b> this is experimental and not very good.
		/// *  </summary>
		/// * <param name="spectrum"> Audio spectrum data, as returned by
		/// *                      <seealso cref="#getResults(float[])"/>. </param>
		/// * <param name="results"> Buffer into which the results will be placed. </param>
		/// * <returns>  The parameter buffer. </returns>
		/// * <exception cref="IllegalArgumentException"> Invalid buffer size. </exception>
		///
		public int findKeyFrequencies(float[] spectrum, float[] results)
		{
			int len = spectrum.Length;

			// Find the average strength.
			float average = 0f;
			for (int i = 0; i < len; ++i)
			{
				average += spectrum[i];
			}
			average /= len;

			// Find all excursions above 2*average. Group adjacent highs
			// together.
			int count = 0;
			for (int i = 0; i < len && count < results.Length; ++i)
			{
				if (spectrum[i] > 2 * average)
				{
					// Compute the weighted average frequency of this peak.
					float tot = 0f;
					float wavg = 0f;
					int j;
					for (j = i; j < len && spectrum[j] > 3 * average; ++j)
					{
						tot += spectrum[j];
						wavg += spectrum[j] * (float) j;
					}
					wavg /= tot;
					results[count++] = wavg;

					// Skip past this peak.
					i = j;
				}
			}

			return count;
		}

		public static bool IsPowerOf2(int num) {
			return num > 0 && (num & (num - 1)) == 0;
		}

		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Fudge factor to scale the FFT output to the range 0-1.
		private const float FUDGE = 0.63610f;


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// Window function to apply to all input data. If null, no windowing.
		private Window windowFunc = null;

		// The FFT transformer.
		private LomontFFT transformer;

		// The size of an input data block.
		private readonly int blockSize;

		// Working array -- real data being processed.
		private readonly double[] xre;

	}

}