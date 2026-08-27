using System.Collections.Generic;
using System.Xml;

public class AiData
{
	public enum HDHPLDFCDOF
	{
		randomAnimation = -2,
		noneTable = -1,
		outcometablesforattack = 0,
		movementsTable = 1,
		dodgeTable = 2,
		attackTable = 3,
		summaryResultTable = 4,
		safeTable = 5,
		quickAttact = 6,
		shiftTable = 7,
		throwTactics = 8,
		evadeList = 9,
		block = 10
	}

	public static bool DNKLLMKCNPP = false;

	private static List<List<global::Pair<TacticalTableHolder, global::Pair<string, string>>>> DCECBCKPJOD = null;

	private static string[] TacticsTableNames;

	private static List<Tactic> PNBAAKIIDGG = new List<Tactic>();

	private static Tactic JMGFJNKBJHM;

	public static int KINPOOFGAGD;

	public static int EFODNHDEKCL;

	public static List<int> AttackTablesFrames = new List<int>();

	public const int JIKCHGLIPJD = 3;

	private static bool JADMPKACIGJ;

	private static List<global::Pair<string, List<string>>> KCLGCKOADLM = new List<global::Pair<string, List<string>>>();

	private static List<string> MANIJDBMOIH = new List<string>();

	private static List<string> DMMADDDAALP = new List<string>();

	private static List<string> CMJBPEBDHMI = new List<string>();

	private static List<string> DMHKCBDMJKA = new List<string>();

	private static List<string> KMCGCCBPGCN = new List<string>();

	private static List<string> HOHPLMFPENC = new List<string>();

	private static List<string> BPAPMIPFMGA = new List<string>();

	private static List<string> DFPGCHKAKOK = new List<string>();

	private static List<string> FEBACABHOML = new List<string>();

	private static List<string> OJLFJPBKJAF = new List<string>();

	private static List<string> BNAGEDKALKO = new List<string>();

	private static List<string> IJFIGCJJMIK = new List<string>();

	private static List<string> FMILEEDDEEM = new List<string>();

	private static List<string> ENGMFIDLPHG = new List<string>();

	private static List<string> NOFHPKKFIOK = new List<string>();

	private static List<TemplateAnimation> KLKMOILIBBO = new List<TemplateAnimation>();

	private static List<TemplateAnimation> JOOELCENMFC = new List<TemplateAnimation>();

	private static List<TemplateAnimation> CHECBPGPICK = new List<TemplateAnimation>();

	private static List<TemplateAnimation> PJJEHBOPLNB = new List<TemplateAnimation>();

	private static List<TemplateAnimation> EPJJKMOFJHC = new List<TemplateAnimation>();

	private static List<TemplateAnimation> OLPDJEKFMGJ = new List<TemplateAnimation>();

	private static List<string> KFACNBAPBHE = new List<string>();

	private static string _DistanceNode;

	private static bool LIPCBILPHEJ;

	public static List<List<global::Pair<TacticalTableHolder, global::Pair<string, string>>>> IMBCKPELMGC
	{
		get
		{
			return get_TablesHoldersNew();
		}
	}

	public static List<Tactic> KMMJCHDKBDO
	{
		get
		{
			return get_Parameters();
		}
	}

	public static bool OPILHCMOKEL
	{
		get
		{
			return get_BothBotEnabled();
		}
	}

	public static List<string> ILLLLJLBFGJ
	{
		get
		{
			return get_NoDecisionIntervals();
		}
	}

	public static List<string> PIJBOFMJIGG
	{
		get
		{
			return get_NoDecisionMoves();
		}
	}

	public static List<string> IAGGIFAOMBJ
	{
		get
		{
			return get_UnexpectedMoves();
		}
	}

	public static List<string> NCOMNOMCKFA
	{
		get
		{
			return get_MovesFirstIteration();
		}
	}

	public static List<string> EFNEDAPKJDE
	{
		get
		{
			return get_MovesLastIteration();
		}
	}

