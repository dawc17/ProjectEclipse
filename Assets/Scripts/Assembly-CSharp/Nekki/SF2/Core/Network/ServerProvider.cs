using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace Nekki.SF2.Core.Network
{
	public class ServerProvider : ServerProviderBase, JNEBPDNJFJG, ILicenseVerificationSender
	{
		public class LoginData
		{
			public bool EOKFDJIIKEA;

			public bool LNCICHDOMAL;

			public bool HADLDHHEOKM;

			public string UserID;

			public JSONNode Json;

			public KeyValuePair<long, long>? NBAJEOFOGIN;
		}

		private static ServerProvider _Instance;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string OFLIMNFAFHN;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string BBNDHEMGHHK;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string KPBKNDNFHEM;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string DGMBMOLNBHL;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string GGBOJPFJBJH;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string BDMEDOELLNO;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static string LCKGFBDBCOF;

		private static bool PFMFIPLKNND;

		private static bool CPINPNIGLAG;

		private static int JONHHMPDKCM;

		private static int JFIICDIOGGB;

		private static int PIINNMAEFIJ;

		private Action<LoginData> AMJMKNFBAMJ;

		private static string JHPIIKEJGNB
		{
			get
			{
				return ODPODCPFPAE();
			}
		}

		public string GKPACGEBJFP
		{
			get
			{
				return get_PutServer();
			}
		}

		public new static ServerProvider BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
		}

		public static string LBGNJDOKHEH
		{
			get
			{
				return get_PutURL();
			}
			set
			{
				set_PutURL(value);
			}
		}

		public static string CMGMOIJANNL
		{
			get
			{
				return get_GetURL();
			}
			set
			{
				set_GetURL(value);
			}
		}

		public static string GJBPJGLDPIJ
		{
			get
			{
				return get_ConfigURL();
			}
			set
			{
				set_ConfigURL(value);
			}
		}

		public static string EEEKJKIKLMD
		{
			get
			{
				return get_TimeServerURL();
			}
			set
			{
				set_TimeServerURL(value);
			}
		}

		public static string IIHIMJFONIL
		{
			get
			{
				return get_DumpPutURL();
			}
			set
			{
				set_DumpPutURL(value);
			}
		}

		public static string AHNMIPKNAKO
		{
			get
			{
				return get_DumpGetURL();
			}
			set
			{
				set_DumpGetURL(value);
			}
		}

		public static int LBHOGGEBNKD
		{
			get
			{
				return get_LoginInterval();
			}
			set
			{
				set_LoginInterval(value);
			}
		}

		private static string AHGJOLBDFLC
		{
			get
			{
				return LKCJJPONECB();
			}
		}

		protected override string NFKOPHMCLFF()
		{
			return get_GetURL();
		}

		private static string ODPODCPFPAE()
		{
			return GeneralConfig.ELEBLBJKDBI().OKJAHGKBGMK().Url;
		}

		public string get_PutServer()
		{
			return get_PutURL();
		}

		public new static ServerProvider get_Instance()
		{
			if (_Instance == null)
			{
				_Instance = ServerProviderBase.Init<ServerProvider>();
				_Instance.Init();
			}
			return _Instance;
		}

		public static string get_PutURL()
		{
			return OFLIMNFAFHN;
		}

		public static void set_PutURL(string value)
		{
			OFLIMNFAFHN = value;
		}

		public static string get_GetURL()
		{
			return BBNDHEMGHHK;
		}

		public static void set_GetURL(string value)
		{
			BBNDHEMGHHK = value;
		}

		public static string get_ConfigURL()
		{
			return KPBKNDNFHEM;
		}

		public static void set_ConfigURL(string value)
		{
			KPBKNDNFHEM = value;
		}

		public static string get_TimeServerURL()
		{
			return DGMBMOLNBHL;
		}

		public static void set_TimeServerURL(string value)
		{
			DGMBMOLNBHL = value;
		}

		public static string get_UserID()
		{
			return GGBOJPFJBJH;
		}

		public static void set_UserID(string value)
		{
			GGBOJPFJBJH = value;
		}

		public static string get_DumpPutURL()
		{
			return BDMEDOELLNO;
		}

		public static void set_DumpPutURL(string value)
		{
			BDMEDOELLNO = value;
		}

		public static string get_DumpGetURL()
		{
			return LCKGFBDBCOF;
		}

		public static void set_DumpGetURL(string value)
		{
			LCKGFBDBCOF = value;
		}

		public static int get_LoginInterval()
		{
			return PIINNMAEFIJ;
		}

		public static void set_LoginInterval(int value)
		{
			PIINNMAEFIJ = value;
		}

		public static void Reset()
		{
			CPINPNIGLAG = false;
			PIINNMAEFIJ = 0;
		}

		public static void Init(string KGBGENDIMBC, int PMCIHCMJDJA, int NJOOCOIIGDL)
		{
			if (!PFMFIPLKNND)
			{
				PFMFIPLKNND = true;
				JONHHMPDKCM = PMCIHCMJDJA;
				JFIICDIOGGB = NJOOCOIIGDL;
				Form.set_Key(KGBGENDIMBC);
			}
		}

		protected override void Init()
		{
		}

		protected override IEnumerator TimeSyncRoutine(Action<long> onDone, Action<string> onError)
		{
			onDone?.Invoke((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
			yield break;
		}

		private IEnumerator DOINCEFMGCL(string OBPHDPKKNLO, WWWForm OLMGMKFEOIK, Action<bool, string, object> p_delegate = null, object JHJDJOFPHPH = null)
		{
			p_delegate?.Invoke(false, "offline build", JHJDJOFPHPH);
			yield break;
		}

		public void Login(string JOPFPMFKGEO, Action<LoginData> PFCLKAALAAL, Dictionary<string, string> FBDKJJBICOK)
		{
			PFCLKAALAAL?.Invoke(new LoginData { EOKFDJIIKEA = false });
		}

		private KeyValuePair<long, long>? LKADGHNHOKL(JSONNode node)
		{
			bool flag = false;
			long key = 0L;
			long value = 0L;
			JSONNode jSONNode = node["timestamp"];
			if (jSONNode != null)
			{
				key = long.Parse(jSONNode.Value);
				flag = true;
			}
			JSONNode jSONNode2 = node["utc_offset"];
			if (jSONNode2 != null)
			{
				value = long.Parse(jSONNode2.Value);
			}
			if (flag)
			{
				return new KeyValuePair<long, long>(key, value);
			}
			return null;
		}

		private void IFIJMNGKNIM(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH)
		{
			bool hADLDHHEOKM = false;
			bool lNCICHDOMAL = false;
			bool eOKFDJIIKEA = false;
			KeyValuePair<long, long>? nBAJEOFOGIN = null;
			JSONNode jSONNode = null;
			if (AMKKLMOONEP)
			{
				try
				{
					jSONNode = JSON.Parse(GHDPPHAAPCA);
				}
				catch
				{
					LLLOJBFMONN.Error("Wrong server answer: {0}", GHDPPHAAPCA);
				}
			}
			if (AMKKLMOONEP && jSONNode != null)
			{
				JSONNode jSONNode2 = jSONNode["data"];
				if (jSONNode2 != null && jSONNode2.Value.Equals("user"))
				{
					JSONNode jSONNode3 = jSONNode["value"];
					if (jSONNode3 != null)
					{
						JSONNode jSONNode4 = jSONNode3["need_update"];
						hADLDHHEOKM = !jSONNode4.Value.Equals("0");
						JSONNode jSONNode5 = jSONNode3["is_new"];
						lNCICHDOMAL = !jSONNode5.Value.Equals("0");
						nBAJEOFOGIN = LKADGHNHOKL(jSONNode3);
						eOKFDJIIKEA = true;
						JSONNode jSONNode6 = jSONNode3["send_full_log"];
						if (jSONNode6 != null)
						{
							int result = 0;
							int.TryParse(jSONNode6.Value, out result);
							if (result == 1)
							{
								StatisticsCollector.AOJJOEHEPGM().LENBEPODJPC();
							}
						}
						JSONNode jSONNode7 = jSONNode3["should_log"];
						if (jSONNode7 != null)
						{
							int result2 = 0;
							int.TryParse(jSONNode7.Value, out result2);
							StatisticsCollector.AOJJOEHEPGM().GLKJABEOHDF(result2 == 1);
						}
						JSONNode jSONNode8 = jSONNode3["should_log_pay"];
						if (jSONNode8 != null)
						{
							int result3 = 0;
							int.TryParse(jSONNode8.Value, out result3);
							StatisticsCollector.AOJJOEHEPGM().IOGFHNKNGHJ(result3 == 1);
						}
						JSONNode jSONNode9 = jSONNode3["config_url"];
						if (jSONNode9 != null)
						{
							set_ConfigURL(jSONNode9.Value);
						}
						JSONNode jSONNode10 = jSONNode3["user_id"];
						if (jSONNode10 != null)
						{
							set_UserID(jSONNode10.Value);
						}
						JSONNode jSONNode11 = jSONNode3["login_frequency"];
						if (jSONNode11 != null)
						{
							set_LoginInterval(int.Parse(jSONNode11.Value));
						}
						JSONNode jSONNode12 = jSONNode3["total_payment_sum"];
						if (jSONNode12 != null)
						{
							float result4 = 0f;
							if (float.TryParse(jSONNode12.Value, out result4))
							{
								ListSF.CCDKHLAMKKO().PBEJGHOIPKC(result4);
							}
						}
						JSONNode jSONNode13 = jSONNode3["total_payment_count"];
						if (jSONNode13 != null)
						{
							int result5 = 0;
							if (int.TryParse(jSONNode13.Value, out result5))
							{
								ListSF.CCDKHLAMKKO().IKIHAIKLLOK(result5);
							}
						}
					}
				}
			}
			if (AMJMKNFBAMJ != null)
			{
				LoginData lNIJFNIAMML = new LoginData();
				lNIJFNIAMML.EOKFDJIIKEA = eOKFDJIIKEA;
				lNIJFNIAMML.LNCICHDOMAL = lNCICHDOMAL;
				lNIJFNIAMML.HADLDHHEOKM = hADLDHHEOKM;
				lNIJFNIAMML.UserID = get_UserID();
				lNIJFNIAMML.Json = jSONNode;
				lNIJFNIAMML.NBAJEOFOGIN = nBAJEOFOGIN;
				LoginData obj2 = lNIJFNIAMML;
				AMJMKNFBAMJ(obj2);
				AMJMKNFBAMJ = null;
			}
		}

		public void SaveJsonLogAction(string GHDPPHAAPCA, Action<bool, string, object> p_delegate, string IBODMPMJELJ = "save_json_log")
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("action", IBODMPMJELJ);
			lBFANOCPALF.Add("log", GHDPPHAAPCA);
			BGDAPMPOMFF(lBFANOCPALF);
			StartCoroutine(DOINCEFMGCL(get_PutServer(), (WWWForm)(lBFANOCPALF), p_delegate));
		}

		public void VerifyLicenseAction(object MIIDIJCIGKJ, string DBKFOHCPLDB, Action<bool, string, object> p_delegate)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("cmd", "verifyApp");
			lBFANOCPALF.Add("platform", DBKFOHCPLDB);
			lBFANOCPALF.Add("project", LKCJJPONECB());
			Form lBFANOCPALF2 = new Form();
			BGDAPMPOMFF(lBFANOCPALF2);
			JSONClass jSONClass = lBFANOCPALF2.LINEPHBFDFM();
			jSONClass["device_id"] = SystemProperties.OKLHMDPCGJL();
			GooglePlayLicenseServerResponse gEBIMFAJMGA = (GooglePlayLicenseServerResponse)MIIDIJCIGKJ;
			if (gEBIMFAJMGA != null)
			{
				jSONClass["response"] = gEBIMFAJMGA.GCHMKEIIAPJ();
				jSONClass["signature"] = gEBIMFAJMGA.MCDDGNJEKEO();
			}
			lBFANOCPALF.Add("data", jSONClass.ToString());
			LLLOJBFMONN.Write(lBFANOCPALF.ToString());
			StartCoroutine(DOINCEFMGCL(ODPODCPFPAE(), (WWWForm)(lBFANOCPALF), p_delegate, MIIDIJCIGKJ));
		}

		public void VerifyPurchaseAction(JLDHCFFAIPK PAENLDALDGB, string DBKFOHCPLDB, Action<bool, string, object> p_delegate)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("cmd", "verifyReceipt");
			lBFANOCPALF.Add("platform", DBKFOHCPLDB);
			lBFANOCPALF.Add("project", LKCJJPONECB());
			Form lBFANOCPALF2 = new Form();
			BGDAPMPOMFF(lBFANOCPALF2);
			JSONClass jSONClass = lBFANOCPALF2.LINEPHBFDFM();
			jSONClass["device_id"] = SystemProperties.OKLHMDPCGJL();
			jSONClass["pack_id"] = PAENLDALDGB.JLDEALIEEJI();
			jSONClass["receipt"] = PAENLDALDGB.JLFAPEOHKFE();
			if (PAENLDALDGB.MCDDGNJEKEO() != null)
			{
				jSONClass["signature"] = PAENLDALDGB.MCDDGNJEKEO();
			}
			lBFANOCPALF.Add("data", jSONClass.ToString());
			LLLOJBFMONN.Write(lBFANOCPALF.ToString());
			StartCoroutine(DOINCEFMGCL(ODPODCPFPAE(), (WWWForm)(lBFANOCPALF), p_delegate, PAENLDALDGB));
		}

		public void ConfirmVerificationAction(JLDHCFFAIPK PAENLDALDGB, string DBKFOHCPLDB, Action<bool, string, object> p_delegate)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("cmd", "confirmReceipt");
			lBFANOCPALF.Add("platform", DBKFOHCPLDB);
			lBFANOCPALF.Add("project", LKCJJPONECB());
			Form lBFANOCPALF2 = new Form();
			BGDAPMPOMFF(lBFANOCPALF2);
			JSONClass jSONClass = lBFANOCPALF2.LINEPHBFDFM();
			jSONClass["device_id"] = SystemProperties.OKLHMDPCGJL();
			jSONClass["receiptId"] = PAENLDALDGB.EJFAHFANGFM();
			lBFANOCPALF.Add("data", jSONClass.ToString());
			LLLOJBFMONN.Write(lBFANOCPALF.ToString());
			StartCoroutine(DOINCEFMGCL(ODPODCPFPAE(), (WWWForm)(lBFANOCPALF), p_delegate, PAENLDALDGB));
		}

		public void SendGiveLogin(Action<bool, string, object> IKFMKMEHJFF)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("action", "gives");
			StartCoroutine(DOINCEFMGCL(get_PutServer(), (WWWForm)(lBFANOCPALF), IKFMKMEHJFF));
		}

		public void CheckLedger(string BEPKJNKCKPH, Action<bool, string, object> IKFMKMEHJFF)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("project", "sf2");
			lBFANOCPALF.Add("device", SystemProperties.OKLHMDPCGJL());
			StartCoroutine(DOINCEFMGCL(BEPKJNKCKPH, (WWWForm)(lBFANOCPALF), IKFMKMEHJFF));
		}

		public void ConfirmLedger(string BEPKJNKCKPH, Action<bool, string, object> IKFMKMEHJFF, string DIAIIPCBMFL)
		{
			Form lBFANOCPALF = new Form();
			lBFANOCPALF.Add("ids", DIAIIPCBMFL);
			lBFANOCPALF.Add("project", "sf2");
			lBFANOCPALF.Add("device", SystemProperties.OKLHMDPCGJL());
			StartCoroutine(DOINCEFMGCL(BEPKJNKCKPH, (WWWForm)(lBFANOCPALF), IKFMKMEHJFF));
		}

		private static string LKCJJPONECB()
		{
			return (!SystemProperties.AFAAJMFLBIC()) ? "sf2" : "sf2_paid";
		}

		private static void BGDAPMPOMFF(Form OLMGMKFEOIK)
		{
			OLMGMKFEOIK.Add("build_version", SystemProperties.KCJMMIEBLHL().ToString());
			OLMGMKFEOIK.Add("data_version", SystemProperties.DFJEJKJECBI().ToString(true));
			OLMGMKFEOIK.Add("type", SystemProperties.ICMOGAMDEMM());
			if (ListSF.CCDKHLAMKKO() != null)
			{
				OLMGMKFEOIK.Add("publisher", ListSF.CCDKHLAMKKO().ODMONBDLMIP());
			}
			OLMGMKFEOIK.Add("os", SystemProperties.CFEDCPDNICD());
			OLMGMKFEOIK.Add("paid", (!SystemProperties.AFAAJMFLBIC()) ? "0" : "1");
			OLMGMKFEOIK.Add("imei", SystemProperties.IIILDACELJP());
			string arg = ((ListSF.CCDKHLAMKKO() == null) ? SystemProperties.IAAKNCJMAAK() : ListSF.CCDKHLAMKKO().ODMONBDLMIP());
			string arg2 = SystemProperties.MakeIdentifier(SystemProperties.GLLJKPBHELE());
			string bAINMLLIKOL = string.Format("{0}_{1}", arg, arg2);
			OLMGMKFEOIK.Add("token", SystemProperties.DBKBHEMJLLC());
			OLMGMKFEOIK.Add("uniqueID", bAINMLLIKOL);
			if (!string.IsNullOrEmpty(SystemProperties.OKLHMDPCGJL()))
			{
				OLMGMKFEOIK.Add("device_id", SystemProperties.OKLHMDPCGJL());
			}
		}

		private static void GBNCELMKNFH(Form OLMGMKFEOIK)
		{
			OLMGMKFEOIK.Add("dev", SystemProperties.OBKPEDOHCOO());
			OLMGMKFEOIK.Add("corecount", SystemProperties.NICPICAMAOH().MOMPODBNJNE.ToString());
			OLMGMKFEOIK.Add("ram", SystemProperties.NICPICAMAOH().AOJLHDILEBJ);
			OLMGMKFEOIK.Add("display_width", SystemProperties.MCGOBLKFGHO());
			OLMGMKFEOIK.Add("display_height", SystemProperties.OACFGEDMCOD());
		}

		private void AHNOPMCFKPJ(Form OLMGMKFEOIK)
		{
		}

		private void EDPIKBGBIGN(Form OLMGMKFEOIK)
		{
			OLMGMKFEOIK.Add("bonus", StatisticsCollector.GOBHGMIFLAA());
			OLMGMKFEOIK.Add("level", StatisticsCollector.FJNKLLJAPNL());
			OLMGMKFEOIK.Add("exp", StatisticsCollector.BFKLBDEEKLN());
			OLMGMKFEOIK.Add("money", StatisticsCollector.OBKJFLENCBC());
			OLMGMKFEOIK.Add("high_score", "0");
			if (ListSF.CCDKHLAMKKO() != null)
			{
				OLMGMKFEOIK.Add("location_id", ListSF.CCDKHLAMKKO().NFKHNICBOIB());
			}
		}

		public const bool OFFLINE = true;

		public void DownloadFile(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)
		{
			p_onDownloadComplete?.Invoke(new byte[0], "offline build", p_url);
		}

		private IEnumerator DownloadFileRoutine(string p_url, Action<byte[], string, string> p_onDownloadComplete, Action<float> MDJEOHMECHA = null, int DGDKHFPEHOG = 0)
		{
			p_onDownloadComplete?.Invoke(new byte[0], "offline build", p_url);
			yield break;
		}

		public void RestoreDump(Dictionary<string, string> data, Action<bool, JSONNode, object> p_delegate, uint DGDKHFPEHOG = 5000u, bool DINOLNKLNNP = false)
		{
			Form lBFANOCPALF = NLPEIONFIEC(data);
			BGDAPMPOMFF(lBFANOCPALF);
			LLLOJBFMONN.Write(lBFANOCPALF.ToString());
			StartCoroutine(DOINCEFMGCL(get_DumpGetURL(), (WWWForm)(lBFANOCPALF), CMIKJCGHINA, p_delegate));
		}

		private Form NLPEIONFIEC(Dictionary<string, string> data)
		{
			Form lBFANOCPALF = new Form();
			if (data != null)
			{
				foreach (KeyValuePair<string, string> item in data)
				{
					lBFANOCPALF.Add(item.Key, item.Value);
				}
			}
			return lBFANOCPALF;
		}

		private void CMIKJCGHINA(bool AMKKLMOONEP, string GHDPPHAAPCA, object JHJDJOFPHPH)
		{
			UnityEngine.Debug.Log("ServerProvider.ParseData: success = " + AMKKLMOONEP + ", p_data: " + ((GHDPPHAAPCA != null) ? GHDPPHAAPCA : "null"));
			bool flag = AMKKLMOONEP;
			JSONNode arg = null;
			if (flag)
			{
				JSONNode jSONNode = JSON.Parse(GHDPPHAAPCA);
				if (jSONNode == null)
				{
					flag = false;
				}
				else
				{
					JSONNode jSONNode2 = jSONNode["data"];
					if (jSONNode2 == null || jSONNode2.Value.Equals("error"))
					{
						flag = false;
					}
					else
					{
						arg = jSONNode;
					}
				}
			}
			if (JHJDJOFPHPH != null)
			{
				Action<bool, JSONNode, object> action = JHJDJOFPHPH as Action<bool, JSONNode, object>;
				action(flag, arg, null);
			}
		}

		public void sendRequestWithFiles(string BEPKJNKCKPH, Dictionary<string, string> data, Dictionary<string, FileData> IJGFBGNHAOI, Action<bool, JSONNode, object> p_delegate = null)
		{
			Form lBFANOCPALF = NLPEIONFIEC(data);
			if (IJGFBGNHAOI != null)
			{
				foreach (KeyValuePair<string, FileData> item in IJGFBGNHAOI)
				{
					lBFANOCPALF.HIIBLOGOILG(item.Key, item.Value);
				}
			}
			BGDAPMPOMFF(lBFANOCPALF);
			StartCoroutine(GBGCNNHAJBI(BEPKJNKCKPH, (WWWForm)(lBFANOCPALF), CMIKJCGHINA, p_delegate));
		}

		private IEnumerator GBGCNNHAJBI(string OBPHDPKKNLO, WWWForm OLMGMKFEOIK, Action<bool, string, object> p_delegate = null, object JHJDJOFPHPH = null)
		{
			p_delegate?.Invoke(false, "offline build", JHJDJOFPHPH);
			yield break;
		}
	}
}
