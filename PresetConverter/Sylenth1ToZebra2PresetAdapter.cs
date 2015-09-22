using System;
using System.Collections.Generic;

using CommonUtils;
using CommonUtils.Audio;

namespace PresetConverter
{
	/// <summary>
	/// An adapter that converts Sylenth 1 presets to Zebra 2 presets
	/// </summary>
	public class Sylenth1ToZebra2PresetAdapter
	{
		#region ZEBRA specific variables
		private const bool USE_XML_FILTER = false; // specificy whether to use VCF or XMF filters
		
		// keep track of nextFreeMatrixSlot
		private int _zebraNextFreeModMatrixSlot = 1;
		
		// keep track of used ModSources
		public List<string> _zebraUsedModSources = new List<string>();
		#endregion
		
		readonly Sylenth1Preset _sylenth1Preset = null;
		
		public Sylenth1ToZebra2PresetAdapter(Sylenth1Preset sylenth1Preset)
		{
			this._sylenth1Preset = sylenth1Preset;
		}
		
		#region Sylenth to Zebra conversion methods
		private static float ConvertSylenthValueToZebra(float storedSylenthValue, float displaySylenthMin, float displaySylenthMax, float zebraMin, float zebraMax) {
			float sylenthDisplayValue = MathUtils.ConvertAndMainainRatio(storedSylenthValue, 0, 1, displaySylenthMin, displaySylenthMax);
			return MathUtils.ConvertAndMainainRatio(sylenthDisplayValue, displaySylenthMin, displaySylenthMax, zebraMin, zebraMax);
		}

		private static float ConvertSylenthModDepthPitchValueToZebra(float storedSylenthValue) {
			// Pitch depth table:
			// Sylenth Display Value	Sylenth Stored Value	Zebra Value
			// -4						0,3						-48
			// -3						0,35					-36
			// -2						0,4						-24
			// -1						0,45					-12
			// 0						0,5						0
			// 1						0,55					12
			// 2						0,6						24
			// 3						0,65					36
			// 4						0,7						48
			
			// The formula for pitch depth is
			// 240x -120 where x is stored Sylenth value
			return (float) (240 * storedSylenthValue - 120);
			
			// TODO: modulation depth for pitch gets wrong when using a LFO that has too much amp
		}
		
		private static void SetZebraEnvelopeFromSylenth(Zebra2Preset z2, float storedSylenthValue, string timeBaseFieldName, string envelopeFieldName) {
			float displaySylenthValue = MathUtils.ConvertAndMainainRatio(storedSylenthValue, 0, 1, 0, 10);
			float sylenthEnvelopeMs = Sylenth1Preset.EnvelopePresetFileValueToMilliseconds(storedSylenthValue);
			
			Zebra2Preset.EnvelopeTimeBase timebase = Zebra2Preset.EnvelopeTimeBase.TIMEBASE_8sX;
			int envValue = (int) Zebra2Preset.MillisecondsToEnvValue(sylenthEnvelopeMs, timebase);
			
			ObjectUtils.SetField(z2, timeBaseFieldName, (int) timebase);
			ObjectUtils.SetField(z2, envelopeFieldName, (float) envValue);
		}
		
		private static void SetZebraArpeggiatorNoteFromSylenth(Zebra2Preset z2, float storedSylenthArpGateValue, Sylenth1Preset.ONOFF arpHoldValue, float storedSylenthArpTransposeValue, float storedSylenthArpVelocityValue, int index) {
			
			// TODO: use storedSylenthArpGateValue for something?
			// range: 0% - 100%
			float arpGatePercentage = MathUtils.ConvertAndMainainRatio(storedSylenthArpGateValue, 0, 1, 0, 100);

			// TODO: use storedSylenthArpVelocityValue for something?
			// range = 0 - 127
			float arpGateVelocity = MathUtils.ConvertAndMainainRatio(storedSylenthArpVelocityValue, 0, 1, 0, 127);
			
			string arpGateFieldName = "VCC_Agte" + index;
			// Gate range: 0 - 5 (2 = Default)
			int arpGateFieldValue = 2;
			if (arpHoldValue == Sylenth1Preset.ONOFF.On) {
				arpGateFieldValue = 5;
			}
			// If the velocity is 0 - use the Sylenth1Preset.VOICES to "zero" this note
			// set Sylenth1Preset.VOICES to 0 as well
			if (arpGateVelocity == 0) {
				arpGateFieldValue = 0;
			}
			ObjectUtils.SetField(z2, arpGateFieldName, arpGateFieldValue);
			
			string arpTransposeFieldName = "VCC_Atrp" + index;
			// Transpose range: -12 - 0 - 12 (0 = Default)
			int arpTransposeFieldValue = 0;
			Sylenth1Preset.ARPTRANSPOSE transpose = Sylenth1Preset.ArpeggiatorTransposeFloatToEnum(storedSylenthArpTransposeValue);
			switch (transpose) {
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_24:
					arpTransposeFieldValue = -12;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_23:
					arpTransposeFieldValue = -11;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_22:
					arpTransposeFieldValue = -10;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_21:
					arpTransposeFieldValue = -9;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_20:
					arpTransposeFieldValue = -8;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_19:
					arpTransposeFieldValue = -7;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_18:
					arpTransposeFieldValue = -6;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_17:
					arpTransposeFieldValue = -5;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_16:
					arpTransposeFieldValue = -4;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_15:
					arpTransposeFieldValue = -3;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_14:
					arpTransposeFieldValue = -2;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_13:
					arpTransposeFieldValue = -1;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_12:
					arpTransposeFieldValue = -12;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_11:
					arpTransposeFieldValue = -11;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_10:
					arpTransposeFieldValue = -10;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_9:
					arpTransposeFieldValue = -9;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_8:
					arpTransposeFieldValue = -8;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_7:
					arpTransposeFieldValue = -7;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_6:
					arpTransposeFieldValue = -6;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_5:
					arpTransposeFieldValue = -5;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_4:
					arpTransposeFieldValue = -4;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_3:
					arpTransposeFieldValue = -3;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_2:
					arpTransposeFieldValue = -2;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.MINUS_1:
					arpTransposeFieldValue = -1;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.ZERO:
					arpTransposeFieldValue = 0;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_1:
					arpTransposeFieldValue = 1;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_2:
					arpTransposeFieldValue = 2;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_3:
					arpTransposeFieldValue = 3;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_4:
					arpTransposeFieldValue = 4;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_5:
					arpTransposeFieldValue = 5;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_6:
					arpTransposeFieldValue = 6;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_7:
					arpTransposeFieldValue = 7;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_8:
					arpTransposeFieldValue = 8;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_9:
					arpTransposeFieldValue = 9;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_10:
					arpTransposeFieldValue = 10;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_11:
					arpTransposeFieldValue = 11;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_12:
					arpTransposeFieldValue = 12;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_13:
					arpTransposeFieldValue = 1;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_14:
					arpTransposeFieldValue = 2;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_15:
					arpTransposeFieldValue = 3;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_16:
					arpTransposeFieldValue = 4;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_17:
					arpTransposeFieldValue = 5;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_18:
					arpTransposeFieldValue = 6;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_19:
					arpTransposeFieldValue = 7;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_20:
					arpTransposeFieldValue = 8;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_21:
					arpTransposeFieldValue = 9;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_22:
					arpTransposeFieldValue = 10;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_23:
					arpTransposeFieldValue = 11;
					break;
				case Sylenth1Preset.ARPTRANSPOSE.PLUSS_24:
					arpTransposeFieldValue = 12;
					break;
			}
			
			ObjectUtils.SetField(z2, arpTransposeFieldName, arpTransposeFieldValue);
			
			string arpVoicesFieldName = "VCC_Avoc" + index;
			// Sylenth1Preset.VOICES range: 0 - 6 (1 = Default)
			int arpVoicesFieldValue = 6;
			// If the velocity is 0 - use the Sylenth1Preset.VOICES to "zero" this note
			if (arpGateVelocity == 0) {
				arpVoicesFieldValue = 0;
			}
			ObjectUtils.SetField(z2, arpVoicesFieldName, arpVoicesFieldValue);
			
			string arpDurationFieldName = "VCC_Amul" + index;
			// Duration range: 1 - 4 (1 = Default)
			// 4 = quarter
			// 3 = eigth dotted
			// 2 = eigth
			// 1 = sixteenth
			const int arpDurationFieldValue = (int)Zebra2Preset.ArpNoteDuration.Sixteenth;
			ObjectUtils.SetField(z2, arpDurationFieldName, arpDurationFieldValue);
			
			string arpStepControlFieldName = "VCC_Amod" + index;
			// Step range:
			// Next = 0,
			// Same = 1,
			// First = 2,
			// Last = 3
			const int arpStepControlFieldValue = (int) Zebra2Preset.ArpNoteStep.Next;
			ObjectUtils.SetField(z2, arpStepControlFieldName, arpStepControlFieldValue);

			string arpStepModAFieldName = "VCC_AMDpt" + index;
			const float arpStepModAFieldValue = 00.00f;
			ObjectUtils.SetField(z2, arpStepModAFieldName, arpStepModAFieldValue);
			
			string arpStepModBFieldName = "VCC_AMDpB" + index;
			const float arpStepModBFieldValue = 00.00f;
			ObjectUtils.SetField(z2, arpStepModBFieldName, arpStepModBFieldValue);
		}
		
		private static int ConvertSylenthVoicesToZebra(Sylenth1Preset.VOICES numberOfVoices) {
			int zebraVoices = (int) Zebra2Preset.OscillatorPoly.single;
			
			switch (numberOfVoices) {
				case Sylenth1Preset.VOICES.VOICES_1:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.single;
					break;
				case Sylenth1Preset.VOICES.VOICES_2:
				case Sylenth1Preset.VOICES.VOICES_3:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.dual;
					break;
				case Sylenth1Preset.VOICES.VOICES_4:
				case Sylenth1Preset.VOICES.VOICES_5:
				case Sylenth1Preset.VOICES.VOICES_6:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.quad;
					break;
				case Sylenth1Preset.VOICES.VOICES_7:
				case Sylenth1Preset.VOICES.VOICES_8:
					zebraVoices = (int) Zebra2Preset.OscillatorPoly.eleven;
					break;
			}
			return zebraVoices;
		}
		
		private static int ConvertSylenthWaveToZebra(Sylenth1Preset.OSCWAVE wave, Sylenth1Preset.ONOFF invert) {
			
			int zebraWave = 1;
			
			switch (wave) {
				case Sylenth1Preset.OSCWAVE.OSC_Sine:
					zebraWave = 1;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_Saw:
					if (invert == Sylenth1Preset.ONOFF.On) {
						zebraWave = 3;
					} else {
						zebraWave = 2;
					}
					break;
				case Sylenth1Preset.OSCWAVE.OSC_Triangle:
					zebraWave = 4;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_Pulse:
					zebraWave = 5;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_HPulse:
					zebraWave = 6;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_QPulse:
					zebraWave = 7;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_TriSaw:
					zebraWave = 8;
					break;
				case Sylenth1Preset.OSCWAVE.OSC_Noise:
					// TODO: special case - noise is not a waveform in zebra but a seperate unit
					zebraWave = 1;
					break;
			}
			return zebraWave;
		}

		private static float ConvertSylenthTuneToZebra(float octave, float note, float fine) {
			
			// sylenth octave + note + fine makes up a zebra Tune?
			float osc_oct = MathUtils.ConvertAndMainainRatio(octave, 0, 1, -3, 3);
			int osc_octave = (int) Math.Floor(osc_oct);
			float osc_note = MathUtils.ConvertAndMainainRatio(note, 0, 1, -7, 7);
			float osc_fine = MathUtils.ConvertAndMainainRatio(fine, 0, 1, -1, 1);
			float osc_tune = (12 * osc_octave) + osc_note + osc_fine;
			return osc_tune;
		}

		private static float ConvertSylenthDetuneToZebra(float osc_detune) {
			// in sylenth detune goes from 0 - 10 where 0 is no detune at all.
			// in zebra detune goes from -50 to 50 where 0 (middle) is no detune at all.
			return ConvertSylenthValueToZebra(osc_detune, 0, 5, 0, 50);
		}
		
		private static float ConvertSylenthFrequencyToZebra(float filterFrequency, float filterControlFrequency, Sylenth1Preset.FloatToHz mode) {
			float actualFrequencyHertz = Sylenth1Preset.ConvertSylenthFrequencyToHertz(filterFrequency, filterControlFrequency, mode);
			int midiNote = Zebra2Preset.FilterFrequencyToMidiNote(actualFrequencyHertz);
			
			// TODO: adjust with two to be on the safe side?!
			return midiNote + 2;
		}
		
