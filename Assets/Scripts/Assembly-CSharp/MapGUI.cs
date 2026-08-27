using System.Collections.Generic;
using System.Xml;

public class MapGUI
{
	public struct IKOLLMFIEPA
	{
		public int IEKAFNFKBNE;

		public int HPJHAIALGHN;

		public int CPLJCIFJAGN;

		public int BGDLPJKHBHP;
	}

	public struct PLDPCIECIIF
	{
		public int IEKAFNFKBNE;

		public int HPJHAIALGHN;

		public int CPLJCIFJAGN;

		public int LHMLMFOMALK;
	}

	public struct FFOFPPKGDCH
	{
		public int IEKAFNFKBNE;

		public int HPJHAIALGHN;

		public int CPLJCIFJAGN;

		public List<string> GBDHOPBMLHK;
	}

	public struct KJNFHJIMBDB
	{
		public float MNADIKCPPIG;

		public float MIFFMBOIAGC;
	}

	public static IKOLLMFIEPA HPDGECMMHBJ = default(IKOLLMFIEPA);

	public static FFOFPPKGDCH JHLMDGBGGEP = default(FFOFPPKGDCH);

	public static KJNFHJIMBDB ELPKAJAKAEL = default(KJNFHJIMBDB);

	public static PLDPCIECIIF GMKNDIELELN = default(PLDPCIECIIF);

	public static void Parse(XmlNode node)
	{
		if (node == null)
		{
			return;
		}
		XmlNode xmlNode = node["RewardLine"];
		ELPKAJAKAEL.MNADIKCPPIG = xmlNode["OscillationPeriod"].Attributes["Value"].ParseFloat();
		ELPKAJAKAEL.MIFFMBOIAGC = xmlNode["OscillationFactor"].Attributes["Value"].ParseFloat();
		XmlNode xmlNode2 = node["Challenge"];
		HPDGECMMHBJ.IEKAFNFKBNE = xmlNode2["MinOpacity"].PNJPEDPDMCP().ParseInt();
		HPDGECMMHBJ.HPJHAIALGHN = xmlNode2["FadeSpeed"].PNJPEDPDMCP().ParseInt();
		HPDGECMMHBJ.CPLJCIFJAGN = xmlNode2["DelayBeforeFade"].Attributes["Value"].ParseInt();
		HPDGECMMHBJ.BGDLPJKHBHP = xmlNode2["DifficultyIsFirstFrame"].Attributes["Value"].ParseInt();
		XmlNode xmlNode3 = node["RaidInfo"];
		if (xmlNode3 != null)
		{
			GMKNDIELELN.IEKAFNFKBNE = xmlNode3["MinOpacity"].PNJPEDPDMCP().ParseInt();
			GMKNDIELELN.HPJHAIALGHN = xmlNode3["FadeSpeed"].PNJPEDPDMCP().ParseInt();
			GMKNDIELELN.CPLJCIFJAGN = xmlNode3["DelayBeforeFade"].Attributes["Value"].ParseInt();
			GMKNDIELELN.LHMLMFOMALK = xmlNode3["PrizeIsFirstFrame"].Attributes["Value"].ParseInt();
		}
		XmlNode xmlNode4 = node["ZoneSwitch"];
		JHLMDGBGGEP.IEKAFNFKBNE = xmlNode4["MinOpacity"].Attributes["Value"].ParseInt();
		JHLMDGBGGEP.HPJHAIALGHN = xmlNode4["FadeSpeed"].Attributes["Value"].ParseInt();
		JHLMDGBGGEP.CPLJCIFJAGN = xmlNode4["DelayBeforeFade"].Attributes["Value"].ParseInt();
		JHLMDGBGGEP.GBDHOPBMLHK = new List<string>();
		XmlNode xmlNode5 = xmlNode4["BattleTypes"];
		foreach (XmlNode childNode in xmlNode5.ChildNodes)
		{
			string item = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			JHLMDGBGGEP.GBDHOPBMLHK.Add(item);
		}
	}
}
