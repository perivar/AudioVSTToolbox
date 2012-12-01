using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

using NAudio;
using NAudio.Wave;

using AudioDevice = com.badlogic.audio.io.AudioDevice;

namespace com.badlogic.audio.visualization
{
	// Takes a plot and a decoder and plays back the audio
	// form the decoder as well as setting the marker in the
	// plot accordingly.
	// @author mzechner
	public class PlaybackVisualizer
	{
		Plot plot;
		int samplesPerPixel;
		ISampleProvider decoder;
		string FILE;
		
		private void PlotShown(object sender, EventArgs e) {
			Thread t = new Thread (new ThreadStart(Filler2));
			t.Start();
		}
		
		private void Filler() {
			AudioDevice device = new AudioDevice();
			float[] samples = new float[1024];

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while(decoder.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
				
				double elapsedTime = stopwatch.Elapsed.TotalSeconds;
				int position = (int)(elapsedTime * (44100/samplesPerPixel) * 2 );
				plot.SetMarker(position, Color.White);
				System.Threading.Thread.Sleep(10); // this is needed or else swing has no chance repainting the plot!
			}
			device.WavePlayer.Stop();
			device.Dispose();
		}
		
		private void Filler2() {
			AudioDevice device = new AudioDevice(FILE);
			float[] samples = new float[1024];

			while(device.SampleChannel.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
				
				double elapsedTime = device.Elapsed.TotalSeconds;
				int position = (int)(elapsedTime * (44100/samplesPerPixel) * 2 );
				plot.SetMarker(position, Color.White);
				System.Threading.Thread.Sleep(10); // this is needed or else swing has no chance repainting the plot!
			}
			device.Dispose();
		}
		
		// Constructor, plays back the audio form the decoder and
		// sets the marker of the plot accordingly. This will return
		// when the playback is done.
		// @param plot The plot.
		// @param samplesPerPixel the numbe of samples per pixel.
		// @param decoder The decoder.
		public PlaybackVisualizer(Plot plot, int samplesPerPixel, ISampleProvider decoder)
		{
			this.plot = plot;
			this.samplesPerPixel = samplesPerPixel;
			this.decoder = decoder;
			
			plot.Shown += new EventHandler(PlotShown);
			Application.Run(plot);
		}
		
		// Constructor, plays back the audio form the decoder and
		// sets the marker of the plot accordingly. This will return
		// when the playback is done.
		// @param plot The plot.
		// @param samplesPerPixel the numbe of samples per pixel.
		// @param FILE The audio file.
		public PlaybackVisualizer(Plot plot, int samplesPerPixel, String FILE)
		{
			this.plot = plot;
			this.samplesPerPixel = samplesPerPixel;
			this.FILE = FILE;
			
			plot.Shown += new EventHandler(PlotShown);
			Application.Run(plot);
		}
	}

}