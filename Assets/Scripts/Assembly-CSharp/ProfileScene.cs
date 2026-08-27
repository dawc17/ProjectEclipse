using System.Collections.Generic;
using Nekki.SF2.Core.Fights;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Menu;
using Nekki.SF2.GUI.Profile;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScene : Scene<ProfileScene>
{
	private const int LLKBFIONHPF = 30;

	private const string NOBEALMIHAN = "image/skills/";

	private const int DDLPJCNEMCD = 1000;

	private const int OLMIKNCJKMJ = 30;

	public MainMenu mainMenu;

	[SerializeField]
	private SpriteRenderer _backgroundLeft;

	[SerializeField]
	private SpriteRenderer _backgroundRight;

	[SerializeField]
	private UserPerksSprite _leftPanel;

	[SerializeField]
	private RightInfoSprite _rightPanel;

	[SerializeField]
	public ModelContainer ModelContainer;

	[SerializeField]
	private GameObject _profileTableViewCellPrefab;

	[SerializeField]
	private TableView _perksTable;

	[SerializeField]
	private GameObject _perkCellPrefab;

	[SerializeField]
	private PerksController _perksCtrl;

	[SerializeField]
	private TableView _tricksTable;

	[SerializeField]
	private GameObject _trickCellPrefab;

	[SerializeField]
	private TricksController _tricksCtrl;

	[SerializeField]
	private TableView _achievementsTable;

	[SerializeField]
	private GameObject _achievementCellPrefab;

	private AchievementsController DLJFCPJPCHI;

	[SerializeField]
	private TableView _sealsTable;

	[SerializeField]
	private GameObject _sealCellPrefab;

	private SealsController JHEHMDFDJEC;

	private TableView DOOGDEDCHBO;

	private bool KMIJDCPHHML;

	private InfoAnimation MKGONDJABAH;

	private bool EIDKMLIOKOD;

	private bool OHDPMGDBCCF = true;

	private bool IBADMKPHOOJ;

	private float BKACEHDPGKC = 1f;

	private int KKEHAMGBIEA;

	[SerializeField]
	private SectionButton _btnPerks;

	[SerializeField]
	private SectionButton _btnTricks;

	[SerializeField]
	private SectionButton _btnAchievements;

	[SerializeField]
	private SectionButton _btnSeals;

	private List<SectionButton> JHFCFBIPGPF = new List<SectionButton>();

	private SectionButton HNCLEDJDODK;

	[SerializeField]
	private SFButton _showAchievementButton;

	private SliderType MFCFAGFGEKJ;

	[SerializeField]
	private ResolutionImage _achievementCircle;

	[SerializeField]
	private ResolutionImage _achievementEllipse;

	[SerializeField]
	private Text _achievementLabel;

	[SerializeField]
	private ResolutionImage _trickCircle;

	[SerializeField]
	private ResolutionImage _trickEllipse;

	[SerializeField]
	private Text _trickLabel;

	[SerializeField]
	private ResolutionImage _perkCircle;

	[SerializeField]
	private ResolutionImage _perkEllipse;

	[SerializeField]
	private Text _perkLabel;

	[SerializeField]
	private ResolutionImage _sealCircle;

	[SerializeField]
	private ResolutionImage _sealEllipse;

	[SerializeField]
	private Text _sealLabel;

	private float AAAAENBBFMI;

	private float GOGMJFKMPAM;

	private bool JGEAEFCNCIL;

	private bool BPNNOHDIMFG;

	public List<SubItem> SubItems = new List<SubItem>();

	private SubItem IFOGILFLCJO;

	[SerializeField]
	private CanvasGroup _profileUIGroup;

	[SerializeField]
	private CanvasGroup _bottomUIGroup;

	public override ScreenType PNAJHDBDDLP
	{
		get
		{
			return get_SceneId();
		}
	}

	public override ScreenType get_SceneId()
	{
		return ScreenType.ModuleProfile;
	}

	protected override void Init(object data)
	{
		KMIJDCPHHML = GameCenterController.OBDJPKOJADA();
		mainMenu.Init();
		KIHHCIHBNLB();
		IJAFAMPCNIJ();
		BLKEFMCNCFO();
		PFJBDAMNBIO();
		CADAAFJHCCC();
		KCBJGFPAOHC();
		JEOAHMHNAEM();
		if (!BKDDIHINJNM())
		{
			IGNOGLBBHDG(_perksTable);
		}
		IGNOGLBBHDG(_achievementsTable);
		IGNOGLBBHDG(_tricksTable);
		IGNOGLBBHDG(_sealsTable);
		SetScreen(SliderType.SliderPerks);
		HPPBLKIPPME();
	}

	private void KIHHCIHBNLB()
	{
		ModelContainer.Init();
		ModelContainer.UpdateModel(null, StageType.FDBBPEGEGMK.STAGE_SHOP_START, "Profile");
		ModelContainer.AddEventListener(0, LDFKBJAHGII);
	}

	public void OnTrickShow(object data)
	{
		if (data != null)
		{
			TrickSubItem trickSubItem = (TrickSubItem)data;
			Trick trick = trickSubItem.GetTrick();
			MKGONDJABAH = trick.KJHMOGGECBN;
			_backgroundLeft.color = Constants.EKJMAIDGKME;
			_backgroundRight.color = Constants.EKJMAIDGKME;
			CGFOHBFAJBL();
		}
	}

	private void CGFOHBFAJBL()
	{
		_leftPanel.gameObject.SetActive(false);
		EIDKMLIOKOD = true;
		OHDPMGDBCCF = false;
		_profileUIGroup.blocksRaycasts = false;
		_bottomUIGroup.blocksRaycasts = false;
		for (int i = 0; i < JHFCFBIPGPF.Count; i++)
		{
		}
		SubItem.EnableAnimation(false);
		GameUtils.FMICOICLCNL(false);
	}

	private void LDFKBJAHGII(object data)
	{
		if (IBADMKPHOOJ)
		{
			ModelContainer.ResetModel();
			IBADMKPHOOJ = false;
			_backgroundLeft.color = Constants.GFBLKELEBEH;
			_backgroundRight.color = Constants.GFBLKELEBEH;
			OOHJAHDHEAP();
		}
	}

	private void OOHJAHDHEAP()
	{
		EIDKMLIOKOD = true;
		OHDPMGDBCCF = true;
		_profileUIGroup.blocksRaycasts = true;
		_bottomUIGroup.blocksRaycasts = true;
		for (int i = 0; i < JHFCFBIPGPF.Count; i++)
		{
		}
	}

	private void Update()
	{
		IBGCDOFNIIF();
		HPHAOJDPNND();
		bool flag = GameCenterController.OBDJPKOJADA();
		if (KMIJDCPHHML != flag)
		{
			INKIBPEDNJL();
			KMIJDCPHHML = flag;
		}
	}

	private void HPHAOJDPNND()
	{
		if (EIDKMLIOKOD)
		{
			if (OHDPMGDBCCF)
			{
				LMPGJLLBFPP();
			}
			else
			{
				MONAFDKJKOP();
			}
		}
	}

	private void LMPGJLLBFPP()
	{
		if (EIDKMLIOKOD)
		{
			float num = 1f / 30f;
			BKACEHDPGKC += num;
			if (BKACEHDPGKC >= 1f)
			{
				BKACEHDPGKC = 1f;
				EIDKMLIOKOD = false;
				_leftPanel.gameObject.SetActive(true);
				SubItem.EnableAnimation(true);
				GameUtils.KKNGFGMJKHG();
			}
			_profileUIGroup.alpha = BKACEHDPGKC;
		}
	}

	private void MONAFDKJKOP()
	{
		if (EIDKMLIOKOD)
		{
			float num = 1f / 30f;
			BKACEHDPGKC -= num;
			if (BKACEHDPGKC <= 0f)
			{
				BKACEHDPGKC = 0f;
				EIDKMLIOKOD = false;
				PlayAnimation();
			}
			_profileUIGroup.alpha = BKACEHDPGKC;
		}
	}

	private void PlayAnimation()
	{
		if (MKGONDJABAH != null)
		{
			ModelContainer.PlayAnimation(MKGONDJABAH.Name);
			IBADMKPHOOJ = true;
			MKGONDJABAH = null;
		}
	}

	private void INKIBPEDNJL()
	{
		if (SystemProperties.IPJFCBAGMJJ() && !AssemblyController.JONCCPLEIBE().BKGIFIPIHAL() && !AssemblyController.JONCCPLEIBE().NPNOMBEEPJD() && !AssemblyController.JONCCPLEIBE().OPCBKOOFMAK() && GameCenterController.OBDJPKOJADA() && DOOGDEDCHBO == _achievementsTable)
		{
			_showAchievementButton.gameObject.SetActive(true);
		}
		else
		{
			_showAchievementButton.gameObject.SetActive(false);
		}
	}

	private void BLKEFMCNCFO()
	{
		_perksCtrl = new PerksController(_perksTable, _perkCellPrefab);
		_perksTable.onSelectCell.AddListener(ONPNNPPAGGK);
		_perksTable.gameObject.SetActive(false);
		DLJFCPJPCHI = new AchievementsController(_achievementsTable, _achievementCellPrefab);
		_achievementsTable.onSelectCell.AddListener(ONPNNPPAGGK);
		_achievementsTable.onSelectCell.AddListener(PDDILIPNJEN);
		_tricksCtrl = new TricksController(_tricksTable, _trickCellPrefab);
		_tricksTable.onSelectCell.AddListener(ONPNNPPAGGK);
		_tricksTable.onSelectCell.AddListener(PDDILIPNJEN);
		OKJCEACNIDC();
		ADAMNHCPBPG();
		CJKBAILOPLN();
	}

	public void SetScreen(SliderType LFLGCDNKNJI)
	{
		QuestParameters hHKLFIIBIFF = ListSF.ELEBLBJKDBI().BNMLDPNCMLB();
		if (MFCFAGFGEKJ != LFLGCDNKNJI && GameUtils.MKADBAEEMFA(GameUtils.NAMBCLFLNIN(hHKLFIIBIFF.OIKHBNOANPP), LFLGCDNKNJI))
		{
			return;
		}
		if (LFLGCDNKNJI == SliderType.SliderPerks)
		{
			JFBPMBFPKHC();
		}
		else
		{
			LGPACAHLAGO();
		}
		if (DOOGDEDCHBO != null)
		{
			DOOGDEDCHBO.gameObject.SetActive(false);
		}
		if (HNCLEDJDODK != null && DOOGDEDCHBO != null)
		{
			HNCLEDJDODK.interactable = true;
		}
		HPPBLKIPPME();
		_rightPanel.Clear();
		switch (LFLGCDNKNJI)
		{
		case SliderType.SliderPerks:
		{
			JFBPMBFPKHC();
			DOOGDEDCHBO = _perksTable;
			HNCLEDJDODK = _btnPerks;
			string noContentMessage = ((!BKDDIHINJNM()) ? "profileNoSkills" : "profileSelectSkill");
			_rightPanel.SetNoContentMessage(noContentMessage);
			break;
		}
		case SliderType.SliderTricks:
			LGPACAHLAGO();
			DOOGDEDCHBO = _tricksTable;
			HNCLEDJDODK = _btnTricks;
			DOOGDEDCHBO.ScrollToCell(0);
			PDDILIPNJEN(0);
			KAHCAGHOMLJ(_tricksTable);
			BMLFAEOGIAN();
			OEDOBIIJGBC();
			break;
		case SliderType.SliderAchievements:
			LGPACAHLAGO();
			DOOGDEDCHBO = _achievementsTable;
			HNCLEDJDODK = _btnAchievements;
			if (DLJFCPJPCHI.IGGFLBOBGLN())
			{
				DLJFCPJPCHI.ICDEBPNMLFB();
			}
			else
			{
				DOOGDEDCHBO.ScrollToCell(0);
			}
			PDDILIPNJEN(0);
			break;
		case SliderType.SliderSeals:
			LGPACAHLAGO();
			BHKDPICIGDI();
			HPPBLKIPPME();
			DOOGDEDCHBO = _sealsTable;
			HNCLEDJDODK = _btnSeals;
			DOOGDEDCHBO.ScrollToCell(0);
			PDDILIPNJEN(0);
			break;
		default:
			LLLOJBFMONN.Write("ERROR: ProfileScreen - onTypeSelect (data = %i)", LFLGCDNKNJI);
			break;
		}
		if ((bool)DOOGDEDCHBO)
		{
			DOOGDEDCHBO.gameObject.SetActive(true);
		}
		if (HNCLEDJDODK != null)
		{
			HNCLEDJDODK.interactable = false;
		}
		INKIBPEDNJL();
		MFCFAGFGEKJ = LFLGCDNKNJI;
		hHKLFIIBIFF.OIKHBNOANPP = GameUtils.IPFBLJOALBN[MFCFAGFGEKJ];
	}

	public void ScrollToItemByName(SliderType _sliderType, string OHCGEEEKEJH)
	{
		SetScreen(_sliderType);
		if (OHCGEEEKEJH != string.Empty)
		{
			switch (_sliderType)
			{
			case SliderType.SliderPerks:
				_perksCtrl.EENODCGBNHC(OHCGEEEKEJH);
				break;
			case SliderType.SliderTricks:
				_tricksCtrl.FEKDKAPJDCJ(OHCGEEEKEJH);
				break;
			case SliderType.SliderAchievements:
				DLJFCPJPCHI.BDHALBHODPG(OHCGEEEKEJH);
				break;
			case SliderType.SliderSeals:
				JHEHMDFDJEC.KCAAFPNBEGL(OHCGEEEKEJH);
				break;
			}
		}
	}

	public void AddSubItem(SubItem item)
	{
		item.AddEventListener(10, OnSubItemClick);
		item.ButtonId = KKEHAMGBIEA++;
		SubItems.Add(item);
	}

	public LabelButton GetBtnPerkImprove()
	{
		return _rightPanel.GetBtnPerkImprove();
	}

	public LabelButton GetBtnStrikeShow()
	{
		return _rightPanel.GetBtnStrikeShow();
	}

	public TableView GetTableByType(SliderType _sliderType)
	{
		switch (_sliderType)
		{
		case SliderType.SliderPerks:
			return _perksTable;
		case SliderType.SliderTricks:
			return _tricksTable;
		case SliderType.SliderAchievements:
			return _achievementsTable;
		case SliderType.SliderSeals:
			return _sealsTable;
		default:
			return null;
		}
	}

	private void PFJBDAMNBIO()
	{
		OHDFPIADEIG(_btnPerks, SliderType.SliderPerks);
		OHDFPIADEIG(_btnTricks, SliderType.SliderTricks);
		OHDFPIADEIG(_btnAchievements, SliderType.SliderAchievements);
		OHDFPIADEIG(_btnSeals, SliderType.SliderSeals);
		DEHENFMAKKM();
	}

	private void CADAAFJHCCC()
	{
		CBDKOFGFMLB(_achievementCircle, _achievementEllipse, _achievementLabel, _btnAchievements);
		CBDKOFGFMLB(_trickCircle, _trickEllipse, _trickLabel, _btnTricks);
		CBDKOFGFMLB(_perkCircle, _perkEllipse, _perkLabel, _btnPerks);
		CBDKOFGFMLB(_sealCircle, _sealEllipse, _sealLabel, _btnSeals);
	}

	private void CBDKOFGFMLB(ResolutionImage MJOHJFEFNGF, ResolutionImage DGLDPAOMOPH, Text ICBBNJMLDJH, SectionButton GAMILDJHFDB)
	{
		MJOHJFEFNGF.gameObject.SetActive(false);
		DGLDPAOMOPH.gameObject.SetActive(false);
		ICBBNJMLDJH.gameObject.SetActive(false);
		ICBBNJMLDJH.color = Constants.PLIDKLDIOKM;
	}

	private void OKJCEACNIDC()
	{
		_achievementsTable.gameObject.SetActive(false);
	}

	private void ADAMNHCPBPG()
	{
		_tricksTable.gameObject.SetActive(false);
	}

	private void CJKBAILOPLN()
	{
		JHEHMDFDJEC = new SealsController(_sealsTable, _sealCellPrefab);
		_sealsTable.gameObject.SetActive(false);
		_sealsTable.onSelectCell.AddListener(PDDILIPNJEN);
		_sealsTable.ScrollToCell(_sealsTable.NumberOfRows() - 1);
	}

	private void KCBJGFPAOHC()
	{
		_showAchievementButton.AddEventListener(2, OnClickButton);
	}

	private void IBGCDOFNIIF()
	{
		if (JGEAEFCNCIL && _leftPanel != null)
		{
			if ((BPNNOHDIMFG && _leftPanel.transform.localPosition.x <= GOGMJFKMPAM + 0.01f) || (!BPNNOHDIMFG && _leftPanel.transform.localPosition.x >= AAAAENBBFMI + 0.01f))
			{
				JGEAEFCNCIL = false;
			}
			else if (BPNNOHDIMFG)
			{
				_leftPanel.transform.OKHPLHPBPKJ(_leftPanel.transform.localPosition.x - 30f);
			}
			else
			{
				_leftPanel.transform.OKHPLHPBPKJ(_leftPanel.transform.localPosition.x + 30f);
			}
		}
	}

	private void JFBPMBFPKHC()
	{
		JGEAEFCNCIL = true;
		BPNNOHDIMFG = true;
	}

	private void LGPACAHLAGO()
	{
		JGEAEFCNCIL = true;
		BPNNOHDIMFG = false;
	}

	private void OHDFPIADEIG(SectionButton GAMILDJHFDB, SliderType OKNNNLIPODI)
	{
		GAMILDJHFDB.ButtonId = (int)OKNNNLIPODI;
		GAMILDJHFDB.AddEventListener(2, NKJAOBEMLBM);
		JHFCFBIPGPF.Add(GAMILDJHFDB);
	}

	private void OnClickButton(object data)
	{
		if (GameCenterController.OBDJPKOJADA())
		{
			GameCenterController.NPMGIFJKAEG();
		}
	}

	private void NKJAOBEMLBM(object data)
	{
		if (data != null)
		{
			SliderType screen = (SliderType)data;
			SetScreen(screen);
		}
	}

	private void ONPNNPPAGGK(object data)
	{
		if (!(DOOGDEDCHBO == null))
		{
			LFAJDIBPGOH();
			if (DOOGDEDCHBO == _perksTable)
			{
				JCMCECGPNGC();
			}
		}
	}

	private void PDDILIPNJEN(object data)
	{
		if (DOOGDEDCHBO == null || (DOOGDEDCHBO != _tricksTable && DOOGDEDCHBO != _achievementsTable && DOOGDEDCHBO != _sealsTable))
		{
			return;
		}
		if (DOOGDEDCHBO != _sealsTable)
		{
			ProfileCell profileCell = (ProfileCell)DOOGDEDCHBO.get_SelectedCell();
			if (profileCell != null)
			{
				SubItem firstIcon = profileCell.GetFirstIcon();
				firstIcon.Choose();
			}
		}
		else
		{
			ShopTableViewCell shopTableViewCell = (ShopTableViewCell)DOOGDEDCHBO.get_SelectedCell();
			if (shopTableViewCell != null)
			{
				ItemInfo itemInfo = shopTableViewCell.get_ItemInfo();
				_rightPanel.SetItemInfo(itemInfo);
			}
		}
	}

	public void OnSubItemClick(object data)
	{
		int oKNNNLIPODI = (int)data;
		if (IFOGILFLCJO != null)
		{
			IFOGILFLCJO.SetSelected(false);
		}
		IFOGILFLCJO = OHNODOAGBJG(oKNNNLIPODI);
		if (IFOGILFLCJO == null)
		{
			return;
		}
		IFOGILFLCJO.SetSelected(true);
		if (IFOGILFLCJO.Data != null)
		{
			if (DOOGDEDCHBO == _perksTable)
			{
				KAHIFHMHDAF kAHIFHMHDAF = (KAHIFHMHDAF)IFOGILFLCJO.Data;
				kAHIFHMHDAF.JMLAKAKDBBL = _rightPanel.GetLabelWidth();
				_rightPanel.SetPerkInfo(kAHIFHMHDAF);
			}
			else if (DOOGDEDCHBO == _tricksTable)
			{
				TrickInfo trickInfo = (TrickInfo)IFOGILFLCJO.Data;
				_rightPanel.SetTrickInfo(trickInfo);
			}
			else if (DOOGDEDCHBO == _achievementsTable)
			{
				AchievementInfo achievementInfo = (AchievementInfo)IFOGILFLCJO.Data;
				_rightPanel.SetAchievementInfo(achievementInfo);
			}
		}
	}

	public void OnPerkImprove(object data)
	{
		if (data == null)
		{
			return;
		}
		PerkSubItem perkSubItem = (PerkSubItem)data;
		ProfilePerk perk = perkSubItem.get_Perk();
		if (perk == null)
		{
			return;
		}
		RosterPerk hOGDBKBFFDJ = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().HGOLHMJEPIA(perk);
		PerkHistory.Perk hNHILOOIIMO = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().GIAEMMLABDL.CBGCAPIMCFH(perk.KAMBOKLFBEE(), perk.PINDEKDNCNL());
		PerkTree.GBPBIPFIOJH().AEOKBBBAANA(hNHILOOIIMO);
		_leftPanel.AddItem(hOGDBKBFFDJ.DFOELJAEEGG());
		perkSubItem.Choose();
		if (hNHILOOIIMO != null)
		{
			PerkCell perkCell = GCFBNDPAMLA((PerkCell)perkSubItem.ParentCell);
			ProfilePerkContainer fHPJJGPJLHD = PerkTree.GBPBIPFIOJH().HPKLHAAFPHK(hNHILOOIIMO.Level);
			if (perkCell != null && fHPJJGPJLHD != null)
			{
				_perksCtrl.LAJJAAAGDLI(perkCell.get_RowNumber());
			}
			_tricksCtrl.LLIMHAHIMML();
			mainMenu.UpdateNewPerks();
			if (perkSubItem.IsInfoAnimation())
			{
			}
			Trick iHNIKIHKFHC = GameUtils.NMNIGIDFKOA(perk.CEENDGFFEFM());
			if (iHNIKIHKFHC != null)
			{
				ListSF.CCDKHLAMKKO().ENFKEIHBICK(iHNIKIHKFHC.Name);
			}
			HPPBLKIPPME();
			ProfilePerk pLKCIINIFMJ = HEKMFIFGALO(hNHILOOIIMO.Level, hOGDBKBFFDJ);
			ArgsDict kEMMIFBFDPK = new ArgsDict();
			if (hOGDBKBFFDJ != null)
			{
				kEMMIFBFDPK["learnedPerk"] = hOGDBKBFFDJ.DFOELJAEEGG();
			}
			if (pLKCIINIFMJ != null)
			{
				kEMMIFBFDPK["rejectedPerk"] = pLKCIINIFMJ.DFOELJAEEGG();
			}
			StatisticsCollector.BPDGOKGHDHB(StatisticsEvent.JDNFFHILFAF.Perk, kEMMIFBFDPK);
		}
	}

	public void OnAchievementRewardTake(object data)
	{
		if (data == null)
		{
			return;
		}
		AchievementSubItem achievementSubItem = (AchievementSubItem)data;
		Achievement achievement = achievementSubItem.GetAchievement();
		if (!achievement.NMCBAKACIGK)
		{
			achievement.NMCBAKACIGK = true;
			achievement.BEBDMOEIEJN(false);
			ListSF.CCDKHLAMKKO().KJNPJKEHGLE().POKNGJJAHAL(achievement);
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			if (achievement.ANCDKCFLHOL > 0)
			{
				nKGLHEGIKKP.OIOOMAKNIOB(nKGLHEGIKKP.BFBOEGMAMNF() + achievement.ANCDKCFLHOL);
			}
			if (achievement.LBJFKGAHBBG > 0)
			{
				nKGLHEGIKKP.LLNELLFMMBB(nKGLHEGIKKP.EHFJHFDACMP() + achievement.LBJFKGAHBBG, Roster.HPOIJPGPOCF.CHANGE_ACHIEVEMENT);
			}
			mainMenu.UpdateMenu();
			Sound.IFKCCDAIADF("snd_buy");
		}
		achievementSubItem.ResetOpacity();
		achievementSubItem.Choose();
		HPPBLKIPPME();
		DLJFCPJPCHI.ICDEBPNMLFB(ProfileGUI.SpeedScrollAchievements);
	}

	private void JEOAHMHNAEM()
	{
		int num = -1;
		int num2 = -1;
		List<ProfilePerk> list = PerkTree.GBPBIPFIOJH().JGCHDCOOGII();
		for (int i = 0; i < list.Count; i++)
		{
			ProfilePerk pLKCIINIFMJ = list[i];
			ProfilePerk.KMHBPKKCNPP kMHBPKKCNPP = list[i].FLBBFDNHJAJ();
			if (kMHBPKKCNPP == ProfilePerk.KMHBPKKCNPP.PERK_LOCK || ListSF.CCDKHLAMKKO().PINDEKDNCNL() < list[i].PINDEKDNCNL())
			{
				break;
			}
			if (list[i].PINDEKDNCNL() > num2)
			{
				num2 = list[i].PINDEKDNCNL();
				num++;
			}
		}
		if (num >= 0 && num < _perksTable.NumberOfRows())
		{
			_perksTable.ScrollToCell(num);
		}
		else
		{
			_perksTable.ScrollToCell(0);
		}
	}

	private void BMLFAEOGIAN()
	{
		if (!(DOOGDEDCHBO != _tricksTable))
		{
			_tricksCtrl.ADDALEKEMCD();
		}
	}

	private void JCMCECGPNGC()
	{
	}

	private bool BKDDIHINJNM()
	{
		List<ProfilePerk> list = PerkTree.GBPBIPFIOJH().JGCHDCOOGII();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].FLBBFDNHJAJ() == ProfilePerk.KMHBPKKCNPP.PERK_AVAILABLE && ListSF.CCDKHLAMKKO().PINDEKDNCNL() >= list[i].PINDEKDNCNL())
			{
				return true;
			}
		}
		return false;
	}

	private void LFAJDIBPGOH()
	{
		if (!(DOOGDEDCHBO == null))
		{
			string text = HPPLHBAOOHN(DOOGDEDCHBO);
			if (text != string.Empty)
			{
				GameUtils.LMGAGBOKCFC.KJIGJEBMILC(text, DOOGDEDCHBO.GetCurrentCellRow());
			}
		}
	}

	private void IGNOGLBBHDG(TableView BFGHBIMJHAK)
	{
		if (BFGHBIMJHAK == null)
		{
			return;
		}
		string text = HPPLHBAOOHN(BFGHBIMJHAK);
		if (text != string.Empty)
		{
			int num = GameUtils.LMGAGBOKCFC.MILAHFHNIIP(text);
			if (num >= 0 && num < BFGHBIMJHAK.NumberOfRows())
			{
				BFGHBIMJHAK.ScrollToCell(num);
			}
		}
	}

	private string HPPLHBAOOHN(TableView BFGHBIMJHAK = null)
	{
		string result = string.Empty;
		if (BFGHBIMJHAK == _tricksTable)
		{
			result = "SKILLS_SLIDER";
		}
		else if (BFGHBIMJHAK == _achievementsTable)
		{
			result = "ACHIEVEMENT_SLIDER";
		}
		else if (BFGHBIMJHAK == _perksTable)
		{
			result = "POWERLEVELING_SLIDER";
		}
		else if (BFGHBIMJHAK == _sealsTable)
		{
			result = "SEALS_SLIDER";
		}
		return result;
	}

	private SubItem OHNODOAGBJG(int OKNNNLIPODI)
	{
		for (int i = 0; i < SubItems.Count; i++)
		{
			if (SubItems[i].ButtonId == OKNNNLIPODI)
			{
				return SubItems[i];
			}
		}
		return null;
	}

	private PerkCell GCFBNDPAMLA(PerkCell HJCPCBLCJJN)
	{
		if (HJCPCBLCJJN == null)
		{
			return null;
		}
		int num = HJCPCBLCJJN.get_RowNumber() + 1;
		if (num < _perksTable.NumberOfRows())
		{
			return (PerkCell)_perksTable.get_visibleCells().GetCellAtIndex(num);
		}
		return null;
	}

	private void DEHENFMAKKM()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			return;
		}
		List<UserItem> list = nKGLHEGIKKP.KHCNHPCPFII().HOPBBLJLHOB("Seal", string.Empty);
		int num = 0;
		foreach (UserItem item in list)
		{
			if (item.OFOPFCJNEBL() > 0)
			{
				num++;
			}
		}
		if (num == 0)
		{
			_btnSeals.enabled = false;
		}
	}

	private void KAHCAGHOMLJ(TableView BFGHBIMJHAK)
	{
		Dictionary<int, TableViewCell> dictionary = BFGHBIMJHAK.get_visibleCells().BFNFADJMAPC();
		foreach (KeyValuePair<int, TableViewCell> item in dictionary)
		{
			ProfileCell profileCell = (ProfileCell)item.Value;
			profileCell.UpdateState();
		}
	}

	private void DJBBJPLEBMN(ResolutionImage MJOHJFEFNGF, ResolutionImage DGLDPAOMOPH, Text ICBBNJMLDJH, int count)
	{
		MJOHJFEFNGF.gameObject.SetActive(false);
		DGLDPAOMOPH.gameObject.SetActive(false);
		ICBBNJMLDJH.gameObject.SetActive(false);
		if (count > 0)
		{
			if (count < 10)
			{
				MJOHJFEFNGF.gameObject.SetActive(true);
			}
			else
			{
				DGLDPAOMOPH.gameObject.SetActive(true);
			}
			ICBBNJMLDJH.text = count.ToString();
			ICBBNJMLDJH.gameObject.SetActive(true);
		}
	}

	private void HPPBLKIPPME()
	{
		DJBBJPLEBMN(_achievementCircle, _achievementEllipse, _achievementLabel, ListSF.CCDKHLAMKKO().KJNPJKEHGLE().JKGGEMEBPCP());
		DJBBJPLEBMN(_trickCircle, _trickEllipse, _trickLabel, ListSF.CCDKHLAMKKO().NPKBPGMNDFJ());
		DJBBJPLEBMN(_perkCircle, _perkEllipse, _perkLabel, ListSF.CCDKHLAMKKO().JLBDOBLHHAF().OPPFMFKAOIG());
		DJBBJPLEBMN(_sealCircle, _sealEllipse, _sealLabel, ListSF.CCDKHLAMKKO().CNFOLIEFJCE());
	}

	private void OEDOBIIJGBC()
	{
		List<Trick> list = GameUtils.KLLGJKHALGH();
		foreach (Trick item in list)
		{
			if (item.IsNew)
			{
				item.IsNew = false;
				ListSF.CCDKHLAMKKO().DECNJIOFODA(item.Name);
			}
		}
	}

	private void BHKDPICIGDI()
	{
		Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
		if (nKGLHEGIKKP == null)
		{
			return;
		}
		List<UserItem> list = nKGLHEGIKKP.KHCNHPCPFII().HOPBBLJLHOB("Seal", string.Empty);
		foreach (UserItem item in list)
		{
			item.BHKHOJPANHE().BEBDMOEIEJN(false);
		}
	}

	private void PMLKFAFGJDC(object data)
	{
	}

	private ProfilePerk HEKMFIFGALO(int GNLOCMLBNHF, RosterPerk PPPNCJLGJPE)
	{
		List<ProfilePerk> jOGBKOJCINM = PerkTree.GBPBIPFIOJH().HKCIFHMLKKM(GNLOCMLBNHF).JOGBKOJCINM;
		for (int i = 0; i < jOGBKOJCINM.Count; i++)
		{
			if (jOGBKOJCINM[i].KAMBOKLFBEE() != PPPNCJLGJPE.get_Name())
			{
				return jOGBKOJCINM[i];
			}
		}
		return null;
	}

	private void IJAFAMPCNIJ()
	{
		FANKDNGGHJG();
		PEKFLFMNMEP();
	}

	private void FANKDNGGHJG()
	{
		_leftPanel.Init();
		GOGMJFKMPAM = _leftPanel.transform.localPosition.x;
		AAAAENBBFMI = GOGMJFKMPAM + _leftPanel.GetComponent<RectTransform>().rect.width;
	}

	private void PEKFLFMNMEP()
	{
		_rightPanel.Init();
	}

	private new void OnDestroy()
	{
		_perksTable.onSelectCell.RemoveAllListeners();
		_achievementsTable.onSelectCell.RemoveAllListeners();
		_achievementsTable.onSelectCell.RemoveAllListeners();
		_tricksTable.onSelectCell.RemoveAllListeners();
		_sealsTable.onSelectCell.RemoveAllListeners();
		ModelContainer.RemoveAllEventListener();
		SubItems.ForEach((SubItem i) =>
		{
			i.RemoveAllEventListener();
		});
		SubItems = null;
		_showAchievementButton.RemoveAllEventListener();
		JHFCFBIPGPF.ForEach((SectionButton AAOIAEJJINO) =>
		{
			AAOIAEJJINO.RemoveAllEventListener();
		});
		JHFCFBIPGPF = null;
		_perkCellPrefab = null;
		_profileTableViewCellPrefab = null;
		_trickCellPrefab = null;
		_achievementCellPrefab = null;
		_sealCellPrefab = null;
		base.OnDestroy();
	}
}
