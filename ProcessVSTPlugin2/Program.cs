using System;
using System.Threading;

using NAudio.Wave;

using CommonUtils;

namespace ProcessVSTPlugin2
{
	class Program
	{
		static string _version = "2.0";
		
		[STAThread]
		public static void Main(string[] args)
		{
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
			
			UtilityAudio.OpenAudio(AudioLibrary.NAudio);
			VST vst = UtilityAudio.LoadVST(pluginPath);
			vst.LoadFXP(fxpFilePath);
			
			vst.StreamCall += new EventHandler<VSTStreamEventArgs>(vst_StreamCall);
			UtilityAudio.StartAudio();
			
			UtilityAudio.vstStream.InputWave = waveInputFilePath;
			UtilityAudio.SaveStream(waveOutputFilePath);
			UtilityAudio.StartStreamingToDisk();
			
			// make sure to play while the stream is playing
			while (UtilityAudio.playbackDevice.PlaybackState == PlaybackState.Playing)
			{
				Thread.Sleep(100);
			}

			UtilityAudio.StopStreamingToDisk();
		}
		
		static void vst_StreamCall(object sender, VSTStreamEventArgs e)
		{
			// stop the audio if nothing is playing any longer
			// TODO: but make sure we have at least waited 5 sec
			float almostZero = 0.000001f;
			if (e.MaxL <  almostZero && e.MaxR < almostZero) {
				Console.Out.Write("-");
				UtilityAudio.StopAudio();
			} else {
				Console.Write(".");
			}
		}
		
		public static void PrintUsage() {
			Console.WriteLine("Process VST Plugin. Version {0}.", _version);
			Console.WriteLine("Copyright (C) 2009-2012 Per Ivar Nerseth.");
			Console.WriteLine();
			Console.WriteLine("Usage: ProcessVSTPlugin2.exe <Arguments>");
			Console.WriteLine();
			Console.WriteLine("Mandatory Arguments:");
			Console.WriteLine("\t-plugin=<path to the vst plugin to use (.dll)>");
			Console.WriteLine("\t-wavein=<path to the wave file to use (.wav)>");
			Console.WriteLine("\t-waveout=<path to the wave file to create (.wav)>");
			Console.WriteLine();
			Console.WriteLine("Optional Arguments:");
			Console.WriteLine("\t-fxp=<path to the vst preset file to use (.fxp or .fxb)>");
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}