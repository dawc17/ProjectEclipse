using System.Xml;

public class ComboRule : DamageRule
{
	private int IPGFIEDBKHA;

	public ComboRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(node, EJPOJJKKICO, BCBLLMPAMLP.RuleCombo)
	{
		IPGFIEDBKHA = 0;
		KOKHKAFELGL = false;
		Parse(node);
		EBJIKKBLBEM(FightEvent.ComboEvent);
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		if (hCPJJKMNMCE.KOJNCHKPLLN == FightEvent.DamageCheckEvent)
		{
			return false;
		}
		return CheckIsNoDamageChange(hCPJJKMNMCE.currentComboLevel < IPGFIEDBKHA);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		IPGFIEDBKHA = node.Attributes["Value"].ParseInt();
	}
}
