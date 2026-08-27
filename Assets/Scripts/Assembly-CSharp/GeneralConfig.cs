using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Nekki.SF2.Core.Network;
using Nekki.Utils;

public class GeneralConfig
{
	private const string PNMKCPIEPCO = "/config_cdn.xml";

	public static News FNHPCBEDKFO = new News();

	public static Packs NNFMKNJJDDD = new Packs();

	public static PricesDataContainer IHHMHNHOLCB = new PricesDataContainer();

	private Action<bool> _Callback;

	private Action<object> IJHFDMNNJJI;

	private List<JBKAOMLJCEL> MJLGAEJFDPK;

	private int FHHIKLABEHN;

	private int AEABMNJHLLC;

	private XmlDocument JNHOOLOOPMO;

	private bool OIPHJPKPPAJ;

	private string KFNGFONMBCA;

	private bool PELGCIAEKIB = true;

	private bool ODHPOJMNFIN = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private LedgerSettings MNIDDMLBPHB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private VerificationSettings EFAGAACJHED;

	private static GeneralConfig _Instance = null;

	public bool DMIHGNFGFCF
	{
		get
		{
			return IHGDCIFNAOA();
		}
	}

	public bool FGEBFKEMFEC
	{
		get
		{
			return CHAPIILIEPK();
		}
	}

	public LedgerSettings JFCEPCNJMIH
	{
		get
		{
			return IMOKGIDCANG();
		}
		private set
		{
			DPHOIOBJABC(value);
		}
	}

	public VerificationSettings LAELKCACBBL
	{
		get
		{
			return OKJAHGKBGMK();
		}
		private set
		{
			KHGMAIADCLI(value);
		}
	}

	public static GeneralConfig BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public bool IHGDCIFNAOA()
	{
		return PELGCIAEKIB;
	}

	public bool CHAPIILIEPK()
	{
		return ODHPOJMNFIN;
	}

	public LedgerSettings IMOKGIDCANG()
	{
		return MNIDDMLBPHB;
	}

	private void DPHOIOBJABC(LedgerSettings value)
	{
		MNIDDMLBPHB = value;
	}

	public VerificationSettings OKJAHGKBGMK()
	{
		return EFAGAACJHED;
	}

	private void KHGMAIADCLI(VerificationSettings value)
	{
		EFAGAACJHED = value;
	}

	public static GeneralConfig ELEBLBJKDBI()
	{
		if (_Instance == null)
		{
			_Instance = new GeneralConfig();
		}
		return _Instance;
	}

	public void DOJCMIFHJKM(string path, Action<bool> callback, Action<object> IPDNNACNOEN, List<JBKAOMLJCEL> DEJEBFLAOIB, int HCCLKJOCHGP, int INGCPFFHBOG)
	{
		_Callback = callback;
		IJHFDMNNJJI = IPDNNACNOEN;
		MJLGAEJFDPK = DEJEBFLAOIB;
		FHHIKLABEHN = HCCLKJOCHGP;
		AEABMNJHLLC = INGCPFFHBOG;
		if (string.IsNullOrEmpty(path))
		{
			OGIJONMKABB();
			return;
		}
		string arg = NekkiMath.randomInt(1000000).ToString();
		string mGPGDPOOCBK = string.Format("{0}?{1}", path, arg);
		ServerProvider.get_Instance().DownloadFile(mGPGDPOOCBK, OnLoadConfig, null, FHHIKLABEHN);
	}

	private void OnLoadConfig(byte[] data, string JDONBAPIJCG, string BEPKJNKCKPH)
	{
		if (string.IsNullOrEmpty(JDONBAPIJCG) && data != null)
		{
			File.WriteAllBytes(SF2Paths.GBOFOFGDMBN() + "/config_cdn.xml", data);
		}
		else
		{
			if (JDONBAPIJCG == "offline build")
			{
				UnityEngine.Debug.LogWarning("[Config] Offline build; using the recovered local config.");
			}
			else
			{
				LLLOJBFMONN.Error("[Config]: failed to download config because " + JDONBAPIJCG);
			}
		}
		Parse(true);
	}

