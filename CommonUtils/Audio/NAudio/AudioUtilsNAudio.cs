using System;
using System.IO;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Collections.Generic;

namespace CommonUtils.Audio.NAudio
{
	/// <summary>
	/// Audio Methods that make use of NAudio
	/// </summary>
	public static class AudioUtilsNAudio
	{
		const string MP3Extension = ".mp3";
		const string WAVExtension = ".wav";
		
		/// <summary>
		/// Creates an WaveStream
		/// (Audio file reader for MP3/WAV)
		/// </summary>
		/// <param name="filename"></param>
		public static WaveStream CreateInputWaveStream(string filename)
		{
			WaveStream m_waveReader = null;

			string fileExt = Path.GetExtension( filename.ToLower() );
			if ( fileExt == MP3Extension ) {
				m_waveReader = new Mp3FileReader(filename);
			} else if ( fileExt == WAVExtension ) {
				m_waveReader = new WaveFileReader(filename);
			} else {
				throw new ApplicationException("Cannot create Input WaveStream - Unknown file type: " + fileExt);
			}
			return m_waveReader;
		}
		
		public static TimeSpan GetWaveFileTotalTime(string filePath) {
			TimeSpan totalTime = TimeSpan.MinValue;
			using (AudioFileReader reader = new AudioFileReader(filePath)) {
				totalTime = reader.TotalTime;
			}
			return totalTime;
		}

