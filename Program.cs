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
			
			// TODO: Implement Functionality Here
			RepositoryGateway repositoryGateway = new RepositoryGateway();

			TAG_INFO tag = repositoryGateway._proxy.GetTagInfoFromFile(fileName);
			
			FingerprintManager manager = new FingerprintManager();
			float[][] spectrogram = manager.CreateSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);
			repositoryGateway.writeImage("Spectrogram", fileName, spectrogram);

			System.Diagnostics.Debug.WriteLine("Spectrogram Length: " + spectrogram[0].Length);
            float[] m_mag = new float[spectrogram[0].Length + 1];       
			for (int i = 0; i < spectrogram[0].Length; i++)
            {
                // 20 log10(mag) => 20/ln(10) ln(mag)
                // Addition of MIN_VALUE prevents log from returning minus infinity if mag is zero
                m_mag[i] = 20 * (float) Math.Log10( (float) spectrogram[0][i] );
            }
            float[] m_freq = new float[spectrogram[0].Length + 1];
			repositoryGateway.drawSpectrum("TestSpectrogram", fileName, m_mag, m_freq);
						
			float[][] logSpectrogram = manager.CreateLogSpectrogram(repositoryGateway._proxy, fileName, RepositoryGateway.MILLISECONDS_TO_PROCESS, RepositoryGateway.MILLISECONDS_START);
			repositoryGateway.writeImage("LogSpectrogram", fileName, logSpectrogram);
			
			List<bool[]> dbFingers = manager.CreateFingerprints(logSpectrogram, repositoryGateway._createStride);
			
			List<string> musicFiles = Helper.GetMusicFiles(path, Program._musicFileFilters, true); //get music file names	
			List<Track> processedMusicFiles = repositoryGateway.ProcessFiles(musicFiles, null);
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
	
}