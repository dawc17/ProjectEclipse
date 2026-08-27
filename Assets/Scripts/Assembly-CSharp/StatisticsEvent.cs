using System;
using SimpleJSON;

public static class StatisticsEvent
{
	public enum HANNAHNOAGH
	{
		Purchased = 0,
		Restored = 1,
		Cancelled = 2,
		Failed = 3,
		Pending = 4,
		Already_Owned = 5
	}

	public enum AICJEAMBGCE
	{
		Start = 0,
		Finish = 1,
		Confirm_Start = 2,
		Confirm_Finish = 3
	}

	public enum JDNFFHILFAF
	{
		Unknown = 0,
		User = 1,
		Fight_End = 2,
		Achievement = 3,
		Gems_Changed = 4,
		Level_Up = 5,
		Purchase = 6,
		Perk = 7,
		Session_End = 8,
		Payment = 9,
		Discount = 10,
		Enchantment = 11,
		Set_Acquisition = 12,
		Raid_Timeout = 13,
		Pay_Request_Start = 14,
		Pay_Status_Update = 15,
		Pay_Handle_Start = 16,
		Pay_Handle_Finish = 17,
		Pay_Transaction_Finish = 18,
		Pay_Handle_Error = 19,
		Pay_Handle_Debug = 20,
		Pay_Request_Fail = 21,
		Pay_Verification_Status_Change = 22
	}

	private static int CGGJCHNOHNI
	{
		get
		{
			return KBAHFOGFNDP();
		}
	}

	private static string FLLBMAMLCFN
	{
		get
		{
			return KMFDGKKFLFI();
		}
	}

	public static bool KLOICDEDMEB(JDNFFHILFAF IGABHEMGKKE)
	{
		return true;
	}

	public static bool IFDADIENEKC(JDNFFHILFAF IGABHEMGKKE)
	{
		return true;
	}

