using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ModelParameters
{
	public enum EGDGFNAFELM
	{
		EstimationTypeNone = 0,
		EstimationTypeShowDifficulty = 1,
		EstimationTypeAttributesAlign = 2
	}

	public enum IHFKGJLIPGH
	{
		DFHard = 0,
		DFNormal = 1,
		DFBoth = 2
	}

	private static HashSet<GroupModel> AKMJJHALCNJ = new HashSet<GroupModel>();

	private static List<RatingEvaluation> PPNILGFDOLN = new List<RatingEvaluation>();

	private static float GOLHPEBMEDB;

	private static float GGKLECFGBOP;

	private static string PCLCPAMGMLF;

	public List<GroupModel> KFKKHACFDPH = new List<GroupModel>();

	public SceneTypes IBBALIJOJMC;

	public List<AttributesAlign> FKJBBIMPCBB = new List<AttributesAlign>();

	public List<string> MNPAALCFAKL = new List<string>();

	public List<ItemInfo> HEKILHEHMMH = new List<ItemInfo>();

	public List<ItemInfo> OJIAKDDCGLB = new List<ItemInfo>();

	public List<int> IAIHFLGBIPB = new List<int>();

	public List<string> DANNKMJOOOH = new List<string>();

	public List<string> KOELCOMEJMI = new List<string>();

	public int PEBKEBIBAFA;

	public float MEECPNMPFPG;

	public bool PMHHMDAIOGL;

	public bool IsPlayer;

	public bool EEGMBGBLLIF;

	public bool ABAPAIEBNGK;

	public bool HGHDBNPIFEJ;

	public List<int> JCMFKLGCEOG = new List<int>();

	public Vector3f JJCKADKCDIF;

	public string HHKODEICDNP;

	public int FLGGADFNNDK;

	public int EHFNCDPPIAF;

	public string OLPCELPEDKD;

	public XmlNode Node;

	public bool KKFBCOKMNDF;

	public ItemInfo PILJCAOFAED;

	public ItemInfo JGMLKIPCFII;

	public ItemInfo LKKFNMBCCDB;

	public ItemInfo FKMOLBBLKDA;

	public ItemInfo LGHMILECPLA;

	public ItemInfo ADBKGIBBNHJ;

	public ItemInfo KKJJONOBHKI;

	public float KDHBBGLCGIL;

	public float CIDCNCDFONA;

	public EndRoundType EndRoundType;

	public bool IsWinner;

	public bool BHHLEBHLBLH;

	public bool PCALDKCJGCK;

	public bool DKAHKGBFJMG;

	public bool HKJFJHBHMND;

	public bool KMNLACDHAFE;

	public bool ABLMGLAKJBL;

	public bool EAJHPCJJCDI;

	public bool IDPHHPNCFED;

	public int FCOALLOHJNP;

	public int HJNOICKOFDL;

	public int AKLPHMOAIGK;

	public int FPIMGHKNHMO;

	// Total one-bar health pools authored by the raid template/warrior.
	public int ShieldTotal;

	public bool HasShieldTotalOverride;

	// Life stays normalized for existing perks, rules and round comparisons.
	// Each incoming combat delta is one bar's worth, not the whole raid pool.
	public int HealthBarCount { get { return System.Math.Max(1, ShieldTotal); } }

	// Combat damage is measured in single-bar units; stored life is a fraction
	// of the whole pool. Only convert at damage comparisons/application, not in
	// the normalized health getters used by perks, AI and the HUD.
	public float RemainingHealthInDamageUnits
	{
		get { return (float)_CurrentLife * HealthBarCount; }
	}

	public float ResolveStrikeDamage(float damage, out bool overkill)
	{
		float remaining = RemainingHealthInDamageUnits;
		overkill = remaining < damage;
		// Retain the original final-hit margin, but in the same units as damage.
		return overkill ? remaining + 0.01f : damage;
	}

	public int RemainingHealthBars
	{
		get { return (int)System.Math.Ceiling(HealthBarsLeft); }
	}

	private double HealthBarsLeft
	{
		get
		{
			double bars = System.Math.Max(0d, System.Math.Min(1d, HABJPOFCIHA())) * HealthBarCount;
			double rounded = System.Math.Round(bars);
			// Repeated float damage must not leave a phantom, almost-empty bar.
			return rounded > 0d && System.Math.Abs(bars - rounded) < 0.0001d ? rounded : bars;
		}
	}

	public float CurrentHealthBarFraction
	{
		get
		{
			double bars = HealthBarsLeft;
			return bars <= 0d ? 0f : (float)(bars - System.Math.Ceiling(bars) + 1d);
		}
	}

	public int ALCFNGIKCCB;

	public uint LotteryLevel;

	public float KFMJMBANIGF;

	public float EHBHNGOGCKO;

	public string BMFLPBLAFLK;

	public string FMOKLKFCCKF;

	public string HNKFHGOOKEG;

	public string CHFEHBNIGKA;

	public List<PerkInfoItem> NHBIJEEKALC = new List<PerkInfoItem>();

	public List<PerkInfoItem> GIKPDPFOAIL = new List<PerkInfoItem>();

	public List<PerkInfoItem> JGCNPHDGHAK = new List<PerkInfoItem>();

	public Attributes MAGFMAFCHLP;

	public Attributes IBLHIAHECLK;

	private float CHACMOJKLAB;

	private float PJGDPCJBLMA;

	private float MEHKADPAHKP;

	private float PIBJOEMLEGF;

	private float NFENFPMIBFD;

	private float CEFONNONAKG;

	public Tactic HBFMBOHLKPJ;

	private ObscuredInt CMOKGMKBGBB;

	private ObscuredFloat _CurrentLife;

	private ObscuredInt DEGCGHDAMDA;

	private ObscuredInt KBPOKMKFIAD;

	private ObscuredInt AFHOBFEEHPL;

	private bool LNHMCKNCGDP;

	public ObscuredInt Level
	{
		get
		{
			return PINDEKDNCNL();
		}
		set
		{
			DLDMOHEGENM(value);
		}
	}

	public ObscuredFloat GPKALMFLOPP
	{
		get
		{
			return KKMCHCNOHMB();
		}
	}

	public ObscuredInt OBJAIHDENNM
	{
		get
		{
			return OJLKDEHMIAC();
		}
	}

	public List<PerkInfoItem> FDAHAAFFGCA
	{
		get
		{
			return JBIOECDAAKP();
		}
	}

	public bool IsGroup
	{
		get
		{
			return IJKINPHBHCF();
		}
	}

	public bool INPPKAHEHEL
	{
		get
		{
			return AGICDDJBPLB();
		}
		set
		{
			set_IsImmortalityEnabled(value);
		}
	}

	public float MEMMPHBHIMM
	{
		get
		{
			return HABJPOFCIHA();
		}
	}

	public bool ICICCLIHNEH
	{
		get
		{
			return OJMIFOAHKBK();
		}
	}

	private bool MCKBGFPLDFM
	{
		get
		{
			return FDDBPFJBHEB();
		}
	}

	private float EJMOKAICKNF
	{
		get
		{
			return MONFIEOOICJ();
		}
	}

	private float LDPFDOKPPJL
	{
		get
		{
			return ENBCGBBGFIK();
		}
	}

	public float DKNKIMMFAIM
	{
		get
		{
			return PMHIIOJPDLO();
		}
		set
		{
			FOPCCNDPFOE(value);
		}
	}

	public float HDBCEDKMGBB
	{
		get
		{
			return CEKIBEJELBM();
		}
		set
		{
			KCLCLMNOIDJ(value);
		}
	}

	public float OCLAMKEGKAN
	{
		get
		{
			return GBKEKEDPBIB();
		}
		set
		{
			JPOCPFLJKDC(value);
		}
	}

	public float HMIEHGJGMIF
	{
		get
		{
			return GCEDCHHBJGM();
		}
		set
		{
			AJLEAGJFKPD(value);
		}
	}

	public float JJNLCJIDFIP
	{
		get
		{
			return FLLKBDGJIKO();
		}
		set
		{
			LEIPGKLCKOP(value);
		}
	}

	public float JLCCEHKHEPI
	{
		get
		{
			return GGIOPHOMCCL();
		}
		set
		{
			NHPLLBNKOAF(value);
		}
	}

	public ModelParameters()
	{
		NHBIJEEKALC = new List<PerkInfoItem>();
		IBLHIAHECLK = new Attributes();
		MAGFMAFCHLP = new Attributes();
		HNKFHGOOKEG = string.Empty;
		PILJCAOFAED = null;
		JGMLKIPCFII = null;
		LKKFNMBCCDB = null;
		FKMOLBBLKDA = null;
		LGHMILECPLA = null;
		ADBKGIBBNHJ = null;
		IsPlayer = false;
		EEGMBGBLLIF = true;
		ABAPAIEBNGK = false;
		IsWinner = false;
		BHHLEBHLBLH = false;
		PCALDKCJGCK = false;
		DKAHKGBFJMG = true;
		HKJFJHBHMND = true;
		EndRoundType = EndRoundType.EndRoundTypeNone;
		PEBKEBIBAFA = 1;
		KDHBBGLCGIL = 0f;
		CIDCNCDFONA = 0f;
		FCOALLOHJNP = 0;
		HJNOICKOFDL = 0;
		AKLPHMOAIGK = 0;
		FPIMGHKNHMO = 0;
		ALCFNGIKCCB = 0;
		LotteryLevel = 0u;
		KFMJMBANIGF = 0f;
		EHBHNGOGCKO = 0f;
		FLGGADFNNDK = 0;
		KMNLACDHAFE = false;
		HGHDBNPIFEJ = false;
		OLPCELPEDKD = string.Empty;
		PMHHMDAIOGL = false;
		HBFMBOHLKPJ = null;
		KKFBCOKMNDF = false;
		IDPHHPNCFED = false;
		MEECPNMPFPG = 0f;
		JJCKADKCDIF = Vector3f.op_Implicit(new Vector3(-100f, -100f, -100f));
		EHFNCDPPIAF = 0;
		KKJJONOBHKI = null;
		ABLMGLAKJBL = false;
		EAJHPCJJCDI = false;
		IBBALIJOJMC = SceneTypes.SceneNone;
		CHACMOJKLAB = -1f;
		PJGDPCJBLMA = -1f;
		MEHKADPAHKP = -1f;
		PIBJOEMLEGF = -1f;
		NFENFPMIBFD = -1f;
		CEFONNONAKG = -1f;
		_CurrentLife = (ObscuredFloat)(0f);
		DEGCGHDAMDA = (ObscuredInt)(-1);
		KBPOKMKFIAD = (ObscuredInt)(-1);
		AFHOBFEEHPL = (ObscuredInt)(-1);
	}

	public ModelParameters(ModelParameters NBMGOEMJJAF)
	{
		NHBIJEEKALC = new List<PerkInfoItem>(NBMGOEMJJAF.NHBIJEEKALC);
		GIKPDPFOAIL = new List<PerkInfoItem>(NBMGOEMJJAF.GIKPDPFOAIL);
		JGCNPHDGHAK = new List<PerkInfoItem>(NBMGOEMJJAF.JGCNPHDGHAK);
		IBLHIAHECLK = new Attributes(NBMGOEMJJAF.IBLHIAHECLK);
		MAGFMAFCHLP = new Attributes(NBMGOEMJJAF.MAGFMAFCHLP);
		DLDMOHEGENM(NBMGOEMJJAF.PINDEKDNCNL());
		BMFLPBLAFLK = NBMGOEMJJAF.BMFLPBLAFLK;
		HNKFHGOOKEG = NBMGOEMJJAF.HNKFHGOOKEG;
		PILJCAOFAED = NBMGOEMJJAF.PILJCAOFAED;
		JGMLKIPCFII = NBMGOEMJJAF.JGMLKIPCFII;
		LKKFNMBCCDB = NBMGOEMJJAF.LKKFNMBCCDB;
		FKMOLBBLKDA = NBMGOEMJJAF.FKMOLBBLKDA;
		LGHMILECPLA = NBMGOEMJJAF.LGHMILECPLA;
		ADBKGIBBNHJ = NBMGOEMJJAF.ADBKGIBBNHJ;
		MNPAALCFAKL.AddRange(NBMGOEMJJAF.MNPAALCFAKL);
		IsPlayer = NBMGOEMJJAF.IsPlayer;
		EEGMBGBLLIF = NBMGOEMJJAF.EEGMBGBLLIF;
		ABAPAIEBNGK = NBMGOEMJJAF.ABAPAIEBNGK;
		IsWinner = NBMGOEMJJAF.IsWinner;
		BHHLEBHLBLH = NBMGOEMJJAF.BHHLEBHLBLH;
		PCALDKCJGCK = NBMGOEMJJAF.PCALDKCJGCK;
		DKAHKGBFJMG = NBMGOEMJJAF.DKAHKGBFJMG;
		HKJFJHBHMND = NBMGOEMJJAF.HKJFJHBHMND;
		EndRoundType = NBMGOEMJJAF.EndRoundType;
		PEBKEBIBAFA = NBMGOEMJJAF.PEBKEBIBAFA;
		KDHBBGLCGIL = NBMGOEMJJAF.KDHBBGLCGIL;
		CIDCNCDFONA = NBMGOEMJJAF.CIDCNCDFONA;
		FCOALLOHJNP = NBMGOEMJJAF.FCOALLOHJNP;
		HJNOICKOFDL = NBMGOEMJJAF.HJNOICKOFDL;
		AKLPHMOAIGK = NBMGOEMJJAF.AKLPHMOAIGK;
		FPIMGHKNHMO = NBMGOEMJJAF.FPIMGHKNHMO;
		ShieldTotal = NBMGOEMJJAF.ShieldTotal;
		HasShieldTotalOverride = NBMGOEMJJAF.HasShieldTotalOverride;
		ALCFNGIKCCB = NBMGOEMJJAF.ALCFNGIKCCB;
		LotteryLevel = NBMGOEMJJAF.LotteryLevel;
		KFMJMBANIGF = NBMGOEMJJAF.KFMJMBANIGF;
		EHBHNGOGCKO = NBMGOEMJJAF.EHBHNGOGCKO;
		FLGGADFNNDK = NBMGOEMJJAF.FLGGADFNNDK;
		KMNLACDHAFE = NBMGOEMJJAF.KMNLACDHAFE;
		HGHDBNPIFEJ = NBMGOEMJJAF.KMNLACDHAFE;
		OLPCELPEDKD = NBMGOEMJJAF.OLPCELPEDKD;
		PMHHMDAIOGL = NBMGOEMJJAF.PMHHMDAIOGL;
		HBFMBOHLKPJ = NBMGOEMJJAF.HBFMBOHLKPJ;
		KKFBCOKMNDF = NBMGOEMJJAF.KKFBCOKMNDF;
		Node = NBMGOEMJJAF.Node;
		IDPHHPNCFED = NBMGOEMJJAF.IDPHHPNCFED;
		MEECPNMPFPG = NBMGOEMJJAF.MEECPNMPFPG;
		JJCKADKCDIF = new Vector3f(NBMGOEMJJAF.JJCKADKCDIF);
		EHFNCDPPIAF = NBMGOEMJJAF.EHFNCDPPIAF;
		KKJJONOBHKI = NBMGOEMJJAF.KKJJONOBHKI;
		ABLMGLAKJBL = NBMGOEMJJAF.ABLMGLAKJBL;
		EAJHPCJJCDI = NBMGOEMJJAF.EAJHPCJJCDI;
		IBBALIJOJMC = NBMGOEMJJAF.IBBALIJOJMC;
		CHACMOJKLAB = NBMGOEMJJAF.CHACMOJKLAB;
		PJGDPCJBLMA = NBMGOEMJJAF.PJGDPCJBLMA;
		MEHKADPAHKP = NBMGOEMJJAF.MEHKADPAHKP;
		PIBJOEMLEGF = NBMGOEMJJAF.PIBJOEMLEGF;
		NFENFPMIBFD = NBMGOEMJJAF.NFENFPMIBFD;
		CEFONNONAKG = NBMGOEMJJAF.CEFONNONAKG;
		_CurrentLife = NBMGOEMJJAF._CurrentLife;
		DEGCGHDAMDA = NBMGOEMJJAF.DEGCGHDAMDA;
		KBPOKMKFIAD = NBMGOEMJJAF.KBPOKMKFIAD;
		AFHOBFEEHPL = NBMGOEMJJAF.AFHOBFEEHPL;
		FKJBBIMPCBB = new List<AttributesAlign>(NBMGOEMJJAF.FKJBBIMPCBB);
	}

	public ObscuredInt PINDEKDNCNL()
	{
		return CMOKGMKBGBB;
	}

	public void DLDMOHEGENM(ObscuredInt value)
	{
		CMOKGMKBGBB = value;
	}

	public ObscuredFloat KKMCHCNOHMB()
	{
		return _CurrentLife;
	}

	public ObscuredInt OJLKDEHMIAC()
	{
		return DEGCGHDAMDA;
	}

	public void RandomizeObscuredVars()
	{
		CMOKGMKBGBB.GMCADPGOCHM();
		_CurrentLife.GMCADPGOCHM();
		DEGCGHDAMDA.GMCADPGOCHM();
		KBPOKMKFIAD.GMCADPGOCHM();
		AFHOBFEEHPL.GMCADPGOCHM();
	}

	public void PFNDNOMGFBC(ItemInfo item)
	{
		OJIAKDDCGLB.AddIfNotExist(item);
	}

	public void LHLEIAKJANI(PerkInfoItem AEFFHJGMNFI)
	{
		GIKPDPFOAIL.AddIfNotExist(AEFFHJGMNFI);
	}

	public List<ItemInfo> DGMDEDKLGMB()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		ItemInfo dJKEECEOCJB = KDABEFBJMOD("Skeleton");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Weapon");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Ranged");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Magic");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("RaidConsumable");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Armor");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Helm");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		dJKEECEOCJB = KDABEFBJMOD("Cheat");
		if (dJKEECEOCJB != null)
		{
			list.Add(dJKEECEOCJB);
		}
		return list;
	}

	public List<ItemInfo> PJNJIJIODHE()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		if (PILJCAOFAED != null)
		{
			list.Add(PILJCAOFAED);
		}
		if (JGMLKIPCFII != null)
		{
			list.Add(JGMLKIPCFII);
		}
		if (LGHMILECPLA != null)
		{
			list.Add(LGHMILECPLA);
		}
		if (ADBKGIBBNHJ != null)
		{
			list.Add(ADBKGIBBNHJ);
		}
		if (LKKFNMBCCDB != null)
		{
			list.Add(LKKFNMBCCDB);
		}
		if (FKMOLBBLKDA != null)
		{
			list.Add(FKMOLBBLKDA);
		}
		return list;
	}

	public ItemInfo KDABEFBJMOD(string LMNNBBKHMEI)
	{
		switch (LMNNBBKHMEI)
		{
		case "Skeleton":
			return PILJCAOFAED;
		case "Weapon":
			return JGMLKIPCFII;
		case "Ranged":
			return LGHMILECPLA;
		case "Magic":
			return ADBKGIBBNHJ;
		case "Armor":
			return LKKFNMBCCDB;
		case "Helm":
			return FKMOLBBLKDA;
		default:
			return null;
		}
	}

	public void OLLNIKFPMKE(string LMNNBBKHMEI, ItemInfo item)
	{
		switch (LMNNBBKHMEI)
		{
		case "Skeleton":
			PILJCAOFAED = item;
			break;
		case "Weapon":
			JGMLKIPCFII = item;
			break;
		case "Ranged":
			LGHMILECPLA = item;
			break;
		case "Magic":
			ADBKGIBBNHJ = item;
			break;
		case "Armor":
			LKKFNMBCCDB = item;
			break;
		case "Helm":
			FKMOLBBLKDA = item;
			break;
		}
	}

	private string HNLFPEOANFA(string LMNNBBKHMEI)
	{
		switch (LMNNBBKHMEI)
		{
		case "HeadDefense":
			return FKMOLBBLKDA.Name;
		case "BodyDefense":
			return LKKFNMBCCDB.Name;
		case "UnarmedDamage":
			return LKKFNMBCCDB.Name;
		case "WeaponDamage":
			return JGMLKIPCFII.Name;
		case "RangedDamage":
			return LGHMILECPLA.Name;
		case "MagicDamage":
			return ADBKGIBBNHJ.Name;
		default:
			return null;
		}
	}

	private ItemInfo JELHKJHBNMF(string LMNNBBKHMEI)
	{
		switch (LMNNBBKHMEI)
		{
		case "HeadDefense":
			return FKMOLBBLKDA;
		case "BodyDefense":
			return LKKFNMBCCDB;
		case "UnarmedDamage":
			return LKKFNMBCCDB;
		case "WeaponDamage":
			return JGMLKIPCFII;
		case "RangedDamage":
			return LGHMILECPLA;
		case "MagicDamage":
			return ADBKGIBBNHJ;
		default:
			return null;
		}
	}

	public void PPFDLIBLNDG()
	{
		MNPAALCFAKL.Clear();
		if (PILJCAOFAED != null && !string.IsNullOrEmpty(PILJCAOFAED.KJDFJPBIGJC))
		{
			MNPAALCFAKL.Add(OKALHAKMOLI(PILJCAOFAED.KJDFJPBIGJC));
		}
		if (JGMLKIPCFII != null && !string.IsNullOrEmpty(JGMLKIPCFII.KJDFJPBIGJC))
		{
			MNPAALCFAKL.Add(OKALHAKMOLI(JGMLKIPCFII.KJDFJPBIGJC));
		}
		if (LKKFNMBCCDB != null && !string.IsNullOrEmpty(LKKFNMBCCDB.KJDFJPBIGJC))
		{
			MNPAALCFAKL.Add(OKALHAKMOLI(LKKFNMBCCDB.KJDFJPBIGJC));
		}
		if (FKMOLBBLKDA != null && !string.IsNullOrEmpty(FKMOLBBLKDA.KJDFJPBIGJC))
		{
			MNPAALCFAKL.Add(OKALHAKMOLI(FKMOLBBLKDA.KJDFJPBIGJC));
		}
		for (int i = 0; i < HEKILHEHMMH.Count; i++)
		{
			MNPAALCFAKL.Add(OKALHAKMOLI(HEKILHEHMMH[i].KJDFJPBIGJC));
		}
	}

	public int DGLDFMCEDDO(string name, ref bool GMEMHMOHFGG)
	{
		List<ItemInfo> hELFDCAIJNE = PJNJIJIODHE();
		return DGLDFMCEDDO(name, hELFDCAIJNE, ref GMEMHMOHFGG);
	}

	public int DGLDFMCEDDO(string name, List<ItemInfo> HELFDCAIJNE, ref bool GMEMHMOHFGG)
	{
		GMEMHMOHFGG = false;
		int num = 0;
		for (int i = 0; i < HELFDCAIJNE.Count; i++)
		{
			int OEMALIFPGPO = 0;
			if (HELFDCAIJNE[i].IBLHIAHECLK.Get(name, ref OEMALIFPGPO))
			{
				GMEMHMOHFGG = true;
				num += OEMALIFPGPO;
			}
		}
		return num;
	}

	private int LGJAHBCFMCF(string name, ref bool GMEMHMOHFGG)
	{
		int num = 0;
		for (int i = 0; i < NHBIJEEKALC.Count; i++)
		{
			int OEMALIFPGPO = 0;
			if (NHBIJEEKALC[i].IBLHIAHECLK.Get(name, ref OEMALIFPGPO))
			{
				GMEMHMOHFGG = true;
				num += OEMALIFPGPO;
			}
		}
		return num;
	}

	public void NOBKKLBJFIL()
	{
		KELCMMKNHEH();
		List<ItemInfo> hELFDCAIJNE = PJNJIJIODHE();
		List<WarriorAttribute> iBLHIAHECLK = GameUtils.BGENALLCKII.IBLHIAHECLK;
		for (int i = 0; i < iBLHIAHECLK.Count; i++)
		{
			string text = iBLHIAHECLK[i].get_Name();
			int OEMALIFPGPO = 0;
			if (MAGFMAFCHLP.Get(text, ref OEMALIFPGPO))
			{
				IBLHIAHECLK.Set(text, OEMALIFPGPO, true);
				continue;
			}
			int num = 0;
			bool GMEMHMOHFGG = false;
			num += DGLDFMCEDDO(text, hELFDCAIJNE, ref GMEMHMOHFGG);
			num += LGJAHBCFMCF(text, ref GMEMHMOHFGG);
			int OEMALIFPGPO2 = 0;
			if (GameUtils.KJJBEHBGKMK.KGMDIGIONNB.Get(text, ref OEMALIFPGPO2))
			{
				num += OEMALIFPGPO2;
				GMEMHMOHFGG = true;
			}
			int OEMALIFPGPO3 = 0;
			if (GameUtils.MKHOLKGKNID.KGMDIGIONNB.Get(text, ref OEMALIFPGPO3))
			{
				num += (ObscuredInt)(CMOKGMKBGBB) * OEMALIFPGPO3;
				GMEMHMOHFGG = true;
			}
			if (GMEMHMOHFGG || !iBLHIAHECLK[i].GMPLHIHNHMD)
			{
				IBLHIAHECLK.Set(text, num, true);
			}
		}
	}

	public List<PerkInfoItem> JBIOECDAAKP()
	{
		List<PerkInfoItem> list = new List<PerkInfoItem>();
		list.AddRange(NHBIJEEKALC);
		list.AddRange(GIKPDPFOAIL);
		list.AddRange(JGCNPHDGHAK);
		List<ItemInfo> list2 = PJNJIJIODHE();
		foreach (ItemInfo item in list2)
		{
			bool bAINMLLIKOL = item.Type == "Weapon";
			foreach (PerkInfoItem item2 in item.NHBIJEEKALC)
			{
				item2.HILDOOOKHGN(bAINMLLIKOL);
				list.Add(item2);
			}
			if (item.GNDLEFFMJDJ || !IsPlayer)
			{
				continue;
			}
			UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(item);
			if (dKCHDHMLKHN == null)
			{
				continue;
			}
			List<PerkInfoItem> list3 = ListSF.KJBMBFHCEIM(item, IsPlayer);
			foreach (PerkInfoItem item3 in list3)
			{
				if (item3.LELHEEDNMBP == PerkInfoItem.DNPGIEGCGKH.COMBO)
				{
					if (item.Type == "Weapon" && GameUtils.ILFJCODGINO(item3))
					{
						item3.HILDOOOKHGN(bAINMLLIKOL);
						list.Add(item3);
					}
				}
				else
				{
					item3.HILDOOOKHGN(bAINMLLIKOL);
					list.Add(item3);
				}
			}
		}
		JEJPEJFLDJC(list, KOELCOMEJMI);
		return list;
	}

	public void AJFGKPFJJNL()
	{
		NHBIJEEKALC.Clear();
		NHBIJEEKALC.AddRange(JBIOECDAAKP());
		JEJPEJFLDJC(NHBIJEEKALC, KOELCOMEJMI);
	}

	private string OKALHAKMOLI(string name)
	{
		return string.Format("{0}.xml", name);
	}

	public override string ToString()
	{
		return string.Format("User ID='{0}' SilhouetteItemID='{1}' WeaponID='{2}' Dan='{3}' Damage='{4}' Difficulty='{5}' FirstName='{6}' LastName='{7}'  Level='{8}' LotteryLevel='{9}' ", 0, LKKFNMBCCDB.NLMDNOBHHKP, (JGMLKIPCFII != null) ? JGMLKIPCFII.NLMDNOBHHKP : 0, AKLPHMOAIGK, KFMJMBANIGF, EHBHNGOGCKO, BMFLPBLAFLK, FMOKLKFCCKF, CMOKGMKBGBB, LotteryLevel);
	}

	public static void DPIDOBMONPA(XmlNode AFHNINCKJEE)
	{
		GOLHPEBMEDB = XmlUtils.ParseFloat(AFHNINCKJEE.Attributes["ImpossibleRatio"]);
		GGKLECFGBOP = XmlUtils.ParseFloat(AFHNINCKJEE.Attributes["EasyRatio"]);
		PPNILGFDOLN.Clear();
		PCLCPAMGMLF = XmlUtils.ParseString(AFHNINCKJEE.Attributes["PerkAspectParameter"]);
		foreach (XmlNode childNode in AFHNINCKJEE.ChildNodes)
		{
			RatingEvaluation dCAFHLLHFJO = new RatingEvaluation();
			PPNILGFDOLN.Add(dCAFHLLHFJO);
			dCAFHLLHFJO.DNKAJNCEGKF = childNode.Name;
			dCAFHLLHFJO.AOAPDHDACPJ = XmlUtils.ParseString(childNode.Attributes["Name"]);
			Evaluation.ParseAttributes(childNode, dCAFHLLHFJO.IBLHIAHECLK);
			Evaluation.BEFJGPEAJCF(childNode, dCAFHLLHFJO);
			Evaluation.MNFPBOPIHDE(childNode, dCAFHLLHFJO);
			dCAFHLLHFJO.GMODDPGBGHM = XmlUtils.ParseString(childNode.Attributes["CancellingItem"]);
			Defense.Parse(childNode, dCAFHLLHFJO.CKJBFNJEDHH);
		}
	}

	private void CPLONGFFFBJ(float ratio)
	{
		float num = (0f - GameUtils.BGJPLNFFEOB) * (Mathf.Log10(ratio) / Mathf.Log10(2f)) / 2f;
		float num2 = float.MinValue;
		foreach (AttributesAlign item in FKJBBIMPCBB)
		{
			float num3 = num * item.Factor + item.Shift;
			if (num2 < num3)
			{
				num2 = num3;
			}
		}
	}

	private float GetParametersValue(string name)
	{
		int OEMALIFPGPO = 0;
		if (IBLHIAHECLK.Get(name, ref OEMALIFPGPO))
		{
			return OEMALIFPGPO;
		}
		Debug.LogErrorFormat("Parameter \"{0}\" not found!", name);
		return float.MinValue;
	}

	private float GetParametersValue(List<string> NIKHAICFGNM)
	{
		float num = float.MinValue;
		foreach (string item in NIKHAICFGNM)
		{
			int OEMALIFPGPO = 0;
			if (IBLHIAHECLK.Get(item, ref OEMALIFPGPO) && num < (float)OEMALIFPGPO)
			{
				num = OEMALIFPGPO;
			}
		}
		if (num == float.MinValue)
		{
			Debug.LogError("Parameters not found!");
		}
		return num;
	}

	private float JKCCFHJHHII(List<Evaluation> EKJDIGGGEBH, ModelParameters JCICKLIMBEF)
	{
		float num = float.MinValue;
		foreach (Evaluation item in EKJDIGGGEBH)
		{
			float num2 = JCICKLIMBEF.GetParametersValue(item.Name) + item.Shift;
			if (num < num2)
			{
				num = num2;
			}
		}
		return num;
	}

	public float DJOIGHCCMJG(ModelParameters AKBNKDBHCEO, List<global::Pair<string, float>> PNLMJFLBGMA)
	{
		float num = 0f;
		List<global::Pair<string, float>> list = new List<global::Pair<string, float>>();
		foreach (RatingEvaluation item in PPNILGFDOLN)
		{
			if (!(item.DNKAJNCEGKF == "Damage"))
			{
				continue;
			}
			RatingEvaluation dCAFHLLHFJO = item;
			if (HasItem(dCAFHLLHFJO.GMODDPGBGHM))
			{
				continue;
			}
			float cDCIEOFCKNO = dCAFHLLHFJO.CDCIEOFCKNO;
			list.Clear();
			AIHCLEKCBPF(list, dCAFHLLHFJO.IBLHIAHECLK);
			BKMGIJHCMPI(list, PNLMJFLBGMA);
			List<Defense> cKJBFNJEDHH = dCAFHLLHFJO.CKJBFNJEDHH;
			BIAFHJKHFGE(AKBNKDBHCEO, cKJBFNJEDHH, PNLMJFLBGMA);
			float num2 = 0f;
			foreach (Defense item2 in cKJBFNJEDHH)
			{
				if (item2.IBLHIAHECLK.Count != 1)
				{
					Debug.LogError("Count of DefenseAttribute != 1");
				}
				if (HasItem(item2.GMODDPGBGHM))
				{
					continue;
				}
				float num3 = GameUtils.GetAttributesHitMultiplier(IsPlayer, this, AKBNKDBHCEO, list, item2.IBLHIAHECLK[0].Name);
				num3 = Mathf.Min(1f, cDCIEOFCKNO * num3);
				List<PerkInfoItem> list2 = JBIOECDAAKP();
				foreach (PerkInfoItem item3 in list2)
				{
					List<Rating> mLMLENHGNDJ = item3.MLMLENHGNDJ;
					if (mLMLENHGNDJ.Count == 0)
					{
						continue;
					}
					foreach (Rating item4 in mLMLENHGNDJ)
					{
						if (!(item4.IHJJBIDMEMB != "Me") && (string.IsNullOrEmpty(item4.KFMJMBANIGF) || item4.KFMJMBANIGF == dCAFHLLHFJO.AOAPDHDACPJ) && (string.IsNullOrEmpty(item4.GBOKABKLCFM) || item4.GBOKABKLCFM == item2.AOAPDHDACPJ))
						{
							float num4 = 0f;
							if (!string.IsNullOrEmpty(PCLCPAMGMLF))
							{
								string s = item3.NGNJGOJJPLD(PCLCPAMGMLF);
								num4 = float.Parse(s);
							}
							num3 *= 1f + (item4.Multiplier - 1f) * PerkInfoItem.DODDEPEMBMC(num4 - AKBNKDBHCEO.GetParametersValue(item4.BIOIOGIBCOE));
						}
					}
				}
				List<PerkInfoItem> list3 = AKBNKDBHCEO.JBIOECDAAKP();
				foreach (PerkInfoItem item5 in list3)
				{
					List<Rating> mLMLENHGNDJ2 = item5.MLMLENHGNDJ;
					if (mLMLENHGNDJ2.Count == 0)
					{
						continue;
					}
					foreach (Rating item6 in mLMLENHGNDJ2)
					{
						if (!(item6.IHJJBIDMEMB != "Enemy") && (string.IsNullOrEmpty(item6.KFMJMBANIGF) || item6.KFMJMBANIGF == dCAFHLLHFJO.AOAPDHDACPJ) && (string.IsNullOrEmpty(item6.GBOKABKLCFM) || item6.GBOKABKLCFM == item2.AOAPDHDACPJ))
						{
							float num5 = 0f;
							if (!string.IsNullOrEmpty(PCLCPAMGMLF))
							{
								string s2 = item5.NGNJGOJJPLD(PCLCPAMGMLF);
								num5 = float.Parse(s2);
							}
							num3 /= 1f + (item6.Multiplier - 1f) * PerkInfoItem.DODDEPEMBMC(num5 - AKBNKDBHCEO.GetParametersValue(item6.BIOIOGIBCOE));
						}
					}
				}
				num2 += item2.Weight * num3;
			}
			if (dCAFHLLHFJO.OFHGAJDLIDB > 0f)
			{
				float oFHGAJDLIDB = dCAFHLLHFJO.OFHGAJDLIDB;
				float num6 = GameUtils.DILKHIFCCGD.JGHFCAPPDED();
				float num7 = GameUtils.DILKHIFCCGD.HOOBFKANPEK();
				float num8 = GameUtils.DILKHIFCCGD.MPIOONCNFOK(this);
				float num9 = GameUtils.DILKHIFCCGD.LLKJJLOMNID(this);
				num2 *= oFHGAJDLIDB * (num6 * num8 + num7 * num9);
			}
			num += num2;
		}
		return num;
	}

	private bool HasItem(string OHCGEEEKEJH)
	{
		List<ItemInfo> list = PJNJIJIODHE();
		foreach (ItemInfo item in list)
		{
			if (item.Name == OHCGEEEKEJH)
			{
				return true;
			}
		}
		return false;
	}

	private void BKMGIJHCMPI(List<global::Pair<string, float>> FFJLHDENIEB, List<global::Pair<string, float>> PNLMJFLBGMA)
	{
		if (PNLMJFLBGMA == null)
		{
			return;
		}
		foreach (global::Pair<string, float> item in PNLMJFLBGMA)
		{
			string lLHEDBIEHAA = item.First;
			float nFNBFHCDEGG = item.Second;
			for (int i = 0; i < FFJLHDENIEB.Count; i++)
			{
				if (FFJLHDENIEB[i].First == lLHEDBIEHAA)
				{
					FFJLHDENIEB[i] = new global::Pair<string, float>(FFJLHDENIEB[i].First, FFJLHDENIEB[i].Second + nFNBFHCDEGG);
					break;
				}
			}
		}
	}

	private void BIAFHJKHFGE(ModelParameters AKBNKDBHCEO, List<Defense> PLPANBKKOEN, List<global::Pair<string, float>> PNLMJFLBGMA)
	{
		if (PNLMJFLBGMA == null)
		{
			return;
		}
		foreach (global::Pair<string, float> item in PNLMJFLBGMA)
		{
			string lLHEDBIEHAA = item.First;
			float nFNBFHCDEGG = item.Second;
			for (int i = 0; i < PLPANBKKOEN.Count; i++)
			{
				foreach (Evaluation item2 in PLPANBKKOEN[i].IBLHIAHECLK)
				{
					if (item2.Name == lLHEDBIEHAA)
					{
						int OEMALIFPGPO = 0;
						AKBNKDBHCEO.IBLHIAHECLK.Get(lLHEDBIEHAA, ref OEMALIFPGPO);
						AKBNKDBHCEO.IBLHIAHECLK.Set(lLHEDBIEHAA, (int)((float)OEMALIFPGPO + nFNBFHCDEGG));
					}
				}
			}
		}
	}

	public void JEJPEJFLDJC(List<PerkInfoItem> JOGBKOJCINM, List<string> NIKHAICFGNM)
	{
		foreach (string item in NIKHAICFGNM)
		{
			for (int num = JOGBKOJCINM.Count - 1; num >= 0; num--)
			{
				if (ILAFHEDMNNL(JOGBKOJCINM[num], item))
				{
					JOGBKOJCINM.RemoveAt(num);
				}
			}
		}
	}

	private bool ILAFHEDMNNL(PerkInfoItem AEFFHJGMNFI, string name)
	{
		if (AEFFHJGMNFI == null)
		{
			return false;
		}
		return AEFFHJGMNFI.IsPerkByNames(name);
	}

	public bool UpdateLife(float DLEDDPFNPOH)
	{
		GEACPINOAAN(DLEDDPFNPOH);
		if (OJMIFOAHKBK())
		{
			PCALDKCJGCK = true;
		}
		return PCALDKCJGCK;
	}

	public bool IJKINPHBHCF()
	{
		return KFKKHACFDPH.Count > 0;
	}

	public bool AGICDDJBPLB()
	{
		return LNHMCKNCGDP;
	}

	public void set_IsImmortalityEnabled(bool value)
	{
		LNHMCKNCGDP = value;
	}

	public void GFNCMLFKBGP(float value)
	{
		if (AGICDDJBPLB())
		{
			_CurrentLife = (ObscuredFloat)(CIDCNCDFONA);
		}
		else if (value < 0f)
		{
			_CurrentLife = (ObscuredFloat)(0f);
		}
		else if (value > CIDCNCDFONA)
		{
			_CurrentLife = (ObscuredFloat)(CIDCNCDFONA);
		}
		else
		{
			_CurrentLife = (ObscuredFloat)(value);
		}
	}

	public void GEACPINOAAN(float value)
	{
		float next = (ObscuredFloat)(_CurrentLife) + value / HealthBarCount;
		// Float subtraction across many shields can leave a microscopic last
		// bar and prevent death even after the exact pool has been depleted.
		if (HealthBarCount > 1 && value < 0f && next * HealthBarCount < 0.0001f)
			next = 0f;
		GFNCMLFKBGP(next);
	}

	private int AIHCLEKCBPF(string name, List<string> OEMALIFPGPO)
	{
		int count = OEMALIFPGPO.Count;
		foreach (RatingEvaluation item in PPNILGFDOLN)
		{
			if (!(item.DNKAJNCEGKF == name))
			{
				continue;
			}
			foreach (Evaluation item2 in item.IBLHIAHECLK)
			{
				OEMALIFPGPO.AddIfNotExist(item2.Name);
			}
		}
		return OEMALIFPGPO.Count - count;
	}

	private int FIIIOBACJBJ(string name, List<string> OEMALIFPGPO)
	{
		int count = OEMALIFPGPO.Count;
		foreach (RatingEvaluation item in PPNILGFDOLN)
		{
			if (!(item.DNKAJNCEGKF == name))
			{
				continue;
			}
			foreach (Defense item2 in item.CKJBFNJEDHH)
			{
				foreach (Evaluation item3 in item2.IBLHIAHECLK)
				{
					OEMALIFPGPO.AddIfNotExist(item3.Name);
				}
			}
		}
		return OEMALIFPGPO.Count - count;
	}

	private int AIHCLEKCBPF(List<global::Pair<string, float>> OEMALIFPGPO, List<Evaluation> JMMIKHLIKOE)
	{
		int count = OEMALIFPGPO.Count;
		foreach (Evaluation item in JMMIKHLIKOE)
		{
			OEMALIFPGPO.Add(new global::Pair<string, float>(item.Name, item.Shift));
		}
		return OEMALIFPGPO.Count - count;
	}

	private int DMPKOKPEDBA(List<string> OEMALIFPGPO)
	{
		return AIHCLEKCBPF("Damage", OEMALIFPGPO);
	}

	private int AEPOPABPMHB(List<string> OEMALIFPGPO)
	{
		return AIHCLEKCBPF("Magic", OEMALIFPGPO);
	}

	private int APPNAJNKMJM(List<string> OEMALIFPGPO)
	{
		return AIHCLEKCBPF("Ranged", OEMALIFPGPO);
	}

	private int OMEDOHGAAHA(List<string> OEMALIFPGPO)
	{
		return FIIIOBACJBJ("Damage", OEMALIFPGPO);
	}

	private int PEBDHELFHMJ(List<string> OEMALIFPGPO)
	{
		return FIIIOBACJBJ("Magic", OEMALIFPGPO);
	}

	private int BFEMIDCPJMH(List<string> OEMALIFPGPO)
	{
		return FIIIOBACJBJ("Ranged", OEMALIFPGPO);
	}

	public void HGLJEBABMIH()
	{
		KDHBBGLCGIL = (ObscuredFloat)(_CurrentLife);
	}

	public float HABJPOFCIHA()
	{
		return CIDCNCDFONA > 0f ? (ObscuredFloat)(_CurrentLife) / CIDCNCDFONA : 0f;
	}

	public bool OJMIFOAHKBK()
	{
		return (ObscuredFloat)(_CurrentLife) <= 0f;
	}

	public void ALNNLCAKCAF(float AOGLLMEFEJB = 1f)
	{
		float num = (ObscuredFloat)(_CurrentLife);
		num += AOGLLMEFEJB;
		num = Mathf.Min(num, CIDCNCDFONA);
		num = Mathf.Max(0f, num);
		GFNCMLFKBGP(num);
	}

	public void BCLGFKDDNKH()
	{
		GFNCMLFKBGP(CIDCNCDFONA);
	}

	private bool FDDBPFJBHEB()
	{
		return FCOALLOHJNP >= HJNOICKOFDL;
	}

	public ModelParameters Clone()
	{
		return new ModelParameters(this);
	}

	private void KELCMMKNHEH()
	{
		IBLHIAHECLK.Clear();
	}

	private void SetAttributes(ModelParameters IHEFAMAFBIA)
	{
		IBLHIAHECLK = IHEFAMAFBIA.IBLHIAHECLK;
	}

	public void ALBOCOGOBCN(EquippedItemsStruct HELFDCAIJNE)
	{
		HELFDCAIJNE.LKKFNMBCCDB = LKKFNMBCCDB;
		HELFDCAIJNE.FKMOLBBLKDA = FKMOLBBLKDA;
		HELFDCAIJNE.KKJJONOBHKI = KKJJONOBHKI;
		HELFDCAIJNE.PILJCAOFAED = PILJCAOFAED;
		HELFDCAIJNE.JGMLKIPCFII = JGMLKIPCFII;
		HELFDCAIJNE.ADBKGIBBNHJ = ADBKGIBBNHJ;
		HELFDCAIJNE.LGHMILECPLA = LGHMILECPLA;
	}

	public void ALGDEEKFPKK(EquippedItemsStruct HELFDCAIJNE)
	{
		LKKFNMBCCDB = HELFDCAIJNE.LKKFNMBCCDB;
		FKMOLBBLKDA = HELFDCAIJNE.FKMOLBBLKDA;
		KKJJONOBHKI = HELFDCAIJNE.KKJJONOBHKI;
		PILJCAOFAED = HELFDCAIJNE.PILJCAOFAED;
		JGMLKIPCFII = HELFDCAIJNE.JGMLKIPCFII;
		ADBKGIBBNHJ = HELFDCAIJNE.ADBKGIBBNHJ;
		LGHMILECPLA = HELFDCAIJNE.LGHMILECPLA;
	}

	private float MONFIEOOICJ()
	{
		return GOLHPEBMEDB;
	}

	private float ENBCGBBGFIK()
	{
		return GGKLECFGBOP;
	}

	private void FIBHNFMDBMH()
	{
		PPNILGFDOLN.Clear();
	}

	private void IPLKCNEGLKM()
	{
		float num = GetParametersValue("WeaponDamage");
		float num2 = GetParametersValue("UnarmedDamage");
		float num3 = GetParametersValue("BodyDefense");
		float num4 = GetParametersValue("HeadDefense");
		float num5 = GetParametersValue("RangedDamage");
		float num6 = GetParametersValue("MagicDamage");
		float num7 = GetParametersValue("RangedQuantity");
		LLLOJBFMONN.Write("- WeaponDamage:   {0}", num);
		LLLOJBFMONN.Write("- UnarmedDamage:  {0}", num2);
		LLLOJBFMONN.Write("- BodyDefense:    {0}", num3);
		LLLOJBFMONN.Write("- HeadDefense:    {0}", num4);
		LLLOJBFMONN.Write("- RangedDamage:   {0}", num5);
		LLLOJBFMONN.Write("- MagicDamage:    {0}", num6);
		LLLOJBFMONN.Write("- RangedQuantity: {0}", num7);
	}

	public void KMPACCIOOLE(List<ItemRule> GEEJLFGCKNJ, bool FFBFPLODJME, int round = 0)
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		foreach (ItemRule item in GEEJLFGCKNJ)
		{
			if (round > 0 && !item.HAKHBAOJBON(round))
			{
				continue;
			}
			UserItem dKCHDHMLKHN = item.get_Item();
			if (dKCHDHMLKHN == null)
			{
				LLLOJBFMONN.Error(" ModelParameters::setItemsFromRules - UserItem not found ");
				continue;
			}
			ItemInfo dJKEECEOCJB = dKCHDHMLKHN.BHKHOJPANHE();
			string text = dKCHDHMLKHN.get_Name();
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			UserItem dKCHDHMLKHN2 = nKGLHEGIKKP.KHCNHPCPFII().CMGOCLGHNLH(text);
			dJKEECEOCJB = null;
			if (dKCHDHMLKHN2 == null)
			{
				dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(text);
				if (dJKEECEOCJB == null)
				{
					LLLOJBFMONN.Error(" Model::equipRulesItems - item not found \"{0}\"", text);
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
				ItemInfo dJKEECEOCJB2 = KDABEFBJMOD(dJKEECEOCJB.Type);
				ItemInfo dJKEECEOCJB3 = dJKEECEOCJB.Clone();
				dJKEECEOCJB3.GNDLEFFMJDJ = true;
				OLLNIKFPMKE(dJKEECEOCJB.Type, dJKEECEOCJB3);
			}
		}
	}

	public float PMHIIOJPDLO()
	{
		return CHACMOJKLAB;
	}

	public void FOPCCNDPFOE(float value)
	{
		CHACMOJKLAB = value;
	}

	public float CEKIBEJELBM()
	{
		return PJGDPCJBLMA;
	}

	public void KCLCLMNOIDJ(float value)
	{
		PJGDPCJBLMA = value;
	}

	public float GBKEKEDPBIB()
	{
		return NFENFPMIBFD;
	}

	public void JPOCPFLJKDC(float value)
	{
		NFENFPMIBFD = value;
	}

	public float GCEDCHHBJGM()
	{
		return CEFONNONAKG;
	}

	public void AJLEAGJFKPD(float value)
	{
		CEFONNONAKG = value;
	}

	public void LEIPGKLCKOP(float value)
	{
		MEHKADPAHKP = value;
	}

	public float FLLKBDGJIKO()
	{
		return MEHKADPAHKP;
	}

	public void NHPLLBNKOAF(float value)
	{
		PIBJOEMLEGF = value;
	}

	public float GGIOPHOMCCL()
	{
		return PIBJOEMLEGF;
	}

	public void AHMMOKMGICA()
	{
		foreach (PerkInfoItem item in NHBIJEEKALC)
		{
			if (item.DLEAKGFKDBH())
			{
				item.DLDANNALFEA(false);
			}
		}
	}

	public void HANOHOBGGJF()
	{
		foreach (PerkInfoItem item in NHBIJEEKALC)
		{
			if (item != null)
			{
				item.DLDANNALFEA(true);
			}
		}
	}

	public void GPOIKJNPDIO(List<global::Pair<string, float>> LHGAKDLAPJB)
	{
		int i = 0;
		for (int count = LHGAKDLAPJB.Count; i < count; i++)
		{
			global::Pair<string, float> cCKLNOPEKHO = LHGAKDLAPJB[i];
			string lLHEDBIEHAA = cCKLNOPEKHO.First;
			float nFNBFHCDEGG = cCKLNOPEKHO.Second;
			int OEMALIFPGPO = 0;
			if (IBLHIAHECLK.Get(lLHEDBIEHAA, ref OEMALIFPGPO))
			{
				IBLHIAHECLK.Set(lLHEDBIEHAA, (int)((float)OEMALIFPGPO + nFNBFHCDEGG));
			}
		}
	}
}
