using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core.Fights.Renders.Model;
using UnityEngine;

public class Model : global::EventDispatcher<object>
{
	private readonly Dictionary<string, int> _transientPerkFlags = new Dictionary<string, int>();

	public void AddTransientPerkFlag(string name, int frames)
	{
		if (!string.IsNullOrEmpty(name))
			_transientPerkFlags[name] = Time.frameCount + Mathf.Max(1, frames);
	}

	public bool HasTransientPerkFlag(string name)
	{
		int expires;
		if (!_transientPerkFlags.TryGetValue(name, out expires))
			return false;
		if (Time.frameCount <= expires)
			return true;
		_transientPerkFlags.Remove(name);
		return false;
	}

	public class KAKDJGKPFEH
	{
		public List<InfoAnimation> PLJJONIFHJN = new List<InfoAnimation>();

		public List<InfoAnimation> POJOOBPNEAK = new List<InfoAnimation>();

		public List<InfoAnimation> ILNELINMJEG = new List<InfoAnimation>();

		public List<InfoAnimation> EMKHLEINEBI = new List<InfoAnimation>();

		public List<InfoAnimation> KHMAPLHHBDI = new List<InfoAnimation>();

		public List<InfoAnimation> LNHBEDPOGBI = new List<InfoAnimation>();

		public List<InfoAnimation> PADGFKKHGFF = new List<InfoAnimation>();

		public List<InfoAnimation> ECOIADPIBJJ = new List<InfoAnimation>();

		public List<InfoAnimation> IFDEGOAJDBP = new List<InfoAnimation>();

		public List<InfoAnimation> GCKHGBKLPHM = new List<InfoAnimation>();

		public List<InfoAnimation> JDGIMJPEGON = new List<InfoAnimation>();

		public List<InfoAnimation> NIMANOOEBAJ = new List<InfoAnimation>();

		public List<InfoAnimation> KMAAHHEBKMG = new List<InfoAnimation>();

		public List<InfoAnimation> JHCMCMFOGCI = new List<InfoAnimation>();

		public void JIKDAIELFBF()
		{
			PLJJONIFHJN.Clear();
			POJOOBPNEAK.Clear();
			ILNELINMJEG.Clear();
			KHMAPLHHBDI.Clear();
			LNHBEDPOGBI.Clear();
			PADGFKKHGFF.Clear();
			ECOIADPIBJJ.Clear();
			IFDEGOAJDBP.Clear();
			GCKHGBKLPHM.Clear();
			JDGIMJPEGON.Clear();
			NIMANOOEBAJ.Clear();
			KMAAHHEBKMG.Clear();
			JHCMCMFOGCI.Clear();
		}

		public void IDIHFINEDMI(List<InfoAnimation> MAHEJFLCCHP)
		{
			for (int i = 0; i < MAHEJFLCCHP.Count; i++)
			{
				InfoAnimation pJAHIOELGGD = MAHEJFLCCHP[i];
				if (pJAHIOELGGD.ODACDCDONJE.DFLNENOIMPO.IsExists)
				{
					JHCMCMFOGCI.Add(pJAHIOELGGD);
				}
				List<EventAnimation> aJCMBMJGJEG = pJAHIOELGGD.ODACDCDONJE.AJCMBMJGJEG;
				for (int j = 0; j < aJCMBMJGJEG.Count; j++)
				{
					List<InfoAnimation> list = NCNDKFCPLEH(aJCMBMJGJEG[j].Type);
					int count = list.Count;
					if (count == 0 || list[count - 1] != pJAHIOELGGD)
					{
						list.Add(pJAHIOELGGD);
					}
				}
				PLJJONIFHJN.Add(pJAHIOELGGD);
			}
		}

		public List<InfoAnimation> NCNDKFCPLEH(EventAnimation.EECEJKADLCK IGABHEMGKKE)
		{
			switch (IGABHEMGKKE)
			{
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END:
				return LNHBEDPOGBI;
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_START:
				return KHMAPLHHBDI;
			case EventAnimation.EECEJKADLCK.EVENT_BIRTH:
				return NIMANOOEBAJ;
			case EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME:
				return JDGIMJPEGON;
			case EventAnimation.EECEJKADLCK.EVENT_HIT:
				return IFDEGOAJDBP;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END:
				return ECOIADPIBJJ;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_START:
				return PADGFKKHGFF;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED:
				return ILNELINMJEG;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_RELEASED:
				return EMKHLEINEBI;
			case EventAnimation.EECEJKADLCK.EVENT_MOD_EXPIRES:
				return KMAAHHEBKMG;
			case EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE:
				return POJOOBPNEAK;
			case EventAnimation.EECEJKADLCK.EVENT_STRIKE:
				return GCKHGBKLPHM;
			default:
				return PLJJONIFHJN;
			}
		}
	}

	public class GCNLDBMDDGD
	{
		public List<Trigger> GIFPBBKCKIK = new List<Trigger>();

		public List<Trigger> POJOOBPNEAK = new List<Trigger>();

		public List<Trigger> ILNELINMJEG = new List<Trigger>();

		public List<Trigger> EMKHLEINEBI = new List<Trigger>();

		public List<Trigger> KHMAPLHHBDI = new List<Trigger>();

		public List<Trigger> LNHBEDPOGBI = new List<Trigger>();

		public List<Trigger> PADGFKKHGFF = new List<Trigger>();

		public List<Trigger> ECOIADPIBJJ = new List<Trigger>();

		public List<Trigger> IFDEGOAJDBP = new List<Trigger>();

		public List<Trigger> GCKHGBKLPHM = new List<Trigger>();

		public List<Trigger> JDGIMJPEGON = new List<Trigger>();

		public List<Trigger> NIMANOOEBAJ = new List<Trigger>();

		public List<Trigger> KMAAHHEBKMG = new List<Trigger>();

		public void NKKOAAKHINN()
		{
			GIFPBBKCKIK.Clear();
			POJOOBPNEAK.Clear();
			ILNELINMJEG.Clear();
			EMKHLEINEBI.Clear();
			KHMAPLHHBDI.Clear();
			LNHBEDPOGBI.Clear();
			PADGFKKHGFF.Clear();
			ECOIADPIBJJ.Clear();
			IFDEGOAJDBP.Clear();
			GCKHGBKLPHM.Clear();
			JDGIMJPEGON.Clear();
			NIMANOOEBAJ.Clear();
			KMAAHHEBKMG.Clear();
		}

		public void HGPNHBMHIKH(List<Trigger> JJAEKPONOBM)
		{
			foreach (Trigger item in JJAEKPONOBM)
			{
				List<EventAnimation> aJCMBMJGJEG = item.IDEMFOLJIFE.AJCMBMJGJEG;
				foreach (EventAnimation item2 in aJCMBMJGJEG)
				{
					List<Trigger> list = KPMMHDGEBCB(item2.Type);
					int count = list.Count;
					if (count == 0 || list[count - 1] != item)
					{
						list.Add(item);
					}
				}
				GIFPBBKCKIK.Add(item);
			}
		}

		public List<Trigger> KPMMHDGEBCB(EventAnimation.EECEJKADLCK IGABHEMGKKE)
		{
			switch (IGABHEMGKKE)
			{
			case EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE:
				return POJOOBPNEAK;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_PRESSED:
				return ILNELINMJEG;
			case EventAnimation.EECEJKADLCK.EVENT_KEY_RELEASED:
				return EMKHLEINEBI;
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_START:
				return KHMAPLHHBDI;
			case EventAnimation.EECEJKADLCK.EVENT_ANIMATION_END:
				return LNHBEDPOGBI;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_START:
				return PADGFKKHGFF;
			case EventAnimation.EECEJKADLCK.EVENT_INTERVAL_END:
				return ECOIADPIBJJ;
			case EventAnimation.EECEJKADLCK.EVENT_HIT:
				return IFDEGOAJDBP;
			case EventAnimation.EECEJKADLCK.EVENT_STRIKE:
				return GCKHGBKLPHM;
			case EventAnimation.EECEJKADLCK.EVENT_EVERY_FRAME:
				return JDGIMJPEGON;
			case EventAnimation.EECEJKADLCK.EVENT_BIRTH:
				return NIMANOOEBAJ;
			case EventAnimation.EECEJKADLCK.EVENT_MOD_EXPIRES:
				return KMAAHHEBKMG;
			default:
				return GIFPBBKCKIK;
			}
		}
	}

	public class EventDelayedModel
	{
		public object data;

		public EventAnimation.EECEJKADLCK LFLGCDNKNJI;
	}

	public class EventModel
	{
		public Model KJDFJPBIGJC;

		public Model GAIBPAGPEGK;

		public object Data;

		public string ConditionName;

		public void Clear()
		{
			KJDFJPBIGJC = null;
			GAIBPAGPEGK = null;
			Data = null;
		}
	}

	public class DisarmData
	{
		public Model KJDFJPBIGJC;

		public List<PerkInfoItem> NHBIJEEKALC;

		public DisarmData(Model _Model, List<PerkInfoItem> NDEOKNAOAKM)
		{
			KJDFJPBIGJC = _Model;
			NHBIJEEKALC = NDEOKNAOAKM;
		}
	}

	public class EventActBtnSettings
	{
		public FightCID NBIBIANJLEA;

		public float Value;

		public int OCFKLCDIEBF;

		public int PKMHOICGDIM;

		public EventActBtnSettings(FightCID ONKIPLHLPCO, float _value, int _frames = -1, int PEGMAGCDDDC = -1)
		{
			NBIBIANJLEA = ONKIPLHLPCO;
			Value = _value;
			OCFKLCDIEBF = _frames;
			PKMHOICGDIM = PEGMAGCDDDC;
		}
	}

	public enum ADODGMIAHKN
	{
		ON_INTERVAL_START = 0,
		ON_INTERVAL_END = 1,
		ON_ANIMATION_START = 2,
		ON_ANIMATION_END = 3,
		ON_EVERY_FRAME = 4,
		ON_MODEL_DELETE = 5,
		ON_MODEL_CREATE = 6,
		ON_START_EFFECT = 7,
		ON_STOP_EFFECT = 8,
		ON_STOP_FOLLOW_EFFECT = 9,
		ON_KEY_PRESS = 10,
		ON_KEY_RELEASE = 11,
		SETTED_ACT_BTN_PERCENTAGE = 12,
		SETTED_ACT_BTN_COUNT = 18,
		ON_COMBO_CHANGE = 13,
		ON_TRY_ON_END = 14,
		ON_SHAKE_SCREEN = 15,
		ON_DISARM = 16,
		ON_ZOOM_EFFECT = 17
	}

	public class StrikeResult
	{
		public Vector3f Point;

		public Vector3f AOFLADELDFB;

		public Vector3f IIIDIKABLOJ = new Vector3f();

		public ModelEdge CMGLHHEJEBN;

		public ModelEdge ALIHGFIJEDN;

		public InfoAnimation PBPDKJNKFCJ;

		public float HMOLHIEDINK;

		public float EEDJBBOCFNL;

		public float NPDHOJEHPDM;

		public string DefenceAttribute = string.Empty;

		public int Target;

		public int HKKIBOKOHHA;

		public bool LOONMILKCFK;

		public bool JMDIIIFJMFH;

		public bool DFOHNJEBDED;

		public bool DNGKOMPMPCD;

		public bool APCAKCCOMLO;

		public bool NIKPBGPPFEP;

		public bool HOJPKPDBPEJ;

		public Model KJDFJPBIGJC;

		public Model GAIBPAGPEGK;

		public List<int> ProcedPerks = new List<int>();

		public int IICJEIHBABC;

		public void GGENIBPJPAG(int KMEFHNNOLLM)
		{
			ProcedPerks.AddIfNotExist(KMEFHNNOLLM);
		}
	}

	public enum IIKHFKIBPJG
	{
		None = 0,
		Prepare = 1,
		Fight = 2,
		Finish = 3
	}

	public class DelayedStrike
	{
		public InfoAnimation FGICHADOEHF;

		public List<string> Names;

		public bool IsStrikeResult;
	}

	protected class IEFCMFEMACD
	{
		public float EDCHBILGFLD;

		public float NNCHJCLKHHA;
	}

	protected class IDLBAAHEJBI
	{
		public int CNOPJHJONNN;

		public int FAAHGOJJHLK;
	}

	private class HitData
	{
		public Vector3f Point;

		public Vector3f IIIDIKABLOJ;

		public float Time;

		public bool DataReady;
	}

	private class OHCIJJHDAJI
	{
		public InfoAnimation FGICHADOEHF;

		public int GFHOIKMBNHF;

		public int Count;

		public bool IsFrameShift;

		public int FrameShift = -1;

		public bool Empty
		{
			get
			{
				return KLNLNKBIDGD();
			}
		}

		public void Clear()
		{
			FGICHADOEHF = null;
			GFHOIKMBNHF = 0;
			Count = 0;
			IsFrameShift = false;
			FrameShift = -1;
		}

		public bool KLNLNKBIDGD()
		{
			return FGICHADOEHF == null;
		}
	}

	private class ActBtnsCooldown
	{
		public bool KCOBIPMMLEI;

		public bool MLDHFPCCCOP;

		public bool CNGALGBKFOK;

		public bool MPIOLPLLFEM;

		public bool FDHAJDFJBCF;

		public float IHPFDOMGKIL;

		public float HFMDOHKMGHE;

		public int DBMLGHOMCEA;

		public float DPMNMLHCJLK;

		public float MDOBBLKHOHI;

		public int KFALPODCLFA;

		public float KIKLFDLLDDP;

		public float PANKKFJFINL;

		public int JKKENKDLJBK;

		public float CPKHGNDBKFL;

		public float AAIDLAFJECE;

		public int GKAPNAOFMFP;

		public float IFMNJHFPDIC;

		public float GBEADNMMOID;

		public int IKNPPGLMBDK;

		public ActBtnsCooldown()
		{
			KCOBIPMMLEI = false;
			MLDHFPCCCOP = false;
			CNGALGBKFOK = false;
			MPIOLPLLFEM = false;
			FDHAJDFJBCF = false;
			IHPFDOMGKIL = 0f;
			HFMDOHKMGHE = 1f;
			DPMNMLHCJLK = 0f;
			MDOBBLKHOHI = 1f;
			KIKLFDLLDDP = 0f;
			PANKKFJFINL = 1f;
			CPKHGNDBKFL = 0f;
			AAIDLAFJECE = 1f;
			IFMNJHFPDIC = 0f;
			GBEADNMMOID = 1f;
			DBMLGHOMCEA = 0;
			KFALPODCLFA = 0;
			JKKENKDLJBK = 0;
			GKAPNAOFMFP = 0;
			IKNPPGLMBDK = 0;
		}
	}

	private const int RESET_BTN_COOLDOWN_FRAMES = 30;

	public KAKDJGKPFEH CEOOLFLLIMC = new KAKDJGKPFEH();

	public GCNLDBMDDGD NCGEHCHIBBH = new GCNLDBMDDGD();

	public EventModel KDAHHIMLJGG = new EventModel();

	public StrikeResult GHHCDAFIKJE = new StrikeResult();

	public DelayedStrike CEAOMPLGBDG = new DelayedStrike();

	protected IEFCMFEMACD LBOLAOBGDEH = new IEFCMFEMACD();

	protected IDLBAAHEJBI CFJCEPHKHOC = new IDLBAAHEJBI();

	private HitData NPACOADCOPJ = new HitData();

	private OHCIJJHDAJI AAJKEBAIJAP = new OHCIJJHDAJI();

	private ActBtnsCooldown MDFEHKBOHEL = new ActBtnsCooldown();

	public int Index;

	private Model PNNMOKIBOPP;

	public int NFOOGKCGFAB;

	private float PJLKIEDMDOG;

	private float DOEPGPAMEEA;

	public ModelParameters KMMJCHDKBDO;

	public int HIGBAPPOOKJ;

	public bool MHLIECOENGH;

	public List<Model> _Enemies = new List<Model>();

	private bool HCPHOJKFIDM;

	public bool NIKPBGPPFEP;

	public int JMHJDHLBHLK;

	public bool IDCHHGHAENM;

	public bool POCBCFMBKLO;

	public bool PKFJFFGDOLB;

	public int NPKHMEHKFMM;

	public int OKDDOLCHDCM;

	public bool FLKMDFDEJPP;

	public bool CAJANPOIPFC;

	public bool HEJMFGFBLDK;

	public int ANBOFDHNKKO;

	public int HLNDHODNMCE;

	public int PACHBHGEIGN;

	public string LDLLJHEDCPD;

	public float CDBOONBLDBK;

	public float JCEOKJKKMCC;

	public int MDNMFCIICAN = -1;

	public EventAnimation.EECEJKADLCK KMDKCFHMECJ;

	public InfoAnimation.MGHNBEPCKIF DFLPNNBIFFN;

	private List<WeaponModel> JLDBGHLBJEL = new List<WeaponModel>();

	private List<ModelEdge> ECNLLKIJIGP;

	private List<CurrentEffect> BJKJBIMPPAM = new List<CurrentEffect>();

	private string _Name;

	private string _ExplicitBirthAnimation;

	private ModelConditions _ModelConditions = new ModelConditions();

	private ModelObject _ModelObject;

	private ModelController FEHOHLMIEBP = new ModelController();

	private FightStatistics _Statistics = new FightStatistics();

	private IIKHFKIBPJG OLGJILOCIEH;

	private ItemInfo ODLJHBDMEIJ;

	private Model BFFLLGHDPEB;

	private ModelStrike _Strike;

	private ModelPhysics _Physics;

	private ModelCollision _Collision;

	private ModelAnimation _Animation;

	// Compatibility state used by newer perk actions.  It lives on the model so
	// timed modifiers can be applied and reliably undone by InfoPerk.
	private int _perkSlowFactor = 1;
	private int _perkSlowFrame;
	private bool _perkCollisionDisabled;
	private Color _perkColor = Color.white;

	private ModelAi HJOGNGDMAKJ;

	private ModelStatistics DKFGOHCNIKL;

	private int FJGNHALJJFF;

	private bool NKNAANIBJPK;

	private bool _IsShock;

