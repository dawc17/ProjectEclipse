using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class IconAndText : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImageLE icon;

		[SerializeField]
		private LabelAliasLE text;

		[SerializeField]
		private LayoutElement layoutElement;

		public void SetIcon(string INFKLFKKJOJ)
		{
			if (icon != null)
			{
				icon.set_SpriteName(INFKLFKKJOJ);
			}
			PMHOLIPDBLC();
		}

		public void SetText(string value)
		{
			if (text != null)
			{
				// Reward counters are single-line. Their larger authored font can
				// exceed the legacy line box even when the measured glyphs fit.
				text.horizontalOverflow = HorizontalWrapMode.Overflow;
				text.verticalOverflow = VerticalWrapMode.Overflow;
				text.set_text(value);
			}
			PMHOLIPDBLC();
		}

		private void PMHOLIPDBLC()
		{
			if (layoutElement != null)
			{
				float num = 0f;
				if (icon != null)
				{
					num += icon.get_LayoutElement().minWidth;
				}
				if (text != null)
				{
					num += text.get_LayoutElement().minWidth;
				}
				// This row participates in its parent's horizontal layout. Without the
				// measured width it collapses and clips the reward amount beside the icon.
				var row = GetComponent<HorizontalLayoutGroup>();
				if (row != null) num += row.padding.horizontal + (icon != null && text != null ? row.spacing : 0f);
				layoutElement.minWidth = num;
				layoutElement.preferredWidth = num;
			}
		}
	}
}
