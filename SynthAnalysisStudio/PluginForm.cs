using System;
using System.Windows.Forms;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using NAudio.Wave;

using CommonUtils.VST;

namespace SynthAnalysisStudio
{
	partial class PluginForm : Form
	{
		VstPlaybackNAudio playback;
		string waveInputFilePath = "";
		
		public PluginForm()
		{
			InitializeComponent();
		}

		public VstPluginContext PluginContext { get; set; }

		private void DataToForm()
		{
			FillPropertyList();
			FillProgram();
			FillParameterList();
		}

		private void FillPropertyList()
		{
			PluginPropertyListVw.Items.Clear();

			// plugin product
			AddProperty("Plugin Name", PluginContext.PluginCommandStub.GetEffectName());
			AddProperty("Product", PluginContext.PluginCommandStub.GetProductString());
			AddProperty("Vendor", PluginContext.PluginCommandStub.GetVendorString());
			AddProperty("Vendor Version", PluginContext.PluginCommandStub.GetVendorVersion().ToString());
			AddProperty("Vst Support", PluginContext.PluginCommandStub.GetVstVersion().ToString());
			AddProperty("Plugin Category", PluginContext.PluginCommandStub.GetCategory().ToString());
			
			// plugin info
			AddProperty("Flags", PluginContext.PluginInfo.Flags.ToString());
			AddProperty("Plugin ID", PluginContext.PluginInfo.PluginID.ToString());
			AddProperty("Plugin Version", PluginContext.PluginInfo.PluginVersion.ToString());
			AddProperty("Audio Input Count", PluginContext.PluginInfo.AudioInputCount.ToString());
			AddProperty("Audio Output Count", PluginContext.PluginInfo.AudioOutputCount.ToString());
			AddProperty("Initial Delay", PluginContext.PluginInfo.InitialDelay.ToString());
			AddProperty("Program Count", PluginContext.PluginInfo.ProgramCount.ToString());
			AddProperty("Parameter Count", PluginContext.PluginInfo.ParameterCount.ToString());
			AddProperty("Tail Size", PluginContext.PluginCommandStub.GetTailSize().ToString());

			// can do
			AddProperty("CanDo: " + VstPluginCanDo.Bypass, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.MidiProgramNames, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MidiProgramNames)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.Offline, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.ReceiveVstEvents, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstEvents)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.ReceiveVstMidiEvent, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.ReceiveVstTimeInfo, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstTimeInfo)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.SendVstEvents, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstEvents)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.SendVstMidiEvent, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstMidiEvent)).ToString());

			AddProperty("CanDo: " + VstPluginCanDo.ConformsToWindowRules, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ConformsToWindowRules)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.Metapass, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Metapass)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.MixDryWet, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MixDryWet)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.Multipass, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Multipass)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.NoRealTime, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.PlugAsChannelInsert, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsChannelInsert)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.PlugAsSend, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsSend)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.SendVstTimeInfo, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstTimeInfo)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x1in1out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in1out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x1in2out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in2out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x2in1out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in1out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x2in2out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in2out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x2in4out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in4out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x4in2out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in2out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x4in4out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in4out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x4in8out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in8out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x8in4out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in4out)).ToString());
			AddProperty("CanDo: " + VstPluginCanDo.x8in8out, PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in8out)).ToString());
		}

		private void AddProperty(string propName, string propValue)
		{
			ListViewItem lvItem = new ListViewItem(propName);
			lvItem.SubItems.Add(propValue);

			PluginPropertyListVw.Items.Add(lvItem);
		}

		private void FillProgram()
		{
			ProgramIndexNud.Value = PluginContext.PluginCommandStub.GetProgram();
			ProgramNameTxt.Text = PluginContext.PluginCommandStub.GetProgramName();
		}

		private void FillParameterList()
		{
			PluginParameterListVw.Items.Clear();

			for (int i = 0; i < PluginContext.PluginInfo.ParameterCount; i++)
			{
				string name = PluginContext.PluginCommandStub.GetParameterName(i);
				string label = PluginContext.PluginCommandStub.GetParameterLabel(i);
				string display = PluginContext.PluginCommandStub.GetParameterDisplay(i);
				bool canBeAutomated = PluginContext.PluginCommandStub.CanParameterBeAutomated(i);

				AddParameter(i, name, display, label, String.Empty, canBeAutomated);
			}
		}

		private void AddParameter(int paramIndex, string paramName, string paramValue, string label, string shortLabel, bool canBeAutomated)
		{
			ListViewItem lvItem = new ListViewItem(paramIndex.ToString());
			lvItem.SubItems.Add(paramName);
			lvItem.SubItems.Add(paramValue);
			lvItem.SubItems.Add(label);
			lvItem.SubItems.Add(shortLabel);
			lvItem.SubItems.Add(canBeAutomated.ToString());

			PluginParameterListVw.Items.Add(lvItem);
		}

		private void PluginForm_Load(object sender, EventArgs e)
		{
			if (PluginContext == null)
			{
				Close();
			}
			else
			{
				DataToForm();
			}
		}

		private void ProgramIndexNud_ValueChanged(object sender, EventArgs e)
		{
			if (ProgramIndexNud.Value < PluginContext.PluginInfo.ProgramCount &&
			    ProgramIndexNud.Value >= 0)
			{
				PluginContext.PluginCommandStub.SetProgram((int)ProgramIndexNud.Value);

				FillProgram();
				FillParameterList();
			}
		}
		
		void ProcessAudioBtnClick(object sender, EventArgs e)
		{
			// plugin does not support processing audio
			if ((PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				MessageBox.Show(this, "This plugin does not process any audio.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
                
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;			
			if (waveInputFilePath != "") {
				host.InputWave = waveInputFilePath;
				// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
				// tblock = 0.15 makes blocksize = 6615.
				int sampleRate = 44100;
				int blockSize = (int) (sampleRate * 0.15f); //6615;
				int channels = 2;
				host.Init(blockSize, sampleRate, channels);
	
				if (playback == null) { 
					playback = new VstPlaybackNAudio(host);
					playback.Play();
				} else {
					// toogle start or stop
					if (playback.PlaybackDevice.PlaybackState == PlaybackState.Playing) {
						playback.Stop();
					} else if (playback.PlaybackDevice.PlaybackState == PlaybackState.Stopped) {
						playback.Play();
					}
				}
			} else {
				MessageBox.Show(this, "Please choose an audio file to process.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);				
			}
		}
				
		private void GenerateNoiseBtn_Click(object sender, EventArgs e)
		{
			// plugin does not support processing audio
			if ((PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				MessageBox.Show(this, "This plugin does not process any audio.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			int inputCount = PluginContext.PluginInfo.AudioInputCount;
			int outputCount = PluginContext.PluginInfo.AudioOutputCount;
			int blockSize = 1024;

			// wrap these in using statements to automatically call Dispose and cleanup the unmanaged memory.
			using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
			{
				using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
				{
					foreach (VstAudioBuffer buffer in inputMgr.ToArray())
					{
						try {
							Random rnd = new Random((int)DateTime.Now.Ticks);
							for (int i = 0; i < blockSize; i++)
							{
								// generate a value between -1.0 and 1.0
								buffer[i] = (float)((rnd.NextDouble() * 2.0) - 1.0);
							}
						} catch (OverflowException oe) {
							System.Diagnostics.Debug.WriteLine(oe);
						}
					}

					PluginContext.PluginCommandStub.SetBlockSize(blockSize);
					PluginContext.PluginCommandStub.SetSampleRate(44100f);

					VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
					VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

					PluginContext.PluginCommandStub.MainsChanged(true);
					PluginContext.PluginCommandStub.StartProcess();
					PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);
					PluginContext.PluginCommandStub.StopProcess();
					PluginContext.PluginCommandStub.MainsChanged(false);

					for (int i = 0; i < inputBuffers.Length && i < outputBuffers.Length; i++)
					{
						for (int j = 0; j < blockSize; j++)
						{
							if (inputBuffers[i][j] != outputBuffers[i][j])
							{
								if (outputBuffers[i][j] != 0.0)
								{
									MessageBox.Show(this, "The plugin has processed the audio.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
									return;
								}
							}
						}
					}

					MessageBox.Show(this, "The plugin has passed the audio unchanged to its outputs.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}

		private void EditorBtn_Click(object sender, EventArgs e)
		{
			EditorFrame dlg = new EditorFrame();
			dlg.PluginContext = PluginContext;
			
			// TODO: Commenting out these disables all sound - due to Sylenth bug?
			//PluginContext.PluginCommandStub.MainsChanged(true);
			dlg.ShowDialog(this);
			//PluginContext.PluginCommandStub.MainsChanged(false);
			FillParameterList();				
		}
        
        void LoadFXPBtnClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|Effect Bank Files (.fxb)|*.fxb|All Files|*.*||";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string fxpFilePath = dialog.FileName;
                VstHost host = VstHost.Instance;
                host.PluginContext = this.PluginContext;

                host.LoadFXP(fxpFilePath);
                FillProgram();
                FillParameterList();
            }
        }
        
        void SaveFXPBtnClick(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|Effect Bank Files (.fxb)|*.fxb|All Files|*.*||";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string comboBoxStringValue = ProgramNameTxt.Text;
                this.PluginContext.PluginCommandStub.SetProgramName(comboBoxStringValue);
                string fxpFilePath = dialog.FileName;
                VstHost host = VstHost.Instance;
                host.PluginContext = this.PluginContext;
                host.SaveFXP(fxpFilePath);
                FillProgram();
                FillParameterList();
            }
        }
        
        void BtnChooseWavefileClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Wave files (.wav)|*.wav|All Files|*.*||";
            if (waveInputFilePath != "") dialog.FileName = waveInputFilePath;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.waveInputFilePath = dialog.FileName;
            }        	
        }
	}
}
