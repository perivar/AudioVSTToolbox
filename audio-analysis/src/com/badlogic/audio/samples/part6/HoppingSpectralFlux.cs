using System;
using System.Drawing;
using System.Collections.Generic;

using NAudio.Wave;

using SpectrumProvider = com.badlogic.audio.analysis.SpectrumProvider;
using ThresholdFunction = com.badlogic.audio.analysis.ThresholdFunction;
using PlaybackVisualizer = com.badlogic.audio.visualization.PlaybackVisualizer;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part6
{
	///
	// * Demonstrates the calculation of the spectral flux function
	// * hopping fractions of the original 1024 sample window.
	// * 
	// * @author mzechner
	public class HoppingSpectralFlux
	{
		public const string FILE = "samples/judith.mp3";
		public const int HOP_SIZE = 512;

		public static void Main(string[] argv)
		{
			ISampleProvider decoder = new AudioFileReader(FILE);
			SpectrumProvider spectrumProvider = new SpectrumProvider(decoder, 1024, HOP_SIZE, true);
			float[] spectrum = spectrumProvider.nextSpectrum();
			float[] lastSpectrum = new float[spectrum.Length];
			List<float> spectralFlux = new List<float>();

			do
			{
				float flux = 0;
				for(int i = 0; i < spectrum.Length; i++)
				{
					float @value = (spectrum[i] - lastSpectrum[i]);
					flux += @value < 0 ? 0 : @value;
				}
				spectralFlux.Add(flux);

				System.Array.Copy(spectrum, 0, lastSpectrum, 0, spectrum.Length);
			} while((spectrum = spectrumProvider.nextSpectrum()) != null);

			Plot plot = new Plot("Hopping Spectral Flux", 1024, 512);
			plot.plot(spectralFlux, 1, Color.Red);
			new PlaybackVisualizer(plot, HOP_SIZE, FILE);
		}
	}

}