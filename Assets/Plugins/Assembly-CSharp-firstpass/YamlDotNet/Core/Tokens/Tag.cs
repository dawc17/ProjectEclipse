using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Tag : Token
	{
		private readonly string handle;

		private readonly string suffix;

		public string Handle
		{
			get
			{
				return handle;
			}
		}

		public string Suffix
		{
			get
			{
				return suffix;
			}
		}

		public Tag(string FODGADCGDBH, string NCFFAGOLJEC)
			: this(FODGADCGDBH, NCFFAGOLJEC, Mark.Empty, Mark.Empty)
		{
		}

		public Tag(string FODGADCGDBH, string NCFFAGOLJEC, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			handle = FODGADCGDBH;
			suffix = NCFFAGOLJEC;
		}
	}
}
