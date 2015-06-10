using System;
using System.Runtime.InteropServices;

namespace fftwlib
{
	#region Single Precision
	/// <summary>
	/// To simplify FFTW memory management
	/// </summary>
	public class fftwf_complexarray
	{
		private IntPtr handle;
		public IntPtr Handle
		{get {return handle;}}

		private int length;
		public int Length
		{get {return length;}}

		/// <summary>
		/// Creates a new array of complex numbers
		/// </summary>
		/// <param name="length">Logical length of the array</param>
		public fftwf_complexarray(int length)
		{
			this.length = length;
			this.handle = fftwf.malloc(this.length*8);
		}

		/// <summary>
		/// Creates an FFTW-compatible array from array of floats, initializes to single precision only
		/// </summary>
		/// <param name="data">Array of floats, alternating real and imaginary</param>
		public fftwf_complexarray(float[] data)
		{
			this.length = data.Length/2;
			this.handle = fftwf.malloc(this.length*8);
			Marshal.Copy(data,0,handle,this.length*2);
		}

		~fftwf_complexarray()
		{
			fftwf.free(handle);
		}
	}
	
	/// <summary>
	/// Creates, stores, and destroys fftw plans
	/// </summary>
	public class fftwf_plan
	{
		protected IntPtr handle;
		public IntPtr Handle
		{get{return handle;}}

		public void Execute()
		{
			fftwf.execute(handle);
		}

		~fftwf_plan()
		{
			fftwf.destroy_plan(handle);
		}

		#region Plan Creation
		//Complex<->Complex transforms
		public static fftwf_plan dft_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_1d(n, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftwf_plan dft_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftwf_plan dft_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftwf_plan dft(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft(rank, n, input.Handle, output.Handle, direction, flags);
			return p;
		}

		//Real->Complex transforms
		public static fftwf_plan dft_r2c_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_r2c_1d(n, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_r2c_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_r2c_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_r2c(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_r2c(rank, n, input.Handle, output.Handle, flags);
			return p;
		}

		//Complex->Real
		public static fftwf_plan dft_c2r_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_c2r_1d(n, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_c2r_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_c2r_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftwf_plan dft_c2r(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.dft_c2r(rank, n, input.Handle, output.Handle, flags);
			return p;
		}

		//Real<->Real
		public static fftwf_plan r2r_1d(int n, fftwf_complexarray input, fftwf_complexarray output, fftw_kind kind, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.r2r_1d(n, input.Handle, output.Handle, kind, flags);
			return p;
		}

		public static fftwf_plan r2r_2d(int nx, int ny, fftwf_complexarray input, fftwf_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags);
			return p;
		}
		
		public static fftwf_plan r2r_3d(int nx, int ny, int nz, fftwf_complexarray input, fftwf_complexarray output,
		                                fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.r2r_3d(nx, ny, nz, input.Handle, output.Handle,
			                        kindx, kindy, kindz, flags);
			return p;
		}

		public static fftwf_plan r2r(int rank, int[] n, fftwf_complexarray input, fftwf_complexarray output,
		                             fftw_kind[] kind, fftw_flags flags)
		{
			var p = new fftwf_plan();
			p.handle = fftwf.r2r(rank, n, input.Handle, output.Handle,
			                     kind, flags);
			return p;
		}
		#endregion
	}
	#endregion

	#region Double Precision
	/// <summary>
	/// So FFTW can manage its own memory nicely
	/// </summary>
	public class fftw_complexarray
	{
		private IntPtr handle;
		public IntPtr Handle
		{
			get {return handle;}
		}

		private int length;
		public int Length
		{
			get {return length;}
		}

		/// <summary>
		/// Return the values
		/// </summary>
		public double[] Values {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);
				return buffer;
			}
		}
		
