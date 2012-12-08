using System;

using NAudio;
using NAudio.Wave;

namespace com.badlogic.audio.analysis
{
	// Provides float[] arrays of successive spectrum frames retrieved via
	// FFT from a Decoder. The frames might overlapp by n samples also called
	// the hop size. Using a hop size smaller than the spectrum size is beneficial
	// in most cases as it smears out the spectra of successive frames somewhat.
	// @author mzechner
	public class SpectrumProvider
	{
		/// the decoder to use
		private readonly ISampleProvider decoder;

		/// the current sample array
		private float[] samples;

		/// the look ahead sample array
		private float[] nextSamples;

		/// temporary samples array
		private float[] tempSamples;

		/// the current sample, always modulo sample window size
		private int currentSample = 0;

		/// the hop size
		private readonly int hopSize;

		/// the fft
		private readonly FFT fft;

		// Constructor, sets the {@link Decoder}, the sample window size and the
		// hop size for the spectra returned. Say the sample window size is 1024
		// samples. To get an overlapp of 50% you specify a hop size of 512 samples,
		// for 25% overlap you specify a hopsize of 256 and so on. Hop sizes are of
		// course not limited to powers of 2.
		// 
		// @param decoder The decoder to get the samples from.
		// @param sampleWindowSize The sample window size.
		// @param hopSize The hop size.
		// @param useHamming Wheter to use hamming smoothing or not.
		public SpectrumProvider(ISampleProvider decoder, int sampleWindowSize, int hopSize, bool useHamming)
		{
			if(decoder == null)
				throw new ArgumentException("Decoder must be != null");

			if(sampleWindowSize <= 0)
				throw new ArgumentException("Sample window size must be > 0");
			if(hopSize <= 0)
				throw new ArgumentException("Hop size must be > 0");

			if(sampleWindowSize < hopSize)
				throw new ArgumentException("Hop size must be <= sampleSize");


			this.decoder = decoder;
			this.samples = new float[sampleWindowSize];
			this.nextSamples = new float[sampleWindowSize];
			this.tempSamples = new float[sampleWindowSize];
			this.hopSize = hopSize;
			fft = new FFT(sampleWindowSize, 44100);

			// calculate averages based on a miminum octave width of 22 Hz
			// split each octave into three bands
			// this should result in 30 averages
			//fft.LogAverages(22, 3);
			
			if(useHamming)
				fft.Window(FFT.HAMMING);

			decoder.Read(samples, 0, samples.Length);
			decoder.Read(nextSamples, 0, nextSamples.Length);
		}

		// Returns the next spectrum or null if there's no more data.
		// @return The next spectrum or null.
		public virtual float[] nextSpectrum()
		{
			if(currentSample >= samples.Length)
			{
				float[] tmp = nextSamples;
				nextSamples = samples;
				samples = tmp;
				if(decoder.Read(nextSamples, 0, nextSamples.Length) == 0)
					return null;
				currentSample -= samples.Length;
			}

			System.Array.Copy(samples, currentSample, tempSamples, 0, samples.Length - currentSample);
			System.Array.Copy(nextSamples, 0, tempSamples, samples.Length - currentSample, currentSample);
			fft.Forward(tempSamples);
			currentSample += hopSize;
			return fft.GetSpectrum();
		}

		// @return the FFT instance used
		public virtual FFT getFFT()
		{
			return fft;
		}
	}
}