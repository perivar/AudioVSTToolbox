// (c) Copyright Jacob Johnston.
// This source is subject to Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using NAudio.Dsp;

namespace NAudio_Visualizing
{
	public class SampleAggregator
	{
		private float volumeLeftMaxValue;
		private float volumeLeftMinValue;
		private float volumeRightMaxValue;
		private float volumeRightMinValue;
		private Complex[] channelData;
		private int bufferSize;
		private int binaryExponentitation;
		private int channelDataPosition;

		public SampleAggregator(int bufferSize)
		{
			this.bufferSize = bufferSize;
			binaryExponentitation = (int)Math.Log(bufferSize, 2);
			channelData = new Complex[bufferSize];
		}

		public void Clear()
		{
			volumeLeftMaxValue = float.MinValue;
			volumeRightMaxValue = float.MinValue;
			volumeLeftMinValue = float.MaxValue;
			volumeRightMinValue = float.MaxValue;
			channelDataPosition = 0;
		}
		
		/// <summary>
		/// Add a sample value to the aggregator.
		/// </summary>
		/// <param name="value">The value of the sample.</param>
		public void Add(float leftValue, float rightValue)
		{
			if (channelDataPosition == 0)
			{
				volumeLeftMaxValue = float.MinValue;
				volumeRightMaxValue = float.MinValue;
				volumeLeftMinValue = float.MaxValue;
				volumeRightMinValue = float.MaxValue;
			}

			// Make stored channel data stereo by averaging left and right values.
			channelData[channelDataPosition].X = (leftValue + rightValue) / 2.0f;
			channelData[channelDataPosition].Y = 0;
			channelDataPosition++;

			volumeLeftMaxValue = Math.Max(volumeLeftMaxValue, leftValue);
			volumeLeftMinValue = Math.Min(volumeLeftMinValue, leftValue);
			volumeRightMaxValue = Math.Max(volumeRightMaxValue, rightValue);
			volumeRightMinValue = Math.Min(volumeRightMinValue, rightValue);

			if (channelDataPosition >= channelData.Length)
			{
				channelDataPosition = 0;
			}
		}
		
		// windows the data in samples with a Hamming window
		private void doWindow(Complex[] complex)
		{
			double[] windowArray = CommonUtils.FFT.FFTWindowFunctions.GetWindowFunction(CommonUtils.FFT.FFTWindowFunctions.HANNING, complex.Length);
			
			for (int i = 0; i < complex.Length; i++)
			{
				//complex[i].X *= (float)(0.54f - 0.46f * Math.Cos(2 * Math.PI * i / (complex.Length - 1)));
				complex[i].X *= (float) windowArray[i];
			}
		}
		
		/// <summary>
		/// Performs an FFT calculation on the channel data upon request.
		/// </summary>
		/// <param name="fftBuffer">A buffer where the FFT data will be stored.</param>
		public void GetFFTResults(float[] fftBuffer)
		{
			Complex[] channelDataClone = new Complex[bufferSize];
			channelData.CopyTo(channelDataClone, 0);
			
			doWindow(channelDataClone);
			
			// Use NAUDIO FFT
			FastFourierTransform.FFT(true, binaryExponentitation, channelDataClone);
			for (int i = 0; i < channelDataClone.Length / 2; i++)
			{
				// Calculate actual intensities for the FFT results.
				fftBuffer[i] = (float)Math.Sqrt(channelDataClone[i].X * channelDataClone[i].X + channelDataClone[i].Y * channelDataClone[i].Y);
			}
			
			/*
			// Use Lomont FFT
			Lomont.LomontFFT fft = new Lomont.LomontFFT();
			double[] complexSignal = new double[2*channelDataClone.Length];
			for (int j = 0; j < channelDataClone.Length; j++)
			{
				complexSignal[2*j] = channelDataClone[j].X;
				complexSignal[2*j + 1] = channelDataClone[j].Y;
			}

			// FFT transform for gathering the spectrum
			fft.FFT(complexSignal, true);
			
			for (int i = 0; i < channelDataClone.Length / 2; i++)
			{
				double re = complexSignal[2*i] / (float) Math.Sqrt(channelDataClone.Length);
				double img = complexSignal[2*i + 1] / (float) Math.Sqrt(channelDataClone.Length);

				// Calculate actual intensities for the FFT results.
				fftBuffer[i] = (float)Math.Sqrt(re*re + img*img) * 4;
			}
			 */
		}

		public float LeftMaxVolume
		{
			get { return volumeLeftMaxValue; }
		}

		public float LeftMinVolume
		{
			get { return volumeLeftMinValue; }
		}

		public float RightMaxVolume
		{
			get { return volumeRightMaxValue; }
		}

		public float RightMinVolume
		{
			get { return volumeRightMinValue; }
		}
	}
}
