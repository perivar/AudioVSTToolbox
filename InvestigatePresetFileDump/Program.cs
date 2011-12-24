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
		public static void Main(string[] args)
		{

			string outputfilename = @"C:\Users\perivar.nerseth\My Projects\InvestigatePresetFileDump\InvestigatePresetFileDump\Sweetscape Template Output.txt";

			// create a writer and open the file
			TextWriter tw = new StreamWriter(outputfilename);
			tw.Write(SylenthPresetHeader());
			
			StringWriter stringWriter = new StringWriter();
			string enumSections = ImportXMLFileReturnEnumSections(
				@"C:\Users\perivar.nerseth\My Projects\InvestigatePresetFileDump\InvestigatePresetFileDump\output.xml",
				stringWriter
			);
			
			tw.WriteLine(enumSections);
			tw.WriteLine(stringWriter.ToString());
			tw.Write(SylenthPresetFooter());
			
			// close the stream
			tw.Close();
		}
		
		public static string SylenthPresetHeader() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("//--------------------------------------");
			sb.AppendLine("//--- 010 Editor v3.1 Binary Template");
			sb.AppendLine("//");
			sb.AppendLine("// File: Sylenth 1 Preset Format");
			sb.AppendLine("// Author: Per Ivar Nerseth");
			sb.AppendLine("// Revision: 1.0");
			sb.AppendLine("// Purpose: Read Sylenth1 Preset Files");
			sb.AppendLine("//--------------------------------------");
			sb.AppendLine("");
			sb.AppendLine("typedef struct {");
			sb.AppendLine("    char presetType[4];     // '1lys' = 'syl1' backwards");
			sb.AppendLine("    long unknown1;          //");
			sb.AppendLine("    long fxVersion;         //");
			sb.AppendLine("    long numPrograms;       //");
			sb.AppendLine("    long unknown2;          //");
			sb.AppendLine("} presetHEADER;");
			sb.AppendLine("");
			return sb.ToString();
		}
		
		public static string SylenthPresetFooter() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("LittleEndian();");
			sb.AppendLine("");
			sb.AppendLine("SetBackColor( cLtYellow );");
			sb.AppendLine("presetHEADER header;");
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
				Index = Convert.ToInt32(row.Element("IndexInFile").Value),
				ByteValue = Convert.ToByte(row.Element("ByteValue").Value),
				NameFormatted = (string)row.Element("ParameterNameFormatted").Value,
				DisplayValue = (string)row.Element("ParameterDisplay").Value,
			};
			
			// then group the data
			var groupQuery = from row in data
				group row by new {
				NameFormatted = row.NameFormatted
			}
			into groupedTable
				select new
			{
				//AccountExpirationDate = string.IsNullOrEmpty((string)settings.Element("AccountExpirationDate")) ? (DateTime?)null : DateTime.Parse(settings.Element("AccountExpirationDate").Value)
				//int.Parse(prod.Element("Price").Value)
				// NumComments = (int?)item.Element(slashNamespace + "comments") ?? 0,
				// var intQuery = query.Where( t => int.TryParse( t.Column, out i ) );
				
				Keys = groupedTable.Key,  // Each Key contains all the grouped by columns (if multiple groups)
				LowestValue = (double)groupedTable.Min(p => GetDouble( p.DisplayValue, Double.MinValue ) ),
				HighestValue = (double)groupedTable.Max(p => GetDouble( p.DisplayValue, Double.MinValue ) ),
				FirstIndex = (int)groupedTable.First().Index,
				LastIndex = (int)groupedTable.Last().Index,
				SubGroup = groupedTable
			};

			int prevIndex = 0;
			foreach (var grp in groupQuery)
			{
				int firstIndex = grp.FirstIndex;
				int lastIndex = grp.LastIndex;
				int numberOfBytes = (lastIndex-firstIndex+1);
				
				string datatype = NumberOfBytesToDataType(numberOfBytes, false );
				
				// write seek part (i.e. move pointer to first byte) if the first byte is not
				// directly succeding the last section that was written
				if (prevIndex+1 != firstIndex) {
					tw.WriteLine("\tFSeek( {0} );", firstIndex);
				}
				if (grp.LowestValue != Double.MinValue && grp.HighestValue != Double.MinValue) {
					double lowVal = (double)grp.LowestValue;
					double highVal = (double)grp.HighestValue;
					string name = grp.Keys.NameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", datatype, CleanInput(name)).PadRight(35);
					string indexAndValueRange = String.Format("// index {0}:{1} (value range {2} -> {3})", firstIndex, lastIndex, lowVal, highVal);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexAndValueRange);
				} else {
					// insert enum instead of datatype
					string enumName = grp.Keys.NameFormatted.ToString();
					string datatypeAndName = String.Format("\t{0} {1};", CleanInput(enumName.ToUpper()), enumName).PadRight(35);
					string indexRange = String.Format("// index {0}:{1} ", firstIndex, lastIndex);
					tw.Write(datatypeAndName);
					tw.WriteLine(indexRange);
					string enumsection = getEnumSectionXMLFormat(xmlfilename, enumName);
					enumSections.AppendLine(enumsection);
				}
				
				prevIndex = lastIndex;
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
			string datatype = NumberOfBytesToDataType(numberOfBytes, true);
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

			return result;
		}
		
		public static string NumberOfBytesToDataType( int numberOfBytes, bool isEnum ) {
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
				case 8:
					datatype = "long";
					break;
				default:
					datatype = "int";
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