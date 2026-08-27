using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class BattlePrizeElement : SFMonoBehaviour<object>
	{
		public const string ICONS_ATLAS = "MiscSprites";

		[SerializeField]
		private ResolutionImage _icon;

		[SerializeField]
		private Text _value;

		public void Init(string ADONPNOBBDE, long value, int CFMPJLLNCFF, float JPDGMJHNKPK = 1f)
		{
			_icon.set_TexturePath("MiscSprites");
			_icon.set_SpriteName(ADONPNOBBDE);
			_icon.SetNativeSize();
			_icon.rectTransform.localScale = new Vector3(JPDGMJHNKPK, JPDGMJHNKPK);
			_value.fontSize = CFMPJLLNCFF;
			_value.color = Constants.PJJIMHMJPAL;
			_value.text = value.ToString();
			JPDODBLOPFJ();
		}

		private void JPDODBLOPFJ()
		{
			float num = 10f;
			float num2 = _icon.rectTransform.rect.width * _icon.rectTransform.localScale.x;
			float num3 = _icon.rectTransform.rect.height * _icon.rectTransform.localScale.y;
			float preferredWidth = LayoutUtility.GetPreferredWidth(_value.rectTransform);
			float preferredHeight = LayoutUtility.GetPreferredHeight(_value.rectTransform);
			_icon.transform.OKHPLHPBPKJ((0f - preferredWidth) / 2f - num / 2f);
			_value.transform.OKHPLHPBPKJ(num2 / 2f + num / 2f);
			float num4 = num2 + preferredWidth + num;
			float num5 = ((!(num3 > preferredHeight)) ? preferredHeight : num3);
			LayoutElement component = GetComponent<LayoutElement>();
			component.minWidth = num4;
			component.preferredWidth = num4;
			component.minHeight = num5;
			component.preferredHeight = num5;
		}
	}
}
