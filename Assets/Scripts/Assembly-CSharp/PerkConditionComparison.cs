using System.Collections.Generic;
using System.Text;
using System.Xml;

public class PerkConditionComparison : PerkConditionFunctionExtension
{
	public PerkConditionComparison()
	{
		set_Type(NHDGLPNNNLH.CONDITION_PERK_COMPARISON);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("?Compare[");
		stringBuilder.Append(node.Attributes["Value1"].CIPOICEEIBK(string.Empty));
		stringBuilder.Append(",");
		stringBuilder.Append(node.Attributes["Value2"].CIPOICEEIBK(string.Empty));
		stringBuilder.Append(",");
		stringBuilder.Append(node.Name);
		stringBuilder.Append("]");
		LFGMKDBLKIM.Parse(stringBuilder.ToString());
		LFGMKDBLKIM.set_Target(this);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		base.IsEqual(ACENLMONNPA, NIKHAICFGNM);
		FunctionResult dEIHAOLOPLC = LFGMKDBLKIM.IBCPKBBAFNH();
		int num = dEIHAOLOPLC.ToInt();
		return num > 0;
	}
}
