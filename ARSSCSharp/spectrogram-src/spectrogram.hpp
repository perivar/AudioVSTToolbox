#ifndef SPECTROGRAM_HPP
#define SPECTROGRAM_HPP

/** \file spectrogram.hpp
 *  \brief Definitions for classes used for spectrogram analysis and generation.
 */

#include <vector>
#include <complex>
#include <memory>
#include "fft.hpp"

/// Represents the window function used for spectrogram generation.
enum Window 
{
    WINDOW_HANN, /**< See http://en.wikipedia.org/wiki/Hann_window */
    WINDOW_BLACKMAN, /**< See http://en.wikipedia.org/wiki/Window_function#Blackman_windows */
    WINDOW_RECTANGULAR, /**< Doesn't do anything. */
    WINDOW_TRIANGULAR /**< http://en.wikipedia.org/wiki/Triangular_window#Triangular_window_.28non-zero_end-points.29 */
};
/// Represents the linear or logarithmic mode for frequency and intensity axes.
enum AxisScale {SCALE_LINEAR, SCALE_LOGARITHMIC};
/// Represents spectrogram synthesis mode.
enum SynthesisType {SYNTHESIS_SINE, SYNTHESIS_NOISE};
/// Represents the brightness correction used in spectrogram generation.
enum BrightCorrection {BRIGHT_NONE, BRIGHT_SQRT};

// --

typedef std::pair<int,int> intpair;

/// Used to divide the frequency domain into suitable intervals.
/** Each interval represents a horizontal band in a spectrogram. */
class Filterbank
{
    public:
        static std::auto_ptr<Filterbank> get_filterbank(AxisScale type,
                double scale, double base, double hzbandwidth, double overlap);

        Filterbank(double scale);
        /// Returns start-finish indexes for a given filterband.
        virtual intpair get_band(int i) const = 0;
        /// Returns the index of the filterband's center.
        virtual int get_center(int i) const = 0;
        /// Estimated total number of intervals.
        virtual int num_bands_est(double maxfreq) const = 0;
        virtual ~Filterbank();
    protected:
        /// The proportion of frequency versus vector indices.
        const double scale_;
};

/// Divides the frequency domain to intervals of constant bandwidth.
class LinearFilterbank : public Filterbank
{
    public:
        LinearFilterbank(double scale, double base, double hzbandwidth,
                double overlap);
        intpair get_band(int i) const;
        int get_center(int i) const;
        int num_bands_est(double maxfreq) const;
    private:
        const double bandwidth_;
        const int startidx_;
        const double step_;
};

/// Divides the frequency domain to intervals with variable (logarithmic, constant-Q) bandwidth.
class LogFilterbank : public Filterbank
{
    public:
        LogFilterbank(double scale, double base, double centsperband,
                double overlap);
        intpair get_band(int i) const;
        int get_center(int i) const;
        int num_bands_est(double maxfreq) const;
    private:
        const double centsperband_;
        const double logstart_;
        const double logstep_;
};

#endif
