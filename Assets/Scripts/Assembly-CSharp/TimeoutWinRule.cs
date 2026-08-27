using System.Xml;

public class TimeoutWinRule : InFightRule
{
	public TimeoutWinRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleTimeoutWin, EJPOJJKKICO, node)
	{
		KOKHKAFELGL = false;
		EBJIKKBLBEM(FightEvent.TimeoutEvent);
		Reset();
		NDBMMPENJNJ = RuleAppliance.AppliancePlayer;
	}

	protected override bool CompareSingle(object data)
	{
		return true;
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new TimeoutWinRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
