using System;
using System.Collections.Generic;

using System.Globalization;
using System.Runtime.InteropServices; // GCHandle
using System.IO; // Stream
using System.Text; // Encoding

using System.Diagnostics;

using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

using System.ComponentModel;

using CommonUtils.Audio; // IWaveformPlayer

using System.Windows.Forms; // Timer

namespace CommonUtils.Audio
{
	/// <summary>
	///   Bass Proxy for Bass.Net API
	/// </summary>
	/// <remarks>
	///   BASS is an audio library for use in Windows and Mac OSX software.
	///   Its purpose is to provide developers with powerful and efficient sample, stream (MP3, MP2, MP1, OGG, WAV, AIFF, custom generated, and more via add-ons),
	///   MOD music (XM, IT, S3M, MOD, MTM, UMX), MO3 music (MP3/OGG compressed MODs), and recording functions.
	///   All in a tiny DLL, under 100KB* in size.
	/// </remarks>
	/// <remarks>
	/// Some of this originated from "Sound Fingerprinting framework" ciumac.sergiu@gmail.com
	/// Modified heaviliy by perivar@nerseth.com
	/// </remarks>
	public class BassProxy : IWaveformPlayer
	{
		#region Fields
		const int DEFAULT_SAMPLE_RATE = 44100; // Default sample rate used at initialization

		// singleton instance
		static BassProxy _instance;
		
		// Position variables
		readonly Timer _positionTimer = new Timer();

		int _currentChannelSamplePosition; // current position in playing stream in samples
		bool _inChannelSet;
		bool _inChannelTimerUpdate;
		
		// Loop variables
		const int LOOP_THRESHOLD_SAMPLES = 2; // what is the minimum amount of samples that can be looped
		int _loopSampleStart;
		int _loopSampleStop;
		bool _inLoopSet;
		
		// Waveform Generator variables
		readonly BackgroundWorker _waveformGenerateWorker = new BackgroundWorker();
		string _pendingWaveformPath;
		
		// Bass variables to track reaching end of stream or loop
		readonly SYNCPROC _endTrackSyncProc;
		readonly SYNCPROC _loopSyncProc;
		int _loopSyncId;
		
		// IAudio variables
		bool _canPlay;
		bool _canPause;
		bool _canStop;
		bool _isPlaying;

		/// <summary>
		/// Shows whether the Bass Engine (proxy) has already been disposed
		/// </summary>
		bool _hasDisposed = false;

		/// <summary>
		/// Currently playing stream
		/// </summary>
		int _playingStream;

		// Properties retrieved when using OpenFile
		int _sampleRate;
		int _bitsPerSample;
		int _channels;
		int _channelSampleLength; // duration in samples per channel
		string _filePath; // file path in openfile
		
		// store the waveform data as IEEE 32 bit float
		float[] _waveformData;
		
		bool _isInitialized = false;
		Dictionary<int, string> _loadedPlugIns = new Dictionary<int, string>();
		
		// playing from memory variables
		const bool _doPlayFromMemory = false;
		STREAMPROC _myStreamCreate; // make it global, so that the GC can not remove it
		int _dataBytePos; // play position in bytes when playing from memory
		GCHandle _hGCFile; // make it global, so that the GC can not remove it
		#endregion

		#region Get Field Methods
		public string FileFilter {
			get {
				return "All supported Audio Files|*.wav;*.ogg;*.mp1;*.m1a;*.mp2;*.m2a;*.mpa;*.mus;*.mp3;*.mpg;*.mpeg;*.mp3pro;*.aif;*.aiff;*.bwf;*.wma;*.wmv;*.aac;*.adts;*.mp4;*.m4a;*.m4b;*.mod;*.mdz;*.mo3;*.s3m;*.s3z;*.xm;*.xmz;*.it;*.itz;*.umx;*.mtm;*.flac;*.fla;*.oga;*.ogg;*.aac;*.m4a;*.m4b;*.mp4;*.mpc;*.mp+;*.mpp;*.ac3;*.wma;*.ape;*.mac;*.m4a;*.tta;*.wv|WAVE Audio|*.wav|Ogg Vorbis|*.ogg|MPEG Layer 1|*.mp1;*.m1a|MPEG Layer 2|*.mp2;*.m2a;*.mpa;*.mus|MPEG Layer 3|*.mp3;*.mpg;*.mpeg;*.mp3pro|Audio IFF|*.aif;*.aiff|Broadcast Wave|*.bwf|Windows Media Audio|*.wma;*.wmv|Advanced Audio Codec|*.aac;*.adts|MPEG 4 Audio|*.mp4;*.m4a;*.m4b|MOD Music|*.mod;*.mdz|MO3 Music|*.mo3|S3M Music|*.s3m;*.s3z|XM Music|*.xm;*.xmz|IT Music|*.it;*.itz;*.umx|MTM Music|*.mtm|Free Lossless Audio Codec|*.flac;*.fla|Free Lossless Audio Codec (Ogg)|*.oga;*.ogg|Advanced Audio Coding|*.aac|Advanced Audio Coding MPEG-4|*.m4a;*.m4b;*.mp4|Musepack|*.mpc;*.mp+;*.mpp|Dolby Digital AC-3|*.ac3|Windows Media Audio|*.wma|Monkey's Audio|*.ape;*.mac|Apple Lossless Audio Codec|*.m4a|The True Audio|*.tta|WavPack|*.wv";
			}
		}
		
		public string FilePath {
			get {
				return _filePath;
			}
		}

		public int SampleRate
		{
			get { return _sampleRate; }
		}

		public int BitsPerSample
		{
			get { return _bitsPerSample; }
		}

		public int BytesPerSample
		{
			get { return BitsPerSample / 8; }
		}
		
		public int Channels
		{
			get { return _channels; }
		}
		
		public int TotalSampleLength {
			get {
				return _waveformData != null ? _waveformData.Length : -1;
			}
		}
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Load BASS plugin dll
		/// </summary>
		/// <param name="plugin">filename to the dll file</param>
		private void LoadPlugin(string plugin) {
			int pluginHandle;
			if((pluginHandle = Bass.BASS_PluginLoad(plugin)) == 0) {
				throw new FileNotFoundException(Bass.BASS_ErrorGetCode().ToString());
			}
			
			_loadedPlugIns.Add(pluginHandle, plugin);
			
			#if DEBUG
			BASS_PLUGININFO info = Bass.BASS_PluginGetInfo(pluginHandle);
			foreach (BASS_PLUGINFORM f in info.formats) {
				Debug.WriteLine("Type={0}, Name={1}, Exts={2}", f.ctype, f.name, f.exts);
			}
			#endif
		}
		
