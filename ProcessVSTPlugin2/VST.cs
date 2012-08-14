using System;
using System.Collections.Generic;
using System.Text;

using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;

using CommonUtils;

namespace ProcessVSTPlugin2
{
	public class VST
	{
		public VstPluginContext pluginContext = null;
		public event EventHandler<VSTStreamEventArgs> StreamCall=null;
		
		internal void Dispose()
		{
			if (pluginContext != null) pluginContext.Dispose();
		}
		
		public void MIDI_NoteOn(byte Note, byte Velocity)
		{
			byte Cmd = 0x90;
			MIDI(Cmd, Note, Velocity);
		}

		public void MIDI_CC(byte Number, byte Value)
		{
			byte Cmd = 0xB0;
			MIDI(Cmd, Number, Value);
		}
		
		private void MIDI(byte Cmd, byte Val1, byte Val2)
		{
			/* 
			 * Just a small note on the code for setting up a midi event:
			 * You can use the VstEventCollection class (Framework) to setup one or more events
			 * and then call the ToArray() method on the collection when passing it to
			 * ProcessEvents. This will save you the hassle of dealing with arrays explicitly.
			 * http://computermusicresource.com/MIDI.Commands.html
			 * 
			 * Freq to Midi notes etc:
			 * http://www.sengpielaudio.com/calculator-notenames.htm
			 * 
			 * Example to use NAudio Midi support
			 * http://stackoverflow.com/questions/6474388/naudio-and-midi-file-reading
			 */

			byte[] midiData = new byte[4];
			midiData[0] = Cmd;
			midiData[1] = Val1;
			midiData[2] = Val2;
			midiData[3] = 0;    // Reserved, unused

			VstMidiEvent vse = new VstMidiEvent(/*DeltaFrames*/ 0,
			                                    /*NoteLength*/ 0,
			                                    /*NoteOffset*/  0,
			                                    midiData,
			                                    /*Detune*/        0,
			                                    /*NoteOffVelocity*/ 127);

			VstEvent[] ve = new VstEvent[1];
			ve[0] = vse;

			pluginContext.PluginCommandStub.ProcessEvents(ve);
		}

		public void SetProgram(int programNumber) {
			if (programNumber < pluginContext.PluginInfo.ProgramCount && programNumber >= 0)
			{
				pluginContext.PluginCommandStub.SetProgram(programNumber);
			}
		}

