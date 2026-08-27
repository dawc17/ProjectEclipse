using System;
using System.Collections.Generic;
using System.Diagnostics;
using BestHTTP;
using Org.BouncyCastle.Crypto.Tls;
using UnityEngine;

public static class HTTPManager
{
	private static byte maxConnectionPerServer;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool BEBAGLBMIDO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool BHCFACLDBAM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static TimeSpan HACDMEKCFOJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool ENOAEBAADAD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static uint AOJEMDDEFIG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool NKCCPFLEILB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static TimeSpan FNHLIDGNHLF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static TimeSpan HALNDDNKDCN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static Func<string> EDJMLMPEBBC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static HTTPProxy FGGPKCKKPNB;

	private static HeartbeatManager GCJIMNCMONP;

	private static ILogger KCAIHGKMIIA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static ICertificateVerifyer FLGLKKBJIAI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool ALIAKDLMJHI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static int LGCEBPKAMHG;

	private static Dictionary<string, List<HTTPConnection>> GKLCMJOHCBJ;

	private static List<HTTPConnection> BPIDLPMODDC;

	private static List<HTTPConnection> HHGJJICJJOJ;

	private static List<HTTPConnection> OBMKJIOLKNI;

	private static List<HTTPRequest> LCCNAGOGEBC;

	private static bool NIEABCELPDJ;

	internal static object Locker;

	public static byte OFJPDEGFEBJ
	{
		get
		{
			return BDCIBFLAPJN();
		}
		set
		{
			set_MaxConnectionPerServer(value);
		}
	}

	public static bool MBGEBPKCHIJ
	{
		get
		{
			return HAIGHJHOEDH();
		}
		set
		{
			CIAJMMMGKLK(value);
		}
	}

	public static bool AKKKFHCMNLO
	{
		get
		{
			return NLGHFPFIMMH();
		}
		set
		{
			PPEMJOOLDDG(value);
		}
	}

	public static TimeSpan FBLNJACMCMN
	{
		get
		{
			return AAKIPAJACAH();
		}
		set
		{
			GIIOIOKMLKE(value);
		}
	}

	public static bool PHLKGBCMHMO
	{
		get
		{
			return IJJCLBHKMDJ();
		}
		set
		{
			AGOGJCLDCGG(value);
		}
	}

	public static bool HGCBJBFOBGM
	{
		get
		{
			return IMFEILECHFL();
		}
		set
		{
			LBKANPAKAJD(value);
		}
	}

	public static TimeSpan PFODFFILGKE
	{
		get
		{
			return DGHOJLHDGPB();
		}
		set
		{
			CLDOBKEACOC(value);
		}
	}

	public static TimeSpan CLKLCGGIBNI
	{
		get
		{
			return AFHJBDAKIPE();
		}
		set
		{
			MODDBPHHKDB(value);
		}
	}

	public static Func<string> MGHKKDNDOKK
	{
		get
		{
			return OLFBIBGHAAC();
		}
		set
		{
			set_RootCacheFolderProvider(value);
		}
	}

	public static HTTPProxy DEFLNIGINCO
	{
		get
		{
			return FHGBKFBCGCO();
		}
		set
		{
			PNGMAECJHID(value);
		}
	}

	public static HeartbeatManager KBHEHJBEGNK
	{
		get
		{
			return MAMNLAJACOD();
		}
	}

	public static ILogger GAEHFPGIDDH
	{
		get
		{
			return MBBMPNDDPIH();
		}
		set
		{
			EHLECGJCDPA(value);
		}
	}

	public static ICertificateVerifyer GELEAIBMAOF
	{
		get
		{
			return MBEAAMJILEI();
		}
		set
		{
			DLAHJOKFDKD(value);
		}
	}

	public static bool OILNBGELKJI
	{
		get
		{
			return IHBIPNGCEFM();
		}
		set
		{
			NNHJKFMGDDO(value);
		}
	}

	internal static int AANEKIFNLDN
	{
		get
		{
			return LACBBEFPIPO();
		}
		set
		{
			set_MaxPathLength(value);
		}
	}

