using System;
using System.Drawing;
using System.Collections.Generic;

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
		
		/// <summary>
		/// HSV stands for hue, saturation, and value,
		/// and is also often called HSB (B for brightness)
		/// </summary>
		/// <param name="a">Alpha (0 - 255)</param>
		/// <param name="h">Hue (0 - 360)</param>
		/// <param name="s">Saturation (0 - 1)</param>
		/// <param name="b">Brightness (0 - 1)</param>
		/// <returns>Color</returns>
		public static Color AhsbToArgb(byte a, double h, double s, double b)
		{
			var color = HsbToRgb(h, s, b);
			return Color.FromArgb(a, color.R, color.G, color.B);
		}
		
		/// <summary>
		/// HSV stands for hue, saturation, and value,
		/// and is also often called HSB (B for brightness)
		/// </summary>
		/// <param name="h">Hue (0 - 360)</param>
		/// <param name="s">Saturation (0 - 1)</param>
		/// <param name="b">Brightness (0 - 1)</param>
		/// <returns>Color</returns>
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

		/// <summary>
		/// HSL stands for hue, saturation, and lightness, and is often also called HLS
		/// </summary>
		/// <param name="h">Hue (0 - 360)</param>
		/// <param name="s">Saturation (0 - 1)</param>
		/// <param name="l">Lightness (0 - 1)</param>
		/// <returns>Color (RGB)</returns>
		public static Color HslToRgb(float h, float s, float l)
		{
			return AhslToArgb(255, h, s, l);
		}
		
		/// <summary>
		/// HSL stands for hue, saturation, and lightness, and is often also called HLS
		/// </summary>
		/// <param name="a">Alpha (0 - 255)</param>
		/// <param name="h">Hue (0 - 360)</param>
		/// <param name="s">Saturation (0 - 1)</param>
		/// <param name="l">Lightness (0 - 1)</param>
		/// <returns>Color (RGB)</returns>
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
		
		/// <summary>
		/// Given H,S,L in range of 0-1
		/// Returns a Color (RGB struct) in range of 0-255
		/// http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </summary>
		/// <param name="h">Hue (0 - 1)</param>
		/// <param name="sl">Saturation (0 - 1)</param>
		/// <param name="l">Luminosity (0 - 1)</param>
		/// <returns>Color (RGB)</returns>
		[Obsolete("HSL2RGB is deprecated, please use HslToRgb instead.", true)]
		public static Color HSL2RGB(double h, double s, double l)
		{
			double v;
			double r,g,b;
			
			r = l;   // default to gray
			g = l;
			b = l;
			v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
			if (v > 0)
			{
				double m;
				double sv;
				int sextant;
				double fract, vsf, mid1, mid2;
				
				m = l + l - v;
				sv = (v - m ) / v;
				h *= 6.0;
				sextant = (int)h;
				fract = h - sextant;
				vsf = v * sv * fract;
				mid1 = m + vsf;
				mid2 = v - vsf;
				switch (sextant)
				{
					case 0:
						r = v;
						g = mid1;
						b = m;
						break;
					case 1:
						r = mid2;
						g = v;
						b = m;
						break;
					case 2:
						r = m;
						g = v;
						b = mid1;
						break;
					case 3:
						r = m;
						g = mid2;
						b = v;
						break;
					case 4:
						r = mid1;
						g = m;
						b = v;
						break;
					case 5:
						r = v;
						g = m;
						b = mid2;
						break;
				}
			}
			
			Color rgb = Color.FromArgb(Convert.ToByte(r * 255.0f),
			                           Convert.ToByte(g * 255.0f),
			                           Convert.ToByte(b * 255.0f));
			return rgb;
		}
		
		/// <summary>
		/// Given a Color (RGB Struct) in range of 0-255
		/// Return H,S,L in range of 0-1
		/// http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm
		/// </summary>
		/// <param name="rgb">Color object (RGB)</param>
		/// <param name="h">Hue (0 - 1)</param>
		/// <param name="sl">Saturation (0 - 1)</param>
		/// <param name="l">Luminosity (0 - 1)</param>
		public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
		{
			double r = rgb.R/255.0;
			double g = rgb.G/255.0;
			double b = rgb.B/255.0;
			double v;
			double m;
			double vm;
			double r2, g2, b2;
			
			h = 0; // default to black
			s = 0;
			l = 0;
			v = Math.Max(r,g);
			v = Math.Max(v,b);
			m = Math.Min(r,g);
			m = Math.Min(m,b);
			l = (m + v) / 2.0;
			if (l <= 0.0)
			{
				return;
			}
			vm = v - m;
			s = vm;
			if (s > 0.0)
			{
				s /= (l <= 0.5) ? (v + m ) : (2.0 - v - m) ;
			}
			else
			{
				return;
			}
			r2 = (v - r) / vm;
			g2 = (v - g) / vm;
			b2 = (v - b) / vm;
			if (r == v)
			{
				h = (g == m ? 5.0 + b2 : 1.0 - g2);
			}
			else if (g == v)
			{
				h = (b == m ? 1.0 + r2 : 3.0 - b2);
			}
			else
			{
				h = (r == m ? 3.0 + g2 : 5.0 - r2);
			}
			h /= 6.0;
		}
		
		/// <summary>
		/// Create a list of gradient colors based on the input list
		/// </summary>
		/// <param name="steps">number of gradients to create</param>
		/// <param name="colors">the list of colors to transition between</param>
		/// <returns>a List of gradients</returns>
		public static List<HSLColor2> HSBGradient(int steps, List<HSLColor2> colors) {
			int parts = colors.Count - 1;
			
			List<HSLColor2> gradients = new List<HSLColor2>();
			
			double partSteps = Math.Floor((double)steps / parts);
			double remainder = steps - (partSteps * parts);
			for (int col = 0; col < parts; col++) {
				// get colors
				HSLColor2 c1 = colors[col];
				HSLColor2 c2 = colors[col + 1];
				
				// determine clockwise and counter-clockwise distance between hues
				double distCCW = (c1.Hue >= c2.Hue) ? c1.Hue - c2.Hue : 1 + c1.Hue - c2.Hue;
				double distCW = (c1.Hue >= c2.Hue) ? 1 + c2.Hue - c1.Hue : c2.Hue - c1.Hue;
				
				// ensure we get the right number of steps by adding remainder to final part
				if (col == parts - 1) partSteps += remainder;
				
				// make gradient for this part
				for (int step = 0; step < partSteps; step ++) {
					double p = step / partSteps;
					
					// interpolate h, s, b
					double h = (distCW <= distCCW) ? c1.Hue + (distCW * p) : c1.Hue - (distCCW * p);
					
					if (h < 0) h = 1 + h;
					if (h > 1) h = h - 1;
					double s = (1 - p) * c1.Saturation + p * c2.Saturation;
					double b = (1 - p) * c1.Luminosity + p * c2.Luminosity;
					
					// add to gradient array
					gradients.Add(new HSLColor2(h, s, b));
				}
			}
			return gradients;
		}
	}

	/// <summary>
	/// Wrapper Class to hold HSL Values
	/// </summary>
	public class HSLColor2 {
		
		/// <summary>
		/// Hue (0 - 1)
		/// </summary>
		public double Hue;
		
		/// <summary>
		/// Saturation (0 - 1)
		/// </summary>
		public double Saturation;
		
		/// <summary>
		/// Luminosity (0 - 1)
		/// </summary>
		public double Luminosity;
		
		public HSLColor2(double h, double s, double l) {
			this.Hue = h;
			this.Saturation = s;
			this.Luminosity = l;
		}

		/// <summary>
		/// Return a Color (RGB) object
		/// </summary>
		public Color Color {
			get {
				//return ColorUtils.HSL2RGB(this.Hue, this.Saturation, this.Luminosity);
				return ColorUtils.HslToRgb((float)this.Hue*360, (float)this.Saturation, (float)this.Luminosity);
			}
		}

		public static HSLColor2 FromRGB(Color color)
		{
			double h;
			double s;
			double l;
			ColorUtils.RGB2HSL(color, out h, out s, out l);
			return new HSLColor2(h, s, l);
		}
		
		public override string ToString()
		{
			return
				Hue + ";" +
				Saturation + ";" +
				Luminosity + ";";
		}
	}
	
	/// <summary>
	/// Convert from HSL Color space to RGB Color space and back
	/// http://stackoverflow.com/questions/4793729/rgb-to-hsl-and-back-calculation-problems
	/// </summary>
	public class HSLColor
	{
		/// <summary>
		/// Hue (0 - 6), Hue can actually be negative sometimes
		/// </summary>
		public float Hue;
		
		/// <summary>
		/// Saturation (0 - 1)
		/// </summary>
		public float Saturation;
		
		/// <summary>
		/// Luminosity (0 - 1)
		/// </summary>
		public float Luminosity;
		
		/// <summary>
		/// Hue (0 - 360)
		/// </summary>
		public float Hue360 { get {
				// The H value returned should be from 0 to 6 so to convert it to degrees
				// you just multiply by 60.
				// H can actually be negative sometimes so if it is just add 360;
				float H = Hue * 60f;
				if (H < 0) H += 360;
				return H;
			}
		}

		/// <summary>
		/// Create a HSLColor object
		/// </summary>
		/// <param name="H">Hue (0 - 6), Hue can actually be negative sometimes</param>
		/// <param name="S">Saturation (0 - 1)</param>
		/// <param name="L">Luminosity (0 - 1)</param>
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

