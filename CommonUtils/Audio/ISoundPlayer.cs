using System.ComponentModel;

namespace CommonUtils.Audio
{
	/// <summary>
	/// Provides access to functionality that is common
	/// across all sound players.
	/// </summary>
	/// <seealso cref="IWaveformPlayer"/>
	/// <seealso cref="ISpectrumPlayer"/>
	/// The original interface idea copyright (C) 2011 - 2012, Jacob Johnston
	public interface ISoundPlayer : INotifyPropertyChanged
	{
		/// <summary>
		/// Return the sample rate for the current audio
		/// </summary>
		int SampleRate { get; }

		// Public Properties
		bool CanPlay { get; }
		bool CanPause { get; }
		bool CanStop { get; }
		bool IsPlaying { get; }
		
		/// <summary>
		/// Start playing audio
		/// </summary>
		void Play();
		
		/// <summary>
		/// Pause playin audio
		/// </summary>
		void Pause();
		
		/// <summary>
		/// Stop playing audio
		/// </summary>
		void Stop();
	}
}