	static HTTPManager()
	{
		GKLCMJOHCBJ = new Dictionary<string, List<HTTPConnection>>();
		BPIDLPMODDC = new List<HTTPConnection>();
		HHGJJICJJOJ = new List<HTTPConnection>();
		OBMKJIOLKNI = new List<HTTPConnection>();
		LCCNAGOGEBC = new List<HTTPRequest>();
		Locker = new object();
		set_MaxConnectionPerServer(4);
		CIAJMMMGKLK(true);
		set_MaxPathLength(255);
		GIIOIOKMLKE(TimeSpan.FromSeconds(30.0));
		AGOGJCLDCGG(true);
		set_CookieJarSize(10485760u);
		LBKANPAKAJD(false);
		CLDOBKEACOC(TimeSpan.FromSeconds(20.0));
		MODDBPHHKDB(TimeSpan.FromSeconds(60.0));
		KCAIHGKMIIA = new DefaultLogger();
		DLAHJOKFDKD(null);
		NNHJKFMGDDO(false);
	}

	public static byte BDCIBFLAPJN()
	{
		return maxConnectionPerServer;
	}

	public static void set_MaxConnectionPerServer(byte value)
	{
		if (value <= 0)
		{
			throw new ArgumentOutOfRangeException("MaxConnectionPerServer must be greater than 0!");
		}
		maxConnectionPerServer = value;
	}

	public static bool HAIGHJHOEDH()
	{
		return BEBAGLBMIDO;
	}

	public static void CIAJMMMGKLK(bool value)
	{
		BEBAGLBMIDO = value;
	}

	public static bool NLGHFPFIMMH()
	{
		return BHCFACLDBAM;
	}

	public static void PPEMJOOLDDG(bool value)
	{
		BHCFACLDBAM = value;
	}

	public static TimeSpan AAKIPAJACAH()
	{
		return HACDMEKCFOJ;
	}

	public static void GIIOIOKMLKE(TimeSpan value)
	{
		HACDMEKCFOJ = value;
	}

	public static bool IJJCLBHKMDJ()
	{
		return ENOAEBAADAD;
	}

	public static void AGOGJCLDCGG(bool value)
	{
		ENOAEBAADAD = value;
	}

	public static uint CFPIDMJOENK()
	{
		return AOJEMDDEFIG;
	}

	public static void set_CookieJarSize(uint value)
	{
		AOJEMDDEFIG = value;
	}

	public static bool IMFEILECHFL()
	{
		return NKCCPFLEILB;
	}

	public static void LBKANPAKAJD(bool value)
	{
		NKCCPFLEILB = value;
	}

	public static TimeSpan DGHOJLHDGPB()
	{
		return FNHLIDGNHLF;
	}

	public static void CLDOBKEACOC(TimeSpan value)
	{
		FNHLIDGNHLF = value;
	}

	public static TimeSpan AFHJBDAKIPE()
	{
		return HALNDDNKDCN;
	}

	public static void MODDBPHHKDB(TimeSpan value)
	{
		HALNDDNKDCN = value;
	}

	public static Func<string> OLFBIBGHAAC()
	{
		return EDJMLMPEBBC;
	}

	public static void set_RootCacheFolderProvider(Func<string> value)
	{
		EDJMLMPEBBC = value;
	}

	public static HTTPProxy FHGBKFBCGCO()
	{
		return FGGPKCKKPNB;
	}

	public static void PNGMAECJHID(HTTPProxy value)
	{
		FGGPKCKKPNB = value;
	}

	public static HeartbeatManager MAMNLAJACOD()
	{
		if (GCJIMNCMONP == null)
		{
			GCJIMNCMONP = new HeartbeatManager();
		}
		return GCJIMNCMONP;
	}

	public static ILogger MBBMPNDDPIH()
	{
		if (KCAIHGKMIIA == null)
		{
			KCAIHGKMIIA = new DefaultLogger();
			KCAIHGKMIIA.DLDMOHEGENM(BFNKPHDJNII.None);
		}
		return KCAIHGKMIIA;
	}

	public static void EHLECGJCDPA(ILogger value)
	{
		KCAIHGKMIIA = value;
	}

