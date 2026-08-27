using System.Collections.Generic;
using System.Xml;

public class PerkConditionCurrentAnimation : PerkConditionMatchMinMax
{
	private string Name;

	public PerkConditionCurrentAnimation()
	{
		set_Type(NHDGLPNNNLH.CONDITION_CURRENT_ANIMATION);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		FMKBHHJDHDM.Parse(node, this, JMDLAMHAJLN());
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		InfoAnimation pJAHIOELGGD = fGCODGKLHED.FHBLLPCEAHG();
		if (pJAHIOELGGD == null || !pJAHIOELGGD.CNPFHBMGDFP(Name))
		{
			return false;
		}
		FMKBHHJDHDM.IBCPKBBAFNH();
		int num = fGCODGKLHED.LPFPGDJALED();
		if (!FMKBHHJDHDM.KEMLMMPIPGJ() && FMKBHHJDHDM.PPCEOKCAEBD() > (float)num)
		{
			return false;
		}
		if (!FMKBHHJDHDM.HFGENILMBKK() && FMKBHHJDHDM.EFDLCJBJNPE() < (float)num)
		{
			return false;
		}
		return true;
	}
}
