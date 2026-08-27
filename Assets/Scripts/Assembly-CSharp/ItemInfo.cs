using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Purchasing;

public class ItemInfo
{
	public enum MEFIBHIDOLA
	{
		SPEND_TYPE_NONE = 0,
		SPEND_TYPE_ENERGY = 1
	}

	public enum HAIJKHDIFBC
	{
		DISCOUNT_NONE = 0,
		DISCOUNT_QUEST = 1,
		DISCOUNT_CONFIG = 2
	}

	public const string HMNIGMPCKFE = "NoneItem";

	public const string PKFHJMIEKFG = "Skeleton";

	public const string NKCPIOAGNAE = "Weapon";

	public const string PNMMEPBMMOC = "Armor";

	public const string OGPJODJEGLM = "Helm";

	public const string BIJMKPGBHMO = "Ranged";

	public const string NFLIMNFMJJI = "Magic";

	public const string BDFIIAGIKHM = "RealMoneyItem";

	public const string HNLBLBNJMPN = "Energy";

	public const string HEBJDAMBIME = "Dummy";

	public const string LEAFPMKEIMF = "Decorate";

	public const string FFFKBNMHDOP = "Cheat";

	public const string DPKFGDKOCMA = "Seal";

	public const string AOHPCDBCLBI = "Free";

	public const string DONHFCBOKKC = "Profile";

	public const string DGAMEBKKNPP = "Recipe";

	public const string HDNONAPIKGK = "Consumable";

	public const string KHOKCIKCNKA = "RaidConsumable";

	public const string OGKHEJBOPAH = "RaidItemPack";

	public const string CKLEEDONENO = "Gold";

	public const string DNBMMGPOBPC = "Bonus";

	public const string GEPKNMNBICG = "UnlimitedEnergy";

	public const string LDJPGMNAKFA = "StarterPack";

	public const string AMBOPJLFKJG = "TapJoy";

	public const string JMOCLCKLIBA = "SponsorPay";

	public const string LMNJLACPFJP = "Metaps";

	public const string PMIDDMDFEBM = "Video";

	public const string MDJNCBFJCJG = "Facebook_Like";

	public const string AMPCCNCIIFN = "PerkReset";

	public const string DLIHMBADCEO = "Currency";

	public const string JOGIHBIANNF = "RaidCurrency";

	public const string ICICDOHDFCB = "RaidCharge";

	public const string HHGPLLMEONG = "RaidPotion";

	public const string EEFPBKDFKAK = "RaidHorn";

	public string Name = string.Empty;

	public string FileName = string.Empty;

	public string KJDFJPBIGJC = string.Empty;

	public string Type = string.Empty;

	public string MDPPNGIEJGD = string.Empty;

	public string IDFNCLPIIMA = string.Empty;

	public string HBCNKNFPAIM = string.Empty;

	public string DBJJONLCHND = string.Empty;

	public string MMHIKEIDDNB = string.Empty;

	private string IIKKHJIIFEE = string.Empty;

	private bool JFOKBAKDBDA;

	public string FPEIFLEBEAA = string.Empty;

	public string EGAJMELKANL = string.Empty;

	public string MIIJIMJDHFP = string.Empty;

	public string PBMHNMOHODB = string.Empty;

	public string GGDJIPKMKFC = string.Empty;

	public string CGGDGCCNKJA = string.Empty;

	public string CMDJPAKOHMK = string.Empty;

	public string PHDCGJOKBLH = string.Empty;

	public bool ANNCECNAEPN;

	public bool KIGOEKKCPOJ;

	private bool PPGBMODEAGD;

	public int Index;

	public int NLMDNOBHHKP;

	public int MHGODOLNDLE;

	public int GDCBBAHKCIE;

	public bool DCHJDPCEODD;

	private bool FOMPCNKEPJF;

	public int ICDIEHCJBGA;

	public int GKODCKNAAHB;

	public int OBJDGBBFJOO;

	public long EHKNIKHPGDN;

	public ObscuredLong KJFAOKLILOC = (ObscuredLong)(0L);

	public ObscuredLong FMHECGHHKGB = (ObscuredLong)(0L);

	public ObscuredLong KLHOKKPALOK = (ObscuredLong)(0L);

	public ObscuredLong NDCOLFHCNLD = (ObscuredLong)(0L);

	public bool MBLKNNAFCOB;

	public ObscuredLong HHIFKGOJFAC = (ObscuredLong)(0L);

	public ObscuredLong BBMLCBEFLGI = (ObscuredLong)(0L);

	public ItemInfo ParentItem;

	public XmlNode NodeXML;

	public bool GNDLEFFMJDJ;