	public static ICertificateVerifyer MBEAAMJILEI()
	{
		return FLGLKKBJIAI;
	}

	public static void DLAHJOKFDKD(ICertificateVerifyer value)
	{
		FLGLKKBJIAI = value;
	}

	public static bool IHBIPNGCEFM()
	{
		return ALIAKDLMJHI;
	}

	public static void NNHJKFMGDDO(bool value)
	{
		ALIAKDLMJHI = value;
	}

	internal static int LACBBEFPIPO()
	{
		return LGCEBPKAMHG;
	}

	internal static void set_MaxPathLength(int value)
	{
		LGCEBPKAMHG = value;
	}

	public static void PAINOJOIGMC()
	{
		HTTPUpdateDelegator.CheckInstance();
		HTTPCacheService.BEGBHCIIOAO();
		CookieJar.ELIJOFFHEBP();
	}

	public static HTTPRequest EMPGOCGHMBI(string BEPKJNKCKPH, OnRequestFinishedDelegate callback)
	{
		return EMPGOCGHMBI(new HTTPRequest(new Uri(BEPKJNKCKPH), LAAFHDKKJFL.Get, callback));
	}

	public static HTTPRequest EMPGOCGHMBI(string BEPKJNKCKPH, LAAFHDKKJFL AMFJIGAEHLD, OnRequestFinishedDelegate callback)
	{
		return EMPGOCGHMBI(new HTTPRequest(new Uri(BEPKJNKCKPH), AMFJIGAEHLD, callback));
	}

	public static HTTPRequest EMPGOCGHMBI(string BEPKJNKCKPH, LAAFHDKKJFL AMFJIGAEHLD, bool LLLAPINJJIJ, OnRequestFinishedDelegate callback)
	{
		return EMPGOCGHMBI(new HTTPRequest(new Uri(BEPKJNKCKPH), AMFJIGAEHLD, LLLAPINJJIJ, callback));
	}

	public static HTTPRequest EMPGOCGHMBI(string BEPKJNKCKPH, LAAFHDKKJFL AMFJIGAEHLD, bool LLLAPINJJIJ, bool JNCJAGIBJFL, OnRequestFinishedDelegate callback)
	{
		return EMPGOCGHMBI(new HTTPRequest(new Uri(BEPKJNKCKPH), AMFJIGAEHLD, LLLAPINJJIJ, JNCJAGIBJFL, callback));
	}

	public static HTTPRequest EMPGOCGHMBI(HTTPRequest ONOCIELLAPL)
	{
		lock (Locker)
		{
			PAINOJOIGMC();
			if (NIEABCELPDJ)
			{
				ONOCIELLAPL.set_State(CFGBMHKCENK.Queued);
				LCCNAGOGEBC.Add(ONOCIELLAPL);
			}
			else
			{
				KDCMNDFKCAA(ONOCIELLAPL);
			}
			return ONOCIELLAPL;
		}
	}

