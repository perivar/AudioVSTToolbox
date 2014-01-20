using System;
using System.IO;
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
		
		public UADSSLChannel()
		{
		}
		
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
	}
}
