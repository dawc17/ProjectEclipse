using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Nekki.SF2.Core.Fights;
using Nekki.SF2.GUI.Common;
using Nekki.SF2.GUI.Menu;
using UnityEngine;

namespace Nekki.SF2.GUI.Shop
{
	public class ShopScene : Scene<ShopScene>, ITableViewDataSource, ITableViewDelegate
	{
		[SerializeField]
		private Vector2 _weaponImageSize = new Vector2(778f, 302f);

		[SerializeField]
		private Vector2 _armorImageSize = new Vector2(778f, 704f);

		[SerializeField]
		private Vector2 _helmetImageSize = new Vector2(778f, 706f);

		[SerializeField]
		private Vector2 _rangedImageSize = new Vector2(778f, 302f);

		[SerializeField]
		private Vector2 _magicImageSize = new Vector2(778f, 682f);

		[SerializeField]
		private Vector2 _paymentImageSize = new Vector2(670f, 500f);

		[SerializeField]
		private Vector2 _freeImageSize = new Vector2(670f, 500f);

		[SerializeField]
		private SidePanel _itemInfo;

		[SerializeField]
		private SidePanel _itemParam;

		[SerializeField]
		private SidePanel _itemProperties;

		[SerializeField]
		private ButtonPanel _buttonPanel;

		[SerializeField]
		private LabelAlias _noItemsMessage;

		[SerializeField]
		private MainMenu _mainMenu;

		[SerializeField]
		private LabelButton _tryItemButton;

		[SerializeField]
		private HintPanel _hintPanel;

		[SerializeField]
		private ModelContainer _modelContainer;

		[SerializeField]
		private CanvasGroup _shopUIGroup;

		private Eclipse.Forge.ShopForgeController _forgeController;

		[SerializeField]
		private GameObject _infoPanelContentPrefab;

		[SerializeField]
		private GameObject _ParametersPanelContentPrefab;

		[SerializeField]
		private GameObject _propertiesPanelContentPrefab;

		[SerializeField]
		private GameObject _cheatsPanelPrefab;

		[SerializeField]
		private TableView _shopTableView;

		[SerializeField]
		private GameObject _cellPrefab;

		[SerializeField]
		private SpriteRenderer _backgroundLeft;

		[SerializeField]
		private SpriteRenderer _backgroundRight;

		private PaymentUI ODCDHJGNPEM;

		private List<ItemInfo> DOHLAAPAOOO = new List<ItemInfo>();

		private List<ItemInfo> KBMOJAPFLAO = new List<ItemInfo>();

		private List<ItemInfo> GIECPODANIL = new List<ItemInfo>();

		private List<ItemInfo> IMHIHAMOFJD = new List<ItemInfo>();

		private List<ItemInfo> JFBDPCMEKMN = new List<ItemInfo>();

		private List<ItemInfo> EBHGBBEGOAM = new List<ItemInfo>();

		private List<ItemInfo> AEABDIBDJAH = new List<ItemInfo>();

		private InfoPanelContent AIEKLCMEKMI;

		private ParametersPanelContent JMJOHONKDDO;

		private PropertiesPanelContent KDFADLAANLM;

		private CheatsPanel _CheatsPanel;

		private DelayedStrike PDDJMHMJACO;

		private string MHDHDBOENCD = "ShopPieces.Left_flag";

		private string HIOEPHBLPCE = "ShopPieces.Right_flag";

		private string GHHDJJEMHAN = "ShopPieces.Left_flag_properties";

		private string NGNPGDAKHJJ = "ShopPieces.Right_flag_properties";

		private string PIDAKCDGJFH = "ShopPieces.Left_flag_properties_none";

		private string AGPCJDDILHK = "ShopPieces.Right_flag_properties_none";

		private float ODHAHECBPHI = 1f;

		private float CIBPLGLKGOI = 0.5f;

		private int MCMMNANBKME = 120;

		private float CLBCHPGCGCB = 1f;

		private float APNGDHAICIE;

		private float BHJIDLIBKDD = 0.7f;

		private int BONAMONOIIC;

		private ShopSection NNGHNIJCKLD = ShopSection.Unknown;

		private static Dictionary<ShopSection, ItemInfo> CLFLAOAADGN = new Dictionary<ShopSection, ItemInfo>();

		private List<ItemInfo> PDHEEIPFFME = new List<ItemInfo>();

		private bool MIAFGIJBKHN = true;

		private Vector2 GCMNMDIBAOE = new Vector2(0f, 0f);

		private ShopTableViewCell NNACFMKLHIB;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static ShopScene OGKMDFDNIEN;

		public PaymentUI GKPMFKIEPPB
		{
			get
			{
				return get_PaymentUI();
			}
		}

		public override ScreenType PNAJHDBDDLP
		{
			get
			{
				return get_SceneId();
			}
		}

