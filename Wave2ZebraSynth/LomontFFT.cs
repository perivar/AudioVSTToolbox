// Code to implement decently performing FFT for complex and real valued
// signals. See www.lomont.org for a derivation of the relevant algorithms 
// from first principles. Copyright Chris Lomont 2010. 
// This code and any ports are free for all to use for any reason as long 
// as this header is left in place.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lomont
{
    /// <summary>
    /// Represent a class that performs real or complex valued Fast Fourier 
    /// Transforms. Instantiate it and use the FFT or TableFFT methods to 
    /// compute complex to complex FFTs. Use FFTReal for real to complex 
    /// FFTs which are much faster than standard FFTs.
    /// </summary>
    class LomontFFT
    {
        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with 
        /// data containing complex valued data as alternating real and 
        /// imaginary parts. The length must be a power of 2.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        public void FFT(double[] data, bool forward)
        {
            int n = data.Length;
            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0) 
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2");
            n /= 2;    // n is the number of samples

            Reverse(data, n); // bit index data reversal

            // do transform: so single point transforms, then doubles, etc.
            double sign = forward ? 1 : -1;
            int mmax = 1;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = sign * Math.PI / mmax;
                double wr = 1, wi = 0;
                double wpr = Math.Cos(theta);
                double wpi = Math.Sin(theta);
                for (int m = 0; m < istep; m += 2)
                {
                    for (int k = m; k < 2 * n; k += 2 * istep)
                    {
                        int j = k + istep;
                        double tempr = wr * data[j] - wi * data[j + 1];
                        double tempi = wi * data[j] + wr * data[j + 1];
                        data[j] = data[k] - tempr;
                        data[j + 1] = data[k + 1] - tempi;
                        data[k] = data[k] + tempr;
                        data[k + 1] = data[k + 1] + tempi;
                    }
                    double t = wr; // trig recurrence
                    wr = wr * wpr - wi * wpi;
                    wi = wi * wpr + t * wpi;
                }
                mmax = istep;
            }

            // inverse scaling in the backward case
            if (!forward)
            {
                double scale = 1.0 / n;
                for (int i = 0; i < 2 * n; ++i)
                    data[i] *= scale;
            }
        }


        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with data
        /// containing complex valued data as alternating real and imaginary 
        /// parts. The length must be a power of 2. This method caches values 
        /// and should be slightly faster on repeated uses than then FFT method. 
        /// It is also slightly more accurate.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        public void TableFFT(double[] data, bool forward)
        {
            int n = data.Length;
            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0) 
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2"
                    );
            n /= 2;    // n is the number of samples

            Reverse(data, n); // bit index data reversal

            // make table if needed
            if ((cosTable == null) || (cosTable.Count != n))
                Initialize(n);

            // do transform: so single point transforms, then doubles, etc.
            double sign = forward ? 1 : -1;
            int mmax = 1;
            int tptr = 0;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = sign * Math.PI / mmax;
                for (int m = 0; m < istep; m += 2)
                {
                    double wr = cosTable[tptr];
                    double wi = sign*sinTable[tptr++];
                    for (int k = m; k < 2 * n; k += 2 * istep)
                    {
                        int j = k + istep;
                        double tempr = wr * data[j] - wi * data[j + 1];
                        double tempi = wi * data[j] + wr * data[j + 1];
                        data[j] = data[k] - tempr;
                        data[j + 1] = data[k + 1] - tempi;
                        data[k] = data[k] + tempr;
                        data[k + 1] = data[k + 1] + tempi;
                    }
                }
                mmax = istep;
            }

            // copy out with optional scaling
            if (!forward)
            {
                double scale = 1.0 / n;
                for (int i = 0; i < 2 * n; ++i)
                    data[i] *= scale;
            }
        }

        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with 
        /// data containing real valued data only. The output is complex 
        /// valued after the first two entries, stored in alternating real 
        /// and imaginary parts. The first two returned entries are the real 
        /// parts of the first and last value from the conjugate symmetric 
        /// output, which are necessarily real. The length must be a power 
        /// of 2.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        public void RealFFT(double[] data, bool forward)
        {
            int n = data.Length; // # of real inputs, 1/2 the complex length
            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0)
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2"
                    );

            double sign = -1;
            if (forward)
            { // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, forward); 
                sign = 1;
            }

            double theta = sign * 2 * Math.PI / n;
            double wpr = Math.Cos(theta);
            double wpi = Math.Sin(theta);
            double wjr = wpr;
            double wji = wpi;
            for (int j = 1; j <= n / 4; ++j)
            {
                int k = n / 2 - j;
                double tnr = data[2 * k];
                double tni = data[2 * k + 1];
                double tjr = data[2 * j];
                double tji = data[2 * j + 1];

                double e = (tjr + tnr);
                double f = (tji - tni);
                double a = (tjr - tnr) * wji;
                double d = (tji + tni) * wji;
                double b = (tji + tni) * wjr;
                double c = (tjr - tnr) * wjr;

                // compute entry y[j]
                data[2 * j] = 0.5 * (e + sign * (a + b));
                data[2 * j + 1] = 0.5 * (f - sign * (c - d));

                // compute entry y[k]
                data[2 * k] = 0.5 * (e - sign * (a + b));
                data[2 * k + 1] = 0.5 * (sign * (-c + d) - f);

                double temp = wjr;
                // todo - allow more accurate version here? make option?
                wjr = wjr * wpr - wji * wpi;  
                wji = temp * wpi + wji * wpr;
            }

            if (forward)
            {
                // compute final y0 and y_{N/2}, store data[0], data[1]
                double temp = data[0];
                data[0] += data[1];
                data[1] = temp - data[1];
            }
            else
            {
                double temp = data[0]; // unpack the y[j], then invert FFT
                data[0] = 0.5 * (temp + data[1]);
                data[1] = 0.5 * (temp - data[1]);
                // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, false);  
            }
        }

        #region Internals

        /// <summary>
        /// Call this with the size before using the TableFFT version
        /// Fills in tables for speed. Done automatically in TableFFT
        /// </summary>
        /// <param name="size">The size of the FFT in samples</param>
        void Initialize(int size)
        {
            // NOTE: if you do not use garbage collected languages 
            // like C# or Java be sure to free these correctly
            cosTable = new List<double>();
            sinTable = new List<double>();

            // forward pass
            int n = size;
            int mmax = 1;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = Math.PI / mmax;
                double wr = 1, wi = 0;
                double wpi = Math.Sin(theta);
                // compute in a slightly slower yet more accurate manner
                double wpr = Math.Sin(theta / 2);
                wpr = -2 * wpr * wpr; 
                for (int m = 0; m < istep; m += 2)
                {
                    cosTable.Add(wr);
                    sinTable.Add(wi);
                    double t = wr;
                    wr = wr * wpr - wi * wpi + wr;
                    wi = wi * wpr + t * wpi + wi;
                }
                mmax = istep;
            }
        } 

        /// <summary>
        /// Swap data indices whenever index i has binary 
        /// digits reversed from index j, where data is
        /// two doubles per index.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="n"></param>
        void Reverse(double [] data, int n)
        {
            // bit reverse the indices. This is exercise 5 in section 
            // 7.2.1.1 of Knuth's TAOCP the idea is a binary counter 
            // in k and one with bits reversed in j
            int j = 0, k = 0; // Knuth R1: initialize
            int top = n / 2;  // this is Knuth's 2^(n-1)
            while (true)
            {
                // Knuth R2: swap - swap j+1 and k+2^(n-1), 2 entries each
                double t = data[j + 2]; 
                data[j + 2] = data[k + n]; 
                data[k + n] = t;
                t = data[j + 3]; 
                data[j + 3] = data[k + n + 1]; 
                data[k + n + 1] = t;
                if (j > k)
                { // swap two more
                    // j and k
                    t = data[j]; 
                    data[j] = data[k]; 
                    data[k] = t;
                    t = data[j + 1]; 
                    data[j + 1] = data[k + 1]; 
                    data[k + 1] = t;
                    // j + top + 1 and k+top + 1
                    t = data[j + n + 2]; 
                    data[j + n + 2] = data[k + n + 2]; 
                    data[k + n + 2] = t;
                    t = data[j + n + 3]; 
                    data[j + n + 3] = data[k + n + 3]; 
                    data[k + n + 3] = t;
                }
                // Knuth R3: advance k
                k += 4;
                if (k >= n)
                    break;
                // Knuth R4: advance j
                int h = top;
                while (j >= h)
                {
                    j -= h;
                    h /= 2;
                }
                j += h;
            } // bit reverse loop
        }

        /// <summary>
        /// Precomputed sin/cos tables for speed
        /// </summary>
        List<double> cosTable;
        List<double> sinTable;
        
        #endregion 

        #region UnitTest
        /// <summary>
        /// Return true if unit tests pass
        /// </summary>
        /// <returns>true if and only if the tests all passed</returns>
        public bool UnitTest()
        {
            // some tests of various lengths
            double[] t4 = { 1, 1, 1, 1 }; // input
            double[] a4r = { 4, 0, 0, 0 }; // real FFT
            double[] a4c = { 2, 2, 0, 0 };    // complex FFT, ...
            double[] t4a = { 1, 2, 3, 4 };
            double[] a4ar = { 10, -2, -2, -2 };
            double[] a4ac = { 4, 6, -2, -2 };
            double[] t8 = { 0.100652, -0.442825, -0.457954, -0.00624455, 0.19978, -0.267328, -0.47192, -0.235878 };
            double[] a8r = { -1.58172, 0.322834, -0.385598, 0.0522465, 1.23031, -0.468031, 0.187343, 0.0243147 };
            double[] a8c = { -0.629442, -0.952276, -0.328761, -0.161531, 1.23031, -0.46803, 0.130505, -0.189463 };
            double[] t32 = { -0.333615, 0.468917, 0.884538, 0.0276625, 0.979812, 0.91061, -0.175599, 0.1756, -0.695263, 0.557298, 0.112251, -0.285586, -0.73988, -0.0750604, -0.332421, 0.391004, 0.0588164, -0.18941, -0.416513, -0.596507, 0.659257, -0.654753, -0.472673, 0.875249, -0.00712734, -0.12367, -0.357211, -0.152413, 0.0130609, -0.0342799, 0.818388, 0.671986 };
            double[] a32r = { 1.96247, -1.97083, 4.71435, 1.34203, 1.41278, 2.2209, -0.301542, 1.30462, 0.717877, -1.42063, -3.19595, -1.52441, -0.474644, -2.90705, 0.747585, 2.44391, -0.125698, -0.247344, -4.4128, -1.07521, -1.28254, 2.42047, -1.30217, -0.450559, -4.49676, -2.19137, 0.193633, 0.848902, 2.05478, -1.91513, 0.417439, 1.79843 };
            double[] a32c = { -0.00417904, 1.96665, 1.44498, 2.18532, 1.46969, 1.82996, -1.0868, 0.620212, 1.23124, 0.951988, -2.48776, -1.88406, -0.412289, -2.73394, 0.564497, 2.93413, -0.125699, -0.247344, -4.22971, -0.584989, -1.3449, 2.59358, -2.01036, -0.810202, -5.01012, 0.181248, 0.97889, 0.164497, 1.99787, -2.30607, 3.68681, 2.64171 };

            double[][] tests = { t4, a4r, a4c, t4a, a4ar, a4ac, t8, a8r, a8c, t32, a32r, a32c };//, t32, a32 };

            bool ret = true;
            for (int testIndex = 0; testIndex < tests.Length; testIndex += 3)
            {
                double[] test = tests[testIndex];
                double[] answerReal = tests[testIndex + 1];
                double[] answerComplex = tests[testIndex + 2];

                ret &= Test(RealFFT, test, answerReal);
                ret &= Test(FFT, test, answerComplex);
                ret &= Test(TableFFT, test, answerComplex);
            }
            return ret;
        }

        /// <summary>
        /// Test the given function on the given data and see if the result is the given answer.
        /// </summary>
        /// <returns>true if matches</returns>
        bool Test(Action<double[], bool> FFTFunction, double[] test, double[] answer)
        {
            bool returnValue = true;
            var copy = test.ToArray(); // make a copy
            FFTFunction(copy, true); // forward transform
            returnValue &= Compare(copy, answer); // check it
            FFTFunction(copy, false); // backward transform
            returnValue &= Compare(copy, test); // check it
            return returnValue;
        }

        /// <summary>
        /// Compare two arrays of doubles for "equality"
        /// up to a small tolerance
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        static bool Compare(double[] arr1, double[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            for (int i = 0; i < arr1.Length; ++i)
                if ((Math.Abs(arr1[i] - arr2[i]) > 0.0001))
                    return false;
            return true;
        }

        #endregion
    }
}
// end of file
