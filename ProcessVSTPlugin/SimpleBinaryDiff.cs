/*
 * Binary Diff
 * http://stackoverflow.com/questions/1893873/is-there-a-way-to-produce-a-binary-diff-on-two-byte-arrays-in-c-sharp-net
 * Dec 12 '09 at 16:46 Aaronaught
 */
using System;
using System.Collections.Generic;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of SimpleBinaryDiff.
	/// </summary>
	public class SimpleBinaryDiff
	{
		public SimpleBinaryDiff()
		{
		}

		public class Diff
		{
			public Diff()
			{
				Points = new List<DiffPoint>();
			}

			public List<DiffPoint> Points { get; private set; }

			public int PointsCount {
				get {
					return Points.Count;
				}
			}
			
			public override string ToString()
			{
				//loop through the List to get all the items
				string concat = "";
				foreach (DiffPoint point in Points) {
					concat += string.Format("[{0}]:{1}=>{2} ", point.Index, point.OldValue, point.NewValue);
				}
				return concat;
			}
		}
		
		public struct DiffPoint
		{
			public int Index;
			public byte OldValue;
			public byte NewValue;

			public DiffPoint(int index, byte oldValue, byte newValue) : this()
			{
				this.Index = index;
				this.OldValue = oldValue;
				this.NewValue = newValue;
			}
		}
		
		public static Diff GetDiff(byte[] a, byte[] b)
		{
			Diff diff = new Diff();
			if (a.Length != b.Length) {
				// cannot use this binary diff method
				// since the lengths have changes
				// assume this means we are dealing with text instead							
				return null;
			}
			
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					diff.Points.Add(new DiffPoint(i, a[i], b[i]));
				}
			}
			return diff;
		}

		// Mutator method - turns "b" into the original "a"
		public void ApplyDiff(byte[] b, Diff diff)
		{
			foreach (DiffPoint point in diff.Points)
			{
				b[point.Index] = point.OldValue;
			}
		}

		// Copy method - recreates "a" leaving "b" intact
		public byte[] ApplyDiffCopy(byte[] b, Diff diff)
		{
			byte[] a = new byte[b.Length];
			int startIndex = 0;
			foreach (DiffPoint point in diff.Points)
			{
				for (int i = startIndex; i < point.Index; i++)
				{
					a[i] = b[i];
				}
				a[point.Index] = point.OldValue;
				startIndex = point.Index + 1;
			}
			for (int j = startIndex; j < b.Length; j++)
			{
				a[j] = b[j];
			}
			return a;
		}
	}
}
