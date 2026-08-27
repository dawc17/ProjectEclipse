using System.Collections.Generic;
using System.Xml;

public class RewardLottery : Rewardable
{
	private string PBLBEFIGNAG;

	public List<MANJCIGJPMK> EDCOGMLOEHE = new List<MANJCIGJPMK>();

	public RewardLottery(XmlNode node, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO)
	{
		CLOGJMBMMPI = GADCOGHCGDP.REWARD_LOTTERY;
		PBLBEFIGNAG = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "Slot")
			{
				MANJCIGJPMK item = new MANJCIGJPMK(childNode, CDCJKJNGPOE, MCDAHGPLLDO);
				EDCOGMLOEHE.Add(item);
			}
			else
			{
				if (!(childNode.Name == "Level"))
				{
					continue;
				}
				int bDJKDCMHEBI = childNode.Attributes["Min"].ParseInt(-1);
				int cIKLDJLOFDJ = childNode.Attributes["Max"].ParseInt(-1);
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					MANJCIGJPMK item2 = new MANJCIGJPMK(childNode2, CDCJKJNGPOE, MCDAHGPLLDO)
					{
						BDJKDCMHEBI = bDJKDCMHEBI,
						CIKLDJLOFDJ = cIKLDJLOFDJ
					};
					EDCOGMLOEHE.Add(item2);
				}
			}
		}
	}
}
