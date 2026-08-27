using System.Diagnostics;
using System.Xml;

public static class InternetController
{
	public class CFEEBFOFKMK
	{
		public string Url;

		public string OLLDPHHNBCC;
	}

	public enum MIPPFGJMDLI
	{
		ANDROID = 0,
		IOS = 1,
		WINPHONE = 2,
		PC = 3
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string FICHLPIOEGM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static CFEEBFOFKMK IIMMBPJFCJE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string CKKPPGNBPMM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool NFEKIAEFMHK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string ODMEFCDBNAK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string IGPCLMEJCJL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static string BIHMBFOCAMI;

	public static string FDNBEILEAEM
	{
		get
		{
			return HPCFJDPAMFE();
		}
		private set
		{
			PMGKDPFHJNM(value);
		}
	}

	public static CFEEBFOFKMK PMELEALOLCF
	{
		get
		{
			return BKGEABLMGKL();
		}
		private set
		{
			LMBHNMBAFDB(value);
		}
	}

	public static string HLBBCJLIDFJ
	{
		get
		{
			return HHJPCEDCLGH();
		}
		private set
		{
			KJDJHGODMPA(value);
		}
	}

	public static bool GGCOEPEDGLP
	{
		get
		{
			return DHDGLNKILPM();
		}
		private set
		{
			set_IsPostAchievements(value);
		}
	}

	public static string MAGGMIHDODO
	{
		get
		{
			return MMIHGFKCMCC();
		}
		private set
		{
			GNKHLLKFFJL(value);
		}
	}

	public static string JDFFNAPHIPJ
	{
		get
		{
			return DMFANLAIJMN();
		}
		private set
		{
			CBKDHJJKMIC(value);
		}
	}

	public static string AEBDFOCDINJ
	{
		get
		{
			return PPPALDPCFPL();
		}
		private set
		{
			BNAJCBJJHPN(value);
		}
	}

	public static string HPCFJDPAMFE()
	{
		return FICHLPIOEGM;
	}

	private static void PMGKDPFHJNM(string value)
	{
		FICHLPIOEGM = value;
	}

	public static CFEEBFOFKMK BKGEABLMGKL()
	{
		return IIMMBPJFCJE;
	}

	private static void LMBHNMBAFDB(CFEEBFOFKMK value)
	{
		IIMMBPJFCJE = value;
	}

	public static string HHJPCEDCLGH()
	{
		return CKKPPGNBPMM;
	}

	private static void KJDJHGODMPA(string value)
	{
		CKKPPGNBPMM = value;
	}

	public static bool DHDGLNKILPM()
	{
		return NFEKIAEFMHK;
	}

	private static void set_IsPostAchievements(bool value)
	{
		NFEKIAEFMHK = value;
	}

	public static string MMIHGFKCMCC()
	{
		return ODMEFCDBNAK;
	}

	private static void GNKHLLKFFJL(string value)
	{
		ODMEFCDBNAK = value;
	}

	public static string DMFANLAIJMN()
	{
		return IGPCLMEJCJL;
	}

	private static void CBKDHJJKMIC(string value)
	{
		IGPCLMEJCJL = value;
	}

	public static string PPPALDPCFPL()
	{
		return BIHMBFOCAMI;
	}

	private static void BNAJCBJJHPN(string value)
	{
		BIHMBFOCAMI = value;
	}

	public static void Parse(XmlNode node)
	{
		PMGKDPFHJNM(node["FBPostPicture"].Attributes["Url"].CIPOICEEIBK(string.Empty));
		XmlNode hKPPBKPJOEO = node["Android"];
		MIPPFGJMDLI jGJNNAHDPBA = MIPPFGJMDLI.ANDROID;
		AKEJKLCMBNP(hKPPBKPJOEO, jGJNNAHDPBA);
		ParseServer(node["Server"]);
	}

	private static void AKEJKLCMBNP(XmlNode node, MIPPFGJMDLI JGJNNAHDPBA)
	{
		LMBHNMBAFDB(new CFEEBFOFKMK());
		if (node["FBLikeUrl"] != null)
		{
			BKGEABLMGKL().Url = node["FBLikeUrl"].Attributes["Url"].CIPOICEEIBK(string.Empty);
			BKGEABLMGKL().OLLDPHHNBCC = node["FBLikeUrl"].Attributes["AltUrl"].CIPOICEEIBK(string.Empty);
		}
		if (node["FBPostLink"] != null)
		{
			if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				KJDJHGODMPA(node["FBPostLink"].Attributes["Amazon"].CIPOICEEIBK(string.Empty));
			}
			else if (SystemProperties.IPJFCBAGMJJ())
			{
				KJDJHGODMPA(node["FBPostLink"].Attributes["PlayMarket"].CIPOICEEIBK(string.Empty));
			}
			else
			{
				KJDJHGODMPA(node["FBPostLink"].Attributes["Url"].CIPOICEEIBK(string.Empty));
			}
		}
		if (node["PostAchievements"] != null)
		{
			set_IsPostAchievements(node["PostAchievements"].Attributes["Value"].ParseBool());
		}
		if (node["MusicStore"] != null)
		{
			if (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK())
			{
				GNKHLLKFFJL(node["MusicStore"].Attributes["Amazon"].CIPOICEEIBK(string.Empty));
			}
			else
			{
				GNKHLLKFFJL(node["MusicStore"].Attributes["Url"].CIPOICEEIBK(string.Empty));
			}
		}
		string name = "Url";
		if (JGJNNAHDPBA == MIPPFGJMDLI.ANDROID)
		{
			name = ((!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL()) ? "PlayMarket" : "China360");
			name = ((!AssemblyController.JONCCPLEIBE().OPCBKOOFMAK()) ? name : "Amazon");
		}
		CBKDHJJKMIC(node["RateUrl"].Attributes[name].CIPOICEEIBK(string.Empty));
	}

	private static void ParseServer(XmlNode node)
	{
		BNAJCBJJHPN(node["DefaultConfigUrl"].Attributes["Url"].CIPOICEEIBK(string.Empty));
	}
}
