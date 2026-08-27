using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using SimpleJSON;
using UnityEngine;

public static class StatisticsGeter
{
	private class IECDAEPLNEP
	{
		public string Name;

		public int OONPNMEPOCC;

		public int FPGNLGFKJAP;

		public int FIMCLKNJDGM;

		public int HMNNFOAPIHD;
	}

	public static void IJFOCLBKGPP(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "b_id")
	{
		MEEAKLDGLDF.Add(BFADPFOIPLL, Application.identifier);
	}

	public static void HJPFOFKCLFC(JSONClass MEEAKLDGLDF)
	{
		JSONArray jSONArray = new JSONArray();
		List<GameCurrency> list = GameUtils.AJDKHINLIDI.IIAPDCECFCN();
		foreach (GameCurrency item in list)
		{
			string mENAJEAJJBE = item.Name;
			int num = ListSF.CCDKHLAMKKO().GetCurrencyCount(mENAJEAJJBE);
			JSONClass jSONClass = new JSONClass();
			jSONClass["name"] = mENAJEAJJBE;
			jSONClass["amount"] = num;
			jSONArray.Add(jSONClass);
		}
		MEEAKLDGLDF.Add("currencies", jSONArray);
	}

	public static void IPJPNKDOKKE(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		long num = ((!PCJAKPJMKGN.ContainsKey("changed")) ? 0 : ((long)PCJAKPJMKGN["changed"]));
		string text = ((!PCJAKPJMKGN.ContainsKey("type")) ? string.Empty : PCJAKPJMKGN["type"].ToString());
		bool flag = PCJAKPJMKGN.ContainsKey("isPaid") && (bool)PCJAKPJMKGN["isPaid"];
		MEEAKLDGLDF["type"] = text;
		MEEAKLDGLDF["gems_free_changed"] = ((!flag) ? num : 0);
		MEEAKLDGLDF["gems_paid_changed"] = ((!flag) ? 0 : num);
	}

	public static void GMNACHGHPAI(JSONClass MEEAKLDGLDF, string name)
	{
		float HCHKFOJEEBK = 0f;
		GeneralConfig.IHHMHNHOLCB.LIKBNIAJHKA(name, out HCHKFOJEEBK);
		MEEAKLDGLDF["price_USD"] = HCHKFOJEEBK;
	}

	public static void NJAJIJKINBL(JSONClass MEEAKLDGLDF)
	{
		MEEAKLDGLDF["money"] = ListSF.CCDKHLAMKKO().BFBOEGMAMNF().ToString();
		MEEAKLDGLDF["gems_paid"] = (long)(ListSF.CCDKHLAMKKO().FJGHKGPAPPN());
		MEEAKLDGLDF["gems_free"] = ListSF.CCDKHLAMKKO().EHFJHFDACMP() - (long)(ListSF.CCDKHLAMKKO().FJGHKGPAPPN());
	}

	public static void BDJADPLCHDA(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		ItemInfo dJKEECEOCJB = ((!PCJAKPJMKGN.ContainsKey("item")) ? null : (PCJAKPJMKGN["item"] as ItemInfo));
		if (dJKEECEOCJB == null)
		{
			return;
		}
		StatisticsCollector.CNCDMFJLMFH cNCDMFJLMFH = (PCJAKPJMKGN.ContainsKey("type") ? ((StatisticsCollector.CNCDMFJLMFH)PCJAKPJMKGN["type"]) : StatisticsCollector.CNCDMFJLMFH.Money);
		bool flag = PCJAKPJMKGN.ContainsKey("immediatelyDelivery") && (bool)PCJAKPJMKGN["immediatelyDelivery"];
		long OMALFAGNPEE = 0L;
		long DBMJEEHOABD = 0L;
		BJINPFAPMGO(dJKEECEOCJB, cNCDMFJLMFH, flag, ref OMALFAGNPEE, ref DBMJEEHOABD);
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		if (cNCDMFJLMFH == StatisticsCollector.CNCDMFJLMFH.Money)
		{
			num = OMALFAGNPEE + DBMJEEHOABD;
		}
		else
		{
			num2 = OMALFAGNPEE;
			num3 = DBMJEEHOABD;
		}
		string text = string.Empty;
		if (dJKEECEOCJB.Type == "Energy")
		{
			text = "refill_Energy";
		}
		else if (dJKEECEOCJB.Type == "Recipe")
		{
			text = "recipe";
		}
		else
		{
			long num4 = ((!flag) ? (-1) : GameUtils.GetLeftTime(dJKEECEOCJB.EHKNIKHPGDN));
			bool flag2 = num4 > -1;
			bool aCOIHHPOBDH = dJKEECEOCJB.ACOIHHPOBDH;
			if (flag2)
			{
				text += "finish_";
			}
			text = ((!aCOIHHPOBDH) ? (text + "buy_item") : (text + "upgrade_item"));
		}
		MEEAKLDGLDF["type"] = text;
		MEEAKLDGLDF["item"] = dJKEECEOCJB.Name;
		MEEAKLDGLDF["item_type"] = dJKEECEOCJB.Type;
		MEEAKLDGLDF["upgrade_level"] = dJKEECEOCJB.OBJDGBBFJOO;
		MEEAKLDGLDF["money_changed"] = num;
		MEEAKLDGLDF["gems_free_changed"] = num2;
		MEEAKLDGLDF["gems_paid_changed"] = num3;
		MEEAKLDGLDF["upgrade"] = ((!dJKEECEOCJB.INEOECGAGGD()) ? "0" : "1");
		MEEAKLDGLDF["paid_item"] = dJKEECEOCJB.PBMHNMOHODB;
	}

