using System;
using System.IO;
using CommonUtils;

public static class ImageIO
{
	public static double[][] BMPRead(BinaryFile bmpfile, ref int y, ref int x)
	{
		int iy = 0; // various iterators
		int ix = 0;
		int ic = 0;
		int offset = 0;
		double[][] image;
		byte zerobytes = new byte();
		byte val = new byte();
		
		#if DEBUG
		Console.Write("BMPRead...\n");
		#endif

		if (Util.ReadUInt16(bmpfile) != 19778) // "BM" format tag check
		{
			Console.Error.WriteLine("This file is not in BMP format\n");
			Util.ReadUserReturn();
			Environment.Exit(1);
		}

		bmpfile.Seek(8, SeekOrigin.Current); // skipping useless tags
		
		offset = (int) Util.ReadUInt32(bmpfile) - 54; // header offset
		
		bmpfile.Seek(4, SeekOrigin.Current); // skipping useless tags
		
		x = (int) Util.ReadUInt32(bmpfile);
		y = (int) Util.ReadUInt32(bmpfile);
		
		bmpfile.Seek(2, SeekOrigin.Current); // skipping useless tags

		if (Util.ReadUInt16(bmpfile) != 24) // Only format supported
		{
			Console.Error.WriteLine("Wrong BMP format, BMP images must be in 24-bit colour\n");
			Util.ReadUserReturn();
			Environment.Exit(1);
		}

		bmpfile.Seek(24+offset, SeekOrigin.Current); // skipping useless tags

		// image allocation
		image = new double[y][];
		
		for (iy = 0; iy< y; iy++) {
			image[iy] = new double[x];
		}

		zerobytes = (byte)(4 - ((x *3) & 3));
		if (zerobytes == 4) {
			zerobytes = 0;
		}

		for (iy = y-1; iy!=-1; iy--) { // backwards reading
			for (ix = 0; ix< x; ix++) {
				for (ic = 2;ic!=-1;ic--) {
					val = bmpfile.ReadByte();
					image[iy][ix] += (double) val * (1.0/(255.0 * 3.0)); // Conversion to grey by averaging the three channels
				}
			}

			bmpfile.Seek(zerobytes, SeekOrigin.Current); // skipping padding bytes
		}

		bmpfile.Close();
		return image;
	}
	
	public static void BMPWrite(BinaryFile bmpfile, double[][] image, int y, int x)
	{
		int i = 0; // various iterators
		int iy = 0;
		int ix = 0;
		int ic = 0;
		int filesize = 0;
		int imagesize = 0;
		byte zerobytes = new byte();
		byte val = new byte();
		byte zero = 0;
		double vald;

		#if DEBUG
		Console.Write("BMPWrite...\n");
		#endif

		zerobytes = (byte) (4 - ((x *3) & 3)); // computation of zero bytes
		if (zerobytes == 4) {
			zerobytes = 0;
		}

		//********Tags********
		filesize = 56 + ((x *3)+zerobytes) * y;
		imagesize = 2 + ((x *3)+zerobytes) * y;

		Util.WriteUInt16(19778, bmpfile);
		Util.WriteUInt32((uint)filesize, bmpfile);
		Util.WriteUInt32(0, bmpfile);
		Util.WriteUInt32(54, bmpfile);
		Util.WriteUInt32(40, bmpfile);
		Util.WriteUInt32((uint)x, bmpfile);
		Util.WriteUInt32((uint)y, bmpfile);
		Util.WriteUInt16(1, bmpfile);
		Util.WriteUInt32(24, bmpfile);
		Util.WriteUInt16(0, bmpfile);
		Util.WriteUInt32((uint)imagesize, bmpfile);
		Util.WriteUInt32(2834, bmpfile);
		Util.WriteUInt32(2834, bmpfile);
		Util.WriteUInt32(0, bmpfile);
		Util.WriteUInt32(0, bmpfile);
		//--------Tags--------

		for (iy = y-1; iy!=-1; iy--) { // backwards writing
			for (ix = 0; ix<x; ix++) {
				
				// define color
				vald = image[iy][ix] * 255.0;

				if (vald > 255.0) {
					vald = 255.0;
				}

				if (vald < 0.0) {
					vald = 0.0;
				}
				
				val = (byte) vald;

				for (ic = 2; ic!=-1; ic--) {
					bmpfile.Write(val);
				}
			}
			
			// write padding bytes
			for (i = 0; i<zerobytes; i++) {
				bmpfile.Write(zero);
			}
		}

		Util.WriteUInt16(0, bmpfile);

		bmpfile.Close();

		#if DEBUG
		Console.Write("Image size : {0:D}x{1:D}\n", x, y);
		#endif
	}
}
