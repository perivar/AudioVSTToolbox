using System;
using System.Collections.Generic;
using System.Globalization;

using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Xml;
using System.Xml.Linq;

using System.IO;

using System.Data;
using System.Data.OleDb;

using CommonUtils;

namespace InvestigatePresetFileDump
{
	class Program
	{
		private const bool DO_FXP_WRAP = true;
		private const int FXP_OFFSET = 60; // 60;
		
		public static void Main(string[] args)
		{
			string outputfilename = @"..\..\Sweetscape Template Output.txt";

			// create a writer and open the file
			TextWriter tw = new StreamWriter(outputfilename);
			tw.Write(PresetHeader());
			
			StringWriter stringWriter = new StringWriter();
			string enumSections = ImportXMLFileReturnEnumSections(
				@"..\..\UAD-SSLChannel-output.xml",
				stringWriter
			);
			
			tw.WriteLine(enumSections);
			tw.WriteLine(stringWriter.ToString());
			tw.Write(PresetFooter());
			
			tw.WriteLine(GetValueTables(@"..\..\UAD-SSLChannel-output.xml"));
			
			// close the stream
			tw.Close();
		}
		
		public static string PresetHeader() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("//--------------------------------------");
			sb.AppendLine("//--- 010 Editor Binary Template");
			sb.AppendLine("//");
			sb.AppendLine("// File: <filename>");
			sb.AppendLine("// Author: Per Ivar Nerseth");
			sb.AppendLine("// Revision: 1.0");
			sb.AppendLine("// Purpose: Read a specific VST's preset files (wrapped in a fxp file)");
			sb.AppendLine("//--------------------------------------");
			
			if (DO_FXP_WRAP) {
				sb.AppendLine("");
				sb.AppendLine("typedef struct {");
				sb.AppendLine("    char chunkMagic[4];     // 'CcnK'");
				sb.AppendLine("    long byteSize;          // of this chunk, excl. magic + byteSize");
				sb.AppendLine("    char fxMagic[4];        // 'FxCk', 'FxBk', 'FBCh' or 'FPCh'");
				sb.AppendLine("");
				sb.AppendLine("    long version;");
				sb.AppendLine("    char fxID[4];           // fx unique id");
				sb.AppendLine("    long fxVersion;");
				sb.AppendLine("    long numPrograms;");
				sb.AppendLine("    char name[28];");
				sb.AppendLine("    long chunkSize;");
				sb.AppendLine("} presetHEADER;");
			}
			sb.AppendLine("");
			
			return sb.ToString();
		}
		
		public static string PresetFooter() {
			StringBuilder sb = new StringBuilder();
			
			if (DO_FXP_WRAP) {
				sb.AppendLine("");
				sb.AppendLine("BigEndian();");
				sb.AppendLine("SetBackColor( cLtYellow );");
				sb.AppendLine("presetHEADER header;");
			}

			sb.AppendLine("");
			sb.AppendLine("LittleEndian();");
			sb.AppendLine("SetBackColor( cLtGray );");
			sb.AppendLine("presetCONTENT content;");

			return sb.ToString();
		}
		