		private void SetupBass()
		{
			// Call to avoid the freeware splash screen.
			BassNet.Registration("perivar@nerseth.com", "2X25233719142322");
			
			// Dummy calls made for loading the assemblies
			int bassVersion = Bass.BASS_GetVersion();
			int bassMixVersion = BassMix.BASS_Mixer_GetVersion();
			int bassfxVersion = BassFx.BASS_FX_GetVersion();
			
			#if DEBUG
			Debug.WriteLine("Bass Version: {0}, Mix Version: {1}, FX Version: {2}", bassVersion, bassMixVersion, bassfxVersion);
			#endif
			
			// Initialize Bass
			if (Bass.BASS_Init(-1, DEFAULT_SAMPLE_RATE, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
			{
				// Load the plugins
				LoadPlugin("bassflac.dll");
				LoadPlugin("bass_aac.dll");
				LoadPlugin("bass_mpc.dll");
				LoadPlugin("bass_ac3.dll");
				LoadPlugin("basswma.dll");
				LoadPlugin("bass_ape.dll");
				LoadPlugin("bass_alac.dll");
				LoadPlugin("bass_tta.dll");
				LoadPlugin("basswv.dll");
				
				#if DEBUG
				var info = new BASS_INFO();
				Bass.BASS_GetInfo(info);
				Debug.WriteLine(info.ToString());
				
				string nativeSupport = Utils.BASSAddOnGetSupportedFileExtensions(null);
				Debug.WriteLine("Native Bass Supported Extensions: " + nativeSupport);
				
				string fileSupportedExtFilter = Utils.BASSAddOnGetPluginFileFilter(_loadedPlugIns, "All supported Audio Files", true);
				Debug.WriteLine("Bass generated FileFilter: " + fileSupportedExtFilter);
				#endif
			} else {
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
			}
			
			// Set filter for anti aliasing
			if (!Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_MIXER_FILTER, 50)) {
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
			}
			
			// Set floating parameters to be passed
			if (!Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true)) {
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
			}
		}

		/// <summary>
		/// Private Constructor
		/// </summary>
		private BassProxy()
		{
			Initialize();
			
			// Set the methods that BASS will call when reaching end of stream or repeat
			_endTrackSyncProc = EndTrackSyncCallback;
			_loopSyncProc = LoopSyncCallback;
			
			_waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
			_waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
			_waveformGenerateWorker.WorkerSupportsCancellation = true;
		}
		#endregion

		#region Singleton Instance
		public static BassProxy Instance
		{
			get
			{
				if (_instance == null)
					_instance = new BassProxy();
				return _instance;
			}
		}
		#endregion

		#region Private Utility Methods
		private void Initialize()
		{
			SetupBass();
			
			// Define the timer that checks the position while playing
			_positionTimer.Interval = 50; // 50 ms
			_positionTimer.Tick += OnTimedEvent;
			
			// The Timer is enabled/disabled in the IsPlaying method
			IsPlaying = false;
			
			_isInitialized = true;
		}

		/// <summary>
		/// Return the Channel Information in the out variables
		/// </summary>
		/// <param name="channelHandle">The channel handle</param>
		/// <param name="sampleRate">Output the Sample rate of the wave file (e.g. 8000, 11025, 22050, 44100, 48000, 96000) in Hz.</param>
		/// <param name="bitsPerSample">Output the Bits per sample of the wave file (must be either 8, 16, 24 or 32).</param>
		/// <param name="channels">Output the Number of channels of the wave file (1=mono, 2=stereo...).</param>
		/// <param name="byteLength">Output the Length of file in bytes</param>
		/// <param name="sampleLength">Output the Length of file in samples per channel</param>
		private static void GetChannelInformation(int channelHandle, out int sampleRate, out int bitsPerSample, out int channels, out long byteLength, out int sampleLength) {
			
			if (channelHandle != 0) {
				var info = Bass.BASS_ChannelGetInfo(channelHandle);
				sampleRate = info.freq;
				bitsPerSample = info.Is8bit ? 8 : (info.Is32bit ? 32 : 16);
				channels = info.chans;

				// length in bytes
				byteLength = Bass.BASS_ChannelGetLength(channelHandle, BASSMode.BASS_POS_BYTES);

				// Get stream length in samples per channel
				sampleLength = (int) (byteLength / (bitsPerSample / 8 * channels));
			} else {
				sampleRate = -1;
				bitsPerSample = -1;
				channels = -1;
				byteLength = -1;
				sampleLength = -1;
			}
		}
		
		/// <summary>
		/// Return the float data as a proper RIFF file in a byte array (IEEE 32 bit float format)
		/// </summary>
		/// <param name="data">The float array</param>
		/// <param name="bitsPerSample">Bits per sample of the wave file (must be either 8, 16, 24 or 32).</param>
		/// <param name="channels">The number of channels of the wave file (1=mono, 2=stereo...).</param>
		/// <param name="sampleRate">The Sample rate of the wave file (e.g. 8000, 11025, 22050, 44100, 48000, 96000) in Hz.</param>
		/// <returns>Returns a proper RIFF file as a byte array</returns>
		/// <remarks>Note! This must be modified to support anything else than 32 bit float</remarks>
		private static byte[] GetRIFFFormat(float[] data, int bitsPerSample, int channels, int sampleRate) {
			if (data == null || data.Length == 0) return null;
			
			int numSamples = data.Length << 2;
			
			using (var stream = new MemoryStream()) {
				using (var wr = new BinaryWriter(stream)){
					wr.Write(Encoding.ASCII.GetBytes("RIFF"));
					wr.Write(36 + (int)(numSamples));
					wr.Write(Encoding.ASCII.GetBytes("WAVE"));
					
					// write format chunk
					wr.Write(Encoding.ASCII.GetBytes("fmt "));
					wr.Write(16); // chunk size in bytes
					
					if (bitsPerSample == 8 || bitsPerSample == 16 || bitsPerSample == 24) {
						wr.Write((ushort)1); // WAVE_FORMAT_PCM
					} else if (bitsPerSample == 32) {
						wr.Write((ushort)3); // IEEE 32 bit float encoding (WAVE_FORMAT_IEEE_FLOAT)
					}
					
					wr.Write((short)channels); // channels
					wr.Write((int)sampleRate); // sample rate
					wr.Write((int)(sampleRate * bitsPerSample / 8 * channels)); // Average bytes per second
					wr.Write((short)(bitsPerSample / 8 * channels)); // block align
					wr.Write((short)(bitsPerSample)); // bits per sample
					
					// write data chunk
					wr.Write(Encoding.ASCII.GetBytes("data"));
					wr.Write((int)(numSamples));
					foreach (var d in data) {
						wr.Write((float)d);
					}
					
					return stream.ToArray();
				}
			}
		}
		
