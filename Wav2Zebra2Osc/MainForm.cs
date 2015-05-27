using System;
using System.Windows.Forms;
using System.IO;

using CommonUtils;
using CommonUtils.Audio;
using CommonUtils.FFT;

namespace Wav2Zebra2Osc
{
	/// <summary>
	/// Main Form
	/// </summary>
	public partial class MainForm : Form
	{
		const string iniFileName = "Wav2Zebra2.ini";

		float[][] soundData;
		float[][] morphedData;
		float[] sineData;
		
		string rawExportName;
		BassProxy audioSystem;
		
		public Wav2Zebra2Osc.WaveDisplayUserControl[] waveDisplays;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Initialize Bass
			audioSystem = BassProxy.Instance;

			// Ensure the paint methods are called if resized
			ResizeRedraw = true;
			
			// Initialize the wave display cells
			waveDisplays = new WaveDisplayUserControl[16];
			int counter = 0;
			for( int row = 0; row != 4; ++row ) {
				for( int col = 0; col != 4; ++col ) {
					this.waveDisplays[counter] = new WaveDisplayUserControl(this);
					this.tableLayoutPanel.Controls.Add(waveDisplays[counter], col, row);
					this.waveDisplays[counter].ResumeLayout(false);
					this.waveDisplays[counter].PerformLayout();
					counter++;
				}
			}
			this.waveDisplays[0].Selected = true;

			// Initalize the jagged data arrays
			this.soundData = RectangularArrays.ReturnRectangularFloatArray(16, 128);
			this.morphedData = RectangularArrays.ReturnRectangularFloatArray(16, 128);
			
			// generate the sine data
			this.sineData = OscillatorGenerator.Sine();
			
			// set sine data to first and last element
			Array.Copy(this.sineData, 0, this.morphedData[0], 0, 128);
			Array.Copy(this.sineData, 0, this.morphedData[15], 0, 128);
			
			this.waveDisplays[0].Loaded = true;
			this.waveDisplays[15].Loaded = true;
			this.waveDisplays[0].MorphedData = this.morphedData[0];
			this.waveDisplays[15].MorphedData = this.morphedData[15];
			this.waveDisplays[0].Refresh();
			this.waveDisplays[15].Refresh();
			
			string pathName = ReadExportPathName();
			this.OutputText = "Export path is: " + pathName;
		}

		#region Read and Write Export Path
		private string ReadExportPathName() {
			string pathName = "";
			if (File.Exists(iniFileName)) {
				try {
					StreamReader reader = File.OpenText(iniFileName);
					pathName = reader.ReadLine();
					reader.Close();
				} catch (Exception e) {
					System.Diagnostics.Debug.WriteLine(e);
				}
			}
			return pathName;
		}
		
		private void WriteExportPathName(string pathName) {
			try
			{
				StreamWriter writer = File.CreateText(iniFileName);
				writer.WriteLine(pathName);
				writer.Close();
			} catch (IOException e) {
				System.Diagnostics.Debug.WriteLine(e);
			}
		}
		#endregion
		
		#region DoShow methods
		public bool DoShowRAWWaves
		{
			get
			{
				return this.showRAWRadioButton.Checked;
			}
		}

		public bool DoShowMorphedWaves
		{
			get
			{
				return this.showMorphedRadioButton.Checked;
			}
		}

		public bool DoExportMorphedWaves
		{
			get
			{
				return this.exportMorphedWavesCheckBox.Checked;
			}
		}

		public bool DoExportRAWWaves
		{
			get
			{
				return this.exportRAWWavesCheckBox.Checked;
			}
		}
		#endregion

		#region Properties
		public string OutputText
		{
			set
			{
				this.outputField.Text = value;
			}
			get
			{
				return this.outputField.Text;
			}
		}

		public int SelectedWaveDisplay
		{
			get
			{
				int selected = -1;
				for (int i = 0; i < 16; i++)
				{
					if (this.waveDisplays[i].Selected)
					{
						selected = i;
					}
				}
				return selected;
			}
		}
		
