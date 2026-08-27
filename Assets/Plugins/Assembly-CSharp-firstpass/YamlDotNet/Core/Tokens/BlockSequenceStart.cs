using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class BlockSequenceStart : Token
	{
		public BlockSequenceStart()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public BlockSequenceStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
