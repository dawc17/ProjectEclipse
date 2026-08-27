using System.Xml;

public class DescriptionRule : Rule
{
	private string NBJPNBAGMDD = string.Empty;

	public string HBCNKNFPAIM
	{
		get
		{
			return MIDPFGENBCF();
		}
	}

	public DescriptionRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleDescription, node)
	{
		Parse(node);
	}

	public string MIDPFGENBCF()
	{
		return NBJPNBAGMDD;
	}

	protected override void Parse(XmlNode node)
	{
		NBJPNBAGMDD = node.Attributes["Alias"].CIPOICEEIBK(string.Empty);
	}
}
