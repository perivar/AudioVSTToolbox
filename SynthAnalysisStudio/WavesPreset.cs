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
	/// Description of WavesPreset
	/// </summary>
	public abstract class WavesPreset : Preset
	{
		public string PresetName = "<not set>";
		public string PluginName = "<not set>";
		public string PluginVersion = "<not set>";
		public string ActiveSetup = "CURRENT";
		public string SetupName = "<not set>";
		public string RealWorldParameters = "<not set>";
		
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

			return ParseXml(chunkData);
		}

		private bool ParseXml(string xmlString) {

			XmlDocument xml = new XmlDocument();
			try {
				if (xmlString != null) xml.LoadXml(xmlString);
				
				// foreach Preset node that has a Name attribude
				XmlNodeList presetNodeList = xml.SelectNodes("//Preset[@Name]");
				foreach (XmlNode presetNode in presetNodeList) {
					ParsePresetNode(presetNode);
				}
				return true;
			} catch (XmlException) {
				return false;
			}
		}
		
		private void ParsePresetNode(XmlNode presetNode) {
			
			// Get the preset node's attributes
			XmlAttribute nameAtt = presetNode.Attributes["Name"];
			if (nameAtt != null && nameAtt.Value != null) {
				PresetName = nameAtt.Value;
			}

			// Read some values from the PresetHeader section
			XmlNode pluginNameNode = presetNode.SelectSingleNode("PresetHeader/PluginName");
			if (pluginNameNode != null && pluginNameNode.InnerText != null) {
				PluginName = pluginNameNode.InnerText;
			}

			XmlNode pluginVersionNode = presetNode.SelectSingleNode("PresetHeader/PluginVersion");
			if (pluginVersionNode != null && pluginVersionNode.InnerText != null) {
				PluginVersion = pluginVersionNode.InnerText;
			}

			XmlNode activeSetupNode = presetNode.SelectSingleNode("PresetHeader/ActiveSetup");
			if (activeSetupNode != null && activeSetupNode.InnerText != null) {
				ActiveSetup = activeSetupNode.InnerText;
			}
			
			// Read the preset data node
			XmlNode presetDataNode = presetNode.SelectSingleNode("PresetData[@Setup='" + ActiveSetup + "']");
			
			// Get the preset data node's attributes
			XmlAttribute setupNameAtt = presetDataNode.Attributes["SetupName"];
			if (setupNameAtt != null && setupNameAtt.Value != null) {
				SetupName = setupNameAtt.Value;
			}

			// And get the real world data
			XmlNode parametersNode = presetDataNode.SelectSingleNode("Parameters[@Type='RealWorld']");
			if (parametersNode != null && parametersNode.InnerText != null) {
				RealWorldParameters = parametersNode.InnerText;
			}
			
			ReadRealWorldParameters();
		}

		protected abstract void ReadRealWorldParameters();
		
	}
}
