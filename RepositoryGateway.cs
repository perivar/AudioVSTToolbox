/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 27.07.2011
 * Time: 12:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using Wave2ZebraSynth.Audio;
using Wave2ZebraSynth.Fingerprinting;
using Wave2ZebraSynth.DataAccess;
using Wave2ZebraSynth.Model;
using Un4seen.Bass.AddOn.Tags;

namespace Wave2ZebraSynth
{
	/// <summary>
	/// Description of RepositoryGateway.
	/// </summary>
	public class RepositoryGateway
	{
        // NB!! ALOT IS TAKEN FROM DuplicateTracks.ViewModelRepositoryGateway
            
        #region Constants

        /// <summary>
        ///   Maximum track length (track's bigger than this value will be discarded)
        /// </summary>
        public const int MAX_TRACK_LENGTH = 60*10; /*10 min - maximal track length*/

        /// <summary>
        ///   Number of milliseconds to process from each song
        /// </summary>
        public const int MILLISECONDS_TO_PROCESS = 15*1000;

        /// <summary>
        ///   Starting processing point
        /// </summary>
        public const int MILLISECONDS_START = 20*1000;

        /// <summary>
        ///   Minimum track length (track's less than this value will be discarded)
        /// </summary>
        public const int MIN_TRACK_LENGTH = (MILLISECONDS_TO_PROCESS + MILLISECONDS_START)/1000 + 1;

        /// <summary>
        ///   Incremental static stride size (1024 samples from the start)
        /// </summary>
        public const int STRIDE_SIZE_INCREMENTAL = 512;

        /// <summary>
        ///   Number of LSH tables
        /// </summary>
        public const int NUMBER_OF_HASH_TABLES = 25;

        /// <summary>
        ///   Number of Min Hash keys per 1 hash function (1 LSH table)
        /// </summary>
        public const int NUMBER_OF_KEYS = 4;

        /// <summary>
        ///   Path to permutations (generated using greedy algorithm)
        /// </summary>
        public const string PATH_TO_PERMUTATIONS = "perms.csv";

        /// <summary>
        ///   Number of threshold votes for a file to be considerate a duplicate
        /// </summary>
        public const int THRESHOLD_VOTES = 8;

        /// <summary>
        /// Value of threshold percentage of fingerprints that needs to be gathered
        /// in order to be considered a possible result
        /// </summary>
        public const double THRESHOLD_PERCENTAGE = 5;

        /// <summary>
        ///   Separator in the .csv files
        /// </summary>
        public const string SEPARATOR = ",";

        /// <summary>
        ///   Number of samples per fingerprint (8192 correspond to 1.48 sec granularity)
        /// </summary>
        public const int SAMPLES_IN_FINGERPRINT = 8192;

        #endregion
        
        #region Read-only components

        /// <summary>
        ///   Creational stride (used in hashing audio objects)
        /// </summary>
        public readonly IStride _createStride;

        /// <summary>
        ///   Permutations provider
        /// </summary>
        public readonly IPermutations _permutations;

        /// <summary>
        ///   Bass proxy used in reading from files
        /// </summary>
        public readonly BassProxy _proxy;

        /// <summary>
        ///   Query stride (used in querying the database)
        /// </summary>
        public readonly IStride _queryStride;

        /// <summary>
        ///   Repository for storage, permutations, algorithm
        /// </summary>
        public readonly Repository _repository;

        /// <summary>
        ///   Storage for hash signatures and tracks
        /// </summary>
        public readonly IStorage _storage;

        #endregion
        
        private bool _aborted;
        
