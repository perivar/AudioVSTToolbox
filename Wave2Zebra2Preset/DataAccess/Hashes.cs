// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System.Collections.Generic;
using Wave2ZebraSynth.Model;

namespace Wave2ZebraSynth.DataAccess
{
    /// <summary>
    ///   Hashes used in Query/Creation mechanisms
    /// </summary>
    public class Hashes
    {
        /// <summary>
        ///   Parameter less constructor
        /// </summary>
        public Hashes()
        {
            Query = new HashSet<HashSignature>();
            Creational = new HashSet<HashSignature>();
        }

        /// <summary>
        ///   Query hashes (used in querying the database)
        /// </summary>
        public HashSet<HashSignature> Query { get; set; }

        /// <summary>
        ///   Creational hashes (used in creation of the database)
        /// </summary>
        public HashSet<HashSignature> Creational { get; set; }
    }
}