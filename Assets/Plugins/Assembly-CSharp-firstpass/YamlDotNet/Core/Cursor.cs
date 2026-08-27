using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class Cursor
	{
		public int Index { get; set; }

		public int Line { get; set; }

		public int LineOffset { get; set; }

		public Cursor()
		{
			Line = 1;
		}

		public Cursor(Cursor LIMLDKKPJIA)
		{
			Index = LIMLDKKPJIA.Index;
			Line = LIMLDKKPJIA.Line;
			LineOffset = LIMLDKKPJIA.LineOffset;
		}

		public Mark BJKDANAAGHK()
		{
			return new Mark(Index, Line, LineOffset + 1);
		}

		public void Skip()
		{
			Index++;
			LineOffset++;
		}

		public void AIGOMGCEJJD(int IPCOBJBKNAO)
		{
			Index += IPCOBJBKNAO;
			Line++;
			LineOffset = 0;
		}

		public void JFJBGABDLJM()
		{
			if (LineOffset != 0)
			{
				Line++;
				LineOffset = 0;
			}
		}
	}
}
