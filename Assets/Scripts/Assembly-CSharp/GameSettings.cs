using System.Collections.Generic;
using System.Xml;
using Nekki.SF2.GUI.Map;
using UnityEngine;

public static class GameSettings
{
	public class VersionSettings
	{
		public bool DFBLLGKPPFC;

		public bool DJNAALHEMOH;

		public VersionSettings()
		{
			DFBLLGKPPFC = false;
			DJNAALHEMOH = false;
		}

		public bool Empty()
		{
			return !DFBLLGKPPFC && !DJNAALHEMOH;
		}
	}

	private static VersionSettings EAJBNEBGFDP = new VersionSettings();

	private static bool GACDAELBEOD = false;

	private static bool KCIMHHCHABH = false;

	private static List<QualityOption> DGOBOBLPICD = new List<QualityOption>();

	public static bool BKFGGJNNOMK
	{
		get
		{
			return HCAJHNKLLGB();
		}
	}

	public static void IFBKAJPILOI()
	{
		// Local, moddable saves are accepted in every player and in the editor.
		KCIMHHCHABH = false;
	}

	public static void OCIPKAONMOP()
	{
		FDLNFIDKOCH();
		CICMIJDPLHG();
		CJOBEINJLLO();
		NEMPHEKKODK();
		KBAPDJLNCJE();
		GameUtils.MEBABPEMMBE();
		GameUtils.LGEPJJPDNOO();
	}

