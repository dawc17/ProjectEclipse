using System;
using System.Runtime.Serialization;

namespace YamlDotNet.Core
{
	[Serializable]
	public class DuplicateAnchorException : YamlException
	{
		public DuplicateAnchorException()
		{
		}

		public DuplicateAnchorException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public DuplicateAnchorException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: base(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH)
		{
		}

		public DuplicateAnchorException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected DuplicateAnchorException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
