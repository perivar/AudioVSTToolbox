// Based on Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

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

        public void writeImage(String prefix, String filename, float[] data) {
        	try {
				String filenameToSave = String.Format("C:\\{0}-{1}.bmp", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);

				int width = data.Length;
				int wdftSize = 2048;
				int height = wdftSize/2 + 1;
			  	System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			  	
				for (int x = 0; x < data.Length; x++)
				{
	 		     	// Compute horizontal position
	                //x = LEFT + FREQTOPIXEL*(freq[i]-MIN_FREQ);
	 
	                // Compute vertical position of point
	                // and clip at top/bottom.
	                //y = BOTTOM - DBTOPIXEL*(mag[i]-MIN_DB);

    				int c1 = (int) (data[x] * 255f);
    				int c2 = Math.Min( 255, Math.Max( 0, c1) );
    				System.Drawing.Color c = System.Drawing.Color.FromArgb( c2, c2, c2 );
   					bmp.SetPixel(x, 255, c);
			  	}
				bmp.Save(filenameToSave);		
        	} catch (Exception ex) {
        		System.Diagnostics.Debug.WriteLine(ex);
        	}
        }
        
		/**
         * Draw a graph of the spectrum
	     *
	     * Released under the MIT License
	     *
	     * Copyright (c) 2010 Gerald T. Beauregard
	     *
	     * Permission is hereby granted, free of charge, to any person obtaining a copy
	     * of this software and associated documentation files (the "Software"), to
	     * deal in the Software without restriction, including without limitation the
	     * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
	     * sell copies of the Software, and to permit persons to whom the Software is
	     * furnished to do so, subject to the following conditions:
	     *
	     * The above copyright notice and this permission notice shall be included in
	     * all copies or substantial portions of the Software.
	     *
	     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	     * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
	     * IN THE SOFTWARE.
	     */        
        public void drawSpectrum( string prefix, string filename, float[] mag, float[] freq)
        {
            // Basic constants
            int MIN_FREQ = 0;                 // Minimum frequency (Hz) on horizontal axis.
            int MAX_FREQ = 4000;           	// Maximum frequency (Hz) on horizontal axis.
            int FREQ_STEP = 500;             	// Interval between ticks (Hz) on horizontal axis.
            float MAX_DB = -0.0f;           	// Maximum dB magnitude on vertical axis.
            float MIN_DB = -60.0f;            // Minimum dB magnitude on vertical axis.
            int DB_STEP = 10;                 // Interval between ticks (dB) on vertical axis.
            int TOP = 50;                     // Top of graph
            int LEFT = 60;                    // Left edge of graph
            int HEIGHT = 300;                 // Height of graph
            int WIDTH = 500;                  // Width of graph
            int TICK_LEN = 10;                // Length of tick in pixels
            String LABEL_X = "Frequency (Hz)";    // Label for X axis
           	String LABEL_Y = "dB";                // Label for Y axis
 
            // Derived constants
            int BOTTOM = TOP+HEIGHT;                   		// Bottom of graph
            float DBTOPIXEL = HEIGHT/(MAX_DB-MIN_DB);    	// Pixels/tick
            float FREQTOPIXEL = (float) WIDTH/(MAX_FREQ-MIN_FREQ);	// Pixels/Hz
 	
        	try {
	            //-----------------------     
				String filenameToSave = String.Format("C:\\{0}-{1}.bmp", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
	            
				Bitmap bmp = new Bitmap( WIDTH+150, HEIGHT+150, PixelFormat.Format32bppArgb );
	    		Color c = Color.FromArgb( 67, 133, 54 );
	    		Graphics newGraphics = Graphics.FromImage(bmp);            
	    		    		
	            int numPoints = mag.Length;
	            if ( mag.Length != freq.Length )
	                System.Diagnostics.Debug.WriteLine( "mag.length != freq.length" );
	  
	            // Draw a rectangular box marking the boundaries of the graph
	    		Pen blackPen = new Pen(Color.Black, 2);
	
	    		// Create rectangle.
	    		Rectangle rect = new Rectangle(LEFT, TOP, WIDTH, HEIGHT);
	    		newGraphics.DrawRectangle(blackPen, rect);
	 
	            //--------------------------------------------
	 
	            // Tick marks on the vertical axis
	            float y = 0;
	            float x = 0;
	            bool m_tickTextAdded = false;
	            for ( float dBTick = MIN_DB; dBTick <= MAX_DB; dBTick += DB_STEP )
	            {
	                y = BOTTOM - DBTOPIXEL*(dBTick-MIN_DB);
	                newGraphics.DrawLine(blackPen, LEFT-TICK_LEN/2, y, LEFT+TICK_LEN/2, y);
	                if ( m_tickTextAdded == false )
	                {
	                    // Numbers on the tick marks
	    				Font drawFont = new Font("Arial", 12);
	    				SolidBrush drawBrush = new SolidBrush(Color.Black);
	    				newGraphics.DrawString("" + dBTick, drawFont, drawBrush, LEFT-20, y - drawFont.GetHeight(newGraphics)/2);
	                }
	            }
	 
	            // Label for vertical axis
	            if ( m_tickTextAdded == false )
	            {
					Font drawFont = new Font("Arial", 16);
					SolidBrush drawBrush = new SolidBrush(Color.Black);
					newGraphics.DrawString(LABEL_Y, drawFont, drawBrush, (float) LEFT-50, (float) TOP + HEIGHT/2 - drawFont.GetHeight(newGraphics)/2);
	            }
	 
	            //--------------------------------------------
	 
	            // Tick marks on the horizontal axis
	            for ( int f = MIN_FREQ; f <= MAX_FREQ; f += FREQ_STEP )
	            {
	                x = LEFT + FREQTOPIXEL*(f-MIN_FREQ);
	                newGraphics.DrawLine(blackPen, x, BOTTOM - TICK_LEN/2, x, BOTTOM + TICK_LEN/2);
	                if ( m_tickTextAdded == false )
	                {
	                    // Numbers on the tick marks
	    				Font drawFont = new Font("Arial", 12);
	    				SolidBrush drawBrush = new SolidBrush(Color.Black);
	    				newGraphics.DrawString("" + f, drawFont, drawBrush, x, BOTTOM+7);                    
	                }
	            }
	 
	            // Label for horizontal axis
	            if ( m_tickTextAdded == false )
	            {
					Font drawFont = new Font("Arial", 16);
					SolidBrush drawBrush = new SolidBrush(Color.Black);
					newGraphics.DrawString(LABEL_X, drawFont, drawBrush, LEFT+WIDTH/2, BOTTOM+30);                    
	            }
	 
	            m_tickTextAdded = true;
	 
	            // -------------------------------------------------
	            // The line in the graph
	 
	            // Ignore points that are too far to the left
	            for ( int i = 0; i < numPoints && freq[i] < MIN_FREQ; i++ )
	            {
	            }
	 
	            // For all remaining points within range of x-axis
	            for ( int i = 0; i < numPoints && freq[i] <= MAX_FREQ; i++ )
	            {
	                // Compute horizontal position
	                x = LEFT + FREQTOPIXEL*(freq[i]-MIN_FREQ);
	 					
	                // Compute vertical position of point
	                // and clip at top/bottom.
	                y = BOTTOM - DBTOPIXEL*(mag[i]-MIN_DB);
	                if ( y < TOP )
	                    y = TOP;
	                else if ( y > BOTTOM )
	                    y = BOTTOM;
	 
	                newGraphics.DrawEllipse(blackPen, x, y, 5, 5);
	            }
	            
				bmp.Save(filenameToSave);		
    			newGraphics.Dispose();
        	} catch (Exception ex) {
        		System.Diagnostics.Debug.WriteLine(ex);
        	}
        }

        public void drawSpectrum2( string prefix, string filename, float[] spectrum )
        {
        	try {
	            //-----------------------     
				String filenameToSave = String.Format("C:\\{0}-{1}.bmp", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
	            
				Bitmap bmp = new Bitmap( spectrum.Length, 400, PixelFormat.Format32bppArgb );
	    		Graphics newGraphics = Graphics.FromImage(bmp);   
    			Pen bluePen = new Pen(Color.LightBlue, 1);
	
	    		for (int i=0; i < spectrum.Length; i++) {
    				//  rect(i+10,390,1,myfft.spectrum[i]*-400);
    				newGraphics.DrawRectangle(bluePen, i+10, 390, 1, spectrum[i]*-400);
  				}
	    		
				bmp.Save(filenameToSave);		
    			newGraphics.Dispose();
        	} catch (Exception ex) {
        		System.Diagnostics.Debug.WriteLine(ex);
        	}
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
