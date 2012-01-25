using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CommonUtils.Audio.NAudio;
using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;
using NAudio.Wave;
using ProcessVSTPlugin;
using CommonUtils;
using CommonUtils.Audio;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WaveDisplayForm.
	/// </summary>
	public partial class WaveDisplayForm : Form
	{
		const int SYLENTH_PARAM_ATTACK = 0;
		const int SYLENTH_PARAM_DECAY = 1;
		const int SYLENTH_PARAM_RELEASE = 2;
		const int SYLENTH_PARAM_SUSTAIN = 3;
		const int SYLENTH_PARAM_LFO1_GAIN = 59;
		const int SYLENTH_PARAM_LFO1_RATE = 61;
		const int SYLENTH_PARAM_LFO1_WAVE = 62;
		const int SYLENTH_PARAM_OSC1_VOLUME = 91;
		const int SYLENTH_PARAM_OSC1_WAVE = 92;
		const int SYLENTH_PARAM_XMODLFO1DEST1AMOUNT = 201;
		const int SYLENTH_PARAM_XMODLFO1DEST2AMOUNT = 202;
		const int SYLENTH_PARAM_YMODLFO1DEST1 = 229;
		const int SYLENTH_PARAM_YMODLFO1DEST2 = 230;

		public VstPluginContext PluginContext { get; set; }
		public VstPlaybackNAudio Playback { get; set; }

		private System.Timers.Timer guiRefreshTimer;
		public bool DoGUIRefresh = false;
		
		public WaveDisplayForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			VstHost host = VstHost.Instance;
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
			
			StartGUIRefreshTimer();
		}
		
		public void RefreshGUI(object source, ElapsedEventArgs e)
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
		
		public void StartGUIRefreshTimer()
		{
			guiRefreshTimer = new System.Timers.Timer();
			guiRefreshTimer.Interval = 200;
			guiRefreshTimer.Elapsed += new ElapsedEventHandler(RefreshGUI);
			guiRefreshTimer.Enabled = true;
			
			DoGUIRefresh = true;

			// start gui refresh timer
			guiRefreshTimer.Start();
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
			VstHost host = VstHost.Instance;
			host.ClearRecording();
			
			MaxResolutionTrackBar.Maximum = 100;
			MaxResolutionTrackBar.TickFrequency = 10;

			this.waveDisplayUserControl1.Resolution = 1;
			this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
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
		
		void AdsrSampleBtnClick(object sender, EventArgs e)
		{

			float envelopeAttack = ((HostCommandStub) PluginContext.HostCommandStub).envelopeAttack;
			float envelopeDecay = ((HostCommandStub) PluginContext.HostCommandStub).envelopeDecay;
			float envelopeSustain = ((HostCommandStub) PluginContext.HostCommandStub).envelopeSustain;
			float envelopeRelease = ((HostCommandStub) PluginContext.HostCommandStub).envelopeRelease;
			
			string envelopeAttackString = ((HostCommandStub) PluginContext.HostCommandStub).envelopeAttackString;
			string envelopeDecayString = ((HostCommandStub) PluginContext.HostCommandStub).envelopeDecayString;
			string envelopeSustainString = ((HostCommandStub) PluginContext.HostCommandStub).envelopeSustainString;
			string envelopeReleaseString = ((HostCommandStub) PluginContext.HostCommandStub).envelopeReleaseString;

			attackTextBox.Text = envelopeAttackString;
			decayTextBox.Text = envelopeDecayString;
			sustainTextBox.Text = envelopeSustainString;
			releaseTextBox.Text = envelopeReleaseString;
			
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
					             new XElement("EnvelopeAttackString", envelopeAttackString),
					             new XElement("EnvelopeAttack", envelopeAttack),
					             new XElement("EnvelopeDecayString", envelopeDecayString),
					             new XElement("EnvelopeDecay", envelopeDecay),
					             new XElement("EnvelopeSustainString", envelopeSustainString),
					             new XElement("EnvelopeSustain", envelopeSustain),
					             new XElement("EnvelopeReleaseString", envelopeReleaseString),
					             new XElement("EnvelopeRelease", envelopeRelease),
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
						                          new XElement("EnvelopeAttackString", envelopeAttackString),
						                          new XElement("EnvelopeAttack", envelopeAttack),
						                          new XElement("EnvelopeDecayString", envelopeDecayString),
						                          new XElement("EnvelopeDecay", envelopeDecay),
						                          new XElement("EnvelopeSustainString", envelopeSustainString),
						                          new XElement("EnvelopeSustain", envelopeSustain),
						                          new XElement("EnvelopeReleaseString", envelopeReleaseString),
						                          new XElement("EnvelopeRelease", envelopeRelease),
						                          new XElement("DurationSamples", numberOfSamples),
						                          new XElement("DurationMS", durationInMilliseconds)
						                         )
						            ));
				xmlDoc.Save(xmlFilePath);
			}
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
		
		void MeasureABtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_ATTACK, 0); // attack
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_DECAY, 0); // decay
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_SUSTAIN, 0); // sustain
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_RELEASE, 0); // release
			
			// step through the Attack steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				// time how long this takes
				Stopwatch stopwatch = Stopwatch.StartNew();

				// init the buffers
				host.ClearRecording();
				host.ClearLastProcessedBuffers();

				// start record
				host.Record = true;
				
				// set the parameter
				PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_ATTACK, paramValue);
				((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(SYLENTH_PARAM_ATTACK, paramValue);
				
				// wait until it has started playing
				while(!host.LastProcessedBufferLeftPlaying) {
					// start playing audio
					Playback.Play();

					// play midi
					host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);

					if (stopwatch.ElapsedMilliseconds > 5000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureABtnClick: Playing Midi Failed!");
						return;
					}
					System.Threading.Thread.Sleep(100);
				}
				
				// wait until it has stopped playing
				stopwatch.Restart();
				while (host.LastProcessedBufferLeftPlaying) {
					if (stopwatch.ElapsedMilliseconds > 40000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureABtnClick: Playing never stopped?!");
						break;
					}
				}
				// stop playing audio
				Playback.Stop();

				stopwatch.Stop();
				System.Console.Out.WriteLine("MeasureABtnClick: Playing stopped: {0} ms.", stopwatch.ElapsedMilliseconds);
				
				// stop recording
				host.Record = false;

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				// crop
				CropBtnClick(sender, e);
				
				// store this in a wav ouput file.
				float param = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_ATTACK);
				string wavFilePath = String.Format("audio-attack-{0:0.00}s-{1}.wav", param, StringUtils.GetCurrentTimestamp());
				AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
				
				// store as a png
				System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 10000, 1, 0, host.SampleRate);
				string fileName = String.Format("audio-attack-{0:0.00}s-{1}.png", param, StringUtils.GetCurrentTimestamp());
				png.Save(fileName);
				
				// store
				AdsrSampleBtnClick(sender, e);
				
				// turn of midi
				host.SendMidiNote(host.SendContinousMidiNote, 0);

				// clear
				ClearBtnClick(sender, e);

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				stopwatch.Stop();
			}
		}
		
		void MeasureDBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);

			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_ATTACK, 0); // attack
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_DECAY, 0); // decay
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_SUSTAIN, 0); // sustain
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_RELEASE, 0); // release
			
			// step through the Decay steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				// time how long this takes
				Stopwatch stopwatch = Stopwatch.StartNew();

				// init the buffers
				host.ClearRecording();
				host.ClearLastProcessedBuffers();
				
				// start record
				host.Record = true;
				
				// set the parameter
				PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_DECAY, paramValue);
				((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(SYLENTH_PARAM_DECAY, paramValue);
				
				// wait until it has started playing
				while(!host.LastProcessedBufferLeftPlaying) {
					// start playing audio
					Playback.Play();

					// play midi
					host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
					
					if (stopwatch.ElapsedMilliseconds > 5000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureDBtnClick: Playing Midi Failed!");
						return;
					}
					System.Threading.Thread.Sleep(100);
				}
				
				// wait until it has stopped playing
				stopwatch.Restart();
				while (host.LastProcessedBufferLeftPlaying) {
					if (stopwatch.ElapsedMilliseconds > 40000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureDBtnClick: Playing never stopped?!");
						break;
					}
				}
				// stop playing audio
				Playback.Stop();

				stopwatch.Stop();
				System.Console.Out.WriteLine("MeasureDBtnClick: Playing stopped: {0} ms.", stopwatch.ElapsedMilliseconds);
				
				// stop recording
				host.Record = false;

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				// crop
				CropBtnClick(sender, e);
				
				// store this in a wav ouput file.
				float param = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_DECAY);
				string wavFilePath = String.Format("audio-decay-{0:0.00}s-{1}.wav", param, StringUtils.GetCurrentTimestamp());
				AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
				
				// store as a png
				System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 10000, 1, 0, host.SampleRate);
				string fileName = String.Format("audio-decay-{0:0.00}s-{1}.png", param, StringUtils.GetCurrentTimestamp());
				png.Save(fileName);
				
				// store
				AdsrSampleBtnClick(sender, e);
				
				// turn of midi
				host.SendMidiNote(host.SendContinousMidiNote, 0);

				// clear
				ClearBtnClick(sender, e);

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				stopwatch.Stop();
			}
		}

		void MeasureRBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_ATTACK, 0); // attack
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_DECAY, 0); // decay
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_SUSTAIN, 10); // sustain
			
			// step through the Release steps
			for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.020f) {
				// time how long this takes
				Stopwatch stopwatch = Stopwatch.StartNew();

				// init the buffers
				host.ClearRecording();
				host.ClearLastProcessedBuffers();

				// start record
				host.Record = true;

				// set the parameter
				PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_RELEASE, paramValue);
				((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(SYLENTH_PARAM_RELEASE, paramValue);
				
				// wait until it has started playing
				while(!host.LastProcessedBufferLeftPlaying) {
					// start playing audio
					Playback.Play();

					// play midi
					host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
					
					if (stopwatch.ElapsedMilliseconds > 5000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureRBtnClick: Playing Midi Failed!");
						return;
					}
					System.Threading.Thread.Sleep(100);
				}
				// stop midi
				host.SendMidiNote(host.SendContinousMidiNote, 0);
				
				// wait until it has stopped playing
				stopwatch.Restart();
				while (host.LastProcessedBufferLeftPlaying) {
					if (stopwatch.ElapsedMilliseconds > 40000) {
						stopwatch.Stop();
						System.Console.Out.WriteLine("MeasureRBtnClick: Playing never stopped?!");
						break;
					}
				}
				// stop playing audio
				Playback.Stop();

				stopwatch.Stop();
				System.Console.Out.WriteLine("MeasureRBtnClick: Playing stopped: {0} ms.", stopwatch.ElapsedMilliseconds);
				
				// stop recording
				host.Record = false;

				// crop
				CropBtnClick(sender, e);
				
				// store this in a wav ouput file.
				float param = PluginContext.PluginCommandStub.GetParameter(SYLENTH_PARAM_RELEASE);
				string wavFilePath = String.Format("audio-release-{0:0.00}s-{1}.wav", param, StringUtils.GetCurrentTimestamp());
				AudioUtilsNAudio.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat(host.SampleRate, 1));
				
				// store as a png
				System.Drawing.Bitmap png = CommonUtils.FFT.AudioAnalyzer.DrawWaveform(host.RecordedLeft.ToArray(), new System.Drawing.Size(1000, 600), 10000, 1, 0, host.SampleRate);
				string fileName = String.Format("audio-release-{0:0.00}s-{1}.png", param, StringUtils.GetCurrentTimestamp());
				png.Save(fileName);
				
				// store
				AdsrSampleBtnClick(sender, e);
				
				// clear
				ClearBtnClick(sender, e);

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				stopwatch.Stop();
			}
		}
		
		void MeasureLFOBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			SetupAudio(host);
			
			List<string> lfoAlreadyProcessed = new List<string>();
			
			// init
			//((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_ATTACK, 0); // attack
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_DECAY, 0); // decay
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_SUSTAIN, 1.0f); // sustain
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_GAIN, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_RATE, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_LFO1_WAVE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.LFOWAVE.LFO_Pulse));
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_OSC1_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_OSC1_WAVE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.OSCWAVE.OSC_Pulse));
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_XMODLFO1DEST1AMOUNT, 1);
			
			PluginContext.PluginCommandStub.SetParameter(SYLENTH_PARAM_YMODLFO1DEST1,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.YMODDEST.VolumeAB));
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
				//((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(SYLENTH_PARAM_LFO1_RATE, paramValue);
				
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
				//System.Threading.Thread.Sleep(3*count);
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
				CropBtnClick(sender, e);
				
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
				ClearBtnClick(sender, e);

				// wait a specfied time
				System.Threading.Thread.Sleep(100);

				stopwatch.Stop();
				
				count++;
			}
		}
	}
}