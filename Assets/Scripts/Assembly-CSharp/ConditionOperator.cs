using System.Collections.Generic;
using System.Xml;

public class ConditionOperator : ConditionCounter
{
	public enum EENJGHHIHIH
	{
		TYPE_NONE = 0,
		TYPE_OR = 1,
		TYPE_AND = 2
	}

	public EENJGHHIHIH Type;

	private List<ConditionCounter> _conditions = new List<ConditionCounter>();

	public ConditionOperator()
		: base(FELOFIAKFCO.OPERATOR)
	{
		Type = EENJGHHIHIH.TYPE_AND;
	}

	public ConditionOperator(XmlNode node)
		: base(FELOFIAKFCO.OPERATOR)
	{
		Parse(node);
	}

	public override bool IsEqual(CounterConditions conditions)
	{
		foreach (ConditionCounter item in _conditions)
		{
			bool flag = item.IsEqual(conditions);
			if (Type == EENJGHHIHIH.TYPE_AND && !flag)
			{
				return IsNotCompare(false);
			}
			if (Type == EENJGHHIHIH.TYPE_OR && flag)
			{
				return IsNotCompare(true);
			}
		}
		if (Type == EENJGHHIHIH.TYPE_AND)
		{
			return IsNotCompare(true);
		}
		if (Type == EENJGHHIHIH.TYPE_OR)
		{
			return IsNotCompare(false);
		}
		LLLOJBFMONN.Error(string.Format("ConditionOperator::isEqual - wrong type: %i", Type));
		return false;
	}

	public void DIJNEIJHDIN(XmlNode EBLIGDMALEA)
	{
		_conditions.Clear();
		foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
		{
			ConditionCounter kAJIECHJBNL = CounterConditionsParser.DKPIKJMJPPH(childNode);
			if (kAJIECHJBNL != null)
			{
				_conditions.Add(kAJIECHJBNL);
			}
		}
	}

	public override void AEPHNNABOEK()
	{
		foreach (ConditionCounter item in _conditions)
		{
			item.AEPHNNABOEK();
		}
	}

	public void BFPIIJDAEME(ConditionCounter EPJGLECOIBG)
	{
		_conditions.Add(EPJGLECOIBG);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		string text = node.Attributes["Type"].CIPOICEEIBK();
		if (text == "OR")
		{
			Type = EENJGHHIHIH.TYPE_OR;
		}
		else if (text == "AND")
		{
			Type = EENJGHHIHIH.TYPE_AND;
		}
		else
		{
			LLLOJBFMONN.Error("ConditionOperator::ConditionOperator - wrong type: %s", text);
			Type = EENJGHHIHIH.TYPE_NONE;
		}
		DIJNEIJHDIN(node);
	}
}
