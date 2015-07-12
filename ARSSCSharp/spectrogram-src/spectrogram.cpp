#include "spectrogram.hpp"

#include <cmath>
#include <cstring>
#include <cassert>
#include <vector>
#include <algorithm>
#include <limits>
#include <iostream>

namespace 
{
    float log10scale(float val)
    {
        assert(val >= 0 && val <= 1);
        return std::log10(1+9*val);
    }

    float log10scale_inv(float val)
    {
        assert(val >= 0 && val <= 1);
        return (std::pow(10, val)-1)/9;
    }

    // cent = octave/1200
    double cent2freq(double cents)
    {
        return std::pow(2, cents/1200);
    }
    double freq2cent(double freq)
    {
        return std::log(freq)/std::log(2)*1200;
    }
    double cent2oct(double cents)
    {
        return cents/1200;
    }
    double oct2cent(double oct)
    {
        return oct*1200;
    }

    void shift90deg(Complex& x)
    {
        x = std::conj(Complex(x.imag(), x.real()));
    }

    /// Envelope detection: http://www.numerix-dsp.com/envelope.html
    real_vec get_envelope(complex_vec& band)
    {
        assert(band.size() > 1);

        // copy + phase shift
        complex_vec shifted(band);
        std::for_each(shifted.begin(), shifted.end(), shift90deg);

        real_vec envelope = padded_IFFT(band);
        real_vec shifted_signal = padded_IFFT(shifted);

        for (size_t i = 0; i < envelope.size(); ++i)
            envelope[i] = std::sqrt(envelope[i]*envelope[i] + 
                shifted_signal[i]*shifted_signal[i]);

        return envelope;
    }

    double blackman_window(double x)
    {
        assert(x >= 0 && x <= 1);
        return std::max(0.42 - 0.5*cos(2*PI*x) + 0.08*cos(4*PI*x), 0.0);
    }

    double hann_window(double x)
    {
        assert(x >= 0 && x <= 1);
        return 0.5*(1-std::cos(x*2*PI));
    }

    double triangular_window(double x)
    {
        assert(x >= 0 && x <= 1);
        return 1-std::abs(2*(x-0.5));
    }

    double window_coef(double x, Window window)
    {
        assert(x >= 0 && x <= 1);
        if (window == WINDOW_RECTANGULAR)
            return 1.0;
        switch (window)
        {
            case WINDOW_HANN:
                return hann_window(x);
            case WINDOW_BLACKMAN:
                return blackman_window(x);
            case WINDOW_TRIANGULAR:
                return triangular_window(x);
            default:
                assert(false);
        }
    }

    float calc_intensity(float val, AxisScale intensity_axis)
    {
        assert(val >= 0 && val <= 1);
        switch (intensity_axis)
        {
            case SCALE_LOGARITHMIC:
                return log10scale(val);
            case SCALE_LINEAR:
                return val;
            default:
                assert(false);
        }
    }

    float calc_intensity_inv(float val, AxisScale intensity_axis)
    {
        assert(val >= 0 && val <= 1);
        switch (intensity_axis)
        {
            case SCALE_LOGARITHMIC:
                return log10scale_inv(val);
            case SCALE_LINEAR:
                return val;
            default:
                assert(false);
        }
    }

    // to <0,1> (cutoff negative)
    void normalize_image(std::vector<real_vec>& data)
    {
        float max = 0.0f;
        for (std::vector<real_vec>::iterator it=data.begin();
                it!=data.end(); ++it)
            max = std::max(*std::max_element(it->begin(), it->end()), max);
        if (max == 0.0f)
            return;
        for (std::vector<real_vec>::iterator it=data.begin();
                it!=data.end(); ++it)
            for (real_vec::iterator i = it->begin(); i != it->end(); ++i)
                *i = std::abs(*i)/max;
    }

