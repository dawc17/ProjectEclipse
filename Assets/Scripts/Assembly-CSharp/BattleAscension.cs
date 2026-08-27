using UnityEngine;

public class BattleAscension : BattleReplayable
{
	public BattleAscension(string LFLGCDNKNJI, Vector2 MGMMDGFPBLP, string name, string ADONPNOBBDE, string LHCFHAIDNDP, string EMDJGBHIAIA, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO, string LOKLDPLAPOL, string PEMOECLNECD, string LPJNEDFCBOI, string PINIIFIOECE, string OAPKHNPPGHP, string IHBMPGKIBAN)
		: base(LFLGCDNKNJI, MGMMDGFPBLP, name, ADONPNOBBDE, LHCFHAIDNDP, EMDJGBHIAIA, CDCJKJNGPOE, MCDAHGPLLDO, LOKLDPLAPOL, PEMOECLNECD, LPJNEDFCBOI, PINIIFIOECE, OAPKHNPPGHP, IHBMPGKIBAN)
	{
	}

	public void FLMLLDJIHMD()
	{
		int num = 0;
		if (!NLLECKHLMAN)
		{
			PDFECMAJIEC();
		}
		int num2 = MNDMLMGAMPH();
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (num + 1 < num2)
			{
				item.PGBKNLAEANJ = ConditionStatus.StatusComplete;
			}
			else
			{
				item.PGBKNLAEANJ = ConditionStatus.StatusOpen;
			}
			num++;
		}
	}

	public new virtual void MJJFFAOLCCK(FightList KGKDKENMAOA)
	{
		int num = 0;
		bool flag = false;
		int num2 = ((MEOMPEEPCJJ == null) ? 1 : MEOMPEEPCJJ.PHCFNACJAAJ());
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item == KGKDKENMAOA)
			{
				flag = true;
				break;
			}
			num++;
		}
		if (flag)
		{
			if (num + 1 < num2)
			{
				KGKDKENMAOA.PGBKNLAEANJ = ConditionStatus.StatusComplete;
			}
			else
			{
				KGKDKENMAOA.PGBKNLAEANJ = ConditionStatus.StatusOpen;
			}
		}
	}

	public new virtual void PDFECMAJIEC()
	{
		base.PDFECMAJIEC();
		FLMLLDJIHMD();
	}

	public int MNDMLMGAMPH()
	{
		return (MEOMPEEPCJJ == null) ? 1 : MEOMPEEPCJJ.PHCFNACJAAJ();
	}

	public void LAGLOEEPGIO(int value)
	{
		if (MEOMPEEPCJJ != null)
		{
			MEOMPEEPCJJ.EAONJGHNJGB(value);
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	public void IDLDHJBJEII(FightList KGKDKENMAOA)
	{
		int num = EBLOIAEMCPN(KGKDKENMAOA);
		if (num >= 0)
		{
			LAGLOEEPGIO(num + 2);
		}
	}

	public int EBLOIAEMCPN(FightList KGKDKENMAOA)
	{
		int num = 0;
		bool flag = false;
		foreach (FightList item in JNPMCNMEOLE)
		{
			if (item == KGKDKENMAOA)
			{
				flag = true;
				break;
			}
			num++;
		}
		if (flag)
		{
			return num;
		}
		return -1;
	}
}
