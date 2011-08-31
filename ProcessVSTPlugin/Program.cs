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
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			/*
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
			
			try
			{
				String pluginPath = "../../TAL-Reverb-2.dll";
				//String pluginPath = "../../Fre(a)koscope.dll";
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
			*/
			
			String pluginPath = "../../TAL-Reverb-2.dll";
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
			
			VstPlaybackNAudio playback = new VstPlaybackNAudio(host);		
			playback.Play();
			Thread.Sleep(100);		

 			Console.WriteLine("Started");

 			while (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing)
			{
            	Thread.Sleep(10 * 1000);
 			}
            
            Console.WriteLine("Ending");
            playback.Stop();
            Console.WriteLine("Stopped");
            playback.Dispose();
		}
	}
}
