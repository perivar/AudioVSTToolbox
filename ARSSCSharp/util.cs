using System;
using CommonUtils;

public static class GlobalMembersUtil
{
	public static Int32 quiet = new Int32();

	public static void win_return()
	{
		if (quiet == 0)
		{
			Console.Write("Press Return to quit\n");
			Console.Read();
		}
	}
	
	public static double fmod(double x, double y)
	{
		//double var = MathUtils.RoundToNearest(x, 0.0001);
		//double remainder = x % y;
		//double remainder = Math.IEEERemainder(var, y);
		//double remainder = x - var;
		//return remainder;

		double remainder = Math.IEEERemainder(x, y);
		if (!Double.IsNaN(remainder) && remainder != 0.0)
		{
			if (x >= 0.0)
			{
				if (remainder < 0.0)
					remainder += Math.Abs(y);
			}
			else // x < 0.0
			{
				if (remainder > 0.0)
					remainder -= Math.Abs(y);
			}
		}
		return remainder;
	}
	
	public static double roundoff(double x)
	{
		if (x > 0)
			return x + 0.5;
		else
			return x - 0.5;
	}
	
	public static Int32 roundup(double x)
	{
		if (GlobalMembersUtil.fmod(x, 1.0) == 0) {
			return (Int32) x;
		} else {
			return (Int32) x + 1;
		}
	}
	
	public static float getfloat()
	{
		float x;
		string a = new string(new char[32]);
		a = Console.ReadLine();
		if (a == null || a == "") {
			return 0.0f;
		} else {
			x = Convert.ToSingle(a);
			return x;
		}
	}
	
	public static Int32 smallprimes(Int32 x)
	{
		Int32 i = new Int32();
		Int32[] p = {2, 3};

		for (i = 0; i<2; i++) {
			while (x%p[i] == 0) {
				x/=p[i];
			}
		}
		return x;
	}
	
	public static Int32 nextsprime(Int32 x)
	{
		while (GlobalMembersUtil.smallprimes(x)!=1) {
			x++;
		}

		return x;
	}
	
	public static double log_b(double x)
	{
		if (GlobalMembersDsp.LOGBASE == 1.0) {
			return x;
		} else {
			if (x == 0) {
				#if DEBUG
				Console.Error.WriteLine("Warning : log(0) returning -infinite\n");
				#endif
				return 0;
			} else {
				return Math.Log(x)/Math.Log(GlobalMembersDsp.LOGBASE);
			}
		}
	}
	
	public static UInt32 rand_u32()
	{
		//C++ TO C# CONVERTER TODO TASK: C# does not allow setting or comparing #define constants:
		//#if RAND_MAX == 2147483647
		return (uint) RandomNumbers.NextNumber();
		
		/*
		//C++ TO C# CONVERTER TODO TASK: C# does not allow setting or comparing #define constants:
		#elif RAND_MAX == 32767
		return ((RandomNumbers.NextNumber()%256)<<24) | ((RandomNumbers.NextNumber()%256)<<16) | ((RandomNumbers.NextNumber()%256)<<8) | (RandomNumbers.NextNumber()%256);
		#else
		Console.Error.WriteLine("Unhandled RAND_MAX value : %d\nPlease signal this error to the developer.", RAND_MAX);
		return RandomNumbers.NextNumber();
		#endif
		 */
	}
	
	public static double dblrand()
	{
		return ((double) GlobalMembersUtil.rand_u32() * (1.0 / 2147483648.0)) - 1.0;
	}
	
	// read from file a 16-bit integer in little endian
	public static UInt16 fread_le_short(BinaryFile file)
	{
		return file.ReadUInt16();
	}
	
	// write to file a 16-bit integer in little endian
	public static void fwrite_le_short(UInt16 s, BinaryFile file)
	{
		file.Write(s);
	}

	// write to file a 16-bit integer in little endian
	public static void fwrite_le_short(Int16 s, BinaryFile file)
	{
		file.Write(s);
	}
	
	// read from file a 32-bit integer in little endian
	public static UInt32 fread_le_word(BinaryFile file)
	{
		return file.ReadUInt32();
	}
	
	// write to file a 32-bit integer in little endian
	public static void fwrite_le_word(UInt32 w, BinaryFile file)
	{
		file.Write(w);
	}
	
	public static string getstring()
	{
		return Console.ReadLine();
		
		/*
		int len_str;
		int i;
		string a = new string(new char[FILENAME_MAX]);
		string b;

		fgets(a, sizeof(sbyte), stdin);
		len_str = a.Length;

		//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		b = malloc(len_str * sizeof(sbyte));

		for (i = 0; i<len_str; i++)
			b[i]=a[i];

		b = b.Substring(0, len_str-1);

		return b;
		 */
	}
	
	public static Int32 gettime() // in milliseconds
	{
		return DateTime.Now.Millisecond;
		
		/*
		timeval t = new timeval();

		gettimeofday(t, null);

		return (Int32) t.tv_sec *1000 + t.tv_usec/1000;
		 */
	}
}
