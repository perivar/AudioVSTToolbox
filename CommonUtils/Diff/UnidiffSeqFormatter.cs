using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DiffPlex;
using DiffPlex.Model;

namespace CommonUtils.Diff
{
	public enum UnidiffType : int {
		NoChange,
		Insert,
		Delete
	}
	
	public class UnidiffEntry {
		public int Index { get; set; }
		public string Text { get; set; }
		public UnidiffType Type { get; set; }
		
		public UnidiffEntry(int index, string text, UnidiffType type) {
			this.Index = index;
			this.Text = text;
			this.Type = type;
		}
	}
	
	
	/// <summary>
	/// Formatter for DiffPlex diff engine
	/// Created by jorgenlindell
	/// http://diffplex.codeplex.com/discussions/254392
	/// </summary>
	public static class UnidiffSeqFormater
	{
		private const string NoChangeSymbol = "=";
		private const string InsertSymbol = "+";
		private const string DeleteSymbol = "-";

		public static List<string> Generate (DiffResult diffresult)
		{
			var uniLines = new List<string>();
			int bPos = 0;

			foreach (DiffBlock diffBlock in diffresult.DiffBlocks)
			{
				for (; bPos < diffBlock.InsertStartB; bPos++)
				{
					uniLines.Add(NoChangeSymbol + diffresult.PiecesNew[bPos]);
				}

				int i = 0;
				for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
				{
					uniLines.Add(DeleteSymbol + diffresult.PiecesOld[i + diffBlock.DeleteStartA]);
					uniLines.Add(InsertSymbol + diffresult.PiecesNew[i + diffBlock.InsertStartB]);
					bPos++;
				}

				if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
				{
					for (; i < diffBlock.DeleteCountA; i++)
					{
						uniLines.Add(DeleteSymbol + diffresult.PiecesOld[i + diffBlock.DeleteStartA]);
					}
				}
				else
				{
					for (; i < diffBlock.InsertCountB; i++)
					{
						uniLines.Add(InsertSymbol + diffresult.PiecesNew[i + diffBlock.InsertStartB]);
						bPos++;
					}
				}
			}
			//***** added bugfix (thanks Wawel for pointing this out)
			for (; bPos < diffresult.PiecesNew.Length; bPos++)
			{
				// bugfix: uniLines.Add(diffresult.PiecesNew[bPos]);
				uniLines.Add(NoChangeSymbol + diffresult.PiecesNew[bPos]);
			}
			//**** end bugfix

			return uniLines;
		}

		public static List<UnidiffEntry> GenerateWithLineNumbers (DiffResult diffresult)
		{
			var uniLines = new List<UnidiffEntry>();
			int bPos = 0;

			foreach (DiffBlock diffBlock in diffresult.DiffBlocks)
			{
				for (; bPos < diffBlock.InsertStartB; bPos++)
				{
					uniLines.Add(new UnidiffEntry(bPos, NoChangeSymbol + diffresult.PiecesNew[bPos], UnidiffType.NoChange));
				}

				int i = 0;
				for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
				{
					uniLines.Add(new UnidiffEntry(bPos, DeleteSymbol + diffresult.PiecesOld[i + diffBlock.DeleteStartA], UnidiffType.Delete));
					uniLines.Add(new UnidiffEntry(bPos, InsertSymbol + diffresult.PiecesNew[i + diffBlock.InsertStartB], UnidiffType.Insert));
					bPos++;
				}

				if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
				{
					for (; i < diffBlock.DeleteCountA; i++)
					{
						uniLines.Add(new UnidiffEntry(bPos, DeleteSymbol + diffresult.PiecesOld[i + diffBlock.DeleteStartA], UnidiffType.Delete));
					}
				}
				else
				{
					for (; i < diffBlock.InsertCountB; i++)
					{
						uniLines.Add(new UnidiffEntry(bPos, InsertSymbol + diffresult.PiecesNew[i + diffBlock.InsertStartB], UnidiffType.Insert));
						bPos++;
					}
				}
			}
			//***** added bugfix (thanks Wawel for pointing this out)
			for (; bPos < diffresult.PiecesNew.Length; bPos++)
			{
				// bugfix: uniLines.Add(diffresult.PiecesNew[bPos]);
				uniLines.Add(new UnidiffEntry(bPos, NoChangeSymbol + diffresult.PiecesNew[bPos], UnidiffType.NoChange));
			}
			//**** end bugfix

			return uniLines;
		}
		
