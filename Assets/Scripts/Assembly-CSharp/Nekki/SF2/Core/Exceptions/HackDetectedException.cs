using System;
using System.Runtime.Serialization;

namespace Nekki.SF2.Core.Exceptions
{
	[Serializable]
	public class HackDetectedException : Exception
	{
		public HackDetectedException()
		{
		}

		public HackDetectedException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public HackDetectedException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		public HackDetectedException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
		{
		}
	}
}
