using System;
using System.Collections.Generic;
using Nekki.SF2.GUI.Shop;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class TrickContent : Content
	{
		public enum PMJPELIOMIL
		{
			zText = 0,
			zSlider = 1,
			zIcon = 2,
			zGradient = 3
		}

		private const int OAKHGJGOGHE = -294;

		private const int BMGBEAOHCIP = 400;

		private const string JMONPFHPBJK = "ComboButtons.base_damage";

		private const int LBGJODHJBHJ = 40;

		private const int KGOONNMIPBM = -20;

		private const int APMECJNLMCL = 10;

		private const int AIHFCFGEDHB = -20;

		[SerializeField]
		private LabelAlias _valueLabel;

		[SerializeField]
		private LabelAlias _descriptionLabel;

		[SerializeField]
		private LabelButton _btnShow;

		[SerializeField]
		private Scroll _slider;

		private List<float> _value = new List<float>();

		private string _description;

		private InfoAnimation BJONHDGCNFE;

		private Action<object> FNOECGMEKGL;

		private void Start()
		{
			_btnShow.onClick.AddListener(KLEKOHHGAEM);
		}

		public void Init(InfoAnimation EMBBNNBFODN, List<float> CKKFKEIELCP, Action<object> ODDEOFKLIAG = null, string EMDJGBHIAIA = "")
		{
			BJONHDGCNFE = EMBBNNBFODN;
			_value = CKKFKEIELCP;
			FNOECGMEKGL = ODDEOFKLIAG;
			_description = EMDJGBHIAIA;
			IOKIOGIMEBC();
			PHKIJLEICHE();
			JILNGKIPLIK();
			bool flag = _descriptionLabel.gameObject.activeSelf && _descriptionLabel.preferredHeight + _valueLabel.preferredHeight > 400f;
			bool flag2 = _valueLabel.preferredHeight > 400f;
			if (flag || flag2)
			{
			}
			JPFEBFEBFMF();
		}

		public override void SetUpBorder(float BGEEALIPKCC)
		{
			if (_btnShow != null && _valueLabel != null)
			{
				float num = _btnShow.transform.localPosition.y + _btnShow.GetComponent<RectTransform>().rect.height / 2f;
				float bAINMLLIKOL = BGEEALIPKCC - (BGEEALIPKCC - num) / 2f;
				_valueLabel.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}

		public LabelButton GetBtnShow()
		{
			return _btnShow;
		}

		private void IOKIOGIMEBC()
		{
			_descriptionLabel.gameObject.SetActive(false);
			if (!(_description == string.Empty))
			{
				_descriptionLabel.set_LabelFontSize(104);
				_descriptionLabel.color = Constants.PJJIMHMJPAL;
				_descriptionLabel.set_Alias(_description);
			}
		}

		private void JILNGKIPLIK()
		{
			string text = string.Empty;
			for (int i = 0; i < _value.Count; i++)
			{
				// The legacy inline-quad glyph depends on atlas/font UV metadata which
				// was not preserved by the decompilation and renders as a noisy square.
				// Keep the value readable and deterministic until the glyph atlas is
				// reconstructed as a real sprite-backed control.
				text = text + LocalizationManager.GetString("lblDamage") + " " + (int)(100f * _value[i])/*cast due to constrained. prefix*/;
				if (i < _value.Count - 1)
				{
					text += "\n";
				}
			}
			_valueLabel.transform.OKHPLHPBPKJ(-20f);
			_valueLabel.transform.BGNJGIACJBG(10f);
			_valueLabel.color = Constants.PJJIMHMJPAL;
			_valueLabel.set_text(text);
			_valueLabel.SetVerticesDirty();
			IEFFLCBGJJM();
			_slider.ScrollToItem(0);
		}

		private void PHKIJLEICHE()
		{
			_btnShow.transform.OKHPLHPBPKJ(0f);
			_btnShow.transform.BGNJGIACJBG(-294f);
			if (BJONHDGCNFE == null || !BJONHDGCNFE.NHNEJKIBPJG)
			{
				_btnShow.gameObject.SetActive(false);
			}
		}

		private void JPFEBFEBFMF()
		{
			if (_descriptionLabel != null && _slider != null && _value.Count > 0)
			{
				if (_valueLabel.rectTransform.rect.height + _descriptionLabel.rectTransform.rect.height < 400f)
				{
					_descriptionLabel.transform.BGNJGIACJBG(-20f + _valueLabel.rectTransform.rect.height / 2f);
				}
				else
				{
					_descriptionLabel.transform.BGNJGIACJBG(155f);
				}
			}
		}

		private void KLEKOHHGAEM()
		{
			if (FNOECGMEKGL != null)
			{
				FNOECGMEKGL(null);
			}
		}

		private void IEFFLCBGJJM()
		{
		}
	}
}
