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
		
		public static uint ColorToUInt(Color color)
		{
			return (uint)((color.A << 24) | (color.R << 16) |
			              (color.G << 8)  | (color.B << 0));
		}
		
		public static Color UIntToColor(uint color)
		{
			byte a = (byte)(color >> 24);
			byte r = (byte)(color >> 16);
			byte g = (byte)(color >> 8);
			byte b = (byte)(color >> 0);
			return Color.FromArgb(a, r, g, b);
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
