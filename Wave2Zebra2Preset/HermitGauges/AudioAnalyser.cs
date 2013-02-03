///
/// <summary> * org.hermit.android.instrument: graphical instruments for Android.
/// * <br>Copyright 2009 Ian Cameron Smith
/// * 
/// * <p>These classes provide input and display functions for creating on-screen
/// * instruments of various kinds in Android apps.
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

using Wave2Zebra2Preset.HermitGauges;
using CommonUtils.Audio.Bass;
using Lomont;

namespace Wave2Zebra2Preset.HermitGauges
{

	///
	/// <summary> * An <seealso cref="Instrument"/> which analyses an audio stream in various ways.
	/// * 
	/// * <p>To use this class, your application must have permission RECORD_AUDIO. </summary>
	/// 
	public class AudioAnalyser : Instrument
	{

		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Create a AudioAnalyser instance. </summary>
		///
		public AudioAnalyser()
		{
			audioReader = new BassProxy();

			spectrumAnalyser = new FFTTransformer(inputBlockSize, windowFunction);

			// Allocate the spectrum data.
			spectrumData = new float[inputBlockSize / 2];
			spectrumHist = RectangularArrays.ReturnRectangularFloatArray(inputBlockSize / 2, historyLen);
			spectrumIndex = 0;

			biasRange = new float[2];
		}


		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		/// <summary> * Set the sample rate for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The desired rate, in samples/sec. </param>
		///
		public virtual int SampleRate
		{
			set
			{
				sampleRate = value;
				
				// The spectrum gauge needs to know this.
				if (spectrumGauge != null)
				{
					spectrumGauge.SampleRate = sampleRate;
				}
				
				// The sonagram gauge needs to know this.
				if (sonagramGauge != null)
				{
					sonagramGauge.SampleRate = sampleRate;
				}
			}
		}


		///
		/// <summary> * Set the input block size for this instrument.
		/// *  </summary>
		/// * <param name="size"> The desired block size, in samples. Typical
		/// *                      values would be 256, 512, or 1024. Larger block
		/// *                      sizes will mean more work to analyse the spectrum. </param>
		///
		public virtual int BlockSize
		{
			set
			{
				inputBlockSize = value;
				
				spectrumAnalyser = new FFTTransformer(inputBlockSize, windowFunction);
				
				// Allocate the spectrum data.
				spectrumData = new float[inputBlockSize / 2];
				spectrumHist = RectangularArrays.ReturnRectangularFloatArray(inputBlockSize / 2, historyLen);
			}
		}

		///
		/// <summary> * Set the spectrum analyser windowing function for this instrument.
		/// *  </summary>
		/// * <param name="func"> The desired windowing function.
		/// *                      Window.Function.BLACKMAN_HARRIS is a good option.
		/// *                      Window.Function.RECTANGULAR turns off windowing. </param>
		///
		public virtual Window.Function WindowFunc
		{
			set
			{
				windowFunction = value;
				spectrumAnalyser.WindowFunc = value;
			}
		}

		///
		/// <summary> * Set the decimation rate for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The desired decimation. Only 1 in rate blocks
		/// *                      will actually be processed. </param>
		///
		public virtual int Decimation
		{
			set
			{
				sampleDecimate = value;
			}
		}


		///
		/// <summary> * Set the histogram averaging window for this instrument.
		/// *  </summary>
		/// * <param name="len">  The averaging interval. 1 means no averaging. </param>
		///
		public virtual int AverageLen
		{
			set
			{
				historyLen = value;
				
				// Set up the history buffer.
				spectrumHist = RectangularArrays.ReturnRectangularFloatArray(inputBlockSize / 2, historyLen);
				spectrumIndex = 0;
			}
		}


		// ******************************************************************** //
		// Gauges.
		// ******************************************************************** //

		///
		/// <summary> * Get a waveform gauge for this audio analyser.
		/// *  </summary>
		/// * <param name="surface"> The surface in which the gauge will be displayed. </param>
		/// * <returns>  A gauge which will display the audio waveform. </returns>
		///
		public virtual WaveformGauge getWaveformGauge()
		{
			if (waveformGauge != null)
			{
				throw new Exception("Already have a WaveformGauge" + " for this AudioAnalyser");
			}
			waveformGauge = new WaveformGauge();
			return waveformGauge;
		}