		public static ShopScene BPCBBHAKFDM
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

		public PaymentUI get_PaymentUI()
		{
			return ODCDHJGNPEM;
		}

		public override ScreenType get_SceneId()
		{
			return ScreenType.ModuleShop;
		}

		public static ShopScene get_Instance()
		{
			return OGKMDFDNIEN;
		}

		public static void set_Instance(ShopScene value)
		{
			OGKMDFDNIEN = value;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			PDDJMHMJACO = data as DelayedStrike;
			set_Instance(this);
			ODCDHJGNPEM = IMDHIBMOAIG<PaymentUI>();
			ODCDHJGNPEM.get_OnProductsUpdateEvent().AddListener(LCHCKOKGFHK);
			if (ListSF.CCDKHLAMKKO() != null && ListSF.CCDKHLAMKKO().KHCNHPCPFII() != null)
			{
				ListSF.CCDKHLAMKKO().KHCNHPCPFII().LHNOOJJNDPK.AddListener(FGHDBGDNHAH);
			}
			if (_mainMenu != null)
			{
				_mainMenu.Init();
			}
			if (_hintPanel != null)
			{
				_hintPanel.Init();
			}
			if (_tryItemButton != null)
			{
				_tryItemButton.onClick.AddListener(GLLPHHALCDA);
			}
			if (_infoPanelContentPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(_infoPanelContentPrefab);
				AIEKLCMEKMI = gameObject.GetComponent<InfoPanelContent>();
				AIEKLCMEKMI.Init();
				AIEKLCMEKMI.updateEvent.AddListener(OEHKNFIDCMI);
			}
			if (_ParametersPanelContentPrefab != null)
			{
				GameObject gameObject2 = Object.Instantiate(_ParametersPanelContentPrefab);
				JMJOHONKDDO = gameObject2.GetComponent<ParametersPanelContent>();
				JMJOHONKDDO.Init();
			}
			if (_propertiesPanelContentPrefab != null)
			{
				GameObject gameObject3 = Object.Instantiate(_propertiesPanelContentPrefab);
				KDFADLAANLM = gameObject3.GetComponent<PropertiesPanelContent>();
				KDFADLAANLM.Init();
				if (KDFADLAANLM.get_PropertiesPanel() != null && _hintPanel != null)
				{
					KDFADLAANLM.get_PropertiesPanel().onPerksClick.AddListener(_hintPanel.ShowPerkHint);
				}
			}
			if (_itemInfo != null)
			{
				bool jOJGKNGGAHB = false;
				_itemInfo.Init(AIEKLCMEKMI, jOJGKNGGAHB);
			}
			if (_itemParam != null)
			{
				bool jOJGKNGGAHB2 = true;
				bool nKGDKKNNJOF = false;
				float mDPGKEDBHNO = -40f;
				_itemParam.Init(JMJOHONKDDO, jOJGKNGGAHB2, mDPGKEDBHNO, nKGDKKNNJOF, HIOEPHBLPCE, MHDHDBOENCD);
			}
			if (_itemProperties != null)
			{
				bool jOJGKNGGAHB3 = true;
				bool nKGDKKNNJOF2 = false;
				float mDPGKEDBHNO2 = 40f;
				_itemProperties.Init(KDFADLAANLM, jOJGKNGGAHB3, mDPGKEDBHNO2, nKGDKKNNJOF2, AGPCJDDILHK, PIDAKCDGJFH);
			}
			if (_buttonPanel != null)
			{
				_buttonPanel.Init();
				if (ListSF.DJBOFEEKJMP().KEFJPEOEPBN().Count == 0)
				{
					_buttonPanel.HideButton(6);
				}
			}
			IDKPGMCBIFM();
			_shopTableView.set_CellPrefab(_cellPrefab);
			_shopTableView.Init(this, this);
			_shopTableView.onSelectCell.AddListener(KPFEGEHJMOH);
			_shopTableView.set_MinScrollVelocity(100f);
			_shopTableView.get_Scroll().onDragBegin.AddListener(_hintPanel.HideHintAndStopCorutine);
			if (_tryItemButton != null)
			{
				Transform forgeParent = _shopUIGroup != null ? _shopUIGroup.transform : transform;
				_forgeController = new Eclipse.Forge.ShopForgeController(this, _mainMenu, _tryItemButton, forgeParent, KDFADLAANLM, _shopTableView != null ? _shopTableView.transform.parent as RectTransform : null, _itemParam != null ? _itemParam.transform as RectTransform : null, _itemProperties != null ? _itemProperties.transform as RectTransform : null);
			}
			if (_modelContainer != null)
			{
				_modelContainer.Init();
			}
			if (SystemProperties.DBBOCENKMGD() && _cheatsPanelPrefab != null)
			{
				_CheatsPanel = Object.Instantiate(_cheatsPanelPrefab).GetComponent<CheatsPanel>();
				Transform parent = ((!(_shopUIGroup != null)) ? base.transform : _shopUIGroup.transform);
				_CheatsPanel.transform.SetParent(parent, false);
				_CheatsPanel.OnShowCheats.AddListener(LAHBGMGMHPO);
				_CheatsPanel.OnAddLevel.AddListener(NKIALIBEBBF);
				_CheatsPanel.HideCheats();
			}
			SetFocusOnStart();
			UpdateModel(ListSF.CCDKHLAMKKO().get_Parameters().JGMLKIPCFII);
			ShowUI();
		}

