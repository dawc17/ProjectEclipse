using System;
using System.IO;
using UnityEngine;

public static class SF2Paths
{
	private const string BLINCLHGPDD = "/gamedata";

	private const string CFJCEPHKHOC = "/userdata";

	private const string LNKJIIGBEDA = "/animations";

	private const string LMEGBHBOMPL = "/animations/binary";

	private const string BPIFJBJBKHA = "/models";

	private const string MHEBDPKJELL = "/localizations";

	private const string IDDGKCDKBIE = "/locations";

	private const string PNFCONFCFHO = "/textures";

	private const string DNFCNIHMHAM = "textures/fullscreen/";

	private const string _Statistics = "/statistics";

	private const string PBEMFHLKNAB = "/news";

	private const string FPKLEIKDAGE = "/bundles";

	private const string LIFGPEIOMIN = "/video";

	private const string MGIADGBHBNI = "/packs.xml";

	private const string IBNJHJBODKG = "Assets/src/GUI/Resources";

	private const string ECEKFJBIBHL = "UI/Items/";

	private const string EBDAHGLNNIL = "UI/Users/";

	private const string DFDNINNMFJF = "UI/Skills/";

	public const string HJMLENEHEJA = "UI/Achievements/";

	public const string COKGDCKBILB = "UI/Fullscreen/";

	public const string FKFCDICFJKF = "textures/Logos/";

	public static string KBOPNEIIDNL = string.Empty;

	public static string FFKEDOBDLOL = string.Empty;

	public static string JKKPDAFGLJL = string.Empty;

	public static bool CGOHPKEBECD = true;

	private static bool LFGMJKBJIEG;

	public static string NPLCFDDGBFN
	{
		get
		{
			return KKIDGPBOBNI();
		}
	}

	public static string HEIDEOHCHJC
	{
		get
		{
			return GBOFOFGDMBN();
		}
	}

	public static string EEOIKBPHJOL
	{
		get
		{
			return MCFPDHOLNGB();
		}
	}

	public static string MJIAIJCJHBK
	{
		get
		{
			return CBKLONCNPCP();
		}
	}

	public static string LNDLFINJHDB
	{
		get
		{
			return BNHLPKEDMOM();
		}
	}

	public static string FPDKKKEMMEC
	{
		get
		{
			return HAHDKJAPIJL();
		}
	}

	public static string DBIBHIGLGGL
	{
		get
		{
			return LFIIMPEAMFG();
		}
	}

	public static string Textures
	{
		get
		{
			return ELPBOBIMAFD();
		}
	}

	public static string EAMILDOGLJA
	{
		get
		{
			return BHCPOOOJAAK();
		}
	}

	public static string IJELKAEELMB
	{
		get
		{
			return KLIDILIHOFF();
		}
	}

	public static string ODBENFNAKOA
	{
		get
		{
			return HIBFNEKOCHC();
		}
	}

	public static string LJILMFAGLOB
	{
		get
		{
			return ENFGGKMDICD();
		}
	}

	public static string OKLDBDJBPDE
	{
		get
		{
			return OCAKEHJCNCC();
		}
	}

	public static string HHNLIOIEAGG
	{
		get
		{
			return APHDBIBDMDG();
		}
	}

	public static string GKKNJKLJAML
	{
		get
		{
			return LCDBGFFDKJB();
		}
	}

	public static string FNHPCBEDKFO
	{
		get
		{
			return GJFFDOJLHGK();
		}
	}

	public static string ELBNBMHBPDP
	{
		get
		{
			return MEKBAHBKMNB();
		}
	}

	public static string NNFMKNJJDDD
	{
		get
		{
			return IDLJHPEDOEH();
		}
	}

	public static string LIAIDGMCBED
	{
		get
		{
			return ECJMHJOMMBC();
		}
	}

	public static bool IOGMEGLBNIJ
	{
		get
		{
			return IANJCHNLMHC();
		}
	}

	public static string KKIDGPBOBNI()
	{
		return KBOPNEIIDNL + "/gamedata";
	}

	public static string GBOFOFGDMBN()
	{
		return JKKPDAFGLJL + "/gamedata";
	}

	public static string MCFPDHOLNGB()
	{
		return KKIDGPBOBNI() + "/animations";
	}

	public static string CBKLONCNPCP()
	{
		return KKIDGPBOBNI() + "/animations/binary";
	}

