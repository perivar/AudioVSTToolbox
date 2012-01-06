// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;

namespace AudioSystem
{
    /// <summary>
    ///   Digital signal processing proxy
    /// </summary>
    public interface IAudio : IDisposable
    {
        /// <summary>
        ///   Read from file at a specific frequency rate. The data is read in mono format
        /// </summary>
        /// <param name = "filename">Filename to read from</param>
        /// <param name = "samplerate">Sample rate</param>
        /// <returns>Array with PCM samples</returns>
        float[] ReadMonoFromFile(string filename, int samplerate);

        /// <summary>
        ///   Read mono from file
        /// </summary>
        /// <param name = "filename">Filename to read from</param>
        /// <param name = "samplerate">Sample rate</param>
        /// <param name = "milliseconds">Milliseconds to read</param>
        /// <param name = "startmillisecond">Starting milliseconds</param>
        /// <returns>Array with PCM samples</returns>
        float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond);
    }
}