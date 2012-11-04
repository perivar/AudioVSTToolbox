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

using System.Drawing;
using System.Drawing.Imaging;

namespace Wave2Zebra2Preset
{
	public class MyColor {
		private Color rgbcolor;
		private HSLColor hslcolor;
		
		public MyColor(Color rgbcolor) {
			this.rgbcolor = rgbcolor;
			this.hslcolor = HSLColor.FromRGB(rgbcolor);
		}

		public MyColor(HSLColor hslcolor) {
			this.hslcolor = hslcolor;
			this.rgbcolor = hslcolor.ToRGB();
		}
		
		public HSLColor HSLColor {
			get { return hslcolor; }
		}

		public Color Color {
			get { return rgbcolor; }
		}
		
		public override string ToString()
		{
			return ColorUtils.ColorToLong(rgbcolor) + ";" +
				rgbcolor.A + ";" +
				rgbcolor.R + ";" +
				rgbcolor.G + ";" +
				rgbcolor.B + ";" +
				hslcolor.Hue + ";" +
				hslcolor.Saturation + ";" +
				hslcolor.Luminosity;
		}
	}
	
	public enum ColorPaletteType {
		REWColorPalette = 1,
		SOXColorPalette = 2
	}
	
	// Color palette URLs:
	// http://stackoverflow.com/questions/3097753/tinting-towards-or-away-from-a-hue-by-a-certain-percentage
	// http://stackoverflow.com/questions/2593832/how-to-interpolate-hue-values-in-hsv-colour-space
	// http://www.stuartdenman.com/improved-color-blending/
	// http://devmag.org.za/2012/07/29/how-to-choose-colours-procedurally-algorithms/
	// http://stackoverflow.com/questions/340209/generate-colors-between-red-and-green-for-a-power-meter
	// http://tabs2.gerg.tamu.edu/gmt/GMT_Docs/node214.html
	// http://geography.uoregon.edu/datagraphics/color_scales.htm (Color Schemes Appropriate for Scientific Data Graphics)
	// https://github.com/jolby/colors/blob/master/src/com/evocomputing/colors/palettes/core.clj
	// https://github.com/jolby/colors/blob/master/src/com/evocomputing/colors/palettes/color_brewer.clj
	
	// Wavelet, Set and Hash etc
	// http://blogs.msdn.com/b/spt/archive/2008/06/10/set-similarity-and-min-hash.aspx (JaccardSimilarity ++)
	// http://laplacian.wordpress.com/2009/01/10/how-shazam-works/
	// http://www.whydomath.org/node/wavlets/index.html
	// http://www.whydomath.org/node/wavlets/hwt.html
	// http://www.codeproject.com/Articles/206507/Duplicates-detector-via-audio-fingerprinting
	
	class Program
	{
		///   Music file filters
		/// </summary>
		private static readonly string[] _musicFileFilters = new[] {"*.mp3", "*.ogg", "*.flac", "*.wav"};
		
		public static void SaveColorbar(String filenameToSave) {
			int width = 33;
			int height = 305;
			System.Console.Out.WriteLine("Writing " + filenameToSave);

			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			Pen pen = new Pen(Color.Black, 1.0f);
			
			// start with red (0, 1, 1)
			float h = 0;
			float s = 1.0f;
			float v = 1.0f;
			for(int y = 0; y <= height; y++)
			{
				// when yellow
				if (y > 0 && y <= 58) {
					h = y;
				} else  if (y > 59 && y <= 118) {
					// decrease v from 0.99 to 0.70
					v = v - (0.3f / 60);
					h = y;
				} else if (y > 118 && y < 240) {
					v = 0.69f;
					h = y;
				} else  if (y >= 240) {
					// decrease v from 0.68 to 0.48
					v = v - (0.2f / 60);
					
					// incease h slowly from 240 - 270
					// for y = 240 - 305
					h = h + (30.0f / 65);
				}
				
				Color c = ColorUtils.AhsbToArgb(255, h, s, v);
				
				pen.Color = c;
				g.DrawLine(pen, 1, y, width, y);
			}
			
			png.Save(filenameToSave);
		}
		
