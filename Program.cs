﻿/*
 * The goal of this project is to be able to take a wave sample and create a synth preset for u-he Zebra 2 synth from that
 * Date: 27.07.2011
 * Time: 12:13
 */
using System;
using System.Collections.Generic;

using Wave2ZebraSynth.Audio;
using Wave2ZebraSynth.Fingerprinting;
using Wave2ZebraSynth.DataAccess;
using Wave2ZebraSynth.Model;
using Wave2ZebraSynth.Fingerprinting.MathUtils;

using Lomont;

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

using Wave2ZebraSynth.HermitGauges;

namespace Wave2ZebraSynth
{
	class Program
	{
		
		///   Music file filters
		/// </summary>
		private static readonly string[] _musicFileFilters = new[] {"*.mp3", "*.ogg", "*.flac", "*.wav"};
		
		public static void Main(string[] args)
		{
			InstrumentPanel panel = new InstrumentPanel(800, 600);
			panel.Instrument = InstrumentPanel.Instruments.SPECTRUM_P_W;
			AudioAnalyser analyser = panel.AudioAnalyser;
			
			String fileName = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl\01. Only Girl (In The World).mp3";
			//String fileName = @"C:\Users\perivar.nerseth\Music\Sine-500hz-60sec.wav";
			//String fileName = @"G:\Cubase and Nuendo Projects\Music To Copy Learn\Britney Spears - Hold It Against Me\02 Hold It Against Me (Instrumental) 1.mp3";
			
			//String path = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl";
			
			Console.WriteLine("Analyzer starting ...");
			
			RepositoryGateway repositoryGateway = new RepositoryGateway();
			FingerprintManager manager = new FingerprintManager();

			// extract tags
			TAG_INFO tag = repositoryGateway._proxy.GetTagInfoFromFile(fileName);
			
			// VB6 FFT
			//double sampleRate = 5512;// 44100  default 5512
			//int fftWindowsSize = 2048; //16384  default 256*8 (2048) to 256*128 (32768), reccomended: 256*64 = 16384
			//float fftOverlapPercentage = 0.10f;
			//float[] wavDataVB6 = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, 20*1000, 20*1000 );
			//double[] magnitude = VB6Spectrogram.Compute(wavDataVB6, 1, sampleRate, fftWindowsSize, fftOverlapPercentage);
			//exportCSV (@"c:\VB6-magnitude.csv", magnitude);
			
			// Lomont FFT
			double sampleRate = 5512;// 44100  default 5512
			int fftWindowsSize = 2048; //16384  default 256*8 (2048) to 256*128 (32768), reccomended: 256*64 = 16384
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			int fftOverlap = fftWindowsSize / 2; //64;
			float[] wavData = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, 15*1000, 20*1000 );
			
			float min = 0.0f;
			float max = 0.0f;
			ComputeMinAndMax(wavData, out min, out max);
			System.Diagnostics.Debug.WriteLine(String.Format("Wav data value range: Min: {0}. Max: {1}.", min, max));
			float[] wavDataFixed = ConvertRangeAndMainainRatio(wavData, min, max, -32768, 32767);
			
			float minNew = 0.0f;
			float maxNew = 0.0f;
			ComputeMinAndMax(wavDataFixed, out minNew, out maxNew);
			System.Diagnostics.Debug.WriteLine(String.Format("New data value range: Min: {0}. Max: {1}.", minNew, maxNew));

			float[] wavDataDb = ConvertRangeAndMainainRatioLog(wavData, min, max, 0, 100);
			exportCSV (String.Format("c:\\{0}-samples-and-short-range-conversion-{1}-{2}.csv", System.IO.Path.GetFileNameWithoutExtension(fileName), sampleRate, fftWindowsSize), wavData, wavDataFixed, wavDataDb);
			
			short[] shortData = new short[wavDataFixed.Length];
			for (int z = 0; z < wavDataFixed.Length; z++) {
				float f = wavDataFixed[z];
				double d = (double) f;
				short s = (short) d;
				shortData[z] = s;
			}
			analyser.SampleRate = (int) sampleRate;
			analyser.ProcessAudio(shortData);
			//return;

			float[][] lomontSpectrogram = CreateSpectrogram(wavData, sampleRate, fftWindowsSize, fftOverlap);
			repositoryGateway.drawSpectrogram2("LomontSpectrum", fileName, lomontSpectrogram, sampleRate, fftWindowsSize);
			//exportCSV (@"c:\LomontSpectrogram-full-not-normalized.csv", lomontSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Lomont", fileName, lomontSpectrogram, sampleRate, fftWindowsSize, fftOverlap);

