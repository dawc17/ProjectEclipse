using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Mark : IEquatable<Mark>, IComparable<Mark>, IComparable
	{
		public static readonly Mark Empty = new Mark();

		public int Index { get; private set; }

		public int Line { get; private set; }

		public int Column { get; private set; }

		public Mark()
		{
			Line = 1;
			Column = 1;
		}

		public Mark(int index, int MGPBPJOHMLH, int DLPJJBPDNDE)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Index must be greater than or equal to zero.");
			}
			if (MGPBPJOHMLH < 1)
			{
				throw new ArgumentOutOfRangeException("line", "Line must be greater than or equal to 1.");
			}
			if (DLPJJBPDNDE < 1)
			{
				throw new ArgumentOutOfRangeException("column", "Column must be greater than or equal to 1.");
			}
			Index = index;
			Line = MGPBPJOHMLH;
			Column = DLPJJBPDNDE;
		}

		public override string ToString()
		{
			return string.Format("Line: {0}, Col: {1}, Idx: {2}", Line, Column, Index);
		}

		public override bool Equals(object AOMLCBHAJJH)
		{
			return Equals(AOMLCBHAJJH as Mark);
		}

		public bool Equals(Mark NOLFMPDGCOC)
		{
			return NOLFMPDGCOC != null && Index == NOLFMPDGCOC.Index && Line == NOLFMPDGCOC.Line && Column == NOLFMPDGCOC.Column;
		}

		public override int GetHashCode()
		{
			return HashCode.CombineHashCodes(Index.GetHashCode(), HashCode.CombineHashCodes(Line.GetHashCode(), Column.GetHashCode()));
		}

		public int CompareTo(object AOMLCBHAJJH)
		{
			if (AOMLCBHAJJH == null)
			{
				throw new ArgumentNullException("obj");
			}
			return CompareTo(AOMLCBHAJJH as Mark);
		}

		public int CompareTo(Mark NOLFMPDGCOC)
		{
			if (NOLFMPDGCOC == null)
			{
				throw new ArgumentNullException("other");
			}
			int num = Line.CompareTo(NOLFMPDGCOC.Line);
			if (num == 0)
			{
				num = Column.CompareTo(NOLFMPDGCOC.Column);
			}
			return num;
		}
	}
}
