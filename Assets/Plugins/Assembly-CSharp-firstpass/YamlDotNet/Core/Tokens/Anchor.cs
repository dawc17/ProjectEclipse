using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Anchor : Token
	{
		private readonly string value;

		public string Value
		{
			get
			{
				return value;
			}
		}

		public Anchor(string value)
			: this(value, Mark.Empty, Mark.Empty)
		{
		}

		public Anchor(string value, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			value = value;
		}
	}
}
