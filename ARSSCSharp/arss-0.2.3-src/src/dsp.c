#include "dsp.h"

void fft(double *in, double *out, int32_t N, uint8_t method)
{
	/* method :
	 * 0 = DFT
	 * 1 = IDFT
	 * 2 = DHT
	 */

	fftw_plan p = fftw_plan_r2r_1d(N, in, out, method, FFTW_ESTIMATE);
	fftw_execute(p);
	fftw_destroy_plan(p);
}

void normi(double **s, int32_t xs, int32_t ys, double ratio)	// normalises a signal to the +/-ratio range
{
	int32_t ix, iy, maxx, maxy;
	double max;

	max=0;
	for (iy=0; iy<ys; iy++)
		for (ix=0; ix<xs; ix++)
			if (fabs(s[iy][ix])>max)
			{
				max=fabs(s[iy][ix]);
				maxx=ix;
				maxy=iy;
			}

	#ifdef DEBUG
	printf("norm : %.3f (Y:%i X:%i)\n", max, maxy, maxx);
	#endif
	
	if (max!=0.0)
	{
		max /= ratio;
		max = 1.0/max;
	}
	else
		max = 0.0;

	for (iy=0; iy<ys; iy++)
		for (ix=0; ix<xs; ix++)
			s[iy][ix]*=max;

	#ifdef DEBUG
	printf("ex : %.3f\n", s[0][0]);
	#endif
}

double log_pos(double x, double min, double max)	// turns a logarithmic position (i.e. band number/band count) to a frequency
{
	if (LOGBASE==1.0)
		return x*(max-min) + min;
	else
		return (max-min) * (min * pow(LOGBASE, x * (log(max)-log(min))/log(2.0)) - min) / (min * pow(LOGBASE, (log(max)-log(min))/log(2.0)) - min) + min;
}

double log_pos_inv(double x, double min, double max)	// turns a frequency to a logarithmic position (i.e. band number/band count)
{
	if (LOGBASE==1.0)
		return (x - min)/(max-min);
	else
		return log(((x-min) * (min * pow(LOGBASE, (log(max) - log(min))/log(2.0)) - min) / (max-min) + min) / log(LOGBASE)) * log(2.0) / (log(max) - log(min));
}

double *freqarray(double basefreq, int32_t bands, double bandsperoctave)
{
	int32_t i;
	double *freq, maxfreq;

	freq=malloc (sizeof(double) * bands);

	if (LOGBASE==1.0)
		maxfreq = bandsperoctave;	// in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
	else
		maxfreq = basefreq * pow(LOGBASE, ((double) (bands-1)/ bandsperoctave));

	for (i=0;i<bands;i++)
	{
		freq[i] = log_pos((double) i/(double) (bands-1), basefreq, maxfreq);	//band's central freq
	}
	if (log_pos((double) bands / (double) (bands-1), basefreq, maxfreq)>0.5)
		printf("Warning: Upper frequency limit above Nyquist frequency\n");	// TODO change sampling rate instead

	return freq;
}

double *blackman_downsampling(double *in, int32_t Mi, int32_t Mo)		// Downsampling of the signal by a Blackman function
{
	int32_t i, j;		// general purpose iterators
	double *out;
	double pos_in;		// position in the original signal
	double x;		// position of the iterator in the blackman(x) formula
	double ratio;		// scaling ratio (> 1.0)
	double ratio_i;		// ratio^-1
	double coef;		// Blackman coefficient
	double coef_sum;	// sum of coefficients, used for weighting

	/*
	 * Mi is the original signal's length
	 * Mo is the output signal's length
	 */

	ratio = (double) Mi/Mo;
	ratio_i = 1.0/ratio;

	out = calloc (Mo, sizeof(double));

	for (i=0; i<Mo; i++)
	{
		pos_in = (double) i * ratio;

		coef_sum = 0;

		for (j=roundup(pos_in - ratio); j<=pos_in + ratio; j++)
		{
			if (j>=0 && j<Mi)			// if the read sample is within bounds
			{
				x = j - pos_in + ratio;		// calculate position within the Blackman function
				coef = 0.42 - 0.5*cos(pi * x * ratio_i) + 0.08*cos(2*pi * x * ratio_i);
				coef_sum += coef;
				out[i] += in[j] * coef;		// convolve
			}
		}

		out[i] /= coef_sum;
	}

	return out;
}

