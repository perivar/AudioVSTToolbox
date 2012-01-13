using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Data;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using ProcessVSTPlugin;

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
	}
}
