using System;

namespace CommonUtils
{
	/// <summary>
	/// Random Utility methods
	/// </summary>
	public static class RandomUtils
	{
		private static Random random;
		
		#region Extension Methods
		/// <summary>
		/// Extension Method to generate a random number between 2 doubles
		/// </summary>
		/// <param name="random">Random object</param>
		/// <param name="minValue">minimum value</param>
		/// <param name="maxValue">maximum value (but never including)</param>
		/// <returns>random number between 2 doubles</returns>
		/// <example>
		/// Random random = new Random();
		/// double value = random.NextDouble(1.23, 5.34);
		/// </example>
		/// <description>
		/// Random.NextDouble never returns 1.0, this function will also never equal the maximum number.
		/// </description>
		public static double NextDouble(
			this Random random,
			double minValue,
			double maxValue)
		{
			return random.NextDouble() * (maxValue - minValue) + minValue;
		}
		#endregion
		
		#region Double
		/// <summary>
		/// Return a random number between -1 and 1
		/// </summary>
		/// <returns>a random number between -1 and 1</returns>
		internal static double NextDoubleMinus1ToPlus1()
		{
			if (random == null)
				Seed();

			double value = random.NextDouble(); 	// between 0..1
			value *= 2;            					// between 0..2
			value--;               					// between -1..1

			return value;
		}

		/// <summary>
		/// Return a random number between 0 and 1
		/// </summary>
		/// <returns>a random number between 0 and 1</returns>
		internal static double NextDouble()
		{
			if (random == null)
				Seed();

			// Random.NextDouble returns a double between 0 and 1.
			double value = random.NextDouble(); 	// between 0..1
			return value;
		}
		#endregion

		#region Int
		/// <summary>
		/// Returns a non-negative random integer.
		/// </summary>
		/// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than MaxValue.</returns>
		internal static int NextInt()
		{
			if (random == null)
				Seed();

			return random.Next();
		}

		/// <summary>
		/// Return a number between 0 and the ceiling value
		/// (but never actually including the ceiling value)
		/// </summary>
		/// <param name="ceiling">max value (but never including)</param>
		/// <returns>a random value</returns>
		/// <example>
		/// NextInt(2) will return either 0 or 1's
		/// </example>
		internal static int NextInt(int ceiling)
		{
			if (random == null)
				Seed();

			return random.Next(ceiling);
		}
		
		/// <summary>
		/// Returns a random integer that is within a specified range.
		/// (but never actually including the ceiling value)
		/// </summary>
		/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
		/// <param name="ceiling">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
		/// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
		/// <example>
		/// NextInt(1,11) will return a number between 1 and 10
		/// </example>
		internal static int NextInt(int minValue, int ceiling)
		{
			if (random == null)
				Seed();

			return random.Next(minValue, ceiling);
		}
		#endregion
		
		#region Seeding
		/// <summary>
		/// Initializes a new instance of the Random class, using a time-dependent default seed value.
		/// </summary>
		internal static void Seed()
		{
			// new Random() already uses the current time. It is equivalent to new Random(Environment.TickCount)
			// but this might change in later version of .NET
			random = new Random();
		}

		/// <summary>
		/// Initializes a new instance of the Random class, using the specified seed value.
		/// </summary>
		/// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence. If a negative number is specified, the absolute value of the number is used.</param>
		/// <example>
		/// RandomUtils.Seed(Guid.NewGuid().GetHashCode()); // supposedly the best option
		/// RandomUtils.Seed(DateTime.Now.Millisecond);
		/// </example>
		internal static void Seed(int seed)
		{
			random = new Random(seed);
		}
		#endregion
	}
}