using System;
using AudioDevice = com.badlogic.audio.io.AudioDevice;

namespace com.badlogic.audio.samples.part2
{

	// * A simple generator that outputs a sinewave at some
	// * frequency (here 440Hz = Note A) in mono to an {@link AudioDevice}.
	// * 
	// * @author mzechner
	public class NoteGenerator
	{
		public static void Main(string[] argv)
		{
			const float frequency = 880; // 440Hz for note A
			float increment = (float)(2*Math.PI) * frequency / 44100; // angular increment for each sample
			float angle = 0;
			AudioDevice device = new AudioDevice();
			float[] samples = new float[1024];

			while(true)
			{
				for(int i = 0; i < samples.Length; i++)
				{
					samples[i] = (float)Math.Sin(angle);
					angle += increment;
				}

				device.WriteSamples(samples);
			}
		}
	}
}