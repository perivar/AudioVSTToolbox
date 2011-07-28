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
			String path = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl";
			
			Console.WriteLine("Analyzer starting ...");
			
			RepositoryGateway repositoryGateway = new RepositoryGateway();
			FingerprintManager manager = new FingerprintManager();

			TAG_INFO tag = repositoryGateway._proxy.GetTagInfoFromFile(fileName);
			
			//read 5512 Hz, Mono, PCM, with a specific proxy
            float[] samples = repositoryGateway._proxy.ReadMonoFromFile(fileName, manager.SampleRate, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);     
			exportCSV (@"c:\test-samples.csv", samples);
        	repositoryGateway.drawWaveform("Waveform", fileName, samples);

            FingerprintManager.NormalizeInPlace(samples);
			exportCSV (@"c:\test-samples-normalized.csv", samples);
        	repositoryGateway.drawWaveform("Waveform-Normalized", fileName, samples);
			
			float[][] spectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START, false);
			repositoryGateway.writeImage("Spectrogram", fileName, spectrogram);
			exportCSV (@"c:\test-full-spectrogram-not-normalized.csv", spectrogram);

			System.Diagnostics.Debug.WriteLine("Spectrogram Length of first band: " + spectrogram[0].Length);
			exportCSV (@"c:\test-spectrogram.csv", spectrogram[0]);
			
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
			 */
	       	float[] m_mag = new float[spectrogram[0].Length + 1];       
			for (int i = 0; i < spectrogram[0].Length; i++)
            {	
                // 20 log10(mag) => 20/ln(10) ln(mag)
                // Addition of MIN_VALUE prevents log from returning minus infinity if mag is zero
                m_mag[i] = 20 * (float) Math.Log10( (float) spectrogram[0][i] );
            }
			exportCSV (@"c:\test-mag.csv", m_mag);
									
		    // Vector with frequencies for each bin number. Used
	        // in the graphing code (not in the analysis itself).
	        float[] m_freq = new float[m_mag.Length + 1];
			for ( int i = 0; i < m_mag.Length; i++ )
			{
				m_freq[i] = i * (float) manager.SampleRate / manager.WdftSize;
			}
			exportCSV (@"c:\test-freq.csv", m_freq);

			repositoryGateway.drawWaveform("Test2Spectrogram", fileName, spectrogram[0]);
			repositoryGateway.drawSpectrum("Test3Spectrogram", fileName, m_mag, m_freq);			
						
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

	}
	
}