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
		
		// store freq measurment variables
		string foundMaxFreq = "";
		string foundMaxDB = "";
		string filterACutoffDisplay = "";
		float filterACutoff = 0.0f;
		string filterAType = "";
		string filterADB = "";
		string filterBCutoffDisplay = "";
		float filterBCutoff = 0.0f;
		string filterBType = "";
		string filterBDB = "";
		string filterCtrlCutoffDisplay = "";
		float filterCtrlCutoff = 0.0f;
		
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
			int windowsSize = 4096;
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
		
		void RetrieveFilterInfo() {
			
			// get found info from user control
			this.foundMaxFreq = String.Format("{0}", this.frequencyAnalyserUserControl1.FoundMaxFrequency);
			this.foundMaxDB = String.Format("{0}", this.frequencyAnalyserUserControl1.FoundMaxDecibel);

			// get filter Info from plugin
			this.filterACutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF);
			this.filterACutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF);
			this.filterAType = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_TYPE);
			this.filterADB = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_DB);
			this.filterBCutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_CUTOFF);
			this.filterBCutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERB_CUTOFF);
			this.filterBType = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_TYPE);
			this.filterBDB = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_DB);
			this.filterCtrlCutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF);
			this.filterCtrlCutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF);
		}
		
		void FreqSampleBtnClick(object sender, EventArgs e)
		{
			RetrieveFilterInfo();
			
			foundFreqTextBox.Text = this.foundMaxFreq;
			foundDBTextBox.Text = this.foundMaxDB;
			filterATextBox.Text = this.filterACutoffDisplay;
			filterBTextBox.Text = this.filterBCutoffDisplay;
			filterCtrlTextBox.Text = this.filterCtrlCutoffDisplay;
			
			// store this in a xml ouput file.
			string xmlFilePath = "frequency-measurement.xml";
			if (File.Exists(xmlFilePath)) {
				// add to existing xml document
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				
				xmlDoc.Element("FrequencyMeasurement").Add(
					new XElement("Row",
					             new XElement("FilterACutoffDisplay", filterACutoffDisplay),
					             new XElement("FilterACutoff", filterACutoff),
					             new XElement("FilterAType", filterAType),
					             new XElement("FilterADB", filterADB),
					             new XElement("FilterBCutoffDisplay", filterBCutoffDisplay),
					             new XElement("FilterBCutoff", filterBCutoff),
					             new XElement("FilterBType", filterBType),
					             new XElement("FilterBDB", filterBDB),
					             new XElement("FilterCtrlCutoffDisplay", filterCtrlCutoffDisplay),
					             new XElement("FilterCtrlCutoff", filterCtrlCutoff),
					             new XElement("FoundFrequency", foundMaxFreq)
					            ));

				xmlDoc.Save(xmlFilePath);
			} else {
				// create xml document first
				XDocument xmlDoc =
					new XDocument(
						new XElement("FrequencyMeasurement",
						             new XElement("Row",
						                          new XElement("FilterACutoffDisplay", filterACutoffDisplay),
						                          new XElement("FilterACutoff", filterACutoff),
						                          new XElement("FilterAType", filterAType),
						                          new XElement("FilterADB", filterADB),
						                          new XElement("FilterBCutoffDisplay", filterBCutoffDisplay),
						                          new XElement("FilterBCutoff", filterBCutoff),
						                          new XElement("FilterBType", filterBType),
						                          new XElement("FilterBDB", filterBDB),
						                          new XElement("FilterCtrlCutoffDisplay", filterCtrlCutoffDisplay),
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
		
		void MeasureFrequencyInit() {
			
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_MAIN_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_MIX_A, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_MIX_B, 0.5f);
			
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_AMP_ENV1_ATTACK, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_AMP_ENV1_DECAY, 0.0f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_AMP_ENV1_SUSTAIN, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_AMP_ENV1_RELEASE, 0.0f);
			
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_OSC1_VOLUME, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_OSC1_WAVE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.OSCWAVE.OSC_Noise));
			
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_RESO, 1.0f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_DB,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.FILTERDB.DB24));
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_TYPE,
			                                             PresetConverter.Sylenth1Preset.EnumUintToFloat(
			                                             	(uint) PresetConverter.Sylenth1Preset.FILTERTYPE.Lowpass));
			
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF, 0.5f);
			PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_RESO, 1.0f);
			
		}

		void PrepFreqAnalysisBtnClick(object sender, EventArgs e)
		{
			MeasureFrequencyInit();
		}
	}
}