		/// <summary>
		/// Return the values
		/// </summary>
		public double[] ValuesDividedByN {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length * 2];
				for (int i = 0; i < Length * 2; i++)
				{
					output[i] = buffer[i] / (Length * 2);
				}
				return output;
			}
		}
		
		/// <summary>
		/// Return the real column from a complex array
		/// </summary>
		public double[] Real {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
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
		/// Return the real column from a complex array
		/// </summary>
		public double[] RealDividedByN {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
				for (int i = 0; i < Length; i++)
				{
					double re = buffer[2 * i];
					double img = buffer[2 * i + 1];
					output[i] = re / Length;
				}
				return output;
			}
		}
		
		/// <summary>
		/// Return the imaginary column from a complex array
		/// </summary>
		public double[] Imag {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
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
		/// Return the imaginary column from a complex array
		/// </summary>
		public double[] ImagDividedByN {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
				for (int i = 0; i < Length; i++)
				{
					double re = buffer[2 * i];
					double img = buffer[2 * i + 1];
					output[i] = img / Length;
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
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
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
		/// Return the absolute column from a complex array
		/// i.e. the array alternates between a real and an imaginary value
		/// The MATLAB abs() is equal to sqrt(real(X).^2 + imag(X).^2)
		/// </summary>
		public double[] AbsDividedByN {
			get {
				var buffer = new double[Length * 2];
				Marshal.Copy(Handle, buffer, 0, Length * 2);

				var output = new double[Length];
				for (int i = 0; i < Length; i++)
				{
					double re = buffer[2 * i];
					double img = buffer[2 * i + 1];
					output[i] = Math.Sqrt(re*re + img*img) / Length;
				}
				return output;
			}
		}
		
		/// <summary>
		/// Creates a new array of complex numbers
		/// </summary>
		/// <param name="length">Logical length of the array</param>
		public fftw_complexarray(int length)
		{
			this.length = length;
			this.handle = fftw.malloc(this.length*16);
		}

		/// <summary>
		/// Creates an FFTW-compatible array from array of double, initializes to double precision only
		/// </summary>
		/// <param name="data">Array of double, alternating real and imaginary</param>
		public fftw_complexarray(double[] data)
		{
			this.length = data.Length/2;
			this.handle = fftw.malloc(this.length*16);
			Marshal.Copy(data,0,handle,this.length*2);
		}

		~fftw_complexarray()
		{
			fftw.free(handle);
		}
	}

	/// <summary>
	/// Creates, stores, and destroys fftw plans
	/// </summary>
	public class fftw_plan
	{
		protected IntPtr handle;
		public IntPtr Handle
		{get{return handle;}}

		public void Execute()
		{
			fftwf.execute(handle);
		}

		~fftw_plan()
		{
			fftw.destroy_plan(handle);
		}

		#region Plan Creation
		//Complex<->Complex transforms
		public static fftw_plan dft_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_1d(n, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftw_plan dft_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftw_plan dft_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags);
			return p;
		}

		public static fftw_plan dft(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft(rank, n, input.Handle, output.Handle, direction, flags);
			return p;
		}

		//Real->Complex transforms
		public static fftw_plan dft_r2c_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_r2c_1d(n, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_r2c_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_r2c_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_r2c(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_r2c(rank, n, input.Handle, output.Handle, flags);
			return p;
		}

		//Complex->Real
		public static fftw_plan dft_c2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_c2r_1d(n, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_c2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_c2r_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags);
			return p;
		}

		public static fftw_plan dft_c2r(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.dft_c2r(rank, n, input.Handle, output.Handle, flags);
			return p;
		}

		//Real<->Real
		public static fftw_plan r2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_kind kind, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.r2r_1d(n, input.Handle, output.Handle, kind, flags);
			return p;
		}

		public static fftw_plan r2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags);
			return p;
		}
		
		public static fftw_plan r2r_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output,
		                               fftw_kind kindx, fftw_kind kindy, fftw_kind kindz, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.r2r_3d(nx, ny, nz, input.Handle, output.Handle,
			                       kindx, kindy, kindz, flags);
			return p;
		}

		public static fftw_plan r2r(int rank, int[] n, fftw_complexarray input, fftw_complexarray output,
		                            fftw_kind[] kind, fftw_flags flags)
		{
			var p = new fftw_plan();
			p.handle = fftw.r2r(rank, n, input.Handle, output.Handle,
			                    kind, flags);
			return p;
		}
		#endregion
	}
	#endregion
}