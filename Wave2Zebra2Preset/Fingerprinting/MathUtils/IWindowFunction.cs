// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com
namespace Wave2ZebraSynth.Fingerprinting.MathUtils
{
    /// <summary>
    ///   Window function for spectrogram computing
    /// </summary>
    public interface IWindowFunction
    {
        /// <summary>
        ///   Window the outer space in place
        /// </summary>
        /// <param name = "outerspace">Array to be windowed</param>
        /// <param name = "windowLength">Window length</param>
        void WindowInPlace(double[] outerspace, int windowLength);

        /// <summary>
        ///   Window the outer space in place
        /// </summary>
        /// <param name = "outerspace">Array to be windowed</param>
        /// <param name = "windowLength">Window length</param>
        void WindowInPlace(Complex[] outerspace, int windowLength);

        /// <summary>
        ///   Gets the corresponding window function
        /// </summary>
        /// <param name = "length">Length of the window</param>
        /// <returns>Window function</returns>
        double[] GetWindow(int length);
    }
}