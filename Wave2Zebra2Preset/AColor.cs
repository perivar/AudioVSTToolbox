/*
 * AColor routine, copied alot from AColor (Audacity)
 * User: perivar.nerseth
 * Date: 01.08.2011
 * Time: 17:55
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wave2ZebraSynth
{
	/// <summary>
	/// Description of AColor.
	/// </summary>
	public class AColor
	{
		
		// Member variables
		public static bool gradient_inited = false;
		public const int gradientSteps = 512;
		public static byte[,,,] gradient_pre = new byte[2, 2, gradientSteps, 3];
		public static Dictionary<int, Color> gradient_preloaded = new Dictionary<int, Color>();

		/**
		 * Constant defining that the visible section of the signal is not changed
		 * if a section is highlighted.
		 */
		public static int ALIGN_NONE = 0;

		/**
		 * Constant defining that the visible section of the signal is centered
		 * around a given sample number (if the sample number is > 0) or the
		 * highlighted section is centered in the visible section (it the given
		 * sample number equals zero).
		 */
		public static int ALIGN_CENTER = 1;

		/**
		 *
		 */
		public static int ALIGN_RIGHT = 2;

		/**
		 *
		 */
		public static int ALIGN_LEFT = 3;

		/**
		 * constant defining the minimum number of samples shown per pixel
		 */
		public static double MIN_SAMPLES_PER_PIXEL = 0.1;

		/**
		 * constant defining the maximum number of samples shown per pixel
		 */
		public static double MAX_SAMPLES_PER_PIXEL = 1000;

		/**
		 * samplerate of the audio signal
		 */
		public int samplerate = 16000;

		/**
		 * number of the first sample of the highlighted section
		 */
		public int highlightedSectionStartSample = 16000;

		/**
		 * number of the last sample of the highlighted section
		 */
		public int highlightedSectionEndSample = 48000;

		/**
		 * indicates whether an area of the signal is highlighted or not
		 */
		public bool isHighlighted = false;

		/**
		 * number of the first sample of the selected section
		 */
		public int selectedSectionStartSample = 0;

		/**
		 * number of the last sample of the selected section
		 */
		public int selectedSectionEndSample = 0;

		/**
		 * indicates whether an area of the signal has been selected by mouse
		 * dragging or not
		 */
		protected bool isSelected = false;

		/**
		 * indicates whether a single sample of the signal has been selected by mouse
		 * click or not
		 */
		protected bool isMarked = false;

		/**
		 * color of the background of the highlighted section of the signal
		 */
		public static Color colorBackgroundHighlightedSection = Color.FromArgb(231, 221, 197);

		/**
		 * color of the highlighted section of the signal
		 */
		public static Color colorHighlightedSignal = Color.FromArgb(120, 103, 75);

		/**
		 * color of the background of the selected section of the signal
		 */
		public static Color colorBackgroundSelectedArea = Color.FromArgb(230, 230, 230);

		/**
		 * color of the selected section of the signal
		 */
		public static Color colorSelectedSignal = Color.FromArgb(120, 120, 120);

		/**
		 * defines whether the highlighted area is actually shown highlighted or not
		 */
		public bool showHighlightedSection = true;

		
		public static Color DetermineColor(int f, bool selected) {
			bool colorSpectrogram = false;
			double brightness = 0.5f;
			
			int c1_r = colorBackgroundHighlightedSection.R;
			int c1_g = colorBackgroundHighlightedSection.G;
			int c1_b = colorBackgroundHighlightedSection.B;
			int c2_r = colorHighlightedSignal.R;
			int c2_g = colorHighlightedSignal.G;
			int c2_b = colorHighlightedSignal.B;

			if (colorSpectrogram) {
				Color c = HSBtoColor((255 - f) / 360.0f, 1.0f,
				                   (float) (brightness + (1.0 - brightness) * f / 255.0));

				if (selected) {
					int f1 = (c.R * c1_r + (255 - c.R) * c2_r) / 255;
					int f2 = (c.G * c1_g + (255 - c.G) * c2_g) / 255;
					int f3 = (c.B * c1_b + (255 - c.B) * c2_b) / 255;
					return Color.FromArgb(f1, f2, f3);
				}
				return c;
			} else {
				f *= (int)((1 - brightness) * 2);
				if (f > 255) {
					f = 255;
				}

				if (selected) {
					int f1 = ((255 - f) * c1_r + f * c2_r) / 255;
					int f2 = ((255 - f) * c1_g + f * c2_g) / 255;
					int f3 = ((255 - f) * c1_b + f * c2_b) / 255;
					return Color.FromArgb(f1, f2, f3);
				}
				return Color.FromArgb(255 - f, 255 - f, 255 - f);
			}

		}

		// HELPER FUNCTIONS
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

		
		
		public static Color GetColorGradient(float percentage)
		{
			if (!AColor.gradient_inited) {
				AColor.PreComputeGradient2();
			}
			
			float i = percentage * (AColor.gradientSteps - 1);
			int idx =  (int) i;
			
			Color c = Color.White;
			try {
				c = gradient_preloaded[idx];
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine("Error looking up color id: " + idx);
				System.Diagnostics.Debug.WriteLine(e);
			}
			return c;
		}

		
		public static Color GetColorGradient(float value, bool selected, bool grayscale)
		{
			if (!AColor.gradient_inited) {
				AColor.PreComputeGradient();
			}

			int idx = (int)(value * (AColor.gradientSteps - 1));

			// TODO: select
			return Color.Aquamarine;
		}

		public static void PreComputeGradient()
		{
			if (!gradient_inited)
			{
				gradient_inited = true;
				
				for (int selected = 0; selected <= 1; selected++)
					for (int grayscale = 0; grayscale <= 1; grayscale++)
				{
					float r;
					float g;
					float b;
					
					int i;
					for (i = 0; i<gradientSteps; i++)
					{
						float value = (float)i/gradientSteps;
						
						if (grayscale != 0)
						{
							r = g = b = 0.84f - 0.84f * value;
						}
						else
						{
							int gsteps = 4;
							float[,] gradient = {
								{(float)(0.75), (float)(0.75), (float)(0.75)},
								{(float)(0.30), (float)(0.60), (float)(1.00)},
								{(float)(0.90), (float)(0.10), (float)(0.90)},
								{(float)(1.00), (float)(0.00), (float)(0.00)},
								{(float)(1.00), (float)(1.00), (float)(1.00)} };
							
							int left = (int) value * gsteps;
							int right = (left == gsteps && left != 0 ? gsteps : left + 1);
							
							float rweight = (value * gsteps) - left;
							float lweight = 1.0f - rweight;
							
							r = (gradient[left, 0] * lweight) + (gradient[right, 0] * rweight);
							g = (gradient[left, 1] * lweight) + (gradient[right, 1] * rweight);
							b = (gradient[left, 2] * lweight) + (gradient[right, 2] * rweight);
						}
						
						if (selected != 0)
						{
							r *= 0.77f;
							g *= 0.77f;
							b *= 0.885f;
						}
						gradient_pre[selected, grayscale, i, 0] = (byte)(255 * r);
						gradient_pre[selected, grayscale, i, 1] = (byte)(255 * g);
						gradient_pre[selected, grayscale, i, 2] = (byte)(255 * b);
					}
				}
			}
		}
		
		public static void PreComputeGradient2()
		{
			if (!gradient_inited)
			{
				gradient_inited = true;
				
				float r;
				float g;
				float b;
				
				int i;
				for (i = 0; i < gradientSteps; i++)
				{
					float value = (float)i/gradientSteps;
					
					int gsteps = 4;
					float[,] gradient = {
						{(float)(0.75), (float)(0.75), (float)(0.75)},
						{(float)(0.30), (float)(0.60), (float)(1.00)},
						{(float)(0.90), (float)(0.10), (float)(0.90)},
						{(float)(1.00), (float)(0.00), (float)(0.00)},
						{(float)(1.00), (float)(1.00), (float)(1.00)} };
					
					int left = (int) value * gsteps;
					int right = (left == gsteps && left != 0 ? gsteps : left + 1);
					
					float rweight = (value * gsteps) - left;
					float lweight = 1.0f - rweight;
					
					r = (gradient[left, 0] * lweight) + (gradient[right, 0] * rweight);
					g = (gradient[left, 1] * lweight) + (gradient[right, 1] * rweight);
					b = (gradient[left, 2] * lweight) + (gradient[right, 2] * rweight);

					if (r < 0 || r > 1) r = 1;
					if (g < 0 || g > 1) g = 1;
					if (b < 0 || b > 1) b = 1;
					
					byte red = (byte)(255 * r);
					byte green = (byte)(255 * g);
					byte blue = (byte)(255 * b);
					
					Color c = Color.FromArgb(red, green, blue);
					gradient_preloaded.Add(i, c);
					//System.Diagnostics.Debug.WriteLine(String.Format("Save Color: {0}={1}", i, c));
				}
			}
		}
		
		// Given H,S,L in range of 0-1
		// Returns a Color (RGB struct) in range of 0-255
		// HSV stands for hue, saturation, and value, and is also often called HSB (B for brightness).
		public static ColorRGB HSL2RGB(double h, double sl, double l)
		{
			double v;
			double r,g,b;

			r = l;   // default to gray
			g = l;
			b = l;

			v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
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
			ColorRGB rgb;
			rgb.R = Convert.ToByte(r * 255.0f);
			rgb.G = Convert.ToByte(g * 255.0f);
			rgb.B = Convert.ToByte(b * 255.0f);
			return rgb;
		}

		// Given a Color (RGB Struct) in range of 0-255
		// Return H,S,L in range of 0-1
		public static void RGB2HSL (ColorRGB rgb, out double h, out double s, out double l)
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
		
		//public static Color AHSBtoRGB(byte a, double h, double s, double b)
		//{
		//	var color = HSBtoColor(h, s, b);
		//	return Color.FromArgb(a, color.R, color.G, color.B);
		//}
		
		// HSV stands for hue, saturation, and value, 
		// and is also often called HSB (B for brightness).
		public static Color HSVToColor(float[] paintColor) {
			if (paintColor.Length == 3) {
				return HSBtoColor(paintColor[0], paintColor[1], paintColor[2]);
			} else {
				return Color.DeepPink;
			}
		}
		
		// HSV stands for hue, saturation, and value, 
		// and is also often called HSB (B for brightness).
		public static Color HSVToColor(double h, double s, double b) {
			return HSBtoColor(h, s, b);
		}
		
		// HSV stands for hue, saturation, and value, 
		// and is also often called HSB (B for brightness).
		public static Color HSBtoColor(double h, double s, double b)
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
		
		//
		// Draw a line while accounting for differences in wxWidgets versions
		//
		public static void Line(Graphics g, int x1, int y1, int x2, int y2)
		{
			bool point = false;
			
			if (x1 == x2)
			{
				if (y1 < y2)
				{
					y2++;
				}
				else if (y2 < y1)
				{
					y1++;
				}
				else
				{
					point = true;
				}
			}
			else if (y1 == y2)
			{
				if (x1 < x2)
				{
					x2++;
				}
				else if (x2 < x1)
				{
					x1++;
				}
				else
				{
					point = true;
				}
			}
			else
			{
				//g.DrawPoint(x2, y2);
			}
			
			if (point)
			{
				//g.DrawPoint(x2, y2);
			}
			else
			{
				//g.DrawLine(x1, y1, x2, y2);
			}
		}
		
	}
	
	public struct ColorRGB
	{

		public byte R;
		public byte G;
		public byte B;

		public ColorRGB(Color value)
		{

			this.R = value.R;
			this.G = value.G;
			this.B = value.B;
		}

		public static implicit operator Color(ColorRGB rgb)
		{
			Color c = Color.FromArgb(rgb.R,rgb.G,rgb.B);
			return c;
		}

		public static explicit operator ColorRGB(Color c)
		{
			return new ColorRGB(c);
		}
	}
	
}
