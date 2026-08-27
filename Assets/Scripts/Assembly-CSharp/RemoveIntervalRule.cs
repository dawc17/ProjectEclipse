using System.Xml;

public class RemoveIntervalRule : InFightRule
{
	private IntervalAnimation.NGAJJDIEDGF MMNMEBICHMH;

	public RemoveIntervalRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleRemoveInterval, EJPOJJKKICO, node)
	{
		MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
		Parse(node);
	}

	protected override bool CompareSingle(object data)
	{
		return false;
	}

	public override void InitRule(object data)
	{
		RuleInitData oIFPCFEGFOB = (RuleInitData)data;
		switch (NDBMMPENJNJ)
		{
		case RuleAppliance.AppliancePlayer:
			oIFPCFEGFOB.DLPKDAIDCBF.PONNDMHBGJK(MMNMEBICHMH);
			break;
		case RuleAppliance.ApplianceOpponent:
			oIFPCFEGFOB.OGBHDKKOIGH.PONNDMHBGJK(MMNMEBICHMH);
			break;
		default:
			LLLOJBFMONN.Error("RemoveIntervalRule::initRule - wrong player appliance - %i", NDBMMPENJNJ);
			break;
		}
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		switch (node.Attributes["Type"].CIPOICEEIBK(string.Empty))
		{
		case "Attack":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK;
			break;
		case "Block":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK;
			break;
		case "Invulnerable":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE;
			break;
		case "None":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
			break;
		case "SelfUninterrupt":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_SELF_UNINTERRUPT;
			break;
		case "Uninterrupt":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNINTERRUPT;
			break;
		case "Unstable":
			MMNMEBICHMH = IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNSTABLE;
			break;
		}
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new RemoveIntervalRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
