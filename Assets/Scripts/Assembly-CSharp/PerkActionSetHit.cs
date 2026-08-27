using System.Diagnostics;
using System.Xml;

public class PerkActionSetHit : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int KKHEAOHOGDP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int ABJLFNMMLHF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int OMKJAOPJECB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int FCHMLCFNHFF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension OEHJODCCHNB;

	public int DNGKOMPMPCD
	{
		get
		{
			return LFJCOGGNFHL();
		}
		protected set
		{
			LHLIHHMEBKM(value);
		}
	}

	public int DFOHNJEBDED
	{
		get
		{
			return IOAHLEKLBLE();
		}
		protected set
		{
			ECLHIGBMCLK(value);
		}
	}

	public int APCAKCCOMLO
	{
		get
		{
			return JEIAJBMLIBP();
		}
		protected set
		{
			BPLDKHPHCON(value);
		}
	}

	public int NIKPBGPPFEP
	{
		get
		{
			return NALPADHBLNH();
		}
		protected set
		{
			MLJCJOFKENH(value);
		}
	}

	public FunctionExtension KFMJMBANIGF
	{
		get
		{
			return GHGGNMBCMNM();
		}
		protected set
		{
			PJEADIKBIGL(value);
		}
	}

	public PerkActionSetHit()
	{
	}

	public PerkActionSetHit(PerkActionSetHit NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		LHLIHHMEBKM(NOLFMPDGCOC.LFJCOGGNFHL());
		ECLHIGBMCLK(NOLFMPDGCOC.IOAHLEKLBLE());
		BPLDKHPHCON(NOLFMPDGCOC.JEIAJBMLIBP());
		MLJCJOFKENH(NOLFMPDGCOC.NALPADHBLNH());
		PJEADIKBIGL(NOLFMPDGCOC.GHGGNMBCMNM());
	}

	public int LFJCOGGNFHL()
	{
		return KKHEAOHOGDP;
	}

	protected void LHLIHHMEBKM(int value)
	{
		KKHEAOHOGDP = value;
	}

	public int IOAHLEKLBLE()
	{
		return ABJLFNMMLHF;
	}

	protected void ECLHIGBMCLK(int value)
	{
		ABJLFNMMLHF = value;
	}

	public int JEIAJBMLIBP()
	{
		return OMKJAOPJECB;
	}

	protected void BPLDKHPHCON(int value)
	{
		OMKJAOPJECB = value;
	}

	public int NALPADHBLNH()
	{
		return FCHMLCFNHFF;
	}

	protected void MLJCJOFKENH(int value)
	{
		FCHMLCFNHFF = value;
	}

	public FunctionExtension GHGGNMBCMNM()
	{
		return OEHJODCCHNB;
	}

	protected void PJEADIKBIGL(FunctionExtension value)
	{
		OEHJODCCHNB = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_HIT);
		LHLIHHMEBKM(node.Attributes["Critical"].ParseInt(-1));
		ECLHIGBMCLK(node.Attributes["Block"].ParseInt(-1));
		BPLDKHPHCON(node.Attributes["Shock"].ParseInt(-1));
		MLJCJOFKENH(node.Attributes["Disarm"].ParseInt(-1));
		PJEADIKBIGL(null);
		string text = node.Attributes["Damage"].CIPOICEEIBK(string.Empty);
		if (text != null && text != string.Empty)
		{
			PJEADIKBIGL(new FunctionExtension());
			GHGGNMBCMNM().Parse(text);
			GHGGNMBCMNM().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			GHGGNMBCMNM().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			GHGGNMBCMNM().set_Target(this);
		}
	}
}
