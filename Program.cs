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
			
			//String fileName = @"C:\Users\perivar.nerseth\Music\Per Ivar Only Girl\01. Only Girl (In The World).mp3";
			//String fileName = @"C:\Users\perivar.nerseth\Music\Sine-500hz-60sec.wav";
			String fileName = @"G:\Cubase and Nuendo Projects\Music To Copy Learn\Britney Spears - Hold It Against Me\02 Hold It Against Me (Instrumental) 1.mp3";
			
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
        	double sampleRate = 44100;// 44100  default 5512 
			int fftWindowsSize = 2048; //16384  default 256*8 (2048) to 256*128 (32768), reccomended: 256*64 = 16384
			int fftOverlap = 64;
			float[] wavData = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, 15*1000, 20*1000 );
			short[] shortData = new short[wavData.Length];
			for (int z = 0; z < wavData.Length; z++) {
				float f = wavData[z];
				double d = f * 32768.0;
				short s = (short) d;
				shortData[z] = s; 
			}
			analyser.SampleRate = (int) sampleRate;
			analyser.ProcessAudio(shortData);
			return;
			float[][] lomontSpectrogram = CreateSpectrogram(wavData, sampleRate, fftWindowsSize, fftOverlap);
			repositoryGateway.drawSpectrogram2("LomontSpectrum", fileName, lomontSpectrogram);
			//exportCSV (@"c:\LomontSpectrogram-full-not-normalized.csv", lomontSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Lomont", fileName, lomontSpectrogram, sampleRate, fftWindowsSize);

			// draw waveform
			exportCSV (String.Format("c:\\{0}-samples-{1}-{2}.csv", System.IO.Path.GetFileNameWithoutExtension(fileName), sampleRate, fftWindowsSize), wavData);
			repositoryGateway.drawWaveform("Waveform", fileName, wavData);
            FingerprintManager.NormalizeInPlace(wavData);
			exportCSV (String.Format("c:\\{0}-samples-normalized-{1}-{2}.csv", System.IO.Path.GetFileNameWithoutExtension(fileName), sampleRate, fftWindowsSize), wavData);
        	repositoryGateway.drawWaveform("Waveform-Normalized", fileName, wavData);
	
        	// Exocortex.DSP FFT
			// read 5512 Hz, Mono, PCM, with a specific proxy
        	float[][] exoSpectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START, false);
			repositoryGateway.drawSpectrogram("ExoSpectrum", fileName, exoSpectrogram);
			//exportCSV (@"c:\ExoSpectrogram-full-not-normalized.csv", exoSpectrogram);
			prepareAndDrawSpectrumAnalysis(repositoryGateway, "Exo", fileName, exoSpectrogram, manager.SampleRate, manager.WdftSize);
			
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
		#endregion
		
		public static float[][] CreateSpectrogram(float[] samples, double sampleRate, int fftWindowsSize, int fftOverlap)
        {
			HanningWindow window = new HanningWindow();
        	//double[] windowArray = window.GetWindow(fftWindowsSize);
        	double[] windowArray = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HANNING, fftWindowsSize);
        	LomontFFT fft = new LomontFFT();

            int width = (samples.Length - fftWindowsSize)/fftOverlap; /*width of the image*/
            float[][] frames = new float[width][];
            double[] complexSignal = new double[2*fftWindowsSize]; /*even - Re, odd - Img*/
            for (int i = 0; i < width; i++)
            {
                // apply Hanning Window
                for (int j = 0; j < fftWindowsSize; j++)
                {
                	// add window with (4.0 / (fftWindowsSize - 1)
                    complexSignal[2*j] = (double) ((4.0/(fftWindowsSize - 1)) * windowArray[j]*samples[i*fftOverlap + j]); /*Weight by Hann Window*/
                    //complexSignal[2*j] = (double) (windowArray[j]*samples[i*fftOverlap + j]); /*Weight by Hann Window*/
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
            
            return frames;
        }
		
		public static void prepareAndDrawSpectrumAnalysis(RepositoryGateway repositoryGateway, String prefix, String fileName, float[][] spectrogramData, double sampleRate, int fftWindowsSize) {
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
                m_freq[i] = ( i + 1 ) * (float) sampleRate / fftWindowsSize;
            }
			exportCSV ("c:\\" + prefix + "-spectrogram-mag-freq.csv", spectrogramData[0], m_mag, m_freq);

			repositoryGateway.drawSpectrumAnalysis(prefix + "SpectrumAnalysis", fileName, m_mag, m_freq);						
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
	}
	
}