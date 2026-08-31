using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using UnityEngine;

public static class LocalizationManager
{
public class Language
	{
		public string name;

		public string PMFEIPCHENB;

		public string MMBELNEBNBM;

		public string OKGJAMBPDGO;

		public string EOMNCDDELLB;

		public string LOKLDPLAPOL;

		public int index;

		public string JDCGKDHJLIC;

		public string LEJJPMMGPAO;

		public bool MINNJBMGKLL;

		public Font HKHFNJNDEND;

		public Font APIDPJICKBC;

		public Font LOICIDHBPMO;

		public string KPCPLLIGKCF;

		public string DPPMDKCHIOA;

		public string DPEBJGKEPOE;

		public float BJFEJHEJJHG = 1f;

		public float EDCKBHNGOHP = 1f;

		public float EAALHHKBGHN = 1f;

		public Language(XmlNode MEEAKLDGLDF, int DCHCFFFFLLK)
		{
			name = MEEAKLDGLDF.Attributes["Name"].CIPOICEEIBK("Name");
			EOMNCDDELLB = MEEAKLDGLDF.Attributes["Locale"].CIPOICEEIBK("Locale");
			PMFEIPCHENB = SF2Paths.ENFGGKMDICD() + "/" + name + ".xml";
			MMBELNEBNBM = "SettingsButtons." + MEEAKLDGLDF.Attributes["FileIcon"].CIPOICEEIBK("FileIcon");
			if (!MEEAKLDGLDF.Attributes["FileIconSelected"].Empty())
			{
				OKGJAMBPDGO = "SettingsButtons." + MEEAKLDGLDF.Attributes["FileIconSelected"].CIPOICEEIBK(string.Empty);
			}
			index = DCHCFFFFLLK;
			LOKLDPLAPOL = MEEAKLDGLDF.Attributes["Alias"].CIPOICEEIBK("Alias");
			JDCGKDHJLIC = MEEAKLDGLDF.Attributes["LoaderImage"].CIPOICEEIBK("logo");
			LEJJPMMGPAO = MEEAKLDGLDF.Attributes["PreloaderImage"].CIPOICEEIBK();
			MINNJBMGKLL = MEEAKLDGLDF.Attributes["IsAsian"].ParseBool();
			if (MEEAKLDGLDF["Fonts"] != null)
			{
				JAEOIDEFOIJ(MEEAKLDGLDF["Fonts"], ref KPCPLLIGKCF, ref DPPMDKCHIOA, ref DPEBJGKEPOE, ref HKHFNJNDEND, ref APIDPJICKBC, ref LOICIDHBPMO);
				BJFEJHEJJHG = MEEAKLDGLDF["Fonts"].Attributes["FontSizeScale"].ParseFloat(1f);
				EDCKBHNGOHP = MEEAKLDGLDF["Fonts"].Attributes["LineSpacing"].ParseFloat(1f);
				EAALHHKBGHN = MEEAKLDGLDF["Fonts"].Attributes["CustomLineSpacingScale"].ParseFloat(1f);
			}
		}

		public void GHHCAJDOLFL()
		{
			if (HKHFNJNDEND == null || DPPMDKCHIOA == null || DPEBJGKEPOE == null)
			{
				JAEOIDEFOIJ(KPCPLLIGKCF, DPPMDKCHIOA, DPEBJGKEPOE, ref HKHFNJNDEND, ref APIDPJICKBC, ref LOICIDHBPMO);
			}
		}
	}

	private static Dictionary<string, string> KCAMAIIHKKH;

	private const string OJPIOFPLHME = "UI/Fonts/";

	public static bool FJLMLAGEJDL;

	public static string POIPGLLCCKC;

	public static List<Language> MCLNNPPCFFL;

	public static Language ILAJKOBCHFH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static Action OnLanguageChanged;

	private static string CBHGODGIABJ;

	private static Font KEAMIDNCCNN;

	private static string DJFPOPBPFGC;

	private static Font IELPAJFFCIC;

	private static string GBALNGFGCPI;

	private static Font FKMPNFBPKCF;

	public static Font HDOEFFOAEMP
	{
		get
		{
			return MBPJIKFOEBJ();
		}
	}