	public static string PCGEAIIJICB(JDNFFHILFAF IGABHEMGKKE, ArgsDict PCJAKPJMKGN)
	{
		switch (IGABHEMGKKE)
		{
		case JDNFFHILFAF.User:
			ADOKACKPDDH();
			return MJAMJOPLMEP(PCJAKPJMKGN);
		case JDNFFHILFAF.Fight_End:
			StatisticsCollector.JNGMOOOPCBN(StatisticsCollector.MHKHJJIJKCD() + 1);
			return GOAECNHBDCG(PCJAKPJMKGN);
		case JDNFFHILFAF.Achievement:
			return MNCOAALINIE(PCJAKPJMKGN);
		case JDNFFHILFAF.Gems_Changed:
			return AAEBOCIOKMB(PCJAKPJMKGN);
		case JDNFFHILFAF.Level_Up:
			return OLHHNGGKJKI(PCJAKPJMKGN);
		case JDNFFHILFAF.Purchase:
			return AAMOBDLBDOB(PCJAKPJMKGN);
		case JDNFFHILFAF.Payment:
			return NMHPIPKJLJG(PCJAKPJMKGN);
		case JDNFFHILFAF.Perk:
			return INIPHNCIGLC(PCJAKPJMKGN);
		case JDNFFHILFAF.Session_End:
			StatisticsCollector.HIMFCMGOMGI(StatisticsCollector.JGMAONJINDK() + 1);
			return IBMHOJGNMIA(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Request_Start:
			return JGBHBFDJMKE(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Status_Update:
			return DBFHBHIAJGD(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Handle_Start:
			return EMACNBCLAME(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Handle_Finish:
			return OEKBFPLNHDK(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Transaction_Finish:
			return AKMODMOANPC(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Handle_Error:
			return MIDGNABOABO(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Handle_Debug:
			return EJIHJCDNFJL(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Request_Fail:
			return CLCKGBPKHPG(PCJAKPJMKGN);
		case JDNFFHILFAF.Pay_Verification_Status_Change:
			return CILMAEFHJNG(PCJAKPJMKGN);
		default:
			return null;
		}
	}

	private static JSONClass DIFAPLOBCCL(JDNFFHILFAF IGABHEMGKKE)
	{
		JSONClass jSONClass = new JSONClass();
		string text = AEMNGCELEDC(IGABHEMGKKE);
		jSONClass["eid"] = StatisticsCollector.MONNGMBGLHH();
		jSONClass["etype"] = text;
		jSONClass["build_version"] = SystemProperties.KCJMMIEBLHL().ToString();
		jSONClass["data_version"] = SystemProperties.DFJEJKJECBI().ToString(true);
		jSONClass["install_id"] = ListSF.CCDKHLAMKKO().BPOHPIJMFMA();
		jSONClass["total_payment_sum"] = ListSF.CCDKHLAMKKO().INPAOPFFKEJ();
		jSONClass["timestamp"] = KBAHFOGFNDP();
		jSONClass["paid_version"] = SystemProperties.AFAAJMFLBIC();
		if (SystemProperties.DBBOCENKMGD())
		{
			HPHAIPJPPHJ(jSONClass, "debug");
		}
		if (SystemProperties.LHGPKEFEHDH())
		{
			HPHAIPJPPHJ(jSONClass, "simulator");
		}
		DFBJGNMDKAL(jSONClass, text);
		return jSONClass;
	}

	private static JSONClass CreateHeadPayJSON(JDNFFHILFAF GIGAFKGDKNH, string HOKDOMALLDB, string JDHJMKDOAMO)
	{
		return CreateHeadPayJSON(AEMNGCELEDC(GIGAFKGDKNH), HOKDOMALLDB, JDHJMKDOAMO);
	}

	private static JSONClass CreateHeadPayJSON(string GIGAFKGDKNH, string HOKDOMALLDB, string JDHJMKDOAMO)
	{
		JSONClass jSONClass = new JSONClass();
		jSONClass["eid"] = StatisticsCollector.POAAIJHJFEG();
		jSONClass["etype"] = "pay";
		jSONClass["build_version"] = SystemProperties.KCJMMIEBLHL().ToString();
		jSONClass["data_version"] = SystemProperties.DFJEJKJECBI().ToString(true);
		jSONClass["subtype"] = GIGAFKGDKNH;
		jSONClass["package_id"] = ((!string.IsNullOrEmpty(HOKDOMALLDB)) ? HOKDOMALLDB : string.Empty);
		jSONClass["rid"] = ((!string.IsNullOrEmpty(JDHJMKDOAMO)) ? MD5Utils.INPENHNJBGJ(JDHJMKDOAMO) : string.Empty);
		jSONClass["paid_version"] = SystemProperties.AFAAJMFLBIC();
		if (SystemProperties.DBBOCENKMGD())
		{
			jSONClass["time"] = ListSF.IDMJOMOMDOJ().ToString("X");
		}
		return jSONClass;
	}

	private static void HPHAIPJPPHJ(JSONClass data, string EDLADAAKMDF)
	{
		JSONArray asArray = data["tags"].AsArray;
		asArray.Add(EDLADAAKMDF);
		data["tags"] = asArray;
	}

	private static void DFBJGNMDKAL(JSONClass data, string DOPHKKGNAEF)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
		}
	}

	private static string AEMNGCELEDC(JDNFFHILFAF IGABHEMGKKE)
	{
		return IGABHEMGKKE.ToString().ToLower();
	}

	private static int KBAHFOGFNDP()
	{
		return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private static string KMFDGKKFLFI()
	{
		if (SystemProperties.MEBGOGMJFLM())
		{
			return "i";
		}
		if (SystemProperties.IPJFCBAGMJJ())
		{
			return "a";
		}
		return "u";
	}

	private static string MJAMJOPLMEP(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.User);
		jSONClass["device_token"] = SystemProperties.DBKBHEMJLLC();
		jSONClass["platform"] = SystemProperties.IAAKNCJMAAK();
		jSONClass["lang"] = SystemProperties.NICPICAMAOH().OAPHJAPMKJG;
		jSONClass["os"] = SystemProperties.CFEDCPDNICD();
		jSONClass["device_name"] = SystemProperties.NICPICAMAOH().Id;
		jSONClass["level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		jSONClass["exp"] = ListSF.CCDKHLAMKKO().EOKLELGLHJJ().ToString();
		jSONClass["location"] = ListSF.CCDKHLAMKKO().NFKHNICBOIB();
		StatisticsGeter.HJPFOFKCLFC(jSONClass);
		StatisticsGeter.NJAJIJKINBL(jSONClass);
		return jSONClass.ToString();
	}

	private static string GOAECNHBDCG(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Fight_End);
		StatisticsGeter.NCECDOKNALB(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.EGHFFAOJCJG(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.MGKINBGMKKE(jSONClass);
		StatisticsGeter.IFKJOJGPEHM(jSONClass);
		StatisticsGeter.KOOBIPOFAEF(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.ACEGMLHJLEO(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.BPGIGFJGJPD(jSONClass, PCJAKPJMKGN);
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		jSONClass["device_name"] = SystemProperties.NICPICAMAOH().Id;
		StatisticsGeter.PFJFKPJDHIN(jSONClass);
		return jSONClass.ToString();
	}

	private static string MNCOAALINIE(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Achievement);
		string text = ((!PCJAKPJMKGN.ContainsKey("name")) ? string.Empty : PCJAKPJMKGN["name"].ToString());
		jSONClass["name"] = text;
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		return jSONClass.ToString();
	}

	private static string AAEBOCIOKMB(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Gems_Changed);
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		StatisticsGeter.IPJPNKDOKKE(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.NJAJIJKINBL(jSONClass);
		return jSONClass.ToString();
	}

	private static string OLHHNGGKJKI(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Level_Up);
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		StatisticsGeter.BFFEGBEDLPF(jSONClass);
		StatisticsGeter.AddFights(jSONClass, ListSF.CCDKHLAMKKO().PINDEKDNCNL() - 1);
		return jSONClass.ToString();
	}

	private static string AAMOBDLBDOB(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Purchase);
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		StatisticsGeter.BDJADPLCHDA(jSONClass, PCJAKPJMKGN);
		StatisticsGeter.NJAJIJKINBL(jSONClass);
		return jSONClass.ToString();
	}

	private static string NMHPIPKJLJG(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Payment);
		string text = PCJAKPJMKGN["item"].ToString();
		jSONClass["item"] = text;
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		jSONClass["price"] = (float)PCJAKPJMKGN["price"];
		jSONClass["price_currency"] = PCJAKPJMKGN["price_currency"].ToString();
		jSONClass["money_changed"] = (long)PCJAKPJMKGN["money_changed"];
		jSONClass["gems_paid_changed"] = (long)PCJAKPJMKGN["gems_paid_changed"];
		jSONClass["total_payment_count"] = ListSF.CCDKHLAMKKO().MNDJBCMLJHF();
		StatisticsGeter.GMNACHGHPAI(jSONClass, text);
		StatisticsGeter.NJAJIJKINBL(jSONClass);
		return jSONClass.ToString();
	}

	private static string INIPHNCIGLC(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Perk);
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		jSONClass["reset_count"] = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().ICIAGDCEMEM();
		jSONClass["level_sum"] = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().IBAOKPECDLF();
		StatisticsGeter.PFJFKPJDHIN(jSONClass, "user_perks");
		StatisticsGeter.HJECOKNPLPA(jSONClass, PCJAKPJMKGN);
		return jSONClass.ToString();
	}

	private static string IBMHOJGNMIA(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = DIFAPLOBCCL(JDNFFHILFAF.Session_End);
		jSONClass["session_id"] = StatisticsCollector.JGMAONJINDK();
		jSONClass["fight_amount"] = StatisticsCollector.MHKHJJIJKCD();
		jSONClass["time_length"] = StatisticsCollector.ODJEHEPNNHH();
		jSONClass["user_level"] = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		jSONClass["energy"] = StatisticsCollector.CGDFOCLOHEB();
		return jSONClass.ToString();
	}

	private static string JGBHBFDJMKE(ArgsDict PCJAKPJMKGN)
	{
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Request_Start, PCJAKPJMKGN["packageName"].ToString(), null);
		return jSONClass.ToString();
	}

	private static string DBFHBHIAJGD(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string text = PCJAKPJMKGN["receipt"].ToString();
		HANNAHNOAGH fFFCCEEDMKI = (HANNAHNOAGH)PCJAKPJMKGN["status"];
		int num = (int)PCJAKPJMKGN["resultCode"];
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Status_Update, hOKDOMALLDB, text);
		jSONClass["status"] = HIEFKKDHAMG(fFFCCEEDMKI);
		jSONClass["result_code"] = num;
		jSONClass["receipt"] = ((!string.IsNullOrEmpty(text)) ? text : string.Empty);
		return jSONClass.ToString();
	}

	private static string EMACNBCLAME(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string jDHJMKDOAMO = PCJAKPJMKGN["receipt"].ToString();
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Handle_Start, hOKDOMALLDB, jDHJMKDOAMO);
		return jSONClass.ToString();
	}

	private static string OEKBFPLNHDK(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string jDHJMKDOAMO = PCJAKPJMKGN["receipt"].ToString();
		long num = (long)PCJAKPJMKGN["money_changed"];
		long num2 = (long)PCJAKPJMKGN["gems_changed"];
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Handle_Finish, hOKDOMALLDB, jDHJMKDOAMO);
		jSONClass["moneyChanged"] = num;
		jSONClass["gemsChanged"] = num2;
		return jSONClass.ToString();
	}

	private static string AKMODMOANPC(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string jDHJMKDOAMO = PCJAKPJMKGN["receipt"].ToString();
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Transaction_Finish, hOKDOMALLDB, jDHJMKDOAMO);
		return jSONClass.ToString();
	}

