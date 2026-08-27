using System.Collections.Generic;
using System.Xml;

public class RewardChoice
{
	public struct GIMEDBLIEFF
	{
		public Rewardable PACEDHFLGHK;

		public float PIHKGGPCADE;

		public GIMEDBLIEFF(XmlNode node)
		{
			PIHKGGPCADE = node.Attributes["Weight"].ParseFloat(1f);
			switch (node.Name)
			{
			case "Item":
				PACEDHFLGHK = new RewardItem(node);
				break;
			case "Money":
				PACEDHFLGHK = new RewardMoney(node);
				break;
			case "Currency":
				PACEDHFLGHK = new RewardCurrency(node);
				break;
			case "Resistance":
				PACEDHFLGHK = new RewardResistance(node);
				break;
			case "Lottery":
				PACEDHFLGHK = new RewardLottery(node, 0, 0);
				break;
			default:
				PACEDHFLGHK = null;
				break;
			}
		}
	}

	private List<GIMEDBLIEFF> GKOHOFKFDFP;

	public RewardChoice(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			GIMEDBLIEFF item = new GIMEDBLIEFF(childNode);
			GKOHOFKFDFP.Add(item);
		}
	}

	public Rewardable OOOBLJIHBEP()
	{
		float num = 0f;
		List<float> list = new List<float>();
		list.Add(0f);
		foreach (GIMEDBLIEFF item in GKOHOFKFDFP)
		{
			num += item.PIHKGGPCADE;
			list.Add(num);
		}
		int index = 0;
		float num2 = NekkiMath.randomFloat(0f, num);
		int i = 0;
		for (int num3 = list.Count - 1; i < num3; i++)
		{
			if (list[i] <= num2 && list[i + 1] >= num2)
			{
				index = i;
				break;
			}
		}
		return GKOHOFKFDFP[index].PACEDHFLGHK;
	}
}
