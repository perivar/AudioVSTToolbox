/* The Analysis & Resynthesis Sound Spectrograph
Copyright (C) 2005-2008 Michel Rouzic

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.*/

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <math.h>
#include <fftw3.h>
#include <float.h>
#include <time.h>
#include <string.h>

#include "util.h"
#include "image_io.h"
#include "sound_io.h"
#include "dsp.h"

char *version = "0.2.3";
char *date = "May 29th, 2008";

#define MSG_NUMBER_EXPECTED "A number is expected after %s\nExiting with error.\n"

void settingsinput(int32_t *bands, int32_t samplecount, int32_t *samplerate, double *basefreq, double maxfreq, double *pixpersec, double *bandsperoctave, int32_t Xsize, int32_t mode)
{
	/* mode :
	 * 0 = Analysis mode
	 * 1 = Synthesis mode
	 */

	int32_t i;
	double gf, f, trash;
	double ma;			// maximum allowed frequency
	FILE *freqcfg;
	char byte;
	int32_t unset=0, set_min=0, set_max=0, set_bpo=0, set_y=0;			// count of unset interdependant settings
	int32_t set_pps=0, set_x=0;
	size_t filesize;		// boolean indicating if the configuration file's last expected byte is there (to verify the file's integrity)
	char conf_path[FILENAME_MAX];	// Path to the configuration file (only used on non-Windows platforms)

	#ifdef DEBUG
	printf("settingsinput...\n");
	#endif

	#ifdef WIN32
	freqcfg=fopen("arss.conf", "rb");					// open saved settings file
	#else
	sprintf(conf_path, "%s/%s", getenv("HOME"), ".arss.conf");
	freqcfg=fopen(conf_path, "rb");
	#endif

	if (*samplerate==0)					// if we're in synthesis mode and that no samplerate has been defined yet
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please provide a sample rate for your output sound.\nUse --sample-rate (-r).\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}
		//********Output settings querying********
	
		printf("Sample rate [44100] : ");			// Query for a samplerate
		*samplerate=getfloat();
		if (*samplerate==0 || *samplerate<-2147483647)		// The -2147483647 check is used for the sake of compatibility with C90
			*samplerate = 44100;				// Default value
		//--------Output settings querying--------
	}

	if (*basefreq!=0)	set_min=1;	// count unset interdependant frequency-domain settings
	if (maxfreq!=0)		set_max=1;
	if (*bandsperoctave!=0)	set_bpo=1;
	if (*bands!=0)		set_y=1;
	unset = set_min + set_max + set_bpo + set_y;

	if (unset==4)				// if too many settings are set
	{
		if (mode==0)
			fprintf(stderr, "You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b)\nExiting with error.\n");
		if (mode==1)
			fprintf(stderr, "You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b) or --height (-y)\nExiting with error.\n");
		exit(EXIT_FAILURE);
	}

	if (*pixpersec!=0)	set_pps=1;
	if (Xsize!=0)		set_x=1;

	if (set_x+set_pps==2 && mode==0)
	{
		fprintf(stderr, "You cannot define both the image width and the horizontal resolution.\nUnset either --pps (-p) or --width (-x)\nExiting with error.\n");
		exit(EXIT_FAILURE);
	}

	if (freqcfg)								// load settings from file if it exists
	{
		for (i=0; i<(int32_t) (4*sizeof(double)); i++)
			filesize=fread(&byte, sizeof(char), 1, freqcfg);	// verify the file's length
		rewind(freqcfg);
	}
	if (filesize==1)							// if the file is at least as long as expected
	{
		if (*basefreq==0)	fread(basefreq, sizeof(double), 1, freqcfg);		// load values from it if they haven't been set yet
		else			fread(&trash, sizeof(double), 1, freqcfg);
		if (maxfreq==0)		fread(&maxfreq, sizeof(double), 1, freqcfg);		// unless we have enough of them (unset==3)
		else			fread(&trash, sizeof(double), 1, freqcfg);
		if (*bandsperoctave==0)	fread(bandsperoctave, sizeof(double), 1, freqcfg);
		else			fread(&trash, sizeof(double), 1, freqcfg);
		if (*pixpersec==0)	fread(pixpersec, sizeof(double), 1, freqcfg);
		else			fread(&trash, sizeof(double), 1, freqcfg);
	}
	else
	{
		if (*basefreq==0)	*basefreq=27.5;				// otherwise load default values
		if (maxfreq==0)		maxfreq=20000;
		if (*bandsperoctave==0)	*bandsperoctave=12;
		if (*pixpersec==0)	*pixpersec=150;
	}
	if (freqcfg)
		fclose(freqcfg);

	if (unset<3 && set_min==0)
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please define a minimum frequency.\nUse --min-freq (-min).\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}
		printf("Min. frequency (Hz) [%.3f]: ", *basefreq);
		gf=getfloat();
		if (gf != 0)
			*basefreq=gf;
		unset++;
		set_min=1;
	}
	*basefreq /= *samplerate;	// turn basefreq from Hz to fractions of samplerate

	if (unset<3 && set_bpo==0)
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please define a bands per octave setting.\nUse --bpo (-b).\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}
		printf("Bands per octave [%.3f]: ", *bandsperoctave);
		gf=getfloat();
		if (gf != 0)
			*bandsperoctave=gf;
		unset++;
		set_bpo=1;
	}

	if (unset<3 && set_max==0)
	{
		i=0;
		do
		{
			i++;
			f=*basefreq * pow(LOGBASE, (i / *bandsperoctave));
		}
		while (f<0.5);
		
		ma=*basefreq * pow(LOGBASE, ((i-2) / *bandsperoctave)) * *samplerate;	// max allowed frequency
		
	
		if (maxfreq > ma)
			if (myfmod(ma, 1.0) == 0.0)
				maxfreq = ma;			// replaces the "Upper frequency limit above Nyquist frequency" warning
			else
				maxfreq = ma - myfmod(ma, 1.0);
	
		if (mode==0)					// if we're in Analysis mode
		{
			if (quiet==1)
			{
				fprintf(stderr, "Please define a maximum frequency.\nUse --max-freq (-max).\nExiting with error.\n");
				exit(EXIT_FAILURE);
			}
			printf("Max. frequency (Hz) (up to %.3f) [%.3f]: ", ma, maxfreq);
			gf=getfloat();
			if (gf != 0)
				maxfreq=gf;
		
			if (maxfreq > ma)
				if (myfmod(ma, 1.0) == 0.0)
					maxfreq = ma;		// replaces the "Upper frequency limit above Nyquist frequency" warning
				else
					maxfreq = ma - myfmod(ma, 1.0);
		}
		
		unset++;
		set_max=1;
	}

	if (set_min==0)
	{
		*basefreq = pow(LOGBASE, (*bands-1) / *bandsperoctave) * maxfreq;		// calculate the lower frequency in Hz
		printf("Min. frequency : %.3f Hz\n", *basefreq);
		*basefreq /= *samplerate;
	}

	if (set_max==0)
	{
		maxfreq = pow(LOGBASE, (*bands-1) / *bandsperoctave) * (*basefreq * *samplerate);	// calculate the upper frequency in Hz
		printf("Max. frequency : %.3f Hz\n", maxfreq);
	}

	if (set_y==0)
	{
		*bands = 1 + roundoff(*bandsperoctave * (log_b(maxfreq) - log_b(*basefreq * *samplerate)));
		printf("Bands : %d\n", *bands);
	}

	if (set_bpo==0)
	{
		if (LOGBASE==1.0)
			*bandsperoctave = maxfreq / *samplerate;
		else
			*bandsperoctave = (*bands-1) / (log_b(maxfreq) - log_b(*basefreq * *samplerate));
		printf("Bands per octave : %.3f\n", *bandsperoctave);
	}

	if (set_x==1 && mode==0)	// If we're in Analysis mode and that X is set (by the user)
	{
		*pixpersec = (double) Xsize * (double) *samplerate / (double) samplecount;	// calculate pixpersec
		printf("Pixels per second : %.3f\n", *pixpersec);
	}

	if ((mode==0 && set_x==0 && set_pps==0) || (mode==1 && set_pps==0))	// If in Analysis mode none are set or pixpersec isn't set in Synthesis mode
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please define a pixels per second setting.\nUse --pps (-p).\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}
		printf("Pixels per second [%.3f]: ", *pixpersec);
		gf=getfloat();
		if (gf != 0)
			*pixpersec=gf;
	}

	*basefreq *= *samplerate;		// turn back to Hz just for the sake of writing to the file

	#ifdef WIN32
	freqcfg=fopen("arss.conf", "wb");	// saving settings to a file
	#else
	freqcfg=fopen(conf_path, "wb");		// saving settings to a file
	#endif
	if (freqcfg==NULL)
	{
		fprintf(stderr, "Cannot write to configuration file");
		exit(EXIT_FAILURE);
	}
	fwrite(basefreq, sizeof(double), 1, freqcfg);
	fwrite(&maxfreq, sizeof(double), 1, freqcfg);
	fwrite(bandsperoctave, sizeof(double), 1, freqcfg);
	fwrite(pixpersec, sizeof(double), 1, freqcfg);
	fclose(freqcfg);

	*basefreq /= *samplerate;	// basefreq is now in fraction of the sampling rate instead of Hz
	*pixpersec /= *samplerate;	// pixpersec is now in fraction of the sampling rate instead of Hz
}

