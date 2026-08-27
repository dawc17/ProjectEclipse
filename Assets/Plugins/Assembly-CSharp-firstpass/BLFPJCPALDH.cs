using YamlDotNet.Core;

public class BLFPJCPALDH : ParsingEvent
{
	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public BLFPJCPALDH(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
	}

	public BLFPJCPALDH()
		: this(Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return -1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.MappingEnd;
	}

	public override string ToString()
	{
		return "Mapping end";
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
