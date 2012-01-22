using System;
using System.IO;

using NAudio;
using NAudio.Wave;

using BigMansStuff.NAudio.FLAC;
using BigMansStuff.NAudio.Ogg;
using NAudio.WindowsMediaFormat;

using System.Collections.Generic;

namespace CommonUtils.Audio
{
	/// <summary>
	/// Description of StringUtils.
	/// </summary>
	public static class AudioUtils
	{
		const string MP3Extension = ".mp3";
		const string WAVExtension = ".wav";
		const string OGGVExtension = ".ogg";
		const string FLACExtension = ".flac";
		const string WMAExtension = ".wma";
		const string AIFFExtension = ".aiff";

		public enum LFOTIMING {
			LFO_8_1D = 0,
			LFO_8_1 = 1,
			LFO_4_1D = 2,
			LFO_8_1T = 3,
			LFO_4_1 = 4,
			LFO_2_1D = 5,
			LFO_4_1T = 6,
			LFO_2_1 = 7,
			LFO_1_1D = 8,
			LFO_2_1T = 9,
			LFO_1_1 = 10,
			LFO_1_2D = 11,
			LFO_1_1T = 12,
			LFO_1_2 = 13,
			LFO_1_4D = 14,
			LFO_1_2T = 15,
			LFO_1_4 = 16,
			LFO_1_8D = 17,
			LFO_1_4T = 18,
			LFO_1_8 = 19,
			LFO_1_16D = 20,
			LFO_1_8T = 21,
			LFO_1_16 = 22,
			LFO_1_32D = 23,
			LFO_1_16T = 24,
			LFO_1_32 = 25,
			LFO_1_64D = 26,
			LFO_1_32T = 27,
			LFO_1_64 = 28,
			LFO_1_128D = 29,
			LFO_1_64T = 30,
			LFO_1_128 = 31,
			LFO_1_256D = 32,
			LFO_1_128T = 33,
			LFO_1_256 = 34,
			LFO_1_256T = 35
		}
		
		public static TimeSpan GetWaveFileTotalTime(string filePath) {
			var stream=new MemoryStream(File.ReadAllBytes(filePath));
			var wave = new WaveFileReader(stream);
			return wave.TotalTime;
		}

		// Read A complete stream into a byte array
		public static byte[] ReadFully(Stream input)
		{
			/*
			 * Stream.Read doesn't guarantee that it will read everything it's asked for.
			 * If you're reading from a network stream, for example, it may read one packet's worth and then return,
			 * even if there will be more data soon. BinaryReader.
			 * Read will keep going until the end of the stream or your specified size,
			 * but you still have to know the size to start with.
			 * The above method will keep reading (and copying into a MemoryStream) until it runs out of data.
			 * It then asks the MemoryStream to return a copy of the data in an array.
			 * If you know the size to start with - or think you know the size, without being sure
			 * - you can construct the MemoryStream to be that size to start with.
			 * Likewise you can put a check at the end, and if the length of the stream is the same size as the buffer
			 * (returned by MemoryStream.GetBuffer) then you can just return the buffer.
			 * So the code isn't quite optimised, but will at least be correct.
			 * See this article (http://www.yoda.arachsys.com/csharp/readbinary.html) for more info (and an alternative implementation).
			 * 
			 * It doesn't assume any responsibility for closing the stream - the caller should do that.
			 */
			byte[] buffer = new byte[16*1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		#region CreateWaveFile methods
		public static void CreateWaveFile(Stream stream, string fileName, WaveFormat waveFormat) {
			using (WaveFileWriter writer = new WaveFileWriter(fileName, waveFormat))
			{
				byte[] buffer = new byte[16*1024];
				for (; ;)
				{
					int bytes = stream.Read(buffer, 0, buffer.Length);
					if (bytes == 0) break;
					writer.Write(buffer, 0, bytes);
				}
			}
		}

		public static void CreateWaveFile(float[] wavData, string fileName, WaveFormat waveFormat) {
			using (WaveFileWriter writer = new WaveFileWriter(fileName, waveFormat))
			{
				writer.WriteSamples(wavData, 0, wavData.Length);
			}
		}
		#endregion
		
		#region ResampleWav methods
		public static void ResampleWav(string wavInFilePath, string wavOutFilePath, WaveFormat waveFormat) {
			using (var reader = new WaveFileReader(wavInFilePath))
			{
				using (var conversionStream = new WaveFormatConversionStream(waveFormat, reader))
				{
					WaveFileWriter.CreateWaveFile(wavOutFilePath, conversionStream);
				}
			}
		}
		
		public static byte[] ResampleWav(Stream wavIn, WaveFormat waveFormat) {
			using (var reader = new WaveFileReader(wavIn))
			{
				using (var conversionStream = new WaveFormatConversionStream(waveFormat, reader))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						int bytes = 0;
						byte[] buffer = new byte[16*1024];
						while ((bytes = conversionStream.Read(buffer, 0, buffer.Length)) != 0) {
							ms.Write(buffer, 0, bytes);
						}
						return ms.ToArray();
					}
				}
			}
		}

