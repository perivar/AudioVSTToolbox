using System.Collections.Generic;
using System.Drawing;

using NAudio.Wave;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part3
{
	// * A simple example that shows how to use the {@link Plot} class.
	// * Note that the plots will not be entirely synchronous to the
	// * music playback. This is just an example, you should not do
	// * real-time plotting with the Plot class it is just not made for
	// * this.
	// * @author mzechner
	public class PlotExample
	{
		public static void Main(string[] argv)
		{
			ISampleProvider reader = new AudioFileReader("samples/sample.wav");
			List<float> allSamples = new List<float>();
			float[] samples = new float[1024];

			while(reader.Read(samples, 0, samples.Length) > 0)
			{
				for(int i = 0; i < samples.Length; i++)
					allSamples.Add(samples[i]);
			}

			samples = new float[allSamples.Count];
			for(int i = 0; i < samples.Length; i++)
				samples[i] = allSamples[i];

			Plot plot = new Plot("Wave Plot", 512, 512);
			plot.plot(samples, 44100 / 1000, Color.Red);
		}
	}
}