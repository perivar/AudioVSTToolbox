#include "fft.hpp"
#include <cassert>

namespace
{
    /// Returns 1 if x is only made of small primes.
    int smallprimes(int x)
    {
        int p[3] = {2, 3, 5};
        for (int i = 0; i < 3; ++i)
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
}

complex_vec padded_FFT(real_vec& in)
{
    assert(in.size() > 0);
    const size_t n = in.size();
    const size_t padded = n > 256 ? padded_size(n) : n;
    in.resize(padded);

    complex_vec out(padded/2+1);

    fftwf_plan plan = fftwf_plan_dft_r2c_1d(padded,
            &in[0], (fftwf_complex*)&out[0], FFTW_ESTIMATE);
    fftwf_execute(plan);
    fftwf_destroy_plan(plan);

    in.resize(n);
    return out;
}

real_vec padded_IFFT(complex_vec& in)
{
    assert(in.size() > 1);
    const size_t n = (in.size()-1)*2;
    const size_t padded = n > 256 ? padded_size(n) : n;
    in.resize(padded/2+1);

    real_vec out(padded);

    // note: fftw3 destroys the input array for c2r transform
    fftwf_plan plan = fftwf_plan_dft_c2r_1d(padded,
            (fftwf_complex*)&in[0], &out[0], FFTW_ESTIMATE);
    fftwf_execute(plan);
    fftwf_destroy_plan(plan);

    in.resize(n/2+1);
    return out;
}
