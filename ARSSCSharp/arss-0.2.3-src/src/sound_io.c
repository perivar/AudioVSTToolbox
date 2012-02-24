#include "sound_io.h"

void in_8(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;
	uint8_t byte;

	#ifdef DEBUG
	printf("in_8...\n");
	#endif

	for (i=0; i<samplecount; i++)
		for (ic=0;ic<channels;ic++)
		{
			fread(&byte, 1, 1, wavfile);
			sound[ic][i] = (double) byte/128.0 - 1.0;
		}
}

void out_8(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;
	double val;
	uint8_t byte;

	#ifdef DEBUG
	printf("out_8...\n");
	#endif

	for (i=0; i<samplecount; i++)
		for (ic=0;ic<channels;ic++)
		{
			val = roundoff((sound[ic][i]+1.0)*128.0);
			if (val>255)
				val=255;
			if (val<0)
				val=0;

			byte = (uint8_t) val;

			fwrite(&byte, sizeof(uint8_t), 1, wavfile);
		}
}

void in_16(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;

	#ifdef DEBUG
	printf("in_16...\n");
	#endif

	for (i=0; i<samplecount; i++)
		for (ic=0; ic<channels; ic++)
			sound[ic][i]=(double) ((int16_t) fread_le_short(wavfile))/32768.0;
}

void out_16(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;
	double val;

	#ifdef DEBUG
	printf("out_16...\n");
	#endif

	for (i=0; i<samplecount; i++)
		for (ic=0;ic<channels;ic++)
		{
			val=roundoff(sound[ic][i]*32768.0);
			if (val>32767.0)
				val=32767.0;
			if (val<-32768.0)
				val=-32768.0;

			fwrite_le_short((int16_t) val, wavfile);
		}
}

void in_32(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;
	float val;

	#ifdef DEBUG
	printf("in_32...\n");
	#endif

	for (i=0;i<samplecount;i++)
		for (ic=0;ic<channels;ic++)
		{
			*(uint32_t *) &val = fread_le_word(wavfile);
			sound[ic][i] = (double) val;
		}
}

void out_32(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels)
{
	int32_t i, ic;
	float val;

	#ifdef DEBUG
	printf("out_32...\n");
	#endif

	for (i=0; i<samplecount; i++)
		for (ic=0;ic<channels;ic++)
		{
			val = (float) sound[ic][i];
			fwrite_le_word(*(uint32_t *) &val, wavfile);
		}
}

double **wav_in(FILE *wavfile, int32_t *channels, int32_t *samplecount, int32_t *samplerate)
{
	int32_t i, ic;
	double **sound;
	int32_t tag[13];

	#ifdef DEBUG
	printf("wav_in...\n");
	#endif

	for (i=0; i<13; i++)					// tag reading
	{
		tag[i]=0;

		if ((i==5) || (i==6) || (i==9) || (i==10))
			tag[i] = fread_le_short(wavfile);
		else
			tag[i] = fread_le_word(wavfile);
	}

	//********File format checking********

	if (tag[0]!=1179011410 || tag[2]!=1163280727)
	{
		fprintf(stderr, "This file is not in WAVE format\n");
		win_return();
		exit(EXIT_FAILURE);
	}

	if (tag[3]!=544501094 || tag[4]!=16 || tag[11]!=1635017060)
	{
		fprintf(stderr, "This WAVE file format is not currently supported\n");
		win_return();
		exit(EXIT_FAILURE);
	}
	//--------File format checking--------

	*channels = tag[6];

	*samplecount = tag[12]/(tag[10]/8) / *channels;
	*samplerate = tag[7];

	sound = malloc (*channels * sizeof(double *));	// allocate sound
	for (ic=0; ic < *channels; ic++)
		sound[ic] = malloc (*samplecount * sizeof(double));

	//********Data loading********

	if (tag[10]==8)
		in_8(wavfile, sound, *samplecount, *channels);
	if (tag[10]==16)
		in_16(wavfile, sound, *samplecount, *channels);
	if (tag[10]==32)
		in_32(wavfile, sound, *samplecount, *channels);
	//--------Data loading--------

	fclose(wavfile);
	return sound;
}

void wav_out(FILE *wavfile, double **sound, int32_t channels, int32_t samplecount, int32_t samplerate, int32_t format_param)
{
	int32_t i;
	int32_t tag[] = {1179011410, 0, 1163280727, 544501094, 16, 1, 1, 0, 0, 0, 0, 1635017060, 0, 0};

	#ifdef DEBUG
	printf("wav_out...\n");
	#endif

	//********WAV tags generation********

	tag[12] = samplecount*(format_param/8)*channels;
	tag[1] = tag[12]+36;
	tag[7] = samplerate;
	tag[8] = samplerate*format_param/8;
	tag[9] = format_param/8;
	tag[6] = channels;
	tag[10] = format_param;

	if ((format_param==8) || (format_param==16))
		tag[5]=1;
	if (format_param==32)
		tag[5]=3;
	//--------WAV tags generation--------

	for (i=0; i<13; i++)					// tag writing
		if ((i==5) || (i==6) || (i==9) || (i==10))
			fwrite_le_short(tag[i], wavfile);
		else
			fwrite_le_word(tag[i], wavfile);

	if (format_param==8)
		out_8(wavfile, sound, samplecount, channels);
	if (format_param==16)
		out_16(wavfile, sound, samplecount, channels);
	if (format_param==32)
		out_32(wavfile, sound, samplecount, channels);

	fclose(wavfile);
}

int32_t wav_out_param()
{
	int32_t bps;

	do
	{
		printf("Bits per sample (8/16/32) [16] : ");
		bps=getfloat();
		if (bps==0 || bps<-2147483647)	// The -2147483647 check is used for the sake of compatibility with C90
			bps = 16;
	}
	while (bps!=8 && bps!=16 && bps!=32);

	return bps;
}
