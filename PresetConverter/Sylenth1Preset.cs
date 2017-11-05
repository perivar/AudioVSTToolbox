using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using CommonUtils;

namespace PresetConverter
{
	/// <summary>
	/// Lennart Digital Sylenth 1 Preset format
	/// </summary>
	public class Sylenth1Preset : Preset
	{
		// define whether we should try to fix a sylenth preset file that has non exact enums?
		const bool DO_TRY_FIX_INCONSISTENT_ENUMS = true;

		#region Sylenth1 Enums
		public enum FloatToHz {
			DelayLowCut,
			DelayHighCut,
			FilterCutoff,
			EQBassFreq,
			EQTrebleFreq,
			LFORateFree
		}
		
		public enum ARPMODE : uint {
			Up         = 0x00000000,	// 0.000000
			Down       = 0x3DE38E39,	// 0.111111
			Up_Down    = 0x3E638E39,	// 0.222222
			Up_Down2   = 0x3EAAAAAB,	// 0.333333
			Down_Up    = 0x3EE38E39,	// 0.444444
			Down_Up2   = 0x3F0E38E4,	// 0.555556
			Random     = 0x3F2AAAAB,	// 0.666667
			Ordered    = 0x3F471C72,	// 0.777778
			Step       = 0x3F638E39,	// 0.888889
			Chord      = 0x3F800000		// 1.000000
		}

		public enum ARPVELO : uint {
			VEL_Key        = 0x00000000,	// 0.000000
			VEL_Hold       = 0x3E800000,	// 0.250000
			VEL_Step       = 0x3F000000,	// 0.500000
			VEL_StepKey    = 0x3F400000,	// 0.750000
			VEL_StepHold   = 0x3F800000		// 1.000000
		}
		
		public enum ARPOCTAVE : uint {
			OCTAVE_1 = 0x00000000,  // 0.000000
			OCTAVE_2 = 0x3EAAAAAB,  // 0.333333
			OCTAVE_3 = 0x3F2AAAAB,  // 0.666667
			OCTAVE_4 = 0x3F800000	// 1.000000
		}
		
		public enum ARPWRAP : uint {
			WRAP_0 = 0x00000000,	// 0.000000
			WRAP_1 = 0x3D800000,	// 0.062500
			WRAP_2 = 0x3E000000,	// 0.125000
			WRAP_3 = 0x3E400000,	// 0.187500
			WRAP_4 = 0x3E800000,	// 0.250000
			WRAP_5 = 0x3EA00000,	// 0.312500
			WRAP_6 = 0x3EC00000,	// 0.375000
			WRAP_7 = 0x3EE00000,	// 0.437500
			WRAP_8 = 0x3F000000,	// 0.500000
			WRAP_9 = 0x3F100000,	// 0.562500
			WRAP_10 = 0x3F200000,	// 0.625000
			WRAP_11 = 0x3F300000,	// 0.687500
			WRAP_12 = 0x3F400000,	// 0.750000
			WRAP_13 = 0x3F500000,	// 0.812500
			WRAP_14 = 0x3F600000,	// 0.875000
			WRAP_15 = 0x3F700000,	// 0.937500
			WRAP_16 = 0x3F800000	// 1.000000
		}

		public enum CHORUSMODE : uint {
			Single  = 0x00000000,	// 0.000000
			//Dual = 0x3F000000, 	// 0.5 sometimes this value is stored to mean dual
			Dual    = 0x3F800000	// 1.000000
		}

		public enum DISTORTTYPE : uint {
			OverDrv = 0x00000000,  // 0.000000
			BitCrush= 0x3F800000,  // 1.000000
			Clip    = 0x3F000000,  // 0.500000
			Decimate= 0x3F400000,  // 0.750000
			FoldBack= 0x3E800000
		}

		public enum FILTERAINPUT : uint {
			FILTER_A     = 0x00000000,  // 0.000000
			FILTER_A_B   = 0x3F000000,  // 0.500000
			FILTER_None  = 0x3F800000	// 1.000000
		}

		public enum FILTERBINPUT : uint {
			FILTER_B     = 0x00000000,  // 0.000000
			FILTER_B_A   = 0x3F000000,  // 0.500000
			FILTER_None  = 0x3F800000	// 1.000000
		}
		
		public enum FILTERTYPE : uint {
			Bypass  = 0x00000000,	// 0.000000
			Lowpass = 0x3EAAAAAB,	// 0.333333
			Bandpass= 0x3F2AAAAB,	// 0.666667
			Highpass= 0x3F800000,	// 1.000000
		}
		
		public enum FILTERDB : uint {
			DB24  = 0x00000000,	// 0.000000
			DB12  = 0x3F800000	// 1.000000
		}

		public enum LFOWAVE : uint {
			LFO_Sine      = 0x00000000,	// 0.000000
			LFO_Saw       = 0x3DCCCCCD,	// 0.100000
			LFO_Ramp      = 0x3E4CCCCD,	// 0.200000
			LFO_Triangle  = 0x3E99999A,	// 0.300000
			LFO_Pulse     = 0x3ECCCCCD,	// 0.400000
			LFO_HPulse    = 0x3F000000,	// 0.500000
			LFO_QPulse    = 0x3F19999A,	// 0.600000
			LFO_TriSaw    = 0x3F333333,	// 0.700000
			LFO_Lorenz    = 0x3F4CCCCD,	// 0.800000
			LFO_SmpHold   = 0x3F666666,	// 0.900000
			LFO_Random    = 0x3F800000	// 1.000000
		}

		public enum OSCWAVE : uint {
			OSC_Sine     = 0x00000000,	// 0.000000
			OSC_Saw      = 0x3E124925,	// 0.142857
			OSC_Triangle = 0x3E924925,	// 0.285714
			OSC_Pulse    = 0x3EDB6DB7,	// 0.428571
			OSC_QPulse   = 0x3F36DB6E,	// 0.714286
			OSC_HPulse   = 0x3F124925,	// 0.571429
			OSC_TriSaw   = 0x3F5B6DB7,	// 0.857143
			OSC_Noise    = 0x3F800000	// 1.000000
		}

		public enum PORTAMODE : uint {
			Slide    = 0x00000000,  // 0.000000
			Normal   = 0x3F800000	// 1.000000
		}

		public enum ONOFF : uint {
			Off = 0x00000000,	// 0.000000
			On  = 0x3F800000	// 1.000000
		}

		public enum XMODSOURCE : uint {
			SOURCE_None     = 0x00000000,	// 0.000000
			SOURCE_Velocity = 0x3DBA2E8C,	// 0.090909
			SOURCE_ModWheel = 0x3E3A2E8C,	// 0.181818
			SOURCE_KeyTrack = 0x3E8BA2E9,	// 0.272727
			SOURCE_AmpEnv_A = 0x3EBA2E8C,	// 0.363636
			SOURCE_AmpEnv_B = 0x3EE8BA2F,	// 0.454545
			SOURCE_ModEnv_1 = 0x3F0BA2E9,	// 0.545455
			SOURCE_ModEnv_2 = 0x3F22E8BA,	// 0.636364
			SOURCE_LFO_1    = 0x3F3A2E8C,	// 0.727273
			SOURCE_LFO_2    = 0x3F51745D,	// 0.818182
			SOURCE_Aftertch = 0x3F68BA2F,	// 0.909091
			SOURCE_StepVlty = 0x3F800000,	// 1.000000
		}

		public enum YMODDEST : uint {
			None        = 0x00000000,  // 0.000000

			// Oscillators
			Volume_A    = 0x3D000000,	// 0.031250
			Volume_B    = 0x3D800000,	// 0.062500
			VolumeAB    = 0x3DC00000,	// 0.093750
			Pitch_A     = 0x3E000000,	// 0.125000
			Pitch_B     = 0x3E200000,	// 0.156250
			Pitch_AB    = 0x3E400000,  	// 0.187500
			Phase_A     = 0x3E600000,	// 0.218750
			Phase_B     = 0x3E800000,  	// 0.250000
			Phase_AB    = 0x3E900000,	// 0.281250
			Pan_A       = 0x3EA00000,  	// 0.312500
			Pan_B       = 0x3EB00000,	// 0.343750
			Pan_AB      = 0x3EC00000,  	// 0.375000

			// Filters
			Cutoff_A    = 0x3ED00000,	// 0.406250
			Cutoff_B    = 0x3EE00000,	// 0.437500
			CutoffAB    = 0x3EF00000,	// 0.468750
			Reso_A      = 0x3F000000,	// 0.500000
			Reso_B      = 0x3F080000,	// 0.531250
			Reso_AB     = 0x3F100000,	// 0.562500

			// Misc
			PhsrFreq    = 0x3F180000,	// 0.593750
			Mix_A       = 0x3F200000,	// 0.625000
			Mix_B       = 0x3F280000,	// 0.656250
			Mix_AB      = 0x3F300000,	// 0.687500
			LFO1Rate    = 0x3F380000,	// 0.718750
			LFO1Gain    = 0x3F400000,	// 0.750000
			LFO2Rate    = 0x3F480000,	// 0.781250
			LFO2Gain    = 0x3F500000,	// 0.812500
			DistAmnt    = 0x3F580000	// 0.843750
		}

		public enum YPARTSELECT : uint {
			Part_A = 0x00000000,	// 0.000000
			Part_B = 0x3F800000		// 1.000000
		}

		public enum ZEQMODE : uint {
			MODE_1_Pole = 0x00000000,	// 0.000000
			MODE_2_Pole = 0x3F800000	// 1.000000
		}
		
		public enum VOICES : uint {
			VOICES_0       = 0x00000000,	// 0.000000
			VOICES_1       = 0x3E000000,	// 0.125000
			VOICES_2       = 0x3E800000,	// 0.250000
			VOICES_3       = 0x3EC00000,	// 0.375000
			VOICES_4       = 0x3F000000,	// 0.500000
			VOICES_5       = 0x3F200000,	// 0.625000
			VOICES_6       = 0x3F400000,	// 0.750000
			VOICES_7       = 0x3F600000,	// 0.875000
			VOICES_8       = 0x3F800000		// 1.000000
		}

		public enum COMPRATIO : int {
			RATIO_UNKNOWN = -1,
			RATIO_1_00_TO_ONE = 0,
			RATIO_1_01_TO_ONE,
			RATIO_1_02_TO_ONE,
			RATIO_1_03_TO_ONE,
			RATIO_1_04_TO_ONE,
			RATIO_1_05_TO_ONE,
			RATIO_1_06_TO_ONE,
			RATIO_1_07_TO_ONE,
			RATIO_1_08_TO_ONE,
			RATIO_1_09_TO_ONE,
			RATIO_1_10_TO_ONE,
			RATIO_1_11_TO_ONE,
			RATIO_1_12_TO_ONE,
			RATIO_1_13_TO_ONE,
			RATIO_1_14_TO_ONE,
			RATIO_1_15_TO_ONE,
			RATIO_1_16_TO_ONE,
			RATIO_1_17_TO_ONE,
			RATIO_1_18_TO_ONE,
			RATIO_1_19_TO_ONE,
			RATIO_1_20_TO_ONE,
			RATIO_1_21_TO_ONE,
			RATIO_1_22_TO_ONE,
			RATIO_1_23_TO_ONE,
			RATIO_1_24_TO_ONE,
			RATIO_1_25_TO_ONE,
			RATIO_1_26_TO_ONE,
			RATIO_1_27_TO_ONE,
			RATIO_1_28_TO_ONE,
			RATIO_1_29_TO_ONE,
			RATIO_1_30_TO_ONE,
			RATIO_1_31_TO_ONE,
			RATIO_1_32_TO_ONE,
			RATIO_1_33_TO_ONE,
			RATIO_1_34_TO_ONE,
			RATIO_1_35_TO_ONE,
			RATIO_1_36_TO_ONE,
			RATIO_1_37_TO_ONE,
			RATIO_1_38_TO_ONE,
			RATIO_1_39_TO_ONE,
			RATIO_1_40_TO_ONE,
			RATIO_1_41_TO_ONE,
			RATIO_1_42_TO_ONE,
			RATIO_1_43_TO_ONE,
			RATIO_1_44_TO_ONE,
			RATIO_1_45_TO_ONE,
			RATIO_1_46_TO_ONE,
			RATIO_1_47_TO_ONE,
			RATIO_1_48_TO_ONE,
			RATIO_1_49_TO_ONE,
			RATIO_1_50_TO_ONE,
			RATIO_1_51_TO_ONE,
			RATIO_1_52_TO_ONE,
			RATIO_1_54_TO_ONE,
			RATIO_1_55_TO_ONE,
			RATIO_1_56_TO_ONE,
			RATIO_1_57_TO_ONE,
			RATIO_1_58_TO_ONE,
			RATIO_1_60_TO_ONE,
			RATIO_1_61_TO_ONE,
			RATIO_1_62_TO_ONE,
			RATIO_1_63_TO_ONE,
			RATIO_1_64_TO_ONE,
			RATIO_1_66_TO_ONE,
			RATIO_1_67_TO_ONE,
			RATIO_1_68_TO_ONE,
			RATIO_1_70_TO_ONE,
			RATIO_1_71_TO_ONE,
			RATIO_1_73_TO_ONE,
			RATIO_1_74_TO_ONE,
			RATIO_1_75_TO_ONE,
			RATIO_1_78_TO_ONE,
			RATIO_1_79_TO_ONE,
			RATIO_1_80_TO_ONE,
			RATIO_1_81_TO_ONE,
			RATIO_1_83_TO_ONE,
			RATIO_1_84_TO_ONE,
			RATIO_1_86_TO_ONE,
			RATIO_1_88_TO_ONE,
			RATIO_1_90_TO_ONE,
			RATIO_1_91_TO_ONE,
			RATIO_1_93_TO_ONE,
			RATIO_1_95_TO_ONE,
			RATIO_1_96_TO_ONE,
			RATIO_1_98_TO_ONE,
			RATIO_2_00_TO_ONE,
			RATIO_2_02_TO_ONE,
			RATIO_2_04_TO_ONE,
			RATIO_2_06_TO_ONE,
			RATIO_2_08_TO_ONE,
			RATIO_2_10_TO_ONE,
			RATIO_2_12_TO_ONE,
			RATIO_2_14_TO_ONE,
			RATIO_2_16_TO_ONE,
			RATIO_2_18_TO_ONE,
			RATIO_2_21_TO_ONE,
			RATIO_2_23_TO_ONE,
			RATIO_2_25_TO_ONE,
			RATIO_2_28_TO_ONE,
			RATIO_2_30_TO_ONE,
			RATIO_2_33_TO_ONE,
			RATIO_2_35_TO_ONE,
			RATIO_2_38_TO_ONE,
			RATIO_2_41_TO_ONE,
			RATIO_2_43_TO_ONE,
			RATIO_2_46_TO_ONE,
			RATIO_2_49_TO_ONE,
			RATIO_2_52_TO_ONE,
			RATIO_2_55_TO_ONE,
			RATIO_2_58_TO_ONE,
			RATIO_2_61_TO_ONE,
			RATIO_2_65_TO_ONE,
			RATIO_2_68_TO_ONE,
			RATIO_2_72_TO_ONE,
			RATIO_2_75_TO_ONE,
			RATIO_2_79_TO_ONE,
			RATIO_2_82_TO_ONE,
			RATIO_2_86_TO_ONE,
			RATIO_2_90_TO_ONE,
			RATIO_2_94_TO_ONE,
			RATIO_2_98_TO_ONE,
			RATIO_3_03_TO_ONE,
			RATIO_3_07_TO_ONE,
			RATIO_3_11_TO_ONE,
			RATIO_3_16_TO_ONE,
			RATIO_3_21_TO_ONE,
			RATIO_3_26_TO_ONE,
			RATIO_3_31_TO_ONE,
			RATIO_3_36_TO_ONE,
			RATIO_3_41_TO_ONE,
			RATIO_3_47_TO_ONE,
			RATIO_3_53_TO_ONE,
			RATIO_3_59_TO_ONE,
			RATIO_3_65_TO_ONE,
			RATIO_3_71_TO_ONE,
			RATIO_3_78_TO_ONE,
			RATIO_3_85_TO_ONE,
			RATIO_3_92_TO_ONE,
			RATIO_3_99_TO_ONE,
			RATIO_4_07_TO_ONE,
			RATIO_4_15_TO_ONE,
			RATIO_4_23_TO_ONE,
			RATIO_4_32_TO_ONE,
			RATIO_4_41_TO_ONE,
			RATIO_4_50_TO_ONE,
			RATIO_4_60_TO_ONE,
			RATIO_4_70_TO_ONE,
			RATIO_4_81_TO_ONE,
			RATIO_4_92_TO_ONE,
			RATIO_5_04_TO_ONE,
			RATIO_5_16_TO_ONE,
			RATIO_5_29_TO_ONE,
			RATIO_5_42_TO_ONE,
			RATIO_5_56_TO_ONE,
			RATIO_5_71_TO_ONE,
			RATIO_5_87_TO_ONE,
			RATIO_6_04_TO_ONE,
			RATIO_6_22_TO_ONE,
			RATIO_6_40_TO_ONE,
			RATIO_6_60_TO_ONE,
			RATIO_6_82_TO_ONE,
			RATIO_7_04_TO_ONE,
			RATIO_7_28_TO_ONE,
			RATIO_7_54_TO_ONE,
			RATIO_7_82_TO_ONE,
			RATIO_8_12_TO_ONE,
			RATIO_8_44_TO_ONE,
			RATIO_8_79_TO_ONE,
			RATIO_9_17_TO_ONE,
			RATIO_9_59_TO_ONE,
			RATIO_10_04_TO_ONE,
			RATIO_10_54_TO_ONE,
			RATIO_11_09_TO_ONE,
			RATIO_11_71_TO_ONE,
			RATIO_12_39_TO_ONE,
			RATIO_13_16_TO_ONE,
			RATIO_14_03_TO_ONE,
			RATIO_15_02_TO_ONE,
			RATIO_16_17_TO_ONE,
			RATIO_17_50_TO_ONE,
			RATIO_19_07_TO_ONE,
			RATIO_20_96_TO_ONE,
			RATIO_23_26_TO_ONE,
			RATIO_26_12_TO_ONE,
			RATIO_29_79_TO_ONE,
			RATIO_34_65_TO_ONE,
			RATIO_41_42_TO_ONE,
			RATIO_51_47_TO_ONE,
			RATIO_67_96_TO_ONE,
			RATIO_100_TO_ONE
		}
		
		public enum ARPTRANSPOSE {
			MINUS_24 = -24,
			MINUS_23 = -23,
			MINUS_22 = -22,
			MINUS_21 = -21,
			MINUS_20 = -20,
			MINUS_19 = -19,
			MINUS_18 = -18,
			MINUS_17 = -17,
			MINUS_16 = -16,
			MINUS_15 = -15,
			MINUS_14 = -14,
			MINUS_13 = -13,
			MINUS_12 = -12,
			MINUS_11 = -11,
			MINUS_10 = -10,
			MINUS_9 = -9,
			MINUS_8 = -8,
			MINUS_7 = -7,
			MINUS_6 = -6,
			MINUS_5 = -5,
			MINUS_4 = -4,
			MINUS_3 = -3,
			MINUS_2 = -2,
			MINUS_1 = -1,
			ZERO = 0,
			PLUSS_1 = 1,
			PLUSS_2 = 2,
			PLUSS_3 = 3,
			PLUSS_4 = 4,
			PLUSS_5 = 5,
			PLUSS_6 = 6,
			PLUSS_7 = 7,
			PLUSS_8 = 8,
			PLUSS_9 = 9,
			PLUSS_10 = 10,
			PLUSS_11 = 11,
			PLUSS_12 = 12,
			PLUSS_13 = 13,
			PLUSS_14 = 14,
			PLUSS_15 = 15,
			PLUSS_16 = 16,
			PLUSS_17 = 17,
			PLUSS_18 = 18,
			PLUSS_19 = 19,
			PLUSS_20 = 20,
			PLUSS_21 = 21,
			PLUSS_22 = 22,
			PLUSS_23 = 23,
			PLUSS_24 = 24
		}
		
		public enum ARPSYNCTIMING {
			ARPSYNC_UNKNOWN = -1,
			ARPSYNC_1_64 = 0,
			ARPSYNC_1_32T = 1,
			ARPSYNC_1_64D = 2,
			ARPSYNC_1_32 = 3,
			ARPSYNC_1_16T = 4,
			ARPSYNC_1_32D = 5,
			ARPSYNC_1_16 = 6,
			ARPSYNC_1_8T = 7,
			ARPSYNC_1_16D = 8,
			ARPSYNC_1_8 = 9,
			ARPSYNC_1_4T = 10,
			ARPSYNC_1_8D = 11,
			ARPSYNC_1_4 = 12,
			ARPSYNC_1_2T = 13,
			ARPSYNC_1_4D = 14,
			ARPSYNC_1_2 = 15,
			ARPSYNC_1_1T = 16,
			ARPSYNC_1_2D = 17,
			ARPSYNC_1_1 = 18
		}
		
