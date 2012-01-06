/*************************************************************************
 *  Compilation:  javac FFT.java
 *  Execution:    java FFT N
 *  Dependencies: Complex.java
 *
 *  Compute the FFT and inverse FFT of a length N complex sequence.
 *  Bare bones implementation that runs in O(N log N) time. Our goal
 *  is to optimize the clarity of the code, rather than performance.
 *
 *  Limitations
 *  -----------
 *   -  assumes N is a power of 2
 *
 *   -  not the most memory efficient algorithm (because it uses
 *      an object type for representing complex numbers and because
 *      it re-allocates memory for the subarray, instead of doing
 *      in-place or reusing a single temporary array)
 *
 *  From Introduction to Programming in Java [Amazon · Addison-Wesley]
 *************************************************************************/
 
 using System;
 
namespace CommonUtils.FFT
{
	public class FFT {

		// compute the FFT of x[], assuming its length is a power of 2
		public static Complex[] fft(Complex[] x) {
			int N = x.Length;

			// base case
			if (N == 1) return new Complex[] { x[0] };

			// radix 2 Cooley-Tukey FFT
			if (N % 2 != 0) { throw new Exception("N is not a power of 2"); }

			// fft of even terms
			Complex[] even = new Complex[N/2];
			for (int k = 0; k < N/2; k++) {
				even[k] = x[2*k];
			}
			Complex[] q = fft(even);

			// fft of odd terms
			Complex[] odd  = even;  // reuse the array
			for (int k = 0; k < N/2; k++) {
				odd[k] = x[2*k + 1];
			}
			Complex[] r = fft(odd);

			// combine
			Complex[] y = new Complex[N];
			for (int k = 0; k < N/2; k++) {
				double kth = -2 * k * Math.PI / N;
				Complex wk = new Complex(Math.Cos(kth), Math.Sin(kth));
				y[k]       = q[k].Plus(wk.Times(r[k]));
				y[k + N/2] = q[k].Minus(wk.Times(r[k]));
			}
			return y;
		}


		// compute the inverse FFT of x[], assuming its length is a power of 2
		public static Complex[] ifft(Complex[] x) {
			int N = x.Length;
			Complex[] y = new Complex[N];

			// take conjugate
			for (int i = 0; i < N; i++) {
				y[i] = x[i].Conjugate();
			}

			// compute forward FFT
			y = fft(y);

			// take conjugate again
			for (int i = 0; i < N; i++) {
				y[i] = y[i].Conjugate();
			}

			// divide by N
			for (int i = 0; i < N; i++) {
				y[i] = y[i].Times(1.0 / N);
			}

			return y;

		}

		// compute the circular convolution of x and y
		public static Complex[] cconvolve(Complex[] x, Complex[] y) {

			// should probably pad x and y with 0s so that they have same length
			// and are powers of 2
			if (x.Length != y.Length) { throw new Exception("Dimensions don't agree"); }

			int N = x.Length;

			// compute FFT of each sequence
			Complex[] a = fft(x);
			Complex[] b = fft(y);

			// point-wise multiply
			Complex[] c = new Complex[N];
			for (int i = 0; i < N; i++) {
				c[i] = a[i].Times(b[i]);
			}

			// compute inverse FFT
			return ifft(c);
		}


		// compute the linear convolution of x and y
		public static Complex[] convolve(Complex[] x, Complex[] y) {
			Complex ZERO = new Complex(0, 0);

			Complex[] a = new Complex[2*x.Length];
			for (int i = 0;        i <   x.Length; i++) a[i] = x[i];
			for (int i = x.Length; i < 2*x.Length; i++) a[i] = ZERO;

			Complex[] b = new Complex[2*y.Length];
			for (int i = 0;        i <   y.Length; i++) b[i] = y[i];
			for (int i = y.Length; i < 2*y.Length; i++) b[i] = ZERO;

			return cconvolve(a, b);
		}

		// display an array of Complex numbers to standard output
		public static void show(Complex[] x, string title) {
			System.Diagnostics.Debug.WriteLine(title);
			System.Diagnostics.Debug.WriteLine("-------------------");
			for (int i = 0; i < x.Length; i++) {
				System.Diagnostics.Debug.WriteLine(x[i]);
			}
		}


		/*********************************************************************
		 *  Test client and sample execution
		 *
		 *  % java FFT 4
		 *  x
		 *  -------------------
		 *  -0.03480425839330703
		 *  0.07910192950176387
		 *  0.7233322451735928
		 *  0.1659819820667019
		 *
		 *  y = fft(x)
		 *  -------------------
		 *  0.9336118983487516
		 *  -0.7581365035668999 + 0.08688005256493803i
		 *  0.44344407521182005
		 *  -0.7581365035668999 - 0.08688005256493803i
		 *
		 *  z = ifft(y)
		 *  -------------------
		 *  -0.03480425839330703
		 *  0.07910192950176387 + 2.6599344570851287E-18i
		 *  0.7233322451735928
		 *  0.1659819820667019 - 2.6599344570851287E-18i
		 *
		 *  c = cconvolve(x, x)
		 *  -------------------
		 *  0.5506798633981853
		 *  0.23461407150576394 - 4.033186818023279E-18i
		 *  -0.016542951108772352
		 *  0.10288019294318276 + 4.033186818023279E-18i
		 *
		 *  d = convolve(x, x)
		 *  -------------------
		 *  0.001211336402308083 - 3.122502256758253E-17i
		 *  -0.005506167987577068 - 5.058885073636224E-17i
		 *  -0.044092969479563274 + 2.1934338938072244E-18i
		 *  0.10288019294318276 - 3.6147323062478115E-17i
		 *  0.5494685269958772 + 3.122502256758253E-17i
		 *  0.240120239493341 + 4.655566391833896E-17i
		 *  0.02755001837079092 - 2.1934338938072244E-18i
		 *  4.01805098805014E-17i
		 *
		 *********************************************************************/

		public static void test(int N) {
			Complex[] x = new Complex[N];

			// original data
			Random rand = new Random();
			for (int i = 0; i < N; i++) {
				x[i] = new Complex(i, 0);
				x[i] = new Complex(-2* rand.NextDouble() + 1, 0);
			}
			show(x, "x");

			// FFT of original data
			Complex[] y = fft(x);
			show(y, "y = fft(x)");

			// take inverse FFT
			Complex[] z = ifft(y);
			show(z, "z = ifft(y)");

			// circular convolution of x with itself
			Complex[] c = cconvolve(x, x);
			show(c, "c = cconvolve(x, x)");

			// linear convolution of x with itself
			Complex[] d = convolve(x, x);
			show(d, "d = convolve(x, x)");
		}
	}
}