using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nekki.SF2.Core;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Common;
using Nekki.SF2.GUI.Menu;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using SF2.Offline;

public static class RaidCheatManager
{
	private static bool _isInited;

	public const string GDHEONNOLME = "help - show all commands";

	public const string PFCNEOKCEFC = "clear - clear console";

	public const string LLKAGAIBDIP = "m1 (int) - add amount of money";

	public const string IBDGMGGBDCH = "m2 (int) - add amount of gems";

	public const string IECNKNBLLEC = "m1s (int) - set amount of money";

	public const string LKBOGPMPLNC = "m2s (int) - set amount of gems";

	public const string AFMIHGDIOIJ = "level/lvl (int) - add levels";

	public const string MFBMAAPIPDK = "user [unlock|reset|print] - actions by user ('unlock' - unlock all items in shop, 'reset' - reset user progress, 'print' - print user to console)";

	public const string AIGOPDIPLHD = "s - skip tutorial";

	public const string EKCBPAHIFEE = "payment [unlock/restore/products/receipt/satb/verify/log/logu/purchase] - some test commands for payment";

	public const string CIGDINFMCPF = "obfuscator/of [logi/logw/loge/logexp] - some test commands for obfuscator";

	public const string PKIAHHJBCAK = "bundles [reset] - some test commands for bundles ('reset' - reset bundles cache)";

	public const string MKPMOMMOIHN = "cheat [win round|win fight|lose round|lose fight|reset round|reset fight]\ncheat [pause|next|min scale|magic|combo|style|crit]\ncheat [player godmode|bot godmode|debug|perks|slow]\ncheat (you can pass any kind of ControlQuadrant enum)";

	public const string DOJLNEANBAE = "notif t1 (int) - run simple Test-notification-1\nnotif [Fa0|Faf | Fa1|Fat] - force show all notifications (Paid/non-Paid SF2) off/on\nnotif ra (int) - run all notifs\nnotif info - print to console notif. infos\nnotif kill - cancel all notifications\nnotif en (t|1 | f|0) - enable or disable all notifications\nlook SFIIU-48 or Wiki for details";

	public static void Init()
	{
		if (!_isInited)
		{
			_isInited = true;
			ConsoleUI.add_OnConsoleActive(GKMLAKACCFO);
			ConsoleDatabase.IMEAIJNKBOP("help", BPOOJDFKMPD);
			ConsoleDatabase.IMEAIJNKBOP("clear", Clear);
			ConsoleDatabase.IMEAIJNKBOP("m1", NICPPAKKIKG);
			ConsoleDatabase.IMEAIJNKBOP("m2", KMJODCEKLDL);
			ConsoleDatabase.IMEAIJNKBOP("m1s", OKIIHGOJAMP);
			ConsoleDatabase.IMEAIJNKBOP("m2s", DLBPBAIJACF);
			ConsoleDatabase.IMEAIJNKBOP("level", LCDGAGJHGBJ);
			ConsoleDatabase.IMEAIJNKBOP("lvl", LCDGAGJHGBJ);
			ConsoleDatabase.IMEAIJNKBOP("user", NKHIGHOCKOP);
			ConsoleDatabase.IMEAIJNKBOP("s", AONNCCIMKJG);
			ConsoleDatabase.IMEAIJNKBOP("payment", EOHJMKFDBEI);
			ConsoleDatabase.IMEAIJNKBOP("obfuscator", BKJDKEAFFNF);
			ConsoleDatabase.IMEAIJNKBOP("of", BKJDKEAFFNF);
			ConsoleDatabase.IMEAIJNKBOP("bundles", GPFCEPPIECF);
			ConsoleDatabase.IMEAIJNKBOP("cheat", MMDKNJAIPLJ);
			ConsoleDatabase.IMEAIJNKBOP("notif", PEJJDCECHPC);
		}
	}

	public static void GKMLAKACCFO(bool OEKIAFFOPJJ)
	{
		if (GameController.get_Current() != null)
		{
			GameController.get_Current().enabled = !OEKIAFFOPJJ;
		}
	}

	public static void Log(string BFFNFGKHBJA)
	{
		ConsoleUI.Log(BFFNFGKHBJA);
		Debug.Log(BFFNFGKHBJA);
	}

