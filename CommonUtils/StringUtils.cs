using System;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

using System.Linq;

namespace CommonUtils
{
	/// <summary>
	/// Assorted string methods that might be helpful
	/// </summary>
	public static class StringUtils
	{
		/// <summary>
		/// Pascal case
		/// The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized. You can use Pascal case for identifiers of three or more characters. For example:
		/// E.g. "BackColor"
		/// Camel case
		/// The first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized. For example:
		/// E.g. "backColor"
		/// </summary>
		public enum Case {
			PascalCase,
			CamelCase
		}
		
		/// <summary>
		/// Converts the phrase to specified convention.
		/// </summary>
		/// <param name="phrase"></param>
		/// <param name="cases">A Capitalization Style (Pascal or Camel)</param>
		/// <description>
		/// Pascal case
		/// The first letter in the identifier and the first letter of each subsequent concatenated word are capitalized. You can use Pascal case for identifiers of three or more characters. For example:
		/// E.g. "BackColor"
		/// Camel case
		/// The first letter of an identifier is lowercase and the first letter of each subsequent concatenated word is capitalized. For example:
		/// E.g. "backColor"
		/// </description>
		/// <returns>string</returns>
		public static string ConvertCaseString(string phrase, Case cases) {
			string[] splittedPhrase = phrase.Split(' ', '-', '.');
			var sb = new StringBuilder();

			if (cases == Case.CamelCase) {
				sb.Append(splittedPhrase[0].ToLower());
				splittedPhrase[0] = string.Empty;
			} else if (cases == Case.PascalCase) {
				sb = new StringBuilder();
			}

			foreach (String s in splittedPhrase) {
				char[] splittedPhraseChars = s.ToCharArray();
				if (splittedPhraseChars.Length > 0)	{
					splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
				}
				sb.Append(new String(splittedPhraseChars));
			}
			return sb.ToString();
		}
		
		
		/// <summary>
		/// Convert the first letter to lowercase and return
		/// </summary>
		/// <param name="phrase">Phrase to convert</param>
		/// <returns>string</returns>
		public static string ConvertFirstLetterLowerCase(string phrase) {
			return Char.ToLowerInvariant(phrase[0]) + phrase.Substring(1);
		}
		