void print_help()
{
	printf(
		"Usage: arss [options] input_file output_file [options]. Example:\n"
		"\n"
		"arss -q in.bmp out.wav --noise --min-freq 55 -max 16000 --pps 100 -r 44100 -f 16\n"
		"\n"
		"--help, -h, /?                Display this help\n"
		"--adv-help                    Display more advanced options\n"
		"--version, -v                 Display the version of this program\n"
		"--quiet, -q                   No-prompt mode. Useful for scripting\n"
		"--analysis, -a                Analysis mode. Not req. if input file is a .wav\n"
		"--sine, -s                    Sine synthesis mode\n"
		"--noise, -n                   Noise synthesis mode\n"
		"--min-freq, -min [real]       Minimum frequency in Hertz\n"
		"--max-freq, -max [real]       Maximum frequency in Hertz\n"
		"--bpo, -b [real]              Frequency resolution in Bands Per Octave\n"
		"--pps, -p [real]              Time resolution in Pixels Per Second\n"
		"--height, -y [integer]        Specifies the desired height of the spectrogram\n"
		"--width, -x [integer]         Specifies the desired width of the spectrogram\n"
		"--sample-rate, -r [integer]   Sample rate of the output sound\n"
		"--brightness, -g [real]       Almost like gamma : f(x) = x^1/brightness\n"
		"--format-param, -f [integer]  Output format option. This is bit-depth for WAV files (8/16/32, default: 32). No option for BMP files.\n"
		);
}