		private static int ConvertSylenthDelayTimingsToZebra(float delayTime) {
			Sylenth1Preset.DELAYTIMING delayTiming = Sylenth1Preset.DelayTimeFloatToEnum(delayTime);
			
			Zebra2Preset.DelaySync delaySync = Zebra2Preset.DelaySync.Delay_1_4;
			switch(delayTiming) {
				case Sylenth1Preset.DELAYTIMING.DELAY_1_64:
					delaySync = Zebra2Preset.DelaySync.Delay_1_64;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_32T:
					delaySync = Zebra2Preset.DelaySync.Delay_1_32;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_64D:
					delaySync = Zebra2Preset.DelaySync.Delay_1_64;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_32:
					delaySync = Zebra2Preset.DelaySync.Delay_1_32;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_16T:
					delaySync = Zebra2Preset.DelaySync.Delay_1_16_trip;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_32D:
					delaySync = Zebra2Preset.DelaySync.Delay_1_32_dot;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_16:
					delaySync = Zebra2Preset.DelaySync.Delay_1_16;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_8T:
					delaySync = Zebra2Preset.DelaySync.Delay_1_8_trip;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_16D:
					delaySync = Zebra2Preset.DelaySync.Delay_1_16_dot;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_8:
					delaySync = Zebra2Preset.DelaySync.Delay_1_8;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_4T:
					delaySync = Zebra2Preset.DelaySync.Delay_1_4_trip;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_8D:
					delaySync = Zebra2Preset.DelaySync.Delay_1_8_dot;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_4:
					delaySync = Zebra2Preset.DelaySync.Delay_1_4;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_2T:
					delaySync = Zebra2Preset.DelaySync.Delay_1_2_trip;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_4D:
					delaySync = Zebra2Preset.DelaySync.Delay_1_4_trip;
					break;
				case Sylenth1Preset.DELAYTIMING.DELAY_1_2:
					delaySync = Zebra2Preset.DelaySync.Delay_1_2;
					break;
			}
			return (int) delaySync;
		}

		private static int ConvertSylenthArpSyncTimingsToZebra(float arpSyncTime) {
			Sylenth1Preset.ARPSYNCTIMING arpSyncTiming = Sylenth1Preset.ArpeggiatorSyncTimeFloatToEnum(arpSyncTime);
			
			Zebra2Preset.ArpSync arpSync = Zebra2Preset.ArpSync.Sync_1_4;
			switch(arpSyncTiming) {
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_64:
					arpSync = Zebra2Preset.ArpSync.Sync_1_64;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_32T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_32;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_64D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_64;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_32:
					arpSync = Zebra2Preset.ArpSync.Sync_1_32;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_16T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_16_trip;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_32D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_32_dot;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_16:
					arpSync = Zebra2Preset.ArpSync.Sync_1_16;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_8T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_8_trip;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_16D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_16_dot;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_8:
					arpSync = Zebra2Preset.ArpSync.Sync_1_8;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_4T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_4_trip;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_8D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_8_dot;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_4:
					arpSync = Zebra2Preset.ArpSync.Sync_1_4;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_2T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_2_trip;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_4D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_4_dot;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_2:
					arpSync = Zebra2Preset.ArpSync.Sync_1_2;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_1T:
					arpSync = Zebra2Preset.ArpSync.Sync_1_1_trip;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_2D:
					arpSync = Zebra2Preset.ArpSync.Sync_1_2_dot;
					break;
				case Sylenth1Preset.ARPSYNCTIMING.ARPSYNC_1_1:
					arpSync = Zebra2Preset.ArpSync.Sync_1_1;
					break;
			}
			return (int) arpSync;
		}
		
		private static Zebra2Preset.ModulationSource ConvertSylenthModSourceToZebra(Sylenth1Preset.XMODSOURCE sylenthModSource) {
			Zebra2Preset.ModulationSource zebraModSource = Zebra2Preset.ModulationSource.ModWhl;
			switch (sylenthModSource) {
				case Sylenth1Preset.XMODSOURCE.SOURCE_None:
					// should never get here
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_Velocity:
					zebraModSource = Zebra2Preset.ModulationSource.Velocity;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_ModWheel:
					zebraModSource = Zebra2Preset.ModulationSource.ModWhl;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_KeyTrack:
					zebraModSource = Zebra2Preset.ModulationSource.KeyFol;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_AmpEnv_A:
					zebraModSource = Zebra2Preset.ModulationSource.Env1;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_AmpEnv_B:
					zebraModSource = Zebra2Preset.ModulationSource.Env2;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_1:
					zebraModSource = Zebra2Preset.ModulationSource.Env3;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_2:
					zebraModSource = Zebra2Preset.ModulationSource.Env4;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_LFO_1:
					zebraModSource = Zebra2Preset.ModulationSource.Lfo1;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_LFO_2:
					zebraModSource = Zebra2Preset.ModulationSource.Lfo2;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_Aftertch:
					zebraModSource = Zebra2Preset.ModulationSource.ATouch;
					break;
				case Sylenth1Preset.XMODSOURCE.SOURCE_StepVlty:
					zebraModSource = Zebra2Preset.ModulationSource.Xpress;
					break;
			}
			return zebraModSource;
		}
		
		private static Zebra2Preset.FilterType ConvertSylenthFilterToZebra(Sylenth1Preset.FILTERTYPE filterType, Sylenth1Preset.FILTERDB filterDb, Sylenth1Preset.ONOFF filterCtlWarmDrive) {
			Zebra2Preset.FilterType zebraFilter = Zebra2Preset.FilterType.LP_12dB;
			switch (filterType) {
				case Sylenth1Preset.FILTERTYPE.Lowpass:
					// LP Xcite: 24dB lowpass, with a frequency-dependent exciter as Drive, adding high frequencies.
					// LP Allround: CPU-friendly 24dB lowpass, with a strong resonance and smooth coloration via Drive.
					// LP MidDrive: Boosts mid-range frequencies via Drive, good for leads that can cut through the mix.
					// LP OldDrive: Adds even-numbered harmonics, for a vintage sound bordering on 'cheesy'!
					// LP Vintage:  CPU-friendly analogue-modeled transistor ladder with 24dB rolloff. Sounds nice and old
					// LP 12dB: A 12dB version of LP Allround
					// LP 6dB: A simple lowpass with a very shallow rolloff, non-resonant
					// LP Vintage2: More CPU-intensive version of LP Vintage, capable of self-oscillation.
					
					// LP TN6SVF* TyrellN6's state variable lowpass
					// LP MS2035* Early version of Diva's rev1 BITE lowpass
					// LP MS20SK* Early version of Diva's rev2 BITE lowpass
					
					if (filterDb == Sylenth1Preset.FILTERDB.DB12) {
						zebraFilter = Zebra2Preset.FilterType.LP_12dB;
					} else {
						if (filterCtlWarmDrive == Sylenth1Preset.ONOFF.On) {
							//zebraFilter = Zebra2Preset.FilterType.LP_Xcite;
							//zebraFilter = Zebra2Preset.FilterType.LP_Allround;
							//zebraFilter = Zebra2Preset.FilterType.LP_OldDrive;
							zebraFilter = Zebra2Preset.FilterType.LP_TN6SVF;
						} else {
							//zebraFilter = Zebra2Preset.FilterType.LP_Vintage;
							zebraFilter = Zebra2Preset.FilterType.LP_Vintage2;
						}
					}
					break;
				case Sylenth1Preset.FILTERTYPE.Highpass:
					if (filterDb == Sylenth1Preset.FILTERDB.DB12) {
						// HP 12dB: 12dB version of the HP 24dB Resonant 24dB highpass
						zebraFilter = Zebra2Preset.FilterType.HP_12dB;
					} else {
						// HP 24dB: Resonant 24dB highpass
						zebraFilter = Zebra2Preset.FilterType.HP_24dB;
					}
					break;
				case Sylenth1Preset.FILTERTYPE.Bandpass:
					if (filterDb == Sylenth1Preset.FILTERDB.DB12) {
						// BP RezBand: A resonant 12dB bandpass model
						zebraFilter = Zebra2Preset.FilterType.BP_RezBand;
					} else {
						// BP QBand Another resonant bandpass, with a different character
						//zebraFilter = Zebra2Preset.FilterType.BP_QBand;
						zebraFilter = Zebra2Preset.FilterType.BP_RezBand;
					}
					break;
			}
			return zebraFilter;
		}
		
		private static void SetZebraLFOFromSylenth(Zebra2Preset z2, Sylenth1Preset.LFOWAVE sylenthLFOWave, Sylenth1Preset.ONOFF sylenthLFOFree, float sylenthLFORate, float sylenthLFOGain,
		                                           string LFOWaveFieldName,
		                                           string LFOSyncFieldName,
		                                           string LFORateFieldName,
		                                           string LFOTrigFieldName,
		                                           string LFOPhseFieldName,
		                                           string LFOAmpFieldName,
		                                           string LFOSlewFieldName,
		                                           int bpm=120) {

			Zebra2Preset.LFOWave zebraLFOWave = Zebra2Preset.LFOWave.sine;
			switch (sylenthLFOWave) {
				case Sylenth1Preset.LFOWAVE.LFO_HPulse:
					zebraLFOWave = Zebra2Preset.LFOWave.sqr_hi_lo;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Lorenz:
					zebraLFOWave = Zebra2Preset.LFOWave.sine;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Pulse:
					zebraLFOWave = Zebra2Preset.LFOWave.sqr_hi_lo;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_QPulse:
					zebraLFOWave = Zebra2Preset.LFOWave.sqr_hi_lo;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Ramp:
					zebraLFOWave = Zebra2Preset.LFOWave.saw_down;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Random:
					zebraLFOWave = Zebra2Preset.LFOWave.rand_glide;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Saw:
					zebraLFOWave = Zebra2Preset.LFOWave.saw_down;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Sine:
					zebraLFOWave = Zebra2Preset.LFOWave.sine;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_SmpHold:
					zebraLFOWave = Zebra2Preset.LFOWave.rand_hold;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_Triangle:
					zebraLFOWave = Zebra2Preset.LFOWave.triangle;
					break;
				case Sylenth1Preset.LFOWAVE.LFO_TriSaw:
					zebraLFOWave = Zebra2Preset.LFOWave.saw_up;
					break;
			}
			ObjectUtils.SetField(z2, LFOWaveFieldName, (int) zebraLFOWave);
			
			if (sylenthLFOFree == Sylenth1Preset.ONOFF.On) {
				// use free LFO = hz (0.04 - 192 Hz)
				float lfo1RateHz = Sylenth1Preset.ValueToHz( sylenthLFORate, Sylenth1Preset.FloatToHz.LFORateFree);
				
				// hz = 1 / s
				float msValue = (float) 1 / lfo1RateHz * 1000;
				
				Zebra2Preset.LFOSync lfoSync = Zebra2Preset.LFOSync.SYNC_0_1s;
				int lfoValue = 0;
				Zebra2Preset.MillisecondsToLFOSyncAndValue(msValue, out lfoSync, out lfoValue);
				
				ObjectUtils.SetField(z2, LFOSyncFieldName, (int) lfoSync);
				ObjectUtils.SetField(z2, LFORateFieldName, lfoValue);
			} else {
				// use LFO preset
				Sylenth1Preset.LFOTIMING timing = Sylenth1Preset.LFOTimeFloatToEnum( sylenthLFORate );
				
				// Rate (0.00 - 200.00)
				const float rateNormal = 100.00f;
				const float rateDotted = 87.00f;  // =8*((50/I15)^3) (87 is the closest)
				const float rateTriple = 114.00f; // =8*((50/I13)^3) (114 is the closest)
				
				float zebraLFORate = rateNormal;
				double msValue = 0;
				int rate = 0;
				Zebra2Preset.LFOSync zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_4;
				switch (timing) {
					case Sylenth1Preset.LFOTIMING.LFO_UNKNOWN:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_4;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_8_1D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_8_1;
						zebraLFORate = rateDotted;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_8_1:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_8_1;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_4_1D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_4_1;
						zebraLFORate = rateDotted;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_8_1T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_8_1;
						zebraLFORate = rateTriple;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_4_1:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_4_1;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_2_1D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_2_1;
						zebraLFORate = rateDotted;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_4_1T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_4_1;
						zebraLFORate = rateTriple;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_2_1:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_2_1;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_1D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_1;
						zebraLFORate = rateDotted;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_2_1T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_2_1;
						zebraLFORate = rateTriple;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_1:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_1;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_2D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_2_dot;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_1T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_1_trip;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_2:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_2;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_4D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_4_dot;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_2T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_2_trip;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_4:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_4;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_8D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_8_dot;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_4T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_4_trip;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_8:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_8;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_16D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_16_dot;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_8T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_8_trip;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_16:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_16;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_32D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_32_dot;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_16T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_16_trip;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_32:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_32;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_64D:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_64;
						zebraLFORate = rateDotted;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_32T:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_32;
						zebraLFORate = rateTriple;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_64:
						zebraLFOSync = Zebra2Preset.LFOSync.SYNC_1_64;
						zebraLFORate = rateNormal;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_128D:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_128D, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_64T:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_64T, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_128:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_128, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
						
						// Zebra 2 does not support values lower than 12.5 ms
					case Sylenth1Preset.LFOTIMING.LFO_1_256D:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_256D, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_128T:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_128T, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_256:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_256, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
					case Sylenth1Preset.LFOTIMING.LFO_1_256T:
						msValue = AudioUtils.LFOOrDelayToMilliseconds(AudioUtils.LFOTIMING.LFO_1_256T, bpm);
						Zebra2Preset.MillisecondsToLFOSyncAndValue((float)msValue, out zebraLFOSync, out rate);
						zebraLFORate = rate;
						break;
				}
				
				ObjectUtils.SetField(z2, LFOSyncFieldName, (int) zebraLFOSync);
				ObjectUtils.SetField(z2, LFORateFieldName, zebraLFORate);
			}
			
			ObjectUtils.SetField(z2, LFOTrigFieldName, (int) Zebra2Preset.LFOGlobalTriggering.Trig_off);
			ObjectUtils.SetField(z2, LFOPhseFieldName, 0.0f);
			
			// Get LFO Amp/Gain
			float lfoAmpGain = ConvertSylenthValueToZebra(sylenthLFOGain, 0, 10, 0, 100);

			// TODO: Amp value can never be zero - add a little (2.5 seems best for some sounds like 006 Hardwell from 'Top 100 DJs Sylenth Presets.fxb')
			if (lfoAmpGain == 0) lfoAmpGain = 2.5f;
			
			ObjectUtils.SetField(z2, LFOAmpFieldName, lfoAmpGain);
			
			ObjectUtils.SetField(z2, LFOSlewFieldName, (int) Zebra2Preset.LFOSlew.fast);	// LFO Slew (Slew=1)
		}
		#endregion

