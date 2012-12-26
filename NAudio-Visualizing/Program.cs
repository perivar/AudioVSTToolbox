using System;
using System.Windows.Forms;

namespace NAudio_Visualizing
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			//CommonUtils.Audio.NAudio.AudioUtilsNAudio.GenerateAudioTestFile(44100, 44, @"c:\audio-test.wav");
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
	}
}
