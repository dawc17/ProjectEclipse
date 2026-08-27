using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class InfoAnimation
{
	public class MirrorNode
	{
		private KeyValuePair<string, string> _Names;

		private bool _Empty;

		public string BMFLPBLAFLK
		{
			get
			{
				return FJANLLCDPCP();
			}
		}

		public string OHLBOKNDEHN
		{
			get
			{
				return ADMAJAJNGBO();
			}
		}

		public bool OOPMAAHJMCE
		{
			get
			{
				return DAIAOBAEDCB();
			}
		}

		public MirrorNode()
		{
			_Names = new KeyValuePair<string, string>(string.Empty, string.Empty);
			_Empty = true;
		}

		public MirrorNode(string name)
		{
			_Empty = false;
			HHACPELEPAK(name);
		}

		public string FJANLLCDPCP()
		{
			return _Names.Key;
		}

		public string ADMAJAJNGBO()
		{
			return _Names.Value;
		}

		public bool DAIAOBAEDCB()
		{
			return _Empty;
		}

		public void HHACPELEPAK(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				string text = name;
				int length = name.Length;
				text.Remove(length - 1, 1);
				text += ((name[length - 1] != '1') ? "1" : "2");
				_Names = new KeyValuePair<string, string>(name, text);
				_Empty = false;
			}
			else
			{
				_Names = new KeyValuePair<string, string>(string.Empty, string.Empty);
				_Empty = true;
			}
		}
	}

	public class CapabilityTable
	{
		public List<InfoAnimation> NINJLLDJLFI = new List<InfoAnimation>();

		public bool IsThePriority(InfoAnimation DBOLBEOCEME)
		{
			for (int i = 0; i < NINJLLDJLFI.Count; i++)
			{
				if (NINJLLDJLFI[i] == DBOLBEOCEME)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsThePriority(List<InfoAnimation> MAHEJFLCCHP)
		{
			for (int i = 0; i < MAHEJFLCCHP.Count; i++)
			{
				if (!IsThePriority(MAHEJFLCCHP[i]))
				{
					return false;
				}
			}
			return true;
		}
	}

	public class AnimationContainerStruct
	{
		public Vector3[][] Container;

		public string FileName;
	}

	public enum BJMGJBGBLAL
	{
		WeaponHand = 0,
		WeaponThrowing = 1,
		AnimationMagic = 2
	}

	public enum MEACEGEJEAC
	{
		PivotNodeNone = 0,
		PivotNodeFront = 1,
		PivotNodeBack = 2
	}

	public enum MGHNBEPCKIF
	{
		AnimationNone = 0,
		AnimationMove = 1,
		AnimationAttack = 2
	}

	public enum EOJCAKOHCHA
	{
		TutorialNone = 0,
		TutorialMove = 1,
		TutorialAttack = 2
	}

	public enum DOLCEABGNGA
	{
		ObjectNone = 0,
		ObjectNodes = 1,
		ObjectWall = 2,
		ObjectAnimation = 3,
		ObjectPivot = 4
	}

	public class MovePivot
	{
		public DOLCEABGNGA CKBGFODEBAJ;

		public DOLCEABGNGA HHPAGAOGGLP;

		public int CLIPMJNJDKI = -1;

		public int BAHKGNNELBL = -1;

		public int JPKDOHPGEBA = -1;

		public int KFMGKDOLKGN = -1;

		public MEACEGEJEAC OLBDPMKCJIF;

		public bool HNDMMOGMOAN;

		public bool IMCDDINEFKC;

		public bool GHKGPDMMHHK;

		public string BLODCIGDJFK = string.Empty;

		public string PMILDGBBLMF = string.Empty;

		public string BONDKHGGCDD = string.Empty;

		public ModelType.KEIDBIOIFGA BAFGOANMBMI = ModelType.KEIDBIOIFGA.MODEL_THIS;

		public ModelType.KEIDBIOIFGA EDBLMNIEKBD = ModelType.KEIDBIOIFGA.MODEL_THIS;

		public Vector2f LDNPHPGEOPJ = new Vector2f();

		public bool IsExists;
	}

	public class MoveInside
	{
		public class ShopAnimation
		{
			public bool IsExists;

			public bool FGMBMNFANHF;

			public string AnimationName = string.Empty;
		}

		public class Direction
		{
			public bool IsExists;

			public DistancePoint CLCFLPDNBNL = new DistancePoint();

			public DistancePoint KAEAKHIEIHH = new DistancePoint();

			public DistancePoint.FKIAPHGNLKC IIIDIKABLOJ;

			public static DistancePoint.FKIAPHGNLKC BBAGKNMNONO(XmlNode node)
			{
				DistancePoint.FKIAPHGNLKC result = DistancePoint.FKIAPHGNLKC.IMPULSE_NONE;
				if (node != null && node.Name == "Impulse")
				{
					result = ((0 >= XmlUtils.ParseInt(node.Attributes["Reverse"])) ? DistancePoint.FKIAPHGNLKC.IMPULSE_NOT_REVERSE : DistancePoint.FKIAPHGNLKC.IMPULSE_REVERSE);
				}
				return result;
			}

			public int IMLFCBLAJGA(ModelConditions conditions)
			{
				float num = 0f;
				num = ((IIIDIKABLOJ == DistancePoint.FKIAPHGNLKC.IMPULSE_NONE) ? (KAEAKHIEIHH.ILIKNABGPNK(conditions) - CLCFLPDNBNL.ILIKNABGPNK(conditions)) : ((float)((IIIDIKABLOJ != DistancePoint.FKIAPHGNLKC.IMPULSE_NOT_REVERSE) ? (conditions.BOECCPNHAII * -1) : conditions.BOECCPNHAII)));
				return (num >= 0f) ? 1 : (-1);
			}
		}

		public List<EventAnimation> AJCMBMJGJEG = new List<EventAnimation>();

		public List<ConditionAnimation> JIFAHHGNPFH = new List<ConditionAnimation>();

		public List<ConditionAnimation> NIDNJFOGBFO = new List<ConditionAnimation>();

		public List<IntervalAnimation> Intervals = new List<IntervalAnimation>();

		public List<ConditionAnimation> HIFPHBNGIPO = new List<ConditionAnimation>();

		public List<TransitionAnimation> ELFBPNOBDKC = new List<TransitionAnimation>();

		public List<ActionAnimation> DJBAIAKOIHM = new List<ActionAnimation>();

		public ShopAnimation DFLNENOIMPO = new ShopAnimation();

		public MovePivot ILOEBFFAEAN = new MovePivot();

		public Direction IHJEKBAEIKK = new Direction();

		public void JGMGIHIBFKA()
		{
			for (int i = 0; i < Intervals.Count; i++)
			{
				Intervals[i].Init();
			}
		}

		public void NKHGGBMOADI()
		{
			string bLODCIGDJFK = ILOEBFFAEAN.BLODCIGDJFK;
			if (bLODCIGDJFK != null && bLODCIGDJFK.Length > 2)
			{
				int num = bLODCIGDJFK.Length - 1;
				char c = bLODCIGDJFK[num];
				char c2 = bLODCIGDJFK[num - 1];
				if (c == '1' && c2 == '_')
				{
					ILOEBFFAEAN.OLBDPMKCJIF = MEACEGEJEAC.PivotNodeFront;
				}
				else if (c == '2' && c2 == '_')
				{
					ILOEBFFAEAN.OLBDPMKCJIF = MEACEGEJEAC.PivotNodeBack;
				}
			}
		}

		public EventAnimation OIGBIFNICBI(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
		{
			for (int i = 0; i < AJCMBMJGJEG.Count; i++)
			{
				if (AJCMBMJGJEG[i].Type == LFLGCDNKNJI)
				{
					return AJCMBMJGJEG[i];
				}
			}
			return null;
		}
	}

	protected List<List<global::Pair<List<GroupTables>, string>>> PNBAAKIIDGG;

	public ModelShiftTable OBIBINIEJJE = new ModelShiftTable();

	public MGHNBEPCKIF Type;

	public EOJCAKOHCHA OFADIIPBEKI;

	public int MNHGBPOIHKG;

	public int Priority;

	public int Rank;

	public int Id;

	public int GOBJCKFGIPA;

	public int LHHAGECFIOL;

	public string Name;

	public string FileName;

	public MoveInside ODACDCDONJE;

	public bool HFBOLCPHMBB;

	public bool FBKGDALBNDJ;

	public bool NHNEJKIBPJG;

	public bool HECHJGBMHIC;

	public bool JEADCBJMEGC;

	public float DCLGDANCGHC;

	public bool ALFPDPEEJFO;

	private List<string> _TemplateNames = new List<string>();

	private List<int> _Delays;

	private int _NodesCount;

	private List<string> EGDIEIPCAAF = new List<string>();

	private Vector3f KACPFNLDNND;

	private Vector3f KNBDGOJAIAF;

	private bool AEDIIEEJKHE;

	private bool INFAGPDFGNL;

	private DistancePoint OJGFJBFBCAP;

	private float KPEMEDJCIIB;

	private bool JCIKOMAMJDI;

	private StageType.FDBBPEGEGMK ENDJLOAGKGO;

	private MirrorNode FDECJHIMNGN = new MirrorNode();

	private Vector3[][] _AnimationContainer;

	public CapabilityTable ICANLHJKKNE = new CapabilityTable();

	private static readonly List<AnimationContainerStruct> LECLDGFPOEA = new List<AnimationContainerStruct>();

	private InfoAnimation _TacticEquivalent;

	public List<List<global::Pair<List<GroupTables>, string>>> DFJMOIDKKOB
	{
		get
		{
			return NLCLHLIPFFH();
		}
	}

	public int OKDGCCPGLMC
	{
		get
		{
			return DFKIHADCFKG();
		}
	}

	public Vector3[][] MMICPIJAFHA
	{
		get
		{
			return DIHJOPGKGFO();
		}
	}

	public string NCKBFHLNKDD
	{
		get
		{
			return KPIMAMCOEAN();
		}
	}

	public List<IntervalAnimation> GDAGDHGLKPB
	{
		get
		{
			return PCKKMNHDDMP();
		}
	}

	public List<string> LANPOMAOOIM
	{
		get
		{
			return FOLOOGCLPNE();
		}
	}

	public int NFKBFGIACOP
	{
		get
		{
			return PGOFHCBPLOE();
		}
	}

	public uint BOLFNFOHJMA
	{
		get
		{
			return BMBKLLNAKJK();
		}
	}

	public ConditionKeys OHOPGOOAEOJ
	{
		get
		{
			return ILBCHANCOBP();
		}
	}

	public List<ConditionKeys> ODCILHKPLAF
	{
		get
		{
			return MOPMGFIIFGA();
		}
	}

	public InfoAnimation JCMFPPJIIIF
	{
		get
		{
			return IMFGMAAEMIC();
		}
		set
		{
			set_TacticEquivalent(value);
		}
	}

	public List<string> DBIKOIDEGGA
	{
		get
		{
			return OIDIJEOMJCB();
		}
	}

	public bool ALMDIDLDGGE
	{
		get
		{
			return AIHDFOPLBIL();
		}
	}

	public bool BPOJBMBEHOB
	{
		get
		{
			return LIKPDIIPABF();
		}
	}

	public Vector3f AIPEIJLMMPH
	{
		get
		{
			return LBJFGCFGMDI();
		}
		set
		{
			DIGCECPPHOH(value);
		}
	}

	public Vector3f BLEMEODBIIM
	{
		get
		{
			return NCENGIOMKOF();
		}
		set
		{
			PICBLJDLDDN(value);
		}
	}

	public bool KDMHCOAAJBM
	{
		get
		{
			return HOPDDLNABCG();
		}
		set
		{
			NFMLONEIJEJ(value);
		}
	}

	public DistancePoint DCMFBGCBCBM
	{
		get
		{
			return KBLFKMECMJP();
		}
		set
		{
			HGJPLKKCKHM(value);
		}
	}

	public bool FGFCNGPALBO
	{
		get
		{
			return BKGIEPOEBOF();
		}
		set
		{
			PFELBJBNEEK(value);
		}
	}

	public float AIBCIHIKBMN
	{
		get
		{
			return NBOLIGLFFEL();
		}
		set
		{
			set_RotationAngle(value);
		}
	}

	public int CPHMBEBIMII
	{
		get
		{
			return ONLKMFOENEH();
		}
	}

	public int FCHEONGLDCL
	{
		get
		{
			return EDDFIABEAGM();
		}
	}

	public bool LBGCFNKKJJL
	{
		get
		{
			return NCEKKNIMHAG();
		}
		set
		{
			LLELLFKJKGE(value);
		}
	}

	public StageType.FDBBPEGEGMK ENHFGKNDOHI
	{
		get
		{
			return PHPHCKAHPOP();
		}
		set
		{
			POOOFPBAJDM(value);
		}
	}

	public MirrorNode GEIHPPOIONJ
	{
		get
		{
			return ECCLELFHNHE();
		}
		set
		{
			NNFKIGLFLKL(value);
		}
	}

	public Vector3[] IFHBEAOHOCI
	{
		get
		{
			return BGHLLHNKFEM();
		}
	}

	public static int MIGJEFHKGOK
	{
		get
		{
			return NJKKOFDBMOO();
		}
	}

	private AnimationContainerStruct NIFLBOLNLII
	{
		get
		{
			return IAPAKFDEKOI();
		}
	}

	public InfoAnimation()
	{
		Type = MGHNBEPCKIF.AnimationNone;
		OFADIIPBEKI = EOJCAKOHCHA.TutorialNone;
		MNHGBPOIHKG = 0;
		Priority = 0;
		Id = 0;
		GOBJCKFGIPA = 0;
		LHHAGECFIOL = 0;
		FBKGDALBNDJ = false;
		HECHJGBMHIC = false;
		JEADCBJMEGC = false;
		NHNEJKIBPJG = true;
		_TacticEquivalent = null;
		AEDIIEEJKHE = false;
		ENDJLOAGKGO = StageType.FDBBPEGEGMK.STAGE_NONE;
		JCIKOMAMJDI = false;
		ODACDCDONJE = new MoveInside();
		HFBOLCPHMBB = false;
		_NodesCount = 0;
		_AnimationContainer = null;
		INFAGPDFGNL = false;
		KPEMEDJCIIB = 0f;
		Rank = 0;
		ODACDCDONJE.ILOEBFFAEAN = new MovePivot();
		ODACDCDONJE.ILOEBFFAEAN.IsExists = false;
		ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI = -1;
		ODACDCDONJE.IHJEKBAEIKK.IsExists = false;
	}

	public List<List<global::Pair<List<GroupTables>, string>>> NLCLHLIPFFH()
	{
		if (PNBAAKIIDGG == null)
		{
			PNBAAKIIDGG = new List<List<global::Pair<List<GroupTables>, string>>>();
			for (int i = 0; i < 3; i++)
			{
				PNBAAKIIDGG.Add(new List<global::Pair<List<GroupTables>, string>>());
			}
		}
		return PNBAAKIIDGG;
	}

	public int DFKIHADCFKG()
	{
		return _NodesCount;
	}

	public Vector3[][] DIHJOPGKGFO()
	{
		return _AnimationContainer;
	}

	public void SetCurrentNode()
	{
	}

	public void Init()
	{
		ODACDCDONJE.JGMGIHIBFKA();
		ODACDCDONJE.NKHGGBMOADI();
		if (_AnimationContainer != null && _AnimationContainer.Length > 0)
		{
			_NodesCount = _AnimationContainer[0].Length;
		}
	}

	public bool CheckLockAnimation(string JKBPMGDJIJC, bool LPGLCGMMPHN = true)
	{
		return true;
	}

	public string KPIMAMCOEAN()
	{
		if (ODACDCDONJE == null)
		{
			LLLOJBFMONN.Error("moveInside is null");
			return string.Empty;
		}
		return ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK;
	}

	public void GetIntervals(int frame, List<IntervalAnimation> NKHPLNBJKLI, List<IntervalAnimation> HLMKBLOHJGC, HashSet<IntervalAnimation.NGAJJDIEDGF> FGBOFDJKLJI = null)
	{
		NKHPLNBJKLI.Clear();
		HLMKBLOHJGC.Clear();
		foreach (IntervalAnimation item in ODACDCDONJE.Intervals)
		{
			int num = ((item.Start < GOBJCKFGIPA) ? GOBJCKFGIPA : item.Start);
			int num2 = ((item.GEJLNPIEDPF > LHHAGECFIOL) ? LHHAGECFIOL : item.GEJLNPIEDPF);
			if (num <= frame && frame <= num2)
			{
				if (FGBOFDJKLJI == null || !FGBOFDJKLJI.Contains(item.Type))
				{
					NKHPLNBJKLI.Add(item);
				}
			}
			else if (frame - 1 == num2 && (FGBOFDJKLJI == null || !FGBOFDJKLJI.Contains(item.Type)))
			{
				HLMKBLOHJGC.Add(item);
			}
		}
	}

	public List<IntervalAnimation> PCKKMNHDDMP()
	{
		return ODACDCDONJE.Intervals;
	}

	public bool JBALJDEOGNK(EventAnimation p_event)
	{
		foreach (EventAnimation item in ODACDCDONJE.AJCMBMJGJEG)
		{
			if (item.IsEqual(p_event))
			{
				return true;
			}
		}
		return false;
	}

	public bool HPPGNJJCEGF(ModelConditions conditions, List<ConditionAnimation> JPGMNIFICDM = null, EventAnimation DOANBADPBGH = null)
	{
		List<ConditionAnimation> list = ((JPGMNIFICDM == null) ? ODACDCDONJE.JIFAHHGNPFH : JPGMNIFICDM);
		if (DOANBADPBGH != null)
		{
			conditions.HFCIDBJJINB = DOANBADPBGH;
			DOANBADPBGH.JIFAHHGNPFH = conditions;
		}
		foreach (ConditionAnimation item in list)
		{
			if (!item.IsEqual(conditions))
			{
				return false;
			}
		}
		return true;
	}

	public bool HPPGNJJCEGF(Model ACENLMONNPA, List<ConditionAnimation> JPGMNIFICDM = null, EventAnimation DOANBADPBGH = null)
	{
		List<ConditionAnimation> list = ((JPGMNIFICDM == null) ? ODACDCDONJE.JIFAHHGNPFH : JPGMNIFICDM);
		for (int i = 0; i < list.Count; i++)
		{
			ConditionAnimation iIDOLPHMOGA = list[i];
			ModelType.KEIDBIOIFGA kEIDBIOIFGA = iIDOLPHMOGA.FHBAPKNECOM();
			Model fGCODGKLHED = iIDOLPHMOGA.DKDAKGDMHAL(ACENLMONNPA, kEIDBIOIFGA);
			if (fGCODGKLHED == null)
			{
				return false;
			}
			ModelConditions dGJJDPIAEAO = fGCODGKLHED.EBABHGHPLFK();
			if (DOANBADPBGH != null)
			{
				dGJJDPIAEAO.HFCIDBJJINB = DOANBADPBGH;
				DOANBADPBGH.JIFAHHGNPFH = dGJJDPIAEAO;
			}
			iIDOLPHMOGA.MJFKNEHGNMB(ModelType.KEIDBIOIFGA.MODEL_THIS);
			bool flag = false;
			if (iIDOLPHMOGA.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = iIDOLPHMOGA as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					flag = eLFKOGJJNMN.DJEJMGCMPPH(ACENLMONNPA.EBABHGHPLFK(), ACENLMONNPA, DOANBADPBGH);
				}
			}
			else
			{
				flag = iIDOLPHMOGA.IsEqual(fGCODGKLHED, this);
			}
			if (!flag)
			{
				iIDOLPHMOGA.GNPMNEDOFPB(kEIDBIOIFGA);
				return false;
			}
			iIDOLPHMOGA.GNPMNEDOFPB(kEIDBIOIFGA);
		}
		return true;
	}

	public void FNGJFDNAPPH(List<global::Pair<int, int>> HMKIJOIJNJD, KeyFrames GCDAKGKMJHF, int index)
	{
		if (0 >= HMKIJOIJNJD.Count || index >= GCDAKGKMJHF.OLINNGEMHMG())
		{
			return;
		}
		int dGILPMANFAF = GCDAKGKMJHF.KLNOLPIADNN(index).Size;
		Vector3f eMAFACPEPDK = new Vector3f();
		for (int i = index; i < GCDAKGKMJHF.OLINNGEMHMG(); i++)
		{
			for (int j = 0; j < HMKIJOIJNJD.Count; j++)
			{
				if (HMKIJOIJNJD[j].First < dGILPMANFAF && HMKIJOIJNJD[j].Second < dGILPMANFAF)
				{
					eMAFACPEPDK.Set(GCDAKGKMJHF.KLNOLPIADNN(i).Data[HMKIJOIJNJD[j].First]);
					GCDAKGKMJHF.KLNOLPIADNN(i).Data[HMKIJOIJNJD[j].First].Set(GCDAKGKMJHF.KLNOLPIADNN(i).Data[HMKIJOIJNJD[j].Second]);
					GCDAKGKMJHF.KLNOLPIADNN(i).Data[HMKIJOIJNJD[j].Second].Set(eMAFACPEPDK);
				}
			}
		}
	}

	public void HAILLLEPCHP(KeyFrames frames, int NHEIOIBOPHN, bool HOHEFHKJIOG)
	{
		int num = ((NHEIOIBOPHN <= -1) ? GOBJCKFGIPA : NHEIOIBOPHN);
		int num2 = _AnimationContainer[num].Length;
		frames.HAILLLEPCHP(num, LHHAGECFIOL, HOHEFHKJIOG, _AnimationContainer);
	}

	public void ABEGFBOKPOI()
	{
		if (!string.IsNullOrEmpty(FileName))
		{
			AnimationContainerStruct aGAMDIHPFPF = IAPAKFDEKOI();
			if (aGAMDIHPFPF == null)
			{
				string iFKJHHPJPLP = SF2Paths.CBKLONCNPCP() + "/" + FileName;
				OOICKIBOFGH(iFKJHHPJPLP);
				DDPBDPEDIGC();
			}
			else
			{
				BAIMGDMKILA(aGAMDIHPFPF);
			}
		}
	}

	public void AddTemplateName(string name)
	{
		_TemplateNames.AddIfNotExist(name);
	}

	public void AddDelay(int value)
	{
		_Delays.Add(value);
	}

	public void NHAEHLFMPNK(MoveInside KECIIKEIJBH)
	{
		if (ODACDCDONJE != null)
		{
			ACGIFMKPBGC(KECIIKEIJBH.AJCMBMJGJEG);
			CPKDGKCHOJJ(KECIIKEIJBH.NIDNJFOGBFO);
			CHDLHMGPDHL(KECIIKEIJBH.JIFAHHGNPFH);
			GAKOLFJGLMM(KECIIKEIJBH.Intervals);
			OFMGLKAGCGO(KECIIKEIJBH.HIFPHBNGIPO);
			PGMIJCNNJAG(KECIIKEIJBH.ELFBPNOBDKC);
			FGAEEJBEGEJ(KECIIKEIJBH.DJBAIAKOIHM);
			if (!ODACDCDONJE.DFLNENOIMPO.IsExists && KECIIKEIJBH.DFLNENOIMPO.IsExists)
			{
				ODACDCDONJE.DFLNENOIMPO = KECIIKEIJBH.DFLNENOIMPO;
			}
			if (!ODACDCDONJE.ILOEBFFAEAN.IsExists && KECIIKEIJBH.ILOEBFFAEAN.IsExists)
			{
				ODACDCDONJE.ILOEBFFAEAN = KECIIKEIJBH.ILOEBFFAEAN;
			}
			if (!ODACDCDONJE.IHJEKBAEIKK.IsExists && KECIIKEIJBH.IHJEKBAEIKK.IsExists)
			{
				ODACDCDONJE.IHJEKBAEIKK = KECIIKEIJBH.IHJEKBAEIKK;
			}
		}
	}

	public void OFMGLKAGCGO(List<ConditionAnimation> value)
	{
		ODACDCDONJE.HIFPHBNGIPO.AddRange(value);
	}

	public void PGMIJCNNJAG(List<TransitionAnimation> value)
	{
		ODACDCDONJE.ELFBPNOBDKC.AddRange(value);
	}

	public void FGAEEJBEGEJ(List<ActionAnimation> value)
	{
		ODACDCDONJE.DJBAIAKOIHM.AddRange(value);
	}

	public void ACGIFMKPBGC(List<EventAnimation> value)
	{
		ODACDCDONJE.AJCMBMJGJEG.AddRange(value);
	}

	public void CPKDGKCHOJJ(List<ConditionAnimation> value)
	{
		ODACDCDONJE.NIDNJFOGBFO.AddRange(value);
	}

	public void CHDLHMGPDHL(List<ConditionAnimation> value)
	{
		ODACDCDONJE.JIFAHHGNPFH.AddRange(value);
	}

	public void GAKOLFJGLMM(List<IntervalAnimation> value)
	{
		foreach (IntervalAnimation item in value)
		{
			item.set_AnimationFinishFrame(LHHAGECFIOL);
		}
		ODACDCDONJE.Intervals.AddRange(value);
	}

	private void OOICKIBOFGH(string path)
	{
		byte[] array = ResourceManager.GetBinary(path);
		if (array != null && array.Length > 0)
		{
			ReadAnimation(array);
			return;
		}
		LLLOJBFMONN.Error("File {0} not found", path);
	}

	private void ReadAnimation(byte[] data)
	{
		using (BinaryReaderNekki pHAPKCOJMHL = new BinaryReaderNekki(data))
		{
			int num = pHAPKCOJMHL.GDFKNFAHHKF();
			_AnimationContainer = new Vector3[num][];
			for (int i = 0; i < num; i++)
			{
				pHAPKCOJMHL.ReadByte();
				int num2 = pHAPKCOJMHL.GDFKNFAHHKF();
				_AnimationContainer[i] = new Vector3[num2];
				for (int j = 0; j < num2; j++)
				{
					_AnimationContainer[i][j] = new Vector3(pHAPKCOJMHL.MMJAOEBFCLN(), 0f - pHAPKCOJMHL.MMJAOEBFCLN(), pHAPKCOJMHL.MMJAOEBFCLN());
				}
			}
			if (LHHAGECFIOL == 0)
			{
				LHHAGECFIOL = num - 1;
			}
		}
	}

	private void BAIMGDMKILA(AnimationContainerStruct EIJNHOPFLGI)
	{
		_AnimationContainer = EIJNHOPFLGI.Container;
		int num = _AnimationContainer.Length;
		if (LHHAGECFIOL == 0)
		{
			LHHAGECFIOL = num - 1;
		}
	}

	public void BPHNHFJCFCD(ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, bool PHADJMAONJG, ModelObject MJCGOJBGFIE = null)
	{
		ModelNode aECCPADGGPG = null;
		if (ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI > -1 && ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI < OECPEDPMKCD.LMBNDIPLBJA().Count)
		{
			aECCPADGGPG = OECPEDPMKCD.LMBNDIPLBJA()[ODACDCDONJE.ILOEBFFAEAN.CLIPMJNJDKI];
		}
		UpdateConditions(ODACDCDONJE.JIFAHHGNPFH, OECPEDPMKCD, EKBOGDKIHIH, PHADJMAONJG, MJCGOJBGFIE, aECCPADGGPG);
		if (0 < ODACDCDONJE.NIDNJFOGBFO.Count)
		{
			UpdateConditions(ODACDCDONJE.NIDNJFOGBFO, OECPEDPMKCD, EKBOGDKIHIH, PHADJMAONJG, MJCGOJBGFIE, aECCPADGGPG);
		}
		ODACDCDONJE.IHJEKBAEIKK.CLCFLPDNBNL.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, null, PHADJMAONJG, MJCGOJBGFIE);
		ODACDCDONJE.IHJEKBAEIKK.KAEAKHIEIHH.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, null, PHADJMAONJG, MJCGOJBGFIE);
		if (KPEMEDJCIIB != 0f)
		{
			OJGFJBFBCAP.UpdateNode(OECPEDPMKCD, EKBOGDKIHIH, null, PHADJMAONJG, MJCGOJBGFIE);
		}
		foreach (ActionAnimation item in ODACDCDONJE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				jFJGGMEJDPG.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, null, PHADJMAONJG, MJCGOJBGFIE);
			}
		}
	}

	private void UpdateConditions(List<ConditionAnimation> conditions, ModelObject OECPEDPMKCD, bool EKBOGDKIHIH, bool PHADJMAONJG, ModelObject MJCGOJBGFIE, ModelNode AECCPADGGPG)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.DISTANCE)
			{
				ConditionDistance jNPIBKBDJAN = item as ConditionDistance;
				if (jNPIBKBDJAN != null)
				{
					jNPIBKBDJAN.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
				}
				else
				{
					LLLOJBFMONN.Error("subcondition is null");
				}
			}
			if (item.Type == ConditionAnimation.DGAGKLODADD.DIRECTION)
			{
				ConditionDirection cFCGJLJBOKI = item as ConditionDirection;
				if (cFCGJLJBOKI != null)
				{
					cFCGJLJBOKI.KJHPCLOFDJB(OECPEDPMKCD, EKBOGDKIHIH, AECCPADGGPG, PHADJMAONJG, MJCGOJBGFIE);
				}
				else
				{
					LLLOJBFMONN.Error("subcondition is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					UpdateConditions(kDOGKKGDOBK, OECPEDPMKCD, EKBOGDKIHIH, PHADJMAONJG, MJCGOJBGFIE, AECCPADGGPG);
				}
				else
				{
					LLLOJBFMONN.Error("subconditions is null");
				}
			}
		}
	}

	private void CJAPHCKAOIE(List<ConditionAnimation> AIDMEPEKEOL)
	{
		foreach (ConditionAnimation item in AIDMEPEKEOL)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.DISTANCE)
			{
				ConditionDistance jNPIBKBDJAN = item as ConditionDistance;
				if (jNPIBKBDJAN != null)
				{
					jNPIBKBDJAN.ABNCNNHMLII();
				}
				else
				{
					LLLOJBFMONN.Error("conditionDistance is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.DIRECTION)
			{
				ConditionDirection cFCGJLJBOKI = item as ConditionDirection;
				if (cFCGJLJBOKI != null)
				{
					cFCGJLJBOKI.ABNCNNHMLII();
				}
				else
				{
					LLLOJBFMONN.Error("conditionDistance is null");
				}
			}
			else if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> aIDMEPEKEOL = eLFKOGJJNMN.KJILOMLMMEN();
					CJAPHCKAOIE(aIDMEPEKEOL);
				}
				else
				{
					LLLOJBFMONN.Error("conditions is null");
				}
			}
		}
	}

	public void ABNCNNHMLII()
	{
		CJAPHCKAOIE(ODACDCDONJE.JIFAHHGNPFH);
		CJAPHCKAOIE(ODACDCDONJE.NIDNJFOGBFO);
		foreach (ActionAnimation item in ODACDCDONJE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				jFJGGMEJDPG.MGCNPBCBMHB();
			}
		}
		ODACDCDONJE.IHJEKBAEIKK.CLCFLPDNBNL.GPGKANDFLNB();
		ODACDCDONJE.IHJEKBAEIKK.KAEAKHIEIHH.GPGKANDFLNB();
		if (OJGFJBFBCAP != null)
		{
			OJGFJBFBCAP.GPGKANDFLNB();
		}
	}

	public List<string> FOLOOGCLPNE()
	{
		return _TemplateNames;
	}

	public int CEDEDCLGJDE(ModelConditions conditions, int CLHNIJGMKBH)
	{
		return (!ODACDCDONJE.IHJEKBAEIKK.IsExists) ? CLHNIJGMKBH : ODACDCDONJE.IHJEKBAEIKK.IMLFCBLAJGA(conditions);
	}

	public int PGOFHCBPLOE()
	{
		return LHHAGECFIOL - GOBJCKFGIPA + 1;
	}

	public uint BMBKLLNAKJK()
	{
		return (uint)(PGOFHCBPLOE() * (MNHGBPOIHKG + 1));
	}

	public int MLLLLMFLOBG(bool NPEIEAHIDKH)
	{
		int num = 0;
		foreach (IntervalAnimation item in ODACDCDONJE.Intervals)
		{
			if (item.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK && num < item.GEJLNPIEDPF)
			{
				num = item.GEJLNPIEDPF;
			}
		}
		if (NPEIEAHIDKH)
		{
			num = DKEJBCMFJEI(num + 1) - 1;
		}
		return num;
	}

	public bool IsItemRequired(string LMNNBBKHMEI, string OCOEFJAMFCG)
	{
		List<ConditionAnimation> hIFPHBNGIPO = ODACDCDONJE.HIFPHBNGIPO;
		foreach (ConditionAnimation item in hIFPHBNGIPO)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.ITEM && !item.IsNot)
			{
				ConditionItemInfo kOOGCJOEANH = item as ConditionItemInfo;
				if (LMNNBBKHMEI == kOOGCJOEANH.get_Type() && OCOEFJAMFCG == kOOGCJOEANH.EAIMKPPOODM())
				{
					return true;
				}
			}
			else
			{
				if (item.Type != ConditionAnimation.DGAGKLODADD.LIST || item.IsNot)
				{
					continue;
				}
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN.get_Type() != ConditionList.PJDDCKKJBNB.OR)
				{
					LLLOJBFMONN.Error(string.Empty);
					continue;
				}
				List<ConditionAnimation> list = eLFKOGJJNMN.KJILOMLMMEN();
				foreach (ConditionAnimation item2 in list)
				{
					if (item2.Type == ConditionAnimation.DGAGKLODADD.ITEM && !item2.IsNot)
					{
						ConditionItemInfo kOOGCJOEANH2 = item2 as ConditionItemInfo;
						if (LMNNBBKHMEI == kOOGCJOEANH2.get_Type() && OCOEFJAMFCG == kOOGCJOEANH2.EAIMKPPOODM())
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public int GetMoveLength(List<string> NFLDEGMEJAK)
	{
		int num = 0;
		foreach (IntervalAnimation item in ODACDCDONJE.Intervals)
		{
			if (NFLDEGMEJAK.Contains(item.Name) && num < item.GEJLNPIEDPF)
			{
				num = item.GEJLNPIEDPF;
			}
		}
		if (0 < num)
		{
			return DKEJBCMFJEI(num + 1);
		}
		return 0;
	}

	public int HGMPJJACFHN()
	{
		List<string> nFLDEGMEJAK = AiData.get_MoveLengthIntervalsStrict();
		return GetMoveLength(nFLDEGMEJAK);
	}

	public int JMIDABBAKEP()
	{
		List<string> nFLDEGMEJAK = AiData.get_MoveLengthIntervalsExtended();
		return GetMoveLength(nFLDEGMEJAK);
	}

	public ConditionKeys ILBCHANCOBP()
	{
		return DHBACBKLADO(ODACDCDONJE.JIFAHHGNPFH);
	}

	private static ConditionKeys DHBACBKLADO(List<ConditionAnimation> conditions)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					ConditionKeys bHDEBDIHDFM = DHBACBKLADO(kDOGKKGDOBK);
					if (bHDEBDIHDFM != null)
					{
						return bHDEBDIHDFM;
					}
				}
				else
				{
					LLLOJBFMONN.Error("conditionList is null");
				}
			}
			else
			{
				ConditionKeys bHDEBDIHDFM2 = JEELAPHJLOE(item);
				if (bHDEBDIHDFM2 != null)
				{
					return bHDEBDIHDFM2;
				}
			}
		}
		return null;
	}

	public List<ConditionKeys> MOPMGFIIFGA()
	{
		List<ConditionKeys> list = new List<ConditionKeys>();
		CIEHMPCOKGK(ODACDCDONJE.JIFAHHGNPFH, list);
		return list;
	}

	private static void CIEHMPCOKGK(List<ConditionAnimation> conditions, List<ConditionKeys> GKHEPKGMEFI)
	{
		foreach (ConditionAnimation item in conditions)
		{
			if (item.Type == ConditionAnimation.DGAGKLODADD.LIST)
			{
				ConditionList eLFKOGJJNMN = item as ConditionList;
				if (eLFKOGJJNMN != null)
				{
					List<ConditionAnimation> kDOGKKGDOBK = eLFKOGJJNMN.KJILOMLMMEN();
					CIEHMPCOKGK(kDOGKKGDOBK, GKHEPKGMEFI);
				}
				else
				{
					LLLOJBFMONN.Error("conditionList is null");
				}
			}
			else
			{
				ConditionKeys bHDEBDIHDFM = JEELAPHJLOE(item);
				if (bHDEBDIHDFM != null)
				{
					GKHEPKGMEFI.Add(bHDEBDIHDFM);
				}
			}
		}
	}

	public static ConditionKeys JEELAPHJLOE(ConditionAnimation IOFGGOCEIAM)
	{
		if (IOFGGOCEIAM.Type == ConditionAnimation.DGAGKLODADD.KEYS)
		{
			return IOFGGOCEIAM as ConditionKeys;
		}
		return null;
	}

	public bool CNPFHBMGDFP(string name)
	{
		return Name == name || LPPIKDGABOL(name);
	}

	public bool LPPIKDGABOL(string IJBOAGICOON)
	{
		foreach (string item in _TemplateNames)
		{
			if (item == IJBOAGICOON)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ComparePrioritets(InfoAnimation LHBNIMGFKIB, InfoAnimation AAOIAEJJINO)
	{
		return LHBNIMGFKIB.Priority < AAOIAEJJINO.Priority;
	}

	public bool CheckAnimationName(List<string> IPFMIJKPABH)
	{
		foreach (string item in _TemplateNames)
		{
			foreach (string item2 in IPFMIJKPABH)
			{
				if (item == item2)
				{
					return true;
				}
			}
		}
		return false;
	}

	public InfoAnimation IMFGMAAEMIC()
	{
		return _TacticEquivalent;
	}

	public void set_TacticEquivalent(InfoAnimation value)
	{
		if (this == value)
		{
			LLLOJBFMONN.Error("this animation == tactic equivalent for {0}", Name);
		}
		_TacticEquivalent = value;
	}

	public List<string> OIDIJEOMJCB()
	{
		return EGDIEIPCAAF;
	}

	public void IBMFCIFKGOO(string INFFOHGHLNG)
	{
		EGDIEIPCAAF.Clear();
		if (INFFOHGHLNG != null)
		{
			EGDIEIPCAAF.AddRange(INFFOHGHLNG.Split('|'));
		}
	}

	public bool AIHDFOPLBIL()
	{
		return ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK == "NHeel_2";
	}

	public bool LIKPDIIPABF()
	{
		return ODACDCDONJE.ILOEBFFAEAN.BLODCIGDJFK == "NHeel_1";
	}

	public int IKFCNCLKDGD(bool NPEIEAHIDKH)
	{
		int num = 0;
		foreach (IntervalAnimation item in ODACDCDONJE.Intervals)
		{
			if ("Uninterrupt" == item.Name && num < item.GEJLNPIEDPF)
			{
				num = item.GEJLNPIEDPF;
			}
		}
		int lHHAGECFIOL = LHHAGECFIOL;
		if (lHHAGECFIOL < num)
		{
			num = lHHAGECFIOL;
		}
		if (NPEIEAHIDKH)
		{
			num = DKEJBCMFJEI(num + 1) - 1;
		}
		return num;
	}

	public int DKEJBCMFJEI(int frame)
	{
		return (frame - GOBJCKFGIPA + 1) * (MNHGBPOIHKG + 1) + 1;
	}

	public int FALLOLJPMGF(int IHICCKAOPKG)
	{
		return GOBJCKFGIPA - 1 + (IHICCKAOPKG - 1) / (MNHGBPOIHKG + 1);
	}

	public void DIGCECPPHOH(Vector3f value)
	{
		KACPFNLDNND = value;
	}

	public Vector3f LBJFGCFGMDI()
	{
		return KACPFNLDNND;
	}

	public void PICBLJDLDDN(Vector3f value)
	{
		KNBDGOJAIAF = value;
	}

	public Vector3f NCENGIOMKOF()
	{
		return KNBDGOJAIAF;
	}

	public void NFMLONEIJEJ(bool value)
	{
		AEDIIEEJKHE = value;
	}

	public bool HOPDDLNABCG()
	{
		return AEDIIEEJKHE;
	}

	public void HGJPLKKCKHM(DistancePoint value)
	{
		OJGFJBFBCAP = value;
	}

	public DistancePoint KBLFKMECMJP()
	{
		return OJGFJBFBCAP;
	}

	public void PFELBJBNEEK(bool value)
	{
		JCIKOMAMJDI = value;
	}

	public bool BKGIEPOEBOF()
	{
		return JCIKOMAMJDI;
	}

	public void set_RotationAngle(float value)
	{
		KPEMEDJCIIB = value;
	}

	public float NBOLIGLFFEL()
	{
		return KPEMEDJCIIB;
	}

	public bool MBENIPEBGBK(string name, int frame)
	{
		foreach (IntervalAnimation item in ODACDCDONJE.Intervals)
		{
			if (item.Name == name && item.Start <= frame && frame <= item.GEJLNPIEDPF)
			{
				return true;
			}
		}
		return false;
	}

	public bool CJAHEDOHHEG(string name, int IHICCKAOPKG)
	{
		int dBEDGEMEFNB = FALLOLJPMGF(IHICCKAOPKG);
		return MBENIPEBGBK(name, dBEDGEMEFNB);
	}

	public bool EHEPILCDIDC(List<string> NIKHAICFGNM, int frame, bool FPMGBALCKPI)
	{
		if (FPMGBALCKPI)
		{
			foreach (string item in NIKHAICFGNM)
			{
				bool flag = false;
				foreach (IntervalAnimation item2 in ODACDCDONJE.Intervals)
				{
					if (item2.Name == item && item2.Start <= frame && frame <= item2.GEJLNPIEDPF)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
		foreach (IntervalAnimation item3 in ODACDCDONJE.Intervals)
		{
			if (NIKHAICFGNM.Contains(item3.Name) && item3.Start <= frame && frame <= item3.GEJLNPIEDPF)
			{
				return true;
			}
		}
		return false;
	}

	public bool FMFFHKJBHNG(List<string> NIKHAICFGNM, int IHICCKAOPKG, bool FPMGBALCKPI)
	{
		int dBEDGEMEFNB = FALLOLJPMGF(IHICCKAOPKG);
		return EHEPILCDIDC(NIKHAICFGNM, dBEDGEMEFNB, FPMGBALCKPI);
	}

	public EventAnimation OIGBIFNICBI(EventAnimation.EECEJKADLCK LFLGCDNKNJI)
	{
		if (ODACDCDONJE != null)
		{
			return ODACDCDONJE.OIGBIFNICBI(LFLGCDNKNJI);
		}
		return null;
	}

	public int ONLKMFOENEH()
	{
		return (MNHGBPOIHKG + 1) * PGOFHCBPLOE();
	}

	public int EDDFIABEAGM()
	{
		return 2 * (MNHGBPOIHKG + 1);
	}

	public void PreloadEffects()
	{
		string text = "Textures/Effects/Magic/";
		foreach (ActionAnimation item in ODACDCDONJE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.EFFECT)
			{
				ActionEffect jFJGGMEJDPG = (ActionEffect)item;
				string oNNKJLOGHGH = text + jFJGGMEJDPG.EPDMGFELIMC();
				LocationSpriteCache.ENFOJMFEGJH(oNNKJLOGHGH);
			}
		}
	}

	public void PreloadSounds()
	{
		foreach (ActionAnimation item in ODACDCDONJE.DJBAIAKOIHM)
		{
			if (item.get_Type() == ActionAnimation.FADAJCEEKIO.SOUND)
			{
				ActionSound nMLKJLJHCIA = (ActionSound)item;
				Sound.IOIEJHLMBLI(nMLKJLJHCIA.get_Name());
			}
		}
	}

	public bool NCEKKNIMHAG()
	{
		return INFAGPDFGNL;
	}

	public void LLELLFKJKGE(bool value)
	{
		INFAGPDFGNL = value;
	}

	public StageType.FDBBPEGEGMK PHPHCKAHPOP()
	{
		return ENDJLOAGKGO;
	}

	public void POOOFPBAJDM(StageType.FDBBPEGEGMK value)
	{
		ENDJLOAGKGO = value;
	}

	public MirrorNode ECCLELFHNHE()
	{
		return FDECJHIMNGN;
	}

	public void NNFKIGLFLKL(MirrorNode value)
	{
		FDECJHIMNGN = value;
	}

	public Vector3[] BGHLLHNKFEM()
	{
		return _AnimationContainer[GOBJCKFGIPA];
	}

	private void DDPBDPEDIGC()
	{
		AnimationContainerStruct aGAMDIHPFPF = new AnimationContainerStruct();
		aGAMDIHPFPF.FileName = FileName;
		aGAMDIHPFPF.Container = _AnimationContainer;
		LECLDGFPOEA.Add(aGAMDIHPFPF);
	}

	public static void EGLKBMCHPNN()
	{
		foreach (AnimationContainerStruct item in LECLDGFPOEA)
		{
			item.Container = null;
		}
		LECLDGFPOEA.Clear();
	}

	public static int NJKKOFDBMOO()
	{
		return LECLDGFPOEA.Count;
	}

	private AnimationContainerStruct IAPAKFDEKOI()
	{
		return LECLDGFPOEA.FirstOrDefault((AnimationContainerStruct EIJNHOPFLGI) => FileName == EIJNHOPFLGI.FileName);
	}

	public override string ToString()
	{
		return "InfoAnim: Name: " + Name;
	}
}
