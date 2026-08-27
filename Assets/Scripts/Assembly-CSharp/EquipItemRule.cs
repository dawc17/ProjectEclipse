using System.Xml;

public class EquipItemRule : ItemRule
{
	public EquipItemRule(XmlNode node)
		: base(node)
	{
		OCLDPNBHLOL = true;
	}
}
