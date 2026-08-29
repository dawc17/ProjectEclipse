using System;
using System.Collections.Generic;
using System.Diagnostics;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.SF2.Core.Fights.Controller;
using Nekki.SF2.GUI.Fight;
using SF2DE.Underworld;
using UnityEngine;

public class Fight
{
	private class PMMOPMNOHOO
	{
		public int CPOOPPKHFHB;

		public float BNMFCPPJIAG;
	}

	private class FightListParametersBuffer
	{
		public int JEEFMFDJJNB;

		public int AIMNHKFPKAF;

		public int GPKIOJKBCAG;
	}

	private class DPEANMABHMN
	{
		public int LLMEEFAHCDH;

		public int JDLCNJKDMAJ;
	}

	private class GameOverParameters
	{
		public ModelParameters ABKBEJBICOA;

		public ModelParameters LEBLJJCFKOP;

		public GameOverTypes MHNEKAEGNBO;
	}

	private class RoundParam
	{
		public float PPFGEADDLNN;

		public float BNMFCPPJIAG;

		public int CPOOPPKHFHB;

		public int HCBNOKJFGLN;

		public float JAOMELOGOOJ;

		public int OGOLNFLBLBD;
	}

	private const float BFBKFFJGNAO = 2f;

	private const int PIJNAGLPNJI = 100;

	private const int LOKNBCKAFBJ = 30;

	private PMMOPMNOHOO ENCEAHGFIPK = new PMMOPMNOHOO();

	private FightListParametersBuffer LIBNDAFNOFG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool GAOPEBOEEGB;

	private static Fight _currentFight;

	private int frame;

	private int fightTimeInFrame;

	private FightList KGKDKENMAOA;

	private int HJCJMEELHPC;

	public List<Model> LNDLFINJHDB = new List<Model>();

	private List<Model> HCPGFOCGDAA = new List<Model>();

	private List<Model> JLEFIKJODGG = new List<Model>();

	private DPEANMABHMN GDKMODLCOIB;

	private Round round = new Round();

	private bool isFirstStrike;

	private bool isRenderCamera;

	private bool isRenderFight;

	private Location _location;

	private Camera _Camera;

	private bool HMFMLBOEPIG;

	private Queue<global::Pair<string, int>> JPCKKIBCAMG;

	private bool MEBIKGAKIMG;

	private bool isGameOver;

	private bool isStopFight;

	private bool MKCLBJEIIHN;

	private bool OEKKLGJMHDD;

	private bool IALDPDAAGCK;

	private bool KJJGBJCMCFF;

	private List<ModelParameters> IDAAONBIBJM;

	private ModelParameters NMNCKBPFCCP;

	private ModelParameters AKBNKDBHCEO;

	private ModelParameters CIFHAMACGFJ;

	private Model _playerModel;

	private Model CKNCPOABFBO;

	private Tactic GINNOLEJDFM;

	private PlayersFightData DPONLGICLEH = new PlayersFightData();

	private InFightRule _endFightRule;

	private EndRoundType _endRoundType;

	private CountersFight MOBFFOHPCOE = new CountersFight();

	private bool PEOIALGBJFB;

	private List<Achievement> FDACGIEEIEE = new List<Achievement>();

	private bool KJKJOJCMDGH;

	private bool GJMHPBIBHMO;

	private bool _isRoundOver;

	private bool BDDBMCNFNMG;

	private bool FJHJNOFPABO;

	private GameOverParameters LBKDADMLJOE = new GameOverParameters();

	private EquippedItemsStruct IEJFDGHCOON = new EquippedItemsStruct();

	private EquippedItemsStruct HNLEDOEPHKG = new EquippedItemsStruct();

	private uint HFCJNMAPPOI;

	private uint CJOODIMEJBB;

	private long _testStartTime;

	private int MKNDEBGPGAM;

	private bool BPMLGDFMKFO;

	private bool JMBEPENJGIG;

	private bool BLDBGJFBDPJ;

	private int ODLDPAKEHKN;

	private float DKDMOJJJHHL;

	private bool MNEOALEBNNA;

	private bool LKNILKJACGJ;

	private bool NCAEOKCFBFD;

	private float ICDHAHADCEH;

	private float JCCDMOJKANN;

	private float EJOIBPNPMFK;

	private int BGJDIGEJIFF;

	private bool isEndRound;

	private bool FCCPOLAMJNO;

	private bool OMBDLIKCNIP;

	private bool IDMICHMHCKE;

	private bool LKCNBFEINCM;

	private bool NLBINDFGKHO;

	private bool IOPJDMCBIMM;

	private RoundParam JEBNOLKKCIK;

	private RoundParam JOEADOFBDOC;

	private bool DOANFKMFJFK;

	private bool KCNHDABOAAA;

	private bool DODCPKOADGF;

	private int ADJAMFGBOAP;

	private bool FIMDJDJOFDM;

	private GameObject _UnityObject;

	public StageType.FDBBPEGEGMK stageType;

	public PreFight preFight;

	public GameController KCJNBFLAMCC;

	public SelectAnimation _SelectAnimation = new SelectAnimation();

	public RulesInspector _rulesInspector;

	private PerksStage EPBDEDGLHJE = new PerksStage();

	public Model.StrikeResult FKGAAFNNCNE;

	public bool POEILGNLILJ
	{
		get
		{
			return PDINEPNPDFI();
		}
		set
		{
			JOJIDODPDLA(value);
		}
	}

	public static Fight JDBIOLLJFCH
	{
		get
		{
			return OHNKFOHIAKG();
		}
		set
		{
			set_CurrentFight(value);
		}
	}

	public FightList HCKCDAFBDDK
	{
		get
		{
			return OGNINOBBHIG();
		}
		set
		{
			ODJNDMPFBMA(value);
		}
	}

	public GameObject ICDCIANNAAI
	{
		get
		{
			return MJNPBMOAFML();
		}
	}

	public PerksStage KBNFKAKJDHN
	{
		get
		{
			return IEEGPNLEKHH();
		}
	}

	private bool LKNLLBNNFIN
	{
		get
		{
			return NHKKFGFNANI();
		}
	}

	private bool BANNJIEICHN
	{
		get
		{
			return HNKJALKBCBN();
		}
	}

	public BattleType DEGIADEEFGG
	{
		get
		{
			return MBEJJCKIIHK();
		}
	}

	public bool PFNKLCDEEPP
	{
		get
		{
			return CONGPMFCIJM();
		}
	}

	public bool HNKMDNMAOML
	{
		get
		{
			return JKMPOFGHKLH();
		}
	}

	public Model JGFGFEJIELN
	{
		get
		{
			return BBGAFGNHFEA();
		}
	}

	public Model FJIKBBJPAKE
	{
		get
		{
			return FHHHIEPAKLP();
		}
	}

	public bool NMLOAGOJOFB
	{
		set
		{
			OHEIDPMLNDE(value);
		}
	}

	public int OHCPACALGMC
	{
		get
		{
			return get_RoundNumber();
		}
	}

	public int OAGBNEDNGDD
	{
		get
		{
			return get_RoundTimeLeft();
		}
	}

	public int DAFBFMPFKOK
	{
		get
		{
			return get_RoundTimeLeftFrames();
		}
	}

	public int HIGCOHKLCFG
	{
		get
		{
			return get_RoundTimePassedFrames();
		}
	}

	public int MBPCDFMMJDJ
	{
		get
		{
			return get_RoundTimeTotalFrames();
		}
	}

	public bool CIHCMIIAMIG
	{
		get
		{
			return get_isFightNone();
		}
	}

	public bool ILNBGPOKMHN
	{
		get
		{
			return get_IsRaidFight();
		}
	}

	public bool MBGLGKCLJAI
	{
		get
		{
			return get_IsFightOver();
		}
	}

	public int OOGBGHALPJD
	{
		get
		{
			return get_FightTimeInFrames();
		}
	}

	public Fight(object data, ModelParameters AIFLOMMDGJB, List<ModelParameters> ELGGAEBPCHI, PreFight preFight = null, GameController LPGANKOAPJL = null)
	{
		_currentFight = this;
		_UnityObject = new GameObject("Fight");
		JOJIDODPDLA(false);
		KGKDKENMAOA = (FightList)data;
		ADJAMFGBOAP = 0;
		NMNCKBPFCCP = AIFLOMMDGJB;
		IDAAONBIBJM = ELGGAEBPCHI;
		HJCJMEELHPC = 0;
		isGameOver = false;
		isStopFight = false;
		FCCPOLAMJNO = false;
		IALDPDAAGCK = false;
		KJJGBJCMCFF = false;
		IDMICHMHCKE = false;
		isFirstStrike = false;
		isEndRound = false;
		stageType = StageType.FDBBPEGEGMK.STAGE_NONE;
		isRenderFight = true;
		isRenderCamera = true;
		MKCLBJEIIHN = false;
		PEOIALGBJFB = false;
		OMBDLIKCNIP = false;
		_endFightRule = null;
		frame = 0;
		fightTimeInFrame = 0;
		FKGAAFNNCNE = null;
		BDDBMCNFNMG = false;
		KJKJOJCMDGH = false;
		GJMHPBIBHMO = false;
		OEKKLGJMHDD = false;
		LKCNBFEINCM = false;
		BPMLGDFMKFO = false;
		JMBEPENJGIG = false;
		MKNDEBGPGAM = 0;
		_testStartTime = 0L;
		_playerModel = null;
		CKNCPOABFBO = null;
		_rulesInspector = null;
		_endRoundType = EndRoundType.EndRoundTypeNone;
		ODLDPAKEHKN = 0;
		DKDMOJJJHHL = 0f;
		BGJDIGEJIFF = 0;
		JEBNOLKKCIK = new RoundParam();
		JOEADOFBDOC = new RoundParam();
		_isRoundOver = false;
		GINNOLEJDFM = null;
		BLDBGJFBDPJ = false;
		ICDHAHADCEH = float.MinValue;
		JCCDMOJKANN = float.MinValue;
		EJOIBPNPMFK = 0f;
		LKNILKJACGJ = false;
		NCAEOKCFBFD = false;
		NLBINDFGKHO = false;
		FJHJNOFPABO = false;
		MNEOALEBNNA = true;
		IOPJDMCBIMM = true;
		KCNHDABOAAA = false;
		DODCPKOADGF = false;
		JPCKKIBCAMG = new Queue<global::Pair<string, int>>();
		MEBIKGAKIMG = true;
		GameUtils.MHMGONPIPKG(KGKDKENMAOA.CNAOMDMIGLJ);
		if (!GameUtils.NMODJEJFFNC())
		{
		}
		if (GameUtils.LDBMFAMEMPF)
		{
			SystemProperties.NHIDOHIJMBG(GameUtils.CDILOOACLKK / GameUtils.MAEBANCIBOP);
		}
		if (data == null)
		{
			LLLOJBFMONN.Error("Fight::Fight - data == 0");
		}
		List<InfoAnimation> list = AnimationData.CCANGHENJAE();
		foreach (InfoAnimation item in list)
		{
			item.ABNCNNHMLII();
		}
		List<Trigger> list2 = AnimationData.GFPPKEAMEBO();
		foreach (Trigger item2 in list2)
		{
			item2.ABNCNNHMLII();
		}
			AKBNKDBHCEO = IDAAONBIBJM[ADJAMFGBOAP];
			GINNOLEJDFM = AKBNKDBHCEO.HBFMBOHLKPJ;
			MIEPNNMDNBO();
			Zone locationZone = KGKDKENMAOA.CNAOMDMIGLJ == null ? null : KGKDKENMAOA.CNAOMDMIGLJ.OAEIILGHJMG;
			bool raidLayout = UnderworldZonePolicy.IsRaidZone(locationZone);
		_location = new Location(KGKDKENMAOA.JKMJHIIMHPG, KGKDKENMAOA.NPPIFKKLNCN, raidLayout);
		_location.init();
		NMNCKBPFCCP.JJCKADKCDIF.Set(_location.JJNMOJLLDEC);
		AKBNKDBHCEO.JJCKADKCDIF.Set(_location.CLGGLBHOMCE);
		bool flag = false;
		ODNEEGLKKCK();
		InitRules();
		CheckChangeFightRules();
		_rulesInspector.ApplyAvatarAndNameRules(NMNCKBPFCCP);
		_rulesInspector.ApplyNoAnimationRules(NMNCKBPFCCP);
		_rulesInspector.ApplyNoPerksRules(NMNCKBPFCCP, _rulesInspector.GetPlayerNoPerks());
		_rulesInspector.ApplyNoPerksRules(AKBNKDBHCEO, _rulesInspector.GetEnemyNoPerks());
		GameUtils.OKIEEBMCGHE(_location.MFAPMDDJBBL);
		GameUtils.MJAPCKDDAMK(_location.JMLAKAKDBBL - _location.MFAPMDDJBBL);
		if (!flag)
		{
			MOBFFOHPCOE.Init(GameUtils.OJNHPHEPFLI.ECMIANLOLHM(KGKDKENMAOA), NMNCKBPFCCP, KGKDKENMAOA.get_Type(), GameUtils.MPNBGBIMEIP(KGKDKENMAOA));
			MOBFFOHPCOE.AddEventListener(0, GJJLEFLCOFL);
		}
		_Camera = new Camera(_UnityObject.transform);
		_Camera.Init(_location);
		_Camera.AddEventListener(0, MOFKFJCIBGC);
		_Camera.AddEventListener(1, OCFDPNKALIJ);
		FKFNHGJNIAA();
		MKCLBJEIIHN = true;
		this.preFight = preFight;
		if (this.preFight != null)
		{
			this.preFight.Init(KGKDKENMAOA);
			this.preFight.ViewerPauseVisible(HNKJALKBCBN());
			this.preFight.OnStopScreen.AddListener(OnStopPreFight);
			this.preFight.OnButtonClick.AddListener(OnButtonClick);
			this.preFight.OnAchievementMessageHide.AddListener(BMELKMACHCM);
		}
		EPBDEDGLHJE.AddEventListener(11, LDBBHGDELIJ);
		BEEEKHIHJPH(LPGANKOAPJL);
		_Camera.AddPreFight(preFight);
		_Camera.DFKKNMDAFDC(false);
		round.round = 0;
		JNBONELPNKE();
		if (KGKDKENMAOA.get_Type() != BattleType.FightNone)
		{
			KGKDKENMAOA.set_IsInFight(true);
			Sound.PlayMusic(_location.MOADJJNKFKB());
			SoundController.IsBackgroundMusicIntro = false;
			StartVS();
		}
		else
		{
			SoundController.KHPHDKFDCLL();
			StartPunchbag();
		}
		GameUtils.CEPJBBGGMDP(1);
		if (!AssemblyController.JEEFAGGMFCK())
		{
			AKBNKDBHCEO.EEGMBGBLLIF = false;
		}
		OMBDLIKCNIP = false;
		ModelAi.set_AiOn(true);
	}

	public bool PDINEPNPDFI()
	{
		return GAOPEBOEEGB;
	}

	public void JOJIDODPDLA(bool value)
	{
		GAOPEBOEEGB = value;
	}

	public static Fight OHNKFOHIAKG()
	{
		return _currentFight;
	}

	public static void set_CurrentFight(Fight value)
	{
		if (value != _currentFight && (_currentFight == null || value == null))
		{
			_currentFight = value;
		}
		else
		{
			LLLOJBFMONN.Error("Fight::setCurrentFight - fight not NULL");
		}
	}

	public FightList OGNINOBBHIG()
	{
		return KGKDKENMAOA;
	}

	public void ODJNDMPFBMA(FightList value)
	{
		KGKDKENMAOA = value;
	}

	public GameObject MJNPBMOAFML()
	{
		return _UnityObject;
	}

	public PerksStage IEEGPNLEKHH()
	{
		return EPBDEDGLHJE;
	}

	private bool NHKKFGFNANI()
	{
		return NMNCKBPFCCP.BHHLEBHLBLH;
	}

	private bool HNKJALKBCBN()
	{
		return !AssemblyController.KMEOEAGGPBI();
	}

	public BattleType MBEJJCKIIHK()
	{
		return KGKDKENMAOA.get_Type();
	}

	public bool CONGPMFCIJM()
	{
		return stageType == StageType.FDBBPEGEGMK.STAGE_FIGHT;
	}

	public bool JKMPOFGHKLH()
	{
		return stageType == StageType.FDBBPEGEGMK.STAGE_NONE;
	}

	public Model BBGAFGNHFEA()
	{
		return _playerModel;
	}

	public Model FHHHIEPAKLP()
	{
		return CKNCPOABFBO;
	}

	public void OHEIDPMLNDE(bool value)
	{
		BLDBGJFBDPJ = value;
	}

	public int get_RoundNumber()
	{
		return round.round;
	}

	public int get_RoundTimeLeft()
	{
		return (preFight != null) ? preFight.get_TimeLeft() : 0;
	}

	public int get_RoundTimeLeftFrames()
	{
		return (preFight != null) ? preFight.get_TimeLeftFrames() : 0;
	}

	public int get_RoundTimePassedFrames()
	{
		return (preFight != null) ? preFight.get_TimePassedFrames() : 0;
	}

	public int get_RoundTimeTotalFrames()
	{
		return (preFight != null) ? preFight.get_TimeTotalRoundFrames() : 0;
	}

	public bool get_isFightNone()
	{
		if (KGKDKENMAOA != null)
		{
			return KGKDKENMAOA.get_Type() == BattleType.FightNone;
		}
		return false;
	}

	public bool get_IsRaidFight()
	{
		return KGKDKENMAOA.get_Type() == BattleType.FightRaid;
	}

	public bool get_IsFightOver()
	{
		return isGameOver;
	}

