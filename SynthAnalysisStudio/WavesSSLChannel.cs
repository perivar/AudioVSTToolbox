using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Globalization;

using PresetConverter;
using CommonUtils;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WavesSSLChannel.
	/// </summary>
	public class WavesSSLChannel : Preset
	{
		#region Public Fields
		public string PresetName = "";
		public string PluginVersion = "";
		
		public float CompThreshold;
		public float CompRatio;
		public bool CompFastAttack;
		public float CompRelease;
		
		public float ExpThreshold;
		public float ExpRange;
		public bool ExpGate;
		public bool ExpFastAttack;
		public float ExpRelease;

		public bool DynToByPass;
		public bool DynToChannelOut;
		
		public bool LFTypeBell;
		public float LFGain;
		public float LFFrq;
		
		public float LMFGain;
		public float LMFFrq;
		public float LMFQ;
		
		public float HMFGain;
		public float HMFFrq;
		public float HMFQ;
		
		public bool HFTypeBell;
		public float HFGain;
		public float HFFrq;
		
		public bool EQToBypass;
		public bool EQToDynSC;

		public float HPFrq;
		public float LPFrq;
		public bool FilterSplit;
		
		public float Gain;
		public bool Analog;
		public bool VUShowOutput;
		public bool PhaseReverse;
		public float InputTrim ;
		#endregion
		
		public WavesSSLChannel()
		{
		}
		
		public bool ReadChunkData(byte[] byteArray) {
			
			BinaryFile bf = new BinaryFile(byteArray, BinaryFile.ByteOrder.BigEndian);
			
			int val1 = bf.ReadInt32();
			int val2 = bf.ReadInt32();
			int val3 = bf.ReadInt32();
			string val4 = bf.ReadString(4);
			string val5 = bf.ReadString(4);
			
			//int chunkSize = byteArray.Length - 32;
			int chunkSize = bf.ReadInt32();
			//int val6 = bf.ReadInt32();

			string val7 = bf.ReadString(4);
			
			byte[] chunkDataByteArray = new byte[chunkSize];
			chunkDataByteArray = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			string chunkData = BinaryFile.ByteArrayToString(chunkDataByteArray);
			
			int val8 = bf.ReadInt32(BinaryFile.ByteOrder.LittleEndian);
			
			// read the xml chunk into memory
			XmlDocument doc = new XmlDocument();
			try {
				if (chunkData != null) doc.LoadXml(chunkData);
				
				XmlNode presetNode = doc.SelectSingleNode("//Preset[@Name]");
				//PresetName = presetNode.Attributes["Name"].Value;
				
				XmlNode pluginVersion = presetNode.SelectSingleNode("PresetHeader/PluginVersion");
				if (pluginVersion != null && pluginVersion.InnerText != null) {
					PluginVersion = pluginVersion.InnerText;
				}
				
				XmlNode pluginName = presetNode.SelectSingleNode("PresetHeader/PluginName");
				if (pluginName != null && pluginName.InnerText != null) {
					if (pluginName.InnerText == "SSLChannel") {
						#region SSLChannel
						XmlNode activeSetup = presetNode.SelectSingleNode("PresetHeader/ActiveSetup");
						XmlNode presetDataNode = doc.SelectSingleNode("//Preset/PresetData[@Setup='" + activeSetup.InnerText + "']");
						
						// Get the node's attributes
						XmlAttribute att = presetDataNode.Attributes["SetupName"];
						if (att != null && att.Value != null) {
							PresetName = att.Value;
						}

						XmlNode parametersNode = presetDataNode.SelectSingleNode("Parameters[@Type='RealWorld']");
						
						// split the parameters text into sections
						string[] splittedPhrase = parametersNode.InnerText.Split(' ', '\n');
						
						CompThreshold = float.Parse(splittedPhrase[0], CultureInfo.InvariantCulture); // compression threshold in dB
						CompRatio = float.Parse(splittedPhrase[1], CultureInfo.InvariantCulture); // compression ratio
						CompFastAttack = (splittedPhrase[2] == "1"); // compression fast attack
						CompRelease = float.Parse(splittedPhrase[3], CultureInfo.InvariantCulture); // compression release in ms

						string Delimiter1 = splittedPhrase[4];
						
						ExpThreshold = float.Parse(splittedPhrase[5], CultureInfo.InvariantCulture); // expander threshold in dB
						ExpRange = float.Parse(splittedPhrase[6], CultureInfo.InvariantCulture); // expander range in dB
						ExpGate = (splittedPhrase[7] == "1"); // expander gate
						ExpFastAttack = (splittedPhrase[8] == "1"); // expander fast attack
						ExpRelease = float.Parse(splittedPhrase[9], CultureInfo.InvariantCulture); // expander release in ms

						string Delimiter2 = splittedPhrase[10];
						
						DynToByPass = (splittedPhrase[11] == "1"); // Dyn To By Pass
						DynToChannelOut = (splittedPhrase[12] == "1"); // Dyn To Channel Out
						
						LFTypeBell = (splittedPhrase[13] == "1"); // Bell
						LFGain = float.Parse(splittedPhrase[14], CultureInfo.InvariantCulture); // dB
						LFFrq = float.Parse(splittedPhrase[15], CultureInfo.InvariantCulture); // Hz
						
						LMFGain = float.Parse(splittedPhrase[16], CultureInfo.InvariantCulture); // dB
						LMFFrq = float.Parse(splittedPhrase[17], CultureInfo.InvariantCulture); // KHz
						LMFQ = float.Parse(splittedPhrase[18], CultureInfo.InvariantCulture);
						
						HMFGain = float.Parse(splittedPhrase[19], CultureInfo.InvariantCulture); // dB
						HMFFrq = float.Parse(splittedPhrase[20], CultureInfo.InvariantCulture); // KHz
						HMFQ = float.Parse(splittedPhrase[21], CultureInfo.InvariantCulture);
						
						HFTypeBell = (splittedPhrase[22] == "1"); // Bell
						HFGain = float.Parse(splittedPhrase[23], CultureInfo.InvariantCulture); // dB
						HFFrq = float.Parse(splittedPhrase[24], CultureInfo.InvariantCulture); // KHz
						
						EQToBypass = (splittedPhrase[25] == "1");
						EQToDynSC  = (splittedPhrase[26] == "1");

						HPFrq = float.Parse(splittedPhrase[27], CultureInfo.InvariantCulture); // Hz
						LPFrq = float.Parse(splittedPhrase[28], CultureInfo.InvariantCulture); // KHz

						FilterSplit = (splittedPhrase[29] == "1");
						
						Gain = float.Parse(splittedPhrase[30], CultureInfo.InvariantCulture); // dB

						Analog = (splittedPhrase[31] == "1");
						
						string Delimiter3 = splittedPhrase[32];
						string Delimiter4 = splittedPhrase[33];
						
						VUShowOutput = (splittedPhrase[34] == "1");

						string Delimiter5 = splittedPhrase[35];
						string Delimiter6 = splittedPhrase[36];

						float Unknown1 = float.Parse(splittedPhrase[37], CultureInfo.InvariantCulture);
						float Unknown2 = float.Parse(splittedPhrase[38], CultureInfo.InvariantCulture);
						
						PhaseReverse = (splittedPhrase[39] == "1");
						InputTrim = float.Parse(splittedPhrase[40], CultureInfo.InvariantCulture); // dB
						#endregion
					} else if (pluginName.InnerText == "SSLComp") {
						#region SSLComp
						XmlNode activeSetup = presetNode.SelectSingleNode("PresetHeader/ActiveSetup");
						XmlNode presetDataNode = doc.SelectSingleNode("//Preset/PresetData[@Setup='" + activeSetup.InnerText + "']");
						
						// Get the node's attributes
						XmlAttribute att = presetDataNode.Attributes["SetupName"];
						if (att != null && att.Value != null) {
							PresetName = att.Value;
						}

						XmlNode parametersNode = presetDataNode.SelectSingleNode("Parameters[@Type='RealWorld']");
						
						// split the parameters text into sections
						string[] splittedPhrase = parametersNode.InnerText.Split(' ', '\n');


						//Threshold (-15 - +15)
						CompThreshold = float.Parse(splittedPhrase[0], CultureInfo.InvariantCulture); // compression threshold in dB

						//Ratio (2:1=0, 4:1=1, 10:1=2)
						int ratio = int.Parse(splittedPhrase[1]);
						switch(ratio) {
							case 0:
								CompRatio = 2;
								break;
							case 1:
								CompRatio = 4;
								break;
							case 2:
								CompRatio = 10;
								break;
						}
						
						// Fade [Off=0,Out=1,In=2]
						string Fade = "";
						int fade = int.Parse(splittedPhrase[2]);
						switch(fade) {
							case 0:
								Fade = "Off";
								break;
							case 1:
								Fade = "Out";
								break;
							case 2:
								Fade = "In";
								break;
						}
						
						// Attack [0 - 5, .1 ms, .3 ms, 1 ms, 3 ms, 10 ms, 30 ms)
						float CompAttack;
						int attack = int.Parse(splittedPhrase[3]);
						switch(attack) {
							case 0:
								CompAttack = 0.1f;
								break;
							case 1:
								CompAttack = 0.3f;
								break;
							case 2:
								CompAttack = 1.0f;
								break;
							case 3:
								CompAttack = 3.0f;
								break;
							case 4:
								CompAttack = 10.0f;
								break;
							case 5:
								CompAttack = 30.0f;
								break;
						}
						
						// Release [0 - 4, .1 s, .3 s, .6 s, 1.2 s, Auto)
						int release = int.Parse(splittedPhrase[4]);
						switch(release) {
							case 0:
								CompRelease = 0.1f;
								break;
							case 1:
								CompRelease = 0.3f;
								break;
							case 2:
								CompRelease = 0.6f;
								break;
							case 3:
								CompRelease = 1.2f;
								break;
							case 4:
								CompRelease = -1.0f;
								break;
						}

						//Make Up (-5 - +15)
						Gain = float.Parse(splittedPhrase[5], CultureInfo.InvariantCulture); // dB
						
						//*
						string Delimiter1 = splittedPhrase[6];
						
						//Rate-S (1 - +60)
						float RateS = float.Parse(splittedPhrase[7], CultureInfo.InvariantCulture);
						
						//In
						bool In = (splittedPhrase[8] == "1");

						//Analog
						Analog = (splittedPhrase[9] == "1");
						
						#endregion
					} else {
						// do nothing
					}
				}
			} catch (XmlException) {
				return false;
			}
			
			return true;
		}
		
		public bool Read(string filePath)
		{
			FXP fxp = new FXP();
			fxp.ReadFile(filePath);
			byte[] chunkData = fxp.ChunkDataByteArray;
			return ReadChunkData(chunkData);
		}
		
		public bool Write(string filePath)
		{
			// create a writer and open the file
			TextWriter tw = new StreamWriter(filePath);
			
			// write the preset string
			tw.Write(ToString());
			
			// close the stream
			tw.Close();
			
			return true;
		}
		
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(String.Format("PresetName: {0}", PresetName));
			sb.AppendLine();
			
			sb.AppendLine("Compression:");
			sb.AppendLine(String.Format("\tThreshold: {0} dB", CompThreshold));
			sb.AppendLine(String.Format("\tRatio: {0}", CompRatio));
			sb.AppendLine(String.Format("\tFast Attack: {0}", CompFastAttack));
			sb.AppendLine(String.Format("\tRelease: {0} ms", CompRelease));
			sb.AppendLine();
			
			sb.AppendLine("Expander/Gate:");
			sb.AppendLine(String.Format("\tThreshold: {0} dB", ExpThreshold));
			sb.AppendLine(String.Format("\tRange: {0} dB", ExpRange));
			sb.AppendLine(String.Format("\tGate: {0}", ExpGate));
			sb.AppendLine(String.Format("\tFast Attack: {0}", ExpFastAttack));
			sb.AppendLine(String.Format("\tRelease: {0} ms", ExpRelease));
			sb.AppendLine();
			
			sb.AppendLine("Dynamics To:");
			sb.AppendLine(String.Format("\tBypass: {0}", DynToByPass));
			sb.AppendLine(String.Format("\tChannel Out (Post EQ): {0}", DynToChannelOut));
			sb.AppendLine();
			
			sb.AppendLine("EQ Section:");
			sb.AppendLine(String.Format("\tLF Type Bell: {0}", LFTypeBell));
			sb.AppendLine(String.Format("\tLF Gain: {0} dB", LFGain));
			sb.AppendLine(String.Format("\tLF Frequency: {0} Hz", LFFrq));
			
			sb.AppendLine(String.Format("\tLMF Gain: {0} dB", LMFGain));
			sb.AppendLine(String.Format("\tLMF Frequency: {0} KHz", LMFFrq));
			sb.AppendLine(String.Format("\tLMF Q: {0}", LMFQ));

			sb.AppendLine(String.Format("\tHMF Gain: {0} dB", HMFGain));
			sb.AppendLine(String.Format("\tHMF Frequency: {0} KHz", HMFFrq));
			sb.AppendLine(String.Format("\tHMF Q: {0}", HMFQ));

			sb.AppendLine(String.Format("\tHF Type Bell: {0}", HFTypeBell));
			sb.AppendLine(String.Format("\tHF Gain: {0} dB", HFGain));
			sb.AppendLine(String.Format("\tHF Frequency: {0} KHz", HFFrq));
			
			sb.AppendLine(String.Format("\tTo Bypass: {0}", EQToBypass));
			sb.AppendLine(String.Format("\tTo Dynamics Side-Chain: {0}", EQToDynSC));
			sb.AppendLine();
			
			sb.AppendLine("Low and High Pass Filters:");
			sb.AppendLine(String.Format("\tHP Frequency: {0} Hz", HPFrq));
			sb.AppendLine(String.Format("\tLP Frequency: {0} KHz", LPFrq));
			sb.AppendLine(String.Format("\tFilter Split (Before Dynamics): {0}", FilterSplit));
			sb.AppendLine();
			
			sb.AppendLine("Master Section:");
			sb.AppendLine(String.Format("\tGain: {0} dB", Gain));
			sb.AppendLine(String.Format("\tAnalog: {0}", Analog));
			sb.AppendLine(String.Format("\tVU Show Output: {0}", VUShowOutput));
			sb.AppendLine(String.Format("\tPhase Reverse: {0}", PhaseReverse));
			sb.AppendLine(String.Format("\tInput Trim : {0} dB", InputTrim ));
			
			return sb.ToString();
		}

	}
}
