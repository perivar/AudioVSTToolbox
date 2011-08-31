using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Jacobi.Vst.Core;
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
		
			int sampleCountx4 = (int) sampleCount * 4;
			int loopSize = (int) sampleCount / Channels;
			
			// 4 bytes per sample (32 bit)
			byte[] naudioBuf = new byte[sampleCountx4];
			int bytesRead = wavStream.Read(naudioBuf, 0, sampleCountx4);

			//System.Diagnostics.Debug.WriteLine(String.Format("{0} - Position: {1}, Time: {2}, Total Time: {3}", count, wavStream.Position, wavStream.CurrentTime, wavStream.TotalTime));
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
		
		public void SendMidiNoteWithProcessEvent() {
			/* 
			 * Just a small note on the code for setting up a midi event:
			 * You can use the VstEventCollection class (Framework) to setup one or more events
			 * and then call the ToArray() method on the collection when passing it to
			 * ProcessEvents. This will save you the hassle of dealing with arrays explicitly.
			 */
			byte[] midiData = new byte[4];
			midiData[0] = 144; // Send note on midi channel 1
			midiData[1] = 72;   // Midi note
			midiData[2] = 127; // Note strike velocity
			midiData[3] = 0;    // Reserved, unused
			
			VstMidiEvent vse = new VstMidiEvent(/*DeltaFrames*/ 0,
			                                    /*NoteLength*/ 	0,
			                                    /*NoteOffset*/ 	0,
			                                    midiData,
			                                    /*Detune*/    	0,
			                                    /*NoteOffVelocity*/ 127);
			
			VstEvent[] ve = new VstEvent[1];
			ve[0] = vse;
			
			PluginContext.PluginCommandStub.ProcessEvents(ve);
		}
		
		private void processMidiOnce(int volume, int pitch)
		{
			//PluginContext.PluginCommandStub.StartProcess();
			//VstEvent[] vEvent = createEvent(volume, pitch);
			//PluginContext.PluginCommandStub.ProcessEvents(vEvent);
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
				pluginInfo.Add("CanDo:" + VstPluginCanDo.Bypass+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Bypass)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.MidiProgramNames+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MidiProgramNames)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.Offline+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Offline)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.ReceiveVstEvents+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstEvents)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.ReceiveVstMidiEvent+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.ReceiveVstTimeInfo+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.SendVstEvents+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstEvents)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.SendVstMidiEvent+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstMidiEvent)).ToString());
				
				pluginInfo.Add("CanDo:" + VstPluginCanDo.ConformsToWindowRules+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ConformsToWindowRules)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.Metapass+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Metapass)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.MixDryWet+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.MixDryWet)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.Multipass+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.Multipass)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.NoRealTime+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.NoRealTime)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.PlugAsChannelInsert+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsChannelInsert)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.PlugAsSend+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.PlugAsSend)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.SendVstTimeInfo+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.SendVstTimeInfo)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x1in1out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in1out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x1in2out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x1in2out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x2in1out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in1out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x2in2out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in2out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x2in4out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x2in4out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x4in2out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in2out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x4in4out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in4out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x4in8out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x4in8out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x8in4out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in4out)).ToString());
				pluginInfo.Add("CanDo:" + VstPluginCanDo.x8in8out+ PluginContext.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.x8in8out)).ToString());
				
				return string.Join("\n", pluginInfo.ToArray());
			}
			return "Nothing";
		}
		
		public void ShowPluginEditor()
		{
			EditorFrame dlg = new EditorFrame();
			dlg.PluginCommandStub = PluginContext.PluginCommandStub;

			PluginContext.PluginCommandStub.MainsChanged(true);
			dlg.ShowDialog();
			PluginContext.PluginCommandStub.MainsChanged(false);
		}		
	}
}
