using System;
using CommonUtils; // for MathUtils
using fftwlib;

// Papers:
// Spectral Analysis, Editing, and Resynthesis: Methods and Applications
// http://www.klingbeil.com/data/Klingbeil_Dissertation_web.pdf

// Spectral Envelopes in Sound Analysis and Synthesis
// http://articles.ircam.fr/textes/Schwarz98a/index.pdf

// Pitch Shifting Using The Fourier Transform
// http://www.dspdimension.com/admin/pitch-shifting-using-the-ft/

public static class DSP
{
	public static double PI;
	public static double LOGBASE; // Base for log() operations. Anything other than 2 isn't really supported
	public static double LOOP_SIZE_SEC; // size of the noise loops in seconds
	public static int BMSQ_LUT_SIZE; // defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached

	public static long clockA;
	
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
	/// <param name="xs">X size</param>
	/// <param name="ys">Y size</param>
	/// <param name="ratio"></param>
	public static void Normalize(ref double[][] s, ref int xs, ref int ys, double ratio)
	{
		double max = 0;
		for (int iy = 0; iy < ys; iy++) {
			for (int ix = 0; ix < xs; ix++) {
				if (Math.Abs(s[iy][ix]) > max) {
					max = Math.Abs(s[iy][ix]);
				}
			}
		}

		if (max != 0.0) {
			max /= ratio;
			max = 1.0 / max;
		} else {
			max = 0.0;
		}

		for (int iy = 0; iy < ys; iy++) {
			for (int ix = 0; ix < xs; ix++) {
				s[iy][ix] *= max;
			}
		}
	}
	
	/// <summary>
	/// Normalises a signal to the +/-ratio range
	/// </summary>
	/// <param name="s">Samples</param>
	/// <param name="xs">X size</param>
	/// <param name="ratio"></param>
	public static void Normalize(ref double[] s, ref int xs, double ratio)
	{
		double max = 0;
		for (int ix = 0; ix < xs; ix++) {
			if (Math.Abs(s[ix]) > max) {
				max = Math.Abs(s[ix]);
			}
		}

		if (max != 0.0) {
			max /= ratio;
			max = 1.0/max;
		} else {
			max = 0.0;
		}

		for (int ix = 0; ix < xs; ix++) {
			s[ix] *= max;
		}
	}
	