		protected override void PJNFHNFLNNO()
		{
			_forgeController?.Shutdown();
			_forgeController = null;
			RememberFocus();
			UpdateNewItemsCounters();
			ODCDHJGNPEM.get_OnProductsUpdateEvent().RemoveListener(LCHCKOKGFHK);
			set_Instance(null);
			if (ListSF.CCDKHLAMKKO() != null && ListSF.CCDKHLAMKKO().KHCNHPCPFII() != null)
			{
				ListSF.CCDKHLAMKKO().KHCNHPCPFII().LHNOOJJNDPK.RemoveListener(FGHDBGDNHAH);
			}
			base.PJNFHNFLNNO();
		}

		protected void KPFEGEHJMOH(TableViewCell HJCPCBLCJJN)
		{
			ShopTableViewCell shopTableViewCell = HJCPCBLCJJN as ShopTableViewCell;
			if (!(shopTableViewCell == null))
			{
				if (NNACFMKLHIB != null)
				{
				}
				NNACFMKLHIB = shopTableViewCell;
				NOCJCNJACAD(shopTableViewCell);
				KNOHDLJCHHL(shopTableViewCell);
				if (AIEKLCMEKMI != null)
				{
					AIEKLCMEKMI.SetItemInfo(shopTableViewCell.get_ItemInfo());
				}
				if (_tryItemButton != null)
				{
					FMJNALPHGGF(shopTableViewCell.get_ItemInfo());
				}
				_forgeController?.OnItemSelected(shopTableViewCell.get_ItemInfo());
			}
		}

		private void LCHCKOKGFHK()
		{
			if (NNACFMKLHIB != null && AIEKLCMEKMI != null)
			{
				AIEKLCMEKMI.SetItemInfo(NNACFMKLHIB.get_ItemInfo());
			}
		}

		private void NOCJCNJACAD(ShopTableViewCell LIBKHDGLJFF)
		{
			if (!(LIBKHDGLJFF == null) && !(_itemProperties == null) && !(KDFADLAANLM == null))
			{
				ItemInfo itemInfo = LIBKHDGLJFF.get_ItemInfo();
				if (itemInfo != null && itemInfo.LFIGBCDJHPG.Count > 0)
				{
					_itemProperties.set_OpenImage(NGNPGDAKHJJ);
					_itemProperties.set_CloseImage(GHHDJJEMHAN);
				}
				else
				{
					_itemProperties.set_OpenImage(AGPCJDDILHK);
					_itemProperties.set_CloseImage(PIDAKCDGJFH);
				}
				KDFADLAANLM.SetItemInfo(itemInfo);
			}
		}

		private void KNOHDLJCHHL(ShopTableViewCell LIBKHDGLJFF)
		{
			if (LIBKHDGLJFF != null && LIBKHDGLJFF.get_ItemInfo() != null && JMJOHONKDDO != null)
			{
				JMJOHONKDDO.UpdateParameters(LIBKHDGLJFF.get_ItemInfo());
			}
		}

		private void KNOHDLJCHHL()
		{
			if (JMJOHONKDDO != null && NNACFMKLHIB != null && NNACFMKLHIB.get_ItemInfo() != null)
			{
				JMJOHONKDDO.UpdateParametersWithDuration(NNACFMKLHIB.get_ItemInfo());
			}
		}

		public void DisableButton(ShopSection KGDHCBNKLMF)
		{
			if (_buttonPanel != null)
			{
				_buttonPanel.DisableButton((int)KGDHCBNKLMF);
			}
		}

		private void LAHBGMGMHPO()
		{
			if (_buttonPanel != null)
			{
				_buttonPanel.EnableAllButtons();
			}
			if (_tryItemButton != null)
			{
				_tryItemButton.gameObject.SetActive(false);
			}
			ShowSidePanels(false);
		}

		public void RefreshItems()
		{
			PHKKGEMDCNG();
			UpdateNewItemsCounters();
		}

		private void NKIALIBEBBF()
		{
			PHKKGEMDCNG();
			UpdateNewItemsCounters();
		}

		public void SetFocusOnStart()
		{
			if (PDDJMHMJACO != null)
			{
				GoToSlider(PDDJMHMJACO.SliderType);
				if (PDDJMHMJACO.DLKPBAJDHBO != null)
				{
					ScrollToItem(PDDJMHMJACO.DLKPBAJDHBO);
				}
			}
			else
			{
				GoToSlider(SliderType.SliderWeapon);
			}
		}

