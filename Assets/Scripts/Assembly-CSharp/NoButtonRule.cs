using System.Xml;

public class NoButtonRule : Rule
{
	public enum AHIDMNNEAEC
	{
		ButtonTypePunch = 0,
		ButtonTypeKick = 1,
		ButtonTypeRanged = 2,
		ButtonTypeMagic = 3,
		ButtonTypeRaidCharge = 4,
		ButtonTypeDefault = 5
	}

	private AHIDMNNEAEC EPICCMPJNOL;

	public NoButtonRule(XmlNode node)
		: base(BCBLLMPAMLP.RuleNoButton, node)
	{
		EPICCMPJNOL = NHLDFLGFPPI(node);
	}

	public AHIDMNNEAEC KBINIBAGEFM()
	{
		return EPICCMPJNOL;
	}

	private AHIDMNNEAEC NHLDFLGFPPI(XmlNode node)
	{
		string text = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		if (text != string.Empty)
		{
			switch (text)
			{
			case "Punch":
				return AHIDMNNEAEC.ButtonTypePunch;
			case "Kick":
				return AHIDMNNEAEC.ButtonTypeKick;
			case "Ranged":
				return AHIDMNNEAEC.ButtonTypeRanged;
			case "Magic":
				return AHIDMNNEAEC.ButtonTypeMagic;
			case "RaidCharge":
				return AHIDMNNEAEC.ButtonTypeRaidCharge;
			}
		}
		LLLOJBFMONN.Error("Error - parseButtonType - unknown type");
		return AHIDMNNEAEC.ButtonTypeDefault;
	}
}
