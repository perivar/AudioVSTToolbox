using System;
using System.IO;
using CommonUtils;

public static class Util
{
	public static bool quiet = false;

	#region Read User Input Methods
	public static string ReadUserInputString()
	{
		return Console.ReadLine();
	}
	
	public static float ReadUserInputFloat()
	{
		string a = Console.ReadLine();
		if (string.IsNullOrEmpty(a)) {
			return 0.0f;
		} else {
			return Convert.ToSingle(a);
		}
	}
	
	public static void ReadUserReturn()
	{
		if (!quiet) {
			Console.Write("Press Return to quit\n");
			Console.Read();
		}
	}
	#endregion
	
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
	
	#region Rounding
	public static int RoundOff(double x)
	{
		int y = 0;
		y = (int) Math.Round(x);
		
		// nearbyint: The value of x rounded to a nearby integral (as a floating-point value).
		// Rounding using to-nearest rounding:
		// nearbyint (2.3) = 2.0
		// nearbyint (3.8) = 4.0
		// nearbyint (-2.3) = -2.0
		// nearbyint (-3.8) = -4.0
		
		// nearbyint() replacement, with the exception that the result contains a non-zero fractional part
		/*
		if (x > 0) {
			y = (int) (x + 0.5);
		} else {
			y = (int) (x - 0.5);
		}
		 */
		return y;
	}
	
	public static int RoundUp(double x)
	{
		int y = 0;
		y = (int) MathUtils.RoundUp(x);

		/*
		if (FMod(x, 1.0) == 0) {
			y = (int) x;
		} else {
			y = (int) x + 1;;
		}
		 */
		return y;
	}
	#endregion
	
	public static int NextLowPrimes(int number) {
		//int[] validPrimes = { 2, 3 }; // these are used in the original arss methods
		//return MathUtils.NextLowPrimeFactorization(number, validPrimes);
		return MathUtils.NextLowPrimeFactorization(number);
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
	
	/// <summary>
	/// Return a random number between -1 and 1
	/// </summary>
	/// <returns>a random number between -1 and 1</returns>
	public static double DoubleRandom()
	{
		return RandomUtils.NextDoubleMinus1ToPlus1();
	}
	
	#region Read and Write to BinaryFile
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
	#endregion
	
	public static long GetTimeTicks()
	{
		return DateTime.Now.Ticks;
	}
}
