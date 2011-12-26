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
			string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Default - Preset.fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Test - Preset.fxp";
				
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).h2p";
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Zebra2.data\Presets\Zebra2\initialize-extended.h2p";
			string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\initialize-extended2.h2p";
			
			string zebraGeneratedPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\Generated-Zebra2-Preset.h2p";
			
			Sylenth1Preset sylenth1 = new Sylenth1Preset(sylenthPreset);
			//sylenth1.TransformToZebra2("");
			//Console.Out.WriteLine(sylenth1);
			string outFilePath = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\Sylenth1PresetOutput.txt";
			TextWriter tw = new StreamWriter(outFilePath);
			tw.WriteLine(sylenth1);
			tw.Close();
			
			//Zebra2Preset zebra2 = new Zebra2Preset();
			//zebra2.GenerateWriteMethod(zebraPreset, zebraPreset + ".txt");
			//zebra2.GenerateClassFields(zebraPreset, zebraPreset + ".txt");
			
			//Zebra2Preset zebra2 = new Zebra2Preset(zebraPreset);
			//zebra2.Write(zebraGeneratedPreset);
			
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