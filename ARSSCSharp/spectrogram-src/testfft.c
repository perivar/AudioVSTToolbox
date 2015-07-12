#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <math.h>
#include <fftw3.h>
#include <float.h>
#include <time.h>
#include <string.h>

#include "util.h"

int main ( );
void testfft ( void );
double frand ( void );
int smallprimes( int );
int padded_size( int );
double* doubleRealloc(double *ptr, int old_size, int new_size);
fftw_complex* padded_FFT(double *in, int in_length);
double* padded_IFFT(fftw_complex *in, int in_length);

int main()
{
    printf ( "%s\n", gettimestring() );
    log_file("====================");
	log_file("Starting testfft ...");
    //testfft();
    
    unsigned int seed = 123456789;
    int i;
    double *in;
    fftw_complex *out;
    int n = 260; 	// length of signal
	int m = 0; 		// temp to calculate complexLength
	int complexLength = 0;
    /*
    Set up an array to hold the data, and assign the data.
    */
    in = fftw_malloc ( sizeof ( double ) * n );
    
    srand ( seed );
    
    for ( i = 0; i < n; i++ )
    {
        in[i] = frand ( );
    }
    
    out = padded_FFT(in, n);

	// calculate what the complex length was
	m = n > 256 ? padded_size(n) : n;
    complexLength = ( m / 2 ) + 1;
	
	padded_IFFT(out, complexLength);
    
    return 1;
}

void testfft( void )

/******************************************************************************/
/*
Purpose:
TEST02: apply FFT to real 1D data.

Licensing:
This code is distributed under the GNU LGPL license.

Modified:
23 October 2005

Author:
John Burkardt
*/
{
    int i;
    double *in;
    double *in2;
    int n = 100;
    int nc;
    fftw_complex *out;
    fftw_plan plan_backward;
    fftw_plan plan_forward;
    unsigned int seed = 123456789;
    
    printf ( "\n" );
    printf ( "TEST02\n" );
    printf ( "  Demonstrate FFTW3 on a single vector of real data.\n" );
    printf ( "\n" );
    printf ( "  Transform data to FFT coefficients.\n" );
    printf ( "  Backtransform FFT coefficients to recover data.\n" );
    printf ( "  Compare recovered data to original data.\n" );
    /*
    Set up an array to hold the data, and assign the data.
    */
    in = fftw_malloc ( sizeof ( double ) * n );
    
    srand ( seed );
    
    for ( i = 0; i < n; i++ )
    {
        in[i] = frand ( );
    }
    
    printf ( "\n" );
    printf ( "  Input Data:\n" );
    printf ( "\n" );
    
    for ( i = 0; i < n; i++ )
    {
        printf ( "  %4d  %12f\n", i, in[i] );
    }
    /*
    Set up an array to hold the transformed data,
    get a "plan", and execute the plan to transform the IN data to
    the OUT FFT coefficients.
    */
    nc = ( n / 2 ) + 1;
    
    out = fftw_malloc ( sizeof ( fftw_complex ) * nc );
    
    plan_forward = fftw_plan_dft_r2c_1d ( n, in, out, FFTW_ESTIMATE );
    
    fftw_execute ( plan_forward );
    
    printf ( "\n" );
    printf ( "  Output FFT Coefficients:\n" );
    printf ( "\n" );
    
    for ( i = 0; i < nc; i++ )
    {
        printf ( "  %4d  %12f  %12f\n", i, out[i][0], out[i][1] );
    }
    /*
    Set up an array to hold the backtransformed data IN2,
    get a "plan", and execute the plan to backtransform the OUT
    FFT coefficients to IN2.
    */
    in2 = fftw_malloc ( sizeof ( double ) * n );
    
    plan_backward = fftw_plan_dft_c2r_1d ( n, out, in2, FFTW_ESTIMATE );
    
    fftw_execute ( plan_backward );
    
    printf ( "\n" );
    printf ( "  Recovered input data divided by N:\n" );
    printf ( "\n" );
    
    for ( i = 0; i < n; i++ )
    {
        printf ( "  %4d  %12f\n", i, in2[i] / ( double ) ( n ) );
    }
    /*
    Release the memory associated with the plans.
    */
    fftw_destroy_plan ( plan_forward );
    fftw_destroy_plan ( plan_backward );
    
    fftw_free ( in );
    fftw_free ( in2 );
    fftw_free ( out );
    
    return;
}
/******************************************************************************/

