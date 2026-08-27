using System.Xml;

public class RewardResistance : Rewardable
{
	public string Name = string.Empty;

	public int Value;

	public RewardResistance(XmlNode node)
	{
		Parse(node);
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_RESISTANCE;
		Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		Value = node.Attributes["Value"].ParseInt();
	}
}
