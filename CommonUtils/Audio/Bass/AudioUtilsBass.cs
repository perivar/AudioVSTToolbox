using System;
using CommonUtils.Audio;

namespace CommonUtils.Audio.Bass
{
	/// <summary>
	/// Audio Methods that make use of Bass
	/// </summary>
	/// <remarks>
	///   BASS is an audio library for use in Windows and Mac OSX software.
	///   Its purpose is to provide developers with powerful and efficient sample, stream (MP3, MP2, MP1, OGG, WAV, AIFF, custom generated, and more via add-ons),
	///   MOD music (XM, IT, S3M, MOD, MTM, UMX), MO3 music (MP3/OGG compressed MODs), and recording functions.
	///   All in a tiny DLL, under 100KB* in size.
	/// </remarks>
	public static class AudioUtilsBass
	{
		/// <summary>
		///   Read mono from file
		/// </summary>
		/// <param name = "filename">Name of the file</param>
		/// <param name = "samplerate">Sample rate</param>
		/// <param name = "milliseconds">milliseconds to read</param>
		/// <param name = "startmillisecond">Start millisecond</param>
		/// <returns>Array of samples</returns>
		public static float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond)
		{
			BassProxy proxy = new BassProxy(); /*audio proxy used in reading the file*/
			return proxy.ReadMonoFromFile(filename, samplerate, milliseconds, startmillisecond);
		}
		
	}
}
