// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

namespace Wave2Zebra2Preset.DataAccess
{
    /// <summary>
    ///   Permutations storage
    /// </summary>
    public interface IPermutations
    {
        /// <summary>
        ///   Get Min Hash random permutations
        /// </summary>
        /// <returns>Permutation indexes</returns>
        int[][] GetPermutations();
    }
}