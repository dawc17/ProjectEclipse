using System.Xml;

public class CopyItemInfo : ItemInfo
{
	public string BLIKNEDFOFG;

	public string PCOBPICANEP;

	public CopyItemInfo(XmlNode node, string FFBGOLDLBHD = "", string IGOAPEILEFP = "")
		: base(node)
	{
		BLIKNEDFOFG = FFBGOLDLBHD;
		PCOBPICANEP = IGOAPEILEFP;
	}
}