	private static string MIDGNABOABO(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string text = PCJAKPJMKGN["reason"].ToString();
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Handle_Error, hOKDOMALLDB, null);
		jSONClass["reason"] = text;
		return jSONClass.ToString();
	}

	private static string EJIHJCDNFJL(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string text = PCJAKPJMKGN["message"].ToString();
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Handle_Debug, hOKDOMALLDB, null);
		jSONClass["message"] = text;
		return jSONClass.ToString();
	}

	private static string CLCKGBPKHPG(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		JSONClass jSONClass = CreateHeadPayJSON(JDNFFHILFAF.Pay_Request_Fail, hOKDOMALLDB, null);
		return jSONClass.ToString();
	}

	private static string CILMAEFHJNG(ArgsDict PCJAKPJMKGN)
	{
		string hOKDOMALLDB = PCJAKPJMKGN["packageName"].ToString();
		string jDHJMKDOAMO = PCJAKPJMKGN["receipt"].ToString();
		AICJEAMBGCE fFFCCEEDMKI = (AICJEAMBGCE)PCJAKPJMKGN["status"];
		string gIGAFKGDKNH = string.Format("pay_verify_{0}", HIEFKKDHAMG(fFFCCEEDMKI));
		JSONClass jSONClass = CreateHeadPayJSON(gIGAFKGDKNH, hOKDOMALLDB, jDHJMKDOAMO);
		return jSONClass.ToString();
	}

	private static string HIEFKKDHAMG(HANNAHNOAGH status)
	{
		return status.ToString().ToLower();
	}

	private static string HIEFKKDHAMG(AICJEAMBGCE status)
	{
		return status.ToString().ToLower();
	}

	private static void ADOKACKPDDH()
	{
		StatisticsCollector.JNGMOOOPCBN(0);
		StatisticsCollector.JKHLAAKKHDI(0);
	}
}
