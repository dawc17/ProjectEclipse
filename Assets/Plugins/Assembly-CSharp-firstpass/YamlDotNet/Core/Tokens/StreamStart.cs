using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class StreamStart : Token
	{
		public StreamStart()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public StreamStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
