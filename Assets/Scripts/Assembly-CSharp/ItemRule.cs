using System.Xml;

public class ItemRule : Rule
{
	protected UserItem PMKLKLNMEKL;

	protected bool OCLDPNBHLOL;

	private bool BNBFHIDFHHH;

	protected RuleAppliance BJHBHKKHENM;

	public UserItem DLKPBAJDHBO
	{
		get
		{
			return get_Item();
		}
	}

	public bool MFFLBFGDCIO
	{
		get
		{
			return KIGLIADCMHK();
		}
	}

	public bool CFMGGHDLAOD
	{
		get
		{
			return DCFMEDKNIDI();
		}
	}

	public ItemRule(XmlNode node, bool NPBEDEFLCAE = true)
		: base(BCBLLMPAMLP.RuleItem, node)
	{
		PMKLKLNMEKL = null;
		BNBFHIDFHHH = false;
		OCLDPNBHLOL = false;
		BJHBHKKHENM = RuleAppliance.AppliancePlayer;
		if (NPBEDEFLCAE)
		{
			JOKNKEAIIKM(node);
		}
		BNBFHIDFHHH = node.Attributes["NoAttributeChange"].ParseBool();
		MMALCMBNPOB(node);
	}

	public UserItem get_Item()
	{
		return PMKLKLNMEKL;
	}

	public bool KIGLIADCMHK()
	{
		return OCLDPNBHLOL;
	}

	public bool DCFMEDKNIDI()
	{
		return BNBFHIDFHHH;
	}

	public override bool Compare(object data)
	{
		UserItem dKCHDHMLKHN = data as UserItem;
		if (dKCHDHMLKHN == null)
		{
			return true;
		}
		ItemInfo dJKEECEOCJB = PMKLKLNMEKL.BHKHOJPANHE();
		ItemInfo dJKEECEOCJB2 = dKCHDHMLKHN.BHKHOJPANHE();
		if (dJKEECEOCJB.Name != string.Empty && dJKEECEOCJB.Name == dJKEECEOCJB2.Name)
		{
			return false;
		}
		if (dJKEECEOCJB.Type != string.Empty && dJKEECEOCJB.Type == dJKEECEOCJB2.Type)
		{
			return false;
		}
		if (dJKEECEOCJB.MDPPNGIEJGD != string.Empty && dJKEECEOCJB.MDPPNGIEJGD == dJKEECEOCJB2.MDPPNGIEJGD)
		{
			return false;
		}
		int num = ((dKCHDHMLKHN.AKKBIFEFDCI() == null) ? dJKEECEOCJB2.MHGODOLNDLE : dKCHDHMLKHN.AKKBIFEFDCI().MHGODOLNDLE);
		if (PMKLKLNMEKL.DHNNCAEEMLL() > num)
		{
			return false;
		}
		return true;
	}

	public RuleAppliance EDAKADCHOLE()
	{
		return BJHBHKKHENM;
	}

	public void MOEAPHGDNAB(RuleAppliance IGFNCCEHFEK)
	{
		BJHBHKKHENM = IGFNCCEHFEK;
	}

	protected void MMALCMBNPOB(XmlNode node)
	{
		RuleAppliance bJHBHKKHENM = RuleAppliance.AppliancePlayer;
		switch (node.Attributes["ApplyTo"].CIPOICEEIBK(string.Empty))
		{
		case "Player":
			bJHBHKKHENM = RuleAppliance.AppliancePlayer;
			break;
		case "Bot":
			bJHBHKKHENM = RuleAppliance.ApplianceOpponent;
			break;
		case "All":
			bJHBHKKHENM = RuleAppliance.ApplianceAll;
			break;
		}
		BJHBHKKHENM = bJHBHKKHENM;
	}

	protected virtual void JOKNKEAIIKM(XmlNode node)
	{
		ItemInfo dJKEECEOCJB = new ItemInfo(node);
		PMKLKLNMEKL = new UserItem(node, dJKEECEOCJB.Name, false, 1, node.Attributes["MinLevel"].ParseInt(), 0L, 0);
		PMKLKLNMEKL.KIGHKCOCJFJ(dJKEECEOCJB);
	}
}