	private bool KICHLMBENOL;

	private int APOHBENDEKO;

	private int HLLAJOBDPEC;

	private int CHOJGIFFEMB;

	private float FJKIGPFIEDN;

	private bool GDGHBKAENHK;

	private bool LELOFPFOBGO;

	private bool HOOKPFLBFPD;

	private float LEAEFADGBBO;

	private bool ILGIOPFIAAA;

	private int LGLIHLJPDIO;

	private int DJOKGDICHAJ;

	private int AIAKAAECMEH;

	private int AAEFMEJBMLH;

	private ComboCounter BNFCCKBIIDB = new ComboCounter();

	private float ONEKAHDNEMF;

	private float EOIAPNIGKAA;

	private List<InfoAnimation> OHAMEHHMEAL = new List<InfoAnimation>();

	private List<Trigger> NMILPLHGCMA = new List<Trigger>();

	private float NJDNNFJAFBG;

	private int MJEJFBHOJKB;

	private int PDGBMLJEJKG;

	private bool PJIHDNFHEGA;

	private DetailedDamages IKPAEKHOJLA = new DetailedDamages();

	private int KLLMOEACGLF;

	private Vector3f ODCOKJKEDOJ;

	private float HNILMKEAMAE;

	private float DIKMCKLIEBK;

	private bool PEACCBDCNCN;

	private GameObject _UnityObject;

	public MeshRender _MeshRender;

	private bool PJLKOEIMJNA;

	public Model OMICHOJFOMN
	{
		get
		{
			return EGGEACCDAEK();
		}
	}

	public float DDFBIOFIDIH
	{
		get
		{
			return DOHFDGMPHMH();
		}
	}

	public float DHHDGCCHIDH
	{
		get
		{
			return NBDHHNJPPEM();
		}
	}

	public float JKCGFAECLEC
	{
		get
		{
			return PAMDOBKGCDF();
		}
	}

	public float JMINNNJEAGF
	{
		get
		{
			return APICLPNBBAD();
		}
	}

	public bool AMKJNPOCODK
	{
		get
		{
			return FPCIAAPDIEI();
		}
		set
		{
			AHBNPODMIOD(value);
		}
	}

	public List<WeaponModel> KHPHALAIIML
	{
		get
		{
			return KGGIDBLBMDJ();
		}
	}

	public ModelConditions FIENGMKDBFA
	{
		get
		{
			return EBABHGHPLFK();
		}
	}

	public ModelObject KFDGGLKBKEP
	{
		get
		{
			return CLDMEJKGLBA();
		}
	}

	public ModelController OPGBICMNFPA
	{
		get
		{
			return DEGJJOMLJGM();
		}
	}

	public FightStatistics KKJJANBIMAG
	{
		get
		{
			return DJLNJPMAHDL();
		}
	}

	public IIKHFKIBPJG GCDHNODCJAA
	{
		get
		{
			return GMLCBPDGIKI();
		}
	}

	public Model OPGFNPGDEFO
	{
		get
		{
			return NJDJHGDMCIJ();
		}
	}

	public ModelPhysics LNMKPKINFPO
	{
		get
		{
			return COBOFMDFLJO();
		}
	}

	public ModelCollision OHMHAKNFPDM
	{
		get
		{
			return ILELHCIDKFC();
		}
	}

	public ModelAnimation DAPLCAPAPDI
	{
		get
		{
			return OCPMJKIEPIG();
		}
	}

	public InfoAnimation KJHMOGGECBN
	{
		get
		{
			return FHBLLPCEAHG();
		}
	}

	public List<IntervalAnimation> Intervals
	{
		get
		{
			return KPJAEBBJFEO();
		}
	}

	public ModelAi EIINKLJLDCI
	{
		get
		{
			return EEIGOJBKFGE();
		}
	}

	public ModelStatistics MHKHFNPAGNC
	{
		get
		{
			return FGACEEPJBIF();
		}
	}

	public bool EAJHPCJJCDI
	{
		get
		{
			return HFHJFOEFPCD();
		}
		set
		{
			MLIIBCBGHBH(value);
		}
	}

	public bool PFDCDIBODCL
	{
		get
		{
			return EDJFLMILEBA();
		}
		set
		{
			set_IsShock(value);
		}
	}

	public bool LFCEHIDMGBN
	{
		get
		{
			return MBCLINNCNAL();
		}
		set
		{
			BPMKBIKKEOI(value);
		}
	}

	public float EFFNFFJAHLJ
	{
		get
		{
			return IDFIBPDPFLK();
		}
	}

	public bool KFCEEKDBJKP
	{
		get
		{
			return ACKCLIFPAEB();
		}
		set
		{
			MOMEOOGDELJ(value);
		}
	}

	public bool FOBGFDMIAFO
	{
		get
		{
			return IJINDLLEGKA();
		}
		set
		{
			MABELGMBHEA(value);
		}
	}

	public float FBNIIOCFEOL
	{
		get
		{
			return DHGBNMLMMLL();
		}
		set
		{
			OLGNPKCPKOJ(value);
		}
	}

	public bool GIBIBHGDIMF
	{
		get
		{
			return LLBJPPAJOHE();
		}
		set
		{
			FHMLAFHENBB(value);
		}
	}

	public int HCAMIJDPFCH
	{
		get
		{
			return JNMIPPPMAJC();
		}
	}

	public int IDNNLIODADH
	{
		get
		{
			return KADMPAHPOLD();
		}
	}

	public float OHLBLAJPHBD
	{
		get
		{
			return LJCFIOPBNKD();
		}
		set
		{
			PFIJCCKDAAB(value);
		}
	}

	public float FBJOKBKHOLL
	{
		get
		{
			return FGNCFGDOELL();
		}
		set
		{
			CBACHJEDIHC(value);
		}
	}

	public List<InfoAnimation> EEOIKBPHJOL
	{
		get
		{
			return MCFPDHOLNGB();
		}
	}

	public List<Trigger> GIFPBBKCKIK
	{
		get
		{
			return NOJEIGNOPII();
		}
	}

	public float GHEBFDIINLD
	{
		get
		{
			return EKAFGLHNMCN();
		}
		set
		{
			OGHAMAGPFLF(value);
		}
	}

	public int JJDNDOLCMMN
	{
		get
		{
			return LPOJKGLFMAL();
		}
		set
		{
			FLBDBIHFJAI(value);
		}
	}

	public int KHDBLNPFDPE
	{
		get
		{
			return CKAKLHDLHJO();
		}
		set
		{
			KBKIMPEHPKF(value);
		}
	}

	public DetailedDamages MNDEOFOHLHI
	{
		get
		{
			return NDOAKHGPOHL();
		}
	}

	public int JEPLKEFHBIN
	{
		get
		{
			return EJJIGHLCKEN();
		}
	}

	public float JNKHFJJGJAF
	{
		get
		{
			return DNOILFCGCGD();
		}
		set
		{
			set_HitEffectScale(value);
		}
	}

	public float OMKEMNJKFEO
	{
		get
		{
			return JKEKBCJHANF();
		}
		set
		{
			set_AdditionalDamageValue(value);
		}
	}

	public bool MODJPMKEIFD
	{
		get
		{
			return NMPHACPBHKO();
		}
		set
		{
			KKLMIAFFKNE(value);
		}
	}

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public Vector3f BPPINEHFOBB
	{
		get
		{
			return PLBNCDCFPML();
		}
	}

	public bool FNFLOBIGBPL
	{
		get
		{
			return FGKAFKFBFEM();
		}
	}

	public bool POICIBFGFGC
	{
		get
		{
			return BCKKCJONNHG();
		}
	}

	public bool BOGDKNENPOA
	{
		get
		{
			return HPCNJFPOONE();
		}
	}

	public bool HKJFJHBHMND
	{
		get
		{
			return JGAOCNLLDFG();
		}
	}

	public bool KBNBAEMOMOC
	{
		get
		{
			return HIPJNBEFGHN();
		}
	}

	public bool IsPlayer
	{
		get
		{
			return EPCNJLEHJCB();
		}
	}

	public virtual bool FDELMAHAAJD
	{
		get
		{
			return KIAFPPHPEEK();
		}
	}

	public int GFHOIKMBNHF
	{
		get
		{
			return KFCNPADAMHA();
		}
	}

	public int PKMHOICGDIM
	{
		get
		{
			return GLEKCPCMINJ();
		}
	}

	public InfoAnimation NPPJFBPHLPI
	{
		get
		{
			return EJOGECPBJCE();
		}
	}

	public bool BILPAMPHDOO
	{
		get
		{
			return IBIDGACDJNF();
		}
	}

	public bool GBFMIGANOAD
	{
		get
		{
			return DCNFLKPDIFJ();
		}
	}

	public KeyData FONEJOKEIEN
	{
		get
		{
			return ANALKHBJKIO();
		}
	}

	public bool EKEPPACCCPI
	{
		get
		{
			return NMEEPBDJHMG();
		}
	}

	public bool JGDIKHKBCIA
	{
		get
		{
			return CDMBCHOJKPH();
		}
	}

	public bool NCBPMBJCFBK
	{
		get
		{
			return NLHFJIEHKMM();
		}
	}

	public List<string> JABJKGKDKPM
	{
		get
		{
			return KGHDFCKGAEO();
		}
	}

	public IntervalAnimation LMPPINHMHJA
	{
		get
		{
			return FDMAIINMCHH();
		}
	}

	public bool KIEGHNCKBKP
	{
		get
		{
			return AMGHOKDANGN();
		}
	}

	public Model BKJOIIOFPKC
	{
		get
		{
			return BDJBNOPNCNB();
		}
	}

	public float BOGHNBAKCEL
	{
		get
		{
			return KJFIBMMOEPI();
		}
	}

	public float PCIBKEOCFAO
	{
		get
		{
			return PHHHEGOBAPB();
		}
	}

	public bool AIOKFELLEEP
	{
		get
		{
			return LAGNKLAADPO();
		}
	}

	public int BJDFMKOCNBN
	{
		get
		{
			return LPFPGDJALED();
		}
	}

	public int BBGDBGIPBJP
	{
		get
		{
			return NODAINEDAKJ();
		}
	}

	public bool FJMLDBBLHEN
	{
		get
		{
			return IMCHEIAGOPF();
		}
	}

	public bool KFFBBLOCLEL
	{
		get
		{
			return ANHGOGDEFCO();
		}
	}

	public int GKAEJDCDMHC
	{
		get
		{
			return NPDOLGNNINO();
		}
	}

	public int DEENENNCBBC
	{
		get
		{
			return CLPDEPPPJFE();
		}
	}

	public float GPKALMFLOPP
	{
		get
		{
			return KKMCHCNOHMB();
		}
	}

	public bool NNLNMDBOILM
	{
		get
		{
			return PDFCAFIMALN();
		}
	}

	public bool BKLPNNIBJBE
	{
		get
		{
			return FPPKOMOPDJJ();
		}
	}

	protected float LMBBAAAIHIK
	{
		get
		{
			return EOGCPJJHCCA();
		}
	}

	public bool JKNIIPNFJJP
	{
		get
		{
			return FGAHBBDGPBO();
		}
		set
		{
			NEHLJGPKHKF(value);
		}
	}

	protected float HCEGCDFEMGK
	{
		get
		{
			return HFLFIKJPGCJ();
		}
	}

	public Model(ModelParameters data)
	{
		_UnityObject = new GameObject("Model");
		GameObject gameObject = new GameObject("Mesh");
		gameObject.transform.SetParent(_UnityObject.transform, false);
		_MeshRender = gameObject.AddComponent<MeshRender>();
		NPKHMEHKFMM = 0;
		JMHJDHLBHLK = -1;
		FLKMDFDEJPP = true;
		Index = 0;
		PNNMOKIBOPP = null;
		NFOOGKCGFAB = 0;
		PJLKIEDMDOG = 0f;
		DOEPGPAMEEA = 0f;
		MHLIECOENGH = false;
		HCPHOJKFIDM = false;
		IDCHHGHAENM = false;
		POCBCFMBKLO = false;
		set_IsShock(false);
		HIGBAPPOOKJ = 0;
		ILGIOPFIAAA = true;
		PKFJFFGDOLB = false;
		BFFLLGHDPEB = null;
		_Strike = null;
		_Physics = null;
		_Collision = null;
		_Animation = null;
		HJOGNGDMAKJ = null;
		CHOJGIFFEMB = 0;
		OLGJILOCIEH = IIKHFKIBPJG.None;
		LBOLAOBGDEH = new IEFCMFEMACD();
		CFJCEPHKHOC = new IDLBAAHEJBI();
		HLLAJOBDPEC = 0;
		LELOFPFOBGO = false;
		GDGHBKAENHK = false;
		APOHBENDEKO = -1;
		KMMJCHDKBDO = data;
		ODLJHBDMEIJ = null;
		FJGNHALJJFF = -1;
		NIKPBGPPFEP = false;
		FJKIGPFIEDN = 0f;
		OKDDOLCHDCM = 0;
		HOOKPFLBFPD = false;
		LEAEFADGBBO = 1f;
		DKFGOHCNIKL = new ModelStatistics(this);
		KICHLMBENOL = false;
		LGLIHLJPDIO = 0;
		NJDNNFJAFBG = 0f;
		MJEJFBHOJKB = 0;
		PDGBMLJEJKG = 0;
		KLLMOEACGLF = 0;
		MDNMFCIICAN = -1;
		PJIHDNFHEGA = false;
		MDFEHKBOHEL = new ActBtnsCooldown();
		ODCOKJKEDOJ = new Vector3f(1f, 1f, 1f);
		HNILMKEAMAE = 1f;
		DIKMCKLIEBK = 0f;
		PEACCBDCNCN = true;
		ONEKAHDNEMF = 1f;
		EOIAPNIGKAA = 0f;
		DJOKGDICHAJ = 0;
		AIAKAAECMEH = 0;
		AAEFMEJBMLH = 0;
		BNFCCKBIIDB.AddEventListener(0, KPAPLCPCOBE);
	}

	public Model EGGEACCDAEK()
	{
		return PNNMOKIBOPP;
	}

	public float DOHFDGMPHMH()
	{
		return PJLKIEDMDOG;
	}

	public float NBDHHNJPPEM()
	{
		return DOEPGPAMEEA;
	}

	public float PAMDOBKGCDF()
	{
		return (KFCNPADAMHA() != 1) ? LBOLAOBGDEH.EDCHBILGFLD : LBOLAOBGDEH.NNCHJCLKHHA;
	}

	public float APICLPNBBAD()
	{
		return (KFCNPADAMHA() != 1) ? LBOLAOBGDEH.NNCHJCLKHHA : LBOLAOBGDEH.EDCHBILGFLD;
	}

	public void AHBNPODMIOD(bool value)
	{
		HCPHOJKFIDM = value;
		if (value)
		{
			FEHOHLMIEBP.Reset();
			BHAFOEICJPE(0);
		}
	}

	public bool FPCIAAPDIEI()
	{
		return HCPHOJKFIDM;
	}

	public List<WeaponModel> KGGIDBLBMDJ()
	{
		return JLDBGHLBJEL;
	}

	public string get_Name()
	{
		return _Name;
	}

	public void set_Name(string value)
	{
		_Name = value;
		_ModelConditions.ModelName = value;
	}

	public ModelConditions EBABHGHPLFK()
	{
		return _ModelConditions;
	}

	public ModelObject CLDMEJKGLBA()
	{
		return _ModelObject;
	}

	public ModelController DEGJJOMLJGM()
	{
		return FEHOHLMIEBP;
	}

	public FightStatistics DJLNJPMAHDL()
	{
		return _Statistics;
	}

	public IIKHFKIBPJG GMLCBPDGIKI()
	{
		return OLGJILOCIEH;
	}

	public Model NJDJHGDMCIJ()
	{
		return BFFLLGHDPEB;
	}

	public ModelPhysics COBOFMDFLJO()
	{
		return _Physics;
	}

	public ModelCollision ILELHCIDKFC()
	{
		return _Collision;
	}

	public ModelAnimation OCPMJKIEPIG()
	{
		return _Animation;
	}

	public InfoAnimation FHBLLPCEAHG()
	{
		return _Animation.NNMAFFCCMHC();
	}

	public List<IntervalAnimation> KPJAEBBJFEO()
	{
		return _Animation.PCKKMNHDDMP();
	}

	public ModelAi EEIGOJBKFGE()
	{
		return HJOGNGDMAKJ;
	}

	public ModelStatistics FGACEEPJBIF()
	{
		return DKFGOHCNIKL;
	}

	public bool HFHJFOEFPCD()
	{
		return NKNAANIBJPK;
	}

	public void MLIIBCBGHBH(bool value)
	{
		NKNAANIBJPK = value;
	}

	public bool EDJFLMILEBA()
	{
		return _IsShock;
	}

	public void set_IsShock(bool value)
	{
		_IsShock = value;
	}

	public bool MBCLINNCNAL()
	{
		return KICHLMBENOL;
	}

	public void BPMKBIKKEOI(bool value)
	{
		KICHLMBENOL = value;
	}

	public float IDFIBPDPFLK()
	{
		return FJKIGPFIEDN;
	}

	public bool ACKCLIFPAEB()
	{
		return GDGHBKAENHK;
	}

	public void MOMEOOGDELJ(bool value)
	{
		_IsShock = value;
	}

	public void MABELGMBHEA(bool value)
	{
		HOOKPFLBFPD = value;
		for (int i = 0; i < JLDBGHLBJEL.Count; i++)
		{
			JLDBGHLBJEL[i].MABELGMBHEA(value);
		}
	}

	public bool IJINDLLEGKA()
	{
		return HOOKPFLBFPD;
	}

	public float DHGBNMLMMLL()
	{
		return LEAEFADGBBO;
	}

	public void OLGNPKCPKOJ(float value)
	{
		LEAEFADGBBO = value;
		for (int i = 0; i < JLDBGHLBJEL.Count; i++)
		{
			JLDBGHLBJEL[i].OLGNPKCPKOJ(value);
		}
	}

