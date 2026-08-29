using System;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Underworld
{
	public sealed class UnderworldMapControls
	{
		private readonly Transform _parent;
		private readonly InfoBattle _infoBattle;
		private readonly Action _toggleMap;
		private readonly Action _scrollDown;
		private readonly Action _togglePowerMode;

		private RectTransform _root;
		private Button _toggleButton;
		private Image _toggleImage;
		private Button _scrollButton;
		private GameObject _powerToggleObject;
		private Image _powerToggleImage;

		public UnderworldMapControls(Transform parent, InfoBattle infoBattle, Action toggleMap,
			Action scrollDown, Action togglePowerMode)
		{
			_parent = parent;
			_infoBattle = infoBattle;
			_toggleMap = toggleMap;
			_scrollDown = scrollDown;
			_togglePowerMode = togglePowerMode;
		}

		public void Initialize(bool raidMode)
		{
			CreateToggleButton();
			UpdateToggleSprite(raidMode);
			CreateRaidModeControls();
		}

		public void SetToggleVisible(bool visible, bool raidMode)
		{
			if (_toggleButton == null && visible)
			{
				CreateToggleButton();
			}
			UpdateToggleSprite(raidMode);
			if (_toggleButton != null)
			{
				_toggleButton.gameObject.SetActive(visible);
			}
		}

		public void UpdateState(bool raidMode, bool powerMode)
		{
			if (_scrollButton != null)
			{
				_scrollButton.gameObject.SetActive(raidMode);
			}
			if (_powerToggleObject != null)
			{
				_powerToggleObject.SetActive(raidMode);
			}
			if (_powerToggleImage != null)
			{
				string sprite = powerMode ? "RaidHardmodeUI.checkboxOn" : "RaidHardmodeUI.checkboxGray";
				_powerToggleImage.sprite = ResolutionImage.GetSprite("UI/Atlases/", sprite);
			}
			UpdateToggleSprite(raidMode);
		}

		public void UpdateToggleSprite(bool raidMode)
		{
			if (_toggleImage == null)
			{
				return;
			}
			string spriteName = raidMode ? "RaidMap.raid_up" : "RaidMap.raid_down";
			_toggleImage.sprite = ResolutionImage.GetSprite("UI/Atlases/", spriteName);
			if (_toggleImage.sprite == null)
			{
				Debug.LogWarning("[Underworld] missing toggle sprite " + spriteName);
			}
		}

		private RectTransform GetRoot()
		{
			if (_root == null)
			{
				_root = UnderworldMapControlsLayout.CreateRoot(_parent);
			}
			return _root;
		}

		private void CreateToggleButton()
		{
			if (_toggleButton != null)
			{
				return;
			}
			bool hasRaidZones = ListSF.FHAIJEAPFEA().Exists(UnderworldZonePolicy.IsRaidZone);
			if (!hasRaidZones)
			{
				Debug.LogWarning("[Underworld] raid toggle hidden because no raid zones were loaded");
				return;
			}

			GameObject toggleObject = new GameObject("UnderworldToggle", typeof(RectTransform),
				typeof(CanvasRenderer), typeof(Image), typeof(Button));
			RectTransform rect = toggleObject.GetComponent<RectTransform>();
			UnderworldMapControlsLayout.AnchorUnderworldToggle(rect, GetRoot());

			_toggleImage = toggleObject.GetComponent<Image>();
			_toggleImage.preserveAspect = true;
			_toggleButton = toggleObject.GetComponent<Button>();
			_toggleButton.targetGraphic = _toggleImage;
			_toggleButton.onClick.AddListener(() => _toggleMap());
			Debug.Log("[Underworld] map toggle created with " +
				ListSF.FHAIJEAPFEA().FindAll(UnderworldZonePolicy.IsRaidZone).Count +
				" raid zone(s)");
		}

		private void CreateRaidModeControls()
		{
			if (_scrollButton != null)
			{
				return;
			}

			GameObject scrollObject = new GameObject("RaidMapScrollButton", typeof(RectTransform),
				typeof(CanvasRenderer), typeof(Image), typeof(Button));
			RectTransform scrollRect = scrollObject.GetComponent<RectTransform>();
			UnderworldMapControlsLayout.AnchorRaidScrollButton(scrollRect, GetRoot());
			Image scrollImage = scrollObject.GetComponent<Image>();
			scrollImage.preserveAspect = true;
			scrollImage.sprite = ResolutionImage.GetSprite("UI/Atlases/", "RaidMap.raid_down_arrow");
			_scrollButton = scrollObject.GetComponent<Button>();
			_scrollButton.targetGraphic = scrollImage;
			_scrollButton.onClick.AddListener(() => _scrollDown());

			_powerToggleObject = new GameObject("RaidPowerModeToggle", typeof(RectTransform),
				typeof(CanvasRenderer), typeof(Image), typeof(Button));
			_powerToggleObject.transform.SetParent(GetRoot(), false);
			_powerToggleObject.layer = _root.gameObject.layer;
			_powerToggleObject.transform.SetAsLastSibling();
			RectTransform powerRect = _powerToggleObject.GetComponent<RectTransform>();
			powerRect.anchorMin = new Vector2(0f, 1f);
			powerRect.anchorMax = new Vector2(0f, 1f);
			powerRect.pivot = new Vector2(0f, 1f);
			powerRect.anchoredPosition = new Vector2(300f, -108f);
			powerRect.sizeDelta = new Vector2(235f, 54f);
			_powerToggleObject.GetComponent<Image>().color = Color.clear;

			GameObject checkObject = new GameObject("Checkbox", typeof(RectTransform), typeof(Image));
			checkObject.transform.SetParent(_powerToggleObject.transform, false);
			_powerToggleImage = checkObject.GetComponent<Image>();
			_powerToggleImage.rectTransform.anchorMin = _powerToggleImage.rectTransform.anchorMax = new Vector2(0f, 0.5f);
			_powerToggleImage.rectTransform.anchoredPosition = new Vector2(27f, 0f);
			_powerToggleImage.rectTransform.sizeDelta = new Vector2(54f, 54f);
			_powerToggleImage.raycastTarget = false;
			_powerToggleImage.preserveAspect = true;

			Button powerButton = _powerToggleObject.GetComponent<Button>();
			powerButton.targetGraphic = _powerToggleImage;
			powerButton.onClick.AddListener(() => _togglePowerMode());

			GameObject labelObject = new GameObject("Label", typeof(RectTransform),
				typeof(CanvasRenderer), typeof(Text));
			labelObject.transform.SetParent(_powerToggleObject.transform, false);
			RectTransform labelRect = labelObject.GetComponent<RectTransform>();
			labelRect.anchorMin = new Vector2(0f, 0f);
			labelRect.anchorMax = new Vector2(1f, 1f);
			labelRect.offsetMin = new Vector2(58f, 0f);
			labelRect.offsetMax = Vector2.zero;
			Text label = labelObject.GetComponent<Text>();
			Text sample = _infoBattle.GetComponentInChildren<Text>(true);
			if (sample != null)
			{
				label.font = sample.font;
				label.material = sample.material;
			}
			label.text = "POWER MODE";
			label.fontSize = 24;
			label.alignment = TextAnchor.MiddleLeft;
			label.color = new Color(0.28f, 0.12f, 0.06f, 1f);
			label.raycastTarget = false;
		}
	}
}
