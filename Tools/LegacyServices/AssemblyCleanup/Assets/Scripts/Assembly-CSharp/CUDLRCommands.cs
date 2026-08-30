using System;
using System.Text;
using CUDLR;
using UnityEngine;

public static class CUDLRCommands
{
	[Command("help", "help - prints commands", true)]
	public static void BPOOJDFKMPD()
	{
		CUDLR.Console.Log(string.Format("Commands:{0}", CUDLR.Console.Instance.HelpData));
	}

	[Command("clear", "clear - clears CUDLR console output", true)]
	public static void Clear()
	{
		CUDLR.Console.Instance.ConsoleOutput.Clear();
	}

	[Command("uset", "uset - replace user on device", true)]
	public static void PADPLHFCJOL(string[] PCJAKPJMKGN)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string value in PCJAKPJMKGN)
		{
			stringBuilder.Append(value);
		}
		ListSF.SetRosterFileContent(stringBuilder.ToString());
	}

	[Command("m1", "m1 (int) - add amount of money", true)]
	public static void NICPPAKKIKG(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.NICPPAKKIKG, PCJAKPJMKGN);
	}

	[Command("m2", "m2 (int) - add amount of gems", true)]
	public static void KMJODCEKLDL(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.KMJODCEKLDL, PCJAKPJMKGN);
	}

	[Command("m1s", "m1s (int) - set amount of money", true)]
	public static void OKIIHGOJAMP(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.OKIIHGOJAMP, PCJAKPJMKGN);
	}

	[Command("m2s", "m2s (int) - set amount of gems", true)]
	public static void DLBPBAIJACF(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.DLBPBAIJACF, PCJAKPJMKGN);
	}

	[Command("level", "level/lvl (int) - add levels", true)]
	public static void LCDGAGJHGBJ(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.LCDGAGJHGBJ, PCJAKPJMKGN);
	}

	[Command("lvl", "level/lvl (int) - add levels", true)]
	public static void NILNJCOOCLL(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.LCDGAGJHGBJ, PCJAKPJMKGN);
	}

	[Command("user", "user [unlock|reset|print] - actions by user ('unlock' - unlock all items in shop, 'reset' - reset user progress, 'print' - print user to console)", true)]
	public static void NKHIGHOCKOP(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.NKHIGHOCKOP, PCJAKPJMKGN);
	}

	[Command("s", "s - skip tutorial", true)]
	public static void AONNCCIMKJG(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.AONNCCIMKJG, PCJAKPJMKGN);
	}

	[Command("payment", "payment [unlock/restore/products/receipt/satb/verify/log/logu/purchase] - some test commands for payment", true)]
	public static void EOHJMKFDBEI(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.EOHJMKFDBEI, PCJAKPJMKGN);
	}

	[Command("obfuscator", "obfuscator/of [logi/logw/loge/logexp] - some test commands for obfuscator", true)]
	public static void BKJDKEAFFNF(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.BKJDKEAFFNF, PCJAKPJMKGN);
	}

	[Command("of", "obfuscator/of [logi/logw/loge/logexp] - some test commands for obfuscator", true)]
	public static void DIBNKNJFOLF(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.BKJDKEAFFNF, PCJAKPJMKGN);
	}

	[Command("bundles", "bundles [reset] - some test commands for bundles ('reset' - reset bundles cache)", true)]
	public static void GPFCEPPIECF(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.GPFCEPPIECF, PCJAKPJMKGN);
	}

	[Command("uid", "uid - get uid for cudlr", true)]
	public static void EKGIBBMKNGJ(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.EKGIBBMKNGJ, PCJAKPJMKGN);
	}

	[Command("security", "security [full/honeypot/binary_enc/user_id/code_signature/file_man/jb/license/signature/installer/debug/emulator/uapps] - license check", true)]
	public static void PHOGKIPMJFI(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.PHOGKIPMJFI, PCJAKPJMKGN);
	}

	[Command("license", "license - get Apple AppReceipt on iOS and GooglePlayLicenseServerResponse on Android", true)]
	public static void FIFCCIDNJPJ(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.FIFCCIDNJPJ, PCJAKPJMKGN);
	}

	[Command("cheat", "cheat [win round|win fight|lose round|lose fight|reset round|reset fight]\ncheat [pause|next|min scale|magic|combo|style|crit]\ncheat [player godmode|bot godmode|debug|perks|slow]\ncheat (you can pass any kind of ControlQuadrant enum)", true)]
	public static void BLBPNMLECJC(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.MMDKNJAIPLJ, PCJAKPJMKGN);
	}

	[Command("notif", "notif t1 (int) - run simple Test-notification-1\nnotif [Fa0|Faf | Fa1|Fat] - force show all notifications (Paid/non-Paid SF2) off/on\nnotif ra (int) - run all notifs\nnotif info - print to console notif. infos\nnotif kill - cancel all notifications\nnotif en (t|1 | f|0) - enable or disable all notifications\nlook SFIIU-48 or Wiki for details", true)]
	public static void PNLANGKNJND(string[] PCJAKPJMKGN)
	{
		LDOJOKJOCME(RaidCheatManager.PEJJDCECHPC, PCJAKPJMKGN);
	}

	private static void LDOJOKJOCME(Func<string[], string> KHELBNPJGNJ, string[] PCJAKPJMKGN)
	{
		string message = KHELBNPJGNJ(PCJAKPJMKGN);
		Debug.Log(message);
	}
}
