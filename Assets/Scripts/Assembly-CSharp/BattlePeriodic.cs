using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using Nekki.Utils;
using UnityEngine;

public class BattlePeriodic : Battle
{
	protected static List<BattlePeriodic> _battles = new List<BattlePeriodic>();

	private static long HJOHKOEICAP = 0L;

	private static long COMLGDDIFNA = 0L;

	protected static long DEJHGDMHGAA;

	public static long Time
	{
		get
		{
			return CCCIFDLEMPI();
		}
	}

	public static long RepeatTime
	{
		get
		{
			return IDGBNPFIDGC();
		}
	}

	public BattlePeriodic(string LFLGCDNKNJI, Vector2 MGMMDGFPBLP, string name, string ADONPNOBBDE, string LHCFHAIDNDP, string EMDJGBHIAIA, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO, string LOKLDPLAPOL, string PEMOECLNECD, string LPJNEDFCBOI, string PINIIFIOECE, string OAPKHNPPGHP, string IHBMPGKIBAN)
		: base(LFLGCDNKNJI, MGMMDGFPBLP, name, ADONPNOBBDE, LHCFHAIDNDP, EMDJGBHIAIA, CDCJKJNGPOE, MCDAHGPLLDO, LOKLDPLAPOL, PEMOECLNECD, LPJNEDFCBOI, PINIIFIOECE, OAPKHNPPGHP, IHBMPGKIBAN)
	{
		_battles.Add(this);
	}

	public static long CCCIFDLEMPI()
	{
		return HJOHKOEICAP;
	}

	public override void SetTime(long value)
	{
		HJOHKOEICAP = ((DEJHGDMHGAA > 0) ? (value - DEJHGDMHGAA) : (-1));
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.SetTime(value);
		}
	}

	public static long IDGBNPFIDGC()
	{
		return COMLGDDIFNA;
	}

	protected void POBDNJJCDLI(long time)
	{
		FightList jDIPBIHBGPF = FBFHBKPFLJC();
		if (jDIPBIHBGPF == null)
		{
			Reset();
			if (JNPMCNMEOLE.Count > 0)
			{
				jDIPBIHBGPF = JNPMCNMEOLE[0];
			}
		}
		if (jDIPBIHBGPF != null)
		{
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			RosterFight pIGKOIFBOME = jDIPBIHBGPF.FLKFFDLLBKA();
			if (pIGKOIFBOME == null)
			{
				pIGKOIFBOME = nKGLHEGIKKP.OBAFPDGJHNN(jDIPBIHBGPF.BCKFACGMOKC);
				jDIPBIHBGPF.HOCFLEMFFKC(pIGKOIFBOME);
			}
			pIGKOIFBOME.CKJFJFPBIFF(time);
			// The map's availability check reads elapsed runtime state, while the
			// line above only persists the completion timestamp. Keep both in sync
			// so a finished duel locks and displays its timer immediately.
			pIGKOIFBOME.ABIELBGOLCA(time);
			ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		}
	}

	protected void ResetSingle(bool IKINMKHLDIB = true)
	{
		long num = 0L;
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.PGBKNLAEANJ = ConditionStatus.StatusOpen;
			RosterFight pIGKOIFBOME = item.FLKFFDLLBKA();
			if (pIGKOIFBOME != null && IKINMKHLDIB && pIGKOIFBOME.ILBNPNIPEHO() > num)
			{
				num = pIGKOIFBOME.ILBNPNIPEHO();
			}
		}
		foreach (FightList item2 in JNPMCNMEOLE)
		{
			RosterFight pIGKOIFBOME2 = item2.FLKFFDLLBKA();
			if (pIGKOIFBOME2 != null)
			{
				pIGKOIFBOME2.CKJFJFPBIFF(num);
				pIGKOIFBOME2.NAAHEPJIFAD(0L);
			}
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	protected void FBCMLKCKOEB(long time)
	{
		COMLGDDIFNA = time;
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.RepeatTime = time;
		}
	}

	public override void AJKBFMLOCOF(FightList KGKDKENMAOA, int index)
	{
		base.AJKBFMLOCOF(KGKDKENMAOA, index);
		SetTime(GlobalTimer.get_GetTime());
	}

	public override void JLPMOKPFECK(long time)
	{
		POBDNJJCDLI(time);
		foreach (BattlePeriodic item in _battles)
		{
			if (item != this)
			{
				item.POBDNJJCDLI(time);
			}
		}
		DEJHGDMHGAA = time;
		ListSF.CCDKHLAMKKO().DEPJCHIFFKA(time);
	}

	public override void EMFABIGKAHC(FightList KGKDKENMAOA, bool FFIBGBMOMPD)
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
		if (!flag)
		{
			LLLOJBFMONN.Error("BattleDaily::setRosterFight ERROR - no fightList found in _fights");
		}
		else
		{
			KGKDKENMAOA.HOCFLEMFFKC(ListSF.IKHJKHMIPEP(KGKDKENMAOA, FFIBGBMOMPD));
		}
	}

	public static void Reset(bool IKINMKHLDIB = true)
	{
		foreach (BattlePeriodic item in _battles)
		{
			item.ResetSingle(IKINMKHLDIB);
			item.SetTime(GlobalTimer.get_GetTime());
		}
		ListSF.CCDKHLAMKKO().DEPJCHIFFKA(0L);
		MapScene current2 = Scene<MapScene>.get_Current();
		if (current2 != null)
		{
			current2.UpdateInfoBattle();
		}
	}

	public static void EEDCDDDNLIH(int BLGLACLODID, long ICBOBIILOFE)
	{
		DEJHGDMHGAA = ICBOBIILOFE;
		bool flag = false;
		foreach (BattlePeriodic item in _battles)
		{
			item.POBDNJJCDLI(ICBOBIILOFE);
			if (GameUtils.GKOEGHLGPPE)
			{
				item.FBCMLKCKOEB(GameUtils.DailyDebugTime);
			}
			if (!flag && item.KCIKELGFHOA() > 0)
			{
				COMLGDDIFNA = item.OAJCBGAKHJJ(0).RepeatTime;
				flag = true;
			}
		}
		if (!flag)
		{
			LLLOJBFMONN.Error("BattlePeriodic::initBattles WARNING - no duel fights found, repeatTime has not been set!");
		}
	}

	public static void Clear()
	{
		_battles.Clear();
		HJOHKOEICAP = 0L;
	}
}
