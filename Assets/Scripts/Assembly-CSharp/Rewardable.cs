using System.Xml;

public class Rewardable
{
	public enum GADCOGHCGDP
	{
		REWARD_NOTHING = 0,
		REWARD_ITEM = 1,
		REWARD_MONEY = 2,
		REWARD_CURRENCY = 3,
		REWARD_RESISTANCE = 4,
		REWARD_LOTTERY = 5
	}

	public GADCOGHCGDP CLOGJMBMMPI;

	public bool IDGKPLBKDIB;

	public bool GOOBKHECJIF;

	public virtual void Parse(XmlNode node)
	{
		IDGKPLBKDIB = node.Attributes["Drop"].ParseBool();
		GOOBKHECJIF = node.Attributes["ShowReward"].ParseBool();
	}
}
