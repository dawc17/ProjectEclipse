using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Nekki.SF2.Core.Network;
using SimpleJSON;
using TheNextFlow.UnityPlugins;
using UnityEngine;

public class DumpController
{
	public delegate void CallbackSimple();

	private List<string> OIEDHOPPKKL;

	private bool JNEKGELOKCO;

	private static string HCLPPOHOJIL;

	private static int JGOBABDMOAG;

	private static long NEGNFJOHBFH;

	private static string DMNBADFCGFG;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private CallbackSimple onEndDump;

	private string CCBHBBMBGFJ
	{
		get
		{
			return CBBIGMBHMPH();
		}
	}

	public event CallbackSimple CDMGDJENNAO
	{
		add
		{
			ACEIJEGLJCD(value);
		}
		remove
		{
			BGONAGCGNGI(value);
		}
	}

	public void ACEIJEGLJCD(CallbackSimple value)
	{
		CallbackSimple bNPJNJFJFJN = onEndDump;
		CallbackSimple bNPJNJFJFJN2;
		do
		{
			bNPJNJFJFJN2 = bNPJNJFJFJN;
			bNPJNJFJFJN = Interlocked.CompareExchange(ref onEndDump, (CallbackSimple)Delegate.Combine(bNPJNJFJFJN2, value), bNPJNJFJFJN);
		}
		while ((object)bNPJNJFJFJN != bNPJNJFJFJN2);
	}

	public void BGONAGCGNGI(CallbackSimple value)
	{
		CallbackSimple bNPJNJFJFJN = onEndDump;
		CallbackSimple bNPJNJFJFJN2;
		do
		{
			bNPJNJFJFJN2 = bNPJNJFJFJN;
			bNPJNJFJFJN = Interlocked.CompareExchange(ref onEndDump, (CallbackSimple)Delegate.Remove(bNPJNJFJFJN2, value), bNPJNJFJFJN);
		}
		while ((object)bNPJNJFJFJN != bNPJNJFJFJN2);
	}

	public void Init(List<string> EDAAOBPEKJF)
	{
		OIEDHOPPKKL = EDAAOBPEKJF;
	}