		public static string GenerateSeq (DiffResult diffresult)
		{
			return GenerateSeq(diffresult,
			                   "[+[",
			                   "]+]",
			                   "[-[",
			                   "]-]");
		}

		public static string GenerateSeq (DiffResult diffresult, string insertSymbolS, string insertSymbolE, string deleteSymbolS, string deleteSymbolE)
		{
			List<string> result = Generate(diffresult);
			var outputSb = new StringBuilder();
			while (result.Count > 0)
			{
				if (result[0].StartsWith(DeleteSymbol))
				{
					int dix = 0;
					outputSb.Append(deleteSymbolS);
					while (dix < result.Count &&  result.Count > 0 && !result[dix].StartsWith(NoChangeSymbol))
					{
						if (result[dix].StartsWith(DeleteSymbol))
						{
							outputSb.Append(result[dix].Substring(1));
							result.RemoveAt(dix);
						}
						else
						{
							++dix;
						}
					}
					outputSb.Append(deleteSymbolE);
				}
				else if (result[0].StartsWith(InsertSymbol))
				{
					int dix = 0;
					outputSb.Append(insertSymbolS);
					while (dix < result.Count &&  result.Count > 0 && !result[dix].StartsWith(NoChangeSymbol))
					{
						if (result[dix].StartsWith(InsertSymbol))
						{
							outputSb.Append(result[dix].Substring(1));
							result.RemoveAt(dix);
						}
						else
						{
							++dix;
						}
					}
					outputSb.Append(insertSymbolE);
				}
				else
				{
					outputSb.Append(result[0].Substring(1));
					result.RemoveAt(0);
				}
			}
			return outputSb.ToString();
		}
		
		public static string GenerateOnlyChangedSeq (DiffResult diffresult, string insertSymbolS, string insertSymbolE, string deleteSymbolS, string deleteSymbolE)
		{
			List<UnidiffEntry> result = GenerateWithLineNumbers(diffresult);
			var outputSb = new StringBuilder();
			while (result.Count > 0)
			{
				if (result[0].Type == UnidiffType.Delete)
				{
					int dix = 0;
					outputSb.Append(deleteSymbolS);
					while (dix < result.Count && result.Count > 0 && result[dix].Type != UnidiffType.NoChange)
					{
						if (result[dix].Type == UnidiffType.Delete)
						{
							outputSb.Append(result[dix].Index + ":");
							outputSb.Append(result[dix].Text.Substring(1));
							result.RemoveAt(dix);
						}
						else
						{
							++dix;
						}
					}
					outputSb.Append(deleteSymbolE);
				}
				else if (result[0].Type == UnidiffType.Insert)
				{
					int dix = 0;
					outputSb.Append(insertSymbolS);
					while (dix < result.Count && result.Count > 0 && result[dix].Type != UnidiffType.NoChange)
					{
						if (result[dix].Type == UnidiffType.Insert)
						{
							outputSb.Append(result[dix].Index + ":");
							outputSb.Append(result[dix].Text.Substring(1));
							result.RemoveAt(dix);
						}
						else
						{
							++dix;
						}
					}
					outputSb.Append(insertSymbolE);
				}
				else
				{
					//outputSb.Append(result[0].Index);
					//outputSb.Append(result[0].Text.Substring(1));
					result.RemoveAt(0);
				}
			}
			return outputSb.ToString();
		}
		
	}
}