		public RepositoryGateway()
		{
	        _storage = new RamStorage(NUMBER_OF_HASH_TABLES); /*Number of LSH Tables, used for storage purposes*/
            _permutations = new LocalPermutations(PATH_TO_PERMUTATIONS, SEPARATOR); /*Permutations*/
            _repository = new Repository(_storage, _permutations);
            _proxy = new BassProxy(); /*audio proxy used in reading the file*/
            _createStride = new IncrementalStaticStride(STRIDE_SIZE_INCREMENTAL, SAMPLES_IN_FINGERPRINT);
            _queryStride = new IncrementalRandomStride(STRIDE_SIZE_INCREMENTAL, SAMPLES_IN_FINGERPRINT, SAMPLES_IN_FINGERPRINT);		
		}
		
		
        /// <summary>
        ///   Process files synchronously (get fingerprint signatures, hash them into storage)
        /// </summary>
        /// <param name = "files">List of files to be hashed</param>
        /// <param name = "processed">Callback invoked once 1 track is processed</param>
        /// <returns>List of processed tracks</returns>
        public List<Track> ProcessFiles(List<string> files, Action<Track> processed)
        {
            /*preprocessing stage ended, now make sure to do the actual job*/
            var tracks = _repository.ProcessTracks(files, _proxy, _queryStride, _createStride, MIN_TRACK_LENGTH, MAX_TRACK_LENGTH,
                                      MILLISECONDS_TO_PROCESS, MILLISECONDS_START, NUMBER_OF_HASH_TABLES,
                                      NUMBER_OF_KEYS, processed);
            return _aborted ? null : tracks;
        }

        /// <summary>
        ///   Find duplicate files for specific tracks
        /// </summary>
        /// <param name = "tracks">Tracks to search in</param>
        /// <param name="callback">Callback invoked at each processed track</param>
        /// <returns>Set of tracks that are duplicate</returns>
        public HashSet<Track>[] FindDuplicates(List<Track> tracks, Action<Track, int, int> callback)
        {
            return _repository.FindDuplicates(tracks, THRESHOLD_VOTES, THRESHOLD_PERCENTAGE, callback);
        }

        /// <summary>
        ///   Find all duplicate files from the storage
        /// </summary>
        /// <param name="callback">Callback invoked at each processed track</param>
        /// <returns>Set of tracks that are duplicate</returns>
        public HashSet<Track>[] FindAllDuplicates(Action<Track, int, int> callback)
        {
            return _repository.FindDuplicates(_storage.GetAllTracks(), THRESHOLD_VOTES, THRESHOLD_PERCENTAGE, callback);
        }
        
        public void writeImage(String prefix, String filename, float[][] data) {
        	try {
				String filenameToSave = String.Format("C:\\{0}-{1}.bmp", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
				int width = data.Length;
				int wdftSize = 2048;
				int height = wdftSize/2 + 1;
			  	System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			  	
				for (int x = 0; x < data.Length; x++)
				{
	    			for (int y = 0; y < data[x].Length; y++)
				    {
	    				int c1 = (int) (data[x][y] * 255f);
	    				int c2 = Math.Min( 255, Math.Max( 0, c1) );
	    				System.Drawing.Color c = System.Drawing.Color.FromArgb( c2, c2, c2 );
	   					bmp.SetPixel(x, y, c);
			    	}
			  	}
				bmp.Save(filenameToSave);		
        	} catch (Exception ex) {
        		System.Diagnostics.Debug.WriteLine(ex);
        	}
        }
        	
        public void writeImage(String prefix, String filename, bool[] data) {
        	try {
				String filenameToSave = String.Format("C:\\{0}-{1}.bmp", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);

            	int width = 128; /*128*/
            	int height = 32; /*32*/
            
			  	System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			  	
				for (int x = 0; x < width; x++)
				{
	    			for (int y = 0; y < height; y++)
				    {
	    				int i = data[x + y * width] ? 255 : 0;	    				             
	    				System.Drawing.Color c = System.Drawing.Color.FromArgb( i, i, i );
	   					bmp.SetPixel(x, y, c);
			    	}
			  	}
				bmp.Save(filenameToSave);		
        	} catch (Exception ex) {
        		System.Diagnostics.Debug.WriteLine(ex);
        	}
        }	       
		
	}
}
