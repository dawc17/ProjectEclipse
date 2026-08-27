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
			}
		}
	}
}
