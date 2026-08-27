using System.Collections.Generic;
using System.Xml;

public class PerkConditionStyle : PerkConditionMatchMinMax
{
	public PerkConditionStyle()
	{
		set_Type(NHDGLPNNNLH.CONDITION_STYLE);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		int num = CJAGHBEHFIL(node.Attributes["Min"].CIPOICEEIBK(string.Empty));
		int num2 = CJAGHBEHFIL(node.Attributes["Max"].CIPOICEEIBK(string.Empty));
		FMKBHHJDHDM.IBCPKBBAFNH();
		if (num != -1)
		{
			FMKBHHJDHDM.KPPNJLNHGME(num);
			FMKBHHJDHDM.DGGMKIGCGLI(false);
		}
		if (num2 != -1)
		{
			FMKBHHJDHDM.BIPMDHGOMBG(num2);
			FMKBHHJDHDM.ENIOHPINGMP(false);
		}
	}

	private int CJAGHBEHFIL(string name)
	{
		switch (name)
		{
		case "Turtle":
			return 0;
		case "Hard":
			return 1;
		case "Brutal":
			return 2;
		case "Aggressive":
			return 3;
		case "Crazy":
			return 4;
		case "Fantastic":
			return 5;
		default:
			return -1;
		}
	}

	public override bool IsEqual(Model GIAMLEDNFJD, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(GIAMLEDNFJD);
		if (fGCODGKLHED == null)
		{
			return false;
		}
		int pACHBHGEIGN = fGCODGKLHED.PACHBHGEIGN;
		if (!FMKBHHJDHDM.KEMLMMPIPGJ() && (int)FMKBHHJDHDM.PPCEOKCAEBD() > pACHBHGEIGN)
		{
			return false;
		}
		if (!FMKBHHJDHDM.HFGENILMBKK() && (int)FMKBHHJDHDM.EFDLCJBJNPE() < pACHBHGEIGN)
		{
			return false;
		}
		return true;
	}
}
