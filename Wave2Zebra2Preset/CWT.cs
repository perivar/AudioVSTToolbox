/* From: "A SURVEY OF COMPUTATIONAL PHYSICS"
   by RH Landau, MJ Paez, and CC BORDEIANU
   Copyright Princeton University Press, Princeton, 2008.
   Electronic Materials copyright: R Landau, Oregon State Univ, 2008;
   MJ Paez, Univ Antioquia, 2008; and CC BORDEIANU, Univ Bucharest, 2008.
   Support by National Science Foundation
 */
 
using System;
using System.IO;

namespace CWT
{
	public class CWT
	{

		static void Main(string[] argv)
		{
			int i , j , o , t , n=120, m=100;
			double[][] c = RectangularArrays.ReturnRectangularDoubleArray(m, n);
			double[] input = new double[1024]; double omega1 = 1.0;
			double tau , dtau , omega , domega , WTreal , WTimag , max , omega2 = 5.0, tau1 = -81.92;
			double[] PsiReal = new double[16834]; double[] PsiImag = new double[16834];

			dtau = 1.0/m;
			Console.WriteLine("Transform in CWT.dat, signal in CWTin.dat");
			//PrintWriter w = new PrintWriter (new FileOutputStream("CWT.dat"), true);
			StreamWriter w = File.CreateText(@"C:\CWT.csv"); 
			//PrintWriter q = new PrintWriter (new FileOutputStream("CWTin.dat"), true);
			StreamWriter q = File.CreateText(@"C:\CWTin.csv"); 
			for(t=0; t < 750; t++)
			{
				if(t > 0 && t <= 250)
				{
					input[t] = 5*Math.Sin(6.28*t);
				}
				if(t >= 250 && t <= 750)
				{
					input[t] = 10*Math.Sin(2.0*6.28*t);
				}
				q.WriteLine(""+input[t]+"");
			}

			// Psi(t) = Psi((t-tau)/s) = Psi((t-tau)*omega), tau2 = tau1 + n*dtau
			domega = Math.Pow(omega2/omega1, 1.0/m); // Scaling
			//domega = df;
			//omega2 = upper_freq
			//omega1 = lower_freq
			//m = 256
			//dtau = dt			
			omega = omega1;
			for (i = 0; i < m; i++) // Compute daughter wavelet
			{
				// compute new kernels for current frequency
				tau = tau1;
				for (o=0; o<16834; o++) // For signals up to 2^13 = 8192 long
				{
					PsiReal[o] = WaveletReal(tau*omega);
					PsiImag[o] = WaveletImag(tau*omega);
					tau = tau + dtau; // Translation
				}				
				// compute values of CWT across row
				for (j=0; j<n; j++) // Compute CWT
				{
					WTreal = 0.0;
					WTimag = 0.0;
					for (o=0;o<input.Length;o++)
					{
						WTreal += input[o]*PsiReal[8192-(j*input.Length)/n+o];
						WTimag += input[o]*PsiImag[8192-(j*input.Length)/n+o];
					}
					c[i][j] = Math.Sqrt(WTreal*WTreal+WTimag*WTimag);
				}
				omega = omega*domega; // Scaling
			}
			max = 0.0001;
			for (i = 0; i<m; i++)
			{
				for (j=0; j<n; j++) // Renormalize
				{
					if (c[i][j] > max)
					{
						max = c[i][j];
					}
					w.WriteLine(" "+c[i][j]/max+" ");
				}
				w.WriteLine("");
			}
		}

		public static double WaveletReal(double t) // Re Morlet wavelet
		{
			double sigma = 4.0;
			return Math.Cos(2.0*Math.PI*t)* Math.Exp(-t*t/(2.0*sigma*sigma)) / (sigma*Math.Sqrt(2.0*Math.PI));
		}

		public static double WaveletImag(double t) // Im Morlet wavelet
		{
			double sigma = 4.0;
			return Math.Sin(2.0*Math.PI*t)* Math.Exp(-1.0*t*t/(2.0*sigma*sigma)) / (sigma*Math.Sqrt(2.0*Math.PI));
		}
	}

	//----------------------------------------------------------------------------------------
	//	Copyright © 2007 - 2011 Tangible Software Solutions Inc.
	//	This class can be used by anyone provided that the copyright notice remains intact.
	//
	//	This class provides the logic to simulate Java rectangular arrays, which are jagged
	//	arrays with inner arrays of the same length.
	//----------------------------------------------------------------------------------------
	internal static partial class RectangularArrays
	{
		internal static double[][] ReturnRectangularDoubleArray(int Size1, int Size2)
		{
			double[][] Array = new double[Size1][];
			for (int Array1 = 0; Array1 < Size1; Array1++)
			{
				Array[Array1] = new double[Size2];
			}
			return Array;
		}
	}
}