		public void RememberFocus()
		{
			if (NNACFMKLHIB != null && NNACFMKLHIB.get_ItemInfo() != null)
			{
				CLFLAOAADGN[NNGHNIJCKLD] = NNACFMKLHIB.get_ItemInfo();
			}
		}

		public void SetFocus()
		{
			if (!FocusOnNewItem() && !FocusOnLastFocus() && !FocusOnEquipedItem() && _shopTableView != null)
			{
				_shopTableView.ScrollToCell(0);
			}
		}

		public bool FocusOnNewItem()
		{
			ItemInfo AOCHMFMOACB = null;
			PDHEEIPFFME.ForEach((ItemInfo DHDMNHCIPEH) =>
			{
				if (DHDMNHCIPEH.DBHJGAGOLOB() && (AOCHMFMOACB == null || (ObscuredLong)(AOCHMFMOACB.FMHECGHHKGB) < (ObscuredLong)(DHDMNHCIPEH.FMHECGHHKGB) || ((ObscuredLong)(AOCHMFMOACB.FMHECGHHKGB) == (ObscuredLong)(DHDMNHCIPEH.FMHECGHHKGB) && (ObscuredLong)(AOCHMFMOACB.KJFAOKLILOC) < (ObscuredLong)(DHDMNHCIPEH.KJFAOKLILOC))))
				{
					AOCHMFMOACB = DHDMNHCIPEH;
				}
			});
			if (AOCHMFMOACB != null)
			{
				ScrollToItem(AOCHMFMOACB);
				return true;
			}
			return false;
		}

