using System;
using System.IO;
using System.Text;
using System.Linq;
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
		public string PresetName = null;
		public string PresetGroup = null;
		public string PluginName = null;
		public string PluginVersion = null;
		public string ActiveSetup = "CURRENT";
		public string SetupName = null;
		public string RealWorldParameters = null;
		
		public bool Read(string filePath)
		{
			FXP fxp = new FXP();
			fxp.ReadFile(filePath);
			byte[] chunkDataByteArray = fxp.ChunkDataByteArray;
			return ReadChunkData(chunkDataByteArray);
		}
		
		public bool Write(string filePath)
		{
			if (PluginName != null) {
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
		/// E.g.
		/// C:\Program Files (x86)\Waves\Plug-Ins\SSLChannel.bundle\Contents\Resources\XPst\1000
		/// or
		/// C:\Users\Public\Waves Audio\Plug-In Settings\*.xps files
		/// </summary>
		/// <param name="filePath">file to xps file (e.g. with the filename '1000' or *.xps)</param>
		/// <param name="writer">TextWriter e.g. Console.Out or TextWriter tw = new StreamWriter()</param>
		/// <returns>true if succesful</returns>
		public bool ReadXps(string filePath, TextWriter tw)
		{
			string xmlString = File.ReadAllText(filePath);
			return ParseXml(xmlString, tw);
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
			return ParseXml(xmlString, null);
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
		
		private bool ParseXml(string xmlString, TextWriter tw) {

			XmlDocument xml = new XmlDocument();
			try {
				if (xmlString != null) xml.LoadXml(xmlString);
				
				// foreach Preset node that has a Name attribude
				XmlNodeList presetNodeList = xml.SelectNodes("//Preset[@Name]");
				foreach (XmlNode presetNode in presetNodeList) {
					ParsePresetNode(presetNode);
					if (tw != null) {
						tw.WriteLine(ToString());
						tw.WriteLine();
						tw.WriteLine("-------------------------------------------------------");
					}
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
			} else {
				PresetName = "";
			}

			// Read some values from the PresetHeader section
			XmlNode pluginNameNode = presetNode.SelectSingleNode("PresetHeader/PluginName");
			if (pluginNameNode != null && pluginNameNode.InnerText != null) {
				PluginName = pluginNameNode.InnerText;
			} else {
				PluginName = "";
			}

			XmlNode pluginVersionNode = presetNode.SelectSingleNode("PresetHeader/PluginVersion");
			if (pluginVersionNode != null && pluginVersionNode.InnerText != null) {
				PluginVersion = pluginVersionNode.InnerText;
			} else {
				PluginVersion = "";
			}

			XmlNode pluginGroupNode = presetNode.SelectSingleNode("PresetHeader/Group");
			if (pluginGroupNode != null && pluginGroupNode.InnerText != null) {
				PresetGroup = pluginGroupNode.InnerText;
			} else {
				PresetGroup = null;
			}

			XmlNode activeSetupNode = presetNode.SelectSingleNode("PresetHeader/ActiveSetup");
			if (activeSetupNode != null && activeSetupNode.InnerText != null) {
				ActiveSetup = activeSetupNode.InnerText;
			} else {
				ActiveSetup = "CURRENT";
			}
			
			// Read the preset data node
			XmlNode presetDataNode = presetNode.SelectSingleNode("PresetData[@Setup='" + ActiveSetup + "']");
			if (presetDataNode != null) {
				// Get the preset data node's attributes
				XmlAttribute setupNameAtt = presetDataNode.Attributes["SetupName"];
				if (setupNameAtt != null && setupNameAtt.Value != null) {
					SetupName = setupNameAtt.Value;
				}

				// And get the real world data
				XmlNode parametersNode = presetDataNode.SelectSingleNode("Parameters[@Type='RealWorld']");
				if (parametersNode != null && parametersNode.InnerText != null) {
					RealWorldParameters = StringUtils.TrimMultiLine(parametersNode.InnerText);
				}
				
				ReadRealWorldParameters();
			}

		}
		
		protected abstract void ReadRealWorldParameters();
		
	}
}
