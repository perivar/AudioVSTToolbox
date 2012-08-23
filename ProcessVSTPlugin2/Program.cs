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
			SampleAltiverbPresets(0, false, false, false, "");
			//SampleAltiverbPresets(4, false, false, true, "ONLINE");
			return;

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
		
		/// <summary>
		/// Sample Altiverb Presets into Wav files
		/// </summary>
		/// <param name="limitCount">if zero = disabled</param>
		/// <param name="doCrop">should we crop the audio</param>
		/// <param name="doQuad">should we sample a quad impulse response (i.e. two stereos) OR a normal stereo file</param>
		/// <param name="doPlay">should we play (online processing = true) or use offline = false</param>
		/// <param name="append">text to append to output filename</param>
		public static void SampleAltiverbPresets(int limitCount, bool doCrop, bool doQuad, bool doPlay, string append) {
			
			/*
	PRE REQUSITES FOR RUNNING THIS SCRIPT:
	1. 	The Altiverb preset files that will be used for sampling (*.fxp)
	2. 	Voxengo Deconvolver
	3. 	Four Sweep Files: L, R and Mono (used for Quad sampling) and LR (used for Stereo sampling)
	Reversed Technique seems to work best (Use Voxengo Deconvolver to generate these):
	- Sweep-48000-32f-M-10,0s-fade.wav
	- Sweep-48000-32f-SL-10,0s-fade.wav
	- Sweep-48000-32f-SLR-10,0s-fade.wav
	- Sweep-48000-32f-SR-10,0s-fade.wav
	
	All steps required for sampling the quad altiverb effects.
	1. 	Create altiverb presets for the Altiverb quad effects that you want to sample (.fxp files)
		either manually or using the "CS Script": cscs CreateAltiverbPresets.cs
		Output: A directory full of .fxp files
		Tip! Use a seperate directory for stereo and quad presets.
	
	2. 	Run this function
		Output:
		For Quad 	- A directory full of *_L.wav and *_R.wav files
		For Stereo 	- A directory full of *.wav files
	
	3. 	Run Voxengo Deconvolver - with the Mono file as main sweep file and the directory where all the
		sampled Altiverb presets were stored.
		(Remember to use the Reversed Technique)
		Output:
		For Quad 	- A directory full of *_L_dc.wav and *_R_dc.wav files
		For Stereo 	- A directory full of *_dc.wav files
	
	4. 	Only for Quad files:
		Combine all the stereo _dc.wav files into Quad files,
		by running the Sound Forge Script "Search Dir and Combine Stereo to Quad.cs" within Sound Forge.
		Output: A directory full of *Quad.wav files
	
	5. 	Create a identical directory structure for the wav files as the original Altiverb Library
		by running the "CS Script": BuildAltiverbWavDirectoryStructure.cs
		Output: An Altiverb Library for wav files.
		I.e. a new directory set full of *Quad.wav (or Stereo) files in their correct locations
		as well as nice corresponding images copied from the original Altiverb Library
			 */
			
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
				                    limitCount, doCrop, doQuad, doPlay, append);
			} catch (Exception e) {
				Console.Out.WriteLine("Unknown Error while processing FXP directory: ", e.ToString());
			}
		}
		
		public static bool processFXPDirectory(string presetDirectoryPath, string pluginPath, string audioOutputDirectoryPath,
		                                       string sweepFileNameLeft, string sweepFileNameRight, string sweepFileNameLeftRight,
		                                       int limitCount, bool doCrop, bool doQuad, bool doPlay,
		                                       string append) {
			
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
						string renderFileNameLeft = String.Format("{0}{1}{2}.{3}", shortPresetName, "_L", append, "wav");
						string renderFileNameFullPathLeft = Path.Combine(audioOutputDirectoryPath, renderFileNameLeft);
						string renderFileNameRight = String.Format("{0}{1}{2}.{3}", shortPresetName, "_R", append, "wav");
						string renderFileNameFullPathRight = Path.Combine(audioOutputDirectoryPath, renderFileNameRight);

						// process first left sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathLeft)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameLeft, renderFileNameFullPathLeft, doCrop, doPlay)) {
								Console.Out.WriteLine("Processing left file failed!");
								continue;
							}
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathLeft);
						}

						// process right sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathRight)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameRight, renderFileNameFullPathRight, doCrop, doPlay)) {
								Console.Out.WriteLine("Processing right file failed!");
								continue;
							}
							presetCounter++;
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathRight);
						}
					} else {
						// stereo not QUAD
						string renderFileNameLeftRight = String.Format("{0}{1}.{2}", shortPresetName, append, "wav");
						string renderFileNameFullPathLeftRight = Path.Combine(audioOutputDirectoryPath, renderFileNameLeftRight);
						
						// process stereo sweep file
						// skip if file exists
						if (!File.Exists(renderFileNameFullPathLeftRight)) {
							if (!processFile(processVSTPlugin, pluginPath, fi.FullName, sweepFileNameLeftRight, renderFileNameFullPathLeftRight, doCrop, doPlay)) {
								Console.Out.WriteLine("Processing right file failed!");
								continue;
							}
							presetCounter++;
						} else {
							Console.Out.WriteLine("File already exists. ('{0}')", renderFileNameFullPathLeftRight);
						}
					}
					
				} // limit count ?

				// wait a little while longer every X preset?
				if (presetCounter>0 &&  presetCounter%5 == 0) {
					Thread.Sleep(5000);
					processVSTPlugin.Dispose();
					processVSTPlugin = null;
					processVSTPlugin = new ProcessVSTPlugin();
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
		                               bool doCrop,
		                               bool doPlay) {
			
			float volume = 1.0f;
			return processVSTPlugin.Process(sweepFilePath, renderFileNameFullPath, pluginPath, presetPath, volume, doPlay);
		}
	}
}