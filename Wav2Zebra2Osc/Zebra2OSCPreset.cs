﻿using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CommonUtils; // for MathUtils

namespace Wav2Zebra2Osc
{
	/// <summary>
	/// Class to read and write Zebra2 Oscillator Presets
	/// Typically stored in the Zebra2.data\Data\Modules\Oscillator folder
	/// </summary>
	public static class Zebra2OSCPreset
	{
		const string VERSION = "0.5";
		
		public static float[][] Read(string zebraPresetFilePath) {
			
			// search for
			// w1[0]=0.992;
			// or
			// Wave[ 0 ] = 1.000;
			// or
			// Wave[0] = -0.015853882;
			// or
			// oXw2[0]=0.467;
			
			// and for each 128 of them, add to slot

			if (File.Exists(zebraPresetFilePath)) {
				var soundData = new List<float[]>();

				string fileContent = File.ReadAllText(zebraPresetFilePath);
				
				// find all float values
				var floatValueOccurences = new Regex(@"\[\s*([0-9]+)\s*\]\s*=\s*([-+]?[0-9]*\.?[0-9]+)\s*;", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var floatValueMatches = floatValueOccurences.Matches(fileContent);
				
				int slot = 0;
				var waveform = new float[128];
				foreach (Match match in floatValueMatches)
				{
					//System.Diagnostics.Debug.WriteLine("{0}={1} '{2}'", match.Groups[1], match.Groups[2], match.Groups[0]);
					int index = int.Parse(match.Groups[1].Value);
					float value = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
					
					waveform[index] = value;
					
					if (index == 127) {
						soundData.Add(waveform);
						waveform = new float[128];
						slot++;
					}
				}
				return soundData.ToArray();
			}
			
			return null;
		}
		
		public static bool Write(float[][] soundData, bool[] enabledSlots, string zebraPresetFilePath) {
			
			try {
				using (var writer = new StreamWriter(zebraPresetFilePath)) {
					
					writer.WriteLine("/*@meta");
					writer.WriteLine("Author: Wav2Zebra2Osc");
					writer.WriteLine("Description: Generated by Wav2Zebra2Osc version {0}", VERSION);
					writer.WriteLine("             Copyleft Per Ivar Nerseth, 2015");
					writer.WriteLine("             https://github.com/perivar/AudioVSTToolbox/releases");
					writer.WriteLine("*/");
					writer.WriteLine();

					writer.WriteLine("#defaults=no");
					writer.WriteLine("#cm=OSC");
					writer.WriteLine("Wave=2"); // 2-Geoblend, 3-Spectroblend
					writer.WriteLine("<?");
					writer.WriteLine();
					writer.WriteLine("float Wave[ 128 ];");

					for (int j = 0; j < 16; j++) {
						if (enabledSlots[j]) {
							for (int i = 0; i < 128; i++) {
								float sampleValue = soundData[j][i];
								if ((soundData[j][i] < 0.001f)
								    && (soundData[j][i] > -0.001f)) {
									sampleValue = 0;
								}
								writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "Wave[ {0} ] = {1:0.0000};", i, sampleValue));
							}
							writer.WriteLine("Selected.WaveTable.set( {0} , Wave );", j + 1);
							writer.WriteLine();
						}
					}
					writer.WriteLine("?>");
				}
				
				return true;
			} catch (IOException e) {
				System.Diagnostics.Debug.WriteLine(e);
			}
			
			return false;
		}
		
		#region Utility Methods for Morphing
		
		/// <summary>
		/// Find all the segments and morph between them.
		/// E.g. if cell 0, 7 and 15 are loaded, this will mean two morphs:
		/// first between cell 0 and 7 and the second between cell 7 and 15
		/// </summary>
		public static void MorphAllSegments(bool[] enabledSlots, ref float[][] data)
		{
			int fromPos = 0;
			int toPos = 0;
			while (toPos < 16) {
				while ((toPos < 16) && enabledSlots[toPos]) {
					toPos++;
				}
				fromPos = toPos - 1;
				while ((toPos < 16) && !enabledSlots[toPos]) {
					toPos++;
				}
				if ((toPos < 16) && (fromPos >= 0)) {
					System.Diagnostics.Debug.WriteLineIf((fromPos < toPos), String.Format("Warning: from value ({0}) is less than to value ({1})", fromPos, toPos));
					MathUtils.Morph(ref data, fromPos, toPos);
				}
			}
		}
		
		#endregion
	}
}