		/// <summary>
		/// Sets the playback position of a stream.
		/// </summary>
		/// <param name="channelHandle">The channel handle</param>
		/// <param name="samplePosition">The sample position</param>
		/// <returns>If successful, then TRUE is returned, else FALSE is returned.</returns>
		private bool SetChannelSamplePosition(int channelHandle, int samplePosition) {
			
			// Set position using bytes
			long bytePosition = (samplePosition * BytesPerSample * Channels);
			
			// Store the data byte position
			_dataBytePos = (int) bytePosition;
			
			// try setting the position
			if (!Bass.BASS_ChannelSetPosition(channelHandle, bytePosition, BASSMode.BASS_POS_BYTES)) {
				// This always failes unless the stream is a file stream.
				String errorCode = Bass.BASS_ErrorGetCode().ToString();
				
				// failed, go to start of file instead
				//Bass.BASS_ChannelSetPosition(channelHandle, 0, BASSMode.BASS_POS_BYTES);
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// Retrieves the playback position of a stream
		/// </summary>
		/// <param name="channelHandle">The channel handle</param>
		/// <returns>If successful, then the channel's position is returned, else -1 is returned.</returns>
		private int GetChannelSamplePosition(int channelHandle) {
			int samplePosition = -1;
			long bytePosition = -1;
			
			bytePosition = Bass.BASS_ChannelGetPosition(channelHandle, BASSMode.BASS_POS_BYTES);
			
			if (bytePosition != -1) {
				samplePosition = (int) (bytePosition / (BytesPerSample * Channels));
			} else {
				String errorCode = Bass.BASS_ErrorGetCode().ToString();
			}
			
			return samplePosition;
		}
		
		private void SetLoopRange(int startSamplePosition, int endSamplePosition) {
			if (_loopSyncId != 0)
				Bass.BASS_ChannelRemoveSync(_playingStream, _loopSyncId);

			if ((endSamplePosition - startSamplePosition) > LOOP_THRESHOLD_SAMPLES) {
				// length in bytes
				long startBytePosition = (startSamplePosition * BytesPerSample * Channels);
				long endBytePosition = (endSamplePosition * BytesPerSample * Channels);
				
				// For Sample Accurate Looping set mixtime POS sync at loop end
				_loopSyncId = Bass.BASS_ChannelSetSync(_playingStream,
				                                       BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME,
				                                       endBytePosition,
				                                       _loopSyncProc,
				                                       IntPtr.Zero);
				
				if (_loopSyncId == 0) {
					// failed
					String errorCode = Bass.BASS_ErrorGetCode().ToString();
				}
				
				// Seek to loop start
				ChannelSamplePosition = startSamplePosition;
			} else {
				ClearLoopRange();
			}
		}
		
		private void ClearLoopRange()
		{
			if (_loopSyncId != 0) {
				Bass.BASS_ChannelRemoveSync(_playingStream, _loopSyncId);
				_loopSyncId = 0;
			}
		}
		#endregion
		
		#region Public Static Methods
		
		/// <summary>
		/// Read an audio file as MONO into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name = "stream">stream</param>
		/// <param name = "samplerate">Sample rate at which to perform reading</param>
		/// <returns>Array with mono data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromFile(Stream stream, int samplerate) {
			var length = stream.Length;
			var source = new byte[length];
			
			// read the file into the buffer
			stream.Read(source, 0, (int)length);
			
			// now create a pinned handle, so that the Garbage Collector will not move this object
			var _hGCFile = GCHandle.Alloc(source, GCHandleType.Pinned);
			try
			{
				var buffer = ReadMonoFromStream(_hGCFile.AddrOfPinnedObject(), source.Length, samplerate, -1, 0);
				return buffer;
			}
			catch(Exception e)
			{
				throw new Exception(e.Message);
			}
			finally
			{
				_hGCFile.Free();
			}
		}
		
		/// <summary>
		/// Read an audio file as MONO into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name = "filename">Filename to be read</param>
		/// <param name = "samplerate">Sample rate at which to perform reading</param>
		/// <returns>Array with mono data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromFile(string filename, int samplerate)
		{
			return ReadMonoFromFile(filename, samplerate, 0, 0);
		}
		
		/// <summary>
		/// Read an audio file as MONO into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name="filename">Name of the file</param>
		/// <param name="samplerate">Output sample rate</param>
		/// <param name="milliseconds">Milliseconds to read</param>
		/// <param name="startmillisecond">Start millisecond</param>
		/// <returns>Array of samples</returns>
		/// <remarks>
		/// Seeking capabilities of Bass where not used because of the possible
		/// timing errors on different formats.
		/// </remarks>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromFile(string filename, int samplerate, int milliseconds, int startmillisecond)
		{
			// BASS_STREAM_DECODE	Decode the sample data, without outputting it.
			// Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data.
			// The BASS_SAMPLE_SOFTWARE, BASS_SAMPLE_3D, BASS_SAMPLE_FX, BASS_STREAM_AUTOFREE and SPEAKER flags can not be used together with this flag.
			
			// BASS_SAMPLE_MONO	Decode/play the stream (MP3/MP2/MP1 only) in mono, reducing the CPU usage (if it was originally stereo).
			// This flag is automatically applied if BASS_DEVICE_MONO was specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr).
			
			// BASS_SAMPLE_FLOAT	Produce 32-bit floating-point output.
			// WDM drivers or the BASS_STREAM_DECODE flag are required to use this flag in Windows.
			
			// create streams for re-sampling
			int stream = Bass.BASS_StreamCreateFile(filename, 0L, 0L,
			                                        BASSFlag.BASS_STREAM_DECODE |
			                                        BASSFlag.BASS_SAMPLE_MONO |
			                                        BASSFlag.BASS_SAMPLE_FLOAT);
			
			return ReadMonoFromFile(stream, samplerate, milliseconds, startmillisecond);
		}

		/// <summary>
		/// Read an audio file as MONO into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name="streamPtr">pinned handle</param>
		/// <param name="length">byte length</param>
		/// <param name="samplerate">Output sample rate</param>
		/// <param name="milliseconds">Milliseconds to read</param>
		/// <param name="startmillisecond">Start millisecond</param>
		/// <returns>Array of samples</returns>
		/// <remarks>
		/// Seeking capabilities of Bass where not used because of the possible
		/// timing errors on different formats.
		/// </remarks>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromStream(IntPtr streamPtr, int length, int samplerate, int milliseconds, int startmillisecond)
		{
			// BASS_STREAM_DECODE	Decode the sample data, without outputting it.
			// Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data.
			// The BASS_SAMPLE_SOFTWARE, BASS_SAMPLE_3D, BASS_SAMPLE_FX, BASS_STREAM_AUTOFREE and SPEAKER flags can not be used together with this flag.
			
			// BASS_SAMPLE_MONO	Decode/play the stream (MP3/MP2/MP1 only) in mono, reducing the CPU usage (if it was originally stereo).
			// This flag is automatically applied if BASS_DEVICE_MONO was specified when calling BASS_Init(Int32, Int32, BASSInit, IntPtr, IntPtr).
			
			// BASS_SAMPLE_FLOAT	Produce 32-bit floating-point output.
			// WDM drivers or the BASS_STREAM_DECODE flag are required to use this flag in Windows.

			// create streams for re-sampling
			int stream = Bass.BASS_StreamCreateFile(streamPtr, 0L, length,
			                                        BASSFlag.BASS_STREAM_DECODE |
			                                        BASSFlag.BASS_SAMPLE_MONO |
			                                        BASSFlag.BASS_SAMPLE_FLOAT);
			
			return ReadMonoFromFile(stream, samplerate, milliseconds, startmillisecond);
		}
		
		/// <summary>
		/// Utility method to read BASS stream as mono
		/// </summary>
		/// <param name="stream">BASS sample stream</param>
		/// <param name="samplerate">Output sample rate</param>
		/// <param name="milliseconds">Milliseconds to read</param>
		/// <param name="startmillisecond">Start millisecond</param>
		/// <returns>Array of samples</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		private static float[] ReadMonoFromFile(int stream, int samplerate, int milliseconds, int startmillisecond)
		{
			float[] data = null;
			int mixerStream = 0;
			
			try
			{
				// failed creating the stream, something wrong with the audio file?
				if (stream == 0) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
				
				// total length
				int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;

				// mixer stream
				mixerStream = BassMix.BASS_Mixer_StreamCreate(samplerate, 1,
				                                              BASSFlag.BASS_STREAM_DECODE |
				                                              BASSFlag.BASS_SAMPLE_MONO |
				                                              BASSFlag.BASS_SAMPLE_FLOAT);
				
				// failed creating mixer stream, something wrong with the audio file?
				if (mixerStream == 0) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}

				// BASS_MIXER_DOWNMIX	If the source has more channels than the mixer output (and the mixer is stereo or mono),
				// then a channel matrix is created, initialized with the appropriate downmixing matrix.
				// Note the source data is assumed to follow the standard channel ordering, as described in the STREAMPROC documentation.
				
				// BASS_MIXER_NORAMPIN	Do not ramp-in the start, including after seeking (BASS_Mixer_ChannelSetPosition).
				// This is useful for gap-less playback, where a source channel is intended to seamlessly follow another. This does not affect volume and pan changes, which are always ramped.
				
				if (BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream,
				                                        BASSFlag.BASS_MIXER_DOWNMIX |
				                                        BASSFlag.BASS_MIXER_NORAMPIN)) {

					int bufferSize = samplerate * 10 * 4; // Read 10 seconds at each iteration
					
					var buffer = new float[bufferSize];
					var chunks = new List<float[]>();
					
					int size = 0;
					while ((float)(size) / samplerate * 1000 < totalmilliseconds) {
						// get re-sampled/mono data
						int bytesRead = Bass.BASS_ChannelGetData(mixerStream, buffer, bufferSize);
						
						if (bytesRead == 0) {
							break;
						}
						
						var chunk = new float[bytesRead / 4]; // each float contains 4 bytes
						Array.Copy(buffer, chunk, bytesRead / 4);
						chunks.Add(chunk);
						size += bytesRead / 4; // size of the data
					}

					// Check if there are enough samples to return the data.
					if ((float)(size) / samplerate * 1000 < (milliseconds + startmillisecond)) {
						// not enough samples to return the requested data
						return null;
					}
					
					int start = (int)((float)startmillisecond * samplerate / 1000);
					
					int end = (milliseconds <= 0)
						? size
						: (int)((float)(startmillisecond + milliseconds) * samplerate / 1000);
					
					data = new float[size];
					int index = 0;
					
					// Concat the pieces of the chunks.
					foreach (float[] chunk in chunks) {
						Array.Copy(chunk, 0, data, index, chunk.Length);
						index += chunk.Length;
					}
					
					// Select specific part of the song
					if (start != 0 || end != size) {
						var temp = new float[end - start];
						Array.Copy(data, start, temp, 0, end - start);
						data = temp;
					}
				} else {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
				
			}
			finally
			{
				Bass.BASS_StreamFree(stream);
				Bass.BASS_StreamFree(mixerStream);
			}
			return data;
		}

