using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core;
using Nekki.SF2.Core.Quests;
using Nekki.SF2.GUI.Menu;
using Nekki.Utils;
using UnityEngine;

public class ListSF
{
	public enum BKDHBIDPKLK
	{
		CHECK_ITEM_NONE = 0,
		CHECK_ITEM_LEVEL = 1,
		CHECK_ITEM_MONEY = 2,
		CHECK_ITEM_BONUS = 3,
		CHECK_ITEM_NO_NETWORK = 4,
		CHECK_ITEM_MATERIALS = 5
	}

	public class CheckItems
	{
		public BKDHBIDPKLK Type;

		public long Value;
	}

	public class TemplateUser
	{
		public ModelParameters KEJDJHAGBMK;

		public XmlNode node;

		public string IJBOAGICOON;

		public string name;
	}

	private class GroupsUser
	{
		public List<ModelParameters> FJMBBMJMOKC = new List<ModelParameters>();

		public XmlNode node;

		public string name;

		public string IJBOAGICOON;

		public int Size
		{
			get
			{
				return OLINNGEMHMG();
			}
		}

		public ModelParameters PNNMLIGMMPG()
		{
			int index = NekkiMath.randomInt(FJMBBMJMOKC.Count);
			return FJMBBMJMOKC[index];
		}

		public ModelParameters PNNMLIGMMPG(List<int> AOKENODDIEN)
		{
			int num = NekkiMath.randomInt(FJMBBMJMOKC.Count);
			bool flag;
			do
			{
				flag = false;
				foreach (int item in AOKENODDIEN)
				{
					if (item == num)
					{
						num = NekkiMath.randomInt(FJMBBMJMOKC.Count);
						flag = true;
					}
				}
			}
			while (flag);
			AOKENODDIEN.Add(num);
			return FJMBBMJMOKC[num];
		}

		public int OLINNGEMHMG()
		{
			return FJMBBMJMOKC.Count;
		}
	}

	private static ListSF _instance = null;

	private static Roster ANEHEDFAPCH = null;

	private static Items _items = new Items();

	private QuestsManager _QuestsManager = QuestsManager.get_Instance();

	public const string ILAAAOHLICG = "Coins";

	public const string DMOAJEIAHDO = "Ruby";

	public const string JIKLMGFDPNO = "Connection";

	public const string BKIGCDBEJPF = "Verification";

	public const string HNIAJBFIJFA = "ServerNoResponse";

	public const string HIPNPNJFNEM = "Materials";

	public const string OHOBMDAOGHE = "TryAgain";

	public static bool LMJMPAOGKJF;

	private IEnumerator KFFLKFPPKHO;

	public QuestParameters HAOHNNFLOGK = new QuestParameters();

	public AdvertConfig NAMLKEMHPHJ = new AdvertConfig();

	public bool JGFGMICMBKL;

	private List<ModelParameters> FCBEJGEKOLA = new List<ModelParameters>();

	private List<KeyValuePair<string, BattleType>> GEDIPKEENJC = new List<KeyValuePair<string, BattleType>>();

	private static bool PMJLDBCABPO;

	private static bool KODPBMFMOLB;

	private static bool PFPEPKJBJGC;

	private static bool FMDDGHBNGHG;

	private static bool IHNFKALCJGJ;

	private XmlDocument IEDEFCBFJAD;

	private XmlNode _CurrentUserNode;

	private XmlDocument IEGJHNHFJFA;

	private XmlDocument ONBBADOAPIB;

	private static long HJOHKOEICAP;

	private long AIJKJHMICNH;

	private List<TemplateUser> HBOHFLOJMAA = new List<TemplateUser>();

	private List<GroupsUser> FAPGPFLOFEE = new List<GroupsUser>();

	private List<Zone> CMEABHLEKNH = new List<Zone>();

	private List<Zone> KHLBNALFOGN = new List<Zone>();

	private List<Zone> CNMGADBPPJK = new List<Zone>();

	private List<Battle> _battles = new List<Battle>();

	private List<FightList> JNPMCNMEOLE = new List<FightList>();

	private static List<BattleReplayable> DMNAHDJIBOP = new List<BattleReplayable>();

	public static bool GKAOOOICJAI = false;

	private bool GJEJCLBAPMP;

	private static bool OEAPBNJBCKP;

	public static string BCNELIKKKLH
	{
		get
		{
			return PFMBKJMEDEF();
		}
	}

	public static string LIHHHPFNEPC
	{
		get
		{
			return IDIFECNLMKO();
		}
	}

	public static string HNPPDODPFFH
	{
		get
		{
			return OPBLKCABALC();
		}
	}

	public static string DPNDJHNALGN
	{
		get
		{
			return GPKBMLALFIM();
		}
	}

	public static ListSF BPCBBHAKFDM
	{
		get
		{
			return ELEBLBJKDBI();
		}
	}

	public static Roster MHINEBPCMLE
	{
		get
		{
			return CCDKHLAMKKO();
		}
	}

	public static Items OJIAKDDCGLB
	{
		get
		{
			return DJBOFEEKJMP();
		}
	}

	public static ModelParameters IECEIBNKFIK
	{
		get
		{
			return GAMMAIGEIOB();
		}
	}

	public static long EFGOAPHFFNH
	{
		get
		{
			return BLBNJKJKMBM();
		}
	}

	public static Zone PHLHELKODOG
	{
		get
		{
			return MGABNFOMDGB();
		}
	}

	private int LNPFCFBMHEM
	{
		get
		{
			return HFPJDOEEDCA();
		}
	}

	public List<Battle> LGIIBNJFADA
	{
		get
		{
			return MMCHMBIKIEP();
		}
	}

	public static string PFMBKJMEDEF()
	{
		return string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
	}