	private static void FDLNFIDKOCH()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "internalSettings.xml");
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["Settings"];
			if (xmlNode != null)
			{
				BBEPAAOBKBH(xmlNode);
			}
			else
			{
				LLLOJBFMONN.Error("ERROR: GameSettings.LoadInternalSettings - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Error("ERROR: loadInternalSettings - file internalSettings.xml doesn't exist");
		}
	}

	private static void BBEPAAOBKBH(XmlNode BAMDEPGMGEN)
	{
		XmlNode xmlNode = BAMDEPGMGEN["Attributes"];
		if (xmlNode != null)
		{
			GameUtils.BGENALLCKII.Parse(xmlNode);
		}
		XmlNode xmlNode2 = BAMDEPGMGEN["RatingEvaluation"];
		if (xmlNode2 != null)
		{
			ModelParameters.DPIDOBMONPA(xmlNode2);
		}
		XmlNode xmlNode3 = BAMDEPGMGEN["DifficultyEvaluation"];
		if (xmlNode3 != null)
		{
			DifficultyPanel.DifficultyEvaluationParse(xmlNode3);
		}
		GameUtils.AJAFNEIPOJB = BAMDEPGMGEN["SlowMode"].Attributes["Value"].ParseInt(10);
		GameUtils.JMFEGEGENII(BAMDEPGMGEN["SlowMotion"].Attributes["Defense"].CIPOICEEIBK(string.Empty));
		GameUtils.PALADCEPDNI(BAMDEPGMGEN["Avatar"].Attributes["Name"].CIPOICEEIBK("avatar_hero"));
		GameUtils.NCGINDFMIFB(BAMDEPGMGEN["Skeleton"].Attributes["Player"].CIPOICEEIBK("Skeleton"));
		XmlNode xmlNode4 = BAMDEPGMGEN["DefaultItems"];
		if (xmlNode4 != null)
		{
			GameUtils.EBNJAECDDIM(xmlNode4);
		}
		GameUtils.NIPABEEAMHJ = BAMDEPGMGEN["Location"].Attributes["Name"].CIPOICEEIBK("dojo");
		GameUtils.AKPBNLKFONO.Parse(BAMDEPGMGEN["Tutorial"]);
		GameUtils.KEFHKHCNBOK = BAMDEPGMGEN["PivotNode"].Attributes["Name"].CIPOICEEIBK("NPivot");
		GameUtils.FPBFDNBDDIE = BAMDEPGMGEN["SellItems"].Attributes["Value"].ParseFloat(0.5f);
		GameUtils.BOGEEJGMHON(BAMDEPGMGEN["Combo"].Attributes["MinHits"].ParseInt(3));
		GameUtils.LKAJIAEEFDL(BAMDEPGMGEN["Combo"].Attributes["Time"].ParseInt(90));
		GameUtils.BBBPHLNOOPN(BAMDEPGMGEN["Announcements"].Attributes["Time"].ParseInt(60));
		GameUtils.NCADEABFAFF(BAMDEPGMGEN["HotGroundTimer"].Attributes["Time"].ParseInt(90));
		PhysicsController.Parse(BAMDEPGMGEN["Physics"]);
		GameUtils.FJNOCJPPJPF(BAMDEPGMGEN["Great"].Attributes["MaxHealth"].ParseFloat(0.3f));
		GameUtils.MEPHOOCEOCI(BAMDEPGMGEN["DamageFactor"].Attributes["Base"].ParseFloat());
		GameUtils.KMOPHHBPOLM(BAMDEPGMGEN["DamageFactor"].Attributes["MaxValue"].ParseInt(20000));
		GameUtils.OKMDBKGNGBJ(BAMDEPGMGEN["DamageFactor"].Attributes["Attribute"].CIPOICEEIBK(string.Empty));
		XmlNode hKPPBKPJOEO = BAMDEPGMGEN["BlockDamageFactor"];
		GameUtils.DAMKDJINILI().Parse(hKPPBKPJOEO);
		XmlNode hKPPBKPJOEO2 = BAMDEPGMGEN["CriticalHit"]["Damage"];
		GameUtils.IOGOPCABLON().Parse(hKPPBKPJOEO2);
		GameUtils.FNCAJMBJJBN(BAMDEPGMGEN["BlockDefense"].Attributes["Attribute"].CIPOICEEIBK(string.Empty));
		GameUtils.BGJPLNFFEOB = BAMDEPGMGEN["DamageDoublingRange"].Attributes["Value"].ParseFloat();
		GameUtils.ENMCFJCNKIG(BAMDEPGMGEN["ResistanceDoublingRange"].Attributes["Value"].ParseFloat());
		GameUtils.LIHCDMIOKKM((BAMDEPGMGEN["StartingMagic"] == null) ? null : BAMDEPGMGEN["StartingMagic"].Attributes["Attribute"].CIPOICEEIBK(string.Empty));
		GameUtils.PFBPLLNHLFB((BAMDEPGMGEN["Power"] == null) ? 10 : BAMDEPGMGEN["Power"].Attributes["Max"].ParseInt(10));
		GameUtils.DIJOCFEFHAK = ((BAMDEPGMGEN["Power"] == null) ? 600 : BAMDEPGMGEN["Power"].Attributes["TimeMax"].ParseInt(600));
		GameUtils.LJGHIDNLJHC((BAMDEPGMGEN["LifeBar"] == null) ? 0f : BAMDEPGMGEN["LifeBar"].Attributes["Value"].ParseFloat());
		GameUtils.LJNCELBEHGN = ((BAMDEPGMGEN["PushRetantionTime"] == null) ? 172800 : BAMDEPGMGEN["PushRetantionTime"].Attributes["Value"].ParseInt(172800));
		XmlNode xmlNode5 = BAMDEPGMGEN["OutdateLevels"];
		if (xmlNode5 != null)
		{
			GameUtils.HPEBEOMLHKF.Parse(xmlNode5);
		}
		XmlNode xmlNode6 = BAMDEPGMGEN["AlignTargetAttributes"];
		if (xmlNode6 != null)
		{
			GameUtils.FPIDOGKOPGC.Clear();
			GameUtils.AlignTargetAttribute.Parse(xmlNode6, GameUtils.FPIDOGKOPGC);
		}
		GameUtils.JOODENKAECE = BAMDEPGMGEN["CounterPunches"].Attributes["Value"].ParseInt(2);
		XmlNode hKPPBKPJOEO3 = BAMDEPGMGEN["RewardsPrize"];
		GameUtils.AAKJKANGFMJ.Parse(hKPPBKPJOEO3);
		XmlNode hKPPBKPJOEO4 = BAMDEPGMGEN["CriticalHit"];
		GameUtils.HHCEIEOOHCJ.Parse(hKPPBKPJOEO4);
		XmlNode hKPPBKPJOEO5 = BAMDEPGMGEN["HitEffects"];
		GameUtils.OCMEOOKALHM().Parse(hKPPBKPJOEO5);
		XmlNode hKPPBKPJOEO6 = BAMDEPGMGEN["Shock"];
		GameUtils.APCAKCCOMLO.Parse(hKPPBKPJOEO6);
		XmlNode hKPPBKPJOEO7 = BAMDEPGMGEN["Camera"];
		GameUtils.LEPANPKBBKI().Parse(hKPPBKPJOEO7);
		GameUtils.JOEMCCADMON.Parse(BAMDEPGMGEN["Supports"]);
		XmlNode hKPPBKPJOEO8 = BAMDEPGMGEN["Shop"];
		GameUtils.JNDLCLLIMMM.Parse(hKPPBKPJOEO8);
		XmlNode hKPPBKPJOEO9 = BAMDEPGMGEN["Currencies"];
		GameUtils.AJDKHINLIDI.Parse(hKPPBKPJOEO9);
		XmlNode eBLIGDMALEA = BAMDEPGMGEN["Resistances"];
		GameUtils.JNIMKHKGPHE.Parse(eBLIGDMALEA);
		XmlNode hKPPBKPJOEO10 = BAMDEPGMGEN["BarScales"];
		GameUtils.NPHEOMBNOLK.Parse(hKPPBKPJOEO10);
		XmlNode hKPPBKPJOEO11 = BAMDEPGMGEN["Magic"];
		GameUtils.DILKHIFCCGD.Parse(hKPPBKPJOEO11);
		XmlNode eBLIGDMALEA2 = BAMDEPGMGEN["AchievementCounter"];
		GameUtils.OJNHPHEPFLI.Parse(eBLIGDMALEA2);
		XmlNode hKPPBKPJOEO12 = BAMDEPGMGEN["Regeneration"];
		GameUtils.DMLPOANHHFI().Parse(hKPPBKPJOEO12);
		XmlNode hKPPBKPJOEO13 = BAMDEPGMGEN["Lifesteal"];
		GameUtils.PPAEHBGNDNF().Parse(hKPPBKPJOEO13);
		GameUtils.CDILOOACLKK = BAMDEPGMGEN["FrameRate"].Attributes["Value"].ParseInt(60);
		BasicGUI.Parse(BAMDEPGMGEN["GUI"]["Basic"]);
		MapGUI.Parse(BAMDEPGMGEN["GUI"]["Map"]);
		FightGUI.Parse(BAMDEPGMGEN["GUI"]["Fight"]);
		ProfileGUI.Parse(BAMDEPGMGEN["GUI"]["Profile"]);
		InternetController.Parse(BAMDEPGMGEN["Internet"]);
		if (!Debug.isDebugBuild)
		{
			GameUtils.GLHMHHIADMK = false;
		}
		else
		{
			GameUtils.GLHMHHIADMK = BAMDEPGMGEN["AlwaysMagicMode"].Attributes["Value"].ParseBool();
		}
		GameUtils.GKOEGHLGPPE = BAMDEPGMGEN["DailyDebugMode"].Attributes["Value"].ParseBool();
		GameUtils.DailyDebugTime = BAMDEPGMGEN["DailyDebugTime"].Attributes["Value"].ParseInt();
		SystemProperties.NHIDOHIJMBG(GameUtils.CDILOOACLKK);
		HMKBHNKCDFJ(BAMDEPGMGEN["QualityOptions"]);
		XmlNode xmlNode7 = BAMDEPGMGEN["Aspects"];
		if (xmlNode7 != null)
		{
			GameUtils.ParseAspects(xmlNode7);
		}
		XmlNode xmlNode8 = BAMDEPGMGEN["Aspect"];
		if (xmlNode8 != null)
		{
			GameUtils.CFHKBAEOOBK(xmlNode8);
		}
		GameUtils.KLHOKMCALFM();
		XmlNode xmlNode9 = BAMDEPGMGEN["StyleLevels"];
		if (xmlNode9 != null)
		{
			GameUtils.NIPBIAGMAOD.Parse(xmlNode9);
		}
		GameUtils.JNOGEPFLLDM = BAMDEPGMGEN["MaximumExperience"].Attributes["Value"].ParseUint(30000000u);
	}

	public static void IIGOJINCIIF()
	{
		bool flag = false;
		EAJBNEBGFDP.DFBLLGKPPFC = false;
		EAJBNEBGFDP.DJNAALHEMOH = false;
		VersionContainer pAMHFPMEPCH = new VersionContainer();
		VersionContainer pAMHFPMEPCH2 = new VersionContainer();
		VersionContainer pAMHFPMEPCH3 = new VersionContainer();
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "versionController.xml");
		XmlNode xmlNode = ((xmlDocument == null) ? null : xmlDocument["Versions"]["Version"]);
		if (xmlNode != null)
		{
			pAMHFPMEPCH.SetVersion(xmlNode.Attributes["Value"].CIPOICEEIBK(string.Empty));
			pAMHFPMEPCH.DPHPJFGOLMJ(0);
		}
		XmlDocument xmlDocument2 = XmlUtils.AIFIAKNJMHG(SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
		if (xmlDocument2 == null)
		{
			xmlDocument2 = XmlUtils.AIFIAKNJMHG(SF2Paths.APHDBIBDMDG(), Constants.GHKPPHAAMBL);
			if (xmlDocument2 != null)
			{
				string kPFELJFPGHJ = string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
				XmlUtils.ONLDJNLKKAL(xmlDocument2, kPFELJFPGHJ);
			}
		}
		if (xmlDocument2 == null)
		{
			flag = true;
			EAJBNEBGFDP.DFBLLGKPPFC = true;
		}
		else if (xmlDocument2["Root"]["Versions"] != null)
		{
			EAJBNEBGFDP.DJNAALHEMOH = true;
		}
		if (EAJBNEBGFDP.DJNAALHEMOH)
		{
			string aHLPODLKBEP = xmlDocument2["Root"]["Versions"]["Version"].Attributes["Value"].CIPOICEEIBK(string.Empty);
			string aHLPODLKBEP2 = xmlDocument2["Root"]["Versions"]["DataVersion"].Attributes["Value"].CIPOICEEIBK(string.Empty);
			pAMHFPMEPCH2.SetVersion(aHLPODLKBEP);
			pAMHFPMEPCH3.SetVersion(aHLPODLKBEP2);
			if (VersionContainer.CGMHEDJDOEK(pAMHFPMEPCH, pAMHFPMEPCH2))
			{
				flag = true;
			}
		}
		if (flag)
		{
			GACDAELBEOD = true;
		}
		SystemProperties.BFBMCAALLHF(pAMHFPMEPCH, pAMHFPMEPCH3);
	}

	public static void AMOMFPOENBF()
	{
		if (GACDAELBEOD)
		{
			VersionContainer pAMHFPMEPCH = SystemProperties.KCJMMIEBLHL();
			VersionContainer pAMHFPMEPCH2 = new VersionContainer();
			string oNEIGMLOGDC = ((!EAJBNEBGFDP.DFBLLGKPPFC) ? SF2Paths.APHDBIBDMDG() : SF2Paths.KKIDGPBOBNI());
			XmlDocument xmlDocument = null;
			xmlDocument = ((!EAJBNEBGFDP.DFBLLGKPPFC) ? XmlUtils.OpenXMLDocument(oNEIGMLOGDC, Constants.OJMIJINKBPJ) : XmlUtils.OpenXMLDocument(oNEIGMLOGDC, "usersDefault.xml", XmlUtils.EBLFEPIOMOL.Normal, true, XmlCryptoUtils.NNLGALNDJCL()));
			if (xmlDocument != null)
			{
				XmlNode xmlNode = xmlDocument["Root"]["Versions"];
				if (xmlNode != null)
				{
					string value = pAMHFPMEPCH.ToString();
					xmlNode["Version"].Attributes["Value"].Value = value;
					string aHLPODLKBEP = xmlNode["DataVersion"].Attributes["Value"].CIPOICEEIBK(string.Empty);
					pAMHFPMEPCH2.SetVersion(aHLPODLKBEP);
				}
				SystemProperties.BFBMCAALLHF(pAMHFPMEPCH, pAMHFPMEPCH2);
				string kPFELJFPGHJ = string.Format("{0}/{1}", SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
				XmlUtils.ONLDJNLKKAL(xmlDocument, kPFELJFPGHJ);
			}
			else
			{
				LLLOJBFMONN.Error("GameSettings.InitVersion userXML is null");
			}
			GeneralConfig.LOGLOMLEHFI();
		}
		GACDAELBEOD = false;
	}

	public static string DGBHBMFEOAA()
	{
		XmlDocument xmlDocument = XmlUtils.AIFIAKNJMHG(SF2Paths.APHDBIBDMDG(), Constants.OJMIJINKBPJ);
		if (xmlDocument != null)
		{
			XmlDocument xmlDocument2 = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "versionController.xml");
			if (xmlDocument2 != null)
			{
				return xmlDocument2["Versions"]["Version"].Attributes["Value"].CIPOICEEIBK();
			}
		}
		return string.Empty;
	}

	public static void AIKBOKDPNOA()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "internalSettings.xml");
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["Settings"];
			if (xmlNode != null)
			{
				XmlNode xmlNode2 = xmlNode["AssemblySettings"];
				if (xmlNode2 != null)
				{
					AssemblyController.Parse(xmlNode2);
				}
				else
				{
					LLLOJBFMONN.Write("ERROR: loadInternalSettings - AssemblySettings section is missing");
				}
			}
			else
			{
				LLLOJBFMONN.Write("ERROR: loadInternalSettings - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Write("ERROR: loadInternalSettings - file \"{0}\" doesn't exist", "internalSettings.xml");
		}
	}

	public static void LNNLDPLDABI()
	{
		for (int i = 0; i < DGOBOBLPICD.Count; i++)
		{
			DGOBOBLPICD[i].IOHJMJKLIOD();
		}
	}

	private static void CICMIJDPLHG()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "tacticSettings.xml");
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["TacticsSettings"];
			if (xmlNode != null)
			{
				CPOGFCHCHFD(xmlNode);
			}
			else
			{
				LLLOJBFMONN.Write("ERROR: loadInternalSettings - wrong file");
			}
		}
	}

	private static void NEMPHEKKODK()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "CharacterProgress.xml");
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["Progress"];
			if (xmlNode != null)
			{
				ABPEINBKMNL(xmlNode);
			}
			else
			{
				LLLOJBFMONN.Write("ERROR: loadCharacterProgress - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Write("ERROR: loadCharacterProgress - file CharacterProgress.xml doesn't exist");
		}
	}

	private static void KBAPDJLNCJE()
	{
		XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(), "Achievements.xml");
		if (xmlDocument != null)
		{
			XmlNode xmlNode = xmlDocument["Achievements"];
			if (xmlNode != null)
			{
				GLLEJFLIMNN(xmlNode);
			}
			else
			{
				LLLOJBFMONN.Write("ERROR: loadAchievements - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Write("ERROR: loadAchievements - file Achievements.xml doesn't exist");
		}
	}

	private static void CJOBEINJLLO()
	{
		XmlDocument EELFNMOHGJL = null;
		PerksCompiler.CompilePerks(ref EELFNMOHGJL, string.Format("{0}/{1}", SF2Paths.KKIDGPBOBNI(), "perks.xml"));
		if (EELFNMOHGJL != null)
		{
			XmlNode xmlNode = EELFNMOHGJL["Perks"];
			if (xmlNode != null)
			{
				GameUtils.FDEJIIDIPBI.Parse(xmlNode);
			}
			else
			{
				LLLOJBFMONN.Error("ERROR: GameSettings.LoadPerks - wrong file");
			}
		}
		else
		{
			LLLOJBFMONN.Error("ERROR: LoadPerks - file perks.xml doesn't exist");
		}
	}

	private static void GLLEJFLIMNN(XmlNode node)
	{
		GameUtils.HHLEKNNJGMJ.Parse(node);
	}

	private static void ABPEINBKMNL(XmlNode BAMDEPGMGEN)
	{
		XmlNode eBLIGDMALEA = BAMDEPGMGEN["Thresholds"];
		XmlNode xmlNode = BAMDEPGMGEN["LotteryThresholds"];
		XmlNode hKPPBKPJOEO = BAMDEPGMGEN["Perks"];
		XmlNode hKPPBKPJOEO2 = BAMDEPGMGEN["LevelAttributeGain"];
		XmlNode hKPPBKPJOEO3 = BAMDEPGMGEN["StartingAttributes"];
		XmlNode hKPPBKPJOEO4 = BAMDEPGMGEN["PerkTree"];
		XmlNode hKPPBKPJOEO5 = BAMDEPGMGEN["CurrencyBaseValues"];
		XmlNode hKPPBKPJOEO6 = BAMDEPGMGEN["MoneyBaseValues"];
		GameUtils.HHONBOCJBLB.Parse(eBLIGDMALEA);
		GameUtils.FDEJIIDIPBI.NLLMCPOPFCI(hKPPBKPJOEO);
		GameUtils.MKHOLKGKNID.Parse(hKPPBKPJOEO2);
		GameUtils.KJJBEHBGKMK.Parse(hKPPBKPJOEO3);
		GameUtils.KIGEPCLPEIE.Parse(hKPPBKPJOEO5);
		GameUtils.NFJEPNHJPEE.Parse(hKPPBKPJOEO6);
		PerkTree.GBPBIPFIOJH().Parse(hKPPBKPJOEO4);
	}

	private static void CPOGFCHCHFD(XmlNode node)
	{
		GameUtils.BJACOFCAHPD.Parse(node["Random"]);
	}

	public static bool HCAJHNKLLGB()
	{
		return KCIMHHCHABH;
	}

	private static void HMKBHNKCDFJ(XmlNode node)
	{
		DGOBOBLPICD.Clear();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			QualityOption item = new QualityOption(childNode);
			DGOBOBLPICD.Add(item);
		}
	}
}
