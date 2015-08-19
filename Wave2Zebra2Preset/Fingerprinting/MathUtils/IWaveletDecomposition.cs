// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

namespace Wave2Zebra2Preset.Fingerprinting.MathUtils
{
    /// <summary>
    ///   Wavelet decomposition algorithm
    /// </summary>
    public interface IWaveletDecomposition
    {
        /// <summary>
        ///   Apply wavelet decomposition on the selected image
        /// </summary>
        /// <param name = "frames">Frames to be decomposed</param>
        void DecomposeImageInPlace(float[][] frames);
    }
}