		public static void ReadColorPaletteBar(String filenameToRead, String csvExport) {
			Bitmap colorimage = new Bitmap(filenameToRead);
			
			List<MyColor> pixels = new List<MyColor>();
			for (int y = 0; y < colorimage.Height; y++)
			{
				Color pixel = colorimage.GetPixel(0, y);
				MyColor pixelcolor = new MyColor(pixel);
				pixels.Add(pixelcolor);
			}
			Export.exportCSV(csvExport, pixels.ToArray(), pixels.Count);
		}

		/// <summary>
		/// Get a Color based on a input value between 0 and 100
		/// </summary>
		/// <param name="value">the input value between 0 and 100</param>
		/// <returns>MyColor</returns>
		public static MyColor GetREWColorPaletteValue(float value) {
			
			float h = 0;
			float s = 1;
			float l = 0;
			
			// determine h, s and l values
			// based on a input value between 0 and 100
			if (value < 20) {
				h = 0.05f * value;
				l = 0.5f;
			} else if (value >= 20 && value < 40) {
				h = 0.05f * value;
				l = -0.0075f * value + 0.6499f;
			} else if (value >= 40 && value < 80) {
				h = 0.05f * value;
				l = 0.3490196f;
			} else if (value >= 80) {
				h = 0.0244f * value + 2.0189f;
				l = -0.0053f * value + 0.7699f;
			}
			HSLColor hslcolor = new HSLColor(h, s, l);
			MyColor mycolor = new MyColor(hslcolor);
			return mycolor;
		}
		
		/// <summary>
		/// Get a Color based on a input value between 0 and 100
		/// </summary>
		/// <param name="value">the input value between 0 and 100</param>
		/// <returns>MyColor</returns>
		public static MyColor GetSOXColorPaletteValue(float value) {
			
			float h = 0;
			float s = 1;
			float l = 0;
			
			// determine h, s and l values
			// based on a input value between 0 and 100
			if (value < 20) {
				h = 0.05f * value;
				l = 0.5f;
			} else if (value >= 20 && value < 40) {
				h = 0.05f * value;
				l = -0.0075f * value + 0.6499f;
			} else if (value >= 40 && value < 80) {
				h = 0.05f * value;
				l = 0.3490196f;
			} else if (value >= 80) {
				h = 0.0244f * value + 2.0189f;
				l = -0.0053f * value + 0.7699f;
			}
			HSLColor hslcolor = new HSLColor(h, s, l);
			MyColor mycolor = new MyColor(hslcolor);
			return mycolor;
		}
		
		public static void SaveColorPaletteBar(string imageToSave, string csvToSave, ColorPaletteType type) {

			int width = 40;
			int height = 400;
			
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			Pen pen = new Pen(Color.Black, 1.0f);
			
			MyColor mycolor;
			List<MyColor> pixels = new List<MyColor>();
			for (float i = 0; i < 100; i = i + 0.25f) {
				switch (type) {
					case ColorPaletteType.REWColorPalette:
						mycolor = GetREWColorPaletteValue(i);
						break;
					case ColorPaletteType.SOXColorPalette:
						mycolor = GetSOXColorPaletteValue(i);
						break;
					default:
						mycolor = GetREWColorPaletteValue(i);
						break;
				}
				pixels.Add(mycolor);
				pen.Color = mycolor.Color;
				g.DrawLine(pen, 0, i*4, width, i*4);
			}
			png.Save(imageToSave);
			Export.exportCSV(csvToSave, pixels.ToArray(), pixels.Count);
		}
		
