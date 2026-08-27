using System.Xml;

public class RaidCurrencyCostRule : CurrencyCostRule
{
	protected string GAFGMNPOEGE;

	public RaidCurrencyCostRule(XmlNode node)
		: base(node)
	{
		_type = BCBLLMPAMLP.RuleRaidCurrencyCost;
		Parse(node);
	}

	public RaidCurrencyCostRule(RaidCurrencyCostRule HNBFMAKFJAM)
		: base(HNBFMAKFJAM)
	{
		GAFGMNPOEGE = HNBFMAKFJAM.FGBHHCAGHFJ();
	}

	public string FGBHHCAGHFJ()
	{
		return GAFGMNPOEGE;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		GAFGMNPOEGE = node.Attributes["PackName"].CIPOICEEIBK(string.Empty);
	}
}
