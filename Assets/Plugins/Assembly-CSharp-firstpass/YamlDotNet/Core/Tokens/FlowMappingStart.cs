using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class FlowMappingStart : Token
	{
		public FlowMappingStart()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public FlowMappingStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
