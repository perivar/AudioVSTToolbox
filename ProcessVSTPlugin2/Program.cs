using System;
using System.Globalization;
using System.Threading;
using System.IO;

using CommonUtils;

namespace ProcessVSTPlugin2
{
	class Program
	{
		static string _version = "2.0.5";
		
		[STAThread]
		public static void Main(string[] args)
		{
			//SampleAltiverbPresets();
			//return;

			string pluginPath = "";
			string waveInputFilePath = "";
			string waveOutputFilePath = "";
			string fxpFilePath = "";
			float volume = 1.0f;
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
			if(CommandLine["volume"] != null) {
				float f = 1.0f;
				float.TryParse(CommandLine["volume"], NumberStyles.Number,CultureInfo.InvariantCulture, out f);
				volume = f;
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

			ProcessVSTPlugin processVSTPlugin = new ProcessVSTPlugin();
			if (!processVSTPlugin.Process(waveInputFilePath, waveOutputFilePath, pluginPath, fxpFilePath, volume, doPlay)) {
				Console.WriteLine("Processing Failed!");

				Console.WriteLine("");
				Console.Write("Press any key to continue . . . ");
				Console.ReadKey(true);
			}

			// clean up
			processVSTPlugin.Dispose();
			processVSTPlugin = null;
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
			Console.WriteLine("\t-volume=x.x <volume to use on the input file. (min=0.0, max=1.0)>");
			Console.WriteLine("\t-play\t<use realtime processing instead of the default=offline>");
			
			Console.WriteLine("");
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		
		
		public static void SampleAltiverbPresets() {
			int limitCount = 10; 		// if zero = disabled
			bool doCrop = false; 		// should we crop audio
			bool doQuad = true; 		// should we sample a quad impulse response (i.e. two stereos)
			// OR a normal stereo file
			
			string audioOutputDirectoryPath = "";
			string presetDirectoryPath = "";
			if (doQuad) {
				// QUAD files
				audioOutputDirectoryPath = @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\ALTIVERB-OUTPUT-QUAD";
				presetDirectoryPath 	= @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\ALTIVERB-QUAD-PRESETS";
			} else {
				// STEREO files
				audioOutputDirectoryPath = @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\ALTIVERB-OUTPUT-STEREO";
				presetDirectoryPath 	= @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\ALTIVERB-STEREO-PRESETS";
			}
			
			// sweep right
			string sweepFileNameRight = @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\Sweep-48000-32f-SR-10,0s-fade.wav";
			
			// sweep left
			string sweepFileNameLeft = @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\Sweep-48000-32f-SL-10,0s-fade.wav";
			
			// sweep stereo left and right
			string sweepFileNameLeftRight = @"F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\Sweep-48000-32f-SLR-10,0s-fade.wav";
			
			// what plugin to use
			string pluginPath = @"C:\Program Files (x86)\Steinberg\Vstplugins\Reverb\Altiverb\Altiverb 6.dll";
			
			try {
				processFXPDirectory(presetDirectoryPath, pluginPath, audioOutputDirectoryPath,
				                    sweepFileNameLeft, sweepFileNameRight, sweepFileNameLeftRight,
				                    limitCount, doCrop, doQuad);
			} catch (Exception e) {
				Console.Out.WriteLine("Unknown Error while processing FXP directory: ", e.ToString());
			}
		}
		
		public static bool processFXPDirectory(string presetDirectoryPath, string pluginPath, string audioOutputDirectoryPath,
		                                       string sweepFileNameLeft, string sweepFileNameRight, string sweepFileNameLeftRight,
		                                       int limitCount, bool doCrop, bool doQuad) {
			
			if (!Directory.Exists(presetDirectoryPath)) {
				Console.Out.WriteLine("Cannot find preset directory '{0}'. Script canceled.", presetDirectoryPath);
				return false;
			}
			
			DirectoryInfo di = new DirectoryInfo(presetDirectoryPath);
			FileInfo[] fxpFiles = di.GetFiles("*.fxp");
			Console.Out.WriteLine("Found {0} preset files to process ...", fxpFiles.Length );
			Console.Out.WriteLine("Audio Output Directory: {0}", audioOutputDirectoryPath);
			ProcessVSTPlugin processVSTPlugin = new ProcessVSTPlugin();
			
			int presetCounter = 0;
			foreach(FileInfo fi in fxpFiles)
			{
				// limit count?
				if ( (limitCount > 0 && limitCount > presetCounter) || limitCount == 0 ) {

					Console.Out.WriteLine("Processing preset number {0} ...", presetCounter);
					
					string presetFileName = fi.Name;
					Console.Out.WriteLine("Preset filename: " +  presetFileName);
					
					string shortPresetName = Path.GetFileNameWithoutExtension(fi.FullName);
					Console.Out.WriteLine("Short Preset Name: {0}", shortPresetName);
					
					// make sure the output directory exist
					Directory.CreateDirectory(audioOutputDirectoryPath);
					
					if (doQuad) {
						string renderFileNameLeft = String.Format("{0}{1}.{2}", shortPresetName, "_L_NET", "wav");
						string renderFileNameFullPathLeft = Path.Combine(audioOutputDirectoryPath, renderFileNameLeft);
						string renderFileNameRight = String.Format("{0}{1}.{2}", shortPresetName, "_R_NET", "wav");
						string renderFileNameFullPathRight = Path.Combine(audioOutputDirectoryPath, renderFileNameRight);

						// process first left sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathLeft)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameLeft, renderFileNameFullPathLeft, doCrop)) {
								Console.Out.WriteLine("Processing left file failed!");
								continue;
							}
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathLeft);
						}

						// process right sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathRight)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameRight, renderFileNameFullPathRight, doCrop)) {
								Console.Out.WriteLine("Processing right file failed!");
								continue;
							}
							presetCounter++;
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathRight);
						}
					} else {
						// stereo not QUAD
						string renderFileNameLeftRight = String.Format("{0}.{1}{2}", shortPresetName, "NET", "wav");
						string renderFileNameFullPathLeftRight = Path.Combine(audioOutputDirectoryPath, renderFileNameLeftRight);
						
						// process stereo sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathLeftRight)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameLeftRight, renderFileNameFullPathLeftRight, doCrop)) {
								Console.Out.WriteLine("Processing right file failed!");
								continue;
							}
							presetCounter++;
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathLeftRight);
						}
					}
					
				} // limit count ?

				// wait a little while longer every tenth preset?
				if (presetCounter>0 &&  presetCounter%10 == 0) {
					Thread.Sleep(1000);
				}
			} // foreach preset file
			
			// clean up
			processVSTPlugin.Dispose();
			processVSTPlugin = null;
			
			return true;
		}
		
		public static bool processFile(ProcessVSTPlugin processVSTPlugin, string pluginPath,
		                               string presetPath,
		                               string sweepFilePath,
		                               string renderFileNameFullPath,
		                               bool doCrop) {
			
			/*
			 * ProcessVSTPlugin2.exe
			 * -plugin="C:\Program Files (x86)\Steinberg\Vstplugins\Reverb\Altiverb\Altiverb 6.dll"
			 * -wavein="F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\Sweep-48000-32f-SL-10,0s-fade.wav"
			 * -fxp="C:\Program Files (x86)\Steinberg\Vstplugins\Reverb\Altiverb\Todd-AO-st to st narrow mics at 08m.fxp"
			 * -waveout="F:\SAMPLES\IMPULSE RESPONSES\PER IVAR IR SAMPLES\VST_NET_Todd-AO-st to st narrow mics at 08m-SL.wav"
			 * -play
			 */
			float volume = 1.0f;
			return processVSTPlugin.Process(sweepFilePath, renderFileNameFullPath, pluginPath, presetPath, volume, false);
		}
	}
}