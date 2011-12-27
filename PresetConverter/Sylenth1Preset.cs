using System;
using System.Text;
using System.Runtime.InteropServices;

namespace PresetConverter
{
	/// <summary>
	/// Description of Sylenth1Preset.
	/// </summary>
	public class Sylenth1Preset : Preset
	{
		#region Enums
		private enum FloatToHz {
			DelayLowCut,
			DelayHighCut,
			FilterCutoff,
			EQBassFreq,
			EQTrebleFreq,
			LFORateFree
		}
		
		public enum ARPMODE : uint {
			Chord      = 0x00003F80,
			Down       = 0x3DE38E39,
			Down_Up    = 0x3EE38E39,
			Down_Up2   = 0x3F0E38E4,
			Ordered    = 0x3F471C72,
			Random     = 0x3F2AAAAB,
			Step       = 0x3F638E39,
			Up         = 0x00000000,
			Up_Down    = 0x3E638E39,
			Up_Down2   = 0x3EAAAAAB
		}

		public enum ARPVELO : uint {
			VEL_Hold       = 0x3E800000,
			VEL_Key        = 0x00000000,
			VEL_Step       = 0x3F000000,
			VEL_StepHold   = 0x3F800000,
			VEL_StepKey    = 0x3F400000
		}

		public enum CHORUSMODE : uint {
			Dual    = 0x3F800000,
			Single  = 0x00000000
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

		public enum DISTORTTYPE : uint {
			BitCrush= 0x3F800000,
			Clip    = 0x3F000000,
			Decimate= 0x3F400000,
			FoldBack= 0x3E800000,
			OverDrv = 0x00000000
		}

		public enum FILTERAINPUT : uint {
			FILTER_A     = 0x00000000,
			FILTER_A_B   = 0x3F000000,
			FILTER_None  = 0x3F800000
		}

		public enum FILTERBINPUT : uint {
			FILTER_B     = 0x00000000,
			FILTER_B_A   = 0x3F000000,
			FILTER_None  = 0x3F800000
		}
		
		public enum FILTERTYPE : uint {
			Highpass= 0x3F800000,
			Bandpass= 0x3F2AAAAB,
			Lowpass = 0x3EAAAAAB,
			Bypass  = 0x00000000
		}
		
		public enum FILTERDB : uint {
			DB12  = 0x3F800000,
			DB24  = 0x00000000
		}

		public enum LFOWAVE : uint {
			LFO_HPulse    = 0x3F000000,
			LFO_Lorenz    = 0x3F4CCCCD,
			LFO_Pulse     = 0x3ECCCCCD,
			LFO_QPulse    = 0x3F19999A,
			LFO_Ramp      = 0x844445CC,
			LFO_Ramp2     = 0xCCCD3E4C,
			LFO_Random    = 0x3F800000,
			LFO_Saw       = 0x3DCCCCCD,
			LFO_Sine      = 0x00000000,
			LFO_SmpHold   = 0x3F666666,
			LFO_Triangle  = 0x3E99999A,
			LFO_TriSaw    = 0x3F333333
		}

		public enum OSCWAVE : uint {
			OSC_HPulse   = 0x3F124925,
			OSC_Noise    = 0x3F800000,
			OSC_Pulse    = 0x3EDB6DB7,
			OSC_QPulse   = 0x3F36DB6E,
			OSC_Saw      = 0x3E124925,
			OSC_Sine     = 0x00000000,
			OSC_Triangle = 0x3E924925,
			OSC_TriSaw   = 0x3F5B6DB7
		}

		public enum PORTAMODE : uint {
			Normal   = 0x3F800000,
			Slide    = 0x00000000
		}

		public enum ONOFF : uint {
			Off = 0x00000000,
			On  = 0x3F800000
		}

