// Based on Sound Fingerprinting framework
// https://code.google.com/p/soundfingerprinting/
// Code license: GNU General Public License v2
// ciumac.sergiu@gmail.com

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using Wave2Zebra2Preset.Fingerprinting;
using Wave2Zebra2Preset.DataAccess;
using Wave2Zebra2Preset.Model;

using CommonUtils;
using CommonUtils.Audio.Bass;

using Un4seen.Bass.AddOn.Tags;

namespace Wave2Zebra2Preset
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
		
		private bool _aborted = false;
		
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
		
		#region Waveform
		public void drawWaveform( string prefix, string filename, float[] samples, bool startDrawingAtMiddle )
		{
			try {
				//-----------------------
				String filenameToSave = String.Format("C:\\{0}-{1}.png", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
				
				int numberOfSamples = samples.Length;
				int width = 1200;
				int height = 300;
				
				// horizontalScaleFactor between 0.25 and 0.5 is quite good
				double horizontalScaleFactor = (double) width / numberOfSamples;
				double verticalScaleFactor = 150;
				
				Bitmap png = new Bitmap( width, height, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				Pen linePen = new Pen(Color.DarkGray, 2);
				Pen wavePen = new Pen(Color.DarkBlue, 1);
				Pen boxPen = new Pen(Color.Black, 2);
				
				// Draw a rectangular box marking the boundaries of the graph
				Rectangle rect = new Rectangle(0, 0, width, height);
				g.DrawRectangle(boxPen, rect);

				// Mark the origin to start drawing at 0,0:
				int oldX = 0;
				int oldY = 0;
				if (startDrawingAtMiddle) {
					oldY = (int) (height / 2);
				} else {
					oldY = (int) (height);
				}
				int xIndex = 0;
				
				// Start by drawing the center line at 0:
				g.DrawLine(linePen, oldX, oldY, width, oldY);
				
				// Now, you need to figure out the incremental jump between samples to adjust for the scale factor. This works out to be:
				int increment = (int) (numberOfSamples / (numberOfSamples * horizontalScaleFactor));
				if (increment == 0) increment = 1;
				
				// The following code grabs the increment and paints a line from the origin to the first sample:
				int t = 0;
				for (t = 0; t < increment; t += increment) {
					g.DrawLine(wavePen, oldX, oldY, xIndex, oldY);
					xIndex++;
					oldX = xIndex;
				}

				// Finish up by iterating through the audio and drawing lines to the scaled samples:
				for (; t < numberOfSamples; t += increment) {
					double scaleFactor = verticalScaleFactor;
					double scaledSample = samples[t] * scaleFactor;
					int y = (int) ((height / 2) - (scaledSample));
					g.DrawLine(wavePen, oldX, oldY, xIndex, y);
					
					xIndex++;
					oldX = xIndex;
					oldY = y;
				}
				
				png.Save(filenameToSave);
				g.Dispose();
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
		
		/*
		 * Converted from a Java Class forom jMusic API version 1.4, February 2003.
		 * 
		 * Copyright (C) 2000 Andrew Sorensen & Andrew Brown
		 * 
		 * This program is free software; you can redistribute it and/or modify
		 * it under the terms of the GNU General Public License as published by
		 * the Free Software Foundation; either version 2 of the License, or any
		 * later version.
		 * 
		 * This program is distributed in the hope that it will be useful, but
		 * WITHOUT ANY WARRANTY; without even the implied warranty of
		 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
		 * GNU General Public License for more details.
		 * 
		 * You should have received a copy of the GNU General Public License
		 * along with this program; if not, write to the Free Software
		 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
		 * 
		 */
		public void drawWaveform( string prefix, string filename, float[] data )
		{
			try {
				//-----------------------
				String filenameToSave = String.Format("C:\\{0}-{1}.png", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
				
				int numberOfSamples = data.Length;
				int width = 1200;
				int height = 200;
				int resolution = 2; //125 // low resolution (2+) means to zoom into the waveform
				int amplitude = 1;

				float max = 0.0f;
				float min = 0.0f;

				float drawMax, drawMin, currData;

				int h2 = height/2 - 1;
				int position = 0;
				int sampleStart = 0;
				
				Bitmap png = new Bitmap( width, height, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				Pen linePen = new Pen(Color.DarkGray, 2);
				Pen wavePen = new Pen(Color.DarkBlue, 1);
				Pen boxPen = new Pen(Color.Black, 2);
				
				// Draw a rectangular box marking the boundaries of the graph
				Rectangle rect = new Rectangle(0, 0, width, height);
				g.DrawRectangle(boxPen, rect);
				
				// mid line
				g.DrawLine(linePen, 0, h2, width, h2);

				// draw wave
				int pixCount = Math.Min(data.Length - resolution, width * resolution);
				if (resolution == 1) {
					for (int i = sampleStart; i < sampleStart + pixCount; i += resolution) {
						currData = data[i];
						g.DrawLine(linePen, position, (int)(h2 - currData * h2 * amplitude), position, (int)(h2 - currData * h2 * amplitude));
						position++;
					}
				} else {
					for (int i = sampleStart; i < sampleStart + pixCount; i += resolution) {
						if( i < numberOfSamples ) {
							currData = data[i];
							
							// max and min
							max = 0.0f;
							min = 0.0f;
							for( int j=0; j< resolution; j++) {
								if (data[i+j] > max) max = data[i+j];
								if (data[i+j] < min) min = data[i+j];
							}
							
							// highest and lowest curve values
							if (resolution > 8) {
								drawMax = Math.Max(currData, data[i+resolution]);
								drawMin = Math.Min(currData, data[i+resolution]);
								
								if (max > 0.0f) g.DrawLine(wavePen, position, (int)(h2 - drawMax * h2 * amplitude), position, (int)(h2 - max * h2 * amplitude));
								
								if (min < 0.0f) g.DrawLine(wavePen, position, (int)(h2 - drawMin * h2 * amplitude), position, (int)(h2 - min * h2 * amplitude));
							}
							
							// draw wave
							g.DrawLine(wavePen, position++, (int)(h2 - currData * h2 * amplitude), position, (int)(h2 - data[i+resolution] * h2 * amplitude));
						}
					}
				}
				
				// base line
				png.Save(filenameToSave);
				g.Dispose();
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
		
		public void drawWaveform2( string prefix, string filename, float[] data, bool sampleBitMono) {
			try {
				String filenameToSave = String.Format("C:\\{0}-{1}.png", prefix, System.IO.Path.GetFileNameWithoutExtension(filename));
				System.Diagnostics.Debug.WriteLine("Writing " + filenameToSave);
				
				int width = 1200;
				int height = 200;
				int numberOfSamples = data.Length;
				
				Bitmap png = new Bitmap( width, height, PixelFormat.Format32bppArgb );
				Graphics g = Graphics.FromImage(png);
				
				Pen pen = new Pen(Color.LightGreen, 1);
				
				float X_Slot = (float) (0.8 * width / 10);
				float Y_Slot = (float) (0.8 * height / 10);
				float X = 0;
				float Y = 0;
				float X_0 = (float) (width * 0.1);
				float X_1 = (float) (width * 0.9);
				float Y_0 = (float) (height * 0.1);
				float Y_1 = (float) (height * 0.9);
				float X_Unit = 1;
				float Y_Unit = 0;
				
				if (sampleBitMono) {
					Y_Unit = (float) (0.8 * height / 256);
					for (int i = 0; i < data.Length; i++) {
						data[i] = data[i] + 128;
					}
				} else {
					Y_Unit = (float) (0.8 * height / 65536);
					for (int i = 0; i < data.Length; i++) {
						data[i] = data[i] + 32768;
					}
				}

				g.Clear(Color.LightGray);
				g.DrawLine(Pens.Black, X_0, Y_0, X_0, Y_1);
				g.DrawLine(Pens.Black, X_0, Y_1, X_1, Y_1);
				
				for (int i = 1; i < 10; i++) {
					g.DrawLine(Pens.DarkGray, X_0, Y_0 + (i * Y_Slot), X_1, Y_0 + (i * Y_Slot));
					g.DrawLine(Pens.DarkGray, X_0 + (i * X_Slot), Y_0, X_0 + (i * X_Slot), Y_1);
				}

				pen.Width = 2.0F;
				g.DrawLine(pen, X_0, Y_0 + (5 * Y_Slot), X_1, Y_0 + (5 * Y_Slot));
				g.DrawLine(pen, X_0 + (5 * X_Slot), Y_0, X_0 + (5 * X_Slot), Y_1);
				
				X_Unit = (float) (0.8 * width / data.Length);
				
				PointF[] pointArray = new PointF[data.Length];
				for (int i = 0; i < data.Length; i++) {
					X = X_0 + (i * X_Unit);
					Y = Y_1 - (data[i] * Y_Unit);
					pointArray[i] = new PointF(X, Y);
				}
				
				g.DrawLines(Pens.DarkBlue, pointArray);
				g.Flush();

				// base line
				png.Save(filenameToSave);
				g.Dispose();
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
		
		#endregion
		
		#region spectrum
		// see https://code.google.com/p/jstk/source/browse/trunk/jstk/src/de/fau/cs/jstk/?r=154#jstk%2Fvc
		public void drawSpectrogram1(String prefix, String filename, float[][] data) {
			VB6Spectrogram vb6Spectrogram = new VB6Spectrogram();
			vb6Spectrogram.ComputeColorPalette();
			
			double numberOfSamplesX = data.Length;
			double numberOfSamplesY = data[0].Length;

			int width = (int) numberOfSamplesX;
			int height = (int) numberOfSamplesY;

			String filenameToSave = String.Format("C:\\{0}-{1}x{2}-{3}.png", prefix, width, height, System.IO.Path.GetFileNameWithoutExtension(filename));
			
			System.Console.Out.WriteLine("Writing " + filenameToSave);
			
			int maxYIndex = height - 1;
			double horizontalScaleFactor = (double) width / numberOfSamplesX;
			double verticalScaleFactor = (double) height/ numberOfSamplesY;
			
			// Now, you need to figure out the incremental jump between samples to adjust for the scale factor. This works out to be:
			int incrementX = (int) (numberOfSamplesX / (numberOfSamplesX * horizontalScaleFactor));
			if (incrementX == 0) incrementX = 1;
			
			int incrementY = (int) (numberOfSamplesY / (numberOfSamplesY * verticalScaleFactor));
			if (incrementY == 0) incrementY = 1;
			
			// prepare the data:
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal)
						maxVal = data[x][y];
					if (data[x][y] < minVal)
						minVal = data[x][y];
				}
			}

			double minIntensity = Math.Abs(minVal);
			double maxIntensity = maxVal + minIntensity;
			
			/* Create the image for displaying the data.
			 */
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);

			Rectangle rect = new Rectangle(0, 0, width, height);
			g.FillRectangle(Brushes.White, rect);
			
			/* Set scaleFactor so that the maximum value, after removing
			 * the offset, will be 0xff.
			 */
			float scaleFactor = (float)(0xff / maxIntensity);
			
			for (int i = 0; i < numberOfSamplesX; i += incrementX)
			{
				for (int j = 0; j < numberOfSamplesY; j += incrementY)
				{
					int x = (int) MathUtils.RoundDown(i*horizontalScaleFactor,0);
					int y = (int) MathUtils.RoundDown(j*verticalScaleFactor,0);

					float f = data[i][j];
					double d = (f + minIntensity) * scaleFactor;
					
					Color c = Color.White;
					int RangedB = 100;
					int RangePaletteIndex = 256;
					double indexDouble = VB6Spectrogram.MapToPixelIndex(f, RangedB, RangePaletteIndex);
					byte vb6Index = (byte) indexDouble;
					c = vb6Spectrogram.LevelPaletteDictionary[vb6Index];
					png.SetPixel(x, maxYIndex - y, c);
				}
			}
			
			png.Save(filenameToSave);
			g.Dispose();
		}

		public void drawSpectrogram2(String prefix, String filename, float[][] data, double sampleRate, int numberOfSamples, double fftWindowsSize) {
			float minDb = -100.0f;
			float maxDb = 0.0f;
			
			float numberOfSamplesX = data.Length;
			float numberOfSamplesY = data[0].Length;
			
			// set width and height
			int width = (int) numberOfSamplesX;
			int height = (int) numberOfSamplesY;

			String filenameToSave = String.Format("C:\\{0}-{1}x{2}-{3}.png", prefix, width, height, System.IO.Path.GetFileNameWithoutExtension(filename));
			System.Console.Out.WriteLine("Writing " + filenameToSave);
			
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			Pen pen = new Pen(Color.Black, 1);

			// prepare the data
			// retrieve the highest and lowest value
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal) {
						maxVal = data[x][y];
					}
					
					if (data[x][y] < minVal) {
						minVal = data[x][y];
					}
				}
			}

			float maxValdB = MathUtils.AmplitudeToDecibel((float)maxVal, minDb, maxDb);
			float minValdB = MathUtils.AmplitudeToDecibel((float)minVal, minDb, maxDb);
			
			Axis.drawAxis(Axis.X_AXIS, 10, 10, 0, (float)MathUtils.ConvertToTime(sampleRate, numberOfSamples), 50, width-50, 50, false, height, g);
			Axis.drawAxis(Axis.Y_AXIS, 100, 10, 20, (float)(sampleRate/2), 50, height-50, 50, true, height, g);
			
			int xCoord = 1;
			int oldX = 1;
			for(int x = 0; x < numberOfSamplesX; x++)
			{
				int oldY = 1;
				for(int y = 0; y < numberOfSamplesY; y++)
				{
					int x1 = Axis.plotValue(x+1, 1, numberOfSamplesX+1, 50, width-50, false, height);
					int y1 = Axis.plotValue(y+1, 1, numberOfSamplesY+1, 50, height-50, true, height);

					/*
					float amplitude = data[x][y];
					float dB = MathUtils.ConvertAmplitudeToDB(amplitude, minDb, maxDb);
					int color = (int) MathUtils.ConvertAndMainainRatio(dB, minValdB, maxValdB, 0, 256);
					Color c = VB6Spectrogram.PaletteValueColor(color, 256);
					*/
					float amplitude = data[x][y];
					Color colorbw = Color.Black;
					if (amplitude > 0) {
						float dB = MathUtils.AmplitudeToDecibel(amplitude, minDb, maxDb);
						int colorval = (int) MathUtils.ConvertAndMainainRatio(dB, minDb, maxDb, 0, 255); // 255 is full brightness, and good for REW colors (for SOX 220 is good)
						colorbw = Color.FromArgb(colorval, colorval, colorval);
					}
					
					
					if (x1 > 0 && x1 < width && y1 > 0 && y1 < height) {
						pen.Color = colorbw;
						xCoord = x1+50;
						g.DrawLine(pen, xCoord, height - oldY - 50, xCoord, height - y1 - 50);
						oldX = x1;
						oldY = y1;
					}
					
				}
			}
			
			png = ColorUtils.Colorize(png, 255, ColorUtils.ColorPaletteType.REW);
			png.Save(filenameToSave);
			g.Dispose();
		}

		public void drawSpectrogram3(String prefix, String filename, float[][] data) {

			float minDb = -65.0f;
			float maxDb = 0.0f;

			double numberOfSamplesX = data.Length;
			double numberOfSamplesY = data[0].Length;

			// set width and height
			int width = (int) numberOfSamplesX;
			int height = (int) numberOfSamplesY;

			String filenameToSave = String.Format("C:\\{0}-{1}x{2}-{3}.png", prefix, width, height, System.IO.Path.GetFileNameWithoutExtension(filename));
			System.Console.Out.WriteLine("Writing " + filenameToSave);
			
			double horizontalScaleFactor = (double) width / numberOfSamplesX;
			double verticalScaleFactor = (double) height/ numberOfSamplesY;
			
			int maxYIndex = height - 1;
			
			// Now, you need to figure out the incremental jump between samples to adjust for the scale factor. This works out to be:
			int incrementX = (int) (numberOfSamplesX / (numberOfSamplesX * horizontalScaleFactor));
			if (incrementX == 0) incrementX = 1;
			
			int incrementY = (int) (numberOfSamplesY / (numberOfSamplesY * verticalScaleFactor));
			if (incrementY == 0) incrementY = 1;
			
			// prepare the data
			// retrieve the highest and lowest value
			double maxVal = double.MinValue;
			double minVal = double.MaxValue;
			
			for(int x = 0; x < data.Length; x++)
			{
				for(int y = 0; y < data[x].Length; y++)
				{
					if (data[x][y] > maxVal) {
						maxVal = data[x][y];
					}
					
					if (data[x][y] < minVal) {
						minVal = data[x][y];
					}
				}
			}

			float maxValdB = MathUtils.AmplitudeToDecibel((float)maxVal, minDb, maxDb);
			float minValdB = MathUtils.AmplitudeToDecibel((float)minVal, minDb, maxDb);

			double minIntensity = Math.Abs(minVal);
			double maxIntensity = maxVal + minIntensity;
			
			System.Console.Out.WriteLine("min value: {0}", minVal);
			System.Console.Out.WriteLine("max value: {0}", maxVal);
			System.Console.Out.WriteLine("min value: {0} dB", minValdB);
			System.Console.Out.WriteLine("max value: {0} dB", maxValdB);
			System.Console.Out.WriteLine("min intensity: {0}", minIntensity);
			System.Console.Out.WriteLine("max intensity: {0}", maxIntensity);
			
			// Create the image for displaying the data.
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);

			Rectangle rect = new Rectangle(0, 0, width, height);
			g.FillRectangle(Brushes.White, rect);
			
			for (int i = 0; i < numberOfSamplesX; i += incrementX)
			{
				for (int j = 0; j < numberOfSamplesY; j += incrementY)
				{
					int x = (int) MathUtils.RoundDown(i*horizontalScaleFactor,0);
					int y = (int) MathUtils.RoundDown(j*verticalScaleFactor,0);

					float amplitude = data[i][j];
					float dB = MathUtils.AmplitudeToDecibel(amplitude, minDb, maxDb);
					
					int color = (int) MathUtils.ConvertAndMainainRatio(dB, minValdB, maxValdB, 0, 256);
					//Color c = VB6Spectrogram.PaletteValueColor(color, 256);
					Color c = VB6Spectrogram.GreyPaletteValueColor(color, 256);
					png.SetPixel(x, maxYIndex - y, c);
				}
			}
			
			png = ColorUtils.Colorize(png, 255, ColorUtils.ColorPaletteType.SOX);
			png.Save(filenameToSave);
			g.Dispose();
		}
		
		public void drawSpectrogram4(String prefix, String filename, float[][] data) {

			float amplitude;

			double numberOfSamplesX = data.Length;
			double numberOfSamplesY = data[0].Length;

			// set width and height
			int width = (int) numberOfSamplesX;
			int height = (int) numberOfSamplesY;

			String filenameToSave = String.Format("C:\\{0}-{1}x{2}-{3}.png", prefix, width, height, System.IO.Path.GetFileNameWithoutExtension(filename));
			System.Console.Out.WriteLine("Writing " + filenameToSave);
			
			double horizontalScaleFactor = (double) width / numberOfSamplesX;
			double verticalScaleFactor = (double) height/ numberOfSamplesY;
			
			// Now, you need to figure out the incremental jump between samples to adjust for the scale factor. This works out to be:
			int incrementX = (int) (numberOfSamplesX / (numberOfSamplesX * horizontalScaleFactor));
			if (incrementX == 0) incrementX = 1;

			int incrementY = (int) (numberOfSamplesY / (numberOfSamplesY * verticalScaleFactor));
			if (incrementY == 0) incrementY = 1;
			
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);

			Rectangle rect = new Rectangle(0, 0, width, height);
			g.FillRectangle(Brushes.White, rect);

			int x = 0;
			int y = 0;
			for (x = 0; x < numberOfSamplesX; x += incrementX)
			{
				for (y = 0; y < numberOfSamplesY; y += incrementY)
				{
					int xCoord = (int) MathUtils.RoundDown(x*horizontalScaleFactor,0);
					int yCoord = (int) MathUtils.RoundDown(y*verticalScaleFactor,0);

					amplitude = data[x][y];
					
					// Convert float to dB
					float MinDb = -60.0f;
					float MaxDb = 0.0f;

					float db = 20 * (float) Math.Log10( (float) amplitude);
					if (db < MinDb) db = MinDb;
					if (db > MaxDb) db = MaxDb;
					float percentage = (db - MinDb) / (MaxDb - MinDb);
					
					Color c = AColor.GetColorGradient(percentage);
					png.SetPixel(xCoord, yCoord, c);
				}
			}
			png.Save(filenameToSave);
			g.Dispose();
		}
		#endregion
		
	}
}
