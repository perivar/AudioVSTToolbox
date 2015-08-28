using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace NAudio_Visualizing
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			NAudioEngine soundEngine = NAudioEngine.Instance;
			soundEngine.PropertyChanged += NAudioEngine_PropertyChanged;
			
			customWaveViewer1.RegisterSoundPlayer(soundEngine);
			customSpectrumAnalyzer1.RegisterSoundPlayer(soundEngine);
		}
		
		#region NAudio Engine Events
		private void NAudioEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NAudioEngine engine = NAudioEngine.Instance;
			switch (e.PropertyName)
			{
				case "FileTag":
					if (engine.FileTag != null)
					{
						TagLib.Tag tag = engine.FileTag.Tag;
					}
					else
					{
						//albumArtPanel.AlbumArtImage = null;
					}
					break;
				case "ChannelPosition":
					txtTime.Text = TimeSpan.FromSeconds(engine.ChannelPosition).ToString();
					break;
				default:
					// Do Nothing
					break;
			}

		}
		#endregion
		
		void BtnBrowseClick(object sender, EventArgs e)
		{
			//openFileDialog.Filter = "(*.mp3)|*.mp3|(*.wav)|*.wav";
			openFileDialog.Filter = "Audio Files(*.wav;*.mp3)|*.wav;*.mp3|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				NAudioEngine.Instance.OpenFile(openFileDialog.FileName);
				txtFilePath.Text = openFileDialog.FileName;
			}
		}
		
		void BtnPlayClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanPlay)
				NAudioEngine.Instance.Play();
		}
		
		void BtnPauseClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanPause)
				NAudioEngine.Instance.Pause();
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			if (NAudioEngine.Instance.CanStop)
				NAudioEngine.Instance.Stop();
			
			NAudioEngine.Instance.ChannelPosition = 0;
			NAudioEngine.Instance.SelectionBegin = TimeSpan.FromMilliseconds(0);
			NAudioEngine.Instance.SelectionEnd = TimeSpan.FromMilliseconds(0);
		}
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			NAudioEngine.Instance.Dispose();
			base.Dispose();
		}
		
		void MainFormKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space) {
				if (NAudioEngine.Instance.IsPlaying) {
					BtnPauseClick(null, EventArgs.Empty);
				} else {
					BtnPlayClick(null, EventArgs.Empty);
				}
			}
		}
		
		void CustomSpectrumAnalyzer1Click(object sender, EventArgs e)
		{
			customSpectrumAnalyzer1.DoSpectrumGraph = !customSpectrumAnalyzer1.DoSpectrumGraph;
		}
		
	}
}