		public enum DELAYTIMING {
			DELAY_UNKNOWN = -1,
			DELAY_1_64 = 0,
			DELAY_1_32T = 1,
			DELAY_1_64D = 2,
			DELAY_1_32 = 3,
			DELAY_1_16T = 4,
			DELAY_1_32D = 5,
			DELAY_1_16 = 6,
			DELAY_1_8T = 7,
			DELAY_1_16D = 8,
			DELAY_1_8 = 9,
			DELAY_1_4T = 10,
			DELAY_1_8D = 11,
			DELAY_1_4 = 12,
			DELAY_1_2T = 13,
			DELAY_1_4D = 14,
			DELAY_1_2 = 15
		}
		
		public enum LFOTIMING {
			LFO_UNKNOWN = -1,
			LFO_8_1D = 0,
			LFO_8_1 = 1,
			LFO_4_1D = 2,
			LFO_8_1T = 3,
			LFO_4_1 = 4,
			LFO_2_1D = 5,
			LFO_4_1T = 6,
			LFO_2_1 = 7,
			LFO_1_1D = 8,
			LFO_2_1T = 9,
			LFO_1_1 = 10,
			LFO_1_2D = 11,
			LFO_1_1T = 12,
			LFO_1_2 = 13,
			LFO_1_4D = 14,
			LFO_1_2T = 15,
			LFO_1_4 = 16,
			LFO_1_8D = 17,
			LFO_1_4T = 18,
			LFO_1_8 = 19,
			LFO_1_16D = 20,
			LFO_1_8T = 21,
			LFO_1_16 = 22,
			LFO_1_32D = 23,
			LFO_1_16T = 24,
			LFO_1_32 = 25,
			LFO_1_64D = 26,
			LFO_1_32T = 27,
			LFO_1_64 = 28,
			LFO_1_128D = 29,
			LFO_1_64T = 30,
			LFO_1_128 = 31,
			LFO_1_256D = 32,
			LFO_1_128T = 33,
			LFO_1_256 = 34,
			LFO_1_256T = 35
		}
		#endregion
		
		public Syl1PresetContent[] ContentArray { set; get; }
		public string FilePath { set; get; }
		
		#region Constructors
		public Sylenth1Preset()
		{
		}
		
		public Sylenth1Preset(string filePath) {
			Read(filePath);
		}
		#endregion
		
		public static float EnumUintToFloat(uint uintValue) {
			// the values are stored as LittleEndian
			byte[] bytes = BitConverter.GetBytes(uintValue);
			return BitConverter.ToSingle(bytes, 0);
		}

		#region Value to Enums or String
		
		// ensure we read the sylenth values correctly
		// everything equal and above 0.5 is dual mode
		private static CHORUSMODE ConvertChorusFloatToEnum(float value) {
			if (value >= 0.5) {
				return CHORUSMODE.Dual;
			} else {
				return CHORUSMODE.Single;
			}
		}
		
		private static ONOFF ConvertOnOffFloatToEnum(float value) {
			if (value >= 0.5) {
				return ONOFF.On;
			} else {
				return ONOFF.Off;
			}
		}
		
		public static ARPTRANSPOSE ArpeggiatorTransposeFloatToEnum(float value) {
			float x = value;
			ARPTRANSPOSE transpose = ARPTRANSPOSE.ZERO;

			if (x >= 0.00f && x < 0.020408163f) {
				transpose = ARPTRANSPOSE.MINUS_24;
			} else if (x >= 0.020408163f && x < 0.040816327f) {
				transpose = ARPTRANSPOSE.MINUS_23;
			} else if (x >= 0.040816327f && x < 0.06122449f) {
				transpose = ARPTRANSPOSE.MINUS_22;
			} else if (x >= 0.06122449f && x < 0.081632653f) {
				transpose = ARPTRANSPOSE.MINUS_21;
			} else if (x >= 0.081632653f && x < 0.102040816f) {
				transpose = ARPTRANSPOSE.MINUS_20;
			} else if (x >= 0.102040816f && x < 0.12244898f) {
				transpose = ARPTRANSPOSE.MINUS_19;
			} else if (x >= 0.12244898f && x < 0.142857143f) {
				transpose = ARPTRANSPOSE.MINUS_18;
			} else if (x >= 0.142857143f && x < 0.163265306f) {
				transpose = ARPTRANSPOSE.MINUS_17;
			} else if (x >= 0.163265306f && x < 0.183673469f) {
				transpose = ARPTRANSPOSE.MINUS_16;
			} else if (x >= 0.183673469f && x < 0.204081633f) {
				transpose = ARPTRANSPOSE.MINUS_15;
			} else if (x >= 0.204081633f && x < 0.224489796f) {
				transpose = ARPTRANSPOSE.MINUS_14;
			} else if (x >= 0.224489796f && x < 0.244897959f) {
				transpose = ARPTRANSPOSE.MINUS_13;
			} else if (x >= 0.244897959f && x < 0.265306122f) {
				transpose = ARPTRANSPOSE.MINUS_12;
			} else if (x >= 0.265306122f && x < 0.285714286f) {
				transpose = ARPTRANSPOSE.MINUS_11;
			} else if (x >= 0.285714286f && x < 0.306122449f) {
				transpose = ARPTRANSPOSE.MINUS_10;
			} else if (x >= 0.306122449f && x < 0.326530612f) {
				transpose = ARPTRANSPOSE.MINUS_9;
			} else if (x >= 0.326530612f && x < 0.346938776f) {
				transpose = ARPTRANSPOSE.MINUS_8;
			} else if (x >= 0.346938776f && x < 0.367346939f) {
				transpose = ARPTRANSPOSE.MINUS_7;
			} else if (x >= 0.367346939f && x < 0.387755102f) {
				transpose = ARPTRANSPOSE.MINUS_6;
			} else if (x >= 0.387755102f && x < 0.408163265f) {
				transpose = ARPTRANSPOSE.MINUS_5;
			} else if (x >= 0.408163265f && x < 0.428571429f) {
				transpose = ARPTRANSPOSE.MINUS_4;
			} else if (x >= 0.428571429f && x < 0.448979592f) {
				transpose = ARPTRANSPOSE.MINUS_3;
			} else if (x >= 0.448979592f && x < 0.469387755f) {
				transpose = ARPTRANSPOSE.MINUS_2;
			} else if (x >= 0.469387755f && x < 0.489795918f) {
				transpose = ARPTRANSPOSE.MINUS_1;
			} else if (x >= 0.489795918f && x < 0.510204082f) {
				transpose = ARPTRANSPOSE.ZERO;
			} else if (x >= 0.510204082f && x < 0.530612245f) {
				transpose = ARPTRANSPOSE.PLUSS_1;
			} else if (x >= 0.530612245f && x < 0.551020408f) {
				transpose = ARPTRANSPOSE.PLUSS_2;
			} else if (x >= 0.551020408f && x < 0.571428571f) {
				transpose = ARPTRANSPOSE.PLUSS_3;
			} else if (x >= 0.571428571f && x < 0.591836735f) {
				transpose = ARPTRANSPOSE.PLUSS_4;
			} else if (x >= 0.591836735f && x < 0.612244898f) {
				transpose = ARPTRANSPOSE.PLUSS_5;
			} else if (x >= 0.612244898f && x < 0.632653061f) {
				transpose = ARPTRANSPOSE.PLUSS_6;
			} else if (x >= 0.632653061f && x < 0.653061224f) {
				transpose = ARPTRANSPOSE.PLUSS_7;
			} else if (x >= 0.653061224f && x < 0.673469388f) {
				transpose = ARPTRANSPOSE.PLUSS_8;
			} else if (x >= 0.673469388f && x < 0.693877551f) {
				transpose = ARPTRANSPOSE.PLUSS_9;
			} else if (x >= 0.693877551f && x < 0.714285714f) {
				transpose = ARPTRANSPOSE.PLUSS_10;
			} else if (x >= 0.714285714f && x < 0.734693878f) {
				transpose = ARPTRANSPOSE.PLUSS_11;
			} else if (x >= 0.734693878f && x < 0.755102041f) {
				transpose = ARPTRANSPOSE.PLUSS_12;
			} else if (x >= 0.755102041f && x < 0.775510204f) {
				transpose = ARPTRANSPOSE.PLUSS_13;
			} else if (x >= 0.775510204f && x < 0.795918367f) {
				transpose = ARPTRANSPOSE.PLUSS_14;
			} else if (x >= 0.795918367f && x < 0.816326531f) {
				transpose = ARPTRANSPOSE.PLUSS_15;
			} else if (x >= 0.816326531f && x < 0.836734694f) {
				transpose = ARPTRANSPOSE.PLUSS_16;
			} else if (x >= 0.836734694f && x < 0.857142857f) {
				transpose = ARPTRANSPOSE.PLUSS_17;
			} else if (x >= 0.857142857f && x < 0.87755102f) {
				transpose = ARPTRANSPOSE.PLUSS_18;
			} else if (x >= 0.87755102f && x < 0.897959184f) {
				transpose = ARPTRANSPOSE.PLUSS_19;
			} else if (x >= 0.897959184f && x < 0.918367347f) {
				transpose = ARPTRANSPOSE.PLUSS_20;
			} else if (x >= 0.918367347f && x < 0.93877551f) {
				transpose = ARPTRANSPOSE.PLUSS_21;
			} else if (x >= 0.93877551f && x < 0.959183673f) {
				transpose = ARPTRANSPOSE.PLUSS_22;
			} else if (x >= 0.959183673f && x < 0.979591837f) {
				transpose = ARPTRANSPOSE.PLUSS_23;
			} else if (x >= 0.979591837f && x <= 1.0f) {
				transpose = ARPTRANSPOSE.PLUSS_24;
			}
			return transpose;
		}

		public static ARPSYNCTIMING ArpeggiatorSyncTimeFloatToEnum(float value) {
			float x = value;
			ARPSYNCTIMING timing = ARPSYNCTIMING.ARPSYNC_UNKNOWN;

			if (x >= 0.0000000000f && x < 0.0380952656f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_1;
			} else if (x >= 0.0428571440f && x < 0.0904762200f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_2D;
			} else if (x >= 0.0952380747f && x < 0.1476190690f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_1T;
			} else if (x >= 0.1523809580f && x < 0.2000000630f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_2;
			} else if (x >= 0.2047619000f && x < 0.2571429000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_4D;
			} else if (x >= 0.2619047460f && x < 0.3095238210f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_2T;
			} else if (x >= 0.3142857000f && x < 0.3666666750f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_4;
			} else if (x >= 0.3714285490f && x < 0.4190476240f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_8D;
			} else if (x >= 0.4238095280f && x < 0.4761905370f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_4T;
			} else if (x >= 0.4809523820f && x < 0.5285714000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_8;
			} else if (x >= 0.5333333610f && x < 0.5857143400f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_16D;
			} else if (x >= 0.5904761550f && x < 0.6428571340f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_8T;
			} else if (x >= 0.6476190690f && x < 0.6952381000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_16;
			} else if (x >= 0.7000000000f && x < 0.7523809670f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_32D;
			} else if (x >= 0.7571428420f && x < 0.8047619000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_16T;
			} else if (x >= 0.8095238000f && x < 0.8619048000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_32;
			} else if (x >= 0.8666667000f && x < 0.9142858000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_64D;
			} else if (x >= 0.9190476000f && x < 0.9714286330f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_32T;
			} else if (x >= 0.9761904480f && x <= 1.0000000000f) {
				timing = ARPSYNCTIMING.ARPSYNC_1_64;
			}
			return timing;
		}
		
		public static DELAYTIMING DelayTimeFloatToEnum(float value) {
			// the rule for finding the Delay timing float ranges in a Sylenth presets
			// seems to be:
			// 
			// timingstep  1 = one divided by the number of unique timings minus one
			// timingstep  2 = one divided by the number of unique timings minus two
			// the first and the last have a special timing step
			// 
			// i.e for Delay:
			// normal timing step 1: =1/(COUNTA(H:H)-1) = 0,0666666667
			// normal timing step 2: =1/(COUNTA(H:H)-2) = 0,0714285714
			// first timing step: normal timing step 2 / 5 = 0,0142857143

			float x = value;
			DELAYTIMING timing = DELAYTIMING.DELAY_UNKNOWN;
			
			if (x >= 0.0000000000f && x < 0.0142857143f) {
				timing = DELAYTIMING.DELAY_1_64;
			} else if (x >= 0.0142857143f && x < 0.0809523810f) {
				timing = DELAYTIMING.DELAY_1_32T;
			} else if (x >= 0.0809523810f && x < 0.1523809524f) {
				timing = DELAYTIMING.DELAY_1_64D;
			} else if (x >= 0.1523809524f && x < 0.2190476190f) {
				timing = DELAYTIMING.DELAY_1_32;
			} else if (x >= 0.2190476190f && x < 0.2904761905f) {
				timing = DELAYTIMING.DELAY_1_16T;
			} else if (x >= 0.2904761905f && x < 0.3571428571f) {
				timing = DELAYTIMING.DELAY_1_32D;
			} else if (x >= 0.3571428571f && x < 0.4285714286f) {
				timing = DELAYTIMING.DELAY_1_16;
			} else if (x >= 0.4285714286f && x < 0.4952380952f) {
				timing = DELAYTIMING.DELAY_1_8T;
			} else if (x >= 0.4952380952f && x < 0.5666666667f) {
				timing = DELAYTIMING.DELAY_1_16D;
			} else if (x >= 0.5666666667f && x < 0.6333333333f) {
				timing = DELAYTIMING.DELAY_1_8;
			} else if (x >= 0.6333333333f && x < 0.7047619048f) {
				timing = DELAYTIMING.DELAY_1_4T;
			} else if (x >= 0.7047619048f && x < 0.7714285714f) {
				timing = DELAYTIMING.DELAY_1_8D;
			} else if (x >= 0.7714285714f && x < 0.8428571429f) {
				timing = DELAYTIMING.DELAY_1_4;
			} else if (x >= 0.8428571429f && x < 0.9095238095f) {
				timing = DELAYTIMING.DELAY_1_2T;
			} else if (x >= 0.9095238095f && x < 0.9809523810f) {
				timing = DELAYTIMING.DELAY_1_4D;
			} else if (x >= 0.9809523810f && x <= 1.0000000000f) {
				timing = DELAYTIMING.DELAY_1_2;
			}
			return timing;
		}

		public static LFOTIMING LFOTimeFloatToEnum(float value) {
			// the rule for finding the LFO timing float ranges in a Sylenth presets
			// seems to be:
			// 
			// timingstep = one divided by the number of unique timings minus one
			// and the first and the last have a half timing step
			// 
			// i.e for LFO:
			// normal timing step: =1/(COUNTA(H:H)-1) = 0,0285714286
			// first and last timing step: normal timing step / 2 = 0,0142857143
			
			float x = value;
			LFOTIMING timing = LFOTIMING.LFO_UNKNOWN;
			
			if (x >= 0.0000000000f && x < 0.0142857143f) {
				timing = LFOTIMING.LFO_8_1D;
			} else if (x >= 0.0142857143f && x < 0.0428571429f) {
				timing = LFOTIMING.LFO_8_1;
			} else if (x >= 0.0428571429f && x < 0.0714285714f) {
				timing = LFOTIMING.LFO_4_1D;
			} else if (x >= 0.0714285714f && x < 0.1000000000f) {
				timing = LFOTIMING.LFO_8_1T;
			} else if (x >= 0.1000000000f && x < 0.1285714286f) {
				timing = LFOTIMING.LFO_4_1;
			} else if (x >= 0.1285714286f && x < 0.1571428571f) {
				timing = LFOTIMING.LFO_2_1D;
			} else if (x >= 0.1571428571f && x < 0.1857142857f) {
				timing = LFOTIMING.LFO_4_1T;
			} else if (x >= 0.1857142857f && x < 0.2142857143f) {
				timing = LFOTIMING.LFO_2_1;
			} else if (x >= 0.2142857143f && x < 0.2428571429f) {
				timing = LFOTIMING.LFO_1_1D;
			} else if (x >= 0.2428571429f && x < 0.2714285714f) {
				timing = LFOTIMING.LFO_2_1T;
			} else if (x >= 0.2714285714f && x < 0.3000000000f) {
				timing = LFOTIMING.LFO_1_1;
			} else if (x >= 0.3000000000f && x < 0.3285714286f) {
				timing = LFOTIMING.LFO_1_2D;
			} else if (x >= 0.3285714286f && x < 0.3571428571f) {
				timing = LFOTIMING.LFO_1_1T;
			} else if (x >= 0.3571428571f && x < 0.3857142857f) {
				timing = LFOTIMING.LFO_1_2;
			} else if (x >= 0.3857142857f && x < 0.4142857143f) {
				timing = LFOTIMING.LFO_1_4D;
			} else if (x >= 0.4142857143f && x < 0.4428571429f) {
				timing = LFOTIMING.LFO_1_2T;
			} else if (x >= 0.4428571429f && x < 0.4714285714f) {
				timing = LFOTIMING.LFO_1_4;
			} else if (x >= 0.4714285714f && x < 0.5000000000f) {
				timing = LFOTIMING.LFO_1_8D;
			} else if (x >= 0.5000000000f && x < 0.5285714286f) {
				timing = LFOTIMING.LFO_1_4T;
			} else if (x >= 0.5285714286f && x < 0.5571428571f) {
				timing = LFOTIMING.LFO_1_8;
			} else if (x >= 0.5571428571f && x < 0.5857142857f) {
				timing = LFOTIMING.LFO_1_16D;
			} else if (x >= 0.5857142857f && x < 0.6142857143f) {
				timing = LFOTIMING.LFO_1_8T;
			} else if (x >= 0.6142857143f && x < 0.6428571429f) {
				timing = LFOTIMING.LFO_1_16;
			} else if (x >= 0.6428571429f && x < 0.6714285714f) {
				timing = LFOTIMING.LFO_1_32D;
			} else if (x >= 0.6714285714f && x < 0.7000000000f) {
				timing = LFOTIMING.LFO_1_16T;
			} else if (x >= 0.7000000000f && x < 0.7285714286f) {
				timing = LFOTIMING.LFO_1_32;
			} else if (x >= 0.7285714286f && x < 0.7571428571f) {
				timing = LFOTIMING.LFO_1_64D;
			} else if (x >= 0.7571428571f && x < 0.7857142857f) {
				timing = LFOTIMING.LFO_1_32T;
			} else if (x >= 0.7857142857f && x < 0.8142857143f) {
				timing = LFOTIMING.LFO_1_64;
			} else if (x >= 0.8142857143f && x < 0.8428571429f) {
				timing = LFOTIMING.LFO_1_128D;
			} else if (x >= 0.8428571429f && x < 0.8714285714f) {
				timing = LFOTIMING.LFO_1_64T;
			} else if (x >= 0.8714285714f && x < 0.9000000000f) {
				timing = LFOTIMING.LFO_1_128;
			} else if (x >= 0.9000000000f && x < 0.9285714286f) {
				timing = LFOTIMING.LFO_1_256D;
			} else if (x >= 0.9285714286f && x < 0.9571428571f) {
				timing = LFOTIMING.LFO_1_128T;
			} else if (x >= 0.9571428571f && x < 0.9857142857f) {
				timing = LFOTIMING.LFO_1_256;
			} else if (x >= 0.9857142857f && x <= 1.0000000000f) {
				timing = LFOTIMING.LFO_1_256T;
			}

			return timing;
		}
		
