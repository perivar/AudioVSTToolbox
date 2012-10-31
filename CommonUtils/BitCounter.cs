using System;

namespace CommonUtils
{
	/// <summary>
	/// BitCounter
	/// 
	/// Credit:
	/// http://code.google.com/p/algs/
	/// http://marvin.cs.uidaho.edu/Teaching/CS472/bitHelpers.cpp
	/// http://folk.uio.no/davidjo/computing.php
	/// 
	/// And also:
	/// http://www.dotnetperls.com/bitcount
	/// http://marvin.cs.uidaho.edu/Teaching/CS472/bitHelpers.cpp
	/// http://bits.stephan-brumme.com/countBits.html
	/// http://stackoverflow.com/questions/3815165/how-to-implement-bitcount-using-only-bitwise-operators
	/// </summary>
	public class BitCounter
	{
		#region BitCount by code.google.com/p/algs/
		
		public BitCounter(int bits) {
			Precompute(bits);
		}
		
		private int precomputedBits;
		private int precomputedSize;
		private int[] precomputed;
		
		public static int CountOnes(int k)
		{
			int count = 0;
			for (; k != 0; k >>= 1)
			{
				count += k & 1;
			}
			return count;
		}

		public static int CountSparseOnes(int k)
		{
			int count = 0;
			while (k != 0)
			{
				count++;
				k &= (k - 1);
			}
			return count;
		}

		public static int CountDenseOnes(int k)
		{
			int count = 32;
			k = ~k;
			while (k != 0)
			{
				count--;
				k &= (k - 1);
			}
			return count;
		}

		public void Precompute(int bits)
		{
			precomputedBits = bits;
			precomputedSize = 1 << bits;
			precomputed = new int[precomputedSize];
			for (int i = 0; i < precomputedSize; i++)
			{
				precomputed[i] = CountOnes(i);
			}
		}

		public int CountOnesWithPrecomputation(ulong k)
		{
			int count = 0;
			for (; k != 0; k >>= precomputedBits)
			{
				count += precomputed[k & (ulong)(precomputedSize - 1)];
			}
			return count;
		}
		#endregion
		
		#region BitCount by marvin.cs.uidaho.edu/Teaching/CS472/bitHelpers.cpp
		
		// basic fast 32 bit helper functions
		public static int BitCount32(uint w)
		{
			w = (0x55555555 & w) + (0x55555555 & (w>> 1));
			w = (0x33333333 & w) + (0x33333333 & (w>> 2));
			w += (w>> 4);
			w &= 0x0f0f0f0f;
			w += (w>> 8);
			return (int)((w + (w>>16)) & 0x1f);
		}
		
		// the Hamming distance between two strings
		public static int Hamming(uint a, uint b)
		{
			return BitCount32(a ^ b);
		}
		
		// basic fast 64 bit helper functions
		public static int BitCount64(ulong w)
		{
			w = (0x5555555555555555UL & w) + (0x5555555555555555UL & (w>> 1));
			w = (0x3333333333333333UL & w) + (0x3333333333333333UL & (w>> 2));
			w += (w>> 4);
			w &= 0x0f0f0f0f0f0f0f0fUL;
			w += (w>> 8);
			w += (w>>16);
			w &= 0x000000ff000000ffUL;
			return (int)(w + (w>>32));
		}

		// the Hamming distance between two strings
		public static int Hamming(ulong a, ulong b)
		{
			return BitCount64(a ^ b);
		}
		#endregion
		
		#region BitCount by David Oftedal
		
		// Count the number of 1-bits in a number.
		// We use a precomputed table to hopefully speed it up.
		// Made in Python as follows:
		// a = list()
		// a.append(0)
		// while len(a) <= 128:
		//  a.extend([b+1 for b in a])
		/// <summary>
		/// Bitcounts array used for BitCount method (used in Similarity comparisons).
		/// Credit goes to David Oftedal of the University of Oslo, Norway for this.
		/// http://folk.uio.no/davidjo/computing.php
		/// </summary>
		private static byte[] bitCounts = {
			0,1,1,2,1,2,2,3,1,2,2,3,2,3,3,4,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,1,2,2,3,2,3,3,4,
			2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,
			2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,
			4,5,5,6,5,6,6,7,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,5,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,
			2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,2,3,3,4,3,4,4,5,
			3,4,4,5,4,5,5,6,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,3,4,4,5,4,5,5,6,4,5,5,6,5,6,6,7,
			4,5,5,6,5,6,6,7,5,6,6,7,6,7,7,8
		};

		/// <summary>
		/// Utility function for similarity.
		/// </summary>
		/// <param name="num">The hash we are counting.</param>
		/// <returns>The total bit count.</returns>
		public static int BitCount(ulong num)
		{
			int count = 0;
			for (; num > 0; num >>= 8)
				count += bitCounts[(num & 0xff)];
			return count;
		}
		#endregion
	}
}
