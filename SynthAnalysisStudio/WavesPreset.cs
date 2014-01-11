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
			byte[] chunkDataByteArray = fxp.ChunkDataByteArray;
			return ReadChunkData(chunkDataByteArray);
		}
		
		public bool Write(string filePath)
		{
			if (PluginName != "<not set>") {
				// create a writer and open the file
				TextWriter tw = new StreamWriter(filePath);
				
				// write the preset string
				tw.Write(ToString());
				
				// close the stream
				tw.Close();
				
				return true;
			}
			return false;
		}

		/// <summary>
		/// Read Waves XPst files
		/// E.g. C:\Program Files (x86)\Waves\Plug-Ins\SSLChannel.bundle\Contents\Resources\XPst\1000
		/// </summary>
		/// <param name="filePath">file to XPst file (e.g. with the filename '1000')</param>
		/// <returns>true if succesful</returns>
		public bool ReadXPst(string filePath)
		{
			string xmlString = File.ReadAllText(filePath);
			return ParseXml(xmlString);
		}

		/// <summary>
		/// Parse out the xml string from the passed chunk data byte array
		/// </summary>
		/// <param name="chunkDataByteArray"></param>
		/// <returns>xml string</returns>
		private static string ParseChunkData(byte[] chunkDataByteArray) {
			BinaryFile bf = new BinaryFile(chunkDataByteArray, BinaryFile.ByteOrder.BigEndian);
			
			int val1 = bf.ReadInt32();
			int val2 = bf.ReadInt32();
			int val3 = bf.ReadInt32();
			string val4 = bf.ReadString(4);
			string val5 = bf.ReadString(4);
			
			//int chunkSize = byteArray.Length - 32;
			int chunkSize = bf.ReadInt32();
			//int val6 = bf.ReadInt32();

			string val7 = bf.ReadString(4);
			
			byte[] xmlChunkBytes = new byte[chunkSize];
			xmlChunkBytes = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			string xmlString = BinaryFile.ByteArrayToString(xmlChunkBytes);
			
			int val8 = bf.ReadInt32(BinaryFile.ByteOrder.LittleEndian);
			
			return xmlString;
		}
		
		public bool ReadChunkData(byte[] chunkDataByteArray) {
			
			string xmlString = ParseChunkData(chunkDataByteArray);
			return ParseXml(xmlString);
		}

		public static string GetPluginName(byte[] chunkDataByteArray) {
			
			string xmlString = ParseChunkData(chunkDataByteArray);
			return GetPluginName(xmlString);
		}
		
		private static string GetPluginName(string xmlString) {
			
			XmlDocument xml = new XmlDocument();
			try {
				if (xmlString != null) xml.LoadXml(xmlString);
				
				// Get preset node that has a Name attribude
				// e.g. <Preset Name=""><PresetHeader><PluginName>SSLChannel</PluginName></PresetHeader></Preset>
				XmlNode firstPresetNode = xml.SelectSingleNode("//Preset[@Name]");

				if (firstPresetNode != null) {

					// Read some values from the PresetHeader section
					XmlNode pluginNameNode = firstPresetNode.SelectSingleNode("PresetHeader/PluginName");
					if (pluginNameNode != null && pluginNameNode.InnerText != null) {
						return pluginNameNode.InnerText;
					}
				}
				return null;
			} catch (XmlException) {
				return null;
			}
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
				RealWorldParameters = parametersNode.InnerText; //.Replace("\r", "").Replace("\n", "");
			}
			
			ReadRealWorldParameters();
		}

		protected abstract void ReadRealWorldParameters();
		
	}
}
