// Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Wave2ZebraSynth.Audio;
using Wave2ZebraSynth.Fingerprinting;
using Wave2ZebraSynth.Model;
using Un4seen.Bass.AddOn.Tags;

namespace Wave2ZebraSynth.DataAccess
{
    /// <summary>
    ///   Singleton class for repository container
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Min hasher
        /// </summary>
        private readonly MinHash _hasher;

        /// <summary>
        ///   Creates fingerprints according to the theoretical constructs
        /// </summary>
        private readonly FingerprintManager _manager;

        /// <summary>
        ///   Storage for min-hash permutations
        /// </summary>
        private readonly IPermutations _permutations;

        /// <summary>
        ///   Storage for hash signatures and tracks
        /// </summary>
        private readonly IStorage _storage;

        /// <summary>
        /// Number of threads executing
        /// </summary>
        private volatile int _threadCounts;

        /// <summary>
        /// Aborted ?
        /// </summary>
        private volatile bool _aborted;

        /// <summary>
        ///   Each repository should have storage for permutations and for tracks/fingerprints
        /// </summary>
        /// <param name = "storage">Track/Signatures storage</param>
        /// <param name = "permutations">Permutations storage</param>
        public Repository(IStorage storage, IPermutations permutations)
        {
            _permutations = permutations;
            _storage = storage;
            _manager = new FingerprintManager();
            _hasher = new MinHash(_permutations);
        }

        /// <summary>
        ///   Create fingerprints/min-hash signatures/LSH buckets and insert them in underlying storage
        /// </summary>
        /// <param name = "files">Files to be processed</param>
        /// <param name = "audioProxy">Audio proxy used in processing</param>
        /// <param name = "queryStride">Stride between 2 consecutive fingerprints on query</param>
        /// <param name = "creationalStride">Stride between 2 consecutive fingerprints on creation</param>
        /// <param name = "mintracklen">Minimum track length</param>
        /// <param name = "maxtracklen">Maximum track length</param>
        /// <param name = "milliSecondsToProcess">Number of milliseconds to process</param>
        /// <param name = "startMillisecond">Start processing at a specific millisecond</param>
        /// <param name = "hashTables">Number of hash-tables used in LSH decomposition</param>
        /// <param name = "hashKeys">Number of Min Hash keys per LSH table</param>
        /// <param name = "trackProcessed">Invoked once the track is processed</param>
        /// <returns>List of processed tracks</returns>
        public List<Track> ProcessTracks(List<string> files, BassProxy audioProxy, IStride queryStride, IStride creationalStride,
                                  int mintracklen, int maxtracklen,
                                  int milliSecondsToProcess, int startMillisecond, int hashTables, int hashKeys, Action<Track> trackProcessed)
        {
            List<Track> tracks = new List<Track>();
            _threadCounts++;

            foreach (string file in files) //filter files that can be processed
            {
                if (_aborted) break;

                Track track = GetTrack(mintracklen, maxtracklen, file, audioProxy);
                if (track == null)
                    continue; /*track is not eligible because of min/max parameters*/
                //create spectrogram of the file
                float[][] logSpectrum = null;
                try
                {
                    //try creating the spectrum from a file
                    logSpectrum = _manager.CreateLogSpectrogram(audioProxy, track.Path, milliSecondsToProcess, startMillisecond);                    
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException)
                        throw;
                    /*the file might be corrupted or missing*/
                    continue; /*Continue processing even if creation of the spectrogram failed*/
                }
                _storage.InsertTrack(track); /*Insert track into the storage*/
                /*Create fingerprints that will be used as initial fingerprints to be queried*/
                List<bool[]> dbFingers = _manager.CreateFingerprints(logSpectrum, creationalStride);
                /*Get fingerprint's hash signature, and associate it to a specific track*/
                List<HashSignature> creationalsignatures = GetSignatures(dbFingers, track, hashTables, hashKeys);
                foreach (HashSignature hash in creationalsignatures)
                {
                    _storage.InsertHash(hash, HashType.Creational);
                    /*Set this hashes as also the query hashes*/
                    _storage.InsertHash(hash, HashType.Query);
                }
                /*Create fingerprints for query*/
                List<bool[]> queryFingers = _manager.CreateFingerprints(logSpectrum, queryStride); /*Create fingerprints*/
                List<HashSignature> querysignatures = GetSignatures(queryFingers, track, hashTables, hashKeys);
                
                // ***** PIN TODO: CHANGE THIS
		        object[][] arr = new object[querysignatures.Count][];
		        int count = 0;
                foreach (HashSignature hash in querysignatures) {
                    _storage.InsertHash(hash, HashType.Query); /*Insert hash-buckets into hash-tables*/                   

                    String signatureText = "{";
                    for (int s = 0; s < hash.Signature.Length; s++) {
                    	signatureText += hash.Signature[s];
                    	signatureText += " ";
                    }
                    signatureText += "}";
                    
			        arr[count++] = new object[5] { 
			        	hash.Id,
			        	hash.Track.Title, 
			        	hash.Track.Artist, 
			        	hash.Track.TrackLength,
			        	signatureText
			        };
		        }
				String filenameToSave = String.Format("C:\\{0}-hash-buckets.txt", System.IO.Path.GetFileNameWithoutExtension(file));                
                CSVWriter csv = new CSVWriter(filenameToSave);
                csv.Write(arr);
				// ***** end PIN
                
                if (trackProcessed != null)
                    trackProcessed.Invoke(track);
                tracks.Add(track);
            }
            _threadCounts--;
            return tracks;
        }
                
        /// <summary>
        /// Get track from the filename
        /// </summary>
        /// <param name="mintracklen">Min track length</param>
        /// <param name="maxtracklen">Max track length</param>
        /// <param name="filename">Filename from which to extract the requested info</param>
        /// <param name="proxy">Audio proxy to read tags</param>
        /// <returns>Track to be analyzed further / null if the track is not eligible</returns>
        private static Track GetTrack(int mintracklen, int maxtracklen, string filename, BassProxy proxy)
        {
            TAG_INFO tags = proxy.GetTagInfoFromFile(filename); //get file tags
            string artist, title;
            double duration;
            if (tags == null)
            {
                /*The song does not contain any tags*/
                artist = "Unknown";
                title = "Unknown";
                duration = 60;
            }
            else
            {
                /*The song contains related tags*/
                artist = tags.artist;
                title = tags.title;
                duration = tags.duration;
            }
            if (String.IsNullOrEmpty(artist)) /*assign a name to music files that don't have tags*/
                artist = "Unknown";
            if (String.IsNullOrEmpty(title)) /*assign a title to music files that don't have tags*/
                title = "Unknown";
            if (duration < mintracklen || duration > maxtracklen) /*check the duration of a music file*/ 
            {
            	System.Diagnostics.Debug.WriteLine(String.Format("File {0} failed the duration validation. Duration: {1} [Min: {2}, Max: {3}]", filename, duration, mintracklen, maxtracklen) );
                return null;
            }
            Track track = new Track {Artist = artist, Title = title, 
                TrackLength = duration, Path = Path.GetFullPath(filename)};
            return track;
        }