		public static COMPRATIO CompRatioFloatToEnum(float value) {
			// there is a very exponential curve going on but
			// i did not manage to find out the calculation
			
			float x = value;
			COMPRATIO ratio = COMPRATIO.RATIO_UNKNOWN;

			if (x >= 0.0000000000f && x < 0.0095238095f) {
				ratio = COMPRATIO.RATIO_1_00_TO_ONE;
			} else if (x >= 0.0095238095f && x < 0.0190476190f) {
				ratio = COMPRATIO.RATIO_1_01_TO_ONE;
			} else if (x >= 0.0190476190f && x < 0.0285714286f) {
				ratio = COMPRATIO.RATIO_1_02_TO_ONE;
			} else if (x >= 0.0285714286f && x < 0.0380952381f) {
				ratio = COMPRATIO.RATIO_1_03_TO_ONE;
			} else if (x >= 0.0380952381f && x < 0.0476190476f) {
				ratio = COMPRATIO.RATIO_1_04_TO_ONE;
			} else if (x >= 0.0476190476f && x < 0.0571428571f) {
				ratio = COMPRATIO.RATIO_1_05_TO_ONE;
			} else if (x >= 0.0571428571f && x < 0.0619047619f) {
				ratio = COMPRATIO.RATIO_1_06_TO_ONE;
			} else if (x >= 0.0619047619f && x < 0.0714285714f) {
				ratio = COMPRATIO.RATIO_1_07_TO_ONE;
			} else if (x >= 0.0714285714f && x < 0.0809523810f) {
				ratio = COMPRATIO.RATIO_1_08_TO_ONE;
			} else if (x >= 0.0809523810f && x < 0.0904761905f) {
				ratio = COMPRATIO.RATIO_1_09_TO_ONE;
			} else if (x >= 0.0904761905f && x < 0.1000000000f) {
				ratio = COMPRATIO.RATIO_1_10_TO_ONE;
			} else if (x >= 0.1000000000f && x < 0.1095238095f) {
				ratio = COMPRATIO.RATIO_1_11_TO_ONE;
			} else if (x >= 0.1095238095f && x < 0.1142857143f) {
				ratio = COMPRATIO.RATIO_1_12_TO_ONE;
			} else if (x >= 0.1142857143f && x < 0.1238095238f) {
				ratio = COMPRATIO.RATIO_1_13_TO_ONE;
			} else if (x >= 0.1238095238f && x < 0.1285714286f) {
				ratio = COMPRATIO.RATIO_1_14_TO_ONE;
			} else if (x >= 0.1285714286f && x < 0.1380952381f) {
				ratio = COMPRATIO.RATIO_1_15_TO_ONE;
			} else if (x >= 0.1380952381f && x < 0.1476190476f) {
				ratio = COMPRATIO.RATIO_1_16_TO_ONE;
			} else if (x >= 0.1476190476f && x < 0.1523809524f) {
				ratio = COMPRATIO.RATIO_1_17_TO_ONE;
			} else if (x >= 0.1523809524f && x < 0.1619047619f) {
				ratio = COMPRATIO.RATIO_1_18_TO_ONE;
			} else if (x >= 0.1619047619f && x < 0.1666666667f) {
				ratio = COMPRATIO.RATIO_1_19_TO_ONE;
			} else if (x >= 0.1666666667f && x < 0.1761904762f) {
				ratio = COMPRATIO.RATIO_1_20_TO_ONE;
			} else if (x >= 0.1761904762f && x < 0.1809523810f) {
				ratio = COMPRATIO.RATIO_1_21_TO_ONE;
			} else if (x >= 0.1809523810f && x < 0.1857142857f) {
				ratio = COMPRATIO.RATIO_1_22_TO_ONE;
			} else if (x >= 0.1857142857f && x < 0.1952380952f) {
				ratio = COMPRATIO.RATIO_1_23_TO_ONE;
			} else if (x >= 0.1952380952f && x < 0.2000000000f) {
				ratio = COMPRATIO.RATIO_1_24_TO_ONE;
			} else if (x >= 0.2000000000f && x < 0.2095238095f) {
				ratio = COMPRATIO.RATIO_1_25_TO_ONE;
			} else if (x >= 0.2095238095f && x < 0.2142857143f) {
				ratio = COMPRATIO.RATIO_1_26_TO_ONE;
			} else if (x >= 0.2142857143f && x < 0.2190476191f) {
				ratio = COMPRATIO.RATIO_1_27_TO_ONE;
			} else if (x >= 0.2190476191f && x < 0.2285714286f) {
				ratio = COMPRATIO.RATIO_1_28_TO_ONE;
			} else if (x >= 0.2285714286f && x < 0.2333333333f) {
				ratio = COMPRATIO.RATIO_1_29_TO_ONE;
			} else if (x >= 0.2333333333f && x < 0.2380952381f) {
				ratio = COMPRATIO.RATIO_1_30_TO_ONE;
			} else if (x >= 0.2380952381f && x < 0.2476190476f) {
				ratio = COMPRATIO.RATIO_1_31_TO_ONE;
			} else if (x >= 0.2476190476f && x < 0.2523809524f) {
				ratio = COMPRATIO.RATIO_1_32_TO_ONE;
			} else if (x >= 0.2523809524f && x < 0.2571428571f) {
				ratio = COMPRATIO.RATIO_1_33_TO_ONE;
			} else if (x >= 0.2571428571f && x < 0.2619047619f) {
				ratio = COMPRATIO.RATIO_1_34_TO_ONE;
			} else if (x >= 0.2619047619f && x < 0.2666666667f) {
				ratio = COMPRATIO.RATIO_1_35_TO_ONE;
			} else if (x >= 0.2666666667f && x < 0.2714285714f) {
				ratio = COMPRATIO.RATIO_1_36_TO_ONE;
			} else if (x >= 0.2714285714f && x < 0.2761904762f) {
				ratio = COMPRATIO.RATIO_1_37_TO_ONE;
			} else if (x >= 0.2761904762f && x < 0.2857142857f) {
				ratio = COMPRATIO.RATIO_1_38_TO_ONE;
			} else if (x >= 0.2857142857f && x < 0.2904761905f) {
				ratio = COMPRATIO.RATIO_1_39_TO_ONE;
			} else if (x >= 0.2904761905f && x < 0.2952380952f) {
				ratio = COMPRATIO.RATIO_1_40_TO_ONE;
			} else if (x >= 0.2952380952f && x < 0.3000000000f) {
				ratio = COMPRATIO.RATIO_1_41_TO_ONE;
			} else if (x >= 0.3000000000f && x < 0.3047619048f) {
				ratio = COMPRATIO.RATIO_1_42_TO_ONE;
			} else if (x >= 0.3047619048f && x < 0.3095238095f) {
				ratio = COMPRATIO.RATIO_1_43_TO_ONE;
			} else if (x >= 0.3095238095f && x < 0.3142857143f) {
				ratio = COMPRATIO.RATIO_1_44_TO_ONE;
			} else if (x >= 0.3142857143f && x < 0.3190476191f) {
				ratio = COMPRATIO.RATIO_1_45_TO_ONE;
			} else if (x >= 0.3190476191f && x < 0.3238095238f) {
				ratio = COMPRATIO.RATIO_1_46_TO_ONE;
			} else if (x >= 0.3238095238f && x < 0.3285714286f) {
				ratio = COMPRATIO.RATIO_1_47_TO_ONE;
			} else if (x >= 0.3285714286f && x < 0.3333333333f) {
				ratio = COMPRATIO.RATIO_1_48_TO_ONE;
			} else if (x >= 0.3333333333f && x < 0.3380952381f) {
				ratio = COMPRATIO.RATIO_1_49_TO_ONE;
			} else if (x >= 0.3380952381f && x < 0.3428571429f) {
				ratio = COMPRATIO.RATIO_1_50_TO_ONE;
			} else if (x >= 0.3428571429f && x < 0.3476190476f) {
				ratio = COMPRATIO.RATIO_1_51_TO_ONE;
			} else if (x >= 0.3476190476f && x < 0.3523809524f) {
				ratio = COMPRATIO.RATIO_1_52_TO_ONE;
			} else if (x >= 0.3523809524f && x < 0.3571428572f) {
				ratio = COMPRATIO.RATIO_1_54_TO_ONE;
			} else if (x >= 0.3571428572f && x < 0.3619047619f) {
				ratio = COMPRATIO.RATIO_1_55_TO_ONE;
			} else if (x >= 0.3619047619f && x < 0.3666666667f) {
				ratio = COMPRATIO.RATIO_1_56_TO_ONE;
			} else if (x >= 0.3666666667f && x < 0.3714285714f) {
				ratio = COMPRATIO.RATIO_1_57_TO_ONE;
			} else if (x >= 0.3714285714f && x < 0.3761904762f) {
				ratio = COMPRATIO.RATIO_1_58_TO_ONE;
			} else if (x >= 0.3761904762f && x < 0.3809523810f) {
				ratio = COMPRATIO.RATIO_1_60_TO_ONE;
			} else if (x >= 0.3809523810f && x < 0.3857142857f) {
				ratio = COMPRATIO.RATIO_1_61_TO_ONE;
			} else if (x >= 0.3857142857f && x < 0.3904761905f) {
				ratio = COMPRATIO.RATIO_1_62_TO_ONE;
			} else if (x >= 0.3904761905f && x < 0.3952380952f) {
				ratio = COMPRATIO.RATIO_1_63_TO_ONE;
			} else if (x >= 0.3952380952f && x < 0.4000000000f) {
				ratio = COMPRATIO.RATIO_1_64_TO_ONE;
			} else if (x >= 0.4000000000f && x < 0.4047619048f) {
				ratio = COMPRATIO.RATIO_1_66_TO_ONE;
			} else if (x >= 0.4047619048f && x < 0.4095238095f) {
				ratio = COMPRATIO.RATIO_1_67_TO_ONE;
			} else if (x >= 0.4095238095f && x < 0.4142857143f) {
				ratio = COMPRATIO.RATIO_1_68_TO_ONE;
			} else if (x >= 0.4142857143f && x < 0.4190476191f) {
				ratio = COMPRATIO.RATIO_1_70_TO_ONE;
			} else if (x >= 0.4190476191f && x < 0.4238095238f) {
				ratio = COMPRATIO.RATIO_1_71_TO_ONE;
			} else if (x >= 0.4238095238f && x < 0.4285714286f) {
				ratio = COMPRATIO.RATIO_1_73_TO_ONE;
			} else if (x >= 0.4285714286f && x < 0.4333333333f) {
				ratio = COMPRATIO.RATIO_1_74_TO_ONE;
			} else if (x >= 0.4333333333f && x < 0.4380952381f) {
				ratio = COMPRATIO.RATIO_1_75_TO_ONE;
			} else if (x >= 0.4380952381f && x < 0.4428571429f) {
				ratio = COMPRATIO.RATIO_1_78_TO_ONE;
			} else if (x >= 0.4428571429f && x < 0.4476190476f) {
				ratio = COMPRATIO.RATIO_1_79_TO_ONE;
			} else if (x >= 0.4476190476f && x < 0.4523809524f) {
				ratio = COMPRATIO.RATIO_1_80_TO_ONE;
			} else if (x >= 0.4523809524f && x < 0.4571428572f) {
				ratio = COMPRATIO.RATIO_1_81_TO_ONE;
			} else if (x >= 0.4571428572f && x < 0.4619047619f) {
				ratio = COMPRATIO.RATIO_1_83_TO_ONE;
			} else if (x >= 0.4619047619f && x < 0.4666666667f) {
				ratio = COMPRATIO.RATIO_1_84_TO_ONE;
			} else if (x >= 0.4666666667f && x < 0.4714285714f) {
				ratio = COMPRATIO.RATIO_1_86_TO_ONE;
			} else if (x >= 0.4714285714f && x < 0.4761904762f) {
				ratio = COMPRATIO.RATIO_1_88_TO_ONE;
			} else if (x >= 0.4761904762f && x < 0.4809523810f) {
				ratio = COMPRATIO.RATIO_1_90_TO_ONE;
			} else if (x >= 0.4809523810f && x < 0.4857142857f) {
				ratio = COMPRATIO.RATIO_1_91_TO_ONE;
			} else if (x >= 0.4857142857f && x < 0.4904761905f) {
				ratio = COMPRATIO.RATIO_1_93_TO_ONE;
			} else if (x >= 0.4904761905f && x < 0.4952380952f) {
				ratio = COMPRATIO.RATIO_1_95_TO_ONE;
			} else if (x >= 0.4952380952f && x < 0.5000000000f) {
				ratio = COMPRATIO.RATIO_1_96_TO_ONE;
			} else if (x >= 0.5000000000f && x < 0.5047619048f) {
				ratio = COMPRATIO.RATIO_1_98_TO_ONE;
			} else if (x >= 0.5047619048f && x < 0.5095238095f) {
				ratio = COMPRATIO.RATIO_2_00_TO_ONE;
			} else if (x >= 0.5095238095f && x < 0.5142857143f) {
				ratio = COMPRATIO.RATIO_2_02_TO_ONE;
			} else if (x >= 0.5142857143f && x < 0.5190476191f) {
				ratio = COMPRATIO.RATIO_2_04_TO_ONE;
			} else if (x >= 0.5190476191f && x < 0.5238095238f) {
				ratio = COMPRATIO.RATIO_2_06_TO_ONE;
			} else if (x >= 0.5238095238f && x < 0.5285714286f) {
				ratio = COMPRATIO.RATIO_2_08_TO_ONE;
			} else if (x >= 0.5285714286f && x < 0.5333333333f) {
				ratio = COMPRATIO.RATIO_2_10_TO_ONE;
			} else if (x >= 0.5333333333f && x < 0.5380952381f) {
				ratio = COMPRATIO.RATIO_2_12_TO_ONE;
			} else if (x >= 0.5380952381f && x < 0.5428571429f) {
				ratio = COMPRATIO.RATIO_2_14_TO_ONE;
			} else if (x >= 0.5428571429f && x < 0.5476190476f) {
				ratio = COMPRATIO.RATIO_2_16_TO_ONE;
			} else if (x >= 0.5476190476f && x < 0.5523809524f) {
				ratio = COMPRATIO.RATIO_2_18_TO_ONE;
			} else if (x >= 0.5523809524f && x < 0.5571428572f) {
				ratio = COMPRATIO.RATIO_2_21_TO_ONE;
			} else if (x >= 0.5571428572f && x < 0.5619047619f) {
				ratio = COMPRATIO.RATIO_2_23_TO_ONE;
			} else if (x >= 0.5619047619f && x < 0.5666666667f) {
				ratio = COMPRATIO.RATIO_2_25_TO_ONE;
			} else if (x >= 0.5666666667f && x < 0.5714285714f) {
				ratio = COMPRATIO.RATIO_2_28_TO_ONE;
			} else if (x >= 0.5714285714f && x < 0.5761904762f) {
				ratio = COMPRATIO.RATIO_2_30_TO_ONE;
			} else if (x >= 0.5761904762f && x < 0.5809523810f) {
				ratio = COMPRATIO.RATIO_2_33_TO_ONE;
			} else if (x >= 0.5809523810f && x < 0.5857142857f) {
				ratio = COMPRATIO.RATIO_2_35_TO_ONE;
			} else if (x >= 0.5857142857f && x < 0.5904761905f) {
				ratio = COMPRATIO.RATIO_2_38_TO_ONE;
			} else if (x >= 0.5904761905f && x < 0.5952380953f) {
				ratio = COMPRATIO.RATIO_2_41_TO_ONE;
			} else if (x >= 0.5952380953f && x < 0.6000000000f) {
				ratio = COMPRATIO.RATIO_2_43_TO_ONE;
			} else if (x >= 0.6000000000f && x < 0.6047619048f) {
				ratio = COMPRATIO.RATIO_2_46_TO_ONE;
			} else if (x >= 0.6047619048f && x < 0.6095238095f) {
				ratio = COMPRATIO.RATIO_2_49_TO_ONE;
			} else if (x >= 0.6095238095f && x < 0.6142857143f) {
				ratio = COMPRATIO.RATIO_2_52_TO_ONE;
			} else if (x >= 0.6142857143f && x < 0.6190476191f) {
				ratio = COMPRATIO.RATIO_2_55_TO_ONE;
			} else if (x >= 0.6190476191f && x < 0.6238095238f) {
				ratio = COMPRATIO.RATIO_2_58_TO_ONE;
			} else if (x >= 0.6238095238f && x < 0.6285714286f) {
				ratio = COMPRATIO.RATIO_2_61_TO_ONE;
			} else if (x >= 0.6285714286f && x < 0.6333333333f) {
				ratio = COMPRATIO.RATIO_2_65_TO_ONE;
			} else if (x >= 0.6333333333f && x < 0.6380952381f) {
				ratio = COMPRATIO.RATIO_2_68_TO_ONE;
			} else if (x >= 0.6380952381f && x < 0.6428571429f) {
				ratio = COMPRATIO.RATIO_2_72_TO_ONE;
			} else if (x >= 0.6428571429f && x < 0.6476190476f) {
				ratio = COMPRATIO.RATIO_2_75_TO_ONE;
			} else if (x >= 0.6476190476f && x < 0.6523809524f) {
				ratio = COMPRATIO.RATIO_2_79_TO_ONE;
			} else if (x >= 0.6523809524f && x < 0.6571428572f) {
				ratio = COMPRATIO.RATIO_2_82_TO_ONE;
			} else if (x >= 0.6571428572f && x < 0.6619047619f) {
				ratio = COMPRATIO.RATIO_2_86_TO_ONE;
			} else if (x >= 0.6619047619f && x < 0.6666666667f) {
				ratio = COMPRATIO.RATIO_2_90_TO_ONE;
			} else if (x >= 0.6666666667f && x < 0.6714285714f) {
				ratio = COMPRATIO.RATIO_2_94_TO_ONE;
			} else if (x >= 0.6714285714f && x < 0.6761904762f) {
				ratio = COMPRATIO.RATIO_2_98_TO_ONE;
			} else if (x >= 0.6761904762f && x < 0.6809523810f) {
				ratio = COMPRATIO.RATIO_3_03_TO_ONE;
			} else if (x >= 0.6809523810f && x < 0.6857142857f) {
				ratio = COMPRATIO.RATIO_3_07_TO_ONE;
			} else if (x >= 0.6857142857f && x < 0.6904761905f) {
				ratio = COMPRATIO.RATIO_3_11_TO_ONE;
			} else if (x >= 0.6904761905f && x < 0.6952380953f) {
				ratio = COMPRATIO.RATIO_3_16_TO_ONE;
			} else if (x >= 0.6952380953f && x < 0.7000000000f) {
				ratio = COMPRATIO.RATIO_3_21_TO_ONE;
			} else if (x >= 0.7000000000f && x < 0.7047619048f) {
				ratio = COMPRATIO.RATIO_3_26_TO_ONE;
			} else if (x >= 0.7047619048f && x < 0.7095238095f) {
				ratio = COMPRATIO.RATIO_3_31_TO_ONE;
			} else if (x >= 0.7095238095f && x < 0.7142857143f) {
				ratio = COMPRATIO.RATIO_3_36_TO_ONE;
			} else if (x >= 0.7142857143f && x < 0.7190476191f) {
				ratio = COMPRATIO.RATIO_3_41_TO_ONE;
			} else if (x >= 0.7190476191f && x < 0.7238095238f) {
				ratio = COMPRATIO.RATIO_3_47_TO_ONE;
			} else if (x >= 0.7238095238f && x < 0.7285714286f) {
				ratio = COMPRATIO.RATIO_3_53_TO_ONE;
			} else if (x >= 0.7285714286f && x < 0.7333333333f) {
				ratio = COMPRATIO.RATIO_3_59_TO_ONE;
			} else if (x >= 0.7333333333f && x < 0.7380952381f) {
				ratio = COMPRATIO.RATIO_3_65_TO_ONE;
			} else if (x >= 0.7380952381f && x < 0.7428571429f) {
				ratio = COMPRATIO.RATIO_3_71_TO_ONE;
			} else if (x >= 0.7428571429f && x < 0.7476190476f) {
				ratio = COMPRATIO.RATIO_3_78_TO_ONE;
			} else if (x >= 0.7476190476f && x < 0.7523809524f) {
				ratio = COMPRATIO.RATIO_3_85_TO_ONE;
			} else if (x >= 0.7523809524f && x < 0.7571428572f) {
				ratio = COMPRATIO.RATIO_3_92_TO_ONE;
			} else if (x >= 0.7571428572f && x < 0.7619047619f) {
				ratio = COMPRATIO.RATIO_3_99_TO_ONE;
			} else if (x >= 0.7619047619f && x < 0.7666666667f) {
				ratio = COMPRATIO.RATIO_4_07_TO_ONE;
			} else if (x >= 0.7666666667f && x < 0.7714285714f) {
				ratio = COMPRATIO.RATIO_4_15_TO_ONE;
			} else if (x >= 0.7714285714f && x < 0.7761904762f) {
				ratio = COMPRATIO.RATIO_4_23_TO_ONE;
			} else if (x >= 0.7761904762f && x < 0.7809523810f) {
				ratio = COMPRATIO.RATIO_4_32_TO_ONE;
			} else if (x >= 0.7809523810f && x < 0.7857142857f) {
				ratio = COMPRATIO.RATIO_4_41_TO_ONE;
			} else if (x >= 0.7857142857f && x < 0.7904761905f) {
				ratio = COMPRATIO.RATIO_4_50_TO_ONE;
			} else if (x >= 0.7904761905f && x < 0.7952380953f) {
				ratio = COMPRATIO.RATIO_4_60_TO_ONE;
			} else if (x >= 0.7952380953f && x < 0.8000000000f) {
				ratio = COMPRATIO.RATIO_4_70_TO_ONE;
			} else if (x >= 0.8000000000f && x < 0.8047619048f) {
				ratio = COMPRATIO.RATIO_4_81_TO_ONE;
			} else if (x >= 0.8047619048f && x < 0.8095238095f) {
				ratio = COMPRATIO.RATIO_4_92_TO_ONE;
			} else if (x >= 0.8095238095f && x < 0.8142857143f) {
				ratio = COMPRATIO.RATIO_5_04_TO_ONE;
			} else if (x >= 0.8142857143f && x < 0.8190476191f) {
				ratio = COMPRATIO.RATIO_5_16_TO_ONE;
			} else if (x >= 0.8190476191f && x < 0.8238095238f) {
				ratio = COMPRATIO.RATIO_5_29_TO_ONE;
			} else if (x >= 0.8238095238f && x < 0.8285714286f) {
				ratio = COMPRATIO.RATIO_5_42_TO_ONE;
			} else if (x >= 0.8285714286f && x < 0.8333333334f) {
				ratio = COMPRATIO.RATIO_5_56_TO_ONE;
			} else if (x >= 0.8333333334f && x < 0.8380952381f) {
				ratio = COMPRATIO.RATIO_5_71_TO_ONE;
			} else if (x >= 0.8380952381f && x < 0.8428571429f) {
				ratio = COMPRATIO.RATIO_5_87_TO_ONE;
			} else if (x >= 0.8428571429f && x < 0.8476190476f) {
				ratio = COMPRATIO.RATIO_6_04_TO_ONE;
			} else if (x >= 0.8476190476f && x < 0.8523809524f) {
				ratio = COMPRATIO.RATIO_6_22_TO_ONE;
			} else if (x >= 0.8523809524f && x < 0.8571428572f) {
				ratio = COMPRATIO.RATIO_6_40_TO_ONE;
			} else if (x >= 0.8571428572f && x < 0.8619047619f) {
				ratio = COMPRATIO.RATIO_6_60_TO_ONE;
			} else if (x >= 0.8619047619f && x < 0.8666666667f) {
				ratio = COMPRATIO.RATIO_6_82_TO_ONE;
			} else if (x >= 0.8666666667f && x < 0.8714285714f) {
				ratio = COMPRATIO.RATIO_7_04_TO_ONE;
			} else if (x >= 0.8714285714f && x < 0.8761904762f) {
				ratio = COMPRATIO.RATIO_7_28_TO_ONE;
			} else if (x >= 0.8761904762f && x < 0.8809523810f) {
				ratio = COMPRATIO.RATIO_7_54_TO_ONE;
			} else if (x >= 0.8809523810f && x < 0.8857142857f) {
				ratio = COMPRATIO.RATIO_7_82_TO_ONE;
			} else if (x >= 0.8857142857f && x < 0.8904761905f) {
				ratio = COMPRATIO.RATIO_8_12_TO_ONE;
			} else if (x >= 0.8904761905f && x < 0.8952380953f) {
				ratio = COMPRATIO.RATIO_8_44_TO_ONE;
			} else if (x >= 0.8952380953f && x < 0.9000000000f) {
				ratio = COMPRATIO.RATIO_8_79_TO_ONE;
			} else if (x >= 0.9000000000f && x < 0.9047619048f) {
				ratio = COMPRATIO.RATIO_9_17_TO_ONE;
			} else if (x >= 0.9047619048f && x < 0.9095238095f) {
				ratio = COMPRATIO.RATIO_9_59_TO_ONE;
			} else if (x >= 0.9095238095f && x < 0.9142857143f) {
				ratio = COMPRATIO.RATIO_10_04_TO_ONE;
			} else if (x >= 0.9142857143f && x < 0.9190476191f) {
				ratio = COMPRATIO.RATIO_10_54_TO_ONE;
			} else if (x >= 0.9190476191f && x < 0.9238095238f) {
				ratio = COMPRATIO.RATIO_11_09_TO_ONE;
			} else if (x >= 0.9238095238f && x < 0.9285714286f) {
				ratio = COMPRATIO.RATIO_11_71_TO_ONE;
			} else if (x >= 0.9285714286f && x < 0.9333333334f) {
				ratio = COMPRATIO.RATIO_12_39_TO_ONE;
			} else if (x >= 0.9333333334f && x < 0.9380952381f) {
				ratio = COMPRATIO.RATIO_13_16_TO_ONE;
			} else if (x >= 0.9380952381f && x < 0.9428571429f) {
				ratio = COMPRATIO.RATIO_14_03_TO_ONE;
			} else if (x >= 0.9428571429f && x < 0.9476190476f) {
				ratio = COMPRATIO.RATIO_15_02_TO_ONE;
			} else if (x >= 0.9476190476f && x < 0.9523809524f) {
				ratio = COMPRATIO.RATIO_16_17_TO_ONE;
			} else if (x >= 0.9523809524f && x < 0.9571428572f) {
				ratio = COMPRATIO.RATIO_17_50_TO_ONE;
			} else if (x >= 0.9571428572f && x < 0.9619047619f) {
				ratio = COMPRATIO.RATIO_19_07_TO_ONE;
			} else if (x >= 0.9619047619f && x < 0.9666666667f) {
				ratio = COMPRATIO.RATIO_20_96_TO_ONE;
			} else if (x >= 0.9666666667f && x < 0.9714285714f) {
				ratio = COMPRATIO.RATIO_23_26_TO_ONE;
			} else if (x >= 0.9714285714f && x < 0.9761904762f) {
				ratio = COMPRATIO.RATIO_26_12_TO_ONE;
			} else if (x >= 0.9761904762f && x < 0.9809523810f) {
				ratio = COMPRATIO.RATIO_29_79_TO_ONE;
			} else if (x >= 0.9809523810f && x < 0.9857142857f) {
				ratio = COMPRATIO.RATIO_34_65_TO_ONE;
			} else if (x >= 0.9857142857f && x < 0.9904761905f) {
				ratio = COMPRATIO.RATIO_41_42_TO_ONE;
			} else if (x >= 0.9904761905f && x < 0.9952380953f) {
				ratio = COMPRATIO.RATIO_51_47_TO_ONE;
			} else if (x >= 0.9952380953f && x < 1.0000000000f) {
				ratio = COMPRATIO.RATIO_67_96_TO_ONE;
			} else if (x == 1.0000000000f) {
				ratio = COMPRATIO.RATIO_100_TO_ONE;
			}
			return ratio;
		}
		
