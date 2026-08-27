using System.Collections.Generic;

public class PerkConditionInTheArea : PerkCondition
{
	public PerkConditionInTheArea()
	{
		set_Type(NHDGLPNNNLH.CONDITION_IN_THE_AREA);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		return fGCODGKLHED.MBCLINNCNAL();
	}
}
