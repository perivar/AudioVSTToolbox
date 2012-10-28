using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Copied from DanceStixUtil:
// "Motion Pattern Recognition for Interactive Dance" by Henning Pohl

namespace CommonUtils.FFT {
	public static class ListExtensions {
		public static int ElementError(this List<int> a, List<int> b) {
			if (a.Count != b.Count) {
				throw new ArgumentException("Lists need to be of equal length when computing element error.");
			}

			int error = 0;
			for (int i = 0; i < a.Count; i++) {
				if (a[i] != b[i])
					error++;
			}
			return error;
		}

		public static double[] GetRelativePower(this double[] a, double mean) {
			double[] output = new double[a.Length];

			for (int i = 0; i < a.Length; i++) {
				output[i] = a[i] / mean;
			}

			return output;
		}

		public static double[] Combine(this double[] a, double[] b) {
			double[] output = new double[a.Length + b.Length];

			for (int i = 0; i < a.Length; i++) {
				output[i] = a[i];
			}
			for (int i = 0; i < b.Length; i++) {
				output[i + a.Length] = b[i];
			}

			return output;
		}

		public static double[] Combine(this double[] a, double[] b, double[] c) {
			double[] output = new double[a.Length + b.Length + c.Length];

			for (int i = 0; i < a.Length; i++) {
				output[i] = a[i];
			}
			for (int i = 0; i < b.Length; i++) {
				output[i + a.Length] = b[i];
			}
			for (int i = 0; i < c.Length; i++) {
				output[i + a.Length + b.Length] = c[i];
			}

			return output;
		}

		public static double GetWeightCenter(this double[] a) {
			double center = 0.0;
			double isum = 1.0 / a.Sum();
			for(int i = 0; i < a.Length; i++) {
				center += (a[i] * isum) * ((i + 1.0) / a.Length);
			}
			return center;
		}

		public static double[] GetWindowedDimension(this List<double[]> a, int dim, int paddedSize, Func<int, int, double> w) {
			double[] output = new double[paddedSize];

			int pos = 0;
			while (pos < a.Count) {
				output[pos] = a[pos][dim] * w(pos, a.Count);
				pos++;
			}
			while (pos < paddedSize) {
				output[pos] = 0.0;
				pos++;
			}

			return output;
		}

		public static double[] GetMeans(this List<double[]> a) {
			double[] means = new double[a[0].Length];
			means.Initialize();

			foreach (var i in a) {
				for (int j = 0; j < means.Length; j++) {
					means[j] += i[j];
				}
			}

			for (int i = 0; i < means.Length; i++) {
				means[i] = means[i] / a.Count;
			}

			return means;
		}

		public static double[] GetStds(this List<double[]> a) {
			double[] means = a.GetMeans();
			double[] stds = new double[means.Length];
			stds.Initialize();

			foreach (var i in a) {
				for (int j = 0; j < means.Length; j++) {
					stds[j] += Math.Pow(i[j] - means[j], 2.0);
				}
			}

			for (int i = 0; i < stds.Length; i++) {
				stds[i] = Math.Sqrt(stds[i] / a.Count);
			}

			return stds;
		}

		public static double[] GetVariances(this List<double[]> a) {
			double[] means = a.GetMeans();
			double[] variances = new double[means.Length];
			variances.Initialize();

			foreach (var i in a) {
				for(int j = 0; j < means.Length; j++) {
					variances[j] += Math.Pow(i[j] - means[j], 2.0);
				}
			}

			for (int i = 0; i < variances.Length; i++) {
				variances[i] = variances[i] / a.Count;
			}

			return variances;
		}

		public static double CosineSimilarity(this double[] a, double[] b) {
			if (a.Length != b.Length) {
				throw new ArgumentException("Arrays need to be of equal size");
			}

			double aNorm, bNorm, dot;
			aNorm = bNorm = dot = 0.0;
			for (int i = 0; i < a.Length; i++) {
				dot += a[i] * b[i];
				aNorm += a[i] * a[i];
				bNorm += b[i] * b[i];
			}

			return dot / (Math.Sqrt(aNorm) * Math.Sqrt(bNorm));
		}

		public static List<double[]> Reduce(this List<List<double[]>> a) {
			int length = a[0].Count();
			foreach (var sublist in a) {
				if (sublist.Count() != length) {
					throw new ArgumentException("Sublist length needs to be equal for all sublists");
				}
			}

			List<double[]> output = new List<double[]>(length);
			for (int i = 0; i < length; i++) {
				double[] temp = new double[0];
				foreach (var sublist in a) {
					temp = temp.Combine(sublist[i]);
				}
				output.Add(temp);
			}

			return output;
		}
	}
}
