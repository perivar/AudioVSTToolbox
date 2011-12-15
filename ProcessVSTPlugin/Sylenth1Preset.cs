using System;
using System.Runtime.InteropServices;

// http://www.developerfusion.com/article/84519/mastering-structs-in-c/
// http://stackoverflow.com/questions/2871/reading-a-c-c-data-structure-in-c-sharp-from-a-byte-array
namespace ProcessVSTPlugin
{
	
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

	// will not work this only works in ranges,
	// i.e. use float instead
	// logarithmic
	public enum COMPRATIO : uint {
		COMPRATIO___1_00_1 = 0x00000000,
		COMPRATIO___1_01_1 = 0x3C6A0EA2,
		COMPRATIO___1_02_1 = 0x3C9C09C1,
		COMPRATIO___1_03_1 = 0x3CEA0EA2,
		// logarithmic ...
		COMPRATIO_100_00_1 = 0x800000
	}

	public enum DISTORTTYPE : uint {
		BitCrush= 0x3F800000,
		Clip    = 0x3F000000,
		Decimate= 0x3F400000,
		FoldBack= 0x3E800000,
		OverDrv = 0x00000000
	}

	public enum FILTERINPUT : uint {
		FILTER_B     = 0x00000000,
		FILTER_A     = 0x00000000,
		FILTER_B_A   = 0x3F000000,
		FILTER_A_B   = 0x3F000000,
		FILTER_None  = 0x3F800000
	}

	public enum FILTERTYPE : uint {
		Highpass= 0x3F800000,
		Bandpass= 0x3F2AAAAB,
		Lowpass = 0x3EAAAAAB,
		Bypass  = 0x00000000,
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
	
	[StructLayout(LayoutKind.Explicit,
	              Size=20)]
	public struct syl1PresetHEADER
	{
		[FieldOffset(0)]
		public char presetType;     // '1lys' = 'syl1' backwards
		[FieldOffset(4)]
		public int UNKNOWN1;
		[FieldOffset(8)]
		public int fxVersion;
		[FieldOffset(12)]
		public int numPrograms;
		[FieldOffset(16)]
		public int UNKNOWN2;
	}
	
	[StructLayout(LayoutKind.Explicit,
	              Size=36)]
	public struct syl1PresetFOOTER
	{
		[FieldOffset(0)]
		public char presetName;
	}
	
	/// <summary>
	/// Description of Sylenth1Preset.
	/// </summary>
	public class Sylenth1Preset
	{
		public Sylenth1Preset()
		{
			// if Sylenth 1 preset format
			/*
			if (fxID == "syl1") {
				LittleEndian();
				SetBackColor( cLtBlue );
				syl1PresetHEADER header;
				SetBackColor( cLtYellow );
				syl1PresetCONTENT content;
				SetBackColor( cLtGray );
				syl1PresetFOOTER footer;
			}
			*/
		}
	}
	
	public class syl1PresetCONTENT {
		
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
		public FILTERINPUT FilterAInput;         // index 198:199
		public float FilterAReso;                // index 200:203 (value range 0 -> 10)
		public FILTERTYPE FilterAType;           // index 204:207
		public float FilterADB;                  // index 210:211 (value range 12 -> 12)
		public float FilterBCutoff;              // index 212:215 (value range 1 -> 21341,28)
		public float FilterBDrive;               // index 216:219 (value range 0 -> 10)
		public FILTERINPUT FilterBInput;         // index 222:223
		public float FilterBReso;                // index 224:227 (value range 0 -> 10)
		public FILTERTYPE FilterBType;           // index 228:231
		public float FilterBDB;                  // index 234:235 (value range 12 -> 24)
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
		public float OscA1Voices;                // index 382:383 (value range 0 -> 8)
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
		public float OscA2Voices;                // index 430:431 (value range 0 -> 8)
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
		public float OscB1Voices;                // index 478:479 (value range 0 -> 8)
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
		public float OscB2Voices;                // index 526:527 (value range 0 -> 8)
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

		public syl1PresetCONTENT() {
			
		}
	}
}
