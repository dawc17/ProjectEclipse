using UnityEngine;

public class LocalNotificationManager
{
	private enum KDEIFBMDHNP
	{
		PushRetention = 1,
		PushPeriodic = 2,
		PushEnergy = 3,
		PushEnergyFull = 4,
		PushItem = 5,
		PushRecipe = 6,
		PushTest1 = 7
	}

	private static LocalNotificationManager _instance;

	private const string NMACCBOGCDN = "push_retention";

	private const string FMBLCGLILOM = "push_periodic";

	private const string GMMEFCDAKBG = "push_energy_to_fight";

	private const string NLEIINDMNOD = "push_full_energy";

	private bool COKIKICKGOO;

	private const string AFBGPKADCOB = "id";

	private bool BGDDAFGPPGI;

	public static LocalNotificationManager BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	private string Title
	{
		get
		{
			return IDLDKFEPJLI();
		}
	}

	private LocalNotificationManager()
	{
		KKJBEGLMHCM(false);
		COKIKICKGOO = false;
		AGONLLFBOFG(AssemblyController.BNEAFMHNIPK());
	}

	public static LocalNotificationManager ELEBLBJKDBI()
	{
		if (_instance == null)
		{
			_instance = new LocalNotificationManager();
		}
		return _instance;
	}

	public void AGONLLFBOFG(bool value)
	{
		if (COKIKICKGOO != value)
		{
			COKIKICKGOO = value;
		}
	}

	private string IDLDKFEPJLI()
	{
		return Application.productName;
	}

	public void KKJBEGLMHCM(bool value)
	{
		BGDDAFGPPGI = value;
	}

	private bool FIMGFNEAFDH()
	{
		return BGDDAFGPPGI || !SystemProperties.AFAAJMFLBIC();
	}

	public void IMABAABEIOI(long IHDMLLNEGIK)
	{
		GAMLNBGMCHB(1, IDLDKFEPJLI(), LocalizationManager.GetString("push_retention"), IHDMLLNEGIK);
	}

	public void HPCBBNCDPEB(long IHDMLLNEGIK)
	{
		GAMLNBGMCHB(2, IDLDKFEPJLI(), LocalizationManager.GetString("push_periodic"), IHDMLLNEGIK);
	}

	public void IOEKOAKONGH(long IHDMLLNEGIK)
	{
		GAMLNBGMCHB(7, IDLDKFEPJLI(), "Test1 notification, delay=" + IHDMLLNEGIK, IHDMLLNEGIK);
	}

	public void DODOMBCHMDN(long IHDMLLNEGIK)
	{
		// Energy is disabled; never schedule refill reminders.
	}

	public void HOHFHDMEDLI(long IHDMLLNEGIK)
	{
		// Energy is disabled; never schedule refill reminders.
	}

	public void EGOMGODAMFF(string LIOGIBJBHAH, long IHDMLLNEGIK)
	{
		if (FIMGFNEAFDH())
		{
			GAMLNBGMCHB(6, IDLDKFEPJLI(), LIOGIBJBHAH, IHDMLLNEGIK);
		}
	}

	public void HGOKJEIHKPE(string LIOGIBJBHAH, long IHDMLLNEGIK)
	{
		if (FIMGFNEAFDH())
		{
			GAMLNBGMCHB(5, IDLDKFEPJLI(), LIOGIBJBHAH, IHDMLLNEGIK);
		}
	}

	public void ECNMCOKOEBF()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushRetention);
	}

	public void AHLFKAGBLEN()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushPeriodic);
	}

	public void JHIJCEJBEGP()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushTest1);
	}

	public void OHMBBMKPAHD()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushEnergy);
	}

	public void GONAFNDNGHK()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushEnergyFull);
	}

	public void ENAFDJHIDJJ()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushRecipe);
	}

	public void DJNHJBNKBIB()
	{
		MKOEHNJBKNM(KDEIFBMDHNP.PushItem);
	}

	public void PLEIIJMCPHF()
	{
		if (COKIKICKGOO)
		{
			OHMBBMKPAHD();
			GONAFNDNGHK();
			ECNMCOKOEBF();
			AHLFKAGBLEN();
			ENAFDJHIDJJ();
			DJNHJBNKBIB();
			JHIJCEJBEGP();
		}
	}

	private void MKOEHNJBKNM(KDEIFBMDHNP KEMMPFEDLAJ)
	{
		if (COKIKICKGOO)
		{
			AndroidLocalNotification.MKOEHNJBKNM((int)KEMMPFEDLAJ);
		}
	}

	private void GAMLNBGMCHB(int OKNNNLIPODI, string PEMOECLNECD, string LIOGIBJBHAH, long ENDPMCNJPEA)
	{
		if (COKIKICKGOO && SystemProperties.IPJFCBAGMJJ())
		{
			AndroidLocalNotification.GAMLNBGMCHB(OKNNNLIPODI, PEMOECLNECD, LIOGIBJBHAH, ENDPMCNJPEA);
		}
	}

	public string IINLKICBLEB()
	{
		return string.Concat(Application.platform, " is not supported, only IOS devices");
	}
}