// ReSharper disable ReturnTypeCanBeEnumerable.Local
        private List<HashSignature> GetSignatures(IEnumerable<bool[]> fingerprints, Track track, int hashTables, int hashKeys)
// ReSharper restore ReturnTypeCanBeEnumerable.Local
        {
            List<HashSignature> signatures = new List<HashSignature>();
            foreach (bool[] fingerprint in fingerprints)
            {
                int[] signature = _hasher.ComputeMinHashSignature(fingerprint); /*Compute min-hash signature out of fingerprint*/
                Dictionary<int, long> buckets = _hasher.GroupMinHashToLSHBuckets(signature, hashTables, hashKeys); /*Group Min-Hash signature into LSH buckets*/
                int[] hashSignature = new int[buckets.Count];
                foreach (KeyValuePair<int, long> bucket in buckets)
                    hashSignature[bucket.Key] = (int) bucket.Value;
                HashSignature hash = new HashSignature(track, hashSignature); /*associate track to hash-signature*/
                signatures.Add(hash);
            }
            return signatures; /*Return the signatures*/
        }

        /// <summary>
        ///   Find duplicates between existing tracks in the database
        /// </summary>
        /// <param name = "tracks">Tracks to be processed (this list should contain only tracks that have been inserted previously)</param>
        /// <param name = "threshold">Number of threshold tables</param>
        /// <param name="percentageThreshold">Percentage of fingerprints threshold</param>
        /// <param name="callback">Callback invoked at each processed track</param>
        /// <returns>Sets of duplicates</returns>
        public HashSet<Track>[] FindDuplicates(List<Track> tracks, int threshold, double percentageThreshold, Action<Track, int, int> callback)
        {
            List<HashSet<Track>> duplicates = new List<HashSet<Track>>();
            int total = tracks.Count, current=0;
            foreach (Track track in tracks)
            {
                Dictionary<Track, int> trackDuplicates = new Dictionary<Track, int>(); /*this will be a set with duplicates*/
                HashSet<HashSignature> fingerprints = _storage.GetHashSignatures(track, HashType.Query); /*get all existing signatures for a specific track*/
                int fingerthreshold = (int) ((float) fingerprints.Count/100*percentageThreshold);
                foreach (HashSignature fingerprint in fingerprints)
                {
                    Dictionary<Track, int> results = _storage.GetTracks(fingerprint.Signature, threshold); /*get all duplicate track including the original track*/
                    foreach (KeyValuePair<Track, int> result in results)
                    {
                        if (!trackDuplicates.ContainsKey(result.Key))
                            trackDuplicates.Add(result.Key, 1);
                        else
                            trackDuplicates[result.Key]++;
                    }
                }
                if (trackDuplicates.Count > 1)
                {
                    IEnumerable<KeyValuePair<Track, int>> d = trackDuplicates.Where((pair) => pair.Value > fingerthreshold);
                    if (d.Count() > 1)
                        duplicates.Add(new HashSet<Track>(d.Select((pair) => pair.Key)));
                }
                if (callback != null)
                    callback.Invoke(track, total, ++current);
            }

            for (int i = 0; i < duplicates.Count - 1; i++)
            {
                HashSet<Track> set = duplicates[i];
                for (int j = i + 1; j < duplicates.Count; j++)
                {
                    IEnumerable<Track> result = set.Intersect(duplicates[j]);
                    if (result.Count() > 0)
                    {
                        duplicates.RemoveAt(j); /*Remove the duplicate set*/
                        i = -1; /*Start iterating from the beginning*/
                        break;
                    }
                }
            }
            return duplicates.ToArray();
        }

        /// <summary>
        /// Abort processing the files
        /// </summary>
        public void AbortProcessing()
        {
            _aborted = true;
            while(_threadCounts != 0)
            {
                Thread.Sleep(10);
            }
            _storage.ClearAll();
            _aborted = false;
        } 
    }
}