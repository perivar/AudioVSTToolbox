#ifndef H_SOUND_IO
#define H_SOUND_IO

#include <stdlib.h>
#include <stdio.h>
#include <stdint.h>

#include "util.h"

extern void in_8(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern void out_8(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern void in_16(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern void out_16(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern void in_32(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern void out_32(FILE *wavfile, double **sound, int32_t samplecount, int32_t channels);
extern double **wav_in(FILE *wavfile, int32_t *channels, int32_t *samplecount, int32_t *samplerate);
extern void wav_out(FILE *wavfile, double **sound, int32_t channels, int32_t samplecount, int32_t samplerate, int32_t format_param);
extern int32_t wav_out_param();

#endif
