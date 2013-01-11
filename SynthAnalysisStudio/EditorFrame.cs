using System;
using System.Drawing;
using System.Windows.Forms;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;

using System.Collections.Generic;
using System.Diagnostics; // for stopwatch

using SynthAnalysisStudio;

using CommonUtils.VST;

using CommonUtils; // for BinaryFile
using CommonUtils.Diff; // for BinaryDiff
using DiffPlex; // for TextDiff
using DiffPlex.Model; // for TextDiff
using System.Xml;

using System.Linq;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// The frame in which a custom plugin editor UI is displayed.
	/// </summary>
	public partial class EditorFrame : Form
	{
		List<InvestigatedPluginPresetFileFormat> InvestigatedPluginPresetFileFormatList = new List<InvestigatedPluginPresetFileFormat>();
		
		VstPlaybackNAudio playback;
		bool hasNoKeyDown = true;
		public bool doGUIRefresh = true;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EditorFrame()
		{
			InitializeComponent();
			KeyPreview = true;
		}
		
		/// <summary>
		/// Gets or sets the Plugin Contex.
		/// </summary>
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
				PluginContext.PluginCommandStub.EditorOpen(this.pluginPanel.Handle);
			}
			
			// Some plugins have the following bug:
			// When calling PluginCommandStub.EditorGetRect(out wndRect) before opening the window,
			// the size returned is too small.
			// The fix is easy, calling EditorGetRect again after EditorOpen returns the correct size.
			if (PluginContext.PluginCommandStub.EditorGetRect(out wndRect))
			{
				this.pluginPanel.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
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
				doGUIRefresh = false;
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
			
			if (selectedIndex > 0) {
				presetComboBox.SelectedIndex = selectedIndex;
			}
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
					
					// only bother with the keys that trigger midi notes
					if (midiNote != 0) {
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
						
						host.SendMidiNote(midiNote, midiVelocity);
						
						hasNoKeyDown = false; // Set to False to disable keyboard Auto Repeat
					}
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
				
				// only bother with the keys that trigger midi notes
				if (midiNote != 0) {
					VstHost host = VstHost.Instance;
					host.PluginContext = this.PluginContext;
					
					host.SendMidiNote(midiNote, midiVelocity);
				}
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
		
		void InvestigatePluginPresetFileCheckboxCheckedChanged(object sender, EventArgs e)
		{
			CheckBox check = (CheckBox) sender;
			if(check.Checked)
			{
				((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= true;
			} else {
				((HostCommandStub) PluginContext.HostCommandStub).DoInvestigatePluginPresetFileFormat= false;
			}
		}
		
		void PresetContentBtnClick(object sender, System.EventArgs e)
		{
			InvestigatedPluginPresetDetailsForm dlg = new InvestigatedPluginPresetDetailsForm();
			dlg.PluginContext = this.PluginContext;
			dlg.InvestigatedPluginPresetFileFormatList = ((HostCommandStub) PluginContext.HostCommandStub).InvestigatedPluginPresetFileFormatList;
			
			//dlg.ShowDialog(this); // modal
			dlg.Show(); // modeless
		}
		
		void TextDiffCheckboxCheckedChanged(object sender, EventArgs e)
		{
			CheckBox check = (CheckBox) sender;
			if(check.Checked)
			{
				((HostCommandStub) PluginContext.HostCommandStub).InvestigatePluginPresetFileFormatDiffType = DiffType.Text;
			} else {
				((HostCommandStub) PluginContext.HostCommandStub).InvestigatePluginPresetFileFormatDiffType = DiffType.Binary;
			}
		}
		
		void MidiNoteCheckboxCheckedChanged(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			
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
			
			CheckBox check = (CheckBox) sender;
			if(check.Checked)
			{
				host.SendMidiNote(host.SendContinousMidiNote, host.SendContinousMidiNoteVelocity);
			} else {
				host.SendMidiNote(host.SendContinousMidiNote, 0);
			}
		}
		
		void AnalyseBtnClick(object sender, EventArgs e)
		{
			VstHost host = VstHost.Instance;
			host.PluginContext = this.PluginContext;
			host.doPluginOpen();
			
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

			AnalyseForm dlg = new AnalyseForm();
			dlg.PluginContext = this.PluginContext;
			dlg.Playback = playback;
			
			//dlg.ShowDialog(this); // modal
			dlg.Show(); // modeless
		}
		
		void WaveBtnClick(object sender, EventArgs e)
		{
			WaveDisplayForm dlg2 = new WaveDisplayForm();
			dlg2.PluginContext = this.PluginContext;
			dlg2.Playback = playback;
			
			//dlg2.ShowDialog(this); // modal
			dlg2.Show(); // modeless
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			// Call these three functions 'getEditorSize', 'processIdle' and 'processReplacing' continually while the GUI is open.
			// If size don't change and you don't need to process audio call the functions anyway because plugins can rely on them being called frequently for their redrawing.
			// http://vstnet.codeplex.com/discussions/281497
			
			// In fact all I had to call was  Jacobi.Vst.Core.Host.IVstPluginCommandStub.EditorIdle()
			// which I do every 100 ms.  This works great ;)
			if (doGUIRefresh) {
				PluginContext.PluginCommandStub.EditorIdle();
			}
		}
		
		void BtnAutoAllAutomatedClick(object sender, EventArgs e)
		{
			// time how long this takes
			Stopwatch stopwatch = Stopwatch.StartNew();
			
			Dictionary<string, int> processedParameters = new Dictionary<string, int>();
			int paramCount = PluginContext.PluginInfo.ParameterCount;
			for (int paramIndex = 0; paramIndex < paramCount; paramIndex++)
			{
				string paramName = PluginContext.PluginCommandStub.GetParameterName(paramIndex);
				string paramLabel = PluginContext.PluginCommandStub.GetParameterLabel(paramIndex);
				
				// check if this plugin has more than one parameter with the same name
				if (!processedParameters.ContainsKey(paramName)) {
					processedParameters.Add(paramName, 1);
				} else {
					processedParameters[paramName]++;
					paramName = paramName + processedParameters[paramName];
				}
				
				// initialize
				PluginContext.PluginCommandStub.SetParameter(paramIndex, 0);
				
				// step through the steps
				for (float paramValue = 1.0f; paramValue >= 0.0f; paramValue -= 0.025f) {
					System.Console.Out.WriteLine("Measuring {0}/{1} {2} at value {3:0.00} ...", paramIndex, paramCount, paramName, paramValue);

					byte[] previousChunkData = PluginContext.PluginCommandStub.GetChunk(true);
					
					// set the parameters
					PluginContext.PluginCommandStub.SetParameter(paramIndex, paramValue);
					//((HostCommandStub) PluginContext.HostCommandStub).SetParameterAutomated(paramIndex, paramValue);
					
					byte[] chunkData = PluginContext.PluginCommandStub.GetChunk(true);
					
					DetectChunkChanges(previousChunkData, chunkData, paramName, paramLabel, String.Format("{0:0.00}", paramValue));
					
					// wait
					System.Threading.Thread.Sleep(10);
				}

				// wait a bit longer
				System.Threading.Thread.Sleep(10);
			}
			
			// store to xml file
			//set formatting options
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			string outputFile = "\\output2.xml";

			using (XmlWriter writer = XmlWriter.Create(Application.StartupPath+outputFile, settings))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("TrackedPresetFileChanges");

				for (int i = 0; i < InvestigatedPluginPresetFileFormatList.Count; i++)
				{
					InvestigatedPluginPresetFileFormat row = InvestigatedPluginPresetFileFormatList[i];
					writer.WriteStartElement("Row");
					writer.WriteElementString("IndexInFile", "" + row.IndexInFile);
					writer.WriteElementString("ByteValue", "" + row.ByteValue);
					writer.WriteElementString("ByteValueHexString", row.ByteValueHexString);
					writer.WriteElementString("ParameterName", row.ParameterName);
					writer.WriteElementString("ParameterNameFormatted", row.ParameterNameFormatted);
					writer.WriteElementString("ParameterLabel", row.ParameterLabel);
					writer.WriteElementString("ParameterDisplay", row.ParameterDisplay);
					writer.WriteElementString("TextChanges", row.TextChanges);
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
				writer.WriteEndDocument();
				MessageBox.Show("Information in the the table is successfully saved in the following location: \n" + Application.StartupPath + outputFile, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		
		
		private void DetectChunkChanges(byte[] previousChunkData, byte[] chunkData, string name, string label, string display) {

			bool debugChunkFiles = true;
			DiffType InvestigatePluginPresetFileFormatDiffType = DiffType.Binary;
			
			// do a binary comparison between what changed before and after this method was called
			
			// if the chunk is not empty, try to detect what has changed
			if (chunkData != null && chunkData.Length > 0) {
				int chunkLength = chunkData.Length;
				
				// binary comparison to find out where the chunk has changed
				if (previousChunkData != null && previousChunkData.Length > 0) {
					
					if (debugChunkFiles) {
						BinaryFile.ByteArrayToFile("Preset Chunk Data - previousChunkData.dat", previousChunkData);
						BinaryFile.ByteArrayToFile("Preset Chunk Data - chunkData.dat", chunkData);
					}
					
					if (InvestigatePluginPresetFileFormatDiffType == DiffType.Binary) {
						SimpleBinaryDiff.Diff diff = SimpleBinaryDiff.GetDiff(previousChunkData, chunkData);
						if (diff != null) {
							System.Diagnostics.Debug.WriteLine("BinDiff: {0}", diff);
							
							// store each of the chunk differences in a list
							foreach (SimpleBinaryDiff.DiffPoint point in diff.Points) {
								this.InvestigatedPluginPresetFileFormatList.Add(
									new InvestigatedPluginPresetFileFormat(point.Index, point.NewValue, name, label, display));
							}
						}
					} else if (InvestigatePluginPresetFileFormatDiffType == DiffType.Text) {
						// assume we are dealing with text and not binary data
						var d = new Differ();
						string OldText = BinaryFile.ByteArrayToString(previousChunkData);
						string NewText = BinaryFile.ByteArrayToString(chunkData);
						
						DiffResult res = d.CreateWordDiffs(OldText, NewText, true, true, new[] {' ', '\r', '\n'});
						//DiffResult res = d.CreateCharacterDiffs(OldText, NewText, true, true);

						List<UnidiffEntry> diffList = UnidiffSeqFormater.GenerateWithLineNumbers(res);
						var queryTextDiffs = from dl in diffList
							where dl.Type == UnidiffType.Insert
							select dl;
						
						foreach (var e in queryTextDiffs) {
							string text = e.Text;
							text = text.Replace("\n", "");
							if (text != "") {
								System.Diagnostics.Debug.WriteLine(String.Format("TextDiff: {0} {1}", e.Index, text));
								
								this.InvestigatedPluginPresetFileFormatList.Add(
									new InvestigatedPluginPresetFileFormat(e.Index, 0, name, label, display, text));
							}
						}
					}
				}
			}
		}
	}
}