	private bool Parse(bool EJGPPDALIOJ)
	{
		bool flag = true;
		if (EJGPPDALIOJ)
		{
			JNHOOLOOPMO = XmlUtils.OpenXMLDocument(SF2Paths.GBOFOFGDMBN(), "/config_cdn.xml", XmlUtils.EBLFEPIOMOL.ForcedExternal);
			if (JNHOOLOOPMO == null)
			{
				JNHOOLOOPMO = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI() + "/config_cdn.xml", string.Empty);
			}
		}
		if (flag)
		{
			KPLGMLEJLFA();
		}
		else
		{
			PELGCIAEKIB = false;
			OGIJONMKABB();
		}
		JNHOOLOOPMO = null;
		return flag;
	}

	private void KPLGMLEJLFA()
	{
		if (JNHOOLOOPMO == null)
		{
			LLLOJBFMONN.Error("GeneralConfig.ParseXML: _DocConfig is null");
			return;
		}
		XmlNode xmlNode = JNHOOLOOPMO["data"];
		XmlNode xmlNode2 = xmlNode["platform"];
		XmlNode xmlNode3 = xmlNode["versions"];
		XmlNode xmlNode4 = xmlNode["settings"];
		XmlNode xmlNode5 = xmlNode["news"];
		XmlNode xmlNode6 = xmlNode["price"];
		if (xmlNode2 != null)
		{
			EDJJHECLEIN(xmlNode2);
		}
		if (xmlNode3 != null)
		{
			MPKFAGOPGPP(xmlNode3);
		}
		if (xmlNode4 != null)
		{
			IOBGEFFIKJA(xmlNode4);
		}
		if (xmlNode5 != null)
		{
			CJFMDMDCFDF(xmlNode5);
		}
		KCPJMMDJEMN(xmlNode);
		if (xmlNode6 != null)
		{
			EKAHAGFFJOO(xmlNode6);
		}
		OGIJONMKABB();
	}

	private void EDJJHECLEIN(XmlNode GLCBJNIIPDG)
	{
		KFNGFONMBCA = "unknown";
		foreach (XmlNode childNode in GLCBJNIIPDG.ChildNodes)
		{
			int bAINMLLIKOL = childNode.Attributes["PlatformID"].ParseInt();
			if (CheckPlatform(bAINMLLIKOL))
			{
				KFNGFONMBCA = childNode.Attributes["Name"].CIPOICEEIBK(KFNGFONMBCA);
			}
		}
	}

	private void MPKFAGOPGPP(XmlNode BPDFMKIGEKF)
	{
		bool oDHPOJMNFIN = false;
		VersionContainer aAOIAEJJINO = SystemProperties.KCJMMIEBLHL();
		foreach (XmlNode childNode in BPDFMKIGEKF.ChildNodes)
		{
			int bAINMLLIKOL = childNode.Attributes["PlatformID"].ParseInt();
			if (CheckPlatform(bAINMLLIKOL))
			{
				VersionContainer pAMHFPMEPCH = new VersionContainer();
				pAMHFPMEPCH.SetVersion(childNode.Attributes["Version"].CIPOICEEIBK());
				if (VersionContainer.CGMHEDJDOEK(pAMHFPMEPCH, aAOIAEJJINO))
				{
					oDHPOJMNFIN = true;
					break;
				}
			}
		}
		ODHPOJMNFIN = oDHPOJMNFIN;
	}

	private void IOBGEFFIKJA(XmlNode node)
	{
		XmlNode aIDFCDDECJB = node["time"];
		XmlNode aIDFCDDECJB2 = node["dumps"];
		XmlNode aIDFCDDECJB3 = node["server"];
		KeyValuePair<string, string> hFCAPMDHLJN = KMJMDEKMCNO(aIDFCDDECJB);
		KeyValuePair<string, string> hFCAPMDHLJN2 = KMJMDEKMCNO(aIDFCDDECJB2);
		KeyValuePair<string, string> hFCAPMDHLJN3 = KMJMDEKMCNO(aIDFCDDECJB3);
		if (!KANBBNPLMMM(hFCAPMDHLJN))
		{
			ServerProvider.set_TimeServerURL(hFCAPMDHLJN.Value);
		}
		if (!KANBBNPLMMM(hFCAPMDHLJN2))
		{
			ServerProvider.set_DumpPutURL(hFCAPMDHLJN2.Key);
			ServerProvider.set_DumpGetURL(hFCAPMDHLJN2.Value);
		}
		if (!KANBBNPLMMM(hFCAPMDHLJN3))
		{
			ServerProvider.set_PutURL(hFCAPMDHLJN3.Key);
			ServerProvider.set_GetURL(hFCAPMDHLJN3.Value);
		}
		XmlNode hKPPBKPJOEO = node["verification"];
		KINLHDGNIIO(hKPPBKPJOEO);
		XmlNode hKPPBKPJOEO2 = node["ledger"];
		IPDGIOGEGOA(hKPPBKPJOEO2);
	}

	private void KINLHDGNIIO(XmlNode node)
	{
		KHGMAIADCLI(new VerificationSettings(node.Attributes["Url"].CIPOICEEIBK(string.Empty), node.Attributes["Timeout"].ParseInt(), node.Attributes["MaxRetry"].ParseInt(), node.Attributes["Frequency"].ParseInt()));
	}

	private void IPDGIOGEGOA(XmlNode node)
	{
		DPHOIOBJABC(new LedgerSettings(node.Attributes["Url"].CIPOICEEIBK(string.Empty), node.Attributes["Timeout"].ParseInt(), node.Attributes["MaxRetry"].ParseInt()));
	}

	private void KCPJMMDJEMN(XmlNode node)
	{
		NNFMKNJJDDD.Reset();
		XmlNode xmlNode = node["packs"];
		XmlNode xmlNode2 = node["fonts"];
		XmlNode xmlNode3 = node["video"];
		if (xmlNode != null)
		{
			GDNFPIBDDBO(xmlNode);
		}
		if (xmlNode2 != null)
		{
			GDNFPIBDDBO(xmlNode2);
		}
		if (xmlNode3 != null)
		{
			GDNFPIBDDBO(xmlNode3);
		}
	}

	private void GDNFPIBDDBO(XmlNode MEEAKLDGLDF)
	{
		Dictionary<string, List<XmlNode>> dictionary = new Dictionary<string, List<XmlNode>>();
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			string key = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			if (!dictionary.ContainsKey(key))
			{
				dictionary[key] = new List<XmlNode>();
			}
			dictionary[key].Add(childNode);
		}
		foreach (KeyValuePair<string, List<XmlNode>> item in dictionary)
		{
			string key2 = item.Key;
			List<XmlNode> value = item.Value;
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, "TmpNode", null);
			foreach (XmlNode item2 in value)
			{
				int bAINMLLIKOL = item2.Attributes["PlatformID"].ParseInt();
				if (CheckPlatform(bAINMLLIKOL))
				{
					xmlNode2.LCOLFMJJDJE(item2);
				}
			}
			XmlNode xmlNode3 = ClosestVersion(xmlNode2);
			string text = ((xmlNode3 == null) ? null : xmlNode3.Attributes["Url"].CIPOICEEIBK());
			string pEEOEOMEBFG = ((xmlNode3 == null) ? null : xmlNode3.Attributes["Size"].CIPOICEEIBK());
			bool lCDCAKLKHMI = xmlNode3 != null && xmlNode3.Attributes["Reload"].ParseBool();
			string hDPBNCNCMOH = ((xmlNode3 == null) ? null : xmlNode3.Attributes["Hash"].CIPOICEEIBK());
			bool aHDLCJFCJMJ = xmlNode3 != null && xmlNode3.Attributes["Attach"].ParseBool();
			if (!string.IsNullOrEmpty(text))
			{
				NNFMKNJJDDD.DDKKLHDOFNG(key2, text, pEEOEOMEBFG, lCDCAKLKHMI, hDPBNCNCMOH, aHDLCJFCJMJ);
			}
		}
	}

	private XmlNode ClosestVersion(XmlNode nodes, bool DFOOHEFGEBG = false, bool MGMDADDKPMP = false)
	{
		XmlNode result = null;
		VersionContainer aAOIAEJJINO = new VersionContainer();
		VersionContainer pAMHFPMEPCH = SystemProperties.KCJMMIEBLHL();
		foreach (XmlNode childNode in nodes.ChildNodes)
		{
			if (MGMDADDKPMP)
			{
				int bAINMLLIKOL = childNode.Attributes["PlatformID"].ParseInt();
				if (!CheckPlatform(bAINMLLIKOL))
				{
					continue;
				}
			}
			VersionContainer pAMHFPMEPCH2 = new VersionContainer();
			pAMHFPMEPCH2.SetVersion(childNode.Attributes["MinVersion"].CIPOICEEIBK());
			if (!DFOOHEFGEBG)
			{
				if (VersionContainer.BCCGLNMPHCE(pAMHFPMEPCH2, aAOIAEJJINO) && VersionContainer.CDOCLICKACF(pAMHFPMEPCH2, pAMHFPMEPCH))
				{
					aAOIAEJJINO = pAMHFPMEPCH2;
					result = childNode;
				}
			}
			else if (VersionContainer.LFPMCJPCJBD(pAMHFPMEPCH, pAMHFPMEPCH2))
			{
				result = childNode;
				break;
			}
		}
		return result;
	}

	private void CJFMDMDCFDF(XmlNode node)
	{
		if (!ParseNewsForLocale(node, SystemProperties.NICPICAMAOH().OAPHJAPMKJG) && !ParseNewsForLocale(node, SystemProperties.NICPICAMAOH().OHCHKFMFDKM()) && !ParseNewsForLocale(node, "Other"))
		{
		}
	}

	private bool ParseNewsForLocale(XmlNode MEEAKLDGLDF, string EADIFEPJKJK)
	{
		bool result = false;
		VersionContainer lHBNIMGFKIB = SystemProperties.KCJMMIEBLHL();
		foreach (XmlNode childNode in MEEAKLDGLDF.ChildNodes)
		{
			if (childNode.Name != "item")
			{
				continue;
			}
			int bAINMLLIKOL = childNode.Attributes["PlatformID"].ParseInt();
			if (!CheckPlatform(bAINMLLIKOL) || !IsOkLocale(childNode.Attributes["LangID"].CIPOICEEIBK(), EADIFEPJKJK))
			{
				continue;
			}
			VersionContainer pAMHFPMEPCH = new VersionContainer();
			pAMHFPMEPCH.SetVersion(childNode.Attributes["MinVersion"].CIPOICEEIBK());
			if (VersionContainer.GLLHGKILFFH(lHBNIMGFKIB, pAMHFPMEPCH))
			{
				continue;
			}
			long num = childNode.Attributes["StartDate"].ParseLong(0L);
			num += SystemProperties.JOFIGLFDPDE();
			if (GlobalTimer.get_LocalTimeUTC() >= num)
			{
				long num2 = childNode.Attributes["EndDate"].ParseLong(-1L);
				num2 += SystemProperties.JOFIGLFDPDE();
				if (num2 <= 0 || num2 >= GlobalTimer.get_LocalTimeUTC())
				{
					string pEMOECLNECD = childNode.Attributes["Title"].CIPOICEEIBK(string.Empty);
					string gOHIIMFFFJI = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
					string bEPKJNKCKPH = childNode.Attributes["Url"].CIPOICEEIBK(string.Empty);
					string mDDOAGNHAHE = childNode.Attributes["ImageURL"].CIPOICEEIBK(string.Empty);
					int oKNNNLIPODI = childNode.Attributes["ID"].ParseInt();
					bool eIKKPDKMMHK = childNode.Attributes["GoShop"].ParseBool();
					string kINPMPFPFHD = childNode.Attributes["RedirectShop"].CIPOICEEIBK(string.Empty);
					string eJENJNPEDOH = childNode.Attributes["SpenderTypeID"].CIPOICEEIBK(string.Empty);
					bool hNJDHGDLLPD = childNode.Attributes["Active"].ParseBool();
					List<NewsButton> hJNAHNICGMH = KBHBNGCIOEO(childNode);
					FNHPCBEDKFO.EJDDCELLCBK(gOHIIMFFFJI, bEPKJNKCKPH, mDDOAGNHAHE, oKNNNLIPODI, hNJDHGDLLPD, num2, hJNAHNICGMH, pEMOECLNECD, eIKKPDKMMHK, kINPMPFPFHD, eJENJNPEDOH);
					result = true;
				}
			}
		}
		return result;
	}

	private List<NewsButton> KBHBNGCIOEO(XmlNode MEEAKLDGLDF)
	{
		List<NewsButton> list = new List<NewsButton>();
		foreach (XmlNode item in MEEAKLDGLDF)
		{
			NewsButton fBKMFDJBJIB = new NewsButton();
			fBKMFDJBJIB.GGDJIPKMKFC = item.Attributes["Text"].CIPOICEEIBK(string.Empty);
			fBKMFDJBJIB.Color = LabelButton.GetBtnColor(item.Attributes["Color"].CIPOICEEIBK(string.Empty));
			fBKMFDJBJIB.Url = item.Attributes["Url"].CIPOICEEIBK(string.Empty);
			fBKMFDJBJIB.EGBHELMJJKO = item.Attributes["GoShop"].ParseBool();
			fBKMFDJBJIB.KCBCGDFKNME = item.Attributes["BuyItem"].ParseBool();
			fBKMFDJBJIB.COIGFENOMJD = item.Attributes["RedirectShop"].CIPOICEEIBK(string.Empty);
			list.Add(fBKMFDJBJIB);
		}
		return list;
	}

	private bool KANBBNPLMMM(KeyValuePair<string, string> HFCAPMDHLJN)
	{
		return string.IsNullOrEmpty(HFCAPMDHLJN.Key) && string.IsNullOrEmpty(HFCAPMDHLJN.Value);
	}

	private KeyValuePair<string, string> KMJMDEKMCNO(XmlNode AIDFCDDECJB)
	{
		string key = null;
		string value = null;
		foreach (XmlNode childNode in AIDFCDDECJB.ChildNodes)
		{
			int bAINMLLIKOL = childNode.Attributes["PlatformID"].ParseInt();
			if (CheckPlatform(bAINMLLIKOL))
			{
				key = childNode.Attributes["PutUrl"].CIPOICEEIBK(string.Empty);
				value = childNode.Attributes["GetUrl"].CIPOICEEIBK(string.Empty);
			}
		}
		return new KeyValuePair<string, string>(key, value);
	}

	private void EKAHAGFFJOO(XmlNode node)
	{
		if (node.ChildNodes.Count != 0)
		{
			IHHMHNHOLCB.GMCBGMPEHLF().Clear();
			ParsePricesForLocale(node, SystemProperties.NICPICAMAOH().OAPHJAPMKJG);
			ParsePricesForLocale(node, SystemProperties.NICPICAMAOH().OHCHKFMFDKM());
		}
	}

	private void ParsePricesForLocale(XmlNode node, string EADIFEPJKJK)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (!CheckPlatform(childNode.Attributes["PlatformID"].ParseInt()) || !childNode.Attributes["MobileOperator"].Empty())
			{
				continue;
			}
			PricesData bEOLBLGJCKA = FFMKNHLDFDF(childNode);
			if (!string.IsNullOrEmpty(bEOLBLGJCKA.EOMNCDDELLB) && !IsOkLocale(bEOLBLGJCKA.EOMNCDDELLB, EADIFEPJKJK))
			{
				continue;
			}
			PricesData bEOLBLGJCKA2 = IHHMHNHOLCB.CCFOOCDFGMF(bEOLBLGJCKA.GNIJPFLLNIC);
			if (bEOLBLGJCKA2 == null)
			{
				IHHMHNHOLCB.GMCBGMPEHLF().Add(bEOLBLGJCKA);
				continue;
			}
			bool flag = bEOLBLGJCKA.PHMJCFDJAMJ == bEOLBLGJCKA2.PHMJCFDJAMJ;
			bool flag2 = bEOLBLGJCKA.EJENJNPEDOH == bEOLBLGJCKA2.EJENJNPEDOH || string.IsNullOrEmpty(bEOLBLGJCKA2.EJENJNPEDOH);
			if (flag && flag2)
			{
				if (string.IsNullOrEmpty(bEOLBLGJCKA2.EOMNCDDELLB))
				{
					IHHMHNHOLCB.GMCBGMPEHLF().Remove(bEOLBLGJCKA2);
					IHHMHNHOLCB.GMCBGMPEHLF().Add(bEOLBLGJCKA);
				}
			}
			else
			{
				IHHMHNHOLCB.GMCBGMPEHLF().Add(bEOLBLGJCKA);
			}
		}
	}

	private PricesData FFMKNHLDFDF(XmlNode node)
	{
		PricesData bEOLBLGJCKA = new PricesData();
		bEOLBLGJCKA.GNIJPFLLNIC = node.Attributes["ProductID"].CIPOICEEIBK();
		bEOLBLGJCKA.GFMKCJPKMOK = node.Attributes["NewProductID"].CIPOICEEIBK();
		bEOLBLGJCKA.NICNMHCJIBJ = node.Attributes["Amount"].ParseLong(0L);
		bEOLBLGJCKA.AJKMNFGEHIJ = node.Attributes["NewAmount"].ParseLong(0L);
		bEOLBLGJCKA.ABAINMKLBAM = node.Attributes["AddAmount"].ParseLong(0L);
		bEOLBLGJCKA.IIHKEOHAKDJ = node.Attributes["NewAddAmount"].ParseLong(0L);
		bEOLBLGJCKA.LMNMPHGIFAF = node.Attributes["Price"].CIPOICEEIBK();
		bEOLBLGJCKA.DDHOJFFGBKM = node.Attributes["NewPrice"].CIPOICEEIBK();
		bEOLBLGJCKA.JGMODPBJHAD = node.Attributes["Currency"].ParseInt();
		bEOLBLGJCKA.KBCEJHOADJK = node.Attributes["AddCurrency"].CIPOICEEIBK();
		bEOLBLGJCKA.name = node.Attributes["Name"].CIPOICEEIBK();
		bEOLBLGJCKA.OFPIHGHEJAH = node.Attributes["StartDate"].ParseInt();
		bEOLBLGJCKA.MCEDKIPLOMO = node.Attributes["EndDate"].ParseInt();
		bEOLBLGJCKA.AOJJBKLCHJO = node.Attributes["Sign"].CIPOICEEIBK();
		bEOLBLGJCKA.AIFNAPNLOML = node.Attributes["SignCode"].CIPOICEEIBK("USD");
		bEOLBLGJCKA.ICBBNJMLDJH = node.Attributes["Label"].CIPOICEEIBK(string.Empty);
		bEOLBLGJCKA.PHMJCFDJAMJ = node.Attributes["GroupID"].CIPOICEEIBK(string.Empty);
		bEOLBLGJCKA.BKDNJPAOAEL = node.Attributes["AddPercent"].ParseInt();
		bEOLBLGJCKA.EOMNCDDELLB = node.Attributes["Locale"].CIPOICEEIBK(string.Empty);
		bEOLBLGJCKA.PBAMOKEPKBG = node.Attributes["Focus"].ParseBool();
		bEOLBLGJCKA.GGDANLHOOKB = node.Attributes["MobileOperator"].CIPOICEEIBK(string.Empty);
		bEOLBLGJCKA.EJENJNPEDOH = node.Attributes["SpenderTypeID"].CIPOICEEIBK(string.Empty);
		PricesData bEOLBLGJCKA2 = bEOLBLGJCKA;
		bEOLBLGJCKA2.ALAFFFIOIFI = node.Attributes["ProductType"].ParseInt(1) != 2;
		return bEOLBLGJCKA2;
	}

	private bool IsOkLocale(string EOMNCDDELLB, string CNGMIFIJKDB)
	{
		if (string.IsNullOrEmpty(EOMNCDDELLB) && string.IsNullOrEmpty(CNGMIFIJKDB))
		{
			return true;
		}
		string[] array = EOMNCDDELLB.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text == CNGMIFIJKDB)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckPlatform(int value)
	{
		return value == 0 || (SystemProperties.MEBGOGMJFLM() && value == 1) || (SystemProperties.IPJFCBAGMJJ() && value == 2) || (SystemProperties.LHGPKEFEHDH() && value == 3) || (SystemProperties.AFKGHBJPLOK() && value == 4) || (SystemProperties.NFFOJCHNPJD() && value == 5);
	}

	private void OGIJONMKABB()
	{
		if (_Callback != null)
		{
			_Callback(OIPHJPKPPAJ);
			OIPHJPKPPAJ = false;
			_Callback = null;
		}
	}

	public static void LOGLOMLEHFI()
	{
		LLLOJBFMONN.Write("[GeneralConfig] wipe external config");
		HCEPBIAOJKG.BKLIKICKDPH(SF2Paths.GBOFOFGDMBN() + "/config_cdn.xml");
	}
}
