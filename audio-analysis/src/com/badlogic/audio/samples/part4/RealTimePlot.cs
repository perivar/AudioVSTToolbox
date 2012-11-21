using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using NAudio;
using NAudio.Wave;

using System.Collections.Generic;
using AudioDevice = com.badlogic.audio.io.AudioDevice;
using Plot = com.badlogic.audio.visualization.Plot;

namespace com.badlogic.audio.samples.part4
{

	// * A simple example on how to do real-time plotting. First all samples
	// * from an mp3 file are read in and plotted, 1024 samples per pixel. Next
	// * we open a new MP3Decoder and an AudioDevice and play back the music. While
	// * doing this we also set a marker in the plot that shows us the current position
	// * of the music playback. The marker position is calculated in pixels by
	// * measuring the elapsed time between the start of the playback and the
	// * current time. The elapsed time is then multiplied by the frequency divided
	// * by the sample window size (1024 samples in this case). This gives us the
	// * x-coordinate of the marker in the plot. After writting a sample window
	// * to the audio device and setting the marker we sleep for 20ms to give
	// * the Swing GUI thread time to repaint the plot with the updated marker
	// * position.
	// * 
	// * @author mzechner
	public class RealTimePlot
	{
		private const int SAMPLE_WINDOW_SIZE = 1024;
		private const string FILE = "samples/sample.mp3";

		public static void Main(string[] argv)
		{
			float[] samples = ReadInAllSamples(FILE);

			Plot plot = new Plot("Wave Plot", 1024, 512);
			plot.plot(samples, SAMPLE_WINDOW_SIZE, Color.Red);

			ISampleProvider reader = new AudioFileReader(FILE);
			
			AudioDevice device = new AudioDevice();
			samples = new float[SAMPLE_WINDOW_SIZE];

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while(reader.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);

				float elapsedTime = stopwatch.Elapsed.Milliseconds;
				int position = (int)(elapsedTime * (44100/SAMPLE_WINDOW_SIZE));
				plot.SetMarker(position, Color.White);
				System.Threading.Thread.Sleep(15); // this is needed or else swing has no chance repainting the plot!
			}
		}

		public static float[] ReadInAllSamples(string file)
		{
			ISampleProvider reader = new AudioFileReader(file);
			
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

			return samples;
		}
	}

}