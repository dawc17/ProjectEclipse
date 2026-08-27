using System.Collections.Generic;
using UnityEngine;

public class ComboStatistic
{
	public FightStatistics.EMKEIEJMONM BPBDGAPENAK;

	public string StatisticCrazyStyleToString = OLONAJAOFOA(FightStatistics.EMKEIEJMONM.STYLE_TURTLE);

	public int JDKFHFOJKPI;

	public int IGMFLCNOKPA;

	public int MOLDOOIJELI;

	public int NFKHLNHIIKH;

	public int BAHCDHKAJBB;

	public int KKJHBKBMPGN;

	public int OGMOILIMCOM;

	public ComboStatisticPrize ECOOCLMNFJM = new ComboStatisticPrize();

	public DetailedDamages MNDEOFOHLHI = new DetailedDamages();

	public void MPCLGKFBGCO(long BLOOFMGLMHP, long GICNLBOICGP, long KNDKJANLIDI, float BHGNKHIKGOG, float FKHKEHICPAH, float IFCOPPPDOCD, float LMKJOMKPOAM, float OJIPBDBMLLO, List<float> LNDELINEHAL)
	{
		float num = 0.5f;
		ECOOCLMNFJM.PJBCIEMHPNN = BLOOFMGLMHP;
		ECOOCLMNFJM.PDJPOBHLIHA = GameUtils.GetDenominatedValue(GICNLBOICGP);
		ECOOCLMNFJM.JNCDLOAEMCG = KNDKJANLIDI;
		ECOOCLMNFJM.MKNGIDKGOLE = GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * BHGNKHIKGOG) * (float)JDKFHFOJKPI + num));
		ECOOCLMNFJM.LOONMILKCFK = GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * FKHKEHICPAH) * (float)MOLDOOIJELI + num));
		ECOOCLMNFJM.GKAEJDCDMHC = GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * LMKJOMKPOAM) * (float)KKJHBKBMPGN + num));
		ECOOCLMNFJM.AIJNPAIMPHG = GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * LNDELINEHAL[(int)BPBDGAPENAK]) + num));
		ECOOCLMNFJM.APCAKCCOMLO = GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * OJIPBDBMLLO) * (float)OGMOILIMCOM + num));
		ECOOCLMNFJM.POPNFGNAOJD = ECOOCLMNFJM.PDJPOBHLIHA + ECOOCLMNFJM.MKNGIDKGOLE + ECOOCLMNFJM.LOONMILKCFK + ECOOCLMNFJM.GKAEJDCDMHC + ECOOCLMNFJM.AIJNPAIMPHG + ECOOCLMNFJM.APCAKCCOMLO;
		ECOOCLMNFJM.AMFFCKOAAED = KNDKJANLIDI;
	}

	public void EAOGOCDLLBD(long BLOOFMGLMHP, long GICNLBOICGP, long KNDKJANLIDI, float BHGNKHIKGOG, float FKHKEHICPAH, float IFCOPPPDOCD, float LMKJOMKPOAM, float OJIPBDBMLLO, List<float> LNDELINEHAL)
	{
		float num = 0.5f;
		ECOOCLMNFJM.PJBCIEMHPNN += BLOOFMGLMHP;
		ECOOCLMNFJM.PDJPOBHLIHA += GameUtils.GetDenominatedValue(GICNLBOICGP);
		ECOOCLMNFJM.JNCDLOAEMCG += KNDKJANLIDI;
		ECOOCLMNFJM.MKNGIDKGOLE += GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * BHGNKHIKGOG) * (float)JDKFHFOJKPI + num));
		ECOOCLMNFJM.LOONMILKCFK += GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * FKHKEHICPAH) * (float)MOLDOOIJELI + num));
		ECOOCLMNFJM.GKAEJDCDMHC += GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * LMKJOMKPOAM) + num) * KKJHBKBMPGN);
		ECOOCLMNFJM.AIJNPAIMPHG += GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * LNDELINEHAL[(int)BPBDGAPENAK]) + num));
		ECOOCLMNFJM.APCAKCCOMLO += GameUtils.GetDenominatedValue((long)(Mathf.Ceil((float)BLOOFMGLMHP * OJIPBDBMLLO) * (float)OGMOILIMCOM + num));
		ECOOCLMNFJM.POPNFGNAOJD = ECOOCLMNFJM.PDJPOBHLIHA + ECOOCLMNFJM.MKNGIDKGOLE + ECOOCLMNFJM.LOONMILKCFK + ECOOCLMNFJM.GKAEJDCDMHC + ECOOCLMNFJM.AIJNPAIMPHG + ECOOCLMNFJM.APCAKCCOMLO;
		ECOOCLMNFJM.AMFFCKOAAED = ECOOCLMNFJM.JNCDLOAEMCG;
	}

	public string OLONAJAOFOA()
	{
		return OLONAJAOFOA(BPBDGAPENAK);
	}

	public static string OLONAJAOFOA(FightStatistics.EMKEIEJMONM KIGNIBIMLKK)
	{
		string result = string.Empty;
		switch (KIGNIBIMLKK)
		{
		case FightStatistics.EMKEIEJMONM.STYLE_TURTLE:
			result = "goldTurtleStyle";
			break;
		case FightStatistics.EMKEIEJMONM.STYLE_HARD:
			result = "goldHardStyle";
			break;
		case FightStatistics.EMKEIEJMONM.STYLE_BRUTAL:
			result = "goldBrutalStyle";
			break;
		case FightStatistics.EMKEIEJMONM.STYLE_AGGRESSIVE:
			result = "goldAgressiveStyle";
			break;
		case FightStatistics.EMKEIEJMONM.STYLE_CRAZY:
			result = "goldCrazyStyle";
			break;
		case FightStatistics.EMKEIEJMONM.STYLE_FANTASTIC:
			result = "goldFantasticStyle";
			break;
		}
		return result;
	}
}