	/// <summary>
	/// Create Frequency Table
	/// </summary>
	/// <param name="basefreq">Minimum frequency in Hertz</param>
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
			freq[i] = LogPositionToFrequency((double) i/(double)(bands-1), basefreq, maxfreq); //band's central freq
		}
		
		if (LogPositionToFrequency((double) bands / (double)(bands-1), basefreq, maxfreq)>0.5) {
			// TODO change sampling rate instead
			Console.Write("Warning: Upper frequency limit above Nyquist frequency\n");
		}

		return freq;
	}
	
	/// <summary>
	/// Downsampling of the signal by a Blackman function
	/// </summary>
	/// <param name="in">Signal</param>
	/// <param name="Mi">Mi is the original signal's length</param>
	/// <param name="Mo">Mo is the output signal's length</param>
	/// <returns>Downsampled Signal</returns>
	public static double[] BlackmanDownsampling(double[] @in, int Mi, int Mo)
	{
		var @out = new double[Mo];

		double pos_in; 				// position in the original signal
		double ratio; 				// scaling ratio (> 1.0)
		double ratio_i; 			// ratio^-1
		double coef; 				// Blackman coefficient
		double coef_sum; 			// sum of coefficients, used for weighting

		// Mi is the original signal's length
		// Mo is the output signal's length
		ratio = (double) Mi/Mo;
		ratio_i = 1.0/ratio;

		for (int i = 0; i < Mo; i++)
		{
			pos_in = (double) i * ratio;

			coef_sum = 0;

			for (int j = Util.RoundUpToClosestInt(pos_in - ratio); j<=pos_in + ratio; j++)
			{
				if (j >= 0 && j < Mi) // if the read sample is within bounds
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
	/// <param name="Mi">Mi is the original signal's length</param>
	/// <param name="Mo">Mo is the output signal's length</param>
	/// <param name="lut">Blackman Square Lookup Table</param>
	/// <param name="lut_size">Defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached</param>
	public static void BlackmanSquareInterpolation(ref double[] @in, ref double[] @out, ref int Mi, ref int Mo, ref double[] lut, int lut_size)
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

		// Mi is the original signal's length
		// Mo is the output signal's length
		ratio = (double) Mi/Mo;
		ratio_i = 1.0 / ratio;

		for (int i = 0; i < Mo; i++) {
			pos_in = (double) i * ratio;

			j_stop = (int) (pos_in + 1.5);

			j_start = j_stop - 2;
			if (j_start < 0) {
				j_start = 0;
			}

			// The boundary check is done after j_start is calculated to avoid miscalculating it
			if (j_stop >= Mi) {
				j_stop = Mi - 1;
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
	/// <param name="bpo">Frequency resolution in Bands Per Octave</param>
	/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
	/// <param name="basefreq">Minimum frequency in Hertz</param>
	/// <returns>Image</returns>
	public static double[][] Analyze(ref double[] s, ref int samplecount, ref int samplerate, ref int Xsize, ref int bands, ref double bpo, ref double pixpersec, ref double basefreq)
	{
		int Mb = 0;			// Mb is the length of the original signal once zero-padded (always even)
		int Mc = 0;			// Mc is the length of the filtered signal
		int Md = 0;			// Md is the length of the envelopes once downsampled (constant)
		int Fa = 0;			// Fa is the index of the band's start in the frequency domain
		int Fd = 0;			// Fd is the index of the band's end in the frequency domain

		double[][] @out;	// @out is the output image
		double[] h;
		double[] freq;		// freq is the band's central frequency
		double[] t;			// t is temporary pointer to a new version of the signal being worked on
		double coef;		// coef is a temporary modulation coefficient
		double La;			// La is the log2 of the frequency of Fa
		double Ld;			// Ld is the log2 of the frequency of Fd
		double Li;			// Li is the iterative frequency between La and Ld defined logarithmically
		double maxfreq;		// maxfreq is the central frequency of the last band

		freq = FrequencyArray(basefreq, bands, bpo);

		//Export.ExportCSV(String.Format("freq.csv"), freq, 256);
		
		if (LOGBASE == 1.0) {
			maxfreq = bpo; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
		} else {
			maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bpo));
		}
		
		Xsize = (int) (samplecount * pixpersec);

		if (Util.FMod((double) samplecount * pixpersec, 1.0) != 0.0) {
			// round-up
			Xsize++;
		}
		Console.Write("Image size : {0:D}x{1:D}\n", Xsize, bands);
		
		@out = new double[bands][];
		
		clockA = Util.GetTimeTicks();

		#region ZEROPADDING Note : Don't do it in Circular mode
		// Mb is the length of the original signal once zero-padded (always even)
		if (LOGBASE == 1.0) {
			Mb = samplecount - 1 + Util.RoundToClosestInt(5.0/ freq[1]-freq[0]); // linear mode
		} else {
			Mb = samplecount - 1 + Util.RoundToClosestInt(2.0 * 5.0/((freq[0] * Math.Pow(LOGBASE, -1.0/(bpo))) * (1.0 - Math.Pow(LOGBASE, -1.0 / bpo))));
		}

		if (Mb % 2 == 1)  { // if Mb is odd
			Mb++; // make it even (for the sake of simplicity)
		}

		Mc = 0;
		Md = 0;

		Mb = Util.RoundToClosestInt((double) Util.NextLowPrimes((int) Util.RoundToClosestInt(Mb * pixpersec)) / pixpersec);

		// Md is the length of the envelopes once downsampled (constant)
		Md = Util.RoundToClosestInt(Mb * pixpersec);

		// realloc to the zeropadded size
		Array.Resize<double>(ref s, Mb);
		
		// zero-out the padded area.
		// don't think this is needed
		//for (i=samplecount; i<Mb; i++) {
		//	s[i] = 0;
		//}
		#endregion
		
		FFT(ref s, ref s, Mb, FFTMethod.DFT); // In-place FFT of the original zero-padded signal

		for (int ib = 0; ib < bands; ib++) {
			#region Filtering
			Fa = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib-1)/(double)(bands-1), basefreq, maxfreq) * Mb);
			Fd = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib+1)/(double)(bands-1), basefreq, maxfreq) * Mb);
			La = FrequencyToLogPosition((double) Fa / (double) Mb, basefreq, maxfreq);
			Ld = FrequencyToLogPosition((double) Fd / (double) Mb, basefreq, maxfreq);

			// stop reading if reaching the Nyquist frequency
			if (Fd > Mb/2) {
				Fd = Mb/2;
			}

			if (Fa < 1) {
				Fa = 1;
			}

			// Mc is the length of the filtered signal
			Mc = (Fd-Fa)*2 + 1;
			// '*2' because the filtering is on both real and imaginary parts,
			// '+1' for the DC.
			// No Nyquist component since the signal length is necessarily odd

			// if the band is going to be too narrow
			if (Md > Mc) {
				Mc = Md;
			}

			// round the larger bands up to the next integer made of 2^n * 3^m
			if (Md < Mc) {
				Mc = Util.NextLowPrimes(Mc);
			}

			Console.Write("{0,4:D}/{1:D} (FFT size: {2,6:D})   {3:f2} Hz - {4:f2} Hz\r", ib+1, bands, Mc, (double) Fa *samplerate/Mb, (double) Fd *samplerate/Mb);

			@out[bands-ib-1] = new double[Mc+1];

			for (int i = 0; i < Fd-Fa; i++) {
				Li = FrequencyToLogPosition((double)(i+Fa) / (double) Mb, basefreq, maxfreq); // calculation of the logarithmic position
				Li = (Li-La)/(Ld-La);
				coef = 0.5 - 0.5 * Math.Cos(2.0 * PI * Li); // Hann function
				
				@out[bands-ib-1][i+1] 		= s[i+1+Fa] * coef;
				@out[bands-ib-1][Mc-1-i] 	= s[Mb-Fa-1-i] * coef;
			}
			#endregion

			#region 90° rotation
			h = new double[Mc+1];
			
			// Rotation : Re' = Im; Im' = -Re
			for (int i = 0; i < Fd-Fa; i++) {
				h[i+1] 		= @out[bands-ib-1][Mc-1-i]; // Re' = Im
				h[Mc-1-i] 	= -@out[bands-ib-1][i+1]; 	// Im' = -Re
			}
			#endregion

			#region Envelope detection

			// In-place IFFT of the filtered band signal
			FFT(ref @out[bands-ib-1], ref @out[bands-ib-1], Mc, FFTMethod.IDFT);
			
			// In-place IFFT of the filtered band signal rotated by 90°
			FFT(ref h, ref h, Mc, FFTMethod.IDFT);

			for (int i = 0; i < Mc; i++) {
				// Magnitude of the analytic signal
				// out[bands-ib-1][i] = sqrt(out[bands-ib-1][i]*out[bands-ib-1][i] + h[i]*h[i]);
				
				double x = @out[bands-ib-1][i];
				double y = h[i];
				double xSquared = x*x;
				double ySquared = y*y;
				double mag = Math.Sqrt(xSquared + ySquared);
				@out[bands-ib-1][i] = mag;
			}
			
			//Array.Clear(h, 0, h.Length); // free h
			#endregion

			#region Downsampling
			if (Mc < Md) { // if the band doesn't have to be resampled
				Array.Resize<double>(ref @out[bands-ib-1], Md); // simply ignore the end of it
			}
			
			// If the band *has* to be downsampled
			if (Mc > Md) {
				t = @out[bands-ib-1];
				@out[bands-ib-1] = BlackmanDownsampling(@out[bands-ib-1], Mc, Md); // Blackman downsampling

				//Array.Clear(t, 0, t.Length); // free t
			}
			#endregion

			// Tail chopping
			Array.Resize<double>(ref @out[bands-ib-1], Xsize);
		}

		Console.Write("\n");

		Normalize(ref @out, ref Xsize, ref bands, 1.0);
		
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
	/// <param name="basefreq">Minimum frequency in Hertz</param>
	/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
	/// <param name="bpo">Frequency resolution in Bands Per Octave</param>
	/// <returns></returns>
	public static double[] SynthesizeSine(ref double[][] d, ref int Xsize, ref int bands, ref int samplecount, ref int samplerate, ref double basefreq, ref double pixpersec, ref double bpo)
	{
		double[] s; 					// s is the output sound
		double[] freq;					// freq is the band's central frequency
		double[] filter;
		double[] sband;					// sband is the band's envelope upsampled and shifted up in frequency
		int sbsize = 0;					// sbsize is the length of sband

		var sine = new double[4];		// sine is the random sine look-up table
		double rphase;					// rphase is the band's sine's random phase
		
		int Fc = 0;						// Fc is the index of the band's centre in the frequency domain on the new signal
		int Bc = 0;						// Bc is the index of the band's centre in the frequency domain on sband (its imaginary match being sbsize-Bc)
		int Mh = 0;						// Mh is the length of the real or imaginary part of the envelope's FFT, DC element included and Nyquist element excluded
		int Mn = 0;						// Mn is the length of the real or imaginary part of the sound's FFT, DC element included and Nyquist element excluded

		freq = FrequencyArray(basefreq, bands, bpo);

		clockA = Util.GetTimeTicks();

		// In Circular mode keep it to sbsize = Xsize * 2;
		sbsize = Util.NextLowPrimes(Xsize * 2);

		samplecount = Util.RoundToClosestInt(Xsize/pixpersec);
		Console.Write("Sound duration : {0:f3} s\n", (double) samplecount/samplerate);
		samplecount = Util.RoundToClosestInt(0.5 *sbsize/pixpersec); // Do not change this value as it would stretch envelopes

		// allocation of the final signal
		s = new double[samplecount];
		
		// allocation of the shifted band
		sband = new double[sbsize];

		// Bc is the index of the band's centre in the frequency domain on sband (its imaginary match being sbsize-Bc)
		Bc = Util.RoundToClosestInt(0.25 * (double) sbsize);

		// Mh is the length of the real or imaginary part of the envelope's FFT, DC element included and Nyquist element excluded
		Mh = (sbsize + 1) >> 1;
		// Mn is the length of the real or imaginary part of the sound's FFT, DC element included and Nyquist element excluded
		Mn = (samplecount + 1) >> 1;

		// generation of the frequency-domain filter
		filter = WindowedSincMax(Mh, 1.0 / Arss.TRANSITION_BW_SYNT);

		for (int ib = 0; ib < bands; ib++) {
			
			// reset sband
			for (int i=0; i < sbsize; i++) {
				sband[i] = 0; // reset sband
			}
			
			#region Frequency shifting
			rphase = Util.DoubleRandom() * PI; // random phase between -pi and +pi

			for (int i = 0; i < 4; i++) {
				// generating the random sine LUT (look up table)
				sine[i] = Math.Cos(i * 2.0 * PI * 0.25 + rphase);
			}

			for (int i = 0; i < Xsize; i++) { // envelope sampling rate * 2 and frequency shifting by 0.25
				if ((i & 1) == 0) {
					sband[i<<1] = d[bands-ib-1][i] * sine[0];
					sband[(i<<1) + 1] = d[bands-ib-1][i] * sine[1];
				} else {
					sband[i<<1] = d[bands-ib-1][i] * sine[2];
					sband[(i<<1) + 1] = d[bands-ib-1][i] * sine[3];
				}
			}
			#endregion Frequency shifting

			// FFT of the envelope
			FFT(ref sband, ref sband, sbsize, FFTMethod.DFT);
			
			// Fc is the index of the band's centre in the frequency domain on the new signal
			Fc = Util.RoundToClosestInt(freq[ib] * samplecount); // band's centre index (envelope's DC element)

			Console.Write("{0,4:D}/{1:D}   {2:f2} Hz\r", ib+1, bands, (double) Fc * samplerate / samplecount);

			#region Write FFT
			for (int i = 1; i < Mh; i++) {
				// if we're between frequencies 0 and 0.5 of the new signal and that we're not at Fc
				if (Fc-Bc+i > 0 && Fc-Bc+i < Mn) {
					s[i+Fc-Bc] += sband[i] * filter[i]; 						// Real part
					s[samplecount-(i+Fc-Bc)] += sband[sbsize-i] * filter[i]; 	// Imaginary part
				}
			}
			#endregion Write FFT
		}

		Console.Write("\n");

		FFT(ref s, ref s, samplecount, FFTMethod.IDFT); // IFFT of the final sound
		samplecount = Util.RoundToClosestInt(Xsize/pixpersec); // chopping tails by ignoring them

		Normalize(ref s, ref samplecount, 1.0);

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
	/// <param name="basefreq">Base frequency in Hertz</param>
	/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
	/// <param name="bpo">Frequency resolution in Bands Per Octave</param>
	/// <returns></returns>
	public static double[] SynthesizeNoise(ref double[][] d, ref int Xsize, ref int bands, ref int samplecount, ref int samplerate, ref double basefreq, ref double pixpersec, ref double bpo)
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
		
		int Fa = 0; // Fa is the index of the band's start in the frequency domain
		int Fd = 0; // Fd is the index of the band's end in the frequency domain
		
		double La; // La is the log2 of the frequency of Fa
		double Ld; // Ld is the log2 of the frequency of Fd
		double Li; // Li is the iterative frequency between La and Ld defined logarithmically

		freq = FrequencyArray(basefreq, bands, bpo);

		if (LOGBASE == 1.0) {
			maxfreq = bpo; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
		} else {
			maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bpo));
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
			loop_size_min = Util.RoundToClosestInt(2.0 * 5.0/((freq[0] * Math.Pow(2.0, -1.0/(bpo))) * (1.0 - Math.Pow(2.0, -1.0 / bpo))));
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
			Console.Write("{0,4:D}/{1:D}\r", ib+1, bands);

			// reset filtered noise
			for (int i=0; i < loop_size; i++) {
				noise[i] = 0; // reset sband
			}
			
			#region Filtering
			Fa = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib-1)/(double)(bands-1), basefreq, maxfreq) * loop_size);
			Fd = Util.RoundToClosestInt(LogPositionToFrequency((double)(ib+1)/(double)(bands-1), basefreq, maxfreq) * loop_size);
			La = FrequencyToLogPosition((double) Fa / (double) loop_size, basefreq, maxfreq);
			Ld = FrequencyToLogPosition((double) Fd / (double) loop_size, basefreq, maxfreq);

			if (Fd > loop_size/2) {
				Fd = loop_size/2; // stop reading if reaching the Nyquist frequency
			}

			if (Fa < 1) {
				Fa = 1;
			}

			Console.Write("{0,4:D}/{1:D}   {2:f2} Hz - {3:f2} Hz\r", ib+1, bands, (double) Fa *samplerate/loop_size, (double) Fd *samplerate/loop_size);

			for (int i = Fa; i < Fd; i++) {
				Li = FrequencyToLogPosition((double) i / (double) loop_size, basefreq, maxfreq); // calculation of the logarithmic position
				Li = (Li-La)/(Ld-La);
				coef = 0.5 - 0.5 *Math.Cos(2.0 * PI * Li); // Hann function
				noise[i+1] = pink_noise[i+1] * coef;
				noise[loop_size-1-i] = pink_noise[loop_size-1-i] * coef;
			}
			#endregion Filtering

			// IFFT of the filtered noise
			FFT(ref noise, ref noise, loop_size, FFTMethod.IDFT);
			
			// allocation of the interpolated envelope
			envelope = new double[samplecount];
			
			// blank the envelope
			//for (int i=0; i < samplecount; i++) envelope[i] = 0; // reset sband
			
			// interpolation of the envelope
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

		Normalize(ref s, ref samplecount, 1.0);

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
	/// <param name="x">Log Position</param>
	/// <param name="min">Minimum</param>
	/// <param name="max">Maximum</param>
	/// <returns></returns>
	public static double LogPositionToFrequency(double x, double min, double max)
	{
		if (LOGBASE == 1.0) {
			return x*(max-min) + min;
		} else {
			return (max-min) * (min * Math.Pow(LOGBASE, x * (Math.Log(max)-Math.Log(min))/Math.Log(2.0)) - min) / (min * Math.Pow(LOGBASE, (Math.Log(max)-Math.Log(min))/Math.Log(2.0)) - min) + min;
		}
	}

	/// <summary>
	/// Turns a frequency to a logarithmic position (i.e. band number/band count)
	/// </summary>
	/// <param name="x">Frequency</param>
	/// <param name="min">Minimum</param>
	/// <param name="max">Maximum</param>
	/// <returns></returns>
	public static double FrequencyToLogPosition(double x, double min, double max)
	{
		if (LOGBASE == 1.0) {
			return (x - min)/(max-min);
		} else {
			return Math.Log(((x-min) * (min * Math.Pow(LOGBASE, (Math.Log(max) - Math.Log(min))/Math.Log(2.0)) - min) / (max-min) + min) / Math.Log(LOGBASE)) * Math.Log(2.0) / (Math.Log(max) - Math.Log(min));
		}
	}
}
