#ifndef H_UTIL
#define H_UTIL

#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <math.h>
#include <float.h>
#include <time.h>
#include <string.h>

#include "dsp.h"

int32_t quiet;

extern inline void test_fft();
extern inline void log_file(char *str);
extern inline void log_double(char *str, double y); 
extern inline void log_double_array(char *filename, double *array, int32_t len);
extern inline void log_int32_t(char *str, int32_t y);
extern inline double myfmod(double x, double mod); 

extern void win_return();
extern int32_t gettime();
extern inline double roundoff(double x);
extern inline int32_t roundup(double x);
extern float getfloat();
extern inline int32_t smallprimes(int32_t x);
extern inline int32_t nextsprime(int32_t x);
extern inline double log_b(double x);
extern inline uint32_t rand_u32();
extern inline double dblrand();
extern inline uint16_t fread_le_short(FILE *file);
extern inline uint32_t fread_le_word(FILE *file);
extern inline void fwrite_le_short(uint16_t s, FILE *file);
extern inline void fwrite_le_word(uint32_t w, FILE *file);
extern char *getstring();
extern int32_t str_isnumber(char *string);

#endif
