using System;
using System.Collections.Generic;
using System.Globalization;

using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Xml;
using System.Xml.Linq;

using System.IO;

using System.Data;
using System.Data.OleDb;

namespace InvestigatePresetFileDump
{
	class Program
	{
		private const bool DO_FXP_WRAP = true;
		private const int FXP_OFFSET = 60; // 60;
		
		public static void Main(string[] args)
		{

			string outputfilename = @"C:\Users\perivar.nerseth\Documents\My Projects\AudioVSTToolbox\InvestigatePresetFileDump\Sweetscape Template Output V2.txt";

			// create a writer and open the file
			TextWriter tw = new StreamWriter(outputfilename);
			tw.Write(PresetHeader());
			
			StringWriter stringWriter = new StringWriter();
			string enumSections = ImportXMLFileReturnEnumSections(
				@"C:\Users\perivar.nerseth\Documents\My Projects\AudioVSTToolbox\SynthAnalysisStudio\bin\Release\output.xml",
				stringWriter
			);
			
			tw.WriteLine(enumSections);
			tw.WriteLine(stringWriter.ToString());
			tw.Write(PresetFooter());
			
			// close the stream
			tw.Close();
		}
		
		public static string PresetHeader() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("//--------------------------------------");
			sb.AppendLine("//--- 010 Editor Binary Template");
			sb.AppendLine("//");
			sb.AppendLine("// File: <filename>");
			sb.AppendLine("// Author: Per Ivar Nerseth");
			sb.AppendLine("// Revision: 1.0");
			sb.AppendLine("// Purpose: Read a specific VST's preset files (wrapped in a fxp file)");
			sb.AppendLine("//--------------------------------------");
			
			if (DO_FXP_WRAP) {
				sb.AppendLine("");
				sb.AppendLine("typedef struct {");
				sb.AppendLine("    char chunkMagic[4];     // 'CcnK'");
				sb.AppendLine("    long byteSize;          // of this chunk, excl. magic + byteSize");
				sb.AppendLine("    char fxMagic[4];        // 'FxCk', 'FxBk', 'FBCh' or 'FPCh'");
				sb.AppendLine("");
				sb.AppendLine("    long version;");
				sb.AppendLine("    char fxID[4];           // fx unique id");
				sb.AppendLine("    long fxVersion;");
				sb.AppendLine("    long numPrograms;");
				sb.AppendLine("    char name[28];");
				sb.AppendLine("    long chunkSize;");
				sb.AppendLine("} presetHEADER;");
			}
			sb.AppendLine("");
			
			return sb.ToString();
		}
		
