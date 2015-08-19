using System;
using System.Drawing;
using System.Collections.Generic;

using NAudio.Wave;

using SpectrumProvider = com.badlogic.audio.analysis.SpectrumProvider;
using ThresholdFunction = com.badlogic.audio.analysis.ThresholdFunction;
using PlaybackVisualizer = com.badlogic.audio.visualization.PlaybackVisualizer;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part7
{
	public class MultiBandThreshold
	{
		public const string FILE = "samples/jazz.mp3";
		public const int HOP_SIZE = 512;
		public const int HISTORY_SIZE = 50;
		public static readonly float[] multipliers = { 2f, 2f, 2f };
		public static readonly float[] bands = { 80, 4000, 4000, 10000, 10000, 16000 };

		public static void Main(string[] argv)
		{
			ISampleProvider decoder = new AudioFileReader(FILE);
			SpectrumProvider spectrumProvider = new SpectrumProvider(decoder, 1024, HOP_SIZE, true);
			float[] spectrum = spectrumProvider.nextSpectrum();
			float[] lastSpectrum = new float[spectrum.Length];
			List<List<float>> spectralFlux = new List<List<float>>();
			for(int i = 0; i < bands.Length / 2; i++) {
				spectralFlux.Add(new List<float>());
			}

			do {
				for(int i = 0; i < bands.Length; i+=2) {
					int startFreq = spectrumProvider.getFFT().FreqToIndex(bands[i]);
					int endFreq = spectrumProvider.getFFT().FreqToIndex(bands[i+1]);
					float flux = 0;
					for(int j = startFreq; j <= endFreq; j++)
					{
						float @value = (spectrum[j] - lastSpectrum[j]);
						@value = (@value + Math.Abs(@value))/2;
						flux += @value;
					}
					spectralFlux[i/2].Add(flux);
				}

				System.Array.Copy(spectrum, 0, lastSpectrum, 0, spectrum.Length);
			} while((spectrum = spectrumProvider.nextSpectrum()) != null);

			List<List<float>> thresholds = new List<List<float>>();
			for(int i = 0; i < bands.Length / 2; i++)
			{
				List<float> threshold = new ThresholdFunction(HISTORY_SIZE, multipliers[i]).calculate(spectralFlux[i]);
				thresholds.Add(threshold);
			}

			Plot plot = new Plot("Multiband Spectral Flux", 1024, 512);
			for(int i = 0; i < bands.Length / 2; i++)
			{
				plot.plot(spectralFlux[i], 1, -0.6f * (bands.Length / 2 - 2) + i, false, Color.Red);
				plot.plot(thresholds[i], 1, -0.6f * (bands.Length / 2 - 2) + i, true, Color.Green);
			}

			new PlaybackVisualizer(plot, HOP_SIZE, FILE);
		}
	}
}