using System.Collections.Generic;
using System.Xml;

public class PerkConditionOperator : PerkCondition
{
	private enum CKKNDJBBFIM
	{
		OPERATOR_NONE = 0,
		OPERATOR_OR = 1,
		OPERATOR_AND = 2
	}

	private CKKNDJBBFIM JJOLMNHMODH;

	private List<PerkCondition> JIFAHHGNPFH = new List<PerkCondition>();

	public PerkConditionOperator()
	{
		set_Type(NHDGLPNNNLH.CONDITION_OPERATOR);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		JJOLMNHMODH = CKKNDJBBFIM.OPERATOR_NONE;
		string text = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		if (text.Equals("Or"))
		{
			JJOLMNHMODH = CKKNDJBBFIM.OPERATOR_OR;
		}
		else if (text.Equals("And"))
		{
			JJOLMNHMODH = CKKNDJBBFIM.OPERATOR_AND;
		}
		JIFAHHGNPFH = PerkCondition.Create(node, JMDLAMHAJLN());
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		foreach (PerkCondition item in JIFAHHGNPFH)
		{
			bool flag = item.IsEqual(ACENLMONNPA, NIKHAICFGNM);
			bool flag2 = ((!item.IsNot) ? flag : (!flag));
			if (JJOLMNHMODH == CKKNDJBBFIM.OPERATOR_AND && !flag2)
			{
				return false;
			}
			if (JJOLMNHMODH == CKKNDJBBFIM.OPERATOR_OR && flag2)
			{
				return true;
			}
		}
		return JJOLMNHMODH != CKKNDJBBFIM.OPERATOR_OR;
	}
}
