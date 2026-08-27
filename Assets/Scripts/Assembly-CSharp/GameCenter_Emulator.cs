using UnityEngine.SocialPlatforms;

public class GameCenter_Emulator : GameCenterAbstract
{
	public override bool ABFAHBGHFOB
	{
		get
		{
			return EPACOIFEICA();
		}
	}

	public override bool JLDADBGJMFA
	{
		get
		{
			return CPOLMPAAHOL();
		}
	}

	public override bool FJBFKLDBMDD
	{
		get
		{
			return OBDJPKOJADA();
		}
	}

	public override string FFNCAFDPLPL
	{
		get
		{
			return CONEABALMEJ();
		}
	}

	public override void Init()
	{
		Log("[GameCenter_Emulator]: Init");
	}

	public override void PJNFHNFLNNO()
	{
		Log("[GameCenter_Emulator]: Free");
	}

	private void JDJNLPHEOAP(bool BPEIMKJIMOF)
	{
		Log("[GameCenter_Emulator]: CB_Authenticate, authed = " + BPEIMKJIMOF);
	}

	private void NDDDABCMLEI(IAchievement[] HELFDCAIJNE)
	{
		Log("[GameCenter_Emulator]: CB_LoadAchievements");
	}

	private void HFCGCEIPGFJ(string OKNNNLIPODI)
	{
		Log("[GameCenter_Emulator]: CB_AchievementUnlocked, id = " + OKNNNLIPODI);
	}

	private void ALFBABCJCDN(string HMDBGGEMICE, int EPFBHJBNIHK)
	{
		Log("[GameCenter_Emulator]: OnAchievementProgess id = " + HMDBGGEMICE + " progress = " + EPFBHJBNIHK);
	}

	public override bool EPACOIFEICA()
	{
		return false;
	}

	public override bool CPOLMPAAHOL()
	{
		return false;
	}

	public override void EFKOIIKEHDO()
	{
		Log("[GameCenter_Emulator]: SignIn");
	}

	public override void CLPNGGPKAHO()
	{
		Log("[GameCenter_Emulator]: SignOut");
	}

	public override bool OBDJPKOJADA()
	{
		return false;
	}

	public override string CONEABALMEJ()
	{
		return string.Empty;
	}

	public override void KBAPDJLNCJE()
	{
	}

	public override void KGKPLKJPDAI()
	{
		Log("[GameCenter_Emulator]: ShowAchievements");
	}

	public override void MMGHEKOEHDB()
	{
		Log("[GameCenter_Emulator]: ResetAchievements");
		GameCenterAbstract.ALEHBEKFIAN(true);
	}

	public override void UnlockAchievement(string OKNNNLIPODI)
	{
		Log("[GameCenter_Emulator]: UnlockAchievement " + OKNNNLIPODI);
	}

	public override void MIMPHPINBNF(string OKNNNLIPODI, double EPFBHJBNIHK)
	{
		Log("[GameCenter_Emulator]: AchievementProgress id = " + OKNNNLIPODI + " ,progress = " + EPFBHJBNIHK);
	}
}
