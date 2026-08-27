using System.Xml;

public class TacticRule : InFightRule
{
	private string JPGBGIMLPDN;

	public TacticRule(XmlNode node, RuleAppliance EJPOJJKKICO = RuleAppliance.ApplianceOpponent)
		: base(BCBLLMPAMLP.RuleTactic, EJPOJJKKICO, node)
	{
		Parse(node);
	}

	public string ICIKNGANCGK()
	{
		return JPGBGIMLPDN;
	}

	protected override void Parse(XmlNode node)
	{
		JPGBGIMLPDN = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new TacticRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
