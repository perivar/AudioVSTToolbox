using System;
using System.Windows.Forms;
using System.IO;

using System.Diagnostics; // for stopwatch

using Jacobi.Vst.Interop.Host;

using System.Linq;
using System.Xml.Linq;

using CommonUtils.VST;

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
			Debug.WriteLine("stringSize: {0}", stringSize);
			WindowsSizeComboBox.Text = stringSize;
			
			VstHost host = VstHost.Instance;
			this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
		}
		
		void FFTWindowsSizeSelectedIndexChanged(object sender, EventArgs e)
		{
			var comboBox = (ComboBox) sender;
			string stringSize = (string) comboBox.SelectedItem;
			int windowsSize = 4096;
			int.TryParse(stringSize, out windowsSize);
			this.frequencyAnalyserUserControl1.FFTWindowsSize = windowsSize;
		}
		
		void OnOffCheckboxCheckedChanged(object sender, EventArgs e)
		{
			var check = (CheckBox) sender;
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
			this.filterACutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF).Trim();
			this.filterACutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF);
			this.filterAType = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_TYPE).Trim();
			this.filterADB = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERA_DB).Trim();
			this.filterBCutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_CUTOFF).Trim();
			this.filterBCutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERB_CUTOFF);
			this.filterBType = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_TYPE).Trim();
			this.filterBDB = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERB_DB).Trim();
			this.filterCtrlCutoffDisplay = PluginContext.PluginCommandStub.GetParameterDisplay(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF).Trim();
			this.filterCtrlCutoff = PluginContext.PluginCommandStub.GetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF);
		}
		
		private void FrequencySave() {
			
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
				var xmlDoc =
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
		
		void InitManualFreqMeasurementBtnClick(object sender, EventArgs e)
		{
			MeasureFrequencyInit();
		}

		void FreqSampleBtnClick(object sender, EventArgs e)
		{
			RetrieveFilterInfo();
			FrequencySave();
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

		void SylenthAutoMeasureFilters() {
			VstHost host = VstHost.Instance;
			
			MeasureFrequencyInit();
			
			// time how long this takes
			Stopwatch stopwatch = Stopwatch.StartNew();

			// wait until the plugin has started playing some noise
			while(!host.LastProcessedBufferLeftPlaying) {
				// start playing audio
				Playback.Play();

				// play midi
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);

				if (stopwatch.ElapsedMilliseconds > 5000) {
					stopwatch.Stop();
					Console.Out.WriteLine("AutoMeasureFreq: Playing Midi Failed!");
					return;
				}
				System.Threading.Thread.Sleep(100);
			}
			
			// step through the filter steps
			for (float paramFilterAValue = 1.0f; paramFilterAValue >= 0.0f; paramFilterAValue -= 0.020f) {
				for (float paramFilterCtlValue = 1.0f; paramFilterCtlValue >= 0.0f; paramFilterCtlValue -= 0.020f) {
					stopwatch.Restart();

					Console.Out.WriteLine("AutoMeasureFreq: Measuring {0} at value {1:0.00} and {2} at value {3:0.00} ...", "filterACutoff", paramFilterAValue, "filterCtlCutoff", paramFilterCtlValue);

					// set the parameters
					PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF, paramFilterAValue);
					((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(WaveDisplayForm.SYLENTH_PARAM_FILTERA_CUTOFF, paramFilterAValue);
					PluginContext.PluginCommandStub.SetParameter(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF, paramFilterCtlValue);
					((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(WaveDisplayForm.SYLENTH_PARAM_FILTERCTL_CUTOFF, paramFilterCtlValue);
					
					// wait
					System.Threading.Thread.Sleep(200);
					
					this.frequencyAnalyserUserControl1.SetAudioData(host.LastProcessedBufferLeft);
					
					// wait
					System.Threading.Thread.Sleep(200);
					
					// create a snapshot of the current freqency
					RetrieveFilterInfo();
					
					stopwatch.Stop();
					Console.Out.WriteLine("AutoMeasureFreq: Used {0} ms. Found max frequency {1:00.00} at {2:00.00} dB.", stopwatch.ElapsedMilliseconds, this.foundMaxFreq, this.foundMaxDB);
					
					// store as a png
					//string fileName = String.Format("frequency-measurement-{0}-{1}.png", StringUtils.MakeValidFileName(String.Format("{0:00.00} {1:00.00} {2:00.00}", this.foundMaxFreq, paramFilterAValue, paramFilterCtlValue)), StringUtils.GetCurrentTimestamp());
					//this.frequencyAnalyserUserControl1.Bitmap.Save(fileName);
					
					// store in xml file
					float maxFreq = 0.0f;
					float.TryParse(this.foundMaxFreq, out maxFreq);
					if (maxFreq > 0) {
						FrequencySave();
					}
				}
			}

			// stop midi
			host.SendMidiNote(host.SendContinousMidiNote, 0);
			
			// stop playing audio
			Playback.Stop();
		}
		
		void SylenthAutoMeasureFreqBtnClick(object sender, EventArgs e)
		{
			SylenthAutoMeasureFilters();
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
