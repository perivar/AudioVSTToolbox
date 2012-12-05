using System;
using System.IO;

using NAudio;
using NAudio.Wave;

using BigMansStuff.NAudio.FLAC;
using BigMansStuff.NAudio.Ogg;
using NAudio.WindowsMediaFormat;

namespace CommonUtils.Audio.NAudio
{
	/// <summary>
	/// Extra Audio Methods that make use of NAudio
	/// Support for OGG, WMA and Flac
	/// </summary>
	public static class AudioUtilsNAudioExtra
	{
		const string MP3Extension = ".mp3";
		const string WAVExtension = ".wav";
		const string OGGVExtension = ".ogg";
		const string FLACExtension = ".flac";
		const string WMAExtension = ".wma";
		const string AIFFExtension = ".aiff";
		
		/// <summary>
		/// Creates an input WaveChannel
		/// (Audio file reader for MP3/WAV/OGG/FLAC/WMA/AIFF/Other formats in the future)
		/// </summary>
		/// <param name="filename"></param>
		public static WaveStream CreateInputWaveChannel(string filename)
		{
			WaveStream m_blockAlignedStream = null;
			WaveStream m_waveReader = null;
			WaveChannel32 m_waveChannel = null;

			string fileExt = Path.GetExtension( filename.ToLower() );
			if ( fileExt == MP3Extension ) {
				m_waveReader = new Mp3FileReader(filename);
				m_blockAlignedStream = new BlockAlignReductionStream(m_waveReader);
				// Wave channel - reads from file and returns raw wave blocks
				m_waveChannel = new WaveChannel32(m_blockAlignedStream);
			} else if ( fileExt == WAVExtension ) {
				m_waveReader = new WaveFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm) {
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				} if (m_waveReader.WaveFormat.BitsPerSample != 16) {
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}
				
				m_waveChannel = new WaveChannel32(m_waveReader);
			} else if (fileExt == OGGVExtension) {
				m_waveReader = new OggFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm) {
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				} if (m_waveReader.WaveFormat.BitsPerSample != 16) {
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			} else if (fileExt == FLACExtension) {
				m_waveReader = new FLACFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm) {
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				} if (m_waveReader.WaveFormat.BitsPerSample != 16) {
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else if (fileExt == WMAExtension) {
				m_waveReader = new WMAFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm) {
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				} if (m_waveReader.WaveFormat.BitsPerSample != 16) {
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			} else if (fileExt == AIFFExtension) {
				m_waveReader = new AiffFileReader(filename);
				m_waveChannel = new WaveChannel32(m_waveReader);
			} else {
				throw new ApplicationException("Cannot create Input WaveChannel - Unknown file type: " + fileExt);
			}
			//return m_waveReader;
			return m_waveChannel;
		}
	}
}
