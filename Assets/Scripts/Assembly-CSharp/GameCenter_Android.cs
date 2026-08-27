using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCenter_Android : GameCenterAbstract
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

	public override string FFNCAFDPLPL
	{
		get
		{
			return CONEABALMEJ();
		}
	}

	public override bool FJBFKLDBMDD
	{
		get
		{
			return OBDJPKOJADA();
		}
	}

	public override void Init()
	{
		Log("[GameCenter_Android]: Init");
		PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().RequestEmail().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.DebugLogEnabled = SystemProperties.DBBOCENKMGD();
		PlayGamesPlatform.Activate();
	}

	public override void PJNFHNFLNNO()
	{
		Log("[GameCenter_Android]: Free");
	}

	private void JDJNLPHEOAP(bool BPEIMKJIMOF)
	{
		Log("[GameCenter_Android]: CB_Authenticate, authed = " + BPEIMKJIMOF);
		if (BPEIMKJIMOF)
		{
			KBAPDJLNCJE();
		}
		GameCenterAbstract.OnAuthenticate(BPEIMKJIMOF);
	}

	private void NDDDABCMLEI(IAchievement[] HELFDCAIJNE)
	{
		Log("[GameCenter_Android]: CB_LoadAchievements");
		GameCenterAbstract.OnLoadAchievements(HELFDCAIJNE);
	}

	private void NIAIKNHFOIH(bool BPEIMKJIMOF)
	{
		Log("[GameCenter_Android]: CB_AuthenticateAndShowAchievementsUI, authed = " + BPEIMKJIMOF);
		if (BPEIMKJIMOF)
		{
			UnityEngine.Social.ShowAchievementsUI();
		}
	}

	private void HFCGCEIPGFJ(string HMDBGGEMICE)
	{
		Log("[GameCenter_Android]: CB_AchievementUnlocked, id = " + HMDBGGEMICE);
		GameCenterAbstract.CBELEBJOJJK(HMDBGGEMICE);
	}

	private void ALFBABCJCDN(string HMDBGGEMICE, double EPFBHJBNIHK)
	{
		Log("[GameCenterAndroid]: OnAchievementProgess id = " + HMDBGGEMICE + " progress = " + EPFBHJBNIHK);
		GameCenterAbstract.OPMNMBEJMED(HMDBGGEMICE, EPFBHJBNIHK);
	}

	public override bool EPACOIFEICA()
	{
		return true;
	}

	public override bool CPOLMPAAHOL()
	{
		return true;
	}

	public override string CONEABALMEJ()
	{
		return PlayGamesPlatform.Instance.GetUserEmail();
	}

	public override void EFKOIIKEHDO()
	{
		Log("[GameCenterAndroid]: SignIn");
		if (!UnityEngine.Social.localUser.authenticated)
		{
			UnityEngine.Social.localUser.Authenticate(JDJNLPHEOAP);
		}
		else
		{
			JDJNLPHEOAP(true);
		}
	}

	public override void CLPNGGPKAHO()
	{
		if (UnityEngine.Social.localUser.authenticated)
		{
			PlayGamesPlatform.Instance.SignOut();
			JDJNLPHEOAP(OBDJPKOJADA());
		}
	}

	public override bool OBDJPKOJADA()
	{
		return PlayGamesPlatform.Instance.IsAuthenticated();
	}

	public override void KBAPDJLNCJE()
	{
		UnityEngine.Social.LoadAchievements(NDDDABCMLEI);
	}

	public override void KGKPLKJPDAI()
	{
		bool authenticated = UnityEngine.Social.localUser.authenticated;
		if (authenticated)
		{
			UnityEngine.Social.localUser.Authenticate(NIAIKNHFOIH);
		}
		else
		{
			NIAIKNHFOIH(authenticated);
		}
	}

	public override void MMGHEKOEHDB()
	{
	}

	public override void UnlockAchievement(string OKNNNLIPODI)
	{
		GooglePlayGames.BasicApi.Achievement NCCHENOEPNF = PlayGamesPlatform.Instance.GetAchievement(OKNNNLIPODI);
		if (NCCHENOEPNF == null)
		{
			Log("[GameCenter_Android]: Error - Achievement null! Info: " + OKNNNLIPODI);
			return;
		}
		if (NCCHENOEPNF.IsUnlocked)
		{
			Log("[GameCenter_Android]: Error - Achievement already unlocked! Info: " + NCCHENOEPNF.ToString());
			return;
		}
		IAchievement achievement = PlayGamesPlatform.Instance.CreateAchievement();
		achievement.id = OKNNNLIPODI;
		achievement.percentCompleted = 100.0;
		achievement.ReportProgress((bool IBFAPIMOMBA) =>
		{
			if (IBFAPIMOMBA)
			{
				Log("[GameCenter_Android]:  Achievement successfully unlocked! Info: " + NCCHENOEPNF.ToString());
				HFCGCEIPGFJ(OKNNNLIPODI);
			}
			else
			{
				Log("[GameCenter_Android]:  Error - failed to unlock Achievement! Info: " + NCCHENOEPNF.ToString());
			}
		});
	}

	public override void MIMPHPINBNF(string HMDBGGEMICE, double EPFBHJBNIHK)
	{
		GooglePlayGames.BasicApi.Achievement achievement = PlayGamesPlatform.Instance.GetAchievement(HMDBGGEMICE);
		if (achievement == null)
		{
			Log("[GameCenter_Android]: Error - Achievement null! Info: " + HMDBGGEMICE);
		}
		else if (achievement.IsUnlocked)
		{
			Log("[GameCenter_Android]: Error - Achievement already unlocked! Info: " + achievement.ToString());
			return;
		}
		PlayGamesPlatform.Instance.SetStepsAtLeast(HMDBGGEMICE, (int)EPFBHJBNIHK, (bool IBFAPIMOMBA) =>
		{
			if (IBFAPIMOMBA)
			{
				Log("[GameCenter_Android]:  Achievement successfully progress! Id: " + HMDBGGEMICE + " Progress: " + EPFBHJBNIHK);
				ALFBABCJCDN(HMDBGGEMICE, EPFBHJBNIHK);
			}
			else
			{
				Log("[GameCenter_Android]:  Error - failed to progress Achievement! Id: " + HMDBGGEMICE + " Progress: " + EPFBHJBNIHK);
			}
		});
	}
}
