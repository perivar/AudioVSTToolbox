#ifndef H_IMAGE_IO
#define H_IMAGE_IO

#include <stdlib.h>
#include <stdio.h>
#include <stdint.h>

#include "util.h"

extern double **bmp_in(FILE *bmpfile, int32_t *y, int32_t *x);
extern void bmp_out(FILE *bmpfile, double **image, int32_t y, int32_t x);

#endif
