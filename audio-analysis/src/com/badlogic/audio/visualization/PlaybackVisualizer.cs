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
	// @author perivar@nerseth.com
	public class PlaybackVisualizer
	{
		Plot plot;
		int samplesPerPixel;
		ISampleProvider decoder;
		string FILE;
		AudioDevice device;
		Thread audioThread;
		
		private void PlotShown(object sender, EventArgs e) {
			audioThread = new Thread (new ThreadStart(FillAudioDeviceUsingElapsed));
			audioThread.IsBackground = true; // make sure the thread is closed on form close
			audioThread.Start();
		}
		
		private void PlotFormClosing(object sender, FormClosingEventArgs e)
		{
			// if the audio is playing when form closes, dispone the audio device
			if (audioThread.IsAlive) device.Dispose();
		}
		
		private void FillAudioDeviceUsingStopwatch() {
			this.device = new AudioDevice();
			float[] samples = new float[1024];

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while(decoder.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
				
				double elapsedTime = stopwatch.Elapsed.TotalSeconds;
				int position = (int)(elapsedTime * (44100 / samplesPerPixel) * 2 );
				plot.SetMarker(position, Color.White);
				System.Threading.Thread.Sleep(10); // this is needed or else swing has no chance repainting the plot!
			}
			stopwatch.Stop();
			device.Dispose();
		}
		
		private void FillAudioDeviceUsingElapsed() {
			this.device = new AudioDevice(FILE);
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
		// @param FILE The audio file.
		public PlaybackVisualizer(Plot plot, int samplesPerPixel, String FILE)
		{
			this.plot = plot;
			this.samplesPerPixel = samplesPerPixel;
			this.FILE = FILE;
			this.decoder = new AudioFileReader(FILE);
			
			plot.Shown += new EventHandler(PlotShown);
			plot.FormClosing += new FormClosingEventHandler(PlotFormClosing);
			Application.Run(plot);
		}
	}
}