	public static List<string> NMKOLDBJMLO
	{
		get
		{
			return get_MissilesFirstIteration();
		}
	}

	public static List<string> PNOECDBKLAE
	{
		get
		{
			return get_MissilesLastIteration();
		}
	}

	public static List<string> CNIMGPIALGN
	{
		get
		{
			return get_MoveLengthIntervalsStrict();
		}
	}

	public static List<string> CKIIMFEDDKP
	{
		get
		{
			return get_MoveLengthIntervalsExtended();
		}
	}

	public static List<string> GOMJNBJOKJG
	{
		get
		{
			return get_IgnoredEnemyAnimations();
		}
	}

	public static List<string> OBNGPKDHFAE
	{
		get
		{
			return get_SafeDodgesAnimations();
		}
	}

	public static List<string> GFJEINICPCJ
	{
		get
		{
			return get_EvadeUnsafeDodgesAnimations();
		}
	}

	public static List<string> KGHFEHEKHAB
	{
		get
		{
			return get_AttackMoves();
		}
	}

	public static List<string> HDMGAHBLHPG
	{
		get
		{
			return get_ThrowableIntervals();
		}
	}

	public static List<string> LLOIHGKBNDF
	{
		get
		{
			return get_Throws();
		}
	}

	public static List<TemplateAnimation> AOMGCOLJHFI
	{
		get
		{
			return get_EmergencyDodgesAnimations();
		}
	}

	public static List<TemplateAnimation> EBLLHOIJEJM
	{
		get
		{
			return get_CautiousMovements();
		}
	}

	public static List<TemplateAnimation> AJPAALEAHJP
	{
		get
		{
			return get_EvadeThrowDodges();
		}
	}

	public static List<TemplateAnimation> KPNNOBLPPGM
	{
		get
		{
			return get_RandomizingEnemyAnimation();
		}
	}

	public static List<TemplateAnimation> DLODHPAMJPL
	{
		get
		{
			return get_MissileAnimations();
		}
	}

	public static List<TemplateAnimation> EHHNNINGEKP
	{
		get
		{
			return get_MagicAnimations();
		}
	}

	public static string BKHMBOEFNLL
	{
		get
		{
			return get_DistanceNode();
		}
	}

	public static bool APGFCCPPJPH
	{
		get
		{
			return get_IsShowErrorIfAnimationNotFound();
		}
	}

	public static List<List<global::Pair<TacticalTableHolder, global::Pair<string, string>>>> get_TablesHoldersNew()
	{
		if (DCECBCKPJOD == null)
		{
			DCECBCKPJOD = new List<List<global::Pair<TacticalTableHolder, global::Pair<string, string>>>>();
			for (int i = 0; i < 3; i++)
			{
				DCECBCKPJOD.Add(new List<global::Pair<TacticalTableHolder, global::Pair<string, string>>>());
			}
		}
		return DCECBCKPJOD;
	}

	public static List<Tactic> get_Parameters()
	{
		return PNBAAKIIDGG;
	}

	public static void Load()
	{
		if (DNKLLMKCNPP)
		{
			LLLOJBFMONN.Write("loadGame - loading tactics");
		}
		RefreshParameters();
	}

