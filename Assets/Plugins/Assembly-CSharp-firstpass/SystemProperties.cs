using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Profiling;

public class SystemProperties
{
	public enum LOHALAKNGFB
	{
		PATH_SMALL = 0,
		PATH_BIG = 1,
		PATH_DEFAULT = -1,
		PATH_NONE = 2
	}

	private static Vector2 EMGIKFNGDMA = new Vector2(1024f, 768f);

	private static Vector2 PALJEJEKBMB = new Vector2(2048f, 1536f);

	private static Vector2 JNMPKENKPJK = new Vector2(1365f, 768f);

	private static DeviceInfo BIFPNNLNKHA = new DeviceInfo();

	private static int _numOfDisplayModes = 0;

	private static List<QualityCondition> AGOHLNDLAEA = new List<QualityCondition>();

	public static Vector2 CALAEHKMGLE = default(Vector2);

	public static float ALOKJEILMLK = 1f;

	public static float NHIDNIPGCPC = 1f;

	public static float PBIEBOJBFMD = 0f;

	public static float GOBOMOJIDOO = 0f;

	private static bool? NMHFJLJCNDH;

	private static string KHFNFFOFKLF
	{
		get
		{
			return DOKBFGCJAGH();
		}
	}

	public static bool CPLBCOBPCCJ
	{
		get
		{
			return DBBOCENKMGD();
		}
	}

	public static bool FIEAMGMOEGL
	{
		get
		{
			return FHHPHDIBEFM();
		}
	}

	public static bool IIGNAAPBDFE
	{
		get
		{
			return NDEPIDFFOBF();
		}
	}

	public static int ILLOKFLDDON
	{
		get
		{
			return NPDPKLMFBHH();
		}
	}

	public static int LCAJCJFBJDM
	{
		get
		{
			return MCGOBLKFGHO();
		}
	}

	public static int CLBGKICNMIH
	{
		get
		{
			return OACFGEDMCOD();
		}
	}

	public static bool ODDGLIEHGFK
	{
		get
		{
			return LHGPKEFEHDH();
		}
	}

	public static bool PPHNHLODNIN
	{
		get
		{
			return PPFPHAKMNLC();
		}
	}

	public static bool MHDCIGHDIME
	{
		get
		{
			return MEBGOGMJFLM();
		}
	}

	public static bool DAAKJIIDJBG
	{
		get
		{
			return IPJFCBAGMJJ();
		}
	}

	public static bool HDEKJHLMKPC
	{
		get
		{
			return CEJMCBKCPOH();
		}
	}

	public static bool LANIPINOLAH
	{
		get
		{
			return AOJIOMDCEKN();
		}
	}

	public static bool DDEOPDADDHK
	{
		get
		{
			return AFKGHBJPLOK();
		}
	}

	public static bool HFPHMLJKIIH
	{
		get
		{
			return NFFOJCHNPJD();
		}
	}

	public static bool FANDPECGOFF
	{
		get
		{
			return DDIDANINPJE();
		}
	}

	public static bool OEPPKLJBEIP
	{
		get
		{
			return GEBFGBAJMIE();
		}
	}

	public static bool DCHNPKKJFPG
	{
		get
		{
			return GAAMHGCDANB();
		}
	}

	public static bool OKNGOOIIOAB
	{
		get
		{
			return PKLFCFBEIIG();
		}
	}

	public static bool OMHPPHBPFDO
	{
		get
		{
			return DCKPKCIFOAG();
		}
	}

	public static bool JGLKJECFHED
	{
		get
		{
			return FBGNIKBPCFB();
		}
	}

	public static bool IPNLEGJFHEB
	{
		get
		{
			return MHOKHLIDJNJ();
		}
	}

	public static string HIBGNCGMLKI
	{
		get
		{
			return IAAKNCJMAAK();
		}
	}

	public static string DOBDPPAJEMM
	{
		get
		{
			return ICMOGAMDEMM();
		}
	}

	public static string BCFGMMPPNPL
	{
		get
		{
			return GLLJKPBHELE();
		}
	}

	public static string BHPFHBHJCFD
	{
		get
		{
			return IJOILMDCIMI();
		}
	}