		public static WaveFormat GetWaveFormat(string filePath) {
			WaveFormat frmt = null;
			using (AudioFileReader reader = new AudioFileReader(filePath)) {
				frmt = reader.WaveFormat;
			}
			return frmt;
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

			WaveStream sourceStream = CreateInputWaveStream(wavInFilePath);
			
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				sourceStream = WaveFormatConversionStream.CreatePcmStream(sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			if (sourceStream.WaveFormat.SampleRate != waveFormat.SampleRate ||
			    sourceStream.WaveFormat.BitsPerSample != waveFormat.BitsPerSample ||
			    sourceStream.WaveFormat.Channels != waveFormat.Channels)
			{
				sourceStream = new WaveFormatConversionStream(waveFormat, sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			
			return sourceStream;
		}
		
		public static SampleChannel ResampleToSampleChannel(string wavInFilePath, WaveFormat waveFormat) {

			WaveStream sourceStream = CreateInputWaveStream(wavInFilePath);
			
			if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				sourceStream = WaveFormatConversionStream.CreatePcmStream(sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			if (sourceStream.WaveFormat.SampleRate != waveFormat.SampleRate ||
			    sourceStream.WaveFormat.BitsPerSample != waveFormat.BitsPerSample ||
			    sourceStream.WaveFormat.Channels != waveFormat.Channels)
			{
				sourceStream = new WaveFormatConversionStream(waveFormat, sourceStream);
				sourceStream = new BlockAlignReductionStream(sourceStream);
			}
			
			SampleChannel sampleChannel = new SampleChannel(sourceStream);
			return sampleChannel;
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
		
		#region ReadMonoFromFile
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
				while (TryReadFloat(wave32, out sampleValue) == true
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
				while (TryReadFloat(wave32, out sampleValueLeft, out sampleValueRight) == true
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

		public static float[] ReadMonoFromFile2(string filename, int samplerate, int milliseconds, int startmillisecond, int channelToUse=1)
		{
			int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
			float[] data = null;

			// read as mono file
			List<float> floatList = new List<float>();
			WaveFormat waveFormat = new WaveFormat(samplerate, 1);
			SampleChannel sampleChannel = ResampleToSampleChannel(filename, waveFormat);
			
			int sampleCount = 0;
			int bufferSize = 5512; /*read ten seconds at each iteration*/
			float[] buffer = new float[bufferSize];

			// read until we have read the number of samples (measured in ms) we are supposed to do
			while (sampleChannel.Read(buffer, 0, bufferSize) > 0 && (float)(sampleCount)/ samplerate * 1000 < totalmilliseconds)
			{
				floatList.AddRange(buffer);
				
				// increment with size of data
				sampleCount += bufferSize;
			}
			data = floatList.ToArray();

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
		#endregion
		
		/// <summary>
		/// Split a Stereo Wave file into two mono float arrays
		/// </summary>
		/// <param name="filePath">file to use</param>
		/// <param name="audioDataLeft">returned float array for the left channel</param>
		/// <param name="audioDataRight">returned float array for the right channel</param>
		public static void SplitStereoWaveFileToMono(string filePath, out float[] audioDataLeft, out float[] audioDataRight) {
			
			//using (WaveFileReader pcm = new WaveFileReader(filePath))
			using (AudioFileReader pcm = new AudioFileReader(filePath))
			{
				int channels = pcm.WaveFormat.Channels;
				int bytesPerSample = pcm.WaveFormat.BitsPerSample/8;
				
				long samplesDesired = pcm.Length;
				byte[] buffer = new byte[samplesDesired];
				audioDataLeft = new float[samplesDesired/bytesPerSample/channels];
				audioDataRight = new float[samplesDesired/bytesPerSample/channels];
				int bytesRead = pcm.Read(buffer, 0, buffer.Length);
				int index = 0;
				
				for(int sample = 0; sample < bytesRead/bytesPerSample/channels; sample++)
				{
					if (bytesPerSample == 4) {
						// 32 bit pcm data as float
						audioDataLeft[sample] = BitConverter.ToSingle(buffer, index);
						index += bytesPerSample;
						audioDataRight[sample] = BitConverter.ToSingle(buffer, index);
						index += bytesPerSample;
					} else if (bytesPerSample == 2) {
						// 16 bit pcm data
						audioDataLeft[sample] = (float)BitConverter.ToInt16(buffer, index) / 32768f;
						index += bytesPerSample;
						audioDataRight[sample] = (float)BitConverter.ToInt16(buffer, index) / 32768f;
						index += bytesPerSample;
					}
				}
			}
		}
		
		/// <summary>
		/// Write a audio float array to a 32 bit float audio file
		/// </summary>
		/// <param name="outputFilePath">file path to output file</param>
		/// <param name="sampleRate">sample rate</param>
		/// <param name="audioData">the audio float array</param>
		public static void WriteIEEE32WaveFileMono(string outputFilePath, int sampleRate, float[] audioData) {
			using (WaveFileWriter wavWriter = new WaveFileWriter(outputFilePath, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1)))
			{
				wavWriter.WriteSamples(audioData, 0, audioData.Length);
			}
		}
		
		/// <summary>
		/// Combine two stereo files to one quad file
		/// </summary>
		/// <param name="filePathLeft">file path to the left stereo file</param>
		/// <param name="filePathRight">file path to the right stereo file</param>
		/// <param name="combinedFileNamePath">file path to the combined quad file</param>
		/// <returns></returns>
		public static bool CombineStereoToQuad(string filePathLeft, string filePathRight, string combinedFileNamePath) {
			
			WaveFormat waveFormatLeft = GetWaveFormat(filePathLeft);
			WaveFormat waveFormatRight = GetWaveFormat(filePathRight);
			if (!waveFormatLeft.Equals(waveFormatRight)) {
				Console.Out.WriteLine("The two files to combine must have the same format");
				return false;
			}
			if (waveFormatLeft.Channels != 2 || waveFormatRight.Channels != 2) {
				Console.Out.WriteLine("The two files to combine must be stereo");
				return false;
			}

			int sampleRate = waveFormatLeft.SampleRate;
			
			float[] channel1;
			float[] channel2;
			float[] channel3;
			float[] channel4;
			SplitStereoWaveFileToMono(filePathLeft, out channel1, out channel2);
			SplitStereoWaveFileToMono(filePathRight, out channel3, out channel4);
			
			// find out what channel is longest
			int maxLength = Math.Max(channel1.Length, channel3.Length);

			using (WaveFileWriter wavWriter = new WaveFileWriter(combinedFileNamePath, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 4)))
			{
				// write one and one float (interlaced), pad if neccesary
				for (int i = 0; i < maxLength; i++) {
					if (i < channel1.Length) {
						wavWriter.WriteSample(channel1[i]);
					} else {
						wavWriter.WriteSample(0.0f);
					}
					if (i < channel2.Length) {
						wavWriter.WriteSample(channel2[i]);
					} else {
						wavWriter.WriteSample(0.0f);
					}
					if (i < channel3.Length) {
						wavWriter.WriteSample(channel3[i]);
					} else {
						wavWriter.WriteSample(0.0f);
					}
					if (i < channel4.Length) {
						wavWriter.WriteSample(channel4[i]);
					} else {
						wavWriter.WriteSample(0.0f);
					}
				}
			}
			return true;
		}
		
		/// <summary>
		/// Convert a MP3 file to a Wav file
		/// </summary>
		/// <param name="mp3FilePath"></param>
		/// <param name="wavFilePath"></param>
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
	}
	
	#region Providers
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
	#endregion
}