	public static GeneralStatistics CBGIGIBGBLD(StatisticsQueryFlags AGADCPIIGLC)
	{
		GeneralStatistics result = new GeneralStatistics
		{
			EMFKBMDJFCJ = AGADCPIIGLC
		};
		if ((AGADCPIIGLC & StatisticsQueryFlags.Connections) != 0)
		{
			int num = 0;
			foreach (KeyValuePair<string, List<HTTPConnection>> item in GKLCMJOHCBJ)
			{
				if (item.Value != null)
				{
					num += item.Value.Count;
				}
			}
			result.GKLCMJOHCBJ = num;
			result.BPIDLPMODDC = BPIDLPMODDC.Count;
			result.HHGJJICJJOJ = HHGJJICJJOJ.Count;
			result.OBMKJIOLKNI = OBMKJIOLKNI.Count;
			result.AOIDCCECOIE = LCCNAGOGEBC.Count;
		}
		if ((AGADCPIIGLC & StatisticsQueryFlags.Cache) != 0)
		{
			result.LMEFONBEGEN = HTTPCacheService.MDIKHCGGACM();
			result.CacheSize = HTTPCacheService.NKOHKEGHKJN();
		}
		if ((AGADCPIIGLC & StatisticsQueryFlags.Cookies) != 0)
		{
			List<Cookie> list = CookieJar.CDFJFIJHDOM();
			result.GPBFMKPPIAL = list.Count;
			uint num2 = 0u;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i].ADGKKEKOJBD();
			}
			result.CookieJarSize = num2;
		}
		return result;
	}

	private static void KDCMNDFKCAA(HTTPRequest ONOCIELLAPL)
	{
		HTTPConnection NNLEEIONBEP = EDFODKJKANN(ONOCIELLAPL);
		if (NNLEEIONBEP != null)
		{
			if (BPIDLPMODDC.Find((HTTPConnection ILHDJDNPFKH) => ILHDJDNPFKH == NNLEEIONBEP) == null)
			{
				BPIDLPMODDC.Add(NNLEEIONBEP);
			}
			HHGJJICJJOJ.Remove(NNLEEIONBEP);
			ONOCIELLAPL.set_State(CFGBMHKCENK.Processing);
			ONOCIELLAPL.NDNOFGKMHDG();
			NNLEEIONBEP.HDEHLIKBKJG(ONOCIELLAPL);
		}
		else
		{
			ONOCIELLAPL.set_State(CFGBMHKCENK.Queued);
			LCCNAGOGEBC.Add(ONOCIELLAPL);
		}
	}

	private static string NHFCAIIJAHD(HTTPRequest ONOCIELLAPL)
	{
		return ((ONOCIELLAPL.FHGBKFBCGCO() == null) ? string.Empty : new UriBuilder(ONOCIELLAPL.FHGBKFBCGCO().DNIJHGFINDG().Scheme, ONOCIELLAPL.FHGBKFBCGCO().DNIJHGFINDG().Host, ONOCIELLAPL.FHGBKFBCGCO().DNIJHGFINDG().Port).Uri.ToString()) + new UriBuilder(ONOCIELLAPL.DKAECMGPGOE().Scheme, ONOCIELLAPL.DKAECMGPGOE().Host, ONOCIELLAPL.DKAECMGPGOE().Port).Uri.ToString();
	}

	private static HTTPConnection EDFODKJKANN(HTTPRequest ONOCIELLAPL)
	{
		HTTPConnection hPNEPPBEKGG = null;
		string text = NHFCAIIJAHD(ONOCIELLAPL);
		List<HTTPConnection> value;
		if (GKLCMJOHCBJ.TryGetValue(text, out value))
		{
			int num = 0;
			for (int i = 0; i < value.Count; i++)
			{
				if (value[i].OPIAGHNCFAM())
				{
					num++;
				}
			}
			if (num <= BDCIBFLAPJN())
			{
				for (int j = 0; j < value.Count; j++)
				{
					if (hPNEPPBEKGG != null)
					{
						break;
					}
					HTTPConnection hPNEPPBEKGG2 = value[j];
					if (hPNEPPBEKGG2 != null && hPNEPPBEKGG2.PMOPEALOIKF() && (!hPNEPPBEKGG2.AOPIGGFCGHC() || hPNEPPBEKGG2.PHMNCEBDLKP() == null || hPNEPPBEKGG2.PHMNCEBDLKP().Host.Equals(ONOCIELLAPL.DKAECMGPGOE().Host, StringComparison.OrdinalIgnoreCase)))
					{
						hPNEPPBEKGG = hPNEPPBEKGG2;
					}
				}
			}
		}
		else
		{
			GKLCMJOHCBJ.Add(text, value = new List<HTTPConnection>(BDCIBFLAPJN()));
		}
		if (hPNEPPBEKGG == null)
		{
			if (value.Count >= BDCIBFLAPJN())
			{
				return null;
			}
			value.Add(hPNEPPBEKGG = new HTTPConnection(text));
		}
		return hPNEPPBEKGG;
	}

	private static bool EHDFMKOHHLA()
	{
		for (int i = 0; i < LCCNAGOGEBC.Count; i++)
		{
			if (EDFODKJKANN(LCCNAGOGEBC[i]) != null)
			{
				return true;
			}
		}
		return false;
	}

	private static void IOACJAKDNAL(HTTPConnection NNLEEIONBEP)
	{
		NNLEEIONBEP.FFKAKHDIBGD();
		OBMKJIOLKNI.Add(NNLEEIONBEP);
	}

	internal static HTTPConnection IBOHPADLFIM(HTTPRequest ONOCIELLAPL)
	{
		lock (Locker)
		{
			for (int i = 0; i < BPIDLPMODDC.Count; i++)
			{
				HTTPConnection hPNEPPBEKGG = BPIDLPMODDC[i];
				if (hPNEPPBEKGG.ONLLAFBCPIJ() == ONOCIELLAPL)
				{
					return hPNEPPBEKGG;
				}
			}
			return null;
		}
	}

	internal static bool HHHKPIJIAPK(HTTPRequest ONOCIELLAPL)
	{
		return LCCNAGOGEBC.Remove(ONOCIELLAPL);
	}

	internal static string DJHDCCJDJGJ()
	{
		try
		{
			if (OLFBIBGHAAC() != null)
			{
				return OLFBIBGHAAC()();
			}
		}
		catch (Exception mPFFFAOGBJE)
		{
			MBBMPNDDPIH().COHEDILAHFD("HTTPManager", "GetRootCacheFolder", mPFFFAOGBJE);
		}
		return Application.persistentDataPath;
	}

	public static void LCNANNAJNGG()
	{
		lock (Locker)
		{
			NIEABCELPDJ = true;
			try
			{
				for (int i = 0; i < BPIDLPMODDC.Count; i++)
				{
					HTTPConnection hPNEPPBEKGG = BPIDLPMODDC[i];
					switch (hPNEPPBEKGG.FLBBFDNHJAJ())
					{
					case AHFEJIOPFGP.Processing:
						hPNEPPBEKGG.PNCNLDHGDLP();
						if (hPNEPPBEKGG.ONLLAFBCPIJ().MDEPOKKKKCL() && hPNEPPBEKGG.ONLLAFBCPIJ().POGDKNCHIBG() != null && hPNEPPBEKGG.ONLLAFBCPIJ().POGDKNCHIBG().PNOCCDHAAHI())
						{
							hPNEPPBEKGG.ICGOKIADHNK();
						}
						if (((!hPNEPPBEKGG.ONLLAFBCPIJ().MDEPOKKKKCL() && hPNEPPBEKGG.ONLLAFBCPIJ().IHMCGKHBLKN() == null) || hPNEPPBEKGG.ONLLAFBCPIJ().IFPLGJHAANE()) && DateTime.UtcNow - hPNEPPBEKGG.MJFPCJODODA() > hPNEPPBEKGG.ONLLAFBCPIJ().FJKGKLJGIJI())
						{
							hPNEPPBEKGG.AKLEEMEHBIC(AHFEJIOPFGP.TimedOut);
						}
						break;
					case AHFEJIOPFGP.TimedOut:
						if (DateTime.UtcNow - hPNEPPBEKGG.MFFGFLEIPOC() > TimeSpan.FromMilliseconds(500.0))
						{
							MBBMPNDDPIH().KDAFBLAKBMI("HTTPManager", "Hard aborting connection becouse of a long waiting TimedOut state");
							hPNEPPBEKGG.ONLLAFBCPIJ().AOMLIJAIJHE(null);
							hPNEPPBEKGG.ONLLAFBCPIJ().set_State(CFGBMHKCENK.TimedOut);
							hPNEPPBEKGG.ICGOKIADHNK();
							IOACJAKDNAL(hPNEPPBEKGG);
						}
						break;
					case AHFEJIOPFGP.Redirected:
						EMPGOCGHMBI(hPNEPPBEKGG.ONLLAFBCPIJ());
						IOACJAKDNAL(hPNEPPBEKGG);
						break;
					case AHFEJIOPFGP.WaitForRecycle:
						hPNEPPBEKGG.ONLLAFBCPIJ().NOEMFDALAGD();
						hPNEPPBEKGG.ICGOKIADHNK();
						IOACJAKDNAL(hPNEPPBEKGG);
						break;
					case AHFEJIOPFGP.Upgraded:
						hPNEPPBEKGG.ICGOKIADHNK();
						break;
					case AHFEJIOPFGP.WaitForProtocolShutdown:
					{
						IProtocol gFACLEJNACD = hPNEPPBEKGG.ONLLAFBCPIJ().POGDKNCHIBG() as IProtocol;
						if (gFACLEJNACD != null)
						{
							gFACLEJNACD.HandleEvents();
						}
						if (gFACLEJNACD == null || gFACLEJNACD.HDDABMLNDPK())
						{
							hPNEPPBEKGG.ICGOKIADHNK();
							hPNEPPBEKGG.Dispose();
							IOACJAKDNAL(hPNEPPBEKGG);
						}
						break;
					}
					case AHFEJIOPFGP.AbortRequested:
					{
						IProtocol gFACLEJNACD = hPNEPPBEKGG.ONLLAFBCPIJ().POGDKNCHIBG() as IProtocol;
						if (gFACLEJNACD != null)
						{
							gFACLEJNACD.HandleEvents();
							if (gFACLEJNACD.HDDABMLNDPK())
							{
								hPNEPPBEKGG.ICGOKIADHNK();
								hPNEPPBEKGG.Dispose();
								IOACJAKDNAL(hPNEPPBEKGG);
							}
						}
						break;
					}
					case AHFEJIOPFGP.Closed:
						hPNEPPBEKGG.ONLLAFBCPIJ().NOEMFDALAGD();
						hPNEPPBEKGG.ICGOKIADHNK();
						IOACJAKDNAL(hPNEPPBEKGG);
						break;
					}
				}
			}
			finally
			{
				NIEABCELPDJ = false;
			}
			if (OBMKJIOLKNI.Count > 0)
			{
				for (int j = 0; j < OBMKJIOLKNI.Count; j++)
				{
					HTTPConnection hPNEPPBEKGG2 = OBMKJIOLKNI[j];
					if (hPNEPPBEKGG2.PMOPEALOIKF())
					{
						BPIDLPMODDC.Remove(hPNEPPBEKGG2);
						HHGJJICJJOJ.Add(hPNEPPBEKGG2);
					}
				}
				OBMKJIOLKNI.Clear();
			}
			if (HHGJJICJJOJ.Count > 0)
			{
				for (int k = 0; k < HHGJJICJJOJ.Count; k++)
				{
					HTTPConnection hPNEPPBEKGG3 = HHGJJICJJOJ[k];
					if (hPNEPPBEKGG3.KKDOCCALBEG())
					{
						List<HTTPConnection> value = null;
						if (GKLCMJOHCBJ.TryGetValue(hPNEPPBEKGG3.JHAJFMBPEDL(), out value))
						{
							value.Remove(hPNEPPBEKGG3);
						}
						hPNEPPBEKGG3.Dispose();
						HHGJJICJJOJ.RemoveAt(k);
						k--;
					}
				}
			}
			if (EHDFMKOHHLA())
			{
				if (LCCNAGOGEBC.Find((HTTPRequest CGOIOKHEGOE) => CGOIOKHEGOE.KCKAPPJABBL() != 0) != null)
				{
					LCCNAGOGEBC.Sort((HTTPRequest OGGFKJBFCLP, HTTPRequest GEODKIAICBK) => OGGFKJBFCLP.KCKAPPJABBL() - GEODKIAICBK.KCKAPPJABBL());
				}
				HTTPRequest[] array = LCCNAGOGEBC.ToArray();
				LCCNAGOGEBC.Clear();
				for (int num = 0; num < array.Length; num++)
				{
					EMPGOCGHMBI(array[num]);
				}
			}
		}
		if (GCJIMNCMONP != null)
		{
			GCJIMNCMONP.JLPMOKPFECK();
		}
	}

	internal static void HIGDMAPIOON()
	{
		lock (Locker)
		{
			HTTPCacheService.FIMLABMLKJF();
			foreach (KeyValuePair<string, List<HTTPConnection>> item in GKLCMJOHCBJ)
			{
				foreach (HTTPConnection item2 in item.Value)
				{
					item2.Dispose();
				}
				item.Value.Clear();
			}
			GKLCMJOHCBJ.Clear();
		}
	}
}
