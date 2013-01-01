using System;
using System.IO;
using System.Diagnostics;

using CommonUtils;

// For FTTW
using fftwlib;

namespace CommonUtils.FFT
{
	/// <summary>
	/// FFTTesting Class
	/// 
	/// Output of testing TimeAll(1000)
	/// ==================================
	/// FFTWTestUsingDouble: 					Time used: 1490,5693 ms
	/// FFTWTestUsingDoubleFFTWLIB: 			Time used: 457,7713 ms
	/// FFTWTestUsingDoubleFFTWLIBR2R_INPLACE: 	Time used: 2233,7726 ms
	/// FFTWTestUsingDoubleFFTWLIBR2R: 			Time used: 2542,5376 ms
	/// LomontFFTTestUsingDouble: 				Time used: 121,729 ms
	/// ExocortexFFTTestUsingComplex: 			Time used: 2400,0494 ms
	/// ExocortexFFTTestUsingComplexF: 			Time used: 1524,2402 ms
	/// ExocortexFFTTestUsingFloats: 			Time used: 1255,6019 ms
	/// </summary>
	public static class FFTTesting
	{
		public static void FFTWTestUsingDouble(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			// prepare the input arrays
			FFTW.DoubleArray fftwInput = new FFTW.DoubleArray(audio_data);
			
			int complexSize = (audio_data.Length >> 1) + 1;
			FFTW.ComplexArray fftwOutput = new FFTW.ComplexArray(complexSize);
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				FFTW.ForwardTransform(fftwInput, fftwOutput);
			}
			
			// get the result
			double[] spectrum_fft_real = fftwOutput.Real;
			double[] spectrum_fft_imag = fftwOutput.Imag;
			double[] spectrum_fft_abs = fftwOutput.Abs;
			
			// prepare the input arrays
			FFTW.ComplexArray fftwBackwardInput = new FFTW.ComplexArray(spectrum_fft_real, spectrum_fft_imag);
			FFTW.DoubleArray fftwBackwardOutput = new FFTW.DoubleArray(audio_data.Length);
			
			// perform the inverse FFT (IFFT)
			FFTW.BackwardTransform(fftwBackwardInput, fftwBackwardOutput);
			
			// get the result
			double[] spectrum_inverse_real = fftwBackwardOutput.ValuesDivedByN;
			
