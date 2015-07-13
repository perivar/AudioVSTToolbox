using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using CommonUtils;
using CommonUtils.CommonMath.FFT;

using ARSS; // for the util classes

namespace ArssSpectrogram
{
	// Converted from C++ to C#
	// https://github.com/krajj7/spectrogram/blob/master/spectrogram.cpp
	public static class SpectrogramUtils
	{
		#region FFT and IFFT
		public static Complex[] padded_FFT(ref double[] @in)
		{
			Debug.Assert(@in.Length > 0);
			int n = @in.Length;
			
			int padded = n > 256 ? Util.NextLowPrimes(n) : n;
			Array.Resize<double>(ref @in, padded);

			// 4096 real numbers on input processed by FFTW dft_r2c_1d transform gives
			// 4096/2+1 = 2049 complex numbers at output
			// prepare the input arrays
			var fftwInput = new FFTW.DoubleArray(@in);
			
			int complexSize = (padded >> 1) + 1; // this is the same as (padded / 2 + 1);
			var fftwOutput = new FFTW.ComplexArray(complexSize);
			
			FFTW.ForwardTransform(fftwInput, fftwOutput);
			
			Array.Resize<double>(ref @in, n);
			
			// free up memory
			GC.Collect();
			
			return FFTUtils.ComplexDoubleToComplex(fftwOutput.ComplexValues);
		}

		public static double[] padded_IFFT(ref Complex[] @in, bool doProperScaling=false)
		{
			Debug.Assert(@in.Length > 1);
			
			int originalLength = @in.Length;
			int n = (@in.Length - 1) * 2;
			
			int padded = n > 256 ? Util.NextLowPrimes(n) : n;

			Array.Resize<Complex>(ref @in, padded / 2 + 1);
			
			// prepare the input arrays
			var complexDouble = FFTUtils.ComplexToComplexDouble(@in);
			var fftwBackwardInput = new FFTW.ComplexArray(complexDouble);
			var fftwBackwardOutput = new FFTW.DoubleArray(padded);
			
			// this method needs that the backwards transform uses the output.length as it's N
			// i.e. FFTW.dft_c2r_1d(output.Length, input.Handle, output.Handle, Flags.Estimate);
			FFTW.BackwardTransform(fftwBackwardInput, fftwBackwardOutput);
			
			double[] @out = null;
			if (doProperScaling) {
				@out = fftwBackwardOutput.ValuesDivedByN;
			} else {
				// in the original method it wasn't scaled correctly (meaning ValuesDivedByN)
				@out = fftwBackwardOutput.Values;
			}
			
			Array.Resize<Complex>(ref @in, n / 2 + 1);
			
			// free up memory
			GC.Collect();

			return @out;
		}
		
		public static object CsvDoubleParser(string[] splittedLine)
		{
			// only store the second element (the first is a counter)
			return double.Parse(splittedLine[1]);
		}
		
		public static string CvsComplexFormatter(object line, int lineCounter, string columnSeparator)
		{
			var elements = new List<string>();
			var complex = (CommonUtils.CommonMath.FFT.Complex) line;
			
			elements.Add(String.Format("{0,4}", lineCounter));
			elements.Add(String.Format("{0,12:N6}", complex.Re));
			elements.Add(String.Format("{0,12:N6}", complex.Im));
			
			return string.Join(columnSeparator, elements);
		}

		public static string CvsDoubleFormatter(object line, int lineCounter, string columnSeparator)
		{
			var elements = new List<string>();
			var d = (double) line;
			
			elements.Add(String.Format("{0,4}", lineCounter));
			elements.Add(String.Format("{0,12:N6}", d));
			
			return string.Join(columnSeparator, elements);
		}
		#endregion
		
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
			//Console.Out.WriteLine("Resample(data size: {0}, len: {1}", @in.Length, len);
			
			if (@in.Length == len)
				return @in;

			double ratio = (double)len / @in.Length;
			if (ratio >= 256) {
				return Resample(Resample(@in, @in.Length*50), len);
			} else if (ratio <= 1.0/256) {
				return Resample(Resample(@in, @in.Length/50), len);
			}

			return MathUtils.FloatToDouble(MathUtils.Resample(MathUtils.DoubleToFloat(@in), len));
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
			
			double[] envelope = padded_IFFT(ref band);
			double[] shifted_signal = padded_IFFT(ref shifted);

			for (int i = 0; i < envelope.Length; ++i) {
				// TODO: why not the magnitude?
				envelope[i] = envelope[i]*envelope[i] + shifted_signal[i]*shifted_signal[i];
			}

			return envelope;
		}

		#region Windows
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
		#endregion

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

		public static double RandomDoubleMinus1ToPlus1()
		{
			return RandomUtils.NextDoubleMinus1ToPlus1();
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
				//double phase = (2 * RandomDouble() -1) * Math.PI; //+-pi random phase
				double phase = RandomDoubleMinus1ToPlus1() * Math.PI; // random phase between -pi and +pi
				var complex = new Complex(mag * Math.Cos(phase), mag * Math.Sin(phase));
				res[i] = complex;
			}
			return res;
		}
	}
}