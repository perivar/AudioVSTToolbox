using System;
using System.IO;

using NAudio;
using NAudio.Wave;

namespace CommonUtils.Audio
{
	/// <summary>
	/// Description of StringUtils.
	/// </summary>
	public static class AudioUtils
	{
		
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

		public static void ResampleWavMono8khz16bit(string wavInFilePath, string wavOutFilePath) {
			ResampleWav(wavInFilePath, wavOutFilePath, new WaveFormat(8000, 16, 1));
		}
		
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
			WaveStream sourceStream = new WaveFileReader(wavInFilePath);
			
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
			
			sourceStream = new WaveChannel32(sourceStream);
			
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
		
		public static byte[] ResampleWavOLD(string wavInFilePath, WaveFormat waveFormat) {
			using (var reader = new WaveFileReader(wavInFilePath))
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
		
		public static void CreateWaveFile(Stream stream, string wavOutFilePath) {
			using (WaveFileWriter writer = new WaveFileWriter(wavOutFilePath, new WaveFormat()))
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
