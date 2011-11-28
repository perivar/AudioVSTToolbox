using System;
using System.Drawing;
using System.Windows.Forms;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// The frame in which a custom plugin editor UI is displayed.
	/// </summary>
	public partial class EditorFrame : Form
	{
		VstPlaybackNAudio playback;
		bool hasNoKeyDown = true;

		/// <summary>
		/// Default ctor.
		/// </summary>
		public EditorFrame()
		{
			InitializeComponent();
			KeyPreview = true;
		}

		/// <summary>
		/// Gets or sets the Plugin Command Stub.
		/// </summary>
		//public Jacobi.Vst.Core.Host.IVstPluginCommandStub PluginCommandStub { get; set; }
		
		public VstPluginContext PluginContext { get; set; }
		
		/// <summary>
		/// Shows the custom plugin editor UI.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public new DialogResult ShowDialog(IWin32Window owner)
		{
			Rectangle wndRect = new Rectangle();

			this.Text = PluginContext.PluginCommandStub.GetEffectName();

			if (PluginContext.PluginCommandStub.EditorGetRect(out wndRect))
			{
				this.pluginPanel.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
				//PluginContext.PluginCommandStub.EditorOpen(this.Handle);
				PluginContext.PluginCommandStub.EditorOpen(this.pluginPanel.Handle);
			}

			FillProgram(0);
			return base.ShowDialog(owner);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			if (e.Cancel == false)
			{
				PluginContext.PluginCommandStub.EditorClose();
			}
		}
		
		private void FillProgram(int selectedIndex)
		{
			presetComboBox.Items.Clear();
			string[] programs = new String[PluginContext.PluginInfo.ProgramCount];
			for (int i = 0; i < PluginContext.PluginInfo.ProgramCount; i++) {
				PluginContext.PluginCommandStub.SetProgram(i);
				//int count = PluginContext.PluginCommandStub.GetProgram();
				string name = PluginContext.PluginCommandStub.GetProgramName();
				programs[i] = name;
			}
			presetComboBox.Items.AddRange(programs);
			//presetComboBox.DataSource = programs;
			presetComboBox.SelectedIndex = selectedIndex;
			
			/*
			 * comboBox1.DataSource = myArray;
			 * 
			 * For the first variant you can only use strings as items, while with data binding you can bind a collection of more complex objects. You can then specify what properties are displayed:
			 * comboBox1.DisplayMember = "Name";
			 * 
			 * and what are treated as value:
			 * comboBox1.ValueMember = "ID";
			 * 
			 * You can access the original object that is selected later with
			 * comboBox1.SelectedItem
			 * 
			 * or the value with
			 * comboBox1.SelectedValue
			 * 
			 * The value is the property you specified with ValueMember
			 */
		}

		void SaveBtnClick(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|Effect Bank Files (.fxb)|*.fxb|All Files|*.*||";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				string comboBoxStringValue = presetComboBox.Text;
				this.PluginContext.PluginCommandStub.SetProgramName(comboBoxStringValue);
				string fxpFilePath = dialog.FileName;
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;
				host.SaveFXP(fxpFilePath);
				FillProgram(PluginContext.PluginCommandStub.GetProgram());
			}
		}
		
		void LoadBtnClick(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|Effect Bank Files (.fxb)|*.fxb|All Files|*.*||";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				string fxpFilePath = dialog.FileName;
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;
				
				host.LoadFXP(fxpFilePath);
				FillProgram(PluginContext.PluginCommandStub.GetProgram());
			}
		}
		
		void PresetComboBoxSelectedValueChanged(object sender, EventArgs e)
		{
			int index = presetComboBox.SelectedIndex;
			PluginContext.PluginCommandStub.SetProgram(index);
			//FillParameterList();
		}
		
		private byte KeyEventArgToMidiNote(KeyEventArgs e) {
			/*
			 * You can use the keyboard of your computer to play notes in a 2-octave range.
			 * It starts from Z at C3 and goes horizontally across the bottom 2 rows of the keyboard to M playing B3
			 * (i.e. Z X C V B N M play the natural notes and S D G H J play the sharps and flats).
			 * It continues with Q playing C4 and uses the top 2 rows of the keyboard to P playing E5
			 * (i.e. Q W E R T Y U I O P play the natural notes and 2 3 5 6 7 9 0 play the sharps and flats).
			 * http://lmms.sourceforge.net/wiki/index.php/File:Keyboard-pianoKeys1.png
			 * http://highlyliquid.com/support/library/midi-note-numbers/
			 * http://vstnet.codeplex.com/discussions/234945
			 */

			byte midiNote = 0;
			try
			{
				switch (e.KeyCode)
				{
					case Keys.Z:
						// C3
						midiNote = 48;
						break;
					case Keys.S:
						// C#3
						midiNote = 49;
						break;
					case Keys.X:
						// D3
						midiNote = 50;
						break;
					case Keys.D:
						// D#3
						midiNote = 51;
						break;
					case Keys.C:
						// E3
						midiNote = 52;
						break;
					case Keys.V:
						// F3
						midiNote = 53;
						break;
					case Keys.G:
						// F#3
						midiNote = 54;
						break;
					case Keys.B:
						// G3
						midiNote = 55;
						break;
					case Keys.H:
						// G#3
						midiNote = 56;
						break;
					case Keys.N:
						// A3
						midiNote = 57;
						break;
					case Keys.J:
						// A#3
						midiNote = 58;
						break;
					case Keys.M:
						// B3
						midiNote = 59;
						break;
				}
				
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return midiNote;
		}
		
		void EditorFrameKeyDown(object sender, KeyEventArgs e)
		{
			if ((PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				return;
			}
			
			if (hasNoKeyDown)  {
				try
				{
					byte midiVelocity = 100;
					byte midiNote = KeyEventArgToMidiNote(e);
					
					System.Diagnostics.Debug.WriteLine("Key Down Event Detected: {0}, {1}, {2}", e.KeyCode, midiNote, midiVelocity);
					
					VstHost host = VstHost.Instance;
					host.PluginContext = this.PluginContext;
					
					// if first keypress setup audio
					if (playback == null) {
						// with iblock=1...Nblocks and blocksize = Fs * tblock. Fs = 44100 and
						// tblock = 0.15 makes blocksize = 6615.
						int sampleRate = 44100;
						int blockSize = (int) (sampleRate * 0.15f); //6615;
						int channels = 2;
						host.Init(blockSize, sampleRate, channels);
						
						playback = new VstPlaybackNAudio(host);
						playback.Play();
					}
					
					if (midiNote != 0) host.SendMidiNote(midiNote, midiVelocity);
					
					hasNoKeyDown = false; // Set to False to disable keyboard Auto Repeat
				} catch (Exception ex) {
					MessageBox.Show(ex.Message);
				}
			} else {
				hasNoKeyDown = false; // Set to False to disable keyboard Auto Repeat
			}
		}
		
		void EditorFrameKeyUp(object sender, KeyEventArgs e)
		{
			if ((PluginContext.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
			{
				return;
			}

			hasNoKeyDown = true; // Set to True to enable next KeyDown event
			
			try
			{
				byte midiVelocity = 0;
				byte midiNote = KeyEventArgToMidiNote(e);
				
				System.Diagnostics.Debug.WriteLine("Key Up Event Detected: {0}, {1}, {2}", e.KeyCode, midiNote, midiVelocity);
				
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;
				
				if (midiNote != 0) host.SendMidiNote(midiNote, midiVelocity);
				
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		
		void EditorFrameFormClosing(object sender, FormClosingEventArgs e)
		{
			if (playback != null) {
				playback.Stop();
			}
		}
	}
}