			// draw waveform
			exportCSV (String.Format("c:\\{0}-samples-{1}-{2}.csv", System.IO.Path.GetFileNameWithoutExtension(fileName), sampleRate, fftWindowsSize), wavData);
			repositoryGateway.drawWaveform("Waveform", fileName, wavData);
			repositoryGateway.drawWaveform2("WaveformV2", fileName, wavDataFixed, false);
			FingerprintManager.NormalizeInPlace(wavData);
			exportCSV (String.Format("c:\\{0}-samples-normalized-{1}-{2}.csv", System.IO.Path.GetFileNameWithoutExtension(fileName), sampleRate, fftWindowsSize), wavData);
			repositoryGateway.drawWaveform("Waveform-Normalized", fileName, wavData);
			
			// Exocortex.DSP FFT
			// read 5512 Hz, Mono, PCM, with a specific proxy
			float[][] exoSpectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START, false);
			repositoryGateway.drawSpectrogram("ExoSpectrum", fileName, exoSpectrogram);
			//exportCSV (@"c:\ExoSpectrogram-full-not-normalized.csv", exoSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Exo", fileName, exoSpectrogram, manager.SampleRate, manager.WdftSize, fftOverlap);
			
			/*
			float[][] logSpectrogram = manager.CreateLogSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);
			repositoryGateway.drawSpectrum("LogSpectrogram", fileName, logSpectrogram);
			
			List<bool[]> dbFingers = manager.CreateFingerprints(logSpectrogram, repositoryGateway._createStride);
			
			List<string> musicFiles = Helper.GetMusicFiles(path, Program._musicFileFilters, true); //get music file names
			List<Track> processedMusicFiles = repositoryGateway.ProcessFiles(musicFiles, null);
			 */
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