void print_adv_help()
{
	printf(
		"More advanced options :\n"
		"\n"
		"--log-base [real]          Frequency scale's logarithmic base (default: 2)\n"
		"--linear, -l               Linear frequency scale. Same as --log-base 1\n"
		"--loop-size [real]         Noise look-up table size in seconds (default: 10)\n"
		"--bmsq-lut-size [integer]  Blackman Square kernel LUT size (default: 16000)\n"
		"--pi [real]                pi (default: 3.1415926535897932)\n"
	      );
}

int main(int argc, char *argv[])
{
	log_file("Starting arss ...");

	#ifdef DEBUG
	test_fft();
	#endif

	FILE *fin;
	FILE *fout;
	int32_t  i;
	double **sound, **image, basefreq=0, maxfreq=0, pixpersec=0, bpo=0, brightness=1;
	int32_t channels, samplecount=0, samplerate=0, Xsize=0, Ysize=0, format_param=0;
	int32_t clockb;
	char mode=0, *in_name=NULL, *out_name=NULL;

	// initialisation of global using defaults defined in dsp.h
	pi=PI_D;
	LOGBASE=LOGBASE_D;
	LOOP_SIZE_SEC=LOOP_SIZE_SEC_D;
	BMSQ_LUT_SIZE=BMSQ_LUT_SIZE_D;
	#ifdef QUIET
	quiet=1;
	#else
	quiet=0;
	#endif

	printf("The Analysis & Resynthesis Sound Spectrograph %s\n", version);

	srand(time(NULL));

	for (i=1; i<argc; i++)
	{
		if (strcmp(argv[i], "/?")==0)	// DOS friendly help
		{
			argv[i][0] = '-';
			argv[i][1] = 'h';
		}

		if (argv[i][0] != '-')				// if the argument is not a function
		{
			if (in_name==NULL)			// if the input file name hasn't been filled in yet
				in_name = argv[i];
			else
				if (out_name==NULL)		// if the input name has been filled but not the output name yet
					out_name = argv[i];
				else				// if both names have already been filled in
				{
					fprintf(stderr, "You can only have two file names as parameters.\nRemove parameter \"%s\".\nExiting with error.\n", argv[i]);
					exit(EXIT_FAILURE);
				}
		}
		else						// if the argument is a parameter
		{
			if (strcmp(argv[i], "--analysis")==0	|| strcmp(argv[i], "-a")==0)
				mode=1;

			if (strcmp(argv[i], "--sine")==0	|| strcmp(argv[i], "-s")==0)
				mode=2;

			if (strcmp(argv[i], "--noise")==0	|| strcmp(argv[i], "-n")==0)
				mode=3;

			if (strcmp(argv[i], "--quiet")==0	|| strcmp(argv[i], "-q")==0)
				quiet=1;

			if (strcmp(argv[i], "--linear")==0	|| strcmp(argv[i], "-l")==0)
				LOGBASE=1.0;

			if (strcmp(argv[i], "--sample-rate")==0	|| strcmp(argv[i], "-r")==0)
				if (str_isnumber(argv[++i]))
						samplerate = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--min-freq")==0	|| strcmp(argv[i], "-min")==0)
				if (str_isnumber(argv[++i]))
				{
					basefreq = atof(argv[i]);
					if (basefreq==0)
							basefreq = DBL_MIN;	// Set it to this extremely close-to-zero number so that it's considered set
				}
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--max-freq")==0	|| strcmp(argv[i], "-max")==0)
				if (str_isnumber(argv[++i]))
						maxfreq = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--bpo")==0		|| strcmp(argv[i], "-b")==0)
				if (str_isnumber(argv[++i]))
						bpo = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--pps")==0		|| strcmp(argv[i], "-p")==0)
				if (str_isnumber(argv[++i]))
						pixpersec = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--height")==0	|| strcmp(argv[i], "-y")==0)
				if (str_isnumber(argv[++i]))
						Ysize = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--width")==0	|| strcmp(argv[i], "-x")==0)
				if (str_isnumber(argv[++i]))
						Xsize = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--loop-size")==0)
				if (str_isnumber(argv[++i]))
						LOOP_SIZE_SEC = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--log-base")==0)
				if (str_isnumber(argv[++i]))
						LOGBASE = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--bmsq-lut-size")==0)
				if (str_isnumber(argv[++i]))
						BMSQ_LUT_SIZE = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--pi")==0)					// lol
				if (str_isnumber(argv[++i]))
						pi = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--format-param")==0	|| strcmp(argv[i], "-f")==0)
				if (str_isnumber(argv[++i]))
						format_param = atoi(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			if (strcmp(argv[i], "--brightness")==0	|| strcmp(argv[i], "-g")==0)
				if (str_isnumber(argv[++i]))
						brightness = atof(argv[i]);
				else
				{
					fprintf(stderr, MSG_NUMBER_EXPECTED, argv[i-1]);
					exit(EXIT_FAILURE);
				}

			// TODO implement --duration, -d

			if (strcmp(argv[i], "--version")==0	|| strcmp(argv[i], "-v")==0)
			{
				printf("Copyright (C) 2005-2008 Michel Rouzic\nProgram last modified by its author on %s\n", date);
				exit(EXIT_SUCCESS);
			}

			if (strcmp(argv[i], "--help")==0	|| strcmp(argv[i], "-h")==0)
			{
				print_help();
				exit(EXIT_SUCCESS);
			}

			if (strcmp(argv[i], "--adv-help")==0)
			{
				print_adv_help();
				exit(EXIT_SUCCESS);
			}



		}
	}

	if (in_name!=NULL)			// if in_name has already been filled in
	{
		fin=fopen(in_name, "rb");	// try to open it

		if (fin==NULL)
		{
			fprintf(stderr, "The input file %s could not be found\nExiting with error.\n", in_name);
			exit(EXIT_FAILURE);
		}
		printf("Input file : %s\n", in_name);
	}
	else
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please specify an input file.\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}

		printf("Type 'help' to read the manual page\n");

		do
		{
			printf("Input file : ");
			in_name=getstring();

			if (strcmp(in_name, "help")==0)		// if 'help' has been typed
			{
				fin=NULL;
				print_help();			// print the help
			}
			else
				fin=fopen(in_name, "rb");
		}
		while (fin==NULL);
	}

	if (out_name!=NULL)			// if out_name has already been filled in
	{
		fout=fopen(out_name, "wb");

		if (fout==NULL)
		{
			fprintf(stderr, "The output file %s could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\nExiting with error.\n", out_name);
			exit(EXIT_FAILURE);
		}
		printf("Output file : %s\n", out_name);
	}
	else
	{
		if (quiet==1)
		{
			fprintf(stderr, "Please specify an output file.\nExiting with error.\n");
			exit(EXIT_FAILURE);
		}
		printf("Output file : ");
		out_name=getstring();
		fout=fopen(out_name, "wb");

		while (fout==NULL)
		{
			fprintf(stderr, "The output file %s could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\n", out_name);
			getchar();
			fout=fopen(out_name, "wb");
		}
	}

	for (i=0; i<strlen(in_name); i++) if (in_name[i]>='A' && in_name[i]<='Z') in_name[i] -= 'A' - 'a';	// make the string lowercase
	if (mode==0 && strstr(in_name, ".wav")!=NULL && strstr(in_name, ".wav")[4]==0)
		mode=1;	// Automatic switch to the Analysis mode

	if (mode==0)
		do
		{
			if (quiet==1)
			{
				fprintf(stderr, "Please specify an operation mode.\nUse either --analysis (-a), --sine (-s) or --noise (-n).\nExiting with error.\n");
				exit(EXIT_FAILURE);
			}
			printf("Choose the mode (Press 1, 2 or 3) :\n\t1. Analysis\n\t2. Sine synthesis\n\t3. Noise synthesis\n> ");
			mode=getfloat();
		}
		while (mode!=1 && mode!=2 && mode!=3);

		
	if (mode==1)
	{
		sound=wav_in(fin, &channels, &samplecount, &samplerate);					// Sound input
		log_file("Read sound...");
		
		#ifdef DEBUG
		printf("samplecount : %i\nchannels : %i\n", samplecount, channels);
		#endif

		settingsinput(&Ysize, samplecount, &samplerate, &basefreq, maxfreq, &pixpersec, &bpo, Xsize, 0);	// User settings input
		image = anal(sound[0], samplecount, samplerate, &Xsize, Ysize, bpo, pixpersec, basefreq);	// Analysis
		if (brightness!=1.0)
			brightness_control(image, Ysize, Xsize, 1.0/brightness);
		bmp_out(fout, image, Ysize, Xsize);								// Image output
	}
	if (mode==2 || mode==3)
	{
		sound = calloc (1, sizeof(double *));
		image = bmp_in(fin, &Ysize, &Xsize);							// Image input

		if (format_param==0)				// if the output format parameter is undefined
			if (quiet==0)				// if prompting is allowed
				format_param = wav_out_param();
			else
				format_param = 32;		// default is 32

		settingsinput(&Ysize, samplecount, &samplerate, &basefreq, maxfreq, &pixpersec, &bpo, Xsize, 1);	// User settings input

		if (brightness!=1.0) brightness_control(image, Ysize, Xsize, brightness);

		if (mode==2)
			sound[0] = synt_sine(image, Xsize, Ysize, &samplecount, samplerate, basefreq, pixpersec, bpo);	// Sine synthesis
		else
			sound[0] = synt_noise(image, Xsize, Ysize, &samplecount, samplerate, basefreq, pixpersec, bpo);	// Noise synthesis

		wav_out(fout, sound, 1, samplecount, samplerate, format_param);
	}

	clockb=gettime();
	printf("Processing time : %.3f s\n", (double) (clockb-clocka)/1000.0); 

	win_return();

	return 0;
}
