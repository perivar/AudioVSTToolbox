using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

// Copied from DanceStixUtil:
// "Motion Pattern Recognition for Interactive Dance" by Henning Pohl

namespace CommonUtils.FFT {
	internal class FFTW {
		
		[Flags]
		internal enum Flags {
			Measure = 0,
			DestroyInput = 1,
			Unaligned = 2,
			ConserveMemory = 4,
			Exhaustive = 8,
			PreserveInput = 16,
			Patient = 32,
			Estimate = 64
		}

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_malloc", ExactSpelling = true)]
		private static extern IntPtr malloc(int n);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_free", ExactSpelling = true)]
		private static extern void free(IntPtr ptr);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_execute", ExactSpelling = true)]
		private static extern void execute(IntPtr plan);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_flops", ExactSpelling = true)]
		private static extern void flops(IntPtr plan, ref double adds, ref double mults, ref double fmas);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_print_plan", ExactSpelling = true)]
		private static extern void print_plan(IntPtr plan);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_plan_dft_r2c_1d", ExactSpelling = true)]
		private static extern IntPtr dft_r2c_1d(int n, IntPtr input, IntPtr output, Flags flags);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_plan_dft_c2r_1d", ExactSpelling = true)]
		private static extern IntPtr dft_c2r_1d(int n, IntPtr input, IntPtr output, Flags flags);

		[DllImport("libfftw3-3.dll", EntryPoint = "fftw_destroy_plan", ExactSpelling = true)]
		private static extern void destroy_plan(IntPtr plan);

		public static void ForwardTransform(FFTW.DoubleArray input, FFTW.ComplexArray output) {
			IntPtr plan = FFTW.dft_r2c_1d(input.Length, input.Handle, output.Handle, Flags.Estimate);

			//FFTW.print_plan(plan);
			FFTW.execute(plan);
			FFTW.destroy_plan(plan);
		}

		public static void BackwardTransform(FFTW.ComplexArray input, FFTW.DoubleArray output) {
			// TODO: make sure to use input.Length and not the output.Length ?!
			IntPtr plan = FFTW.dft_c2r_1d(input.Length, input.Handle, output.Handle, Flags.Estimate);

			//FFTW.print_plan(plan);
			FFTW.execute(plan);
			FFTW.destroy_plan(plan);
		}


		internal class ComplexArray {
			public IntPtr Handle { get; private set; }
			public int Length { get; private set; }

			/// <summary>
			/// Return the values from a complex array
			/// i.e. real*real + imag*imag
			/// </summary>
			public double[] Values {
				get {
					double[] buffer = new double[Length * 2];
					Marshal.Copy(Handle, buffer, 0, Length * 2);

					double[] output = new double[Length];
					for (int i = 0; i < Length; i++)
					{
						double re = buffer[2 * i];
						double img = buffer[2 * i + 1];
						output[i] = Math.Pow(re, 2.0) + Math.Pow(img, 2.0);
					}
					return output;
				}
			}

			/// <summary>
			/// Return the real column from a complex array
			/// </summary>
			public double[] Real {
				get {
					double[] buffer = new double[Length * 2];
					Marshal.Copy(Handle, buffer, 0, Length * 2);

					double[] output = new double[Length];
					for (int i = 0; i < Length; i++)
					{
						double re = buffer[2 * i];
						double img = buffer[2 * i + 1];
						output[i] = re;
					}
					return output;
				}
			}

			/// <summary>
			/// Return the imaginary column from a complex array
			/// </summary>
			public double[] Imag {
				get {
					double[] buffer = new double[Length * 2];
					Marshal.Copy(Handle, buffer, 0, Length * 2);

					double[] output = new double[Length];
					for (int i = 0; i < Length; i++)
					{
						double re = buffer[2 * i];
						double img = buffer[2 * i + 1];
						output[i] = img;
					}
					return output;
				}
			}

			/// <summary>
			/// Return the absolute column from a complex array
			/// i.e. the array alternates between a real and an imaginary value
			/// The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
			/// </summary>
			public double[] Abs {
				get {
					double[] buffer = new double[Length * 2];
					Marshal.Copy(Handle, buffer, 0, Length * 2);

					double[] output = new double[Length];
					for (int i = 0; i < Length; i++)
					{
						double re = buffer[2 * i];
						double img = buffer[2 * i + 1];
						output[i] = Math.Sqrt(re*re + img*img);
					}
					return output;
				}
			}
			
			/// <summary>
			/// FFTW tries to take advantage of the fact that if the input array is real (not complex),
			/// then there is some redundancy in the output vector.
			/// In fact, if the input array is real, the fourier component of a frequency
			/// w is equal to the complex conjugate of the fourier component of the freq -w.
			/// So half the vector can be known from the other half.
			/// That means that in fact, we only need (N/2)+1 complex numbers for the output (as opposed to N complex numbers).
			/// I.e. for an real input array of size 256, we need 256/2+1 = 129 complex numbers
			/// </summary>
			/// <param name="length">Actual length of one column of data (i.e. N/2 + 1)</param>
			public ComplexArray(int length) {
				Length = length;
				Handle = FFTW.malloc(Marshal.SizeOf(typeof(double)) * Length * 2);
			}
			
			/// <summary>
			/// Initialize a complex array using real and imaginary data
			/// that has the length of N/2+1.
			/// </summary>
			/// <param name="realData">real data</param>
			/// <param name="imagData">imaginary data</param>
			public ComplexArray(double[] realData, double[] imagData) {
				if (realData.Length != imagData.Length) throw new ArgumentException(
					"data length for real data [" + realData.Length + "] is not the same as imaginary data [" + imagData.Length + "]");

				// make sure to have room for both arrays
				int length = realData.Length * 2;
				double[] buffer = new double[length];
				for (int i = 0; i < realData.Length; i++)
				{
					buffer[2 * i] 		= realData[i];
					buffer[2 * i + 1] 	= imagData[i];
				}
				Handle = FFTW.malloc(Marshal.SizeOf(typeof(double)) * length);
				Marshal.Copy(buffer, 0, Handle, length);
				
				// Update length to reflect a N/2+1 input
				Length = (realData.Length - 1) * 2;
			}
			
			~ComplexArray() {
				FFTW.free(Handle);
			}
		}

		internal class DoubleArray {
			public IntPtr Handle { get; private set; }
			public int Length { get; private set; }

			public double[] Values {
				get {
					double[] buffer = new double[Length];
					Marshal.Copy(Handle, buffer, 0, Length);
					return buffer;
				}
			}

			public double[] ValuesDivedByN {
				get {
					double[] buffer = this.Values;
					for (int i = 0; i < buffer.Length; i++) {
						buffer[i] /= buffer.Length;
					}
					return buffer;
				}
			}
			
			public DoubleArray(int length) {
				Length = length;
				Handle = FFTW.malloc(Marshal.SizeOf(typeof(double)) * Length);
			}

			public DoubleArray(IEnumerable<double> data, Func<int, int, double> window) {
				Length = data.Count();
				Handle = FFTW.malloc(Marshal.SizeOf(typeof(double)) * Length);
				double[] buffer = new double[Length];
				for (int i = 0; i < Length; i++) {
					buffer[i] = data.ElementAt(i) * window(i, Length);
				}
				Marshal.Copy(buffer, 0, Handle, Length);
			}

			public DoubleArray(double[] data) {
				Length = data.Length;
				Handle = FFTW.malloc(Marshal.SizeOf(typeof(double)) * Length);
				Marshal.Copy(data, 0, Handle, Length);
			}

			~DoubleArray() {
				FFTW.free(Handle);
			}
		}
	}
}
