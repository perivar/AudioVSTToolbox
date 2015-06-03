using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

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
		public static string VERSION = "0.4";
		
		const string iniFileName = "Wav2Zebra2.ini";
		
		// supported files
		string[] EXTENSIONS = { ".h2p", ".wav", ".ogg", ".mp1", ".m1a", ".mp2", ".m2a", ".mpa", ".mus", ".mp3", ".mpg", ".mpeg", ".mp3pro", ".aif", ".aiff", ".bwf", ".wma", ".wmv", ".aac", ".adts", ".mp4", ".m4a", ".m4b", ".mod", ".mdz", ".mo3", ".s3m", ".s3z", ".xm", ".xmz", ".it", ".itz", ".umx", ".mtm", ".flac", ".fla", ".oga", ".ogg", ".aac", ".m4a", ".m4b", ".mp4", ".mpc", ".mp+", ".mpp", ".ac3", ".wma", ".ape", ".mac" };
		
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
			
			// set title
			this.Text = "Wav2Zebra2Osc Version " + VERSION;
			
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
			this.soundData = MathUtils.CreateJaggedArray<float[][]>(16, 128);
			this.morphedData = MathUtils.CreateJaggedArray<float[][]>(16, 128);
			
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
			this.OutputText = "Export path: " + pathName;
		}

		#region Read and Write Export Path
		string ReadExportPathName() {
			return IOUtils.ReadTextFromFile(iniFileName).Trim();
		}
		
		void WriteExportPathName(string pathName) {
			IOUtils.WriteTextToFile(iniFileName, pathName);
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
						break;
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
				
				beginning = IOUtils.GetFullPathWithoutExtension(beginning);
				end = IOUtils.GetFullPathWithoutExtension(end);
				beginning = RemoveWhiteSpace(beginning);
				end = RemoveWhiteSpace(end);
				
				this.rawExportName = (beginning + "To" + end);
				
				return (end != "Default") || (beginning != "Default");
			}
		}
		#endregion
		
		#region Export Method
		void ExportToZebra2(bool doExportMorphed, bool doExportRaw)
		{
			// keep on asking for file path until it is succesfully stored
			// in the ini file
			while (!File.Exists(iniFileName)) {
				this.OutputText = "Export path is not set";
				MakeUserSetExportPath();
				this.OutputText = "";
			}
			
			string pathName = ReadExportPathName();
			
			if (!HasExportFileNames) {
				this.OutputText = "There's nothing to export.";
			} else {
				string filePath = "";
				
				#region Morphed
				if (doExportMorphed) {
					filePath = pathName + Path.DirectorySeparatorChar + this.exportFileName.Text;
					filePath = FixExportNames(filePath, true);
					filePath = IOUtils.NextAvailableFilename(filePath);
					
					// set morph enabled slots
					var enabledSounds = new bool[16];
					for (int j = 0; j < 16; j++) {
						enabledSounds[j] = true;
					}
					Zebra2OSCPreset.Write(this.morphedData, enabledSounds, filePath);
				}
				#endregion
				
				#region RAW
				if (doExportRaw)
				{
					filePath = pathName + Path.DirectorySeparatorChar + this.exportFileName.Text;
					filePath = FixExportNames(filePath, false);
					filePath = IOUtils.NextAvailableFilename(filePath);
					
					// set wav enabled slots
					var enabledSounds = new bool[16];
					for (int j = 0; j < 16; j++) {
						if (!string.IsNullOrEmpty(this.waveDisplays[j].FileName)) {
							enabledSounds[j] = true;
						}
					}
					Zebra2OSCPreset.Write(this.soundData, enabledSounds, filePath);
				}
				#endregion

				this.outputField.Text = "File exported as: " + Path.GetFileName(filePath);
			}
		}
		#endregion
		
		void MakeUserSetExportPath()
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
		
		/// <summary>
		/// Load a set into the cell grid starting from startCell
		/// </summary>
		/// <param name="files">an array of files</param>
		/// <param name="startCell">cell index to start from</param>
		/// <returns>the last cell added</returns>
		int LoadFilesIntoCells(string[] files, int startCell) {
			int count = files.Length;
			int cellPosition = startCell;
			for (int i = 0; (i < count) && (i + startCell < 16); i++) {
				cellPosition = startCell + i;
				try {
					LoadFileIntoCell(new FileInfo(files[i]), cellPosition);
					this.waveDisplays[(cellPosition)].FileName = files[i];
				} catch (Exception e) {
					this.OutputText = "Not an audiofile";
					System.Diagnostics.Debug.WriteLine(e);
				}
			}
			return cellPosition;
		}
		
		/// <summary>
		/// Load one file into a specific cell
		/// </summary>
		/// <param name="file">file info</param>
		/// <param name="selectedCell">selected cell</param>
		void LoadFileIntoCell(FileInfo file, int selectedCell)
		{
			if (file.Extension.Equals(".h2p")) {
				var wavetable = Zebra2OSCPreset.Read(file.FullName);
				if (wavetable != null) {

					// set the data
					for (int i = 0; i < wavetable.Length; i++)
					{
						Array.Copy(wavetable[i], 0, this.soundData[i], 0, 128);

						// TODO: use loaded instead of filename to determine that
						// these are also loaded
						this.waveDisplays[i].FileName = file.FullName;
						
						// Interpolate using the raw waves
						this.morphedData[i] = this.soundData[i];
						
						this.waveDisplays[i].WaveData = this.soundData[i];
						this.waveDisplays[i].MorphedData = this.morphedData[i];
						this.waveDisplays[i].Loaded = true;
						this.waveDisplays[i].Refresh();
					}
				} else {
					throw new FileLoadException("Could not read the u-he Zebra 2 Oscillator preset file.", file.FullName);
				}
			} else {
				float[] tempAudioBuffer = BassProxy.ReadMonoFromFile(file.FullName);
				float[] tempAudioBuffer2 = MathUtils.Resample(tempAudioBuffer, 128);
				
				Array.Copy(tempAudioBuffer2, 0, this.soundData[selectedCell], 0, 128);

				// Interpolate using the raw waves
				this.morphedData[selectedCell] = this.soundData[selectedCell];
				
				this.waveDisplays[selectedCell].WaveData = this.soundData[selectedCell];
				this.waveDisplays[selectedCell].MorphedData = this.morphedData[selectedCell];
				this.waveDisplays[selectedCell].Loaded = true;
				this.waveDisplays[selectedCell].Refresh();
			}
		}
		
		/// <summary>
		/// Load the audio cell that is currently selected into the audio system so that it can be played
		/// </summary>
		public void LoadSelectedCellIntoAudioSystem() {
			int selectedCell = this.SelectedWaveDisplay;
			if (selectedCell > -1)
			{
				if (DoShowRAWWaves) {
					LoadOscIntoAudioSystem(this.waveDisplays[selectedCell].WaveData);
				} else if (DoShowMorphedWaves) {
					LoadOscIntoAudioSystem(this.waveDisplays[selectedCell].MorphedData);
				}
			}
		}
		
		/// <summary>
		/// Load generated oscillator data into the audio system so that it can be played
		/// </summary>
		/// <param name="data">oscillator sound data</param>
		void LoadOscIntoAudioSystem(float[] data) {
			
			// ensure this is playing at a sensible volume
			var data2 = MathUtils.ConvertRangeAndMainainRatio(data, -1.0f, 1.0f, -0.15f, 0.15f);
			
			// and load the sample into the audio system
			audioSystem.OpenWaveSample(data2);
		}

		public void MakeUserLoadCell()
		{
			var fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = true;
			fileDialog.Filter = "All Supported Files|*.h2p;*.wav;*.ogg;*.mp1;*.m1a;*.mp2;*.m2a;*.mpa;*.mus;*.mp3;*.mpg;*.mpeg;*.mp3pro;*.aif;*.aiff;*.bwf;*.wma;*.wmv;*.aac;*.adts;*.mp4;*.m4a;*.m4b;*.mod;*.mdz;*.mo3;*.s3m;*.s3z;*.xm;*.xmz;*.it;*.itz;*.umx;*.mtm;*.flac;*.fla;*.oga;*.ogg;*.aac;*.m4a;*.m4b;*.mp4;*.mpc;*.mp+;*.mpp;*.ac3;*.wma;*.ape;*.mac;*.m4a;*.tta;*.wv|WAVE Audio|*.wav|Ogg Vorbis|*.ogg|MPEG Layer 1|*.mp1;*.m1a|MPEG Layer 2|*.mp2;*.m2a;*.mpa;*.mus|MPEG Layer 3|*.mp3;*.mpg;*.mpeg;*.mp3pro|Audio IFF|*.aif;*.aiff|Broadcast Wave|*.bwf|Windows Media Audio|*.wma;*.wmv|Advanced Audio Codec|*.aac;*.adts|MPEG 4 Audio|*.mp4;*.m4a;*.m4b|MOD Music|*.mod;*.mdz|MO3 Music|*.mo3|S3M Music|*.s3m;*.s3z|XM Music|*.xm;*.xmz|IT Music|*.it;*.itz;*.umx|MTM Music|*.mtm|Free Lossless Audio Codec|*.flac;*.fla|Free Lossless Audio Codec (Ogg)|*.oga;*.ogg|Advanced Audio Coding|*.aac|Advanced Audio Coding MPEG-4|*.m4a;*.m4b;*.mp4|Musepack|*.mpc;*.mp+;*.mpp|Dolby Digital AC-3|*.ac3|Windows Media Audio|*.wma|Monkey's Audio|*.ape;*.mac|Apple Lossless Audio Codec|*.m4a|The True Audio|*.tta|WavPack|*.wv|u-he Zebra 2 Osc Preset|*.h2p";
			fileDialog.Title = "Select an audio file";

			this.OutputText = "";
			int selected = this.SelectedWaveDisplay;
			if (selected > -1)
			{
				DialogResult retval = fileDialog.ShowDialog();
				if (retval == DialogResult.OK)
				{
					string[] files = fileDialog.FileNames;
					LoadFilesIntoCells(files, selected);
				}
			} else {
				this.OutputText = "No slot selected. Click on one to select slot.";
			}
			
			// initialise the raw export name field
			bool unused = HasExportFileNames;
			this.exportFileName.Text = this.rawExportName;
			
			CalculateGhosts();
			LoadSelectedCellIntoAudioSystem();
		}

		public void SelectCell(int selectedCell) {
			
			// ensure all other cells are deselected
			for (int i = 0; i < 16; i++) {
				if (i == selectedCell) {
					this.waveDisplays[i].Selected = true;
					this.waveDisplays[i].Refresh();
				} else {
					this.waveDisplays[i].Selected = false;
					this.waveDisplays[i].Refresh();
				}
			}
		}
		
		void CalculateGhosts()
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
					MathUtils.Morph(ref this.morphedData, fromPos, toPos);
				}
			}
			
			for (int j = 0; j < 16; j++)
			{
				// Set the morphed data
				this.waveDisplays[j].MorphedData = this.morphedData[j];
				this.waveDisplays[j].Refresh();
			}
		}

		
		void ClearAllCells()
		{
			audioSystem.Pause();
			
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
		static string FixExportNames(string fullFilePath, bool isMorphed)
		{
			string tempPath = IOUtils.GetFullPathWithoutExtension(fullFilePath);
			if (isMorphed) {
				tempPath += "_Morph";
			}
			tempPath = IOUtils.EnsureExtension(tempPath, ".h2p");
			return tempPath;
		}
		
		static string RemoveWhiteSpace(string a)
		{
			return a.Replace(" ", "");
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
			
			LoadSelectedCellIntoAudioSystem();
		}
		
		void QuitToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		
		void SetExportPathToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			MakeUserSetExportPath();
		}
		
		void ClearAllCellsToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			ClearAllCells();
		}
		
		void LoadCellToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			MakeUserLoadCell();
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

		void BtnPlayClick(object sender, EventArgs e)
		{
			audioSystem.Play();
		}
		void BtnStopClick(object sender, EventArgs e)
		{
			audioSystem.Pause();
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
			LoadSelectedCellIntoAudioSystem();
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

		#region other menu item events
		void ClearToolStripMenuItemClick(object sender, EventArgs e)
		{
			SetOscillator(new float[128]);
		}

		void LargeWaveformToolStripMenuItemClick(object sender, EventArgs e)
		{
			int selectedCell = this.SelectedWaveDisplay;
			if (selectedCell > -1)
			{
				float[] tempSoundData = null;
				if (DoShowRAWWaves) {
					tempSoundData = this.waveDisplays[selectedCell].WaveData;
				} else if (DoShowMorphedWaves) {
					tempSoundData = this.waveDisplays[selectedCell].MorphedData;
				}
				new WaveFormView(tempSoundData).ShowDialog();
			}
		}
		#endregion
		
		#region Move Methods
		public void MoveUp() {
			int selected = this.SelectedWaveDisplay;
			if (selected > 3 && selected < 16)
			{
				int newPosition = selected - 4;
				this.waveDisplays[selected].Selected = false;
				this.waveDisplays[selected].Refresh();

				this.waveDisplays[newPosition].Selected = true;
				this.waveDisplays[newPosition].Refresh();
				LoadSelectedCellIntoAudioSystem();
			}
		}

		public void MoveDown() {
			int selected = this.SelectedWaveDisplay;
			if (selected > -1 && selected < 12)
			{
				int newPosition = selected + 4;
				this.waveDisplays[selected].Selected = false;
				this.waveDisplays[selected].Refresh();

				this.waveDisplays[newPosition].Selected = true;
				this.waveDisplays[newPosition].Refresh();
				LoadSelectedCellIntoAudioSystem();
			}
		}

		public void MoveRight() {
			int selected = this.SelectedWaveDisplay;
			if (selected > -1 && selected < 15)
			{
				int newPosition = selected + 1;
				this.waveDisplays[selected].Selected = false;
				this.waveDisplays[selected].Refresh();

				this.waveDisplays[newPosition].Selected = true;
				this.waveDisplays[newPosition].Refresh();
				LoadSelectedCellIntoAudioSystem();
			}
		}

		public void MoveLeft() {
			int selected = this.SelectedWaveDisplay;
			if (selected > 0 && selected < 16)
			{
				int newPosition = selected - 1;
				this.waveDisplays[selected].Selected = false;
				this.waveDisplays[selected].Refresh();

				this.waveDisplays[newPosition].Selected = true;
				this.waveDisplays[newPosition].Refresh();
				LoadSelectedCellIntoAudioSystem();
			}
		}
		#endregion
		
		void ConvertMassiveOscsToolStripMenuItemClick(object sender, EventArgs e)
		{
			
		}
		
		#region Drag and Drop
		void TableLayoutPanelDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			}
		}
		
		void TableLayoutPanelDragDrop(object sender, DragEventArgs e)
		{
			// Supported files
			string[] extensions = { ".wav", ".ogg", ".mp1", ".m1a", ".mp2", ".m2a", ".mpa", ".mus", ".mp3", ".mpg", ".mpeg", ".mp3pro", ".aif", ".aiff", ".bwf", ".wma", ".wmv", ".aac", ".adts", ".mp4", ".m4a", ".m4b", ".mod", ".mdz", ".mo3", ".s3m", ".s3z", ".xm", ".xmz", ".it", ".itz", ".umx", ".mtm", ".flac", ".fla", ".oga", ".ogg", ".aac", ".m4a", ".m4b", ".mp4", ".mpc", ".mp+", ".mpp", ".ac3", ".wma", ".ape", ".mac" };
			
			// find out what table layout cell we are dragging over
			var cellPos = GetRowColIndex(
				tableLayoutPanel,
				tableLayoutPanel.PointToClient(Cursor.Position));
			
			int startCell = PointToIndex(cellPos);
			
			if (startCell > -1) {
				if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
					var files = (string[])e.Data.GetData(DataFormats.FileDrop);
					
					// remove unsupported files
					var filesFixed = IOUtils.FilterOutUnsupportedFiles(files, EXTENSIONS);
					
					if (filesFixed.Length > 0) {
						// and load
						int lastCell = LoadFilesIntoCells(filesFixed, startCell);
						SelectCell(lastCell);
						
						// initialise the raw export name field
						bool unused = HasExportFileNames;
						this.exportFileName.Text = this.rawExportName;
						
						CalculateGhosts();
						LoadSelectedCellIntoAudioSystem();
					} else {
						this.OutputText = "No valid files selected.";
					}
				}
			}
		}
		
		public int PointToIndex(Point? point) {
			if (point != null) {
				int index = point.Value.X + 4 * point.Value.Y;
				return index;
			} else {
				return -1;
			}
		}
		
		Point? GetRowColIndex(TableLayoutPanel tlp, Point point)
		{
			if (point.X > tlp.Width || point.Y > tlp.Height)
				return null;

			int w = tlp.Width;
			int h = tlp.Height;
			int[] widths = tlp.GetColumnWidths();

			int i;
			for (i = widths.Length - 1; i >= 0 && point.X < w; i--)
				w -= widths[i];
			int col = i + 1;

			int[] heights = tlp.GetRowHeights();
			for (i = heights.Length - 1; i >= 0 && point.Y < h; i--)
				h -= heights[i];

			int row = i + 1;

			return new Point(col, row);
		}
		
		#endregion
		
	}
}
