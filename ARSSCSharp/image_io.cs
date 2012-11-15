using System;
using System.IO;
using CommonUtils;

public static class GlobalMembersImageIO
{
	public static double[][] BMPRead(BinaryFile bmpfile, ref Int32 y, ref Int32 x)
	{
		Int32 iy = new Int32(); // various iterators
		Int32 ix = new Int32();
		Int32 ic = new Int32();
		Int32 offset = new Int32();
		double[][] image;
		byte zerobytes = new byte();
		byte val = new byte();
		
		#if DEBUG
		Console.Write("BMPRead...\n");
		#endif

		if (GlobalMembersUtil.ReadUInt16(bmpfile) != 19778) // "BM" format tag check
		{
			Console.Error.WriteLine("This file is not in BMP format\n");
			GlobalMembersUtil.WinReturn();
			Environment.Exit(1);
		}

		bmpfile.Seek(8, SeekOrigin.Current); // skipping useless tags
		
		offset = (int) GlobalMembersUtil.ReadUInt32(bmpfile) - 54; // header offset
		
		bmpfile.Seek(4, SeekOrigin.Current); // skipping useless tags
		
		x = (int) GlobalMembersUtil.ReadUInt32(bmpfile);
		y = (int) GlobalMembersUtil.ReadUInt32(bmpfile);
		
		bmpfile.Seek(2, SeekOrigin.Current); // skipping useless tags

		if (GlobalMembersUtil.ReadUInt16(bmpfile) != 24) // Only format supported
		{
			Console.Error.WriteLine("Wrong BMP format, BMP images must be in 24-bit colour\n");
			GlobalMembersUtil.WinReturn();
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

		for (iy = y-1; iy!=-1; iy--) // backwards reading
		{
			for (ix = 0; ix< x; ix++)
			{
				for (ic = 2;ic!=-1;ic--)
				{
					val = bmpfile.ReadByte();
					image[iy][ix] += (double) val * (1.0/(255.0 * 3.0)); // Conversion to grey by averaging the three channels
				}
			}

			bmpfile.Seek(zerobytes, SeekOrigin.Current); // skipping padding bytes
		}

		bmpfile.Close();
		return image;
	}
	
	public static void BMPWrite(BinaryFile bmpfile, double[][] image, Int32 y, Int32 x)
	{
		Int32 i = new Int32(); // various iterators
		Int32 iy = new Int32();
		Int32 ix = new Int32();
		Int32 ic = new Int32();
		Int32 filesize = new Int32();
		Int32 imagesize = new Int32();
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

		GlobalMembersUtil.WriteUInt16(19778, bmpfile);
		GlobalMembersUtil.WriteUInt32((UInt32)filesize, bmpfile);
		GlobalMembersUtil.WriteUInt32(0, bmpfile);
		GlobalMembersUtil.WriteUInt32(54, bmpfile);
		GlobalMembersUtil.WriteUInt32(40, bmpfile);
		GlobalMembersUtil.WriteUInt32((UInt32)x, bmpfile);
		GlobalMembersUtil.WriteUInt32((UInt32)y, bmpfile);
		GlobalMembersUtil.WriteUInt16(1, bmpfile);
		GlobalMembersUtil.WriteUInt32(24, bmpfile);
		GlobalMembersUtil.WriteUInt16(0, bmpfile);
		GlobalMembersUtil.WriteUInt32((UInt32)imagesize, bmpfile);
		GlobalMembersUtil.WriteUInt32(2834, bmpfile);
		GlobalMembersUtil.WriteUInt32(2834, bmpfile);
		GlobalMembersUtil.WriteUInt32(0, bmpfile);
		GlobalMembersUtil.WriteUInt32(0, bmpfile);
		//--------Tags--------

		for (iy = y-1; iy!=-1; iy--) // backwards writing
		{
			for (ix = 0; ix<x; ix++)
			{
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

		GlobalMembersUtil.WriteUInt16(0, bmpfile);

		bmpfile.Close();

		#if DEBUG
		Console.Write("Image size : {0:D}x{1:D}\n", x, y);
		#endif
	}
}
