using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

using NAudio.Wave;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

using CommonUtils;

using DiffPlex;
using DiffPlex.Model;

namespace ProcessVSTPlugin
{
	static class Program
	{
		static string _version = "1.1";
		
		static void StartGUI() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void StartAudioOutput(string pluginPath, string waveFilePath) {
			try
			{
				HostCommandStub hostCmdStub = new HostCommandStub();
				VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);
				
				// add custom data to the context
				ctx.Set("PluginPath", pluginPath);
				ctx.Set("HostCmdStub", hostCmdStub);
				
				// actually open the plugin itself
				ctx.PluginCommandStub.Open();
				
				AudioOutput audioOut = new AudioOutput(
					new List<IVstPluginCommandStub>() {ctx.PluginCommandStub},
					waveFilePath);
				Thread.Sleep(100);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		static void StartVstHost(string pluginPath, string waveInputFilePath, string fxpFilePath, string waveOutputFilePath, bool doPlay) {

			VstHost host = VstHost.Instance;
			host.OpenPlugin(pluginPath);
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
				VstPlaybackNAudio playback = new VstPlaybackNAudio(host);
				playback.Play();
				
				Console.WriteLine("Started Audio Playback");
				
				// make sure to play while the stream is playing
				while (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(100);
				}
				
				Console.WriteLine("Ending Audio Playback");
				playback.Stop();
				Console.WriteLine("Stopped Audio Playback");
				playback.Dispose();
			}
			
			if (waveOutputFilePath != "") {
				VstFileWriter fileWriter = new VstFileWriter(host);
				fileWriter.CreateWaveFile(waveOutputFilePath);
			}
		}
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			/*
			float frequency = 12000;
			float amplitude = 0.25f; // let's not hurt our ears
			double sampleRate = 44100;
			int fftWindowsSize = 4096;
			int fftOverlap = 1;
			int numSeconds = 1;
			float[] buffer = MathUtils.GetSineWave(frequency, amplitude, (float) sampleRate, 0, (int) sampleRate*numSeconds);
			float[][] spectrogram = CommonUtils.FFT.FFTUtils.CreateSpectrogram(buffer, sampleRate, fftWindowsSize, fftOverlap, true);
			System.Drawing.Bitmap png = CommonUtils.FFT.FFTUtils.PrepareAndDrawSpectrumAnalysis(spectrogram, sampleRate, fftWindowsSize, fftOverlap,
			                                                                                    new System.Drawing.Size(600, 300));
			png.Save("test.png");
			return;
			*/
			
			string pluginPath = "";
			string waveInputFilePath = "";
			string waveOutputFilePath = "";
			string fxpFilePath = "";
			bool doPlay = false;
			bool useGui = false;

			// Command line parsing
			Arguments CommandLine = new Arguments(args);
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
				//StartAudioOutput(pluginPath);
				StartVstHost(pluginPath, waveInputFilePath, fxpFilePath, waveOutputFilePath, doPlay );
			}
		}
		
		public static void PrintUsage() {
			Console.WriteLine("Process VST Plugin. Version {0}.", _version);
			Console.WriteLine("Copyright (C) 2009-2012 Per Ivar Nerseth.");
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
