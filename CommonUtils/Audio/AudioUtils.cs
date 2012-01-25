using System;
using System.Collections.Generic;

namespace CommonUtils.Audio
{
	/// <summary>
	/// Generic Audio Methods not dependent on NAudio or Bass.
	/// </summary>
	public static class AudioUtils
	{
		public enum LFOTIMING {
			LFO_8_1D = 0,
			LFO_8_1 = 1,
			LFO_4_1D = 2,
			LFO_8_1T = 3,
			LFO_4_1 = 4,
			LFO_2_1D = 5,
			LFO_4_1T = 6,
			LFO_2_1 = 7,
			LFO_1_1D = 8,
			LFO_2_1T = 9,
			LFO_1_1 = 10,
			LFO_1_2D = 11,
			LFO_1_1T = 12,
			LFO_1_2 = 13,
			LFO_1_4D = 14,
			LFO_1_2T = 15,
			LFO_1_4 = 16,
			LFO_1_8D = 17,
			LFO_1_4T = 18,
			LFO_1_8 = 19,
			LFO_1_16D = 20,
			LFO_1_8T = 21,
			LFO_1_16 = 22,
			LFO_1_32D = 23,
			LFO_1_16T = 24,
			LFO_1_32 = 25,
			LFO_1_64D = 26,
			LFO_1_32T = 27,
			LFO_1_64 = 28,
			LFO_1_128D = 29,
			LFO_1_64T = 30,
			LFO_1_128 = 31,
			LFO_1_256D = 32,
			LFO_1_128T = 33,
			LFO_1_256 = 34,
			LFO_1_256T = 35
		}
		
		public static float[] CropAudioAtSilence(float[] data, double threshold, bool onlyDetectEnd, int addSamples) {

			if (data == null || data.Length == 0) {
				return new float[0];
			}
			
			// process whole file
			int dataLength = data.Length;

			// start at beginning
			int position = 0;

			int beginning = 0;
			int end = data.Length;

			//double threshold = 0.000001; // what is silence
			double sampleValue = 0;

			int count = 0;
			int a = 0;
			if (!onlyDetectEnd) {
				
				// detect start silence
				// count silent samples
				for (a = 0; a < dataLength; a++) {
					sampleValue = Math.Abs(data[a]);
					if (sampleValue > threshold) break;
				}

				// add number of silent samples
				count += a;

				beginning = count;
				System.Diagnostics.Debug.WriteLine("Detected beginning at sample: {0}", beginning);
			}
			
			// detect end silence
			position = data.Length;
			int c = 0;
			while (position > count)
			{
				// step back a bit
				position=position < 20000 ? 0 : position - 20000;

				// count silent samples
				for (c = data.Length; c > 0 ; c--) {
					sampleValue = Math.Abs(data[c - 1]);
					if (sampleValue > threshold) break;
				}

				// if sound has begun...
				if ( c > 0 ) {
					// silence begins here
					//count = position + c;
					count = c;
					break;
				}
			}
			
			// set end position
			end = count;
			if ( (end + addSamples) > data.Length) {
				end = data.Length;
			} else {
				end = end + addSamples;
			}

			// Crop Audio here
			int croppedAudioLength = end-beginning;
			float[] croppedAudio = new float[croppedAudioLength];
			Array.Copy(data, beginning, croppedAudio, 0, croppedAudioLength);
			System.Diagnostics.Debug.WriteLine("Successfully cropping to selection: {0} to {1} (Original Length: {2} samples).", beginning, end, dataLength);
			return croppedAudio;
		}

		public static bool HasDataAboveThreshold(float[] data, double threshold) {
			if (data == null || data.Length == 0) {
				return false;
			}
			
			for (int i = 0; i < data.Length; i++) {
				if (data[i] > threshold) return true;
			}
			return false;
		}
		