		///
		/// <summary> * Get a spectrum analyser gauge for this audio analyser.
		/// *  </summary>
		/// * <param name="surface"> The surface in which the gauge will be displayed. </param>
		/// * <returns>  A gauge which will display the audio waveform. </returns>
		///
		public virtual SpectrumGauge getSpectrumGauge()
		{
			if (spectrumGauge != null)
			{
				throw new Exception("Already have a SpectrumGauge" + " for this AudioAnalyser");
			}
			spectrumGauge = new SpectrumGauge(sampleRate);
			return spectrumGauge;
		}


		///
		/// <summary> * Get a sonagram analyser gauge for this audio analyser.
		/// *  </summary>
		/// * <param name="surface"> The surface in which the gauge will be displayed. </param>
		/// * <returns>  A gauge which will display the audio waveform
		///	 *						as a sonogram. </returns>
		///
		public virtual SonagramGauge getSonagramGauge()
		{
			if (sonagramGauge != null)
			{
				throw new Exception("Already have a SonagramGauge" + " for this AudioAnalyser");
			}
			sonagramGauge = new SonagramGauge(sampleRate, inputBlockSize);
			return sonagramGauge;
		}


		///
		/// <summary> * Get a signal power gauge for this audio analyser.
		/// *  </summary>
		/// * <param name="surface"> The surface in which the gauge will be displayed. </param>
		/// * <returns>  A gauge which will display the signal power in
		/// *                      a dB meter. </returns>
		///
		public virtual PowerGauge getPowerGauge()
		{
			if (powerGauge != null)
			{
				throw new Exception("Already have a PowerGauge" + " for this AudioAnalyser");
			}
			powerGauge = new PowerGauge();
			return powerGauge;
		}


		///
		/// <summary> * Reset all Gauges before choosing new ones. </summary>
		///
		public virtual void resetGauge()
		{
			lock (this)
			{
				waveformGauge=null;
				spectrumGauge=null;
				sonagramGauge=null;
				powerGauge=null;
			}
		}


		// ******************************************************************** //
		// Audio Processing.
		// ******************************************************************** //

		///
		/// <summary> * Handle audio input. This is called on the thread of the audio
		/// * reader.
		/// *  </summary>
		/// * <param name="buffer"> Audio data that was just read. </param>
		///
		private void receiveAudio(short[] buffer)
		{
			// Lock to protect updates to these local variables. See run().
			lock (this)
			{
				audioData = buffer;
				++audioSequence;
			}
		}


		///
		/// <summary> * An error has occurred. The reader has been terminated.
		/// *  </summary>
		/// * <param name="error">  ERR_XXX code describing the error. </param>
		///
		private void handleError(int error)
		{
			lock (this)
			{
				readError = error;
			}
		}


		// ******************************************************************** //
		// Main Loop.
		// ******************************************************************** //

		///
		/// <summary> * Update the state of the instrument for the current frame.
		/// * This method must be invoked from the doUpdate() method of the
		/// * application's <seealso cref=""/>.
		/// * 
		/// * <p>Since this is called frequently, we first check whether new
		/// * audio data has actually arrived.
		/// *  </summary>
		/// * <param name="now">  Nominal time of the current frame in ms. </param>
		///
		protected internal override sealed void doUpdate(long now)
		{
			short[] buffer = null;
			lock (this)
			{
				if (audioData != null && audioSequence > audioProcessed)
				{
					audioProcessed = audioSequence;
					buffer = audioData;
				}
			}

			// If we got data, process it without the lock.
			if (buffer != null)
			{
				ProcessAudio(buffer);
			}

			//if (readError != AudioReader.Listener.ERR_OK)
			//{
			//	processError(readError);
			//}
		}