		public string getPluginInfo() {
			if (pluginContext != null) {
				List<string> pluginInfo = new List<string>(); // Create new list of strings
				
				// plugin product
				pluginInfo.Add("Plugin Name " +  pluginContext.PluginCommandStub.GetEffectName());
				pluginInfo.Add("Product " +  pluginContext.PluginCommandStub.GetProductString());
				pluginInfo.Add("Vendor " +  pluginContext.PluginCommandStub.GetVendorString());
				pluginInfo.Add("Vendor Version " +  pluginContext.PluginCommandStub.GetVendorVersion().ToString());
				pluginInfo.Add("Vst Support " +  pluginContext.PluginCommandStub.GetVstVersion().ToString());
				pluginInfo.Add("Plugin Category " +  pluginContext.PluginCommandStub.GetCategory().ToString());
				
				// plugin info
				pluginInfo.Add("Flags " +  pluginContext.PluginInfo.Flags.ToString());
				pluginInfo.Add("Plugin ID " +  pluginContext.PluginInfo.PluginID.ToString());
				pluginInfo.Add("Plugin Version " +  pluginContext.PluginInfo.PluginVersion.ToString());
				pluginInfo.Add("Audio Input Count " +  pluginContext.PluginInfo.AudioInputCount.ToString());
				pluginInfo.Add("Audio Output Count " +  pluginContext.PluginInfo.AudioOutputCount.ToString());
				pluginInfo.Add("Initial Delay " +  pluginContext.PluginInfo.InitialDelay.ToString());
				pluginInfo.Add("Program Count " +  pluginContext.PluginInfo.ProgramCount.ToString());
				pluginInfo.Add("Parameter Count " +  pluginContext.PluginInfo.ParameterCount.ToString());
				pluginInfo.Add("Tail Size " + pluginContext.PluginCommandStub.GetTailSize().ToString());
				
				// can do
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Bypass + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.MidiProgramNames + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MidiProgramNames)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Offline + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstEvents + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstEvents)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstMidiEvent + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ReceiveVstTimeInfo + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstEvents + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstEvents)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstMidiEvent + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstMidiEvent)).ToString());
				
				pluginInfo.Add("CanDo: " + VstPluginCanDo.ConformsToWindowRules + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ConformsToWindowRules)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Metapass + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Metapass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.MixDryWet + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MixDryWet)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.Multipass + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Multipass)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.NoRealTime + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.PlugAsChannelInsert + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsChannelInsert)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.PlugAsSend + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsSend)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.SendVstTimeInfo + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x1in1out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in1out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x1in2out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in1out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in1out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in2out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x2in4out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in2out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in2out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in4out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x4in8out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in8out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x8in4out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in4out)).ToString());
				pluginInfo.Add("CanDo: " + VstPluginCanDo.x8in8out + pluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in8out)).ToString());
				
				pluginInfo.Add("Program: " + pluginContext.PluginCommandStub.GetProgram());
				pluginInfo.Add("Program Name: " + pluginContext.PluginCommandStub.GetProgramName());

				for (int i = 0; i < pluginContext.PluginInfo.ParameterCount; i++)
				{
					string name = pluginContext.PluginCommandStub.GetParameterName(i);
					string label = pluginContext.PluginCommandStub.GetParameterLabel(i);
					string display = pluginContext.PluginCommandStub.GetParameterDisplay(i);
					bool canBeAutomated = pluginContext.PluginCommandStub.CanParameterBeAutomated(i);
					
					pluginInfo.Add(String.Format("Parameter Index: {0} Parameter Name: {1} Display: {2} Label: {3} Can be automated: {4}", i, name, display, label, canBeAutomated));
				}
				return string.Join("\n", pluginInfo.ToArray());
			}
			return "Nothing";
		}
		
		private int EffSetChunk(byte[] data, bool isPreset) {
			bool beginSetProgramResult = pluginContext.PluginCommandStub.BeginSetProgram();
			int iResult = pluginContext.PluginCommandStub.SetChunk(data, isPreset);
			bool endSetProgramResult = pluginContext.PluginCommandStub.EndSetProgram();
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
			if ((pluginContext.PluginInfo.Flags & VstPluginFlags.ProgramChunks) == 0)
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
			int currentPluginID = pluginContext.PluginInfo.PluginID;
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
					
					//pluginContext.PluginCommandStub.SetProgramName(fxp.name);
					byte[] chunkData = fxp.chunkDataByteArray;
					int setChunkResult = EffSetChunk(chunkData, true);
				} else {
					// NB! non chunk presets are not supported yet!
					Console.Out.WriteLine("Presets with non-chunk data is not yet supported! Loading preset failed!");
					throw new System.NotSupportedException("Presets with non-chunk data is not yet supported! Loading preset failed!");
				}
			}
		}

		public void SaveFXP(string filePath) {
			FXP fxp = new FXP();
			fxp.chunkMagic = "CcnK";
			fxp.byteSize = 0; // will be set correctly by FXP class
			fxp.fxMagic = "FPCh"; // FPCh = FXP (preset), FBCh = FXB (bank)
			fxp.version = 1; // Format Version (should be 1)
			fxp.fxID = PluginIDNumberToIDString(pluginContext.PluginInfo.PluginID);
			fxp.fxVersion = pluginContext.PluginInfo.PluginVersion;
			fxp.numPrograms = pluginContext.PluginInfo.ProgramCount;
			fxp.name = pluginContext.PluginCommandStub.GetProgramName();
			
			byte[] chunkData = pluginContext.PluginCommandStub.GetChunk(true);
			fxp.chunkSize = chunkData.Length;
			fxp.chunkDataByteArray = chunkData;
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
		
		internal void Stream_ProcessCalled(object sender, VSTStreamEventArgs e)
		{
			if (StreamCall != null) StreamCall(sender, e);
		}
	}
}