		/// <summary>
		/// Retrieve the frequency in Ms for a Rhythmic Value for a given bpm value
		/// </summary>
		/// <param name="bpm"></param>
		public static double LFOOrDelayToMilliseconds(LFOTIMING timing, int bpm=120) {
			// http://classes.berklee.edu/mbierylo/mtec111_Pages/processing/time_based.html
			// see http://tomhess.net/Tools/DelayCalculator.aspx
			// 60,000 / BPM = Quarter beats
			// 40,000 / BPM = Triplets
			// 90,000 / BPM = Dotted
			double DelayTimeInMs = 60000 / bpm;
			
			// return value
			double delayMs = 0.0f;
			switch (timing) {
				case LFOTIMING.LFO_8_1D:
					double Delay8_1NoteDotted 		= DelayTimeInMs*6*8;
					delayMs = Delay8_1NoteDotted;
					break;
				case LFOTIMING.LFO_8_1:
					double Delay8_1Note 			= DelayTimeInMs*4*8;
					delayMs = Delay8_1Note;
					break;
				case LFOTIMING.LFO_8_1T:
					double Delay8_1NoteTriplet 		= DelayTimeInMs*8/3*8;
					delayMs = Delay8_1NoteTriplet;
					break;

				case LFOTIMING.LFO_4_1D:
					double Delay4_1NoteDotted 		= DelayTimeInMs*6*4;
					delayMs = Delay4_1NoteDotted;
					break;
				case LFOTIMING.LFO_4_1:
					double Delay4_1Note		 		= DelayTimeInMs*4*4;
					delayMs = Delay4_1Note;
					break;
				case LFOTIMING.LFO_4_1T:
					double Delay4_1NoteTriplet 		= DelayTimeInMs*8/3*4;
					delayMs = Delay4_1NoteTriplet;
					break;

				case LFOTIMING.LFO_2_1D:
					double Delay2_1NoteDotted 		= DelayTimeInMs*6*2;
					delayMs = Delay2_1NoteDotted;
					break;
				case LFOTIMING.LFO_2_1:
					double Delay2_1Note 			= DelayTimeInMs*4*2;
					delayMs = Delay2_1Note;
					break;
				case LFOTIMING.LFO_2_1T:
					double Delay2_1NoteTriplet 		= DelayTimeInMs*8/3*2;
					delayMs = Delay2_1NoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_1D:
					double DelayWholeNoteDotted 	= DelayTimeInMs*6;
					delayMs = DelayWholeNoteDotted;
					break;
				case LFOTIMING.LFO_1_1:
					double DelayWholeNote 			= DelayTimeInMs*4;
					delayMs = DelayWholeNote;
					break;
				case LFOTIMING.LFO_1_1T:
					double DelayWholeNoteTriplet 	= DelayTimeInMs*8/3;
					delayMs = DelayWholeNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_2D:
					double DelayHalfNoteDotted 		= DelayTimeInMs*6/2;
					delayMs = DelayHalfNoteDotted;
					break;
				case LFOTIMING.LFO_1_2:
					double DelayHalfNote 			= DelayTimeInMs*4/2;
					delayMs = DelayHalfNote;
					break;
				case LFOTIMING.LFO_1_2T:
					double DelayHalfNoteTriplet 	= DelayTimeInMs*8/3/2;
					delayMs = DelayHalfNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_4D:
					double DelayQuarterNoteDotted	= DelayTimeInMs*6/4;
					delayMs = DelayQuarterNoteDotted;
					break;
				case LFOTIMING.LFO_1_4:
					double DelayQuarterNote 		= DelayTimeInMs*4/4;
					delayMs = DelayQuarterNote;
					break;
				case LFOTIMING.LFO_1_4T:
					double DelayQuarterNoteTriplet 	= DelayTimeInMs*8/3/4;
					delayMs = DelayQuarterNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_8D:
					double DelayEighthNoteDotted 	= DelayTimeInMs*6/8;
					delayMs = DelayEighthNoteDotted;
					break;
				case LFOTIMING.LFO_1_8:
					double DelayEighthNote 			= DelayTimeInMs*4/8;
					delayMs = DelayEighthNote;
					break;
				case LFOTIMING.LFO_1_8T:
					double DelayEighthNoteTriplet 	= DelayTimeInMs*8/3/8;
					delayMs = DelayEighthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_16D:
					double DelaySixteenthNoteDotted = DelayTimeInMs*6/16;
					delayMs = DelaySixteenthNoteDotted;
					break;
				case LFOTIMING.LFO_1_16:
					double DelaySixteenthNote 		= DelayTimeInMs*4/16;
					delayMs = DelaySixteenthNote;
					break;
				case LFOTIMING.LFO_1_16T:
					double DelaySixteenthNoteTriplet = DelayTimeInMs*8/3/16;
					delayMs = DelaySixteenthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_32D:
					double Delay32NoteDotted 		= DelayTimeInMs*6/32;
					delayMs = Delay32NoteDotted;
					break;
				case LFOTIMING.LFO_1_32:
					double Delay32Note 				= DelayTimeInMs*4/32;
					delayMs = Delay32Note;
					break;
				case LFOTIMING.LFO_1_32T:
					double Delay32NoteTriplet 		= DelayTimeInMs*8/3/32;
					delayMs = Delay32NoteTriplet;
					break;

				case LFOTIMING.LFO_1_64D:
					double Delay64NoteDotted 		= DelayTimeInMs*6/64;
					delayMs = Delay64NoteDotted;
					break;
				case LFOTIMING.LFO_1_64:
					double Delay64Note 				= DelayTimeInMs*4/64;
					delayMs = Delay64Note;
					break;
				case LFOTIMING.LFO_1_64T:
					double Delay64NoteTriplet 		= DelayTimeInMs*8/3/64;
					delayMs = Delay64NoteTriplet;
					break;

				case LFOTIMING.LFO_1_128D:
					double Delay128NoteDotted 		= DelayTimeInMs*6/128;
					delayMs = Delay128NoteDotted;
					break;
				case LFOTIMING.LFO_1_128:
					double Delay128Note 			= DelayTimeInMs*4/128;
					delayMs = Delay128Note;
					break;
				case LFOTIMING.LFO_1_128T:
					double Delay128NoteTriplet 		= DelayTimeInMs*8/3/128;
					delayMs = Delay128NoteTriplet;
					break;

				case LFOTIMING.LFO_1_256D:
					double Delay256NoteDotted 		= DelayTimeInMs*6/256;
					delayMs = Delay256NoteDotted;
					break;
				case LFOTIMING.LFO_1_256:
					double Delay256Note 			= DelayTimeInMs*4/256;
					delayMs = Delay256Note;
					break;
				case LFOTIMING.LFO_1_256T:
					double Delay256NoteTriplet 		= DelayTimeInMs*8/3/256;
					delayMs = Delay256NoteTriplet;
					break;
			}

			double SecondsPerBar = DelayTimeInMs*4/1000;
			
			return delayMs;
		}
		