	private static void BJINPFAPMGO(ItemInfo item, StatisticsCollector.CNCDMFJLMFH LFLGCDNKNJI, bool CNIOCCCBDBJ, ref long OMALFAGNPEE, ref long DBMJEEHOABD)
	{
		if (LFLGCDNKNJI == StatisticsCollector.CNCDMFJLMFH.Bonus)
		{
			DBMJEEHOABD = item.PEGDPDINDDO;
			if (CNIOCCCBDBJ)
			{
				OMALFAGNPEE = (ObscuredLong)(item.KLHOKKPALOK) - DBMJEEHOABD;
			}
			else
			{
				OMALFAGNPEE = item.MCNMMBCJADI() - DBMJEEHOABD;
			}
		}
		else
		{
			DBMJEEHOABD = item.NNLMNNAEDIE;
			if (CNIOCCCBDBJ)
			{
				OMALFAGNPEE = (ObscuredLong)(item.NDCOLFHCNLD) - DBMJEEHOABD;
			}
			else
			{
				OMALFAGNPEE = item.OHBBLIMNIMJ() - DBMJEEHOABD;
			}
		}
	}

	public static void MGKINBGMKKE(JSONClass MEEAKLDGLDF)
	{
		int num = GameUtils.CDILOOACLKK;
		if (GameUtils.LDBMFAMEMPF)
		{
			num /= GameUtils.MAEBANCIBOP;
		}
		MEEAKLDGLDF["fps_limit"] = num;
	}

	public static void IFKJOJGPEHM(JSONClass MEEAKLDGLDF)
	{
		MEEAKLDGLDF["eclipse"] = (ListSF.CCDKHLAMKKO().JPMPIDFGCJL() ? 1 : 0);
	}

