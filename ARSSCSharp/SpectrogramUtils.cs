using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using CommonUtils;
using CommonUtils.FFT;

// Converted from C++ to C#
// https://github.com/krajj7/spectrogram/blob/master/spectrogram.cpp
public static class SpectrogramUtils
{
	public static double[] Zeros(double[] signal, int length) {
		if (length > signal.Length) {
			Array.Resize<double>(ref signal, length);
		}
		return signal;
	}

	public static Complex[] Zeros(Complex[] signal, int length) {
		if (length > signal.Length) {
			Array.Resize<Complex>(ref signal, length);
		}
		return signal;
	}

	public static double[] PaddedIFFT(Complex[] complexSignal) {
		
		int N = MathUtils.NextPowerOfTwo(complexSignal.Length);
		if (N <= 4096) {
			complexSignal = Zeros(complexSignal, N);
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		} else {
			N = 4096;
			Array.Resize(ref complexSignal, N);
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		}
		
		// get the result
		var fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static double[] PaddedIFFT(double[] signal) {
		
		int N = signal.Length;
		Complex[] complexSignal = FFTUtils.DoubleToComplex(signal);
		Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		
		// get the result
		var fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static double[] PaddedFFT(Complex[] complexSignal) {

		int N = complexSignal.Length;
		Fourier.FFT(complexSignal, N, FourierDirection.Forward);

		// get the result
		var fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static Complex[] PaddedFFT(double[] signal) {
		
		int N = MathUtils.NextPowerOfTwo(signal.Length);
		signal = Zeros(signal, N);

		double[] signal_fft = FFTUtils.FFT(signal);
		Complex[] complexSignal = FFTUtils.ComplexDoubleToComplex(signal_fft);
		
		return complexSignal;
	}
	
	public static double Log10Scale(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);
		return Math.Log10(1 + 9 * val);
	}

	public static double Log10ScaleInverse(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);
		return (Math.Pow(10, val) -1) / 9;
	}

	// cent = octave/1200
	public static double Cent2Freq(double cents)
	{
		return Math.Pow(2, cents / 1200);
	}
	
	public static double Freq2Cent(double freq)
	{
		return Math.Log(freq) / Math.Log(2) * 1200;
	}
	
	public static double Cent2Oct(double cents)
	{
		return cents / 1200;
	}
	
	public static double Oct2Cent(double oct)
	{
		return oct * 1200;
	}

	public static void Shift90Degrees(ref Complex x)
	{
		x = x.GetConjugate();
	}

	public static double[] Resample(double[] @in, int len)
	{
		Debug.Assert(len > 0);
		Console.Out.WriteLine("Resample(data size: {0}, len: {1}", @in.Length, len);
		
		if (@in.Length == len)
			return @in;

		return MathUtils.FloatToDouble(MathUtils.ResampleToArbitrary(MathUtils.DoubleToFloat(@in), len));
	}

	public static double[] GetEnvelope(ref Complex[] band)
	{
		Debug.Assert(band.Length > 1);

		// copy + phase shift
		var shifted = new Complex[band.Length];
		for (int i = 0; i < band.Length; i++) {
			var compl = new Complex(band[i]);
			Shift90Degrees(ref compl);
			shifted[i] = compl;
		}
		
		double[] envelope = PaddedIFFT(band);
		double[] shifted_signal = PaddedIFFT(shifted);

		for (int i = 0; i < envelope.Length; ++i) {
			envelope[i] = envelope[i]*envelope[i] + shifted_signal[i]*shifted_signal[i];
		}

		return envelope;
	}

	public static double BlackmanWindow(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return Math.Max(0.42 - 0.5 * Math.Cos(2 * Math.PI * x) + 0.08 * Math.Cos(4 * Math.PI * x), 0.0);
	}

	public static double HannWindow(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return 0.5 * (1 - Math.Cos(x * 2 * Math.PI));
	}

	public static double TriangularWindow(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return 1 - Math.Abs( 2 * (x - 0.5));
	}

	public static double WindowCoef(double x, Window window)
	{
		Debug.Assert(x >= 0 && x <= 1);
		
		if (window == Window.WINDOW_RECTANGULAR)
			return 1.0;
		switch (window) {
			case Window.WINDOW_HANN:
				return SpectrogramUtils.HannWindow(x);
			case Window.WINDOW_BLACKMAN:
				return SpectrogramUtils.BlackmanWindow(x);
			case Window.WINDOW_TRIANGULAR:
				return SpectrogramUtils.TriangularWindow(x);
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	public static double CalcIntensity(double val, AxisScale intensity_axis)
	{
		Debug.Assert(val >= 0 && val <= 1);
		switch (intensity_axis) {
			case AxisScale.SCALE_LOGARITHMIC:
				return SpectrogramUtils.Log10Scale(val);
			case AxisScale.SCALE_LINEAR:
				return val;
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	public static double CalcIntensityInv(double val, AxisScale intensity_axis)
	{
		Debug.Assert(val >= 0 && val <= 1);
		switch (intensity_axis) {
			case AxisScale.SCALE_LOGARITHMIC:
				return SpectrogramUtils.Log10ScaleInverse(val);
			case AxisScale.SCALE_LINEAR:
				return val;
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	// to <0,1> (cutoff negative)
	public static void NormalizeImageCutoffNegative(ref List<double[]> data)
	{
		// Find maximum number when all numbers are made positive.
		double max = data.Max((b) => b.Max((v) => Math.Abs(v)));
		
		Debug.Assert(max > 0);
		if (max == 0.0f)
			return;

		// divide by max and return
		data = data.Select(i => i.Select(j => Math.Abs(j)/max).ToArray()).ToList();
	}

	// to <-1,1>
	public static void NormalizeSignal(ref double[] data)
	{
		// Find maximum number when all numbers are made positive.
		double max = data.Max((b) => Math.Abs(b));
		
		Debug.Assert(max > 0);
		if (max == 0.0f)
			return;

		// divide by max and return
		data = data.Select(i => i/max).ToArray();
	}

	// random number from <0,1>
	public static double RandomDouble()
	{
		return RandomUtils.NextDouble();
	}

	public static double BrightnessCorrection(double intensity, BrightCorrection correction)
	{
		switch (correction) {
			case BrightCorrection.BRIGHT_NONE:
				return intensity;
			case BrightCorrection.BRIGHT_SQRT:
				return Math.Sqrt(intensity);
		}
		
		Debug.Assert(false);
		return 0.0;
	}

	public static Complex[] GetPinkNoise(int size)
	{
		var res = new Complex[size];
		for (int i = 0; i < (size+1)/2; ++i)
		{
			double mag = Math.Pow((double) i, -0.5f);
			double phase = (2 *SpectrogramUtils.RandomDouble()-1) * Math.PI; //+-pi random phase
			var complex = new Complex(mag * Math.Cos(phase), mag * Math.Sin(phase));
			res[i] = complex;
		}
		return res;
	}
}