	private bool DHDDDJFLDBD;

	public Attributes IBLHIAHECLK = new Attributes();

	public List<PerkInfoItem> NHBIJEEKALC = new List<PerkInfoItem>();

	public List<UpgradeData> KEFPALGDBOC = new List<UpgradeData>();

	public List<PerkInfoItem> LFIGBCDJHPG = new List<PerkInfoItem>();

	public List<PerkInfoItem> BAHCGAGHPNE = new List<PerkInfoItem>();

	public List<PerkStruct> APMJCGBNEDI = new List<PerkStruct>();

	private bool DJNOJLDEHDD;

	public string FAEGJAEEMGH = string.Empty;

	public ObscuredInt CPODJDDPJHB = (ObscuredInt)(0);

	private int FMOJBFNFLNM;

	private int OJMODONDEHE;

	private bool DGOMAGNAMMD;

	public ObscuredInt FOLLHACLPNB = (ObscuredInt)(0);

	public bool ACOIHHPOBDH;

	public long NNLMNNAEDIE;

	public long PEGDPDINDDO;

	public string FCCNPMNNGAN
	{
		get
		{
			return JLDEALIEEJI();
		}
		set
		{
			set_MarketID(value);
		}
	}

	public bool LBPBADPNHLJ
	{
		get
		{
			return DFFFFIHOOKL();
		}
	}

	public bool IsNew
	{
		get
		{
			return DBHJGAGOLOB();
		}
		set
		{
			BEBDMOEIEJN(value);
		}
	}

	public ItemInfo(XmlNode node)
	{
		if (node != null)
		{
			Init();
			JKJLFOAOLFI(node);
		}
	}

	protected ItemInfo()
	{
		Init();
	}

	protected ItemInfo(ItemInfo item)
	{
		Name = item.Name;
		FileName = item.FileName;
		KJDFJPBIGJC = item.KJDFJPBIGJC;
		Type = item.Type;
		MDPPNGIEJGD = item.MDPPNGIEJGD;
		IDFNCLPIIMA = item.IDFNCLPIIMA;
		HBCNKNFPAIM = item.HBCNKNFPAIM;
		DBJJONLCHND = item.DBJJONLCHND;
		MMHIKEIDDNB = item.MMHIKEIDDNB;
		IIKKHJIIFEE = item.IIKKHJIIFEE;
		JFOKBAKDBDA = item.JFOKBAKDBDA;
		FPEIFLEBEAA = item.FPEIFLEBEAA;
		EGAJMELKANL = item.EGAJMELKANL;
		MIIJIMJDHFP = item.MIIJIMJDHFP;
		PBMHNMOHODB = item.PBMHNMOHODB;
		GGDJIPKMKFC = item.GGDJIPKMKFC;
		CGGDGCCNKJA = item.CGGDGCCNKJA;
		CMDJPAKOHMK = item.CMDJPAKOHMK;
		ANNCECNAEPN = item.ANNCECNAEPN;
		KIGOEKKCPOJ = item.KIGOEKKCPOJ;
		PHDCGJOKBLH = item.PHDCGJOKBLH;
		PPGBMODEAGD = item.PPGBMODEAGD;
		Index = item.Index;
		NLMDNOBHHKP = item.NLMDNOBHHKP;
		MHGODOLNDLE = item.MHGODOLNDLE;
		GDCBBAHKCIE = item.GDCBBAHKCIE;
		DCHJDPCEODD = item.DCHJDPCEODD;
		FOMPCNKEPJF = item.FOMPCNKEPJF;
		ICDIEHCJBGA = item.ICDIEHCJBGA;
		GKODCKNAAHB = item.GKODCKNAAHB;
		OBJDGBBFJOO = item.OBJDGBBFJOO;
		EHKNIKHPGDN = item.EHKNIKHPGDN;
		KJFAOKLILOC = item.KJFAOKLILOC;
		FMHECGHHKGB = item.FMHECGHHKGB;
		KLHOKKPALOK = item.KLHOKKPALOK;
		NDCOLFHCNLD = item.NDCOLFHCNLD;
		MBLKNNAFCOB = item.MBLKNNAFCOB;
		HHIFKGOJFAC = item.HHIFKGOJFAC;
		BBMLCBEFLGI = item.BBMLCBEFLGI;
		ParentItem = item.ParentItem;
		GNDLEFFMJDJ = item.GNDLEFFMJDJ;
		DJNOJLDEHDD = item.DJNOJLDEHDD;
		IBLHIAHECLK = new Attributes(item.IBLHIAHECLK);
		NHBIJEEKALC = new List<PerkInfoItem>(item.NHBIJEEKALC);
		KEFPALGDBOC = new List<UpgradeData>(item.KEFPALGDBOC);
		LFIGBCDJHPG = new List<PerkInfoItem>(item.LFIGBCDJHPG);
		BAHCGAGHPNE = new List<PerkInfoItem>(item.BAHCGAGHPNE);
		APMJCGBNEDI = new List<PerkStruct>(item.APMJCGBNEDI);
	}

