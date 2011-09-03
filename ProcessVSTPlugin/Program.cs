using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

using NAudio.Wave;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

namespace ProcessVSTPlugin
{
	static class Program
	{
		
		static void StartGUI() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void StartAudioOutput(string pluginPath) {
			try
			{
				HostCommandStub hostCmdStub = new HostCommandStub();
				VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);
				
				// add custom data to the context
				ctx.Set("PluginPath", pluginPath);
				ctx.Set("HostCmdStub", hostCmdStub);
				
				// actually open the plugin itself
				ctx.PluginCommandStub.Open();
				
				AudioOutput audioOut = new AudioOutput(new List<IVstPluginCommandStub>() {ctx.PluginCommandStub});
				Thread.Sleep(100);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		static void StartVstHost(string pluginPath, bool doPlay) {

			VstHost host = VstHost.Instance;
			host.OpenPlugin(pluginPath);
			host.InputWave = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl\Intro.wav";
			// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
			// tblock = 0.15 makes blocksize = 6615.
			int sampleRate = 44100;
			int blockSize = (int) (sampleRate * 0.15f); //6615;
			int channels = 2;
			host.Init(blockSize, sampleRate, channels);
			System.Diagnostics.Debug.WriteLine(host.getPluginInfo());

			if (doPlay) {
				VstPlaybackNAudio playback = new VstPlaybackNAudio(host);
				playback.Play();
				
				Console.WriteLine("Started");
				
				// make sure to play while the stream is playing
				while (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(100);
				}
				
				Console.WriteLine("Ending");
				playback.Stop();
				Console.WriteLine("Stopped");
				playback.Dispose();
			}
			
			host.LoadFXP(@"..\..\Scoring Stages (Orchestral Studios)_Todd-AO - California US_st to st wide mics at 18m90.fxp");
			VstFileWriter fileWriter = new VstFileWriter(host);
			fileWriter.CreateWaveFile("test.wav");
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			StartGUI();

			String pluginPath = "../../TAL-Reverb-2.dll";
			//String pluginPath = "../../Fre(a)koscope.dll";
			//String pluginPath = "../../SIR2.dll";
			
			//StartAudioOutput(pluginPath);
			//StartVstHost(pluginPath, false);
		}
	}
}
