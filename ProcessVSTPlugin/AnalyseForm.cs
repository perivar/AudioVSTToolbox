using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Timers;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using NAudio.Wave;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of AnalyseForm.
	/// </summary>
	public partial class AnalyseForm : Form
	{
		public VstPluginContext PluginContext { get; set; }
		public VstPlaybackNAudio Playback { get; set; }

		private System.Timers.Timer guiRefreshTimer;
		public bool DoGUIRefresh = false;
		
		public AnalyseForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			VstHost host = VstHost.Instance;
			this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
			
			WindowsSizeComboBox.SelectedItem = this.frequencyAnalyserUserControl1.FFTWindowsSize;
			StartGUIRefreshTimer();
		}
		
		public void RefreshGUI(object source, ElapsedEventArgs e)
		{
			if (DoGUIRefresh) {
				VstHost host = VstHost.Instance;
				this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
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
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = (ComboBox) sender;
			string stringSize = (string) comboBox.SelectedItem;
			int windowsSize = 2048;
			int.TryParse(stringSize, out windowsSize);
			this.frequencyAnalyserUserControl1.FFTWindowsSize = windowsSize;
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
	}
}