double *bmsq_lut(int32_t size)	// Blackman Square look-up table generator
{
	int32_t i;					// general purpose iterator
	double coef;					// Blackman square final coefficient
	double bar = pi * (3.0 / (double) size) * (1.0/1.5);
	double foo;

	double f1 = -0.6595044010905501;		// Blackman Square coefficients
	double f2 =  0.1601741366715479;
	double f4 = -0.0010709178680006;
	double f5 =  0.0001450093579222;
	double f7 =  0.0001008528049040;
	double f8 =  0.0000653092892874;
	double f10 = 0.0000293385615146;
	double f11 = 0.0000205351559060;
	double f13 = 0.0000108567682890;
	double f14 = 0.0000081549460136;
	double f16 = 0.0000048519309366;
	double f17 = 0.0000038284344102;
	double f19 = 0.0000024753630724;

	size++;		// allows to read value 3.0

	double *lut = calloc (size, sizeof(double));

	for (i=0; i<size; i++)
	{
		foo = (double) i * bar;
		coef = 0;

		coef += cos(       foo) * f1  - f1;
		coef += cos( 2.0 * foo) * f2  - f2;
		coef += cos( 4.0 * foo) * f4  - f4;
		coef += cos( 5.0 * foo) * f5  - f5;
		coef += cos( 7.0 * foo) * f7  - f7;
		coef += cos( 8.0 * foo) * f8  - f8;
		coef += cos(10.0 * foo) * f10 - f10;
		coef += cos(11.0 * foo) * f11 - f11;
		coef += cos(13.0 * foo) * f13 - f13;
		coef += cos(14.0 * foo) * f14 - f14;
		coef += cos(16.0 * foo) * f16 - f16;
		coef += cos(17.0 * foo) * f17 - f17;
		coef += cos(19.0 * foo) * f19 - f19;

		lut[i] = coef;
	}

	return lut;
}

void blackman_square_interpolation(double *in, double *out, int32_t Mi, int32_t Mo, double *lut, int32_t lut_size)	// Interpolation based on an estimate of the Blackman Square function, which is a Blackman function convolved with a square. It's like smoothing the result of a nearest neighbour interpolation with a Blackman FIR
{
	int32_t i, j;		// general purpose iterators
	double pos_in;		// position in the original signal
	double x;		// position of the iterator in the blackman_square(x) formula
	double ratio;		// scaling ratio (> 1.0)
	double ratio_i;		// ratio^-1
	double coef;		// Blackman square final coefficient
	double pos_lut;		// Index on the look-up table
	int32_t pos_luti;	// Integer index on the look-up table
	double mod_pos;		// modulo of the position on the look-up table
	double y0, y1;		// values of the two closest values on the LUT
	double foo = (double) lut_size / 3.0;
	int32_t j_start, j_stop;	// boundary values for the j loop

	/*
	 * Mi is the original signal's length
	 * Mo is the output signal's length
	 */

	ratio = (double) Mi/Mo;
	ratio_i = 1.0/ratio;

	for (i=0; i<Mo; i++)
	{
		pos_in = (double) i * ratio;

		j_stop = pos_in + 1.5;

		j_start = j_stop - 2;
		if (j_start<0)
			j_start=0;

		if (j_stop >= Mi)		// The boundary check is done after j_start is calculated to avoid miscalculating it
			j_stop = Mi - 1;

		for (j=j_start; j<=j_stop; j++)
		{
			x = j - pos_in + 1.5;			// calculate position within the Blackman square function in the [0.0 ; 3.0] range
			pos_lut = x * foo;
			pos_luti = (int32_t) pos_lut;

			mod_pos = myfmod(pos_lut, 1.0);		// modulo of the index

			y0 = lut[pos_luti];		// interpolate linearly between the two closest values
			y1 = lut[pos_luti + 1];
			coef = y0 + mod_pos * (y1 - y0);	// linear interpolation

			out[i] += in[j] * coef;	// convolve
		}
	}
}

