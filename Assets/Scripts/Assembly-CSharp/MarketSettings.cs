using System.Xml;

public class MarketSettings
{
	private bool MFEKNCELHAP;

	private bool POGHNJGEFEA;

	private bool FNAMFKPJPGC;

	private bool MBPOBBKJGEP;

	private bool GEOCMAAICHH;

	private bool GGLDMLFGFKB;

	private bool ELACJCBLALD;

	private bool OHINGABDMDH;

	public bool KBBBCKFOGBM
	{
		get
		{
			return BKGIFIPIHAL();
		}
	}

	public bool OIMBIEPEEAK
	{
		get
		{
			return GIGEOMONCON();
		}
	}

	public bool HPBDACCIMKG
	{
		get
		{
			return DMJJDFCAKFG();
		}
	}

	public bool LJKLEBDLAFI
	{
		get
		{
			return NPNOMBEEPJD();
		}
	}

	public bool ANMKPNMPKJA
	{
		get
		{
			return OPCBKOOFMAK();
		}
	}

	public bool FKCDDHMHFLG
	{
		get
		{
			return DBJOHGNPDDO();
		}
	}

	public bool PEFMKEAGJGK
	{
		get
		{
			return COPJOJAMBKA();
		}
	}

	public bool EDMLAOFBHHC
	{
		get
		{
			return OKALPNOADLJ();
		}
	}

	public void Parse(XmlNode node)
	{
		MFEKNCELHAP = node["China"].PNJPEDPDMCP().ParseBool();
		POGHNJGEFEA = node["Japan"].PNJPEDPDMCP().ParseBool();
		FNAMFKPJPGC = node["Korea"].PNJPEDPDMCP().ParseBool();
		MBPOBBKJGEP = node["Amazon"].PNJPEDPDMCP().ParseBool();
		GEOCMAAICHH = node["AmazonMobile"].PNJPEDPDMCP().ParseBool();
		GGLDMLFGFKB = node["Steam"].PNJPEDPDMCP().ParseBool();
		ELACJCBLALD = node["AndroidTV"].PNJPEDPDMCP().ParseBool();
		OHINGABDMDH = node["WinStore"].PNJPEDPDMCP().ParseBool();
	}

	public bool BKGIFIPIHAL()
	{
		return MFEKNCELHAP;
	}

	public bool GIGEOMONCON()
	{
		return POGHNJGEFEA;
	}

	public bool DMJJDFCAKFG()
	{
		return FNAMFKPJPGC;
	}

	public bool NPNOMBEEPJD()
	{
		return MBPOBBKJGEP;
	}

	public bool OPCBKOOFMAK()
	{
		return GEOCMAAICHH;
	}

	public bool DBJOHGNPDDO()
	{
		return GGLDMLFGFKB;
	}

	public bool COPJOJAMBKA()
	{
		return ELACJCBLALD;
	}

	public bool OKALPNOADLJ()
	{
		return OHINGABDMDH;
	}
}