	public int get_FightTimeInFrames()
	{
		return fightTimeInFrame;
	}

	public void ANIDBLANMIC()
	{
		if (GameUtils.LDBMFAMEMPF && !SystemProperties.AFKGHBJPLOK() && !SystemProperties.NFFOJCHNPJD())
		{
			SystemProperties.NHIDOHIJMBG(GameUtils.CDILOOACLKK);
		}
		set_CurrentFight(null);
		IGIANHEMGKA(KGKDKENMAOA);
		KGKDKENMAOA.set_IsInFight(false);
		KGKDKENMAOA.JENGHOJIOFK();
		ResetParameters();
		GameUtils.CEPJBBGGMDP(1);
		KCJNBFLAMCC.RemoveEventListener(0, ControlPress);
		KCJNBFLAMCC.RemoveEventListener(1, ControlRelease);
		KCJNBFLAMCC.ResetController();
		EPBDEDGLHJE.RemoveEventListener(11, LDBBHGDELIJ);
		_rulesInspector.ClearRules();
		foreach (Model item in LNDLFINJHDB)
		{
			RemoveModel(item);
		}
		_SelectAnimation.FDBHLFMBECM();
		ModelLoader.PAGDHDKNBPK();
		if (KGKDKENMAOA.get_Type() != BattleType.FightNone)
		{
			SoundController.KHPHDKFDCLL();
		}
		_Camera.RemoveAllEventListener();
		_Camera.Clear();
		_Camera = null;
		if (MOBFFOHPCOE != null)
		{
			MOBFFOHPCOE.RemoveEventListener(0, GJJLEFLCOFL);
		}
		EPBDEDGLHJE.RemoveEventListener(11, LDBBHGDELIJ);
		AiData.ClearTables();
		List<InfoAnimation> list = AnimationData.CCANGHENJAE();
		foreach (InfoAnimation item2 in list)
		{
			item2.ABNCNNHMLII();
		}
		List<Trigger> list2 = AnimationData.GFPPKEAMEBO();
		foreach (Trigger item3 in list2)
		{
			item3.ABNCNNHMLII();
		}
		LocationSpriteCache.Clear();
	}

