using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class SimpleKey
	{
		private readonly Cursor cursor;

		public bool IsPossible { get; set; }

		public bool IsRequired { get; private set; }

		public int TokenNumber { get; private set; }

		public int Index
		{
			get
			{
				return cursor.Index;
			}
		}

		public int Line
		{
			get
			{
				return cursor.Line;
			}
		}

		public int LineOffset
		{
			get
			{
				return cursor.LineOffset;
			}
		}

		public Mark Mark
		{
			get
			{
				return cursor.BJKDANAAGHK();
			}
		}

		public SimpleKey()
		{
			cursor = new Cursor();
		}

		public SimpleKey(bool LNJDPJDHNKI, bool MMIJJJMNNND, int MCJIOGPPMMF, Cursor LIMLDKKPJIA)
		{
			IsPossible = LNJDPJDHNKI;
			IsRequired = MMIJJJMNNND;
			TokenNumber = MCJIOGPPMMF;
			cursor = new Cursor(LIMLDKKPJIA);
		}
	}
}