		/// <summary>
		/// Retrieve the frequency in Hz for a Rhythmic Value for a given bpm value
		/// </summary>
		/// <param name="bpm"></param>
		public static double LFOOrDelayToFrequency(LFOTIMING timing, int bpm=120) {
			// http://classes.berklee.edu/mbierylo/mtec111_Pages/processing/time_based.html
			// see http://tomhess.net/Tools/DelayCalculator.aspx
			// 60,000 / BPM = Quarter beats
			// 40,000 / BPM = Triplets
			// 90,000 / BPM = Dotted
			double DelayTimeInMs = 60000 / bpm;
			
			// return value
			double delayHz = 0.0f;
			switch (timing) {
				case LFOTIMING.LFO_8_1D:
					double Delay8_1NoteDotted 		= 1000/(DelayTimeInMs*6*8);
					delayHz = Delay8_1NoteDotted;
					break;
				case LFOTIMING.LFO_8_1:
					double Delay8_1Note 			= 1000/(DelayTimeInMs*4*8);
					delayHz = Delay8_1Note;
					break;
				case LFOTIMING.LFO_8_1T:
					double Delay8_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*8);
					delayHz = Delay8_1NoteTriplet;
					break;

				case LFOTIMING.LFO_4_1D:
					double Delay4_1NoteDotted 		= 1000/(DelayTimeInMs*6*4);
					delayHz = Delay4_1NoteDotted;
					break;
				case LFOTIMING.LFO_4_1:
					double Delay4_1Note		 		= 1000/(DelayTimeInMs*4*4);
					delayHz = Delay4_1Note;
					break;
				case LFOTIMING.LFO_4_1T:
					double Delay4_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*4);
					delayHz = Delay4_1NoteTriplet;
					break;