			// pad for output
			if (spectrum_fft_real.Length != audio_data.Length) Array.Resize(ref spectrum_fft_real, audio_data.Length);
			if (spectrum_fft_imag.Length != audio_data.Length) Array.Resize(ref spectrum_fft_imag, audio_data.Length);
			if (spectrum_fft_abs.Length != audio_data.Length) Array.Resize(ref spectrum_fft_abs, audio_data.Length);
			
			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real);
			}
		}
		
		public static void FFTWTestUsingDoubleFFTWLIB(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			// prepare the input arrays
			double[] complexSignal = FFTUtils.DoubleToComplexDouble(audio_data);
			fftw_complexarray complexInput = new fftw_complexarray(complexSignal);
			fftw_complexarray complexOutput = new fftw_complexarray(audio_data.Length);
			fftw_plan fft = fftw_plan.dft_1d(audio_data.Length, complexInput, complexOutput, fftw_direction.Forward, fftw_flags.Estimate);
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				fft.Execute();
			}

			// get the result
			double[] spectrum_fft_real = complexOutput.Real;
			double[] spectrum_fft_imag = complexOutput.Imag;
			double[] spectrum_fft_abs = complexOutput.Abs;
			
			// perform the inverse FFT (IFFT)
			fftw_complexarray complexOutput2 = new fftw_complexarray(audio_data.Length);
			fftw_plan ifft = fftw_plan.dft_1d(audio_data.Length, complexOutput, complexOutput2, fftw_direction.Backward, fftw_flags.Estimate);

			// perform the IFFT
			ifft.Execute();
			
			// get the result
			double[] spectrum_inverse_real = complexOutput2.RealDividedByN;
			double[] spectrum_inverse_imag = complexOutput2.ImagDividedByN;
			double[] spectrum_inverse_abs = complexOutput2.AbsDividedByN;

			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}
		
		public enum fftMethod : int {
			DFT = 0,
			IDFT = 1,
			DHT = 2
		}
		
		public static void FFTW_FFT_R2R(ref double[] @in, ref double[] @out, Int32 N, fftMethod method) {

			fftw_complexarray complexInput = new fftw_complexarray(@in);
			fftw_complexarray complexOutput = new fftw_complexarray(@out);
			
			fftw_kind kind = fftw_kind.R2HC;
			switch(method) {
				case fftMethod.DFT:
					kind = fftw_kind.R2HC;
					fftw_plan fft = fftw_plan.r2r_1d(N, complexInput, complexOutput, kind, fftw_flags.Estimate);
					fft.Execute();
					@out = complexOutput.Values;
					
					// free up memory
					fft = null;
					break;
				case fftMethod.IDFT:
					kind = fftw_kind.HC2R;
					fftw_plan ifft = fftw_plan.r2r_1d(N, complexInput, complexOutput, kind, fftw_flags.Estimate);
					ifft.Execute();
					@out = complexOutput.ValuesDividedByN;
					
					// free up memory
					ifft = null;
					break;
				case fftMethod.DHT:
					kind = fftw_kind.DHT;
					fftw_plan dht = fftw_plan.r2r_1d(N, complexInput, complexOutput, kind, fftw_flags.Estimate);
					dht.Execute();
					@out = complexOutput.Values;
					
					// free up memory
					dht = null;
					break;
			}

			// free up memory
			complexInput = null;
			complexOutput = null;
			GC.Collect();
		}

		public static void FFTWTestUsingDoubleFFTWLIBR2R_INPLACE(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			int N = audio_data.Length;

			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				FFTW_FFT_R2R(ref audio_data, ref audio_data, N, fftMethod.DFT);
			}
			
			// get the result
			double[] complexDout = FFTUtils.HC2C(audio_data);
			double[] spectrum_fft_real = FFTUtils.Real(complexDout);
			double[] spectrum_fft_imag = FFTUtils.Imag(complexDout);
			double[] spectrum_fft_abs = FFTUtils.Abs(complexDout);
			
			// pad for output
			if (spectrum_fft_real.Length != audio_data.Length) Array.Resize(ref spectrum_fft_real, audio_data.Length);
			if (spectrum_fft_imag.Length != audio_data.Length) Array.Resize(ref spectrum_fft_imag, audio_data.Length);
			if (spectrum_fft_abs.Length != audio_data.Length) Array.Resize(ref spectrum_fft_abs, audio_data.Length);
			
			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs);
			}
			
		}

		public static void FFTWTestUsingDoubleFFTWLIBR2R(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			int N = audio_data.Length;
			double[] din = audio_data;
			double[] dout = new double[N];
			double[] dout2 = new double[N];
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				FFTW_FFT_R2R(ref din, ref dout, N, fftMethod.DFT);
			}
			
			// get the result
			double[] complexDout = FFTUtils.HC2C(dout);
			double[] spectrum_fft_real = FFTUtils.Real(complexDout);
			double[] spectrum_fft_imag = FFTUtils.Imag(complexDout);
			double[] spectrum_fft_abs = FFTUtils.Abs(complexDout);
			
			// perform the IFFT
			FFTW_FFT_R2R(ref dout, ref dout2, N, fftMethod.IDFT);
			
			// get the result
			double[] spectrum_inverse_real = dout2;
			
			// pad for output
			if (spectrum_fft_real.Length != audio_data.Length) Array.Resize(ref spectrum_fft_real, audio_data.Length);
			if (spectrum_fft_imag.Length != audio_data.Length) Array.Resize(ref spectrum_fft_imag, audio_data.Length);
			if (spectrum_fft_abs.Length != audio_data.Length) Array.Resize(ref spectrum_fft_abs, audio_data.Length);
			
			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real);
			}
		}
		
		public static void NAaudioFFTTestUsingDouble(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			int binaryExponentitation = (int)Math.Log(audio_data.Length, 2);
			NAudio.Dsp.Complex[] complexArray = new NAudio.Dsp.Complex[audio_data.Length];
			for (int i = 0; i < audio_data.Length; i++) {
				complexArray[i].X = (float) audio_data[i];
				complexArray[i].Y = 0;
			}
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				NAudio.Dsp.FastFourierTransform.FFT(true, binaryExponentitation, complexArray);
			}
			
			// get the result
			double[] spectrum_fft_real = new double[complexArray.Length];
			double[] spectrum_fft_imag = new double[complexArray.Length];
			double[] spectrum_fft_abs = new double[complexArray.Length];
			
			for (int i = 0; i < complexArray.Length; i++) {
				float re  = complexArray[i].X;
				float img = complexArray[i].Y;
				spectrum_fft_real[i] = re;
				spectrum_fft_imag[i] = img;
				spectrum_fft_abs[i] = (float) Math.Sqrt(re*re + img*img);
			}

			// perform the inverse FFT (IFFT)
			NAudio.Dsp.FastFourierTransform.FFT(false, binaryExponentitation, complexArray);

			// get the result
			double[] spectrum_inverse_real = new double[complexArray.Length];
			double[] spectrum_inverse_imag = new double[complexArray.Length];
			double[] spectrum_inverse_abs = new double[complexArray.Length];

			for (int i = 0; i < complexArray.Length; i++) {
				float re  = complexArray[i].X;
				float img = complexArray[i].Y;
				spectrum_inverse_real[i] = re;
				spectrum_inverse_imag[i] = img;
				spectrum_inverse_abs[i] = (float) Math.Sqrt(re*re + img*img);
			}

			
			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}
		
		public static void LomontFFTTestUsingDouble(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			double[] spectrum_fft = null;

			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				spectrum_fft = FFTUtils.FFT(audio_data);
			}
			
			// get the result
			double lengthSqrt = Math.Sqrt(audio_data.Length);
			double[] spectrum_fft_real = FFTUtils.Real(spectrum_fft, lengthSqrt);
			double[] spectrum_fft_imag = FFTUtils.Imag(spectrum_fft, lengthSqrt);
			double[] spectrum_fft_abs = FFTUtils.Abs(spectrum_fft, lengthSqrt);

			// perform the inverse FFT (IFFT)
			double[] spectrum_inverse = FFTUtils.IFFT(spectrum_fft);

			// get the result
			double[] spectrum_inverse_real = FFTUtils.Real(spectrum_inverse);
			double[] spectrum_inverse_imag = FFTUtils.Imag(spectrum_inverse);
			double[] spectrum_inverse_abs = FFTUtils.Abs(spectrum_inverse);
			
			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}

		public static void ExocortexFFTTestUsingFloats(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			float[] audio_data_float = MathUtils.DoubleToFloat(audio_data);
			
			// build the complex array
			int N = audio_data_float.Length * 2;
			float[] complexSignal = FFTUtils.FloatToComplexFloat(audio_data_float);

			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				Fourier.FFT(complexSignal, N/2, FourierDirection.Forward);
			}
			
			// get the result
			float[] spectrum_fft_real = FFTUtils.Real(complexSignal);
			float[] spectrum_fft_imag = FFTUtils.Imag(complexSignal);
			float[] spectrum_fft_abs = FFTUtils.Abs(complexSignal);

			// perform the inverse FFT (IFFT)
			Fourier.FFT(complexSignal, N/2, FourierDirection.Backward);
			
			// get the result
			float[] spectrum_inverse_real = FFTUtils.Real(complexSignal, (float) 2/N);
			float[] spectrum_inverse_imag = FFTUtils.Imag(complexSignal, (float) 2/N);
			float[] spectrum_inverse_abs = FFTUtils.Abs(complexSignal, (float) 2/N);

			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}

		public static void ExocortexFFTTestUsingComplexF(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			float[] audio_data_float = MathUtils.DoubleToFloat(audio_data);
			
			// build the complex array
			int N = audio_data_float.Length;
			ComplexF[] complexSignal = FFTUtils.FloatToComplex(audio_data_float);
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				Fourier.FFT(complexSignal, N, FourierDirection.Forward);
			}

			// get the result
			float[] spectrum_fft_real = new float[N];
			float[] spectrum_fft_imag = new float[N];
			float[] spectrum_fft_abs = new float[N];
			for (int j = 0; j < N; j++)
			{
				spectrum_fft_real[j] = complexSignal[j].Re;
				spectrum_fft_imag[j] = complexSignal[j].Im;
				spectrum_fft_abs[j] = complexSignal[j].GetModulus();
			}
			
			// perform the inverse FFT (IFFT)
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
			
			// get the result
			float[] spectrum_inverse_real = new float[N];
			float[] spectrum_inverse_imag = new float[N];
			float[] spectrum_inverse_abs = new float[N];
			for (int j = 0; j < N; j++)
			{
				spectrum_inverse_real[j] = complexSignal[j].Re * 1/N;
				spectrum_inverse_imag[j] = complexSignal[j].Im * 1/N;
				spectrum_inverse_abs[j] = complexSignal[j].GetModulus() * 1/N;
			}

			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}
		
		public static void ExocortexFFTTestUsingComplex(string CSVFilePath=null, double[] audio_data=null, int testLoopCount=1) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			// build the complex array
			int N = audio_data.Length;
			Complex[] complexSignal = FFTUtils.DoubleToComplex(audio_data);
			
			// loop if neccesary - e.g. for performance test purposes
			for (int i = 0; i < testLoopCount; i++) {
				// perform the FFT
				Fourier.FFT(complexSignal, N, FourierDirection.Forward);
			}

			// get the result
			double[] spectrum_fft_real = new double[N];
			double[] spectrum_fft_imag = new double[N];
			double[] spectrum_fft_abs = new double[N];
			for (int j = 0; j < N; j++)
			{
				spectrum_fft_real[j] = complexSignal[j].Re;
				spectrum_fft_imag[j] = complexSignal[j].Im;
				spectrum_fft_abs[j] = complexSignal[j].GetModulus();
			}
			
			// perform the inverse FFT (IFFT)
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
			
			// get the result
			double[] spectrum_inverse_real = new double[N];
			double[] spectrum_inverse_imag = new double[N];
			double[] spectrum_inverse_abs = new double[N];
			for (int j = 0; j < N; j++)
			{
				spectrum_inverse_real[j] = complexSignal[j].Re * 1/N;
				spectrum_inverse_imag[j] = complexSignal[j].Im * 1/N;
				spectrum_inverse_abs[j] = complexSignal[j].GetModulus() * 1/N;
			}

			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}
		
		public static void OctaveFFTWOuput(string CSVFilePath) {
			
			double[] audio_data = GetSignalTestData();
			double[] spectrum_fft_real = MatLabSpectrumFFTReal();
			double[] spectrum_fft_imag = MatLabSpectrumFFTImag();
			double[] spectrum_fft_abs = MatLabSpectrumFFTAbs();
			double[] spectrum_inverse_real = MatLabSpectrumIFFTReal();
			double[] spectrum_inverse_imag = MatLabSpectrumIFFTImag();
			double[] spectrum_inverse_abs = MatLabSpectrumIFFTAbs();

			if (CSVFilePath!=null) {
				CommonUtils.Export.exportCSV(CSVFilePath, audio_data, spectrum_fft_real, spectrum_fft_imag, spectrum_fft_abs, spectrum_inverse_real, spectrum_inverse_imag, spectrum_inverse_abs);
			}
		}
		
		public static void TimeAll(int testLoopCount) {
			
			// Start the stopwatch
			Stopwatch sw = Stopwatch.StartNew();
			
			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDouble(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIB(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIB: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R_INPLACE(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIBR2R_INPLACE: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIBR2R: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("LomontFFTTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
			
			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingComplex: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplexF(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingComplexF: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingFloats(null, null, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingFloats: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
		}
		
		public static void TimeAll(double[] audio_data, int testLoopCount=1) {
			
			// Start the stopwatch
			Stopwatch sw = Stopwatch.StartNew();
			
			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDouble(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIB(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIB: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R_INPLACE(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIBR2R_INPLACE: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("FFTWTestUsingDoubleFFTWLIBR2R: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("LomontFFTTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
			
			sw.Restart();
			CommonUtils.FFT.FFTTesting.NAaudioFFTTestUsingDouble(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("NAaudioFFTTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
			
			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingComplex: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplexF(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingComplexF: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);

			sw.Restart();
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingFloats(null, audio_data, testLoopCount);
			sw.Stop();
			Console.Out.WriteLine("ExocortexFFTTestUsingFloats: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
		}
		
		public static void TestAll() {
			CommonUtils.FFT.FFTTesting.OctaveFFTWOuput("OctaveFFTWOuput.csv");
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDouble("FFTWTestUsingDouble.csv");
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIB("FFTWTestUsingDoubleFFTWLIB.csv");
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R_INPLACE("FFTWTestUsingDoubleFFTWLIBR2R_INPLACE.csv");
			CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R("FFTWTestUsingDoubleFFTWLIBR2R.csv");
			CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble("LomontFFTTestUsingDouble.csv");
			CommonUtils.FFT.FFTTesting.NAaudioFFTTestUsingDouble("NAaudioFFTTestUsingDouble.csv");
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex("ExocortexFFTTestUsingComplex.csv");
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplexF("ExocortexFFTTestUsingComplexF.csv");
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingFloats("ExocortexFFTTestUsingFloats.csv");
		}
		
		#region test data
		private static double[] GetSignalTestData() {
			return new double[] {
				0.000865391,
				0.104118064,
				-0.105368771,
				-0.096618481,
				0.274461865,
				0.100624651,
				-0.031682827,
				0.178861931,
				-0.041154884,
				0.0745764,
				-0.056584343,
				-0.011519475,
				0.37244159,
				-0.333296567,
				-0.07670486,
				0.012635638,
				0.06430091,
				0.061967451,
				0.035306752,
				-0.011976154,
				0.013347088,
				-0.015255818,
				-0.032171197,
				-0.421613872,
				0.176026717,
				-0.035100222,
				-0.04386339,
				0.008789533,
				-0.046787836,
				0.010265357,
				0.035256006,
				0.009122181,
				-0.022775872,
				-0.043074373,
				0.002139942,
				0.002891588,
				-0.04839379,
				0.004932872,
				0.009055731,
				0.120602608,
				0.350419849,
				-0.049741529,
				0.018746957,
				-0.020023627,
				-0.018909276,
				0.05838256,
				0.003512445,
				0.032333404,
				-0.004187524,
				0.053402066,
				-0.193661004,
				-0.030811183,
				-0.010904786,
				-0.002647284,
				-0.019114079,
				0.148751348,
				-0.258452237,
				0.19318524,
				-0.024676971,
				-0.08230494,
				0.153271362,
				-0.013393482,
				0.225574851,
				0.066862829,
				-0.005007621,
				0.054133821,
				0.304719567,
				0.035531294,
				-0.074139223,
				0.01723122,
				0.054142788,
				-0.05307924,
				0.027416158,
				0.050989781,
				0.045185816,
				-0.02669557,
				0.020914664,
				0.020171048,
				-0.032324258,
				-0.038908169,
				0.002208921,
				-0.008629396,
				-0.04518985,
				-0.047292139,
				0.029560423,
				-0.011835856,
				-0.003346157,
				-0.174357966,
				0.080544405,
				-0.167937592,
				0.296601146,
				-0.019905306,
				-0.011410435,
				0.008175705,
				0.006888151,
				0.021011366,
				-0.068038754,
				-0.04927912,
				-0.078994833,
				0.031503096,
				0.08944536,
				0.110646628,
				0.144155294,
				0.035049524,
				-0.109210745,
				0.026256427,
				-0.019852556,
				-0.062412113,
				-0.06595698,
				0.27951315,
				0.081969298,
				0.016485604,
				0.055248857,
				-0.021263424,
				0.020316027,
				-0.068477564,
				0.009670784,
				-0.014175341,
				-0.012252999,
				0.013099598,
				0.019670077,
				-0.008961075,
				-0.012455529,
				-0.010228375,
				0.012758554,
				0.008688235,
				0.130880281,
				0.004299275,
				0.002608773,
				-0.038659882,
				-0.035320021,
				0.03946986,
				0.045862138,
				0.025777206,
				0.015493953,
				0.361725807,
				-0.041336037,
				0.065300807,
				-0.006711667,
				0.048016701,
				-0.412198603,
				-0.522793591,
				-0.305609763,
				-0.041047633,
				-0.146241426,
				-0.105980113,
				-0.022788802,
				0.006023734,
				0.038044345,
				0.022009104,
				-0.025985572,
				0.025816889,
				-0.005321069,
				0.045612838,
				0.022013335,
				0.018998574,
				-0.030563755,
				0.014832703,
				-0.014628937,
				-0.009070123,
				-0.02606459,
				-0.013203338,
				-0.359192133,
				0.112673149,
				-0.174063757,
				0.024307109,
				0.048371878,
				-0.109712929,
				-0.028758414,
				0.007553618,
				0.030553628,
				0.010247151,
				0.034802798,
				0.001573765,
				0.009841984,
				-0.009444373,
				-0.030185122,
				-0.009690596,
				-0.310107172,
				0.059100546,
				0.086071484,
				0.014892465,
				-0.045540437,
				-0.449009806,
				-0.018961258,
				-0.081764825,
				0.17429018,
				-0.006051018,
				-0.103804767,
				-0.288634688,
				0.114703834,
				-0.138847873,
				0.09735018,
				0.382123768,
				-0.047125801,
				0.081403479,
				-0.083455987,
				0.074241497,
				0.027118556,
				-0.074722409,
				-0.045518398,
				-0.040908173,
				0.012777518,
				0.038880546,
				0.049085863,
				0.032236215,
				-0.059107684,
				-0.02500126,
				0.00640589,
				0.039520133,
				0.066216588,
				0.03575873,
				0.028480541,
				-0.014959369,
				-0.00399899,
				-0.035790734,
				-0.03992743,
				-0.048084497,
				0.002207351,
				0.030950662,
				-0.320640296,
				0.034640063,
				-0.006563974,
				-0.029881194,
				0.022486171,
				-0.011634802,
				-0.273039907,
				0.48763299,
				0.111061722,
				0.05188515,
				0.095795095,
				0.078814439,
				0.105761215,
				-0.140504986,
				-0.085491545,
				-0.087526135,
				-0.008067675,
				0.033521228,
				0.006883552,
				0.064519316,
				0.003384398,
				-0.014617815,
				0.032730252,
				0.001646112,
				-0.00196098,
				0.012564101,
				-0.304068267,
				-0.267732918,
				-0.209717855,
				-0.06585405,
				-0.00726669,
				-0.037195243,
				-0.074893326,
				-0.012096515,
				0.046475943,
				0.0681374
			};
		}
		
		private static double[] MatLabSpectrumFFTReal() {
			return new double[] {
				-1.037635295,
				0.663001566,
				-0.943641744,
				-1.150228497,
				0.573954623,
				0.477188832,
				-0.290983263,
				-0.722503224,
				2.590934575,
				-0.68816601,
				1.104263502,
				-1.368103563,
				3.312844567,
				-0.029501301,
				-1.414415542,
				0.710597891,
				0.216652982,
				3.178517502,
				-0.111243457,
				2.028417279,
				-0.183472837,
				0.9767673,
				-3.675914864,
				-0.584633149,
				-1.687670571,
				-0.070369689,
				0.152210834,
				-1.448531464,
				2.861091496,
				-1.364679395,
				0.648508986,
				-1.522203458,
				-0.37852472,
				0.433638672,
				-0.447702047,
				-2.068199429,
				-1.593621491,
				1.286454253,
				-0.113429886,
				1.170382521,
				1.905812296,
				0.623934809,
				-1.104826433,
				1.356711948,
				1.806626882,
				1.920998518,
				-0.200748694,
				0.497816037,
				1.428463452,
				-1.486440036,
				-0.135839048,
				1.212968753,
				0.193301999,
				0.030912144,
				1.422296329,
				-1.169228818,
				-3.031004769,
				-0.579606552,
				-0.987550262,
				0.156058096,
				-1.628738648,
				2.402583186,
				0.970476075,
				2.195689381,
				0.377834072,
				1.212648749,
				1.387606765,
				-1.138481846,
				0.322778053,
				-0.877736207,
				2.545805751,
				-0.145795169,
				0.165831193,
				0.773869112,
				1.387673442,
				-0.840106016,
				-1.974847129,
				-0.142911815,
				-1.456988782,
				-1.873160425,
				0.114243492,
				0.135144925,
				0.489987092,
				-0.189761683,
				-0.552208214,
				1.956527825,
				-0.463739507,
				-1.569297787,
				0.622734732,
				1.929650341,
				1.073491728,
				-1.163413956,
				-3.276841515,
				-0.110349001,
				0.078593928,
				-2.004412983,
				0.439943854,
				1.632113055,
				-2.003503258,
				-1.284657959,
				-2.577491631,
				-2.923918498,
				-0.948306774,
				-0.050385754,
				0.621746654,
				1.597530309,
				0.438427793,
				1.296426098,
				2.132806845,
				-1.150386194,
				1.987140429,
				-1.253178014,
				-1.795845974,
				0.372650135,
				0.572253676,
				-1.147380718,
				2.995526815,
				1.586379126,
				-1.344750241,
				-1.785534414,
				-0.784473918,
				-0.226827682,
				-0.644287549,
				-2.116126275,
				0.631646472,
				1.647531815,
				-0.814653619,
				0.671530358,
				-0.530161709,
				0.671530358,
				-0.814653619,
				1.647531815,
				0.631646472,
				-2.116126275,
				-0.644287549,
				-0.226827682,
				-0.784473918,
				-1.785534414,
				-1.344750241,
				1.586379126,
				2.995526815,
				-1.147380718,
				0.572253676,
				0.372650135,
				-1.795845974,
				-1.253178014,
				1.987140429,
				-1.150386194,
				2.132806845,
				1.296426098,
				0.438427793,
				1.597530309,
				0.621746654,
				-0.050385754,
				-0.948306774,
				-2.923918498,
				-2.577491631,
				-1.284657959,
				-2.003503258,
				1.632113055,
				0.439943854,
				-2.004412983,
				0.078593928,
				-0.110349001,
				-3.276841515,
				-1.163413956,
				1.073491728,
				1.929650341,
				0.622734732,
				-1.569297787,
				-0.463739507,
				1.956527825,
				-0.552208214,
				-0.189761683,
				0.489987092,
				0.135144925,
				0.114243492,
				-1.873160425,
				-1.456988782,
				-0.142911815,
				-1.974847129,
				-0.840106016,
				1.387673442,
				0.773869112,
				0.165831193,
				-0.145795169,
				2.545805751,
				-0.877736207,
				0.322778053,
				-1.138481846,
				1.387606765,
				1.212648749,
				0.377834072,
				2.195689381,
				0.970476075,
				2.402583186,
				-1.628738648,
				0.156058096,
				-0.987550262,
				-0.579606552,
				-3.031004769,
				-1.169228818,
				1.422296329,
				0.030912144,
				0.193301999,
				1.212968753,
				-0.135839048,
				-1.486440036,
				1.428463452,
				0.497816037,
				-0.200748694,
				1.920998518,
				1.806626882,
				1.356711948,
				-1.104826433,
				0.623934809,
				1.905812296,
				1.170382521,
				-0.113429886,
				1.286454253,
				-1.593621491,
				-2.068199429,
				-0.447702047,
				0.433638672,
				-0.37852472,
				-1.522203458,
				0.648508986,
				-1.364679395,
				2.861091496,
				-1.448531464,
				0.152210834,
				-0.070369689,
				-1.687670571,
				-0.584633149,
				-3.675914864,
				0.9767673,
				-0.183472837,
				2.028417279,
				-0.111243457,
				3.178517502,
				0.216652982,
				0.710597891,
				-1.414415542,
				-0.029501301,
				3.312844567,
				-1.368103563,
				1.104263502,
				-0.68816601,
				2.590934575,
				-0.722503224,
				-0.290983263,
				0.477188832,
				0.573954623,
				-1.150228497,
				-0.943641744,
				0.663001566
			};
		}

		private static double[] MatLabSpectrumFFTImag() {
			return new double[] {
				0,
				-2.340346849,
				1.50382509,
				-0.863972665,
				0.500156922,
				-2.167777055,
				0.213041858,
				-1.794589871,
				-3.166906953,
				-2.492154999,
				-0.749835583,
				1.153675785,
				-1.593833747,
				2.377954214,
				-2.722847318,
				-0.334084905,
				-1.389115371,
				0.272441769,
				0.317891809,
				0.591308583,
				1.159951414,
				-0.260299692,
				2.99503739,
				-0.596956066,
				2.058289347,
				0.125051062,
				1.046363091,
				-0.202646095,
				-0.42715192,
				-1.069239671,
				1.944540073,
				-0.098007638,
				-0.329592985,
				-0.910146939,
				-0.530791495,
				-0.107197299,
				-1.939978523,
				1.763667424,
				1.052625453,
				-0.802871262,
				-0.184606504,
				1.456227874,
				1.957032606,
				-1.635555843,
				0.41994519,
				2.080746245,
				1.151624811,
				-1.451582413,
				2.717807272,
				1.241330316,
				-0.262851378,
				0.060439941,
				-1.740019431,
				-0.025505989,
				-1.171621014,
				1.044341047,
				1.016113599,
				1.700969322,
				0.950287826,
				-0.35641763,
				-0.825302619,
				-0.69465967,
				2.492574527,
				-0.71901184,
				-0.125485047,
				-0.024656396,
				0.206029101,
				-1.930770763,
				-0.584989636,
				-0.956606337,
				0.196204317,
				-1.898023133,
				-0.616971212,
				-0.489913287,
				-0.902007521,
				0.777618188,
				-1.329011542,
				-0.26211419,
				-1.482022999,
				-0.106913374,
				-1.119378177,
				1.058310981,
				1.057773778,
				1.566127161,
				1.156760556,
				-1.759302775,
				-2.38440835,
				-1.784040548,
				-1.987496839,
				0.328267293,
				-0.887215788,
				0.12968757,
				-0.587185399,
				0.270832171,
				0.371499688,
				-1.505469095,
				-1.823404115,
				0.905119541,
				1.912116645,
				-0.606734127,
				0.680783619,
				1.971930283,
				2.638899591,
				0.29136593,
				-0.670463478,
				0.768457763,
				0.632215132,
				1.524727113,
				-0.188490388,
				-2.107589097,
				1.011610198,
				0.485342407,
				-2.09411944,
				-1.405904323,
				0.810038508,
				0.317242631,
				-1.016730163,
				-0.867868902,
				-0.004728449,
				2.808046707,
				-1.935795102,
				0.424015814,
				-0.535941128,
				-0.59817949,
				1.185584158,
				2.826705701,
				0.117688788,
				1.830143331,
				0,
				-1.830143331,
				-0.117688788,
				-2.826705701,
				-1.185584158,
				0.59817949,
				0.535941128,
				-0.424015814,
				1.935795102,
				-2.808046707,
				0.004728449,
				0.867868902,
				1.016730163,
				-0.317242631,
				-0.810038508,
				1.405904323,
				2.09411944,
				-0.485342407,
				-1.011610198,
				2.107589097,
				0.188490388,
				-1.524727113,
				-0.632215132,
				-0.768457763,
				0.670463478,
				-0.29136593,
				-2.638899591,
				-1.971930283,
				-0.680783619,
				0.606734127,
				-1.912116645,
				-0.905119541,
				1.823404115,
				1.505469095,
				-0.371499688,
				-0.270832171,
				0.587185399,
				-0.12968757,
				0.887215788,
				-0.328267293,
				1.987496839,
				1.784040548,
				2.38440835,
				1.759302775,
				-1.156760556,
				-1.566127161,
				-1.057773778,
				-1.058310981,
				1.119378177,
				0.106913374,
				1.482022999,
				0.26211419,
				1.329011542,
				-0.777618188,
				0.902007521,
				0.489913287,
				0.616971212,
				1.898023133,
				-0.196204317,
				0.956606337,
				0.584989636,
				1.930770763,
				-0.206029101,
				0.024656396,
				0.125485047,
				0.71901184,
				-2.492574527,
				0.69465967,
				0.825302619,
				0.35641763,
				-0.950287826,
				-1.700969322,
				-1.016113599,
				-1.044341047,
				1.171621014,
				0.025505989,
				1.740019431,
				-0.060439941,
				0.262851378,
				-1.241330316,
				-2.717807272,
				1.451582413,
				-1.151624811,
				-2.080746245,
				-0.41994519,
				1.635555843,
				-1.957032606,
				-1.456227874,
				0.184606504,
				0.802871262,
				-1.052625453,
				-1.763667424,
				1.939978523,
				0.107197299,
				0.530791495,
				0.910146939,
				0.329592985,
				0.098007638,
				-1.944540073,
				1.069239671,
				0.42715192,
				0.202646095,
				-1.046363091,
				-0.125051062,
				-2.058289347,
				0.596956066,
				-2.99503739,
				0.260299692,
				-1.159951414,
				-0.591308583,
				-0.317891809,
				-0.272441769,
				1.389115371,
				0.334084905,
				2.722847318,
				-2.377954214,
				1.593833747,
				-1.153675785,
				0.749835583,
				2.492154999,
				3.166906953,
				1.794589871,
				-0.213041858,
				2.167777055,
				-0.500156922,
				0.863972665,
				-1.50382509,
				2.340346849,
			};
		}
		
		private static double[] MatLabSpectrumFFTAbs() {
			return new double[] {
				1.037635295,
				2.432446186,
				1.7753731,
				1.438566774,
				0.761302079,
				2.219677126,
				0.360635678,
				1.934570679,
				4.091728439,
				2.585422402,
				1.334785107,
				1.789601961,
				3.676308629,
				2.378137206,
				3.068300644,
				0.785214675,
				1.405908969,
				3.1901721,
				0.336794164,
				2.112847012,
				1.174371987,
				1.010856215,
				4.741581915,
				0.835555183,
				2.661726318,
				0.143490979,
				1.05737593,
				1.462637632,
				2.892801982,
				1.733673362,
				2.049829261,
				1.525355324,
				0.501908856,
				1.008171587,
				0.694389468,
				2.070975649,
				2.510606724,
				2.182999617,
				1.05871936,
				1.419294723,
				1.914732376,
				1.584264582,
				2.247358019,
				2.125020053,
				1.854792347,
				2.83191459,
				1.168990908,
				1.534572353,
				3.070339428,
				1.936596224,
				0.295876823,
				1.214473624,
				1.750723646,
				0.040076379,
				1.842721533,
				1.567719443,
				3.196791635,
				1.797008734,
				1.370511756,
				0.389085667,
				1.825900872,
				2.500991408,
				2.67483674,
				2.310417686,
				0.398126968,
				1.212899388,
				1.402818778,
				2.241431831,
				0.668130635,
				1.298274444,
				2.553355255,
				1.903614469,
				0.638868892,
				0.915908527,
				1.65506953,
				1.144756814,
				2.380397627,
				0.298542519,
				2.078270551,
				1.876209063,
				1.125192907,
				1.066905002,
				1.165749851,
				1.577581624,
				1.281806887,
				2.631187448,
				2.429085735,
				2.376025299,
				2.082772727,
				1.957373203,
				1.392672375,
				1.170619878,
				3.329035447,
				0.292449939,
				0.379722298,
				2.506812439,
				1.875727368,
				1.866288939,
				2.769515367,
				1.420729522,
				2.665882488,
				3.526727722,
				2.804117828,
				0.295690428,
				0.914379668,
				1.772746576,
				0.76936006,
				2.001377875,
				2.141119722,
				2.401108119,
				2.229816647,
				1.343879603,
				2.758695161,
				1.454453536,
				0.99178458,
				1.190430762,
				3.163371797,
				1.808257493,
				1.344758554,
				3.327650741,
				2.088708214,
				0.48087442,
				0.838056883,
				2.199047319,
				1.343349196,
				3.271792506,
				0.823110666,
				1.949455727,
				0.530161709,
				1.949455727,
				0.823110666,
				3.271792506,
				1.343349196,
				2.199047319,
				0.838056883,
				0.48087442,
				2.088708214,
				3.327650741,
				1.344758554,
				1.808257493,
				3.163371797,
				1.190430762,
				0.99178458,
				1.454453536,
				2.758695161,
				1.343879603,
				2.229816647,
				2.401108119,
				2.141119722,
				2.001377875,
				0.76936006,
				1.772746576,
				0.914379668,
				0.295690428,
				2.804117828,
				3.526727722,
				2.665882488,
				1.420729522,
				2.769515367,
				1.866288939,
				1.875727368,
				2.506812439,
				0.379722298,
				0.292449939,
				3.329035447,
				1.170619878,
				1.392672375,
				1.957373203,
				2.082772727,
				2.376025299,
				2.429085735,
				2.631187448,
				1.281806887,
				1.577581624,
				1.165749851,
				1.066905002,
				1.125192907,
				1.876209063,
				2.078270551,
				0.298542519,
				2.380397627,
				1.144756814,
				1.65506953,
				0.915908527,
				0.638868892,
				1.903614469,
				2.553355255,
				1.298274444,
				0.668130635,
				2.241431831,
				1.402818778,
				1.212899388,
				0.398126968,
				2.310417686,
				2.67483674,
				2.500991408,
				1.825900872,
				0.389085667,
				1.370511756,
				1.797008734,
				3.196791635,
				1.567719443,
				1.842721533,
				0.040076379,
				1.750723646,
				1.214473624,
				0.295876823,
				1.936596224,
				3.070339428,
				1.534572353,
				1.168990908,
				2.83191459,
				1.854792347,
				2.125020053,
				2.247358019,
				1.584264582,
				1.914732376,
				1.419294723,
				1.05871936,
				2.182999617,
				2.510606724,
				2.070975649,
				0.694389468,
				1.008171587,
				0.501908856,
				1.525355324,
				2.049829261,
				1.733673362,
				2.892801982,
				1.462637632,
				1.05737593,
				0.143490979,
				2.661726318,
				0.835555183,
				4.741581915,
				1.010856215,
				1.174371987,
				2.112847012,
				0.336794164,
				3.1901721,
				1.405908969,
				0.785214675,
				3.068300644,
				2.378137206,
				3.676308629,
				1.789601961,
				1.334785107,
				2.585422402,
				4.091728439,
				1.934570679,
				0.360635678,
				2.219677126,
				0.761302079,
				1.438566774,
				1.7753731,
				2.432446186,
			};
		}

		private static double[] MatLabSpectrumIFFTReal() {
			return new double[] {
				0.000865391,
				0.104118064,
				-0.105368771,
				-0.096618481,
				0.274461865,
				0.100624651,
				-0.031682827,
				0.178861931,
				-0.041154884,
				0.0745764,
				-0.056584343,
				-0.011519475,
				0.37244159,
				-0.333296567,
				-0.07670486,
				0.012635638,
				0.06430091,
				0.061967451,
				0.035306752,
				-0.011976154,
				0.013347088,
				-0.015255818,
				-0.032171197,
				-0.421613872,
				0.176026717,
				-0.035100222,
				-0.04386339,
				0.008789533,
				-0.046787836,
				0.010265357,
				0.035256006,
				0.009122181,
				-0.022775872,
				-0.043074373,
				0.002139942,
				0.002891588,
				-0.04839379,
				0.004932872,
				0.009055731,
				0.120602608,
				0.350419849,
				-0.049741529,
				0.018746957,
				-0.020023627,
				-0.018909276,
				0.05838256,
				0.003512445,
				0.032333404,
				-0.004187524,
				0.053402066,
				-0.193661004,
				-0.030811183,
				-0.010904786,
				-0.002647284,
				-0.019114079,
				0.148751348,
				-0.258452237,
				0.19318524,
				-0.024676971,
				-0.08230494,
				0.153271362,
				-0.013393482,
				0.225574851,
				0.066862829,
				-0.005007621,
				0.054133821,
				0.304719567,
				0.035531294,
				-0.074139223,
				0.01723122,
				0.054142788,
				-0.05307924,
				0.027416158,
				0.050989781,
				0.045185816,
				-0.02669557,
				0.020914664,
				0.020171048,
				-0.032324258,
				-0.038908169,
				0.002208921,
				-0.008629396,
				-0.04518985,
				-0.047292139,
				0.029560423,
				-0.011835856,
				-0.003346157,
				-0.174357966,
				0.080544405,
				-0.167937592,
				0.296601146,
				-0.019905306,
				-0.011410435,
				0.008175705,
				0.006888151,
				0.021011366,
				-0.068038754,
				-0.04927912,
				-0.078994833,
				0.031503096,
				0.08944536,
				0.110646628,
				0.144155294,
				0.035049524,
				-0.109210745,
				0.026256427,
				-0.019852556,
				-0.062412113,
				-0.06595698,
				0.27951315,
				0.081969298,
				0.016485604,
				0.055248857,
				-0.021263424,
				0.020316027,
				-0.068477564,
				0.009670784,
				-0.014175341,
				-0.012252999,
				0.013099598,
				0.019670077,
				-0.008961075,
				-0.012455529,
				-0.010228375,
				0.012758554,
				0.008688235,
				0.130880281,
				0.004299275,
				0.002608773,
				-0.038659882,
				-0.035320021,
				0.03946986,
				0.045862138,
				0.025777206,
				0.015493953,
				0.361725807,
				-0.041336037,
				0.065300807,
				-0.006711667,
				0.048016701,
				-0.412198603,
				-0.522793591,
				-0.305609763,
				-0.041047633,
				-0.146241426,
				-0.105980113,
				-0.022788802,
				0.006023734,
				0.038044345,
				0.022009104,
				-0.025985572,
				0.025816889,
				-0.005321069,
				0.045612838,
				0.022013335,
				0.018998574,
				-0.030563755,
				0.014832703,
				-0.014628937,
				-0.009070123,
				-0.02606459,
				-0.013203338,
				-0.359192133,
				0.112673149,
				-0.174063757,
				0.024307109,
				0.048371878,
				-0.109712929,
				-0.028758414,
				0.007553618,
				0.030553628,
				0.010247151,
				0.034802798,
				0.001573765,
				0.009841984,
				-0.009444373,
				-0.030185122,
				-0.009690596,
				-0.310107172,
				0.059100546,
				0.086071484,
				0.014892465,
				-0.045540437,
				-0.449009806,
				-0.018961258,
				-0.081764825,
				0.17429018,
				-0.006051018,
				-0.103804767,
				-0.288634688,
				0.114703834,
				-0.138847873,
				0.09735018,
				0.382123768,
				-0.047125801,
				0.081403479,
				-0.083455987,
				0.074241497,
				0.027118556,
				-0.074722409,
				-0.045518398,
				-0.040908173,
				0.012777518,
				0.038880546,
				0.049085863,
				0.032236215,
				-0.059107684,
				-0.02500126,
				0.00640589,
				0.039520133,
				0.066216588,
				0.03575873,
				0.028480541,
				-0.014959369,
				-0.00399899,
				-0.035790734,
				-0.03992743,
				-0.048084497,
				0.002207351,
				0.030950662,
				-0.320640296,
				0.034640063,
				-0.006563974,
				-0.029881194,
				0.022486171,
				-0.011634802,
				-0.273039907,
				0.48763299,
				0.111061722,
				0.05188515,
				0.095795095,
				0.078814439,
				0.105761215,
				-0.140504986,
				-0.085491545,
				-0.087526135,
				-0.008067675,
				0.033521228,
				0.006883552,
				0.064519316,
				0.003384398,
				-0.014617815,
				0.032730252,
				0.001646112,
				-0.00196098,
				0.012564101,
				-0.304068267,
				-0.267732918,
				-0.209717855,
				-0.06585405,
				-0.00726669,
				-0.037195243,
				-0.074893326,
				-0.012096515,
				0.046475943,
				0.0681374
			};
		}
		
		private static double[] MatLabSpectrumIFFTImag() {
			return new double[256];
		}
		
		private static double[] MatLabSpectrumIFFTAbs() {
			return new double[] {
				0.000865391,
				0.104118064,
				0.105368771,
				0.096618481,
				0.274461865,
				0.100624651,
				0.031682827,
				0.178861931,
				0.041154884,
				0.0745764,
				0.056584343,
				0.011519475,
				0.37244159,
				0.333296567,
				0.07670486,
				0.012635638,
				0.06430091,
				0.061967451,
				0.035306752,
				0.011976154,
				0.013347088,
				0.015255818,
				0.032171197,
				0.421613872,
				0.176026717,
				0.035100222,
				0.04386339,
				0.008789533,
				0.046787836,
				0.010265357,
				0.035256006,
				0.009122181,
				0.022775872,
				0.043074373,
				0.002139942,
				0.002891588,
				0.04839379,
				0.004932872,
				0.009055731,
				0.120602608,
				0.350419849,
				0.049741529,
				0.018746957,
				0.020023627,
				0.018909276,
				0.05838256,
				0.003512445,
				0.032333404,
				0.004187524,
				0.053402066,
				0.193661004,
				0.030811183,
				0.010904786,
				0.002647284,
				0.019114079,
				0.148751348,
				0.258452237,
				0.19318524,
				0.024676971,
				0.08230494,
				0.153271362,
				0.013393482,
				0.225574851,
				0.066862829,
				0.005007621,
				0.054133821,
				0.304719567,
				0.035531294,
				0.074139223,
				0.01723122,
				0.054142788,
				0.05307924,
				0.027416158,
				0.050989781,
				0.045185816,
				0.02669557,
				0.020914664,
				0.020171048,
				0.032324258,
				0.038908169,
				0.002208921,
				0.008629396,
				0.04518985,
				0.047292139,
				0.029560423,
				0.011835856,
				0.003346157,
				0.174357966,
				0.080544405,
				0.167937592,
				0.296601146,
				0.019905306,
				0.011410435,
				0.008175705,
				0.006888151,
				0.021011366,
				0.068038754,
				0.04927912,
				0.078994833,
				0.031503096,
				0.08944536,
				0.110646628,
				0.144155294,
				0.035049524,
				0.109210745,
				0.026256427,
				0.019852556,
				0.062412113,
				0.06595698,
				0.27951315,
				0.081969298,
				0.016485604,
				0.055248857,
				0.021263424,
				0.020316027,
				0.068477564,
				0.009670784,
				0.014175341,
				0.012252999,
				0.013099598,
				0.019670077,
				0.008961075,
				0.012455529,
				0.010228375,
				0.012758554,
				0.008688235,
				0.130880281,
				0.004299275,
				0.002608773,
				0.038659882,
				0.035320021,
				0.03946986,
				0.045862138,
				0.025777206,
				0.015493953,
				0.361725807,
				0.041336037,
				0.065300807,
				0.006711667,
				0.048016701,
				0.412198603,
				0.522793591,
				0.305609763,
				0.041047633,
				0.146241426,
				0.105980113,
				0.022788802,
				0.006023734,
				0.038044345,
				0.022009104,
				0.025985572,
				0.025816889,
				0.005321069,
				0.045612838,
				0.022013335,
				0.018998574,
				0.030563755,
				0.014832703,
				0.014628937,
				0.009070123,
				0.02606459,
				0.013203338,
				0.359192133,
				0.112673149,
				0.174063757,
				0.024307109,
				0.048371878,
				0.109712929,
				0.028758414,
				0.007553618,
				0.030553628,
				0.010247151,
				0.034802798,
				0.001573765,
				0.009841984,
				0.009444373,
				0.030185122,
				0.009690596,
				0.310107172,
				0.059100546,
				0.086071484,
				0.014892465,
				0.045540437,
				0.449009806,
				0.018961258,
				0.081764825,
				0.17429018,
				0.006051018,
				0.103804767,
				0.288634688,
				0.114703834,
				0.138847873,
				0.09735018,
				0.382123768,
				0.047125801,
				0.081403479,
				0.083455987,
				0.074241497,
				0.027118556,
				0.074722409,
				0.045518398,
				0.040908173,
				0.012777518,
				0.038880546,
				0.049085863,
				0.032236215,
				0.059107684,
				0.02500126,
				0.00640589,
				0.039520133,
				0.066216588,
				0.03575873,
				0.028480541,
				0.014959369,
				0.00399899,
				0.035790734,
				0.03992743,
				0.048084497,
				0.002207351,
				0.030950662,
				0.320640296,
				0.034640063,
				0.006563974,
				0.029881194,
				0.022486171,
				0.011634802,
				0.273039907,
				0.48763299,
				0.111061722,
				0.05188515,
				0.095795095,
				0.078814439,
				0.105761215,
				0.140504986,
				0.085491545,
				0.087526135,
				0.008067675,
				0.033521228,
				0.006883552,
				0.064519316,
				0.003384398,
				0.014617815,
				0.032730252,
				0.001646112,
				0.00196098,
				0.012564101,
				0.304068267,
				0.267732918,
				0.209717855,
				0.06585405,
				0.00726669,
				0.037195243,
				0.074893326,
				0.012096515,
				0.046475943,
				0.0681374
			};
		}
		#endregion
	}
}

