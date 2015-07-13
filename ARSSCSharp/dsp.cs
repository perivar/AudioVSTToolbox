using System;
using CommonUtils; // for MathUtils
using fftwlib;

namespace ARSS
{
	// Papers:
	// Spectral Analysis, Editing, and Resynthesis: Methods and Applications
	// http://www.klingbeil.com/data/Klingbeil_Dissertation_web.pdf

	// Spectral Envelopes in Sound Analysis and Synthesis
	// http://articles.ircam.fr/textes/Schwarz98a/index.pdf

	// Pitch Shifting Using The Fourier Transform
	// http://www.dspdimension.com/admin/pitch-shifting-using-the-ft/
	public static class DSP
	{
		public static double PI = Math.PI;
		public static double LOGBASE = 2; // Base for log() operations. Anything other than 2 isn't really supported
		public static double LOOP_SIZE_SEC = 10.0; // size of the noise loops in seconds
		public static int BMSQ_LUT_SIZE  = 16000; // defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached
		public const double TRANSITION_BW_SYNT = 16.0;
		
		public static int clockA;
		
		public enum FFTMethod : int {
			DFT = 0,
			IDFT = 1,
			DHT = 2
		}

		/// <summary>
		/// Perform the Fast Fourier Transform utilisizing the FFTW library
		/// </summary>
		/// <param name="in">Input Signal</param>
		/// <param name="out">Output Signal</param>
		/// <param name="N">N</param>
		/// <param name="method">FFT Method (DFT, IDFT, DHT)</param>
		public static void FFT(ref double[] @in, ref double[] @out, int N, FFTMethod method)
		{
			var complexInput = new fftw_complexarray(@in);
			var complexOutput = new fftw_complexarray(@out);
			
			switch(method) {
				case FFTMethod.DFT:
					// fftw_kind.R2HC: input is expected to be real while output is returned in the halfcomplex format
					fftw_plan fft = fftw_plan.r2r_1d(N, complexInput, complexOutput, fftw_kind.R2HC, fftw_flags.Estimate);
					fft.Execute();
					@out = complexOutput.Values;
					
					// free up memory
					fft = null;
					break;
				case FFTMethod.IDFT:
					// fftw_kind.HC2R: input is expected to be halfcomplex format while output is returned as real
					fftw_plan ifft = fftw_plan.r2r_1d(N, complexInput, complexOutput, fftw_kind.HC2R, fftw_flags.Estimate);
					ifft.Execute();
					//@out = complexOutput.ValuesDividedByN; // dividing by N gives the correct scale
					@out = complexOutput.Values;

					// free up memory
					ifft = null;
					break;
				case FFTMethod.DHT:
					fftw_plan dht = fftw_plan.r2r_1d(N, complexInput, complexOutput, fftw_kind.DHT, fftw_flags.Estimate);
					dht.Execute();
					@out = complexOutput.Values;

					// free up memory
					dht = null;
					break;
			}

			// free up memory
			complexInput = null;
			complexOutput = null;
			GC.Collect();
		}

		/// <summary>
		/// Normalises a signal to the +/-ratio range
		/// </summary>
		/// <param name="s">Samples</param>
		public static void Normalize(ref double[][] s)
		{
			MathUtils.Normalize(ref s);
		}
		
		/// <summary>
		/// Normalises a signal to the +/-ratio range
		/// </summary>
		/// <param name="s">Samples</param>
		public static void Normalize(ref double[] s)
		{
			MathUtils.Normalize(ref s);
		}
		
		/// <summary>
		/// Create Frequency Table (table with doubles between 0 and 1)
		/// To get Herz just multiply with the samplerate
		/// </summary>
		/// <param name="basefreq">Minimum frequency (double between 0 and 1)</param>
		/// <param name="bands">Number of frequency bands</param>
		/// <param name="bandsperoctave">Frequency resolution in Bands Per Octave</param>
		/// <returns>Frequency Array</returns>
		public static double[] FrequencyArray(double basefreq, int bands, double bandsperoctave)
		{
			var freq = new double[bands];
			double maxfreq;

			if (LOGBASE == 1.0) {
				maxfreq = bandsperoctave; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
			} else {
				maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bandsperoctave));
			}

			for (int i = 0; i < bands; i++) {
				freq[i] = LogPositionToFrequency((double) i/(double)(bands-1), basefreq, maxfreq); // band's central freq
			}
			
			if (LogPositionToFrequency((double) bands / (double)(bands-1), basefreq, maxfreq) > 0.5) {
				// TODO change sampling rate instead
				Console.Write("Warning: Upper frequency limit above Nyquist frequency\n");
			}