		/// <summary>
		/// Read an audio file into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name="fileName">Fully referenced path and file name of the Wave file to create.</param>
		/// <param name="sampleRate">Sample rate of the wave file (e.g. 8000, 11025, 22050, 44100, 48000, 96000) in Hz.</param>
		/// <param name="bitsPerSample">Bits per sample of the wave file (must be either 8, 16, 24 or 32).</param>
		/// <param name="channels">Number of channels of the wave file (1=mono, 2=stereo...).</param>
		/// <param name="byteLength">Length of file in bytes</param>
		/// <param name="channelSampleLength">Length of file in samples per channel</param>
		/// <returns>Array with multi channel data</returns>
		/// <remarks>
		/// Audio data will be structured in an array where each sucessive index
		/// alternates between left or right channel data, starting with left. Index 0
		/// will be the first left level, index 1 will be the first right level, index
		/// 2 will be the second left level, etc.
		/// </remarks>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadFromFile(string fileName, out int sampleRate, out int bitsPerSample, out int channels, out long byteLength, out int channelSampleLength) {
			
			// BASS_STREAM_DECODE	Decode the sample data, without outputting it.
			// Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data.
			// The BASS_SAMPLE_SOFTWARE, BASS_SAMPLE_3D, BASS_SAMPLE_FX, BASS_STREAM_AUTOFREE and SPEAKER flags can not be used together with this flag.
			
			// BASS_SAMPLE_FLOAT	Produce 32-bit floating-point output.
			// WDM drivers or the BASS_STREAM_DECODE flag are required to use this flag in Windows.

			float[] buffer = null;
			
