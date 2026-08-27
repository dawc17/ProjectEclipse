using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class RewardPrize
{
	public int number;

	public ObscuredLong GBGNFPNCGED = (ObscuredLong)(0L);

	public ObscuredLong PNDAIFALIKF = (ObscuredLong)(0L);

	public ObscuredUInt exp = (ObscuredUInt)(0u);

	public ObscuredFloat prizeBase = (ObscuredFloat)(-1f);

	public List<RewardItem> HELFDCAIJNE = new List<RewardItem>();

	public List<RewardMoney> MDJFGLELOBA = new List<RewardMoney>();

	public List<RewardCurrency> KIMJGOHCCPO = new List<RewardCurrency>();

	public List<RewardResistance> KBMDJACLAOH = new List<RewardResistance>();

	public List<RewardChoice> PNFMKMLLFHK = new List<RewardChoice>();

	public RewardLottery FAPDEKOMOGH;

	public bool IsCloned;

	public void Parse(XmlNode node, ushort CDCJKJNGPOE = 0, ushort MCDAHGPLLDO = 0)
	{
		number = node.Attributes["Number"].ParseInt();
		GBGNFPNCGED = (ObscuredLong)(node.Attributes["Money"].ParseLong(0L) * (long)Mathf.Pow(10f, (int)CDCJKJNGPOE));
		PNDAIFALIKF = (ObscuredLong)(node.Attributes["Bonus"].ParseLong(0L));
		exp = (ObscuredUInt)(node.Attributes["Exp"].ParseUint());
		prizeBase = (ObscuredFloat)(node.Attributes["PrizeBase"].ParseFloat(-1f));
		if ((ObscuredFloat)(prizeBase) != -1f)
		{
			prizeBase = (ObscuredFloat)((ObscuredFloat)(prizeBase) * Mathf.Pow(10f, (int)MCDAHGPLLDO));
		}
		foreach (XmlNode item6 in node.SelectNodes("Money"))
		{
			RewardMoney item = new RewardMoney(item6);
			MDJFGLELOBA.Add(item);
		}
		foreach (XmlNode item7 in node.SelectNodes("Currency"))
		{
			RewardCurrency item2 = new RewardCurrency(item7);
			KIMJGOHCCPO.Add(item2);
		}
		foreach (XmlNode item8 in node.SelectNodes("Resistance"))
		{
			RewardResistance item3 = new RewardResistance(item8);
			KBMDJACLAOH.Add(item3);
		}
		foreach (XmlNode item9 in node.SelectNodes("Lottery"))
		{
			RewardLottery fAPDEKOMOGH = new RewardLottery(item9, CDCJKJNGPOE, MCDAHGPLLDO);
			if (FAPDEKOMOGH == null)
			{
				FAPDEKOMOGH = fAPDEKOMOGH;
			}
		}
		foreach (XmlNode item10 in node.SelectNodes("Item"))
		{
			RewardItem item4 = new RewardItem(item10);
			HELFDCAIJNE.Add(item4);
		}
		foreach (XmlNode item11 in node.SelectNodes("Choice"))
		{
			RewardChoice item5 = new RewardChoice(item11);
			PNFMKMLLFHK.Add(item5);
		}
	}

	public void HNJGHOKCDJF(RewardPrize DPIIJICBGGA)
	{
		PNDAIFALIKF = (ObscuredLong)((ObscuredLong)(PNDAIFALIKF) + (ObscuredLong)(DPIIJICBGGA.PNDAIFALIKF));
		GBGNFPNCGED = (ObscuredLong)((ObscuredLong)(GBGNFPNCGED) + (ObscuredLong)(DPIIJICBGGA.GBGNFPNCGED));
		exp = (ObscuredUInt)((ObscuredUInt)(exp) + (ObscuredUInt)(DPIIJICBGGA.exp));
		if ((ObscuredFloat)(prizeBase) < 0f)
		{
			prizeBase = DPIIJICBGGA.prizeBase;
		}
		else if ((ObscuredFloat)(DPIIJICBGGA.prizeBase) > 0f)
		{
			prizeBase = (ObscuredFloat)((ObscuredFloat)(prizeBase) + (ObscuredFloat)(DPIIJICBGGA.prizeBase));
		}
		HELFDCAIJNE.AddRange(DPIIJICBGGA.HELFDCAIJNE);
		MDJFGLELOBA.AddRange(DPIIJICBGGA.MDJFGLELOBA);
		KIMJGOHCCPO.AddRange(DPIIJICBGGA.KIMJGOHCCPO);
		KBMDJACLAOH.AddRange(DPIIJICBGGA.KBMDJACLAOH);
		PNFMKMLLFHK.AddRange(DPIIJICBGGA.PNFMKMLLFHK);
		if (FAPDEKOMOGH != null)
		{
			FAPDEKOMOGH.EDCOGMLOEHE.AddRange(DPIIJICBGGA.FAPDEKOMOGH.EDCOGMLOEHE);
		}
		else
		{
			FAPDEKOMOGH = DPIIJICBGGA.FAPDEKOMOGH;
		}
	}

	public void RandomizeObscuredVars()
	{
		GBGNFPNCGED.GMCADPGOCHM();
		PNDAIFALIKF.GMCADPGOCHM();
		exp.GMCADPGOCHM();
		prizeBase.GMCADPGOCHM();
	}
}
