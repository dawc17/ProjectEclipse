using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class FlowSequenceEnd : Token
	{
		public FlowSequenceEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public FlowSequenceEnd(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
		}
	}
}
