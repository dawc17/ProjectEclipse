using System;
using System.Runtime.Serialization;

namespace YamlDotNet.Core
{
	[Serializable]
	public class ForwardAnchorNotSupportedException : YamlException
	{
		public ForwardAnchorNotSupportedException()
		{
		}

		public ForwardAnchorNotSupportedException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public ForwardAnchorNotSupportedException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: base(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH)
		{
		}

		public ForwardAnchorNotSupportedException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected ForwardAnchorNotSupportedException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
