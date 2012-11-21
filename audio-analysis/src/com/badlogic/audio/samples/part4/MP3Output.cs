using AudioDevice = com.badlogic.audio.io.AudioDevice;
using NAudio.Wave;

namespace com.badlogic.audio.samples.part4
{
	///
	// * Simple example that shows how to decode an mp3 file.
	// * 
	// * @author mzechner
	// *
	// 
	public class MP3Output
	{
		public static void Main(string[] argv)
		{
			AudioDevice device = new AudioDevice();
			ISampleProvider reader = new AudioFileReader("samples/mozart.mp3");
			
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