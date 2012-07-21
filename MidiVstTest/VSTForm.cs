using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
			this.SetClientSizeCore(rect.Width, rect.Height + 100);
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
			this.Close();
			//base.Dispose();

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
		}
	}
}