	public bool LLBJPPAJOHE()
	{
		return (BFFLLGHDPEB == null) ? ILGIOPFIAAA : BFFLLGHDPEB.LLBJPPAJOHE();
	}

	public void FHMLAFHENBB(bool value)
	{
		if (BFFLLGHDPEB != null)
		{
			BFFLLGHDPEB.FHMLAFHENBB(value);
		}
		else
		{
			ILGIOPFIAAA = value;
		}
	}

	public int DKDGOOLAAKN()
	{
		return LGLIHLJPDIO;
	}

	public void set_Round(int value)
	{
		LGLIHLJPDIO = value;
	}

	public int JNMIPPPMAJC()
	{
		return DJOKGDICHAJ;
	}

	public int KADMPAHPOLD()
	{
		return AIAKAAECMEH;
	}

	public float LJCFIOPBNKD()
	{
		return ONEKAHDNEMF;
	}

	public void PFIJCCKDAAB(float value)
	{
		ONEKAHDNEMF = value;
		for (int i = 0; i < JLDBGHLBJEL.Count; i++)
		{
			JLDBGHLBJEL[i].PFIJCCKDAAB(value);
		}
	}

	public float FGNCFGDOELL()
	{
		return EOIAPNIGKAA;
	}

	public void CBACHJEDIHC(float value)
	{
		EOIAPNIGKAA = value;
		for (int i = 0; i < JLDBGHLBJEL.Count; i++)
		{
			JLDBGHLBJEL[i].CBACHJEDIHC(value);
		}
	}

	public List<InfoAnimation> MCFPDHOLNGB()
	{
		return OHAMEHHMEAL;
	}

	public List<Trigger> NOJEIGNOPII()
	{
		return NMILPLHGCMA;
	}

	public void OGHAMAGPFLF(float value)
	{
		if (1f < value)
		{
			NJDNNFJAFBG = 1f;
		}
		else if (value < 0f)
		{
			NJDNNFJAFBG = 0f;
		}
		else
		{
			NJDNNFJAFBG = value;
		}
	}

	public float EKAFGLHNMCN()
	{
		return NJDNNFJAFBG;
	}

	public void FLBDBIHFJAI(int value)
	{
		if (value != 0 && value != 1)
		{
			LLLOJBFMONN.Error("Wrong magic count {0}", value);
		}
		MJEJFBHOJKB = value;
	}

	public int LPOJKGLFMAL()
	{
		return MJEJFBHOJKB;
	}

	public int CKAKLHDLHJO()
	{
		return PDGBMLJEJKG;
	}

	public void KBKIMPEHPKF(int value)
	{
		PDGBMLJEJKG = value;
		DDNEBDBABCM();
	}

	public DetailedDamages NDOAKHGPOHL()
	{
		return IKPAEKHOJLA;
	}

	public int EJJIGHLCKEN()
	{
		return KLLMOEACGLF;
	}

	public float DNOILFCGCGD()
	{
		return HNILMKEAMAE;
	}

	public void set_HitEffectScale(float value)
	{
		HNILMKEAMAE = value;
	}

	public float JKEKBCJHANF()
	{
		return DIKMCKLIEBK;
	}

	public void set_AdditionalDamageValue(float value)
	{
		DIKMCKLIEBK = value;
	}

	public bool NMPHACPBHKO()
	{
		return PEACCBDCNCN;
	}

	public void KKLMIAFFKNE(bool value)
	{
		PEACCBDCNCN = value;
	}

	public void set_color(Color value)
	{
		_perkColor = value;
		CapsuleRender.set_color(value);
		_MeshRender.set_Color(value);
	}

	public Color GetPerkColor()
	{
		return _perkColor;
	}

	public void SetPerkSlowFactor(int value)
	{
		_perkSlowFactor = Mathf.Max(1, value);
		_perkSlowFrame = 0;
	}

	public void SetPerkCollisionDisabled(bool value)
	{
		_perkCollisionDisabled = value;
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public void IMFOFFFLGOM()
	{
		if (BNFCCKBIIDB != null)
		{
			BNFCCKBIIDB.RemoveAllEventListener();
		}
		if (_UnityObject != null)
		{
			_UnityObject.SetActive(false);
			Object.Destroy(_UnityObject);
		}
		_ModelObject.Clear();
		Clear();
		_ModelConditions.Reset();
		_ModelConditions = null;
	}

	public void PJNFHNFLNNO()
	{
	}

	public static Vector3f MHFFCMKNIKM(Model LHBNIMGFKIB, Model AAOIAEJJINO)
	{
		return ModelObject.MHFFCMKNIKM(LHBNIMGFKIB._ModelObject.HOFFDCFEBGA(), AAOIAEJJINO._ModelObject.HOFFDCFEBGA());
	}

	public static Vector3f MHFFCMKNIKM(ModelObject LHBNIMGFKIB, ModelObject AAOIAEJJINO)
	{
		return ModelObject.MHFFCMKNIKM(LHBNIMGFKIB.HOFFDCFEBGA(), AAOIAEJJINO.HOFFDCFEBGA());
	}

	public static float GetDistanceModels(Model LHBNIMGFKIB, Model AAOIAEJJINO)
	{
		return Vector3f.Distance(LHBNIMGFKIB.PLBNCDCFPML(), AAOIAEJJINO.PLBNCDCFPML());
	}

	public void ChangeSpeed(float ELDDBMFEFIP)
	{
		_Physics.ChangeSpeed(ELDDBMFEFIP);
	}

	public void DGNDJBDKNAI()
	{
		List<ModelEdge> list = _Animation.CPNOFKIMMCK();
		List<ModelEdge> list2 = _ModelObject.ODDEMLAODPM();
		foreach (ModelEdge item in list)
		{
			item.AGMHEHLBFCG();
		}
		foreach (ModelEdge item2 in list2)
		{
			item2.AGMHEHLBFCG();
		}
	}

	private void MNHAGALCNFB(List<Vector3f> KPLANIHPMED, bool OOPJHIPPCMD = false)
	{
		if (OOPJHIPPCMD)
		{
			int num = KFCNPADAMHA();
			if (num == -1)
			{
				int count = KPLANIHPMED.Count;
				List<Vector3f> list = new List<Vector3f>(count);
				int i = 0;
				for (int num2 = count; i < num2; i++)
				{
					list.Add(new Vector3f(0f - KPLANIHPMED[i].GILCBJJPKBK(), KPLANIHPMED[i].OBIMBNIBEFG(), KPLANIHPMED[i].KMFEKANLCFO()));
				}
				_ModelObject.MNHAGALCNFB(list);
			}
			else
			{
				_ModelObject.MNHAGALCNFB(KPLANIHPMED);
			}
		}
		else
		{
			_ModelObject.MNHAGALCNFB(KPLANIHPMED);
		}
	}

	public void MNHAGALCNFB(List<Vector3f> KPLANIHPMED, int AOJJBKLCHJO, ModelNode AECCPADGGPG)
	{
		if (AOJJBKLCHJO == -1)
		{
			int count = KPLANIHPMED.Count;
			List<Vector3f> list = new List<Vector3f>(count);
			int i = 0;
			for (int num = count; i < num; i++)
			{
				list.Add(new Vector3f(0f - KPLANIHPMED[i].GILCBJJPKBK(), KPLANIHPMED[i].OBIMBNIBEFG(), KPLANIHPMED[i].KMFEKANLCFO()));
			}
			_ModelObject.MNHAGALCNFB(list, AECCPADGGPG);
		}
		else
		{
			_ModelObject.MNHAGALCNFB(KPLANIHPMED, AECCPADGGPG);
		}
	}

	public void Init()
	{
		POCBCFMBKLO = false;
		set_IsShock(false);
		NKNAANIBJPK = false;
		_IsShock = false;
		FLKMDFDEJPP = true;
		HIHFKKGBGPC();
		FCPIDGIFNKE(KMMJCHDKBDO.MNPAALCFAKL);
		OHAMEHHMEAL.Clear();
		GCIMAADHICB(KMMJCHDKBDO);
		MBOCCLOLEFH(KMMJCHDKBDO);
		CEOOLFLLIMC.IDIHFINEDMI(OHAMEHHMEAL);
		NCGEHCHIBBH.HGPNHBMHIKH(NMILPLHGCMA);
		SetCurrentNode();
		_Animation.Init();
		KDAHHIMLJGG.KJDFJPBIGJC = this;
		SetModelPosition(KMMJCHDKBDO.JJCKADKCDIF);
	}

	public void MKAEDALPGDI()
	{
		Reset();
		SetModelPosition(KMMJCHDKBDO.JJCKADKCDIF);
	}

	public void Reset()
	{
		_perkSlowFactor = 1;
		_perkSlowFrame = 0;
		_perkCollisionDisabled = false;
		BIMGIFDAIGD();
		EEDJEDBMIMI(ODLJHBDMEIJ, false);
		ODLJHBDMEIJ = null;
		FJGNHALJJFF = -1;
		NIKPBGPPFEP = false;
		KICHLMBENOL = false;
		FJKIGPFIEDN = 0f;
		OKDDOLCHDCM = 0;
		KLLMOEACGLF = 0;
		PACHBHGEIGN = 0;
		LDLLJHEDCPD = string.Empty;
		CDBOONBLDBK = 0f;
		JCEOKJKKMCC = 0f;
		_Statistics.Reset();
		_ModelObject.Reset();
		_ModelConditions.Reset();
		if (_Animation != null)
		{
			_Animation.Reset();
		}
	}

	public void HIHFKKGBGPC()
	{
	}

	public void Render()
	{
		if (_perkSlowFactor > 1)
		{
			_perkSlowFrame = (_perkSlowFrame + 1) % _perkSlowFactor;
			if (_perkSlowFrame != 0)
				return;
		}
		if (RenderStrikeDelay())
		{
			AAJKEBAIJAP.Clear();
			PJIHDNFHEGA = false;
		}
		else if (RenderAnimationDelay())
		{
			PJIHDNFHEGA = false;
		}
		FFBNDGBFEKE();
		GBGJIOLEEJK();
		DIIIIDDKHNG();
		if (JGAOCNLLDFG())
		{
			_Animation.Render();
			if (BCKKCJONNHG())
			{
				FEHOHLMIEBP.Render();
			}
			if (!_Animation.NMEEPBDJHMG() && NLHFJIEHKMM())
			{
				_Animation.RenderPhysics();
			}
		}
		UpdateCombo();
		_Physics.Render();
		_ModelObject.NDDMFBCIHPC();
		_ModelObject.JANOFOIKIAP();
		_Statistics.Draw();
		DJLLAOJCOIM();
		CallEvent(4, KDAHHIMLJGG);
		GPMGEJKBAJG();
	}

	public bool RenderCollision(bool FHPKEJMDFLK)
	{
		if (!_perkCollisionDisabled && HIPJNBEFGHN() && !KMMJCHDKBDO.BHHLEBHLBLH && _Animation.NNMAFFCCMHC() != null && !NLHFJIEHKMM())
		{
			return CheckCollision(EGGEACCDAEK(), FHPKEJMDFLK);
		}
		return false;
	}

	public void RenderAi()
	{
		if ((!FGKAFKFBFEM() && !AiData.get_BothBotEnabled()) || JMHJDHLBHLK != 2)
		{
			return;
		}
		Tactic hBFMBOHLKPJ = KMMJCHDKBDO.HBFMBOHLKPJ;
		if (hBFMBOHLKPJ != null)
		{
			if (hBFMBOHLKPJ.get_Type() == Tactic.GKJKJFJALCA.TacticRandom)
			{
				JAOKKOPAKKL();
			}
			else if (hBFMBOHLKPJ.get_Type() == Tactic.GKJKJFJALCA.TacticTabular)
			{
				FNLFMFNNOIF();
			}
		}
	}

	public void FNLFMFNNOIF()
	{
		Model fNKFIMEDNLP = EGGEACCDAEK();
		ModelConditions oADECAPBOND = _ModelConditions;
		InfoAnimation pJAHIOELGGD = null;
		if (oADECAPBOND == null)
		{
			Debug.LogError("modelConditions is Empty");
		}
		else
		{
			pJAHIOELGGD = HJOGNGDMAKJ.Render(fNKFIMEDNLP, NPKHMEHKFMM);
		}
		if (pJAHIOELGGD != null)
		{
			ConditionKeys bHDEBDIHDFM = pJAHIOELGGD.ILBCHANCOBP();
			if (bHDEBDIHDFM == null)
			{
				LLLOJBFMONN.Error("tactics: conditionKeys is null for {0}", pJAHIOELGGD.Name);
				bHDEBDIHDFM = pJAHIOELGGD.ILBCHANCOBP();
			}
			else
			{
				KeyData fONEJOKEIEN = bHDEBDIHDFM.FONEJOKEIEN;
				_Animation.GJGDKFAAGOD = pJAHIOELGGD;
				fONEJOKEIEN.IsInverted = true;
				PlayAnimation(fONEJOKEIEN);
			}
		}
		else
		{
			FEHOHLMIEBP.Reset();
		}
	}

	public void JAOKKOPAKKL(bool DFILNPNDHHP = true)
	{
		if (DFILNPNDHHP)
		{
			if (APOHBENDEKO > 0)
			{
				APOHBENDEKO--;
				return;
			}
			if (APOHBENDEKO == 0)
			{
				GDGHBKAENHK = true;
				APOHBENDEKO = -1;
			}
			if (!GDGHBKAENHK)
			{
				return;
			}
			GDGHBKAENHK = false;
		}
		KDAHHIMLJGG.Data = null;
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().GPABGFNBALE(KDAHHIMLJGG);
		}
	}

