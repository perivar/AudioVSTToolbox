using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Reflection;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of Zebra2Preset.
	/// </summary>
	public class Zebra2Preset
	{
		#region Zebra2 Enums
		public enum LFOGlobalTriggering : int {
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
		
		public enum EnvelopeMode : int {
			quadric = 0,
			linear = 1,
			v_slope = 2
		}

		public enum EnvelopeInitMode : int {
			none = 0,
			Init = 1,
			Delay = 2
		}

		public enum EnvelopeSustainMode : int {
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
		
		public enum EnvelopeTimeBase : int {
			TIMEBASE_8sX = 0,
			TIMEBASE_16sX = 1,
			TIMEBASE_10s = 2,
			TIMEBASE_1_4 = 3,
			TIMEBASE_1_1 = 4,
			TIMEBASE_4_1 = 5
		}
		
		public enum LFOSync : int {
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

		public enum LFOTriggering : int {
			free = 0,
			gate = 1
		}
		
		public enum LFOWave : int {
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
		
		public enum LFOSlew : int {
			off = 0,
			fast = 1,
			slow = 2
		}
		
		public enum LFOModulationSource : int {
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
		
		public enum ModulationSource : int {
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

		public enum OscillatorEffect : int {
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
		
		public enum OscillatorPoly : int {
			single = 0,
			dual = 1,
			quad = 2,
			eleven = 3
		}

		public enum OnOff : int {
			off = 0,
			on = 1
		}
		
		public enum FilterType : int {
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
		
		public enum DelayMode : int {
			stereo_2 = 0,
			multitap_4 = 1,
			dubby_2_plus_2 = 2,
			serial_2 = 3
		}
		
		public enum DelaySync : int {
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
		#endregion
		
		#region Zebra2 Fields

		// Section: Main (#cm=main)
		public float Main_CcOp = 40.37f;             // Output (CcOp=40.37)
		public int Main_LFOG = 1;                    // Active #LFOG (#LFOG=1)
		public int Main_LFOG2 = 1;                   // Active #LFOG2 (#LFOG2=1)

		// Section: PCore (#cm=PCore)
		public float PCore_X_1 = 00.00f;             // X1 (X_1=0.00)
		public float PCore_Y_1 = 00.00f;             // Y1 (Y_1=0.00)
		public float PCore_X_2 = 00.00f;             // X2 (X_2=0.00)
		public float PCore_Y_2 = 00.00f;             // Y2 (Y_2=0.00)
		public float PCore_X_3 = 00.00f;             // X3 (X_3=0.00)
		public float PCore_Y_3 = 00.00f;             // Y3 (Y_3=0.00)
		public float PCore_X_4 = 00.00f;             // X4 (X_4=0.00)
		public float PCore_Y_4 = 00.00f;             // Y4 (Y_4=0.00)
		public string PCore_MT11 = "none:assigned";  // XY1 TargetX1 (MT11=none:assigned)
		public float PCore_ML11 = 15.00f;            // XY1 Right1 (ML11=15.00)
		public float PCore_MR11 = -15.00f;           // XY1 Left1 (MR11=-15.00)
		public string PCore_MT12 = "none:assigned";  // XY1 TargetX2 (MT12=none:assigned)
		public float PCore_ML12 = 50.00f;            // XY1 Right2 (ML12=50.00)
		public float PCore_MR12 = -50.00f;           // XY1 Left2 (MR12=-50.00)
		public string PCore_MT13 = "none:assigned";  // XY1 TargetX3 (MT13=none:assigned)
		public float PCore_ML13 = 50.00f;            // XY1 Right3 (ML13=50.00)
		public float PCore_MR13 = -50.00f;           // XY1 Left3 (MR13=-50.00)
		public string PCore_MT14 = "none:assigned";  // XY1 TargetX4 (MT14=none:assigned)
		public float PCore_ML14 = 50.00f;            // XY1 Right4 (ML14=50.00)
		public float PCore_MR14 = -50.00f;           // XY1 Left4 (MR14=-50.00)
		public string PCore_MT15 = "none:assigned";  // XY1 TargetX5 (MT15=none:assigned)
		public float PCore_ML15 = 50.00f;            // XY1 Right5 (ML15=50.00)
		public float PCore_MR15 = -50.00f;           // XY1 Left5 (MR15=-50.00)
		public string PCore_MT16 = "none:assigned";  // XY1 TargetX6 (MT16=none:assigned)
		public float PCore_ML16 = 50.00f;            // XY1 Right6 (ML16=50.00)
		public float PCore_MR16 = -50.00f;           // XY1 Left6 (MR16=-50.00)
		public string PCore_MT17 = "none:assigned";  // XY1 TargetX7 (MT17=none:assigned)
		public float PCore_ML17 = 50.00f;            // XY1 Right7 (ML17=50.00)
		public float PCore_MR17 = -50.00f;           // XY1 Left7 (MR17=-50.00)
		public string PCore_MT18 = "none:assigned";  // XY1 TargetX8 (MT18=none:assigned)
		public float PCore_ML18 = 50.00f;            // XY1 Right8 (ML18=50.00)
		public float PCore_MR18 = -50.00f;           // XY1 Left8 (MR18=-50.00)
		public string PCore_MT21 = "none:assigned";  // XY1 TargetY1 (MT21=none:assigned)
		public float PCore_ML21 = 50.00f;            // XY1 Up1 (ML21=50.00)
		public float PCore_MR21 = -50.00f;           // XY1 Down1 (MR21=-50.00)
		public string PCore_MT22 = "none:assigned";  // XY1 TargetY2 (MT22=none:assigned)
		public float PCore_ML22 = 50.00f;            // XY1 Up2 (ML22=50.00)
		public float PCore_MR22 = -50.00f;           // XY1 Down2 (MR22=-50.00)
		public string PCore_MT23 = "none:assigned";  // XY1 TargetY3 (MT23=none:assigned)
		public float PCore_ML23 = 50.00f;            // XY1 Up3 (ML23=50.00)
		public float PCore_MR23 = -50.00f;           // XY1 Down3 (MR23=-50.00)
		public string PCore_MT24 = "none:assigned";  // XY1 TargetY4 (MT24=none:assigned)
		public float PCore_ML24 = 48.00f;            // XY1 Up4 (ML24=48.00)
		public float PCore_MR24 = -48.00f;           // XY1 Down4 (MR24=-48.00)
		public string PCore_MT25 = "none:assigned";  // XY1 TargetY5 (MT25=none:assigned)
		public float PCore_ML25 = 50.00f;            // XY1 Up5 (ML25=50.00)
		public float PCore_MR25 = -50.00f;           // XY1 Down5 (MR25=-50.00)
		public string PCore_MT26 = "none:assigned";  // XY1 TargetY6 (MT26=none:assigned)
		public float PCore_ML26 = 50.00f;            // XY1 Up6 (ML26=50.00)
		public float PCore_MR26 = -50.00f;           // XY1 Down6 (MR26=-50.00)
		public string PCore_MT27 = "none:assigned";  // XY1 TargetY7 (MT27=none:assigned)
		public float PCore_ML27 = 50.00f;            // XY1 Up7 (ML27=50.00)
		public float PCore_MR27 = -50.00f;           // XY1 Down7 (MR27=-50.00)
		public string PCore_MT28 = "none:assigned";  // XY1 TargetY8 (MT28=none:assigned)
		public float PCore_ML28 = 50.00f;            // XY1 Up8 (ML28=50.00)
		public float PCore_MR28 = -50.00f;           // XY1 Down8 (MR28=-50.00)
		public string PCore_MT31 = "none:assigned";  // XY2 TargetX1 (MT31=none:assigned)
		public float PCore_ML31 = 50.00f;            // XY2 Right1 (ML31=50.00)
		public float PCore_MR31 = -50.00f;           // XY2 Left1 (MR31=-50.00)
		public string PCore_MT32 = "none:assigned";  // XY2 TargetX2 (MT32=none:assigned)
		public float PCore_ML32 = 50.00f;            // XY2 Right2 (ML32=50.00)
		public float PCore_MR32 = -50.00f;           // XY2 Left2 (MR32=-50.00)
		public string PCore_MT33 = "none:assigned";  // XY2 TargetX3 (MT33=none:assigned)
		public float PCore_ML33 = 50.00f;            // XY2 Right3 (ML33=50.00)
		public float PCore_MR33 = -50.00f;           // XY2 Left3 (MR33=-50.00)
		public string PCore_MT34 = "none:assigned";  // XY2 TargetX4 (MT34=none:assigned)
		public float PCore_ML34 = 50.00f;            // XY2 Right4 (ML34=50.00)
		public float PCore_MR34 = -50.00f;           // XY2 Left4 (MR34=-50.00)
		public string PCore_MT35 = "none:assigned";  // XY2 TargetX5 (MT35=none:assigned)
		public float PCore_ML35 = 50.00f;            // XY2 Right5 (ML35=50.00)
		public float PCore_MR35 = -50.00f;           // XY2 Left5 (MR35=-50.00)
		public string PCore_MT36 = "none:assigned";  // XY2 TargetX6 (MT36=none:assigned)
		public float PCore_ML36 = 50.00f;            // XY2 Right6 (ML36=50.00)
		public float PCore_MR36 = -50.00f;           // XY2 Left6 (MR36=-50.00)
		public string PCore_MT37 = "none:assigned";  // XY2 TargetX7 (MT37=none:assigned)
		public float PCore_ML37 = 50.00f;            // XY2 Right7 (ML37=50.00)
		public float PCore_MR37 = -50.00f;           // XY2 Left7 (MR37=-50.00)
		public string PCore_MT38 = "none:assigned";  // XY2 TargetX8 (MT38=none:assigned)
		public float PCore_ML38 = 50.00f;            // XY2 Right8 (ML38=50.00)
		public float PCore_MR38 = -50.00f;           // XY2 Left8 (MR38=-50.00)
		public string PCore_MT41 = "none:assigned";  // XY2 TargetY1 (MT41=none:assigned)
		public float PCore_ML41 = 50.00f;            // XY2 Up1 (ML41=50.00)
		public float PCore_MR41 = -50.00f;           // XY2 Down1 (MR41=-50.00)
		public string PCore_MT42 = "none:assigned";  // XY2 TargetY2 (MT42=none:assigned)
		public float PCore_ML42 = 50.00f;            // XY2 Up2 (ML42=50.00)
		public float PCore_MR42 = -50.00f;           // XY2 Down2 (MR42=-50.00)
		public string PCore_MT43 = "none:assigned";  // XY2 TargetY3 (MT43=none:assigned)
		public float PCore_ML43 = 50.00f;            // XY2 Up3 (ML43=50.00)
		public float PCore_MR43 = -50.00f;           // XY2 Down3 (MR43=-50.00)
		public string PCore_MT44 = "none:assigned";  // XY2 TargetY4 (MT44=none:assigned)
		public float PCore_ML44 = 50.00f;            // XY2 Up4 (ML44=50.00)
		public float PCore_MR44 = -50.00f;           // XY2 Down4 (MR44=-50.00)
		public string PCore_MT45 = "none:assigned";  // XY2 TargetY5 (MT45=none:assigned)
		public float PCore_ML45 = 50.00f;            // XY2 Up5 (ML45=50.00)
		public float PCore_MR45 = -50.00f;           // XY2 Down5 (MR45=-50.00)
		public string PCore_MT46 = "none:assigned";  // XY2 TargetY6 (MT46=none:assigned)
		public float PCore_ML46 = 50.00f;            // XY2 Up6 (ML46=50.00)
		public float PCore_MR46 = -50.00f;           // XY2 Down6 (MR46=-50.00)
		public string PCore_MT47 = "none:assigned";  // XY2 TargetY7 (MT47=none:assigned)
		public float PCore_ML47 = 50.00f;            // XY2 Up7 (ML47=50.00)
		public float PCore_MR47 = -50.00f;           // XY2 Down7 (MR47=-50.00)
		public string PCore_MT48 = "none:assigned";  // XY2 TargetY8 (MT48=none:assigned)
		public float PCore_ML48 = 50.00f;            // XY2 Up8 (ML48=50.00)
		public float PCore_MR48 = -50.00f;           // XY2 Down8 (MR48=-50.00)
		public string PCore_MT51 = "none:assigned";  // XY3 TargetX1 (MT51=none:assigned)
		public float PCore_ML51 = 08.00f;            // XY3 Right1 (ML51=8.00)
		public float PCore_MR51 = -08.00f;           // XY3 Left1 (MR51=-8.00)
		public string PCore_MT52 = "none:assigned";  // XY3 TargetX2 (MT52=none:assigned)
		public float PCore_ML52 = 50.00f;            // XY3 Right2 (ML52=50.00)
		public float PCore_MR52 = -50.00f;           // XY3 Left2 (MR52=-50.00)
		public string PCore_MT53 = "none:assigned";  // XY3 TargetX3 (MT53=none:assigned)
		public float PCore_ML53 = 50.00f;            // XY3 Right3 (ML53=50.00)
		public float PCore_MR53 = -50.00f;           // XY3 Left3 (MR53=-50.00)
		public string PCore_MT54 = "none:assigned";  // XY3 TargetX4 (MT54=none:assigned)
		public float PCore_ML54 = 50.00f;            // XY3 Right4 (ML54=50.00)
		public float PCore_MR54 = -50.00f;           // XY3 Left4 (MR54=-50.00)
		public string PCore_MT55 = "none:assigned";  // XY3 TargetX5 (MT55=none:assigned)
		public float PCore_ML55 = 50.00f;            // XY3 Right5 (ML55=50.00)
		public float PCore_MR55 = -50.00f;           // XY3 Left5 (MR55=-50.00)
		public string PCore_MT56 = "none:assigned";  // XY3 TargetX6 (MT56=none:assigned)
		public float PCore_ML56 = 50.00f;            // XY3 Right6 (ML56=50.00)
		public float PCore_MR56 = -50.00f;           // XY3 Left6 (MR56=-50.00)
		public string PCore_MT57 = "none:assigned";  // XY3 TargetX7 (MT57=none:assigned)
		public float PCore_ML57 = 50.00f;            // XY3 Right7 (ML57=50.00)
		public float PCore_MR57 = -50.00f;           // XY3 Left7 (MR57=-50.00)
		public string PCore_MT58 = "none:assigned";  // XY3 TargetX8 (MT58=none:assigned)
		public float PCore_ML58 = 50.00f;            // XY3 Right8 (ML58=50.00)
		public float PCore_MR58 = -50.00f;           // XY3 Left8 (MR58=-50.00)
		public string PCore_MT61 = "none:assigned";  // XY3 TargetY1 (MT61=none:assigned)
		public float PCore_ML61 = 50.00f;            // XY3 Up1 (ML61=50.00)
		public float PCore_MR61 = -50.00f;           // XY3 Down1 (MR61=-50.00)
		public string PCore_MT62 = "none:assigned";  // XY3 TargetY2 (MT62=none:assigned)
		public float PCore_ML62 = 50.00f;            // XY3 Up2 (ML62=50.00)
		public float PCore_MR62 = -50.00f;           // XY3 Down2 (MR62=-50.00)
		public string PCore_MT63 = "none:assigned";  // XY3 TargetY3 (MT63=none:assigned)
		public float PCore_ML63 = 50.00f;            // XY3 Up3 (ML63=50.00)
		public float PCore_MR63 = -50.00f;           // XY3 Down3 (MR63=-50.00)
		public string PCore_MT64 = "none:assigned";  // XY3 TargetY4 (MT64=none:assigned)
		public float PCore_ML64 = 50.00f;            // XY3 Up4 (ML64=50.00)
		public float PCore_MR64 = -50.00f;           // XY3 Down4 (MR64=-50.00)
		public string PCore_MT65 = "none:assigned";  // XY3 TargetY5 (MT65=none:assigned)
		public float PCore_ML65 = 50.00f;            // XY3 Up5 (ML65=50.00)
		public float PCore_MR65 = -50.00f;           // XY3 Down5 (MR65=-50.00)
		public string PCore_MT66 = "none:assigned";  // XY3 TargetY6 (MT66=none:assigned)
		public float PCore_ML66 = 50.00f;            // XY3 Up6 (ML66=50.00)
		public float PCore_MR66 = -50.00f;           // XY3 Down6 (MR66=-50.00)
		public string PCore_MT67 = "none:assigned";  // XY3 TargetY7 (MT67=none:assigned)
		public float PCore_ML67 = 50.00f;            // XY3 Up7 (ML67=50.00)
		public float PCore_MR67 = -50.00f;           // XY3 Down7 (MR67=-50.00)
		public string PCore_MT68 = "none:assigned";  // XY3 TargetY8 (MT68=none:assigned)
		public float PCore_ML68 = 50.00f;            // XY3 Up8 (ML68=50.00)
		public float PCore_MR68 = -50.00f;           // XY3 Down8 (MR68=-50.00)
		public string PCore_MT71 = "none:assigned";  // XY4 TargetX1 (MT71=none:assigned)
		public float PCore_ML71 = 50.00f;            // XY4 Right1 (ML71=50.00)
		public float PCore_MR71 = -50.00f;           // XY4 Left1 (MR71=-50.00)
		public string PCore_MT72 = "none:assigned";  // XY4 TargetX2 (MT72=none:assigned)
		public float PCore_ML72 = 50.00f;            // XY4 Right2 (ML72=50.00)
		public float PCore_MR72 = -50.00f;           // XY4 Left2 (MR72=-50.00)
		public string PCore_MT73 = "none:assigned";  // XY4 TargetX3 (MT73=none:assigned)
		public float PCore_ML73 = 50.00f;            // XY4 Right3 (ML73=50.00)
		public float PCore_MR73 = -50.00f;           // XY4 Left3 (MR73=-50.00)
		public string PCore_MT74 = "none:assigned";  // XY4 TargetX4 (MT74=none:assigned)
		public float PCore_ML74 = 50.00f;            // XY4 Right4 (ML74=50.00)
		public float PCore_MR74 = -50.00f;           // XY4 Left4 (MR74=-50.00)
		public string PCore_MT75 = "none:assigned";  // XY4 TargetX5 (MT75=none:assigned)
		public float PCore_ML75 = 50.00f;            // XY4 Right5 (ML75=50.00)
		public float PCore_MR75 = -50.00f;           // XY4 Left5 (MR75=-50.00)
		public string PCore_MT76 = "none:assigned";  // XY4 TargetX6 (MT76=none:assigned)
		public float PCore_ML76 = 50.00f;            // XY4 Right6 (ML76=50.00)
		public float PCore_MR76 = -50.00f;           // XY4 Left6 (MR76=-50.00)
		public string PCore_MT77 = "none:assigned";  // XY4 TargetX7 (MT77=none:assigned)
		public float PCore_ML77 = 50.00f;            // XY4 Right7 (ML77=50.00)
		public float PCore_MR77 = -50.00f;           // XY4 Left7 (MR77=-50.00)
		public string PCore_MT78 = "none:assigned";  // XY4 TargetX8 (MT78=none:assigned)
		public float PCore_ML78 = 50.00f;            // XY4 Right8 (ML78=50.00)
		public float PCore_MR78 = -50.00f;           // XY4 Left8 (MR78=-50.00)
		public string PCore_MT81 = "none:assigned";  // XY4 TargetY1 (MT81=none:assigned)
		public float PCore_ML81 = 50.00f;            // XY4 Up1 (ML81=50.00)
		public float PCore_MR81 = -50.00f;           // XY4 Down1 (MR81=-50.00)
		public string PCore_MT82 = "none:assigned";  // XY4 TargetY2 (MT82=none:assigned)
		public float PCore_ML82 = 50.00f;            // XY4 Up2 (ML82=50.00)
		public float PCore_MR82 = -50.00f;           // XY4 Down2 (MR82=-50.00)
		public string PCore_MT83 = "none:assigned";  // XY4 TargetY3 (MT83=none:assigned)
		public float PCore_ML83 = 08.00f;            // XY4 Up3 (ML83=8.00)
		public float PCore_MR83 = -08.00f;           // XY4 Down3 (MR83=-8.00)
		public string PCore_MT84 = "none:assigned";  // XY4 TargetY4 (MT84=none:assigned)
		public float PCore_ML84 = 50.00f;            // XY4 Up4 (ML84=50.00)
		public float PCore_MR84 = -50.00f;           // XY4 Down4 (MR84=-50.00)
		public string PCore_MT85 = "none:assigned";  // XY4 TargetY5 (MT85=none:assigned)
		public float PCore_ML85 = 50.00f;            // XY4 Up5 (ML85=50.00)
		public float PCore_MR85 = -50.00f;           // XY4 Down5 (MR85=-50.00)
		public string PCore_MT86 = "none:assigned";  // XY4 TargetY6 (MT86=none:assigned)
		public float PCore_ML86 = 50.00f;            // XY4 Up6 (ML86=50.00)
		public float PCore_MR86 = -50.00f;           // XY4 Down6 (MR86=-50.00)
		public string PCore_MT87 = "none:assigned";  // XY4 TargetY7 (MT87=none:assigned)
		public float PCore_ML87 = 50.00f;            // XY4 Up7 (ML87=50.00)
		public float PCore_MR87 = -50.00f;           // XY4 Down7 (MR87=-50.00)
		public string PCore_MT88 = "none:assigned";  // XY4 TargetY8 (MT88=none:assigned)
		public float PCore_ML88 = 50.00f;            // XY4 Up8 (ML88=50.00)
		public float PCore_MR88 = -50.00f;           // XY4 Down8 (MR88=-50.00)
		public string PCore_MMT1 = "none:assigned";  // Matrix1 Target (MMT1=none:assigned)
		public int PCore_MMS1 = 0;                   // Matrix1 Source (MMS1=0)
		public float PCore_MMD1 = 00.00f;            // Matrix1 Depth (MMD1=0.00)
		public int PCore_MMVS1 = 0;                  // Matrix1 ViaSrc (MMVS1=0)
		public float PCore_MMVD1 = 00.00f;           // Matrix1 Via (MMVD1=0.00)
		public string PCore_MMT2 = "none:assigned";  // Matrix2 Target (MMT2=none:assigned)
		public int PCore_MMS2 = 0;                   // Matrix2 Source (MMS2=0)
		public float PCore_MMD2 = 00.00f;            // Matrix2 Depth (MMD2=0.00)
		public int PCore_MMVS2 = 0;                  // Matrix2 ViaSrc (MMVS2=0)
		public float PCore_MMVD2 = 00.00f;           // Matrix2 Via (MMVD2=0.00)
		public string PCore_MMT3 = "none:assigned";  // Matrix3 Target (MMT3=none:assigned)
		public int PCore_MMS3 = 0;                   // Matrix3 Source (MMS3=0)
		public float PCore_MMD3 = 00.00f;            // Matrix3 Depth (MMD3=0.00)
		public int PCore_MMVS3 = 0;                  // Matrix3 ViaSrc (MMVS3=0)
		public float PCore_MMVD3 = 00.00f;           // Matrix3 Via (MMVD3=0.00)
		public string PCore_MMT4 = "none:assigned";  // Matrix4 Target (MMT4=none:assigned)
		public int PCore_MMS4 = 0;                   // Matrix4 Source (MMS4=0)
		public float PCore_MMD4 = 00.00f;            // Matrix4 Depth (MMD4=0.00)
		public int PCore_MMVS4 = 0;                  // Matrix4 ViaSrc (MMVS4=0)
		public float PCore_MMVD4 = 00.00f;           // Matrix4 Via (MMVD4=0.00)
		public string PCore_MMT5 = "none:assigned";  // Matrix5 Target (MMT5=none:assigned)
		public int PCore_MMS5 = 0;                   // Matrix5 Source (MMS5=0)
		public float PCore_MMD5 = 00.00f;            // Matrix5 Depth (MMD5=0.00)
		public int PCore_MMVS5 = 0;                  // Matrix5 ViaSrc (MMVS5=0)
		public float PCore_MMVD5 = 00.00f;           // Matrix5 Via (MMVD5=0.00)
		public string PCore_MMT6 = "none:assigned";  // Matrix6 Target (MMT6=none:assigned)
		public int PCore_MMS6 = 0;                   // Matrix6 Source (MMS6=0)
		public float PCore_MMD6 = 00.00f;            // Matrix6 Depth (MMD6=0.00)
		public int PCore_MMVS6 = 0;                  // Matrix6 ViaSrc (MMVS6=0)
		public float PCore_MMVD6 = 00.00f;           // Matrix6 Via (MMVD6=0.00)
		public string PCore_MMT7 = "none:assigned";  // Matrix7 Target (MMT7=none:assigned)
		public int PCore_MMS7 = 0;                   // Matrix7 Source (MMS7=0)
		public float PCore_MMD7 = 00.00f;            // Matrix7 Depth (MMD7=0.00)
		public int PCore_MMVS7 = 0;                  // Matrix7 ViaSrc (MMVS7=0)
		public float PCore_MMVD7 = 00.00f;           // Matrix7 Via (MMVD7=0.00)
		public string PCore_MMT8 = "none:assigned";  // Matrix8 Target (MMT8=none:assigned)
		public int PCore_MMS8 = 0;                   // Matrix8 Source (MMS8=0)
		public float PCore_MMD8 = 00.00f;            // Matrix8 Depth (MMD8=0.00)
		public int PCore_MMVS8 = 0;                  // Matrix8 ViaSrc (MMVS8=0)
		public float PCore_MMVD8 = 00.00f;           // Matrix8 Via (MMVD8=0.00)
		public string PCore_MMT9 = "none:assigned";  // Matrix9 Target (MMT9=none:assigned)
		public int PCore_MMS9 = 0;                   // Matrix9 Source (MMS9=0)
		public float PCore_MMD9 = 00.00f;            // Matrix9 Depth (MMD9=0.00)
		public int PCore_MMVS9 = 0;                  // Matrix9 ViaSrc (MMVS9=0)
		public float PCore_MMVD9 = 00.00f;           // Matrix9 Via (MMVD9=0.00)
		public string PCore_MMT10 = "none:assigned"; // Matrix10 Target (MMT10=none:assigned)
		public int PCore_MMS10 = 0;                  // Matrix10 Source (MMS10=0)
		public float PCore_MMD10 = 00.00f;           // Matrix10 Depth (MMD10=0.00)
		public int PCore_MMVS10 = 0;                 // Matrix10 ViaSrc (MMVS10=0)
		public float PCore_MMVD10 = 00.00f;          // Matrix10 Via (MMVD10=0.00)
		public string PCore_MMT11 = "none:assigned"; // Matrix11 Target (MMT11=none:assigned)
		public int PCore_MMS11 = 0;                  // Matrix11 Source (MMS11=0)
		public float PCore_MMD11 = 00.00f;           // Matrix11 Depth (MMD11=0.00)
		public int PCore_MMVS11 = 0;                 // Matrix11 ViaSrc (MMVS11=0)
		public float PCore_MMVD11 = 00.00f;          // Matrix11 Via (MMVD11=0.00)
		public string PCore_MMT12 = "none:assigned"; // Matrix12 Target (MMT12=none:assigned)
		public int PCore_MMS12 = 0;                  // Matrix12 Source (MMS12=0)
		public float PCore_MMD12 = 00.00f;           // Matrix12 Depth (MMD12=0.00)
		public int PCore_MMVS12 = 0;                 // Matrix12 ViaSrc (MMVS12=0)
		public float PCore_MMVD12 = 00.00f;          // Matrix12 Via (MMVD12=0.00)
		public int PCore_SBase = 4;                  // SwingBase (SBase=4)
		public float PCore_Swing = 00.00f;           // Swing (Swing=0.00)
		public int PCore_STrig = 1;                  // SwingTrigger (STrig=1)
		public int PCore_PSong = 0;                  // PatchSong (PSong=0)
		public int PCore_PFold = 0;                  // binary data for PatchFolder (PFold=0)
		public int PCore_PFile = 1;                  // binary data for PatchFileName (PFile=1)
		public int PCore_GFile = 2;                  // binary data for GUI FileName (GFile=2)
		public int PCore_GScale = 0;                 // GUI Scale (GScale=0)
		public int PCore_ChLay = 0;                  // Channel Layout (ChLay=0)
		public int PCore_SurrO = 1;                  // Surround Options (SurrO=1)

		// Section: LFOG (#cm=LFOG)
		public int LFOG_Sync = 4;                    // Sync (Sync=4)
		public int LFOG_Trig = 0;                    // Restart (Trig=0)
		public int LFOG_Wave = 0;                    // Waveform (Wave=0)
		public float LFOG_Phse = 00.00f;             // Phase (Phse=0.00)
		public float LFOG_Rate = 100.00f;            // Rate (Rate=100.00)
		public float LFOG_Amp = 100.00f;             // Amplitude (Amp=100.00)
		public int LFOG_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFOG_Nstp = 16;                   // Num Steps (Nstp=16)
		public int LFOG_Stps = 3;                    // binary data for Steps (Stps=3)
		public int LFOG_UWv = 0;                     // User Wave Mode (UWv=0)

		// Section: LFOG2 (#cm=LFOG2)
		public int LFOG2_Sync = 4;                   // Sync (Sync=4)
		public int LFOG2_Trig = 0;                   // Restart (Trig=0)
		public int LFOG2_Wave = 0;                   // Waveform (Wave=0)
		public float LFOG2_Phse = 00.00f;            // Phase (Phse=0.00)
		public float LFOG2_Rate = 100.00f;           // Rate (Rate=100.00)
		public float LFOG2_Amp = 100.00f;            // Amplitude (Amp=100.00)
		public int LFOG2_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFOG2_Nstp = 16;                  // Num Steps (Nstp=16)
		public int LFOG2_Stps = 4;                   // binary data for Steps (Stps=4)
		public int LFOG2_UWv = 0;                    // User Wave Mode (UWv=0)

		// Section: VCC (#cm=VCC)
		public int VCC_LFO1 = 1;                     // Active #LFO1 (#LFO1=1)
		public int VCC_LFO2 = 1;                     // Active #LFO2 (#LFO2=1)
		public int VCC_LFO3 = 1;                     // Active #LFO3 (#LFO3=1)
		public int VCC_LFO4 = 1;                     // Active #LFO4 (#LFO4=1)
		public int VCC_Voices = 1;                   // Voices (Voices=1)
		public int VCC_Voicing = 0;                  // Voicing (Voicing=0)
		public int VCC_Mode = 0;                     // Mode (Mode=0)
		public float VCC_Porta = 00.00f;             // Portamento (Porta=0.00)
		public int VCC_PB = 2;                       // PitchBendUp (PB=2)
		public int VCC_PBD = 2;                      // PitchBendDown (PBD=2)
		public int VCC_ArSc = 4;                     // ArpSync (ArSc=4)
		public int VCC_ArOrd = 0;                    // ArpOrder (ArOrd=0)
		public int VCC_ArLp = 0;                     // ArpLoop (ArLp=0)
		public int VCC_ArOct = 1;                    // ArpOctave (ArOct=1)
		public int VCC_ArLL = 16;                    // ArpLoopLength (ArLL=16)
		public int VCC_ArTr = 0;                     // ArpPortamento (ArTr=0)
		public int VCC_Drft = 1;                     // Drift (Drft=1)
		public int VCC_MTunS = 0;                    // TuningMode (MTunS=0)
		public int VCC_MTunN = 5;                    // binary data for Tuning (MTunN=5)
		public int VCC_MTunT = 6;                    // binary data for TuningTable (MTunT=6)
		public int VCC_Trsp = -12;                   // Transpose (Trsp=-12)
		public float VCC_FTun = 00.00f;              // FineTuneCents (FTun=0.00)
		public float VCC_PortRg = 100.00f;           // PortaRange (PortRg=100.00)
		public int VCC_PortaM = 0;                   // PortamentoMode (PortaM=0)
		public float VCC_Porta2 = 00.00f;            // Portamento2 (Porta2=0.00)
		public int VCC_Agte1 = 2;                    // Arp Gate1 (Agte1=2)
		public int VCC_Atrp1 = 0;                    // Arp Transpose1 (Atrp1=0)
		public int VCC_Avoc1 = 1;                    // Arp Voices1 (Avoc1=1)
		public int VCC_Amul1 = 1;                    // Arp Duration1 (Amul1=1)
		public int VCC_Amod1 = 0;                    // Arp Step Control1 (Amod1=0)
		public float VCC_AMDpt1 = 00.00f;            // Arp Step ModA1 (AMDpt1=0.00)
		public float VCC_AMDpB1 = 00.00f;            // Arp Step ModB1 (AMDpB1=0.00)
		public int VCC_Agte2 = 2;                    // Arp Gate2 (Agte2=2)
		public int VCC_Atrp2 = 0;                    // Arp Transpose2 (Atrp2=0)
		public int VCC_Avoc2 = 1;                    // Arp Voices2 (Avoc2=1)
		public int VCC_Amul2 = 1;                    // Arp Duration2 (Amul2=1)
		public int VCC_Amod2 = 0;                    // Arp Step Control2 (Amod2=0)
		public float VCC_AMDpt2 = 00.00f;            // Arp Step ModA2 (AMDpt2=0.00)
		public float VCC_AMDpB2 = 00.00f;            // Arp Step ModB2 (AMDpB2=0.00)
		public int VCC_Agte3 = 2;                    // Arp Gate3 (Agte3=2)
		public int VCC_Atrp3 = 0;                    // Arp Transpose3 (Atrp3=0)
		public int VCC_Avoc3 = 1;                    // Arp Voices3 (Avoc3=1)
		public int VCC_Amul3 = 1;                    // Arp Duration3 (Amul3=1)
		public int VCC_Amod3 = 0;                    // Arp Step Control3 (Amod3=0)
		public float VCC_AMDpt3 = 00.00f;            // Arp Step ModA3 (AMDpt3=0.00)
		public float VCC_AMDpB3 = 00.00f;            // Arp Step ModB3 (AMDpB3=0.00)
		public int VCC_Agte4 = 2;                    // Arp Gate4 (Agte4=2)
		public int VCC_Atrp4 = 0;                    // Arp Transpose4 (Atrp4=0)
		public int VCC_Avoc4 = 1;                    // Arp Voices4 (Avoc4=1)
		public int VCC_Amul4 = 1;                    // Arp Duration4 (Amul4=1)
		public int VCC_Amod4 = 0;                    // Arp Step Control4 (Amod4=0)
		public float VCC_AMDpt4 = 00.00f;            // Arp Step ModA4 (AMDpt4=0.00)
		public float VCC_AMDpB4 = 00.00f;            // Arp Step ModB4 (AMDpB4=0.00)
		public int VCC_Agte5 = 2;                    // Arp Gate5 (Agte5=2)
		public int VCC_Atrp5 = 0;                    // Arp Transpose5 (Atrp5=0)
		public int VCC_Avoc5 = 1;                    // Arp Voices5 (Avoc5=1)
		public int VCC_Amul5 = 1;                    // Arp Duration5 (Amul5=1)
		public int VCC_Amod5 = 0;                    // Arp Step Control5 (Amod5=0)
		public float VCC_AMDpt5 = 00.00f;            // Arp Step ModA5 (AMDpt5=0.00)
		public float VCC_AMDpB5 = 00.00f;            // Arp Step ModB5 (AMDpB5=0.00)
		public int VCC_Agte6 = 2;                    // Arp Gate6 (Agte6=2)
		public int VCC_Atrp6 = 0;                    // Arp Transpose6 (Atrp6=0)
		public int VCC_Avoc6 = 1;                    // Arp Voices6 (Avoc6=1)
		public int VCC_Amul6 = 1;                    // Arp Duration6 (Amul6=1)
		public int VCC_Amod6 = 0;                    // Arp Step Control6 (Amod6=0)
		public float VCC_AMDpt6 = 00.00f;            // Arp Step ModA6 (AMDpt6=0.00)
		public float VCC_AMDpB6 = 00.00f;            // Arp Step ModB6 (AMDpB6=0.00)
		public int VCC_Agte7 = 2;                    // Arp Gate7 (Agte7=2)
		public int VCC_Atrp7 = 0;                    // Arp Transpose7 (Atrp7=0)
		public int VCC_Avoc7 = 1;                    // Arp Voices7 (Avoc7=1)
		public int VCC_Amul7 = 1;                    // Arp Duration7 (Amul7=1)
		public int VCC_Amod7 = 0;                    // Arp Step Control7 (Amod7=0)
		public float VCC_AMDpt7 = 00.00f;            // Arp Step ModA7 (AMDpt7=0.00)
		public float VCC_AMDpB7 = 00.00f;            // Arp Step ModB7 (AMDpB7=0.00)
		public int VCC_Agte8 = 2;                    // Arp Gate8 (Agte8=2)
		public int VCC_Atrp8 = 0;                    // Arp Transpose8 (Atrp8=0)
		public int VCC_Avoc8 = 1;                    // Arp Voices8 (Avoc8=1)
		public int VCC_Amul8 = 1;                    // Arp Duration8 (Amul8=1)
		public int VCC_Amod8 = 0;                    // Arp Step Control8 (Amod8=0)
		public float VCC_AMDpt8 = 00.00f;            // Arp Step ModA8 (AMDpt8=0.00)
		public float VCC_AMDpB8 = 00.00f;            // Arp Step ModB8 (AMDpB8=0.00)
		public int VCC_Agte9 = 2;                    // Arp Gate9 (Agte9=2)
		public int VCC_Atrp9 = 0;                    // Arp Transpose9 (Atrp9=0)
		public int VCC_Avoc9 = 1;                    // Arp Voices9 (Avoc9=1)
		public int VCC_Amul9 = 1;                    // Arp Duration9 (Amul9=1)
		public int VCC_Amod9 = 0;                    // Arp Step Control9 (Amod9=0)
		public float VCC_AMDpt9 = 00.00f;            // Arp Step ModA9 (AMDpt9=0.00)
		public float VCC_AMDpB9 = 00.00f;            // Arp Step ModB9 (AMDpB9=0.00)
		public int VCC_Agte10 = 2;                   // Arp Gate10 (Agte10=2)
		public int VCC_Atrp10 = 0;                   // Arp Transpose10 (Atrp10=0)
		public int VCC_Avoc10 = 1;                   // Arp Voices10 (Avoc10=1)
		public int VCC_Amul10 = 1;                   // Arp Duration10 (Amul10=1)
		public int VCC_Amod10 = 0;                   // Arp Step Control10 (Amod10=0)
		public float VCC_AMDpt10 = 00.00f;           // Arp Step ModA10 (AMDpt10=0.00)
		public float VCC_AMDpB10 = 00.00f;           // Arp Step ModB10 (AMDpB10=0.00)
		public int VCC_Agte11 = 2;                   // Arp Gate11 (Agte11=2)
		public int VCC_Atrp11 = 0;                   // Arp Transpose11 (Atrp11=0)
		public int VCC_Avoc11 = 1;                   // Arp Voices11 (Avoc11=1)
		public int VCC_Amul11 = 1;                   // Arp Duration11 (Amul11=1)
		public int VCC_Amod11 = 0;                   // Arp Step Control11 (Amod11=0)
		public float VCC_AMDpt11 = 00.00f;           // Arp Step ModA11 (AMDpt11=0.00)
		public float VCC_AMDpB11 = 00.00f;           // Arp Step ModB11 (AMDpB11=0.00)
		public int VCC_Agte12 = 2;                   // Arp Gate12 (Agte12=2)
		public int VCC_Atrp12 = 0;                   // Arp Transpose12 (Atrp12=0)
		public int VCC_Avoc12 = 1;                   // Arp Voices12 (Avoc12=1)
		public int VCC_Amul12 = 1;                   // Arp Duration12 (Amul12=1)
		public int VCC_Amod12 = 0;                   // Arp Step Control12 (Amod12=0)
		public float VCC_AMDpt12 = 00.00f;           // Arp Step ModA12 (AMDpt12=0.00)
		public float VCC_AMDpB12 = 00.00f;           // Arp Step ModB12 (AMDpB12=0.00)
		public int VCC_Agte13 = 2;                   // Arp Gate13 (Agte13=2)
		public int VCC_Atrp13 = 0;                   // Arp Transpose13 (Atrp13=0)
		public int VCC_Avoc13 = 1;                   // Arp Voices13 (Avoc13=1)
		public int VCC_Amul13 = 1;                   // Arp Duration13 (Amul13=1)
		public int VCC_Amod13 = 0;                   // Arp Step Control13 (Amod13=0)
		public float VCC_AMDpt13 = 00.00f;           // Arp Step ModA13 (AMDpt13=0.00)
		public float VCC_AMDpB13 = 00.00f;           // Arp Step ModB13 (AMDpB13=0.00)
		public int VCC_Agte14 = 2;                   // Arp Gate14 (Agte14=2)
		public int VCC_Atrp14 = 0;                   // Arp Transpose14 (Atrp14=0)
		public int VCC_Avoc14 = 1;                   // Arp Voices14 (Avoc14=1)
		public int VCC_Amul14 = 1;                   // Arp Duration14 (Amul14=1)
		public int VCC_Amod14 = 0;                   // Arp Step Control14 (Amod14=0)
		public float VCC_AMDpt14 = 00.00f;           // Arp Step ModA14 (AMDpt14=0.00)
		public float VCC_AMDpB14 = 00.00f;           // Arp Step ModB14 (AMDpB14=0.00)
		public int VCC_Agte15 = 2;                   // Arp Gate15 (Agte15=2)
		public int VCC_Atrp15 = 0;                   // Arp Transpose15 (Atrp15=0)
		public int VCC_Avoc15 = 1;                   // Arp Voices15 (Avoc15=1)
		public int VCC_Amul15 = 1;                   // Arp Duration15 (Amul15=1)
		public int VCC_Amod15 = 0;                   // Arp Step Control15 (Amod15=0)
		public float VCC_AMDpt15 = 00.00f;           // Arp Step ModA15 (AMDpt15=0.00)
		public float VCC_AMDpB15 = 00.00f;           // Arp Step ModB15 (AMDpB15=0.00)
		public int VCC_Agte16 = 2;                   // Arp Gate16 (Agte16=2)
		public int VCC_Atrp16 = 0;                   // Arp Transpose16 (Atrp16=0)
		public int VCC_Avoc16 = 1;                   // Arp Voices16 (Avoc16=1)
		public int VCC_Amul16 = 1;                   // Arp Duration16 (Amul16=1)
		public int VCC_Amod16 = 0;                   // Arp Step Control16 (Amod16=0)
		public float VCC_AMDpt16 = 00.00f;           // Arp Step ModA16 (AMDpt16=0.00)
		public float VCC_AMDpB16 = 00.00f;           // Arp Step ModB16 (AMDpB16=0.00)

		// Section: ENV1 (#cm=ENV1)
		public int ENV1_Mode = 0;                    // Mode (Mode=0)
		public int ENV1_IMode = 0;                   // InitMode (iMode=0)
		public int ENV1_SMode = 0;                   // SustainMode (sMode=0)
		public float ENV1_Init = 00.00f;             // Init (init=0.00)
		public float ENV1_Atk = 00.00f;              // Attack (Atk=0.00)
		public float ENV1_Dec = 50.00f;              // Decay (Dec=50.00)
		public float ENV1_Sus = 80.00f;              // Sustain (Sus=80.00)
		public float ENV1_SusT = 00.00f;             // Fall/Rise (SusT=0.00)
		public float ENV1_Sus2 = 00.00f;             // Sustain2 (Sus2=0.00)
		public float ENV1_Rel = 15.00f;              // Release (Rel=15.00)
		public float ENV1_Vel = 30.00f;              // Velocity (Vel=30.00)
		public float ENV1_V2I = 00.00f;              // Vel2I (V2I=0.00)
		public float ENV1_V2A = 00.00f;              // Vel2A (V2A=0.00)
		public float ENV1_V2D = 00.00f;              // Vel2D (V2D=0.00)
		public float ENV1_V2S = 00.00f;              // Vel2S (V2S=0.00)
		public float ENV1_V2FR = 00.00f;             // Vel2FR (V2FR=0.00)
		public float ENV1_V2S2 = 00.00f;             // Vel2S2 (V2S2=0.00)
		public float ENV1_V2R = 00.00f;              // Vel2R (V2R=0.00)
		public float ENV1_K2I = 00.00f;              // Key2I (K2I=0.00)
		public float ENV1_K2A = 00.00f;              // Key2A (K2A=0.00)
		public float ENV1_K2D = 00.00f;              // Key2D (K2D=0.00)
		public float ENV1_K2S = 00.00f;              // Key2S (K2S=0.00)
		public float ENV1_K2FR = 00.00f;             // Key2FR (K2FR=0.00)
		public float ENV1_K2S2 = 00.00f;             // Key2S2 (K2S2=0.00)
		public float ENV1_K2R = 00.00f;              // Key2R (K2R=0.00)
		public float ENV1_Slope = 00.00f;            // Slope (Slope=0.00)
		public int ENV1_TBase = 0;                   // Timebase (TBase=0)

		// Section: ENV2 (#cm=ENV2)
		public int ENV2_Mode = 0;                    // Mode (Mode=0)
		public int ENV2_IMode = 0;                   // InitMode (iMode=0)
		public int ENV2_SMode = 0;                   // SustainMode (sMode=0)
		public float ENV2_Init = 00.00f;             // Init (init=0.00)
		public float ENV2_Atk = 00.00f;              // Attack (Atk=0.00)
		public float ENV2_Dec = 50.00f;              // Decay (Dec=50.00)
		public float ENV2_Sus = 80.00f;              // Sustain (Sus=80.00)
		public float ENV2_SusT = 00.00f;             // Fall/Rise (SusT=0.00)
		public float ENV2_Sus2 = 00.00f;             // Sustain2 (Sus2=0.00)
		public float ENV2_Rel = 15.00f;              // Release (Rel=15.00)
		public float ENV2_Vel = 30.00f;              // Velocity (Vel=30.00)
		public float ENV2_V2I = 00.00f;              // Vel2I (V2I=0.00)
		public float ENV2_V2A = 00.00f;              // Vel2A (V2A=0.00)
		public float ENV2_V2D = 00.00f;              // Vel2D (V2D=0.00)
		public float ENV2_V2S = 00.00f;              // Vel2S (V2S=0.00)
		public float ENV2_V2FR = 00.00f;             // Vel2FR (V2FR=0.00)
		public float ENV2_V2S2 = 00.00f;             // Vel2S2 (V2S2=0.00)
		public float ENV2_V2R = 00.00f;              // Vel2R (V2R=0.00)
		public float ENV2_K2I = 00.00f;              // Key2I (K2I=0.00)
		public float ENV2_K2A = 00.00f;              // Key2A (K2A=0.00)
		public float ENV2_K2D = 00.00f;              // Key2D (K2D=0.00)
		public float ENV2_K2S = 00.00f;              // Key2S (K2S=0.00)
		public float ENV2_K2FR = 00.00f;             // Key2FR (K2FR=0.00)
		public float ENV2_K2S2 = 00.00f;             // Key2S2 (K2S2=0.00)
		public float ENV2_K2R = 00.00f;              // Key2R (K2R=0.00)
		public float ENV2_Slope = 00.00f;            // Slope (Slope=0.00)
		public int ENV2_TBase = 0;                   // Timebase (TBase=0)

		// Section: ENV3 (#cm=ENV3)
		public int ENV3_Mode = 0;                    // Mode (Mode=0)
		public int ENV3_IMode = 0;                   // InitMode (iMode=0)
		public int ENV3_SMode = 0;                   // SustainMode (sMode=0)
		public float ENV3_Init = 00.00f;             // Init (init=0.00)
		public float ENV3_Atk = 00.00f;              // Attack (Atk=0.00)
		public float ENV3_Dec = 50.00f;              // Decay (Dec=50.00)
		public float ENV3_Sus = 80.00f;              // Sustain (Sus=80.00)
		public float ENV3_SusT = 00.00f;             // Fall/Rise (SusT=0.00)
		public float ENV3_Sus2 = 00.00f;             // Sustain2 (Sus2=0.00)
		public float ENV3_Rel = 15.00f;              // Release (Rel=15.00)
		public float ENV3_Vel = 30.00f;              // Velocity (Vel=30.00)
		public float ENV3_V2I = 00.00f;              // Vel2I (V2I=0.00)
		public float ENV3_V2A = 00.00f;              // Vel2A (V2A=0.00)
		public float ENV3_V2D = 00.00f;              // Vel2D (V2D=0.00)
		public float ENV3_V2S = 00.00f;              // Vel2S (V2S=0.00)
		public float ENV3_V2FR = 00.00f;             // Vel2FR (V2FR=0.00)
		public float ENV3_V2S2 = 00.00f;             // Vel2S2 (V2S2=0.00)
		public float ENV3_V2R = 00.00f;              // Vel2R (V2R=0.00)
		public float ENV3_K2I = 00.00f;              // Key2I (K2I=0.00)
		public float ENV3_K2A = 00.00f;              // Key2A (K2A=0.00)
		public float ENV3_K2D = 00.00f;              // Key2D (K2D=0.00)
		public float ENV3_K2S = 00.00f;              // Key2S (K2S=0.00)
		public float ENV3_K2FR = 00.00f;             // Key2FR (K2FR=0.00)
		public float ENV3_K2S2 = 00.00f;             // Key2S2 (K2S2=0.00)
		public float ENV3_K2R = 00.00f;              // Key2R (K2R=0.00)
		public float ENV3_Slope = 00.00f;            // Slope (Slope=0.00)
		public int ENV3_TBase = 0;                   // Timebase (TBase=0)

		// Section: ENV4 (#cm=ENV4)
		public int ENV4_Mode = 0;                    // Mode (Mode=0)
		public int ENV4_IMode = 0;                   // InitMode (iMode=0)
		public int ENV4_SMode = 0;                   // SustainMode (sMode=0)
		public float ENV4_Init = 00.00f;             // Init (init=0.00)
		public float ENV4_Atk = 00.00f;              // Attack (Atk=0.00)
		public float ENV4_Dec = 50.00f;              // Decay (Dec=50.00)
		public float ENV4_Sus = 80.00f;              // Sustain (Sus=80.00)
		public float ENV4_SusT = 00.00f;             // Fall/Rise (SusT=0.00)
		public float ENV4_Sus2 = 00.00f;             // Sustain2 (Sus2=0.00)
		public float ENV4_Rel = 15.00f;              // Release (Rel=15.00)
		public float ENV4_Vel = 30.00f;              // Velocity (Vel=30.00)
		public float ENV4_V2I = 00.00f;              // Vel2I (V2I=0.00)
		public float ENV4_V2A = 00.00f;              // Vel2A (V2A=0.00)
		public float ENV4_V2D = 00.00f;              // Vel2D (V2D=0.00)
		public float ENV4_V2S = 00.00f;              // Vel2S (V2S=0.00)
		public float ENV4_V2FR = 00.00f;             // Vel2FR (V2FR=0.00)
		public float ENV4_V2S2 = 00.00f;             // Vel2S2 (V2S2=0.00)
		public float ENV4_V2R = 00.00f;              // Vel2R (V2R=0.00)
		public float ENV4_K2I = 00.00f;              // Key2I (K2I=0.00)
		public float ENV4_K2A = 00.00f;              // Key2A (K2A=0.00)
		public float ENV4_K2D = 00.00f;              // Key2D (K2D=0.00)
		public float ENV4_K2S = 00.00f;              // Key2S (K2S=0.00)
		public float ENV4_K2FR = 00.00f;             // Key2FR (K2FR=0.00)
		public float ENV4_K2S2 = 00.00f;             // Key2S2 (K2S2=0.00)
		public float ENV4_K2R = 00.00f;              // Key2R (K2R=0.00)
		public float ENV4_Slope = 00.00f;            // Slope (Slope=0.00)
		public int ENV4_TBase = 0;                   // Timebase (TBase=0)

		// Section: MSEG1 (#cm=MSEG1)
		public int MSEG1_TmUn = 1;                   // TimeUnit (TmUn=1)
		public int MSEG1_Env = 7;                    // binary data for Envelope (Env=7)
		public float MSEG1_Vel = 00.00f;             // Velocity (Vel=0.00)
		public float MSEG1_Atk = 00.00f;             // Attack (Atk=0.00)
		public float MSEG1_Lpt = 00.00f;             // Loop (Lpt=0.00)
		public float MSEG1_Rel = 00.00f;             // Release (Rel=0.00)
		public int MSEG1_Trig = 0;                   // Trigger (Trig=0)

		// Section: MSEG2 (#cm=MSEG2)
		public int MSEG2_TmUn = 1;                   // TimeUnit (TmUn=1)
		public int MSEG2_Env = 8;                    // binary data for Envelope (Env=8)
		public float MSEG2_Vel = 00.00f;             // Velocity (Vel=0.00)
		public float MSEG2_Atk = 00.00f;             // Attack (Atk=0.00)
		public float MSEG2_Lpt = 00.00f;             // Loop (Lpt=0.00)
		public float MSEG2_Rel = 00.00f;             // Release (Rel=0.00)
		public int MSEG2_Trig = 0;                   // Trigger (Trig=0)

		// Section: MSEG3 (#cm=MSEG3)
		public int MSEG3_TmUn = 1;                   // TimeUnit (TmUn=1)
		public int MSEG3_Env = 9;                    // binary data for Envelope (Env=9)
		public float MSEG3_Vel = 00.00f;             // Velocity (Vel=0.00)
		public float MSEG3_Atk = 00.00f;             // Attack (Atk=0.00)
		public float MSEG3_Lpt = 00.00f;             // Loop (Lpt=0.00)
		public float MSEG3_Rel = 00.00f;             // Release (Rel=0.00)
		public int MSEG3_Trig = 0;                   // Trigger (Trig=0)

		// Section: MSEG4 (#cm=MSEG4)
		public int MSEG4_TmUn = 1;                   // TimeUnit (TmUn=1)
		public int MSEG4_Env = 10;                   // binary data for Envelope (Env=10)
		public float MSEG4_Vel = 00.00f;             // Velocity (Vel=0.00)
		public float MSEG4_Atk = 00.00f;             // Attack (Atk=0.00)
		public float MSEG4_Lpt = 00.00f;             // Loop (Lpt=0.00)
		public float MSEG4_Rel = 00.00f;             // Release (Rel=0.00)
		public int MSEG4_Trig = 0;                   // Trigger (Trig=0)

		// Section: LFO1 (#cm=LFO1)
		public int LFO1_Sync = 4;                    // Sync (Sync=4)
		public int LFO1_Trig = 0;                    // Restart (Trig=0)
		public int LFO1_Wave = 0;                    // Waveform (Wave=0)
		public float LFO1_Phse = 00.00f;             // Phase (Phse=0.00)
		public float LFO1_Rate = 100.00f;            // Rate (Rate=100.00)
		public float LFO1_Amp = 100.00f;             // Amplitude (Amp=100.00)
		public int LFO1_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFO1_Nstp = 16;                   // Num Steps (Nstp=16)
		public int LFO1_Stps = 11;                   // binary data for Steps (Stps=11)
		public int LFO1_UWv = 0;                     // User Wave Mode (UWv=0)
		public float LFO1_Dly = 00.00f;              // Delay (Dly=0.00)
		public int LFO1_DMS1 = 0;                    // DepthMod Src1 (DMS1=0)
		public float LFO1_DMD1 = 00.00f;             // DepthMod Dpt1 (DMD1=0.00)
		public int LFO1_FMS1 = 0;                    // FreqMod Src1 (FMS1=0)
		public float LFO1_FMD1 = 00.00f;             // FreqMod Dpt (FMD1=0.00)

		// Section: LFO2 (#cm=LFO2)
		public int LFO2_Sync = 4;                    // Sync (Sync=4)
		public int LFO2_Trig = 0;                    // Restart (Trig=0)
		public int LFO2_Wave = 0;                    // Waveform (Wave=0)
		public float LFO2_Phse = 00.00f;             // Phase (Phse=0.00)
		public float LFO2_Rate = 100.00f;            // Rate (Rate=100.00)
		public float LFO2_Amp = 100.00f;             // Amplitude (Amp=100.00)
		public int LFO2_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFO2_Nstp = 16;                   // Num Steps (Nstp=16)
		public int LFO2_Stps = 12;                   // binary data for Steps (Stps=12)
		public int LFO2_UWv = 0;                     // User Wave Mode (UWv=0)
		public float LFO2_Dly = 00.00f;              // Delay (Dly=0.00)
		public int LFO2_DMS1 = 0;                    // DepthMod Src1 (DMS1=0)
		public float LFO2_DMD1 = 00.00f;             // DepthMod Dpt1 (DMD1=0.00)
		public int LFO2_FMS1 = 0;                    // FreqMod Src1 (FMS1=0)
		public float LFO2_FMD1 = 00.00f;             // FreqMod Dpt (FMD1=0.00)

		// Section: LFO3 (#cm=LFO3)
		public int LFO3_Sync = 4;                    // Sync (Sync=4)
		public int LFO3_Trig = 0;                    // Restart (Trig=0)
		public int LFO3_Wave = 0;                    // Waveform (Wave=0)
		public float LFO3_Phse = 00.00f;             // Phase (Phse=0.00)
		public float LFO3_Rate = 100.00f;            // Rate (Rate=100.00)
		public float LFO3_Amp = 100.00f;             // Amplitude (Amp=100.00)
		public int LFO3_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFO3_Nstp = 16;                   // Num Steps (Nstp=16)
		public int LFO3_Stps = 13;                   // binary data for Steps (Stps=13)
		public int LFO3_UWv = 0;                     // User Wave Mode (UWv=0)
		public float LFO3_Dly = 00.00f;              // Delay (Dly=0.00)
		public int LFO3_DMS1 = 0;                    // DepthMod Src1 (DMS1=0)
		public float LFO3_DMD1 = 00.00f;             // DepthMod Dpt1 (DMD1=0.00)
		public int LFO3_FMS1 = 0;                    // FreqMod Src1 (FMS1=0)
		public float LFO3_FMD1 = 00.00f;             // FreqMod Dpt (FMD1=0.00)

		// Section: LFO4 (#cm=LFO4)
		public int LFO4_Sync = 4;                    // Sync (Sync=4)
		public int LFO4_Trig = 0;                    // Restart (Trig=0)
		public int LFO4_Wave = 0;                    // Waveform (Wave=0)
		public float LFO4_Phse = 00.00f;             // Phase (Phse=0.00)
		public float LFO4_Rate = 100.00f;            // Rate (Rate=100.00)
		public float LFO4_Amp = 100.00f;             // Amplitude (Amp=100.00)
		public int LFO4_Slew = (int) LFOSlew.fast;	 // LFO Slew (Slew=1)
		public int LFO4_Nstp = 16;                   // Num Steps (Nstp=16)
		public int LFO4_Stps = 14;                   // binary data for Steps (Stps=14)
		public int LFO4_UWv = 0;                     // User Wave Mode (UWv=0)
		public float LFO4_Dly = 00.00f;              // Delay (Dly=0.00)
		public int LFO4_DMS1 = 0;                    // DepthMod Src1 (DMS1=0)
		public float LFO4_DMD1 = 00.00f;             // DepthMod Dpt1 (DMD1=0.00)
		public int LFO4_FMS1 = 0;                    // FreqMod Src1 (FMS1=0)
		public float LFO4_FMD1 = 00.00f;             // FreqMod Dpt (FMD1=0.00)

		// Section: MMap1 (#cm=MMap1)
		public int MMap1_Mode = 0;                   // Mode (Mode=0)
		public int MMap1_MSrc = 0;                   // MSrc (MSrc=0)
		public int MMap1_Stps = 15;                  // binary data for Steps (Stps=15)
		public int MMap1_Num = 17;                   // Number (Num=17)

		// Section: MMap2 (#cm=MMap2)
		public int MMap2_Mode = 0;                   // Mode (Mode=0)
		public int MMap2_MSrc = 0;                   // MSrc (MSrc=0)
		public int MMap2_Stps = 16;                  // binary data for Steps (Stps=16)
		public int MMap2_Num = 17;                   // Number (Num=17)

		// Section: MMix1 (#cm=MMix1)
		public int MMix1_Type = 0;                   // Mode (Type=0)
		public int MMix1_Mod1 = 0;                   // Mod1 (Mod1=0)
		public int MMix1_Mod2 = 0;                   // Mod2 (Mod2=0)
		public int MMix1_Mod3 = 0;                   // Mod3 (Mod3=0)
		public float MMix1_Cst = 50.00f;             // Constant (Cst=50.00)

		// Section: MMix2 (#cm=MMix2)
		public int MMix2_Type = 0;                   // Mode (Type=0)
		public int MMix2_Mod1 = 0;                   // Mod1 (Mod1=0)
		public int MMix2_Mod2 = 0;                   // Mod2 (Mod2=0)
		public int MMix2_Mod3 = 0;                   // Mod3 (Mod3=0)
		public float MMix2_Cst = 50.00f;             // Constant (Cst=50.00)

		// Section: MMix3 (#cm=MMix3)
		public int MMix3_Type = 0;                   // Mode (Type=0)
		public int MMix3_Mod1 = 0;                   // Mod1 (Mod1=0)
		public int MMix3_Mod2 = 0;                   // Mod2 (Mod2=0)
		public int MMix3_Mod3 = 0;                   // Mod3 (Mod3=0)
		public float MMix3_Cst = 50.00f;             // Constant (Cst=50.00)

		// Section: MMix4 (#cm=MMix4)
		public int MMix4_Type = 0;                   // Mode (Type=0)
		public int MMix4_Mod1 = 0;                   // Mod1 (Mod1=0)
		public int MMix4_Mod2 = 0;                   // Mod2 (Mod2=0)
		public int MMix4_Mod3 = 0;                   // Mod3 (Mod3=0)
		public float MMix4_Cst = 50.00f;             // Constant (Cst=50.00)

		// Section: Grid (#cm=Grid)
		public int Grid_Grid = 17;                   // binary data for Grid Structure (Grid=17)
		public int Grid_GByp = 0;                    // Bypass (GByp=0)

		// Section: OSC1 (#cm=OSC1)
		public int OSC1_Wave = 0;                    // WaveForm (Wave=0)
		public float OSC1_Tune = 00.00f;             // Tune (Tune=0.00)
		public float OSC1_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int OSC1_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float OSC1_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float OSC1_Phse = 00.00f;             // Phase (Phse=0.00)
		public int OSC1_PhsMSrc = 0;                 // PhaseModSrc (PhsMSrc=0)
		public float OSC1_PhsMDpt = 00.00f;          // PhaseModDepth (PhsMDpt=0.00)
		public float OSC1_WNum = 01.00f;             // WaveWarp (WNum=1.00)
		public int OSC1_WPSrc = 0;                   // WarpModSrc (WPSrc=0)
		public float OSC1_WPDpt = 00.00f;            // WarpModDepth (WPDpt=0.00)
		public float OSC1_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public int OSC1_Curve = 18;                  // binary data for Curve (Curve=18)
		public float OSC1_Prec = 05.00f;             // Resolution (Prec=5.00)
		public int OSC1_FX1Tp = 0;                   // SpectraFX1 Type (FX1Tp=0)
		public float OSC1_SFX1 = 00.00f;             // SpectraFX1 Val (SFX1=0.00)
		public int OSC1_FX1Sc = 0;                   // SFX1ModSrc (FX1Sc=0)
		public float OSC1_FX1Dt = 00.00f;            // SFX1ModDepth (FX1Dt=0.00)
		public int OSC1_FX2Tp = 0;                   // SpectraFX2 Type (FX2Tp=0)
		public float OSC1_SFX2 = 00.00f;             // SpectraFX2 Val (SFX2=0.00)
		public int OSC1_FX2Sc = 0;                   // SFX2ModSrc (FX2Sc=0)
		public float OSC1_FX2Dt = 00.00f;            // SFX2ModDepth (FX2Dt=0.00)
		public int OSC1_Poly = 0;                    // PolyWave (Poly=0)
		public float OSC1_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int OSC1_KVsc = 19;                   // binary data for KeyVelZones (KVsc=19)
		public float OSC1_Vol = 100.00f;             // Volume (Vol=100.00)
		public int OSC1_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float OSC1_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float OSC1_Pan = 00.00f;              // Pan (Pan=0.00)
		public int OSC1_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float OSC1_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public float OSC1_Sync = 00.00f;             // SyncTune (Sync=0.00)
		public int OSC1_SncSc = 0;                   // SyncModSrc (SncSc=0)
		public float OSC1_SncDt = 00.00f;            // SyncModDepth (SncDt=0.00)
		public int OSC1_SncOn = 0;                   // Sync Active (SncOn=0)
		public float OSC1_PolW = 50.00f;             // Poly Width (PolW=50.00)
		public int OSC1_PwmOn = 0;                   // PWM Mode (PwmOn=0)
		public int OSC1_WaTb = 20;                   // binary data for WaveTable (WaTb=20)
		public int OSC1_RePhs = 0;                   // Reset Phase (RePhs=0)
		public float OSC1_Norm = 00.00f;             // Normalize (Norm=0.00)
		public int OSC1_Rend = 0;                    // Renderer (Rend=0)

		// Section: OSC2 (#cm=OSC2)
		public int OSC2_Wave = 0;                    // WaveForm (Wave=0)
		public float OSC2_Tune = 00.00f;             // Tune (Tune=0.00)
		public float OSC2_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int OSC2_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float OSC2_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float OSC2_Phse = 00.00f;             // Phase (Phse=0.00)
		public int OSC2_PhsMSrc = 0;                 // PhaseModSrc (PhsMSrc=0)
		public float OSC2_PhsMDpt = 00.00f;          // PhaseModDepth (PhsMDpt=0.00)
		public float OSC2_WNum = 01.00f;             // WaveWarp (WNum=1.00)
		public int OSC2_WPSrc = 0;                   // WarpModSrc (WPSrc=0)
		public float OSC2_WPDpt = 00.00f;            // WarpModDepth (WPDpt=0.00)
		public float OSC2_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public int OSC2_Curve = 21;                  // binary data for Curve (Curve=21)
		public float OSC2_Prec = 05.00f;             // Resolution (Prec=5.00)
		public int OSC2_FX1Tp = 0;                   // SpectraFX1 Type (FX1Tp=0)
		public float OSC2_SFX1 = 00.00f;             // SpectraFX1 Val (SFX1=0.00)
		public int OSC2_FX1Sc = 0;                   // SFX1ModSrc (FX1Sc=0)
		public float OSC2_FX1Dt = 00.00f;            // SFX1ModDepth (FX1Dt=0.00)
		public int OSC2_FX2Tp = 0;                   // SpectraFX2 Type (FX2Tp=0)
		public float OSC2_SFX2 = 00.00f;             // SpectraFX2 Val (SFX2=0.00)
		public int OSC2_FX2Sc = 0;                   // SFX2ModSrc (FX2Sc=0)
		public float OSC2_FX2Dt = 00.00f;            // SFX2ModDepth (FX2Dt=0.00)
		public int OSC2_Poly = 0;                    // PolyWave (Poly=0)
		public float OSC2_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int OSC2_KVsc = 22;                   // binary data for KeyVelZones (KVsc=22)
		public float OSC2_Vol = 100.00f;             // Volume (Vol=100.00)
		public int OSC2_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float OSC2_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float OSC2_Pan = 00.00f;              // Pan (Pan=0.00)
		public int OSC2_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float OSC2_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public float OSC2_Sync = 00.00f;             // SyncTune (Sync=0.00)
		public int OSC2_SncSc = 0;                   // SyncModSrc (SncSc=0)
		public float OSC2_SncDt = 00.00f;            // SyncModDepth (SncDt=0.00)
		public int OSC2_SncOn = 0;                   // Sync Active (SncOn=0)
		public float OSC2_PolW = 50.00f;             // Poly Width (PolW=50.00)
		public int OSC2_PwmOn = 0;                   // PWM Mode (PwmOn=0)
		public int OSC2_WaTb = 23;                   // binary data for WaveTable (WaTb=23)
		public int OSC2_RePhs = 0;                   // Reset Phase (RePhs=0)
		public float OSC2_Norm = 00.00f;             // Normalize (Norm=0.00)
		public int OSC2_Rend = 0;                    // Renderer (Rend=0)

		// Section: OSC3 (#cm=OSC3)
		public int OSC3_Wave = 0;                    // WaveForm (Wave=0)
		public float OSC3_Tune = 00.00f;             // Tune (Tune=0.00)
		public float OSC3_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int OSC3_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float OSC3_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float OSC3_Phse = 00.00f;             // Phase (Phse=0.00)
		public int OSC3_PhsMSrc = 0;                 // PhaseModSrc (PhsMSrc=0)
		public float OSC3_PhsMDpt = 00.00f;          // PhaseModDepth (PhsMDpt=0.00)
		public float OSC3_WNum = 01.00f;             // WaveWarp (WNum=1.00)
		public int OSC3_WPSrc = 0;                   // WarpModSrc (WPSrc=0)
		public float OSC3_WPDpt = 00.00f;            // WarpModDepth (WPDpt=0.00)
		public float OSC3_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public int OSC3_Curve = 24;                  // binary data for Curve (Curve=24)
		public float OSC3_Prec = 05.00f;             // Resolution (Prec=5.00)
		public int OSC3_FX1Tp = 0;                   // SpectraFX1 Type (FX1Tp=0)
		public float OSC3_SFX1 = 00.00f;             // SpectraFX1 Val (SFX1=0.00)
		public int OSC3_FX1Sc = 0;                   // SFX1ModSrc (FX1Sc=0)
		public float OSC3_FX1Dt = 00.00f;            // SFX1ModDepth (FX1Dt=0.00)
		public int OSC3_FX2Tp = 0;                   // SpectraFX2 Type (FX2Tp=0)
		public float OSC3_SFX2 = 00.00f;             // SpectraFX2 Val (SFX2=0.00)
		public int OSC3_FX2Sc = 0;                   // SFX2ModSrc (FX2Sc=0)
		public float OSC3_FX2Dt = 00.00f;            // SFX2ModDepth (FX2Dt=0.00)
		public int OSC3_Poly = 0;                    // PolyWave (Poly=0)
		public float OSC3_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int OSC3_KVsc = 25;                   // binary data for KeyVelZones (KVsc=25)
		public float OSC3_Vol = 100.00f;             // Volume (Vol=100.00)
		public int OSC3_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float OSC3_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float OSC3_Pan = 00.00f;              // Pan (Pan=0.00)
		public int OSC3_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float OSC3_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public float OSC3_Sync = 00.00f;             // SyncTune (Sync=0.00)
		public int OSC3_SncSc = 0;                   // SyncModSrc (SncSc=0)
		public float OSC3_SncDt = 00.00f;            // SyncModDepth (SncDt=0.00)
		public int OSC3_SncOn = 0;                   // Sync Active (SncOn=0)
		public float OSC3_PolW = 50.00f;             // Poly Width (PolW=50.00)
		public int OSC3_PwmOn = 0;                   // PWM Mode (PwmOn=0)
		public int OSC3_WaTb = 26;                   // binary data for WaveTable (WaTb=26)
		public int OSC3_RePhs = 0;                   // Reset Phase (RePhs=0)
		public float OSC3_Norm = 00.00f;             // Normalize (Norm=0.00)
		public int OSC3_Rend = 0;                    // Renderer (Rend=0)

		// Section: OSC4 (#cm=OSC4)
		public int OSC4_Wave = 0;                    // WaveForm (Wave=0)
		public float OSC4_Tune = 00.00f;             // Tune (Tune=0.00)
		public float OSC4_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int OSC4_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float OSC4_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float OSC4_Phse = 00.00f;             // Phase (Phse=0.00)
		public int OSC4_PhsMSrc = 0;                 // PhaseModSrc (PhsMSrc=0)
		public float OSC4_PhsMDpt = 00.00f;          // PhaseModDepth (PhsMDpt=0.00)
		public float OSC4_WNum = 01.00f;             // WaveWarp (WNum=1.00)
		public int OSC4_WPSrc = 0;                   // WarpModSrc (WPSrc=0)
		public float OSC4_WPDpt = 00.00f;            // WarpModDepth (WPDpt=0.00)
		public float OSC4_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public int OSC4_Curve = 27;                  // binary data for Curve (Curve=27)
		public float OSC4_Prec = 05.00f;             // Resolution (Prec=5.00)
		public int OSC4_FX1Tp = 0;                   // SpectraFX1 Type (FX1Tp=0)
		public float OSC4_SFX1 = 00.00f;             // SpectraFX1 Val (SFX1=0.00)
		public int OSC4_FX1Sc = 0;                   // SFX1ModSrc (FX1Sc=0)
		public float OSC4_FX1Dt = 00.00f;            // SFX1ModDepth (FX1Dt=0.00)
		public int OSC4_FX2Tp = 0;                   // SpectraFX2 Type (FX2Tp=0)
		public float OSC4_SFX2 = 00.00f;             // SpectraFX2 Val (SFX2=0.00)
		public int OSC4_FX2Sc = 0;                   // SFX2ModSrc (FX2Sc=0)
		public float OSC4_FX2Dt = 00.00f;            // SFX2ModDepth (FX2Dt=0.00)
		public int OSC4_Poly = 0;                    // PolyWave (Poly=0)
		public float OSC4_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int OSC4_KVsc = 28;                   // binary data for KeyVelZones (KVsc=28)
		public float OSC4_Vol = 100.00f;             // Volume (Vol=100.00)
		public int OSC4_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float OSC4_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float OSC4_Pan = 00.00f;              // Pan (Pan=0.00)
		public int OSC4_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float OSC4_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public float OSC4_Sync = 00.00f;             // SyncTune (Sync=0.00)
		public int OSC4_SncSc = 0;                   // SyncModSrc (SncSc=0)
		public float OSC4_SncDt = 00.00f;            // SyncModDepth (SncDt=0.00)
		public int OSC4_SncOn = 0;                   // Sync Active (SncOn=0)
		public float OSC4_PolW = 50.00f;             // Poly Width (PolW=50.00)
		public int OSC4_PwmOn = 0;                   // PWM Mode (PwmOn=0)
		public int OSC4_WaTb = 29;                   // binary data for WaveTable (WaTb=29)
		public int OSC4_RePhs = 0;                   // Reset Phase (RePhs=0)
		public float OSC4_Norm = 00.00f;             // Normalize (Norm=0.00)
		public int OSC4_Rend = 0;                    // Renderer (Rend=0)

		// Section: Noise1 (#cm=Noise1)
		public int Noise1_Type = 0;                  // Type (Type=0)
		public float Noise1_F1 = 100.00f;            // Filter1 (F1=100.00)
		public int Noise1_F1Src = 0;                 // F1 ModSrc (F1Src=0)
		public float Noise1_F1Dpt = 00.00f;          // F1 ModDepth (F1Dpt=0.00)
		public float Noise1_F2 = 00.00f;             // Filter2 (F2=0.00)
		public int Noise1_F2Src = 0;                 // F2 ModSrc (F2Src=0)
		public float Noise1_F2Dpt = 00.00f;          // F2 ModDepth (F2Dpt=0.00)
		public int Noise1_KVsc = 30;                 // binary data for KeyVelZones (KVsc=30)
		public float Noise1_Vol = 100.00f;           // Volume (Vol=100.00)
		public int Noise1_VolSc = 0;                 // VolumeModSrc (VolSc=0)
		public float Noise1_VolDt = 00.00f;          // VolumeModDepth (VolDt=0.00)
		public float Noise1_Pan = 00.00f;            // Pan (Pan=0.00)
		public int Noise1_PanSc = 0;                 // PanModSrc (PanSc=0)
		public float Noise1_PanDt = 00.00f;          // PanModDepth (PanDt=0.00)
		public int Noise1_Poly = 0;                  // Poly (Poly=0)
		public float Noise1_PolW = 50.00f;           // Width (PolW=50.00)

		// Section: Noise2 (#cm=Noise2)
		public int Noise2_Type = 0;                  // Type (Type=0)
		public float Noise2_F1 = 100.00f;            // Filter1 (F1=100.00)
		public int Noise2_F1Src = 0;                 // F1 ModSrc (F1Src=0)
		public float Noise2_F1Dpt = 00.00f;          // F1 ModDepth (F1Dpt=0.00)
		public float Noise2_F2 = 00.00f;             // Filter2 (F2=0.00)
		public int Noise2_F2Src = 0;                 // F2 ModSrc (F2Src=0)
		public float Noise2_F2Dpt = 00.00f;          // F2 ModDepth (F2Dpt=0.00)
		public int Noise2_KVsc = 31;                 // binary data for KeyVelZones (KVsc=31)
		public float Noise2_Vol = 100.00f;           // Volume (Vol=100.00)
		public int Noise2_VolSc = 0;                 // VolumeModSrc (VolSc=0)
		public float Noise2_VolDt = 00.00f;          // VolumeModDepth (VolDt=0.00)
		public float Noise2_Pan = 00.00f;            // Pan (Pan=0.00)
		public int Noise2_PanSc = 0;                 // PanModSrc (PanSc=0)
		public float Noise2_PanDt = 00.00f;          // PanModDepth (PanDt=0.00)
		public int Noise2_Poly = 0;                  // Poly (Poly=0)
		public float Noise2_PolW = 50.00f;           // Width (PolW=50.00)

		// Section: VCF1 (#cm=VCF1)
		public int VCF1_Typ = 0;                     // Type (Typ=0)
		public float VCF1_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF1_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF1_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF1_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF1_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF1_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF1_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF1_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF1_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: VCF2 (#cm=VCF2)
		public int VCF2_Typ = 0;                     // Type (Typ=0)
		public float VCF2_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF2_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF2_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF2_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF2_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF2_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF2_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF2_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF2_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: VCF3 (#cm=VCF3)
		public int VCF3_Typ = 0;                     // Type (Typ=0)
		public float VCF3_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF3_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF3_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF3_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF3_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF3_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF3_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF3_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF3_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: VCF4 (#cm=VCF4)
		public int VCF4_Typ = 0;                     // Type (Typ=0)
		public float VCF4_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF4_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF4_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF4_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF4_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF4_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF4_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF4_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF4_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: FMO1 (#cm=FMO1)
		public int FMO1_Wave = 0;                    // Mode (Wave=0)
		public float FMO1_Tune = 00.00f;             // Tune (Tune=0.00)
		public float FMO1_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int FMO1_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float FMO1_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float FMO1_FM = 00.00f;               // FM Depth (FM=0.00)
		public int FMO1_FMSrc = 0;                   // FM ModSrc (FMSrc=0)
		public float FMO1_FMDpt = 00.00f;            // FM ModDepth (FMDpt=0.00)
		public float FMO1_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public float FMO1_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int FMO1_KVsc = 32;                   // binary data for KeyVelZones (KVsc=32)
		public float FMO1_Vol = 100.00f;             // Volume (Vol=100.00)
		public int FMO1_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float FMO1_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float FMO1_Pan = 00.00f;              // Pan (Pan=0.00)
		public int FMO1_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float FMO1_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public int FMO1_Poly = 0;                    // Poly (Poly=0)
		public float FMO1_PolW = 50.00f;             // Width (PolW=50.00)
		public int FMO1_Getr = 0;                    // Generator (Getr=0)

		// Section: FMO2 (#cm=FMO2)
		public int FMO2_Wave = 0;                    // Mode (Wave=0)
		public float FMO2_Tune = 00.00f;             // Tune (Tune=0.00)
		public float FMO2_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int FMO2_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float FMO2_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float FMO2_FM = 00.00f;               // FM Depth (FM=0.00)
		public int FMO2_FMSrc = 0;                   // FM ModSrc (FMSrc=0)
		public float FMO2_FMDpt = 00.00f;            // FM ModDepth (FMDpt=0.00)
		public float FMO2_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public float FMO2_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int FMO2_KVsc = 33;                   // binary data for KeyVelZones (KVsc=33)
		public float FMO2_Vol = 100.00f;             // Volume (Vol=100.00)
		public int FMO2_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float FMO2_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float FMO2_Pan = 00.00f;              // Pan (Pan=0.00)
		public int FMO2_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float FMO2_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public int FMO2_Poly = 0;                    // Poly (Poly=0)
		public float FMO2_PolW = 50.00f;             // Width (PolW=50.00)
		public int FMO2_Getr = 0;                    // Generator (Getr=0)

		// Section: FMO3 (#cm=FMO3)
		public int FMO3_Wave = 0;                    // Mode (Wave=0)
		public float FMO3_Tune = 00.00f;             // Tune (Tune=0.00)
		public float FMO3_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int FMO3_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float FMO3_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float FMO3_FM = 00.00f;               // FM Depth (FM=0.00)
		public int FMO3_FMSrc = 0;                   // FM ModSrc (FMSrc=0)
		public float FMO3_FMDpt = 00.00f;            // FM ModDepth (FMDpt=0.00)
		public float FMO3_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public float FMO3_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int FMO3_KVsc = 34;                   // binary data for KeyVelZones (KVsc=34)
		public float FMO3_Vol = 100.00f;             // Volume (Vol=100.00)
		public int FMO3_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float FMO3_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float FMO3_Pan = 00.00f;              // Pan (Pan=0.00)
		public int FMO3_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float FMO3_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public int FMO3_Poly = 0;                    // Poly (Poly=0)
		public float FMO3_PolW = 50.00f;             // Width (PolW=50.00)
		public int FMO3_Getr = 0;                    // Generator (Getr=0)

		// Section: FMO4 (#cm=FMO4)
		public int FMO4_Wave = 0;                    // Mode (Wave=0)
		public float FMO4_Tune = 00.00f;             // Tune (Tune=0.00)
		public float FMO4_KeyScl = 100.00f;          // key scale (KeyScl=100.00)
		public int FMO4_TMSrc = 0;                   // TuneModSrc (TMSrc=0)
		public float FMO4_TMDpt = 00.00f;            // TuneModDepth (TMDpt=0.00)
		public float FMO4_FM = 00.00f;               // FM Depth (FM=0.00)
		public int FMO4_FMSrc = 0;                   // FM ModSrc (FMSrc=0)
		public float FMO4_FMDpt = 00.00f;            // FM ModDepth (FMDpt=0.00)
		public float FMO4_VtoD = 00.00f;             // Vibrato (VtoD=0.00)
		public float FMO4_Dtun = 00.00f;             // Detune (Dtun=0.00)
		public int FMO4_KVsc = 35;                   // binary data for KeyVelZones (KVsc=35)
		public float FMO4_Vol = 100.00f;             // Volume (Vol=100.00)
		public int FMO4_VolSc = 0;                   // VolumeModSrc (VolSc=0)
		public float FMO4_VolDt = 00.00f;            // VolumeModDepth (VolDt=0.00)
		public float FMO4_Pan = 00.00f;              // Pan (Pan=0.00)
		public int FMO4_PanSc = 0;                   // PanModSrc (PanSc=0)
		public float FMO4_PanDt = 00.00f;            // PanModDepth (PanDt=0.00)
		public int FMO4_Poly = 0;                    // Poly (Poly=0)
		public float FMO4_PolW = 50.00f;             // Width (PolW=50.00)
		public int FMO4_Getr = 0;                    // Generator (Getr=0)

		// Section: Comb1 (#cm=Comb1)
		public int Comb1_Mode = 0;                   // Mode (Mode=0)
		public float Comb1_Tune = 00.00f;            // Tune (Tune=0.00)
		public float Comb1_KeyScl = 100.00f;         // key scale (KeyScl=100.00)
		public int Comb1_TMSrc = 0;                  // TuneModSrc (TMSrc=0)
		public float Comb1_TMDpt = 00.00f;           // TuneModDepth (TMDpt=0.00)
		public float Comb1_Detn = 00.00f;            // Detune (Detn=0.00)
		public float Comb1_VtoD = 00.00f;            // Vibrato (VtoD=0.00)
		public float Comb1_FB = 00.00f;              // Feedback (FB=0.00)
		public int Comb1_FBSrc = 0;                  // FBModSrc (FBSrc=0)
		public float Comb1_FBDpt = 00.00f;           // FBModDepth (FBDpt=0.00)
		public float Comb1_Damp = 00.00f;            // Damp (Damp=0.00)
		public int Comb1_DmpSrc = 0;                 // DampModSrc (DmpSrc=0)
		public float Comb1_DmpDpt = 00.00f;          // DampModDepth (DmpDpt=0.00)
		public float Comb1_Exc = 00.00f;             // PreFill (Exc=0.00)
		public float Comb1_Inj = 00.00f;             // Input (Inj=0.00)
		public int Comb1_InjSrc = 0;                 // InModSrc (InjSrc=0)
		public float Comb1_InjDpt = 00.00f;          // InputMod (InjDpt=0.00)
		public float Comb1_Tne = 50.00f;             // Tone (Tne=50.00)
		public int Comb1_TneSrc = 0;                 // ToneModSrc (TneSrc=0)
		public float Comb1_TneDpt = 00.00f;          // ToneMod (TneDpt=0.00)
		public float Comb1_Sec = 00.00f;             // Flavour (Sec=0.00)
		public int Comb1_SecSrc = 0;                 // SecModSrc (SecSrc=0)
		public float Comb1_SecDpt = 00.00f;          // FlavourMod (SecDpt=0.00)
		public float Comb1_Dist = 00.00f;            // Distortion (Dist=0.00)
		public float Comb1_Dry = 00.00f;             // Dry (Dry=0.00)
		public float Comb1_Vol = 100.00f;            // Volume (Vol=100.00)
		public int Comb1_VolSc = 0;                  // VolumeModSrc (VolSc=0)
		public float Comb1_VolDt = 00.00f;           // VolumeModDepth (VolDt=0.00)
		public float Comb1_Pan = 00.00f;             // Pan (Pan=0.00)
		public int Comb1_PanSc = 0;                  // PanModSrc (PanSc=0)
		public float Comb1_PanDt = 00.00f;           // PanModDepth (PanDt=0.00)
		public int Comb1_Poly = 0;                   // Poly (Poly=0)
		public float Comb1_PolW = 50.00f;            // Width (PolW=50.00)
		public int Comb1_Fill = 0;                   // Fill (Fill=0)

		// Section: Comb2 (#cm=Comb2)
		public int Comb2_Mode = 0;                   // Mode (Mode=0)
		public float Comb2_Tune = 00.00f;            // Tune (Tune=0.00)
		public float Comb2_KeyScl = 100.00f;         // key scale (KeyScl=100.00)
		public int Comb2_TMSrc = 0;                  // TuneModSrc (TMSrc=0)
		public float Comb2_TMDpt = 00.00f;           // TuneModDepth (TMDpt=0.00)
		public float Comb2_Detn = 00.00f;            // Detune (Detn=0.00)
		public float Comb2_VtoD = 00.00f;            // Vibrato (VtoD=0.00)
		public float Comb2_FB = 00.00f;              // Feedback (FB=0.00)
		public int Comb2_FBSrc = 0;                  // FBModSrc (FBSrc=0)
		public float Comb2_FBDpt = 00.00f;           // FBModDepth (FBDpt=0.00)
		public float Comb2_Damp = 00.00f;            // Damp (Damp=0.00)
		public int Comb2_DmpSrc = 0;                 // DampModSrc (DmpSrc=0)
		public float Comb2_DmpDpt = 00.00f;          // DampModDepth (DmpDpt=0.00)
		public float Comb2_Exc = 00.00f;             // PreFill (Exc=0.00)
		public float Comb2_Inj = 00.00f;             // Input (Inj=0.00)
		public int Comb2_InjSrc = 0;                 // InModSrc (InjSrc=0)
		public float Comb2_InjDpt = 00.00f;          // InputMod (InjDpt=0.00)
		public float Comb2_Tne = 50.00f;             // Tone (Tne=50.00)
		public int Comb2_TneSrc = 0;                 // ToneModSrc (TneSrc=0)
		public float Comb2_TneDpt = 00.00f;          // ToneMod (TneDpt=0.00)
		public float Comb2_Sec = 00.00f;             // Flavour (Sec=0.00)
		public int Comb2_SecSrc = 0;                 // SecModSrc (SecSrc=0)
		public float Comb2_SecDpt = 00.00f;          // FlavourMod (SecDpt=0.00)
		public float Comb2_Dist = 00.00f;            // Distortion (Dist=0.00)
		public float Comb2_Dry = 00.00f;             // Dry (Dry=0.00)
		public float Comb2_Vol = 100.00f;            // Volume (Vol=100.00)
		public int Comb2_VolSc = 0;                  // VolumeModSrc (VolSc=0)
		public float Comb2_VolDt = 00.00f;           // VolumeModDepth (VolDt=0.00)
		public float Comb2_Pan = 00.00f;             // Pan (Pan=0.00)
		public int Comb2_PanSc = 0;                  // PanModSrc (PanSc=0)
		public float Comb2_PanDt = 00.00f;           // PanModDepth (PanDt=0.00)
		public int Comb2_Poly = 0;                   // Poly (Poly=0)
		public float Comb2_PolW = 50.00f;            // Width (PolW=50.00)
		public int Comb2_Fill = 0;                   // Fill (Fill=0)

		// Section: Shape1 (#cm=Shape1)
		public int Shape1_Type = 0;                  // Type (Type=0)
		public float Shape1_Depth = 00.00f;          // Depth (Depth=0.00)
		public int Shape1_DMSrc = 0;                 // D_ModSrc (DMSrc=0)
		public float Shape1_DMDpt = 00.00f;          // D_ModDepth (DMDpt=0.00)
		public float Shape1_Edge = 75.00f;           // Edge (Edge=75.00)
		public int Shape1_EMSrc = 0;                 // Edge ModSrc (EMSrc=0)
		public float Shape1_EMDpt = 00.00f;          // Edge ModDepth (EMDpt=0.00)
		public float Shape1_Input = 00.00f;          // Input (Input=0.00)
		public float Shape1_Output = 00.00f;         // Output (Output=0.00)
		public float Shape1_HiOut = 00.00f;          // HiOut (HiOut=0.00)

		// Section: Shape2 (#cm=Shape2)
		public int Shape2_Type = 0;                  // Type (Type=0)
		public float Shape2_Depth = 00.00f;          // Depth (Depth=0.00)
		public int Shape2_DMSrc = 0;                 // D_ModSrc (DMSrc=0)
		public float Shape2_DMDpt = 00.00f;          // D_ModDepth (DMDpt=0.00)
		public float Shape2_Edge = 75.00f;           // Edge (Edge=75.00)
		public int Shape2_EMSrc = 0;                 // Edge ModSrc (EMSrc=0)
		public float Shape2_EMDpt = 00.00f;          // Edge ModDepth (EMDpt=0.00)
		public float Shape2_Input = 00.00f;          // Input (Input=0.00)
		public float Shape2_Output = 00.00f;         // Output (Output=0.00)
		public float Shape2_HiOut = 00.00f;          // HiOut (HiOut=0.00)

		// Section: Mix1 (#cm=Mix1)
		public float Mix1_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix1_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix1_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix1_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix1_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: Mix2 (#cm=Mix2)
		public float Mix2_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix2_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix2_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix2_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix2_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: Mix3 (#cm=Mix3)
		public float Mix3_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix3_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix3_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix3_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix3_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: Mix4 (#cm=Mix4)
		public float Mix4_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix4_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix4_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix4_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix4_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: XMF1 (#cm=XMF1)
		public int XMF1_Typ = 0;                     // Type (Typ=0)
		public float XMF1_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float XMF1_Res = 00.00f;              // Resonance (Res=0.00)
		public float XMF1_FM1 = 00.00f;              // Freq mod 1 (FM1=0.00)
		public int XMF1_FS1 = 0;                     // Modsource1 (FS1=0)
		public float XMF1_FM2 = 00.00f;              // Freq mod 2 (FM2=0.00)
		public int XMF1_FS2 = 0;                     // Modsource2 (FS2=0)
		public float XMF1_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)
		public float XMF1_FOff = 00.00f;             // FreqOffset (FOff=0.00)
		public float XMF1_FOMod = 00.00f;            // FreqOffMod (FOMod=0.00)
		public int XMF1_FOSrc = 0;                   // FreqOffSrc (FOSrc=0)
		public float XMF1_XFM = 00.00f;              // FilterFM (XFM=0.00)
		public float XMF1_XFMD = 00.00f;             // XFMmod (XFMD=0.00)
		public int XMF1_XFMS = 0;                    // XFMrc (XFMS=0)
		public float XMF1_Bias = 00.00f;             // Bias (Bias=0.00)
		public float XMF1_OLoad = 00.00f;            // Overload (OLoad=0.00)
		public float XMF1_Click = 00.00f;            // Click (Click=0.00)
		public int XMF1_Drv = 0;                     // Driver (Drv=0)
		public int XMF1_Rout = 0;                    // Routing (Rout=0)
		public int XMF1_Typ2 = -1;                   // Type2 (Typ2=-1)

		// Section: XMF2 (#cm=XMF2)
		public int XMF2_Typ = 0;                     // Type (Typ=0)
		public float XMF2_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float XMF2_Res = 00.00f;              // Resonance (Res=0.00)
		public float XMF2_FM1 = 00.00f;              // Freq mod 1 (FM1=0.00)
		public int XMF2_FS1 = 0;                     // Modsource1 (FS1=0)
		public float XMF2_FM2 = 00.00f;              // Freq mod 2 (FM2=0.00)
		public int XMF2_FS2 = 0;                     // Modsource2 (FS2=0)
		public float XMF2_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)
		public float XMF2_FOff = 00.00f;             // FreqOffset (FOff=0.00)
		public float XMF2_FOMod = 00.00f;            // FreqOffMod (FOMod=0.00)
		public int XMF2_FOSrc = 0;                   // FreqOffSrc (FOSrc=0)
		public float XMF2_XFM = 00.00f;              // FilterFM (XFM=0.00)
		public float XMF2_XFMD = 00.00f;             // XFMmod (XFMD=0.00)
		public int XMF2_XFMS = 0;                    // XFMrc (XFMS=0)
		public float XMF2_Bias = 00.00f;             // Bias (Bias=0.00)
		public float XMF2_OLoad = 00.00f;            // Overload (OLoad=0.00)
		public float XMF2_Click = 00.00f;            // Click (Click=0.00)
		public int XMF2_Drv = 0;                     // Driver (Drv=0)
		public int XMF2_Rout = 0;                    // Routing (Rout=0)
		public int XMF2_Typ2 = -1;                   // Type2 (Typ2=-1)

		// Section: SB1 (#cm=SB1)
		public int SB1_Range = 0;                    // Range (Range=0)
		public float SB1_Freq = 00.00f;              // Frequency (Freq=0.00)
		public int SB1_FMSrc = 0;                    // FModSource (FMSrc=0)
		public float SB1_FMDpt = 00.00f;             // FModDepth (FMDpt=0.00)
		public float SB1_Offs = 00.00f;              // Offset (Offs=0.00)
		public int SB1_OMSrc = 0;                    // OModSource (OMSrc=0)
		public float SB1_OMDpt = 00.00f;             // OModDepth (OMDpt=0.00)
		public float SB1_Mix = 50.00f;               // Mix (Mix=50.00)
		public int SB1_MMSrc = 0;                    // MModSource (MMSrc=0)
		public float SB1_MMDpt = 00.00f;             // MModDepth (MMDpt=0.00)

		// Section: SB2 (#cm=SB2)
		public int SB2_Range = 0;                    // Range (Range=0)
		public float SB2_Freq = 00.00f;              // Frequency (Freq=0.00)
		public int SB2_FMSrc = 0;                    // FModSource (FMSrc=0)
		public float SB2_FMDpt = 00.00f;             // FModDepth (FMDpt=0.00)
		public float SB2_Offs = 00.00f;              // Offset (Offs=0.00)
		public int SB2_OMSrc = 0;                    // OModSource (OMSrc=0)
		public float SB2_OMDpt = 00.00f;             // OModDepth (OMDpt=0.00)
		public float SB2_Mix = 50.00f;               // Mix (Mix=50.00)
		public int SB2_MMSrc = 0;                    // MModSource (MMSrc=0)
		public float SB2_MMDpt = 00.00f;             // MModDepth (MMDpt=0.00)

		// Section: VCA1 (#cm=VCA1)
		public float VCA1_Pan1 = 00.00f;             // Pan1 (Pan1=0.00)
		public int VCA1_PanMS1 = 0;                  // Pan Mod Src1 (PanMS1=0)
		public float VCA1_PanMD1 = 00.00f;           // Pan Mod Dpt1 (PanMD1=0.00)
		public float VCA1_Vol1 = 50.00f;             // Volume1 (Vol1=50.00)
		public int VCA1_VCA1 = 1;                    // VCA1 (VCA1=1)
		public int VCA1_ModSrc1 = 0;                 // Modulation1 (ModSrc1=0)
		public float VCA1_ModDpt1 = 00.00f;          // Mod Depth1 (ModDpt1=0.00)
		public float VCA1_Pan2 = 00.00f;             // Pan2 (Pan2=0.00)
		public int VCA1_PanMS2 = 0;                  // Pan Mod Src2 (PanMS2=0)
		public float VCA1_PanMD2 = 00.00f;           // Pan Mod Dpt2 (PanMD2=0.00)
		public float VCA1_Vol2 = 50.00f;             // Volume2 (Vol2=50.00)
		public int VCA1_VCA2 = 1;                    // VCA2 (VCA2=1)
		public int VCA1_ModSrc2 = 0;                 // Modulation2 (ModSrc2=0)
		public float VCA1_ModDpt2 = 00.00f;          // Mod Depth2 (ModDpt2=0.00)
		public float VCA1_Pan3 = 00.00f;             // Pan3 (Pan3=0.00)
		public int VCA1_PanMS3 = 0;                  // Pan Mod Src3 (PanMS3=0)
		public float VCA1_PanMD3 = 00.00f;           // Pan Mod Dpt3 (PanMD3=0.00)
		public float VCA1_Vol3 = 50.00f;             // Volume3 (Vol3=50.00)
		public int VCA1_VCA3 = 1;                    // VCA3 (VCA3=1)
		public int VCA1_ModSrc3 = 0;                 // Modulation3 (ModSrc3=0)
		public float VCA1_ModDpt3 = 00.00f;          // Mod Depth3 (ModDpt3=0.00)
		public float VCA1_Pan4 = 00.00f;             // Pan4 (Pan4=0.00)
		public int VCA1_PanMS4 = 0;                  // Pan Mod Src4 (PanMS4=0)
		public float VCA1_PanMD4 = 00.00f;           // Pan Mod Dpt4 (PanMD4=0.00)
		public float VCA1_Vol4 = 50.00f;             // Volume4 (Vol4=50.00)
		public int VCA1_VCA4 = 1;                    // VCA4 (VCA4=1)
		public int VCA1_ModSrc4 = 0;                 // Modulation4 (ModSrc4=0)
		public float VCA1_ModDpt4 = 00.00f;          // Mod Depth4 (ModDpt4=0.00)
		public int VCA1_MT1 = 0;                     // Mute1 (MT1=0)
		public int VCA1_MT2 = 0;                     // Mute2 (MT2=0)
		public int VCA1_MT3 = 0;                     // Mute3 (MT3=0)
		public int VCA1_MT4 = 0;                     // Mute4 (MT4=0)
		public int VCA1_PB1 = 0;                     // Panning1 (PB1=0)
		public int VCA1_PB2 = 0;                     // Panning2 (PB2=0)
		public int VCA1_PB3 = 0;                     // Panning3 (PB3=0)
		public int VCA1_PB4 = 0;                     // Panning4 (PB4=0)
		public int VCA1_Bus1 = 0;                    // Bus1 (Bus1=0)
		public int VCA1_Bus2 = 0;                    // Bus2 (Bus2=0)
		public int VCA1_Bus3 = 0;                    // Bus3 (Bus3=0)
		public int VCA1_Bus4 = 0;                    // Bus4 (Bus4=0)
		public float VCA1_Send1 = 00.00f;            // Send1 (Send1=0.00)
		public int VCA1_SnSrc1 = 0;                  // SendMod1 (SnSrc1=0)
		public float VCA1_SnDpt1 = 00.00f;           // SendDepth1 (SnDpt1=0.00)
		public float VCA1_Send2 = 00.00f;            // Send2 (Send2=0.00)
		public int VCA1_SnSrc2 = 0;                  // SendMod2 (SnSrc2=0)
		public float VCA1_SnDpt2 = 00.00f;           // SendDepth2 (SnDpt2=0.00)
		public int VCA1_AttS = 1;                    // AttackSmooth (AttS=1)

		// Section: GridFX (#cm=GridFX)
		public int GridFX_Grid = 36;                 // binary data for Grid Structure (Grid=36)
		public int GridFX_GByp = 0;                  // Bypass (GByp=0)

		// Section: ModFX1 (#cm=ModFX1)
		public int ModFX1_Mode = 0;                  // Mode (Mode=0)
		public float ModFX1_Cent = 20.00f;           // Center (Cent=20.00)
		public float ModFX1_Sped = 50.00f;           // Speed (Sped=50.00)
		public float ModFX1_PhOff = 50.00f;          // Stereo (PhOff=50.00)
		public float ModFX1_Dpth = 50.00f;           // Depth (Dpth=50.00)
		public float ModFX1_FeeB = 00.00f;           // Feedback (FeeB=0.00)
		public float ModFX1_Mix = 00.00f;            // Mix (Mix=0.00)
		public float ModFX1_LCut = 00.00f;           // LowCut Freq (LCut=0.00)
		public float ModFX1_HCut = 100.00f;          // HiCut Freq (HCut=100.00)
		public float ModFX1_Quad = 00.00f;           // Quad (Quad=0.00)
		public float ModFX1_Qphs = 25.00f;           // QuadPhase (Qphs=25.00)
		public float ModFX1_Leq = 00.00f;            // Low Boost dB (Leq=0.00)
		public float ModFX1_Heq = 00.00f;            // High Boost dB (Heq=0.00)
		public float ModFX1_Q1 = 00.00f;             // Q1 (Q1=0.00)
		public float ModFX1_Q2 = 00.00f;             // Q2 (Q2=0.00)
		public int ModFX1_EQon = 1;                  // EQ (EQon=1)

		// Section: ModFX2 (#cm=ModFX2)
		public int ModFX2_Mode = 0;                  // Mode (Mode=0)
		public float ModFX2_Cent = 20.00f;           // Center (Cent=20.00)
		public float ModFX2_Sped = 50.00f;           // Speed (Sped=50.00)
		public float ModFX2_PhOff = 50.00f;          // Stereo (PhOff=50.00)
		public float ModFX2_Dpth = 50.00f;           // Depth (Dpth=50.00)
		public float ModFX2_FeeB = 00.00f;           // Feedback (FeeB=0.00)
		public float ModFX2_Mix = 00.00f;            // Mix (Mix=0.00)
		public float ModFX2_LCut = 00.00f;           // LowCut Freq (LCut=0.00)
		public float ModFX2_HCut = 100.00f;          // HiCut Freq (HCut=100.00)
		public float ModFX2_Quad = 00.00f;           // Quad (Quad=0.00)
		public float ModFX2_Qphs = 25.00f;           // QuadPhase (Qphs=25.00)
		public float ModFX2_Leq = 00.00f;            // Low Boost dB (Leq=0.00)
		public float ModFX2_Heq = 00.00f;            // High Boost dB (Heq=0.00)
		public float ModFX2_Q1 = 00.00f;             // Q1 (Q1=0.00)
		public float ModFX2_Q2 = 00.00f;             // Q2 (Q2=0.00)
		public int ModFX2_EQon = 1;                  // EQ (EQon=1)

		// Section: Delay1 (#cm=Delay1)
		public int Delay1_Mode = 0;                  // Mode (Mode=0)
		public float Delay1_Mix = 00.00f;            // Mix (Mix=0.00)
		public float Delay1_FB = 10.00f;             // Feedback (FB=10.00)
		public float Delay1_CB = 25.00f;             // X-back (CB=25.00)
		public float Delay1_LP = 100.00f;            // Lowpass (LP=100.00)
		public float Delay1_HP = 00.00f;             // Hipass (HP=0.00)
		public float Delay1_Drv = 00.00f;            // Drive (Drv=0.00)
		public int Delay1_Sync1 = 4;                 // Sync1 (Sync1=4)
		public int Delay1_Sync2 = 4;                 // Sync2 (Sync2=4)
		public int Delay1_Sync3 = 4;                 // Sync3 (Sync3=4)
		public int Delay1_Sync4 = 4;                 // Sync4 (Sync4=4)
		public float Delay1_T0 = 100.00f;            // Ratio1 (T0=100.00)
		public float Delay1_T1 = 100.00f;            // Ratio2 (T1=100.00)
		public float Delay1_T2 = 100.00f;            // Ratio3 (T2=100.00)
		public float Delay1_T3 = 100.00f;            // Ratio4 (T3=100.00)
		public float Delay1_Pan1 = -100.00f;         // Pan1 (Pan1=-100.00)
		public float Delay1_Pan2 = 100.00f;          // Pan2 (Pan2=100.00)
		public float Delay1_Pan3 = -100.00f;         // Pan3 (Pan3=-100.00)
		public float Delay1_Pan4 = 100.00f;          // Pan4 (Pan4=100.00)

		// Section: Delay2 (#cm=Delay2)
		public int Delay2_Mode = 0;                  // Mode (Mode=0)
		public float Delay2_Mix = 00.00f;            // Mix (Mix=0.00)
		public float Delay2_FB = 10.00f;             // Feedback (FB=10.00)
		public float Delay2_CB = 25.00f;             // X-back (CB=25.00)
		public float Delay2_LP = 100.00f;            // Lowpass (LP=100.00)
		public float Delay2_HP = 00.00f;             // Hipass (HP=0.00)
		public float Delay2_Drv = 00.00f;            // Drive (Drv=0.00)
		public int Delay2_Sync1 = 4;                 // Sync1 (Sync1=4)
		public int Delay2_Sync2 = 4;                 // Sync2 (Sync2=4)
		public int Delay2_Sync3 = 4;                 // Sync3 (Sync3=4)
		public int Delay2_Sync4 = 4;                 // Sync4 (Sync4=4)
		public float Delay2_T0 = 100.00f;            // Ratio1 (T0=100.00)
		public float Delay2_T1 = 100.00f;            // Ratio2 (T1=100.00)
		public float Delay2_T2 = 100.00f;            // Ratio3 (T2=100.00)
		public float Delay2_T3 = 100.00f;            // Ratio4 (T3=100.00)
		public float Delay2_Pan1 = -100.00f;         // Pan1 (Pan1=-100.00)
		public float Delay2_Pan2 = 100.00f;          // Pan2 (Pan2=100.00)
		public float Delay2_Pan3 = -100.00f;         // Pan3 (Pan3=-100.00)
		public float Delay2_Pan4 = 100.00f;          // Pan4 (Pan4=100.00)

		// Section: Shape3 (#cm=Shape3)
		public int Shape3_Type = 0;                  // Type (Type=0)
		public float Shape3_Depth = 00.00f;          // Depth (Depth=0.00)
		public int Shape3_DMSrc = 0;                 // D_ModSrc (DMSrc=0)
		public float Shape3_DMDpt = 00.00f;          // D_ModDepth (DMDpt=0.00)
		public float Shape3_Edge = 75.00f;           // Edge (Edge=75.00)
		public int Shape3_EMSrc = 0;                 // Edge ModSrc (EMSrc=0)
		public float Shape3_EMDpt = 00.00f;          // Edge ModDepth (EMDpt=0.00)
		public float Shape3_Input = 00.00f;          // Input (Input=0.00)
		public float Shape3_Output = 00.00f;         // Output (Output=0.00)
		public float Shape3_HiOut = 00.00f;          // HiOut (HiOut=0.00)

		// Section: Shape4 (#cm=Shape4)
		public int Shape4_Type = 0;                  // Type (Type=0)
		public float Shape4_Depth = 00.00f;          // Depth (Depth=0.00)
		public int Shape4_DMSrc = 0;                 // D_ModSrc (DMSrc=0)
		public float Shape4_DMDpt = 00.00f;          // D_ModDepth (DMDpt=0.00)
		public float Shape4_Edge = 75.00f;           // Edge (Edge=75.00)
		public int Shape4_EMSrc = 0;                 // Edge ModSrc (EMSrc=0)
		public float Shape4_EMDpt = 00.00f;          // Edge ModDepth (EMDpt=0.00)
		public float Shape4_Input = 00.00f;          // Input (Input=0.00)
		public float Shape4_Output = 00.00f;         // Output (Output=0.00)
		public float Shape4_HiOut = 00.00f;          // HiOut (HiOut=0.00)

		// Section: Mix5 (#cm=Mix5)
		public float Mix5_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix5_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix5_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix5_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix5_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: Mix6 (#cm=Mix6)
		public float Mix6_Pan = 00.00f;              // Pan (Pan=0.00)
		public float Mix6_Mix = 50.00f;              // Mix (Mix=50.00)
		public int Mix6_PnMd = 0;                    // Pan Mode (PnMd=0)
		public float Mix6_PnMD = 00.00f;             // PanMod Depth (PnMD=0.00)
		public int Mix6_PnMS = 0;                    // PanMod Source (PnMS=0)

		// Section: Rev1 (#cm=Rev1)
		public int Rev1_Mode = 0;                    // Mode (Mode=0)
		public float Rev1_Dry = 82.00f;              // Dry (Dry=82.00)
		public float Rev1_Wet = 52.00f;              // Wet (Wet=52.00)
		public float Rev1_FB = 43.00f;               // Feedback (FB=43.00)
		public float Rev1_Damp = 04.00f;             // Damp (Damp=4.00)
		public float Rev1_Size = 75.00f;             // Range (Size=75.00)
		public float Rev1_Spd = 50.00f;              // Speed (Spd=50.00)
		public float Rev1_Dpt = 50.00f;              // Modulation (Dpt=50.00)
		public float Rev1_DFB = 70.00f;              // Diff Feedback (DFB=70.00)
		public float Rev1_DSize = 50.00f;            // Diff Range (DSize=50.00)
		public float Rev1_EMix = 100.00f;            // Diff Mix (EMix=100.00)
		public float Rev1_DMod = 50.00f;             // Diff Mod (DMod=50.00)
		public float Rev1_DSpd = 55.00f;             // Diff Speed (DSpd=55.00)
		public float Rev1_Pre = 00.00f;              // PreDelay (Pre=0.00)

		// Section: Comp1 (#cm=Comp1)
		public int Comp1_Type = 0;                   // Type (Type=0)
		public float Comp1_Rat = 100.00f;            // Compression (Rat=100.00)
		public float Comp1_Thres = -20.00f;          // Threshold (Thres=-20.00)
		public float Comp1_Att = 30.00f;             // Attack (Att=30.00)
		public float Comp1_Rel = 50.00f;             // Release (Rel=50.00)
		public float Comp1_Input = 00.00f;           // Input (Input=0.00)
		public float Comp1_Output = 00.00f;          // Output (Output=0.00)

		// Section: Comp2 (#cm=Comp2)
		public int Comp2_Type = 0;                   // Type (Type=0)
		public float Comp2_Rat = 100.00f;            // Compression (Rat=100.00)
		public float Comp2_Thres = -20.00f;          // Threshold (Thres=-20.00)
		public float Comp2_Att = 30.00f;             // Attack (Att=30.00)
		public float Comp2_Rel = 50.00f;             // Release (Rel=50.00)
		public float Comp2_Input = 00.00f;           // Input (Input=0.00)
		public float Comp2_Output = 00.00f;          // Output (Output=0.00)

		// Section: EQ1 (#cm=EQ1)
		public float EQ1_Fc1 = 20.00f;               // Freq LowShelf (fc1=20.00)
		public float EQ1_Res1 = 25.00f;              // Q LowShelf (res1=25.00)
		public float EQ1_Gain1 = 00.00f;             // Gain LowShelf (gain1=0.00)
		public float EQ1_Fc2 = 40.00f;               // Freq Mid1 (fc2=40.00)
		public float EQ1_Res2 = 25.00f;              // Q Mid1 (res2=25.00)
		public float EQ1_Gain2 = 00.00f;             // Gain Mid1 (gain2=0.00)
		public float EQ1_Fc3 = 60.00f;               // Freq Mid2 (fc3=60.00)
		public float EQ1_Res3 = 25.00f;              // Q Mid2 (res3=25.00)
		public float EQ1_Gain3 = 00.00f;             // Gain Mid2 (gain3=0.00)
		public float EQ1_Fc4 = 80.00f;               // Freq HiShelf (fc4=80.00)
		public float EQ1_Res4 = 25.00f;              // Q HiShelf (res4=25.00)
		public float EQ1_Gain4 = 00.00f;             // Gain HiShelf (gain4=0.00)

		// Section: EQ2 (#cm=EQ2)
		public float EQ2_Fc1 = 20.00f;               // Freq LowShelf (fc1=20.00)
		public float EQ2_Res1 = 25.00f;              // Q LowShelf (res1=25.00)
		public float EQ2_Gain1 = 00.00f;             // Gain LowShelf (gain1=0.00)
		public float EQ2_Fc2 = 40.00f;               // Freq Mid1 (fc2=40.00)
		public float EQ2_Res2 = 25.00f;              // Q Mid1 (res2=25.00)
		public float EQ2_Gain2 = 00.00f;             // Gain Mid1 (gain2=0.00)
		public float EQ2_Fc3 = 60.00f;               // Freq Mid2 (fc3=60.00)
		public float EQ2_Res3 = 25.00f;              // Q Mid2 (res3=25.00)
		public float EQ2_Gain3 = 00.00f;             // Gain Mid2 (gain3=0.00)
		public float EQ2_Fc4 = 80.00f;               // Freq HiShelf (fc4=80.00)
		public float EQ2_Res4 = 25.00f;              // Q HiShelf (res4=25.00)
		public float EQ2_Gain4 = 00.00f;             // Gain HiShelf (gain4=0.00)

		// Section: VCF5 (#cm=VCF5)
		public int VCF5_Typ = 0;                     // Type (Typ=0)
		public float VCF5_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF5_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF5_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF5_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF5_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF5_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF5_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF5_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF5_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: VCF6 (#cm=VCF6)
		public int VCF6_Typ = 0;                     // Type (Typ=0)
		public float VCF6_Cut = 150.00f;             // Cutoff (Cut=150.00)
		public float VCF6_Res = 00.00f;              // Resonance (Res=0.00)
		public float VCF6_Drv = 00.00f;              // Drive (Drv=0.00)
		public float VCF6_Gain = 00.00f;             // Gain (Gain=0.00)
		public float VCF6_FM1 = 00.00f;              // ModDepth1 (FM1=0.00)
		public int VCF6_FS1 = 0;                     // Modsource1 (FS1=0)
		public float VCF6_FM2 = 00.00f;              // ModDepth2 (FM2=0.00)
		public int VCF6_FS2 = 0;                     // Modsource2 (FS2=0)
		public float VCF6_KeyScl = 00.00f;           // KeyFollow (KeyScl=0.00)

		// Section: SB3 (#cm=SB3)
		public int SB3_Range = 0;                    // Range (Range=0)
		public float SB3_Freq = 00.00f;              // Frequency (Freq=0.00)
		public int SB3_FMSrc = 0;                    // FModSource (FMSrc=0)
		public float SB3_FMDpt = 00.00f;             // FModDepth (FMDpt=0.00)
		public float SB3_Offs = 00.00f;              // Offset (Offs=0.00)
		public int SB3_OMSrc = 0;                    // OModSource (OMSrc=0)
		public float SB3_OMDpt = 00.00f;             // OModDepth (OMDpt=0.00)
		public float SB3_Mix = 50.00f;               // Mix (Mix=50.00)
		public int SB3_MMSrc = 0;                    // MModSource (MMSrc=0)
		public float SB3_MMDpt = 00.00f;             // MModDepth (MMDpt=0.00)

		// Section: ZMas (#cm=ZMas)
		public float ZMas_Ret1 = 00.00f;             // Return1 (Ret1=0.00)
		public float ZMas_Ret2 = 00.00f;             // Return2 (Ret2=0.00)
		public float ZMas_Mast = 100.00f;            // Master (Mast=100.00)
		public int ZMas_XY1L = 37;                   // binary data for XY1 Label (XY1L=37)
		public int ZMas_XY2L = 38;                   // binary data for XY2 Label (XY2L=38)
		public int ZMas_XY3L = 39;                   // binary data for XY3 Label (XY3L=39)
		public int ZMas_XY4L = 40;                   // binary data for XY4 Label (XY4L=40)
		public int ZMas_XY1T = 41;                   // binary data for XY1 Text (XY1T=41)
		public int ZMas_XY2T = 42;                   // binary data for XY2 Text (XY2T=42)
		public int ZMas_XY3T = 43;                   // binary data for XY3 Text (XY3T=43)
		public int ZMas_XY4T = 44;                   // binary data for XY4 Text (XY4T=44)
		public int ZMas_OSC1 = 45;                   // binary data for Oscillator1 (OSC1=45)
		public int ZMas_OSC2 = 46;                   // binary data for Oscillator2 (OSC2=46)
		public int ZMas_OSC3 = 47;                   // binary data for Oscillator3 (OSC3=47)
		public int ZMas_OSC4 = 48;                   // binary data for Oscillator4 (OSC4=48)
		public int ZMas_MSEG1 = 49;                  // binary data for MSEG1 (MSEG1=49)
		public int ZMas_MSEG2 = 50;                  // binary data for MSEG2 (MSEG2=50)
		public int ZMas_MSEG3 = 51;                  // binary data for MSEG3 (MSEG3=51)
		public int ZMas_MSEG4 = 52;                  // binary data for MSEG4 (MSEG4=52)
		public int ZMas_Rev1 = 53;                   // binary data for Rev1 (Rev1=53)
		public int ZMas_Pn3 = 0;                     // FXPaneMem (Pn3=0)
		public int ZMas_Pn4 = 0;                     // OSC1PaneMem (Pn4=0)
		public int ZMas_Pn5 = 0;                     // OSC2PaneMem (Pn5=0)
		public int ZMas_Pn6 = 0;                     // OSC3PaneMem (Pn6=0)
		public int ZMas_Pn7 = 0;                     // OSC4PaneMem (Pn7=0)
		public int ZMas_Pn8 = 0;                     // VCF1PaneMem (Pn8=0)
		public int ZMas_Pn9 = 0;                     // VCF2PaneMem (Pn9=0)
		public int ZMas_Pn10 = 0;                    // VCF3PaneMem (Pn10=0)
		public int ZMas_Pn11 = 0;                    // VCF4PaneMem (Pn11=0)
		public int ZMas_Rack0 = 54;                  // binary data for MainRackMem (Rack0=54)
		public int ZMas_Rack1 = 55;                  // binary data for ModRackMem (Rack1=55)
		
		public int strangeNumberBeforeBinaryChunk = 0;
		public string UglyCompressedBinaryData = ""; // uglyCompressedBinaryData
		#endregion
		
		public Zebra2Preset()
		{
		}

		public Zebra2Preset(string filePath)
		{
			int counter = 0;
			string line;
			bool startSavingBinaryData = false;
			StringBuilder uglyCompressedBinaryData = new StringBuilder();
			
			// Read the file line by line.
			StreamReader file = new StreamReader(filePath);
			string storedSectionName = "";
			while((line = file.ReadLine()) != null)
			{
				if (startSavingBinaryData) {
					uglyCompressedBinaryData.Append(line);
				} else if (line.Contains("=")) {
					// look for #cm= entries
					if (line.StartsWith("#cm")) {
						// new section
						var section = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
						var sectionpair = new KeyValuePair<string, string>(section[0], section[1]);
						storedSectionName = StringUtils.ConvertCaseString(sectionpair.Value, StringUtils.Case.PascalCase);
					} else {
						if (storedSectionName != "") {
							var parameters = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
							var parameter = new KeyValuePair<string, string>(parameters[0], parameters[1]);

							string paramkey = parameter.Key;
							string paramvalue = parameter.Value;
							paramkey = StringUtils.ConvertCaseString(StringUtils.RemoveInvalidCharacters(paramkey), StringUtils.Case.PascalCase);

							bool IsValueStringOrInt = false;
							bool IsValueString = false;
							string valueType = "";
							object val = null;
							if (paramvalue.Contains(".")) {
								float f = 0.0f;
								try {
									f = float.Parse(paramvalue, CultureInfo.InvariantCulture);
									val = f;
									valueType = "float";
								} catch (Exception) {
									// is not a float, must be string or int
									IsValueStringOrInt = true;
								}
							} else {
								// string or int
								IsValueStringOrInt = true;
							}
							if (IsValueStringOrInt) {
								int i = 0;
								try {
									i = int.Parse(paramvalue);
									val = i;
									valueType = "int";
								} catch (Exception) {
									// not float and not int, equals string
									IsValueString = true;
								}
							}
							if (IsValueString) {
								string v = paramvalue.Trim();
								val = v;
								valueType = "string";
							}
							
							string fieldName = String.Format("{0}_{1}", storedSectionName, paramkey);
							
							// use reflection to try to add correct field based on the loaded preset
							try {
								var type = typeof(Zebra2Preset);
								var field = type.GetField(fieldName);
								field.SetValue(this, val);
							} catch (Exception) {
								Console.Out.WriteLine("Could not find field {0} and store {1} {2}!", fieldName, valueType, val);
							}
						}
					}
				} else {
					if (line.StartsWith("$$$$")) {
						strangeNumberBeforeBinaryChunk = int.Parse(line.Substring(4));

						// start saving in buffer
						startSavingBinaryData = true;
					}
				}
				counter++;
			}
			
			UglyCompressedBinaryData = uglyCompressedBinaryData.ToString();
			file.Close();

			Console.Out.WriteLine("Finished reading preset file {0} ...", filePath);
		}

		// Read a zebra 2 native preset (initialize extended format) and create all the class fields
		public void GenerateClassFields(string inFilePath, string outFilePath)
		{
			int strangeNumberBeforeBinaryChunk = 0;
			int counter = 0;
			string line;
			bool startSavingBinaryData = false;
			StringBuilder uglyCompressedBinaryData = new StringBuilder();
			
			TextWriter tw = new StreamWriter(outFilePath);
			tw.WriteLine("#region Zebra2 Fields");
			
			// Read the file line by line.
			StreamReader file = new StreamReader(inFilePath);
			string storedSectionName = "";
			while((line = file.ReadLine()) != null)
			{
				if (startSavingBinaryData) {
					uglyCompressedBinaryData.Append(line);
				} else if (line.Contains("=")) {
					// look for #cm= entries
					if (line.StartsWith("#cm")) {
						// new section
						var section = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
						var sectionpair = new KeyValuePair<string, string>(section[0], section[1]);
						storedSectionName = StringUtils.ConvertCaseString(sectionpair.Value, StringUtils.Case.PascalCase);
						tw.WriteLine ("\n// Section: {0} ({1}={2})", storedSectionName, sectionpair.Key, sectionpair.Value);
					} else {
						if (storedSectionName != "") {
							var parameters = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
							var parameter = new KeyValuePair<string, string>(parameters[0], parameters[1]);

							string paramkey = parameter.Key;
							string paramvalue = parameter.Value.Trim();

							string comment = "";
							if (parameters.Length > 2 && parameters[2] != null) {
								comment = String.Format("// {0} ({1}={2})", parameters[2].Trim(), paramkey, paramvalue);
							}
							
							// formatting
							paramkey = StringUtils.ConvertCaseString(StringUtils.RemoveInvalidCharacters(paramkey), StringUtils.Case.PascalCase);

							bool IsValueStringOrInt = false;
							bool IsValueString = false;
							string valueType = "";
							object val = null;
							if (paramvalue.Contains(".")) {
								float f = 0.0f;
								try {
									f = float.Parse(paramvalue, CultureInfo.InvariantCulture);
									val = string.Format(CultureInfo.InvariantCulture, "{0:00.00}f", f);
									valueType = "float";
								} catch (Exception) {
									// is not a float, must be string or int
									IsValueStringOrInt = true;
								}
							} else {
								// string or int
								IsValueStringOrInt = true;
							}
							if (IsValueStringOrInt) {
								int i = 0;
								try {
									i = int.Parse(paramvalue);
									val = i;
									valueType = "int";
								} catch (Exception) {
									// not float and not int, equals string
									IsValueString = true;
								}
							}
							if (IsValueString) {
								string v = paramvalue.Trim();
								val = String.Format("\"{0}\"", v);
								valueType = "string";
							}
							
							if (comment == null) {
								tw.WriteLine("public {0} {1}_{2} = {3};", valueType, storedSectionName, paramkey, val);
							} else {
								StringBuilder outWithComment = new StringBuilder();
								outWithComment.Append(String.Format("public {0} {1}_{2} = {3};", valueType, storedSectionName, paramkey, val).PadRight(45)).Append(comment);
								tw.WriteLine(outWithComment.ToString());
							}
						}
					}
				} else {
					if (line.StartsWith("$$$$")) {
						strangeNumberBeforeBinaryChunk = int.Parse(line.Substring(4));

						// start saving in buffer
						startSavingBinaryData = true;
					}
				}
				counter++;
			}

			// skip the actual binary data
			//tw.WriteLine(uglyCompressedBinaryData.ToString());
			
			tw.WriteLine("public string UglyCompressedBinaryData = \"\"; // uglyCompressedBinaryData");
			tw.WriteLine("#endregion");
			
			tw.Close();

			file.Close();
		}

		// Read a zebra 2 native preset (initialize extended format) and create a Write Preset method
		public void GenerateWriteMethod(string inFilePath, string outFilePath)
		{
			int strangeNumberBeforeBinaryChunk = 0;
			int counter = 0;
			string line;
			bool startSavingBinaryData = false;
			StringBuilder uglyCompressedBinaryData = new StringBuilder();
			
			TextWriter tw = new StreamWriter(outFilePath);
			tw.WriteLine("#region Zebra2 Write Preset Method");
			
			tw.WriteLine("public string GeneratePresetContentNew() {");
			tw.WriteLine("\tStringBuilder buffer = new StringBuilder();");
			
			// Read the file line by line.
			StreamReader file = new StreamReader(inFilePath);
			string storedSectionName = "";
			while((line = file.ReadLine()) != null)
			{
				if (startSavingBinaryData) {
					uglyCompressedBinaryData.Append(line);
				} else if (line.Contains("=")) {
					// look for #cm= entries
					if (line.StartsWith("#cm")) {
						// new section
						var section = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
						var sectionpair = new KeyValuePair<string, string>(section[0], section[1]);
						storedSectionName = StringUtils.ConvertCaseString(sectionpair.Value, StringUtils.Case.PascalCase);
						tw.WriteLine("\n\tbuffer.AppendLine(\"\\n{0}={1}\");", sectionpair.Key, sectionpair.Value);
					} else {
						if (storedSectionName != "") {
							var parameters = line.Split( new string[] { "=",  "//" }, StringSplitOptions.None );
							var parameter = new KeyValuePair<string, string>(parameters[0], parameters[1]);

							string paramkey = parameter.Key;
							string paramvalue = parameter.Value;

							string comment = "";
							if (parameters.Length > 2 && parameters[2] != null) {
								comment = String.Format("// {0}", parameters[2].Trim());
							}
							
							// formatting
							paramkey = StringUtils.ConvertCaseString(StringUtils.RemoveInvalidCharacters(paramkey), StringUtils.Case.PascalCase);

							bool IsValueStringOrInt = false;
							bool IsValueString = false;
							object val = null;
							if (paramvalue.Contains(".")) {
								float f = 0.0f;
								try {
									f = float.Parse(paramvalue, CultureInfo.InvariantCulture);
									val = string.Format(CultureInfo.InvariantCulture, "{0:00.00}f", f);
									tw.WriteLine("\tbuffer.Append(String.Format(CultureInfo.InvariantCulture, \"{0}={{0:0.00}}\", {1}_{2}).PadRight(20)).AppendLine(\"{3}\");", parameter.Key, storedSectionName, paramkey, comment);
								} catch (Exception) {
									// is not a float, must be string or int
									IsValueStringOrInt = true;
								}
							} else {
								// string or int
								IsValueStringOrInt = true;
							}
							if (IsValueStringOrInt) {
								int i = 0;
								try {
									i = int.Parse(paramvalue);
									val = i;
									tw.WriteLine("\tbuffer.Append(String.Format(\"{0}={{0}}\", {1}_{2}).PadRight(20)).AppendLine(\"{3}\");", parameter.Key, storedSectionName, paramkey, comment);
								} catch (Exception) {
									// not float and not int, equals string
									IsValueString = true;
								}
							}
							if (IsValueString) {
								string v = paramvalue.Trim();
								val = String.Format("\"{0}\"", v);
								tw.WriteLine("\tbuffer.Append(String.Format(\"{0}={{0}}\", {1}_{2}).PadRight(20)).AppendLine(\"{3}\");", parameter.Key, storedSectionName, paramkey, comment);
							}
						}
					}
				} else {
					if (line.StartsWith("$$$$")) {
						strangeNumberBeforeBinaryChunk = int.Parse(line.Substring(4));

						// start saving in buffer
						startSavingBinaryData = true;
					}
				}
				counter++;
			}

			tw.WriteLine();
			tw.WriteLine("\tbuffer.Append(UglyCompressedBinaryData); // uglyCompressedBinaryData");
			tw.WriteLine("\treturn buffer.ToString();");
			tw.WriteLine("}");
			tw.WriteLine("#endregion");
			
			tw.Close();
			file.Close();
		}
		
		#region Zebra2 GeneratePresetContent Method
		public string GeneratePresetContent() {
			StringBuilder buffer = new StringBuilder();

			buffer.Append(GeneratePresetHeader());
			buffer.Append(GenerateModulatorReferenceTable());
			
			buffer.AppendLine("\n#cm=main");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "CcOp={0:0.00}", Main_CcOp).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format("#LFOG={0}", Main_LFOG).PadRight(20)).AppendLine("// Active #LFOG");
			buffer.Append(String.Format("#LFOG2={0}", Main_LFOG2).PadRight(20)).AppendLine("// Active #LFOG2");

			buffer.AppendLine("\n#cm=PCore");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_1={0:0.00}", PCore_X_1).PadRight(20)).AppendLine("// X1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_1={0:0.00}", PCore_Y_1).PadRight(20)).AppendLine("// Y1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_2={0:0.00}", PCore_X_2).PadRight(20)).AppendLine("// X2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_2={0:0.00}", PCore_Y_2).PadRight(20)).AppendLine("// Y2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_3={0:0.00}", PCore_X_3).PadRight(20)).AppendLine("// X3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_3={0:0.00}", PCore_Y_3).PadRight(20)).AppendLine("// Y3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "X_4={0:0.00}", PCore_X_4).PadRight(20)).AppendLine("// X4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Y_4={0:0.00}", PCore_Y_4).PadRight(20)).AppendLine("// Y4");
			buffer.Append(String.Format("MT11={0}", PCore_MT11).PadRight(20)).AppendLine("// XY1 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML11={0:0.00}", PCore_ML11).PadRight(20)).AppendLine("// XY1 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR11={0:0.00}", PCore_MR11).PadRight(20)).AppendLine("// XY1 Left1");
			buffer.Append(String.Format("MT12={0}", PCore_MT12).PadRight(20)).AppendLine("// XY1 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML12={0:0.00}", PCore_ML12).PadRight(20)).AppendLine("// XY1 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR12={0:0.00}", PCore_MR12).PadRight(20)).AppendLine("// XY1 Left2");
			buffer.Append(String.Format("MT13={0}", PCore_MT13).PadRight(20)).AppendLine("// XY1 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML13={0:0.00}", PCore_ML13).PadRight(20)).AppendLine("// XY1 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR13={0:0.00}", PCore_MR13).PadRight(20)).AppendLine("// XY1 Left3");
			buffer.Append(String.Format("MT14={0}", PCore_MT14).PadRight(20)).AppendLine("// XY1 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML14={0:0.00}", PCore_ML14).PadRight(20)).AppendLine("// XY1 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR14={0:0.00}", PCore_MR14).PadRight(20)).AppendLine("// XY1 Left4");
			buffer.Append(String.Format("MT15={0}", PCore_MT15).PadRight(20)).AppendLine("// XY1 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML15={0:0.00}", PCore_ML15).PadRight(20)).AppendLine("// XY1 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR15={0:0.00}", PCore_MR15).PadRight(20)).AppendLine("// XY1 Left5");
			buffer.Append(String.Format("MT16={0}", PCore_MT16).PadRight(20)).AppendLine("// XY1 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML16={0:0.00}", PCore_ML16).PadRight(20)).AppendLine("// XY1 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR16={0:0.00}", PCore_MR16).PadRight(20)).AppendLine("// XY1 Left6");
			buffer.Append(String.Format("MT17={0}", PCore_MT17).PadRight(20)).AppendLine("// XY1 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML17={0:0.00}", PCore_ML17).PadRight(20)).AppendLine("// XY1 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR17={0:0.00}", PCore_MR17).PadRight(20)).AppendLine("// XY1 Left7");
			buffer.Append(String.Format("MT18={0}", PCore_MT18).PadRight(20)).AppendLine("// XY1 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML18={0:0.00}", PCore_ML18).PadRight(20)).AppendLine("// XY1 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR18={0:0.00}", PCore_MR18).PadRight(20)).AppendLine("// XY1 Left8");
			buffer.Append(String.Format("MT21={0}", PCore_MT21).PadRight(20)).AppendLine("// XY1 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML21={0:0.00}", PCore_ML21).PadRight(20)).AppendLine("// XY1 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR21={0:0.00}", PCore_MR21).PadRight(20)).AppendLine("// XY1 Down1");
			buffer.Append(String.Format("MT22={0}", PCore_MT22).PadRight(20)).AppendLine("// XY1 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML22={0:0.00}", PCore_ML22).PadRight(20)).AppendLine("// XY1 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR22={0:0.00}", PCore_MR22).PadRight(20)).AppendLine("// XY1 Down2");
			buffer.Append(String.Format("MT23={0}", PCore_MT23).PadRight(20)).AppendLine("// XY1 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML23={0:0.00}", PCore_ML23).PadRight(20)).AppendLine("// XY1 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR23={0:0.00}", PCore_MR23).PadRight(20)).AppendLine("// XY1 Down3");
			buffer.Append(String.Format("MT24={0}", PCore_MT24).PadRight(20)).AppendLine("// XY1 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML24={0:0.00}", PCore_ML24).PadRight(20)).AppendLine("// XY1 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR24={0:0.00}", PCore_MR24).PadRight(20)).AppendLine("// XY1 Down4");
			buffer.Append(String.Format("MT25={0}", PCore_MT25).PadRight(20)).AppendLine("// XY1 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML25={0:0.00}", PCore_ML25).PadRight(20)).AppendLine("// XY1 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR25={0:0.00}", PCore_MR25).PadRight(20)).AppendLine("// XY1 Down5");
			buffer.Append(String.Format("MT26={0}", PCore_MT26).PadRight(20)).AppendLine("// XY1 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML26={0:0.00}", PCore_ML26).PadRight(20)).AppendLine("// XY1 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR26={0:0.00}", PCore_MR26).PadRight(20)).AppendLine("// XY1 Down6");
			buffer.Append(String.Format("MT27={0}", PCore_MT27).PadRight(20)).AppendLine("// XY1 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML27={0:0.00}", PCore_ML27).PadRight(20)).AppendLine("// XY1 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR27={0:0.00}", PCore_MR27).PadRight(20)).AppendLine("// XY1 Down7");
			buffer.Append(String.Format("MT28={0}", PCore_MT28).PadRight(20)).AppendLine("// XY1 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML28={0:0.00}", PCore_ML28).PadRight(20)).AppendLine("// XY1 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR28={0:0.00}", PCore_MR28).PadRight(20)).AppendLine("// XY1 Down8");
			buffer.Append(String.Format("MT31={0}", PCore_MT31).PadRight(20)).AppendLine("// XY2 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML31={0:0.00}", PCore_ML31).PadRight(20)).AppendLine("// XY2 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR31={0:0.00}", PCore_MR31).PadRight(20)).AppendLine("// XY2 Left1");
			buffer.Append(String.Format("MT32={0}", PCore_MT32).PadRight(20)).AppendLine("// XY2 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML32={0:0.00}", PCore_ML32).PadRight(20)).AppendLine("// XY2 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR32={0:0.00}", PCore_MR32).PadRight(20)).AppendLine("// XY2 Left2");
			buffer.Append(String.Format("MT33={0}", PCore_MT33).PadRight(20)).AppendLine("// XY2 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML33={0:0.00}", PCore_ML33).PadRight(20)).AppendLine("// XY2 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR33={0:0.00}", PCore_MR33).PadRight(20)).AppendLine("// XY2 Left3");
			buffer.Append(String.Format("MT34={0}", PCore_MT34).PadRight(20)).AppendLine("// XY2 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML34={0:0.00}", PCore_ML34).PadRight(20)).AppendLine("// XY2 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR34={0:0.00}", PCore_MR34).PadRight(20)).AppendLine("// XY2 Left4");
			buffer.Append(String.Format("MT35={0}", PCore_MT35).PadRight(20)).AppendLine("// XY2 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML35={0:0.00}", PCore_ML35).PadRight(20)).AppendLine("// XY2 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR35={0:0.00}", PCore_MR35).PadRight(20)).AppendLine("// XY2 Left5");
			buffer.Append(String.Format("MT36={0}", PCore_MT36).PadRight(20)).AppendLine("// XY2 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML36={0:0.00}", PCore_ML36).PadRight(20)).AppendLine("// XY2 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR36={0:0.00}", PCore_MR36).PadRight(20)).AppendLine("// XY2 Left6");
			buffer.Append(String.Format("MT37={0}", PCore_MT37).PadRight(20)).AppendLine("// XY2 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML37={0:0.00}", PCore_ML37).PadRight(20)).AppendLine("// XY2 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR37={0:0.00}", PCore_MR37).PadRight(20)).AppendLine("// XY2 Left7");
			buffer.Append(String.Format("MT38={0}", PCore_MT38).PadRight(20)).AppendLine("// XY2 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML38={0:0.00}", PCore_ML38).PadRight(20)).AppendLine("// XY2 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR38={0:0.00}", PCore_MR38).PadRight(20)).AppendLine("// XY2 Left8");
			buffer.Append(String.Format("MT41={0}", PCore_MT41).PadRight(20)).AppendLine("// XY2 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML41={0:0.00}", PCore_ML41).PadRight(20)).AppendLine("// XY2 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR41={0:0.00}", PCore_MR41).PadRight(20)).AppendLine("// XY2 Down1");
			buffer.Append(String.Format("MT42={0}", PCore_MT42).PadRight(20)).AppendLine("// XY2 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML42={0:0.00}", PCore_ML42).PadRight(20)).AppendLine("// XY2 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR42={0:0.00}", PCore_MR42).PadRight(20)).AppendLine("// XY2 Down2");
			buffer.Append(String.Format("MT43={0}", PCore_MT43).PadRight(20)).AppendLine("// XY2 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML43={0:0.00}", PCore_ML43).PadRight(20)).AppendLine("// XY2 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR43={0:0.00}", PCore_MR43).PadRight(20)).AppendLine("// XY2 Down3");
			buffer.Append(String.Format("MT44={0}", PCore_MT44).PadRight(20)).AppendLine("// XY2 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML44={0:0.00}", PCore_ML44).PadRight(20)).AppendLine("// XY2 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR44={0:0.00}", PCore_MR44).PadRight(20)).AppendLine("// XY2 Down4");
			buffer.Append(String.Format("MT45={0}", PCore_MT45).PadRight(20)).AppendLine("// XY2 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML45={0:0.00}", PCore_ML45).PadRight(20)).AppendLine("// XY2 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR45={0:0.00}", PCore_MR45).PadRight(20)).AppendLine("// XY2 Down5");
			buffer.Append(String.Format("MT46={0}", PCore_MT46).PadRight(20)).AppendLine("// XY2 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML46={0:0.00}", PCore_ML46).PadRight(20)).AppendLine("// XY2 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR46={0:0.00}", PCore_MR46).PadRight(20)).AppendLine("// XY2 Down6");
			buffer.Append(String.Format("MT47={0}", PCore_MT47).PadRight(20)).AppendLine("// XY2 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML47={0:0.00}", PCore_ML47).PadRight(20)).AppendLine("// XY2 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR47={0:0.00}", PCore_MR47).PadRight(20)).AppendLine("// XY2 Down7");
			buffer.Append(String.Format("MT48={0}", PCore_MT48).PadRight(20)).AppendLine("// XY2 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML48={0:0.00}", PCore_ML48).PadRight(20)).AppendLine("// XY2 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR48={0:0.00}", PCore_MR48).PadRight(20)).AppendLine("// XY2 Down8");
			buffer.Append(String.Format("MT51={0}", PCore_MT51).PadRight(20)).AppendLine("// XY3 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML51={0:0.00}", PCore_ML51).PadRight(20)).AppendLine("// XY3 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR51={0:0.00}", PCore_MR51).PadRight(20)).AppendLine("// XY3 Left1");
			buffer.Append(String.Format("MT52={0}", PCore_MT52).PadRight(20)).AppendLine("// XY3 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML52={0:0.00}", PCore_ML52).PadRight(20)).AppendLine("// XY3 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR52={0:0.00}", PCore_MR52).PadRight(20)).AppendLine("// XY3 Left2");
			buffer.Append(String.Format("MT53={0}", PCore_MT53).PadRight(20)).AppendLine("// XY3 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML53={0:0.00}", PCore_ML53).PadRight(20)).AppendLine("// XY3 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR53={0:0.00}", PCore_MR53).PadRight(20)).AppendLine("// XY3 Left3");
			buffer.Append(String.Format("MT54={0}", PCore_MT54).PadRight(20)).AppendLine("// XY3 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML54={0:0.00}", PCore_ML54).PadRight(20)).AppendLine("// XY3 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR54={0:0.00}", PCore_MR54).PadRight(20)).AppendLine("// XY3 Left4");
			buffer.Append(String.Format("MT55={0}", PCore_MT55).PadRight(20)).AppendLine("// XY3 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML55={0:0.00}", PCore_ML55).PadRight(20)).AppendLine("// XY3 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR55={0:0.00}", PCore_MR55).PadRight(20)).AppendLine("// XY3 Left5");
			buffer.Append(String.Format("MT56={0}", PCore_MT56).PadRight(20)).AppendLine("// XY3 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML56={0:0.00}", PCore_ML56).PadRight(20)).AppendLine("// XY3 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR56={0:0.00}", PCore_MR56).PadRight(20)).AppendLine("// XY3 Left6");
			buffer.Append(String.Format("MT57={0}", PCore_MT57).PadRight(20)).AppendLine("// XY3 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML57={0:0.00}", PCore_ML57).PadRight(20)).AppendLine("// XY3 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR57={0:0.00}", PCore_MR57).PadRight(20)).AppendLine("// XY3 Left7");
			buffer.Append(String.Format("MT58={0}", PCore_MT58).PadRight(20)).AppendLine("// XY3 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML58={0:0.00}", PCore_ML58).PadRight(20)).AppendLine("// XY3 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR58={0:0.00}", PCore_MR58).PadRight(20)).AppendLine("// XY3 Left8");
			buffer.Append(String.Format("MT61={0}", PCore_MT61).PadRight(20)).AppendLine("// XY3 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML61={0:0.00}", PCore_ML61).PadRight(20)).AppendLine("// XY3 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR61={0:0.00}", PCore_MR61).PadRight(20)).AppendLine("// XY3 Down1");
			buffer.Append(String.Format("MT62={0}", PCore_MT62).PadRight(20)).AppendLine("// XY3 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML62={0:0.00}", PCore_ML62).PadRight(20)).AppendLine("// XY3 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR62={0:0.00}", PCore_MR62).PadRight(20)).AppendLine("// XY3 Down2");
			buffer.Append(String.Format("MT63={0}", PCore_MT63).PadRight(20)).AppendLine("// XY3 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML63={0:0.00}", PCore_ML63).PadRight(20)).AppendLine("// XY3 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR63={0:0.00}", PCore_MR63).PadRight(20)).AppendLine("// XY3 Down3");
			buffer.Append(String.Format("MT64={0}", PCore_MT64).PadRight(20)).AppendLine("// XY3 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML64={0:0.00}", PCore_ML64).PadRight(20)).AppendLine("// XY3 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR64={0:0.00}", PCore_MR64).PadRight(20)).AppendLine("// XY3 Down4");
			buffer.Append(String.Format("MT65={0}", PCore_MT65).PadRight(20)).AppendLine("// XY3 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML65={0:0.00}", PCore_ML65).PadRight(20)).AppendLine("// XY3 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR65={0:0.00}", PCore_MR65).PadRight(20)).AppendLine("// XY3 Down5");
			buffer.Append(String.Format("MT66={0}", PCore_MT66).PadRight(20)).AppendLine("// XY3 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML66={0:0.00}", PCore_ML66).PadRight(20)).AppendLine("// XY3 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR66={0:0.00}", PCore_MR66).PadRight(20)).AppendLine("// XY3 Down6");
			buffer.Append(String.Format("MT67={0}", PCore_MT67).PadRight(20)).AppendLine("// XY3 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML67={0:0.00}", PCore_ML67).PadRight(20)).AppendLine("// XY3 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR67={0:0.00}", PCore_MR67).PadRight(20)).AppendLine("// XY3 Down7");
			buffer.Append(String.Format("MT68={0}", PCore_MT68).PadRight(20)).AppendLine("// XY3 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML68={0:0.00}", PCore_ML68).PadRight(20)).AppendLine("// XY3 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR68={0:0.00}", PCore_MR68).PadRight(20)).AppendLine("// XY3 Down8");
			buffer.Append(String.Format("MT71={0}", PCore_MT71).PadRight(20)).AppendLine("// XY4 TargetX1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML71={0:0.00}", PCore_ML71).PadRight(20)).AppendLine("// XY4 Right1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR71={0:0.00}", PCore_MR71).PadRight(20)).AppendLine("// XY4 Left1");
			buffer.Append(String.Format("MT72={0}", PCore_MT72).PadRight(20)).AppendLine("// XY4 TargetX2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML72={0:0.00}", PCore_ML72).PadRight(20)).AppendLine("// XY4 Right2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR72={0:0.00}", PCore_MR72).PadRight(20)).AppendLine("// XY4 Left2");
			buffer.Append(String.Format("MT73={0}", PCore_MT73).PadRight(20)).AppendLine("// XY4 TargetX3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML73={0:0.00}", PCore_ML73).PadRight(20)).AppendLine("// XY4 Right3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR73={0:0.00}", PCore_MR73).PadRight(20)).AppendLine("// XY4 Left3");
			buffer.Append(String.Format("MT74={0}", PCore_MT74).PadRight(20)).AppendLine("// XY4 TargetX4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML74={0:0.00}", PCore_ML74).PadRight(20)).AppendLine("// XY4 Right4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR74={0:0.00}", PCore_MR74).PadRight(20)).AppendLine("// XY4 Left4");
			buffer.Append(String.Format("MT75={0}", PCore_MT75).PadRight(20)).AppendLine("// XY4 TargetX5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML75={0:0.00}", PCore_ML75).PadRight(20)).AppendLine("// XY4 Right5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR75={0:0.00}", PCore_MR75).PadRight(20)).AppendLine("// XY4 Left5");
			buffer.Append(String.Format("MT76={0}", PCore_MT76).PadRight(20)).AppendLine("// XY4 TargetX6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML76={0:0.00}", PCore_ML76).PadRight(20)).AppendLine("// XY4 Right6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR76={0:0.00}", PCore_MR76).PadRight(20)).AppendLine("// XY4 Left6");
			buffer.Append(String.Format("MT77={0}", PCore_MT77).PadRight(20)).AppendLine("// XY4 TargetX7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML77={0:0.00}", PCore_ML77).PadRight(20)).AppendLine("// XY4 Right7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR77={0:0.00}", PCore_MR77).PadRight(20)).AppendLine("// XY4 Left7");
			buffer.Append(String.Format("MT78={0}", PCore_MT78).PadRight(20)).AppendLine("// XY4 TargetX8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML78={0:0.00}", PCore_ML78).PadRight(20)).AppendLine("// XY4 Right8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR78={0:0.00}", PCore_MR78).PadRight(20)).AppendLine("// XY4 Left8");
			buffer.Append(String.Format("MT81={0}", PCore_MT81).PadRight(20)).AppendLine("// XY4 TargetY1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML81={0:0.00}", PCore_ML81).PadRight(20)).AppendLine("// XY4 Up1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR81={0:0.00}", PCore_MR81).PadRight(20)).AppendLine("// XY4 Down1");
			buffer.Append(String.Format("MT82={0}", PCore_MT82).PadRight(20)).AppendLine("// XY4 TargetY2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML82={0:0.00}", PCore_ML82).PadRight(20)).AppendLine("// XY4 Up2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR82={0:0.00}", PCore_MR82).PadRight(20)).AppendLine("// XY4 Down2");
			buffer.Append(String.Format("MT83={0}", PCore_MT83).PadRight(20)).AppendLine("// XY4 TargetY3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML83={0:0.00}", PCore_ML83).PadRight(20)).AppendLine("// XY4 Up3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR83={0:0.00}", PCore_MR83).PadRight(20)).AppendLine("// XY4 Down3");
			buffer.Append(String.Format("MT84={0}", PCore_MT84).PadRight(20)).AppendLine("// XY4 TargetY4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML84={0:0.00}", PCore_ML84).PadRight(20)).AppendLine("// XY4 Up4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR84={0:0.00}", PCore_MR84).PadRight(20)).AppendLine("// XY4 Down4");
			buffer.Append(String.Format("MT85={0}", PCore_MT85).PadRight(20)).AppendLine("// XY4 TargetY5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML85={0:0.00}", PCore_ML85).PadRight(20)).AppendLine("// XY4 Up5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR85={0:0.00}", PCore_MR85).PadRight(20)).AppendLine("// XY4 Down5");
			buffer.Append(String.Format("MT86={0}", PCore_MT86).PadRight(20)).AppendLine("// XY4 TargetY6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML86={0:0.00}", PCore_ML86).PadRight(20)).AppendLine("// XY4 Up6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR86={0:0.00}", PCore_MR86).PadRight(20)).AppendLine("// XY4 Down6");
			buffer.Append(String.Format("MT87={0}", PCore_MT87).PadRight(20)).AppendLine("// XY4 TargetY7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML87={0:0.00}", PCore_ML87).PadRight(20)).AppendLine("// XY4 Up7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR87={0:0.00}", PCore_MR87).PadRight(20)).AppendLine("// XY4 Down7");
			buffer.Append(String.Format("MT88={0}", PCore_MT88).PadRight(20)).AppendLine("// XY4 TargetY8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ML88={0:0.00}", PCore_ML88).PadRight(20)).AppendLine("// XY4 Up8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MR88={0:0.00}", PCore_MR88).PadRight(20)).AppendLine("// XY4 Down8");
			buffer.Append(String.Format("MMT1={0}", PCore_MMT1).PadRight(20)).AppendLine("// Matrix1 Target");
			buffer.Append(String.Format("MMS1={0}", PCore_MMS1).PadRight(20)).AppendLine("// Matrix1 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD1={0:0.00}", PCore_MMD1).PadRight(20)).AppendLine("// Matrix1 Depth");
			buffer.Append(String.Format("MMVS1={0}", PCore_MMVS1).PadRight(20)).AppendLine("// Matrix1 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD1={0:0.00}", PCore_MMVD1).PadRight(20)).AppendLine("// Matrix1 Via");
			buffer.Append(String.Format("MMT2={0}", PCore_MMT2).PadRight(20)).AppendLine("// Matrix2 Target");
			buffer.Append(String.Format("MMS2={0}", PCore_MMS2).PadRight(20)).AppendLine("// Matrix2 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD2={0:0.00}", PCore_MMD2).PadRight(20)).AppendLine("// Matrix2 Depth");
			buffer.Append(String.Format("MMVS2={0}", PCore_MMVS2).PadRight(20)).AppendLine("// Matrix2 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD2={0:0.00}", PCore_MMVD2).PadRight(20)).AppendLine("// Matrix2 Via");
			buffer.Append(String.Format("MMT3={0}", PCore_MMT3).PadRight(20)).AppendLine("// Matrix3 Target");
			buffer.Append(String.Format("MMS3={0}", PCore_MMS3).PadRight(20)).AppendLine("// Matrix3 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD3={0:0.00}", PCore_MMD3).PadRight(20)).AppendLine("// Matrix3 Depth");
			buffer.Append(String.Format("MMVS3={0}", PCore_MMVS3).PadRight(20)).AppendLine("// Matrix3 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD3={0:0.00}", PCore_MMVD3).PadRight(20)).AppendLine("// Matrix3 Via");
			buffer.Append(String.Format("MMT4={0}", PCore_MMT4).PadRight(20)).AppendLine("// Matrix4 Target");
			buffer.Append(String.Format("MMS4={0}", PCore_MMS4).PadRight(20)).AppendLine("// Matrix4 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD4={0:0.00}", PCore_MMD4).PadRight(20)).AppendLine("// Matrix4 Depth");
			buffer.Append(String.Format("MMVS4={0}", PCore_MMVS4).PadRight(20)).AppendLine("// Matrix4 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD4={0:0.00}", PCore_MMVD4).PadRight(20)).AppendLine("// Matrix4 Via");
			buffer.Append(String.Format("MMT5={0}", PCore_MMT5).PadRight(20)).AppendLine("// Matrix5 Target");
			buffer.Append(String.Format("MMS5={0}", PCore_MMS5).PadRight(20)).AppendLine("// Matrix5 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD5={0:0.00}", PCore_MMD5).PadRight(20)).AppendLine("// Matrix5 Depth");
			buffer.Append(String.Format("MMVS5={0}", PCore_MMVS5).PadRight(20)).AppendLine("// Matrix5 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD5={0:0.00}", PCore_MMVD5).PadRight(20)).AppendLine("// Matrix5 Via");
			buffer.Append(String.Format("MMT6={0}", PCore_MMT6).PadRight(20)).AppendLine("// Matrix6 Target");
			buffer.Append(String.Format("MMS6={0}", PCore_MMS6).PadRight(20)).AppendLine("// Matrix6 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD6={0:0.00}", PCore_MMD6).PadRight(20)).AppendLine("// Matrix6 Depth");
			buffer.Append(String.Format("MMVS6={0}", PCore_MMVS6).PadRight(20)).AppendLine("// Matrix6 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD6={0:0.00}", PCore_MMVD6).PadRight(20)).AppendLine("// Matrix6 Via");
			buffer.Append(String.Format("MMT7={0}", PCore_MMT7).PadRight(20)).AppendLine("// Matrix7 Target");
			buffer.Append(String.Format("MMS7={0}", PCore_MMS7).PadRight(20)).AppendLine("// Matrix7 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD7={0:0.00}", PCore_MMD7).PadRight(20)).AppendLine("// Matrix7 Depth");
			buffer.Append(String.Format("MMVS7={0}", PCore_MMVS7).PadRight(20)).AppendLine("// Matrix7 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD7={0:0.00}", PCore_MMVD7).PadRight(20)).AppendLine("// Matrix7 Via");
			buffer.Append(String.Format("MMT8={0}", PCore_MMT8).PadRight(20)).AppendLine("// Matrix8 Target");
			buffer.Append(String.Format("MMS8={0}", PCore_MMS8).PadRight(20)).AppendLine("// Matrix8 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD8={0:0.00}", PCore_MMD8).PadRight(20)).AppendLine("// Matrix8 Depth");
			buffer.Append(String.Format("MMVS8={0}", PCore_MMVS8).PadRight(20)).AppendLine("// Matrix8 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD8={0:0.00}", PCore_MMVD8).PadRight(20)).AppendLine("// Matrix8 Via");
			buffer.Append(String.Format("MMT9={0}", PCore_MMT9).PadRight(20)).AppendLine("// Matrix9 Target");
			buffer.Append(String.Format("MMS9={0}", PCore_MMS9).PadRight(20)).AppendLine("// Matrix9 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD9={0:0.00}", PCore_MMD9).PadRight(20)).AppendLine("// Matrix9 Depth");
			buffer.Append(String.Format("MMVS9={0}", PCore_MMVS9).PadRight(20)).AppendLine("// Matrix9 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD9={0:0.00}", PCore_MMVD9).PadRight(20)).AppendLine("// Matrix9 Via");
			buffer.Append(String.Format("MMT10={0}", PCore_MMT10).PadRight(20)).AppendLine("// Matrix10 Target");
			buffer.Append(String.Format("MMS10={0}", PCore_MMS10).PadRight(20)).AppendLine("// Matrix10 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD10={0:0.00}", PCore_MMD10).PadRight(20)).AppendLine("// Matrix10 Depth");
			buffer.Append(String.Format("MMVS10={0}", PCore_MMVS10).PadRight(20)).AppendLine("// Matrix10 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD10={0:0.00}", PCore_MMVD10).PadRight(20)).AppendLine("// Matrix10 Via");
			buffer.Append(String.Format("MMT11={0}", PCore_MMT11).PadRight(20)).AppendLine("// Matrix11 Target");
			buffer.Append(String.Format("MMS11={0}", PCore_MMS11).PadRight(20)).AppendLine("// Matrix11 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD11={0:0.00}", PCore_MMD11).PadRight(20)).AppendLine("// Matrix11 Depth");
			buffer.Append(String.Format("MMVS11={0}", PCore_MMVS11).PadRight(20)).AppendLine("// Matrix11 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD11={0:0.00}", PCore_MMVD11).PadRight(20)).AppendLine("// Matrix11 Via");
			buffer.Append(String.Format("MMT12={0}", PCore_MMT12).PadRight(20)).AppendLine("// Matrix12 Target");
			buffer.Append(String.Format("MMS12={0}", PCore_MMS12).PadRight(20)).AppendLine("// Matrix12 Source");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMD12={0:0.00}", PCore_MMD12).PadRight(20)).AppendLine("// Matrix12 Depth");
			buffer.Append(String.Format("MMVS12={0}", PCore_MMVS12).PadRight(20)).AppendLine("// Matrix12 ViaSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMVD12={0:0.00}", PCore_MMVD12).PadRight(20)).AppendLine("// Matrix12 Via");
			buffer.Append(String.Format("SBase={0}", PCore_SBase).PadRight(20)).AppendLine("// SwingBase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Swing={0:0.00}", PCore_Swing).PadRight(20)).AppendLine("// Swing");
			buffer.Append(String.Format("STrig={0}", PCore_STrig).PadRight(20)).AppendLine("// SwingTrigger");
			buffer.Append(String.Format("PSong={0}", PCore_PSong).PadRight(20)).AppendLine("// PatchSong");
			buffer.Append(String.Format("PFold={0}", PCore_PFold).PadRight(20)).AppendLine("// binary data for PatchFolder");
			buffer.Append(String.Format("PFile={0}", PCore_PFile).PadRight(20)).AppendLine("// binary data for PatchFileName");
			buffer.Append(String.Format("GFile={0}", PCore_GFile).PadRight(20)).AppendLine("// binary data for GUI FileName");
			buffer.Append(String.Format("GScale={0}", PCore_GScale).PadRight(20)).AppendLine("// GUI Scale");
			buffer.Append(String.Format("ChLay={0}", PCore_ChLay).PadRight(20)).AppendLine("// Channel Layout");
			buffer.Append(String.Format("SurrO={0}", PCore_SurrO).PadRight(20)).AppendLine("// Surround Options");

			buffer.AppendLine("\n#cm=LFOG");
			buffer.Append(String.Format("Sync={0}", LFOG_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFOG_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFOG_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFOG_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFOG_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFOG_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFOG_Slew).PadRight(20)).AppendLine("// Slew");
			buffer.Append(String.Format("Nstp={0}", LFOG_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFOG_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFOG_UWv).PadRight(20)).AppendLine("// User Wave Mode");

			buffer.AppendLine("\n#cm=LFOG2");
			buffer.Append(String.Format("Sync={0}", LFOG2_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFOG2_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFOG2_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFOG2_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFOG2_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFOG2_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFOG2_Slew).PadRight(20)).AppendLine("// Slew");			
			buffer.Append(String.Format("Nstp={0}", LFOG2_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFOG2_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFOG2_UWv).PadRight(20)).AppendLine("// User Wave Mode");

			buffer.AppendLine("\n#cm=VCC");
			buffer.Append(String.Format("#LFO1={0}", VCC_LFO1).PadRight(20)).AppendLine("// Active #LFO1");
			buffer.Append(String.Format("#LFO2={0}", VCC_LFO2).PadRight(20)).AppendLine("// Active #LFO2");
			buffer.Append(String.Format("#LFO3={0}", VCC_LFO3).PadRight(20)).AppendLine("// Active #LFO3");
			buffer.Append(String.Format("#LFO4={0}", VCC_LFO4).PadRight(20)).AppendLine("// Active #LFO4");
			buffer.Append(String.Format("Voices={0}", VCC_Voices).PadRight(20)).AppendLine("// Voices");
			buffer.Append(String.Format("Voicing={0}", VCC_Voicing).PadRight(20)).AppendLine("// Voicing");
			buffer.Append(String.Format("Mode={0}", VCC_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Porta={0:0.00}", VCC_Porta).PadRight(20)).AppendLine("// Portamento");
			buffer.Append(String.Format("PB={0}", VCC_PB).PadRight(20)).AppendLine("// PitchBendUp");
			buffer.Append(String.Format("PBD={0}", VCC_PBD).PadRight(20)).AppendLine("// PitchBendDown");
			buffer.Append(String.Format("ArSc={0}", VCC_ArSc).PadRight(20)).AppendLine("// ArpSync");
			buffer.Append(String.Format("ArOrd={0}", VCC_ArOrd).PadRight(20)).AppendLine("// ArpOrder");
			buffer.Append(String.Format("ArLp={0}", VCC_ArLp).PadRight(20)).AppendLine("// ArpLoop");
			buffer.Append(String.Format("ArOct={0}", VCC_ArOct).PadRight(20)).AppendLine("// ArpOctave");
			buffer.Append(String.Format("ArLL={0}", VCC_ArLL).PadRight(20)).AppendLine("// ArpLoopLength");
			buffer.Append(String.Format("ArTr={0}", VCC_ArTr).PadRight(20)).AppendLine("// ArpPortamento");
			buffer.Append(String.Format("Drft={0}", VCC_Drft).PadRight(20)).AppendLine("// Drift");
			buffer.Append(String.Format("MTunS={0}", VCC_MTunS).PadRight(20)).AppendLine("// TuningMode");
			buffer.Append(String.Format("MTunN={0}", VCC_MTunN).PadRight(20)).AppendLine("// binary data for Tuning");
			buffer.Append(String.Format("MTunT={0}", VCC_MTunT).PadRight(20)).AppendLine("// binary data for TuningTable");
			buffer.Append(String.Format("Trsp={0}", VCC_Trsp).PadRight(20)).AppendLine("// Transpose");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FTun={0:0.00}", VCC_FTun).PadRight(20)).AppendLine("// FineTuneCents");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PortRg={0:0.00}", VCC_PortRg).PadRight(20)).AppendLine("// PortaRange");
			buffer.Append(String.Format("PortaM={0}", VCC_PortaM).PadRight(20)).AppendLine("// PortamentoMode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Porta2={0:0.00}", VCC_Porta2).PadRight(20)).AppendLine("// Portamento2");
			buffer.Append(String.Format("Agte1={0}", VCC_Agte1).PadRight(20)).AppendLine("// Arp Gate1");
			buffer.Append(String.Format("Atrp1={0}", VCC_Atrp1).PadRight(20)).AppendLine("// Arp Transpose1");
			buffer.Append(String.Format("Avoc1={0}", VCC_Avoc1).PadRight(20)).AppendLine("// Arp Voices1");
			buffer.Append(String.Format("Amul1={0}", VCC_Amul1).PadRight(20)).AppendLine("// Arp Duration1");
			buffer.Append(String.Format("Amod1={0}", VCC_Amod1).PadRight(20)).AppendLine("// Arp Step Control1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt1={0:0.00}", VCC_AMDpt1).PadRight(20)).AppendLine("// Arp Step ModA1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB1={0:0.00}", VCC_AMDpB1).PadRight(20)).AppendLine("// Arp Step ModB1");
			buffer.Append(String.Format("Agte2={0}", VCC_Agte2).PadRight(20)).AppendLine("// Arp Gate2");
			buffer.Append(String.Format("Atrp2={0}", VCC_Atrp2).PadRight(20)).AppendLine("// Arp Transpose2");
			buffer.Append(String.Format("Avoc2={0}", VCC_Avoc2).PadRight(20)).AppendLine("// Arp Voices2");
			buffer.Append(String.Format("Amul2={0}", VCC_Amul2).PadRight(20)).AppendLine("// Arp Duration2");
			buffer.Append(String.Format("Amod2={0}", VCC_Amod2).PadRight(20)).AppendLine("// Arp Step Control2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt2={0:0.00}", VCC_AMDpt2).PadRight(20)).AppendLine("// Arp Step ModA2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB2={0:0.00}", VCC_AMDpB2).PadRight(20)).AppendLine("// Arp Step ModB2");
			buffer.Append(String.Format("Agte3={0}", VCC_Agte3).PadRight(20)).AppendLine("// Arp Gate3");
			buffer.Append(String.Format("Atrp3={0}", VCC_Atrp3).PadRight(20)).AppendLine("// Arp Transpose3");
			buffer.Append(String.Format("Avoc3={0}", VCC_Avoc3).PadRight(20)).AppendLine("// Arp Voices3");
			buffer.Append(String.Format("Amul3={0}", VCC_Amul3).PadRight(20)).AppendLine("// Arp Duration3");
			buffer.Append(String.Format("Amod3={0}", VCC_Amod3).PadRight(20)).AppendLine("// Arp Step Control3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt3={0:0.00}", VCC_AMDpt3).PadRight(20)).AppendLine("// Arp Step ModA3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB3={0:0.00}", VCC_AMDpB3).PadRight(20)).AppendLine("// Arp Step ModB3");
			buffer.Append(String.Format("Agte4={0}", VCC_Agte4).PadRight(20)).AppendLine("// Arp Gate4");
			buffer.Append(String.Format("Atrp4={0}", VCC_Atrp4).PadRight(20)).AppendLine("// Arp Transpose4");
			buffer.Append(String.Format("Avoc4={0}", VCC_Avoc4).PadRight(20)).AppendLine("// Arp Voices4");
			buffer.Append(String.Format("Amul4={0}", VCC_Amul4).PadRight(20)).AppendLine("// Arp Duration4");
			buffer.Append(String.Format("Amod4={0}", VCC_Amod4).PadRight(20)).AppendLine("// Arp Step Control4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt4={0:0.00}", VCC_AMDpt4).PadRight(20)).AppendLine("// Arp Step ModA4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB4={0:0.00}", VCC_AMDpB4).PadRight(20)).AppendLine("// Arp Step ModB4");
			buffer.Append(String.Format("Agte5={0}", VCC_Agte5).PadRight(20)).AppendLine("// Arp Gate5");
			buffer.Append(String.Format("Atrp5={0}", VCC_Atrp5).PadRight(20)).AppendLine("// Arp Transpose5");
			buffer.Append(String.Format("Avoc5={0}", VCC_Avoc5).PadRight(20)).AppendLine("// Arp Voices5");
			buffer.Append(String.Format("Amul5={0}", VCC_Amul5).PadRight(20)).AppendLine("// Arp Duration5");
			buffer.Append(String.Format("Amod5={0}", VCC_Amod5).PadRight(20)).AppendLine("// Arp Step Control5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt5={0:0.00}", VCC_AMDpt5).PadRight(20)).AppendLine("// Arp Step ModA5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB5={0:0.00}", VCC_AMDpB5).PadRight(20)).AppendLine("// Arp Step ModB5");
			buffer.Append(String.Format("Agte6={0}", VCC_Agte6).PadRight(20)).AppendLine("// Arp Gate6");
			buffer.Append(String.Format("Atrp6={0}", VCC_Atrp6).PadRight(20)).AppendLine("// Arp Transpose6");
			buffer.Append(String.Format("Avoc6={0}", VCC_Avoc6).PadRight(20)).AppendLine("// Arp Voices6");
			buffer.Append(String.Format("Amul6={0}", VCC_Amul6).PadRight(20)).AppendLine("// Arp Duration6");
			buffer.Append(String.Format("Amod6={0}", VCC_Amod6).PadRight(20)).AppendLine("// Arp Step Control6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt6={0:0.00}", VCC_AMDpt6).PadRight(20)).AppendLine("// Arp Step ModA6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB6={0:0.00}", VCC_AMDpB6).PadRight(20)).AppendLine("// Arp Step ModB6");
			buffer.Append(String.Format("Agte7={0}", VCC_Agte7).PadRight(20)).AppendLine("// Arp Gate7");
			buffer.Append(String.Format("Atrp7={0}", VCC_Atrp7).PadRight(20)).AppendLine("// Arp Transpose7");
			buffer.Append(String.Format("Avoc7={0}", VCC_Avoc7).PadRight(20)).AppendLine("// Arp Voices7");
			buffer.Append(String.Format("Amul7={0}", VCC_Amul7).PadRight(20)).AppendLine("// Arp Duration7");
			buffer.Append(String.Format("Amod7={0}", VCC_Amod7).PadRight(20)).AppendLine("// Arp Step Control7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt7={0:0.00}", VCC_AMDpt7).PadRight(20)).AppendLine("// Arp Step ModA7");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB7={0:0.00}", VCC_AMDpB7).PadRight(20)).AppendLine("// Arp Step ModB7");
			buffer.Append(String.Format("Agte8={0}", VCC_Agte8).PadRight(20)).AppendLine("// Arp Gate8");
			buffer.Append(String.Format("Atrp8={0}", VCC_Atrp8).PadRight(20)).AppendLine("// Arp Transpose8");
			buffer.Append(String.Format("Avoc8={0}", VCC_Avoc8).PadRight(20)).AppendLine("// Arp Voices8");
			buffer.Append(String.Format("Amul8={0}", VCC_Amul8).PadRight(20)).AppendLine("// Arp Duration8");
			buffer.Append(String.Format("Amod8={0}", VCC_Amod8).PadRight(20)).AppendLine("// Arp Step Control8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt8={0:0.00}", VCC_AMDpt8).PadRight(20)).AppendLine("// Arp Step ModA8");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB8={0:0.00}", VCC_AMDpB8).PadRight(20)).AppendLine("// Arp Step ModB8");
			buffer.Append(String.Format("Agte9={0}", VCC_Agte9).PadRight(20)).AppendLine("// Arp Gate9");
			buffer.Append(String.Format("Atrp9={0}", VCC_Atrp9).PadRight(20)).AppendLine("// Arp Transpose9");
			buffer.Append(String.Format("Avoc9={0}", VCC_Avoc9).PadRight(20)).AppendLine("// Arp Voices9");
			buffer.Append(String.Format("Amul9={0}", VCC_Amul9).PadRight(20)).AppendLine("// Arp Duration9");
			buffer.Append(String.Format("Amod9={0}", VCC_Amod9).PadRight(20)).AppendLine("// Arp Step Control9");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt9={0:0.00}", VCC_AMDpt9).PadRight(20)).AppendLine("// Arp Step ModA9");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB9={0:0.00}", VCC_AMDpB9).PadRight(20)).AppendLine("// Arp Step ModB9");
			buffer.Append(String.Format("Agte10={0}", VCC_Agte10).PadRight(20)).AppendLine("// Arp Gate10");
			buffer.Append(String.Format("Atrp10={0}", VCC_Atrp10).PadRight(20)).AppendLine("// Arp Transpose10");
			buffer.Append(String.Format("Avoc10={0}", VCC_Avoc10).PadRight(20)).AppendLine("// Arp Voices10");
			buffer.Append(String.Format("Amul10={0}", VCC_Amul10).PadRight(20)).AppendLine("// Arp Duration10");
			buffer.Append(String.Format("Amod10={0}", VCC_Amod10).PadRight(20)).AppendLine("// Arp Step Control10");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt10={0:0.00}", VCC_AMDpt10).PadRight(20)).AppendLine("// Arp Step ModA10");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB10={0:0.00}", VCC_AMDpB10).PadRight(20)).AppendLine("// Arp Step ModB10");
			buffer.Append(String.Format("Agte11={0}", VCC_Agte11).PadRight(20)).AppendLine("// Arp Gate11");
			buffer.Append(String.Format("Atrp11={0}", VCC_Atrp11).PadRight(20)).AppendLine("// Arp Transpose11");
			buffer.Append(String.Format("Avoc11={0}", VCC_Avoc11).PadRight(20)).AppendLine("// Arp Voices11");
			buffer.Append(String.Format("Amul11={0}", VCC_Amul11).PadRight(20)).AppendLine("// Arp Duration11");
			buffer.Append(String.Format("Amod11={0}", VCC_Amod11).PadRight(20)).AppendLine("// Arp Step Control11");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt11={0:0.00}", VCC_AMDpt11).PadRight(20)).AppendLine("// Arp Step ModA11");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB11={0:0.00}", VCC_AMDpB11).PadRight(20)).AppendLine("// Arp Step ModB11");
			buffer.Append(String.Format("Agte12={0}", VCC_Agte12).PadRight(20)).AppendLine("// Arp Gate12");
			buffer.Append(String.Format("Atrp12={0}", VCC_Atrp12).PadRight(20)).AppendLine("// Arp Transpose12");
			buffer.Append(String.Format("Avoc12={0}", VCC_Avoc12).PadRight(20)).AppendLine("// Arp Voices12");
			buffer.Append(String.Format("Amul12={0}", VCC_Amul12).PadRight(20)).AppendLine("// Arp Duration12");
			buffer.Append(String.Format("Amod12={0}", VCC_Amod12).PadRight(20)).AppendLine("// Arp Step Control12");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt12={0:0.00}", VCC_AMDpt12).PadRight(20)).AppendLine("// Arp Step ModA12");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB12={0:0.00}", VCC_AMDpB12).PadRight(20)).AppendLine("// Arp Step ModB12");
			buffer.Append(String.Format("Agte13={0}", VCC_Agte13).PadRight(20)).AppendLine("// Arp Gate13");
			buffer.Append(String.Format("Atrp13={0}", VCC_Atrp13).PadRight(20)).AppendLine("// Arp Transpose13");
			buffer.Append(String.Format("Avoc13={0}", VCC_Avoc13).PadRight(20)).AppendLine("// Arp Voices13");
			buffer.Append(String.Format("Amul13={0}", VCC_Amul13).PadRight(20)).AppendLine("// Arp Duration13");
			buffer.Append(String.Format("Amod13={0}", VCC_Amod13).PadRight(20)).AppendLine("// Arp Step Control13");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt13={0:0.00}", VCC_AMDpt13).PadRight(20)).AppendLine("// Arp Step ModA13");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB13={0:0.00}", VCC_AMDpB13).PadRight(20)).AppendLine("// Arp Step ModB13");
			buffer.Append(String.Format("Agte14={0}", VCC_Agte14).PadRight(20)).AppendLine("// Arp Gate14");
			buffer.Append(String.Format("Atrp14={0}", VCC_Atrp14).PadRight(20)).AppendLine("// Arp Transpose14");
			buffer.Append(String.Format("Avoc14={0}", VCC_Avoc14).PadRight(20)).AppendLine("// Arp Voices14");
			buffer.Append(String.Format("Amul14={0}", VCC_Amul14).PadRight(20)).AppendLine("// Arp Duration14");
			buffer.Append(String.Format("Amod14={0}", VCC_Amod14).PadRight(20)).AppendLine("// Arp Step Control14");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt14={0:0.00}", VCC_AMDpt14).PadRight(20)).AppendLine("// Arp Step ModA14");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB14={0:0.00}", VCC_AMDpB14).PadRight(20)).AppendLine("// Arp Step ModB14");
			buffer.Append(String.Format("Agte15={0}", VCC_Agte15).PadRight(20)).AppendLine("// Arp Gate15");
			buffer.Append(String.Format("Atrp15={0}", VCC_Atrp15).PadRight(20)).AppendLine("// Arp Transpose15");
			buffer.Append(String.Format("Avoc15={0}", VCC_Avoc15).PadRight(20)).AppendLine("// Arp Voices15");
			buffer.Append(String.Format("Amul15={0}", VCC_Amul15).PadRight(20)).AppendLine("// Arp Duration15");
			buffer.Append(String.Format("Amod15={0}", VCC_Amod15).PadRight(20)).AppendLine("// Arp Step Control15");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt15={0:0.00}", VCC_AMDpt15).PadRight(20)).AppendLine("// Arp Step ModA15");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB15={0:0.00}", VCC_AMDpB15).PadRight(20)).AppendLine("// Arp Step ModB15");
			buffer.Append(String.Format("Agte16={0}", VCC_Agte16).PadRight(20)).AppendLine("// Arp Gate16");
			buffer.Append(String.Format("Atrp16={0}", VCC_Atrp16).PadRight(20)).AppendLine("// Arp Transpose16");
			buffer.Append(String.Format("Avoc16={0}", VCC_Avoc16).PadRight(20)).AppendLine("// Arp Voices16");
			buffer.Append(String.Format("Amul16={0}", VCC_Amul16).PadRight(20)).AppendLine("// Arp Duration16");
			buffer.Append(String.Format("Amod16={0}", VCC_Amod16).PadRight(20)).AppendLine("// Arp Step Control16");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpt16={0:0.00}", VCC_AMDpt16).PadRight(20)).AppendLine("// Arp Step ModA16");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "AMDpB16={0:0.00}", VCC_AMDpB16).PadRight(20)).AppendLine("// Arp Step ModB16");

			buffer.AppendLine("\n#cm=ENV1");
			buffer.Append(String.Format("Mode={0}", ENV1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("iMode={0}", ENV1_IMode).PadRight(20)).AppendLine("// InitMode");
			buffer.Append(String.Format("sMode={0}", ENV1_SMode).PadRight(20)).AppendLine("// SustainMode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "init={0:0.00}", ENV1_Init).PadRight(20)).AppendLine("// Init");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", ENV1_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dec={0:0.00}", ENV1_Dec).PadRight(20)).AppendLine("// Decay");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus={0:0.00}", ENV1_Sus).PadRight(20)).AppendLine("// Sustain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SusT={0:0.00}", ENV1_SusT).PadRight(20)).AppendLine("// Fall/Rise");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus2={0:0.00}", ENV1_Sus2).PadRight(20)).AppendLine("// Sustain2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", ENV1_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", ENV1_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2I={0:0.00}", ENV1_V2I).PadRight(20)).AppendLine("// Vel2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2A={0:0.00}", ENV1_V2A).PadRight(20)).AppendLine("// Vel2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2D={0:0.00}", ENV1_V2D).PadRight(20)).AppendLine("// Vel2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S={0:0.00}", ENV1_V2S).PadRight(20)).AppendLine("// Vel2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2FR={0:0.00}", ENV1_V2FR).PadRight(20)).AppendLine("// Vel2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S2={0:0.00}", ENV1_V2S2).PadRight(20)).AppendLine("// Vel2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2R={0:0.00}", ENV1_V2R).PadRight(20)).AppendLine("// Vel2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2I={0:0.00}", ENV1_K2I).PadRight(20)).AppendLine("// Key2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2A={0:0.00}", ENV1_K2A).PadRight(20)).AppendLine("// Key2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2D={0:0.00}", ENV1_K2D).PadRight(20)).AppendLine("// Key2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S={0:0.00}", ENV1_K2S).PadRight(20)).AppendLine("// Key2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2FR={0:0.00}", ENV1_K2FR).PadRight(20)).AppendLine("// Key2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S2={0:0.00}", ENV1_K2S2).PadRight(20)).AppendLine("// Key2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2R={0:0.00}", ENV1_K2R).PadRight(20)).AppendLine("// Key2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Slope={0:0.00}", ENV1_Slope).PadRight(20)).AppendLine("// Slope");
			buffer.Append(String.Format("TBase={0}", ENV1_TBase).PadRight(20)).AppendLine("// Timebase");

			buffer.AppendLine("\n#cm=ENV2");
			buffer.Append(String.Format("Mode={0}", ENV2_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("iMode={0}", ENV2_IMode).PadRight(20)).AppendLine("// InitMode");
			buffer.Append(String.Format("sMode={0}", ENV2_SMode).PadRight(20)).AppendLine("// SustainMode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "init={0:0.00}", ENV2_Init).PadRight(20)).AppendLine("// Init");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", ENV2_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dec={0:0.00}", ENV2_Dec).PadRight(20)).AppendLine("// Decay");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus={0:0.00}", ENV2_Sus).PadRight(20)).AppendLine("// Sustain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SusT={0:0.00}", ENV2_SusT).PadRight(20)).AppendLine("// Fall/Rise");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus2={0:0.00}", ENV2_Sus2).PadRight(20)).AppendLine("// Sustain2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", ENV2_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", ENV2_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2I={0:0.00}", ENV2_V2I).PadRight(20)).AppendLine("// Vel2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2A={0:0.00}", ENV2_V2A).PadRight(20)).AppendLine("// Vel2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2D={0:0.00}", ENV2_V2D).PadRight(20)).AppendLine("// Vel2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S={0:0.00}", ENV2_V2S).PadRight(20)).AppendLine("// Vel2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2FR={0:0.00}", ENV2_V2FR).PadRight(20)).AppendLine("// Vel2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S2={0:0.00}", ENV2_V2S2).PadRight(20)).AppendLine("// Vel2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2R={0:0.00}", ENV2_V2R).PadRight(20)).AppendLine("// Vel2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2I={0:0.00}", ENV2_K2I).PadRight(20)).AppendLine("// Key2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2A={0:0.00}", ENV2_K2A).PadRight(20)).AppendLine("// Key2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2D={0:0.00}", ENV2_K2D).PadRight(20)).AppendLine("// Key2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S={0:0.00}", ENV2_K2S).PadRight(20)).AppendLine("// Key2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2FR={0:0.00}", ENV2_K2FR).PadRight(20)).AppendLine("// Key2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S2={0:0.00}", ENV2_K2S2).PadRight(20)).AppendLine("// Key2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2R={0:0.00}", ENV2_K2R).PadRight(20)).AppendLine("// Key2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Slope={0:0.00}", ENV2_Slope).PadRight(20)).AppendLine("// Slope");
			buffer.Append(String.Format("TBase={0}", ENV2_TBase).PadRight(20)).AppendLine("// Timebase");

			buffer.AppendLine("\n#cm=ENV3");
			buffer.Append(String.Format("Mode={0}", ENV3_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("iMode={0}", ENV3_IMode).PadRight(20)).AppendLine("// InitMode");
			buffer.Append(String.Format("sMode={0}", ENV3_SMode).PadRight(20)).AppendLine("// SustainMode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "init={0:0.00}", ENV3_Init).PadRight(20)).AppendLine("// Init");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", ENV3_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dec={0:0.00}", ENV3_Dec).PadRight(20)).AppendLine("// Decay");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus={0:0.00}", ENV3_Sus).PadRight(20)).AppendLine("// Sustain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SusT={0:0.00}", ENV3_SusT).PadRight(20)).AppendLine("// Fall/Rise");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus2={0:0.00}", ENV3_Sus2).PadRight(20)).AppendLine("// Sustain2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", ENV3_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", ENV3_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2I={0:0.00}", ENV3_V2I).PadRight(20)).AppendLine("// Vel2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2A={0:0.00}", ENV3_V2A).PadRight(20)).AppendLine("// Vel2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2D={0:0.00}", ENV3_V2D).PadRight(20)).AppendLine("// Vel2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S={0:0.00}", ENV3_V2S).PadRight(20)).AppendLine("// Vel2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2FR={0:0.00}", ENV3_V2FR).PadRight(20)).AppendLine("// Vel2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S2={0:0.00}", ENV3_V2S2).PadRight(20)).AppendLine("// Vel2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2R={0:0.00}", ENV3_V2R).PadRight(20)).AppendLine("// Vel2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2I={0:0.00}", ENV3_K2I).PadRight(20)).AppendLine("// Key2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2A={0:0.00}", ENV3_K2A).PadRight(20)).AppendLine("// Key2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2D={0:0.00}", ENV3_K2D).PadRight(20)).AppendLine("// Key2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S={0:0.00}", ENV3_K2S).PadRight(20)).AppendLine("// Key2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2FR={0:0.00}", ENV3_K2FR).PadRight(20)).AppendLine("// Key2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S2={0:0.00}", ENV3_K2S2).PadRight(20)).AppendLine("// Key2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2R={0:0.00}", ENV3_K2R).PadRight(20)).AppendLine("// Key2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Slope={0:0.00}", ENV3_Slope).PadRight(20)).AppendLine("// Slope");
			buffer.Append(String.Format("TBase={0}", ENV3_TBase).PadRight(20)).AppendLine("// Timebase");

			buffer.AppendLine("\n#cm=ENV4");
			buffer.Append(String.Format("Mode={0}", ENV4_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("iMode={0}", ENV4_IMode).PadRight(20)).AppendLine("// InitMode");
			buffer.Append(String.Format("sMode={0}", ENV4_SMode).PadRight(20)).AppendLine("// SustainMode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "init={0:0.00}", ENV4_Init).PadRight(20)).AppendLine("// Init");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", ENV4_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dec={0:0.00}", ENV4_Dec).PadRight(20)).AppendLine("// Decay");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus={0:0.00}", ENV4_Sus).PadRight(20)).AppendLine("// Sustain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SusT={0:0.00}", ENV4_SusT).PadRight(20)).AppendLine("// Fall/Rise");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sus2={0:0.00}", ENV4_Sus2).PadRight(20)).AppendLine("// Sustain2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", ENV4_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", ENV4_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2I={0:0.00}", ENV4_V2I).PadRight(20)).AppendLine("// Vel2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2A={0:0.00}", ENV4_V2A).PadRight(20)).AppendLine("// Vel2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2D={0:0.00}", ENV4_V2D).PadRight(20)).AppendLine("// Vel2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S={0:0.00}", ENV4_V2S).PadRight(20)).AppendLine("// Vel2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2FR={0:0.00}", ENV4_V2FR).PadRight(20)).AppendLine("// Vel2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2S2={0:0.00}", ENV4_V2S2).PadRight(20)).AppendLine("// Vel2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "V2R={0:0.00}", ENV4_V2R).PadRight(20)).AppendLine("// Vel2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2I={0:0.00}", ENV4_K2I).PadRight(20)).AppendLine("// Key2I");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2A={0:0.00}", ENV4_K2A).PadRight(20)).AppendLine("// Key2A");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2D={0:0.00}", ENV4_K2D).PadRight(20)).AppendLine("// Key2D");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S={0:0.00}", ENV4_K2S).PadRight(20)).AppendLine("// Key2S");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2FR={0:0.00}", ENV4_K2FR).PadRight(20)).AppendLine("// Key2FR");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2S2={0:0.00}", ENV4_K2S2).PadRight(20)).AppendLine("// Key2S2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "K2R={0:0.00}", ENV4_K2R).PadRight(20)).AppendLine("// Key2R");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Slope={0:0.00}", ENV4_Slope).PadRight(20)).AppendLine("// Slope");
			buffer.Append(String.Format("TBase={0}", ENV4_TBase).PadRight(20)).AppendLine("// Timebase");

			buffer.AppendLine("\n#cm=MSEG1");
			buffer.Append(String.Format("TmUn={0}", MSEG1_TmUn).PadRight(20)).AppendLine("// TimeUnit");
			buffer.Append(String.Format("Env={0}", MSEG1_Env).PadRight(20)).AppendLine("// binary data for Envelope");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", MSEG1_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", MSEG1_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Lpt={0:0.00}", MSEG1_Lpt).PadRight(20)).AppendLine("// Loop");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", MSEG1_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format("Trig={0}", MSEG1_Trig).PadRight(20)).AppendLine("// Trigger");

			buffer.AppendLine("\n#cm=MSEG2");
			buffer.Append(String.Format("TmUn={0}", MSEG2_TmUn).PadRight(20)).AppendLine("// TimeUnit");
			buffer.Append(String.Format("Env={0}", MSEG2_Env).PadRight(20)).AppendLine("// binary data for Envelope");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", MSEG2_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", MSEG2_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Lpt={0:0.00}", MSEG2_Lpt).PadRight(20)).AppendLine("// Loop");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", MSEG2_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format("Trig={0}", MSEG2_Trig).PadRight(20)).AppendLine("// Trigger");

			buffer.AppendLine("\n#cm=MSEG3");
			buffer.Append(String.Format("TmUn={0}", MSEG3_TmUn).PadRight(20)).AppendLine("// TimeUnit");
			buffer.Append(String.Format("Env={0}", MSEG3_Env).PadRight(20)).AppendLine("// binary data for Envelope");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", MSEG3_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", MSEG3_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Lpt={0:0.00}", MSEG3_Lpt).PadRight(20)).AppendLine("// Loop");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", MSEG3_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format("Trig={0}", MSEG3_Trig).PadRight(20)).AppendLine("// Trigger");

			buffer.AppendLine("\n#cm=MSEG4");
			buffer.Append(String.Format("TmUn={0}", MSEG4_TmUn).PadRight(20)).AppendLine("// TimeUnit");
			buffer.Append(String.Format("Env={0}", MSEG4_Env).PadRight(20)).AppendLine("// binary data for Envelope");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vel={0:0.00}", MSEG4_Vel).PadRight(20)).AppendLine("// Velocity");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Atk={0:0.00}", MSEG4_Atk).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Lpt={0:0.00}", MSEG4_Lpt).PadRight(20)).AppendLine("// Loop");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", MSEG4_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format("Trig={0}", MSEG4_Trig).PadRight(20)).AppendLine("// Trigger");

			buffer.AppendLine("\n#cm=LFO1");
			buffer.Append(String.Format("Sync={0}", LFO1_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFO1_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFO1_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFO1_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFO1_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFO1_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFO1_Slew).PadRight(20)).AppendLine("// Slew");			
			buffer.Append(String.Format("Nstp={0}", LFO1_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFO1_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFO1_UWv).PadRight(20)).AppendLine("// User Wave Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dly={0:0.00}", LFO1_Dly).PadRight(20)).AppendLine("// Delay");
			buffer.Append(String.Format("DMS1={0}", LFO1_DMS1).PadRight(20)).AppendLine("// DepthMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMD1={0:0.00}", LFO1_DMD1).PadRight(20)).AppendLine("// DepthMod Dpt1");
			buffer.Append(String.Format("FMS1={0}", LFO1_FMS1).PadRight(20)).AppendLine("// FreqMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMD1={0:0.00}", LFO1_FMD1).PadRight(20)).AppendLine("// FreqMod Dpt");

			buffer.AppendLine("\n#cm=LFO2");
			buffer.Append(String.Format("Sync={0}", LFO2_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFO2_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFO2_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFO2_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFO2_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFO2_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFO2_Slew).PadRight(20)).AppendLine("// Slew");						
			buffer.Append(String.Format("Nstp={0}", LFO2_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFO2_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFO2_UWv).PadRight(20)).AppendLine("// User Wave Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dly={0:0.00}", LFO2_Dly).PadRight(20)).AppendLine("// Delay");
			buffer.Append(String.Format("DMS1={0}", LFO2_DMS1).PadRight(20)).AppendLine("// DepthMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMD1={0:0.00}", LFO2_DMD1).PadRight(20)).AppendLine("// DepthMod Dpt1");
			buffer.Append(String.Format("FMS1={0}", LFO2_FMS1).PadRight(20)).AppendLine("// FreqMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMD1={0:0.00}", LFO2_FMD1).PadRight(20)).AppendLine("// FreqMod Dpt");

			buffer.AppendLine("\n#cm=LFO3");
			buffer.Append(String.Format("Sync={0}", LFO3_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFO3_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFO3_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFO3_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFO3_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFO3_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFO3_Slew).PadRight(20)).AppendLine("// Slew");						
			buffer.Append(String.Format("Nstp={0}", LFO3_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFO3_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFO3_UWv).PadRight(20)).AppendLine("// User Wave Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dly={0:0.00}", LFO3_Dly).PadRight(20)).AppendLine("// Delay");
			buffer.Append(String.Format("DMS1={0}", LFO3_DMS1).PadRight(20)).AppendLine("// DepthMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMD1={0:0.00}", LFO3_DMD1).PadRight(20)).AppendLine("// DepthMod Dpt1");
			buffer.Append(String.Format("FMS1={0}", LFO3_FMS1).PadRight(20)).AppendLine("// FreqMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMD1={0:0.00}", LFO3_FMD1).PadRight(20)).AppendLine("// FreqMod Dpt");

			buffer.AppendLine("\n#cm=LFO4");
			buffer.Append(String.Format("Sync={0}", LFO4_Sync).PadRight(20)).AppendLine("// Sync");
			buffer.Append(String.Format("Trig={0}", LFO4_Trig).PadRight(20)).AppendLine("// Restart");
			buffer.Append(String.Format("Wave={0}", LFO4_Wave).PadRight(20)).AppendLine("// Waveform");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", LFO4_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rate={0:0.00}", LFO4_Rate).PadRight(20)).AppendLine("// Rate");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Amp={0:0.00}", LFO4_Amp).PadRight(20)).AppendLine("// Amplitude");
			buffer.Append(String.Format("Slew={0}", LFO4_Slew).PadRight(20)).AppendLine("// Slew");						
			buffer.Append(String.Format("Nstp={0}", LFO4_Nstp).PadRight(20)).AppendLine("// Num Steps");
			buffer.Append(String.Format("Stps={0}", LFO4_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("UWv={0}", LFO4_UWv).PadRight(20)).AppendLine("// User Wave Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dly={0:0.00}", LFO4_Dly).PadRight(20)).AppendLine("// Delay");
			buffer.Append(String.Format("DMS1={0}", LFO4_DMS1).PadRight(20)).AppendLine("// DepthMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMD1={0:0.00}", LFO4_DMD1).PadRight(20)).AppendLine("// DepthMod Dpt1");
			buffer.Append(String.Format("FMS1={0}", LFO4_FMS1).PadRight(20)).AppendLine("// FreqMod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMD1={0:0.00}", LFO4_FMD1).PadRight(20)).AppendLine("// FreqMod Dpt");

			buffer.AppendLine("\n#cm=MMap1");
			buffer.Append(String.Format("Mode={0}", MMap1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("MSrc={0}", MMap1_MSrc).PadRight(20)).AppendLine("// MSrc");
			buffer.Append(String.Format("Stps={0}", MMap1_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("Num={0}", MMap1_Num).PadRight(20)).AppendLine("// Number");

			buffer.AppendLine("\n#cm=MMap2");
			buffer.Append(String.Format("Mode={0}", MMap2_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("MSrc={0}", MMap2_MSrc).PadRight(20)).AppendLine("// MSrc");
			buffer.Append(String.Format("Stps={0}", MMap2_Stps).PadRight(20)).AppendLine("// binary data for Steps");
			buffer.Append(String.Format("Num={0}", MMap2_Num).PadRight(20)).AppendLine("// Number");

			buffer.AppendLine("\n#cm=MMix1");
			buffer.Append(String.Format("Type={0}", MMix1_Type).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("Mod1={0}", MMix1_Mod1).PadRight(20)).AppendLine("// Mod1");
			buffer.Append(String.Format("Mod2={0}", MMix1_Mod2).PadRight(20)).AppendLine("// Mod2");
			buffer.Append(String.Format("Mod3={0}", MMix1_Mod3).PadRight(20)).AppendLine("// Mod3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cst={0:0.00}", MMix1_Cst).PadRight(20)).AppendLine("// Constant");

			buffer.AppendLine("\n#cm=MMix2");
			buffer.Append(String.Format("Type={0}", MMix2_Type).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("Mod1={0}", MMix2_Mod1).PadRight(20)).AppendLine("// Mod1");
			buffer.Append(String.Format("Mod2={0}", MMix2_Mod2).PadRight(20)).AppendLine("// Mod2");
			buffer.Append(String.Format("Mod3={0}", MMix2_Mod3).PadRight(20)).AppendLine("// Mod3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cst={0:0.00}", MMix2_Cst).PadRight(20)).AppendLine("// Constant");

			buffer.AppendLine("\n#cm=MMix3");
			buffer.Append(String.Format("Type={0}", MMix3_Type).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("Mod1={0}", MMix3_Mod1).PadRight(20)).AppendLine("// Mod1");
			buffer.Append(String.Format("Mod2={0}", MMix3_Mod2).PadRight(20)).AppendLine("// Mod2");
			buffer.Append(String.Format("Mod3={0}", MMix3_Mod3).PadRight(20)).AppendLine("// Mod3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cst={0:0.00}", MMix3_Cst).PadRight(20)).AppendLine("// Constant");

			buffer.AppendLine("\n#cm=MMix4");
			buffer.Append(String.Format("Type={0}", MMix4_Type).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format("Mod1={0}", MMix4_Mod1).PadRight(20)).AppendLine("// Mod1");
			buffer.Append(String.Format("Mod2={0}", MMix4_Mod2).PadRight(20)).AppendLine("// Mod2");
			buffer.Append(String.Format("Mod3={0}", MMix4_Mod3).PadRight(20)).AppendLine("// Mod3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cst={0:0.00}", MMix4_Cst).PadRight(20)).AppendLine("// Constant");

			buffer.AppendLine("\n#cm=Grid");
			buffer.Append(String.Format("Grid={0}", Grid_Grid).PadRight(20)).AppendLine("// binary data for Grid Structure");
			buffer.Append(String.Format("GByp={0}", Grid_GByp).PadRight(20)).AppendLine("// Bypass");

			buffer.AppendLine("\n#cm=OSC1");
			buffer.Append(String.Format("Wave={0}", OSC1_Wave).PadRight(20)).AppendLine("// WaveForm");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", OSC1_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", OSC1_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", OSC1_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", OSC1_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", OSC1_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format("PhsMSrc={0}", OSC1_PhsMSrc).PadRight(20)).AppendLine("// PhaseModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhsMDpt={0:0.00}", OSC1_PhsMDpt).PadRight(20)).AppendLine("// PhaseModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WNum={0:0.00}", OSC1_WNum).PadRight(20)).AppendLine("// WaveWarp");
			buffer.Append(String.Format("WPSrc={0}", OSC1_WPSrc).PadRight(20)).AppendLine("// WarpModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WPDpt={0:0.00}", OSC1_WPDpt).PadRight(20)).AppendLine("// WarpModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", OSC1_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format("Curve={0}", OSC1_Curve).PadRight(20)).AppendLine("// binary data for Curve");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Prec={0:0.00}", OSC1_Prec).PadRight(20)).AppendLine("// Resolution");
			buffer.Append(String.Format("FX1Tp={0}", OSC1_FX1Tp).PadRight(20)).AppendLine("// SpectraFX1 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX1={0:0.00}", OSC1_SFX1).PadRight(20)).AppendLine("// SpectraFX1 Val");
			buffer.Append(String.Format("FX1Sc={0}", OSC1_FX1Sc).PadRight(20)).AppendLine("// SFX1ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX1Dt={0:0.00}", OSC1_FX1Dt).PadRight(20)).AppendLine("// SFX1ModDepth");
			buffer.Append(String.Format("FX2Tp={0}", OSC1_FX2Tp).PadRight(20)).AppendLine("// SpectraFX2 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX2={0:0.00}", OSC1_SFX2).PadRight(20)).AppendLine("// SpectraFX2 Val");
			buffer.Append(String.Format("FX2Sc={0}", OSC1_FX2Sc).PadRight(20)).AppendLine("// SFX2ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX2Dt={0:0.00}", OSC1_FX2Dt).PadRight(20)).AppendLine("// SFX2ModDepth");
			buffer.Append(String.Format("Poly={0}", OSC1_Poly).PadRight(20)).AppendLine("// PolyWave");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", OSC1_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", OSC1_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", OSC1_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", OSC1_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", OSC1_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", OSC1_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", OSC1_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", OSC1_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sync={0:0.00}", OSC1_Sync).PadRight(20)).AppendLine("// SyncTune");
			buffer.Append(String.Format("SncSc={0}", OSC1_SncSc).PadRight(20)).AppendLine("// SyncModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SncDt={0:0.00}", OSC1_SncDt).PadRight(20)).AppendLine("// SyncModDepth");
			buffer.Append(String.Format("SncOn={0}", OSC1_SncOn).PadRight(20)).AppendLine("// Sync Active");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", OSC1_PolW).PadRight(20)).AppendLine("// Poly Width");
			buffer.Append(String.Format("PwmOn={0}", OSC1_PwmOn).PadRight(20)).AppendLine("// PWM Mode");
			buffer.Append(String.Format("WaTb={0}", OSC1_WaTb).PadRight(20)).AppendLine("// binary data for WaveTable");
			buffer.Append(String.Format("RePhs={0}", OSC1_RePhs).PadRight(20)).AppendLine("// Reset Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Norm={0:0.00}", OSC1_Norm).PadRight(20)).AppendLine("// Normalize");
			buffer.Append(String.Format("Rend={0}", OSC1_Rend).PadRight(20)).AppendLine("// Renderer");

			buffer.AppendLine("\n#cm=OSC2");
			buffer.Append(String.Format("Wave={0}", OSC2_Wave).PadRight(20)).AppendLine("// WaveForm");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", OSC2_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", OSC2_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", OSC2_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", OSC2_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", OSC2_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format("PhsMSrc={0}", OSC2_PhsMSrc).PadRight(20)).AppendLine("// PhaseModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhsMDpt={0:0.00}", OSC2_PhsMDpt).PadRight(20)).AppendLine("// PhaseModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WNum={0:0.00}", OSC2_WNum).PadRight(20)).AppendLine("// WaveWarp");
			buffer.Append(String.Format("WPSrc={0}", OSC2_WPSrc).PadRight(20)).AppendLine("// WarpModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WPDpt={0:0.00}", OSC2_WPDpt).PadRight(20)).AppendLine("// WarpModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", OSC2_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format("Curve={0}", OSC2_Curve).PadRight(20)).AppendLine("// binary data for Curve");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Prec={0:0.00}", OSC2_Prec).PadRight(20)).AppendLine("// Resolution");
			buffer.Append(String.Format("FX1Tp={0}", OSC2_FX1Tp).PadRight(20)).AppendLine("// SpectraFX1 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX1={0:0.00}", OSC2_SFX1).PadRight(20)).AppendLine("// SpectraFX1 Val");
			buffer.Append(String.Format("FX1Sc={0}", OSC2_FX1Sc).PadRight(20)).AppendLine("// SFX1ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX1Dt={0:0.00}", OSC2_FX1Dt).PadRight(20)).AppendLine("// SFX1ModDepth");
			buffer.Append(String.Format("FX2Tp={0}", OSC2_FX2Tp).PadRight(20)).AppendLine("// SpectraFX2 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX2={0:0.00}", OSC2_SFX2).PadRight(20)).AppendLine("// SpectraFX2 Val");
			buffer.Append(String.Format("FX2Sc={0}", OSC2_FX2Sc).PadRight(20)).AppendLine("// SFX2ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX2Dt={0:0.00}", OSC2_FX2Dt).PadRight(20)).AppendLine("// SFX2ModDepth");
			buffer.Append(String.Format("Poly={0}", OSC2_Poly).PadRight(20)).AppendLine("// PolyWave");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", OSC2_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", OSC2_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", OSC2_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", OSC2_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", OSC2_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", OSC2_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", OSC2_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", OSC2_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sync={0:0.00}", OSC2_Sync).PadRight(20)).AppendLine("// SyncTune");
			buffer.Append(String.Format("SncSc={0}", OSC2_SncSc).PadRight(20)).AppendLine("// SyncModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SncDt={0:0.00}", OSC2_SncDt).PadRight(20)).AppendLine("// SyncModDepth");
			buffer.Append(String.Format("SncOn={0}", OSC2_SncOn).PadRight(20)).AppendLine("// Sync Active");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", OSC2_PolW).PadRight(20)).AppendLine("// Poly Width");
			buffer.Append(String.Format("PwmOn={0}", OSC2_PwmOn).PadRight(20)).AppendLine("// PWM Mode");
			buffer.Append(String.Format("WaTb={0}", OSC2_WaTb).PadRight(20)).AppendLine("// binary data for WaveTable");
			buffer.Append(String.Format("RePhs={0}", OSC2_RePhs).PadRight(20)).AppendLine("// Reset Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Norm={0:0.00}", OSC2_Norm).PadRight(20)).AppendLine("// Normalize");
			buffer.Append(String.Format("Rend={0}", OSC2_Rend).PadRight(20)).AppendLine("// Renderer");

			buffer.AppendLine("\n#cm=OSC3");
			buffer.Append(String.Format("Wave={0}", OSC3_Wave).PadRight(20)).AppendLine("// WaveForm");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", OSC3_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", OSC3_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", OSC3_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", OSC3_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", OSC3_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format("PhsMSrc={0}", OSC3_PhsMSrc).PadRight(20)).AppendLine("// PhaseModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhsMDpt={0:0.00}", OSC3_PhsMDpt).PadRight(20)).AppendLine("// PhaseModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WNum={0:0.00}", OSC3_WNum).PadRight(20)).AppendLine("// WaveWarp");
			buffer.Append(String.Format("WPSrc={0}", OSC3_WPSrc).PadRight(20)).AppendLine("// WarpModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WPDpt={0:0.00}", OSC3_WPDpt).PadRight(20)).AppendLine("// WarpModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", OSC3_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format("Curve={0}", OSC3_Curve).PadRight(20)).AppendLine("// binary data for Curve");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Prec={0:0.00}", OSC3_Prec).PadRight(20)).AppendLine("// Resolution");
			buffer.Append(String.Format("FX1Tp={0}", OSC3_FX1Tp).PadRight(20)).AppendLine("// SpectraFX1 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX1={0:0.00}", OSC3_SFX1).PadRight(20)).AppendLine("// SpectraFX1 Val");
			buffer.Append(String.Format("FX1Sc={0}", OSC3_FX1Sc).PadRight(20)).AppendLine("// SFX1ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX1Dt={0:0.00}", OSC3_FX1Dt).PadRight(20)).AppendLine("// SFX1ModDepth");
			buffer.Append(String.Format("FX2Tp={0}", OSC3_FX2Tp).PadRight(20)).AppendLine("// SpectraFX2 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX2={0:0.00}", OSC3_SFX2).PadRight(20)).AppendLine("// SpectraFX2 Val");
			buffer.Append(String.Format("FX2Sc={0}", OSC3_FX2Sc).PadRight(20)).AppendLine("// SFX2ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX2Dt={0:0.00}", OSC3_FX2Dt).PadRight(20)).AppendLine("// SFX2ModDepth");
			buffer.Append(String.Format("Poly={0}", OSC3_Poly).PadRight(20)).AppendLine("// PolyWave");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", OSC3_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", OSC3_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", OSC3_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", OSC3_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", OSC3_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", OSC3_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", OSC3_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", OSC3_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sync={0:0.00}", OSC3_Sync).PadRight(20)).AppendLine("// SyncTune");
			buffer.Append(String.Format("SncSc={0}", OSC3_SncSc).PadRight(20)).AppendLine("// SyncModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SncDt={0:0.00}", OSC3_SncDt).PadRight(20)).AppendLine("// SyncModDepth");
			buffer.Append(String.Format("SncOn={0}", OSC3_SncOn).PadRight(20)).AppendLine("// Sync Active");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", OSC3_PolW).PadRight(20)).AppendLine("// Poly Width");
			buffer.Append(String.Format("PwmOn={0}", OSC3_PwmOn).PadRight(20)).AppendLine("// PWM Mode");
			buffer.Append(String.Format("WaTb={0}", OSC3_WaTb).PadRight(20)).AppendLine("// binary data for WaveTable");
			buffer.Append(String.Format("RePhs={0}", OSC3_RePhs).PadRight(20)).AppendLine("// Reset Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Norm={0:0.00}", OSC3_Norm).PadRight(20)).AppendLine("// Normalize");
			buffer.Append(String.Format("Rend={0}", OSC3_Rend).PadRight(20)).AppendLine("// Renderer");

			buffer.AppendLine("\n#cm=OSC4");
			buffer.Append(String.Format("Wave={0}", OSC4_Wave).PadRight(20)).AppendLine("// WaveForm");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", OSC4_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", OSC4_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", OSC4_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", OSC4_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Phse={0:0.00}", OSC4_Phse).PadRight(20)).AppendLine("// Phase");
			buffer.Append(String.Format("PhsMSrc={0}", OSC4_PhsMSrc).PadRight(20)).AppendLine("// PhaseModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhsMDpt={0:0.00}", OSC4_PhsMDpt).PadRight(20)).AppendLine("// PhaseModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WNum={0:0.00}", OSC4_WNum).PadRight(20)).AppendLine("// WaveWarp");
			buffer.Append(String.Format("WPSrc={0}", OSC4_WPSrc).PadRight(20)).AppendLine("// WarpModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "WPDpt={0:0.00}", OSC4_WPDpt).PadRight(20)).AppendLine("// WarpModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", OSC4_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format("Curve={0}", OSC4_Curve).PadRight(20)).AppendLine("// binary data for Curve");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Prec={0:0.00}", OSC4_Prec).PadRight(20)).AppendLine("// Resolution");
			buffer.Append(String.Format("FX1Tp={0}", OSC4_FX1Tp).PadRight(20)).AppendLine("// SpectraFX1 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX1={0:0.00}", OSC4_SFX1).PadRight(20)).AppendLine("// SpectraFX1 Val");
			buffer.Append(String.Format("FX1Sc={0}", OSC4_FX1Sc).PadRight(20)).AppendLine("// SFX1ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX1Dt={0:0.00}", OSC4_FX1Dt).PadRight(20)).AppendLine("// SFX1ModDepth");
			buffer.Append(String.Format("FX2Tp={0}", OSC4_FX2Tp).PadRight(20)).AppendLine("// SpectraFX2 Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SFX2={0:0.00}", OSC4_SFX2).PadRight(20)).AppendLine("// SpectraFX2 Val");
			buffer.Append(String.Format("FX2Sc={0}", OSC4_FX2Sc).PadRight(20)).AppendLine("// SFX2ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FX2Dt={0:0.00}", OSC4_FX2Dt).PadRight(20)).AppendLine("// SFX2ModDepth");
			buffer.Append(String.Format("Poly={0}", OSC4_Poly).PadRight(20)).AppendLine("// PolyWave");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", OSC4_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", OSC4_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", OSC4_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", OSC4_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", OSC4_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", OSC4_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", OSC4_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", OSC4_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sync={0:0.00}", OSC4_Sync).PadRight(20)).AppendLine("// SyncTune");
			buffer.Append(String.Format("SncSc={0}", OSC4_SncSc).PadRight(20)).AppendLine("// SyncModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SncDt={0:0.00}", OSC4_SncDt).PadRight(20)).AppendLine("// SyncModDepth");
			buffer.Append(String.Format("SncOn={0}", OSC4_SncOn).PadRight(20)).AppendLine("// Sync Active");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", OSC4_PolW).PadRight(20)).AppendLine("// Poly Width");
			buffer.Append(String.Format("PwmOn={0}", OSC4_PwmOn).PadRight(20)).AppendLine("// PWM Mode");
			buffer.Append(String.Format("WaTb={0}", OSC4_WaTb).PadRight(20)).AppendLine("// binary data for WaveTable");
			buffer.Append(String.Format("RePhs={0}", OSC4_RePhs).PadRight(20)).AppendLine("// Reset Phase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Norm={0:0.00}", OSC4_Norm).PadRight(20)).AppendLine("// Normalize");
			buffer.Append(String.Format("Rend={0}", OSC4_Rend).PadRight(20)).AppendLine("// Renderer");

			buffer.AppendLine("\n#cm=Noise1");
			buffer.Append(String.Format("Type={0}", Noise1_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1={0:0.00}", Noise1_F1).PadRight(20)).AppendLine("// Filter1");
			buffer.Append(String.Format("F1Src={0}", Noise1_F1Src).PadRight(20)).AppendLine("// F1 ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1Dpt={0:0.00}", Noise1_F1Dpt).PadRight(20)).AppendLine("// F1 ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2={0:0.00}", Noise1_F2).PadRight(20)).AppendLine("// Filter2");
			buffer.Append(String.Format("F2Src={0}", Noise1_F2Src).PadRight(20)).AppendLine("// F2 ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2Dpt={0:0.00}", Noise1_F2Dpt).PadRight(20)).AppendLine("// F2 ModDepth");
			buffer.Append(String.Format("KVsc={0}", Noise1_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", Noise1_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", Noise1_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", Noise1_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Noise1_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", Noise1_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", Noise1_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", Noise1_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", Noise1_PolW).PadRight(20)).AppendLine("// Width");

			buffer.AppendLine("\n#cm=Noise2");
			buffer.Append(String.Format("Type={0}", Noise2_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1={0:0.00}", Noise2_F1).PadRight(20)).AppendLine("// Filter1");
			buffer.Append(String.Format("F1Src={0}", Noise2_F1Src).PadRight(20)).AppendLine("// F1 ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F1Dpt={0:0.00}", Noise2_F1Dpt).PadRight(20)).AppendLine("// F1 ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2={0:0.00}", Noise2_F2).PadRight(20)).AppendLine("// Filter2");
			buffer.Append(String.Format("F2Src={0}", Noise2_F2Src).PadRight(20)).AppendLine("// F2 ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "F2Dpt={0:0.00}", Noise2_F2Dpt).PadRight(20)).AppendLine("// F2 ModDepth");
			buffer.Append(String.Format("KVsc={0}", Noise2_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", Noise2_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", Noise2_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", Noise2_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Noise2_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", Noise2_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", Noise2_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", Noise2_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", Noise2_PolW).PadRight(20)).AppendLine("// Width");

			buffer.AppendLine("\n#cm=VCF1");
			buffer.Append(String.Format("Typ={0}", VCF1_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF1_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF1_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF1_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF1_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF1_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF1_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF1_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF1_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF1_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=VCF2");
			buffer.Append(String.Format("Typ={0}", VCF2_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF2_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF2_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF2_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF2_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF2_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF2_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF2_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF2_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF2_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=VCF3");
			buffer.Append(String.Format("Typ={0}", VCF3_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF3_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF3_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF3_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF3_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF3_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF3_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF3_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF3_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF3_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=VCF4");
			buffer.Append(String.Format("Typ={0}", VCF4_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF4_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF4_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF4_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF4_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF4_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF4_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF4_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF4_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF4_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=FMO1");
			buffer.Append(String.Format("Wave={0}", FMO1_Wave).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", FMO1_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", FMO1_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", FMO1_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", FMO1_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM={0:0.00}", FMO1_FM).PadRight(20)).AppendLine("// FM Depth");
			buffer.Append(String.Format("FMSrc={0}", FMO1_FMSrc).PadRight(20)).AppendLine("// FM ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", FMO1_FMDpt).PadRight(20)).AppendLine("// FM ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", FMO1_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", FMO1_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", FMO1_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", FMO1_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", FMO1_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", FMO1_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", FMO1_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", FMO1_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", FMO1_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", FMO1_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", FMO1_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Getr={0}", FMO1_Getr).PadRight(20)).AppendLine("// Generator");

			buffer.AppendLine("\n#cm=FMO2");
			buffer.Append(String.Format("Wave={0}", FMO2_Wave).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", FMO2_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", FMO2_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", FMO2_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", FMO2_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM={0:0.00}", FMO2_FM).PadRight(20)).AppendLine("// FM Depth");
			buffer.Append(String.Format("FMSrc={0}", FMO2_FMSrc).PadRight(20)).AppendLine("// FM ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", FMO2_FMDpt).PadRight(20)).AppendLine("// FM ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", FMO2_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", FMO2_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", FMO2_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", FMO2_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", FMO2_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", FMO2_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", FMO2_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", FMO2_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", FMO2_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", FMO2_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", FMO2_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Getr={0}", FMO2_Getr).PadRight(20)).AppendLine("// Generator");

			buffer.AppendLine("\n#cm=FMO3");
			buffer.Append(String.Format("Wave={0}", FMO3_Wave).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", FMO3_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", FMO3_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", FMO3_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", FMO3_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM={0:0.00}", FMO3_FM).PadRight(20)).AppendLine("// FM Depth");
			buffer.Append(String.Format("FMSrc={0}", FMO3_FMSrc).PadRight(20)).AppendLine("// FM ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", FMO3_FMDpt).PadRight(20)).AppendLine("// FM ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", FMO3_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", FMO3_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", FMO3_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", FMO3_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", FMO3_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", FMO3_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", FMO3_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", FMO3_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", FMO3_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", FMO3_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", FMO3_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Getr={0}", FMO3_Getr).PadRight(20)).AppendLine("// Generator");

			buffer.AppendLine("\n#cm=FMO4");
			buffer.Append(String.Format("Wave={0}", FMO4_Wave).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", FMO4_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", FMO4_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", FMO4_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", FMO4_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM={0:0.00}", FMO4_FM).PadRight(20)).AppendLine("// FM Depth");
			buffer.Append(String.Format("FMSrc={0}", FMO4_FMSrc).PadRight(20)).AppendLine("// FM ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", FMO4_FMDpt).PadRight(20)).AppendLine("// FM ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", FMO4_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dtun={0:0.00}", FMO4_Dtun).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format("KVsc={0}", FMO4_KVsc).PadRight(20)).AppendLine("// binary data for KeyVelZones");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", FMO4_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", FMO4_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", FMO4_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", FMO4_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", FMO4_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", FMO4_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", FMO4_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", FMO4_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Getr={0}", FMO4_Getr).PadRight(20)).AppendLine("// Generator");

			buffer.AppendLine("\n#cm=Comb1");
			buffer.Append(String.Format("Mode={0}", Comb1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", Comb1_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", Comb1_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", Comb1_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", Comb1_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Detn={0:0.00}", Comb1_Detn).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", Comb1_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", Comb1_FB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format("FBSrc={0}", Comb1_FBSrc).PadRight(20)).AppendLine("// FBModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FBDpt={0:0.00}", Comb1_FBDpt).PadRight(20)).AppendLine("// FBModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Damp={0:0.00}", Comb1_Damp).PadRight(20)).AppendLine("// Damp");
			buffer.Append(String.Format("DmpSrc={0}", Comb1_DmpSrc).PadRight(20)).AppendLine("// DampModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DmpDpt={0:0.00}", Comb1_DmpDpt).PadRight(20)).AppendLine("// DampModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Exc={0:0.00}", Comb1_Exc).PadRight(20)).AppendLine("// PreFill");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Inj={0:0.00}", Comb1_Inj).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format("InjSrc={0}", Comb1_InjSrc).PadRight(20)).AppendLine("// InModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "InjDpt={0:0.00}", Comb1_InjDpt).PadRight(20)).AppendLine("// InputMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tne={0:0.00}", Comb1_Tne).PadRight(20)).AppendLine("// Tone");
			buffer.Append(String.Format("TneSrc={0}", Comb1_TneSrc).PadRight(20)).AppendLine("// ToneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TneDpt={0:0.00}", Comb1_TneDpt).PadRight(20)).AppendLine("// ToneMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sec={0:0.00}", Comb1_Sec).PadRight(20)).AppendLine("// Flavour");
			buffer.Append(String.Format("SecSrc={0}", Comb1_SecSrc).PadRight(20)).AppendLine("// SecModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SecDpt={0:0.00}", Comb1_SecDpt).PadRight(20)).AppendLine("// FlavourMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dist={0:0.00}", Comb1_Dist).PadRight(20)).AppendLine("// Distortion");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dry={0:0.00}", Comb1_Dry).PadRight(20)).AppendLine("// Dry");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", Comb1_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", Comb1_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", Comb1_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Comb1_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", Comb1_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", Comb1_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", Comb1_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", Comb1_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Fill={0}", Comb1_Fill).PadRight(20)).AppendLine("// Fill");

			buffer.AppendLine("\n#cm=Comb2");
			buffer.Append(String.Format("Mode={0}", Comb2_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tune={0:0.00}", Comb2_Tune).PadRight(20)).AppendLine("// Tune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", Comb2_KeyScl).PadRight(20)).AppendLine("// key scale");
			buffer.Append(String.Format("TMSrc={0}", Comb2_TMSrc).PadRight(20)).AppendLine("// TuneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TMDpt={0:0.00}", Comb2_TMDpt).PadRight(20)).AppendLine("// TuneModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Detn={0:0.00}", Comb2_Detn).PadRight(20)).AppendLine("// Detune");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VtoD={0:0.00}", Comb2_VtoD).PadRight(20)).AppendLine("// Vibrato");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", Comb2_FB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format("FBSrc={0}", Comb2_FBSrc).PadRight(20)).AppendLine("// FBModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FBDpt={0:0.00}", Comb2_FBDpt).PadRight(20)).AppendLine("// FBModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Damp={0:0.00}", Comb2_Damp).PadRight(20)).AppendLine("// Damp");
			buffer.Append(String.Format("DmpSrc={0}", Comb2_DmpSrc).PadRight(20)).AppendLine("// DampModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DmpDpt={0:0.00}", Comb2_DmpDpt).PadRight(20)).AppendLine("// DampModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Exc={0:0.00}", Comb2_Exc).PadRight(20)).AppendLine("// PreFill");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Inj={0:0.00}", Comb2_Inj).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format("InjSrc={0}", Comb2_InjSrc).PadRight(20)).AppendLine("// InModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "InjDpt={0:0.00}", Comb2_InjDpt).PadRight(20)).AppendLine("// InputMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Tne={0:0.00}", Comb2_Tne).PadRight(20)).AppendLine("// Tone");
			buffer.Append(String.Format("TneSrc={0}", Comb2_TneSrc).PadRight(20)).AppendLine("// ToneModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "TneDpt={0:0.00}", Comb2_TneDpt).PadRight(20)).AppendLine("// ToneMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sec={0:0.00}", Comb2_Sec).PadRight(20)).AppendLine("// Flavour");
			buffer.Append(String.Format("SecSrc={0}", Comb2_SecSrc).PadRight(20)).AppendLine("// SecModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SecDpt={0:0.00}", Comb2_SecDpt).PadRight(20)).AppendLine("// FlavourMod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dist={0:0.00}", Comb2_Dist).PadRight(20)).AppendLine("// Distortion");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dry={0:0.00}", Comb2_Dry).PadRight(20)).AppendLine("// Dry");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol={0:0.00}", Comb2_Vol).PadRight(20)).AppendLine("// Volume");
			buffer.Append(String.Format("VolSc={0}", Comb2_VolSc).PadRight(20)).AppendLine("// VolumeModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "VolDt={0:0.00}", Comb2_VolDt).PadRight(20)).AppendLine("// VolumeModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Comb2_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format("PanSc={0}", Comb2_PanSc).PadRight(20)).AppendLine("// PanModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanDt={0:0.00}", Comb2_PanDt).PadRight(20)).AppendLine("// PanModDepth");
			buffer.Append(String.Format("Poly={0}", Comb2_Poly).PadRight(20)).AppendLine("// Poly");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PolW={0:0.00}", Comb2_PolW).PadRight(20)).AppendLine("// Width");
			buffer.Append(String.Format("Fill={0}", Comb2_Fill).PadRight(20)).AppendLine("// Fill");

			buffer.AppendLine("\n#cm=Shape1");
			buffer.Append(String.Format("Type={0}", Shape1_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Depth={0:0.00}", Shape1_Depth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format("DMSrc={0}", Shape1_DMSrc).PadRight(20)).AppendLine("// D_ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMDpt={0:0.00}", Shape1_DMDpt).PadRight(20)).AppendLine("// D_ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Edge={0:0.00}", Shape1_Edge).PadRight(20)).AppendLine("// Edge");
			buffer.Append(String.Format("EMSrc={0}", Shape1_EMSrc).PadRight(20)).AppendLine("// Edge ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMDpt={0:0.00}", Shape1_EMDpt).PadRight(20)).AppendLine("// Edge ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Shape1_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Shape1_Output).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HiOut={0:0.00}", Shape1_HiOut).PadRight(20)).AppendLine("// HiOut");

			buffer.AppendLine("\n#cm=Shape2");
			buffer.Append(String.Format("Type={0}", Shape2_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Depth={0:0.00}", Shape2_Depth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format("DMSrc={0}", Shape2_DMSrc).PadRight(20)).AppendLine("// D_ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMDpt={0:0.00}", Shape2_DMDpt).PadRight(20)).AppendLine("// D_ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Edge={0:0.00}", Shape2_Edge).PadRight(20)).AppendLine("// Edge");
			buffer.Append(String.Format("EMSrc={0}", Shape2_EMSrc).PadRight(20)).AppendLine("// Edge ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMDpt={0:0.00}", Shape2_EMDpt).PadRight(20)).AppendLine("// Edge ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Shape2_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Shape2_Output).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HiOut={0:0.00}", Shape2_HiOut).PadRight(20)).AppendLine("// HiOut");

			buffer.AppendLine("\n#cm=Mix1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix1_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix1_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix1_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix1_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix1_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=Mix2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix2_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix2_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix2_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix2_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix2_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=Mix3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix3_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix3_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix3_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix3_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix3_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=Mix4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix4_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix4_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix4_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix4_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix4_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=XMF1");
			buffer.Append(String.Format("Typ={0}", XMF1_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", XMF1_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", XMF1_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", XMF1_FM1).PadRight(20)).AppendLine("// Freq mod 1");
			buffer.Append(String.Format("FS1={0}", XMF1_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", XMF1_FM2).PadRight(20)).AppendLine("// Freq mod 2");
			buffer.Append(String.Format("FS2={0}", XMF1_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", XMF1_KeyScl).PadRight(20)).AppendLine("// KeyFollow");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOff={0:0.00}", XMF1_FOff).PadRight(20)).AppendLine("// FreqOffset");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOMod={0:0.00}", XMF1_FOMod).PadRight(20)).AppendLine("// FreqOffMod");
			buffer.Append(String.Format("FOSrc={0}", XMF1_FOSrc).PadRight(20)).AppendLine("// FreqOffSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFM={0:0.00}", XMF1_XFM).PadRight(20)).AppendLine("// FilterFM");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFMD={0:0.00}", XMF1_XFMD).PadRight(20)).AppendLine("// XFMmod");
			buffer.Append(String.Format("XFMS={0}", XMF1_XFMS).PadRight(20)).AppendLine("// XFMrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Bias={0:0.00}", XMF1_Bias).PadRight(20)).AppendLine("// Bias");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OLoad={0:0.00}", XMF1_OLoad).PadRight(20)).AppendLine("// Overload");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Click={0:0.00}", XMF1_Click).PadRight(20)).AppendLine("// Click");
			buffer.Append(String.Format("Drv={0}", XMF1_Drv).PadRight(20)).AppendLine("// Driver");
			buffer.Append(String.Format("Rout={0}", XMF1_Rout).PadRight(20)).AppendLine("// Routing");
			buffer.Append(String.Format("Typ2={0}", XMF1_Typ2).PadRight(20)).AppendLine("// Type2");

			buffer.AppendLine("\n#cm=XMF2");
			buffer.Append(String.Format("Typ={0}", XMF2_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", XMF2_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", XMF2_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", XMF2_FM1).PadRight(20)).AppendLine("// Freq mod 1");
			buffer.Append(String.Format("FS1={0}", XMF2_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", XMF2_FM2).PadRight(20)).AppendLine("// Freq mod 2");
			buffer.Append(String.Format("FS2={0}", XMF2_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", XMF2_KeyScl).PadRight(20)).AppendLine("// KeyFollow");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOff={0:0.00}", XMF2_FOff).PadRight(20)).AppendLine("// FreqOffset");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FOMod={0:0.00}", XMF2_FOMod).PadRight(20)).AppendLine("// FreqOffMod");
			buffer.Append(String.Format("FOSrc={0}", XMF2_FOSrc).PadRight(20)).AppendLine("// FreqOffSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFM={0:0.00}", XMF2_XFM).PadRight(20)).AppendLine("// FilterFM");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "XFMD={0:0.00}", XMF2_XFMD).PadRight(20)).AppendLine("// XFMmod");
			buffer.Append(String.Format("XFMS={0}", XMF2_XFMS).PadRight(20)).AppendLine("// XFMrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Bias={0:0.00}", XMF2_Bias).PadRight(20)).AppendLine("// Bias");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OLoad={0:0.00}", XMF2_OLoad).PadRight(20)).AppendLine("// Overload");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Click={0:0.00}", XMF2_Click).PadRight(20)).AppendLine("// Click");
			buffer.Append(String.Format("Drv={0}", XMF2_Drv).PadRight(20)).AppendLine("// Driver");
			buffer.Append(String.Format("Rout={0}", XMF2_Rout).PadRight(20)).AppendLine("// Routing");
			buffer.Append(String.Format("Typ2={0}", XMF2_Typ2).PadRight(20)).AppendLine("// Type2");

			buffer.AppendLine("\n#cm=SB1");
			buffer.Append(String.Format("Range={0}", SB1_Range).PadRight(20)).AppendLine("// Range");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Freq={0:0.00}", SB1_Freq).PadRight(20)).AppendLine("// Frequency");
			buffer.Append(String.Format("FMSrc={0}", SB1_FMSrc).PadRight(20)).AppendLine("// FModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", SB1_FMDpt).PadRight(20)).AppendLine("// FModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Offs={0:0.00}", SB1_Offs).PadRight(20)).AppendLine("// Offset");
			buffer.Append(String.Format("OMSrc={0}", SB1_OMSrc).PadRight(20)).AppendLine("// OModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OMDpt={0:0.00}", SB1_OMDpt).PadRight(20)).AppendLine("// OModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", SB1_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("MMSrc={0}", SB1_MMSrc).PadRight(20)).AppendLine("// MModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMDpt={0:0.00}", SB1_MMDpt).PadRight(20)).AppendLine("// MModDepth");

			buffer.AppendLine("\n#cm=SB2");
			buffer.Append(String.Format("Range={0}", SB2_Range).PadRight(20)).AppendLine("// Range");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Freq={0:0.00}", SB2_Freq).PadRight(20)).AppendLine("// Frequency");
			buffer.Append(String.Format("FMSrc={0}", SB2_FMSrc).PadRight(20)).AppendLine("// FModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", SB2_FMDpt).PadRight(20)).AppendLine("// FModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Offs={0:0.00}", SB2_Offs).PadRight(20)).AppendLine("// Offset");
			buffer.Append(String.Format("OMSrc={0}", SB2_OMSrc).PadRight(20)).AppendLine("// OModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OMDpt={0:0.00}", SB2_OMDpt).PadRight(20)).AppendLine("// OModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", SB2_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("MMSrc={0}", SB2_MMSrc).PadRight(20)).AppendLine("// MModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMDpt={0:0.00}", SB2_MMDpt).PadRight(20)).AppendLine("// MModDepth");

			buffer.AppendLine("\n#cm=VCA1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan1={0:0.00}", VCA1_Pan1).PadRight(20)).AppendLine("// Pan1");
			buffer.Append(String.Format("PanMS1={0}", VCA1_PanMS1).PadRight(20)).AppendLine("// Pan Mod Src1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD1={0:0.00}", VCA1_PanMD1).PadRight(20)).AppendLine("// Pan Mod Dpt1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol1={0:0.00}", VCA1_Vol1).PadRight(20)).AppendLine("// Volume1");
			buffer.Append(String.Format("VCA1={0}", VCA1_VCA1).PadRight(20)).AppendLine("// VCA1");
			buffer.Append(String.Format("ModSrc1={0}", VCA1_ModSrc1).PadRight(20)).AppendLine("// Modulation1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt1={0:0.00}", VCA1_ModDpt1).PadRight(20)).AppendLine("// Mod Depth1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan2={0:0.00}", VCA1_Pan2).PadRight(20)).AppendLine("// Pan2");
			buffer.Append(String.Format("PanMS2={0}", VCA1_PanMS2).PadRight(20)).AppendLine("// Pan Mod Src2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD2={0:0.00}", VCA1_PanMD2).PadRight(20)).AppendLine("// Pan Mod Dpt2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol2={0:0.00}", VCA1_Vol2).PadRight(20)).AppendLine("// Volume2");
			buffer.Append(String.Format("VCA2={0}", VCA1_VCA2).PadRight(20)).AppendLine("// VCA2");
			buffer.Append(String.Format("ModSrc2={0}", VCA1_ModSrc2).PadRight(20)).AppendLine("// Modulation2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt2={0:0.00}", VCA1_ModDpt2).PadRight(20)).AppendLine("// Mod Depth2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan3={0:0.00}", VCA1_Pan3).PadRight(20)).AppendLine("// Pan3");
			buffer.Append(String.Format("PanMS3={0}", VCA1_PanMS3).PadRight(20)).AppendLine("// Pan Mod Src3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD3={0:0.00}", VCA1_PanMD3).PadRight(20)).AppendLine("// Pan Mod Dpt3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol3={0:0.00}", VCA1_Vol3).PadRight(20)).AppendLine("// Volume3");
			buffer.Append(String.Format("VCA3={0}", VCA1_VCA3).PadRight(20)).AppendLine("// VCA3");
			buffer.Append(String.Format("ModSrc3={0}", VCA1_ModSrc3).PadRight(20)).AppendLine("// Modulation3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt3={0:0.00}", VCA1_ModDpt3).PadRight(20)).AppendLine("// Mod Depth3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan4={0:0.00}", VCA1_Pan4).PadRight(20)).AppendLine("// Pan4");
			buffer.Append(String.Format("PanMS4={0}", VCA1_PanMS4).PadRight(20)).AppendLine("// Pan Mod Src4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PanMD4={0:0.00}", VCA1_PanMD4).PadRight(20)).AppendLine("// Pan Mod Dpt4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Vol4={0:0.00}", VCA1_Vol4).PadRight(20)).AppendLine("// Volume4");
			buffer.Append(String.Format("VCA4={0}", VCA1_VCA4).PadRight(20)).AppendLine("// VCA4");
			buffer.Append(String.Format("ModSrc4={0}", VCA1_ModSrc4).PadRight(20)).AppendLine("// Modulation4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "ModDpt4={0:0.00}", VCA1_ModDpt4).PadRight(20)).AppendLine("// Mod Depth4");
			buffer.Append(String.Format("MT1={0}", VCA1_MT1).PadRight(20)).AppendLine("// Mute1");
			buffer.Append(String.Format("MT2={0}", VCA1_MT2).PadRight(20)).AppendLine("// Mute2");
			buffer.Append(String.Format("MT3={0}", VCA1_MT3).PadRight(20)).AppendLine("// Mute3");
			buffer.Append(String.Format("MT4={0}", VCA1_MT4).PadRight(20)).AppendLine("// Mute4");
			buffer.Append(String.Format("PB1={0}", VCA1_PB1).PadRight(20)).AppendLine("// Panning1");
			buffer.Append(String.Format("PB2={0}", VCA1_PB2).PadRight(20)).AppendLine("// Panning2");
			buffer.Append(String.Format("PB3={0}", VCA1_PB3).PadRight(20)).AppendLine("// Panning3");
			buffer.Append(String.Format("PB4={0}", VCA1_PB4).PadRight(20)).AppendLine("// Panning4");
			buffer.Append(String.Format("Bus1={0}", VCA1_Bus1).PadRight(20)).AppendLine("// Bus1");
			buffer.Append(String.Format("Bus2={0}", VCA1_Bus2).PadRight(20)).AppendLine("// Bus2");
			buffer.Append(String.Format("Bus3={0}", VCA1_Bus3).PadRight(20)).AppendLine("// Bus3");
			buffer.Append(String.Format("Bus4={0}", VCA1_Bus4).PadRight(20)).AppendLine("// Bus4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Send1={0:0.00}", VCA1_Send1).PadRight(20)).AppendLine("// Send1");
			buffer.Append(String.Format("SnSrc1={0}", VCA1_SnSrc1).PadRight(20)).AppendLine("// SendMod1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SnDpt1={0:0.00}", VCA1_SnDpt1).PadRight(20)).AppendLine("// SendDepth1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Send2={0:0.00}", VCA1_Send2).PadRight(20)).AppendLine("// Send2");
			buffer.Append(String.Format("SnSrc2={0}", VCA1_SnSrc2).PadRight(20)).AppendLine("// SendMod2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "SnDpt2={0:0.00}", VCA1_SnDpt2).PadRight(20)).AppendLine("// SendDepth2");
			buffer.Append(String.Format("AttS={0}", VCA1_AttS).PadRight(20)).AppendLine("// AttackSmooth");

			buffer.AppendLine("\n#cm=GridFX");
			buffer.Append(String.Format("Grid={0}", GridFX_Grid).PadRight(20)).AppendLine("// binary data for Grid Structure");
			buffer.Append(String.Format("GByp={0}", GridFX_GByp).PadRight(20)).AppendLine("// Bypass");

			buffer.AppendLine("\n#cm=ModFX1");
			buffer.Append(String.Format("Mode={0}", ModFX1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cent={0:0.00}", ModFX1_Cent).PadRight(20)).AppendLine("// Center");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sped={0:0.00}", ModFX1_Sped).PadRight(20)).AppendLine("// Speed");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhOff={0:0.00}", ModFX1_PhOff).PadRight(20)).AppendLine("// Stereo");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dpth={0:0.00}", ModFX1_Dpth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FeeB={0:0.00}", ModFX1_FeeB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", ModFX1_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LCut={0:0.00}", ModFX1_LCut).PadRight(20)).AppendLine("// LowCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HCut={0:0.00}", ModFX1_HCut).PadRight(20)).AppendLine("// HiCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Quad={0:0.00}", ModFX1_Quad).PadRight(20)).AppendLine("// Quad");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Qphs={0:0.00}", ModFX1_Qphs).PadRight(20)).AppendLine("// QuadPhase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Leq={0:0.00}", ModFX1_Leq).PadRight(20)).AppendLine("// Low Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Heq={0:0.00}", ModFX1_Heq).PadRight(20)).AppendLine("// High Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q1={0:0.00}", ModFX1_Q1).PadRight(20)).AppendLine("// Q1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q2={0:0.00}", ModFX1_Q2).PadRight(20)).AppendLine("// Q2");
			buffer.Append(String.Format("EQon={0}", ModFX1_EQon).PadRight(20)).AppendLine("// EQ");

			buffer.AppendLine("\n#cm=ModFX2");
			buffer.Append(String.Format("Mode={0}", ModFX2_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cent={0:0.00}", ModFX2_Cent).PadRight(20)).AppendLine("// Center");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Sped={0:0.00}", ModFX2_Sped).PadRight(20)).AppendLine("// Speed");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PhOff={0:0.00}", ModFX2_PhOff).PadRight(20)).AppendLine("// Stereo");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dpth={0:0.00}", ModFX2_Dpth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FeeB={0:0.00}", ModFX2_FeeB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", ModFX2_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LCut={0:0.00}", ModFX2_LCut).PadRight(20)).AppendLine("// LowCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HCut={0:0.00}", ModFX2_HCut).PadRight(20)).AppendLine("// HiCut Freq");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Quad={0:0.00}", ModFX2_Quad).PadRight(20)).AppendLine("// Quad");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Qphs={0:0.00}", ModFX2_Qphs).PadRight(20)).AppendLine("// QuadPhase");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Leq={0:0.00}", ModFX2_Leq).PadRight(20)).AppendLine("// Low Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Heq={0:0.00}", ModFX2_Heq).PadRight(20)).AppendLine("// High Boost dB");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q1={0:0.00}", ModFX2_Q1).PadRight(20)).AppendLine("// Q1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Q2={0:0.00}", ModFX2_Q2).PadRight(20)).AppendLine("// Q2");
			buffer.Append(String.Format("EQon={0}", ModFX2_EQon).PadRight(20)).AppendLine("// EQ");

			buffer.AppendLine("\n#cm=Delay1");
			buffer.Append(String.Format("Mode={0}", Delay1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Delay1_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", Delay1_FB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "CB={0:0.00}", Delay1_CB).PadRight(20)).AppendLine("// X-back");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LP={0:0.00}", Delay1_LP).PadRight(20)).AppendLine("// Lowpass");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HP={0:0.00}", Delay1_HP).PadRight(20)).AppendLine("// Hipass");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", Delay1_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format("Sync1={0}", Delay1_Sync1).PadRight(20)).AppendLine("// Sync1");
			buffer.Append(String.Format("Sync2={0}", Delay1_Sync2).PadRight(20)).AppendLine("// Sync2");
			buffer.Append(String.Format("Sync3={0}", Delay1_Sync3).PadRight(20)).AppendLine("// Sync3");
			buffer.Append(String.Format("Sync4={0}", Delay1_Sync4).PadRight(20)).AppendLine("// Sync4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T0={0:0.00}", Delay1_T0).PadRight(20)).AppendLine("// Ratio1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T1={0:0.00}", Delay1_T1).PadRight(20)).AppendLine("// Ratio2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T2={0:0.00}", Delay1_T2).PadRight(20)).AppendLine("// Ratio3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T3={0:0.00}", Delay1_T3).PadRight(20)).AppendLine("// Ratio4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan1={0:0.00}", Delay1_Pan1).PadRight(20)).AppendLine("// Pan1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan2={0:0.00}", Delay1_Pan2).PadRight(20)).AppendLine("// Pan2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan3={0:0.00}", Delay1_Pan3).PadRight(20)).AppendLine("// Pan3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan4={0:0.00}", Delay1_Pan4).PadRight(20)).AppendLine("// Pan4");

			buffer.AppendLine("\n#cm=Delay2");
			buffer.Append(String.Format("Mode={0}", Delay2_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Delay2_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", Delay2_FB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "CB={0:0.00}", Delay2_CB).PadRight(20)).AppendLine("// X-back");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "LP={0:0.00}", Delay2_LP).PadRight(20)).AppendLine("// Lowpass");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HP={0:0.00}", Delay2_HP).PadRight(20)).AppendLine("// Hipass");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", Delay2_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format("Sync1={0}", Delay2_Sync1).PadRight(20)).AppendLine("// Sync1");
			buffer.Append(String.Format("Sync2={0}", Delay2_Sync2).PadRight(20)).AppendLine("// Sync2");
			buffer.Append(String.Format("Sync3={0}", Delay2_Sync3).PadRight(20)).AppendLine("// Sync3");
			buffer.Append(String.Format("Sync4={0}", Delay2_Sync4).PadRight(20)).AppendLine("// Sync4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T0={0:0.00}", Delay2_T0).PadRight(20)).AppendLine("// Ratio1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T1={0:0.00}", Delay2_T1).PadRight(20)).AppendLine("// Ratio2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T2={0:0.00}", Delay2_T2).PadRight(20)).AppendLine("// Ratio3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "T3={0:0.00}", Delay2_T3).PadRight(20)).AppendLine("// Ratio4");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan1={0:0.00}", Delay2_Pan1).PadRight(20)).AppendLine("// Pan1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan2={0:0.00}", Delay2_Pan2).PadRight(20)).AppendLine("// Pan2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan3={0:0.00}", Delay2_Pan3).PadRight(20)).AppendLine("// Pan3");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan4={0:0.00}", Delay2_Pan4).PadRight(20)).AppendLine("// Pan4");

			buffer.AppendLine("\n#cm=Shape3");
			buffer.Append(String.Format("Type={0}", Shape3_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Depth={0:0.00}", Shape3_Depth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format("DMSrc={0}", Shape3_DMSrc).PadRight(20)).AppendLine("// D_ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMDpt={0:0.00}", Shape3_DMDpt).PadRight(20)).AppendLine("// D_ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Edge={0:0.00}", Shape3_Edge).PadRight(20)).AppendLine("// Edge");
			buffer.Append(String.Format("EMSrc={0}", Shape3_EMSrc).PadRight(20)).AppendLine("// Edge ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMDpt={0:0.00}", Shape3_EMDpt).PadRight(20)).AppendLine("// Edge ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Shape3_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Shape3_Output).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HiOut={0:0.00}", Shape3_HiOut).PadRight(20)).AppendLine("// HiOut");

			buffer.AppendLine("\n#cm=Shape4");
			buffer.Append(String.Format("Type={0}", Shape4_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Depth={0:0.00}", Shape4_Depth).PadRight(20)).AppendLine("// Depth");
			buffer.Append(String.Format("DMSrc={0}", Shape4_DMSrc).PadRight(20)).AppendLine("// D_ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMDpt={0:0.00}", Shape4_DMDpt).PadRight(20)).AppendLine("// D_ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Edge={0:0.00}", Shape4_Edge).PadRight(20)).AppendLine("// Edge");
			buffer.Append(String.Format("EMSrc={0}", Shape4_EMSrc).PadRight(20)).AppendLine("// Edge ModSrc");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMDpt={0:0.00}", Shape4_EMDpt).PadRight(20)).AppendLine("// Edge ModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Shape4_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Shape4_Output).PadRight(20)).AppendLine("// Output");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "HiOut={0:0.00}", Shape4_HiOut).PadRight(20)).AppendLine("// HiOut");

			buffer.AppendLine("\n#cm=Mix5");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix5_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix5_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix5_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix5_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix5_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=Mix6");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pan={0:0.00}", Mix6_Pan).PadRight(20)).AppendLine("// Pan");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", Mix6_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("PnMd={0}", Mix6_PnMd).PadRight(20)).AppendLine("// Pan Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "PnMD={0:0.00}", Mix6_PnMD).PadRight(20)).AppendLine("// PanMod Depth");
			buffer.Append(String.Format("PnMS={0}", Mix6_PnMS).PadRight(20)).AppendLine("// PanMod Source");

			buffer.AppendLine("\n#cm=Rev1");
			buffer.Append(String.Format("Mode={0}", Rev1_Mode).PadRight(20)).AppendLine("// Mode");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dry={0:0.00}", Rev1_Dry).PadRight(20)).AppendLine("// Dry");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Wet={0:0.00}", Rev1_Wet).PadRight(20)).AppendLine("// Wet");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FB={0:0.00}", Rev1_FB).PadRight(20)).AppendLine("// Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Damp={0:0.00}", Rev1_Damp).PadRight(20)).AppendLine("// Damp");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Size={0:0.00}", Rev1_Size).PadRight(20)).AppendLine("// Range");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Spd={0:0.00}", Rev1_Spd).PadRight(20)).AppendLine("// Speed");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Dpt={0:0.00}", Rev1_Dpt).PadRight(20)).AppendLine("// Modulation");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DFB={0:0.00}", Rev1_DFB).PadRight(20)).AppendLine("// Diff Feedback");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DSize={0:0.00}", Rev1_DSize).PadRight(20)).AppendLine("// Diff Range");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "EMix={0:0.00}", Rev1_EMix).PadRight(20)).AppendLine("// Diff Mix");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DMod={0:0.00}", Rev1_DMod).PadRight(20)).AppendLine("// Diff Mod");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "DSpd={0:0.00}", Rev1_DSpd).PadRight(20)).AppendLine("// Diff Speed");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Pre={0:0.00}", Rev1_Pre).PadRight(20)).AppendLine("// PreDelay");

			buffer.AppendLine("\n#cm=Comp1");
			buffer.Append(String.Format("Type={0}", Comp1_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rat={0:0.00}", Comp1_Rat).PadRight(20)).AppendLine("// Compression");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Thres={0:0.00}", Comp1_Thres).PadRight(20)).AppendLine("// Threshold");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Att={0:0.00}", Comp1_Att).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", Comp1_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Comp1_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Comp1_Output).PadRight(20)).AppendLine("// Output");

			buffer.AppendLine("\n#cm=Comp2");
			buffer.Append(String.Format("Type={0}", Comp2_Type).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rat={0:0.00}", Comp2_Rat).PadRight(20)).AppendLine("// Compression");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Thres={0:0.00}", Comp2_Thres).PadRight(20)).AppendLine("// Threshold");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Att={0:0.00}", Comp2_Att).PadRight(20)).AppendLine("// Attack");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Rel={0:0.00}", Comp2_Rel).PadRight(20)).AppendLine("// Release");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Input={0:0.00}", Comp2_Input).PadRight(20)).AppendLine("// Input");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Output={0:0.00}", Comp2_Output).PadRight(20)).AppendLine("// Output");

			buffer.AppendLine("\n#cm=EQ1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc1={0:0.00}", EQ1_Fc1).PadRight(20)).AppendLine("// Freq LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res1={0:0.00}", EQ1_Res1).PadRight(20)).AppendLine("// Q LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain1={0:0.00}", EQ1_Gain1).PadRight(20)).AppendLine("// Gain LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc2={0:0.00}", EQ1_Fc2).PadRight(20)).AppendLine("// Freq Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res2={0:0.00}", EQ1_Res2).PadRight(20)).AppendLine("// Q Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain2={0:0.00}", EQ1_Gain2).PadRight(20)).AppendLine("// Gain Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc3={0:0.00}", EQ1_Fc3).PadRight(20)).AppendLine("// Freq Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res3={0:0.00}", EQ1_Res3).PadRight(20)).AppendLine("// Q Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain3={0:0.00}", EQ1_Gain3).PadRight(20)).AppendLine("// Gain Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc4={0:0.00}", EQ1_Fc4).PadRight(20)).AppendLine("// Freq HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res4={0:0.00}", EQ1_Res4).PadRight(20)).AppendLine("// Q HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain4={0:0.00}", EQ1_Gain4).PadRight(20)).AppendLine("// Gain HiShelf");

			buffer.AppendLine("\n#cm=EQ2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc1={0:0.00}", EQ2_Fc1).PadRight(20)).AppendLine("// Freq LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res1={0:0.00}", EQ2_Res1).PadRight(20)).AppendLine("// Q LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain1={0:0.00}", EQ2_Gain1).PadRight(20)).AppendLine("// Gain LowShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc2={0:0.00}", EQ2_Fc2).PadRight(20)).AppendLine("// Freq Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res2={0:0.00}", EQ2_Res2).PadRight(20)).AppendLine("// Q Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain2={0:0.00}", EQ2_Gain2).PadRight(20)).AppendLine("// Gain Mid1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc3={0:0.00}", EQ2_Fc3).PadRight(20)).AppendLine("// Freq Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res3={0:0.00}", EQ2_Res3).PadRight(20)).AppendLine("// Q Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain3={0:0.00}", EQ2_Gain3).PadRight(20)).AppendLine("// Gain Mid2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "fc4={0:0.00}", EQ2_Fc4).PadRight(20)).AppendLine("// Freq HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "res4={0:0.00}", EQ2_Res4).PadRight(20)).AppendLine("// Q HiShelf");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "gain4={0:0.00}", EQ2_Gain4).PadRight(20)).AppendLine("// Gain HiShelf");

			buffer.AppendLine("\n#cm=VCF5");
			buffer.Append(String.Format("Typ={0}", VCF5_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF5_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF5_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF5_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF5_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF5_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF5_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF5_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF5_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF5_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=VCF6");
			buffer.Append(String.Format("Typ={0}", VCF6_Typ).PadRight(20)).AppendLine("// Type");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Cut={0:0.00}", VCF6_Cut).PadRight(20)).AppendLine("// Cutoff");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Res={0:0.00}", VCF6_Res).PadRight(20)).AppendLine("// Resonance");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Drv={0:0.00}", VCF6_Drv).PadRight(20)).AppendLine("// Drive");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Gain={0:0.00}", VCF6_Gain).PadRight(20)).AppendLine("// Gain");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM1={0:0.00}", VCF6_FM1).PadRight(20)).AppendLine("// ModDepth1");
			buffer.Append(String.Format("FS1={0}", VCF6_FS1).PadRight(20)).AppendLine("// Modsource1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FM2={0:0.00}", VCF6_FM2).PadRight(20)).AppendLine("// ModDepth2");
			buffer.Append(String.Format("FS2={0}", VCF6_FS2).PadRight(20)).AppendLine("// Modsource2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "KeyScl={0:0.00}", VCF6_KeyScl).PadRight(20)).AppendLine("// KeyFollow");

			buffer.AppendLine("\n#cm=SB3");
			buffer.Append(String.Format("Range={0}", SB3_Range).PadRight(20)).AppendLine("// Range");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Freq={0:0.00}", SB3_Freq).PadRight(20)).AppendLine("// Frequency");
			buffer.Append(String.Format("FMSrc={0}", SB3_FMSrc).PadRight(20)).AppendLine("// FModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "FMDpt={0:0.00}", SB3_FMDpt).PadRight(20)).AppendLine("// FModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Offs={0:0.00}", SB3_Offs).PadRight(20)).AppendLine("// Offset");
			buffer.Append(String.Format("OMSrc={0}", SB3_OMSrc).PadRight(20)).AppendLine("// OModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "OMDpt={0:0.00}", SB3_OMDpt).PadRight(20)).AppendLine("// OModDepth");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mix={0:0.00}", SB3_Mix).PadRight(20)).AppendLine("// Mix");
			buffer.Append(String.Format("MMSrc={0}", SB3_MMSrc).PadRight(20)).AppendLine("// MModSource");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "MMDpt={0:0.00}", SB3_MMDpt).PadRight(20)).AppendLine("// MModDepth");

			buffer.AppendLine("\n#cm=ZMas");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Ret1={0:0.00}", ZMas_Ret1).PadRight(20)).AppendLine("// Return1");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Ret2={0:0.00}", ZMas_Ret2).PadRight(20)).AppendLine("// Return2");
			buffer.Append(String.Format(CultureInfo.InvariantCulture, "Mast={0:0.00}", ZMas_Mast).PadRight(20)).AppendLine("// Master");
			buffer.Append(String.Format("XY1L={0}", ZMas_XY1L).PadRight(20)).AppendLine("// binary data for XY1 Label");
			buffer.Append(String.Format("XY2L={0}", ZMas_XY2L).PadRight(20)).AppendLine("// binary data for XY2 Label");
			buffer.Append(String.Format("XY3L={0}", ZMas_XY3L).PadRight(20)).AppendLine("// binary data for XY3 Label");
			buffer.Append(String.Format("XY4L={0}", ZMas_XY4L).PadRight(20)).AppendLine("// binary data for XY4 Label");
			buffer.Append(String.Format("XY1T={0}", ZMas_XY1T).PadRight(20)).AppendLine("// binary data for XY1 Text");
			buffer.Append(String.Format("XY2T={0}", ZMas_XY2T).PadRight(20)).AppendLine("// binary data for XY2 Text");
			buffer.Append(String.Format("XY3T={0}", ZMas_XY3T).PadRight(20)).AppendLine("// binary data for XY3 Text");
			buffer.Append(String.Format("XY4T={0}", ZMas_XY4T).PadRight(20)).AppendLine("// binary data for XY4 Text");
			buffer.Append(String.Format("OSC1={0}", ZMas_OSC1).PadRight(20)).AppendLine("// binary data for Oscillator1");
			buffer.Append(String.Format("OSC2={0}", ZMas_OSC2).PadRight(20)).AppendLine("// binary data for Oscillator2");
			buffer.Append(String.Format("OSC3={0}", ZMas_OSC3).PadRight(20)).AppendLine("// binary data for Oscillator3");
			buffer.Append(String.Format("OSC4={0}", ZMas_OSC4).PadRight(20)).AppendLine("// binary data for Oscillator4");
			buffer.Append(String.Format("MSEG1={0}", ZMas_MSEG1).PadRight(20)).AppendLine("// binary data for MSEG1");
			buffer.Append(String.Format("MSEG2={0}", ZMas_MSEG2).PadRight(20)).AppendLine("// binary data for MSEG2");
			buffer.Append(String.Format("MSEG3={0}", ZMas_MSEG3).PadRight(20)).AppendLine("// binary data for MSEG3");
			buffer.Append(String.Format("MSEG4={0}", ZMas_MSEG4).PadRight(20)).AppendLine("// binary data for MSEG4");
			buffer.Append(String.Format("Rev1={0}", ZMas_Rev1).PadRight(20)).AppendLine("// binary data for Rev1");
			buffer.Append(String.Format("Pn3={0}", ZMas_Pn3).PadRight(20)).AppendLine("// FXPaneMem");
			buffer.Append(String.Format("Pn4={0}", ZMas_Pn4).PadRight(20)).AppendLine("// OSC1PaneMem");
			buffer.Append(String.Format("Pn5={0}", ZMas_Pn5).PadRight(20)).AppendLine("// OSC2PaneMem");
			buffer.Append(String.Format("Pn6={0}", ZMas_Pn6).PadRight(20)).AppendLine("// OSC3PaneMem");
			buffer.Append(String.Format("Pn7={0}", ZMas_Pn7).PadRight(20)).AppendLine("// OSC4PaneMem");
			buffer.Append(String.Format("Pn8={0}", ZMas_Pn8).PadRight(20)).AppendLine("// VCF1PaneMem");
			buffer.Append(String.Format("Pn9={0}", ZMas_Pn9).PadRight(20)).AppendLine("// VCF2PaneMem");
			buffer.Append(String.Format("Pn10={0}", ZMas_Pn10).PadRight(20)).AppendLine("// VCF3PaneMem");
			buffer.Append(String.Format("Pn11={0}", ZMas_Pn11).PadRight(20)).AppendLine("// VCF4PaneMem");
			buffer.Append(String.Format("Rack0={0}", ZMas_Rack0).PadRight(20)).AppendLine("// binary data for MainRackMem");
			buffer.Append(String.Format("Rack1={0}", ZMas_Rack1).PadRight(20)).AppendLine("// binary data for ModRackMem");

			buffer.AppendLine();
			buffer.AppendLine();
			buffer.AppendLine();
			buffer.AppendLine("// Section for ugly compressed binary Data");
			buffer.AppendLine("// DON'T TOUCH THIS");
			buffer.AppendLine();
			buffer.Append("$$$$");
			buffer.AppendLine(strangeNumberBeforeBinaryChunk.ToString());
			
			// split into chunks of 64
			foreach (var s in StringUtils.SplitByLength(UglyCompressedBinaryData, 64)) {
				buffer.AppendLine(s);
			}
			return buffer.ToString();
		}
		#endregion

		public void Write(string filePath) {
			string presetString = GeneratePresetContent();
			
			// create a writer and open the file
			TextWriter tw = new StreamWriter(filePath);
			
			// write the preset string
			tw.Write(presetString);
			
			// close the stream
			tw.Close();
			
			Console.Out.WriteLine("Finished writing preset file: {0} ...", filePath);			
		}

		#region Generate Preset Sections
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
			buffer.Append(String.Format("Slew={0}", (int)LFOSlew.off).PadRight(20)).AppendLine("// Slew"); // 0 = off, 1 = fast, 2 = slow
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
		
		public enum ReverbMode : int {
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
		#endregion
		
		public string GeneratePresetContentOLD() {
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
