using System.Xml;

public class GameCurrency
{
	public enum DEFOMBPHMBP
	{
		CURRENCY_GROUP_NONE = 0,
		CURRENCY_GROUP_FORGE = 1
	}

	public DEFOMBPHMBP NBIHGGLGMCN;

	public string Name;

	public string MJBPMLCLMFN;

	public GameCurrency(XmlNode node)
	{
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		MJBPMLCLMFN = node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		string text = node.Attributes["Group"].CIPOICEEIBK(string.Empty);
		if (text == "Forge")
		{
			NBIHGGLGMCN = DEFOMBPHMBP.CURRENCY_GROUP_FORGE;
		}
		else
		{
			NBIHGGLGMCN = DEFOMBPHMBP.CURRENCY_GROUP_NONE;
		}
	}

	public GameCurrency(string PIKIACPLHJE, string NFBKDDABPOM, DEFOMBPHMBP APLILFFIMMM = DEFOMBPHMBP.CURRENCY_GROUP_NONE)
	{
		Name = PIKIACPLHJE;
		MJBPMLCLMFN = NFBKDDABPOM;
		NBIHGGLGMCN = APLILFFIMMM;
	}

	public void DHCNGGCOONP(GameCurrency MDDNHLBDJBN)
	{
		Name = MDDNHLBDJBN.Name;
		MJBPMLCLMFN = MDDNHLBDJBN.MJBPMLCLMFN;
		NBIHGGLGMCN = MDDNHLBDJBN.NBIHGGLGMCN;
	}
}
