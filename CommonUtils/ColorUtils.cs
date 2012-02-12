using System;
using System.Drawing;

namespace CommonUtils
{
	/// <summary>
	/// Description of ColorUtils.
	/// </summary>
	public static class ColorUtils
	{
		public static long ColorToLong(Color color)
		{
			return (long)((color.A << 24) | (color.R << 16) |
			              (color.G << 8)  | (color.B << 0));
		}
		
		public static Color LongToColor(long color)
		{
			return UIntToColor((uint) color);
		}
		
		public static uint ColorToUInt(Color color)
		{
			return (uint)((color.A << 24) | (color.R << 16) |
			              (color.G << 8)  | (color.B << 0));
		}
		
		// white = 0xFFFFFFFF, black = 0xFF000000
		public static Color UIntToColor(uint color)
		{
			uint transparency = (color & 0xff000000) >> 24;
			uint red = (color & 0xff0000) >> 16;
			uint green = (color & 0x00ff00) >> 8;
			uint blue = color & 0x0000ff;
			
			return Color.FromArgb((int)transparency, (int)red, (int)green, (int)blue);
		}
		
		// Color conversion
		public static Color IntToColor(int number) {
			byte[] values = BitConverter.GetBytes(number);
			
			if (!BitConverter.IsLittleEndian) Array.Reverse(values);
			
			// The array will have four bytes. The first three bytes contain your number:
			byte b = values[0];
			byte g = values[1];
			byte r = values[2];
			
			Color c = Color.FromArgb( r, g, b );
			return c;
		}
		
		// http://stackoverflow.com/questions/2900837/does-the-net-framework-3-5-have-an-hsbtorgb-converter-or-do-i-need-to-roll-my-o
		// alpha channel, hue, saturation, and brightness
		public static Color AhsbToArgb(byte a, double h, double s, double b)
		{
			var color = HsbToRgb(h, s, b);
			return Color.FromArgb(a, color.R, color.G, color.B);
		}
		
		// HSV stands for hue, saturation, and value,
		// and is also often called HSB (B for brightness).
		public static Color HsvToRgb(double h, double s, double b) {
			return HsbToRgb(h, s, b);
		}
		
		// HSV stands for hue, saturation, and value,
		// and is also often called HSB (B for brightness).
		public static Color HsbToRgb(double h, double s, double b)
		{
			if (s == 0)
				return RawRgbToRgb(b, b, b);
			else
			{
				var sector = h / 60;
				var sectorNumber = (int)Math.Truncate(sector);
				var sectorFraction = sector - sectorNumber;
				var b1 = b * (1 - s);
				var b2 = b * (1 - s * sectorFraction);
				var b3 = b * (1 - s * (1 - sectorFraction));
				switch (sectorNumber)
				{
					case 0:
						return RawRgbToRgb(b, b3, b1);
					case 1:
						return RawRgbToRgb(b2, b, b1);
					case 2:
						return RawRgbToRgb(b1, b, b3);
					case 3:
						return RawRgbToRgb(b1, b2, b);
					case 4:
						return RawRgbToRgb(b3, b1, b);
					case 5:
						return RawRgbToRgb(b, b1, b2);
					default:
						throw new ArgumentException("Hue must be between 0 and 360");
				}
			}
		}
		
		private static Color RawRgbToRgb(double rawR, double rawG, double rawB)
		{
			return Color.FromArgb(
				(int)Math.Round(rawR * 255),
				(int)Math.Round(rawG * 255),
				(int)Math.Round(rawB * 255));
		}
	}
}
