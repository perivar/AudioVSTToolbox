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
	}
}
