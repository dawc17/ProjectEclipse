using System;
using System.Runtime.Serialization;

namespace YamlDotNet.Core
{
	[Serializable]
	public class SyntaxErrorException : YamlException
	{
		public SyntaxErrorException()
		{
		}

		public SyntaxErrorException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public SyntaxErrorException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: base(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH)
		{
		}

		public SyntaxErrorException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected SyntaxErrorException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
