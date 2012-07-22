using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using CommonUtils;
using CommonUtils.Audio;
using CommonUtils.Audio.NAudio;
using CommonUtils.VST;

using NAudio.Wave;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WaveDisplayForm.
	/// </summary>
	public partial class WaveDisplayForm : Form
	{
		const int SYLENTH_PARAM_AMP_ENV1_ATTACK = 0;
		const int SYLENTH_PARAM_AMP_ENV1_DECAY = 1;
		const int SYLENTH_PARAM_AMP_ENV1_RELEASE = 2;
		const int SYLENTH_PARAM_AMP_ENV1_SUSTAIN = 3;
		const int SYLENTH_PARAM_LFO1_GAIN = 59;
		const int SYLENTH_PARAM_LFO1_RATE = 61;
		const int SYLENTH_PARAM_LFO1_WAVE = 62;
		const int SYLENTH_PARAM_MAIN_VOLUME = 68;
		const int SYLENTH_PARAM_MIX_A = 69;
		const int SYLENTH_PARAM_MIX_B = 70;
		const int SYLENTH_PARAM_MOD_ENV1_ATTACK = 71;
		const int SYLENTH_PARAM_MOD_ENV1_DECAY = 72;
		const int SYLENTH_PARAM_MOD_ENV1_RELEASE = 73;
		const int SYLENTH_PARAM_MOD_ENV1_SUSTAIN = 74;
		const int SYLENTH_PARAM_OSC1_VOLUME = 91;
		const int SYLENTH_PARAM_OSC1_WAVE = 92;

		const int SYLENTH_PARAM_XMOD_ENV1_DEST1AMOUNT = 197;
		const int SYLENTH_PARAM_XMOD_LFO1_DEST1AMOUNT = 201;

		const int SYLENTH_PARAM_YMOD_LFO1_DEST1 = 229;
		const int SYLENTH_PARAM_YMOD_ENV1_DEST1 = 225;

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
			CheckBox check = (CheckBox) sender;
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
			
			MaxResolutionTrackBar.Maximum = (int) this.waveDisplayUserControl1.MaxResolution;
			MaxResolutionTrackBar.TickFrequency = MaxResolutionTrackBar.Maximum / 10;
			MaxResolutionTrackBar.Value = (int) this.waveDisplayUserControl1.MaxResolution;
			
			StartPositionTrackBar.Maximum = (int) this.waveDisplayUserControl1.NumberOfSamples;
			StartPositionTrackBar.TickFrequency = StartPositionTrackBar.Maximum / 10;
			StartPositionTrackBar.Value = 0;
			
			this.waveDisplayUserControl1.Resolution = (int) this.waveDisplayUserControl1.MaxResolution;
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		void ClearBtnClick(object sender, EventArgs e)
		{
			ClearAudio();
		}
		
		void MaxResolutionTrackBarScroll(object sender, EventArgs e)
		{
			this.waveDisplayUserControl1.Resolution = MaxResolutionTrackBar.Value;
		}
		
		void AmplitudeTrackBarScroll(object sender, EventArgs e)
		{
			this.waveDisplayUserControl1.Amplitude = AmplitudeTrackBar.Value;
		}
		
		void StartPositionTrackBarScroll(object sender, EventArgs e)
		{
			this.waveDisplayUserControl1.StartPosition = StartPositionTrackBar.Value;
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
				int sampleRate = 44100;
				int blockSize = (int) (sampleRate * 0.15f); //6615;
				int channels = 2;
				host.Init(blockSize, sampleRate, channels);
				
				Playback = new VstPlaybackNAudio(host);
				Playback.Play();
			}
			
			CheckBox check = (CheckBox) sender;
			if(check.Checked)
			{
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
			} else {
				host.SendMidiNote(host.SendContinousMidiNote, 0);
			}
		}
		
		void SaveXMLBtnClick(object sender, EventArgs e)
		{
			// store this in a xml ouput file.
			string xmlFilePath = "audio-data.xml";

			DataTable dt = new DataTable();
			dt.Columns.Add("float", typeof(float));
			
			VstHost host = VstHost.Instance;
			foreach (float f in host.RecordedLeft.ToArray())
			{
				dt.Rows.Add( new object[] { f } );
			}
			
			dt.TableName = "Audio Data";
			dt.WriteXml(xmlFilePath);
		}
		
		void SaveWAVBtnClick(object sender, EventArgs e)
		{
			// store this in a wav ouput file.
			string wavFilePath = String.Format("audio-data-{0}.wav", StringUtils.GetCurrentTimestamp());

			VstHost host = VstHost.Instance;
			AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
		}
		
		void AdsrSaveBtnClick(object sender, EventArgs e)
		{
			AdsrSave();
			
			/*
			float ampEnvelopeAttack = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeAttack;
			float ampEnvelopeDecay = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeDecay;
			float ampEnvelopeSustain = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeSustain;
			float ampEnvelopeRelease = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeRelease;
			
			string ampEnvelopeAttackString = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeAttackString;
			string ampEnvelopeDecayString = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeDecayString;
			string ampEnvelopeSustainString = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeSustainString;
			string ampEnvelopeReleaseString = ((HostCommandStub) PluginContext.HostCommandStub).ampEnvelopeReleaseString;

			ampAttackTextBox.Text = ampEnvelopeAttackString;
			ampDecayTextBox.Text = ampEnvelopeDecayString;
			ampSustainTextBox.Text = ampEnvelopeSustainString;
			ampReleaseTextBox.Text = ampEnvelopeReleaseString;
			
			double durationInMilliseconds = this.waveDisplayUserControl1.DurationInMilliseconds;
			int numberOfSamples = this.waveDisplayUserControl1.NumberOfSamples;
			
			durationSamplesTextBox.Text = "" + numberOfSamples;
			durationMsTextBox.Text = String.Format("{0:0.00}", durationInMilliseconds);
			
			// store this in a xml ouput file.
			string xmlFilePath = "ADSR-sampler.xml";
			if (File.Exists(xmlFilePath)) {
				// add to existing xml document
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				
				xmlDoc.Element("ADSRSampler").Add(
					new XElement("Row",
					             new XElement("AmpEnvelopeAttackString", ampEnvelopeAttackString),
					             new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
					             new XElement("AmpEnvelopeDecayString", ampEnvelopeDecayString),
					             new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
					             new XElement("AmpEnvelopeSustainString", ampEnvelopeSustainString),
					             new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
					             new XElement("AmpEnvelopeReleaseString", ampEnvelopeReleaseString),
					             new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
					             new XElement("DurationSamples", numberOfSamples),
					             new XElement("DurationMS", durationInMilliseconds)
					            ));

				xmlDoc.Save(xmlFilePath);
			} else {
				// create xml document first
				XDocument xmlDoc =
					new XDocument(
						new XElement("ADSRSampler",
						             new XElement("Row",
						                          new XElement("AmpEnvelopeAttackString", ampEnvelopeAttackString),
						                          new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
						                          new XElement("AmpEnvelopeDecayString", ampEnvelopeDecayString),
						                          new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
						                          new XElement("AmpEnvelopeSustainString", ampEnvelopeSustainString),
						                          new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
						                          new XElement("AmpEnvelopeReleaseString", ampEnvelopeReleaseString),
						                          new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
						                          new XElement("DurationSamples", numberOfSamples),
						                          new XElement("DurationMS", durationInMilliseconds)
						                         )
						            ));
				xmlDoc.Save(xmlFilePath);
			}
			 */
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
		
		private void AdsrSave() {
			
			string ampEnvelopeAttack = ampAttackTextBox.Text;
			string ampEnvelopeDecay = ampDecayTextBox.Text;
			string ampEnvelopeSustain = ampSustainTextBox.Text;
			string ampEnvelopeRelease = ampReleaseTextBox.Text;
			string modEnvelopeAttack = modAttackTextBox.Text;
			string modEnvelopeDecay = modDecayTextBox.Text;
			string modEnvelopeSustain = modSustainTextBox.Text;
			string modEnvelopeRelease = modReleaseTextBox.Text;
			string durationInSamples = durationSamplesTextBox.Text;
			string durationInMilliseconds = durationMsTextBox.Text;
			
			// store this in a xml ouput file.
			string xmlFilePath = "ADSR-measurement.xml";
			if (File.Exists(xmlFilePath)) {
				// add to existing xml document
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				
				xmlDoc.Element("ADSRMeasurement").Add(
					new XElement("Row",
					             new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
					             new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
					             new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
					             new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
					             new XElement("ModEnvelopeAttack", modEnvelopeAttack),
					             new XElement("ModEnvelopeDecay", modEnvelopeDecay),
					             new XElement("ModEnvelopeSustain", modEnvelopeSustain),
					             new XElement("ModEnvelopeRelease", modEnvelopeRelease),
					             new XElement("DurationSamples", durationInSamples),
					             new XElement("DurationMS", durationInMilliseconds)
					            ));

				xmlDoc.Save(xmlFilePath);
			} else {
				// create xml document first
				XDocument xmlDoc =
					new XDocument(
						new XElement("ADSRMeasurement",
						             new XElement("Row",
						                          new XElement("AmpEnvelopeAttack", ampEnvelopeAttack),
						                          new XElement("AmpEnvelopeDecay", ampEnvelopeDecay),
						                          new XElement("AmpEnvelopeSustain", ampEnvelopeSustain),
						                          new XElement("AmpEnvelopeRelease", ampEnvelopeRelease),
						                          new XElement("ModEnvelopeAttack", modEnvelopeAttack),
						                          new XElement("ModEnvelopeDecay", modEnvelopeDecay),
						                          new XElement("ModEnvelopeSustain", modEnvelopeSustain),
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
			
			MaxResolutionTrackBar.Maximum = 100;
			MaxResolutionTrackBar.TickFrequency = 10;

			this.waveDisplayUserControl1.Resolution = 1;
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
		}
		
		private void CropAudio() {
			VstHost host = VstHost.Instance;

			// crop the audio at silence
			float[] data = AudioUtils.CropAudioAtSilence(host.RecordedLeft.ToArray(), 0, false, 0);

			host.RecordedLeft.Clear();
			host.RecordedLeft.AddRange(data);
			
			MaxResolutionTrackBar.Maximum = (int) this.waveDisplayUserControl1.MaxResolution;
			MaxResolutionTrackBar.TickFrequency = MaxResolutionTrackBar.Maximum / 10;
			MaxResolutionTrackBar.Value = (int) this.waveDisplayUserControl1.MaxResolution;
			
			StartPositionTrackBar.Maximum = (int) this.waveDisplayUserControl1.NumberOfSamples;
			StartPositionTrackBar.TickFrequency = StartPositionTrackBar.Maximum / 10;
			StartPositionTrackBar.Value = 0;
			
			this.waveDisplayUserControl1.Resolution = (int) this.waveDisplayUserControl1.MaxResolution;
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
		
		void MeasureADSRParameter(int paramName, float paramValue, string envName) {
			
			System.Console.Out.WriteLine("MeasureADSREntry: Measuring {0} at value {1:0.00}...", envName, paramValue);
			
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
					System.Console.Out.WriteLine("MeasureADSREntry: Playing Midi Failed!");
					return;
				}
				System.Threading.Thread.Sleep(100);
			}
			
			// wait until it has stopped playing
			stopwatch.Restart();
			while (host.LastProcessedBufferLeftPlaying) {
				if (stopwatch.ElapsedMilliseconds > 40000) {
					stopwatch.Stop();
					System.Console.Out.WriteLine("MeasureADSREntry: Playing never stopped?!");
					break;
				}
			}
			
			// stop playing audio
			Playback.Stop();

			stopwatch.Stop();
			System.Console.Out.WriteLine("MeasureADSREntry: Playing stopped: {0} ms.", stopwatch.ElapsedMilliseconds);
			
			// stop recording
			host.Record = false;

			// wait a specfied time
			System.Threading.Thread.Sleep(100);

			// crop
			CropAudio();
			
			// get duration
			double durationInMilliseconds = this.waveDisplayUserControl1.DurationInMilliseconds;
			int durationInSamples = this.waveDisplayUserControl1.NumberOfSamples;
			
			// and store in GUI
			durationSamplesTextBox.Text = "" + durationInSamples;
			durationMsTextBox.Text = String.Format("{0:0.00}", durationInMilliseconds);
			
			// store this in a wav ouput file.
			float param = PluginContext.PluginCommandStub.GetParameter(paramName);
			string wavFilePath = String.Format("{0}{1:0.00}s-{2}.wav", envName, param, StringUtils.GetCurrentTimestamp());
			AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
			
			// store as a png
			System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 10000, 1, 0, host.SampleRate);
			string fileName = String.Format("{0}{1:0.00}s-{2}.png", envName, param, StringUtils.GetCurrentTimestamp());
			png.Save(fileName);
			
			// turn of midi
			host.SendMidiNote(host.SendContinousMidiNote, 0);

			// clear
			ClearAudio();

			// wait a specfied time
			System.Threading.Thread.Sleep(100);

			stopwatch.Stop();
		}
		
		
		void MeasureAmpABtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// step through the Attack steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK, paramValue, "amp-env-attack");
				
				ampAttackTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK);
				ampDecayTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY);
				ampSustainTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN);
				ampReleaseTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE);
				
				// store in xml file
				AdsrSave();
			}
		}
		
		void MeasureAmpDBtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// step through the Decay steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_DECAY, paramValue, "amp-env-decay");

				ampAttackTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK);
				ampDecayTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY);
				ampSustainTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN);
				ampReleaseTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE);

				// store in xml file
				AdsrSave();
			}
		}

		void MeasureAmpRBtnClick(object sender, EventArgs e)
		{
			MeasureAmpEnvelopeInit();
			
			// step through the Release steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE, paramValue, "amp-env-release");

				ampAttackTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_ATTACK);
				ampDecayTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_DECAY);
				ampSustainTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_SUSTAIN);
				ampReleaseTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_AMP_ENV1_RELEASE);

				// store in xml file
				AdsrSave();
			}
		}
		
		void MeasureModABtnClick(object sender, EventArgs e)
		{
			MeasureModEnvelopeInit();

			// step through the Attack steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK, paramValue, "mod-env-attack");

				modAttackTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK);
				modDecayTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_DECAY);
				modSustainTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_SUSTAIN);
				modReleaseTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_RELEASE);

				// store in xml file
				AdsrSave();
			}
		}
		
		void MeasureModDBtnClick(object sender, EventArgs e)
		{
			MeasureModEnvelopeInit();
			
			// step through the Decay steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				MeasureADSRParameter(SYLENTH_PARAM_MOD_ENV1_DECAY, paramValue, "mod-env-decay");

				modAttackTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_ATTACK);
				modDecayTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_DECAY);
				modSustainTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_SUSTAIN);
				modReleaseTextBox.Text = "" + PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_MOD_ENV1_RELEASE);

				// store in xml file
				AdsrSave();
			}
		}
		
		void MeasureLFOBtnClick(object sender, EventArgs e)
		{
			MeasureLFOInit();
			VstHost host = VstHost.Instance;
			
			List<string> lfoAlreadyProcessed = new List<string>();

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
						System.Console.Out.WriteLine("MeasureLFOBtnClick: Playing Midi Failed!");
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
						System.Console.Out.WriteLine("MeasureLFOBtnClick: Playing never stopped?!");
						break;
					}
				}
				// stop playing audio
				Playback.Stop();

				stopwatch.Stop();
				System.Console.Out.WriteLine("MeasureLFOBtnClick: Playing stopped: {0} ms. {1}", stopwatch.ElapsedMilliseconds, paramDisplay);
				
				// stop recording
				host.Record = false;

				// crop
				CropAudio();
				
				// store this in a wav ouput file.
				string wavFilePath = String.Format("audio-LFO-{0}-{1}.wav", StringUtils.MakeValidFileName(paramDisplay), StringUtils.GetCurrentTimestamp());
				AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
				
				// store as a png
				System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 10000, 1, 0, host.SampleRate);
				string fileName = String.Format("audio-LFO-{0}-{1}.png", StringUtils.MakeValidFileName(paramDisplay), StringUtils.GetCurrentTimestamp());
				png.Save(fileName);
				
				// store
				//AdsrSampleBtnClick(sender, e);
				
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
					this.waveDisplayUserControl1.Resolution = (int) this.waveDisplayUserControl1.MaxResolution;
				}
				this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
			}
		}
	}
}