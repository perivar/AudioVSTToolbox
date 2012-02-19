using System;
using CommonUtils;
using CommonUtils.FFT;

using System.Runtime.InteropServices;
using fftwlib;

public static class GlobalMembersDsp
{
	//#define PI_D
	//#define LOGBASE_D
	//#define LOOP_SIZE_SEC_D
	//#define BMSQ_LUT_SIZE_D

	public static double pi;
	public static double LOGBASE; // Base for log() operations. Anything other than 2 isn't really supported
	public static double LOOP_SIZE_SEC; // size of the noise loops in seconds
	public static Int32 BMSQ_LUT_SIZE = new Int32(); // defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached

	public static Int32 clocka = new Int32();

	public static void normi(double[][] s, Int32 xs, Int32 ys, double ratio)
	{
		Int32 ix = new Int32();
		Int32 iy = new Int32();
		Int32 maxx = new Int32();
		Int32 maxy = new Int32();
		double max;

		max = 0;
		for (iy = 0; iy<ys; iy++)
			for (ix = 0; ix<xs; ix++)
				if (Math.Abs(s[iy][ix])>max)
		{
			if (!double.IsInfinity(Math.Abs(s[iy][ix]))) {
				max = Math.Abs(s[iy][ix]);
				maxx = ix;
				maxy = iy;
			}
		}

		#if DEBUG
		Console.Write("norm : {0:f3} (Y:{1:D} X:{2:D})\n", max, maxy, maxx);
		#endif

		if (max!=0.0)
		{
			max /= ratio;
			max = 1.0/max;
		}
		else
			max = 0.0;

		for (iy = 0; iy<ys; iy++)
			for (ix = 0; ix<xs; ix++)
				s[iy][ix]*=max;

		#if DEBUG
		Console.Write("ex : {0:f3}\n", s[0][0]);
		#endif
	}
	
	// normalises a signal to the +/-ratio range
	public static void fft(double[] @in, double[] @out, Int32 N, UInt16 method)
	{
		/* method :
		 * 0 = DFT
		 * 1 = IDFT
		 * 2 = DHT
		 */
		/*
		@out = null;
		
		switch (method) {
			case 0: // DFT
				// perform the FFT
				@out = FFTUtils.Real(FFTUtils.FFT(@in));
				break;
			case 1: // IDFT
				// perform the FFT
				@out = FFTUtils.IFFT(@in, true, false);
				break;
			case 2: // DHT?!
				break;
		}
		 */
		
		//fftw_plan p = fftw_plan_r2r_1d(N, in, out, method, FFTW_ESTIMATE);
		//fftw_execute(p);
		//fftw_destroy_plan(p);
		
		//handles to managed arrays, keeps them pinned in memory
		//get handles and pin arrays so the GC doesn't move them
		//GCHandle hin = GCHandle.Alloc(@in,GCHandleType.Pinned);
		//GCHandle hout = GCHandle.Alloc(@out,GCHandleType.Pinned);
		
		
		
		//pointer to the FFTW plan object
		IntPtr p = IntPtr.Zero;

		// pointers to the unmanaged arrays
		IntPtr pin = fftwf.malloc(Marshal.SizeOf(@in[0]) * @in.Length);
		IntPtr pout = fftwf.malloc(Marshal.SizeOf(@out[0]) * @out.Length);

		try
		{
			//copy managed arrays to unmanaged arrays
			Marshal.Copy(@in, 0, pin, @in.Length);
			Marshal.Copy(@out, 0, pout, @out.Length);

			switch (method) {
				case 0: // DFT: R2HC=0
					//p = fftwf.r2r_1d(N, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(), fftw_kind.R2HC, fftw_flags.Estimate);
					p = fftwf.r2r_1d(N, pin, pout, fftw_kind.R2HC, fftw_flags.Estimate);
					break;
				case 1: // IDFT: HC2R=1
					//p = fftwf.r2r_1d(N, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(), fftw_kind.HC2R, fftw_flags.Estimate);
					p = fftwf.r2r_1d(N, pin, pout, fftw_kind.HC2R, fftw_flags.Estimate);
					break;
				case 2: // DHT: DHT=2
					//p = fftwf.r2r_1d(N, hin.AddrOfPinnedObject(), hout.AddrOfPinnedObject(), fftw_kind.DHT, fftw_flags.Estimate);
					p = fftwf.r2r_1d(N, pin, pout, fftw_kind.DHT, fftw_flags.Estimate);
					break;
			}

			fftwf.execute(p);
			fftwf.destroy_plan(p);

			//copy unmanaged arrays to managed arrays
			Marshal.Copy(pin, @in, 0, @in.Length);
			Marshal.Copy(pout, @out, 0 , @out.Length);
			
		} finally {
			// Free the unmanaged memory.
			fftwf.free(pin);
			fftwf.free(pout);
		}
	}

