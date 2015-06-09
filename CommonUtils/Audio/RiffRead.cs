using System;
using System.IO;
using System.Collections.Generic;

namespace CommonUtils
{
	/// <summary>
	/// Originally based by JavaScience Consulting's RiffRead32 Java class
	/// Converted to C# by Per Ivar Nerseth 2011
	/// </summary>
	
	public class RiffRead {

		const int WAVE_FORMAT_UNKNOWN           =  0x0000; // Microsoft Corporation
		const int WAVE_FORMAT_PCM               =  0x0001; // Microsoft Corporation
		const int WAVE_FORMAT_ADPCM             =  0x0002; // Microsoft Corporation
		const int WAVE_FORMAT_IEEE_FLOAT        =  0x0003; // Microsoft Corporation
		const int WAVE_FORMAT_ALAW              =  0x0006; // Microsoft Corporation
		const int WAVE_FORMAT_MULAW             =  0x0007; // Microsoft Corporation
		const int WAVE_FORMAT_DTS_MS            =  0x0008; // Microsoft Corporation
		const int WAVE_FORMAT_WMAS              =  0x000a; // WMA 9 Speech
		const int WAVE_FORMAT_IMA_ADPCM         =  0x0011; // Intel Corporation
		const int WAVE_FORMAT_TRUESPEECH        =  0x0022; // TrueSpeech
		const int WAVE_FORMAT_GSM610            =  0x0031; // Microsoft Corporation
		const int WAVE_FORMAT_MSNAUDIO          =  0x0032; // Microsoft Corporation
		const int WAVE_FORMAT_G726              =  0x0045; // ITU-T standard
		const int WAVE_FORMAT_MPEG              =  0x0050; // Microsoft Corporation
		const int WAVE_FORMAT_MPEGLAYER3        =  0x0055; // ISO/MPEG Layer3 Format Tag
		const int WAVE_FORMAT_DOLBY_AC3_SPDIF   =  0x0092; // Sonic Foundry
		const int WAVE_FORMAT_A52               =  0x2000;
		const int WAVE_FORMAT_DTS               =  0x2001;
		const int WAVE_FORMAT_WMA1              =  0x0160; // WMA version 1
		const int WAVE_FORMAT_WMA2              =  0x0161; // WMA (v2) 7, 8, 9 Series
		const int WAVE_FORMAT_WMAP              =  0x0162; // WMA 9 Professional
		const int WAVE_FORMAT_WMAL              =  0x0163; // WMA 9 Lossless
		const int WAVE_FORMAT_DIVIO_AAC         =  0x4143;
		const int WAVE_FORMAT_AAC               =  0x00FF;
		const int WAVE_FORMAT_FFMPEG_AAC        =  0x706D;

		const int WAVE_FORMAT_DK3               =  0x0061;
		const int WAVE_FORMAT_DK4               =  0x0062;
		const int WAVE_FORMAT_VORBIS            =  0x566f;
		const int WAVE_FORMAT_VORB_1            =  0x674f;
		const int WAVE_FORMAT_VORB_2            =  0x6750;
		const int WAVE_FORMAT_VORB_3            =  0x6751;
		const int WAVE_FORMAT_VORB_1PLUS        =  0x676f;
		const int WAVE_FORMAT_VORB_2PLUS        =  0x6770;
		const int WAVE_FORMAT_VORB_3PLUS        =  0x6771;
		const int WAVE_FORMAT_SPEEX             =  0xa109; // Speex audio
		const int WAVE_FORMAT_EXTENSIBLE        =  0xFFFE; // Microsoft

		static string[] infotype = { "IARL", "IART", "ICMS", "ICMT", "ICOP",
			"ICRD", "ICRP", "IDIM", "IDPI", "IENG", "IGNR", "IKEY",
			"ILGT", "IMED", "INAM", "IPLT", "IPRD", "ISBJ",
			"ISFT", "ISHP", "ISRC", "ISRF", "ITCH",
			"ISMP", "IDIT", "VXNG", "TURL" };

		static string[] infodesc = { "Archival location", "Artist", "Commissioned", "Comments", "Copyright",
			"Creation date", "Cropped", "Dimensions", "Dots per inch", "Engineer", "Genre", "Keywords",
			"Lightness settings", "Medium", "Name of subject", "Palette settings", "Product", "Description",
			"Software package", "Sharpness", "Source", "Source form", "Digitizing technician",
			"SMPTE time code", "Digitization time", "VXNG", "Url" };
		