		#region Zebra Modulation Methods
		private class ZebraModulationPair {
			public string ZebraModSourceFieldName { set; get; }
			public Object ZebraModSourceFieldValue { set; get; }
			public string ZebraModDepthFieldName { set; get; }
			public Object ZebraModDepthFieldValue { set; get; }
			public bool DoForceModMatrix { set; get; }

			public ZebraModulationPair(string zebraModSourceFieldName,
			                           Object zebraModSourceFieldValue,
			                           string zebraModDepthFieldName,
			                           Object zebraModDepthFieldValue,
			                           bool doForceModMatrix = false) {
				this.ZebraModSourceFieldName = zebraModSourceFieldName;
				this.ZebraModSourceFieldValue = zebraModSourceFieldValue;
				this.ZebraModDepthFieldName = zebraModDepthFieldName;
				this.ZebraModDepthFieldValue = zebraModDepthFieldValue;
				this.DoForceModMatrix = doForceModMatrix;
			}
		}
		
		private void SetZebraModSourcesFromSylenth(Sylenth1Preset.Syl1PresetContent s1, Zebra2Preset z2, Sylenth1Preset.XMODSOURCE sylenthModSource, Sylenth1Preset.YMODDEST sylenthModDestination, float sylenthXModDestAm,
		                                           Dictionary<string,List<string>> processedModulationSourceAndDest) {
			
			if (sylenthModSource != Sylenth1Preset.XMODSOURCE.SOURCE_None && sylenthModDestination != Sylenth1Preset.YMODDEST.None) {
				Logger.DoDebug(String.Format("Processing Sylenth1 Modulation Source: {0}, Destination: {1}, Preset-file Depth: {2}", sylenthModSource, sylenthModDestination, sylenthXModDestAm));
				
				Zebra2Preset.ModulationSource zebraModSource = ConvertSylenthModSourceToZebra(sylenthModSource);
				
				string zebraModSourceFieldName = null;
				Object zebraModSourceFieldValue = null;
				string zebraModDepthFieldName = null;
				Object zebraModDepthFieldValue = null;
				
				// use List to store modulation pairs
				var modPairs = new List<ZebraModulationPair>();

				// Filters have real range is -150 to 150)
				// Increase the Cutoff modulation depth values slightly (20% seem to work better)
				//float CutoffModDepthValue = (float) (ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150) * 1.20);
				float CutoffModDepthValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);
				
				switch (sylenthModDestination) {
					case Sylenth1Preset.YMODDEST.None:
						// should never get here
						break;
						
						// Oscillators
					case Sylenth1Preset.YMODDEST.Volume_A:
						// Volume A:
						zebraModSourceFieldName = "OSC1_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC1_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC2_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						break;
					case Sylenth1Preset.YMODDEST.Volume_B:
						// Volume B:
						zebraModSourceFieldName = "OSC3_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC3_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC4_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.VolumeAB:
						// Volume A:
						zebraModSourceFieldName = "OSC1_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC1_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC2_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						// Volume B:
						zebraModSourceFieldName = "OSC3_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC3_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_VolSc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC4_VolDt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						break;
					case Sylenth1Preset.YMODDEST.Pitch_A:
						// TMSrc = Tune Modulation Source
						// TMDpt = Tune Modulation Depth
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = ConvertSylenthModDepthPitchValueToZebra(sylenthXModDestAm);

						zebraModSourceFieldName = "OSC1_TMSrc";
						zebraModDepthFieldName = "OSC1_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_TMSrc";
						zebraModDepthFieldName = "OSC2_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Pitch_B:
						// TMSrc = Tune Modulation Source
						// TMDpt = Tune Modulation Depth
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = ConvertSylenthModDepthPitchValueToZebra(sylenthXModDestAm);

						zebraModSourceFieldName = "OSC3_TMSrc";
						zebraModDepthFieldName = "OSC3_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_TMSrc";
						zebraModDepthFieldName = "OSC4_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Pitch_AB:
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = ConvertSylenthModDepthPitchValueToZebra(sylenthXModDestAm);
						
						zebraModSourceFieldName = "OSC1_TMSrc";
						zebraModDepthFieldName = "OSC1_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_TMSrc";
						zebraModDepthFieldName = "OSC2_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC3_TMSrc";
						zebraModDepthFieldName = "OSC3_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_TMSrc";
						zebraModDepthFieldName = "OSC4_TMDpt";
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Phase_A:
						// PhsMSrc = PhaseModSrc
						// PhsMDpt = PhaseModDepth
						zebraModSourceFieldName = "OSC1_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC1_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC2_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Phase_B:
						// PhsMSrc = PhaseModSrc
						// PhsMDpt = PhaseModDepth
						zebraModSourceFieldName = "OSC3_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC3_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC4_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Phase_AB:
						// PhsMSrc = PhaseModSrc
						// PhsMDpt = PhaseModDepth
						zebraModSourceFieldName = "OSC1_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC1_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC2_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC2_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC3_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC3_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "OSC4_PhsMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "OSC4_PhsMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -50, 50);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Pan_A:
						// Pan A:
						zebraModSourceFieldName = "VCA1_PanMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_PanMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Pan Mod Dpt1
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Pan_B:
						// Pan B:
						zebraModSourceFieldName = "VCA1_PanMS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_PanMD2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Pan Mod Dpt2
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Pan_AB:
						// Pan A and B:
						zebraModSourceFieldName = "VCA1_PanMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_PanMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Pan Mod Dpt1
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						zebraModSourceFieldName = "VCA1_PanMS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_PanMD2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Pan Mod Dpt2
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;

						// Filters
						// The target for a '...' knob is usually the parameter to its immediate left.
						// The two filter modules (VCF and XMF) are exceptions to the above rule.
						// By all appearances, the two unlabeled knobs should affect Resonance
						// – in fact they both modulate Cutoff.
						// FS1 = Modsource1
						// FM1 = ModDepth1 (-150 - 150)
						// FS2 = Modsource2
						// FM2 = ModDepth2 (-150 - 150)
					case Sylenth1Preset.YMODDEST.Cutoff_A:
						zebraModSourceFieldName = "VCF1_FS1";
						zebraModDepthFieldName = "VCF1_FM1";
						if (_zebraUsedModSources.Contains(zebraModSourceFieldName)) {
							// already used the first cutoff knob
							zebraModSourceFieldName = "VCF1_FS2";
							zebraModDepthFieldName = "VCF1_FM2";
						}
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = CutoffModDepthValue;
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Cutoff_B:
						zebraModSourceFieldName = "VCF2_FS1";
						zebraModDepthFieldName = "VCF2_FM1";
						if (_zebraUsedModSources.Contains(zebraModSourceFieldName)) {
							// already used the first cutoff knob
							zebraModSourceFieldName = "VCF2_FS2";
							zebraModDepthFieldName = "VCF2_FM2";
						}
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = CutoffModDepthValue;
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.CutoffAB:
						zebraModSourceFieldName = "VCF1_FS1";
						zebraModDepthFieldName = "VCF1_FM1";
						if (_zebraUsedModSources.Contains(zebraModSourceFieldName)) {
							// already used the first cutoff knob
							zebraModSourceFieldName = "VCF1_FS2";
							zebraModDepthFieldName = "VCF1_FM2";
						}
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = CutoffModDepthValue;
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						zebraModSourceFieldName = "VCF2_FS1";
						zebraModDepthFieldName = "VCF2_FM1";
						if (_zebraUsedModSources.Contains(zebraModSourceFieldName)) {
							// already used the first cutoff knob
							zebraModSourceFieldName = "VCF2_FS2";
							zebraModDepthFieldName = "VCF2_FM2";
						}
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldValue = CutoffModDepthValue;
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.Reso_A:
						zebraModSourceFieldName = "VCF1_FS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCF1_FM2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);
						
						// Force using the Mod Matrix since resonance cannot be modulated by any other means
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));

						break;
					case Sylenth1Preset.YMODDEST.Reso_B:
						zebraModSourceFieldName = "VCF2_FS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCF2_FM2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);
						
