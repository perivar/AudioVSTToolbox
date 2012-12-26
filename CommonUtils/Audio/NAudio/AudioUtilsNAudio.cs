using System;
using System.IO;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Collections.Generic;

using System.Linq;

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
		
		/// <summary>
		/// Return the total time of an audio file
		/// </summary>
		/// <param name="filePath">audio file</param>
		/// <returns>a TimeSpan of the total time</returns>
		public static TimeSpan GetWaveFileTotalTime(string filePath) {
			TimeSpan totalTime = TimeSpan.MinValue;
			using (AudioFileReader reader = new AudioFileReader(filePath)) {
				totalTime = reader.TotalTime;
			}
			return totalTime;
		}

		/// <summary>
		/// Return the waveformat of an audio file
		/// </summary>
		/// <param name="filePath">audio file</param>
		/// <returns>the files WaveFormat</returns>
		public static WaveFormat GetWaveFormat(string filePath) {
			WaveFormat frmt = null;
			using (AudioFileReader reader = new AudioFileReader(filePath)) {
				frmt = reader.WaveFormat;
			}
			return frmt;
		}
		
		/// <summary>
		/// Read a complete stream into a byte array
		/// </summary>
		/// <param name="input">a stream</param>
		/// <returns>byte[] array</returns>
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
		
		/// <summary>
		/// Create a wavefile using a stream
		/// </summary>
		/// <param name="inputStream">the input audio stream</param>
		/// <param name="fileName">the audio file to save</param>
		/// <param name="waveFormat">waveformat</param>
		public static void CreateWaveFile(Stream inputStream, string fileName, WaveFormat waveFormat) {
			using (WaveFileWriter writer = new WaveFileWriter(fileName, waveFormat))
			{
				byte[] buffer = new byte[16*1024];
				for (; ;)
				{
					int bytes = inputStream.Read(buffer, 0, buffer.Length);
					if (bytes == 0) break;
					writer.Write(buffer, 0, bytes);
				}
			}
		}

		/// <summary>
		/// Create a wavefile using audiodata
		/// </summary>
		/// <param name="audioData">the input audio data</param>
		/// <param name="fileName">the audio file to save</param>
		/// <param name="waveFormat">waveformat</param>
		public static void CreateWaveFile(float[] audioData, string fileName, WaveFormat waveFormat) {
			using (WaveFileWriter writer = new WaveFileWriter(fileName, waveFormat))
			{
				writer.WriteSamples(audioData, 0, audioData.Length);
			}
		}
		#endregion
		
		#region ResampleWav methods

		/// <summary>
		/// Resample wavefile to new waveformat and save output file
		/// </summary>
		/// <param name="wavInFilePath">input audiofile</param>
		/// <param name="wavOutFilePath">output wavefile</param>
		/// <param name="waveFormat">waveformat</param>
		public static void ResampleWav(string wavInFilePath, string wavOutFilePath, WaveFormat waveFormat) {
			using (var reader = new WaveFileReader(wavInFilePath))
			{
				using (var conversionStream = new WaveFormatConversionStream(waveFormat, reader))
				{
					WaveFileWriter.CreateWaveFile(wavOutFilePath, conversionStream);
				}
			}
		}
		
		/// <summary>
		/// Resample wavefile to new waveformat and save output file
		/// </summary>
		/// <param name="wavIn">input stream</param>
		/// <param name="waveFormat">waveformat</param>
		/// <returns>byte[] array</returns>
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

		/// <summary>
		/// Resample wavefile to new waveformat and return WaveStream
		/// </summary>
		/// <param name="wavInFilePath">audiofile</param>
		/// <param name="waveFormat">waveformat</param>
		/// <returns>WaveStream</returns>
		public static WaveStream ResampleToWaveStream(string wavInFilePath, WaveFormat waveFormat) {

			//WaveStream sourceStream = CreateInputWaveStream(wavInFilePath);
			WaveStream sourceStream = (WaveStream) new AudioFileReader(wavInFilePath);
			
			if (sourceStream.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat) {
				// TODO: if the source is IeeFloat, the CreatePcmSteeam and ConverstionStream fail!?
				return sourceStream;
			} else if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm) {
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
		
		/// <summary>
		/// Resample wavefile to new waveformat and return SampleChannel (supports floats)
		/// </summary>
		/// <param name="wavInFilePath">audiofile</param>
		/// <param name="waveFormat">waveformat</param>
		/// <returns>SampleChannel (supports floats)</returns>
		public static SampleChannel ResampleToSampleChannel(string wavInFilePath, WaveFormat waveFormat) {
			return new SampleChannel(ResampleToWaveStream(wavInFilePath, waveFormat));
		}

		/// <summary>
		/// Resample wavefile to new waveformat and return byte array
		/// </summary>
		/// <param name="wavInFilePath">audiofile</param>
		/// <param name="waveFormat">waveformat</param>
		/// <returns>byte[] array</returns>
		public static byte[] ResampleWavToBytes(string wavInFilePath, WaveFormat waveFormat) {
			using (var reader = ResampleToWaveStream(wavInFilePath, waveFormat))
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
		
		/// <summary>
		/// Resample wavefile to new waveformat and return float array
		/// </summary>
		/// <param name="wavInFilePath">audiofile</param>
		/// <param name="waveFormat">waveformat</param>
		/// <returns>float[] array</returns>
		public static float[] ResampleWavToFloats(string wavInFilePath, WaveFormat waveFormat) {

			List<float> floatList = new List<float>();
			SampleChannel sampleChannel = ResampleToSampleChannel(wavInFilePath, waveFormat);
			
			int readCount = 0;
			int bufferSize = 16*1024;
			float[] buffer = new float[bufferSize];
			
			// read until we have read the number of samples (measured in ms) we are supposed to do
			while ((readCount = sampleChannel.Read(buffer, 0, bufferSize)) > 0)
			{
				floatList.AddRange(buffer.Take(readCount));
			}
			return floatList.ToArray();
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
		public static float[] ReadMonoFromFileOld(string filename, int samplerate, int milliseconds, int startmillisecond, int channelToUse=1)
		{
			int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
			float[] data = null;

			// read as stereo file - do the mono thing later
			List<float> floatList = new List<float>();
			WaveFormat waveFormat = new WaveFormat(samplerate, 2);
			WaveStream wave32 = ResampleToWaveStream(filename, waveFormat);
			
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
			int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
			float[] data = null;

			// read as mono file
			List<float> floatList = new List<float>();
			WaveFormat waveFormat = new WaveFormat(samplerate, 1);
			//WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(samplerate, 1);
			SampleChannel sampleChannel = ResampleToSampleChannel(filename, waveFormat);
			
			int sampleCount = 0;
			int readCount = 0;
			int bufferSize = 16*1024;
			float[] buffer = new float[bufferSize];
			
			// read until we have read the number of samples (measured in ms) we are supposed to do
			while ((readCount = sampleChannel.Read(buffer, 0, bufferSize)) > 0 && (float)(sampleCount)/ samplerate * 1000 < totalmilliseconds)
			{
				floatList.AddRange(buffer.Take(readCount));
				
				// increment with size of data
				sampleCount += readCount;
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
			if (start != 0 || end != sampleCount) {
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
		
		public static void GenerateAudioTestFile(int sampleRate, int secondsToSample, string filePath) {
			
			int totalNumberOfSamples = sampleRate * secondsToSample;
			float[] audioData = new float[totalNumberOfSamples];
			BasicOscillatorProvider basic = new BasicOscillatorProvider();

			int length = (int) MathUtils.RoundDown(totalNumberOfSamples / 22);
			int offset = 0;
			int startFrequency = 110;

			basic.Amplitude = MathUtils.DecibelToAmplitude(-90);
			basic.SetFrequency(startFrequency*1);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(-80);
			basic.SetFrequency(startFrequency*2);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = MathUtils.DecibelToAmplitude(-70);
			basic.SetFrequency(startFrequency*3);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SQUARE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(-60);
			basic.SetFrequency(startFrequency*4);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.TRIANGLE);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = MathUtils.DecibelToAmplitude(-50);
			basic.SetFrequency(startFrequency*5);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(-40);
			basic.SetFrequency(startFrequency*6);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = MathUtils.DecibelToAmplitude(-30);
			basic.SetFrequency(startFrequency*7);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SQUARE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(-20);
			basic.SetFrequency(startFrequency*8);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.TRIANGLE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(-10);
			basic.SetFrequency(startFrequency*9);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(0);
			basic.SetFrequency(startFrequency*10);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = MathUtils.DecibelToAmplitude(10);
			basic.SetFrequency(startFrequency*11);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SQUARE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = MathUtils.DecibelToAmplitude(20);
			basic.SetFrequency(startFrequency*12);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.TRIANGLE);
			offset += basic.Read(audioData, offset, length);

			// Down again
			basic.Amplitude = 1.0f;
			basic.SetFrequency(startFrequency*11);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.9f;
			basic.SetFrequency(startFrequency*10);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = 0.8f;
			basic.SetFrequency(startFrequency*9);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SQUARE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.7f;
			basic.SetFrequency(startFrequency*8);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.TRIANGLE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.6f;
			basic.SetFrequency(startFrequency*7);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.5f;
			basic.SetFrequency(startFrequency*6);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			basic.Amplitude = 0.4f;
			basic.SetFrequency(startFrequency*5);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SQUARE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.3f;
			basic.SetFrequency(startFrequency*4);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.TRIANGLE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.2f;
			basic.SetFrequency(startFrequency*3);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SINE);
			offset += basic.Read(audioData, offset, length);

			basic.Amplitude = 0.1f;
			basic.SetFrequency(startFrequency*2);
			basic.SetOscWaveshape(BasicOscillatorProvider.WAVESHAPE.SAW);
			offset += basic.Read(audioData, offset, length);
			
			WriteIEEE32WaveFileMono(filePath, sampleRate, audioData);
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
	
	public class BasicOscillatorProvider : WaveProvider32
	{
		// Instance data
		private WAVESHAPE waveshape;
		private long periodSamples;
		private long sampleNumber;
		private Random rnd;

		public float Amplitude { get; set; }
		
		/**
		 * Waveshape enumeration
		 */
		public enum WAVESHAPE {
			SINE, SQUARE, SAW, TRIANGLE, NOISE
		}
		
		/**
		 * Set waveshape of oscillator
		 * @param waveshape Determines the waveshape of this oscillator
		 */
		public void SetOscWaveshape(WAVESHAPE waveshape) {
			
			this.waveshape = waveshape;
		}
		
		/**
		 * Set the frequency of the oscillator in Hz.
		 * @param frequency Frequency in Hz for this oscillator
		 */
		public void SetFrequency(double frequency) {
			
			periodSamples = (long)(this.WaveFormat.SampleRate / frequency);
		}
		
		public BasicOscillatorProvider() : base (44100, 1) // 44.1 kHz stereo
		{
			// Set defaults
			SetOscWaveshape(WAVESHAPE.SINE);
			SetFrequency(440.0);
			Amplitude = 1.0f; // Max
			
			rnd = new Random();
		}
		
		/**
		 * Return the next sample of the oscillator's waveform
		 *
		 * @return Next oscillator sample
		 */
		protected double getSample() {
			
			/**	The relative position inside the period
				of the waveform. 0.0 = beginning, 1.0 = end
			 */
			double x = sampleNumber / (double) periodSamples;
			double value;
			
			switch (waveshape) {
				default:
				case WAVESHAPE.SINE:
					value = Math.Sin(2.0 * Math.PI * x);
					break;
					
				case WAVESHAPE.SQUARE:
					if (sampleNumber < (periodSamples / 2)) {
						value = 1.0;
					}  else  {
						value = -1.0;
					}
					break;
					
				case WAVESHAPE.SAW:
					value = 2.0 * (x - Math.Floor(x + 0.5));
					break;

				case WAVESHAPE.TRIANGLE:
					if (x < 0.25f) {
						value = 4.0f * x;
					}
					else if (x < 0.75f) {
						value = -4.0f * (x - 0.5f);
					} else {
						value = 4.0f * (x - 1.0f);
					}
					break;
					
				case WAVESHAPE.NOISE:
					value = rnd.NextDouble(); 	// between 0..1
					value *= 2;            		// between 0..2
					value--;               		// between -1..1
					break;
			}
			sampleNumber = (sampleNumber + 1) % periodSamples;
			return value;
		}
		
		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			int sampleRate = WaveFormat.SampleRate * WaveFormat.Channels;

			for (int n = 0; n < sampleCount; n++)
			{
				float fs = (float) (Amplitude * getSample());
				buffer[n+offset] = fs;
				if (sampleNumber >= sampleRate) sampleNumber = 0;
			}
			
			return sampleCount;
		}
	}

	#endregion
}