		public static string ImportXMLFileReturnEnumSectionsILHarmor(string xmlfilename, TextWriter tw)
		{
			#region nameMap
			Dictionary<string, string> nameMap = new Dictionary<string, string>();
			
			nameMap.Add("AStartingPhase", "A - Starting phase");
			nameMap.Add("APhaseRandomness", "A - Phase randomness");
			nameMap.Add("APredelay", "A - Predelay");
			nameMap.Add("AEnvelopeReleasePo", "A - Envelope release polyphonic scale");
			nameMap.Add("ATimbre1_2Mix", "A - Timbre 1 & 2 mix");
			nameMap.Add("ASubHarmonic1", "A - Sub-harmonic 1");
			nameMap.Add("ASubHarmonic2", "A - Sub-harmonic 2");
			nameMap.Add("ASubHarmonic3", "A - Sub-harmonic 3");
			nameMap.Add("AHarmonicProtection", "A - Harmonic protection");
			nameMap.Add("AClippingThreshold", "A - Clipping threshold");
			nameMap.Add("AFX_DryMix", "A - FX / dry mix");
			nameMap.Add("ATimeDomainVolume", "A - Time domain volume");
			nameMap.Add("AFrequencyDomainEn", "A - Frequency domain envelope volume");
			nameMap.Add("AAutoGain", "A - Auto-gain");
			nameMap.Add("AVelocityToVolume", "A - Velocity to volume envelope attack time");
			nameMap.Add("AVelocityToVolume2", "A - Velocity to volume envelope attack scale");
			nameMap.Add("AReleaseVelocityTo", "A - Release velocity to envelope release scale");

			nameMap.Add("ATremoloDepth", "A - Tremolo depth");
			nameMap.Add("ATremoloSpeed", "A - Tremolo speed");
			nameMap.Add("ATremoloStereoGap", "A - Tremolo stereo gap");

			nameMap.Add("ABlurMix", "A - Blur mix");
			nameMap.Add("ATimeBlurAttack", "A - Time blur attack");
			nameMap.Add("ATimeBlurDecay", "A - Time blur decay");
			nameMap.Add("AHarmonicBlurAmoun", "A - Harmonic blur amount");
			nameMap.Add("AHarmonicBlurBotto", "A - Harmonic blur bottom tension");
			nameMap.Add("AHarmonicBlurTopT", "A - Harmonic blur top tension");

			nameMap.Add("APrismAmount", "A - Prism amount");
			nameMap.Add("AHarmonizerMix", "A - Harmonizer mix");
			nameMap.Add("AHarmonizerWidth", "A - Harmonizer width");
			nameMap.Add("AHarmonizerStrength", "A - Harmonizer strength");
			nameMap.Add("AHarmonizerPattern", "A - Harmonizer pattern - shift - offset");
			nameMap.Add("AHarmonizerPattern2", "A - Harmonizer pattern - shift - step");
			nameMap.Add("AHarmonizerPattern3", "A - Harmonizer pattern - gap - offset");
			nameMap.Add("AHarmonizerPattern4", "A - Harmonizer pattern - gap - step");

			nameMap.Add("AUnisonOrder", "A - Unison order");
			nameMap.Add("AUnisonDistribution", "A - Unison distribution");
			nameMap.Add("AUnisonAlternateDi", "A - Unison alternate distribution");
			nameMap.Add("AUnisonPanning", "A - Unison panning");
			nameMap.Add("AUnisonPitchThickn", "A - Unison pitch thickness");
			nameMap.Add("AUnisonPhase", "A - Unison phase");

			nameMap.Add("AFreqMultiplicator", "A - Freq multiplicator");
			nameMap.Add("AFreqDivider", "A - Freq divider");
			nameMap.Add("AHarmonicDetuningM", "A - Harmonic detuning multiplicator");
			nameMap.Add("AHarmonicDetuningD", "A - Harmonic detuning divider");
			nameMap.Add("APitchArticulatorA", "A - Pitch articulator amount");
			nameMap.Add("APitchVibratoDepth", "A - Pitch vibrato depth");
			nameMap.Add("APitchVibratoSpeed", "A - Pitch vibrato speed");
			nameMap.Add("ALinear_Logarithmi", "A - Linear / logarithmic portamento curve");
			nameMap.Add("AFixed_VariablePo", "A - Fixed / variable portamento / legato time");
			nameMap.Add("APortamento_Legato", "A - Portamento / legato time");
			nameMap.Add("APortamentoPitchLi", "A - Portamento pitch limit");

			nameMap.Add("AAdaptiveFilter1E", "A - Adaptive filter 1 envelope mode");
			nameMap.Add("AFilter1Frequency", "A - Filter 1 frequency");
			nameMap.Add("AFilter1Scale", "A - Filter 1 scale");
			nameMap.Add("AFilter1Width", "A - Filter 1 width");
			nameMap.Add("AKeyToFilter1Fre", "A - Key to filter 1 frequency");
			nameMap.Add("AFilter1EnvelopeA", "A - Filter 1 envelope amount");

			//"A - Filter 1 resonance type";

			nameMap.Add("AFilter1Resonance", "A - Filter 1 resonance / noise amount");
			nameMap.Add("AFilter1Resonance2", "A - Filter 1 resonance / noise scale");
			nameMap.Add("AFilter1Resonance3", "A - Filter 1 resonance width / noise length");
			nameMap.Add("AFilter1Resonance4", "A - Filter 1 resonance offset");
			nameMap.Add("AFilter1Resonance5", "A - Filter 1 resonance adaptive width / free noise");
			nameMap.Add("AFilter1Resonance6", "A - Filter 1 resonance self-oscillation");

			nameMap.Add("AAdaptiveFilter2E", "A - Adaptive filter 2 envelope mode");
			nameMap.Add("AFilter2Frequency", "A - Filter 2 frequency");
			nameMap.Add("AFilter2Scale", "A - Filter 2 scale");
			nameMap.Add("AFilter2Width", "A - Filter 2 width");
			nameMap.Add("AKeyToFilter2Fre", "A - Key to filter 2 frequency");
			nameMap.Add("AFilter2EnvelopeA", "A - Filter 2 envelope amount");

			nameMap.Add("AFilter2Resonance", "A - Filter 2 resonance / noise amount");
			nameMap.Add("AFilter2Resonance2", "A - Filter 2 resonance / noise scale");
			nameMap.Add("AFilter2Resonance3", "A - Filter 2 resonance width / noise length");
			nameMap.Add("AFilter2Resonance4", "A - Filter 2 resonance offset");
			nameMap.Add("AFilter2Resonance5", "A - Filter 2 resonance adaptive width / free noise");

			nameMap.Add("AFilter1_2Mix_p", "A - Filter 1 & 2 mix (parallel - serial)");

			nameMap.Add("APluckDecayLength", "A - Pluck decay length");

			nameMap.Add("APhaserMix", "A - Phaser mix");
			nameMap.Add("APhaserScale", "A - Phaser scale");
			nameMap.Add("APhaserWidth", "A - Phaser width");
			nameMap.Add("APhaserOffset", "A - Phaser offset");
			nameMap.Add("APhaserOffsetMotio", "A - Phaser offset motion speed");
			nameMap.Add("AKeyToPhaserOffse", "A - Key to phaser offset");

			nameMap.Add("AImageTimeOffsetS", "A - Image time offset smoothing");
			nameMap.Add("AImageTimeOffset", "A - Image time offset");
			nameMap.Add("AImageFineSpeed", "A - Image fine speed");
			nameMap.Add("AImageCoarseSpeed", "A - Image coarse speed");
			nameMap.Add("AImageSharpening", "A - Image sharpening");
			nameMap.Add("AImageGainInterpol", "A - Image gain interpolation curve");
			nameMap.Add("AImageGainPixelSc", "A - Image gain pixel scale");
			nameMap.Add("AImageGainMix", "A - Image gain mix");
			nameMap.Add("AImageFreqMode", "A - Image freq mode");
			nameMap.Add("AImageFreqInterpol", "A - Image freq interpolation curve");
			nameMap.Add("AImageFreqPixelSc", "A - Image freq pixel scale");
			nameMap.Add("AFormantShift", "A - Formant shift");
			nameMap.Add("AImageFormantShift", "A - Image formant shift mix");

			// Articulators
			nameMap.Add("APanningArticulat", "A - Panning - Articulator output smoothing");
			nameMap.Add("AVolumeArticulato", "A - Volume - Articulator output smoothing");
			nameMap.Add("AFX_DryMixArti", "A - FX / dry mix - Articulator output smoothing");
			nameMap.Add("ATimbre1_2Mix9", "A - Timbre 1 & 2 mix - Articulator output smoothing");
			nameMap.Add("AFilter1Frequency9", "A - Filter 1 frequency - Articulator output smoothing");
			nameMap.Add("AFilter2Frequency9", "A - Filter 2 frequency - Articulator output smoothing");
			nameMap.Add("AFilter1WidthAr", "A - Filter 1 width - Articulator output smoothing");
			nameMap.Add("AFilter2WidthAr", "A - Filter 2 width - Articulator output smoothing");
			nameMap.Add("AFilter1Resonance31", "A - Filter 1 resonance amount - Articulator output smoothing");
			nameMap.Add("AFilter2Resonance30", "A - Filter 2 resonance amount - Articulator output smoothing");
			nameMap.Add("AFilter1Resonance32", "A - Filter 1 resonance width - Articulator output smoothing");
			nameMap.Add("AFilter2Resonance31", "A - Filter 2 resonance width - Articulator output smoothing");
			nameMap.Add("AFilter1Resonance33", "A - Filter 1 resonance offset - Articulator output smoothing");
			nameMap.Add("AFilter2Resonance32", "A - Filter 2 resonance offset - Articulator output smoothing");
			nameMap.Add("AFilter1_2Mix9", "A - Filter 1 & 2 mix - Articulator output smoothing");
			nameMap.Add("APluckAmountArti", "A - Pluck amount - Articulator output smoothing");
			nameMap.Add("APhaserMixArticu", "A - Phaser mix - Articulator output smoothing");
			nameMap.Add("APhaserWidthArti", "A - Phaser width - Articulator output smoothing");
			nameMap.Add("APhaserOffsetArt", "A - Phaser offset - Articulator output smoothing");
			nameMap.Add("AHarmonizerMixAr", "A - Harmonizer mix - Articulator output smoothing");
			nameMap.Add("AHarmonizerWidth9", "A - Harmonizer width - Articulator output smoothing");
			nameMap.Add("AHarmonicClippingT9", "A - Harmonic clipping threshold - Articulator output smoothing");
			nameMap.Add("AHarmonicBlurAmoun10", "A - Harmonic blur amount - Articulator output smoothing");
			nameMap.Add("APrismAmountArti", "A - Prism amount - Articulator output smoothing");
			nameMap.Add("APitchArticulator", "A - Pitch - Articulator output smoothing");
			nameMap.Add("APitchVibratoDepth10", "A - Pitch vibrato depth - Articulator output smoothing");
			nameMap.Add("AUnisonPitchThickn10", "A - Unison pitch thickness - Articulator output smoothing");
			nameMap.Add("AImageTimeOffset9", "A - Image time offset - Articulator output smoothing");
			nameMap.Add("AImageFineSpeed9", "A - Image fine speed - Articulator output smoothing");
			nameMap.Add("AImageFormantShift10", "A - Image formant shift - Articulator output smoothing");
			nameMap.Add("AImageFreqPixelSc10", "A - Image freq pixel scale - Articulator output smoothing");
			nameMap.Add("AFilter1EnvelopeA2", "A - Filter 1 envelope amount - Articulator output smoothing");
			nameMap.Add("AFilter2EnvelopeA2", "A - Filter 2 envelope amount - Articulator output smoothing");
			nameMap.Add("AGlobalEnvelopeAtt", "A - Global envelope attack time - Articulator output smoothing");
			nameMap.Add("AGlobalEnvelopeAtt2", "A - Global envelope attack scale - Articulator output smoothing");
			nameMap.Add("AGlobalEnvelopeDec", "A - Global envelope decay scale - Articulator output smoothing");
			nameMap.Add("AGlobalEnvelopeSus", "A - Global envelope sustain offset - Articulator output smoothing");
			nameMap.Add("AGlobalEnvelopeRel", "A - Global envelope release scale - Articulator output smoothing");
			nameMap.Add("AGlobalLFOAmount", "A - Global LFO amount - Articulator output smoothing");
			nameMap.Add("AGlobalLFOSpeed", "A - Global LFO speed - Articulator output smoothing");
			nameMap.Add("AGlobalLFOPhase", "A - Global LFO phase - Articulator output smoothing");
			nameMap.Add("AUnisonPhaseArti", "A - Unison phase - Articulator output smoothing");
			nameMap.Add("APredelayArticula", "A - Predelay - Articulator output smoothing");

			// Envelopes
			nameMap.Add("APanningAttackTi", "A - Panning - Attack time scale");
			nameMap.Add("APanningDecayTim", "A - Panning - Decay time scale");
			nameMap.Add("APanningSustainL", "A - Panning - Sustain level offset");
			nameMap.Add("APanningReleaseT", "A - Panning - Release time scale");
			nameMap.Add("APanningLFOSpeed", "A - Panning - LFO speed");
			nameMap.Add("APanningLFOTensi", "A - Panning - LFO tension");
			nameMap.Add("APanningLFOSkew", "A - Panning - LFO skew");
			nameMap.Add("APanningLFOPulse", "A - Panning - LFO pulse width");

			nameMap.Add("AVolumeAttackTim", "A - Volume - Attack time scale");
			nameMap.Add("AVolumeDecayTime", "A - Volume - Decay time scale");
			nameMap.Add("AVolumeSustainLe", "A - Volume - Sustain level offset");
			nameMap.Add("AVolumeReleaseTi", "A - Volume - Release time scale");
			nameMap.Add("AVolumeLFOSpeed", "A - Volume - LFO speed");
			nameMap.Add("AVolumeLFOTensio", "A - Volume - LFO tension");
			nameMap.Add("AVolumeLFOSkew", "A - Volume - LFO skew");
			nameMap.Add("AVolumeLFOPulse", "A - Volume - LFO pulse width");

			nameMap.Add("AFX_DryMixAtta", "A - FX / dry mix - Attack time scale");
			nameMap.Add("AFX_DryMixDeca", "A - FX / dry mix - Decay time scale");
			nameMap.Add("AFX_DryMixSust", "A - FX / dry mix - Sustain level offset");
			nameMap.Add("AFX_DryMixRele", "A - FX / dry mix - Release time scale");
			nameMap.Add("AFX_DryMixLFO", "A - FX / dry mix - LFO speed");
			nameMap.Add("AFX_DryMixLFO2", "A - FX / dry mix - LFO tension");
			nameMap.Add("AFX_DryMixLFO3", "A - FX / dry mix - LFO skew");
			nameMap.Add("AFX_DryMixLFO4", "A - FX / dry mix - LFO pulse width");

			nameMap.Add("ATimbre1_2Mix__2", "A - Timbre 1 & 2 mix - Attack time scale");
			nameMap.Add("ATimbre1_2Mix2", "A - Timbre 1 & 2 mix - Decay time scale");
			nameMap.Add("ATimbre1_2Mix3", "A - Timbre 1 & 2 mix - Sustain level offset");
			nameMap.Add("ATimbre1_2Mix4", "A - Timbre 1 & 2 mix - Release time scale");
			nameMap.Add("ATimbre1_2Mix5", "A - Timbre 1 & 2 mix - LFO speed");
			nameMap.Add("ATimbre1_2Mix6", "A - Timbre 1 & 2 mix - LFO tension");
			nameMap.Add("ATimbre1_2Mix7", "A - Timbre 1 & 2 mix - LFO skew");
			nameMap.Add("ATimbre1_2Mix8", "A - Timbre 1 & 2 mix - LFO pulse width");

			nameMap.Add("AFilter1Frequency__2", "A - Filter 1 frequency - Attack time scale");
			nameMap.Add("AFilter1Frequency2", "A - Filter 1 frequency - Decay time scale");
			nameMap.Add("AFilter1Frequency3", "A - Filter 1 frequency - Sustain level offset");
			nameMap.Add("AFilter1Frequency4", "A - Filter 1 frequency - Release time scale");
			nameMap.Add("AFilter1Frequency5", "A - Filter 1 frequency - LFO speed");
			nameMap.Add("AFilter1Frequency6", "A - Filter 1 frequency - LFO tension");
			nameMap.Add("AFilter1Frequency7", "A - Filter 1 frequency - LFO skew");
			nameMap.Add("AFilter1Frequency8", "A - Filter 1 frequency - LFO pulse width");

			nameMap.Add("AFilter2Frequency__2", "A - Filter 2 frequency - Attack time scale");
			nameMap.Add("AFilter2Frequency2", "A - Filter 2 frequency - Decay time scale");
			nameMap.Add("AFilter2Frequency3", "A - Filter 2 frequency - Sustain level offset");
			nameMap.Add("AFilter2Frequency4", "A - Filter 2 frequency - Release time scale");
			nameMap.Add("AFilter2Frequency5", "A - Filter 2 frequency - LFO speed");
			nameMap.Add("AFilter2Frequency6", "A - Filter 2 frequency - LFO tension");
			nameMap.Add("AFilter2Frequency7", "A - Filter 2 frequency - LFO skew");
			nameMap.Add("AFilter2Frequency8", "A - Filter 2 frequency - LFO pulse width");

			nameMap.Add("AFilter1WidthAt", "A - Filter 1 width - Attack time scale");
			nameMap.Add("AFilter1WidthDe", "A - Filter 1 width - Decay time scale");
			nameMap.Add("AFilter1WidthSu", "A - Filter 1 width - Sustain level offset");
			nameMap.Add("AFilter1WidthRe", "A - Filter 1 width - Release time scale");
			nameMap.Add("AFilter1WidthLF", "A - Filter 1 width - LFO speed");
			nameMap.Add("AFilter1WidthLF2", "A - Filter 1 width - LFO tension");
			nameMap.Add("AFilter1WidthLF3", "A - Filter 1 width - LFO skew");
			nameMap.Add("AFilter1WidthLF4", "A - Filter 1 width - LFO pulse width");

			nameMap.Add("AFilter2WidthAt", "A - Filter 2 width - Attack time scale");
			nameMap.Add("AFilter2WidthDe", "A - Filter 2 width - Decay time scale");
			nameMap.Add("AFilter2WidthSu", "A - Filter 2 width - Sustain level offset");
			nameMap.Add("AFilter2WidthRe", "A - Filter 2 width - Release time scale");
			nameMap.Add("AFilter2WidthLF", "A - Filter 2 width - LFO speed");
			nameMap.Add("AFilter2WidthLF2", "A - Filter 2 width - LFO tension");
			nameMap.Add("AFilter2WidthLF3", "A - Filter 2 width - LFO skew");
			nameMap.Add("AFilter2WidthLF4", "A - Filter 2 width - LFO pulse width");

			nameMap.Add("AFilter1Resonance7", "A - Filter 1 resonance amount - Attack time scale");
			nameMap.Add("AFilter1Resonance8", "A - Filter 1 resonance amount - Decay time scale");
			nameMap.Add("AFilter1Resonance9", "A - Filter 1 resonance amount - Sustain level offset");
			nameMap.Add("AFilter1Resonance10", "A - Filter 1 resonance amount - Release time scale");
			nameMap.Add("AFilter1Resonance11", "A - Filter 1 resonance amount - LFO speed");
			nameMap.Add("AFilter1Resonance12", "A - Filter 1 resonance amount - LFO tension");
			nameMap.Add("AFilter1Resonance13", "A - Filter 1 resonance amount - LFO skew");
			nameMap.Add("AFilter1Resonance14", "A - Filter 1 resonance amount - LFO pulse width");

			nameMap.Add("AFilter2Resonance6", "A - Filter 2 resonance amount - Attack time scale");
			nameMap.Add("AFilter2Resonance7", "A - Filter 2 resonance amount - Decay time scale");
			nameMap.Add("AFilter2Resonance8", "A - Filter 2 resonance amount - Sustain level offset");
			nameMap.Add("AFilter2Resonance9", "A - Filter 2 resonance amount - Release time scale");
			nameMap.Add("AFilter2Resonance10", "A - Filter 2 resonance amount - LFO speed");
			nameMap.Add("AFilter2Resonance11", "A - Filter 2 resonance amount - LFO tension");
			nameMap.Add("AFilter2Resonance12", "A - Filter 2 resonance amount - LFO skew");
			nameMap.Add("AFilter2Resonance13", "A - Filter 2 resonance amount - LFO pulse width");

			nameMap.Add("AFilter1Resonance15", "A - Filter 1 resonance width - Attack time scale");
			nameMap.Add("AFilter1Resonance16", "A - Filter 1 resonance width - Decay time scale");
			nameMap.Add("AFilter1Resonance17", "A - Filter 1 resonance width - Sustain level offset");
			nameMap.Add("AFilter1Resonance18", "A - Filter 1 resonance width - Release time scale");
			nameMap.Add("AFilter1Resonance19", "A - Filter 1 resonance width - LFO speed");
			nameMap.Add("AFilter1Resonance20", "A - Filter 1 resonance width - LFO tension");
			nameMap.Add("AFilter1Resonance21", "A - Filter 1 resonance width - LFO skew");
			nameMap.Add("AFilter1Resonance22", "A - Filter 1 resonance width - LFO pulse width");

			nameMap.Add("AFilter2Resonance14", "A - Filter 2 resonance width - Attack time scale");
			nameMap.Add("AFilter2Resonance15", "A - Filter 2 resonance width - Decay time scale");
			nameMap.Add("AFilter2Resonance16", "A - Filter 2 resonance width - Sustain level offset");
			nameMap.Add("AFilter2Resonance17", "A - Filter 2 resonance width - Release time scale");
			nameMap.Add("AFilter2Resonance18", "A - Filter 2 resonance width - LFO speed");
			nameMap.Add("AFilter2Resonance19", "A - Filter 2 resonance width - LFO tension");
			nameMap.Add("AFilter2Resonance20", "A - Filter 2 resonance width - LFO skew");
			nameMap.Add("AFilter2Resonance21", "A - Filter 2 resonance width - LFO pulse width");

			nameMap.Add("AFilter1Resonance23", "A - Filter 1 resonance offset - Attack time scale");
			nameMap.Add("AFilter1Resonance24", "A - Filter 1 resonance offset - Decay time scale");
			nameMap.Add("AFilter1Resonance25", "A - Filter 1 resonance offset - Sustain level offset");
			nameMap.Add("AFilter1Resonance26", "A - Filter 1 resonance offset - Release time scale");
			nameMap.Add("AFilter1Resonance27", "A - Filter 1 resonance offset - LFO speed");
			nameMap.Add("AFilter1Resonance28", "A - Filter 1 resonance offset - LFO tension");
			nameMap.Add("AFilter1Resonance29", "A - Filter 1 resonance offset - LFO skew");
			nameMap.Add("AFilter1Resonance30", "A - Filter 1 resonance offset - LFO pulse width");

			nameMap.Add("AFilter2Resonance22", "A - Filter 2 resonance offset - Attack time scale");
			nameMap.Add("AFilter2Resonance23", "A - Filter 2 resonance offset - Decay time scale");
			nameMap.Add("AFilter2Resonance24", "A - Filter 2 resonance offset - Sustain level offset");
			nameMap.Add("AFilter2Resonance25", "A - Filter 2 resonance offset - Release time scale");
			nameMap.Add("AFilter2Resonance26", "A - Filter 2 resonance offset - LFO speed");
			nameMap.Add("AFilter2Resonance27", "A - Filter 2 resonance offset - LFO tension");
			nameMap.Add("AFilter2Resonance28", "A - Filter 2 resonance offset - LFO skew");
			nameMap.Add("AFilter2Resonance29", "A - Filter 2 resonance offset - LFO pulse width");

			nameMap.Add("AFilter1_2Mix", "A - Filter 1 & 2 mix - Attack time scale");
			nameMap.Add("AFilter1_2Mix2", "A - Filter 1 & 2 mix - Decay time scale");
			nameMap.Add("AFilter1_2Mix3", "A - Filter 1 & 2 mix - Sustain level offset");
			nameMap.Add("AFilter1_2Mix4", "A - Filter 1 & 2 mix - Release time scale");
			nameMap.Add("AFilter1_2Mix5", "A - Filter 1 & 2 mix - LFO speed");
			nameMap.Add("AFilter1_2Mix6", "A - Filter 1 & 2 mix - LFO tension");
			nameMap.Add("AFilter1_2Mix7", "A - Filter 1 & 2 mix - LFO skew");
			nameMap.Add("AFilter1_2Mix8", "A - Filter 1 & 2 mix - LFO pulse width");

			nameMap.Add("APluckAmountAtta", "A - Pluck amount - Attack time scale");
			nameMap.Add("APluckAmountDeca", "A - Pluck amount - Decay time scale");
			nameMap.Add("APluckAmountSust", "A - Pluck amount - Sustain level offset");
			nameMap.Add("APluckAmountRele", "A - Pluck amount - Release time scale");
			nameMap.Add("APluckAmountLFO", "A - Pluck amount - LFO speed");
			nameMap.Add("APluckAmountLFO2", "A - Pluck amount - LFO tension");
			nameMap.Add("APluckAmountLFO3", "A - Pluck amount - LFO skew");
			nameMap.Add("APluckAmountLFO4", "A - Pluck amount - LFO pulse width");

			nameMap.Add("APhaserMixAttack", "A - Phaser mix - Attack time scale");
			nameMap.Add("APhaserMixDecay", "A - Phaser mix - Decay time scale");
			nameMap.Add("APhaserMixSustai", "A - Phaser mix - Sustain level offset");
			nameMap.Add("APhaserMixReleas", "A - Phaser mix - Release time scale");
			nameMap.Add("APhaserMixLFOSp", "A - Phaser mix - LFO speed");
			nameMap.Add("APhaserMixLFOTe", "A - Phaser mix - LFO tension");
			nameMap.Add("APhaserMixLFOSk", "A - Phaser mix - LFO skew");
			nameMap.Add("APhaserMixLFOPu", "A - Phaser mix - LFO pulse width");

			nameMap.Add("APhaserWidthAtta", "A - Phaser width - Attack time scale");
			nameMap.Add("APhaserWidthDeca", "A - Phaser width - Decay time scale");
			nameMap.Add("APhaserWidthSust", "A - Phaser width - Sustain level offset");
			nameMap.Add("APhaserWidthRele", "A - Phaser width - Release time scale");
			nameMap.Add("APhaserWidthLFO", "A - Phaser width - LFO speed");
			nameMap.Add("APhaserWidthLFO2", "A - Phaser width - LFO tension");
			nameMap.Add("APhaserWidthLFO3", "A - Phaser width - LFO skew");
			nameMap.Add("APhaserWidthLFO4", "A - Phaser width - LFO pulse width");

			nameMap.Add("APhaserOffsetAtt", "A - Phaser offset - Attack time scale");
			nameMap.Add("APhaserOffsetDec", "A - Phaser offset - Decay time scale");
			nameMap.Add("APhaserOffsetSus", "A - Phaser offset - Sustain level offset");
			nameMap.Add("APhaserOffsetRel", "A - Phaser offset - Release time scale");
			nameMap.Add("APhaserOffsetLFO", "A - Phaser offset - LFO speed");
			nameMap.Add("APhaserOffsetLFO2", "A - Phaser offset - LFO tension");
			nameMap.Add("APhaserOffsetLFO3", "A - Phaser offset - LFO skew");
			nameMap.Add("APhaserOffsetLFO4", "A - Phaser offset - LFO pulse width");

			nameMap.Add("AHarmonizerMixAt", "A - Harmonizer mix - Attack time scale");
			nameMap.Add("AHarmonizerMixDe", "A - Harmonizer mix - Decay time scale");
			nameMap.Add("AHarmonizerMixSu", "A - Harmonizer mix - Sustain level offset");
			nameMap.Add("AHarmonizerMixRe", "A - Harmonizer mix - Release time scale");
			nameMap.Add("AHarmonizerMixLF", "A - Harmonizer mix - LFO speed");
			nameMap.Add("AHarmonizerMixLF2", "A - Harmonizer mix - LFO tension");
			nameMap.Add("AHarmonizerMixLF3", "A - Harmonizer mix - LFO skew");
			nameMap.Add("AHarmonizerMixLF4", "A - Harmonizer mix - LFO pulse width");

			nameMap.Add("AHarmonizerWidth__2", "A - Harmonizer width - Attack time scale");
			nameMap.Add("AHarmonizerWidth2", "A - Harmonizer width - Decay time scale");
			nameMap.Add("AHarmonizerWidth3", "A - Harmonizer width - Sustain level offset");
			nameMap.Add("AHarmonizerWidth4", "A - Harmonizer width - Release time scale");
			nameMap.Add("AHarmonizerWidth5", "A - Harmonizer width - LFO speed");
			nameMap.Add("AHarmonizerWidth6", "A - Harmonizer width - LFO tension");
			nameMap.Add("AHarmonizerWidth7", "A - Harmonizer width - LFO skew");
			nameMap.Add("AHarmonizerWidth8", "A - Harmonizer width - LFO pulse width");

			nameMap.Add("AHarmonicClippingT", "A - Harmonic clipping threshold - Attack time scale");
			nameMap.Add("AHarmonicClippingT2", "A - Harmonic clipping threshold - Decay time scale");
			nameMap.Add("AHarmonicClippingT3", "A - Harmonic clipping threshold - Sustain level offset");
			nameMap.Add("AHarmonicClippingT4", "A - Harmonic clipping threshold - Release time scale");
			nameMap.Add("AHarmonicClippingT5", "A - Harmonic clipping threshold - LFO speed");
			nameMap.Add("AHarmonicClippingT6", "A - Harmonic clipping threshold - LFO tension");
			nameMap.Add("AHarmonicClippingT7", "A - Harmonic clipping threshold - LFO skew");
			nameMap.Add("AHarmonicClippingT8", "A - Harmonic clipping threshold - LFO pulse width");

			nameMap.Add("AHarmonicBlurAmoun2", "A - Harmonic blur amount - Attack time scale");
			nameMap.Add("AHarmonicBlurAmoun3", "A - Harmonic blur amount - Decay time scale");
			nameMap.Add("AHarmonicBlurAmoun4", "A - Harmonic blur amount - Sustain level offset");
			nameMap.Add("AHarmonicBlurAmoun5", "A - Harmonic blur amount - Release time scale");
			nameMap.Add("AHarmonicBlurAmoun6", "A - Harmonic blur amount - LFO speed");
			nameMap.Add("AHarmonicBlurAmoun7", "A - Harmonic blur amount - LFO tension");
			nameMap.Add("AHarmonicBlurAmoun8", "A - Harmonic blur amount - LFO skew");
			nameMap.Add("AHarmonicBlurAmoun9", "A - Harmonic blur amount - LFO pulse width");

			nameMap.Add("APrismAmountAtta", "A - Prism amount - Attack time scale");
			nameMap.Add("APrismAmountDeca", "A - Prism amount - Decay time scale");
			nameMap.Add("APrismAmountSust", "A - Prism amount - Sustain level offset");
			nameMap.Add("APrismAmountRele", "A - Prism amount - Release time scale");
			nameMap.Add("APrismAmountLFO", "A - Prism amount - LFO speed");
			nameMap.Add("APrismAmountLFO2", "A - Prism amount - LFO tension");
			nameMap.Add("APrismAmountLFO3", "A - Prism amount - LFO skew");
			nameMap.Add("APrismAmountLFO4", "A - Prism amount - LFO pulse width");

			nameMap.Add("APitchAttackTime", "A - Pitch - Attack time scale");
			nameMap.Add("APitchDecayTime", "A - Pitch - Decay time scale");
			nameMap.Add("APitchSustainLev", "A - Pitch - Sustain level offset");
			nameMap.Add("APitchReleaseTim", "A - Pitch - Release time scale");
			nameMap.Add("APitchLFOSpeed", "A - Pitch - LFO speed");
			nameMap.Add("APitchLFOTension", "A - Pitch - LFO tension");
			nameMap.Add("APitchLFOSkew", "A - Pitch - LFO skew");
			nameMap.Add("APitchLFOPulseW", "A - Pitch - LFO pulse width");

			nameMap.Add("APitchVibratoDepth2", "A - Pitch vibrato depth - Attack time scale");
			nameMap.Add("APitchVibratoDepth3", "A - Pitch vibrato depth - Decay time scale");
			nameMap.Add("APitchVibratoDepth4", "A - Pitch vibrato depth - Sustain level offset");
			nameMap.Add("APitchVibratoDepth5", "A - Pitch vibrato depth - Release time scale");
			nameMap.Add("APitchVibratoDepth6", "A - Pitch vibrato depth - LFO speed");
			nameMap.Add("APitchVibratoDepth7", "A - Pitch vibrato depth - LFO tension");
			nameMap.Add("APitchVibratoDepth8", "A - Pitch vibrato depth - LFO skew");
			nameMap.Add("APitchVibratoDepth9", "A - Pitch vibrato depth - LFO pulse width");

			nameMap.Add("AUnisonPitchThickn2", "A - Unison pitch thickness - Attack time scale");
			nameMap.Add("AUnisonPitchThickn3", "A - Unison pitch thickness - Decay time scale");
			nameMap.Add("AUnisonPitchThickn4", "A - Unison pitch thickness - Sustain level offset");
			nameMap.Add("AUnisonPitchThickn5", "A - Unison pitch thickness - Release time scale");
			nameMap.Add("AUnisonPitchThickn6", "A - Unison pitch thickness - LFO speed");
			nameMap.Add("AUnisonPitchThickn7", "A - Unison pitch thickness - LFO tension");
			nameMap.Add("AUnisonPitchThickn8", "A - Unison pitch thickness - LFO skew");
			nameMap.Add("AUnisonPitchThickn9", "A - Unison pitch thickness - LFO pulse width");

			nameMap.Add("AImageTimeOffset__2", "A - Image time offset - Attack time scale");
			nameMap.Add("AImageTimeOffset2", "A - Image time offset - Decay time scale");
			nameMap.Add("AImageTimeOffset3", "A - Image time offset - Sustain level offset");
			nameMap.Add("AImageTimeOffset4", "A - Image time offset - Release time scale");
			nameMap.Add("AImageTimeOffset5", "A - Image time offset - LFO speed");
			nameMap.Add("AImageTimeOffset6", "A - Image time offset - LFO tension");
			nameMap.Add("AImageTimeOffset7", "A - Image time offset - LFO skew");
			nameMap.Add("AImageTimeOffset8", "A - Image time offset - LFO pulse width");

			nameMap.Add("AImageFineSpeed__2", "A - Image fine speed - Attack time scale");
			nameMap.Add("AImageFineSpeed2", "A - Image fine speed - Decay time scale");
			nameMap.Add("AImageFineSpeed3", "A - Image fine speed - Sustain level offset");
			nameMap.Add("AImageFineSpeed4", "A - Image fine speed - Release time scale");
			nameMap.Add("AImageFineSpeed5", "A - Image fine speed - LFO speed");
			nameMap.Add("AImageFineSpeed6", "A - Image fine speed - LFO tension");
			nameMap.Add("AImageFineSpeed7", "A - Image fine speed - LFO skew");
			nameMap.Add("AImageFineSpeed8", "A - Image fine speed - LFO pulse width");

			nameMap.Add("AImageFormantShift2", "A - Image formant shift - Attack time scale");
			nameMap.Add("AImageFormantShift3", "A - Image formant shift - Decay time scale");
			nameMap.Add("AImageFormantShift4", "A - Image formant shift - Sustain level offset");
			nameMap.Add("AImageFormantShift5", "A - Image formant shift - Release time scale");
			nameMap.Add("AImageFormantShift6", "A - Image formant shift - LFO speed");
			nameMap.Add("AImageFormantShift7", "A - Image formant shift - LFO tension");
			nameMap.Add("AImageFormantShift8", "A - Image formant shift - LFO skew");
			nameMap.Add("AImageFormantShift9", "A - Image formant shift - LFO pulse width");

			nameMap.Add("AImageFreqPixelSc2", "A - Image freq pixel scale - Attack time scale");
			nameMap.Add("AImageFreqPixelSc3", "A - Image freq pixel scale - Decay time scale");
			nameMap.Add("AImageFreqPixelSc4", "A - Image freq pixel scale - Sustain level offset");
			nameMap.Add("AImageFreqPixelSc5", "A - Image freq pixel scale - Release time scale");
			nameMap.Add("AImageFreqPixelSc6", "A - Image freq pixel scale - LFO speed");
			nameMap.Add("AImageFreqPixelSc7", "A - Image freq pixel scale - LFO tension");
			nameMap.Add("AImageFreqPixelSc8", "A - Image freq pixel scale - LFO skew");
			nameMap.Add("AImageFreqPixelSc9", "A - Image freq pixel scale - LFO pulse width");

			nameMap.Add("ModulationX", "Modulation X");
			nameMap.Add("ModulationY", "Modulation Y");
			nameMap.Add("ModulationZ", "Modulation Z");
			nameMap.Add("LegatoPitchLimit", "Legato pitch limit");
			nameMap.Add("LegatoMode", "Legato mode");
			nameMap.Add("PortamentoMode", "Portamento mode");
			nameMap.Add("Velocity_ReleaseToP", "Velocity / release to portamento / legato time");
			nameMap.Add("StrumMode", "Strum mode");
			nameMap.Add("StrumTime", "Strum time");
			nameMap.Add("StrumTension", "Strum tension");
			nameMap.Add("PartA_BMix", "Part A & B mix");
			nameMap.Add("PreFXMainVolume", "Pre-FX main volume");
			nameMap.Add("PostFXMainVolume", "Post-FX main volume");
			nameMap.Add("VelocityToVolume", "Velocity to volume");
			nameMap.Add("MainPitch", "Main pitch");
			nameMap.Add("MainLFOAmount", "Main LFO amount");
			nameMap.Add("InvertArpeggiator_if", "Invert arpeggiator (if any)");

			nameMap.Add("DistortionAmount", "Distortion amount");
			nameMap.Add("DistortionAsymmetry_", "Distortion asymmetry / extra");
			nameMap.Add("DistortionWetVolume", "Distortion wet volume");
			nameMap.Add("DistortionMix", "Distortion mix");
			nameMap.Add("DistortionHighcut", "Distortion highcut");

			nameMap.Add("EnableReverb", "Enable reverb");
			nameMap.Add("ReverbLowcut", "Reverb lowcut");
			nameMap.Add("ReverbHighcut", "Reverb highcut");
			nameMap.Add("ReverbPredelay", "Reverb predelay");
			nameMap.Add("ReverbRoomSize", "Reverb room size");
			nameMap.Add("ReverbDiffusion", "Reverb diffusion");
			nameMap.Add("ReverbDecay", "Reverb decay");
			nameMap.Add("ReverbHighDamping", "Reverb high damping");
			nameMap.Add("ReverbColor", "Reverb color");
			nameMap.Add("ReverbWetVolume", "Reverb wet volume");

			nameMap.Add("ChorusOrder", "Chorus order");
			nameMap.Add("ChorusDepth", "Chorus depth");
			nameMap.Add("ChorusSpeed", "Chorus speed");
			nameMap.Add("ChorusDelay", "Chorus delay");
			nameMap.Add("ChorusSpread", "Chorus spread");
			nameMap.Add("ChorusCross", "Chorus cross");
			nameMap.Add("ChorusMix", "Chorus mix");

			nameMap.Add("EnableDelay", "Enable delay");
			nameMap.Add("DelayFeedbackMode", "Delay feedback mode");
			nameMap.Add("DelayFeedbackLevel", "Delay feedback level");
			nameMap.Add("DelayTime", "Delay time");
			nameMap.Add("DelayTimeStereoOffse", "Delay time stereo offset");
			nameMap.Add("DelayInputVolume", "Delay input volume");
			nameMap.Add("DelayInputPanning", "Delay input panning");
			nameMap.Add("DelayLowcut", "Delay lowcut");
			nameMap.Add("DelayHighcut", "Delay highcut");
			nameMap.Add("DelayFeedbackDamping", "Delay feedback damping");

			nameMap.Add("CompressionAmount", "Compression amount");
			nameMap.Add("CompressionLowBand", "Compression low band");
			nameMap.Add("CompressionMidBand", "Compression mid band");
			#endregion
			
			StringBuilder enumSections = new StringBuilder();
			tw.WriteLine("typedef struct {");
			
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			Dictionary<string, List<byte>> dictionary = new Dictionary<string, List<byte>>();

			// first sort the data
			//http://stackoverflow.com/questions/5603284/linq-to-xml-groupby
			var data = from row in xmlDoc.Descendants("Row")
				orderby Convert.ToInt32(row.Element("IndexInFile").Value) ascending
				select new {
				IndexInFile = Convert.ToInt32(row.Element("IndexInFile").Value),
				ByteValue = Convert.ToByte(row.Element("ByteValue").Value),
				ParameterName = (string)row.Element("ParameterName").Value,
				ParameterNameFormatted = (string)row.Element("ParameterNameFormatted").Value,
				ParameterLabel = (string)row.Element("ParameterLabel").Value,
				ParameterDisplay = (string)row.Element("ParameterDisplay").Value,
			};
			
			// then group the data
			var groupQuery = from row in data
				group row by new {
				IndexInFile = row.IndexInFile,
				ParameterNameFormatted = row.ParameterNameFormatted,
				ParameterName = row.ParameterName
			}
			into groupedTable
				select new
			{
				Keys = groupedTable.Key,  // Each Key contains all the grouped by columns (if multiple groups)
				LowestValue = (double)groupedTable.Min(p => GetDouble( p.ParameterDisplay, Double.MinValue ) ),
				HighestValue = (double)groupedTable.Max(p => GetDouble( p.ParameterDisplay, Double.MinValue ) ),
				SubGroup = groupedTable
			};
			
			// turn into dictionary
			//var groupedDict = groupQuery.ToDictionary(p => p.Keys.IndexInFile, t => t.SubGroup);
			var groupedDict = groupQuery.Select(i => new TemplateEntry {
			                                    	ParameterName = i.Keys.ParameterName,
			                                    	ParameterNameFormatted = i.Keys.ParameterNameFormatted,
			                                    	IndexInFile = i.Keys.IndexInFile,
			                                    	LowestValue = i.LowestValue,
			                                    	HighestValue = i.HighestValue
			                                    }).ToDictionary(p => p.IndexInFile, t => t);
			
			// read in file	with manual enum entries
			TextReader manualFileReader = new StreamReader(@"..\..\manual harmor entries.txt");
			int lineCount = 0;
			string line = null;
			
			// the enums dictionary
			// the dictionary key is a pair where the key is the enum name and the value is the enum byte size
			// the dictionary value is a list of pairs where the key is the enum param name and the value is the enum param value
			Dictionary<KeyValuePair<string, int>, List<KeyValuePair<string, int>>> enums = new Dictionary<KeyValuePair<string, int>, List<KeyValuePair<string, int>>>();
			string enumBeingProcessed = "";
			KeyValuePair<string, int> enumBeingProcessedPair = new KeyValuePair<string, int>();
			while ((line = manualFileReader.ReadLine()) != null) {
				Match indexMatch = Regex.Match(line, @"(^\d+),(\d+)\s+(.+)$");
				Match enumMatch = Regex.Match(line, @"(^[a-zA-Z0-9\s]+)\s+(\d+)$");
				if (indexMatch.Success) {
					// new enum
					string index = indexMatch.Groups[1].Value;
					int indexKey = int.Parse(index);
					string bytes = indexMatch.Groups[2].Value;
					int numBytes = int.Parse(bytes);
					string field = indexMatch.Groups[3].Value;
					string fieldFormatted = StringUtils.ConvertCaseString(field, StringUtils.Case.PascalCase);
					
					enumBeingProcessed = CleanInput(fieldFormatted.ToUpper());
					enumBeingProcessedPair = new KeyValuePair<string, int>(enumBeingProcessed, numBytes);
					
					if (!groupedDict.ContainsKey(indexKey)) {
						groupedDict.Add(indexKey, new TemplateEntry {
						                	ParameterName = field,
						                	ParameterNameFormatted = fieldFormatted,
						                	IndexInFile = indexKey,
						                	IsEnum = true
						                } );

					} else {
						groupedDict[indexKey].ParameterName = field;
						groupedDict[indexKey].ParameterNameFormatted = fieldFormatted;
						groupedDict[indexKey].IsEnum = true;
					}
					
					enums.Add(enumBeingProcessedPair, new List<KeyValuePair<string, int>>() );
				} else if (enumMatch.Success) {
					// new enum value element
					string enumEntry = CleanInput(enumMatch.Groups[1].Value.Trim());
					string enumValue = enumMatch.Groups[2].Value;
					
					enums[enumBeingProcessedPair].Add(new KeyValuePair<string, int>(enumEntry, int.Parse(enumValue)));
				} else {
					// shouldn't get here
				}
				lineCount++;
			}
			
			// create the enum sections
			StringBuilder sb = new StringBuilder();
			foreach (var enumEntry in enums)
			{
				sb.AppendLine(String.Format("typedef enum <{0}> {{", NumberOfBytesToDataType(enumEntry.Key.Value)));
				int count = 1;
				foreach (var enumValueEntry in enumEntry.Value) {
					sb.Append(String.Format("\t{0}_{1}", enumEntry.Key.Key, enumValueEntry.Key).PadRight(20));
					sb.Append(String.Format("= {0}", enumValueEntry.Value));
					if (count < enumEntry.Value.Count) {
						sb.AppendLine(",");
						count++;
					} else {
						sb.AppendLine();
					}
				}
				sb.AppendLine(String.Format("}} {0};\n", enumEntry.Key.Key));
			}
			enumSections = sb;
			
			int low = groupedDict.Keys.Min();
			int high = groupedDict.Keys.Max();
			
			int numberOfBytes = 0;
			int prevFirstIndex = 0;
			int prevLastIndex = 0;
			int prevPrevLastIndex = 0;
			string prevNameFormatted = "";
			string prevName = "";
			bool isPrevEnum = false;
			Dictionary<string, int> processedNames = new Dictionary<string, int>();
			for (int i = low; i <= high; i++) {
				if (groupedDict.ContainsKey(i)) {
					bool isEnum = groupedDict[i].IsEnum;
					string nameFormatted = groupedDict[i].ParameterNameFormatted;
					string name = groupedDict[i].ParameterName;
					
					int firstIndex = groupedDict[i].IndexInFile;
					if (!nameFormatted.Equals(prevNameFormatted)) {
						// we detected a new parameter
						
						// check if we have processed name before
						if (!processedNames.ContainsKey(prevNameFormatted)) {
							processedNames.Add(prevNameFormatted, 1);
						} else {
							processedNames[prevNameFormatted]++;
							prevNameFormatted = String.Format("{0}__{1}", prevNameFormatted, processedNames[prevNameFormatted]);
						}
						
						// convert shortened name to correct name
						if (nameMap.ContainsKey(CleanInput(prevNameFormatted))) {
							prevName = nameMap[CleanInput(prevNameFormatted)];
							prevNameFormatted = CleanInput(StringUtils.ConvertCaseString(prevName, StringUtils.Case.PascalCase));
						}
						
						// skip first entry (since we are just writing one entry after the fact)
						if (i != low) {
							OutputTemplateLine(tw, isPrevEnum, prevName, prevNameFormatted, numberOfBytes, prevFirstIndex, ref prevLastIndex, prevPrevLastIndex);
						}
						
						// reset
						numberOfBytes = 1;
						prevPrevLastIndex = prevLastIndex;
						prevFirstIndex = firstIndex;
						prevNameFormatted = nameFormatted;
						prevName = name;
						isPrevEnum = isEnum;
					} else {
						// continue processing the same field
						numberOfBytes++;
					}
				} else {
					// missing entry
					numberOfBytes++;
				}
			}
			
			// output the last entry
			OutputTemplateLine(tw, isPrevEnum, prevName, prevNameFormatted, numberOfBytes, prevFirstIndex, ref prevLastIndex, prevPrevLastIndex);
			tw.WriteLine("} presetCONTENT;");
			
			return enumSections.ToString();
		}