						// Force using the Mod Matrix since resonance cannot be modulated by any other means
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));

						break;
					case Sylenth1Preset.YMODDEST.Reso_AB:
						zebraModSourceFieldName = "VCF1_FS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCF1_FM2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);
						
						// Force using the Mod Matrix since resonance cannot be modulated by any other means
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));

						zebraModSourceFieldName = "VCF2_FS2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCF2_FM2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);

						// Force using the Mod Matrix since resonance cannot be modulated by any other means
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));

						break;
						
						// Misc
					case Sylenth1Preset.YMODDEST.PhsrFreq:
						zebraModSourceFieldName = "ModFX1_Sped";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "DUMMY";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -150, 150);

						// Force using the Mod Matrix since the phaser cannot be modulated by any other means
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));
						
						break;
						
					case Sylenth1Preset.YMODDEST.Mix_A:
						// Volume A:
						zebraModSourceFieldName = "VCA1_ModSrc1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_ModDpt1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));

						// Force using the Mod Matrix since volume cannot be modulated by any other means
						_zebraUsedModSources.Add(zebraModSourceFieldName);
						
						break;
					case Sylenth1Preset.YMODDEST.Mix_B:
						// Volume B:
						zebraModSourceFieldName = "VCA1_ModSrc2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_ModDpt2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Mod Depth2
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						// Force using the Mod Matrix since volume cannot be modulated by any other means
						_zebraUsedModSources.Add(zebraModSourceFieldName);
						
						break;
					case Sylenth1Preset.YMODDEST.Mix_AB:
						// Volume A and B:
						zebraModSourceFieldName = "VCA1_ModSrc1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_ModDpt1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Mod Depth1
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						// Force using the Mod Matrix since volume cannot be modulated by any other means
						_zebraUsedModSources.Add(zebraModSourceFieldName);

						zebraModSourceFieldName = "VCA1_ModSrc2";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "VCA1_ModDpt2";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);			// Mod Depth2
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						// Force using the Mod Matrix since volume cannot be modulated by any other means
						_zebraUsedModSources.Add(zebraModSourceFieldName);

						break;

					case Sylenth1Preset.YMODDEST.LFO1Rate:
						// FMS = FreqMod Src1=none
						// FMD = FreqMod Dpt (-100.00 - 100.00)
						zebraModSourceFieldName = "LFO1_FMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "LFO1_FMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						
						// Force using the Mod Matrix since e.g. LFO1 rate or gain cannot be modulated by any other LFO
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));
						
						break;
					case Sylenth1Preset.YMODDEST.LFO1Gain:
						// DMS = DepthMod Src1=none
						// DMD = DepthMod Dpt1 (0.00 - 100.00)
						zebraModSourceFieldName = "LFO1_DMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "LFO1_DMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, 0, 100);
						
						// Force using the Mod Matrix since e.g. LFO1 rate or gain cannot be modulated by any other LFO
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));
						
						break;
					case Sylenth1Preset.YMODDEST.LFO2Rate:
						// FMS = FreqMod Src1=none
						// FMD = FreqMod Dpt (-100.00 - 100.00)
						zebraModSourceFieldName = "LFO2_FMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "LFO2_FMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.LFO2Gain:
						// DMS = DepthMod Src1=none
						// DMD = DepthMod Dpt1 (0.00 - 100.00)
						zebraModSourceFieldName = "LFO2_DMS1";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "LFO2_DMD1";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, 0, 100);
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue));
						
						break;
					case Sylenth1Preset.YMODDEST.DistAmnt:
						// DMSrc = D_ModSrc=none
						// DMDpt = D_ModDepth
						zebraModSourceFieldName = "Dist3_DMSrc";
						zebraModSourceFieldValue = (int) zebraModSource;
						zebraModDepthFieldName = "Dist3_DMDpt";
						zebraModDepthFieldValue = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
						
						// Force using the Mod Matrix since it's impossible to modulate distortion directly on the distortion channel
						modPairs.Add(new ZebraModulationPair(zebraModSourceFieldName, zebraModSourceFieldValue, zebraModDepthFieldName, zebraModDepthFieldValue, true));
						break;
				}

				// Use var keyword to enumerate dictionary
				foreach (ZebraModulationPair modElement in modPairs)
				{
					SetZebraModSourceFromSylenth(s1, z2, modElement.ZebraModSourceFieldName, modElement.ZebraModSourceFieldValue, modElement.ZebraModDepthFieldName, modElement.ZebraModDepthFieldValue, modElement.DoForceModMatrix, sylenthModSource, sylenthModDestination, sylenthXModDestAm, processedModulationSourceAndDest);
				}
			}
		}
		
		private void SetZebraModSourceFromSylenth(Sylenth1Preset.Syl1PresetContent s1,
		                                          Zebra2Preset z2,
		                                          string zebraModSourceFieldName,
		                                          Object zebraModSourceFieldValue,
		                                          string zebraModDepthFieldName,
		                                          Object zebraModDepthFieldValue,
		                                          bool doForceModMatrix,
		                                          Sylenth1Preset.XMODSOURCE sylenthModSource,
		                                          Sylenth1Preset.YMODDEST sylenthModDestination,
		                                          float sylenthXModDestAm,
		                                          Dictionary<string,List<string>> processedModulationSourceAndDest) {
			
			if (zebraModSourceFieldName == null || zebraModDepthFieldName == null) {
				return;
			}
			
			if (!doForceModMatrix && !_zebraUsedModSources.Contains(zebraModSourceFieldName)) {
				// setting modulation source directly on the source (not via the mod matrix)
				Logger.DoDebug(String.Format("Using the Zebra2 Modulation Source Slot: {0}={1} {2}={3}", zebraModSourceFieldName, Enum.GetName(typeof(Zebra2Preset.ModulationSource), zebraModSourceFieldValue), zebraModDepthFieldName, zebraModDepthFieldValue));
				_zebraUsedModSources.Add(zebraModSourceFieldName);
				ObjectUtils.SetField(z2, zebraModSourceFieldName, zebraModSourceFieldValue);
				ObjectUtils.SetField(z2, zebraModDepthFieldName, zebraModDepthFieldValue);
			} else {
				if (doForceModMatrix) {
					// Force use mod matrix
					Logger.DoDebug(String.Format("Forcing the usage of the mod matrix. {0}={1} {2}={3}", zebraModSourceFieldName, Enum.GetName(typeof(Zebra2Preset.ModulationSource), zebraModSourceFieldValue), zebraModDepthFieldName, zebraModDepthFieldValue));
				} else {
					// Already used the mod source, must revert to using the mod matrix
					Logger.DoDebug(String.Format("Already used up the Zebra2 Modulation Source Slot ({0}). Trying to use the Mod Matrix instead. {0}={1} {2}={3}", zebraModSourceFieldName, Enum.GetName(typeof(Zebra2Preset.ModulationSource), zebraModSourceFieldValue), zebraModDepthFieldName, zebraModDepthFieldValue));
				}
				
				SetZebraModMatrixFromSylenth(s1,
				                             z2,
				                             sylenthModSource,
				                             sylenthModDestination,
				                             sylenthXModDestAm,
				                             processedModulationSourceAndDest);
			}
		}
		
		private void SetZebraModMatrixFromSylenth(Sylenth1Preset.Syl1PresetContent s1,
		                                          Zebra2Preset z2,
		                                          Sylenth1Preset.XMODSOURCE sylenthModSource,
		                                          Sylenth1Preset.YMODDEST sylenthModDestination,
		                                          float sylenthXModDestAm,
		                                          IDictionary<string, List<string>> processedModulationSourceAndDest) {

			// check if we have already processed this exact Sylenth1Preset.XMODSOURCE and Sylenth1Preset.YMODDEST
			string currentSylenthModSourceAndDest = String.Format("{0}_{1}", sylenthModSource, sylenthModDestination);
			var zebraUsedModMatrixSlots = new List<string>();
			if (processedModulationSourceAndDest.ContainsKey(currentSylenthModSourceAndDest)) {
				// TODO: Sometimes "double booking" of source and destination is used to increase the effect of a destination
				// so increase the depth ?!
				// example: 4 BASS Fidget from "Adam Van Baker Sylenth1 Soundset Part 2.fxb"
				if ( (s1.YModLFO1Dest1 != Sylenth1Preset.YMODDEST.None && s1.YModLFO1Dest1 == s1.YModLFO1Dest2)
				    || (s1.YModLFO2Dest1 != Sylenth1Preset.YMODDEST.None && s1.YModLFO2Dest1 == s1.YModLFO2Dest2)
				    || (s1.YModEnv1Dest1 != Sylenth1Preset.YMODDEST.None && s1.YModEnv1Dest1 == s1.YModEnv1Dest2)
				    || (s1.YModEnv2Dest1 != Sylenth1Preset.YMODDEST.None && s1.YModEnv2Dest1 == s1.YModEnv2Dest2)
				   ) {
					Logger.DoDebug(String.Format("It seems like two modulation sources are duplicated. Increasing the zebra modulation depth to encompass this."));
					
					List<string> myList = processedModulationSourceAndDest[currentSylenthModSourceAndDest];
					foreach(string fieldName in myList)
					{
						if(fieldName.StartsWith("PCore_MMD"))
						{
							// increase the value
							ObjectUtils.SetField(z2, fieldName, ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, 90, 90));
						}
					}
					return;
				} else {
					Logger.DoDebug(String.Format("Sylenth1 Modulation Source has already been set in the Mod Matrix. Skipping! {0}_{1}!", sylenthModSource, sylenthModDestination));
					return;
				}
			} else {
				processedModulationSourceAndDest.Add(currentSylenthModSourceAndDest, zebraUsedModMatrixSlots);
			}
			
			// use the _zebraNextFreeModMatrixSlot to keep track of the slot usage
			if (_zebraNextFreeModMatrixSlot > 12) return;
			
			int zebraNumberOfSlotsUsed = 0;
			if (sylenthModSource != Sylenth1Preset.XMODSOURCE.SOURCE_None && sylenthModDestination != Sylenth1Preset.YMODDEST.None) {
				const string zebraModMatrixDepthPrefix = "PCore_MMD";
				const string zebraModMatrixSourcePrefix = "PCore_MMS";
				const string zebraModMatrixTargetPrefix = "PCore_MMT";
				const string zebraModMatrixViaSourcePrefix = "PCore_MMVS"; // not used
				const string zebraModMatrixViaSourceDepthPrefix = "PCore_MMVD"; // not used

				switch (sylenthModDestination) {
					case Sylenth1Preset.YMODDEST.None:
						// should never get here
						break;
						
						// Oscillators
					case Sylenth1Preset.YMODDEST.Volume_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Vol", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Vol", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Volume_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC3:Vol", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC4:Vol", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.VolumeAB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Vol", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Vol", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 3, "OSC3:Vol", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 4, "OSC4:Vol", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 4;
						break;
					case Sylenth1Preset.YMODDEST.Pitch_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Tune", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Tune", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Pitch_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC3:Tune", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC4:Tune", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Pitch_AB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Tune", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Tune", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 3, "OSC3:Tune", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 4, "OSC4:Tune", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 4;
						break;
					case Sylenth1Preset.YMODDEST.Phase_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Phse", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Phse", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Phase_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC3:Phse", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC4:Phse", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Phase_AB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "OSC1:Phse", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "OSC2:Phse", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 3, "OSC3:Phse", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 4, "OSC4:Phse", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 4;
						break;
					case Sylenth1Preset.YMODDEST.Pan_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Pan1", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Pan_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Pan2", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Pan_AB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Pan1", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "VCA1:Pan2", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;

						// Filters
					case Sylenth1Preset.YMODDEST.Cutoff_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF1:Cut", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Cutoff_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF2:Cut", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.CutoffAB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF1:Cut", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "VCF2:Cut", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;
					case Sylenth1Preset.YMODDEST.Reso_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF1:Res", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Reso_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF2:Res", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Reso_AB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCF1:Res", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "VCF2:Res", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;

						// Misc
					case Sylenth1Preset.YMODDEST.PhsrFreq:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "ModFX1:Sped", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;

					case Sylenth1Preset.YMODDEST.Mix_A:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Vol1", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Mix_B:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Vol2", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.Mix_AB:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "VCA1:Vol1", zebraUsedModMatrixSlots);
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 2, "VCA1:Vol2", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 2;
						break;

					case Sylenth1Preset.YMODDEST.LFO1Rate:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "LFO1:Rate", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.LFO1Gain:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "LFO1:Amp", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.LFO2Rate:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "LFO2:Rate", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.LFO2Gain:
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "LFO2:Amp", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
					case Sylenth1Preset.YMODDEST.DistAmnt:
						// TODO: should we use Dist3:Input or Dist3:Output ?
						SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixTargetPrefix, _zebraNextFreeModMatrixSlot, 1, "Dist3:Input", zebraUsedModMatrixSlots);
						zebraNumberOfSlotsUsed = 1;
						break;
				}

				
				for (int i = 1; i <= zebraNumberOfSlotsUsed; i++) {
					switch (sylenthModSource) {
						case Sylenth1Preset.XMODSOURCE.SOURCE_None:
							// should never get here
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_Velocity:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Velocity, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_ModWheel:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.ModWhl, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_KeyTrack:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.KeyFol, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_AmpEnv_A:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Env1, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_AmpEnv_B:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Env2, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_1:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Env3, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_2:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Env4, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_LFO_1:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Lfo1, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_LFO_2:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Lfo2, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_Aftertch:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.ATouch, zebraUsedModMatrixSlots);
							break;
						case Sylenth1Preset.XMODSOURCE.SOURCE_StepVlty:
							SetZebraModMatrixElementFromSylenth(z2, sylenthModSource, sylenthModDestination, zebraModMatrixSourcePrefix, _zebraNextFreeModMatrixSlot, i, (int) Zebra2Preset.ModulationSource.Velocity, zebraUsedModMatrixSlots);
							break;
					}
					
					// set the modulation depth amount
					// have to constrain the amount due to too high converstion (real zebra range is 0 - 100)
					float matrixDepth = ConvertSylenthValueToZebra(sylenthXModDestAm, -10, 10, -100, 100);
					SetZebraModMatrixElementFromSylenth(z2,
					                                    sylenthModSource,
					                                    sylenthModDestination,
					                                    zebraModMatrixDepthPrefix,
					                                    _zebraNextFreeModMatrixSlot,
					                                    i,
					                                    matrixDepth,
					                                    zebraUsedModMatrixSlots
					                                   );
				}
				_zebraNextFreeModMatrixSlot += zebraNumberOfSlotsUsed;
			}
		}
		
		private void SetZebraModMatrixElementFromSylenth(Zebra2Preset z2, Sylenth1Preset.XMODSOURCE sylenthModSource, Sylenth1Preset.YMODDEST sylenthModDestination,
		                                                 string fieldNamePrefix, int startMatrixSlot, int slotIndex, Object fieldValue,
		                                                 List<string> zebraUsedModMatrixSlots) {
			
			int fieldIndex = startMatrixSlot + slotIndex -1;
			string fieldName = String.Format("{0}{1}", fieldNamePrefix,  fieldIndex);
			if (fieldIndex > 12) {
				Console.Out.WriteLine("Warning! Not enough matrix slots available. Discarding matrix slot: {0}={1} !", fieldName, fieldValue);
				Logger.DoDebug(String.Format("Warning! Not enough matrix slots available. Discarding matrix slot: {0}={1} !", fieldName, fieldValue));
				return;
			}
			Logger.DoDebug(String.Format("Setting Zebra2 Mod Matrix Element: {0}->{1} {2}={3}" , sylenthModSource, sylenthModDestination, fieldName, fieldValue));
			ObjectUtils.SetField(z2, fieldName, fieldValue);
			
			// store Zebra mod matrix field information
			zebraUsedModMatrixSlots.Add(fieldName);
		}
		#endregion
		
		#region ToZebra2Preset
		
		/// <summary>
		/// Convert a sylenth preset to zebra preset
		/// </summary>
		/// <param name="defaultZebra2PresetFile">default zebra 2 preset file</param>
		/// <param name="doProcessInitPresets">whether to process presets with name 'Init'</param>
		/// <param name="skipAfterCounter">if number is higher than 0, skip the rest</param>
		/// <returns>a list of zebra preset files</returns>
		public List<Zebra2Preset> ToZebra2Preset(string defaultZebra2PresetFile, bool doProcessInitPresets, int skipAfterCounter=-1) {
			var zebra2PresetList = new List<Zebra2Preset>();

			// TODO: problems that should be fixed
			// Using Adam Van Baker Sylenth1 Soundset Part 2:
			// BP freq problem : 64 FX Uprise 3, 135 Arp clean state
			// Reverb problem: 170 Key Stone Valley
			
			// Alot wrong:
			// 176 LD Arab flute (Top 100 DJs Sylenth Presets.fxb)
			
			int convertCounter = 0;
			foreach (Sylenth1Preset.Syl1PresetContent sylenthPresetContent in _sylenth1Preset.ContentArray) {
				_zebraNextFreeModMatrixSlot = 1; // reset the index that keeps track of the used matrix slots for a preset
				_zebraUsedModSources.Clear(); // reset the list that keeps track of the used mod sources for a preset
				convertCounter++;
				
				// break if the break counter is set (i.e. skip the rest)
				if (skipAfterCounter > 0) {
					if (convertCounter == skipAfterCounter) break;
				}
				
				// Skip if the Preset Name is Init
				if (!doProcessInitPresets && (sylenthPresetContent.PresetName == "Init" || sylenthPresetContent.PresetName == "Default")) { //  || !Content.PresetName.StartsWith("SEQ Afrodiseq"
					// skipping
					Console.Out.WriteLine("Skipping Sylenth preset number {0} - {1} ...", convertCounter, sylenthPresetContent.PresetName);
					Logger.DoDebug(String.Format("*** Skipping preset number {0} - {1}! ***", convertCounter, sylenthPresetContent.PresetName));
				} else {
					Console.Out.WriteLine("Converting Sylenth preset number {0} - {1} ...", convertCounter, sylenthPresetContent.PresetName);
					Logger.DoDebug(String.Format("**** Converting preset number {0} - {1} ***", convertCounter, sylenthPresetContent.PresetName));
					
					// Load a default preset file and modify this one
					var zebra2Preset = new Zebra2Preset();
					zebra2Preset.BankIndex = convertCounter;
					zebra2Preset.Read(defaultZebra2PresetFile, true);
					
					// add to list
					zebra2PresetList.Add(zebra2Preset);
					
					// set name
					zebra2Preset.PresetName = sylenthPresetContent.PresetName;
					
					// set master volume (50?)
					zebra2Preset.Main_CcOp = ConvertSylenthValueToZebra(sylenthPresetContent.MainVolume, 0, 10, 0, 80); // restrict the limit from 100 - x
					zebra2Preset.ZMas_Mast = 80;
					
					// set mix volume
					zebra2Preset.VCA1_Vol1 = ConvertSylenthValueToZebra(sylenthPresetContent.MixA, 0, 10, 0, 100); // the range is 0, 100, but using 0 - x i get a more correct conversion
					zebra2Preset.VCA1_Vol2 = ConvertSylenthValueToZebra(sylenthPresetContent.MixB, 0, 10, 0, 100); // the range is 0, 100, but using 0 - x i get a more correct conversion
					
					// set portamento
					zebra2Preset.VCC_Porta = ConvertSylenthValueToZebra(sylenthPresetContent.PortaTime, 0, 10, 0, 60); // the range is 0, 100, but using 0 - 40 i get a more correct conversion
					
					// set mode (mono legato etc)
					if (sylenthPresetContent.MonoLegato == Sylenth1Preset.ONOFF.On) {
						zebra2Preset.VCC_Mode = (int) Zebra2Preset.VoiceMode.legato;
					} else {
						zebra2Preset.VCC_Mode = (int) Zebra2Preset.VoiceMode.poly;
					}
					
					// set pitch bend up and down
					float pitchBendRange = ConvertSylenthValueToZebra(sylenthPresetContent.PitchBendRange, 1, 24, 1, 24);
					zebra2Preset.VCC_PB = (int) pitchBendRange;
					zebra2Preset.VCC_PBD = (int) pitchBendRange;
					
					#region Oscillators
					// OscA1
					if (sylenthPresetContent.OscA1Voices != Sylenth1Preset.VOICES.VOICES_0) {
						if (sylenthPresetContent.OscA1Wave != Sylenth1Preset.OSCWAVE.OSC_Noise) {
							zebra2Preset.OSC1_WNum = ConvertSylenthWaveToZebra(sylenthPresetContent.OscA1Wave, sylenthPresetContent.OscA1Invert);
							zebra2Preset.OSC1_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Volume, 0, 10, 0, 200);

							// cannot let the volume be 0?!
							if (zebra2Preset.OSC1_Vol == 0) zebra2Preset.OSC1_Vol = 100;

							// turn the volume on Noise1 down
							zebra2Preset.Noise1_Vol = 0;
						} else {
							// turn the volume on OSC1 all the way down
							zebra2Preset.OSC1_Vol = 0;
							
							// turn the volume on Noise1 up
							zebra2Preset.Noise1_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Volume, 0, 10, 0, 200);
							zebra2Preset.Noise1_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Stereo, 0, 10, 0, 100);
							zebra2Preset.Noise1_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Pan, -10, 10, -100, 100);
						}
						zebra2Preset.OSC1_Tune = ConvertSylenthTuneToZebra(sylenthPresetContent.OscA1Octave, sylenthPresetContent.OscA1Note, sylenthPresetContent.OscA1Fine);
						zebra2Preset.OSC1_Poly = ConvertSylenthVoicesToZebra(sylenthPresetContent.OscA1Voices);
						zebra2Preset.OSC1_Dtun = ConvertSylenthDetuneToZebra(sylenthPresetContent.OscA1Detune);
						zebra2Preset.OSC1_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Pan, -10, 10, -100, 100);
						zebra2Preset.OSC1_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Stereo, 0, 10, 0, 100);
						zebra2Preset.OSC1_RePhs = (sylenthPresetContent.OscA1Retrig == Sylenth1Preset.ONOFF.On ? 1 : 0);
						zebra2Preset.OSC1_Phse = ConvertSylenthValueToZebra(sylenthPresetContent.OscA1Phase, 0, 360, 0, 100);
					} else {
						// set volume to zero?
						zebra2Preset.OSC1_Vol = 0;
					}

					// OscA2
					if (sylenthPresetContent.OscA2Voices != Sylenth1Preset.VOICES.VOICES_0) {
						if (sylenthPresetContent.OscA2Wave != Sylenth1Preset.OSCWAVE.OSC_Noise) {
							zebra2Preset.OSC2_WNum = ConvertSylenthWaveToZebra(sylenthPresetContent.OscA2Wave, sylenthPresetContent.OscA2Invert);
							zebra2Preset.OSC2_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Volume, 0, 10, 0, 200);

							// cannot let the volume be 0?!
							if (zebra2Preset.OSC2_Vol == 0) zebra2Preset.OSC2_Vol = 100;

							// turn the volume on Noise1 down
							zebra2Preset.Noise1_Vol = 0;
						} else {
							// turn the volume on OSC1 all the way down
							zebra2Preset.OSC2_Vol = 0;
							
							// turn the volume on Noise1 up
							zebra2Preset.Noise1_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Volume, 0, 10, 0, 200);
							zebra2Preset.Noise1_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Stereo, 0, 10, 0, 100);
							zebra2Preset.Noise1_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Pan, -10, 10, -100, 100);
						}
						zebra2Preset.OSC2_Tune = ConvertSylenthTuneToZebra(sylenthPresetContent.OscA2Octave, sylenthPresetContent.OscA2Note, sylenthPresetContent.OscA2Fine);
						zebra2Preset.OSC2_Poly = ConvertSylenthVoicesToZebra(sylenthPresetContent.OscA2Voices);
						zebra2Preset.OSC2_Dtun = ConvertSylenthDetuneToZebra(sylenthPresetContent.OscA2Detune);
						zebra2Preset.OSC2_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Pan, -10, 10, -100, 100);
						zebra2Preset.OSC2_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Stereo, 0, 10, 0, 100);
						zebra2Preset.OSC2_RePhs = (sylenthPresetContent.OscA2Retrig == Sylenth1Preset.ONOFF.On ? 1 : 0);
						zebra2Preset.OSC2_Phse = ConvertSylenthValueToZebra(sylenthPresetContent.OscA2Phase, 0, 360, 0, 100);
					} else {
						// set volume to zero?
						zebra2Preset.OSC2_Vol = 0;
					}
					
					// OscB1
					if (sylenthPresetContent.OscB1Voices != Sylenth1Preset.VOICES.VOICES_0) {
						if (sylenthPresetContent.OscB1Wave != Sylenth1Preset.OSCWAVE.OSC_Noise) {
							zebra2Preset.OSC3_WNum = ConvertSylenthWaveToZebra(sylenthPresetContent.OscB1Wave, sylenthPresetContent.OscB1Invert);
							zebra2Preset.OSC3_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Volume, 0, 10, 0, 200);

							// cannot let the volume be 0?!
							if (zebra2Preset.OSC3_Vol == 0) zebra2Preset.OSC3_Vol = 100;

							// turn the volume on Noise2 down
							zebra2Preset.Noise2_Vol = 0;
						} else {
							// turn the volume on OSC1 all the way down
							zebra2Preset.OSC3_Vol = 0;
							
							// turn the volume on Noise2 up
							zebra2Preset.Noise2_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Volume, 0, 10, 0, 200);
							zebra2Preset.Noise2_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Stereo, 0, 10, 0, 100);
							zebra2Preset.Noise2_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Pan, -10, 10, -100, 100);
						}
						zebra2Preset.OSC3_Tune = ConvertSylenthTuneToZebra(sylenthPresetContent.OscB1Octave, sylenthPresetContent.OscB1Note, sylenthPresetContent.OscB1Fine);
						zebra2Preset.OSC3_Poly = ConvertSylenthVoicesToZebra(sylenthPresetContent.OscB1Voices);
						zebra2Preset.OSC3_Dtun = ConvertSylenthDetuneToZebra(sylenthPresetContent.OscB1Detune);
						zebra2Preset.OSC3_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Pan, -10, 10, -100, 100);
						zebra2Preset.OSC3_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Stereo, 0, 10, 0, 100);
						zebra2Preset.OSC3_RePhs = (sylenthPresetContent.OscB1Retrig == Sylenth1Preset.ONOFF.On ? 1 : 0);
						zebra2Preset.OSC3_Phse = ConvertSylenthValueToZebra(sylenthPresetContent.OscB1Phase, 0, 360, 0, 100);
					} else {
						// set volume to zero?
						zebra2Preset.OSC3_Vol = 0;
					}

					// OscB2
					if (sylenthPresetContent.OscB2Voices != Sylenth1Preset.VOICES.VOICES_0) {
						if (sylenthPresetContent.OscB2Wave != Sylenth1Preset.OSCWAVE.OSC_Noise) {
							zebra2Preset.OSC4_WNum = ConvertSylenthWaveToZebra(sylenthPresetContent.OscB2Wave, sylenthPresetContent.OscB2Invert);
							zebra2Preset.OSC4_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Volume, 0, 10, 0, 200);

							// cannot let the volume be 0?!
							if (zebra2Preset.OSC4_Vol == 0) zebra2Preset.OSC4_Vol = 100;

							// turn the volume on Noise2 down
							zebra2Preset.Noise2_Vol = 0;
						} else {
							// turn the volume on OSC1 all the way down
							zebra2Preset.OSC4_Vol = 0;
							
							// turn the volume on Noise2 up
							zebra2Preset.Noise2_Vol = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Volume, 0, 10, 0, 200);
							zebra2Preset.Noise2_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Stereo, 0, 10, 0, 100);
							zebra2Preset.Noise2_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Pan, -10, 10, -100, 100);
						}
						zebra2Preset.OSC4_Tune = ConvertSylenthTuneToZebra(sylenthPresetContent.OscB2Octave, sylenthPresetContent.OscB2Note, sylenthPresetContent.OscB2Fine);
						zebra2Preset.OSC4_Poly = ConvertSylenthVoicesToZebra(sylenthPresetContent.OscB2Voices);
						zebra2Preset.OSC4_Dtun = ConvertSylenthDetuneToZebra(sylenthPresetContent.OscB2Detune);
						zebra2Preset.OSC4_Pan = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Pan, -10, 10, -100, 100);
						zebra2Preset.OSC4_PolW = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Stereo, 0, 10, 0, 100);
						zebra2Preset.OSC4_RePhs = (sylenthPresetContent.OscB2Retrig == Sylenth1Preset.ONOFF.On ? 1 : 0);
						zebra2Preset.OSC4_Phse = ConvertSylenthValueToZebra(sylenthPresetContent.OscB2Phase, 0, 360, 0, 100);
					} else {
						// set volume to zero?
						zebra2Preset.OSC4_Vol = 0;
					}
					#endregion

					#region Filters
					// The filter control knob values are added to the knob values of both filter A and B.
					// The frequency scale is logarithmic though, so the cutoff values are added
					// on a logarithmic scale.

					// FilterA
					if (sylenthPresetContent.FilterAType != Sylenth1Preset.FILTERTYPE.Bypass) {
						if (sylenthPresetContent.FilterAInput == Sylenth1Preset.FILTERAINPUT.FILTER_A) {
							zebra2Preset.VCF1_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterACutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterADrive, 0, 10, 0, 100);
							zebra2Preset.VCF1_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterAReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterAType, sylenthPresetContent.FilterADB, sylenthPresetContent.FilterCtlWarmDrive);
						} else if (sylenthPresetContent.FilterAInput == Sylenth1Preset.FILTERAINPUT.FILTER_A_B) {
							zebra2Preset.VCF1_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterACutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterADrive, 0, 10, 0, 100);
							zebra2Preset.VCF1_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterAReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterAType, sylenthPresetContent.FilterADB, sylenthPresetContent.FilterCtlWarmDrive);
							
							zebra2Preset.VCF2_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterACutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterADrive, 0, 10, 0, 100);
							zebra2Preset.VCF2_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterAReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterAType, sylenthPresetContent.FilterADB, sylenthPresetContent.FilterCtlWarmDrive);
						}
					}

					// FilterB
					if (sylenthPresetContent.FilterBType != Sylenth1Preset.FILTERTYPE.Bypass) {
						if (sylenthPresetContent.FilterBInput == Sylenth1Preset.FILTERBINPUT.FILTER_B) {
							zebra2Preset.VCF2_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBCutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterBDrive, 0, 10, 0, 100);
							zebra2Preset.VCF2_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterBType, sylenthPresetContent.FilterBDB, sylenthPresetContent.FilterCtlWarmDrive);
						} else if (sylenthPresetContent.FilterBInput == Sylenth1Preset.FILTERBINPUT.FILTER_B_A) {
							zebra2Preset.VCF2_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBCutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterBDrive, 0, 10, 0, 100);
							zebra2Preset.VCF2_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF2_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterBType, sylenthPresetContent.FilterBDB, sylenthPresetContent.FilterCtlWarmDrive);

							zebra2Preset.VCF1_Cut = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBCutoff, sylenthPresetContent.FilterCtlCutoff, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Drv = ConvertSylenthValueToZebra(sylenthPresetContent.FilterBDrive, 0, 10, 0, 100);
							zebra2Preset.VCF1_Res = ConvertSylenthFrequencyToZebra(sylenthPresetContent.FilterBReso, sylenthPresetContent.FilterCtlReso, Sylenth1Preset.FloatToHz.FilterCutoff);
							zebra2Preset.VCF1_Typ = (int) ConvertSylenthFilterToZebra(sylenthPresetContent.FilterBType, sylenthPresetContent.FilterBDB, sylenthPresetContent.FilterCtlWarmDrive);
						}
					}
					#endregion
					
					#region LFOs
					// LFO1 is used for something
					if (sylenthPresetContent.YModLFO1Dest1 != Sylenth1Preset.YMODDEST.None || sylenthPresetContent.YModLFO1Dest2 != Sylenth1Preset.YMODDEST.None ) {
						SetZebraLFOFromSylenth(zebra2Preset, sylenthPresetContent.LFO1Wave, sylenthPresetContent.LFO1Free, sylenthPresetContent.LFO1Rate, sylenthPresetContent.LFO1Gain,
						                       "LFO1_Wave",
						                       "LFO1_Sync",
						                       "LFO1_Rate",
						                       "LFO1_Trig",
						                       "LFO1_Phse",
						                       "LFO1_Amp",
						                       "LFO1_Slew");
					}

					// LFO2 is used for something
					if (sylenthPresetContent.YModLFO2Dest1 != Sylenth1Preset.YMODDEST.None || sylenthPresetContent.YModLFO2Dest2 != Sylenth1Preset.YMODDEST.None ) {
						SetZebraLFOFromSylenth(zebra2Preset, sylenthPresetContent.LFO2Wave, sylenthPresetContent.LFO2Free, sylenthPresetContent.LFO2Rate, sylenthPresetContent.LFO2Gain,
						                       "LFO2_Wave",
						                       "LFO2_Sync",
						                       "LFO2_Rate",
						                       "LFO2_Trig",
						                       "LFO2_Phse",
						                       "LFO2_Amp",
						                       "LFO2_Slew");
					}
					#endregion
					
					#region Effects
					
					#region Arpeggiator
					if (sylenthPresetContent.XSwArpOnOff == Sylenth1Preset.ONOFF.On) {
						
						// Set correct voice mode
						zebra2Preset.VCC_Mode = (int) Zebra2Preset.VoiceMode.arpeggiator;

						// ArpSync (0=1:64, 1=1:32, 2=1:16, 3=1:8, 4=1:4, 5=1:2, 6=1:1, 7=1:32 dot, 8=1:16 dot, 9=1:8 dot, 10=1:4 dot, 11=1:2 dot, 12=1:16 trip, 13=1:8 trip, 14=1:4 trip, 15=1:2 trip, 16=1:1 trip)
						zebra2Preset.VCC_ArSc = ConvertSylenthArpSyncTimingsToZebra(sylenthPresetContent.ArpTime);
						
						// ArpOrder (0 = By Note, 1 = As Played)
						zebra2Preset.VCC_ArOrd = (int) Zebra2Preset.ArpOrder.By_Note;
						
						// ArpLoop (0 = Forward F-->, 1 = Backward B <--, 2 = ForwardBackward FB <->, 3 = BackwardForward BF >-<)
						switch (sylenthPresetContent.ArpMode) {
							case Sylenth1Preset.ARPMODE.Up:
							case Sylenth1Preset.ARPMODE.Chord:
							case Sylenth1Preset.ARPMODE.Random:
							case Sylenth1Preset.ARPMODE.Ordered:
							case Sylenth1Preset.ARPMODE.Step:
								zebra2Preset.VCC_ArLp = (int) Zebra2Preset.ArpLoop.Forward;
								break;
							case Sylenth1Preset.ARPMODE.Down:
								zebra2Preset.VCC_ArLp = (int) Zebra2Preset.ArpLoop.Backward;
								break;
							case Sylenth1Preset.ARPMODE.Up_Down:
							case Sylenth1Preset.ARPMODE.Up_Down2:
								zebra2Preset.VCC_ArLp = (int) Zebra2Preset.ArpLoop.ForwardBackward;
								break;
							case Sylenth1Preset.ARPMODE.Down_Up:
							case Sylenth1Preset.ARPMODE.Down_Up2:
								zebra2Preset.VCC_ArLp = (int) Zebra2Preset.ArpLoop.BackwardForward;
								break;
						}

						// ArpOctave (0, 1, 2)
						switch (sylenthPresetContent.ArpOctave) {
							case Sylenth1Preset.ARPOCTAVE.OCTAVE_1:
								zebra2Preset.VCC_ArOct = 0;
								break;
							case Sylenth1Preset.ARPOCTAVE.OCTAVE_2:
								zebra2Preset.VCC_ArOct = 1;
								break;
							case Sylenth1Preset.ARPOCTAVE.OCTAVE_3:
							case Sylenth1Preset.ARPOCTAVE.OCTAVE_4:
								zebra2Preset.VCC_ArOct = 2;
								break;
						}

						// ArpLoopLength (1 - 16)
						switch (sylenthPresetContent.ArpWrap) {
							case Sylenth1Preset.ARPWRAP.WRAP_0:
								zebra2Preset.VCC_ArLL = 16;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_1:
								zebra2Preset.VCC_ArLL = 1;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_2:
								zebra2Preset.VCC_ArLL = 2;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_3:
								zebra2Preset.VCC_ArLL = 3;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_4:
								zebra2Preset.VCC_ArLL = 4;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_5:
								zebra2Preset.VCC_ArLL = 5;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_6:
								zebra2Preset.VCC_ArLL = 6;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_7:
								zebra2Preset.VCC_ArLL = 7;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_8:
								zebra2Preset.VCC_ArLL = 8;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_9:
								zebra2Preset.VCC_ArLL = 9;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_10:
								zebra2Preset.VCC_ArLL = 10;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_11:
								zebra2Preset.VCC_ArLL = 11;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_12:
								zebra2Preset.VCC_ArLL = 12;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_13:
								zebra2Preset.VCC_ArLL = 13;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_14:
								zebra2Preset.VCC_ArLL = 14;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_15:
								zebra2Preset.VCC_ArLL = 15;
								break;
							case Sylenth1Preset.ARPWRAP.WRAP_16:
								zebra2Preset.VCC_ArLL = 16;
								break;
						}
						
						// ArpPortamento ( 0 = Off, 1 = On)
						zebra2Preset.VCC_ArTr = (int) Zebra2Preset.ArpPortamento.On;
						
						// Set each of the 16 arp notes
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold01, sylenthPresetContent.XArpTransp01, sylenthPresetContent.XArpVelo01, 1);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold02, sylenthPresetContent.XArpTransp02, sylenthPresetContent.XArpVelo02, 2);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold03, sylenthPresetContent.XArpTransp03, sylenthPresetContent.XArpVelo03, 3);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold04, sylenthPresetContent.XArpTransp04, sylenthPresetContent.XArpVelo04, 4);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold05, sylenthPresetContent.XArpTransp05, sylenthPresetContent.XArpVelo05, 5);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold06, sylenthPresetContent.XArpTransp06, sylenthPresetContent.XArpVelo06, 6);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold07, sylenthPresetContent.XArpTransp07, sylenthPresetContent.XArpVelo07, 7);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold08, sylenthPresetContent.XArpTransp08, sylenthPresetContent.XArpVelo08, 8);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold09, sylenthPresetContent.XArpTransp09, sylenthPresetContent.XArpVelo09, 9);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold10, sylenthPresetContent.XArpTransp10, sylenthPresetContent.XArpVelo10, 10);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold11, sylenthPresetContent.XArpTransp11, sylenthPresetContent.XArpVelo11, 11);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold12, sylenthPresetContent.XArpTransp12, sylenthPresetContent.XArpVelo12, 12);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold13, sylenthPresetContent.XArpTransp13, sylenthPresetContent.XArpVelo13, 13);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold14, sylenthPresetContent.XArpTransp14, sylenthPresetContent.XArpVelo14, 14);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold15, sylenthPresetContent.XArpTransp15, sylenthPresetContent.XArpVelo15, 15);
						SetZebraArpeggiatorNoteFromSylenth(zebra2Preset, sylenthPresetContent.ArpGate, sylenthPresetContent.XArpHold16, sylenthPresetContent.XArpTransp16, sylenthPresetContent.XArpVelo16, 16);
					}
					#endregion
					
					#region Distortion = Distortion 3
					if (sylenthPresetContent.XSwDistOnOff == Sylenth1Preset.ONOFF.On) {
						// get how hard to distort
						float distortionAmount = ConvertSylenthValueToZebra(sylenthPresetContent.DistortAmount, 0, 10, -12, 48);
						
						// regulate using the dry / wet percentage
						float distortDryWet = ConvertSylenthValueToZebra(sylenthPresetContent.DistortDryWet, 0, 10, 0, 100);
						
						zebra2Preset.Dist3_Input = distortionAmount * distortDryWet / 100;
						zebra2Preset.Dist3_Output = 5;
						
						zebra2Preset.Dist3_PreTilt = 0.00f;			// Pre Tilt
						zebra2Preset.Dist3_PstTilt = 0.00f;			// Post Tilt
						zebra2Preset.Dist3_CntFreq = 100.00f;		// Center Freq
						zebra2Preset.Dist3_Low = 0.00f;				// Low
						zebra2Preset.Dist3_High = 18.00f;			// High
						zebra2Preset.Dist3_PostFlt = (int) Zebra2Preset.DistortionPostFilter.DualBandShelf;	// Post Filter=Dual-Band Shelf
						
						switch(sylenthPresetContent.DistortType) {
							case Sylenth1Preset.DISTORTTYPE.OverDrv:
								zebra2Preset.Dist3_Type = (int) Zebra2Preset.DistortionType.TubeClassAB;
								
								// increase output when using overdrive
								zebra2Preset.Dist3_Output = 10;
								break;
							case Sylenth1Preset.DISTORTTYPE.Clip:
								zebra2Preset.Dist3_Type = (int) Zebra2Preset.DistortionType.HardClip;
								break;
							case Sylenth1Preset.DISTORTTYPE.Decimate:
								zebra2Preset.Dist3_Type = (int) Zebra2Preset.DistortionType.Rectify;
								
								// increase output when using decimate
								zebra2Preset.Dist3_Output = 20;
								
								// and reduce highs
								zebra2Preset.Dist3_High = 0.00f;
								break;
							case Sylenth1Preset.DISTORTTYPE.BitCrush:
								zebra2Preset.Dist3_Type = (int)Zebra2Preset.DistortionType.Rectify;
								
								// increase output when using bitcrush
								zebra2Preset.Dist3_Output = 20;
								
								// and reduce highs
								zebra2Preset.Dist3_High = 0.00f;
								break;
							case Sylenth1Preset.DISTORTTYPE.FoldBack:
								zebra2Preset.Dist3_Type = (int) Zebra2Preset.DistortionType.Foldback;
								break;
						}
					} else {
						// set everything to zero?
						zebra2Preset.Shape3_Type = (int) Zebra2Preset.DistortionType.TubeClassA; // Type (Type=0)
						zebra2Preset.Dist3_Input = 0;
						zebra2Preset.Dist3_Output = 0;
						
						zebra2Preset.Dist3_PreTilt = 0.00f;			// Pre Tilt
						zebra2Preset.Dist3_PstTilt = 0.00f;			// Post Tilt
						zebra2Preset.Dist3_CntFreq = 100.00f;		// Center Freq
						zebra2Preset.Dist3_Low = 0.00f;				// Low
						zebra2Preset.Dist3_High = 0.00f;			// High
						zebra2Preset.Dist3_PostFlt = (int) Zebra2Preset.DistortionPostFilter.DualBandShelf;	// Post Filter=Dual-Band Shelf
					}
					#endregion
					
					#region Phaser
					if (sylenthPresetContent.XSwPhaserOnOff == Sylenth1Preset.ONOFF.On) {
						// Mode Phaser: classic phaser unit
						zebra2Preset.ModFX1_Mode = (int) Zebra2Preset.ModFXType.Phaser;
						// Center, Nominal delay time / allpass cutoff, i.e. before modulation.
						zebra2Preset.ModFX1_Cent = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserCenterFreq, 0, 10, 0, 100);
						// Speed, The rate of the ModFX module’s own LFO (from 0.001Hz to 1Hz).
						zebra2Preset.ModFX1_Sped = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserLFORate, 0, 10, 0, 100);
						// Depth, Amount of LFO modulation.
						zebra2Preset.ModFX1_Dpth = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserLFOGain, 0, 10, 0, 100);
						// Feedbk, Bipolar feedback control for ‘flanger’ type resonances – especially at extreme values.
						zebra2Preset.ModFX2_FeeB = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserLFOGain, 0, 100, -100, 100);
						// Mix, Balance between dry and wet signal.
						zebra2Preset.ModFX1_Mix = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserDry_Wet, 0, 100, 0, 100);
						// Stereo, LFO phase offset between the two stereo channels. Note that 50% is often more ‘stereo’ than 100%.
						zebra2Preset.ModFX1_PhOff = ConvertSylenthValueToZebra(sylenthPresetContent.PhaserSpread, 0, 10, 0, 100);
						// Quad, The volume of an additional chorus effect, with independant LFO.
						zebra2Preset.ModFX1_Quad = 0.00f;			// Quad
						// Q-Phase, Modulation LFO phase offset (see Stereo above) for the Quad effect.
						zebra2Preset.ModFX1_Qphs = 25.00f;			// QuadPhase
						
						// Equalizer
						// This unique feature can e.g. preserve the stereo image of bass frequencies via low cut,
						// while at the same time making the chorus effect sound less harsh via high cut.
						// EQ: switches ModFX equalization on/off
						// LowFreq: low crossover frequency
						// HiFreq: high crossover frequency
						// Boost: cut/boost controls for the two frequencie ranges
						zebra2Preset.ModFX1_LCut = 0.00f;			// LowCut Freq
						zebra2Preset.ModFX1_Leq = 0.00f;			// Low Boost dB
						zebra2Preset.ModFX1_HCut = 100.00f;			// HiCut Freq
						zebra2Preset.ModFX1_Heq = 0.00f;			// High Boost dB
						zebra2Preset.ModFX1_Q1 = 0.00f;				// Q1
						zebra2Preset.ModFX1_Q2 = 0.00f;				// Q2
						zebra2Preset.ModFX1_EQon = 1;				// EQ
					} else {
						zebra2Preset.ModFX1_Mode = (int) Zebra2Preset.ModFXType.Phaser;	// Mode=Chorus
						zebra2Preset.ModFX1_Cent = 20.00f;			// Center
						zebra2Preset.ModFX1_Sped = 50.00f;			// Speed
						zebra2Preset.ModFX1_PhOff = 50.00f;			// Stereo
						zebra2Preset.ModFX1_Dpth = 50.00f;			// Depth
						zebra2Preset.ModFX1_FeeB = 0.00f;			// Feedback
						zebra2Preset.ModFX1_Mix = 0.00f;			// Mix
						zebra2Preset.ModFX1_Quad = 0.00f;			// Quad
						zebra2Preset.ModFX1_Qphs = 25.00f;			// QuadPhase
						zebra2Preset.ModFX1_LCut = 0.00f;			// LowCut Freq
						zebra2Preset.ModFX1_Leq = 0.00f;			// Low Boost dB
						zebra2Preset.ModFX1_HCut = 100.00f;			// HiCut Freq
						zebra2Preset.ModFX1_Heq = 0.00f;			// High Boost dB
						zebra2Preset.ModFX1_Q1 = 0.00f;				// Q1
						zebra2Preset.ModFX1_Q2 = 0.00f;				// Q2
						zebra2Preset.ModFX1_EQon = 1;				// EQ
					}
					#endregion
					
					#region Chorus
					if (sylenthPresetContent.XSwChorusOnOff == Sylenth1Preset.ONOFF.On) {
						// Mode Chorus: chorus / flanger using short delay lines
						zebra2Preset.ModFX2_Mode = (int) Zebra2Preset.ModFXType.Chorus;
						// Center, Nominal delay time / allpass cutoff, i.e. before modulation.
						zebra2Preset.ModFX2_Cent = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusDelay, 1, 40, 0, 100);
						// Speed, The rate of the ModFX module’s own LFO (from 0.001Hz to 1Hz).
						zebra2Preset.ModFX2_Sped = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusRate, 0.01f, 27.5f, 0, 100);
						// Stereo, LFO phase offset between the two stereo channels. Note that 50% is often more ‘stereo’ than 100%.
						zebra2Preset.ModFX2_PhOff = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusWidth, 0, 100, 0, 100); // Stereo
						// Depth, Amount of LFO modulation.
						zebra2Preset.ModFX2_Dpth = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusDepth, 0, 100, 0, 100);
						// Feedbk, Bipolar feedback control for ‘flanger’ type resonances – especially at extreme values.
						zebra2Preset.ModFX2_FeeB = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusFeedback, 0, 100, 0, 100);
						// Mix, Balance between dry and wet signal.
						zebra2Preset.ModFX2_Mix = ConvertSylenthValueToZebra(sylenthPresetContent.ChorusDry_Wet, 0, 100, 0, 100);
						// Quad, The volume of an additional chorus effect, with independant LFO.
						if (sylenthPresetContent.ChorusMode == Sylenth1Preset.CHORUSMODE.Dual) {
							zebra2Preset.ModFX2_Quad = 100.00f;			// Quad
						} else {
							zebra2Preset.ModFX2_Quad = 0.00f;			// Quad
						}
						// Q-Phase, Modulation LFO phase offset (see Stereo above) for the Quad effect.
						zebra2Preset.ModFX2_Qphs = 25.00f;			// QuadPhase
						// Equalizer
						// This unique feature can e.g. preserve the stereo image of bass frequencies via low cut,
						// while at the same time making the chorus effect sound less harsh via high cut.
						// EQ: switches ModFX equalization on/off
						// LowFreq: low crossover frequency
						// HiFreq: high crossover frequency
						// Boost: cut/boost controls for the two frequencie ranges
						zebra2Preset.ModFX2_LCut = 0.00f;			// LowCut Freq
						zebra2Preset.ModFX2_Leq = 0.00f;			// Low Boost dB
						zebra2Preset.ModFX2_HCut = 100.00f;			// HiCut Freq
						zebra2Preset.ModFX2_Heq = 0.00f;			// High Boost dB
						zebra2Preset.ModFX2_Q1 = 0.00f;				// Q1
						zebra2Preset.ModFX2_Q2 = 0.00f;				// Q2
						zebra2Preset.ModFX2_EQon = 1;				// EQ
					} else {
						zebra2Preset.ModFX2_Mode = (int) Zebra2Preset.ModFXType.Chorus;	// Mode=Chorus
						zebra2Preset.ModFX2_Cent = 20.00f;			// Center
						zebra2Preset.ModFX2_Sped = 50.00f;			// Speed
						zebra2Preset.ModFX2_PhOff = 50.00f;			// Stereo
						zebra2Preset.ModFX2_Dpth = 50.00f;			// Depth
						zebra2Preset.ModFX2_FeeB = 0.00f;			// Feedback
						zebra2Preset.ModFX2_Mix = 0.00f;			// Mix
						zebra2Preset.ModFX2_Quad = 0.00f;			// Quad
						zebra2Preset.ModFX2_Qphs = 25.00f;			// QuadPhase
						zebra2Preset.ModFX2_LCut = 0.00f;			// LowCut Freq
						zebra2Preset.ModFX2_Leq = 0.00f;			// Low Boost dB
						zebra2Preset.ModFX2_HCut = 100.00f;			// HiCut Freq
						zebra2Preset.ModFX2_Heq = 0.00f;			// High Boost dB
						zebra2Preset.ModFX2_Q1 = 0.00f;				// Q1
						zebra2Preset.ModFX2_Q2 = 0.00f;				// Q2
						zebra2Preset.ModFX2_EQon = 1;				// EQ
					}
					#endregion
					
					#region Equaliser on MasterBus
					if (sylenthPresetContent.XSwEQOnOff == Sylenth1Preset.ONOFF.On) {
						float eqBassFreq = Sylenth1Preset.ValueToHz(sylenthPresetContent.EQBassFreq, Sylenth1Preset.FloatToHz.EQBassFreq); // 13.75f, 880.0f
						float eqTrebleFreq = Sylenth1Preset.ValueToHz(sylenthPresetContent.EQTrebleFreq, Sylenth1Preset.FloatToHz.EQTrebleFreq); // 440, 28160
						// the Sylenth eq only works upwards even though zebra has the range -24 -> 24
						zebra2Preset.EQ1_Fc1 = Zebra2Preset.EqualiserHzToFreqValue(eqBassFreq);
						zebra2Preset.EQ1_Gain1 = ConvertSylenthValueToZebra(sylenthPresetContent.EQBass, 0, 15, 0, 15);
						zebra2Preset.EQ1_Res1 = 25.00f;
						zebra2Preset.EQ1_Fc4 = Zebra2Preset.EqualiserHzToFreqValue(eqTrebleFreq);
						zebra2Preset.EQ1_Gain4 = ConvertSylenthValueToZebra(sylenthPresetContent.EQTreble, 0, 15, 0, 15);
						zebra2Preset.EQ1_Res4 = 25.00f;
					} else {
						zebra2Preset.EQ1_Fc1 = 20.00f;               // Freq LowShelf (fc1=20.00)
						zebra2Preset.EQ1_Res1 = 25.00f;              // Q LowShelf (res1=25.00)
						zebra2Preset.EQ1_Gain1 = 00.00f;             // Gain LowShelf (gain1=0.00)
						zebra2Preset.EQ1_Fc2 = 40.00f;               // Freq Mid1 (fc2=40.00)
						zebra2Preset.EQ1_Res2 = 25.00f;              // Q Mid1 (res2=25.00)
						zebra2Preset.EQ1_Gain2 = 00.00f;             // Gain Mid1 (gain2=0.00)
						zebra2Preset.EQ1_Fc3 = 60.00f;               // Freq Mid2 (fc3=60.00)
						zebra2Preset.EQ1_Res3 = 25.00f;              // Q Mid2 (res3=25.00)
						zebra2Preset.EQ1_Gain3 = 00.00f;             // Gain Mid2 (gain3=0.00)
						zebra2Preset.EQ1_Fc4 = 80.00f;               // Freq HiShelf (fc4=80.00)
						zebra2Preset.EQ1_Res4 = 25.00f;              // Q HiShelf (res4=25.00)
						zebra2Preset.EQ1_Gain4 = 00.00f;             // Gain HiShelf (gain4=0.00)
					}
					#endregion
					
					#region Delay
					if (sylenthPresetContent.XSwDelayOnOff == Sylenth1Preset.ONOFF.On) {

						zebra2Preset.Delay1_Mix = ConvertSylenthValueToZebra(sylenthPresetContent.DelayDry_Wet, 0, 100, 0, 50); // the range is 0, 100, but using 0 - x i get a more correct volume
						
						// Feedback
						//zebra2Preset.Delay1_FB = ConvertSylenthValueToZebra(Content.DelayFeedback, 0, 100, 0, 50);
						zebra2Preset.Delay1_CB = ConvertSylenthValueToZebra(sylenthPresetContent.DelayFeedback, 0, 100, 0, 50);
						zebra2Preset.Delay1_FB = 0;
						
						// If ping pong, use X-back and serial 2
						if (sylenthPresetContent.DelayPingPong == Sylenth1Preset.ONOFF.On) {
							zebra2Preset.Delay1_Mode = (int) Zebra2Preset.DelayMode.serial_2;
							zebra2Preset.Delay1_CB = 50;
							zebra2Preset.Delay1_FB = 0;
						} else {
							zebra2Preset.Delay1_Mode = (int) Zebra2Preset.DelayMode.stereo_2;
						}
						
						// high pass == low cut and low pass == high cut
						// these does not work since zebra delay freq has a range of 0 - 100 not 0 - 150 like the rest ?
						// ConvertSylenthFrequencyToZebra(Content.DelayLowCut, Sylenth1Preset.FloatToHz.DelayLowCut);
						// ConvertSylenthFrequencyToZebra(Content.DelayHighCut, Sylenth1Preset.FloatToHz.DelayHighCut);
						zebra2Preset.Delay1_HP = 0;
						zebra2Preset.Delay1_LP = 60; // 35 - 60 sounds more like the default sylenth delay settings
						
						zebra2Preset.Delay1_Sync1 = ConvertSylenthDelayTimingsToZebra(sylenthPresetContent.DelayTimeLeft);
						zebra2Preset.Delay1_Pan1 = -80;
						zebra2Preset.Delay1_Sync2 = ConvertSylenthDelayTimingsToZebra(sylenthPresetContent.DelayTimeRight);
						zebra2Preset.Delay1_Pan2 = 80;
					} else {
						// set volume to zero?
						zebra2Preset.Delay1_Mix = 0;
					}
					#endregion

					#region Reverb
					if (sylenthPresetContent.XSwReverbOnOff == Sylenth1Preset.ONOFF.On) {
						zebra2Preset.Rev1_Damp = ConvertSylenthValueToZebra(sylenthPresetContent.ReverbDamp, 0, 10, 0, 100);
						
						// sylenth dry/wet slider makes up two zebra2 sliders (dry and wet)
						zebra2Preset.Rev1_Dry = 80;
						// reduce the range to limit wetness (80 is better than 100)
						zebra2Preset.Rev1_Wet = ConvertSylenthValueToZebra(sylenthPresetContent.ReverbDry_Wet, 0, 100, 0, 80);
						
						// sylenth stores the predelay in ms (0 - 200)
						zebra2Preset.Rev1_Pre = ConvertSylenthValueToZebra(sylenthPresetContent.ReverbPredelay, 0, 200, 0, 100);
						
						zebra2Preset.Rev1_Size = ConvertSylenthValueToZebra(sylenthPresetContent.ReverbSize, 0, 10, 0, 100);
					} else {
						// set volume to zero?
						zebra2Preset.Rev1_Wet = 0;
						zebra2Preset.Rev1_Dry = 100;
					}
					#endregion
					
					#region Compression
					if (sylenthPresetContent.XSwCompOnOff == Sylenth1Preset.ONOFF.On) {
						zebra2Preset.Comp1_Att = ConvertSylenthValueToZebra(sylenthPresetContent.CompAttack, 0.1f, 300, 0, 100);
						zebra2Preset.Comp1_Rel = ConvertSylenthValueToZebra(sylenthPresetContent.CompRelease, 1, 500, 0, 100);
						zebra2Preset.Comp1_Thres = ConvertSylenthValueToZebra(sylenthPresetContent.CompThreshold, -30, 0, -96, 0);
						zebra2Preset.Comp1_Rat = MathUtils.ConvertAndMainainRatio(sylenthPresetContent.CompRatio, 0, 1, 0, 100);
					}
					#endregion
					#endregion
					
					#region Envelopes
					const float envelopeMin = 0;
					const float envelopeMax = 120;
					// Set correct Envelope Slope (v-slope)
					const int envelopeMode = (int) Zebra2Preset.EnvelopeMode.v_slope;
					const float envelopeSlope = -70.00f;
					// Envelope 1 - Used as Amp Envelope
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvAAttack, "ENV1_TBase", "ENV1_Atk");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvADecay, "ENV1_TBase", "ENV1_Dec");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvARelease, "ENV1_TBase", "ENV1_Rel");
					zebra2Preset.ENV1_Sus = ConvertSylenthValueToZebra(sylenthPresetContent.AmpEnvASustain, 0, 10, envelopeMin, envelopeMax);
					zebra2Preset.ENV1_Vel = 70;
					zebra2Preset.ENV1_Mode = envelopeMode;
					zebra2Preset.ENV1_Slope = envelopeSlope;
					
					// Envelope 2 - Used as Amp Envelope
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvBAttack, "ENV2_TBase", "ENV2_Atk");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvBDecay, "ENV2_TBase", "ENV2_Dec");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.AmpEnvBRelease, "ENV2_TBase", "ENV2_Rel");
					zebra2Preset.ENV2_Sus = ConvertSylenthValueToZebra(sylenthPresetContent.AmpEnvBSustain, 0, 10, envelopeMin, envelopeMax);
					zebra2Preset.ENV2_Vel = 70;
					zebra2Preset.ENV2_Mode = envelopeMode;
					zebra2Preset.ENV2_Slope = envelopeSlope;
					
					// Envelope 3 - Used as Mod Envelope
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv1Attack, "ENV3_TBase", "ENV3_Atk");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv1Decay, "ENV3_TBase", "ENV3_Dec");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv1Release, "ENV3_TBase", "ENV3_Rel");
					zebra2Preset.ENV3_Sus = ConvertSylenthValueToZebra(sylenthPresetContent.ModEnv1Sustain, 0, 10, envelopeMin, envelopeMax);
					zebra2Preset.ENV3_Vel = 70;
					zebra2Preset.ENV3_Mode = envelopeMode;
					zebra2Preset.ENV3_Slope = envelopeSlope;

					// Envelope 4 - Used as Mod Envelope
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv2Attack, "ENV4_TBase", "ENV4_Atk");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv2Decay, "ENV4_TBase", "ENV4_Dec");
					SetZebraEnvelopeFromSylenth(zebra2Preset, sylenthPresetContent.ModEnv2Release, "ENV4_TBase", "ENV4_Rel");
					zebra2Preset.ENV4_Sus = ConvertSylenthValueToZebra(sylenthPresetContent.ModEnv2Sustain, 0, 10, envelopeMin, envelopeMax);
					zebra2Preset.ENV4_Vel = 70;
					zebra2Preset.ENV4_Mode = envelopeMode;
					zebra2Preset.ENV4_Slope = envelopeSlope;
					#endregion
					
					#region Modulation
					
					// keep track of the modulation sources that already has been processed
					var processedModulationSourceAndDest = new Dictionary<string, List<string>>();
					
					// See if the mod envelopes are used
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_1, sylenthPresetContent.YModEnv1Dest1, sylenthPresetContent.XModEnv1Dest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_1, sylenthPresetContent.YModEnv1Dest2, sylenthPresetContent.XModEnv1Dest2Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_2, sylenthPresetContent.YModEnv2Dest1, sylenthPresetContent.XModEnv2Dest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_ModEnv_2, sylenthPresetContent.YModEnv2Dest2, sylenthPresetContent.XModEnv2Dest2Am, processedModulationSourceAndDest);

					// See if the LFOs are used
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_LFO_1, sylenthPresetContent.YModLFO1Dest1, sylenthPresetContent.XModLFO1Dest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_LFO_1, sylenthPresetContent.YModLFO1Dest2, sylenthPresetContent.XModLFO1Dest2Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_LFO_2, sylenthPresetContent.YModLFO2Dest1, sylenthPresetContent.XModLFO2Dest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, Sylenth1Preset.XMODSOURCE.SOURCE_LFO_2, sylenthPresetContent.YModLFO2Dest2, sylenthPresetContent.XModLFO2Dest2Am, processedModulationSourceAndDest);

					// Set the matrix slots
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc1ASource, sylenthPresetContent.YModMisc1ADest1, sylenthPresetContent.XModMisc1ADest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc1ASource, sylenthPresetContent.YModMisc1ADest2, sylenthPresetContent.XModMisc1ADest2Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc1BSource, sylenthPresetContent.YModMisc1BDest1, sylenthPresetContent.XModMisc1BDest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc1BSource, sylenthPresetContent.YModMisc1BDest2, sylenthPresetContent.XModMisc1BDest2Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc2ASource, sylenthPresetContent.YModMisc2ADest1, sylenthPresetContent.XModMisc2ADest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc2ASource, sylenthPresetContent.YModMisc2ADest2, sylenthPresetContent.XModMisc2ADest2Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc2BSource, sylenthPresetContent.YModMisc2BDest1, sylenthPresetContent.XModMisc2BDest1Am, processedModulationSourceAndDest);
					SetZebraModSourcesFromSylenth(sylenthPresetContent, zebra2Preset, sylenthPresetContent.XModMisc2BSource, sylenthPresetContent.YModMisc2BDest2, sylenthPresetContent.XModMisc2BDest2Am, processedModulationSourceAndDest);
					#endregion
				}
			}
			return zebra2PresetList;
		}
		#endregion
	}
}