			// Create channel stream
			int mchan = Bass.BASS_StreamCreateFile(fileName, 0, 0,
			                                       BASSFlag.BASS_STREAM_DECODE |
			                                       BASSFlag.BASS_SAMPLE_FLOAT);
			try
			{
				// failed creating the stream, something wrong with the audio file?
				if (mchan == 0) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
				
				// read int the channel handle information
				GetChannelInformation(mchan, out sampleRate, out bitsPerSample, out channels, out byteLength, out channelSampleLength);
				
				// define float array
				buffer = new float[byteLength/4];

				// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT flag).
				// If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code.
				int bytesRead = Bass.BASS_ChannelGetData(mchan, buffer, (int)byteLength);
				if (bytesRead == -1) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				} else if (bytesRead == 0) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
			}
			finally
			{
				// free resource
				Bass.BASS_StreamFree(mchan);
			}
			return buffer;
		}

		/// <summary>
		/// Read an audio file into a float array (32-bit floating-point sample data)
		/// </summary>
		/// <param name="fileName">Fully referenced path and file name of the Wave file to create.</param>
		/// <returns>Array with multi channel data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadFromFile(string fileName) {
			
			int sampleRate = -1;
			int bitsPerSample = -1;
			int channels = -1;
			long byteLength = -1;
			int channelSampleLength = -1;
			
			return ReadFromFile(fileName, out sampleRate, out bitsPerSample, out channels, out byteLength, out channelSampleLength);
		}
		
		/// <summary>
		/// Define how converting to mono should happen,
		/// A mix of left and right or only one of the channels?
		/// </summary>
		public enum MonoSummingType {
			Mix,
			Left,
			Right
		}
		
		/// <summary>
		/// Read an audio file into a float array as a Mono file (32-bit floating-point sample data)
		/// </summary>
		/// <param name="fileName">Fully referenced path and file name of the Wave file to create.</param>
		/// <returns>Array with multi channel data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromFile(string fileName) {
			return ReadMonoFromFile(fileName, MonoSummingType.Mix);
		}
		
		/// <summary>
		/// Read an audio file into a float array as a Mono file (32-bit floating-point sample data)
		/// </summary>
		/// <param name="fileName">Fully referenced path and file name of the Wave file to create.</param>
		/// <param name="monoType">Define how converting to mono should happen (mix, left or right)</param>
		/// <returns>Array with multi channel data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[] ReadMonoFromFile(string fileName, MonoSummingType monoType) {

			int sampleRate = -1;
			int bitsPerSample = -1;
			int channels = -1;
			long byteLength = -1;
			int channelSampleLength = -1;
			
			float[] audioSamples = ReadFromFile(fileName, out sampleRate, out bitsPerSample, out channels, out byteLength, out channelSampleLength);

			if (channels == 1) {
				return audioSamples;
			} else if (channels == 2) {
				// we are getting a stereo channel file back
				float sampleValueLeft = 0;
				float sampleValueRight = 0;
				float sampleValueMono = 0;

				int samplesPerChannel = (int) ((double) audioSamples.Length / (double) channels);
				var channelSamples = new float[samplesPerChannel];
				
				for (int i = 0; i < samplesPerChannel; i++) {
					sampleValueLeft = audioSamples[channels*i];
					sampleValueRight = audioSamples[channels*i+1];
					
					switch (monoType) {
						case MonoSummingType.Mix:
							// convert to mono by taking an average of the first two channels:
							// f_mono = function(l, r) {
							//   return (l + r) / 2;
							//}
							sampleValueMono = (sampleValueLeft + sampleValueRight) / 2;
							break;
						case MonoSummingType.Left:
							sampleValueMono = sampleValueLeft;
							break;
						case MonoSummingType.Right:
							sampleValueMono = sampleValueRight;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					
					channelSamples[i] = sampleValueMono;
				}
				
				return channelSamples;
			} else {
				// don't support multi channel audio files (more than 2 channels)
				return null;
			}
		}

		/// <summary>
		/// Read the spectrum from file
		/// </summary>
		/// <param name="filename">filename</param>
		/// <param name="samplerate">Output sample rate</param>
		/// <param name="milliseconds">Milliseconds to read</param>
		/// <param name="startmillisecond">Start millisecond</param>
		/// <param name="overlap"></param>
		/// <param name="wdftsize"></param>
		/// <param name="logbins"></param>
		/// <param name="startfreq"></param>
		/// <param name="endfreq"></param>
		/// <returns>spectrum data</returns>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static float[][] ReadSpectrum(string filename, int samplerate, int startmillisecond, int milliseconds, int overlap, int wdftsize, int logbins, int startfreq, int endfreq)
		{
			int totalmilliseconds = 0;
			if (milliseconds <= 0) {
				totalmilliseconds = Int32.MaxValue;
			} else {
				totalmilliseconds = milliseconds + startmillisecond;
			}
			const int logbase = 2;
			double logMin = Math.Log(startfreq, logbase);
			double logMax = Math.Log(endfreq, logbase);
			double delta = (logMax - logMin)/logbins;
			double accDelta = 0;
			var freqs = new float[logbins + 1];
			for (int i = 0; i <= logbins /*32 octaves*/; ++i)
			{
				freqs[i] = (float) Math.Pow(logbase, logMin + accDelta);
				accDelta += delta; // accDelta = delta * i
			}

			var data = new List<float[]>();
			var streams = new int[wdftsize/overlap - 1];
			var mixerstreams = new int[wdftsize/overlap - 1];
			double sec = (double) overlap/samplerate;
			for (int i = 0; i < wdftsize/overlap - 1; i++)
			{
				streams[i] = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_FLOAT); //Decode the stream
				if (!Bass.BASS_ChannelSetPosition(streams[i], (float)startmillisecond/1000 + sec*i)) {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
				
				mixerstreams[i] = BassMix.BASS_Mixer_StreamCreate(samplerate, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_FLOAT);
				if (!BassMix.BASS_Mixer_StreamAddChannel(mixerstreams[i], streams[i], BASSFlag.BASS_MIXER_FILTER))  {
					throw new Exception(Bass.BASS_ErrorGetCode().ToString());
				}
			}

			var buffer = new float[wdftsize/2];
			int size = 0;
			int iter = 0;
			while ((float) (size)/samplerate*1000 < totalmilliseconds)
			{
				int bytesRead = Bass.BASS_ChannelGetData(mixerstreams[iter%(wdftsize/overlap - 1)], buffer, (int) BASSData.BASS_DATA_FFT2048);
				if (bytesRead == 0)
					break;
				var chunk = new float[logbins];
				for (int i = 0; i < logbins; i++)
				{
					int lowBound = (int) freqs[i];
					int endBound = (int) freqs[i + 1];
					int startIndex = Un4seen.Bass.Utils.FFTFrequency2Index(lowBound, wdftsize, samplerate);
					int endIndex = Un4seen.Bass.Utils.FFTFrequency2Index(endBound, wdftsize, samplerate);
					float sum = 0f;
					for (int j = startIndex; j < endIndex; j++)
					{
						sum += buffer[j];
					}
					chunk[i] = sum/(endIndex - startIndex);
				}
				
				data.Add(chunk);
				size += bytesRead/4;
				iter++;
			}

			return data.ToArray();
		}
		
		/// <summary>
		/// Get's tag info from file
		/// </summary>
		/// <param name = "filename">Filename to decode</param>
		/// <returns>TAG_INFO structure</returns>
		/// <remarks>
		///   The tags can be extracted using the following code:
		///   <code>
		///     tags.album
		///     tags.albumartist
		///     tags.artist
		///     tags.title
		///     tags.duration
		///     tags.genre, and so on.
		///   </code>
		/// </remarks>
		public static TAG_INFO GetTagInfoFromFile(string filename)
		{
			return BassTags.BASS_TAG_GetFromFile(filename);
		}

		/// <summary>
		/// Return the duration in seconds
		/// </summary>
		/// <param name="filename">filename</param>
		/// <param name="preScanMPStreams">whether to pre scan mp3</param>
		/// <returns>duration in seconds</returns>
		public static double GetDurationInSeconds(string filename, bool preScanMPStreams = false) {
			
			double timeLength = -1;
			
			// BASS_STREAM_DECODE	Decode the sample data, without outputting it.
			// Use BASS_ChannelGetData(Int32, IntPtr, Int32) to retrieve decoded sample data.
			// The BASS_SAMPLE_SOFTWARE, BASS_SAMPLE_3D, BASS_SAMPLE_FX, BASS_STREAM_AUTOFREE and SPEAKER flags can not be used together with this flag.
			
			// BASS_STREAM_PRESCAN	Enable pin-point accurate seeking (to the exact byte) on the MP3/MP2/MP1 stream.
			// This also increases the time taken to create the stream, due to the entire file being pre-scanned for the seek points.
			
			int stream = Bass.BASS_StreamCreateFile(filename, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | (preScanMPStreams ? BASSFlag.BASS_STREAM_PRESCAN : BASSFlag.BASS_DEFAULT));
			
			try
			{
				if (stream != 0) {
					
					// length in bytes (float = 32 bits = 4 bytes)
					long byteLength = Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTES);
					
					// the time length
					timeLength = Bass.BASS_ChannelBytes2Seconds(stream, byteLength);
				}
			}
			finally
			{
				// free resource
				Bass.BASS_StreamFree(stream);
			}
			return timeLength;
		}
		
		/// <summary>
		/// Recode a file
		/// </summary>
		/// <param name="fileName">Initial file</param>
		/// <param name="outFileName">Target file</param>
		/// <param name="targetSampleRate">Target sample rate</param>
		/// <exception cref="Exception">Thrown when an error occurs in the BASS Audio system</exception>
		public static void RecodeFileMono(string fileName, string outFileName, int targetSampleRate)
		{
			int stream = Bass.BASS_StreamCreateFile(fileName, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_FLOAT);
			var tags = new TAG_INFO();
			BassTags.BASS_TAG_GetFromFile(stream, tags);
			int mixerStream = BassMix.BASS_Mixer_StreamCreate(targetSampleRate, 1, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_SAMPLE_FLOAT);
			if (BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream, BASSFlag.BASS_MIXER_FILTER))
			{
				var waveWriter = new WaveWriter(outFileName, mixerStream, true);
				const int length = 5512 * 10 * 4;
				var buffer = new float[length];
				while (true)
				{
					int bytesRead = Bass.BASS_ChannelGetData(mixerStream, buffer, length);
					if (bytesRead == 0)
						break;
					waveWriter.Write(buffer, bytesRead);
				}
				waveWriter.Close();
			}
			else
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
		}
		
		/// <summary>
		/// Save float buffer as IEEE Float wave file
		/// </summary>
		/// <param name="buffer">Float array</param>
		/// <param name="fileName">Fully referenced path and file name of the Wave file to create.</param>
		/// <param name="numChannels">Number of channels of the wave file (1=mono, 2=stereo...).</param>
		/// <param name="sampleRate">Sample rate of the wave file (e.g. 8000, 11025, 22050, 44100, 48000, 96000) in Hz.</param>
		/// <param name="bitsPerSample">Bits per sample of the wave file (must be either 8, 16, 24 or 32).</param>
		public static void SaveFile(float[] buffer, string fileName, int numChannels, int sampleRate, int bitsPerSample) {
			var writer = new WaveWriter(fileName, numChannels, sampleRate, bitsPerSample, true);
			writer.Write(buffer, buffer.Length << 2);
			writer.Close();
		}
		#endregion
		
		#region Event Handlers
		void OnTimedEvent(object sender, EventArgs e)
		{
			if (_playingStream == 0)
			{
				ChannelSamplePosition = 0;
			}
			else
			{
				// Make sure to not Set the position at the same time we are reading it
				_inChannelTimerUpdate = true;

				ChannelSamplePosition = GetChannelSamplePosition(_playingStream);
				
				_inChannelTimerUpdate = false;
			}
		}
		#endregion
		
		#region Waveform Generation
		private class WaveformGenerationParams
		{
			public WaveformGenerationParams(string path)
			{
				Path = path;
			}

			public string Path { get; protected set; }
		}
		
		private void GenerateWaveformData(string path)
		{
			if (_waveformGenerateWorker.IsBusy)
			{
				_pendingWaveformPath = path;
				_waveformGenerateWorker.CancelAsync();
				return;
			}

			if (!_waveformGenerateWorker.IsBusy)
				_waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(path));
		}
		
		void waveformGenerateWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var waveformParams = e.Argument as WaveformGenerationParams;
			WaveformData = ReadFromFile(waveformParams.Path);
			
			// TODO: Since ther main work is happening outside of this method, this have no purpose
			/*
			if (waveformGenerateWorker.CancellationPending)
			{
				e.Cancel = true;
				break; ;
			}
			 */
		}

		void waveformGenerateWorker_RunWorkerCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (!_waveformGenerateWorker.IsBusy)
					_waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(_pendingWaveformPath));
			}
		}
		#endregion

		#region IWaveformPlayer Members
		public double ChannelLength {
			get {
				return (double) _channelSampleLength / (double) _sampleRate;
			}
		}
		
		public int ChannelSampleLength {
			get {
				return _channelSampleLength;
			}
			set {
				int oldValue = _channelSampleLength;
				_channelSampleLength = value;
				if ( oldValue != _channelSampleLength) {
					NotifyPropertyChanged("ChannelSampleLength");
					NotifyPropertyChanged("ChannelLength");
				}
			}
		}
		
		public int ChannelSamplePosition {
			get { return _currentChannelSamplePosition; }
			set
			{
				if (!_inChannelSet)
				{
					_inChannelSet = true; // Avoid recursion
					
					int oldValue = _currentChannelSamplePosition;
					int samplePosition = Math.Max(0, Math.Min(value, ChannelSampleLength)); // position in samples
					
					if (!_inChannelTimerUpdate) {
						SetChannelSamplePosition(_playingStream, samplePosition);
					}
					_currentChannelSamplePosition = samplePosition;
					
					if (oldValue != _currentChannelSamplePosition)
						NotifyPropertyChanged("ChannelSamplePosition");
					
					_inChannelSet = false;
				}
			}
		}
		
		public float[] WaveformData {
			get { return _waveformData; }
			protected set
			{
				float[] oldValue = _waveformData;
				_waveformData = value;
				if (oldValue != _waveformData)
					NotifyPropertyChanged("WaveformData");
			}
		}
		
		public int SelectionSampleBegin {
			get { return _loopSampleStart; }
			set
			{
				if (!_inLoopSet)
				{
					_inLoopSet = true;
					int oldValue = _loopSampleStart;
					_loopSampleStart = value;
					if (oldValue != _loopSampleStart)
						NotifyPropertyChanged("SelectionSampleBegin");
					
					SetLoopRange(value, SelectionSampleEnd);
					_inLoopSet = false;
				}
			}
		}
		
		public int SelectionSampleEnd {
			get { return _loopSampleStop; }
			set
			{
				if (!_inLoopSet) // TODO: this was !_inChannelSet, i suppose this was wrong?
				{
					_inLoopSet = true;
					int oldValue = _loopSampleStop;
					_loopSampleStop = value;
					if (oldValue != _loopSampleStop)
						NotifyPropertyChanged("SelectionSampleEnd");
					
					SetLoopRange(SelectionSampleBegin, value);
					_inLoopSet = false;
				}
			}
		}
		#endregion
		
		#region IDisposable implementation
		
		/// <summary>
		/// Public implementation of Dispose pattern callable by consumers.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary>
		/// Protected implementation of Dispose pattern.
		/// </summary>
		/// <param name="isDisposing">If value is disposing</param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_hasDisposed)
				return;
			
			if (!isDisposing)
			{
				// Free any other managed objects here.
				if (_playingStream != 0)
				{
					Bass.BASS_StreamFree(_playingStream);
					_playingStream = 0;
				}
				
				_waveformData = null;
				
				//Bass.BASS_Free();
				
				// when playback has ended and the pinned object is not needed anymore,
				// we need to free the handle!
				// Note: calling this method to early will crash the application,
				// since the buffer would be stolen from BASS while still playing!
				//_hGCFile.Free();
			}
			
			_hasDisposed = true;
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~BassProxy()
		{
			Dispose(false);
		}
		#endregion
		
		#region Bass Proc Methods when playing using memory
		private int BASSReadFloatArrayLoop(int handle, IntPtr buffer, int length, IntPtr user)
		{
			// here we need to deliver PCM sample data
			if (length == 0 || buffer == IntPtr.Zero)
				return 0;

			// 32-bit floats in bytes
			const int bytesPerSample = 4;
			
			// number of bytes in 32-bit floats, since length is in bytes
			int l4 = length/bytesPerSample;
			
			// increase the data buffer as needed
			var _floatData = new float[l4];
			
			// copy from managed to unmanaged memory
			Marshal.Copy(buffer, _floatData, 0, l4);

			// variables in bytes
			int dataByteLen = _channelSampleLength * bytesPerSample * _channels; // its length
			int loopByteStart = _loopSampleStart * bytesPerSample * _channels; // the loop start
			int loopByteEnd = _loopSampleStop * bytesPerSample * _channels; // the loop stop
			
			int doneByteCount = 0;
			do {
				// limit amount of data to what's available
				int byteCount = Math.Min(length-doneByteCount, dataByteLen-_dataBytePos);
				
				if (_loopSyncId != 0) {
					byteCount = Math.Min(loopByteEnd-_dataBytePos, byteCount);
				}
				
				// copy the segment into the float data buffer
				if (byteCount > 0) {
					Array.Copy(_waveformData, _dataBytePos / 4, _floatData, doneByteCount / 4, byteCount / 4);

					// advance the position
					_dataBytePos += byteCount;

					// store the bytes already processed
					doneByteCount += byteCount;
				}
				
				// if reached the end, loop
				if (_dataBytePos == dataByteLen || _dataBytePos == loopByteEnd) {
					if (_loopSyncId != 0) {
						// this also ensure that the byteCount is never zero
						_dataBytePos = loopByteStart;
					} else {
						// copy back from unmanaged to managed memory
						Marshal.Copy(_floatData, 0, buffer, doneByteCount / 4);
						doneByteCount |= (int)BASSStreamProc.BASS_STREAMPROC_END;
						break;
					}
				}
				
			} while (doneByteCount < length);

			// copy back from unmanaged to managed memory if we haven't already
			// (BASS_STREAMPROC_END will make the number negative)
			if (doneByteCount > 0) Marshal.Copy(_floatData, 0, buffer, doneByteCount / 4);

			return doneByteCount;
		}
		#endregion

		#region Public Open and Save method
		public void OpenFile(string path, bool doPlayFromMemory = false) {
			
			if (doPlayFromMemory) {
				OpenFileUsingMemoryAsFileStream(path);
			} else {
				OpenFileUsingFileStream(path);
			}
		}
		
		public void OpenFileUsingMemoryAsStreamProc(string path) {

			Stop();

			if (_playingStream != 0)
			{
				ClearLoopRange();
				ChannelSamplePosition = 0;
				Bass.BASS_StreamFree(_playingStream);
			}
			
			// reset data byte position
			_dataBytePos = 0;
			
			long byteLength = -1;
			int channelSampleLength = -1;
			WaveformData = ReadFromFile(path, out _sampleRate, out _bitsPerSample, out _channels, out byteLength, out channelSampleLength);
			ChannelSampleLength = channelSampleLength; // Notify
			
			// Choose the stream procedure to use
			_myStreamCreate = new STREAMPROC(BASSReadFloatArrayLoop);
			
			// Create playback stream
			_playingStream = Bass.BASS_StreamCreate(_sampleRate, _channels, BASSFlag.BASS_SAMPLE_FLOAT, _myStreamCreate, IntPtr.Zero);
			
			if (_playingStream != 0) {
				
				// Set the stream to call Stop() when it ends.
				int syncHandle = Bass.BASS_ChannelSetSync(_playingStream,
				                                          BASSSync.BASS_SYNC_END,
				                                          0,
				                                          _endTrackSyncProc,
				                                          IntPtr.Zero);

				if (syncHandle == 0)
					throw new ArgumentException("Error establishing End Sync on file stream.", "path");
				
				_filePath = path;
				CanPlay = true;
			} else {
				CanPlay = false;
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
			}
		}
		
		public void OpenFileUsingMemoryAsFileStream(string path) {

			Stop();

			if (_playingStream != 0)
			{
				ClearLoopRange();
				ChannelSamplePosition = 0;
				Bass.BASS_StreamFree(_playingStream);
			}
			
			long byteLength = -1;
			int channelSampleLength = -1;
			WaveformData = ReadFromFile(path, out _sampleRate, out _bitsPerSample, out _channels, out byteLength, out channelSampleLength);
			ChannelSampleLength = channelSampleLength; // Notify
			//SaveFile("proper.wav");
			
			// convert to proper riff format
			byte[] riffArray = GetRIFFFormat(_waveformData, _bitsPerSample, _channels, _sampleRate);
			//CommonUtils.BinaryFile.ByteArrayToFile("test.wav", riffArray);
			
			// now create a pinned handle, so that the Garbage Collector will not move this object
			_hGCFile = GCHandle.Alloc( riffArray, GCHandleType.Pinned );

			// create the stream (AddrOfPinnedObject delivers the necessary IntPtr)
			_playingStream = Bass.BASS_StreamCreateFile(_hGCFile.AddrOfPinnedObject(),
			                                            0L, byteLength, BASSFlag.BASS_SAMPLE_FLOAT);
			
			if (_playingStream != 0) {
				
				// Set the stream to call Stop() when it ends.
				int syncHandle = Bass.BASS_ChannelSetSync(_playingStream,
				                                          BASSSync.BASS_SYNC_END,
				                                          0,
				                                          _endTrackSyncProc,
				                                          IntPtr.Zero);

				if (syncHandle == 0)
					throw new ArgumentException("Error establishing End Sync on file stream.", "path");
				
				_filePath = path;
				CanPlay = true;
			} else {
				CanPlay = false;
				throw new Exception(Bass.BASS_ErrorGetCode().ToString());
			}
		}
		
		public void OpenFileUsingFileStream(string path) {
			
			Stop();

			if (_playingStream != 0)
			{
				ClearLoopRange();
				ChannelSamplePosition = 0;
				Bass.BASS_StreamFree(_playingStream);
			}
			
			// Example:
			// int stream = Bass.BASS_StreamCreateFile(path, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);
			//
			// BASS_STREAM_PRESCAN = Pre-scan the file for accurate seek points and length reading in
			// MP3/MP2/MP1 files and chained OGG files (has no effect on normal OGG files).
			// This can significantly increase the time taken to create the stream, particularly with a large file and/or slow storage media.
			//
			// BASS_SAMPLE_FLOAT = Use 32-bit floating-point sample data.
			//
			// BASS_DEFAULT = 0 = default create stream: 16 Bit, stereo, no Float, hardware mixing, no Loop, no 3D, no speaker assignments...
			
			_playingStream = Bass.BASS_StreamCreateFile(path, 0L, 0L, BASSFlag.BASS_DEFAULT);
			
			GenerateWaveformData(path);
			
			if (_playingStream != 0) {
				
				long byteLength = -1;
				int channelSampleLength = -1;
				GetChannelInformation(_playingStream, out _sampleRate, out _bitsPerSample, out _channels, out byteLength, out channelSampleLength);
				ChannelSampleLength = channelSampleLength; // Notify
				
				// Set the stream to call Stop() when it ends.
				int syncHandle = Bass.BASS_ChannelSetSync(_playingStream,
				                                          BASSSync.BASS_SYNC_END,
				                                          0,
				                                          _endTrackSyncProc,
				                                          IntPtr.Zero);

				if (syncHandle == 0)
					throw new ArgumentException("Error establishing End Sync on file stream.", "path");
				
				_filePath = path;
				CanPlay = true;
			} else {
				CanPlay = false;
			}
		}
		
		public void SaveFile(string path) {
			SaveFile(this.WaveformData, path, this.Channels, this.SampleRate, this.BitsPerSample);
		}
		#endregion
		
		#region Public Play, Pause and Stop Methods
		public void Play() {
			if (CanPlay)
			{
				if (_isInitialized && _playingStream != 0) {
					Bass.BASS_ChannelPlay(_playingStream, false);
				}
				IsPlaying = true;
				CanPause = true;
				CanPlay = false;
				CanStop = true;
			}
		}

		public void Pause()
		{
			if (IsPlaying && CanPause)
			{
				if (_isInitialized && _playingStream != 0) {
					Bass.BASS_ChannelPause(_playingStream);
				}
				IsPlaying = false;
				CanPlay = true;
				CanPause = false;
			}
		}
		
		public void Stop()
		{
			ChannelSamplePosition = SelectionSampleBegin;
			if (_isInitialized && _playingStream != 0) {
				Bass.BASS_ChannelStop(_playingStream);
				SetChannelSamplePosition(_playingStream, ChannelSamplePosition);
			}
			IsPlaying = false;
			CanStop = false;
			CanPlay = true;
			CanPause = false;
		}
		#endregion

		#region Callbacks
		private void EndTrackSyncCallback(int handle, int channel, int data, IntPtr user)
		{
			Stop();
		}

		private void LoopSyncCallback(int handle, int channel, int data, IntPtr user)
		{
			//ChannelSamplePosition = SelectionSampleBegin;

			// Setting the loop position using the Set property for ChannelSamplePosition does
			// sometimes fail because of conflicting progress (timer update) at the same time
			// therefore do it directly:
			SetChannelSamplePosition(_playingStream, SelectionSampleBegin);
		}
		#endregion
		
		#region Public Properties
		public bool CanPlay
		{
			get { return _canPlay; }
			protected set
			{
				bool oldValue = _canPlay;
				_canPlay = value;
				if (oldValue != _canPlay)
					NotifyPropertyChanged("CanPlay");
			}
		}

		public bool CanPause
		{
			get { return _canPause; }
			protected set
			{
				bool oldValue = _canPause;
				_canPause = value;
				if (oldValue != _canPause)
					NotifyPropertyChanged("CanPause");
			}
		}

		public bool CanStop
		{
			get { return _canStop; }
			protected set
			{
				bool oldValue = _canStop;
				_canStop = value;
				if (oldValue != _canStop)
					NotifyPropertyChanged("CanStop");
			}
		}

		public bool IsPlaying
		{
			get { return _isPlaying; }
			protected set
			{
				bool oldValue = _isPlaying;
				_isPlaying = value;
				if (oldValue != _isPlaying)
					NotifyPropertyChanged("IsPlaying");

				_positionTimer.Enabled = value;
			}
		}
		#endregion
		
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}