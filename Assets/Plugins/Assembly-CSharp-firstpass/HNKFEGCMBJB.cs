using YamlDotNet.Core;

public class HNKFEGCMBJB : ParsingEvent
{
	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public HNKFEGCMBJB(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
	}

	public HNKFEGCMBJB()
		: this(Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return -1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.StreamEnd;
	}

	public override string ToString()
	{
		return "Stream end";
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