		private static string ValueToString(object val, float newMin, float newMax, string type = "") {
			float oldMin = 0.0f;
			float oldMax = 1.0f;
			
			if (val is int) {
				//int oldValue = (int) val;
				return "<TODO: int support is not implemented>";
			} else if (val is float) {
				float oldValue = (float) val;
				float newValue = MathUtils.ConvertAndMainainRatio(oldValue, oldMin, oldMax, newMin, newMax);
				if (type != "") {
					type = " " + type;
				}
				return String.Format("{0:0.0000}. Display Value: {1:0.00}{2} (Range: {3} -> {4})", oldValue, newValue, type, newMin, newMax);
			}
			return val.ToString();
		}

		private static string ValueToStringLFORate(object val, float newMin, float newMax, ONOFF lfoFree) {
			if (lfoFree == ONOFF.On) {
				// use Hz not timings
				return ValueToStringHz(val, newMin, newMax, FloatToHz.LFORateFree);
			} else {
				float oldValue = (float) val;
				LFOTIMING timing = LFOTimeFloatToEnum( oldValue );
				return String.Format("{0:0.0000}. Display Value: {1} (Range: {2} -> {3})", oldValue, timing, newMin, newMax);
			}
		}
		
		public static float ConvertSylenthFrequencyToHertz(float filterFrequency, float filterControlFrequency, Sylenth1Preset.FloatToHz mode) {
			
			if (mode != FloatToHz.FilterCutoff) {
				throw new NotImplementedException();
			}
			
			// Determine a commont filter frequency based on
			// two filter values:
			// i.e.
			// Sylenth Filter A = 590,00 Hz (0,64)
			// +
			// Sylenth FilterControl = 24,30 Hz (0,32)
			// = 1100 Hz
			// Gives Zebra Cutoff = ?
			
			// Excel:
			// =IF($J$1*EXP($K$1*[@[Both Filters]])>22028,47;22028,47;$J$1*EXP($K$1*[@[Both Filters]]))
			// J: 0,064749088	K: 10,152717
			
			double bothFilters = filterControlFrequency + filterFrequency;
			
			float actualFrequencyHertz = (float) (0.064749088 * Math.Exp(10.152717 * bothFilters));
			if (actualFrequencyHertz > 22028.47f) {
				actualFrequencyHertz = 22028.47f;
			}
			
			return actualFrequencyHertz;
		}

		public static float EnvelopePresetFileValueToMilliseconds(float oldValue) {
			
			// Could not find a proper function for the correct slope,
			// therefore I had to use three functions to get the proper slope
			// 1. An Exponential Trendline for the top
			// 2. An Sixth Order Polynomial Trendline for the middle
			// 3. An Third Order Polynomial Trendline for the bottom
			
			float newValue = 0;
			if (oldValue <= 0.1f) {
				// 3. Equation: y = (c3 * x^3) + (c2 * x^2) + (c1 * x^1) + b
				// 2326,388889	650,5952381	204,6646825	0,541666667
				newValue = (float) (
					(2326.388889 * Math.Pow(oldValue, 3))
					+ (650.5952381 * Math.Pow(oldValue, 2))
					+ (204.6646825 * oldValue)
					+ (0.541666667)
				);
			} else if (oldValue > 0.1f && oldValue < 0.51f) {
				// 2. Equation: y = (c6 * x^6) + (c5 * x^5) + (c4 * x^4) + (c3 * x^3) + (c2 * x^2) + (c1 * x^1) + b
				// -10825,28025	51052,14783	-37084,52751	17464,58482	-2556,284025	537,7264341	-12,90202926
				newValue = (float) (
					(-10825.28025 * Math.Pow(oldValue, 6))
					+ (51052.14783 * Math.Pow(oldValue, 5))
					+ (-37084.52751 * Math.Pow(oldValue, 4))
					+ (17464.58482 * Math.Pow(oldValue, 3))
					+ (-2556.284025 * Math.Pow(oldValue, 2))
					+ (537.7264341 * oldValue)
					+ (-12.90202926)
				);
			} else {
				// 1. Equation: y = c *e ^(b * x)
				// 28,18696734	6,970916827
				newValue = (float) (28.18696734 * Math.Exp(6.970916827 * oldValue));
			}
			return newValue;
		}

		public static float MillisecondsToEnvelopeValue(float oldValue) {
			throw new NotImplementedException();
		}
		
		public static float ValueToHz(float oldValue, FloatToHz mode) {
			float newValue = 0;
			if (mode == FloatToHz.DelayLowCut) {
				newValue = 3.2328f * (float) Math.Exp(9.0109f * oldValue);
			} else if (mode == FloatToHz.DelayHighCut) {
				newValue = 21120.0f * (float) Math.Exp(-5.545f * oldValue);
			} else if (mode == FloatToHz.FilterCutoff) {
				newValue = 1.0011f * (float) Math.Exp(9.9673f * oldValue);
			} else if (mode == FloatToHz.EQBassFreq) {
				newValue = 13.75f * (float) Math.Exp(4.1589f * oldValue);
			} else if (mode == FloatToHz.EQTrebleFreq) {
				newValue = 440.0f * (float) Math.Exp(4.1589f * oldValue);
			} else if (mode == FloatToHz.LFORateFree) {
				newValue = 0.0417f * (float) Math.Exp(8.4352f * oldValue);
			}
			return newValue;
		}
		
		public static string ValueToStringHz(object val, float newMin, float newMax, FloatToHz mode) {
			if (val is int) {
				return "<TODO: int support is not implemented>";
			} else if (val is float) {
				float oldValue = (float) val;
				float newValue = ValueToHz(oldValue, mode);
				return String.Format("{0:0.0000}. Display Value: {1:0.00} Hz (Range: {2} -> {3})", oldValue, newValue, newMin, newMax);
			}
			return val.ToString();
		}
		#endregion
		