		/// <summary>
		/// Convert a hex string to uint (typically used for ARGB decimal representation)
		/// </summary>
		/// <param name="hexString">hex string (0x00... or just 00...)</param>
		/// <returns>the uint number</returns>
		public static uint HexStringToUint(string hexString) {
			uint color;

			if (hexString.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
			{
				hexString = hexString.Substring(2);
			}

			bool parsedSuccessfully = uint.TryParse(hexString,
			                                        NumberStyles.HexNumber,
			                                        CultureInfo.CurrentCulture,
			                                        out color);
			return color;
		}
		
		/// <summary>
		/// Convert uint to hex string
		/// </summary>
		/// <param name="c">uint</param>
		/// <returns>hex string on the format 0x00</returns>
		public static string ToHexString(uint c) {
			string s = String.Format("0x{0:X}", c);
			return s;
		}
		
		/// <summary>
		/// Convert byte to hex string
		/// </summary>
		/// <param name="b">byte</param>
		/// <returns>hex string</returns>
		public static string ToHexString(byte b) {
			char c = (char) b;
			string s = String.Format("{0,0:X2}", (int) c);
			return s;
		}
		
		/// <summary>
		/// Convert byte array to hex and ascii string (like hex editor)
		/// </summary>
		/// <param name="b">byte array</param>
		/// <param name="invert">whether to invert the byte array</param>
		/// <returns>string</returns>
		public static string ToHexAndAsciiString(byte[] b, bool invert) {
			var strb = new StringBuilder();
			var text = new StringBuilder();
			if (b != null) {
				var bClone = (byte[])b.Clone();
				if (invert) {
					Array.Reverse(bClone);
				}
				var ch = new char[1];
				for (int x = 0; x < bClone.Length; x++)
				{
					ch[0] = (char) bClone[x];
					strb.AppendFormat ("{0,0:X2} ", (int) ch[0]);
					
					if (((int) ch[0] < 32) || ((int) ch[0] > 127))
						ch[0] = '.';
					text.Append (ch);
				}
				
				// append the text chunk after the hex chunk
				strb.Append ("    ");
				strb.Append (text.ToString());
			}
			return strb.ToString();
		}
		
		/// <summary>
		/// Replace invalid characters with empty strings.
		/// only letters, dots, the email 'at' and '-' are allowed
		/// </summary>
		/// <param name="strIn">string</param>
		/// <returns>formatted string</returns>
		public static string RemoveInvalidCharacters(string strIn) {
			return Regex.Replace(strIn, @"[^\w\.@-]", "");
		}

		/// <summary>
		/// Remove non ascii characters from string but allow the space character
		/// </summary>
		/// <param name="strIn">string</param>
		/// <returns>formatted string</returns>
		public static string RemoveInvalidCharactersAllowSpace(string strIn) {
			// Replace invalid characters with empty strings.
			// only letters, dots, the email 'at' and '-' are allowed
			return Regex.Replace(strIn, @"[^\w\.\s@-]", "");
		}
		
		/// <summary>
		/// Remove non ascii characters from string
		/// </summary>
		/// <param name="strIn">string</param>
		/// <returns>formatted string</returns>
		public static string RemoveNonAsciiCharacters(string strIn) {
			return Regex.Replace(strIn, @"[^\u0000-\u007F]", string.Empty);
		}

		/// <summary>
		/// Trim each line of the multi-line string and then trim the result.
		/// </summary>
		/// <param name="multiLineString">untrimmed multi-line string</param>
		/// <returns>trimmed multi-line string</returns>
		public static string TrimMultiLine(string multiLineString) {
			string trimmedByLine = string.Join(
				"\n",
				multiLineString.Split('\n').Select(s => s.Trim())).Trim();
			return trimmedByLine;
		}
		
		/// <summary>
		/// Make sure the passed string can be used as a filename
		/// </summary>
		/// <param name="name">filename to convert</param>
		/// <returns>valid filename</returns>
		public static string MakeValidFileName( string name )
		{
			string invalidChars = Regex.Escape( new string( Path.GetInvalidFileNameChars() ) );
			string invalidReStr = string.Format( @"[{0}]+", invalidChars );
			return Regex.Replace( name, invalidReStr, "_" );
		}
		
		/// <summary>
		/// Split string by length
		/// </summary>
		/// <param name="str">string</param>
		/// <param name="maxLength">max length</param>
		/// <returns>collection of strings</returns>
		public static IEnumerable<string> SplitByLength(this string str, int maxLength) {
			int index = 0;
			while(true) {
				if (index + maxLength >= str.Length) {
					yield return str.Substring(index);
					yield break;
				}
				yield return str.Substring(index, maxLength);
				index += maxLength;
			}
		}

		/// <summary>
		/// Remove trailing bytes that has passed value
		/// </summary>
		/// <param name="array">byte array</param>
		/// <param name="valueToCheck">byte value to remove</param>
		/// <returns>shortened byte array</returns>
		public static byte[] RemoveTrailingBytes(byte[] array, byte valueToCheck = 0) {
			int i = array.Length - 1;
			while(i >= 0 && array[i] == valueToCheck) {
				--i;
			}
			
			if (i < 0) {
				// every entry in the array has the valueToCheck value.
				return array;
			} else {
				// now array[i] is the last non-zero byte (if checking for zero)
				var fixedArray = new byte[i+1];
				Array.Copy(array, fixedArray, i+1);
				return fixedArray;
			}
		}
		
		/// <summary>
		/// Return current timestamp (i.e. Now)
		/// </summary>
		/// <returns>string representation of current time</returns>
		public static String GetCurrentTimestamp()
		{
			return GetTimestamp(DateTime.Now);
		}
		
		/// <summary>
		/// Return datetime as string
		/// </summary>
		/// <param name="value">datetime value</param>
		/// <returns>string</returns>
		public static String GetTimestamp(this DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}
		
		/// <summary>
		/// Return a number formatted with plus and minus signs
		/// </summary>
		/// <param name="number">double value</param>
		/// <returns>formatted string</returns>
		public static string GetNumberWithPlussAndMinusSign(double number) {
			return number.ToString("+#;-#;0");
		}
		
		/// <summary>
		/// Convert string to Enum type
		/// </summary>
		/// <param name="name">enum string value</param>
		/// <param name="ignoreCase">whether to ignore case</param>
		/// <returns>Enum</returns>
		/// <example>
		/// DaysOfWeek d = StringToEnum<DaysOfWeek>("Monday");
		/// d is now DaysOfWeek.Monday
		///
		/// MonthsInYear m = StringToEnum<MonthsInYear>("January");
		/// m is now MonthsInYear.January
		/// 
		/// So what happens if you enter a string value that doesn't correspond to an enum? The Enum.Parse will fail with an ArgumentException.
		/// DaysOfWeek d = StringToEnum<DaysOfWeek>("Katillsday");
		/// 	throws an ArgumentException
		/// 	Requested value "Katillsday" was not found.
		///
		/// We can get around this problem by first checking that the enum exists using Enum.IsDefined.
		/// if(Enum.IsDefined(typeof(DaysOfWeek), "Katillsday"))
		///   StringToEnum<DaysOfWeek>("Katillsday");
		/// </example>
		public static T StringToEnum<T>(string name, bool ignoreCase = true)
		{
			return (T) Enum.Parse( typeof( T ), name, ignoreCase );
		}
		
		/// <summary>
		/// Return all the enum values as a list
		/// </summary>
		/// <returns>Enum values as List</returns>
		public static List<T> EnumValuesToList<T>()
		{
			Type enumType = typeof(T);
			
			// Can't use type constraints on value types, so have to do check like this
			if (enumType.BaseType != typeof(Enum))
				throw new ArgumentException("T must be of type System.Enum");

			return new List<T>(Enum.GetValues(enumType) as IEnumerable<T>);
		}
		
		/// <summary>
		/// Test whether passsed object is a numberic value
		/// </summary>
		/// <param name="expression">object to be evaluated</param>
		/// <returns>true if numeric</returns>
		public static System.Boolean IsNumeric(System.Object expression)
		{
			if(expression == null || expression is DateTime)
				return false;

			if(expression is Int16 || expression is Int32 || expression is Int64 || expression is Decimal || expression is Single || expression is Double || expression is Boolean)
				return true;
			
			try {
				if (expression is string)
					Double.Parse(expression as string);
				else
					Double.Parse(expression.ToString());
				return true;
			} catch {
			} // just dismiss errors but return false
			return false;
		}
		
		/// <summary>
		/// Convert Binary String to hex representation
		/// </summary>
		/// <param name="binary">binary string</param>
		/// <returns>hexadecimal string representation</returns>
		public static string BinaryStringToHexString(string binary)
		{
			var result = new StringBuilder(binary.Length / 8 + 1);

			// TODO: check all 1's or 0's... Will throw otherwise

			int mod4Len = binary.Length % 8;
			if (mod4Len != 0)
			{
				// pad to length multiple of 8
				binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
			}

			for (int i = 0; i < binary.Length; i += 8)
			{
				string eightBits = binary.Substring(i, 8);
				result.AppendFormat("{0:x2}", Convert.ToByte(eightBits, 2));
			}

			return result.ToString();
		}
		
		/// <summary>
		/// Convert integer value to binary string
		/// </summary>
		/// <param name="value">integer value</param>
		/// <returns>string</returns>
		public static string IntegerToBinaryString(int value) {
			string binValue = Convert.ToString(value, 2);
			binValue = binValue.PadLeft(32, '0');
			return binValue;
		}

		/// <summary>
		/// Convert Binary String to integer
		/// </summary>
		/// <param name="binary">binary string</param>
		/// <returns>integer value</returns>
		public static int BinaryStringToInteger(string binary) {
			return Convert.ToInt32(binary, 2);
		}

		/// <summary>
		/// Convert long value to binary strign
		/// </summary>
		/// <param name="value">long value</param>
		/// <returns>binary string</returns>
		public static string LongToBinaryString(long value) {
			string binValue = Convert.ToString(value, 2);
			binValue = binValue.PadLeft(64, '0');
			return binValue;
		}

		/// <summary>
		/// Convert Binary String to ulong
		/// </summary>
		/// <param name="binary">binary string</param>
		/// <returns>ulong value</returns>
		public static ulong BinaryStringToLong(string binary) {
			return Convert.ToUInt64(binary, 2);
		}

		/// <summary>
		/// Convert string to byte array
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>byte array</returns>
		public static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		/// <summary>
		/// Convert byte array to string
		/// </summary>
		/// <param name="bytes">byte array</param>
		/// <returns>string</returns>
		public static string GetString(byte[] bytes)
		{
			var chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
		
		
		// Convert the string to Pascal case.
		public static string ToPascalCase(this string the_string)
		{
			// If there are 0 or 1 characters, just return the string.
			if (the_string == null) return the_string;
			if (the_string.Length < 2) return the_string.ToUpper();

			// Split the string into words.
			string[] words = the_string.Split(
				new char[] { },
				StringSplitOptions.RemoveEmptyEntries);

			// Combine the words.
			string result = "";
			foreach (string word in words)
			{
				result +=
					word.Substring(0, 1).ToUpper() +
					word.Substring(1);
			}

			return result;
		}
		
		// Convert the string to camel case.
		public static string ToCamelCase(this string the_string)
		{
			// If there are 0 or 1 characters, just return the string.
			if (the_string == null || the_string.Length < 2) return the_string;

			// Split the string into words.
			string[] words = the_string.Split(
				new char[] { },
				StringSplitOptions.RemoveEmptyEntries);

			// Combine the words.
			string result = words[0].ToLower();
			for (int i = 1; i < words.Length; i++)
			{
				result +=
					words[i].Substring(0, 1).ToUpper() +
					words[i].Substring(1);
			}

			return result;
		}
		
		// Capitalize the first character and add a space before
		// each capitalized letter (except the first character).
		public static string ToProperCase(this string the_string)
		{
			// If there are 0 or 1 characters, just return the string.
			if (the_string == null) return the_string;
			if (the_string.Length < 2) return the_string.ToUpper();

			// Start with the first character.
			string result = the_string.Substring(0, 1).ToUpper();

			// Add the remaining characters.
			for (int i = 1; i < the_string.Length; i++)
			{
				if (Char.IsUpper(the_string[i])) result += " ";
				result += the_string[i];
			}

			return result;
		}
		
		/// <summary>
		/// Return everything in a string after a search word is found
		/// </summary>
		/// <param name="fullString">the full string</param>
		/// <param name="searchfor">a word within the string</param>
		/// <returns>everything in a string after a search word is found</returns>
		public static string GetStringAfterSearchWord(string fullString, string searchfor) {
			
			// we could have used regexp but this method is faster and safer
			string stringAfterSearchWord = "";
			int ix = fullString.IndexOf(searchfor, StringComparison.Ordinal);

			if (ix != -1) {
				stringAfterSearchWord = fullString.Substring(ix + searchfor.Length);
			}
			
			return stringAfterSearchWord;
		}
	}
}
