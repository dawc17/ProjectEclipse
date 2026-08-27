using System.Diagnostics;
using Nekki.SF2.Core.Quests;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MainMenu : SFMonoBehaviour<object>, BackKeyController
	{
		public enum BGGGJCMEGPH
		{
			MENU_DOJO = 0,
			MENU_SHOP = 1,
			MENU_MAP = 2,
			MENU_PROFILE = 3,
			MENU_MONEY = 4,
			MENU_EXIT = 5,
			MENU_SETTINGS = 6,
			MENU_DOJO_DISCIPLE = 7
		}

		private enum NNKHJFKDPHJ
		{
			ZCompare = 0,
			ZDojoDisciple = 1,
			ZScroll = 2,
			ZBackground = 3,
			ZContent = 4,
			ZTopContent = 5,
			ZMaterialsPanel = 6,
			ZHint = 7
		}

		public const float MENU_BAR_SHADOW_HEIGHT = 20f;

		public const float MENU_SCROLL_SPEED = 0.25f;

		public const string MENU_BUTTON_DISCIPLE_ON = "MenuButtons.btn_punching_bag";

		public const string MENU_BUTTON_DISCIPLE_OFF = "MenuButtons.btn_disciple";

		private bool LGIPIBGBOOG = true;

		private bool BFKILDFOEBD = true;

		[SerializeField]
		private MenuExpPanel _experience;

		[SerializeField]
		private MenuEnergyPanel _energy;

		[SerializeField]
		private MenuMoneyPanel _money;

		[SerializeField]
		private MenuMaterialsPanel _materials;

		[SerializeField]
		private GameObject _raidRating;

		[SerializeField]
		private Button _skipTutorialBtn;

		[SerializeField]
		private Image _newPerksCircle;

		[SerializeField]
		private Image _newPerksEllipse;

		[SerializeField]
		private Text _newPerksLabel;

		[SerializeField]
		private Image _newItemsCircle;

		[SerializeField]
		private Image _newItemsEllipse;

		[SerializeField]
		private Text _newItemsLabel;

		[SerializeField]
		private LabelAlias _menuLabel;

		private Slider JFMPFHEPMIE;

		private SliderType GNECFGFOMCO;

		private SectionButton HNCLEDJDODK;

		[SerializeField]
		private SectionButton btnDojo;

		[SerializeField]
		private SectionButton btnMap;

		[SerializeField]
		private SectionButton btnShop;

		[SerializeField]
		private SectionButton btnProfile;

		[SerializeField]
		private SectionButton btnSettings;

		[SerializeField]
		private Toggle btnDojoDisciple;

		[SerializeField]
		private Image _backgroundPicture;

		[SerializeField]
		public MenuScroll Scroll;

		[SerializeField]
		private GameObject _settingsDlgPrefab;

		[SerializeField]
		private GameObject _menuBlocker;

		private int _sliderVisibleItems;

		private float HNOJHOEAHIJ;

		private MenuScroll.ANJKEGGALAG GLJEDBNOHBM = MenuScroll.ANJKEGGALAG.ScrollOpen;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static MainMenu OGKMDFDNIEN;

		public static MainMenu BPCBBHAKFDM
		{
			get
			{
				return get_Instance();
			}
			set
			{
				set_Instance(value);
			}
		}

		public static MainMenu get_Instance()
		{
			return OGKMDFDNIEN;
		}

		public static void set_Instance(MainMenu value)
		{
			OGKMDFDNIEN = value;
		}

		public void Init()
		{
			set_Instance(this);
			_money.Init();
			_experience.Init();
			_energy.Init();
			_materials.Init();
			LGGBLFOKHAO();
			GBKFLJIEHBH();
			HIPEIJPLBJJ();
			LFJEDFKNKCH();
			AIJADDNEMIP();
			AMLMDKMDLEA();
			IDEDNCKEDHG();
			KEBHMKNOOFC();
			LINDDBLFMHJ();
			if (AssemblyController.KMEOEAGGPBI())
			{
			}
			SetNormalViewMode(false);
			UpdateMenu();
			_menuBlocker.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			if (btnDojoDisciple != null)
			{
				btnDojoDisciple.onValueChanged.RemoveListener(OnDojoDiscipleChanged);
			}
			set_Instance(null);
			Scroll.RemoveAllEventListener();
			_money.RemoveAllEventListener();
			_energy.RemoveAllEventListener();
			_experience.RemoveAllEventListener();
		}

		public void Destroy()
		{
			if (AssemblyController.KMEOEAGGPBI())
			{
			}
			_skipTutorialBtn.onClick.RemoveListener(() =>
			{
				SkipTutorial();
			});
		}

		private void LGGBLFOKHAO()
		{
			GameUtils.LHHKFKLELMK = _backgroundPicture.rectTransform.rect.height;
			GameUtils.FPDINCCPGMO = 20f;
		}

		private void GBKFLJIEHBH()
		{
			BOJNFFALDHH();
			AOLACIPNFFP();
			FEOMOKGELOH();
			Scroll.Init();
			Scroll.SetAllowRolling(true);
			Scroll.SetOutsideTouchProperties(true);
			Scroll.AddEventListener(2, IDFCHLJMFJC);
			Scroll.AddEventListener(3, FCLOKKBAMIL);
			Scroll.AddEventListener(4, NPPDCDCLJKN);
			Scroll.Collapse(0f);
			NIGAFHNNOPH();
			ScreenType cCGJDFLIKFN = Module.ELEBLBJKDBI().NMCNDOPKFJD();
			UpdateCurrentButton(cCGJDFLIKFN);
		}

		private void NIGAFHNNOPH()
		{
		}

		private string BJHIIKEJKMM()
		{
			return string.Empty;
		}

		private void IDFCHLJMFJC(object data)
		{
		}

		private void HNNNDCPDOJF()
		{
		}

		private void OGGMAHKAJNL()
		{
		}

		private void FCLOKKBAMIL(object data)
		{
			MenuScroll.ANJKEGGALAG aNJKEGGALAG = (MenuScroll.ANJKEGGALAG)data;
			if (GLJEDBNOHBM == aNJKEGGALAG)
			{
				return;
			}
			GLJEDBNOHBM = aNJKEGGALAG;
			switch (aNJKEGGALAG)
			{
			case MenuScroll.ANJKEGGALAG.ScrollOpen:
				BFKILDFOEBD = true;
				ModelAi.set_AiOn(false);
				BackKeyManager.get_Instance().AddBackKeyController(this);
				HNNNDCPDOJF();
				break;
			case MenuScroll.ANJKEGGALAG.ScrollClose:
				if (BFKILDFOEBD)
				{
					ModelAi.set_AiOn(true);
				}
				BackKeyManager.get_Instance().RemoveBackKeyController(this);
				OGGMAHKAJNL();
				break;
			}
		}

		private void NPPDCDCLJKN(object data)
		{
			if (!Scroll.IsExpanded())
			{
				UpdateScrollMenu();
			}
		}

		public void UpdateScrollMenu()
		{
			UpdateNewItems();
			UpdateNewPerks();
		}

		private void BOJNFFALDHH()
		{
			btnDojo.onClick.AddListener(() =>
			{
				OnClickButton(BGGGJCMEGPH.MENU_DOJO);
			});
			btnMap.onClick.AddListener(() =>
			{
				OnClickButton(BGGGJCMEGPH.MENU_MAP);
			});
			btnShop.onClick.AddListener(() =>
			{
				OnClickButton(BGGGJCMEGPH.MENU_SHOP);
			});
			btnProfile.onClick.AddListener(() =>
			{
				OnClickButton(BGGGJCMEGPH.MENU_PROFILE);
			});
			btnSettings.onClick.AddListener(() =>
			{
				OnClickButton(BGGGJCMEGPH.MENU_SETTINGS);
			});
			if (btnDojoDisciple != null)
			{
				// The exported Toggle lost its persistent callback, so the kid/bag
				// selector rendered and animated but never changed the dojo model.
				btnDojoDisciple.onValueChanged.AddListener(OnDojoDiscipleChanged);
			}
			btnDojo.IsOneShot = true;
			btnMap.IsOneShot = true;
			btnShop.IsOneShot = true;
			btnProfile.IsOneShot = true;
		}

		private void OnDojoDiscipleChanged(bool value)
		{
			if (Module.ELEBLBJKDBI().NMCNDOPKFJD() == ScreenType.ModuleDojo)
			{
				OnClickButton(BGGGJCMEGPH.MENU_DOJO_DISCIPLE);
			}
		}

		private void HIPEIJPLBJJ()
		{
			_experience.AddEventListener(0, GAHLLNAMBAG);
		}

		public void UpdateLevel()
		{
			if ((bool)_experience)
			{
				_experience.UpdateLevel();
			}
		}

		public void UpdateRaidRating()
		{
			if (!_raidRating)
			{
			}
		}

		private void AIJADDNEMIP()
		{
			bool flag = SystemProperties.DBBOCENKMGD() && Module.ELEBLBJKDBI().OMDLOOFIJDF();
			_skipTutorialBtn.gameObject.SetActive(flag);
			if (flag)
			{
				_skipTutorialBtn.onClick.AddListener(() =>
				{
					SkipTutorial();
				});
			}
		}

		private void AMLMDKMDLEA()
		{
			_money.AddEventListener(0, LCLMOFGJJBB);
		}

		public void UpdateMoney()
		{
			if ((bool)_money)
			{
				_money.UpdateValues();
			}
		}

		public void UpdateRubySale()
		{
			if ((bool)_money)
			{
				_money.UpdateRubySale();
			}
		}

		public Button GetRubyBtn()
		{
			if (!_money)
			{
				return null;
			}
			return _money.GetRubyBtn();
		}

		private void IDEDNCKEDHG()
		{
			_materials.gameObject.SetActive(false);
		}

		public void UpdateMainMenu()
		{
			_money.UpdateRuby();
			UpdateMenu();
		}

		public void UpdateBarExp(float OBLEMIHLFII, float KAEPJHHLLPK)
		{
			if ((bool)_experience)
			{
				_experience.UpdateBarExp(OBLEMIHLFII, KAEPJHHLLPK);
			}
		}

		private void LFJEDFKNKCH()
		{
			_energy.AddEventListener(0, PDAINDBEFAH);
		}

		public void UpdateBarEnergy()
		{
			_energy.UpdateBar();
		}

		public void UpdateEnergyView()
		{
			_energy.UpdateView();
			LINDDBLFMHJ();
		}

		private void PDAINDBEFAH(object data)
		{
			if (!ListSF.CCDKHLAMKKO().ADKHNLAMDJP && ListSF.ELEBLBJKDBI().FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENERGY_BAR_PRESS))
			{
				ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
			}
		}

		public void UpdateMenu()
		{
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			if (0 == 0)
			{
				_experience.gameObject.SetActive(true);
				_energy.gameObject.SetActive(true);
				if ((bool)_raidRating)
				{
					_raidRating.gameObject.SetActive(false);
				}
				UpdateLevel();
				UpdateBarExp(nKGLHEGIKKP.EOKLELGLHJJ(), nKGLHEGIKKP.HEOHJNFGEDH());
			}
			else
			{
				_experience.gameObject.SetActive(false);
				_energy.gameObject.SetActive(false);
				_raidRating.gameObject.SetActive(true);
				UpdateRaidRating();
			}
			UpdateMoney();
			UpdateNewPerks();
			UpdateNewItems();
			UpdateMaterials();
			UpdateBarEnergy();
			LINDDBLFMHJ();
		}

		public void UpdateMenuSize()
		{
			UpdateMenu();
		}

		public void CloseMenu(float _Duration = 0f)
		{
			Scroll.Collapse(_Duration);
		}

		private void LCLMOFGJJBB(object data)
		{
			OnClickButton(BGGGJCMEGPH.MENU_MONEY);
		}

		private void OnClickButton(BGGGJCMEGPH KNCNFGABHCL)
		{
			BFKILDFOEBD = true;
			switch (KNCNFGABHCL)
			{
			case BGGGJCMEGPH.MENU_DOJO:
				Module.DLOKJOHNDID(ScreenType.ModuleDojo);
				break;
			case BGGGJCMEGPH.MENU_SHOP:
				Module.DLOKJOHNDID(ScreenType.ModuleShop);
				break;
			case BGGGJCMEGPH.MENU_MAP:
				Module.DLOKJOHNDID(ScreenType.ModuleMap);
				break;
			case BGGGJCMEGPH.MENU_EXIT:
				GameUtils.PGLIKMEJBPK();
				break;
			case BGGGJCMEGPH.MENU_PROFILE:
				Module.DLOKJOHNDID(ScreenType.ModuleProfile);
				break;
			case BGGGJCMEGPH.MENU_MONEY:
			{
				GNECFGFOMCO = SliderType.SliderRuby;
				ShopScene current = Scene<ShopScene>.get_Current();
				if (current != null)
				{
					current.GoToSlider(GNECFGFOMCO);
				}
				else
				{
					Module.DLOKJOHNDID(ScreenType.ModuleShop, new DelayedStrike(GNECFGFOMCO));
				}
				break;
			}
			case BGGGJCMEGPH.MENU_SETTINGS:
				BFKILDFOEBD = false;
				DialogsOpener.DBHBIMGMIEH();
				CloseMenu(0.25f);
				break;
			case BGGGJCMEGPH.MENU_DOJO_DISCIPLE:
				ListSF.CCDKHLAMKKO().MHGIEFLBBGM();
				UpdateDojoDiscipleButton();
				Module.DLOKJOHNDID(ScreenType.ModuleDojo);
				break;
			}
			if (KNCNFGABHCL == BGGGJCMEGPH.MENU_SETTINGS)
			{
			}
		}

		public void SetEnabled(bool PKHDLOGJKAD)
		{
			if (IsEnabled() != PKHDLOGJKAD)
			{
				LGIPIBGBOOG = PKHDLOGJKAD;
				_menuBlocker.gameObject.SetActive(!LGIPIBGBOOG);
				if (!LGIPIBGBOOG)
				{
					Scroll.Collapse(0f);
				}
			}
		}

		public bool IsEnabled()
		{
			return LGIPIBGBOOG;
		}

		public void SkipTutorial()
		{
			if (ListSF.ELEBLBJKDBI().OMDLOOFIJDF())
			{
				string currentQuestName = QuestsManager.get_Instance().CurrentQuestName;
				QuestStage questByName = QuestsManager.get_Instance().GetQuestByName(currentQuestName);
				if (questByName != null)
				{
					questByName.MFGLIALECAM();
				}
			}
			ListSF.CCDKHLAMKKO().BKBHIMEEDBG().set_StoryTutorialStep(GameUtils.AKPBNLKFONO.StepsNames[GameUtils.AKPBNLKFONO.StepsNames.Count - 1]);
			_skipTutorialBtn.gameObject.SetActive(false);
			Module.DLOKJOHNDID(ScreenType.ModuleMap);
		}

		public void UpdateCurrentButton(ScreenType CCGJDFLIKFN)
		{
			switch (CCGJDFLIKFN)
			{
			case ScreenType.ModuleShop:
				SetCurrentButton(btnShop);
				break;
			case ScreenType.ModuleMap:
				SetCurrentButton(btnMap);
				break;
			case ScreenType.ModuleDojo:
				SetCurrentButton(btnDojo);
				break;
			case ScreenType.ModuleProfile:
				SetCurrentButton(btnProfile);
				break;
			default:
				SetCurrentButton(null);
				break;
			}
			UpdateDojoDiscipleVisibility(CCGJDFLIKFN);
		}

		public SectionButton GetButtonFromScreen(ScreenType CCGJDFLIKFN)
		{
			switch (CCGJDFLIKFN)
			{
			case ScreenType.ModuleDojo:
				return btnDojo;
			case ScreenType.ModuleMap:
				return btnMap;
			case ScreenType.ModuleShop:
				return btnShop;
			case ScreenType.ModuleProfile:
				return btnProfile;
			default:
				return null;
			}
		}

		private void KEBHMKNOOFC()
		{
			bool flag = ListSF.CCDKHLAMKKO().BGBFBIDOECK() == 1;
			ResolutionImage resolutionImage = btnDojoDisciple.targetGraphic as ResolutionImage;
			resolutionImage.set_SpriteName((!flag) ? "MenuButtons.btn_disciple" : "MenuButtons.btn_punching_bag");
			Roster roster = ListSF.CCDKHLAMKKO();
			UpdateDojoDiscipleVisibility(Module.ELEBLBJKDBI().NMCNDOPKFJD());
		}

		private void UpdateDojoDiscipleVisibility(ScreenType screen)
		{
			if (btnDojoDisciple == null)
			{
				return;
			}
			Roster roster = ListSF.CCDKHLAMKKO();
			bool unlocked = roster != null && roster.FJGCOOAACLD("ShowDojoDisciple") &&
				roster.GetSettingsXML("ShowDojoDisciple") != "0";
			btnDojoDisciple.gameObject.SetActive(unlocked && screen == ScreenType.ModuleDojo);
		}

		public void UpdateDojoDiscipleButton()
		{
			if (btnDojoDisciple != null)
			{
				bool flag = ListSF.CCDKHLAMKKO().BGBFBIDOECK() == 1;
				ResolutionImage resolutionImage = btnDojoDisciple.targetGraphic as ResolutionImage;
				resolutionImage.set_SpriteName((!flag) ? "MenuButtons.btn_disciple" : "MenuButtons.btn_punching_bag");
			}
		}

		public void ShowDojoDiscipleButton(bool value)
		{
			if ((bool)btnDojoDisciple)
			{
				btnDojoDisciple.gameObject.SetActive(value);
			}
		}

		private void LINDDBLFMHJ()
		{
		}

		private void GAHLLNAMBAG(object EMBBNNBFODN)
		{
			Roster nKGLHEGIKKP = ListSF.CCDKHLAMKKO();
			string hCPNFPMHFCM = ((nKGLHEGIKKP.EOKLELGLHJJ() != nKGLHEGIKKP.HEOHJNFGEDH()) ? LocalizationManager.GetString("experienceHint", nKGLHEGIKKP.EOKLELGLHJJ().ToString(), nKGLHEGIKKP.HEOHJNFGEDH().ToString()) : LocalizationManager.GetString("dlgComingSoonText"));
			_experience.ShowHint(hCPNFPMHFCM);
		}

		private void LIMBEOKCMOD()
		{
			NIGAFHNNOPH();
			FEOMOKGELOH();
			AOLACIPNFFP();
		}

		private int AJENLAJMCCI()
		{
			if (HNCLEDJDODK == btnDojo)
			{
				return 0;
			}
			if (HNCLEDJDODK == btnMap)
			{
				return 1;
			}
			if (HNCLEDJDODK == btnShop)
			{
				return 2;
			}
			if (HNCLEDJDODK == btnProfile)
			{
				return 3;
			}
			if (HNCLEDJDODK == btnSettings)
			{
				return 4;
			}
			return -1;
		}

		public void ReloadAllTextureAtlas()
		{
		}

		public void OnBackKeyClicked(object data)
		{
			CloseMenu(0.25f);
			BackKeyManager.get_Instance().RemoveBackKeyController(this);
		}

		private void AOLACIPNFFP()
		{
			_newPerksCircle.gameObject.SetActive(false);
			_newPerksEllipse.gameObject.SetActive(false);
			_newPerksLabel.gameObject.SetActive(false);
		}

		private void FEOMOKGELOH()
		{
			_newItemsCircle.gameObject.SetActive(false);
			_newItemsEllipse.gameObject.SetActive(false);
			_newItemsLabel.gameObject.SetActive(false);
		}

		public void UpdateNewPerks()
		{
			_newPerksCircle.gameObject.SetActive(false);
			_newPerksEllipse.gameObject.SetActive(false);
			_newPerksLabel.gameObject.SetActive(false);
			int num = ListSF.CCDKHLAMKKO().JLBDOBLHHAF().OPPFMFKAOIG() + ListSF.CCDKHLAMKKO().KJNPJKEHGLE().JKGGEMEBPCP() + ListSF.CCDKHLAMKKO().NPKBPGMNDFJ() + ListSF.CCDKHLAMKKO().CNFOLIEFJCE();
			if (num > 0)
			{
				if (num < 10)
				{
					_newPerksCircle.gameObject.SetActive(true);
				}
				else
				{
					_newPerksEllipse.gameObject.SetActive(true);
				}
				_newPerksLabel.text = num.ToString();
				_newPerksLabel.gameObject.SetActive(true);
			}
		}

		public void UpdateNewItems()
		{
			_newItemsCircle.gameObject.SetActive(false);
			_newItemsEllipse.gameObject.SetActive(false);
			_newItemsLabel.gameObject.SetActive(false);
			int num = ListSF.DJBOFEEKJMP().EFEJPENECKN();
			if (num > 0)
			{
				if (num < 10)
				{
					_newItemsCircle.gameObject.SetActive(true);
				}
				else
				{
					_newItemsEllipse.gameObject.SetActive(true);
				}
				_newItemsLabel.text = num.ToString();
				_newItemsLabel.gameObject.SetActive(true);
			}
		}

		public void UpdateMaterials()
		{
			if ((bool)_materials)
			{
				_materials.UpdateView();
				LINDDBLFMHJ();
			}
		}

		private void BMCLJOPBBNA(object data)
		{
		}

		private void GAALGNEPKEF()
		{
			ScreenType cCGJDFLIKFN = Module.ELEBLBJKDBI().NMCNDOPKFJD();
			if (HNCLEDJDODK != GetButtonFromScreen(cCGJDFLIKFN))
			{
				CloseMenu(0.25f);
			}
		}

		private void INCIAEMHDHE()
		{
			CloseMenu(0.25f);
		}

		public void SetCurrentButton(SectionButton KLNKEPMAGKF)
		{
			if ((bool)HNCLEDJDODK)
			{
				HNCLEDJDODK.interactable = true;
				HNCLEDJDODK.transition = Selectable.Transition.ColorTint;
				HNCLEDJDODK.OFPNNIBBNCE(NFOGOFFAPPP.HHGPKAJENGF.PressNormal);
			}
			HNCLEDJDODK = KLNKEPMAGKF;
			if ((bool)HNCLEDJDODK)
			{
				HNCLEDJDODK.interactable = false;
				HNCLEDJDODK.transition = Selectable.Transition.SpriteSwap;
				HNCLEDJDODK.OFPNNIBBNCE(NFOGOFFAPPP.HHGPKAJENGF.PressInactive);
			}
		}

		public void RecreateMoney()
		{
			AMLMDKMDLEA();
			LINDDBLFMHJ();
			UpdateMenu();
		}

		public void SetNormalViewMode(bool DGNLFEPIANN)
		{
			_energy.gameObject.SetActive(true);
			_money.SetNormalViewMode();
			BHBADDAEICJ(DGNLFEPIANN);
		}

		public void SetForgeViewMode(bool DGNLFEPIANN)
		{
			PDBCNNFEMJA(DGNLFEPIANN);
		}

		private void BHBADDAEICJ(bool DGNLFEPIANN)
		{
		}

		private void DMNJFBBNOOO()
		{
			_materials.gameObject.SetActive(false);
		}

		private void PDBCNNFEMJA(bool DGNLFEPIANN)
		{
		}

		private void GAAPKDHPJNB()
		{
			_energy.gameObject.SetActive(false);
			_money.SetForgeViewMode();
		}

		private void GAKACHNBENN()
		{
			NIGAFHNNOPH();
			if (!btnDojoDisciple)
			{
			}
		}

		private void KAMOJAKJILE()
		{
			NIGAFHNNOPH();
			if (!btnDojoDisciple)
			{
			}
		}

		public Button GetScrollBtn()
		{
			return Scroll.GetButton();
		}

		public Button GetSkipBtn()
		{
			return _skipTutorialBtn;
		}
	}
}
