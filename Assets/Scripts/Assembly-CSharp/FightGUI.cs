using System.Xml;

public static class FightGUI
{
	private static float FFKBDBGCCEN;

	public static float FCPEPJALAJF
	{
		get
		{
			return JIBDDCOHPCC();
		}
	}

	public static float JIBDDCOHPCC()
	{
		return FFKBDBGCCEN;
	}

	public static void Parse(XmlNode node)
	{
		PerkGUI.Parse(node["PerkIcons"]);
		FFKBDBGCCEN = node["LifeBarMin"].PNJPEDPDMCP().ParseFloat();
	}
}