		/// <summary>
		/// check that at least one file are loaded and set the rawExportName property
		/// </summary>
		private bool HasExportFileNames
		{
			get
			{
				string beginning = "Default";
				string end = "Default";
				for (int i = 0; i < 16; i++)
				{
					string temp = this.waveDisplays[i].FileName;
					if ((beginning == "Default") && !string.IsNullOrEmpty(temp))
					{
						beginning = Path.GetFileName(this.waveDisplays[i].FileName);
					}
				}
				
				for (int i = 15; i >= 0; i--)
				{
					string temp = this.waveDisplays[i].FileName;
					if ((end == "Default") && !string.IsNullOrEmpty(temp))
					{
						end = Path.GetFileName(this.waveDisplays[i].FileName);
					}
				}
				
				beginning = RemoveFileSuffix(beginning);
				end = RemoveFileSuffix(end);
				beginning = RemoveWhiteSpace(beginning);
				end = RemoveWhiteSpace(end);
				
				this.rawExportName = (beginning + "To" + end);
				
				return (end != "Default") || (beginning != "Default");
			}
		}
		#endregion
		
		#region Export Method
		public void ExportToZebra2(bool doExportMorphed, bool doExportRaw)
		{
			// keep on asking for file path until it is succesfully stored
			// in the ini file
			while (!File.Exists(iniFileName)) {
				this.OutputText = "Export path is not set";
				SetExportPath();
				this.OutputText = "";
			}
			
			string pathName = ReadExportPathName();
			
			if (!HasExportFileNames) {
				this.OutputText = "There's nothing to export.";
			} else {
				string exportName = "";
				
				#region Morphed
				if (doExportMorphed) {
					exportName = pathName + Path.DirectorySeparatorChar + this.exportFileName.Text;
					exportName = FixExportNames(exportName, true);
					exportName = RenameIfFileExists(exportName);
					
					try
					{
						StreamWriter writer = File.CreateText(exportName);
						string temp1 = "";
						temp1 = temp1 + "#defaults=no \n";
						temp1 = temp1 + "#cm=OSC \n";
						temp1 = temp1 + "Wave=2 \n";
						temp1 = temp1 + "<? \n\n";
						temp1 = temp1 + "float Wave[ 128 ]; \n";
						
						for (int j = 0; j < 16; j++)
						{
							for (int i = 0; i < 128; i++)
							{
								string sampleValue = "" + this.morphedData[j][i];
								if ((this.morphedData[j][i] < 0.001F) && (this.morphedData[j][i] > -0.001F))
								{
									sampleValue = "0.0";
								}
								temp1 = temp1 + "Wave[" + i + "] = " + sampleValue + "; \n";
							}
							temp1 = temp1 + "Selected.WaveTable.set( " + (j + 1) + " , Wave ); \n\n";
						}
						
						temp1 = temp1 + "?> \n";
						writer.Write(temp1);
						writer.Close();
					} catch (IOException e) {
						System.Diagnostics.Debug.WriteLine(e);
					}
				}
				#endregion
				
				#region RAW
				if (doExportRaw)
				{
					exportName = pathName + Path.DirectorySeparatorChar + this.exportFileName.Text;
					exportName = FixExportNames(exportName, false);
					exportName = RenameIfFileExists(exportName);
					
					try
					{
						StreamWriter writer = File.CreateText(exportName);
						string temp1 = "";
						temp1 = temp1 + "#defaults=no \n";
						temp1 = temp1 + "#cm=OSC \n";
						temp1 = temp1 + "Wave=2 \n";
						temp1 = temp1 + "<? \n\n";
						temp1 = temp1 + "float Wave[ 128 ]; \n";
						
						for (int j = 0; j < 16; j++)
						{
							if (this.waveDisplays[j].FileName != "")
							{
								for (int i = 0; i < 128; i++)
								{
									string sampleValue = "" + this.soundData[j][i];
									if ((this.soundData[j][i] < 0.001F) && (this.soundData[j][i] > -0.001F))
									{
										sampleValue = "0.0";
									}
									temp1 = temp1 + "Wave[" + i + "] = " + sampleValue + "; \n";
								}
								temp1 = temp1 + "Selected.WaveTable.set( " + (j + 1) + " , Wave ); \n\n";
							}
						}
						
						temp1 = temp1 + "?> \n";
						writer.Write(temp1);
						writer.Close();
					} catch (IOException e) {
						System.Diagnostics.Debug.WriteLine(e);
					}
				}
				#endregion

				this.outputField.Text = "File exported as: " + exportName;
			}
		}
		#endregion
		
		public void SetExportPath()
		{
			var folderDialog = new FolderBrowserDialog();
			folderDialog.Description = "Set export path";
			
			DialogResult retval = folderDialog.ShowDialog();
			if (retval == DialogResult.OK)
			{
				string pathName = folderDialog.SelectedPath;
				WriteExportPathName(pathName);
				this.OutputText = "Export path is: " + pathName;
			}
		}

