using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Timers;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using ProcessVSTPlugin;

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
			//this.waveDisplayUserControl1.SetAudioData(host.LastProcessedBufferLeft);
			
			StartGUIRefreshTimer();
		}
		
		public void RefreshGUI(object source, ElapsedEventArgs e)
		{
			if (DoGUIRefresh) {
				VstHost host = VstHost.Instance;
				this.waveDisplayUserControl1.SetAudioData(host.RecordedLeft.ToArray());
				//this.waveDisplayUserControl1.SetAudioData(host.LastProcessedBufferLeft);
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
		
		void TrackBar1Scroll(object sender, EventArgs e)
		{
			this.waveDisplayUserControl1.Resolution = trackBar1.Value;
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
		}
		
		void ClearBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.ClearRecording();
		}
	}
}