		public bool FocusOnLastFocus()
		{
			ItemInfo dJKEECEOCJB = null;
			if (CLFLAOAADGN.ContainsKey(NNGHNIJCKLD))
			{
				ItemInfo DIPNFHJPJGA = CLFLAOAADGN[NNGHNIJCKLD];
				dJKEECEOCJB = PDHEEIPFFME.Find((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(DIPNFHJPJGA.Name));
			}
			if (dJKEECEOCJB != null)
			{
				ScrollToItem(dJKEECEOCJB);
				return true;
			}
			return false;
		}

		public bool FocusOnEquipedItem()
		{
			List<UserItem> list = ListSF.CCDKHLAMKKO().KHCNHPCPFII().DJBOFEEKJMP()
				.FindAll((UserItem DHDMNHCIPEH) => DHDMNHCIPEH.EFMFGEPDAOP());
			ItemInfo dJKEECEOCJB = null;
			foreach (ItemInfo item in PDHEEIPFFME)
			{
				UserItem dKCHDHMLKHN = list.Find((UserItem DHDMNHCIPEH) => DHDMNHCIPEH.BHKHOJPANHE() != null && DHDMNHCIPEH.BHKHOJPANHE().Name.Equals(item.Name));
				if (dKCHDHMLKHN != null)
				{
					dJKEECEOCJB = dKCHDHMLKHN.BHKHOJPANHE();
					break;
				}
			}
			if (dJKEECEOCJB != null)
			{
				ScrollToItem(dJKEECEOCJB);
				return true;
			}
			return false;
		}

		public void ScrollToItem(ItemInfo item)
		{
			ItemInfo dJKEECEOCJB = PDHEEIPFFME.Find((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(item.Name));
			if (dJKEECEOCJB != null && _shopTableView != null)
			{
				int iBAKGENOEPH = PDHEEIPFFME.IndexOf(dJKEECEOCJB);
				_shopTableView.ScrollToCell(iBAKGENOEPH);
			}
		}

		public void ScrollToItemByName(SliderType MNHKGIHKBPO, string FDJFBMNPPLM)
		{
			GoToSlider(MNHKGIHKBPO);
			ItemInfo dJKEECEOCJB = PDHEEIPFFME.Find((ItemInfo FAKOMBAIFPP) => FAKOMBAIFPP.Name == FDJFBMNPPLM);
			if (dJKEECEOCJB != null && _shopTableView != null)
			{
				int iBAKGENOEPH = PDHEEIPFFME.IndexOf(dJKEECEOCJB);
				_shopTableView.ScrollToCell(iBAKGENOEPH);
			}
		}

		public void ScrollToItemByName(ShopSection KGDHCBNKLMF, string OHCGEEEKEJH)
		{
			SetShopSection(KGDHCBNKLMF);
			ItemInfo dJKEECEOCJB = PDHEEIPFFME.Find((ItemInfo DHDMNHCIPEH) => DHDMNHCIPEH.Name.Equals(OHCGEEEKEJH));
			if (dJKEECEOCJB != null && _shopTableView != null)
			{
				int iBAKGENOEPH = PDHEEIPFFME.IndexOf(dJKEECEOCJB);
				_shopTableView.ScrollToCell(iBAKGENOEPH);
			}
		}

		public void GoToSlider(SliderType JFMPFHEPMIE)
		{
			switch (JFMPFHEPMIE)
			{
			case SliderType.SliderWeapon:
				SetShopSection(ShopSection.Weapon);
				break;
			case SliderType.SliderArmor:
				SetShopSection(ShopSection.Armor);
				break;
			case SliderType.SliderHelmet:
				SetShopSection(ShopSection.Helmet);
				break;
			case SliderType.SliderMissile:
				SetShopSection(ShopSection.Ranged);
				break;
			case SliderType.SliderMagic:
				SetShopSection(ShopSection.Magic);
				break;
			case SliderType.SliderRuby:
				SetShopSection(ShopSection.Payment);
				break;
			case SliderType.SliderFree:
				SetShopSection(ShopSection.Free);
				break;
			default:
				SetShopSection(ShopSection.Weapon);
				break;
			}
		}

		private void IDKPGMCBIFM()
		{
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().MJKFCBMNNGJ(), DOHLAAPAOOO);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().MCGKNJPLIIH(), KBMOJAPFLAO);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().EKKIBLDGNHH(), GIECPODANIL);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().LKGPBHADANE(), IMHIHAMOFJD);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().OGFOBKIEGKA(), JFBDPCMEKMN);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().KCIHHGCHEKM(), EBHGBBEGOAM);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().BFFNOIPELKC(), EBHGBBEGOAM);
			DFADPMMIAAL(ListSF.DJBOFEEKJMP().KEFJPEOEPBN(), AEABDIBDJAH);
		}

		private void DFADPMMIAAL(List<ItemInfo> CAIHJJFKFLP, List<ItemInfo> PPFNLLCMHGM)
		{
			foreach (ItemInfo item in CAIHJJFKFLP)
			{
				bool flag = !string.IsNullOrEmpty(item.MMHIKEIDDNB) && !ListSF.CCDKHLAMKKO().FLFKOIPCEPI(item.MMHIKEIDDNB);
				if (item.DCHJDPCEODD && !item.GOKHJMOEGIJ() && !flag)
				{
					PPFNLLCMHGM.Add(item);
				}
			}
		}

		public void SetShopSection(ShopSection KGDHCBNKLMF)
		{
			if (NNGHNIJCKLD == KGDHCBNKLMF)
			{
				return;
			}
			RememberFocus();
			NNGHNIJCKLD = KGDHCBNKLMF;
			ShowSidePanels(true);
			UpdateNewItemsCounters();
			if (_buttonPanel != null)
			{
				_buttonPanel.EnableAllButtons();
			}
			if (_noItemsMessage != null)
			{
				_noItemsMessage.SetAlias(string.Empty);
				_noItemsMessage.set_text(string.Empty);
			}
			if (_CheatsPanel != null)
			{
				_CheatsPanel.HideCheats();
			}
			switch (KGDHCBNKLMF)
			{
			case ShopSection.Weapon:
				CDPHCFAKPLO(DOHLAAPAOOO, 0f, _weaponImageSize);
				DisableButton(ShopSection.Weapon);
				break;
			case ShopSection.Armor:
				CDPHCFAKPLO(KBMOJAPFLAO, 110f, _armorImageSize);
				DisableButton(ShopSection.Armor);
				break;
			case ShopSection.Helmet:
				CDPHCFAKPLO(GIECPODANIL, 110f, _helmetImageSize);
				DisableButton(ShopSection.Helmet);
				break;
			case ShopSection.Ranged:
				CDPHCFAKPLO(IMHIHAMOFJD, 0f, _rangedImageSize);
				DisableButton(ShopSection.Ranged);
				if (_noItemsMessage != null)
				{
					_noItemsMessage.SetAlias("shopRangedLocked");
				}
				break;
			case ShopSection.Magic:
				CDPHCFAKPLO(JFBDPCMEKMN, 120f, _magicImageSize);
				DisableButton(ShopSection.Magic);
				if (_noItemsMessage != null)
				{
					_noItemsMessage.SetAlias("shopMagicLocked");
				}
				break;
			case ShopSection.Payment:
				CDPHCFAKPLO(EBHGBBEGOAM, 20f, _paymentImageSize, false);
				DisableButton(ShopSection.Payment);
				break;
			case ShopSection.Free:
				CDPHCFAKPLO(AEABDIBDJAH, 20f, _freeImageSize, false);
				DisableButton(ShopSection.Free);
				break;
			}
			if (_modelContainer != null && _modelContainer.get__StageType() == StageType.FDBBPEGEGMK.STAGE_SHOP_TRY_ON)
			{
				AKCNEACJIOG(null);
			}
			SetFocus();
		}

		public void UpdateNewItemsCounters()
		{
			if (PDHEEIPFFME != null)
			{
				PDHEEIPFFME.ForEach((ItemInfo DHDMNHCIPEH) =>
				{
					DHDMNHCIPEH.BEBDMOEIEJN(false);
				});
			}
			int num = ListSF.DJBOFEEKJMP().EFEJPENECKN();
			if (BONAMONOIIC != num)
			{
				BONAMONOIIC = num;
				if (_buttonPanel != null)
				{
					_buttonPanel.UpdateNewItemsCounter();
				}
				if (_mainMenu != null)
				{
					_mainMenu.UpdateNewItems();
				}
				ListSF.ELEBLBJKDBI().EJANJEEGOOE();
			}
		}

		private void CDPHCFAKPLO(List<ItemInfo> HELFDCAIJNE, float CPLBEMJADEL, Vector2 PEEOEOMEBFG, bool PMKIFLOFHJG = true)
		{
			if (HELFDCAIJNE == null || _shopTableView == null)
			{
				LLLOJBFMONN.Error("ShopScene.SetItems some field is null");
				return;
			}
			OPGJGJGNLMO(HELFDCAIJNE);
			PDHEEIPFFME = HELFDCAIJNE;
			MIAFGIJBKHN = PMKIFLOFHJG;
			GCMNMDIBAOE = PEEOEOMEBFG;
			_shopTableView.set_Spacing(CPLBEMJADEL);
			_shopTableView.ReloadData();
			_shopTableView.ScrollToCell(0);
			if (_noItemsMessage != null)
			{
				bool active = PDHEEIPFFME.Count == 0;
				_noItemsMessage.gameObject.SetActive(active);
			}
			ShowSidePanels(PDHEEIPFFME.Count > 0);
		}

		private void OPGJGJGNLMO(List<ItemInfo> HELFDCAIJNE)
		{
			HELFDCAIJNE.Sort((ItemInfo FGBJPFPGHKC, ItemInfo ACJEJOKKGNI) =>
			{
				UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(FGBJPFPGHKC);
				UserItem dKCHDHMLKHN2 = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(ACJEJOKKGNI);
				int num = ((dKCHDHMLKHN == null) ? FGBJPFPGHKC.OBJDGBBFJOO : dKCHDHMLKHN.DHNNCAEEMLL());
				int num2 = ((dKCHDHMLKHN2 == null) ? ACJEJOKKGNI.OBJDGBBFJOO : dKCHDHMLKHN2.DHNNCAEEMLL());
				int num3 = HELFDCAIJNE.IndexOf(FGBJPFPGHKC);
				int value = HELFDCAIJNE.IndexOf(ACJEJOKKGNI);
				return (num == num2) ? num3.CompareTo(value) : num.CompareTo(num2);
			});
		}

		private void PHKKGEMDCNG()
		{
			ShopTableViewCell nNACFMKLHIB = NNACFMKLHIB;
			OPGJGJGNLMO(PDHEEIPFFME);
			_shopTableView.ReloadData();
			if (_shopTableView != null && nNACFMKLHIB != null && nNACFMKLHIB != NNACFMKLHIB)
			{
				_shopTableView.ScrollToCell(nNACFMKLHIB.get_RowNumber());
			}
		}

		private void GHKDKGJJPFA(ShopScrollItem item)
		{
			if (item == _shopTableView.get_SelectedCell())
			{
				GLLPHHALCDA();
			}
		}

		private void OEHKNFIDCMI()
		{
			HCONMLFJGII();
			PHKKGEMDCNG();
			KNOHDLJCHHL();
			if (NNACFMKLHIB != null && NNACFMKLHIB.get_ItemInfo() != null)
			{
				FMJNALPHGGF(NNACFMKLHIB.get_ItemInfo());
			}
			if (_mainMenu != null)
			{
				_mainMenu.UpdateMoney();
			}
		}

		private bool ICILAGHAPOO()
		{
			ItemInfo itemInfo = NNACFMKLHIB.get_ItemInfo();
			return itemInfo.Type.Equals("Weapon") || itemInfo.Type.Equals("Armor") || itemInfo.Type.Equals("Helm") || itemInfo.Type.Equals("Ranged") || itemInfo.Type.Equals("Magic");
		}

		private void FMJNALPHGGF(ItemInfo item)
		{
			if (!(_tryItemButton == null) && item != null)
			{
				if (ICILAGHAPOO())
				{
					_tryItemButton.gameObject.SetActive(true);
				}
				else
				{
					_tryItemButton.gameObject.SetActive(false);
				}
				bool PNKJLPDJOJF = false;
				bool CBDBANOPFDM = false;
				InputDeviceExtension.AOGLFIHGKCN(ref PNKJLPDJOJF, ref CBDBANOPFDM, item);
				if (CBDBANOPFDM)
				{
					_tryItemButton.SetAlias("btnShopUnequip");
				}
				else if (PNKJLPDJOJF)
				{
					_tryItemButton.SetAlias("btnShopEquip");
				}
				else
				{
					_tryItemButton.SetAlias("btnShopTry");
				}
			}
		}

		private void GLLPHHALCDA()
		{
			if (!ICILAGHAPOO() || NNACFMKLHIB.get_ItemInfo() == null)
			{
				return;
			}
			ItemInfo itemInfo = NNACFMKLHIB.get_ItemInfo();
			bool PNKJLPDJOJF = false;
			bool CBDBANOPFDM = false;
			InputDeviceExtension.AOGLFIHGKCN(ref PNKJLPDJOJF, ref CBDBANOPFDM, itemInfo);
			if (PNKJLPDJOJF && CBDBANOPFDM)
			{
				ListSF.CCDKHLAMKKO().KHCNHPCPFII().JALMHIICOPB(itemInfo, true);
				PHKKGEMDCNG();
				HCONMLFJGII();
				KNOHDLJCHHL();
				FMJNALPHGGF(itemInfo);
				return;
			}
			if (PNKJLPDJOJF && !CBDBANOPFDM)
			{
				ListSF.CCDKHLAMKKO().KHCNHPCPFII().EEDJEDBMIMI(itemInfo, true);
				PHKKGEMDCNG();
				HCONMLFJGII(itemInfo);
				KNOHDLJCHHL();
				FMJNALPHGGF(itemInfo);
				return;
			}
			ShopTableViewCell shopTableViewCell = _shopTableView.get_SelectedCell() as ShopTableViewCell;
			if (shopTableViewCell != null)
			{
				if (_modelContainer != null)
				{
					_modelContainer.AddEventListener(1, AKCNEACJIOG);
				}
				UpdateModel(shopTableViewCell.get_ItemInfo(), StageType.FDBBPEGEGMK.STAGE_SHOP_TRY_ON);
				HideUI();
			}
		}

		private void AKCNEACJIOG(object data)
		{
			if (_modelContainer != null)
			{
				_modelContainer.RemoveEventListener(1, AKCNEACJIOG);
			}
			ShowUI();
			StartCoroutine(EKGBMDLFHGA());
		}

		private IEnumerator EKGBMDLFHGA()
		{
			yield return new WaitForEndOfFrame();
			UpdateModel(null, StageType.FDBBPEGEGMK.STAGE_PEACEFUL_RESTORE);
		}

		private void FGHDBGDNHAH(List<UserItem> GOGGLLFHAMB)
		{
			if (AIEKLCMEKMI != null && NNACFMKLHIB != null)
			{
				AIEKLCMEKMI.SetItemInfo(NNACFMKLHIB.get_ItemInfo());
			}
			PHKKGEMDCNG();
		}

		public override void UpdateScene(object data)
		{
			if (AIEKLCMEKMI != null)
			{
				AIEKLCMEKMI.UpdateContent();
			}
			_forgeController?.Tick();
		}

		public bool IsForgeCanBeOpened()
		{
			return _forgeController != null && _forgeController.CanOpen();
		}

		public bool OpenForgeAndSetRecipe(string recipeName)
		{
			return _forgeController != null && _forgeController.Open(recipeName);
		}

		public void RestoreForgeClosedState()
		{
			bool hasSelection = NNACFMKLHIB != null && NNACFMKLHIB.get_ItemInfo() != null;
			ShowSidePanels(hasSelection);
			if (_tryItemButton != null)
			{
				if (hasSelection) FMJNALPHGGF(NNACFMKLHIB.get_ItemInfo());
				else _tryItemButton.gameObject.SetActive(false);
			}
		}

		public void RefreshAfterForgeMutation()
		{
			ItemInfo selectedInfo = NNACFMKLHIB != null ? NNACFMKLHIB.get_ItemInfo() : null;
			PHKKGEMDCNG();
			if (selectedInfo == null) return;

			if (AIEKLCMEKMI != null) AIEKLCMEKMI.SetItemInfo(selectedInfo);
			if (JMJOHONKDDO != null) JMJOHONKDDO.UpdateParameters(selectedInfo);
			if (KDFADLAANLM != null) KDFADLAANLM.SetItemInfo(selectedInfo);
			if (_itemProperties != null)
			{
				bool hasEnchantments = ListSF.EIMKEJNJMEJ(selectedInfo).Count > 0;
				_itemProperties.set_OpenImage(hasEnchantments ? NGNPGDAKHJJ : AGPCJDDILHK);
				_itemProperties.set_CloseImage(hasEnchantments ? GHHDJJEMHAN : PIDAKCDGJFH);
			}
			if (_tryItemButton != null) FMJNALPHGGF(selectedInfo);
		}

		private void HCONMLFJGII(ItemInfo DDCFPIDHLGJ = null)
		{
			if (_modelContainer != null && _modelContainer.IsItemDiffer(ListSF.CCDKHLAMKKO().get_Parameters()))
			{
				UpdateModel(DDCFPIDHLGJ);
			}
		}

		private void UpdateModel(ItemInfo DDCFPIDHLGJ, StageType.FDBBPEGEGMK LGPIFNMFPAN = StageType.FDBBPEGEGMK.STAGE_SHOP_START)
		{
			if (_modelContainer != null)
			{
				ItemInfo dJKEECEOCJB = DDCFPIDHLGJ;
				if (dJKEECEOCJB == null)
				{
					ShopTableViewCell shopTableViewCell = _shopTableView.get_SelectedCell() as ShopTableViewCell;
					dJKEECEOCJB = ((!(shopTableViewCell != null)) ? ListSF.CCDKHLAMKKO().get_Parameters().LKKFNMBCCDB : shopTableViewCell.get_ItemInfo());
				}
				if (dJKEECEOCJB != null)
				{
					_modelContainer.UpdateModel(DDCFPIDHLGJ, LGPIFNMFPAN, dJKEECEOCJB.Type);
				}
			}
		}

		public void ShowUI()
		{
			_backgroundLeft.color = Constants.GFBLKELEBEH;
			_backgroundRight.color = Constants.GFBLKELEBEH;
			SetOpacity(CLBCHPGCGCB);
			if (_shopUIGroup != null)
			{
				_shopUIGroup.blocksRaycasts = true;
			}
		}

		private bool DKCEAEKPJHC()
		{
			ShopSection nNGHNIJCKLD = NNGHNIJCKLD;
			if (nNGHNIJCKLD == ShopSection.Armor || nNGHNIJCKLD == ShopSection.Helmet)
			{
				return false;
			}
			return true;
		}

		public void HideUI()
		{
			if (DKCEAEKPJHC())
			{
				_backgroundLeft.color = Constants.EKJMAIDGKME;
				_backgroundRight.color = Constants.EKJMAIDGKME;
				SetOpacity(APNGDHAICIE);
				if (_shopUIGroup != null)
				{
					_shopUIGroup.blocksRaycasts = false;
				}
			}
		}

		public void SetOpacity(float KGJALFLDIBG)
		{
			if (_shopUIGroup != null)
			{
				_shopUIGroup.DOFade(KGJALFLDIBG, BHJIDLIBKDD);
			}
		}

		public void ShowSidePanels(bool value)
		{
			bool active = NNGHNIJCKLD != ShopSection.Payment && NNGHNIJCKLD != ShopSection.Free && value;
			if (_itemInfo != null)
			{
				_itemInfo.gameObject.SetActive(value);
			}
			if (_itemParam != null)
			{
				_itemParam.gameObject.SetActive(active);
			}
			if (_itemProperties != null)
			{
				_itemProperties.gameObject.SetActive(active);
			}
		}

		public InfoPanelContent GetInfoPanel()
		{
			return AIEKLCMEKMI;
		}

		public int NumberOfRowsInTableView(TableView OIDFBEAABBA)
		{
			return PDHEEIPFFME.Count;
		}

		public float SizeForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
		{
			switch (NNGHNIJCKLD)
			{
			case ShopSection.Weapon:
				return 422f;
			case ShopSection.Armor:
				return 824f;
			case ShopSection.Helmet:
				return 826f;
			case ShopSection.Ranged:
				return 422f;
			case ShopSection.Magic:
				return 802f;
			case ShopSection.Payment:
				return 500f;
			case ShopSection.Free:
				return 500f;
			default:
				return 500f;
			}
		}

		public TableViewCell CellForRowInTableView(TableView OIDFBEAABBA, int IBAKGENOEPH)
		{
			TableViewCell tableViewCell = OIDFBEAABBA.ReusableCellForRow(IBAKGENOEPH);
			ShopTableViewCell component = tableViewCell.GetComponent<ShopTableViewCell>();
			component.set_BaseSize(GCMNMDIBAOE);
			component.set_IconPanelActive(MIAFGIJBKHN);
			component.SetItemInfo(PDHEEIPFFME[IBAKGENOEPH]);
			component.set_Index(IBAKGENOEPH);
			if (component.get_PerksPanel() != null && _hintPanel != null)
			{
				component.get_PerksPanel().onPerksClick.RemoveListener(_hintPanel.ShowPerkHint);
				component.get_PerksPanel().onPerksClick.AddListener(_hintPanel.ShowPerkHint);
			}
			return tableViewCell;
		}

		public void TableViewDidHighlightCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
		{
		}

		public void TableViewDidSelectCellForRow(TableView OIDFBEAABBA, int IBAKGENOEPH)
		{
			_shopTableView.ScrollToCell(IBAKGENOEPH, 0.5f);
		}
	}
}
