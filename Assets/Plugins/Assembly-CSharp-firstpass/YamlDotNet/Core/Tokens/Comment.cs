using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Comment : Token
	{
		public string Value { get; private set; }

		public bool IsInline { get; private set; }

		public Comment(string value, bool EKOKIGANOMO)
			: this(value, EKOKIGANOMO, Mark.Empty, Mark.Empty)
		{
		}

		public Comment(string value, bool EKOKIGANOMO, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			IsInline = EKOKIGANOMO;
			Value = value;
		}
	}
}
