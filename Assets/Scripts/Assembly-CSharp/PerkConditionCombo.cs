using System.Collections.Generic;
using System.Xml;

public class PerkConditionCombo : PerkConditionMatchMinMax
{
	public PerkConditionCombo()
	{
		set_Type(NHDGLPNNNLH.CONDITION_COMBO);
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
			return false;
		}
		FMKBHHJDHDM.IBCPKBBAFNH();
		int num = fGCODGKLHED.NPDOLGNNINO();
		if (!FMKBHHJDHDM.KEMLMMPIPGJ() && (int)FMKBHHJDHDM.PPCEOKCAEBD() > num)
		{
			return false;
		}
		if (!FMKBHHJDHDM.HFGENILMBKK() && (int)FMKBHHJDHDM.EFDLCJBJNPE() < num)
		{
			return false;
		}
		return true;
	}
}
