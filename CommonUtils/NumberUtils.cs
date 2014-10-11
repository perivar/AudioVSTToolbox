using System;
using System.Linq;
using System.Globalization;

namespace CommonUtils
{
	/// <summary>
	/// Assorted number methods that might be helpful
	/// </summary>
	public static class NumberUtils
	{

		public static Decimal DecimalTryParseOrZero(String input) {
			Decimal d;
			Decimal.TryParse(input, out d);
			return d;
		}

		public static Decimal DecimalTryParse(String input, decimal defaultValue) {
			Decimal result;
			if (!Decimal.TryParse(input, out result)) {
				result = defaultValue;
			}
			return result;
		}

		public static Boolean BooleanTryParseOrZero(String str)
		{
			return BooleanTryParse(str, false);
		}

		public static Boolean BooleanTryParse(String str, Boolean bDefault)
		{
			String[] BooleanStringOff = { "0", "off", "no", "false" };

			if (str == null) {
				return bDefault;
			} else if (str.Equals("")) {
				return bDefault;
			} else if(BooleanStringOff.Contains(str,StringComparer.InvariantCultureIgnoreCase)) {
				return false;
			}

			Boolean result;
			if (!Boolean.TryParse(str, out result)) {
				result = true;
			}

			return result;
		}
		
		public static double DoubleTryParse(string value, double defaultValue) {
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
	}
}
