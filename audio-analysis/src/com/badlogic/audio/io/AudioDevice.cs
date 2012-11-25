using System;

using NAudio;
using NAudio.Wave;
using AudioDevice = com.badlogic.audio.io.AudioDevice;

namespace com.badlogic.audio.io
{
	/**
	 * Class that allows directly passing PCM float mono
	 * data to the sound card for playback. The sampling
	 * rate of the PCM data must be 44100Hz.
	 * @author mzechner
	 * @author perivar@nerseth.com
	 */
	public class AudioDevice
	{
		/// the buffer size in samples
		private const int BUFFER_SIZE = 1024;

		// the sound line we write our samples to
		private WaveOut waveOut;

		// buffer for BUFFER_SIZE 32-bit samples
		private byte[] buffer = new byte[BUFFER_SIZE*4];

		// PlayBuffer
		private BufferedWaveProvider PlayBuffer;
		
		/**
		 * Constructor, initializes the audio system for
		 * 44100Hz 32-bit float stereo output.
		 */
		public AudioDevice()
		{
			// BufferedWaveProvider
			PlayBuffer = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
			PlayBuffer.BufferDuration = TimeSpan.FromMinutes(10);

			// waveOut
			/*
			 * This is the default and recommended approach if you are creating a WaveOut object
			 * from the GUI thread of a Windows Forms or WPF application.
			 * Whenever WaveOut wants more data it posts a message that is handled by the
			 * Windows message pump of an invisible new window.
			 * You get this callback model by default when you call the empty WaveOut constructor.
			 * However, it will not work on a background thread, since there is no message pump.
			 * */
			//waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
			waveOut = new WaveOut();
			//waveOut.Volume = 0.5f;
			waveOut.Init(PlayBuffer);
			waveOut.Play();
		}
		
		/**
		 * Writes the given samples to the audio device. The samples
		 * have to be sampled at 44100Hz stereo and have to be in
		 * the range [-1,1].
		 * @param samples The samples.
		 */
		public virtual void WriteSamples(float[] samples)
		{
			FillBuffer(samples);
			PlayBuffer.AddSamples(buffer, 0, buffer.Length);
		}

		/**
		 * Writes the given samples to the audio device.
		 * @param samples The samples.
		 */
		public virtual void WriteSamples(byte[] buffer)
		{
			PlayBuffer.AddSamples(buffer, 0, buffer.Length);
		}
		
		public void Dispose() {
			waveOut.Stop();
			waveOut = null;
		}
		
		private void FillBuffer(float[] samples)
		{
			for(int i = 0, j = 0; i < samples.Length; i++, j+=4)
			{
				byte[] array = BitConverter.GetBytes(samples[i]);
				buffer[j] = array[0];
				buffer[j+1] = array[1];
				buffer[j+2] = array[2];
				buffer[j+3] = array[3];
			}
		}
		
		public static void Main(string[] argv)
		{
			//ISampleProvider reader = new AudioFileReader(@"C:\Users\perivar.nerseth\Music\Sleep Away16.wav");
			//ISampleProvider reader = new AudioFileReader("samples/sample.wav");

			ISampleProvider reader = new AudioFileReader(@"C:\Users\perivar.nerseth\Music\Sleep Away32f.wav");
			AudioDevice device = new AudioDevice();
			
			float[] samples = new float[1024];
			while(reader.Read(samples, 0, samples.Length) > 0)
			{
				device.WriteSamples(samples);
			}
			
			System.Threading.Thread.Sleep(10000);
			device.Dispose();

			/*
			WaveFileReader wfr = new WaveFileReader("samples/sample.wav");
			WaveOut audioOutput = new WaveOut();
			WaveChannel32 wc = new WaveChannel32(wfr);
			wc.PadWithZeroes = false;
			audioOutput.Init(wc);
			audioOutput.PlaybackStopped += new EventHandler<StoppedEventArgs>(_waveOutDevice_PlaybackStopped);
			audioOutput.Play();
			
			while (audioOutput.PlaybackState != PlaybackState.Stopped) {
				System.Threading.Thread.Sleep(100);
			}
			 */
		}
		
		private static void _waveOutDevice_PlaybackStopped (object sender, EventArgs e) {
			System.Console.Out.WriteLine("Stopped!");
		}
	}
}