using System.Xml;

public class NoBulletsReplenishmentRule : InFightRule
{
	public NoBulletsReplenishmentRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleNoBulletsReplenishment, EJPOJJKKICO, node)
	{
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new NoBulletsReplenishmentRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
