using System.Xml;

public class RandomAquiredItemRule : ItemRule
{
	private string OLAAAIPEBBF;

	public RandomAquiredItemRule(XmlNode node)
		: base(node, false)
	{
		_type = BCBLLMPAMLP.RuleRandomAquiredItem;
		OLAAAIPEBBF = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		PINICFPAOAK();
	}

	public void PINICFPAOAK()
	{
		PMKLKLNMEKL = ListSF.CCDKHLAMKKO().KHCNHPCPFII().PKKKAFIHHMI(OLAAAIPEBBF);
	}

	protected virtual void JOKNKEAIIKM()
	{
	}
}
