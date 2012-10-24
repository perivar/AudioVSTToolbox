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

		// Slow grayscale conversion method from
		// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
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
		
		// Slightly faster grayscale conversion method from
		// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
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
		
		// Fastest grayscale conversion method from
		// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
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
		
		public static Bitmap Resize(Image originalImage, int width, int height) {
			Bitmap newImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
			Graphics canvas = Graphics.FromImage(newImage);
			canvas.CompositingQuality = CompositingQuality.HighQuality;
			canvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
			canvas.SmoothingMode = SmoothingMode.HighQuality;
			canvas.DrawImage(originalImage, 0, 0, width, height);
			return newImage;
		}
		
		// Get the image's color pixels as a byte array
		public static byte[] ImageToByteArray(Image imageIn)
		{
			Bitmap bmp = new Bitmap(imageIn);
			
			// Lock the bitmap's bits.
			Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			BitmapData bmpData =
				bmp.LockBits(rect, ImageLockMode.ReadWrite,
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
		
		// Take a byte array of color pixels and create a bitmap Image
		public static Image ByteArrayToImage(byte[] rgbValues, int width, int height, PixelFormat pixelFormat)
		{
			//Here create the Bitmap to the know height, width and format
			Bitmap bmp = new Bitmap(width, height, pixelFormat);

			//Create a BitmapData and Lock all pixels to be written
			BitmapData bmpData = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				ImageLockMode.ReadWrite, bmp.PixelFormat);

			//Copy the data from the byte array into BitmapData.Scan0
			System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);

			//Unlock the pixels
			bmp.UnlockBits(bmpData);
			
			return bmp;
		}
		
		// Take a byte array of 8 bit greyscale pixels and create a bitmap Image
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

		// Take a byte array of 8 bit greyscale pixels and create a bitmap Image
		public static Image ByteArrayGrayscaleToImage(byte[] grayscaleByteArray, int width, int height)
		{
			//Here create the Bitmap to the know height, width and format
			Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

			// Declare an array to hold the bytes of the bitmap.
			for (int y = 0; y < newBitmap.Height; y++)
			{
				for (int x = 0; x < newBitmap.Width; x++)
				{
					byte gray = grayscaleByteArray[x + (y * newBitmap.Height)];
					
					//create the color object
					Color newColor =  Color.FromArgb(gray, gray, gray);
					
					//set the new image's pixel to the grayscale version
					newBitmap.SetPixel(x, y, newColor);
				}
			}
			return newBitmap;
		}
		
		// Reduce colors to 8-bit grayscale and calculate average color value
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
	}
}
