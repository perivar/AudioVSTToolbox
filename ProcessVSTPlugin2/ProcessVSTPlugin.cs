using System;
using System.Threading;
using System.IO;
using NAudio.Wave;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using CommonUtils.VSTPlugin;

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
		private bool stoppedPlaying = false;
		private bool audioBufferEmpty = false;
		
		private VST vst = null;
		private VSTStream vstStream = null;
		
		private string _pluginPath = null;
		private int _sampleRate = 0;
		private int _channels = 0;
		
		public bool ProcessOffline(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null, float volume=1.0f) {

			var wavFileReader = new WaveFileReader(waveInputFilePath);

			// reuse if batch processing
			bool doUpdateVstPlugin = false;
			if (_pluginPath != null) {
				if (_pluginPath.Equals(pluginPath)) {
					// plugin has not changed
				} else {
					// plugin has changed!
					doUpdateVstPlugin = true;
				}
			} else {
				_pluginPath = pluginPath;
				doUpdateVstPlugin = true;
			}

			if (doUpdateVstPlugin) {
				var hcs = new HostCommandStub();
				hcs.Directory = System.IO.Path.GetDirectoryName(pluginPath);
				vst = new VST();
				
				try
				{
					vst.PluginContext = VstPluginContext.Create(pluginPath, hcs);
					
					if (vst.PluginContext == null) {
						Console.Out.WriteLine("Could not open up the plugin specified by {0}!", pluginPath);
						return false;
					}
					
					// plugin does not support processing audio
					if ((vst.PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
					{
						Console.Out.WriteLine("This plugin does not process any audio.");
						return false;
					}
					
					// check if the plugin supports offline proccesing
					if(vst.PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)) == VstCanDoResult.No) {
						Console.Out.WriteLine("This plugin does not support offline processing.");
						Console.Out.WriteLine("Try use realtime (-play) instead!");
						return false;
					}
					
					// add custom data to the context
					vst.PluginContext.Set("PluginPath", pluginPath);
					vst.PluginContext.Set("HostCmdStub", hcs);

					// actually open the plugin itself
					vst.PluginContext.PluginCommandStub.Open();
					
					Console.Out.WriteLine("Enabling the audio output on the VST!");
					vst.PluginContext.PluginCommandStub.MainsChanged(true);
					
					// setup the VSTStream
					vstStream = new VSTStream();
					vstStream.ProcessCalled += new EventHandler<VSTStreamEventArgs>(vst_ProcessCalled);
					vstStream.PlayingStarted += new EventHandler(vst_PlayingStarted);
					vstStream.PlayingStopped += new EventHandler(vst_PlayingStopped);
					vstStream.pluginContext = vst.PluginContext;
					
					vstStream.SetWaveFormat(wavFileReader.WaveFormat.SampleRate, wavFileReader.WaveFormat.Channels);
				} catch (Exception ex) {
					Console.Out.WriteLine("Could not load VST! ({0})", ex.Message);
					return false;
				}
			}

			if (File.Exists(fxpFilePath)) {
				vst.LoadFXP(fxpFilePath);
			} else {
				Console.Out.WriteLine("Could not find preset file (fxp|fxb) ({0})", fxpFilePath);
			}
			
			// each float is 4 bytes
			var buffer = new byte[512*4];
			using (var ms = new MemoryStream())
			{
				vstStream.SetInputWave(waveInputFilePath, volume);
				vstStream.DoProcess = true;
				
				// wait a little while
				Thread.Sleep(1000);
				
				// keep on reading until it stops playing.
				while (!stoppedPlaying)
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
			
			// reset the input wave file
			vstStream.DoProcess = false;
			vstStream.DisposeInputWave();

			// reset if calling this method multiple times
			stoppedPlaying = false;
			return true;
		}
		
		public bool ProcessRealTime(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null, float volume=1.0f)
		{
			var wavFileReader = new WaveFileReader(waveInputFilePath);

			// reuse if batch processing
			bool doUpdateVstPlugin = false;
			bool doUpdateSampleRate = false;
			bool doUpdateNoChannels = false;
			if (_pluginPath != null) {
				if (_pluginPath.Equals(pluginPath)) {
					// plugin has not changed
				} else {
					// plugin has changed!
					doUpdateVstPlugin = true;
				}
			} else {
				_pluginPath = pluginPath;
				doUpdateVstPlugin = true;
			}
			
			if (_sampleRate != 0) {
				if (_sampleRate == wavFileReader.WaveFormat.SampleRate) {
					// same sample rate
				} else {
					// sample rate has changed!
					doUpdateSampleRate = true;
				}
			} else {
				_sampleRate = wavFileReader.WaveFormat.SampleRate;
				doUpdateSampleRate = true;
			}

			if (_channels != 0) {
				if (_channels == wavFileReader.WaveFormat.Channels) {
					// same number of channels
				} else {
					// number of channels has changed!
					doUpdateNoChannels = true;
				}
			} else {
				_channels = wavFileReader.WaveFormat.Channels;
				doUpdateNoChannels = true;
			}

			if (doUpdateNoChannels || doUpdateSampleRate) {
				Console.Out.WriteLine("Opening Audio driver using samplerate {0} and {1} channels.", _sampleRate, _channels);
				UtilityAudio.OpenAudio(AudioLibrary.NAudio, _sampleRate, _channels);
			}

			if (doUpdateVstPlugin || doUpdateNoChannels || doUpdateSampleRate) {
				Console.Out.WriteLine("Loading Vstplugin using samplerate {0} and {1} channels.", _sampleRate, _channels);
				vst = UtilityAudio.LoadVST(_pluginPath, _sampleRate, _channels);
				UtilityAudio.VstStream.ProcessCalled += new EventHandler<VSTStreamEventArgs>(vst_ProcessCalled);
				UtilityAudio.VstStream.PlayingStarted += new EventHandler(vst_PlayingStarted);
				UtilityAudio.VstStream.PlayingStopped += new EventHandler(vst_PlayingStopped);
				
				// plugin does not support processing audio
				if ((vst.PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
				{
					Console.Out.WriteLine("This plugin does not process any audio.");
					return false;
				}
				
				// check if the plugin supports real time proccesing
				if(vst.PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)) == VstCanDoResult.Yes) {
					Console.Out.WriteLine("This plugin does not support realtime processing.");
					return false;
				}
			}

			if (File.Exists(fxpFilePath)) {
				Console.Out.WriteLine("Loading preset file {0}.", fxpFilePath);
				vst.LoadFXP(fxpFilePath);
			} else {
				Console.Out.WriteLine("Could not find preset file (fxp|fxb) ({0})", fxpFilePath);
			}

			if (UtilityAudio.PlaybackDevice.PlaybackState != PlaybackState.Playing) {
				Console.Out.WriteLine("Starting audio playback engine.");
				UtilityAudio.StartAudio();
			}
			
			Console.Out.WriteLine("Setting input wave {0}.", waveInputFilePath);
			UtilityAudio.VstStream.SetInputWave(waveInputFilePath, volume);

			Console.Out.WriteLine("Setting output wave {0}.", waveOutputFilePath);
			UtilityAudio.SaveStream(waveOutputFilePath);
			
			UtilityAudio.VstStream.DoProcess = true;
			
			// just wait while the stream is playing
			// the events will trigger and set the stoppedPlaying flag
			while (!stoppedPlaying)
			{
				Thread.Sleep(50);
			}

			// reset if calling this method multiple times
			stoppedPlaying = false;
			return true;
		}
		
		public bool Process(String waveInputFilePath, String waveOutputFilePath, String pluginPath, String fxpFilePath=null, float volume=1.0f, bool doPlay=false)
		{
			if (doPlay) {
				return ProcessRealTime(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath, volume);
			} else {
				return ProcessOffline(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath, volume);
			}
		}

		private void vst_PlayingStarted(object sender, EventArgs e) {
			if (UtilityAudio.PlaybackDevice != null) {
				UtilityAudio.StartStreamingToDisk();
				Console.Out.WriteLine("Started streaming to disk ...");
			}
			Console.Out.WriteLine("Vst Plugin Started playing ...");
			stoppedPlaying = false;
		}

		private void vst_PlayingStopped(object sender, EventArgs e) {
			if (UtilityAudio.PlaybackDevice != null) {
				UtilityAudio.VstStream.DoProcess = false;
				UtilityAudio.StopStreamingToDisk();
				Console.Out.WriteLine("Stopped streaming to disk ...");
			}
			Console.Out.WriteLine("Vst Plugin Stopped playing ...");
			stoppedPlaying = true;
		}
		
		private void vst_ProcessCalled(object sender, VSTStreamEventArgs e)
		{
			if (e.MaxL == 0 && e.MaxR == 0) {
				audioBufferEmpty = true;
			} else {
				audioBufferEmpty = false;
			}
		}
		
		public void Dispose() {
			if (UtilityAudio.PlaybackDevice != null) UtilityAudio.Dispose();
		}
	}
}