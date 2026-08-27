using System.Collections.Generic;
using System.Xml;

public class PerkConditionBullets : PerkConditionMatchMinMax
{
	private string KCIIELDOBOM;

	public PerkConditionBullets()
	{
		set_Type(NHDGLPNNNLH.CONDITION_BULLETS);
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
		int num = 0;
		if (KCIIELDOBOM.Equals("MagicBullet"))
		{
			num = fGCODGKLHED.LPOJKGLFMAL();
		}
		else
		{
			if (!KCIIELDOBOM.Equals("RaidChargeBullet"))
			{
				return false;
			}
			num = fGCODGKLHED.CKAKLHDLHJO();
		}
		FMKBHHJDHDM.IBCPKBBAFNH();
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