	public static void KOOBIPOFAEF(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.ContainsKey("completedRounds"))
		{
			MEEAKLDGLDF["rounds"] = (int)PCJAKPJMKGN["completedRounds"];
		}
	}

	public static void ACEGMLHJLEO(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.ContainsKey("avgFps"))
		{
			MEEAKLDGLDF["fps"] = (float)PCJAKPJMKGN["avgFps"];
		}
	}

	public static void BPGIGFJGJPD(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.ContainsKey("fightTimeElapsed"))
		{
			MEEAKLDGLDF["fight_time_elapsed"] = (float)PCJAKPJMKGN["fightTimeElapsed"];
		}
	}

	public static void OGLDPCFFKEE(JSONClass MEEAKLDGLDF, FightList KGKDKENMAOA)
	{
		int num = 0;
		if (KGKDKENMAOA != null)
		{
			Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
			if (cNAOMDMIGLJ != null)
			{
				RosterBattle dDNLCGOPAGC = cNAOMDMIGLJ.NNPNEABKHPP();
				if (dDNLCGOPAGC != null)
				{
					num = dDNLCGOPAGC.ODCFKCJJDKN();
				}
			}
		}
		MEEAKLDGLDF["replay_count"] = num;
	}

	public static void EGHFFAOJCJG(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		FightResult nHIDAJFLHJN = ((!PCJAKPJMKGN.ContainsKey("fightList")) ? null : (PCJAKPJMKGN["fightResult"] as FightResult));
		if (nHIDAJFLHJN != null)
		{
			MEEAKLDGLDF["money_reward"] = nHIDAJFLHJN.KMGLLBMIDHJ();
			MEEAKLDGLDF["gems_reward"] = nHIDAJFLHJN.BNILCODHHKC();
			int num = ((!PCJAKPJMKGN.ContainsKey("isSurrender") || !(bool)PCJAKPJMKGN["isSurrender"]) ? Convert.ToInt32(nHIDAJFLHJN.IsWinner()) : (-1));
			MEEAKLDGLDF["fight_result"] = num;
			DetailedDamages mNDEOFOHLHI = nHIDAJFLHJN.AIOMDIAFHGB.MNDEOFOHLHI;
			DetailedDamages mNDEOFOHLHI2 = nHIDAJFLHJN.MOJHPBGGNAH.MNDEOFOHLHI;
			GFNHHDPOEIB(MEEAKLDGLDF, "player_damage", mNDEOFOHLHI2);
			GFNHHDPOEIB(MEEAKLDGLDF, "enemy_damage", mNDEOFOHLHI);
			ComboStatistic aIOMDIAFHGB = nHIDAJFLHJN.AIOMDIAFHGB;
			ComboStatistic mOJHPBGGNAH = nHIDAJFLHJN.MOJHPBGGNAH;
			GPIGNONFFPJ(MEEAKLDGLDF, "player", aIOMDIAFHGB);
			GPIGNONFFPJ(MEEAKLDGLDF, "enemy", mOJHPBGGNAH);
			ModelParameters kIKOGDEPGHB = ((!nHIDAJFLHJN.IsWinner()) ? nHIDAJFLHJN.LEBLJJCFKOP : nHIDAJFLHJN.ABKBEJBICOA);
			if (kIKOGDEPGHB != null)
			{
				BNHIGONAEJG(MEEAKLDGLDF, kIKOGDEPGHB);
			}
		}
	}

	public static void GPIGNONFFPJ(JSONClass MEEAKLDGLDF, string JMOHMLIGHHD, ComboStatistic AIOMDIAFHGB)
	{
		if (AIOMDIAFHGB != null)
		{
			MEEAKLDGLDF[JMOHMLIGHHD + "_perfects"] = AIOMDIAFHGB.JDKFHFOJKPI;
			MEEAKLDGLDF[JMOHMLIGHHD + "_first_strikes"] = AIOMDIAFHGB.MOLDOOIJELI;
			MEEAKLDGLDF[JMOHMLIGHHD + "_shocks"] = AIOMDIAFHGB.OGMOILIMCOM;
			MEEAKLDGLDF[JMOHMLIGHHD + "_max_combo"] = AIOMDIAFHGB.KKJHBKBMPGN;
			MEEAKLDGLDF[JMOHMLIGHHD + "_max_crazy"] = AIOMDIAFHGB.OLONAJAOFOA();
		}
	}

	public static void GFNHHDPOEIB(JSONClass MEEAKLDGLDF, string IMGCANJHPND, DetailedDamages KNKLGEAIKGE)
	{
		JSONClass jSONClass = (JSONClass)(MEEAKLDGLDF[IMGCANJHPND] = new JSONClass());
		if (KNKLGEAIKGE == null)
		{
			return;
		}
		Dictionary<string, Dictionary<string, float>> bEOLFOFKIAG = KNKLGEAIKGE.BEOLFOFKIAG;
		foreach (KeyValuePair<string, Dictionary<string, float>> item in bEOLFOFKIAG)
		{
			JSONClass jSONClass2 = new JSONClass();
			foreach (KeyValuePair<string, float> item2 in item.Value)
			{
				jSONClass2[item2.Key] = item2.Value;
			}
			jSONClass[item.Key] = jSONClass2;
		}
	}

	public static void NCECDOKNALB(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		FightList jDIPBIHBGPF = ((!PCJAKPJMKGN.ContainsKey("fightList")) ? null : (PCJAKPJMKGN["fightList"] as FightList));
		if (jDIPBIHBGPF != null)
		{
			MEEAKLDGLDF["zone"] = jDIPBIHBGPF.BCKFACGMOKC.PELHCAEAOFE();
			MEEAKLDGLDF["fight_name"] = jDIPBIHBGPF.BCKFACGMOKC.CPHDPCAECJN();
			MEEAKLDGLDF["fight_type"] = ListSF.ELEBLBJKDBI().ADHNLNFEOKN(jDIPBIHBGPF.get_Type());
			MEEAKLDGLDF["stage_number"] = jDIPBIHBGPF.BCKFACGMOKC.EJPNIFANKDG();
			MEEAKLDGLDF["difficulty"] = GameUtils.JEILJMPPEGL(jDIPBIHBGPF);
			OGLDPCFFKEE(MEEAKLDGLDF, jDIPBIHBGPF);
		}
	}

	public static void AddFights(JSONClass MEEAKLDGLDF, int GNLOCMLBNHF)
	{
		Dictionary<string, IECDAEPLNEP> dictionary = new Dictionary<string, IECDAEPLNEP>();
		List<RosterFight> list = ListSF.CCDKHLAMKKO().FHDLNKAAAOK(GNLOCMLBNHF);
		foreach (RosterFight item in list)
		{
			string text = item.GIDNOKCJLPL();
			if (dictionary.ContainsKey(text))
			{
				IECDAEPLNEP iECDAEPLNEP = dictionary[text];
				iECDAEPLNEP.OONPNMEPOCC += item.JAJNIKDMPPO();
				iECDAEPLNEP.FPGNLGFKJAP += item.HCMBHIGGMDF();
				iECDAEPLNEP.FIMCLKNJDGM += item.PEHLNNEFFLI();
				iECDAEPLNEP.HMNNFOAPIHD += item.PHKCBMAOHIF();
			}
			else
			{
				IECDAEPLNEP iECDAEPLNEP2 = new IECDAEPLNEP();
				iECDAEPLNEP2.Name = text;
				iECDAEPLNEP2.OONPNMEPOCC = item.JAJNIKDMPPO();
				iECDAEPLNEP2.FPGNLGFKJAP = item.HCMBHIGGMDF();
				iECDAEPLNEP2.FIMCLKNJDGM = item.PEHLNNEFFLI();
				iECDAEPLNEP2.HMNNFOAPIHD = item.PHKCBMAOHIF();
				IECDAEPLNEP value = iECDAEPLNEP2;
				dictionary[text] = value;
			}
		}
		foreach (KeyValuePair<string, IECDAEPLNEP> item2 in dictionary)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["fight_name"] = item2.Value.Name;
			jSONClass["win_count"] = item2.Value.OONPNMEPOCC;
			jSONClass["loss_count"] = item2.Value.FPGNLGFKJAP;
			jSONClass["eclipse_win_count"] = item2.Value.FIMCLKNJDGM;
			jSONClass["eclipse_loss_count"] = item2.Value.HMNNFOAPIHD;
			MEEAKLDGLDF["fights"] = jSONClass;
		}
	}

	public static void PFJFKPJDHIN(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "perks")
	{
		JSONArray jSONArray = new JSONArray();
		List<RosterPerk> list = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().KEHFPLBNDHI();
		foreach (RosterPerk item in list)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass["perk"] = item.get_Name();
			jSONClass["upgrade_level"] = item.DHNNCAEEMLL();
			jSONArray.Add(jSONClass);
		}
		MEEAKLDGLDF[BFADPFOIPLL] = jSONArray;
	}

	public static void HJECOKNPLPA(JSONClass MEEAKLDGLDF, ArgsDict PCJAKPJMKGN)
	{
		if (PCJAKPJMKGN.ContainsKey("learnedPerk"))
		{
			PerkInfoItem aCONCDFDNJH = (PerkInfoItem)PCJAKPJMKGN["learnedPerk"];
			MEEAKLDGLDF["taken"] = aCONCDFDNJH.Name;
			MEEAKLDGLDF["taken_upgrade_level"] = aCONCDFDNJH.AKKLOMFOLNO;
		}
		if (PCJAKPJMKGN.ContainsKey("rejectedPerk"))
		{
			PerkInfoItem aCONCDFDNJH2 = (PerkInfoItem)PCJAKPJMKGN["rejectedPerk"];
			MEEAKLDGLDF["rejected"] = aCONCDFDNJH2.Name;
			MEEAKLDGLDF["rejected_upgrade_level"] = aCONCDFDNJH2.AKKLOMFOLNO;
		}
	}

	public static void IOHNPPCBIFL(ItemInfo item, JSONClass MEEAKLDGLDF, string IMGCANJHPND)
	{
		JSONArray jSONArray = null;
		List<PerkInfoItem> list = ListSF.EIMKEJNJMEJ(item);
		if (list.Count > 0)
		{
			jSONArray = new JSONArray();
			foreach (PerkInfoItem perkInfo in list)
			{
				JSONClass jSONClass = new JSONClass();
				jSONClass["perk"] = perkInfo.Name;
				jSONClass["aspect"] = perkInfo.NGNJGOJJPLD("Aspect");
				jSONArray.Add(jSONClass);
			}
		}
		if (jSONArray != null)
		{
			MEEAKLDGLDF[IMGCANJHPND] = jSONArray;
		}
	}

	public static void FHKAFKBFJLI(JSONClass MEEAKLDGLDF, string JMOHMLIGHHD, ItemInfo item)
	{
		if (item != null)
		{
			MEEAKLDGLDF[JMOHMLIGHHD + "_name"] = item.Name;
			MEEAKLDGLDF[JMOHMLIGHHD + "_upg_level"] = item.OBJDGBBFJOO;
			IOHNPPCBIFL(item, MEEAKLDGLDF, JMOHMLIGHHD + "_enchantments");
		}
	}

	public static void BNHIGONAEJG(JSONClass MEEAKLDGLDF, ModelParameters JCICKLIMBEF)
	{
		if (JCICKLIMBEF != null)
		{
			FHKAFKBFJLI(MEEAKLDGLDF, "weapon", JCICKLIMBEF.JGMLKIPCFII);
			FHKAFKBFJLI(MEEAKLDGLDF, "armor", JCICKLIMBEF.LKKFNMBCCDB);
			FHKAFKBFJLI(MEEAKLDGLDF, "helmet", JCICKLIMBEF.FKMOLBBLKDA);
			FHKAFKBFJLI(MEEAKLDGLDF, "ranged", JCICKLIMBEF.LGHMILECPLA);
			FHKAFKBFJLI(MEEAKLDGLDF, "magic", JCICKLIMBEF.ADBKGIBBNHJ);
		}
	}

	public static void CDLBBCPOPAL(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "rank")
	{
	}

	public static void LJFCIKFBDHA(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "ranks")
	{
		JSONArray aItem = new JSONArray();
		MEEAKLDGLDF.Add(BFADPFOIPLL, aItem);
	}

	public static void PKLKDFIPCAG(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "energy")
	{
	}

	public static void CLFIGOGBMCP(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "runs")
	{
	}

	public static void PPLGCOOMBIA(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "seed")
	{
	}

	public static void BIOFFBEPDEO(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "floor")
	{
	}

	public static void CJLPNFIODNF(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "time")
	{
	}

	public static void EMCBGFOBEMA(JSONClass MEEAKLDGLDF)
	{
	}

	public static void PMANGDENICN(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "prev_floor_time")
	{
	}

	public static void LCBNLAIDCNA(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "scene")
	{
	}

	public static void JIALMFBGDII(JSONClass MEEAKLDGLDF, string BFADPFOIPLL = "screen")
	{
	}

	public static void BFFEGBEDLPF(JSONClass MEEAKLDGLDF)
	{
		List<UserItem> list = ListSF.CCDKHLAMKKO().KHCNHPCPFII().DJBOFEEKJMP();
		foreach (UserItem item in list)
		{
			int num = NNFFKNCIHHK(item);
			if (num == ListSF.CCDKHLAMKKO().PINDEKDNCNL() - 1)
			{
				JSONClass jSONClass = null;
				jSONClass = ((!MEEAKLDGLDF.HasValue("items")) ? new JSONClass() : ((JSONClass)MEEAKLDGLDF["items"]));
				LMCHCDJEDID(jSONClass, item);
				MEEAKLDGLDF["items"] = jSONClass;
			}
		}
	}

	public static void LMCHCDJEDID(JSONClass MEEAKLDGLDF, UserItem item)
	{
		MEEAKLDGLDF["name"] = item.get_Name();
		MEEAKLDGLDF["upgrade_level"] = item.DHNNCAEEMLL();
		MEEAKLDGLDF["type"] = item.BHKHOJPANHE().Type;
		IOHNPPCBIFL(item.BHKHOJPANHE(), MEEAKLDGLDF, "enchantments");
	}

	private static int NNFFKNCIHHK(UserItem item)
	{
		ItemInfo dJKEECEOCJB = item.AKKBIFEFDCI();
		if (dJKEECEOCJB == null)
		{
			dJKEECEOCJB = item.BHKHOJPANHE();
		}
		if (dJKEECEOCJB != null)
		{
			return dJKEECEOCJB.MHGODOLNDLE;
		}
		LLLOJBFMONN.Error("logging UserItem without ItemInfo");
		return 0;
	}
}
