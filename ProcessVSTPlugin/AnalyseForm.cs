using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using CommonUtils.GUI;
using CommonUtils.VST;

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

		private bool DoGUIRefresh = true;
		
		public AnalyseForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			string stringSize = "" + this.frequencyAnalyserUserControl1.FFTWindowsSize;
			WindowsSizeComboBox.Text = stringSize;
			
			VstHost host = VstHost.Instance;
			this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
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
		
		void TrackBar1Scroll(object sender, EventArgs e)
		{
			this.frequencyAnalyserUserControl1.MaximumFrequency = (float) trackBar1.Value;
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			if (DoGUIRefresh) {
				VstHost host = VstHost.Instance;
				this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
			}
		}
	}
}
