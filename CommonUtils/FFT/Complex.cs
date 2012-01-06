/*************************************************************************
 *  Compilation:  javac Complex.java
 *  Execution:    java Complex
 *
 *  Data type for complex numbers.
 *
 *  The data type is "immutable" so once you create and initialize
 *  a Complex object, you cannot change it. The "final" keyword
 *  when declaring re and im enforces this rule, making it a
 *  compile-time error to change the .re or .im fields after
 *  they've been initialized.
 *
 *  % java Complex
 *  a            = 5.0 + 6.0i
 *  b            = -3.0 + 4.0i
 *  Re(a)        = 5.0
 *  Im(a)        = 6.0
 *  b + a        = 2.0 + 10.0i
 *  a - b        = 8.0 + 2.0i
 *  a * b        = -39.0 + 2.0i
 *  b * a        = -39.0 + 2.0i
 *  a / b        = 0.36 - 1.52i
 *  (a / b) * b  = 5.0 + 6.0i
 *  conj(a)      = 5.0 - 6.0i
 *  |a|          = 7.810249675906654
 *  tan(a)       = -6.685231390246571E-6 + 1.0000103108981198i
 *
 *  From Introduction to Programming in Java [Amazon · Addison-Wesley]
 *************************************************************************/

using System;

namespace CommonUtils.FFT
{

	public class Complex {
		
		private double re;   // the real part
		private double im;   // the imaginary part
		
		public double Re {
			get { return this.re; }
			set { this.re = value; }
		}
		public double Im
		{
			get { return this.im; }
			set { this.im = value; }
		}
		
		// create a new object with the given real and imaginary parts
		public Complex(double real, double imag) {
			re = real;
			im = imag;
		}

		// return a string representation of the invoking Complex object
		public override string ToString() {
			if (im == 0) return re + "";
			if (re == 0) return im + "i";
			if (im <  0) return re + " - " + (-im) + "i";
			return re + " + " + im + "i";
		}

		// return abs/modulus/magnitude and angle/phase/argument
		public double Abs()   { return Math.Sqrt(re*re + im*im); }  // Math.hypot(re, im);
		public double Phase() { return Math.Atan2(im, re); }  // between -pi and pi

		// return a new Complex object whose value is (this + b)
		public Complex Plus(Complex b) {
			Complex a = this;             // invoking object
			double real = a.re + b.re;
			double imag = a.im + b.im;
			return new Complex(real, imag);
		}

		// return a new Complex object whose value is (this - b)
		public Complex Minus(Complex b) {
			Complex a = this;
			double real = a.re - b.re;
			double imag = a.im - b.im;
			return new Complex(real, imag);
		}

		// return a new Complex object whose value is (this * b)
		public Complex Times(Complex b) {
			Complex a = this;
			double real = a.re * b.re - a.im * b.im;
			double imag = a.re * b.im + a.im * b.re;
			return new Complex(real, imag);
		}

		// scalar multiplication
		// return a new object whose value is (this * alpha)
		public Complex Times(double alpha) {
			return new Complex(alpha * re, alpha * im);
		}

		// return a new Complex object whose value is the conjugate of this
		public Complex Conjugate() {  return new Complex(re, -im); }

		// return a new Complex object whose value is the reciprocal of this
		public Complex Reciprocal() {
			double scale = re*re + im*im;
			return new Complex(re / scale, -im / scale);
		}

		// return a / b
		public Complex Divides(Complex b) {
			Complex a = this;
			return a.Times(b.Reciprocal());
		}

		// return a new Complex object whose value is the complex exponential of this
		public Complex Exp() {
			return new Complex(Math.Exp(re) * Math.Cos(im), Math.Exp(re) * Math.Sin(im));
		}

		// return a new Complex object whose value is the complex sine of this
		public Complex Sin() {
			return new Complex(Math.Sin(re) * Math.Cosh(im), Math.Cos(re) * Math.Sinh(im));
		}

		// return a new Complex object whose value is the complex cosine of this
		public Complex Cos() {
			return new Complex(Math.Cos(re) * Math.Cosh(im), -Math.Sin(re) * Math.Sinh(im));
		}

		// return a new Complex object whose value is the complex tangent of this
		public Complex Tan() {
			return Sin().Divides(Cos());
		}
		
		// a static version of plus
		public static Complex Plus(Complex a, Complex b) {
			double real = a.re + b.re;
			double imag = a.im + b.im;
			Complex sum = new Complex(real, imag);
			return sum;
		}

		// sample client for testing
		public static void test() {
			Complex a = new Complex(5.0, 6.0);
			Complex b = new Complex(-3.0, 4.0);
						
			System.Diagnostics.Debug.WriteLine("a            = " + a);
			System.Diagnostics.Debug.WriteLine("b            = " + b);
			System.Diagnostics.Debug.WriteLine("Re(a)        = " + a.Re);
			System.Diagnostics.Debug.WriteLine("Im(a)        = " + a.Im);
			System.Diagnostics.Debug.WriteLine("b + a        = " + b.Plus(a));
			System.Diagnostics.Debug.WriteLine("a - b        = " + a.Minus(b));
			System.Diagnostics.Debug.WriteLine("a * b        = " + a.Times(b));
			System.Diagnostics.Debug.WriteLine("b * a        = " + b.Times(a));
			System.Diagnostics.Debug.WriteLine("a / b        = " + a.Divides(b));
			System.Diagnostics.Debug.WriteLine("(a / b) * b  = " + a.Divides(b).Times(b));
			System.Diagnostics.Debug.WriteLine("conj(a)      = " + a.Conjugate());
			System.Diagnostics.Debug.WriteLine("|a|          = " + a.Abs());
			System.Diagnostics.Debug.WriteLine("tan(a)       = " + a.Tan());
		}
	}
}