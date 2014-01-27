using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

using PresetConverter;
using CommonUtils;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of UADSSLChannel.
	/// </summary>
	public class UADSSLChannel : Preset
	{
		string FilePath;
		public string PresetName;
		public int PresetHeaderVar1 = 3;
		public int PresetHeaderVar2 = 2;
		
		#region Parameter Variable Names
		public float Input;      // (-20.0 dB -> 20.0 dB)
		public float Phase;      // (Normal -> Inverted)
		public float HPFreq;     // (Out -> 304 Hz)
		public float LPFreq;     // (Out -> 3.21 k)
		public float HP_LPDynSC; // (Off -> On)
		public float CMPRatio;   // (1.00:1 -> Limit)
		public float CMPThresh;  // (10.0 dB -> -20.0 dB)
		public float CMPRelease; // (0.10 s -> 4.00 s)
		public float CMPAttack;  // (Auto -> Fast)
		public float StereoLink; // (UnLink -> Link)
		public float Select;     // (Expand -> Gate 2)
		public float EXPThresh;  // (-30.0 dB -> 10.0 dB)
		public float EXPRange;   // (0.0 dB -> 40.0 dB)
		public float EXPRelease; // (0.10 s -> 4.00 s)
		public float EXPAttack;  // (Auto -> Fast)
		public float DYNIn;      // (Out -> In)
		public float CompIn;     // (Out -> In)
		public float ExpIn;      // (Out -> In)
		public float LFGain;     // (-10.0 dB -> 10.0 dB)
		public float LFFreq;     // (36.1 Hz -> 355 Hz)
		public float LFBell;     // (Shelf -> Bell)
		public float LMFGain;    // (-15.6 dB -> 15.6 dB)
		public float LMFFreq;    // (251 Hz -> 2.17 k)
		public float LMFQ;       // (2.50 -> 2.50)
		public float HMFQ;       // (4.00 -> 0.40)
		public float HMFGain;    // (-16.5 dB -> 16.5 dB)
		public float HMFFreq;    // (735 Hz -> 6.77 k)
		public float HFGain;     // (-16.0 dB -> 16.1 dB)
		public float HFFreq;     // (6.93 k -> 21.7 k)
		public float HFBell;     // (Shelf -> Bell)
		public float EQIn;       // (Out -> In)
		public float EQDynSC;    // (Off -> On)
		public float PreDyn;     // (Off -> On)
		public float Output;     // (-20.0 dB -> 20.0 dB)
		public float EQType;     // (Black -> Brown)
		public float Power;      // (Off -> On)
		#endregion
		
		// lists to store lookup values
		Dictionary<string, List<string>> displayTextDict = new Dictionary<string, List<string>>();
		Dictionary<string, List<float>> displayNumbersDict = new Dictionary<string, List<float>>();
		Dictionary<string, List<float>> valuesDict = new Dictionary<string, List<float>>();
		
		public UADSSLChannel()
		{
			InitializeMappingTables(@"C:\Users\perivar.nerseth\Documents\My Projects\AudioVSTToolbox\InvestigatePresetFileDump\ParametersMap.xml");
		}
		
		#region FindClosest Example Methods
		/*
			var numbers = new List<float> { 10f, 20f, 22f, 30f };
			var target = 21f;

			//gets single number which is closest
			var closest = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.First().n;

			//get two closest
			var take = 2;
			var closests = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.Select( p => p.n )
				.Take( take );

			//gets any that are within x of target
			var within = 1;
			var withins = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.Where( p => p.distance <= within )
				.Select( p => p.n );
		 */
		#endregion

		#region FindClosest and Dependent Methods
		private void InitializeMappingTables(string xmlfilename) {
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			
			var entries = (from entry in xmlDoc.Descendants("Entry")
			               group entry by (string) entry.Parent.Attribute("name").Value into g
			               select new {
			               	g.Key,
			               	Value = g.ToList()
			               });

			displayTextDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("DisplayText").Select(p => p.Value).ToList());
			displayNumbersDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("DisplayNumber").Select(p => (float)GetDouble(p.Value, 0)).ToList());
			valuesDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("Value").Select(p => float.Parse(p.Value)).ToList());
		}
		
		/// <summary>
		/// Search for the display value that is closest to the passed parameter and return the float value
		/// </summary>
		/// <param name="paramName">parameter to search within</param>
		/// <param name="searchDisplayValue">display value (e.g. '2500' from the 2.5 kHz)</param>
		/// <returns>the float value (between 0 - 1) that corresponds to the closest match</returns>
		public float FindClosestValue(string paramName, float searchDisplayValue) {
			
			// find closest float value
			float foundClosest = displayNumbersDict[paramName].Aggregate( (x,y) => Math.Abs(x - searchDisplayValue) < Math.Abs(y - searchDisplayValue) ? x : y);
			int foundIndex = displayNumbersDict[paramName].IndexOf(foundClosest);
			string foundClosestDisplayText = displayTextDict[paramName][foundIndex];
			float foundParameterValue = valuesDict[paramName][foundIndex];
			
			//Console.Out.WriteLine("Searching '{0}' for value {1}. Found {2} with text '{3}'. Value = {4}", paramName, searchDisplayValue, foundClosest, foundClosestDisplayText, foundParameterValue);
			
			return foundParameterValue;
		}

		/// <summary>
		/// Search for the float value that is closest to the passed parameter and return the display text
		/// </summary>
		/// <param name="paramName">parameter to search within</param>
		/// <param name="searchParamValue">float value (between 0 - 1)</param>
		/// <returns>the display text that corresponds to the closest match</returns>
		public string FindClosestDisplayText(string paramName, float searchParamValue) {

			// find closest display text
			float foundClosest = valuesDict[paramName].Aggregate( (x,y) => Math.Abs(x - searchParamValue) < Math.Abs(y - searchParamValue) ? x : y);
			int foundIndex = valuesDict[paramName].IndexOf(foundClosest);
			string foundClosestDisplayText = displayTextDict[paramName][foundIndex];
			float foundParameterValue = valuesDict[paramName][foundIndex];
			
			//Console.Out.WriteLine("Searching '{0}' for value {1}. Found {2} with text '{3}'. Value = {4}", paramName, searchParamValue, foundClosest, foundClosestDisplayText, foundParameterValue);
			
			return foundClosestDisplayText;
		}
		
		private static double GetDouble(string value, double defaultValue)
		{
			double result;

			// Try parsing in the current culture
			if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
			    // Then try in US english
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
			    // Then in neutral language
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				result = defaultValue;
			}
			
			// special case
			if (result == 3.911555E-07) result = 0;

			return result;
		}
		#endregion
		
		#region Read and Write Methods
		public bool ReadFXP(FXP fxp, string filePath="")
		{
			BinaryFile bFile = new BinaryFile(fxp.ChunkDataByteArray, BinaryFile.ByteOrder.LittleEndian);

			// Read UAD Preset Header information
			PresetHeaderVar1 = bFile.ReadInt32();
			PresetHeaderVar2 = bFile.ReadInt32();
			PresetName = bFile.ReadString(32).Trim('\0');
			
			// Read Parameters
			Input = bFile.ReadSingle();
			Phase = bFile.ReadSingle();
			HPFreq = bFile.ReadSingle();
			LPFreq = bFile.ReadSingle();
			HP_LPDynSC = bFile.ReadSingle();
			CMPRatio = bFile.ReadSingle();
			CMPThresh = bFile.ReadSingle();
			CMPRelease = bFile.ReadSingle();
			CMPAttack = bFile.ReadSingle();
			StereoLink = bFile.ReadSingle();
			Select = bFile.ReadSingle();
			EXPThresh = bFile.ReadSingle();
			EXPRange = bFile.ReadSingle();
			EXPRelease = bFile.ReadSingle();
			EXPAttack = bFile.ReadSingle();
			DYNIn = bFile.ReadSingle();
			CompIn = bFile.ReadSingle();
			ExpIn = bFile.ReadSingle();
			LFGain = bFile.ReadSingle();
			LFFreq = bFile.ReadSingle();
			LFBell = bFile.ReadSingle();
			LMFGain = bFile.ReadSingle();
			LMFFreq = bFile.ReadSingle();
			LMFQ = bFile.ReadSingle();
			HMFQ = bFile.ReadSingle();
			HMFGain = bFile.ReadSingle();
			HMFFreq = bFile.ReadSingle();
			HFGain = bFile.ReadSingle();
			HFFreq = bFile.ReadSingle();
			HFBell = bFile.ReadSingle();
			EQIn = bFile.ReadSingle();
			EQDynSC = bFile.ReadSingle();
			PreDyn = bFile.ReadSingle();
			Output = bFile.ReadSingle();
			EQType = bFile.ReadSingle();
			Power = bFile.ReadSingle();
			
			return false;
		}
		
		public bool WriteFXP(string filePath) {

			FXP fxp = new FXP();
			fxp.ChunkMagic = "CcnK";
			fxp.ByteSize = 0; // will be set correctly by FXP class
			
			// Preset (Program) (.fxp) with chunk (magic = 'FPCh')
			fxp.FxMagic = "FPCh"; // FPCh = FXP (preset), FBCh = FXB (bank)
			fxp.Version = 1; // Format Version (should be 1)
			fxp.FxID = "J9AU";
			fxp.FxVersion = 1;
			fxp.ProgramCount = 36; // I.e. nummber of parameters
			fxp.Name = PresetName;
			
			byte[] chunkData = GetChunkData();
			fxp.ChunkSize = chunkData.Length;
			fxp.ChunkDataByteArray = chunkData;
			
			fxp.WriteFile(filePath);
			return true;
		}
		
		public bool Read(string filePath)
		{
			// store filepath
			FilePath = filePath;
			
			FXP fxp = new FXP();
			fxp.ReadFile(filePath);
			
			if (!ReadFXP(fxp, filePath)) {
				return false;
			}
			return true;
		}

		public bool Write(string filePath)
		{
			if (!WriteFXP(filePath)) {
				return false;
			}
			return true;
		}
		
		private byte[] GetChunkData()
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFile bFile = new BinaryFile(memStream, BinaryFile.ByteOrder.LittleEndian);
			
			// Write UAD Preset Header information
			bFile.Write((int) PresetHeaderVar1);
			bFile.Write((int) PresetHeaderVar2);
			bFile.Write(PresetName, 32);
			
			// Write Parameters
			bFile.Write((float) Input); // (-20.0 dB -> 20.0 dB)
			bFile.Write((float) Phase); // (Normal -> Inverted)
			bFile.Write((float) HPFreq); // (Out -> 304 Hz)
			bFile.Write((float) LPFreq); // (Out -> 3.21 k)
			bFile.Write((float) HP_LPDynSC); // (Off -> On)
			bFile.Write((float) CMPRatio); // (1.00:1 -> Limit)
			bFile.Write((float) CMPThresh); // (10.0 dB -> -20.0 dB)
			bFile.Write((float) CMPRelease); // (0.10 s -> 4.00 s)
			bFile.Write((float) CMPAttack); // (Auto -> Fast)
			bFile.Write((float) StereoLink); // (UnLink -> Link)
			bFile.Write((float) Select); // (Expand -> Gate 2)
			bFile.Write((float) EXPThresh); // (-30.0 dB -> 10.0 dB)
			bFile.Write((float) EXPRange); // (0.0 dB -> 40.0 dB)
			bFile.Write((float) EXPRelease); // (0.10 s -> 4.00 s)
			bFile.Write((float) EXPAttack); // (Auto -> Fast)
			bFile.Write((float) DYNIn); // (Out -> In)
			bFile.Write((float) CompIn); // (Out -> In)
			bFile.Write((float) ExpIn); // (Out -> In)
			bFile.Write((float) LFGain); // (-10.0 dB -> 10.0 dB)
			bFile.Write((float) LFFreq); // (36.1 Hz -> 355 Hz)
			bFile.Write((float) LFBell); // (Shelf -> Bell)
			bFile.Write((float) LMFGain); // (-15.6 dB -> 15.6 dB)
			bFile.Write((float) LMFFreq); // (251 Hz -> 2.17 k)
			bFile.Write((float) LMFQ); // (2.50 -> 2.50)
			bFile.Write((float) HMFQ); // (4.00 -> 0.40)
			bFile.Write((float) HMFGain); // (-16.5 dB -> 16.5 dB)
			bFile.Write((float) HMFFreq); // (735 Hz -> 6.77 k)
			bFile.Write((float) HFGain); // (-16.0 dB -> 16.1 dB)
			bFile.Write((float) HFFreq); // (6.93 k -> 21.7 k)
			bFile.Write((float) HFBell); // (Shelf -> Bell)
			bFile.Write((float) EQIn); // (Out -> In)
			bFile.Write((float) EQDynSC); // (Off -> On)
			bFile.Write((float) PreDyn); // (Off -> On)
			bFile.Write((float) Output); // (-20.0 dB -> 20.0 dB)
			bFile.Write((float) EQType); // (Black -> Brown)
			bFile.Write((float) Power); // (Off -> On)
			
			byte[] chunkData = memStream.ToArray();
			memStream.Close();
			
			return chunkData;
		}
		#endregion
		
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(String.Format("PresetName: {0}", PresetName));
			sb.Append(String.Format("Input: {0:0.00}", Input).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Input", Input), "-20.0 dB -> 20.0 dB");
			sb.Append(String.Format("Phase: {0:0.00}", Phase).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Phase", Phase), "Normal -> Inverted");
			sb.Append(String.Format("HP Freq: {0:0.00}", HPFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HP Freq", HPFreq), "Out -> 304 Hz");
			sb.Append(String.Format("LP Freq: {0:0.00}", LPFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LP Freq", LPFreq), "Out -> 3.21 k");
			sb.Append(String.Format("HP/LP Dyn SC: {0:0.00}", HP_LPDynSC).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HP/LP Dyn SC", HP_LPDynSC), "Off -> On");
			sb.Append(String.Format("CMP Ratio: {0:0.00}", CMPRatio).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("CMP Ratio", CMPRatio), "1.00:1 -> Limit");
			sb.Append(String.Format("CMP Thresh: {0:0.00}", CMPThresh).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("CMP Thresh", CMPThresh), "10.0 dB -> -20.0 dB");
			sb.Append(String.Format("CMP Release: {0:0.00}", CMPRelease).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("CMP Release", CMPRelease), "0.10 s -> 4.00 s");
			sb.Append(String.Format("CMP Attack: {0:0.00}", CMPAttack).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("CMP Attack", CMPAttack), "Auto -> Fast");
			sb.Append(String.Format("Stereo Link: {0:0.00}", StereoLink).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Stereo Link", StereoLink), "UnLink -> Link");
			sb.Append(String.Format("Select: {0:0.00}", Select).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Select", Select), "Expand -> Gate 2");
			sb.Append(String.Format("EXP Thresh: {0:0.00}", EXPThresh).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EXP Thresh", EXPThresh), "-30.0 dB -> 10.0 dB");
			sb.Append(String.Format("EXP Range: {0:0.00}", EXPRange).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EXP Range", EXPRange), "0.0 dB -> 40.0 dB");
			sb.Append(String.Format("EXP Release: {0:0.00}", EXPRelease).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EXP Release", EXPRelease), "0.10 s -> 4.00 s");
			sb.Append(String.Format("EXP Attack: {0:0.00}", EXPAttack).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EXP Attack", EXPAttack), "Auto -> Fast");
			sb.Append(String.Format("DYN In: {0:0.00}", DYNIn).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("DYN In", DYNIn), "Out -> In");
			sb.Append(String.Format("Comp In: {0:0.00}", CompIn).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Comp In", CompIn), "Out -> In");
			sb.Append(String.Format("Exp In: {0:0.00}", ExpIn).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Exp In", ExpIn), "Out -> In");
			sb.Append(String.Format("LF Gain: {0:0.00}", LFGain).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LF Gain", LFGain), "-10.0 dB -> 10.0 dB");
			sb.Append(String.Format("LF Freq: {0:0.00}", LFFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LF Freq", LFFreq), "36.1 Hz -> 355 Hz");
			sb.Append(String.Format("LF Bell: {0:0.00}", LFBell).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LF Bell", LFBell), "Shelf -> Bell");
			sb.Append(String.Format("LMF Gain: {0:0.00}", LMFGain).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LMF Gain", LMFGain), "-15.6 dB -> 15.6 dB");
			sb.Append(String.Format("LMF Freq: {0:0.00}", LMFFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LMF Freq", LMFFreq), "251 Hz -> 2.17 k");
			sb.Append(String.Format("LMF Q: {0:0.00}", LMFQ).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("LMF Q", LMFQ), "2.50 -> 2.50");
			sb.Append(String.Format("HMF Q: {0:0.00}", HMFQ).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HMF Q", HMFQ), "4.00 -> 0.40");
			sb.Append(String.Format("HMF Gain: {0:0.00}", HMFGain).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HMF Gain", HMFGain), "-16.5 dB -> 16.5 dB");
			sb.Append(String.Format("HMF Freq: {0:0.00}", HMFFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HMF Freq", HMFFreq), "735 Hz -> 6.77 k");
			sb.Append(String.Format("HF Gain: {0:0.00}", HFGain).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HF Gain", HFGain), "-16.0 dB -> 16.1 dB");
			sb.Append(String.Format("HF Freq: {0:0.00}", HFFreq).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HF Freq", HFFreq), "6.93 k -> 21.7 k");
			sb.Append(String.Format("HF Bell: {0:0.00}", HFBell).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("HF Bell", HFBell), "Shelf -> Bell");
			sb.Append(String.Format("EQ In: {0:0.00}", EQIn).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EQ In", EQIn), "Out -> In");
			sb.Append(String.Format("EQ Dyn SC: {0:0.00}", EQDynSC).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EQ Dyn SC", EQDynSC), "Off -> On");
			sb.Append(String.Format("Pre Dyn: {0:0.00}", PreDyn).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Pre Dyn", PreDyn), "Off -> On");
			sb.Append(String.Format("Output: {0:0.00}", Output).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Output", Output), "-20.0 dB -> 20.0 dB");
			sb.Append(String.Format("EQ Type: {0:0.00}", EQType).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("EQ Type", EQType), "Black -> Brown");
			sb.Append(String.Format("Power: {0:0.00}", Power).PadRight(20)).AppendFormat("= {0} ({1})\n", FindClosestDisplayText("Power", Power), "Off -> On");
			return sb.ToString();
		}
	}
}
