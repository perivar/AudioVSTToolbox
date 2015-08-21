using System;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;

using MidiVstTest;

// Copied from the microDRUM project
// https://github.com/microDRUM
// I think it is created by massimo.bernava@gmail.com
// Modified by perivar@nerseth.com
namespace CommonUtils.VSTPlugin
{
	public class VST
	{
		public VstPluginContext pluginContext = null;
		public event EventHandler<VSTStreamEventArgs> StreamCall=null;
		
		EditParametersForm edit = new EditParametersForm();

		internal void Dispose()
		{
			edit.Close();
			if(pluginContext!=null) pluginContext.Dispose();
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

		private void MIDI(byte Cmd,byte Val1,byte Val2)
		{
			var midiData = new byte[4];
			midiData[0] = Cmd;
			midiData[1] = Val1;
			midiData[2] = Val2;
			midiData[3] = 0;    // Reserved, unused

			var vse = new VstMidiEvent(/*DeltaFrames*/ 0,
			                           /*NoteLength*/ 0,
			                           /*NoteOffset*/  0,
			                           midiData,
			                           /*Detune*/        0,
			                           /*NoteOffVelocity*/ 127);

			var ve = new VstEvent[1];
			ve[0] = vse;

			pluginContext.PluginCommandStub.ProcessEvents(ve);
		}

		internal void ShowEditParameters()
		{
			edit.AddParameters(pluginContext);
			edit.Show();
		}

		internal void Stream_ProcessCalled(object sender, VSTStreamEventArgs e)
		{
			if (StreamCall != null) StreamCall(sender, e);
		}
	}
}