		public enum XMODSOURCE : uint {
			SOURCE_None     = 0x00000000,
			SOURCE_Velocity = 0x3DBA2E8C,
			SOURCE_ModWheel = 0x3E3A2E8C,
			SOURCE_KeyTrack = 0x3E8BA2E9,
			SOURCE_AmpEnv_A = 0x3EBA2E8C,
			SOURCE_AmpEnv_B = 0x3EE8BA2F,
			SOURCE_ModEnv_1 = 0x3F0BA2E9,
			SOURCE_ModEnv_2 = 0x3F22E8BA,
			SOURCE_LFO_1    = 0x3F3A2E8C,
			SOURCE_LFO_2    = 0x3F51745D,
			SOURCE_Aftertch = 0x3F68BA2F,
			SOURCE_StepVlty = 0x3F800000,
		}

		public enum YMODDEST : uint {
			None        = 0x00000000,

			// Oscillators
			Volume_A    = 0x3D000000,
			Volume_B    = 0x3D800000,
			VolumeAB    = 0x3DC00000,
			Pitch_A     = 0x3E000000,
			Pitch_B     = 0x3E200000,
			Pitch_AB    = 0x3E400000,
			Phase_A     = 0x3E600000,
			Phase_B     = 0x3E800000,
			Phase_AB    = 0x3E900000,
			Pan_A       = 0x3EA00000,
			Pan_B       = 0x3EB00000,
			Pan_AB      = 0x3EC00000,

			// Filters
			Cutoff_A    = 0x3ED00000,
			Cutoff_B    = 0x3EE00000,
			CutoffAB    = 0x3EF00000,
			Reso_A      = 0x3F000000,
			Reso_B      = 0x3F080000,
			Reso_AB     = 0x3F100000,

			// Misc
			PhsrFreq    = 0x3F180000,
			Mix_A       = 0x3F200000,
			Mix_B       = 0x3F280000,
			Mix_AB      = 0x3F300000,
			LFO1Rate    = 0x3F380000,
			LFO1Gain    = 0x3F400000,
			LFO2Rate    = 0x3F480000,
			LFO2Gain    = 0x3F500000,
			DistAmnt    = 0x3F580000
		}

		public enum YPARTSELECT : uint {
			Part_A = 0x00000000,
			Part_B = 0x3F800000
		}

		public enum ZEQMODE : uint {
			MODE_1_Pole = 0x00000000,
			MODE_2_Pole = 0x3F800000
		}
		
		public enum VOICES : uint {
			VOICES_0       = 0x00000000,
			VOICES_1       = 0x3E000000,
			VOICES_2       = 0x3E800000,
			VOICES_3       = 0x3EC00000,
			VOICES_4       = 0x3F000000,
			VOICES_5       = 0x3F200000,
			VOICES_6       = 0x3F400000,
			VOICES_7       = 0x3F600000,
			VOICES_8       = 0x3F800000
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
		
		public Syl1PresetContent Content { set; get; }
		public string PresetName { set; get; }
		
		public Sylenth1Preset()
		{
		}
		
		public Sylenth1Preset(string filePath) {
			Read(filePath);
		}
		
		private static DELAYTIMING DelayTimeFloatToEnum(float value) {
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
			} else if (x >= 0.9809523810f && x < 1.0000000000f) {
				timing = DELAYTIMING.DELAY_1_2;
			}
			return timing;
		}

		private static LFOTIMING LFOTimeFloatToEnum(float value) {
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
			} else if (x >= 0.9857142857f && x < 1.0000000000f) {
				timing = LFOTIMING.LFO_1_256T;
			}

			return timing;
		}
		