		public static void OutputTemplateLine(TextWriter tw, bool isPrevEnum, string prevName, string prevNameFormatted, int numberOfBytes, int prevFirstIndex, ref int prevLastIndex, int prevPrevLastIndex) {

			string dataType = "";
			string datatypeAndName = "";
			// determine the dataType and Name
			if (isPrevEnum) {
				// insert enum instead of datatype
				string enumName = CleanInput(prevNameFormatted.ToUpper());
				datatypeAndName = String.Format("\t{0} {1};", enumName, prevNameFormatted).PadRight(55);
				if (numberOfBytes < 4) {
					// TODO: do we have to do something with the fact that it's not 4 bytes (int)?
				} else {
					numberOfBytes = 4;
				}
			} else {
				if (numberOfBytes > 8) {
					//datatypeAndName = String.Format("\tchar {0}[{1}];", CleanInput(prevNameFormatted), numberOfBytes).PadRight(55);

					// force int32 - 4 bytes
					datatypeAndName = String.Format("\tint32 {0};", CleanInput(prevNameFormatted)).PadRight(55);
					numberOfBytes = 4;
				} else {
					dataType = NumberOfBytesToDataType(numberOfBytes);
					datatypeAndName = String.Format("\t{0} {1};", dataType, CleanInput(prevNameFormatted)).PadRight(55);
				}
			}
			
			// set prevLastIndex
			prevLastIndex = prevFirstIndex + numberOfBytes - 1;
			
			// create comment
			string indexAndValueRange = String.Format("// index {0}:{1} = {2} bytes ({3})", prevFirstIndex, prevLastIndex, numberOfBytes, prevName);

			// write seek part (i.e. move pointer to first byte) if the first byte is not
			// directly succeding the last section that was written
			bool prevSkipSeek = false;
			if (!prevSkipSeek && (prevPrevLastIndex + 1 != prevFirstIndex)) {
				string seekEntry = String.Format("\tFSeek( {0} );", prevFirstIndex + FXP_OFFSET).PadRight(55);
				string seekComment = String.Format("// skipped {0} bytes", prevFirstIndex - prevPrevLastIndex - 1);
				tw.WriteLine("{0}{1}", seekEntry, seekComment);
			}
			
			// output
			tw.Write(datatypeAndName);
			tw.WriteLine(indexAndValueRange);

			/*
							tw.Write(CleanInput(prevNameFormatted));
							tw.Write("=\"");
							tw.Write(prevName);
							tw.Write("\";\n");
			 */
			
		}
		
