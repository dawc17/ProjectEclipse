using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BattleRaid : Battle
{
	public BattleRaid(string LFLGCDNKNJI, Vector2 MGMMDGFPBLP, string name, string ADONPNOBBDE, string LHCFHAIDNDP, string EMDJGBHIAIA, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO, string LOKLDPLAPOL, string PEMOECLNECD, string LPJNEDFCBOI, string PINIIFIOECE, string OAPKHNPPGHP, string IHBMPGKIBAN)
		: base(LFLGCDNKNJI, MGMMDGFPBLP, name, ADONPNOBBDE, LHCFHAIDNDP, EMDJGBHIAIA, CDCJKJNGPOE, MCDAHGPLLDO, LOKLDPLAPOL, PEMOECLNECD, LPJNEDFCBOI, PINIIFIOECE, OAPKHNPPGHP, IHBMPGKIBAN)
	{
	}

	public void Parse(XmlNode node)
	{
		XmlNode hKPPBKPJOEO = node["RaidData"];
		JNMILPCDAFM(hKPPBKPJOEO);
	}

	public bool DJCDFEAMPDA(FightList KGKDKENMAOA)
	{
		List<CurrencyCostRule> list = KGKDKENMAOA.LBGNOMEFLBA();
		if (list.Count == 0)
		{
			return true;
		}
		foreach (CurrencyCostRule item in list)
		{
			string text = item.JFDCHNBPPNH();
			if (!(text == string.Empty))
			{
				continue;
			}
			int num = 0;
			int num2 = 0;
			foreach (CurrencyCostRule item2 in list)
			{
				if (item2.JFDCHNBPPNH() == text)
				{
					int num3 = item2.LHNHLANLHMN();
					num2 += num3;
				}
			}
			if (num < num2)
			{
				return false;
			}
		}
		return true;
	}

	private void JNMILPCDAFM(XmlNode node)
	{
	}
}
