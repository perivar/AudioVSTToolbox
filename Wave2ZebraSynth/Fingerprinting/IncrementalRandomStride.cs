// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;

namespace Wave2ZebraSynth.Fingerprinting
{
    /// <summary>
    ///   Incremental random stride
    /// </summary>
    public class IncrementalRandomStride : IStride
    {
        /// <summary>
        ///   Random stride
        /// </summary>
        private static readonly Random Random = new Random(unchecked((int) DateTime.Now.Ticks));

        private readonly int _firstStride;

        /// <summary>
        ///   Incremental random stride constructor
        /// </summary>
        /// <param name = "min">Min step in samples</param>
        /// <param name = "max">Max step in samples</param>
        /// <param name = "samplesPerFingerprint">Samples per fingerprint</param>
        public IncrementalRandomStride(int min, int max, int samplesPerFingerprint)
        {
            Min = min;
            Max = max;
            SamplesPerFingerprint = samplesPerFingerprint;
            _firstStride = 0;
        }

        /// <summary>
        ///   Minimal step between consecutive strides
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        ///   Maximal step between consecutive strides
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        ///   Number of samples per fingerprint
        /// </summary>
        public int SamplesPerFingerprint { get; set; }

        #region IStride Members

        /// <summary>
        ///   Get stride size
        /// </summary>
        /// <returns>Gets the stride</returns>
        public int GetStride()
        {
            return -SamplesPerFingerprint + Random.Next(Min, Max);
        }

        /// <summary>
        /// Get starting stride
        /// </summary>
        /// <returns>Stride</returns>
        public int GetFirstStride()
        {
            return _firstStride;
        }

        #endregion
    }
}