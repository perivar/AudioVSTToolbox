using System;
using System.IO;

using CommonUtils;

namespace ProcessVSTPlugin2
{
	class Program
	{
		static string _version = "2.0.3";
		
		[STAThread]
		public static void Main(string[] args)
		{
			string pluginPath = "";
			string waveInputFilePath = "";
			string waveOutputFilePath = "";
			string fxpFilePath = "";
			bool doPlay = false;

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
			
			if ((pluginPath == "" || waveInputFilePath == "" || waveOutputFilePath == "")) {
				PrintUsage();
				return;
			}
			
			if (!File.Exists(pluginPath)) {
				Console.WriteLine("VST Plugin cannot be found! ({0})", pluginPath);
				Console.WriteLine("Processing Failed!");
				PrintUsage();
				return;
			}
			if (!File.Exists(waveInputFilePath)) {
				Console.WriteLine("Wave Input File cannot be found! ({0})", waveInputFilePath);
				Console.WriteLine("Processing Failed!");
				PrintUsage();
				return;
			}
			if (!ProcessVSTPlugin.Process(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath, doPlay)) {
				Console.WriteLine("Processing Failed!");

				Console.WriteLine("");
				Console.Write("Press any key to continue . . . ");
				Console.ReadKey(true);
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
			
			Console.WriteLine("");
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}