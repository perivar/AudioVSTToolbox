/*
 * The goal of this project is to be able to take a wave sample and create a synth preset for u-he Zebra 2 synth from that
 * Date: 27.07.2011
 * Time: 12:13
 * test
 */
using System;
using System.IO;
using System.Collections.Generic;

using CommonUtils.Audio;
using Wave2Zebra2Preset.Fingerprinting;
using Wave2Zebra2Preset.DataAccess;
using Wave2Zebra2Preset.Model;
using Wave2Zebra2Preset.Fingerprinting.MathUtils;

using Lomont;

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

using Wave2Zebra2Preset.HermitGauges;

using CommonUtils;
using CommonUtils.FFT;

namespace Wave2Zebra2Preset
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
			//String fileName = @"G:\Cubase and Nuendo Projects\Music To Copy Learn\Britney Spears - Hold It Against Me\02 Hold It Against Me (Instrumental) 1.mp3";

			Console.WriteLine("Analyzer starting ...");
			
			RepositoryGateway repositoryGateway = new RepositoryGateway();
			FingerprintManager manager = new FingerprintManager();

			// VB6 FFT
			double sampleRate = 44100;// 44100  default 5512
			int fftWindowsSize = 4096; //4096  default 256*8 (2048) to 256*128 (32768), reccomended: 256*64 = 16384
			float fftOverlapPercentage = 95.0f; // number between 0 and 100
			int secondsToSample = 3; //15;
			float[] wavDataVB6 = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, secondsToSample*1000, 20*1000 );
			VB6Spectrogram vb6Spect = new VB6Spectrogram();
			vb6Spect.ComputeColorPalette();
			float[][] vb6Spectrogram = vb6Spect.Compute(wavDataVB6, sampleRate, fftWindowsSize, fftOverlapPercentage);
			//Export.exportCSV (@"c:\VB6Spectrogram-full.csv", vb6Spectrogram);
						
			// Exocortex.DSP FFT
			int numberOfSamples = wavDataVB6.Length; 
			fftOverlapPercentage = fftOverlapPercentage / 100;
			long ColSampleWidth = (long)(fftWindowsSize * (1 - fftOverlapPercentage));
			double fftOverlapSamples = fftWindowsSize * fftOverlapPercentage;
			long NumCols = numberOfSamples / ColSampleWidth;
			
			int fftOverlap = (int)((numberOfSamples - fftWindowsSize) / NumCols);
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			
			System.Console.Out.WriteLine(String.Format("EXO: fftWindowsSize: {0}, Overlap samples: {1:n2}.", fftWindowsSize, fftOverlap ));

			float[][] exoSpectrogram = AudioAnalyzer.CreateSpectrogramExocortex(wavDataVB6, sampleRate, fftWindowsSize, fftOverlap);
			repositoryGateway.drawSpectrogram3("Spectrogram", fileName, exoSpectrogram);
			repositoryGateway.drawColorGradient("ColorGradient", fileName);
			//Export.exportCSV (@"c:\exoSpectrogram-full.csv", exoSpectrogram);
						
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
		
		// http://www.triplecorrelation.com/courses/fundsp/showspectrogram.html
		// R Kakarala, PhD
		// U C Berkeley Extesion
		// last rev: 5 Oct 2003
		// see also: http://aug.ment.org/synthbilder/paper/
		public static float[][] CreateSpectrogram2(float[] y, double fs, int fftWindowsSize, int fftOverlap)
		{
			// fs = 8192;                    // Sampling frequency
			// T = 1/fs;                     // Sample time
			
			// compute the spectrogram
			// assume a 256 point sliding Hamming window, with an overlap of 128 samples
			// is applied to the data.
			int Ly = y.Length;
			// if the length < 256, add zeros to increase length to match window
			if (Ly < fftWindowsSize) {
				Array.Resize<float>(ref y, fftWindowsSize);
			}

			int L = fftWindowsSize;//256;
			int Nfft = fftWindowsSize;//256;
			int Noverlap = fftWindowsSize; //256;  // this, the max overlap, is adjusted below
			int num_segs = 1025;
			int shift = 0;
			while (num_segs > 1024) {
				Noverlap = Noverlap - 10;
				shift = Math.Abs( L - Noverlap);
				// num_segs = 1 + fix( (Ly-L)/shift );
				num_segs = (int) (1 + Fix( (Ly-L) / shift ));
			}
			
			// window = 0.54 - 0.46  *      cos (2     * pi * (0:L-1) / (L-1) );  // Hamming window
			// for (i = 0; i < fftWindowsSize; i++)
			//  window = (0.54 - 0.46 * Math.Cos(2 * Math.PI *      i / (fftWindowsSize - 1)));
			double[] window = FFTWindowFunctions.GetWindowFunction(FFTWindowFunctions.HAMMING, Nfft);
			LomontFFT fft = new LomontFFT();

			// the following idea is from The DSP FIRST Toolbox, by McClellan, Shafer
			// and Yoder. Basically, it divides the input into segments, each of length
			// L and takes the windowed fft of each one
			
			// B = zeros( Nfft/2+1, num_segs );
			// float[][] frames = new float[numberOfSegments][];
			// double[][] B = new double[Nfft/2+1][num_segs];     //- Pre-allocate the matrix
			float[][] B = new float[num_segs][];
			
			int nstart = 0;
			double[] ysegw = new double[2*Nfft]; 		// even - Re, odd - Img
			for (int iseg = 0; iseg < num_segs; iseg++) {
				nstart = 1 + iseg * shift;

				// apply Hanning Window
				// ysegw = window .* y( nstart:nstart + L-1);
				
				// Matlab Vector Functions:
				// v 	= [1   	 2 	  3]'
				// b 	= [2 	 4 	  6]'
				// v+b 	= [3 	 6 	  9]
				// v-b 	= [-1   -2   -3]
				// v*b' = [2     4    6
				//         4     8   12
				//         6    12   18]
				// v'*b = 28
				// v.*b = [2 	 8   18]
				// v./b = [0.5 0.5  0.5]
				
				for (int j = 0; j < Nfft; j++)
					//for (int j = nstart; j < nstart + L-1; j++)
				{
					//ysegw[2*j] = (double) (window[j] * y[i*fftOverlap + j]);
					ysegw[2*j] = (double) (window[j] * y[nstart + j]);
					ysegw[2*j + 1] = 0;  // need to clear out as fft modifies buffer
				}

				// FFT transform for gathering the spectrum
				// YF = fft( ysegw, Nfft );
				fft.FFT(ysegw, true);
				float[] YF = new float[Nfft/2 + 1];
				for (int j = 0; j < Nfft/2 + 1; j++)
				{
					double re = ysegw[2*j];
					double img = ysegw[2*j + 1];
					YF[j] = (float) Math.Sqrt(re*re + img*img);
				}
				//frames[i] = band;
				//B(:,iseg) = YF(1:Nfft/2+1);
				B[iseg] = YF;
			}
			
			// use imagesc -- image scaled to show the histogram in dB
			//F = (0:(Nfft/2))/Nfft * fs;
			//T = ( L/2 + shift*(0:num_segs-1) ) / fs;
			//subplot(2,1,1);
			// dB = 20 * log10 (abs(B) + eps);
			//imagesc(T,F,20*log10(abs(B)+eps)); axis xy; colormap(jet);
			//ylabel('frequency (Hz)'); xlabel('time (sec)'); title('Spectrogram (dB)');
			
			// plot time signal, trying to align time axes for both plots
			//maxsampleused = nstart+L-2;
			//subplot(2,1,2);
			//plot((0:maxsampleused)/fs,y(1:maxsampleused+1));
			//axis([min(T),max(T),min(y),max(y)]);
			//xlabel('time (sec)'); ylabel('signal');
			
			return B;
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
		
		// MATLAB:
		// B = fix(A) rounds the elements of A toward zero, resulting in an array of integers.
		// For complex A, the imaginary and real parts are rounded independently
		// if we have data with value :
		// X = [-1.9, -0.2	, 3.4	, 5.6, 7.0]
		// Y = [   1, 	 0	,   3	,   5,   7]
		public static int Fix(float a)
		{
			int sign = a < 0 ? -1 : 1;
			double dout = Math.Abs(a);
			double output = MathUtils.RoundDown(dout, 0);
			//float output = (float) (Math.Floor(a) * sign);
			return (int)output;
		}
		
	}
	
}