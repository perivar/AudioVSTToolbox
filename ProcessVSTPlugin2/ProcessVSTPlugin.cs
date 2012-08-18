using System;
using System.Threading;
using System.IO;

using NAudio.Wave;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using CommonUtils.VSTPlugin;
using CommonUtils.Audio;

namespace ProcessVSTPlugin2
{
	/// <summary>
	/// The class where you can call a method to process an audio file with a
	/// vst plugin, with sound or without sound playing
	/// Per Ivar Nerseth, 2011 - 2012
	/// perivar@nerseth.com
	/// </summary>
	public class ProcessVSTPlugin
	{
		static bool foundSilence = false;
		
		public static bool ProcessOffline(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null) {

			HostCommandStub hcs = new HostCommandStub();
			hcs.Directory = System.IO.Path.GetDirectoryName(pluginPath);
			VST vst = new VST();
			
			try
			{
				vst.pluginContext = VstPluginContext.Create(pluginPath, hcs);
				
				// plugin does not support processing audio
				if ((vst.pluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
				{
					Console.Out.WriteLine("This plugin does not process any audio.");
					return false;
				}
				
				// add custom data to the context
				vst.pluginContext.Set("PluginPath", pluginPath);
				vst.pluginContext.Set("HostCmdStub", hcs);

				// actually open the plugin itself
				vst.pluginContext.PluginCommandStub.Open();
				
			} catch (Exception ex) {
				Console.Out.WriteLine(ex.Message);
			}

			vst.LoadFXP(fxpFilePath);
			
			using (VSTStream vstStream = new VSTStream()) {
				vstStream.ProcessCalled += new EventHandler<VSTStreamEventArgs>(vst.Stream_ProcessCalled);
				vstStream.pluginContext = vst.pluginContext;
				vstStream.SetWaveFormat(44100, 2);
				
				vst.StreamCall += new EventHandler<VSTStreamEventArgs>(vst_StreamCall);
				
				vstStream.InputWave = waveInputFilePath;
				
				byte[] buffer = new byte[512*4];
				using (MemoryStream ms = new MemoryStream())
				{
					while (!foundSilence)
					{
						int read = vstStream.Read(buffer, 0, buffer.Length);
						if (read <= 0) {
							break;
						}
						ms.Write(buffer, 0, read);
					}

					// save
					using (WaveStream ws = new RawSourceWaveStream(ms, vstStream.WaveFormat))
					{
						ws.Position = 0;
						WaveFileWriter.CreateWaveFile(waveOutputFilePath, ws);
					}
				}
			}
			
			return true;
		}
		
		public static bool ProcessOnline(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null)
		{
			UtilityAudio.OpenAudio(AudioLibrary.NAudio);
			VST vst = UtilityAudio.LoadVST(pluginPath);

			// plugin does not support processing audio
			if ((vst.pluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				Console.Out.WriteLine("This plugin does not process any audio.");
				return false;
			}

			vst.LoadFXP(fxpFilePath);
			
			vst.StreamCall += new EventHandler<VSTStreamEventArgs>(vst_StreamCall);
			UtilityAudio.StartAudio();
			
			UtilityAudio.vstStream.InputWave = waveInputFilePath;
			UtilityAudio.SaveStream(waveOutputFilePath);
			UtilityAudio.StartStreamingToDisk();
			
			// just wait while the stream is playing
			while (UtilityAudio.playbackDevice.PlaybackState == PlaybackState.Playing)
			{
				Thread.Sleep(50);
			}

			UtilityAudio.StopStreamingToDisk();
			vst.Dispose();
			
			return true;
		}
		
		public static bool Process(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null, bool doPlay=false)
		{
			if (doPlay) {
				return ProcessOnline(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath);
			} else {
				return ProcessOffline(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath);
			}
		}
		
		static void vst_StreamCall(object sender, VSTStreamEventArgs e)
		{
			// stop the audio if nothing is playing any longer
			// TODO: but make sure we have at least waited 5 sec
			// in case of silence in the beginning
			float almostZero = 0.000001f;
			if (e.MaxL <  almostZero && e.MaxR < almostZero) {
				Console.Out.Write("-");
				
				UtilityAudio.StopAudio(); // when playing sound
				foundSilence = true; // when doing offline processing
			} else {
				Console.Write(".");
			}
		}
	}
}