		public void SetImportSound(FileInfo file, int selected)
		{
			float[] tempAudioBuffer = BassProxy.ReadMonoFromFile(file.FullName);
			float[] tempAudioBuffer2 = MathUtils.Resample(tempAudioBuffer, 128);
			
			Array.Copy(tempAudioBuffer2, 0, this.soundData[selected], 0, 128);
			
			// Interpolate using the raw waves
			this.morphedData[selected] = this.soundData[selected];
			
			this.waveDisplays[selected].WaveData = this.soundData[selected];
			this.waveDisplays[selected].MorphedData = this.morphedData[selected];
			this.waveDisplays[selected].Refresh();
		}
		
		public void LoadCell()
		{
			var fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = true;
			fileDialog.Filter = audioSystem.FileFilter;
			fileDialog.Title = "Select an audio file";

			this.OutputText = "";
			int selected = this.SelectedWaveDisplay;
			if (selected > -1)
			{
				DialogResult retval = fileDialog.ShowDialog();
				if (retval == DialogResult.OK)
				{
					string[] files = fileDialog.FileNames;
					int count = files.Length;
					for (int i = 0; (i < count) && (i + selected < 16); i++)
					{
						try
						{
							SetImportSound(new FileInfo(files[i]), selected + i);
							this.waveDisplays[(selected + i)].FileName = files[i];
						} catch (Exception e) {
							this.OutputText = "Not an audiofile";
							System.Diagnostics.Debug.WriteLine(e);
						}
					}
				}
			} else {
				this.OutputText = "No slot selected. Click on one to select slot.";
			}
			
			bool unused = HasExportFileNames;
			this.exportFileName.Text = this.rawExportName;
			this.waveDisplays[selected].Loaded = true;
			CalculateGhosts();
		}

		private void CalculateGhosts()
		{
			int fromPos = 0;
			int toPos = 0;
			while (toPos < 16)
			{
				while ((toPos < 16) && (this.waveDisplays[toPos].Loaded))
				{
					toPos++;
				}
				fromPos = toPos - 1;
				while ((toPos < 16) && (!this.waveDisplays[toPos].Loaded))
				{
					toPos++;
				}
				if ((toPos < 16) && (fromPos >= 0))
				{
					System.Diagnostics.Debug.WriteLineIf((fromPos < toPos), String.Format("Warning: from value ({0}) is less than to value ({1})", fromPos, toPos));
					ReCalculateMorphed(fromPos, toPos);
				}
			}
			
			for (int j = 0; j < 16; j++)
			{
				// Set the morphed data
				this.waveDisplays[j].MorphedData = this.morphedData[j];
				this.waveDisplays[j].Refresh();
			}
		}

		private void ReCalculateMorphed(int fromPos, int toPos)
		{
			float[] harmFrom = this.morphedData[fromPos];
			float[] harmTo = this.morphedData[toPos];
			
			var tempHarm = new float[128];
			
			int steps = toPos - fromPos - 1;
			float stepSize = 1.0F / (steps + 1.0F);
			float stepHelper = 0.0F;
			for (int i = 0; i < steps; i++)
			{
				for (int j = 0; j < 128; j++)
				{
					stepHelper = stepSize * (i + 1);
					tempHarm[j] = (harmFrom[j] * (1.0F - stepHelper) + harmTo[j] * stepHelper);
				}
				int sanityCheck = fromPos + i + 1;
				Array.Copy(tempHarm, 0, this.morphedData[sanityCheck], 0, 128);
			}
		}
		
		public void ClearAllCells()
		{
			// clear data
			for (int i = 0; i < 16; i++)
			{
				Array.Clear(this.soundData[i], 0, this.soundData[i].Length);
				Array.Clear(this.morphedData[i], 0, this.morphedData[i].Length);
				
				this.waveDisplays[i].Loaded = false;
				this.waveDisplays[i].FileName = "";
				this.waveDisplays[i].ClearWaveData();
				this.waveDisplays[i].ClearMorphedData();
				this.waveDisplays[i].Refresh();
			}
			
			// set sine data to first and last element
			Array.Copy(this.sineData, 0, this.morphedData[0], 0, 128);
			Array.Copy(this.sineData, 0, this.morphedData[15], 0, 128);
			
			this.waveDisplays[0].Loaded = true;
			this.waveDisplays[15].Loaded = true;
			this.waveDisplays[0].MorphedData = this.morphedData[0];
			this.waveDisplays[15].MorphedData = this.morphedData[15];
			this.waveDisplays[0].Refresh();
			this.waveDisplays[15].Refresh();
			
			this.exportFileName.Text = "";
		}
		
