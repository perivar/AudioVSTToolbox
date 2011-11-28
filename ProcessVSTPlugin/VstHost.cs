using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Plugin;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;

using NAudio;
using NAudio.Wave;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of VstHost.
	/// </summary>
	public sealed class VstHost
	{
		private int count = 0;
		
		private static readonly VstHost instance = new VstHost();

		public VstAudioBuffer[] vstInputBuffers = null;
		public VstAudioBuffer[] vstOutputBuffers = null;
		public VstPluginContext PluginContext { get; set; }
		
		public int BlockSize { get; set; }
		public int SampleRate { get; set; }
		public int Channels { get; set; }
		
		private WaveChannel32 wavStream;
		private WaveFileReader wavFileReader;
		
		private int tailWaitForNumberOfSeconds = 5;
		
		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static VstHost()
		{
		}
		
		private VstHost()
		{
		}
		
		public static VstHost Instance
		{
			get
			{
				return instance;
			}
		}
		
		public string InputWave
		{
			set
			{
				// 4 bytes per sample (32 bit)
				this.wavFileReader = new WaveFileReader(value);
				this.wavStream = new WaveChannel32(this.wavFileReader);
				this.wavStream.Volume = 1f;
			}
		}

		public int TailWaitForNumberOfSeconds
		{
			get
			{
				return this.tailWaitForNumberOfSeconds;
			}
			set
			{
				this.tailWaitForNumberOfSeconds = value;
			}
		}
		
		public void OpenPlugin(string pluginPath)
		{
			try
			{
				HostCommandStub hostCmdStub = new HostCommandStub();
				//hostCmdStub.PluginCalled += new EventHandler<PluginCalledEventArgs>(HostCmdStub_PluginCalled);

				VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);

				// add custom data to the context
				ctx.Set("PluginPath", pluginPath);
				ctx.Set("HostCmdStub", hostCmdStub);

				// actually open the plugin itself
				ctx.PluginCommandStub.Open();

				PluginContext = ctx;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
		}

		private void ReleasePlugin()
		{
			// dispose of all (unmanaged) resources
			PluginContext.Dispose();
		}
		
		public void Init(int blockSize, int sampleRate, int channels)
		{
			int inputCount = PluginContext.PluginInfo.AudioInputCount;
			int outputCount = PluginContext.PluginInfo.AudioOutputCount;
			this.BlockSize = blockSize;
			this.SampleRate = sampleRate;
			this.Channels = channels;
			
			InitBuffer(inputCount, outputCount, blockSize, sampleRate);
		}
		
		private void InitBuffer(int inputCount, int outputCount, int blockSize, int sampleRate)
		{
			VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize);
			VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize);
			
			this.vstInputBuffers = inputMgr.ToArray();
			this.vstOutputBuffers = outputMgr.ToArray();

			this.PluginContext.PluginCommandStub.SetBlockSize(blockSize);
			this.PluginContext.PluginCommandStub.SetSampleRate((float)sampleRate);
			this.PluginContext.PluginCommandStub.SetProcessPrecision(VstProcessPrecision.Process32);
		}
		
		// This function fills vstOutputBuffers with audio processed by a plugin
		public int ProcessReplacing(uint sampleCount) {

			// check if we are processing a wavestrem or if this is audio outputting only (VSTi?)
			if (wavStream != null) {
				int sampleCountx4 = (int) sampleCount * 4;
				int loopSize = (int) sampleCount / Channels;
				
				// 4 bytes per sample (32 bit)
				byte[] naudioBuf = new byte[sampleCountx4];
				int bytesRead = wavStream.Read(naudioBuf, 0, sampleCountx4);

				if (wavStream.CurrentTime > wavStream.TotalTime.Add(TimeSpan.FromSeconds(tailWaitForNumberOfSeconds))) {
					return 0;
				}
				
				unsafe
				{
					fixed (byte* byteBuf = &naudioBuf[0])
					{
						float* floatBuf = (float*)byteBuf;
						int j = 0;
						for (int i = 0; i < loopSize; i++)
						{
							vstInputBuffers[0][i] = *(floatBuf + j);
							j++;
							vstInputBuffers[1][i] = *(floatBuf + j);
							j++;
						}
					}
				}
			}
			
			// The calls to Mainschanged and Start/Stop Process should be made only once, not for every cycle in the audio processing.
			// So it should look something like:
			// 
			// [plugin.Open()]
			// plugin.MainsChanged(true) // turn on 'power' on plugin.
			// plugin.StartProcess() // let the plugin know the audio engine has started
			// PluginContext.PluginCommandStub.ProcessEvents(ve); // process events (like VstMidiEvent)
			// 
			// while(audioEngineIsRunning)
			// {
			//     plugin.ProcessReplacing(inputBuffers, outputBuffers)  // letplugin process audio stream
			// }
			// 
			// plugin.StopProcess()
			// plugin.MainsChanged(false)
			// 
			// [plugin.Close()]
			
			
			// Open Resources
			//PluginContext.PluginCommandStub.MainsChanged(true);
			//PluginContext.PluginCommandStub.StartProcess();
			
			PluginContext.PluginCommandStub.ProcessReplacing(vstInputBuffers, vstOutputBuffers);
			
			// Close resources
			//PluginContext.PluginCommandStub.StopProcess();
			//PluginContext.PluginCommandStub.MainsChanged(false);
			
			count++;
			return (int) sampleCount;
		}
		
		public void SaveWavFile(string fileName) {
			WaveFileWriter.CreateWaveFile(fileName, this.wavStream);
		}
		
		public void SetProgram(int programNumber) {
			if (programNumber < PluginContext.PluginInfo.ProgramCount && programNumber >= 0)
			{
				PluginContext.PluginCommandStub.SetProgram(programNumber);
			}
		}
		
		private VstEvent[] CreateMidiEvent(byte midiNote, byte midiVelocity) {
			/* 
			 * Just a small note on the code for setting up a midi event:
			 * You can use the VstEventCollection class (Framework) to setup one or more events
			 * and then call the ToArray() method on the collection when passing it to
			 * ProcessEvents. This will save you the hassle of dealing with arrays explicitly.
			 * http://computermusicresource.com/MIDI.Commands.html
			 */
			byte[] midiData = new byte[4];
			
			midiData[0] = 144; 			// Send note on midi channel 1
			midiData[1] = midiNote;   	// Midi note
			midiData[2] = midiVelocity; // Note strike velocity
			midiData[3] = 0;    		// Reserved, unused
			
			VstMidiEvent vse1 = new VstMidiEvent(/*DeltaFrames*/ 	0,
			                                     /*NoteLength*/ 	0,
			                                     /*NoteOffset*/ 	0,
			                                     midiData,
			                                     /*Detune*/    		0,
			                                     /*NoteOffVelocity*/ 0);
			
			VstEvent[] ve = new VstEvent[1];
			ve[0] = vse1;
			return ve;
		}

		public void SendMidiNote(byte midiNote, byte midiVelocity) {
			
			//PluginContext.PluginCommandStub.MainsChanged(true);
			//PluginContext.PluginCommandStub.StartProcess();
			
			VstEvent[] vEvent = CreateMidiEvent(midiNote, midiVelocity);
			PluginContext.PluginCommandStub.ProcessEvents(vEvent);
			//PluginContext.PluginCommandStub.ProcessReplacing(vstInputBuffers, vstOutputBuffers);
			
			//PluginContext.PluginCommandStub.StopProcess();
			//PluginContext.PluginCommandStub.MainsChanged(false);
		}
		
		public string getPluginInfo() {
			if (PluginContext != null) {
				List<string> pluginInfo = new List<string>(); // Create new list of strings
				
				// plugin product
				pluginInfo.Add("Plugin Name " +  PluginContext.PluginCommandStub.GetEffectName());
				pluginInfo.Add("Product " +  PluginContext.PluginCommandStub.GetProductString());
				pluginInfo.Add("Vendor " +  PluginContext.PluginCommandStub.GetVendorString());
				pluginInfo.Add("Vendor Version " +  PluginContext.PluginCommandStub.GetVendorVersion().ToString());
				pluginInfo.Add("Vst Support " +  PluginContext.PluginCommandStub.GetVstVersion().ToString());
				pluginInfo.Add("Plugin Category " +  PluginContext.PluginCommandStub.GetCategory().ToString());
				
				// plugin info
				pluginInfo.Add("Flags " +  PluginContext.PluginInfo.Flags.ToString());
				pluginInfo.Add("Plugin ID " +  PluginContext.PluginInfo.PluginID.ToString());
				pluginInfo.Add("Plugin Version " +  PluginContext.PluginInfo.PluginVersion.ToString());
				pluginInfo.Add("Audio Input Count " +  PluginContext.PluginInfo.AudioInputCount.ToString());
				pluginInfo.Add("Audio Output Count " +  PluginContext.PluginInfo.AudioOutputCount.ToString());
				pluginInfo.Add("Initial Delay " +  PluginContext.PluginInfo.InitialDelay.ToString());
				pluginInfo.Add("Program Count " +  PluginContext.PluginInfo.ProgramCount.ToString());
				pluginInfo.Add("Parameter Count " +  PluginContext.PluginInfo.ParameterCount.ToString());
				pluginInfo.Add("Tail Size " + PluginContext.PluginCommandStub.GetTailSize().ToString());
				
				// can do
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Bypass + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.MidiProgramNames + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MidiProgramNames)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Offline + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstEvents + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstEvents)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstMidiEvent + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstTimeInfo + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstEvents + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstEvents)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstMidiEvent + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstMidiEvent)).ToString());
				
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ConformsToWindowRules + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ConformsToWindowRules)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Metapass + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Metapass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.MixDryWet + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MixDryWet)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Multipass + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Multipass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.NoRealTime + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.PlugAsChannelInsert + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsChannelInsert)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.PlugAsSend + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsSend)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstTimeInfo + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x1in1out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in1out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x1in2out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in1out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in1out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in2out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in4out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in2out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in4out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in8out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in8out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x8in4out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x8in8out + PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in8out)).ToString());
				
				pluginInfo.Add("Program: " + PluginContext.PluginCommandStub.GetProgram());
				pluginInfo.Add("Program Name: " + PluginContext.PluginCommandStub.GetProgramName());

				for (int i = 0; i < PluginContext.PluginInfo.ParameterCount; i++)
				{
					string name = PluginContext.PluginCommandStub.GetParameterName(i);
					string label = PluginContext.PluginCommandStub.GetParameterLabel(i);
					string display = PluginContext.PluginCommandStub.GetParameterDisplay(i);
					
					pluginInfo.Add(String.Format("Parameter Name: {0} Display: {1} Label: {2}", name, display, label));
				}
				return string.Join("\n", pluginInfo.ToArray());
			}
			return "Nothing";
		}
		
		public void ShowPluginEditor()
		{
			EditorFrame dlg = new EditorFrame();
			dlg.PluginContext = PluginContext;

			PluginContext.PluginCommandStub.MainsChanged(true);
			dlg.ShowDialog();
			PluginContext.PluginCommandStub.MainsChanged(false);
		}

		private int EffSetChunk(byte[] data, bool isPreset) {
			bool beginSetProgramResult = PluginContext.PluginCommandStub.BeginSetProgram();
			int iResult = PluginContext.PluginCommandStub.SetChunk(data, isPreset);
			bool endSetProgramResult = PluginContext.PluginCommandStub.EndSetProgram();
			return iResult;
		}

		public void LoadFXP(string filePath) {
			if (filePath == null || filePath == "") {
				return;
			}
			// How does the GetChunk/SetChunk interface work? What information should be in those chunks?
			// How does the BeginLoadProgram and BeginLoadBank work?
			// There doesn't seem to be any restriction on what data is put in the chunks.
			// The beginLoadBank/Program methods are also part of the persistence call sequence.
			// GetChunk returns a buffer with program information of either the current/active program
			// or all programs.
			// SetChunk should read this information back in and initialize either the current/active program
			// or all programs.
			// Before SetChunk is called, the beginLoadBank/Program method is called
			// passing information on the version of the plugin that wrote the data.
			// This will allow you to support older data versions of your plugin's data or
			// even support reading other plugin's data.
			// Some hosts will call GetChunk before calling beginLoadBakn/Program and SetChunk.
			// This is an optimazation of the host to determine if the information to load is
			// actually different than the state your plugin program(s) (are) in.

			bool UseChunk = false;
			if ((PluginContext.PluginInfo.Flags & VstPluginFlags.ProgramChunks) == 0)
			{
				// Chunks not supported.
				UseChunk = false;
			}
			else
			{
				// Chunks supported.
				UseChunk = true;
			}
			FXP fxp = new FXP();
			fxp.ReadFile(filePath);
			if (fxp.chunkMagic != "CcnK")
			{
				// not a fxp or fxb file
				Console.Out.WriteLine("Error - Cannot Load. Loaded preset is not a fxp or fxb file");
				return;
			}

			int pluginUniqueID = PluginIDStringToIDNumber(fxp.fxID);
			int currentPluginID = PluginContext.PluginInfo.PluginID;
			if (pluginUniqueID != currentPluginID) {
				Console.Out.WriteLine("Error - Cannot Load. Loaded preset has another ID!");
			} else {
				// Preset (Program) (.fxp) with chunk (magic = 'FPCh')
				// Bank (.fxb) with chunk (magic = 'FBCh')
				if (fxp.fxMagic == "FPCh" || fxp.fxMagic == "FBCh") {
					UseChunk = true;
				} else {
					UseChunk = false;
				}
				if (UseChunk) {
					// If your plug-in is configured to use chunks
					// the Host will ask for a block of memory describing the current
					// plug-in state for saving.
					// To restore the state at a later stage, the same data is passed
					// back to setChunk.
					// Alternatively, when not using chunk, the Host will simply
					// save all parameter values.
					
					//PluginContext.PluginCommandStub.SetProgramName(fxp.name);
					byte[] chunkData = BinaryFile.StringToByteArray(fxp.chunkData);
					int setChunkResult = EffSetChunk(chunkData, true);
				} else {
					// NB! non chunk presets are not supported yet!
					Console.Out.WriteLine("Presets with non-chunk data is not yet supported! Loading preset failed!");
					return;
					
					int version = fxp.version; // should be 1
					int pluginVersion = fxp.fxVersion;
					int numElements = fxp.numPrograms;
					VstPatchChunkInfo chunkInfo = new VstPatchChunkInfo(version, pluginUniqueID, pluginVersion, numElements);
					
					// BeginLoadProgram()
					// Called before a Program is loaded, points to VstPatchChunkInfo structure
					// return -1 if the Program can not be loaded, return 1 if it can be loaded else 0 (for compatibility)
					// Called before BeginSetProgram.
					VstCanDoResult beginLoadProgramReturn = PluginContext.PluginCommandStub.BeginLoadProgram(chunkInfo);
					if (beginLoadProgramReturn == Jacobi.Vst.Core.VstCanDoResult.Yes) {
						// BeginSetProgram()
						// Host calls this before a new program (SetProgram) is loaded
						// x[return]: 1 = the plug-in took the notification into account
						bool beginSetProgramReturn = PluginContext.PluginCommandStub.BeginSetProgram();
						if (beginSetProgramReturn) {
							// SetProgram()
							// host has changed the current program number
							// host must call this inside a beginSetProgram() ... endSetProgram() sequence
							// e[value]: program number
							for (int i = 0; i < fxp.numPrograms; i++) {
								PluginContext.PluginCommandStub.SetProgram(i);
								PluginContext.PluginCommandStub.SetProgramName(fxp.name);
								
								int numParm = 0;
								for (int j = 0; j < numParm; i++) {
									int parameterValue = 10;
									PluginContext.PluginCommandStub.SetParameter(j, parameterValue);
								}
							}
							
							// called when the program is loaded
							bool endSetProgramReturn = PluginContext.PluginCommandStub.EndSetProgram();
							if (endSetProgramReturn) {
								int index = PluginContext.PluginCommandStub.GetProgram();
							} else {
								Console.Out.WriteLine("Error - Cannot Load. EndSetProgram failed");
							}
						} else {
							Console.Out.WriteLine("Error - Cannot Load. BeginSetProgram failed");
						}
					} else {
						Console.Out.WriteLine("Error - Cannot Load. BeginLoadProgram failed");
					}
				}
			}
		}

		public void SaveFXP(string filePath) {
			FXP fxp = new FXP();
			fxp.chunkMagic = "CcnK";
			fxp.byteSize = 0; // will be set correctly by FXP class
			fxp.fxMagic = "FPCh"; // FPCh = FXP (preset), FBCh = FXB (bank)
			fxp.version = 1; // Format Version (should be 1)
			fxp.fxID = PluginIDNumberToIDString(PluginContext.PluginInfo.PluginID);
			fxp.fxVersion = PluginContext.PluginInfo.PluginVersion;
			fxp.numPrograms = PluginContext.PluginInfo.ProgramCount;
			fxp.name = PluginContext.PluginCommandStub.GetProgramName();
			
			byte[] chunkData = PluginContext.PluginCommandStub.GetChunk(true);
			fxp.chunkSize = chunkData.Length;
			fxp.chunkData = BinaryFile.ByteArrayToString(chunkData);
			
			fxp.WriteFile(filePath);
		}
		
		private static string PluginIDNumberToIDString(int pluginUniqueID) {
			byte[] fxIdArray = BitConverter.GetBytes(pluginUniqueID);
			Array.Reverse(fxIdArray);
			string fxIdString = BinaryFile.ByteArrayToString(fxIdArray);
			return fxIdString;
		}

		private static int PluginIDStringToIDNumber(string fxIdString) {
			byte[] pluginUniqueIDArray = BinaryFile.StringToByteArray(fxIdString); // 58h8 = 946354229
			Array.Reverse(pluginUniqueIDArray);
			int pluginUniqueID = BitConverter.ToInt32(pluginUniqueIDArray, 0);
			return pluginUniqueID;
		}
	}
}
