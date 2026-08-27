using System;
using System.Collections.Generic;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class Roster : MELBIBHDPCE
{
	public enum LHIKHHPIHPL
	{
		BALANCE_NONE = 0,
		BALANCE_BONUS = 1,
		BALANCE_BONUS_PAID = 2,
		BALANCE_MONEY = 3,
		BALANCE_MONEY_PAID = 4
	}

	public enum HPOIJPGPOCF
	{
		CHANGE_INIT = 0,
		CHANGE_PAYMENT = 1,
		CHANGE_ACHIEVEMENT = 2,
		CHANGE_FIGHT_REWARD = 3,
		CHANGE_VIDEO_VIEW = 4,
		CHANGE_FACEBOOK = 5,
		CHANGE_QUEST = 6,
		CHANGE_CHEAT = 7,
		CHANGE_SERVER_GIVE = 8,
		CHANGE_OFFER = 9,
		CHANGE_BUY_ITEM = 10,
		CHANGE_BUY_DELIVERY = 11,
		CHANGE_RAIDS_REWARD = 12,
		CHANGE_LEDGER = 13
	}

	public enum LDIJCCNBNNM
	{
		DELIVERY_ITEM = 0,
		DELIVERY_RECIPE = 1,
		ADD_CURRENCY = 2,
		ADD_RESISTANCE = 3,
		ECLIPSE_MODE_TOGGLE = 4
	}

	private string DNBFEGDILIA = SF2Paths.KKIDGPBOBNI() + "/music/";

	private string PDGHOOLJNMI = SF2Paths.KKIDGPBOBNI() + "/sounds/";

	public const int FFOOOBCMMNF = 1;

	public const int KOFIHALIJJL = 900;

	private XmlNode FPCJCIIBLND;

	private UserItems JEMDPOAHOAP = new UserItems();

	private UserAchievements EHLHFJLCKKE = new UserAchievements();

	private UserTutorials HKINOMCMDDL = new UserTutorials();

	private UserPerks ABBGMNHDECI;

	private XmlNode JADHFPKIHDF;

	private XmlNode GJDMCIILLBJ;

	private XmlNode IOFKIODDAMJ;

	private XmlNode ADEJBJHOEDF;

	private XmlNode OLEKIOJIFEK;

	private XmlNode AMCNNLHPCGI;

	private XmlNode HALBNLAFAMM;

	private XmlNode ALMPFGCDAOP;

	private XmlNode KHKMHPDKDIJ;

	private List<string> EOJCMGDPHLL = new List<string>();

	private List<RosterBattle> HLHEFIKFBHH = new List<RosterBattle>();

	private List<RosterFight> JNPMCNMEOLE = new List<RosterFight>();

	private List<RosterQuest> CNFCPCJPGLM = new List<RosterQuest>();

	private List<string> COJMHNDCACJ = new List<string>();

	private List<string> BMKAMGGIELK = new List<string>();

	private List<CurrencyStruct> DNPGEMMDPNN = new List<CurrencyStruct>();

	private List<ResistanceStruct> OEJCIFGACNG = new List<ResistanceStruct>();

	private static FMGDPLEEKEM EHAAMEPDOGJ;

	private ObscuredLong MJBFFBPLAGC;

	private ObscuredLong BDONIKLHFLJ;

	private ObscuredLong BIOMOEDPLIP = (ObscuredLong)(0L);

	private ObscuredLong IAFKGOFKDKE = (ObscuredLong)(0L);

	private uint _indexSlider;

	private ObscuredUInt _experience;

	private ObscuredInt _power;

	public int OGLHGFJKMCO;

	public int DIJOCFEFHAK;

	private long CHJNGJLCICH;

	private long HBPJBBJFHME = -1L;

	public bool ADKHNLAMDJP;

	private long GKLJJHLFACI;

	private long GKIONOFPEGB;

	private long BHOEHPFBIJE;

	private long JOLENHJGDLH;

	private bool AJGEKAADGEJ;

	private bool NPEENBBIFFB;

	private int CHDMBKEHHLH;

	private int CFNNEGHPCMN;

	private FightIDS GDFIGNHHKDC;

	private Color _MapMaskColor = Color.white;

	public bool CLODDOOGDBB;

	public int MMIMAJCKFKL;

	protected ModelParameters HEGIABHIPHA;

	protected string BGMCCMHOMJL = string.Empty;

	protected string NBBNANIILBL = string.Empty;

	protected XmlAttribute HPAHCBILEOE;

	protected FightIDS FHJHPGDPNBH = new FightIDS();

	protected XmlAttribute AALOLMPMCDH;

	protected FightIDS LINHCAGANFC = new FightIDS();

	private bool OKLFFBPGIAN;

	private long DLKPHLOAFGM;

	private RosterTimerContainer GNOMNIKGAPE;

	private bool JHJLHNHCPMP;

	public Dictionary<string, RosterQuest.NOKCOAHJIPB> INLEOAPIEDJ = new Dictionary<string, RosterQuest.NOKCOAHJIPB>();

	private bool NDIEOONABLA;

	private bool CFIMMOBMEIP;

	private bool GNCENEKPINA;

	private List<ObscuredUInt> _levelThresholds = new List<ObscuredUInt>();

	private int GAKAEGHGNGD;

	private string OFKGMKADHBD = string.Empty;

	private bool MCPCBHPLOPP;

	public XmlNode AIPFJFDHDBF
	{
		get
		{
			return BABKABBEFEL();
		}
	}

	public UserItems FCBGGMFCDDA
	{
		get
		{
			return KHCNHPCPFII();
		}
	}

	public UserAchievements IMGACJKJAPA
	{
		get
		{
			return KJNPJKEHGLE();
		}
	}

	public UserTutorials KEGPEFPCKOG
	{
		get
		{
			return BKBHIMEEDBG();
		}
	}

	public UserPerks MCFHMBCELDC
	{
		get
		{
			return JLBDOBLHHAF();
		}
	}

	public List<RosterBattle> IDDAHHFELEM
	{
		get
		{
			return IEANNFIECJA();
		}
	}

	public List<RosterFight> BKBFHLADPIM
	{
		get
		{
			return NIDBIFOJMAP();
		}
	}

	public List<RosterQuest> OEDLGIEPACN
	{
		get
		{
			return JNHBGEDJBLJ();
		}
	}

	public List<string> KBMFMBLNFLE
	{
		get
		{
			return AMAELLHKNDJ();
		}
	}

	public static FMGDPLEEKEM MIJLPJONGNM
	{
		get
		{
			return PKACFPCOHJH();
		}
	}

	public long JDPAGMPKLHB
	{
		get
		{
			return BFBOEGMAMNF();
		}
	}

	public long OHHLCBPGOIM
	{
		get
		{
			return EHFJHFDACMP();
		}
	}

	public ObscuredLong IHCGIHFKIGF
	{
		get
		{
			return KNHDCEBIMEE();
		}
		set
		{
			HGDLPMDHHOJ(value);
		}
	}

	public ObscuredLong LJIJBLBPODG
	{
		get
		{
			return FJGHKGPAPPN();
		}
		set
		{
			DBJLKIJNBHJ(value);
		}
	}

	public uint KFHNMIDOKAM
	{
		get
		{
			return BBLDLAIBOLP();
		}
		set
		{
			set_IndexSlider(value);
		}
	}

	public uint KGJJCGMHBKD
	{
		get
		{
			return HEOHJNFGEDH();
		}
	}

	public uint NIPCKCLLBLJ
	{
		get
		{
			return EOKLELGLHJJ();
		}
	}

	public int MFGLDPKEDJB
	{
		get
		{
			return NHKMGNPADKI();
		}
	}

	public long HNJMOKDHCHG
	{
		get
		{
			return FNDAKFILBOE();
		}
		set
		{
			PABEFKBJNEF(value);
		}
	}

	public long FDBNBGAGGBF
	{
		get
		{
			return NHFHDFIJEJG();
		}
	}

	public long KENOPHJGJDL
	{
		get
		{
			return NBDICCLKEAC();
		}
		set
		{
			BNGLIONOOAG(value);
		}
	}

	public long FELJIMDLIDI
	{
		get
		{
			return CPGGBLDAHBG();
		}
		set
		{
			DEPJCHIFFKA(value);
		}
	}

	public long OCKFAOHHOGO
	{
		get
		{
			return ECGBNDKJIPD();
		}
		set
		{
			DFBDNDMOLFH(value);
		}
	}

	public long KMGBCEEDEBC
	{
		get
		{
			return AHAAGOHCGHN();
		}
		set
		{
			ODPOOEPKJBP(value);
		}
	}

	public bool KAJIIOIPDCA
	{
		get
		{
			return HFINDOBJHNK();
		}
		set
		{
			MJLDOEOMLEG(value);
		}
	}

	public bool IIEHAMOGEHM
	{
		get
		{
			return ENBKLLMAALP();
		}
		set
		{
			PLBEEGGFKDH(value);
		}
	}

	public int AIHIDCEKGMH
	{
		get
		{
			return BGBFBIDOECK();
		}
	}

	public int FKDIBAHAJGG
	{
		get
		{
			return NPGECMDDNFO();
		}
		set
		{
			KGEHCFADNLI(value);
		}
	}

	public FightIDS CBMEHICIGFB
	{
		get
		{
			return HEBDMAIEAPM();
		}
		set
		{
			MICFFKODJME(value);
		}
	}

	public Color LHNBHJHEPNG
	{
		get
		{
			return EPEDEDLCAJF();
		}
		set
		{
			set_MapMaskColor(value);
		}
	}

	public ModelParameters KMMJCHDKBDO
	{
		get
		{
			return get_Parameters();
		}
	}

	public string OAPHJAPMKJG
	{
		get
		{
			return GKLECBABFCP();
		}
		set
		{
			COKACMKOIGD(value);
		}
	}

	public string HNHKICMKDEA
	{
		get
		{
			return OGJBDMNBMLJ();
		}
		set
		{
			HEIPPEGBOCK(value);
		}
	}

	public FightIDS IDEEJDJLFLH
	{
		get
		{
			return KNJNHKDCINB();
		}
		set
		{
			KPDHMBIDAHA(value);
		}
	}

	public FightIDS PEOPBHOAJLF
	{
		get
		{
			return MGICKOOCNAJ();
		}
		set
		{
			AEGEEMMBLDF(value);
		}
	}

	public bool IJOONMBEKDM
	{
		get
		{
			return MLOHMAGMIAI();
		}
		set
		{
			EKHEBJIOMFH(value);
		}
	}

	public long ELGFEIJCNEJ
	{
		get
		{
			return AACMNAJJKME();
		}
		set
		{
			EPGEKHNCEJF(value);
		}
	}

	public RosterTimerContainer GCEIELIEGJH
	{
		get
		{
			return AEMFLPNDDKL();
		}
	}

	public bool OGNHPKLGPDB
	{
		get
		{
			return JPMPIDFGCJL();
		}
	}

	public int Level
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

	public int JLPPKJPFJEO
	{
		get
		{
			return NPKBPGMNDFJ();
		}
	}

	public int DJCNNIDIHCE
	{
		get
		{
			return CNFOLIEFJCE();
		}
	}

	public bool CCJIANKFBGK
	{
		get
		{
			return DFNEGEEHLFJ();
		}
		set
		{
			OEAOPFDLMJJ(value);
		}
	}

	public bool FJLFILEFNNK
	{
		get
		{
			return FDPIBNJJDAK();
		}
		set
		{
			AJCCEFKDKIO(value);
		}
	}

	public bool LJEGNLNKJNF
	{
		get
		{
			return DHPNBBILDPB();
		}
		set
		{
			BCMKHEKOMDB(value);
		}
	}

	public string JCABJECCOCB
	{
		get
		{
			return ODMONBDLMIP();
		}
	}

	public string JHJDOOIJCBK
	{
		get
		{
			return NFKHNICBOIB();
		}
		set
		{
			AOIBKCOBABL(value);
		}
	}

	public bool JBKGCKKLHGO
	{
		get
		{
			return LDHANGLFDPJ();
		}
		set
		{
			MOBIJGMLNLI(value);
		}
	}

	public Roster(XmlNode node, ModelParameters JCICKLIMBEF)
		: base(node)
	{
		MMIMAJCKFKL = 0;
		CLODDOOGDBB = false;
		HCMLOIDALKC(node.Attributes["ServerUserID"].CIPOICEEIBK(string.Empty));
		AJCCEFKDKIO(node.Attributes["AskedForDumps"].ParseBool());
		HEGIABHIPHA = JCICKLIMBEF;
		PLELELJIKEL();
		_indexSlider = node.Attributes["IndexSlider"].ParseUint();
		NBBNANIILBL = node.Attributes["CoinIcon"].CIPOICEEIBK("MiscSprites.gold");
		if (NBBNANIILBL.Contains(".png"))
		{
			HEIPPEGBOCK(NBBNANIILBL.Replace(".png", string.Empty));
		}
		if (!NBBNANIILBL.Contains("MiscSprites"))
		{
			HEIPPEGBOCK(string.Format("{0}.{1}", "MiscSprites", NBBNANIILBL));
		}
		OIOOMAKNIOB(node.Attributes["Money"].ParseLong(0L));
		LLNELLFMMBB(node.Attributes["Bonus"].ParseLong(0L), HPOIJPGPOCF.CHANGE_INIT);
		IAFKGOFKDKE = (ObscuredLong)(node.Attributes["PaidBonus"].ParseLong(0L));
		BIOMOEDPLIP = (ObscuredLong)(node.Attributes["PaidMoney"].ParseLong(0L));
		CFNNEGHPCMN = node.Attributes["DenominationDigits"].ParseInt();
		FHCPEIGMGMK();
		DLDMOHEGENM(node.Attributes["Level"].ParseInt());
		_experience = (ObscuredUInt)(node.Attributes["Experience"].ParseUint());
		OGLHGFJKMCO = GameUtils.NAMEDMHAFKA();
		DIJOCFEFHAK = GameUtils.DIJOCFEFHAK;
		_power = (ObscuredInt)(node.Attributes["Power"].ParseInt());
		if ((ObscuredInt)(_power) > OGLHGFJKMCO)
		{
			_power = (ObscuredInt)(OGLHGFJKMCO);
		}
		ADKHNLAMDJP = false;
		CHJNGJLCICH = node.Attributes["PowerSyncTime"].ParseInt();
		BHOEHPFBIJE = node.Attributes["LastDailyTimeOffset"].ParseInt();
		JOLENHJGDLH = node.Attributes["LastEnergyTimeOffset"].ParseInt();
		GKLJJHLFACI = node.Attributes["LastDumpTime"].ParseInt();
		IGBNKIKIDII(node.Attributes["FightIDS"].CIPOICEEIBK(string.Empty));
		HPAHCBILEOE = node.Attributes["MapFocus"];
		NDFLHPGHKMP(node.Attributes["MapFocus"].CIPOICEEIBK(string.Empty));
		AALOLMPMCDH = node.Attributes["RaidMapFocus"];
		EOPPBJKPKGD(node.Attributes["RaidMapFocus"].CIPOICEEIBK(string.Empty));
		COKACMKOIGD(node.Attributes["Language"].CIPOICEEIBK(string.Empty));
		AJGEKAADGEJ = node.Attributes["ShowUpgrades"].ParseBool();
		NDIEOONABLA = node.Attributes["TrySocialLogin"].ParseBool(true);
		GKIONOFPEGB = node.Attributes["PeriodicPlayTime"].ParseLong(0L);
		OFKGMKADHBD = node.Attributes["CurrentZone"].CIPOICEEIBK(string.Empty);
		HKINOMCMDDL.Parse(node);
		JHJLHNHCPMP = node.Attributes["EclipseMode"].CIPOICEEIBK("Off") == "On";
		FPCJCIIBLND = node["Items"];
		JEMDPOAHOAP.Parse(FPCJCIIBLND);
		UserItem dKCHDHMLKHN = JEMDPOAHOAP.CMGOCLGHNLH("Unlimited_Energy");
		ADKHNLAMDJP = dKCHDHMLKHN != null;
		IOFKIODDAMJ = node["Battles"];
		foreach (XmlNode childNode in IOFKIODDAMJ.ChildNodes)
		{
			KJIMPNEGNAN(new RosterBattle(childNode));
		}
		JADHFPKIHDF = node["Fights"];
		if (JADHFPKIHDF != null)
		{
			foreach (XmlNode childNode2 in JADHFPKIHDF.ChildNodes)
			{
				AJKBFMLOCOF(new RosterFight(childNode2));
			}
		}
		GJDMCIILLBJ = node["Shop"];
		if (GJDMCIILLBJ != null)
		{
			foreach (XmlNode childNode3 in GJDMCIILLBJ.ChildNodes)
			{
				AddShopLock(childNode3.Attributes["Name"].CIPOICEEIBK(string.Empty));
			}
		}
		ABBGMNHDECI = new UserPerks(HEGIABHIPHA);
		ABBGMNHDECI.Parse(node);
		if (node["Quests"] != null)
		{
			ADEJBJHOEDF = node["Quests"]["Quests"];
			if (ADEJBJHOEDF != null)
			{
				foreach (XmlNode childNode4 in ADEJBJHOEDF.ChildNodes)
				{
					FDJJIIBHAOG(childNode4);
				}
			}
			INLEOAPIEDJ = new Dictionary<string, RosterQuest.NOKCOAHJIPB>();
			OLEKIOJIFEK = node["Quests"]["Variables"];
			if (OLEKIOJIFEK != null)
			{
				foreach (XmlNode childNode5 in OLEKIOJIFEK.ChildNodes)
				{
					OCDOPMPENHP(childNode5);
				}
			}
		}
		EHLHFJLCKKE.Parse(node);
		AMCNNLHPCGI = node["OpenTricks"];
		if (AMCNNLHPCGI != null)
		{
			foreach (XmlNode childNode6 in AMCNNLHPCGI.ChildNodes)
			{
				ENFKEIHBICK(childNode6.Attributes["Name"].CIPOICEEIBK(string.Empty), false);
			}
		}
		HALBNLAFAMM = node["Timers"];
		if (HALBNLAFAMM == null)
		{
			HALBNLAFAMM = node.ACBPMPMPKJJ("Timers");
		}
		GNOMNIKGAPE = new RosterTimerContainer(HALBNLAFAMM);
		OJNFOCEKFNC(node);
		// The decompiled roster never restored the dojo opponent toggle from
		// SessionSettings.  A click wrote Disciple correctly, but reloading the
		// dojo immediately reset the backing field to zero and made the selector
		// appear non-functional.
		int disciple;
		if (!int.TryParse(GetSettingsXML("Disciple"), out disciple))
		{
			disciple = 0;
		}
		CHDMBKEHHLH = (disciple != 0) ? 1 : 0;
		EJLOBJIFEAL();
		MapButtonController.ELEBLBJKDBI().Parse(node);
		ALMPFGCDAOP = node["Currencies"];
		if (ALMPFGCDAOP == null)
		{
			ALMPFGCDAOP = node.ACBPMPMPKJJ("Currencies");
		}
		BCLGMKICMJM(ALMPFGCDAOP);
		KHKMHPDKDIJ = node["Payments"];
		if (KHKMHPDKDIJ == null)
		{
			KHKMHPDKDIJ = node.ACBPMPMPKJJ("Payments");
		}
		if (EHAAMEPDOGJ == null)
		{
			EHAAMEPDOGJ = new FMGDPLEEKEM();
		}
		if (!EHAAMEPDOGJ.JAMADKCIMMB(node.Attributes["PaymentOrders"]))
		{
			EHAAMEPDOGJ.BPEPGALPBAE(KHKMHPDKDIJ);
		}
	}

	public XmlNode BABKABBEFEL()
	{
		return FPCJCIIBLND;
	}

	public UserItems KHCNHPCPFII()
	{
		return JEMDPOAHOAP;
	}

	public UserAchievements KJNPJKEHGLE()
	{
		return EHLHFJLCKKE;
	}

	public UserTutorials BKBHIMEEDBG()
	{
		return HKINOMCMDDL;
	}

	public UserPerks JLBDOBLHHAF()
	{
		return ABBGMNHDECI;
	}

	public List<RosterBattle> IEANNFIECJA()
	{
		return HLHEFIKFBHH;
	}

	public List<RosterFight> NIDBIFOJMAP()
	{
		return JNPMCNMEOLE;
	}

	public List<RosterQuest> JNHBGEDJBLJ()
	{
		return CNFCPCJPGLM;
	}

	public List<string> AMAELLHKNDJ()
	{
		return COJMHNDCACJ;
	}

	public static FMGDPLEEKEM PKACFPCOHJH()
	{
		return EHAAMEPDOGJ;
	}

	public long BFBOEGMAMNF()
	{
		return (ObscuredLong)(MJBFFBPLAGC);
	}

	public void OIOOMAKNIOB(long value)
	{
		long num = (long)Math.Pow(10.0, CFNNEGHPCMN);
		if (num <= 0)
		{
			num = 1L;
		}
		long num2 = (ObscuredLong)(MJBFFBPLAGC);
		MJBFFBPLAGC = (ObscuredLong)(value);
		EMDLLIGKONG("Money", (ObscuredLong)(MJBFFBPLAGC) * num);
	}

	public long EHFJHFDACMP()
	{
		return (ObscuredLong)(BDONIKLHFLJ);
	}

	public void LLNELLFMMBB(long value, HPOIJPGPOCF LFLGCDNKNJI, bool JEEOLJIFIOF = false)
	{
		long num = (ObscuredLong)(BDONIKLHFLJ);
		BDONIKLHFLJ = (ObscuredLong)(value);
		EMDLLIGKONG("Bonus", (ObscuredLong)(BDONIKLHFLJ));
		long num2 = Math.Abs(num - value);
		if (LFLGCDNKNJI == HPOIJPGPOCF.CHANGE_INIT)
		{
			return;
		}
		if (value > num)
		{
			if (JEEOLJIFIOF)
			{
				DBJLKIJNBHJ((ObscuredLong)((ObscuredLong)(IAFKGOFKDKE) + num2));
			}
			ArgsDict kEMMIFBFDPK = new ArgsDict();
			kEMMIFBFDPK["changed"] = num2;
			kEMMIFBFDPK["type"] = HEODNLPIJHL(LFLGCDNKNJI);
			kEMMIFBFDPK["isPaid"] = JEEOLJIFIOF;
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Gems_Changed, kEMMIFBFDPK);
		}
		else if (value < num)
		{
			long num3 = num - (ObscuredLong)(IAFKGOFKDKE);
			long num4 = Math.Max(0L, num2 - num3);
			DBJLKIJNBHJ((ObscuredLong)((ObscuredLong)(IAFKGOFKDKE) - num4));
		}
	}

	public ObscuredLong KNHDCEBIMEE()
	{
		return BIOMOEDPLIP;
	}

	public void HGDLPMDHHOJ(ObscuredLong value)
	{
		long num = (long)Math.Pow(10.0, CFNNEGHPCMN);
		if (num <= 0)
		{
			num = 1L;
		}
		long num2 = (ObscuredLong)(BIOMOEDPLIP);
		BIOMOEDPLIP = value;
		EMDLLIGKONG("PaidMoney", (ObscuredLong)(value) * num);
	}

	public ObscuredLong FJGHKGPAPPN()
	{
		return IAFKGOFKDKE;
	}

	public void DBJLKIJNBHJ(ObscuredLong value)
	{
		IAFKGOFKDKE = value;
		EMDLLIGKONG("PaidBonus", (ObscuredLong)(value));
	}

	public uint BBLDLAIBOLP()
	{
		return _indexSlider;
	}

	public void set_IndexSlider(uint value)
	{
		_indexSlider = value;
		EMDLLIGKONG("IndexSlider", value);
	}

	public uint HEOHJNFGEDH()
	{
		if (HEGIABHIPHA != null)
		{
			int num = (ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()) - 1;
			if (num < _levelThresholds.Count)
			{
				return (ObscuredUInt)(_levelThresholds[num]);
			}
		}
		return 2147483647u;
	}

	public uint EOKLELGLHJJ()
	{
		return (ObscuredUInt)(_experience);
	}

	public bool DBPBGBNHAIP(uint value)
	{
		if (HEGIABHIPHA == null)
		{
			return false;
		}
		bool flag = false;
		uint num = HEOHJNFGEDH();
		_experience = (ObscuredUInt)(value);
		uint num2 = (ObscuredUInt)(_experience);
		if (num <= num2)
		{
			flag = true;
			int num3 = (ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL());
			while (num <= num2)
			{
				num2 -= num;
				num3++;
				HEGIABHIPHA.DLDMOHEGENM((ObscuredInt)(num3));
				num = HEOHJNFGEDH();
				if (num2 > GameUtils.JNOGEPFLLDM)
				{
					num2 = GameUtils.JNOGEPFLLDM;
				}
			}
			if (num3 > _levelThresholds.Count)
			{
				num3 = _levelThresholds.Count;
				num2 = HEOHJNFGEDH();
				flag = false;
			}
			_experience = (ObscuredUInt)(num2);
			HEGIABHIPHA.DLDMOHEGENM((ObscuredInt)(num3));
			EMDLLIGKONG("Level", (ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()));
		}
		if ((ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()) == _levelThresholds.Count && (ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()) > 1)
		{
			num2 = (ObscuredUInt)(_levelThresholds[(ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()) - 2]);
			_experience = (ObscuredUInt)(num2);
		}
		if (flag)
		{
			KHCNHPCPFII().NHJAHNDOLAE();
			KHCNHPCPFII().UpdateLockItems(PINDEKDNCNL());
			GameUtils.PIHNKCIDDJB();
			ListSF.ELEBLBJKDBI().PLNBHLPHDJG(PINDEKDNCNL());
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Level_Up);
			GameUtils.OFOKPNFGDMD("Level Up");
		}
		EMDLLIGKONG("Experience", (ObscuredUInt)(_experience));
		return flag;
	}

	public bool IDGMHHAJDMO(uint value)
	{
		return DBPBGBNHAIP((ObscuredUInt)(_experience) + value);
	}

	public void BBHFFLJGDDL()
	{
		bool flag = DBPBGBNHAIP((ObscuredUInt)(_experience));
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		hHKLFIIBIFF.BJIDALJIKNC = (flag ? 1 : 0);
	}

	public int NHKMGNPADKI()
	{
		return (ObscuredInt)(_power);
	}

	public long FNDAKFILBOE()
	{
		return CHJNGJLCICH;
	}

	public void PABEFKBJNEF(long value)
	{
		CHJNGJLCICH = value;
		EMDLLIGKONG("PowerSyncTime", value);
	}

	public long NHFHDFIJEJG()
	{
		return HBPJBBJFHME;
	}

	public long NBDICCLKEAC()
	{
		return GKLJJHLFACI;
	}

	public void BNGLIONOOAG(long value)
	{
		GKLJJHLFACI = value;
		EMDLLIGKONG("LastDumpTime", GKLJJHLFACI);
	}

	public long CPGGBLDAHBG()
	{
		return GKIONOFPEGB;
	}

	public void DEPJCHIFFKA(long value)
	{
		GKIONOFPEGB = value;
		EMDLLIGKONG("PeriodicPlayTime", GKIONOFPEGB);
	}

	public long ECGBNDKJIPD()
	{
		return BHOEHPFBIJE;
	}

	public void DFBDNDMOLFH(long value)
	{
		BHOEHPFBIJE = value;
		EMDLLIGKONG("LastDailyTimeOffset", BHOEHPFBIJE);
	}

	public long AHAAGOHCGHN()
	{
		return JOLENHJGDLH;
	}

	public void ODPOOEPKJBP(long value)
	{
		JOLENHJGDLH = value;
		EMDLLIGKONG("LastEnergyTimeOffset", JOLENHJGDLH);
	}

	public bool HFINDOBJHNK()
	{
		return AJGEKAADGEJ;
	}

	public void MJLDOEOMLEG(bool value)
	{
		AJGEKAADGEJ = value;
		EMDLLIGKONG("ShowUpgrades", AJGEKAADGEJ);
	}

	public bool ENBKLLMAALP()
	{
		return NPEENBBIFFB;
	}

	public void PLBEEGGFKDH(bool value)
	{
		NPEENBBIFFB = value;
		EMDLLIGKONG("ShowForge", NPEENBBIFFB);
	}

	public int BGBFBIDOECK()
	{
		return CHDMBKEHHLH;
	}

	public int NPGECMDDNFO()
	{
		return CFNNEGHPCMN;
	}

	public void KGEHCFADNLI(int value)
	{
		CFNNEGHPCMN = value;
		EMDLLIGKONG("DenominationDigits", CFNNEGHPCMN);
	}

	public FightIDS HEBDMAIEAPM()
	{
		return GDFIGNHHKDC;
	}

	public void MICFFKODJME(FightIDS value)
	{
		GDFIGNHHKDC = value;
		EMDLLIGKONG("FightIDS", GDFIGNHHKDC.ToString());
	}

	public void IGBNKIKIDII(string JFIIJBAOOIK)
	{
		if (GDFIGNHHKDC != null)
		{
			GDFIGNHHKDC.SetFightIDSByString(JFIIJBAOOIK);
		}
		else
		{
			GDFIGNHHKDC = new FightIDS(JFIIJBAOOIK);
		}
	}

	public Color EPEDEDLCAJF()
	{
		return _MapMaskColor;
	}

	public void set_MapMaskColor(Color value)
	{
		_MapMaskColor = value;
	}

	public ModelParameters get_Parameters()
	{
		return HEGIABHIPHA;
	}

	public string GKLECBABFCP()
	{
		return BGMCCMHOMJL;
	}

	public void COKACMKOIGD(string value)
	{
		BGMCCMHOMJL = value;
		EMDLLIGKONG("Language", BGMCCMHOMJL);
	}

	public string OGJBDMNBMLJ()
	{
		return NBBNANIILBL;
	}

	public void HEIPPEGBOCK(string value)
	{
		NBBNANIILBL = value;
		EMDLLIGKONG("CoinIcon", NBBNANIILBL);
	}

	public FightIDS KNJNHKDCINB()
	{
		return FHJHPGDPNBH;
	}

	public void KPDHMBIDAHA(FightIDS value)
	{
		NDFLHPGHKMP(value.ToString());
	}

	public void NDFLHPGHKMP(string JFIIJBAOOIK)
	{
		if (!FHJHPGDPNBH.OLAJNGPILGL(JFIIJBAOOIK))
		{
			FHJHPGDPNBH.SetFightIDSByString(JFIIJBAOOIK);
			if (HPAHCBILEOE != null)
			{
				HPAHCBILEOE.Value = FHJHPGDPNBH.OOBHBGJIBGP();
			}
			else
			{
				EMDLLIGKONG("MapFocus", FHJHPGDPNBH.OOBHBGJIBGP());
			}
		}
	}

	public FightIDS MGICKOOCNAJ()
	{
		if (LINHCAGANFC == null)
		{
			LINHCAGANFC = new FightIDS("ZONE_RAID|BOSS_1");
		}
		else if (LINHCAGANFC.OOPMAAHJMCE())
		{
			LINHCAGANFC.SetFightIDSByString("ZONE_RAID|BOSS_1");
		}
		return LINHCAGANFC;
	}

	public void AEGEEMMBLDF(FightIDS value)
	{
		EOPPBJKPKGD(value.ToString());
	}

	public void EOPPBJKPKGD(string JFIIJBAOOIK)
	{
		if (!LINHCAGANFC.OLAJNGPILGL(JFIIJBAOOIK))
		{
			LINHCAGANFC.SetFightIDSByString(JFIIJBAOOIK);
			if (LINHCAGANFC.OOPMAAHJMCE())
			{
				LINHCAGANFC.SetFightIDSByString("ZONE_RAID|BOSS_1");
			}
			if (AALOLMPMCDH != null)
			{
				AALOLMPMCDH.Value = LINHCAGANFC.OOBHBGJIBGP();
			}
			else
			{
				EMDLLIGKONG("RaidMapFocus", LINHCAGANFC.OOBHBGJIBGP());
			}
		}
	}

	public bool MLOHMAGMIAI()
	{
		return OKLFFBPGIAN;
	}

	public void EKHEBJIOMFH(bool value)
	{
		OKLFFBPGIAN = value;
		EMDLLIGKONG("FacebookLiked", OKLFFBPGIAN);
	}

	public long AACMNAJJKME()
	{
		return DLKPHLOAFGM;
	}

	public void EPGEKHNCEJF(long value)
	{
		DLKPHLOAFGM = value;
		EMDLLIGKONG("StarterPackTimerEndTime", DLKPHLOAFGM);
	}

	public RosterTimerContainer AEMFLPNDDKL()
	{
		return GNOMNIKGAPE;
	}

	public bool JPMPIDFGCJL()
	{
		return JHJLHNHCPMP;
	}

	public void SetEclipseMode(bool value)
	{
		JHJLHNHCPMP = value;
		EMDLLIGKONG("EclipseMode", value ? "On" : "Off");
	}

	public int PINDEKDNCNL()
	{
		if (!CLODDOOGDBB)
		{
			return (HEGIABHIPHA != null) ? (int)(HEGIABHIPHA.PINDEKDNCNL()) : 0;
		}
		return MMIMAJCKFKL;
	}

	public void DLDMOHEGENM(int value)
	{
		if (HEGIABHIPHA != null)
		{
			HEGIABHIPHA.DLDMOHEGENM((ObscuredInt)(value));
			EMDLLIGKONG("Level", (ObscuredInt)(HEGIABHIPHA.PINDEKDNCNL()));
		}
	}

	public int GetCurrencyCount(GameCurrency MDDNHLBDJBN)
	{
		CurrencyStruct lAPFHLGNAAF = MEOLAFECPHI(MDDNHLBDJBN);
		if (lAPFHLGNAAF != null)
		{
			return (ObscuredInt)(lAPFHLGNAAF.Count);
		}
		return 0;
	}

	public void MHGIEFLBBGM()
	{
		int num = ((CHDMBKEHHLH == 0) ? 1 : 0);
		SessionSettings("Disciple", num);
		CHDMBKEHHLH = num;
	}

	public int NPKBPGMNDFJ()
	{
		int num = 0;
		List<Trick> list = GameUtils.KLLGJKHALGH();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].IsNew)
			{
				num++;
			}
		}
		return num;
	}

	public int CNFOLIEFJCE()
	{
		int num = 0;
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			return 0;
		}
		List<UserItem> list = nKGLHEGIKKP.KHCNHPCPFII().HOPBBLJLHOB("Seal", string.Empty);
		foreach (UserItem item in list)
		{
			if (item.OFOPFCJNEBL() > 0 && item.BHKHOJPANHE().DBHJGAGOLOB())
			{
				num++;
			}
		}
		return num;
	}

	public bool FLFKOIPCEPI(string name)
	{
		return JNOGGCIPGCI(name) >= 0;
	}

	public bool HAMPNCKAJKD(FightIDS DIAIIPCBMFL)
	{
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (item.KHGCEFNBDDG().Equals(DIAIIPCBMFL.ToString()))
			{
				return true;
			}
		}
		return false;
	}

	public string GetSettingsXML(string name)
	{
		if (_node == null || _node["SessionSettings"] == null || _node["SessionSettings"][name] == null)
		{
			return string.Empty;
		}
		return _node["SessionSettings"][name].Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public RosterQuest OOMJEHAKOBA(string name)
	{
		foreach (RosterQuest item in CNFCPCJPGLM)
		{
			if (name == item.Name)
			{
				return item;
			}
		}
		return null;
	}

	public bool FJGCOOAACLD(string name)
	{
		XmlNode xmlNode = _node["SessionSettings"];
		if (xmlNode == null)
		{
			return false;
		}
		XmlNode xmlNode2 = xmlNode[name];
		if (xmlNode2 == null)
		{
			return false;
		}
		return true;
	}

	public bool FNLMHKJGCMC(string name)
	{
		return false;
	}

	public void APDCCIEJLMD()
	{
		XmlNode sounds = ((_node["Sounds"] != null) ? _node["Sounds"] : _node.ACBPMPMPKJJ("Sounds"));
		XmlNode music = ((sounds["Music"] != null) ? sounds["Music"] : sounds.ACBPMPMPKJJ("Music"));
		music.LLIKNHNLGJJ("Value").Value = Sound.EAIGFAPKILL().ToString(System.Globalization.CultureInfo.InvariantCulture);
		music.LLIKNHNLGJJ("Mute").Value = ((!Sound.ELHMADOKHHE()) ? "0" : "1");
		GGGEHAGCLGC();
	}

	public void ABODKHDPHMI()
	{
		XmlNode sounds = ((_node["Sounds"] != null) ? _node["Sounds"] : _node.ACBPMPMPKJJ("Sounds"));
		XmlNode sound = ((sounds["Sound"] != null) ? sounds["Sound"] : sounds.ACBPMPMPKJJ("Sound"));
		sound.LLIKNHNLGJJ("Value").Value = Sound.NBHPABEBLOP().ToString(System.Globalization.CultureInfo.InvariantCulture);
		sound.LLIKNHNLGJJ("Mute").Value = ((!Sound.AAFLCDKJEPL()) ? "0" : "1");
		GGGEHAGCLGC();
	}

	public static string HEODNLPIJHL(HPOIJPGPOCF LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case HPOIJPGPOCF.CHANGE_INIT:
			return "init";
		case HPOIJPGPOCF.CHANGE_PAYMENT:
			return "payment";
		case HPOIJPGPOCF.CHANGE_ACHIEVEMENT:
			return "achievement";
		case HPOIJPGPOCF.CHANGE_FIGHT_REWARD:
			return "fight_reward";
		case HPOIJPGPOCF.CHANGE_VIDEO_VIEW:
			return "video_view";
		case HPOIJPGPOCF.CHANGE_FACEBOOK:
			return "facebook";
		case HPOIJPGPOCF.CHANGE_QUEST:
			return "quest";
		case HPOIJPGPOCF.CHANGE_CHEAT:
			return "cheat";
		case HPOIJPGPOCF.CHANGE_SERVER_GIVE:
			return "server";
		case HPOIJPGPOCF.CHANGE_OFFER:
			return "offer";
		case HPOIJPGPOCF.CHANGE_BUY_ITEM:
			return "buy_item";
		case HPOIJPGPOCF.CHANGE_BUY_DELIVERY:
			return "buy_delivery";
		case HPOIJPGPOCF.CHANGE_RAIDS_REWARD:
			return "raids_reward";
		case HPOIJPGPOCF.CHANGE_LEDGER:
			return "ledger";
		default:
			LLLOJBFMONN.Error("unknown BalanceChangeType");
			return "unknown";
		}
	}

	public List<RosterFight> FHDLNKAAAOK(int GNLOCMLBNHF)
	{
		List<RosterFight> list = new List<RosterFight>();
		foreach (RosterFight item in JNPMCNMEOLE)
		{
			if (item.PINDEKDNCNL() == GNLOCMLBNHF)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public RosterFight DBMHOBPNIIA(FightIDS JFIIJBAOOIK)
	{
		foreach (RosterFight item in JNPMCNMEOLE)
		{
			if (JFIIJBAOOIK.Equals(item.EKOIBAIIKHL()))
			{
				return item;
			}
		}
		return null;
	}

	public RosterFight JJHCGOIKBCP(FightIDS DIAIIPCBMFL)
	{
		RosterFight pIGKOIFBOME = DBMHOBPNIIA(DIAIIPCBMFL);
		pIGKOIFBOME.GICDABHEMML();
		if (JPMPIDFGCJL())
		{
			pIGKOIFBOME.LOEBHEODPAH();
		}
		return pIGKOIFBOME;
	}

	public RosterFight NALCLBDLBKN(FightIDS DIAIIPCBMFL)
	{
		RosterFight pIGKOIFBOME = DBMHOBPNIIA(DIAIIPCBMFL);
		if (pIGKOIFBOME != null)
		{
			pIGKOIFBOME.ICAKCDMOMDF();
			if (JPMPIDFGCJL())
			{
				pIGKOIFBOME.HBIAOHGMLDK();
			}
			return pIGKOIFBOME;
		}
		return null;
	}

	public RosterFight OBAFPDGJHNN(FightIDS DIAIIPCBMFL)
	{
		string text = "Fights";
		string jLEKBBJBLOE = "Fight";
		XmlNode mEEAKLDGLDF = ((_node[text] == null) ? _node.ACBPMPMPKJJ(text) : _node[text]);
		XmlNode hKPPBKPJOEO = mEEAKLDGLDF.ACBPMPMPKJJ(jLEKBBJBLOE);
		RosterFight pIGKOIFBOME = new RosterFight(hKPPBKPJOEO);
		pIGKOIFBOME.set_FightIDS(DIAIIPCBMFL.ToString());
		JNPMCNMEOLE.Add(pIGKOIFBOME);
		return pIGKOIFBOME;
	}

	public RosterBattle MKGLPNLDDKF(FightIDS DIAIIPCBMFL)
	{
		string text = "Battles";
		string jLEKBBJBLOE = "Battle";
		XmlNode mEEAKLDGLDF = (_node[text].IsEmpty ? _node.ACBPMPMPKJJ(text) : _node[text]);
		string fOOKNBHPOOA = DIAIIPCBMFL.PELHCAEAOFE() + "|" + DIAIIPCBMFL.CPHDPCAECJN() + "|";
		XmlNode xmlNode = mEEAKLDGLDF.LJGLMGNAFHJ("Battle", "Name", fOOKNBHPOOA);
		if (xmlNode == null)
		{
			xmlNode = mEEAKLDGLDF.ACBPMPMPKJJ(jLEKBBJBLOE);
		}
		RosterBattle dDNLCGOPAGC = new RosterBattle(xmlNode);
		dDNLCGOPAGC.GEGKFFGACDI(DIAIIPCBMFL);
		HLHEFIKFBHH.Add(dDNLCGOPAGC);
		return dDNLCGOPAGC;
	}

	public void MEJJNKMPMFE(FightIDS DIAIIPCBMFL)
	{
		string text = "Battles";
		string jLEKBBJBLOE = "Battle";
		XmlNode mEEAKLDGLDF = (_node[text].IsEmpty ? _node.ACBPMPMPKJJ(text) : _node[text]);
		string text2 = DIAIIPCBMFL.PELHCAEAOFE() + "|" + DIAIIPCBMFL.CPHDPCAECJN() + "|";
		XmlNode xmlNode = mEEAKLDGLDF.LJGLMGNAFHJ("Battle", "Name", text2);
		if (xmlNode == null)
		{
			XmlNode mEEAKLDGLDF2 = mEEAKLDGLDF.ACBPMPMPKJJ(jLEKBBJBLOE);
			mEEAKLDGLDF2.LLIKNHNLGJJ("Name").Value = text2;
		}
	}

	public RosterFight AJKBFMLOCOF(RosterFight value, bool EFCPLDABOIF = false)
	{
		if (EFCPLDABOIF)
		{
			bool flag = false;
			foreach (RosterFight item in JNPMCNMEOLE)
			{
				if (item.EKOIBAIIKHL() == value.EKOIBAIIKHL())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				JNPMCNMEOLE.Add(value);
			}
		}
		else
		{
			JNPMCNMEOLE.Add(value);
		}
		return value;
	}

	public void KJIMPNEGNAN(FightIDS DIAIIPCBMFL, bool EFCPLDABOIF = false, bool PEJELKNFEKJ = true, bool NIBIMBDBPMI = false, bool MDEHPLPLNNF = false, int OAHPBDFKJOK = 0)
	{
		if (DIAIIPCBMFL == null)
		{
			LLLOJBFMONN.Error("Roster::addBattle ERROR - ids is NULL");
			return;
		}
		if (!PEJELKNFEKJ)
		{
			HEHJKDPAPLA(DIAIIPCBMFL);
			return;
		}
		MEJJNKMPMFE(DIAIIPCBMFL);
		ListSF.ELEBLBJKDBI().OnAuthenticate();
		if (EFCPLDABOIF)
		{
			foreach (RosterBattle item in HLHEFIKFBHH)
			{
				if (item.KHGCEFNBDDG().Equals(DIAIIPCBMFL))
				{
					item.GEGKFFGACDI(DIAIIPCBMFL);
					item.HLNEICNJDCF(NIBIMBDBPMI);
					item.HCEOCBOFIGC(MDEHPLPLNNF);
					item.FHCHCHPPMEI(OAHPBDFKJOK);
					return;
				}
			}
		}
		RosterBattle dDNLCGOPAGC = MKGLPNLDDKF(DIAIIPCBMFL);
		dDNLCGOPAGC.HLNEICNJDCF(NIBIMBDBPMI);
		dDNLCGOPAGC.HCEOCBOFIGC(MDEHPLPLNNF);
		dDNLCGOPAGC.FHCHCHPPMEI(OAHPBDFKJOK);
		Battle cGJCGEBPCAF = ListSF.MKHAAGMJOPG(DIAIIPCBMFL);
		if (cGJCGEBPCAF != null)
		{
			dDNLCGOPAGC.EDHMHFONDAI = cGJCGEBPCAF;
			cGJCGEBPCAF.FOMHAGJJCLJ(dDNLCGOPAGC);
			cGJCGEBPCAF.BDAELBFECAJ();
		}
	}

	public void KJIMPNEGNAN(RosterBattle ELBLEPOEKIL)
	{
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (item.KHGCEFNBDDG().Equals(ELBLEPOEKIL.KHGCEFNBDDG()))
			{
				LLLOJBFMONN.Error("Battle already exists: " + item.KHGCEFNBDDG().CPHDPCAECJN());
				return;
			}
		}
		HLHEFIKFBHH.Add(ELBLEPOEKIL);
	}

	public void KJIMPNEGNAN(Battle DPOOIONCEOA, bool EFCPLDABOIF = false, bool PEJELKNFEKJ = true, bool NIBIMBDBPMI = false, bool MDEHPLPLNNF = false, int OAHPBDFKJOK = 0)
	{
		FightIDS dIAIIPCBMFL = new FightIDS(DPOOIONCEOA.LKDFFCADHNO().get_Name(), DPOOIONCEOA.get_Name(), string.Empty);
		KJIMPNEGNAN(dIAIIPCBMFL, EFCPLDABOIF, PEJELKNFEKJ, NIBIMBDBPMI, MDEHPLPLNNF, OAHPBDFKJOK);
	}

	public void HEHJKDPAPLA(RosterBattle ELBLEPOEKIL)
	{
		string text = ELBLEPOEKIL.KHGCEFNBDDG().ToString();
		XmlNode xmlNode = _node["Battles"];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			string text2 = childNode.Attributes["Name"].CIPOICEEIBK(string.Empty);
			if (text2 == text)
			{
				xmlNode.RemoveChild(childNode);
				break;
			}
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (ELBLEPOEKIL == item)
			{
				HLHEFIKFBHH.Remove(item);
				break;
			}
		}
		Battle eDHMHFONDAI = ELBLEPOEKIL.EDHMHFONDAI;
		eDHMHFONDAI.JNPDHAFMKID();
	}

	public void HEHJKDPAPLA(FightIDS DIAIIPCBMFL)
	{
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (item.KHGCEFNBDDG().Equals(DIAIIPCBMFL))
			{
				HEHJKDPAPLA(item);
				break;
			}
		}
	}

	public List<RosterBattle> CIOHLDJJNAO(string PPBIPCKMFKB)
	{
		List<RosterBattle> list = new List<RosterBattle>();
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (item.KHGCEFNBDDG().PELHCAEAOFE() == PPBIPCKMFKB)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public RosterQuest FLMIDLIKKOG(string GACKIHNGHLE, string PMFEIPCHENB)
	{
		XmlNode xmlNode = ((_node["Quests"] == null) ? _node.ACBPMPMPKJJ("Quests") : _node["Quests"]);
		XmlNode mEEAKLDGLDF = ((xmlNode["Quests"] == null) ? xmlNode.ACBPMPMPKJJ("Quests") : xmlNode["Quests"]);
		XmlNode xmlNode2 = mEEAKLDGLDF.ACBPMPMPKJJ("Quest");
		xmlNode2.LLIKNHNLGJJ("Name").Value = GACKIHNGHLE;
		string value = DirectoryController.BAANOCLBLKM(PMFEIPCHENB);
		xmlNode2.LLIKNHNLGJJ("FileName").Value = value;
		return FDJJIIBHAOG(xmlNode2);
	}

	public RosterQuest FDJJIIBHAOG(XmlNode FNMHECBANNJ)
	{
		RosterQuest dKBDLDGOFDN = new RosterQuest(FNMHECBANNJ);
		CNFCPCJPGLM.Add(dKBDLDGOFDN);
		return dKBDLDGOFDN;
	}

	public void OCDOPMPENHP(XmlNode node)
	{
		string text = "_" + node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		RosterQuest.NOKCOAHJIPB nOKCOAHJIPB = PFMIBOCGGPC(text);
		if (nOKCOAHJIPB != null)
		{
			nOKCOAHJIPB.MCPIOGALBMK(node.Attributes["Value"].CIPOICEEIBK(string.Empty));
			return;
		}
		nOKCOAHJIPB = new RosterQuest.NOKCOAHJIPB(node);
		INLEOAPIEDJ[text] = nOKCOAHJIPB;
	}

	public RosterQuest.NOKCOAHJIPB PFMIBOCGGPC(string name)
	{
		if (INLEOAPIEDJ.ContainsKey(name))
		{
			return INLEOAPIEDJ[name];
		}
		return null;
	}

	public void SetQuestVariable(string name, string value)
	{
		string gOHIIMFFFJI = "_" + name;
		RosterQuest.NOKCOAHJIPB nOKCOAHJIPB = PFMIBOCGGPC(gOHIIMFFFJI);
		if (nOKCOAHJIPB != null)
		{
			nOKCOAHJIPB.MCPIOGALBMK(value);
			return;
		}
		XmlNode xmlNode = ((_node["Quests"] == null) ? _node.ACBPMPMPKJJ("Quests") : _node["Quests"]);
		XmlNode mEEAKLDGLDF = ((xmlNode["Variables"] == null) ? xmlNode.ACBPMPMPKJJ("Variables") : xmlNode["Variables"]);
		XmlNode xmlNode2 = mEEAKLDGLDF.ACBPMPMPKJJ("Variable");
		xmlNode2.LLIKNHNLGJJ("Name").Value = name;
		xmlNode2.LLIKNHNLGJJ("Value").Value = value;
		nOKCOAHJIPB = new RosterQuest.NOKCOAHJIPB(xmlNode2);
		gOHIIMFFFJI = "_" + nOKCOAHJIPB.Name;
		INLEOAPIEDJ[gOHIIMFFFJI] = nOKCOAHJIPB;
	}

	public void SessionSettings(string name, string value)
	{
		XmlNode xmlNode = _node["SessionSettings"];
		if (xmlNode == null)
		{
			xmlNode = _node.ACBPMPMPKJJ("SessionSettings");
		}
		XmlNode xmlNode2 = xmlNode[name];
		if (xmlNode2 == null)
		{
			xmlNode2 = xmlNode.ACBPMPMPKJJ(name);
		}
		XmlAttribute xmlAttribute = xmlNode2.Attributes["Value"];
		if (xmlAttribute == null)
		{
			xmlAttribute = xmlNode2.LLIKNHNLGJJ("Value");
		}
		xmlAttribute.Value = value;
	}

	public void SessionSettings(string name, int value)
	{
		SessionSettings(name, value.ToString());
	}

	public bool FEDNFNOMBNG(ItemInfo item)
	{
		UserItem dKCHDHMLKHN = KHCNHPCPFII().CMGOCLGHNLH(item);
		return dKCHDHMLKHN != null && dKCHDHMLKHN.EFMFGEPDAOP();
	}

	public long PEJFMMHOOGN()
	{
		return EHFJHFDACMP() - (ObscuredLong)(FJGHKGPAPPN());
	}

	public bool DFNEGEEHLFJ()
	{
		return NDIEOONABLA;
	}

	public void OEAOPFDLMJJ(bool value)
	{
		NDIEOONABLA = value;
		EMDLLIGKONG("TrySocialLogin", NDIEOONABLA);
	}

	public bool ChangePower(int value)
	{
		if (value == 0)
		{
			return true;
		}
		int num = Mathf.Min((ObscuredInt)(_power) + value, OGLHGFJKMCO);
		if (num < 0)
		{
			return false;
		}
		DKAAELKJJOP(num);
		return true;
	}

	public void DKAAELKJJOP(int value)
	{
		int num = Mathf.Min(value, OGLHGFJKMCO);
		int num2 = (ObscuredInt)(_power);
		if (num != num2)
		{
			_power = (ObscuredInt)(num);
			num2 = (ObscuredInt)(_power);
			EMDLLIGKONG("Power", num2);
			if (num2 == OGLHGFJKMCO && OGLHGFJKMCO != 0)
			{
				PABEFKBJNEF(-1L);
				DHKODKHPGGN(-1L);
			}
		}
	}

	public void ALJEKDDKPJJ(long LBIGLJLMIDG)
	{
		int num = (ObscuredInt)(_power);
		if (num == OGLHGFJKMCO)
		{
			return;
		}
		if (FNDAKFILBOE() == -1)
		{
			PABEFKBJNEF(LBIGLJLMIDG);
			DHKODKHPGGN(LBIGLJLMIDG);
			return;
		}
		long num2 = LBIGLJLMIDG - FNDAKFILBOE();
		int num3 = (int)(num2 / DIJOCFEFHAK);
		if (FNDAKFILBOE() > LBIGLJLMIDG)
		{
			PABEFKBJNEF(LBIGLJLMIDG);
		}
		if (num3 > 0)
		{
			long bAINMLLIKOL = LBIGLJLMIDG - num2 % DIJOCFEFHAK;
			PABEFKBJNEF(bAINMLLIKOL);
			DKAAELKJJOP(num + num3);
		}
		DHKODKHPGGN(LBIGLJLMIDG);
	}

	public bool BMADIJMPENJ(UserItem item, bool JBCMFEPAKLK = true)
	{
		if (!JBCMFEPAKLK)
		{
			GGGEHAGCLGC();
			return true;
		}
		if (HEGIABHIPHA != null)
		{
			ItemInfo dJKEECEOCJB = item.BHKHOJPANHE();
			if (dJKEECEOCJB.ILDOPPMOOOF(item.DHNNCAEEMLL()) != null)
			{
				dJKEECEOCJB = dJKEECEOCJB.ILDOPPMOOOF(item.DHNNCAEEMLL());
			}
			HEGIABHIPHA.OLLNIKFPMKE(dJKEECEOCJB.Type, dJKEECEOCJB);
			EMDLLIGKONG(dJKEECEOCJB.Type, (!JBCMFEPAKLK) ? string.Empty : dJKEECEOCJB.Name);
			HEGIABHIPHA.PPFDLIBLNDG();
			return true;
		}
		return false;
	}

	public void JALMHIICOPB(ItemInfo PJDAGCBPLJE)
	{
		if (HEGIABHIPHA != null)
		{
			HEGIABHIPHA.OLLNIKFPMKE(PJDAGCBPLJE.Type, null);
			EMDLLIGKONG(PJDAGCBPLJE.Type, string.Empty);
			HEGIABHIPHA.PPFDLIBLNDG();
		}
	}

	public void OMJDCEEEJMB()
	{
		DKAAELKJJOP(OGLHGFJKMCO);
	}

	public bool PBOFBNFALNN()
	{
		RosterQuest dKBDLDGOFDN = null;
		int num = 0;
		List<QuestStage> list = new List<QuestStage>();
		foreach (RosterQuest item in CNFCPCJPGLM)
		{
			if (item.get_Parameters() == null)
			{
				continue;
			}
			if (!JHHBKBENNNA(item.FileName))
			{
				ListSF.ELEBLBJKDBI().PDCHBPKOBFI(item.FileName);
			}
			QuestStage mLLKDGBEGJI = ListSF.ELEBLBJKDBI().PBGCEEBDBGG(item.Name);
			if (mLLKDGBEGJI != null)
			{
				if (mLLKDGBEGJI.IDGAAJAFCHC())
				{
					mLLKDGBEGJI.AGJGEBBLFGA();
					continue;
				}
				if (num == 0)
				{
					dKBDLDGOFDN = item;
				}
				foreach (QuestStage item2 in list)
				{
					if (item2 == mLLKDGBEGJI)
					{
						LLLOJBFMONN.Error("same name of quest %s", item.Name);
					}
				}
				list.Add(mLLKDGBEGJI);
				num++;
			}
			else
			{
				item.LCIHKPPGNPF();
				item.PLNNKKBPDJK = true;
			}
		}
		if (list.Count > 0)
		{
			ListSF.ELEBLBJKDBI().FGAEEJBEGEJ(list);
			if (dKBDLDGOFDN != null)
			{
				ScreenType iPKNDMINFMJ = (ScreenType)dKBDLDGOFDN.ELBKKOPHLHK();
				ScreenType iPKNDMINFMJ2 = Module.ELEBLBJKDBI().NMCNDOPKFJD();
				if (iPKNDMINFMJ == ScreenType.ModuleFight && iPKNDMINFMJ != iPKNDMINFMJ2)
				{
					Module.DLOKJOHNDID(ScreenType.ModuleDojo);
				}
				else if (iPKNDMINFMJ != (ScreenType)(-1) && iPKNDMINFMJ != iPKNDMINFMJ2)
				{
					Module.DLOKJOHNDID(iPKNDMINFMJ, 0);
				}
				return true;
			}
		}
		return false;
	}

	public bool FDPIBNJJDAK()
	{
		return CFIMMOBMEIP;
	}

	public void AJCCEFKDKIO(bool value)
	{
		CFIMMOBMEIP = value;
		EMDLLIGKONG("AskedForDumps", CFIMMOBMEIP);
	}

	public void BIHELGAGPGO()
	{
		long num = ListSF.IDMJOMOMDOJ();
		if (num - NBDICCLKEAC() >= 900)
		{
			BNGLIONOOAG(num);
		}
	}

	public void PNHPFNGCFGO()
	{
		if (_node == null)
		{
			return;
		}
		List<ItemInfo> list = ListSF.DJBOFEEKJMP().ONFMAJEAACM("RealMoneyItem");
		XmlNode xmlNode = _node["Billing"];
		if (xmlNode == null)
		{
			xmlNode = _node.KDPLHGGPJHN("Billing");
		}
		foreach (ItemInfo item in list)
		{
			XmlNode xmlNode2 = xmlNode.LJGLMGNAFHJ("Item", "Name", item.Name);
			if (xmlNode2 == null)
			{
				xmlNode2 = xmlNode.KDPLHGGPJHN("Item");
			}
			XmlAttribute xmlAttribute = xmlNode2.LLIKNHNLGJJ("Name");
			xmlAttribute.Value = item.Name;
			xmlAttribute = xmlNode2.LLIKNHNLGJJ("RealPrice");
			xmlAttribute.Value = item.FPEIFLEBEAA;
			xmlAttribute = xmlNode2.LLIKNHNLGJJ("RealPriceConst");
			xmlAttribute.Value = item.EGAJMELKANL;
			xmlAttribute = xmlNode2.LLIKNHNLGJJ("RealPriceCurrency");
			xmlAttribute.Value = item.MIIJIMJDHFP;
		}
		ListSF.ELEBLBJKDBI().EJANJEEGOOE();
	}

	public bool AddShopLock(string name, bool FLOAHAOBNAP = false)
	{
		if (!FLFKOIPCEPI(name))
		{
			if (FLOAHAOBNAP)
			{
				string text = "Shop";
				string iMGCANJHPND = "Lock";
				XmlNode xmlNode = _node[text];
				if (xmlNode == null)
				{
					xmlNode = _node.KDPLHGGPJHN(text);
				}
				XmlNode mEEAKLDGLDF = xmlNode.KDPLHGGPJHN(iMGCANJHPND);
				mEEAKLDGLDF.LLIKNHNLGJJ("Name").Value = name;
			}
			BMKAMGGIELK.Add(name);
			return true;
		}
		return false;
	}

	public bool OAHDKIDMOCG(string name)
	{
		int num = JNOGGCIPGCI(name);
		if (num >= 0)
		{
			XmlNode xmlNode = _node["Shop"];
			XmlNode xmlNode2 = xmlNode.LJGLMGNAFHJ("Lock", "Name", name);
			if (xmlNode2 != null)
			{
				xmlNode.RemoveChild(xmlNode2);
			}
			BMKAMGGIELK.RemoveAt(num);
			return true;
		}
		return false;
	}

	public int JNOGGCIPGCI(string name)
	{
		if (name == null)
		{
			return -1;
		}
		int num = 0;
		foreach (string item in BMKAMGGIELK)
		{
			if (item.Equals(name))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public void ENFKEIHBICK(string name, bool NLCCJEHMAOF = true)
	{
		if (COJMHNDCACJ.Contains(name))
		{
			return;
		}
		COJMHNDCACJ.Add(name);
		List<Trick> list = AnimationData.BFNFDDLNHPA();
		for (int i = 0; i < list.Count; i++)
		{
			Trick iHNIKIHKFHC = list[i];
			if (iHNIKIHKFHC.Name == name)
			{
				iHNIKIHKFHC.IsNew = true;
				break;
			}
		}
		if (NLCCJEHMAOF)
		{
			string text = "OpenTricks";
			string jLEKBBJBLOE = "Trick";
			XmlNode mEEAKLDGLDF = ((_node[text] == null) ? _node.ACBPMPMPKJJ(text) : _node[text]);
			XmlNode mEEAKLDGLDF2 = mEEAKLDGLDF.ACBPMPMPKJJ(jLEKBBJBLOE);
			mEEAKLDGLDF2.LLIKNHNLGJJ("Name").Value = name;
			GGGEHAGCLGC();
		}
	}

	public void DECNJIOFODA(string name, bool NLCCJEHMAOF = true)
	{
		string nameKey = "OpenTricks";
		if (_node[nameKey] == null)
		{
			return;
		}
		XmlNode xmlNode = _node[nameKey];
		foreach (XmlNode childNode in xmlNode.ChildNodes)
		{
			if (childNode.Attributes["Name"].CIPOICEEIBK(string.Empty) == nameKey)
			{
				xmlNode.RemoveChild(childNode);
				break;
			}
		}
		if (COJMHNDCACJ.Contains(name))
		{
			COJMHNDCACJ.Remove(name);
		}
		if (NLCCJEHMAOF)
		{
			GGGEHAGCLGC();
		}
	}

	private void EJLOBJIFEAL()
	{
		XmlNode xmlNode = _node["CounterItems"];
		if (xmlNode == null)
		{
			return;
		}
		XmlNode xmlNode2 = xmlNode["Items"];
		if (xmlNode2 == null)
		{
			return;
		}
		foreach (XmlNode childNode in xmlNode2.ChildNodes)
		{
			ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(childNode.Attributes["Name"].CIPOICEEIBK());
			if (dJKEECEOCJB != null && dJKEECEOCJB.GKODCKNAAHB == 0)
			{
				dJKEECEOCJB.BEBDMOEIEJN(true);
			}
		}
	}

	public void KGFJPLKOABI()
	{
		List<string> PIDDFMECFAO = new List<string>();
		List<ItemInfo> list = ListSF.DJBOFEEKJMP().HCDLKHKBEPF();
		list.ForEach((ItemInfo DHDMNHCIPEH) =>
		{
			if (DHDMNHCIPEH.DBHJGAGOLOB())
			{
				PIDDFMECFAO.Add(DHDMNHCIPEH.Name);
			}
		});
		string text = "CounterItems";
		XmlNode xmlNode = _node[text];
		if (xmlNode != null)
		{
			_node.RemoveChild(xmlNode);
			xmlNode = null;
		}
		if (PIDDFMECFAO.Count <= 0)
		{
			return;
		}
		xmlNode = _node.ACBPMPMPKJJ(text);
		XmlNode mEEAKLDGLDF = xmlNode.ACBPMPMPKJJ("Items");
		foreach (string item in PIDDFMECFAO)
		{
			mEEAKLDGLDF.ACBPMPMPKJJ("Item").LLIKNHNLGJJ("Name").Value = item;
		}
	}

	public void PMIIHIFGIIN()
	{
		EHAAMEPDOGJ.INECBIPEJNL(KHKMHPDKDIJ);
	}

	public void DDBBADOGHHB(string EIDDAFDJJCJ)
	{
		if (!JHHBKBENNNA(EIDDAFDJJCJ))
		{
			EOJCMGDPHLL.Add(EIDDAFDJJCJ);
		}
	}

	public bool JHHBKBENNNA(string EIDDAFDJJCJ)
	{
		foreach (string item in EOJCMGDPHLL)
		{
			if (item == EIDDAFDJJCJ)
			{
				return true;
			}
		}
		return false;
	}

	public bool DHPNBBILDPB()
	{
		return GNCENEKPINA && GAKAEGHGNGD < 1;
	}

	public void BCMKHEKOMDB(bool value)
	{
		GNCENEKPINA = value;
		EMDLLIGKONG("GPlusAutoLogin", GNCENEKPINA);
	}

	public void INNHOAPGCHI()
	{
		GAKAEGHGNGD++;
		KLCCBHMJKFI(GAKAEGHGNGD);
	}

	public void MFJLCDAEFFD()
	{
		GAKAEGHGNGD = 0;
		KLCCBHMJKFI(GAKAEGHGNGD);
	}

	public void AOBEHOILNOG(string name, bool FLOAHAOBNAP)
	{
	}

	public void FHCPEIGMGMK(int NPFOBKBJAOB = 0)
	{
		double num = Math.Pow(10.0, CFNNEGHPCMN - NPFOBKBJAOB);
		long num2 = 0L;
		double num3 = (double)(ObscuredLong)(MJBFFBPLAGC) / num;
		if (num > 1.0)
		{
			num2 = (ObscuredLong)(MJBFFBPLAGC) % (long)num;
			if (0 < num2)
			{
				num3++;
			}
		}
		MJBFFBPLAGC = (ObscuredLong)((long)num3);
		num2 = 0L;
		double num4 = (double)(ObscuredLong)(BIOMOEDPLIP) / num;
		if (num > 1.0)
		{
			num2 = (ObscuredLong)(BIOMOEDPLIP) % (long)num;
		}
		if (0 < num2)
		{
			num4++;
		}
		BIOMOEDPLIP = (ObscuredLong)((long)num4);
	}

	public void GADHOGMDMIG(string name, int value)
	{
		GameCurrency cJJOFMHLFFM = GameUtils.AJDKHINLIDI.ICFINJLNCPM(name);
		if (cJJOFMHLFFM == null)
		{
			cJJOFMHLFFM = new GameCurrency(name, name);
			GameUtils.AJDKHINLIDI.FCDLIEFIIGG(cJJOFMHLFFM);
		}
		GADHOGMDMIG(cJJOFMHLFFM, value);
	}

	public void GADHOGMDMIG(GameCurrency MDDNHLBDJBN, int value)
	{
		if (value < 0)
		{
			value = 0;
		}
		CurrencyStruct lAPFHLGNAAF = MEOLAFECPHI(MDDNHLBDJBN);
		if (lAPFHLGNAAF == null)
		{
			CurrencyStruct item = new CurrencyStruct(MDDNHLBDJBN, value);
			DNPGEMMDPNN.Add(item);
			lAPFHLGNAAF = DNPGEMMDPNN[DNPGEMMDPNN.Count - 1];
		}
		lAPFHLGNAAF.Count = (ObscuredInt)(value);
		string mENAJEAJJBE = lAPFHLGNAAF.BKDEAGGPNAO.Name;
		if (ALMPFGCDAOP == null)
		{
			ALMPFGCDAOP = _node.ACBPMPMPKJJ("Currencies");
		}
		if (ALMPFGCDAOP.Attributes[mENAJEAJJBE] == null)
		{
			ALMPFGCDAOP.LLIKNHNLGJJ(mENAJEAJJBE);
		}
		ALMPFGCDAOP.Attributes[mENAJEAJJBE].Value = value.ToString();
		GGGEHAGCLGC();
		CallEvent(2, lAPFHLGNAAF);
	}

	public CurrencyStruct MEOLAFECPHI(string name)
	{
		foreach (CurrencyStruct item in DNPGEMMDPNN)
		{
			if (item.BKDEAGGPNAO.Name == name)
			{
				return item;
			}
		}
		return null;
	}

	public CurrencyStruct MEOLAFECPHI(GameCurrency MDDNHLBDJBN)
	{
		foreach (CurrencyStruct item in DNPGEMMDPNN)
		{
			if (item.BKDEAGGPNAO == MDDNHLBDJBN)
			{
				return item;
			}
		}
		return null;
	}

	public void AddCurrencyCount(string name, int value)
	{
		GameCurrency cJJOFMHLFFM = GameUtils.AJDKHINLIDI.ICFINJLNCPM(name);
		if (cJJOFMHLFFM == null)
		{
			cJJOFMHLFFM = new GameCurrency(name, name);
			GameUtils.AJDKHINLIDI.FCDLIEFIIGG(cJJOFMHLFFM);
		}
		if (cJJOFMHLFFM != null)
		{
			AddCurrencyCount(cJJOFMHLFFM, value);
		}
	}

	public void AddCurrencyCount(GameCurrency MDDNHLBDJBN, int value)
	{
		CurrencyStruct lAPFHLGNAAF = MEOLAFECPHI(MDDNHLBDJBN);
		int num = 0;
		if (lAPFHLGNAAF != null)
		{
			num = (ObscuredInt)(lAPFHLGNAAF.Count);
		}
		GADHOGMDMIG(MDDNHLBDJBN, num + value);
	}

	public bool GetIsCurrencyExist(string name)
	{
		return MEOLAFECPHI(name) != null;
	}

	public int GetCurrencyCount(string name)
	{
		CurrencyStruct lAPFHLGNAAF = MEOLAFECPHI(name);
		if (lAPFHLGNAAF != null)
		{
			return (ObscuredInt)(lAPFHLGNAAF.Count);
		}
		return 0;
	}

	public int IJCGBPDAAJF(string name)
	{
		ResistanceStruct lONBLHKCFDH = PMMBCHOFBNL(name);
		if (lONBLHKCFDH != null)
		{
			return (ObscuredInt)(lONBLHKCFDH.Count);
		}
		return 0;
	}

	public string ODMONBDLMIP()
	{
		MarketSettings cAGGDFBMJKG = AssemblyController.JONCCPLEIBE();
		if (cAGGDFBMJKG.NPNOMBEEPJD() || cAGGDFBMJKG.OPCBKOOFMAK())
		{
			return "ama";
		}
		if (cAGGDFBMJKG.BKGIFIPIHAL())
		{
			return "chn";
		}
		if (cAGGDFBMJKG.DMJJDFCAKFG())
		{
			return "kak";
		}
		return SystemProperties.IAAKNCJMAAK();
	}

	public void BAOKBJGLKEF(string FHLFEBDNIFF)
	{
		if (get_Parameters() != null)
		{
			get_Parameters().HNKFHGOOKEG = FHLFEBDNIFF;
		}
		EMDLLIGKONG("Avatar", FHLFEBDNIFF);
	}

	public void AFAKCAMAACM()
	{
		LocalizationManager.Language pPNFBAFOOAH = null;
		if (BGMCCMHOMJL == string.Empty)
		{
			string eOMNCDDELLB = SystemProperties.NICPICAMAOH().OHCHKFMFDKM();
			pPNFBAFOOAH = LocalizationManager.HHKANICOAAG(eOMNCDDELLB);
			if (pPNFBAFOOAH == null)
			{
				string pOIPGLLCCKC = LocalizationManager.POIPGLLCCKC;
				pPNFBAFOOAH = LocalizationManager.NLFKNPBICED(pOIPGLLCCKC);
			}
		}
		else
		{
			pPNFBAFOOAH = LocalizationManager.NLFKNPBICED(BGMCCMHOMJL);
		}
		if (pPNFBAFOOAH == null)
		{
			LLLOJBFMONN.Error("Roster parse - null language");
			return;
		}
		if (!LocalizationManager.GGBKNBFCBEJ(pPNFBAFOOAH))
		{
			string pOIPGLLCCKC2 = LocalizationManager.POIPGLLCCKC;
			pPNFBAFOOAH = LocalizationManager.NLFKNPBICED(pOIPGLLCCKC2);
		}
		COKACMKOIGD(pPNFBAFOOAH.name);
		LocalizationManager.BJPNKAGDKFL(pPNFBAFOOAH);
	}

	private void DHKODKHPGGN(long LBIGLJLMIDG)
	{
		HBPJBBJFHME = (((ObscuredInt)(_power) != OGLHGFJKMCO) ? (DIJOCFEFHAK - (LBIGLJLMIDG - CHJNGJLCICH)) : (-1));
	}

	private void PLELELJIKEL()
	{
		List<global::Pair<int, uint>> pEDIMBMABIG = GameUtils.HHONBOCJBLB.PEDIMBMABIG;
		foreach (global::Pair<int, uint> item2 in pEDIMBMABIG)
		{
			ObscuredUInt item = (ObscuredUInt)(item2.Second);
			_levelThresholds.Add(item);
		}
	}

	public string NFKHNICBOIB()
	{
		if (OFKGMKADHBD == string.Empty)
		{
			AOIBKCOBABL(EKCEFDBKPAC());
		}
		return OFKGMKADHBD;
	}

	public void AOIBKCOBABL(string value)
	{
		OFKGMKADHBD = value;
		EMDLLIGKONG("CurrentZone", OFKGMKADHBD);
	}

	public void RandomizeObscuredVars()
	{
		MJBFFBPLAGC.GMCADPGOCHM();
		BIOMOEDPLIP.GMCADPGOCHM();
		BDONIKLHFLJ.GMCADPGOCHM();
		IAFKGOFKDKE.GMCADPGOCHM();
		_experience.GMCADPGOCHM();
		_power.GMCADPGOCHM();
		_levelThresholds.ForEach((ObscuredUInt DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.GMCADPGOCHM();
		});
	}

	public string EKCEFDBKPAC()
	{
		int num = 0;
		foreach (RosterBattle item in HLHEFIKFBHH)
		{
			if (item.NLIJBCHAEBK())
			{
				continue;
			}
			bool flag = false;
			foreach (RosterFight item2 in JNPMCNMEOLE)
			{
				if (item2.GIDNOKCJLPL() == item.KHGCEFNBDDG().CPHDPCAECJN())
				{
					flag = true;
					int num2 = item.EDHMHFONDAI.LKDFFCADHNO().KDJNDHLHAFH();
					if (num2 > num)
					{
						num = num2;
					}
				}
				if (flag)
				{
					continue;
				}
				int num3 = item.EDHMHFONDAI.KCIKELGFHOA();
				if (num3 > 0)
				{
					int num4 = item.EDHMHFONDAI.LKDFFCADHNO().KDJNDHLHAFH();
					if (num4 > num)
					{
						num = num4;
					}
				}
			}
		}
		if (num < ListSF.FHAIJEAPFEA().Count)
		{
			Zone pKCPOJKLMOK = ListSF.FHAIJEAPFEA()[num];
			return pKCPOJKLMOK.get_Name();
		}
		return "ZONE_1";
	}

	private void KLCCBHMJKFI(int count)
	{
		GAKAEGHGNGD = count;
		EMDLLIGKONG("GPlusFiledLogins", GAKAEGHGNGD);
	}

	private void OJNFOCEKFNC(XmlNode node)
	{
		Sound.NMKBJANLIEO(PDGHOOLJNMI, DNBFEGDILIA);
		XmlNode xmlNode = node["Sounds"];
		if (xmlNode != null)
		{
			XmlNode xmlNode2 = xmlNode["Sound"];
			Sound.JOFLPDCONNC(xmlNode2.Attributes["Value"].ParseFloat(1f));
			Sound.FLOFHMBDHNM(xmlNode2.Attributes["Mute"].ParseBool());
			xmlNode2 = xmlNode["Music"];
			Sound.OAFCOFNOIJK(xmlNode2.Attributes["Value"].ParseFloat(1f));
			Sound.FMLHEDIPGAF(xmlNode2.Attributes["Mute"].ParseBool());
			ListSF.GKAOOOICJAI = Sound.ELHMADOKHHE();
		}
		else
		{
			// Older/default desktop saves may not contain the per-warrior copy of
			// these settings.  The values below are the runtime defaults, so this is
			// recoverable and must not abort a Windows Editor playtest.
			Debug.LogWarning("[UserData] Warrior Sounds section missing; using default volumes.");
			Sound.JOFLPDCONNC(1f);
			Sound.FLOFHMBDHNM(false);
			Sound.OAFCOFNOIJK(1f);
			Sound.FMLHEDIPGAF(false);
		}
	}

	private void BCLGMKICMJM(XmlNode HBKKCEFLPPE)
	{
		List<GameCurrency> list = GameUtils.AJDKHINLIDI.IIAPDCECFCN();
		foreach (GameCurrency item2 in list)
		{
			XmlAttribute cJBEMNNNHDM = HBKKCEFLPPE.Attributes[item2.Name];
			if (cJBEMNNNHDM.Empty())
			{
				HBKKCEFLPPE.LLIKNHNLGJJ(item2.Name).Value = "0";
			}
			CurrencyStruct item = new CurrencyStruct(item2, cJBEMNNNHDM.ParseInt());
			DNPGEMMDPNN.Add(item);
		}
	}

	private ResistanceStruct PMMBCHOFBNL(string name)
	{
		foreach (ResistanceStruct item in OEJCIFGACNG)
		{
			if (item.PIFOHOOFJDE.Name == name)
			{
				return item;
			}
		}
		return null;
	}

	public bool LDHANGLFDPJ()
	{
		return MCPCBHPLOPP;
	}

	public void MOBIJGMLNLI(bool value)
	{
		MCPCBHPLOPP = value;
		EMDLLIGKONG("RaidRemindRandomRule", MCPCBHPLOPP);
	}
}