double **anal(double *s, int32_t samplecount, int32_t samplerate, int32_t *Xsize, int32_t bands, double bpo, double pixpersec, double basefreq)
{
	int32_t i, ib, Mb, Mc, Md, Fa, Fd;
	double **out, *h, *freq, *t, coef, La, Ld, Li, maxfreq;

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
	
	freq = freqarray(basefreq, bands, bpo);
	
	#ifdef DEBUG	
	log_double_array("freq.csv", freq, bands);
	#endif

	if (LOGBASE==1.0)
		maxfreq = bpo;	// in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
	else
		maxfreq = basefreq * pow(LOGBASE, ((double) (bands-1)/ bpo));

	#ifdef DEBUG	
	log_double("Maxfreq", maxfreq);		
	log_double("bpo", bpo);		
	log_double("pixpersec", pixpersec);
	log_double("basefreq", basefreq);
	log_int32_t("bands", bands);
	log_int32_t("samplecount", samplecount);
	log_int32_t("samplerate", samplerate);
	#endif
	
	*Xsize = samplecount * pixpersec;

	#ifdef DEBUG	
	log_int32_t("Xsize", *Xsize);
	#endif
	
	if (myfmod((double) samplecount * pixpersec, 1.0) != 0.0)		// round-up
		(*Xsize)++;
		
	printf("Image size : %dx%d\n", *Xsize, bands);
	out = malloc (bands * sizeof(double *));

	clocka=gettime();

	//********ZEROPADDING********	Note : Don't do it in Circular mode

	if (LOGBASE==1.0)
		Mb = samplecount - 1 + (int32_t) roundoff(5.0/ freq[1]-freq[0]);	// linear mode
	else
		Mb = samplecount - 1 + (int32_t) roundoff(2.0*5.0/((freq[0] * pow(LOGBASE, -1.0/(bpo))) * (1.0 - pow(LOGBASE, -1.0 / bpo))));

	#ifdef DEBUG
	log_int32_t("Mb", Mb);
	#endif
	
	if (Mb % 2 == 1)	// if Mb is odd
		Mb++;		// make it even (for the sake of simplicity)

	#ifdef DEBUG	
	log_int32_t("Mb", Mb);
	#endif
		
	Mb = roundoff((double) nextsprime((int32_t) roundoff(Mb * pixpersec)) / pixpersec);

	#ifdef DEBUG	
	log_int32_t("Mb", Mb);
	#endif
	
	Md = roundoff(Mb * pixpersec);

	#ifdef DEBUG	
	log_int32_t("Md", Md);
	#endif

	s = realloc (s, Mb * sizeof(double));	// realloc to the zeropadded size
	memset(&s[samplecount], 0, (Mb-samplecount) * sizeof(double));	// zero-out the padded area. Equivalent of : for (i=samplecount; i<Mb; i++) s[i] = 0;
	//--------ZEROPADDING--------

	#ifdef DEBUG
	log_double_array("samples-before.csv", s, Mb * sizeof(double));
	#endif
	
	fft(s, s, Mb, 0);			// In-place FFT of the original zero-padded signal

	#ifdef DEBUG
	log_double_array("samples-after.csv", s, Mb * sizeof(double));
	#endif
	
	for (ib=0; ib<bands; ib++)
	{
		//********Filtering********
		Fa = roundoff(log_pos((double) (ib-1)/(double) (bands-1), basefreq, maxfreq) * Mb);
		Fd = roundoff(log_pos((double) (ib+1)/(double) (bands-1), basefreq, maxfreq) * Mb);
		La = log_pos_inv((double) Fa / (double) Mb, basefreq, maxfreq);
		Ld = log_pos_inv((double) Fd / (double) Mb, basefreq, maxfreq);

		#ifdef DEBUG
		log_int32_t("Fa", Fa);
		log_int32_t("Fd", Fd);
		log_double("La", La);
		log_double("Ld", Ld);		
		#endif
		
		if (Fd > Mb/2)
			Fd = Mb/2;	// stop reading if reaching the Nyquist frequency

		if (Fa<1)
			Fa=1;

		Mc = (Fd-Fa)*2 + 1;	// '*2' because the filtering is on both real and imaginary parts, '+1' for the DC. No Nyquist component since the signal length is necessarily odd

		#ifdef DEBUG
		log_int32_t("Mc", Mc);
		#endif

		if (Md>Mc)					// if the band is going to be too narrow
			Mc = Md;

		if (Md<Mc)					// round the larger bands up to the next integer made of 2^n * 3^m
			Mc = nextsprime(Mc);

		#ifdef DEBUG
		log_int32_t("Mc", Mc);
		#endif

		printf("%4d/%d (FFT size: %6d)   %.2f Hz - %.2f Hz\r", ib+1, bands, Mc, (double) Fa*samplerate/Mb, (double) Fd*samplerate/Mb);

		out[bands-ib-1] = calloc(Mc, sizeof(double));	// allocate new band

		for (i=0; i<Fd-Fa; i++)
		{
			Li = log_pos_inv((double) (i+Fa) / (double) Mb, basefreq, maxfreq);	// calculation of the logarithmic position
			Li = (Li-La)/(Ld-La);
			coef = 0.5 - 0.5*cos(2.0*pi*Li);		// Hann function
			out[bands-ib-1][i+1] 	= s[i+1+Fa] * coef;
			out[bands-ib-1][Mc-1-i] = s[Mb-Fa-1-i] * coef;
		}
		//--------Filtering--------

		//********90° rotation********

		h = calloc(Mc, sizeof(double));			// allocate the 90° rotated version of the band
		// Rotation : Re' = Im; Im' = -Re

		for (i=0; i<Fd-Fa; i++)
		{
			h[i+1] = out[bands-ib-1][Mc-1-i];	// Re' = Im
			h[Mc-1-i] = -out[bands-ib-1][i+1];	// Im' = -Re
		}
		//--------90° rotation--------

		//********Envelope detection********

		fft(out[bands-ib-1], out[bands-ib-1], Mc, 1);		// In-place IFFT of the filtered band signal
		fft(h, h, Mc, 1);					// In-place IFFT of the filtered band signal rotated by 90°

		for (i=0; i<Mc; i++)
			out[bands-ib-1][i] = sqrt(out[bands-ib-1][i]*out[bands-ib-1][i] + h[i]*h[i]);	// Magnitude of the analytic signal

		free(h);
		//--------Envelope detection--------

		//********Downsampling********

		if (Mc < Md)									// if the band doesn't have to be resampled
			out[bands-ib-1] = realloc(out[bands-ib-1], Md * sizeof(double));	// simply ignore the end of it

		if (Mc > Md)	// If the band *has* to be downsampled
		{
			t = out[bands-ib-1];

			out[bands-ib-1] = blackman_downsampling(out[bands-ib-1], Mc, Md);	// Blackman downsampling

			free(t);
		}
		//--------Downsampling--------

		out[bands-ib-1] = realloc(out[bands-ib-1], *Xsize * sizeof(double));		// Tail chopping
	}

	printf("\n");

	normi(out, *Xsize, bands, 1.0);
	return out;
}

