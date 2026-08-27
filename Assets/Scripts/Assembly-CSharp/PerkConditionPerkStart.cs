using System.Collections.Generic;

public class PerkConditionPerkStart : PerkCondition
{
	private string ParentPerkName;

	private bool IsPlayer;

	public PerkConditionPerkStart(string OCIEELPKJKL, bool EKBOGDKIHIH)
	{
		set_Type(NHDGLPNNNLH.CONDITION_PERK_START);
		ParentPerkName = OCIEELPKJKL;
		IsPlayer = EKBOGDKIHIH;
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		if (!JMDLAMHAJLN().ELPJBGIPEIB().EPCNJLEHJCB())
		{
			return true;
		}
		KAOPLEPILDH kAOPLEPILDH = JMDLAMHAJLN().ELPJBGIPEIB().KMMJCHDKBDO as KAOPLEPILDH;
		if (kAOPLEPILDH == null)
		{
			return true;
		}
		return PerksStage.CanUsePerk(ParentPerkName);
	}
}
