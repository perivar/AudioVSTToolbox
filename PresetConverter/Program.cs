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
			
			/*
			 // test the envelope values
			double[] dArray = new double[] {
				0
					,0.020000387
					,0.040000387
					,0.060000386
					,0.080000386
					,0.100000381
					,0.120000377
					,0.140000373
					,0.160000369
					,0.180000365
					,0.200000361
					,0.220000356
					,0.240000352
					,0.260000348
					,0.280000359
					,0.30000037
					,0.32000038
					,0.3400004
					,0.3600004
					,0.3800004
					,0.400000423
					,0.420000434
					,0.440000445
					,0.460000455
					,0.480000466
					,0.5000005
					,0.520000458
					,0.540000439
					,0.5600004
					,0.5800004
					,0.6000004
					,0.620000362
					,0.640000343
					,0.6600003
					,0.6800003
					,0.7000003
					,0.720000267
					,0.740000248
					,0.7600002
					,0.7800002
					,0.8000002
					,0.8200002
					,0.840000153
					,0.860000134
					,0.8800001
					,0.9000001
					,0.9200001
					,0.940000057
					,0.960000038
					,0.98
					,1};
			
			foreach (double d in dArray) {
				float f = (float) d;
				float sylenthEnvelopeMs = Sylenth1Preset.EnvelopePresetFileValueToMilliseconds(f);
				double envValue = Zebra2Preset.MillisecondsToValue(sylenthEnvelopeMs, Zebra2Preset.EnvelopeTimeBase.TIMEBASE_8sX);
				double ms = Zebra2Preset.EnvTypeAndValueToMilliseconds(Zebra2Preset.EnvelopeTimeBase.TIMEBASE_8sX, envValue);
				Console.Out.WriteLine("stored {0:0.000} = {1:0.00} ms (Zebra midi: {2}: {3} ms)", f, sylenthEnvelopeMs, envValue, ms);
			}

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
			return;
			
			string[] filePaths = Directory.GetFiles("../..", "*.fxp");
			foreach (string file in filePaths) {
				// perivar-filter-2022hz.fxp
				string hertz = "";
				var regex = new Regex(@"perivar-filter-(\d+)hz");
				var match = regex.Match(file);
				if (match.Success) {
					hertz = match.Groups[1].Value;
				}
				int hertzValue = 0;
				int.TryParse(hertz, out hertzValue);
				
				Sylenth1Preset sylenth = new Sylenth1Preset(file);
				float filterACutoff = sylenth.ContentArray[0].FilterACutoff;
				float filterCtlCutoff = sylenth.ContentArray[0].FilterCtlCutoff;
				
				float filterACutoffHz = Sylenth1Preset.ValueToHz(filterACutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
				float filterCtlCutoffHz = Sylenth1Preset.ValueToHz(filterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
				
				// test the algorithm
				float testHz1 = Sylenth1Preset.ConvertSylenthFrequencyToHertz(filterACutoff, filterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
				float testHz1Midi = Sylenth1Preset.ConvertSylenthFrequencyToZebra(filterACutoff, filterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
				float testZebraHz1 = Zebra2Preset.MidiNoteToFilterFrequency((int)testHz1Midi);
				Console.Out.WriteLine("Hz {0:0} => {5:0.00} => {6:0.00} [{7}] ({1:0.0000}={2:0.00} hz, {3:0.0000}={4:0.00} hz)", hertzValue, filterACutoff, filterACutoffHz, filterCtlCutoff, filterCtlCutoffHz, testHz1, testZebraHz1, testHz1Midi);
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
			return;
			 */
			
			/*
			float filterFrequencyHertz = 0.4047619f; 			//	56,57 Hz
			float filterControlFrequencyHertz = 0.823809564f; 	// 3685,85 Hz
			
			float f1 = Sylenth1Preset.ConvertSylenthFrequencyToZebra(filterFrequencyHertz, filterControlFrequencyHertz, Sylenth1Preset.FloatToHz.FilterCutoff);
			float f2 = Sylenth1Preset.ConvertSylenthFrequencyToZebraOLD(filterFrequencyHertz, filterControlFrequencyHertz, Sylenth1Preset.FloatToHz.FilterCutoff);
			return;

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
			
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-2022hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-4066hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-6000hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-10000hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-12000hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-16400hz.fxp";
			//string sylenthPreset = @"C:\Program Files (x86)\Steinberg\Vstplugins\Synth\Sylenth1\perivar-filter-18600hz.fxp";
			
			
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