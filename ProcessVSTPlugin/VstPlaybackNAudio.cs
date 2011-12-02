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
		public long Length { get { throw new System.NotSupportedException(); } }
		public long Position { get { throw new System.NotSupportedException(); } set { throw new System.NotSupportedException(); } }
		
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
			
			// CALL VST PROCESS HERE WITH BLOCK SIZE OF sampleCount
			int processedCount = Host.ProcessReplacing((uint)sampleCount);
			
			// read from the vstOutputBuffers
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
			
			return processedCount;
		}
	}

	public class VstPlaybackNAudio : IDisposable
	{
		private IWavePlayer playbackDevice = null;
		private IWaveProvider vstStream = null;

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
				playbackDevice.Pause();
				playbackDevice.Stop();
				playbackDevice.Dispose();
				playbackDevice = null;
			}
		}
	}
	
	public class VstFileWriter
	{
		private IWaveProvider vstStream = null;
		private int blocksize;
		
		// Pointer to VstHost singleton
		public VstHost Host  { get; set; }

		public VstFileWriter(VstHost host)
		{
			Host = host;
			this.blocksize = 32768; // 16384 8192 4096
			vstStream = new VstStreamNAudio(Host);
		}

		public virtual int BlockSize
		{
			set
			{
				this.blocksize = value;
			}	
			get
			{
				return this.blocksize;
			}
		}

		public virtual IWaveProvider VstStream
		{
			get
			{
				return this.vstStream;
			}
		}
		
		public void CreateWaveFile(string outputFile) {
			//using (WaveStream pcmStream = new WaveProviderToWaveStream(this.VstStream))
			//{
			//	WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
			//}
	
 			Console.WriteLine("Saving wav file: " + outputFile);			
			using (WaveFileWriter writer = new WaveFileWriter(outputFile, this.VstStream.WaveFormat))
			{
				byte[] buf = new byte[this.blocksize];
				for (; ;)
				{
					int cnt = this.VstStream.Read(buf, 0, buf.Length);
					if (cnt == 0) break;
					writer.WriteData(buf, 0, cnt);
				}
			}
		}
	}
	
	public class WaveProviderToWaveStream : WaveStream
	{
		private readonly IWaveProvider source;
		private long position;

		public WaveProviderToWaveStream(IWaveProvider source)
		{
			this.source = source;
		}

		public override WaveFormat WaveFormat
		{
			get { return source.WaveFormat;  }
		}

		/// <summary>
		/// Don't know the real length of the source, just return a big number
		/// </summary>
		public override long Length
		{
			get { return Int32.MaxValue; }
		}

		public override long Position
		{
			get
			{
				// we'll just return the number of bytes read so far
				return position;
			}
			set
			{
				// can't set position on the source
				// n.b. could alternatively ignore this
				throw new NotImplementedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = source.Read(buffer, offset, count);
			position += read;
			return read;
		}
	}
	
}