		private string selectedFile;
		private long fileLength;
		private int nChannels;
		private int nSamplesPerSec;
		private int nAvgBytesPerSec;
		private int nBlockAlign;
		private int wBitsPerSample;
		private int riffDataSize=0;  // size of RIFF data chunk.
		private long dataSize = 0;
		private int sampleCount;
		private int wFormatTag;
		private float[][] soundData;
		
		private Dictionary<string, string> infoChunks = new Dictionary<string, string>();

		public string SelectedFile { get { return selectedFile;  } set { selectedFile = value; } }
		public long FileLength { get { return fileLength;  } set { fileLength = value; } }
		public int Channels { get { return nChannels;  } set { nChannels = value; } }
		public int SampleRate { get { return nSamplesPerSec;  } set { nSamplesPerSec = value; } }
		public int AvgBytesPerSec { get { return nAvgBytesPerSec;  } set { nAvgBytesPerSec = value; } }
		public int BlockAlign { get { return nBlockAlign;  } set { nBlockAlign = value; } }
		public int BitsPerSample { get { return wBitsPerSample;  } set { wBitsPerSample = value; } }
		public int RiffDataSize { get { return riffDataSize;  } set { riffDataSize = value; } }
		public long DataSize { get { return dataSize;  } set { dataSize = value; } }
		public int SampleCount { get { return sampleCount;  } set { sampleCount = value; } }
		public int Format { get { return wFormatTag;  } set { wFormatTag = value; } }
		public float[][] SoundData { get { return soundData;  } set { soundData = value; } }
		public Dictionary<string, string> InfoChunks { get { return infoChunks;  } set { infoChunks = value; } }

		public RiffRead (string value) {
			selectedFile = value;
		}

