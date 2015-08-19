using System;
using System.Windows.Forms;
using System.Threading;

using NAudio.Wave;

using CommonUtils;
using CommonUtils.VST;

namespace ProcessVSTPlugin
{
	static class Program
	{
		static string _version = "1.3.1";
		
		static void StartGUI() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void StartVstHost(string pluginPath, string waveInputFilePath, string fxpFilePath, string waveOutputFilePath, bool doPlay) {

			VstHost host = VstHost.Instance;
			var hcs = new HostCommandStub();
			host.OpenPlugin(pluginPath, hcs);
			host.InputWave = waveInputFilePath;
			// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
			// tblock = 0.15 makes blocksize = 6615.
			int sampleRate = 44100;
			int blockSize = (int) (sampleRate * 0.15f); //6615;
			int channels = 2;
			host.Init(blockSize, sampleRate, channels);
			System.Diagnostics.Debug.WriteLine(host.getPluginInfo());
			host.LoadFXP(fxpFilePath);
			
			if (doPlay) {
				var playback = new VstPlaybackNAudio(host);
				playback.Play();
				
				Console.WriteLine("Started Audio Playback");
				
				// make sure to play while the stream is playing
				if (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(5000);
				}
				
				Console.WriteLine("Ending Audio Playback");
				playback.Stop();
				Console.WriteLine("Stopped Audio Playback");
				playback.Dispose();
			}			

			if (waveOutputFilePath != "") {
				var fileWriter = new VstFileWriter(host);
				fileWriter.CreateWaveFile(waveOutputFilePath);
			}
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string pluginPath = "";
			string waveInputFilePath = "";
			string waveOutputFilePath = "";
			string fxpFilePath = "";
			bool doPlay = false;
			bool useGui = false;

			// Command line parsing
			var CommandLine = new Arguments(args);
			if(CommandLine["plugin"] != null) {
				pluginPath = CommandLine["plugin"];
			}
			if(CommandLine["wavein"] != null) {
				waveInputFilePath = CommandLine["wavein"];
			}
			if(CommandLine["waveout"] != null) {
				waveOutputFilePath = CommandLine["waveout"];
			}
			if(CommandLine["fxp"] != null) {
				fxpFilePath = CommandLine["fxp"];
			}
			if(CommandLine["play"] != null) {
				doPlay = true;
			}
			if(CommandLine["gui"] != null) {
				useGui = true;
			}
			
			if ((!useGui && pluginPath == "" && waveInputFilePath == "") || (!useGui && waveOutputFilePath == "" && !doPlay)) {
				PrintUsage();
				return;
			}
			
			if (useGui) {
				StartGUI();
			} else {
				StartVstHost(pluginPath, waveInputFilePath, fxpFilePath, waveOutputFilePath, doPlay );
			}
		}
		
		public static void PrintUsage() {
			Console.WriteLine("Process VST Plugin. Version {0}.", _version);
			Console.WriteLine("Copyright (C) 2009-2015 Per Ivar Nerseth.");
			Console.WriteLine();
			Console.WriteLine("Usage: ProcessVSTPlugin.exe <Arguments>");
			Console.WriteLine();
			Console.WriteLine("Mandatory Arguments:");
			Console.WriteLine("\t-plugin=<path to the vst plugin to use (.dll)>");
			Console.WriteLine("\t-wavein=<path to the wave file to use (.wav)>");
			Console.WriteLine();
			Console.WriteLine("Optional Arguments:");
			Console.WriteLine("\t-fxp=<path to the vst preset file to use (.fxp or .fxb)>");
			Console.WriteLine("\t-play=<should we play the wave file, or only process it?>");
			Console.WriteLine("\t-gui=<Use GUI instead>");
			Console.WriteLine("\t-waveout=<path to the wave file to create (.wav)>");
		}
		
	}
}
