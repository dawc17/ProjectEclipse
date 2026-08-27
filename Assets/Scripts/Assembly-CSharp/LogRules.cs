using System.Diagnostics;
using System.Xml;

public class LogRules
{
	private static LogRules instance;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ENHCEHLAMMH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool NAOCDIGNDDF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KHEIMJHEKJI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HNNNPDIBBIF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool LCNBCCGBONK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HLCEMMFOJNJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool INJGBGBBJPC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool LIPLBNBAGCP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ENGCLADFDKK;

	public static LogRules BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public bool CJGACJBAFLB
	{
		get
		{
			return AEAIDFAJDDK();
		}
		set
		{
			BLCHPOJDLOB(value);
		}
	}

	public bool GLHICPIHDKA
	{
		get
		{
			return MDKADLMMJLD();
		}
		set
		{
			CEEBLKBHOOA(value);
		}
	}

	public bool ACNLGBKEKNA
	{
		get
		{
			return PIAKPGMPGMN();
		}
		set
		{
			NMLHALIEGID(value);
		}
	}

	public bool CHDBPGDJLIL
	{
		get
		{
			return JFHDKEJCLIL();
		}
		set
		{
			LBNKDCHANDE(value);
		}
	}

	public bool LLNGIKNDILO
	{
		get
		{
			return OLCCPLPEBNG();
		}
		set
		{
			MLOKOKJBCBI(value);
		}
	}

	public bool IOLEMKNCHOC
	{
		get
		{
			return CHLJPKPGAMB();
		}
		set
		{
			NDGLEJOKMIL(value);
		}
	}

	public bool LGAFEIJMKGL
	{
		get
		{
			return PKFOGIBLDJK();
		}
		set
		{
			JBDJFDBPMPL(value);
		}
	}

	public bool AEMOAEPCCMM
	{
		get
		{
			return BJCOMMOMKCJ();
		}
		set
		{
			FLFFOMGNACI(value);
		}
	}

	public bool JHMKINJHFGM
	{
		get
		{
			return DKHBLILFCOA();
		}
		set
		{
			AJIGADCNOON(value);
		}
	}

	private LogRules()
	{
		BLCHPOJDLOB(false);
		CEEBLKBHOOA(false);
		NMLHALIEGID(false);
		LBNKDCHANDE(false);
		MLOKOKJBCBI(false);
		NDGLEJOKMIL(false);
		JBDJFDBPMPL(false);
		FLFFOMGNACI(false);
		AJIGADCNOON(false);
	}

	public static LogRules ELEBLBJKDBI()
	{
		if (instance == null)
		{
			instance = new LogRules();
		}
		return instance;
	}

	public bool AEAIDFAJDDK()
	{
		return ENHCEHLAMMH;
	}

	public void BLCHPOJDLOB(bool value)
	{
		ENHCEHLAMMH = value;
	}

	public bool MDKADLMMJLD()
	{
		return NAOCDIGNDDF;
	}

	public void CEEBLKBHOOA(bool value)
	{
		NAOCDIGNDDF = value;
	}

	public bool PIAKPGMPGMN()
	{
		return KHEIMJHEKJI;
	}

	public void NMLHALIEGID(bool value)
	{
		KHEIMJHEKJI = value;
	}

	public bool JFHDKEJCLIL()
	{
		return HNNNPDIBBIF;
	}

	public void LBNKDCHANDE(bool value)
	{
		HNNNPDIBBIF = value;
	}

	public bool OLCCPLPEBNG()
	{
		return LCNBCCGBONK;
	}

	public void MLOKOKJBCBI(bool value)
	{
		LCNBCCGBONK = value;
	}

	public bool CHLJPKPGAMB()
	{
		return HLCEMMFOJNJ;
	}

	public void NDGLEJOKMIL(bool value)
	{
		HLCEMMFOJNJ = value;
	}

	public bool PKFOGIBLDJK()
	{
		return INJGBGBBJPC;
	}

	public void JBDJFDBPMPL(bool value)
	{
		INJGBGBBJPC = value;
	}

	public bool BJCOMMOMKCJ()
	{
		return LIPLBNBAGCP;
	}

	public void FLFFOMGNACI(bool value)
	{
		LIPLBNBAGCP = value;
	}

	public bool DKHBLILFCOA()
	{
		return ENGCLADFDKK;
	}

	public void AJIGADCNOON(bool value)
	{
		ENGCLADFDKK = value;
	}

	private bool EBFKDDAMNHD(XmlNode node, bool AGADEMLBJGJ = false)
	{
		return (node == null) ? AGADEMLBJGJ : XmlUtils.ParseBool(node.Attributes[0], AGADEMLBJGJ);
	}

	public void Parse(XmlNode node)
	{
		BLCHPOJDLOB(EBFKDDAMNHD(node));
		if (AEAIDFAJDDK())
		{
			XmlNode xmlNode = node["Quests"];
			CEEBLKBHOOA(EBFKDDAMNHD(xmlNode));
			if (MDKADLMMJLD())
			{
				XmlNode hKPPBKPJOEO = xmlNode["Actions"];
				NMLHALIEGID(EBFKDDAMNHD(hKPPBKPJOEO));
			}
			LBNKDCHANDE(EBFKDDAMNHD(node["Animations"]));
			FLFFOMGNACI(EBFKDDAMNHD(node["Tactics"]));
			AJIGADCNOON(EBFKDDAMNHD(node["Perks"]));
			XmlNode xmlNode2 = node["Hits"];
			MLOKOKJBCBI(EBFKDDAMNHD(xmlNode2));
			if (OLCCPLPEBNG())
			{
				NDGLEJOKMIL(EBFKDDAMNHD(xmlNode2["Damage"]));
				JBDJFDBPMPL(EBFKDDAMNHD(xmlNode2["Style"]));
			}
		}
	}
}