	public static string IDIFECNLMKO()
	{
		return string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.GHKPPHAAMBL);
	}

	public static string OPBLKCABALC()
	{
		return string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.BICBNMCJFLK);
	}

	public static string GPKBMLALFIM()
	{
		return string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), "assets/packs.xml");
	}

	public static ListSF ELEBLBJKDBI()
	{
		if (_instance == null)
		{
			_instance = new ListSF();
		}
		return _instance;
	}

	public static Roster CCDKHLAMKKO()
	{
		return ANEHEDFAPCH;
	}

	public static Items DJBOFEEKJMP()
	{
		return _items;
	}

	public static void Reset()
	{
		_instance = null;
		ANEHEDFAPCH = null;
		_items = new Items();
		QuestsManager.Reset();
	}

	public void IIKDNMBIHCM()
	{
		EAFEBFMIDLF();
		GameUtils.OJNHPHEPFLI.AEPHNNABOEK();
		NMMBHENGDJO();
		PBNNPBEDOOJ();
		ItemInfo.DenominateItems();
		OFIPOGGCKIN();
		KIEEPEOPJGB();
		JGFGMICMBKL = true;
		PDCHBPKOBFI(string.Empty);
		LDADJAGGGPA();
		PacksController.ELEBLBJKDBI().GDNFPIBDDBO();
		IEGJHNHFJFA = null;
		GlobalTimer.get_Instance().addEventListener(0, ILFBDHDMHPD);
	}

	public void IAAELKAKHPN()
	{
		FMDDGHBNGHG = false;
		CEPOJOPGFIG();
	}

	public void MHOCDMBMALI()
	{
		bool flag = true;
		bool flag2 = true;
		LLLOJBFMONN.INNGABABJPC("Login sequence: initSocial");
		if (flag)
		{
			FMDDGHBNGHG = false;
			if (ANEHEDFAPCH.DFNEGEEHLFJ())
			{
				if ((ANEHEDFAPCH.DHPNBBILDPB() && !SystemProperties.AFKGHBJPLOK()) || GameCenterController.CPOLMPAAHOL())
				{
					Debug.Log("Login sequence: GameCenterController.SignIn");
					KOGNPNFOPIE();
					GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Combine(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
					GameCenterController.EFKOIIKEHDO();
				}
				else
				{
					Debug.Log("Login sequence: skip social");
					CEPOJOPGFIG();
				}
			}
			else
			{
				Debug.Log("Login sequence: skip social");
				CEPOJOPGFIG();
			}
		}
		if (!flag2)
		{
		}
	}

	public static ItemInfo PGKBAEGCABK(string GLLLLNHKCOF, long HNPMAENBMCO = 0L)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<ItemInfo> list2 = DJBOFEEKJMP().HCDLKHKBEPF();
		foreach (ItemInfo item in list2)
		{
		}
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
			foreach (ItemInfo item2 in list)
			{
				if (item2.MMHIKEIDDNB == string.Empty || nKGLHEGIKKP.FLFKOIPCEPI(item2.MMHIKEIDDNB))
				{
					return item2;
				}
			}
		}
		return null;
	}

	public static void CDCHFKPDDFH(ItemInfo FAKOMBAIFPP)
	{
		UserItem dKCHDHMLKHN = ((FAKOMBAIFPP == null) ? null : CMGOCLGHNLH(FAKOMBAIFPP.Name));
		if (FAKOMBAIFPP != null && dKCHDHMLKHN == null)
		{
			Roster aNEHEDFAPCH = ANEHEDFAPCH;
			aNEHEDFAPCH.PBEJGHOIPKC(aNEHEDFAPCH.INPAOPFFKEJ() + FAKOMBAIFPP.EGAJMELKANL.ToFloat());
			Roster aNEHEDFAPCH2 = ANEHEDFAPCH;
			aNEHEDFAPCH2.IKIHAIKLLOK(aNEHEDFAPCH2.MNDJBCMLJHF() + 1);
			if (FAKOMBAIFPP.MBLKNNAFCOB)
			{
				Roster aNEHEDFAPCH3 = ANEHEDFAPCH;
				aNEHEDFAPCH3.HGDLPMDHHOJ((ObscuredLong)((ObscuredLong)(aNEHEDFAPCH3.KNHDCEBIMEE()) + (ObscuredLong)(FAKOMBAIFPP.HHIFKGOJFAC)));
			}
			ANEHEDFAPCH.OIOOMAKNIOB(ANEHEDFAPCH.BFBOEGMAMNF() + (ObscuredLong)(FAKOMBAIFPP.HHIFKGOJFAC));
			ANEHEDFAPCH.LLNELLFMMBB(ANEHEDFAPCH.EHFJHFDACMP() + (ObscuredLong)(FAKOMBAIFPP.BBMLCBEFLGI), Roster.HPOIJPGPOCF.CHANGE_PAYMENT, true);
			if (MainMenu.get_Instance() != null)
			{
				MainMenu.get_Instance().UpdateMoney();
			}
			switch (FAKOMBAIFPP.MDPPNGIEJGD)
			{
			case "StarterPack":
				GEFDJDIINND(FAKOMBAIFPP, 1, 0L, false);
				break;
			case "UnlimitedEnergy":
				GEFDJDIINND(FAKOMBAIFPP, 1, 0L, false);
				break;
			case "PerkReset":
				GEFDJDIINND(FAKOMBAIFPP, 1, 0L, false);
				ANEHEDFAPCH.JLBDOBLHHAF().LCDFOLAAEGM();
				break;
			}
			ANEHEDFAPCH.GGGEHAGCLGC(true);
		}
	}

	public static ItemInfo CKCMJAJAELO(string FDKNIPNGFNF)
	{
		List<ItemInfo> list = DJBOFEEKJMP().CKCMJAJAELO(FDKNIPNGFNF);
		if (list.Count > 0 && ANEHEDFAPCH != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				if (string.IsNullOrEmpty(list[i].MMHIKEIDDNB) || ANEHEDFAPCH.FLFKOIPCEPI(list[i].MMHIKEIDDNB))
				{
					return list[i];
				}
			}
		}
		return null;
	}

	public List<ModelParameters> APOACOEFALC(ModelParameters KKNOCIPBIIK, int count)
	{
		List<ModelParameters> list = new List<ModelParameters>();
		if (!KKNOCIPBIIK.IJKINPHBHCF())
		{
			for (int i = 0; i < count; i++)
			{
				list.Add(KKNOCIPBIIK.Clone());
			}
		}
		else
		{
			int num = 0;
			List<List<int>> list2 = new List<List<int>>();
			list2.CPCAJIKOIEE(KKNOCIPBIIK.KFKKHACFDPH.Count);
			for (int j = 0; j < count; j++)
			{
				int num2 = 0;
				ModelParameters kIKOGDEPGHB = null;
				foreach (GroupModel item in KKNOCIPBIIK.KFKKHACFDPH)
				{
					GroupsUser oFIHDFNGDLA = HJDCNPMEHGJ(item.Name);
					if (oFIHDFNGDLA != null)
					{
						ModelParameters kIKOGDEPGHB2 = null;
						if (!item.IsRandom)
						{
							int index = j % oFIHDFNGDLA.OLINNGEMHMG();
							kIKOGDEPGHB2 = oFIHDFNGDLA.FJMBBMJMOKC[index];
						}
						else if (!item.PMHHMDAIOGL)
						{
							kIKOGDEPGHB2 = oFIHDFNGDLA.PNNMLIGMMPG();
						}
						else
						{
							kIKOGDEPGHB2 = oFIHDFNGDLA.PNNMLIGMMPG(list2[num2]);
							int count2 = oFIHDFNGDLA.FJMBBMJMOKC.Count;
							if (num >= count2 - 1)
							{
								list2[num2].Clear();
								num = 0;
							}
							else
							{
								num++;
							}
						}
						if (kIKOGDEPGHB2 != null)
						{
							if (kIKOGDEPGHB == null)
							{
								kIKOGDEPGHB = kIKOGDEPGHB2;
							}
							else
							{
								kIKOGDEPGHB = IAOBIMJFBMH(kIKOGDEPGHB2.Node, kIKOGDEPGHB);
								OLHPLOMLKLE(kIKOGDEPGHB);
							}
						}
					}
					num2++;
				}
				if (kIKOGDEPGHB != null)
				{
					ModelParameters kIKOGDEPGHB3 = null;
					if (KKNOCIPBIIK.Node.Attributes["Template"] == null)
					{
						kIKOGDEPGHB3 = IAOBIMJFBMH(KKNOCIPBIIK.Node, kIKOGDEPGHB);
					}
					else
					{
						kIKOGDEPGHB3 = ((kIKOGDEPGHB == null) ? new ModelParameters() : kIKOGDEPGHB.Clone());
						PLEDFOHECDC(kIKOGDEPGHB3, KKNOCIPBIIK);
					}
					list.Add(kIKOGDEPGHB3);
				}
			}
		}
		if (list.Count == 0)
		{
			list.Add(KKNOCIPBIIK.Clone());
		}
		return list;
	}

	public static ModelParameters GAMMAIGEIOB()
	{
		return CCDKHLAMKKO().get_Parameters();
	}

	public static long IDMJOMOMDOJ()
	{
		return HJOHKOEICAP;
	}

	public static long BLBNJKJKMBM()
	{
		return 0L;
	}

	public static CheckItems CLKECIFEMNB(ItemInfo item, ItemAction LFLGCDNKNJI, int count = 1)
	{
		ELEBLBJKDBI().JLEMHLLLCLD();
		CheckItems bJEBPDNMNAE = new CheckItems();
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		long num = 0L;
		long num2 = 0L;
		bool flag = true;
		switch (LFLGCDNKNJI)
		{
		case ItemAction.Item_Buy_Gold:
		case ItemAction.Item_Upgrade_Gold:
			num = nKGLHEGIKKP.BFBOEGMAMNF();
			num2 = item.OHBBLIMNIMJ() * count;
			break;
		case ItemAction.Item_Buy_Ruby:
		case ItemAction.Item_Upgrade_Ruby:
			num = nKGLHEGIKKP.EHFJHFDACMP();
			num2 = item.MCNMMBCJADI() * count;
			break;
		case ItemAction.Item_Buy_Real:
		case ItemAction.Item_Free:
			num = 0L;
			num2 = 0L;
			break;
		case ItemAction.Item_Order_Ruby:
		case ItemAction.Item_Delivery_Ruby:
		case ItemAction.Item_Recipe_Delivery_Ruby:
			num = nKGLHEGIKKP.EHFJHFDACMP();
			num2 = (ObscuredLong)(item.KLHOKKPALOK);
			break;
		case ItemAction.Item_Consumable:
			num = nKGLHEGIKKP.EHFJHFDACMP();
			num2 = item.MCNMMBCJADI() * count;
			break;
		case ItemAction.Item_Recipe:
		{
			RecipeItemInfo bNJOCBKNPMG = (RecipeItemInfo)item;
			Recipe iNODIOJPNJH = bNJOCBKNPMG.OIMGNCLBPHD();
			UserItem nDMCFNGEPOA = bNJOCBKNPMG.MFEAIEJFDAM();
			flag = iNODIOJPNJH.IHHJGMBGHEB(nDMCFNGEPOA);
			break;
		}
		default:
			LLLOJBFMONN.Error("ListSF::isBuyItem - unknown type: %i", LFLGCDNKNJI);
			break;
		}
		long num3 = num - num2;
		bJEBPDNMNAE.Type = BKDHBIDPKLK.CHECK_ITEM_NONE;
		bJEBPDNMNAE.Value = num3;
		if ((LFLGCDNKNJI == ItemAction.Item_Buy_Real || LFLGCDNKNJI == ItemAction.Item_Free) && !SystemProperties.PKLFCFBEIIG())
		{
			bJEBPDNMNAE.Type = BKDHBIDPKLK.CHECK_ITEM_NO_NETWORK;
			bJEBPDNMNAE.Value = -1L;
		}
		else if (item.MHGODOLNDLE > nKGLHEGIKKP.PINDEKDNCNL())
		{
			bJEBPDNMNAE.Type = BKDHBIDPKLK.CHECK_ITEM_LEVEL;
			bJEBPDNMNAE.Value = -1L;
		}
		else if (num3 < 0)
		{
			bJEBPDNMNAE.Type = ((LFLGCDNKNJI != ItemAction.Item_Buy_Gold && LFLGCDNKNJI != ItemAction.Item_Upgrade_Gold) ? BKDHBIDPKLK.CHECK_ITEM_BONUS : BKDHBIDPKLK.CHECK_ITEM_MONEY);
			bJEBPDNMNAE.Value = -1L;
		}
		else if (num2 < 0)
		{
			bJEBPDNMNAE.Type = BKDHBIDPKLK.CHECK_ITEM_NONE;
			bJEBPDNMNAE.Value = -1L;
		}
		else if (LFLGCDNKNJI == ItemAction.Item_Recipe && !flag)
		{
			bJEBPDNMNAE.Type = BKDHBIDPKLK.CHECK_ITEM_MATERIALS;
			bJEBPDNMNAE.Value = -1L;
		}
		return bJEBPDNMNAE;
	}

	public static bool KCBCGDFKNME(ItemInfo item, ItemAction LFLGCDNKNJI, long FLCBMGGIDDA, int count = 1, Action<object> callback = null)
	{
		if (item != null)
		{
			Roster nKGLHEGIKKP = CCDKHLAMKKO();
			long num = GameUtils.ECCPJAPIABG();
			long bMNFPNBAMAF = -1L;
			if (item.Type == "RaidItemPack")
			{
			}
			switch (LFLGCDNKNJI)
			{
			case ItemAction.Item_Buy_Gold:
			case ItemAction.Item_Upgrade_Gold:
				MBBMOKFGABP(item);
				nKGLHEGIKKP.OIOOMAKNIOB(FLCBMGGIDDA);
				bMNFPNBAMAF = ((item.EHKNIKHPGDN == 0) ? (-1) : (num + item.EHKNIKHPGDN));
				break;
			case ItemAction.Item_Buy_Ruby:
			case ItemAction.Item_Upgrade_Ruby:
				BLNHEMCHIGF(item, false);
				nKGLHEGIKKP.LLNELLFMMBB(FLCBMGGIDDA, Roster.HPOIJPGPOCF.CHANGE_BUY_ITEM);
				break;
			case ItemAction.Item_Buy_Real:
				return EMEMDEAEMCB(item);
			case ItemAction.Item_Free:
				return HJHCCBGILAJ(item);
			case ItemAction.Item_Consumable:
				BLNHEMCHIGF(item, false);
				BDNBHBOJLDN(item, FLCBMGGIDDA);
				break;
			case ItemAction.Item_Delivery_Ruby:
			case ItemAction.Item_Recipe_Delivery_Ruby:
				BLNHEMCHIGF(item, true);
				nKGLHEGIKKP.LLNELLFMMBB(FLCBMGGIDDA, Roster.HPOIJPGPOCF.CHANGE_BUY_DELIVERY);
				bMNFPNBAMAF = 0L;
				break;
			default:
				LLLOJBFMONN.Error("ListSF::buyItem - unknown type: %i", LFLGCDNKNJI);
				break;
			}
			if (LFLGCDNKNJI == ItemAction.Item_Recipe_Delivery_Ruby)
			{
				PIFPAMKOPFK((RecipeItemInfo)item, LFLGCDNKNJI, FLCBMGGIDDA, bMNFPNBAMAF);
			}
			else
			{
				IGLBLDKOMML(item, LFLGCDNKNJI, FLCBMGGIDDA, bMNFPNBAMAF);
			}
			GameUtils.OFOKPNFGDMD("Virtual Good Purchased");
			MenuController.IAMGKKOINFC();
			ABKBFADGNBM(LFLGCDNKNJI);
		}
		return true;
	}

	public static bool IGLBLDKOMML(ItemInfo item, ItemAction LFLGCDNKNJI, long FLCBMGGIDDA, long BMNFPNBAMAF, int count = 1)
	{
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		UserItem dKCHDHMLKHN = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(item.Name);
		long bMNFPNBAMAF = ((dKCHDHMLKHN == null) ? (-1) : dKCHDHMLKHN.IJGAOHJNLAH());
		bool flag = BMNFPNBAMAF <= 0 || item.EHKNIKHPGDN == 0;
		UserItem dKCHDHMLKHN2 = GEFDJDIINND(item, count, BMNFPNBAMAF, flag);
		bool jBCMFEPAKLK = dKCHDHMLKHN2.GKGIKMCMCPB() || flag;
		if (item.Type == "RealMoneyItem" || item.Type == "Consumable")
		{
			jBCMFEPAKLK = false;
		}
		AFGHCIDFAHB(dKCHDHMLKHN2, jBCMFEPAKLK);
		GameUtils.LFKOMCMPKKC(item, LFLGCDNKNJI, count, bMNFPNBAMAF);
		QuestParameters hHKLFIIBIFF = ELEBLBJKDBI().BNMLDPNCMLB();
		FightIDS jLGLBLDPAAF = hHKLFIIBIFF.JLGLBLDPAAF;
		hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
		hHKLFIIBIFF.HEIADONEACH = string.Empty;
		hHKLFIIBIFF.DLKPBAJDHBO = dKCHDHMLKHN2.BHKHOJPANHE();
		if (ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE))
		{
			ELEBLBJKDBI().MHHNIPBJNAD();
		}
		hHKLFIIBIFF.JLGLBLDPAAF = jLGLBLDPAAF;
		return true;
	}

	public static bool PIFPAMKOPFK(RecipeItemInfo AJBJAPPEAFH, ItemAction LFLGCDNKNJI, long FLCBMGGIDDA, long BMNFPNBAMAF, int count = 1)
	{
		return true;
	}

	public static bool AFGHCIDFAHB(UserItem NDMCFNGEPOA, bool JBCMFEPAKLK, bool CMDMHFKJBHB = true)
	{
		if (NDMCFNGEPOA == null)
		{
			return false;
		}
		NDMCFNGEPOA.CDFODJBJIPI(CCDKHLAMKKO().PINDEKDNCNL());
		bool aNNCECNAEPN = NDMCFNGEPOA.BHKHOJPANHE().ANNCECNAEPN;
		if (aNNCECNAEPN)
		{
			EFPHIJGNGKP(NDMCFNGEPOA, NDMCFNGEPOA.OFOPFCJNEBL());
		}
		JBCMFEPAKLK = JBCMFEPAKLK && !aNNCECNAEPN;
		LLLOJBFMONN.Write("   UseItem: " + ((!JBCMFEPAKLK) ? "Unequiped " : "Equiped ") + NDMCFNGEPOA.get_Name());
		NDMCFNGEPOA.JBLKCIBKMKB(JBCMFEPAKLK);
		if (CMDMHFKJBHB)
		{
			return CCDKHLAMKKO().BMADIJMPENJ(NDMCFNGEPOA, JBCMFEPAKLK);
		}
		return true;
	}

	public static void CNMDDKCANCJ(ItemInfo item)
	{
		if (item == null)
		{
			return;
		}
		string kKJHFNGGFCG = item.Type;
		string mDPPNGIEJGD = item.MDPPNGIEJGD;
		switch (kKJHFNGGFCG)
		{
		case "Weapon":
		{
			UserItem nDMCFNGEPOA2 = CMGOCLGHNLH("Fists");
			AFGHCIDFAHB(nDMCFNGEPOA2, true);
			break;
		}
		case "Armor":
		{
			UserItem nDMCFNGEPOA6 = CMGOCLGHNLH("Body");
			AFGHCIDFAHB(nDMCFNGEPOA6, true);
			break;
		}
		case "Helm":
		{
			UserItem nDMCFNGEPOA5 = CMGOCLGHNLH("Head");
			AFGHCIDFAHB(nDMCFNGEPOA5, true);
			break;
		}
		case "Ranged":
		{
			UserItem nDMCFNGEPOA4 = CMGOCLGHNLH("NoRanged");
			AFGHCIDFAHB(nDMCFNGEPOA4, true);
			break;
		}
		case "Magic":
		{
			UserItem nDMCFNGEPOA3 = CMGOCLGHNLH("NoMagic");
			AFGHCIDFAHB(nDMCFNGEPOA3, true);
			break;
		}
		case "RaidConsumable":
			if (mDPPNGIEJGD == "RaidCharge")
			{
				UserItem nDMCFNGEPOA = CMGOCLGHNLH("NoRaidCharge");
				AFGHCIDFAHB(nDMCFNGEPOA, true);
			}
			break;
		}
	}

	public static bool EFPHIJGNGKP(UserItem item, int count = 1)
	{
		if (count > item.OFOPFCJNEBL())
		{
			count = item.OFOPFCJNEBL();
		}
		ItemInfo.MEFIBHIDOLA mEFIBHIDOLA = ItemInfo.MEFIBHIDOLA.SPEND_TYPE_NONE;
		if (item.BHKHOJPANHE().Type == "Energy")
		{
			mEFIBHIDOLA = ItemInfo.MEFIBHIDOLA.SPEND_TYPE_ENERGY;
		}
		if (mEFIBHIDOLA == ItemInfo.MEFIBHIDOLA.SPEND_TYPE_ENERGY)
		{
			ANEHEDFAPCH.OMJDCEEEJMB();
			item.CHILOKHFALD(item.OFOPFCJNEBL() - count);
			return true;
		}
		LLLOJBFMONN.Write("Error - no such SpendItemType");
		return false;
	}

	public static void JALMHIICOPB(ItemInfo PJDAGCBPLJE)
	{
		CCDKHLAMKKO().JALMHIICOPB(PJDAGCBPLJE);
	}

	public static void FAAAGBACKAE(ItemInfo item)
	{
		List<UserItem> list = CCDKHLAMKKO().KHCNHPCPFII().HOPBBLJLHOB(item.Type, string.Empty, false);
		foreach (UserItem userItem in list)
		{
			if (userItem.EFMFGEPDAOP() && userItem.get_Name() != userItem.OFMCNLBFIDF.Name)
			{
				AFGHCIDFAHB(userItem, false);
			}
		}
	}

	public static bool IMLFOOIBLJA(ItemInfo item, int count = 1)
	{
		if (item == null)
		{
			return false;
		}
		UserItem mBIJKDIEFIF = GEFDJDIINND(item, count, 0L, false);
		return CCDKHLAMKKO().BMADIJMPENJ(mBIJKDIEFIF, false);
	}

	public static bool GEOPLDMBCBD(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		UserItem dKCHDHMLKHN = CMGOCLGHNLH(item.Name);
		dKCHDHMLKHN.set_DeliveryTime(0L);
		dKCHDHMLKHN.BAMLNLIDEBG(-1);
		item.BEBDMOEIEJN(true);
		return dKCHDHMLKHN != null && ANEHEDFAPCH.BMADIJMPENJ(dKCHDHMLKHN, false);
	}

	public static UserItem GEFDJDIINND(ItemInfo item, int count = 1, long CNIOCCCBDBJ = 0L, bool JBCMFEPAKLK = true, bool KGOIDDCKBKI = true)
	{
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		if (item.MDPPNGIEJGD == "UnlimitedEnergy")
		{
			nKGLHEGIKKP.ADKHNLAMDJP = true;
			MenuController.ADPMENDMMKJ();
		}
		UserItem dKCHDHMLKHN = CMGOCLGHNLH(item.Name);
		int oMHDLKNHNMJ = nKGLHEGIKKP.PINDEKDNCNL();
		if (dKCHDHMLKHN == null)
		{
			XmlNode fMBDAPOMFGN = nKGLHEGIKKP.BABKABBEFEL();
			int bLJGEOEHIGP = ((CNIOCCCBDBJ <= 0) ? count : 0);
			UserItem dKCHDHMLKHN2 = new UserItem(fMBDAPOMFGN, item.Name, JBCMFEPAKLK, bLJGEOEHIGP, -1, CNIOCCCBDBJ);
			dKCHDHMLKHN2.KIGHKCOCJFJ(item);
			dKCHDHMLKHN = nKGLHEGIKKP.KHCNHPCPFII().GEFDJDIINND(dKCHDHMLKHN2);
			dKCHDHMLKHN.CDFODJBJIPI(oMHDLKNHNMJ);
			dKCHDHMLKHN.IJCEKDCPBAG(false);
			if (KGOIDDCKBKI)
			{
				dKCHDHMLKHN.PJEEGECBHMH();
			}
		}
		else if (dKCHDHMLKHN.OFOPFCJNEBL() != 0 || CNIOCCCBDBJ <= 0)
		{
			if (dKCHDHMLKHN.OFOPFCJNEBL() != 0)
			{
				dKCHDHMLKHN.IJCEKDCPBAG(true);
			}
			if (item.ParentItem == null)
			{
				dKCHDHMLKHN.CHILOKHFALD(dKCHDHMLKHN.OFOPFCJNEBL() + count);
			}
			if (CNIOCCCBDBJ <= 0)
			{
				dKCHDHMLKHN.FMMDLMGHPIB(item.OBJDGBBFJOO);
				dKCHDHMLKHN.CDFODJBJIPI(oMHDLKNHNMJ);
			}
			dKCHDHMLKHN.set_DeliveryTime(CNIOCCCBDBJ);
			dKCHDHMLKHN.BAMLNLIDEBG(item.OBJDGBBFJOO);
		}
		else
		{
			dKCHDHMLKHN.CDFODJBJIPI(oMHDLKNHNMJ);
			dKCHDHMLKHN.set_DeliveryTime(CNIOCCCBDBJ);
			dKCHDHMLKHN.BAMLNLIDEBG(-1);
			dKCHDHMLKHN.IJCEKDCPBAG(false);
		}
		if (JBCMFEPAKLK)
		{
			AFGHCIDFAHB(dKCHDHMLKHN, true);
		}
		return dKCHDHMLKHN;
	}

	public static void ADIFNIKODHH(UserItem NDMCFNGEPOA, int count = 1)
	{
		if (NDMCFNGEPOA == null)
		{
			return;
		}
		int num = NDMCFNGEPOA.OFOPFCJNEBL();
		if (num < count)
		{
			count = num;
		}
		NDMCFNGEPOA.CHILOKHFALD(NDMCFNGEPOA.OFOPFCJNEBL() - count);
		if (NDMCFNGEPOA.OFOPFCJNEBL() == 0 && CCDKHLAMKKO().FEDNFNOMBNG(NDMCFNGEPOA.BHKHOJPANHE()))
		{
			if (NDMCFNGEPOA.BHKHOJPANHE() != null)
			{
				NDMCFNGEPOA.BHKHOJPANHE().LEKDAILCFEG();
			}
			AFGHCIDFAHB(NDMCFNGEPOA, false);
			CNMDDKCANCJ(NDMCFNGEPOA.BHKHOJPANHE());
		}
	}

	public static bool IncreaseExp(uint value)
	{
		return CCDKHLAMKKO().IDGMHHAJDMO(value);
	}

	public static bool GCPJADIMNKI(long CDCBEDDONKK)
	{
		long num = CCDKHLAMKKO().BFBOEGMAMNF() + CDCBEDDONKK;
		if (num < 0)
		{
			return false;
		}
		CCDKHLAMKKO().OIOOMAKNIOB(num);
		return true;
	}

	public static bool PNINBKEIBHO(long OKEFHDDPMEC)
	{
		if (OKEFHDDPMEC < 0)
		{
			return false;
		}
		CCDKHLAMKKO().OIOOMAKNIOB(OKEFHDDPMEC);
		return true;
	}

	public static bool FPIJEOMBFJN(long CDCBEDDONKK, Roster.HPOIJPGPOCF LFLGCDNKNJI, bool JEEOLJIFIOF = false)
	{
		long num = CCDKHLAMKKO().EHFJHFDACMP() + CDCBEDDONKK;
		if (num < 0)
		{
			return false;
		}
		CCDKHLAMKKO().LLNELLFMMBB(num, LFLGCDNKNJI, JEEOLJIFIOF);
		return true;
	}

	public static bool BMHBGNDHPIJ(long value, Roster.HPOIJPGPOCF LFLGCDNKNJI, bool JEEOLJIFIOF = false)
	{
		if (value < 0)
		{
			return false;
		}
		CCDKHLAMKKO().LLNELLFMMBB(value, LFLGCDNKNJI, JEEOLJIFIOF);
		return true;
	}

	public static bool ChangeEnergy(int value)
	{
		return CCDKHLAMKKO().ChangePower(value);
	}

	public static Zone MGABNFOMDGB()
	{
		List<Zone> cMEABHLEKNH = ELEBLBJKDBI().CMEABHLEKNH;
		foreach (Zone item in cMEABHLEKNH)
		{
			if (item.AMBLIADMEOC())
			{
				return item;
			}
		}
		List<Zone> kHLBNALFOGN = ELEBLBJKDBI().KHLBNALFOGN;
		foreach (Zone item2 in kHLBNALFOGN)
		{
			if (item2.AMBLIADMEOC())
			{
				return item2;
			}
		}
		List<Zone> cNMGADBPPJK = ELEBLBJKDBI().CNMGADBPPJK;
		foreach (Zone item3 in cNMGADBPPJK)
		{
			if (item3.AMBLIADMEOC())
			{
				return item3;
			}
		}
		return null;
	}

	public static Zone CFEDCFACBLE(string name)
	{
		List<Zone> cMEABHLEKNH = ELEBLBJKDBI().CMEABHLEKNH;
		foreach (Zone item in cMEABHLEKNH)
		{
			if (item.get_Name() == name)
			{
				return item;
			}
		}
		List<Zone> kHLBNALFOGN = ELEBLBJKDBI().KHLBNALFOGN;
		foreach (Zone item2 in kHLBNALFOGN)
		{
			if (item2.get_Name() == name)
			{
				return item2;
			}
		}
		List<Zone> cNMGADBPPJK = ELEBLBJKDBI().CNMGADBPPJK;
		foreach (Zone item3 in cNMGADBPPJK)
		{
			if (item3.get_Name() == name)
			{
				return item3;
			}
		}
		return null;
	}

	public static List<Zone> FHAIJEAPFEA()
	{
		return ELEBLBJKDBI().CMEABHLEKNH;
	}

	public static List<FightList> JEBHJOKNENP(Battle DPOOIONCEOA)
	{
		List<FightList> list = new List<FightList>();
		foreach (FightList item in ELEBLBJKDBI().JNPMCNMEOLE)
		{
			if (item.CNAOMDMIGLJ == DPOOIONCEOA)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static FightList CHMCKGCDGCM(FightIDS DIAIIPCBMFL)
	{
		if (DIAIIPCBMFL == null)
		{
			return null;
		}
		List<FightList> jNPMCNMEOLE = ELEBLBJKDBI().JNPMCNMEOLE;
		foreach (FightList item in jNPMCNMEOLE)
		{
			if (item.BCKFACGMOKC.Equals(DIAIIPCBMFL))
			{
				return item;
			}
		}
		Battle cGJCGEBPCAF = MKHAAGMJOPG(DIAIIPCBMFL);
		return (cGJCGEBPCAF == null) ? null : cGJCGEBPCAF.OEJCNHOEFIJ(DIAIIPCBMFL.EJPNIFANKDG());
	}

	public FightList AOEPHEPGLAK(string name)
	{
		FightIDS mOCEDDJOAEB = new FightIDS(name);
		mOCEDDJOAEB.SetFightIDSByString(name);
		return CHMCKGCDGCM(mOCEDDJOAEB);
	}

	public static Battle MKHAAGMJOPG(FightIDS DIAIIPCBMFL)
	{
		if (DIAIIPCBMFL == null)
		{
			return null;
		}
		Zone pKCPOJKLMOK = CFEDCFACBLE(DIAIIPCBMFL.PELHCAEAOFE());
		if (pKCPOJKLMOK != null)
		{
			return pKCPOJKLMOK.MJINKOFNIAE(DIAIIPCBMFL.CPHDPCAECJN());
		}
		return null;
	}

	public static UserItem CMGOCLGHNLH(string name)
	{
		return CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(name);
	}

	public static RosterFight IKHJKHMIPEP(FightList KGKDKENMAOA, bool FFIBGBMOMPD)
	{
		RosterFight pIGKOIFBOME = null;
		pIGKOIFBOME = ((!FFIBGBMOMPD) ? CCDKHLAMKKO().NALCLBDLBKN(KGKDKENMAOA.BCKFACGMOKC) : CCDKHLAMKKO().JJHCGOIKBCP(KGKDKENMAOA.BCKFACGMOKC));
		if (pIGKOIFBOME != null)
		{
			pIGKOIFBOME.DLDMOHEGENM(CCDKHLAMKKO().PINDEKDNCNL());
			KGKDKENMAOA.HOCFLEMFFKC(pIGKOIFBOME);
		}
		CGJCKGAFPED();
		return pIGKOIFBOME;
	}

	public static bool OPKPFKJPHNN(ConditionFight IOFGGOCEIAM)
	{
		ConditionType kKJHFNGGFCG = IOFGGOCEIAM.Type;
		if (kKJHFNGGFCG == ConditionType.ConditionFightCount)
		{
			if (!NJDLOHEFIHO(IOFGGOCEIAM))
			{
				return false;
			}
		}
		else
		{
			LLLOJBFMONN.Error("ListSF::isConditionComplete - No handler for this type: " + IOFGGOCEIAM.Type);
		}
		return true;
	}

	public static bool NJDLOHEFIHO(ConditionFight IOFGGOCEIAM)
	{
		int iICJGJJGIMC = IOFGGOCEIAM.Count;
		ConditionCompare iFNPGAFFDNC = IOFGGOCEIAM.Compare;
		int num = 0;
		bool result = true;
		ConditionSubType dCJPHFALIND = IOFGGOCEIAM.DCJPHFALIND;
		if (dCJPHFALIND == ConditionSubType.ConditionSubTypeFight)
		{
			RosterFight pIGKOIFBOME = CCDKHLAMKKO().DBMHOBPNIIA(IOFGGOCEIAM.CCILLAHEENI());
			if (pIGKOIFBOME != null)
			{
				num = pIGKOIFBOME.JAJNIKDMPPO();
			}
		}
		else
		{
			LLLOJBFMONN.Error("ListSF::isConditionFightCount - No handler for this object type: " + IOFGGOCEIAM.DCJPHFALIND);
		}
		switch (iFNPGAFFDNC)
		{
		case ConditionCompare.ConditionMoreEqually:
			result = num >= iICJGJJGIMC;
			break;
		case ConditionCompare.ConditionLessEqually:
			result = num <= iICJGJJGIMC;
			break;
		case ConditionCompare.ConditionMore:
			result = num > iICJGJJGIMC;
			break;
		case ConditionCompare.ConditionLess:
			result = num < iICJGJJGIMC;
			break;
		case ConditionCompare.ConditionEqually:
			result = num == iICJGJJGIMC;
			break;
		default:
			LLLOJBFMONN.Error("ListSF::isConditionFightCount - No handler for this compare type: " + iFNPGAFFDNC);
			break;
		}
		return result;
	}

	public static bool MHGGANGEOIA(List<ConditionFight> conditions)
	{
		int i = 0;
		for (int count = conditions.Count; i < count; i++)
		{
			if (!OPKPFKJPHNN(conditions[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static void BKDAMDOMLGK()
	{
		List<FightList> jNPMCNMEOLE = ELEBLBJKDBI().JNPMCNMEOLE;
		foreach (FightList item in jNPMCNMEOLE)
		{
			DINPFDGMEAB(item);
		}
	}

	public static void DINPFDGMEAB(FightList fight)
	{
		int eJGGHHEOGPG = fight.EJGGHHEOGPG;
		RosterFight pIGKOIFBOME = fight.FLKFFDLLBKA();
		BattleType pJMEMGHKKBM = fight.get_Type();
		if (eJGGHHEOGPG > 0 && pIGKOIFBOME != null)
		{
			if (pJMEMGHKKBM != BattleType.FightPeriodic && pJMEMGHKKBM != BattleType.FightAscension && pIGKOIFBOME.JAJNIKDMPPO() >= eJGGHHEOGPG)
			{
				if (pJMEMGHKKBM == BattleType.FightReplayable || pJMEMGHKKBM == BattleType.FightBossesReplayable || pJMEMGHKKBM == BattleType.FightFinalReplayable)
				{
					BattleReplayable bKKPCBGAEHC = (BattleReplayable)fight.CNAOMDMIGLJ;
					bKKPCBGAEHC.MJJFFAOLCCK(fight);
				}
				else
				{
					fight.PGBKNLAEANJ = ConditionStatus.StatusComplete;
				}
				return;
			}
			if (pJMEMGHKKBM == BattleType.FightAscension)
			{
				BattleAscension bGFLODNGLPK = (BattleAscension)fight.CNAOMDMIGLJ;
				bGFLODNGLPK.MJJFFAOLCCK(fight);
				return;
			}
		}
		if (MHGGANGEOIA(fight.KJILOMLMMEN()))
		{
			fight.PGBKNLAEANJ = ConditionStatus.StatusOpen;
		}
		else
		{
			fight.PGBKNLAEANJ = ConditionStatus.StatusIncomplete;
		}
	}

	public static void BOFGHBJABMK()
	{
		List<Zone> cMEABHLEKNH = ELEBLBJKDBI().CMEABHLEKNH;
		foreach (Zone item in cMEABHLEKNH)
		{
			item.CGJCKGAFPED();
		}
		List<Zone> kHLBNALFOGN = ELEBLBJKDBI().KHLBNALFOGN;
		foreach (Zone item2 in kHLBNALFOGN)
		{
			item2.CGJCKGAFPED();
		}
		List<Zone> cNMGADBPPJK = ELEBLBJKDBI().CNMGADBPPJK;
		foreach (Zone item3 in cNMGADBPPJK)
		{
			item3.CGJCKGAFPED();
		}
	}

	public static void CGJCKGAFPED()
	{
		BKDAMDOMLGK();
		BOFGHBJABMK();
	}

	public void EJANJEEGOOE(object data = null)
	{
		if (data == null || (int)data == 0)
		{
			GJEJCLBAPMP = true;
		}
		else
		{
			OnAuthenticate(true);
		}
	}

	public void OnAuthenticate(bool BOFABDEJGFL = false)
	{
		if (GJEJCLBAPMP || BOFABDEJGFL)
		{
			CCDKHLAMKKO().KGFJPLKOABI();
			CCDKHLAMKKO().PMIIHIFGIIN();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(SF2Paths.APHDBIBDMDG());
			stringBuilder.Append("/");
			stringBuilder.Append(Constants.OJMIJINKBPJ);
			XmlUtils.ONLDJNLKKAL(IEDEFCBFJAD, stringBuilder.ToString());
			stringBuilder.Replace(Constants.OJMIJINKBPJ, Constants.GHKPPHAAMBL);
			XmlUtils.ONLDJNLKKAL(IEDEFCBFJAD, stringBuilder.ToString());
			GJEJCLBAPMP = false;
		}
	}

	public static void CELGPFFHLIM()
	{
		if (File.Exists(PFMBKJMEDEF()))
		{
			File.Delete(PFMBKJMEDEF());
			UserDataValidator.KAFMCNCGOJH(PFMBKJMEDEF());
		}
		if (File.Exists(IDIFECNLMKO()))
		{
			File.Delete(IDIFECNLMKO());
			UserDataValidator.KAFMCNCGOJH(IDIFECNLMKO());
		}
		if (File.Exists(OPBLKCABALC()))
		{
			File.Delete(OPBLKCABALC());
			UserDataValidator.KAFMCNCGOJH(OPBLKCABALC());
		}
		PacksController.ELEBLBJKDBI().AKMIAJPGHDC();
		ApplicationController.Quit();
	}

	public static string FDJICKDCIBI()
	{
		return (!File.Exists(PFMBKJMEDEF())) ? string.Empty : File.ReadAllText(PFMBKJMEDEF());
	}

	public static void SetRosterFileContent(string GHDPPHAAPCA)
	{
		if (File.Exists(PFMBKJMEDEF()))
		{
			File.Delete(PFMBKJMEDEF());
			UserDataValidator.KAFMCNCGOJH(PFMBKJMEDEF());
		}
		if (File.Exists(IDIFECNLMKO()))
		{
			File.Delete(IDIFECNLMKO());
			UserDataValidator.KAFMCNCGOJH(IDIFECNLMKO());
		}
		if (File.Exists(OPBLKCABALC()))
		{
			File.Delete(OPBLKCABALC());
			UserDataValidator.KAFMCNCGOJH(OPBLKCABALC());
		}
		if (File.Exists(GPKBMLALFIM()))
		{
			File.Delete(GPKBMLALFIM());
			UserDataValidator.KAFMCNCGOJH(GPKBMLALFIM());
		}
		XmlDocument xmlDocument = XmlUtils.DGOAOLEEMDG(GHDPPHAAPCA);
		if (xmlDocument == null)
		{
			LLLOJBFMONN.Write("Trying to set incorrect user - " + PFMBKJMEDEF());
			ApplicationController.Quit();
		}
		else
		{
			XmlUtils.ONLDJNLKKAL(xmlDocument, PFMBKJMEDEF());
			LLLOJBFMONN.Write("User successfully replaced in - " + PFMBKJMEDEF());
			ApplicationController.Quit();
		}
	}

	public bool FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ LFLGCDNKNJI)
	{
		return _QuestsManager.ActionQuest(LFLGCDNKNJI);
	}

	public void MHHNIPBJNAD()
	{
		if (!HIAMHMEGBEI())
		{
			_QuestsManager.RunActionsAll();
		}
	}

	public void FGAEEJBEGEJ(List<QuestStage> NKNMCOEBMNG)
	{
		_QuestsManager.AddActionQuest(NKNMCOEBMNG);
	}

	public bool OMDLOOFIJDF()
	{
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
			return nKGLHEGIKKP.BKBHIMEEDBG().JBPHIAEPHAH();
		}
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
		int num = HFPJDOEEDCA();
		XmlNode xmlNode = xmlDocument["Users"];
		foreach (XmlNode item in xmlNode)
		{
			if (num == item.Attributes["ID"].ParseInt())
			{
				XmlNode xmlNode3 = item;
				string text = xmlNode3.Attributes["Tutorial"].CIPOICEEIBK(string.Empty);
				return text == "END";
			}
		}
		return false;
	}

	public void HandleAuthenticateResult(bool CELFBNLILMA)
	{
		Debug.Log("OnAuthenticate, isAuth = " + CELFBNLILMA);
		NHHADCOPOJE();
		if (CELFBNLILMA)
		{
			DKBINLMJIJG();
		}
		else
		{
			LHMCFCCBKPA();
		}
		LDNNNJGKDDC();
	}

	private void KOGNPNFOPIE()
	{
		NHHADCOPOJE();
		KFFLKFPPKHO = BMAPNFJFEHF();
		CoroutineManager.get_Current().StartRoutine(KFFLKFPPKHO);
	}

	private void NHHADCOPOJE()
	{
		if (KFFLKFPPKHO != null)
		{
			CoroutineManager.get_Current().StopRoutine(KFFLKFPPKHO);
		}
		KFFLKFPPKHO = null;
	}

	private IEnumerator BMAPNFJFEHF()
	{
		float num = 1f;
		Debug.LogFormat("Setup Authenticate Timeout: {0} sec", num);
		yield return new WaitForSeconds(num);
		Debug.Log("OnAuthenticateTimeout: time is up!");
		LDNNNJGKDDC();
	}

	private void LDNNNJGKDDC()
	{
		Debug.Log("OnAuthenticateFinished");
		GameCenterAbstract.OnAuthenticate = (Action<bool>)Delegate.Remove(GameCenterAbstract.OnAuthenticate, new Action<bool>(OnAuthenticate));
		CEPOJOPGFIG();
	}

	public void CEPOJOPGFIG()
	{
		LLLOJBFMONN.INNGABABJPC("Login sequence: ListSF::serverAPIAuthorize");
		if (!FMDDGHBNGHG)
		{
			FMDDGHBNGHG = true;
			KODPBMFMOLB = false;
			PMJLDBCABPO = true;
			PFPEPKJBJGC = true;
			JIJFBPGDBKF();
		}
	}

	public void DKBINLMJIJG()
	{
		GameUtils.DKBINLMJIJG();
	}

	public void LHMCFCCBKPA()
	{
		CCDKHLAMKKO().INNHOAPGCHI();
	}

	public bool HIAMHMEGBEI()
	{
		return false;
	}

	public QuestParameters BNMLDPNCMLB()
	{
		return _QuestsManager.QuestParameters;
	}

	public QuestStage PBGCEEBDBGG(string name)
	{
		return _QuestsManager.GetQuestByName(name);
	}

	public bool AddQuestToStek(string name, bool OBJGGIPDKDF)
	{
		return _QuestsManager.AddQuestToStek(name, OBJGGIPDKDF);
	}

	public bool AddQuestToStek(QuestStage DOKAIKMLLDK, bool OBJGGIPDKDF)
	{
		return _QuestsManager.AddQuestToStek(DOKAIKMLLDK, OBJGGIPDKDF);
	}

	public BattleType HIDKFHHJBDH(string name)
	{
		foreach (KeyValuePair<string, BattleType> item in GEDIPKEENJC)
		{
			if (item.Key == name)
			{
				return item.Value;
			}
		}
		return BattleType.FightNone;
	}

	public string ADHNLNFEOKN(BattleType LFLGCDNKNJI)
	{
		foreach (KeyValuePair<string, BattleType> item in GEDIPKEENJC)
		{
			if (item.Value == LFLGCDNKNJI)
			{
				return item.Key;
			}
		}
		return "DUMMY";
	}

	public void EMJLEBDAALP()
	{
	}

	public void DLAJNCEILEH(string value)
	{
		XmlNode xmlNode = IEDEFCBFJAD["Root"]["Versions"];
		if (xmlNode != null)
		{
			xmlNode["DataVersion"].Attributes["Value"].Value = value;
		}
		EJANJEEGOOE();
	}

	public void DLAJNCEILEH(VersionContainer version)
	{
		string bAINMLLIKOL = version.ToString(true);
		DLAJNCEILEH(bAINMLLIKOL);
	}

	public void ClearQuestsStack(List<string> NIKHAICFGNM = null)
	{
		_QuestsManager.ClearStack(NIKHAICFGNM);
	}

	public void EAFEBFMIDLF()
	{
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("DUMMY", BattleType.FightNone));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("TUTORIAL", BattleType.FightTutorial));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("CHALLENGE", BattleType.FightChallenge));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("ASCENSION", BattleType.FightAscension));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("BOSSES", BattleType.FightBosses));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("TOURNAMENT", BattleType.FightTournament));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("STORY", BattleType.FightStory));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("SURVIVAL", BattleType.FightSurvival));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("TACTICS", BattleType.FightFriendly));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("AUTO", BattleType.FightAuto));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("AI", BattleType.FightAi));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("HIDDEN", BattleType.FightUnregister));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("FAKE", BattleType.FightFake));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("PVP", BattleType.FightPVP));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("PERIODIC", BattleType.FightPeriodic));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("FINAL_BATTLE", BattleType.FightFinal));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("FINAL_BATTLE_REPLAYABLE", BattleType.FightFinalReplayable));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("BOSSES_INTERMISSION", BattleType.FightBossesIntermission));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("REPLAYABLE", BattleType.FightReplayable));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("BOSSES_REPLAYABLE", BattleType.FightBossesReplayable));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("FINAL_BATTLE_TITAN", BattleType.FightFinalTitan));
		GEDIPKEENJC.Add(new KeyValuePair<string, BattleType>("RAID", BattleType.FightRaid));
	}

	public void PDCHBPKOBFI(string EIDDAFDJJCJ = "")
	{
		string text = ((!(EIDDAFDJJCJ == string.Empty)) ? EIDDAFDJJCJ : "quests.xml");
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		if (nKGLHEGIKKP.JHHBKBENNNA(text))
		{
			return;
		}
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), text, XmlUtils.EBLFEPIOMOL.Normal, true, XmlCryptoUtils.NNLGALNDJCL());
		if (xmlDocument == null)
		{
			return;
		}
		XmlNode xmlNode = xmlDocument["Quests"];
		_QuestsManager.set_QuestsAllCapacity(xmlNode.ChildNodes.Count);
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			_QuestsManager.AddQuest(new QuestStage(childNode, text));
		}
		nKGLHEGIKKP.DDBBADOGHHB(text);
	}

	public void KBCBLOMDKCA(FightList KGKDKENMAOA)
	{
		if (Module.ELEBLBJKDBI().NMCNDOPKFJD() != ScreenType.ModuleMap)
		{
		}
	}

	public bool NKLCAPEMDIO(string EJENJNPEDOH)
	{
		if (EJENJNPEDOH == string.Empty)
		{
			return true;
		}
		string[] collection = EJENJNPEDOH.Split(',');
		List<string> list = new List<string>(collection);
		int kEKGEBCKLJI = NAMLKEMHPHJ.KEKGEBCKLJI;
		foreach (string item in list)
		{
			if (kEKGEBCKLJI == item.ToInt())
			{
				return true;
			}
		}
		return false;
	}

	public void PLNBHLPHDJG(int PPGFCLBFLEK)
	{
		foreach (FightList item in JNPMCNMEOLE)
		{
			item.UpdateLevel(PPGFCLBFLEK);
		}
	}

	public void MAOPKFNKHOI()
	{
		List<Achievement> list = GameUtils.HHLEKNNJGMJ.NMHMFCKBMKN();
		for (int i = 0; i < list.Count; i++)
		{
			Achievement jNPIOKEKMII = list[i];
			if (ANEHEDFAPCH.KJNPJKEHGLE().JABBCCJLOOC(jNPIOKEKMII.Name) == null)
			{
				ANEHEDFAPCH.KJNPJKEHGLE().POKNGJJAHAL(jNPIOKEKMII, false, false);
				ANEHEDFAPCH.KJNPJKEHGLE().CreateRepostAchievement(jNPIOKEKMII.Name);
			}
		}
	}

	public void LCFENEAGDDG(string NEPOLDCKNJL)
	{
		LLLOJBFMONN.Error("{0}", NEPOLDCKNJL);
		string title = "ERROR 576";
		string message = "Your game data may be corrupted. Please restart the application.\r\nContact support if this does not help: http://support.nekki.com\r\nDon`t uninstall the game to avoid savegame loss.";
		string cancel = "Reload";
		if (LocalizationManager.FJLMLAGEJDL)
		{
			title = LocalizationManager.GetString("HackTitle");
			message = LocalizationManager.GetString("HackMessage");
			cancel = LocalizationManager.GetString("HackButton");
		}
		DialogsOpener.OpenLocalAlertDialog(title, message, cancel, HAKPODKIJDJ);
	}

	private void HAKPODKIJDJ()
	{
		Application.Quit();
	}

	public void BBDOJLNOHLO(object data)
	{
		if (data == null)
		{
			LLLOJBFMONN.Error("ListSF::energyRefillTimer ERROR - data is NULL");
			return;
		}
		TextTimer.TimerDataStruct eHMOCHBPAGE = (TextTimer.TimerDataStruct)data;
		eHMOCHBPAGE.ABIELBGOLCA(CCDKHLAMKKO().NHFHDFIJEJG());
		if (0 >= eHMOCHBPAGE.CCCIFDLEMPI())
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
	}

	public void JNKBLMLEJGE(object data)
	{
		if (data == null)
		{
			LLLOJBFMONN.Error("ListSF::duelAccessibilityTimer ERROR - data is NULL");
			return;
		}
		TextTimer.TimerDataStruct eHMOCHBPAGE = (TextTimer.TimerDataStruct)data;
		if (BattlePeriodic.CCCIFDLEMPI() > 0)
		{
			eHMOCHBPAGE.ABIELBGOLCA(BattlePeriodic.IDGBNPFIDGC() - BattlePeriodic.CCCIFDLEMPI());
		}
		else
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
		if (0 >= eHMOCHBPAGE.CCCIFDLEMPI())
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
	}

	public void ENMEBKHLCHF(object data)
	{
		if (data == null)
		{
			LLLOJBFMONN.Error("ListSF::deliveryTimer ERROR - data is NULL");
			return;
		}
		TextTimer.TimerDataStruct eHMOCHBPAGE = (TextTimer.TimerDataStruct)data;
		if (eHMOCHBPAGE.Data != null)
		{
			UserItem dKCHDHMLKHN = (UserItem)eHMOCHBPAGE.Data;
			eHMOCHBPAGE.ABIELBGOLCA(GameUtils.GetLeftTime(dKCHDHMLKHN.IJGAOHJNLAH()));
			if (0 > eHMOCHBPAGE.CCCIFDLEMPI())
			{
				eHMOCHBPAGE.ABIELBGOLCA(0L);
			}
		}
		else
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
	}

	public void OKNJMHBIIGJ(object data)
	{
		if (data == null)
		{
			LLLOJBFMONN.Error("ListSF::startPackTimer ERROR - data is NULL");
			return;
		}
		TextTimer.TimerDataStruct eHMOCHBPAGE = (TextTimer.TimerDataStruct)data;
		eHMOCHBPAGE.ABIELBGOLCA(CCDKHLAMKKO().AACMNAJJKME() - IDMJOMOMDOJ());
		if (0 > eHMOCHBPAGE.CCCIFDLEMPI())
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
	}

	public void IAKAPNOBAMJ(object data)
	{
		if (data == null)
		{
			LLLOJBFMONN.Error("ListSF::customRosterTimer ERROR - data is NULL");
			return;
		}
		TextTimer.TimerDataStruct eHMOCHBPAGE = (TextTimer.TimerDataStruct)data;
		if (eHMOCHBPAGE.Data != null)
		{
			string gOHIIMFFFJI = (string)eHMOCHBPAGE.Data;
			RosterTimerContainer kCMICMHCEBB = CCDKHLAMKKO().AEMFLPNDDKL();
			RosterTimer fPNMILOHPMB = kCMICMHCEBB.PPCMACMLHCA(gOHIIMFFFJI);
			if (fPNMILOHPMB != null)
			{
				eHMOCHBPAGE.ABIELBGOLCA(GameUtils.GetLeftTime(fPNMILOHPMB.CMIABOOJOEN()));
				if (eHMOCHBPAGE.CCCIFDLEMPI() < 0)
				{
					eHMOCHBPAGE.ABIELBGOLCA(0L);
				}
			}
			else
			{
				eHMOCHBPAGE.ABIELBGOLCA(0L);
			}
		}
		else
		{
			eHMOCHBPAGE.ABIELBGOLCA(0L);
		}
	}

	public static List<PerkInfoItem> EIMKEJNJMEJ(ItemInfo PJDAGCBPLJE, bool EKBOGDKIHIH = true)
	{
		if (EKBOGDKIHIH)
		{
			UserItem dKCHDHMLKHN = CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(PJDAGCBPLJE.Name);
			if (dKCHDHMLKHN != null)
			{
				return dKCHDHMLKHN.IGACBNCNDBG();
			}
		}
		return PJDAGCBPLJE.LFIGBCDJHPG;
	}

	public void JLCGOODFKAK(ItemInfo item)
	{
	}

	public void OLHPLOMLKLE(ModelParameters JCICKLIMBEF)
	{
		FCBEJGEKOLA.Add(JCICKLIMBEF);
	}

	public void OOLIADKLGLJ(ItemInfo item)
	{
		string text = string.Empty;
		if (item.Type == "Armor")
		{
			text = GameUtils.GetDefaultItem("Armor");
		}
		else if (item.Type == "Helm")
		{
			text = GameUtils.GetDefaultItem("Helm");
		}
		else if (item.Type == "Weapon")
		{
			text = GameUtils.GetDefaultItem("Weapon");
		}
		else if (item.Type == "Ranged")
		{
			text = GameUtils.GetDefaultItem("Ranged");
		}
		else if (item.Type == "Magic")
		{
			text = GameUtils.GetDefaultItem("Magic");
		}
		else if (item.Type == "RaidConsumable" && item.MDPPNGIEJGD == "RaidCharge")
		{
			text = GameUtils.GetDefaultItem("RaidCharge");
		}
		else
		{
			JALMHIICOPB(item);
		}
		if (text != string.Empty)
		{
			UserItem dKCHDHMLKHN = CMGOCLGHNLH(text);
			if (dKCHDHMLKHN != null)
			{
				AFGHCIDFAHB(dKCHDHMLKHN, true);
			}
		}
	}

	public bool IMDGMNFHFCN(FightResult HEIADONEACH)
	{
		return IMDGMNFHFCN(HEIADONEACH.PMIHPJFAJIO);
	}

	public bool IMDGMNFHFCN(FightResult.ResultPrizeStruct PMIHPJFAJIO)
	{
		long gBGNFPNCGED = PMIHPJFAJIO.GBGNFPNCGED;
		long pNDAIFALIKF = PMIHPJFAJIO.PNDAIFALIKF;
		bool result = IncreaseExp(PMIHPJFAJIO.exp);
		if (gBGNFPNCGED > 0)
		{
			GCPJADIMNKI(gBGNFPNCGED);
		}
		if (pNDAIFALIKF > 0)
		{
			FPIJEOMBFJN(pNDAIFALIKF, Roster.HPOIJPGPOCF.CHANGE_FIGHT_REWARD);
		}
		List<CurrencyStruct> list = PMIHPJFAJIO.JGJLJMHKJBM();
		if (list.Count > 0)
		{
			foreach (CurrencyStruct item in list)
			{
				if (CCDKHLAMKKO().ENBKLLMAALP() || item.BKDEAGGPNAO.NBIHGGLGMCN == GameCurrency.DEFOMBPHMBP.CURRENCY_GROUP_NONE)
				{
					ANEHEDFAPCH.AddCurrencyCount(item.BKDEAGGPNAO, (ObscuredInt)(item.Count));
				}
			}
		}
		List<ResistanceStruct> list2 = PMIHPJFAJIO.IHLPFEPHBPG();
		if (list2.Count > 0)
		{
			foreach (ResistanceStruct item2 in list2)
			{
			}
		}
		List<FightResult.LJFFIBFBGID> hELFDCAIJNE = PMIHPJFAJIO.HELFDCAIJNE;
		if (hELFDCAIJNE.Count > 0)
		{
			foreach (FightResult.LJFFIBFBGID item3 in hELFDCAIJNE)
			{
				ItemInfo dLKPBAJDHBO = item3.DLKPBAJDHBO;
				RewardItem nAIEGGHELIH = item3.NAIEGGHELIH;
				if (dLKPBAJDHBO.ParentItem != null)
				{
					GEFDJDIINND(dLKPBAJDHBO.ParentItem, 1, 0L, false, false);
				}
				UserItem dKCHDHMLKHN = GEFDJDIINND(dLKPBAJDHBO, 1, 0L, false, false);
				int mHGODOLNDLE = dLKPBAJDHBO.MHGODOLNDLE;
				int mHNCENBCECJ = ANEHEDFAPCH.PINDEKDNCNL();
				dKCHDHMLKHN.GDBFNNLHPOB(nAIEGGHELIH.LDLPCOFHFKE, mHGODOLNDLE, mHNCENBCECJ);
			}
		}
		EJANJEEGOOE();
		return result;
	}

	public ModelParameters IAOBIMJFBMH(XmlNode node, ModelParameters NENHLHHFDCN, bool CHOLFBIPDIM = true)
	{
		ModelParameters kIKOGDEPGHB = ((NENHLHHFDCN == null) ? new ModelParameters() : NENHLHHFDCN.Clone());
		if (node.Attributes["Voice"] != null)
		{
			kIKOGDEPGHB.OLPCELPEDKD = node.Attributes["Voice"].CIPOICEEIBK(string.Empty);
		}
		if (node.Attributes["Number"] != null)
		{
			kIKOGDEPGHB.PEBKEBIBAFA = node.Attributes["Number"].ParseInt();
		}
		if (node.Attributes["NoDoubles"] != null)
		{
			kIKOGDEPGHB.PMHHMDAIOGL = node.Attributes["NoDoubles"].ParseInt() > 0;
		}
		kIKOGDEPGHB.EHFNCDPPIAF = node.Attributes["Random"].ParseInt();
		kIKOGDEPGHB.MEECPNMPFPG = node.Attributes["AutoTuneFactor"].ParseFloat();
		kIKOGDEPGHB.DLDMOHEGENM((ObscuredInt)(node.Attributes["Level"].ParseInt((ObscuredInt)(kIKOGDEPGHB.PINDEKDNCNL()))));
		kIKOGDEPGHB.AKLPHMOAIGK = node.Attributes["Dan"].ParseInt(kIKOGDEPGHB.AKLPHMOAIGK);
		kIKOGDEPGHB.KFMJMBANIGF = node.Attributes["Damage"].ParseFloat(kIKOGDEPGHB.KFMJMBANIGF);
		kIKOGDEPGHB.EHBHNGOGCKO = node.Attributes["Difficulty"].ParseFloat(kIKOGDEPGHB.EHBHNGOGCKO);
		kIKOGDEPGHB.KMNLACDHAFE = node.Attributes["BeginnerCheat"] != null && node.Attributes["BeginnerCheat"].ParseInt() > 0;
		kIKOGDEPGHB.FPIMGHKNHMO = node.Attributes["WarriorPower"].ParseInt();
		if (node.Attributes["ShieldTotal"] != null)
		{
			kIKOGDEPGHB.ShieldTotal = Mathf.Max(0, node.Attributes["ShieldTotal"].ParseInt());
			kIKOGDEPGHB.HasShieldTotalOverride = true;
		}
		kIKOGDEPGHB.ALCFNGIKCCB = node.Attributes["RatingCorrection"].ParseInt();
		kIKOGDEPGHB.HGHDBNPIFEJ = node.Attributes["Unknown"].ParseBool();
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item in iBLHIAHECLK)
		{
			bool flag = node.Attributes[item.get_Name()] == null;
			if (flag && CHOLFBIPDIM)
			{
				int OEMALIFPGPO = 0;
				kIKOGDEPGHB.MAGFMAFCHLP.Get(item.get_Name(), ref OEMALIFPGPO);
				OEMALIFPGPO = ((!GameUtils.HILEAHAAFIC(item.get_Name())) ? OEMALIFPGPO : (OEMALIFPGPO + kIKOGDEPGHB.FPIMGHKNHMO));
				kIKOGDEPGHB.MAGFMAFCHLP.Set(item.get_Name(), OEMALIFPGPO);
			}
			else if (!flag)
			{
				int num = node.Attributes[item.get_Name()].ParseInt();
				num = ((!GameUtils.HILEAHAAFIC(item.get_Name())) ? num : (num + kIKOGDEPGHB.FPIMGHKNHMO));
				kIKOGDEPGHB.MAGFMAFCHLP.Set(item.get_Name(), num);
			}
		}
		if (node.Attributes["FirstName"] != null)
		{
			kIKOGDEPGHB.BMFLPBLAFLK = node.Attributes["FirstName"].CIPOICEEIBK(string.Empty);
		}
		if (node.Attributes["LastName"] != null)
		{
			kIKOGDEPGHB.FMOKLKFCCKF = node.Attributes["LastName"].CIPOICEEIBK(string.Empty);
		}
		if (node.Attributes["Avatar"] != null)
		{
			kIKOGDEPGHB.HNKFHGOOKEG = node.Attributes["Avatar"].CIPOICEEIBK(string.Empty);
		}
		if (node.Attributes["PlayerRating"] != null)
		{
			kIKOGDEPGHB.FOPCCNDPFOE(node.Attributes["PlayerRating"].ParseFloat());
		}
		if (node.Attributes["EnemyRating"] != null)
		{
			kIKOGDEPGHB.KCLCLMNOIDJ(node.Attributes["EnemyRating"].ParseFloat());
		}
		if (node.Attributes["PlayerRatingMagic"] != null)
		{
			kIKOGDEPGHB.LEIPGKLCKOP(node.Attributes["PlayerRatingMagic"].ParseFloat());
		}
		if (node.Attributes["EnemyRatingMagic"] != null)
		{
			kIKOGDEPGHB.NHPLLBNKOAF(node.Attributes["EnemyRatingMagic"].ParseFloat());
		}
		if (node.Attributes["PlayerRatingRanged"] != null)
		{
			kIKOGDEPGHB.JPOCPFLJKDC(node.Attributes["PlayerRatingRanged"].ParseFloat());
		}
		if (node.Attributes["EnemyRatingRanged"] != null)
		{
			kIKOGDEPGHB.AJLEAGJFKPD(node.Attributes["EnemyRatingRanged"].ParseFloat());
		}
		XmlNode xmlNode = node["AttributesAlign"];
		if (xmlNode != null)
		{
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				AttributesAlign hLHDMKCPIJP = new AttributesAlign();
				hLHDMKCPIJP.Factor = childNode.Attributes["Factor"].ParseFloat();
				hLHDMKCPIJP.Shift = childNode.Attributes["Shift"].ParseFloat();
				hLHDMKCPIJP.Priority = childNode.Attributes["Priority"].ParseInt();
				if (childNode.Attributes["Eclipse"] == null)
				{
					hLHDMKCPIJP.KONCHIPGFGO = ModelParameters.IHFKGJLIPGH.DFBoth;
				}
				else if (childNode.Attributes["Eclipse"].ParseInt() == 0)
				{
					hLHDMKCPIJP.KONCHIPGFGO = ModelParameters.IHFKGJLIPGH.DFNormal;
				}
				else
				{
					hLHDMKCPIJP.KONCHIPGFGO = ModelParameters.IHFKGJLIPGH.DFHard;
				}
				kIKOGDEPGHB.FKJBBIMPCBB.Add(hLHDMKCPIJP);
			}
		}
		XmlNode xmlNode3 = node["Groups"];
		if (xmlNode3 != null)
		{
			foreach (XmlNode item2 in xmlNode3)
			{
				GroupModel fIHMEHKAOCP = new GroupModel();
				fIHMEHKAOCP.Name = item2.Attributes["Name"].CIPOICEEIBK(string.Empty);
				fIHMEHKAOCP.IsRandom = item2.Attributes["Random"].ParseBool();
				fIHMEHKAOCP.PMHHMDAIOGL = item2.Attributes["NoDoubles"].ParseBool();
				kIKOGDEPGHB.KFKKHACFDPH.Add(fIHMEHKAOCP);
			}
		}
		if (!CHOLFBIPDIM)
		{
			if (node.Attributes["Skeleton"] != null)
			{
				kIKOGDEPGHB.PILJCAOFAED = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Skeleton"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["Armor"] != null)
			{
				kIKOGDEPGHB.LKKFNMBCCDB = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Armor"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["Helm"] != null)
			{
				kIKOGDEPGHB.FKMOLBBLKDA = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Helm"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["Weapon"] != null)
			{
				kIKOGDEPGHB.JGMLKIPCFII = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Weapon"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["Ranged"] != null)
			{
				kIKOGDEPGHB.LGHMILECPLA = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Ranged"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["Magic"] != null)
			{
				kIKOGDEPGHB.ADBKGIBBNHJ = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Magic"].CIPOICEEIBK(string.Empty));
			}
			if (node.Attributes["RaidCharge"] != null)
			{
				KAOPLEPILDH kAOPLEPILDH = kIKOGDEPGHB as KAOPLEPILDH;
				if (kAOPLEPILDH != null)
				{
					kAOPLEPILDH.LMIBBJIKLNO = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["RaidCharge"].CIPOICEEIBK(string.Empty));
				}
			}
			if (node.Attributes["Seal"] != null)
			{
				kIKOGDEPGHB.KKJJONOBHKI = DJBOFEEKJMP().KCCDBEEKBCG(node.Attributes["Seal"].CIPOICEEIBK(string.Empty));
			}
		}
		else
		{
			XmlNode xmlNode5 = node["Items"];
			if (xmlNode5 != null)
			{
				kIKOGDEPGHB.HEKILHEHMMH.Clear();
				foreach (XmlNode item3 in xmlNode5)
				{
					ItemInfo dJKEECEOCJB = null;
					if (item3.Attributes["Name"] != null)
					{
						dJKEECEOCJB = DJBOFEEKJMP().KCCDBEEKBCG(item3.Attributes["Name"].CIPOICEEIBK(string.Empty));
					}
					else if (item3.Attributes["Type"] != null)
					{
						dJKEECEOCJB = kIKOGDEPGHB.KDABEFBJMOD(item3.Attributes["Type"].CIPOICEEIBK(string.Empty));
					}
					if (dJKEECEOCJB != null)
					{
						if (dJKEECEOCJB.Type.Equals("Decorate"))
						{
							kIKOGDEPGHB.HEKILHEHMMH.Add(dJKEECEOCJB);
							continue;
						}
						ItemInfo dJKEECEOCJB2 = dJKEECEOCJB.Clone();
						XmlNode hKPPBKPJOEO = item3["Enchantments"];
						dJKEECEOCJB2.APMJCGBNEDI.Clear();
						dJKEECEOCJB2.JCJKLMICDIC(hKPPBKPJOEO);
						kIKOGDEPGHB.OLLNIKFPMKE(dJKEECEOCJB2.Type, dJKEECEOCJB2);
						if (!JGFGMICMBKL)
						{
							kIKOGDEPGHB.PFNDNOMGFBC(dJKEECEOCJB2);
						}
					}
					else if (!string.IsNullOrEmpty(kIKOGDEPGHB.HHKODEICDNP))
					{
						string text = item3.Attributes["Name"].CIPOICEEIBK(string.Empty);
						string text2 = item3.Attributes["Type"].CIPOICEEIBK(string.Empty);
						LLLOJBFMONN.Error("No item Wariror {0} ---- {1}", text, text2);
					}
				}
			}
			XmlNode xmlNode7 = node["Perks"];
			if (xmlNode7 != null)
			{
				foreach (XmlNode item4 in xmlNode7)
				{
					string gOHIIMFFFJI = item4.Attributes["Name"].CIPOICEEIBK(string.Empty);
					PerkInfoItem aCONCDFDNJH = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(gOHIIMFFFJI);
					if (aCONCDFDNJH != null && !kIKOGDEPGHB.GIKPDPFOAIL.Contains(aCONCDFDNJH))
					{
						if (item4["Set"] != null || item4["RatingEvaluation"] != null)
						{
							aCONCDFDNJH = aCONCDFDNJH.Clone(item4["Set"], item4["RatingEvaluation"]);
							kIKOGDEPGHB.LHLEIAKJANI(aCONCDFDNJH);
						}
						kIKOGDEPGHB.GIKPDPFOAIL.AddIfNotExist(aCONCDFDNJH);
					}
				}
			}
		}
		Tactic hBFMBOHLKPJ = AiData.GetTacticByName(node.Attributes["Tactic"].CIPOICEEIBK(string.Empty));
		kIKOGDEPGHB.HBFMBOHLKPJ = hBFMBOHLKPJ;
		kIKOGDEPGHB.EEGMBGBLLIF = node.Attributes["NotAI"] == null;
		kIKOGDEPGHB.HKJFJHBHMND = node.Attributes["NotAnimation"] == null;
		kIKOGDEPGHB.ABAPAIEBNGK = node.Attributes["Controlled"] != null;
		kIKOGDEPGHB.IsPlayer = false;
		kIKOGDEPGHB.IsWinner = false;
		kIKOGDEPGHB.BHHLEBHLBLH = false;
		kIKOGDEPGHB.PCALDKCJGCK = false;
		kIKOGDEPGHB.FCOALLOHJNP = 0;
		kIKOGDEPGHB.CIDCNCDFONA = 0f;
		if (!kIKOGDEPGHB.KKFBCOKMNDF)
		{
			kIKOGDEPGHB.Node = node;
			kIKOGDEPGHB.KKFBCOKMNDF = true;
		}
		return kIKOGDEPGHB;
	}

	public static void KNCECPAINLI(BattleReplayable BBAGBFDMNJE)
	{
		DMNAHDJIBOP.AddIfNotExist(BBAGBFDMNJE);
	}

	public void FOKCPLOMLOK(FightList fight, XmlNode node, BattleType LFLGCDNKNJI, string LPJNEDFCBOI, string PINIIFIOECE, Battle DPOOIONCEOA)
	{
		fight.Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		fight.set_Type(LFLGCDNKNJI);
		fight.EJGGHHEOGPG = node.Attributes["Replays"].ParseInt();
		fight.RepeatTime = node.Attributes["ReplayInterval"].ParseInt();
		fight.set_PowerRequired(node.Attributes["Power"].ParseInt(1));
		fight.BDBBNECNMBP = node.Attributes["Rounds"].ParseInt(2);
		fight.RoundTime = (ObscuredInt)(node.Attributes["RoundTime"].ParseInt(60));
		fight.JKMJHIIMHPG = node.Attributes["Location"].CIPOICEEIBK(LPJNEDFCBOI);
		fight.NPPIFKKLNCN = node.Attributes["Music"].CIPOICEEIBK(PINIIFIOECE);
		fight.FANGNMDAINE = node.Attributes["EvaluatedRating"].ParseFloat(-1f);
		fight.OMFDJPFGKAB = node.Attributes["HealthRecovery"].ParseFloat(1f);
		fight.MOPEDKMDLFA = node.Attributes["PrizeBase"].ParseFloat(-1f);
		fight.set_Description(node.Attributes["Description"].CIPOICEEIBK(string.Empty));
		fight.CNNCIENODGE = node.Attributes["Locked"].ParseBool();
		fight.JKCHHOMGGBN = node.Attributes["RewardImage"].CIPOICEEIBK(string.Empty);
		fight.ANIFGJGHNLN = LFLGCDNKNJI != BattleType.FightNone;
		ushort aNHLAHFDDCE = 0;
		ushort lPMDOHPIEOP = 0;
		if (!node.Attributes["RewardDigits"].Empty())
		{
			aNHLAHFDDCE = (ushort)node.Attributes["RewardDigits"].ParseUint();
		}
		else if (DPOOIONCEOA != null)
		{
			aNHLAHFDDCE = DPOOIONCEOA.GIOJPNNLKKK();
		}
		if (!node.Attributes["PrizeBaseDigits"].Empty())
		{
			lPMDOHPIEOP = (ushort)node.Attributes["PrizeBaseDigits"].ParseUint();
		}
		else if (DPOOIONCEOA != null)
		{
			lPMDOHPIEOP = DPOOIONCEOA.MCEGLDIFDBI();
		}
		fight.ANHLAHFDDCE = aNHLAHFDDCE;
		fight.LPMDOHPIEOP = lPMDOHPIEOP;
		ModelParameters kIKOGDEPGHB = null;
		XmlNode xmlNode = node["Warriors"];
		if (xmlNode != null)
		{
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (!childNode.Attributes["Template"].Empty())
				{
					TemplateUser aHIIGFBIGAA = CNFBCBDPKCI(childNode.Attributes["Template"].CIPOICEEIBK());
					kIKOGDEPGHB = ((aHIIGFBIGAA == null) ? IAOBIMJFBMH(childNode, null) : IAOBIMJFBMH(childNode, aHIIGFBIGAA.KEJDJHAGBMK));
				}
				else
				{
					kIKOGDEPGHB = IAOBIMJFBMH(childNode, null);
				}
				kIKOGDEPGHB.HHKODEICDNP = childNode.Attributes["Group"].CIPOICEEIBK();
				kIKOGDEPGHB.EHFNCDPPIAF = childNode.Attributes["Random"].ParseInt();
				kIKOGDEPGHB.Node = childNode;
				fight.KMLFBLCMMDO(kIKOGDEPGHB);
			}
		}
		XmlNode hKPPBKPJOEO = node["Rules"];
		XmlNode hKPPBKPJOEO2 = node["Rewards"];
		EEPPJEMHBCK(fight, hKPPBKPJOEO);
		if (LFLGCDNKNJI != BattleType.FightRaid)
		{
			HCJDHMGAMIE(fight, hKPPBKPJOEO2);
		}
	}

	public void AJKBFMLOCOF(FightList KGKDKENMAOA)
	{
		JNPMCNMEOLE.AddIfNotExist(KGKDKENMAOA);
		if (IHNFKALCJGJ)
		{
			NEIBAJKEJDE(KGKDKENMAOA);
			DINPFDGMEAB(KGKDKENMAOA);
		}
	}

	public void KINHMMGJEMP(FightList KGKDKENMAOA)
	{
		JNPMCNMEOLE.Remove(KGKDKENMAOA);
	}

	public static void MBBMOKFGABP(ItemInfo item)
	{
		long num = item.OHBBLIMNIMJ();
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		long num2 = nKGLHEGIKKP.BFBOEGMAMNF() - (ObscuredLong)(nKGLHEGIKKP.KNHDCEBIMEE());
		long num3 = (item.NNLMNNAEDIE = Math.Max(0L, num - num2));
		nKGLHEGIKKP.HGDLPMDHHOJ((ObscuredLong)((ObscuredLong)(nKGLHEGIKKP.KNHDCEBIMEE()) - num3));
	}

	public static void BLNHEMCHIGF(ItemInfo item, bool CNIOCCCBDBJ)
	{
		long num = 0L;
		num = (CNIOCCCBDBJ ? (long)(ObscuredLong)(item.KLHOKKPALOK) : item.MCNMMBCJADI());
		long pEGDPDINDDO = Math.Max(0L, num - CCDKHLAMKKO().PEJFMMHOOGN());
		item.PEGDPDINDDO = pEGDPDINDDO;
	}

	public TemplateUser CNFBCBDPKCI(string name)
	{
		foreach (TemplateUser item in HBOHFLOJMAA)
		{
			if (item.name == name)
			{
				return item;
			}
		}
		return null;
	}

	public void PLEDFOHECDC(ModelParameters OEMALIFPGPO, ModelParameters BBNKIBKPBLO)
	{
		if (BBNKIBKPBLO.IBBALIJOJMC != SceneTypes.SceneNone)
		{
			OEMALIFPGPO.IBBALIJOJMC = BBNKIBKPBLO.IBBALIJOJMC;
		}
		if (BBNKIBKPBLO.MEECPNMPFPG != 0f)
		{
			OEMALIFPGPO.MEECPNMPFPG = BBNKIBKPBLO.MEECPNMPFPG;
		}
		if (BBNKIBKPBLO.OLPCELPEDKD.Length != 0)
		{
			OEMALIFPGPO.OLPCELPEDKD = BBNKIBKPBLO.OLPCELPEDKD;
		}
		if (BBNKIBKPBLO.PILJCAOFAED != null)
		{
			OEMALIFPGPO.PILJCAOFAED = BBNKIBKPBLO.PILJCAOFAED;
		}
		if (BBNKIBKPBLO.JGMLKIPCFII != null)
		{
			OEMALIFPGPO.JGMLKIPCFII = BBNKIBKPBLO.JGMLKIPCFII;
		}
		if (BBNKIBKPBLO.LKKFNMBCCDB != null)
		{
			OEMALIFPGPO.LKKFNMBCCDB = BBNKIBKPBLO.LKKFNMBCCDB;
		}
		if (BBNKIBKPBLO.FKMOLBBLKDA != null)
		{
			OEMALIFPGPO.FKMOLBBLKDA = BBNKIBKPBLO.FKMOLBBLKDA;
		}
		if (BBNKIBKPBLO.LGHMILECPLA != null)
		{
			OEMALIFPGPO.LGHMILECPLA = BBNKIBKPBLO.LGHMILECPLA;
		}
		if (BBNKIBKPBLO.ADBKGIBBNHJ != null)
		{
			OEMALIFPGPO.ADBKGIBBNHJ = BBNKIBKPBLO.ADBKGIBBNHJ;
		}
		if (BBNKIBKPBLO.KKJJONOBHKI != null)
		{
			OEMALIFPGPO.KKJJONOBHKI = BBNKIBKPBLO.KKJJONOBHKI;
		}
		KAOPLEPILDH kAOPLEPILDH = OEMALIFPGPO as KAOPLEPILDH;
		KAOPLEPILDH kAOPLEPILDH2 = BBNKIBKPBLO as KAOPLEPILDH;
		if (kAOPLEPILDH != null && kAOPLEPILDH2 != null && kAOPLEPILDH2.LMIBBJIKLNO != null)
		{
			kAOPLEPILDH.LMIBBJIKLNO = kAOPLEPILDH2.LMIBBJIKLNO;
		}
		if (BBNKIBKPBLO.EndRoundType != EndRoundType.EndRoundTypeNone)
		{
			OEMALIFPGPO.EndRoundType = BBNKIBKPBLO.EndRoundType;
		}
		if (BBNKIBKPBLO.FPIMGHKNHMO != 0)
		{
			OEMALIFPGPO.FPIMGHKNHMO = BBNKIBKPBLO.FPIMGHKNHMO;
		}
		if (BBNKIBKPBLO.HasShieldTotalOverride)
		{
			OEMALIFPGPO.ShieldTotal = BBNKIBKPBLO.ShieldTotal;
			OEMALIFPGPO.HasShieldTotalOverride = true;
		}
		if (BBNKIBKPBLO.ALCFNGIKCCB != 0)
		{
			OEMALIFPGPO.ALCFNGIKCCB = BBNKIBKPBLO.ALCFNGIKCCB;
		}
		if (BBNKIBKPBLO.KFMJMBANIGF != 0f)
		{
			OEMALIFPGPO.KFMJMBANIGF = BBNKIBKPBLO.KFMJMBANIGF;
		}
		if (BBNKIBKPBLO.EHBHNGOGCKO != 0f)
		{
			OEMALIFPGPO.EHBHNGOGCKO = BBNKIBKPBLO.EHBHNGOGCKO;
		}
		if (!string.IsNullOrEmpty(BBNKIBKPBLO.BMFLPBLAFLK))
		{
			OEMALIFPGPO.BMFLPBLAFLK = BBNKIBKPBLO.BMFLPBLAFLK;
		}
		if (!string.IsNullOrEmpty(BBNKIBKPBLO.FMOKLKFCCKF))
		{
			OEMALIFPGPO.FMOKLKFCCKF = BBNKIBKPBLO.FMOKLKFCCKF;
		}
		if (!string.IsNullOrEmpty(BBNKIBKPBLO.HNKFHGOOKEG))
		{
			OEMALIFPGPO.HNKFHGOOKEG = BBNKIBKPBLO.HNKFHGOOKEG;
		}
		if (BBNKIBKPBLO.NHBIJEEKALC.Count != 0)
		{
			foreach (PerkInfoItem item in BBNKIBKPBLO.NHBIJEEKALC)
			{
				OEMALIFPGPO.NHBIJEEKALC.AddIfNotExist(item);
			}
		}
		if (BBNKIBKPBLO.GIKPDPFOAIL.Count != 0)
		{
			foreach (PerkInfoItem item2 in BBNKIBKPBLO.GIKPDPFOAIL)
			{
				OEMALIFPGPO.GIKPDPFOAIL.AddIfNotExist(item2);
			}
		}
		if (BBNKIBKPBLO.JGCNPHDGHAK.Count != 0)
		{
			foreach (PerkInfoItem item3 in BBNKIBKPBLO.JGCNPHDGHAK)
			{
				OEMALIFPGPO.JGCNPHDGHAK.AddIfNotExist(item3);
			}
		}
		if (BBNKIBKPBLO.PMHIIOJPDLO() != -1f)
		{
			OEMALIFPGPO.FOPCCNDPFOE(BBNKIBKPBLO.PMHIIOJPDLO());
		}
		if (BBNKIBKPBLO.CEKIBEJELBM() != -1f)
		{
			OEMALIFPGPO.KCLCLMNOIDJ(BBNKIBKPBLO.CEKIBEJELBM());
		}
		if (BBNKIBKPBLO.FLLKBDGJIKO() != -1f)
		{
			OEMALIFPGPO.LEIPGKLCKOP(BBNKIBKPBLO.FLLKBDGJIKO());
		}
		if (BBNKIBKPBLO.GGIOPHOMCCL() != -1f)
		{
			OEMALIFPGPO.NHPLLBNKOAF(BBNKIBKPBLO.GGIOPHOMCCL());
		}
		if (BBNKIBKPBLO.GBKEKEDPBIB() != -1f)
		{
			OEMALIFPGPO.JPOCPFLJKDC(BBNKIBKPBLO.GBKEKEDPBIB());
		}
		if (BBNKIBKPBLO.GCEDCHHBJGM() != -1f)
		{
			OEMALIFPGPO.AJLEAGJFKPD(BBNKIBKPBLO.GCEDCHHBJGM());
		}
		if (BBNKIBKPBLO.HBFMBOHLKPJ != null)
		{
			OEMALIFPGPO.HBFMBOHLKPJ = BBNKIBKPBLO.HBFMBOHLKPJ;
		}
		if (BBNKIBKPBLO.FKJBBIMPCBB.Count != 0)
		{
			foreach (AttributesAlign item4 in BBNKIBKPBLO.FKJBBIMPCBB)
			{
				OEMALIFPGPO.FKJBBIMPCBB.Add(new AttributesAlign(item4));
			}
		}
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		foreach (WarriorAttribute item5 in iBLHIAHECLK)
		{
			string kGBGENDIMBC = item5.get_Name();
			int OEMALIFPGPO2 = 0;
			if (BBNKIBKPBLO.IBLHIAHECLK.Get(kGBGENDIMBC, ref OEMALIFPGPO2))
			{
				OEMALIFPGPO.IBLHIAHECLK.Set(kGBGENDIMBC, OEMALIFPGPO2);
			}
		}
	}

	private void ILFBDHDMHPD(object data)
	{
		long getTime = GlobalTimer.get_GetTime();
		HJOHKOEICAP = getTime;
		FCAACLKFFLH(HJOHKOEICAP);
		ALJEKDDKPJJ(HJOHKOEICAP);
		if (GameUtils.OBJEKOBDMOE)
		{
			bool flag = Module.ELEBLBJKDBI().NMCNDOPKFJD() != ScreenType.ModulePreloader;
			bool flag2 = Module.ELEBLBJKDBI().NMCNDOPKFJD() != ScreenType.ModuleFight;
			bool flag3 = Fight.OHNKFOHIAKG() != null && Fight.OHNKFOHIAKG().get_isFightNone();
			if (flag && (flag2 || flag3))
			{
				GHDNJMDEALP(HJOHKOEICAP);
			}
			RosterTimerContainer kCMICMHCEBB = ANEHEDFAPCH.AEMFLPNDDKL();
			kCMICMHCEBB.CheckTimers(HJOHKOEICAP);
		}
		OnAuthenticate();
	}

	private void NMMBHENGDJO()
	{
		_items.NMMBHENGDJO(SF2Paths.KKIDGPBOBNI());
	}

	private void PBNNPBEDOOJ()
	{
		IEDEFCBFJAD = XmlUtils.AIFIAKNJMHG(SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
		if (IEDEFCBFJAD == null)
		{
			ANEHEDFAPCH = new Roster(null, null);
			return;
		}
		int num = HFPJDOEEDCA();
		XmlNode xmlNode = IEDEFCBFJAD["Root"]["Warriors"];
		bool flag = false;
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string text = childNode.Attributes["IsFake"].CIPOICEEIBK(string.Empty);
			if (text == "True")
			{
				flag = true;
			}
			else if (num == childNode.Attributes["ID"].ParseInt())
			{
				_CurrentUserNode = childNode;
				ANEHEDFAPCH = NHAMDLEDOHM(_CurrentUserNode);
				ANEHEDFAPCH.KHCNHPCPFII().HOMCPNCGPDB(DJBOFEEKJMP().HCDLKHKBEPF());
				break;
			}
		}
		ANEHEDFAPCH.get_Parameters().NOBKKLBJFIL();
		XmlNode hKPPBKPJOEO = IEDEFCBFJAD["Root"]["Billing"];
		JMDJEEFELCD(hKPPBKPJOEO);
	}

	private int HFPJDOEEDCA()
	{
		return IEDEFCBFJAD["Root"]["CurrentUser"].Attributes["ID"].ParseInt();
	}

	private void JMDJEEFELCD(XmlNode node)
	{
		if (node == null)
		{
			return;
		}
		List<ItemInfo> list = DJBOFEEKJMP().ONFMAJEAACM("RealMoneyItem");
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string name = childNode.Attributes["Name"].CIPOICEEIBK();
			if (!string.IsNullOrEmpty(name))
			{
				ItemInfo dJKEECEOCJB = list.Find((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(name));
				if (dJKEECEOCJB != null)
				{
					dJKEECEOCJB.FPEIFLEBEAA = childNode.Attributes["RealPrice"].CIPOICEEIBK(dJKEECEOCJB.FPEIFLEBEAA);
					dJKEECEOCJB.EGAJMELKANL = childNode.Attributes["RealPriceConst"].CIPOICEEIBK(dJKEECEOCJB.EGAJMELKANL);
					dJKEECEOCJB.MIIJIMJDHFP = childNode.Attributes["RealPriceCurrency"].CIPOICEEIBK(dJKEECEOCJB.MIIJIMJDHFP);
				}
			}
		}
	}

	private void OFIPOGGCKIN()
	{
		if (IEGJHNHFJFA == null)
		{
			IEGJHNHFJFA = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "stages.xml", XmlUtils.EBLFEPIOMOL.Normal, true, XmlCryptoUtils.NNLGALNDJCL());
		}
		ONBBADOAPIB = new XmlDocument();
		ONBBADOAPIB.LCOLFMJJDJE(IEGJHNHFJFA["Stages"]["Warriors"]);
		XmlNode xmlNode = ONBBADOAPIB["Warriors"]["Templates"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			KJMKFNHFCGM(childNode);
		}
		EANLGFEDADB();
		xmlNode = ONBBADOAPIB["Warriors"]["WarriorGroups"];
		LNCPHCBFNJO(xmlNode);
	}

	private TemplateUser KJMKFNHFCGM(XmlNode node)
	{
		TemplateUser aHIIGFBIGAA = new TemplateUser();
		aHIIGFBIGAA.node = node;
		aHIIGFBIGAA.IJBOAGICOON = node.Attributes["Template"].CIPOICEEIBK(string.Empty);
		aHIIGFBIGAA.name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		aHIIGFBIGAA.KEJDJHAGBMK = IAOBIMJFBMH(node, null);
		foreach (TemplateUser item in HBOHFLOJMAA)
		{
			if (item.name == aHIIGFBIGAA.name)
			{
				HBOHFLOJMAA.Remove(item);
				break;
			}
		}
		HBOHFLOJMAA.Add(aHIIGFBIGAA);
		return aHIIGFBIGAA;
	}

	private GroupsUser HJDCNPMEHGJ(string name)
	{
		foreach (GroupsUser item in FAPGPFLOFEE)
		{
			if (item.name == name)
			{
				return item;
			}
		}
		return null;
	}

	private void EANLGFEDADB()
	{
		EANLGFEDADB(HBOHFLOJMAA);
	}

	private void EANLGFEDADB(List<TemplateUser> PHEDCOGJGLE)
	{
		foreach (TemplateUser item in HBOHFLOJMAA)
		{
			if (item.IJBOAGICOON == null || item.IJBOAGICOON.Equals(string.Empty))
			{
				continue;
			}
			TemplateUser aHIIGFBIGAA = CNFBCBDPKCI(item.IJBOAGICOON);
			XmlNode xmlNode = ONBBADOAPIB["Warriors"]["Templates_tmp"];
			if (xmlNode == null)
			{
				xmlNode = ONBBADOAPIB["Warriors"].ACBPMPMPKJJ("Templates_tmp");
			}
			bool flag = false;
			XmlNode xmlNode2 = null;
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Attributes["Name"].CIPOICEEIBK(string.Empty) == item.name)
				{
					flag = true;
					xmlNode2 = childNode;
					break;
				}
			}
			if (!flag)
			{
				xmlNode2 = xmlNode.LCOLFMJJDJE(aHIIGFBIGAA.KEJDJHAGBMK.Node);
				xmlNode2 = MergeUserXML(xmlNode2, item.node);
			}
			item.KEJDJHAGBMK = IAOBIMJFBMH(xmlNode2, aHIIGFBIGAA.KEJDJHAGBMK);
			item.KEJDJHAGBMK.Node = xmlNode2;
		}
	}

	private void LNCPHCBFNJO(XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			GroupsUser oFIHDFNGDLA = new GroupsUser();
			oFIHDFNGDLA.node = childNode;
			oFIHDFNGDLA.name = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			oFIHDFNGDLA.IJBOAGICOON = childNode.Attributes["Template"].CIPOICEEIBK(string.Empty);
			PNMNKCNOHIF(childNode, oFIHDFNGDLA.FJMBBMJMOKC);
			FAPGPFLOFEE.Add(oFIHDFNGDLA);
		}
	}

	private void PNMNKCNOHIF(XmlNode node, List<ModelParameters> target)
	{
		int num = 0;
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string text = childNode.Attributes["Template"].CIPOICEEIBK(string.Empty);
			TemplateUser aHIIGFBIGAA = ((!(text == string.Empty)) ? CNFBCBDPKCI(text) : null);
			ModelParameters kIKOGDEPGHB = ((aHIIGFBIGAA == null) ? IAOBIMJFBMH(childNode, null) : CNMFNFDIOOK(aHIIGFBIGAA.KEJDJHAGBMK, childNode));
			if (kIKOGDEPGHB != null)
			{
				kIKOGDEPGHB.HHKODEICDNP = childNode.Attributes["Group"].CIPOICEEIBK(string.Empty);
				kIKOGDEPGHB.EHFNCDPPIAF = childNode.Attributes["Random"].ParseInt();
				kIKOGDEPGHB.FLGGADFNNDK = num;
				target.Add(kIKOGDEPGHB);
			}
			num++;
		}
	}

	private ModelParameters CNMFNFDIOOK(ModelParameters KPAICOOKACB, XmlNode node)
	{
		if (KPAICOOKACB != null)
		{
			ModelParameters kIKOGDEPGHB = IAOBIMJFBMH(node, KPAICOOKACB);
			kIKOGDEPGHB.Node = MergeUserXML(kIKOGDEPGHB.Node, node);
			return kIKOGDEPGHB;
		}
		return null;
	}

	private Roster NHAMDLEDOHM(XmlNode node)
	{
		Roster nKGLHEGIKKP = new Roster(node, IAOBIMJFBMH(node, null, false));
		nKGLHEGIKKP.AddEventListener(0, EJANJEEGOOE);
		return nKGLHEGIKKP;
	}

	public List<Battle> MMCHMBIKIEP()
	{
		return _battles;
	}

	private void KIEEPEOPJGB()
	{
		IEGJHNHFJFA = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "stages.xml", XmlUtils.EBLFEPIOMOL.Normal, true, XmlCryptoUtils.NNLGALNDJCL());
		XmlNode xmlNode = IEGJHNHFJFA["Stages"]["Zones"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			bool cBFACOIOIAK = false;
			CMEABHLEKNH.Add(PGGMIJMOJHA(childNode, cBFACOIOIAK));
		}
		foreach (FightList item in JNPMCNMEOLE)
		{
			Battle cNAOMDMIGLJ = item.CNAOMDMIGLJ;
			Zone pKCPOJKLMOK = cNAOMDMIGLJ.LKDFFCADHNO();
			item.BCKFACGMOKC.SetFightIDSByZBF(string.Copy(pKCPOJKLMOK.get_Name()), string.Copy(cNAOMDMIGLJ.get_Name()), string.Copy(item.Name));
			RosterFight pIGKOIFBOME = ANEHEDFAPCH.DBMHOBPNIIA(item.BCKFACGMOKC);
			if (pIGKOIFBOME != null)
			{
				item.HOCFLEMFFKC(pIGKOIFBOME);
				item.ResetRandomRules();
			}
		}
		IHNFKALCJGJ = true;
	}

	private Zone PGGMIJMOJHA(XmlNode node, bool CBFACOIOIAK = false)
	{
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		string pMFEIPCHENB = node.Attributes["FileName"].CIPOICEEIBK(string.Empty);
		bool pENNHKHFEOM = 0 < node.Attributes["Start"].ParseInt();
		ConditionStatus fFFCCEEDMKI = ConditionStatus.StatusOpen;
		uint cDCJKJNGPOE = node.Attributes["RewardDigits"].ParseUint();
		uint mCDAHGPLLDO = node.Attributes["PrizeBaseDigits"].ParseUint();
		Zone pKCPOJKLMOK = new Zone(gOHIIMFFFJI, pMFEIPCHENB, pENNHKHFEOM, fFFCCEEDMKI, 0, cDCJKJNGPOE, mCDAHGPLLDO);
		Roster nKGLHEGIKKP = CCDKHLAMKKO();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			Battle cGJCGEBPCAF = MMDBBEFAHJH(childNode, pKCPOJKLMOK, CBFACOIOIAK);
			ELCJGIEJHNE(cGJCGEBPCAF);
			cGJCGEBPCAF.BDAELBFECAJ();
			FightIDS dIAIIPCBMFL = new FightIDS(string.Copy(pKCPOJKLMOK.get_Name()), string.Copy(cGJCGEBPCAF.get_Name()), string.Empty);
			bool dCHJDPCEODD = nKGLHEGIKKP.HAMPNCKAJKD(dIAIIPCBMFL);
			if (cGJCGEBPCAF.get_Type() == BattleType.FightRaid)
			{
				dCHJDPCEODD = true;
			}
			cGJCGEBPCAF.DCHJDPCEODD = dCHJDPCEODD;
			pKCPOJKLMOK.LGIIBNJFADA.Add(cGJCGEBPCAF);
		}
		return pKCPOJKLMOK;
	}

	private Battle MMDBBEFAHJH(XmlNode node, Zone HLJKOKMKMLM = null, bool CBFACOIOIAK = false)
	{
		Vector2 mGMMDGFPBLP = new Vector2(node.Attributes["X"].ParseInt(), node.Attributes["Y"].ParseInt());
		string gOHIIMFFFJI = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		string lOKLDPLAPOL = node.Attributes["Alias"].CIPOICEEIBK(string.Empty);
		string pEMOECLNECD = node.Attributes["Title"].CIPOICEEIBK(string.Empty);
		string aDONPNOBBDE = ((!node.Attributes["Icon"].Empty()) ? node.Attributes["Icon"].CIPOICEEIBK(string.Empty) : "training");
		string lHCFHAIDNDP = node.Attributes["Preview"].CIPOICEEIBK(string.Empty);
		string eMDJGBHIAIA = node.Attributes["Description"].CIPOICEEIBK(string.Empty);
		string lPJNEDFCBOI = node.Attributes["Location"].CIPOICEEIBK(GameUtils.NIPABEEAMHJ);
		string pINIIFIOECE = node.Attributes["Music"].CIPOICEEIBK();
		string text = node.Attributes["Type"].CIPOICEEIBK();
		string oAPKHNPPGHP = node.Attributes["RewardImage"].CIPOICEEIBK();
		string iHBMPGKIBAN = node.Attributes["ShowResistance"].CIPOICEEIBK();
		ushort cDCJKJNGPOE = 0;
		ushort mCDAHGPLLDO = 0;
		if (!node.Attributes["RewardDigits"].Empty())
		{
			cDCJKJNGPOE = (ushort)node.Attributes["RewardDigits"].ParseUint();
		}
		else if (HLJKOKMKMLM != null)
		{
			cDCJKJNGPOE = (ushort)HLJKOKMKMLM.GIOJPNNLKKK();
		}
		if (!node.Attributes["PrizeBaseDigits"].Empty())
		{
			mCDAHGPLLDO = (ushort)node.Attributes["PrizeBaseDigits"].ParseUint();
		}
		else if (HLJKOKMKMLM != null)
		{
			mCDAHGPLLDO = (ushort)HLJKOKMKMLM.MCEGLDIFDBI();
		}
		Battle cGJCGEBPCAF;
		switch (HIDKFHHJBDH(text))
		{
		case BattleType.FightPeriodic:
			cGJCGEBPCAF = new BattlePeriodic(text, mGMMDGFPBLP, gOHIIMFFFJI, aDONPNOBBDE, lHCFHAIDNDP, eMDJGBHIAIA, cDCJKJNGPOE, mCDAHGPLLDO, lOKLDPLAPOL, pEMOECLNECD, lPJNEDFCBOI, pINIIFIOECE, oAPKHNPPGHP, iHBMPGKIBAN);
			break;
		case BattleType.FightReplayable:
		case BattleType.FightBossesReplayable:
		case BattleType.FightFinalReplayable:
		{
			BattleReplayable bKKPCBGAEHC = new BattleReplayable(text, mGMMDGFPBLP, gOHIIMFFFJI, aDONPNOBBDE, lHCFHAIDNDP, eMDJGBHIAIA, cDCJKJNGPOE, mCDAHGPLLDO, lOKLDPLAPOL, pEMOECLNECD, lPJNEDFCBOI, pINIIFIOECE, oAPKHNPPGHP, iHBMPGKIBAN);
			bKKPCBGAEHC.Parse(node);
			cGJCGEBPCAF = bKKPCBGAEHC;
			break;
		}
		case BattleType.FightAscension:
		{
			BattleAscension bGFLODNGLPK = new BattleAscension(text, mGMMDGFPBLP, gOHIIMFFFJI, aDONPNOBBDE, lHCFHAIDNDP, eMDJGBHIAIA, cDCJKJNGPOE, mCDAHGPLLDO, lOKLDPLAPOL, pEMOECLNECD, lPJNEDFCBOI, pINIIFIOECE, oAPKHNPPGHP, iHBMPGKIBAN);
			bGFLODNGLPK.Parse(node);
			cGJCGEBPCAF = bGFLODNGLPK;
			break;
		}
		case BattleType.FightRaid:
		{
			BattleRaid pAHLFJIMKCL = new BattleRaid(text, mGMMDGFPBLP, gOHIIMFFFJI, aDONPNOBBDE, lHCFHAIDNDP, eMDJGBHIAIA, cDCJKJNGPOE, mCDAHGPLLDO, lOKLDPLAPOL, pEMOECLNECD, lPJNEDFCBOI, pINIIFIOECE, oAPKHNPPGHP, iHBMPGKIBAN);
			pAHLFJIMKCL.Parse(node);
			cGJCGEBPCAF = pAHLFJIMKCL;
			break;
		}
		default:
			cGJCGEBPCAF = new Battle(text, mGMMDGFPBLP, gOHIIMFFFJI, aDONPNOBBDE, lHCFHAIDNDP, eMDJGBHIAIA, cDCJKJNGPOE, mCDAHGPLLDO, lOKLDPLAPOL, pEMOECLNECD, lPJNEDFCBOI, pINIIFIOECE, oAPKHNPPGHP, iHBMPGKIBAN);
			break;
		}
		cGJCGEBPCAF.EENNGGIMMMI(HLJKOKMKMLM);
		cGJCGEBPCAF.JNIIGKNBCCL(node);
		_battles.Add(cGJCGEBPCAF);
		if (CBFACOIOIAK)
		{
			EGMDBNNBJFM(cGJCGEBPCAF);
		}
		return cGJCGEBPCAF;
	}

	private void EGMDBNNBJFM(Battle DPOOIONCEOA)
	{
		XmlNode xmlNode = DPOOIONCEOA.MMLPEMNIFBD().IOJIGDNFCFL();
		string lPJNEDFCBOI = DPOOIONCEOA.CBABFGDMLIH();
		string pINIIFIOECE = DPOOIONCEOA.MOADJJNKFKB();
		int num = 0;
		XmlNodeList xmlNodeList = xmlNode.SelectNodes("Fight");
		foreach (XmlNode item in xmlNodeList)
		{
			FightList jDIPBIHBGPF = new FightList();
			FOKCPLOMLOK(jDIPBIHBGPF, item, DPOOIONCEOA.get_Type(), lPJNEDFCBOI, pINIIFIOECE, DPOOIONCEOA);
			DPOOIONCEOA.AJKBFMLOCOF(jDIPBIHBGPF, num);
			num++;
		}
		DPOOIONCEOA.LEGLFDDINKO = 1;
	}

	private void NEIBAJKEJDE(FightList KGKDKENMAOA)
	{
		Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
		Zone pKCPOJKLMOK = cNAOMDMIGLJ.LKDFFCADHNO();
		KGKDKENMAOA.BCKFACGMOKC.SetFightIDSByZBF(string.Copy(pKCPOJKLMOK.get_Name()), string.Copy(cNAOMDMIGLJ.get_Name()), string.Copy(KGKDKENMAOA.Name));
		RosterFight pIGKOIFBOME = ANEHEDFAPCH.DBMHOBPNIIA(KGKDKENMAOA.BCKFACGMOKC);
		if (pIGKOIFBOME != null)
		{
			KGKDKENMAOA.HOCFLEMFFKC(pIGKOIFBOME);
			KGKDKENMAOA.ResetRandomRules();
		}
	}

	private void EEPPJEMHBCK(FightList fight, XmlNode node)
	{
		if (node == null)
		{
			return;
		}
		List<Rule> list = new List<Rule>();
		RuleParser.EEPPJEMHBCK(node, list);
		foreach (Rule item in list)
		{
			if (item != null)
			{
				fight.PutRule(item);
			}
		}
	}

	private void HCJDHMGAMIE(FightList fight, XmlNode node)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			RewardStruct lGDIIADDFLH = new RewardStruct(childNode, fight.ANHLAHFDDCE, fight.LPMDOHPIEOP);
			fight.OJJLHLPLFKC(lGDIIADDFLH);
		}
		fight.UpdateLevel(CCDKHLAMKKO().PINDEKDNCNL());
	}

	private void ELCJGIEJHNE(Battle DPOOIONCEOA)
	{
		if (DPOOIONCEOA == null)
		{
			LLLOJBFMONN.Error("ListSF::setRosterBattle - battle is NULL");
			return;
		}
		List<RosterBattle> list = ANEHEDFAPCH.IEANNFIECJA();
		foreach (RosterBattle item in list)
		{
			FightIDS mOCEDDJOAEB = new FightIDS(item.KHGCEFNBDDG());
			Zone pKCPOJKLMOK = DPOOIONCEOA.LKDFFCADHNO();
			if (mOCEDDJOAEB.PELHCAEAOFE() == pKCPOJKLMOK.get_Name() && mOCEDDJOAEB.CPHDPCAECJN() == DPOOIONCEOA.get_Name())
			{
				DPOOIONCEOA.FOMHAGJJCLJ(item);
				item.EDHMHFONDAI = DPOOIONCEOA;
				break;
			}
		}
	}

	private void FCAACLKFFLH(long DGGEIIBGENC)
	{
		int i = 0;
		for (int count = CMEABHLEKNH.Count; i < count; i++)
		{
			CMEABHLEKNH[i].SetTime(DGGEIIBGENC);
		}
		int j = 0;
		for (int count2 = KHLBNALFOGN.Count; j < count2; j++)
		{
			KHLBNALFOGN[j].SetTime(DGGEIIBGENC);
		}
		int k = 0;
		for (int count3 = CNMGADBPPJK.Count; k < count3; k++)
		{
			CNMGADBPPJK[k].SetTime(DGGEIIBGENC);
		}
	}

	private void ALJEKDDKPJJ(long time)
	{
		if (ANEHEDFAPCH != null)
		{
			ANEHEDFAPCH.ALJEKDDKPJJ(time);
		}
		MenuController.CFGIJDFFLLA();
	}

	private void JIJFBPGDBKF(object data = null)
	{
		long num = BLBNJKJKMBM();
		long aIJKJHMICNH = ELEBLBJKDBI().AIJKJHMICNH;
		NetworkController.ELEBLBJKDBI().IFFDOFMDABC();
	}

	private void JLEMHLLLCLD()
	{
		if (!GameSettings.HCAJHNKLLGB())
		{
			return;
		}
		float num = ANEHEDFAPCH.BFBOEGMAMNF();
		float num2 = ANEHEDFAPCH.EHFJHFDACMP();
		int num3 = HFPJDOEEDCA();
		XmlNode xmlNode = IEDEFCBFJAD["Users"];
		Roster nKGLHEGIKKP = null;
		if (xmlNode != null)
		{
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (num3 == childNode.Attributes["ID"].ParseInt())
				{
					nKGLHEGIKKP = NHAMDLEDOHM(childNode);
					break;
				}
			}
		}
		if (nKGLHEGIKKP != null && (float)nKGLHEGIKKP.BFBOEGMAMNF() == num && (float)nKGLHEGIKKP.EHFJHFDACMP() == num2)
		{
		}
	}

	private void GHDNJMDEALP(long time)
	{
		List<RecipeItemInfo> list = ANEHEDFAPCH.KHCNHPCPFII().PHKEAPFEOLP();
		foreach (RecipeItemInfo item in list)
		{
		}
	}

	private void LDADJAGGGPA()
	{
		BattlePeriodic.EEDCDDDNLIH(1, CCDKHLAMKKO().CPGGBLDAHBG());
	}

	private XmlNode MergeUserXML(XmlNode FAIPFEKENIM, XmlNode node)
	{
		foreach (XmlAttribute attribute in node.Attributes)
		{
			string name = attribute.Name;
			string value = attribute.Value;
			if (FAIPFEKENIM.Attributes[name] == null)
			{
				FAIPFEKENIM.LLIKNHNLGJJ(name).Value = value;
				continue;
			}
			string value2 = FAIPFEKENIM.Attributes[name].Value;
			if (value2 != value)
			{
				FAIPFEKENIM.Attributes[name].Value = value;
			}
		}
		if (node["Items"] != null)
		{
			if (FAIPFEKENIM["Items"] != null)
			{
				FAIPFEKENIM.RemoveChild(FAIPFEKENIM["Items"]);
			}
			FAIPFEKENIM.LCOLFMJJDJE(node["Items"]);
		}
		if (node["Perks"] != null)
		{
			if (FAIPFEKENIM["Perks"] != null)
			{
				FAIPFEKENIM.RemoveChild(FAIPFEKENIM["Perks"]);
			}
			FAIPFEKENIM.LCOLFMJJDJE(node["Perks"]);
		}
		if (node["AttributesAlign"] != null)
		{
			if (FAIPFEKENIM["AttributesAlign"] != null)
			{
				FAIPFEKENIM.RemoveChild(FAIPFEKENIM["AttributesAlign"]);
			}
			FAIPFEKENIM.LCOLFMJJDJE(node["AttributesAlign"]);
		}
		return FAIPFEKENIM;
	}

	private static bool BDNBHBOJLDN(ItemInfo item, long BCBAOELLEPB)
	{
		return true;
	}

	private static bool EMEMDEAEMCB(ItemInfo item)
	{
		return true;
	}

	private static bool HJHCCBGILAJ(ItemInfo item)
	{
		return true;
	}

	private static void ABKBFADGNBM(ItemAction LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case ItemAction.Item_Buy_Gold:
		case ItemAction.Item_Buy_Ruby:
		case ItemAction.Item_Buy_Real:
		case ItemAction.Item_Consumable:
			Sound.IFKCCDAIADF("snd_buy");
			break;
		case ItemAction.Item_Upgrade_Gold:
		case ItemAction.Item_Upgrade_Ruby:
		case ItemAction.Item_Delivery_Ruby:
			Sound.IFKCCDAIADF("snd_upgrade");
			break;
		}
	}

	public static List<PerkInfoItem> KJBMBFHCEIM(ItemInfo PJDAGCBPLJE, bool EKBOGDKIHIH)
	{
		if (EKBOGDKIHIH)
		{
			UserItem dKCHDHMLKHN = CMGOCLGHNLH(PJDAGCBPLJE.Name);
			if (dKCHDHMLKHN != null)
			{
				return dKCHDHMLKHN.IGACBNCNDBG();
			}
		}
		return PJDAGCBPLJE.LFIGBCDJHPG;
	}

	public void RandomizeObscuredVars()
	{
		JNPMCNMEOLE.ForEach((FightList DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}
}
