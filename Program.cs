/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 27.07.2011
 * Time: 12:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using Wave2ZebraSynth.Audio;
using Wave2ZebraSynth.Fingerprinting;
using Wave2ZebraSynth.DataAccess;
using Wave2ZebraSynth.Model;
using Un4seen.Bass.AddOn.Tags;

namespace Wave2ZebraSynth
{
	class Program
	{
		
        ///   Music file filters
        /// </summary>
        private static readonly string[] _musicFileFilters = new[] {"*.mp3", "*.ogg", "*.flac", "*.wav"};	
        
		public static void Main(string[] args)
		{
			String fileName = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl\01. Only Girl (In The World).mp3";
			//String fileName = @"C:\Users\perivar.nerseth\Music\Sine-500hz-60sec.wav";
			
			String path = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl";
			
			Console.WriteLine("Analyzer starting ...");
			
			RepositoryGateway repositoryGateway = new RepositoryGateway();
			FingerprintManager manager = new FingerprintManager();
			//manager.SampleRate = 44100;
			//manager.WdftSize = 8192 * 10; // 16384;
	
			TAG_INFO tag = repositoryGateway._proxy.GetTagInfoFromFile(fileName);
				
			//read 5512 Hz, Mono, PCM, with a specific proxy
            float[] samples = repositoryGateway._proxy.ReadMonoFromFile(fileName, manager.SampleRate, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);     
			exportCSV (@"c:\samples.csv", samples);
        	repositoryGateway.drawWaveform("Waveform", fileName, samples, true);

            FingerprintManager.NormalizeInPlace(samples);
			exportCSV (@"c:\samples-normalized.csv", samples);
        	repositoryGateway.drawWaveform("Waveform-Normalized", fileName, samples, true);
			
			float[][] spectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START, false);
			repositoryGateway.writeImage("Spectrogram", fileName, spectrogram);
			//exportCSV (@"c:\spectrogram-full-not-normalized.csv", spectrogram);

			//System.Diagnostics.Debug.WriteLine("Spectrogram Length of first band: " + spectrogram[0].Length);
			//exportCSV (@"c:\spectrogram.csv", spectrogram[0]);
			
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
             * 20 log10 (mag) => 20/ ln(10) ln(mag)
			 */
			int spectrogramLength = spectrogram[0].Length;
	       	float[] m_mag = new float[spectrogramLength];       
	        float[] m_freq = new float[spectrogramLength];
			for (int i = 0; i < spectrogramLength; i++)
            {	
                // Addition of MIN_VALUE prevents log from returning minus infinity if mag is zero
                // int.MinValue
                m_mag[i] = 20 * (float) Math.Log10( (float) spectrogram[0][i] );
                m_freq[i] = ( i + 1 ) * (float) manager.SampleRate / manager.WdftSize;
            }
			//exportCSV (@"c:\mag.csv", m_mag);
			//exportCSV (@"c:\freq.csv", m_freq);
			exportCSV (@"c:\spectrogram-mag-freq.csv", spectrogram[0], m_mag, m_freq);

			repositoryGateway.drawSpectrum("Spectrogram", fileName, m_mag, m_freq);			
						
			float[][] logSpectrogram = manager.CreateLogSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);
			repositoryGateway.writeImage("LogSpectrogram", fileName, logSpectrogram);
			
			List<bool[]> dbFingers = manager.CreateFingerprints(logSpectrogram, repositoryGateway._createStride);
			
			List<string> musicFiles = Helper.GetMusicFiles(path, Program._musicFileFilters, true); //get music file names	
			List<Track> processedMusicFiles = repositoryGateway.ProcessFiles(musicFiles, null);
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
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

		
	}
	
}