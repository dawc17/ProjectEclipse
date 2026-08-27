using System.Collections.Generic;
using System.Xml;

public class PerkConditionPain : PerkConditionMatchMinMax
{
	public PerkConditionPain()
	{
		set_Type(NHDGLPNNNLH.CONDITION_PAIN);
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
		float num = fGCODGKLHED.IDFIBPDPFLK();
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
