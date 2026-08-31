using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Shop;
using Eclipse.Underworld.Diagnostics;
using UnityEngine;

public static class GameUtils
{
	public enum FGOILIHAKKD
	{
		STEP_DIALOGS_NONE = 0,
		STEP_DIALOGS_WIN_BODYGARD_1 = 1,
		STEP_DIALOGS_LOSS_BODYGARD_1 = 2,
		STEP_DIALOGS_TOURNAMENT = 3,
		STEP_DIALOGS_WIN_TOURNAMENT_1 = 4,
		STEP_DIALOGS_WIN_BODYGARD_2 = 5,
		STEP_DIALOGS_LEVEL_LOSS = 6,
		STEP_DIALOGS_LOSS_BODYGARD_2 = 7,
		STEP_DIALOGS_LOSS_TOURNAMENT_ARMOR = 8,
		STEP_DIALOGS_LOSS_TOURNAMENT_HELMET = 9,
		STEP_DIALOGS_TRAINING_FIGHT = 10,
		STEP_DIALOGS_WELLCOM_TUTORIAL = 11,
		STEP_DIALOGS_FAMILIARTY_GIRL = 12,
		STEP_DIALOGS_FAMILIARTY_PLEASE = 13,
		STEP_DIALOGS_FAMILIARTY_GIRL_END = 14,
		STEP_DIALOGS_START_BODYGARD_1 = 15,
		STEP_DIALOGS_BUY_KNIVES = 16,
		STEP_DIALOGS_NOT_ENOUGH_RUBY = 17,
		STEP_DIALOGS_NOT_ENOUGH_GOLD = 18,
		STEP_DIALOGS_NOT_ENOUGH_LEVEL = 19,
		STEP_DIALOGS_CHALLENGE_REQUIREMENTS_FAIL = 20,
		STEP_DIALOGS_NOT_NETWORK = 21,
		STEP_DIALOGS_TUTORIAL_END = 22
	}

	public enum LMIELHAGGFI
	{
		NEXT_DAY = 0,
		AFTER_NEXT_DAY = 1,
		THIS_DAY = 2,
		BEFORE_THIS_DAY = 3
	}

	public class ZoomEffect
	{
		public int OFJCKMNLAEP;

		public int BJDFMKOCNBN;

		public float AFBPPNDBMEC;

		public float ALOKJEILMLK;

		public float JCNPAOMNJCL;
	}

	public class HitEffect
	{
		public string Type;

		public int NHKPODHHDPF;

		public int OFJCKMNLAEP;

		public float FMICELIGLPG;

		public float PPKAMOILNLN;

		public float KFEMKHHANDC;

		public float GGJBPLHAHFH;
	}

	public class HitEffects
	{
		public List<HitEffect> EOPFGIDLHKP = new List<HitEffect>();

		public void Parse(XmlNode node)
		{
			EOPFGIDLHKP.Clear();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				HitEffect pIHIIMOOICM = new HitEffect();
				pIHIIMOOICM.Type = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
				pIHIIMOOICM.NHKPODHHDPF = childNode.Attributes["PauseTime"].ParseInt();
				pIHIIMOOICM.OFJCKMNLAEP = childNode.Attributes["EffectTime"].ParseInt();
				pIHIIMOOICM.FMICELIGLPG = childNode.Attributes["AmplitudeX"].ParseFloat();
				pIHIIMOOICM.PPKAMOILNLN = childNode.Attributes["AmplitudeY"].ParseFloat();
				pIHIIMOOICM.KFEMKHHANDC = childNode.Attributes["FrequencyX"].ParseFloat();
				pIHIIMOOICM.GGJBPLHAHFH = childNode.Attributes["FrequencyY"].ParseFloat();
				EOPFGIDLHKP.Add(pIHIIMOOICM);
			}
		}

