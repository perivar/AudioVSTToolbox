using System;
using System.Drawing;
using System.Windows.Forms;

using CommonUtils.VSTPlugin;

// Copied from the microDRUM project
// https://github.com/microDRUM
// I think it is created by massimo.bernava@gmail.com
// Modified by perivar@nerseth.com
namespace MidiVstTest
{
	public partial class VSTForm : Form
	{
		public static VSTForm Singleton = null;
		public static VST vst = null;

		public bool doGUIRefresh = true;
		EditParametersForm edit = new EditParametersForm();

		public VSTForm(string VSTPath)
		{
			Singleton = this;
			UtilityAudio.OpenAudio(AudioLibrary.NAudio);

			InitializeComponent();
			
			vst = UtilityAudio.LoadVST(VSTPath, this.Handle);
			this.Text = vst.pluginContext.PluginCommandStub.GetProgramName();
			Rectangle rect = new Rectangle();
			vst.pluginContext.PluginCommandStub.EditorGetRect(out rect);
			this.SetClientSizeCore(rect.Width, rect.Height + 125);
			vst.StreamCall += new EventHandler<VSTStreamEventArgs>(vst_StreamCall);
			
			UtilityAudio.StartAudio();
		}

		void vst_StreamCall(object sender, VSTStreamEventArgs e)
		{
			waveformPainter1.AddMax(e.MaxL);
			waveformPainter2.AddMax(e.MaxR);
		}

		public new void Dispose()
		{
			UtilityAudio.DisposeVST();
			vst = null;
			base.Dispose();

			Singleton = null;
		}

		private void VSTForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Singleton = null;
		}

		internal void ShowEditParameters()
		{
			edit.AddParameters(vst.pluginContext);
			edit.Show();
		}

		private void VSTForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}
		
		private void tsbPlay_Click(object sender, EventArgs e)
		{
			UtilityAudio.PlayMP3();
			tsbStop.Image = (System.Drawing.Image) new System.ComponentModel.ComponentResourceManager(typeof(VSTForm)).GetObject("tsbPause.Image");

		}

		private void tsbLoad_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Title = "Select MP3 file:";
			fileDialog.Filter = "MP3 Files (*.mp3)|*.mp3";
			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName)) return;

			UtilityAudio.LoadMP3(fileDialog.FileName);

			tslTotalTime.Text = " / " + UtilityAudio.GetMp3TotalTime().ToString();
		}

		private void tsbStop_Click(object sender, EventArgs e)
		{
			try
			{
				if (UtilityAudio.IsMP3Played())
				{
					UtilityAudio.PauseMP3();
					tsbStop.Image = (System.Drawing.Image) new System.ComponentModel.ComponentResourceManager(typeof(VSTForm)).GetObject("tsbStop.Image");
				}
				else
				{
					UtilityAudio.StopMp3();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void tsbSave_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFile = new SaveFileDialog();
			saveFile.Title = "Select output file:";
			saveFile.Filter = "WAV Files (*.wav)|*.wav";
			saveFile.ShowDialog();

			UtilityAudio.SaveStream(saveFile.FileName);

		}

		private void tsbRec_CheckedChanged(object sender, EventArgs e)
		{
			if (tsbRec.Checked)
			{
				tsbRec.BackColor = Color.Red;
				UtilityAudio.StartStreamingToDisk();
				
			}
			else
			{
				tsbRec.BackColor = Color.Transparent;
				UtilityAudio.StopStreamingToDisk();
			}
		}

		private void tsbMixer_Click(object sender, EventArgs e)
		{
			UtilityAudio.ShowMixer();
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			// Call these three functions 'getEditorSize', 'processIdle' and 'processReplacing' continually while the GUI is open.
			// If size don't change and you don't need to process audio call the functions anyway because plugins can rely on them being called frequently for their redrawing.
			// http://vstnet.codeplex.com/discussions/281497
			
			// In fact all I had to call was  Jacobi.Vst.Core.Host.IVstPluginCommandStub.EditorIdle()
			// which I do every 100 ms.  This works great ;)
			if (vst != null && doGUIRefresh) {
				vst.pluginContext.PluginCommandStub.EditorIdle();
			}

			// update play time
			tslNowTime.Text = UtilityAudio.GetMp3CurrentTime().ToString();
		}
	}
}