		public static string PresetFooter() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("LittleEndian();");
			if (DO_FXP_WRAP) {
				sb.AppendLine("");
				sb.AppendLine("SetBackColor( cLtYellow );");
				sb.AppendLine("presetHEADER header;");
			}
			sb.AppendLine("");
			sb.AppendLine("SetBackColor( cLtGray );");
			sb.AppendLine("presetCONTENT content;");
			return sb.ToString();
		}

		/* Import XML file output from
		 * */
		public static string ImportXMLFileReturnEnumSections(string xmlfilename, TextWriter tw)
		{
			StringBuilder enumSections = new StringBuilder();
			tw.WriteLine("typedef struct {");
			
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			Dictionary<string, List<byte>> dictionary = new Dictionary<string, List<byte>>();

			// first sort the data
			//http://stackoverflow.com/questions/5603284/linq-to-xml-groupby
			var data = from row in xmlDoc.Descendants("Row")
				orderby Convert.ToInt32(row.Element("IndexInFile").Value) ascending
				select new {
				IndexInFile = Convert.ToInt32(row.Element("IndexInFile").Value),
				ByteValue = Convert.ToByte(row.Element("ByteValue").Value),
				ParameterName = (string)row.Element("ParameterName").Value,
				ParameterNameFormatted = (string)row.Element("ParameterNameFormatted").Value,
				ParameterLabel = (string)row.Element("ParameterLabel").Value,
				ParameterDisplay = (string)row.Element("ParameterDisplay").Value,
			};
			
			// then group the data
			var groupQuery = from row in data
				group row by new {
				ParameterNameFormatted = row.ParameterNameFormatted
			}
			into groupedTable
				select new
			{
				//AccountExpirationDate = string.IsNullOrEmpty((string)settings.Element("AccountExpirationDate")) ? (DateTime?)null : DateTime.Parse(settings.Element("AccountExpirationDate").Value)
				//int.Parse(prod.Element("Price").Value)
				// NumComments = (int?)item.Element(slashNamespace + "comments") ?? 0,
				// var intQuery = query.Where( t => int.TryParse( t.Column, out i ) );
				
				Keys = groupedTable.Key,  // Each Key contains all the grouped by columns (if multiple groups)
				LowestValue = (double)groupedTable.Min(p => GetDouble( p.ParameterDisplay, Double.MinValue ) ),
				HighestValue = (double)groupedTable.Max(p => GetDouble( p.ParameterDisplay, Double.MinValue ) ),
				FirstIndex = (int)groupedTable.First().IndexInFile,
				LastIndex = (int)groupedTable.Last().IndexInFile,
				SubGroup = groupedTable
			};

			// turn into list
			var groupedList = groupQuery.ToList();

			int prevIndex = 0;
			bool prevSkipSeek = false;
			for (int i = 0; i < groupedList.Count; i++) {
				var curElement = groupedList.ElementAt(i);

				int firstIndex = curElement.FirstIndex;
				int lastIndex = curElement.LastIndex;
				int numberOfBytes = (lastIndex-firstIndex+1);
				
				// for IL Harmor use true
				// otherwise false
				string dataType = NumberOfBytesToDataType(ref numberOfBytes, true );
				if (numberOfBytes != (lastIndex-firstIndex+1)) {
					// the number of bytes was changed.
					lastIndex = firstIndex + numberOfBytes - 1;
				}
				
				// check if we should convert ushorts to ints and skip the Seek next time around?
				bool skipSeek = false;
				if (i + 1 < groupedList.Count) {
					var nextElement = groupedList.ElementAt(i + 1);
					if ((dataType.Equals("ushort") || dataType.Equals("byte") || dataType.Equals("unknown")) && nextElement.FirstIndex == firstIndex + 4) {
						dataType = "int";
						skipSeek = true;
						lastIndex = firstIndex + 3;
						numberOfBytes = 4;
					}
				}
				
				// write seek part (i.e. move pointer to first byte) if the first byte is not
				// directly succeding the last section that was written
				if (!prevSkipSeek && (prevIndex + 1 != firstIndex)) {
					tw.WriteLine("\tFSeek( {0} );", firstIndex + FXP_OFFSET);
				}
				
				if (curElement.LowestValue != Double.MinValue && curElement.HighestValue != Double.MinValue) {
					double lowVal = (double)curElement.LowestValue;
					double highVal = (double)curElement.HighestValue;
					string name = curElement.Keys.ParameterNameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", dataType, CleanInput(name)).PadRight(35);
					string indexAndValueRange = String.Format("// index {0}:{1} = {4} bytes (value range {2} -> {3})", firstIndex, lastIndex, lowVal, highVal, numberOfBytes);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexAndValueRange);
				} else {
					// insert enum instead of datatype
					string enumName = curElement.Keys.ParameterNameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", CleanInput(enumName.ToUpper()), enumName).PadRight(35);
					string indexRange = String.Format("// index {0}:{1} = {2} bytes ", firstIndex, lastIndex, numberOfBytes);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexRange);
					string enumsection = getEnumSectionXMLFormat(xmlfilename, enumName);
					enumSections.AppendLine(enumsection);
				}
				
				prevIndex = lastIndex;
				prevSkipSeek = skipSeek;
			}
			
			tw.WriteLine("} presetCONTENT;");
			
			return enumSections.ToString();
		}
		
		public static List<string> getUniqueValues(string xmlfilename, string NameFormattedValue)
		{
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			List<string> uniqueList = new List<string>();
			var listQuery = (from row in xmlDoc.Descendants("Row")
			                 where row.Element("NameFormatted").Value == NameFormattedValue
			                 orderby row.Element("DisplayValue").Value descending
			                 select new {
			                 	Index = row.Element("Index").Value,
			                 	DisplayValue = row.Element("DisplayValue").Value,
			                 	ByteValue = row.Element("ByteValue").Value
			                 }
			                ).Distinct();
			foreach (var li in listQuery)
			{
				byte b = Byte.Parse(li.ByteValue);
				string hex = b.ToString("X2");
				
				uniqueList.Add(String.Format("{0}:{1}={2} 0x{3}",li.Index, li.DisplayValue,li.ByteValue, hex));
			}
			return uniqueList;
		}
		
		public static string getEnumSectionXMLFormat(string xmlfilename, string NameFormattedValue)
		{
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			Dictionary<string, List<byte>> dictionary = new Dictionary<string, List<byte>>();
			
			var listQuery = (from row in xmlDoc.Descendants("Row")
			                 where row.Element("ParameterNameFormatted").Value == NameFormattedValue
			                 orderby row.Element("ParameterDisplay").Value ascending
			                 select new {
			                 	Index = row.Element("IndexInFile").Value,
			                 	DisplayValue = row.Element("ParameterDisplay").Value,
			                 	ByteValue = row.Element("ByteValue").Value
			                 }
			                ).Distinct();
			
			foreach (var li in listQuery)
			{
				string key = li.DisplayValue;
				byte value = byte.Parse(li.ByteValue);
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(value);
				}
				else
				{
					List<byte> valueList = new List<byte>();
					valueList.Add(value);
					dictionary.Add(key, valueList);
				}
			}
			
			int numberOfBytes = dictionary.First().Value.Count;
			string datatype = NumberOfBytesToDataType(ref numberOfBytes, true);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(String.Format("typedef enum <{0}> {{", datatype));
			
			//typedef enum <uint> {
			//	Lowpass     = 0x3EAAAAAB,
			//	Highpass    = 0x3F800000,
			//  Bypass      = 0x00000000,
			//  Bandpass    = 0x3F2AAAAB
			//} FILTERTYPE <format=hex>;
			
			int count = 1;
			foreach (var pair in dictionary)
			{
				string key = FixEnumEntries(CleanInput(pair.Key), CleanInput(NameFormattedValue.ToUpper()));
				sb.Append(String.Format("\t{0}", key).PadRight(20));
				sb.Append("= 0x");
				
				byte[] bArray = pair.Value.ToArray();
				Array.Reverse( bArray );
				sb.Append(String.Format("{0}", ByteArrayToString(bArray, numberOfBytes)));
				if (count < dictionary.Count) {
					sb.AppendLine(",");
					count++;
				} else {
					sb.AppendLine();
				}
			}

			sb.AppendLine(String.Format("}} {0} <format=hex>;", CleanInput(NameFormattedValue.ToUpper())));
			return sb.ToString();
		}

		
		public static double GetDouble(string value, double defaultValue)
		{
			double result;

			//Try parsing in the current culture
			if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
			    //Then try in US english
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
			    //Then in neutral language
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				result = defaultValue;
			}
			
			// special case
			if (result == 3.911555E-07) result = 0;

			return result;
		}
		
		public static string NumberOfBytesToDataType(ref int numberOfBytes, bool isEnum ) {
			string datatype = "";
			switch (numberOfBytes) {
				case 1:
					datatype = "byte";
					break;
				case 2:
					datatype = "ushort";
					break;
				case 4:
					if (isEnum) {
						datatype = "int";
					} else {
						datatype = "float";
					}
					break;
				case 5:
				case 6:
				case 7:
				case 8:
					numberOfBytes = 8;
					datatype = "uint64";
					break;
					//case 16:
					//	numberOfBytes = 16;
					//	datatype = "16bytes";
					//	break;
				default:
					numberOfBytes = 4;
					datatype = "uint32";
					break;
			}
			return datatype;
		}
		
		public static string ByteArrayToString(byte[] ba, int numberOfBytes)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			for (int i = 0; i < numberOfBytes && i < ba.Length && i < 8; i++) {
				byte b = ba[i];
				hex.AppendFormat("{0:X2}", b);
			}
			return hex.ToString();
		}

		public static byte[] StringToByteArray(String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}
		
		public static string CleanInput(string strIn)
		{
			// Replace invalid characters with empty strings.
			return Regex.Replace(strIn, @"[^\w]", "_");
		}
		
		public static string FixEnumEntries(string strIn, string name) {
			
			return name + "_" + strIn;
			/*
			char[] chars = strIn.ToCharArray();
			if (Char.IsDigit(chars[0])) {
				// an enum cannot start with a number?!
				strIn = "ENUM_" + strIn;
			}
			return strIn;
			 */
		}
	}
}