using AudioDevice = com.badlogic.audio.io.AudioDevice;
using NAudio.Wave;

namespace com.badlogic.audio.samples.part2
{

	// * A simple example how to read in a Wave file via
	// * a {@link WaveDecoder} and output its contents to
	// * an {@link AudioDevice}.
	// * @author mzechner
	public class WaveOutput
	{
		public static void Main(string[] argv)
		{
			AudioDevice device = new AudioDevice();
			ISampleProvider reader = new AudioFileReader("samples/sample.wav");
			
			float[] samples = new float[1024];
			while(reader.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
			}
			
			System.Threading.Thread.Sleep(10000);
			device.Dispose();
		}
	}

}