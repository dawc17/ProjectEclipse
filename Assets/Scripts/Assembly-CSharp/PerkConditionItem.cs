using System.Collections.Generic;
using System.Xml;

public class PerkConditionItem : PerkCondition
{
	private string Name;

	private string KCIIELDOBOM;

	private string SubType;

	public PerkConditionItem()
	{
		set_Type(NHDGLPNNNLH.CONDITION_ITEM);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		KCIIELDOBOM = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		SubType = node.Attributes["Subtype"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		List<ItemInfo> list = fGCODGKLHED.Parameters.DGMDEDKLGMB();
		foreach (ItemInfo item in list)
		{
			if ((KCIIELDOBOM.Equals(string.Empty) || KCIIELDOBOM.Equals(item.Type)) && (SubType.Equals(string.Empty) || SubType.Equals(item.SubType)) && (Name.Equals(string.Empty) || Name.Equals(item.Name)))
			{
				return true;
			}
		}
		return false;
	}
}
