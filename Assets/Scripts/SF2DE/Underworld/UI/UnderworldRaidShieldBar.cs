using UnityEngine;
using UnityEngine.UI;

namespace SF2DE.Underworld.UI
{
	// Count includes the currently draining bar. ShieldTotal is a total,
	// not a number of bonus bars. Keep large pools inside the HUD.
	public class UnderworldRaidShieldBar : MonoBehaviour
	{
		private ModelParameters _parameters;
		private Text _count;
		private int _lastCount = -1;

		public void UpdateBar(float unusedFraction)
		{
			int remaining = _parameters.RemainingHealthBars;
			if (_lastCount != remaining)
			{
				_count.text = "x " + remaining;
				_lastCount = remaining;
			}
		}

		public void SetVisible(bool value)
		{
			gameObject.SetActive(value && _parameters.ShieldTotal > 0);
		}

		public static UnderworldRaidShieldBar Attach(Transform parent, ModelParameters parameters, Font font)
		{
			if (parent == null || parameters == null || parameters.ShieldTotal <= 0)
				return null;
			GameObject host = new GameObject("RaidShieldBar", typeof(RectTransform));
			host.layer = parent.gameObject.layer;
			host.transform.SetParent(parent, false);
			RectTransform rect = (RectTransform)host.transform;
			// The enemy bar's local right edge is its SCREEN-left edge. Anchor
			// there so the counter follows the HUD at every canvas resolution.
			rect.anchorMin = rect.anchorMax = new Vector2(1f, 0f);
			rect.pivot = new Vector2(0f, 1f);
			rect.anchoredPosition = new Vector2(-12f, -2f);
			// The right-hand life bar is rotated ~180 degrees around Y in the
			// prefab. Counter-rotate its text, not the draining bar itself.
			rect.localRotation = Quaternion.Inverse(parent.localRotation);
			rect.sizeDelta = new Vector2(115f, 42f);
			GameObject countObject = new GameObject("Count", typeof(RectTransform), typeof(Text));
			countObject.layer = host.layer;
			countObject.transform.SetParent(host.transform, false);
			Text count = countObject.GetComponent<Text>();
			count.rectTransform.anchorMin = Vector2.zero;
			count.rectTransform.anchorMax = Vector2.one;
			count.rectTransform.offsetMin = Vector2.zero;
			count.rectTransform.offsetMax = Vector2.zero;
			count.font = font;
			count.fontSize = 32;
			// Sakkal Majalla's line metrics exceed this compact HUD rect. The
			// default Truncate mode discards the entire first line, even though
			// its glyphs fit. Do not wrap or truncate this single-line counter.
			count.horizontalOverflow = HorizontalWrapMode.Overflow;
			count.verticalOverflow = VerticalWrapMode.Overflow;
			count.alignment = TextAnchor.MiddleLeft;
			count.color = new Color(1f, 0.84f, 0.55f, 1f);
			count.raycastTarget = false;
			UnderworldRaidShieldBar bar = host.AddComponent<UnderworldRaidShieldBar>();
			bar._parameters = parameters;
			bar._count = count;
			bar.UpdateBar(1f);
			return bar;
		}
	}
}
