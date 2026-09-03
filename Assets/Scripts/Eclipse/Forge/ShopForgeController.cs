using System;
using System.Collections.Generic;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Menu;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Forge
{
	/// <summary>
	/// Projects the recovered forge lifecycle into the recovered shop.
	///
	/// Every rect, sprite name, font size and layout constant below is taken from the
	/// intact 1.0.6 assets archived under
	/// ResearchSources/ReferenceSF2DE106/ExportedProject:
	///
	///   src/GUI/Scenes/Shop/Shop.unity            -> ForgePanel (670x902, paper scroll art)
	///   Resources/prefabs/shop/forge/RecipeUI      -> 566x758 recipe cell
	///   Resources/prefabs/shop/forge/RecipePropertyUI
	///   Resources/prefabs/shop/forge/MaterialUI
	///   Resources/prefabs/shop/PropertiesPanelContent -> forge ButtonsPanel
	///
	/// Both projects author their shop canvas at the same 800x1536 reference
	/// resolution, so the reference metrics are used unscaled.
	/// Gameplay stays owned by ForgeManager/ListSF/UserItem.
	/// </summary>
	public sealed class ShopForgeController
	{
		private const float RefreshInterval = 0.25f;

		// SidePanel drives its open/close state through a DOTween; a non-zero
		// duration keeps its completion callback (which restores BlocksRaycasts and
		// the move-button state) on the normal code path.
		private const float SidePanelMoveDuration = 0.2f;

		// Shop.unity: ShopScene._ForgeModeX moves the panels container in forge mode.
		private const float ForgeModeOffset = 605f;

		// Shop.unity: ForgePanel rect, anchored to the left edge of ShopUIGroup.
		private const float PanelWidth = 670f;
		private const float PanelHeight = 902f;
		private const float PanelY = 42f;

		// RecipeUI.prefab root rect.
		private const float RecipeCellWidth = 566f;
		private const float RecipeCellHeight = 758f;

		private static readonly Color DarkText = new Color(0.18431373f, 0.14509805f, 0.105882354f, 1f);
		private static readonly Color MissingMaterialText = new Color(0.60f, 0.13f, 0.08f, 1f);
		private static readonly Color FreeText = new Color(0.99607843f, 0.99607843f, 0.37254903f, 1f);

		private readonly ShopScene _shop;
		private readonly MainMenu _mainMenu;
		private readonly LabelButton _buttonTemplate;
		private readonly Transform _uiParent;
		private readonly PropertiesPanelContent _propertiesContent;
		private readonly RectTransform _itemsRoot;
		private readonly RectTransform _parametersRoot;
		private readonly RectTransform _propertiesRoot;
		private readonly SidePanel _parametersPanel;
		private readonly SidePanel _propertiesPanel;
		private readonly Vector2 _itemsNormalPosition;
		private readonly Vector2 _parametersNormalPosition;
		private readonly Vector2 _propertiesNormalPosition;
		private readonly List<Recipe> _displayedRecipes = new List<Recipe>();
		private readonly List<RecipeCard> _recipeCards = new List<RecipeCard>();

		private GameObject _drawer;
		private ScrollRect _recipeScroll;
		private RectTransform _recipeContent;
		private GameObject _shopButtonsContainer;
		private Transform _tryOriginalParent;
		private int _tryOriginalSiblingIndex;
		private GameObject _propertyControls;
		private LabelButton _forgeOpenButton;
		private LabelButton _applyButton;
		private LabelButton _closeButton;
		private ItemInfo _selectedInfo;
		private UserItem _selectedItem;
		private Recipe _selectedRecipe;
		private bool _isOpen;
		private bool _ignoreScrollSelection;
		private float _nextRefresh;
		private bool _layoutOffsetApplied;
		private ForgeUiDriver _driver;

		public bool IsOpen => _isOpen;

		public ShopForgeController(
			ShopScene shop,
			MainMenu mainMenu,
			LabelButton buttonTemplate,
			Transform uiParent,
			PropertiesPanelContent propertiesContent,
			RectTransform itemsRoot,
			RectTransform parametersRoot,
			RectTransform propertiesRoot)
		{
			_shop = shop;
			_mainMenu = mainMenu;
			_buttonTemplate = buttonTemplate;
			_uiParent = uiParent;
			_propertiesContent = propertiesContent;
			_itemsRoot = itemsRoot;
			_parametersRoot = parametersRoot;
			_propertiesRoot = propertiesRoot;
			_parametersPanel = _parametersRoot != null ? _parametersRoot.GetComponent<SidePanel>() : null;
			_propertiesPanel = _propertiesRoot != null ? _propertiesRoot.GetComponent<SidePanel>() : null;
			_itemsNormalPosition = _itemsRoot != null ? _itemsRoot.anchoredPosition : Vector2.zero;
			_parametersNormalPosition = _parametersRoot != null ? _parametersRoot.anchoredPosition : Vector2.zero;
			_propertiesNormalPosition = _propertiesRoot != null ? _propertiesRoot.anchoredPosition : Vector2.zero;
			CreateUi();
		}

		public void OnItemSelected(ItemInfo itemInfo)
		{
			_selectedInfo = itemInfo;
			ResolveSelectedItem();
			FinishPendingEnchantment();
			RefreshPropertyControls();

			if (_isOpen)
			{
				if (!CanOpen()) Close();
				else RefreshRecipes(true);
			}
			else if (ForgeOpenRequest.TryConsume(out string recipeName))
			{
				Open(recipeName);
			}
		}

		public bool CanOpen()
		{
			Roster roster = ListSF.CCDKHLAMKKO();
			if (roster == null || !roster.IIEHAMOGEHM || _selectedItem == null || _selectedItem.OFOPFCJNEBL() <= 0)
				return false;
			if (_selectedItem.PHDBCIHJKON() != null) return false;
			ItemInfo info = CurrentInfo(_selectedItem);
			return info != null && ForgeManager.ELEBLBJKDBI().IsAvailableRecipesForItemType(info.Type);
		}

		public bool Open(string recipeName = null)
		{
			ResolveSelectedItem();
			FinishPendingEnchantment();
			if (!CanOpen())
			{
				if (!string.IsNullOrEmpty(recipeName)) ForgeOpenRequest.Queue(recipeName);
				return false;
			}

			_isOpen = true;
			SetShopForgeOffset(true);
			_drawer.SetActive(true);
			_buttonTemplate.gameObject.SetActive(false);
			// The forge action buttons live in the properties side panel, so forge mode
			// slides that panel open and the parameters panel away, exactly like the
			// 1.0.6 shop does when ShopMode switches to Forge.
			_parametersPanel?.SetOpen(false, SidePanelMoveDuration);
			_propertiesPanel?.SetOpen(true, SidePanelMoveDuration);
			_mainMenu?.SetForgeViewMode(true);
			RefreshRecipes(true, recipeName);
			RefreshPropertyControls();
			return true;
		}

		public void Close()
		{
			if (!_isOpen) return;
			_isOpen = false;
			if (_drawer != null) _drawer.SetActive(false);
			SetShopForgeOffset(false);
			_propertiesPanel?.SetOpen(false, SidePanelMoveDuration);
			_mainMenu?.SetNormalViewMode(false);
			_shop.RestoreForgeClosedState();
			RefreshPropertyControls();
		}

		public void Tick()
		{
			if (Time.unscaledTime < _nextRefresh) return;
			_nextRefresh = Time.unscaledTime + RefreshInterval;

			ResolveSelectedItem();
			FinishPendingEnchantment();
			RefreshPropertyControls();
			if (_isOpen) RefreshRecipeCardState();
		}

		public void Shutdown()
		{
			SetShopForgeOffset(false);
			if (_isOpen) _mainMenu?.SetNormalViewMode(false);
			if (_driver != null) UnityEngine.Object.Destroy(_driver);
			if (_shopButtonsContainer != null)
			{
				if (_buttonTemplate != null && _tryOriginalParent != null)
				{
					_buttonTemplate.transform.SetParent(_tryOriginalParent, false);
					_buttonTemplate.transform.SetSiblingIndex(_tryOriginalSiblingIndex);
				}
				UnityEngine.Object.Destroy(_shopButtonsContainer);
			}
			else if (_forgeOpenButton != null) UnityEngine.Object.Destroy(_forgeOpenButton.gameObject);
			if (_drawer != null) UnityEngine.Object.Destroy(_drawer);
			if (_propertyControls != null) UnityEngine.Object.Destroy(_propertyControls);
			_recipeCards.Clear();
			_displayedRecipes.Clear();
			_isOpen = false;
		}

		private void CreateUi()
		{
			if (_shop == null || _buttonTemplate == null || _uiParent == null) return;
			CreateShopForgeButton();
			CreatePropertyControls();
			CreateRecipeDrawer();

			_driver = _shop.gameObject.AddComponent<ForgeUiDriver>();
			_driver.Controller = this;
			_drawer.SetActive(false);
			RefreshPropertyControls();
		}

		private void CreateShopForgeButton()
		{
			// 1.0.6 Shop.unity: TryItemButton and ForgeButton are siblings under a
			// 600x265 ButtonsContainer with a VerticalLayoutGroup (12px spacing,
			// middle-center alignment). The recovered older scene predates that
			// container and leaves TryItemButton directly under ShopUIGroup.
			_tryOriginalParent = _buttonTemplate.transform.parent != null ? _buttonTemplate.transform.parent : _uiParent;
			_tryOriginalSiblingIndex = _buttonTemplate.transform.GetSiblingIndex();
			RectTransform sourceRect = _buttonTemplate.GetComponent<RectTransform>();

			_shopButtonsContainer = new GameObject("ButtonsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
			_shopButtonsContainer.layer = _tryOriginalParent.gameObject.layer;
			_shopButtonsContainer.transform.SetParent(_tryOriginalParent, false);
			_shopButtonsContainer.transform.SetSiblingIndex(_tryOriginalSiblingIndex);
			RectTransform containerRect = _shopButtonsContainer.GetComponent<RectTransform>();
			containerRect.anchorMin = containerRect.anchorMax = new Vector2(0.5f, 0.5f);
			containerRect.pivot = new Vector2(0.5f, 0.5f);
			containerRect.anchoredPosition = sourceRect != null ? sourceRect.anchoredPosition : new Vector2(-710f, 325f);
			containerRect.sizeDelta = new Vector2(600f, 265f);

			VerticalLayoutGroup layout = _shopButtonsContainer.GetComponent<VerticalLayoutGroup>();
			layout.padding = new RectOffset(0, 0, 0, 0);
			layout.childAlignment = TextAnchor.MiddleCenter;
			layout.spacing = 12f;
			layout.childForceExpandWidth = true;
			layout.childForceExpandHeight = true;
			layout.childControlWidth = false;
			layout.childControlHeight = false;

			_buttonTemplate.transform.SetParent(_shopButtonsContainer.transform, false);
			RectTransform tryRect = _buttonTemplate.GetComponent<RectTransform>();
			if (tryRect != null) tryRect.sizeDelta = new Vector2(486f, 112f);

			_forgeOpenButton = UnityEngine.Object.Instantiate(_buttonTemplate, _shopButtonsContainer.transform, false);
			_forgeOpenButton.name = "ForgeButton";
			_forgeOpenButton.gameObject.layer = _shopButtonsContainer.layer;
			_forgeOpenButton.onClick.RemoveAllListeners();
			_forgeOpenButton.SetColor(LabelButton.FBMGEHJPPIK.BUTTON_GREEN);
			_forgeOpenButton.SetAlias("btnEnchantment");
			_forgeOpenButton.onClick.AddListener(() => Open());
			RectTransform forgeRect = _forgeOpenButton.GetComponent<RectTransform>();
			if (forgeRect != null) forgeRect.sizeDelta = new Vector2(486f, 112f);
		}

		private void CreatePropertyControls()
		{
			// 1.0.6 PropertiesPanelContent.prefab owns its forge action controls in a
			// final ButtonsPanel: LayoutElement.minHeight 242, VerticalLayoutGroup with
			// 15px side/bottom padding, 15px spacing and lower-center alignment.
			Transform parent = _propertiesContent != null ? _propertiesContent.transform : _propertiesRoot;
			if (parent == null) parent = _uiParent;
			ConfigurePropertyContentLayout(parent);
			_propertyControls = new GameObject("ForgeButtonsPanel", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement), typeof(CanvasGroup), typeof(Canvas), typeof(GraphicRaycaster));
			_propertyControls.layer = parent.gameObject.layer;
			_propertyControls.transform.SetParent(parent, false);
			_propertyControls.transform.SetAsLastSibling();

			// Keep an explicit group for the actions while still respecting the shop and
			// side-panel CanvasGroups (modal/closed panels must continue to block input).
			CanvasGroup controlsGroup = _propertyControls.GetComponent<CanvasGroup>();
			controlsGroup.alpha = 1f;
			controlsGroup.interactable = true;
			controlsGroup.blocksRaycasts = true;

			// The shop item scroller is a later sibling of SidePanels and otherwise wins
			// both drawing and raycast priority where their rects overlap. Give only the
			// forge actions a nested canvas/raycaster; raising the whole SidePanels group
			// also raises ItemInfoPanel and changes the authored panel stacking.
			Canvas controlsCanvas = _propertyControls.GetComponent<Canvas>();
			Canvas parentCanvas = parent.GetComponentInParent<Canvas>();
			controlsCanvas.overrideSorting = true;
			if (parentCanvas != null)
			{
				controlsCanvas.sortingLayerID = parentCanvas.sortingLayerID;
				controlsCanvas.sortingOrder = parentCanvas.sortingOrder + 1;
			}

			LayoutElement panelLayout = _propertyControls.GetComponent<LayoutElement>();
			panelLayout.minHeight = 242f;
			VerticalLayoutGroup layout = _propertyControls.GetComponent<VerticalLayoutGroup>();
			layout.padding = new RectOffset(15, 15, 0, 15);
			layout.childAlignment = TextAnchor.LowerCenter;
			layout.spacing = 15f;
			layout.childControlWidth = true;
			layout.childControlHeight = true;
			layout.childForceExpandWidth = true;
			layout.childForceExpandHeight = false;

			_applyButton = CloneLayoutButton("EnchantApplyButton", _propertyControls.transform, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, "btnApply");
			_applyButton.onClick.AddListener(StartEnchant);

			_closeButton = CloneLayoutButton("ForgeCloseButton", _propertyControls.transform, LabelButton.FBMGEHJPPIK.BUTTON_DARK, "Settings_Back");
			_closeButton.onClick.AddListener(Close);
		}

		private void ConfigurePropertyContentLayout(Transform parent)
		{
			// The recovered prefab is older than forge support: its three rows use
			// 120/400/600px minimums plus 50px top padding. The intact 1.0.6 prefab
			// uses 100/200/200px with no padding, leaving room for ButtonsPanel as a
			// normal final row instead of making it overlap the perks or panel edge.
			VerticalLayoutGroup contentLayout = parent.GetComponent<VerticalLayoutGroup>();
			if (contentLayout == null) return;

			contentLayout.padding = new RectOffset(0, 0, 0, 0);
			contentLayout.childAlignment = TextAnchor.UpperCenter;
			contentLayout.spacing = 0f;
			contentLayout.childControlWidth = true;
			contentLayout.childControlHeight = true;
			contentLayout.childForceExpandWidth = true;
			contentLayout.childForceExpandHeight = true;

			SetLayoutMinHeight(parent.Find("Header"), 100f);
			SetLayoutMinHeight(parent.Find("NotExistText"), 200f);
			Transform properties = parent.Find("PropertiesPanel");
			SetLayoutMinHeight(properties, 200f);

			VerticalLayoutGroup perksLayout = properties != null ? properties.GetComponent<VerticalLayoutGroup>() : null;
			if (perksLayout != null)
				perksLayout.padding = new RectOffset(-50, 0, 0, 0);
		}

		private static void SetLayoutMinHeight(Transform target, float height)
		{
			if (target == null) return;
			LayoutElement element = target.GetComponent<LayoutElement>();
			if (element != null) element.minHeight = height;
		}

		private void CreateRecipeDrawer()
		{
			// Shop.unity > ShopUIGroup > ForgePanel: 670x902, anchored to the left edge,
			// pivot (0, 0.5), y +42. The panel art is the standard paper scroll used by
			// the recovered shop item list, not a flat info-panel sprite.
			_drawer = new GameObject("ForgePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			_drawer.layer = _uiParent.gameObject.layer;
			_drawer.transform.SetParent(_uiParent, false);
			_drawer.transform.SetAsLastSibling();
			RectTransform drawerRect = _drawer.GetComponent<RectTransform>();
			drawerRect.anchorMin = drawerRect.anchorMax = new Vector2(0f, 0.5f);
			drawerRect.pivot = new Vector2(0f, 0.5f);
			drawerRect.anchoredPosition = new Vector2(0f, PanelY);
			drawerRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
			Image blocker = _drawer.GetComponent<Image>();
			blocker.color = new Color(0f, 0f, 0f, 0.001f);
			blocker.raycastTarget = true;

			GameObject renderRoot = CreateStretched("RenderRoot", _drawer.transform, Vector2.zero, Vector2.zero);

			// Scroll (paper body): panel rect inset by 18x94.
			GameObject scroll = CreateStretched("Scroll", renderRoot.transform, Vector2.zero, new Vector2(-18f, -94f));
			ResolutionImage scrollCenter = CreateSprite("ScrollCenter", scroll.transform, "CommonScrolls.Roll_MAP", Vector2.zero, false);
			Stretch(scrollCenter.rectTransform, Vector2.zero, new Vector2(-40f, 0f));
			ResolutionImage scrollLeft = CreateSprite("ScrollLeft", scroll.transform, "CommonScrolls.Paper_left", Vector2.zero, false);
			scrollLeft.rectTransform.anchorMin = new Vector2(0f, 0f);
			scrollLeft.rectTransform.anchorMax = new Vector2(0f, 1f);
			scrollLeft.rectTransform.pivot = new Vector2(0f, 0.5f);
			scrollLeft.rectTransform.anchoredPosition = Vector2.zero;
			scrollLeft.rectTransform.sizeDelta = new Vector2(40f, 0f);
			ResolutionImage scrollRight = CreateSprite("ScrollRight", scroll.transform, "CommonScrolls.Paper_right", Vector2.zero, false);
			scrollRight.rectTransform.anchorMin = new Vector2(1f, 0f);
			scrollRight.rectTransform.anchorMax = new Vector2(1f, 1f);
			scrollRight.rectTransform.pivot = new Vector2(1f, 0.5f);
			scrollRight.rectTransform.anchoredPosition = new Vector2(17f, 0f);
			scrollRight.rectTransform.sizeDelta = new Vector2(57f, 0f);

			// TableView: panel rect inset by 104x144 -> exactly one 566x758 recipe cell.
			GameObject viewport = new GameObject("TableView", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
			viewport.layer = _drawer.layer;
			viewport.transform.SetParent(renderRoot.transform, false);
			RectTransform viewportRect = viewport.GetComponent<RectTransform>();
			Stretch(viewportRect, Vector2.zero, new Vector2(-104f, -144f));
			Image hitSurface = viewport.GetComponent<Image>();
			hitSurface.color = new Color(0f, 0f, 0f, 0.001f);

			GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
			content.layer = _drawer.layer;
			content.transform.SetParent(viewport.transform, false);
			_recipeContent = content.GetComponent<RectTransform>();
			_recipeContent.anchorMin = new Vector2(0.5f, 1f);
			_recipeContent.anchorMax = new Vector2(0.5f, 1f);
			_recipeContent.pivot = new Vector2(0.5f, 1f);
			_recipeContent.anchoredPosition = Vector2.zero;
			_recipeContent.sizeDelta = new Vector2(RecipeCellWidth, RecipeCellHeight);
			VerticalLayoutGroup vertical = content.GetComponent<VerticalLayoutGroup>();
			vertical.spacing = 0f;
			vertical.childAlignment = TextAnchor.UpperCenter;
			vertical.childControlWidth = true;
			vertical.childControlHeight = true;
			vertical.childForceExpandWidth = false;
			vertical.childForceExpandHeight = false;
			ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
			fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			_recipeScroll = viewport.GetComponent<ScrollRect>();
			_recipeScroll.viewport = viewportRect;
			_recipeScroll.content = _recipeContent;
			_recipeScroll.horizontal = false;
			_recipeScroll.vertical = true;
			_recipeScroll.movementType = ScrollRect.MovementType.Elastic;
			_recipeScroll.elasticity = 0.1f;
			_recipeScroll.inertia = true;
			_recipeScroll.decelerationRate = 0.135f;
			_recipeScroll.scrollSensitivity = 35f;
			_recipeScroll.onValueChanged.AddListener(OnRecipeScrollChanged);

			// Shadow and rolls are drawn over the paper, exactly as in ForgePanel.
			ResolutionImage rollShadow = CreateSprite("RollShadow", renderRoot.transform, "Roll_Shadow", Vector2.zero, false, "UI/Textures/");
			Stretch(rollShadow.rectTransform, Vector2.zero, new Vector2(-22f, 0f));
			rollShadow.raycastTarget = false;

			CreateRoll("RollTop", renderRoot.transform, true);
			CreateRoll("RollBottom", renderRoot.transform, false);
		}

		private void CreateRoll(string name, Transform parent, bool top)
		{
			GameObject roll = new GameObject(name, typeof(RectTransform));
			roll.layer = parent.gameObject.layer;
			roll.transform.SetParent(parent, false);
			RectTransform rect = roll.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(0f, top ? 1f : 0f);
			rect.anchorMax = new Vector2(1f, top ? 1f : 0f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.anchoredPosition = new Vector2(0f, top ? 0f : 72f);
			rect.sizeDelta = new Vector2(0f, 72f);

			ResolutionImage center = CreateSprite("RollCenter", roll.transform, "CommonScrolls.Roll_center", Vector2.zero, false);
			Stretch(center.rectTransform, Vector2.zero, new Vector2(-180f, 0f));

			ResolutionImage left = CreateSprite("RollLeft", roll.transform, "CommonScrolls.Roll_left", Vector2.zero, false);
			left.rectTransform.anchorMin = new Vector2(0f, 0f);
			left.rectTransform.anchorMax = new Vector2(0f, 1f);
			left.rectTransform.pivot = new Vector2(0f, 0.5f);
			left.rectTransform.anchoredPosition = Vector2.zero;
			left.rectTransform.sizeDelta = new Vector2(90f, 0f);

			ResolutionImage right = CreateSprite("RollRight", roll.transform, "CommonScrolls.Roll_left", Vector2.zero, false);
			right.rectTransform.anchorMin = new Vector2(1f, 0f);
			right.rectTransform.anchorMax = new Vector2(1f, 1f);
			right.rectTransform.pivot = new Vector2(0f, 0.5f);
			right.rectTransform.anchoredPosition = Vector2.zero;
			right.rectTransform.sizeDelta = new Vector2(90f, 0f);
			// The reference scene mirrors the left roll cap instead of shipping a
			// separate right-hand sprite.
			right.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
		}

		private void RefreshRecipes(bool rebuild, string requestedRecipe = null)
		{
			ResolveSelectedItem();
			if (_selectedItem == null) return;
			ItemInfo currentInfo = CurrentInfo(_selectedItem);
			if (currentInfo == null) return;

			if (rebuild || _displayedRecipes.Count == 0)
			{
				_displayedRecipes.Clear();
				IReadOnlyList<Recipe> recipes = ForgeManager.ELEBLBJKDBI().Recipes;
				for (int i = 0; i < recipes.Count; i++)
				{
					Recipe recipe = recipes[i];
					if (recipe.IsRecipeAvailableForItemType(currentInfo.Type) && recipe.GetPriceByItem(_selectedItem) != null)
						_displayedRecipes.Add(recipe);
				}
				BuildRecipeCards();
			}

			int selectedIndex = 0;
			if (!string.IsNullOrEmpty(requestedRecipe))
			{
				for (int i = 0; i < _displayedRecipes.Count; i++)
				{
					if (string.Equals(_displayedRecipes[i].Name, requestedRecipe, StringComparison.OrdinalIgnoreCase))
					{
						selectedIndex = i;
						break;
					}
				}
			}
			else if (_selectedRecipe != null)
			{
				int existing = _displayedRecipes.IndexOf(_selectedRecipe);
				if (existing >= 0) selectedIndex = existing;
			}

			SelectRecipe(selectedIndex, true);
			RefreshRecipeCardState();
		}

		private void BuildRecipeCards()
		{
			for (int i = _recipeContent.childCount - 1; i >= 0; i--)
			{
				Transform child = _recipeContent.GetChild(i);
				child.SetParent(null, false);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			_recipeCards.Clear();

			for (int i = 0; i < _displayedRecipes.Count; i++)
			{
				int index = i;
				RecipeCard card = CreateRecipeCard(_displayedRecipes[i]);
				card.Button.onClick.AddListener(() => SelectRecipe(index, true));
				_recipeCards.Add(card);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(_recipeContent);
		}

		private RecipeCard CreateRecipeCard(Recipe recipe)
		{
			// RecipeUI.prefab
			GameObject root = new GameObject("RecipeUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(CanvasGroup), typeof(LayoutElement));
			root.layer = _drawer.layer;
			root.transform.SetParent(_recipeContent, false);
			RectTransform rect = root.GetComponent<RectTransform>();
			rect.sizeDelta = new Vector2(RecipeCellWidth, RecipeCellHeight);
			LayoutElement layout = root.GetComponent<LayoutElement>();
			layout.minWidth = layout.preferredWidth = RecipeCellWidth;
			layout.minHeight = layout.preferredHeight = RecipeCellHeight;
			Image raycast = root.GetComponent<Image>();
			raycast.color = new Color(0f, 0f, 0f, 0.001f);
			Button button = root.GetComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = raycast;
			CanvasGroup canvas = root.GetComponent<CanvasGroup>();

			RecipeCard card = new RecipeCard(recipe, root, button, canvas);

			// RecipeNameLabel: top stretch, y -115, height 105, font 105.
			Text name = CreateText("RecipeNameLabel", root.transform, new Vector2(0f, -115f), new Vector2(0f, 105f), 105, TextAnchor.MiddleCenter, DarkText);
			StretchHorizontally(name.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -115f), 105f);
			name.text = LocalizationManager.GetString(recipe.Alias);

			card.PropertiesRoot = CreateRecipeProperties(root.transform, recipe, card);
			card.ComplexLockedLabel = CreateCardLabel(root.transform, "RecipeComplexLockedLabel", "forgeRecipeComplexLocked", 66f, 270f, 50);
			card.WarningLabel = CreateCardLabel(root.transform, "EnchantWarningLabel", "enchantWarning", 0f, 170f, 90);
			card.PriceRoot = CreateRecipePrice(root.transform, recipe, card);
			card.FreeRoot = CreateFreePrice(root.transform);
			return card;
		}

		private GameObject CreateCardLabel(Transform parent, string name, string alias, float y, float height, int fontSize)
		{
			Text label = CreateText(name, parent, new Vector2(0f, y), new Vector2(0f, height), fontSize, TextAnchor.MiddleCenter, DarkText);
			StretchHorizontally(label.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, y), height);
			label.text = LocalizationManager.GetString(alias);
			label.gameObject.SetActive(false);
			return label.gameObject;
		}

		private GameObject CreateRecipeProperties(Transform parent, Recipe recipe, RecipeCard card)
		{
			// RecipeUI.prefab > RecipePropertiesUI: middle stretch band, y +66, height 290,
			// VerticalLayoutGroup (upper-left, no spacing, controls child size).
			GameObject area = new GameObject("RecipePropertiesUI", typeof(RectTransform), typeof(VerticalLayoutGroup));
			area.layer = parent.gameObject.layer;
			area.transform.SetParent(parent, false);
			RectTransform areaRect = area.GetComponent<RectTransform>();
			StretchHorizontally(areaRect, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, 66f), 290f);
			VerticalLayoutGroup vertical = area.GetComponent<VerticalLayoutGroup>();
			vertical.childAlignment = TextAnchor.UpperLeft;
			vertical.spacing = 0f;
			vertical.childControlWidth = true;
			vertical.childControlHeight = true;
			vertical.childForceExpandWidth = true;
			vertical.childForceExpandHeight = false;

			int rows = Mathf.Max(1, recipe.GetRequiredEnchantmentsByItem(_selectedItem));
			for (int i = 0; i < rows; i++) card.Properties.Add(CreateRecipeProperty(area.transform));
			return area;
		}

		private RecipeProperty CreateRecipeProperty(Transform parent)
		{
			// RecipePropertyUI.prefab: 566x135 row, HorizontalLayoutGroup
			// (padding 9/95, spacing 10, middle-center).
			GameObject row = new GameObject("RecipePropertyUI", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
			row.layer = parent.gameObject.layer;
			row.transform.SetParent(parent, false);
			LayoutElement rowLayout = row.GetComponent<LayoutElement>();
			rowLayout.minWidth = 536f;
			rowLayout.minHeight = 135f;
			rowLayout.preferredWidth = RecipeCellWidth;
			rowLayout.preferredHeight = 135f;
			HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
			horizontal.padding = new RectOffset(9, 95, 0, 0);
			horizontal.childAlignment = TextAnchor.MiddleCenter;
			horizontal.spacing = 10f;
			horizontal.childControlWidth = true;
			horizontal.childControlHeight = true;
			horizontal.childForceExpandWidth = false;
			horizontal.childForceExpandHeight = false;

			ResolutionImage icon = CreateSprite("Icon", row.transform, "MiscSprites.random", new Vector2(109f, 95f), true);
			LayoutElement iconLayout = icon.gameObject.AddComponent<LayoutElement>();
			iconLayout.minWidth = 109f;
			iconLayout.minHeight = 95f;

			GameObject column = new GameObject("VerticalLayout", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
			column.layer = row.layer;
			column.transform.SetParent(row.transform, false);
			LayoutElement columnLayout = column.GetComponent<LayoutElement>();
			columnLayout.preferredHeight = 100f;
			VerticalLayoutGroup columnGroup = column.GetComponent<VerticalLayoutGroup>();
			columnGroup.padding = new RectOffset(0, 0, 0, 5);
			columnGroup.childAlignment = TextAnchor.MiddleCenter;
			columnGroup.spacing = 3f;
			columnGroup.childControlWidth = true;
			columnGroup.childControlHeight = true;
			columnGroup.childForceExpandWidth = true;
			columnGroup.childForceExpandHeight = false;

			Text numbers = CreateText("Numbers", column.transform, Vector2.zero, new Vector2(343f, 60f), 42, TextAnchor.MiddleCenter, DarkText);
			LayoutElement numbersLayout = numbers.gameObject.AddComponent<LayoutElement>();
			numbersLayout.minWidth = 343f;
			numbersLayout.minHeight = 60f;

			GameObject bar = new GameObject("ProgressBar", typeof(RectTransform), typeof(LayoutElement));
			bar.layer = row.layer;
			bar.transform.SetParent(column.transform, false);
			LayoutElement barLayout = bar.GetComponent<LayoutElement>();
			barLayout.minWidth = 343f;
			barLayout.minHeight = 35f;

			ResolutionImage background = CreateSprite("Background", bar.transform, "ParametersBar.bar_0", Vector2.zero, false);
			Stretch(background.rectTransform, Vector2.zero, Vector2.zero);
			background.type = Image.Type.Filled;
			background.fillMethod = Image.FillMethod.Horizontal;
			background.fillOrigin = 0;
			background.fillAmount = 1f;

			ResolutionImage stripe = CreateSprite("StripeOrange", bar.transform, "ParametersBar.bar_2", Vector2.zero, false);
			Stretch(stripe.rectTransform, Vector2.zero, Vector2.zero);
			stripe.type = Image.Type.Filled;
			stripe.fillMethod = Image.FillMethod.Horizontal;
			stripe.fillOrigin = 0;
			stripe.fillAmount = 0f;

			return new RecipeProperty(row, numbers, stripe);
		}

		private GameObject CreateRecipePrice(Transform parent, Recipe recipe, RecipeCard card)
		{
			// RecipeUI.prefab > RecipePriceUI: bottom stretch band, y +170, height 262.
			GameObject priceRoot = new GameObject("RecipePriceUI", typeof(RectTransform));
			priceRoot.layer = parent.gameObject.layer;
			priceRoot.transform.SetParent(parent, false);
			RectTransform rect = priceRoot.GetComponent<RectTransform>();
			StretchHorizontally(rect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 170f), 262f);

			Text costLabel = CreateText("ReceiptPriceLabel", priceRoot.transform, new Vector2(0f, 213f), new Vector2(-240f, 95f), 75, TextAnchor.MiddleCenter, DarkText);
			StretchHorizontally(costLabel.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 213f), 95f);
			costLabel.rectTransform.sizeDelta = new Vector2(-240f, 95f);
			costLabel.text = LocalizationManager.GetString("forgeCurrenciesCost");

			GameObject materials = new GameObject("Materials", typeof(RectTransform), typeof(HorizontalLayoutGroup));
			materials.layer = priceRoot.layer;
			materials.transform.SetParent(priceRoot.transform, false);
			RectTransform materialsRect = materials.GetComponent<RectTransform>();
			StretchHorizontally(materialsRect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 85f), 170f);
			HorizontalLayoutGroup row = materials.GetComponent<HorizontalLayoutGroup>();
			row.childAlignment = TextAnchor.UpperCenter;
			row.spacing = 44f;
			row.childControlWidth = true;
			row.childControlHeight = true;
			row.childForceExpandWidth = false;
			row.childForceExpandHeight = true;

			RecipePrice price = recipe.GetPriceByItem(_selectedItem);
			if (price == null) return priceRoot;

			for (int i = 0; i < price.Materials.Count; i++)
			{
				CurrencyStruct material = price.Materials[i];
				if (material?.BKDEAGGPNAO == null) continue;
				card.Materials.Add(CreateMaterial(materials.transform, material));
			}
			return priceRoot;
		}

		private MaterialEntry CreateMaterial(Transform parent, CurrencyStruct material)
		{
			// MaterialUI.prefab: 164x170 cell, icon band 94 tall at y -58, price 78.8 tall.
			GameObject materialUi = new GameObject("MaterialUI", typeof(RectTransform), typeof(LayoutElement));
			materialUi.layer = parent.gameObject.layer;
			materialUi.transform.SetParent(parent, false);
			LayoutElement materialLayout = materialUi.GetComponent<LayoutElement>();
			materialLayout.preferredWidth = 164f;
			materialLayout.preferredHeight = 170f;

			ResolutionImage icon = CreateSprite("Icon", materialUi.transform, material.BKDEAGGPNAO.MJBPMLCLMFN, Vector2.zero, true);
			RectTransform iconRect = icon.rectTransform;
			iconRect.anchorMin = new Vector2(0f, 1f);
			iconRect.anchorMax = new Vector2(1f, 1f);
			iconRect.pivot = new Vector2(0.5f, 0.5f);
			iconRect.anchoredPosition = new Vector2(-2.5f, -58f);
			iconRect.sizeDelta = new Vector2(-70f, 94f);

			Text value = CreateText("Price", materialUi.transform, new Vector2(0f, 39.4f), new Vector2(0f, 78.8f), 56, TextAnchor.MiddleCenter, DarkText);
			StretchHorizontally(value.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 39.4f), 78.8f);
			value.text = ((int)material.Count).ToString();
			return new MaterialEntry(material, value);
		}

		private GameObject CreateFreePrice(Transform parent)
		{
			// RecipeUI.prefab > FreeRecipeUI (inactive by default).
			GameObject freeRoot = new GameObject("FreeRecipeUI", typeof(RectTransform));
			freeRoot.layer = parent.gameObject.layer;
			freeRoot.transform.SetParent(parent, false);
			StretchHorizontally(freeRoot.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 123f), 170f);

			ResolutionImage stripe = CreateSprite("Stripe", freeRoot.transform, "ShopPieces.Stripe2", new Vector2(412f, 88f), false);
			stripe.rectTransform.anchoredPosition = new Vector2(0f, -14f);
			Text free = CreateText("Text", freeRoot.transform, new Vector2(0f, -19f), new Vector2(412f, 88f), 47, TextAnchor.MiddleCenter, FreeText);
			free.rectTransform.anchorMin = free.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			free.rectTransform.pivot = new Vector2(0.5f, 0.5f);
			free.rectTransform.anchoredPosition = new Vector2(0f, -19f);
			free.text = LocalizationManager.GetString("free");
			freeRoot.SetActive(false);
			return freeRoot;
		}

		private void SelectRecipe(int index, bool scrollIntoView)
		{
			if (index < 0 || index >= _displayedRecipes.Count) return;
			_selectedRecipe = _displayedRecipes[index];
			for (int i = 0; i < _recipeCards.Count; i++)
				_recipeCards[i].Canvas.alpha = i == index ? 1f : 0.5f;

			if (scrollIntoView && _displayedRecipes.Count > 1)
			{
				_ignoreScrollSelection = true;
				_recipeScroll.verticalNormalizedPosition = 1f - index / (float)(_displayedRecipes.Count - 1);
				_ignoreScrollSelection = false;
			}
			RefreshPropertyControls();
		}

		private void OnRecipeScrollChanged(Vector2 position)
		{
			if (_ignoreScrollSelection || _displayedRecipes.Count <= 1) return;
			int index = Mathf.Clamp(Mathf.RoundToInt((1f - position.y) * (_displayedRecipes.Count - 1)), 0, _displayedRecipes.Count - 1);
			if (_selectedRecipe != _displayedRecipes[index]) SelectRecipe(index, false);
		}

		private void RefreshRecipeCardState()
		{
			ItemInfo info = CurrentInfo(_selectedItem);
			int itemLevel = info != null ? info.MHGODOLNDLE : 1;
			Roster roster = ListSF.CCDKHLAMKKO();

			for (int i = 0; i < _recipeCards.Count; i++)
			{
				RecipeCard card = _recipeCards[i];
				Recipe recipe = card.Recipe;
				// Shop.unity: ForgePanel._ActiveTableViewCellOpacity 1 /
				// _InactiveTableViewCellOpacity 0.5. Availability is communicated by the
				// locked/warning labels, not by dimming the cell further.
				card.Button.interactable = _selectedItem != null;
				card.Canvas.alpha = recipe == _selectedRecipe ? 1f : 0.5f;

				// RecipeUI shows exactly one of: the property rows, the locked-recipe
				// hint, or the duplicate-enchantment warning.
				int possible = _selectedItem == null ? 0 : recipe.GetPossibleEnchantments(_selectedItem, itemLevel, false).Count;
				int ready = _selectedItem == null ? 0 : recipe.GetPossibleEnchantments(_selectedItem, itemLevel, true).Count;
				int required = _selectedItem == null ? 0 : recipe.GetRequiredEnchantmentsByItem(_selectedItem);
				bool locked = possible <= 0;
				bool duplicate = !locked && ready < required;

				card.PropertiesRoot.SetActive(!locked && !duplicate);
				card.ComplexLockedLabel.SetActive(locked);
				card.WarningLabel.SetActive(duplicate);

				if (!locked && !duplicate) RefreshRecipeProperties(card, recipe, itemLevel);

				bool free = recipe.IsFree;
				card.PriceRoot.SetActive(!free);
				card.FreeRoot.SetActive(free);
				if (!free)
				{
					for (int m = 0; m < card.Materials.Count; m++)
					{
						MaterialEntry entry = card.Materials[m];
						int owned = roster == null ? 0 : roster.GetCurrencyCount(entry.Material.BKDEAGGPNAO);
						entry.Value.color = owned >= entry.Material.Count ? DarkText : MissingMaterialText;
					}
				}
			}
		}

		private void RefreshRecipeProperties(RecipeCard card, Recipe recipe, int itemLevel)
		{
			RecipeItem item = recipe.GetRecipeItemByItem(_selectedItem);
			if (item == null) return;
			int baseAspect = ForgeManager.ELEBLBJKDBI().GetAspectValueByLevel(itemLevel);
			int min = baseAspect + item.MinDeviation;
			int max = baseAspect + item.MaxDeviation;
			float fill = EnchantmentBarFill(item.BarScale, baseAspect, itemLevel);

			for (int i = 0; i < card.Properties.Count; i++)
			{
				RecipeProperty property = card.Properties[i];
				property.Numbers.text = min == max ? min.ToString() : min + " - " + max;
				property.Stripe.fillAmount = fill;
			}
		}

		/// <summary>
		/// Mirrors ParameterScrollItem.GetPercentFromValue for the forge bar, using the
		/// item limits of the BarScale named by the recipe item (vanilla: "Enchantment").
		/// </summary>
		private static float EnchantmentBarFill(string barScaleName, float value, int itemLevel)
		{
			if (string.IsNullOrEmpty(barScaleName)) return 0f;
			BarScale scale = GameUtils.NPHEOMBNOLK?.HNECOCDPENN(barScaleName);
			if (scale == null) return 0f;

			Limit limit = scale.EHKJEKAIDFF(itemLevel) ?? scale.NMMHOKHKFEE();
			if (limit == null) return 0f;

			float rightLimit = limit.OBGGBMDABAD >= 0 && limit.NGPJDHKOEJC >= 0
				? limit.NGPJDHKOEJC
				: itemLevel * limit.LevelMultiplier + limit.Shift;
			if (rightLimit <= 0f) return 0f;

			float power = scale.MFGLDPKEDJB >= 0f ? scale.MFGLDPKEDJB : 0f;
			float minimum = scale.DPGMCKCDMBC >= 0f ? scale.DPGMCKCDMBC : 0f;
			float percent;
			if (!string.IsNullOrEmpty(scale.Type) && scale.Type.Equals("Linear"))
			{
				percent = Mathf.Pow(value / rightLimit, power);
			}
			else
			{
				float doublingRange = GameUtils.BGJPLNFFEOB;
				if (doublingRange <= 0f) doublingRange = 10f;
				percent = Mathf.Pow(2f, (value - rightLimit) * power / doublingRange);
			}
			return Mathf.Max(Mathf.Clamp01(percent), minimum);
		}

		private void RefreshPropertyControls()
		{
			if (_propertyControls == null) return;
			ResolveSelectedItem();
			bool canOpen = CanOpen();

			bool showControls = _isOpen;
			_propertyControls.SetActive(showControls);
			// Content mounted into the side panel at runtime can be added after this
			// block; keep the forge controls on top of it.
			if (showControls) _propertyControls.transform.SetAsLastSibling();
			bool showForgeButton = ShouldShowForgeButton();
			_forgeOpenButton.gameObject.SetActive(!_isOpen && showForgeButton);
			_forgeOpenButton.interactable = canOpen;
			_applyButton.gameObject.SetActive(_isOpen);
			_closeButton.gameObject.SetActive(_isOpen);

			if (_isOpen)
			{
				bool available = _selectedRecipe != null && _selectedItem != null && _selectedRecipe.IsRecipeAvailableForItem(_selectedItem);
				bool materials = available && _selectedRecipe.CheckMaterialsForItem(_selectedItem);
				_applyButton.interactable = available && materials;
			}
		}

		private void StartEnchant()
		{
			if (_selectedItem == null || _selectedRecipe == null) return;
			if (!ListSF.TryEnchantItem(_selectedItem, _selectedRecipe))
			{
				RefreshPropertyControls();
				return;
			}

			ResolveSelectedItem();
			RecipeItemInfo pending = _selectedItem?.PHDBCIHJKON();
			if (pending != null && !ListSF.ApplyRecipeToItem(pending))
			{
				RefreshPropertyControls();
				return;
			}

			_mainMenu?.UpdateMainMenu();
			_shop.RefreshAfterForgeMutation();
			ResolveSelectedItem();
			Close();
			RefreshPropertyControls();
		}

		private bool FinishPendingEnchantment()
		{
			RecipeItemInfo pending = _selectedItem?.PHDBCIHJKON();
			if (pending == null) return false;
			if (!ListSF.ApplyRecipeToItem(pending)) return false;

			_mainMenu?.UpdateMainMenu();
			_shop.RefreshAfterForgeMutation();
			ResolveSelectedItem();
			return true;
		}

		private void SetShopForgeOffset(bool forgeMode)
		{
			if (_layoutOffsetApplied == forgeMode) return;
			Vector2 forgeOffset = forgeMode ? new Vector2(ForgeModeOffset, 0f) : Vector2.zero;

			// 1.0.6 Shop.unity keeps ItemInfoPanel outside _PanelsContainer and only
			// shifts the container that holds ItemParametersPanel, ItemPropertiesPanel
			// and ItemsPanel. The recovered scene nests the info panel next to the two
			// side panels, so the offset is applied to the three moving rects directly
			// and the info panel stays where the shop authored it.
			if (_itemsRoot != null) _itemsRoot.anchoredPosition = _itemsNormalPosition + forgeOffset;
			if (_parametersRoot != null) _parametersRoot.anchoredPosition = _parametersNormalPosition + forgeOffset;
			if (_propertiesRoot != null) _propertiesRoot.anchoredPosition = _propertiesNormalPosition + forgeOffset;
			_layoutOffsetApplied = forgeMode;
		}

		private bool ShouldShowForgeButton()
		{
			Roster roster = ListSF.CCDKHLAMKKO();
			if (roster == null || !roster.IIEHAMOGEHM || _selectedItem == null || _selectedItem.OFOPFCJNEBL() <= 0)
				return false;
			ItemInfo info = CurrentInfo(_selectedItem);
			return info != null && ForgeManager.ELEBLBJKDBI().IsAvailableRecipesForItemType(info.Type);
		}

		private void ResolveSelectedItem()
		{
			Roster roster = ListSF.CCDKHLAMKKO();
			_selectedItem = _selectedInfo != null ? roster?.KHCNHPCPFII()?.CMGOCLGHNLH(_selectedInfo) : null;
		}

		private LabelButton CloneLayoutButton(string name, Transform parent, LabelButton.FBMGEHJPPIK color, string alias)
		{
			LabelButton button = UnityEngine.Object.Instantiate(_buttonTemplate, parent, false);
			button.name = name;
			button.gameObject.layer = parent.gameObject.layer;
			button.onClick.RemoveAllListeners();
			button.SetColor(color);
			if (!string.IsNullOrEmpty(alias)) button.SetAlias(alias);
			LayoutElement element = button.GetComponent<LayoutElement>();
			if (element == null) element = button.gameObject.AddComponent<LayoutElement>();
			element.minHeight = 112f;
			element.preferredHeight = 112f;
			button.gameObject.SetActive(true);
			return button;
		}

		private static GameObject CreateStretched(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta)
		{
			GameObject node = new GameObject(name, typeof(RectTransform));
			node.layer = parent.gameObject.layer;
			node.transform.SetParent(parent, false);
			Stretch(node.GetComponent<RectTransform>(), anchoredPosition, sizeDelta);
			return node;
		}

		private static void Stretch(RectTransform rect, Vector2 anchoredPosition, Vector2 sizeDelta)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = anchoredPosition;
			rect.sizeDelta = sizeDelta;
		}

		private static void StretchHorizontally(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, float height)
		{
			rect.anchorMin = anchorMin;
			rect.anchorMax = anchorMax;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = anchoredPosition;
			rect.sizeDelta = new Vector2(0f, height);
		}

		private static ResolutionImage CreateSprite(string name, Transform parent, string spriteName, Vector2 size, bool preserveAspect, string texturePath = "UI/Atlases/")
		{
			GameObject spriteObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(ResolutionImage));
			spriteObject.layer = parent.gameObject.layer;
			spriteObject.transform.SetParent(parent, false);
			RectTransform rect = spriteObject.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = size;
			ResolutionImage image = spriteObject.GetComponent<ResolutionImage>();
			image.raycastTarget = false;
			image.set_TexturePath(texturePath);
			image.set_SpriteName(spriteName);
			image.preserveAspect = preserveAspect;
			return image;
		}

		private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment, Color color)
		{
			GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
			textObject.layer = parent.gameObject.layer;
			textObject.transform.SetParent(parent, false);
			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = anchoredPosition;
			rect.sizeDelta = size;
			Text text = textObject.GetComponent<Text>();
			text.font = LocalizationManager.MBPJIKFOEBJ();
			text.fontSize = fontSize;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 1;
			text.resizeTextMaxSize = fontSize;
			text.alignment = alignment;
			text.color = color;
			text.raycastTarget = false;
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Truncate;
			return text;
		}

		private static ItemInfo CurrentInfo(UserItem userItem)
		{
			if (userItem == null) return null;
			return userItem.DBLCMCEGJGI(false) ?? userItem.BHKHOJPANHE();
		}

		private sealed class RecipeCard
		{
			public readonly Recipe Recipe;
			public readonly GameObject Root;
			public readonly Button Button;
			public readonly CanvasGroup Canvas;
			public readonly List<RecipeProperty> Properties = new List<RecipeProperty>();
			public readonly List<MaterialEntry> Materials = new List<MaterialEntry>();

			public GameObject PropertiesRoot;
			public GameObject ComplexLockedLabel;
			public GameObject WarningLabel;
			public GameObject PriceRoot;
			public GameObject FreeRoot;

			public RecipeCard(Recipe recipe, GameObject root, Button button, CanvasGroup canvas)
			{
				Recipe = recipe;
				Root = root;
				Button = button;
				Canvas = canvas;
			}
		}

		private sealed class RecipeProperty
		{
			public readonly GameObject Root;
			public readonly Text Numbers;
			public readonly Image Stripe;

			public RecipeProperty(GameObject root, Text numbers, Image stripe)
			{
				Root = root;
				Numbers = numbers;
				Stripe = stripe;
			}
		}

		private sealed class MaterialEntry
		{
			public readonly CurrencyStruct Material;
			public readonly Text Value;

			public MaterialEntry(CurrencyStruct material, Text value)
			{
				Material = material;
				Value = value;
			}
		}
	}

	internal sealed class ForgeUiDriver : MonoBehaviour
	{
		public ShopForgeController Controller;

		private void Update()
		{
			Controller?.Tick();
		}
	}

	/// <summary>
	/// Quest actions may request the forge before ShopScene has selected a table cell.
	/// Keep only the requested recipe identity until the next eligible item selection.
	/// </summary>
	public static class ForgeOpenRequest
	{
		private static string _recipeName;
		private static bool _pending;

		public static void Queue(string recipeName)
		{
			_recipeName = recipeName ?? string.Empty;
			_pending = true;
		}

		public static bool TryConsume(out string recipeName)
		{
			recipeName = _recipeName;
			if (!_pending) return false;
			_pending = false;
			_recipeName = string.Empty;
			return true;
		}
	}
}
