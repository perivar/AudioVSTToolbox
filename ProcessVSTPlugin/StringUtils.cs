using System;
using System.Text;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of StringUtils.
	/// </summary>
	public class StringUtils
	{
		public StringUtils()
		{
		}
		
		/// <summary>
		/// Converts the phrase to specified convention.
		/// </summary>
		/// <param name="phrase"></param>
		/// <param name="cases">The cases.</param>
		/// <returns>string</returns>
		public static string ConvertCaseString(string phrase, Case cases) {
			/*
				string a = "background color-red.brown";
				string camelCase = ConvertCaseString(a, Case.CamelCase);
				string pascalCase = ConvertCaseString(a, Case.PascalCase);
				Console.WriteLine(String.Format("Original: {0}", a));
				Console.WriteLine(String.Format("Camel Case: {0}", camelCase));
				Console.WriteLine(String.Format("Pascal Case: {0}", pascalCase));
			 */

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

		public enum Case {
			PascalCase,
			CamelCase
		}
		
		public static string ToHexString(byte b) {
			char c = (char) b;
			string s = String.Format("{0,0:X2}", (int) c);
			return s;
		}
		
		public static string ToHexAndAsciiString(byte[] b) {
			StringBuilder strb = new StringBuilder();
			StringBuilder text = new StringBuilder();
			if (b != null) {
				char [] ch = new char [1];
				for (int x = 0; x < b.Length; x++)
				{
					ch[0] = (char) b[x];
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
	}
}