		private static string GetValueTables(string xmlfilename) {
			
			XDocument xmlDoc = XDocument.Load(xmlfilename);

			// find highest and lowest display value
			var sortedValues = from row in xmlDoc.Descendants("Row")
				orderby float.Parse(row.Element("ParameterValue").Value) ascending
				group row by row.Element("ParameterName") into groupedTable
				select groupedTable;

			StringWriter sw = new StringWriter();
			foreach (var group in sortedValues)
				sw.WriteLine("{0} in {1}", group.Distinct().Count(), group.Key);
			
			return sw.ToString();
		}
		
		/* Import XML file output from
		 * */
		public static string ImportXMLFileReturnEnumSections(string xmlfilename, TextWriter tw)
		{
			StringBuilder enumSections = new StringBuilder();
			tw.WriteLine("typedef struct {");
			
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			Dictionary<string, List<byte>> dictionary = new Dictionary<string, List<byte>>();

			// first sort the data
			//http://stackoverflow.com/questions/5603284/linq-to-xml-groupby
			var data = from row in xmlDoc.Descendants("Row")
				orderby Convert.ToInt32(row.Element("IndexInFile").Value) ascending
				select new {
				IndexInFile = Convert.ToInt32(row.Element("IndexInFile").Value),
				ByteValue = Convert.ToByte(row.Element("ByteValue").Value),
				ParameterName = (string)row.Element("ParameterName").Value,
				ParameterNameFormatted = (string)row.Element("ParameterNameFormatted").Value,
				ParameterLabel = (string)row.Element("ParameterLabel").Value,
				ParameterDisplay = (string)row.Element("ParameterDisplay").Value,
				ParameterValue = (string)row.Element("ParameterValue").Value
			};
			
			// then group the data
			var groupQuery = from row in data
				group row by new {
				ParameterNameFormatted = row.ParameterNameFormatted
			}
			into groupedTable
				select new
			{
				//AccountExpirationDate = string.IsNullOrEmpty((string)settings.Element("AccountExpirationDate")) ? (DateTime?)null : DateTime.Parse(settings.Element("AccountExpirationDate").Value)
				//int.Parse(prod.Element("Price").Value)
				// NumComments = (int?)item.Element(slashNamespace + "comments") ?? 0,
				// var intQuery = query.Where( t => int.TryParse( t.Column, out i ) );
				
				Keys = groupedTable.Key,  // Each Key contains all the grouped by columns (if multiple groups)
				LowestValue = (double)groupedTable.Min(p => GetDouble( p.ParameterValue, Double.MinValue ) ),
				HighestValue = (double)groupedTable.Max(p => GetDouble( p.ParameterValue, Double.MinValue ) ),
				LowestDisplay = groupedTable.First().ParameterDisplay,
				HighestDisplay = groupedTable.Last().ParameterDisplay,
				FirstIndex = (int)groupedTable.First().IndexInFile,
				LastIndex = (int)groupedTable.Last().IndexInFile,
				SubGroup = groupedTable
			};

			// turn into list
			var groupedList = groupQuery.ToList();

			int originalLastIndex = 0;
			int prevIndex = 0;
			bool prevSkipSeek = false;
			for (int i = 0; i < groupedList.Count; i++) {
				var curElement = groupedList.ElementAt(i);

				int firstIndex = curElement.FirstIndex;
				int lastIndex = curElement.LastIndex;
				originalLastIndex = lastIndex;
				int numberOfBytes = (lastIndex-firstIndex+1);
				
				// use floats
				string dataType = NumberOfBytesToDataType(ref numberOfBytes, false, true );
				if (numberOfBytes != (lastIndex-firstIndex+1)) {
					// the number of bytes was changed.
					lastIndex = firstIndex + numberOfBytes - 1;
				}
				
				// check if we should convert ushorts to ints and skip the Seek next time around?
				bool skipSeek = false;
				if (i + 1 < groupedList.Count) {
					var nextElement = groupedList.ElementAt(i + 1);
					if ((dataType.Equals("ushort") || dataType.Equals("byte") || dataType.Equals("unknown")) && nextElement.FirstIndex == firstIndex + 4) {
						dataType = "int";
						skipSeek = true;
						lastIndex = firstIndex + 3;
						numberOfBytes = 4;
					}
				}
				
				// write seek part (i.e. move pointer to first byte) if the first byte is not
				// directly succeding the last section that was written
				if (!prevSkipSeek && (prevIndex + 1 != firstIndex)) {
					tw.WriteLine("\tFSeek( {0} );", firstIndex + FXP_OFFSET);
				}
				
				if (curElement.LowestValue != Double.MinValue && curElement.HighestValue != Double.MinValue) {
					double lowVal = (double)curElement.LowestValue;
					double highVal = (double)curElement.HighestValue;
					string name = curElement.Keys.ParameterNameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", dataType, CleanInput(name)).PadRight(35);
					
					// find highest and lowest display value
					var highlowdisplay = from row in xmlDoc.Descendants("Row")
						where (string)row.Element("ParameterNameFormatted") == name
						orderby float.Parse(row.Element("ParameterValue").Value) ascending
						select new {
						IndexInFile = Convert.ToInt32(row.Element("IndexInFile").Value),
						ByteValue = Convert.ToByte(row.Element("ByteValue").Value),
						ParameterName = (string)row.Element("ParameterName").Value,
						ParameterNameFormatted = (string)row.Element("ParameterNameFormatted").Value,
						ParameterLabel = (string)row.Element("ParameterLabel").Value,
						ParameterDisplay = (string)row.Element("ParameterDisplay").Value,
						ParameterValue = (string)row.Element("ParameterValue").Value
					};
					string lowestDisplay = highlowdisplay.First().ParameterDisplay;
					string highestDisplay = highlowdisplay.Last().ParameterDisplay;
					
					string indexAndValueRange = String.Format("// index {0}:{1} = {4} bytes (value range {2} -> {3}) ({5} -> {6})",
					                                          firstIndex,
					                                          originalLastIndex,
					                                          lowVal,
					                                          highVal,
					                                          numberOfBytes,
					                                          lowestDisplay,
					                                          highestDisplay);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexAndValueRange);
				} else {
					// insert enum instead of datatype
					string enumName = curElement.Keys.ParameterNameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", CleanInput(enumName.ToUpper()), enumName).PadRight(35);
					string indexRange = String.Format("// index {0}:{1} = {2} bytes ", firstIndex, originalLastIndex, numberOfBytes);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexRange);
					string enumsection = getEnumSectionXMLFormat(xmlfilename, enumName);
					enumSections.AppendLine(enumsection);
				}
				
