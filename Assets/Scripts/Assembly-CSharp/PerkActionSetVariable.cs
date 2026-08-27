using System.Diagnostics;
using System.Xml;

public class PerkActionSetVariable : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension IELPCLONGKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FOEIHFCIDNJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension EBLMFFEMNCC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ECOOFPHOHMP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension JMKHMGFMDJP;

	public bool LGCOMCPNMJM
	{
		get
		{
			return CPDLBMAKCEK();
		}
		protected set
		{
			LPNGJDNOJNJ(value);
		}
	}

	public FunctionExtension IMEGHKOIKLC
	{
		get
		{
			return MCOHCDPJHAK();
		}
		protected set
		{
			CMEPINAFLGH(value);
		}
	}

	public bool ODEBGKLHLIF
	{
		get
		{
			return OMLFBFOFJDD();
		}
		protected set
		{
			BICMECLDLIP(value);
		}
	}

	public FunctionExtension EFANAIIGEMO
	{
		get
		{
			return BHIGOIHJBDK();
		}
		protected set
		{
			ALDNPBOAANA(value);
		}
	}

	public PerkActionSetVariable()
	{
	}

	public PerkActionSetVariable(PerkActionSetVariable NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Value(NOLFMPDGCOC.OEAKCOHMIHH());
		LPNGJDNOJNJ(NOLFMPDGCOC.CPDLBMAKCEK());
		CMEPINAFLGH(NOLFMPDGCOC.MCOHCDPJHAK());
		BICMECLDLIP(NOLFMPDGCOC.OMLFBFOFJDD());
		ALDNPBOAANA(NOLFMPDGCOC.BHIGOIHJBDK());
	}

	public FunctionExtension OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	protected void set_Value(FunctionExtension value)
	{
		IELPCLONGKP = value;
	}

	public bool CPDLBMAKCEK()
	{
		return FOEIHFCIDNJ;
	}

	protected void LPNGJDNOJNJ(bool value)
	{
		FOEIHFCIDNJ = value;
	}

	public FunctionExtension MCOHCDPJHAK()
	{
		return EBLMFFEMNCC;
	}

	protected void CMEPINAFLGH(FunctionExtension value)
	{
		EBLMFFEMNCC = value;
	}

	public bool OMLFBFOFJDD()
	{
		return ECOOFPHOHMP;
	}

	protected void BICMECLDLIP(bool value)
	{
		ECOOFPHOHMP = value;
	}

	public FunctionExtension BHIGOIHJBDK()
	{
		return JMKHMGFMDJP;
	}

	protected void ALDNPBOAANA(FunctionExtension value)
	{
		JMKHMGFMDJP = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_VARIABLE);
		string text = node.Attributes["Value"].CIPOICEEIBK(string.Empty);
		if (text != null && text != string.Empty)
		{
			set_Value(new FunctionExtension());
			OEAKCOHMIHH().Parse(text);
			OEAKCOHMIHH().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			OEAKCOHMIHH().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			OEAKCOHMIHH().set_Target(this);
		}
		XmlAttribute xmlAttribute = node.Attributes["MinValue"];
		LPNGJDNOJNJ(xmlAttribute != null);
		string text2 = xmlAttribute.CIPOICEEIBK(string.Empty);
		if (text2 != null && text2 != string.Empty)
		{
			CMEPINAFLGH(new FunctionExtension());
			MCOHCDPJHAK().Parse(text2);
			MCOHCDPJHAK().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			MCOHCDPJHAK().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			MCOHCDPJHAK().set_Target(this);
		}
		XmlAttribute xmlAttribute2 = node.Attributes["MaxValue"];
		BICMECLDLIP(xmlAttribute2 != null);
		string text3 = xmlAttribute2.CIPOICEEIBK(string.Empty);
		if (text3 != null && text3 != string.Empty)
		{
			ALDNPBOAANA(new FunctionExtension());
			BHIGOIHJBDK().Parse(text3);
			BHIGOIHJBDK().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			BHIGOIHJBDK().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			BHIGOIHJBDK().set_Target(this);
		}
	}
}
