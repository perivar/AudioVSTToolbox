using System;
using System.IO;
using System.Diagnostics;
using CommonUtils;

namespace CommonUtils.FFT
{
	/// <summary>
	/// FFTTesting Class.
	/// </summary>
	public static class FFTTesting
	{
		public static void LomontFFTTestUsingDouble(string CSVFilePath=null, double[] audio_data=null) {
			
			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			// perform the FFT
			double[] spectrum_fft = FFTUtils.FFT(audio_data);

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

		public static void ExocortexFFTTestUsingFloats(string CSVFilePath=null, double[] audio_data=null) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			float[] audio_data_float = MathUtils.DoubleToFloat(audio_data);
			
			// build the complex array
			int N = audio_data_float.Length * 2;
			float[] complexSignal = FFTUtils.FloatToComplexFloat(audio_data_float);

			// perform the FFT
			Fourier.FFT(complexSignal, N/2, FourierDirection.Forward);
			
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

		public static void ExocortexFFTTestUsingComplexF(string CSVFilePath=null, double[] audio_data=null) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}

			float[] audio_data_float = MathUtils.DoubleToFloat(audio_data);
			
			// build the complex array
			int N = audio_data_float.Length;
			ComplexF[] complexSignal = FFTUtils.FloatToComplex(audio_data_float);
			
			// perform the FFT
			Fourier.FFT(complexSignal, N, FourierDirection.Forward);

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
		
		public static void ExocortexFFTTestUsingComplex(string CSVFilePath=null, double[] audio_data=null) {

			if (audio_data == null) {
				audio_data = GetSignalTestData();
			}
			
			// build the complex array
			int N = audio_data.Length;
			Complex[] complexSignal = FFTUtils.DoubleToComplex(audio_data);
			
			// perform the FFT
			Fourier.FFT(complexSignal, N, FourierDirection.Forward);

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
		
		public static void TimeAll(int count, bool DoCVSExport) {

			// ouput results to csv
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			
			// Start the stopwatch
			Stopwatch sw = Stopwatch.StartNew();
			
			string export = null;
			if (DoCVSExport) {
				export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingComplexF.csv";
			}
			for (int i = 0; i < count; i++) {
				CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplexF(export);
			}

			// Stop the stopwatch
			sw.Stop();

			// Report the results
			System.Diagnostics.Debug.WriteLine("ExocortexFFTTestUsingComplexF: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
			

			
			sw.Restart();

			export = null;
			if (DoCVSExport) {
				export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingComplex.csv";
			}
			for (int i = 0; i < count; i++) {
				CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex(export);
			}

			// Stop the stopwatch
			sw.Stop();

			// Report the results
			System.Diagnostics.Debug.WriteLine("ExocortexFFTTestUsingComplex: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);


			sw.Restart();

			export = null;
			if (DoCVSExport) {
				export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingFloats.csv";
			}
			for (int i = 0; i < count; i++) {
				CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingFloats(export);
			}

			// Stop the stopwatch
			sw.Stop();

			// Report the results
			System.Diagnostics.Debug.WriteLine("ExocortexFFTTestUsingFloats: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);



			sw.Restart();

			export = null;
			if (DoCVSExport) {
				export = path + Path.DirectorySeparatorChar + "LomontFFTTestUsingDouble.csv";
			}
			for (int i = 0; i < count; i++) {
				CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble(export);
			}

			// Stop the stopwatch
			sw.Stop();

			// Report the results
			System.Diagnostics.Debug.WriteLine("LomontFFTTestUsingDouble: Time used: {0} ms",sw.Elapsed.TotalMilliseconds);
		}
		
		public static void TestAll(double[] audio_data) {
			
			// ouput results to csv
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string export;
			
			export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingComplexF.csv";
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplexF(export, audio_data);

			export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingComplex.csv";
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex(export, audio_data);

			export = path + Path.DirectorySeparatorChar + "ExocortexFFTTestUsingFloats.csv";
			CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingFloats(export, audio_data);

			export = path + Path.DirectorySeparatorChar + "LomontFFTTestUsingDouble.csv";
			CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble(export, audio_data);
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
		#endregion
	}
}
