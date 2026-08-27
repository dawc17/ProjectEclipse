using System.Collections.Generic;
using System.Xml;

public class ComplexRule : Rule
{
	private List<Rule> _rules = new List<Rule>();

	public ComplexRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleComplex, node)
	{
		Parse(node);
	}

	public override void SetActive(bool value)
	{
		base.SetActive(value);
		foreach (Rule item in _rules)
		{
			item.SetActive(value);
		}
	}

	public List<Rule> BONNMLEJBJH()
	{
		return _rules;
	}

	protected override void Parse(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Rule gKAJMMNJBGA = RuleParser.LBDEIDNPJMO(childNode);
			if (gKAJMMNJBGA != null)
			{
				_rules.Add(gKAJMMNJBGA);
			}
		}
	}
}
