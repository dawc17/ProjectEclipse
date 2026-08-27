using System.Xml;

public class PerkRule : InFightRule
{
	private PerkInfoItem ILJJPHHDIJI;

	public PerkRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RulePerk, EJPOJJKKICO, node)
	{
		ILJJPHHDIJI = null;
		Parse(node);
	}

	public PerkInfoItem GNIICEKAJKC()
	{
		return ILJJPHHDIJI;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		ILJJPHHDIJI = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(gOHIIMFFFJI);
		if (ILJJPHHDIJI != null)
		{
			ILJJPHHDIJI = ILJJPHHDIJI.Clone(node["Set"], node["RatingEvaluation"]);
		}
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new PerkRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