	public static string BNHLPKEDMOM()
	{
		return KKIDGPBOBNI() + "/models";
	}

	public static string HAHDKJAPIJL()
	{
		return KKIDGPBOBNI() + "/video";
	}

	public static string LFIIMPEAMFG()
	{
		return "UI/Items/";
	}

	public static string ELPBOBIMAFD()
	{
		return "/textures";
	}

	public static string BHCPOOOJAAK()
	{
		return "UI/Users/";
	}

	public static string KLIDILIHOFF()
	{
		return "UI/Skills/";
	}

	public static string HIBFNEKOCHC()
	{
		return "textures/fullscreen/";
	}

	public static string ENFGGKMDICD()
	{
		return KKIDGPBOBNI() + "/localizations";
	}

	public static string OCAKEHJCNCC()
	{
		return KKIDGPBOBNI() + "/locations";
	}

	public static string APHDBIBDMDG()
	{
		return FFKEDOBDLOL + "/userdata";
	}

	public static string LCDBGFFDKJB()
	{
		return GBOFOFGDMBN() + "/statistics";
	}

	public static string GJFFDOJLHGK()
	{
		return GBOFOFGDMBN() + "/news";
	}

	public static string MEKBAHBKMNB()
	{
		return GBOFOFGDMBN() + "/bundles";
	}

	public static string IDLJHPEDOEH()
	{
		return GBOFOFGDMBN() + "/packs.xml";
	}

	public static string ECJMHJOMMBC()
	{
		return "Assets/src/GUI/Resources";
	}

	public static bool IANJCHNLMHC()
	{
		return LFGMJKBJIEG;
	}

	public static void Init()
	{
		if (LFGMJKBJIEG)
		{
			return;
		}
		LFGMJKBJIEG = true;
		KBOPNEIIDNL = string.Empty;
		FFKEDOBDLOL = Application.persistentDataPath;
		JKKPDAFGLJL = Application.persistentDataPath;
		CGOHPKEBECD = true;
		string text = CBFMFIHKMFI();
		if (string.IsNullOrEmpty(FFKEDOBDLOL))
		{
			FFKEDOBDLOL = (JKKPDAFGLJL = text);
		}
		else
		{
			try
			{
				if (!Directory.Exists(FFKEDOBDLOL + "/userdata") && Directory.Exists(text + "/userdata"))
				{
					FFKEDOBDLOL = (JKKPDAFGLJL = text);
				}
			}
			catch
			{
			}
		}
		EJGMLNCEPNC();
	}

	public static void EJGMLNCEPNC()
	{
		if (!Directory.Exists(APHDBIBDMDG()))
		{
			Directory.CreateDirectory(APHDBIBDMDG());
		}
		if (!Directory.Exists(GBOFOFGDMBN()))
		{
			Directory.CreateDirectory(GBOFOFGDMBN());
		}
	}

	public static string COGELDOPEJG(string ONEIGMLOGDC)
	{
		if (ONEIGMLOGDC.Contains(JKKPDAFGLJL))
		{
			return ONEIGMLOGDC;
		}
		return string.Format("{0}/{1}", JKKPDAFGLJL, ONEIGMLOGDC);
	}

	public static void CKCGLNHIDFN()
	{
		if (Directory.Exists(MEKBAHBKMNB()))
		{
			Directory.Delete(MEKBAHBKMNB(), true);
		}
		Directory.CreateDirectory(MEKBAHBKMNB());
	}

	public static string CBFMFIHKMFI()
	{
		string text = string.Empty;
		if (Application.platform != RuntimePlatform.Android || Application.isEditor)
		{
			return text;
		}
		try
		{
			IntPtr javaClass = AndroidJNI.FindClass("android/content/ContextWrapper");
			IntPtr methodID = AndroidJNIHelper.GetMethodID(javaClass, "getFilesDir", "()Ljava/io/File;");
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					IntPtr obj = AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, new jvalue[0]);
					IntPtr javaClass2 = AndroidJNI.FindClass("java/io/File");
					IntPtr methodID2 = AndroidJNIHelper.GetMethodID(javaClass2, "getAbsolutePath", "()Ljava/lang/String;");
					text = AndroidJNI.CallStringMethod(obj, methodID2, new jvalue[0]);
					if (text == null)
					{
						Debug.Log("Using fallback path");
						text = "/data/data/com.nekki.shadowfight/files";
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		return text;
	}
}
