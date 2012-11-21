using System;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

using NAudio;
using NAudio.Wave;

using AudioDevice = com.badlogic.audio.io.AudioDevice;

namespace com.badlogic.audio.visualization
{
	// * Takes a plot and a decoder and plays back the audio
	// * form the decoder as well as setting the marker in the
	// * plot accordingly.
	// * 
	// * @author mzechner
	public class PlaybackVisualizer
	{

		//	 * Consturctor, plays back the audio form the decoder and
		//	 * sets the marker of the plot accordingly. This will return
		//	 * when the playback is done.
		//	 * 
		//	 * @param plot The plot.
		//	 * @param samplesPerPixel the numbe of samples per pixel.
		//	 * @param decoder The decoder.
		public PlaybackVisualizer(Plot plot, int samplesPerPixel, ISampleProvider decoder)
		{
			AudioDevice device = new AudioDevice();
			float[] samples = new float[1024];

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while(decoder.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);

				float elapsedTime = stopwatch.Elapsed.Milliseconds;
				int position = (int)(elapsedTime * (44100/samplesPerPixel));
				plot.SetMarker(position, Color.White);
				System.Threading.Thread.Sleep(20); // this is needed or else swing has no chance repainting the plot!
			}
		}
	}

}