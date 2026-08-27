using System.Collections.Generic;
using System.Xml;

public class PerkConditionRound : PerkConditionFunctionExtension
{
	public PerkConditionRound()
	{
		set_Type(NHDGLPNNNLH.CONDITION_ROUND);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		string bLLCOEAOJGF = node.Attributes["Number"].CIPOICEEIBK(string.Empty);
		LFGMKDBLKIM.Parse(bLLCOEAOJGF);
		LFGMKDBLKIM.set_Target(this);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		FunctionResult dEIHAOLOPLC = LFGMKDBLKIM.IBCPKBBAFNH();
		int num = dEIHAOLOPLC.DCJLKCFKCOM.ToInt();
		int num2 = fGCODGKLHED.DKDGOOLAAKN();
		if (num2 != num)
		{
			return false;
		}
		return true;
	}
}
