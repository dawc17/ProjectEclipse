using System.Collections.Generic;

public static class GameCenterController
{
	private static GameCenterAbstract _Current;

	public static GameCenterAbstract BLOOLFFMKFI
	{
		get
		{
			return AOJJOEHEPGM();
		}
	}

	public static bool ABFAHBGHFOB
	{
		get
		{
			return EPACOIFEICA();
		}
	}

	public static bool JLDADBGJMFA
	{
		get
		{
			return CPOLMPAAHOL();
		}
	}

	public static bool FJBFKLDBMDD
	{
		get
		{
			return OBDJPKOJADA();
		}
	}

	public static string FFNCAFDPLPL
	{
		get
		{
			return CONEABALMEJ();
		}
	}

	public static GameCenterAbstract AOJJOEHEPGM()
	{
		if (_Current == null)
		{
			Init();
		}
		return _Current;
	}

	public static void Init()
	{
		if (_Current == null)
		{
			_Current = new GameCenter_Android();
			_Current.Init();
		}
	}

	public static void PJNFHNFLNNO()
	{
		AOJJOEHEPGM().PJNFHNFLNNO();
	}

	public static bool EPACOIFEICA()
	{
		return _Current.EPACOIFEICA();
	}

	public static bool CPOLMPAAHOL()
	{
		return _Current.CPOLMPAAHOL();
	}

	public static void EFKOIIKEHDO()
	{
		if (EPACOIFEICA())
		{
			AOJJOEHEPGM().EFKOIIKEHDO();
		}
	}

	public static void CLPNGGPKAHO()
	{
		if (EPACOIFEICA())
		{
			AOJJOEHEPGM().CLPNGGPKAHO();
		}
	}

	public static bool OBDJPKOJADA()
	{
		if (EPACOIFEICA())
		{
			return AOJJOEHEPGM().OBDJPKOJADA();
		}
		return false;
	}

	public static string CONEABALMEJ()
	{
		if (OBDJPKOJADA())
		{
			return SystemProperties.MakeIdentifier(AOJJOEHEPGM().CONEABALMEJ());
		}
		return string.Empty;
	}

	public static void UnlockAchievement(string OKNNNLIPODI)
	{
		if (OBDJPKOJADA())
		{
			AOJJOEHEPGM().UnlockAchievement(OKNNNLIPODI);
		}
	}

	public static void MIMPHPINBNF(string OKNNNLIPODI, double EPFBHJBNIHK)
	{
		if (OBDJPKOJADA())
		{
			AOJJOEHEPGM().MIMPHPINBNF(OKNNNLIPODI, EPFBHJBNIHK);
		}
	}

	public static void NPMGIFJKAEG()
	{
		if (OBDJPKOJADA())
		{
			AOJJOEHEPGM().KGKPLKJPDAI();
		}
	}

	public static void KBAPDJLNCJE()
	{
		if (OBDJPKOJADA())
		{
			AOJJOEHEPGM().KBAPDJLNCJE();
		}
	}

	public static void MMGHEKOEHDB()
	{
		if (OBDJPKOJADA() || SystemProperties.LHGPKEFEHDH())
		{
			AOJJOEHEPGM().MMGHEKOEHDB();
		}
	}

	public static bool GEJNIMAILDA()
	{
		return false;
	}

	public static void FLJILJDHNLJ(List<SocialAchievement> CIMGCGDDKCE)
	{
		foreach (SocialAchievement item in CIMGCGDDKCE)
		{
			if (item.value < item.MFODOCNLNPH)
			{
				int num = 0;
				num = item.value;
				MIMPHPINBNF(item.name, num);
			}
			else
			{
				UnlockAchievement(item.name);
			}
		}
	}
}
