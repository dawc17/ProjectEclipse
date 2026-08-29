using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BattleReplayable : Battle
{
	protected List<Rule> _rules = new List<Rule>();

	protected bool _rulesAndWarriorsSet;

	public BattleReplayable(string LFLGCDNKNJI, Vector2 MGMMDGFPBLP, string name, string ADONPNOBBDE, string LHCFHAIDNDP, string EMDJGBHIAIA, ushort CDCJKJNGPOE, ushort MCDAHGPLLDO, string LOKLDPLAPOL, string PEMOECLNECD, string LPJNEDFCBOI, string PINIIFIOECE, string OAPKHNPPGHP, string IHBMPGKIBAN)
		: base(LFLGCDNKNJI, MGMMDGFPBLP, name, ADONPNOBBDE, LHCFHAIDNDP, EMDJGBHIAIA, CDCJKJNGPOE, MCDAHGPLLDO, LOKLDPLAPOL, PEMOECLNECD, LPJNEDFCBOI, PINIIFIOECE, OAPKHNPPGHP, IHBMPGKIBAN)
	{
		_rulesAndWarriorsSet = false;
	}

	public void Parse(XmlNode node)
	{
		XmlNode hKPPBKPJOEO = node["Rules"];
		EEPPJEMHBCK(hKPPBKPJOEO);
	}

	public int HLBOMMKJAAO()
	{
		return (MEOMPEEPCJJ != null) ? MEOMPEEPCJJ.ODCFKCJJDKN() : 0;
	}

	public bool TryStartNextReplay()
	{
		if (MEOMPEEPCJJ == null || MEOMPEEPCJJ.NLIJBCHAEBK())
		{
			return false;
		}
		List<FightList> fights = ANNHMNIHKCC();
		if (fights.Count == 0)
		{
			return false;
		}
		// Wins are lifetime counters used by rewards and quests. Advance the
		// cycle instead of clearing them, and only after EVERY fight is done.
		// Deriving the cycle from saved wins also repairs already completed saves.
		int completedCycles = int.MaxValue;
		foreach (FightList fight in fights)
		{
			RosterFight rosterFight = fight.FLKFFDLLBKA();
			if (rosterFight == null || fight.EJGGHHEOGPG <= 0)
			{
				return false;
			}
			completedCycles = System.Math.Min(completedCycles, rosterFight.JAJNIKDMPPO() / fight.EJGGHHEOGPG);
		}
		if (completedCycles <= HLBOMMKJAAO())
		{
			return false;
		}
		MEOMPEEPCJJ.FHCHCHPPMEI(completedCycles);
		foreach (FightList fight in fights)
		{
			ListSF.DINPFDGMEAB(fight);
		}
		return true;
	}

	public virtual void MJJFFAOLCCK(FightList KGKDKENMAOA)
	{
		int num = HLBOMMKJAAO();
		RosterFight pIGKOIFBOME = KGKDKENMAOA.FLKFFDLLBKA();
		int eJGGHHEOGPG = KGKDKENMAOA.EJGGHHEOGPG;
		if (pIGKOIFBOME != null)
		{
			if (pIGKOIFBOME.JAJNIKDMPPO() >= eJGGHHEOGPG * (num + 1))
			{
				KGKDKENMAOA.PGBKNLAEANJ = ConditionStatus.StatusComplete;
			}
			else
			{
				KGKDKENMAOA.PGBKNLAEANJ = ConditionStatus.StatusOpen;
			}
		}
	}

	protected void EEPPJEMHBCK(XmlNode node)
	{
		RuleParser.EEPPJEMHBCK(node, _rules);
	}
}
