using System;
using System.Drawing;
using System.Drawing.Imaging;

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
		
		// HSV stands for hue, saturation, and value, and is also often called HSB (B for brightness)
		// alpha channel, hue, saturation, and brightness
		public static Color AhsbToArgb(byte a, double h, double s, double b)
		{
			var color = HsbToRgb(h, s, b);
			return Color.FromArgb(a, color.R, color.G, color.B);
		}
		
		// HSV stands for hue, saturation, and value,
		// and is also often called HSB (B for brightness)
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

		// HSL stands for hue, saturation, and lightness, and is often also called HLS
		// alpha channel, hue, saturation, and brightness
		public static Color HslToRgb(float h, float s, float l)
		{
			return AhslToArgb(255, h, s, l);
		}
		
		// HSL stands for hue, saturation, and lightness, and is often also called HLS
		public static Color AhslToArgb(int a, float h, float s, float l) {

			if (0 > a || 255 < a) {
				throw new ArgumentOutOfRangeException("a", a,
				                                      "InvalidAlpha");
			}
			if (0f > h || 360f < h) {
				throw new ArgumentOutOfRangeException("h", h,
				                                      "InvalidHue");
			}
			if (0f > s || 1f < s) {
				throw new ArgumentOutOfRangeException("s", s,
				                                      "InvalidSaturation");
			}
			if (0f > l || 1f < l) {
				throw new ArgumentOutOfRangeException("l", l,
				                                      "InvalidBrightness");
			}

			if (0 == s) {
				return Color.FromArgb(a, Convert.ToInt32(l * 255),
				                      Convert.ToInt32(l * 255), Convert.ToInt32(l * 255));
			}

			float fMax, fMid, fMin;
			int iSextant, iMax, iMid, iMin;

			if (0.5 < l) {
				fMax = l - (l * s) + s;
				fMin = l + (l * s) - s;
			} else {
				fMax = l + (l * s);
				fMin = l - (l * s);
			}

			iSextant = (int)Math.Floor(h / 60f);
			if (300f <= h) {
				h -= 360f;
			}
			h /= 60f;
			h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
			if (0 == iSextant % 2) {
				fMid = h * (fMax - fMin) + fMin;
			} else {
				fMid = fMin - h * (fMax - fMin);
			}
			
			iMax = Convert.ToInt32(fMax * 255);
			iMid = Convert.ToInt32(fMid * 255);
			iMin = Convert.ToInt32(fMin * 255);

			switch (iSextant) {
				case 1:
					return Color.FromArgb(a, iMid, iMax, iMin);
				case 2:
					return Color.FromArgb(a, iMin, iMax, iMid);
				case 3:
					return Color.FromArgb(a, iMin, iMid, iMax);
				case 4:
					return Color.FromArgb(a, iMid, iMin, iMax);
				case 5:
					return Color.FromArgb(a, iMax, iMin, iMid);
				default:
					return Color.FromArgb(a, iMax, iMid, iMin);
			}
		}
		
		public static void drawColorGradient(string directory, string filename, bool useHSL) {
			
			string mode = "";
			if (useHSL) {
				mode = "HSL";
			} else {
				mode = "HSB";
			}
			String filenameToSave = String.Format("{0}/{1}_{2}.png", directory, System.IO.Path.GetFileNameWithoutExtension(filename), mode);
			System.Console.Out.WriteLine("Writing " + filenameToSave);
			
			int width = 360;
			int height = 200;
			
			// Create the image for displaying the data.
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);

			float saturation = 1.0f;
			
			// http://en.wikipedia.org/wiki/HSL_and_HSV
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					float brightness = 1 - ((float)y / height);
					
					Color c = Color.White;
					if (useHSL) {
						// HSL
						c = ColorUtils.AhslToArgb(255, x, saturation, brightness);
					} else {
						// HSB
						c = ColorUtils.AhsbToArgb(255, x, saturation, brightness);
					}
					
					png.SetPixel(x, y, c);
				}
			}
			
			png.Save(filenameToSave);
			g.Dispose();
		}		
	}
}