double *wsinc_max(int32_t length, double bw)
{
	int32_t i;
	int32_t bwl;	// integer transition bandwidth
	double tbw;	// double transition bandwidth
	double *h;	// kernel
	double x;	// position in the antiderivate of the Blackman function of the sample we're at
	double coef;	// coefficient obtained from the function

	tbw = bw * (double) (length-1);
	bwl = roundup(tbw);
	h = calloc (length, sizeof(double));

	for (i=1; i<length; i++)
		h[i] = 1.0;

	for (i=0; i<bwl; i++)
	{
		x = (double) i / tbw;								// position calculation between 0.0 and 1.0
		coef = 0.42*x - (0.5/(2.0*pi))*sin(2.0*pi*x) + (0.08/(4.0*pi))*sin(4.0*pi*x);	// antiderivative of the Blackman function
		coef *= 1.0/0.42;
		h[i+1] = coef;
		h[length-1-i] = coef;
	}

	return h;
}

double *synt_sine(double **d, int32_t Xsize, int32_t bands, int32_t *samplecount, int32_t samplerate, double basefreq, double pixpersec, double bpo)
{
	double *s, *freq, *filter, *sband, sine[4], rphase;
	int32_t i, ib;
	int32_t Fc, Bc, Mh, Mn, sbsize;

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

	freq = freqarray(basefreq, bands, bpo);

	clocka=gettime();

	sbsize = nextsprime(Xsize * 2);				// In Circular mode keep it to sbsize = Xsize * 2;
	
	*samplecount = roundoff(Xsize/pixpersec);
	printf("Sound duration : %.3f s\n", (double) *samplecount/samplerate);
	*samplecount = roundoff(0.5*sbsize/pixpersec);		// Do not change this value as it would stretch envelopes
	
	s = calloc(*samplecount, sizeof(double));		// allocation of the sound signal
	sband = malloc (sbsize * sizeof(double));		// allocation of the shifted band

	Bc = roundoff(0.25 * (double) sbsize);

	Mh = (sbsize + 1) >> 1;
	Mn = (*samplecount + 1) >> 1;

	filter = wsinc_max(Mh, 1.0/TRANSITION_BW_SYNT);		// generation of the frequency-domain filter

	for (ib=0; ib<bands; ib++)
	{
		memset(sband, 0, sbsize * sizeof(double));	// reset sband

		//********Frequency shifting********

		rphase = dblrand() * pi;			// random phase between -pi and +pi

		for (i=0; i<4; i++)				// generating the random sine LUT
			sine[i]=cos(i*2.0*pi*0.25 + rphase);

		for (i=0; i<Xsize; i++)				// envelope sampling rate * 2 and frequency shifting by 0.25
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

		fft(sband, sband, sbsize, 0);			// FFT of the envelope
		Fc = roundoff(freq[ib] * *samplecount);	// band's centre index (envelope's DC element)

		printf("%4d/%d   %.2f Hz\r", ib+1, bands, (double) Fc*samplerate / *samplecount);

		//********Write FFT********

		for (i=1; i<Mh; i++)
		{
			if (Fc-Bc+i > 0 && Fc-Bc+i < Mn)	// if we're between frequencies 0 and 0.5 of the new signal and that we're not at Fc
			{
				s[i+Fc-Bc] += sband[i] * filter[i];				// Real part
				s[*samplecount-(i+Fc-Bc)] += sband[sbsize-i] * filter[i]; 	// Imaginary part
			}
		}
		//--------Write FFT--------
	}

	printf("\n");

	fft(s, s, *samplecount, 1);			// IFFT of the final sound
	*samplecount = roundoff(Xsize/pixpersec);	// chopping tails by ignoring them

	normi(&s, *samplecount, 1, 1.0);

	return s;
}