    // to <-1,1>
    void normalize_signal(real_vec& vector)
    {
        float max = 0;
        for (real_vec::iterator it = vector.begin(); it != vector.end(); ++it)
            max = std::max(max, std::abs(*it));
        //std::cout <<"max: "<<max<<"\n";
        assert(max > 0);
        for (real_vec::iterator it = vector.begin(); it != vector.end(); ++it)
            *it /= max;
    }

    // random number from <0,1>
    double random_double()
    {
        return ((double)rand()/(double)RAND_MAX);
    }

    float brightness_correction(float intensity, BrightCorrection correction)
    {
        switch (correction)
        {
            case BRIGHT_NONE:
                return intensity;
            case BRIGHT_SQRT:
                return std::sqrt(intensity);
        }
        assert(false);
    }

    /// Creates a random pink noise signal in the frequency domain
    /** \param size Desired number of samples in time domain (after IFFT). */
    complex_vec get_pink_noise(size_t size)
    {
        complex_vec res;
        for (size_t i = 0; i < (size+1)/2; ++i)
        {
            const float mag = std::pow((float) i, -0.5f);
            const double phase = (2*random_double()-1) * PI;//+-pi random phase 
            res.push_back(Complex(mag*std::cos(phase), mag*std::sin(phase)));
        }
        return res;
    }
}

Filterbank::Filterbank(double scale)
    : scale_(scale)
{
}

Filterbank::~Filterbank()
{
}

LinearFilterbank::LinearFilterbank(double scale, double base, 
        double hzbandwidth, double overlap)
    : Filterbank(scale)
    , bandwidth_(hzbandwidth*scale)
    , startidx_(std::max(scale_*base-bandwidth_/2, 0.0))
    , step_((1-overlap)*bandwidth_)
{
    //std::cout << "bandwidth: " << bandwidth_ << "\n";
    //std::cout << "step_: " << step_ << " hz\n";
    assert(step_ > 0);
}

int LinearFilterbank::num_bands_est(double maxfreq) const
{
    return (maxfreq*scale_-startidx_)/step_;
}

intpair LinearFilterbank::get_band(int i) const
{
    intpair out;
    out.first = startidx_ + i*step_;
    out.second = out.first + bandwidth_;
    return out;
}

int LinearFilterbank::get_center(int i) const
{
    return startidx_ + i*step_ + bandwidth_/2.0;
}

LogFilterbank::LogFilterbank(double scale, double base, 
        double centsperband, double overlap)
    : Filterbank(scale)
    , centsperband_(centsperband)
    , logstart_(freq2cent(base))
    , logstep_((1-overlap)*centsperband_)
{
    assert(logstep_ > 0);
    //std::cout << "bandwidth: " << centsperband_ << " cpb\n";
    //std::cout << "logstep_: " << logstep_ << " cents\n";
}

int LogFilterbank::num_bands_est(double maxfreq) const
{
    return (freq2cent(maxfreq)-logstart_)/logstep_+4;
}

int LogFilterbank::get_center(int i) const
{
    const double logcenter = logstart_ + i*logstep_;
    return cent2freq(logcenter)*scale_;
}

intpair LogFilterbank::get_band(int i) const
{
    const double logcenter = logstart_ + i*logstep_;
    const double loglow = logcenter - centsperband_/2.0;
    const double loghigh = loglow + centsperband_;
    intpair out;
    out.first = cent2freq(loglow)*scale_;
    out.second = cent2freq(loghigh)*scale_;
    //std::cout << "centerfreq: " << cent2freq(logcenter)<< "\n";
    //std::cout << "lowfreq: " << cent2freq(loglow) << " highfreq: "<<cent2freq(loghigh)<< "\n";
    return out;
}

std::auto_ptr<Filterbank> Filterbank::get_filterbank(AxisScale type,
        double scale, double base, double bandwidth, double overlap)
{
    Filterbank* filterbank;
    if (type == SCALE_LINEAR)
        filterbank=new LinearFilterbank(scale, base, bandwidth, overlap);
    else
        filterbank=new LogFilterbank(scale, base, bandwidth, overlap);
    return std::auto_ptr<Filterbank>(filterbank);
}
