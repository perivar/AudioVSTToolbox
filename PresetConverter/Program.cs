using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CommonUtils;

namespace PresetConverter
{
	class Program
	{
		static string _version = "1.1";
		
		public static void Main(string[] args)
		{
			// Build preset file paths
			//string allProjectDir = GetDevelopProjectDir();
			//string sylenthPreset = Path.Combine(allProjectDir, "SynthAnalysisStudio", "Per Ivar - Test Preset (Zebra vs Sylenth).fxp");
			
			// define input file and output directory
			string sylenthPresetDirString = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1";
			string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\www.vengeance-sound.de - Sylenth Trilogy v1 - HandsUpDance Soundset.fxb";
			string convertedPresetOutputDir = @"C:\Program Files\Steinberg\Vstplugins\Synth\u-he\Zebra\Zebra2.data\Presets\Zebra2\Converted Sylenth1 Presets";
			//string convertedPresetOutputDir = Path.Combine(allProjectDir, "PresetConverter");
			bool doProcessInitPresets = false;

			// define default sylenth template for Zebra2
			string zebra2_Sylenth1_PresetTemplate = @"Zebra2-Default Sylenth1 Template.h2p";
			
			// Output a dump of the Sylenth1 Preset File
			//string outFilePath = Path.Combine(allProjectDir, "PresetConverter", "Sylenth1PresetOutput.txt");
			//TextWriter tw = new StreamWriter(outFilePath);
			//tw.WriteLine(sylenth1);
			//tw.Close();
			
			DirectoryInfo sylenthPresetDir = new DirectoryInfo(sylenthPresetDirString);
			FileInfo[] presetFiles = sylenthPresetDir.GetFiles("*.fxb");
			
			foreach (FileInfo presetFile in presetFiles) {
				// read preset file
				Sylenth1Preset sylenth1 = new Sylenth1Preset();
				sylenth1.Read(presetFile.FullName);
				
				// define output dir
				string outputDir = Path.Combine(convertedPresetOutputDir, Path.GetFileNameWithoutExtension(presetFile.Name));
				if (!Directory.Exists(outputDir)) {
					Directory.CreateDirectory(outputDir);
				}

				// and convert to zebra 2
				List<Zebra2Preset> zebra2ConvertedList = sylenth1.ToZebra2Preset(zebra2_Sylenth1_PresetTemplate, doProcessInitPresets);
				int count = 1;
				foreach (Zebra2Preset zebra2Converted in zebra2ConvertedList) {
					string presetName = StringUtils.MakeValidFileName(zebra2Converted.PresetName);
					string zebraGeneratedPreset = Path.Combine(outputDir, String.Format("{0:000}_{1}.h2p", zebra2Converted.BankIndex, presetName));
					zebra2Converted.Write(zebraGeneratedPreset);
					count++;
				}
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);

			return;
			
			// Command line parsing
			string presetInputFilePath = "";
			string presetOutputFilePath = "";
			
			Arguments CommandLine = new Arguments(args);
			if(CommandLine["in"] != null) {
				presetInputFilePath = CommandLine["in"];
			}
			if(CommandLine["out"] != null) {
				presetOutputFilePath = CommandLine["presetOutputFilePath"];
			}
			
			if (presetInputFilePath == "" || presetOutputFilePath == "") {
				PrintUsage();
				return;
			}
			
		}
		
		public static void PrintUsage() {
			Console.WriteLine("Preset Converter. Version {0}.", _version);
			Console.WriteLine("Copyright (C) 2009-2012 Per Ivar Nerseth.");
			Console.WriteLine();
			Console.WriteLine("Usage: PresetConverter.exe <Arguments>");
			Console.WriteLine();
			Console.WriteLine("Mandatory Arguments:");
			Console.WriteLine("\t-in=<path to the preset file to convert from>");
			Console.WriteLine("\t-out=<path to the preset file to convert to>");
			Console.WriteLine();
		}
		
		private static string GetDevelopProjectDir() {
			// to get the location the assembly is executing from
			//(not neccesarily where the it normally resides on disk)
			// in the case of the using shadow copies, for instance in NUnit tests,
			// this will be in a temp directory.
			string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

			//once you have the path you get the directory with:
			string execDirectory = System.IO.Path.GetDirectoryName(execPath);
			
			// move two directories up to get the base dir when executing from SharpDevelop
			// since it will then run from either the project-dir/bin/Debug or project-dir/bin/Release directory
			DirectoryInfo projectDirInfo = new DirectoryInfo(execDirectory).Parent.Parent;
			string projectDir = projectDirInfo.FullName;
			
			// move yeat another dir up to get start dir for all AudioVSTToolbox projects
			DirectoryInfo allProjectDirInfo = new DirectoryInfo(projectDir).Parent;
			string allProjectDir = allProjectDirInfo.FullName;

			return allProjectDir;
		}
	}
}