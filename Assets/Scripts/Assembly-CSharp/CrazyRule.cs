using System.Xml;

public class CrazyRule : DamageRule
{
	private FightStatistics.EMKEIEJMONM HDNAANHGIDN;

	public CrazyRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(node, EJPOJJKKICO, BCBLLMPAMLP.RuleCrazy)
	{
		HDNAANHGIDN = FightStatistics.EMKEIEJMONM.STYLE_TURTLE;
		KOKHKAFELGL = false;
		Parse(node);
		EBJIKKBLBEM(FightEvent.CrazyEvent);
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		if (hCPJJKMNMCE.KOJNCHKPLLN == FightEvent.DamageCheckEvent)
		{
			return false;
		}
		return CheckIsNoDamageChange(hCPJJKMNMCE.DPBGICDNFAM < HDNAANHGIDN);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		HDNAANHGIDN = RuleParser.KMAKHHHMGMH(node);
	}
}