		public static WaveStream ResampleWaveStream(string wavInFilePath, WaveFormat waveFormat) {
			//WaveStream sourceStream = new WaveFileReader(wavInFilePath);
			WaveStream sourceStream = CreateInputWaveChannel(wavInFilePath);
			
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				sourceStream = WaveFormatConversionStream.CreatePcmStream(sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			if (sourceStream.WaveFormat.SampleRate != waveFormat.SampleRate ||
			    sourceStream.WaveFormat.BitsPerSample != waveFormat.BitsPerSample)
			{
				sourceStream = new WaveFormatConversionStream(waveFormat, sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			
			//sourceStream = new WaveChannel32(sourceStream);
			return sourceStream;
		}

		public static byte[] ResampleWav(string wavInFilePath, WaveFormat waveFormat) {
			using (var reader = ResampleWaveStream(wavInFilePath, waveFormat))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					int bytes = 0;
					byte[] buffer = new byte[16*1024];
					while ((bytes = reader.Read(buffer, 0, buffer.Length)) != 0) {
						ms.Write(buffer, 0, bytes);
					}
					return ms.ToArray();
				}
			}
		}
		#endregion
		
		// Read Stereo
		public static bool TryReadFloat(WaveStream waveStream, out float sampleValueLeft, out float sampleValueRight)
		{
			// 16 bit PCM data
			sampleValueLeft = 0f;
			sampleValueRight = 0f;
			if (waveStream.WaveFormat.BitsPerSample == 16)
			{
				byte[] buffer = new byte[4];
				int read = waveStream.Read(buffer, 0, buffer.Length);
				if (read < buffer.Length)
					return false;

				sampleValueLeft = (float)BitConverter.ToInt16(buffer, 0) / 32768f;
				sampleValueRight = (float)BitConverter.ToInt16(buffer, 2) / 32768f;
				return true;
			}
			return false;
		}
		
		// Read Mono
		public static bool TryReadFloat(WaveStream waveStream, out float sampleValue)
		{
			// 16 bit PCM data
			sampleValue = 0f;
			if (waveStream.WaveFormat.BitsPerSample == 16)
			{
				byte[] buffer = new byte[2];
				int read = waveStream.Read(buffer, 0, buffer.Length);
				if (read < buffer.Length)
					return false;

				sampleValue = (float)BitConverter.ToInt16(buffer, 0) / 32768f;
				return true;
			}
			return false;
		}
		
		/// <summary>
		///   Read mono from file
		/// </summary>
		/// <param name = "filename">Name of the file</param>
		/// <param name = "samplerate">Sample rate</param>
		/// <param name = "milliseconds">milliseconds to read</param>
		/// <param name = "startmillisecond">Start millisecond</param>
		/// <returns>Array of samples</returns>
		public static float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond, int channelToUse=1)
		{
			int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
			float[] data = null;

			// read as stereo file - do the mono thing later
			List<float> floatList = new List<float>();
			WaveFormat waveFormat = new WaveFormat(samplerate, 2);
			WaveStream wave32 = ResampleWaveStream(filename, waveFormat);
			
			int sampleCount = 0;

			// check the input file number of channels
			if (wave32.WaveFormat.Channels == 1) {
				// we already have a mono file
				float sampleValue = 0;
				
				// read until we have read the number of samples (measured in ms) we are supposed to do
				while (AudioUtils.TryReadFloat(wave32, out sampleValue) == true
				       && (float)(sampleCount)/ samplerate * 1000 < totalmilliseconds)
				{
					floatList.Add(sampleValue);
					
					// increment with size of data
					sampleCount++;
				}
				data = floatList.ToArray();
				
			} else if (wave32.WaveFormat.Channels == 2) {
				// we are getting a stereo file back
				// convert to mono by taking an average of both values:
				// f_mono = function(l, r) {
				//   return (l + r) / 2;
				//}
				float sampleValueLeft = 0;
				float sampleValueRight = 0;
				float sampleValueMono = 0;
				
				// read until we have read the number of samples (measured in ms) we are supposed to do
				while (AudioUtils.TryReadFloat(wave32, out sampleValueLeft, out sampleValueRight) == true
				       && (float)(sampleCount)/ samplerate * 1000 < totalmilliseconds)
				{

					// TODO: Use the summed mono signal to anything?
					sampleValueMono = (sampleValueLeft + sampleValueRight) / 2;

					// return channel 1 as default
					if (channelToUse == 2) {
						floatList.Add(sampleValueRight);
					} else {
						floatList.Add(sampleValueLeft);
					}

					// increment with size of data
					sampleCount++;
				}
				data = floatList.ToArray();
			}
			
			if ( (float)(sampleCount) / samplerate * 1000 < (milliseconds + startmillisecond)) {
				// not enough samples to return the requested data
				return null;
			}
			
			// Select specific part of the song
			int start = (int) ( (float) startmillisecond * samplerate / 1000);
			int end = (milliseconds <= 0) ?
				sampleCount :
				(int) ( (float)(startmillisecond + milliseconds) * samplerate / 1000);
			if (start != 0 || end != sampleCount)
			{
				float[] temp = new float[end - start];
				Array.Copy(data, start, temp, 0, end - start);
				data = temp;
			}
			
			return data;
		}
		
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
			if ( fileExt == MP3Extension )
			{
				m_waveReader = new Mp3FileReader(filename);
				m_blockAlignedStream = new BlockAlignReductionStream(m_waveReader);
				// Wave channel - reads from file and returns raw wave blocks
				m_waveChannel = new WaveChannel32(m_blockAlignedStream);
			}
			else if ( fileExt == WAVExtension )
			{
				m_waveReader = new WaveFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				}
				if (m_waveReader.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}
				
				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else if (fileExt == OGGVExtension)
			{
				m_waveReader = new OggFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				}
				if (m_waveReader.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else if (fileExt == FLACExtension)
			{
				m_waveReader = new FLACFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				}
				if (m_waveReader.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else if (fileExt == WMAExtension)
			{
				m_waveReader = new WMAFileReader(filename);
				if (m_waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
				{
					m_waveReader = WaveFormatConversionStream.CreatePcmStream(m_waveReader);
					m_waveReader = new BlockAlignReductionStream(m_waveReader);
				}
				if (m_waveReader.WaveFormat.BitsPerSample != 16)
				{
					var format = new WaveFormat(m_waveReader.WaveFormat.SampleRate,
					                            16, m_waveReader.WaveFormat.Channels);
					m_waveReader = new WaveFormatConversionStream(format, m_waveReader);
				}

				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else if (fileExt == AIFFExtension)
			{
				m_waveReader = new AiffFileReader(filename);
				m_waveChannel = new WaveChannel32(m_waveReader);
			}
			else
			{
				throw new ApplicationException("Cannot create Input WaveChannel - Unknown file type: " + fileExt);
			}
			return m_waveReader;
			//return m_waveChannel;
		}
		
		public static void ConvertMp3ToWav(string mp3FilePath, string wavFilePath) {
			using (var reader = new Mp3FileReader(mp3FilePath)) {
				using (var writer = new WaveFileWriter(wavFilePath, new WaveFormat())) {
					byte[] buffer = new byte[16*1024];
					for (;;) {
						int bytes = reader.Read(buffer, 0, buffer.Length);
						if (bytes == 0) break;
						writer.Write(buffer, 0, bytes);
					}
				}
			}
		}
		
		public static float[] CropAudioAtSilence(float[] data, double threshold, bool onlyDetectEnd, int addSamples) {

			if (data == null || data.Length == 0) {
				return new float[0];
			}
			
			// process whole file
			int dataLength = data.Length;

			// start at beginning
			int position = 0;

			int beginning = 0;
			int end = data.Length;

			//double threshold = 0.000001; // what is silence
			double sampleValue = 0;

			int count = 0;
			int a = 0;
			if (!onlyDetectEnd) {
				
				// detect start silence
				// count silent samples
				for (a = 0; a < dataLength; a++) {
					sampleValue = Math.Abs(data[a]);
					if (sampleValue > threshold) break;
				}

				// add number of silent samples
				count += a;

				beginning = count;
				System.Diagnostics.Debug.WriteLine("Detected beginning at sample: {0}", beginning);
			}
			
			// detect end silence
			position = data.Length;
			int c = 0;
			while (position > count)
			{
				// step back a bit
				position=position < 20000 ? 0 : position - 20000;

				// count silent samples
				for (c = data.Length; c > 0 ; c--) {
					sampleValue = Math.Abs(data[c - 1]);
					if (sampleValue > threshold) break;
				}

				// if sound has begun...
				if ( c > 0 ) {
					// silence begins here
					//count = position + c;
					count = c;
					break;
				}
			}
			
			// set end position
			end = count;
			if ( (end + addSamples) > data.Length) {
				end = data.Length;
			} else {
				end = end + addSamples;
			}

			// Crop Audio here
			int croppedAudioLength = end-beginning;
			float[] croppedAudio = new float[croppedAudioLength];
			Array.Copy(data, beginning, croppedAudio, 0, croppedAudioLength);
			System.Diagnostics.Debug.WriteLine("Successfully cropping to selection: {0} to {1} (Original Length: {2} samples).", beginning, end, dataLength);
			return croppedAudio;
		}

		public static bool HasDataAboveThreshold(float[] data, double threshold) {
			if (data == null || data.Length == 0) {
				return false;
			}
			
			for (int i = 0; i < data.Length; i++) {
				if (data[i] > threshold) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Retrieve the frequency in Ms for a Rhythmic Value for a given bpm value
		/// </summary>
		/// <param name="bpm"></param>
		public static double LFOOrDelayToMilliseconds(LFOTIMING timing, int bpm=120) {
			// http://classes.berklee.edu/mbierylo/mtec111_Pages/processing/time_based.html
			// see http://tomhess.net/Tools/DelayCalculator.aspx
			// 60,000 / BPM = Quarter beats
			// 40,000 / BPM = Triplets
			// 90,000 / BPM = Dotted
			double DelayTimeInMs = 60000 / bpm;
			
			// return value
			double delayMs = 0.0f;
			switch (timing) {
				case LFOTIMING.LFO_8_1D:
					double Delay8_1NoteDotted 		= DelayTimeInMs*6*8;
					delayMs = Delay8_1NoteDotted;
					break;
				case LFOTIMING.LFO_8_1:
					double Delay8_1Note 			= DelayTimeInMs*4*8;
					delayMs = Delay8_1Note;
					break;
				case LFOTIMING.LFO_8_1T:
					double Delay8_1NoteTriplet 		= DelayTimeInMs*8/3*8;
					delayMs = Delay8_1NoteTriplet;
					break;

				case LFOTIMING.LFO_4_1D:
					double Delay4_1NoteDotted 		= DelayTimeInMs*6*4;
					delayMs = Delay4_1NoteDotted;
					break;
				case LFOTIMING.LFO_4_1:
					double Delay4_1Note		 		= DelayTimeInMs*4*4;
					delayMs = Delay4_1Note;
					break;
				case LFOTIMING.LFO_4_1T:
					double Delay4_1NoteTriplet 		= DelayTimeInMs*8/3*4;
					delayMs = Delay4_1NoteTriplet;
					break;

				case LFOTIMING.LFO_2_1D:
					double Delay2_1NoteDotted 		= DelayTimeInMs*6*2;
					delayMs = Delay2_1NoteDotted;
					break;
				case LFOTIMING.LFO_2_1:
					double Delay2_1Note 			= DelayTimeInMs*4*2;
					delayMs = Delay2_1Note;
					break;
				case LFOTIMING.LFO_2_1T:
					double Delay2_1NoteTriplet 		= DelayTimeInMs*8/3*2;
					delayMs = Delay2_1NoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_1D:
					double DelayWholeNoteDotted 	= DelayTimeInMs*6;
					delayMs = DelayWholeNoteDotted;
					break;
				case LFOTIMING.LFO_1_1:
					double DelayWholeNote 			= DelayTimeInMs*4;
					delayMs = DelayWholeNote;
					break;
				case LFOTIMING.LFO_1_1T:
					double DelayWholeNoteTriplet 	= DelayTimeInMs*8/3;
					delayMs = DelayWholeNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_2D:
					double DelayHalfNoteDotted 		= DelayTimeInMs*6/2;
					delayMs = DelayHalfNoteDotted;
					break;
				case LFOTIMING.LFO_1_2:
					double DelayHalfNote 			= DelayTimeInMs*4/2;
					delayMs = DelayHalfNote;
					break;
				case LFOTIMING.LFO_1_2T:
					double DelayHalfNoteTriplet 	= DelayTimeInMs*8/3/2;
					delayMs = DelayHalfNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_4D:
					double DelayQuarterNoteDotted	= DelayTimeInMs*6/4;
					delayMs = DelayQuarterNoteDotted;
					break;
				case LFOTIMING.LFO_1_4:
					double DelayQuarterNote 		= DelayTimeInMs*4/4;
					delayMs = DelayQuarterNote;
					break;
				case LFOTIMING.LFO_1_4T:
					double DelayQuarterNoteTriplet 	= DelayTimeInMs*8/3/4;
					delayMs = DelayQuarterNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_8D:
					double DelayEighthNoteDotted 	= DelayTimeInMs*6/8;
					delayMs = DelayEighthNoteDotted;
					break;
				case LFOTIMING.LFO_1_8:
					double DelayEighthNote 			= DelayTimeInMs*4/8;
					delayMs = DelayEighthNote;
					break;
				case LFOTIMING.LFO_1_8T:
					double DelayEighthNoteTriplet 	= DelayTimeInMs*8/3/8;
					delayMs = DelayEighthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_16D:
					double DelaySixteenthNoteDotted = DelayTimeInMs*6/16;
					delayMs = DelaySixteenthNoteDotted;
					break;
				case LFOTIMING.LFO_1_16:
					double DelaySixteenthNote 		= DelayTimeInMs*4/16;
					delayMs = DelaySixteenthNote;
					break;
				case LFOTIMING.LFO_1_16T:
					double DelaySixteenthNoteTriplet = DelayTimeInMs*8/3/16;
					delayMs = DelaySixteenthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_32D:
					double Delay32NoteDotted 		= DelayTimeInMs*6/32;
					delayMs = Delay32NoteDotted;
					break;
				case LFOTIMING.LFO_1_32:
					double Delay32Note 				= DelayTimeInMs*4/32;
					delayMs = Delay32Note;
					break;
				case LFOTIMING.LFO_1_32T:
					double Delay32NoteTriplet 		= DelayTimeInMs*8/3/32;
					delayMs = Delay32NoteTriplet;
					break;

				case LFOTIMING.LFO_1_64D:
					double Delay64NoteDotted 		= DelayTimeInMs*6/64;
					delayMs = Delay64NoteDotted;
					break;
				case LFOTIMING.LFO_1_64:
					double Delay64Note 				= DelayTimeInMs*4/64;
					delayMs = Delay64Note;
					break;
				case LFOTIMING.LFO_1_64T:
					double Delay64NoteTriplet 		= DelayTimeInMs*8/3/64;
					delayMs = Delay64NoteTriplet;
					break;

				case LFOTIMING.LFO_1_128D:
					double Delay128NoteDotted 		= DelayTimeInMs*6/128;
					delayMs = Delay128NoteDotted;
					break;
				case LFOTIMING.LFO_1_128:
					double Delay128Note 			= DelayTimeInMs*4/128;
					delayMs = Delay128Note;
					break;
				case LFOTIMING.LFO_1_128T:
					double Delay128NoteTriplet 		= DelayTimeInMs*8/3/128;
					delayMs = Delay128NoteTriplet;
					break;

				case LFOTIMING.LFO_1_256D:
					double Delay256NoteDotted 		= DelayTimeInMs*6/256;
					delayMs = Delay256NoteDotted;
					break;
				case LFOTIMING.LFO_1_256:
					double Delay256Note 			= DelayTimeInMs*4/256;
					delayMs = Delay256Note;
					break;
				case LFOTIMING.LFO_1_256T:
					double Delay256NoteTriplet 		= DelayTimeInMs*8/3/256;
					delayMs = Delay256NoteTriplet;
					break;
			}

			double SecondsPerBar = DelayTimeInMs*4/1000;
			
			return delayMs;
		}
		

		/// <summary>
		/// Retrieve the frequency in Hz for a Rhythmic Value for a given bpm value
		/// </summary>
		/// <param name="bpm"></param>
		public static double LFOOrDelayToFrequency(LFOTIMING timing, int bpm=120) {
			// http://classes.berklee.edu/mbierylo/mtec111_Pages/processing/time_based.html
			// see http://tomhess.net/Tools/DelayCalculator.aspx
			// 60,000 / BPM = Quarter beats
			// 40,000 / BPM = Triplets
			// 90,000 / BPM = Dotted
			double DelayTimeInMs = 60000 / bpm;
			
			// return value
			double delayHz = 0.0f;
			switch (timing) {
				case LFOTIMING.LFO_8_1D:
					double Delay8_1NoteDotted 		= 1000/(DelayTimeInMs*6*8);
					delayHz = Delay8_1NoteDotted;
					break;
				case LFOTIMING.LFO_8_1:
					double Delay8_1Note 			= 1000/(DelayTimeInMs*4*8);
					delayHz = Delay8_1Note;
					break;
				case LFOTIMING.LFO_8_1T:
					double Delay8_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*8);
					delayHz = Delay8_1NoteTriplet;
					break;

				case LFOTIMING.LFO_4_1D:
					double Delay4_1NoteDotted 		= 1000/(DelayTimeInMs*6*4);
					delayHz = Delay4_1NoteDotted;
					break;
				case LFOTIMING.LFO_4_1:
					double Delay4_1Note		 		= 1000/(DelayTimeInMs*4*4);
					delayHz = Delay4_1Note;
					break;
				case LFOTIMING.LFO_4_1T:
					double Delay4_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*4);
					delayHz = Delay4_1NoteTriplet;
					break;

				case LFOTIMING.LFO_2_1D:
					double Delay2_1NoteDotted 		= 1000/(DelayTimeInMs*6*2);
					delayHz = Delay2_1NoteDotted;
					break;
				case LFOTIMING.LFO_2_1:
					double Delay2_1Note 			= 1000/(DelayTimeInMs*4*2);
					delayHz = Delay2_1Note;
					break;
				case LFOTIMING.LFO_2_1T:
					double Delay2_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*2);
					delayHz = Delay2_1NoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_1D:
					double DelayWholeNoteDotted 	= 1000/(DelayTimeInMs*6);
					delayHz = DelayWholeNoteDotted;
					break;
				case LFOTIMING.LFO_1_1:
					double DelayWholeNote 			= 1000/(DelayTimeInMs*4);
					delayHz = DelayWholeNote;
					break;
				case LFOTIMING.LFO_1_1T:
					double DelayWholeNoteTriplet 	= 1000/(DelayTimeInMs*8/3);
					delayHz = DelayWholeNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_2D:
					double DelayHalfNoteDotted 		= 1000/(DelayTimeInMs*6/2);
					delayHz = DelayHalfNoteDotted;
					break;
				case LFOTIMING.LFO_1_2:
					double DelayHalfNote 			= 1000/(DelayTimeInMs*4/2);
					delayHz = DelayHalfNote;
					break;
				case LFOTIMING.LFO_1_2T:
					double DelayHalfNoteTriplet 	= 1000/(DelayTimeInMs*8/3/2);
					delayHz = DelayHalfNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_4D:
					double DelayQuarterNoteDotted	= 1000/(DelayTimeInMs*6/4);
					delayHz = DelayQuarterNoteDotted;
					break;
				case LFOTIMING.LFO_1_4:
					double DelayQuarterNote 		= 1000/(DelayTimeInMs*4/4);
					delayHz = DelayQuarterNote;
					break;
				case LFOTIMING.LFO_1_4T:
					double DelayQuarterNoteTriplet 	= 1000/(DelayTimeInMs*8/3/4);
					delayHz = DelayQuarterNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_8D:
					double DelayEighthNoteDotted 	= 1000/(DelayTimeInMs*6/8);
					delayHz = DelayEighthNoteDotted;
					break;
				case LFOTIMING.LFO_1_8:
					double DelayEighthNote 			= 1000/(DelayTimeInMs*4/8);
					delayHz = DelayEighthNote;
					break;
				case LFOTIMING.LFO_1_8T:
					double DelayEighthNoteTriplet 	= 1000/(DelayTimeInMs*8/3/8);
					delayHz = DelayEighthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_16D:
					double DelaySixteenthNoteDotted = 1000/(DelayTimeInMs*6/16);
					delayHz = DelaySixteenthNoteDotted;
					break;
				case LFOTIMING.LFO_1_16:
					double DelaySixteenthNote 		= 1000/(DelayTimeInMs*4/16);
					delayHz = DelaySixteenthNote;
					break;
				case LFOTIMING.LFO_1_16T:
					double DelaySixteenthNoteTriplet = 1000/(DelayTimeInMs*8/3/16);
					delayHz = DelaySixteenthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_32D:
					double Delay32NoteDotted 		= 1000/(DelayTimeInMs*6/32);
					delayHz = Delay32NoteDotted;
					break;
				case LFOTIMING.LFO_1_32:
					double Delay32Note 				= 1000/(DelayTimeInMs*4/32);
					delayHz = Delay32Note;
					break;
				case LFOTIMING.LFO_1_32T:
					double Delay32NoteTriplet 		= 1000/(DelayTimeInMs*8/3/32);
					delayHz = Delay32NoteTriplet;
					break;

				case LFOTIMING.LFO_1_64D:
					double Delay64NoteDotted 		= 1000/(DelayTimeInMs*6/64);
					delayHz = Delay64NoteDotted;
					break;
				case LFOTIMING.LFO_1_64:
					double Delay64Note 				= 1000/(DelayTimeInMs*4/64);
					delayHz = Delay64Note;
					break;
				case LFOTIMING.LFO_1_64T:
					double Delay64NoteTriplet 		= 1000/(DelayTimeInMs*8/3/64);
					delayHz = Delay64NoteTriplet;
					break;

				case LFOTIMING.LFO_1_128D:
					double Delay128NoteDotted 		= 1000/(DelayTimeInMs*6/128);
					delayHz = Delay128NoteDotted;
					break;
				case LFOTIMING.LFO_1_128:
					double Delay128Note 			= 1000/(DelayTimeInMs*4/128);
					delayHz = Delay128Note;
					break;
				case LFOTIMING.LFO_1_128T:
					double Delay128NoteTriplet 		= 1000/(DelayTimeInMs*8/3/128);
					delayHz = Delay128NoteTriplet;
					break;

				case LFOTIMING.LFO_1_256D:
					double Delay256NoteDotted 		= 1000/(DelayTimeInMs*6/256);
					delayHz = Delay256NoteDotted;
					break;
				case LFOTIMING.LFO_1_256:
					double Delay256Note 			= 1000/(DelayTimeInMs*4/256);
					delayHz = Delay256Note;
					break;
				case LFOTIMING.LFO_1_256T:
					double Delay256NoteTriplet 		= 1000/(DelayTimeInMs*8/3/256);
					delayHz = Delay256NoteTriplet;
					break;
			}
			return delayHz;
		}

		public static void OutputLFOTimings() {
			foreach (LFOTIMING timing in Enum.GetValues(typeof(LFOTIMING)))
			{
				Console.Out.WriteLine("{0}\t{1:0.000} ms\t{2:0.000} Hz", Enum.GetName(typeof(LFOTIMING), timing).PadRight(12), LFOOrDelayToMilliseconds(timing), AudioUtils.LFOOrDelayToFrequency(timing));
			}			
		}
	}
	
	
	public class SineWaveProvider : WaveProvider32
	{
		private int sample = 0;

		public SineWaveProvider() : base (44100, 2)
		{
			Frequency1 = 1000;
			Frequency2 = 200;
			Amplitude1 = 0.05f;
			Amplitude2 = 0.25f;
		}
		
		public float Frequency1 { get; set; }
		public float Frequency2 { get; set; }
		public float Amplitude1 { get; set; }
		public float Amplitude2 { get; set; }

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			int sampleRate = WaveFormat.SampleRate * WaveFormat.Channels;

			double freqCoef1 = 2 * Math.PI * Frequency1;
			double freqCoef2 = 2 * Math.PI * Frequency2;
			
			for (int n = 0; n < sampleCount; n+=2)
			{
				buffer[offset++] = (float)(Amplitude1 * Math.Sin((freqCoef1 * sample) / sampleRate));
				buffer[offset++] = (float)(Amplitude2 * Math.Sin((freqCoef2 * sample) / sampleRate));
				if (++sample >= sampleRate) sample = 0;
			}
			
			return sampleCount;
		}
	}
}
