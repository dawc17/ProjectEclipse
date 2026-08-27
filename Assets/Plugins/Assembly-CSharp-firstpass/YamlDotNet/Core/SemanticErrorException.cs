using System;
using System.Runtime.Serialization;

namespace YamlDotNet.Core
{
	[Serializable]
	public class SemanticErrorException : YamlException
	{
		public SemanticErrorException()
		{
		}

		public SemanticErrorException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public SemanticErrorException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: base(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH)
		{
		}

		public SemanticErrorException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected SemanticErrorException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
