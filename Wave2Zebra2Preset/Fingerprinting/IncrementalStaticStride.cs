// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

namespace Wave2Zebra2Preset.Fingerprinting
{
    /// <summary>
    ///   Incremental stride
    /// </summary>
    public class IncrementalStaticStride : IStride
    {
        /// <summary>
        ///   Increment by parameter (usually negative)
        /// </summary>
        private readonly int _incrementBy;

        private int _firstStride;

        /// <summary>
        ///   Incremental stride constructor
        /// </summary>
        /// <param name = "incrementBy">Increment by parameter in audio samples</param>
        /// <param name = "samplesInFingerprint">Number of samples in one fingerprint [normally 8192]</param>
        public IncrementalStaticStride(int incrementBy, int samplesInFingerprint)
        {
            _incrementBy = -samplesInFingerprint + incrementBy; /*Negative stride will guarantee that the signal is incremented by the parameter specified*/
            _firstStride = 0;
        }

        #region IStride Members

        /// <summary>
        ///   Gets stride size
        /// </summary>
        /// <returns>Negative stride</returns>
        public int GetStride()
        {
            return _incrementBy;
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

        /// <summary>
        /// Set starting stride
        /// </summary>
        /// <param name="firstStride">First stride</param>
        public void SetFirstStride(int firstStride)
        {
            _firstStride = firstStride;
        }
    }
}