	public static void Load(List<string> ODODFFKBOEG, List<string> DKDIKAHDOBF)
	{
		if (!DNKLLMKCNPP)
		{
			return;
		}
		RemoveDuplicateSubtypes(ODODFFKBOEG);
		string jIIFFJAJNNN = GameUtils.APCAKCCOMLO.JIIFFJAJNNN;
		ODODFFKBOEG.AddIfNotExist(jIIFFJAJNNN);
		ODODFFKBOEG.AddIfNotExist(string.Empty);
		if (DKDIKAHDOBF.Contains(jIIFFJAJNNN))
		{
			DKDIKAHDOBF.Remove(jIIFFJAJNNN);
		}
		List<string> list = new List<string>();
		list.Add(string.Empty);
		list.Add(GameUtils.APCAKCCOMLO.JIIFFJAJNNN);
		RemoveExept(list);
		KFACNBAPBHE.Clear();
		LLLOJBFMONN.Write("weaponsSubtypes");
		foreach (string item in ODODFFKBOEG)
		{
			LLLOJBFMONN.Write(item);
		}
		foreach (string item2 in ODODFFKBOEG)
		{
			if (KFACNBAPBHE.Contains(item2))
			{
				continue;
			}
			Loadfor(item2, ODODFFKBOEG);
			foreach (string item3 in ODODFFKBOEG)
			{
				if (item2 == item3 && DKDIKAHDOBF.Contains(item2))
				{
					LLLOJBFMONN.Write("Skipped load tactics: {0} - {1}", item2, item3);
				}
				else
				{
					LoadMovementsTablefor(item2, item3);
				}
			}
		}
		LLLOJBFMONN.Write("Old tactic:");
		foreach (string item4 in KFACNBAPBHE)
		{
			LLLOJBFMONN.Write("  - {0}", item4);
		}
		LLLOJBFMONN.Write("New tactic:");
		foreach (string item5 in ODODFFKBOEG)
		{
			LLLOJBFMONN.Write("  - {0}", item5);
		}
		KFACNBAPBHE.Clear();
		KFACNBAPBHE.AddIfNotExist(ODODFFKBOEG);
		LLLOJBFMONN.Write("Available tables:");
		LLLOJBFMONN.Write("  outcometablesforattack:");
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list2 = get_TablesHoldersNew()[0];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item6 in list2)
		{
			item6.First.DLEINJHGIIL();
			if (!item6.First.Empty())
			{
				LLLOJBFMONN.Write("    - {0}/{1}", item6.Second.First, item6.Second.Second);
			}
			else
			{
				LLLOJBFMONN.Write("outcometablesforattack = {0}/{1} *empty*", item6.Second.First, item6.Second.Second);
			}
		}
		LLLOJBFMONN.Write("  movementsTable:");
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list3 = get_TablesHoldersNew()[1];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item7 in list3)
		{
			item7.First.DLEINJHGIIL();
			if (!item7.First.Empty())
			{
				LLLOJBFMONN.Write("    - {0}/{1}", item7.Second.First, item7.Second.Second);
			}
			else
			{
				LLLOJBFMONN.Write("movementsTable = {0}/{1} *empty*", item7.Second.First, item7.Second.Second);
			}
		}
		LLLOJBFMONN.Write("  dodgeTable:");
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list4 = get_TablesHoldersNew()[2];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item8 in list4)
		{
			item8.First.DLEINJHGIIL();
			if (!item8.First.Empty())
			{
				LLLOJBFMONN.Write("    - {0}", item8.Second.First);
			}
			else
			{
				LLLOJBFMONN.Write("dodgeTable = {0} *empty*", item8.Second.First);
			}
		}
	}

	public static void ClearAll()
	{
		ClearTables();
		AttackTablesFrames.Clear();
		ClearAllTacticSettings();
	}

	public static void ClearAllTacticSettings()
	{
		KCLGCKOADLM.Clear();
		PNBAAKIIDGG.Clear();
		MANIJDBMOIH.Clear();
		DMMADDDAALP.Clear();
		CMJBPEBDHMI.Clear();
		DMHKCBDMJKA.Clear();
		KMCGCCBPGCN.Clear();
		DFPGCHKAKOK.Clear();
		FEBACABHOML.Clear();
		OJLFJPBKJAF.Clear();
		BNAGEDKALKO.Clear();
		IJFIGCJJMIK.Clear();
		FMILEEDDEEM.Clear();
		ENGMFIDLPHG.Clear();
		NOFHPKKFIOK.Clear();
		KLKMOILIBBO.Clear();
		JOOELCENMFC.Clear();
		CHECBPGPICK.Clear();
		PJJEHBOPLNB.Clear();
		EPJJKMOFJHC.Clear();
		OLPDJEKFMGJ.Clear();
	}

	public static void ClearTables()
	{
		List<string> lCIGOHHEDGK = new List<string>();
		RemoveExept(lCIGOHHEDGK);
		KFACNBAPBHE.Clear();
	}

	public static string GetTacticsTableName(HDHPLDFCDOF LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case HDHPLDFCDOF.randomAnimation:
			return "RandomAnimation";
		case HDHPLDFCDOF.noneTable:
			return "NoneTable";
		case HDHPLDFCDOF.outcometablesforattack:
			return "AttackTable";
		case HDHPLDFCDOF.movementsTable:
			return "MovementsTable";
		case HDHPLDFCDOF.dodgeTable:
			return "DodgeTable";
		case HDHPLDFCDOF.attackTable:
			return "AttackTableOld";
		case HDHPLDFCDOF.summaryResultTable:
			return "SummaryResultTable";
		case HDHPLDFCDOF.safeTable:
			return "CautiousMovements";
		case HDHPLDFCDOF.quickAttact:
			return "QuickAttack";
		case HDHPLDFCDOF.shiftTable:
			return "ShiftTable";
		case HDHPLDFCDOF.throwTactics:
			return "ThrowTactics";
		case HDHPLDFCDOF.evadeList:
			return "EvadeThrowDodges";
		case HDHPLDFCDOF.block:
			return "Block";
		default:
			return "??????";
		}
	}

	public static bool get_BothBotEnabled()
	{
		return JADMPKACIGJ;
	}

	public static void ParseAnimationList(XmlNode ABPANOKOIEF, List<TemplateAnimation> BMMCGJDICOJ)
	{
		if (ABPANOKOIEF == null)
		{
			return;
		}
		foreach (XmlNode childNode in ABPANOKOIEF.ChildNodes)
		{
			if (childNode.Name == "Animation")
			{
				string gOHIIMFFFJI = childNode.Attributes["Name"].CIPOICEEIBK();
				TemplateAnimation bHIDAHDCPHM = AnimationData.ANEMJNGKFDB(gOHIIMFFFJI);
				if (bHIDAHDCPHM != null)
				{
					BMMCGJDICOJ.Add(bHIDAHDCPHM);
				}
			}
		}
	}

	public static void ParseStringList(XmlNode ABPANOKOIEF, List<string> CHLCLGKFLPP, string CEELFMIPAII = "Animation")
	{
		if (ABPANOKOIEF == null)
		{
			return;
		}
		foreach (XmlNode childNode in ABPANOKOIEF.ChildNodes)
		{
			if (childNode.Name == CEELFMIPAII)
			{
				string item = childNode.Attributes["Name"].CIPOICEEIBK();
				CHLCLGKFLPP.Add(item);
			}
		}
	}

	private static void RefreshParameters()
	{
		string lOBFDOKFJIP = DirectoryController.BECKNKJNFJB("tacticSettings.xml");
		string lOBFDOKFJIP2 = DirectoryController.BECKNKJNFJB("ComputerSettings.xml");
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), lOBFDOKFJIP);
		if (xmlDocument != null)
		{
			TacticsCompiler.CompileTacticsSettings(xmlDocument);
		}
		if (xmlDocument != null)
		{
			RefreshTacticSetParameters(xmlDocument);
		}
		xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), lOBFDOKFJIP2);
		if (xmlDocument == null)
		{
			return;
		}
		XmlNode xmlNode = xmlDocument["Settings"]["TablesReduction"];
		KINPOOFGAGD = xmlNode["MovementsTables"].Attributes["Step"].ParseInt(1);
		EFODNHDEKCL = xmlNode["MovementsTables"].Attributes["Step"].ParseInt(1);
		string text = xmlNode["AttackTables"].Attributes["Frames"].CIPOICEEIBK(string.Empty);
		string[] array = text.Split('|');
		int num = array.Length;
		if (0 < num)
		{
			string[] array2 = array;
			foreach (string iGGFGLLIGCG in array2)
			{
				AttackTablesFrames.Add(iGGFGLLIGCG.ToInt());
			}
		}
		xmlNode = xmlDocument["Settings"]["MovementsTables"]["MovementsMainIterations"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Name == "Animation")
			{
				string item = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				DMHKCBDMJKA.Add(item);
			}
		}
		xmlNode = xmlDocument["Settings"]["MovementsTables"]["MovementsLastIteration"];
		foreach (XmlNode childNode2 in xmlNode.ChildNodes)
		{
			if (childNode2.Name == "Animation")
			{
				string item2 = childNode2.Attributes["Name"].CIPOICEEIBK(string.Empty);
				KMCGCCBPGCN.Add(item2);
			}
		}
		xmlNode = xmlDocument["Settings"]["MissileTables"]["MovementsMainIterations"];
		foreach (XmlNode childNode3 in xmlNode.ChildNodes)
		{
			if (childNode3.Name == "Animation")
			{
				string item3 = childNode3.Attributes["Name"].CIPOICEEIBK(string.Empty);
				HOHPLMFPENC.Add(item3);
			}
		}
		xmlNode = xmlDocument["Settings"]["MissileTables"]["MovementsLastIteration"];
		foreach (XmlNode childNode4 in xmlNode.ChildNodes)
		{
			if (childNode4.Name == "Animation")
			{
				string item4 = childNode4.Attributes["Name"].CIPOICEEIBK(string.Empty);
				BPAPMIPFMGA.Add(item4);
			}
		}
		xmlNode = xmlDocument["Settings"]["MoveLengthIntervals"]["Strict"];
		foreach (XmlNode childNode5 in xmlNode.ChildNodes)
		{
			if (childNode5.Name == "Interval")
			{
				string item5 = childNode5.Attributes["Name"].CIPOICEEIBK(string.Empty);
				DFPGCHKAKOK.Add(item5);
			}
		}
		xmlNode = xmlDocument["Settings"]["MoveLengthIntervals"]["Extended"];
		foreach (XmlNode childNode6 in xmlNode.ChildNodes)
		{
			if (childNode6.Name == "Interval")
			{
				string item6 = childNode6.Attributes["Name"].CIPOICEEIBK(string.Empty);
				FEBACABHOML.Add(item6);
			}
		}
		xmlNode = xmlDocument["Settings"]["OutcomeTables"]["Throws"]["Throws"];
		foreach (XmlNode childNode7 in xmlNode.ChildNodes)
		{
			if (childNode7.Name == "Animation")
			{
				string item7 = childNode7.Attributes["Name"].CIPOICEEIBK(string.Empty);
				NOFHPKKFIOK.Add(item7);
			}
		}
		xmlNode = xmlDocument["Settings"]["OutcomeTables"]["Throws"]["ThrowableIntervals"];
		foreach (XmlNode childNode8 in xmlNode.ChildNodes)
		{
			if (childNode8.Name == "Interval")
			{
				string item8 = childNode8.Attributes["Name"].CIPOICEEIBK(string.Empty);
				ENGMFIDLPHG.Add(item8);
			}
		}
	}

	public static void RefreshTacticSetParameters(XmlNode EELFNMOHGJL)
	{
		JADMPKACIGJ = EELFNMOHGJL["TacticsSettings"]["BothBot"].Attributes["Enabled"].ParseBool();
		XmlNode xmlNode = EELFNMOHGJL["TacticsSettings"]["Tactics"];
		if (xmlNode != null)
		{
			ParseTactics(xmlNode);
		}
		List<string> list = new List<string>();
		XmlNode xmlNode2 = EELFNMOHGJL["TacticsSettings"]["ItemEquivalents"];
		foreach (XmlNode childNode in xmlNode2.ChildNodes)
		{
			if (childNode.Name == "Item")
			{
				string text = childNode.Attributes["Type"].CIPOICEEIBK();
				if (text != "Weapon")
				{
					LLLOJBFMONN.Error("strange item type '{0}'", text);
				}
				string gBCLEDJAOBM = childNode.Attributes["SubType"].CIPOICEEIBK();
				list.Clear();
				foreach (XmlNode childNode2 in childNode.ChildNodes)
				{
					if (childNode2.Name == "Equivalent")
					{
						string text2 = childNode2.Attributes["Type"].CIPOICEEIBK();
						if (text2 != "Weapon")
						{
							LLLOJBFMONN.Error("strange item type '%s'", text2);
						}
						string item = childNode2.Attributes["SubType"].CIPOICEEIBK();
						list.Add(item);
					}
					else
					{
						LLLOJBFMONN.Error("strange xml node '%s'", childNode2.Name);
					}
				}
				KCLGCKOADLM.Add(new global::Pair<string, List<string>>(gBCLEDJAOBM, list));
			}
			else
			{
				LLLOJBFMONN.Error("strange xml node '%s'", childNode.Name);
			}
		}
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["NoDecision"]["Intervals"];
		ParseStringList(xmlNode2, MANIJDBMOIH, "Interval");
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["NoDecision"]["Moves"];
		ParseStringList(xmlNode2, DMMADDDAALP, "Move");
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["UnexpectedMoves"];
		ParseStringList(xmlNode2, CMJBPEBDHMI, "Move");
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["IgnoredEnemyAnimations"];
		ParseStringList(xmlNode2, OJLFJPBKJAF);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["SafeDodges"];
		ParseStringList(xmlNode2, BNAGEDKALKO);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["EmergencyDodges"];
		ParseAnimationList(xmlNode2, KLKMOILIBBO);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["CautiousMovements"];
		ParseAnimationList(xmlNode2, JOOELCENMFC);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["EvadeThrowDodges"];
		ParseAnimationList(xmlNode2, CHECBPGPICK);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["RandomizingEnemyAnimation"];
		ParseAnimationList(xmlNode2, PJJEHBOPLNB);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["MissileAnimations"];
		ParseAnimationList(xmlNode2, EPJJKMOFJHC);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["MagicAnimations"];
		ParseAnimationList(xmlNode2, OLPDJEKFMGJ);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["EvadeUnsafeDodges"];
		ParseStringList(xmlNode2, IJFIGCJJMIK);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["AttackMoves"];
		ParseStringList(xmlNode2, FMILEEDDEEM);
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["DistanceNode"];
		if (xmlNode2 != null)
		{
			_DistanceNode = xmlNode2.Attributes["Name"].CIPOICEEIBK();
		}
		xmlNode2 = EELFNMOHGJL["TacticsSettings"]["Debug"]["ShowErrorIfAnimationNotFound"];
		if (xmlNode2 != null)
		{
			LIPCBILPHEJ = xmlNode2.Attributes["Value"].ParseBool();
		}
	}

	public static void RefreshTacticParamWithRaidData(XmlDocument FJCFBLBNDNG)
	{
		TacticsCompiler.CompileTacticsSettings(FJCFBLBNDNG);
		RefreshTacticSetParameters(FJCFBLBNDNG);
	}

	public static void ParseTactics(XmlNode AFHNINCKJEE)
	{
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name == "Tactic")
			{
				Tactic item = new Tactic(childNode);
				PNBAAKIIDGG.Add(item);
			}
		}
	}

	public static string GetItemEquivalent(string LKBJNLBIDGP)
	{
		foreach (global::Pair<string, List<string>> item in KCLGCKOADLM)
		{
			if (item.First == LKBJNLBIDGP)
			{
				return LKBJNLBIDGP;
			}
			foreach (string item2 in item.Second)
			{
				if (item2 == LKBJNLBIDGP)
				{
					return item.First;
				}
			}
		}
		return LKBJNLBIDGP;
	}

	public static Tactic GetTacticByName(string BHNDJOGLEOI)
	{
		foreach (Tactic item in PNBAAKIIDGG)
		{
			if (item.get_Name() == BHNDJOGLEOI)
			{
				return item;
			}
		}
		return JMGFJNKBJHM;
	}

	public static void AddTableHolder(TacticalTableHolder IOAAHOMGEPI, string KEEMLGNLKPF, string ANCBHPMAAFI, HDHPLDFCDOF GLBPKPEIOKE)
	{
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list = DCECBCKPJOD[(int)GLBPKPEIOKE];
		list.Add(new global::Pair<TacticalTableHolder, global::Pair<string, string>>(IOAAHOMGEPI, new global::Pair<string, string>(KEEMLGNLKPF, ANCBHPMAAFI)));
	}

	public static bool CheckIfTableExists(string LGCMGHAFEDD, HDHPLDFCDOF GLBPKPEIOKE)
	{
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list = DCECBCKPJOD[(int)GLBPKPEIOKE];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item in list)
		{
			if (item.Second.First == LGCMGHAFEDD && item.Second.Second == LGCMGHAFEDD)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckIfTableExists(string NDAJLDOMNLK, string AFKFIEAMFKG, HDHPLDFCDOF GLBPKPEIOKE)
	{
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list = DCECBCKPJOD[(int)GLBPKPEIOKE];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item in list)
		{
			if (item.Second.First == NDAJLDOMNLK && item.Second.Second == AFKFIEAMFKG)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckIfTableExists(List<string> PGHJNFEGLJE, HDHPLDFCDOF GLBPKPEIOKE)
	{
		List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list = DCECBCKPJOD[(int)GLBPKPEIOKE];
		foreach (global::Pair<TacticalTableHolder, global::Pair<string, string>> item in list)
		{
			bool flag = false;
			foreach (string item2 in PGHJNFEGLJE)
			{
				if (item2 == item.Second.First)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			foreach (string item3 in PGHJNFEGLJE)
			{
				if (item3 == item.Second.Second)
				{
					flag2 = true;
					break;
				}
			}
			if (flag && flag2)
			{
				return true;
			}
		}
		return false;
	}

	public static List<string> get_NoDecisionIntervals()
	{
		return MANIJDBMOIH;
	}

	public static List<string> get_NoDecisionMoves()
	{
		return DMMADDDAALP;
	}

	public static List<string> get_UnexpectedMoves()
	{
		return CMJBPEBDHMI;
	}

	public static List<string> get_MovesFirstIteration()
	{
		return DMHKCBDMJKA;
	}

	public static List<string> get_MovesLastIteration()
	{
		return KMCGCCBPGCN;
	}

	public static List<string> get_MissilesFirstIteration()
	{
		return HOHPLMFPENC;
	}

	public static List<string> get_MissilesLastIteration()
	{
		return BPAPMIPFMGA;
	}

	public static List<string> get_MoveLengthIntervalsStrict()
	{
		return DFPGCHKAKOK;
	}

	public static List<string> get_MoveLengthIntervalsExtended()
	{
		return FEBACABHOML;
	}

	public static List<string> get_IgnoredEnemyAnimations()
	{
		return OJLFJPBKJAF;
	}

	public static List<string> get_SafeDodgesAnimations()
	{
		return BNAGEDKALKO;
	}

	public static List<string> get_EvadeUnsafeDodgesAnimations()
	{
		return IJFIGCJJMIK;
	}

	public static List<string> get_AttackMoves()
	{
		return FMILEEDDEEM;
	}

	public static List<string> get_ThrowableIntervals()
	{
		return ENGMFIDLPHG;
	}

	public static List<string> get_Throws()
	{
		return NOFHPKKFIOK;
	}

	public static List<TemplateAnimation> get_EmergencyDodgesAnimations()
	{
		return KLKMOILIBBO;
	}

	public static List<TemplateAnimation> get_CautiousMovements()
	{
		return JOOELCENMFC;
	}

	public static List<TemplateAnimation> get_EvadeThrowDodges()
	{
		return CHECBPGPICK;
	}

	public static List<TemplateAnimation> get_RandomizingEnemyAnimation()
	{
		return PJJEHBOPLNB;
	}

	public static List<TemplateAnimation> get_MissileAnimations()
	{
		return EPJJKMOFJHC;
	}

	public static List<TemplateAnimation> get_MagicAnimations()
	{
		return OLPDJEKFMGJ;
	}

	public static string get_DistanceNode()
	{
		return _DistanceNode;
	}

	public static bool get_IsShowErrorIfAnimationNotFound()
	{
		return LIPCBILPHEJ;
	}

	private static void Loadfor(string LEPELMEGAOE, List<string> LDNMLKGMABH)
	{
		LoadShiftTablesfor(LEPELMEGAOE);
	}

	private static void LoadShiftTablesfor(string LEBFGLIGPOK)
	{
		TacticsArchiver.MFMGMPPALEG(LEBFGLIGPOK);
	}

	private static void LoadMovementsTablefor(string KEEMLGNLKPF, List<string> LDNMLKGMABH)
	{
		foreach (string item in LDNMLKGMABH)
		{
			LoadMovementsTablefor(KEEMLGNLKPF, item);
		}
	}

	private static void LoadMovementsTablefor(string KEEMLGNLKPF, string ANCBHPMAAFI)
	{
		TacticsArchiver.MFMGMPPALEG(KEEMLGNLKPF, ANCBHPMAAFI);
	}

	private static void RemoveExept(List<string> LCIGOHHEDGK)
	{
		HDHPLDFCDOF[] array = new HDHPLDFCDOF[3]
		{
			HDHPLDFCDOF.dodgeTable,
			HDHPLDFCDOF.movementsTable,
			HDHPLDFCDOF.outcometablesforattack
		};
		List<InfoAnimation> list = AnimationData.CCANGHENJAE();
		foreach (InfoAnimation item in list)
		{
			item.OBIBINIEJJE.Clear();
			item.ABNCNNHMLII();
			for (int i = 0; i < array.Length; i++)
			{
				List<global::Pair<List<GroupTables>, string>> list2 = item.NLCLHLIPFFH()[i];
				foreach (global::Pair<List<GroupTables>, string> item2 in list2)
				{
					if (item2.First != null)
					{
						item2.First = null;
						item2.Second = "delete";
					}
				}
				list2.Clear();
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			List<global::Pair<TacticalTableHolder, global::Pair<string, string>>> list3 = get_TablesHoldersNew()[j];
			for (int k = 0; k < list3.Count; k++)
			{
				global::Pair<TacticalTableHolder, global::Pair<string, string>> cCKLNOPEKHO = list3[k];
				if (!cCKLNOPEKHO.First.Empty())
				{
					if (CheckExceptionWeapons(cCKLNOPEKHO.Second.First, cCKLNOPEKHO.Second.Second, LCIGOHHEDGK))
					{
						LLLOJBFMONN.Write("Skipping exeptional weapons (%s/%s) on tactic table clear", cCKLNOPEKHO.Second.First, cCKLNOPEKHO.Second.Second);
					}
					else
					{
						list3.RemoveAt(k);
						k--;
					}
				}
			}
		}
	}

	private static bool CheckExceptionWeapons(string NDAJLDOMNLK, string AFKFIEAMFKG, List<string> LCIGOHHEDGK)
	{
		bool flag = false;
		bool flag2 = false;
		string text = NDAJLDOMNLK;
		foreach (string item in LCIGOHHEDGK)
		{
			if (text == item)
			{
				flag = true;
				break;
			}
		}
		text = AFKFIEAMFKG;
		foreach (string item2 in LCIGOHHEDGK)
		{
			if (text == item2)
			{
				flag2 = true;
				break;
			}
		}
		return flag && flag2;
	}

	private static void RemoveDuplicateSubtypes(List<string> JIGEFEPNCIN)
	{
		for (int i = 0; i < JIGEFEPNCIN.Count; i++)
		{
			for (int j = i + 1; j < JIGEFEPNCIN.Count; j++)
			{
				if (JIGEFEPNCIN[i] == JIGEFEPNCIN[j])
				{
					JIGEFEPNCIN.RemoveAt(j);
					j--;
				}
			}
		}
	}
}