	public void PAHKDLLDCDP()
	{
		if (!SystemProperties.DCKPKCIFOAG())
		{
			FNCFLJEFPIN("Failed check dump, no connection");
			return;
		}
		bool flag = !ListSF.CCDKHLAMKKO().FDPIBNJJDAK();
		bool flag2 = !GameCenterController.CONEABALMEJ().BKOIKMEEHDK();
		if (flag || flag2)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["action"] = "dump_restore";
			dictionary["first_launch"] = ((!flag) ? "0" : "1");
			MGEJMGEMKMJ(dictionary);
			ServerProvider.get_Instance().RestoreDump(dictionary, NKGBMHODKKN);
		}
		else
		{
			DMJPJPNDMNM();
		}
	}

	public bool IAKPFGCEFBE()
	{
		return !JNEKGELOKCO;
	}

	private void MJHDCMFJHEI()
	{
		if (IAKPFGCEFBE())
		{
			DMJPJPNDMNM();
		}
	}

	private string CBBIGMBHMPH()
	{
		return SF2Paths.GBOFOFGDMBN() + "/" + Constants.BCLABJJMIKI;
	}

	private void NKGBMHODKKN(bool AMKKLMOONEP, JSONNode GHDPPHAAPCA, object JHJDJOFPHPH)
	{
		if (!AMKKLMOONEP || GHDPPHAAPCA == null)
		{
			FNCFLJEFPIN("failed get dump");
			return;
		}
		JSONNode jSONNode = GHDPPHAAPCA["data"];
		if (jSONNode != null && jSONNode.Value.Equals("dump_restore"))
		{
			JSONNode jSONNode2 = GHDPPHAAPCA["value"];
			if (!jSONNode2["restore"].ParseBool())
			{
				ListSF.CCDKHLAMKKO().AJCCEFKDKIO(true);
				DMJPJPNDMNM();
				return;
			}
			HCLPPOHOJIL = jSONNode2["build_version"].CIPOICEEIBK();
			JGOBABDMOAG = jSONNode2["level"].ParseInt();
			NEGNFJOHBFH = jSONNode2["ctime"].ParseLong(0L);
			DMNBADFCGFG = jSONNode2["md5"].CIPOICEEIBK();
			string bEPKJNKCKPH = jSONNode2["url"].CIPOICEEIBK();
			PIAKLDFCLPB(bEPKJNKCKPH);
		}
		else
		{
			FNCFLJEFPIN("wrong format of dump backup");
		}
	}

	private void PIAKLDFCLPB(string BEPKJNKCKPH)
	{
		ServerProvider.get_Instance().DownloadFile(BEPKJNKCKPH, AFBJIOBLGGA);
	}

	private void AFBJIOBLGGA(byte[] DINHIPBNCDB, string JDONBAPIJCG, string BEPKJNKCKPH)
	{
		if (string.IsNullOrEmpty(JDONBAPIJCG) && DINHIPBNCDB != null)
		{
			File.WriteAllBytes(CBBIGMBHMPH(), DINHIPBNCDB);
			if (KHLLICCFPGI(DMNBADFCGFG))
			{
				if (VersionContainer.BCCGLNMPHCE(SystemProperties.KCJMMIEBLHL(), HCLPPOHOJIL))
				{
					DGILGBJBPGJ();
				}
				else
				{
					BLCHJHLLMBA();
				}
			}
			else
			{
				FNCFLJEFPIN("md5 mismatch");
			}
		}
		else
		{
			string text = "download failed with error: ";
			text += ((JDONBAPIJCG != null) ? "uknown error" : JDONBAPIJCG);
			FNCFLJEFPIN(text);
		}
	}

	private void CLMFAPNKJKC()
	{
		Compressor.Uncompress(CBBIGMBHMPH(), SF2Paths.APHDBIBDMDG() + "/");
		HGDMOIJAIMG();
		string aPFECPFKMMH = GameSettings.DGBHBMFEOAA();
		GameLoader.SetVersion(aPFECPFKMMH);
		FCIJEBAKDED();
	}

	private void HGDMOIJAIMG()
	{
		foreach (string item in OIEDHOPPKKL)
		{
			XmlUtils.IBPEILODDJP(item);
		}
	}

	private void MGEJMGEMKMJ(Dictionary<string, string> data)
	{
		if (data != null)
		{
			data.Add("user_id", ListSF.CCDKHLAMKKO().KNGJJEOLFHF());
			data.Add("account", GameCenterController.CONEABALMEJ());
			data.Add("level", ListSF.CCDKHLAMKKO().PINDEKDNCNL().ToString());
		}
	}

	private string NJBDLDIJCFK()
	{
		return MD5Utils.PIFDHBHOMJL(CBBIGMBHMPH());
	}

	private bool KHLLICCFPGI(string CIMFBHFACPM)
	{
		if (string.IsNullOrEmpty(CIMFBHFACPM))
		{
			return true;
		}
		return CIMFBHFACPM.Equals(NJBDLDIJCFK());
	}

	private void DMJPJPNDMNM()
	{
		ListSF.CCDKHLAMKKO().AJCCEFKDKIO(true);
		ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
		onEndDump();
		Compressor.Compress(OIEDHOPPKKL, CBBIGMBHMPH());
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action", "dump_save");
		dictionary.Add("md5", NJBDLDIJCFK());
		MGEJMGEMKMJ(dictionary);
		Dictionary<string, ServerProviderBase.FileData> dictionary2 = new Dictionary<string, ServerProviderBase.FileData>();
		dictionary2.Add("dump_file", new ServerProviderBase.FileData(CBBIGMBHMPH(), CBBIGMBHMPH(), "text/xml"));
		ServerProvider.get_Instance().sendRequestWithFiles(ServerProvider.get_DumpPutURL(), dictionary, dictionary2, KHHLKPPINPH);
		JNEKGELOKCO = false;
	}

	private void KHHLKPPINPH(bool AMKKLMOONEP, JSONNode GHDPPHAAPCA, object JHJDJOFPHPH)
	{
		FCIJEBAKDED();
	}

	private void FNCFLJEFPIN(string JDONBAPIJCG)
	{
		if (JDONBAPIJCG == "failed get dump")
		{
			UnityEngine.Debug.LogWarning("[UserData] Cloud dump unavailable; continuing with the local save.");
		}
		else
		{
			UnityEngine.Debug.LogErrorFormat("Dump error: {0}", JDONBAPIJCG);
		}
		JNEKGELOKCO = true;
		onEndDump();
	}

	private void CLMJNIEPEHP()
	{
		FCIJEBAKDED();
		ListSF.CCDKHLAMKKO().AJCCEFKDKIO(true);
		DMJPJPNDMNM();
	}

	private void FCIJEBAKDED()
	{
		File.Delete(CBBIGMBHMPH());
	}

	private void BLCHJHLLMBA()
	{
		string message = LocalizationManager.GetString("restoreMessage2", JGOBABDMOAG.ToString(), LocalizationManager.DateString(NEGNFJOHBFH));
		string title = LocalizationManager.GetString("restoreTitle");
		string ok = LocalizationManager.GetString("restoreBtnUpdate");
		string cancel = LocalizationManager.GetString("CANCEL");
		MobileNativePopups.OpenAlertDialog(title, message, ok, cancel, IELIDGGGNMN, EIOLICMENMJ);
	}

	private void IELIDGGGNMN()
	{
		DialogsOpener.LMHIIMALDKF();
	}

	private void EIOLICMENMJ()
	{
		CLMJNIEPEHP();
	}

	private void DGILGBJBPGJ()
	{
		int num = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		string message = LocalizationManager.GetString("restoreMessage1", JGOBABDMOAG.ToString(), LocalizationManager.DateString(NEGNFJOHBFH), num.ToString());
		string title = LocalizationManager.GetString("restoreTitle");
		string ok = LocalizationManager.GetString("restoreBtnRestore");
		string cancel = LocalizationManager.GetString("CANCEL");
		MobileNativePopups.OpenAlertDialog(title, message, ok, cancel, EJJOACFABFE, FGBCJACJMKF);
	}

	private void EJJOACFABFE()
	{
		JKFNCEDNMAD();
	}

	private void FGBCJACJMKF()
	{
		CLMJNIEPEHP();
	}

	private void JKFNCEDNMAD()
	{
		string message = LocalizationManager.GetString("restoreMessage3");
		string title = LocalizationManager.GetString("restoreTitle");
		string ok = LocalizationManager.GetString("restoreBtnRestore");
		string cancel = LocalizationManager.GetString("CANCEL");
		MobileNativePopups.OpenAlertDialog(title, message, ok, cancel, LGMMGIDANCG, MLJJFPFKKFF);
	}

	private void LGMMGIDANCG()
	{
		CLMFAPNKJKC();
		ListSF.Reset();
		ListSF.ELEBLBJKDBI().IIKDNMBIHCM();
		GameUtils.CGFHDKDJCPL();
		onEndDump();
	}

	private void MLJJFPFKKFF()
	{
		DGILGBJBPGJ();
	}
}
