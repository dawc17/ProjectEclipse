using System;
using System.Runtime.Serialization;

namespace Unity.IO.Compression
{
	[Serializable]
	public sealed class InvalidDataException : SystemException
	{
		public InvalidDataException()
			: base(SR.GetString("Invalid data"))
		{
		}

		public InvalidDataException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public InvalidDataException(string LIOGIBJBHAH, Exception OLABPFGLNFC)
			: base(LIOGIBJBHAH, OLABPFGLNFC)
		{
		}

		internal InvalidDataException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
		}
	}
}
