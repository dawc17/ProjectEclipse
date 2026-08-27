using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public class PerkEventPostHit : PerkEvent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HBOIDIAFOLL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NCKLKPMLJBI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int BKKNGMFIJOK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int NPOILBCHAKH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int FKDHEOMLJGF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float BMJGECOIEHP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float CHFBEEIDMCI;

	public string GBOKABKLCFM
	{
		get
		{
			return NLLGDDMMJJN();
		}
		protected set
		{
			CFFCAJLFBEM(value);
		}
	}

	public string FGICHADOEHF
	{
		get
		{
			return NNMAFFCCMHC();
		}
		protected set
		{
			DBDJHIHLCFD(value);
		}
	}

	public int FHJMFEEMMGO
	{
		get
		{
			return KDCHLKEMDPC();
		}
		protected set
		{
			NDIGOAPGCPI(value);
		}
	}

	public int POCBCFMBKLO
	{
		get
		{
			return MONOMCAGAEO();
		}
		protected set
		{
			MPCHGJHHJMC(value);
		}
	}

	public int PFDCDIBODCL
	{
		get
		{
			return EDJFLMILEBA();
		}
		protected set
		{
			set_IsShock(value);
		}
	}

	public float NDLKKCILIPA
	{
		get
		{
			return KJPMFOLDHJH();
		}
		protected set
		{
			OGAJJIPGFGK(value);
		}
	}

	public float MOFNJCLLKMB
	{
		get
		{
			return GLIHMMBPDKB();
		}
		protected set
		{
			MJLFPHCPKBN(value);
		}
	}

	public PerkEventPostHit()
	{
	}

	public PerkEventPostHit(PerkEventPostHit NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		CFFCAJLFBEM(NOLFMPDGCOC.NLLGDDMMJJN());
		DBDJHIHLCFD(NOLFMPDGCOC.NNMAFFCCMHC());
		NDIGOAPGCPI(NOLFMPDGCOC.KDCHLKEMDPC());
		MPCHGJHHJMC(NOLFMPDGCOC.MONOMCAGAEO());
		set_IsShock(NOLFMPDGCOC.EDJFLMILEBA());
		OGAJJIPGFGK(NOLFMPDGCOC.KJPMFOLDHJH());
		MJLFPHCPKBN(NOLFMPDGCOC.GLIHMMBPDKB());
	}

	public string NLLGDDMMJJN()
	{
		return HBOIDIAFOLL;
	}

	protected void CFFCAJLFBEM(string value)
	{
		HBOIDIAFOLL = value;
	}

	public string NNMAFFCCMHC()
	{
		return NCKLKPMLJBI;
	}

	protected void DBDJHIHLCFD(string value)
	{
		NCKLKPMLJBI = value;
	}

	public int KDCHLKEMDPC()
	{
		return BKKNGMFIJOK;
	}

	protected void NDIGOAPGCPI(int value)
	{
		BKKNGMFIJOK = value;
	}

	public int MONOMCAGAEO()
	{
		return NPOILBCHAKH;
	}

	protected void MPCHGJHHJMC(int value)
	{
		NPOILBCHAKH = value;
	}

	public int EDJFLMILEBA()
	{
		return FKDHEOMLJGF;
	}

	protected void set_IsShock(int value)
	{
		FKDHEOMLJGF = value;
	}

	public float KJPMFOLDHJH()
	{
		return BMJGECOIEHP;
	}

	protected void OGAJJIPGFGK(float value)
	{
		BMJGECOIEHP = value;
	}

	public float GLIHMMBPDKB()
	{
		return CHFBEEIDMCI;
	}

	protected void MJLFPHCPKBN(float value)
	{
		CHFBEEIDMCI = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		CFFCAJLFBEM(node.Attributes["Defense"].CIPOICEEIBK(string.Empty));
		NDIGOAPGCPI(node.Attributes["Block"].ParseInt(-1));
		MPCHGJHHJMC(node.Attributes["Critical"].ParseInt(-1));
		set_IsShock(node.Attributes["Shock"].ParseInt(-1));
		DBDJHIHLCFD(node.Attributes["Animation"].CIPOICEEIBK(string.Empty));
		OGAJJIPGFGK(node.Attributes["DamageMin"].ParseFloat(-1f));
		MJLFPHCPKBN(node.Attributes["DamageMax"].ParseFloat(-1f));
	}

	public override bool IsEqual(EventStruct EJMEALJNNIL)
	{
		if (!base.IsEqual(EJMEALJNNIL) || EJMEALJNNIL == null || EJMEALJNNIL.Info == null)
		{
			return false;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)EJMEALJNNIL.Info;
		InfoAnimation pJAHIOELGGD = ((!dictionary.ContainsKey("Animation")) ? null : ((InfoAnimation)dictionary["Animation"]));
		string text = ((!dictionary.ContainsKey("Defense")) ? null : ((string)dictionary["Defense"]));
		bool flag = dictionary.ContainsKey("Critical") && (bool)dictionary["Critical"];
		bool flag2 = dictionary.ContainsKey("Shock") && (bool)dictionary["Shock"];
		bool flag3 = dictionary.ContainsKey("Block") && (bool)dictionary["Block"];
		float num = ((!dictionary.ContainsKey("Damage")) ? 0f : ((float)dictionary["Damage"]));
		if (NLLGDDMMJJN() != string.Empty && NLLGDDMMJJN() != text)
		{
			return false;
		}
		if (NNMAFFCCMHC() != null && !NNMAFFCCMHC().Equals(string.Empty) && (pJAHIOELGGD == null || !pJAHIOELGGD.CNPFHBMGDFP(NNMAFFCCMHC())))
		{
			return false;
		}
		if (MONOMCAGAEO() > -1 && MONOMCAGAEO() != (flag ? 1 : 0))
		{
			return false;
		}
		if (EDJFLMILEBA() > -1 && EDJFLMILEBA() != (flag2 ? 1 : 0))
		{
			return false;
		}
		if (KDCHLKEMDPC() > -1 && KDCHLKEMDPC() != (flag3 ? 1 : 0))
		{
			return false;
		}
		if (KJPMFOLDHJH() > -1f && num < KJPMFOLDHJH())
		{
			return false;
		}
		if (GLIHMMBPDKB() > -1f && num > GLIHMMBPDKB())
		{
			return false;
		}
		return true;
	}
}
