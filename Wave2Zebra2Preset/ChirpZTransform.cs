using System;

namespace CommonUtils {
	/// <summary>
	/// ChirpZTransform.
	/// http://www.dsprelated.com/showmessage/13193/1.php
	/// http://www.embedded.com/design/configurable-systems/4006427/A-DSP-algorithm-for-frequency-analysis
	/// </summary>
	public class ChirpZTransform
	{

		/************Chirp Z Transform *****************
		 * N = # input samples. M = # output samples.
		 *
		 * fsam   = the sample frequency in Hz.
		 * fstart = the start frequency in Hz.
		 * fstop  = the stop frequency in Hz for the band over which the transform is computed.
		 *
		 * (fstart - fstop)/M = new resolution
		 *
		 * Note: this method returns an array of length L.
		 * L = the returned transform length. L will always be larger than M.
		 * See code for how L is determined
		 *
		 **********************************************/
		public double[][] czt( double [][] array, int N, int M, double fStart, double fStop, double fSam )
		{
			int L;

			/***Determine length of CZT output***/
			if( (N+M) > 512){
				L = 1024;
			}else if( ((N+M) > 256) && ((N+M) <= 512) ){
				L = 512;
			}else if( ((N+M) > 128) && ((N+M) <= 256) ){
				L = 256;
			}else if( ((N+M) > 64 ) && ((N+M) <= 128) ){
				L = 128;
			}else if( ((N+M) > 32 ) && ((N+M) <= 64) ){
				L = 64;
			}else if( ((N+M) > 16 ) && ((N+M) <= 32) ){
				L = 32;
			}else{
				L = 16;
			}

			//double[][] g = new double[L][2];
			//double[][] h = new double[L][2];
			double[][] g = new double[L][];
			double[][] h = new double[L][];
			
			double theta0;
			double phi0;
			double psi;
			double a;
			double b;
			int n;
			int k;

			phi0 = 2.0 * Math.PI*(fStop-fStart)/fSam/(M-1);
			theta0 = 2.0 * Math.PI*fStart/fSam;

			/*** Create arc coefficients ***/
			for( n = 0; n < M; n++ ){
				h[n][0] = Math.Cos( n*n/2.0 * phi0 );
				h[n][1] = Math.Sin( n*n/2.0 * phi0 );
			}
			for( n = M; n < L-N; n++ ){
				h[n][0] = 0.0;
				h[n][1] = 0.0;
			}
			for( n = L-N; n < L; n++){
				h[n][0] = Math.Cos( (L-n)*(L-n)/2.0 * phi0 );
				h[n][1] = Math.Sin( (L-n)*(L-n)/2.0 * phi0 );
			}

			/*** Prepare signal ***/
			for( n = 0; n < N; n++ ){
				g[n][0] = array[n][0];
				g[n][1] = array[n][1];
			}
			for( n = N; n < L; n++ ){
				g[n][0] = 0.0;
				g[n][1] = 0.0;
			}
			
			double s;
			double c;
			
			for( n = 0; n < N; n++ ){
				psi = n*theta0 + n*n/2.0 * phi0;
				c =  Math.Cos( psi);
				s = -Math.Sin( psi );
				a = c*g[n][0] - s*g[n][1];
				b = s*g[n][0] + c*g[n][1];
				g[n][0] = a;
				g[n][1] = b;
			}
			//use your favorite fft algorithm here.
			g = fft_1d( g );   //fft of samples
			h = fft_1d( h );   //fft of arc coeff
			
			/** convolution in the time domain is multiplication in the frequency domain **/
			/** multiplication in the time domain is convolution in the frequency domain **/
			for( n = 0; n < L; n++ ){
				c = g[n][0];
				s = g[n][1];
				a = c*h[n][0] - s*h[n][1];
				b = s*h[n][0] + c*h[n][1];
				g[n][0] = a/L;  /* for scaling purposes since fft_1d does not use scale */
				g[n][1] = b/L;
			}

			g = ifft_1d( g );   //use your favorite fft algorithm here.

			for( k = 0; k < M; k++ ){
				psi = k*k/2.0 * phi0;
				c =  Math.Cos( psi );
				s = -Math.Sin( psi );
				a = c*g[k][0] - s*g[k][1];
				b = s*g[k][0] + c*g[k][1];
				g[k][0] = a;
				g[k][1] = b;
			}
			
			return g;
		}

		private static double[][] fft_1d(double[][] g) {
			return g;
		}
		
		private static double[][] ifft_1d(double[][] g) {
			return g;
		}
	}
}
