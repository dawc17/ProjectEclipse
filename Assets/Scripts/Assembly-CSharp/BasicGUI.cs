using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public static class BasicGUI
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static KeyValuePair<int, int> EOBKMNOMCDM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static KeyValuePair<int, int> JEABOCGBAEG;

	private static int NPAANDHOALC = 0;

	private static float ANFMAFNDIBG = 0f;

	private static int MOFPGFCLIBL = 0;

	private static float BJOBJCMCGJM = 0f;

	private static bool _ShowMenuTime = false;

	private static int EJMLGPHEACK = 0;

	private static Dictionary<int, int> _CounterRollTime = new Dictionary<int, int>();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static float PBBPPAHGGPH;

	public static KeyValuePair<int, int> NCEBBPBMAPO
	{
		get
		{
			return NPLEAPOPBLG();
		}
		private set
		{
			PDEIJLKGDIE(value);
		}
	}

	public static KeyValuePair<int, int> KGBEIBIKLBO
	{
		get
		{
			return BHIFPDKNBBE();
		}
		private set
		{
			BHBECFMOKFK(value);
		}
	}

	public static int EJDEIMMODDL
	{
		get
		{
			return BMLFDECMNLO();
		}
	}

	public static float PCGLNBCBDHB
	{
		get
		{
			return NEADIBJAJCM();
		}
	}

	public static int MJMLIKHFJJE
	{
		get
		{
			return HNJBADGLFEC();
		}
	}

	public static float IEBABMOIKEF
	{
		get
		{
			return HLLCEHACMLI();
		}
	}

	public static bool FENLEANBINI
	{
		get
		{
			return JKNCLNKNOKC();
		}
	}

	public static int BPKIIPLOPKF
	{
		get
		{
			return EPBJMALCHAA();
		}
	}

	public static Dictionary<int, int> PGMNMDLDGMC
	{
		get
		{
			return JHKFAMCDPNB();
		}
	}

	public static float PFBALNJIDJP
	{
		get
		{
			return KMJDBLBFEMF();
		}
		private set
		{
			set_NotificationReadTime(value);
		}
	}

	public static void Parse(XmlNode node)
	{
		PDEIJLKGDIE(node["ButtonWidth"].MMHOOIPHOMI(242, 342));
		BHBECFMOKFK(node["HintWidth"].MMHOOIPHOMI(242, 342));
		NPAANDHOALC = node["DefaultButtonCenterWidth"].PNJPEDPDMCP().ParseInt();
		ANFMAFNDIBG = node["HintTimeout"].PNJPEDPDMCP().ParseFloat(1f);
		MOFPGFCLIBL = node["ArrowFlashingFrames"].PNJPEDPDMCP().ParseInt(120);
		BJOBJCMCGJM = node["CreditsScrollSpeed"].PNJPEDPDMCP().ParseFloat(2f);
		_ShowMenuTime = node["ShowMenuTime"].PNJPEDPDMCP().ParseBool();
		set_NotificationReadTime(node["NotificationDlgDefaultReadTime"].PNJPEDPDMCP().ParseFloat());
		XmlNode xmlNode = node["CurrencyCounterRollTime"];
		if (xmlNode == null)
		{
			return;
		}
		_CounterRollTime.Clear();
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Name.Equals("DefaultRollTime"))
			{
				EJMLGPHEACK = childNode.MGCGBMLHIDP().ParseInt(120);
				continue;
			}
			int key = childNode.PNJPEDPDMCP().ParseInt();
			int value = childNode.MGCGBMLHIDP().ParseInt();
			_CounterRollTime[key] = value;
		}
	}

	public static KeyValuePair<int, int> NPLEAPOPBLG()
	{
		return EOBKMNOMCDM;
	}

	private static void PDEIJLKGDIE(KeyValuePair<int, int> value)
	{
		EOBKMNOMCDM = value;
	}

	public static KeyValuePair<int, int> BHIFPDKNBBE()
	{
		return JEABOCGBAEG;
	}

	private static void BHBECFMOKFK(KeyValuePair<int, int> value)
	{
		JEABOCGBAEG = value;
	}

	public static int BMLFDECMNLO()
	{
		return NPAANDHOALC;
	}

	public static float NEADIBJAJCM()
	{
		return ANFMAFNDIBG;
	}

	public static int HNJBADGLFEC()
	{
		return MOFPGFCLIBL;
	}

	public static float HLLCEHACMLI()
	{
		return HLLCEHACMLI();
	}

	public static bool JKNCLNKNOKC()
	{
		return JKNCLNKNOKC();
	}

	public static int EPBJMALCHAA()
	{
		return EJMLGPHEACK;
	}

	public static Dictionary<int, int> JHKFAMCDPNB()
	{
		return _CounterRollTime;
	}

	public static float KMJDBLBFEMF()
	{
		return PBBPPAHGGPH;
	}

	private static void set_NotificationReadTime(float value)
	{
		PBBPPAHGGPH = value;
	}
}
