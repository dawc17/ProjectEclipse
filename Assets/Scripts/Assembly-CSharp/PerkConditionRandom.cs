using System.Collections.Generic;
using System.Xml;

public class PerkConditionRandom : PerkConditionFunctionExtension
{
	public PerkConditionRandom()
	{
		set_Type(NHDGLPNNNLH.CONDITION_RANDOM);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		string bLLCOEAOJGF = node.Attributes["Chance"].CIPOICEEIBK(string.Empty);
		LFGMKDBLKIM.Parse(bLLCOEAOJGF);
		LFGMKDBLKIM.set_Target(this);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		base.IsEqual(ACENLMONNPA, NIKHAICFGNM);
		FunctionResult dEIHAOLOPLC = LFGMKDBLKIM.IBCPKBBAFNH();
		float num = dEIHAOLOPLC.ToFloat();
		return NekkiMath.randomChance(num * 100f);
	}
}
