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

		float[][] harmonicsData;
		float[][] soundData;
		float[][] dftData;
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
			waveDisplays = new Wav2Zebra2Osc.WaveDisplayUserControl[16];
			int counter = 0;
			for( int row = 0; row != 4; ++row ) {
				for( int col = 0; col != 4; ++col ) {
					this.waveDisplays[counter] = new Wav2Zebra2Osc.WaveDisplayUserControl(this);
					this.tableLayoutPanel.Controls.Add(waveDisplays[counter], col, row);
					this.waveDisplays[counter].ResumeLayout(false);
					this.waveDisplays[counter].PerformLayout();
					counter++;
				}
			}
			this.waveDisplays[0].Selected = true;

			// Initalize the jagged data arrays
			this.harmonicsData = RectangularArrays.ReturnRectangularFloatArray(16, 128);
			this.soundData = RectangularArrays.ReturnRectangularFloatArray(16, 128);
			this.dftData = RectangularArrays.ReturnRectangularFloatArray(16, 128);
			
			// generate the sine data
			this.sineData = new float[128];
			for (int j = 0; j < 128; j++)
			{
				this.sineData[j] = (float) Math.Sin(j * Conversions.TWO_PI / 128.0F);
			}
			
			// set sine data to first and last element
			Array.Copy(this.sineData, 0, this.dftData[0], 0, 128);
			Array.Copy(this.sineData, 0, this.dftData[15], 0, 128);
			
			this.harmonicsData[0][0] = 1.0F;
			this.harmonicsData[15][0] = 1.0F;
			this.waveDisplays[0].Loaded = true;
			this.waveDisplays[15].Loaded = true;
			this.waveDisplays[0].DftData = this.dftData[0];
			this.waveDisplays[15].DftData = this.dftData[15];
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
		public virtual bool DoShowRAWWaves
		{
			get
			{
				return this.showRAWRadioButton.Checked;
			}
		}

		public virtual bool DoShowDFTWaves
		{
			get
			{
				return this.showDFTRadioButton.Checked;
			}
		}

		public virtual bool DoExportDFTWaves
		{
			get
			{
				return this.exportDFTWavesCheckBox.Checked;
			}
		}

		public virtual bool DoExportRAWWaves
		{
			get
			{
				return this.exportRAWWavesCheckBox.Checked;
			}
		}
		#endregion

		#region Properties
		public virtual string OutputText
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

		public virtual int SelectedWaveDisplay
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
		
		public virtual void SetImportSound(FileInfo file, int selected)
		{
			float[] tempAudioBuffer = BassProxy.ReadMonoFromFile(file.FullName);
			float[] tempAudioBuffer2 = MathUtils.Resample(tempAudioBuffer, 128);
			
			#if DEBUG
			Export.ExportCSV(file.Name + ".csv", tempAudioBuffer2);
			#endif
			
			Array.Copy(tempAudioBuffer2, 0, this.soundData[selected], 0, 128);
			
			// TODO: use DFT ?
			//this.harmonicsData[selected] = FFTUtils.AbsFFT(this.soundData[selected]);
			//this.dftData[selected] = FFTUtils.IFFT(this.harmonicsData[selected], false, false);
			//this.harmonicsData[selected] = Conversions.DFT(this.soundData[selected]);
			//this.dftData[selected] = Conversions.iDFT(this.harmonicsData[selected]);

			// ignore using DFT/iDFT and interpolate using the raw waves instead
			this.harmonicsData[selected] = this.soundData[selected];
			this.dftData[selected] = this.harmonicsData[selected];
			
			/*
			double[] doubleArray = MathUtils.FloatToDouble(this.soundData[selected]);
			double[] fftArray = FFTUtils.FFT(doubleArray);
			double scaling = 1.0 / 64;
			double[] absArray = FFTUtils.Abs(fftArray, scaling);
			double[] realArray = FFTUtils.Real(fftArray);
			this.harmonicsData[selected] = MathUtils.DoubleToFloat(absArray);
			
			double[] ifftArray = FFTUtils.IFFT(fftArray, true, false);
			float[] floatArray = MathUtils.DoubleToFloat(ifftArray);
			this.dftData[selected] = floatArray;
			 */
			
			this.waveDisplays[selected].HarmonicsData = this.harmonicsData[selected];
			this.waveDisplays[selected].WaveData = this.soundData[selected];
			this.waveDisplays[selected].DftData = this.dftData[selected];
			this.waveDisplays[selected].Refresh();
		}
		
		#region Export Method
		public virtual void ExportToZebra2(bool dft, bool raw)
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
				
				#region DFT
				if (dft) {
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
								string sampleValue = "" + this.dftData[j][i];
								if ((this.dftData[j][i] < 0.001F) && (this.dftData[j][i] > -0.001F))
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
				if (raw)
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
		
		public virtual void SetExportPath()
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
		
		public virtual void LoadCell()
		{
			var fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = true;
			
			//fileDialog.Filter = "MP3|*.mp3|WAV|*.wav|All files (*.*)|*.*";
			//fileDialog.InitialDirectory = initialDirectory;
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
					ReCalculateHarmonics(fromPos, toPos);
				}
			}
			
			for (int j = 0; j < 16; j++)
			{
				// TODO: use DFT ?
				//this.dftData[j] = Conversions.iDFT(this.harmonicsData[j]);
				//this.dftData[j] = FFTUtils.IFFT(this.harmonicsData[j], false, false);

				// ignore using DFT/iDFT and interpolate using the raw waves instead
				this.dftData[j] = this.harmonicsData[j];
				
				this.waveDisplays[j].DftData = this.dftData[j];
				this.waveDisplays[j].HarmonicsData = this.harmonicsData[j];
				this.waveDisplays[j].Refresh();
			}
		}
		
		public virtual void ReCalculateHarmonics(int fromPos, int toPos)
		{
			float[] harmFrom = this.harmonicsData[fromPos];
			float[] harmTo = this.harmonicsData[toPos];
			
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
				Array.Copy(tempHarm, 0, this.harmonicsData[sanityCheck], 0, 128);
			}
		}
		
		public virtual void ClearAllCells()
		{
			// clear data
			for (int i = 0; i < 16; i++)
			{
				Array.Clear(this.harmonicsData[i], 0, this.harmonicsData[i].Length);
				Array.Clear(this.soundData[i], 0, this.soundData[i].Length);
				Array.Clear(this.dftData[i], 0, this.dftData[i].Length);
				
				this.waveDisplays[i].Loaded = false;
				this.waveDisplays[i].FileName = "";
				this.waveDisplays[i].ClearHarmonics();
				this.waveDisplays[i].ClearWaveData();
				this.waveDisplays[i].ClearDftData();
				this.waveDisplays[i].Refresh();
			}
			
			// set sine data to first and last element
			Array.Copy(this.sineData, 0, this.dftData[0], 0, 128);
			Array.Copy(this.sineData, 0, this.dftData[15], 0, 128);
			
			this.harmonicsData[0][0] = 1.0F;
			this.harmonicsData[15][0] = 1.0F;
			this.waveDisplays[0].Loaded = true;
			this.waveDisplays[15].Loaded = true;
			this.waveDisplays[0].DftData = this.dftData[0];
			this.waveDisplays[15].DftData = this.dftData[15];
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
		
		private static string FixExportNames(string fullFilePath, bool dft)
		{
			string temp = fullFilePath;
			temp = RemoveFileSuffix(temp);
			if (dft)
			{
				temp = temp + "Dft";
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
		
		void HelpToolStripMenuItem1Click(object sender, EventArgs e)
		{
			new Help().Show();
		}
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