		public override string ToString() {
			var buffer = new StringBuilder();

			foreach (Syl1PresetContent Content in ContentArray) {
				buffer.AppendLine("//");
				buffer.AppendLine("// Sylenth1 Preset Content");
				buffer.AppendLine("//");
				buffer.AppendLine("// Dump generated by PresetConverter, Copyleft Per Ivar Nerseth");
				buffer.AppendLine("// 2009-2017");
				buffer.AppendFormat("// Date: {0:G}\n", System.DateTime.Now);
				buffer.AppendLine("//");
				buffer.AppendFormat("// FilePath: {0}\n", FilePath);
				buffer.AppendLine("//");
				buffer.AppendFormat("// PresetName: {0}\n", Content.PresetName);
				buffer.AppendLine("//");
				
				buffer.AppendFormat("AmpEnvAAttack: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvAAttack, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvAAttack));	// index 20:23 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvADecay: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvADecay, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvADecay));		// index 24:27 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvARelease: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvARelease, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvARelease));	// index 28:31 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvASustain: {0}\n", ValueToString(Content.AmpEnvASustain, 0, 10));	// index 32:35 (value range 0 -> 10)
				buffer.AppendLine();
				buffer.AppendFormat("AmpEnvBAttack: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvBAttack, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvBAttack));	// index 36:39 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvBDecay: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvBDecay, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvBDecay));		// index 40:43 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvBRelease: {0} ({1:0.00} ms)\n", ValueToString(Content.AmpEnvBRelease, 0, 10), EnvelopePresetFileValueToMilliseconds(Content.AmpEnvBRelease));	// index 44:47 (value range 0 -> 10)
				buffer.AppendFormat("AmpEnvBSustain: {0}\n", ValueToString(Content.AmpEnvBSustain, 0, 10));	// index 48:51 (value range 0 -> 10)
				buffer.AppendLine();
				buffer.AppendFormat("ArpGate: {0}\n", ValueToString(Content.ArpGate, 0, 100, "%"));				// index 52:55 (value range 0 -> 100)
				buffer.AppendFormat("ArpMode: {0}\n", Content.ArpMode);                  					// index 56:59
				buffer.AppendFormat("ArpOctave: {0}\n", ValueToString(Content.ArpOctave, 1, 4));				// index 60:63 (value range 1 -> 4)
				buffer.AppendFormat("ArpTime: {0}\n", Content.ArpTime);                    					// index 64:67 (value range 1/1 -> 1/64)
				buffer.AppendFormat("ArpVelo: {0}\n", Content.ArpVelo);                  					// index 70:71
				buffer.AppendFormat("ArpWrap: {0}\n", ValueToString(Content.ArpWrap, 0, 16));				// index 72:75 (value range 0 -> 16)
				buffer.AppendLine();
				buffer.AppendFormat("ChorusDelay: {0}\n", ValueToString(Content.ChorusDelay, 1, 40, "ms"));		// index 76:79 (value range 1 -> 40)
				buffer.AppendFormat("ChorusDepth: {0}\n", ValueToString(Content.ChorusDepth, 0, 100, "%"));		// index 80:83 (value range 0 -> 100)
				buffer.AppendFormat("ChorusDry_Wet: {0}\n", ValueToString(Content.ChorusDry_Wet, 0, 100, "%"));	// index 84:87 (value range 0 -> 100)
				buffer.AppendFormat("ChorusFeedback: {0}\n", ValueToString(Content.ChorusFeedback, 0, 100, "%"));	// index 88:91 (value range 0 -> 100)
				buffer.AppendFormat("ChorusMode: {0}\n", Content.ChorusMode);            					// index 94:95
				// TODO: Fix Chorus Rate because the value to string method is not correct
				buffer.AppendFormat("ChorusRate: {0}\n", ValueToString(Content.ChorusRate, 0.01f, 27.5f, "Hz"));   // index 96:99 (value range 0,01 -> 27,5)
				buffer.AppendFormat("ChorusWidth: {0}\n", ValueToString(Content.ChorusWidth, 0, 100, "%"));		// index 100:103 (value range 0 -> 100)
				buffer.AppendLine();
				buffer.AppendFormat("CompAttack: {0}\n", ValueToString(Content.CompAttack, 0.1f, 300f, "ms"));      // index 104:107 (value range 0,1 -> 300)
				buffer.AppendFormat("CompRatio: {0}\n", CompRatioFloatToEnum(Content.CompRatio));                  				// index 108:111 (value range 1.00:1 -> 100.00:1)
				buffer.AppendFormat("CompRelease: {0}\n", ValueToString(Content.CompRelease, 1, 500, "ms"));		// index 112:115 (value range 1 -> 500)
				buffer.AppendFormat("CompThreshold: {0}\n", ValueToString(Content.CompThreshold, -30, 0, "dB"));	// index 116:119 (value range -30 -> 0)
				buffer.AppendLine();
				buffer.AppendFormat("DelayDry_Wet: {0}\n", ValueToString(Content.DelayDry_Wet, 0, 100, "%"));		// index 120:123 (value range 0 -> 100)
				buffer.AppendFormat("DelayFeedback: {0}\n", ValueToString(Content.DelayFeedback, 0, 100, "%"));	// index 124:127 (value range 0 -> 100)
				buffer.AppendFormat("DelayHighCut: {0}\n", ValueToStringHz(Content.DelayHighCut, 21120.0f, 82.5f, FloatToHz.DelayHighCut));	// index 128:131 (value range 82,5 -> 21120)
				buffer.AppendFormat("DelayLowCut: {0}\n", ValueToStringHz(Content.DelayLowCut, 3.23f, 26483.12f, FloatToHz.DelayLowCut));		// index 132:135 (value range 3,23 -> 26483,12)
				buffer.AppendFormat("DelayPingPong: {0}\n", Content.DelayPingPong);              			// index 138:139
				buffer.AppendFormat("DelaySmear: {0}\n", ValueToString(Content.DelaySmear, 0, 10));			// index 140:143 (value range 0 -> 10)
				buffer.AppendFormat("DelaySpread: {0}\n", ValueToString(Content.DelaySpread, 0, 100, "%"));		// index 144:147 (value range 0 -> 100)
				buffer.AppendFormat("DelayTimeLeft: {0}\n", DelayTimeFloatToEnum(Content.DelayTimeLeft));              			// index 148:151 (value range 1/64 -> 1/2)
				buffer.AppendFormat("DelayTimeRight: {0}\n", DelayTimeFloatToEnum(Content.DelayTimeRight));             			// index 152:155 (value range 1/64 -> 1/2)
				buffer.AppendFormat("DelayWidth: {0}\n", ValueToString(Content.DelayWidth, 0, 100, "%"));			// index 156:159 (value range 0 -> 100)
				buffer.AppendLine();
				buffer.AppendFormat("DistortAmount: {0}\n", ValueToString(Content.DistortAmount, 0, 10));	// index 160:163 (value range 0 -> 10)
				buffer.AppendFormat("DistortDryWet: {0}\n", ValueToString(Content.DistortDryWet, 0, 100, "%"));	// index 164:167 (value range 0 -> 100)
				buffer.AppendFormat("DistortType: {0}\n", Content.DistortType);          					// index 170:171
				buffer.AppendLine();
				buffer.AppendFormat("EQBass: {0}\n", ValueToString(Content.EQBass, 0, 15, "dB"));					// index 172:175 (value range 0 -> 15)
				buffer.AppendFormat("EQBassFreq: {0}\n", ValueToStringHz(Content.EQBassFreq, 13.75f, 880.0f, FloatToHz.EQBassFreq)); // index 176:179 (value range 13,75 -> 880)
				buffer.AppendFormat("EQTreble: {0}\n", ValueToString(Content.EQTreble, 0, 15, "dB"));				// index 180:183 (value range 0 -> 15)
				buffer.AppendFormat("EQTrebleFreq: {0}\n", ValueToStringHz(Content.EQTrebleFreq, 440, 28160, FloatToHz.EQTrebleFreq));	// index 184:187 (value range 440 -> 28160)
				buffer.AppendLine();
				buffer.AppendFormat("FilterACutoff: {0}\n", ValueToStringHz(Content.FilterACutoff, 1.0f, 21341.28f, FloatToHz.FilterCutoff));	// index 188:191 (value range 1 -> 21341,28)
				buffer.AppendFormat("FilterADrive: {0}\n", ValueToString(Content.FilterADrive, 0, 10));		// index 192:195 (value range 0 -> 10)
				buffer.AppendFormat("FilterAInput: {0}\n", Content.FilterAInput);         					// index 198:199
				buffer.AppendFormat("FilterAReso: {0}\n", ValueToString(Content.FilterAReso, 0, 10));		// index 200:203 (value range 0 -> 10)
				buffer.AppendFormat("FilterAType: {0}\n", Content.FilterAType);           					// index 204:207
				buffer.AppendFormat("FilterADB: {0}\n", Content.FilterADB);									// index 210:211 (value range 12 -> 12)
				buffer.AppendFormat("FilterA actual Hz: {0} (FilterACutoff + FilterCtrlCutoff)\n", ConvertSylenthFrequencyToHertz(Content.FilterACutoff, Content.FilterCtlCutoff, FloatToHz.FilterCutoff));
				buffer.AppendLine();
				buffer.AppendFormat("FilterBCutoff: {0}\n", ValueToStringHz(Content.FilterBCutoff, 1.0f, 21341.28f, FloatToHz.FilterCutoff));	// index 212:215 (value range 1 -> 21341,28)
				buffer.AppendFormat("FilterBDrive: {0}\n", ValueToString(Content.FilterBDrive, 0, 10));		// index 216:219 (value range 0 -> 10)
				buffer.AppendFormat("FilterBInput: {0}\n", Content.FilterBInput);         					// index 222:223
				buffer.AppendFormat("FilterBReso: {0}\n", ValueToString(Content.FilterBReso, 0, 10));		// index 224:227 (value range 0 -> 10)
				buffer.AppendFormat("FilterBType: {0}\n", Content.FilterBType);           					// index 228:231
				buffer.AppendFormat("FilterBDB: {0}\n", Content.FilterBDB);									// index 234:235 (value range 12 -> 24)
				buffer.AppendFormat("FilterB actual Hz: {0} (FilterBCutoff + FilterCtrlCutoff)\n", ConvertSylenthFrequencyToHertz(Content.FilterBCutoff, Content.FilterCtlCutoff, FloatToHz.FilterCutoff));
				buffer.AppendLine();
				buffer.AppendFormat("FilterCtlCutoff: {0}\n", ValueToStringHz(Content.FilterCtlCutoff, 1.0f, 21341.28f, FloatToHz.FilterCutoff)); 	// index 236:239 (value range 1 -> 21341,28)
				buffer.AppendFormat("FilterCtlKeyTrk: {0}\n", ValueToString(Content.FilterCtlKeyTrk, 0, 10));	// index 240:243 (value range 0 -> 10)
				buffer.AppendFormat("FilterCtlReso: {0}\n", ValueToString(Content.FilterCtlReso, 0, 10));	// index 244:247 (value range 0 -> 10)
				buffer.AppendFormat("FilterCtlWarmDrive: {0}\n", Content.FilterCtlWarmDrive);         		// index 250:251
				buffer.AppendLine();
				buffer.AppendFormat("LFO1Free: {0}\n", Content.LFO1Free);                   					// index 254:255
				buffer.AppendFormat("LFO1Gain: {0}\n", ValueToString(Content.LFO1Gain, 0, 10));				// index 256:259 (value range 0 -> 10)
				buffer.AppendFormat("LFO1Offset: {0}\n", ValueToString(Content.LFO1Offset, -10, 10));        // index 260:263 (value range -10 -> 10)
				buffer.AppendFormat("LFO1Rate: {0}\n", ValueToStringLFORate(Content.LFO1Rate, 0.04f, 192f, Content.LFO1Free));                   					// index 264:267 (value range 8/1D -> 1/256T)
				buffer.AppendFormat("LFO1Wave: {0}\n", Content.LFO1Wave);                 					// index 268:271
				buffer.AppendLine();
				buffer.AppendFormat("LFO2Free: {0}\n", Content.LFO2Free);                   					// index 274:275
				buffer.AppendFormat("LFO2Gain: {0}\n", ValueToString(Content.LFO2Gain, 0, 10));				// index 276:279 (value range 0 -> 10)
				buffer.AppendFormat("LFO2Offset: {0}\n", ValueToString(Content.LFO2Offset, -10, 10));		// index 280:283 (value range -10 -> 10)
				buffer.AppendFormat("LFO2Rate: {0}\n", ValueToStringLFORate(Content.LFO2Rate, 0.04f, 192f, Content.LFO2Free));                   					// index 284:287 (value range 8/1D -> 1/256T)
				buffer.AppendFormat("LFO2Wave: {0}\n", Content.LFO2Wave);                 					// index 288:291
				buffer.AppendLine();
				buffer.AppendFormat("MainVolume: {0}\n", ValueToString(Content.MainVolume, 0, 10));			// index 292:295 (value range 0 -> 10)
				buffer.AppendFormat("MixA: {0}\n", ValueToString(Content.MixA, 0, 10));						// index 296:299 (value range 0 -> 10)
				buffer.AppendFormat("MixB: {0}\n", ValueToString(Content.MixB, 0, 10));						// index 300:303 (value range 0 -> 10)
				buffer.AppendLine();
				buffer.AppendFormat("ModEnv1Attack: {0}\n", ValueToString(Content.ModEnv1Attack, 0, 10));	// index 304:307 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv1Decay: {0}\n", ValueToString(Content.ModEnv1Decay, 0, 10));		// index 308:311 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv1Release: {0}\n", ValueToString(Content.ModEnv1Release, 0, 10));	// index 312:315 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv1Sustain: {0}\n", ValueToString(Content.ModEnv1Sustain, 0, 10));	// index 316:319 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv2Attack: {0}\n", ValueToString(Content.ModEnv2Attack, 0, 10));	// index 320:323 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv2Decay: {0}\n", ValueToString(Content.ModEnv2Decay, 0, 10));		// index 324:327 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv2Release: {0}\n", ValueToString(Content.ModEnv2Release, 0, 10));	// index 328:331 (value range 0 -> 10)
				buffer.AppendFormat("ModEnv2Sustain: {0}\n", ValueToString(Content.ModEnv2Sustain, 0, 10));	// index 332:335 (value range 0 -> 10)
				buffer.AppendFormat("ModWheel: {0}\n", ValueToString(Content.ModWheel, 0, 10));				// index 336:339 (value range 0 -> 10)
				buffer.AppendLine();
				buffer.AppendFormat("MonoLegato: {0}\n", Content.MonoLegato);                 				// index 342:343
				buffer.AppendLine();
				buffer.AppendFormat("OscA1Detune: {0}\n", ValueToString(Content.OscA1Detune, 0, 10));		// index 344:347 (value range 0 -> 10)
				buffer.AppendFormat("OscA1Fine: {0}\n", ValueToString(Content.OscA1Fine, -1, 1));            // index 348:351 (value range -1 -> 1)
				buffer.AppendFormat("OscA1Invert: {0}\n", Content.OscA1Invert);                				// index 354:355
				buffer.AppendFormat("OscA1Note: {0}\n", ValueToString(Content.OscA1Note, -7, 7));           	// index 356:359 (value range -7 -> 7)
				buffer.AppendFormat("OscA1Octave: {0}\n", ValueToString(Content.OscA1Octave, -3, 3));        // index 360:363 (value range -3 -> 3)
				buffer.AppendFormat("OscA1Pan: {0}\n", ValueToString(Content.OscA1Pan, -10, 10));            // index 364:367 (value range -10 -> 10)
				buffer.AppendFormat("OscA1Phase: {0}\n", ValueToString(Content.OscA1Phase, 0, 360, "deg"));         // index 368:371 (value range 0 -> 360)
				buffer.AppendFormat("OscA1Retrig: {0}\n", Content.OscA1Retrig);                				// index 374:375
				buffer.AppendFormat("OscA1Stereo: {0}\n", ValueToString(Content.OscA1Stereo, 0, 10));		// index 376:379 (value range 0 -> 10)
				buffer.AppendFormat("OscA1Voices: {0}\n", ValueToString(Content.OscA1Voices, 0, 8));			// index 382:383 (value range 0 -> 8)
				buffer.AppendFormat("OscA1Volume: {0}\n", ValueToString(Content.OscA1Volume, 0, 10));		// index 384:387 (value range 0 -> 10)
				buffer.AppendFormat("OscA1Wave: {0}\n", Content.OscA1Wave);                					// index 388:391
				buffer.AppendLine();
				buffer.AppendFormat("OscA2Detune: {0}\n", ValueToString(Content.OscA2Detune, 0, 10));		// index 392:395 (value range 0 -> 10)
				buffer.AppendFormat("OscA2Fine: {0}\n", ValueToString(Content.OscA2Fine, -1, 1));            // index 396:399 (value range -1 -> 1)
				buffer.AppendFormat("OscA2Invert: {0}\n", Content.OscA2Invert);               				// index 402:403
				buffer.AppendFormat("OscA2Note: {0}\n", ValueToString(Content.OscA2Note, -7, 7));            // index 404:407 (value range -7 -> 7)
				buffer.AppendFormat("OscA2Octave: {0}\n", ValueToString(Content.OscA2Octave, -3, 3));        // index 408:411 (value range -3 -> 3)
				buffer.AppendFormat("OscA2Pan: {0}\n", ValueToString(Content.OscA2Pan, -10, 10));            // index 412:415 (value range -10 -> 10)
				buffer.AppendFormat("OscA2Phase: {0}\n", ValueToString(Content.OscA2Phase, 0, 360, "deg"));			// index 416:419 (value range 0 -> 360)
				buffer.AppendFormat("OscA2Retrig: {0}\n", Content.OscA2Retrig);                				// index 422:423
				buffer.AppendFormat("OscA2Stereo: {0}\n", ValueToString(Content.OscA2Stereo, 0, 10));		// index 424:427 (value range 0 -> 10)
				buffer.AppendFormat("OscA2Voices: {0}\n", ValueToString(Content.OscA2Voices, 0, 8));			// index 430:431 (value range 0 -> 8)
				buffer.AppendFormat("OscA2Volume: {0}\n", ValueToString(Content.OscA2Volume, 0, 10));		// index 432:435 (value range 0 -> 10)
				buffer.AppendFormat("OscA2Wave: {0}\n", Content.OscA2Wave);                					// index 436:439
				buffer.AppendLine();
				buffer.AppendFormat("OscB1Detune: {0}\n", ValueToString(Content.OscB1Detune, 0, 10));		// index 440:443 (value range 0 -> 10)
				buffer.AppendFormat("OscB1Fine: {0}\n", ValueToString(Content.OscB1Fine, -1, 1));            // index 444:447 (value range -1 -> 1)
				buffer.AppendFormat("OscB1Invert: {0}\n", Content.OscB1Invert);                				// index 450:451
				buffer.AppendFormat("OscB1Note: {0}\n", ValueToString(Content.OscB1Note, -7, 7));            // index 452:455 (value range -7 -> 7)
				buffer.AppendFormat("OscB1Octave: {0}\n", ValueToString(Content.OscB1Octave, -3, 3));        // index 456:459 (value range -3 -> 3)
				buffer.AppendFormat("OscB1Pan: {0}\n", ValueToString(Content.OscB1Pan, -10, 10));            // index 460:463 (value range -10 -> 10)
				buffer.AppendFormat("OscB1Phase: {0}\n", ValueToString(Content.OscB1Phase, 0, 360, "deg"));			// index 464:467 (value range 0 -> 360)
				buffer.AppendFormat("OscB1Retrig: {0}\n", Content.OscB1Retrig);                				// index 470:471
				buffer.AppendFormat("OscB1Stereo: {0}\n", ValueToString(Content.OscB1Stereo, 0, 10));		// index 472:475 (value range 0 -> 10)
				buffer.AppendFormat("OscB1Voices: {0}\n", ValueToString(Content.OscB1Voices, 0, 8));			// index 478:479 (value range 0 -> 8)
				buffer.AppendFormat("OscB1Volume: {0}\n", ValueToString(Content.OscB1Volume, 0, 10));		// index 480:483 (value range 0 -> 10)
				buffer.AppendFormat("OscB1Wave: {0}\n", Content.OscB1Wave);                					// index 484:487
				buffer.AppendLine();
				buffer.AppendFormat("OscB2Detune: {0}\n", ValueToString(Content.OscB2Detune, 0, 10));		// index 488:491 (value range 0 -> 10)
				buffer.AppendFormat("OscB2Fine: {0}\n", ValueToString(Content.OscB2Fine, -1, 1));           	// index 492:495 (value range -1 -> 1)
				buffer.AppendFormat("OscB2Invert: {0}\n", Content.OscB2Invert);                				// index 498:499
				buffer.AppendFormat("OscB2Note: {0}\n", ValueToString(Content.OscB2Note, -7, 7));            // index 500:503 (value range -7 -> 7)
				buffer.AppendFormat("OscB2Octave: {0}\n", ValueToString(Content.OscB2Octave, -3, 3));        // index 504:507 (value range -3 -> 3)
				buffer.AppendFormat("OscB2Pan: {0}\n", ValueToString(Content.OscB2Pan, -10, 10));            // index 508:511 (value range -10 -> 10)
				buffer.AppendFormat("OscB2Phase: {0}\n", ValueToString(Content.OscB2Phase, 0, 360, "deg"));			// index 512:515 (value range 0 -> 360)
				buffer.AppendFormat("OscB2Retrig: {0}\n", Content.OscB2Retrig);                				// index 518:519
				buffer.AppendFormat("OscB2Stereo: {0}\n", ValueToString(Content.OscB2Stereo, 0, 10));		// index 520:523 (value range 0 -> 10)
				buffer.AppendFormat("OscB2Voices: {0}\n", ValueToString(Content.OscB2Voices, 0, 8));			// index 526:527 (value range 0 -> 8)
				buffer.AppendFormat("OscB2Volume: {0}\n", ValueToString(Content.OscB2Volume, 0, 10));		// index 528:531 (value range 0 -> 10)
				buffer.AppendFormat("OscB2Wave: {0}\n", Content.OscB2Wave);                					// index 532:535
				buffer.AppendLine();
				buffer.AppendFormat("PhaserCenterFreq: {0}\n", ValueToString(Content.PhaserCenterFreq, 0, 10));	// index 536:539 (value range 0 -> 10)
				buffer.AppendFormat("PhaserDry_Wet: {0}\n", ValueToString(Content.PhaserDry_Wet, 0, 100, "%"));	// index 540:543 (value range 0 -> 100)
				buffer.AppendFormat("PhaserFeedback: {0}\n", ValueToString(Content.PhaserFeedback, 0, 100, "%"));	// index 544:547 (value range 0 -> 100)
				buffer.AppendFormat("PhaserLFOGain: {0}\n", ValueToString(Content.PhaserLFOGain, 0, 10));	// index 548:551 (value range 0 -> 10)
				buffer.AppendFormat("PhaserLFORate: {0}\n", Content.PhaserLFORate);              			// index 552:555 (value range 8/1D -> 1/256T))
				buffer.AppendFormat("PhaserLROffset: {0}\n", ValueToString(Content.PhaserLROffset, -10, 10));// index 556:559 (value range -10 -> 10)
				buffer.AppendFormat("PhaserSpread: {0}\n", ValueToString(Content.PhaserSpread, 0, 10));		// index 560:563 (value range 0 -> 10)
				buffer.AppendFormat("PhaserWidth: {0}\n", ValueToString(Content.PhaserWidth, 0, 100, "%"));		// index 564:567 (value range 0 -> 100)
				buffer.AppendLine();
				buffer.AppendFormat("PitchBend: {0}\n", ValueToString(Content.PitchBend, -10, 10));          // index 568:571 (value range -10 -> 10)
				buffer.AppendFormat("PitchBendRange: {0}\n", ValueToString(Content.PitchBendRange, 1, 24));	// index 572:575 (value range 1 -> 24)
				buffer.AppendLine();
				buffer.AppendFormat("Polyphony: {0}\n", ValueToString(Content.Polyphony, 0, 16));			// index 578:579 (value range 0 -> 16)
				buffer.AppendFormat("PortaMode: {0}\n", Content.PortaMode);              					// index 582:583
				buffer.AppendFormat("PortaTime: {0}\n", ValueToString(Content.PortaTime, 0, 10));			// index 584:587 (value range 0 -> 10)
				buffer.AppendLine();
				buffer.AppendFormat("ReverbDamp: {0}\n", ValueToString(Content.ReverbDamp, 0, 10));			// index 588:591 (value range 0 -> 10)
				buffer.AppendFormat("ReverbDry_Wet: {0}\n", ValueToString(Content.ReverbDry_Wet, 0, 100, "%"));	// index 592:595 (value range 0 -> 100)
				buffer.AppendFormat("ReverbPredelay: {0}\n", ValueToString(Content.ReverbPredelay, 0, 200, "ms"));	// index 596:599 (value range 0 -> 200)
				buffer.AppendFormat("ReverbSize: {0}\n", ValueToString(Content.ReverbSize, 0, 10));			// index 600:603 (value range 0 -> 10)
				buffer.AppendFormat("ReverbWidth: {0}\n", ValueToString(Content.ReverbWidth, 0, 100, "%"));		// index 604:607 (value range 0 -> 100)
				buffer.AppendLine();
				buffer.AppendFormat("Solo: {0}\n", Content.Solo);                       		// index 610:611
				buffer.AppendFormat("Sync: {0}\n", Content.Sync);                       		// index 614:615
				buffer.AppendLine();
				buffer.AppendFormat("XArpHold01: {0}\n", Content.XArpHold01);                 // index 618:619
				buffer.AppendFormat("XArpHold02: {0}\n", Content.XArpHold02);                 // index 622:623
				buffer.AppendFormat("XArpHold03: {0}\n", Content.XArpHold03);                 // index 626:627
				buffer.AppendFormat("XArpHold04: {0}\n", Content.XArpHold04);                 // index 630:631
				buffer.AppendFormat("XArpHold05: {0}\n", Content.XArpHold05);                 // index 634:635
				buffer.AppendFormat("XArpHold06: {0}\n", Content.XArpHold06);                 // index 638:639
				buffer.AppendFormat("XArpHold07: {0}\n", Content.XArpHold07);                 // index 642:643
				buffer.AppendFormat("XArpHold08: {0}\n", Content.XArpHold08);                 // index 646:647
				buffer.AppendFormat("XArpHold09: {0}\n", Content.XArpHold09);                 // index 650:651
				buffer.AppendFormat("XArpHold10: {0}\n", Content.XArpHold10);                 // index 654:655
				buffer.AppendFormat("XArpHold11: {0}\n", Content.XArpHold11);                 // index 658:659
				buffer.AppendFormat("XArpHold12: {0}\n", Content.XArpHold12);                 // index 662:663
				buffer.AppendFormat("XArpHold13: {0}\n", Content.XArpHold13);                 // index 666:667
				buffer.AppendFormat("XArpHold14: {0}\n", Content.XArpHold14);                 // index 670:671
				buffer.AppendFormat("XArpHold15: {0}\n", Content.XArpHold15);                 // index 674:675
				buffer.AppendFormat("XArpHold16: {0}\n", Content.XArpHold16);                 // index 678:679
				buffer.AppendFormat("XArpTransp01: {0}\n", ValueToString(Content.XArpTransp01, -24, 24));  // index 680:684 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp02: {0}\n", ValueToString(Content.XArpTransp02, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp03: {0}\n", ValueToString(Content.XArpTransp03, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp04: {0}\n", ValueToString(Content.XArpTransp04, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp05: {0}\n", ValueToString(Content.XArpTransp05, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp06: {0}\n", ValueToString(Content.XArpTransp06, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp07: {0}\n", ValueToString(Content.XArpTransp07, -24, 24));  // (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp08: {0}\n", ValueToString(Content.XArpTransp08, -24, 24));  // (value range -24 -> 14)
				buffer.AppendFormat("XArpTransp09: {0}\n", ValueToString(Content.XArpTransp09, -24, 24));  // index 712:715 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp10: {0}\n", ValueToString(Content.XArpTransp10, -24, 24));  // index 716:719 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp11: {0}\n", ValueToString(Content.XArpTransp11, -24, 24));  // index 720:723 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp12: {0}\n", ValueToString(Content.XArpTransp12, -24, 24));  // index 724:727 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp13: {0}\n", ValueToString(Content.XArpTransp13, -24, 24));  // index 728:731 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp14: {0}\n", ValueToString(Content.XArpTransp14, -24, 24));  // index 732:735 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp15: {0}\n", ValueToString(Content.XArpTransp15, -24, 24));  // index 736:739 (value range -24 -> 24)
				buffer.AppendFormat("XArpTransp16: {0}\n", ValueToString(Content.XArpTransp16, -24, 24));  // index 740:743 (value range -24 -> 14)
				buffer.AppendFormat("XArpVelo01: {0}\n", ValueToString(Content.XArpVelo01, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo02: {0}\n", ValueToString(Content.XArpVelo02, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo03: {0}\n", ValueToString(Content.XArpVelo03, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo04: {0}\n", ValueToString(Content.XArpVelo04, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo05: {0}\n", ValueToString(Content.XArpVelo05, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo06: {0}\n", ValueToString(Content.XArpVelo06, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo07: {0}\n", ValueToString(Content.XArpVelo07, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo08: {0}\n", ValueToString(Content.XArpVelo08, 0, 127));	// (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo09: {0}\n", ValueToString(Content.XArpVelo09, 0, 127));	// index 776:779 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo10: {0}\n", ValueToString(Content.XArpVelo10, 0, 127));	// index 780:783 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo11: {0}\n", ValueToString(Content.XArpVelo11, 0, 127));	// index 784:787 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo12: {0}\n", ValueToString(Content.XArpVelo12, 0, 127));	// index 788:791 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo13: {0}\n", ValueToString(Content.XArpVelo13, 0, 127));	// index 792:795 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo14: {0}\n", ValueToString(Content.XArpVelo14, 0, 127));	// index 796:799 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo15: {0}\n", ValueToString(Content.XArpVelo15, 0, 127));	// index 800:803 (value range 0 -> 127)
				buffer.AppendFormat("XArpVelo16: {0}\n", ValueToString(Content.XArpVelo16, 0, 127));	// index 804:807 (value range 0 -> 127)
				buffer.AppendFormat("XModEnv1Dest1Am: {0}\n", ValueToString(Content.XModEnv1Dest1Am, -10, 10));           	// index 808:811 (value range -10 -> 10)
				buffer.AppendFormat("XModEnv1Dest2Am: {0}\n", ValueToString(Content.XModEnv1Dest2Am, -10, 10));            	// index 812:815 (value range -10 -> 10)
				buffer.AppendFormat("XModEnv2Dest1Am: {0}\n", ValueToString(Content.XModEnv2Dest1Am, -10, 10));            	// index 816:819 (value range -10 -> 10)
				buffer.AppendFormat("XModEnv2Dest2Am: {0}\n", ValueToString(Content.XModEnv2Dest2Am, -10, 10));            	// index 820:823 (value range -10 -> 10)
				buffer.AppendFormat("XModLFO1Dest1Am: {0}\n", ValueToString(Content.XModLFO1Dest1Am, -10, 10));            	// index 824:827 (value range -10 -> 10)
				buffer.AppendFormat("XModLFO1Dest2Am: {0}\n", ValueToString(Content.XModLFO1Dest2Am, -10, 10));            	// index 828:831 (value range -10 -> 10)
				buffer.AppendFormat("XModLFO2Dest1Am: {0}\n", ValueToString(Content.XModLFO2Dest1Am, -10, 10));            	// index 832:835 (value range -10 -> 10)
				buffer.AppendFormat("XModLFO2Dest2Am: {0}\n", ValueToString(Content.XModLFO2Dest2Am, -10, 10));            	// index 836:839 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc1ADest1Am: {0}\n", ValueToString(Content.XModMisc1ADest1Am, -10, 10));        	// index 840:843 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc1ADest2Am: {0}\n", ValueToString(Content.XModMisc1ADest2Am, -10, 10));          // index 844:847 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc1ASource: {0}\n", Content.XModMisc1ASource);      // index 848:851
				buffer.AppendFormat("XModMisc1BDest1Am: {0}\n", ValueToString(Content.XModMisc1BDest1Am, -10, 10));          // index 852:855 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc1BDest2Am: {0}\n", ValueToString(Content.XModMisc1BDest2Am, -10, 10));          // index 856:859 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc1BSource: {0}\n", Content.XModMisc1BSource);      // index 860:863
				buffer.AppendFormat("XModMisc2ADest1Am: {0}\n", ValueToString(Content.XModMisc2ADest1Am, -10, 10));          // index 864:867 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc2ADest2Am: {0}\n", ValueToString(Content.XModMisc2ADest2Am, -10, 10));          // index 868:871 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc2ASource: {0}\n", Content.XModMisc2ASource);      // index 872:875
				buffer.AppendFormat("XModMisc2BDest1Am: {0}\n", ValueToString(Content.XModMisc2BDest1Am, -10, 10));          // index 876:879 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc2BDest2Am: {0}\n", ValueToString(Content.XModMisc2BDest2Am, -10, 10));          // index 880:883 (value range -10 -> 10)
				buffer.AppendFormat("XModMisc2BSource: {0}\n", Content.XModMisc2BSource);      // index 884:887
				buffer.AppendFormat("XSwArpOnOff: {0}\n", Content.XSwArpOnOff);                // index 890:891
				buffer.AppendFormat("XSwChorusOnOff: {0}\n", Content.XSwChorusOnOff);             // index 894:895
				buffer.AppendFormat("XSwCompOnOff: {0}\n", Content.XSwCompOnOff);               // index 898:899
				buffer.AppendFormat("XSwDelayOnOff: {0}\n", Content.XSwDelayOnOff);              // index 902:903
				buffer.AppendFormat("XSwDistOnOff: {0}\n", Content.XSwDistOnOff);               // index 906:907
				buffer.AppendFormat("XSwEQOnOff: {0}\n", Content.XSwEQOnOff);                 // index 910:911
				buffer.AppendFormat("XSwPhaserOnOff: {0}\n", Content.XSwPhaserOnOff);             // index 914:915
				buffer.AppendFormat("XSwReverbOnOff: {0}\n", Content.XSwReverbOnOff);             // index 918:919
				buffer.AppendFormat("YModEnv1Dest1: {0}\n", Content.YModEnv1Dest1);           // index 922:923
				buffer.AppendFormat("YModEnv1Dest2: {0}\n", Content.YModEnv1Dest2);           // index 926:927
				buffer.AppendFormat("YModEnv2Dest1: {0}\n", Content.YModEnv2Dest1);           // index 930:931
				buffer.AppendFormat("YModEnv2Dest2: {0}\n", Content.YModEnv2Dest2);           // index 934:935
				buffer.AppendFormat("YModLFO1Dest1: {0}\n", Content.YModLFO1Dest1);          // index 939:939
				buffer.AppendFormat("YModLFO1Dest2: {0}\n", Content.YModLFO1Dest2);          // index 942:943
				buffer.AppendFormat("YModLFO2Dest1: {0}\n", Content.YModLFO2Dest1);          // index 946:947
				buffer.AppendFormat("YModLFO2Dest2: {0}\n", Content.YModLFO2Dest2);          // index 950:951
				buffer.AppendFormat("YModMisc1ADest1: {0}\n", Content.YModMisc1ADest1);        // index 954:955
				buffer.AppendFormat("YModMisc1ADest2: {0}\n", Content.YModMisc1ADest2);        // index 958:959
				buffer.AppendFormat("YModMisc1BDest1: {0}\n", Content.YModMisc1BDest1);        // index 962:963
				buffer.AppendFormat("YModMisc1BDest2: {0}\n", Content.YModMisc1BDest2);        // index 966:967
				buffer.AppendFormat("YModMisc2ADest1: {0}\n", Content.YModMisc2ADest1);        // index 970:971
				buffer.AppendFormat("YModMisc2ADest2: {0}\n", Content.YModMisc2ADest2);        // index 974:975
				buffer.AppendFormat("YModMisc2BDest1: {0}\n", Content.YModMisc2BDest1);        // index 978:979
				buffer.AppendFormat("YModMisc2BDest2: {0}\n", Content.YModMisc2BDest2);        // index 982:983
				buffer.AppendFormat("YPartSelect: {0}\n", Content.YPartSelect);         // index 986:987
				buffer.AppendFormat("ZEQMode: {0}\n", Content.ZEQMode);                 // index 990:991
				buffer.AppendLine();
			}
			return buffer.ToString();
		}
		
		public class Syl1PresetContent {
			public string PresetName { set; get; }

			public float AmpEnvAAttack;              // index 20:23 (value range 0 -> 10)
			public float AmpEnvADecay;               // index 24:27 (value range 0 -> 10)
			public float AmpEnvARelease;             // index 28:31 (value range 0 -> 10)
			public float AmpEnvASustain;             // index 32:35 (value range 0 -> 10)
			public float AmpEnvBAttack;              // index 36:39 (value range 0 -> 10)
			public float AmpEnvBDecay;               // index 40:43 (value range 0 -> 10)
			public float AmpEnvBRelease;             // index 44:47 (value range 0 -> 10)
			public float AmpEnvBSustain;             // index 48:51 (value range 0 -> 10)
			public float ArpGate;                    // index 52:55 (value range 0 -> 100)
			public ARPMODE ArpMode;                  // index 56:59
			public ARPOCTAVE ArpOctave;              // index 60:63 (value range 1 -> 4)
			public float ArpTime;                    // index 64:67 (value range 1/1 -> 1/64)
			public ARPVELO ArpVelo;                  // index 70:71
			public ARPWRAP ArpWrap;                  // index 72:75 (value range 0 -> 16)
			public float ChorusDelay;                // index 76:79 (value range 1 -> 40)
			public float ChorusDepth;                // index 80:83 (value range 0 -> 100)
			public float ChorusDry_Wet;              // index 84:87 (value range 0 -> 100)
			public float ChorusFeedback;             // index 88:91 (value range 0 -> 100)
			public CHORUSMODE ChorusMode;            // index 94:95
			public float ChorusRate;                 // index 96:99 (value range 0,01 -> 27,5)
			public float ChorusWidth;                // index 100:103 (value range 0 -> 100)
			public float CompAttack;                 // index 104:107 (value range 0,1 -> 300)
			public float CompRatio;                  // index 108:111 (value range 1.00:1 -> 100.00:1)
			public float CompRelease;                // index 112:115 (value range 1 -> 500)
			public float CompThreshold;              // index 116:119 (value range -30 -> 0)
			public float DelayDry_Wet;               // index 120:123 (value range 0 -> 100)
			public float DelayFeedback;              // index 124:127 (value range 0 -> 100)
			public float DelayHighCut;               // index 128:131 (value range 82,5 -> 21120)
			public float DelayLowCut;                // index 132:135 (value range 3,23 -> 26483,12)
			public ONOFF DelayPingPong;              // index 138:139
			public float DelaySmear;                 // index 140:143 (value range 0 -> 10)
			public float DelaySpread;                // index 144:147 (value range 0 -> 100)
			public float DelayTimeLeft;              // index 148:151 (value range 1/64 -> 1/2)
			public float DelayTimeRight;             // index 152:155 (value range 1/64 -> 1/2)
			public float DelayWidth;                 // index 156:159 (value range 0 -> 100)
			public float DistortAmount;              // index 160:163 (value range 0 -> 10)
			public float DistortDryWet;              // index 164:167 (value range 0 -> 100)
			public DISTORTTYPE DistortType;          // index 170:171
			public float EQBass;                     // index 172:175 (value range 0 -> 15)
			public float EQBassFreq;                 // index 176:179 (value range 13,75 -> 880)
			public float EQTreble;                   // index 180:183 (value range 0 -> 15)
			public float EQTrebleFreq;               // index 184:187 (value range 440 -> 28160)
			public float FilterACutoff;              // index 188:191 (value range 1 -> 21341,28)
			public float FilterADrive;               // index 192:195 (value range 0 -> 10)
			public FILTERAINPUT FilterAInput;        // index 198:199
			public float FilterAReso;                // index 200:203 (value range 0 -> 10)
			public FILTERTYPE FilterAType;           // index 204:207
			public FILTERDB FilterADB;               // index 210:211 (value range 12 -> 12)
			public float FilterBCutoff;              // index 212:215 (value range 1 -> 21341,28)
			public float FilterBDrive;               // index 216:219 (value range 0 -> 10)
			public FILTERBINPUT FilterBInput;        // index 222:223
			public float FilterBReso;                // index 224:227 (value range 0 -> 10)
			public FILTERTYPE FilterBType;           // index 228:231
			public FILTERDB FilterBDB;               // index 234:235 (value range 12 -> 24)
			public float FilterCtlCutoff;            // index 236:239 (value range 1 -> 21341,28)
			public float FilterCtlKeyTrk;            // index 240:243 (value range 0 -> 10)
			public float FilterCtlReso;              // index 244:247 (value range 0 -> 10)
			public ONOFF FilterCtlWarmDrive;         // index 250:251
			public ONOFF LFO1Free;                   // index 254:255
			public float LFO1Gain;                   // index 256:259 (value range 0 -> 10)
			public float LFO1Offset;                 // index 260:263 (value range -10 -> 10)
			public float LFO1Rate;                   // index 264:267 (value range 8/1D -> 1/256T)
			public LFOWAVE LFO1Wave;                 // index 268:271
			public ONOFF LFO2Free;                   // index 274:275
			public float LFO2Gain;                   // index 276:279 (value range 0 -> 10)
			public float LFO2Offset;                 // index 280:283 (value range -10 -> 10)
			public float LFO2Rate;                   // index 284:287 (value range 8/1D -> 1/256T)
			public LFOWAVE LFO2Wave;                 // index 288:291
			public float MainVolume;                 // index 292:295 (value range 0 -> 10)
			public float MixA;                       // index 296:299 (value range 0 -> 10)
			public float MixB;                       // index 300:303 (value range 0 -> 10)
			public float ModEnv1Attack;              // index 304:307 (value range 0 -> 10)
			public float ModEnv1Decay;               // index 308:311 (value range 0 -> 10)
			public float ModEnv1Release;             // index 312:315 (value range 0 -> 10)
			public float ModEnv1Sustain;             // index 316:319 (value range 0 -> 10)
			public float ModEnv2Attack;              // index 320:323 (value range 0 -> 10)
			public float ModEnv2Decay;               // index 324:327 (value range 0 -> 10)
			public float ModEnv2Release;             // index 328:331 (value range 0 -> 10)
			public float ModEnv2Sustain;             // index 332:335 (value range 0 -> 10)
			public float ModWheel;                   // index 336:339 (value range 0 -> 10)
			public ONOFF MonoLegato;                 // index 342:343
			public float OscA1Detune;                // index 344:347 (value range 0 -> 10)
			public float OscA1Fine;                  // index 348:351 (value range -1 -> 1)
			public ONOFF OscA1Invert;                // index 354:355
			public float OscA1Note;                  // index 356:359 (value range -7 -> 7)
			public float OscA1Octave;                // index 360:363 (value range -3 -> 3)
			public float OscA1Pan;                   // index 364:367 (value range -10 -> 10)
			public float OscA1Phase;                 // index 368:371 (value range 0 -> 360)
			public ONOFF OscA1Retrig;                // index 374:375
			public float OscA1Stereo;                // index 376:379 (value range 0 -> 10)
			public VOICES OscA1Voices;                // index 382:383 (value range 0 -> 8)
			public float OscA1Volume;                // index 384:387 (value range 0 -> 10)
			public OSCWAVE OscA1Wave;                // index 388:391
			public float OscA2Detune;                // index 392:395 (value range 0 -> 10)
			public float OscA2Fine;                  // index 396:399 (value range -1 -> 1)
			public ONOFF OscA2Invert;                // index 402:403
			public float OscA2Note;                  // index 404:407 (value range -7 -> 7)
			public float OscA2Octave;                // index 408:411 (value range -3 -> 3)
			public float OscA2Pan;                   // index 412:415 (value range -10 -> 10)
			public float OscA2Phase;                 // index 416:419 (value range 0 -> 360)
			public ONOFF OscA2Retrig;                // index 422:423
			public float OscA2Stereo;                // index 424:427 (value range 0 -> 10)
			public VOICES OscA2Voices;                // index 430:431 (value range 0 -> 8)
			public float OscA2Volume;                // index 432:435 (value range 0 -> 10)
			public OSCWAVE OscA2Wave;                // index 436:439
			public float OscB1Detune;                // index 440:443 (value range 0 -> 10)
			public float OscB1Fine;                  // index 444:447 (value range -1 -> 1)
			public ONOFF OscB1Invert;                // index 450:451
			public float OscB1Note;                  // index 452:455 (value range -7 -> 7)
			public float OscB1Octave;                // index 456:459 (value range -3 -> 3)
			public float OscB1Pan;                   // index 460:463 (value range -10 -> 10)
			public float OscB1Phase;                 // index 464:467 (value range 0 -> 360)
			public ONOFF OscB1Retrig;                // index 470:471
			public float OscB1Stereo;                // index 472:475 (value range 0 -> 10)
			public VOICES OscB1Voices;                // index 478:479 (value range 0 -> 8)
			public float OscB1Volume;                // index 480:483 (value range 0 -> 10)
			public OSCWAVE OscB1Wave;                // index 484:487
			public float OscB2Detune;                // index 488:491 (value range 0 -> 10)
			public float OscB2Fine;                  // index 492:495 (value range -1 -> 1)
			public ONOFF OscB2Invert;                // index 498:499
			public float OscB2Note;                  // index 500:503 (value range -7 -> 7)
			public float OscB2Octave;                // index 504:507 (value range -3 -> 3)
			public float OscB2Pan;                   // index 508:511 (value range -10 -> 10)
			public float OscB2Phase;                 // index 512:515 (value range 0 -> 360)
			public ONOFF OscB2Retrig;                // index 518:519
			public float OscB2Stereo;                // index 520:523 (value range 0 -> 10)
			public VOICES OscB2Voices;                // index 526:527 (value range 0 -> 8)
			public float OscB2Volume;                // index 528:531 (value range 0 -> 10)
			public OSCWAVE OscB2Wave;                // index 532:535
			public float PhaserCenterFreq;           // index 536:539 (value range 0 -> 10)
			public float PhaserDry_Wet;              // index 540:543 (value range 0 -> 100)
			public float PhaserFeedback;             // index 544:547 (value range 0 -> 100)
			public float PhaserLFOGain;              // index 548:551 (value range 0 -> 10)
			public float PhaserLFORate;              // index 552:555 (value range 8/1D -> 1/256T))
			public float PhaserLROffset;             // index 556:559 (value range -10 -> 10)
			public float PhaserSpread;               // index 560:563 (value range 0 -> 10)
			public float PhaserWidth;                // index 564:567 (value range 0 -> 100)
			public float PitchBend;                  // index 568:571 (value range -10 -> 10)
			public float PitchBendRange;             // index 572:575 (value range 1 -> 24)
			public float Polyphony;                  // index 578:579 (value range 0 -> 16)
			public PORTAMODE PortaMode;              // index 582:583
			public float PortaTime;                  // index 584:587 (value range 0 -> 10)
			public float ReverbDamp;                 // index 588:591 (value range 0 -> 10)
			public float ReverbDry_Wet;              // index 592:595 (value range 0 -> 100)
			public float ReverbPredelay;             // index 596:599 (value range 0 -> 200)
			public float ReverbSize;                 // index 600:603 (value range 0 -> 10)
			public float ReverbWidth;                // index 604:607 (value range 0 -> 100)
			public ONOFF Solo;                       // index 610:611
			public ONOFF Sync;                       // index 614:615
			public ONOFF XArpHold01;                 // index 618:619
			public ONOFF XArpHold02;                 // index 622:623
			public ONOFF XArpHold03;                 // index 626:627
			public ONOFF XArpHold04;                 // index 630:631
			public ONOFF XArpHold05;                 // index 634:635
			public ONOFF XArpHold06;                 // index 638:639
			public ONOFF XArpHold07;                 // index 642:643
			public ONOFF XArpHold08;                 // index 646:647
			public ONOFF XArpHold09;                 // index 650:651
			public ONOFF XArpHold10;                 // index 654:655
			public ONOFF XArpHold11;                 // index 658:659
			public ONOFF XArpHold12;                 // index 662:663
			public ONOFF XArpHold13;                 // index 666:667
			public ONOFF XArpHold14;                 // index 670:671
			public ONOFF XArpHold15;                 // index 674:675
			public ONOFF XArpHold16;                 // index 678:679
			public float XArpTransp01;               // index 680:684 (value range -24 -> 24)
			public float XArpTransp02;               // (value range -24 -> 24)
			public float XArpTransp03;               // (value range -24 -> 24)
			public float XArpTransp04;               // (value range -24 -> 24)
			public float XArpTransp05;               // (value range -24 -> 24)
			public float XArpTransp06;               // (value range -24 -> 24)
			public float XArpTransp07;               // (value range -24 -> 24)
			public float XArpTransp08;               // (value range -24 -> 14)
			public float XArpTransp09;               // index 712:715 (value range -24 -> 24)
			public float XArpTransp10;               // index 716:719 (value range -24 -> 24)
			public float XArpTransp11;               // index 720:723 (value range -24 -> 24)
			public float XArpTransp12;               // index 724:727 (value range -24 -> 24)
			public float XArpTransp13;               // index 728:731 (value range -24 -> 24)
			public float XArpTransp14;               // index 732:735 (value range -24 -> 24)
			public float XArpTransp15;               // index 736:739 (value range -24 -> 24)
			public float XArpTransp16;               // index 740:743 (value range -24 -> 14)
			public float XArpVelo01;                 // (value range 0 -> 127)
			public float XArpVelo02;                 // (value range 0 -> 127)
			public float XArpVelo03;                 // (value range 0 -> 127)
			public float XArpVelo04;                 // (value range 0 -> 127)
			public float XArpVelo05;                 // (value range 0 -> 127)
			public float XArpVelo06;                 // (value range 0 -> 127)
			public float XArpVelo07;                 // (value range 0 -> 127)
			public float XArpVelo08;                 // (value range 0 -> 127)
			public float XArpVelo09;                 // index 776:779 (value range 0 -> 127)
			public float XArpVelo10;                 // index 780:783 (value range 0 -> 127)
			public float XArpVelo11;                 // index 784:787 (value range 0 -> 127)
			public float XArpVelo12;                 // index 788:791 (value range 0 -> 127)
			public float XArpVelo13;                 // index 792:795 (value range 0 -> 127)
			public float XArpVelo14;                 // index 796:799 (value range 0 -> 127)
			public float XArpVelo15;                 // index 800:803 (value range 0 -> 127)
			public float XArpVelo16;                 // index 804:807 (value range 0 -> 127)
			public float XModEnv1Dest1Am;            // index 808:811 (value range -10 -> 10)
			public float XModEnv1Dest2Am;            // index 812:815 (value range -10 -> 10)
			public float XModEnv2Dest1Am;            // index 816:819 (value range -10 -> 10)
			public float XModEnv2Dest2Am;            // index 820:823 (value range -10 -> 10)
			public float XModLFO1Dest1Am;            // index 824:827 (value range -10 -> 10)
			public float XModLFO1Dest2Am;            // index 828:831 (value range -10 -> 10)
			public float XModLFO2Dest1Am;            // index 832:835 (value range -10 -> 10)
			public float XModLFO2Dest2Am;            // index 836:839 (value range -10 -> 10)
			public float XModMisc1ADest1Am;          // index 840:843 (value range -10 -> 10)
			public float XModMisc1ADest2Am;          // index 844:847 (value range -10 -> 10)
			public XMODSOURCE XModMisc1ASource;      // index 848:851
			public float XModMisc1BDest1Am;          // index 852:855 (value range -10 -> 10)
			public float XModMisc1BDest2Am;          // index 856:859 (value range -10 -> 10)
			public XMODSOURCE XModMisc1BSource;      // index 860:863
			public float XModMisc2ADest1Am;          // index 864:867 (value range -10 -> 10)
			public float XModMisc2ADest2Am;          // index 868:871 (value range -10 -> 10)
			public XMODSOURCE XModMisc2ASource;      // index 872:875
			public float XModMisc2BDest1Am;          // index 876:879 (value range -10 -> 10)
			public float XModMisc2BDest2Am;          // index 880:883 (value range -10 -> 10)
			public XMODSOURCE XModMisc2BSource;      // index 884:887
			public ONOFF XSwArpOnOff;                // index 890:891
			public ONOFF XSwChorusOnOff;             // index 894:895
			public ONOFF XSwCompOnOff;               // index 898:899
			public ONOFF XSwDelayOnOff;              // index 902:903
			public ONOFF XSwDistOnOff;               // index 906:907
			public ONOFF XSwEQOnOff;                 // index 910:911
			public ONOFF XSwPhaserOnOff;             // index 914:915
			public ONOFF XSwReverbOnOff;             // index 918:919
			public YMODDEST YModEnv1Dest1;           // index 922:923
			public YMODDEST YModEnv1Dest2;           // index 926:927
			public YMODDEST YModEnv2Dest1;           // index 930:931
			public YMODDEST YModEnv2Dest2;           // index 934:935
			public YMODDEST YModLFO1Dest1;          // index 939:939
			public YMODDEST YModLFO1Dest2;          // index 942:943
			public YMODDEST YModLFO2Dest1;          // index 946:947
			public YMODDEST YModLFO2Dest2;          // index 950:951
			public YMODDEST YModMisc1ADest1;        // index 954:955
			public YMODDEST YModMisc1ADest2;        // index 958:959
			public YMODDEST YModMisc1BDest1;        // index 962:963
			public YMODDEST YModMisc1BDest2;        // index 966:967
			public YMODDEST YModMisc2ADest1;        // index 970:971
			public YMODDEST YModMisc2ADest2;        // index 974:975
			public YMODDEST YModMisc2BDest1;        // index 978:979
			public YMODDEST YModMisc2BDest2;        // index 982:983
			public YPARTSELECT YPartSelect;         // index 986:987
			public ZEQMODE ZEQMode;                 // index 990:991
			
			public Syl1PresetContent(BinaryFile bFile, bool newPresetVersion = true) {
				this.AmpEnvAAttack = bFile.ReadSingle();              // index 20:23 (value range 0 -> 10)
				this.AmpEnvADecay = bFile.ReadSingle();               // index 24:27 (value range 0 -> 10)
				this.AmpEnvARelease = bFile.ReadSingle();             // index 28:31 (value range 0 -> 10)
				this.AmpEnvASustain = bFile.ReadSingle();             // index 32:35 (value range 0 -> 10)
				this.AmpEnvBAttack = bFile.ReadSingle();              // index 36:39 (value range 0 -> 10)
				this.AmpEnvBDecay = bFile.ReadSingle();               // index 40:43 (value range 0 -> 10)
				this.AmpEnvBRelease = bFile.ReadSingle();             // index 44:47 (value range 0 -> 10)
				this.AmpEnvBSustain = bFile.ReadSingle();             // index 48:51 (value range 0 -> 10)
				this.ArpGate = bFile.ReadSingle();                    // index 52:55 (value range 0 -> 100)
				this.ArpMode = (ARPMODE) bFile.ReadUInt32();          // index 56:59
				this.ArpOctave = (ARPOCTAVE) bFile.ReadUInt32();      // index 60:63 (value range 1 -> 4)
				this.ArpTime = bFile.ReadSingle();                    // index 64:67 (value range 1/1 -> 1/64)
				this.ArpVelo = (ARPVELO) bFile.ReadUInt32();          // index 70:71
				this.ArpWrap = (ARPWRAP) bFile.ReadUInt32();          // index 72:75 (value range 0 -> 16)
				this.ChorusDelay = bFile.ReadSingle();                // index 76:79 (value range 1 -> 40)
				this.ChorusDepth = bFile.ReadSingle();                // index 80:83 (value range 0 -> 100)
				this.ChorusDry_Wet = bFile.ReadSingle();              // index 84:87 (value range 0 -> 100)
				this.ChorusFeedback = bFile.ReadSingle();             // index 88:91 (value range 0 -> 100)
				this.ChorusMode = ConvertChorusFloatToEnum(bFile.ReadSingle());    // index 94:95
				this.ChorusRate = bFile.ReadSingle();                 // index 96:99 (value range 0,01 -> 27,5)
				this.ChorusWidth = bFile.ReadSingle();                // index 100:103 (value range 0 -> 100)
				this.CompAttack = bFile.ReadSingle();                 // index 104:107 (value range 0,1 -> 300)
				this.CompRatio = bFile.ReadSingle();                  // index 108:111 (value range 1.00:1 -> 100.00:1)
				this.CompRelease = bFile.ReadSingle();                // index 112:115 (value range 1 -> 500)
				this.CompThreshold = bFile.ReadSingle();              // index 116:119 (value range -30 -> 0)
				this.DelayDry_Wet = bFile.ReadSingle();               // index 120:123 (value range 0 -> 100)
				this.DelayFeedback = bFile.ReadSingle();              // index 124:127 (value range 0 -> 100)
				this.DelayHighCut = bFile.ReadSingle();               // index 128:131 (value range 82,5 -> 21120)
				this.DelayLowCut = bFile.ReadSingle();                // index 132:135 (value range 3,23 -> 26483,12)
				this.DelayPingPong = ConvertOnOffFloatToEnum(bFile.ReadSingle());      // index 138:139
				this.DelaySmear = bFile.ReadSingle();                 // index 140:143 (value range 0 -> 10)
				this.DelaySpread = bFile.ReadSingle();                // index 144:147 (value range 0 -> 100)
				this.DelayTimeLeft = bFile.ReadSingle();              // index 148:151 (value range 1/64 -> 1/2)
				this.DelayTimeRight = bFile.ReadSingle();             // index 152:155 (value range 1/64 -> 1/2)
				this.DelayWidth = bFile.ReadSingle();                 // index 156:159 (value range 0 -> 100)
				this.DistortAmount = bFile.ReadSingle();              // index 160:163 (value range 0 -> 10)
				this.DistortDryWet = bFile.ReadSingle();              // index 164:167 (value range 0 -> 100)
				this.DistortType = (DISTORTTYPE) bFile.ReadUInt32();          // index 170:171
				this.EQBass = bFile.ReadSingle();                     // index 172:175 (value range 0 -> 15)
				this.EQBassFreq = bFile.ReadSingle();                 // index 176:179 (value range 13,75 -> 880)
				this.EQTreble = bFile.ReadSingle();                   // index 180:183 (value range 0 -> 15)
				this.EQTrebleFreq = bFile.ReadSingle();               // index 184:187 (value range 440 -> 28160)
				this.FilterACutoff = bFile.ReadSingle();              // index 188:191 (value range 1 -> 21341,28)
				this.FilterADrive = bFile.ReadSingle();               // index 192:195 (value range 0 -> 10)
				this.FilterAInput = (FILTERAINPUT) bFile.ReadUInt32();         // index 198:199
				this.FilterAReso = bFile.ReadSingle();                // index 200:203 (value range 0 -> 10)
				this.FilterAType = (FILTERTYPE) bFile.ReadUInt32();           // index 204:207
				this.FilterADB = (FILTERDB) bFile.ReadUInt32();                  // index 210:211 (value range 12 -> 12)
				this.FilterBCutoff = bFile.ReadSingle();              // index 212:215 (value range 1 -> 21341,28)
				this.FilterBDrive = bFile.ReadSingle();               // index 216:219 (value range 0 -> 10)
				this.FilterBInput = (FILTERBINPUT) bFile.ReadUInt32();         // index 222:223
				this.FilterBReso = bFile.ReadSingle();                // index 224:227 (value range 0 -> 10)
				this.FilterBType = (FILTERTYPE) bFile.ReadUInt32();           // index 228:231
				this.FilterBDB = (FILTERDB) bFile.ReadUInt32();                  // index 234:235 (value range 12 -> 24)
				this.FilterCtlCutoff = bFile.ReadSingle();            // index 236:239 (value range 1 -> 21341,28)
				this.FilterCtlKeyTrk = bFile.ReadSingle();            // index 240:243 (value range 0 -> 10)
				this.FilterCtlReso = bFile.ReadSingle();              // index 244:247 (value range 0 -> 10)
				this.FilterCtlWarmDrive = ConvertOnOffFloatToEnum(bFile.ReadSingle());         // index 250:251
				this.LFO1Free = ConvertOnOffFloatToEnum(bFile.ReadSingle());                   // index 254:255
				this.LFO1Gain = bFile.ReadSingle();                   // index 256:259 (value range 0 -> 10)
				this.LFO1Offset = bFile.ReadSingle();                 // index 260:263 (value range -10 -> 10)
				this.LFO1Rate = bFile.ReadSingle();                   // index 264:267 (value range 8/1D -> 1/256T)
				this.LFO1Wave = (LFOWAVE) bFile.ReadUInt32();                 	// index 268:271
				this.LFO2Free = ConvertOnOffFloatToEnum(bFile.ReadSingle()); 	// index 274:275
				this.LFO2Gain = bFile.ReadSingle();                   // index 276:279 (value range 0 -> 10)
				this.LFO2Offset = bFile.ReadSingle();                 // index 280:283 (value range -10 -> 10)
				this.LFO2Rate = bFile.ReadSingle();                   // index 284:287 (value range 8/1D -> 1/256T)
				this.LFO2Wave = (LFOWAVE) bFile.ReadUInt32();                 // index 288:291
				this.MainVolume = bFile.ReadSingle();                 // index 292:295 (value range 0 -> 10)
				this.MixA = bFile.ReadSingle();                       // index 296:299 (value range 0 -> 10)
				this.MixB = bFile.ReadSingle();                       // index 300:303 (value range 0 -> 10)
				this.ModEnv1Attack = bFile.ReadSingle();              // index 304:307 (value range 0 -> 10)
				this.ModEnv1Decay = bFile.ReadSingle();               // index 308:311 (value range 0 -> 10)
				this.ModEnv1Release = bFile.ReadSingle();             // index 312:315 (value range 0 -> 10)
				this.ModEnv1Sustain = bFile.ReadSingle();             // index 316:319 (value range 0 -> 10)
				this.ModEnv2Attack = bFile.ReadSingle();              // index 320:323 (value range 0 -> 10)
				this.ModEnv2Decay = bFile.ReadSingle();               // index 324:327 (value range 0 -> 10)
				this.ModEnv2Release = bFile.ReadSingle();             // index 328:331 (value range 0 -> 10)
				this.ModEnv2Sustain = bFile.ReadSingle();             // index 332:335 (value range 0 -> 10)
				this.ModWheel = bFile.ReadSingle();                   // index 336:339 (value range 0 -> 10)
				this.MonoLegato = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 342:343
				this.OscA1Detune = bFile.ReadSingle();                // index 344:347 (value range 0 -> 10)
				this.OscA1Fine = bFile.ReadSingle();                  // index 348:351 (value range -1 -> 1)
				this.OscA1Invert = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 354:355
				this.OscA1Note = bFile.ReadSingle();                  // index 356:359 (value range -7 -> 7)
				this.OscA1Octave = bFile.ReadSingle();                // index 360:363 (value range -3 -> 3)
				this.OscA1Pan = bFile.ReadSingle();                   // index 364:367 (value range -10 -> 10)
				this.OscA1Phase = bFile.ReadSingle();                 // index 368:371 (value range 0 -> 360)
				this.OscA1Retrig = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 374:375
				this.OscA1Stereo = bFile.ReadSingle();                // index 376:379 (value range 0 -> 10)
				this.OscA1Voices = (VOICES) bFile.ReadUInt32();                // index 382:383 (value range 0 -> 8)
				this.OscA1Volume = bFile.ReadSingle();                // index 384:387 (value range 0 -> 10)
				this.OscA1Wave = (OSCWAVE) bFile.ReadUInt32();                // index 388:391
				this.OscA2Detune = bFile.ReadSingle();                // index 392:395 (value range 0 -> 10)
				this.OscA2Fine = bFile.ReadSingle();                  // index 396:399 (value range -1 -> 1)
				this.OscA2Invert = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 402:403
				this.OscA2Note = bFile.ReadSingle();                  // index 404:407 (value range -7 -> 7)
				this.OscA2Octave = bFile.ReadSingle();                // index 408:411 (value range -3 -> 3)
				this.OscA2Pan = bFile.ReadSingle();                   // index 412:415 (value range -10 -> 10)
				this.OscA2Phase = bFile.ReadSingle();                 // index 416:419 (value range 0 -> 360)
				this.OscA2Retrig = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 422:423
				this.OscA2Stereo = bFile.ReadSingle();                // index 424:427 (value range 0 -> 10)
				this.OscA2Voices = (VOICES) bFile.ReadUInt32();                // index 430:431 (value range 0 -> 8)
				this.OscA2Volume = bFile.ReadSingle();                // index 432:435 (value range 0 -> 10)
				this.OscA2Wave = (OSCWAVE) bFile.ReadUInt32();                // index 436:439
				this.OscB1Detune = bFile.ReadSingle();                // index 440:443 (value range 0 -> 10)
				this.OscB1Fine = bFile.ReadSingle();                  // index 444:447 (value range -1 -> 1)
				this.OscB1Invert = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 450:451
				this.OscB1Note = bFile.ReadSingle();                  // index 452:455 (value range -7 -> 7)
				this.OscB1Octave = bFile.ReadSingle();                // index 456:459 (value range -3 -> 3)
				this.OscB1Pan = bFile.ReadSingle();                   // index 460:463 (value range -10 -> 10)
				this.OscB1Phase = bFile.ReadSingle();                 // index 464:467 (value range 0 -> 360)
				this.OscB1Retrig = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 470:471
				this.OscB1Stereo = bFile.ReadSingle();                // index 472:475 (value range 0 -> 10)
				this.OscB1Voices = (VOICES) bFile.ReadUInt32();                // index 478:479 (value range 0 -> 8)
				this.OscB1Volume = bFile.ReadSingle();                // index 480:483 (value range 0 -> 10)
				this.OscB1Wave = (OSCWAVE) bFile.ReadUInt32();                // index 484:487
				this.OscB2Detune = bFile.ReadSingle();                // index 488:491 (value range 0 -> 10)
				this.OscB2Fine = bFile.ReadSingle();                  // index 492:495 (value range -1 -> 1)
				this.OscB2Invert = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 498:499
				this.OscB2Note = bFile.ReadSingle();                  // index 500:503 (value range -7 -> 7)
				this.OscB2Octave = bFile.ReadSingle();                // index 504:507 (value range -3 -> 3)
				this.OscB2Pan = bFile.ReadSingle();                   // index 508:511 (value range -10 -> 10)
				this.OscB2Phase = bFile.ReadSingle();                 // index 512:515 (value range 0 -> 360)
				this.OscB2Retrig = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 518:519
				this.OscB2Stereo = bFile.ReadSingle();                // index 520:523 (value range 0 -> 10)
				this.OscB2Voices = (VOICES) bFile.ReadUInt32();                // index 526:527 (value range 0 -> 8)
				this.OscB2Volume = bFile.ReadSingle();                // index 528:531 (value range 0 -> 10)
				this.OscB2Wave = (OSCWAVE) bFile.ReadUInt32();                // index 532:535
				this.PhaserCenterFreq = bFile.ReadSingle();           // index 536:539 (value range 0 -> 10)
				this.PhaserDry_Wet = bFile.ReadSingle();              // index 540:543 (value range 0 -> 100)
				this.PhaserFeedback = bFile.ReadSingle();             // index 544:547 (value range 0 -> 100)
				this.PhaserLFOGain = bFile.ReadSingle();              // index 548:551 (value range 0 -> 10)
				this.PhaserLFORate = bFile.ReadSingle();              // index 552:555 (value range 8/1D -> 1/256T))
				this.PhaserLROffset = bFile.ReadSingle();             // index 556:559 (value range -10 -> 10)
				this.PhaserSpread = bFile.ReadSingle();               // index 560:563 (value range 0 -> 10)
				this.PhaserWidth = bFile.ReadSingle();                // index 564:567 (value range 0 -> 100)
				this.PitchBend = bFile.ReadSingle();                  // index 568:571 (value range -10 -> 10)
				this.PitchBendRange = bFile.ReadSingle();             // index 572:575 (value range 1 -> 24)
				this.Polyphony = bFile.ReadSingle();                  // index 578:579 (value range 0 -> 16)
				this.PortaMode = (PORTAMODE) bFile.ReadUInt32();              // index 582:583
				this.PortaTime = bFile.ReadSingle();                  // index 584:587 (value range 0 -> 10)
				this.ReverbDamp = bFile.ReadSingle();                 // index 588:591 (value range 0 -> 10)
				this.ReverbDry_Wet = bFile.ReadSingle();              // index 592:595 (value range 0 -> 100)
				this.ReverbPredelay = bFile.ReadSingle();             // index 596:599 (value range 0 -> 200)
				this.ReverbSize = bFile.ReadSingle();                 // index 600:603 (value range 0 -> 10)
				this.ReverbWidth = bFile.ReadSingle();                // index 604:607 (value range 0 -> 100)
				this.Solo = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 610:611
				this.Sync = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 614:615
				this.XArpHold01 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 618:619
				this.XArpHold02 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 622:623
				this.XArpHold03 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 626:627
				this.XArpHold04 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 630:631
				this.XArpHold05 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 634:635
				this.XArpHold06 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 638:639
				this.XArpHold07 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 642:643
				this.XArpHold08 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 646:647
				this.XArpHold09 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 650:651
				this.XArpHold10 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 654:655
				this.XArpHold11 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 658:659
				this.XArpHold12 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 662:663
				this.XArpHold13 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 666:667
				this.XArpHold14 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 670:671
				this.XArpHold15 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 674:675
				this.XArpHold16 = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 678:679
				this.XArpTransp01 = bFile.ReadSingle();               // index 680:684 (value range -24 -> 24)
				this.XArpTransp02 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp03 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp04 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp05 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp06 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp07 = bFile.ReadSingle();               // (value range -24 -> 24)
				this.XArpTransp08 = bFile.ReadSingle();               // (value range -24 -> 14)
				this.XArpTransp09 = bFile.ReadSingle();               // index 712:715 (value range -24 -> 24)
				this.XArpTransp10 = bFile.ReadSingle();               // index 716:719 (value range -24 -> 24)
				this.XArpTransp11 = bFile.ReadSingle();               // index 720:723 (value range -24 -> 24)
				this.XArpTransp12 = bFile.ReadSingle();               // index 724:727 (value range -24 -> 24)
				this.XArpTransp13 = bFile.ReadSingle();               // index 728:731 (value range -24 -> 24)
				this.XArpTransp14 = bFile.ReadSingle();               // index 732:735 (value range -24 -> 24)
				this.XArpTransp15 = bFile.ReadSingle();               // index 736:739 (value range -24 -> 24)
				this.XArpTransp16 = bFile.ReadSingle();               // index 740:743 (value range -24 -> 14)
				this.XArpVelo01 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo02 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo03 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo04 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo05 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo06 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo07 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo08 = bFile.ReadSingle();                 // (value range 0 -> 127)
				this.XArpVelo09 = bFile.ReadSingle();                 // index 776:779 (value range 0 -> 127)
				this.XArpVelo10 = bFile.ReadSingle();                 // index 780:783 (value range 0 -> 127)
				this.XArpVelo11 = bFile.ReadSingle();                 // index 784:787 (value range 0 -> 127)
				this.XArpVelo12 = bFile.ReadSingle();                 // index 788:791 (value range 0 -> 127)
				this.XArpVelo13 = bFile.ReadSingle();                 // index 792:795 (value range 0 -> 127)
				this.XArpVelo14 = bFile.ReadSingle();                 // index 796:799 (value range 0 -> 127)
				this.XArpVelo15 = bFile.ReadSingle();                 // index 800:803 (value range 0 -> 127)
				this.XArpVelo16 = bFile.ReadSingle();                 // index 804:807 (value range 0 -> 127)
				this.XModEnv1Dest1Am = bFile.ReadSingle();            // index 808:811 (value range -10 -> 10)
				this.XModEnv1Dest2Am = bFile.ReadSingle();            // index 812:815 (value range -10 -> 10)
				this.XModEnv2Dest1Am = bFile.ReadSingle();            // index 816:819 (value range -10 -> 10)
				this.XModEnv2Dest2Am = bFile.ReadSingle();            // index 820:823 (value range -10 -> 10)
				this.XModLFO1Dest1Am = bFile.ReadSingle();            // index 824:827 (value range -10 -> 10)
				this.XModLFO1Dest2Am = bFile.ReadSingle();            // index 828:831 (value range -10 -> 10)
				this.XModLFO2Dest1Am = bFile.ReadSingle();            // index 832:835 (value range -10 -> 10)
				this.XModLFO2Dest2Am = bFile.ReadSingle();            // index 836:839 (value range -10 -> 10)
				this.XModMisc1ADest1Am = bFile.ReadSingle();          // index 840:843 (value range -10 -> 10)
				this.XModMisc1ADest2Am = bFile.ReadSingle();          // index 844:847 (value range -10 -> 10)
				this.XModMisc1ASource = (XMODSOURCE) bFile.ReadUInt32();      // index 848:851
				this.XModMisc1BDest1Am = bFile.ReadSingle();          // index 852:855 (value range -10 -> 10)
				this.XModMisc1BDest2Am = bFile.ReadSingle();          // index 856:859 (value range -10 -> 10)
				this.XModMisc1BSource = (XMODSOURCE) bFile.ReadUInt32();      // index 860:863
				this.XModMisc2ADest1Am = bFile.ReadSingle();          // index 864:867 (value range -10 -> 10)
				this.XModMisc2ADest2Am = bFile.ReadSingle();          // index 868:871 (value range -10 -> 10)
				this.XModMisc2ASource = (XMODSOURCE) bFile.ReadUInt32();      // index 872:875
				this.XModMisc2BDest1Am = bFile.ReadSingle();          // index 876:879 (value range -10 -> 10)
				this.XModMisc2BDest2Am = bFile.ReadSingle();          // index 880:883 (value range -10 -> 10)
				this.XModMisc2BSource = (XMODSOURCE) bFile.ReadUInt32();      // index 884:887
				this.XSwArpOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());		// index 890:891
				this.XSwChorusOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 894:895
				this.XSwCompOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 898:899
				this.XSwDelayOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 902:903
				this.XSwDistOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 906:907
				this.XSwEQOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());		// index 910:911
				this.XSwPhaserOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 914:915
				this.XSwReverbOnOff = ConvertOnOffFloatToEnum(bFile.ReadSingle());	// index 918:919
				this.YModEnv1Dest1 = (YMODDEST) bFile.ReadUInt32();           // index 922:923
				this.YModEnv1Dest2 = (YMODDEST) bFile.ReadUInt32();           // index 926:927
				this.YModEnv2Dest1 = (YMODDEST) bFile.ReadUInt32();           // index 930:931
				this.YModEnv2Dest2 = (YMODDEST) bFile.ReadUInt32();           // index 934:935
				this.YModLFO1Dest1 = (YMODDEST) bFile.ReadUInt32();          // index 939:939
				this.YModLFO1Dest2 = (YMODDEST) bFile.ReadUInt32();          // index 942:943
				this.YModLFO2Dest1 = (YMODDEST) bFile.ReadUInt32();          // index 946:947
				this.YModLFO2Dest2 = (YMODDEST) bFile.ReadUInt32();          // index 950:951
				this.YModMisc1ADest1 = (YMODDEST) bFile.ReadUInt32();        // index 954:955
				this.YModMisc1ADest2 = (YMODDEST) bFile.ReadUInt32();        // index 958:959
				this.YModMisc1BDest1 = (YMODDEST) bFile.ReadUInt32();        // index 962:963
				this.YModMisc1BDest2 = (YMODDEST) bFile.ReadUInt32();        // index 966:967
				this.YModMisc2ADest1 = (YMODDEST) bFile.ReadUInt32();        // index 970:971
				this.YModMisc2ADest2 = (YMODDEST) bFile.ReadUInt32();        // index 974:975
				this.YModMisc2BDest1 = (YMODDEST) bFile.ReadUInt32();        // index 978:979
				this.YModMisc2BDest2 = (YMODDEST) bFile.ReadUInt32();        // index 982:983
				this.YPartSelect = (YPARTSELECT) bFile.ReadUInt32();         // index 986:987
				
				if (newPresetVersion) {
					this.ZEQMode = (ZEQMODE) bFile.ReadUInt32();                 // index 990:991
				}
				
				byte[] presetNameArray = StringUtils.RemoveTrailingBytes(bFile.ReadBytes(36));
				this.PresetName = StringUtils.RemoveInvalidCharactersAllowSpace(BinaryFile.ByteArrayToString(presetNameArray));
			}
		}
		
		#region Sylenth Preset Consistency Verification
		
		// find out what value is stored in the Enum Value and return a text string
		private static string EnumToLongText(object enumValue) {
			string enumString = null;
			uint i = Convert.ToUInt32(enumValue);
			float f = BinaryFile.ByteArrayToSingle(BitConverter.GetBytes(i), BinaryFile.ByteOrder.LittleEndian);
			enumString = String.Format("0x{0} (float: {1} uint: {2})", ((Enum)enumValue).ToString("X"), f, i);
			return enumString;
		}

		// find closest enum based on value, return text
		private static T FindClosestEnum<T>(Enum enumValue) {
			
			List<T> enumList = StringUtils.EnumValuesToList<T>();
			
			List<float> floatList = enumList.ConvertAll<float>( x => BinaryFile.UIntToSingle(Convert.ToUInt32(x), BinaryFile.ByteOrder.LittleEndian));
			float target = BinaryFile.UIntToSingle(Convert.ToUInt32(enumValue), BinaryFile.ByteOrder.LittleEndian);
			float closestFloat = MathUtils.FindClosest(floatList, target);
			uint closestValue = BinaryFile.SingleToUInt(closestFloat, BinaryFile.ByteOrder.LittleEndian);
			
			// convert to Enum type
			T closest = (T)Enum.ToObject(typeof(T), closestValue);
			
			return closest;
		}

		// find closest enum based on value, return text
		private static string FindClosestEnumText<T>(Enum enumValue) {
			
			List<T> enumList = StringUtils.EnumValuesToList<T>();
			
			// find closest Enum
			T closest = FindClosestEnum<T>(enumValue);

			// build result text
			string enumFoundText = closest.ToString() + " " + EnumToLongText(closest);
			List<string> enumAllTextList = enumList.ConvertAll<string>(x => x.ToString() + " " + EnumToLongText(x));
			string enumAllString = "";
			foreach (string s in enumAllTextList) {
				if (enumFoundText == s) {
					enumAllString += "\t" + s + " <----- Closest match! \n";
				} else {
					enumAllString += "\t" + s + "\n";
				}
			}
			return String.Format("\nClosest enum is {0} in: \n{1}", enumFoundText, enumAllString);
		}
		
		private static bool IsEnumConsistent<T>(Enum enumValue, bool doTryToFix) {
			string errorMsg = "";
			if (!Enum.IsDefined(typeof(T), enumValue)) {
				errorMsg = String.Format("Error! Consistency check failed for {0}! {1} is not a valid entry.", typeof(T), EnumToLongText(enumValue));
				if (!doTryToFix) {
					errorMsg += FindClosestEnumText<T>(enumValue);
				}
				Logger.DoError(errorMsg);
				return false;
			}
			return true;
		}

		private static bool MakeEnumConsistent<T>(Sylenth1Preset.Syl1PresetContent presetContent, Enum enumValue, string fieldName, bool doTryToFix) {
			if (doTryToFix) {
				T enumFound = FindClosestEnum<T>(enumValue);
				
				// use reflection to try to add correct field
				try {
					var type = typeof(Sylenth1Preset.Syl1PresetContent);
					var field = type.GetField(fieldName);
					field.SetValue(presetContent, enumFound);
				} catch (Exception) {
					Logger.DoError(String.Format("Warning! Could not find field {0} and store {1}!", fieldName, enumFound));
					return false;
				}
				
				Logger.DoLog(String.Format("Stored closest match for {0}: {1} {2}", typeof(T), enumFound.ToString(), EnumToLongText(enumFound)));
			}
			return true;
		}
		
		/// <summary>
		/// Verify that the loaded Sylenth content is consistent. I.e. that none of the ny of the enums are undefined
		/// </summary>
		/// <param name="presetContent">Preset Content</param>
		/// <param name="doTryToFix">Whether to try to estimate the enums or not</param>
		/// <returns></returns>
		private static bool IsConsistent(Sylenth1Preset.Syl1PresetContent presetContent, bool doTryToFix) {
			bool result = true;

			// check if any of the enums are undefined
			if (!IsEnumConsistent<ARPMODE>(presetContent.ArpMode, doTryToFix)) {
				result = MakeEnumConsistent<ARPMODE>(presetContent, presetContent.ArpMode, "ArpMode", doTryToFix);
			}
			if (!IsEnumConsistent<ARPVELO>(presetContent.ArpVelo, doTryToFix)) {
				result = MakeEnumConsistent<ARPVELO>(presetContent, presetContent.ArpVelo, "ArpVelo", doTryToFix);
			}
			if (!IsEnumConsistent<CHORUSMODE>(presetContent.ChorusMode, doTryToFix)) {
				result = MakeEnumConsistent<CHORUSMODE>(presetContent, presetContent.ChorusMode, "ChorusMode", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.DelayPingPong, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.DelayPingPong, "DelayPingPong", doTryToFix);
			}
			if (!IsEnumConsistent<DISTORTTYPE>(presetContent.DistortType, doTryToFix)) {
				result = MakeEnumConsistent<DISTORTTYPE>(presetContent, presetContent.DistortType, "DistortType", doTryToFix);
			}
			
			if (!IsEnumConsistent<FILTERAINPUT>(presetContent.FilterAInput, doTryToFix)) {
				result = MakeEnumConsistent<FILTERAINPUT>(presetContent, presetContent.FilterAInput, "FilterAInput", doTryToFix);
			}
			if (!IsEnumConsistent<FILTERTYPE>(presetContent.FilterAType, doTryToFix)) {
				result = MakeEnumConsistent<FILTERTYPE>(presetContent, presetContent.FilterAType, "FilterAType", doTryToFix);
			}
			if (!IsEnumConsistent<FILTERDB>(presetContent.FilterADB, doTryToFix)) {
				result = MakeEnumConsistent<FILTERDB>(presetContent, presetContent.FilterADB, "FilterADB", doTryToFix);
			}
			if (!IsEnumConsistent<FILTERBINPUT>(presetContent.FilterBInput, doTryToFix)) {
				result = MakeEnumConsistent<FILTERBINPUT>(presetContent, presetContent.FilterBInput, "FilterBInput", doTryToFix);
			}
			if (!IsEnumConsistent<FILTERTYPE>(presetContent.FilterBType, doTryToFix)) {
				result = MakeEnumConsistent<FILTERTYPE>(presetContent, presetContent.FilterBType, "FilterBType", doTryToFix);
			}
			if (!IsEnumConsistent<FILTERDB>(presetContent.FilterBDB, doTryToFix)) {
				result = MakeEnumConsistent<FILTERDB>(presetContent, presetContent.FilterBDB, "FilterBDB", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.FilterCtlWarmDrive, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.FilterCtlWarmDrive, "FilterCtlWarmDrive", doTryToFix);
			}

			if (!IsEnumConsistent<ONOFF>(presetContent.LFO1Free, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.LFO1Free, "LFO1Free", doTryToFix);
			}
			if (!IsEnumConsistent<LFOWAVE>(presetContent.LFO1Wave, doTryToFix)) {
				result = MakeEnumConsistent<LFOWAVE>(presetContent, presetContent.LFO1Wave, "LFO1Wave", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.LFO2Free, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.LFO2Free, "LFO2Free", doTryToFix);
			}
			if (!IsEnumConsistent<LFOWAVE>(presetContent.LFO2Wave, doTryToFix)) {
				result = MakeEnumConsistent<LFOWAVE>(presetContent, presetContent.LFO2Wave, "LFO2Wave", doTryToFix);
			}
			
			if (!IsEnumConsistent<ONOFF>(presetContent.MonoLegato, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.MonoLegato, "MonoLegato", doTryToFix);
			}

			if (!IsEnumConsistent<ONOFF>(presetContent.OscA1Invert, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscA1Invert, "OscA1Invert", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscA1Retrig, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscA1Retrig, "OscA1Retrig", doTryToFix);
			}
			if (!IsEnumConsistent<VOICES>(presetContent.OscA1Voices, doTryToFix)) {
				result = MakeEnumConsistent<VOICES>(presetContent, presetContent.OscA1Voices, "OscA1Voices", doTryToFix);
			}
			if (!IsEnumConsistent<OSCWAVE>(presetContent.OscA1Wave, doTryToFix)) {
				result = MakeEnumConsistent<OSCWAVE>(presetContent, presetContent.OscA1Wave, "OscA1Wave", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscA2Invert, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscA2Invert, "OscA2Invert", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscA2Retrig, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscA2Retrig, "OscA2Retrig", doTryToFix);
			}
			if (!IsEnumConsistent<VOICES>(presetContent.OscA2Voices, doTryToFix)) {
				result = MakeEnumConsistent<VOICES>(presetContent, presetContent.OscA2Voices, "OscA2Voices", doTryToFix);
			}
			if (!IsEnumConsistent<OSCWAVE>(presetContent.OscA2Wave, doTryToFix)) {
				result = MakeEnumConsistent<OSCWAVE>(presetContent, presetContent.OscA2Wave, "OscA2Wave", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscB1Invert, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscB1Invert, "OscB1Invert", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscB1Retrig, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscB1Retrig, "OscB1Retrig", doTryToFix);
			}
			if (!IsEnumConsistent<VOICES>(presetContent.OscB1Voices, doTryToFix)) {
				result = MakeEnumConsistent<VOICES>(presetContent, presetContent.OscB1Voices, "OscB1Voices", doTryToFix);
			}
			if (!IsEnumConsistent<OSCWAVE>(presetContent.OscB1Wave, doTryToFix)) {
				result = MakeEnumConsistent<OSCWAVE>(presetContent, presetContent.OscB1Wave, "OscB1Wave", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscB2Invert, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscB2Invert, "OscB2Invert", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.OscB2Retrig, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.OscB2Retrig, "OscB2Retrig", doTryToFix);
			}
			if (!IsEnumConsistent<VOICES>(presetContent.OscB2Voices, doTryToFix)) {
				result = MakeEnumConsistent<VOICES>(presetContent, presetContent.OscB2Voices, "OscB2Voices", doTryToFix);
			}
			if (!IsEnumConsistent<OSCWAVE>(presetContent.OscB2Wave, doTryToFix)) {
				result = MakeEnumConsistent<OSCWAVE>(presetContent, presetContent.OscB2Wave, "OscB2Wave", doTryToFix);
			}
			
			if (!IsEnumConsistent<PORTAMODE>(presetContent.PortaMode, doTryToFix)) {
				result = MakeEnumConsistent<PORTAMODE>(presetContent, presetContent.PortaMode, "PortaMode", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.Solo, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.Solo, "Solo", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.Sync, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.Sync, "Sync", doTryToFix);
			}

			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold01, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold01, "XArpHold01", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold02, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold02, "XArpHold02", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold03, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold03, "XArpHold03", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold04, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold04, "XArpHold04", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold05, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold05, "XArpHold05", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold06, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold06, "XArpHold06", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold07, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold07, "XArpHold07", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold08, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold08, "XArpHold08", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold09, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold09, "XArpHold09", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold10, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold10, "XArpHold10", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold11, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold11, "XArpHold11", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold12, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold12, "XArpHold12", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold13, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold13, "XArpHold13", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold14, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold14, "XArpHold14", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold15, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold15, "XArpHold15", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XArpHold16, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XArpHold16, "XArpHold16", doTryToFix);
			}
			
			if (!IsEnumConsistent<XMODSOURCE>(presetContent.XModMisc1ASource, doTryToFix)) {
				result = MakeEnumConsistent<XMODSOURCE>(presetContent, presetContent.XModMisc1ASource, "XModMisc1ASource", doTryToFix);
			}
			if (!IsEnumConsistent<XMODSOURCE>(presetContent.XModMisc1BSource, doTryToFix)) {
				result = MakeEnumConsistent<XMODSOURCE>(presetContent, presetContent.XModMisc1BSource, "XModMisc1BSource", doTryToFix);
			}
			if (!IsEnumConsistent<XMODSOURCE>(presetContent.XModMisc2ASource, doTryToFix)) {
				result = MakeEnumConsistent<XMODSOURCE>(presetContent, presetContent.XModMisc2ASource, "XModMisc2ASource", doTryToFix);
			}
			if (!IsEnumConsistent<XMODSOURCE>(presetContent.XModMisc2BSource, doTryToFix)) {
				result = MakeEnumConsistent<XMODSOURCE>(presetContent, presetContent.XModMisc2BSource, "XModMisc2BSource", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwArpOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwArpOnOff, "XSwArpOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwChorusOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwChorusOnOff, "XSwChorusOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwCompOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwCompOnOff, "XSwCompOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwDelayOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwDelayOnOff, "XSwDelayOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwDistOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwDistOnOff, "XSwDistOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwEQOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwEQOnOff, "XSwEQOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwPhaserOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwPhaserOnOff, "XSwPhaserOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<ONOFF>(presetContent.XSwReverbOnOff, doTryToFix)) {
				result = MakeEnumConsistent<ONOFF>(presetContent, presetContent.XSwReverbOnOff, "XSwReverbOnOff", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModEnv1Dest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModEnv1Dest1, "YModEnv1Dest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModEnv1Dest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModEnv1Dest2, "YModEnv1Dest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModEnv2Dest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModEnv2Dest1, "YModEnv2Dest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModEnv2Dest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModEnv2Dest2, "YModEnv2Dest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModLFO1Dest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModLFO1Dest1, "YModLFO1Dest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModLFO1Dest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModLFO1Dest2, "YModLFO1Dest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModLFO2Dest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModLFO2Dest1, "YModLFO2Dest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModLFO2Dest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModLFO2Dest2, "YModLFO2Dest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc1ADest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc1ADest1, "YModMisc1ADest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc1ADest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc1ADest2, "YModMisc1ADest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc1BDest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc1BDest1, "YModMisc1BDest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc1BDest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc1BDest2, "YModMisc1BDest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc2ADest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc2ADest1, "YModMisc2ADest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc2ADest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc2ADest2, "YModMisc2ADest2", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc2BDest1, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc2BDest1, "YModMisc2BDest1", doTryToFix);
			}
			if (!IsEnumConsistent<YMODDEST>(presetContent.YModMisc2BDest2, doTryToFix)) {
				result = MakeEnumConsistent<YMODDEST>(presetContent, presetContent.YModMisc2BDest2, "YModMisc2BDest2", doTryToFix);
			}
			if (!IsEnumConsistent<YPARTSELECT>(presetContent.YPartSelect, doTryToFix)) {
				result = MakeEnumConsistent<YPARTSELECT>(presetContent, presetContent.YPartSelect, "YPartSelect", doTryToFix);
			}
			if (!IsEnumConsistent<ZEQMODE>(presetContent.ZEQMode, doTryToFix)) {
				result = MakeEnumConsistent<ZEQMODE>(presetContent, presetContent.ZEQMode, "ZEQMode", doTryToFix);
			}
			
			return result;
		}
		#endregion
		
		#region Read and Write Methods
		public bool ReadFXP(FXP fxp, string filePath="")
		{
			if (fxp == null || fxp.ChunkDataByteArray == null) return false;
			
			var bFile = new BinaryFile(fxp.ChunkDataByteArray, BinaryFile.ByteOrder.LittleEndian);
			
			string presetType = bFile.ReadString(4);
			int presetVersion1 = bFile.ReadInt32();
			int fxVersion = bFile.ReadInt32();
			int numProgramsUnused = bFile.ReadInt32();
			int presetVersion2 = bFile.ReadInt32();
			
			Logger.DoLog(String.Format("Preset Type: '{0}', Preset Version1: '{1}', fxVersion: '{2}',  numProgramsUnused: '{3}', presetVersion2: '{4}'", presetType, presetVersion1, fxVersion, numProgramsUnused, presetVersion2));
			
			// if Sylenth 1 preset format
			if (presetType == "1lys") { // '1lys' = 'syl1' backwards
				
				// check if this is a bank or a single preset file
				if (fxp.FxMagic == "FBCh") {
					// this is a bank of presets
					
					// use numberOfPrograms to read in an array of Sylenth Presets
					int numPrograms = fxp.ProgramCount;
					var contentList = new List<Sylenth1Preset.Syl1PresetContent>();
					int i = 0;
					try {
						Sylenth1Preset.Syl1PresetContent presetContent = null;
						for (i = 0; i < numPrograms; i++) {
							presetContent = new Sylenth1Preset.Syl1PresetContent(bFile, (presetVersion1==2202 || presetVersion1 == 2211));
							Logger.DoLog(String.Format("Start processing {0} '{1}' ...", i, presetContent.PresetName));
							if (IsConsistent(presetContent, DO_TRY_FIX_INCONSISTENT_ENUMS)) {
								contentList.Add(presetContent);
							} else {
								Logger.DoError(String.Format("Error! The loaded preset failed the consistency check! {0} '{1}' ({2})", i, presetContent.PresetName, filePath));
							}
						}
					} catch(EndOfStreamException eose) {
						Logger.DoError(String.Format("Error! Failed reading presets from bank {0}. Read {1} presets. (Expected {2}) Msg: {3}!", filePath, i, numPrograms, eose.Message));
						return false;
					}

					// store list as array
					this.ContentArray = contentList.ToArray();
					Logger.DoLog(String.Format("Finished reading {0} presets from bank {1} ...", numPrograms, filePath));
					return true;
				} else if (fxp.FxMagic == "FPCh") {
					// single preset file
					this.ContentArray = new Syl1PresetContent[1];
					this.ContentArray[0] = new Sylenth1Preset.Syl1PresetContent(bFile, presetVersion1==2202);

					Logger.DoLog(String.Format("Finished reading preset file {0} ...", filePath));
					return true;
				}
			} else {
				Logger.DoError(String.Format("Error! The preset file is not a valid Sylenth1 Preset! ({0}). If you are sure it is a Sylenth preset, try to open it in Sylenth and resave (Tip! init full bank before loading and resaving)", filePath));
				return false;
			}
			return false;
		}
		
		public bool Read(string filePath)
		{
			// store filepath
			FilePath = filePath;
			
			var fxp = new FXP();
			fxp.ReadFile(filePath);
			
			if (!ReadFXP(fxp, filePath)) {
				return false;
			}
			return true;
		}
		
		public bool Write(string filePath)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