		public bool Process() {

			int bytespersec = 0;
			int byteread = 0;
			bool isPCM = false;

			var listinfo = new Dictionary<string,string>();
			for (int i=0; i<infotype.Length; i++) {
				listinfo.Add(infotype[i], infodesc[i]);
			}

			var bf = new BinaryFile(selectedFile);
			try {
				var fileInfo = new FileInfo(selectedFile);
				fileLength = fileInfo.Length;

				int chunkSize=0, infochunksize=0, bytecount=0, listbytecount=0;
				string sfield="", infofield="", infodescription="", infodata="";
				
				/*  --------  Get RIFF chunk header --------- */
				sfield = bf.ReadString(4);
				#if DEBUG
				Console.WriteLine("RIFF Header: {0}", sfield);
				#endif
				if (sfield != "RIFF") {
					Console.WriteLine(" ****  Not a valid RIFF file  ****");
					return false;
				}

				// read RIFF data size
				chunkSize  = bf.ReadInt32();
				#if DEBUG
				Console.WriteLine("RIFF data size: {0}", chunkSize);
				#endif
				
				// read form-type (WAVE etc)
				sfield = bf.ReadString(4);
				#if DEBUG
				Console.WriteLine("Form-type: {0}", sfield);
				#endif

				riffDataSize = chunkSize;

				bytecount = 4;  // initialize bytecount to include RIFF form-type bytes.
				while (bytecount < riffDataSize )  {    // check for chunks inside RIFF data area.
					sfield="";
					int firstbyte = bf.ReadByte() ;
					if (firstbyte == 0) {  // if previous data had odd bytecount, was padded by null so skip
						bytecount++;
						continue;
					}
					
					sfield+= (char) firstbyte;  // if we have a new chunk
					for (int i=1; i<=3; i++)  {
						sfield += (char) bf.ReadByte() ;
					}

					chunkSize = 0;
					chunkSize = bf.ReadInt32();
					bytecount += ( 8 + chunkSize );
					#if DEBUG
					Console.WriteLine("{0} ----- data size: {1} bytes", sfield, chunkSize);
					#endif
					
					if (sfield == "data") {
						//get data size to compute duration later.
						dataSize = chunkSize;
					}

					if (sfield == "fmt ") {
						/*
					Offset   Size  Description                  Value
					0x00     4     Chunk ID                     "fmt " (0x666D7420)
					0x04     4     Chunk Data Size              16 + extra format bytes
					0x08     2     Compression code             1 - 65,535
					0x0a     2     Number of channels           1 - 65,535
					0x0c     4     Sample rate                  1 - 0xFFFFFFFF
					0x10     4     Average bytes per second     1 - 0xFFFFFFFF
					0x14     2     Block align                  1 - 65,535
					0x16     2     Significant bits per sample  2 - 65,535
					0x18     2     Extra format bytes           0 - 65,535
					0x1a
					Extra format bytes *
						 */

						// extract info from "format" chunk.
						if (chunkSize < 16) {
							Console.WriteLine(" ****  Not a valid fmt chunk  ****");
							return false;
						}

						// Read compression code, 2 bytes
						wFormatTag = bf.ReadInt16();
						switch (wFormatTag) {
							case WAVE_FORMAT_PCM:
							case WAVE_FORMAT_EXTENSIBLE:
							case WAVE_FORMAT_IEEE_FLOAT:
								isPCM = true;
								break;
						}
						switch (wFormatTag) {
							case WAVE_FORMAT_PCM:
								Console.WriteLine("\twFormatTag:  WAVE_FORMAT_PCM");
								break;
							case WAVE_FORMAT_EXTENSIBLE:
								Console.WriteLine("\twFormatTag:  WAVE_FORMAT_EXTENSIBLE");
								break;
							case WAVE_FORMAT_IEEE_FLOAT:
								Console.WriteLine("\twFormatTag:  WAVE_FORMAT_IEEE_FLOAT");
								break;
							case WAVE_FORMAT_MPEGLAYER3:
								Console.WriteLine("\twFormatTag:  WAVE_FORMAT_MPEGLAYER3");
								break;
							default:
								Console.WriteLine("\twFormatTag:  non-PCM format {0}", wFormatTag);
								break;
						}

						// Read number of channels, 2 bytes
						nChannels = bf.ReadInt16();
						#if DEBUG
						Console.WriteLine("\tnChannels: {0}", nChannels);
						#endif
						
						// Read sample rate, 4 bytes
						nSamplesPerSec = bf.ReadInt32();
						#if DEBUG
						Console.WriteLine("\tnSamplesPerSec: {0}", nSamplesPerSec);
						#endif
						
						// Read average bytes per second, 4 bytes
						nAvgBytesPerSec = bf.ReadInt32();
						bytespersec = nAvgBytesPerSec;
						#if DEBUG
						Console.WriteLine("\tnAvgBytesPerSec: {0}", nAvgBytesPerSec);
						#endif
						
						// Read block align, 2 bytes
						nBlockAlign = bf.ReadInt16();
						#if DEBUG
						Console.WriteLine("\tnBlockAlign: {0}", nBlockAlign);
						#endif
						
						// Read significant bits per sample, 2 bytes
						if (isPCM) {     // if PCM or EXTENSIBLE format
							wBitsPerSample = bf.ReadInt16();
							#if DEBUG
							Console.WriteLine("\twBitsPerSample: {0}", wBitsPerSample);
							#endif
						} else {
							bf.ReadBytes(2);
							wBitsPerSample = 0;
						}

						//skip over any extra bytes in format specific field.
						bf.ReadBytes(chunkSize - 16);

					} else if (sfield == "LIST") {
						String listtype = bf.ReadString(4);
						
						// skip over LIST chunks which don't contain INFO subchunks
						if (listtype != "INFO") {
							bf.ReadBytes(chunkSize - 4);
							continue;
						}

						try {
							listbytecount = 4;
							#if DEBUG
							Console.WriteLine("------- INFO chunks: {0} bytes -------", chunkSize);
							#endif
							// iterate over all entries in LIST chunk
							while (listbytecount < chunkSize) {
								infofield = "";
								infodescription = "";
								infodata = "";

								firstbyte = bf.ReadByte();
								// if previous data had odd bytecount, was padded by null so skip
								if (firstbyte == 0) {
									listbytecount++;
									continue;
								}
								
								// if firstbyte is not an alpha char, read one more byte
								if (!Char.IsLetterOrDigit( (char) firstbyte )) {
									firstbyte = bf.ReadByte();
									listbytecount++;
								}
								
								// if we have a new chunk
								infofield += (char) firstbyte;
								for (int i=1; i<=3; i++)  { //get the remaining part of info chunk name ID
									infofield += (char) bf.ReadByte() ;
								}
								
								// get the info chunk data byte size
								infochunksize = bf.ReadInt32();
								listbytecount += ( 8 + infochunksize );
								//Console.WriteLine("infofield: {0}, listbytecount: {1}, infochunksize: {2}", infofield, listbytecount, infochunksize);

								if (listbytecount > chunkSize) {
									bf.SetPosition(bf.GetPosition() - 8);
									break;
								}
								
								// get the info chunk data
								for (int i=0; i < infochunksize; i++) {
									byteread = bf.ReadByte();
									if (byteread == 0) { // if null byte in string, ignore it
										continue;
									}
									infodata += (char) byteread;
								}
								
								int unknownCount = 1;
								try {
									infodescription = (string) listinfo[infofield];
								} catch (KeyNotFoundException) {}
								if (infodescription != null) {
									#if DEBUG
									Console.WriteLine("{0} ({1}) = {2}", infofield, infodescription, infodata);
									#endif
									InfoChunks.Add(infodescription, infodata);
								} else {
									#if DEBUG
									Console.WriteLine("unknown: {0} = {1}", infofield, infodata);
									#endif
									InfoChunks.Add(String.Format("unknown{0}", unknownCount), infodata);
									unknownCount++;
								}
							} //------- end iteration over LIST chunk ------------
							
						} catch (Exception) {;
							// don't care about these?
							//Console.WriteLine("Error: {0}", e.ToString());
						}
						
						#if DEBUG
						Console.WriteLine("------- end INFO chunks -------");
						#endif
						
					} else {
						if (sfield.Equals("data")) {
							sampleCount = (int) dataSize / (wBitsPerSample / 8) / nChannels;
							
							soundData = new float[nChannels][];
							for (int ic = 0; ic < nChannels; ic++) {
								soundData[ic] = new float[sampleCount];
							}

							//********Data loading********
							if (BitsPerSample == 8) {
								Read8Bit(bf, soundData, sampleCount, nChannels);
							}
							if (BitsPerSample == 16) {
								Read16Bit(bf, soundData, sampleCount, nChannels);
							}
							if (BitsPerSample == 32) {
								if (wFormatTag == WAVE_FORMAT_PCM) {
									Read32Bit(bf, soundData, sampleCount, nChannels);
								} else if (wFormatTag == WAVE_FORMAT_IEEE_FLOAT) {
									Read32BitFloat(bf, soundData, sampleCount, nChannels);
								}
							}
						} else {
							// if NOT the fmt or LIST chunks skip data
							bf.ReadBytes(chunkSize);
						}
					}
				}  // end while.

				//-----------  End of chunk iteration -------------
				if(isPCM && dataSize > 0) {   // compute duration of PCM wave file
					long waveduration = 1000L * dataSize / bytespersec; // in msec units
					long mins = waveduration / 60000;    // integer minutes
					double secs = 0.001 * (waveduration % 60000);    //double secs.
					#if DEBUG
					Console.WriteLine("wav duration:  {0} mins  {1} sec", mins, secs);
					#endif
				}

				#if DEBUG
				Console.WriteLine("Final RIFF data bytecount: {0}", bytecount);
				#endif
				if ( ( 8 + bytecount) != (int) fileLength)  {
					Console.WriteLine("!!!!!!! Problem with file structure  !!!!!!!!!");
					return false;
				} else {
					#if DEBUG
					Console.WriteLine("File chunk structure consistent with valid RIFF") ;
					#endif
				}

				return true;
			} catch (Exception e) {
				Console.WriteLine("Error: {0}", e.ToString()) ;
				return false;
			} finally {
				// close all streams.
				bf.Close();
			}
		}

