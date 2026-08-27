using System.Xml;

public class LifeStealRule : InFightRule
{
	private float JPOOKCGBLJO;

	private float OCGGCHDAPMI;

	public LifeStealRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleLifeSteal, EJPOJJKKICO, node)
	{
		OCGGCHDAPMI = 0f;
		EBJIKKBLBEM(FightEvent.StrikeEvent);
		Parse(node);
		Reset();
	}

	public float FGJOBADADEB()
	{
		return OCGGCHDAPMI;
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		FightEvent kOJNCHKPLLN = hCPJJKMNMCE.KOJNCHKPLLN;
		if (kOJNCHKPLLN == FightEvent.StrikeEvent)
		{
			OCGGCHDAPMI = hCPJJKMNMCE.OJIKDIDLBAF * JPOOKCGBLJO;
			return OCGGCHDAPMI != 0f;
		}
		return false;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		JPOOKCGBLJO = node.Attributes["DamagePart"].ParseFloat();
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new LifeStealRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