	public void RandomizeObscuredVars()
	{
		IDAAONBIBJM.ForEach((ModelParameters DHDMNHCIPEH) =>
		{
			DHDMNHCIPEH.RandomizeObscuredVars();
		});
		if (NMNCKBPFCCP != null)
		{
			NMNCKBPFCCP.RandomizeObscuredVars();
		}
		if (AKBNKDBHCEO != null)
		{
			AKBNKDBHCEO.RandomizeObscuredVars();
		}
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			preFight.get_ViewerFight().RandomizeObscuredVars();
		}
	}

	public void ApplyDamageFromServer(string INGHMAIMCMJ, int AFMMKADPHGM, int CACNLKMAHBO)
	{
	}

	public void Draw()
	{
		int num = ((!GameUtils.LDBMFAMEMPF) ? 1 : 2);
		for (int i = 0; i < num; i++)
		{
			if ((bool)preFight)
			{
				preFight.SetPause(PDINEPNPDFI());
			}
			if (PDINEPNPDFI())
			{
				break;
			}
			Render();
		}
	}

	public void ReleaseAnyKey(FightCID PBFPKFPMFCI)
	{
		if ((stageType != StageType.FDBBPEGEGMK.STAGE_FIGHT && PBFPKFPMFCI != FightCID.NextFrameButton && PBFPKFPMFCI != FightCID.PauseButton) || (!Application.isEditor && !SystemProperties.DBBOCENKMGD() && !CUDLRConsole.EHGELAGEGDL()))
		{
			return;
		}
		switch (PBFPKFPMFCI)
		{
		case FightCID.PauseButton:
			JOJIDODPDLA(!PDINEPNPDFI());
			break;
		case FightCID.NextFrameButton:
			if (PDINEPNPDFI())
			{
				Render();
			}
			break;
		case FightCID.EnableMinScale:
			_Camera.JMGBMIDNCFP();
			break;
		case FightCID.WinRoundButton:
			KillModel(false, false);
			break;
		case FightCID.WinFightButton:
			KillModel(false, true);
			break;
		case FightCID.LossRoundButton:
			KillModel(true, false);
			break;
		case FightCID.LossFightButton:
			KillModel(true, true);
			break;
		case FightCID.ResetRoundButton:
			MLJCABABNDB();
			break;
		case FightCID.ResetFightButton:
			GJIGBLMLJLD();
			break;
		case FightCID.RechargeMagic:
			_playerModel.IPGBFKOCOCK(1);
			_playerModel.BFBFNKMLOJA();
			break;
		case FightCID.IncreaseComboHit:
			_playerModel.KDJPMHGEPAF();
			break;
		case FightCID.IncreaseStyle:
		{
			ScreenModel screenModel = ((!(preFight.get_ViewerFight() != null)) ? null : preFight.get_ViewerFight().get_LeftModel());
			if (screenModel != null)
			{
				screenModel.IncreaseStyleByValue(1f);
			}
			break;
		}
		case FightCID.SetPlayerAllHitsCritical:
			CKNCPOABFBO.NEHLJGPKHKF(!CKNCPOABFBO.FGAHBBDGPBO());
			break;
		case FightCID.SetPlayerImmortality:
			_playerModel.KMMJCHDKBDO.set_IsImmortalityEnabled(!_playerModel.KMMJCHDKBDO.AGICDDJBPLB());
			break;
		case FightCID.SetBotImmortality:
			CKNCPOABFBO.KMMJCHDKBDO.set_IsImmortalityEnabled(!CKNCPOABFBO.KMMJCHDKBDO.AGICDDJBPLB());
			break;
		case FightCID.ShowEdgesButton:
			break;
		case FightCID.ShowDebugPerksButton:
			NLBINDFGKHO = !NLBINDFGKHO;
			break;
		case FightCID.SlowModeKey:
			IDMICHMHCKE = !IDMICHMHCKE;
			IFKFINOGOLC(IDMICHMHCKE);
			break;
		case FightCID.SoundMuteButton:
		case FightCID.TestTactic:
		case FightCID.StartBenchmarkKey:
		case FightCID.StartSuper:
		case FightCID.FullscreenMode:
			break;
		}
	}

	public void OnIntervalStart(object data)
	{
		Model.EventModel oJDOHGBGPFK = (Model.EventModel)data;
		IntervalAnimation mNOIEOBBCMI = (IntervalAnimation)oJDOHGBGPFK.Data;
		if (mNOIEOBBCMI.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVISIBLE)
		{
			_Camera.KKFIJLOMOJI().FPNKBJPKKGB().NGPIALAGGBI(oJDOHGBGPFK.KJDFJPBIGJC.CLDMEJKGLBA(), false);
		}
		KCACCJNMOFM(oJDOHGBGPFK);
	}

	public void OnIntervalEnd(object data)
	{
		Model.EventModel oJDOHGBGPFK = (Model.EventModel)data;
		IntervalAnimation mNOIEOBBCMI = (IntervalAnimation)oJDOHGBGPFK.Data;
		if (mNOIEOBBCMI.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVISIBLE)
		{
			_Camera.KKFIJLOMOJI().FPNKBJPKKGB().NGPIALAGGBI(oJDOHGBGPFK.KJDFJPBIGJC.CLDMEJKGLBA(), true);
		}
		EPBDEDGLHJE.OFKIKABKDFD()["Interval"] = mNOIEOBBCMI;
		EPBDEDGLHJE.JALOHCICLGN(oJDOHGBGPFK.KJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_INTERVAL_END, true);
		CMMLPNPPGKH(oJDOHGBGPFK);
	}

	public void OnAnimationStart(object data)
	{
		if (stageType == StageType.FDBBPEGEGMK.STAGE_END_STANCE && !isEndRound)
		{
			ModelParameters kIKOGDEPGHB = GetWinner(true);
			switch (kIKOGDEPGHB.EndRoundType)
			{
			case EndRoundType.EndRoundTypeTimeOut:
				if (preFight != null)
				{
					preFight.CreateTimesUp();
				}
				break;
			case EndRoundType.EndRoundTypeRingOut:
				if (preFight != null)
				{
					preFight.CreateRingOut();
				}
				break;
			case EndRoundType.EndRoundTypeLose:
				if (kIKOGDEPGHB.IsPlayer)
				{
					if (preFight != null)
					{
						preFight.CreateYouWin();
					}
				}
				else if (preFight != null)
				{
					preFight.CreateYouLose();
				}
				break;
			default:
				if (!kIKOGDEPGHB.IsPlayer)
				{
					break;
				}
				if (kIKOGDEPGHB.DKAHKGBFJMG)
				{
					if (preFight != null)
					{
						preFight.CreateWinner(true);
					}
				}
				else if (kIKOGDEPGHB.HABJPOFCIHA() <= GameUtils.EBMNPGEKENM() && preFight != null)
				{
					preFight.CreateWinner(false);
				}
				break;
			}
			isEndRound = true;
		}
		Model.EventModel oJDOHGBGPFK = (Model.EventModel)data;
		InfoAnimation value = (InfoAnimation)oJDOHGBGPFK.Data;
		EPBDEDGLHJE.OFKIKABKDFD()["Animation"] = value;
		EPBDEDGLHJE.JALOHCICLGN(oJDOHGBGPFK.KJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_ANIMATION_START, true);
		CheckFightRules(FightEvent.AnimationStartEvent, ((Model.EventModel)data).KJDFJPBIGJC.EPCNJLEHJCB() ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		if (!oJDOHGBGPFK.KJDFJPBIGJC.NMPHACPBHKO())
		{
			_Camera.KKFIJLOMOJI().FPNKBJPKKGB().NGPIALAGGBI(oJDOHGBGPFK.KJDFJPBIGJC.CLDMEJKGLBA(), true);
			oJDOHGBGPFK.KJDFJPBIGJC.KKLMIAFFKNE(true);
		}
	}

	public void OnAnimationEnd(object data)
	{
		Model.EventModel oJDOHGBGPFK = (Model.EventModel)data;
		InfoAnimation value = (InfoAnimation)oJDOHGBGPFK.Data;
		EPBDEDGLHJE.OFKIKABKDFD()["Animation"] = value;
		EPBDEDGLHJE.JALOHCICLGN(oJDOHGBGPFK.KJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_ANIMATION_END, true);
		Model fGCODGKLHED = oJDOHGBGPFK.KJDFJPBIGJC.EGGEACCDAEK();
		bool flag = oJDOHGBGPFK.KJDFJPBIGJC.CDMBCHOJKPH() && fGCODGKLHED != null && fGCODGKLHED.CDMBCHOJKPH();
		if (stageType == StageType.FDBBPEGEGMK.STAGE_START_STANCE && flag)
		{
			BPFFCNAGLCN();
			if (KGKDKENMAOA.get_Type() != BattleType.FightNone)
			{
				StartFight();
			}
			else
			{
				SetStage(StageType.FDBBPEGEGMK.STAGE_FIGHT);
			}
		}
		if (stageType == StageType.FDBBPEGEGMK.STAGE_END_STANCE && flag)
		{
			BPFFCNAGLCN();
			FinishRound();
		}
	}

	public void PIJPBDGHHGE(Model.EventModel EGHPHELLOGO)
	{
		if (EGHPHELLOGO.KJDFJPBIGJC.NJDJHGDMCIJ() == null)
		{
			CheckFightRules(FightEvent.PhysicsStartEvent, EGHPHELLOGO.KJDFJPBIGJC.EPCNJLEHJCB() ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		}
	}

	public void BGBFDJENABO(object data)
	{
	}

	public void OnEveryFrame(object data)
	{
		Model kJDFJPBIGJC = ((Model.EventModel)data).KJDFJPBIGJC;
		EPBDEDGLHJE.OFKIKABKDFD()["StepFrame"] = BGJDIGEJIFF;
		EPBDEDGLHJE.JALOHCICLGN(kJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_EVERY_FRAME, true);
		FJHJNOFPABO = true;
	}

	public void HLIOEELKFCP(object data)
	{
		EPBDEDGLHJE.HLIOEELKFCP(data);
	}

	public void GPABGFNBALE(Model.EventModel EGHPHELLOGO)
	{
		_SelectAnimation.PKFPDKFLKBL(EGHPHELLOGO);
	}

	private void SetStage(StageType.FDBBPEGEGMK LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case StageType.FDBBPEGEGMK.STAGE_FIGHT:
			KCJNBFLAMCC.StartController();
			break;
		case StageType.FDBBPEGEGMK.STAGE_END_STANCE:
			KCJNBFLAMCC.StopController();
			break;
		}
		stageType = LFLGCDNKNJI;
		foreach (Model item in LNDLFINJHDB)
		{
			item.KDAHHIMLJGG.Data = LFLGCDNKNJI;
			item.JMHJDHLBHLK = (int)LFLGCDNKNJI;
			EPBDEDGLHJE.JALOHCICLGN(item, PerkEvent.KNKIIEPDCPN.EVENT_ROUND_STAGE_START);
			item.KDAHHIMLJGG.Data = LFLGCDNKNJI;
			item.JMHJDHLBHLK = (int)LFLGCDNKNJI;
			_SelectAnimation.CheckEvent(EventAnimation.EECEJKADLCK.EVENT_ROUND_STAGE, item.KDAHHIMLJGG);
		}
	}

	public void JNBONELPNKE()
	{
	}

	public void CGIFNFMDBDH()
	{
	}

	public void Render()
	{
		if (isRenderFight)
		{
			RenderFight();
		}
		if (isRenderCamera)
		{
			RenderCamera();
		}
		frame++;
	}

	private void RenderFight()
	{
		if (round.processing)
		{
			fightTimeInFrame++;
		}
		if (DODCPKOADGF)
		{
			HPIIICCLOON(FHHHIEPAKLP());
			DODCPKOADGF = false;
		}
		List<Model> list = new List<Model>();
		FJHJNOFPABO = false;
		EPBDEDGLHJE.Render();
		foreach (Model item in LNDLFINJHDB)
		{
			bool flag = item.IBIDGACDJNF();
			item.Render();
			if (flag)
			{
				InfoAnimation pJAHIOELGGD = item.OCPMJKIEPIG().NNMAFFCCMHC();
				if (pJAHIOELGGD != null && pJAHIOELGGD.PHPHCKAHPOP() == stageType)
				{
					list.Add(item);
				}
			}
		}
		if (FJHJNOFPABO)
		{
			BGJDIGEJIFF++;
		}
		if (list.Count > 1)
		{
			AlignCameraOnModels(list);
		}
		if (HCPGFOCGDAA.Count > 0)
		{
			foreach (Model item2 in HCPGFOCGDAA)
			{
				item2.Render();
				LNDLFINJHDB.Add(item2);
			}
			HCPGFOCGDAA.Clear();
		}
		PAIOMLKCNOP();
		if (!NHKKFGFNANI())
		{
			RenderCollisions();
			_SelectAnimation.UpdateConditions();
			foreach (Model item3 in LNDLFINJHDB)
			{
				item3.RenderAi();
			}
		}
		IGLLNGNGPOA();
		_SelectAnimation.Render();
		EPBDEDGLHJE.PAHPCIFKDEA();
		if (MKCLBJEIIHN)
		{
			CheckFightRules(FightEvent.RenderEvent, RuleAppliance.ApplianceAll);
			_Camera.OMPFAMELAII();
			_Camera.GDOPCJEGPFL();
			_Camera.KKFIJLOMOJI().GOCPBKNDKMC().DHOMHKADCFG();
			_Camera.KKFIJLOMOJI().GDBMKMFFOCF().DHOMHKADCFG();
			_Camera.KKFIJLOMOJI().IFDHBLGKEHN();
		}
		if (PEOIALGBJFB)
		{
			PEOIALGBJFB = false;
			KGKPLKJPDAI();
		}
		RenderRound();
		if (preFight != null)
		{
			preFight.Render();
		}
		IFKELPCCEHC();
		BELLAEIMEAB();
		ResetModelsHitData();
		HBGMKCNFKHM();
	}

	private void RenderCamera()
	{
		if (preFight != null)
		{
			preFight.RenderComboModel();
		}
		if (isEndRound)
		{
			GOCNEMPBJIH(2f);
		}
		_Camera.Render();
	}

	public void RenderCollisions()
	{
		DGNDJBDKNAI();
		bool fHPKEJMDFLK = false;
		bool flag = frame % 2 == 0;
		int count = LNDLFINJHDB.Count;
		if (flag)
		{
			for (int i = 0; i < count; i++)
			{
				fHPKEJMDFLK = LNDLFINJHDB[i].RenderCollision(fHPKEJMDFLK);
			}
			return;
		}
		for (int num = count - 1; num >= 0; num--)
		{
			fHPKEJMDFLK = LNDLFINJHDB[num].RenderCollision(fHPKEJMDFLK);
		}
	}

	public override string ToString()
	{
		return string.Empty;
	}

	public void HIIGDMMGBBD(bool ONFJJLFGNCH = false)
	{
		if (KGKDKENMAOA.get_Type() != BattleType.FightNone)
		{
			if (PDINEPNPDFI())
			{
				ClosePauseScreen();
			}
			else
			{
				OpenPauseScreen();
			}
		}
	}

	public void CreateRingout(float HIKKOEOGMEK, float NMMCJGHAJBB, float DKJCJBAGKIL, string AJBGJNMLMKE)
	{
		_Camera.KKFIJLOMOJI().BFLMJIEIIFM(HIKKOEOGMEK, NMMCJGHAJBB, DKJCJBAGKIL, AJBGJNMLMKE);
	}

	public void LCDPAAFCLPB()
	{
		_Camera.KKFIJLOMOJI().DKLLNGOMCHN();
	}

	public void CreateHotGround(string AJBGJNMLMKE, float ABKMCKDJCGB)
	{
	}

	public void HEJMDNEJKLL()
	{
	}

	public void CreatePerkActivationArea(float JMLAKAKDBBL, string KHPKDMGDMAB, string ADONPNOBBDE)
	{
		LKNILKJACGJ = true;
		EJOIBPNPMFK = JMLAKAKDBBL;
		_Camera.KKFIJLOMOJI().CreatePerkActivationArea(JMLAKAKDBBL, KHPKDMGDMAB, ADONPNOBBDE);
	}

	public void UpdatePerkActivationArea(float MGMMDGFPBLP, float KGJALFLDIBG, bool EBKPFEFCIIH)
	{
		ICDHAHADCEH = MGMMDGFPBLP - EJOIBPNPMFK / 2f + _location.JMLAKAKDBBL / 2f;
		JCCDMOJKANN = MGMMDGFPBLP + EJOIBPNPMFK / 2f + _location.JMLAKAKDBBL / 2f;
		NCAEOKCFBFD = EBKPFEFCIIH;
		_Camera.KKFIJLOMOJI().UpdatePerkActivationArea(MGMMDGFPBLP, KGJALFLDIBG);
	}

	public void NPFHCPAAIFJ()
	{
		LKNILKJACGJ = false;
		_Camera.KKFIJLOMOJI().NPFHCPAAIFJ();
	}

	public void JKPOGNMHDNK(RuleAppliance EJPOJJKKICO, bool KFIECNIMAOA)
	{
		if (preFight != null)
		{
			preFight.SetHealthBarVisible(EJPOJJKKICO, KFIECNIMAOA);
		}
	}

	public bool UpdateLife(Model ACENLMONNPA, float AACBFABMADJ)
	{
		if (KGKDKENMAOA.get_Type() == BattleType.FightNone)
		{
			return false;
		}
		ACENLMONNPA.GEACPINOAAN(AACBFABMADJ);
		if (ACENLMONNPA.KMMJCHDKBDO.OJMIFOAHKBK())
		{
			ACENLMONNPA.KMMJCHDKBDO.PCALDKCJGCK = true;
		}
		return !ACENLMONNPA.PDFCAFIMALN();
	}

	public void SetLife(Model ACENLMONNPA, float DLEDDPFNPOH)
	{
		ACENLMONNPA.GFNCMLFKBGP(DLEDDPFNPOH);
	}

	public bool UpdateLife(RuleAppliance EJPOJJKKICO, float AACBFABMADJ)
	{
		Model fGCODGKLHED = null;
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			fGCODGKLHED = _playerModel;
			break;
		case RuleAppliance.ApplianceOpponent:
			fGCODGKLHED = CKNCPOABFBO;
			break;
		default:
			LLLOJBFMONN.Error("Fight::updateLife: wrong RuleAppliance - %i", EJPOJJKKICO);
			return false;
		}
		return UpdateLife(fGCODGKLHED, AACBFABMADJ);
	}

	public void DBIHABKLFHP(float KGJALFLDIBG)
	{
		_Camera.KKFIJLOMOJI().DBIHABKLFHP(KGJALFLDIBG);
	}

	public void HKOMIIDELBC()
	{
		_Camera.KKFIJLOMOJI().HKOMIIDELBC();
	}

	public void OBICGGFDMLN()
	{
		_Camera.KKFIJLOMOJI().OBICGGFDMLN();
	}

	public void DNJMJGFGHBC(Model ACENLMONNPA, PerkTrigger CPBHKJFPFJB)
	{
	}

	public void PHNCLBJKCOE(Model ACENLMONNPA, bool CCBEDPIHKAD)
	{
		_Camera.KKFIJLOMOJI().FPNKBJPKKGB().NGPIALAGGBI(ACENLMONNPA.CLDMEJKGLBA(), CCBEDPIHKAD);
	}

	public void CKCCBJKIGIO(Model ACENLMONNPA, PerksStage.ActionPerk IBODMPMJELJ, bool CCBEDPIHKAD)
	{
		ScreenModel screenModel = null;
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			screenModel = ((!ACENLMONNPA.EPCNJLEHJCB()) ? preFight.get_ViewerFight().get_RightModel() : preFight.get_ViewerFight().get_LeftModel());
		}
		if (screenModel != null)
		{
			if (CCBEDPIHKAD)
			{
				screenModel.ADNAPNJMLBC(IBODMPMJELJ);
			}
			else
			{
				screenModel.PBCOANKNICH(IBODMPMJELJ);
			}
		}
	}

	public void GICAFBABMGA(Model ACENLMONNPA, PerksStage.ActionPerk CKOEFOCPMGK, PerksStage.ActionPerk IBODMPMJELJ)
	{
		ScreenModel screenModel = null;
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			screenModel = ((!ACENLMONNPA.EPCNJLEHJCB()) ? preFight.get_ViewerFight().get_RightModel() : preFight.get_ViewerFight().get_LeftModel());
		}
		if (screenModel != null)
		{
			screenModel.DHHCHBNJDGH(CKOEFOCPMGK, IBODMPMJELJ);
		}
	}

	public void OGIFEKGLKDK(Model ACENLMONNPA, PerkInfoItem AEFFHJGMNFI)
	{
	}

	public void HAANFNBPMBE(InFightRule HNBFMAKFJAM)
	{
		_endFightRule = HNBFMAKFJAM;
		if (_endFightRule != null)
		{
			switch (_endFightRule.get_Type())
			{
			case Rule.BCBLLMPAMLP.RuleRingout:
				_endRoundType = EndRoundType.EndRoundTypeRingOut;
				break;
			case Rule.BCBLLMPAMLP.RuleHotGround:
			case Rule.BCBLLMPAMLP.RuleLoseFall:
			case Rule.BCBLLMPAMLP.RuleCrazy:
			case Rule.BCBLLMPAMLP.RuleTimeoutWin:
			case Rule.BCBLLMPAMLP.RulePoints:
			case Rule.BCBLLMPAMLP.RuleWinStyle:
			case Rule.BCBLLMPAMLP.RuleWinCombo:
			case Rule.BCBLLMPAMLP.RuleWinShock:
				_endRoundType = EndRoundType.EndRoundTypeLose;
				break;
			case Rule.BCBLLMPAMLP.RuleRegeneration:
			case Rule.BCBLLMPAMLP.RuleLifeSteal:
				_endRoundType = EndRoundType.EndRoundTypeZeroHealth;
				break;
			}
		}
	}

	public void ALNNLCAKCAF(RuleAppliance IGFNCCEHFEK)
	{
		Model fGCODGKLHED = null;
		switch (IGFNCCEHFEK)
		{
		case RuleAppliance.AppliancePlayer:
			fGCODGKLHED = _playerModel;
			break;
		case RuleAppliance.ApplianceOpponent:
			fGCODGKLHED = CKNCPOABFBO;
			break;
		default:
			LLLOJBFMONN.Error("Fight::resetLife: wrong RuleAppliance - %i", IGFNCCEHFEK);
			break;
		}
		if (fGCODGKLHED != null)
		{
			fGCODGKLHED.GFNCMLFKBGP(0f);
		}
	}

	public void LCIOEPJIOMG()
	{
	}

	public void LNKBHDFPODI(Model ACENLMONNPA, Model.StrikeResult BNBAOJOJDGJ, PerkEvent.KNKIIEPDCPN LFLGCDNKNJI)
	{
		string text = ((BNBAOJOJDGJ.CMGLHHEJEBN == null) ? string.Empty : BNBAOJOJDGJ.CMGLHHEJEBN.NLLGDDMMJJN());
		InfoAnimation pBPDKJNKFCJ = BNBAOJOJDGJ.PBPDKJNKFCJ;
		EPBDEDGLHJE.OFKIKABKDFD()["Defense"] = BNBAOJOJDGJ.DefenceAttribute;
		EPBDEDGLHJE.OFKIKABKDFD()["Animation"] = pBPDKJNKFCJ;
		EPBDEDGLHJE.OFKIKABKDFD()["Critical"] = BNBAOJOJDGJ.DNGKOMPMPCD;
		EPBDEDGLHJE.OFKIKABKDFD()["Shock"] = BNBAOJOJDGJ.APCAKCCOMLO;
		EPBDEDGLHJE.OFKIKABKDFD()["Block"] = BNBAOJOJDGJ.DFOHNJEBDED;
		EPBDEDGLHJE.OFKIKABKDFD()["Damage"] = BNBAOJOJDGJ.EEDJBBOCFNL;
		EPBDEDGLHJE.JALOHCICLGN(ACENLMONNPA, LFLGCDNKNJI, true);
	}

	public void OnModelPreCrit(Model.EventModel EGHPHELLOGO)
	{
		Model.StrikeResult gHHCDAFIKJE = EGHPHELLOGO.KJDFJPBIGJC.GHHCDAFIKJE;
		LNKBHDFPODI(EGHPHELLOGO.KJDFJPBIGJC, gHHCDAFIKJE, PerkEvent.KNKIIEPDCPN.EVENT_HIT_PRECRIT);
	}

	public void OnModelPostCrit(Model.EventModel EGHPHELLOGO)
	{
		Model.StrikeResult gHHCDAFIKJE = EGHPHELLOGO.KJDFJPBIGJC.GHHCDAFIKJE;
		LNKBHDFPODI(EGHPHELLOGO.KJDFJPBIGJC, gHHCDAFIKJE, PerkEvent.KNKIIEPDCPN.EVENT_HIT_POSTCRIT);
	}

	public void OnModelHit(Model.EventModel EGHPHELLOGO)
	{
		Model.StrikeResult gHHCDAFIKJE = EGHPHELLOGO.KJDFJPBIGJC.GHHCDAFIKJE;
		IntervalAttack hFIIPNLCIEE = EGHPHELLOGO.Data as IntervalAttack;
		if (hFIIPNLCIEE.HPLOFLKCLHG())
		{
			gHHCDAFIKJE.DNGKOMPMPCD = false;
		}
		LNKBHDFPODI(EGHPHELLOGO.KJDFJPBIGJC, gHHCDAFIKJE, PerkEvent.KNKIIEPDCPN.EVENT_POST_HIT);
		if (hFIIPNLCIEE.HPLOFLKCLHG())
		{
			gHHCDAFIKJE.DNGKOMPMPCD = false;
		}
		if (KGKDKENMAOA.get_Type() == BattleType.FightNone)
		{
			gHHCDAFIKJE.DNGKOMPMPCD = false;
			gHHCDAFIKJE.APCAKCCOMLO = false;
			gHHCDAFIKJE.NIKPBGPPFEP = false;
		}
		if (gHHCDAFIKJE.APCAKCCOMLO)
		{
			if (EGHPHELLOGO.KJDFJPBIGJC.EDJFLMILEBA())
			{
				gHHCDAFIKJE.APCAKCCOMLO = false;
			}
			else
			{
				EGHPHELLOGO.KJDFJPBIGJC.set_IsShock(true);
			}
		}
		if (gHHCDAFIKJE.NIKPBGPPFEP)
		{
			ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(GameUtils.APCAKCCOMLO.JIIFFJAJNNN);
			bool flag = dJKEECEOCJB != null && EGHPHELLOGO.KJDFJPBIGJC.KMMJCHDKBDO.JGMLKIPCFII.Name == dJKEECEOCJB.Name;
			if (EGHPHELLOGO.KJDFJPBIGJC.HFHJFOEFPCD() || flag)
			{
				gHHCDAFIKJE.NIKPBGPPFEP = false;
			}
			else
			{
				EGHPHELLOGO.KJDFJPBIGJC.MLIIBCBGHBH(true);
				EGHPHELLOGO.KJDFJPBIGJC.ALJKJJKKIEF();
			}
		}
		gHHCDAFIKJE.LOONMILKCFK = !isFirstStrike;
		if (gHHCDAFIKJE.ALIHGFIJEDN != null)
		{
			ModelNode lCDGOCIAIDK = gHHCDAFIKJE.ALIHGFIJEDN.OGLAOHGLBHI();
			ModelNode lCDGOCIAIDK2 = gHHCDAFIKJE.ALIHGFIJEDN.KMHHBEKNHCJ();
			Vector3f nBMEGFBPGFE = lCDGOCIAIDK.ICLEOFDKDIF();
			Vector3f aKKEJFKBIHF = lCDGOCIAIDK.FOGHEPNAPLC();
			Vector3f nBMEGFBPGFE2 = lCDGOCIAIDK2.ICLEOFDKDIF();
			Vector3f aKKEJFKBIHF2 = lCDGOCIAIDK2.FOGHEPNAPLC();
			float num = 1f / 120f;
			Vector3f kKIKIDNALOL = Vector3f.PHEFFKMOOCM(Vector3f.MJOKEBGPHKB(nBMEGFBPGFE, aKKEJFKBIHF), Vector3f.MJOKEBGPHKB(nBMEGFBPGFE2, aKKEJFKBIHF2));
			IntervalAttack hFIIPNLCIEE2 = EGHPHELLOGO.GAIBPAGPEGK.OCPMJKIEPIG().HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK) as IntervalAttack;
			if (hFIIPNLCIEE2.PIKCMLIAFOI())
			{
				EGHPHELLOGO.KJDFJPBIGJC.SetHitData(gHHCDAFIKJE.Point, kKIKIDNALOL, (!gHHCDAFIKJE.DNGKOMPMPCD) ? num : (2f * num));
			}
			if (gHHCDAFIKJE.DNGKOMPMPCD)
			{
				_Camera.LCBPCEHILJD(gHHCDAFIKJE.Point, gHHCDAFIKJE.IIIDIKABLOJ);
			}
		}
		if (!gHHCDAFIKJE.DFOHNJEBDED)
		{
			EGHPHELLOGO.KJDFJPBIGJC.RemoveInterval(IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK);
			isFirstStrike = true;
		}
		ModelParameters kMMJCHDKBDO = EGHPHELLOGO.KJDFJPBIGJC.KMMJCHDKBDO;
		if (preFight != null)
		{
			preFight.ViewerStrike(gHHCDAFIKJE.PBPDKJNKFCJ, gHHCDAFIKJE.EEDJBBOCFNL, gHHCDAFIKJE.Target, gHHCDAFIKJE.LOONMILKCFK, gHHCDAFIKJE.JMDIIIFJMFH, gHHCDAFIKJE.DNGKOMPMPCD, gHHCDAFIKJE.DFOHNJEBDED, gHHCDAFIKJE.APCAKCCOMLO);
		}
		if (EGHPHELLOGO.KJDFJPBIGJC.IJINDLLEGKA())
		{
			gHHCDAFIKJE.EEDJBBOCFNL = 0f;
		}
		EGHPHELLOGO.KJDFJPBIGJC.LogDamage(gHHCDAFIKJE.EEDJBBOCFNL, BHLIBKKJNKH(hFIIPNLCIEE), gHHCDAFIKJE.DefenceAttribute);
		UpdateLife(EGHPHELLOGO.KJDFJPBIGJC, 0f - gHHCDAFIKJE.EEDJBBOCFNL);
		KDMDOBOKAIB(EGHPHELLOGO.KJDFJPBIGJC.EGGEACCDAEK(), gHHCDAFIKJE.EEDJBBOCFNL);
		if (!gHHCDAFIKJE.PBPDKJNKFCJ.BKGIEPOEBOF())
		{
			float num2 = EGHPHELLOGO.KJDFJPBIGJC.LPOJKGLFMAL();
			float num3 = EGHPHELLOGO.GAIBPAGPEGK.LPOJKGLFMAL();
			float cKKFKEIELCP = hFIIPNLCIEE.GHGGNMBCMNM();
			EGHPHELLOGO.KJDFJPBIGJC.UpdateMagicCharge(cKKFKEIELCP, EGHPHELLOGO.GAIBPAGPEGK, gHHCDAFIKJE.DFOHNJEBDED, gHHCDAFIKJE.DNGKOMPMPCD, false);
			EGHPHELLOGO.GAIBPAGPEGK.UpdateMagicCharge(cKKFKEIELCP, EGHPHELLOGO.KJDFJPBIGJC, gHHCDAFIKJE.DFOHNJEBDED, gHHCDAFIKJE.DNGKOMPMPCD, true);
			if (num2 < 1f && EGHPHELLOGO.KJDFJPBIGJC.LPOJKGLFMAL() >= 1)
			{
				EPBDEDGLHJE.JALOHCICLGN(EGHPHELLOGO.KJDFJPBIGJC, PerkEvent.KNKIIEPDCPN.EVENT_MAGIC_CHARGED, true);
			}
			if (num3 < 1f && EGHPHELLOGO.GAIBPAGPEGK.LPOJKGLFMAL() >= 1)
			{
				EPBDEDGLHJE.JALOHCICLGN(EGHPHELLOGO.GAIBPAGPEGK, PerkEvent.KNKIIEPDCPN.EVENT_MAGIC_CHARGED, true);
			}
		}
		if (EGHPHELLOGO.KJDFJPBIGJC.KMMJCHDKBDO.OJMIFOAHKBK())
		{
			EGHPHELLOGO.KJDFJPBIGJC.KMMJCHDKBDO.PCALDKCJGCK = true;
		}
		IFKFINOGOLC(false);
		if (gHHCDAFIKJE.DNGKOMPMPCD || (gHHCDAFIKJE.JMDIIIFJMFH && !gHHCDAFIKJE.DFOHNJEBDED) || gHHCDAFIKJE.APCAKCCOMLO)
		{
			GameUtils.HitEffect pIHIIMOOICM = PPCKJAOGBHO(gHHCDAFIKJE.DNGKOMPMPCD, gHHCDAFIKJE.JMDIIIFJMFH && !gHHCDAFIKJE.DFOHNJEBDED, gHHCDAFIKJE.APCAKCCOMLO);
			if (pIHIIMOOICM != null)
			{
				_Camera.FIEBIONJCCI(pIHIIMOOICM);
			}
		}
		EGHPHELLOGO.KJDFJPBIGJC.POCBCFMBKLO = gHHCDAFIKJE.DNGKOMPMPCD;
		EGHPHELLOGO.KJDFJPBIGJC.set_IsShock(gHHCDAFIKJE.APCAKCCOMLO);
		RuleAppliance eJPOJJKKICO = ((!EGHPHELLOGO.KJDFJPBIGJC.EPCNJLEHJCB()) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		UpdateFightDataDamage(gHHCDAFIKJE, eJPOJJKKICO);
		_SelectAnimation.CheckEvent(EventAnimation.EECEJKADLCK.EVENT_HIT, EGHPHELLOGO);
		_SelectAnimation.CheckEvent(EventAnimation.EECEJKADLCK.EVENT_STRIKE, EGHPHELLOGO);
		if (!Module.ELEBLBJKDBI().OMDLOOFIJDF() && EGHPHELLOGO.KJDFJPBIGJC.OKDDOLCHDCM == GameUtils.JOODENKAECE)
		{
			EGHPHELLOGO.KJDFJPBIGJC.ABAOJIMJIDG();
		}
		CheckFightRules(FightEvent.HitEvent, EGHPHELLOGO.KJDFJPBIGJC.EPCNJLEHJCB() ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		CheckFightRules(FightEvent.StrikeEvent, (!EGHPHELLOGO.KJDFJPBIGJC.EPCNJLEHJCB()) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		bool lGNDOAHHHNP = (ObscuredFloat)(kMMJCHDKBDO.KKMCHCNOHMB()) == 0f;
		if (EGHPHELLOGO.GAIBPAGPEGK.EPCNJLEHJCB())
		{
			InfoAnimation dBOLBEOCEME = EGHPHELLOGO.GAIBPAGPEGK.FHBLLPCEAHG();
			MOBFFOHPCOE.NELEDHIIDCG(dBOLBEOCEME, gHHCDAFIKJE.JMDIIIFJMFH, gHHCDAFIKJE.LOONMILKCFK, gHHCDAFIKJE.NIKPBGPPFEP, lGNDOAHHHNP, gHHCDAFIKJE.DFOHNJEBDED, gHHCDAFIKJE.APCAKCCOMLO);
			return;
		}
		MOBFFOHPCOE.OHHKIAMNCKI(gHHCDAFIKJE.DFOHNJEBDED);
		if (gHHCDAFIKJE.APCAKCCOMLO)
		{
			DOANFKMFJFK = true;
		}
	}

	public void PHGNIPMBJEH(Vector3f NAAPALOFBCI, Vector3f KKIKIDNALOL, float time, string AJBGJNMLMKE, float NOOOCHHKECH)
	{
		_Camera.PHGNIPMBJEH(NAAPALOFBCI, KKIKIDNALOL, time, false, AJBGJNMLMKE, NOOOCHHKECH);
	}

	public void BPLPMLGJENF(Model ACENLMONNPA)
	{
		ACENLMONNPA.UpdateAnimationParameters(LNDLFINJHDB);
		ACENLMONNPA.UpdateAnimationParameters(HCPGFOCGDAA);
	}

	public void BIHIGIIOANC()
	{
		KILMEMFHJHH();
	}

	public void PPDEKDMGIMH(object data)
	{
		Model.EventActBtnSettings bOMCDIIDKPD = (Model.EventActBtnSettings)data;
		float num = bOMCDIIDKPD.Value * 100f;
		if (bOMCDIIDKPD.NBIBIANJLEA == FightCID.MagicButton && num > 97f && num < 100f)
		{
			num = 97f;
		}
		ActionButtons actionButtons = KCJNBFLAMCC.GetActionButtons();
		actionButtons.SetNeededPercentageToActBtn(bOMCDIIDKPD.NBIBIANJLEA, num, bOMCDIIDKPD.OCFKLCDIEBF);
	}

	public void BHBGIMOHFPI(object data)
	{
		Model.EventActBtnSettings bOMCDIIDKPD = (Model.EventActBtnSettings)data;
		ActionButtons actionButtons = KCJNBFLAMCC.GetActionButtons();
		actionButtons.SetBulletsCountToActBtn(bOMCDIIDKPD.NBIBIANJLEA, bOMCDIIDKPD.PKMHOICGDIM);
	}

	public Model AMLOPBMHPHC(RuleAppliance EJPOJJKKICO)
	{
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			return _playerModel;
		case RuleAppliance.ApplianceOpponent:
			return CKNCPOABFBO;
		case RuleAppliance.ApplianceAll:
			LLLOJBFMONN.Error("Fight::getModelByAppliance ERROR - wrong appliance {0}", EJPOJJKKICO);
			break;
		}
		return null;
	}

	public void ANAOBOCPCON(float FNDOOJNDJDC, float GBCONNBABLL, PointsTableType NOPJGLHKJPG, int LOMKKEAMMIG, float CFMPJLLNCFF = 100f)
	{
		if (preFight != null)
		{
			preFight.CreatePointsTable(FNDOOJNDJDC, GBCONNBABLL, (int)CFMPJLLNCFF, NOPJGLHKJPG, LOMKKEAMMIG);
		}
	}

	public void UpdatePointsTable(int BBNOPLBAOCF, int HBIKJBGFFBM)
	{
		if (preFight != null)
		{
			preFight.UpdatePointsTable(BBNOPLBAOCF, HBIKJBGFFBM);
		}
	}

	public void GLLAMEEPPHK()
	{
		if (preFight != null)
		{
			preFight.RemovePointsTable();
		}
	}

	public void DGECGHDGPFO()
	{
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			preFight.get_ViewerFight().RemoveCombo();
		}
	}

	public void IFFANEPCAJB(RuleAppliance EJPOJJKKICO)
	{
		Model fGCODGKLHED = null;
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			fGCODGKLHED = _playerModel;
			break;
		case RuleAppliance.ApplianceOpponent:
			fGCODGKLHED = CKNCPOABFBO;
			break;
		}
		fGCODGKLHED.ACJBEOMHFOO();
	}

	public void AJFGKPFJJNL()
	{
		EPBDEDGLHJE.JBOGMAPDLHG();
		GGJJDLNDFLF(BBGAFGNHFEA());
		GGJJDLNDFLF(FHHHIEPAKLP());
	}

	public void GGJJDLNDFLF(Model ACENLMONNPA)
	{
		ACENLMONNPA.KMMJCHDKBDO.AJFGKPFJJNL();
		List<PerkInfoItem> list = BGBDGPDPCMP(ACENLMONNPA.EPCNJLEHJCB());
		foreach (PerkInfoItem item in list)
		{
			ACENLMONNPA.KMMJCHDKBDO.NHBIJEEKALC.Add(item);
		}
		ACENLMONNPA.GLKOLOBIHLP();
		List<NoPerksRule> gOMIMEDNKHH = NDDMGLCJDOB(ACENLMONNPA.EPCNJLEHJCB());
		_rulesInspector.ApplyNoPerksRules(ACENLMONNPA.KMMJCHDKBDO, gOMIMEDNKHH);
		if (!ACENLMONNPA.EPCNJLEHJCB() || KGKDKENMAOA.get_Type() == BattleType.FightRaid)
		{
		}
		EPBDEDGLHJE.AddModel(ACENLMONNPA);
	}

	public List<PerkInfoItem> BGBDGPDPCMP(bool EKBOGDKIHIH)
	{
		return (!EKBOGDKIHIH) ? _rulesInspector.GetEnemyPerks() : _rulesInspector.GetPlayerPerks();
	}

	public List<NoPerksRule> NDDMGLCJDOB(bool EKBOGDKIHIH)
	{
		return (!EKBOGDKIHIH) ? _rulesInspector.GetEnemyNoPerks() : _rulesInspector.GetPlayerNoPerks();
	}

	public void FMNMAOFNGDK()
	{
		FDACGIEEIEE.Sort((Achievement LHBNIMGFKIB, Achievement AAOIAEJJINO) => AAOIAEJJINO.Priority.CompareTo(LHBNIMGFKIB.Priority));
	}

	public void SetBotTactic(string BHNDJOGLEOI)
	{
		CKNCPOABFBO.LFNOLPFIBKC(BHNDJOGLEOI);
	}

	public FightCID GBHGMIBDJGN(FightCID IHNNCICNEJE)
	{
		if (!BLDBGJFBDPJ)
		{
			return IHNNCICNEJE;
		}
		switch (IHNNCICNEJE)
		{
		case FightCID.QuadrantUp:
			return FightCID.QuadrantDown;
		case FightCID.QuadrantUpForward:
			return FightCID.QuadrantDownBack;
		case FightCID.QuadrantForward:
			return FightCID.QuadrantBack;
		case FightCID.QuadrantDownForward:
			return FightCID.QuadrantUpBack;
		case FightCID.QuadrantDown:
			return FightCID.QuadrantUp;
		case FightCID.QuadrantDownBack:
			return FightCID.QuadrantUpForward;
		case FightCID.QuadrantBack:
			return FightCID.QuadrantForward;
		case FightCID.QuadrantUpBack:
			return FightCID.QuadrantDownForward;
		default:
			return IHNNCICNEJE;
		}
	}

	public void BPFBPOCPPCB()
	{
		_Camera.NPFMKCHKGND();
	}

	public virtual void NPMIHDFCBBH(object data)
	{
	}

	public void KBJGEAIPBMF(bool state)
	{
		IOPJDMCBIMM = state;
	}

	public void AOMJIMPGBMO()
	{
		GJIGBLMLJLD();
		GameUtils.StartFight(KGKDKENMAOA, false, KGKDKENMAOA.CNAOMDMIGLJ);
	}

	public void AJKJEMODFGN()
	{
	}

	private void HPIIICCLOON(Model ACENLMONNPA)
	{
	}

	private bool KillModel(bool EKBOGDKIHIH, bool KIDOEGEPDKL)
	{
		if (KGKDKENMAOA.get_Type() == BattleType.FightNone)
		{
			return false;
		}
		List<InfoAnimation> list = new List<InfoAnimation>();
		AnimationData.NEBELEFIDMB("PhysicalFall", list);
		if (list.Count == 0)
		{
			return false;
		}
		InfoAnimation cMGIPKIPIPA = list[0];
		bool flag = false;
		foreach (Model item in LNDLFINJHDB)
		{
			if (item.EPCNJLEHJCB() == EKBOGDKIHIH && item.HIPJNBEFGHN() && !item.KMMJCHDKBDO.BHHLEBHLBLH && item.OCPMJKIEPIG().NMEEPBDJHMG() && item.OCPMJKIEPIG().HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE) == null)
			{
				item.KDAHHIMLJGG.Data = null;
				item.KMMJCHDKBDO.PCALDKCJGCK = true;
				item.GFNCMLFKBGP(0f);
				item.IFDGGKPAHMC(cMGIPKIPIPA, true);
				flag = true;
				if (KIDOEGEPDKL)
				{
					item.EGGEACCDAEK().KMMJCHDKBDO.FCOALLOHJNP = round.roundTotal;
				}
			}
		}
		if (flag && KIDOEGEPDKL && !EKBOGDKIHIH && KGKDKENMAOA.CBJOENICLAF())
		{
			ADJAMFGBOAP = IDAAONBIBJM.Count - 1;
		}
		return flag;
	}

	private void GCFNBENECFD(bool AMKJEICFNFL)
	{
		for (int i = 0; i < LNDLFINJHDB.Count; i++)
		{
			if (AMKJEICFNFL)
			{
				LNDLFINJHDB[i].ChangeSpeed(GameUtils.AJAFNEIPOJB);
			}
			else
			{
				LNDLFINJHDB[i].ChangeSpeed(1f / (float)GameUtils.AJAFNEIPOJB);
			}
		}
	}

	private void GJIGBLMLJLD()
	{
		if (KGKDKENMAOA.get_Type() == BattleType.FightNone)
		{
			return;
		}
		isGameOver = false;
		isStopFight = false;
		ResetParameters();
		round.round = 0;
		BHOPDEJOKOJ(CKNCPOABFBO);
		foreach (ModelParameters item in IDAAONBIBJM)
		{
			item.FCOALLOHJNP = 0;
			item.ALNNLCAKCAF();
			item.IsWinner = false;
			item.PCALDKCJGCK = false;
			item.DKAHKGBFJMG = true;
			item.BHHLEBHLBLH = false;
			item.EAJHPCJJCDI = false;
			item.ABLMGLAKJBL = true;
			item.IDPHHPNCFED = false;
			item.EndRoundType = EndRoundType.EndRoundTypeNone;
		}
		AKBNKDBHCEO = IDAAONBIBJM[0];
		GINNOLEJDFM = AKBNKDBHCEO.HBFMBOHLKPJ;
		ADJAMFGBOAP = 0;
		if (!AssemblyController.JEEFAGGMFCK())
		{
			AKBNKDBHCEO.EEGMBGBLLIF = false;
		}
		CKNCPOABFBO = AddModel(AKBNKDBHCEO);
		ADCBNMPOKOJ();
		ModelParameters kMMJCHDKBDO = _playerModel.KMMJCHDKBDO;
		kMMJCHDKBDO.ALNNLCAKCAF();
		kMMJCHDKBDO.FCOALLOHJNP = 0;
		_Camera.DFKKNMDAFDC(false);
		EPBDEDGLHJE.MIPABIOGDBH(LNDLFINJHDB);
		_SelectAnimation.set_Models(LNDLFINJHDB);
		if (preFight != null)
		{
			preFight.Reset();
			preFight.InitPreFight();
			preFight.ViewerPauseVisible(HNKJALKBCBN());
		}
		SetStage(StageType.FDBBPEGEGMK.STAGE_NONE);
		StartVS();
	}

	private void MLJCABABNDB()
	{
		if (KGKDKENMAOA.get_Type() != BattleType.FightNone)
		{
			isGameOver = false;
			isStopFight = false;
			ResetParameters();
			round.round--;
			BHOPDEJOKOJ(CKNCPOABFBO);
			IDAAONBIBJM[ADJAMFGBOAP].GFNCMLFKBGP(JOEADOFBDOC.PPFGEADDLNN);
			IDAAONBIBJM[ADJAMFGBOAP].FCOALLOHJNP = JOEADOFBDOC.OGOLNFLBLBD;
			AKBNKDBHCEO = IDAAONBIBJM[ADJAMFGBOAP];
			GINNOLEJDFM = AKBNKDBHCEO.HBFMBOHLKPJ;
			if (!AssemblyController.JEEFAGGMFCK())
			{
				AKBNKDBHCEO.EEGMBGBLLIF = false;
			}
			CKNCPOABFBO = AddModel(AKBNKDBHCEO);
			CKNCPOABFBO.OGHAMAGPFLF(JOEADOFBDOC.BNMFCPPJIAG);
			CKNCPOABFBO.FLBDBIHFJAI(JOEADOFBDOC.CPOOPPKHFHB);
			CKNCPOABFBO.KBKIMPEHPKF(JOEADOFBDOC.HCBNOKJFGLN);
			CKNCPOABFBO.PFIJCCKDAAB(JOEADOFBDOC.JAOMELOGOOJ);
			ADCBNMPOKOJ();
			ModelParameters kMMJCHDKBDO = _playerModel.KMMJCHDKBDO;
			kMMJCHDKBDO.GFNCMLFKBGP(JEBNOLKKCIK.PPFGEADDLNN);
			kMMJCHDKBDO.FCOALLOHJNP = JEBNOLKKCIK.OGOLNFLBLBD;
			_playerModel.OGHAMAGPFLF(JEBNOLKKCIK.BNMFCPPJIAG);
			_playerModel.FLBDBIHFJAI(JEBNOLKKCIK.CPOOPPKHFHB);
			_playerModel.KBKIMPEHPKF(JEBNOLKKCIK.HCBNOKJFGLN);
			_playerModel.PFIJCCKDAAB(JEBNOLKKCIK.JAOMELOGOOJ);
			_Camera.DFKKNMDAFDC(false);
			EPBDEDGLHJE.MIPABIOGDBH(LNDLFINJHDB);
			_SelectAnimation.set_Models(LNDLFINJHDB);
			if (preFight != null)
			{
				preFight.Reset();
				preFight.InitPreFight();
				preFight.ViewerPauseVisible(HNKJALKBCBN());
			}
			SetStage(StageType.FDBBPEGEGMK.STAGE_NONE);
			StartVS();
			if (preFight != null)
			{
				preFight.ViewerUpdateVictorys();
			}
		}
	}

	private void IFKELPCCEHC()
	{
	}

	private void BEEEKHIHJPH(GameController GOPFBDGGNGI)
	{
		if (!(GOPFBDGGNGI == null))
		{
			KCJNBFLAMCC = GOPFBDGGNGI;
			KCJNBFLAMCC.AddEventListener(0, ControlPress);
			KCJNBFLAMCC.AddEventListener(1, ControlRelease);
			KCJNBFLAMCC.ResetController();
			KCJNBFLAMCC.Init();
			_Camera.HDFAOMAONJI(KCJNBFLAMCC);
			KCJNBFLAMCC.IsShowController(AssemblyController.PGFJMOGKEID());
			MMOHFIMMFDF(!get_isFightNone());
		}
	}

	private void PreloadEffects()
	{
		for (int i = 0; i < LNDLFINJHDB.Count; i++)
		{
			OPCCHBOGHNO(LNDLFINJHDB[i]);
		}
	}

	private void OPCCHBOGHNO(Model ACENLMONNPA)
	{
		List<InfoAnimation> list = ACENLMONNPA.MCFPDHOLNGB();
		for (int i = 0; i < list.Count; i++)
		{
			InfoAnimation pJAHIOELGGD = list[i];
			for (int j = 0; j < pJAHIOELGGD.ODACDCDONJE.DJBAIAKOIHM.Count; j++)
			{
				ActionAnimation gELPMIAIGDF = pJAHIOELGGD.ODACDCDONJE.DJBAIAKOIHM[j];
				if (gELPMIAIGDF.get_Type() == ActionAnimation.FADAJCEEKIO.CREATE_MODEL)
				{
					ActionCreateModel kPFLDMNAFAP = (ActionCreateModel)gELPMIAIGDF;
					Model fGCODGKLHED = LBNICNOLFGO(ACENLMONNPA, kPFLDMNAFAP.DJBOFEEKJMP(), kPFLDMNAFAP.AEGHBDJDPNA());
					EILHKFPKMOF(fGCODGKLHED);
					HCPGFOCGDAA.Remove(fGCODGKLHED);
				}
			}
		}
		BELLAEIMEAB();
	}

	private void KDMDOBOKAIB(Model ACENLMONNPA, float CKKFKEIELCP)
	{
		string nJFGLOECJEK = GameUtils.PPAEHBGNDNF().Attribute;
		int OEMALIFPGPO = 0;
		if (ACENLMONNPA.KMMJCHDKBDO.IBLHIAHECLK.Get(nJFGLOECJEK, ref OEMALIFPGPO))
		{
			float num = (float)OEMALIFPGPO * GameUtils.PPAEHBGNDNF().Base * CKKFKEIELCP * (ACENLMONNPA.EGGEACCDAEK().LJCFIOPBNKD() / ACENLMONNPA.LJCFIOPBNKD());
			if (num != 0f)
			{
				UpdateLife(ACENLMONNPA, num);
			}
		}
	}

	private string GDADOKEIEIC(Model ACENLMONNPA)
	{
		return string.Empty;
	}

	private string CKAAKEHFAML(Model ACENLMONNPA)
	{
		return null;
	}

	private string PNNJDBONMDP(Model ACENLMONNPA)
	{
		return null;
	}

	private void FKFNHGJNIAA()
	{
		_playerModel = AddModel(NMNCKBPFCCP);
		CKNCPOABFBO = AddModel(AKBNKDBHCEO);
		HFGBKBKNCOB();
		PreloadEffects();
	}

	private Model AddModel(ModelParameters JCICKLIMBEF)
	{
		JCICKLIMBEF.IBBALIJOJMC = SceneTypes.SceneFight;
		Model fGCODGKLHED = new Model(JCICKLIMBEF);
		fGCODGKLHED.CGEKLPLKIDC();
		foreach (Model item in LNDLFINJHDB)
		{
			if (item != fGCODGKLHED)
			{
				if (item == null)
				{
					LLLOJBFMONN.Error("enemy is null");
				}
				fGCODGKLHED.CJNGMIMHFCC(item);
				item.CJNGMIMHFCC(fGCODGKLHED);
			}
		}
		fGCODGKLHED.Index = _Camera.AddModel(fGCODGKLHED, JCICKLIMBEF.IsPlayer, true);
		SetModelOnListening(fGCODGKLHED);
		EPBDEDGLHJE.AddModel(fGCODGKLHED);
		_SelectAnimation.AddModel(fGCODGKLHED);
		LNDLFINJHDB.Add(fGCODGKLHED);
		return fGCODGKLHED;
	}

	private void EABCJLKKPCL()
	{
	}

	private void HGGGBDFFGNM()
	{
		_playerModel.DJLNJPMAHDL().KHFMMPCKMKE(HKCKLJBBNJM(0));
		CKNCPOABFBO.DJLNJPMAHDL().KHFMMPCKMKE(HKCKLJBBNJM(1));
		int num = 0;
		ComboStatistic aBPJBNADBLA = null;
		ComboStatistic aBPJBNADBLA2 = null;
		if (preFight != null)
		{
			num = preFight.get_TimeLeft();
			aBPJBNADBLA = preFight.GetStatistic(0);
			aBPJBNADBLA2 = preFight.GetStatistic(1);
		}
		ModelParameters kIKOGDEPGHB;
		ModelParameters lEBLJJCFKOP;
		if (NMNCKBPFCCP.IsWinner)
		{
			kIKOGDEPGHB = NMNCKBPFCCP;
			lEBLJJCFKOP = AKBNKDBHCEO;
			if (kIKOGDEPGHB.DKAHKGBFJMG)
			{
				_playerModel.DJLNJPMAHDL().POPNNILNKAE();
			}
			LBKDADMLJOE.MHNEKAEGNBO = GameOverTypes.GAME_OVER_WIN;
		}
		else
		{
			MNEOALEBNNA = false;
			kIKOGDEPGHB = AKBNKDBHCEO;
			lEBLJJCFKOP = NMNCKBPFCCP;
			if (kIKOGDEPGHB.DKAHKGBFJMG)
			{
				CKNCPOABFBO.DJLNJPMAHDL().POPNNILNKAE();
			}
			if (num <= 0 && KGKDKENMAOA.get_Type() == BattleType.FightRaid)
			{
				LBKDADMLJOE.MHNEKAEGNBO = GameOverTypes.GAME_OVER_RAID_ROUND_TIMEOUT;
			}
			else
			{
				LBKDADMLJOE.MHNEKAEGNBO = GameOverTypes.GAME_OVER_LOSS;
			}
		}
		CheckCountersEndRound(kIKOGDEPGHB, lEBLJJCFKOP);
		GJMHPBIBHMO = true;
		LBKDADMLJOE.ABKBEJBICOA = kIKOGDEPGHB;
		LBKDADMLJOE.LEBLJJCFKOP = lEBLJJCFKOP;
	}

	private void CMAOBIFAOCI(bool IHBIGLMLKKG = true)
	{
		ResetParameters();
		LOGIFPHMNJM(CKNCPOABFBO);
		int fCOALLOHJNP = AKBNKDBHCEO.FCOALLOHJNP;
		if (IHBIGLMLKKG)
		{
			CKNCPOABFBO.LFNOLPFIBKC(GINNOLEJDFM);
			BHOPDEJOKOJ(CKNCPOABFBO);
			ADJAMFGBOAP++;
			AKBNKDBHCEO = IDAAONBIBJM[ADJAMFGBOAP];
			AKBNKDBHCEO.JJCKADKCDIF = _location.CLGGLBHOMCE;
			AKBNKDBHCEO.FCOALLOHJNP = fCOALLOHJNP;
			GINNOLEJDFM = AKBNKDBHCEO.HBFMBOHLKPJ;
			if (!AssemblyController.JEEFAGGMFCK())
			{
				AKBNKDBHCEO.EEGMBGBLLIF = false;
			}
			_rulesInspector.ApplyNoPerksRules(AKBNKDBHCEO, _rulesInspector.GetEnemyNoPerks());
			CKNCPOABFBO = AddModel(AKBNKDBHCEO);
			_isRoundOver = true;
			PreloadEffects();
			MEDGLEDPHKD(CKNCPOABFBO);
		}
		AKBNKDBHCEO.ALBOCOGOBCN(HNLEDOEPHKG);
		EPBDEDGLHJE.MIPABIOGDBH(LNDLFINJHDB);
		_SelectAnimation.set_Models(LNDLFINJHDB);
		isGameOver = false;
		isStopFight = false;
		if (preFight != null)
		{
			ComboStatistic statistic = preFight.GetStatistic(0);
			ComboStatistic statistic2 = preFight.GetStatistic(1);
			preFight.Reset();
			preFight.InitPreFight(statistic, statistic2);
			preFight.ViewerPauseVisible(HNKJALKBCBN());
		}
		SetStage(StageType.FDBBPEGEGMK.STAGE_NONE);
		StartVS();
	}

	private void EILHKFPKMOF(object data)
	{
		Model fGCODGKLHED = (Model)data;
		if (!fGCODGKLHED.LLBJPPAJOHE())
		{
			IFKFINOGOLC(false);
		}
		JLEFIKJODGG.AddIfNotExist(fGCODGKLHED);
	}

	private void RemoveModelByIndex(int index, Model LEKHCMIFJAO = null)
	{
		int count = LNDLFINJHDB.Count;
		Model fGCODGKLHED = null;
		if (count == 0 || index < 0 || count - 1 < index)
		{
			fGCODGKLHED = LEKHCMIFJAO;
		}
		else
		{
			fGCODGKLHED = LNDLFINJHDB[index];
			LNDLFINJHDB.Remove(fGCODGKLHED);
		}
		RemoveModel(fGCODGKLHED);
	}

	private void BHOPDEJOKOJ(Model ACENLMONNPA)
	{
		int num = 0;
		foreach (Model item in LNDLFINJHDB)
		{
			if (item == ACENLMONNPA)
			{
				RemoveModelByIndex(num);
				return;
			}
			num++;
		}
		RemoveModel(ACENLMONNPA);
	}

	private void IGLLNGNGPOA()
	{
		if (LKNILKJACGJ)
		{
			GFBIBGIOBND(_playerModel);
			GFBIBGIOBND(CKNCPOABFBO);
		}
	}

	private void GFBIBGIOBND(Model ACENLMONNPA)
	{
		float num = ACENLMONNPA.CLDMEJKGLBA().CJELIBMCCMA().ICLEOFDKDIF()
			.GILCBJJPKBK();
		if (!ACENLMONNPA.MBCLINNCNAL())
		{
			if (NCAEOKCFBFD && num >= ICDHAHADCEH && num <= JCCDMOJKANN)
			{
				ACENLMONNPA.BPMKBIKKEOI(true);
				EPBDEDGLHJE.JALOHCICLGN(ACENLMONNPA, PerkEvent.KNKIIEPDCPN.EVENT_AREA_ENTER);
			}
		}
		else if (!NCAEOKCFBFD || num < ICDHAHADCEH || num > JCCDMOJKANN)
		{
			ACENLMONNPA.BPMKBIKKEOI(false);
			EPBDEDGLHJE.JALOHCICLGN(ACENLMONNPA, PerkEvent.KNKIIEPDCPN.EVENT_AREA_EXIT);
		}
	}

	private void ActionModels(bool value)
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.AHBNPODMIOD(value);
		}
	}

	private void ResetModels(bool ABFHKKILGOP)
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.MKAEDALPGDI();
			if (item.NJDJHGDMCIJ() != null)
			{
				EILHKFPKMOF(item);
			}
		}
		EPBDEDGLHJE.Reset();
		_SelectAnimation.Reset();
		BELLAEIMEAB();
	}

	private void StartPunchbag()
	{
		GBJEFAOANBA();
		AJFGKPFJJNL();
		MMOHFIMMFDF(!get_isFightNone());
		StartStance();
		ActionModels(true);
		NetworkController.ELEBLBJKDBI().KDILDKDNIID.Check();
	}

	private void StartVS()
	{
		round.processing = false;
		round.roundTotal = KGKDKENMAOA.BDBBNECNMBP;
		round.time = 0;
		round.timeTotal = (ObscuredInt)(KGKDKENMAOA.RoundTime);
		List<ModelParameters> list = null;
		int num = 0;
		Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
		bool bBBNBKIMHJC = false;
		bool flag = true;
		bool flag2 = true;
		flag2 = !KGKDKENMAOA.CBJOENICLAF() || (NMNCKBPFCCP.FCOALLOHJNP == 0 && AKBNKDBHCEO.FCOALLOHJNP == 0);
		flag = flag2;
		if (cNAOMDMIGLJ.get_Type() == BattleType.FightBosses || cNAOMDMIGLJ.get_Type() == BattleType.FightBossesReplayable || cNAOMDMIGLJ.get_Type() == BattleType.FightFinalTitan)
		{
			list = new List<ModelParameters>();
			foreach (FightList item in cNAOMDMIGLJ.ANNHMNIHKCC())
			{
				List<ModelParameters> list2 = GameUtils.IGNNMAKHBFF(item.OFKJMHPMCCD());
				if (list2.Count > 0)
				{
					list.Add(list2[0]);
				}
			}
			num = KGKDKENMAOA.Index;
			bBBNBKIMHJC = true;
		}
		else
		{
			list = IDAAONBIBJM;
			num = ADJAMFGBOAP;
		}
		if (cNAOMDMIGLJ.get_Type() == BattleType.FightSurvival || cNAOMDMIGLJ.get_Type() == BattleType.FightRaid)
		{
			flag = false;
			flag2 = false;
		}
			bool eNCAKAAMEPN = UnderworldZonePolicy.ShouldShowRoundPips(cNAOMDMIGLJ);
		if (preFight != null)
		{
			preFight.CreateVS(NMNCKBPFCCP, list, num, bBBNBKIMHJC, flag2, flag);
			preFight.ViewerInit(round, NMNCKBPFCCP, AKBNKDBHCEO, eNCAKAAMEPN);
			ScreenModel screenModel = ((!(preFight.get_ViewerFight() != null)) ? null : preFight.get_ViewerFight().get_LeftModel());
			if (screenModel != null)
			{
				screenModel.AddEventListener(0, OnStyleChanged);
			}
			else
			{
				LLLOJBFMONN.Error("Fight - Cant listen to ScreenModel user");
			}
			ScreenModel screenModel2 = ((!(preFight.get_ViewerFight() != null)) ? null : preFight.get_ViewerFight().get_RightModel());
			if (screenModel2 != null)
			{
				screenModel2.AddEventListener(0, OnStyleChanged);
			}
			else
			{
				LLLOJBFMONN.Error("Fight - Cant listen to ScreenModel bot");
			}
		}
	}

	private void NextRound()
	{
		GC.Collect();
		JEBNOLKKCIK.PPFGEADDLNN = (ObscuredFloat)(NMNCKBPFCCP.KKMCHCNOHMB());
		JEBNOLKKCIK.BNMFCPPJIAG = _playerModel.EKAFGLHNMCN();
		JEBNOLKKCIK.CPOOPPKHFHB = _playerModel.LPOJKGLFMAL();
		JEBNOLKKCIK.HCBNOKJFGLN = _playerModel.CKAKLHDLHJO();
		JEBNOLKKCIK.JAOMELOGOOJ = _playerModel.LJCFIOPBNKD();
		JEBNOLKKCIK.OGOLNFLBLBD = NMNCKBPFCCP.FCOALLOHJNP;
		JOEADOFBDOC.PPFGEADDLNN = (ObscuredFloat)(IDAAONBIBJM[ADJAMFGBOAP].KKMCHCNOHMB());
		JOEADOFBDOC.BNMFCPPJIAG = CKNCPOABFBO.EKAFGLHNMCN();
		JOEADOFBDOC.CPOOPPKHFHB = CKNCPOABFBO.LPOJKGLFMAL();
		JOEADOFBDOC.HCBNOKJFGLN = CKNCPOABFBO.CKAKLHDLHJO();
		JOEADOFBDOC.JAOMELOGOOJ = CKNCPOABFBO.LJCFIOPBNKD();
		JOEADOFBDOC.OGOLNFLBLBD = IDAAONBIBJM[ADJAMFGBOAP].FCOALLOHJNP;
		Sound.IBHIPOOHNFK();
		_Camera.KKFIJLOMOJI().JPPGJBHLAGC();
		KFGCODDPNJP();
		isStopFight = false;
		isFirstStrike = false;
		isEndRound = false;
		OEKKLGJMHDD = false;
		BLDBGJFBDPJ = false;
		_endRoundType = EndRoundType.EndRoundTypeNone;
		_endFightRule = null;
		HJCJMEELHPC = 0;
		DOANFKMFJFK = false;
		round.round++;
		if (preFight != null)
		{
			preFight.ClearInscription();
			if (KGKDKENMAOA.get_Type() == BattleType.FightRaid)
			{
				preFight.CreateSkipRound();
			}
			else
			{
				preFight.CreateRound(round.round, false);
			}
		}
		GBJEFAOANBA();
		if (_isRoundOver)
		{
			HFGBKBKNCOB();
			_isRoundOver = false;
		}
		NMNCKBPFCCP.HGLJEBABMIH();
		foreach (Model item in LNDLFINJHDB)
		{
			item.NextRound(round.round);
			item.KMMJCHDKBDO.HANOHOBGGJF();
		}
		EPBDEDGLHJE.DEHPKPPDIIA();
		IFKFINOGOLC(false);
		_isRoundOver = false;
		GC.Collect();
	}

	private void StartStance()
	{
		_Camera.DFKKNMDAFDC(true);
		SetStage(StageType.FDBBPEGEGMK.STAGE_START_STANCE);
	}

	private void FinishStance(ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP, EndRoundType LFLGCDNKNJI)
	{
		HJCJMEELHPC = 0;
		SetStage(StageType.FDBBPEGEGMK.STAGE_END_STANCE);
	}

	private void StartFight()
	{
		if (preFight != null)
		{
			preFight.CreateFight();
		}
	}

	private void PlayFight()
	{
		SetStage(StageType.FDBBPEGEGMK.STAGE_FIGHT);
		if (preFight != null)
		{
			preFight.ViewerPlay();
		}
		ActionModels(true);
		_rulesInspector.RulesActive = true;
		CENFCGAKDOL();
		if (KCNHDABOAAA)
		{
			DODCPKOADGF = !KillModel(false, false);
			KCNHDABOAAA = false;
		}
	}

	private void OnStopPreFight(ScreenFightType data)
	{
		if (preFight == null)
		{
			return;
		}
		switch (preFight.get_Type())
		{
		case ScreenFightType.TYPE_INFO_VS:
			NextRound();
			break;
		case ScreenFightType.TYPE_INFO_ROUND:
		case ScreenFightType.TYPE_INFO_SKIP_ROUND:
			StartStance();
			if (_currentFight.OGNINOBBHIG() != null && _currentFight.OGNINOBBHIG().get_Type() == BattleType.FightRaid && _currentFight.OGNINOBBHIG().GJOAJAIJHOE() != string.Empty && preFight != null)
			{
				preFight.CreateFightRule();
			}
			break;
		case ScreenFightType.TYPE_INFO_FIGHT:
			PlayFight();
			break;
		case ScreenFightType.TYPE_INFO_FIGHT_RULE:
			break;
		}
	}

	private void OnButtonClick(ViewerFight.PLGDCJPCLPN LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case ViewerFight.PLGDCJPCLPN.ButtonPause:
			OpenPauseScreen();
			break;
		case ViewerFight.PLGDCJPCLPN.ButtonPauseSurrender:
			DialogsOpener.OEDGOIHPJJK(SurrenderButtonCallback);
			break;
		case ViewerFight.PLGDCJPCLPN.ButtonPausePlay:
			ClosePauseScreen();
			break;
		default:
			OnCheatClicked(LFLGCDNKNJI);
			break;
		}
	}

	private void SurrenderButtonCallback(object data)
	{
		ClosePauseScreen();
		OBNEDPKCNKJ();
	}

	private void OnCheatClicked(ViewerFight.PLGDCJPCLPN LFLGCDNKNJI)
	{
		if (stageType == StageType.FDBBPEGEGMK.STAGE_FIGHT)
		{
			switch (LFLGCDNKNJI)
			{
			case ViewerFight.PLGDCJPCLPN.ButtonCheatLoseFight:
				KillModel(true, true);
				break;
			case ViewerFight.PLGDCJPCLPN.ButtonCheatWinFight:
				KillModel(false, true);
				break;
			case ViewerFight.PLGDCJPCLPN.ButtonCheatStartBenchmark:
				break;
			case ViewerFight.PLGDCJPCLPN.ButtonCheatWinRound:
			case ViewerFight.PLGDCJPCLPN.ButtonCheatLoseRound:
				break;
			}
		}
	}

	private bool RenderRaidEndFight()
	{
		return KJJGBJCMCFF;
	}

	private void RenderRound()
	{
		if (GJMHPBIBHMO)
		{
			bool flag = LBKDADMLJOE.ABKBEJBICOA.FCOALLOHJNP >= round.roundTotal;
			if (get_IsRaidFight() && flag)
			{
				GJMHPBIBHMO = false;
				OMBDLIKCNIP = false;
				GameOver(LBKDADMLJOE.ABKBEJBICOA, LBKDADMLJOE.LEBLJJCFKOP);
			}
			else
			{
				if (BDDBMCNFNMG || KJKJOJCMDGH)
				{
					return;
				}
				GJMHPBIBHMO = false;
				bool flag2 = LBKDADMLJOE.ABKBEJBICOA.IsPlayer && ADJAMFGBOAP < IDAAONBIBJM.Count - 1;
				bool flag3 = KGKDKENMAOA.CBJOENICLAF();
				OMBDLIKCNIP = false;
				if ((!flag && flag3) || (flag && flag2))
				{
					_Camera.DFKKNMDAFDC(false);
					ResetModels(false);
					bool flag4 = true;
					flag4 = !flag3 || (flag3 && NMNCKBPFCCP == LBKDADMLJOE.ABKBEJBICOA);
					CMAOBIFAOCI(flag4);
					if (flag3)
					{
						preFight.ViewerUpdateVictorys();
					}
					else
					{
						NMNCKBPFCCP.FCOALLOHJNP = 0;
					}
				}
				else if (flag)
				{
					GameOver(LBKDADMLJOE.ABKBEJBICOA, LBKDADMLJOE.LEBLJJCFKOP);
				}
				else
				{
					_Camera.DFKKNMDAFDC(false);
					ResetModels(false);
					ResetParameters();
					NextRound();
				}
			}
		}
		else if (isGameOver)
		{
			if (get_IsRaidFight())
			{
				EndFightRaid();
			}
			else if (!BDDBMCNFNMG && !KJKJOJCMDGH)
			{
				EndFight();
			}
		}
		else if (isStopFight && !isGameOver)
		{
			HGGGBDFFGNM();
		}
		else if (KGKDKENMAOA.get_Type() != BattleType.FightNone && round.processing && (NMNCKBPFCCP.PCALDKCJGCK || AKBNKDBHCEO.PCALDKCJGCK || (preFight != null && preFight.IsTimeOut()) || _endFightRule != null))
		{
			if (FCCPOLAMJNO)
			{
				IFKFINOGOLC(false);
			}
			ActionModels(false);
			round.processing = false;
			if (preFight != null && preFight.IsTimeOut())
			{
				_endRoundType = EndRoundType.EndRoundTypeTimeOut;
				UpdateFightData(FightEvent.TimeoutEvent);
				_rulesInspector.CheckEvent(FightEvent.TimeoutEvent, RuleAppliance.ApplianceAll, DPONLGICLEH);
			}
			EndRound(GetWinner(true), GetWinner(false), _endRoundType);
		}
	}

	private void EndRound(ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP, EndRoundType LFLGCDNKNJI)
	{
		_rulesInspector.RulesActive = false;
		_rulesInspector.StopRules();
		if (LFLGCDNKNJI == EndRoundType.EndRoundTypeTimeOut || LFLGCDNKNJI == EndRoundType.EndRoundTypeRingOut || LFLGCDNKNJI == EndRoundType.EndRoundTypeLose)
		{
			bool flag = false;
			if (_endFightRule != null)
			{
				switch (_endFightRule.IMINMDOFHMG())
				{
				case RuleAppliance.AppliancePlayer:
					flag = true;
					break;
				case RuleAppliance.ApplianceOpponent:
					flag = false;
					break;
				}
			}
			if (flag)
			{
				ABKBEJBICOA = NMNCKBPFCCP;
				LEBLJJCFKOP = AKBNKDBHCEO;
			}
			else
			{
				MNEOALEBNNA = false;
				LEBLJJCFKOP = NMNCKBPFCCP;
				ABKBEJBICOA = AKBNKDBHCEO;
			}
		}
		ABKBEJBICOA.FCOALLOHJNP++;
		ABKBEJBICOA.IsWinner = true;
		ABKBEJBICOA.BHHLEBHLBLH = true;
		ABKBEJBICOA.EndRoundType = LFLGCDNKNJI;
		LEBLJJCFKOP.IsWinner = false;
		LEBLJJCFKOP.BHHLEBHLBLH = true;
		LEBLJJCFKOP.EndRoundType = LFLGCDNKNJI;
		HJCJMEELHPC = 0;
		if (preFight != null)
		{
			preFight.ViewerUpdateVictorys();
		}
		IDMICHMHCKE = false;
		IFKFINOGOLC(false);
		FinishStance(ABKBEJBICOA, LEBLJJCFKOP, LFLGCDNKNJI);
	}

	private void FinishRound()
	{
		isStopFight = true;
	}

	private void GameOver(ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP)
	{
		MDJEDDJCGGE(true);
		if (KGKDKENMAOA.get_Type() != BattleType.FightRaid)
		{
			ResetParameters();
		}
		isGameOver = true;
		KGKDKENMAOA.JABJLCEJDDM = ADJAMFGBOAP;
		CheckCountersStopFight(ABKBEJBICOA, LEBLJJCFKOP);
		if (FDACGIEEIEE.Count > 0 || KJKJOJCMDGH)
		{
			BDDBMCNFNMG = true;
		}
	}

	private ModelParameters GetWinner(bool PLGGPKEJPPJ)
	{
		if (_endRoundType != EndRoundType.EndRoundTypeZeroHealth && _endFightRule != null)
		{
			switch (_endFightRule.IMINMDOFHMG())
			{
			case RuleAppliance.AppliancePlayer:
				return NMNCKBPFCCP;
			case RuleAppliance.ApplianceOpponent:
				return AKBNKDBHCEO;
			}
		}
		if ((ObscuredFloat)(NMNCKBPFCCP.KKMCHCNOHMB()) <= (ObscuredFloat)(AKBNKDBHCEO.KKMCHCNOHMB()))
		{
			return (!PLGGPKEJPPJ) ? NMNCKBPFCCP : AKBNKDBHCEO;
		}
		return (!PLGGPKEJPPJ) ? AKBNKDBHCEO : NMNCKBPFCCP;
	}

	private void ResetParameters()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.MLIIBCBGHBH(false);
			item.set_IsShock(false);
			ModelParameters kMMJCHDKBDO = item.KMMJCHDKBDO;
			if (!OMBDLIKCNIP)
			{
				kMMJCHDKBDO.ALNNLCAKCAF(KGKDKENMAOA.OMFDJPFGKAB);
			}
			kMMJCHDKBDO.IsWinner = false;
			kMMJCHDKBDO.PCALDKCJGCK = false;
			kMMJCHDKBDO.DKAHKGBFJMG = true;
			kMMJCHDKBDO.BHHLEBHLBLH = false;
			kMMJCHDKBDO.EAJHPCJJCDI = false;
			kMMJCHDKBDO.ABLMGLAKJBL = true;
			kMMJCHDKBDO.IDPHHPNCFED = false;
			kMMJCHDKBDO.EndRoundType = EndRoundType.EndRoundTypeNone;
			kMMJCHDKBDO.NOBKKLBJFIL();
		}
		if (!OMBDLIKCNIP)
		{
			OMBDLIKCNIP = true;
		}
		ScreenModel screenModel = null;
		ScreenModel screenModel2 = null;
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			screenModel = preFight.get_ViewerFight().get_LeftModel();
			screenModel2 = preFight.get_ViewerFight().get_RightModel();
		}
		if (screenModel != null)
		{
			screenModel.IBKPFLEMEAJ();
		}
		if (screenModel2 != null)
		{
			screenModel2.IBKPFLEMEAJ();
		}
	}

	private void KCACCJNMOFM(Model.EventModel EGHPHELLOGO)
	{
		if (!round.processing || KGKDKENMAOA.get_Type() == BattleType.FightNone || FCCPOLAMJNO || LKCNBFEINCM || !EGHPHELLOGO.KJDFJPBIGJC.LLBJPPAJOHE())
		{
			return;
		}
		IntervalAnimation mNOIEOBBCMI = (IntervalAnimation)EGHPHELLOGO.Data;
		if (mNOIEOBBCMI.Type != IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
		{
			return;
		}
		if (EGHPHELLOGO.GAIBPAGPEGK == null)
		{
			LLLOJBFMONN.Error("Enemy for slowmode not found");
			return;
		}
		float num = EGHPHELLOGO.GAIBPAGPEGK.GetTotalDamage((IntervalAttack)mNOIEOBBCMI, false, false, null);
		float num2 = EGHPHELLOGO.GAIBPAGPEGK.KMMJCHDKBDO.RemainingHealthInDamageUnits;
		if (num2 <= num)
		{
			EGHPHELLOGO.KJDFJPBIGJC.FHMLAFHENBB(false);
			LKCNBFEINCM = true;
		}
	}

	private void CMMLPNPPGKH(Model.EventModel EGHPHELLOGO)
	{
		IntervalAnimation mNOIEOBBCMI = (IntervalAnimation)EGHPHELLOGO.Data;
		bool flag = mNOIEOBBCMI.Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK;
		flag = flag;
		if (FCCPOLAMJNO && flag)
		{
			IFKFINOGOLC(false);
		}
	}

	public void IFKFINOGOLC(bool value)
	{
		if (FCCPOLAMJNO != value)
		{
			FCCPOLAMJNO = value || IDMICHMHCKE;
			GameUtils.CEPJBBGGMDP((!FCCPOLAMJNO) ? 1 : GameUtils.AJAFNEIPOJB);
			if (FCCPOLAMJNO)
			{
				LKCNBFEINCM = false;
				GCFNBENECFD(!FCCPOLAMJNO);
			}
			else
			{
				GCFNBENECFD(FCCPOLAMJNO);
			}
		}
	}

	private void PAIOMLKCNOP()
	{
		if (LKCNBFEINCM)
		{
			IFKFINOGOLC(true);
		}
	}

	private FightStatistics.EMKEIEJMONM HKCKLJBBNJM(int index)
	{
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			return preFight.get_ViewerFight().GetScreenModel(index).MaxStyle;
		}
		return FightStatistics.EMKEIEJMONM.STYLE_AGGRESSIVE;
	}

	private void MOFKFJCIBGC(object data)
	{
		if (preFight != null)
		{
			preFight.OnFightPause(true);
		}
		isRenderFight = false;
	}

	private void OCFDPNKALIJ(object data)
	{
		if (preFight != null)
		{
			preFight.OnFightPause(false);
		}
		isRenderFight = true;
	}

	private void BPFFCNAGLCN()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.PKFJFFGDOLB = false;
		}
	}

	private void DGNDJBDKNAI()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.DGNDJBDKNAI();
		}
	}

	private void ODNEEGLKKCK()
	{
		_rulesInspector = new RulesInspector(this, KGKDKENMAOA);
		_rulesInspector.CurrentRound = round.round;
	}

	private void GBJEFAOANBA()
	{
		CKNCPOABFBO.LFNOLPFIBKC(GINNOLEJDFM);
		if (round.round > 1)
		{
			InitRules();
		}
		PFNHDCIOJKJ();
		_rulesInspector.ApplyNoAnimationRules(NMNCKBPFCCP);
		EIICPBKOIMO();
		LGKCGLEIODH();
		AKBNKDBHCEO.NOBKKLBJFIL();
		ApplyRules();
		MMOHFIMMFDF();
		if (KCJNBFLAMCC != null)
		{
			KCJNBFLAMCC.ClearButtonsAppearance();
			_rulesInspector.CheckButtonRules(KCJNBFLAMCC);
		}
		AJFGKPFJJNL();
	}

	private void ADCBNMPOKOJ()
	{
		LOGIFPHMNJM(_playerModel);
		JLEFIKJODGG.AddIfNotExist(_playerModel);
		BELLAEIMEAB();
		_playerModel = AddModel(NMNCKBPFCCP);
		MEDGLEDPHKD(_playerModel);
		OPCCHBOGHNO(_playerModel);
		int num = 0;
		foreach (Model item in LNDLFINJHDB)
		{
			item.Index = num;
			num++;
		}
		HCPGFOCGDAA.Clear();
		BELLAEIMEAB();
		ResetModels(false);
		ResetParameters();
		_isRoundOver = true;
	}

	private void EIICPBKOIMO()
	{
		EquippedItemsStruct pFMMOILIHMP = new EquippedItemsStruct();
		EquippedItemsStruct pFMMOILIHMP2 = new EquippedItemsStruct();
		NMNCKBPFCCP.ALBOCOGOBCN(pFMMOILIHMP);
		NMNCKBPFCCP.ALGDEEKFPKK(IEJFDGHCOON);
		NMNCKBPFCCP.NOBKKLBJFIL();
		GFAOMMLPKAN(NMNCKBPFCCP);
		NMNCKBPFCCP.ALBOCOGOBCN(pFMMOILIHMP2);
		if (!pFMMOILIHMP2.Compare(pFMMOILIHMP))
		{
			ADCBNMPOKOJ();
			NMNCKBPFCCP.IBLHIAHECLK = CIFHAMACGFJ.IBLHIAHECLK;
		}
	}

	private void LCPGIDLMDEJ()
	{
		LOGIFPHMNJM(CKNCPOABFBO);
		JLEFIKJODGG.AddIfNotExist(CKNCPOABFBO);
		BELLAEIMEAB();
		CKNCPOABFBO = AddModel(AKBNKDBHCEO);
		MEDGLEDPHKD(CKNCPOABFBO);
		OPCCHBOGHNO(CKNCPOABFBO);
		int num = 0;
		foreach (Model item in LNDLFINJHDB)
		{
			item.Index = num;
			num++;
		}
		HCPGFOCGDAA.Clear();
		BELLAEIMEAB();
		ResetModels(false);
		ResetParameters();
		_isRoundOver = true;
	}

	private void LGKCGLEIODH()
	{
		EquippedItemsStruct pFMMOILIHMP = new EquippedItemsStruct();
		EquippedItemsStruct pFMMOILIHMP2 = new EquippedItemsStruct();
		AKBNKDBHCEO.ALBOCOGOBCN(pFMMOILIHMP);
		AKBNKDBHCEO.ALGDEEKFPKK(HNLEDOEPHKG);
		AKBNKDBHCEO.NOBKKLBJFIL();
		GFAOMMLPKAN(AKBNKDBHCEO);
		AKBNKDBHCEO.ALBOCOGOBCN(pFMMOILIHMP2);
		if (!pFMMOILIHMP2.Compare(pFMMOILIHMP))
		{
			LCPGIDLMDEJ();
			AKBNKDBHCEO.IBLHIAHECLK = CIFHAMACGFJ.IBLHIAHECLK;
		}
	}

	private void CheckFightRules(FightEvent KOJNCHKPLLN, RuleAppliance EJPOJJKKICO)
	{
		UpdateFightData(KOJNCHKPLLN);
		if (_rulesInspector != null)
		{
			_rulesInspector.CheckEvent(KOJNCHKPLLN, EJPOJJKKICO, DPONLGICLEH);
		}
	}

	private void ANGEEDBHCKJ()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.ACJBEOMHFOO();
		}
	}

	private void MMOHFIMMFDF(bool APDPBLADDCN = true)
	{
		if (!(KCJNBFLAMCC == null))
		{
			if (!APDPBLADDCN || round.round == 1)
			{
				IPILDDCKHMP();
				BIHIGIIOANC();
				BNAGOFIAABA();
			}
			else
			{
				FINFDFAMMDJ();
				KILMEMFHJHH();
				FELJFJOEJNC();
			}
		}
	}

	private void SetModelOnListening(Model ACENLMONNPA)
	{
		bool flag = ACENLMONNPA.KIAFPPHPEEK();
		ACENLMONNPA.SetWalls(GameUtils.CKOPPGCIHPL(), GameUtils.FBOGLADLJML(), (!flag) ? 100 : 0, (!flag) ? 30 : 0);
		ACENLMONNPA.AddEventListener(2, OnAnimationStart);
		ACENLMONNPA.AddEventListener(3, OnAnimationEnd);
		ACENLMONNPA.AddEventListener(0, OnIntervalStart);
		ACENLMONNPA.AddEventListener(1, OnIntervalEnd);
		ACENLMONNPA.AddEventListener(4, OnEveryFrame);
		ACENLMONNPA.AddEventListener(5, EILHKFPKMOF);
		ACENLMONNPA.AddEventListener(6, DEIOPLMPOHK);
		ACENLMONNPA.AddEventListener(12, PPDEKDMGIMH);
		ACENLMONNPA.AddEventListener(18, BHBGIMOHFPI);
		ACENLMONNPA.AddEventListener(13, GKLNFHKKIAI);
		ACENLMONNPA.AddEventListener(15, IBONKBLOKNM);
		ACENLMONNPA.AddEventListener(16, HLIOEELKFCP);
		ACENLMONNPA.AddEventListener(17, EPLCECBMOOB);
	}

	private void UpdateFightData(FightEvent KOJNCHKPLLN = FightEvent.NoneEvent)
	{
		DPONLGICLEH.MPLPEMOFHGI.KOJNCHKPLLN = KOJNCHKPLLN;
		DPONLGICLEH.EKBMBILHBMC.KOJNCHKPLLN = KOJNCHKPLLN;
		DPONLGICLEH.MPLPEMOFHGI.LKLHCEEMINM = _playerModel.FHBLLPCEAHG();
		DPONLGICLEH.EKBMBILHBMC.LKLHCEEMINM = CKNCPOABFBO.FHBLLPCEAHG();
		DPONLGICLEH.MPLPEMOFHGI.DPBGICDNFAM = (FightStatistics.EMKEIEJMONM)_playerModel.PACHBHGEIGN;
		DPONLGICLEH.EKBMBILHBMC.DPBGICDNFAM = (FightStatistics.EMKEIEJMONM)CKNCPOABFBO.PACHBHGEIGN;
		DPONLGICLEH.MPLPEMOFHGI.CBLNOFELDOE = _playerModel.LAGNKLAADPO();
		DPONLGICLEH.MPLPEMOFHGI.JGNIIBBNIEI = CKNCPOABFBO.LAGNKLAADPO();
		DPONLGICLEH.EKBMBILHBMC.CBLNOFELDOE = CKNCPOABFBO.LAGNKLAADPO();
		DPONLGICLEH.EKBMBILHBMC.JGNIIBBNIEI = _playerModel.LAGNKLAADPO();
		DPONLGICLEH.MPLPEMOFHGI.OGOFFCEGLHJ = _playerModel.EDJFLMILEBA();
		DPONLGICLEH.MPLPEMOFHGI.PFFJNBOFMLI = CKNCPOABFBO.EDJFLMILEBA();
		DPONLGICLEH.EKBMBILHBMC.OGOFFCEGLHJ = CKNCPOABFBO.EDJFLMILEBA();
		DPONLGICLEH.EKBMBILHBMC.PFFJNBOFMLI = _playerModel.EDJFLMILEBA();
		DPONLGICLEH.SlowMode = GameUtils.GGBABPJBGJB();
	}

	private void UpdateFightDataDamage(Model.StrikeResult PPIAOBPLGOK, RuleAppliance EJPOJJKKICO)
	{
		FightData hCPJJKMNMCE = null;
		FightData hCPJJKMNMCE2 = null;
		switch (EJPOJJKKICO)
		{
		case RuleAppliance.AppliancePlayer:
			hCPJJKMNMCE = DPONLGICLEH.MPLPEMOFHGI;
			hCPJJKMNMCE2 = DPONLGICLEH.EKBMBILHBMC;
			break;
		case RuleAppliance.ApplianceOpponent:
			hCPJJKMNMCE = DPONLGICLEH.EKBMBILHBMC;
			hCPJJKMNMCE2 = DPONLGICLEH.MPLPEMOFHGI;
			break;
		default:
			LLLOJBFMONN.Error("Fight::updateFightDataDamage ERROR - wrong RuleAppliance %i", EJPOJJKKICO);
			return;
		}
		hCPJJKMNMCE.OJIKDIDLBAF = PPIAOBPLGOK.EEDJBBOCFNL;
		hCPJJKMNMCE.PCMJEFDLCOB = 0f;
		hCPJJKMNMCE.ONBMPLCEONN = true;
		hCPJJKMNMCE.FIJOEIOHJFA = PPIAOBPLGOK.DFOHNJEBDED;
		hCPJJKMNMCE.IDAJOBOKPPP = PPIAOBPLGOK.DNGKOMPMPCD;
		hCPJJKMNMCE.BNPGBHPDGHM = PPIAOBPLGOK.JMDIIIFJMFH;
		hCPJJKMNMCE2.PCMJEFDLCOB = PPIAOBPLGOK.EEDJBBOCFNL;
		hCPJJKMNMCE2.OJIKDIDLBAF = 0f;
		hCPJJKMNMCE2.ONBMPLCEONN = false;
		hCPJJKMNMCE2.FIJOEIOHJFA = PPIAOBPLGOK.DFOHNJEBDED;
	}

	private void CheckCountersStopFight(ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP)
	{
		BattleType pJMEMGHKKBM = KGKDKENMAOA.get_Type();
		Battle cNAOMDMIGLJ = KGKDKENMAOA.CNAOMDMIGLJ;
		int num = cNAOMDMIGLJ.ANNHMNIHKCC().Count - 1;
		int gCAABNKEIBN = KGKDKENMAOA.Index;
		bool jEDBJFMHGCH = ABKBEJBICOA.IsPlayer;
		if (jEDBJFMHGCH)
		{
			if (pJMEMGHKKBM == BattleType.FightBosses || pJMEMGHKKBM == BattleType.FightBossesReplayable || pJMEMGHKKBM == BattleType.FightFinalTitan)
			{
				bool flag = gCAABNKEIBN == num;
				if (!flag)
				{
					MOBFFOHPCOE.GEAEKJJBMDG();
				}
				else if (flag)
				{
					MOBFFOHPCOE.MGKKANDMALJ();
				}
				if (flag && MNEOALEBNNA)
				{
					MOBFFOHPCOE.DFONENABHBO();
				}
			}
			else if (pJMEMGHKKBM == BattleType.FightTournament && gCAABNKEIBN == num)
			{
				MOBFFOHPCOE.GFENMJJDLCL();
			}
			else if (pJMEMGHKKBM == BattleType.FightChallenge && gCAABNKEIBN == num)
			{
				MOBFFOHPCOE.MGANFEMKLPM();
			}
			else if (pJMEMGHKKBM == BattleType.FightAscension && gCAABNKEIBN == num)
			{
				MOBFFOHPCOE.LAHGOBJIOOG();
			}
			MOBFFOHPCOE.IHANMCFEJJG(KGKDKENMAOA.BCKFACGMOKC);
			MOBFFOHPCOE.MEFALNAFBNG(KGKDKENMAOA.BCKFACGMOKC);
			MOBFFOHPCOE.PIPGPHELPPK();
		}
		else
		{
			MOBFFOHPCOE.PMKNEKPKFFA();
		}
		NLKDFBEAEEH(jEDBJFMHGCH);
		if (pJMEMGHKKBM == BattleType.FightSurvival || pJMEMGHKKBM == BattleType.FightRaid)
		{
			MOBFFOHPCOE.JKOBOBJMDDE((!jEDBJFMHGCH) ? ADJAMFGBOAP : (ADJAMFGBOAP + 1));
		}
		MOBFFOHPCOE.Complete(round.roundTotal);
	}

	private void CheckCountersEndRound(ModelParameters ABKBEJBICOA, ModelParameters LEBLJJCFKOP)
	{
		Model nPPONCJECLA = _playerModel;
		if (ABKBEJBICOA.IsPlayer)
		{
			int bAINMLLIKOL = (ObscuredInt)(KGKDKENMAOA.RoundTime) - preFight.get_TimeLeft();
			ComboStatistic statistic = preFight.GetStatistic(0);
			MOBFFOHPCOE.SetTime(bAINMLLIKOL);
			float bAINMLLIKOL2 = (ObscuredFloat)(ABKBEJBICOA.KKMCHCNOHMB());
			MOBFFOHPCOE.SetLife(bAINMLLIKOL2);
			if (ABKBEJBICOA.DKAHKGBFJMG)
			{
				MOBFFOHPCOE.DMBJKBBFMPH();
			}
			if (DOANFKMFJFK)
			{
				MOBFFOHPCOE.NDJHKKLEGPC();
			}
		}
		MOBFFOHPCOE.MLJCABABNDB();
	}

	private GameUtils.HitEffect PPCKJAOGBHO(bool EDKDBAJCEHI, bool IFCOPPPDOCD, bool EPKEEMFHHFM)
	{
		foreach (GameUtils.HitEffect item in GameUtils.OCMEOOKALHM().EOPFGIDLHKP)
		{
			if (EPKEEMFHHFM && item.Type == "Shock")
			{
				return item;
			}
			if (EDKDBAJCEHI && item.Type == "CriticalHit")
			{
				return item;
			}
			if (IFCOPPPDOCD && item.Type == "HeadHit")
			{
				return item;
			}
		}
		return null;
	}

	private void GJJLEFLCOFL(object data)
	{
		if (!KGKDKENMAOA.ANIFGJGHNLN)
		{
			return;
		}
		CountersFight.CurrentCounter pEMLBKDIDHA = (CountersFight.CurrentCounter)data;
		if (pEMLBKDIDHA != null)
		{
			Achievement jNPIOKEKMII = GameUtils.EKBBPLEHGHD(pEMLBKDIDHA.EOGLBDCLMBM, pEMLBKDIDHA.Value);
			if (jNPIOKEKMII != null)
			{
				FDACGIEEIEE.Add(jNPIOKEKMII);
				PEOIALGBJFB = true;
			}
		}
	}

	private void OnStyleChanged(object data)
	{
		ModelStyleChange lONCJPNBHEA = (ModelStyleChange)data;
		int kNBKAELNFDD = lONCJPNBHEA.StyleIndex;
		string bPJNHNCOPOP = lONCJPNBHEA.StyleName;
		float hFKPJPBCIEK = lONCJPNBHEA.StyleGain;
		bool oJAHEEIFMBM = lONCJPNBHEA.IsHit;
		Model fGCODGKLHED = IEECMHLHIAC(lONCJPNBHEA.KJDFJPBIGJC);
		if (kNBKAELNFDD != fGCODGKLHED.PACHBHGEIGN)
		{
			if (lONCJPNBHEA.KJDFJPBIGJC == ScreenModel.JEDPGMIGGKK.TYPE_LEFT)
			{
				MOBFFOHPCOE.HFCLLLHJBGH(kNBKAELNFDD);
			}
			EPBDEDGLHJE.JALOHCICLGN(fGCODGKLHED, PerkEvent.KNKIIEPDCPN.EVENT_STYLE);
			fGCODGKLHED.OnStyleChanged(kNBKAELNFDD, bPJNHNCOPOP, hFKPJPBCIEK, oJAHEEIFMBM);
			RuleAppliance eJPOJJKKICO = ((lONCJPNBHEA.KJDFJPBIGJC == ScreenModel.JEDPGMIGGKK.TYPE_LEFT) ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
			CheckFightRules(FightEvent.CrazyEvent, eJPOJJKKICO);
		}
	}

	private void LDBBHGDELIJ(PerksStage.PerkEventStruct data)
	{
		data.KJDFJPBIGJC.KDAHHIMLJGG.Data = data.Data;
		_SelectAnimation.CheckEvent(EventAnimation.EECEJKADLCK.EVENT_MOD_EXPIRES, data.KJDFJPBIGJC.KDAHHIMLJGG);
	}

	private int JHMIAONIAPN(ScreenModel.JEDPGMIGGKK LFLGCDNKNJI)
	{
		int result = 0;
		switch (LFLGCDNKNJI)
		{
		case ScreenModel.JEDPGMIGGKK.TYPE_LEFT:
			result = 0;
			break;
		case ScreenModel.JEDPGMIGGKK.TYPE_RIGHT:
			result = 1;
			break;
		default:
			LLLOJBFMONN.Error("Fight::getModelIndexByScreenModel - Unknown model: %i", LFLGCDNKNJI);
			break;
		}
		return result;
	}

	private Model IEECMHLHIAC(ScreenModel.JEDPGMIGGKK LFLGCDNKNJI)
	{
		Model result = null;
		switch (LFLGCDNKNJI)
		{
		case ScreenModel.JEDPGMIGGKK.TYPE_LEFT:
			result = _playerModel;
			break;
		case ScreenModel.JEDPGMIGGKK.TYPE_RIGHT:
			result = CKNCPOABFBO;
			break;
		default:
			LLLOJBFMONN.Error("Fight::getModelByScreenModel - Unknown model: %i", LFLGCDNKNJI);
			break;
		}
		return result;
	}

	private void GKLNFHKKIAI(object data)
	{
		Model fGCODGKLHED = (Model)data;
		if (fGCODGKLHED != _playerModel && fGCODGKLHED != CKNCPOABFBO)
		{
			LLLOJBFMONN.Error("Fight::onUserComboIncrease ERROR - Model is not player nor enemy");
			return;
		}
		int num = fGCODGKLHED.NPDOLGNNINO();
		FightData hCPJJKMNMCE = ((!fGCODGKLHED.EPCNJLEHJCB()) ? DPONLGICLEH.EKBMBILHBMC : DPONLGICLEH.MPLPEMOFHGI);
		hCPJJKMNMCE.currentComboLevel = num;
		CheckFightRules(FightEvent.ComboEvent, fGCODGKLHED.EPCNJLEHJCB() ? RuleAppliance.AppliancePlayer : RuleAppliance.ApplianceOpponent);
		if (fGCODGKLHED.EPCNJLEHJCB())
		{
			MOBFFOHPCOE.MKIPHHMHIOC(fGCODGKLHED.CLPDEPPPJFE());
		}
		EPBDEDGLHJE.JALOHCICLGN(fGCODGKLHED, PerkEvent.KNKIIEPDCPN.EVENT_COMBO);
		int hIGBAPPOOKJ = fGCODGKLHED.HIGBAPPOOKJ;
		if (preFight != null)
		{
			preFight.get_ViewerFight().UpdateCombo(fGCODGKLHED.EPCNJLEHJCB(), num, hIGBAPPOOKJ);
		}
	}

	private void KGKPLKJPDAI()
	{
		if (!KJKJOJCMDGH)
		{
			KJKJOJCMDGH = true;
			FMNMAOFNGDK();
			Achievement jNPIOKEKMII = FDACGIEEIEE[0];
			FDACGIEEIEE.Remove(jNPIOKEKMII);
			if (preFight != null)
			{
				preFight.ShowAchievementMessage(jNPIOKEKMII);
			}
			else
			{
				BMELKMACHCM();
			}
		}
	}

	private void BMELKMACHCM()
	{
		KJKJOJCMDGH = false;
		if (FDACGIEEIEE.Count == 0)
		{
			BDDBMCNFNMG = false;
		}
		else
		{
			KGKPLKJPDAI();
		}
	}

	private void IBONKBLOKNM(object data)
	{
		ActionShakeScreen oEAOHCOKDPF = (ActionShakeScreen)data;
		_Camera.FIEBIONJCCI(oEAOHCOKDPF.CBNIELBJDAO());
	}

	private void EPLCECBMOOB(object data)
	{
		ActionZoomEffect pHFCNOBALGE = (ActionZoomEffect)data;
		_Camera.FFIAMGHGPPA(pHFCNOBALGE.DJDCBEMKLIP());
	}

	private void OBNEDPKCNKJ()
	{
		HCNDAFDHACI(GameOverTypes.GAME_OVER_SURRENDER);
	}

	private void HCNDAFDHACI(GameOverTypes MHNEKAEGNBO)
	{
		Sound.IBHIPOOHNFK();
		MOBFFOHPCOE.Complete(round.roundTotal, true);
		MOBFFOHPCOE.HOCBEHCHOFL(true);
		ComboStatistic aIOMDIAFHGB = null;
		ComboStatistic mOJHPBGGNAH = null;
		int num = 0;
		if (preFight != null)
		{
			aIOMDIAFHGB = preFight.GetStatistic(0);
			mOJHPBGGNAH = preFight.GetStatistic(1);
			num = preFight.get_TimeLeft();
		}
		if (KGKDKENMAOA.get_Type() != BattleType.FightRaid)
		{
			GameUtils.EndFight(aIOMDIAFHGB, KGKDKENMAOA, null, null, MHNEKAEGNBO, mOJHPBGGNAH, DKDMOJJJHHL);
		}
		if (KGKDKENMAOA.ANIFGJGHNLN)
		{
			ListSF.CCDKHLAMKKO().KJNPJKEHGLE().BFCLLIKOJGD();
		}
	}

	private void OpenPauseScreen()
	{
		if (preFight != null)
		{
			JOJIDODPDLA(true);
			preFight.OpenPauseScreen();
		}
	}

	private void ClosePauseScreen()
	{
		if (preFight != null)
		{
			JOJIDODPDLA(false);
			preFight.ClosePauseScreen();
		}
	}

	public void BCFBHJOLGNL(FightResult DCJLKCFKCOM)
	{
		if (preFight != null)
		{
			JOJIDODPDLA(true);
			preFight.OpenEndFightScreen(DCJLKCFKCOM);
		}
	}

	private void OLINHHIJCDL(Model ACENLMONNPA)
	{
		if (ACENLMONNPA != null)
		{
			ACENLMONNPA.Index = _Camera.AddModel(ACENLMONNPA, false, false);
			HCPGFOCGDAA.Add(ACENLMONNPA);
			SetModelOnListening(ACENLMONNPA);
			EPBDEDGLHJE.AddModel(ACENLMONNPA);
			ACENLMONNPA.KKLMIAFFKNE(false);
		}
	}

	private Model LBNICNOLFGO(Model MDKDAHCNCMC, List<CopyItemInfo> HELFDCAIJNE = null, string JLHDJLHLGND = "")
	{
		if (HELFDCAIJNE == null)
		{
			HELFDCAIJNE = new List<CopyItemInfo>();
		}
		return MDKDAHCNCMC.EGFIFHKBNML(HELFDCAIJNE, JLHDJLHLGND);
	}

	private void DEIOPLMPOHK(object data)
	{
		Model aCENLMONNPA = (Model)data;
		OLINHHIJCDL(aCENLMONNPA);
	}

	private void BELLAEIMEAB()
	{
		foreach (Model item in JLEFIKJODGG)
		{
			BHOPDEJOKOJ(item);
		}
		JLEFIKJODGG.Clear();
	}

	private void RemoveModel(Model ACENLMONNPA)
	{
		if (ACENLMONNPA == null)
		{
			LLLOJBFMONN.Error("Fight::removeModel - cant find model");
			return;
		}
		Model fGCODGKLHED = ACENLMONNPA.NJDJHGDMCIJ();
		if (fGCODGKLHED != null)
		{
			fGCODGKLHED.MGGBIBAHDEE((WeaponModel)ACENLMONNPA);
		}
		_Camera.RemoveObject(ACENLMONNPA);
		foreach (Model item in LNDLFINJHDB)
		{
			item.CNIAJPBJHIM(ACENLMONNPA);
			item.SetNearestEnemy();
		}
		EPBDEDGLHJE.RemoveModel(ACENLMONNPA);
		_SelectAnimation.RemoveModel(ACENLMONNPA);
		ACENLMONNPA.FKIBECCHIJC();
		ACENLMONNPA.IMFOFFFLGOM();
		ACENLMONNPA.RemoveAllEventListener();
	}

	private void GFAOMMLPKAN(ModelParameters JCICKLIMBEF)
	{
		CIFHAMACGFJ = JCICKLIMBEF;
		int num = round.round;
		if (num < 1)
		{
			num = 1;
		}
		List<ItemRule> list = ((!JCICKLIMBEF.IsPlayer) ? _rulesInspector.GetEnemyItemRules() : _rulesInspector.GetPlayerItemRules());
		_rulesInspector.PrepareItemRules(list);
		if (_rulesInspector != null)
		{
			JCICKLIMBEF.KMPACCIOOLE(list, false, num);
			CIFHAMACGFJ.KMPACCIOOLE(list, true, num);
		}
		JCICKLIMBEF.PPFDLIBLNDG();
		CIFHAMACGFJ.NOBKKLBJFIL();
		JCICKLIMBEF.IBLHIAHECLK = CIFHAMACGFJ.IBLHIAHECLK;
	}

	private void ControlPress(object data)
	{
		if (!IOPJDMCBIMM)
		{
			return;
		}
		CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
		Model fGCODGKLHED = ADOHNBMKNBG(cBBEIGACPPD.Index);
		FightCID eCHINOPKGGI = (FightCID)PCMNDFEAICH(cBBEIGACPPD);
		if (stageType == StageType.FDBBPEGEGMK.STAGE_START_STANCE)
		{
			if (fGCODGKLHED.MDNMFCIICAN == -1)
			{
				fGCODGKLHED.MDNMFCIICAN = (int)eCHINOPKGGI;
			}
		}
		else if (stageType == StageType.FDBBPEGEGMK.STAGE_FIGHT && eCHINOPKGGI != (FightCID)(-1))
		{
			fGCODGKLHED.PressAnyKey(eCHINOPKGGI);
		}
	}

	private void ControlRelease(object data)
	{
		if (IOPJDMCBIMM)
		{
			CBBEIGACPPD cBBEIGACPPD = (CBBEIGACPPD)data;
			Model fGCODGKLHED = ADOHNBMKNBG(cBBEIGACPPD.Index);
			FightCID eCHINOPKGGI = (FightCID)PCMNDFEAICH(cBBEIGACPPD);
			if (fGCODGKLHED.MDNMFCIICAN == (int)eCHINOPKGGI)
			{
				fGCODGKLHED.MDNMFCIICAN = -1;
			}
			if (eCHINOPKGGI != (FightCID)(-1))
			{
				fGCODGKLHED.ReleaseAnyKey(eCHINOPKGGI);
			}
			ReleaseAnyKey(cBBEIGACPPD.KMOPCKPBHIA);
		}
	}

	private int PCMNDFEAICH(CBBEIGACPPD DFIBLGKFAHN)
	{
		int count = LNDLFINJHDB.Count;
		if (count > 0 && DFIBLGKFAHN.Index < count)
		{
			Model fGCODGKLHED = ADOHNBMKNBG(DFIBLGKFAHN.Index);
			if (fGCODGKLHED.BCKKCJONNHG())
			{
				return (int)GBHGMIBDJGN(DFIBLGKFAHN.KMOPCKPBHIA);
			}
		}
		return -1;
	}

	private void KFGCODDPNJP()
	{
		OBICGGFDMLN();
		JKPOGNMHDNK(RuleAppliance.ApplianceAll, true);
		LCDPAAFCLPB();
		HEJMDNEJKLL();
		GLLAMEEPPHK();
		DGECGHDGPFO();
		NPFHCPAAIFJ();
	}

	private void IPILDDCKHMP()
	{
		FINFDFAMMDJ();
		KCJNBFLAMCC.GetActionButtons().ResetMagicButton();
	}

	private void MIEPNNMDNBO()
	{
		NMNCKBPFCCP.ALBOCOGOBCN(IEJFDGHCOON);
		AKBNKDBHCEO.ALBOCOGOBCN(HNLEDOEPHKG);
	}

	private void BNAGOFIAABA()
	{
		FELJFJOEJNC();
		KCJNBFLAMCC.GetActionButtons().ResetRaidChargeButton();
	}

	private void AlignCameraOnModels(List<Model> INNLAFHKJNI)
	{
		Vector3f eMAFACPEPDK = new Vector3f();
		float num = 0f;
		foreach (Model item in INNLAFHKJNI)
		{
			ModelObject oIEODIEHJMH = item.CLDMEJKGLBA();
			float num2 = oIEODIEHJMH.PAJLIKBIAPA();
			Vector3f eMAFACPEPDK2 = new Vector3f(oIEODIEHJMH.PLBNCDCFPML());
			eMAFACPEPDK2.Multiply(num2);
			eMAFACPEPDK.Add(eMAFACPEPDK2);
			num += num2;
		}
		eMAFACPEPDK.Multiply(1f / num);
		Vector3f eMAFACPEPDK3 = new Vector3f(_Camera.NPJHOCJIPDL());
		eMAFACPEPDK3.EHGLHOGAIDI(eMAFACPEPDK);
		eMAFACPEPDK3.IBNFLLGPOLD(0f);
		eMAFACPEPDK3.set_Z(0f);
		foreach (Model item2 in INNLAFHKJNI)
		{
			item2.ShiftModelPosition(eMAFACPEPDK3, true);
		}
	}

	private void EndFight()
	{
		Sound.IBHIPOOHNFK();
		if (MNEOALEBNNA)
		{
			MOBFFOHPCOE.OPLKJKPHHOH();
		}
		_Camera.CLOBNBAHAHF(false);
		_Camera.DFKKNMDAFDC(false);
		ResetModels(true);
		CKNCPOABFBO.LFNOLPFIBKC(GINNOLEJDFM);
		ComboStatistic aIOMDIAFHGB = null;
		ComboStatistic mOJHPBGGNAH = null;
		if (preFight != null)
		{
			aIOMDIAFHGB = preFight.GetStatistic(0);
			mOJHPBGGNAH = preFight.GetStatistic(1);
			preFight.gameObject.SetActive(false);
		}
		float pIFMOMMPFFM = (float)fightTimeInFrame / 60f;
		GameUtils.EndFight(aIOMDIAFHGB, KGKDKENMAOA, LBKDADMLJOE.ABKBEJBICOA, LBKDADMLJOE.LEBLJJCFKOP, LBKDADMLJOE.MHNEKAEGNBO, mOJHPBGGNAH, DKDMOJJJHHL, _playerModel.KADMPAHPOLD(), pIFMOMMPFFM, (int)_playerModel.DJLNJPMAHDL().HALCJLMJDII());
		MOBFFOHPCOE.HOCBEHCHOFL(false);
		GameUtils.AHJGPLGCNGI();
		if (KGKDKENMAOA.ANIFGJGHNLN)
		{
			ListSF.CCDKHLAMKKO().KJNPJKEHGLE().BFCLLIKOJGD();
		}
		if (KGKDKENMAOA.get_Type() != BattleType.FightRaid)
		{
		}
	}

	private void EndFightRaid()
	{
	}

	private Model ADOHNBMKNBG(int index)
	{
		switch (index)
		{
		case 0:
			return _playerModel;
		case 1:
			return CKNCPOABFBO;
		default:
			if (index < LNDLFINJHDB.Count)
			{
				return LNDLFINJHDB[index];
			}
			return null;
		}
	}

	private void LOGIFPHMNJM(Model ACENLMONNPA)
	{
		if (ACENLMONNPA == null)
		{
			LLLOJBFMONN.Error("Fight::fillMagicAndMissilesBuffer ERROR - model is NULL");
			return;
		}
		ENCEAHGFIPK.CPOOPPKHFHB = ACENLMONNPA.LPOJKGLFMAL();
		ENCEAHGFIPK.BNMFCPPJIAG = ACENLMONNPA.EKAFGLHNMCN();
	}

	private void MEDGLEDPHKD(Model ACENLMONNPA)
	{
		if (ACENLMONNPA == null)
		{
			LLLOJBFMONN.Error("Fight::setMagicAndMissilesFromBuffer ERROR - model is NULL");
			return;
		}
		ACENLMONNPA.FLBDBIHFJAI(ENCEAHGFIPK.CPOOPPKHFHB);
		ACENLMONNPA.OGHAMAGPFLF(ENCEAHGFIPK.BNMFCPPJIAG);
	}

	private void ELLBMOPJHJI(FightList KGKDKENMAOA)
	{
	}

	private void IGIANHEMGKA(FightList KGKDKENMAOA)
	{
	}

	private void MDJEDDJCGGE(bool value)
	{
		if (preFight != null && preFight.get_ViewerFight() != null)
		{
			preFight.get_ViewerFight().SetLockLifeUpdate(true, value);
			preFight.get_ViewerFight().SetLockLifeUpdate(false, value);
		}
	}

	private void InitRules()
	{
		_rulesInspector.ResetRules((round.round <= 0) ? 1 : round.round);
		if (KGKDKENMAOA != null)
		{
			KGKDKENMAOA.GJFPAFPEPLK();
		}
	}

	private void ApplyRules()
	{
		_rulesInspector.CheckPreDraws();
		RuleInitData oIFPCFEGFOB = new RuleInitData(_playerModel, CKNCPOABFBO, _location, DPONLGICLEH);
		oIFPCFEGFOB.NMNCKBPFCCP = NMNCKBPFCCP;
		oIFPCFEGFOB.AKBNKDBHCEO = AKBNKDBHCEO;
		_rulesInspector.InitRules(oIFPCFEGFOB);
	}

	private void PFNHDCIOJKJ()
	{
		DPONLGICLEH.MPLPEMOFHGI.Reset();
		DPONLGICLEH.EKBMBILHBMC.Reset();
	}

	private void CNEPNGMIHJH()
	{
	}

	private void EOFLFADGIJK()
	{
	}

	private void StopAllRules()
	{
		_rulesInspector.RulesActive = false;
		_rulesInspector.StopRules();
		KFGCODDPNJP();
	}

	private void CheckChangeFightRules()
	{
		ELLBMOPJHJI(KGKDKENMAOA);
		_rulesInspector.CheckChangeFightRules(KGKDKENMAOA);
	}

	private void CENFCGAKDOL()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			if (item.MDNMFCIICAN != -1)
			{
				item.PressAnyKey((FightCID)item.MDNMFCIICAN);
				item.MDNMFCIICAN = -1;
			}
		}
	}

	private void HBGMKCNFKHM()
	{
	}

	private void HFGBKBKNCOB()
	{
		List<string> list = new List<string>(4);
		List<string> list2 = new List<string>();
		ItemInfo jGMLKIPCFII = _playerModel.KMMJCHDKBDO.JGMLKIPCFII;
		if (jGMLKIPCFII != null)
		{
			string mDPPNGIEJGD = jGMLKIPCFII.MDPPNGIEJGD;
			list.AddIfNotExist(mDPPNGIEJGD);
			list2.AddIfNotExist(mDPPNGIEJGD);
		}
		ItemInfo jGMLKIPCFII2 = CKNCPOABFBO.KMMJCHDKBDO.JGMLKIPCFII;
		if (jGMLKIPCFII2 != null)
		{
			string mDPPNGIEJGD2 = jGMLKIPCFII2.MDPPNGIEJGD;
			list.AddIfNotExist(mDPPNGIEJGD2);
			if (list2.Contains(mDPPNGIEJGD2))
			{
				list2.Remove(mDPPNGIEJGD2);
			}
			else
			{
				list2.AddIfNotExist(mDPPNGIEJGD2);
			}
		}
		LLLOJBFMONN.Write("Loading tactics for next subtypes:");
		foreach (string item in list)
		{
			LLLOJBFMONN.Write(item);
		}
		AiData.ClearTables();
		AiData.Load(list, list2);
	}

	private void GOCNEMPBJIH(float GPEGBHMKKJL)
	{
		if (MKCLBJEIIHN)
		{
			_Camera.KKFIJLOMOJI().GOCNEMPBJIH(GPEGBHMKKJL);
		}
	}

	private void FINFDFAMMDJ()
	{
		if (AssemblyController.PGFJMOGKEID() || AssemblyController.KMEOEAGGPBI())
		{
			bool hFIIEPMEMFF = NMNCKBPFCCP.ADBKGIBBNHJ != null && NMNCKBPFCCP.ADBKGIBBNHJ.Name != GameUtils.GetDefaultItem("Magic");
			KCJNBFLAMCC.GetActionButtons().ShowMagic(hFIIEPMEMFF);
		}
	}

	private void KILMEMFHJHH()
	{
		if (AssemblyController.PGFJMOGKEID() || AssemblyController.JONCCPLEIBE().NPNOMBEEPJD())
		{
			bool gKGKKCLPGBB = NMNCKBPFCCP.LGHMILECPLA != null && NMNCKBPFCCP.LGHMILECPLA.Name != GameUtils.GetDefaultItem("Ranged");
			KCJNBFLAMCC.GetActionButtons().ShowRanged(gKGKKCLPGBB);
		}
	}

	private void FELJFJOEJNC()
	{
		if (AssemblyController.PGFJMOGKEID() || AssemblyController.KMEOEAGGPBI())
		{
			KAOPLEPILDH kAOPLEPILDH = NMNCKBPFCCP as KAOPLEPILDH;
			bool oPPBHOOBHOE = KGKDKENMAOA.get_Type() == BattleType.FightRaid && kAOPLEPILDH != null && kAOPLEPILDH.LMIBBJIKLNO != null && kAOPLEPILDH.LMIBBJIKLNO.Name != GameUtils.GetDefaultItem("RaidCharge") && _playerModel.CKAKLHDLHJO() > 0;
			KCJNBFLAMCC.GetActionButtons().ShowRaidCharge(oPPBHOOBHOE);
		}
	}

	private void ResetModelsHitData()
	{
		foreach (Model item in LNDLFINJHDB)
		{
			item.ResetHitData();
		}
	}

	private string BHLIBKKJNKH(IntervalAttack FLGCMOKINLI)
	{
		return string.Empty;
	}

	private void GAKACHNBENN(object AOMLCBHAJJH)
	{
		OpenPauseScreen();
	}

	private void KAMOJAKJILE(object AOMLCBHAJJH)
	{
		OpenPauseScreen();
	}

	private void NLKDFBEAEEH(bool MFDIOECHDOA)
	{
		uint num = ListSF.CCDKHLAMKKO().EOKLELGLHJJ();
		uint num2 = ListSF.CCDKHLAMKKO().HEOHJNFGEDH();
		uint num3 = GameUtils.IBNHPCFKGOH(KGKDKENMAOA, MFDIOECHDOA);
		int num4 = ListSF.CCDKHLAMKKO().PINDEKDNCNL();
		int count = GameUtils.HHONBOCJBLB.PEDIMBMABIG.Count;
		global::Pair<int, uint> cCKLNOPEKHO = GameUtils.HHONBOCJBLB.PEDIMBMABIG[count - 1];
		int lLHEDBIEHAA = cCKLNOPEKHO.First;
		if (num + num3 >= num2 && num4 + 1 == lLHEDBIEHAA)
		{
			MOBFFOHPCOE.PMEOOPEEAEM();
		}
	}
}