	public bool CheckCollision(Model HFGPAELCNMF = null, bool FHPKEJMDFLK = false)
	{
		bool result = false;
		IntervalAttack hFIIPNLCIEE = _Animation.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK) as IntervalAttack;
		if (hFIIPNLCIEE != null)
		{
			if (HFGPAELCNMF == null)
			{
				HFGPAELCNMF = EGGEACCDAEK();
			}
			if ((HFGPAELCNMF.OCPMJKIEPIG().HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE) == null || (hFIIPNLCIEE.MOILKOLCNBP() && hFIIPNLCIEE.DNPLIFOABPB().Count == 0) || (hFIIPNLCIEE.MOILKOLCNBP() && HFGPAELCNMF.OCPMJKIEPIG().CheckIntervals(hFIIPNLCIEE.DNPLIFOABPB()))) && _Collision.Render(HFGPAELCNMF._ModelObject, _Animation.CPNOFKIMMCK(), hFIIPNLCIEE))
			{
				if (!FHPKEJMDFLK)
				{
					StrikeModel(HFGPAELCNMF, hFIIPNLCIEE);
				}
				result = true;
			}
		}
		return result;
	}

	public void PKFPDKFLKBL()
	{
	}

	public void PressAnyKey(FightCID JDDDODIJODK)
	{
		bool inputAccepted = (JDDDODIJODK != FightCID.MagicButton || MJEJFBHOJKB != 0 || MJEJFBHOJKB != 0 || GameUtils.GLHMHHIADMK) && (JDDDODIJODK != FightCID.MissileButton || !MDFEHKBOHEL.MLDHFPCCCOP || MDFEHKBOHEL.DPMNMLHCJLK == MDFEHKBOHEL.MDOBBLKHOHI) && (JDDDODIJODK != FightCID.Kick || !MDFEHKBOHEL.MPIOLPLLFEM || MDFEHKBOHEL.CPKHGNDBKFL == MDFEHKBOHEL.AAIDLAFJECE) && (JDDDODIJODK != FightCID.Punch || !MDFEHKBOHEL.CNGALGBKFOK || MDFEHKBOHEL.KIKLFDLLDDP == MDFEHKBOHEL.PANKKFJFINL) && (JDDDODIJODK != FightCID.RaidChargeButton || !MDFEHKBOHEL.FDHAJDFJBCF || MDFEHKBOHEL.IFMNJHFPDIC == MDFEHKBOHEL.GBEADNMMOID) && HCPHOJKFIDM;
		if (JDDDODIJODK == FightCID.MagicButton)
		{
			InfoAnimation current = FHBLLPCEAHG();
			Debug.Log("[MagicTrace] request actor=" + get_Name() +
				" player=" + KMMJCHDKBDO.IsPlayer +
				" accepted=" + inputAccepted +
				" charge=" + MJEJFBHOJKB +
				" currentAnimation=" + ((current != null) ? current.Name : "<none>") +
				" items=" + GetMagicTraceItems());
		}
		if (inputAccepted)
		{
			FEHOHLMIEBP.OnPressAnyKey((int)JDDDODIJODK);
		}
	}

	public void ReleaseAnyKey(FightCID KJPGKHJNOMC)
	{
		if (HCPHOJKFIDM)
		{
			FEHOHLMIEBP.OnReleaseAnyKey((int)KJPGKHJNOMC);
		}
	}

	public Vector3f PLBNCDCFPML()
	{
		return _ModelObject.PLBNCDCFPML();
	}

	public void SetModelPosition(Vector3f MGMMDGFPBLP)
	{
		_ModelObject.SetModelPosition(MGMMDGFPBLP);
		_Physics.IterativeProcess();
		_ModelObject.NDDMFBCIHPC();
		_ModelObject.JANOFOIKIAP();
		_ModelObject.OBFONONKIAN();
	}

	public void ShiftModelPosition(Vector3f OPNPKNEOALJ, bool LFFNFGOECLB = false)
	{
		_ModelObject.JBHFODLCNIA(OPNPKNEOALJ);
		_Physics.IterativeProcess();
		_ModelObject.NDDMFBCIHPC();
		_ModelObject.JANOFOIKIAP();
		_ModelObject.OBFONONKIAN();
		if (LFFNFGOECLB)
		{
			_Animation.ShiftSequence(OPNPKNEOALJ.GILCBJJPKBK(), OPNPKNEOALJ.OBIMBNIBEFG(), OPNPKNEOALJ.KMFEKANLCFO());
			_Animation.ShiftBuffer(OPNPKNEOALJ);
		}
	}

	public void GMFOJPHEHHI(ModelParameters MPBIEICCBMM)
	{
		List<ItemInfo> oJIAKDDCGLB = MPBIEICCBMM.OJIAKDDCGLB;
		List<PerkInfoItem> mAFPBEFKNGE = MPBIEICCBMM.JBIOECDAAKP();
		List<PerkInfoItem> cFKCGBEONAM = null;
		if (EGGEACCDAEK() != null && EGGEACCDAEK().KMMJCHDKBDO != null)
		{
			cFKCGBEONAM = EGGEACCDAEK().KMMJCHDKBDO.JBIOECDAAKP();
		}
		GMFOJPHEHHI(oJIAKDDCGLB, mAFPBEFKNGE, cFKCGBEONAM);
	}

	public void GMFOJPHEHHI(List<ItemInfo> HELFDCAIJNE, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		OHAMEHHMEAL.Clear();
		AnimationData.AKJLPGMEFFD(OHAMEHHMEAL, HELFDCAIJNE, false, KMMJCHDKBDO.DANNKMJOOOH, KMMJCHDKBDO.IBBALIJOJMC, MAFPBEFKNGE, CFKCGBEONAM);
		CEOOLFLLIMC.JIKDAIELFBF();
		CEOOLFLLIMC.IDIHFINEDMI(OHAMEHHMEAL);
		SetCurrentNode();
		NIKPBGPPFEP = true;
		KMMJCHDKBDO.EAJHPCJJCDI = true;
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().BPLPMLGJENF(this);
		}
	}

	public bool FGKAFKFBFEM()
	{
		return KMMJCHDKBDO.EEGMBGBLLIF;
	}

	public bool BCKKCJONNHG()
	{
		return KMMJCHDKBDO.ABAPAIEBNGK;
	}

	public bool HPCNJFPOONE()
	{
		return KMMJCHDKBDO.LKKFNMBCCDB.Type == "Dummy";
	}

	public bool JGAOCNLLDFG()
	{
		return KMMJCHDKBDO.HKJFJHBHMND;
	}

	public bool HIPJNBEFGHN()
	{
		return _Enemies.Count > 0;
	}

	public bool EPCNJLEHJCB()
	{
		return KMMJCHDKBDO.IsPlayer;
	}

	public virtual bool KIAFPPHPEEK()
	{
		return false;
	}

	public void BIMGIFDAIGD()
	{
		JLDBGHLBJEL.Clear();
	}

	public void SetNearestEnemy()
	{
		PNNMOKIBOPP = ((_Enemies.Count <= 0) ? null : _Enemies[0]);
		if (PNNMOKIBOPP != null && PNNMOKIBOPP.KIAFPPHPEEK())
		{
			Debug.LogError("Model::setNearestEnemy - enemy is weapon, fix code bug");
		}
		if (KDAHHIMLJGG != null)
		{
			KDAHHIMLJGG.GAIBPAGPEGK = PNNMOKIBOPP;
		}
	}

	public void CNIAJPBJHIM(Model ACENLMONNPA)
	{
		_Enemies.Remove(ACENLMONNPA);
	}

	public Model FindNearestEnemy()
	{
		Model result = null;
		float num = float.MaxValue;
		float num2 = _ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK();
		foreach (Model item in _Enemies)
		{
			float num3 = num2 - item._ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK();
			if (num3 < num)
			{
				num = num3;
				result = item;
			}
		}
		return result;
	}

	public void SetSign()
	{
		if (CHOJGIFFEMB == 0)
		{
			Model fGCODGKLHED = EGGEACCDAEK();
			if (fGCODGKLHED != null)
			{
				NFOOGKCGFAB = ((!(_ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK() > fGCODGKLHED._ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK())) ? 1 : (-1));
			}
			else
			{
				NFOOGKCGFAB = 1;
			}
		}
		else
		{
			NFOOGKCGFAB = CHOJGIFFEMB;
		}
	}

	public int KFCNPADAMHA()
	{
		return (_Animation == null) ? 1 : _Animation.KFCNPADAMHA();
	}

	public void SetDistanceToEnemy(Model HFGPAELCNMF)
	{
		float num = _ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK() - HFGPAELCNMF._ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK();
		PJLKIEDMDOG = ((!(num < 0f)) ? num : (0f - num));
	}

	public void SetDistanceToNearestWall()
	{
		float num = _ModelObject.CJELIBMCCMA().ICLEOFDKDIF().GILCBJJPKBK();
		DOEPGPAMEEA = ((KFCNPADAMHA() != -1) ? (num - LBOLAOBGDEH.EDCHBILGFLD) : (LBOLAOBGDEH.NNCHJCLKHHA - num));
	}

	private void FCPIDGIFNKE(List<string> NIKHAICFGNM)
	{
		Clear();
		_ModelObject = new ModelObject();
		_ModelObject.set_Model(this);
		ModelLoader.Load(_ModelObject, NIKHAICFGNM);
		_Physics = new ModelPhysics(_ModelObject);
		_Strike = new ModelStrike(_ModelObject);
		_Animation = new ModelAnimation(_ModelObject);
		_Collision = new ModelCollision(_ModelObject);
		HJOGNGDMAKJ = new ModelAi(_Animation, _Physics, (KMMJCHDKBDO.JGMLKIPCFII == null) ? string.Empty : KMMJCHDKBDO.JGMLKIPCFII.MDPPNGIEJGD, KMMJCHDKBDO);
		HJOGNGDMAKJ.set_Model(this);
		KDAHHIMLJGG = new EventModel();
		FEHOHLMIEBP.AddEventListener(0, LONHMEJHOOO);
		FEHOHLMIEBP.AddEventListener(1, EIEDDGHLHDL);
		_Animation.AddEventListener(0, PAMICDLAMHC);
		_Animation.AddEventListener(1, OnStopAnimation);
		_Animation.AddEventListener(2, OnStartInterval);
		_Animation.AddEventListener(3, OnStopInteval);
		_Animation.AddEventListener(4, NBIPOHOPFGA);
	}

	public void SetWalls(float NGHJOCKCCHH, float KCNCLAANGGJ, int CDNFFEFGLKN, int JNKHDFNCOGK)
	{
		LBOLAOBGDEH.EDCHBILGFLD = NGHJOCKCCHH;
		LBOLAOBGDEH.NNCHJCLKHHA = KCNCLAANGGJ;
		_Physics.SetWallShift(LBOLAOBGDEH.EDCHBILGFLD, LBOLAOBGDEH.NNCHJCLKHHA);
		_Animation.SetAligns(NGHJOCKCCHH, KCNCLAANGGJ, CDNFFEFGLKN, JNKHDFNCOGK);
	}

	public void SetFixedSign(bool value, int AOJJBKLCHJO = 1)
	{
		if (value)
		{
			CHOJGIFFEMB = AOJJBKLCHJO;
		}
		else
		{
			CHOJGIFFEMB = 0;
		}
	}

	public void CJNGMIMHFCC(Model HFGPAELCNMF)
	{
		_Animation.CBKLDPIBGHD((BFFLLGHDPEB == null) ? null : BFFLLGHDPEB._Animation);
		if (HFGPAELCNMF == null)
		{
			return;
		}
		_Enemies.Add(HFGPAELCNMF);
		List<WeaponModel> list = HFGPAELCNMF.KGGIDBLBMDJ();
		foreach (WeaponModel item in list)
		{
			_Enemies.Add(item);
		}
		_Animation.NFEGCGJIICB(HFGPAELCNMF._Animation);
		if (HFGPAELCNMF.KMMJCHDKBDO.JGMLKIPCFII != null)
		{
			HJOGNGDMAKJ.SetWeaponEnemy(HFGPAELCNMF.KMMJCHDKBDO.JGMLKIPCFII.MDPPNGIEJGD);
		}
		SetNearestEnemy();
	}

	public void ACJBEOMHFOO()
	{
		FLBDBIHFJAI(0);
		OGHAMAGPFLF(GameUtils.DILKHIFCCGD.HCJBIAGKIGI(this));
		BFBFNKMLOJA();
	}

	public void JJHLOKBPBLD(float FOIPKLDNGDL)
	{
		if (MJEJFBHOJKB == 0)
		{
			OGHAMAGPFLF(NJDNNFJAFBG + FOIPKLDNGDL);
		}
	}

	public void CIFKBIPDCHK(string DIGKODNINPB)
	{
		HJOGNGDMAKJ.ChangeTactic(DIGKODNINPB);
	}

	public void CIFKBIPDCHK(Tactic KHAKOJKLDHO)
	{
		HJOGNGDMAKJ.ChangeTactic(KHAKOJKLDHO);
	}

	public void LFNOLPFIBKC(string DIGKODNINPB)
	{
		Tactic kHAKOJKLDHO = AiData.GetTacticByName(DIGKODNINPB);
		LFNOLPFIBKC(kHAKOJKLDHO);
	}

	public void LFNOLPFIBKC(Tactic KHAKOJKLDHO)
	{
		if (KHAKOJKLDHO != null)
		{
			KMMJCHDKBDO.HBFMBOHLKPJ = KHAKOJKLDHO;
			CIFKBIPDCHK(KHAKOJKLDHO);
		}
	}

	public void IPGBFKOCOCK(int FOIPKLDNGDL)
	{
		if (FOIPKLDNGDL < 0)
		{
			DJOKGDICHAJ++;
		}
		FLBDBIHFJAI(MJEJFBHOJKB + FOIPKLDNGDL);
	}

	public void BFBFNKMLOJA()
	{
		Model bFFLLGHDPEB = BFFLLGHDPEB;
		if (bFFLLGHDPEB == null)
		{
			float num = NJDNNFJAFBG;
			if (1f <= num)
			{
				IPGBFKOCOCK(1);
				num = 0f;
				OGHAMAGPFLF(num);
			}
			if (1 < MJEJFBHOJKB)
			{
				FLBDBIHFJAI(1);
			}
			if (EPCNJLEHJCB())
			{
				if (MJEJFBHOJKB == 0)
				{
					EventActBtnSettings eHCLMBADLKH = new EventActBtnSettings(FightCID.MagicButton, num);
					CallEvent(12, eHCLMBADLKH);
				}
				else
				{
					float aIEGFACLFKE = 1f;
					EventActBtnSettings eHCLMBADLKH2 = new EventActBtnSettings(FightCID.MagicButton, aIEGFACLFKE);
					CallEvent(12, eHCLMBADLKH2);
				}
			}
		}
		else
		{
			bFFLLGHDPEB.BFBFNKMLOJA();
		}
	}

	public int GLEKCPCMINJ()
	{
		if (KMMJCHDKBDO.LGHMILECPLA != null && KMMJCHDKBDO.LGHMILECPLA.MDPPNGIEJGD == "NoRanged")
		{
			return 1;
		}
		return -1;
	}

	public InfoAnimation EJOGECPBJCE()
	{
		return AAJKEBAIJAP.FGICHADOEHF;
	}

	public virtual bool PlayAnimation(InfoAnimation CMGIPKIPIPA, int AOJJBKLCHJO = 0, bool HHJGACBCGBP = false, int BADKABIKMBD = -1)
	{
		bool traceMagicAnimation = CMGIPKIPIPA != null &&
			CMGIPKIPIPA.Name != null && CMGIPKIPIPA.Name.IndexOf("Magic") >= 0;
		if (JGAOCNLLDFG() && !_Physics.EGNOOKHNFLK())
		{
			if (AOJJBKLCHJO == 0)
			{
				AOJJBKLCHJO = KFCNPADAMHA();
			}
			_Collision.ResetLastStrike();
			bool started = _Animation.PlayInfo(CMGIPKIPIPA, AOJJBKLCHJO, !CMGIPKIPIPA.JEADCBJMEGC, HHJGACBCGBP, BADKABIKMBD);
			if (traceMagicAnimation)
			{
				Debug.Log("[MagicTrace] animation-select actor=" + get_Name() +
					" animation=" + CMGIPKIPIPA.Name +
					" animationFile=" + CMGIPKIPIPA.FileName +
					" started=" + started +
					" direction=" + AOJJBKLCHJO +
					" items=" + GetMagicTraceItems());
			}
			return started;
		}
		if (traceMagicAnimation)
		{
			Debug.Log("[MagicTrace] animation-select actor=" + get_Name() +
				" animation=" + CMGIPKIPIPA.Name +
				" animationFile=" + CMGIPKIPIPA.FileName +
				" started=False blockedByModelState=True" +
				" items=" + GetMagicTraceItems());
		}
		return false;
	}

	public bool PlayAnimation(string name, int AOJJBKLCHJO = 0)
	{
		InfoAnimation cMGIPKIPIPA = null;
		foreach (InfoAnimation item in OHAMEHHMEAL)
		{
			if (item.Name == name)
			{
				cMGIPKIPIPA = item;
				break;
			}
		}
		if (cMGIPKIPIPA == null)
		{
			Debug.LogWarning("Animation '" + name + "' is unavailable on model '" + get_Name() + "'");
			return false;
		}
		return PlayAnimation(cMGIPKIPIPA, AOJJBKLCHJO);
	}

	public bool PlayAnimation(KeyData AHBBDGGGEIE)
	{
		FEHOHLMIEBP.Reset();
		FEHOHLMIEBP.IPGLLIAHDPE(AHBBDGGGEIE);
		FEHOHLMIEBP.CallKeyPressed();
		return true;
	}

	public void PlayAnimationDelay(InfoAnimation CMGIPKIPIPA, int AOJJBKLCHJO = 0, bool HHJGACBCGBP = false, int BADKABIKMBD = -1)
	{
		AAJKEBAIJAP.FGICHADOEHF = CMGIPKIPIPA;
		AAJKEBAIJAP.GFHOIKMBNHF = AOJJBKLCHJO;
		AAJKEBAIJAP.IsFrameShift = HHJGACBCGBP;
		AAJKEBAIJAP.FrameShift = BADKABIKMBD;
		Vector3f eMAFACPEPDK = new Vector3f(CMGIPKIPIPA.LBJFGCFGMDI());
		if (!eMAFACPEPDK.IsEqual(0f, 0f, 0f))
		{
			eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() * (float)AOJJBKLCHJO);
			_Animation.MoveByVelocity(eMAFACPEPDK);
		}
	}

	public void BACNPEIJLPE()
	{
		CEAOMPLGBDG.FGICHADOEHF = null;
		CEAOMPLGBDG.Names = null;
		CEAOMPLGBDG.IsStrikeResult = false;
	}

	public bool IBIDGACDJNF()
	{
		return !AAJKEBAIJAP.KLNLNKBIDGD();
	}

	public bool DCNFLKPDIFJ()
	{
		return CEAOMPLGBDG.FGICHADOEHF != null;
	}

	public void IFDGGKPAHMC(InfoAnimation CMGIPKIPIPA, bool CGHPLEOFEFM)
	{
		CEAOMPLGBDG.FGICHADOEHF = CMGIPKIPIPA;
		CEAOMPLGBDG.Names = CMGIPKIPIPA.FOLOOGCLPNE();
		CEAOMPLGBDG.IsStrikeResult = CGHPLEOFEFM;
	}

	public bool RenderAnimationDelay()
	{
		if (IBIDGACDJNF())
		{
			if (_Physics.EGNOOKHNFLK())
			{
				_Physics.Stop();
			}
			if (PlayAnimation(AAJKEBAIJAP.FGICHADOEHF, AAJKEBAIJAP.GFHOIKMBNHF, AAJKEBAIJAP.IsFrameShift, AAJKEBAIJAP.FrameShift))
			{
				AAJKEBAIJAP.Clear();
			}
			return true;
		}
		return false;
	}

	public bool RenderStrikeDelay()
	{
		if (DCNFLKPDIFJ())
		{
			StrikePhysics(CEAOMPLGBDG.Names, CEAOMPLGBDG.IsStrikeResult);
			_Animation.Reset();
			_Animation.DBDJHIHLCFD(CEAOMPLGBDG.FGICHADOEHF);
			BACNPEIJLPE();
			return true;
		}
		return false;
	}

	public void NFADDANANJL()
	{
		InfoAnimation dBOLBEOCEME = FHBLLPCEAHG();
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED != null)
		{
			Model fGCODGKLHED2 = fGCODGKLHED.BDJBNOPNCNB();
			fGCODGKLHED2.DKFGOHCNIKL.PIOIIIMCFMJ(true, dBOLBEOCEME);
		}
		Model fGCODGKLHED3 = BDJBNOPNCNB();
		fGCODGKLHED3.DKFGOHCNIKL.PIOIIIMCFMJ(false, dBOLBEOCEME);
	}

	public void LONHMEJHOOO(object data)
	{
		KDAHHIMLJGG.Data = null;
		CallEvent(10, KDAHHIMLJGG);
	}

	public void EIEDDGHLHDL(object data)
	{
		KDAHHIMLJGG.Data = null;
		CallEvent(11, KDAHHIMLJGG);
	}

	public void PAMICDLAMHC(object EMBBNNBFODN)
	{
		_Collision.ResetInterval();
		KDAHHIMLJGG.Data = EMBBNNBFODN;
		CallEvent(2, KDAHHIMLJGG);
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED != null && !KIAFPPHPEEK())
		{
			fGCODGKLHED.FDDIAFGKODA(_Animation.NNMAFFCCMHC());
			fGCODGKLHED.BHAFOEICJPE(0);
		}
	}

	public void OnStopAnimation(object EMBBNNBFODN)
	{
		KDAHHIMLJGG.Data = EMBBNNBFODN;
		InfoAnimation pJAHIOELGGD = EMBBNNBFODN as InfoAnimation;
		if (pJAHIOELGGD.HECHJGBMHIC)
		{
			PKFJFFGDOLB = pJAHIOELGGD.HECHJGBMHIC;
		}
		NFADDANANJL();
		CallEvent(3, KDAHHIMLJGG);
	}

	public void OnStartInterval(object EMBBNNBFODN)
	{
		KDAHHIMLJGG.Data = EMBBNNBFODN;
		CallEvent(0, KDAHHIMLJGG);
	}

	public void OnStopInteval(object EMBBNNBFODN)
	{
		KDAHHIMLJGG.Data = EMBBNNBFODN;
		CallEvent(1, KDAHHIMLJGG);
		IntervalAnimation mNOIEOBBCMI = EMBBNNBFODN as IntervalAnimation;
		if (GameUtils.BJACOFCAHPD.IsIntervalByName(mNOIEOBBCMI.Name))
		{
			InfoAnimation pJAHIOELGGD = FHBLLPCEAHG();
			if (pJAHIOELGGD != null)
			{
				APOHBENDEKO = GameUtils.BJACOFCAHPD.GetDelayByName(pJAHIOELGGD.FOLOOGCLPNE());
			}
		}
		if (mNOIEOBBCMI.Name == "Uninterrupt")
		{
			InfoAnimation dBOLBEOCEME = _Animation.NNMAFFCCMHC();
			Model fGCODGKLHED = EGGEACCDAEK();
			if (fGCODGKLHED != null)
			{
				Model fGCODGKLHED2 = fGCODGKLHED.BDJBNOPNCNB();
				fGCODGKLHED2.DKFGOHCNIKL.NFPKBOGGPBA(true, dBOLBEOCEME);
			}
			Model fGCODGKLHED3 = BDJBNOPNCNB();
			fGCODGKLHED3.DKFGOHCNIKL.NFPKBOGGPBA(false, dBOLBEOCEME);
		}
	}

	public void StrikeModel(Model HFGPAELCNMF, IntervalAttack CHCGJBLDPML)
	{
		ModelCollision.StrikeHit dEJLIPMOIHC = _Collision.Strike;
		Vector3f eMAFACPEPDK = new Vector3f(CHCGJBLDPML.GIFLLJFAJCO());
		eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() * (float)_Animation.KFCNPADAMHA());
		eMAFACPEPDK.JPFALPBDBAP(eMAFACPEPDK.GILCBJJPKBK() * ODCOKJKEDOJ.GILCBJJPKBK());
		eMAFACPEPDK.IBNFLLGPOLD(eMAFACPEPDK.OBIMBNIBEFG() * ODCOKJKEDOJ.OBIMBNIBEFG());
		eMAFACPEPDK.set_Z(eMAFACPEPDK.KMFEKANLCFO() * ODCOKJKEDOJ.KMFEKANLCFO());
		HIGBAPPOOKJ = CHCGJBLDPML.KCBHAMHLGBC();
		HFGPAELCNMF.Strike(dEJLIPMOIHC.CMGLHHEJEBN, dEJLIPMOIHC.ALIHGFIJEDN, dEJLIPMOIHC.EGCPOJIDHKK(), dEJLIPMOIHC.OJOMOLOIAOJ(), this, eMAFACPEPDK);
	}

	public void Strike(ModelEdge GCFJNDJBBOI, ModelEdge AOBJMMHGMPG, Vector3f NAAPALOFBCI, Vector3f GKCGDDBMHNJ, Model HFGPAELCNMF, Vector3f KKIKIDNALOL)
	{
		KLLMOEACGLF++;
		IntervalAttack hFIIPNLCIEE = HFGPAELCNMF._Animation.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK) as IntervalAttack;
		KDAHHIMLJGG.Data = hFIIPNLCIEE;
		KDAHHIMLJGG.ConditionName = hFIIPNLCIEE.GetReactionName(HFGPAELCNMF.NODAINEDAKJ());
		if (hFIIPNLCIEE.NPHDDMAIGKN())
		{
			if (hFIIPNLCIEE.KBENFIOADCG().Count == 0)
			{
				RemoveInterval(IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK);
			}
			else
			{
				RemoveIntervals(hFIIPNLCIEE.KBENFIOADCG());
			}
		}
		HJOGNGDMAKJ.OnGetHit();
		if (HFGPAELCNMF != null)
		{
			HFGPAELCNMF.HJOGNGDMAKJ.OnHitEnemy();
		}
		KMMJCHDKBDO.DKAHKGBFJMG = false;
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().FKGAAFNNCNE = GHHCDAFIKJE;
		}
		GHHCDAFIKJE.ProcedPerks.Clear();
		GHHCDAFIKJE.KJDFJPBIGJC = this;
		GHHCDAFIKJE.GAIBPAGPEGK = HFGPAELCNMF;
		GHHCDAFIKJE.Target = ((!KMMJCHDKBDO.IsPlayer) ? 1 : 0);
		GHHCDAFIKJE.CMGLHHEJEBN = GCFJNDJBBOI;
		GHHCDAFIKJE.IIIDIKABLOJ.Set(KKIKIDNALOL);
		GHHCDAFIKJE.ALIHGFIJEDN = AOBJMMHGMPG;
		GHHCDAFIKJE.PBPDKJNKFCJ = HFGPAELCNMF._Animation.NNMAFFCCMHC();
		GHHCDAFIKJE.Point = NAAPALOFBCI;
		GHHCDAFIKJE.AOFLADELDFB = GKCGDDBMHNJ;
		GHHCDAFIKJE.HMOLHIEDINK = hFIIPNLCIEE.GHGGNMBCMNM();
		GHHCDAFIKJE.DFOHNJEBDED = AMGHOKDANGN();
		GHHCDAFIKJE.DefenceAttribute = GetDefenseAttribute(hFIIPNLCIEE, GHHCDAFIKJE.DFOHNJEBDED, GCFJNDJBBOI);
		if (!GHHCDAFIKJE.DFOHNJEBDED)
		{
			if (!HFGPAELCNMF.FPPKOMOPDJJ())
			{
				AAEFMEJBMLH++;
			}
			HFGPAELCNMF.KDJPMHGEPAF();
		}
		GHHCDAFIKJE.IICJEIHBABC = AAEFMEJBMLH;
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().OnModelPreCrit(KDAHHIMLJGG);
		}
		bool flag = hFIIPNLCIEE.HPLOFLKCLHG();
		GHHCDAFIKJE.DNGKOMPMPCD = !GHHCDAFIKJE.DFOHNJEBDED && !flag && GameUtils.IsProbality(HFLFIKJPGCJ());
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().OnModelPostCrit(KDAHHIMLJGG);
		}
		GHHCDAFIKJE.NPDHOJEHPDM = GetTotalDamage(hFIIPNLCIEE, GHHCDAFIKJE.DFOHNJEBDED, GHHCDAFIKJE.DNGKOMPMPCD, GCFJNDJBBOI);
		GHHCDAFIKJE.EEDJBBOCFNL = KMMJCHDKBDO.ResolveStrikeDamage(
			GHHCDAFIKJE.NPDHOJEHPDM, out GHHCDAFIKJE.HOJPKPDBPEJ);
		string text = "Head";
		string text2 = string.Empty;
		if (!string.IsNullOrEmpty(hFIIPNLCIEE.ELHIBCEADCG()))
		{
			text2 = hFIIPNLCIEE.ELHIBCEADCG();
		}
		else if (GCFJNDJBBOI != null)
		{
			text2 = GCFJNDJBBOI.ELHIBCEADCG();
		}
		GHHCDAFIKJE.JMDIIIFJMFH = text2 == text;
		GHHCDAFIKJE.APCAKCCOMLO = BGOBFLIKMNN(GHHCDAFIKJE, HFGPAELCNMF);
		GHHCDAFIKJE.NIKPBGPPFEP = GHHCDAFIKJE.APCAKCCOMLO;
		GHHCDAFIKJE.LOONMILKCFK = OKDDOLCHDCM == 0;
		PJIHDNFHEGA = true;
		OKDDOLCHDCM++;
		RuleAppliance eJPOJJKKICO = ((!EPCNJLEHJCB()) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		hFIIPNLCIEE.UpdateFactor(eJPOJJKKICO);
		KDAHHIMLJGG.GAIBPAGPEGK = HFGPAELCNMF;
		if (!GHHCDAFIKJE.DFOHNJEBDED)
		{
			KMMJCHDKBDO.ABLMGLAKJBL = false;
		}
		CAJANPOIPFC = GHHCDAFIKJE.DFOHNJEBDED;
		HEJMFGFBLDK = GHHCDAFIKJE.DNGKOMPMPCD;
		KDAHHIMLJGG.Data = hFIIPNLCIEE;
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().OnModelHit(KDAHHIMLJGG);
		}
		KDAHHIMLJGG.GAIBPAGPEGK = EGGEACCDAEK();
		HEEHFLHNPOH(HFGPAELCNMF, GHHCDAFIKJE.PBPDKJNKFCJ, GHHCDAFIKJE.EEDJBBOCFNL, GHHCDAFIKJE);
	}

	public void StrikePhysics(List<string> NIKHAICFGNM, StrikeResult PPIAOBPLGOK)
	{
		_Animation.DeleteAnimation();
		_Physics.Start(NIKHAICFGNM);
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().PIJPBDGHHGE(KDAHHIMLJGG);
		}
		ApplyStrike(PPIAOBPLGOK);
	}

	public void StrikePhysics(List<string> NIKHAICFGNM, bool CGHPLEOFEFM)
	{
		StrikeResult pPIAOBPLGOK = null;
		if (CGHPLEOFEFM)
		{
			pPIAOBPLGOK = GHHCDAFIKJE;
		}
		StrikePhysics(NIKHAICFGNM, pPIAOBPLGOK);
	}

	public void ApplyStrike(StrikeResult PPIAOBPLGOK)
	{
		if (PPIAOBPLGOK != null)
		{
			_Strike.Strike(GHHCDAFIKJE.CMGLHHEJEBN, GHHCDAFIKJE.AOFLADELDFB, GHHCDAFIKJE.IIIDIKABLOJ);
			_Physics.IterativeProcess();
		}
	}

	public void SetCurrentNode()
	{
		foreach (InfoAnimation item in OHAMEHHMEAL)
		{
			if (item.ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI == -1)
			{
				InfoAnimation.DOLCEABGNGA cKBGFODEBAJ = item.ODACDCDONJE.ILOEBFFAEAN.CKBGFODEBAJ;
				if (cKBGFODEBAJ == InfoAnimation.DOLCEABGNGA.ObjectNodes)
				{
					ModelObject oIEODIEHJMH = OEKFONJCEFG(item.ODACDCDONJE.ILOEBFFAEAN.BAFGOANMBMI);
					int num = oIEODIEHJMH.GetNodeIDByName(item.ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK);
					if (num == -1)
					{
						LLLOJBFMONN.GLCKHLCAPIN("'Pivot' node '{0}' not found for '{1}' animation", item.ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK, item.Name);
					}
					item.ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI = num;
					item.ODACDCDONJE.ILOEBFFAEAN.BAHKGNNELBL = oIEODIEHJMH.GetNodeIDByPairName(item.ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI);
				}
			}
			if (item.ODACDCDONJE.ILOEBFFAEAN.JPKDOHPGEBA != -1 && item.ODACDCDONJE.ILOEBFFAEAN.EDBLMNIEKBD != ModelType.KEIDBIOIFGA.MODEL_OTHER)
			{
				continue;
			}
			InfoAnimation.DOLCEABGNGA hHPAGAOGGLP = item.ODACDCDONJE.ILOEBFFAEAN.HHPAGAOGGLP;
			if (hHPAGAOGGLP == InfoAnimation.DOLCEABGNGA.ObjectNodes)
			{
				ModelObject oIEODIEHJMH2 = OEKFONJCEFG(item.ODACDCDONJE.ILOEBFFAEAN.EDBLMNIEKBD);
				if (oIEODIEHJMH2 != null)
				{
					item.ODACDCDONJE.ILOEBFFAEAN.JPKDOHPGEBA = oIEODIEHJMH2.GetNodeIDByName(item.ODACDCDONJE.ILOEBFFAEAN.PMILDGBBLMF);
					item.ODACDCDONJE.ILOEBFFAEAN.KFMGKDOLKGN = oIEODIEHJMH2.GetNodeIDByPairName(item.ODACDCDONJE.ILOEBFFAEAN.JPKDOHPGEBA);
				}
				else
				{
					LLLOJBFMONN.Error("Model::setCurrentNode() m == 0 : {0}", item.Name);
					item.ODACDCDONJE.ILOEBFFAEAN.JPKDOHPGEBA = 0;
				}
			}
		}
	}

	public KeyData ANALKHBJKIO()
	{
		return FEHOHLMIEBP.ANALKHBJKIO();
	}

	public KeyData GetKeyDataBySign(int KJKCHFALFDD)
	{
		return FEHOHLMIEBP.GetKeyDataBySign(KJKCHFALFDD);
	}

	public bool NMEEPBDJHMG()
	{
		return _Animation.NMEEPBDJHMG();
	}

	public bool CDMBCHOJKPH()
	{
		return PKFJFFGDOLB || !KMMJCHDKBDO.HKJFJHBHMND;
	}

	public bool NLHFJIEHKMM()
	{
		return _Physics.EGNOOKHNFLK();
	}

	public List<string> KGHDFCKGAEO()
	{
		return _Physics.IDAEPMLGFLG();
	}

	public float GetDamageBlockCritical(bool KHOKDADOJCG, string FFLFOELEKIG, float Base)
	{
		if (KHOKDADOJCG)
		{
			int OEMALIFPGPO = 0;
			KMMJCHDKBDO.IBLHIAHECLK.Get(FFLFOELEKIG, ref OEMALIFPGPO);
			return Mathf.Pow(2f, (float)OEMALIFPGPO * Base);
		}
		return 1f;
	}

	public float GetBlock(bool OOCLHFGEPML)
	{
		GameUtils.BaseSettigs aMBADLGCMJE = GameUtils.DAMKDJINILI();
		return GetDamageBlockCritical(OOCLHFGEPML, aMBADLGCMJE.Attribute, aMBADLGCMJE.Base);
	}

	public float GetCritical(bool OOGIBOBMGJA)
	{
		GameUtils.BaseSettigs aMBADLGCMJE = GameUtils.IOGOPCABLON();
		return GetDamageBlockCritical(OOGIBOBMGJA, aMBADLGCMJE.Attribute, aMBADLGCMJE.Base);
	}

	public float GetTotalDamage(IntervalAttack CHCGJBLDPML, bool OOCLHFGEPML, bool OOGIBOBMGJA, ModelEdge GCFJNDJBBOI)
	{
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED == null)
		{
			Debug.LogError("attacker is null");
		}
		if (fGCODGKLHED.EPCNJLEHJCB() && EPCNJLEHJCB())
		{
			Debug.LogError("Both is player! Wat!?");
		}
		List<global::Pair<string, float>> list = CHCGJBLDPML.ACCOBHPHDDN();
		foreach (global::Pair<string, float> item in list)
		{
			if (item.First == "RaidChargeDamage")
			{
				int OEMALIFPGPO = 0;
				fGCODGKLHED.KMMJCHDKBDO.IBLHIAHECLK.Get(item.First, ref OEMALIFPGPO);
				return OEMALIFPGPO;
			}
		}
		string kLIIDDMHNOL = GetDefenseAttribute(CHCGJBLDPML, OOCLHFGEPML, GCFJNDJBBOI);
		float num = GameUtils.MGPIOCMLCLF();
		string kGBGENDIMBC = GameUtils.CJMOJMKCLMJ();
		int OEMALIFPGPO2 = 0;
		fGCODGKLHED.KMMJCHDKBDO.IBLHIAHECLK.Get(kGBGENDIMBC, ref OEMALIFPGPO2);
		OEMALIFPGPO2 = Mathf.Min(OEMALIFPGPO2, (int)GameUtils.PCDIBMDDAEF());
		float num2 = Mathf.Pow(2f, num * (float)OEMALIFPGPO2);
		float num3 = GetBlock(OOCLHFGEPML);
		float num4 = fGCODGKLHED.GetCritical(OOGIBOBMGJA);
		float num5 = 0f;
		float num6 = GameUtils.GetAttributesHitMultiplier(fGCODGKLHED.EPCNJLEHJCB(), fGCODGKLHED.KMMJCHDKBDO, KMMJCHDKBDO, list, kLIIDDMHNOL);
		float num7 = CHCGJBLDPML.GHGGNMBCMNM();
		float num8 = fGCODGKLHED.JKEKBCJHANF();
		float a = (num7 + num8) * num6 * num3 * num4 * num2;
		a = Mathf.Max(a, 0f);
		RuleAppliance eJPOJJKKICO = (fGCODGKLHED.EPCNJLEHJCB() ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		a *= CHCGJBLDPML.GetFactors(eJPOJJKKICO).Factor;
		a *= fGCODGKLHED.DHGBNMLMMLL();
		a *= fGCODGKLHED.LJCFIOPBNKD();
		if (a < 0f || 100000f < a)
		{
			Debug.LogError("Model::getTotalDamage - wtf so strong");
		}
		return a;
	}

	public IntervalAnimation FDMAIINMCHH()
	{
		return _Animation.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK);
	}

	public bool AMGHOKDANGN()
	{
		return FDMAIINMCHH() != null;
	}

	public virtual void CGEKLPLKIDC(Model MDKDAHCNCMC = null)
	{
		BFFLLGHDPEB = MDKDAHCNCMC;
		PNNMOKIBOPP = ((MDKDAHCNCMC == null) ? null : MDKDAHCNCMC.EGGEACCDAEK());
		_Strike = null;
		_Physics = null;
		_Animation = null;
		_Collision = null;
		HJOGNGDMAKJ = null;
		CAJANPOIPFC = false;
		HEJMFGFBLDK = false;
		HLNDHODNMCE = 0;
		PACHBHGEIGN = 0;
		LDLLJHEDCPD = string.Empty;
		CDBOONBLDBK = 0f;
		JCEOKJKKMCC = 0f;
		FJGNHALJJFF = -1;
		NIKPBGPPFEP = false;
		FJKIGPFIEDN = 0f;
		OKDDOLCHDCM = 0;
		ODLJHBDMEIJ = null;
		HCPHOJKFIDM = false;
		MHLIECOENGH = false;
		IDCHHGHAENM = true;
		PKFJFFGDOLB = false;
		FLKMDFDEJPP = true;
		AAJKEBAIJAP.Clear();
		Init();
		HLLAJOBDPEC = 0;
		OLGJILOCIEH = IIKHFKIBPJG.Prepare;
		NFOOGKCGFAB = 1;
		if (MDKDAHCNCMC != null)
		{
			JMHJDHLBHLK = MDKDAHCNCMC.JMHJDHLBHLK;
		}
		SetFixedSign(false);
		GDGHBKAENHK = false;
	}

	public Model BDJBNOPNCNB()
	{
		if (BFFLLGHDPEB != null)
		{
			return BFFLLGHDPEB.BDJBNOPNCNB();
		}
		return this;
	}

	public Model NMGNPBMFJKP(ModelType.KEIDBIOIFGA LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return this;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return NJDJHGDMCIJ();
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			return EGGEACCDAEK();
		case ModelType.KEIDBIOIFGA.MODEL_CHILD:
			if (JLDBGHLBJEL.Count != 0)
			{
				return JLDBGHLBJEL[JLDBGHLBJEL.Count - 1];
			}
			return null;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER_CHILD:
		{
			Model other = EGGEACCDAEK();
			return (other == null) ? null : other.NMGNPBMFJKP(ModelType.KEIDBIOIFGA.MODEL_CHILD);
		}
		default:
			LLLOJBFMONN.Error("Model::getModelByType ERROR - wrong model type: {0}", LFLGCDNKNJI);
			return null;
		}
	}

	public int JFCOFAELOCC(List<global::Pair<string, int>> IBLHIAHECLK)
	{
		int num = 0;
		foreach (global::Pair<string, int> item in IBLHIAHECLK)
		{
			int OEMALIFPGPO = 0;
			if (KMMJCHDKBDO.IBLHIAHECLK.Get(item.First, ref OEMALIFPGPO))
			{
				OEMALIFPGPO += item.Second;
				if (num < OEMALIFPGPO)
				{
					num = OEMALIFPGPO;
				}
			}
		}
		return num;
	}

	public float KJFIBMMOEPI()
	{
		return LBOLAOBGDEH.EDCHBILGFLD;
	}

	public float PHHHEGOBAPB()
	{
		return LBOLAOBGDEH.NNCHJCLKHHA;
	}

	public void BHAFOEICJPE(int value)
	{
		APOHBENDEKO = value;
	}

	public void KNCKHDNGKFO(WeaponModel LGCMGHAFEDD)
	{
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED != null)
		{
			Model fGCODGKLHED2 = fGCODGKLHED.BDJBNOPNCNB();
			if (fGCODGKLHED2.FGKAFKFBFEM())
			{
				fGCODGKLHED2.HJOGNGDMAKJ.StartRangedEnemy();
			}
		}
	}

	public void MGGBIBAHDEE(WeaponModel GHDEFJAEHLF)
	{
		JLDBGHLBJEL.Remove(GHDEFJAEHLF);
	}

	public void ABAOJIMJIDG()
	{
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			if (item.FJIJJNLLDPM())
			{
				item.MGPLABIFCAH(false);
			}
		}
	}

	public bool SetPain(float CKKFKEIELCP)
	{
		FJKIGPFIEDN += CKKFKEIELCP;
		if (!_ModelObject.EDJFLMILEBA() && FJKIGPFIEDN > GameUtils.APCAKCCOMLO.LILMAHHANIL)
		{
			return true;
		}
		return false;
	}

	public void OnStyleChanged(int JKJCPCCHJJN, string ECJDAIHCDBA, float KPNNGFGBNCI, bool GOAGDIANENH = false)
	{
		if (GOAGDIANENH)
		{
			if (PACHBHGEIGN == JKJCPCCHJJN)
			{
				JCEOKJKKMCC = KPNNGFGBNCI - CDBOONBLDBK;
			}
			else
			{
				JCEOKJKKMCC = 1f - CDBOONBLDBK + KPNNGFGBNCI + (float)(JKJCPCCHJJN - PACHBHGEIGN - 1);
			}
		}
		PACHBHGEIGN = JKJCPCCHJJN;
		LDLLJHEDCPD = ECJDAIHCDBA;
		CDBOONBLDBK = KPNNGFGBNCI;
	}

	public void KPAPLCPCOBE(object data)
	{
		CallEvent(13, this);
	}

	public bool LAGNKLAADPO()
	{
		return ODLJHBDMEIJ != null;
	}

	public void PONNDMHBGJK(IntervalAnimation.NGAJJDIEDGF LFLGCDNKNJI)
	{
		if (_Animation != null)
		{
			_Animation.PONNDMHBGJK(LFLGCDNKNJI);
		}
	}

	public void FDDONCMEAHA()
	{
		if (_Animation != null)
		{
			_Animation.FDDONCMEAHA();
		}
	}

	public void JELHEOLIDNG(ActionAnimation IBODMPMJELJ)
	{
		Model fGCODGKLHED = NMGNPBMFJKP(IBODMPMJELJ.OJLDHGKPLNC());
		if (fGCODGKLHED != null)
		{
			fGCODGKLHED.OPPIKLBKMPN(IBODMPMJELJ);
		}
	}

	public void OPPIKLBKMPN(ActionAnimation IBODMPMJELJ)
	{
		LLLOJBFMONN.Error("Model::startAction - unknown action: {0}", IBODMPMJELJ.get_Type());
	}

	public void OPPIKLBKMPN(ActionCreateModel IBODMPMJELJ)
	{
		EGFIFHKBNML(IBODMPMJELJ.DJBOFEEKJMP(), IBODMPMJELJ.AEGHBDJDPNA(), IBODMPMJELJ.StartAnimation);
	}

	public void OPPIKLBKMPN(ActionDelete IBODMPMJELJ)
	{
		Model eHCLMBADLKH = NMGNPBMFJKP(IBODMPMJELJ.OJLDHGKPLNC());
		NFADDANANJL();
		CallEvent(5, eHCLMBADLKH);
	}

	public void OPPIKLBKMPN(ActionSound IBODMPMJELJ)
	{
		if (IBODMPMJELJ.SameGender(KMMJCHDKBDO.OLPCELPEDKD))
		{
			Sound.IFKCCDAIADF(IBODMPMJELJ.get_Name(), IBODMPMJELJ.DBIOMDEIIKI(), IBODMPMJELJ.AFKMLMCCJLI());
		}
	}

	public void OPPIKLBKMPN(ActionStopSound IBODMPMJELJ)
	{
		Sound.StopSound(IBODMPMJELJ.get_Name());
	}

	public void OPPIKLBKMPN(ActionRandomSound IBODMPMJELJ)
	{
		if (IBODMPMJELJ.SameGender(KMMJCHDKBDO.OLPCELPEDKD))
		{
			Sound.IFKCCDAIADF(IBODMPMJELJ.get_Name());
		}
	}

	public void OPPIKLBKMPN(ActionEffect IBODMPMJELJ)
	{
		IBODMPMJELJ.set_Model(this);
		CallEvent(7, IBODMPMJELJ);
		IBODMPMJELJ.set_Model(null);
	}

	public void OPPIKLBKMPN(ActionStopEffect IBODMPMJELJ)
	{
		IBODMPMJELJ.set_Model(this);
		CallEvent(8, IBODMPMJELJ);
		IBODMPMJELJ.set_Model(null);
	}

	public void OPPIKLBKMPN(ActionAddBullets IBODMPMJELJ)
	{
		switch (IBODMPMJELJ.AOLGKCANKLL())
		{
		case BulletType.MAGIC_BULLET:
		{
			int fOIPKLDNGDL2 = IBODMPMJELJ.OEAKCOHMIHH();
			int bulletsBefore = MJEJFBHOJKB;
			InfoAnimation currentMagicAnimation = FHBLLPCEAHG();
			IPGBFKOCOCK(fOIPKLDNGDL2);
			Debug.Log("[MagicTrace] cast actor=" + get_Name() +
				" player=" + KMMJCHDKBDO.IsPlayer +
				" animation=" + ((currentMagicAnimation != null) ? currentMagicAnimation.Name : "<none>") +
				" animationFile=" + ((currentMagicAnimation != null) ? currentMagicAnimation.FileName : "<none>") +
				" items=" + GetMagicTraceItems() +
				" charge=" + bulletsBefore + "->" + MJEJFBHOJKB +
				" delta=" + fOIPKLDNGDL2);
			BFBFNKMLOJA();
			break;
		}
		case BulletType.RAID_CHARGE_BULLET:
		{
			int fOIPKLDNGDL = IBODMPMJELJ.OEAKCOHMIHH();
			MBPGHIINMJF(fOIPKLDNGDL);
			DDNEBDBABCM();
			break;
		}
		default:
			Debug.LogError("ERROR: Unknown bulletType");
			break;
		}
	}

	public void OPPIKLBKMPN(ActionStopFollowEffect IBODMPMJELJ)
	{
		IBODMPMJELJ.set_Model(this);
		CallEvent(9, IBODMPMJELJ);
		IBODMPMJELJ.set_Model(null);
	}

	public void OPPIKLBKMPN(ActionTryOnEnd IBODMPMJELJ)
	{
		CallEvent(14, null);
	}

	public void OPPIKLBKMPN(ActionShakeScreen IBODMPMJELJ)
	{
		CallEvent(15, IBODMPMJELJ);
	}

	public void OPPIKLBKMPN(ActionHitEffect IBODMPMJELJ)
	{
		if (NPACOADCOPJ.DataReady && Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().PHGNIPMBJEH(NPACOADCOPJ.Point, NPACOADCOPJ.IIIDIKABLOJ, NPACOADCOPJ.Time, IBODMPMJELJ.PCLGDBGPEKM(), HNILMKEAMAE);
		}
	}

	public void OPPIKLBKMPN(ActionZoomEffect IBODMPMJELJ)
	{
		CallEvent(17, IBODMPMJELJ);
	}

	public void OPPIKLBKMPN(ActionSetCooldown IBODMPMJELJ)
	{
		int pLKFFGILBOP = IBODMPMJELJ.OMIEPGOPPBO();
		string bAINMLLIKOL = IBODMPMJELJ.GHHAKGGLBCN();
		FightCID eCHINOPKGGI = (FightCID)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.KEY_TYPE, bAINMLLIKOL);
		OBJCCBMMDJH(eCHINOPKGGI, 0);
		bool flag = true;
		if (eCHINOPKGGI == FightCID.RaidChargeButton)
		{
			flag = false;
			KAOPLEPILDH kAOPLEPILDH = KMMJCHDKBDO as KAOPLEPILDH;
			if (kAOPLEPILDH != null && kAOPLEPILDH.LMIBBJIKLNO != null)
			{
				NoAnimationMove.JNMKPGHOFIF jNMKPGHOFIF = QuestUtils.BKBHIHMEMEH().HLINMMJEANJ(kAOPLEPILDH.LMIBBJIKLNO.Name);
				if (jNMKPGHOFIF != null)
				{
					pLKFFGILBOP = (int)jNMKPGHOFIF.CJDIOEEBKAL;
					flag = PDGBMLJEJKG > 0;
				}
			}
		}
		if (flag)
		{
			PJGPCDPPOHA(eCHINOPKGGI, pLKFFGILBOP);
			if (!SystemProperties.DBBOCENKMGD())
			{
			}
		}
		else
		{
			OBJCCBMMDJH(eCHINOPKGGI, 30);
		}
	}

	public void OPPIKLBKMPN(ActionSetEndStage IBODMPMJELJ)
	{
		Model target = NMGNPBMFJKP(IBODMPMJELJ.OJLDHGKPLNC());
		if (target == null)
			target = this;
		target.PKFJFFGDOLB = true;
	}

	public void OPPIKLBKMPN(ActionPlayAnimation IBODMPMJELJ)
	{
		Model target = null;
		if (!string.IsNullOrEmpty(IBODMPMJELJ.ChildName))
		{
			foreach (WeaponModel child in KGGIDBLBMDJ())
			{
				if (child.get_Name() == IBODMPMJELJ.ChildName)
				{
					target = child;
					break;
				}
			}
		}
		if (target == null)
			target = NMGNPBMFJKP(IBODMPMJELJ.OJLDHGKPLNC());
		if (target != null)
			target.PlayAnimation(IBODMPMJELJ.AnimationName);
	}

	public WeaponModel EGFIFHKBNML(List<CopyItemInfo> HELFDCAIJNE = null, string JLHDJLHLGND = "", string startAnimation = "")
	{
		if (HELFDCAIJNE == null)
		{
			HELFDCAIJNE = new List<CopyItemInfo>();
		}
		ModelParameters kIKOGDEPGHB = PAKCLIHBHKG(HELFDCAIJNE);
		kIKOGDEPGHB.IsPlayer = EPCNJLEHJCB();
		WeaponModel gKIANLDJFCH = new WeaponModel(kIKOGDEPGHB);
		gKIANLDJFCH.set_Name(JLHDJLHLGND);
		gKIANLDJFCH.SetExplicitBirthAnimation(startAnimation);
		gKIANLDJFCH.MABELGMBHEA(HOOKPFLBFPD);
		gKIANLDJFCH.KMMJCHDKBDO.IBBALIJOJMC = SceneTypes.SceneFight;
		gKIANLDJFCH.CGEKLPLKIDC(this);
		gKIANLDJFCH.CJNGMIMHFCC(EGGEACCDAEK());
		gKIANLDJFCH.SetImpulseFactor(ODCOKJKEDOJ);
		gKIANLDJFCH.PFIJCCKDAAB(LJCFIOPBNKD());
		JLDBGHLBJEL.Add(gKIANLDJFCH);
		CallEvent(6, gKIANLDJFCH);
		return gKIANLDJFCH;
	}

	public void SetExplicitBirthAnimation(string animationName)
	{
		_ExplicitBirthAnimation = animationName;
	}

	public bool HasExplicitBirthAnimation()
	{
		return !string.IsNullOrEmpty(_ExplicitBirthAnimation);
	}

	public bool TryPlayExplicitBirthAnimation()
	{
		if (string.IsNullOrEmpty(_ExplicitBirthAnimation))
			return false;

		string animationName = _ExplicitBirthAnimation;
		_ExplicitBirthAnimation = string.Empty;
		InfoAnimation animation = null;
		foreach (InfoAnimation candidate in OHAMEHHMEAL)
		{
			if (candidate.Name == animationName)
			{
				animation = candidate;
				break;
			}
		}
		// A newer StartAnimation can reference a move excluded from the legacy
		// per-model cache.  It is still a valid parsed move and is safe to play on
		// the helper for which the XML explicitly requested it.
		if (animation == null)
			animation = AnimationData.BCIFKBJAFEC(animationName, false);
		if (animation == null)
		{
			Debug.LogError("[MagicTrace] explicit-start missing actor=" + get_Name() +
				" animation=" + animationName + " items=" + GetMagicTraceItems());
			return false;
		}
		bool started = PlayAnimation(animation);
		Debug.Log("[MagicTrace] explicit-start actor=" + get_Name() +
			" animation=" + animationName + " started=" + started +
			" items=" + GetMagicTraceItems());
		return started;
	}

	public bool IsControlKeys(int KGBGENDIMBC)
	{
		if (KMMJCHDKBDO.JCMFKLGCEOG.Count == 0)
		{
			return true;
		}
		foreach (int item in KMMJCHDKBDO.JCMFKLGCEOG)
		{
			int num = item;
			if (num == KGBGENDIMBC)
			{
				return true;
			}
		}
		return false;
	}

	public void CMBHIBKEAJH(CurrentEffect LLOLBKJMKNC)
	{
		if (!BJKJBIMPPAM.Contains(LLOLBKJMKNC))
		{
			BJKJBIMPPAM.Add(LLOLBKJMKNC);
		}
	}

	public void MICMDHGOCAN(CurrentEffect LLOLBKJMKNC)
	{
		BJKJBIMPPAM.Remove(LLOLBKJMKNC);
	}

	public void FKIBECCHIJC()
	{
		for (int i = 0; i < BJKJBIMPPAM.Count; i++)
		{
			BJKJBIMPPAM[i].ACENLMONNPA = null;
		}
	}

	public int LPFPGDJALED()
	{
		if (NLHFJIEHKMM())
		{
			return _Physics.PGOFHCBPLOE();
		}
		InfoAnimation pJAHIOELGGD = FHBLLPCEAHG();
		if (pJAHIOELGGD != null)
		{
			return _Animation.LPFPGDJALED();
		}
		return -1;
	}

	public int NODAINEDAKJ()
	{
		if (NLHFJIEHKMM())
		{
			return _Physics.PGOFHCBPLOE();
		}
		InfoAnimation pJAHIOELGGD = FHBLLPCEAHG();
		if (pJAHIOELGGD != null)
		{
			return _Animation.NODAINEDAKJ();
		}
		return -1;
	}

	public bool IMCHEIAGOPF()
	{
		int num = LPFPGDJALED();
		if (num > -1)
		{
			InfoAnimation pJAHIOELGGD = FHBLLPCEAHG();
			if (pJAHIOELGGD == null)
			{
				return true;
			}
			return num > _Animation.LOIJGOPOGMO();
		}
		return false;
	}

	public bool ANHGOGDEFCO()
	{
		return _Animation.ANHGOGDEFCO();
	}

	public void UpdateMagicCharge(float CKKFKEIELCP, Model HFGPAELCNMF, bool OOCLHFGEPML, bool OOGIBOBMGJA, bool FNMEMMLNKJL)
	{
		Model fGCODGKLHED = NJDJHGDMCIJ();
		if (fGCODGKLHED != null)
		{
			fGCODGKLHED.UpdateMagicCharge(CKKFKEIELCP, HFGPAELCNMF, OOCLHFGEPML, OOGIBOBMGJA, FNMEMMLNKJL);
		}
		else if (MJEJFBHOJKB == 0)
		{
			HFGPAELCNMF = HFGPAELCNMF.BDJBNOPNCNB();
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (FNMEMMLNKJL)
			{
				num = GameUtils.DILKHIFCCGD.LLKJJLOMNID(this);
				num2 = HFGPAELCNMF.GetBlock(OOCLHFGEPML);
				num3 = GetCritical(OOGIBOBMGJA);
			}
			else
			{
				num = GameUtils.DILKHIFCCGD.MPIOONCNFOK(this);
				num2 = GetBlock(OOCLHFGEPML);
				num3 = HFGPAELCNMF.GetCritical(OOGIBOBMGJA);
			}
			float fOIPKLDNGDL = Mathf.Pow(2f, num) * num2 * num3 * CKKFKEIELCP;
			JJHLOKBPBLD(fOIPKLDNGDL);
			BFBFNKMLOJA();
		}
	}

	public int NPDOLGNNINO()
	{
		if (BFFLLGHDPEB != null)
		{
			return BFFLLGHDPEB.NPDOLGNNINO();
		}
		return BNFCCKBIIDB.NPDOLGNNINO();
	}

	public void CKCCBJKIGIO(PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().CKCCBJKIGIO(this, IBODMPMJELJ, CCBEDPIHKAD);
		}
	}

	public void GICAFBABMGA(PerksStage.ActionPerk CKOEFOCPMGK, PerksStage.ActionPerk IBODMPMJELJ)
	{
		if (Fight.OHNKFOHIAKG() != null)
		{
			Fight.OHNKFOHIAKG().GICAFBABMGA(this, CKOEFOCPMGK, IBODMPMJELJ);
		}
	}

	public void RemoveInterval(IntervalAnimation.NGAJJDIEDGF LFLGCDNKNJI)
	{
		_Animation.RemoveInterval(LFLGCDNKNJI);
	}

	public void RemoveInterval(string name)
	{
		_Animation.RemoveInterval(name);
	}

	public void RemoveIntervals(List<string> NFLDEGMEJAK)
	{
		_Animation.RemoveIntervals(NFLDEGMEJAK);
	}

	public void ALJKJJKKIEF()
	{
		if (FJGNHALJJFF < 0)
		{
			FJGNHALJJFF = GameUtils.APCAKCCOMLO.JKOMIENEACF;
		}
	}

	public int CLPDEPPPJFE()
	{
		return BNFCCKBIIDB.CLPDEPPPJFE();
	}

	public void NextRound(int MHPLDHBGBFO)
	{
		set_Round(MHPLDHBGBFO);
		FHMLAFHENBB(true);
		BHAFOEICJPE(-1);
		MOMEOOGDELJ(false);
		DKFGOHCNIKL.MLJCABABNDB();
	}

	public void GFNCMLFKBGP(float DLEDDPFNPOH)
	{
		KMMJCHDKBDO.GFNCMLFKBGP(DLEDDPFNPOH);
	}

	public float KKMCHCNOHMB()
	{
		return (ObscuredFloat)(KMMJCHDKBDO.KKMCHCNOHMB());
	}

	public void GEACPINOAAN(float AACBFABMADJ)
	{
		KMMJCHDKBDO.GEACPINOAAN(AACBFABMADJ);
	}

	public bool PDFCAFIMALN()
	{
		return !KMMJCHDKBDO.OJMIFOAHKBK();
	}

	public static string GetDefenseAttribute(IntervalAttack CHCGJBLDPML, bool OOCLHFGEPML, ModelEdge GCFJNDJBBOI)
	{
		List<string> list = CHCGJBLDPML.DONAJGIBKCC();
		if (0 < list.Count)
		{
			return list[0];
		}
		if (OOCLHFGEPML)
		{
			return GameUtils.IIFHFGAENMH();
		}
		if (GCFJNDJBBOI != null && !string.IsNullOrEmpty(GCFJNDJBBOI.NLLGDDMMJJN()))
		{
			return GCFJNDJBBOI.NLLGDDMMJJN();
		}
		return GameUtils.HLBPBLMMPCB();
	}

	public void UpdateAnimationParameters(List<Model> INNLAFHKJNI)
	{
		ModelObject eFALNIGJKLB = _ModelObject;
		bool dPKOKLCJEHI = EPCNJLEHJCB();
		bool eMGNKKHPGCJ = NJDJHGDMCIJ() != null;
		List<InfoAnimation> lNKFKJKLCKP = MCFPDHOLNGB();
		List<Trigger> aIPCBIBMFCB = NOJEIGNOPII();
		foreach (Model item in INNLAFHKJNI)
		{
			item.KMKOHGBJNBK(lNKFKJKLCKP, eFALNIGJKLB, dPKOKLCJEHI, eMGNKKHPGCJ, aIPCBIBMFCB);
		}
	}

	public void KMKOHGBJNBK(List<InfoAnimation> LNKFKJKLCKP, ModelObject BBGCMFGFMCL, bool DPKOKLCJEHI, bool EMGNKKHPGCJ, List<Trigger> AIPCBIBMFCB)
	{
		List<InfoAnimation> list = MCFPDHOLNGB();
		foreach (InfoAnimation item in list)
		{
			item.BPHNHFJCFCD(BBGCMFGFMCL, DPKOKLCJEHI, EMGNKKHPGCJ, BBGCMFGFMCL);
		}
		List<Trigger> list2 = NOJEIGNOPII();
		foreach (Trigger item2 in list2)
		{
			item2.BPHNHFJCFCD(BBGCMFGFMCL, DPKOKLCJEHI, EMGNKKHPGCJ, BBGCMFGFMCL);
		}
		ModelObject eFALNIGJKLB = _ModelObject;
		bool eKBOGDKIHIH = EPCNJLEHJCB();
		bool pHADJMAONJG = NJDJHGDMCIJ() != null;
		foreach (InfoAnimation item3 in LNKFKJKLCKP)
		{
			item3.BPHNHFJCFCD(eFALNIGJKLB, eKBOGDKIHIH, pHADJMAONJG, eFALNIGJKLB);
		}
		foreach (Trigger item4 in AIPCBIBMFCB)
		{
			item4.BPHNHFJCFCD(eFALNIGJKLB, eKBOGDKIHIH, pHADJMAONJG, eFALNIGJKLB);
		}
	}

	public void LogDamage(float CKKFKEIELCP, string BBNKIBKPBLO, string target)
	{
		NDOAKHGPOHL().Add(CKKFKEIELCP, BBNKIBKPBLO, target);
	}

	public void GKDJBGMABDO(List<ActionAnimation> AFENHJFICNN)
	{
		foreach (ActionAnimation item in AFENHJFICNN)
		{
			bool canVisit = item.CanVisit(this);
			ActionEffect conditionalEffect = item as ActionEffect;
			if (conditionalEffect != null && item.GetConditionCount() > 0)
			{
				InfoAnimation current = FHBLLPCEAHG();
				Debug.Log("[MagicTrace] effect-gate actor=" + get_Name() +
					" animation=" + ((current != null) ? current.Name : "<none>") +
					" action=" + conditionalEffect.get_Name() +
					" sequence=" + conditionalEffect.EPDMGFELIMC() +
					" conditions=" + item.GetConditionCount() +
					" allowed=" + canVisit +
					" items=" + GetMagicTraceItems());
			}
			if (canVisit)
				item.Visit(this);
		}
	}

	public string GetMagicTraceItems()
	{
		List<string> items = new List<string>();
		foreach (ItemInfo item in GetModelConditionItems())
		{
			if (item != null && (item.Type == "Magic" || item.Type == "Weapon" || item.Type == "Ranged"))
				items.Add(item.Type + ":" + item.Name + "/" + item.MDPPNGIEJGD);
		}
		return (items.Count == 0) ? "<none>" : string.Join(",", items.ToArray());
	}

	private List<ItemInfo> GetModelConditionItems()
	{
		List<ItemInfo> items = KMMJCHDKBDO.PJNJIJIODHE();
		if (KMMJCHDKBDO.OJIAKDDCGLB != null)
		{
			foreach (ItemInfo item in KMMJCHDKBDO.OJIAKDDCGLB)
			{
				if (item != null && !items.Contains(item))
					items.Add(item);
			}
		}
		return items;
	}

	public void SetHitData(Vector3f NAAPALOFBCI, Vector3f KKIKIDNALOL, float time)
	{
		NPACOADCOPJ.Point = NAAPALOFBCI;
		NPACOADCOPJ.IIIDIKABLOJ = KKIKIDNALOL;
		NPACOADCOPJ.Time = time;
		NPACOADCOPJ.DataReady = true;
	}

	public void ResetHitData()
	{
		NPACOADCOPJ.DataReady = false;
	}

	public void OBJCCBMMDJH(FightCID DDNBGEJJGMG, int frames)
	{
		switch (DDNBGEJJGMG)
		{
		case FightCID.Punch:
		{
			MDFEHKBOHEL.CNGALGBKFOK = false;
			MDFEHKBOHEL.KIKLFDLLDDP = 0f;
			EventActBtnSettings eHCLMBADLKH4 = new EventActBtnSettings(FightCID.Punch, 0f, frames);
			CallEvent(12, eHCLMBADLKH4);
			break;
		}
		case FightCID.Kick:
		{
			MDFEHKBOHEL.MPIOLPLLFEM = false;
			MDFEHKBOHEL.CPKHGNDBKFL = 0f;
			EventActBtnSettings eHCLMBADLKH3 = new EventActBtnSettings(FightCID.Kick, 0f, frames);
			CallEvent(12, eHCLMBADLKH3);
			break;
		}
		case FightCID.MissileButton:
		{
			MDFEHKBOHEL.MLDHFPCCCOP = false;
			MDFEHKBOHEL.DPMNMLHCJLK = 0f;
			EventActBtnSettings eHCLMBADLKH2 = new EventActBtnSettings(FightCID.MissileButton, 0f, frames);
			CallEvent(12, eHCLMBADLKH2);
			break;
		}
		case FightCID.RaidChargeButton:
		{
			MDFEHKBOHEL.FDHAJDFJBCF = false;
			MDFEHKBOHEL.IFMNJHFPDIC = 0f;
			EventActBtnSettings eHCLMBADLKH = new EventActBtnSettings(FightCID.RaidChargeButton, 0f, frames);
			CallEvent(12, eHCLMBADLKH);
			break;
		}
		case FightCID.MagicButton:
			break;
		}
	}

	public void PJGPCDPPOHA(FightCID DDNBGEJJGMG, int frames)
	{
		if (frames <= 0)
		{
			frames = 1;
		}
		switch (DDNBGEJJGMG)
		{
		case FightCID.Punch:
			MDFEHKBOHEL.CNGALGBKFOK = true;
			MDFEHKBOHEL.DBMLGHOMCEA = frames;
			break;
		case FightCID.Kick:
			MDFEHKBOHEL.MPIOLPLLFEM = true;
			MDFEHKBOHEL.GKAPNAOFMFP = frames;
			break;
		case FightCID.MissileButton:
			MDFEHKBOHEL.MLDHFPCCCOP = true;
			MDFEHKBOHEL.KFALPODCLFA = frames;
			break;
		case FightCID.RaidChargeButton:
			MDFEHKBOHEL.FDHAJDFJBCF = true;
			MDFEHKBOHEL.IKNPPGLMBDK = frames;
			break;
		case FightCID.MagicButton:
			break;
		}
	}

	public void GLKOLOBIHLP()
	{
		MBOCCLOLEFH(KMMJCHDKBDO);
		NCGEHCHIBBH.NKKOAAKHINN();
		NCGEHCHIBBH.HGPNHBMHIKH(NMILPLHGCMA);
	}

	public void SetImpulseFactor(Vector3f OLOAPIIOBKK)
	{
		SetImpulseFactor(OLOAPIIOBKK.GILCBJJPKBK(), OLOAPIIOBKK.OBIMBNIBEFG(), OLOAPIIOBKK.KMFEKANLCFO());
	}

	public void SetImpulseFactor(float DHDMNHCIPEH, float BGEEALIPKCC, float LKPCKJOLJDO)
	{
		ODCOKJKEDOJ.Set(DHDMNHCIPEH, BGEEALIPKCC, LKPCKJOLJDO);
	}

	public void MGNOBDLOINP()
	{
		ODCOKJKEDOJ.Set(1f, 1f, 1f);
	}

	public void CKOEEMFHCFK()
	{
		HNILMKEAMAE = 1f;
	}

	public void LNBCEJDJPAH()
	{
		DIKMCKLIEBK = 1f;
	}

	public void MBPGHIINMJF(int FOIPKLDNGDL)
	{
		if (FOIPKLDNGDL < 0)
		{
			AIAKAAECMEH++;
		}
		PDGBMLJEJKG += FOIPKLDNGDL;
	}

	public void DDNEBDBABCM()
	{
		Model fGCODGKLHED = NJDJHGDMCIJ();
		if (fGCODGKLHED == null)
		{
			if (EPCNJLEHJCB())
			{
				if (CKAKLHDLHJO() == 0)
				{
					EventActBtnSettings eHCLMBADLKH = new EventActBtnSettings(FightCID.RaidChargeButton, 0f, 0);
					CallEvent(12, eHCLMBADLKH);
				}
				EventActBtnSettings eHCLMBADLKH2 = new EventActBtnSettings(FightCID.RaidChargeButton, -1f, -1, CKAKLHDLHJO());
				CallEvent(18, eHCLMBADLKH2);
			}
		}
		else
		{
			fGCODGKLHED.BFBFNKMLOJA();
		}
	}

	public bool FPPKOMOPDJJ()
	{
		if (BFFLLGHDPEB != null)
		{
			return BFFLLGHDPEB.FPPKOMOPDJJ();
		}
		return BNFCCKBIIDB.FPPKOMOPDJJ();
	}

	protected virtual void GCIMAADHICB(ModelParameters data)
	{
		List<PerkInfoItem> mAFPBEFKNGE = KMMJCHDKBDO.JBIOECDAAKP();
		List<PerkInfoItem> cFKCGBEONAM = null;
		if (EGGEACCDAEK() != null && EGGEACCDAEK().KMMJCHDKBDO != null)
		{
			cFKCGBEONAM = EGGEACCDAEK().KMMJCHDKBDO.JBIOECDAAKP();
		}
		List<ItemInfo> fJKCMJNAFJD = KMMJCHDKBDO.PJNJIJIODHE();
		BJLLJHDFMOO(fJKCMJNAFJD, false, KMMJCHDKBDO.DANNKMJOOOH, KMMJCHDKBDO.IBBALIJOJMC, mAFPBEFKNGE, cFKCGBEONAM);
	}

	protected virtual void MBOCCLOLEFH(ModelParameters data)
	{
		List<PerkInfoItem> mAFPBEFKNGE = KMMJCHDKBDO.JBIOECDAAKP();
		// Equipped items and action-created items live in separate collections.
		// Newer move actions test the equipped Magic/Weapon subtype directly, so
		// using only OJIAKDDCGLB left shop previews with an empty condition context
		// and made shared templates choose unrelated fallback effects.
		List<ItemInfo> oJIAKDDCGLB = GetModelConditionItems();
		_ModelConditions.OJIAKDDCGLB = oJIAKDDCGLB;
		List<PerkInfoItem> cFKCGBEONAM = null;
		if (EGGEACCDAEK() != null && EGGEACCDAEK().KMMJCHDKBDO != null)
		{
			cFKCGBEONAM = EGGEACCDAEK().KMMJCHDKBDO.JBIOECDAAKP();
		}
		LBEFFCACPJL(oJIAKDDCGLB, false, KMMJCHDKBDO.IBBALIJOJMC, mAFPBEFKNGE, cFKCGBEONAM);
	}

	protected void DIIIIDDKHNG()
	{
		if (_Physics.EGNOOKHNFLK())
		{
			return;
		}
		float eDCHBILGFLD = LBOLAOBGDEH.EDCHBILGFLD;
		float nNCHJCLKHHA = LBOLAOBGDEH.NNCHJCLKHHA;
		float num = LBOLAOBGDEH.EDCHBILGFLD;
		float num2 = LBOLAOBGDEH.NNCHJCLKHHA;
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		for (int i = 0; i < list.Count; i++)
		{
			float num3 = list[i].ICLEOFDKDIF().GILCBJJPKBK();
			if (num3 < num && num3 < eDCHBILGFLD)
			{
				num = num3;
			}
			if (num2 < num3 && nNCHJCLKHHA < num3)
			{
				num2 = num3;
			}
		}
		_Physics.SetWallShift(num, num2);
	}

	protected void Clear()
	{
		RemoveAllEventListener();
		if (_Collision != null)
		{
			FEHOHLMIEBP.RemoveAllEventListener();
		}
		if (_Animation != null)
		{
			_Animation.RemoveAllEventListener();
		}
		_Physics = null;
		_Strike = null;
		_Animation = null;
		_Collision = null;
		HJOGNGDMAKJ = null;
		_ModelObject = null;
		KDAHHIMLJGG.Clear();
		KDAHHIMLJGG = null;
		_Enemies.Clear();
		JLDBGHLBJEL.Clear();
	}

	protected void FDDIAFGKODA(InfoAnimation DBOLBEOCEME)
	{
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED != null)
		{
			if (FGKAFKFBFEM() || AiData.get_BothBotEnabled())
			{
				HJOGNGDMAKJ.StartAnimationEnemy(fGCODGKLHED);
			}
			if (fGCODGKLHED.FGKAFKFBFEM() || AiData.get_BothBotEnabled())
			{
				fGCODGKLHED.HJOGNGDMAKJ.StartAnimationBot(DBOLBEOCEME);
			}
		}
	}

	protected float EOGCPJJHCCA()
	{
		if (_Animation.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK) != null)
		{
			return 5f;
		}
		return 1f;
	}

	protected void EEDJEDBMIMI(ItemInfo item, bool EPKEEMFHHFM, bool BNDJNLALHKL = true, bool DDMEACNNLJN = true)
	{
		if (item == null)
		{
			return;
		}
		ItemInfo dJKEECEOCJB = KMMJCHDKBDO.KDABEFBJMOD(item.Type);
		if (dJKEECEOCJB == null || !(dJKEECEOCJB.Name != item.Name))
		{
			return;
		}
		ODLJHBDMEIJ = ((!EPKEEMFHHFM) ? null : dJKEECEOCJB);
		KMMJCHDKBDO.OLLNIKFPMKE(item.Type, item);
		string aPJJEFJHJGK = GameUtils.APCAKCCOMLO.APJJEFJHJGK;
		int OEMALIFPGPO = 0;
		item.IBLHIAHECLK.Get(aPJJEFJHJGK, ref OEMALIFPGPO);
		KMMJCHDKBDO.IBLHIAHECLK.Set(aPJJEFJHJGK, (!DDMEACNNLJN) ? GameUtils.APCAKCCOMLO.OMPDIOBDAKB : OEMALIFPGPO);
		if (BNDJNLALHKL)
		{
			List<ItemInfo> hELFDCAIJNE = KMMJCHDKBDO.PJNJIJIODHE();
			List<PerkInfoItem> mAFPBEFKNGE = KMMJCHDKBDO.JBIOECDAAKP();
			List<PerkInfoItem> cFKCGBEONAM = null;
			if (EGGEACCDAEK() != null && EGGEACCDAEK().KMMJCHDKBDO != null)
			{
				cFKCGBEONAM = EGGEACCDAEK().KMMJCHDKBDO.JBIOECDAAKP();
			}
			GMFOJPHEHHI(hELFDCAIJNE, mAFPBEFKNGE, cFKCGBEONAM);
		}
	}

	public void SwapPerkItem(ItemInfo item)
	{
		EEDJEDBMIMI(item, false, true, false);
	}

	protected void DIDBGCFDKGD()
	{
		if (_ModelObject.EDJFLMILEBA())
		{
			return;
		}
		ItemInfo jGMLKIPCFII = KMMJCHDKBDO.JGMLKIPCFII;
		ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(GameUtils.APCAKCCOMLO.JIIFFJAJNNN);
		if (dJKEECEOCJB == null)
		{
			return;
		}
		EEDJEDBMIMI(dJKEECEOCJB, true, true, false);
		_ModelObject.set_IsShock(true);
		KMMJCHDKBDO.AHMMOKMGICA();
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			if (item.EDJFLMILEBA())
			{
				float lHNJJFDIJKK = GameUtils.APCAKCCOMLO.IIIDIKABLOJ.GILCBJJPKBK() / item.FJJFKAJOFNJ();
				float fFFHIOALHGM = GameUtils.APCAKCCOMLO.IIIDIKABLOJ.OBIMBNIBEFG() / item.FJJFKAJOFNJ();
				float pDCENMEKIAP = GameUtils.APCAKCCOMLO.IIIDIKABLOJ.KMFEKANLCFO() / item.FJJFKAJOFNJ();
				item.ICLEOFDKDIF().Add(lHNJJFDIJKK, fFFHIOALHGM, pDCENMEKIAP);
			}
		}
		Model fGCODGKLHED = EGGEACCDAEK();
		if (fGCODGKLHED != null)
		{
			Model fGCODGKLHED2 = fGCODGKLHED.BDJBNOPNCNB();
			fGCODGKLHED2.HJOGNGDMAKJ.SetWeaponEnemy(dJKEECEOCJB.MDPPNGIEJGD);
		}
		HJOGNGDMAKJ.SetWeaponBot(dJKEECEOCJB.MDPPNGIEJGD);
		DisarmData eHCLMBADLKH = new DisarmData(this, jGMLKIPCFII.NHBIJEEKALC);
		CallEvent(16, eHCLMBADLKH);
	}

	protected void GBGJIOLEEJK()
	{
		if (!_ModelObject.EDJFLMILEBA() && FJGNHALJJFF >= 0)
		{
			if (FJGNHALJJFF == 0)
			{
				DIDBGCFDKGD();
			}
			FJGNHALJJFF--;
		}
		FJKIGPFIEDN = Mathf.Max(FJKIGPFIEDN - GameUtils.APCAKCCOMLO.LPHBGLLAEOG, 0f);
	}

	protected ModelParameters PAKCLIHBHKG(List<CopyItemInfo> HELFDCAIJNE = null)
	{
		if (HELFDCAIJNE == null)
		{
			HELFDCAIJNE = new List<CopyItemInfo>();
		}
		KAOPLEPILDH kAOPLEPILDH = new KAOPLEPILDH();
		kAOPLEPILDH.DLDMOHEGENM((ObscuredInt)(1));
		kAOPLEPILDH.AKLPHMOAIGK = 1;
		kAOPLEPILDH.KFMJMBANIGF = 1f;
		kAOPLEPILDH.EHBHNGOGCKO = 1f;
		kAOPLEPILDH.BMFLPBLAFLK = string.Empty;
		kAOPLEPILDH.FMOKLKFCCKF = string.Empty;
		kAOPLEPILDH.HNKFHGOOKEG = GameUtils.BGGMLFLFONJ();
		kAOPLEPILDH.EEGMBGBLLIF = false;
		kAOPLEPILDH.HKJFJHBHMND = true;
		kAOPLEPILDH.IsPlayer = false;
		kAOPLEPILDH.ABAPAIEBNGK = false;
		kAOPLEPILDH.IsWinner = false;
		kAOPLEPILDH.BHHLEBHLBLH = false;
		kAOPLEPILDH.PCALDKCJGCK = false;
		kAOPLEPILDH.FCOALLOHJNP = 0;
		kAOPLEPILDH.CIDCNCDFONA = 0f;
		kAOPLEPILDH.PILJCAOFAED = null;
		kAOPLEPILDH.LKKFNMBCCDB = null;
		kAOPLEPILDH.FKMOLBBLKDA = null;
		kAOPLEPILDH.JGMLKIPCFII = null;
		kAOPLEPILDH.LGHMILECPLA = null;
		kAOPLEPILDH.ADBKGIBBNHJ = null;
		kAOPLEPILDH.LMIBBJIKLNO = null;
		if (HELFDCAIJNE.Count != 0)
		{
			int i = 0;
			for (int count = HELFDCAIJNE.Count; i < count; i++)
			{
				BGNBNDBDGGP(HELFDCAIJNE[i], kAOPLEPILDH);
			}
		}
		kAOPLEPILDH.PPFDLIBLNDG();
		kAOPLEPILDH.NOBKKLBJFIL();
		return kAOPLEPILDH;
	}

	protected ModelObject OEKFONJCEFG(ModelType.KEIDBIOIFGA HJMMACIELFG)
	{
		ModelObject result = null;
		switch (HJMMACIELFG)
		{
		case ModelType.KEIDBIOIFGA.MODEL_NULL:
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			result = _ModelObject;
			break;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			if (PNNMOKIBOPP != null)
			{
				result = PNNMOKIBOPP._ModelObject;
			}
			else
			{
				// Shop previews have no opponent. The original diagnostic says this
				// should align to self, but the decompiled body returned null and left
				// projectile nodes unresolved (notably Death Ray and Fire Pillar).
				result = _ModelObject;
				Debug.Log("[MagicTrace] align-fallback actor=" + get_Name() + " requested=Enemy resolved=Self");
			}
			break;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			if (BFFLLGHDPEB != null)
			{
				result = BFFLLGHDPEB._ModelObject;
			}
			break;
		}
		return result;
	}

	public bool FGAHBBDGPBO()
	{
		return PJLKOEIMJNA;
	}

	public void NEHLJGPKHKF(bool value)
	{
		PJLKOEIMJNA = value;
	}

	protected float HFLFIKJPGCJ()
	{
		if (FGAHBBDGPBO())
		{
			return 100f;
		}
		Model fGCODGKLHED = EGGEACCDAEK();
		if (Fight.OHNKFOHIAKG().MBEJJCKIIHK() != BattleType.FightRaid || !fGCODGKLHED.EPCNJLEHJCB() || fGCODGKLHED.DKFGOHCNIKL.FMGDKLFNKGM())
		{
			return GameUtils.HHCEIEOOHCJ.JJNCDHOKEIA(fGCODGKLHED);
		}
		return 0f;
	}

	protected void GPMGEJKBAJG()
	{
		Fight gDBOMJODDEA = Fight.OHNKFOHIAKG();
		if (gDBOMJODDEA == null)
		{
			return;
		}
		int OEMALIFPGPO = 0;
		if (KMMJCHDKBDO.IBLHIAHECLK.Get(GameUtils.DMLPOANHHFI().Attribute, ref OEMALIFPGPO) && EGGEACCDAEK() != null)
		{
			float num = (float)OEMALIFPGPO * GameUtils.DMLPOANHHFI().Base * EGGEACCDAEK().LJCFIOPBNKD();
			if (num != 0f && (num > 0f || !HOOKPFLBFPD))
			{
				gDBOMJODDEA.UpdateLife(this, num);
			}
		}
	}

	protected void NBIPOHOPFGA(object data)
	{
		List<ActionAnimation> aFENHJFICNN = (List<ActionAnimation>)data;
		GKDJBGMABDO(aFENHJFICNN);
	}

	protected void BJLLJHDFMOO(List<ItemInfo> FJKCMJNAFJD, bool ILMJFHCNLHC, List<string> DANNKMJOOOH = null, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		AnimationData.AKJLPGMEFFD(OHAMEHHMEAL, FJKCMJNAFJD, ILMJFHCNLHC, KMMJCHDKBDO.DANNKMJOOOH, NFNJJIGAKNN, MAFPBEFKNGE, CFKCGBEONAM);
		EKOGDEOEHLI();
	}

	protected void LBEFFCACPJL(List<ItemInfo> FJKCMJNAFJD, bool ILMJFHCNLHC, SceneTypes NFNJJIGAKNN = SceneTypes.SceneFight, List<PerkInfoItem> MAFPBEFKNGE = null, List<PerkInfoItem> CFKCGBEONAM = null)
	{
		AnimationData.FMDFKKEDMJG(NMILPLHGCMA, FJKCMJNAFJD, ILMJFHCNLHC, NFNJJIGAKNN, MAFPBEFKNGE, CFKCGBEONAM);
		GJDJAIPPGLO();
	}

	protected void EKOGDEOEHLI()
	{
		foreach (InfoAnimation item in OHAMEHHMEAL)
		{
			item.PreloadEffects();
			item.PreloadSounds();
		}
	}

	protected void GJDJAIPPGLO()
	{
		foreach (Trigger item in NMILPLHGCMA)
		{
			item.PreloadEffects();
			item.PreloadSounds();
		}
	}

	protected void BGNBNDBDGGP(CopyItemInfo item, ModelParameters IHEFAMAFBIA)
	{
		ItemInfo dJKEECEOCJB = null;
		if (!string.IsNullOrEmpty(item.Name))
		{
			dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(item.Name).Clone();
		}
		else if (!string.IsNullOrEmpty(item.BLIKNEDFOFG) && KMMJCHDKBDO.KDABEFBJMOD(item.BLIKNEDFOFG) != null)
		{
			dJKEECEOCJB = KMMJCHDKBDO.KDABEFBJMOD(item.BLIKNEDFOFG).Clone();
		}
		if (dJKEECEOCJB != null)
		{
			dJKEECEOCJB.MergeWithItem(item);
			IHEFAMAFBIA.OLLNIKFPMKE(dJKEECEOCJB.Type, dJKEECEOCJB);
			ListSF.ELEBLBJKDBI().JLCGOODFKAK(dJKEECEOCJB);
		}
	}

	protected void UpdateCombo()
	{
		if (BFFLLGHDPEB != null)
		{
			BFFLLGHDPEB.UpdateCombo();
		}
		else
		{
			BNFCCKBIIDB.HHHDLDIHKBJ();
		}
	}

	public void KDJPMHGEPAF()
	{
		if (BFFLLGHDPEB != null)
		{
			BFFLLGHDPEB.KDJPMHGEPAF();
		}
		else
		{
			BNFCCKBIIDB.INNGMENHNEL();
		}
	}

	protected void FFBNDGBFEKE()
	{
		if (PJIHDNFHEGA && _Animation.NNMAFFCCMHC().FBKGDALBNDJ)
		{
			ApplyStrike(GHHCDAFIKJE);
		}
		PJIHDNFHEGA = false;
	}

	private void HEEHFLHNPOH(Model KKCCDBPOFOC, InfoAnimation DBOLBEOCEME, float CKKFKEIELCP, StrikeResult PPIAOBPLGOK)
	{
		Model fGCODGKLHED = BDJBNOPNCNB();
		Model fGCODGKLHED2 = KKCCDBPOFOC.BDJBNOPNCNB();
		fGCODGKLHED.DKFGOHCNIKL.LPCJBPFDFLD(true, DBOLBEOCEME, CKKFKEIELCP);
		fGCODGKLHED2.DKFGOHCNIKL.LPCJBPFDFLD(false, DBOLBEOCEME, CKKFKEIELCP);
		fGCODGKLHED2.DKFGOHCNIKL.AddRaidHitInfo(GHHCDAFIKJE.DFOHNJEBDED, GHHCDAFIKJE.DNGKOMPMPCD);
	}

	private bool BGOBFLIKMNN(StrikeResult PPIAOBPLGOK, Model HFGPAELCNMF)
	{
		if (_IsShock)
		{
			return false;
		}
		float num = GHHCDAFIKJE.EEDJBBOCFNL / HFGPAELCNMF.LJCFIOPBNKD();
		bool flag = SetPain((!HOOKPFLBFPD) ? num : 0f);
		ModelParameters kMMJCHDKBDO = HFGPAELCNMF.KMMJCHDKBDO;
		float nIPKAAEFMNG = GameUtils.APCAKCCOMLO.NIPKAAEFMNG;
		string aDAOLENDOME = GameUtils.APCAKCCOMLO.ADAOLENDOME;
		int OEMALIFPGPO = 0;
		kMMJCHDKBDO.IBLHIAHECLK.Get(aDAOLENDOME, ref OEMALIFPGPO);
		float num2 = nIPKAAEFMNG * (float)OEMALIFPGPO;
		float pAKGFJEEJLD = GameUtils.APCAKCCOMLO.PAKGFJEEJLD;
		string pOJAOGMJBDC = GameUtils.APCAKCCOMLO.POJAOGMJBDC;
		int OEMALIFPGPO2 = 0;
		kMMJCHDKBDO.IBLHIAHECLK.Get(pOJAOGMJBDC, ref OEMALIFPGPO2);
		float num3 = pAKGFJEEJLD * (float)OEMALIFPGPO2;
		bool flag2 = false;
		bool flag3 = false;
		if (GHHCDAFIKJE.DNGKOMPMPCD)
		{
			float num4 = num2 * num;
			float num5 = Random.Range(0f, 1f);
			flag2 = num4 > num5;
		}
		if (GHHCDAFIKJE.JMDIIIFJMFH && !GHHCDAFIKJE.DFOHNJEBDED)
		{
			float num6 = num3 * num;
			float num7 = Random.Range(0f, 1f);
			flag3 = num6 > num7;
		}
		return flag || flag3 || flag2;
	}

	private void DJLLAOJCOIM()
	{
		if (MDFEHKBOHEL.CNGALGBKFOK && MDFEHKBOHEL.KIKLFDLLDDP != MDFEHKBOHEL.PANKKFJFINL)
		{
			MDFEHKBOHEL.KIKLFDLLDDP += MDFEHKBOHEL.PANKKFJFINL / (float)(MDFEHKBOHEL.JKKENKDLJBK * GameUtils.GGBABPJBGJB());
			if (MDFEHKBOHEL.KIKLFDLLDDP > MDFEHKBOHEL.PANKKFJFINL)
			{
				MDFEHKBOHEL.KIKLFDLLDDP = MDFEHKBOHEL.PANKKFJFINL;
			}
			EventActBtnSettings eHCLMBADLKH = new EventActBtnSettings(FightCID.Punch, MDFEHKBOHEL.KIKLFDLLDDP, 1);
			CallEvent(12, eHCLMBADLKH);
		}
		if (MDFEHKBOHEL.MPIOLPLLFEM && MDFEHKBOHEL.CPKHGNDBKFL != MDFEHKBOHEL.AAIDLAFJECE)
		{
			MDFEHKBOHEL.CPKHGNDBKFL += MDFEHKBOHEL.AAIDLAFJECE / (float)(MDFEHKBOHEL.GKAPNAOFMFP * GameUtils.GGBABPJBGJB());
			if (MDFEHKBOHEL.CPKHGNDBKFL > MDFEHKBOHEL.AAIDLAFJECE)
			{
				MDFEHKBOHEL.CPKHGNDBKFL = MDFEHKBOHEL.AAIDLAFJECE;
			}
			EventActBtnSettings eHCLMBADLKH2 = new EventActBtnSettings(FightCID.Kick, MDFEHKBOHEL.CPKHGNDBKFL, 1);
			CallEvent(12, eHCLMBADLKH2);
		}
		if (MDFEHKBOHEL.MLDHFPCCCOP && MDFEHKBOHEL.DPMNMLHCJLK != MDFEHKBOHEL.MDOBBLKHOHI)
		{
			MDFEHKBOHEL.DPMNMLHCJLK += MDFEHKBOHEL.MDOBBLKHOHI / (float)(MDFEHKBOHEL.KFALPODCLFA * GameUtils.GGBABPJBGJB());
			if (MDFEHKBOHEL.DPMNMLHCJLK > MDFEHKBOHEL.MDOBBLKHOHI)
			{
				MDFEHKBOHEL.DPMNMLHCJLK = MDFEHKBOHEL.MDOBBLKHOHI;
			}
			EventActBtnSettings eHCLMBADLKH3 = new EventActBtnSettings(FightCID.MissileButton, MDFEHKBOHEL.DPMNMLHCJLK, 1);
			CallEvent(12, eHCLMBADLKH3);
		}
		if (MDFEHKBOHEL.FDHAJDFJBCF && MDFEHKBOHEL.IFMNJHFPDIC != MDFEHKBOHEL.GBEADNMMOID)
		{
			MDFEHKBOHEL.IFMNJHFPDIC += MDFEHKBOHEL.GBEADNMMOID / (float)(MDFEHKBOHEL.IKNPPGLMBDK * GameUtils.GGBABPJBGJB());
			if (MDFEHKBOHEL.IFMNJHFPDIC > MDFEHKBOHEL.GBEADNMMOID)
			{
				MDFEHKBOHEL.IFMNJHFPDIC = MDFEHKBOHEL.GBEADNMMOID;
			}
			EventActBtnSettings eHCLMBADLKH4 = new EventActBtnSettings(FightCID.RaidChargeButton, MDFEHKBOHEL.IFMNJHFPDIC, 1);
			CallEvent(12, eHCLMBADLKH4);
		}
	}
}
