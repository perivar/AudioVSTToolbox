using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Data;

using System.Threading;
using System.Diagnostics;

using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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
				host.DoSendContinousMidiNote = true;
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
			} else {
				host.DoSendContinousMidiNote = false;
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
			AudioUtils.CreateWaveFile(host.RecordedLeft.ToArray(), wavFilePath, new WaveFormat());
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
		
		void MeasureABtnClick(object sender, EventArgs e)
		{
			// step through the Attack steps
			for (float paramValue = 0.0f; paramValue <= 10; paramValue += 0.1f) {
				PluginContext.PluginCommandStub.SetParameter(0, paramValue);
			}
		}
		
		void MeasureDBtnClick(object sender, EventArgs e)
		{
			// step through the Decay steps
			for (float paramValue = 0.0f; paramValue <= 10; paramValue += 0.1f) {
				PluginContext.PluginCommandStub.SetParameter(1, paramValue);
			}
		}

		void MeasureRBtnClick(object sender, EventArgs e)
		{		
			// init
			((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			PluginContext.PluginCommandStub.SetParameter(0, 0); // attack
			PluginContext.PluginCommandStub.SetParameter(1, 0); // decay
			PluginContext.PluginCommandStub.SetParameter(3, 10); // sustain
			
			// step through the Release steps
			for (float paramValue = 0.0f; paramValue <= 2; paramValue += 0.1f) {
				PluginContext.PluginCommandStub.SetParameter(2, paramValue);
				((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(2, paramValue);
				
				// wait a specfied time
				//System.Threading.Thread.Sleep(10);

				// start record
				RecordBtnClick(sender, e);
				
				// wait a specfied time
				//System.Threading.Thread.Sleep(10);

				// play midi
				PlayMidiC5100msBtnClick(sender, e);
				
				// wait a specfied time
				System.Threading.Thread.Sleep((int)(10000 * paramValue));
				
				// stop recording
				StopBtnClick(sender, e);
				
				// crop
				CropBtnClick(sender, e);
				
				// save wav
				SaveWAVBtnClick(sender, e);
				
				// store
				AdsrSampleBtnClick(sender, e);
				
				// clear
				ClearBtnClick(sender, e);

				// wait a specfied time
				System.Threading.Thread.Sleep(10);
			}
		}
	}
}
