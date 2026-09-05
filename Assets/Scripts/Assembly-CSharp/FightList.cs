using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class FightList
{
	public FightIDS BCKFACGMOKC = new FightIDS();

	protected BattleType _type;

	protected RosterFight ECHMHCODAFA;

	protected int DNKLOKMPMPL;

	protected string _description = string.Empty;

	public string JKMJHIIMHPG = string.Empty;

	public Battle CNAOMDMIGLJ;

	public string Name = string.Empty;

	public int Index = -1;

	public int EJGGHHEOGPG;

	public long RepeatTime;

	public bool ANIFGJGHNLN;

	private bool JPNHGEBCPAF;

	private bool AMCPPIOLKGC;

	public ConditionStatus PGBKNLAEANJ;

	public bool CNNCIENODGE;

	public int BDBBNECNMBP;

	public ObscuredInt RoundTime;

	public int JABJLCEJDDM;

	public ObscuredLong LDHOBIADNEC;

	public ObscuredLong JBNAJPPNGFB;

	public ObscuredUInt PrizeExp;

	public string PPCNJPCPGGP = string.Empty;

	public float MOPEDKMDLFA;

	public float FANGNMDAINE;

	public float OMFDJPFGKAB;

	public string NPPIFKKLNCN = string.Empty;

	private List<ModelParameters> KCIGNIAJLBM = new List<ModelParameters>();

	private List<ConditionFight> _conditions = new List<ConditionFight>();

	private List<RewardStruct> BIKCEGKOBME = new List<RewardStruct>();

	private List<ItemRule> _itemRules = new List<ItemRule>();

	private List<RandomRule> _randomRules = new List<RandomRule>();

	private DescriptionRule DGIJOJONCFO;

	private List<Rule> _rules = new List<Rule>();

	private List<Rule> IACOELKGMAA = new List<Rule>();

	private List<Rule> BLJOKCLPFGN = new List<Rule>();

	private List<Rule> FKDOPPMODKH = new List<Rule>();

	public ushort ANHLAHFDDCE;

	public ushort LPMDOHPIEOP;

	public string JKCHHOMGGBN = string.Empty;

	public RosterFight MIENCCOKHCP
	{
		get
		{
			return FLKFFDLLBKA();
		}
		set
		{
			HOCFLEMFFKC(value);
		}
	}

	public int PPLPFDOODPA
	{
		get
		{
			return BFMMAFJFABG();
		}
		set
		{
			set_PowerRequired(value);
		}
	}

	public string MGNNJPBCOGD
	{
		get
		{
			return GJOAJAIJHOE();
		}
		set
		{
			set_Description(value);
		}
	}

	public bool LMLBLFINMMC
	{
		get
		{
			return DBAEHGILOCO();
		}
		set
		{
			set_IsInFight(value);
		}
	}

	public List<ModelParameters> KGNAIGHPJMA
	{
		get
		{
			return OFKJMHPMCCD();
		}
	}

	public List<ConditionFight> JIFAHHGNPFH
	{
		get
		{
			return KJILOMLMMEN();
		}
	}

	public List<RewardStruct> MNKBCDFHGJD
	{
		get
		{
			return APKPCGDBMEP();
		}
	}

	public List<ItemRule> GPFPHKNJNNF
	{
		get
		{
			return EHGIKANKJNJ();
		}
	}

	public List<RandomRule> MKCOEBKJGOF
	{
		get
		{
			return CENNLFIPNLH();
		}
	}

	public BattleType get_Type()
	{
		return _type;
	}

	public void set_Type(BattleType value)
	{
		_type = value;
	}

	public RosterFight FLKFFDLLBKA()
	{
		return ECHMHCODAFA;
	}

	public void HOCFLEMFFKC(RosterFight value)
	{
		ECHMHCODAFA = value;
		ECHMHCODAFA.GAHNGDBKFNO = this;
	}

	public int BFMMAFJFABG()
	{
		return DNKLOKMPMPL;
	}

	public void set_PowerRequired(int value)
	{
		DNKLOKMPMPL = value;
	}

	public string GJOAJAIJHOE()
	{
		if (DGIJOJONCFO == null)
		{
			return _description;
		}
		return DGIJOJONCFO.MIDPFGENBCF();
	}

	public void set_Description(string value)
	{
		_description = value;
	}

	public bool DBAEHGILOCO()
	{
		return JPNHGEBCPAF;
	}

	public void set_IsInFight(bool value)
	{
		JPNHGEBCPAF = value;
	}

	public List<ModelParameters> OFKJMHPMCCD()
	{
		return KCIGNIAJLBM;
	}

	public List<ConditionFight> KJILOMLMMEN()
	{
		return _conditions;
	}

	public List<RewardStruct> APKPCGDBMEP()
	{
		return BIKCEGKOBME;
	}

	public List<ItemRule> EHGIKANKJNJ()
	{
		return _itemRules;
	}

	public List<RandomRule> CENNLFIPNLH()
	{
		return _randomRules;
	}

	public void RandomizeObscuredVars()
	{
		RoundTime.GMCADPGOCHM();
		LDHOBIADNEC.GMCADPGOCHM();
		JBNAJPPNGFB.GMCADPGOCHM();
		PrizeExp.GMCADPGOCHM();
		APKPCGDBMEP().ForEach((RewardStruct DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
		OFKJMHPMCCD().ForEach((ModelParameters DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
	}

	public long HHJHCLBCEEA()
	{
		long num = RepeatTime - ECHMHCODAFA.CCCIFDLEMPI();
		if (num < 0)
		{
			num = 0L;
		}
		return num;
	}

	public string GetRuleItemName(string LMNNBBKHMEI)
	{
		foreach (ItemRule item in _itemRules)
		{
			if (item.CHDEIEMINPF() && !item.KIGLIADCMHK())
			{
				UserItem dKCHDHMLKHN = item.get_Item();
				if (dKCHDHMLKHN.BHKHOJPANHE().Type == LMNNBBKHMEI)
				{
					return dKCHDHMLKHN.get_Name();
				}
			}
		}
		return string.Empty;
	}

	public int EENGGKCGLEB(string LMNNBBKHMEI)
	{
		foreach (ItemRule item in _itemRules)
		{
			if (item.CHDEIEMINPF() && !item.KIGLIADCMHK())
			{
				UserItem dKCHDHMLKHN = item.get_Item();
				if (dKCHDHMLKHN.BHKHOJPANHE().Type == LMNNBBKHMEI)
				{
					return dKCHDHMLKHN.DHNNCAEEMLL();
				}
			}
		}
		return 0;
	}

	public bool PCEPDPMOPKC()
	{
		List<CurrencyCostRule> list = LBGNOMEFLBA();
		foreach (CurrencyCostRule item in list)
		{
			if (item.JFDCHNBPPNH() != string.Empty && item.LHNHLANLHMN() > 0)
			{
				return true;
			}
		}
		return false;
	}

	public int BCGHJHJBCME(string currencyName)
	{
		int num = 0;
		List<CurrencyCostRule> list = LBGNOMEFLBA();
		foreach (CurrencyCostRule item in list)
		{
			if (item.JFDCHNBPPNH() == currencyName)
			{
				num += item.LHNHLANLHMN();
			}
		}
		return num;
	}

	public void KMPACCIOOLE(ModelParameters IHEFAMAFBIA, bool FFBFPLODJME, int round = 0)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		List<ItemRule> list = EHGIKANKJNJ();
		foreach (ItemRule item in list)
		{
			if (round > 0 && !item.HAKHBAOJBON(round) && !item.CHDEIEMINPF())
			{
				continue;
			}
			UserItem dKCHDHMLKHN = item.get_Item();
			ItemInfo dJKEECEOCJB = dKCHDHMLKHN.BHKHOJPANHE();
			string text = dKCHDHMLKHN.get_Name();
			if (text == string.Empty)
			{
				LLLOJBFMONN.Error("name for item is empty");
			}
			UserItem dKCHDHMLKHN2 = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(text);
			dJKEECEOCJB = null;
			if (dKCHDHMLKHN2 == null)
			{
				dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(text);
				if (dJKEECEOCJB == null)
				{
					LLLOJBFMONN.Error(" Model::equipRulesItems - item not found \"%s\"", text);
					continue;
				}
			}
			else
			{
				dJKEECEOCJB = dKCHDHMLKHN2.BHKHOJPANHE();
			}
			if ((dJKEECEOCJB == null || !nKGLHEGIKKP.KHCNHPCPFII().MHMFKLLIFEJ(dJKEECEOCJB)) && !item.KIGLIADCMHK())
			{
				dJKEECEOCJB = null;
			}
			if (dJKEECEOCJB != null && (!FFBFPLODJME || !item.DCFMEDKNIDI()))
			{
				ItemInfo dJKEECEOCJB2 = dJKEECEOCJB.Clone();
				dJKEECEOCJB2.GNDLEFFMJDJ = true;
				IHEFAMAFBIA.OLLNIKFPMKE(dJKEECEOCJB.Type, dJKEECEOCJB2);
			}
		}
	}

	public RewardStruct OOOBLJIHBEP(int index)
	{
		if (APKPCGDBMEP().Count > index)
		{
			return APKPCGDBMEP()[index];
		}
		LLLOJBFMONN.Write("FightList::getReward - wrong index: " + index + " from " + APKPCGDBMEP().Count + " (we need this error?)");
		return null;
	}

	public bool ECEFCOJPBPG()
	{
		return ECHMHCODAFA == null || ECHMHCODAFA.GHCHJIBBBOK(RepeatTime);
	}

	public void OJJLHLPLFKC(RewardStruct LGDIIADDFLH)
	{
		BIKCEGKOBME.Add(LGDIIADDFLH);
	}

	public List<Rule> BONNMLEJBJH()
	{
		return (!ListSF.CCDKHLAMKKO().JPMPIDFGCJL()) ? _rules : IACOELKGMAA;
	}

	public bool MeetsPlayerItemRequirements(ModelParameters parameters)
	{
		if (_type != BattleType.FightChallenge)
		{
			return true;
		}
		foreach (Rule rule in BONNMLEJBJH())
		{
			ItemRule itemRule = rule as ItemRule;
			if (itemRule == null || !itemRule.IsEntryRequirement() || !rule.CHDEIEMINPF())
			{
				continue;
			}
			RuleAppliance appliance = itemRule.EDAKADCHOLE();
			if ((appliance == RuleAppliance.AppliancePlayer || appliance == RuleAppliance.ApplianceAll) &&
				!itemRule.IsSatisfiedBy(parameters))
			{
				return false;
			}
		}
		return true;
	}

	public void FGPICFIPAGG()
	{
		IACOELKGMAA = _rules;
	}

	public void SetTime(long time)
	{
		RosterFight pIGKOIFBOME = FLKFFDLLBKA();
		if (pIGKOIFBOME != null)
		{
			if (_type == BattleType.FightPeriodic)
			{
				pIGKOIFBOME.CLCBNOCDIPF(time);
				CNIIKMBPIDG();
			}
			pIGKOIFBOME.ABIELBGOLCA(time);
		}
	}

	public int PNHLGCBPFIG()
	{
		int num = 0;
		foreach (ModelParameters item in KCIGNIAJLBM)
		{
			num += item.PEBKEBIBAFA;
		}
		return num;
	}

	public float LGGEHKEJJHO(ModelParameters ACENLMONNPA, ModelParameters HFGPAELCNMF)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		RatingEvaluationRule hIFCIAHLLAE = NJCFKELLCCB();
		if (hIFCIAHLLAE != null)
		{
			num = hIFCIAHLLAE.JLDBFIKOALE();
			num2 = hIFCIAHLLAE.IJCNLEOEFAG();
			num3 = hIFCIAHLLAE.FIOPALJIOEC();
		}
		EquippedItemsStruct hELFDCAIJNE = new EquippedItemsStruct();
		EquippedItemsStruct hELFDCAIJNE2 = new EquippedItemsStruct();
		ACENLMONNPA.ALBOCOGOBCN(hELFDCAIJNE);
		HFGPAELCNMF.ALBOCOGOBCN(hELFDCAIJNE2);
		DEIOEPHPBGO(ACENLMONNPA);
		DEIOEPHPBGO(HFGPAELCNMF);
		List<global::Pair<string, float>> list = NGEICMPHDBG();
		List<global::Pair<string, float>> list2 = DNACAMPFMKN();
		if (num == 0f)
		{
			num = HFGPAELCNMF.PMHIIOJPDLO();
		}
		if (num < 0f)
		{
			num = ACENLMONNPA.DJOIGHCCMJG(HFGPAELCNMF, list);
		}
		if (num2 == 0f)
		{
			num2 = HFGPAELCNMF.CEKIBEJELBM();
		}
		if (num2 < 0f)
		{
			num2 = HFGPAELCNMF.DJOIGHCCMJG(ACENLMONNPA, list2);
		}
		float num4 = GameUtils.MGPIOCMLCLF();
		string kGBGENDIMBC = GameUtils.CJMOJMKCLMJ();
		int OEMALIFPGPO = 0;
		int OEMALIFPGPO2 = 0;
		ModelParameters kIKOGDEPGHB = HFGPAELCNMF.Clone();
		ModelParameters kIKOGDEPGHB2 = ACENLMONNPA.Clone();
		kIKOGDEPGHB.GPOIKJNPDIO(list2);
		kIKOGDEPGHB2.GPOIKJNPDIO(list);
		kIKOGDEPGHB.IBLHIAHECLK.Get(kGBGENDIMBC, ref OEMALIFPGPO);
		kIKOGDEPGHB2.IBLHIAHECLK.Get(kGBGENDIMBC, ref OEMALIFPGPO2);
		float num5 = 1f;
		float num6 = 1f;
		List<Rule> list3 = BONNMLEJBJH();
		List<InFightRule> list4 = new List<InFightRule>();
		foreach (Rule item in list3)
		{
			if (item.get_Type() == Rule.BCBLLMPAMLP.RuleResistance)
			{
				InFightRule aAJIFBJLJOA = item as InFightRule;
				if (aAJIFBJLJOA != null)
				{
					list4.Add(aAJIFBJLJOA);
				}
			}
		}
		foreach (InFightRule item2 in list4)
		{
			ResistanceRule hCOHJNFLKIF = item2 as ResistanceRule;
			if (hCOHJNFLKIF != null)
			{
				string gOHIIMFFFJI = hCOHJNFLKIF.DJBFLJAIKLI();
				int num7 = hCOHJNFLKIF.GLBEGDFMDBO();
				int num8 = ListSF.CCDKHLAMKKO().IJCGBPDAAJF(gOHIIMFFFJI);
				if (num8 < num7)
				{
					float num9 = Mathf.Pow(2f, (float)(num7 - num8) / GameUtils.CHOGPMPEDIC());
					float num10 = Mathf.Pow(2f, (float)(num8 - num7) / GameUtils.CHOGPMPEDIC());
					num6 *= num9;
					num5 *= num10;
				}
			}
		}
		float num11 = num2 / num * Mathf.Pow(2f, (float)(OEMALIFPGPO - OEMALIFPGPO2) * num4) * num6 / num5;
		float num12 = (float)HFGPAELCNMF.ALCFNGIKCCB + num3;
		num11 *= Mathf.Pow(2f, 2f * num12 / GameUtils.BGJPLNFFEOB);
		ACENLMONNPA.ALGDEEKFPKK(hELFDCAIJNE);
		HFGPAELCNMF.ALGDEEKFPKK(hELFDCAIJNE2);
		return num11;
	}

	public float MPNBGBIMEIP(ModelParameters ACENLMONNPA, List<ModelParameters> IDAAONBIBJM)
	{
		float result = 0f;
		int count = IDAAONBIBJM.Count;
		if (0 < count)
		{
			ModelParameters hFGPAELCNMF = IDAAONBIBJM[count - 1];
			result = LGGEHKEJJHO(ACENLMONNPA, hFGPAELCNMF);
		}
		else
		{
			LLLOJBFMONN.Error("enemy less than 1");
		}
		return result;
	}

	public bool CBJOENICLAF()
	{
		return OFKJMHPMCCD().Count > 1 && BDBBNECNMBP > 1;
	}

	public void PutRule(Rule HNBFMAKFJAM)
	{
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleItem:
		case Rule.BCBLLMPAMLP.RuleEquipItem:
		case Rule.BCBLLMPAMLP.RuleRandomAquiredItem:
			_itemRules.AddIfNotExist((ItemRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleRandom:
			_randomRules.AddIfNotExist((RandomRule)HNBFMAKFJAM);
			break;
		case Rule.BCBLLMPAMLP.RuleComplex:
			GEIHPJNNDGG((ComplexRule)HNBFMAKFJAM);
			break;
		default:
			LLLOJBFMONN.Error("FightList::putRule ERROR - wrong rule type %i. Rule not added to fight.", HNBFMAKFJAM.get_Type());
			return;
		case Rule.BCBLLMPAMLP.RuleNoButton:
		case Rule.BCBLLMPAMLP.RuleNoAnimation:
		case Rule.BCBLLMPAMLP.RuleRingout:
		case Rule.BCBLLMPAMLP.RuleDarkness:
		case Rule.BCBLLMPAMLP.RuleHotGround:
		case Rule.BCBLLMPAMLP.RuleLoseFall:
		case Rule.BCBLLMPAMLP.RuleRegeneration:
		case Rule.BCBLLMPAMLP.RuleAttributes:
		case Rule.BCBLLMPAMLP.RuleDamageFactor:
		case Rule.BCBLLMPAMLP.RuleRemoveInterval:
		case Rule.BCBLLMPAMLP.RuleCrazy:
		case Rule.BCBLLMPAMLP.RuleLifeSteal:
		case Rule.BCBLLMPAMLP.RuleNoHealthBar:
		case Rule.BCBLLMPAMLP.RuleCombo:
		case Rule.BCBLLMPAMLP.RuleTimeoutWin:
		case Rule.BCBLLMPAMLP.RulePoints:
		case Rule.BCBLLMPAMLP.RuleRechargeMagicEachRound:
		case Rule.BCBLLMPAMLP.RuleNoBulletsReplenishment:
		case Rule.BCBLLMPAMLP.RuleDescription:
		case Rule.BCBLLMPAMLP.RulePerk:
		case Rule.BCBLLMPAMLP.RuleNoPerks:
		case Rule.BCBLLMPAMLP.RuleWinStyle:
		case Rule.BCBLLMPAMLP.RuleWinCombo:
		case Rule.BCBLLMPAMLP.RuleWinShock:
		case Rule.BCBLLMPAMLP.RuleChangeFight:
		case Rule.BCBLLMPAMLP.RuleTactic:
		case Rule.BCBLLMPAMLP.RuleInvertJoystick:
		case Rule.BCBLLMPAMLP.RuleRandomArea:
		case Rule.BCBLLMPAMLP.RuleRatingEvaluation:
		case Rule.BCBLLMPAMLP.RuleInvulnerability:
		case Rule.BCBLLMPAMLP.RuleCurrencyCost:
		case Rule.BCBLLMPAMLP.RuleResistance:
		case Rule.BCBLLMPAMLP.RuleRaidCurrencyCost:
		case Rule.BCBLLMPAMLP.RuleAvatar:
		case Rule.BCBLLMPAMLP.RuleName:
			break;
		}
		switch (HNBFMAKFJAM.PGOPBNMFAAG)
		{
		case Rule.DIMPPDKCBLE.MODE_ECLIPSE:
			IACOELKGMAA.AddIfNotExist(HNBFMAKFJAM);
			break;
		case Rule.DIMPPDKCBLE.MODE_NORMAL:
			_rules.AddIfNotExist(HNBFMAKFJAM);
			break;
		default:
			IACOELKGMAA.AddIfNotExist(HNBFMAKFJAM);
			_rules.AddIfNotExist(HNBFMAKFJAM);
			break;
		}
		FKDOPPMODKH.AddIfNotExist(HNBFMAKFJAM);
	}

	public void KMLFBLCMMDO(ModelParameters AIIALIFJJMB)
	{
		KCIGNIAJLBM.AddIfNotExist(AIIALIFJJMB);
	}

	public void KNIBCMJBDBP(Rule HNBFMAKFJAM)
	{
		BLJOKCLPFGN.AddIfNotExist(HNBFMAKFJAM);
		PutRule(HNBFMAKFJAM);
	}

	public void CNIIKMBPIDG()
	{
		if (ECHMHCODAFA.AANKNHJKJII(RepeatTime) && !ECEFCOJPBPG() && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_DUEL_UNLOCKED))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}

	public void ResetRandomRules()
	{
		if (_type != BattleType.FightPeriodic || _randomRules.Count == 0)
		{
			return;
		}
		if (JPNHGEBCPAF)
		{
			AMCPPIOLKGC = true;
			return;
		}
		NekkiMath.KACCBCCEPGB(ECHMHCODAFA.BKDOAOCGJLJ());
		foreach (RandomRule item in _randomRules)
		{
			if (item.EPMBMBMNJIA() == RandomRule.EOAOMBKFMPF.REFRESH_EACH_FIGHT)
			{
				item.OIOJKNKDFJM();
			}
		}
		NekkiMath.KACCBCCEPGB();
		GJFPAFPEPLK();
		AMCPPIOLKGC = false;
	}

	public void JENGHOJIOFK()
	{
		if (AMCPPIOLKGC)
		{
			ResetRandomRules();
		}
	}

	public void UpdateLevel(int PPGFCLBFLEK)
	{
		int count = BIKCEGKOBME.Count;
		if (count > 0)
		{
			RewardStruct fDFKLPHBAHJ = BIKCEGKOBME[count - 1];
			RewardPrize cMHHEHILIIH = fDFKLPHBAHJ.KOBOIFJNPMO(PPGFCLBFLEK);
			PrizeExp = cMHHEHILIIH.exp;
			LDHOBIADNEC = cMHHEHILIIH.GBGNFPNCGED;
			JBNAJPPNGFB = cMHHEHILIIH.PNDAIFALIKF;
		}
		else
		{
			PrizeExp = (ObscuredUInt)(0u);
			LDHOBIADNEC = (ObscuredLong)(0L);
			JBNAJPPNGFB = (ObscuredLong)(0L);
		}
	}

	public void IJCMEOONKND()
	{
		foreach (Rule item in BLJOKCLPFGN)
		{
			FKDOPPMODKH.Remove(item);
			switch (item.get_Type())
			{
			case Rule.BCBLLMPAMLP.RuleItem:
			case Rule.BCBLLMPAMLP.RuleEquipItem:
			case Rule.BCBLLMPAMLP.RuleRandomAquiredItem:
				_itemRules.Remove((ItemRule)item);
				break;
			case Rule.BCBLLMPAMLP.RuleRandom:
				_randomRules.Remove((RandomRule)item);
				break;
			}
			_rules.Remove(item);
			IACOELKGMAA.Remove(item);
		}
		BLJOKCLPFGN.Clear();
	}

	public void GJFPAFPEPLK()
	{
		DGIJOJONCFO = IOMIAAJBPAA();
		AEHLMKODMBJ();
	}

	public virtual List<CurrencyCostRule> LBGNOMEFLBA()
	{
		List<CurrencyCostRule> list = new List<CurrencyCostRule>();
		List<Rule> list2 = BONNMLEJBJH();
		foreach (Rule item2 in list2)
		{
			if (item2.get_Type() == Rule.BCBLLMPAMLP.RuleCurrencyCost)
			{
				CurrencyCostRule item = (CurrencyCostRule)item2;
				list.Add(item);
			}
		}
		return list;
	}

	private DescriptionRule IOMIAAJBPAA(Rule HNBFMAKFJAM)
	{
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleDescription:
			return (DescriptionRule)HNBFMAKFJAM;
		case Rule.BCBLLMPAMLP.RuleRandom:
			return IOMIAAJBPAA(((RandomRule)HNBFMAKFJAM).GHLEKCGJAEP());
		case Rule.BCBLLMPAMLP.RuleComplex:
		{
			DescriptionRule result = null;
			List<Rule> list = ((ComplexRule)HNBFMAKFJAM).BONNMLEJBJH();
			{
				foreach (Rule item in list)
				{
					DescriptionRule gNBDNDOBLDO = IOMIAAJBPAA(item);
					if (gNBDNDOBLDO != null)
					{
						result = gNBDNDOBLDO;
					}
				}
				return result;
			}
		}
		default:
			return null;
		}
	}

	private DescriptionRule IOMIAAJBPAA()
	{
		DescriptionRule result = null;
		List<Rule> list = BONNMLEJBJH();
		foreach (Rule item in list)
		{
			if (item.CHDEIEMINPF())
			{
				DescriptionRule gNBDNDOBLDO = IOMIAAJBPAA(item);
				if (gNBDNDOBLDO != null)
				{
					result = gNBDNDOBLDO;
				}
			}
		}
		return result;
	}

	private void GEIHPJNNDGG(ComplexRule FPMPFCGEBKE)
	{
		foreach (Rule item in FPMPFCGEBKE.BONNMLEJBJH())
		{
			if (item.get_Type() == Rule.BCBLLMPAMLP.RuleRandom)
			{
				_randomRules.AddIfNotExist((RandomRule)item);
			}
			if (item.get_Type() == Rule.BCBLLMPAMLP.RuleComplex)
			{
				GEIHPJNNDGG((ComplexRule)item);
			}
		}
	}

	private void AEHLMKODMBJ()
	{
		ListSF.ELEBLBJKDBI().KBCBLOMDKCA(this);
	}

	private RatingEvaluationRule NJCFKELLCCB()
	{
		List<Rule> list = BONNMLEJBJH();
		foreach (Rule item in list)
		{
			if (item.get_Type() == Rule.BCBLLMPAMLP.RuleRatingEvaluation)
			{
				return (RatingEvaluationRule)item;
			}
		}
		return null;
	}

	private List<global::Pair<string, float>> NGEICMPHDBG()
	{
		return EHFAACGDJEP(RuleAppliance.AppliancePlayer);
	}

	private List<global::Pair<string, float>> DNACAMPFMKN()
	{
		return EHFAACGDJEP(RuleAppliance.ApplianceOpponent);
	}

	private List<global::Pair<string, float>> EHFAACGDJEP(RuleAppliance IGFNCCEHFEK)
	{
		List<global::Pair<string, float>> list = new List<global::Pair<string, float>>();
		List<Rule> list2 = BONNMLEJBJH();
		foreach (Rule item in list2)
		{
			if (item.get_Type() != Rule.BCBLLMPAMLP.RuleAttributes)
			{
				continue;
			}
			AttributesRule bGIGBBHDIDB = (AttributesRule)item;
			Dictionary<string, float> dictionary = bGIGBBHDIDB.MAKMDLMJNPO();
			foreach (KeyValuePair<string, float> item2 in dictionary)
			{
				if (bGIGBBHDIDB.EDAKADCHOLE() == IGFNCCEHFEK || bGIGBBHDIDB.EDAKADCHOLE() == RuleAppliance.ApplianceAll)
				{
					if (!item2.Key.Contains("Defense"))
					{
						list.Add(new global::Pair<string, float>(item2.Key, item2.Value));
					}
				}
				else if (item2.Key.Contains("Defense"))
				{
					list.Add(new global::Pair<string, float>(item2.Key, item2.Value));
				}
			}
		}
		return list;
	}

	private void DEIOEPHPBGO(ModelParameters IHEFAMAFBIA)
	{
		RuleAppliance iGFNCCEHFEK = (IHEFAMAFBIA.IsPlayer ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		List<ItemRule> list = NEFPHIJEMLM(iGFNCCEHFEK);
		foreach (RandomRule item in _randomRules)
		{
			Rule gKAJMMNJBGA = item.GHLEKCGJAEP();
			if (gKAJMMNJBGA != null)
			{
				OGMJKBDKLMP(gKAJMMNJBGA, list);
			}
		}
		IHEFAMAFBIA.KMPACCIOOLE(list, false);
	}

	private List<ItemRule> NEFPHIJEMLM(RuleAppliance IGFNCCEHFEK)
	{
		List<ItemRule> list = new List<ItemRule>();
		foreach (ItemRule item in _itemRules)
		{
			if (item.EDAKADCHOLE() == IGFNCCEHFEK || item.EDAKADCHOLE() == RuleAppliance.ApplianceAll)
			{
				list.Add(item);
			}
		}
		return list;
	}

	private void OGMJKBDKLMP(Rule HNBFMAKFJAM, List<ItemRule> OEMALIFPGPO)
	{
		switch (HNBFMAKFJAM.get_Type())
		{
		case Rule.BCBLLMPAMLP.RuleRandom:
		{
			RandomRule lEAKKGFJBLL = (RandomRule)HNBFMAKFJAM;
			OGMJKBDKLMP(lEAKKGFJBLL.GHLEKCGJAEP(), OEMALIFPGPO);
			break;
		}
		case Rule.BCBLLMPAMLP.RuleComplex:
		{
			ComplexRule cDFLHDCCMMN = (ComplexRule)HNBFMAKFJAM;
			{
				foreach (Rule item in cDFLHDCCMMN.BONNMLEJBJH())
				{
					OGMJKBDKLMP(item, OEMALIFPGPO);
				}
				break;
			}
		}
		case Rule.BCBLLMPAMLP.RuleItem:
		case Rule.BCBLLMPAMLP.RuleEquipItem:
		case Rule.BCBLLMPAMLP.RuleRandomAquiredItem:
			OEMALIFPGPO.Add((ItemRule)HNBFMAKFJAM);
			break;
		}
	}
}
