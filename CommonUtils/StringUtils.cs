using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace CommonUtils
{
	/// <summary>
	/// Description of StringUtils.
	/// </summary>
	public static class StringUtils
	{
		public enum Case {
			PascalCase,
			CamelCase
		}

		
		/// <summary>
		/// Converts the phrase to specified convention.
		/// </summary>
		/// <param name="phrase"></param>
		/// <param name="cases">The cases.</param>
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
		
		public static string ToHexString(byte b) {
			char c = (char) b;
			string s = String.Format("{0,0:X2}", (int) c);
			return s;
		}
		
		public static string ToHexAndAsciiString(byte[] b, bool invert) {
			StringBuilder strb = new StringBuilder();
			StringBuilder text = new StringBuilder();
			if (b != null) {
				byte [] bClone = (byte[])b.Clone();
				if (invert) {
					Array.Reverse(bClone);
				}
				char [] ch = new char [1];
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
		
		public static string RemoveInvalidCharacters(string strIn) {
			// Replace invalid characters with empty strings.
			return Regex.Replace(strIn, @"[^\w\.@-]", "");
		}
				
		public static string MakeValidFileName( string name )
		{
			string invalidChars = Regex.Escape( new string( Path.GetInvalidFileNameChars() ) );
			string invalidReStr = string.Format( @"[{0}]+", invalidChars );
			return Regex.Replace( name, invalidReStr, "_" );
		}
		
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
				byte[] fixedArray = new byte[i+1];
				Array.Copy(array, fixedArray, i+1);
				return fixedArray;
			}
		}
	}
}
