using System;
using System.Windows.Forms;

namespace Wav2Zebra2Osc
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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());

			/*
			string filename = @"..\..\amen.wav";
			AudioSystem.BassProxy bassProxy = new AudioSystem.BassProxy();
			float[] tempAudioBuffer = bassProxy.ReadMonoFromFile(filename, 44100);
			float[] tempAudioBuffer2 = Conversions.ReSampleToArbitrary(tempAudioBuffer, 512);

			//CommonUtils.FFT.FFTTesting.TimeAll(10000, false);

			double[] audio_data = CommonUtils.MathUtils.FloatToDouble(tempAudioBuffer2);
			CommonUtils.FFT.FFTTesting.TestAll(audio_data);
			 */
		}
	}
}