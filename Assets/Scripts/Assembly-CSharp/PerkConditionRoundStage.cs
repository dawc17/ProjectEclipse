using System.Collections.Generic;
using System.Xml;

public class PerkConditionRoundStage : PerkCondition
{
	private int JMHJDHLBHLK;

	public PerkConditionRoundStage()
	{
		set_Type(NHDGLPNNNLH.CONDITION_ROUND_STAGE);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		JMHJDHLBHLK = GetRoundStage(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		int jMHJDHLBHLK = fGCODGKLHED.JMHJDHLBHLK;
		if (JMHJDHLBHLK != 0 && JMHJDHLBHLK != jMHJDHLBHLK)
		{
			return false;
		}
		return true;
	}
}