		#region exportCSV
		private static void exportCSV(string filenameToSave, float[] data) {
			object[][] arr = new object[data.Length][];
			
			int count = 1;
			for (int i = 0; i < data.Length; i++)
			{
				arr[i] = new object[2] {
					count,
					data[i]
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}

		private static void exportCSV(string filenameToSave, double[] data) {
			object[][] arr = new object[data.Length][];
			
			int count = 1;
			for (int i = 0; i < data.Length; i++)
			{
				arr[i] = new object[2] {
					count,
					data[i]
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}
		
		private static void exportCSV(string filenameToSave, float[][] data) {
			object[][] arr = new object[data.Length][];

			for (int i = 0; i < data.Length; i++)
			{
				arr[i] = new object[data[i].Length];
				for (int j = 0; j < data[i].Length; j++)
				{
					arr[i][j] = data[i][j];
				}
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}

		private static void exportCSV(string filenameToSave, float[] column1, float[] column2) {
			if (column1.Length != column2.Length) return;
			
			object[][] arr = new object[column1.Length][];

			int count = 1;
			for (int i = 0; i < column1.Length; i++)
			{
				arr[i] = new object[3] {
					count,
					column1[i],
					column2[i]
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}

		private static void exportCSV(string filenameToSave, float[] column1, float[] column2, float[] column3) {
			if (column1.Length != column2.Length || column1.Length != column3.Length) return;
			
			object[][] arr = new object[column1.Length][];

			int count = 1;
			for (int i = 0; i < column1.Length; i++)
			{
				arr[i] = new object[4] {
					count,
					column1[i],
					column2[i],
					column3[i]
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}

		private static void exportCSV(string filenameToSave, object[] column1, object[] column2, object[] column3, object[] column4) {
			if (column1.Length != column2.Length || column1.Length != column3.Length || column1.Length != column4.Length) return;
			
			object[][] arr = new object[column1.Length][];

			int count = 1;
			for (int i = 0; i < column1.Length; i++)
			{
				arr[i] = new object[5] {
					count,
					column1[i],
					column2[i],
					column3[i],
					column4[i]
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}

		private static void exportCSV(string filenameToSave, float[] column1, float[] column2, float[] column3, float[] column4, string[] column5) {
			if (column1.Length != column2.Length || column1.Length != column3.Length || column1.Length != column4.Length || column1.Length != column5.Length) return;
			
			object[][] arr = new object[column1.Length][];

			int count = 1;
			for (int i = 0; i < column1.Length; i++)
			{
				arr[i] = new object[6] {
					count,
					column1[i],
					column2[i],
					column3[i],
					column4[i],
					column5[i],					
				};
				count++;
			};
			
			CSVWriter csv = new CSVWriter(filenameToSave);
			csv.Write(arr);
		}
		#endregion
		
		public static float[][] CreateSpectrogram(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
		{
			// overlap must be an integer smaller than the window size
			// half the windows size is quite normal
			//HanningWindow window = new HanningWindow();
			//double[] windowArray = window.GetWindow(fftWindowsSize);
			double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
			LomontFFT fft = new LomontFFT();
			int numberOfSamples = samples.Length;
			double seconds = numberOfSamples / sampleRate;
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap; 	// width of the segment - i.e. split the file into 78 time slots (numberOfSegments) and do analysis on each slot
			float[][] frames = new float[numberOfSegments][];
			double[] complexSignal = new double[2*fftWindowsSize]; 		// even - Re, odd - Img
			for (int i = 0; i < numberOfSegments; i++)
			{
				// apply Hanning Window
				for (int j = 0; j < fftWindowsSize; j++)
				{
					// add window with (4.0 / (fftWindowsSize - 1)
					complexSignal[2*j] = (double) ((4.0/(fftWindowsSize - 1)) * windowArray[j] * samples[i * fftOverlap + j]); 	// Weight by Hann Window
					//complexSignal[2*j] = (double) (windowArray[j] * samples[i * fftOverlap + j]); 	// Weight by Hann Window
					complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer (phase)
				}

				// FFT transform for gathering the spectrum
				fft.FFT(complexSignal, true);
				float[] band = new float[fftWindowsSize/2];
				for (int j = 0; j < fftWindowsSize/2; j++)
				{
					double re = complexSignal[2*j];
					double img = complexSignal[2*j + 1];
					band[j] = (float) Math.Sqrt(re*re + img*img);
				}
				frames[i] = band;
			}
			
			return frames;
		}

		// http://www.triplecorrelation.com/courses/fundsp/showspectrogram.html
		// R Kakarala, PhD
		// U C Berkeley Extesion
		// last rev: 5 Oct 2003
		// http://aug.ment.org/synthbilder/paper/
		public static float[][] CreateSpectrogram2(float[] y, double fs, int fftWindowsSize, int fftOverlap)
		{
			//Fs = 8192;                    % Sampling frequency
			//T = 1/Fs;                     % Sample time
			//L = 1000;                     % Length of signal
			//t = (0:L-1)*T;                % Time vector
			
			// compute the spectrogram
			// assume a 256 point sliding Hamming window, with an overlap of 128 samples
			// is applied to the data.
			int Ly = y.Length;
			// if the length < 256, add zeros to increase length to match window
			if (Ly < 256) {
				Array.Resize<float>(ref y, 256);
			}

			int fftWindowSize = 256;
			//int fftOverlap = 256;  // this, the max overlap, is adjusted below
			int num_segs = 1025;
			while (num_segs > 1024) {
				fftOverlap = fftOverlap - 10;
				double shift = Math.Abs(fftWindowSize - fftOverlap); // returns always positive value
				num_segs = (int) (1 + RepositoryGateway.RoundDown( (Ly-fftWindowSize) / shift, 0 ));
			}

			double[] window = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HAMMING, fftWindowsSize);

			// the following idea is from The DSP FIRST Toolbox, by McClellan, Shafer
			// and Yoder. Basically, it divides the input into segments, each of length
			// fftWindowSize and takes the windowed fft of each one
			LomontFFT fft = new LomontFFT();
			int width = (y.Length - fftWindowsSize)/fftOverlap; /*width of the image*/
			float[][] frames = new float[width][];
			double[] complexSignal = new double[2*fftWindowsSize]; /*even - Re, odd - Img*/
			for (int i = 0; i < width; i++)
			{
				// apply Hanning Window
				for (int j = 0; j < fftWindowsSize; j++)
				{
					complexSignal[2*j] = (double) (window[j] * y[i*fftOverlap + j]);
					complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer
				}

				// FFT transform for gathering the spectrum
				fft.FFT(complexSignal, true);
				float[] band = new float[fftWindowsSize/2 + 1];
				for (int j = 0; j < fftWindowsSize/2 + 1; j++)
				{
					double re = complexSignal[2*j];
					double img = complexSignal[2*j + 1];
					band[j] = (float) Math.Sqrt(re*re + img*img);
				}
				frames[i] = band;
			}

			//F = (0:(fftWindowSize / 2))/fftWindowsSize * fs;
			//T = ( fftWindowSize/2 + shift*(0:num_segs-1) ) / fs;
			// imagesc(T,F,20*log10(abs(B)+eps));
			// axis xy;
			// colormap(jet);
			// ylabel('frequency (Hz)'); xlabel('time (sec)'); title('Spectrogram (dB)');

			// plot time signal, trying to align time axes for both plots
			
			return frames;
		}
		
		public static void prepareAndDrawSpectrumAnalysis(RepositoryGateway repositoryGateway, String prefix, String fileName, float[][] spectrogramData, double sampleRate, int fftWindowsSize, int fftOverlap) {
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
			 * 20 log10 (mag) => 20/ ln(10) ln(mag)
			 * 
			 * 
			 * 
				int numberOfSamples = samples.Length;
				double seconds = numberOfSamples / sampleRate;
				int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap; 	// width of the segment - i.e. split the file into 78 time slots (numberOfSegments) and do analysis on each slot

				fftOverlap * numberOfSegments = numberOfSamples - fftWindowsSize
				numberOfSamples = (fftOverlap * numberOfSegments) + fftWindowsSize
			 */
			 	
			int numberOfSegments = spectrogramData.Length; // i.e. 78 images which containt a spectrum which is half the fftWindowsSize (2048)
			int spectrogramLength = spectrogramData[0].Length; // 1024 - half the fftWindowsSize (2048)
			double numberOfSamples = (fftOverlap * numberOfSegments) + fftWindowsSize;
			double seconds = numberOfSamples / sampleRate;		
						
			float[] m_mag = new float[spectrogramLength];
			float[] m_freq = new float[spectrogramLength];
			float[] m_freqV2 = new float[spectrogramLength];
			string[] m_time = new string[spectrogramLength];			
			for (int i = 0; i < spectrogramLength; i++)
			{
				m_mag[i] = ConvertAmplitudeToDB((float) spectrogramData[0][i], -120.0f, 18.0f);
				m_freq[i] = ( i + 1 ) * (float) sampleRate / fftWindowsSize;
				m_freqV2[i] = ConvertIndexToHz (i, spectrogramLength, sampleRate, fftWindowsSize);

				double sampleIndex = (numberOfSamples * i) / spectrogramLength; // since we are not iteration on the actual sample length, we need to convert spectrogram index to sample index
				m_time[i] = GetSampleTimeString( (int)sampleIndex, (int)sampleRate);
			}

			exportCSV ("c:\\" + prefix + "-spectrogram-mag-freq-freqV2-time.csv", spectrogramData[0], m_mag, m_freq, m_freqV2, m_time);

			repositoryGateway.drawSpectrumAnalysis(prefix + "SpectrumAnalysis", fileName, m_mag, m_freqV2);
		}
		
		public static void GetAudioInformation(string filename)
		{
			float lFrequency = 0;
			float lVolume = 0;
			float lPan = 0;
			
			int stream = Bass.BASS_StreamCreateFile(filename, 0L, 0L, BASSFlag.BASS_STREAM_DECODE);

			// the info members will contain most of it...
			BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(stream);

			if (Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, ref lVolume))
				System.Diagnostics.Debug.WriteLine("Volume: " + lVolume);
			
			if (Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_PAN, ref lPan))
				System.Diagnostics.Debug.WriteLine("Pan: " + lPan);

			if (Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, ref lFrequency))
				System.Diagnostics.Debug.WriteLine("Frequency: " + lFrequency);
			
			int nChannels = info.chans;
			System.Diagnostics.Debug.WriteLine("Channels: " + nChannels);

			int nSamplesPerSec = info.freq;
			System.Diagnostics.Debug.WriteLine("SamplesPerSec: " + nSamplesPerSec);
		}
		
		public static float[] ConvertRangeAndMainainRatioLog(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			float[] newValueArray = new float[oldValueArray.Length];
			
			// TODO: Addition of MIN_VALUE prevents log from returning minus infinity if value is zero
			float newRange = (newMax - newMin);
			float log_oldMin = Axis.flog10(Math.Abs(oldMin) + float.MinValue);
			float log_oldMax = Axis.flog10(oldMax + float.MinValue);
			float oldRange = (oldMax - oldMin);
			float log_oldRange = (log_oldMax - log_oldMin);
			float data_per_log_unit = newRange / log_oldRange;
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float log_oldValue = Axis.flog10(oldValueArray[x] + float.MinValue);
				float newValue = (((log_oldValue - log_oldMin) * newRange) / log_oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}
		
		public static double[] ConvertRangeAndMainainRatio(double[] oldValueArray, double oldMin, double oldMax, double newMin, double newMax) {
			double[] newValueArray = new double[oldValueArray.Length];
			double oldRange = (oldMax - oldMin);
			double newRange = (newMax - newMin);
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				double newValue = (((oldValueArray[x] - oldMin) * newRange) / oldRange) + newMin;
				newValueArray[x] = newValue;
			}
			
			return newValueArray;
		}
		
		public static float[] ConvertRangeAndMainainRatio(float[] oldValueArray, float oldMin, float oldMax, float newMin, float newMax) {
			float[] newValueArray = new float[oldValueArray.Length];
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			
			for(int x = 0; x < oldValueArray.Length; x++)
			{
				float newValue = (((oldValueArray[x] - oldMin) * newRange) / oldRange) + newMin;
				newValueArray[x] = newValue;
			}

			return newValueArray;
		}

		public static void ComputeMinAndMax(double[] data, out double min, out double max) {
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				if (data[x] > maxVal)
					maxVal = data[x];
				if (data[x] < minVal)
					minVal = data[x];
			}
			min = minVal;
			max = maxVal;
		}
		
		public static void ComputeMinAndMax(double[][] data, out double min, out double max) {
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal)
						maxVal = data[x][y];
					if (data[x][y] < minVal)
						minVal = data[x][y];
				}
			}
			min = minVal;
			max = maxVal;
		}

		public static void ComputeMinAndMax(float[] data, out float min, out float max) {
			// prepare the data:
			float maxVal = float.MinValue;
			float minVal = float.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				if (data[x] > maxVal)
					maxVal = data[x];
				if (data[x] < minVal)
					minVal = data[x];
			}
			min = minVal;
			max = maxVal;
		}
		
		public static void ComputeMinAndMax(float[][] data, out float min, out float max) {
			// prepare the data:
			float maxVal = float.MinValue;
			float minVal = float.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal)
						maxVal = data[x][y];
					if (data[x][y] < minVal)
						minVal = data[x][y];
				}
			}
			min = minVal;
			max = maxVal;
		}

