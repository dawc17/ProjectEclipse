using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Menu
{
	public class MenuMaterSprite : MonoBehaviour
	{
		private GameCurrency JJPFBOKGIEF;

		private int _value;

		[SerializeField]
		private Image _icon;

		[SerializeField]
		private Text _valueLbl;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(GameCurrency MDDNHLBDJBN)
		{
			JJPFBOKGIEF = MDDNHLBDJBN;
			_value = ListSF.CCDKHLAMKKO().GetCurrencyCount(JJPFBOKGIEF);
			string mJBPMLCLMFN = JJPFBOKGIEF.MJBPMLCLMFN;
			if (_icon != null)
			{
				_icon.sprite = Nekki.SF2.GUI.ResolutionImage.GetSprite("UI/Atlases/", mJBPMLCLMFN);
			}
			if (_valueLbl != null)
			{
				_valueLbl.font = LocalizationManager.MBPJIKFOEBJ();
				_valueLbl.resizeTextForBestFit = false;
				_valueLbl.horizontalOverflow = HorizontalWrapMode.Overflow;
				_valueLbl.verticalOverflow = VerticalWrapMode.Truncate;
				_valueLbl.rectTransform.anchorMin = new Vector2(0f, 0.5f);
				_valueLbl.rectTransform.anchorMax = new Vector2(1f, 0.5f);
				_valueLbl.rectTransform.offsetMin = new Vector2(110f, -33f);
				_valueLbl.rectTransform.offsetMax = new Vector2(-10f, 33f);
				_valueLbl.text = _value.ToString();
			}
		}

		private void LateUpdate()
		{
			if (_valueLbl == null || _valueLbl.font == null) return;
			// Fit the complete integer to the space remaining after the orb icon.
			var settings = _valueLbl.GetGenerationSettings(Vector2.zero);
			settings.fontSize = 60;
			float width = _valueLbl.cachedTextGeneratorForLayout.GetPreferredWidth(_valueLbl.text, settings) / _valueLbl.pixelsPerUnit;
			float available = _valueLbl.rectTransform.rect.width;
			_valueLbl.fontSize = Mathf.Clamp(Mathf.FloorToInt(60f * available / Mathf.Max(1f, width)), 1, 60);
		}

		public void UpdateView()
		{
			int num = ListSF.CCDKHLAMKKO().GetCurrencyCount(JJPFBOKGIEF);
			if (_value != num)
			{
				_value = num;
				_valueLbl.text = _value.ToString();
			}
		}
	}
}