	// normalises a signal to the +/-ratio range
	public static void normi(double[] s, Int32 xs, double ratio)
	{
		Int32 ix = new Int32();
		Int32 maxx = new Int32();
		double max;

		max = 0;
		for (ix = 0; ix<xs; ix++)
			if (Math.Abs(s[ix])>max)
		{
			max = Math.Abs(s[ix]);
			maxx = ix;
		}

		#if DEBUG
		Console.Write("norm : {0:f3} (X:{1:D})\n", max, maxx);
		#endif

		if (max!=0.0)
		{
			max /= ratio;
			max = 1.0/max;
		}
		else
			max = 0.0;

		for (ix = 0; ix<xs; ix++)
			s[ix]*=max;

		#if DEBUG
		Console.Write("ex : {0:f3}\n", s[0]);
		#endif
	}
	
	public static double[] freqarray(double basefreq, Int32 bands, double bandsperoctave)
	{
		Int32 i = new Int32();
		double[] freq = new double[bands];
		double maxfreq;

		if (LOGBASE == 1.0)
			maxfreq = bandsperoctave; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
		else
			maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bandsperoctave));

		for (i = 0;i<bands;i++)
		{
			freq[i] = GlobalMembersDsp.log_pos((double) i/(double)(bands-1), basefreq, maxfreq); //band's central freq
		}
		
		if (GlobalMembersDsp.log_pos((double) bands / (double)(bands-1), basefreq, maxfreq)>0.5)
			Console.Write("Warning: Upper frequency limit above Nyquist frequency\n"); // TODO change sampling rate instead

		return freq;
	}
	
	// Downsampling of the signal by a Blackman function
	public static double[] blackman_downsampling(double[] @in, Int32 Mi, Int32 Mo)
	{
		Int32 i = new Int32(); 		// general purpose iterators
		Int32 j = new Int32();
		double[] @out = new double[Mo];
		double pos_in; 				// position in the original signal
		double x; 					// position of the iterator in the blackman(x) formula
		double ratio; 				// scaling ratio (> 1.0)
		double ratio_i; 			// ratio^-1
		double coef; 				// Blackman coefficient
		double coef_sum; 			// sum of coefficients, used for weighting

		/*
		 * Mi is the original signal's length
		 * Mo is the output signal's length
		 */
		ratio = (double) Mi/Mo;
		ratio_i = 1.0/ratio;

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//@out = calloc (Mo, sizeof(double));

		for (i = 0; i<Mo; i++)
		{
			pos_in = (double) i * ratio;

			coef_sum = 0;

			for (j = GlobalMembersUtil.roundup(pos_in - ratio); j<=pos_in + ratio; j++)
			{
				if (j>=0 && j<Mi) // if the read sample is within bounds
				{
					x = j - pos_in + ratio; // calculate position within the Blackman function
					coef = 0.42 - 0.5 *Math.Cos(pi * x * ratio_i) + 0.08 *Math.Cos(2 *pi * x * ratio_i);
					coef_sum += coef;
					@out[i] += @in[j] * coef; // convolve
				}
			}

			@out[i] /= coef_sum;
		}

		return @out;
	}
	
	// Blackman Square look-up table generator
	public static double[] bmsq_lut(Int32 size)
	{
		Int32 i = new Int32(); // general purpose iterator
		double coef; // Blackman square final coefficient
		double bar = pi * (3.0 / (double) size) * (1.0/1.5);
		double foo;

		double f1 = -0.6595044010905501; // Blackman Square coefficients
		double f2 = 0.1601741366715479;
		double f4 = -0.0010709178680006;
		double f5 = 0.0001450093579222;
		double f7 = 0.0001008528049040;
		double f8 = 0.0000653092892874;
		double f10 = 0.0000293385615146;
		double f11 = 0.0000205351559060;
		double f13 = 0.0000108567682890;
		double f14 = 0.0000081549460136;
		double f16 = 0.0000048519309366;
		double f17 = 0.0000038284344102;
		double f19 = 0.0000024753630724;

		size++; // allows to read value 3.0

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//double[] lut = calloc (size, sizeof(double));
		double[] lut = new double[size];
		
		for (i = 0; i<size; i++)
		{
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
	
	// Interpolation based on an estimate of the Blackman Square function,
	// which is a Blackman function convolved with a square.
	// It's like smoothing the result of a nearest neighbour interpolation
	// with a Blackman FIR
	public static void blackman_square_interpolation(double[] @in, double[] @out, Int32 Mi, Int32 Mo, double[] lut, Int32 lut_size)
	{
		Int32 i = new Int32(); // general purpose iterators
		Int32 j = new Int32();
		double pos_in; // position in the original signal
		double x; // position of the iterator in the blackman_square(x) formula
		double ratio; // scaling ratio (> 1.0)
		double ratio_i; // ratio^-1
		double coef; // Blackman square final coefficient
		double pos_lut; // Index on the look-up table
		Int32 pos_luti = new Int32(); // Integer index on the look-up table
		double mod_pos; // modulo of the position on the look-up table
		double y0; // values of the two closest values on the LUT
		double y1;
		double foo = (double) lut_size / 3.0;
		Int32 j_start = new Int32(); // boundary values for the j loop
		Int32 j_stop = new Int32();

		/*
		 * Mi is the original signal's length
		 * Mo is the output signal's length
		 */
		ratio = (double) Mi/Mo;
		ratio_i = 1.0/ratio;

		for (i = 0; i<Mo; i++)
		{
			pos_in = (double) i * ratio;

			j_stop = (int) (pos_in + 1.5);

			j_start = j_stop - 2;
			if (j_start<0)
				j_start = 0;

			if (j_stop >= Mi) // The boundary check is done after j_start is calculated to avoid miscalculating it
				j_stop = Mi - 1;

			for (j = j_start; j<=j_stop; j++)
			{
				x = j - pos_in + 1.5; // calculate position within the Blackman square function in the [0.0 ; 3.0] range
				pos_lut = x * foo;
				pos_luti = (Int32) pos_lut;

				mod_pos = Math.IEEERemainder(pos_lut, 1.0); // modulo of the index

				y0 = lut[pos_luti]; // interpolate linearly between the two closest values
				y1 = lut[pos_luti + 1];
				coef = y0 + mod_pos * (y1 - y0); // linear interpolation

				@out[i] += @in[j] * coef; // convolve
			}
		}
	}
	
	public static double[][] anal(double[] s, Int32 samplecount, Int32 samplerate, out Int32 Xsize, Int32 bands, double bpo, double pixpersec, double basefreq)
	{
		Int32 i = new Int32();
		Int32 ib = new Int32();
		Int32 Mb = new Int32();
		Int32 Mc = new Int32();
		Int32 Md = new Int32();
		Int32 Fa = new Int32();
		Int32 Fd = new Int32();
		double[][] @out;
		double[] h;
		double[] freq;
		//double[] t;
		double coef;
		double La;
		double Ld;
		double Li;
		double maxfreq;

		/*
		 s is the original signal
		 samplecount is the original signal's orginal length
		 ib is the band iterator
		 i is a general purpose iterator
		 Mb is the length of the original signal once zero-padded (always even)
		 Mc is the length of the filtered signal
		 Md is the length of the envelopes once downsampled (constant)
		 Fa is the index of the band's start in the frequency domain
		 Fd is the index of the band's end in the frequency domain
		 La is the log2 of the frequency of Fa
		 Ld is the log2 of the frequency of Fd
		 Li is the iterative frequency between La and Ld defined logarithmically
		 coef is a temporary modulation coefficient
		 t is temporary pointer to a new version of the signal being worked on
		 bands is the total count of bands
		 freq is the band's central frequency
		 maxfreq is the central frequency of the last band
		 */
		freq = GlobalMembersDsp.freqarray(basefreq, bands, bpo);

		if (LOGBASE == 1.0)
			maxfreq = bpo; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
		else
			maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bpo));

		Xsize = (int) (samplecount * pixpersec);

		if (Math.IEEERemainder((double) samplecount * pixpersec, 1.0) != 0.0) // round-up
			Xsize++;
		Console.Write("Image size : {0:D}x{1:D}\n", Xsize, bands);
		
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		//@out = malloc (bands * sizeof(double));
		@out = new double[bands][];
		
		clocka = GlobalMembersUtil.gettime();

		//********ZEROPADDING********	Note : Don't do it in Circular mode
		if (LOGBASE == 1.0) {
			Mb = samplecount - 1 + (Int32) GlobalMembersUtil.roundoff(5.0/ freq[1]-freq[0]); // linear mode
		} else {
			Mb = samplecount - 1 + (Int32) GlobalMembersUtil.roundoff(2.0 *5.0/((freq[0] * Math.Pow(LOGBASE, -1.0/(bpo))) * (1.0 - Math.Pow(LOGBASE, -1.0 / bpo))));
		}

		if (Mb % 2 == 1) // if Mb is odd
			Mb++; // make it even (for the sake of simplicity)

		Mb = (int) GlobalMembersUtil.roundoff((double) GlobalMembersUtil.nextsprime((Int32) GlobalMembersUtil.roundoff(Mb * pixpersec)) / pixpersec);

		Md = (int) GlobalMembersUtil.roundoff(Mb * pixpersec);
		
		// PIN: Make sure the Mb is a power of two
		// TODO: Check http://www.mathworks.se/support/tech-notes/1700/1702.html
		//Mb = (int)MathUtils.NextPowerOfTwo((uint)Mb);

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
		//s = realloc (s, Mb * sizeof(double));
		// realloc to the zeropadded size
		Array.Resize<double>(ref s, Mb);
		
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		//memset(s[samplecount], 0, (Mb-samplecount) * sizeof(double));
		// zero-out the padded area. Equivalent of : for (i=samplecount; i<Mb; i++) s[i] = 0;
		for (i=samplecount; i<Mb; i++) s[i] = 0;
		//--------ZEROPADDING--------
		
		GlobalMembersDsp.fft(s, s, Mb, 0); // In-place FFT of the original zero-padded signal

		for (ib = 0; ib<bands; ib++)
		{
			//********Filtering********
			Fa = (int) GlobalMembersUtil.roundoff(GlobalMembersDsp.log_pos((double)(ib-1)/(double)(bands-1), basefreq, maxfreq) * Mb);
			Fd = (int) GlobalMembersUtil.roundoff(GlobalMembersDsp.log_pos((double)(ib+1)/(double)(bands-1), basefreq, maxfreq) * Mb);
			La = GlobalMembersDsp.log_pos_inv((double) Fa / (double) Mb, basefreq, maxfreq);
			Ld = GlobalMembersDsp.log_pos_inv((double) Fd / (double) Mb, basefreq, maxfreq);

			if (Fd > Mb/2)
				Fd = Mb/2; // stop reading if reaching the Nyquist frequency

			if (Fa < 1)
				Fa = 1;

			Mc = (Fd-Fa)*2 + 1;
			// '*2' because the filtering is on both real and imaginary parts,
			// '+1' for the DC.
			// No Nyquist component since the signal length is necessarily odd

			if (Md > Mc) // if the band is going to be too narrow
				Mc = Md;

			if (Md < Mc) // round the larger bands up to the next integer made of 2^n * 3^m
				Mc = GlobalMembersUtil.nextsprime(Mc);

			Console.Write("{0,4:D}/{1:D} (FFT size: {2,6:D})   {3:f2} Hz - {4:f2} Hz\r", ib+1, bands, Mc, (double) Fa *samplerate/Mb, (double) Fd *samplerate/Mb);

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
			//@out[bands-ib-1]   calloc(Mc, sizeof(double)); // allocate new band
			@out[bands-ib-1] = new double[Mc];

			for (i = 0; i<Fd-Fa; i++)
			{
				Li = GlobalMembersDsp.log_pos_inv((double)(i+Fa) / (double) Mb, basefreq, maxfreq); // calculation of the logarithmic position
				Li = (Li-La)/(Ld-La);
				coef = 0.5 - 0.5 * Math.Cos(2.0 *pi * Li); // Hann function
				
				@out[bands-ib-1][i+1] = s[i+1+Fa] * coef;
				@out[bands-ib-1][Mc-1-i] = s[Mb-Fa-1-i] * coef;
			}
			//--------Filtering--------
			Export.exportCSV(String.Format("test/band_{0}_filtered.csv", bands-ib-1), @out[bands-ib-1]);

			//********90° rotation********

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
			//h = calloc(Mc, sizeof(double)); // allocate the 90° rotated version of the band
			h = new double[Mc];
			
			// Rotation : Re' = Im; Im' = -Re
			for (i = 0; i<Fd-Fa; i++)
			{
				h[i+1] = @out[bands-ib-1][Mc-1-i]; 	// Re' = Im
				h[Mc-1-i] = -@out[bands-ib-1][i+1]; // Im' = -Re
			}
			//--------90° rotation--------

			//********Envelope detection********
			Export.exportCSV(String.Format("test/band_{0}_rotated.csv", bands-ib-1), @out[bands-ib-1]);

			GlobalMembersDsp.fft(@out[bands-ib-1], @out[bands-ib-1], Mc, 1); 	// In-place IFFT of the filtered band signal
			GlobalMembersDsp.fft(h, h, Mc, 1); 									// In-place IFFT of the filtered band signal rotated by 90°

			Export.exportCSV(String.Format("test/band_{0}_before.csv", bands-ib-1), @out[bands-ib-1]);

			for (i = 0; i<Mc; i++) {
				// Magnitude of the analytic signal
				double x = @out[bands-ib-1][i];
				double y = h[i];
				double xSquared = x*x;
				double ySquared = y*y;
				if (double.IsInfinity(xSquared)) {
					xSquared = 0.0;
				}
				if (double.IsInfinity(ySquared)) {
					ySquared = 0.0;
				}
				double mag = Math.Sqrt(xSquared + ySquared);
				@out[bands-ib-1][i] = mag;
			}

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			//free(h);
			//--------Envelope detection--------

			//********Downsampling********
			if (Mc < Md) // if the band doesn't have to be resampled
			{
				//@out[bands-ib-1] = realloc(@out[bands-ib-1], Md * sizeof(double)); // simply ignore the end of it
				Array.Resize<double>(ref @out[bands-ib-1], Md); // simply ignore the end of it
			}
			
			if (Mc > Md) // If the band *has* to be downsampled
			{
				//t = @out[bands-ib-1];
				@out[bands-ib-1] = GlobalMembersDsp.blackman_downsampling(@out[bands-ib-1], Mc, Md); // Blackman downsampling

				//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
				//free(t);
			}
			//--------Downsampling--------

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
			//@out[bands-ib-1] = realloc(@out[bands-ib-1], Xsize * sizeof(double));
			Array.Resize<double>(ref @out[bands-ib-1], Xsize); // Tail chopping
			
			Export.exportCSV(String.Format("test/band_{0}_after.csv", bands-ib-1), @out[bands-ib-1]);
		}

		Console.Write("\n");

		GlobalMembersDsp.normi(@out, Xsize, bands, 1.0);
		return @out;
	}

	public static double[] wsinc_max(Int32 length, double bw)
	{
		Int32 i = new Int32();
		Int32 bwl = new Int32(); // integer transition bandwidth
		double tbw; // double transition bandwidth
		double[] h; // kernel
		double x; // position in the antiderivate of the Blackman function of the sample we're at
		double coef; // coefficient obtained from the function

		tbw = bw * (double)(length-1);
		bwl = GlobalMembersUtil.roundup(tbw);
		
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//h = calloc (length, sizeof(double));
		h = new double[length];

		for (i = 1; i<length; i++)
			h[i] = 1.0;

		for (i = 0; i<bwl; i++)
		{
			x = (double) i / tbw; // position calculation between 0.0 and 1.0
			coef = 0.42 *x - (0.5/(2.0 *pi))*Math.Sin(2.0 *pi *x) + (0.08/(4.0 *pi))*Math.Sin(4.0 *pi *x); // antiderivative of the Blackman function
			coef *= 1.0/0.42;
			h[i+1] = coef;
			h[length-1-i] = coef;
		}

		return h;
	}
	
	public static double[] synt_sine(double[][] d, Int32 Xsize, Int32 bands, Int32 samplecount, Int32 samplerate, double basefreq, double pixpersec, double bpo)
	{
		double[] s;
		double[] freq;
		double[] filter;
		double[] sband;
		double[] sine = new double[4];
		double rphase;
		Int32 i = new Int32();
		Int32 ib = new Int32();
		Int32 Fc = new Int32();
		Int32 Bc = new Int32();
		Int32 Mh = new Int32();
		Int32 Mn = new Int32();
		Int32 sbsize = new Int32();

		/*
		 d is the original image (spectrogram)
		 s is the output sound
		 sband is the band's envelope upsampled and shifted up in frequency
		 sbsize is the length of sband
		 sine is the random sine look-up table
		 *samplecount is the output sound's length
		 ib is the band iterator
		 i is a general purpose iterator
		 bands is the total count of bands
		 Fc is the index of the band's centre in the frequency domain on the new signal
		 Bc is the index of the band's centre in the frequency domain on sband (its imaginary match being sbsize-Bc)
		 Mh is the length of the real or imaginary part of the envelope's FFT, DC element included and Nyquist element excluded
		 Mn is the length of the real or imaginary part of the sound's FFT, DC element included and Nyquist element excluded
		 freq is the band's central frequency
		 rphase is the band's sine's random phase
		 */

		freq = GlobalMembersDsp.freqarray(basefreq, bands, bpo);

		clocka = GlobalMembersUtil.gettime();

		sbsize = GlobalMembersUtil.nextsprime(Xsize * 2); // In Circular mode keep it to sbsize = Xsize * 2;

		samplecount = (int) GlobalMembersUtil.roundoff(Xsize/pixpersec);
		Console.Write("Sound duration : {0:f3} s\n", (double) samplecount/samplerate);
		samplecount = (int) GlobalMembersUtil.roundoff(0.5 *sbsize/pixpersec); // Do not change this value as it would stretch envelopes

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//s = calloc(samplecount, sizeof(double)); // allocation of the sound signal
		s = new double[samplecount];
		
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		//sband = malloc (sbsize * sizeof(double)); // allocation of the shifted band
		sband = new double[sbsize];

		Bc = (int) GlobalMembersUtil.roundoff(0.25 * (double) sbsize);

		Mh = (sbsize + 1) >> 1;
		Mn = (samplecount + 1) >> 1;

		filter = GlobalMembersDsp.wsinc_max(Mh, 1.0 / GlobalMembersArss.TRANSITION_BW_SYNT); // generation of the frequency-domain filter

		for (ib = 0; ib<bands; ib++)
		{
			//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
			//memset(sband, 0, sbsize * sizeof(double)); // reset sband

			//********Frequency shifting********
			rphase = GlobalMembersUtil.dblrand() * pi; // random phase between -pi and +pi

			for (i = 0; i<4; i++) // generating the random sine LUT
				sine[i] = Math.Cos(i *2.0 *pi *0.25 + rphase);

			for (i = 0; i<Xsize; i++) // envelope sampling rate * 2 and frequency shifting by 0.25
			{
				if ((i & 1) == 0)
				{
					sband[i<<1] = d[bands-ib-1][i] * sine[0];
					sband[(i<<1) + 1] = d[bands-ib-1][i] * sine[1];
				}
				else
				{
					sband[i<<1] = d[bands-ib-1][i] * sine[2];
					sband[(i<<1) + 1] = d[bands-ib-1][i] * sine[3];
				}
			}
			//--------Frequency shifting--------

			GlobalMembersDsp.fft(sband, sband, sbsize, 0); // FFT of the envelope
			Fc = (int) GlobalMembersUtil.roundoff(freq[ib] * samplecount); // band's centre index (envelope's DC element)

			Console.Write("{0,4:D}/{1:D}   {2:f2} Hz\r", ib+1, bands, (double) Fc *samplerate / samplecount);

			//********Write FFT********
			for (i = 1; i<Mh; i++)
			{
				if (Fc-Bc+i > 0 && Fc-Bc+i < Mn) // if we're between frequencies 0 and 0.5 of the new signal and that we're not at Fc
				{
					s[i+Fc-Bc] += sband[i] * filter[i]; 						// Real part
					s[samplecount-(i+Fc-Bc)] += sband[sbsize-i] * filter[i]; 	// Imaginary part
				}
			}
			//--------Write FFT--------
		}

		Console.Write("\n");

		GlobalMembersDsp.fft(s, s, samplecount, 1); // IFFT of the final sound
		samplecount = (int) GlobalMembersUtil.roundoff(Xsize/pixpersec); // chopping tails by ignoring them

		GlobalMembersDsp.normi(s, samplecount, 1.0);

		return s;
	}
	
	public static double[] synt_noise(double[][] d, Int32 Xsize, Int32 bands, Int32 samplecount, Int32 samplerate, double basefreq, double pixpersec, double bpo)
	{
		Int32 i = new Int32(); 					// general purpose iterator
		Int32 ib = new Int32(); 				// bands iterator
		Int32 il = new Int32(); 				// loop iterator
		double[] s; 							// final signal
		double coef;
		double[] noise; 						// filtered looped noise
		double loop_size_sec = LOOP_SIZE_SEC; 	// size of the filter bank loop, in seconds. Later to be taken from user input
		Int32 loop_size = new Int32(); 			// size of the filter bank loop, in samples. Deduced from loop_size_sec
		Int32 loop_size_min = new Int32(); 		// minimum required size for the filter bank loop, in samples. Calculated from the longest windowed sinc's length
		double[] pink_noise; 					// original pink noise (in the frequency domain)
		double mag; 							// parameters for the creation of pink_noise's samples
		double phase;
		double[] envelope; 	// interpolated envelope
		double[] lut; 		// Blackman Sqaure look-up table
		double[] freq; 		// frequency look-up table
		double maxfreq; 	// central frequency of the last band
		
		Int32 Fa = new Int32(); // Fa is the index of the band's start in the frequency domain
		Int32 Fd = new Int32(); // Fd is the index of the band's end in the frequency domain
		
		double La; // La is the log2 of the frequency of Fa
		double Ld; // Ld is the log2 of the frequency of Fd
		double Li; // Li is the iterative frequency between La and Ld defined logarithmically

		freq = GlobalMembersDsp.freqarray(basefreq, bands, bpo);

		if (LOGBASE == 1.0)
			maxfreq = bpo; // in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
		else
			maxfreq = basefreq * Math.Pow(LOGBASE, ((double)(bands-1)/ bpo));

		clocka = GlobalMembersUtil.gettime();

		samplecount = (int) GlobalMembersUtil.roundoff(Xsize/pixpersec); // calculation of the length of the final signal
		Console.Write("Sound duration : {0:f3} s\n", (double) samplecount/samplerate);

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//s = calloc (samplecount, sizeof(double)); // allocation of the final signal
		s = new double[samplecount];
		
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//envelope = calloc (samplecount, sizeof(double)); // allocation of the interpolated envelope
		envelope = new double[samplecount];

		//********Loop size calculation********

		loop_size = (int) loop_size_sec * samplerate;

		if (LOGBASE == 1.0)
			loop_size_min = (Int32) GlobalMembersUtil.roundoff(4.0 *5.0/ freq[1]-freq[0]); // linear mode
		else
			loop_size_min = (Int32) GlobalMembersUtil.roundoff(2.0 *5.0/((freq[0] * Math.Pow(2.0, -1.0/(bpo))) * (1.0 - Math.Pow(2.0, -1.0 / bpo)))); // this is the estimate of how many samples the longest FIR will take up in the time domain

		if (loop_size_min > loop_size)
			loop_size = loop_size_min;

		loop_size = GlobalMembersUtil.nextsprime(loop_size); // enlarge the loop_size to the next multiple of short primes in order to make IFFTs faster
		//--------Loop size calculation--------

		//********Pink noise generation********

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		//pink_noise = calloc (loop_size, sizeof(double));
		pink_noise = new double[loop_size];

		for (i = 1; i<(loop_size+1)>>1; i++)
		{
			mag = Math.Pow((double) i, 0.5 - 0.5 *LOGBASE); // FIXME something's not necessarily right with that formula
			phase = GlobalMembersUtil.dblrand() * pi; // random phase between -pi and +pi

			pink_noise[i] = mag * Math.Cos(phase); // real part
			pink_noise[loop_size-i] = mag * Math.Sin(phase); // imaginary part
		}
		//--------Pink noise generation--------

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		//noise = malloc(loop_size * sizeof(double)); // allocate noise
		noise = new double[loop_size];
		
		lut = GlobalMembersDsp.bmsq_lut(BMSQ_LUT_SIZE); // Blackman Square look-up table initalisation

		for (ib = 0; ib<bands; ib++)
		{
			Console.Write("{0,4:D}/{1:D}\r", ib+1, bands);

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
			//memset(noise, 0, loop_size * sizeof(double)); // reset filtered noise

			//********Filtering********

			Fa = (int) GlobalMembersUtil.roundoff(GlobalMembersDsp.log_pos((double)(ib-1)/(double)(bands-1), basefreq, maxfreq) * loop_size);
			Fd = (int) GlobalMembersUtil.roundoff(GlobalMembersDsp.log_pos((double)(ib+1)/(double)(bands-1), basefreq, maxfreq) * loop_size);
			La = GlobalMembersDsp.log_pos_inv((double) Fa / (double) loop_size, basefreq, maxfreq);
			Ld = GlobalMembersDsp.log_pos_inv((double) Fd / (double) loop_size, basefreq, maxfreq);

			if (Fd > loop_size/2)
				Fd = loop_size/2; // stop reading if reaching the Nyquist frequency

			if (Fa<1)
				Fa = 1;

			Console.Write("{0,4:D}/{1:D}   {2:f2} Hz - {3:f2} Hz\r", ib+1, bands, (double) Fa *samplerate/loop_size, (double) Fd *samplerate/loop_size);

			for (i = Fa; i<Fd; i++)
			{
				Li = GlobalMembersDsp.log_pos_inv((double) i / (double) loop_size, basefreq, maxfreq); // calculation of the logarithmic position
				Li = (Li-La)/(Ld-La);
				coef = 0.5 - 0.5 *Math.Cos(2.0 *pi *Li); // Hann function
				noise[i+1] = pink_noise[i+1] * coef;
				noise[loop_size-1-i] = pink_noise[loop_size-1-i] * coef;
			}
			//--------Filtering--------

			GlobalMembersDsp.fft(noise, noise, loop_size, 1); // IFFT of the filtered noise

			//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
			//memset(envelope, 0, samplecount * sizeof(double)); // blank the envelope
			GlobalMembersDsp.blackman_square_interpolation(d[bands-ib-1], envelope, Xsize, samplecount, lut, BMSQ_LUT_SIZE); // interpolation of the envelope

			il = 0;
			for (i = 0; i< samplecount; i++)
			{
				s[i] += envelope[i] * noise[il]; // modulation
				il++; // increment loop iterator
				if (il == loop_size) // if the array iterator has reached the end of the array, it's reset
					il = 0;
			}
		}

		Console.Write("\n");

		GlobalMembersDsp.normi(s, samplecount, 1.0);

		return s;
	}
	
	public static void brightness_control(double[][] image, Int32 width, Int32 height, double ratio)
	{
		// Actually this is based on the idea of converting values to decibels, for example, 0.01 becomes -40 dB, dividing them by ratio, so if ratio is 2 then -40 dB/2 = -20 dB, and then turning it back to regular values, so -20 dB becomes 0.1
		// If ratio==2 then this function is equivalent to a square root
		// 1/ratio is used for the forward transformation
		// ratio is used for the reverse transformation

		Int32 ix = new Int32();
		Int32 iy = new Int32();

		for (iy = 0; iy<width; iy++)
			for (ix = 0; ix<height; ix++)
				image[iy][ix] = Math.Pow(image[iy][ix], ratio);
	}

	// turns a logarithmic position (i.e. band number/band count) to a frequency
	public static double log_pos(double x, double min, double max)
	{
		if (LOGBASE == 1.0)
			return x*(max-min) + min;
		else
			return (max-min) * (min * Math.Pow(LOGBASE, x * (Math.Log(max)-Math.Log(min))/Math.Log(2.0)) - min) / (min * Math.Pow(LOGBASE, (Math.Log(max)-Math.Log(min))/Math.Log(2.0)) - min) + min;
	}

	// turns a frequency to a logarithmic position (i.e. band number/band count)
	public static double log_pos_inv(double x, double min, double max)
	{
		if (LOGBASE == 1.0)
			return (x - min)/(max-min);
		else
			return Math.Log(((x-min) * (min * Math.Pow(LOGBASE, (Math.Log(max) - Math.Log(min))/Math.Log(2.0)) - min) / (max-min) + min) / Math.Log(LOGBASE)) * Math.Log(2.0) / (Math.Log(max) - Math.Log(min));
	}
}
