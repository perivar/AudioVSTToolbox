using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

// Copied from DanceStixUtil:
// "Motion Pattern Recognition for Interactive Dance" by Henning Pohl

namespace CommonUtils.FFT {
	public class FFT {
		private static double HannWindow(int i, int n) {
			double t = (2.0 * Math.PI * i) / (n - 1.0);
			return 0.5 * (1.0 - Math.Cos(t));
		}

		private static double GetComponentWeightCenter(List<double[]> a, int size, int component) {
			double[] windowedValues = a.GetWindowedDimension(component, size, HannWindow);

			FFTW.DoubleArray fftwInput = new FFTW.DoubleArray(windowedValues);
			FFTW.ComplexArray fftwOutput = new FFTW.ComplexArray((size >> 1) + 1);

			FFTW.ForwardTransform(fftwInput, fftwOutput);

			return fftwOutput.Values.GetWeightCenter();
		}

		public static double[] GetFeatureVector(List<double[]> a) {
			int pow2Len = 1;
			while (pow2Len < a.Count) {
				pow2Len <<= 1;
			}

			double[] aCoeffs = new double[a[0].Length];
			for (int i = 0; i < a[0].Length; i++) {
				aCoeffs[i] = GetComponentWeightCenter(a, pow2Len, i);
			}

			return aCoeffs;
		}

		
	}
}
