using System.Collections.Generic;
using System.Xml;

public class PerkConditionCurrentInterval : PerkCondition
{
	private string CCFKABDNCLA;

	private string MOLEHILDAGP;

	public PerkConditionCurrentInterval()
	{
		set_Type(NHDGLPNNNLH.CONDITION_CURRENT_INTERVAL);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		CCFKABDNCLA = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		MOLEHILDAGP = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		List<IntervalAnimation> list = fGCODGKLHED.KPJAEBBJFEO();
		IntervalAnimation.NGAJJDIEDGF nGAJJDIEDGF = IntervalAnimation.LAJMDAFFPJE(MOLEHILDAGP);
		bool flag = CCFKABDNCLA.Equals(string.Empty);
		bool flag2 = nGAJJDIEDGF == IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
		foreach (IntervalAnimation item in list)
		{
			if (!flag && CCFKABDNCLA.Equals(item.Name))
			{
				flag = true;
			}
			if (!flag2 && nGAJJDIEDGF == item.Type)
			{
				flag2 = true;
			}
		}
		return flag && flag2;
	}
}
