using System;
using System.Text;
using System.IO;
using CommandLine.Utility;

namespace PresetConverter
{
	class Program
	{
		static string _version = "1.0";
		
		public static void Main(string[] args)
		{
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
			
			// Build preset file paths
			string sylenthPreset = Path.Combine(allProjectDir, "ProcessVSTPlugin", "Per Ivar - Test Preset (Zebra vs Sylenth).fxp");
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Default - Preset.fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Test - Preset.fxp";
			
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).h2p";
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Zebra2.data\Presets\Zebra2\initialize-extended.h2p";
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\initialize-extended2.h2p";
			//string zebraPreset = Path.Combine(allProjectDir, "PresetConverter", "initialize-extended2.h2p");
			string zebraPreset = @"C:\Program Files\Steinberg\Vstplugins\Synth\u-he\Zebra\Zebra2.data\Presets\Zebra2\Per Ivar\Zebra2-Default Sylenth1 Template.h2p";
			
			//string zebraGeneratedPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\Generated-Zebra2-Preset.h2p";
			//string zebraGeneratedPreset = Path.Combine(allProjectDir, "PresetConverter", "Generated-Zebra2-Preset.h2p");
			string zebraGeneratedPreset = @"C:\Program Files\Steinberg\Vstplugins\Synth\u-he\Zebra\Zebra2.data\Presets\Zebra2\Per Ivar\1111-deleteme-testing.h2p";
			
			Sylenth1Preset sylenth1 = new Sylenth1Preset();
			sylenth1.Read(sylenthPreset);
			
			//sylenth1.TransformToZebra2("");
			//Console.Out.WriteLine(sylenth1);
			//string outFilePath = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\Sylenth1PresetOutput.txt";
			string outFilePath = Path.Combine(allProjectDir, "PresetConverter", "Sylenth1PresetOutput.txt");
			TextWriter tw = new StreamWriter(outFilePath);
			tw.WriteLine(sylenth1);
			tw.Close();
			
			Zebra2Preset zebra2 = new Zebra2Preset();
			//zebra2.Read(zebraPreset);
			
			//string outFilePath2 = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\Zebra2PresetOutput.txt";
			string outFilePath2 = Path.Combine(allProjectDir, "PresetConverter", "Zebra2PresetOutput.txt");
			
			Zebra2Preset zebra2Converted = sylenth1.ToZebra2Preset(zebraPreset);

			TextWriter tw2 = new StreamWriter(outFilePath2);
			tw2.WriteLine(zebra2Converted);
			tw2.Close();
			
			zebra2Converted.Write(zebraGeneratedPreset);
			
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
			Console.WriteLine("Copyright (C) 2009-2011 Per Ivar Nerseth.");
			Console.WriteLine();
			Console.WriteLine("Usage: PresetConverter.exe <Arguments>");
			Console.WriteLine();
			Console.WriteLine("Mandatory Arguments:");
			Console.WriteLine("\t-in=<path to the preset file to convert from>");
			Console.WriteLine("\t-out=<path to the preset file to convert to>");
			Console.WriteLine();
		}
		
	}
}