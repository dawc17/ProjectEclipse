using System.Collections.Generic;
using System.Xml;

public class PerkConditionItem : PerkCondition
{
	private string Name;

	private string KCIIELDOBOM;

	private string MDPPNGIEJGD;

	public PerkConditionItem()
	{
		set_Type(NHDGLPNNNLH.CONDITION_ITEM);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		KCIIELDOBOM = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		MDPPNGIEJGD = node.Attributes["Subtype"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (ACENLMONNPA == null)
		{
			return false;
		}
		List<ItemInfo> list = fGCODGKLHED.KMMJCHDKBDO.DGMDEDKLGMB();
		foreach (ItemInfo item in list)
		{
			if ((KCIIELDOBOM.Equals(string.Empty) || KCIIELDOBOM.Equals(item.Type)) && (MDPPNGIEJGD.Equals(string.Empty) || MDPPNGIEJGD.Equals(item.MDPPNGIEJGD)) && (Name.Equals(string.Empty) || Name.Equals(item.Name)))
			{
				return true;
			}
		}
		return false;
	}
}
