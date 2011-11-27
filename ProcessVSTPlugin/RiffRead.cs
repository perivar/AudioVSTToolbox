using System;
using System.IO;
using System.Collections.Generic;

/*  Originally based by JavaScience Consulting's RiffRead32 Java class
	Converted to C# by Per Ivar Nerseth 2011 */
public class RiffRead {

	static int WAVE_FORMAT_UNKNOWN           =  0x0000; /* Microsoft Corporation */
	static int WAVE_FORMAT_PCM               =  0x0001; /* Microsoft Corporation */
	static int WAVE_FORMAT_ADPCM             =  0x0002; /* Microsoft Corporation */
	static int WAVE_FORMAT_IEEE_FLOAT        =  0x0003; /* Microsoft Corporation */
	static int WAVE_FORMAT_ALAW              =  0x0006; /* Microsoft Corporation */
	static int WAVE_FORMAT_MULAW             =  0x0007; /* Microsoft Corporation */
	static int WAVE_FORMAT_DTS_MS            =  0x0008; /* Microsoft Corporation */
	static int WAVE_FORMAT_WMAS              =  0x000a; /* WMA 9 Speech */
	static int WAVE_FORMAT_IMA_ADPCM         =  0x0011; /* Intel Corporation */
	static int WAVE_FORMAT_TRUESPEECH        =  0x0022; /* TrueSpeech */
	static int WAVE_FORMAT_GSM610            =  0x0031; /* Microsoft Corporation */
	static int WAVE_FORMAT_MSNAUDIO          =  0x0032; /* Microsoft Corporation */
	static int WAVE_FORMAT_G726              =  0x0045; /* ITU-T standard  */
	static int WAVE_FORMAT_MPEG              =  0x0050; /* Microsoft Corporation */
	static int WAVE_FORMAT_MPEGLAYER3        =  0x0055; /* ISO/MPEG Layer3 Format Tag */
	static int WAVE_FORMAT_DOLBY_AC3_SPDIF   =  0x0092; /* Sonic Foundry */
	static int WAVE_FORMAT_A52               =  0x2000;
	static int WAVE_FORMAT_DTS               =  0x2001;
	static int WAVE_FORMAT_WMA1              =  0x0160; /* WMA version 1 */
	static int WAVE_FORMAT_WMA2              =  0x0161; /* WMA (v2) 7, 8, 9 Series */
	static int WAVE_FORMAT_WMAP              =  0x0162; /* WMA 9 Professional */
	static int WAVE_FORMAT_WMAL              =  0x0163; /* WMA 9 Lossless */
	static int WAVE_FORMAT_DIVIO_AAC         =  0x4143;
	static int WAVE_FORMAT_AAC               =  0x00FF;
	static int WAVE_FORMAT_FFMPEG_AAC        =  0x706D;

	static int WAVE_FORMAT_DK3               =  0x0061;
	static int WAVE_FORMAT_DK4               =  0x0062;
	static int WAVE_FORMAT_VORBIS            =  0x566f;
	static int WAVE_FORMAT_VORB_1            =  0x674f;
	static int WAVE_FORMAT_VORB_2            =  0x6750;
	static int WAVE_FORMAT_VORB_3            =  0x6751;
	static int WAVE_FORMAT_VORB_1PLUS        =  0x676f;
	static int WAVE_FORMAT_VORB_2PLUS        =  0x6770;
	static int WAVE_FORMAT_VORB_3PLUS        =  0x6771;
	static int WAVE_FORMAT_SPEEX             =  0xa109; /* Speex audio */

	static int WAVE_FORMAT_EXTENSIBLE        =  0xFFFE; /* Microsoft */

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
	private int numberOfChannels;
	private int numberOfSamplesPerSec;
	private int numberOfAvgBytesPerSec;
	private int numberOfBlockAlign;
	private int numberOfBitsPerSample;
	private Dictionary<string, string> infoChunks = new Dictionary<string, string>();

	public string SelectedFile { get { return selectedFile;  } set { selectedFile = value; } }
	public long FileLength { get { return fileLength;  } set { fileLength = value; } }
	public int NumberOfChannels { get { return numberOfChannels;  } set { numberOfChannels = value; } }
	public int NumberOfSamplesPerSec { get { return numberOfSamplesPerSec;  } set { numberOfSamplesPerSec = value; } }
	public int NumberOfAvgBytesPerSec { get { return numberOfAvgBytesPerSec;  } set { numberOfAvgBytesPerSec = value; } }
	public int NumberOfBlockAlign { get { return numberOfBlockAlign;  } set { numberOfBlockAlign = value; } }
	public int NumberOfBitsPerSample { get { return numberOfBitsPerSample;  } set { numberOfBitsPerSample = value; } }
	public Dictionary<string, string> InfoChunks { get { return infoChunks;  } set { infoChunks = value; } }

	public RiffRead (string value) {
		selectedFile = value;
	}