	public static string ECGBLGBPFMK
	{
		get
		{
			return DBKBHEMJLLC();
		}
		set
		{
			ICMBPCDMDIP(value);
		}
	}

	public static string DFIPKAIDIDE
	{
		get
		{
			return IIILDACELJP();
		}
	}

	public static string PPCFIFIMOEG
	{
		get
		{
			return CFEDCPDNICD();
		}
	}

	public static long NHHNLGMHJBA
	{
		get
		{
			return JOFIGLFDPDE();
		}
	}

	public static bool NCJMBJBIGCK
	{
		get
		{
			return AFAAJMFLBIC();
		}
	}

	public static string OONBPJPAHEO
	{
		get
		{
			return OKLHMDPCGJL();
		}
	}

	public static int[] AHMCBLFHKDD
	{
		get
		{
			return LFICEOIFOMI();
		}
		set
		{
			set_UnconfirmedLedgerIDs(value);
		}
	}

	public static string OLHCEJKIIOB
	{
		get
		{
			return OBKPEDOHCOO();
		}
	}

	public static VersionContainer OGMKHOHOGPD
	{
		get
		{
			return DFJEJKJECBI();
		}
	}

	public static VersionContainer Version
	{
		get
		{
			return KCJMMIEBLHL();
		}
	}

	private static string DOKBFGCJAGH()
	{
		return "-UP";
	}

	public static string MakeIdentifier(string OONGHHGHHFG)
	{
		if (!string.IsNullOrEmpty(OONGHHGHHFG))
		{
			return OONGHHGHHFG + DOKBFGCJAGH();
		}
		return OONGHHGHHFG;
	}

	public static bool DBBOCENKMGD()
	{
		return false;
	}

	public static bool FHHPHDIBEFM()
	{
		return true;
	}

	public static bool NDEPIDFFOBF()
	{
		return true;
	}

	public static bool FDENJPADIDJ()
	{
		return false;
	}

	public static int NPDPKLMFBHH()
	{
		return _numOfDisplayModes;
	}

	public static void CMEKDMFKDEO(int NMMPBADCFHK)
	{
		Vector2 eMGIKFNGDMA = EMGIKFNGDMA;
		ALOKJEILMLK = (float)BIFPNNLNKHA.LBKMKDKDFJF / eMGIKFNGDMA.x;
		NHIDNIPGCPC = (float)BIFPNNLNKHA.LLFOGEMDMJD / JNMPKENKPJK.y;
		PBIEBOJBFMD = (float)BIFPNNLNKHA.LBKMKDKDFJF / NHIDNIPGCPC;
		GOBOMOJIDOO = (float)BIFPNNLNKHA.LLFOGEMDMJD / NHIDNIPGCPC;
	}

	public static int MCGOBLKFGHO()
	{
		return Screen.width;
	}

	public static int OACFGEDMCOD()
	{
		return Screen.height;
	}

	public static void HBCGFAKAJOA()
	{
		BIFPNNLNKHA.Id = SystemInfo.deviceModel;
		LLLOJBFMONN.Write(BIFPNNLNKHA.Id);
		BIFPNNLNKHA.ODDGLIEHGFK = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
		BIFPNNLNKHA.PPHNHLODNIN = Application.platform == RuntimePlatform.WindowsEditor;
		BIFPNNLNKHA.DHPGNIFEOPI = JBBJGPNELBK();
		BIFPNNLNKHA.MDPBKNDOGKJ = SystemInfo.operatingSystem;
		BIFPNNLNKHA.OAPHJAPMKJG = PreciseLocale.BGMAJFGKCEB();
		BIFPNNLNKHA.MOMPODBNJNE = SystemInfo.processorCount;
		BIFPNNLNKHA.AOJLHDILEBJ = (int)((float)SystemInfo.systemMemorySize / 1024f);
		BIFPNNLNKHA.LGEEAANABHH = (int)((float)(SystemInfo.systemMemorySize - Profiler.GetTotalAllocatedMemoryLong()) / 1024f);
		BIFPNNLNKHA.DFIPKAIDIDE = SystemInfo.deviceUniqueIdentifier;
		BIFPNNLNKHA.FFNCAFDPLPL = SFSocial.GBPBIPFIOJH().HBPJFLOFIJO();
		if (AFKGHBJPLOK())
		{
			int length = BIFPNNLNKHA.Id.IndexOf('_');
			BIFPNNLNKHA.Id = BIFPNNLNKHA.Id.Substring(0, length);
		}
	}

