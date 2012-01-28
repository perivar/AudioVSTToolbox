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

using CommonUtils.VST;

using NAudio.Wave;

namespace SynthAnalysisStudio
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
			System.Diagnostics.Debug.WriteLine("stringSize: {0}", stringSize);
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
		
		void FreqSampleBtnClick(object sender, EventArgs e)
		{
			string foundMaxFreq = String.Format("{0}", this.frequencyAnalyserUserControl1.FoundMaxFrequency);
			string foundMaxDB = String.Format("{0}", this.frequencyAnalyserUserControl1.FoundMaxDecibel);
			
			string filterACutoffString = ((HostCommandStub) PluginContext.HostCommandStub).filterACutoffString;
			string filterBCutoffString = ((HostCommandStub) PluginContext.HostCommandStub).filterBCutoffString;
			string filterCtrlCutoffString = ((HostCommandStub) PluginContext.HostCommandStub).filterCtrlCutoffString;

			float filterACutoff = ((HostCommandStub) PluginContext.HostCommandStub).filterACutoff;
			float filterBCutoff = ((HostCommandStub) PluginContext.HostCommandStub).filterBCutoff;
			float filterCtrlCutoff = ((HostCommandStub) PluginContext.HostCommandStub).filterCtrlCutoff;
			
			foundFreqTextBox.Text = foundMaxFreq;
			foundDBTextBox.Text = foundMaxDB;
			filterATextBox.Text = filterACutoffString;
			filterBTextBox.Text = filterBCutoffString;
			filterCtrlTextBox.Text = filterCtrlCutoffString;
			
			// store this in a xml ouput file.
			string xmlFilePath = "frequency-sampler.xml";
			if (File.Exists(xmlFilePath)) {
				// add to existing xml document
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				
				xmlDoc.Element("FrequencySampler").Add(
					new XElement("Row",
					             new XElement("FilterACutoffString", filterACutoffString),
					             new XElement("FilterACutoff", filterACutoff),
					             new XElement("FilterBCutoffString", filterBCutoffString),
					             new XElement("FilterBCutoff", filterBCutoff),
					             new XElement("FilterCtrlCutoffString", filterCtrlCutoffString),
					             new XElement("FilterCtrlCutoff", filterCtrlCutoff),
					             new XElement("FoundFrequency", foundMaxFreq)
					            ));

				xmlDoc.Save(xmlFilePath);
			} else {
				// create xml document first
				XDocument xmlDoc =
					new XDocument(
						new XElement("FrequencySampler",
						             new XElement("Row",
						                          new XElement("FilterACutoffString", filterACutoffString),
						                          new XElement("FilterACutoff", filterACutoff),
						                          new XElement("FilterBCutoffString", filterBCutoffString),
						                          new XElement("FilterBCutoff", filterBCutoff),
						                          new XElement("FilterCtrlCutoffString", filterCtrlCutoffString),
						                          new XElement("FilterCtrlCutoff", filterCtrlCutoff),
						                          new XElement("FoundFrequency", foundMaxFreq)
						                         )
						            ));
				xmlDoc.Save(xmlFilePath);
			}
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
