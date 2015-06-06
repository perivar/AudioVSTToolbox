using System;
using CommonUtils;

public static class Util
{
	public static bool quiet = false;

	public static void ReadUserReturn()
	{
		if (!quiet)
		{
			Console.Write("Press Return to quit\n");
			Console.Read();
		}
	}
	
	/// <summary>
	/// Compute remainder of division
	/// Returns the floating-point remainder of numerator/denominator.
	/// The remainder of a division operation is the result of subtracting the integral quotient multiplied by the denominator from the numerator:
	/// remainder = numerator - quotient * denominator
	/// </summary>
	/// <param name="numerator">double</param>
	/// <param name="denominator">double</param>
	/// <returns>remainder of division</returns>
	public static double FMod(double numerator, double denominator)
	{
		double remainder = Math.IEEERemainder(numerator, denominator);
		if (!Double.IsNaN(remainder) && remainder != 0.0)
		{
			if (numerator >= 0.0)
			{
				if (remainder < 0.0)
					remainder += Math.Abs(denominator);
			}
			else // x < 0.0
			{
				if (remainder > 0.0)
					remainder -= Math.Abs(denominator);
			}
		}
		return remainder;
	}
	
	// nearbyint() replacement, with the exception that the result contains a non-zero fractional part
	public static double RoundOff(double x)
	{
		if (x > 0)
			return x + 0.5;
		else
			return x - 0.5;
	}
	
	public static int RoundUp(double x)
	{
		if (FMod(x, 1.0) == 0) {
			return (int) x;
		} else {
			return (int) x + 1;
		}
	}
	
	public static float ReadUserInputFloat()
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
	
	// returns 1 if x is only made of these small primes
	public static int SmallPrimes(int x)
	{
		int[] p = {2, 3};

		for (int i = 0; i<2; i++) {
			while (x%p[i] == 0) {
				x/=p[i];
			}
		}
		return x;
	}
	
	public static int NextPrime(int x)
	{
		while (SmallPrimes(x) != 1) {
			x++;
		}

		return x;
	}
	
	public static double Log(double x)
	{
		if (DSP.LOGBASE == 1.0) {
			return x;
		} else {
			if (x == 0) {
				#if DEBUG
				Console.Error.WriteLine("Warning : log(0) returning -infinite\n");
				#endif
				return 0;
			} else {
				return Math.Log(x) / Math.Log(DSP.LOGBASE);
			}
		}
	}
	
	public static uint Random()
	{
		return (uint) RandomNumbers.NextNumber();
	}
	
	// range is +/- 1.0
	// TODO: this looks wrong?!
	public static double DoubleRandom()
	{
		return ((double) Util.Random() * (1.0 / 2147483648.0)) - 1.0;
	}
	
	// read from file a 16-bit integer in little endian
	public static ushort ReadUInt16(BinaryFile file)
	{
		return file.ReadUInt16();
	}
	
	// write to file a 16-bit integer in little endian
	public static void WriteUInt16(ushort s, BinaryFile file)
	{
		file.Write(s);
	}

	// write to file a 16-bit integer in little endian
	public static void WriteInt16(Int16 s, BinaryFile file)
	{
		file.Write(s);
	}
	
	// read from file a 32-bit integer in little endian
	public static uint ReadUInt32(BinaryFile file)
	{
		return file.ReadUInt32();
	}
	
	// write to file a 32-bit integer in little endian
	public static void WriteUInt32(uint w, BinaryFile file)
	{
		file.Write(w);
	}
	
	public static string ReadUserInputString()
	{
		return Console.ReadLine();
	}
	
	public static long GetTime() // in milliseconds
	{
		return DateTime.Now.Ticks;
	}
}