		// look at this http://jvalentino2.tripod.com/dft/index.html
		public static float ConvertAmplitudeToDB(float amplitude, float MinDb, float MaxDb) {
			// db = 20 * log10( fft[index] );
			
			// Convert float to dB
			//float MinDb = -120.0f;
			//float MaxDb = 18.0f;

			// TODO: Addition of MIN_VALUE prevents log from returning minus infinity if mag is zero
			float db = 20 * (float) Math.Log10( (float) amplitude + float.MinValue);
			
			if (db < MinDb) db = MinDb;
			if (db > MaxDb) db = MaxDb;
			
			//float percentage = (db - MinDb) / (MaxDb - MinDb);
			
			return db;
		}
		
		public static float ConvertIndexToHz(int i, int numberOfSamples, double sampleRate, double fftWindowsSize) {
			// either
			// freq = index * samplerate / fftsize;
			// frequency = ( i + 1 ) * (float) sampleRate / fftWindowsSize;
			
			// or
			// ( i + 1 ) * ((sampleRate / 2) / numberOfSamples)		
			double nyquistFreq = sampleRate / 2;
			double firstFrequency = nyquistFreq / numberOfSamples;
			double frequency = firstFrequency * ( i + 1 );
			return (float) frequency;
		}
		
		public static double ConvertIndexToTime(double sampleRate, int numberOfSamples) {
			//T = ( fftWindowSize / 2 + shift * (0:num_segs - 1) ) / fs;
			double time = sampleRate / numberOfSamples;
			return time;
		}
		
		public static int GetSampleForTime(int msecs, int nSamplesPerSec)
		{
			double t = 1.0 / nSamplesPerSec;
			return (int)(msecs / 1000.0 / t);
		}

		public static int GetSampleTime(int nSample, int nSamplesPerSec)
		{
			return (int)(nSample * 1000.0 / nSamplesPerSec);
		}
		
		public static String GetSampleTimeString(int nSample, int nSamplesPerSec)
		{
			int ms = (int)(nSample * 1000.0 / nSamplesPerSec);
			return GetTimeString(ms, nSamplesPerSec);
		}

		public static String GetTimeString(int msecs, int nSamplesPerSec)
		{
			int s = msecs / 1000;
			int m = msecs / 60000;
			int h = msecs / 3600000;
			DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, h % 100, m % 60, s % 60, msecs % 1000);
			return String.Format("{0:HH:mm:ss.fff}", date);
		}
		
	}
	
}