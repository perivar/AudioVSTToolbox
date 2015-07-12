#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <math.h>
#include <float.h>
#include <time.h>
#include <string.h>
#include <fftw3.h>

extern inline void test_fft();
extern inline void log_file(char *str);
extern inline void log_double(char *str, double y); 
extern inline void log_double_array(char *filename, double *array, int32_t len);
extern inline void log_complex_array(char *filename, fftw_complex *array, int32_t len);
extern inline void log_double_array_placeholder(char *filename, int32_t filename_counter, double *array, int32_t len);
extern inline void log_double_array2D(char *filename, double **array, int32_t xs, int32_t ys);
extern inline void log_int32_t(char *str, int32_t y);
extern int32_t gettime();
extern char* gettimestring();