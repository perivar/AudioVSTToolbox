#ifndef FFT_HPP
#define FFT_HPP

/** \file fft.hpp
 * \brief Contains utility functions for performing the fast fourier transform and its inverse.
 *
 * It uses the FFTW3 library to perform the transforms.  For better performance, the functions temporarily change the size of the input vector by padding it with zeros to a size that can be expressed as a product of small primes, that is 2^x * 3^y * 5^z.
 */

#include <vector>
#include <complex>
#include "fftw3.h"
#include "types.hpp"

/// Performs a fast fourier transform.
/** The input vector is padded with zeros for better performance and shrunk again to original size when the transform is done. */
complex_vec padded_FFT(real_vec& in);
/// Performs a fast inverse fourier transform.
/** The input vector is destroyed in the process! */
real_vec padded_IFFT(complex_vec& in);

#endif