	public string JLDEALIEEJI()
	{
		return IIKKHJIIFEE;
	}

	public void set_MarketID(string value)
	{
		IIKKHJIIFEE = value;
	}

	public bool DFFFFIHOOKL()
	{
		return JFOKBAKDBDA;
	}

	public bool DBHJGAGOLOB()
	{
		return DJNOJLDEHDD;
	}

	public void BEBDMOEIEJN(bool value)
	{
		if (!value || GKODCKNAAHB == 0)
		{
			DJNOJLDEHDD = value;
		}
	}

	public void RandomizeObscuredVars()
	{
		CPODJDDPJHB.GMCADPGOCHM();
		FOLLHACLPNB.GMCADPGOCHM();
		KJFAOKLILOC.GMCADPGOCHM();
		FMHECGHHKGB.GMCADPGOCHM();
		KLHOKKPALOK.GMCADPGOCHM();
		NDCOLFHCNLD.GMCADPGOCHM();
		HHIFKGOJFAC.GMCADPGOCHM();
		BBMLCBEFLGI.GMCADPGOCHM();
		KEFPALGDBOC.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}

	private long GetPrice()
	{
		return (ObscuredLong)((!PLBFFNCCCGO()) ? KJFAOKLILOC : FMHECGHHKGB);
	}

	private long HIPLKBCODAH()
	{
		return (!PLBFFNCCCGO()) ? OHBBLIMNIMJ() : MCNMMBCJADI();
	}

	private long MKEHOGFBMMA()
	{
		return (ObscuredLong)(KJFAOKLILOC) * (long)GameUtils.FPBFDNBDDIE;
	}

	public long OHBBLIMNIMJ()
	{
		return (ObscuredLong)(KJFAOKLILOC);
	}

	public long MCNMMBCJADI()
	{
		return (ObscuredLong)(FMHECGHHKGB);
	}

	private int DNDHIHJPIEA()
	{
		return (int)((float)(ObscuredInt)(FOLLHACLPNB) * GECGFACDOBA());
	}

	public bool PLBFFNCCCGO()
	{
		return 0 < (ObscuredLong)(FMHECGHHKGB);
	}

	public bool INCBGIDFIDN()
	{
		return 0 < (ObscuredLong)(KJFAOKLILOC);
	}

	public bool CAIEBJHILON()
	{
		return 0 < (ObscuredInt)(FOLLHACLPNB);
	}

	public void LEKDAILCFEG()
	{
		DCHJDPCEODD = FOMPCNKEPJF;
	}

	public bool GOKHJMOEGIJ()
	{
		return GDCBBAHKCIE > 0;
	}

	public bool INEOECGAGGD()
	{
		return (ParentItem != null) ? true : false;
	}

	public virtual ItemInfo Clone()
	{
		ItemInfo dJKEECEOCJB = new ItemInfo(this);
		dJKEECEOCJB.APMJCGBNEDI.Clear();
		return dJKEECEOCJB;
	}

