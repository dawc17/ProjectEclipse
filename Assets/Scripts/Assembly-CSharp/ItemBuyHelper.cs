using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Utils;

public static class ItemBuyHelper
{
	private static bool KCBCGDFKNME(ItemInfo item)
	{
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN == null)
		{
			XmlNode fMBDAPOMFGN = ListSF.CCDKHLAMKKO().BABKABBEFEL();
			UserItem dKCHDHMLKHN2 = new UserItem(fMBDAPOMFGN, item.Name, false, 1, -1, -1L);
			dKCHDHMLKHN2.KIGHKCOCJFJ(item);
			dKCHDHMLKHN2.IJCEKDCPBAG(false);
			dKCHDHMLKHN2.PJEEGECBHMH();
			ListSF.CCDKHLAMKKO().KHCNHPCPFII().GEFDJDIINND(dKCHDHMLKHN2);
			dKCHDHMLKHN2.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
			Sound.IFKCCDAIADF("snd_buy");
			return true;
		}
		dKCHDHMLKHN.CHILOKHFALD(dKCHDHMLKHN.OFOPFCJNEBL() + 1);
		return true;
	}

	private static bool OBHEMCJGMHE(ItemInfo item)
	{
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN == null)
		{
			long aFHNFJLOGIC = GlobalTimer.get_LocalTimeUTC() + item.EHKNIKHPGDN;
			XmlNode fMBDAPOMFGN = ListSF.CCDKHLAMKKO().BABKABBEFEL();
			UserItem dKCHDHMLKHN2 = new UserItem(fMBDAPOMFGN, item.Name, false, 0, -1, aFHNFJLOGIC);
			dKCHDHMLKHN2.KIGHKCOCJFJ(item);
			dKCHDHMLKHN2.IJCEKDCPBAG(false);
			dKCHDHMLKHN2.PJEEGECBHMH();
			ListSF.CCDKHLAMKKO().KHCNHPCPFII().GEFDJDIINND(dKCHDHMLKHN2);
			dKCHDHMLKHN2.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
			Sound.IFKCCDAIADF("snd_upgrade");
			return true;
		}
		return false;
	}

	private static bool LBCJLCDMJLI(ItemInfo item, UserItem NDMCFNGEPOA)
	{
		if (NDMCFNGEPOA != null)
		{
			NDMCFNGEPOA.IJCEKDCPBAG(true);
			NDMCFNGEPOA.FMMDLMGHPIB(item.OBJDGBBFJOO);
			NDMCFNGEPOA.CDFODJBJIPI(ListSF.CCDKHLAMKKO().PINDEKDNCNL());
			Sound.IFKCCDAIADF("snd_upgrade");
			return true;
		}
		return false;
	}

	private static bool JHLILCFNLAE(ItemInfo item, UserItem NDMCFNGEPOA)
	{
		if (NDMCFNGEPOA != null)
		{
			long bAINMLLIKOL = GlobalTimer.get_LocalTimeUTC() + item.EHKNIKHPGDN;
			NDMCFNGEPOA.set_DeliveryTime(bAINMLLIKOL);
			NDMCFNGEPOA.BAMLNLIDEBG(item.OBJDGBBFJOO);
			NDMCFNGEPOA.IJCEKDCPBAG(true);
			NDMCFNGEPOA.PJEEGECBHMH();
			ListSF.CCDKHLAMKKO().KHCNHPCPFII().GEFDJDIINND(NDMCFNGEPOA, true);
			Sound.IFKCCDAIADF("snd_upgrade");
			return true;
		}
		return false;
	}

	public static bool IHHKNBPKGHD(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		if (ListSF.CCDKHLAMKKO().BFBOEGMAMNF() >= (ObscuredLong)(item.KJFAOKLILOC))
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().BFBOEGMAMNF() - (ObscuredLong)(item.KJFAOKLILOC);
			bool flag = false;
			// Desktop/offline builds have no reliable server-backed delivery clock.
			// Complete coin purchases immediately so an order cannot strand the item.
			flag = KCBCGDFKNME(item);
			if (flag)
			{
				ListSF.CCDKHLAMKKO().OIOOMAKNIOB(bAINMLLIKOL);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
				LMBHFAHHDKI(item, StatisticsCollector.CNCDMFJLMFH.Money, false);
				CBADCGAEPGA(item);
			}
			return flag;
		}
		return false;
	}

	public static bool MGMAJHLAICA(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() >= (ObscuredLong)(item.FMHECGHHKGB))
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().EHFJHFDACMP() - (ObscuredLong)(item.FMHECGHHKGB);
			bool flag = KCBCGDFKNME(item);
			if (flag)
			{
				ListSF.CCDKHLAMKKO().LLNELLFMMBB(bAINMLLIKOL, Roster.HPOIJPGPOCF.CHANGE_BUY_ITEM);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
				LMBHFAHHDKI(item, StatisticsCollector.CNCDMFJLMFH.Bonus, false);
				CBADCGAEPGA(item);
			}
			return flag;
		}
		return false;
	}

	public static bool APICBINEPGJ(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN == null)
		{
			return false;
		}
		ItemInfo dJKEECEOCJB = dKCHDHMLKHN.HADDPFNDPDG();
		if (dJKEECEOCJB == null)
		{
			return false;
		}
		if (ListSF.CCDKHLAMKKO().BFBOEGMAMNF() >= (ObscuredLong)(dJKEECEOCJB.KJFAOKLILOC))
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().BFBOEGMAMNF() - (ObscuredLong)(dJKEECEOCJB.KJFAOKLILOC);
			bool flag = false;
			// Shop upgrades are immediate in the offline runtime. This also avoids
			// entering the legacy delivery branch without reporting success.
			flag = LBCJLCDMJLI(dJKEECEOCJB, dKCHDHMLKHN);
			if (flag)
			{
				ListSF.CCDKHLAMKKO().OIOOMAKNIOB(bAINMLLIKOL);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
				LMBHFAHHDKI(dJKEECEOCJB, StatisticsCollector.CNCDMFJLMFH.Money, false);
				CBADCGAEPGA(dJKEECEOCJB);
			}
			return flag;
		}
		return false;
	}

	public static bool JAJLOABHIMA(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN == null)
		{
			return false;
		}
		ItemInfo dJKEECEOCJB = dKCHDHMLKHN.HADDPFNDPDG();
		if (dJKEECEOCJB == null)
		{
			return false;
		}
		if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() >= (ObscuredLong)(dJKEECEOCJB.FMHECGHHKGB))
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().EHFJHFDACMP() - (ObscuredLong)(dJKEECEOCJB.FMHECGHHKGB);
			bool flag = LBCJLCDMJLI(dJKEECEOCJB, dKCHDHMLKHN);
			if (flag)
			{
				ListSF.CCDKHLAMKKO().LLNELLFMMBB(bAINMLLIKOL, Roster.HPOIJPGPOCF.CHANGE_BUY_ITEM);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
				LMBHFAHHDKI(dJKEECEOCJB, StatisticsCollector.CNCDMFJLMFH.Bonus, false);
				CBADCGAEPGA(dJKEECEOCJB);
			}
			return flag;
		}
		return false;
	}

	public static bool BuyImmediatelyDelivery(string OHCGEEEKEJH)
	{
		ItemInfo mBIJKDIEFIF = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(OHCGEEEKEJH);
		return BuyImmediatelyDelivery(mBIJKDIEFIF);
	}

	public static bool BuyImmediatelyDelivery(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN == null)
		{
			return false;
		}
		ItemInfo dJKEECEOCJB = dKCHDHMLKHN.HADDPFNDPDG();
		if (dJKEECEOCJB == null)
		{
			return false;
		}
		bool flag = ListSF.CCDKHLAMKKO().EHFJHFDACMP() >= (ObscuredLong)(dJKEECEOCJB.KLHOKKPALOK);
		bool flag2 = dKCHDHMLKHN.IJGAOHJNLAH() > GlobalTimer.get_LocalTimeUTC();
		if (flag && flag2)
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().EHFJHFDACMP() - (ObscuredLong)(dJKEECEOCJB.KLHOKKPALOK);
			ListSF.CCDKHLAMKKO().KHCNHPCPFII().GBLHFNGPIOF(dKCHDHMLKHN);
			ListSF.CCDKHLAMKKO().LLNELLFMMBB(bAINMLLIKOL, Roster.HPOIJPGPOCF.CHANGE_BUY_DELIVERY);
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
			LMBHFAHHDKI(dJKEECEOCJB, StatisticsCollector.CNCDMFJLMFH.Bonus, true);
			CBADCGAEPGA(dJKEECEOCJB);
			Sound.IFKCCDAIADF("snd_upgrade");
			return true;
		}
		return false;
	}

	public static bool NIEAANPCGLC(ItemInfo item)
	{
		if (item == null)
		{
			return false;
		}
		if (ListSF.CCDKHLAMKKO().EHFJHFDACMP() >= (ObscuredLong)(item.FMHECGHHKGB))
		{
			long bAINMLLIKOL = ListSF.CCDKHLAMKKO().EHFJHFDACMP() - (ObscuredLong)(item.FMHECGHHKGB);
			bool flag = KCBCGDFKNME(item);
			if (flag)
			{
				switch (item.MDPPNGIEJGD)
				{
				case "PerkReset":
					ListSF.CCDKHLAMKKO().JLBDOBLHHAF().LCDFOLAAEGM();
					break;
				case "Currency":
					ListSF.CCDKHLAMKKO().AddCurrencyCount(item.FAEGJAEEMGH, (ObscuredInt)(item.CPODJDDPJHB));
					break;
				}
				ListSF.CCDKHLAMKKO().LLNELLFMMBB(bAINMLLIKOL, Roster.HPOIJPGPOCF.CHANGE_BUY_ITEM);
				ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
				LMBHFAHHDKI(item, StatisticsCollector.CNCDMFJLMFH.Bonus, false);
				CBADCGAEPGA(item);
			}
			return flag;
		}
		return false;
	}

	private static void LMBHFAHHDKI(ItemInfo item, StatisticsCollector.CNCDMFJLMFH LFLGCDNKNJI, bool MNGGLFFHDJG)
	{
		ArgsDict kEMMIFBFDPK = new ArgsDict();
		kEMMIFBFDPK["item"] = item;
		kEMMIFBFDPK["type"] = LFLGCDNKNJI;
		kEMMIFBFDPK["immediatelyDelivery"] = MNGGLFFHDJG;
		StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Purchase, kEMMIFBFDPK);
	}

	private static void CBADCGAEPGA(ItemInfo item)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		FightIDS jLGLBLDPAAF = hHKLFIIBIFF.JLGLBLDPAAF;
		hHKLFIIBIFF.JLGLBLDPAAF = FightIDS.Empty();
		hHKLFIIBIFF.HEIADONEACH = string.Empty;
		hHKLFIIBIFF.AIEHNBBFNPF = string.Empty;
		hHKLFIIBIFF.DLKPBAJDHBO = item;
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
		hHKLFIIBIFF.JLGLBLDPAAF = jLGLBLDPAAF;
	}
}