		///
		/// <summary> * Handle audio input. This is called on the thread of the
		/// * parent surface.
		/// *  </summary>
		/// * <param name="buffer"> Audio data that was just read. </param>
		///
		public void ProcessAudio(short[] buffer)
		{
			// Process the buffer. While reading it, it needs to be locked.
			lock (buffer)
			{
				// Calculate the power now, while we have the input
				// buffer; this is pretty cheap.
				int len = buffer.Length;

				// Draw the waveform now, while we have the raw data.
				if (waveformGauge != null)
				{
					SignalPower.BiasAndRange(buffer, len - inputBlockSize, inputBlockSize, biasRange);
					float bias = biasRange[0];
					float range = biasRange[1];
					if (range < 1f)
					{
						range = 1f;
					}

					// long wavStart = System.currentTimeMillis();
					waveformGauge.Update(buffer, len - inputBlockSize, inputBlockSize, bias, range);
					// long wavEnd = System.currentTimeMillis();
					// parentSurface.statsTime(1, (wavEnd - wavStart) * 1000);
				}

				// If we have a power gauge, calculate the signal power.
				if (powerGauge != null)
				{
					currentPower = SignalPower.CalculatePowerDb(buffer, 0, len);
				}

				// If we have a spectrum or sonagram analyser, set up the FFT input data.
				if (spectrumGauge != null || sonagramGauge != null)
				{
					spectrumAnalyser.SetInput(buffer, len - inputBlockSize, inputBlockSize);
				}

				// Tell the reader we're done with the buffer.
				//buffer.notify();
			}

			// If we have a spectrum or sonagram analyser, perform the FFT.
			if (spectrumGauge != null || sonagramGauge != null)
			{
				// Do the (expensive) transformation.
				// The transformer has its own state, no need to lock here.
				spectrumAnalyser.transform();

				// Get the FFT output.
				if (historyLen <= 1)
				{
					spectrumAnalyser.getResults(spectrumData);
				}
				else
				{
					spectrumIndex = spectrumAnalyser.getResults(spectrumData, spectrumHist, spectrumIndex);
				}
			}

			// If we have a spectrum gauge, update data and draw.
			if (spectrumGauge != null)
			{
				spectrumGauge.Update(spectrumData);
			}

			// If we have a sonagram gauge, update data and draw.
			if (sonagramGauge != null)
			{
				sonagramGauge.Update(spectrumData);
			}

			// If we have a power gauge, display the signal power.
			if (powerGauge != null)
			{
				powerGauge.Update(currentPower);
			}
		}


		///
		/// <summary> * Handle an audio input error.
		/// *  </summary>
		/// * <param name="error">  ERR_XXX code describing the error. </param>
		///
		private void processError(int error)
		{
			// Pass the error to all the gauges we have.
			if (waveformGauge != null)
			{
				waveformGauge.error(error);
			}
			if (spectrumGauge != null)
			{
				spectrumGauge.error(error);
			}
			if (sonagramGauge != null)
			{
				sonagramGauge.error(error);
			}
			if (powerGauge != null)
			{
				powerGauge.error(error);
			}
		}

		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		// Debugging tag.
		private const String TAG = "instrument";


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// The desired sampling rate for this analyser, in samples/sec.
		private int sampleRate = 8000;

		// Audio input block size, in samples.
		private int inputBlockSize = 256;

		// The selected windowing function.
		private Window.Function windowFunction = Window.Function.BLACKMAN_HARRIS;

		// The desired decimation rate for this analyser. Only 1 in
		// sampleDecimate blocks will actually be processed.
		private int sampleDecimate = 1;

		// The desired histogram averaging window. 1 means no averaging.
		private int historyLen = 4;

		// Our audio input device.
		private readonly BassProxy audioReader;

		// Fourier Transform calculator we use for calculating the spectrum
		// and sonagram.
		private FFTTransformer spectrumAnalyser;

		// The gauges associated with this instrument. Any may be null if not
		// in use.
		private WaveformGauge waveformGauge = null;
		private SpectrumGauge spectrumGauge = null;
		private SonagramGauge sonagramGauge = null;
		private PowerGauge powerGauge = null;

		// Buffered audio data, and sequence number of the latest block.
		private short[] audioData;
		private long audioSequence = 0;

		// If we got a read error, the error code.
		private int readError = -1;

		// Sequence number of the last block we processed.
		private long audioProcessed = 0;

		// Analysed audio spectrum data; history data for each frequency
		// in the spectrum; index into the history data; and buffer for
		// peak frequencies.
		private float[] spectrumData;
		private float[][] spectrumHist;
		private int spectrumIndex;

		// Current signal power level, in dB relative to max. input power.
		private double currentPower = 0f;

		// Temp. buffer for calculated bias and range.
		private float[] biasRange = null;

	}

}

//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length.
//----------------------------------------------------------------------------------------
internal static partial class RectangularArrays
{
	internal static float[][] ReturnRectangularFloatArray(int Size1, int Size2)
	{
		float[][] Array = new float[Size1][];
		for (int Array1 = 0; Array1 < Size1; Array1++)
		{
			Array[Array1] = new float[Size2];
		}
		return Array;
	}
}