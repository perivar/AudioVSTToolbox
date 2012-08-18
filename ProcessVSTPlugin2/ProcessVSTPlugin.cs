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
		static int foundSilenceCounter = 0;
		
		public static bool ProcessOffline(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null) {

			HostCommandStub hcs = new HostCommandStub();
			hcs.Directory = System.IO.Path.GetDirectoryName(pluginPath);
			VST vst = new VST();
			
			try
			{
				vst.pluginContext = VstPluginContext.Create(pluginPath, hcs);
				
				if (vst.pluginContext == null) {
					Console.Out.WriteLine("Could not open up the plugin specified by {0}!", pluginPath);
					return false;
				}
				
				// plugin does not support processing audio
				if ((vst.pluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
				{
					Console.Out.WriteLine("This plugin does not process any audio.");
					return false;
				}
				
				// check if the plugin supports offline proccesing
				if(vst.pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)) != VstCanDoResult.Yes) {
					Console.Out.WriteLine("This plugin does not support offline processing.");
					return false;
				}
				
				// add custom data to the context
				vst.pluginContext.Set("PluginPath", pluginPath);
				vst.pluginContext.Set("HostCmdStub", hcs);

				// actually open the plugin itself
				vst.pluginContext.PluginCommandStub.Open();
				
			} catch (Exception ex) {
				Console.Out.WriteLine("Could not load VST! ({0})", ex.Message);
				return false;
			}

			if (File.Exists(fxpFilePath)) {
				vst.LoadFXP(fxpFilePath);
			} else {
				Console.Out.WriteLine("Could not find preset file (fxp|fxb) ({0})", fxpFilePath);
			}
			
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
		
		public static bool ProcessRealTime(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null)
		{
			UtilityAudio.OpenAudio(AudioLibrary.NAudio);
			VST vst = UtilityAudio.LoadVST(pluginPath);

			if (vst == null || vst.pluginContext == null) {
				return false;
			}
			
			// plugin does not support processing audio
			if ((vst.pluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				Console.Out.WriteLine("This plugin does not process any audio.");
				return false;
			}
			
			// check if the plugin supports real time proccesing
			if(vst.pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)) == VstCanDoResult.Yes) {
				Console.Out.WriteLine("This plugin does not support realtime processing.");
				return false;
			}			

			if (File.Exists(fxpFilePath)) {
				vst.LoadFXP(fxpFilePath);
			} else {
				Console.Out.WriteLine("Could not find preset file (fxp|fxb) ({0})", fxpFilePath);
			}
			
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
				return ProcessRealTime(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath);
			} else {
				return ProcessOffline(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath);
			}
		}
		
		static void vst_StreamCall(object sender, VSTStreamEventArgs e)
		{
			// stop the audio if nothing is playing any longer
			// TODO: but make sure we have at least waited 5 sec
			// in case of silence in the beginning
			float almostZero = 0.0000001f;
			if (e.MaxL <  almostZero && e.MaxR < almostZero) {
				Console.Out.Write("-");
				
				// don't stop until we have x consequtive silence calls after each other
				if (foundSilenceCounter >= 5) {
					UtilityAudio.StopAudio(); // when playing sound
					foundSilence = true; // when doing offline processing
				} else {
					foundSilenceCounter++;
				}
			} else {
				foundSilenceCounter = 0;
				Console.Write(".");
			}
		}
	}
}