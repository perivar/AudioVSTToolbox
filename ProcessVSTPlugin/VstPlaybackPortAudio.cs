// http://vstnet.codeplex.com/discussions/246206
// YuryK

using System;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

using PortAudioSharp;

namespace ProcessVSTPlugin
{
	// Standard parameters for audio processing
	//[type: System.CLSCompliant(false)]
	static class AudioParameters
	{
		public static int NbChannels = 2;
		public static int SampleRate = 44100;
		public static uint BlockSize = 512;
		public static float globalVolume = 1f;
	}

	// Class for VstPlayblack using PortAudio
	//[type: System.CLSCompliant(false)]
	public class VstPlaybackPortAudio : IDisposable
	{
		// Pointer to VstHost singleton
		private VstHost host;

		// Port audio instance
		private PortAudioSharp.Audio audio;

		// Audio buffer pointer
		private unsafe float* audioBuffer;

		// Index for number of audio output buffer samples
		private uint i;

		// Index for number of vst output buffer samples per channel
		private int j;

		// Index for number of channels
		private int k;
		
		public VstPlaybackPortAudio(VstHost host)
		{
			this.host = host;
			Init(AudioParameters.SampleRate);
			Play();
		}
		
		private PortAudio.PaStreamCallbackResult AudioCallback(IntPtr audioInputBuffer, IntPtr audioOutputBuffer, uint blockSize, ref PortAudio.PaStreamCallbackTimeInfo timeInfo, PortAudio.PaStreamCallbackFlags statusFlags, IntPtr userData)
		{
			// If host and VstBuffer are initialized
			if (host != null && host.vstOutputBuffers != null)
			{
				// This function fills vstOutputBuffers with audio processed by a plugin
				host.ProcessReplacing(blockSize);
				
				// Init loop indexes
				i = 0;
				j = 0;

				unsafe
				{
					// Fill audio buffer for all channels, multiply samples by volume range [0, 1]
					audioBuffer = (float*)audioOutputBuffer.ToPointer();

					while (i < blockSize * AudioParameters.NbChannels)
					{
						for (k = 0; k < AudioParameters.NbChannels; k++)
							audioBuffer[i++] = host.vstOutputBuffers[k][j] * AudioParameters.globalVolume;

						j++;
					}
				}
			}
			else if (host == null)
			{
				// Check if pointer to VstHost is available
				//host = VstHostActions.Obj.Host;
			}
			
			return PortAudio.PaStreamCallbackResult.paContinue;
		}

		public void Init(int sampleRate)
		{
			try
			{
				audio = new PortAudioSharp.Audio(AudioParameters.NbChannels,
				                                 AudioParameters.SampleRate,
				                                 AudioParameters.BlockSize,
				                                 new PortAudio.PaStreamCallbackDelegate(AudioCallback));
			}
			catch (Exception e)
			{
				audio = null;
				System.Diagnostics.Debug.WriteLine(e);
			}
			finally
			{
				if (audio != null) audio.Dispose();
			}
		}

		public void Play()
		{
			audio.Start();
		}

		public void Stop()
		{
			audio.Stop();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}