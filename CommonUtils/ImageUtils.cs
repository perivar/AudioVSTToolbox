using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CommonUtils
{
	/// <summary>
	/// The ImageUtils Class holds utility methods that apply to Bitmaps and Images
	/// </summary>
	public class ImageUtils
	{
		/// <summary>
		/// Store color gradient to file
		/// </summary>
		/// <param name="directory">directory name</param>
		/// <param name="filename">filename to use (extension is ignored and replaced with png)</param>
		/// <param name="useHSL">bool whether to use HSL or HSB</param>
		public static void DrawColorGradient(string directory, string filename, bool useHSL) {
			
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

		/// <summary>
		/// Slow grayscale conversion method from
		/// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
		/// </summary>
		/// <param name="original">original bitmap to change</param>
		/// <returns>grayscaled version</returns>
		public static Bitmap MakeGrayscaleSlow(Bitmap original)
		{
			//make an empty bitmap the same size as original
			Bitmap newBitmap = new Bitmap(original.Width, original.Height);

			for (int y = 0; y < original.Height; y++)
			{
				for (int x = 0; x < original.Width; x++)
				{

					//get the pixel from the original image
					Color originalColor = original.GetPixel(x, y);

					//create the grayscale version of the pixel
					int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
					                      + (originalColor.B * .11));

					//create the color object
					Color newColor =  Color.FromArgb(grayScale, grayScale, grayScale);
					
					//set the new image's pixel to the grayscale version
					newBitmap.SetPixel(x, y, newColor);
				}
			}

			return newBitmap;
		}
		
		/// <summary>
		/// Slightly faster grayscale conversion method from
		/// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
		/// </summary>
		/// <param name="original">original bitmap to change</param>
		/// <returns>grayscaled version</returns>
		public static Bitmap MakeGrayscaleFast(Bitmap original)
		{
			unsafe
			{
				//create an empty bitmap the same size as original
				Bitmap newBitmap = new Bitmap(original.Width, original.Height);

				//lock the original bitmap in memory
				BitmapData originalData = original.LockBits(
					new Rectangle(0, 0, original.Width, original.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

				//lock the new bitmap in memory
				BitmapData newData = newBitmap.LockBits(
					new Rectangle(0, 0, original.Width, original.Height),
					ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				
				//set the number of bytes per pixel
				int pixelSize = 3;

				for (int y = 0; y < original.Height; y++)
				{
					//get the data from the original image
					byte* oRow = (byte*)originalData.Scan0 + (y * originalData.Stride);

					//get the data from the new image
					byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

					for (int x = 0; x < original.Width; x++)
					{
						//create the grayscale version
						byte grayScale =
							(byte)((oRow[x * pixelSize] * .11) + //B
							       (oRow[x * pixelSize + 1] * .59) +  //G
							       (oRow[x * pixelSize + 2] * .3)); //R

						//set the new image's pixel to the grayscale version
						nRow[x * pixelSize] = grayScale; //B
						nRow[x * pixelSize + 1] = grayScale; //G
						nRow[x * pixelSize + 2] = grayScale; //R
					}
				}

				//unlock the bitmaps
				newBitmap.UnlockBits(newData);
				original.UnlockBits(originalData);

				return newBitmap;
			}
		}
		
		/// <summary>
		/// Fastest grayscale conversion method from
		/// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
		/// </summary>
		/// <param name="original">original bitmap to change</param>
		/// <returns>grayscaled version</returns>
		public static Bitmap MakeGrayscaleFastest(Bitmap original)
		{
			//create a blank bitmap the same size as original
			Bitmap newBitmap = new Bitmap(original.Width, original.Height);
			
			//get a graphics object from the new image
			Graphics g = Graphics.FromImage(newBitmap);

			//create the grayscale ColorMatrix
			ColorMatrix colorMatrix = new ColorMatrix(
				new float[][]
				{
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				});

			//create some image attributes
			ImageAttributes attributes = new ImageAttributes();

			//set the color matrix attribute
			attributes.SetColorMatrix(colorMatrix);

			//draw the original image on the new image
			//using the grayscale color matrix
			g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
			            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

			//dispose the Graphics object
			g.Dispose();
			return newBitmap;
		}

		/// <summary>
		/// Create a grayscaled version of the passed image
		/// </summary>
		/// <param name="original">original bitmap to change</param>
		/// <returns>grayscaled version</returns>
		public static Bitmap MakeGrayscale(Bitmap original)
		{
			return MakeGrayscaleFastest(original);
		}
		
		/// <summary>
		/// Resize an image using high quality scaling
		/// </summary>
		/// <param name="originalImage">original image</param>
		/// <param name="width">new width</param>
		/// <param name="height">new height</param>
		/// <returns></returns>
		public static Bitmap Resize(Image originalImage, int width, int height) {
			Bitmap newImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			Graphics canvas = Graphics.FromImage(newImage);
			canvas.CompositingQuality = CompositingQuality.HighQuality;
			canvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
			canvas.SmoothingMode = SmoothingMode.HighQuality;
			canvas.DrawImage(originalImage, 0, 0, width, height);
			return newImage;
		}

		/// <summary>
		/// Resize an image using low quality scaling
		/// </summary>
		/// <param name="image">image</param>
		/// <param name="maxWidth">max width</param>
		/// <param name="maxHeight">max height</param>
		/// <param name="preserveRatio">bool whether to preserve ratio or force both width and height</param>
		/// <returns></returns>
		public static Image Resize(Image originalImage, int maxWidth, int maxHeight, bool preserveRatio)
		{
			double ratioX = (double)maxWidth / originalImage.Width;
			double ratioY = (double)maxHeight / originalImage.Height;
			double ratio = Math.Min(ratioX, ratioY);

			Bitmap newImage = null;
			int newWidth = 0;
			int newHeight = 0;
			if (preserveRatio) {
				newWidth = (int)(originalImage.Width * ratio);
				newHeight = (int)(originalImage.Height * ratio);
			} else {
				newWidth = maxWidth;
				newHeight = maxHeight;
			}
			newImage = new Bitmap(newWidth, newHeight);
			
			Graphics canvas = Graphics.FromImage(newImage);
			canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

			canvas.DrawImage(originalImage, 0, 0, newWidth, newHeight);
			return newImage;
		}
		
		/// <summary>
		/// Get the image's color pixels as a byte array
		/// </summary>
		/// <param name="imageIn">image</param>
		/// <returns>byte array</returns>
		public static byte[] ImageToByteArray(Image imageIn)
		{
			Bitmap bmp = new Bitmap(imageIn);
			
			// Lock the bitmap's bits.
			Rectangle rect = new Rectangle(Point.Empty, bmp.Size);
			BitmapData bmpData =
				bmp.LockBits(rect, ImageLockMode.ReadOnly,
				             bmp.PixelFormat);
			
			// Get the address of the first line.
			IntPtr ptr = bmpData.Scan0;
			
			// Declare an array to hold the bytes of the bitmap.
			int colorDepthBitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat);

			// This code is specific to a bitmap with 24 bits per pixels.
			// int bytes = bmp.Width * bmp.Height * 3 ;
			// therefore take into account the number of bits per pixel instead
			int bytes = bmp.Width * bmp.Height * (colorDepthBitsPerPixel / 8);
			byte[] rgbValues = new byte[bytes];
			
			// Copy the RGB values into the array.
			System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
			
			return rgbValues;
		}
		
		/// <summary>
		/// Take a byte array of color pixels and create a bitmap Image
		/// </summary>
		/// <param name="rgbValues">byte array of rgbValues</param>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <param name="pixelFormat">pixelformat</param>
		/// <returns>image</returns>
		public static Image ByteArrayToImage(byte[] rgbValues, int width, int height, PixelFormat pixelFormat)
		{
			//Here create the Bitmap to the know height, width and format
			Bitmap bmp = new Bitmap(width, height, pixelFormat);

			//Create a BitmapData and Lock all pixels to be written
			BitmapData bmpData = bmp.LockBits(
				new Rectangle(Point.Empty, bmp.Size),
				ImageLockMode.WriteOnly, bmp.PixelFormat);

			//Copy the data from the byte array into BitmapData.Scan0
			System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

			//Unlock the pixels
			bmp.UnlockBits(bmpData);
			
			return bmp;
		}
		
		/// <summary>
		/// Take a byte array of 8 bit greyscale pixels and create a bitmap Image
		/// </summary>
		/// <param name="grayscaleByteArray">byte array of 8bit grayscale values</param>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <returns>image</returns>
		public static Image ByteArray8BitGrayscaleToImage(byte[] grayscaleByteArray, int width, int height)
		{
			//Here create the Bitmap to the know height, width and format
			Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

			// Declare an array to hold the bytes of the bitmap.
			uint colorDepthBitsPerPixel = (uint)Image.GetPixelFormatSize(newBitmap.PixelFormat);
			uint multiplicationFactor = colorDepthBitsPerPixel / 8;
			
			for (int y = 0; y < newBitmap.Height; y++)
			{
				for (int x = 0; x < newBitmap.Width; x++)
				{
					byte gray = grayscaleByteArray[x + (y * newBitmap.Height)];
					
					int grayScale = (int) (gray * multiplicationFactor);
					
					//create the color object
					Color newColor =  Color.FromArgb(grayScale, grayScale, grayScale);
					
					//set the new image's pixel to the grayscale version
					newBitmap.SetPixel(x, y, newColor);
				}
			}
			return newBitmap;
		}

		/// <summary>
		/// Take a byte array of 8 bit greyscale pixels and create a bitmap Image
		/// </summary>
		/// <param name="grayscaleByteArray">byte array of 8bit grayscale values</param>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <returns>image</returns>
		public static Image ByteArrayGrayscaleToImage(byte[] grayscaleByteArray, int width, int height)
		{
			//Here create the Bitmap to the know height, width and format
			Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

			// Declare an array to hold the bytes of the bitmap.
			for (int x = 0; x < newBitmap.Width; x++)
			{
				for (int y = 0; y < newBitmap.Height; y++)
				{
					byte gray = grayscaleByteArray[x + (y * newBitmap.Width)];
					
					//create the color object
					Color newColor =  Color.FromArgb(gray, gray, gray);
					
					//set the new image's pixel to the grayscale version
					newBitmap.SetPixel(x, y, newColor);
				}
			}
			return newBitmap;
		}
		
		/// <summary>
		/// Reduce colors to 8-bit grayscale and calculate average color value
		/// </summary>
		/// <param name="bmp">an image</param>
		/// <param name="averageValue">calculated average color value</param>
		/// <returns>byte array</returns>
		public static byte[] ImageToByteArray8BitGrayscale(Bitmap bmp, out uint averageValue) {
			
			// Declare an array to hold the bytes of the bitmap.
			uint colorDepthBitsPerPixel = (uint)Image.GetPixelFormatSize(bmp.PixelFormat);
			uint divisionFactor = colorDepthBitsPerPixel / 8;
			
			averageValue = 0;
			byte[] grayscaleByteArray = new byte[bmp.Width * bmp.Height];
			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					//get the pixel from the original image
					Color pixelColor = bmp.GetPixel(x, y);

					//create the grayscale version of the pixel
					//get average (sum all three and divide by three)
					//uint gray = (uint)(pixelColor.R + pixelColor.G + pixelColor.B) / 3;

					// better method that takes how the eyes determine gray
					uint gray = (uint)((pixelColor.R * .3) + (pixelColor.G * .59)
					                   + (pixelColor.B * .11));
					
					// reduce from colorDepthBitsPerPixel to 8 bit
					// i.e. PixelFormat.Format32bppRgb (32 bit) to 8 bit = /4
					gray /= divisionFactor;

					// add to byte array
					grayscaleByteArray[x + (y * bmp.Height)] = (byte)gray;
					averageValue += gray;
				}
			}
			averageValue /= (uint)(bmp.Width * bmp.Height);
			return grayscaleByteArray;
		}
		
		/// <summary>
		/// Convert a double array with values between [0 - 1] to an image
		/// </summary>
		/// <param name="rawImage">double 2d array</param>
		/// <returns>an Image</returns>
		public unsafe static Image DoubleArrayToImage(double[][] rawImage)
		{
			//int width = rawImage.GetLength(1);
			//int height = rawImage.GetLength(0);
			int width = rawImage[0].Length;
			int height = rawImage.Length;

			Bitmap Image = new Bitmap(width, height);
			BitmapData bitmapData = Image.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite,
				PixelFormat.Format32bppArgb
			);
			ColorARGB* startingPosition = (ColorARGB*) bitmapData.Scan0;


			for (int i = 0; i < height; i++)
				for (int j = 0; j < width; j++)
			{
				double color = rawImage[i][j];
				byte rgb = (byte)(color * 255);

				ColorARGB* position = startingPosition + j + i * width;
				position->A = 255;
				position->R = rgb;
				position->G = rgb;
				position->B = rgb;
			}

			Image.UnlockBits(bitmapData);
			return Image;
		}
	}
	
	/// <summary>
	/// Color struct used by the DoubleArrayToImage method
	/// </summary>
	public struct ColorARGB
	{
		public byte B;
		public byte G;
		public byte R;
		public byte A;

		public ColorARGB(Color color)
		{
			A = color.A;
			R = color.R;
			G = color.G;
			B = color.B;
		}

		public ColorARGB(byte a, byte r, byte g, byte b)
		{
			A = a;
			R = r;
			G = g;
			B = b;
		}

		public Color ToColor()
		{
			return Color.FromArgb(A, R, G, B);
		}
	}
}