	private static string BPOOJDFKMPD(params string[] PCJAKPJMKGN)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("All commands:\n");
		stringBuilder.AppendLine("help - show all commands");
		stringBuilder.AppendLine("clear - clear console");
		stringBuilder.AppendLine("m1 (int) - add amount of money");
		stringBuilder.AppendLine("m2 (int) - add amount of gems");
		stringBuilder.AppendLine("m1s (int) - set amount of money");
		stringBuilder.AppendLine("m2s (int) - set amount of gems");
		stringBuilder.AppendLine("level/lvl (int) - add levels");
		stringBuilder.AppendLine("user [unlock|reset|print] - actions by user ('unlock' - unlock all items in shop, 'reset' - reset user progress, 'print' - print user to console)");
		stringBuilder.AppendLine("s - skip tutorial");
		stringBuilder.AppendLine("payment [unlock/restore/products/receipt/satb/verify/log/logu/purchase] - some test commands for payment");
		stringBuilder.AppendLine("obfuscator/of [logi/logw/loge/logexp] - some test commands for obfuscator");
		stringBuilder.AppendLine("cheat [win round|win fight|lose round|lose fight|reset round|reset fight]\ncheat [pause|next|min scale|magic|combo|style|crit]\ncheat [player godmode|bot godmode|debug|perks|slow]\ncheat (you can pass any kind of ControlQuadrant enum)");
		stringBuilder.AppendLine("notif t1 (int) - run simple Test-notification-1\nnotif [Fa0|Faf | Fa1|Fat] - force show all notifications (Paid/non-Paid SF2) off/on\nnotif ra (int) - run all notifs\nnotif info - print to console notif. infos\nnotif kill - cancel all notifications\nnotif en (t|1 | f|0) - enable or disable all notifications\nlook SFIIU-48 or Wiki for details");
		return stringBuilder.ToString().TrimEnd('\r', '\n');
	}

	public static string Clear(params string[] PCJAKPJMKGN)
	{
		ConsoleUI.Clear();
		return "Ok";
	}

	private static int AAGLGCNIKJL(params string[] LKIOKGCNKHE)
	{
		int result;
		if (LKIOKGCNKHE.Length > 0 && int.TryParse(LKIOKGCNKHE[0], out result))
		{
			return result;
		}
		return 1000000;
	}

	public static string NICPPAKKIKG(params string[] LKIOKGCNKHE)
	{
		ListSF.GCPJADIMNKI(AAGLGCNIKJL(LKIOKGCNKHE));
		if (MainMenu.get_Instance() != null)
		{
			MainMenu.get_Instance().UpdateMoney();
		}
		return string.Format("Ok. Money={0}", ListSF.CCDKHLAMKKO().BFBOEGMAMNF());
	}

	public static string KMJODCEKLDL(params string[] LKIOKGCNKHE)
	{
		ListSF.FPIJEOMBFJN(AAGLGCNIKJL(LKIOKGCNKHE), Roster.HPOIJPGPOCF.CHANGE_CHEAT);
		if (MainMenu.get_Instance() != null)
		{
			MainMenu.get_Instance().UpdateMoney();
		}
		return string.Format("Ok. Gems={0}", ListSF.CCDKHLAMKKO().EHFJHFDACMP());
	}

	public static string OKIIHGOJAMP(params string[] LKIOKGCNKHE)
	{
		int num = AAGLGCNIKJL(LKIOKGCNKHE);
		if (num < 0)
		{
			return "Must be >= 0";
		}
		ListSF.PNINBKEIBHO(num);
		if (MainMenu.get_Instance() != null)
		{
			MainMenu.get_Instance().UpdateMoney();
		}
		return string.Format("Ok. Money={0}", ListSF.CCDKHLAMKKO().BFBOEGMAMNF());
	}

	public static string DLBPBAIJACF(params string[] LKIOKGCNKHE)
	{
		int num = AAGLGCNIKJL(LKIOKGCNKHE);
		if (num < 0)
		{
			return "Must be >= 0";
		}
		ListSF.BMHBGNDHPIJ(num, Roster.HPOIJPGPOCF.CHANGE_CHEAT);
		if (MainMenu.get_Instance() != null)
		{
			MainMenu.get_Instance().UpdateMoney();
		}
		return string.Format("Ok. Gems={0}", ListSF.CCDKHLAMKKO().EHFJHFDACMP());
	}

	public static string LCDGAGJHGBJ(params string[] PCJAKPJMKGN)
	{
		int result = 1;
		if (PCJAKPJMKGN.Length > 0)
		{
			int.TryParse(PCJAKPJMKGN[0], out result);
		}
		while (result > 0)
		{
			ListSF.CCDKHLAMKKO().DBPBGBNHAIP(ListSF.CCDKHLAMKKO().HEOHJNFGEDH());
			result--;
		}
		if (MainMenu.get_Instance() != null)
		{
			MainMenu.get_Instance().UpdateLevel();
		}
		if (Scene<ShopScene>.get_Current() != null)
		{
			Scene<ShopScene>.get_Current().RefreshItems();
		}
		return string.Format("Ok. Level={0}", ListSF.CCDKHLAMKKO().PINDEKDNCNL());
	}

	public static string NKHIGHOCKOP(params string[] PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.Length == 0)
		{
			return "No Arguments! Try: user unlock/reset/print";
		}
		switch (PCJAKPJMKGN[0])
		{
		case "unlock":
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_2", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_3", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_4", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_5", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_6", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_IM", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_1", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_2", true);
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_7_3", true);
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
			return "Ok";
		case "reset":
			ListSF.CELGPFFHLIM();
			return "Ok";
		case "print":
			return ListSF.FDJICKDCIBI();
		default:
			return string.Format("unknown argument {0}", PCJAKPJMKGN[0]);
		}
	}

	public static string AONNCCIMKJG(params string[] PCJAKPJMKGN)
	{
		if (MainMenu.get_Instance() != null && Module.ELEBLBJKDBI() != null && Module.ELEBLBJKDBI().OMDLOOFIJDF())
		{
			MainMenu.get_Instance().SkipTutorial();
		}
		return "Ok";
	}

	public static string EOHJMKFDBEI(params string[] PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.Length == 0)
		{
			return "No Arguments! Try: payment [unlock/restore/products/receipt/satb/verify/log/logu/purchase]";
		}
		switch (PCJAKPJMKGN[0])
		{
		case "unlock":
			ListSF.CCDKHLAMKKO().AddShopLock("ZONE_1_DONATE", true);
			ListSF.CCDKHLAMKKO().AddShopLock("COMMON_RUBY_DONATE", true);
			ListSF.CCDKHLAMKKO().AddShopLock("COMMON_RUBY_DONATE_CASKET", true);
			ListSF.CCDKHLAMKKO().GGGEHAGCLGC();
			return "Ok";
		case "restore":
			ICFMIHIKGOD.OFFDIMCJOIC().JDMELMJCKMN();
			return "Ok";
		case "products":
			ICFMIHIKGOD.OFFDIMCJOIC().BKFGAIHBCHL();
			return "Ok";
		case "receipt":
			if (SystemProperties.MEBGOGMJFLM())
			{
				var appleExt = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();
				if (appleExt != null)
				{
					appleExt.KJGFLKHCEJM();
				}
				return "Ok";
			}
			return "Can run 'receipt' only on iOS platform!";
		case "satb":
			if (SystemProperties.MEBGOGMJFLM())
			{
				bool flag = false;
				if (PCJAKPJMKGN.Length > 1)
				{
					flag = PCJAKPJMKGN[1] == "on" || PCJAKPJMKGN[1] == "1";
				}
				var appleExtSim = ICFMIHIKGOD.OFFDIMCJOIC().MDMDFHPCOEI<LFFGCBPOGPJ>();
				if (appleExtSim != null)
				{
					appleExtSim.JHDECICMLKO(flag);
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendLine("SimulateAskToBuy=" + ((!flag) ? "0" : "1"));
				stringBuilder2.AppendLine("Ok");
				return stringBuilder2.ToString();
			}
			return "Can run 'satb' only on iOS platform!";
		case "verify":
		{
			bool flag2 = false;
			if (PCJAKPJMKGN.Length > 1)
			{
				flag2 = PCJAKPJMKGN[1] == "on" || PCJAKPJMKGN[1] == "1";
			}
			KMAENHJICNF.DLDANNALFEA(flag2);
			if (flag2)
			{
				ICFMIHIKGOD.DCPEBKEGOHG();
			}
			return "Ok";
		}
		case "log":
		{
			if (ICFMIHIKGOD.LHGPKEFEHDH())
			{
				return "Can't log products in emulator";
			}
			StringBuilder stringBuilder = new StringBuilder();
			Product[] array2 = ICFMIHIKGOD.OFFDIMCJOIC().NABJBCEKEHK();
			stringBuilder.AppendLine("Products:");
			Product[] array3 = array2;
			foreach (Product product in array3)
			{
				stringBuilder.AppendLine(product.definition.Log());
			}
			stringBuilder.AppendLine("Ok");
			return stringBuilder.ToString();
		}
		case "logu":
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			List<JLDHCFFAIPK> list = ICFMIHIKGOD.MDLJADJGDOL();
			stringBuilder3.AppendLine("InProgress:");
			foreach (JLDHCFFAIPK item in list)
			{
				stringBuilder3.AppendLine(item.ToString());
			}
			List<JLDHCFFAIPK> list2 = ICFMIHIKGOD.JJIKCAEFPIO();
			stringBuilder3.AppendLine("Completed:");
			foreach (JLDHCFFAIPK item2 in list2)
			{
				stringBuilder3.AppendLine(item2.ToString());
			}
			stringBuilder3.AppendLine("Ok");
			return stringBuilder3.ToString();
		}
		case "purchase":
		{
			if (PCJAKPJMKGN.Length < 2)
			{
				return "No Arguments! Try: payment [purchase] [int]";
			}
			int result = 0;
			int.TryParse(PCJAKPJMKGN[1], out result);
			Product[] array = ICFMIHIKGOD.OFFDIMCJOIC().NABJBCEKEHK();
			if (result < 0 || result >= array.Length)
			{
				return "Incorrect index";
			}
			ICFMIHIKGOD.OFFDIMCJOIC().BDAAKHOLPOF(array[result].definition.id);
			return "purchaseStart: " + array[result].definition.id;
		}
		default:
			return "Unknown payment action!";
		}
	}

	public static string BKJDKEAFFNF(params string[] PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.Length == 0)
		{
			return "No Arguments! Try: obfuscator/of [logi/logw/loge/logexp]";
		}
		switch (PCJAKPJMKGN[0])
		{
		case "logi":
			Debug.Log("Info");
			return "Ok";
		case "logw":
			Debug.LogWarning("Warning");
			return "Ok";
		case "loge":
			Debug.LogError("Error");
			return "Ok";
		case "logexp":
			try
			{
				throw new Exception("Exception");
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return "Ok";
		default:
			return "Unknown obfuscator action!";
		}
	}

	public static string GPFCEPPIECF(params string[] PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.Length == 0)
		{
			return "No Arguments! Try: bundles [reset]";
		}
		string text = PCJAKPJMKGN[0];
		if (text != null && text == "reset")
		{
			PacksController.ELEBLBJKDBI().AKMIAJPGHDC();
			ApplicationController.Quit();
			return "Ok";
		}
		return "Unknown bundles action!";
	}

	public static string MMDKNJAIPLJ(params string[] LKIOKGCNKHE)
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA == null)
		{
			return "NOT IN FIGHT";
		}
		string text = string.Join(string.Empty, LKIOKGCNKHE).ToLower();
		text = text.Replace("magic", "rechargemagic").Replace("lose", "loss").Replace("godmode", "immortality");
		string[] names = Enum.GetNames(typeof(FightCID));
		foreach (string text2 in names)
		{
			if (text2.ToLower().Contains(text))
			{
				FightCID eCHINOPKGGI = (FightCID)Enum.Parse(typeof(FightCID), text2);
				gDBOMJODDEA.ReleaseAnyKey(eCHINOPKGGI);
				return "OK - " + eCHINOPKGGI;
			}
		}
		return "NOT RECOGNIZED\nUSAGE:\ncheat [win round|win fight|lose round|lose fight|reset round|reset fight]\ncheat [pause|next|min scale|magic|combo|style|crit]\ncheat [player godmode|bot godmode|debug|perks|slow]\ncheat (you can pass any kind of ControlQuadrant enum)";
	}

	private static long OBFEHAECKNG(params string[] LKIOKGCNKHE)
	{
		int result = 3;
		if (LKIOKGCNKHE.Length >= 2)
		{
			int.TryParse(LKIOKGCNKHE[1], out result);
		}
		return result;
	}

	private static bool? EIOCCPBAELC(int index, params string[] LKIOKGCNKHE)
	{
		if (LKIOKGCNKHE.Length <= index)
		{
			return null;
		}
		string text = LKIOKGCNKHE[index].ToLower();
		if (text.Equals("t") || text.Equals("1"))
		{
			return true;
		}
		if (text.Equals("f") || text.Equals("0"))
		{
			return false;
		}
		return null;
	}

	public static string PEJJDCECHPC(params string[] LKIOKGCNKHE)
	{
		if (LKIOKGCNKHE.Length == 0)
		{
			return "No Arguments! Try:\nnotif t1 (int) - run simple Test-notification-1\nnotif [Fa0|Faf | Fa1|Fat] - force show all notifications (Paid/non-Paid SF2) off/on\nnotif ra (int) - run all notifs\nnotif info - print to console notif. infos\nnotif kill - cancel all notifications\nnotif en (t|1 | f|0) - enable or disable all notifications\nlook SFIIU-48 or Wiki for details";
		}
		switch (LKIOKGCNKHE[0].ToLower())
		{
		case "kill":
			LocalNotificationManager.ELEBLBJKDBI().PLEIIJMCPHF();
			return "canceled all notifications";
		case "info":
			return LocalNotificationManager.ELEBLBJKDBI().IINLKICBLEB();
		case "t1":
		{
			long num = OBFEHAECKNG(LKIOKGCNKHE);
			LocalNotificationManager.ELEBLBJKDBI().JHIJCEJBEGP();
			LocalNotificationManager.ELEBLBJKDBI().IOEKOAKONGH(num);
			return "Test-notification-1 launched, delay=" + num;
		}
		case "faf":
		case "fa0":
			LocalNotificationManager.ELEBLBJKDBI().KKJBEGLMHCM(false);
			return "disabled force-All notifications";
		case "fat":
		case "fa1":
			LocalNotificationManager.ELEBLBJKDBI().KKJBEGLMHCM(true);
			return "enabled force-All notifications";
		case "ra":
		{
			LocalNotificationManager.ELEBLBJKDBI().PLEIIJMCPHF();
			long num = OBFEHAECKNG(LKIOKGCNKHE);
			LocalNotificationManager.ELEBLBJKDBI().DODOMBCHMDN(num);
			LocalNotificationManager.ELEBLBJKDBI().HOHFHDMEDLI(num + 2);
			LocalNotificationManager.ELEBLBJKDBI().HGOKJEIHKPE("Test item", num + 4);
			LocalNotificationManager.ELEBLBJKDBI().HPCBBNCDPEB(num + 6);
			LocalNotificationManager.ELEBLBJKDBI().EGOMGODAMFF("Test recipe", num + 8);
			LocalNotificationManager.ELEBLBJKDBI().IMABAABEIOI(num + 10);
			LocalNotificationManager.ELEBLBJKDBI().IOEKOAKONGH(num + 12);
			int num2 = 7;
			return "Run all notifications(" + num2 + "), min delay=" + num + ", max delay=" + (num + 12);
		}
		case "en":
		{
			bool? flag = EIOCCPBAELC(1, LKIOKGCNKHE);
			if (!flag.HasValue)
			{
				break;
			}
			LocalNotificationManager.ELEBLBJKDBI().AGONLLFBOFG(flag.Value);
			return (!flag.Value) ? "Ok, notifs disabled (IOS-remotes to)" : "Ok, notifs enabled";
		}
		}
		return "NOT RECOGNIZED USAGE:\nnotif t1 (int) - run simple Test-notification-1\nnotif [Fa0|Faf | Fa1|Fat] - force show all notifications (Paid/non-Paid SF2) off/on\nnotif ra (int) - run all notifs\nnotif info - print to console notif. infos\nnotif kill - cancel all notifications\nnotif en (t|1 | f|0) - enable or disable all notifications\nlook SFIIU-48 or Wiki for details";
	}
}