		public static void Main(string[] args)
		{
			string filenameToSave = "c:\\colorbar2.png";
			string csvToSave = "c:\\colorbar2.csv";
			SaveColorPaletteBar(filenameToSave, csvToSave, ColorPaletteType.SOXColorPalette);
			//string filenameToRead = @"C:\Users\perivar.nerseth\SkyDrive\Temp\soundforge_colorbar.png";
			//string filenameToRead = @"C:\Users\perivar.nerseth\SkyDrive\Temp\rew_colorbar.png";
			//string filenameToRead = @"C:\Users\perivar.nerseth\SkyDrive\Temp\sox_colorbar.png";
			//ReadColorPaletteBar(filenameToRead, "c:\\test.csv");
			
			return;
			
			String fileName = @"C:\Users\perivar.nerseth\Music\Sleep Away.mp3";
			//String fileName = @"C:\Users\perivar.nerseth\Music\Sine-500hz-60sec.wav";
			//String fileName = @"G:\Cubase and Nuendo Projects\Music To Copy Learn\Britney Spears - Hold It Against Me\02 Hold It Against Me (Instrumental) 1.mp3";

			Console.WriteLine("Analyzer starting ...");
			
			RepositoryGateway repositoryGateway = new RepositoryGateway();
			FingerprintManager manager = new FingerprintManager();

			// VB6 FFT
			double sampleRate = 44100;// 44100  default 5512
			int fftWindowsSize = 4096; //4096  default 256*8 (2048) to 256*128 (32768), reccomended: 256*64 = 16384
			float fftOverlapPercentage = 50.0f; // number between 0 and 100
			int secondsToSample = 15; //15;
			float[] wavDataVB6 = repositoryGateway._proxy.ReadMonoFromFile(fileName, (int) sampleRate, secondsToSample*1000, 20*1000 );
			VB6Spectrogram vb6Spect = new VB6Spectrogram();
			//vb6Spect.ComputeColorPalette();
			//float[][] vb6Spectrogram = vb6Spect.Compute(wavDataVB6, sampleRate, fftWindowsSize, fftOverlapPercentage);
			//Export.exportCSV (@"c:\VB6Spectrogram-full.csv", vb6Spectrogram);
			
			// Exocortex.DSP FFT
			int numberOfSamples = wavDataVB6.Length;
			fftOverlapPercentage = fftOverlapPercentage / 100;
			long ColSampleWidth = (long)(fftWindowsSize * (1 - fftOverlapPercentage));
			double fftOverlapSamples = fftWindowsSize * fftOverlapPercentage;
			long NumCols = numberOfSamples / ColSampleWidth;
			
			int fftOverlap = (int)((numberOfSamples - fftWindowsSize) / NumCols);
			int numberOfSegments = (numberOfSamples - fftWindowsSize)/fftOverlap;
			
			//System.Console.Out.WriteLine(String.Format("EXO: fftWindowsSize: {0}, Overlap samples: {1:n2}.", fftWindowsSize, fftOverlap ));

			float[][] exoSpectrogram = AudioAnalyzer.CreateSpectrogramExocortex(wavDataVB6, sampleRate, fftWindowsSize, fftOverlap);
			//float[][] exoSpectrogram = AudioAnalyzer.CreateSpectrogramLomont(wavDataVB6, sampleRate, fftWindowsSize, fftOverlap);
			//repositoryGateway.drawSpectrogram1("Spectrogram1", fileName, exoSpectrogram);
			repositoryGateway.drawSpectrogram2("Spectrogram2", fileName, exoSpectrogram, sampleRate, numberOfSamples, fftWindowsSize);
			//repositoryGateway.drawSpectrogram3("Spectrogram3", fileName, exoSpectrogram);
			//repositoryGateway.drawSpectrogram4("Spectrogram4", fileName, exoSpectrogram);
			//Export.exportCSV (@"c:\exoSpectrogram-full.csv", exoSpectrogram);
			
			//ColorUtils.drawColorGradient(@"C:\", "ColorGradient.png", true);
			//ColorUtils.drawColorGradient(@"C:\", "ColorGradient.png", false);
			
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