//*****************************************************************************/

double frand ( void )

//*****************************************************************************/
/*
Purpose:
FRAND returns random values between 0 and 1.

Discussion:
The random seed can be set by a call to SRAND ( unsigned int ).
Note that Kernighan and Ritchie suggest using

( ( double ) rand ( ) / ( RAND_MAX + 1 ) )

but this seems to result in integer overflow for RAND_MAX + 1,
resulting in negative values for the random numbers.

Licensing:
This code is distributed under the GNU LGPL license.

Modified:
23 October 2005

Author:
John Burkardt

Reference:
Brian Kernighan, Dennis Ritchie,
The C Programming Language,
Prentice Hall, 1988.

Parameters:
Output, double FRAND, a random value between 0 and 1.
*/
{
    double value;
    
    value = ( ( double ) rand ( ) / ( RAND_MAX ) );
    
    return value;
}
//*****************************************************************************/


/// Returns 1 if x is only made of small primes.
int smallprimes(int x)
{
    int i;
    int p[2] = {2, 3};
    for (i = 0; i < 2; ++i)
		while (x%p[i] == 0)
			x /= p[i];
    
	return x;
}

/// Returns the next integer only made of small primes.
int padded_size(int x)
{
    while (smallprimes(x)!=1)
		x++;
    
    return x;
}

// reallocates double vector ptr to a larger size
double* doubleRealloc(double *old_ptr, int old_size, int new_size)
{
    log_double("Reallocating -> Old array length:", old_size);
    log_double("Reallocating -> New array length:", new_size);
    
    double *new_ptr = fftw_malloc ( sizeof ( double ) * new_size );
    
    if (!new_ptr)
		exit(1);
    
    if (old_size > new_size)
		old_size = new_size;
    
    memcpy(new_ptr, old_ptr, sizeof ( double ) * old_size );
    free(old_ptr);
    
    return new_ptr;
}

fftw_complex* padded_FFT(double *in, int in_length)
{
    int n;
    int nc;
    fftw_plan plan_forward;
    fftw_complex *out;
    
	log_double("FFT input array length:", in_length);	
    log_double_array("fft_input.csv", in, in_length);
    
    // n is padded size
    n = in_length > 256 ? padded_size(in_length) : in_length;
    
    // resize input array
    in = doubleRealloc(in, in_length, n);
    
    log_double_array("fft_input_resized.csv", in, n);
    
    /*
    Set up an array to hold the transformed data,
    get a "plan", and execute the plan to transform the IN data to
    the OUT FFT coefficients.
    */
    nc = ( n / 2 ) + 1;
    
    out = fftw_malloc ( sizeof ( fftw_complex ) * nc );
    
	log_double("FFT output array length:", nc);	
	
    plan_forward = fftw_plan_dft_r2c_1d ( n, in, out, FFTW_ESTIMATE );
    fftw_execute(plan_forward);
    
    log_complex_array("fft.csv", out, nc);
    
    fftw_destroy_plan(plan_forward);
    
    // resize input array
    in = doubleRealloc(in, n, in_length);    
    log_double_array("fft_input_resized_back.csv", in, in_length);
	
    return out;
}

double* padded_IFFT(fftw_complex *in, int in_length)
{
	int i;
    int n;
	int unpadded;
    fftw_plan plan_backward;
    double *out;
    
	log_double("IFFT input array length:", in_length);	
    log_complex_array("ifft_input.csv", in, in_length);
    
	unpadded = ( in_length - 1 ) * 2;	

    // n is padded size	
    n = unpadded > 256 ? padded_size(unpadded) : unpadded;
	//in.resize(padded/2+1);
	
    /*
    Set up an arrray to hold the backtransformed data OUT,
    get a "plan", and execute the plan to backtransform the IN
    FFT coefficients to OUT.
    */
    out = fftw_malloc ( sizeof ( double ) * n );
    
	log_double("IFFT output array length:", n);	
	
    plan_backward = fftw_plan_dft_c2r_1d ( n, in, out, FFTW_ESTIMATE );    
    fftw_execute ( plan_backward );

	// divide by n
    for ( i = 0; i < n; i++ )
    {
        out[i] = out[i] / ( double ) ( n );
    }
	
    log_double_array("ifft.csv", out, n);

    fftw_destroy_plan(plan_backward);
	//in.resize(n/2+1);
	
    return out;
}

