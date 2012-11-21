using System;

//
// *  Copyright (c) 2007 - 2008 by Damien Di Fede <ddf@compartmental.net>
// *
// *   This program is free software; you can redistribute it and/or modify
// *   it under the terms of the GNU Library General Public License as published
// *   by the Free Software Foundation; either version 2 of the License, or
// *   (at your option) any later version.
// *
// *   This program is distributed in the hope that it will be useful,
// *   but WITHOUT ANY WARRANTY; without even the implied warranty of
// *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// *   GNU Library General Public License for more details.
// *
// *   You should have received a copy of the GNU Library General Public
// *   License along with this program; if not, write to the Free Software
// *   Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// 

namespace com.badlogic.audio.analysis
{
	///
	// * DFT stands for Discrete Fourier Transform and is the most widely used Fourier
	// * Transform. You will never want to use this class due to the fact that it is a
	// * brute force implementation of the DFT and as such is quite slow. Use an FFT
	// * instead. This exists primarily as a way to ensure that other implementations
	// * of the DFT are working properly. This implementation expects an even
	// * <code>timeSize</code> and will throw and ArgumentException if this
	// * is not the case.
	// * 
	// * @author Damien Di Fede
	// * 
	// * @see FourierTransform
	// * @see FFT
	// * @see <a href="http://www.dspguide.com/ch8.htm">The Discrete Fourier Transform</a>
	// * 
	// 
	public class DFT : FourierTransform
	{
		//  *
		//   * Constructs a DFT that expects audio buffers of length <code>timeSize</code> that
		//   * have been recorded with a sample rate of <code>sampleRate</code>. Will throw an
		//   * ArgumentException if <code>timeSize</code> is not even.
		//   * 
		//   * @param timeSize the length of the audio buffers you plan to analyze
		//   * @param sampleRate the sample rate of the audio samples you plan to analyze
		//
		public DFT(int timeSize, float sampleRate) : base(timeSize, sampleRate)
		{
			if (timeSize % 2 != 0)
				throw new ArgumentException("DFT: timeSize must be even.");
			buildTrigTables();
		}

		protected internal override void AllocateArrays()
		{
			spectrum = new float[timeSize / 2 + 1];
			real = new float[timeSize / 2 + 1];
			imag = new float[timeSize / 2 + 1];
		}

		//  *
		//   * Not currently implemented.
		//
		public override void ScaleBand(int i, float s)
		{
		}

		//  *
		//   * Not currently implemented.
		//
		public override void SetBand(int i, float a)
		{
		}

		public override void Forward(float[] samples)
		{
			if (samples.Length != timeSize)
			{
				throw new ArgumentException("DFT.forward: The length of the passed sample buffer must be equal to DFT.timeSize().");
			}
			DoWindow(samples);
			int N = samples.Length;
			for (int f = 0; f <= N / 2; f++)
			{
				real[f] = 0.0f;
				imag[f] = 0.0f;
				for (int t = 0; t < N; t++)
				{
					real[f] += samples[t] * cos(t * f);
					imag[f] += samples[t] * -sin(t * f);
				}
			}
			FillSpectrum();
		}

		public override void Inverse(float[] buffer)
		{
			int N = buffer.Length;
			real[0] /= N;
			imag[0] = -imag[0] / (N / 2);
			real[N / 2] /= N;
			imag[N / 2] = -imag[0] / (N / 2);
			for (int i = 0; i < N / 2; i++)
			{
				real[i] /= (N / 2);
				imag[i] = -imag[i] / (N / 2);
			}
			for (int t = 0; t < N; t++)
			{
				buffer[t] = 0.0f;
				for (int f = 0; f < N / 2; f++)
				{
					buffer[t] += real[f] * cos(t * f) + imag[f] * sin(t * f);
				}
			}
		}

		// lookup table data and functions

		private float[] sinlookup;
		private float[] coslookup;

		private void buildTrigTables()
		{
			int N = spectrum.Length * timeSize;
			sinlookup = new float[N];
			coslookup = new float[N];
			for (int i = 0; i < N; i++)
			{
				sinlookup[i] = (float) Math.Sin(i * TWO_PI / timeSize);
				coslookup[i] = (float) Math.Cos(i * TWO_PI / timeSize);
			}
		}

		private float sin(int i)
		{
			return sinlookup[i];
		}

		private float cos(int i)
		{
			return coslookup[i];
		}
	}
}