using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Scalar : Token
	{
		private readonly string value;

		private readonly IBEOFCPMMJJ style;

		public string Value
		{
			get
			{
				return value;
			}
		}

		public IBEOFCPMMJJ Style
		{
			get
			{
				return style;
			}
		}

		public Scalar(string value)
			: this(value, IBEOFCPMMJJ.Any)
		{
		}

		public Scalar(string value, IBEOFCPMMJJ KIGNIBIMLKK)
			: this(value, KIGNIBIMLKK, Mark.Empty, Mark.Empty)
		{
		}

		public Scalar(string value, IBEOFCPMMJJ KIGNIBIMLKK, Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
			: base(ILENLCMAMBH, PCLFFOBJJFO)
		{
			value = value;
			style = KIGNIBIMLKK;
		}
	}
}