	public static Font OKDNDPCNJNO
	{
		get
		{
			return GNIENOIHLNO();
		}
	}

	public static Font FJOGJJAKLGG
	{
		get
		{
			return DIJFGLJHDBI();
		}
	}

	public static float DMCFJFPGJNE
	{
		get
		{
			return GCBEBEGKAOE();
		}
	}

	public static float PNHLDAKNBOI
	{
		get
		{
			return DLGKFIICJMG();
		}
	}

	public static float ALODDAAKCEB
	{
		get
		{
			return OKIIEMCLAHH();
		}
	}

	public static bool MMJAKDAEGDM
	{
		get
		{
			return KGEOCPBDJIF();
		}
	}

	public static event Action OCLBJLPOKLB
	{
		add
		{
			LKFNMDCLMCD(value);
		}
		remove
		{
			FFIJPHDLPCF(value);
		}
	}

	static LocalizationManager()
	{
		FJLMLAGEJDL = false;
	}

	public static void LKFNMDCLMCD(Action value)
	{
		Action action = OnLanguageChanged;
		Action action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnLanguageChanged, (Action)Delegate.Combine(action2, value), action);
		}
		while ((object)action != action2);
	}

	public static void FFIJPHDLPCF(Action value)
	{
		Action action = OnLanguageChanged;
		Action action2;
		do
		{
			action2 = action;
			action = Interlocked.CompareExchange(ref OnLanguageChanged, (Action)Delegate.Remove(action2, value), action);
		}
		while ((object)action != action2);
	}

	public static Font MBPJIKFOEBJ()
	{
		if (ILAJKOBCHFH != null)
		{
			if (ILAJKOBCHFH.HKHFNJNDEND != null)
			{
				return ILAJKOBCHFH.HKHFNJNDEND;
			}
		}
		if (KEAMIDNCCNN == null)
		{
			KEAMIDNCCNN = ResourcesAndBundles.Load<Font>("UI/Fonts/majallab");
		}
		return KEAMIDNCCNN;
	}

	// Thin Eclipse modding seam. External aliases are namespaced and are reapplied after the
	// recovered language loader clears/rebuilds this dictionary.
	public static void SetExternalString(string key, string value)
	{
		if (KCAMAIIHKKH == null)
		{
			throw new InvalidOperationException("LocalizationManager is not initialized.");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("External localization key must not be empty.", "key");
		}
		KCAMAIIHKKH[key] = value ?? string.Empty;
	}

	public static void RemoveExternalString(string key)
	{
		if (KCAMAIIHKKH != null && !string.IsNullOrEmpty(key))
		{
			KCAMAIIHKKH.Remove(key);
		}
	}

	public static Font GNIENOIHLNO()
	{
		if (ILAJKOBCHFH != null)
		{
			if (ILAJKOBCHFH.APIDPJICKBC != null)
			{
				return ILAJKOBCHFH.APIDPJICKBC;
			}
		}
		if (IELPAJFFCIC == null)
		{
			IELPAJFFCIC = ResourcesAndBundles.Load<Font>("UI/Fonts/majallab");
		}
		return IELPAJFFCIC;
	}

	public static Font DIJFGLJHDBI()
	{
		if (ILAJKOBCHFH != null)
		{
			if (ILAJKOBCHFH.LOICIDHBPMO != null)
			{
				return ILAJKOBCHFH.LOICIDHBPMO;
			}
		}
		if (FKMPNFBPKCF == null)
		{
			FKMPNFBPKCF = ResourcesAndBundles.Load<Font>("UI/Fonts/majallab");
		}
		return FKMPNFBPKCF;
	}

	public static float GCBEBEGKAOE()
	{
		if (ILAJKOBCHFH != null)
		{
			return ILAJKOBCHFH.BJFEJHEJJHG;
		}
		return 1f;
	}

	public static float DLGKFIICJMG()
	{
		if (ILAJKOBCHFH != null)
		{
			return ILAJKOBCHFH.EDCKBHNGOHP;
		}
		return 1f;
	}

	public static float OKIIEMCLAHH()
	{
		if (ILAJKOBCHFH != null)
		{
			return ILAJKOBCHFH.EAALHHKBGHN;
		}
		return 1f;
	}

	public static bool KGEOCPBDJIF()
	{
		return ILAJKOBCHFH != null;
	}

	public static void Init()
	{
		MCLNNPPCFFL = new List<Language>();
		KCAMAIIHKKH = new Dictionary<string, string>();
		OEBFNIGOPDB();
		foreach (Language item in MCLNNPPCFFL)
		{
			if (item.name == POIPGLLCCKC)
			{
				ILAJKOBCHFH = item;
				break;
			}
		}
		NADENLPLKGC(null, false);
	}

	public static void OEBFNIGOPDB()
	{
		string text = "/localization.xml";
		string text2 = SF2Paths.KKIDGPBOBNI();
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(text2 + text, string.Empty);
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["Localization"]["DefaultFonts"];
			if (xmlNode != null)
			{
				JAEOIDEFOIJ(xmlNode, ref CBHGODGIABJ, ref DJFPOPBPFGC, ref GBALNGFGCPI, ref KEAMIDNCCNN, ref IELPAJFFCIC, ref FKMPNFBPKCF);
				if (KEAMIDNCCNN == null || IELPAJFFCIC == null || FKMPNFBPKCF == null)
				{
					LLLOJBFMONN.Error(string.Format("ERROR: LoadFonts: one or more defalut font is missing"));
				}
			}
			else
			{
				LLLOJBFMONN.Error("ERROR: LocalizationManager.LoadLocalization - wrong file");
			}
			XmlNode xmlNode2 = xmlDocument["Localization"]["Languages"];
			if (xmlNode2 != null)
			{
				COAMAKOABOP(xmlNode2);
			}
			else
			{
				LLLOJBFMONN.Error("ERROR: LocalizationManager.LoadLocalization - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Error(string.Format("ERROR: LoadLocalization - file \"{0}\" doesn't exist", text2 + text));
		}
	}

	private static void JAEOIDEFOIJ(XmlNode HPGOCHNDPOO, ref string LICDEKGKFOG, ref string PGFDIINNPIP, ref string KIAADBBGNOI, ref Font CCJIANGGFEF, ref Font DHOMOPOLLGH, ref Font PDENGPGFJOB)
	{
		LICDEKGKFOG = HPGOCHNDPOO.Attributes["ContentFont"].CIPOICEEIBK(string.Empty);
		PGFDIINNPIP = HPGOCHNDPOO.Attributes["TitleFont"].CIPOICEEIBK(string.Empty);
		KIAADBBGNOI = HPGOCHNDPOO.Attributes["ButtonFont"].CIPOICEEIBK(string.Empty);
		JAEOIDEFOIJ(LICDEKGKFOG, PGFDIINNPIP, KIAADBBGNOI, ref CCJIANGGFEF, ref DHOMOPOLLGH, ref PDENGPGFJOB);
	}

	private static void JAEOIDEFOIJ(string LICDEKGKFOG, string PGFDIINNPIP, string KIAADBBGNOI, ref Font CCJIANGGFEF, ref Font DHOMOPOLLGH, ref Font PDENGPGFJOB)
	{
		CCJIANGGFEF = ResourcesAndBundles.Load<Font>("UI/Fonts/" + LICDEKGKFOG);
		DHOMOPOLLGH = ResourcesAndBundles.Load<Font>("UI/Fonts/" + PGFDIINNPIP);
		PDENGPGFJOB = ResourcesAndBundles.Load<Font>("UI/Fonts/" + KIAADBBGNOI);
	}

	private static void COAMAKOABOP(XmlNode DAENMBIHKEB)
	{
		POIPGLLCCKC = DAENMBIHKEB.Attributes["Default"].CIPOICEEIBK(string.Empty);
		MCLNNPPCFFL.Clear();
		int num = 0;
		foreach (XmlNode childNode in DAENMBIHKEB.ChildNodes)
		{
			Language item = new Language(childNode, num);
			MCLNNPPCFFL.Add(item);
			num++;
		}
	}

	public static string GetString(string PEMOECLNECD, params string[] JCICKLIMBEF)
	{
		if (PEMOECLNECD == null || ILAJKOBCHFH == null)
		{
			return string.Empty;
		}
		List<string> list = new List<string>(JCICKLIMBEF);
		string key = PEMOECLNECD;
		bool flag = false;
		int num = PEMOECLNECD.IndexOf("{");
		if (num != -1)
		{
			flag = true;
			// Several original aliases are authored as "title {value}".  The
			// separating space is presentation syntax, not part of the localization
			// key.  Keeping it made valid entries such as "replays {999}" look up
			// "replays " and spam a false missing-localization error.
			key = PEMOECLNECD.Substring(0, num).TrimEnd();
		}
		if (!KCAMAIIHKKH.ContainsKey(key))
		{
			if (PEMOECLNECD != string.Empty)
			{
				LLLOJBFMONN.Error(string.Format("ERROR: localization does not contain title \"{0}\"", PEMOECLNECD));
			}
			return "%%ERROR%%";
		}
		string text = KCAMAIIHKKH[key];
		// Newer localization files use {br} as an explicit line break.  The
		// original formatter treated every brace token as a numeric argument;
		// int.TryParse("br") therefore became argument zero and produced strings
		// such as "EXPERIENCE6060/190".
		text = text.Replace("{br}", "\n").Replace("{BR}", "\n");
		if (flag)
		{
			if (list.Count != 0)
			{
				LLLOJBFMONN.Error(string.Format("ERROR: GetString - parameters passed both through arguments and title in \"{0}\"", PEMOECLNECD));
			}
			for (int num2 = num; num2 != -1; num2 = PEMOECLNECD.IndexOf('{', num2 + 1))
			{
				int num3 = PEMOECLNECD.IndexOf('}', num2 + 1);
				if ((num3 > PEMOECLNECD.IndexOf('{', num2 + 1) && PEMOECLNECD.IndexOf('{', num2 + 1) != -1) || num3 == -1)
				{
					LLLOJBFMONN.Error(string.Format("ERROR: GetString - parameters brackets broken in title \"{0}\"", PEMOECLNECD));
					break;
				}
				string bFFNFGKHBJA = PEMOECLNECD.Substring(num2 + 1, num3 - num2 - 1);
				bFFNFGKHBJA = CAKFDPGBLLG(bFFNFGKHBJA);
				list.Add(bFFNFGKHBJA);
			}
		}
		if (list.Count != 0)
		{
			for (int num4 = text.IndexOf("{"); num4 != -1; num4 = text.IndexOf('{', num4 + 1))
			{
				int num5 = text.IndexOf('}', num4 + 1);
				if ((num5 > text.IndexOf('{', num4 + 1) && text.IndexOf('{', num4 + 1) != -1) || num5 == -1)
				{
					LLLOJBFMONN.Error(string.Format("ERROR: GetString - parameters brackets broken in content of title \"{0}\"", PEMOECLNECD));
					break;
				}
				string text2 = text.Substring(num4 + 1, num5 - num4 - 1);
				int result;
				if (!int.TryParse(text2, out result) || result < 0 || result >= list.Count)
				{
					continue;
				}
				string oldValue = "{" + text2 + "}";
				string text3 = list[result];
				if (text3.StartsWith("img::"))
				{
					text3 = text3.Replace("img::", "<quad name=") + " size=25 width=1 />";
				}
				text = text.Replace(oldValue, text3);
			}
		}
		return CAKFDPGBLLG(text);
	}

	public static string DateString(long NNBJNDAFEDH)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(NNBJNDAFEDH);
		return string.Format("{0}.{1}.{2}", dateTime.Day, dateTime.Month, dateTime.Year);
	}

	private static string CAKFDPGBLLG(string BFFNFGKHBJA)
	{
		string text = BFFNFGKHBJA;
		for (int num = text.IndexOf('%'); num != -1; num = text.IndexOf('%', num + 1))
		{
			if (num < text.Length && text[num + 1] == '%')
			{
				text = text.Remove(num, 1);
			}
			else
			{
				int num2 = GetWordEndSymbol(BFFNFGKHBJA, num);
				string text2 = BFFNFGKHBJA.Substring(num + 1, num2 - num - 1);
				string newValue = GetString(text2);
				text = text.Replace("%" + text2, newValue);
			}
		}
		return text;
	}

	private static int GetWordEndSymbol(string BFFNFGKHBJA, int IOOFDAIOCEL)
	{
		int num = BFFNFGKHBJA.IndexOf(' ', IOOFDAIOCEL);
		int num2 = BFFNFGKHBJA.IndexOf('\n', IOOFDAIOCEL);
		if ((num2 < num && num2 != -1) || num == -1)
		{
			num = num2;
		}
		if (num == -1)
		{
			num = BFFNFGKHBJA.Length;
		}
		return num;
	}

	public static Language KNEELNNCIBG(Language DLKMOGEJJCO = null)
	{
		if (DLKMOGEJJCO == null)
		{
			DLKMOGEJJCO = ILAJKOBCHFH;
		}
		int iHPMGHJPLBP = DLKMOGEJJCO.index;
		int count = MCLNNPPCFFL.Count;
		iHPMGHJPLBP = (iHPMGHJPLBP + 1) % count;
		return MCLNNPPCFFL[iHPMGHJPLBP];
	}

	public static Language NLFKNPBICED(string KEEACJILEEK)
	{
		return MCLNNPPCFFL.Find((Language DHDMNHCIPEH) => DHDMNHCIPEH.name.Equals(KEEACJILEEK));
	}

	public static Language HHKANICOAAG(string EOMNCDDELLB)
	{
		return MCLNNPPCFFL.Find((Language DHDMNHCIPEH) => DHDMNHCIPEH.EOMNCDDELLB.Equals(EOMNCDDELLB));
	}

	private static void Load(string PMFEIPCHENB)
	{
		Clear();
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(PMFEIPCHENB, string.Empty);
		if (xmlDocument != null)
		{
			DGHGCAHEHJN(xmlDocument["Localization"]["Words"]);
			FJLMLAGEJDL = true;
		}
		else
		{
			LLLOJBFMONN.Error(string.Format("ERROR: load - file \"{0}\" doesn't exist", PMFEIPCHENB));
		}
	}

	private static void DGHGCAHEHJN(XmlNode FOKEBDFAEEA)
	{
		foreach (XmlNode childNode in FOKEBDFAEEA.ChildNodes)
		{
			if (childNode.NodeType == XmlNodeType.Element)
			{
				KCAMAIIHKKH[childNode.Attributes["Title"].CIPOICEEIBK(string.Empty)] = childNode.InnerText;
			}
		}
	}

	private static void NADENLPLKGC(Language DLKMOGEJJCO = null, bool DANDCEBFMHM = true)
	{
		if (DLKMOGEJJCO == null)
		{
			DLKMOGEJJCO = ILAJKOBCHFH;
		}
		if (!GGBKNBFCBEJ(DLKMOGEJJCO))
		{
			LLLOJBFMONN.Error(string.Format("ERROR: Language \"{0}\" doesn't have fonts", DLKMOGEJJCO.name));
		}
		Load(DLKMOGEJJCO.PMFEIPCHENB);
		ILAJKOBCHFH = DLKMOGEJJCO;
		if (ILAJKOBCHFH != null)
		{
			ILAJKOBCHFH.GHHCAJDOLFL();
		}
		if (DANDCEBFMHM)
		{
			ListSF.CCDKHLAMKKO().COKACMKOIGD(DLKMOGEJJCO.name);
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
		}
	}

	private static void Clear()
	{
		KCAMAIIHKKH.Clear();
		FJLMLAGEJDL = false;
	}

	public static bool GGBKNBFCBEJ(Language HBGOBBALPBP)
	{
		if (HBGOBBALPBP != null)
		{
			if (HBGOBBALPBP.LOICIDHBPMO == null)
			{
				return false;
			}
			if (HBGOBBALPBP.HKHFNJNDEND == null)
			{
				return false;
			}
			if (HBGOBBALPBP.APIDPJICKBC == null)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static void BJPNKAGDKFL(Language HBGOBBALPBP = null)
	{
		if (HBGOBBALPBP != null)
		{
			NADENLPLKGC(HBGOBBALPBP);
		}
		else
		{
			NADENLPLKGC(KNEELNNCIBG());
		}
		if (OnLanguageChanged != null)
		{
			OnLanguageChanged();
		}
	}
}
