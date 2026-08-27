using YamlDotNet.Core;

public abstract class ParsingEvent
{
	private readonly Mark ILENLCMAMBH;

	private readonly Mark PCLFFOBJJFO;

	public virtual int OHJMGKADENE
	{
		get
		{
			return DPIMLJJFMCO();
		}
	}

	public Mark Start
	{
		get
		{
			return OGPHJPFHBJL();
		}
	}

	public Mark PLHPGFGAGKJ
	{
		get
		{
			return GDJHIJHFPHA();
		}
	}

	internal ParsingEvent(Mark ILENLCMAMBH, Mark PCLFFOBJJFO)
	{
		this.ILENLCMAMBH = ILENLCMAMBH;
		this.PCLFFOBJJFO = PCLFFOBJJFO;
	}

	public virtual int DPIMLJJFMCO()
	{
		return 0;
	}

	internal abstract BHBPOHDAGPH get_Type();

	public Mark OGPHJPFHBJL()
	{
		return ILENLCMAMBH;
	}

	public Mark GDJHIJHFPHA()
	{
		return PCLFFOBJJFO;
	}

	public abstract void GPHIFFOGOGN(IParsingEventVisitor NKECMANOOEM);
}
