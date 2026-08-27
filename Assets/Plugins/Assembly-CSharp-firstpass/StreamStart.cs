using YamlDotNet.Core;

public class StreamStart : ParsingEvent
{
	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public StreamStart()
		: this(Mark.Empty, Mark.Empty)
	{
	}

	public StreamStart(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return 1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.StreamStart;
	}

	public override string ToString()
	{
		return "Stream start";
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
