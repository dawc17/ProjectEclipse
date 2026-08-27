using System.Collections.Generic;
using System.Xml;

public class PerkConditionModExists : PerkCondition
{
	private string Name;

	private string Namespace;

	public PerkConditionModExists()
	{
		set_Type(NHDGLPNNNLH.CONDITION_MOD_EXISTS);
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Namespace = node.Attributes["Namespace"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(Model ACENLMONNPA, List<string> NIKHAICFGNM)
	{
		if (Namespace != string.Empty && PerksStage.CheckModNameInNamespace(Name, Namespace))
		{
			return true;
		}
		Model fGCODGKLHED = EPCPGEPPHLO(ACENLMONNPA);
		if (fGCODGKLHED == null)
		{
			return false;
		}
		if (fGCODGKLHED.HasTransientPerkFlag(Name))
		{
			return true;
		}
		if (NIKHAICFGNM == null)
		{
			return false;
		}
		foreach (string item in NIKHAICFGNM)
		{
			string value = item;
			if (Name.Equals(value))
			{
				return true;
			}
		}
		return false;
	}
}
