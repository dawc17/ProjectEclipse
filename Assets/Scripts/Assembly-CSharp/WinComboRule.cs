using System.Xml;

public class WinComboRule : InFightRule
{
	private int IPGFIEDBKHA;

	public WinComboRule(XmlNode node, RuleAppliance EJPOJJKKICO)
		: base(BCBLLMPAMLP.RuleWinCombo, EJPOJJKKICO, node)
	{
		Parse(node);
		KOKHKAFELGL = false;
		EBJIKKBLBEM(FightEvent.ComboEvent);
	}

	protected override bool CompareSingle(object data)
	{
		FightData hCPJJKMNMCE = (FightData)data;
		return hCPJJKMNMCE.currentComboLevel >= IPGFIEDBKHA;
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		IPGFIEDBKHA = node.Attributes["Value"].ParseInt();
	}

	public override InFightRule Copy()
	{
		InFightRule aAJIFBJLJOA = null;
		RuleAppliance eJPOJJKKICO = EDAKADCHOLE();
		XmlNode hKPPBKPJOEO = GIFDJEEGCJI().IOJIGDNFCFL();
		aAJIFBJLJOA = new WinComboRule(hKPPBKPJOEO, eJPOJJKKICO);
		aAJIFBJLJOA.IsRandom = IsRandom;
		return aAJIFBJLJOA;
	}
}
