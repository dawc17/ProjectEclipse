using Nekki.SF2.GUI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI.TopBar
{
	public static class DesktopTopBarLayout
	{
		public static void Configure(MenuExpPanel experience, MenuEnergyPanel energy, MenuMoneyPanel money)
		{
			energy.gameObject.SetActive(false);
			money.ConfigureCompactTopBar();

			HorizontalLayoutGroup layout = experience.transform.parent.GetComponent<HorizontalLayoutGroup>();
			if (layout == null)
			{
				return;
			}
			layout.childForceExpandWidth = false;
			layout.childControlWidth = true;
			layout.childAlignment = TextAnchor.MiddleCenter;
			layout.spacing = 30f;
		}

		public static void ConfigureMoneyPanel(Component panel, Button rubyButton, Button shopButton,
			ImageAnimation rubySale, RectTransform coinIcon, RectTransform coinText,
			RectTransform bonusIcon, RectTransform bonusText)
		{
			if (rubyButton != null)
			{
				rubyButton.gameObject.SetActive(false);
			}
			if (shopButton != null)
			{
				shopButton.gameObject.SetActive(false);
			}
			if (rubySale != null)
			{
				rubySale.gameObject.SetActive(false);
			}

			RectTransform[] content = { coinIcon, coinText, bonusIcon, bonusText };
			float left = float.MaxValue;
			float right = float.MinValue;
			foreach (RectTransform rect in content)
			{
				float halfWidth = rect.rect.width / 2f;
				left = Mathf.Min(left, rect.anchoredPosition.x - halfWidth);
				right = Mathf.Max(right, rect.anchoredPosition.x + halfWidth);
			}

			float centerOffset = -(left + right) / 2f;
			foreach (RectTransform rect in content)
			{
				Vector2 position = rect.anchoredPosition;
				position.x += centerOffset;
				rect.anchoredPosition = position;
			}

			LayoutElement layout = panel.GetComponent<LayoutElement>();
			if (layout != null)
			{
				float width = right - left + 60f;
				layout.minWidth = width;
				layout.preferredWidth = width;
			}
		}
	}
}