				prevIndex = lastIndex;
				prevSkipSeek = skipSeek;
			}
			
			tw.WriteLine("} presetCONTENT;");
			
			return enumSections.ToString();
		}
		
		public static List<string> getUniqueValues(string xmlfilename, string NameFormattedValue)
		{
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			List<string> uniqueList = new List<string>();
			var listQuery = (from row in xmlDoc.Descendants("Row")
			                 where row.Element("NameFormatted").Value == NameFormattedValue
			                 orderby row.Element("DisplayValue").Value descending
			                 select new {
			                 	Index = row.Element("Index").Value,
			                 	DisplayValue = row.Element("DisplayValue").Value,
			                 	ByteValue = row.Element("ByteValue").Value
			                 }
			                ).Distinct();
			foreach (var li in listQuery)
			{
				byte b = Byte.Parse(li.ByteValue);
				string hex = b.ToString("X2");
				
				uniqueList.Add(String.Format("{0}:{1}={2} 0x{3}",li.Index, li.DisplayValue,li.ByteValue, hex));
			}
			return uniqueList;
		}
		
		public static string getEnumSectionXMLFormat(string xmlfilename, string NameFormattedValue)
		{
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			Dictionary<string, List<byte>> dictionary = new Dictionary<string, List<byte>>();
			
			var listQuery = (from row in xmlDoc.Descendants("Row")
			                 where row.Element("ParameterNameFormatted").Value == NameFormattedValue
			                 orderby row.Element("ParameterDisplay").Value ascending
			                 select new {
			                 	Index = row.Element("IndexInFile").Value,
			                 	DisplayValue = row.Element("ParameterDisplay").Value,
			                 	ByteValue = row.Element("ByteValue").Value
			                 }
			                ).Distinct();
			
			foreach (var li in listQuery)
			{
				string key = li.DisplayValue;
				byte value = byte.Parse(li.ByteValue);
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(value);
				}
				else
				{
					List<byte> valueList = new List<byte>();
					valueList.Add(value);
					dictionary.Add(key, valueList);
				}
			}
			
			int numberOfBytes = dictionary.First().Value.Count;
			string datatype = NumberOfBytesToDataType(ref numberOfBytes, true);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(String.Format("typedef enum <{0}> {{", datatype));
			
			//typedef enum <uint> {
			//	Lowpass     = 0x3EAAAAAB,
			//	Highpass    = 0x3F800000,
			//  Bypass      = 0x00000000,
			//  Bandpass    = 0x3F2AAAAB
			//} FILTERTYPE <format=hex>;
			
			int count = 1;
			foreach (var pair in dictionary)
			{
				string key = FixEnumEntries(CleanInput(pair.Key), CleanInput(NameFormattedValue.ToUpper()));
				sb.Append(String.Format("\t{0}", key).PadRight(20));
				sb.Append("= 0x");
				
				byte[] bArray = pair.Value.ToArray();
				Array.Reverse( bArray );
				sb.Append(String.Format("{0}", ByteArrayToString(bArray, numberOfBytes)));
				if (count < dictionary.Count) {
					sb.AppendLine(",");
					count++;
				} else {
					sb.AppendLine();
				}
			}

			sb.AppendLine(String.Format("}} {0} <format=hex>;", CleanInput(NameFormattedValue.ToUpper())));
			return sb.ToString();
		}

		public static string ExtractSortableString(string value) {

			string returnValue = "";
			double result = 0;
			double rootNumber = 0;
			
			// also check if there might be a engineering number there
			// like 0.00 k or 100 m
			Regex r1 = new Regex(@"(-?[0-9]*\.?[0-9]+)\s?([km]?)");
			Match m1 = r1.Match(value);
			if (m1.Success) {
				//string match1 = m1.Groups[0].Value; 	// contains the whole regexp
				string match2 = m1.Groups[1].Value; 	// contains first group
				string match3 = m1.Groups[2].Value;
				
				rootNumber = double.Parse(match2, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
				
				// did it find a metric value like k or m?
				if (!"".Equals(match3)) {
					if ("k".Equals(match3)) {
						result = rootNumber * 1e3;
					} else if ("m".Equals(match3)) {
						result = rootNumber * 1e6;
					}
				} else {
					result = rootNumber;
				}
				returnValue = String.Format("{0:0.##}", result);
			} else {
				returnValue = value;
			}

			return returnValue;
		}

		public static double ExtractDouble(string value, double defaultValue) {

			double result = defaultValue;
			double rootNumber = 0;
			
			// also check if there might be a engineering number there
			// like 0.00 k or 100 m
			Regex r1 = new Regex(@"(-?[0-9]*\.?[0-9]+)\s?([km]?)");
			Match m1 = r1.Match(value);
			if (m1.Success) {
				//string match1 = m1.Groups[0].Value;
				string match2 = m1.Groups[1].Value;
				string match3 = m1.Groups[2].Value;
				
				rootNumber = double.Parse(match2, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
				
				// did it find a metric value like k or m?
				if (!"".Equals(match3)) {
					if ("k".Equals(match3)) {
						result = rootNumber * 1e3;
					} else if ("m".Equals(match3)) {
						result = rootNumber * 1e6;
					}
				} else {
					result = rootNumber;
				}
			}

			return result;
		}
		
		public static double GetDouble(string value, double defaultValue)
		{
			double result;

			//Try parsing in the current culture
			if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
			    //Then try in US english
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
			    //Then in neutral language
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				result = defaultValue;
			}
			
			// special case
			if (result == 3.911555E-07) result = 0;

			return result;
		}
		
		public static string NumberOfBytesToDataType(int numberOfBytes) {
			string dataType = "";
			switch (numberOfBytes) {
				case 1:
					dataType = "byte";
					break;
				case 2:
					dataType = "int16";
					break;
				case 4:
					dataType = "int32";
					break;
				case 8:
					dataType = "int64";
					break;
				default:
					//dataType = numberOfBytes + "bytes";
					dataType = "uint32";
					break;
			}
			return dataType;
		}
		
		public static string NumberOfBytesToDataType(ref int numberOfBytes, bool use4BytesAsInt, bool update = false ) {
			string datatype = "";
			switch (numberOfBytes) {
				case 1:
					datatype = "byte";
					break;
				case 2:
					datatype = "ushort";
					break;
				case 4:
					if (use4BytesAsInt) {
						datatype = "int";
					} else {
						datatype = "float";
					}
					break;
				case 5:
				case 6:
				case 7:
				case 8:
					if (update) numberOfBytes = 8;
					datatype = "uint64";
					break;
					//case 16:
					//	numberOfBytes = 16;
					//	datatype = "16bytes";
					//	break;
				default:
					if (update) numberOfBytes = 4;
					datatype = "uint32";
					break;
			}
			return datatype;
		}
		
		public static string ByteArrayToString(byte[] ba, int numberOfBytes)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			for (int i = 0; i < numberOfBytes && i < ba.Length && i < 8; i++) {
				byte b = ba[i];
				hex.AppendFormat("{0:X2}", b);
			}
			return hex.ToString();
		}

		public static byte[] StringToByteArray(String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}
		
		public static string CleanInput(string strIn)
		{
			// Replace invalid characters with empty strings.
			return Regex.Replace(strIn, @"[^\w]", "_");
		}
		
		public static string FixEnumEntries(string strIn, string name) {
			
			return name + "_" + strIn;
			/*
			char[] chars = strIn.ToCharArray();
			if (Char.IsDigit(chars[0])) {
				// an enum cannot start with a number?!
				strIn = "ENUM_" + strIn;
			}
			return strIn;
			 */
		}
	}
	
	public class TemplateEntry {
		
		public string ParameterName { get; set; }
		public string ParameterNameFormatted { get; set; }
		public int IndexInFile { get; set; }
		public double LowestValue { get; set; }
		public double HighestValue { get; set; }
		public bool IsEnum { get; set; }
		
		public override string ToString()
		{
			if (IsEnum) {
				return string.Format("{0} = {1} [enum]", IndexInFile, ParameterName);
			} else {
				return string.Format("{0} = {1} [{2} - {3}]", IndexInFile, ParameterName, LowestValue, HighestValue);
			}
		}
	}
	
	public class MixedNumbersAndStringsComparer : IComparer<string> {
		public int Compare(string x, string y) {
			double xVal, yVal;

			if(double.TryParse(x, out xVal) && double.TryParse(y, out yVal))
				return xVal.CompareTo(yVal);
			else
				return string.Compare(x, y);
		}
	}
}