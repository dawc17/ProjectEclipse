using System.Xml;

public class WinStyleRule : InFightRule
{
	private FightStatistics.EMKEIEJMONM LPBLPMKABDP;

	public WinStyleRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleWinStyle, EJPOJJKKICO, node)
	{
		Parse(node);
		KOKHKAFELGL = false;
		EBJIKKBLBEM(FightEvent.CrazyEvent);
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		return hCPJJKMNMCE.DPBGICDNFAM >= LPBLPMKABDP;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		LPBLPMKABDP = RuleParser.KMAKHHHMGMH(node);
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new WinStyleRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