		public static void Read8Bit(BinaryFile wavfile, float[][] sound, int samplecount, int channels)
		{
			var i = new int();
			var ic = new int();
			byte @byte = new byte();

			#if DEBUG
			Console.Write("Read8Bit...\n");
			#endif

			for (i = 0; i<samplecount; i++) {
				for (ic = 0;ic<channels;ic++)
				{
					@byte = wavfile.ReadByte();
					sound[ic][i] = (float) @byte/128.0f - 1.0f;
				}
			}
		}

		public static void Read16Bit(BinaryFile wavfile, float[][] sound, int samplecount, int channels)
		{
			var i = new int();
			var ic = new int();

			#if DEBUG
			Console.Write("Read16Bit...\n");
			#endif

			for (i = 0; i<samplecount; i++) {
				for (ic = 0; ic<channels; ic++) {
					float f = (float) wavfile.ReadInt16();
					f = f / 32768.0f;
					sound[ic][i] = f;
				}
			}
		}

		public static void Read32Bit(BinaryFile wavfile, float[][] sound, int samplecount, int channels)
		{
			var i = new int();
			var ic = new int();

			#if DEBUG
			Console.Write("Read32Bit...\n");
			#endif

			for (i = 0;i<samplecount;i++) {
				for (ic = 0;ic<channels;ic++)
				{
					float f = (float) wavfile.ReadInt32();
					f = f / 2147483648.0f;
					sound[ic][i] = f;
				}
			}
		}

		public static void Read32BitFloat(BinaryFile wavfile, float[][] sound, int samplecount, int channels)
		{
			var i = new int();
			var ic = new int();

			#if DEBUG
			Console.Write("Read32BitFloat...\n");
			#endif

			for (i = 0;i<samplecount;i++) {
				for (ic = 0;ic<channels;ic++)
				{
					float f = (float) wavfile.ReadSingle();
					sound[ic][i] = f;
				}
			}
		}
		
	}
}