double *synt_noise(double **d, int32_t Xsize, int32_t bands, int32_t *samplecount, int32_t samplerate, double basefreq, double pixpersec, double bpo)
{
	int32_t	i;			// general purpose iterator
	int32_t	ib;			// bands iterator
	int32_t	il;			// loop iterator
	double	*s;			// final signal
	double	coef;
	double	*noise;			// filtered looped noise
	double	loop_size_sec=LOOP_SIZE_SEC;	// size of the filter bank loop, in seconds. Later to be taken from user input
	int32_t	loop_size;		// size of the filter bank loop, in samples. Deduced from loop_size_sec
	int32_t	loop_size_min;		// minimum required size for the filter bank loop, in samples. Calculated from the longest windowed sinc's length
	double	*pink_noise;		// original pink noise (in the frequency domain)
	double	mag, phase;		// parameters for the creation of pink_noise's samples
	double	*envelope;		// interpolated envelope
	double	*lut;			// Blackman Sqaure look-up table

	double	*freq;		// frequency look-up table
	double	maxfreq;	// central frequency of the last band
	int32_t	Fa;		// Fa is the index of the band's start in the frequency domain
	int32_t	Fd;		// Fd is the index of the band's end in the frequency domain
	double	La;		// La is the log2 of the frequency of Fa
	double	Ld;		// Ld is the log2 of the frequency of Fd
	double	Li;		// Li is the iterative frequency between La and Ld defined logarithmically

	freq = freqarray(basefreq, bands, bpo);

	if (LOGBASE==1.0)
		maxfreq = bpo;	// in linear mode we use bpo to store the maxfreq since we couldn't deduce maxfreq otherwise
	else
		maxfreq = basefreq * pow(LOGBASE, ((double) (bands-1)/ bpo));

	clocka=gettime();

	*samplecount = roundoff(Xsize/pixpersec);		// calculation of the length of the final signal
	printf("Sound duration : %.3f s\n", (double) *samplecount/samplerate);

	s = calloc (*samplecount, sizeof(double));		// allocation of the final signal
	envelope = calloc (*samplecount, sizeof(double));	// allocation of the interpolated envelope

	//********Loop size calculation********

	loop_size = loop_size_sec * samplerate;

	if (LOGBASE==1.0)
		loop_size_min = (int32_t) roundoff(4.0*5.0/ freq[1]-freq[0]);	// linear mode
	else
		loop_size_min = (int32_t) roundoff(2.0*5.0/((freq[0] * pow(2.0, -1.0/(bpo))) * (1.0 - pow(2.0, -1.0 / bpo))));	// this is the estimate of how many samples the longest FIR will take up in the time domain

	if (loop_size_min > loop_size)
		loop_size = loop_size_min;

	loop_size = nextsprime(loop_size);	// enlarge the loop_size to the next multiple of short primes in order to make IFFTs faster
	//--------Loop size calculation--------

	//********Pink noise generation********

	pink_noise = calloc (loop_size, sizeof(double));

	for (i=1; i<(loop_size+1)>>1; i++)
	{
		mag = pow((double) i, 0.5 - 0.5*LOGBASE);	// FIXME something's not necessarily right with that formula
		phase = dblrand() * pi;				// random phase between -pi and +pi

		pink_noise[i]= mag * cos(phase);		// real part
		pink_noise[loop_size-i]= mag * sin(phase);	// imaginary part
	}
	//--------Pink noise generation--------

	noise = malloc(loop_size * sizeof(double));		// allocate noise
	lut = bmsq_lut(BMSQ_LUT_SIZE);				// Blackman Square look-up table initalisation

	for (ib=0; ib<bands; ib++)
	{
		printf("%4d/%d\r", ib+1, bands);

		memset(noise, 0, loop_size * sizeof(double));	// reset filtered noise

		//********Filtering********

		Fa = roundoff(log_pos((double) (ib-1)/(double) (bands-1), basefreq, maxfreq) * loop_size);
		Fd = roundoff(log_pos((double) (ib+1)/(double) (bands-1), basefreq, maxfreq) * loop_size);
		La = log_pos_inv((double) Fa / (double) loop_size, basefreq, maxfreq);
		Ld = log_pos_inv((double) Fd / (double) loop_size, basefreq, maxfreq);

		if (Fd > loop_size/2)
			Fd = loop_size/2;	// stop reading if reaching the Nyquist frequency

		if (Fa<1)
			Fa=1;

		printf("%4d/%d   %.2f Hz - %.2f Hz\r", ib+1, bands, (double) Fa*samplerate/loop_size, (double) Fd*samplerate/loop_size);

		for (i=Fa; i<Fd; i++)
		{
			Li = log_pos_inv((double) i / (double) loop_size, basefreq, maxfreq);	// calculation of the logarithmic position
			Li = (Li-La)/(Ld-La);
			coef = 0.5 - 0.5*cos(2.0*pi*Li);		// Hann function
			noise[i+1] = pink_noise[i+1] * coef;
			noise[loop_size-1-i] = pink_noise[loop_size-1-i] * coef;
		}
		//--------Filtering--------

		fft(noise, noise, loop_size, 1);	// IFFT of the filtered noise

		memset(envelope, 0, *samplecount * sizeof(double));							// blank the envelope
		blackman_square_interpolation(d[bands-ib-1], envelope, Xsize, *samplecount, lut, BMSQ_LUT_SIZE);	// interpolation of the envelope

		il = 0;
		for (i=0; i<*samplecount; i++)
		{
			s[i] += envelope[i] * noise[il];	// modulation
			il++;					// increment loop iterator
			if (il==loop_size)			// if the array iterator has reached the end of the array, it's reset
				il=0;
		}
	}
	
	printf("\n");

	normi(&s, *samplecount, 1, 1.0);

	return s;
}

void brightness_control(double **image, int32_t width, int32_t height, double ratio)	// Almost like a gamma correction, but uses a different formula
{
	// Actually this is based on the idea of converting values to decibels, for example, 0.01 becomes -40 dB, dividing them by ratio, so if ratio is 2 then -40 dB/2 = -20 dB, and then turning it back to regular values, so -20 dB becomes 0.1
	// If ratio==2 then this function is equivalent to a square root
	// 1/ratio is used for the forward transformation
	// ratio is used for the reverse transformation

	int32_t ix, iy;

	for (iy=0; iy<width; iy++)
		for (ix=0; ix<height; ix++)
			image[iy][ix] = pow(image[iy][ix], ratio);
}
