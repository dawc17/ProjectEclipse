using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class BlockMappingStart : Token
	{
		public BlockMappingStart()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public BlockMappingStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