		#region Static File Utility Methods
		private static string RenameIfFileExists(string checkName)
		{
			var fileInfo = new FileInfo(checkName);
			DirectoryInfo folder = fileInfo.Directory;
			string folderName = folder.FullName;
			int version = 0;
			string fileName = fileInfo.FullName;
			
			while (fileInfo.Exists)
			{
				version++;
				fileName = RemoveFileSuffix(fileName);
				string tmp = "" + version;
				while (tmp.Length < 3)
				{
					tmp = "0" + tmp;
				}
				checkName = folderName + Path.DirectorySeparatorChar + fileName + tmp + ".h2p";
				fileInfo = new FileInfo(checkName);
			}
			
			return checkName;
		}
		
		private static string FixExportNames(string fullFilePath, bool isMorphed)
		{
			string temp = fullFilePath;
			temp = RemoveFileSuffix(temp);
			if (isMorphed)
			{
				temp = temp + "Morphed";
			}
			else
			{
				temp = temp + "";
			}
			temp = CheckH2pSuffix(temp);
			return temp;
		}
		
		private static string RemoveFileSuffix(string fullPath)
		{
			return Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
		}
		
		private static string RemoveWhiteSpace(string a)
		{
			return a.Replace(" ", "");
		}
		
		private static string CheckH2pSuffix(string a)
		{
			if (!a.EndsWith(".h2p", StringComparison.Ordinal)) {
				return a + ".h2p";
			} else {
				return a;
			}
		}
		#endregion
		
		#region Events
		void ExportToZebra2MenuItemClick(object sender, System.EventArgs e)
		{
			ExportToZebra2(DoExportMorphedWaves, DoExportRAWWaves);
		}
		
		void ShowMorphedRadioButtonCheckedChanged(object sender, System.EventArgs e)
		{
			for (int i = 0; i < 16; i++)
			{
				this.waveDisplays[i].Refresh();
			}
			
			if (DoShowRAWWaves)
			{
				this.outputField.Text = "Raw View";
			}
			else
			{
				this.outputField.Text = "Morphed View";
			}
		}
		
		void QuitToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		
		void SetExportPathToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			SetExportPath();
		}
		
		void ClearAllCellsToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			ClearAllCells();
		}
		
		void LoadCellToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			LoadCell();
		}
		
		void MainFormResize(object sender, System.EventArgs e)
		{
			if (this.waveDisplays != null && this.waveDisplays[0] != null) {
				int margins = this.waveDisplays[0].Margin.All;
				int newCellWidth = this.tableLayoutPanel.Width / 4 - (margins * 2);
				int newCellHeight = this.tableLayoutPanel.Height / 4 - (margins * 2);
				
				for (int i = 0; i < 16; i++)
				{
					this.waveDisplays[i].Size = new System.Drawing.Size(newCellWidth, newCellHeight);
					this.waveDisplays[i].ResumeLayout(false);
					this.waveDisplays[i].PerformLayout();
				}
			}
		}
		
		void HelpToolStripMenuItem1Click(object sender, EventArgs e)
		{
			new Help().Show();
		}
		#endregion
		
		#region Generate wave forms events
		void SetOscillator(float[] waveform) {
			int selected = this.SelectedWaveDisplay;
			if (selected > -1) {
				this.soundData[selected] = waveform;
				this.morphedData[selected] = this.soundData[selected];
				
				this.waveDisplays[selected].WaveData = this.soundData[selected];
				this.waveDisplays[selected].MorphedData = this.morphedData[selected];
				this.waveDisplays[selected].Refresh();
				this.waveDisplays[selected].Loaded = true;
				CalculateGhosts();
			}
		}
		
		void SineToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.Sine());
		}
		void SawRisingToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.SawRising());
		}
		void SawFallingToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.SawFalling());
		}
		void TriangleToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.Triangle());
		}
		void SquareHighLowToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.SquareHighLow());
		}
		void PulseHighLowToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.PulseHighLowI());
		}
		void PulseHighLowIIToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.PulseHighLowII());
		}
		void TriangleSawToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(OscillatorGenerator.TriangleSaw());
		}
		#endregion
		
	}
}

//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length.
//----------------------------------------------------------------------------------------
internal static partial class RectangularArrays
{
	internal static float[][] ReturnRectangularFloatArray(int Size1, int Size2)
	{
		var Array = new float[Size1][];
		for (int Array1 = 0; Array1 < Size1; Array1++)
		{
			Array[Array1] = new float[Size2];
		}
		return Array;
	}
}
