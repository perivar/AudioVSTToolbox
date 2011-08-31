using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Interop.Host;

using NAudio;
using NAudio.Wave;

// http://vstnet.codeplex.com/discussions/228692
// daniel_s
namespace ProcessVSTPlugin
{
	class AudioOutput : IDisposable
	{
		// NAudio Player
		private IWavePlayer playbackDevice = null;
		private VSTStream vstStream = null;
		private List<IVstPluginCommandStub> plugins;
		
		public AudioOutput(List<IVstPluginCommandStub> plugins)
		{
			this.plugins = plugins;
			Init();
			Play();
		}
		
		public void Init()
		{
			// 4410 samples == 100 milliseconds
			int sampleRate = 44100;			
			int blockSize = (int) (sampleRate * 0.15f); //6615;
			int channels = 2;			
			
			vstStream = new VSTStream(sampleRate, channels, blockSize, this.plugins); //blocksize 4410 samples gave stuttering? 6615 was perfect, 8820 was OK (small glitches)!
			playbackDevice = new WaveOut(WaveCallbackInfo.FunctionCallback());
			playbackDevice.Init(vstStream);
		}
		
		public void Play()
		{
			if (playbackDevice != null && playbackDevice.PlaybackState != PlaybackState.Playing)
				playbackDevice.Play();
			
			// this seams not to be needed if this is run within a gui thread
			while (playbackDevice.PlaybackState == PlaybackState.Playing)
			{
				Thread.Sleep(100);
			}
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

	class VSTStream : WaveProvider32
	{
		public long Length { get { throw new System.NotSupportedException(); } }
		public long Position { get { throw new System.NotSupportedException(); } set { throw new System.NotSupportedException(); } }
		
		public List<IVstPluginCommandStub> plugins;
		VstAudioBufferManager vstBufManIn, vstBufManOut;
		
		private VstAudioBuffer[] vstBufIn = null;
		private VstAudioBuffer[] vstBufOut = null;
		
		private byte[] naudioBuf;
		private int sampleRate, channels, blockSize;
		private WaveChannel32 wavStream;
		
		public VSTStream(int sampleRate, int channels, int blockSize, List<IVstPluginCommandStub> plugins)
			: base(sampleRate, channels)
		{
			this.plugins = plugins;
			this.sampleRate = sampleRate;
			this.channels = channels;
			this.blockSize = blockSize;
			
			plugins[0].SetBlockSize(blockSize);
			plugins[0].SetSampleRate((float)sampleRate);
			plugins[0].SetProcessPrecision(VstProcessPrecision.Process32);

			vstBufManIn = new VstAudioBufferManager(channels, blockSize);
			vstBufManOut = new VstAudioBufferManager(channels, blockSize);
			
			vstBufIn = vstBufManIn.ToArray();
			vstBufOut = vstBufManOut.ToArray();
			
			// 4 bytes per sample (32 bit)
			naudioBuf = new byte[blockSize * channels * 4];
			
			wavStream = new WaveChannel32(new WaveFileReader(@"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl\Intro.wav"));
			wavStream.Volume = 1f;
			
		}
		
		public static float[] ConvertByteToFloat(byte[] array) {
			float[] floatArr = new float[array.Length / 4];
			for (int i = 0; i < floatArr.Length; i++) {
				if (BitConverter.IsLittleEndian) {
					Array.Reverse(array, i * 4, 4);
				}
				floatArr[i] = BitConverter.ToSingle(array, i * 4);
			}
			return floatArr;
		}
		
		// buffer is call by reference!!!
		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			// 	Right I've got it! I changed the blockSize when initializing the output managers 
			// to match the buffer.Length attribute in the read function.
			// The sound is now continuous wahoo! Still not sure if this is the best way to do this but it works!
			int bufferLength = buffer.Length;	
			
			int sampleCountx4 = sampleCount * 4;
			int loopSize = sampleCount / channels;
			naudioBuf = new byte[blockSize * channels * 4];
			int bytesRead = wavStream.Read(naudioBuf, offset, sampleCountx4);
			
			/*
			// Convert byte array into float array and store in Vst Buffers
			// naudio reads an buffer of interlaced float's
			// must take every 4th byte and convert to float
			// Vst.Net audio buffer format (-1 to 1 floats).
			unsafe
			{
				fixed (byte* byteBuf = &naudioBuf[0])
				{
					float* floatBuf = (float*)byteBuf;
					int j = 0;
					for (int i = 0; i < loopSize; i++)
					{
						vstBufIn[0][i] = *(floatBuf + j);
						j++;
						vstBufIn[1][i] = *(floatBuf + j);
						j++;
					}
				}
			}
			*/
			
			byte[] b = naudioBuf;
			int nOut = 0;
			for (int nIn = 0; nIn < loopSize; nIn += 8) // nSamplesIn
			{
				vstBufIn[0][nOut] = System.BitConverter.ToSingle(naudioBuf, nIn);
				vstBufIn[1][nOut] = System.BitConverter.ToSingle(naudioBuf, nIn + 4);
				nOut++;
			}
			
			// The calls to Mainschanged and Start/Stop Process should be made only once, not for every cycle in the audio processing.
			// So it should look something like:
			// 
			// [plugin.Open()]
			// plugin.MainsChanged(true) // turn on 'power' on plugin.
			// plugin.StartProcess() // let the plugin know the audio engine has started
			// PluginContext.PluginCommandStub.ProcessEvents(ve); // process events (like VstMidiEvent)
			// 
			// while(audioEngineIsRunning)
			// {
			//     plugin.ProcessReplacing(inputBuffers, outputBuffers)  // letplugin process audio stream
			// }
			// 
			// plugin.StopProcess()
			// plugin.MainsChanged(false)
			// 
			// [plugin.Close()]
			
			//plugins[0].MainsChanged(true);
			//plugins[0].StartProcess();
			plugins[0].ProcessReplacing(vstBufIn, vstBufOut);
			//plugins[0].StopProcess();
			//plugins[0].MainsChanged(false);

			unsafe
			{
				float* tmpBufL = ((IDirectBufferAccess32)vstBufOut[0]).Buffer;
				float* tmpBufR = ((IDirectBufferAccess32)vstBufOut[1]).Buffer;
				int j = 0;
				for (int i = 0; i < loopSize; i++)
				{
					buffer[j] = *(tmpBufL + i);
					j++;
					buffer[j] = *(tmpBufR + i);
					j++;
				}
			}
			
			return sampleCount;
		}
	}
}