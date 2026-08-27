using System.Xml;

public class ChangeFightRule : Rule
{
	private int DKIGKNAHBFI;

	private int CKOAOOINLHF;

	public ChangeFightRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleChangeFight, node)
	{
		Parse(node);
	}

	public int NNMOHPAAFGI()
	{
		return DKIGKNAHBFI;
	}

	public int IBHBDDFGEDN()
	{
		return CKOAOOINLHF;
	}

	protected override void Parse(XmlNode node)
	{
		DKIGKNAHBFI = node.Attributes["Rounds"].ParseInt(-1);
		CKOAOOINLHF = node.Attributes["RoundTime"].ParseInt(-1);
	}
}
