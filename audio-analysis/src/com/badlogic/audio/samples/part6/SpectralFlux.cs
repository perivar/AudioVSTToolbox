using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using NAudio;
using NAudio.Wave;

using FFT = com.badlogic.audio.analysis.FFT;
using PlaybackVisualizer = com.badlogic.audio.visualization.PlaybackVisualizer;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part6
{
	// * Calculates the spectral flux of a song and displays the
	// * resulting plot.
	// * 
	// * @author mzechner
	public class SpectralFlux
	{
		public const string FILE = "samples/judith.mp3";

		public static void Main(string[] argv)
		{
			ISampleProvider decoder = new AudioFileReader(FILE);
			FFT fft = new FFT(1024, 44100);
			float[] samples = new float[1024];
			float[] spectrum = new float[1024 / 2 + 1];
			float[] lastSpectrum = new float[1024 / 2 + 1];
			List<float> spectralFlux = new List<float>();

			while(decoder.Read(samples, 0, samples.Length) > 0)
			{
				fft.Forward(samples);
				System.Array.Copy(spectrum, 0, lastSpectrum, 0, spectrum.Length);
				System.Array.Copy(fft.GetSpectrum(), 0, spectrum, 0, spectrum.Length);

				float flux = 0;
				for(int i = 0; i < spectrum.Length; i++)
					flux += (spectrum[i] - lastSpectrum[i]);
				
				spectralFlux.Add(flux);
			}

			Plot plot = new Plot("Spectral Flux", 1024, 512);
			plot.plot(spectralFlux, 1, Color.Red);
			new PlaybackVisualizer(plot, 1024, FILE);
			//new PlaybackVisualizer(plot, spectralFlux.Count, FILE);
		}
	}

}