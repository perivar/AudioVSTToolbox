using System;
using System.Drawing;
using System.Drawing.Imaging;
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
		public enum ColorPaletteType {
			REW = 1,
			SOX = 2,
			PHOTOSOUNDER = 3,
			BLACK_AND_WHITE = 4
		}
		
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
		/// http://stackoverflow.com/questions/4123998/algorithm-to-switch-between-rgb-and-hsb-color-values
		/// </summary>
		/// <param name="h">Hue (0 - 360)</param>
		/// <param name="s">Saturation (0 - 1)</param>
		/// <param name="b">Brightness (0 - 1)</param>
		/// <returns>Color</returns>
		public static Color HsbToRgb(double hue, double saturation, double brightness)
		{
			if (saturation == 0)
				return RawRgbToRgb(brightness, brightness, brightness);
			else
			{
				// the color wheel consists of 6 sectors. Figure out which sector you're in.
				double sectorPos = hue / 60.0;
				int sectorNumber = (int)(Math.Floor(sectorPos));
				//var sectorNumber = (int)Math.Truncate(sector);

				// get the fractional part of the sector
				double fractionalSector = sectorPos - sectorNumber;
				
				// calculate values for the three axes of the color.
				double p = brightness * (1.0 - saturation);
				double q = brightness * (1.0 - (saturation * fractionalSector));
				double t = brightness * (1.0 - (saturation * (1 - fractionalSector)));
				switch (sectorNumber)
				{
					case 0:
						return RawRgbToRgb(brightness, t, p);
					case 1:
						return RawRgbToRgb(q, brightness, p);
					case 2:
						return RawRgbToRgb(p, brightness, t);
					case 3:
						return RawRgbToRgb(p, q, brightness);
					case 4:
						return RawRgbToRgb(t, p, brightness);
					case 5:
						return RawRgbToRgb(brightness, p, q);
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
		/// Convert a Color to HSB (Also called HSV)
		/// </summary>
		/// <param name="color">Color (RGB)</param>
		/// <param name="hue">Hue (0 - 360)</param>
		/// <param name="saturation">Saturation (0 - 1)</param>
		/// <param name="brightness">Brightness (0 - 1)</param>
		public static void RgbToHsb(Color color, out double hue, out double saturation, out double brightness)
		{
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			hue = color.GetHue();

			// Calculate the saturation (between 0 and 1)
			saturation = (max == 0) ? 0 : 1d - (1d * min / max);
			
			brightness = max / 255d;
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
		/// Given a Color (RGB Struct) in range of 0-255
		/// Return H,S,L in range of 0-1
		/// </summary>
		/// <param name="rgb">Color object (RGB)</param>
		/// <param name="hue">Hue (0 - 360)</param>
		/// <param name="sl">Saturation (0 - 1)</param>
		/// <param name="l">Luminosity (0 - 1)</param>
		public static void RgbToHsl(Color rgb, out double h, out double s, out double l)
		{
			h = rgb.GetHue();
			s = rgb.GetSaturation();
			l = rgb.GetBrightness();
		}
		
		/// <summary>
		/// Linear interpolation using the RGB color space
		/// </summary>
		/// <param name="start">color start</param>
		/// <param name="end">color end</param>
		/// <param name="colorCount">number of gradients (colors) to create</param>
		/// <returns>a List of Colors</returns>
		public static List<Color> RgbLinearInterpolate(Color start, Color end, int colorCount)
		{
			List<Color> ret = new List<Color>();

			// linear interpolation lerp (r,a,b) = (1-r)*a + r*b = (1-r)*(ax,ay,az) + r*(bx,by,bz)
			for (int n = 0; n < colorCount; n++)
			{
				double r = (double)n / (double)(colorCount - 1);
				double nr = 1.0 - r;
				double A = (nr * start.A) + (r * end.A);
				double R = (nr * start.R) + (r * end.R);
				double G = (nr * start.G) + (r * end.G);
				double B = (nr * start.B) + (r * end.B);

				ret.Add(Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B));
			}

			return ret;
		}
		
		/// <summary>
		/// Create a list of gradient colors based on the input list
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <param name="colors">the list of colors to transition between</param>
		/// <returns>a List of gradients</returns>
		public static List<Color> RgbLinearInterpolate(int steps, List<Color> colors) {
			int parts = colors.Count - 1;
			
			List<Color> gradients = new List<Color>();

			double partSteps = Math.Floor((double)steps / parts);
			double remainder = steps - (partSteps * parts);
			for (int col = 0; col < parts; col++) {
				// get colors
				Color c1 = colors[col];
				Color c2 = colors[col + 1];

				// ensure we get the right number of steps by adding remainder to final part
				if (col == parts - 1) partSteps += remainder;
				
				// make gradient for this part
				for (int step = 0; step < partSteps; step ++) {
					double p = step / partSteps;

					// interpolate r, g, b
					double r = MathUtils.Interpolate(c1.R, c2.R, p);
					double g = MathUtils.Interpolate(c1.G, c2.G, p);
					double b = MathUtils.Interpolate(c1.B, c2.B, p);
					
					// add to gradient array
					gradients.Add(Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b)));
				}
			}
			
			return gradients;
		}
		
		/// <summary>
		/// Create a list of gradient colors based on the input list
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <param name="colors">the list of colors to transition between</param>
		/// <returns>a List of gradients</returns>
		public static List<Color> HsbLinearInterpolate(int steps, List<IColor> colors) {
			int parts = colors.Count - 1;
			
			List<Color> gradients = new List<Color>();
			
			double partSteps = Math.Floor((double)steps / parts);
			double remainder = steps - (partSteps * parts);
			for (int col = 0; col < parts; col++) {
				// get colors
				IColor c1 = colors[col];
				IColor c2 = colors[col + 1];
				
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
					double b = (1 - p) * c1.Value + p * c2.Value;
					
					// add to gradient array
					gradients.Add(new HSBColor(h, s, b).Color);
				}
			}
			return gradients;
		}
		
		/// <summary>
		/// Create a list of gradient colors based on the input list
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <param name="colors">the list of colors to transition between</param>
		/// <returns>a List of gradients</returns>
		public static List<Color> HslLinearInterpolate(int steps, List<IColor> colors) {
			int parts = colors.Count - 1;
			
			List<Color> gradients = new List<Color>();
			
			double partSteps = Math.Floor((double)steps / parts);
			double remainder = steps - (partSteps * parts);
			for (int col = 0; col < parts; col++) {
				// get colors
				IColor c1 = colors[col];
				IColor c2 = colors[col + 1];
				
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
					double b = (1 - p) * c1.Value + p * c2.Value;
					
					// add to gradient array
					gradients.Add(new HSLColor(h, s, b).Color);
				}
			}
			return gradients;
		}
		
		/// <summary>
		/// Return a list of gradients based on the REW color palette
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <returns>a list of gradients</returns>
		public static List<Color> GetHSBColorGradients(int steps, ColorPaletteType type) {
			
			List<IColor> colors = new List<IColor>();

			switch (type) {
				case ColorPaletteType.REW:
					// create REW gradient
					colors.Add(HSBColor.FromRGB(Color.Red));
					colors.Add(HSBColor.FromRGB(Color.Yellow));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(2, 178, 0))); // green
					colors.Add(HSBColor.FromRGB(Color.FromArgb(0, 176, 178))); // light blue
					colors.Add(HSBColor.FromRGB(Color.FromArgb(0, 0, 177))); // blue
					colors.Add(HSBColor.FromRGB(Color.FromArgb(61, 0, 124))); // purple
					break;
				case ColorPaletteType.SOX:
					// create SOX gradient
					colors.Add(HSBColor.FromRGB(Color.FromArgb(255,255,254))); // white
					colors.Add(HSBColor.FromRGB(Color.FromArgb(255,235,60)));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(252,86,0)));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(210,0,64)));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(131,0,125)));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(25,0,98)));
					colors.Add(HSBColor.FromRGB(Color.FromArgb(0,0,0))); // black
					break;
				case ColorPaletteType.PHOTOSOUNDER:
					// create Photosounder gradient
					colors.Add(HSBColor.FromRGB(Color.FromArgb(255, 255, 254))); // white
					colors.Add(HSBColor.FromRGB(Color.FromArgb(249, 247, 78))); // 
					colors.Add(HSBColor.FromRGB(Color.FromArgb(0, 0, 100))); // blue
					break;
				case ColorPaletteType.BLACK_AND_WHITE:
				default:
					// create black and white gradient
					colors.Add(HSBColor.FromRGB(Color.White));
					colors.Add(HSBColor.FromRGB(Color.Black));
					break;
			}
			List<Color> gradients = ColorUtils.HsbLinearInterpolate(steps, colors);
			return gradients;
		}

		/// <summary>
		/// Return a list of gradients based on the REW color palette
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <returns>a list of gradients</returns>
		public static List<Color> GetHSLColorGradients(int steps, ColorPaletteType type) {
			
			List<IColor> colors = new List<IColor>();

			switch (type) {
				case ColorPaletteType.REW:
					// create REW gradient
					colors.Add(HSLColor.FromRGB(Color.Red));
					colors.Add(HSLColor.FromRGB(Color.Yellow));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(2, 178, 0))); // green
					colors.Add(HSLColor.FromRGB(Color.FromArgb(0, 176, 178))); // light blue
					colors.Add(HSLColor.FromRGB(Color.FromArgb(0, 0, 177))); // blue
					colors.Add(HSLColor.FromRGB(Color.FromArgb(61, 0, 124))); // purple
					break;
				case ColorPaletteType.SOX:
					// create SOX gradient
					colors.Add(HSLColor.FromRGB(Color.FromArgb(255,255,254))); // white
					colors.Add(HSLColor.FromRGB(Color.FromArgb(255,235,60)));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(252,86,0)));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(210,0,64)));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(131,0,125)));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(25,0,98)));
					colors.Add(HSLColor.FromRGB(Color.FromArgb(0,0,0))); // black
					break;
				case ColorPaletteType.PHOTOSOUNDER:
					// create Photosounder gradient
					colors.Add(HSLColor.FromRGB(Color.FromArgb(255, 255, 254))); // white
					colors.Add(HSLColor.FromRGB(Color.FromArgb(249, 247, 78))); // 
					colors.Add(HSLColor.FromRGB(Color.FromArgb(0, 0, 100))); // blue
					break;
				case ColorPaletteType.BLACK_AND_WHITE:
				default:
					// create black and white gradient
					colors.Add(HSLColor.FromRGB(Color.White));
					colors.Add(HSLColor.FromRGB(Color.Black));
					break;
			}
			List<Color> gradients = ColorUtils.HslLinearInterpolate(steps, colors);
			return gradients;
		}

		/// <summary>
		/// Return a list of gradients based on the REW color palette
		/// </summary>
		/// <param name="steps">number of gradients (colors) to create</param>
		/// <returns>a list of gradients</returns>
		public static List<Color> GetRGBColorGradients(int steps, ColorPaletteType type) {
			
			List<Color> colors = new List<Color>();

			switch (type) {
				case ColorPaletteType.REW:
					// create REW gradient
					colors.Add(Color.Red);
					colors.Add(Color.Yellow);
					colors.Add(Color.FromArgb(2, 178, 0)); // green
					colors.Add(Color.FromArgb(0, 176, 178)); // light blue
					colors.Add(Color.FromArgb(0, 0, 177)); // blue
					colors.Add(Color.FromArgb(61, 0, 124)); // purple
					break;
				case ColorPaletteType.SOX:
					// create SOX gradient
					colors.Add(Color.FromArgb(255,255,254)); // white
					colors.Add(Color.FromArgb(255,235,60));
					colors.Add(Color.FromArgb(252,86,0));
					colors.Add(Color.FromArgb(210,0,64));
					colors.Add(Color.FromArgb(131,0,125));
					colors.Add(Color.FromArgb(25,0,98));
					colors.Add(Color.FromArgb(0,0,0)); // black
					break;
				case ColorPaletteType.PHOTOSOUNDER:
					// create Photosounder gradient
					colors.Add(Color.FromArgb(255, 255, 255)); // white
					colors.Add(Color.FromArgb(255, 255, 112)); // skin color
					colors.Add(Color.FromArgb(63, 120, 190)); // 
					colors.Add(Color.FromArgb(16, 70, 180)); // 
					colors.Add(Color.FromArgb(14, 55, 170)); // 
					colors.Add(Color.FromArgb(12, 30, 155)); // 
					colors.Add(Color.FromArgb(12, 30, 130)); // 
					colors.Add(Color.FromArgb(10, 22, 110)); // 
					colors.Add(Color.FromArgb(10, 21, 106)); // 
					colors.Add(Color.FromArgb(10, 21, 104)); // 
					colors.Add(Color.FromArgb(10, 21, 100)); // dark blue
					break;
				case ColorPaletteType.BLACK_AND_WHITE:
				default:
					// create black and white gradient
					colors.Add(Color.White);
					colors.Add(Color.Black);
					break;
			}
			List<Color> gradients = ColorUtils.RgbLinearInterpolate(steps, colors);
			return gradients;
		}
		
		/// <summary>
		/// Save a list of gradients as a image file, using the number of gradients as the height
		/// </summary>
		/// <param name="imageToSave">image file name</param>
		/// <param name="gradients">list of gradients</param>
		/// <param name="width">image width</param>
		public static void SaveColorGradients(string imageToSave, List<Color> gradients, int width) {
			int height = gradients.Count;
			
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			Pen pen = new Pen(Color.Black, 1.0f);
			
			for (int i = 0; i < height; i++) {
				pen.Color = gradients[i];
				g.DrawLine(pen, 0, i, width, i);
			}
			png.Save(imageToSave);
		}
		
		public static Bitmap GetColorGradientsPalette(List<Color> gradients, int height = 0) {
			int width = gradients.Count;
			
			Bitmap png = new Bitmap(width, height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage(png);
			Pen pen = new Pen(Color.Black, 1.0f);
			
			for (int i = 0; i < width; i++) {
				pen.Color = gradients[i];
				g.DrawLine(pen, i, 0, i, height);
			}
			//png.Save(@"c:\colorgradient-palette.png");
			return png;
		}

		/// <summary>
		/// Colorizing the Intensity Mask
		/// Create a color map that tells the GDI rendering method how it should draw the colors.
		/// What we're actually doing here is creating a table that specifies a new color for each shade of gray
		/// (all 256) of them. The easiest way to do this is to create a palette image that's
		/// 256 pixels wide by 1 pixel high.
		/// That means that we can directly map each pixel in the palette image to a shade of gray.
		/// </summary>
		/// <param name="mask">Gray shaded image to be colorized</param>
		/// <param name="alpha">0-255</param>
		/// <returns>a colorized bitmap</returns>
		public static Bitmap Colorize(Bitmap mask, byte alpha, ColorPaletteType type)
		{
			// Create new bitmap to act as a work surface for the colorization process
			Bitmap output = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);

			// Create a graphics object from our memory bitmap so we can draw on it and clear it's drawing surface
			Graphics surface = Graphics.FromImage(output);
			surface.Clear(Color.Transparent);

			// Build an array of color mappings to remap our greyscale mask to full color
			// Accept an alpha byte to specify the transparancy of the output image
			ColorMap[] colors = CreatePaletteIndex(alpha, type);

			// Create new image attributes class to handle the color remappings
			// Inject our color map array to instruct the image attributes class how to do the colorization
			ImageAttributes remapper = new ImageAttributes();
			remapper.SetRemapTable(colors);

			// Draw our mask onto our memory bitmap work surface using the new color mapping scheme
			surface.DrawImage(mask, new Rectangle(0, 0, mask.Width, mask.Height), 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, remapper);

			// Send back newly colorized memory bitmap
			return output;
		}

		private static ColorMap[] CreatePaletteIndex(byte Alpha, ColorPaletteType type)
		{
			ColorMap[] outputMap = new ColorMap[256];

			// Change this path to wherever you saved the palette image.
			//Bitmap palette = (Bitmap)Bitmap.FromFile(@"C:\Users\Dylan\Documents\Visual Studio 2005\Projects\HeatMapTest\palette.bmp");
			
			List<Color> gradients;
			switch (type) {
				case ColorPaletteType.PHOTOSOUNDER:
					gradients = ColorUtils.GetRGBColorGradients(256, ColorUtils.ColorPaletteType.PHOTOSOUNDER);
					break;
				case ColorPaletteType.REW:
					gradients = ColorUtils.GetHSBColorGradients(256, ColorUtils.ColorPaletteType.REW);
					break;
				case ColorPaletteType.SOX:
					gradients = ColorUtils.GetHSBColorGradients(256, ColorUtils.ColorPaletteType.SOX);
					break;
				default:
					gradients = ColorUtils.GetHSBColorGradients(256, ColorUtils.ColorPaletteType.SOX);
					break;
			}
			
			Bitmap palette = GetColorGradientsPalette(gradients, 1);
			
			// Loop through each pixel and create a new color mapping
			for (int x = 0; x <= 255; x++)
			{
				outputMap[x] = new ColorMap();
				outputMap[x].OldColor = Color.FromArgb(x, x, x);
				outputMap[x].NewColor = Color.FromArgb(Alpha, palette.GetPixel(255-x, 0));
			}
			return outputMap;
		}
	}
	
	public interface IColor {

		double Hue { get; set; }
		double Saturation { get; set; }
		double Value { get; set; }
		
		Color Color { get; }
	}

	/// <summary>
	/// Convert from HSL Color space to RGB Color space and back
	/// </summary>
	public class HSLColor : IColor {
		
		/// <summary>
		/// Hue (0 - 1)
		/// </summary>
		public double Hue { get; set; }
		
		/// <summary>
		/// Saturation (0 - 1)
		/// </summary>
		public double Saturation { get; set; }
		
		/// <summary>
		/// Luminosity (0 - 1)
		/// </summary>
		public double Value { get; set; }
		
		/// <summary>
		/// Create a HSLColor object
		/// </summary>
		/// <param name="H">Hue (0 - 1)</param>
		/// <param name="S">Saturation (0 - 1)</param>
		/// <param name="L">Luminosity (0 - 1)</param>
		public HSLColor(double h, double s, double l) {
			this.Hue = h;
			this.Saturation = s;
			this.Value = l;
		}

		/// <summary>
		/// Return a Color (RGB) object
		/// </summary>
		public Color Color {
			get {
				return ColorUtils.HslToRgb((float)this.Hue*360, (float)this.Saturation, (float)this.Value);
			}
		}

		public static HSLColor FromRGB(Color color)
		{
			double h;
			double s;
			double l;
			ColorUtils.RgbToHsl(color, out h, out s, out l);
			return new HSLColor(h/360, s, l);
		}
		
		public override string ToString()
		{
			return
				Hue + ";" +
				Saturation + ";" +
				Value + ";";
		}
	}
	
	/// <summary>
	/// Convert from HSV Color space to RGB Color space and back
	/// </summary>
	public class HSBColor : IColor {
		
		/// <summary>
		/// Hue (0 - 1)
		/// </summary>
		public double Hue { get; set; }
		
		/// <summary>
		/// Saturation (0 - 1)
		/// </summary>
		public double Saturation { get; set; }
		
		/// <summary>
		/// Brightness (0 - 1)
		/// </summary>
		public double Value { get; set; }
		
		/// <summary>
		/// Create a HSBColor object
		/// </summary>
		/// <param name="H">Hue (0 - 1)</param>
		/// <param name="S">Saturation (0 - 1)</param>
		/// <param name="B">Brightness (0 - 1)</param>
		public HSBColor(double h, double s, double b) {
			this.Hue = h;
			this.Saturation = s;
			this.Value = b;
		}

		/// <summary>
		/// Return a Color (RGB) object
		/// </summary>
		public Color Color {
			get {
				return ColorUtils.HsbToRgb((float)this.Hue*360, (float)this.Saturation, (float)this.Value);
			}
		}

		public static HSBColor FromRGB(Color color)
		{
			double h;
			double s;
			double l;
			ColorUtils.RgbToHsb(color, out h, out s, out l);
			return new HSBColor(h/360, s, l);
		}
		
		public override string ToString()
		{
			return
				Hue + ";" +
				Saturation + ";" +
				Value + ";";
		}
	}
}

