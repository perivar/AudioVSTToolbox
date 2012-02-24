#ifndef H_DSP
#define H_DSP

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <math.h>
#include <fftw3.h>
#include <float.h>
#include <time.h>
#include <string.h>

#include "util.h"

#define PI_D		3.1415926535897932
#define LOGBASE_D	2
#define LOOP_SIZE_SEC_D	10.0
#define BMSQ_LUT_SIZE_D	16000

double pi;
double LOGBASE;			// Base for log() operations. Anything other than 2 isn't really supported
#define TRANSITION_BW_SYNT 16.0		// defines the transition bandwidth for the low-pass filter on the envelopes during synthesisation
double LOOP_SIZE_SEC;		// size of the noise loops in seconds
int32_t BMSQ_LUT_SIZE;		// defines the number of elements in the Blackman Square look-up table. It's best to make it small enough to be entirely cached

int32_t clocka;

extern void fft(double *in, double *out, int32_t N, uint8_t method);
extern void normi(double **s, int32_t xs, int32_t ys, double ratio);
extern double *freqarray(double basefreq, int32_t bands, double bandsperoctave);
extern double *blackman_downsampling(double *in, int32_t Mi, int32_t Mo);
extern double *bmsq_lut(int32_t size);
extern void blackman_square_interpolation(double *in, double *out, int32_t Mi, int32_t Mo, double *lut, int32_t lut_size);	
extern double **anal(double *s, int32_t samplecount, int32_t samplerate, int32_t *Xsize, int32_t bands, double bpo, double pixpersec, double basefreq);
extern double *wsinc_max(int32_t length, double bw);
extern double *synt_sine(double **d, int32_t Xsize, int32_t bands, int32_t *samplecount, int32_t samplerate, double basefreq, double pixpersec, double bpo);
extern double *synt_noise(double **d, int32_t Xsize, int32_t bands, int32_t *samplecount, int32_t samplerate, double basefreq, double pixpersec, double bpo);
extern void brightness_control(double **image, int32_t width, int32_t height, double ratio);

#endif