		public HitEffect PCGEIDLBEJM(string LFLGCDNKNJI)
		{
			foreach (HitEffect item in EOPFGIDLHKP)
			{
				if (item.Type == LFLGCDNKNJI)
				{
					return item;
				}
			}
			return null;
		}
	}

	public class AlignTargetAttribute
	{
		public string Name;

		public float Value;

		public static float GetValue(string name)
		{
			return GetValue(FPIDOGKOPGC, name);
		}

		public static float GetValue(List<AlignTargetAttribute> BBNKIBKPBLO, string name)
		{
			for (int i = 0; i < BBNKIBKPBLO.Count; i++)
			{
				if (BBNKIBKPBLO[i].Name == name)
				{
					return BBNKIBKPBLO[i].Value;
				}
			}
			return 0f;
		}

		public static int Parse(XmlNode AFHNINCKJEE, List<AlignTargetAttribute> OEMALIFPGPO)
		{
			int count = OEMALIFPGPO.Count;
			foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
			{
				AlignTargetAttribute kDDAFBJDOMN = new AlignTargetAttribute();
				kDDAFBJDOMN.Parse(childNode);
				OEMALIFPGPO.Add(kDDAFBJDOMN);
			}
			return OEMALIFPGPO.Count - count;
		}

		private void Parse(XmlNode AFHNINCKJEE)
		{
			Name = XmlUtils.ParseString(AFHNINCKJEE.Attributes["Name"]);
			Value = XmlUtils.ParseFloat(AFHNINCKJEE.Attributes["Value"]);
		}
	}

	public class CameraSettigs
	{
		public string MEIHGLKHLFC;

		public string MNDFNOCCOKI;

		public float PHKGOBGNDEC;

		public float KKPKKIJFFMP;

		public float IMHPAHJDAFP;

		public void Parse(XmlNode node)
		{
			MEIHGLKHLFC = node["CameraSettings"].Attributes["CameraNode"].CIPOICEEIBK("COM");
			MNDFNOCCOKI = node["CameraSettings"].Attributes["BindingNode"].CIPOICEEIBK("NPivot");
			PHKGOBGNDEC = node["CameraSettings"].Attributes["BindingLength"].ParseFloat();
			IMHPAHJDAFP = node["CameraSettings"].Attributes["MaxWidth"].ParseFloat(-1f);
			KKPKKIJFFMP = node["CameraSettings"].Attributes["MaxWidthDelta"].ParseFloat();
		}
	}

	public class BaseSettigs
	{
		public string Attribute;

		public float Base;

		public void Parse(XmlNode node)
		{
			Attribute = node.Attributes["Attribute"].CIPOICEEIBK("COM");
			Base = node.Attributes["Base"].ParseFloat();
		}
	}

	public class KDFMEPMFMEE
	{
		public string Name = string.Empty;

		public string Url = string.Empty;
	}

	public class SupportChoiceStruct
	{
		public List<KDFMEPMFMEE> NOCABEGBMCN = new List<KDFMEPMFMEE>();

		public void Parse(XmlNode IKGBGEEMPCD)
		{
			NOCABEGBMCN.Clear();
			foreach (XmlNode childNode in IKGBGEEMPCD.ChildNodes)
			{
				KDFMEPMFMEE kDFMEPMFMEE = new KDFMEPMFMEE();
				kDFMEPMFMEE.Name = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				kDFMEPMFMEE.Url = childNode.Attributes["Url"].CIPOICEEIBK(string.Empty);
				NOCABEGBMCN.Add(kDFMEPMFMEE);
			}
		}

		public KDFMEPMFMEE IBEAICDEEBB(string name)
		{
			foreach (KDFMEPMFMEE item in NOCABEGBMCN)
			{
				if (item.Name == name)
				{
					return item;
				}
			}
			return null;
		}

		public string EBCODADFJLB(string name, string JAGNJBDLEMF)
		{
			KDFMEPMFMEE kDFMEPMFMEE = IBEAICDEEBB(name);
			if (kDFMEPMFMEE == null)
			{
				kDFMEPMFMEE = IBEAICDEEBB(JAGNJBDLEMF);
			}
			return (kDFMEPMFMEE == null) ? string.Empty : kDFMEPMFMEE.Url;
		}
	}

	public class SlidersIndexStruct
	{
		public Dictionary<string, int> NPJKGNIKDGP = new Dictionary<string, int>();

		public void KJIGJEBMILC(string name, int index)
		{
			NPJKGNIKDGP[name] = index;
		}

		public int MILAHFHNIIP(string name)
		{
			int result = -1;
			foreach (KeyValuePair<string, int> item in NPJKGNIKDGP)
			{
				if (item.Key == name)
				{
					result = item.Value;
					break;
				}
			}
			if (name == "SHOP_RUBY_SLIDER")
			{
				result = 4;
			}
			else if (name == "SHOP_FREE_SLIDER")
			{
				result = 1;
			}
			return result;
		}
	}

	public class Currencies
	{
		private List<GameCurrency> DNPGEMMDPNN = new List<GameCurrency>();

		public void Parse(XmlNode node)
		{
			DNPGEMMDPNN.Clear();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				GameCurrency item = new GameCurrency(childNode);
				DNPGEMMDPNN.Add(item);
			}
		}

		public void NIHPGJACOFM(XmlNode node)
		{
			GameCurrency cJJOFMHLFFM = new GameCurrency(node);
			bool flag = true;
			foreach (GameCurrency item in DNPGEMMDPNN)
			{
				if (item.Name == cJJOFMHLFFM.Name)
				{
					item.DHCNGGCOONP(cJJOFMHLFFM);
					flag = false;
				}
			}
			if (flag)
			{
				DNPGEMMDPNN.Add(cJJOFMHLFFM);
			}
		}

		public void FCDLIEFIIGG(GameCurrency KHBNGFMPEBG)
		{
			DNPGEMMDPNN.Add(KHBNGFMPEBG);
		}

		public List<GameCurrency> IIAPDCECFCN()
		{
			return DNPGEMMDPNN;
		}

		public GameCurrency ICFINJLNCPM(string name)
		{
			foreach (GameCurrency item in DNPGEMMDPNN)
			{
				if (item.Name == name)
				{
					return item;
				}
			}
			return null;
		}
	}

	public class AALKFCCIGJJ
	{
		private List<GameResistance> OEJCIFGACNG = new List<GameResistance>();

		public List<GameResistance> KMBOBPOLEJL
		{
			get
			{
				return DALPGLAFJNJ();
			}
		}

		public List<GameResistance> DALPGLAFJNJ()
		{
			return OEJCIFGACNG;
		}

		public void Parse(XmlNode EBLIGDMALEA)
		{
			OEJCIFGACNG.Clear();
			foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
			{
				GameResistance item = new GameResistance(childNode);
				OEJCIFGACNG.Add(item);
			}
		}

		public GameResistance NDMEGBEFBPJ(string name)
		{
			foreach (GameResistance item in OEJCIFGACNG)
			{
				if (item.Name == name)
				{
					return item;
				}
			}
			return null;
		}
	}

	public class AchievementCounters
	{
		public Dictionary<string, Counter> LLMLCLKNAAN = new Dictionary<string, Counter>();

		public Dictionary<string, Counter> HCNBIOHIMKD = new Dictionary<string, Counter>();

		public Dictionary<string, Counter> DJLALLBICPB = new Dictionary<string, Counter>();

		public Dictionary<string, Counter> EJBPPKHILBF = new Dictionary<string, Counter>();

		public void Parse(XmlNode EBLIGDMALEA)
		{
			LLMLCLKNAAN.Clear();
			HCNBIOHIMKD.Clear();
			DJLALLBICPB.Clear();
			EJBPPKHILBF.Clear();
			foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
			{
				Counter jLOIMNNHFKH = new Counter(childNode);
				LLMLCLKNAAN[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
				switch (jLOIMNNHFKH.NHDPMIGHKPF)
				{
				case Counter.NENOEMHAEFH.ECLIPSE_MODE:
					DJLALLBICPB[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
					break;
				case Counter.NENOEMHAEFH.NORMAL_MODE:
					HCNBIOHIMKD[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
					break;
				case Counter.NENOEMHAEFH.RAID_MODE:
					EJBPPKHILBF[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
					break;
				case Counter.NENOEMHAEFH.NONE_MODE:
					DJLALLBICPB[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
					HCNBIOHIMKD[jLOIMNNHFKH.Name] = jLOIMNNHFKH;
					break;
				}
			}
		}

		public void AEPHNNABOEK()
		{
			foreach (KeyValuePair<string, Counter> item in LLMLCLKNAAN)
			{
				item.Value.AEPHNNABOEK();
			}
		}

		public Dictionary<string, Counter> ECMIANLOLHM(Counter.NENOEMHAEFH NMMPBADCFHK)
		{
			switch (NMMPBADCFHK)
			{
			case Counter.NENOEMHAEFH.ECLIPSE_MODE:
				return DJLALLBICPB;
			case Counter.NENOEMHAEFH.RAID_MODE:
				return EJBPPKHILBF;
			default:
				return HCNBIOHIMKD;
			}
		}

		public Dictionary<string, Counter> ECMIANLOLHM(FightList KGKDKENMAOA)
		{
			if (KGKDKENMAOA != null && KGKDKENMAOA.get_Type() == BattleType.FightRaid)
			{
				return EJBPPKHILBF;
			}
			Counter.NENOEMHAEFH nMMPBADCFHK = ((!ListSF.CCDKHLAMKKO().JPMPIDFGCJL()) ? Counter.NENOEMHAEFH.NORMAL_MODE : Counter.NENOEMHAEFH.ECLIPSE_MODE);
			return ECMIANLOLHM(nMMPBADCFHK);
		}

		public Dictionary<string, Counter> EFMDONOJJNH(string LFLGCDNKNJI, Counter.NENOEMHAEFH NMMPBADCFHK)
		{
			Dictionary<string, Counter> dictionary = new Dictionary<string, Counter>();
			Dictionary<string, Counter> dictionary2 = ECMIANLOLHM(NMMPBADCFHK);
			foreach (KeyValuePair<string, Counter> item in dictionary2)
			{
				Counter value = item.Value;
				if (value.Type == LFLGCDNKNJI)
				{
					dictionary[item.Key] = value;
				}
			}
			return dictionary;
		}

		public Counter CBGAEFLNGAC(string LMBBKNMNKOB)
		{
			for (int i = 0; i < 3; i++)
			{
				Counter.NENOEMHAEFH nMMPBADCFHK = (Counter.NENOEMHAEFH)i;
				Dictionary<string, Counter> dictionary = ECMIANLOLHM(nMMPBADCFHK);
				foreach (KeyValuePair<string, Counter> item in dictionary)
				{
					Counter value = item.Value;
					AchievCounter iFDAFNGCIBP = HHLEKNNJGMJ.KJPLIHEMLJL(value.Name);
					if (iFDAFNGCIBP == null)
					{
						continue;
					}
					List<Achievement> fOICCCGPCMJ = iFDAFNGCIBP.FOICCCGPCMJ;
					bool flag = false;
					for (int j = 0; j < fOICCCGPCMJ.Count; j++)
					{
						Achievement jNPIOKEKMII = fOICCCGPCMJ[j];
						if (jNPIOKEKMII.Name == LMBBKNMNKOB)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					return value;
				}
			}
			return null;
		}
	}

	public class AchievCounters
	{
		public List<AchievCounter> MDNKEAFGAOB = new List<AchievCounter>();

		public void Parse(XmlNode EBLIGDMALEA)
		{
			MDNKEAFGAOB.Clear();
			foreach (XmlNode childNode in EBLIGDMALEA.ChildNodes)
			{
				AchievCounter item = new AchievCounter(childNode);
				MDNKEAFGAOB.Add(item);
			}
		}

		public AchievCounter KJPLIHEMLJL(string name)
		{
			foreach (AchievCounter item in MDNKEAFGAOB)
			{
				if (item.Name == name)
				{
					return item;
				}
			}
			return null;
		}

		public Achievement ABNAODNDHDM(string name)
		{
			foreach (AchievCounter item in MDNKEAFGAOB)
			{
				foreach (Achievement item2 in item.FOICCCGPCMJ)
				{
					if (item2.Name == name)
					{
						return item2;
					}
				}
			}
			return null;
		}

		public List<global::Pair<Achievement, int>> DLACNJLPKBK(List<string> EAHPNCOEHJG)
		{
			List<global::Pair<Achievement, int>> list = new List<global::Pair<Achievement, int>>();
			foreach (string item in EAHPNCOEHJG)
			{
				AchievCounter iFDAFNGCIBP = HHLEKNNJGMJ.KJPLIHEMLJL(item);
				RosterAchievCounter cKJBHGKBPPM = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().KJPLIHEMLJL(item);
				if (iFDAFNGCIBP == null || cKJBHGKBPPM == null)
				{
					continue;
				}
				int num = cKJBHGKBPPM.MCIPEJBLIDC();
				foreach (Achievement item2 in iFDAFNGCIBP.FOICCCGPCMJ)
				{
					if (!IEPPDHFHEIC(item2.Name))
					{
						list.Add(new global::Pair<Achievement, int>(item2, num));
					}
					if (num < item2.EOGLBDCLMBM)
					{
						break;
					}
				}
			}
			return list;
		}

		public List<Achievement> NMHMFCKBMKN()
		{
			List<Achievement> list = new List<Achievement>();
			List<RosterAchievCounter> list2 = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().HOBHAAAEELG();
			for (int i = 0; i < list2.Count; i++)
			{
				RosterAchievCounter cKJBHGKBPPM = list2[i];
				string gOHIIMFFFJI = cKJBHGKBPPM.get_Name();
				AchievCounter iFDAFNGCIBP = HHLEKNNJGMJ.KJPLIHEMLJL(gOHIIMFFFJI);
				if (iFDAFNGCIBP == null)
				{
					continue;
				}
				List<Achievement> fOICCCGPCMJ = iFDAFNGCIBP.FOICCCGPCMJ;
				for (int j = 0; j < fOICCCGPCMJ.Count; j++)
				{
					Achievement jNPIOKEKMII = fOICCCGPCMJ[j];
					if (cKJBHGKBPPM.MCIPEJBLIDC() >= jNPIOKEKMII.EOGLBDCLMBM)
					{
						list.Add(jNPIOKEKMII);
					}
				}
			}
			return list;
		}
	}

	public class TutorSettingsStruct
	{
		public string CDNCPBKAHKJ = string.Empty;

		public string HGEAENEHEFI = string.Empty;

		public string KDKJKOICKJG = string.Empty;

		public float DefaultTutorialStepTimeout;

		public List<string> StepsNames = new List<string>();

		public void Parse(XmlNode node)
		{
			CDNCPBKAHKJ = node["TutorialWeapon"].Attributes["Name"].CIPOICEEIBK("WEAPON_KNIVES");
			HGEAENEHEFI = node["TutorialBoss"].Attributes["Name"].CIPOICEEIBK("ZONE_1|BOSS_LYNX|1");
			KDKJKOICKJG = node["TutorialTournament"].Attributes["Name"].CIPOICEEIBK("ZONE_1|Tournament|1");
			DefaultTutorialStepTimeout = node["TutorialStepTimeout"].Attributes["Value"].ParseFloat();
			foreach (XmlNode childNode in node["StepsNames"].ChildNodes)
			{
				if ("Step" == childNode.Name)
				{
					StepsNames.Add(childNode.Attributes["Name"].CIPOICEEIBK());
				}
			}
		}

		public bool IsStepName(string NOFMEBBEDKK)
		{
			return StepsNames.Contains(NOFMEBBEDKK);
		}
	}

	public struct INGFODOLNDB
	{
		public bool CJGACJBAFLB;

		public bool GLHICPIHDKA;

		public bool ACNLGBKEKNA;

		public bool CHDBPGDJLIL;

		public bool LLNGIKNDILO;

		public bool IOLEMKNCHOC;

		public bool LGAFEIJMKGL;

		public bool AEMOAEPCCMM;

		public bool JHMKINJHFGM;

		private void Parse(XmlNode node)
		{
			CJGACJBAFLB = false;
			GLHICPIHDKA = false;
			ACNLGBKEKNA = false;
			CHDBPGDJLIL = false;
			LLNGIKNDILO = false;
			IOLEMKNCHOC = false;
			LGAFEIJMKGL = false;
			AEMOAEPCCMM = false;
			JHMKINJHFGM = false;
			CJGACJBAFLB = node.PNJPEDPDMCP().ParseBool();
			if (CJGACJBAFLB)
			{
				GLHICPIHDKA = node["Quests"].PNJPEDPDMCP().ParseBool();
				if (GLHICPIHDKA)
				{
					ACNLGBKEKNA = node["Quests"]["Actions"].PNJPEDPDMCP().ParseBool();
				}
				CHDBPGDJLIL = node["Animations"].PNJPEDPDMCP().ParseBool();
				AEMOAEPCCMM = node["Tactics"].PNJPEDPDMCP().ParseBool();
				JHMKINJHFGM = node["Perks"].PNJPEDPDMCP().ParseBool();
				LLNGIKNDILO = node["Hits"].PNJPEDPDMCP().ParseBool();
				if (LLNGIKNDILO)
				{
					IOLEMKNCHOC = node["Hits"]["Damage"].PNJPEDPDMCP().ParseBool();
					LGAFEIJMKGL = node["Hits"]["Style"].PNJPEDPDMCP().ParseBool();
				}
			}
		}
	}

	public static WarriorAttributes BGENALLCKII = new WarriorAttributes();

	public static LevelAttributeGain KJJBEHBGKMK = new LevelAttributeGain();

	public static LevelAttributeGain MKHOLKGKNID = new LevelAttributeGain();

	public static List<AlignTargetAttribute> FPIDOGKOPGC = new List<AlignTargetAttribute>();

	public static ModifiedAlignFormula KMDIKMNMAOG = new ModifiedAlignFormula();

	public static MagicSettings DILKHIFCCGD = new MagicSettings();

	public static RandomTactic BJACOFCAHPD = new RandomTactic();

	public static Shock APCAKCCOMLO = new Shock();

	public static CritialDefault HHCEIEOOHCJ = new CritialDefault();

	public static PerkItems FDEJIIDIPBI = new PerkItems();

	public static OutdateLevels HPEBEOMLHKF = new OutdateLevels();

	public static StyleLevels NIPBIAGMAOD = new StyleLevels();

	public static RewardsPrize AAKJKANGFMJ = new RewardsPrize();

	private static AspectConstants HBDIIFNOHPF = new AspectConstants();

	private static List<Aspect> OKMEHFCEPIK = new List<Aspect>();

	private static AspectDoublingRange MCBAAGKCDOE = new AspectDoublingRange();

	public static float BGJPLNFFEOB = 0f;

	public static bool GLHMHHIADMK = false;

	public static LevelThresholds HHONBOCJBLB = new LevelThresholds();

	public static float LHHKFKLELMK = 0f;

	public static float FPDINCCPGMO = 0f;

	private static float JNNKGKOJBEI = 0f;

	private static float IGJGNFHKEKC = 0f;

	private static int NJDEBAFKGID = 1;

	private static int DPIFMDPNEGL = 5;

	private static float LKINJCDJPJD = 0f;

	private static string FEBHHIHDLBK = string.Empty;

	private static float BPMFKLKMIGH = 20000f;

	private static int _ComboTime = 0;

	private static int IHLGIDMENOP = 0;

	private static int OEKIEIDCGML = 0;

	private static int MIBLCOOGPHB = 0;

	public static Currencies AJDKHINLIDI = new Currencies();

	public static AALKFCCIGJJ JNIMKHKGPHE = new AALKFCCIGJJ();

	public static int CDILOOACLKK = 60;

	public static int MAEBANCIBOP = 2;

	public static AchievCounters HHLEKNNJGMJ = new AchievCounters();

	public static SlidersIndexStruct LMGAGBOKCFC = new SlidersIndexStruct();

	public static CurrencyBaseValues KIGEPCLPEIE = new CurrencyBaseValues();

	public static MoneyBaseValues NFJEPNHJPEE = new MoneyBaseValues();

	public static BarScales NPHEOMBNOLK = new BarScales();

	public static AchievementCounters OJNHPHEPFLI = new AchievementCounters();

	public static ShopOverrideConteyner JNDLCLLIMMM = new ShopOverrideConteyner();

	public static int JOODENKAECE = 0;

	public static int DIJOCFEFHAK = 0;

	private static float FECMKFKDLIJ;

	private static bool FPMDHBHDGKK = false;

	public static bool GCDIGFODNFO = true;

	public static bool LJOJHDOIFLN = false;

	public static SupportChoiceStruct JOEMCCADMON = new SupportChoiceStruct();

	private static HitEffects ILMELNHPLDF = new HitEffects();

	private static CameraSettigs FCMOMOGKAOH = new CameraSettigs();

	private static BaseSettigs GJJDNKGFJAN = new BaseSettigs();

	private static BaseSettigs LOBNDKMHFOM = new BaseSettigs();

	private static BaseSettigs BODMJLIADGC = new BaseSettigs();

	private static BaseSettigs KIKPMJLNDNM = new BaseSettigs();

	private static float AGKNPNBOHKC;

	private static float KIBGGBKKPFK = 0f;

	public static bool OBJEKOBDMOE = false;

	public static int AJAFNEIPOJB = 10;

	public static bool DAOBANIOMAN;

	private static string BFNACAJCOJI = string.Empty;

	private static string MIMENKMIOMI = string.Empty;

	private static string ABMGNMFBDID = string.Empty;

	public static bool KJNOABODHMG = false;

	public static TutorSettingsStruct AKPBNLKFONO = new TutorSettingsStruct();

	public static string KEFHKHCNBOK = string.Empty;

	public static float FPBFDNBDDIE = 1f;

	private static string EOKAPIGANEE = string.Empty;

	private static string MODJMLNMBJH = string.Empty;

	public const int FFEMNONLAMD = 1;

	public const int OHPGOLBBHON = 172800;

	public static int LJNCELBEHGN = 172800;

	public static uint JNOGEPFLLDM = 30000000u;

	public static bool GKOEGHLGPPE = false;

	public static long DailyDebugTime = 0L;

	public static bool LDBMFAMEMPF = false;

	public static bool LEEIGNICAMN = false;

	public static bool GBCMHICHIOI = false;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool IBCOLFJCDNG;

	public static Dictionary<SliderType, string> IPFBLJOALBN = new Dictionary<SliderType, string>();

	public static bool HHKHINLNCJB = false;

	public static string NFBKHONMMDL = string.Empty;

	public static INGFODOLNDB IEJDNMPFLPP;

	public static Dictionary<RaidTutorialStepCode, string> HEJIFIHLLJF = new Dictionary<RaidTutorialStepCode, string>();

	private static int LCBOCBHCBNP = 28800;

	private static bool NIDLHHCCDFJ = false;

	private static List<string> _notShowRateFights = new List<string>();

	public static string NIPABEEAMHJ = string.Empty;

	private static List<global::Pair<string, string>> GMKKGOHMEOM = new List<global::Pair<string, string>>();

	private static List<Achievement> CCNGPMBGNAC = new List<Achievement>();

	public static AspectConstants BDHJDKGHIEA
	{
		get
		{
			return JDHBJMHAJOG();
		}
	}

	public static List<Aspect> DILEMPDIJLG
	{
		get
		{
			return IGHPKGCJGHM();
		}
	}

	public static AspectDoublingRange FAEGGMGHOJB
	{
		get
		{
			return HBDECHDPLCA();
		}
	}

	public static float HEJEOMEBHGL
	{
		get
		{
			return CKOPPGCIHPL();
		}
		set
		{
			OKIEEBMCGHE(value);
		}
	}

	public static float JCAKIBIACFC
	{
		get
		{
			return FBOGLADLJML();
		}
		set
		{
			MJAPCKDDAMK(value);
		}
	}

	public static int SlowMode
	{
		get
		{
			return GGBABPJBGJB();
		}
		set
		{
			CEPJBBGGMDP(value);
		}
	}

	public static int OGLHGFJKMCO
	{
		get
		{
			return NAMEDMHAFKA();
		}
		set
		{
			PFBPLLNHLFB(value);
		}
	}

	public static float MIAONGMCEDA
	{
		get
		{
			return MGPIOCMLCLF();
		}
		set
		{
			MEPHOOCEOCI(value);
		}
	}

	public static string MCBCMFGLCBJ
	{
		get
		{
			return CJMOJMKCLMJ();
		}
		set
		{
			OKMDBKGNGBJ(value);
		}
	}

	public static float GIDNINANMCC
	{
		get
		{
			return PCDIBMDDAEF();
		}
		set
		{
			KMOPHHBPOLM(value);
		}
	}

	public static int HIGBAPPOOKJ
	{
		get
		{
			return KCBHAMHLGBC();
		}
		set
		{
			LKAJIAEEFDL(value);
		}
	}

	public static int GKAEJDCDMHC
	{
		get
		{
			return NPDOLGNNINO();
		}
		set
		{
			BOGEEJGMHON(value);
		}
	}

	public static int BKDJLFBHHHE
	{
		get
		{
			return MLAHKALHANF();
		}
		set
		{
			BBBPHLNOOPN(value);
		}
	}

	public static int MJFAGPDIPFH
	{
		get
		{
			return LDHIBCJCHFK();
		}
		set
		{
			NCADEABFAFF(value);
		}
	}

	public static float HMNBNIFBDFD
	{
		get
		{
			return KNAFNPGCLFP();
		}
		set
		{
			LJGHIDNLJHC(value);
		}
	}

	public static bool CODLJKAMGLL
	{
		get
		{
			return JJIHFNGBPEM();
		}
		set
		{
			ECBGGDNBKJC(value);
		}
	}

	public static HitEffects NMBBOEHHKNK
	{
		get
		{
			return OCMEOOKALHM();
		}
	}

	public static CameraSettigs DJELPPGNJEH
	{
		get
		{
			return LEPANPKBBKI();
		}
	}

	public static BaseSettigs LKLFLAKKFPC
	{
		get
		{
			return DAMKDJINILI();
		}
	}

	public static BaseSettigs GKIKBPMJPAC
	{
		get
		{
			return IOGOPCABLON();
		}
	}

	public static BaseSettigs GIEINANKIIA
	{
		get
		{
			return DMLPOANHHFI();
		}
	}

	public static BaseSettigs AHAKPJINJBL
	{
		get
		{
			return PPAEHBGNDNF();
		}
	}

	public static float LPMGOFOGINJ
	{
		get
		{
			return CHOGPMPEDIC();
		}
		set
		{
			ENMCFJCNKIG(value);
		}
	}

	public static float ICFLAENKNDI
	{
		get
		{
			return EBMNPGEKENM();
		}
		set
		{
			FJNOCJPPJPF(value);
		}
	}

	public static string FDIJEJHPNOG
	{
		get
		{
			return HLBPBLMMPCB();
		}
		set
		{
			JMFEGEGENII(value);
		}
	}

	public static string IEFIJKENPMP
	{
		get
		{
			return IIFHFGAENMH();
		}
		set
		{
			FNCAJMBJJBN(value);
		}
	}

	public static string CJDFKNHEJDE
	{
		get
		{
			return LBIMBGDGNNL();
		}
		set
		{
			LIHCDMIOKKM(value);
		}
	}

	public static ModelParameters DPIKLENNPEF
	{
		get
		{
			return LBMPHBNJMGG();
		}
	}

	public static string PDJHEIKDHEC
	{
		get
		{
			return BGGMLFLFONJ();
		}
		set
		{
			PALADCEPDNI(value);
		}
	}

	public static string MMGEIOGCBAO
	{
		get
		{
			return MNMGDBGCKOM();
		}
		set
		{
			NCGINDFMIFB(value);
		}
	}

	public static bool NFAGLALCFLD
	{
		get
		{
			return NMODJEJFFNC();
		}
		set
		{
			FBEHNHNOKKO(value);
		}
	}

	public static AspectConstants JDHBJMHAJOG()
	{
		return HBDIIFNOHPF;
	}

	public static List<Aspect> IGHPKGCJGHM()
	{
		return OKMEHFCEPIK;
	}

	public static AspectDoublingRange HBDECHDPLCA()
	{
		return MCBAAGKCDOE;
	}

	public static float CKOPPGCIHPL()
	{
		return JNNKGKOJBEI;
	}

	public static void OKIEEBMCGHE(float value)
	{
		JNNKGKOJBEI = value;
	}

	public static float FBOGLADLJML()
	{
		return IGJGNFHKEKC;
	}

	public static void MJAPCKDDAMK(float value)
	{
		IGJGNFHKEKC = value;
	}

	public static int GGBABPJBGJB()
	{
		return NJDEBAFKGID;
	}

	public static void CEPJBBGGMDP(int value)
	{
		NJDEBAFKGID = value;
	}

	public static int NAMEDMHAFKA()
	{
		return DPIFMDPNEGL;
	}

	public static void PFBPLLNHLFB(int value)
	{
		DPIFMDPNEGL = value;
	}

	public static float MGPIOCMLCLF()
	{
		return LKINJCDJPJD;
	}

	public static void MEPHOOCEOCI(float value)
	{
		LKINJCDJPJD = value;
	}

	public static string CJMOJMKCLMJ()
	{
		return FEBHHIHDLBK;
	}

	public static void OKMDBKGNGBJ(string value)
	{
		FEBHHIHDLBK = value;
	}

	public static float PCDIBMDDAEF()
	{
		return BPMFKLKMIGH;
	}

	public static void KMOPHHBPOLM(float value)
	{
		BPMFKLKMIGH = value;
	}

	public static int KCBHAMHLGBC()
	{
		return _ComboTime;
	}

	public static void LKAJIAEEFDL(int value)
	{
		_ComboTime = value;
	}

	public static int NPDOLGNNINO()
	{
		return IHLGIDMENOP;
	}

	public static void BOGEEJGMHON(int value)
	{
		IHLGIDMENOP = value;
	}

	public static int MLAHKALHANF()
	{
		return OEKIEIDCGML;
	}

	public static void BBBPHLNOOPN(int value)
	{
		OEKIEIDCGML = value;
	}

	public static int LDHIBCJCHFK()
	{
		return MIBLCOOGPHB;
	}

	public static void NCADEABFAFF(int value)
	{
		MIBLCOOGPHB = value;
	}

	public static Achievement EKBBPLEHGHD(Counter EPJGLECOIBG, int value)
	{
		Achievement jNPIOKEKMII = null;
		AchievCounter iFDAFNGCIBP = HHLEKNNJGMJ.KJPLIHEMLJL(EPJGLECOIBG.Name);
		if (iFDAFNGCIBP == null)
		{
			return null;
		}
		RosterAchievCounter cKJBHGKBPPM = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().KJPLIHEMLJL(EPJGLECOIBG.Name);
		int num = ((cKJBHGKBPPM != null) ? cKJBHGKBPPM.MCIPEJBLIDC() : 0);
		List<Achievement> fOICCCGPCMJ = iFDAFNGCIBP.FOICCCGPCMJ;
		int num2 = num + value;
		for (int i = 0; i < fOICCCGPCMJ.Count; i++)
		{
			Achievement jNPIOKEKMII2 = fOICCCGPCMJ[i];
			if (jNPIOKEKMII2.EOGLBDCLMBM >= num && jNPIOKEKMII2.EOGLBDCLMBM <= num2 && !jNPIOKEKMII2.HGMHEOGJDMM)
			{
				jNPIOKEKMII = jNPIOKEKMII2;
				if (!EPJGLECOIBG.IsFightEnd)
				{
					jNPIOKEKMII.HGMHEOGJDMM = true;
				}
				else
				{
					CCNGPMBGNAC.Add(jNPIOKEKMII);
				}
			}
			else if (jNPIOKEKMII2.EOGLBDCLMBM >= num && jNPIOKEKMII2.EOGLBDCLMBM >= num2)
			{
				break;
			}
		}
		return jNPIOKEKMII;
	}

	public static Achievement POFHBHOIMAI(string name)
	{
		Achievement jNPIOKEKMII = HHLEKNNJGMJ.ABNAODNDHDM(name);
		if (jNPIOKEKMII == null)
		{
			return null;
		}
		if (!jNPIOKEKMII.HGMHEOGJDMM)
		{
			jNPIOKEKMII.HGMHEOGJDMM = true;
		}
		RosterAchievement pMGCOHHMIIC = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().JABBCCJLOOC(name);
		if (pMGCOHHMIIC == null)
		{
			Counter jLOIMNNHFKH = OJNHPHEPFLI.CBGAEFLNGAC(name);
			if (jLOIMNNHFKH != null && jLOIMNNHFKH.CompleteValue <= 0)
			{
				jLOIMNNHFKH.CompleteValue = 1;
			}
		}
		ListSF.CCDKHLAMKKO().KJNPJKEHGLE().BFCLLIKOJGD();
		return jNPIOKEKMII;
	}

	public static void NKGCBJAAJMA(List<global::Pair<Achievement, int>> CIMGCGDDKCE)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		List<SocialAchievement> list = new List<SocialAchievement>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < CIMGCGDDKCE.Count; i++)
		{
			Achievement lLHEDBIEHAA = CIMGCGDDKCE[i].First;
			int nFNBFHCDEGG = CIMGCGDDKCE[i].Second;
			SocialAchievement iFLOCBJBEGL = new SocialAchievement(lLHEDBIEHAA.EIEBHLJCOKE, nFNBFHCDEGG, lLHEDBIEHAA.EOGLBDCLMBM);
			if (nFNBFHCDEGG >= lLHEDBIEHAA.EOGLBDCLMBM)
			{
				nKGLHEGIKKP.KJNPJKEHGLE().POKNGJJAHAL(lLHEDBIEHAA, false);
				iFLOCBJBEGL.value = lLHEDBIEHAA.EOGLBDCLMBM;
			}
			list.Add(iFLOCBJBEGL);
			list2.Add(lLHEDBIEHAA.Name);
		}
		if (InternetController.DHDGLNKILPM())
		{
			if (SystemProperties.PKLFCFBEIIG() && GameCenterController.OBDJPKOJADA())
			{
				GameCenterController.FLJILJDHNLJ(list);
			}
			else
			{
				ListSF.CCDKHLAMKKO().KJNPJKEHGLE().AddRepostAchievements(list2);
			}
		}
	}

	public static void HGJCGOPPNBI()
	{
		List<RosterAchievement> list = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().NOJKMMJJPHF();
		List<SocialAchievement> list2 = new List<SocialAchievement>();
		List<string> list3 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			Achievement jNPIOKEKMII = HHLEKNNJGMJ.ABNAODNDHDM(list[i].get_Name());
			if (jNPIOKEKMII != null)
			{
				list2.Add(new SocialAchievement(jNPIOKEKMII.EIEBHLJCOKE, jNPIOKEKMII.EOGLBDCLMBM, jNPIOKEKMII.EOGLBDCLMBM));
				list3.Add(jNPIOKEKMII.Name);
			}
		}
		if (InternetController.DHDGLNKILPM())
		{
			if (SystemProperties.PKLFCFBEIIG() && GameCenterController.OBDJPKOJADA())
			{
				GameCenterController.FLJILJDHNLJ(list2);
			}
			else
			{
				ListSF.CCDKHLAMKKO().KJNPJKEHGLE().AddRepostAchievements(list3);
			}
		}
	}

	private static bool IEPPDHFHEIC(string name)
	{
		List<RosterAchievement> list = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().NOJKMMJJPHF();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].get_Name() == name)
			{
				return true;
			}
		}
		return false;
	}

	public static void DKBINLMJIJG()
	{
		if (!InternetController.DHDGLNKILPM() || !SystemProperties.PKLFCFBEIIG() || !GameCenterController.OBDJPKOJADA())
		{
			return;
		}
		List<RepostAchievement> list = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().NEEDPAEIOLH();
		List<SocialAchievement> list2 = new List<SocialAchievement>();
		List<AchievCounter> mDNKEAFGAOB = HHLEKNNJGMJ.MDNKEAFGAOB;
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < mDNKEAFGAOB.Count; j++)
			{
				RosterAchievCounter cKJBHGKBPPM = ListSF.CCDKHLAMKKO().KJNPJKEHGLE().KJPLIHEMLJL(mDNKEAFGAOB[j].Name);
				if (cKJBHGKBPPM == null)
				{
					continue;
				}
				List<Achievement> fOICCCGPCMJ = mDNKEAFGAOB[j].FOICCCGPCMJ;
				for (int k = 0; k < fOICCCGPCMJ.Count; k++)
				{
					if (fOICCCGPCMJ[k].Name == list[i].get_Name())
					{
						if (fOICCCGPCMJ[k].EOGLBDCLMBM == 0)
						{
							LLLOJBFMONN.Write("Error - counter == 0");
						}
						SocialAchievement item = new SocialAchievement(fOICCCGPCMJ[k].EIEBHLJCOKE, cKJBHGKBPPM.MCIPEJBLIDC(), fOICCCGPCMJ[k].EOGLBDCLMBM);
						list2.Add(item);
					}
				}
			}
		}
		GameCenterController.FLJILJDHNLJ(list2);
		ListSF.CCDKHLAMKKO().KJNPJKEHGLE().PEBJNEJLONK(list);
	}

	public static bool CJKKIOIMGAC(string FHLFEBDNIFF)
	{
		string path = string.Format("{0}{1}", SF2Paths.BHCPOOOJAAK(), FHLFEBDNIFF);
		UnityEngine.Object obj = Resources.Load(path);
		return obj != null;
	}

	public static float KNAFNPGCLFP()
	{
		return FECMKFKDLIJ;
	}

	public static void LJGHIDNLJHC(float value)
	{
		FECMKFKDLIJ = value;
	}

	public static bool JJIHFNGBPEM()
	{
		return FPMDHBHDGKK;
	}

	public static void ECBGGDNBKJC(bool value)
	{
		FPMDHBHDGKK = value;
	}

	public static HitEffects OCMEOOKALHM()
	{
		return ILMELNHPLDF;
	}

	public static CameraSettigs LEPANPKBBKI()
	{
		return FCMOMOGKAOH;
	}

	public static BaseSettigs DAMKDJINILI()
	{
		return GJJDNKGFJAN;
	}

	public static BaseSettigs IOGOPCABLON()
	{
		return LOBNDKMHFOM;
	}

	public static BaseSettigs DMLPOANHHFI()
	{
		return BODMJLIADGC;
	}

	public static BaseSettigs PPAEHBGNDNF()
	{
		return KIKPMJLNDNM;
	}

	public static float CHOGPMPEDIC()
	{
		return AGKNPNBOHKC;
	}

	public static void ENMCFJCNKIG(float value)
	{
		AGKNPNBOHKC = value;
	}

	public static bool HILEAHAAFIC(string MJMEBBCLHII)
	{
		AlignTargetAttribute kDDAFBJDOMN = FPIDOGKOPGC.Find((AlignTargetAttribute DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(MJMEBBCLHII));
		return kDDAFBJDOMN != null;
	}

	public static float EBMNPGEKENM()
	{
		return KIBGGBKKPFK;
	}

	public static void FJNOCJPPJPF(float value)
	{
		KIBGGBKKPFK = value;
	}

	public static string HLBPBLMMPCB()
	{
		return BFNACAJCOJI;
	}

	public static void JMFEGEGENII(string value)
	{
		BFNACAJCOJI = value;
	}

	public static string IIFHFGAENMH()
	{
		return MIMENKMIOMI;
	}

	public static void FNCAJMBJJBN(string value)
	{
		MIMENKMIOMI = value;
	}

	public static string LBIMBGDGNNL()
	{
		return ABMGNMFBDID;
	}

	public static void LIHCDMIOKKM(string value)
	{
		ABMGNMFBDID = value;
	}

	public static bool IsProbality(float value)
	{
		return NekkiMath.randomChance(value * 100f);
	}

	public static float MPNBGBIMEIP(FightList KGKDKENMAOA)
	{
		return 0f;
	}

	public static float MBPNHCGKKGJ(bool GHFKMONMAJG, ModelParameters DPLNMPKPOJD, ModelParameters JEHPCONLCMI, List<global::Pair<string, float>> CHLLJCFCGHO, string KLIIDDMHNOL, ref float AJEDGBBFGDF, ref float DGILKKFKPAL, ref float ILGBEJNHNMH)
	{
		float num = AlignTargetAttribute.GetValue(KLIIDDMHNOL);
		float num2 = 0f - num;
		float num3 = 0f;
		float num4 = 0f;
		int OEMALIFPGPO = 0;
		JEHPCONLCMI.IBLHIAHECLK.Get(KLIIDDMHNOL, ref OEMALIFPGPO);
		float num5 = OEMALIFPGPO;
		float num6 = float.MinValue;
		ModelParameters kIKOGDEPGHB = ((!GHFKMONMAJG) ? DPLNMPKPOJD : JEHPCONLCMI);
		List<AttributesAlign> list = new List<AttributesAlign>();
		int num7 = AttributesAlign.GGBAGGMLFHE(kIKOGDEPGHB.FKJBBIMPCBB, list);
		for (int i = 0; i < CHLLJCFCGHO.Count; i++)
		{
			string lLHEDBIEHAA = CHLLJCFCGHO[i].First;
			OEMALIFPGPO = 0;
			DPLNMPKPOJD.IBLHIAHECLK.Get(lLHEDBIEHAA, ref OEMALIFPGPO);
			float num8 = OEMALIFPGPO;
			float num9 = num8 + CHLLJCFCGHO[i].Second;
			float num10 = AlignTargetAttribute.GetValue(lLHEDBIEHAA);
			float num11 = 0f;
			if (GHFKMONMAJG)
			{
				num11 = float.MaxValue;
				foreach (AttributesAlign item in list)
				{
					bool flag = ListSF.CCDKHLAMKKO().JPMPIDFGCJL();
					bool flag2 = QuestUtils.DPLFDKODKIC().NCPKGHDFOFL();
					if ((flag && item.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFHard) || (!flag && item.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFNormal) || item.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFBoth || flag2)
					{
						float oNMMKLDMHJD = item.Factor;
						float fJAHKFNFNCK = item.Shift;
						float num12 = (num9 - num5) * (1f - oNMMKLDMHJD) + (num10 - num) * oNMMKLDMHJD - fJAHKFNFNCK;
						if (num12 < num11)
						{
							num11 = num12;
							num3 = oNMMKLDMHJD;
							num4 = fJAHKFNFNCK;
						}
					}
				}
			}
			else
			{
				num11 = float.MinValue;
				foreach (AttributesAlign item2 in list)
				{
					bool flag3 = ListSF.CCDKHLAMKKO().JPMPIDFGCJL();
					bool flag4 = QuestUtils.DPLFDKODKIC().NCPKGHDFOFL();
					if ((flag3 && item2.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFHard) || (!flag3 && item2.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFNormal) || item2.KONCHIPGFGO == ModelParameters.IHFKGJLIPGH.DFBoth || flag4)
					{
						float oNMMKLDMHJD2 = item2.Factor;
						float fJAHKFNFNCK2 = item2.Shift;
						float num13 = (num9 - num5) * (1f - oNMMKLDMHJD2) + (num10 - num) * oNMMKLDMHJD2 + fJAHKFNFNCK2;
						if (num11 < num13)
						{
							num11 = num13;
							num3 = oNMMKLDMHJD2;
							num4 = fJAHKFNFNCK2;
						}
					}
				}
			}
			ModifiedAlignFormula.DamageAttribute jNLEHBPFPBN = KMDIKMNMAOG.NOADKFMGODA(lLHEDBIEHAA);
			if (jNLEHBPFPBN != null)
			{
				float gFHOHECBODM = jNLEHBPFPBN.GFHOHECBODM;
				string kLAIAPBONFM = jNLEHBPFPBN.KLAIAPBONFM;
				bool flag5 = kLAIAPBONFM == "Player";
				if (num11 >= gFHOHECBODM && GHFKMONMAJG == flag5)
				{
					float bGJPLNFFEOB = BGJPLNFFEOB;
					float bPPJAMCGICA = jNLEHBPFPBN.BPPJAMCGICA;
					float oPHGJJGKIHE = jNLEHBPFPBN.OPHGJJGKIHE;
					num11 = bGJPLNFFEOB * Mathf.Log((1f - (1f - bPPJAMCGICA) * Mathf.Pow(oPHGJJGKIHE, (0f - num11) / bGJPLNFFEOB)) / bPPJAMCGICA) / Mathf.Log(2f);
				}
			}
			if (num6 < num11)
			{
				num6 = num11;
				num2 = num10 - num;
			}
		}
		AJEDGBBFGDF = num2;
		DGILKKFKPAL = num3;
		ILGBEJNHNMH = num4;
		return num6;
	}

	public static float GetAttributesHitMultiplier(bool GHFKMONMAJG, ModelParameters DPLNMPKPOJD, ModelParameters JEHPCONLCMI, List<global::Pair<string, float>> CHLLJCFCGHO, string KLIIDDMHNOL)
	{
		float AJEDGBBFGDF = 0f;
		float DGILKKFKPAL = 0f;
		float ILGBEJNHNMH = 0f;
		float eEPAABANDBL = MBPNHCGKKGJ(GHFKMONMAJG, DPLNMPKPOJD, JEHPCONLCMI, CHLLJCFCGHO, KLIIDDMHNOL, ref AJEDGBBFGDF, ref DGILKKFKPAL, ref ILGBEJNHNMH);
		return GetAttributesHitMultiplier(eEPAABANDBL);
	}

	public static float GetAttributesHitMultiplier(float EEPAABANDBL)
	{
		float bGJPLNFFEOB = BGJPLNFFEOB;
		return Mathf.Pow(2f, EEPAABANDBL / bGJPLNFFEOB);
	}

	public static List<ModelParameters> IGNNMAKHBFF(List<ModelParameters> JCICKLIMBEF)
	{
		List<ModelParameters> list = new List<ModelParameters>();
		foreach (ModelParameters item2 in JCICKLIMBEF)
		{
			List<ModelParameters> list2 = ListSF.ELEBLBJKDBI().APOACOEFALC(item2, item2.PEBKEBIBAFA);
			foreach (ModelParameters item3 in list2)
			{
				ModelParameters item = CDCAOHHFNPL(item3);
				list.Add(item);
			}
		}
		return list;
	}

	public static ModelParameters LBMPHBNJMGG()
	{
		ModelParameters kIKOGDEPGHB = ListSF.GAMMAIGEIOB();
		List<ItemInfo> list = kIKOGDEPGHB.DGMDEDKLGMB();
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			ItemInfo dJKEECEOCJB = list[i];
			if (dJKEECEOCJB != null)
			{
				UserItem dKCHDHMLKHN = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(dJKEECEOCJB.Name);
				if (dKCHDHMLKHN != null && dKCHDHMLKHN.JBCOAMLEBFG())
				{
					kIKOGDEPGHB.OLLNIKFPMKE(list[i].Type, dKCHDHMLKHN.AKKBIFEFDCI());
				}
			}
		}
		CDCAOHHFNPL(kIKOGDEPGHB);
		kIKOGDEPGHB.ABAPAIEBNGK = true;
		kIKOGDEPGHB.EEGMBGBLLIF = false;
		kIKOGDEPGHB.IsPlayer = true;
		return kIKOGDEPGHB;
	}

	private static ModelParameters CDCAOHHFNPL(ModelParameters JCICKLIMBEF)
	{
		if (JCICKLIMBEF.PILJCAOFAED == null)
		{
			JCICKLIMBEF.PILJCAOFAED = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(MNMGDBGCKOM());
		}
		JCICKLIMBEF.FCOALLOHJNP = 0;
		JCICKLIMBEF.IsPlayer = false;
		int num = (ObscuredInt)(JCICKLIMBEF.OJLKDEHMIAC());
		if (num > 0)
		{
			JCICKLIMBEF.CIDCNCDFONA = num;
		}
		else
		{
			JCICKLIMBEF.CIDCNCDFONA = 1f;
		}
		JCICKLIMBEF.EAJHPCJJCDI = false;
		JCICKLIMBEF.ABLMGLAKJBL = true;
		JCICKLIMBEF.BCLGFKDDNKH();
		JCICKLIMBEF.IAIHFLGBIPB = LoadMoves(AnimationData.DJDLCMCLOJN());
		JCICKLIMBEF.PPFDLIBLNDG();
		JCICKLIMBEF.NOBKKLBJFIL();
		return JCICKLIMBEF;
	}

	public static void FMICOICLCNL(bool LLOLBKJMKNC = true)
	{
		Module.ELEBLBJKDBI().JJFFNDJDNAJ(true, LLOLBKJMKNC);
	}

	public static void KKNGFGMJKHG()
	{
		Module.ELEBLBJKDBI().JJFFNDJDNAJ(false);
	}

	public static void BKFMHANNIEF()
	{
		SceneManagerSF.Reset();
	}

	private static List<int> LoadMoves(int KAEPJHHLLPK = 50)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < KAEPJHHLLPK; i++)
		{
			list.Add(i);
		}
		return list;
	}

	public static FightList GKBHKJNGNPO(Battle DPOOIONCEOA)
	{
		if (DPOOIONCEOA != null)
		{
			List<FightList> list = DPOOIONCEOA.ANNHMNIHKCC();
			foreach (FightList item in list)
			{
				if (item.PGBKNLAEANJ == ConditionStatus.StatusOpen)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static FightList JGDLLEAGBBD(Battle DPOOIONCEOA)
	{
		if (DPOOIONCEOA != null)
		{
			List<FightList> list = DPOOIONCEOA.ANNHMNIHKCC();
			if (list.Count > 0)
			{
				return list[list.Count - 1];
			}
		}
		return null;
	}

	public static Fight ABAIHGFPHMO(object data, PreFight preFight = null, GameController LPGANKOAPJL = null)
	{
		FightList jDIPBIHBGPF = (FightList)data;
		ModelParameters kIKOGDEPGHB = null;
		kIKOGDEPGHB = LBMPHBNJMGG().Clone();
		List<ModelParameters> list = new List<ModelParameters>();
		BattleType pJMEMGHKKBM = jDIPBIHBGPF.get_Type();
		if (pJMEMGHKKBM == BattleType.FightPeriodic)
		{
			if (!jDIPBIHBGPF.FLKFFDLLBKA().HasRandomSeeds)
			{
				jDIPBIHBGPF.FLKFFDLLBKA().BABOCEFFPII();
			}
			NekkiMath.KACCBCCEPGB(jDIPBIHBGPF.FLKFFDLLBKA().PFJKCOPFNHB());
			list = IGNNMAKHBFF(jDIPBIHBGPF.OFKJMHPMCCD());
			NekkiMath.KACCBCCEPGB();
		}
		else
		{
			list = IGNNMAKHBFF(jDIPBIHBGPF.OFKJMHPMCCD());
		}
		if (list.Count == 0)
		{
			// Keep a malformed/newer fight playable and make the migration defect
			// explicit. AdaptStages normally materializes battle-level warrior pools;
			// this is the final safety net for future unsupported stage shapes.
			ModelParameters fallbackEnemy = CDCAOHHFNPL(kIKOGDEPGHB.Clone());
			fallbackEnemy.IsPlayer = false;
			fallbackEnemy.HNKFHGOOKEG = "man_fist";
			list.Add(fallbackEnemy);
			UnityEngine.Debug.LogError("[Fight] '" + jDIPBIHBGPF.Name +
				"' contained no usable enemies; using a player-equipment compatibility opponent.");
		}
		UnderworldRaidDiagnostics.LogEnemies(jDIPBIHBGPF, kIKOGDEPGHB, list);
		return new Fight(jDIPBIHBGPF, kIKOGDEPGHB, list, preFight, LPGANKOAPJL);
	}

	public static void MHMGONPIPKG(Battle DPOOIONCEOA)
	{
	}

	public static FightList HIPIGHPMBIJ(FightList KGKDKENMAOA)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP.BGBFBIDOECK() > 0)
		{
			Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
			List<FightList> list = cNAOMDMIGLJ.ANNHMNIHKCC();
			return list[list.Count - 1];
		}
		return KGKDKENMAOA;
	}

	public static bool KBHDKPAMOJN(ItemInfo item, ItemAction LFLGCDNKNJI, int count = 1, Action<object> callback = null)
	{
		UserItem nDMCFNGEPOA = ListSF.CMGOCLGHNLH(item.Name);
		return KBHDKPAMOJN(item, nDMCFNGEPOA, LFLGCDNKNJI, count, callback);
	}

	private static bool KBHDKPAMOJN(ItemInfo item, UserItem NDMCFNGEPOA, ItemAction LFLGCDNKNJI, int count = 1, Action<object> callback = null)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.OOFHDANMCJB = string.Empty;
		ListSF.CheckItems bJEBPDNMNAE = new ListSF.CheckItems();
		if (LFLGCDNKNJI == ItemAction.Item_Buy_Gold || LFLGCDNKNJI == ItemAction.Item_Buy_Ruby || LFLGCDNKNJI == ItemAction.Item_Buy_Real || LFLGCDNKNJI == ItemAction.Item_Upgrade_Gold || LFLGCDNKNJI == ItemAction.Item_Upgrade_Ruby || LFLGCDNKNJI == ItemAction.Item_Delivery_Ruby || LFLGCDNKNJI == ItemAction.Item_Recipe || LFLGCDNKNJI == ItemAction.Item_Recipe_Delivery_Ruby || LFLGCDNKNJI == ItemAction.Item_Consumable)
		{
			bJEBPDNMNAE = ListSF.CLKECIFEMNB(item, LFLGCDNKNJI, count);
			if (bJEBPDNMNAE.Value == -1)
			{
				if (LFLGCDNKNJI == ItemAction.Item_Recipe)
				{
					GGKPADOGDCG(item, bJEBPDNMNAE.Type);
				}
				else
				{
					EEKHDNNBDCH(item, bJEBPDNMNAE.Type);
				}
				return false;
			}
		}
		if ((item.EHKNIKHPGDN == 0 && (LFLGCDNKNJI == ItemAction.Item_Buy_Gold || LFLGCDNKNJI == ItemAction.Item_Upgrade_Gold)) || (NDMCFNGEPOA != null && NDMCFNGEPOA.GKGIKMCMCPB()) || LFLGCDNKNJI == ItemAction.Item_Buy_Ruby || LFLGCDNKNJI == ItemAction.Item_Equip || LFLGCDNKNJI == ItemAction.Item_Upgrade_Ruby || LFLGCDNKNJI == ItemAction.Item_Delivery_Ruby)
		{
			ListSF.FAAAGBACKAE(item);
		}
		if (NDMCFNGEPOA != null && NDMCFNGEPOA.EFMFGEPDAOP() && LFLGCDNKNJI == ItemAction.Item_Unequip)
		{
			ListSF.ELEBLBJKDBI().OOLIADKLGLJ(item);
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool result = false;
		switch (LFLGCDNKNJI)
		{
		case ItemAction.Item_Buy_Gold:
		case ItemAction.Item_Buy_Ruby:
		case ItemAction.Item_Buy_Real:
		case ItemAction.Item_Upgrade_Gold:
		case ItemAction.Item_Upgrade_Ruby:
		case ItemAction.Item_Delivery_Ruby:
		case ItemAction.Item_Free:
		case ItemAction.Item_Consumable:
			result = ListSF.KCBCGDFKNME(item, LFLGCDNKNJI, bJEBPDNMNAE.Value, count, callback);
			break;
		case ItemAction.Item_Recipe_Delivery_Ruby:
			flag2 = true;
			flag3 = true;
			result = ListSF.KCBCGDFKNME(item, LFLGCDNKNJI, bJEBPDNMNAE.Value, count, callback);
			break;
		case ItemAction.Item_Equip:
			result = ListSF.AFGHCIDFAHB(NDMCFNGEPOA, true);
			break;
		case ItemAction.Item_Unequip:
			result = ListSF.AFGHCIDFAHB(NDMCFNGEPOA, false);
			break;
		case ItemAction.Item_Delivery_End:
			result = ListSF.IMLFOOIBLJA(item);
			break;
		case ItemAction.Item_Update:
			result = true;
			break;
		case ItemAction.Item_Recipe:
		{
			flag = true;
			RecipeItemInfo mBIJKDIEFIF2 = (RecipeItemInfo)item;
			NIANNOBAHMH("enchantment_notification", mBIJKDIEFIF2);
			break;
		}
		case ItemAction.Item_Recipe_Delivery_End:
		{
			flag2 = true;
			flag3 = true;
			RecipeItemInfo mBIJKDIEFIF = (RecipeItemInfo)item;
			FBINMBCCMHA("enchantment_notification", mBIJKDIEFIF);
			break;
		}
		}
		UserItem dKCHDHMLKHN = ListSF.CMGOCLGHNLH(item.Name);
		if (flag3)
		{
		}
		if (dKCHDHMLKHN != null && dKCHDHMLKHN.IJGAOHJNLAH() > 0)
		{
			DMBJCCFNFKK("item_notification", dKCHDHMLKHN);
		}
		else if (dKCHDHMLKHN != null && dKCHDHMLKHN.IJGAOHJNLAH() == 0)
		{
			NELKFJJHFDC("item_notification", dKCHDHMLKHN);
		}
		return result;
	}

	public static void EEKHDNNBDCH(ItemInfo item, ListSF.BKDHBIDPKLK DDEDNPLHOJH)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.DLKPBAJDHBO = item;
		switch (DDEDNPLHOJH)
		{
		case ListSF.BKDHBIDPKLK.CHECK_ITEM_MONEY:
			hHKLFIIBIFF.OOFHDANMCJB = "Coins";
			break;
		case ListSF.BKDHBIDPKLK.CHECK_ITEM_BONUS:
			hHKLFIIBIFF.OOFHDANMCJB = "Ruby";
			break;
		case ListSF.BKDHBIDPKLK.CHECK_ITEM_NO_NETWORK:
			hHKLFIIBIFF.OOFHDANMCJB = "Connection";
			break;
		}
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE_UNSUCCESSFUL))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
		OFOKPNFGDMD("Insufficient Currency");
	}

	private static void GGKPADOGDCG(ItemInfo item, ListSF.BKDHBIDPKLK DDEDNPLHOJH)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		RecipeItemInfo bNJOCBKNPMG = (RecipeItemInfo)item;
		hHKLFIIBIFF.DPLEGFCHOCE.OHCGEEEKEJH = bNJOCBKNPMG.MFEAIEJFDAM().get_Name();
		hHKLFIIBIFF.DPLEGFCHOCE.FHELNNCGCGC = bNJOCBKNPMG.OIMGNCLBPHD().Name;
		long num = ListSF.BLBNJKJKMBM();
		long bMNFPNBAMAF = num + bNJOCBKNPMG.ADAJKDEOAAG().EHKNIKHPGDN;
		hHKLFIIBIFF.DPLEGFCHOCE.BMNFPNBAMAF = bMNFPNBAMAF;
		hHKLFIIBIFF.DPLEGFCHOCE.MECEADEKGJB = "Materials";
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT_UNSUCCESSFUL))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}

	public static bool MKADBAEEMFA(SliderType OFEMKBGPNBH, SliderType CFDMHKKBGIN)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.HJGNGBHONCP = IPFBLJOALBN[OFEMKBGPNBH];
		hHKLFIIBIFF.KBKAGDKOGNJ = IPFBLJOALBN[CFDMHKKBGIN];
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_CHANGE_TAB))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			return true;
		}
		return false;
	}

	public static bool OIGPBEKELCP(ScreenType JPDNPODKKJP)
	{
		if (JPDNPODKKJP == ScreenType.ModuleShop &&
			ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_BUTTON_PRESS))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			// The migrated quest graph observes this event but does not always issue
			// its own scene transition.  Continue the user's requested navigation.
			return false;
		}
		if (JPDNPODKKJP == ScreenType.ModuleShop && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOP_ENTER))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			return false;
		}
		return false;
	}

	public static void PGLIKMEJBPK()
	{
		LLLOJBFMONN.Write("exitApplication() called, quiting");
		OFOKPNFGDMD("App Close");
		ApplicationController.Quit();
	}

	public static uint IBNHPCFKGOH(FightList KGKDKENMAOA, bool FFIBGBMOMPD)
	{
		FightResult nHIDAJFLHJN = new FightResult();
		nHIDAJFLHJN.LFLGCDNKNJI = KGKDKENMAOA.get_Type();
		nHIDAJFLHJN.DIAIIPCBMFL = KGKDKENMAOA.BCKFACGMOKC;
		int num = KGKDKENMAOA.JABJLCEJDDM;
		if (FFIBGBMOMPD)
		{
			num++;
		}
		if (KGKDKENMAOA.CBJOENICLAF())
		{
			num = (FFIBGBMOMPD ? 1 : 0);
		}
		RewardStruct lGDIIADDFLH = KGKDKENMAOA.OOOBLJIHBEP(num);
		nHIDAJFLHJN.FCKFOPMNFOF(lGDIIADDFLH, null, null, KGKDKENMAOA);
		return nHIDAJFLHJN.PMIHPJFAJIO.exp;
	}

	public static void FKMEIHGOFDD(FightResult HEIADONEACH)
	{
		KJNOABODHMG = true;
		KJNOABODHMG = false;
		Module.DLOKJOHNDID(ScreenType.ModuleMap);
	}

	public static void EndFight(ComboStatistic AIOMDIAFHGB, FightList KGKDKENMAOA, ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP, GameOverTypes MHNEKAEGNBO, ComboStatistic MOJHPBGGNAH = null, float OEIFGEHDHLE = 0f, int JEGCHOOLDLB = 0, float PIFMOMMPFFM = 0f, int JEDPGMGCJGA = 0)
	{
		bool flag = LEBLJJCFKOP == null && MOJHPBGGNAH == null;
		bool flag2 = KGKDKENMAOA == null || MHNEKAEGNBO == GameOverTypes.GAME_OVER_SURRENDER;
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		KGKDKENMAOA = ((KGKDKENMAOA == null) ? ListSF.CHMCKGCDGCM(nKGLHEGIKKP.HEBDMAIEAPM()) : KGKDKENMAOA);
		if (KGKDKENMAOA == null)
		{
			LLLOJBFMONN.Error("0 == fightList");
		}
		FightResult nHIDAJFLHJN = new FightResult();
		nHIDAJFLHJN.KGKDKENMAOA = KGKDKENMAOA;
		nHIDAJFLHJN.LFLGCDNKNJI = KGKDKENMAOA.get_Type();
		nHIDAJFLHJN.DIAIIPCBMFL = new FightIDS(KGKDKENMAOA.BCKFACGMOKC);
		nHIDAJFLHJN.ABKBEJBICOA = ABKBEJBICOA;
		nHIDAJFLHJN.LEBLJJCFKOP = LEBLJJCFKOP;
		nHIDAJFLHJN.MHNEKAEGNBO = MHNEKAEGNBO;
		int num = KGKDKENMAOA.JABJLCEJDDM;
		if (nHIDAJFLHJN.IsWinner())
		{
			num++;
		}
		if (KGKDKENMAOA.CBJOENICLAF())
		{
			num = (nHIDAJFLHJN.IsWinner() ? 1 : 0);
		}
		RewardStruct lGDIIADDFLH = KGKDKENMAOA.OOOBLJIHBEP(num);
		nHIDAJFLHJN.FCKFOPMNFOF(lGDIIADDFLH, AIOMDIAFHGB, MOJHPBGGNAH, KGKDKENMAOA);
		if (!flag)
		{
			ArgsDict kEMMIFBFDPK = new ArgsDict();
			kEMMIFBFDPK.Add("fightResult", nHIDAJFLHJN);
			kEMMIFBFDPK.Add("isSurrender", flag2);
			kEMMIFBFDPK.Add("avgFps", OEIFGEHDHLE);
			kEMMIFBFDPK.Add("fightList", KGKDKENMAOA);
			kEMMIFBFDPK.Add("fightTimeElapsed", PIFMOMMPFFM);
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Fight_End, kEMMIFBFDPK);
		}
		bool flag3 = false;
		if (!flag2)
		{
			KGKDKENMAOA.CNAOMDMIGLJ.EMFABIGKAHC(KGKDKENMAOA, nHIDAJFLHJN.IsWinner());
			if (KGKDKENMAOA.get_Type() == BattleType.FightPeriodic)
			{
				if (nHIDAJFLHJN.IsWinner())
				{
					KGKDKENMAOA.CNAOMDMIGLJ.JLPMOKPFECK(ListSF.IDMJOMOMDOJ());
					KGKDKENMAOA.FLKFFDLLBKA().BABOCEFFPII();
				}
				OKHLHAMGLOE(KGKDKENMAOA, nHIDAJFLHJN.IsWinner());
			}
			BattleType pJMEMGHKKBM = KGKDKENMAOA.get_Type();
			if (pJMEMGHKKBM == BattleType.FightReplayable || pJMEMGHKKBM == BattleType.FightBossesReplayable || pJMEMGHKKBM == BattleType.FightFinalReplayable)
			{
				BattleReplayable bKKPCBGAEHC = (BattleReplayable)KGKDKENMAOA.CNAOMDMIGLJ;
				if (bKKPCBGAEHC.MNHLGELMOEJ() == ConditionStatus.StatusComplete)
				{
					ListSF.KNCECPAINLI(bKKPCBGAEHC);
				}
			}
			flag3 = ListSF.ELEBLBJKDBI().IMDGMNFHFCN(nHIDAJFLHJN);
		}
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		if (KGKDKENMAOA.get_Type() == BattleType.FightAscension)
		{
			Battle cGJCGEBPCAF = ListSF.MKHAAGMJOPG(KGKDKENMAOA.BCKFACGMOKC);
			BattleAscension bGFLODNGLPK = (BattleAscension)cGJCGEBPCAF;
			if (flag2 || !nHIDAJFLHJN.IsWinner())
			{
				bGFLODNGLPK.LAGLOEEPGIO(1);
				ListSF.KNCECPAINLI(bGFLODNGLPK);
				hHKLFIIBIFF.JLGLBLDPAAF = KGKDKENMAOA.BCKFACGMOKC;
				if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RESET_ASCENSION))
				{
					ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
				}
			}
			else
			{
				bGFLODNGLPK.IDLDHJBJEII(KGKDKENMAOA);
				if (bGFLODNGLPK.NNPNEABKHPP() != null && bGFLODNGLPK.NNPNEABKHPP().PHCFNACJAAJ() > bGFLODNGLPK.KCIKELGFHOA())
				{
					bGFLODNGLPK.LAGLOEEPGIO(1);
					ListSF.KNCECPAINLI(bGFLODNGLPK);
				}
			}
			bGFLODNGLPK.FLMLLDJIHMD();
		}
		hHKLFIIBIFF.JLGLBLDPAAF = KGKDKENMAOA.BCKFACGMOKC;
		hHKLFIIBIFF.HEIADONEACH = (nHIDAJFLHJN.IsWinner() ? "Win" : ((!flag2) ? "Loss" : "Surrender"));
		hHKLFIIBIFF.BJIDALJIKNC = (flag3 ? 1 : 0);
		hHKLFIIBIFF.fightAvgFps = OEIFGEHDHLE;
		if (KGKDKENMAOA.get_Type() == BattleType.FightRaid)
		{
			hHKLFIIBIFF.OHPHPJBMNLH = KGKDKENMAOA.BCKFACGMOKC.CPHDPCAECJN();
		}
		bool flag4 = false;
		List<RewardStruct> list = KGKDKENMAOA.APKPCGDBMEP();
		foreach (RewardStruct item in list)
		{
			RewardPrize cMHHEHILIIH = item.KOBOIFJNPMO(nKGLHEGIKKP.PINDEKDNCNL());
			RewardLottery fAPDEKOMOGH = cMHHEHILIIH.FAPDEKOMOGH;
			if (fAPDEKOMOGH != null)
			{
				flag4 = true;
				break;
			}
		}
		if (flag4 && nHIDAJFLHJN.IsWinner())
		{
			ListSF.ELEBLBJKDBI().HAOHNNFLOGK = hHKLFIIBIFF;
		}
		if ((!flag4 || !nHIDAJFLHJN.IsWinner()) && ((KGKDKENMAOA.get_Type() != BattleType.FightRaid && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_END)) || (KGKDKENMAOA.get_Type() == BattleType.FightRaid && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_END))) && flag)
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		MenuController.IAMGKKOINFC();
		if (LEDKPLCIEBP(KGKDKENMAOA.BCKFACGMOKC.ToString()) && nHIDAJFLHJN.IsWinner() && (KGKDKENMAOA.get_Type() == BattleType.FightBosses || KGKDKENMAOA.get_Type() == BattleType.FightFinalTitan || KGKDKENMAOA.get_Type() == BattleType.FightBossesReplayable || KGKDKENMAOA.get_Type() == BattleType.FightTournament || KGKDKENMAOA.get_Type() == BattleType.FightAscension))
		{
			NIDLHHCCDFJ = true;
		}
		MenuController.KGACOEJKEBP();
		if (!flag)
		{
			if (KGKDKENMAOA.get_Type() == BattleType.FightRaid)
			{
				int num2 = 0;
				ModelParameters kIKOGDEPGHB = ((!ABKBEJBICOA.IsPlayer) ? ABKBEJBICOA : LEBLJJCFKOP);
				float num3 = (ObscuredFloat)(kIKOGDEPGHB.KKMCHCNOHMB());
				float num4 = (float)num2 - num3;
				float num5 = 0f;
				if (num5 > num4)
				{
					num5 = num4;
				}
				float num6 = num4 - num5;
				if (nHIDAJFLHJN.IsWinner() || num6 > (float)num2 - num5)
				{
					num6 = (float)num2 - num5;
				}
				KAOPLEPILDH kAOPLEPILDH = ((!ABKBEJBICOA.IsPlayer) ? LEBLJJCFKOP : ABKBEJBICOA) as KAOPLEPILDH;
				if (JEGCHOOLDLB > 0 && kAOPLEPILDH == null)
				{
				}
			}
			else if (flag2)
			{
				FKMEIHGOFDD(nHIDAJFLHJN);
			}
			else if (Fight.OHNKFOHIAKG() != null)
			{
				Fight.OHNKFOHIAKG().BCFBHJOLGNL(nHIDAJFLHJN);
			}
		}
		if (nHIDAJFLHJN.IsWinner())
		{
			OFOKPNFGDMD("Stage Complete");
		}
		else
		{
			OFOKPNFGDMD("Stage Failed");
		}
	}

	public static void CGFHDKDJCPL()
	{
		if (ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_START_APPLICATION))
		{
			ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
		}
	}

	public static bool StartFight(FightList KGKDKENMAOA = null, bool FLLKCPMJOEL = false, Battle DPOOIONCEOA = null, bool CDFICPGIBEE = true, bool IINNCMDDLGE = true)
	{
		if (KGKDKENMAOA == null)
		{
			LLLOJBFMONN.Error("GameUtils::StartFight(..) Error! FightList is empty!");
			return false;
		}
		ScreenType iPKNDMINFMJ = Module.ELEBLBJKDBI().NMCNDOPKFJD();
		FightList jDIPBIHBGPF = null;
		if (iPKNDMINFMJ == ScreenType.ModuleDojo || iPKNDMINFMJ == ScreenType.ModuleFight)
		{
			jDIPBIHBGPF = FightHolder.fightList;
		}
		if (iPKNDMINFMJ == ScreenType.ModuleFight && KGKDKENMAOA == jDIPBIHBGPF)
		{
			return false;
		}
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		RosterFight pIGKOIFBOME = nKGLHEGIKKP.DBMHOBPNIIA(KGKDKENMAOA.BCKFACGMOKC);
		if (pIGKOIFBOME == null)
		{
			pIGKOIFBOME = nKGLHEGIKKP.OBAFPDGJHNN(KGKDKENMAOA.BCKFACGMOKC);
		}
		KGKDKENMAOA.HOCFLEMFFKC(pIGKOIFBOME);
		Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
		if (cNAOMDMIGLJ.get_Type() != BattleType.FightUnregister)
		{
			ListSF.CCDKHLAMKKO().MICFFKODJME(KGKDKENMAOA.BCKFACGMOKC);
		}
		if (IINNCMDDLGE)
		{
			QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
			hHKLFIIBIFF.JLGLBLDPAAF = KGKDKENMAOA.BCKFACGMOKC;
			hHKLFIIBIFF.HEIADONEACH = string.Empty;
			hHKLFIIBIFF.AIEHNBBFNPF = string.Empty;
			if (KGKDKENMAOA.get_Type() == BattleType.FightRaid)
			{
				hHKLFIIBIFF.OHPHPJBMNLH = KGKDKENMAOA.BCKFACGMOKC.CPHDPCAECJN();
			}
			if ((KGKDKENMAOA.get_Type() != BattleType.FightRaid && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_FIGHT_ENTER)) || (KGKDKENMAOA.get_Type() == BattleType.FightRaid && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_FIGHT_ENTER)))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
				return true;
			}
		}
		if (KGKDKENMAOA.get_Type() != BattleType.FightPeriodic)
		{
			KGKDKENMAOA.FLKFFDLLBKA().CKJFJFPBIFF(ListSF.BLBNJKJKMBM());
		}
		if (KGKDKENMAOA.get_Type() == BattleType.FightAscension)
		{
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		if (CDFICPGIBEE)
		{
			Sound.IFKCCDAIADF("snd_gong");
			KJNOABODHMG = false;
		}
		MapScene current = Scene<MapScene>.get_Current();
		if (!HHKHINLNCJB || (!(NFBKHONMMDL == KGKDKENMAOA.JKMJHIIMHPG + KGKDKENMAOA.CNAOMDMIGLJ.get_Name() + KGKDKENMAOA.Name) && !(KGKDKENMAOA.BCKFACGMOKC.CPHDPCAECJN() == "QuestBattle")))
		{
			if (current != null)
			{
				current.EnableMapButtons(false);
			}
			if (!Module.DLOKJOHNDID(ScreenType.ModuleFight, KGKDKENMAOA) && current != null)
			{
				current.EnableMapButtons(true);
			}
		}
		else
		{
			HHKHINLNCJB = false;
			RosterFight pIGKOIFBOME2 = nKGLHEGIKKP.DBMHOBPNIIA(KGKDKENMAOA.BCKFACGMOKC);
			if (pIGKOIFBOME2 == null)
			{
				pIGKOIFBOME2 = nKGLHEGIKKP.OBAFPDGJHNN(KGKDKENMAOA.BCKFACGMOKC);
			}
			KGKDKENMAOA.HOCFLEMFFKC(pIGKOIFBOME2);
			int num = KGKDKENMAOA.APKPCGDBMEP().Count - 2;
			if (num < 0)
			{
				num = 0;
			}
			KGKDKENMAOA.JABJLCEJDDM = num;
			ComboStatistic aIOMDIAFHGB = new ComboStatistic();
			EndFight(aIOMDIAFHGB, KGKDKENMAOA, ListSF.CCDKHLAMKKO().get_Parameters(), null, GameOverTypes.GAME_OVER_WIN);
			if (current != null)
			{
				current.GetInfoBattle().UpdateBattleInfo(current.GetInfoBattle().GetCurrentBattle());
			}
		}
		FightHolder.fightList = KGKDKENMAOA;
		return true;
	}

	public static void MGBPBNNNLLO()
	{
		ListSF.ChangeEnergy(10);
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		LocalNotificationManager.ELEBLBJKDBI().OHMBBMKPAHD();
		LocalNotificationManager.ELEBLBJKDBI().GONAFNDNGHK();
	}

	public static void MIKJIBPCLLF()
	{
		Module.DLOKJOHNDID(ScreenType.ModuleProfile, 0);
	}

	public static string BGGMLFLFONJ()
	{
		return EOKAPIGANEE;
	}

	public static void PALADCEPDNI(string value)
	{
		EOKAPIGANEE = value;
	}

	public static string MNMGDBGCKOM()
	{
		return MODJMLNMBJH;
	}

	public static void NCGINDFMIFB(string value)
	{
		MODJMLNMBJH = value;
	}

	public static void EBNJAECDDIM(XmlNode AFHNINCKJEE)
	{
		GMKKGOHMEOM.Clear();
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if ("Item" == childNode.Name)
			{
				string gBCLEDJAOBM = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
				string pOFHDGJAFMP = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
				GMKKGOHMEOM.Add(new global::Pair<string, string>(gBCLEDJAOBM, pOFHDGJAFMP));
			}
		}
	}

	public static string GetDefaultItem(string CNKBLODAFDO)
	{
		foreach (global::Pair<string, string> item in GMKKGOHMEOM)
		{
			if (item.First == CNKBLODAFDO)
			{
				return item.Second;
			}
		}
		return string.Empty;
	}

	public static long ECCPJAPIABG()
	{
		return ListSF.IDMJOMOMDOJ();
	}

	public static long GetLeftTime(long time)
	{
		long num = ECCPJAPIABG();
		return (time <= num) ? 0 : (time - num);
	}

	public static void OEKOKKCILAG()
	{
		LocalNotificationManager.ELEBLBJKDBI().ECNMCOKOEBF();
		LocalNotificationManager.ELEBLBJKDBI().IMABAABEIOI(LJNCELBEHGN);
	}

	public static void ShowEnterScreen(string HCPNFPMHFCM, Action FLHKCGAMDJJ)
	{
		EnterScreen enterScreen = EnterScreen.Create();
		enterScreen.Init(HCPNFPMHFCM, FLHKCGAMDJJ);
	}

	public static void ShowEnterScreen(List<KeyValuePair<string, int>> IGLEKOAILHD, Action FLHKCGAMDJJ)
	{
		EnterScreen enterScreen = EnterScreen.Create();
		enterScreen.Init(IGLEKOAILHD, FLHKCGAMDJJ);
	}

	public static Trick NMNIGIDFKOA(string name)
	{
		return KLLGJKHALGH().Find((Trick GNAONAPDDLD) => GNAONAPDDLD.Name == name);
	}

	public static List<Trick> KLLGJKHALGH(SceneTypes MHOCFOODLLL = SceneTypes.SceneFight)
	{
		List<Trick> list = new List<Trick>();
		List<ItemInfo> hELFDCAIJNE = LBMPHBNJMGG().PJNJIJIODHE();
		List<PerkInfoItem> jOGBKOJCINM = LBMPHBNJMGG().JBIOECDAAKP();
		bool aBGINCCBACK = false;
		AnimationData.PHNMANPDPKG(list, hELFDCAIJNE, aBGINCCBACK, null, jOGBKOJCINM, MHOCFOODLLL);
		return list;
	}

	private static bool LEDKPLCIEBP(string DIAIIPCBMFL)
	{
		int count = _notShowRateFights.Count;
		for (int i = 0; i < count; i++)
		{
			if (_notShowRateFights[i] == DIAIIPCBMFL)
			{
				return false;
			}
		}
		return true;
	}

	public static void LFKOMCMPKKC(ItemInfo item, ItemAction LFLGCDNKNJI, int count, long BMNFPNBAMAF)
	{
	}

	public static void PKMIJDBNNFK(string CPMFDAEAHAM)
	{
	}

	private static void NIANNOBAHMH(string LOKLDPLAPOL, RecipeItemInfo item)
	{
		if (item != null && item.MFEAIEJFDAM() != null)
		{
			string text = item.MFEAIEJFDAM().get_Name();
			string lIOGIBJBHAH = LocalizationManager.GetString(LOKLDPLAPOL, "1", text);
			long num = item.HGDELDFDFNH() - ECCPJAPIABG();
			if (num > 0)
			{
				LocalNotificationManager.ELEBLBJKDBI().EGOMGODAMFF(lIOGIBJBHAH, num);
			}
		}
	}

	private static void FBINMBCCMHA(string LOKLDPLAPOL, RecipeItemInfo item)
	{
		if (item != null && item.MFEAIEJFDAM() != null)
		{
			LocalNotificationManager.ELEBLBJKDBI().ENAFDJHIDJJ();
		}
	}

	private static void DMBJCCFNFKK(string LOKLDPLAPOL, UserItem item)
	{
		if (item != null)
		{
			string text = LocalizationManager.GetString(item.get_Name());
			string lIOGIBJBHAH = LocalizationManager.GetString(LOKLDPLAPOL, "1", text);
			long num = item.IJGAOHJNLAH() - ECCPJAPIABG();
			if (num > 0)
			{
				LocalNotificationManager.ELEBLBJKDBI().HGOKJEIHKPE(lIOGIBJBHAH, num);
			}
		}
	}

	private static void NELKFJJHFDC(string LOKLDPLAPOL, UserItem item)
	{
		if (item != null)
		{
			LocalNotificationManager.ELEBLBJKDBI().DJNHJBNKBIB();
		}
	}

	private static void OKHLHAMGLOE(FightList KGKDKENMAOA, bool EEEJJGMOKDF)
	{
		if (KGKDKENMAOA.get_Type() == BattleType.FightPeriodic)
		{
			RosterFight pIGKOIFBOME = KGKDKENMAOA.FLKFFDLLBKA();
			long iHDMLLNEGIK = ((!EEEJJGMOKDF) ? (pIGKOIFBOME.FDAEBPDIEEE() + KGKDKENMAOA.RepeatTime - ListSF.IDMJOMOMDOJ()) : (pIGKOIFBOME.ILBNPNIPEHO() + KGKDKENMAOA.RepeatTime - ListSF.IDMJOMOMDOJ()));
			LocalNotificationManager.ELEBLBJKDBI().AHLFKAGBLEN();
			LocalNotificationManager.ELEBLBJKDBI().HPCBBNCDPEB(iHDMLLNEGIK);
		}
	}

	public static void PIHNKCIDDJB()
	{
		ShopScene current = Scene<ShopScene>.get_Current();
		if (current != null)
		{
			current.UpdateNewItemsCounters();
		}
	}

	public static void FBEHNHNOKKO(bool value)
	{
		IBCOLFJCDNG = value;
	}

	public static bool NMODJEJFFNC()
	{
		return IBCOLFJCDNG;
	}

	public static float FCMBGDFIBPK()
	{
		return 650f;
	}

	public static void InitVariables()
	{
		LDBMFAMEMPF = false;
		LEEIGNICAMN = false;
		GBCMHICHIOI = false;
	}

	public static SliderType NAMBCLFLNIN(string LIKHBAFOEND)
	{
		foreach (KeyValuePair<SliderType, string> item in IPFBLJOALBN)
		{
			if (item.Value == LIKHBAFOEND)
			{
				return item.Key;
			}
		}
		return SliderType.SliderNone;
	}

	public static float JEILJMPPEGL(FightList fight)
	{
		List<ModelParameters> list = IGNNMAKHBFF(fight.OFKJMHPMCCD());
		ModelParameters kIKOGDEPGHB = LBMPHBNJMGG().Clone();
		kIKOGDEPGHB.KMPACCIOOLE(fight.EHGIKANKJNJ(), true);
		kIKOGDEPGHB.NOBKKLBJFIL();
		float result = fight.MPNBGBIMEIP(kIKOGDEPGHB, list);
		list.Clear();
		return result;
	}

	public static int ParseAspects(XmlNode AFHNINCKJEE)
	{
		OKMEHFCEPIK.Clear();
		int num = 0;
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			if (childNode.Name.Equals("Aspect"))
			{
				Aspect hOHAPDGFMHL = new Aspect();
				hOHAPDGFMHL.Parse(childNode);
				OKMEHFCEPIK.Add(hOHAPDGFMHL);
				num++;
			}
		}
		return num;
	}

	public static void CFHKBAEOOBK(XmlNode AFHNINCKJEE)
	{
		JDHBJMHAJOG().LFIPMCAHODJ = AFHNINCKJEE.Attributes["Antilimit"].ParseFloat();
		JDHBJMHAJOG().JHJAFHLMOBJ = AFHNINCKJEE.Attributes["DoublingRange"].ParseFloat();
		JDHBJMHAJOG().GPEPDPOJJLM = AFHNINCKJEE.Attributes["Limit"].ParseFloat();
	}

	public static void KLHOKMCALFM()
	{
		foreach (Aspect item in IGHPKGCJGHM())
		{
			WarriorAttribute bCNOAOPGAEI = new WarriorAttribute();
			bCNOAOPGAEI.set_Name(item.get_Name());
			bCNOAOPGAEI.GDCBBAHKCIE = true;
			bCNOAOPGAEI.GDECIAJAFHH = true;
			bCNOAOPGAEI.KDKHPMHNPCN = true;
			bCNOAOPGAEI.GMPLHIHNHMD = true;
			BGENALLCKII.IBLHIAHECLK.Add(bCNOAOPGAEI);
		}
	}

	public static Aspect MGDCJKKGKAB(string name)
	{
		List<Aspect> list = IGHPKGCJGHM();
		foreach (Aspect item in list)
		{
			if (item.get_Name().Equals(name))
			{
				return item;
			}
		}
		return null;
	}

	public static void MEBABPEMMBE()
	{
		IPFBLJOALBN[SliderType.SliderNone] = "Default";
		IPFBLJOALBN[SliderType.SliderWeapon] = "Weapon";
		IPFBLJOALBN[SliderType.SliderArmor] = "Armor";
		IPFBLJOALBN[SliderType.SliderHelmet] = "Helm";
		IPFBLJOALBN[SliderType.SliderMissile] = "Ranged";
		IPFBLJOALBN[SliderType.SliderMagic] = "Magic";
		IPFBLJOALBN[SliderType.SliderRuby] = "Ruby";
		IPFBLJOALBN[SliderType.SliderFree] = "Free";
		IPFBLJOALBN[SliderType.SliderCheat] = "Cheat";
		IPFBLJOALBN[SliderType.SliderPerks] = "Perks";
		IPFBLJOALBN[SliderType.SliderTricks] = "Moves";
		IPFBLJOALBN[SliderType.SliderAchievements] = "Achievements";
		IPFBLJOALBN[SliderType.SliderSeals] = "QuestItems";
		IPFBLJOALBN[SliderType.SliderCount] = "Count";
		IPFBLJOALBN[SliderType.SliderRaidItemPack] = "RaidConsumable";
		IPFBLJOALBN[SliderType.SliderRaidMap] = "RaidMapStage";
		IPFBLJOALBN[SliderType.SliderStoryMap] = "StoryMapStage";
	}

	public static void LGEPJJPDNOO()
	{
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutNotStarted] = "NotStarted";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutToggleButton] = "RaidButtonTutorial";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutGiveKey] = "RaidGiveKeyTutorial";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutFirstRaidEnter] = "FirstRaidEnterTutorial";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutFinalFight] = "RaidFinalFightTutorial";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutLeagueWindow] = "LeagueWindowTutorial";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutShopNotification] = "RaidShopNotification";
		HEJIFIHLLJF[RaidTutorialStepCode.RaidTutComplete] = "TutorialComplete";
	}

	public static RaidTutorialStepCode PHHOCKCCGMM(string NOFMEBBEDKK)
	{
		foreach (KeyValuePair<RaidTutorialStepCode, string> item in HEJIFIHLLJF)
		{
			if (item.Value == NOFMEBBEDKK)
			{
				return item.Key;
			}
		}
		return RaidTutorialStepCode.RaidTutNotStarted;
	}

	public static void AHJGPLGCNGI()
	{
		foreach (Achievement item in CCNGPMBGNAC)
		{
			item.HGMHEOGJDMM = true;
		}
		CCNGPMBGNAC.Clear();
	}

	public static long GetDenominatedValue(long value, int NPFOBKBJAOB = 0)
	{
		int num = ListSF.CCDKHLAMKKO().NPGECMDDNFO();
		float f = (float)value / Mathf.Pow(10f, num - NPFOBKBJAOB);
		return (long)Mathf.Ceil(f);
	}

	public static bool DJCDFEAMPDA(FightList KGKDKENMAOA)
	{
		List<CurrencyCostRule> list = KGKDKENMAOA.LBGNOMEFLBA();
		if (list.Count == 0)
		{
			return true;
		}
		foreach (CurrencyCostRule item in list)
		{
			string text = item.JFDCHNBPPNH();
			if (!(text != string.Empty))
			{
				continue;
			}
			int num = ListSF.CCDKHLAMKKO().GetCurrencyCount(text);
			int num2 = 0;
			foreach (CurrencyCostRule item2 in list)
			{
				if (item2.JFDCHNBPPNH() == text)
				{
					int num3 = item2.LHNHLANLHMN();
					num2 += num3;
				}
			}
			if (num < num2)
			{
				return false;
			}
		}
		return true;
	}

	public static void OFOKPNFGDMD(string DOPHKKGNAEF)
	{
	}

	public static bool ILFJCODGINO(PerkInfoItem AEFFHJGMNFI)
	{
		List<UserItem> list = ListSF.CCDKHLAMKKO().KHCNHPCPFII().JCMOHPFKPBO();
		for (int i = 0; i < list.Count; i++)
		{
			if (COLBOMHLHBB(list[i].BHKHOJPANHE()) && !list[i].OJNNHFNPNEM(AEFFHJGMNFI))
			{
				return false;
			}
		}
		return true;
	}

	private static bool COLBOMHLHBB(ItemInfo item)
	{
		// Newer vanilla lists can legitimately omit historical/seasonal item
		// definitions still present in an existing local save. Such UserItems
		// remain useful as save data, but they cannot participate in equipment
		// perk compatibility until an ItemInfo exists in the active list.
		if (item == null)
		{
			return false;
		}
		switch (item.Type)
		{
		case "Weapon":
		case "Armor":
		case "Helm":
		case "Ranged":
		case "Magic":
			return true;
		default:
			return false;
		}
	}

	public static void ICAEEDLKGEF()
	{
		if (AssemblyController.BNIHABLDELL())
		{
			LLLOJBFMONN.INNGABABJPC(string.Empty);
			LLLOJBFMONN.INNGABABJPC("------------------------------Print texture cache ------------------------------");
			LLLOJBFMONN.INNGABABJPC("------------------------------Screen: " + Module.INIOOEKJIDI(Module.ELEBLBJKDBI().NMCNDOPKFJD()) + " ------------------------------");
		}
	}
}
