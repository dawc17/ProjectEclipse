using UnityEngine;

namespace Nekki.SF2.GUI.Map
{
	public static class RaidMapControlsLayout
	{
		public static RectTransform CreateRoot(Transform parent)
		{
			// WideScreenController rewrites offsets of direct canvas children.
			// Give it a stretched layer, never a fixed-size button: rewriting
			// offsets on a right-anchored button collapses its width and moves it.
			var root = new GameObject("RaidControlsLayer", typeof(RectTransform));
			root.layer = parent.gameObject.layer;
			var rect = (RectTransform)root.transform;
			rect.SetParent(parent, false);
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = rect.offsetMax = Vector2.zero;
			rect.SetAsLastSibling();
			return rect;
		}

		public static void AnchorNavigationButton(RectTransform rect, RectTransform root, float y)
		{
			rect.SetParent(root, false);
			rect.gameObject.layer = root.gameObject.layer;
			rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector2(-445f, y);
			rect.sizeDelta = new Vector2(102f, 98f);
		}

		public static void AnchorUnderworldToggle(RectTransform rect, RectTransform root)
		{
			rect.SetParent(root, false);
			rect.gameObject.layer = root.gameObject.layer;
			rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector2(-885f, -615f);
			rect.sizeDelta = new Vector2(145f, 140f);
		}

		public static void AnchorRaidScrollButton(RectTransform rect, RectTransform root)
		{
			rect.SetParent(root, false);
			rect.gameObject.layer = root.gameObject.layer;
			rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector2(-730f, -615f);
			rect.sizeDelta = new Vector2(125f, 120f);
		}
	}
}
