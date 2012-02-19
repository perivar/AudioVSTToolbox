//----------------------------------------------------------------------------------------
//	Copyright © 2006 - 2011 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the ability to simulate the behavior of the C/C++ functions for 
//	generating random numbers, using the .NET Framework System.Random class.
//	'rand' converts to the parameterless overload of NextNumber
//	'random' converts to the single-parameter overload of NextNumber
//	'randomize' converts to the parameterless overload of Seed
//	'srand' converts to the single-parameter overload of Seed
//----------------------------------------------------------------------------------------
internal static class RandomNumbers
{
	private static System.Random r;

	internal static int NextNumber()
	{
		if (r == null)
			Seed();

		return r.Next();
	}

	internal static int NextNumber(int ceiling)
	{
		if (r == null)
			Seed();

		return r.Next(ceiling);
	}

	internal static void Seed()
	{
		r = new System.Random();
	}

	internal static void Seed(int seed)
	{
		r = new System.Random(seed);
	}
}