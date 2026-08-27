using System.Xml;

public class ResistanceRule : InFightRule
{
	private string _resistanceName;

	private int _resistanceValue;

	public ResistanceRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleResistance, RuleAppliance.ApplianceAll, node)
	{
		Parse(node);
		EBJIKKBLBEM(FightEvent.ResistanceCheckEvent);
	}

	public string DJBFLJAIKLI()
	{
		return _resistanceName;
	}

	public int GLBEGDFMDBO()
	{
		return _resistanceValue;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_resistanceName = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		_resistanceValue = node.Attributes["Value"].ParseInt();
		if (_resistanceValue < 0)
		{
			_resistanceValue = 0;
		}
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new ResistanceRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
