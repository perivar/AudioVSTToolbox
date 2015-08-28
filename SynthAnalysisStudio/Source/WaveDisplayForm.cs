using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using Jacobi.Vst.Interop.Host;

using CommonUtils;
using CommonUtils.Audio;
using CommonUtils.Audio.NAudio;
using CommonUtils.MathLib.FFT;
using CommonUtils.VST;

using NAudio.Wave;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WaveDisplayForm.
	/// </summary>
	public partial class WaveDisplayForm : Form
	{
		public const int SYLENTH_PARAM_AMP_ENV1_ATTACK = 0;
		public const int SYLENTH_PARAM_AMP_ENV1_DECAY = 1;
		public const int SYLENTH_PARAM_AMP_ENV1_RELEASE = 2;
		public const int SYLENTH_PARAM_AMP_ENV1_SUSTAIN = 3;
		
		public const int SYLENTH_PARAM_FILTERA_CUTOFF = 42;
		public const int SYLENTH_PARAM_FILTERA_RESO = 45;
		public const int SYLENTH_PARAM_FILTERA_TYPE = 46;
		public const int SYLENTH_PARAM_FILTERA_DB = 47;
		public const int SYLENTH_PARAM_FILTERB_CUTOFF = 48;
		public const int SYLENTH_PARAM_FILTERB_RESO = 51;
		public const int SYLENTH_PARAM_FILTERB_TYPE = 52;
		public const int SYLENTH_PARAM_FILTERB_DB = 53;
		public const int SYLENTH_PARAM_FILTERCTL_CUTOFF = 54;
		public const int SYLENTH_PARAM_FILTERCTL_RESO = 56;
		
		public const int SYLENTH_PARAM_LFO1_GAIN = 59;
		public const int SYLENTH_PARAM_LFO1_RATE = 61;
		public const int SYLENTH_PARAM_LFO1_WAVE = 62;
		public const int SYLENTH_PARAM_MAIN_VOLUME = 68;
		public const int SYLENTH_PARAM_MIX_A = 69;
		public const int SYLENTH_PARAM_MIX_B = 70;
		public const int SYLENTH_PARAM_MOD_ENV1_ATTACK = 71;
		public const int SYLENTH_PARAM_MOD_ENV1_DECAY = 72;
		public const int SYLENTH_PARAM_MOD_ENV1_RELEASE = 73;
		public const int SYLENTH_PARAM_MOD_ENV1_SUSTAIN = 74;
		public const int SYLENTH_PARAM_OSC1_VOLUME = 91;
		public const int SYLENTH_PARAM_OSC1_WAVE = 92;

		public const int SYLENTH_PARAM_XMOD_ENV1_DEST1AMOUNT = 197;
		public const int SYLENTH_PARAM_XMOD_LFO1_DEST1AMOUNT = 201;

		public const int SYLENTH_PARAM_YMOD_LFO1_DEST1 = 229;
		public const int SYLENTH_PARAM_YMOD_ENV1_DEST1 = 225;

		float ampAttack = 0;
		float ampDecay = 0;
		float ampSustain = 0;
		float ampRelease = 0;
		string ampAttackDisplay = "";
		string ampDecayDisplay = "";
		string ampSustainDisplay = "";
		string ampReleaseDisplay = "";

		float modAttack = 0;
		float modDecay = 0;
		float modSustain = 0;
		float modRelease = 0;
		string modAttackDisplay = "";
		string modDecayDisplay = "";
		string modSustainDisplay = "";
		string modReleaseDisplay = "";
		
		int durationSamples = 0;
		double durationMilliseconds = 0;
		
		public VstPluginContext PluginContext { get; set; }
		public VstPlaybackNAudio Playback { get; set; }

		private bool DoGUIRefresh = true;
		
		public WaveDisplayForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			VstHost host = VstHost.Instance;
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		void OnOffCheckboxCheckedChanged(object sender, EventArgs e)
		{
			var check = (CheckBox) sender;
			if(check.Checked)
			{
				DoGUIRefresh = true;
			} else {
				DoGUIRefresh = false;
			}
		}
		
		void RecordBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.Record = true;
		}
		
		void StopBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.Record = false;
			
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		void ClearBtnClick(object sender, EventArgs e)
		{
			ClearAudio();
		}
		
		void AmplitudeTrackBarScroll(object sender, EventArgs e)
		{
			this.waveDisplayUserControl1.Amplitude = AmplitudeTrackBar.Value;
		}
		
		void CropBtnClick(object sender, EventArgs e)
		{
			CropAudio();
		}
		
		void MidiNoteCheckboxCheckedChanged(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			
			// if first keypress setup audio
			if (Playback == null) {
				// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
				// tblock = 0.15 makes blocksize = 6615.
				const int sampleRate = 44100;
				int blockSize = (int) (sampleRate * 0.15f); //6615;
				const int channels = 2;
				host.Init(blockSize, sampleRate, channels);
				
				Playback = new VstPlaybackNAudio(host);
				Playback.Play();
			}
			
			var check = (CheckBox) sender;
			if(check.Checked)
			{
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
			} else {
				host.SendMidiNote(host.SendContinousMidiNote, 0);
			}
		}
		
		void SaveWAVBtnClick(object sender, EventArgs e)
		{
			// store this in a wav ouput file.
			string wavFilePath = String.Format("audio-data-left-{0}.wav", StringUtils.GetCurrentTimestamp());

			VstHost host = VstHost.Instance;
			AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
		}
		
		void AdsrSaveBtnClick(object sender, EventArgs e)
		{
			ADSRSave();
		}
		
		void PlayMidiC5100msBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			
			// if first keypress setup audio
			if (Playback == null) {
				// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
				// tblock = 0.15 makes blocksize = 6615.
				int sampleRate = 44100;
				int blockSize = (int) (sampleRate * 0.15f); //6615;
				int channels = 2;
				host.Init(blockSize, sampleRate, channels);
				
				Playback = new VstPlaybackNAudio(host);
				Playback.Play();
			}
			
			// time how long this took
			Stopwatch stopwatch = Stopwatch.StartNew();
			
			// end midi note on
			host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
			
			// wait 100 ms
			System.Threading.Thread.Sleep(100);

			// send midi note off
			host.SendMidiNote(host.SendContinousMidiNote, 0);
			
			stopwatch.Stop();
			Console.WriteLine("Midi Note Sent. Time used {0} ms", stopwatch.ElapsedMilliseconds);
		}
		
		private void ADSRSave() {
			
			string ampEnvelopeAttack = String.Format("{0:0.00}", ampAttack);
			string ampEnvelopeDecay = String.Format("{0:0.00}", ampDecay);
			string ampEnvelopeSustain = String.Format("{0:0.00}", ampSustain);
			string ampEnvelopeRelease = String.Format("{0:0.00}", ampRelease);
			string ampEnvelopeAttackDisplay = ampAttackDisplay.Trim();
			string ampEnvelopeDecayDisplay = ampDecayDisplay.Trim();
			string ampEnvelopeSustainDisplay = ampSustainDisplay.Trim();
			string ampEnvelopeReleaseDisplay = ampReleaseDisplay.Trim();
			string modEnvelopeAttack = String.Format("{0:0.00}", modAttack);
			string modEnvelopeDecay = String.Format("{0:0.00}", modDecay);
			string modEnvelopeSustain = String.Format("{0:0.00}", modSustain);
			string modEnvelopeRelease = String.Format("{0:0.00}", modRelease);
			string modEnvelopeAttackDisplay = modAttackDisplay.Trim();
			string modEnvelopeDecayDisplay = modDecayDisplay.Trim();
			string modEnvelopeSustainDisplay = modSustainDisplay.Trim();
			string modEnvelopeReleaseDisplay = modReleaseDisplay.Trim();
			
			string durationInSamples = "" + durationSamples;
			string durationInMilliseconds = String.Format("{0:0.00}", durationMilliseconds);
			
			// store this in a xml ouput file.
			string xmlFilePath = "ADSR-measurement.xml";
			if (File.Exists(xmlFilePath)) {
				// add to existing xml document
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				
				xmlDoc.Element("ADSRMeasurement").Add(
					new XElement("Row",
					             new XElement("AmpEnvelopeAttackDisplay", ampEnvelopeAttackDisplay),
					             new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
					             new XElement("AmpEnvelopeDecayDisplay", ampEnvelopeDecayDisplay),
					             new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
					             new XElement("AmpEnvelopeSustainDisplay", ampEnvelopeSustainDisplay),
					             new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
					             new XElement("AmpEnvelopeReleaseDisplay", ampEnvelopeReleaseDisplay),
					             new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
					             new XElement("ModEnvelopeAttackDisplay", modEnvelopeAttackDisplay),
					             new XElement("ModEnvelopeAttack", modEnvelopeAttack),
					             new XElement("ModEnvelopeDecayDisplay", modEnvelopeDecayDisplay),
					             new XElement("ModEnvelopeDecay", modEnvelopeDecay),
					             new XElement("ModEnvelopeSustainDisplay", modEnvelopeSustainDisplay),
					             new XElement("ModEnvelopeSustain", modEnvelopeSustain),
					             new XElement("ModEnvelopeReleaseDisplay", modEnvelopeReleaseDisplay),
					             new XElement("ModEnvelopeRelease", modEnvelopeRelease),
					             new XElement("DurationSamples", durationInSamples),
					             new XElement("DurationMS", durationInMilliseconds)
					            ));

				xmlDoc.Save(xmlFilePath);
			} else {
				// create xml document first
				var xmlDoc =
					new XDocument(
						new XElement("ADSRMeasurement",
						             new XElement("Row",
						                          new XElement("AmpEnvelopeAttackDisplay", ampEnvelopeAttackDisplay),
						                          new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
						                          new XElement("AmpEnvelopeDecayDisplay", ampEnvelopeDecayDisplay),
						                          new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
						                          new XElement("AmpEnvelopeSustainDisplay", ampEnvelopeSustainDisplay),
						                          new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
						                          new XElement("AmpEnvelopeReleaseDisplay", ampEnvelopeReleaseDisplay),
						                          new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
						                          new XElement("ModEnvelopeAttackDisplay", modEnvelopeAttackDisplay),
						                          new XElement("ModEnvelopeAttack", modEnvelopeAttack),
						                          new XElement("ModEnvelopeDecayDisplay", modEnvelopeDecayDisplay),
						                          new XElement("ModEnvelopeDecay", modEnvelopeDecay),
						                          new XElement("ModEnvelopeSustainDisplay", modEnvelopeSustainDisplay),
						                          new XElement("ModEnvelopeSustain", modEnvelopeSustain),
						                          new XElement("ModEnvelopeReleaseDisplay", modEnvelopeReleaseDisplay),
						                          new XElement("ModEnvelopeRelease", modEnvelopeRelease),
						                          new XElement("DurationSamples", durationInSamples),
						                          new XElement("DurationMS", durationInMilliseconds)
						                         )
						            ));
				xmlDoc.Save(xmlFilePath);
			}
		}
		
		private void ClearAudio() {
			VstHost host = VstHost.Instance;
			host.ClearRecording();
			
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		private void CropAudio() {
			VstHost host = VstHost.Instance;

			// crop the audio at silence
			float[] data = AudioUtils.CropAudioAtSilence(host.RecordedLeft.ToArray(), 0, false, 0);

			host.RecordedLeft.Clear();
			host.RecordedLeft.AddRange(data);
			
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		private void SetupAudio(VstHost host) {
			// if first keypress setup audio
			if (Playback == null) {
				// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
				// tblock = 0.15 makes blocksize = 6615.
				int sampleRate = 44100;
				int blockSize = (int) (sampleRate * 0.15f); //6615;
				int channels = 2;
				host.Init(blockSize, sampleRate, channels);
				
				Playback = new VstPlaybackNAudio(host);
			}
		}
		

		// -----------------------------------
		// Automatic measurement methods
		// Currently support auto-measurment of the following
		// Amplitude Envelopes: A D and R
		// Modulation Envelopes: A D and R
		// LFO
		// -----------------------------------
		
		// How to measure Amplitude Envelopes:
		
		// Sylenth (Amplitude Env Attack, Decay and Release)
		// Init:
		// AMP ENV A: A = 0, D = 0, S = 0, R = 0
		// Then step through the A, D and R steps and measure
		// Attack: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
		// Decay: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
		
		// For Release you have to make sure the Sustain is full.
		// Then send a short midi message and measure the length of the tail
		// Release: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
		

		// How to measure Modulation Envelopes:
		
		// Sylenth (Mod Env Attack and Decay):
		// Init:
		// AMP ENV A: A = 0, D = 0, R = 0, S = max (10)
		// Mix A and Mix B = 0
		// Master (Main Volume) = half (5)
		// Mod Env A destination: "Mix AB"
		// Mod Env A destination amount (ModEnv1Dest1Am): max (10)
		// Mod Env A: A = 0, D = 0; R = 0, S = 0
		// Then step through the A and D steps and measure
		
		
		// How to measure LFO:
		
		// Sylenth:
		// Init:
		// AMP ENV A: A = 0, D = 0, S = 0, R = 0
		// LFO Gain = Max (1)
		// LFO Rate = Min (0)
		// LFO Wave = LFO_Pulse
		// OSC VOL = Half (0.5)
		// OSC Wave = OSC_Pulse
		// SYLENTH_PARAM_XMODLFO1DEST1AMOUNT = Max (1)
		// SYLENTH_PARAM_YMODLFO1DEST1 = YMODDEST.VolumeAB
		// Then through the LFO steps and measure
		// for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)

		void MeasureAmpEnvelopeInit() {
			
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			
			// Sylenth (Amplitude Env Attack, Decay and Release)
			// Init:
			// AMP ENV A: A = 0, D = 0, S = 0, R = 0
			// Then step through the A, D and R steps and measure
			// Attack: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
			// Decay: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
			// Release: for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f)
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MAIN_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MIX_A, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MIX_B, 0.5f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE, 0.0f);

			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMOD_ENV1_DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.None));
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMOD_ENV1_DEST1AMOUNT, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMOD_LFO1_DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.None));
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMOD_LFO1_DEST1AMOUNT, 0.0f);
		}

		void MeasureModEnvelopeInit() {
			
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			
			// Sylenth (Mod Env Attack and Decay):
			// Init:
			// AMP ENV A: A = 0, D = 0, R = 0, S = max (10)
			// Mix A and Mix B = 0
			// Master (Main Volume) = half (5)
			// Mod Env A destination: "Mix AB"
			// Mod Env A destination amount (ModEnv1Dest1Am): max (10)
			// Mod Env A: A = 0, D = 0; R = 0, S = 0
			// Then step through the A and D steps and measure

			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MAIN_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MIX_A, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MIX_B, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MOD_ENV1_DECAY, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MOD_ENV1_SUSTAIN, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_MOD_ENV1_RELEASE, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMOD_ENV1_DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.Mix_AB));
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMOD_ENV1_DEST1AMOUNT, 1.0f);
		}

		void MeasureLFOInit() {
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			// init
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_GAIN, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_RATE, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_WAVE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.LFOWAVE.LFO_Pulse));
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_OSC1_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_OSC1_WAVE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.OSCWAVE.OSC_Pulse));
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMOD_LFO1_DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.VolumeAB));
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMOD_LFO1_DEST1AMOUNT, 1.0f);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMOD_ENV1_DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.None));
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMOD_ENV1_DEST1AMOUNT, 0.0f);
		}
		
		void MeasureADSRParameter(int paramName, float paramValue, string envName, bool measureRelease = false) {
			
			Console.Out.WriteLine("MeasureADSREntry: Measuring {0} at value {1:0.00}...", envName, paramValue);
			
			VstHost host = VstHost.Instance;
			
			// time how long this takes
			Stopwatch stopwatch = Stopwatch.StartNew();

			// init the buffers
			host.ClearRecording();
			host.ClearLastProcessedBuffers();

			// start record
			host.Record = true;
			
			// set the parameter
			PluginContext.PluginCommandStub.SetParameter(paramName, paramValue);
			((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(paramName, paramValue);
			
			// wait until it has started playing
			while(!host.LastProcessedBufferLeftPlaying) {
				// start playing audio
				Playback.Play();

				// play midi
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);

				if (stopwatch.ElapsedMilliseconds > 5000) {
					stopwatch.Stop();
					Console.Out.WriteLine("MeasureADSREntry: Playing Midi Failed!");
					return;
				}
				System.Threading.Thread.Sleep(100);
			}
			
			if (measureRelease) {
				// release is a special case where you only measure the tail after
				// a short midi signal
				// therefore we need to stop the midi message here
				host.SendMidiNote(host.SendContinousMidiNote, 0);
			}
			
			// wait until it has stopped playing
			stopwatch.Stop();
			stopwatch.Start();
			while (host.LastProcessedBufferLeftPlaying) {
				if (stopwatch.ElapsedMilliseconds > 40000) {
					stopwatch.Stop();
					Console.Out.WriteLine("MeasureADSREntry: Playing never stopped?!");
					break;
				}
			}
			
			// stop playing audio
			Playback.Stop();

			stopwatch.Stop();
			Console.Out.WriteLine("MeasureADSREntry: Playing stopped: {0} ms.", stopwatch.ElapsedMilliseconds);
			
			// stop recording
			host.Record = false;

			// wait a specfied time
			System.Threading.Thread.Sleep(100);

			// crop
			CropAudio();
			
			// store the duration
			RetrieveDuration();
			
			// store this in a wav ouput file.
			float param = PluginContext.PluginCommandStub.GetParameter(paramName);
			string wavFilePath = String.Format("{0}{1:0.00}s-{2}.wav", envName, param, StringUtils.GetCurrentTimestamp());
			AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
			
			// store as a png
			System.Drawing.Bitmap png = AudioAnalyzer.DrawWaveformMono(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 1, host.SampleRate);
			string fileName = String.Format("{0}{1:0.00}s-{2}.png", envName, param, StringUtils.GetCurrentTimestamp());
			png.Save(fileName);
			
			if (!measureRelease) {
				// turn of midi unless we are measuring a release envelope
				// then it should have been turned of immideately after a small signal is generated
				host.SendMidiNote(host.SendContinousMidiNote, 0);
			}

			// clear
			ClearAudio();

			// wait a specfied time
			System.Threading.Thread.Sleep(100);

			stopwatch.Stop();
		}

		void ZeroOutPluginAmpADSR() {
			// zero out amp ADSR
			this.ampAttack = 0;
			this.ampDecay = 0;
			this.ampSustain = 0;
			this.ampRelease = 0;
			this.ampAttackDisplay = "";
			this.ampDecayDisplay = "";
			this.ampSustainDisplay = "";
			this.ampReleaseDisplay = "";
		}
		
		void RetrievePluginAmpADSR() {
			// get amp ADSR from plugin
			this.ampAttack = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK);
			this.ampDecay = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY);
			this.ampSustain = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN);
			this.ampRelease = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE);
			this.ampAttackDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_AMP_ENV1_ATTACK);
			this.ampDecayDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_AMP_ENV1_DECAY);
			this.ampSustainDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_AMP_ENV1_SUSTAIN);
			this.ampReleaseDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_AMP_ENV1_RELEASE);
		}
		
		void ZeroOutPluginModADSR() {
			// zero out mod ADSR
			this.modAttack = 0;
			this.modDecay = 0;
			this.modSustain = 0;
			this.modRelease = 0;
			this.modAttackDisplay = "";
			this.modDecayDisplay = "";
			this.modSustainDisplay = "";
			this.modReleaseDisplay = "";
		}

		void RetrievePluginModADSR() {
			// get mod ADSR from plugin
			this.modAttack = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK);
			this.modDecay = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_DECAY);
			this.modSustain = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_SUSTAIN);
			this.modRelease = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_RELEASE);
			this.modAttackDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_MOD_ENV1_ATTACK);
			this.modDecayDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_MOD_ENV1_DECAY);
			this.modSustainDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_MOD_ENV1_SUSTAIN);
			this.modReleaseDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_MOD_ENV1_RELEASE);
		}
		
		void RetrieveDuration() {
			// get duration
			this.durationMilliseconds = this.waveDisplayUserControl1.DurationInMilliseconds;
			this.durationSamples = this.waveDisplayUserControl1.NumberOfSamples;
		}
		
		void MeasureAmpABtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// step through the Attack steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK, paramValue, "amp-env-attack");
				
				ZeroOutPluginModADSR();
				RetrievePluginAmpADSR();
				
				// store in xml file
				ADSRSave();
			}
		}
		
		void MeasureAmpDBtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// step through the Decay steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_DECAY, paramValue, "amp-env-decay");

				ZeroOutPluginModADSR();
				RetrievePluginAmpADSR();

				// store in xml file
				ADSRSave();
			}
		}

		void MeasureAmpRBtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// make sure to set the sustain to full
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN, 1.0f);
			
			// step through the Release steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE, paramValue, "amp-env-release", true);

				ZeroOutPluginModADSR();
				RetrievePluginAmpADSR();

				// store in xml file
				ADSRSave();
			}
		}
		
		void MeasureModABtnClick(object sender, EventArgs e)
		{
			MeasureModEnvelopeInit();

			// step through the Attack steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK, paramValue, "mod-env-attack");

				ZeroOutPluginAmpADSR();
				RetrievePluginModADSR();

				// store in xml file
				ADSRSave();
			}
		}
		
		void MeasureModDBtnClick(object sender, EventArgs e)
		{
			MeasureModEnvelopeInit();
			
			// step through the Decay steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_MOD_ENV1_DECAY, paramValue, "mod-env-decay");

				ZeroOutPluginAmpADSR();
				RetrievePluginModADSR();

				// store in xml file
				ADSRSave();
			}
		}
		
		void MeasureLFOBtnClick(object sender, EventArgs e)
		{
			MeasureLFOInit();
			VstHost host = VstHost.Instance;
			
			var lfoAlreadyProcessed = new List<string>();

			// step through the LFO steps
			int count = 0;
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				
				// time how long this takes
				Stopwatch stopwatch = Stopwatch.StartNew();

				// init the buffers
				host.ClearRecording();
				host.ClearLastProcessedBuffers();

				// start record
				host.Record = true;

				// set the parameter
				PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_RATE, paramValue);
				
				// get param display value
				string paramDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(SYLENTH_PARAM_LFO1_RATE);
				
				// check if already processed
				if (lfoAlreadyProcessed.Contains(paramDisplay)) {
					continue;
				} else {
					lfoAlreadyProcessed.Add(paramDisplay);
				}

				// wait until it has started playing
				while(!host.LastProcessedBufferLeftPlaying) {
					// start playing audio
					Playback.Play();

					// play midi
					host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
					
					if (stopwatch.ElapsedMilliseconds > 5000) {
						stopwatch.Stop();
						Console.Out.WriteLine("MeasureLFOBtnClick: Playing Midi Failed!");
						return;
					}
					System.Threading.Thread.Sleep(100);
				}
				
				// play for approx 1000 ms
				// paramValue: 1 = 1/256 Triple 	(count = 0)
				// paramValue: 0 = 8/1 D			(count = 10)
				System.Threading.Thread.Sleep((int) (20*Math.Pow(count, 2)));
				
				// stop midi
				host.SendMidiNote(host.SendContinousMidiNote, 0);
				
				// wait until it has stopped playing
				while (host.LastProcessedBufferLeftPlaying) {
					if (stopwatch.ElapsedMilliseconds > 40000) {
						Console.Out.WriteLine("MeasureLFOBtnClick: Playing never stopped?!");
						break;
					}
				}
				// stop playing audio
				Playback.Stop();

				stopwatch.Stop();
				Console.Out.WriteLine("MeasureLFOBtnClick: Playing stopped: {0} ms. {1}", stopwatch.ElapsedMilliseconds, paramDisplay);
				
				// stop recording
				host.Record = false;

				// crop
				CropAudio();
				
				// store this in a wav ouput file.
				string wavFilePath = String.Format("audio-LFO-{0}-{1}.wav", StringUtils.MakeValidFileName(paramDisplay), StringUtils.GetCurrentTimestamp());
				AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
				
				// store as a png
				System.Drawing.Bitmap png = AudioAnalyzer.DrawWaveformMono(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 1, host.SampleRate);
				string fileName = String.Format("audio-LFO-{0}-{1}.png", StringUtils.MakeValidFileName(paramDisplay), StringUtils.GetCurrentTimestamp());
				png.Save(fileName);
				
				// clear
				ClearAudio();

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				stopwatch.Stop();
				
				count++;
			}
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			if (DoGUIRefresh) {
				VstHost host = VstHost.Instance;
				
				// update while recording
				if (host.Record) {
					//this.waveDisplayUserControl1.Resolution = (int) this.waveDisplayUserControl1.MaxResolution;
				}
				this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
			}
		}
	}
}