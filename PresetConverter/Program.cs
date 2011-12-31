using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using CommandLine.Utility;

namespace PresetConverter
{
	class Program
	{
		static string _version = "1.0";
		
		public static void Main(string[] args)
		{
			
			/*
			// test the sylenth envelope conversion methods
			float envValue = 4f;
			float envMs = Sylenth1Preset.EnvelopeValueToMilliseconds(envValue);
			Console.Out.WriteLine("{0} = {1} ms", envValue, envMs);

			float envMs2 = envMs;
			float envValue2 = Sylenth1Preset.MillisecondsToEnvelopeValue(envMs2);
			Console.Out.WriteLine("{0} ms = {1}", envMs2, envValue2);

			float envMs3 = 2500;
			float envValue3 = Sylenth1Preset.MillisecondsToEnvelopeValue(envMs3);
			Console.Out.WriteLine("{0} ms = {1}", envMs3, envValue3);

			float envMs4 = Sylenth1Preset.EnvelopeValueToMilliseconds(envValue3);
			Console.Out.WriteLine("{0} = {1} ms", envValue3, envMs4);
			
			// test the zebra LFO conversion methods
			float msValue = 15;
			Zebra2Preset.LFOSync lfoSync = Zebra2Preset.LFOSync.SYNC_0_1s;
			double lfoValue = 0.0;
			Zebra2Preset.MillisecondsToLFOSyncAndValue(msValue, out lfoSync, out lfoValue);
			Console.Out.WriteLine("{0}ms = {1} {2}", msValue, lfoValue, lfoSync );
			
			float freqValue = 70.5f;
			float freqHz = Zebra2Preset.EqualiserFreqValueToHz(freqValue);
			Console.Out.WriteLine("{0} = {1} Hz", freqValue, freqHz);
			
			float freqValueTest = Zebra2Preset.EqualiserHzToFreqValue(freqHz);
			Console.Out.WriteLine("{0} Hz = {1}", freqHz, freqValueTest);
			
			float freqHzTest = 3000.0f;
			float freqValueTest2 = Zebra2Preset.EqualiserHzToFreqValue(freqHzTest);
			Console.Out.WriteLine("{0} Hz = {1}", freqHzTest, freqValueTest2);
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
			return;
			 */			
			
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
			//string sylenthPreset = Path.Combine(allProjectDir, "ProcessVSTPlugin", "Per Ivar - Test Preset (Zebra vs Sylenth).fxp");
			string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\www.vengeance-sound.de - Sylenth Trilogy v1 - HandsUpDance Soundset.fxb";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Default - Preset.fxp";
			//string sylenthPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Sylenth - Test - Preset.fxp";
			
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Per Ivar - Test Preset (Zebra vs Sylenth).h2p";
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\ProcessVSTPlugin\Zebra2.data\Presets\Zebra2\initialize-extended.h2p";
			//string zebraPreset = @"C:\Users\perivar.nerseth\My Projects\AudioVSTToolbox\PresetConverter\initialize-extended2.h2p";
			//string zebraPreset = Path.Combine(allProjectDir, "PresetConverter", "initialize-extended2.h2p");
			string zebra2_Sylenth1_PresetTemplate = @"C:\Program Files\Steinberg\Vstplugins\Synth\u-he\Zebra\Zebra2.data\Presets\Zebra2\Per Ivar\Zebra2-Default Sylenth1 Template.h2p";
			
			Sylenth1Preset sylenth1 = new Sylenth1Preset();
			sylenth1.Read(sylenthPreset);

			// Output a dump of the Sylenth1 Preset File
			string outFilePath = Path.Combine(allProjectDir, "PresetConverter", "Sylenth1PresetOutput.txt");
			TextWriter tw = new StreamWriter(outFilePath);
			tw.WriteLine(sylenth1);
			tw.Close();
			
			// Output a dump of the Zebra2 Preset File
			string outFilePath2 = Path.Combine(allProjectDir, "PresetConverter", "Zebra2PresetOutput.txt");
			TextWriter tw2 = new StreamWriter(outFilePath2);
			
			List<Zebra2Preset> zebra2ConvertedList = sylenth1.ToZebra2Preset(zebra2_Sylenth1_PresetTemplate, false);
			int count = 1;
			foreach (Zebra2Preset zebra2Converted in zebra2ConvertedList) {
				string presetName = StringUtils.MakeValidFileName(zebra2Converted.PresetName);
				string zebraGeneratedPreset = Path.Combine(@"C:\Program Files\Steinberg\Vstplugins\Synth\u-he\Zebra\Zebra2.data\Presets\Zebra2\Converted Sylenth1 Presets", String.Format("{0:00}_{1}.h2p", count, presetName));
				zebra2Converted.Write(zebraGeneratedPreset);
				tw2.WriteLine(zebra2Converted);
				count++;
			}
			tw2.Close();
			
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
		
	}
}