using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class QuestCondition : ConditionExtension
{
	public enum NFFNINLIPJJ
	{
		QUEST_CONDITION_NONE = 0,
		QUEST_CONDITION_EQUAL = 1,
		QUEST_CONDITION_GREATER = 2,
		QUEST_CONDITION_GREATER_EQUAL = 3,
		QUEST_CONDITION_LESS = 4,
		QUEST_CONDITION_LESS_EQUAL = 5,
		QUEST_CONDITION_OPERATOR = 6
	}

	public enum OLGPBCCFJCD
	{
		QUEST_CONDITION_SUB_NONE = 0,
		QUEST_CONDITION_SUB_OR = 1,
		QUEST_CONDITION_SUB_AND = 2
	}

	public NFFNINLIPJJ LFLGCDNKNJI;

	public OLGPBCCFJCD FDHOMBHPNEF;

	public string HJBGHOJCEKO;

	public string FKGFIKHINII;

	public List<QuestCondition> conditions = new List<QuestCondition>();

	public bool isNot;

	private QuestParameters GFIHPBCEEOB;

	private RosterQuest HFHCJABFEPE;

	public static NFFNINLIPJJ MHKNIEBONKD(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Equal":
			return NFFNINLIPJJ.QUEST_CONDITION_EQUAL;
		case "Greater":
			return NFFNINLIPJJ.QUEST_CONDITION_GREATER;
		case "GreaterEqual":
			return NFFNINLIPJJ.QUEST_CONDITION_GREATER_EQUAL;
		case "Less":
			return NFFNINLIPJJ.QUEST_CONDITION_LESS;
		case "LessEqual":
			return NFFNINLIPJJ.QUEST_CONDITION_LESS_EQUAL;
		case "Operator":
			return NFFNINLIPJJ.QUEST_CONDITION_OPERATOR;
		default:
			return NFFNINLIPJJ.QUEST_CONDITION_NONE;
		}
	}

	public static OLGPBCCFJCD FDFDIMHDFNH(string FDHOMBHPNEF)
	{
		switch (FDHOMBHPNEF)
		{
		case "Or":
			return OLGPBCCFJCD.QUEST_CONDITION_SUB_OR;
		case "And":
			return OLGPBCCFJCD.QUEST_CONDITION_SUB_AND;
		default:
			return OLGPBCCFJCD.QUEST_CONDITION_SUB_NONE;
		}
	}

	public virtual void Parse(XmlNode BGPKIKNPIKP)
	{
		isNot = XmlUtils.ParseBool(BGPKIKNPIKP.Attributes["Not"]);
		LFLGCDNKNJI = MHKNIEBONKD(BGPKIKNPIKP.Name);
		FDHOMBHPNEF = FDFDIMHDFNH(XmlUtils.ParseString(BGPKIKNPIKP.Attributes["Type"]));
		HJBGHOJCEKO = ClearGaps(XmlUtils.ParseString(BGPKIKNPIKP.Attributes["Value1"]));
		FKGFIKHINII = ClearGaps(XmlUtils.ParseString(BGPKIKNPIKP.Attributes["Value2"]));
	}

	public bool Compare(QuestParameters GFIHPBCEEOB, RosterQuest HFHCJABFEPE)
	{
		bool dCJLKCFKCOM = false;
		if (LFLGCDNKNJI != NFFNINLIPJJ.QUEST_CONDITION_OPERATOR)
		{
			dCJLKCFKCOM = IsCompare(GFIHPBCEEOB, HFHCJABFEPE);
		}
		else
		{
			foreach (QuestCondition item in conditions)
			{
				bool flag = item.Compare(GFIHPBCEEOB, HFHCJABFEPE);
				if (FDHOMBHPNEF == OLGPBCCFJCD.QUEST_CONDITION_SUB_AND && !flag)
				{
					return IsNotCompare(false);
				}
				if (FDHOMBHPNEF == OLGPBCCFJCD.QUEST_CONDITION_SUB_OR && flag)
				{
					return IsNotCompare(true);
				}
			}
			if (FDHOMBHPNEF == OLGPBCCFJCD.QUEST_CONDITION_SUB_AND)
			{
				return IsNotCompare(true);
			}
			if (FDHOMBHPNEF == OLGPBCCFJCD.QUEST_CONDITION_SUB_OR)
			{
				return IsNotCompare(false);
			}
		}
		return IsNotCompare(dCJLKCFKCOM);
	}

	public void LIMHBJBEEIA(QuestParameters JCICKLIMBEF)
	{
		GFIHPBCEEOB = JCICKLIMBEF;
	}

	protected override void IDHOFHMDIPL(string value, CompareResult BMDEBHIHIAJ)
	{
		if (GFIHPBCEEOB != null)
		{
			switch (value)
			{
			case "_$Fight":
				BMDEBHIHIAJ.resultSTR = ((GFIHPBCEEOB.LBGOMJFFEPP() == null) ? string.Empty : GFIHPBCEEOB.LBGOMJFFEPP().BCKFACGMOKC.ToString());
				break;
			case "_$Raid":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.OHPHPJBMNLH;
				break;
			case "_$FightResult":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.HEIADONEACH;
				break;
			case "_$RaidResult":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.AIEHNBBFNPF;
				break;
			case "_$LevelUp":
				BMDEBHIHIAJ.resultNumber = GFIHPBCEEOB.BJIDALJIKNC;
				break;
			case "_$ActionID":
				BMDEBHIHIAJ.resultSTR = ((GFIHPBCEEOB.NPMDMOIHBFP == null) ? string.Empty : GFIHPBCEEOB.NPMDMOIHBFP.Value);
				break;
			case "_$SceneFrom":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.GAEPENBCCPB;
				break;
			case "_$SceneTo":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.GMDFCHJBJGO;
				break;
			case "_$Purchase":
				BMDEBHIHIAJ.resultSTR = ((GFIHPBCEEOB.DLKPBAJDHBO == null) ? string.Empty : GFIHPBCEEOB.DLKPBAJDHBO.Name);
				break;
			case "_$PurchaseUnsuccessful":
				BMDEBHIHIAJ.resultSTR = GJIKJOCPKOI();
				break;
			case "_$Deliver":
				BMDEBHIHIAJ.resultSTR = ((GFIHPBCEEOB.DLKPBAJDHBO == null) ? string.Empty : GFIHPBCEEOB.DLKPBAJDHBO.Name);
				break;
			case "_$EnergyChange":
				BMDEBHIHIAJ.resultNumber = GFIHPBCEEOB.JNGFNNFAAGN;
				break;
			case "_$Iterator":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.PFKPHBPBPAF;
				break;
			case "_$ChosenLocale":
				BMDEBHIHIAJ.resultSTR = ((GFIHPBCEEOB.GMGMEEIKGLG == null) ? string.Empty : GFIHPBCEEOB.GMGMEEIKGLG.name);
				break;
			case "_$ButtonType":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.LCJIAGBCJBM;
				break;
			case "_$TabTo":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.KBKAGDKOGNJ;
				break;
			case "_$TabFrom":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.HJGNGBHONCP;
				break;
			case "_$FightAvgFPS":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.fightAvgFps.ToString();
				break;
			case "_$TimerName":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.NAMGBBCEEEI.ToString();
				break;
			case "_$ButtonName":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.GCKANEECDHE;
				break;
			case "_$PackName":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.JINDOHILBBO;
				break;
			case "_$GemsPrice":
				BMDEBHIHIAJ.resultNumber = GFIHPBCEEOB.AELDOJNIIME;
				break;
			case "_$Enchantment":
				BMDEBHIHIAJ.resultSTR = LJKBDLLNNCO();
				break;
			case "_$PerkName":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.NJDDPMPFCGB;
				break;
			case "_$LotteryLastSpinNumber":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.EGAPDJLHHNJ.ToString();
				break;
			case "_$InLottery":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.inLottery.ToString();
				break;
			case "_$SetItem":
				BMDEBHIHIAJ.resultSTR = GFIHPBCEEOB.FOODLENBJGI.ToString();
				break;
			case "_$StoryTutorialStep":
				BMDEBHIHIAJ.resultSTR = ListSF.CCDKHLAMKKO().BKBHIMEEDBG().JILGHNPIHME();
				break;
			default:
			{
				Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
				RosterQuest.NOKCOAHJIPB nOKCOAHJIPB = nKGLHEGIKKP.PFMIBOCGGPC(value);
				string bAINMLLIKOL = ((nOKCOAHJIPB == null) ? value : nOKCOAHJIPB.Value);
				JLDFNJEALLB(bAINMLLIKOL, BMDEBHIHIAJ);
				break;
			}
			}
		}
		if (!BMDEBHIHIAJ.resultSTR.Equals(string.Empty) && FANCHMOGIEI(BMDEBHIHIAJ.resultSTR) == HAEOKBKNCHE.QUEST_CONDITION_VARIABLE_NUMBER)
		{
			string iBBAMMHHBFE = BMDEBHIHIAJ.resultSTR;
			BMDEBHIHIAJ.resultSTR = string.Empty;
			BMDEBHIHIAJ.resultNumber = float.Parse(iBBAMMHHBFE);
		}
	}

	private bool IsCompare(QuestParameters GFIHPBCEEOB, RosterQuest HFHCJABFEPE)
	{
		this.GFIHPBCEEOB = GFIHPBCEEOB;
		this.HFHCJABFEPE = HFHCJABFEPE;
		CompareResult lNIDLHOIHIM = new CompareResult();
		CompareResult lNIDLHOIHIM2 = new CompareResult();
		MCPIOGALBMK(HJBGHOJCEKO, lNIDLHOIHIM);
		MCPIOGALBMK(FKGFIKHINII, lNIDLHOIHIM2);
		return KGCPIDICOJB(lNIDLHOIHIM, lNIDLHOIHIM2);
	}

	private bool KGCPIDICOJB(CompareResult HJJBNECFJGO, CompareResult KAGCCGKOPFM)
	{
		if (!HJJBNECFJGO.INCOIAANDCO() && !KAGCCGKOPFM.INCOIAANDCO())
		{
			return StringCompare(HJJBNECFJGO.resultSTR, KAGCCGKOPFM.resultSTR);
		}
		if (HJJBNECFJGO.INCOIAANDCO() && KAGCCGKOPFM.INCOIAANDCO())
		{
			return NumberCompare((int)HJJBNECFJGO.resultNumber, (int)KAGCCGKOPFM.resultNumber);
		}
		string oFJMLPGDNKP = HJJBNECFJGO.ToString();
		string iJHJOLLMOHA = KAGCCGKOPFM.ToString();
		return StringCompare(oFJMLPGDNKP, iJHJOLLMOHA);
	}

	private bool StringCompare(string OFJMLPGDNKP, string IJHJOLLMOHA)
	{
		return OFJMLPGDNKP.Equals(IJHJOLLMOHA);
	}

	private bool NumberCompare(int ADADNFFCFII, int ILCHIGNGLPL)
	{
		switch (LFLGCDNKNJI)
		{
		case NFFNINLIPJJ.QUEST_CONDITION_EQUAL:
			return ADADNFFCFII == ILCHIGNGLPL;
		case NFFNINLIPJJ.QUEST_CONDITION_GREATER:
			return ADADNFFCFII > ILCHIGNGLPL;
		case NFFNINLIPJJ.QUEST_CONDITION_GREATER_EQUAL:
			return ADADNFFCFII >= ILCHIGNGLPL;
		case NFFNINLIPJJ.QUEST_CONDITION_LESS:
			return ADADNFFCFII < ILCHIGNGLPL;
		case NFFNINLIPJJ.QUEST_CONDITION_LESS_EQUAL:
			return ADADNFFCFII <= ILCHIGNGLPL;
		default:
			return false;
		}
	}

	private bool IsNotCompare(bool DCJLKCFKCOM)
	{
		return isNot ? (!DCJLKCFKCOM) : DCJLKCFKCOM;
	}

	protected override void AIFNPKLNPEE(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		BMDEBHIHIAJ.Clear();
		switch (KJFKPMCPIBH.FJLOLCPJACB)
		{
		case "Fight":
			COOHPPGOONL(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Player":
			ADOHCGHOCJI(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Item":
			CDOPLOPGDGC(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "PackAssert":
			OPJMEDPINLK(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "UniformIntRandom":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_RAND);
			break;
		case "RandomAspect":
			RandomAspect(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "PerkInfo":
			PerkInfo(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Purchase":
			AGOLEMIMADD(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "ItemsOfType":
			BFJAIODJHJJ(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Battle":
			DAKDCEDOMOJ(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "DataVersion":
			OJHJEMKMDCP(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "VersionController":
			LPBPJBCIGCO(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "SysInfo":
			KCGCNMPCCGH(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Deliver":
			JJLFGBPCDPM(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "SessionSettings":
			SessionSettings(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Sub":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_SUB);
			break;
		case "Sum":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_SUM);
			break;
		case "Multi":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_MULTI);
			break;
		case "Div":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_DIVISION);
			break;
		case "NDiv":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_DIVISION_INT);
			break;
		case "Mod":
			MDENBJJAPMH(KJFKPMCPIBH, BMDEBHIHIAJ, KDEAPAPEEAO.MATH_MOD);
			break;
		case "Concat":
			MLPIEBOJBNM(KJFKPMCPIBH, BMDEBHIHIAJ, FBKBGPPHALB.STRING_CONCAT);
			break;
		case "Slice":
			MLPIEBOJBNM(KJFKPMCPIBH, BMDEBHIHIAJ, FBKBGPPHALB.STRING_SLICE);
			break;
		case "Gift":
			GDPDAMBNKOC(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Timer":
			KCEHOKELKMK(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "ABGroupExists":
			KPHOGPDIHBF(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "Enchantment":
			FAALAGAJAJB(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "FightCurrencyCost":
			AFHLGJJEJIH(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "ShopAssert":
			PPBBCOFDDPI(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "GetSimOperator":
			DLKCIBDPPBA(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		case "RaidInfo":
			KBCJOPOCNKB(KJFKPMCPIBH, BMDEBHIHIAJ);
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},{1}", "QuestCondition::fullFunction - unknown function: ", KJFKPMCPIBH.FJLOLCPJACB));
			break;
		}
	}

	private void RandomAspect(QuestFunctions function, CompareResult result)
	{
		if (function.arguments.Count < 2)
		{
			result.resultNumber = 0.0;
			return;
		}
		int minimum;
		int maximum;
		if (!int.TryParse(function.arguments[0].DCJLKCFKCOM, out minimum) ||
			!int.TryParse(function.arguments[1].DCJLKCFKCOM, out maximum))
		{
			result.resultNumber = 0.0;
			return;
		}
		int level = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		int baseAspect = ForgeManager.ELEBLBJKDBI().GetAspectValueByLevel(level);
		result.resultNumber = baseAspect + NekkiMath.randomInt(minimum, maximum + 1);
	}

	private void PerkInfo(QuestFunctions function, CompareResult result)
	{
		string name = function.OMHIDHHNPEF();
		PerkInfoItem perk = GameUtils.FDEJIIDIPBI.MNMFPCBNLJI(name);
		if (perk == null)
		{
			perk = GameUtils.FDEJIIDIPBI.LAAJJBEEDKL(name);
		}
		if (perk == null)
		{
			perk = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(name);
		}
		if (perk == null)
		{
			return;
		}
		switch (function.HBDLDIKHFEG)
		{
		case "Icon":
		case "Image":
			result.resultSTR = perk.NHKMCLPOMFK;
			break;
		case "Name":
			result.resultSTR = perk.Name;
			break;
		case "Description":
			result.resultSTR = perk.MGNNJPBCOGD;
			break;
		}
	}

	private void COOHPPGOONL(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string text = KJFKPMCPIBH.OMHIDHHNPEF();
		FightList jDIPBIHBGPF = ListSF.ELEBLBJKDBI().AOEPHEPGLAK(text);
		if (jDIPBIHBGPF == null)
		{
			LLLOJBFMONN.Write(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.FightFunction - cant fight fight: ", text));
			return;
		}
		string hBDLDIKHFEG = KJFKPMCPIBH.HBDLDIKHFEG;
		switch (hBDLDIKHFEG)
		{
		case "Name":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.BCKFACGMOKC.ToString();
			break;
		case "Zone":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.BCKFACGMOKC.PELHCAEAOFE();
			break;
		case "Battle":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.BCKFACGMOKC.CPHDPCAECJN();
			break;
		case "Fight":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.BCKFACGMOKC.EJPNIFANKDG();
			break;
		case "Money":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.LDHOBIADNEC.ToString();
			break;
		case "Bonus":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.JBNAJPPNGFB.ToString();
			break;
		case "Type":
			BMDEBHIHIAJ.resultSTR = ListSF.ELEBLBJKDBI().ADHNLNFEOKN(jDIPBIHBGPF.get_Type());
			break;
		case "LossCount":
			BMDEBHIHIAJ.resultNumber = ((jDIPBIHBGPF.FLKFFDLLBKA() != null) ? jDIPBIHBGPF.FLKFFDLLBKA().HCMBHIGGMDF() : 0);
			break;
		case "WinCount":
			BMDEBHIHIAJ.resultNumber = ((jDIPBIHBGPF.FLKFFDLLBKA() != null) ? jDIPBIHBGPF.FLKFFDLLBKA().JAJNIKDMPPO() : 0);
			break;
		case "TimeLeft":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.HHJHCLBCEEA();
			break;
		case "Difficulty":
			BMDEBHIHIAJ.resultNumber = GameUtils.MPNBGBIMEIP(jDIPBIHBGPF);
			break;
		case "Description":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GJOAJAIJHOE();
			break;
		case "Helm":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("Helm");
			break;
		case "Weapon":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("Weapon");
			break;
		case "Armor":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("Armor");
			break;
		case "Magic":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("Magic");
			break;
		case "RaidCharge":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("RaidCharge");
			break;
		case "Ranged":
			BMDEBHIHIAJ.resultSTR = jDIPBIHBGPF.GetRuleItemName("Ranged");
			break;
		case "HelmLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("Helm");
			break;
		case "WeaponLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("Weapon");
			break;
		case "ArmorLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("Armor");
			break;
		case "MagicLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("Magic");
			break;
		case "RaidChargeLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("RaidCharge");
			break;
		case "RangedLevel":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.EENGGKCGLEB("Ranged");
			break;
		case "CheckCurrency":
			BMDEBHIHIAJ.resultNumber = Convert.ToDouble(jDIPBIHBGPF.PCEPDPMOPKC());
			break;
		case "EnoughCurrency":
			BMDEBHIHIAJ.resultNumber = Convert.ToDouble(GameUtils.DJCDFEAMPDA(jDIPBIHBGPF));
			break;
		case "Timestamp":
			BMDEBHIHIAJ.resultNumber = ((jDIPBIHBGPF.FLKFFDLLBKA() == null) ? 0 : jDIPBIHBGPF.FLKFFDLLBKA().ILBNPNIPEHO());
			break;
		case "Level":
			BMDEBHIHIAJ.resultNumber = ((jDIPBIHBGPF.FLKFFDLLBKA() != null) ? jDIPBIHBGPF.FLKFFDLLBKA().PINDEKDNCNL() : 0);
			break;
		case "Power":
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.BFMMAFJFABG();
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.FightFunction - unknown property: ", hBDLDIKHFEG));
			break;
		}
	}

	private void ADOHCGHOCJI(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			return;
		}
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Skeleton":
		{
			ItemInfo dJKEECEOCJB6 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Skeleton");
			if (dJKEECEOCJB6 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB6.Name;
			}
			break;
		}
		case "Helm":
		{
			ItemInfo dJKEECEOCJB2 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Helm");
			if (dJKEECEOCJB2 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB2.Name;
			}
			break;
		}
		case "Armor":
		{
			ItemInfo dJKEECEOCJB4 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Armor");
			if (dJKEECEOCJB4 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB4.Name;
			}
			break;
		}
		case "Weapon":
		{
			ItemInfo dJKEECEOCJB7 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Weapon");
			if (dJKEECEOCJB7 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB7.Name;
			}
			break;
		}
		case "Magic":
		{
			ItemInfo dJKEECEOCJB5 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Magic");
			if (dJKEECEOCJB5 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB5.Name;
			}
			break;
		}
		case "RaidCharge":
		{
			ItemInfo dJKEECEOCJB3 = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("RaidConsumable");
			if (dJKEECEOCJB3 != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB3.Name;
			}
			break;
		}
		case "Ranged":
		{
			ItemInfo dJKEECEOCJB = nKGLHEGIKKP.get_Parameters().KDABEFBJMOD("Ranged");
			if (dJKEECEOCJB != null)
			{
				BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.Name;
			}
			break;
		}
		case "Money":
			BMDEBHIHIAJ.resultNumber = nKGLHEGIKKP.BFBOEGMAMNF();
			break;
		case "Bonus":
			BMDEBHIHIAJ.resultNumber = nKGLHEGIKKP.EHFJHFDACMP();
			break;
		case "Level":
			BMDEBHIHIAJ.resultNumber = nKGLHEGIKKP.PINDEKDNCNL();
			break;
		case "Power":
			BMDEBHIHIAJ.resultNumber = nKGLHEGIKKP.NHKMGNPADKI();
			break;
		case "Language":
			BMDEBHIHIAJ.resultSTR = nKGLHEGIKKP.GKLECBABFCP();
			break;
		case "CoinIcon":
			BMDEBHIHIAJ.resultSTR = nKGLHEGIKKP.OGJBDMNBMLJ();
			break;
		case "MapFocus":
		{
			FightIDS mOCEDDJOAEB2 = ListSF.CCDKHLAMKKO().KNJNHKDCINB();
			BMDEBHIHIAJ.resultSTR = mOCEDDJOAEB2.OOBHBGJIBGP();
			break;
		}
		case "RaidMapFocus":
		{
			FightIDS mOCEDDJOAEB = ListSF.CCDKHLAMKKO().MGICKOOCNAJ();
			BMDEBHIHIAJ.resultSTR = mOCEDDJOAEB.OOBHBGJIBGP();
			break;
		}
		case "IsLoggedInRaids":
			BMDEBHIHIAJ.resultNumber = Convert.ToDouble(NMBFNPAEECM.ELEBLBJKDBI().PCHMOEKBLHB());
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.PlayerFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void CDOPLOPGDGC(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string gOHIIMFFFJI = KJFKPMCPIBH.OMHIDHHNPEF();
		ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(gOHIIMFFFJI);
		if (dJKEECEOCJB == null)
		{
			return;
		}
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Price":
			BMDEBHIHIAJ.resultNumber = (ObscuredLong)(dJKEECEOCJB.KJFAOKLILOC);
			break;
		case "BonusPrice":
			BMDEBHIHIAJ.resultNumber = (ObscuredLong)(dJKEECEOCJB.FMHECGHHKGB);
			break;
		case "BonusDeliveryPrice":
			BMDEBHIHIAJ.resultNumber = (ObscuredLong)(dJKEECEOCJB.KLHOKKPALOK);
			break;
		case "MoneyDeliveryPrice":
			BMDEBHIHIAJ.resultNumber = (ObscuredLong)(dJKEECEOCJB.NDCOLFHCNLD);
			break;
		case "Level":
			BMDEBHIHIAJ.resultNumber = dJKEECEOCJB.MHGODOLNDLE;
			break;
		case "Equipped":
		{
			UserItem dKCHDHMLKHN2 = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(dJKEECEOCJB);
			if (dKCHDHMLKHN2 != null)
			{
				BMDEBHIHIAJ.resultNumber = Convert.ToDouble(dKCHDHMLKHN2.EFMFGEPDAOP());
			}
			break;
		}
		case "Quantity":
		{
			UserItem dKCHDHMLKHN = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(dJKEECEOCJB);
			BMDEBHIHIAJ.resultNumber = ((dKCHDHMLKHN != null) ? dKCHDHMLKHN.OFOPFCJNEBL() : 0);
			break;
		}
		case "NextMoneyUpgradePrice":
		{
			ItemInfo mBIJKDIEFIF = NKAONHENEGF(dJKEECEOCJB);
			BMDEBHIHIAJ.resultNumber = MPPKMHFENKC(mBIJKDIEFIF, true);
			break;
		}
		case "NextBonusUpgradePrice":
		{
			ItemInfo mBIJKDIEFIF2 = NKAONHENEGF(dJKEECEOCJB);
			BMDEBHIHIAJ.resultNumber = MPPKMHFENKC(mBIJKDIEFIF2, false);
			break;
		}
		case "NextUpgradeDeliveryPrice":
		{
			ItemInfo mBIJKDIEFIF4 = NKAONHENEGF(dJKEECEOCJB);
			BMDEBHIHIAJ.resultNumber = IIDGONMFNOG(mBIJKDIEFIF4, false);
			break;
		}
		case "NextUpgradeDeliveryTime":
		{
			ItemInfo mBIJKDIEFIF3 = NKAONHENEGF(dJKEECEOCJB);
			BMDEBHIHIAJ.resultNumber = IGLJKOFOKNI(mBIJKDIEFIF3);
			break;
		}
		case "Type":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.Type;
			break;
		case "SubType":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.MDPPNGIEJGD;
			break;
		case "Availability":
		{
			bool flag = dJKEECEOCJB.GOKHJMOEGIJ() || !dJKEECEOCJB.DCHJDPCEODD || (!dJKEECEOCJB.MMHIKEIDDNB.Equals(string.Empty) && !nKGLHEGIKKP.FLFKOIPCEPI(dJKEECEOCJB.MMHIKEIDDNB));
			BMDEBHIHIAJ.resultNumber = ((!flag) ? 1 : 0);
			break;
		}
		case "RealPrice":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.FPEIFLEBEAA;
			break;
		case "RecieveGold":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.HHIFKGOJFAC.ToString();
			break;
		case "RecieveBonus":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.BBMLCBEFLGI.ToString();
			break;
		case "Image":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.FileName;
			break;
		case "PackLabel":
			BMDEBHIHIAJ.resultSTR = dJKEECEOCJB.MMHIKEIDDNB;
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.ItemFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private ItemInfo NKAONHENEGF(ItemInfo item)
	{
		ItemInfo dJKEECEOCJB = null;
		UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
		if (dKCHDHMLKHN != null && dKCHDHMLKHN.IJGAOHJNLAH() > 0 && dKCHDHMLKHN.EIMMBNNMBCN() > 0)
		{
			ItemInfo dJKEECEOCJB2 = dKCHDHMLKHN.AENKEPCBHJG();
			dJKEECEOCJB = ((dJKEECEOCJB2 == null) ? null : dJKEECEOCJB2.Clone());
		}
		if (dJKEECEOCJB != null)
		{
			dJKEECEOCJB = ((dKCHDHMLKHN == null) ? item.GJAMPOFICNK(0) : dKCHDHMLKHN.HADDPFNDPDG().Clone());
		}
		return dJKEECEOCJB;
	}

	private long MPPKMHFENKC(ItemInfo item, bool EDNGDDEPAPA)
	{
		long result = 2147483647L;
		if (item != null)
		{
			if ((EDNGDDEPAPA && item.INCBGIDFIDN()) || (!EDNGDDEPAPA && item.PLBFFNCCCGO()))
			{
				result = (ObscuredLong)((!EDNGDDEPAPA) ? item.FMHECGHHKGB : item.KJFAOKLILOC);
			}
			else
			{
				LLLOJBFMONN.Write(string.Format("{0},{2},{1},{3}", "QuestCondition::itemFunction ", " price - no price: ", (!EDNGDDEPAPA) ? "bonus" : "money", item.Name));
			}
		}
		else
		{
			LLLOJBFMONN.Write(string.Format("{0},{2},{1}", "QuestCondition::itemFunction ", " price - no next upgrade", (!EDNGDDEPAPA) ? "bonus" : "money"));
		}
		return result;
	}

	private long IIDGONMFNOG(ItemInfo item, bool EDNGDDEPAPA)
	{
		long result = 2147483647L;
		if (item != null)
		{
			result = (ObscuredLong)((!EDNGDDEPAPA) ? item.KLHOKKPALOK : item.NDCOLFHCNLD);
		}
		else
		{
			LLLOJBFMONN.Write(string.Format("{0},{2},{1}", "QuestCondition::itemFunction ", " price - no next upgrade", (!EDNGDDEPAPA) ? "bonus" : "money"));
		}
		return result;
	}

	private long IGLJKOFOKNI(ItemInfo item)
	{
		long result = 2147483647L;
		if (item != null)
		{
			result = item.EHKNIKHPGDN;
		}
		else
		{
			LLLOJBFMONN.Write("QuestCondition::getDeliveryUpgradeTime ERROR - no upgrade found");
		}
		return result;
	}

	private void OPJMEDPINLK(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string gOHIIMFFFJI = KJFKPMCPIBH.OMHIDHHNPEF();
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Availability":
		{
			bool flag = PacksController.ELEBLBJKDBI().IsPackByName(gOHIIMFFFJI);
			BMDEBHIHIAJ.resultNumber = (flag ? 1 : 0);
			break;
		}
		case "Existence":
		{
			JBKAOMLJCEL jBKAOMLJCEL2 = PacksController.ELEBLBJKDBI().OCKOCHAINHG(gOHIIMFFFJI);
			BMDEBHIHIAJ.resultNumber = ((jBKAOMLJCEL2 != null) ? 1 : 0);
			break;
		}
		case "Size":
		{
			JBKAOMLJCEL jBKAOMLJCEL = GeneralConfig.NNFMKNJJDDD.OCKOCHAINHG(gOHIIMFFFJI);
			if (jBKAOMLJCEL != null)
			{
				BMDEBHIHIAJ.resultSTR = jBKAOMLJCEL.Size;
			}
			break;
		}
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.PackFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void AGOLEMIMADD(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string[] array = KJFKPMCPIBH.OMHIDHHNPEF().Split('|');
		if (array.Length != 0)
		{
			string gOHIIMFFFJI = array[0];
			string iBBAMMHHBFE = ((array.Length <= 1) ? string.Empty : array[1]);
			UserItem dKCHDHMLKHN = ListSF.CMGOCLGHNLH(gOHIIMFFFJI);
			ItemInfo dJKEECEOCJB = ((dKCHDHMLKHN == null) ? ListSF.DJBOFEEKJMP().KCCDBEEKBCG(gOHIIMFFFJI) : dKCHDHMLKHN.BHKHOJPANHE());
			switch (KJFKPMCPIBH.HBDLDIKHFEG)
			{
			case "Type":
				BMDEBHIHIAJ.resultSTR = ((dJKEECEOCJB == null) ? string.Empty : dJKEECEOCJB.Type);
				break;
			case "Name":
				BMDEBHIHIAJ.resultSTR = ((dJKEECEOCJB == null) ? string.Empty : dJKEECEOCJB.Name);
				break;
			case "UpgradeLevel":
				BMDEBHIHIAJ.resultNumber = ((dKCHDHMLKHN != null) ? dKCHDHMLKHN.DHNNCAEEMLL() : 0);
				break;
			case "Timeout":
			{
				long num = ((dKCHDHMLKHN == null) ? 0 : dKCHDHMLKHN.IJGAOHJNLAH());
				long num2 = GameUtils.ECCPJAPIABG();
				long num3 = ((num <= num2) ? 0 : (num - num2));
				BMDEBHIHIAJ.resultNumber = num3;
				break;
			}
			case "Failure":
				BMDEBHIHIAJ.resultSTR = iBBAMMHHBFE;
				break;
			case "PaidItem":
				BMDEBHIHIAJ.resultSTR = ((dJKEECEOCJB == null) ? string.Empty : dJKEECEOCJB.PBMHNMOHODB);
				break;
			default:
				LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.PurchaseFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
				break;
			}
		}
	}

	private void BFJAIODJHJJ(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP != null)
		{
			string lFLGCDNKNJI = KJFKPMCPIBH.OMHIDHHNPEF();
			List<UserItem> list = nKGLHEGIKKP.KHCNHPCPFII().HOPBBLJLHOB(lFLGCDNKNJI, string.Empty);
			if (KJFKPMCPIBH.HBDLDIKHFEG.Equals("Quantity"))
			{
				BMDEBHIHIAJ.resultNumber = list.Count;
			}
		}
	}

	private void DAKDCEDOMOJ(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string dIAIIPCBMFL = KJFKPMCPIBH.OMHIDHHNPEF();
		FightIDS mOCEDDJOAEB = new FightIDS(dIAIIPCBMFL);
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Available":
		{
			Battle cGJCGEBPCAF3 = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
			if (cGJCGEBPCAF3 != null)
			{
				Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
				bool flag = nKGLHEGIKKP.HAMPNCKAJKD(mOCEDDJOAEB);
				BMDEBHIHIAJ.resultNumber = (flag ? 1 : 0);
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "Quest Error: no such battle in stages: ", mOCEDDJOAEB.ToString()));
				BMDEBHIHIAJ.resultNumber = 0.0;
			}
			break;
		}
		case "Locked":
		{
			Battle cGJCGEBPCAF5 = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
			bool flag2 = true;
			if (cGJCGEBPCAF5 != null)
			{
				flag2 = cGJCGEBPCAF5.NNPNEABKHPP() == null || cGJCGEBPCAF5.NNPNEABKHPP().NLIJBCHAEBK();
			}
			BMDEBHIHIAJ.resultNumber = (flag2 ? 1 : 0);
			break;
		}
		case "Name":
		{
			Battle cGJCGEBPCAF4 = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
			if (cGJCGEBPCAF4 != null)
			{
				BMDEBHIHIAJ.resultSTR = cGJCGEBPCAF4.get_Name();
				break;
			}
			LLLOJBFMONN.Error(string.Format("{0},{1}", "Quest Error: no such battle in stages: ", mOCEDDJOAEB.ToString()));
			BMDEBHIHIAJ.resultSTR = string.Empty;
			break;
		}
		case "Type":
		{
			Battle cGJCGEBPCAF2 = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
			BattleType lFLGCDNKNJI = BattleType.FightDummy;
			if (cGJCGEBPCAF2 != null)
			{
				lFLGCDNKNJI = cGJCGEBPCAF2.get_Type();
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "Quest Error: no such battle in stages: ", mOCEDDJOAEB.ToString()));
			}
			BMDEBHIHIAJ.resultSTR = ListSF.ELEBLBJKDBI().ADHNLNFEOKN(lFLGCDNKNJI);
			break;
		}
		case "Zone":
		{
			Battle cGJCGEBPCAF = ListSF.MKHAAGMJOPG(mOCEDDJOAEB);
			if (cGJCGEBPCAF != null)
			{
				Zone pKCPOJKLMOK = cGJCGEBPCAF.LKDFFCADHNO();
				if (pKCPOJKLMOK != null)
				{
					BMDEBHIHIAJ.resultSTR = pKCPOJKLMOK.get_Name();
				}
				else
				{
					BMDEBHIHIAJ.resultSTR = string.Empty;
				}
			}
			else
			{
				LLLOJBFMONN.Error(string.Format("{0},{1}", "Quest Error: no such battle in stages: ", mOCEDDJOAEB.ToString()));
				BMDEBHIHIAJ.resultSTR = string.Empty;
			}
			break;
		}
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.BattleFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void OJHJEMKMDCP(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		VersionContainer pAMHFPMEPCH = SystemProperties.DFJEJKJECBI();
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Production":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.FAOHNABGKFH();
			break;
		case "Major":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.ELEBDJHKBPL();
			break;
		case "Minor":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.FMHLIFBPFBN();
			break;
		case "DataVersion":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.DFJEJKJECBI();
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.UserVersionFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void LPBPJBCIGCO(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		VersionContainer pAMHFPMEPCH = SystemProperties.KCJMMIEBLHL();
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Production":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.FAOHNABGKFH();
			break;
		case "Major":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.ELEBDJHKBPL();
			break;
		case "Minor":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.FMHLIFBPFBN();
			break;
		case "DataVersion":
			BMDEBHIHIAJ.resultNumber = pAMHFPMEPCH.DFJEJKJECBI();
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition.VersionControllerFunction - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void KCGCNMPCCGH(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		DeviceInfo fFMKFOCMPBN = SystemProperties.NICPICAMAOH();
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "DeviceType":
			if (SystemProperties.FBGNIKBPCFB())
			{
				BMDEBHIHIAJ.resultSTR = "Tablet";
			}
			else
			{
				BMDEBHIHIAJ.resultSTR = "Phone";
			}
			break;
		case "ResolutionLocation":
			BMDEBHIHIAJ.resultSTR = SystemProperties.DCPMKCGDHPJ(fFMKFOCMPBN.BLIOAMODNOH);
			break;
		case "ResolutionGUI":
			BMDEBHIHIAJ.resultSTR = SystemProperties.DCPMKCGDHPJ(fFMKFOCMPBN.ACHKMBJANGN);
			break;
		case "Id":
			BMDEBHIHIAJ.resultSTR = fFMKFOCMPBN.Id;
			break;
		case "Os":
			BMDEBHIHIAJ.resultSTR = fFMKFOCMPBN.DHPGNIFEOPI;
			break;
		case "OsName":
			BMDEBHIHIAJ.resultSTR = fFMKFOCMPBN.MDPBKNDOGKJ;
			break;
		case "Language":
			BMDEBHIHIAJ.resultSTR = fFMKFOCMPBN.OAPHJAPMKJG;
			break;
		case "CpuCount":
			BMDEBHIHIAJ.resultNumber = fFMKFOCMPBN.MOMPODBNJNE;
			break;
		case "Ram":
			BMDEBHIHIAJ.resultNumber = fFMKFOCMPBN.AOJLHDILEBJ;
			break;
		case "DisplayWidth":
			BMDEBHIHIAJ.resultNumber = fFMKFOCMPBN.LBKMKDKDFJF;
			break;
		case "DisplayHeight":
			BMDEBHIHIAJ.resultNumber = fFMKFOCMPBN.LLFOGEMDMJD;
			break;
		case "Account":
			BMDEBHIHIAJ.resultSTR = GameCenterController.CONEABALMEJ();
			break;
		case "China":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() ? 1 : 0);
			break;
		case "Korea":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().DMJJDFCAKFG() ? 1 : 0);
			break;
		case "Amazon":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() ? 1 : 0);
			break;
		case "Steam":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().DBJOHGNPDDO() ? 1 : 0);
			break;
		case "Japan":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().GIGEOMONCON() ? 1 : 0);
			break;
		case "AmazonMobile":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().OPCBKOOFMAK() ? 1 : 0);
			break;
		case "AndroidTV":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().COPJOJAMBKA() ? 1 : 0);
			break;
		case "WinStore":
			BMDEBHIHIAJ.resultNumber = (AssemblyController.JONCCPLEIBE().OKALPNOADLJ() ? 1 : 0);
			break;
		case "Time":
			BMDEBHIHIAJ.resultSTR = ListSF.IDMJOMOMDOJ().ToString();
			BMDEBHIHIAJ.resultSTR = "0";
			break;
		case "RatingUrl":
			BMDEBHIHIAJ.resultSTR = InternetController.DMFANLAIJMN();
			break;
		case "FacebookLiked":
			BMDEBHIHIAJ.resultNumber = (ListSF.CCDKHLAMKKO().MLOHMAGMIAI() ? 1 : 0);
			break;
		case "FBLikeUrl":
			BMDEBHIHIAJ.resultSTR = InternetController.BKGEABLMGKL().Url;
			break;
		case "FBLikeAltUrl":
			BMDEBHIHIAJ.resultSTR = InternetController.BKGEABLMGKL().OLLDPHHNBCC;
			break;
		case "UserObserved":
			BMDEBHIHIAJ.resultNumber = (EventLog.ELEBLBJKDBI().DKKNEBIGLPJ() ? 1 : 0);
			break;
		case "Connection":
			BMDEBHIHIAJ.resultNumber = (SystemProperties.PKLFCFBEIIG() ? 1 : 0);
			break;
		case "QualityCondition":
			BMDEBHIHIAJ.resultSTR = GraphicsController.PMAODLMLDLK();
			break;
		case "DeviceTotalMem":
		{
			float num = SystemProperties.NICPICAMAOH().AOJLHDILEBJ / 1024;
			BMDEBHIHIAJ.resultNumber = num;
			break;
		}
		case "LastSessionCrashed":
			BMDEBHIHIAJ.resultNumber = (NGOFBFGBICM.ELEBLBJKDBI().FHKBBLCJNFH() ? 1 : 0);
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "QuestCondition.SysInfoFunction - unknown property ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void GDPDAMBNKOC(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Exist":
			BMDEBHIHIAJ.resultNumber = (NetworkController.ELEBLBJKDBI().LBDHOLEICEG.GMBOPFIPNAE ? 1 : 0);
			break;
		case "Money":
			BMDEBHIHIAJ.resultNumber = NetworkController.ELEBLBJKDBI().LBDHOLEICEG.JDPAGMPKLHB;
			break;
		case "Bonus":
			BMDEBHIHIAJ.resultNumber = NetworkController.ELEBLBJKDBI().LBDHOLEICEG.OHHLCBPGOIM;
			break;
		case "Items":
			BMDEBHIHIAJ.resultNumber = NetworkController.ELEBLBJKDBI().LBDHOLEICEG.OJIAKDDCGLB.Count;
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "QuestCondition.GiftFunction - unknown property ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private void JJLFGBPCDPM(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		AGOLEMIMADD(KJFKPMCPIBH, BMDEBHIHIAJ);
	}

	private void SessionSettings(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP.FJGCOOAACLD(KJFKPMCPIBH.HBDLDIKHFEG))
		{
			string text = nKGLHEGIKKP.GetSettingsXML(KJFKPMCPIBH.HBDLDIKHFEG);
			BMDEBHIHIAJ.resultNumber = (text.Equals(string.Empty) ? 0.0 : double.Parse(text));
		}
	}

	private void KCEHOKELKMK(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string text = KJFKPMCPIBH.OMHIDHHNPEF();
		ListSF oPLPFMFAGMN = ListSF.ELEBLBJKDBI();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (!KJFKPMCPIBH.HBDLDIKHFEG.Equals("Value"))
		{
			return;
		}
		if (text.Equals("EnergyRefillTimer"))
		{
			BMDEBHIHIAJ.resultNumber = nKGLHEGIKKP.NHFHDFIJEJG();
			return;
		}
		if (text.Equals("DuelAccessibilityTimer"))
		{
			if (BattlePeriodic.CCCIFDLEMPI() > 0)
			{
				BMDEBHIHIAJ.resultNumber = BattlePeriodic.IDGBNPFIDGC() - BattlePeriodic.CCCIFDLEMPI();
			}
			return;
		}
		if (text.Equals("StarterPackTimer"))
		{
			BMDEBHIHIAJ.resultNumber = GameUtils.GetLeftTime(nKGLHEGIKKP.AACMNAJJKME());
			return;
		}
		RosterTimerContainer kCMICMHCEBB = nKGLHEGIKKP.AEMFLPNDDKL();
		RosterTimer fPNMILOHPMB = kCMICMHCEBB.PPCMACMLHCA(text);
		if (fPNMILOHPMB != null)
		{
			BMDEBHIHIAJ.resultNumber = GameUtils.GetLeftTime(fPNMILOHPMB.CMIABOOJOEN());
		}
	}

	private void KPHOGPDIHBF(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		string gOHIIMFFFJI = KJFKPMCPIBH.OMHIDHHNPEF();
		BMDEBHIHIAJ.resultNumber = (nKGLHEGIKKP.FNLMHKJGCMC(gOHIIMFFFJI) ? 1 : 0);
	}

	private void FAALAGAJAJB(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string[] array = KJFKPMCPIBH.OMHIDHHNPEF().Split('|');
		if (array.Length < 2)
		{
			return;
		}
		string bAINMLLIKOL = array[0];
		CompareResult lNIDLHOIHIM = new CompareResult();
		QuestCondition kKDGLNECFHA = new QuestCondition();
		kKDGLNECFHA.LIMHBJBEEIA(GFIHPBCEEOB);
		kKDGLNECFHA.MCPIOGALBMK(bAINMLLIKOL, lNIDLHOIHIM);
		bAINMLLIKOL = lNIDLHOIHIM.resultSTR;
		string bAINMLLIKOL2 = array[1];
		kKDGLNECFHA.MCPIOGALBMK(bAINMLLIKOL2, lNIDLHOIHIM);
		bAINMLLIKOL2 = lNIDLHOIHIM.resultSTR;
		long num = 0L;
		if (array.Length == 3)
		{
			string bAINMLLIKOL3 = array[2];
			kKDGLNECFHA.MCPIOGALBMK(bAINMLLIKOL3, lNIDLHOIHIM);
			num = (long)lNIDLHOIHIM.resultNumber;
		}
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "Item":
			BMDEBHIHIAJ.resultSTR = bAINMLLIKOL;
			break;
		case "Recipe":
			BMDEBHIHIAJ.resultSTR = bAINMLLIKOL2;
			break;
		case "Timeout":
		{
			RecipePrice pANAKJICBKI2 = ForgeManager.ELEBLBJKDBI().FIGKJLNILIN(bAINMLLIKOL, bAINMLLIKOL2);
			BMDEBHIHIAJ.resultNumber = ((pANAKJICBKI2 != null) ? pANAKJICBKI2.EHKNIKHPGDN : 0);
			break;
		}
		case "DeliveryTime":
			BMDEBHIHIAJ.resultNumber = num;
			break;
		case "BonusDeliveryPrice":
		{
			RecipePrice pANAKJICBKI = ForgeManager.ELEBLBJKDBI().FIGKJLNILIN(bAINMLLIKOL, bAINMLLIKOL2);
			BMDEBHIHIAJ.resultNumber = (int)((pANAKJICBKI != null) ? (ObscuredLong)(pANAKJICBKI.KLHOKKPALOK) : 0);
			break;
		}
		case "Available":
		{
			Recipe iNODIOJPNJH = ForgeManager.ELEBLBJKDBI().GetRecipeByName(bAINMLLIKOL2);
			UserItem dKCHDHMLKHN = ListSF.CMGOCLGHNLH(bAINMLLIKOL);
			bool flag = false;
			if (iNODIOJPNJH != null && dKCHDHMLKHN != null)
			{
				flag = iNODIOJPNJH.IHHJGMBGHEB(dKCHDHMLKHN);
			}
			BMDEBHIHIAJ.resultNumber = (flag ? 1 : 0);
			break;
		}
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "QuestCondition.EnchantmentFunction - unknown property ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}

	private string GJIKJOCPKOI()
	{
		StringBuilder stringBuilder = new StringBuilder();
		ItemInfo dLKPBAJDHBO = GFIHPBCEEOB.DLKPBAJDHBO;
		if (dLKPBAJDHBO != null)
		{
			stringBuilder.Append(dLKPBAJDHBO.Name);
			if (!GFIHPBCEEOB.OOFHDANMCJB.Equals(string.Empty))
			{
				stringBuilder.Append("|");
				stringBuilder.Append(GFIHPBCEEOB.OOFHDANMCJB);
			}
		}
		return stringBuilder.ToString();
	}

	private string LJKBDLLNNCO()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!GFIHPBCEEOB.DPLEGFCHOCE.OHCGEEEKEJH.Equals(string.Empty))
		{
			long num = 0L;
			if (GFIHPBCEEOB.DPLEGFCHOCE.BMNFPNBAMAF > 0)
			{
				long num2 = ListSF.BLBNJKJKMBM();
				num = GFIHPBCEEOB.DPLEGFCHOCE.BMNFPNBAMAF - num2;
				if (num < 0)
				{
					num = 0L;
				}
			}
			stringBuilder.Append(GFIHPBCEEOB.DPLEGFCHOCE.OHCGEEEKEJH);
			stringBuilder.Append("|");
			stringBuilder.Append(GFIHPBCEEOB.DPLEGFCHOCE.FHELNNCGCGC);
			stringBuilder.Append("|");
			if (num > 0)
			{
				stringBuilder.Append(num);
			}
		}
		return stringBuilder.ToString();
	}

	private void AFHLGJJEJIH(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string text = ((KJFKPMCPIBH.arguments.Count <= 0) ? string.Empty : KJFKPMCPIBH.arguments[0].DCJLKCFKCOM);
		string text2 = ((KJFKPMCPIBH.arguments.Count <= 1) ? string.Empty : KJFKPMCPIBH.arguments[1].DCJLKCFKCOM);
		FightList jDIPBIHBGPF = ListSF.ELEBLBJKDBI().AOEPHEPGLAK(text);
		if (jDIPBIHBGPF == null)
		{
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition::fightCurrencyCostFunction - cant fight fight: ", text));
		}
		else if (text2.Equals(string.Empty))
		{
			LLLOJBFMONN.Error("ERROR: QuestCondition::fightCurrencyCostFunction - currencyName is empty");
		}
		else
		{
			BMDEBHIHIAJ.resultNumber = jDIPBIHBGPF.BCGHJHJBCME(text2);
		}
	}

	private void PPBBCOFDDPI(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string gOHIIMFFFJI = KJFKPMCPIBH.OMHIDHHNPEF();
		string hBDLDIKHFEG = KJFKPMCPIBH.HBDLDIKHFEG;
		if (hBDLDIKHFEG.Equals("IsOpen"))
		{
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			bool flag = nKGLHEGIKKP.FLFKOIPCEPI(gOHIIMFFFJI);
			BMDEBHIHIAJ.resultNumber = (flag ? 1 : 0);
		}
		else
		{
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition::shopAssertFunction - unknown property: ", hBDLDIKHFEG));
		}
	}

	private void DLKCIBDPPBA(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		string text = KJFKPMCPIBH.OMHIDHHNPEF();
		string hBDLDIKHFEG = KJFKPMCPIBH.HBDLDIKHFEG;
		if (hBDLDIKHFEG != null && hBDLDIKHFEG == "Code")
		{
			BMDEBHIHIAJ.resultSTR = string.Empty;
		}
		else
		{
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition::getSimOperator - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
		}
	}

	private void KBCJOPOCNKB(QuestFunctions KJFKPMCPIBH, CompareResult BMDEBHIHIAJ)
	{
		switch (KJFKPMCPIBH.HBDLDIKHFEG)
		{
		case "TutorialStep":
			BMDEBHIHIAJ.resultSTR = GameUtils.HEJIFIHLLJF[ListSF.CCDKHLAMKKO().BKBHIMEEDBG().NAGDMOLMLGH()];
			break;
		case "RestoreCurrencyValue":
			break;
		default:
			LLLOJBFMONN.Error(string.Format("{0},\"{1}\"", "ERROR: QuestCondition::getSimOperator - unknown property: ", KJFKPMCPIBH.HBDLDIKHFEG));
			break;
		}
	}
}