				case LFOTIMING.LFO_2_1D:
					double Delay2_1NoteDotted 		= 1000/(DelayTimeInMs*6*2);
					delayHz = Delay2_1NoteDotted;
					break;
				case LFOTIMING.LFO_2_1:
					double Delay2_1Note 			= 1000/(DelayTimeInMs*4*2);
					delayHz = Delay2_1Note;
					break;
				case LFOTIMING.LFO_2_1T:
					double Delay2_1NoteTriplet 		= 1000/(DelayTimeInMs*8/3*2);
					delayHz = Delay2_1NoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_1D:
					double DelayWholeNoteDotted 	= 1000/(DelayTimeInMs*6);
					delayHz = DelayWholeNoteDotted;
					break;
				case LFOTIMING.LFO_1_1:
					double DelayWholeNote 			= 1000/(DelayTimeInMs*4);
					delayHz = DelayWholeNote;
					break;
				case LFOTIMING.LFO_1_1T:
					double DelayWholeNoteTriplet 	= 1000/(DelayTimeInMs*8/3);
					delayHz = DelayWholeNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_2D:
					double DelayHalfNoteDotted 		= 1000/(DelayTimeInMs*6/2);
					delayHz = DelayHalfNoteDotted;
					break;
				case LFOTIMING.LFO_1_2:
					double DelayHalfNote 			= 1000/(DelayTimeInMs*4/2);
					delayHz = DelayHalfNote;
					break;
				case LFOTIMING.LFO_1_2T:
					double DelayHalfNoteTriplet 	= 1000/(DelayTimeInMs*8/3/2);
					delayHz = DelayHalfNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_4D:
					double DelayQuarterNoteDotted	= 1000/(DelayTimeInMs*6/4);
					delayHz = DelayQuarterNoteDotted;
					break;
				case LFOTIMING.LFO_1_4:
					double DelayQuarterNote 		= 1000/(DelayTimeInMs*4/4);
					delayHz = DelayQuarterNote;
					break;
				case LFOTIMING.LFO_1_4T:
					double DelayQuarterNoteTriplet 	= 1000/(DelayTimeInMs*8/3/4);
					delayHz = DelayQuarterNoteTriplet;
					break;
					
				case LFOTIMING.LFO_1_8D:
					double DelayEighthNoteDotted 	= 1000/(DelayTimeInMs*6/8);
					delayHz = DelayEighthNoteDotted;
					break;
				case LFOTIMING.LFO_1_8:
					double DelayEighthNote 			= 1000/(DelayTimeInMs*4/8);
					delayHz = DelayEighthNote;
					break;
				case LFOTIMING.LFO_1_8T:
					double DelayEighthNoteTriplet 	= 1000/(DelayTimeInMs*8/3/8);
					delayHz = DelayEighthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_16D:
					double DelaySixteenthNoteDotted = 1000/(DelayTimeInMs*6/16);
					delayHz = DelaySixteenthNoteDotted;
					break;
				case LFOTIMING.LFO_1_16:
					double DelaySixteenthNote 		= 1000/(DelayTimeInMs*4/16);
					delayHz = DelaySixteenthNote;
					break;
				case LFOTIMING.LFO_1_16T:
					double DelaySixteenthNoteTriplet = 1000/(DelayTimeInMs*8/3/16);
					delayHz = DelaySixteenthNoteTriplet;
					break;

				case LFOTIMING.LFO_1_32D:
					double Delay32NoteDotted 		= 1000/(DelayTimeInMs*6/32);
					delayHz = Delay32NoteDotted;
					break;
				case LFOTIMING.LFO_1_32:
					double Delay32Note 				= 1000/(DelayTimeInMs*4/32);
					delayHz = Delay32Note;
					break;
				case LFOTIMING.LFO_1_32T:
					double Delay32NoteTriplet 		= 1000/(DelayTimeInMs*8/3/32);
					delayHz = Delay32NoteTriplet;
					break;

				case LFOTIMING.LFO_1_64D:
					double Delay64NoteDotted 		= 1000/(DelayTimeInMs*6/64);
					delayHz = Delay64NoteDotted;
					break;
				case LFOTIMING.LFO_1_64:
					double Delay64Note 				= 1000/(DelayTimeInMs*4/64);
					delayHz = Delay64Note;
					break;
				case LFOTIMING.LFO_1_64T:
					double Delay64NoteTriplet 		= 1000/(DelayTimeInMs*8/3/64);
					delayHz = Delay64NoteTriplet;
					break;

				case LFOTIMING.LFO_1_128D:
					double Delay128NoteDotted 		= 1000/(DelayTimeInMs*6/128);
					delayHz = Delay128NoteDotted;
					break;
				case LFOTIMING.LFO_1_128:
					double Delay128Note 			= 1000/(DelayTimeInMs*4/128);
					delayHz = Delay128Note;
					break;
				case LFOTIMING.LFO_1_128T:
					double Delay128NoteTriplet 		= 1000/(DelayTimeInMs*8/3/128);
					delayHz = Delay128NoteTriplet;
					break;

