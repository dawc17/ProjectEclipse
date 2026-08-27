using System.Xml;

public class NoPerksRule : InFightRule
{
	protected string _name = string.Empty;

	public NoPerksRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleNoPerks, EJPOJJKKICO, node)
	{
		_name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public string DMEDLGGNAIK()
	{
		return _name;
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new NoPerksRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
