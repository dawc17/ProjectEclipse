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
	/// Projects the recovered forge lifecycle into the recovered shop using the layout
	/// and state model from the intact 1.0.6 ForgePanel/PropertiesPanelContent assets.
	/// Gameplay remains owned by ForgeManager/ListSF/UserItem.
	/// </summary>
	public sealed class ShopForgeController
	{
		private const float RefreshInterval = 0.25f;
		private const float ForgeModeOffset = 605f;
		private const float RecipeCellWidth = 566f;
		private const float RecipeCellHeight = 758f;

		private static readonly Color DarkText = new Color(0.18431373f, 0.14509805f, 0.105882354f, 1f);
		private static readonly Color MissingMaterialText = new Color(0.60f, 0.13f, 0.08f, 1f);
		private static readonly Color FreeText = new Color(1f, 0.79f, 0.17f, 1f);

		private readonly ShopScene _shop;
		private readonly MainMenu _mainMenu;
		private readonly LabelButton _buttonTemplate;
		private readonly Transform _uiParent;
		private readonly PropertiesPanelContent _propertiesContent;
		private readonly RectTransform _itemsRoot;
		private readonly RectTransform _parametersRoot;
		private readonly RectTransform _propertiesRoot;
		private readonly RectTransform _sidePanelsRoot;
		private readonly Vector2 _itemsNormalPosition;
		private readonly Vector2 _sidePanelsNormalPosition;
		private readonly List<Recipe> _displayedRecipes = new List<Recipe>();
		private readonly List<RecipeCard> _recipeCards = new List<RecipeCard>();

		private GameObject _drawer;
		private ScrollRect _recipeScroll;
		private RectTransform _recipeContent;
		private GameObject _shopButtonsContainer;
		private Transform _tryOriginalParent;
		private int _tryOriginalSiblingIndex;
		private GameObject _propertyControls;
		private GameObject _deliveryPanel;
		private Text _deliveryText;
		private LabelButton _forgeOpenButton;
		private LabelButton _applyButton;
		private LabelButton _skipButton;
		private LabelButton _closeButton;
		private ItemInfo _selectedInfo;
		private UserItem _selectedItem;
		private Recipe _selectedRecipe;
		private bool _isOpen;
		private bool _hadPendingDelivery;
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
			_sidePanelsRoot = (_parametersRoot != null ? _parametersRoot.parent : _propertiesRoot?.parent) as RectTransform;
			_itemsNormalPosition = _itemsRoot != null ? _itemsRoot.anchoredPosition : Vector2.zero;
			_sidePanelsNormalPosition = _sidePanelsRoot != null ? _sidePanelsRoot.anchoredPosition : Vector2.zero;
			CreateUi();
		}

		public void OnItemSelected(ItemInfo itemInfo)
		{
			_selectedInfo = itemInfo;
			ResolveSelectedItem();
			_hadPendingDelivery = _selectedItem != null && _selectedItem.PHDBCIHJKON() != null;
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
			if (!CanOpen())
			{
				if (!string.IsNullOrEmpty(recipeName)) ForgeOpenRequest.Queue(recipeName);
				return false;
			}

			_isOpen = true;
			SetShopForgeOffset(true);
			_drawer.SetActive(true);
			_buttonTemplate.gameObject.SetActive(false);
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
			_mainMenu?.SetNormalViewMode(false);
			_shop.RestoreForgeClosedState();
			RefreshPropertyControls();
		}

		public void Tick()
		{
			if (Time.unscaledTime < _nextRefresh) return;
			_nextRefresh = Time.unscaledTime + RefreshInterval;

			ResolveSelectedItem();
			bool pendingNow = _selectedItem != null && _selectedItem.PHDBCIHJKON() != null;
			if (_hadPendingDelivery && !pendingNow)
			{
				_hadPendingDelivery = false;
				_shop.RefreshAfterForgeMutation();
				if (_isOpen) Close();
				ResolveSelectedItem();
			}
			else
			{
				_hadPendingDelivery = pendingNow;
			}

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
			// 1.0.6 PropertiesPanelContent.prefab owns its forge action controls.
			// They live in a final 242px layout block with 15px side/bottom padding,
			// 15px vertical spacing and lower-center alignment.
			Transform parent = _propertiesContent != null ? _propertiesContent.transform : _propertiesRoot;
			if (parent == null) parent = _uiParent;
			_propertyControls = new GameObject("ForgeButtonsPanel", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
			_propertyControls.layer = parent.gameObject.layer;
			_propertyControls.transform.SetParent(parent, false);
			_propertyControls.transform.SetAsLastSibling();

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

			_deliveryPanel = CreateDeliveryPanel(_propertyControls.transform);
			_skipButton = CloneLayoutButton("SkipDeliveryButton", _propertyControls.transform, LabelButton.FBMGEHJPPIK.BUTTON_GREEN, string.Empty);
			_skipButton.onClick.AddListener(SkipDelivery);
			CreateButtonIcon(_skipButton.transform, "MiscSprites.ruby");

			_applyButton = CloneLayoutButton("EnchantApplyButton", _propertyControls.transform, LabelButton.FBMGEHJPPIK.BUTTON_WHITE, "btnApply");
			_applyButton.onClick.AddListener(StartEnchant);

			_closeButton = CloneLayoutButton("ForgeCloseButton", _propertyControls.transform, LabelButton.FBMGEHJPPIK.BUTTON_DARK, "Settings_Back");
			_closeButton.onClick.AddListener(Close);
		}

		private GameObject CreateDeliveryPanel(Transform parent)
		{
			GameObject row = new GameObject("DeliveryTimePanel", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
			row.layer = parent.gameObject.layer;
			row.transform.SetParent(parent, false);
			LayoutElement element = row.GetComponent<LayoutElement>();
			element.minHeight = 61f;
			element.preferredHeight = 61f;
			HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
			layout.childAlignment = TextAnchor.MiddleCenter;
			layout.spacing = 10f;
			layout.childControlWidth = false;
			layout.childControlHeight = false;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = false;

			ResolutionImage hourglass = CreateSprite("Hourglass", row.transform, "MiscSprites.hourglass", new Vector2(58f, 58f));
			LayoutElement iconLayout = hourglass.gameObject.AddComponent<LayoutElement>();
			iconLayout.preferredWidth = 58f;
			iconLayout.preferredHeight = 58f;
			_deliveryText = CreateText("DeliveryTimeText", row.transform, Vector2.zero, new Vector2(300f, 61f), 42, TextAnchor.MiddleLeft, DarkText);
			LayoutElement textLayout = _deliveryText.gameObject.AddComponent<LayoutElement>();
			textLayout.preferredWidth = 300f;
			textLayout.preferredHeight = 61f;
			return row;
		}

		private void CreateRecipeDrawer()
		{
			_drawer = new GameObject("EclipseForgePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(ResolutionImage));
			_drawer.layer = _uiParent.gameObject.layer;
			_drawer.transform.SetParent(_uiParent, false);
			_drawer.transform.SetAsLastSibling();
			RectTransform drawerRect = _drawer.GetComponent<RectTransform>();
			drawerRect.anchorMin = drawerRect.anchorMax = new Vector2(0f, 0.5f);
			drawerRect.pivot = new Vector2(0f, 0.5f);
			drawerRect.anchoredPosition = new Vector2(0f, 42f);
			drawerRect.sizeDelta = new Vector2(670f, 902f);
			ResolutionImage drawerBackground = _drawer.GetComponent<ResolutionImage>();
			drawerBackground.raycastTarget = true;
			drawerBackground.set_TexturePath("UI/Atlases/");
			drawerBackground.set_SpriteName("ShopPieces.Info_Panel");
			drawerBackground.type = Image.Type.Sliced;

			GameObject viewport = new GameObject("TableView", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
			viewport.layer = _drawer.layer;
			viewport.transform.SetParent(_drawer.transform, false);
			RectTransform viewportRect = viewport.GetComponent<RectTransform>();
			viewportRect.anchorMin = Vector2.zero;
			viewportRect.anchorMax = Vector2.one;
			viewportRect.offsetMin = new Vector2(52f, 72f);
			viewportRect.offsetMax = new Vector2(-52f, -72f);
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
			_recipeScroll.inertia = true;
			_recipeScroll.decelerationRate = 0.135f;
			_recipeScroll.scrollSensitivity = 35f;
			_recipeScroll.onValueChanged.AddListener(OnRecipeScrollChanged);
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
			GameObject root = new GameObject("RecipeUI_" + recipe.Name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(CanvasGroup), typeof(LayoutElement));
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
			button.targetGraphic = raycast;
			CanvasGroup canvas = root.GetComponent<CanvasGroup>();

			Text name = CreateText("RecipeNameLabel", root.transform, new Vector2(0f, -115f), new Vector2(566f, 105f), 52, TextAnchor.MiddleCenter, DarkText);
			name.text = LocalizationManager.GetString(recipe.Alias);

			CreateRecipeProperty(root.transform, recipe);
			CreateRecipePrice(root.transform, recipe);
			return new RecipeCard(recipe, root, button, canvas);
		}

		private void CreateRecipeProperty(Transform parent, Recipe recipe)
		{
			RecipeItem item = recipe.GetRecipeItemByItem(_selectedItem);
			if (item == null) return;
			int level = CurrentInfo(_selectedItem)?.MHGODOLNDLE ?? 1;
			int baseAspect = ForgeManager.ELEBLBJKDBI().GetAspectValueByLevel(level);
			int min = baseAspect + item.MinDeviation;
			int max = baseAspect + item.MaxDeviation;

			GameObject area = new GameObject("RecipePropertiesUI", typeof(RectTransform));
			area.layer = parent.gameObject.layer;
			area.transform.SetParent(parent, false);
			RectTransform areaRect = area.GetComponent<RectTransform>();
			areaRect.anchorMin = areaRect.anchorMax = new Vector2(0.5f, 0.5f);
			areaRect.pivot = new Vector2(0.5f, 0.5f);
			areaRect.anchoredPosition = new Vector2(0f, 66f);
			areaRect.sizeDelta = new Vector2(515f, 290f);

			ResolutionImage icon = CreateSprite("Icon", area.transform, "RaidMisc.random", new Vector2(74f, 67f));
			RectTransform iconRect = icon.rectTransform;
			iconRect.anchorMin = iconRect.anchorMax = new Vector2(0f, 0.5f);
			iconRect.pivot = new Vector2(0f, 0.5f);
			iconRect.anchoredPosition = new Vector2(34f, 45f);

			Text range = CreateText("Numbers", area.transform, new Vector2(75f, -20f), new Vector2(343f, 80f), 42, TextAnchor.MiddleCenter, DarkText);
			range.text = min == max ? min.ToString() : min + " - " + max;

			ResolutionImage background = CreateSprite("Background", area.transform, "ParametersBar.bar_0", new Vector2(343f, 35f));
			RectTransform bgRect = background.rectTransform;
			bgRect.anchorMin = bgRect.anchorMax = new Vector2(0.5f, 0.5f);
			bgRect.anchoredPosition = new Vector2(75f, -60f);
			ResolutionImage stripe = CreateSprite("StripeOrange", area.transform, "ParametersBar.bar_2", new Vector2(343f, 35f));
			RectTransform stripeRect = stripe.rectTransform;
			stripeRect.anchorMin = stripeRect.anchorMax = new Vector2(0.5f, 0.5f);
			stripeRect.anchoredPosition = new Vector2(75f, -60f);
		}

		private void CreateRecipePrice(Transform parent, Recipe recipe)
		{
			RecipePrice price = recipe.GetPriceByItem(_selectedItem);
			if (price == null) return;

			GameObject priceRoot = new GameObject("RecipePriceUI", typeof(RectTransform));
			priceRoot.layer = parent.gameObject.layer;
			priceRoot.transform.SetParent(parent, false);
			RectTransform rect = priceRoot.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
			rect.pivot = new Vector2(0.5f, 0f);
			rect.anchoredPosition = new Vector2(0f, 35f);
			rect.sizeDelta = new Vector2(566f, 262f);

			if (recipe.IsFree)
			{
				ResolutionImage stripe = CreateSprite("Stripe", priceRoot.transform, "ShopPieces.Stripe2", new Vector2(412f, 88f));
				stripe.rectTransform.anchorMin = stripe.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				stripe.rectTransform.anchoredPosition = new Vector2(0f, -14f);
				Text free = CreateText("Free", priceRoot.transform, new Vector2(0f, -19f), new Vector2(412f, 88f), 38, TextAnchor.MiddleCenter, FreeText);
				free.text = LocalizationManager.GetString("free").ToUpperInvariant();
				return;
			}

			Text costLabel = CreateText("ReceiptPriceLabel", priceRoot.transform, new Vector2(0f, -5f), new Vector2(326f, 70f), 34, TextAnchor.MiddleCenter, DarkText);
			costLabel.text = LocalizationManager.GetString("forgeCurrenciesCost");

			GameObject materials = new GameObject("Materials", typeof(RectTransform), typeof(HorizontalLayoutGroup));
			materials.layer = priceRoot.layer;
			materials.transform.SetParent(priceRoot.transform, false);
			RectTransform materialsRect = materials.GetComponent<RectTransform>();
			materialsRect.anchorMin = materialsRect.anchorMax = new Vector2(0.5f, 0f);
			materialsRect.pivot = new Vector2(0.5f, 0f);
			materialsRect.anchoredPosition = new Vector2(0f, 18f);
			materialsRect.sizeDelta = new Vector2(515f, 145f);
			HorizontalLayoutGroup row = materials.GetComponent<HorizontalLayoutGroup>();
			row.childAlignment = TextAnchor.MiddleCenter;
			row.spacing = 44f;
			row.childControlWidth = false;
			row.childControlHeight = false;
			row.childForceExpandWidth = false;
			row.childForceExpandHeight = false;

			Roster roster = ListSF.CCDKHLAMKKO();
			for (int i = 0; i < price.Materials.Count; i++)
			{
				CurrencyStruct material = price.Materials[i];
				if (material?.BKDEAGGPNAO == null) continue;
				GameObject materialUi = new GameObject("MaterialUI", typeof(RectTransform), typeof(LayoutElement));
				materialUi.layer = materials.layer;
				materialUi.transform.SetParent(materials.transform, false);
				LayoutElement materialLayout = materialUi.GetComponent<LayoutElement>();
				materialLayout.preferredWidth = 110f;
				materialLayout.preferredHeight = 130f;
				string iconName = material.BKDEAGGPNAO.MJBPMLCLMFN;
				ResolutionImage icon = CreateSprite("Icon", materialUi.transform, iconName, new Vector2(70f, 70f));
				icon.rectTransform.anchorMin = icon.rectTransform.anchorMax = new Vector2(0.5f, 1f);
				icon.rectTransform.anchoredPosition = new Vector2(0f, -35f);
				int owned = roster == null ? 0 : roster.GetCurrencyCount(material.BKDEAGGPNAO);
				Color priceColor = owned >= material.Count ? DarkText : MissingMaterialText;
				Text value = CreateText("Price", materialUi.transform, new Vector2(0f, -78f), new Vector2(110f, 52f), 34, TextAnchor.MiddleCenter, priceColor);
				value.text = ((int)material.Count).ToString();
			}
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
			for (int i = 0; i < _recipeCards.Count; i++)
			{
				bool available = _selectedItem != null && _recipeCards[i].Recipe.IsRecipeAvailableForItem(_selectedItem);
				_recipeCards[i].Button.interactable = available;
				if (_recipeCards[i].Recipe != _selectedRecipe)
					_recipeCards[i].Canvas.alpha = available ? 0.5f : 0.3f;
				else
					_recipeCards[i].Canvas.alpha = available ? 1f : 0.45f;
			}
		}

		private void RefreshPropertyControls()
		{
			if (_propertyControls == null) return;
			ResolveSelectedItem();
			RecipeItemInfo pending = _selectedItem?.PHDBCIHJKON();
			bool canOpen = CanOpen();

			_propertyControls.SetActive(_isOpen || pending != null);
			_deliveryPanel.SetActive(pending != null);
			_skipButton.gameObject.SetActive(pending != null);
			bool showForgeButton = ShouldShowForgeButton();
			_forgeOpenButton.gameObject.SetActive(!_isOpen && showForgeButton);
			_forgeOpenButton.interactable = pending == null && canOpen;
			_applyButton.gameObject.SetActive(_isOpen && pending == null);
			_closeButton.gameObject.SetActive(_isOpen);

			if (pending != null)
			{
				_deliveryText.text = FormatDuration(pending.TimeLeft);
				long rubyPrice = pending.KLHOKKPALOK;
				_skipButton.SetText(rubyPrice.ToString());
				ListSF.CheckItems check = ListSF.CLKECIFEMNB(pending, ItemAction.Item_Recipe_Delivery_Ruby);
				_skipButton.interactable = check != null && check.Value >= 0;
			}

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

			_hadPendingDelivery = true;
			_mainMenu?.UpdateMaterials();
			_shop.RefreshAfterForgeMutation();
			ResolveSelectedItem();
			Close();
			RefreshPropertyControls();
		}

		private void SkipDelivery()
		{
			ResolveSelectedItem();
			RecipeItemInfo pending = _selectedItem?.PHDBCIHJKON();
			if (pending == null) return;
			ListSF.CheckItems check = ListSF.CLKECIFEMNB(pending, ItemAction.Item_Recipe_Delivery_Ruby);
			if (check == null || check.Value < 0) return;
			if (!ListSF.KCBCGDFKNME(pending, ItemAction.Item_Recipe_Delivery_Ruby, check.Value)) return;

			_hadPendingDelivery = false;
			_mainMenu?.UpdateMainMenu();
			_shop.RefreshAfterForgeMutation();
			ResolveSelectedItem();
			RefreshPropertyControls();
		}

		private void SetShopForgeOffset(bool forgeMode)
		{
			if (_layoutOffsetApplied == forgeMode) return;
			Vector2 forgeOffset = forgeMode ? new Vector2(ForgeModeOffset, 0f) : Vector2.zero;

			// The intact ShopScene moves one _PanelsContainer by _ForgeModeX.  This
			// recovered scene split that container into two top-level groups: the item
			// scroller and the side panels. Move each group once; never move the
			// Parameters/Properties children independently or the shop tears apart.
			if (_itemsRoot != null) _itemsRoot.anchoredPosition = _itemsNormalPosition + forgeOffset;
			if (_sidePanelsRoot != null && _sidePanelsRoot != _itemsRoot)
				_sidePanelsRoot.anchoredPosition = _sidePanelsNormalPosition + forgeOffset;
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

		private static ResolutionImage CreateButtonIcon(Transform parent, string spriteName)
		{
			ResolutionImage icon = CreateSprite("Icon", parent, spriteName, new Vector2(58f, 58f));
			RectTransform rect = icon.rectTransform;
			rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
			rect.pivot = new Vector2(1f, 0.5f);
			rect.anchoredPosition = new Vector2(-45f, 0f);
			return icon;
		}

		private static ResolutionImage CreateSprite(string name, Transform parent, string spriteName, Vector2 size)
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
			image.set_TexturePath("UI/Atlases/");
			image.set_SpriteName(spriteName);
			image.preserveAspect = true;
			return image;
		}

		private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor alignment, Color color)
		{
			GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
			textObject.layer = parent.gameObject.layer;
			textObject.transform.SetParent(parent, false);
			RectTransform rect = textObject.GetComponent<RectTransform>();
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.anchoredPosition = anchoredPosition;
			rect.sizeDelta = size;
			Text text = textObject.GetComponent<Text>();
			text.font = LocalizationManager.MBPJIKFOEBJ();
			text.fontSize = fontSize;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 12;
			text.resizeTextMaxSize = fontSize;
			text.alignment = alignment;
			text.color = color;
			text.raycastTarget = false;
			return text;
		}

		private static ItemInfo CurrentInfo(UserItem userItem)
		{
			if (userItem == null) return null;
			return userItem.DBLCMCEGJGI(false) ?? userItem.BHKHOJPANHE();
		}

		private static string FormatDuration(long seconds)
		{
			if (seconds < 0L) seconds = 0L;
			TimeSpan duration = TimeSpan.FromSeconds(seconds);
			if (duration.TotalDays >= 1d)
				return string.Format("{0}d {1:00}:{2:00}:{3:00}", (int)duration.TotalDays, duration.Hours, duration.Minutes, duration.Seconds);
			return string.Format("{0:00}:{1:00}:{2:00}", (int)duration.TotalHours, duration.Minutes, duration.Seconds);
		}

		private sealed class RecipeCard
		{
			public readonly Recipe Recipe;
			public readonly GameObject Root;
			public readonly Button Button;
			public readonly CanvasGroup Canvas;

			public RecipeCard(Recipe recipe, GameObject root, Button button, CanvasGroup canvas)
			{
				Recipe = recipe;
				Root = root;
				Button = button;
				Canvas = canvas;
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