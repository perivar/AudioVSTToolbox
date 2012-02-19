using System;
using System.IO;
using CommonUtils;

public static class GlobalMembersImage_io
{
	public static double[][] bmp_in(BinaryFile bmpfile, Int32 y, Int32 x)
	{
		Int32 iy = new Int32(); // various iterators
		Int32 ix = new Int32();
		Int32 ic = new Int32();
		Int32 offset = new Int32();
		double[][] image;
		UInt16 zerobytes = new UInt16();
		UInt16 val = new UInt16();
		
		#if DEBUG
		Console.Write("bmp_in...\n");
		#endif

		if (GlobalMembersUtil.fread_le_short(bmpfile) != 19778) // "BM" format tag check
		{
			Console.Error.WriteLine("This file is not in BMP format\n");
			GlobalMembersUtil.win_return();
			Environment.Exit(1);
		}

		bmpfile.Seek(8, SeekOrigin.Current);
		//fseek(bmpfile, 8, SEEK_CUR); // skipping useless tags
		
		offset = (int) GlobalMembersUtil.fread_le_word(bmpfile) - 54; // header offset
		
		bmpfile.Seek(4, SeekOrigin.Current);
		//fseek(bmpfile, 4, SEEK_CUR); // skipping useless tags
		
		x = (int) GlobalMembersUtil.fread_le_word(bmpfile);
		y = (int) GlobalMembersUtil.fread_le_word(bmpfile);
		
		bmpfile.Seek(2, SeekOrigin.Current);
		//fseek(bmpfile, 2, SEEK_CUR); // skipping useless tags

		if (GlobalMembersUtil.fread_le_short(bmpfile) != 24) // Only format supported
		{
			Console.Error.WriteLine("Wrong BMP format, BMP images must be in 24-bit colour\n");
			GlobalMembersUtil.win_return();
			Environment.Exit(1);
		}

		bmpfile.Seek(24+offset, SeekOrigin.Current);
		//fseek(bmpfile, 24+offset, SEEK_CUR); // skipping useless tags

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		//image = malloc (y * sizeof(double)); // image allocation
		image = new double[y][];
		
		//for (iy = 0; iy< y; iy++)
		//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
		// image[iy] =calloc (x, sizeof(double));
		image[iy] = new double[x];

		zerobytes = (ushort) (4 - ((x *3) & 3));
		if (zerobytes == 4)
			zerobytes = 0;

		for (iy = y-1; iy!=-1; iy--) // backwards reading
		{
			for (ix = 0; ix< x; ix++)
			{
				for (ic = 2;ic!=-1;ic--)
				{
					//fread(val, 1, 1, bmpfile);
					val = bmpfile.ReadUInt16();
					image[iy][ix] += (double) val * (1.0/(255.0 * 3.0)); // Conversion to grey by averaging the three channels
				}
			}

			bmpfile.Seek(zerobytes, SeekOrigin.Current);
			//fseek(bmpfile, zerobytes, SEEK_CUR); // skipping padding bytes
		}

		bmpfile.Close();
		return image;
	}
	
	public static void bmp_out(BinaryFile bmpfile, double[][] image, Int32 y, Int32 x)
	{
		Int32 i = new Int32(); // various iterators
		Int32 iy = new Int32();
		Int32 ix = new Int32();
		Int32 ic = new Int32();
		Int32 filesize = new Int32();
		Int32 imagesize = new Int32();
		UInt16 zerobytes = new UInt16();
		UInt16 val = new UInt16();
		UInt16 zero = 0;
		double vald;

		#if DEBUG
		Console.Write("bmp_out...\n");
		#endif

		zerobytes = (ushort) (4 - ((x *3) & 3)); // computation of zero bytes
		if (zerobytes == 4)
			zerobytes = 0;

		//********Tags********

		filesize = 56 + ((x *3)+zerobytes) * y;
		imagesize = 2 + ((x *3)+zerobytes) * y;

		GlobalMembersUtil.fwrite_le_short(19778, bmpfile);
		GlobalMembersUtil.fwrite_le_word((uint)filesize, bmpfile);
		GlobalMembersUtil.fwrite_le_word(0, bmpfile);
		GlobalMembersUtil.fwrite_le_word(54, bmpfile);
		GlobalMembersUtil.fwrite_le_word(40, bmpfile);
		GlobalMembersUtil.fwrite_le_word((uint)x, bmpfile);
		GlobalMembersUtil.fwrite_le_word((uint)y, bmpfile);
		GlobalMembersUtil.fwrite_le_short(1, bmpfile);
		GlobalMembersUtil.fwrite_le_word(24, bmpfile);
		GlobalMembersUtil.fwrite_le_short(0, bmpfile);
		GlobalMembersUtil.fwrite_le_word((uint)imagesize, bmpfile);
		GlobalMembersUtil.fwrite_le_word(2834, bmpfile);
		GlobalMembersUtil.fwrite_le_word(2834, bmpfile);
		GlobalMembersUtil.fwrite_le_word(0, bmpfile);
		GlobalMembersUtil.fwrite_le_word(0, bmpfile);
		//--------Tags--------

		for (iy = y-1; iy!=-1; iy--) // backwards writing
		{
			for (ix = 0; ix<x; ix++)
			{
				vald = image[iy][ix] * 255.0;

				if (vald > 255.0)
					vald = 255.0;

				if (vald < 0.0)
					vald = 0.0;
				
				//if (double.IsNaN(vald)) {
				//	vald = 0.0;
				//}
				
				val = (ushort) vald;

				for (ic = 2; ic!=-1; ic--)
					bmpfile.Write(val);
				//fwrite(val, 1, 1, bmpfile);
			}
			
			// write padding bytes
			for (i = 0; i<zerobytes; i++)
				bmpfile.Write(zero);
			//fwrite(zero, 1, 1, bmpfile); // write padding bytes
		}

		GlobalMembersUtil.fwrite_le_short(0, bmpfile);

		bmpfile.Close();

		#if DEBUG
		Console.Write("Image size : {0:D}x{1:D}\n", x, y);
		#endif
	}
}
