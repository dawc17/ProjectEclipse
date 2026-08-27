using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class AnchorAlias : Token
	{
		private readonly string value;

		public string Value
		{
			get
			{
				return value;
			}
		}

		public AnchorAlias(string value)
			: this(value, Mark.Empty, Mark.Empty)
		{
		}

		public AnchorAlias(string value, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			value = value;
		}
	}
}
