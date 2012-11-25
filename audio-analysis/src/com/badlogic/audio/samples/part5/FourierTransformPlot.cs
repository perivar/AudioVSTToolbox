using System;
using System.Drawing;
using System.Windows.Forms;

using FFT = com.badlogic.audio.analysis.FFT;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part5
{
	// * Simple example that generates a 1024 samples sine wave at 440Hz
	// * and plots the resulting spectrum.
	// * 
	// * @author mzechner
	public class FourierTransformPlot
	{
		public static void Main(string[] argv)
		{
			const float frequency = 440; // Note A
			float increment = (float)(2*Math.PI) * frequency / 44100;
			float angle = 0;
			float[] samples = new float[1024];
			FFT fft = new FFT(1024, 44100);

			for(int i = 0; i < samples.Length; i++)
			{
				samples[i] = (float)Math.Sin(angle);
				angle += increment;
			}

			fft.Forward(samples);

			Plot plot = new Plot("Note A Spectrum", 512, 512);
			plot.plot(fft.GetSpectrum(), 1, Color.Red);
			Application.Run(plot);
		}
	}
}