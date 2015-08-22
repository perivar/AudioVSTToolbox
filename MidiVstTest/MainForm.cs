using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using NAudio.Midi;

using Jacobi.Vst.Core;

namespace MidiVstTest
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		VSTForm vstForm = null;
		MidiIn midiIn;
		MidiOut midiOut;
		bool isKeyDown = false;
		
		public static Dictionary<string, string> LastDirectoryUsed = new Dictionary<string, string>();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			for (int device = 0; device < MidiIn.NumberOfDevices; device++)
			{
				comboBoxMidiInDevices.Items.Add(MidiIn.DeviceInfo(device).ProductName);
			}
			if (comboBoxMidiInDevices.Items.Count > 0)
			{
				comboBoxMidiInDevices.SelectedIndex = 0;
			}
			for (int device = 0; device < MidiOut.NumberOfDevices; device++)
			{
				comboBoxMidiOutDevices.Items.Add(MidiOut.DeviceInfo(device).ProductName);
			}
			if (comboBoxMidiOutDevices.Items.Count > 0)
			{
				comboBoxMidiOutDevices.SelectedIndex = 0;
			}

			if (comboBoxMidiInDevices.Items.Count == 0)
			{
				//MessageBox.Show("No MIDI input devices available");
			} else {
				if (midiIn == null)
				{
					midiIn = new MidiIn(comboBoxMidiInDevices.SelectedIndex);
					midiIn.MessageReceived += new EventHandler<MidiInMessageEventArgs>(midiIn_MessageReceived);
					midiIn.ErrorReceived += new EventHandler<MidiInMessageEventArgs>(midiIn_ErrorReceived);
				}
				midiIn.Start();
				comboBoxMidiInDevices.Enabled = false;
			}
			
			if (comboBoxMidiOutDevices.Items.Count == 0)
			{
				MessageBox.Show("No MIDI output devices available");
			} else {
				if (midiOut == null)
				{
					midiOut = new MidiOut(comboBoxMidiOutDevices.SelectedIndex);
				}
			}
		}
		
		void LoadToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (vstForm != null)
			{
				vstForm.Dispose();
				vstForm = null;

				showToolStripMenuItem.Enabled = false;
				editParametersToolStripMenuItem.Enabled = false;
				loadToolStripMenuItem.Text = "Load...";
			}
			else
			{
				var ofd = new OpenFileDialog();
				ofd.Title = "Select VST:";
				ofd.Filter = "VST Files (*.dll)|*.dll";
				if (LastDirectoryUsed.ContainsKey("VSTDir")) {
					ofd.InitialDirectory = LastDirectoryUsed["VSTDir"];
				} else {
					ofd.InitialDirectory = UtilityAudio.GetVSTDirectory();
				}
				DialogResult res = ofd.ShowDialog();

				if (res != DialogResult.OK || !File.Exists(ofd.FileName)) return;

				try
				{
					if (LastDirectoryUsed.ContainsKey("VSTDir")) {
						LastDirectoryUsed["VSTDir"] = Directory.GetParent(ofd.FileName).FullName;
					} else {
						LastDirectoryUsed.Add("VSTDir", Directory.GetParent(ofd.FileName).FullName);
					}
					vstForm = new VSTForm(ofd.FileName);
					vstForm.Show();

					showToolStripMenuItem.Enabled = true;
					editParametersToolStripMenuItem.Enabled = true;

					loadToolStripMenuItem.Text = "Unload...";
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			
		}
		
		void ShowToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (vstForm != null)
			{
				if (vstForm.Visible) vstForm.BringToFront();
				else vstForm.Visible = true;
			}
		}
		
		void EditParametersToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (vstForm != null)
				vstForm.ShowEditParameters();
		}

		
		void SelectMIDIINToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			if (selectMIDIINToolStripMenuItem.Checked)
				comboBoxMidiInDevices.Enabled = true;
			else
			{
				comboBoxMidiInDevices.Enabled = false;
			}
			
		}
		
		void SelectMIDIOUTToolStripMenuItemCheckedChanged(object sender, EventArgs e)
		{
			if (selectMIDIOUTToolStripMenuItem.Checked)
				comboBoxMidiOutDevices.Enabled = true;
			else
			{
				comboBoxMidiOutDevices.Enabled = false;
			}
		}
		
		void TscMIDIINSelectedIndexChanged(object sender, EventArgs e)
		{
		}
		
		void TscMIDIOUTSelectedIndexChanged(object sender, EventArgs e)
		{
		}
		
		void midiIn_ErrorReceived(object sender, MidiInMessageEventArgs e)
		{
			progressLog1.LogMessage(Color.Red, String.Format("Time {0} Message 0x{1:X8} Event {2}",
			                                                 e.Timestamp, e.RawMessage, e.MidiEvent));
		}
		
		void midiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
		{
			progressLog1.LogMessage(Color.Blue, String.Format("Time {0} Message 0x{1:X8} Event {2}",
			                                                  e.Timestamp, e.RawMessage, e.MidiEvent));

			//SendMidiOutMessage(e.MidiEvent);
			if (VSTForm.vst != null) {
				MidiEvent midiEvent = e.MidiEvent;
				byte[] midiData = { 0, 0, 0 };
				if (midiEvent is NAudio.Midi.NoteEvent)
				{
					var me = (NAudio.Midi.NoteEvent) midiEvent;
					midiData = new byte[] {
						0x90, 					// Cmd
						(byte) me.NoteNumber,	// Val 1
						(byte) me.Velocity,		// Val 2
					};
				}
				else if (midiEvent is NAudio.Midi.ControlChangeEvent)
				{
					var cce = (NAudio.Midi.ControlChangeEvent) midiEvent;
					midiData = new byte[] {
						0xB0, 						// Cmd
						(byte)cce.Controller,		// Val 1
						(byte)cce.ControllerValue,	// Val 2
					};
				}
				else if (midiEvent is NAudio.Midi.PitchWheelChangeEvent)
				{
					// Pitch Wheel Value 0 is minimum, 0x2000 (8192) is default, 0x4000 (16384) is maximum
					NAudio.Midi.PitchWheelChangeEvent pe = (PitchWheelChangeEvent) midiEvent;
					midiData = new byte[] {
						0xE0, 							// Cmd
						(byte)(pe.Pitch & 0x7f),		// Val 1
						(byte)((pe.Pitch >> 7) & 0x7f),	// Val 2
					};
				}
				progressLog1.LogMessage(Color.Chocolate, String.Format("Sending mididata 0x00{0:X2}{1:X2}{2:X2}",
				                                                       midiData[2], midiData[1], midiData[0]));
				var vse =
					new VstMidiEvent(/*DeltaFrames*/ 0,
					                 /*NoteLength*/ 0,
					                 /*NoteOffset*/ 0,
					                 midiData,
					                 /*Detune*/ 0,
					                 /*NoteOffVelocity*/ 0);

				var ve = new VstEvent[1];
				ve[0] = vse;
				
				VSTForm.vst.pluginContext.PluginCommandStub.ProcessEvents(ve);
			}
		}
		
		void ButtonClearLogClick(object sender, EventArgs e)
		{
			progressLog1.ClearLog();
		}
		
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			// disable anoying beep sound when pressing down key
			e.SuppressKeyPress = true;
			
			if (isKeyDown) {
				return;
			}
			isKeyDown = true;
			
			// do what you want to do
			progressLog1.LogMessage(Color.Blue, String.Format("Key Down {0}, {1}",
			                                                  e.KeyCode, e.KeyValue));
			
			const byte midiVelocity = 100;
			byte midiNote = KeyEventArgToMidiNote(e);
			
			// only bother with the keys that trigger midi notes
			if (VSTForm.vst != null && midiNote != 0) {
				VSTForm.vst.MIDI_NoteOn(midiNote, midiVelocity);
			}
		}
		
		void MainFormKeyUp(object sender, KeyEventArgs e)
		{
			isKeyDown = false;

			// do you key up event, if any.
			progressLog1.LogMessage(Color.Blue, String.Format("Key Up {0}, {1}",
			                                                  e.KeyCode, e.KeyValue));
			const byte midiVelocity = 0;
			byte midiNote = KeyEventArgToMidiNote(e);
			
			// only bother with the keys that trigger midi notes
			if (VSTForm.vst != null && midiNote != 0) {
				VSTForm.vst.MIDI_NoteOn(midiNote, midiVelocity);
			}
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

					case Keys.Q:
						// C4
						midiNote = 60;
						break;
					case Keys.D2:
						// C#4
						midiNote = 61;
						break;
					case Keys.W:
						// D4
						midiNote = 62;
						break;
					case Keys.D3:
						// D#4
						midiNote = 63;
						break;
					case Keys.E:
						// E4
						midiNote = 64;
						break;
					case Keys.R:
						// F4
						midiNote = 65;
						break;
					case Keys.D5:
						// F#4
						midiNote = 66;
						break;
					case Keys.T:
						// G4
						midiNote = 67;
						break;
					case Keys.D6:
						// G#4
						midiNote = 68;
						break;
					case Keys.Y:
						// A4
						midiNote = 69;
						break;
					case Keys.D7:
						// A#4
						midiNote = 70;
						break;
					case Keys.U:
						// B4
						midiNote = 71;
						break;

					case Keys.I:
						// C4
						midiNote = 72;
						break;
					case Keys.D9:
						// C#4
						midiNote = 73;
						break;
					case Keys.O:
						// D4
						midiNote = 74;
						break;
					case Keys.D0:
						// D#4
						midiNote = 75;
						break;
					case Keys.P:
						// E4
						midiNote = 76;
						break;
				}
				
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return midiNote;
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (midiIn != null)
			{
				midiIn.Dispose();
				midiIn = null;
			}
			if (midiOut != null)
			{
				midiOut.Dispose();
				midiOut = null;
			}
			if (vstForm != null)
			{
				vstForm.Dispose();
				vstForm = null;
			}
			UtilityAudio.Dispose();
		}
	}
}
