using YamlDotNet.Core;

public class AKMKLAINLOL : ParsingEvent
{
	public override int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public AKMKLAINLOL(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
		: base(ILENLCMAMBH, PCLFFOBJJFO)
	{
	}

	public AKMKLAINLOL()
		: this(Mark.Empty, Mark.Empty)
	{
	}

	public override int DPIMLJJFMCO()
	{
		return -1;
	}

	internal override BHBPOHDAGPH get_Type()
	{
		return BHBPOHDAGPH.SequenceEnd;
	}

	public override string ToString()
	{
		return "Sequence end";
	}

	public override void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM)
	{
		NKECMANOOEM.Visit(this);
	}
}
