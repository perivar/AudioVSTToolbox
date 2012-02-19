//----------------------------------------------------------------------------------------
//	Copyright © 2006 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the ability to simulate various classic C string functions
//	which don't have exact equivalents in the .NET Framework.
//----------------------------------------------------------------------------------------
internal static class StringFunctions
{
	//------------------------------------------------------------------------------------
	//	This method allows replacing a single character in a string, to help convert
	//	C++ code where a single character in a character array is replaced.
	//------------------------------------------------------------------------------------
	internal static string ChangeCharacter(string sourcestring, int charindex, char changechar)
	{
		return (charindex > 0 ? sourcestring.Substring(0, charindex) : "")
			+ changechar.ToString() + (charindex < sourcestring.Length - 1 ? sourcestring.Substring(charindex + 1) : "");
	}

	//------------------------------------------------------------------------------------
	//	This method simulates the classic C string function 'isxdigit' (and 'iswxdigit').
	//------------------------------------------------------------------------------------
	internal static bool IsXDigit(char character)
	{
		if (char.IsDigit(character))
			return true;
		else if ("ABCDEFabcdef".IndexOf(character) > -1)
			return true;
		else
			return false;
	}

	//------------------------------------------------------------------------------------
	//	This method simulates the classic C string function 'strchr' (and 'wcschr').
	//------------------------------------------------------------------------------------
	internal static string StrChr(string stringtosearch, char chartofind)
	{
		int index = stringtosearch.IndexOf(chartofind);
		if (index > -1)
			return stringtosearch.Substring(index);
		else
			return null;
	}

	//------------------------------------------------------------------------------------
	//	This method simulates the classic C string function 'strrchr' (and 'wcsrchr').
	//------------------------------------------------------------------------------------
	internal static string StrRChr(string stringtosearch, char chartofind)
	{
		int index = stringtosearch.LastIndexOf(chartofind);
		if (index > -1)
			return stringtosearch.Substring(index);
		else
			return null;
	}

	//------------------------------------------------------------------------------------
	//	This method simulates the classic C string function 'strstr' (and 'wcsstr').
	//------------------------------------------------------------------------------------
	internal static string StrStr(string stringtosearch, string stringtofind)
	{
		int index = stringtosearch.IndexOf(stringtofind);
		if (index > -1)
			return stringtosearch.Substring(index);
		else
			return null;
	}

	//------------------------------------------------------------------------------------
	//	This method simulates the classic C string function 'strtok' (and 'wcstok').
	//	Note that the .NET string 'Split' method cannot be used to simulate 'strtok' since
	//	it doesn't allow changing the delimiters between each token retrieval.
	//------------------------------------------------------------------------------------
	private static string activestring;
	private static int activeposition;
	internal static string StrTok(string stringtotokenize, string delimiters)
	{
		if (stringtotokenize != null)
		{
			activestring = stringtotokenize;
			activeposition = -1;
		}

		//the stringtotokenize was never set:
		if (activestring == null)
			return null;

		//all tokens have already been extracted:
		if (activeposition == activestring.Length)
			return null;

		//bypass delimiters:
		activeposition++;
		while (activeposition < activestring.Length && delimiters.IndexOf(activestring[activeposition]) > -1)
		{
			activeposition++;
		}

		//only delimiters were left, so return null:
		if (activeposition == activestring.Length)
			return null;

		//get starting position of string to return:
		int startingposition = activeposition;

		//read until next delimiter:
		do
		{
			activeposition++;
		} while (activeposition < activestring.Length && delimiters.IndexOf(activestring[activeposition]) == -1);

		return activestring.Substring(startingposition, activeposition - startingposition);
	}
}