			return freq;
		}
		
		/// <summary>
		/// Downsampling of the signal by a Blackman function
		/// </summary>
		/// <param name="in">Signal</param>
		/// <param name="inputLength">Mi is the original signal's length</param>
		/// <param name="outputLength">Mo is the output signal's length</param>
		/// <returns>Downsampled Signal</returns>
		public static double[] BlackmanDownsampling(double[] @in, int inputLength, int outputLength)
		{
			var @out = new double[outputLength];

			double pos_in; 				// position in the original signal
			double ratio; 				// scaling ratio (> 1.0)
			double ratio_i; 			// ratio^-1
			double coef; 				// Blackman coefficient
			double coef_sum; 			// sum of coefficients, used for weighting

			// Mi is the original signal's length
			// Mo is the output signal's length
			ratio = (double) inputLength/outputLength;
			ratio_i = 1.0/ratio;

			for (int i = 0; i < outputLength; i++)
			{
				pos_in = (double) i * ratio;

				coef_sum = 0;

				for (int j = Util.RoundUpToClosestInt(pos_in - ratio); j<=pos_in + ratio; j++)
				{
					if (j >= 0 && j < inputLength) // if the read sample is within bounds
					{
						double x = j - pos_in + ratio; // calculate position within the Blackman function
						coef = 0.42 - 0.5 * Math.Cos(PI * x * ratio_i) + 0.08 * Math.Cos(2 *PI * x * ratio_i);
						coef_sum += coef;
						@out[i] += @in[j] * coef; // convolve
					}
				}

				@out[i] /= coef_sum;
			}

			return @out;
		}
		
		/// <summary>
		/// Blackman Square look-up table generator
		/// </summary>
		/// <param name="size">Size</param>
		/// <returns>Blackman Square look-up table</returns>
		public static double[] BlackmanSquareLookupTable(ref int size)
		{
			double coef; // Blackman square final coefficient
			double bar = PI * (3.0 / (double) size) * (1.0/1.5);
			double foo;

			const double f1 = -0.6595044010905501; // Blackman Square coefficients
			const double f2 = 0.1601741366715479;
			const double f4 = -0.0010709178680006;
			const double f5 = 0.0001450093579222;
			const double f7 = 0.0001008528049040;
			const double f8 = 0.0000653092892874;
			const double f10 = 0.0000293385615146;
			const double f11 = 0.0000205351559060;
			const double f13 = 0.0000108567682890;
			const double f14 = 0.0000081549460136;
			const double f16 = 0.0000048519309366;
			const double f17 = 0.0000038284344102;
			const double f19 = 0.0000024753630724;

			size++; // allows to read value 3.0

			// LUT = look up table
			var lut = new double[size];
			
			for (int i = 0; i < size; i++) {
				foo = (double) i * bar;
				coef = 0;

				coef += Math.Cos(foo) * f1 - f1;
				coef += Math.Cos(2.0 * foo) * f2 - f2;
				coef += Math.Cos(4.0 * foo) * f4 - f4;
				coef += Math.Cos(5.0 * foo) * f5 - f5;
				coef += Math.Cos(7.0 * foo) * f7 - f7;
				coef += Math.Cos(8.0 * foo) * f8 - f8;
				coef += Math.Cos(10.0 * foo) * f10 - f10;
				coef += Math.Cos(11.0 * foo) * f11 - f11;
				coef += Math.Cos(13.0 * foo) * f13 - f13;
				coef += Math.Cos(14.0 * foo) * f14 - f14;
				coef += Math.Cos(16.0 * foo) * f16 - f16;
				coef += Math.Cos(17.0 * foo) * f17 - f17;
				coef += Math.Cos(19.0 * foo) * f19 - f19;

				lut[i] = coef;
			}

			return lut;
		}
		
		/// <summary>
		/// Interpolation based on an estimate of the Blackman Square function,
		/// which is a Blackman function convolved with a square.
		/// It's like smoothing the result of a nearest neighbour interpolation
		/// with a Blackman FIR
		/// </summary>
		/// <param name="in">Signal in</param>
		/// <param name="out">Signal out</param>
		/// <param name="inputLength">Mi is the original signal's length</param>
		/// <param name="outputLength">Mo is the output signal's length</param>
		/// <param name="lut">Blackman Square Lookup Table</param>
		/// <param name="lut_size">Defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached</param>
		public static void BlackmanSquareInterpolation(ref double[] @in, ref double[] @out, ref int inputLength, ref int outputLength, ref double[] lut, int lut_size)
		{
			double pos_in; // position in the original signal
			double x; // position of the iterator in the blackman_square(x) formula
			double ratio; // scaling ratio (> 1.0)
			double ratio_i; // ratio^-1
			double coef; // Blackman square final coefficient
			double pos_lut; // Index on the look-up table
			int pos_luti = 0; // Integer index on the look-up table
			double mod_pos; // modulo of the position on the look-up table
			double y0; // values of the two closest values on the LUT
			double y1;
			double foo = (double) lut_size / 3.0;
			int j_start = 0; // boundary values for the j loop
			int j_stop = 0;

			ratio = (double) inputLength/outputLength;
			ratio_i = 1.0 / ratio;

			for (int i = 0; i < outputLength; i++) {
				pos_in = (double) i * ratio;

				j_stop = (int) (pos_in + 1.5);

				j_start = j_stop - 2;
				if (j_start < 0) {
					j_start = 0;
				}

				// The boundary check is done after j_start is calculated to avoid miscalculating it
				if (j_stop >= inputLength) {
					j_stop = inputLength - 1;
				}

				for (int j = j_start; j <= j_stop; j++) {
					x = j - pos_in + 1.5; // calculate position within the Blackman square function in the [0.0 ; 3.0] range
					pos_lut = x * foo;
					pos_luti = (int) pos_lut;

					// modulo of the index
					mod_pos = Util.FMod(pos_lut, 1.0);

					if (pos_luti + 1 < lut.Length) {
						y0 = lut[pos_luti]; // interpolate linearly between the two closest values
						y1 = lut[pos_luti + 1];
						coef = y0 + mod_pos * (y1 - y0); // linear interpolation

						@out[i] += @in[j] * coef; // convolve
					}
				}
			}
		}
		
		/// <summary>
		/// Analyze the input
		/// </summary>
		/// <param name="s">Sound (original signal)</param>
		/// <param name="samplecount">Sample count (samplecount is the original signal's orginal length)</param>
		/// <param name="samplerate">Sample rate</param>
		/// <param name="Xsize">Specifies the desired width of the spectrogram</param>
		/// <param name="bands">Specifies the desired height of the spectrogram (bands is the total count of bands)</param>
		/// <param name="bandsperoctave">Frequency resolution in Bands Per Octave</param>
		/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
		/// <param name="minFreq">Minimum frequency in Hertz</param>
		/// <returns>Image</returns>
		public static double[][] Analyze(ref double[] s, ref int samplecount, ref int samplerate, ref int Xsize, ref int bands, ref double bandsperoctave, ref double pixpersec, ref double minFreq)
		{
			int paddedLength = 0;				// Mb is the length of the original signal once zero-padded (always even)
			int filteredLength = 0;				// Mc is the length of the filtered signal
			int envelopeDownsampledLength = 0;	// Md is the length of the envelopes once downsampled (constant)
			int bandIndexStart = 0;				// Fa is the index of the band's start in the frequency domain
			int bandIndexEnd = 0;				// Fd is the index of the band's end in the frequency domain

			double[][] @out;			// @out is the output image
			double[] filterBandRotated;	// filtered band signal rotated by 90°
			double[] freqCentral;		// freq is the band's central frequency
			double coef;				// coef is a temporary modulation coefficient
			double logBandFreqStart;	// La is the log2 of the frequency of Fa
			double logBandFreqEnd;		// Ld is the log2 of the frequency of Fd
			double logFreqIterator;		// Li is the iterative frequency between La and Ld defined logarithmically
			double maxFreq;				// maxfreq is the central frequency of the last band
			
			if (LOGBASE == 1.0) {
				maxFreq = bandsperoctave; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
			} else {
				maxFreq = minFreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bandsperoctave));
			}
			
			Xsize = (int) (samplecount * pixpersec);

			freqCentral = FrequencyArray(minFreq, bands, bandsperoctave);

			//var logFrequenciesIndex = new int[1];
			//var logFrequencies = new float[1];
			//CommonUtils.FFT.AudioAnalyzer.GetLogFrequenciesIndex(samplerate, minFreq*samplerate, maxFreq*samplerate, bands-1, 1024, 2, out logFrequenciesIndex, out logFrequencies);
			//MathUtils.Divide(ref logFrequencies, samplerate);
			//freqCentral = MathUtils.FloatToDouble(logFrequencies);
			
			// round-up
			if (Util.FMod((double) samplecount * pixpersec, 1.0) != 0.0) {
				Xsize++;
			}
			Console.Write("Image size : {0:D}x{1:D}\n", Xsize, bands);
			
			@out = new double[bands][];
			
			clockA = Util.GetTimeTicks();

			#region ZEROPADDING Note : Don't do it in Circular mode
			// Mb is the length of the original signal once zero-padded (always even)
			if (LOGBASE == 1.0) {
				paddedLength = samplecount - 1 + Util.RoundToClosestInt(5.0/ freqCentral[1]-freqCentral[0]); // linear mode
			} else {
				paddedLength = samplecount - 1 + Util.RoundToClosestInt(2.0 * 5.0/((freqCentral[0] * Math.Pow(LOGBASE, -1.0/bandsperoctave)) * (1.0 - Math.Pow(LOGBASE, -1.0/bandsperoctave))));
			}

			if (paddedLength % 2 == 1)  { // if Mb is odd
				paddedLength++; // make it even (for the sake of simplicity)
			}

			filteredLength = 0;
			envelopeDownsampledLength = 0;

			paddedLength = Util.RoundToClosestInt((double) Util.NextLowPrimes((int) Util.RoundToClosestInt(paddedLength * pixpersec)) / pixpersec);

			// Md is the length of the envelopes once downsampled
			envelopeDownsampledLength = Util.RoundToClosestInt(paddedLength * pixpersec);

			// reallocate to the zeropadded size
			Array.Resize<double>(ref s, paddedLength);
			#endregion
			
			// In-place FFT of the original zero-padded signal
			FFT(ref s, ref s, paddedLength, FFTMethod.DFT);

			for (int bandCounter = 0; bandCounter < bands; bandCounter++) {
				#region Filtering
				bandIndexStart = Util.RoundToClosestInt(LogPositionToFrequency((double)(bandCounter-1)/(double)(bands-1), minFreq, maxFreq) * paddedLength);
				bandIndexEnd = Util.RoundToClosestInt(LogPositionToFrequency((double)(bandCounter+1)/(double)(bands-1), minFreq, maxFreq) * paddedLength);
				logBandFreqStart = FrequencyToLogPosition((double) bandIndexStart / (double) paddedLength, minFreq, maxFreq);
				logBandFreqEnd = FrequencyToLogPosition((double) bandIndexEnd / (double) paddedLength, minFreq, maxFreq);

				// stop reading if reaching the Nyquist frequency
				if (bandIndexEnd > paddedLength/2) {
					bandIndexEnd = paddedLength/2;
				}

				// ensure the band index is at least 1
				if (bandIndexStart < 1) {
					bandIndexStart = 1;
				}

				// the actual band length
				int bandIndexLength = bandIndexEnd-bandIndexStart + 1;
				
				// Mc is the length of the filtered signal
				// '*2' because the filtering is on both real and imaginary parts,
				// '+1' for the DC.
				// No Nyquist component since the signal length is necessarily odd
				filteredLength = (bandIndexEnd-bandIndexStart)*2 + 1;

				// if the band is going to be too narrow
				if (envelopeDownsampledLength > filteredLength) {
					filteredLength = envelopeDownsampledLength;
				}

				// round the larger bands up to the next integer made of 2^n * 3^m
				if (envelopeDownsampledLength < filteredLength) {
					filteredLength = Util.NextLowPrimes(filteredLength);
				}

				Console.Write("{0,4:D}/{1:D} (FFT size: {2,6:D})   {3:f2} Hz - {4:f2} Hz\r", bandCounter+1, bands, filteredLength, (double) bandIndexStart*samplerate/paddedLength, (double) bandIndexEnd*samplerate/paddedLength);

				int currentBandIndex = bands-bandCounter-1;
				
				@out[currentBandIndex] = new double[filteredLength+1];

				for (int i = 0; i < bandIndexLength-1; i++) {
					logFreqIterator = FrequencyToLogPosition((double)(i+bandIndexStart) / (double) paddedLength, minFreq, maxFreq); // calculation of the logarithmic position
					logFreqIterator = (logFreqIterator-logBandFreqStart)/(logBandFreqEnd-logBandFreqStart);
					coef = 0.5 - 0.5 * Math.Cos(2.0 * PI * logFreqIterator); // Hann function
					
					@out[currentBandIndex][i+1] = s[i+1+bandIndexStart] * coef;
					@out[currentBandIndex][filteredLength-1-i] = s[paddedLength-bandIndexStart-1-i] * coef;
				}
				#endregion

				#region 90° rotation
				filterBandRotated = new double[filteredLength+1];
				
				// Rotation : Re' = Im; Im' = -Re
				for (int i = 0; i < bandIndexLength-1; i++) {
					filterBandRotated[i+1] = @out[currentBandIndex][filteredLength-1-i]; // Re' = Im
					filterBandRotated[filteredLength-1-i] = -@out[currentBandIndex][i+1]; 	// Im' = -Re
				}
				#endregion

				#region Envelope detection

				// In-place IFFT of the filtered band signal
				FFT(ref @out[currentBandIndex], ref @out[currentBandIndex], filteredLength, FFTMethod.IDFT);
				
				// In-place IFFT of the filtered band signal rotated by 90°
				FFT(ref filterBandRotated, ref filterBandRotated, filteredLength, FFTMethod.IDFT);

				for (int i = 0; i < filteredLength; i++) {
					// Magnitude of the analytic signal
					// out[bands-ib-1][i] = sqrt(out[bands-ib-1][i]*out[bands-ib-1][i] + h[i]*h[i]);
					double x = @out[currentBandIndex][i];
					double y = filterBandRotated[i];
					double mag = Math.Sqrt(x*x + y*y);
					@out[currentBandIndex][i] = mag;
				}
				#endregion

				#region Downsampling
				if (filteredLength < envelopeDownsampledLength) { // if the band doesn't have to be resampled
					Array.Resize<double>(ref @out[currentBandIndex], envelopeDownsampledLength); // simply ignore the end of it
				}
				
				// If the band *has* to be downsampled
				if (filteredLength > envelopeDownsampledLength) {
					@out[currentBandIndex] = BlackmanDownsampling(@out[currentBandIndex], filteredLength, envelopeDownsampledLength); // Blackman downsampling
				}
				#endregion

				// Tail chopping
				Array.Resize<double>(ref @out[currentBandIndex], Xsize);
			}

			Console.Write("\n");

			Normalize(ref @out);
			
			return @out;
		}

		/// <summary>
		/// Windowed Sinc method
		/// </summary>
		/// <param name="length">Length</param>
		/// <param name="bw">Bandwidth</param>
		/// <returns></returns>
		public static double[] WindowedSincMax(int length, double bw)
		{
			// http://www.dspguide.com/ch16/1.htm
			int bwl = 0; // integer transition bandwidth
			double tbw; // double transition bandwidth
			double x; // position in the antiderivate of the Blackman function of the sample we're at
			double coef; // coefficient obtained from the function

			tbw = bw * (double)(length-1);
			bwl = Util.RoundUpToClosestInt(tbw);
			
			var h = new double[length]; // kernel
			for (int i = 1; i < length; i++) {
				h[i] = 1.0;
			}

			for (int i = 0; i < bwl; i++) {
				x = (double) i / tbw; // position calculation between 0.0 and 1.0
				// antiderivative of the Blackman window function
				coef = 0.42 * x - (0.5/(2.0 * PI)) * Math.Sin(2.0 * PI * x) + (0.08/(4.0 * PI)) * Math.Sin(4.0 *PI *x);
				coef *= 1.0 / 0.42;
				h[i+1] = coef;
				h[length-1-i] = coef;
			}
			return h;
		}
		
		/// <summary>
		/// Sine synthesis mode
		/// </summary>
		/// <param name="d">Image (d is the original image - spectrogram)</param>
		/// <param name="Xsize">Specifies the desired width of the spectrogram</param>
		/// <param name="bands">Specifies the desired height of the spectrogram (total count of bands)</param>
		/// <param name="samplecount">Number of samples (samplecount is the output sound's length)</param>
		/// <param name="samplerate">Sample rate</param>
		/// <param name="minFreq">Minimum frequency in Hertz</param>
		/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
		/// <param name="bpo">Frequency resolution in Bands Per Octave</param>
		/// <returns></returns>
		public static double[] SynthesizeSine(ref double[][] d, ref int Xsize, ref int bands, ref int samplecount, ref int samplerate, ref double minFreq, ref double pixpersec, ref double bpo)
		{
			double[] s; 					// s is the output sound
			double[] freq;					// freq is the band's central frequency
			double[] filter;
			double[] shiftedBand;			// sband is the band's envelope upsampled and shifted up in frequency
			int shiftedBandSize = 0;		// sbsize is the length of sband

			var sine = new double[4];		// sine is the random sine look-up table
			double rphase;					// rphase is the band's sine's random phase
			
			int newSignalBandCenterIndex = 0;	// Fc is the index of the band's centre in the frequency domain on the new signal
			int shiftedBandCenterIndex = 0;		// Bc is the index of the band's centre in the frequency domain on sband (its imaginary match being sbsize-Bc)
			int envelopeFFTSize = 0;			// Mh is the length of the real or imaginary part of the envelope's FFT, DC element included and Nyquist element excluded
			int soundFFTSize = 0;				// Mn is the length of the real or imaginary part of the sound's FFT, DC element included and Nyquist element excluded

			freq = FrequencyArray(minFreq, bands, bpo);

			clockA = Util.GetTimeTicks();

			// In Circular mode keep it to sbsize = Xsize * 2;
			shiftedBandSize = Util.NextLowPrimes(Xsize * 2);

			samplecount = Util.RoundToClosestInt((double) Xsize/pixpersec);
			Console.Write("Sound duration : {0:f3} s\n", (double) samplecount/samplerate);
			
			// Do not change this value as it would stretch envelopes
			samplecount = Util.RoundToClosestInt(0.5 * (double) shiftedBandSize/pixpersec);

			// allocation of the final signal
			s = new double[samplecount];
			
			// allocation of the shifted band
			shiftedBand = new double[shiftedBandSize];

			// Bc is the index of the band's centre in the frequency domain on sband (its imaginary match being shiftedBandSize-shiftedBandCenterIndex)
			shiftedBandCenterIndex = Util.RoundToClosestInt(0.25 * (double) shiftedBandSize);

			// Mh is the length of the real or imaginary part of the envelope's FFT, DC element included and Nyquist element excluded
			envelopeFFTSize = (shiftedBandSize + 1) >> 1;
			
			// Mn is the length of the real or imaginary part of the sound's FFT, DC element included and Nyquist element excluded
			soundFFTSize = (samplecount + 1) >> 1;

			// generation of the frequency-domain filter
			filter = WindowedSincMax(envelopeFFTSize, 1.0 / TRANSITION_BW_SYNT);

			for (int bandCounter = 0; bandCounter < bands; bandCounter++) {
				
				// reset sband
				Array.Clear(shiftedBand, 0, shiftedBand.Length);
				
				#region Frequency shifting
				rphase = Util.DoubleRandom() * PI; // random phase between -pi and +pi

				for (int i = 0; i < 4; i++) {
					// generating the random sine LUT (look up table)
					sine[i] = Math.Cos(i * 2.0 * PI * 0.25 + rphase);
				}

				int currentBandIndex = bands-bandCounter-1;
				for (int i = 0; i < Xsize; i++) { // envelope sampling rate * 2 and frequency shifting by 0.25
					if ((i & 1) == 0) {
						shiftedBand[i<<1] = d[currentBandIndex][i] * sine[0];
						shiftedBand[(i<<1) + 1] = d[currentBandIndex][i] * sine[1];
					} else {
						shiftedBand[i<<1] = d[currentBandIndex][i] * sine[2];
						shiftedBand[(i<<1) + 1] = d[currentBandIndex][i] * sine[3];
					}
				}
				#endregion Frequency shifting

				// FFT of the envelope
				FFT(ref shiftedBand, ref shiftedBand, shiftedBandSize, FFTMethod.DFT);
				
				// Fc is the index of the band's centre in the frequency domain on the new signal
				newSignalBandCenterIndex = Util.RoundToClosestInt(freq[bandCounter] * samplecount); // band's centre index (envelope's DC element)

				Console.Write("{0,4:D}/{1:D}   {2:f2} Hz\r", bandCounter+1, bands, (double) newSignalBandCenterIndex * samplerate / samplecount);

				#region Write FFT
				for (int i = 1; i < envelopeFFTSize; i++) {
					// if we're between frequencies 0 and 0.5 of the new signal and that we're not at Fc (newSignalBandCenterIndex)
					if (newSignalBandCenterIndex-shiftedBandCenterIndex+i > 0 && newSignalBandCenterIndex-shiftedBandCenterIndex+i < soundFFTSize) {
						s[i+newSignalBandCenterIndex-shiftedBandCenterIndex] += shiftedBand[i] * filter[i]; // Real part
						s[samplecount-(i+newSignalBandCenterIndex-shiftedBandCenterIndex)] += shiftedBand[shiftedBandSize-i] * filter[i]; // Imaginary part
					}
				}
				#endregion Write FFT
			}

			Console.Write("\n");

			FFT(ref s, ref s, samplecount, FFTMethod.IDFT); // IFFT of the final sound
			samplecount = Util.RoundToClosestInt((double)Xsize/pixpersec); // chopping tails by ignoring them

			Normalize(ref s);

			return s;
		}
		
		/// <summary>
		/// Noise synthesis mode
		/// </summary>
		/// <param name="d">Image</param>
		/// <param name="Xsize">Specifies the desired width of the spectrogram</param>
		/// <param name="bands">Specifies the desired height of the spectrogram (bands)</param>
		/// <param name="samplecount">Number of samples</param>
		/// <param name="samplerate">Sample rate</param>
		/// <param name="minFreq">Base frequency in Hertz</param>
		/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
		/// <param name="bpo">Frequency resolution in Bands Per Octave</param>
		/// <returns></returns>
		public static double[] SynthesizeNoise(ref double[][] d, ref int Xsize, ref int bands, ref int samplecount, ref int samplerate, ref double minFreq, ref double pixpersec, ref double bpo)
		{
			double[] s; 							// final signal
			double coef;
			
			double[] noise; 						// filtered looped noise
			double loop_size_sec = LOOP_SIZE_SEC; 	// size of the filter bank loop, in seconds. Later to be taken from user input
			int loop_size = 0; 						// size of the filter bank loop, in samples. Deduced from loop_size_sec
			int loop_size_min = 0; 					// minimum required size for the filter bank loop, in samples. Calculated from the longest windowed sinc's length
			
			double[] pink_noise; 					// original pink noise (in the frequency domain)
			double mag; 							// parameters for the creation of pink_noise's samples
			double phase;
			double[] envelope; 	// interpolated envelope
			double[] lut; 		// Blackman Sqaure look-up table
			double[] freq; 		// frequency look-up table
			double maxfreq; 	// central frequency of the last band
			
			int bandIndexStart = 0; // Fa is the index of the band's start in the frequency domain
			int bandIndexEnd = 0; 	// Fd is the index of the band's end in the frequency domain
			
			double LogBandFreqStart; // La is the log2 of the frequency of Fa
			double LogBandFreqEnd; 	 // Ld is the log2 of the frequency of Fd
			double LogFreqIterator; // Li is the iterative frequency between La and Ld defined logarithmically

			freq = FrequencyArray(minFreq, bands, bpo);

			if (LOGBASE == 1.0) {
				maxfreq = bpo; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
			} else {
				maxfreq = minFreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bpo));
			}

			clockA = Util.GetTimeTicks();

			samplecount = Util.RoundToClosestInt(Xsize/pixpersec); // calculation of the length of the final signal
			Console.Write("Sound duration : {0:f3} s\n", (double) samplecount/samplerate);

			#region Loop size calculation
			loop_size = (int) loop_size_sec * samplerate;

			if (LOGBASE == 1.0) {
				// linear mode
				loop_size_min = Util.RoundToClosestInt(4.0 * 5.0/ freq[1]-freq[0]);
			} else {
				// this is the estimate of how many samples the longest FIR
				// will take up in the time domain
				loop_size_min = Util.RoundToClosestInt(2.0 * 5.0/((freq[0] * Math.Pow(2.0, -1.0/bpo)) * (1.0 - Math.Pow(2.0, -1.0/bpo))));
			}

			if (loop_size_min > loop_size) {
				loop_size = loop_size_min;
			}

			// enlarge the loop_size to the next multiple of short primes in order to make IFFTs faster
			loop_size = Util.NextLowPrimes(loop_size);
			#endregion Loop size calculation

			#region Pink noise generation
			pink_noise = new double[loop_size];

			for (int i = 1; i < (loop_size+1)>>1; i++) {
				mag = Math.Pow((double) i, 0.5 - 0.5 * LOGBASE); // FIXME something's not necessarily right with that formula
				phase = Util.DoubleRandom() * PI; // random phase between -pi and +pi

				pink_noise[i] = mag * Math.Cos(phase); // real part
				pink_noise[loop_size-i] = mag * Math.Sin(phase); // imaginary part
			}
			#endregion Pink noise generation

			// allocate noise
			noise = new double[loop_size];
			
			// Blackman Square look-up table initalisation
			lut = BlackmanSquareLookupTable(ref BMSQ_LUT_SIZE);

			// allocation of the final signal
			s = new double[samplecount];
			
			for (int ib = 0; ib < bands; ib++) {
				// reset filtered noise
				Array.Clear(noise, 0, noise.Length);
				
				#region Filtering
				bandIndexStart = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib-1)/(double)(bands-1), minFreq, maxfreq) * loop_size);
				bandIndexEnd = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib+1)/(double)(bands-1), minFreq, maxfreq) * loop_size);
				LogBandFreqStart = FrequencyToLogPosition((double) bandIndexStart / (double) loop_size, minFreq, maxfreq);
				LogBandFreqEnd = FrequencyToLogPosition((double) bandIndexEnd / (double) loop_size, minFreq, maxfreq);

				if (bandIndexEnd > loop_size/2) {
					bandIndexEnd = loop_size/2; // stop reading if reaching the Nyquist frequency
				}

				if (bandIndexStart < 1) {
					bandIndexStart = 1;
				}

				Console.Write("{0,4:D}/{1:D}   {2:f2} Hz - {3:f2} Hz\r", ib+1, bands, (double) bandIndexStart *samplerate/loop_size, (double) bandIndexEnd *samplerate/loop_size);

				for (int i = bandIndexStart; i < bandIndexEnd; i++) {
					LogFreqIterator = FrequencyToLogPosition((double) i / (double) loop_size, minFreq, maxfreq); // calculation of the logarithmic position
					LogFreqIterator = (LogFreqIterator-LogBandFreqStart)/(LogBandFreqEnd-LogBandFreqStart);
					coef = 0.5 - 0.5 *Math.Cos(2.0 * PI * LogFreqIterator); // Hann function
					noise[i+1] = pink_noise[i+1] * coef;
					noise[loop_size-1-i] = pink_noise[loop_size-1-i] * coef;
				}
				#endregion Filtering

				// IFFT of the filtered noise
				FFT(ref noise, ref noise, loop_size, FFTMethod.IDFT);
				
				// allocation of the interpolated envelope
				envelope = new double[samplecount];
				
				// interpolation of the envelope
				// this is the slowest part of the noise synthesis
				BlackmanSquareInterpolation(ref d[bands-ib-1], ref envelope, ref Xsize, ref samplecount, ref lut, BMSQ_LUT_SIZE);
				
				int il = 0;
				for (int i = 0; i < samplecount; i++)
				{
					s[i] += envelope[i] * noise[il]; // modulation
					il++; // increment loop iterator
					
					// if the array iterator has reached the end of the array, it's reset
					if (il == loop_size) {
						il = 0;
					}
				}
			}

			Console.Write("\n");

			Normalize(ref s);

			return s;
		}
		
		/// <summary>
		/// Almost like gamma : f(x) = x^1/brightness\n
		/// Actually this is based on the idea of converting values to decibels,
		/// for example, 0.01 becomes -40 dB, dividing them by ratio,
		/// so if ratio is 2 then -40 dB/2 = -20 dB,
		/// and then turning it back to regular values, so -20 dB becomes 0.1
		/// If ratio==2 then this function is equivalent to a square root
		/// 1/ratio is used for the forward transformation
		/// ratio is used for the reverse transformation
		/// </summary>
		/// <param name="image">Image</param>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		/// <param name="ratio">Ratio</param>
		public static void BrightnessControl(ref double[][] image, ref int width, ref int height, double ratio)
		{
			// Actually this is based on the idea of converting values to decibels, for example, 0.01 becomes -40 dB, dividing them by ratio, so if ratio is 2 then -40 dB/2 = -20 dB, and then turning it back to regular values, so -20 dB becomes 0.1
			// If ratio==2 then this function is equivalent to a square root
			// 1/ratio is used for the forward transformation
			// ratio is used for the reverse transformation
			for (int iy = 0; iy<width; iy++) {
				for (int ix = 0; ix<height; ix++) {
					image[iy][ix] = Math.Pow(image[iy][ix], ratio);
				}
			}
		}

		/// <summary>
		/// Turns a logarithmic position (i.e. band number/band count) to a frequency
		/// </summary>
		/// <param name="x">Log Position (double between 0 and 1)</param>
		/// <param name="min">Minimum (double between 0 and 1)</param>
		/// <param name="max">Maximum (double between 0 and 1)</param>
		/// <returns></returns>
		public static double LogPositionToFrequency(double x, double min, double max)
		{
			if (LOGBASE == 1.0) {
				return x * (max - min) + min;
			} else {
				double logMin = Math.Log(min);
				double logMax = Math.Log(max);
				double delta = (logMax - logMin) / Math.Log(2.0);
				
				return (max - min) * (min * Math.Pow(LOGBASE, x * delta) - min) / (min * Math.Pow(LOGBASE, delta) - min) + min;
			}
		}

		/// <summary>
		/// Turns a frequency to a logarithmic position (i.e. band number/band count)
		/// </summary>
		/// <param name="x">Frequency (double between 0 and 1)</param>
		/// <param name="min">Minimum (double between 0 and 1)</param>
		/// <param name="max">Maximum (double between 0 and 1)</param>
		/// <returns></returns>
		public static double FrequencyToLogPosition(double x, double min, double max)
		{
			if (LOGBASE == 1.0) {
				return (x - min)/(max-min);
			} else {
				double logMin = Math.Log(min);
				double logMax = Math.Log(max);
				double delta = (logMax - logMin);
				double logDelta = delta / Math.Log(2.0);

				return Math.Log(((x - min) * (min * Math.Pow(LOGBASE, logDelta) - min) / (max - min) + min) / Math.Log(LOGBASE)) * Math.Log(2.0) / delta;
			}
		}
	}
}