using System.Collections.Generic;
using System.Xml;

public class PerkConditionMagicCharge : PerkConditionMatchMinMax
{
	private string KCIIELDOBOM;

	public PerkConditionMagicCharge()
	{
		set_Type(NHDGLPNNNLH.CONDITION_MAGIC_CHARGE);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		FMKBHHJDHDM.Parse(node, this, JMDLAMHAJLN());
		KCIIELDOBOM = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		FMKBHHJDHDM.IBCPKBBAFNH();
		float num = fGCODGKLHED.EKAFGLHNMCN();
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