	private static string JBBJGPNELBK()
	{
		if (IPJFCBAGMJJ())
		{
			string operatingSystem = SystemInfo.operatingSystem;
			int num = operatingSystem.IndexOf("(");
			if (num == -1)
			{
				return SystemInfo.operatingSystem;
			}
			return operatingSystem.Substring(0, num - 1);
		}
		return SystemInfo.operatingSystem;
	}

	public static void GOCLBADJDGK(XmlDocument JFJPKEONJIJ)
	{
		ICNACCADGEJ(JFJPKEONJIJ);
		LOHALAKNGFB lOHALAKNGFB = DMCFAGPGNIE();
		if (lOHALAKNGFB == LOHALAKNGFB.PATH_DEFAULT)
		{
			lOHALAKNGFB = (FKPHLDIBPLO() ? LOHALAKNGFB.PATH_BIG : LOHALAKNGFB.PATH_SMALL);
		}
		OBNFPIPOHMH(lOHALAKNGFB);
		LOHALAKNGFB lOHALAKNGFB2 = JGBFPENNILG();
		if (lOHALAKNGFB2 == LOHALAKNGFB.PATH_DEFAULT)
		{
			lOHALAKNGFB2 = (FKPHLDIBPLO() ? LOHALAKNGFB.PATH_BIG : LOHALAKNGFB.PATH_SMALL);
		}
		SetInverseLocationScale((lOHALAKNGFB2 != LOHALAKNGFB.PATH_SMALL) ? 1 : 2);
		Vector2 eMGIKFNGDMA = EMGIKFNGDMA;
		ALOKJEILMLK = (float)MCGOBLKFGHO() / eMGIKFNGDMA.x;
		NHIDNIPGCPC = (float)OACFGEDMCOD() / JNMPKENKPJK.y;
		PBIEBOJBFMD = (float)MCGOBLKFGHO() / NHIDNIPGCPC;
		GOBOMOJIDOO = (float)OACFGEDMCOD() / NHIDNIPGCPC;
	}

	public static bool LHGPKEFEHDH()
	{
		return BIFPNNLNKHA.ODDGLIEHGFK;
	}

	public static bool PPFPHAKMNLC()
	{
		return BIFPNNLNKHA.PPHNHLODNIN;
	}

