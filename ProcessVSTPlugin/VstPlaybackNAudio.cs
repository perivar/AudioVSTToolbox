using System;
using System.Threading;
//using System.Linq;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

using NAudio.Wave;

namespace ProcessVSTPlugin
{
	// Derive the NAudio stream class from WaveProvider32
	// instead of WaveStream if you don't want to deal with
	// converting the Vst buffer to the NAudio buffer.
	public class VstStreamNAudio : WaveProvider32
	{
		//int sample;
		//public float Frequency { get; set; }
		//public float Amplitude { get; set; }
		//public int BlockSize { get; set; }
		
		// Pointer to VstHost singleton
		public VstHost Host  { get; set; }
		
		public VstStreamNAudio(VstHost host)
		{
			Host = host;
			//Frequency = 1000;
			//Amplitude = 0.25f; // let's not hurt our ears
			//BlockSize = 1024;
			this.SetWaveFormat(host.SampleRate, host.Channels);
		}
		
		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			// 	Right I've got it! I changed the blockSize when initializing the output managers 
			// to match the buffer.Length attribute in the read function.
			// The sound is now continuous wahoo! Still not sure if this is the best way to do this but it works!
			int bufferLength = buffer.Length;							
			int loopSize = sampleCount / Host.Channels;
			
			// generate sine wave
			/*
			int sampleRate = WaveFormat.SampleRate;
			for (int n = 0; n < sampleCount; n++)
			{
				buffer[n+offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
				sample++;
				if (sample >= sampleRate) sample = 0;
			}
			return sampleCount;
			 */
			
			// CALL VST PROCESS HERE WITH BLOCK SIZE OF sampleCount
			int processedCount = Host.ProcessReplacing((uint)sampleCount);

			/*
			// Copying Vst buffer inside Audio buffer, no conversion needed for WaveProvider32
			// this does not work for stereo!!
			for (int i = 0; i < Host.vstOutputBuffers.Length; i++) // i < inputBuffers.Length &&
			{
				for (int j = 0; j < loopSize; j++)
				{
					float value = Host.vstOutputBuffers[i][j];
					buffer[i] = value;
				}
			}
			 */
			
			unsafe
			{
				float* tmpBufL = ((IDirectBufferAccess32)Host.vstOutputBuffers[0]).Buffer;
				float* tmpBufR = ((IDirectBufferAccess32)Host.vstOutputBuffers[1]).Buffer;
				int j = 0;
				for (int i = 0; i < loopSize; i++)
				{
					buffer[j] = *(tmpBufL + i);
					j++;
					buffer[j] = *(tmpBufR + i);
					j++;
				}
			}
			
			//return sampleCount;
			return processedCount;
		}
	}

	public class VstPlaybackNAudio : IDisposable
	{
		private IWavePlayer playbackDevice = null;
		//private IWaveProvider vstStream = null;
		private WaveProvider32 vstStream = null;

		// Pointer to VstHost singleton
		public VstHost Host  { get; set; }

		public VstPlaybackNAudio(VstHost host)
		{
			Host = host;
			Init();
		}

		public virtual IWavePlayer PlaybackDevice
		{
			get
			{
				return this.playbackDevice;
			}
		}
		
		public void Init()
		{
			vstStream = new VstStreamNAudio(Host);
			//vstStream = CreateInputStream("../../Sine-500hz-60sec.wav");
			
			// You need to make sure you are using function callbacks if you are trying to
			// play audio from a console app, since the default for WaveOut is to use window callbacks.
			playbackDevice = new WaveOut(WaveCallbackInfo.FunctionCallback());
			//playbackDevice = new WaveOut();
			playbackDevice.Init(vstStream);
			
			/*
			using (var playbackDevice =  new WaveOut(WaveCallbackInfo.FunctionCallback()))
			{
				playbackDevice.Init(vstStream);
				playbackDevice.Play();
				while (playbackDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(100);
				}
			}
			*/
			
		}

		public static WaveStream CreateInputStream(string name)
		{
			WaveChannel32 inputStream;
			if (name.EndsWith(".wav"))
			{
				WaveStream readerStream = new WaveFileReader(name);
				if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
					readerStream = new BlockAlignReductionStream(readerStream);
				}

				if (readerStream.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(readerStream.WaveFormat.SampleRate, 16, readerStream.WaveFormat.Channels);
					readerStream = new WaveFormatConversionStream(format, readerStream);
				}
				inputStream = new WaveChannel32(readerStream);
			}
			else
			{
				throw new InvalidOperationException("Invalid extension");
			}
			return inputStream;
		}
		
		public void Play()
		{
			if (playbackDevice != null && playbackDevice.PlaybackState != PlaybackState.Playing)
				playbackDevice.Play();
			
			// this seams not to be needed if this is run within a gui thread
			//while (playbackDevice.PlaybackState == PlaybackState.Playing)
			//{
			//   Thread.Sleep(100);
			//}
		}

		public void Stop()
		{
			if (playbackDevice != null && playbackDevice.PlaybackState != PlaybackState.Stopped)
				playbackDevice.Stop();
		}

		public void Dispose()
		{
			if (playbackDevice != null)
			{
				playbackDevice.Dispose();
				playbackDevice = null;
			}
		}
	}
}
