using System.Xml;

public class PointsRule : InFightRule
{
	public enum KEJJHEAMBAP
	{
		STRIKE_ZONE_HEAD = 0,
		STRIKE_ZONE_BODY = 1,
		STRIKE_ZONE_ALL = 2
	}

	private PointsTableType FMLJFBHMJKK;

	private int HEKFOIEOBPN;

	private int DIHLAMOGKJE;

	private int KNDGDKJILNN;

	private int OIHDELFOFHF;

	private bool ECLDOOLAMPF;

	private bool CPGDLAMPMLP;

	private bool GGOMIDAKMKN;

	private bool NEGPPGMNLGG;

	private bool GMMFLGHLNPE;

	private bool ENCPNDEDFPC;

	private bool ELLKDNDKKKA;

	private KEJJHEAMBAP DHKCMCEOEOH;

	public PointsRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RulePoints, EJPOJJKKICO, node)
	{
		KNDGDKJILNN = 0;
		DIHLAMOGKJE = 0;
		ECLDOOLAMPF = false;
		DHKCMCEOEOH = KEJJHEAMBAP.STRIKE_ZONE_ALL;
		FMLJFBHMJKK = PointsTableType.POINTS_TABLE_CONTEST;
		CPGDLAMPMLP = false;
		GGOMIDAKMKN = false;
		NEGPPGMNLGG = false;
		KOKHKAFELGL = false;
		EBJIKKBLBEM(FightEvent.StrikeEvent);
		EBJIKKBLBEM(FightEvent.TimeoutEvent);
		Parse(node);
		Reset();
	}

	public override void Reset()
	{
		KNDGDKJILNN = 0;
		DIHLAMOGKJE = 0;
		ECLDOOLAMPF = false;
	}

	public override bool Compare(object data)
	{
		AGCBHKBNMKL(data);
		PlayersFightData jNGGHELCPFM = (PlayersFightData)data;
		bool result = false || IPJHOBCMOAC(jNGGHELCPFM.MPLPEMOFHGI, jNGGHELCPFM.EKBMBILHBMC, true) || IPJHOBCMOAC(jNGGHELCPFM.EKBMBILHBMC, jNGGHELCPFM.MPLPEMOFHGI, false);
		if (jNGGHELCPFM.MPLPEMOFHGI.KOJNCHKPLLN == FightEvent.TimeoutEvent || jNGGHELCPFM.EKBMBILHBMC.KOJNCHKPLLN == FightEvent.TimeoutEvent)
		{
			ECLDOOLAMPF = true;
			result = true;
		}
		return result;
	}

	public int BDHKJEFJNFJ()
	{
		return DIHLAMOGKJE;
	}

	public int MHCBPGMIEEH()
	{
		return KNDGDKJILNN;
	}

	public int OEDHHGKAMID()
	{
		return OIHDELFOFHF;
	}

	public bool FEIKKONCLFE()
	{
		return ECLDOOLAMPF;
	}

	public override void InitRule(object data)
	{
		Reset();
	}

	public override RuleAppliance IMINMDOFHMG()
	{
		switch (FMLJFBHMJKK)
		{
		case PointsTableType.POINTS_TABLE_CONTEST:
			return (DIHLAMOGKJE > KNDGDKJILNN) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent;
		case PointsTableType.POINTS_TABLE_SCORE:
			if (DIHLAMOGKJE >= OIHDELFOFHF)
			{
				return RuleAppliance.AppliancePlayer;
			}
			return RuleAppliance.ApplianceOpponent;
		default:
			return RuleAppliance.ApplianceNone;
		}
	}

	public PointsTableType GCKBDFJKPDC()
	{
		return FMLJFBHMJKK;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		string text = node.Attributes["Type"].CIPOICEEIBK("Contest");
		if (text == "Contest")
		{
			FMLJFBHMJKK = PointsTableType.POINTS_TABLE_CONTEST;
		}
		else if (text == "Score")
		{
			FMLJFBHMJKK = PointsTableType.POINTS_TABLE_SCORE;
		}
		OIHDELFOFHF = node.Attributes["Max"].ParseInt();
		HEKFOIEOBPN = node.Attributes["PointsPerHit"].ParseInt();
		GMMFLGHLNPE = !node.Attributes["Block"].Empty();
		CPGDLAMPMLP = node.Attributes["Block"].ParseBool();
		ENCPNDEDFPC = !node.Attributes["Critical"].Empty();
		GGOMIDAKMKN = node.Attributes["Critical"].ParseBool();
		ELLKDNDKKKA = !node.Attributes["Shock"].Empty();
		NEGPPGMNLGG = node.Attributes["Shock"].ParseBool();
		MPHICIMMGJB(node);
	}

	protected void MPHICIMMGJB(XmlNode node)
	{
		string text = node.Attributes["Defense"].CIPOICEEIBK(string.Empty);
		if (text == string.Empty)
		{
			DHKCMCEOEOH = KEJJHEAMBAP.STRIKE_ZONE_ALL;
		}
		else if (text == "BodyDefense")
		{
			DHKCMCEOEOH = KEJJHEAMBAP.STRIKE_ZONE_BODY;
		}
		else if (text == "HeadDefense")
		{
			DHKCMCEOEOH = KEJJHEAMBAP.STRIKE_ZONE_HEAD;
		}
	}

	protected bool CheckStrikeZone(bool BNPGBHPDGHM)
	{
		return DHKCMCEOEOH == KEJJHEAMBAP.STRIKE_ZONE_ALL || (DHKCMCEOEOH == KEJJHEAMBAP.STRIKE_ZONE_HEAD && BNPGBHPDGHM) || (DHKCMCEOEOH == KEJJHEAMBAP.STRIKE_ZONE_BODY && !BNPGBHPDGHM);
	}

	protected bool IPJHOBCMOAC(FightData MKIPNLEHIGE, FightData PHPLHIDFGMG, bool AKBKFMJLNFK)
	{
		bool flag = !GMMFLGHLNPE || MKIPNLEHIGE.FIJOEIOHJFA == CPGDLAMPMLP;
		bool flag2 = !ENCPNDEDFPC || MKIPNLEHIGE.IDAJOBOKPPP == GGOMIDAKMKN;
		bool flag3 = !ELLKDNDKKKA || PHPLHIDFGMG.OGOFFCEGLHJ == NEGPPGMNLGG;
		if (MKIPNLEHIGE.KOJNCHKPLLN == FightEvent.StrikeEvent && MKIPNLEHIGE.ONBMPLCEONN && flag && CheckStrikeZone(MKIPNLEHIGE.BNPGBHPDGHM) && flag2 && flag3)
		{
			if (AKBKFMJLNFK)
			{
				DIHLAMOGKJE++;
			}
			else
			{
				KNDGDKJILNN++;
			}
			if (FMLJFBHMJKK == PointsTableType.POINTS_TABLE_SCORE && ((AKBKFMJLNFK && DIHLAMOGKJE >= OIHDELFOFHF) || (!AKBKFMJLNFK && KNDGDKJILNN >= OIHDELFOFHF)))
			{
				ECLDOOLAMPF = true;
			}
			return true;
		}
		return false;
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new PointsRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