		private static COMPRATIO CompRatioFloatToEnum(float value) {
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
		
		private static float ValueToHz(float oldValue, FloatToHz mode) {
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
		
		private static string ValueToStringHz(object val, float newMin, float newMax, FloatToHz mode) {
			if (val is int) {
				//int oldValue = (int) val;
				return "<TODO: int support is not implemented>";
			} else if (val is float) {
				float oldValue = (float) val;
				float newValue = ValueToHz(oldValue, mode);
				return String.Format("{0:0.0000}. Display Value: {1:0.00} Hz (Range: {2} -> {3})", oldValue, newValue, newMin, newMax);
			}
			return val.ToString();
		}
		
		public override string ToString() {
			StringBuilder buffer = new StringBuilder();

			buffer.AppendLine("//");
			buffer.AppendLine("// Sylenth1 Preset Content");
			buffer.AppendLine("//");

			buffer.AppendFormat("AmpEnvAAttack: {0}\n", ValueToString(Content.AmpEnvAAttack, 0, 10));	// index 20:23 (value range 0 -> 10)
			buffer.AppendFormat("AmpEnvADecay: {0}\n", ValueToString(Content.AmpEnvADecay, 0, 10));		// index 24:27 (value range 0 -> 10)
			buffer.AppendFormat("AmpEnvARelease: {0}\n", ValueToString(Content.AmpEnvARelease, 0, 10));	// index 28:31 (value range 0 -> 10)
			buffer.AppendFormat("AmpEnvASustain: {0}\n", ValueToString(Content.AmpEnvASustain, 0, 10));	// index 32:35 (value range 0 -> 10)
			buffer.AppendLine();
			buffer.AppendFormat("AmpEnvBAttack: {0}\n", ValueToString(Content.AmpEnvBAttack, 0, 10));	// index 36:39 (value range 0 -> 10)
			buffer.AppendFormat("AmpEnvBDecay: {0}\n", ValueToString(Content.AmpEnvBDecay, 0, 10));		// index 40:43 (value range 0 -> 10)
			buffer.AppendFormat("AmpEnvBRelease: {0}\n", ValueToString(Content.AmpEnvBRelease, 0, 10));	// index 44:47 (value range 0 -> 10)
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
			buffer.AppendLine();
			buffer.AppendFormat("FilterBCutoff: {0}\n", ValueToStringHz(Content.FilterBCutoff, 1.0f, 21341.28f, FloatToHz.FilterCutoff));	// index 212:215 (value range 1 -> 21341,28)
			buffer.AppendFormat("FilterBDrive: {0}\n", ValueToString(Content.FilterBDrive, 0, 10));		// index 216:219 (value range 0 -> 10)
			buffer.AppendFormat("FilterBInput: {0}\n", Content.FilterBInput);         					// index 222:223
			buffer.AppendFormat("FilterBReso: {0}\n", ValueToString(Content.FilterBReso, 0, 10));		// index 224:227 (value range 0 -> 10)
			buffer.AppendFormat("FilterBType: {0}\n", Content.FilterBType);           					// index 228:231
			buffer.AppendFormat("FilterBDB: {0}\n", Content.FilterBDB);									// index 234:235 (value range 12 -> 24)
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
			buffer.AppendFormat("PresetName: {0}\n", PresetName);
			
			return buffer.ToString();
		}
		
		public class Syl1PresetContent {
			
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
			public float ArpOctave;                  // index 60:63 (value range 1 -> 4)
			public float ArpTime;                    // index 64:67 (value range 1/1 -> 1/64)
			public ARPVELO ArpVelo;                  // index 70:71
			public float ArpWrap;                    // index 72:75 (value range 0 -> 16)
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

			public Syl1PresetContent(BinaryFile bFile) {
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
				this.ArpOctave = bFile.ReadSingle();                  // index 60:63 (value range 1 -> 4)
				this.ArpTime = bFile.ReadSingle();                    // index 64:67 (value range 1/1 -> 1/64)
				this.ArpVelo = (ARPVELO) bFile.ReadUInt32();          // index 70:71
				this.ArpWrap = bFile.ReadSingle();                    // index 72:75 (value range 0 -> 16)
				this.ChorusDelay = bFile.ReadSingle();                // index 76:79 (value range 1 -> 40)
				this.ChorusDepth = bFile.ReadSingle();                // index 80:83 (value range 0 -> 100)
				this.ChorusDry_Wet = bFile.ReadSingle();              // index 84:87 (value range 0 -> 100)
				this.ChorusFeedback = bFile.ReadSingle();             // index 88:91 (value range 0 -> 100)
				this.ChorusMode = (CHORUSMODE) bFile.ReadUInt32();    // index 94:95
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
				this.DelayPingPong = (ONOFF) bFile.ReadUInt32();      // index 138:139
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
				this.FilterCtlWarmDrive = (ONOFF) bFile.ReadUInt32();         // index 250:251
				this.LFO1Free = (ONOFF) bFile.ReadUInt32();                   // index 254:255
				this.LFO1Gain = bFile.ReadSingle();                   // index 256:259 (value range 0 -> 10)
				this.LFO1Offset = bFile.ReadSingle();                 // index 260:263 (value range -10 -> 10)
				this.LFO1Rate = bFile.ReadSingle();                   // index 264:267 (value range 8/1D -> 1/256T)
				this.LFO1Wave = (LFOWAVE) bFile.ReadUInt32();                 // index 268:271
				this.LFO2Free = (ONOFF) bFile.ReadUInt32();                   // index 274:275
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
				this.MonoLegato = (ONOFF) bFile.ReadUInt32();                 // index 342:343
				this.OscA1Detune = bFile.ReadSingle();                // index 344:347 (value range 0 -> 10)
				this.OscA1Fine = bFile.ReadSingle();                  // index 348:351 (value range -1 -> 1)
				this.OscA1Invert = (ONOFF) bFile.ReadUInt32();                // index 354:355
				this.OscA1Note = bFile.ReadSingle();                  // index 356:359 (value range -7 -> 7)
				this.OscA1Octave = bFile.ReadSingle();                // index 360:363 (value range -3 -> 3)
				this.OscA1Pan = bFile.ReadSingle();                   // index 364:367 (value range -10 -> 10)
				this.OscA1Phase = bFile.ReadSingle();                 // index 368:371 (value range 0 -> 360)
				this.OscA1Retrig = (ONOFF) bFile.ReadUInt32();                // index 374:375
				this.OscA1Stereo = bFile.ReadSingle();                // index 376:379 (value range 0 -> 10)
				this.OscA1Voices = (VOICES) bFile.ReadUInt32();                // index 382:383 (value range 0 -> 8)
				this.OscA1Volume = bFile.ReadSingle();                // index 384:387 (value range 0 -> 10)
				this.OscA1Wave = (OSCWAVE) bFile.ReadUInt32();                // index 388:391
				this.OscA2Detune = bFile.ReadSingle();                // index 392:395 (value range 0 -> 10)
				this.OscA2Fine = bFile.ReadSingle();                  // index 396:399 (value range -1 -> 1)
				this.OscA2Invert = (ONOFF) bFile.ReadUInt32();                // index 402:403
				this.OscA2Note = bFile.ReadSingle();                  // index 404:407 (value range -7 -> 7)
				this.OscA2Octave = bFile.ReadSingle();                // index 408:411 (value range -3 -> 3)
				this.OscA2Pan = bFile.ReadSingle();                   // index 412:415 (value range -10 -> 10)
				this.OscA2Phase = bFile.ReadSingle();                 // index 416:419 (value range 0 -> 360)
				this.OscA2Retrig = (ONOFF) bFile.ReadUInt32();                // index 422:423
				this.OscA2Stereo = bFile.ReadSingle();                // index 424:427 (value range 0 -> 10)
				this.OscA2Voices = (VOICES) bFile.ReadUInt32();                // index 430:431 (value range 0 -> 8)
				this.OscA2Volume = bFile.ReadSingle();                // index 432:435 (value range 0 -> 10)
				this.OscA2Wave = (OSCWAVE) bFile.ReadUInt32();                // index 436:439
				this.OscB1Detune = bFile.ReadSingle();                // index 440:443 (value range 0 -> 10)
				this.OscB1Fine = bFile.ReadSingle();                  // index 444:447 (value range -1 -> 1)
				this.OscB1Invert = (ONOFF) bFile.ReadUInt32();                // index 450:451
				this.OscB1Note = bFile.ReadSingle();                  // index 452:455 (value range -7 -> 7)
				this.OscB1Octave = bFile.ReadSingle();                // index 456:459 (value range -3 -> 3)
				this.OscB1Pan = bFile.ReadSingle();                   // index 460:463 (value range -10 -> 10)
				this.OscB1Phase = bFile.ReadSingle();                 // index 464:467 (value range 0 -> 360)
				this.OscB1Retrig = (ONOFF) bFile.ReadUInt32();                // index 470:471
				this.OscB1Stereo = bFile.ReadSingle();                // index 472:475 (value range 0 -> 10)
				this.OscB1Voices = (VOICES) bFile.ReadUInt32();                // index 478:479 (value range 0 -> 8)
				this.OscB1Volume = bFile.ReadSingle();                // index 480:483 (value range 0 -> 10)
				this.OscB1Wave = (OSCWAVE) bFile.ReadUInt32();                // index 484:487
				this.OscB2Detune = bFile.ReadSingle();                // index 488:491 (value range 0 -> 10)
				this.OscB2Fine = bFile.ReadSingle();                  // index 492:495 (value range -1 -> 1)
				this.OscB2Invert = (ONOFF) bFile.ReadUInt32();                // index 498:499
				this.OscB2Note = bFile.ReadSingle();                  // index 500:503 (value range -7 -> 7)
				this.OscB2Octave = bFile.ReadSingle();                // index 504:507 (value range -3 -> 3)
				this.OscB2Pan = bFile.ReadSingle();                   // index 508:511 (value range -10 -> 10)
				this.OscB2Phase = bFile.ReadSingle();                 // index 512:515 (value range 0 -> 360)
				this.OscB2Retrig = (ONOFF) bFile.ReadUInt32();                // index 518:519
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
				this.Solo = (ONOFF) bFile.ReadUInt32();                       // index 610:611
				this.Sync = (ONOFF) bFile.ReadUInt32();                       // index 614:615
				this.XArpHold01 = (ONOFF) bFile.ReadUInt32();                 // index 618:619
				this.XArpHold02 = (ONOFF) bFile.ReadUInt32();                 // index 622:623
				this.XArpHold03 = (ONOFF) bFile.ReadUInt32();                 // index 626:627
				this.XArpHold04 = (ONOFF) bFile.ReadUInt32();                 // index 630:631
				this.XArpHold05 = (ONOFF) bFile.ReadUInt32();                 // index 634:635
				this.XArpHold06 = (ONOFF) bFile.ReadUInt32();                 // index 638:639
				this.XArpHold07 = (ONOFF) bFile.ReadUInt32();                 // index 642:643
				this.XArpHold08 = (ONOFF) bFile.ReadUInt32();                 // index 646:647
				this.XArpHold09 = (ONOFF) bFile.ReadUInt32();                 // index 650:651
				this.XArpHold10 = (ONOFF) bFile.ReadUInt32();                 // index 654:655
				this.XArpHold11 = (ONOFF) bFile.ReadUInt32();                 // index 658:659
				this.XArpHold12 = (ONOFF) bFile.ReadUInt32();                 // index 662:663
				this.XArpHold13 = (ONOFF) bFile.ReadUInt32();                 // index 666:667
				this.XArpHold14 = (ONOFF) bFile.ReadUInt32();                 // index 670:671
				this.XArpHold15 = (ONOFF) bFile.ReadUInt32();                 // index 674:675
				this.XArpHold16 = (ONOFF) bFile.ReadUInt32();                 // index 678:679
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
				this.XSwArpOnOff = (ONOFF) bFile.ReadUInt32();                // index 890:891
				this.XSwChorusOnOff = (ONOFF) bFile.ReadUInt32();             // index 894:895
				this.XSwCompOnOff = (ONOFF) bFile.ReadUInt32();               // index 898:899
				this.XSwDelayOnOff = (ONOFF) bFile.ReadUInt32();              // index 902:903
				this.XSwDistOnOff = (ONOFF) bFile.ReadUInt32();               // index 906:907
				this.XSwEQOnOff = (ONOFF) bFile.ReadUInt32();                 // index 910:911
				this.XSwPhaserOnOff = (ONOFF) bFile.ReadUInt32();             // index 914:915
				this.XSwReverbOnOff = (ONOFF) bFile.ReadUInt32();             // index 918:919
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
				this.ZEQMode = (ZEQMODE) bFile.ReadUInt32();                 // index 990:991
			}
		}
		
		private static float ConvertValueSylenthToZebra(float storedSylenthValue, float displaySylenthMin, float displaySylenthMax, float zebraMin, float zebraMax) {
			float sylenthDisplayValue = MathUtils.ConvertAndMainainRatio(storedSylenthValue, 0, 1, displaySylenthMin, displaySylenthMax);
			return MathUtils.ConvertAndMainainRatio(sylenthDisplayValue, displaySylenthMin, displaySylenthMax, zebraMin, zebraMax);
		}

		private static int ConvertSylenthVoicesToZebra(VOICES numberOfVoices) {
			int zebraVoices = (int) Zebra2Preset.OscillatorPoly.single;
			
			switch (numberOfVoices) {
				case VOICES.VOICES_1:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.single;
					break;
				case VOICES.VOICES_2:
				case VOICES.VOICES_3:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.dual;
					break;
				case VOICES.VOICES_4:
				case VOICES.VOICES_5:
				case VOICES.VOICES_6:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.quad;
					break;
				case VOICES.VOICES_7:
				case VOICES.VOICES_8:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.eleven;
					break;
			}
			return zebraVoices;
		}
		
		private static int ConvertSylenthWaveToZebra(OSCWAVE wave) {
			
			int zebraWave = 1;
			
			switch (wave) {
				case OSCWAVE.OSC_HPulse:
					zebraWave = 1;
					break;
				case OSCWAVE.OSC_Noise:
					// TODO: special case - noise is not a waveform in zebra but a seperate unit
					zebraWave = 2;
					break;
				case OSCWAVE.OSC_Pulse:
					zebraWave = 3;
					break;
				case OSCWAVE.OSC_QPulse:
					zebraWave = 4;
					break;
				case OSCWAVE.OSC_Saw:
					zebraWave = 5;
					break;
				case OSCWAVE.OSC_Triangle:
					zebraWave = 6;
					break;
				case OSCWAVE.OSC_TriSaw:
					zebraWave = 7;
					break;
			}
			return zebraWave;
		}

		private static float ConvertSylenthTuneToZebra(float octave, float note, float fine) {
			
			// sylenth octave + note + fine makes up a zebra Tune?
			float osc_octave = MathUtils.ConvertAndMainainRatio(octave, 0, 1, -3, 3);
			float osc_note = MathUtils.ConvertAndMainainRatio(note, 0, 1, -7, 7);
			float osc_fine = MathUtils.ConvertAndMainainRatio(fine, 0, 1, -1, 1);
			float osc_tune = (12 * osc_octave) + osc_note + osc_fine;
			return osc_tune;
		}
		
		private static float ConvertSylenthFrequencyToZebra(float frequency, FloatToHz mode) {
			float hertz = ValueToHz(frequency, mode);	
			int midiNote = Zebra2Preset.FilterFrequencyToMidiNote(hertz);
			return midiNote;
		}
				
		public Zebra2Preset ToZebra2Preset() {
			
			// load a default preset file and modify this one
			Zebra2Preset zebra2Preset = new Zebra2Preset();
			
			// OscA1
			if (Content.OscA1Voices != VOICES.VOICES_0) {
				zebra2Preset.OSC1_Tune = ConvertSylenthTuneToZebra(Content.OscA1Octave, Content.OscA1Note, Content.OscA1Fine);
				zebra2Preset.OSC1_WNum = ConvertSylenthWaveToZebra(Content.OscA1Wave);
				zebra2Preset.OSC1_Poly = ConvertSylenthVoicesToZebra(Content.OscA1Voices);
				zebra2Preset.OSC1_Dtun = ConvertValueSylenthToZebra(Content.OscA1Detune, 0, 10, -50, 50);
				zebra2Preset.OSC1_Pan = ConvertValueSylenthToZebra(Content.OscA1Pan, -10, 10, -100, 100);
				zebra2Preset.OSC1_PolW = ConvertValueSylenthToZebra(Content.OscA1Stereo, 0, 10, 0, 100);
				zebra2Preset.OSC1_Phse = ConvertValueSylenthToZebra(Content.OscA1Phase, 0, 360, 0, 100);
				zebra2Preset.OSC1_Vol = ConvertValueSylenthToZebra(Content.OscA1Volume, 0, 10, 0, 200);
				
				// These are not yet handled!
				// Content.OscA1Retrig
				// Content.OscA1Wave
				// Content.OscA1Invert
			}

			// OscA2
			if (Content.OscA2Voices != VOICES.VOICES_0) {
				zebra2Preset.OSC2_Tune = ConvertSylenthTuneToZebra(Content.OscA2Octave, Content.OscA2Note, Content.OscA2Fine);
				zebra2Preset.OSC2_WNum = ConvertSylenthWaveToZebra(Content.OscA2Wave);
				zebra2Preset.OSC2_Poly = ConvertSylenthVoicesToZebra(Content.OscA2Voices);
				zebra2Preset.OSC2_Dtun = ConvertValueSylenthToZebra(Content.OscA2Detune, 0, 10, -50, 50);
				zebra2Preset.OSC2_Pan = ConvertValueSylenthToZebra(Content.OscA2Pan, -10, 10, -100, 100);
				zebra2Preset.OSC2_PolW = ConvertValueSylenthToZebra(Content.OscA2Stereo, 0, 10, 0, 100);
				zebra2Preset.OSC2_Phse = ConvertValueSylenthToZebra(Content.OscA2Phase, 0, 360, 0, 100);
				zebra2Preset.OSC2_Vol = ConvertValueSylenthToZebra(Content.OscA2Volume, 0, 10, 0, 200);
				
				// These are not yet handled!
				// Content.OscA2Retrig
				// Content.OscA2Wave
				// Content.OscA2Invert
			}
			
			// OscB1
			if (Content.OscB1Voices != VOICES.VOICES_0) {
				zebra2Preset.OSC3_Tune = ConvertSylenthTuneToZebra(Content.OscB1Octave, Content.OscB1Note, Content.OscB1Fine);
				zebra2Preset.OSC3_WNum = ConvertSylenthWaveToZebra(Content.OscB1Wave);
				zebra2Preset.OSC3_Poly = ConvertSylenthVoicesToZebra(Content.OscB1Voices);
				zebra2Preset.OSC3_Dtun = ConvertValueSylenthToZebra(Content.OscB1Detune, 0, 10, -50, 50);
				zebra2Preset.OSC3_Pan = ConvertValueSylenthToZebra(Content.OscB1Pan, -10, 10, -100, 100);
				zebra2Preset.OSC3_PolW = ConvertValueSylenthToZebra(Content.OscB1Stereo, 0, 10, 0, 100);
				zebra2Preset.OSC3_Phse = ConvertValueSylenthToZebra(Content.OscB1Phase, 0, 360, 0, 100);
				zebra2Preset.OSC3_Vol = ConvertValueSylenthToZebra(Content.OscB1Volume, 0, 10, 0, 200);
				
				// These are not yet handled!
				// Content.OscB1Retrig
				// Content.OscB1Wave
				// Content.OscB1Invert
			}

			// OscB2
			if (Content.OscB2Voices != VOICES.VOICES_0) {
				zebra2Preset.OSC4_Tune = ConvertSylenthTuneToZebra(Content.OscB2Octave, Content.OscB2Note, Content.OscB2Fine);
				zebra2Preset.OSC4_WNum = ConvertSylenthWaveToZebra(Content.OscB2Wave);
				zebra2Preset.OSC4_Poly = ConvertSylenthVoicesToZebra(Content.OscB2Voices);
				zebra2Preset.OSC4_Dtun = ConvertValueSylenthToZebra(Content.OscB2Detune, 0, 10, -50, 50);
				zebra2Preset.OSC4_Pan = ConvertValueSylenthToZebra(Content.OscB2Pan, -10, 10, -100, 100);
				zebra2Preset.OSC4_PolW = ConvertValueSylenthToZebra(Content.OscB2Stereo, 0, 10, 0, 100);
				zebra2Preset.OSC4_Phse = ConvertValueSylenthToZebra(Content.OscB2Phase, 0, 360, 0, 100);
				zebra2Preset.OSC4_Vol = ConvertValueSylenthToZebra(Content.OscB2Volume, 0, 10, 0, 200);
				
				// These are not yet handled!
				// Content.OscB2Retrig
				// Content.OscB2Wave
				// Content.OscB2Invert
			}

			// FilterA
			if (Content.FilterAType != FILTERTYPE.Bypass) {
				zebra2Preset.VCF1_Cut = ConvertSylenthFrequencyToZebra(Content.FilterACutoff, FloatToHz.FilterCutoff);
				zebra2Preset.VCF1_Drv = ConvertValueSylenthToZebra(Content.FilterADrive, 0, 10, 0, 100);
				zebra2Preset.VCF1_Res = ConvertValueSylenthToZebra(Content.FilterAReso, 0, 10, 0, 100);

				FILTERDB filterDb = Content.FilterADB;
				Zebra2Preset.FilterType zebraFilter = Zebra2Preset.FilterType.LP_12dB;
				switch (Content.FilterAType) {
					case FILTERTYPE.Lowpass:
						zebraFilter = Zebra2Preset.FilterType.LP_12dB;
						break;
					case FILTERTYPE.Highpass:
						if (filterDb == FILTERDB.DB12) {
							zebraFilter = Zebra2Preset.FilterType.HP_12dB;
						} else {
							zebraFilter = Zebra2Preset.FilterType.HP_24dB;
						}
						break;
					case FILTERTYPE.Bandpass:
						zebraFilter = Zebra2Preset.FilterType.BP_QBand;
						break;
				}
				
				zebra2Preset.VCF1_Typ = (int) zebraFilter;
			}

			// FilterB
			if (Content.FilterBType != FILTERTYPE.Bypass) {			
				zebra2Preset.VCF2_Cut = ConvertSylenthFrequencyToZebra(Content.FilterBCutoff, FloatToHz.FilterCutoff);
				zebra2Preset.VCF2_Drv = ConvertValueSylenthToZebra(Content.FilterBDrive, 0, 10, 0, 100);
				zebra2Preset.VCF2_Res = ConvertValueSylenthToZebra(Content.FilterBReso, 0, 10, 0, 100);

				FILTERDB filterDb = Content.FilterBDB;
				Zebra2Preset.FilterType zebraFilter = Zebra2Preset.FilterType.LP_12dB;
				switch (Content.FilterBType) {
					case FILTERTYPE.Lowpass:
						zebraFilter = Zebra2Preset.FilterType.LP_12dB;
						break;
					case FILTERTYPE.Highpass:
						if (filterDb == FILTERDB.DB12) {
							zebraFilter = Zebra2Preset.FilterType.HP_12dB;
						} else {
							zebraFilter = Zebra2Preset.FilterType.HP_24dB;
						}
						break;
					case FILTERTYPE.Bandpass:
						zebraFilter = Zebra2Preset.FilterType.BP_QBand;
						break;
				}
				
				zebra2Preset.VCF2_Typ = (int) zebraFilter;
			}
			
			return zebra2Preset;
		}
		
		public void Read(string filePath)
		{
			FXP fxp = new FXP();
			fxp.ReadFile(filePath);
			
			byte[] bArray = fxp.chunkDataByteArray;
			BinaryFile bFile = new BinaryFile(bArray, BinaryFile.ByteOrder.LittleEndian);
			
			string presetType = bFile.ReadString(4);
			int UNKNOWN1 = bFile.ReadInt32();
			int fxVersion = bFile.ReadInt32();
			int numPrograms = bFile.ReadInt32();
			int UNKNOWN2 = bFile.ReadInt32();
			
			// if Sylenth 1 preset format
			if (presetType == "1lys") { // '1lys' = 'syl1' backwards
				this.Content = new Syl1PresetContent(bFile);
				
				byte[] presetNameArray = StringUtils.RemoveTrailingBytes(bFile.ReadBytes(36));
				this.PresetName = BinaryFile.ByteArrayToString(presetNameArray);
				
				Console.Out.WriteLine("Finished reading preset file {0} ...", filePath);
			} else {
				Console.Out.WriteLine("Error. The preset file is not a valid Sylenth1 Preset! ({0})", filePath);
			}
		}
		
		public void Write(string filePath)
		{
			throw new NotImplementedException();
		}
	}
}
