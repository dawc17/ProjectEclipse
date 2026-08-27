using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class PerkConditionHealth : PerkConditionMatchMinMax
{
	public PerkConditionHealth()
	{
		set_Type(NHDGLPNNNLH.CONDITION_HEALTH);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		FMKBHHJDHDM.Parse(node, this, JMDLAMHAJLN());
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			LLLOJBFMONN.Error("PerkConditionHealth::isEqual - model is null");
			return false;
		}
		FMKBHHJDHDM.IBCPKBBAFNH();
		float num = (ObscuredFloat)(fGCODGKLHED.KMMJCHDKBDO.KKMCHCNOHMB());
		if (!FMKBHHJDHDM.KEMLMMPIPGJ() && FMKBHHJDHDM.PPCEOKCAEBD() > num)
		{
			return false;
		}
		if (!FMKBHHJDHDM.HFGENILMBKK() && FMKBHHJDHDM.EFDLCJBJNPE() < num)
		{
			return false;
		}
		return true;
	}
}