				case LFOTIMING.LFO_1_256D:
					double Delay256NoteDotted 		= 1000/(DelayTimeInMs*6/256);
					delayHz = Delay256NoteDotted;
					break;
				case LFOTIMING.LFO_1_256:
					double Delay256Note 			= 1000/(DelayTimeInMs*4/256);
					delayHz = Delay256Note;
					break;
				case LFOTIMING.LFO_1_256T:
					double Delay256NoteTriplet 		= 1000/(DelayTimeInMs*8/3/256);
					delayHz = Delay256NoteTriplet;
					break;
			}
			return delayHz;
		}

		public static void OutputLFOTimings() {
			foreach (LFOTIMING timing in Enum.GetValues(typeof(LFOTIMING)))
			{
				Console.Out.WriteLine("{0}\t{1:0.000} ms\t{2:0.000} Hz", Enum.GetName(typeof(LFOTIMING), timing).PadRight(12), LFOOrDelayToMilliseconds(timing), AudioUtils.LFOOrDelayToFrequency(timing));
			}
		}
		
		#region Calculate Silence
		public class Silence : IComparable<Silence> {
			
			public int StartIndex {
				get; set;
			}
			
			public int EndIndex {
				get; set;
			}

			public int Length {
				get; set;
			}
			
			public Silence(int startIndex, int endIndex, int length) {
				StartIndex = startIndex;
				EndIndex = endIndex;
				Length = length;
			}
			
			public double Milliseconds(int sampleRate) {
				return Length / (double) sampleRate * 1000;
			}

			public double Hertz(int sampleRate) {
				return 1000 / (Length / (double) sampleRate * 1000);
			}
			
			public static Comparison<Silence> StartIndexComparison = delegate(Silence s1, Silence s2)
			{
				return s1.StartIndex.CompareTo(s2.StartIndex);
			};

			public static Comparison<Silence> EndIndexComparison = delegate(Silence s1, Silence s2)
			{
				return s1.EndIndex.CompareTo(s2.EndIndex);
			};
			
			#region IComparable<Silence> Members
			public int CompareTo(Silence other)
			{
				return Length.CompareTo(other.Length);
			}
			#endregion
			
			public override string ToString()
			{
				return string.Format("StartIndex: {0} EndIndex: {1} Length: {2}", StartIndex, EndIndex, Length);
			}
		}
		
		public static List<Silence> CalculateSilenceGaps(float[] data, float zero = 0.0f, int numberOfSamplesToConsider = 50) {
			
			// define some constants
			// float zero = 0.0001f;
			// int numberOfSamplesToConsider = 50;
			
			// list to store the silence's found
			List<Silence> silence = new List<Silence>();
			
			// internal
			bool isZero = false;
			int zeroStartIndex = -1;
			int zeroEndIndex = -1;
			for (int i = 0; i < data.Length; i++) {
				float sampleValue = Math.Abs(data[i]);
				
				if (isZero) {
					// detect "zero"
					if (sampleValue <= zero) {
						// count while "zero"
						zeroEndIndex++;
					} else {
						isZero = false;
						int zeroLength = zeroEndIndex - zeroStartIndex;
						if (zeroLength > numberOfSamplesToConsider) {
							// treat as found zero gap
							// store start and end index
							silence.Add(new Silence(zeroStartIndex, zeroEndIndex, zeroLength));
						}
					}
				} else {
					// detect "zero"
					if (sampleValue <= zero) {
						// "zero" found, store index
						isZero = true;
						zeroStartIndex = i;
						zeroEndIndex = i;
					} else {
						isZero = false;
					}
				}
			}
			
			return silence;
		}
		
		public static bool CalculateLFODelay(float[] data, int sampleRate, out double ms, out double hz, float zero = 0.0f, int numberOfSamplesToConsider = 50) {
			
			List<Silence> silence = CalculateSilenceGaps(data, zero, numberOfSamplesToConsider);
			if (silence.Count > 0) {
				silence.Sort();
				int[] silenceLengths = new int[silence.Count];
				int silenceCount = 0;
				foreach (Silence sil in silence) {
					//Console.Out.WriteLine("{0} {1:0.000} ms {2:0.000} hz", sil, sil.Milliseconds(sampleRate), sil.Hertz(sampleRate));
					silenceLengths[silenceCount] = sil.Length;
					silenceCount++;
				}
				double median = MathUtils.GetMedian(silenceLengths);
				int delaySamples = (int) median * 2;
				
				hz = SamplesToHertz(delaySamples, sampleRate);
				ms = SamplesToMilliseconds(delaySamples, sampleRate);
				
				return true;
			}
			
			hz = -1;
			ms = -1;
			return false;
		}
		#endregion
		
		public static double SamplesToMilliseconds(int samples, int sampleRate) {
			return samples / (double) sampleRate * 1000;
		}

		public static double SamplesToHertz(int samples, int sampleRate) {
			return 1000 / (samples / (double) sampleRate * 1000);
		}
	}
}
