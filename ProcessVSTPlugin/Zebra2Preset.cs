using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of Zebra2Preset.
	/// </summary>
	public class Zebra2Preset
	{
		enum LFOGlobalTriggering : int {
			Trig_off = 0,
			Trig_each_bar = 1,
			Trig_2_bars = 2,
			Trig_3_bars = 3,
			Trig_4_bars = 4,
			Trig_5_bars = 5,
			Trig_6_bars = 6,
			Trig_7_bars = 7,
			Trig_8_bars = 8,
			Trig_9_bars = 9,
			Trig_10_bars = 10,
			Trig_11_bars = 11,
			Trig_12_bars = 12,
			Trig_13_bars = 13,
			Trig_14_bars = 14,
			Trig_15_bars = 15,
			Trig_16_bars = 16,
			Trig_17_bars = 17,
			Trig_18_bars = 18,
			Trig_19_bars = 19,
			Trig_20_bars = 20,
			Trig_21_bars = 21,
			Trig_22_bars = 22,
			Trig_23_bars = 23,
			Trig_24_bars = 24,
			Trig_25_bars = 25,
			Trig_26_bars = 26,
			Trig_27_bars = 27,
			Trig_28_bars = 28,
			Trig_29_bars = 29,
			Trig_30_bars = 30,
			Trig_31_bars = 31,
			Trig_32_bars = 32
		}
		
		enum EnvelopeMode : int {
			quadric = 0,
			linear = 1,
			v_slope = 2
		}

		enum EnvelopeInitMode : int {
			none = 0,
			Init = 1,
			Delay = 2
		}

		enum EnvelopeSustainMode : int {
			none = 0,
			Sust2 = 1,
			LoopA = 2,
			LoopD = 3,
			LoopS = 4,
			Rel25 = 5,
			Rel50 = 6,
			Rel75 = 7,
			Re100 = 8
		}
		
		enum EnvelopeTimeBase : int {
			TIMEBASE_8sX = 0,
			TIMEBASE_16sX = 1,
			TIMEBASE_10s = 2,
			TIMEBASE_1_4 = 3,
			TIMEBASE_1_1 = 4,
			TIMEBASE_4_1 = 5
		}
		
		enum LFOSync : int {
			// -3 = 0.1s, -2 = 1s, -1 = 10s, 0 = 1/64, 1 = 1/32, 2 = 1/16, 3 = 1/8,
			// 4 = 1/4, 5 = 1/2, 6 = 1/1, 7 = 1/32 dot, 8 = 1/16 dot, 9 = 1/8 dot, 10 = 1/4 dot,
			// 11 = 1/2 dot, 12 = 1/16 trip, 13 = 1/8 trip, 14 = 1/4 trip, 15 = 1/2 trip,
			// 16 = 1/1 trip, 17 = 2/1, 18 = 3/1 =>23 = 8/1
			SYNC_0_1s = -3,
			SYNC_1s = -2,
			SYNC_10s = -1,
			SYNC_1_64 = 0,
			SYNC_1_32 = 1,
			SYNC_1_16 = 2,
			SYNC_1_8 = 3,
			SYNC_1_4 = 4,
			SYNC_1_2 = 5,
			SYNC_1_1 = 6,
			SYNC_1_32_dot = 7,
			SYNC_1_16_dot = 8,
			SYNC_1_8_dot = 9,
			SYNC_1_4_dot = 10,
			SYNC_1_2_dot = 11,
			SYNC_1_16_trip = 12,
			SYNC_1_8_trip = 13,
			SYNC_1_4_trip = 14,
			SYNC_1_2_trip = 15,
			SYNC_1_1_trip = 16,
			SYNC_2_1 = 17,
			SYNC_3_1 = 18,
			SYNC_4_1 = 19,
			SYNC_5_1 = 20,
			SYNC_6_1 = 21,
			SYNC_7_1 = 22,
			SYNC_8_1 = 23,
		}

		enum LFOTriggering : int {
			free = 0,
			gate = 1
		}
		
		enum LFOWave : int {
			sine = 0,
			triangle = 1,
			saw_up = 2,
			saw_down = 3,
			sqr_hi_lo = 4,
			sqr_lo_hi = 5,
			rand_hold = 6,
			rand_glide = 7,
			user = 8
		}
		
		enum LFOSlew : int {
			off = 0,
			fast = 1,
			slow = 2
		}
		
		enum LFOModulationSource : int {
			none = 0,
			ModWhl = 1,
			PitchW = 2,
			Breath = 3,
			Xpress = 4,
			LfoG1 = 5,
			LfoG2 = 6,
			Gate = 7,
			KeyFol = 8,
			KeyFol2 = 9,
			Velocity = 10,
			ATouch = 11,
			ArpMod = 12,
			ArpMd2 = 13,
			Env1 = 14,
			Env2 = 15,
			Env3 = 16,
			Env4 = 17,
			MSEG1 = 18,
			MSEG2 = 19,
			MSEG3 = 20,
			MSEG4 = 21
		}
		
		enum ModulationSource : int {
			none = 0,
			ModWhl = 1,
			PitchW = 2,
			Breath = 3,
			Xpress = 4,
			LfoG1 = 5,
			LfoG2 = 6,
			Gate = 7,
			KeyFol = 8,
			KeyFol2 = 9,
			Velocity = 10,
			ATouch = 11,
			ArpMod = 12,
			ArpMd2 = 13,
			Env1 = 14,
			Env2 = 15,
			Env3 = 16,
			Env4 = 17,
			MSEG1 = 18,
			MSEG2 = 19,
			MSEG3 = 20,
			MSEG4 = 21,
			Lfo1 = 22,
			Lfo2 = 23,
			Lfo3 = 24,
			Lfo4 = 25,
			MMap1 = 26,
			MMap2 = 27,
			MMix1 = 28,
			MMix2 = 29,
			MMix3 = 30,
			MMix4 = 31
		}

		enum OscillatorEffect : int {
			none = 0,
			Fundamental = 1,
			Odd_For_Even = 2,
			Brilliance = 3,
			Filter = 4,
			Bandworks = 5,
			Registerizer = 6,
			Scrambler = 7,
			Turbulence = 8,
			Expander = 9,
			Symmetry = 10,
			Phase_XFer = 11,
			Phase_Root = 12,
			Trajector = 13,
			Ripples = 14,
			Formanzilla = 15,
			Sync_Mojo = 16,
			Fractalz = 17,
			Exophase = 18,
			Scale = 19,
			Scatter = 20,
			ChopLift = 21,
			HyperComb = 22,
			PhaseDist = 23,
			Wrap = 24
		}
		
		enum OscillatorPoly : int {
			single = 0,
			dual = 1,
			quad = 2,
			eleven = 3
		}

		enum OnOff : int {
			off = 0,
			on = 1
		}		
		
		enum FilterType : int {
			LP_Xcite = 0,
			LP_Allround = 1,
			LP_MidDrive = 2,
			LP_OldDrive = 3,
			LP_Formant = 4,
			LP_Vintage = 5,
			LP_12dB = 6,
			LP_6dB = 7,
			BP_RezBand = 8,
			BP_QBand = 9,
			HP_24dB = 10,
			HP_12dB = 11,
			BR_Notch = 12,
			EQ_Peaking = 13,
			EQ_LoShelf = 14,
			EQ_HiShelf = 15,
			AP_Phaser_4 = 16,
			AP_Phaser_8 = 17,
			LP_Vintage2 = 18,
			SR_Decimate = 19
		}		
		
		enum DelayMode : int {
			stereo_2 = 0,
			multitap_4 = 1,
			dubby_2_plus_2 = 2,
			serial_2 = 3
		}
		
		enum DelaySync : int {
			Delay_1_sec = -1,
			Delay_1_64 = 0,
			Delay_1_32 = 1,
			Delay_1_16 = 2,
			Delay_1_8 = 3,
			Delay_1_4 = 4,
			Delay_1_2 = 5,
			Delay_1_1 = 6,
			Delay_1_32_dot = 7,
			Delay_1_16_dot = 8,
			Delay_1_8_dot = 9,
			Delay_1_4_dot = 10,
			Delay_1_2_dot = 11,
			Delay_1_16_trip = 12,
			Delay_1_8_trip = 13,
			Delay_1_4_trip = 14,
			Delay_1_2_trip = 15,
			Delay_1_1_trip = 16,
		}		
		
		public Zebra2Preset()
		{

		}

		public Zebra2Preset(string filePath)
		{
			//FXP fxp = new FXP();
			//fxp.ReadFile(filePath);
			//string chunkData = fxp.chunkData;
			
			int strangeNumber = 0;
			int counter = 0;
			string line;
			bool startSavingBinaryData = false;
			StringBuilder uglyCompressedBinaryData = new StringBuilder();
			
			// Read the file and display it line by line.
			StreamReader file = new StreamReader(filePath);
			while((line = file.ReadLine()) != null)
			{
				if (startSavingBinaryData) {
					uglyCompressedBinaryData.Append(line);
				} else if (line.Contains("=")) {
					var parameters = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
					var pair = new KeyValuePair<string, string>(parameters[0], parameters[1]);
					Console.WriteLine ("key: {0}, value: {1}", pair.Key, pair.Value);
				} else {
					if (line.StartsWith("$$$$")) {
						strangeNumber = int.Parse(line.Substring(4));

						// start saving in buffer
						startSavingBinaryData = true;
					}
					//Console.WriteLine (line);
				}
				counter++;
			}
			Console.Out.WriteLine(uglyCompressedBinaryData.ToString());
			file.Close();
		}
		
		public void Write(string filePath) {
			string presetString = GeneratePresetContent();
			
			// create a writer and open the file
			TextWriter tw = new StreamWriter(filePath);
			
			// write the preset string
			tw.Write(presetString);
			
			// close the stream
			tw.Close();
		}

		private string GeneratePresetHeader() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*@meta");
			buffer.AppendLine("Author:");
			buffer.AppendLine("'Generated by Zebra2-generator'");
			buffer.AppendLine("*/");
			buffer.AppendLine();
			buffer.AppendLine("#AM=Zebra2");
			buffer.AppendLine("#Vers=20500");
			buffer.AppendLine("#Endian=little");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GenerateModulatorReferenceTable() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	Modulator Reference Table");
			buffer.AppendLine("*/");
			buffer.AppendLine("#nm=32");
			buffer.AppendLine("#ms=none");
			buffer.AppendLine("#ms=ModWhl");
			buffer.AppendLine("#ms=PitchW");
			buffer.AppendLine("#ms=Breath");
			buffer.AppendLine("#ms=Xpress");
			buffer.AppendLine("#ms=LfoG1");
			buffer.AppendLine("#ms=LfoG2");
			buffer.AppendLine("#ms=Gate");
			buffer.AppendLine("#ms=KeyFol");
			buffer.AppendLine("#ms=KeyFol2");
			buffer.AppendLine("#ms=Velocity");
			buffer.AppendLine("#ms=ATouch");
			buffer.AppendLine("#ms=ArpMod");
			buffer.AppendLine("#ms=ArpMd2");
			buffer.AppendLine("#ms=Env1");
			buffer.AppendLine("#ms=Env2");
			buffer.AppendLine("#ms=Env3");
			buffer.AppendLine("#ms=Env4");
			buffer.AppendLine("#ms=MSEG1");
			buffer.AppendLine("#ms=MSEG2");
			buffer.AppendLine("#ms=MSEG3");
			buffer.AppendLine("#ms=MSEG4");
			buffer.AppendLine("#ms=Lfo1");
			buffer.AppendLine("#ms=Lfo2");
			buffer.AppendLine("#ms=Lfo3");
			buffer.AppendLine("#ms=Lfo4");
			buffer.AppendLine("#ms=MMap1");
			buffer.AppendLine("#ms=MMap2");
			buffer.AppendLine("#ms=MMix1");
			buffer.AppendLine("#ms=MMix2");
			buffer.AppendLine("#ms=MMix3");
			buffer.AppendLine("#ms=MMix4");
			buffer.AppendLine("#nv=5");
			buffer.AppendLine("#mv=Gate");
			buffer.AppendLine("#mv=Env1");
			buffer.AppendLine("#mv=Env2");
			buffer.AppendLine("#mv=Env3");
			buffer.AppendLine("#mv=Env4");
			return buffer.ToString();
		}
		
		private string GeneratePresetMainLoopCircuit() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	Main Loop Circuit");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=main");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "CcOp={0:0.00}", 50.00).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format("#LFOG={0}", 1).PadRight(20)).AppendLine("// Active #LFOG");
			buffer.Append(String.Format("#LFOG2={0}", 1).PadRight(20)).AppendLine("// Active #LFOG2");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetCore() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	Core");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=PCore");

			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_1={0:0.00}", 0.00).PadRight(20)).AppendLine("// X1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Y1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_2={0:0.00}", 0.00).PadRight(20)).AppendLine("// X2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Y2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_3={0:0.00}", 0.00).PadRight(20)).AppendLine("// X3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Y3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_4={0:0.00}", 0.00).PadRight(20)).AppendLine("// X4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Y4");
			buffer.Append(String.Format("MT11={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML11={0:0.00}", 15.00).PadRight(20)).AppendLine("// XY1 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR11={0:0.00}", -15.00).PadRight(20)).AppendLine("// XY1 Left1");
			buffer.Append(String.Format("MT12={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML12={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR12={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left2");
			buffer.Append(String.Format("MT13={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML13={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR13={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left3");
			buffer.Append(String.Format("MT14={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML14={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR14={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left4");
			buffer.Append(String.Format("MT15={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML15={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR15={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left5");
			buffer.Append(String.Format("MT16={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML16={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR16={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left6");
			buffer.Append(String.Format("MT17={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML17={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR17={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left7");
			buffer.Append(String.Format("MT18={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML18={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR18={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Left8");
			buffer.Append(String.Format("MT21={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML21={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR21={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down1");
			buffer.Append(String.Format("MT22={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML22={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR22={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down2");
			buffer.Append(String.Format("MT23={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML23={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR23={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down3");
			buffer.Append(String.Format("MT24={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML24={0:0.00}", 48.00).PadRight(20)).AppendLine("// XY1 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR24={0:0.00}", -48.00).PadRight(20)).AppendLine("// XY1 Down4");
			buffer.Append(String.Format("MT25={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML25={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR25={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down5");
			buffer.Append(String.Format("MT26={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML26={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR26={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down6");
			buffer.Append(String.Format("MT27={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML27={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR27={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down7");
			buffer.Append(String.Format("MT28={0}", "none:assigned").PadRight(20)).AppendLine("// XY1 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML28={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY1 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR28={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY1 Down8");
			buffer.Append(String.Format("MT31={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML31={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR31={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left1");
			buffer.Append(String.Format("MT32={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML32={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR32={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left2");
			buffer.Append(String.Format("MT33={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML33={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR33={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left3");
			buffer.Append(String.Format("MT34={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML34={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR34={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left4");
			buffer.Append(String.Format("MT35={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML35={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR35={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left5");
			buffer.Append(String.Format("MT36={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML36={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR36={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left6");
			buffer.Append(String.Format("MT37={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML37={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR37={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left7");
			buffer.Append(String.Format("MT38={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML38={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR38={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Left8");
			buffer.Append(String.Format("MT41={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML41={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR41={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down1");
			buffer.Append(String.Format("MT42={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML42={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR42={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down2");
			buffer.Append(String.Format("MT43={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML43={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR43={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down3");
			buffer.Append(String.Format("MT44={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML44={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR44={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down4");
			buffer.Append(String.Format("MT45={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML45={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR45={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down5");
			buffer.Append(String.Format("MT46={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML46={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR46={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down6");
			buffer.Append(String.Format("MT47={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML47={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR47={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down7");
			buffer.Append(String.Format("MT48={0}", "none:assigned").PadRight(20)).AppendLine("// XY2 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML48={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY2 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR48={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY2 Down8");
			buffer.Append(String.Format("MT51={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML51={0:0.00}", 8.00).PadRight(20)).AppendLine("// XY3 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR51={0:0.00}", -8.00).PadRight(20)).AppendLine("// XY3 Left1");
			buffer.Append(String.Format("MT52={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML52={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR52={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left2");
			buffer.Append(String.Format("MT53={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML53={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR53={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left3");
			buffer.Append(String.Format("MT54={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML54={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR54={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left4");
			buffer.Append(String.Format("MT55={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML55={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR55={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left5");
			buffer.Append(String.Format("MT56={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML56={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR56={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left6");
			buffer.Append(String.Format("MT57={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML57={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR57={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left7");
			buffer.Append(String.Format("MT58={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML58={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR58={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Left8");
			buffer.Append(String.Format("MT61={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML61={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR61={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down1");
			buffer.Append(String.Format("MT62={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML62={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR62={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down2");
			buffer.Append(String.Format("MT63={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML63={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR63={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down3");
			buffer.Append(String.Format("MT64={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML64={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR64={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down4");
			buffer.Append(String.Format("MT65={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML65={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR65={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down5");
			buffer.Append(String.Format("MT66={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML66={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR66={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down6");
			buffer.Append(String.Format("MT67={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML67={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR67={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down7");
			buffer.Append(String.Format("MT68={0}", "none:assigned").PadRight(20)).AppendLine("// XY3 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML68={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY3 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR68={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY3 Down8");
			buffer.Append(String.Format("MT71={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML71={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR71={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left1");
			buffer.Append(String.Format("MT72={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML72={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR72={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left2");
			buffer.Append(String.Format("MT73={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML73={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR73={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left3");
			buffer.Append(String.Format("MT74={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML74={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR74={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left4");
			buffer.Append(String.Format("MT75={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML75={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR75={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left5");
			buffer.Append(String.Format("MT76={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML76={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR76={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left6");
			buffer.Append(String.Format("MT77={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML77={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR77={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left7");
			buffer.Append(String.Format("MT78={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML78={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR78={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Left8");
			buffer.Append(String.Format("MT81={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML81={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR81={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down1");
			buffer.Append(String.Format("MT82={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML82={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR82={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down2");
			buffer.Append(String.Format("MT83={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML83={0:0.00}", 8.00).PadRight(20)).AppendLine("// XY4 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR83={0:0.00}", -8.00).PadRight(20)).AppendLine("// XY4 Down3");
			buffer.Append(String.Format("MT84={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML84={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR84={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down4");
			buffer.Append(String.Format("MT85={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML85={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR85={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down5");
			buffer.Append(String.Format("MT86={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML86={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR86={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down6");
			buffer.Append(String.Format("MT87={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML87={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR87={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down7");
			buffer.Append(String.Format("MT88={0}", "none:assigned").PadRight(20)).AppendLine("// XY4 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML88={0:0.00}", 50.00).PadRight(20)).AppendLine("// XY4 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR88={0:0.00}", -50.00).PadRight(20)).AppendLine("// XY4 Down8");
			
			buffer.Append(String.Format("MMT1={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix1 Target");
			buffer.Append(String.Format("MMS1={0}", 0).PadRight(20)).AppendLine("// Matrix1 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix1 Depth");
			buffer.Append(String.Format("MMVS1={0}", 0).PadRight(20)).AppendLine("// Matrix1 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix1 Via");
			buffer.Append(String.Format("MMT2={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix2 Target");
			buffer.Append(String.Format("MMS2={0}", 0).PadRight(20)).AppendLine("// Matrix2 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix2 Depth");
			buffer.Append(String.Format("MMVS2={0}", 0).PadRight(20)).AppendLine("// Matrix2 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix2 Via");
			buffer.Append(String.Format("MMT3={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix3 Target");
			buffer.Append(String.Format("MMS3={0}", 0).PadRight(20)).AppendLine("// Matrix3 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix3 Depth");
			buffer.Append(String.Format("MMVS3={0}", 0).PadRight(20)).AppendLine("// Matrix3 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix3 Via");
			buffer.Append(String.Format("MMT4={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix4 Target");
			buffer.Append(String.Format("MMS4={0}", 0).PadRight(20)).AppendLine("// Matrix4 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix4 Depth");
			buffer.Append(String.Format("MMVS4={0}", 0).PadRight(20)).AppendLine("// Matrix4 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix4 Via");
			buffer.Append(String.Format("MMT5={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix5 Target");
			buffer.Append(String.Format("MMS5={0}", 0).PadRight(20)).AppendLine("// Matrix5 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD5={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix5 Depth");
			buffer.Append(String.Format("MMVS5={0}", 0).PadRight(20)).AppendLine("// Matrix5 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD5={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix5 Via");
			buffer.Append(String.Format("MMT6={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix6 Target");
			buffer.Append(String.Format("MMS6={0}", 0).PadRight(20)).AppendLine("// Matrix6 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD6={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix6 Depth");
			buffer.Append(String.Format("MMVS6={0}", 0).PadRight(20)).AppendLine("// Matrix6 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD6={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix6 Via");
			buffer.Append(String.Format("MMT7={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix7 Target");
			buffer.Append(String.Format("MMS7={0}", 0).PadRight(20)).AppendLine("// Matrix7 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD7={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix7 Depth");
			buffer.Append(String.Format("MMVS7={0}", 0).PadRight(20)).AppendLine("// Matrix7 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD7={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix7 Via");
			buffer.Append(String.Format("MMT8={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix8 Target");
			buffer.Append(String.Format("MMS8={0}", 0).PadRight(20)).AppendLine("// Matrix8 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD8={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix8 Depth");
			buffer.Append(String.Format("MMVS8={0}", 0).PadRight(20)).AppendLine("// Matrix8 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD8={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix8 Via");
			buffer.Append(String.Format("MMT9={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix9 Target");
			buffer.Append(String.Format("MMS9={0}", 0).PadRight(20)).AppendLine("// Matrix9 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD9={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix9 Depth");
			buffer.Append(String.Format("MMVS9={0}", 0).PadRight(20)).AppendLine("// Matrix9 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD9={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix9 Via");
			buffer.Append(String.Format("MMT10={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix10 Target");
			buffer.Append(String.Format("MMS10={0}", 0).PadRight(20)).AppendLine("// Matrix10 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD10={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix10 Depth");
			buffer.Append(String.Format("MMVS10={0}", 0).PadRight(20)).AppendLine("// Matrix10 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD10={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix10 Via");
			buffer.Append(String.Format("MMT11={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix11 Target");
			buffer.Append(String.Format("MMS11={0}", 0).PadRight(20)).AppendLine("// Matrix11 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD11={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix11 Depth");
			buffer.Append(String.Format("MMVS11={0}", 0).PadRight(20)).AppendLine("// Matrix11 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD11={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix11 Via");
			buffer.Append(String.Format("MMT12={0}", "none:assigned").PadRight(20)).AppendLine("// Matrix12 Target");
			buffer.Append(String.Format("MMS12={0}", 0).PadRight(20)).AppendLine("// Matrix12 Source=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD12={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix12 Depth");
			buffer.Append(String.Format("MMVS12={0}", 0).PadRight(20)).AppendLine("// Matrix12 ViaSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD12={0:0.00}", 0.00).PadRight(20)).AppendLine("// Matrix12 Via");
			
			buffer.Append(String.Format("SBase={0}", 4).PadRight(20)).AppendLine("// SwingBase=1/4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Swing={0:0.00}", 0.00).PadRight(20)).AppendLine("// Swing");
			buffer.Append(String.Format("STrig={0}", 1).PadRight(20)).AppendLine("// SwingTrigger");
			buffer.Append(String.Format("PSong={0}", 0).PadRight(20)).AppendLine("// PatchSong");
			buffer.Append(String.Format("PFold={0}", 0).PadRight(20)).AppendLine("// binary data for PatchFolder");
			buffer.Append(String.Format("PFile={0}", 1).PadRight(20)).AppendLine("// binary data for PatchFileName");
			buffer.Append(String.Format("GFile={0}", 2).PadRight(20)).AppendLine("// binary data for GUI FileName");
			buffer.Append(String.Format("GScale={0}", 0).PadRight(20)).AppendLine("// GUI Scale");
			buffer.Append(String.Format("ChLay={0}", 0).PadRight(20)).AppendLine("// Channel Layout=auto/surround");
			buffer.Append(String.Format("SurrO={0}", 1).PadRight(20)).AppendLine("// Surround Options=no lfe");

			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetLFOG(string description, string name, int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Sync={0}", (int)LFOSync.SYNC_1_4).PadRight(20)).AppendLine("// Sync=1/4");
			buffer.Append(String.Format("Trig={0}", (int)LFOGlobalTriggering.Trig_off).PadRight(20)).AppendLine("// Restart=off");
			buffer.Append(String.Format("Wave={0}", (int)LFOWave.sine).PadRight(20)).AppendLine("// Waveform=sine");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", 0.00).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", 100.00).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", 100.00).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Nstp={0}", 16).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", 0).PadRight(20)).AppendLine("// User Wave Mode=steps");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetVoiceCircuit() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	VoiceCircuit");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=VCC");

			buffer.Append(String.Format("#LFO1={0}", 1).PadRight(20)).AppendLine("// Active #LFO1");
			buffer.Append(String.Format("#LFO2={0}", 1).PadRight(20)).AppendLine("// Active #LFO2");
			buffer.Append(String.Format("#LFO3={0}", 1).PadRight(20)).AppendLine("// Active #LFO3");
			buffer.Append(String.Format("#LFO4={0}", 1).PadRight(20)).AppendLine("// Active #LFO4");
			buffer.Append(String.Format("Voices={0}", 1).PadRight(20)).AppendLine("// Voices=medium");
			buffer.Append(String.Format("Voicing={0}", 0).PadRight(20)).AppendLine("// Voicing");
			buffer.Append(String.Format("Mode={0}", 0).PadRight(20)).AppendLine("// Mode=poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Porta={0:0.00}", 0.00).PadRight(20)).AppendLine("// Portamento");
			buffer.Append(String.Format("PB={0}", 2).PadRight(20)).AppendLine("// PitchBendUp");
			buffer.Append(String.Format("PBD={0}", 2).PadRight(20)).AppendLine("// PitchBendDown");
			buffer.Append(String.Format("ArSc={0}", 4).PadRight(20)).AppendLine("// ArpSync=1/4");
			buffer.Append(String.Format("ArOrd={0}", 0).PadRight(20)).AppendLine("// ArpOrder=by note");
			buffer.Append(String.Format("ArLp={0}", 0).PadRight(20)).AppendLine("// ArpLoop=F -->");
			buffer.Append(String.Format("ArOct={0}", 1).PadRight(20)).AppendLine("// ArpOctave");
			buffer.Append(String.Format("ArLL={0}", 16).PadRight(20)).AppendLine("// ArpLoopLength");
			buffer.Append(String.Format("ArTr={0}", 0).PadRight(20)).AppendLine("// ArpPortamento");
			buffer.Append(String.Format("Drft={0}", 1).PadRight(20)).AppendLine("// Drift");
			buffer.Append(String.Format("MTunS={0}", 0).PadRight(20)).AppendLine("// TuningMode");
			
			buffer.Append(String.Format("MTunN={0}", 5).PadRight(20)).AppendLine("// binary data for Tuning");
			buffer.Append(String.Format("MTunT={0}", 6).PadRight(20)).AppendLine("// binary data for TuningTable");
			
			buffer.Append(String.Format("Trsp={0}", -12).PadRight(20)).AppendLine("// Transpose");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FTun={0:0.00}", 0.00).PadRight(20)).AppendLine("// FineTuneCents");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PortRg={0:0.00}", 100.00).PadRight(20)).AppendLine("// PortaRange");
			buffer.Append(String.Format("PortaM={0}", 0).PadRight(20)).AppendLine("// PortamentoMode=time");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Porta2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Portamento2");
			
			buffer.Append(String.Format("Agte1={0}", 2).PadRight(20)).AppendLine("// Arp Gate1");
			buffer.Append(String.Format("Atrp1={0}", 0).PadRight(20)).AppendLine("// Arp Transpose1=< 12");
			buffer.Append(String.Format("Avoc1={0}", 1).PadRight(20)).AppendLine("// Arp Voices1");
			buffer.Append(String.Format("Amul1={0}", 1).PadRight(20)).AppendLine("// Arp Duration1");
			buffer.Append(String.Format("Amod1={0}", 0).PadRight(20)).AppendLine("// Arp Step Control1=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB1");

			buffer.Append(String.Format("Agte2={0}", 2).PadRight(20)).AppendLine("// Arp Gate2");
			buffer.Append(String.Format("Atrp2={0}", 0).PadRight(20)).AppendLine("// Arp Transpose2=< 12");
			buffer.Append(String.Format("Avoc2={0}", 1).PadRight(20)).AppendLine("// Arp Voices2");
			buffer.Append(String.Format("Amul2={0}", 1).PadRight(20)).AppendLine("// Arp Duration2");
			buffer.Append(String.Format("Amod2={0}", 0).PadRight(20)).AppendLine("// Arp Step Control2=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB2");

			buffer.Append(String.Format("Agte3={0}", 2).PadRight(20)).AppendLine("// Arp Gate3");
			buffer.Append(String.Format("Atrp3={0}", 0).PadRight(20)).AppendLine("// Arp Transpose3=< 12");
			buffer.Append(String.Format("Avoc3={0}", 1).PadRight(20)).AppendLine("// Arp Voices3");
			buffer.Append(String.Format("Amul3={0}", 1).PadRight(20)).AppendLine("// Arp Duration3");
			buffer.Append(String.Format("Amod3={0}", 0).PadRight(20)).AppendLine("// Arp Step Control3=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB3");

			buffer.Append(String.Format("Agte4={0}", 2).PadRight(20)).AppendLine("// Arp Gate4");
			buffer.Append(String.Format("Atrp4={0}", 0).PadRight(20)).AppendLine("// Arp Transpose4=< 12");
			buffer.Append(String.Format("Avoc4={0}", 1).PadRight(20)).AppendLine("// Arp Voices4");
			buffer.Append(String.Format("Amul4={0}", 1).PadRight(20)).AppendLine("// Arp Duration4");
			buffer.Append(String.Format("Amod4={0}", 0).PadRight(20)).AppendLine("// Arp Step Control4=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB4");

			buffer.Append(String.Format("Agte5={0}", 2).PadRight(20)).AppendLine("// Arp Gate5");
			buffer.Append(String.Format("Atrp5={0}", 0).PadRight(20)).AppendLine("// Arp Transpose5=< 12");
			buffer.Append(String.Format("Avoc5={0}", 1).PadRight(20)).AppendLine("// Arp Voices5");
			buffer.Append(String.Format("Amul5={0}", 1).PadRight(20)).AppendLine("// Arp Duration5");
			buffer.Append(String.Format("Amod5={0}", 0).PadRight(20)).AppendLine("// Arp Step Control5=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt5={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB5={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB5");

			buffer.Append(String.Format("Agte6={0}", 2).PadRight(20)).AppendLine("// Arp Gate6");
			buffer.Append(String.Format("Atrp6={0}", 0).PadRight(20)).AppendLine("// Arp Transpose6=< 12");
			buffer.Append(String.Format("Avoc6={0}", 1).PadRight(20)).AppendLine("// Arp Voices6");
			buffer.Append(String.Format("Amul6={0}", 1).PadRight(20)).AppendLine("// Arp Duration6");
			buffer.Append(String.Format("Amod6={0}", 0).PadRight(20)).AppendLine("// Arp Step Control6=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt6={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB6={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB6");

			buffer.Append(String.Format("Agte7={0}", 2).PadRight(20)).AppendLine("// Arp Gate7");
			buffer.Append(String.Format("Atrp7={0}", 0).PadRight(20)).AppendLine("// Arp Transpose7=< 12");
			buffer.Append(String.Format("Avoc7={0}", 1).PadRight(20)).AppendLine("// Arp Voices7");
			buffer.Append(String.Format("Amul7={0}", 1).PadRight(20)).AppendLine("// Arp Duration7");
			buffer.Append(String.Format("Amod7={0}", 0).PadRight(20)).AppendLine("// Arp Step Control7=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt7={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB7={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB7");

			buffer.Append(String.Format("Agte8={0}", 2).PadRight(20)).AppendLine("// Arp Gate8");
			buffer.Append(String.Format("Atrp8={0}", 0).PadRight(20)).AppendLine("// Arp Transpose8=< 12");
			buffer.Append(String.Format("Avoc8={0}", 1).PadRight(20)).AppendLine("// Arp Voices8");
			buffer.Append(String.Format("Amul8={0}", 1).PadRight(20)).AppendLine("// Arp Duration8");
			buffer.Append(String.Format("Amod8={0}", 0).PadRight(20)).AppendLine("// Arp Step Control8=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt8={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB8={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB8");

			buffer.Append(String.Format("Agte9={0}", 2).PadRight(20)).AppendLine("// Arp Gate9");
			buffer.Append(String.Format("Atrp9={0}", 0).PadRight(20)).AppendLine("// Arp Transpose9=< 12");
			buffer.Append(String.Format("Avoc9={0}", 1).PadRight(20)).AppendLine("// Arp Voices9");
			buffer.Append(String.Format("Amul9={0}", 1).PadRight(20)).AppendLine("// Arp Duration9");
			buffer.Append(String.Format("Amod9={0}", 0).PadRight(20)).AppendLine("// Arp Step Control9=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt9={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA9");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB9={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB9");

			buffer.Append(String.Format("Agte10={0}", 2).PadRight(20)).AppendLine("// Arp Gate10");
			buffer.Append(String.Format("Atrp10={0}", 0).PadRight(20)).AppendLine("// Arp Transpose10=< 12");
			buffer.Append(String.Format("Avoc10={0}", 1).PadRight(20)).AppendLine("// Arp Voices10");
			buffer.Append(String.Format("Amul10={0}", 1).PadRight(20)).AppendLine("// Arp Duration10");
			buffer.Append(String.Format("Amod10={0}", 0).PadRight(20)).AppendLine("// Arp Step Control10=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt10={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA10");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB10={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB10");

			buffer.Append(String.Format("Agte11={0}", 2).PadRight(20)).AppendLine("// Arp Gate11");
			buffer.Append(String.Format("Atrp11={0}", 0).PadRight(20)).AppendLine("// Arp Transpose11=< 12");
			buffer.Append(String.Format("Avoc11={0}", 1).PadRight(20)).AppendLine("// Arp Voices11");
			buffer.Append(String.Format("Amul11={0}", 1).PadRight(20)).AppendLine("// Arp Duration11");
			buffer.Append(String.Format("Amod11={0}", 0).PadRight(20)).AppendLine("// Arp Step Control11=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt11={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA11");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB11={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB11");
			buffer.Append(String.Format("Agte12={0}", 2).PadRight(20)).AppendLine("// Arp Gate12");
			buffer.Append(String.Format("Atrp12={0}", 0).PadRight(20)).AppendLine("// Arp Transpose12=< 12");
			buffer.Append(String.Format("Avoc12={0}", 1).PadRight(20)).AppendLine("// Arp Voices12");
			buffer.Append(String.Format("Amul12={0}", 1).PadRight(20)).AppendLine("// Arp Duration12");
			buffer.Append(String.Format("Amod12={0}", 0).PadRight(20)).AppendLine("// Arp Step Control12=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt12={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA12");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB12={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB12");

			buffer.Append(String.Format("Agte13={0}", 2).PadRight(20)).AppendLine("// Arp Gate13");
			buffer.Append(String.Format("Atrp13={0}", 0).PadRight(20)).AppendLine("// Arp Transpose13=< 12");
			buffer.Append(String.Format("Avoc13={0}", 1).PadRight(20)).AppendLine("// Arp Voices13");
			buffer.Append(String.Format("Amul13={0}", 1).PadRight(20)).AppendLine("// Arp Duration13");
			buffer.Append(String.Format("Amod13={0}", 0).PadRight(20)).AppendLine("// Arp Step Control13=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt13={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA13");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB13={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB13");

			buffer.Append(String.Format("Agte14={0}", 2).PadRight(20)).AppendLine("// Arp Gate14");
			buffer.Append(String.Format("Atrp14={0}", 0).PadRight(20)).AppendLine("// Arp Transpose14=< 12");
			buffer.Append(String.Format("Avoc14={0}", 1).PadRight(20)).AppendLine("// Arp Voices14");
			buffer.Append(String.Format("Amul14={0}", 1).PadRight(20)).AppendLine("// Arp Duration14");
			buffer.Append(String.Format("Amod14={0}", 0).PadRight(20)).AppendLine("// Arp Step Control14=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt14={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA14");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB14={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB14");

			buffer.Append(String.Format("Agte15={0}", 2).PadRight(20)).AppendLine("// Arp Gate15");
			buffer.Append(String.Format("Atrp15={0}", 0).PadRight(20)).AppendLine("// Arp Transpose15=< 12");
			buffer.Append(String.Format("Avoc15={0}", 1).PadRight(20)).AppendLine("// Arp Voices15");
			buffer.Append(String.Format("Amul15={0}", 1).PadRight(20)).AppendLine("// Arp Duration15");
			buffer.Append(String.Format("Amod15={0}", 0).PadRight(20)).AppendLine("// Arp Step Control15=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt15={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA15");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB15={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB15");

			buffer.Append(String.Format("Agte16={0}", 2).PadRight(20)).AppendLine("// Arp Gate16");
			buffer.Append(String.Format("Atrp16={0}", 0).PadRight(20)).AppendLine("// Arp Transpose16=< 12");
			buffer.Append(String.Format("Avoc16={0}", 1).PadRight(20)).AppendLine("// Arp Voices16");
			buffer.Append(String.Format("Amul16={0}", 1).PadRight(20)).AppendLine("// Arp Duration16");
			buffer.Append(String.Format("Amod16={0}", 0).PadRight(20)).AppendLine("// Arp Step Control16=next");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt16={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModA16");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB16={0:0.00}", 0.00).PadRight(20)).AppendLine("// Arp Step ModB16");
			
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetEnvelope(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Mode={0}", (int)EnvelopeMode.quadric).PadRight(20)).AppendLine("// Mode=quadric");		// 0 = quadric, 1 = linear, 2 = v-slope
			buffer.Append(String.Format("iMode={0}", (int)EnvelopeInitMode.none).PadRight(20)).AppendLine("// InitMode=none"); 	// 0 = none,  1 = Init, 2 = Delay
			buffer.Append(String.Format("sMode={0}", (int)EnvelopeSustainMode.none).PadRight(20)).AppendLine("// SustainMode=none"); // 0 = none, 1 = Sust2, 2 = LoopA, 3 = LoopD, 4 = LoopS, 5 = Rel25, 6 = Rel50, 7 = Rel75, 8 = Re100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "init={0:0.00}", 0.00).PadRight(20)).AppendLine("// Init");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", 0.00).PadRight(20)).AppendLine("// Attack"); 		// 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dec={0:0.00}", 50.00).PadRight(20)).AppendLine("// Decay"); 		// 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus={0:0.00}", 80.00).PadRight(20)).AppendLine("// Sustain"); 	// 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SusT={0:0.00}", 0.00).PadRight(20)).AppendLine("// Fall/Rise"); 	// -100 - 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Sustain2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", 15.00).PadRight(20)).AppendLine("// Release"); 	// 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", 30.00).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2I={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2A={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2D={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2FR={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2R={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vel2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2I={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2A={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2D={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2FR={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2R={0:0.00}", 0.00).PadRight(20)).AppendLine("// Key2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Slope={0:0.00}", 0.00).PadRight(20)).AppendLine("// Slope"); 	// v-slope -100 - 0 - 100
			buffer.Append(String.Format("TBase={0}", (int)EnvelopeTimeBase.TIMEBASE_8sX).PadRight(20)).AppendLine("// Timebase=8sX");  // 0 = 8sX, 1 = 16sX, 2 = 10s, 3 = 1/4, 4 = 1/1, 5 = 4/1
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetMSEG(string name, int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("TmUn={0}", 1).PadRight(20)).AppendLine("// TimeUnit=Quarters");
			buffer.Append(String.Format("Env={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Envelope");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", 0.00).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", 0.00).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Lpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// Loop");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", 0.00).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format("Trig={0}", 0).PadRight(20)).AppendLine("// Trigger=poly");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetLFO(string description, string name, int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Sync={0}", (int)LFOSync.SYNC_1_4).PadRight(20)).AppendLine("// Sync=1/32");
			buffer.Append(String.Format("Trig={0}", (int)LFOTriggering.free).PadRight(20)).AppendLine("// Restart=free");	// 0 = free, 1 = gate
			buffer.Append(String.Format("Wave={0}", (int)LFOWave.sine).PadRight(20)).AppendLine("// Waveform=sine");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", 0.00).PadRight(20)).AppendLine("// Phase");	// 0 - 100
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", 100.00).PadRight(20)).AppendLine("// Rate"); // 0 - 200
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", 100.00).PadRight(20)).AppendLine("// Amplitude"); // 0 - 100
			buffer.Append(String.Format("Slew={0}", (int)LFOSlew.off).PadRight(20)).AppendLine("// Slew"); // 0 = off, 1 = fast, 2 = slow
			buffer.Append(String.Format("Nstp={0}", 16).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", 0).PadRight(20)).AppendLine("// User Wave Mode=steps");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dly={0:0.00}", 0.00).PadRight(20)).AppendLine("// Delay");	// 0 - 100
			buffer.Append(String.Format("DMS1={0}", (int)LFOModulationSource.none).PadRight(20)).AppendLine("// DepthMod Src1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMD1={0:0.00}", 0.00).PadRight(20)).AppendLine("// DepthMod Dpt1"); // 0 - 100
			buffer.Append(String.Format("FMS1={0}", (int)LFOModulationSource.none).PadRight(20)).AppendLine("// FreqMod Src1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMD1={0:0.00}", 0.00).PadRight(20)).AppendLine("// FreqMod Dpt");	// -100 - 100
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetMMap(string name, int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Mode={0}", 0).PadRight(20)).AppendLine("// Mode=Key");
			buffer.Append(String.Format("MSrc={0}", 0).PadRight(20)).AppendLine("// MSrc=none");
			buffer.Append(String.Format("Stps={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("Num={0}", 17).PadRight(20)).AppendLine("// Number=àf5AÐ‚C");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetMMix(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Type={0}", 0).PadRight(20)).AppendLine("// Mode=sum modulations");
			buffer.Append(String.Format("Mod1={0}", 0).PadRight(20)).AppendLine("// Mod1=none");
			buffer.Append(String.Format("Mod2={0}", 0).PadRight(20)).AppendLine("// Mod2=none");
			buffer.Append(String.Format("Mod3={0}", 0).PadRight(20)).AppendLine("// Mod3=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cst={0:0.00}", 50.00).PadRight(20)).AppendLine("// Constant");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetGrid(string name, int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Grid={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Grid Structure");
			buffer.Append(String.Format("GByp={0}", 0).PadRight(20)).AppendLine("// Bypass");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetOscillator(string description, string name, int binaryDataCurveIndex, int binaryDataKeyVelZonesIndex, int binaryDataWaveTableIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Wave={0}", 0).PadRight(20)).AppendLine("// WaveForm=GeoMorph");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", 0.00).PadRight(20)).AppendLine("// Tune"); // -48.00 - 48.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", 100.00).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// TuneModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// TuneModDepth"); // -48.00 - 48.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", 0.00).PadRight(20)).AppendLine("// Phase"); // 0.00 - 100.00
			buffer.Append(String.Format("PhsMSrc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// PhaseModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhsMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// PhaseModDepth"); // -50.00 - 50.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WNum={0:0.00}", 1.00).PadRight(20)).AppendLine("// WaveWarp"); // 1.00 - 16.00
			buffer.Append(String.Format("WPSrc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// WarpModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WPDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// WarpModDepth"); // -16.00 - 16.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vibrato"); // Vibrato: 0.00 - 100.00
			buffer.Append(String.Format("Curve={0}", binaryDataCurveIndex).PadRight(20)).AppendLine("// binary data for Curve");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Prec={0:0.00}", 5.00).PadRight(20)).AppendLine("// Resolution");
			buffer.Append(String.Format("FX1Tp={0}", (int)OscillatorEffect.none).PadRight(20)).AppendLine("// SpectraFX1 Type=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX1={0:0.00}", 0.00).PadRight(20)).AppendLine("// SpectraFX1 Val"); // -100.00 - 100.00
			buffer.Append(String.Format("FX1Sc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// SFX1ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX1Dt={0:0.00}", 0.00).PadRight(20)).AppendLine("// SFX1ModDepth"); 	// -100.00 - 100.00
			buffer.Append(String.Format("FX2Tp={0}", (int)OscillatorEffect.none).PadRight(20)).AppendLine("// SpectraFX2 Type=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX2={0:0.00}", 0.00).PadRight(20)).AppendLine("// SpectraFX2 Val"); // -100.00 - 100.00
			buffer.Append(String.Format("FX2Sc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// SFX2ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX2Dt={0:0.00}", 0.00).PadRight(20)).AppendLine("// SFX2ModDepth"); 	// -100.00 - 100.00
			buffer.Append(String.Format("Poly={0}", (int)OscillatorPoly.single).PadRight(20)).AppendLine("// PolyWave=single");	// 0 = single, 1 = dual, 2 = quad, 3 = eleven
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", 0.00).PadRight(20)).AppendLine("// Detune"); 		// -50.00 - 50.00
			buffer.Append(String.Format("KVsc={0}", binaryDataKeyVelZonesIndex).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", 100.00).PadRight(20)).AppendLine("// Volume");		// 0.00 - 200.00
			buffer.Append(String.Format("VolSc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// VolumeModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// VolumeModDepth");// -100.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan"); 			// -100.00 - 100.00
			buffer.Append(String.Format("PanSc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// PanModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// PanModDepth");	// -100.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sync={0:0.00}", 0.00).PadRight(20)).AppendLine("// SyncTune");		// 0.00 - 36.00
			buffer.Append(String.Format("SncSc={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// SyncModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SncDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// SyncModDepth"); 	// -36.00 - 36.00
			buffer.Append(String.Format("SncOn={0}", (int)OnOff.off).PadRight(20)).AppendLine("// Sync Active");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", 50.00).PadRight(20)).AppendLine("// Poly Width");	// 0.00 - 100.00
			buffer.Append(String.Format("PwmOn={0}", (int)OnOff.off).PadRight(20)).AppendLine("// PWM Mode");
			buffer.Append(String.Format("WaTb={0}", binaryDataWaveTableIndex).PadRight(20)).AppendLine("// binary data for WaveTable");
			buffer.Append(String.Format("RePhs={0}", (int)OnOff.off).PadRight(20)).AppendLine("// Reset Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Norm={0:0.00}", 0.00).PadRight(20)).AppendLine("// Normalize");
			buffer.Append(String.Format("Rend={0}", 0).PadRight(20)).AppendLine("// Renderer=soft");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetNoise(string name, int binaryDataKeyVelZonesIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Type={0}", 0).PadRight(20)).AppendLine("// Type=White");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1={0:0.00}", 100.00).PadRight(20)).AppendLine("// Filter1");
			buffer.Append(String.Format("F1Src={0}", 0).PadRight(20)).AppendLine("// F1 ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1Dpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// F1 ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Filter2");
			buffer.Append(String.Format("F2Src={0}", 0).PadRight(20)).AppendLine("// F2 ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2Dpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// F2 ModDepth");
			buffer.Append(String.Format("KVsc={0}", binaryDataKeyVelZonesIndex).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", 100.00).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", 0).PadRight(20)).AppendLine("// VolumeModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", 0).PadRight(20)).AppendLine("// PanModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", 0).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", 50.00).PadRight(20)).AppendLine("// Width");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetFilter(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Typ={0}", (int)FilterType.LP_Xcite).PadRight(20)).AppendLine("// Type=LP Xcite");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", 150.00).PadRight(20)).AppendLine("// Cutoff"); 	// 0.00 - 150.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", 0.00).PadRight(20)).AppendLine("// Resonance");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", 0.00).PadRight(20)).AppendLine("// Drive");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", 0.00).PadRight(20)).AppendLine("// Gain");		// (for the EQs) -24.00 - 24.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", 0.00).PadRight(20)).AppendLine("// ModDepth1");	// -150.00 - 150.00
			buffer.Append(String.Format("FS1={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// Modsource1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", 0.00).PadRight(20)).AppendLine("// ModDepth2");	// -150.00 - 150.00
			buffer.Append(String.Format("FS2={0}", (int)ModulationSource.none).PadRight(20)).AppendLine("// Modsource2=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", 0.00).PadRight(20)).AppendLine("// KeyFollow");// 0.00 - 100.00
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetFMO(string name, int binaryDataKeyVelZonesIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Wave={0}", 0).PadRight(20)).AppendLine("// Mode=FM by Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", 0.00).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", 100.00).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", 0).PadRight(20)).AppendLine("// TuneModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM={0:0.00}", 0.00).PadRight(20)).AppendLine("// FM Depth");
			buffer.Append(String.Format("FMSrc={0}", 0).PadRight(20)).AppendLine("// FM ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// FM ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", 0.00).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", binaryDataKeyVelZonesIndex).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", 100.00).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", 0).PadRight(20)).AppendLine("// VolumeModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", 0).PadRight(20)).AppendLine("// PanModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", 0).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", 50.00).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Getr={0}", 0).PadRight(20)).AppendLine("// Generator=pure sine");
			buffer.AppendLine();
			
			return buffer.ToString();
		}

		private string GeneratePresetComb(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Mode={0}", 0).PadRight(20)).AppendLine("// Mode=Comb");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", 0.00).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", 100.00).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", 0).PadRight(20)).AppendLine("// TuneModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Detn={0:0.00}", 0.00).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", 0.00).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", 0.00).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format("FBSrc={0}", 0).PadRight(20)).AppendLine("// FBModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FBDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// FBModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Damp={0:0.00}", 0.00).PadRight(20)).AppendLine("// Damp");
			buffer.Append(String.Format("DmpSrc={0}", 0).PadRight(20)).AppendLine("// DampModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DmpDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// DampModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Exc={0:0.00}", 0.00).PadRight(20)).AppendLine("// PreFill");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Inj={0:0.00}", 0.00).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format("InjSrc={0}", 0).PadRight(20)).AppendLine("// InModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "InjDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// InputMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tne={0:0.00}", 50.00).PadRight(20)).AppendLine("// Tone");
			buffer.Append(String.Format("TneSrc={0}", 0).PadRight(20)).AppendLine("// ToneModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TneDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// ToneMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sec={0:0.00}", 0.00).PadRight(20)).AppendLine("// Flavour");
			buffer.Append(String.Format("SecSrc={0}", 0).PadRight(20)).AppendLine("// SecModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SecDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// FlavourMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dist={0:0.00}", 0.00).PadRight(20)).AppendLine("// Distortion");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dry={0:0.00}", 0.00).PadRight(20)).AppendLine("// Dry");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", 100.00).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", 0).PadRight(20)).AppendLine("// VolumeModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", 0).PadRight(20)).AppendLine("// PanModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", 0.00).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", 0).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", 50.00).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Fill={0}", 0).PadRight(20)).AppendLine("// Fill=Noise");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetShape(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Type={0}", 0).PadRight(20)).AppendLine("// Type=Shape");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Depth={0:0.00}", 0.00).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format("DMSrc={0}", 0).PadRight(20)).AppendLine("// D_ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// D_ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Edge={0:0.00}", 75.00).PadRight(20)).AppendLine("// Edge");
			buffer.Append(String.Format("EMSrc={0}", 0).PadRight(20)).AppendLine("// Edge ModSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// Edge ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", 0.00).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", 0.00).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HiOut={0:0.00}", 0.00).PadRight(20)).AppendLine("// HiOut");
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetChannelMix(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", 50.00).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", 0).PadRight(20)).AppendLine("// Pan Mode=Bal L-R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", 0.00).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", 0).PadRight(20)).AppendLine("// PanMod Source=none");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetXMF(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Typ={0}", 0).PadRight(20)).AppendLine("// Type=LP4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", 150.00).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", 0.00).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Freq mod 1");
			buffer.Append(String.Format("FS1={0}", 0).PadRight(20)).AppendLine("// Modsource1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Freq mod 2");
			buffer.Append(String.Format("FS2={0}", 0).PadRight(20)).AppendLine("// Modsource2=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", 0.00).PadRight(20)).AppendLine("// KeyFollow");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOff={0:0.00}", 0.00).PadRight(20)).AppendLine("// FreqOffset");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOMod={0:0.00}", 0.00).PadRight(20)).AppendLine("// FreqOffMod");
			buffer.Append(String.Format("FOSrc={0}", 0).PadRight(20)).AppendLine("// FreqOffSrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFM={0:0.00}", 0.00).PadRight(20)).AppendLine("// FilterFM");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFMD={0:0.00}", 0.00).PadRight(20)).AppendLine("// XFMmod");
			buffer.Append(String.Format("XFMS={0}", 0).PadRight(20)).AppendLine("// XFMrc=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Bias={0:0.00}", 0.00).PadRight(20)).AppendLine("// Bias");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OLoad={0:0.00}", 0.00).PadRight(20)).AppendLine("// Overload");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Click={0:0.00}", 0.00).PadRight(20)).AppendLine("// Click");
			buffer.Append(String.Format("Drv={0}", 0).PadRight(20)).AppendLine("// Driver=XMF");
			buffer.Append(String.Format("Rout={0}", 0).PadRight(20)).AppendLine("// Routing=single");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Typ2={0:0.00}", -1).PadRight(20)).AppendLine("// Type2=");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetSideBand(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name.ToUpper());
			buffer.Append(String.Format("Range={0}", 0).PadRight(20)).AppendLine("// Range=10Hz");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Freq={0:0.00}", 0.00).PadRight(20)).AppendLine("// Frequency");
			buffer.Append(String.Format("FMSrc={0}", 0).PadRight(20)).AppendLine("// FModSource=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// FModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Offs={0:0.00}", 0.00).PadRight(20)).AppendLine("// Offset");
			buffer.Append(String.Format("OMSrc={0}", 0).PadRight(20)).AppendLine("// OModSource=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// OModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", 50.00).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("MMSrc={0}", 0).PadRight(20)).AppendLine("// MModSource=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMDpt={0:0.00}", 0.00).PadRight(20)).AppendLine("// MModDepth");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetVoiceMix() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	VoiceMix");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=VCA1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan1");
			buffer.Append(String.Format("PanMS1={0}", 0).PadRight(20)).AppendLine("// Pan Mod Src1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan Mod Dpt1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol1={0:0.00}", 50.00).PadRight(20)).AppendLine("// Volume1");
			buffer.Append(String.Format("VCA1={0}", 1).PadRight(20)).AppendLine("// VCA1=Env1");
			buffer.Append(String.Format("ModSrc1={0}", 0).PadRight(20)).AppendLine("// Modulation1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mod Depth1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan2");
			buffer.Append(String.Format("PanMS2={0}", 0).PadRight(20)).AppendLine("// Pan Mod Src2=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan Mod Dpt2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol2={0:0.00}", 50.00).PadRight(20)).AppendLine("// Volume2");
			buffer.Append(String.Format("VCA2={0}", 1).PadRight(20)).AppendLine("// VCA2=Env1");
			buffer.Append(String.Format("ModSrc2={0}", 0).PadRight(20)).AppendLine("// Modulation2=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mod Depth2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan3");
			buffer.Append(String.Format("PanMS3={0}", 0).PadRight(20)).AppendLine("// Pan Mod Src3=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan Mod Dpt3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol3={0:0.00}", 50.00).PadRight(20)).AppendLine("// Volume3");
			buffer.Append(String.Format("VCA3={0}", 1).PadRight(20)).AppendLine("// VCA3=Env1");
			buffer.Append(String.Format("ModSrc3={0}", 0).PadRight(20)).AppendLine("// Modulation3=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mod Depth3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan4");
			buffer.Append(String.Format("PanMS4={0}", 0).PadRight(20)).AppendLine("// Pan Mod Src4=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Pan Mod Dpt4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol4={0:0.00}", 50.00).PadRight(20)).AppendLine("// Volume4");
			buffer.Append(String.Format("VCA4={0}", 1).PadRight(20)).AppendLine("// VCA4=Env1");
			buffer.Append(String.Format("ModSrc4={0}", 0).PadRight(20)).AppendLine("// Modulation4=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mod Depth4");
			buffer.Append(String.Format("MT1={0}", 0).PadRight(20)).AppendLine("// Mute1");
			buffer.Append(String.Format("MT2={0}", 0).PadRight(20)).AppendLine("// Mute2");
			buffer.Append(String.Format("MT3={0}", 0).PadRight(20)).AppendLine("// Mute3");
			buffer.Append(String.Format("MT4={0}", 0).PadRight(20)).AppendLine("// Mute4");
			buffer.Append(String.Format("PB1={0}", 0).PadRight(20)).AppendLine("// Panning1=Pan");
			buffer.Append(String.Format("PB2={0}", 0).PadRight(20)).AppendLine("// Panning2=Pan");
			buffer.Append(String.Format("PB3={0}", 0).PadRight(20)).AppendLine("// Panning3=Pan");
			buffer.Append(String.Format("PB4={0}", 0).PadRight(20)).AppendLine("// Panning4=Pan");
			buffer.Append(String.Format("Bus1={0}", 0).PadRight(20)).AppendLine("// Bus1=main");
			buffer.Append(String.Format("Bus2={0}", 0).PadRight(20)).AppendLine("// Bus2=main");
			buffer.Append(String.Format("Bus3={0}", 0).PadRight(20)).AppendLine("// Bus3=main");
			buffer.Append(String.Format("Bus4={0}", 0).PadRight(20)).AppendLine("// Bus4=main");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Send1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Send1");
			buffer.Append(String.Format("SnSrc1={0}", 0).PadRight(20)).AppendLine("// SendMod1=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SnDpt1={0:0.00}", 0.00).PadRight(20)).AppendLine("// SendDepth1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Send2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Send2");
			buffer.Append(String.Format("SnSrc2={0}", 0).PadRight(20)).AppendLine("// SendMod2=none");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SnDpt2={0:0.00}", 0.00).PadRight(20)).AppendLine("// SendDepth2");
			buffer.Append(String.Format("AttS={0}", 1).PadRight(20)).AppendLine("// AttackSmooth");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetFXGrid(int binaryDataIndex) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	FXGrid");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=GridFX");
			buffer.Append(String.Format("Grid={0}", binaryDataIndex).PadRight(20)).AppendLine("// binary data for Grid Structure");
			buffer.Append(String.Format("GByp={0}", 0).PadRight(20)).AppendLine("// Bypass");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetModFX(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Mode={0}", 0).PadRight(20)).AppendLine("// Mode=Chorus");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cent={0:0.00}", 20.00).PadRight(20)).AppendLine("// Center");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sped={0:0.00}", 50.00).PadRight(20)).AppendLine("// Speed");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhOff={0:0.00}", 50.00).PadRight(20)).AppendLine("// Stereo");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dpth={0:0.00}", 50.00).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FeeB={0:0.00}", 0.00).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LCut={0:0.00}", 0.00).PadRight(20)).AppendLine("// LowCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HCut={0:0.00}", 100.00).PadRight(20)).AppendLine("// HiCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Quad={0:0.00}", 0.00).PadRight(20)).AppendLine("// Quad");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Qphs={0:0.00}", 25.00).PadRight(20)).AppendLine("// QuadPhase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Leq={0:0.00}", 0.00).PadRight(20)).AppendLine("// Low Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Heq={0:0.00}", 0.00).PadRight(20)).AppendLine("// High Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Q1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Q2");
			buffer.Append(String.Format("EQon={0}", 1).PadRight(20)).AppendLine("// EQ");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetDelay(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Mode={0}", (int)DelayMode.stereo_2).PadRight(20)).AppendLine("// Mode=stereo 2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", 0.00).PadRight(20)).AppendLine("// Mix");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", 10.00).PadRight(20)).AppendLine("// Feedback");// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "CB={0:0.00}", 25.00).PadRight(20)).AppendLine("// X-back"); 	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LP={0:0.00}", 100.00).PadRight(20)).AppendLine("// Lowpass");// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HP={0:0.00}", 0.00).PadRight(20)).AppendLine("// Hipass"); 	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", 0.00).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format("Sync1={0}", (int)DelaySync.Delay_1_4).PadRight(20)).AppendLine("// Sync1=1/4");
			buffer.Append(String.Format("Sync2={0}", (int)DelaySync.Delay_1_4).PadRight(20)).AppendLine("// Sync2=1/4");
			buffer.Append(String.Format("Sync3={0}", (int)DelaySync.Delay_1_4).PadRight(20)).AppendLine("// Sync3=1/4");
			buffer.Append(String.Format("Sync4={0}", (int)DelaySync.Delay_1_4).PadRight(20)).AppendLine("// Sync4=1/4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T0={0:0.00}", 100.00).PadRight(20)).AppendLine("// Ratio1");	// 0.00 - 200.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T1={0:0.00}", 100.00).PadRight(20)).AppendLine("// Ratio2");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T2={0:0.00}", 100.00).PadRight(20)).AppendLine("// Ratio3");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T3={0:0.00}", 100.00).PadRight(20)).AppendLine("// Ratio4");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan1={0:0.00}", -100.00).PadRight(20)).AppendLine("// Pan1");// -100.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan2={0:0.00}", 100.00).PadRight(20)).AppendLine("// Pan2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan3={0:0.00}", -100.00).PadRight(20)).AppendLine("// Pan3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan4={0:0.00}", 100.00).PadRight(20)).AppendLine("// Pan4");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		enum ReverbMode : int {
			Reverb = 0,
			MetalVerb = 1
		}
		
		private string GeneratePresetReverb(string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + name);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Mode={0}", (int)ReverbMode.Reverb).PadRight(20)).AppendLine("// Mode=Reverb");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dry={0:0.00}", 82.00).PadRight(20)).AppendLine("// Dry");			// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Wet={0:0.00}", 52.00).PadRight(20)).AppendLine("// Wet");			// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", 43.00).PadRight(20)).AppendLine("// Feedback");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Damp={0:0.00}", 4.00).PadRight(20)).AppendLine("// Damp");			// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Size={0:0.00}", 75.00).PadRight(20)).AppendLine("// Range");			// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Spd={0:0.00}", 50.00).PadRight(20)).AppendLine("// Speed");			// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dpt={0:0.00}", 50.00).PadRight(20)).AppendLine("// Modulation");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DFB={0:0.00}", 70.00).PadRight(20)).AppendLine("// Diff Feedback");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DSize={0:0.00}", 50.00).PadRight(20)).AppendLine("// Diff Range"); 	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMix={0:0.00}", 100.00).PadRight(20)).AppendLine("// Diff Mix");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMod={0:0.00}", 50.00).PadRight(20)).AppendLine("// Diff Mod");		// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DSpd={0:0.00}", 55.00).PadRight(20)).AppendLine("// Diff Speed");	// 0.00 - 100.00
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pre={0:0.00}", 0.00).PadRight(20)).AppendLine("// PreDelay");		// 0.00 - 250.00
			buffer.AppendLine();
			return buffer.ToString();
		}

		private string GeneratePresetCompressor(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format("Type={0}", 0).PadRight(20)).AppendLine("// Type=Eco");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rat={0:0.00}", 100.00).PadRight(20)).AppendLine("// Compression");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Thres={0:0.00}", -20.00).PadRight(20)).AppendLine("// Threshold");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Att={0:0.00}", 30.00).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", 50.00).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", 0.00).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", 0.00).PadRight(20)).AppendLine("// Output");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetEqualizer(string description, string name) {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	" + description);
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=" + name);
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc1={0:0.00}", 20.00).PadRight(20)).AppendLine("// Freq LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res1={0:0.00}", 25.00).PadRight(20)).AppendLine("// Q LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Gain LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc2={0:0.00}", 40.00).PadRight(20)).AppendLine("// Freq Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res2={0:0.00}", 25.00).PadRight(20)).AppendLine("// Q Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Gain Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc3={0:0.00}", 60.00).PadRight(20)).AppendLine("// Freq Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res3={0:0.00}", 25.00).PadRight(20)).AppendLine("// Q Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain3={0:0.00}", 0.00).PadRight(20)).AppendLine("// Gain Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc4={0:0.00}", 80.00).PadRight(20)).AppendLine("// Freq HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res4={0:0.00}", 25.00).PadRight(20)).AppendLine("// Q HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain4={0:0.00}", 0.00).PadRight(20)).AppendLine("// Gain HiShelf");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetMaster() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine("/*");
			buffer.AppendLine("	Master");
			buffer.AppendLine("*/");
			buffer.AppendLine("#cm=ZMas");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Ret1={0:0.00}", 0.00).PadRight(20)).AppendLine("// Return1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Ret2={0:0.00}", 0.00).PadRight(20)).AppendLine("// Return2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mast={0:0.00}", 100.00).PadRight(20)).AppendLine("// Master");
			buffer.Append(String.Format("XY1L={0}", 37).PadRight(20)).AppendLine("// binary data for XY1 Label");
			buffer.Append(String.Format("XY2L={0}", 38).PadRight(20)).AppendLine("// binary data for XY2 Label");
			buffer.Append(String.Format("XY3L={0}", 39).PadRight(20)).AppendLine("// binary data for XY3 Label");
			buffer.Append(String.Format("XY4L={0}", 40).PadRight(20)).AppendLine("// binary data for XY4 Label");
			buffer.Append(String.Format("XY1T={0}", 41).PadRight(20)).AppendLine("// binary data for XY1 Text");
			buffer.Append(String.Format("XY2T={0}", 42).PadRight(20)).AppendLine("// binary data for XY2 Text");
			buffer.Append(String.Format("XY3T={0}", 43).PadRight(20)).AppendLine("// binary data for XY3 Text");
			buffer.Append(String.Format("XY4T={0}", 44).PadRight(20)).AppendLine("// binary data for XY4 Text");
			buffer.Append(String.Format("OSC1={0}", 45).PadRight(20)).AppendLine("// binary data for Oscillator1");
			buffer.Append(String.Format("OSC2={0}", 46).PadRight(20)).AppendLine("// binary data for Oscillator2");
			buffer.Append(String.Format("OSC3={0}", 47).PadRight(20)).AppendLine("// binary data for Oscillator3");
			buffer.Append(String.Format("OSC4={0}", 48).PadRight(20)).AppendLine("// binary data for Oscillator4");
			buffer.Append(String.Format("MSEG1={0}", 49).PadRight(20)).AppendLine("// binary data for MSEG1");
			buffer.Append(String.Format("MSEG2={0}", 50).PadRight(20)).AppendLine("// binary data for MSEG2");
			buffer.Append(String.Format("MSEG3={0}", 51).PadRight(20)).AppendLine("// binary data for MSEG3");
			buffer.Append(String.Format("MSEG4={0}", 52).PadRight(20)).AppendLine("// binary data for MSEG4");
			buffer.Append(String.Format("Rev1={0}", 53).PadRight(20)).AppendLine("// binary data for Rev1");
			buffer.Append(String.Format("Pn3={0}", 0).PadRight(20)).AppendLine("// FXPaneMem");
			buffer.Append(String.Format("Pn4={0}", 0).PadRight(20)).AppendLine("// OSC1PaneMem");
			buffer.Append(String.Format("Pn5={0}", 0).PadRight(20)).AppendLine("// OSC2PaneMem");
			buffer.Append(String.Format("Pn6={0}", 0).PadRight(20)).AppendLine("// OSC3PaneMem");
			buffer.Append(String.Format("Pn7={0}", 0).PadRight(20)).AppendLine("// OSC4PaneMem");
			buffer.Append(String.Format("Pn8={0}", 0).PadRight(20)).AppendLine("// VCF1PaneMem");
			buffer.Append(String.Format("Pn9={0}", 0).PadRight(20)).AppendLine("// VCF2PaneMem");
			buffer.Append(String.Format("Pn10={0}", 0).PadRight(20)).AppendLine("// VCF3PaneMem");
			buffer.Append(String.Format("Pn11={0}", 0).PadRight(20)).AppendLine("// VCF4PaneMem");
			buffer.Append(String.Format("Rack0={0}", 54).PadRight(20)).AppendLine("// binary data for MainRackMem");
			buffer.Append(String.Format("Rack1={0}", 55).PadRight(20)).AppendLine("// binary data for ModRackMem");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		private string GeneratePresetUglyCompressedBinaryData() {
			StringBuilder buffer = new StringBuilder();
			buffer.AppendLine();
			buffer.AppendLine();
			buffer.AppendLine();
			buffer.AppendLine("// Section for ugly compressed binary Data");
			buffer.AppendLine("// DON'T TOUCH THIS");
			buffer.AppendLine();
			buffer.AppendLine("$$$$75112");
			buffer.AppendLine("?aaaaiadp:jkjjjjlo:aaaaialp:aaaaaadp:gdgceneb:aaaaaaea:baaaaaaa:");
			buffer.AppendLine("caaaaaaa:aagbhjgf:gegfgggb:aa:eb:ec:ca:ea:dc:db:gf:dp:lp:ak:gp:d");
			buffer.AppendLine("o:he:en:gb:go:ia:lo:ef:fi:gm:gj:hc:ha:ae!uAabA9emLgdPVAnknodiKbp");
			buffer.AppendLine("cclaceanDbhA2biSboccbhA2RAcjccbgUonxAjindbocclipibcAjindboccbeck");
			buffer.AppendLine("okDnmpphdcbEBhncbA4RaiegA4UlbTA1jgedA1maEA1gdedA1miBA1jgedA1REAR");
			buffer.AppendLine("kjedA2CA1jgedA1maEAmabheeA1REA2CA2CAmabpeeA1REAnijbclcipjbcAlngl");
			buffer.AppendLine("onDaipjbcAjanjanDbkkeopDmcafA9oickfcDabA2lepmbcAlbbodgaipp3Apmbc");
			buffer.AppendLine("AjlaipaDgiIanDckA18DXopDcmcjNcbdhkiopDcmcjNcbA3janjanDuEA10WQWNW");
			buffer.AppendLine("PVWhkHcogiFYADbhA2biSboccbhA2RAcjccbgUonxAjindbocclipibcAjindboc");
			buffer.AppendLine("cbeckokDnmpphdcbEBhncbuEA74uRA6qA127uRA6qA127uEA10zhfVNDhdgdPVHA");
			buffer.AppendLine("22qvA1E1A1REA1kaEA1maEA1oaEuAacA5qA3qvA1E1A1REA1kaEA1maEA1oaEA2B");
			buffer.AppendLine("A1baBA1DBA1daBA1EBA1faBA1gaBA1YBA1RBA1iiBA1jaBA1jiBA1kaBA1kiBA1l");
			buffer.AppendLine("aBA1liBA1maBA1miBA1naBA1niBA1oaBA1oiBA1paBA1piBA2CA1ZCA1aiCA1amC");
			buffer.AppendLine("A1baCA1beCA1biCA1bmCA1DCA1ceCA1ciCA1cmCA1daCA1deCA1diCA1dmCA1ECA");
			buffer.AppendLine("1eeCA1eiCA1emCA1faCA1feCA1UCA1fmCA1gaCA1geCA1giCA1VCA1YCA1NCA1hi");
			buffer.AppendLine("CA1hmCA1RCA1icCA1ieCA1igCA1iiCA1ikCA1imCA1ioCA1jaCA1jcCA1jeCA1jg");
			buffer.AppendLine("CA1jiCA1jkCA1jmCA1joCA1kaCA1kcCA1keCA1kgCA1kiCA1kkCA1kmCA1koCA1l");
			buffer.AppendLine("aCA1lcCA1leCA1lgCA1liCA1lkCA1lmCA1SCA1maCA1mcCA1meCA1mgCA1miCA1m");
			buffer.AppendLine("kCA1mmCA1moCA1naCA1ncCA1neCA1ngCA1niCA1nkCA1nmCA1noCA1oaCA1ocCA1");
			buffer.AppendLine("oeCA1ogCA1oiCA1okCA1omCA1ooCA1paCA1pcCA1peCA1pgCA1piCA1pkCA1pmCA");
			buffer.AppendLine("1poCudaacA5qA3wZA2aiA2abZA25rqA7rvA7rA1E1A7rA1REA7rA1kaEA7rA1maE");
			buffer.AppendLine("A7rA1oaEA7rA2BA7rA1baBA7rA1DBA7rA1daBA7rA1EBA7rA1faBA7rA1gaBA7rA");
			buffer.AppendLine("1YBA7rA1RBA7rA1iiBA7rA1jaBA7rA1jiBA7rA1kaBA7rA1kiBA7rA1laBA7rA1l");
			buffer.AppendLine("iBA7rA1maBA7rA1miBA7rA1naBA7rA1niBA7rA1oaBA7rA1oiBA7rA1paBA7rA1p");
			buffer.AppendLine("iBA7rA15udaacA5qA3wZA2aiA2abZA25rqA7rvA7rA1E1A7rA1REA7rA1kaEA7rA");
			buffer.AppendLine("1maEA7rA1oaEA7rA2BA7rA1baBA7rA1DBA7rA1daBA7rA1EBA7rA1faBA7rA1gaB");
			buffer.AppendLine("A7rA1YBA7rA1RBA7rA1iiBA7rA1jaBA7rA1jiBA7rA1kaBA7rA1kiBA7rA1laBA7");
			buffer.AppendLine("rA1liBA7rA1maBA7rA1miBA7rA1naBA7rA1niBA7rA1oaBA7rA1oiBA7rA1paBA7");
			buffer.AppendLine("rA1piBA7rA15udaacA5qA3wZA2aiA2abZA25rqA7rvA7rA1E1A7rA1REA7rA1kaE");
			buffer.AppendLine("A7rA1maEA7rA1oaEA7rA2BA7rA1baBA7rA1DBA7rA1daBA7rA1EBA7rA1faBA7rA");
			buffer.AppendLine("1gaBA7rA1YBA7rA1RBA7rA1iiBA7rA1jaBA7rA1jiBA7rA1kaBA7rA1kiBA7rA1l");
			buffer.AppendLine("aBA7rA1liBA7rA1maBA7rA1miBA7rA1naBA7rA1niBA7rA1oaBA7rA1oiBA7rA1p");
			buffer.AppendLine("aBA7rA1piBA7rA15udaacA5qA3wZA2aiA2abZA25rqA7rvA7rA1E1A7rA1REA7rA");
			buffer.AppendLine("1kaEA7rA1maEA7rA1oaEA7rA2BA7rA1baBA7rA1DBA7rA1daBA7rA1EBA7rA1faB");
			buffer.AppendLine("A7rA1gaBA7rA1YBA7rA1RBA7rA1iiBA7rA1jaBA7rA1jiBA7rA1kaBA7rA1kiBA7");
			buffer.AppendLine("rA1laBA7rA1liBA7rA1maBA7rA1miBA7rA1naBA7rA1niBA7rA1oaBA7rA1oiBA7");
			buffer.AppendLine("rA1paBA7rA1piBA7rA15uRA6qA127uRA6qA127uRA6qA127uRA6qA127uAacA5qb");
			buffer.AppendLine("jbhVIkibbbgJnlkhhhShdjlmjSVeijoMlnflcgSmilfibMlfdfEJkddngiJlkjef");
			buffer.AppendLine("ednhjmkaoSeicnaiIpgpjhgdnXgnohMehUQIdakfTJekjejflngdgnKJq1jlabip");
			buffer.AppendLine("SmbpgcoIopikpjMibbcfkJGlicpImlnehiJegalacIkajkbfIpmFhdSLlneiSLfo");
			buffer.AppendLine("GIghpjTMbkPdjJbcVillmfggndadnilhoelJqbhkcmnSnebhFJfgadoclnknnoLJ");
			buffer.AppendLine("lekfcdJocolpoMcjOgeMjnlhfdIAgdZIclipLJnlelpoMojajpnMdnSciJgkieHI");
			buffer.AppendLine("bejbamMeiimnnSoflhlgdnXadagJdhSagJbnegklMJmgilMlgfmXJldojdjIphok");
			buffer.AppendLine("hmIkhlfjidnpjehjkMRjecdJcoHdlJinlafmIhhcdjpSnnbgkglnbjWhoJSgdemJ");
			buffer.AppendLine("lbnmfdIneaielInnQmfMifbonglmlgmndiMiolgehJgkieHInjldbbMjiY1Sodad");
			buffer.AppendLine("abIbkPdjJahphiiMeddgofShfipjfSjbhdogMldkfaldnSoidnInkigdmdnKYooM");
			buffer.AppendLine("dgmkKJhjjpcjJlhgnkgMFadanMbkmcbfIohelpcdnie1ppMoacbaiJmoanGJdihn");
			buffer.AppendLine("ihMocFjpdnofolMIamcikjllbpYpkdnidnmchJehbmbiJpcdjkodnancgbbSohmi");
			buffer.AppendLine("AIogpiicSjlbmblSfccphkJphdeCIjkoclm1QlnaiJehUQIkmcmdgJDnhmfSfhMe");
			buffer.AppendLine("jIgknifcI1ikjmSNonfaJpkUpfSkkcphnIsnglhdaMdfGjilnoamdaiIlnehibMu");
			buffer.AppendLine("AacA5qA511uamadA5qeeejfcehZA2amA2epfdedGyanbbA1eoLWhdHGA1aobbA1e");
			buffer.AppendLine("pfdedddA3aobeA1epfdeddeADGdiaobiA1epfdedFA3aobbA1eoLWhdHFA1aobcA");
			buffer.AppendLine("1fgedegGA3acbeA1fgedegFA3acbiA1fgedegddyacbbA1fgedegdeA3acbcA1eg");
			buffer.AppendLine("OepGA3aobeA1egOepFADFGaobiA1egOepddA3aobbA1egOepdeA3aobcA1edLgng");
			buffer.AppendLine("cGA2KbeA1edLgngcFA2KbiA1fdgiPYHGAHKbbA1fdgiPYHFA1KbcA1OWhiGA3KNA");
			buffer.AppendLine("1OWhiFADFdeKhiA1OWhiddA3KhbA1OWhideA3KXA1fcWQghGA2KNA1fcWQghFA2K");
			buffer.AppendLine("hiA1UOegGyKhbA1UOegFA3KXA1fdCGA4KbeA1fdCFAXDFdhKbiA26DFdiA23emPh");
			buffer.AppendLine("jHA27yXDdddaA28DddGA23emPhjHA31xA243uZxA2vA7tqA119tqA119tqA119tq");
			buffer.AppendLine("A119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119");
			buffer.AppendLine("tqA119tqA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3q");
			buffer.AppendLine("A115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3q");
			buffer.AppendLine("A115sA3qA4207ZA2uwA3vUemdmcehp3gaeidabihp3uAxA2vA8191uZxA2vA7tqA");
			buffer.AppendLine("119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119t");
			buffer.AppendLine("qA119tqA119tqA119tqA119tqA115sA3qA115sA3qA115sA3qA115sA3qA115sA3");
			buffer.AppendLine("qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3");
			buffer.AppendLine("qA115sA3qA115sA3qA115sA3qA4207ZA2uwA3vUemdmcehp3gaeidabihp3uAxA2");
			buffer.AppendLine("vA8191uZxA2vA7tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tq");
			buffer.AppendLine("A119tqA119tqA119tqA119tqA119tqA119tqA119tqA115sA3qA115sA3qA115sA");
			buffer.AppendLine("3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA");
			buffer.AppendLine("3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA4207ZA2uwA3vUemdmcehp");
			buffer.AppendLine("3gaeidabihp3uAxA2vA8191uZxA2vA7tqA119tqA119tqA119tqA119tqA119tqA");
			buffer.AppendLine("119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA119tqA115s");
			buffer.AppendLine("A3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115s");
			buffer.AppendLine("A3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA115sA3qA4207");
			buffer.AppendLine("ZA2uwA3vUemdmcehp3gaeidabihp3uAxA2vA8191uwA3vfcfbTAhp4hoEIhp3uwA");
			buffer.AppendLine("3vfcfbTAhp4hoEIhp3uwA3vfcfbTAhp4hoEIhp3uwA3vfcfbTAhp4hoEIhp3uwA3");
			buffer.AppendLine("vfcfbTAhp4hoEIhp3uwA3vfcfbTAhp4hoEIhp3uamadA5qeeejfcehadA2agA2OL");
			buffer.AppendLine("geegUGAFKbbA1OLgeegUFA1KbcA1eeHVPhjGA1KbeA1yXDFA4eeHVPhjFA1KbbA1");
			buffer.AppendLine("fdgiPYHddA1KbcA1fdgiPYHdeA1KbeA13OWhidfyKhbA1OWhidgA3KXA1fcHhgGA");
			buffer.AppendLine("3KbeA1yXDdfA4fcWQghddA2KhbA1edLgnYGA2KbcA1edLgnYFA2KbeA13TfbGAem");
			buffer.AppendLine("PhjHKbbA1TfbFA4KbcA1fgedegdfA3acbeA1yXDdiA4fgedegdgA3acbbA1fdCdd");
			buffer.AppendLine("A4KbcA2DdjA24emPhjHA27yXDG1A28DGFA23emPhjHA27yXDGdeA28DGdfA23emP");
			buffer.AppendLine("hjHA27yXDGdhA28DGdiA23emPhjHA27yXDFdaA28DFGA23emPhjHA27yXDFddA28");
			buffer.AppendLine("DFdeA19uxA7UfjGA20emggLehFA2uxA7UfjFA20ehPNHA3uxA7UfjddA20elHhje");
			buffer.AppendLine("gLVA1uxA7UfjdeA20fgHVLgdWNhjuAabA9UfjGDQLNDPhd1WghQHgeA7BfeLhfgd");
			buffer.AppendLine("giA25BXYOLgeA25BXYOgeFA25TQhgGA27TQhgFA27TQhgddA27TQhgdeA27OfdTe");
			buffer.AppendLine("hGA2uAabA9UfjFDQLNDPhd1WghQHgeA7OfdTehFA26OfdTehddA26OfdTehdeA26");
			buffer.AppendLine("emggLGA131uAabA9UfjddDQLNDPhd1WghQHgeA197ZAQLQHA27OLgefhgiVA1uAa");
			buffer.AppendLine("bA9UfjdeDQLNDPhd1WghQHgeA7faWNgdgifhA25CXHPNgiA25UYXHhd1A25emggL");
			buffer.AppendLine("ehGA26emggLehFA26ehPNHA27elHhjegLVA25fgHVLgdWNhjuxA7zhfVNA16BfeL");
			buffer.AppendLine("hfgdgiA1uxA7zhfVNA16BXYOLgeA1uxA7zhfVNA16BXYOgeFA1uxA7zhfVNA16TQ");
			buffer.AppendLine("hgGA3uxA7zhfVNA16TQhgFA3uxA7zhfVNA16TQhgddA3uxA7zhfVNA16TQhgdeA3");
			buffer.AppendLine("uxA7zhfVNA16OfdTehGA2uxA7zhfVNA16OfdTehFA2uAabA8CA255uAabA8CA252");
			buffer.AppendLine("v=494296");
			buffer.AppendLine();
			return buffer.ToString();
		}
		
		/*
		 * Notepad++ rexexp patterns to replace appendLine with correct
		 * STEP 1 text and single digits:
		 * regexp source: 	buffer.AppendLine\("([\w#]+)=([\w:]+)\s+(//.+)\"\);
		 * regexp dest: 	buffer.Append(String.Format("\1={0}", \2).PadRight(20)).AppendLine("\3");
		 * 
		 * STEP 2 NUMBERS:
		 * regexp source: 	buffer.AppendLine\("([\w#]+)=([-\d\.]+)\s+(//.+)\"\);
		 * regexp dest: 	buffer.Append(String.Format(CultureInfo.InvariantCulture, "\1={0:0.00}", \2).PadRight(20)).AppendLine("\3");
		 * 
		 * */
		
		public string GeneratePresetContent() {
			StringBuilder buffer = new StringBuilder();
			
			buffer.Append(GeneratePresetHeader());
			buffer.Append(GenerateModulatorReferenceTable());
			buffer.Append(GeneratePresetMainLoopCircuit());
			buffer.Append(GeneratePresetCore());
			
			buffer.Append(GeneratePresetLFOG("LfoG1", "LFOG", 3));
			buffer.Append(GeneratePresetLFOG("LfoG2", "LFOG2", 4));
			
			buffer.Append(GeneratePresetVoiceCircuit());
			
			buffer.Append(GeneratePresetEnvelope("Envelope1", "Env1"));
			buffer.Append(GeneratePresetEnvelope("Envelope2", "Env2"));
			buffer.Append(GeneratePresetEnvelope("Envelope3", "Env3"));
			buffer.Append(GeneratePresetEnvelope("Envelope4", "Env4"));

			buffer.Append(GeneratePresetMSEG("MSEG1", 7));
			buffer.Append(GeneratePresetMSEG("MSEG2", 8));
			buffer.Append(GeneratePresetMSEG("MSEG3", 9));
			buffer.Append(GeneratePresetMSEG("MSEG4", 10));

			buffer.Append(GeneratePresetLFO("Lfo 1", "LFO1", 11));
			buffer.Append(GeneratePresetLFO("Lfo 2", "LFO2", 12));
			buffer.Append(GeneratePresetLFO("Lfo 3", "LFO3", 13));
			buffer.Append(GeneratePresetLFO("Lfo 4", "LFO4", 14));

			buffer.Append(GeneratePresetMMap("MMap1", 15));
			buffer.Append(GeneratePresetMMap("MMap2", 16));
			
			buffer.Append(GeneratePresetMMix("MMix1"));
			buffer.Append(GeneratePresetMMix("MMix2"));
			buffer.Append(GeneratePresetMMix("MMix3"));
			buffer.Append(GeneratePresetMMix("MMix4"));
			
			buffer.Append(GeneratePresetGrid("Grid", 17));
			
			buffer.Append(GeneratePresetOscillator("Oscillator1", "Osc1", 18, 19, 20));
			buffer.Append(GeneratePresetOscillator("Oscillator2", "Osc2", 21, 22, 23));
			buffer.Append(GeneratePresetOscillator("Oscillator3", "Osc3", 24, 25, 26));
			buffer.Append(GeneratePresetOscillator("Oscillator4", "Osc4", 27, 28, 29));
			
			buffer.Append(GeneratePresetNoise("Noise1", 30));
			buffer.Append(GeneratePresetNoise("Noise2", 31));
			
			buffer.Append(GeneratePresetFilter("Filter1", "VCF1"));
			buffer.Append(GeneratePresetFilter("Filter2", "VCF2"));
			buffer.Append(GeneratePresetFilter("Filter3", "VCF3"));
			buffer.Append(GeneratePresetFilter("Filter4", "VCF4"));

			buffer.Append(GeneratePresetFMO("FMO1", 32));
			buffer.Append(GeneratePresetFMO("FMO2", 33));
			buffer.Append(GeneratePresetFMO("FMO3", 34));
			buffer.Append(GeneratePresetFMO("FMO4", 35));
			
			buffer.Append(GeneratePresetComb("Comb1"));
			buffer.Append(GeneratePresetComb("Comb2"));
			
			buffer.Append(GeneratePresetShape("Shape1"));
			buffer.Append(GeneratePresetShape("Shape2"));
			
			buffer.Append(GeneratePresetChannelMix("ChannelMix1", "Mix1"));
			buffer.Append(GeneratePresetChannelMix("ChannelMix2", "Mix2"));
			buffer.Append(GeneratePresetChannelMix("ChannelMix3", "Mix3"));
			buffer.Append(GeneratePresetChannelMix("ChannelMix4", "Mix4"));
			
			buffer.Append(GeneratePresetXMF("Xmf1"));
			buffer.Append(GeneratePresetXMF("Xmf2"));
			
			buffer.Append(GeneratePresetSideBand("SideBand1", "SB1"));
			buffer.Append(GeneratePresetSideBand("SideBand2", "SB2"));
			
			buffer.Append(GeneratePresetVoiceMix());
			
			buffer.Append(GeneratePresetFXGrid(36));
			
			buffer.Append(GeneratePresetModFX("ModFX1"));
			buffer.Append(GeneratePresetModFX("ModFX2"));
			
			buffer.Append(GeneratePresetDelay("Delay1"));
			buffer.Append(GeneratePresetDelay("Delay2"));
			
			buffer.Append(GeneratePresetShape("Shape3"));
			buffer.Append(GeneratePresetShape("Shape4"));
			
			buffer.Append(GeneratePresetChannelMix("ChannelMix5", "Mix5"));
			buffer.Append(GeneratePresetChannelMix("ChannelMix6", "Mix6"));
			
			buffer.Append(GeneratePresetReverb("Rev1"));
			
			buffer.Append(GeneratePresetCompressor("Compressor1", "Comp1"));
			buffer.Append(GeneratePresetCompressor("Compressor2", "Comp2"));
			
			buffer.Append(GeneratePresetEqualizer("Equalizer1", "EQ1"));
			buffer.Append(GeneratePresetEqualizer("Equalizer2", "EQ2"));

			buffer.Append(GeneratePresetFilter("Filter5", "VCF5"));
			buffer.Append(GeneratePresetFilter("Filter6", "VCF6"));
			
			buffer.Append(GeneratePresetSideBand("SideBand3", "SB3"));
			
			buffer.Append(GeneratePresetMaster());
			
			buffer.Append(GeneratePresetUglyCompressedBinaryData());
			
			return buffer.ToString();
		}
		
	}
}