	public static bool MEBGOGMJFLM()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer;
	}

	public static bool IPJFCBAGMJJ()
	{
		return Application.platform == RuntimePlatform.Android;
	}

	public static bool CEJMCBKCPOH()
	{
		return Application.platform == RuntimePlatform.WindowsPlayer;
	}

	public static bool AOJIOMDCEKN()
	{
		return Application.platform == RuntimePlatform.OSXPlayer;
	}

	public static bool AFKGHBJPLOK()
	{
		return Application.platform == RuntimePlatform.MetroPlayerARM;
	}

	public static bool NFFOJCHNPJD()
	{
		return Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64;
	}

	public static bool DDIDANINPJE()
	{
		return Application.platform == RuntimePlatform.WP8Player;
	}

	public static bool GEBFGBAJMIE()
	{
		return Application.platform == RuntimePlatform.TizenPlayer;
	}

	public static bool GAAMHGCDANB()
	{
		return Application.isMobilePlatform;
	}

	public static bool PKLFCFBEIIG()
	{
		return true;
	}

	public static bool DCKPKCIFOAG()
	{
		return PKLFCFBEIIG();
	}

	public static bool FBGNIKBPCFB()
	{
		return BIFPNNLNKHA.JGLKJECFHED;
	}

	public static bool MHOKHLIDJNJ()
	{
		return (float)Screen.width / (float)Screen.height < 1.66f;
	}

	public static bool FKPHLDIBPLO()
	{
		return (float)BIFPNNLNKHA.LLFOGEMDMJD > EMGIKFNGDMA.y;
	}

	public static LOHALAKNGFB DMCFAGPGNIE()
	{
		return BIFPNNLNKHA.ACHKMBJANGN;
	}

	public static LOHALAKNGFB JGBFPENNILG()
	{
		return BIFPNNLNKHA.BLIOAMODNOH;
	}

	public static void NHIDOHIJMBG(int value)
	{
	}

	public static string IAAKNCJMAAK()
	{
		if (NFFOJCHNPJD())
		{
			return "winstore";
		}
		if (AFKGHBJPLOK() || DDIDANINPJE())
		{
			return "win";
		}
		if (IPJFCBAGMJJ())
		{
			return "and";
		}
		if (MEBGOGMJFLM())
		{
			return "ios";
		}
		if (LHGPKEFEHDH())
		{
			return "tst";
		}
		if (AOJIOMDCEKN())
		{
			return "mac";
		}
		return "unk";
	}

	public static string ICMOGAMDEMM()
	{
		return string.Format("{0}{1}", IAAKNCJMAAK(), (!FBGNIKBPCFB()) ? "_phone" : "_pad");
	}

	public static string GLLJKPBHELE()
	{
		string text = BPGAOEMIFNN.OBGMKPLOMJL();
		if (string.IsNullOrEmpty(text))
		{
			text = SystemInfo.deviceUniqueIdentifier;
		}
		return text;
	}

	public static string IJOILMDCIMI()
	{
		return string.Format("{0}_{1}", IAAKNCJMAAK(), GLLJKPBHELE());
	}

	public static DeviceInfo NICPICAMAOH()
	{
		return BIFPNNLNKHA;
	}

	public static string DCPMKCGDHPJ(LOHALAKNGFB EBIGIKHLFNL)
	{
		switch (EBIGIKHLFNL)
		{
		case LOHALAKNGFB.PATH_DEFAULT:
			return (!FKPHLDIBPLO()) ? "LOW" : "HIGH";
		case LOHALAKNGFB.PATH_BIG:
			return "HIGH";
		case LOHALAKNGFB.PATH_SMALL:
			return "LOW";
		default:
			return "DEFAULT";
		}
	}

	public static LOHALAKNGFB PLKALGPCALI(string name)
	{
		switch (name)
		{
		case "DEFAULT":
			return LOHALAKNGFB.PATH_DEFAULT;
		case "LOW":
			return LOHALAKNGFB.PATH_SMALL;
		case "HIGH":
			return LOHALAKNGFB.PATH_BIG;
		default:
			LLLOJBFMONN.Error("ERROR: SystemProperties::getPathType - %s", name);
			return LOHALAKNGFB.PATH_DEFAULT;
		}
	}

	public static string DBKBHEMJLLC()
	{
		string text = BIFPNNLNKHA.CJPJNFFJNGN;
		if (text == null)
		{
			text = SystemInfo.deviceUniqueIdentifier;
		}
		return MakeIdentifier(text);
	}

	public static void ICMBPCDMDIP(string value)
	{
		BIFPNNLNKHA.CJPJNFFJNGN = value;
	}

	public static void BFBMCAALLHF(VersionContainer version, VersionContainer JJCDPPFGPDO)
	{
		BIFPNNLNKHA.Version = version;
		BIFPNNLNKHA.OGMKHOHOGPD = JJCDPPFGPDO;
	}

	public static string HBPJFLOFIJO()
	{
		BIFPNNLNKHA.FFNCAFDPLPL = SFSocial.GBPBIPFIOJH().HBPJFLOFIJO();
		return BIFPNNLNKHA.FFNCAFDPLPL;
	}

	public static string IIILDACELJP()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}

	public static string CFEDCPDNICD()
	{
		return BIFPNNLNKHA.DHPGNIFEOPI;
	}

	public static long JOFIGLFDPDE()
	{
		return (long)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalSeconds;
	}

	public static void Clear()
	{
		AGOHLNDLAEA.Clear();
	}

	public static string PMAODLMLDLK()
	{
		if (!BIFPNNLNKHA.QualityCondition.BKOIKMEEHDK())
		{
			return BIFPNNLNKHA.QualityCondition;
		}
		if (NFFOJCHNPJD())
		{
			return AGOHLNDLAEA[0].get_Name();
		}
		foreach (QualityCondition item in AGOHLNDLAEA)
		{
			if (item.CEHMBJOALEM())
			{
				return item.get_Name();
			}
		}
		return string.Empty;
	}

	public static void SetInverseLocationScale(float value)
	{
		BIFPNNLNKHA.InverseLocationScale = value;
	}

	private static void ICNACCADGEJ(XmlDocument EELFNMOHGJL)
	{
		Clear();
		HJKFCAIBCPP();
		BIFPNNLNKHA.JGLKJECFHED = FKPHLDIBPLO();
		BIFPNNLNKHA.ACHKMBJANGN = PLKALGPCALI("DEFAULT");
		BIFPNNLNKHA.BLIOAMODNOH = PLKALGPCALI("DEFAULT");
		XmlElement xmlElement = EELFNMOHGJL["Root"];
		if (xmlElement == null)
		{
			return;
		}
		XmlNode xmlNode = xmlElement["Config"];
		if (xmlNode != null)
		{
			float f = (float)Screen.width / Screen.dpi;
			float f2 = (float)Screen.height / Screen.dpi;
			float num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
			float num2 = float.Parse(xmlNode["TabletDiagonal"].Attributes["Value"].Value);
			BIFPNNLNKHA.JGLKJECFHED = num >= num2;
		}
		XmlElement xmlElement2 = xmlElement["Devices"];
		if (xmlElement2 != null)
		{
			DeviceInfoForcibly kGDCPJPEKKE = new DeviceInfoForcibly();
			for (int i = 0; i < xmlElement2.ChildNodes.Count; i++)
			{
				if (xmlElement2.ChildNodes[i].Attributes == null)
				{
					continue;
				}
				if (((xmlElement2.Attributes["Forcibly"] != null && int.Parse(xmlElement2.Attributes["Forcibly"].Value) != 0) ? 1 : 0) > (false ? 1 : 0))
				{
					if (xmlElement2.ChildNodes[i].Attributes["Tablet"] != null)
					{
						kGDCPJPEKKE.BFDKILHELJA = xmlElement2.ChildNodes[i].Attributes["Tablet"].Value;
					}
					if (xmlElement2.ChildNodes[i].Attributes["Resolution"] != null)
					{
						kGDCPJPEKKE.MIKMPEHBLBN = xmlElement2.ChildNodes[i].Attributes["Resolution"].Value;
					}
					if (xmlElement2.ChildNodes[i].Attributes["LocationResolution"] != null)
					{
						kGDCPJPEKKE.FNAJOKNINLA = xmlElement2.ChildNodes[i].Attributes["LocationResolution"].Value;
					}
					if (xmlElement2.ChildNodes[i].Attributes["QualityCondition"] != null)
					{
						kGDCPJPEKKE.HEPNIDFNHBA = xmlElement2.ChildNodes[i].Attributes["QualityCondition"].Value;
					}
				}
				if (BIFPNNLNKHA.Id == xmlElement2.ChildNodes[i].Attributes["Name"].Value)
				{
					BIFPNNLNKHA.JGLKJECFHED = xmlElement2.ChildNodes[i].Attributes["Tablet"] != null && int.Parse(xmlElement2.ChildNodes[i].Attributes["Tablet"].Value) > 0;
					string gOHIIMFFFJI = ((xmlElement2.Attributes["Resolution"] == null) ? "DEFAULT" : xmlNode.Attributes["Resolution"].Value);
					BIFPNNLNKHA.ACHKMBJANGN = PLKALGPCALI(gOHIIMFFFJI);
					string gOHIIMFFFJI2 = ((xmlElement2.Attributes["LocationResolution"] == null) ? "DEFAULT" : xmlNode.Attributes["LocationResolution"].Value);
					BIFPNNLNKHA.BLIOAMODNOH = PLKALGPCALI(gOHIIMFFFJI2);
					BIFPNNLNKHA.QualityCondition = ((xmlElement2.Attributes["QualityCondition"] == null) ? string.Empty : xmlNode.Attributes["QualityCondition"].Value);
				}
			}
			if (!kGDCPJPEKKE.KLNLNKBIDGD())
			{
				if (!string.IsNullOrEmpty(kGDCPJPEKKE.BFDKILHELJA))
				{
					BIFPNNLNKHA.JGLKJECFHED = int.Parse(kGDCPJPEKKE.BFDKILHELJA) > 0;
				}
				if (kGDCPJPEKKE.MIKMPEHBLBN != string.Empty)
				{
					BIFPNNLNKHA.ACHKMBJANGN = PLKALGPCALI(kGDCPJPEKKE.MIKMPEHBLBN);
				}
				if (kGDCPJPEKKE.FNAJOKNINLA != string.Empty)
				{
					BIFPNNLNKHA.BLIOAMODNOH = PLKALGPCALI(kGDCPJPEKKE.FNAJOKNINLA);
				}
				if (kGDCPJPEKKE.HEPNIDFNHBA != string.Empty)
				{
					BIFPNNLNKHA.QualityCondition = kGDCPJPEKKE.HEPNIDFNHBA;
				}
			}
		}
		ParseQualityConditions(xmlElement["QualityConditions"]);
	}

	private static void OBNFPIPOHMH(LOHALAKNGFB LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case LOHALAKNGFB.PATH_SMALL:
			CALAEHKMGLE = EMGIKFNGDMA;
			return;
		case LOHALAKNGFB.PATH_BIG:
			CALAEHKMGLE = PALJEJEKBMB;
			return;
		}
		LLLOJBFMONN.Error("ERROR: SystemProperties::setPicturePaths - %i", LFLGCDNKNJI);
		CALAEHKMGLE = EMGIKFNGDMA;
	}

	private static void HJKFCAIBCPP()
	{
		int width = Screen.currentResolution.width;
		int height = Screen.currentResolution.height;
		if (height < width)
		{
			BIFPNNLNKHA.LBKMKDKDFJF = width;
			BIFPNNLNKHA.LLFOGEMDMJD = height;
		}
		else
		{
			BIFPNNLNKHA.LBKMKDKDFJF = height;
			BIFPNNLNKHA.LLFOGEMDMJD = width;
		}
	}

	private static void ParseQualityConditions(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			QualityCondition item = new QualityCondition(childNode);
			AGOHLNDLAEA.Add(item);
		}
	}

	public static bool AFAAJMFLBIC()
	{
		if (!NMHFJLJCNDH.HasValue)
		{
			NMHFJLJCNDH = Application.identifier == "com.nekki.shadowfight2.paid" || Application.identifier == "com.nekki.shadowfight.paid";
		}
		return NMHFJLJCNDH.Value;
	}

	public static string OKLHMDPCGJL()
	{
		if (MEBGOGMJFLM())
		{
			return GLLJKPBHELE();
		}
		if (IPJFCBAGMJJ())
		{
			if (!PlayerPrefs.HasKey("AndroidGUID"))
			{
				PlayerPrefs.SetString("AndroidGUID", Guid.NewGuid().ToString());
			}
			return PlayerPrefs.GetString("AndroidGUID");
		}
		if (LHGPKEFEHDH() || CEJMCBKCPOH() || AOJIOMDCEKN())
		{
			if (!PlayerPrefs.HasKey("EmulatorGUID"))
			{
				PlayerPrefs.SetString("EmulatorGUID", Guid.NewGuid().ToString());
			}
			return PlayerPrefs.GetString("EmulatorGUID");
		}
		return null;
	}

	public static int[] LFICEOIFOMI()
	{
		string text = PlayerPrefs.GetString("UnconfirmedLedgerIDs", null);
		List<int> list = new List<int>();
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(',');
			foreach (string s in array)
			{
				int result;
				if (int.TryParse(s, out result))
				{
					list.Add(result);
				}
			}
		}
		return list.ToArray();
	}

	public static void set_UnconfirmedLedgerIDs(int[] value)
	{
		PlayerPrefs.SetString("UnconfirmedLedgerIDs", string.Join(",", value.Select((int OKNNNLIPODI) => OKNNNLIPODI.ToString()).ToArray()));
	}

	public static string OBKPEDOHCOO()
	{
		return SystemInfo.deviceModel;
	}

	public static VersionContainer DFJEJKJECBI()
	{
		return BIFPNNLNKHA.OGMKHOHOGPD;
	}

	public static VersionContainer KCJMMIEBLHL()
	{
		return BIFPNNLNKHA.Version;
	}
}
