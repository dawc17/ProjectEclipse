using System;
using System.Runtime.Serialization;

namespace YamlDotNet.Core
{
	[Serializable]
	public class AnchorNotFoundException : YamlException
	{
		public AnchorNotFoundException()
		{
		}

		public AnchorNotFoundException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public AnchorNotFoundException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: base(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH)
		{
		}

		public AnchorNotFoundException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected AnchorNotFoundException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
