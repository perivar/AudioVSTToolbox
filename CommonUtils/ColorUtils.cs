using System;
using System.Drawing;

namespace CommonUtils
{
	/// <summary>
	/// The ColorUtils Class holds utility methods that in one way has to do with Colors
	/// and Color conversions.
	/// Also contains the HSLColor class.
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
		
		public static Color interpolate(Color start, Color end, float p)
		{
			HSLColor startHSL = HSLColor.FromRGB(start);
			HSLColor endHSL = HSLColor.FromRGB(end);
			
			float brightness = (startHSL.Luminosity + endHSL.Luminosity) / 2;
			float saturation = (startHSL.Saturation + endHSL.Saturation) / 2;
			
			float hueMax = 0;
			float hueMin = 0;
			if (startHSL.Hue > endHSL.Hue)
			{
				hueMax = startHSL.Hue;
				hueMin = endHSL.Hue;
			}
			else
			{
				hueMin = startHSL.Hue;
				hueMax = endHSL.Hue;
			}

			float hue = ((hueMax - hueMin) * p) + hueMin;

			return new HSLColor(hue, saturation, brightness).ToRGB();
		}
	}

	public class HSLColor
	{
		public float Hue;
		public float Saturation;
		public float Luminosity;

		public HSLColor(float H, float S, float L)
		{
			Hue = H;
			Saturation = S;
			Luminosity = L;
		}

		public static HSLColor FromRGB(Color Clr)
		{
			return FromRGB(Clr.R, Clr.G, Clr.B);
		}
		
		public static HSLColor FromRGB(Byte R, Byte G, Byte B)
		{
			float _R = (R / 255f);
			float _G = (G / 255f);
			float _B = (B / 255f);

			float _Min = Math.Min(Math.Min(_R, _G), _B);
			float _Max = Math.Max(Math.Max(_R, _G), _B);
			float _Delta = _Max - _Min;

			float H = 0;
			float S = 0;
			float L = (float)((_Max + _Min) / 2.0f);

			if (_Delta != 0)
			{
				if (L < 0.5f)
				{
					S = (float)(_Delta / (_Max + _Min));
				}
				else
				{
					S = (float)(_Delta / (2.0f - _Max - _Min));
				}


				if (_R == _Max)
				{
					H = (_G - _B) / _Delta;
				}
				else if (_G == _Max)
				{
					H = 2f + (_B - _R) / _Delta;
				}
				else if (_B == _Max)
				{
					H = 4f + (_R - _G) / _Delta;
				}
			}

			return new HSLColor(H, S, L);
		}
		
		public Color ToRGB()
		{
			byte r, g, b;
			if (Saturation == 0)
			{
				r = (byte)Math.Round(Luminosity * 255d);
				g = (byte)Math.Round(Luminosity * 255d);
				b = (byte)Math.Round(Luminosity * 255d);
			}
			else
			{
				double t1, t2;
				double th = Hue / 6.0d;

				if (Luminosity < 0.5d)
				{
					t2 = Luminosity * (1d + Saturation);
				}
				else
				{
					t2 = (Luminosity + Saturation) - (Luminosity * Saturation);
				}
				t1 = 2d * Luminosity - t2;

				double tr, tg, tb;
				tr = th + (1.0d / 3.0d);
				tg = th;
				tb = th - (1.0d / 3.0d);

				tr = ColorCalc(tr, t1, t2);
				tg = ColorCalc(tg, t1, t2);
				tb = ColorCalc(tb, t1, t2);
				r = (byte)Math.Round(tr * 255d);
				g = (byte)Math.Round(tg * 255d);
				b = (byte)Math.Round(tb * 255d);
			}
			return Color.FromArgb(r, g, b);
		}
		
		private static double ColorCalc(double c, double t1, double t2)
		{

			if (c < 0) c += 1d;
			if (c > 1) c -= 1d;
			if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
			if (2.0d * c < 1.0d) return t2;
			if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
			return t1;
		}
	}
}

