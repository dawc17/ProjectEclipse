using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class StreamEnd : Token
	{
		public StreamEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public StreamEnd(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
