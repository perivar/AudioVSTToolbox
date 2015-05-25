using System;
namespace CommonUtils.Audio
{
	/// <summary>
	/// Provides access to sound player functionality needed to
	/// generate a Waveform.
	/// The original interface idea copyright (C) 2011 - 2012, Jacob Johnston
	/// Heavily modified by perivar@nerseth.com
	/// </summary>
	public interface IWaveformPlayer : ISoundPlayer, IDisposable
	{
		/// <summary>
		/// The file name filter string which can be used in a standard 'OpenFileDialog' in the format:
		/// allFormatName|AllExtensions|FormatName1|externtions1|FormatName2|externtions2...
		/// </summary>
		string FileFilter { get; }
		
		/// <summary>
		/// Return the file path, can be null
		/// </summary>
		string FilePath { get; }
		
		/// <summary>
		/// Gets or sets the current sound streams playback position.
		/// </summary>
		int ChannelSamplePosition { get; set; }

		/// <summary>
		/// Gets the number of Channels (2 = stereo)
		/// </summary>
		int Channels { get; }
		
		/// <summary>
		/// Return the length in seconds per channel
		/// </summary>
		double ChannelLength { get; }
		
		/// <summary>
		/// Return the sample length per Channel (i.e. if the waveform is stereo, this is half the total sample length)
		/// </summary>
		int ChannelSampleLength { get; }
		
		/// <summary>
		/// Return number of bits per sample (e.g. 16, 32, etc.)
		/// </summary>
		int BitsPerSample { get; }

		/// <summary>
		/// Return the total sample length (i.e. if the waveform is stereo, this is double the channel sample length)
		/// </summary>
		int TotalSampleLength { get; }
		
		/// <summary>
		/// Gets the raw interleaved data for the waveform.
		/// </summary>
		/// <remarks>
		/// Samples in a multi-channel PCM wave file are interleaved.
		/// That is, in a stereo file, one sample for the left channel will be followed by
		/// one sample for the right channel,
		/// followed by another sample for the left channel, then right channel, and so forth.
		/// Index 0 should be the first left level, index 1 should be the first right level,
		/// index 2 should be the second left level, etc.
		/// </remarks>
		float[] WaveformData { get; }

		/// <summary>
		/// Gets or sets the starting sample time for a section of repeat/looped audio.
		/// </summary>
		int SelectionSampleBegin { get; set; }

		/// <summary>
		/// Gets or sets the ending sample time for a section of repeat/looped audio.
		/// </summary>
		int SelectionSampleEnd { get; set; }
		
		/// <summary>
		/// Open File using passed file path
		/// </summary>
		/// <param name="path">path to audio file</param>
		/// <param name="doPlayFromMemory">Load into memory instead of using filestream</param>
		void OpenFile(string path, bool doPlayFromMemory = false);
		
		/// <summary>
		/// Save File using passed path
		/// </summary>
		/// <param name="path">path to audio file</param>
		void SaveFile(string path);
	}
}