	public bool Process() {

		long datasize = 0;
		int bytespersec = 0;
		int byteread = 0;
		bool isPCM = false;

		Dictionary <string,string>listinfo = new Dictionary<string,string>();
		for (int i=0; i<infotype.Length; i++) {
			listinfo.Add(infotype[i], infodesc[i]);
		}

		BinaryFile bf = new BinaryFile(selectedFile);
		try {
			FileInfo fileInfo = new FileInfo(selectedFile);
			fileLength = fileInfo.Length;

			int riffdata=0;  // size of RIFF data chunk.
			int chunkSize=0, infochunksize=0, bytecount=0, listbytecount=0;
			string sfield="", infofield="", infodescription="", infodata="";
			
			/*  --------  Get RIFF chunk header --------- */
			sfield = bf.ReadString(4);
			Console.WriteLine("RIFF Header: {0}", sfield);
			if (sfield != "RIFF") {
				Console.WriteLine(" ****  Not a valid RIFF file  ****");
				return false;
			}

			// read RIFF data size
			chunkSize  = bf.ReadInt32();
			Console.WriteLine("RIFF data size: {0}", chunkSize);
			
			// read form-type (WAVE etc)
			sfield = bf.ReadString(4);
			Console.WriteLine("Form-type: {0}", sfield);

			riffdata = chunkSize;

			bytecount = 4;  // initialize bytecount to include RIFF form-type bytes.
			while (bytecount < riffdata )  {    // check for chunks inside RIFF data area.
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
				Console.WriteLine("{0} ----- data size: {1} bytes", sfield, chunkSize);
				
				if (sfield == "data") {
					//get data size to compute duration later.
					datasize = chunkSize;
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
					int wFormatTag = bf.ReadInt16();
					if (wFormatTag == WAVE_FORMAT_PCM || wFormatTag == WAVE_FORMAT_EXTENSIBLE || wFormatTag == WAVE_FORMAT_IEEE_FLOAT) {
						isPCM = true;
					}
					if (wFormatTag == WAVE_FORMAT_PCM) {
						Console.WriteLine("\twFormatTag:  WAVE_FORMAT_PCM") ;
					} else if (wFormatTag == WAVE_FORMAT_EXTENSIBLE) {
						Console.WriteLine("\twFormatTag:  WAVE_FORMAT_EXTENSIBLE") ;
					} else if (wFormatTag == WAVE_FORMAT_IEEE_FLOAT) {
						Console.WriteLine("\twFormatTag:  WAVE_FORMAT_IEEE_FLOAT") ;
					} else if (wFormatTag == WAVE_FORMAT_MPEGLAYER3) {
						Console.WriteLine("\twFormatTag:  WAVE_FORMAT_MPEGLAYER3") ;
					} else {
						Console.WriteLine("\twFormatTag:  non-PCM format {0}", wFormatTag) ;
					}

					// Read number of channels, 2 bytes
					int nChannels = bf.ReadInt16();
					Console.WriteLine("\tnChannels: {0}", nChannels);
					NumberOfChannels = nChannels;
					
					// Read sample rate, 4 bytes
					int nSamplesPerSec = bf.ReadInt32();
					Console.WriteLine("\tnSamplesPerSec: {0}", nSamplesPerSec);
					NumberOfSamplesPerSec = nSamplesPerSec;
					
					// Read average bytes per second, 4 bytes
					int nAvgBytesPerSec = bf.ReadInt32();
					bytespersec = nAvgBytesPerSec;
					Console.WriteLine("\tnAvgBytesPerSec: {0}", nAvgBytesPerSec);
					NumberOfAvgBytesPerSec = nAvgBytesPerSec;
					
					// Read block align, 2 bytes
					int nBlockAlign = bf.ReadInt16();
					Console.WriteLine("\tnBlockAlign: {0}", nBlockAlign);
					NumberOfBlockAlign = nBlockAlign;
					
					// Read significant bits per sample, 2 bytes
					if (isPCM) {     // if PCM or EXTENSIBLE format
						int wBitsPerSample = bf.ReadInt16();
						Console.WriteLine("\twBitsPerSample: {0}", wBitsPerSample);
						NumberOfBitsPerSample = wBitsPerSample;
					} else {
						bf.ReadBytes(2);
						NumberOfBitsPerSample = 0;
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
						Console.WriteLine("------- INFO chunks: {0} bytes -------", chunkSize);
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
								Console.WriteLine("{0} ({1}) = {2}", infofield, infodescription, infodata);
								InfoChunks.Add(infodescription, infodata);
							} else {
								Console.WriteLine("unknown: {0} = {1}", infofield, infodata);
								InfoChunks.Add(String.Format("unknown{0}", unknownCount), infodata);
								unknownCount++;
							}
						} //------- end iteration over LIST chunk ------------
						
					} catch (Exception) {;
						// don't care about these?
						//Console.WriteLine("Error: {0}", e.ToString());
					}
					
					Console.WriteLine("------- end INFO chunks -------");
					
				} else {    // if NOT the fmt or LIST chunks just skip over the data.
					bf.ReadBytes(chunkSize);
				}
				
			}  // end while.

			//-----------  End of chunk iteration -------------
			if(isPCM && datasize > 0) {   // compute duration of PCM wave file
				long waveduration = 1000L * datasize / bytespersec; // in msec units
				long mins = waveduration / 60000;    // integer minutes
				double secs = 0.001 * (waveduration % 60000);    //double secs.
				Console.WriteLine("wav duration:  {0} mins  {1} sec", mins, secs);
			}

			Console.WriteLine("Final RIFF data bytecount: {0}", bytecount);
			if ( ( 8 + bytecount) != (int) fileLength)  {
				Console.WriteLine("!!!!!!! Problem with file structure  !!!!!!!!!");
				return false;
			} else {
				Console.WriteLine("File chunk structure consistent with valid RIFF") ;
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
}