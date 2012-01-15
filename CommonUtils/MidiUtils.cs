using System;

namespace CommonUtils
{
	/// <summary>
	/// Description of MidiUtils.
	/// Alot of the midi methods as copied from
	/// http://pitchtracker.codeplex.com/
	/// </summary>
	public static class MidiUtils
	{
		public const byte CHANNEL1_NOTE_OFF = 128;
		public const byte CHANNEL2_NOTE_OFF = 129;
		public const byte CHANNEL3_NOTE_OFF = 130;
		public const byte CHANNEL4_NOTE_OFF = 131;
		public const byte CHANNEL5_NOTE_OFF = 132;
		public const byte CHANNEL6_NOTE_OFF = 133;
		public const byte CHANNEL7_NOTE_OFF = 134;
		public const byte CHANNEL8_NOTE_OFF = 135;
		public const byte CHANNEL9_NOTE_OFF = 136;
		public const byte CHANNEL10_NOTE_OFF = 137;
		public const byte CHANNEL11_NOTE_OFF = 138;
		public const byte CHANNEL12_NOTE_OFF = 139;
		public const byte CHANNEL13_NOTE_OFF = 140;
		public const byte CHANNEL14_NOTE_OFF = 141;
		public const byte CHANNEL15_NOTE_OFF = 142;
		public const byte CHANNEL16_NOTE_OFF = 143;
		public const byte CHANNEL1_NOTE_ON = 144;
		public const byte CHANNEL2_NOTE_ON = 145;
		public const byte CHANNEL3_NOTE_ON = 146;
		public const byte CHANNEL4_NOTE_ON = 147;
		public const byte CHANNEL5_NOTE_ON = 148;
		public const byte CHANNEL6_NOTE_ON = 149;
		public const byte CHANNEL7_NOTE_ON = 150;
		public const byte CHANNEL8_NOTE_ON = 151;
		public const byte CHANNEL9_NOTE_ON = 152;
		public const byte CHANNEL10_NOTE_ON = 153;
		public const byte CHANNEL11_NOTE_ON = 154;
		public const byte CHANNEL12_NOTE_ON = 155;
		public const byte CHANNEL13_NOTE_ON = 156;
		public const byte CHANNEL14_NOTE_ON = 157;
		public const byte CHANNEL15_NOTE_ON = 158;
		public const byte CHANNEL16_NOTE_ON = 159;
		
		public static readonly double InverseLog2 = 1.0 / Math.Log10(2.0);
		
		private const int kMinMidiNote = 21;  // 21 = A0
		private const int kMaxMidiNote = 127; // 108 = C8
		
		private static float minPitch = 8f;
		private static float maxPitch = 20000f;
		
		// Good table of MIDI NOTES AND FREQUENCIES
		// http://www.richardbrice.net/midi_notes.htm
		
		/// <summary>
		/// Get the MIDI note and cents of the pitch
		/// </summary>
		/// <param name="pitch"></param>
		/// <param name="note"></param>
		/// <param name="cents"></param>
		/// <returns></returns>
		public static bool PitchToMidiNote(float pitch, out int note, out int cents)
		{
			if (pitch < minPitch)
			{
				note = 0;
				cents = 0;
				return false;
			}

			var fNote = (float)((12.0 * Math.Log10(pitch / 55.0) * InverseLog2)) + 33.0f;
			note = (int)(fNote + 0.5f);
			cents = (int)((note - fNote) * 100);
			
			if (note < 0) {
				note = 0;
				cents = 0;
			} else if (note > 127) {
				note = 127;
				cents = 0;
			}
			return true;
		}

		/// <summary>
		/// Get the pitch from the MIDI note
		/// </summary>
		/// <param name="pitch"></param>
		/// <returns></returns>
		public static float PitchToMidiNote(float pitch)
		{
			if (pitch < minPitch)
				return 0.0f;

			return (float)(12.0 * Math.Log10(pitch / 55.0) * InverseLog2) + 33.0f;
		}

		/// <summary>
		/// Get the pitch from the MIDI note
		/// </summary>
		/// <param name="note"></param>
		/// <returns></returns>
		public static float MidiNoteToPitch(float note)
		{
			//if (note < 33.0f)
			//	return 0.0f;

			var pitch = (float)Math.Pow(10.0, (note - 33.0f) / InverseLog2 / 12.0f) * 55.0f;
			return pitch <= maxPitch ? pitch : 0.0f;
		}
		
		/// <summary>
		/// Format a midi note to text
		/// </summary>
		/// <param name="note"></param>
		/// <param name="sharps"></param>
		/// <param name="showOctave"></param>
		/// <returns></returns>
		public static string GetNoteName(int note, bool sharps, bool showOctave)
		{
			if (note < kMinMidiNote || note > kMaxMidiNote)
				return null;

			note -= kMinMidiNote;

			int octave = (note + 9) / 12;
			note = note % 12;
			string noteText = null;

			switch (note)
			{
				case 0:
					noteText = "A";
					break;

				case 1:
					noteText = sharps ? "A#" : "Bb";
					break;

				case 2:
					noteText = "B";
					break;

				case 3:
					noteText = "C";
					break;

				case 4:
					noteText = sharps ? "C#" : "Db";
					break;

				case 5:
					noteText = "D";
					break;

				case 6:
					noteText = sharps ? "D#" : "Eb";
					break;

				case 7:
					noteText = "E";
					break;

				case 8:
					noteText = "F";
					break;

				case 9:
					noteText = sharps ? "F#" : "Gb";
					break;

				case 10:
					noteText = "G";
					break;

				case 11:
					noteText = sharps ? "G#" : "Ab";
					break;
			}

			if (showOctave) {
				//noteText += " " + octave.ToString();
				noteText += octave.ToString();
			}

			return noteText;
		}
	}
}
