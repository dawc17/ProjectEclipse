using System.Collections.Generic;
using System.Xml;

public class WarriorAttributes
{
	public Dictionary<string, WarriorAttribute> IOEOOBBJJIO = new Dictionary<string, WarriorAttribute>();

	public List<WarriorAttribute> IBLHIAHECLK = new List<WarriorAttribute>();

	public void Parse(XmlNode node)
	{
		IBLHIAHECLK.Clear();
		IOEOOBBJJIO.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			WarriorAttribute bCNOAOPGAEI = new WarriorAttribute();
			bCNOAOPGAEI.set_Name(XmlUtils.ParseString(childNode.Attributes["Name"]));
			bCNOAOPGAEI.MJBPMLCLMFN = XmlUtils.ParseString(childNode.Attributes["Icon"]);
			bCNOAOPGAEI.HBCNKNFPAIM = XmlUtils.ParseString(childNode.Attributes["Alias"]);
			bCNOAOPGAEI.Point = XmlUtils.ParseInt(childNode.Attributes["Point"]);
			bCNOAOPGAEI.CGNPILCDCCF = XmlUtils.ParseString(childNode.Attributes["Format"]);
			bCNOAOPGAEI.GDCBBAHKCIE = XmlUtils.ParseBool(childNode.Attributes["Hidden"]);
			bCNOAOPGAEI.GDECIAJAFHH = XmlUtils.ParseBool(childNode.Attributes["ShopHidden"]);
			bCNOAOPGAEI.KDKHPMHNPCN = XmlUtils.ParseBool(childNode.Attributes["ProfileHidden"]);
			bCNOAOPGAEI.HCCKLLOEPJN = XmlUtils.ParseString(childNode.Attributes["BarScale"]);
			if (bCNOAOPGAEI.CGNPILCDCCF == "Percent")
			{
				bCNOAOPGAEI.CGNPILCDCCF = "%";
			}
			IBLHIAHECLK.Add(bCNOAOPGAEI);
			IOEOOBBJJIO.Add(bCNOAOPGAEI.get_Name(), bCNOAOPGAEI);
		}
	}

	public WarriorAttribute NGNDIGFKKHE(string name)
	{
		return (!IOEOOBBJJIO.ContainsKey(name)) ? null : IOEOOBBJJIO[name];
	}
}
