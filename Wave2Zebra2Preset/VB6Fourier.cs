/*
// -----------------------------------------------------------------------
// Fourier.bas
// Fourier transform and related operations
// 
// This file is part of the DynaPlot sample application Spectrogram
// Wilhelm Kurz, 2001
// -----------------------------------------------------------------------
 */
using System;

namespace Wave2Zebra2Preset
{
	/// <summary>
	/// Description of VB6Fourier.
	/// </summary>
	public class VB6Fourier
	{	
        // -----------------------------------------------------------------------
        // This file is part of the DynaPlot sample application Spectrogram
        // Wilhelm Kurz, 2001
        // -----------------------------------------------------------------------
        private const double pi = Math.PI; // pi = 3.14159265358979;
        public const double W0Hanning = 0.5;
        public const double W0Hamming = 0.54;
        public const double W0Blackman = 0.42;
        
        public static void MagnitudeSpectrum(double[] real, double[] imag, long arraysize, double W0, out float[] magnitude)
        {
			magnitude = new float[arraysize / 2];
        	long i = 0;

            magnitude[0] = (float) Math.Sqrt(SquareSum(real[0], imag[0]));
            for(i = 1;
                i <= arraysize / 2.0 - 1.0;
                i = Convert.ToInt64(i + 1))
            {
            	magnitude[i] = (float)((Math.Sqrt(SquareSum(real[i], imag[i]) + SquareSum(real[arraysize - i], imag[arraysize - i]))) / W0);
            }
        }

        public static double Hanning(long n, long j) 
        {
            return W0Hanning - 0.5 * Math.Cos(2D * pi * j / n);
        }

        public static double Hamming(long n, long j) 
        {
            return W0Hamming - 0.46 * Math.Cos(2D * pi * j / n);
        }

        public static double Blackman(long n, long j) 
        {
            return W0Blackman - 0.5 * Math.Cos(2D * pi * j / n) + 0.08 * Math.Cos(4D * pi * j / n);
        }

        public static void FourierTransform(double[] real, double[] imag, long arraysize, bool forward)
        {
            long LdArraysize = 0;
            long arg = 0;
            long count = 0;
            long c0 = 0;
            long c1 = 0;
            long i = 0;
            long j = 0;
            long a = 0;
            long b = 0;
            long k = 0;
            double sign = 0;
            double prodreal = 0;
            double prodimag = 0;
            double phase0 = 0;
            double[] cosarray = null;
            double[] sinarray = null;

            cosarray = new double[(int)(arraysize - 1) + 1];
            sinarray = new double[(int)(arraysize - 1) + 1];

            j = 0;
            if (forward == true)
            {
                sign = -1D;
                for(i = 0;
                    i <= arraysize - 1;
                    i = Convert.ToInt64(i + 1))
                {
                    real[i] = real[i] / arraysize;
                    imag[i] = imag[i] / arraysize;
                }
            }
            else
            {
                sign = 1D;
            }

            for(i = 0;
                i <= arraysize - 2;
                i = Convert.ToInt64(i + 1))
            {
                if (i < j)
                {
                    Swap(ref real[i], ref real[j]);
                    Swap(ref imag[i], ref imag[j]);
                }
                k = arraysize / 2;
                while(k <= j)
                {
                    j = j - k;
                    k = k / 2;
                }
                j = j + k;
            }

            LdArraysize = 0;
            i = arraysize;
            while((i != 1))
            {
                LdArraysize = LdArraysize + 1;
                i = i / 2;
            }
            phase0 = 2.0 * pi / arraysize;
            for(i = 0;
                i <= arraysize - 1;
                i = Convert.ToInt64(i + 1))
            {
                sinarray[i] = sign * Math.Sin(phase0 * i);
                cosarray[i] = Math.Cos(phase0 * i);
            }

            a = 2;
            b = 1;
            for(count = 1;
                count <= LdArraysize;
                count = Convert.ToInt64(count + 1))
            {
                c0 = Convert.ToInt64(arraysize / a);
                c1 = 0;
                for(k = 0;
                    k <= b - 1;
                    k = Convert.ToInt64(k + 1))
                {
                    i = k;
                    while((i < arraysize))
                    {
                        arg = i + b;
                        if (k == 0)
                        {
                            prodreal = Convert.ToDouble(real[arg]);
                            prodimag = Convert.ToDouble(imag[arg]);
                        }
                        else
                        {
                            prodreal = Convert.ToDouble(real[arg]) * cosarray[(int)(c1)] - Convert.ToDouble(imag[arg]) * sinarray[(int)(c1)];
                            prodimag = Convert.ToDouble(real[arg]) * sinarray[(int)(c1)] + Convert.ToDouble(imag[arg]) * cosarray[(int)(c1)];
                        }
                        real[arg] = Convert.ToDouble(real[i]) - prodreal;
                        imag[arg] = Convert.ToDouble(imag[i]) - prodimag;
                        real[i] = Convert.ToDouble(real[i]) + prodreal;
                        imag[i] = Convert.ToDouble(imag[i]) + prodimag;
                        i = i + a;
                    }
                    c1 = c1 + c0;
                }
                a = a * 2;
                b = b * 2;
            }
        }

        private static void Swap(ref double a, ref double b)
        {
        	double tempr = 0;
            tempr = a;
            a = b;
            b = tempr;
        }

        private static double SquareSum(double a, double b) 
        {
            return a * a + b * b;
        }
        
    }
	
}
