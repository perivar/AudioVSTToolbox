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

		System.Timers.Timer guiRefreshTimer;
		public bool doGUIRefresh = false;
		
		public AnalyseForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			VstHost host = VstHost.Instance;
			this.frequencyAnalyserUserControl1.SetData(host.LastProcessedBuffer);

			StartGUIRefreshTimer();
		}
		
		public void RefreshGUI(object source, ElapsedEventArgs e)
		{
			VstHost host = VstHost.Instance;
			this.frequencyAnalyserUserControl1.SetData(host.LastProcessedBuffer);
			this.frequencyAnalyserUserControl1.Invalidate();
			this.frequencyAnalyserUserControl1.Update();
			//this.frequencyAnalyserUserControl1.Refresh();
		}
		
		public void StartGUIRefreshTimer()
		{
			guiRefreshTimer = new System.Timers.Timer();
			guiRefreshTimer.Interval = 4000;
			guiRefreshTimer.Elapsed += new ElapsedEventHandler(RefreshGUI);
			guiRefreshTimer.Enabled = true;
			
			// start gui refresh timer
			guiRefreshTimer.Start();
			doGUIRefresh = true;
		}
	}
}
