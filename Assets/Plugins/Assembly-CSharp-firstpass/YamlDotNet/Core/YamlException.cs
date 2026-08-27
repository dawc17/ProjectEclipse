using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace YamlDotNet.Core
{
	[Serializable]
	public class YamlException : Exception
	{
		public Mark Start { get; private set; }

		public Mark End { get; private set; }

		public YamlException()
		{
		}

		public YamlException(string LIOGIBJBHAH)
			: base(LIOGIBJBHAH)
		{
		}

		public YamlException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH)
			: this(ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH, null)
		{
		}

		public YamlException(Mark ILENLCMAMBH, Mark PCLFFOBJJFO, string LIOGIBJBHAH, Exception OLABPFGLNFC)
			: base(string.Format("({0}) - ({1}): {2}", ILENLCMAMBH, PCLFFOBJJFO, LIOGIBJBHAH), OLABPFGLNFC)
		{
			Start = ILENLCMAMBH;
			End = PCLFFOBJJFO;
		}

		public YamlException(string LIOGIBJBHAH, Exception LEPEAKBGHLB)
			: base(LIOGIBJBHAH, LEPEAKBGHLB)
		{
		}

		protected YamlException(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
			: base(EMBBNNBFODN, PDCAHMPCPOC)
		{
			Start = (Mark)EMBBNNBFODN.GetValue("Start", typeof(Mark));
			End = (Mark)EMBBNNBFODN.GetValue("End", typeof(Mark));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo EMBBNNBFODN, StreamingContext PDCAHMPCPOC)
		{
			base.GetObjectData(EMBBNNBFODN, PDCAHMPCPOC);
			EMBBNNBFODN.AddValue("Start", Start);
			EMBBNNBFODN.AddValue("End", End);
		}
	}
}