	private void JKJLFOAOLFI(XmlNode node)
	{
		if (!node.Attributes["Name"].Empty())
		{
			Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["PackLabel"].Empty())
		{
			MMHIKEIDDNB = node.Attributes["PackLabel"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["GroupID"].Empty())
		{
			MMHIKEIDDNB = node.Attributes["GroupID"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Image"].Empty())
		{
			FileName = node.Attributes["Image"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Model"].Empty())
		{
			KJDFJPBIGJC = node.Attributes["Model"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Type"].Empty())
		{
			Type = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["SubType"].Empty())
		{
			MDPPNGIEJGD = node.Attributes["SubType"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Text"].Empty())
		{
			GGDJIPKMKFC = node.Attributes["Text"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["TextButton"].Empty())
		{
			CGGDGCCNKJA = node.Attributes["TextButton"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["Price"].Empty())
		{
			KJFAOKLILOC = (ObscuredLong)(node.Attributes["Price"].ParseLong(0L));
		}
		if (!node.Attributes["PriceDigits"].Empty())
		{
			OJMODONDEHE = node.Attributes["PriceDigits"].ParseInt();
		}
		if (!node.Attributes["BonusPrice"].Empty())
		{
			FMHECGHHKGB = (ObscuredLong)(node.Attributes["BonusPrice"].ParseLong(0L));
		}
		if (!node.Attributes["LotteryPrice"].Empty())
		{
			FOLLHACLPNB = (ObscuredInt)(node.Attributes["LotteryPrice"].ParseInt());
		}
		if (!node.Attributes["SilentRecieve"].Empty())
		{
			GKODCKNAAHB = node.Attributes["SilentRecieve"].ParseInt();
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && (SystemProperties.LHGPKEFEHDH() || SystemProperties.MEBGOGMJFLM()) && !node.Attributes["IphoneID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["IphoneID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && !AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && SystemProperties.IPJFCBAGMJJ() && !node.Attributes["AndroidID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["AndroidID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && SystemProperties.IPJFCBAGMJJ() && !node.Attributes["ChineseID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["ChineseID"].CIPOICEEIBK(string.Empty);
		}
		if (string.IsNullOrEmpty(IIKKHJIIFEE) && SystemProperties.AFKGHBJPLOK() && !node.Attributes["WinPhoneID"].Empty())
		{
			IIKKHJIIFEE = node.Attributes["WinPhoneID"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["ConsumableProduct"].Empty())
		{
			JFOKBAKDBDA = node.Attributes["ConsumableProduct"].ParseBool();
		}
		if (!AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !node.Attributes["RealPrice"].Empty())
		{
			FPEIFLEBEAA = node.Attributes["RealPrice"].CIPOICEEIBK(string.Empty);
			EGAJMELKANL = FPEIFLEBEAA.Substring(1);
			MIIJIMJDHFP = "USD";
		}
		if (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !node.Attributes["RealPriceChina"].Empty())
		{
			FPEIFLEBEAA = node.Attributes["RealPriceChina"].CIPOICEEIBK(string.Empty);
			EGAJMELKANL = FPEIFLEBEAA.Substring(2);
			MIIJIMJDHFP = "CNY";
		}
		if (!node.Attributes["isPaid"].Empty())
		{
			MBLKNNAFCOB = node.Attributes["isPaid"].ParseBool();
		}
		if (!node.Attributes["RecieveGold"].Empty())
		{
			HHIFKGOJFAC = (ObscuredLong)(node.Attributes["RecieveGold"].ParseLong(0L));
		}
		if (!node.Attributes["RecieveBonus"].Empty())
		{
			BBMLCBEFLGI = (ObscuredLong)(node.Attributes["RecieveBonus"].ParseLong(0L));
		}
		if (!node.Attributes["CurrencyName"].Empty())
		{
			FAEGJAEEMGH = node.Attributes["CurrencyName"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["CurrencyValue"].Empty())
		{
			CPODJDDPJHB = (ObscuredInt)(node.Attributes["CurrencyValue"].ParseInt());
		}
		if (!node.Attributes["ShopHide"].Empty())
		{
			bool flag = node.Attributes["ShopHide"].ParseBool();
			DCHJDPCEODD = !flag;
			FOMPCNKEPJF = DCHJDPCEODD;
		}
		if (!node.Attributes["Hidden"].Empty())
		{
			GDCBBAHKCIE = node.Attributes["Hidden"].ParseInt();
		}
		KIGOEKKCPOJ = !node.Attributes["Level"].Empty();
		if (!node.Attributes["Level"].Empty())
		{
			MHGODOLNDLE = node.Attributes["Level"].ParseInt();
		}
		if (!node.Attributes["UpgradeLevel"].Empty())
		{
			OBJDGBBFJOO = node.Attributes["UpgradeLevel"].ParseInt();
		}
		if (!node.Attributes["SpendAfterUse"].Empty())
		{
			ANNCECNAEPN = node.Attributes["SpendAfterUse"].ParseBool();
		}
		if (!node.Attributes["DeliveryTime"].Empty())
		{
			EHKNIKHPGDN = node.Attributes["DeliveryTime"].ParseLong(0L);
		}
		if (!node.Attributes["DeliveryDescription"].Empty())
		{
			DHDDDJFLDBD = node.Attributes["DeliveryDescription"].ParseBool();
		}
		if (!node.Attributes["BonusDeliveryPrice"].Empty())
		{
			KLHOKKPALOK = (ObscuredLong)(node.Attributes["BonusDeliveryPrice"].ParseLong(0L));
		}
		if (!node.Attributes["Milestone"].Empty())
		{
			ICDIEHCJBGA = node.Attributes["Milestone"].ParseInt();
		}
		XmlNode xmlNode = node["Perks"];
		if (xmlNode != null)
		{
			DELGGDKPMKP(xmlNode);
		}
		XmlNode xmlNode2 = node["Enchantments"];
		if (xmlNode2 != null)
		{
			CKIBPGDJHNO(xmlNode2);
			BMAOLOLLBEI(xmlNode2);
		}
		if (!node.Attributes["AddPercent"].Empty())
		{
			FMOJBFNFLNM = node.Attributes["AddPercent"].ParseInt();
		}
		if (!node.Attributes["Icon"].Empty())
		{
			CMDJPAKOHMK = node.Attributes["Icon"].CIPOICEEIBK(string.Empty);
		}
		if (!node.Attributes["PaidItem"].Empty())
		{
			PBMHNMOHODB = node.Attributes["PaidItem"].CIPOICEEIBK(string.Empty);
		}
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			XmlAttribute cJBEMNNNHDM = node.Attributes[item.get_Name()];
			if (!cJBEMNNNHDM.Empty())
			{
				IBLHIAHECLK.Set(item.get_Name(), cJBEMNNNHDM.ParseInt());
			}
		}
		XmlNode xmlNode3 = node["Upgrades"];
		if (xmlNode3 != null)
		{
			PHDCGJOKBLH = xmlNode3.Attributes["Template"].CIPOICEEIBK(string.Empty);
		}
	}

	private float GECGFACDOBA()
	{
		return 1f;
	}

	public void MergeWithItem(ItemInfo item)
	{
		if (!string.IsNullOrEmpty(item.Type))
		{
			Type = item.Type;
		}
		if (!string.IsNullOrEmpty(item.MDPPNGIEJGD))
		{
			MDPPNGIEJGD = item.MDPPNGIEJGD;
		}
	}

	public void DELGGDKPMKP(XmlNode node)
	{
		NHBIJEEKALC.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				NHBIJEEKALC.Add(aCONCDFDNJH);
			}
		}
	}

	public static PerkInfoItem APPAODDDDKI(XmlNode node)
	{
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(gOHIIMFFFJI);
		if (aCONCDFDNJH != null)
		{
			if (node["Set"] != null || node["RatingEvaluation"] != null)
			{
				aCONCDFDNJH = aCONCDFDNJH.Clone(node["Set"], node["RatingEvaluation"]);
				string text = node.Attributes["Description"].CIPOICEEIBK(string.Empty);
				if (text != null && !text.Equals(string.Empty))
				{
					aCONCDFDNJH.MGNNJPBCOGD = text;
				}
			}
			return aCONCDFDNJH;
		}
		return null;
	}

	public void CKIBPGDJHNO(XmlNode node)
	{
		LFIGBCDJHPG.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				LFIGBCDJHPG.Add(aCONCDFDNJH);
			}
		}
	}

	public void JCJKLMICDIC(XmlNode node)
	{
		APPEHIAIAAM();
		if (node == null)
		{
			return;
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkInfoItem aCONCDFDNJH = APPAODDDDKI(childNode);
			if (aCONCDFDNJH != null)
			{
				BAHCGAGHPNE.Add(aCONCDFDNJH);
				NHBIJEEKALC.Add(aCONCDFDNJH);
			}
		}
	}

	public void BMAOLOLLBEI(XmlNode node)
	{
		GAHFEAAHDCL();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			PerkStruct item = new PerkStruct(childNode);
			APMJCGBNEDI.Add(item);
		}
	}

	public void GAHFEAAHDCL()
	{
		APMJCGBNEDI.Clear();
	}

	private void FHILKOAPBKG(int NPFOBKBJAOB)
	{
	}

	private void BJBEKBECHAI(int NPFOBKBJAOB)
	{
	}

	public void HNMFDILOBMJ(UpgradeData IFOFMGAKHEP)
	{
		UpgradeData item = DBPHNGLCHHO(IFOFMGAKHEP);
		KEFPALGDBOC.Add(item);
	}

	public int FMHIKMNJHDL()
	{
		int LPINKLMDEEF = int.MinValue;
		KEFPALGDBOC.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			if (DHDMNHCIPEH.OGLHOJNMEBD.AKKLOMFOLNO > LPINKLMDEEF)
			{
				LPINKLMDEEF = DHDMNHCIPEH.OGLHOJNMEBD.AKKLOMFOLNO;
			}
		});
		return LPINKLMDEEF;
	}

	public List<UpgradeData> DNFDAGFAANJ(bool NNDOJGMBEDC = false, int JELPMBDMLAB = int.MaxValue)
	{
		List<UpgradeData> list = new List<UpgradeData>();
		List<UpgradeData> list2 = new List<UpgradeData>();
		int num = FMHIKMNJHDL();
		list2.AddRange(KEFPALGDBOC);
		UpgradeDataContainer aKHJNNDCKMK = ListSF.DJBOFEEKJMP().BKPOCLGODDM(PHDCGJOKBLH);
		if (aKHJNNDCKMK != null)
		{
			foreach (UpgradeData item in aKHJNNDCKMK.KPAPEBOAKIE)
			{
				if (item.OGLHOJNMEBD.AKKLOMFOLNO > num)
				{
					list2.Add(item);
				}
			}
		}
		list2.Sort();
		foreach (UpgradeData item2 in list2)
		{
			if ((!NNDOJGMBEDC || item2.OGLHOJNMEBD.AKKLOMFOLNO > OBJDGBBFJOO) && item2.OGLHOJNMEBD.Level <= JELPMBDMLAB)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	public void HPCGCMMGAAP(UpgradeData LILLEENHNCG)
	{
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			int OEMALIFPGPO = 0;
			if (LILLEENHNCG.OGLHOJNMEBD.IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO))
			{
				IBLHIAHECLK.Set(item.get_Name(), OEMALIFPGPO);
			}
		}
		if (LILLEENHNCG.EBHOFBFKNMB.KLHOKKPALOK)
		{
			KLHOKKPALOK = LILLEENHNCG.OGLHOJNMEBD.KLHOKKPALOK;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.FMHECGHHKGB)
		{
			FMHECGHHKGB = LILLEENHNCG.OGLHOJNMEBD.FMHECGHHKGB;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.EHKNIKHPGDN)
		{
			EHKNIKHPGDN = LILLEENHNCG.OGLHOJNMEBD.EHKNIKHPGDN;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.Level)
		{
			MHGODOLNDLE = LILLEENHNCG.OGLHOJNMEBD.Level;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.ICDIEHCJBGA)
		{
			ICDIEHCJBGA = LILLEENHNCG.OGLHOJNMEBD.ICDIEHCJBGA;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.MDAAJFBENON)
		{
			KJFAOKLILOC = LILLEENHNCG.OGLHOJNMEBD.MDAAJFBENON;
		}
		if (LILLEENHNCG.EBHOFBFKNMB.AKKLOMFOLNO)
		{
			OBJDGBBFJOO = LILLEENHNCG.OGLHOJNMEBD.AKKLOMFOLNO;
		}
	}

	public ItemInfo ILDOPPMOOOF(int GNLOCMLBNHF)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			if (item.OGLHOJNMEBD.AKKLOMFOLNO == GNLOCMLBNHF)
			{
				return MPADIPJLMLH(item);
			}
		}
		return null;
	}

	public ItemInfo HIOBANJPMKF(int GNLOCMLBNHF)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			if (item.OGLHOJNMEBD.AKKLOMFOLNO >= GNLOCMLBNHF)
			{
				return MPADIPJLMLH(item);
			}
		}
		return null;
	}

	public ItemInfo GetUpdateItemByLevel(int JHLGOAFNPNM, bool GHNLHKBJOIH = true)
	{
		UpgradeData fKFLGOCPFEB = null;
		UpgradeData fKFLGOCPFEB2 = null;
		UpgradeData fKFLGOCPFEB3 = null;
		bool flag = false;
		List<UpgradeData> list = DNFDAGFAANJ();
		foreach (UpgradeData item in list)
		{
			int gCAPLEJMMPM = item.OGLHOJNMEBD.Level;
			if (!flag && gCAPLEJMMPM == JHLGOAFNPNM)
			{
				fKFLGOCPFEB = item;
				flag = true;
			}
			if (gCAPLEJMMPM > JHLGOAFNPNM)
			{
				fKFLGOCPFEB2 = fKFLGOCPFEB3;
				break;
			}
			fKFLGOCPFEB3 = item;
		}
		if (fKFLGOCPFEB == null && fKFLGOCPFEB2 == null)
		{
			return null;
		}
		UpgradeData lILLEENHNCG = ((!GHNLHKBJOIH) ? fKFLGOCPFEB : fKFLGOCPFEB2);
		return MPADIPJLMLH(lILLEENHNCG);
	}

	public ItemInfo GJAMPOFICNK(int index)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		if (0 <= index && index < list.Count)
		{
			return MPADIPJLMLH(list[index]);
		}
		LLLOJBFMONN.Error("ItemInfo.getUpdateItemByIndex wrong index: {0}", index);
		return null;
	}

	public UpgradeIndexItem MJNILIJLCMI(int OMHDLKNHNMJ, int upgradeLevel)
	{
		UpgradeIndexItem aACAFOBANOH = new UpgradeIndexItem();
		int num = 0;
		if (ParentItem != null)
		{
			List<UpgradeData> list = ParentItem.DNFDAGFAANJ();
			foreach (UpgradeData item in list)
			{
				UpgradeData.AGKOBJMBAEC oGLHOJNMEBD = item.OGLHOJNMEBD;
				if (oGLHOJNMEBD.Level == MHGODOLNDLE && oGLHOJNMEBD.AKKLOMFOLNO < OBJDGBBFJOO && oGLHOJNMEBD.AKKLOMFOLNO > ParentItem.OBJDGBBFJOO)
				{
					num++;
				}
			}
			if (ParentItem.MHGODOLNDLE == MHGODOLNDLE)
			{
				num++;
			}
		}
		if (num == 0)
		{
			aACAFOBANOH.Type = UpgradeIndexItem.LIPHFAOKLCA.UPGRADE_INDEX_MILESTONE;
			aACAFOBANOH.Index = MHGODOLNDLE;
		}
		else
		{
			aACAFOBANOH.Index = num;
		}
		return aACAFOBANOH;
	}

	public void NHJAHNDOLAE(int OMHDLKNHNMJ, int upgradeLevel, ref ItemInfo HDMHCCKLLGK, ref ItemInfo JLNLOCNBGEK)
	{
		List<UpgradeData> list = DNFDAGFAANJ();
		List<UpgradeData> list2 = new List<UpgradeData>();
		UpgradeData fKFLGOCPFEB = null;
		UpgradeData fKFLGOCPFEB2 = null;
		UpgradeData fKFLGOCPFEB3 = null;
		float num = GameUtils.HPEBEOMLHKF.GetValue(Type);
		int num2 = upgradeLevel / 100;
		foreach (UpgradeData item in list)
		{
			int aKKLOMFOLNO = item.OGLHOJNMEBD.AKKLOMFOLNO;
			if (aKKLOMFOLNO == upgradeLevel)
			{
				fKFLGOCPFEB = item;
			}
			if (item.OGLHOJNMEBD.Level <= OMHDLKNHNMJ && aKKLOMFOLNO > upgradeLevel)
			{
				if (item.OGLHOJNMEBD.ICDIEHCJBGA > 0 && (float)item.OGLHOJNMEBD.Level >= (float)num2 + num && (fKFLGOCPFEB2 == null || fKFLGOCPFEB2.OGLHOJNMEBD.AKKLOMFOLNO < aKKLOMFOLNO))
				{
					fKFLGOCPFEB2 = item;
				}
				if (item.OGLHOJNMEBD.ICDIEHCJBGA <= 0 && (fKFLGOCPFEB3 == null || fKFLGOCPFEB3.OGLHOJNMEBD.AKKLOMFOLNO > aKKLOMFOLNO))
				{
					fKFLGOCPFEB3 = item;
				}
			}
		}
		if (fKFLGOCPFEB != null)
		{
			HDMHCCKLLGK = MPADIPJLMLH(fKFLGOCPFEB);
		}
		else
		{
			HDMHCCKLLGK = null;
		}
		if (fKFLGOCPFEB2 != null)
		{
			JLNLOCNBGEK = MPADIPJLMLH(fKFLGOCPFEB2);
		}
		else if (fKFLGOCPFEB3 != null)
		{
			JLNLOCNBGEK = MPADIPJLMLH(fKFLGOCPFEB3);
		}
		else
		{
			JLNLOCNBGEK = null;
		}
	}

	public static void DenominateItems(int NPFOBKBJAOB = 0)
	{
		List<ItemInfo> list = ListSF.DJBOFEEKJMP().HCDLKHKBEPF();
		foreach (ItemInfo item in list)
		{
			item.KJFAOKLILOC = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item.KJFAOKLILOC), NPFOBKBJAOB));
			item.FHILKOAPBKG(NPFOBKBJAOB);
			item.BJBEKBECHAI(NPFOBKBJAOB);
			List<UpgradeData> kEFPALGDBOC = item.KEFPALGDBOC;
			foreach (UpgradeData item2 in kEFPALGDBOC)
			{
				item2.OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item2.OGLHOJNMEBD.MDAAJFBENON), NPFOBKBJAOB));
			}
			if (item.Type.Equals("RealMoneyItem"))
			{
				item.HHIFKGOJFAC = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item.HHIFKGOJFAC), NPFOBKBJAOB));
			}
		}
		foreach (UpgradeDataContainer item3 in ListSF.DJBOFEEKJMP().CKCGBCNMOOP())
		{
			foreach (UpgradeData item4 in item3.KPAPEBOAKIE)
			{
				item4.OGLHOJNMEBD.MDAAJFBENON = (ObscuredLong)(GameUtils.GetDenominatedValue((ObscuredLong)(item4.OGLHOJNMEBD.MDAAJFBENON), NPFOBKBJAOB));
			}
		}
		ListSF.CCDKHLAMKKO().KHCNHPCPFII().NHJAHNDOLAE();
	}

	public ItemInfo MPADIPJLMLH(UpgradeData LILLEENHNCG)
	{
		if (LILLEENHNCG == null)
		{
			return null;
		}
		ItemInfo dJKEECEOCJB = Clone();
		dJKEECEOCJB.ParentItem = this;
		dJKEECEOCJB.HPCGCMMGAAP(LILLEENHNCG);
		return dJKEECEOCJB;
	}

	private void APPEHIAIAAM()
	{
		BAHCGAGHPNE.ForEach((PerkInfoItem DHDMNHCIPEH) =>
		{
			MLOOKBFCOHM(DHDMNHCIPEH);
		});
		BAHCGAGHPNE.Clear();
	}

	private void MLOOKBFCOHM(PerkInfoItem DPLEGFCHOCE)
	{
		NHBIJEEKALC.Remove(DPLEGFCHOCE);
	}

	private void Init()
	{
		KJFAOKLILOC = (ObscuredLong)(0L);
		FMHECGHHKGB = (ObscuredLong)(0L);
		FOLLHACLPNB = (ObscuredInt)(0);
		HHIFKGOJFAC = (ObscuredLong)(0L);
		BBMLCBEFLGI = (ObscuredLong)(0L);
		FAEGJAEEMGH = string.Empty;
		CPODJDDPJHB = (ObscuredInt)(0);
		DCHJDPCEODD = true;
		GDCBBAHKCIE = 0;
		MHGODOLNDLE = 0;
		OBJDGBBFJOO = 0;
		ACOIHHPOBDH = false;
		ANNCECNAEPN = false;
		NNLMNNAEDIE = 0L;
		PEGDPDINDDO = 0L;
		MBLKNNAFCOB = false;
		EHKNIKHPGDN = 0L;
		DHDDDJFLDBD = false;
		KLHOKKPALOK = (ObscuredLong)(0L);
		NDCOLFHCNLD = (ObscuredLong)(0L);
		ICDIEHCJBGA = 0;
		GKODCKNAAHB = 0;
		ParentItem = null;
		FMOJBFNFLNM = 0;
		OJMODONDEHE = 0;
		DGOMAGNAMMD = false;
		CMDJPAKOHMK = string.Empty;
		GNDLEFFMJDJ = false;
		PBMHNMOHODB = "None";
		PPGBMODEAGD = true;
	}

	private UpgradeData DBPHNGLCHHO(UpgradeData IFOFMGAKHEP)
	{
		UpgradeData fKFLGOCPFEB = new UpgradeData(IFOFMGAKHEP);
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			int OEMALIFPGPO = 0;
			if (IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO) && !fKFLGOCPFEB.OGLHOJNMEBD.IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO))
			{
				IBLHIAHECLK.Get(item.get_Name(), ref OEMALIFPGPO);
				fKFLGOCPFEB.OGLHOJNMEBD.IBLHIAHECLK.Set(item.get_Name(), OEMALIFPGPO);
			}
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.KLHOKKPALOK)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.KLHOKKPALOK = KLHOKKPALOK;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.FMHECGHHKGB)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.FMHECGHHKGB = FMHECGHHKGB;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.EHKNIKHPGDN)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.EHKNIKHPGDN = EHKNIKHPGDN;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.Level)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.Level = MHGODOLNDLE;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.ICDIEHCJBGA)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.ICDIEHCJBGA = ICDIEHCJBGA;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.MDAAJFBENON)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.MDAAJFBENON = KJFAOKLILOC;
		}
		if (!fKFLGOCPFEB.EBHOFBFKNMB.AKKLOMFOLNO)
		{
			fKFLGOCPFEB.OGLHOJNMEBD.AKKLOMFOLNO = OBJDGBBFJOO;
		}
		return fKFLGOCPFEB;
	}

	public void IEIKLANLOPL(ProductMetadata CFDFJHGLMNH)
	{
		FPEIFLEBEAA = CFDFJHGLMNH.localizedPriceString;
		EGAJMELKANL = CFDFJHGLMNH.localizedPrice.ToString();
		MIIJIMJDHFP = CFDFJHGLMNH.isoCurrencyCode;
	}

	public void GEEGNGNLPGO()
	{
		KEFPALGDBOC.Sort();
		int index = 0;
		KEFPALGDBOC.ForEach((UpgradeData DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.UpgradeIndex = index;
			index++;
		});
	}
}
