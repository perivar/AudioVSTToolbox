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
using Wave2ZebraSynth.Fingerprinting.MathUtils;

using Un4seen.Bass.AddOn.Tags;
using Lomont;

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

			// extract tags			
			TAG_INFO tag = repositoryGateway._proxy.GetTagInfoFromFile(fileName);
				
			// read 5512 Hz, Mono, PCM, with a specific proxy
            float[] samples = repositoryGateway._proxy.ReadMonoFromFile(fileName, manager.SampleRate, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);     
			exportCSV (@"c:\samples.csv", samples);
        	repositoryGateway.drawWaveform("Waveform", fileName, samples, true);

            FingerprintManager.NormalizeInPlace(samples);
			exportCSV (@"c:\samples-normalized.csv", samples);
        	repositoryGateway.drawWaveform("Waveform-Normalized", fileName, samples, true);
			
        	// Lomont FFT
        	double sampleRate = 44100;//5512; // 44100; 
			int fftSize = 16384;//2048; //8192 * 10; or 16384;
			int fftOverlap = 64;
			float[] wavData = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, 2*1000, 20*1000 );
			float[][] lomontSpectrogram = CreateSpectrogram(wavData, sampleRate, fftSize, fftOverlap);
			//repositoryGateway.writeImage("LomontSpectrogram", fileName, lomontSpectrogram);
			//exportCSV (@"c:\LomontSpectrogram-full-not-normalized.csv", lomontSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Lomont", fileName, lomontSpectrogram, sampleRate, fftSize);
	
        	// Exocortex.DSP
			float[][] exoSpectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START, false);
			//repositoryGateway.writeImage("ExoSpectrogram", fileName, exoSpectrogram);
			exportCSV (@"c:\ExoSpectrogram-full-not-normalized.csv", exoSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Exo", fileName, exoSpectrogram, manager.SampleRate, manager.WdftSize);
			
			/*			
			float[][] logSpectrogram = manager.CreateLogSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);
			repositoryGateway.writeImage("LogSpectrogram", fileName, logSpectrogram);
			
			List<bool[]> dbFingers = manager.CreateFingerprints(logSpectrogram, repositoryGateway._createStride);
			
			List<string> musicFiles = Helper.GetMusicFiles(path, Program._musicFileFilters, true); //get music file names	
			List<Track> processedMusicFiles = repositoryGateway.ProcessFiles(musicFiles, null);
			*/
			
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
		
		public static float[][] CreateSpectrogram(float[] samples, double sampleRate, int fftSize, int fftOverlap)
        {
			HanningWindow window = new HanningWindow();
        	double[] windowArray = window.GetWindow(fftSize);
        	LomontFFT fft = new LomontFFT();

            int width = (samples.Length - fftSize)/fftOverlap; /*width of the image*/
            float[][] frames = new float[width][];
            double[] complexSignal = new double[2*fftSize]; /*even - Re, odd - Img*/
            for (int i = 0; i < width; i++)
            {
                // apply Hanning Window
                for (int j = 0; j < fftSize; j++)
                {
                    complexSignal[2*j] = (double) ((4.0/(fftSize - 1)) * windowArray[j]*samples[i*fftOverlap + j]); /*Weight by Hann Window*/
                    //complexSignal[2*j] = (double) (windowArray[j]*samples[i*fftOverlap + j]); /*Weight by Hann Window*/
                    complexSignal[2*j + 1] = 0;  // need to clear out as fft modifies buffer
                }

                // FFT transform for gathering the spectrum
                fft.FFT(complexSignal, true);
                float[] band = new float[fftSize/2 + 1];
                for (int j = 0; j < fftSize/2 + 1; j++)
                {
                    double re = complexSignal[2*j];
                    double img = complexSignal[2*j + 1]; 
                    band[j] = (float) Math.Sqrt(re*re + img*img);
                }
                frames[i] = band;
            }
            
            return frames;
        }
		
		public static void prepareAndDrawSpectrumAnalysis(RepositoryGateway repositoryGateway, String prefix, String fileName, float[][] spectrogramData, double sampleRate, int fftSize) {
			/*
			 * freq = index * samplerate / fftsize;
			 * db = 20 * log10(fft[index]);
             * 20 log10 (mag) => 20/ ln(10) ln(mag)
			 */
			int spectrogramLength = spectrogramData[0].Length;
	       	float[] m_mag = new float[spectrogramLength];       
	        float[] m_freq = new float[spectrogramLength];
			for (int i = 0; i < spectrogramLength; i++)
            {	
                // TODO: Addition of MIN_VALUE prevents log from returning minus infinity if mag is zero
                // c# int.MinValue ?
                m_mag[i] = 20 * (float) Math.Log10( (float) spectrogramData[0][i] );
                m_freq[i] = ( i + 1 ) * (float) sampleRate / fftSize;
            }
			exportCSV ("c:\\" + prefix + "-spectrogram-mag-freq.csv", spectrogramData[0], m_mag, m_freq);

			repositoryGateway.drawSpectrum(prefix + "SpectrumAnalysis", fileName, m_mag, m_freq);						
		}
		
	}
	
}