using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ParameterScrollItem : BaseScrollItem
	{
		public enum JDEIHDIBFOP
		{
			ORANGE = 0,
			GREEN = 1,
			RED = 2
		}

		[SerializeField]
		protected ResolutionImage _icon;

		[SerializeField]
		protected LabelAlias _value;

		[SerializeField]
		protected LabelAlias _additionalValue;

		[SerializeField]
		protected ParameterProgressBar _progressBar;

		[SerializeField]
		protected LayoutElement _layoutElement;

		private string BJNGHILDLDB = "Attributes.";

		protected string LIPDIOBCMBC = "Linear";

		protected string MLENIKFEMAO = "Exp";

		protected float FMHFHOLIFEB;

		protected float BDIIMJGEEJI;

		protected string DMCABMCOKGB;

		protected string KJPICMNJFGA;

		protected string _type;

		protected float NHFCOJNLMOI;

		protected float DANEDLPLBAG;

		protected float AMNKPEFDEEO;

		protected float _power;

		protected float DPODKOHAPGP;

		protected int HAOPIJJPNBD;

		protected int CCLCIIOEAGN;

		protected int LOMGCFPCHGH;

		protected Tween FNAKFEMPAOL;

		protected Tween ANMLMAEBNFA;

		public ResolutionImage MJBPMLCLMFN
		{
			get
			{
				return get_Icon();
			}
		}

		public float BKLMHPPPPGH
		{
			get
			{
				return get_MinWidth();
			}
			set
			{
				set_MinWidth(value);
			}
		}

		public float FFKGEMEMCAK
		{
			get
			{
				return get_MinHeight();
			}
			set
			{
				set_MinHeight(value);
			}
		}

		public string COJMGDDNDEF
		{
			get
			{
				return get_AtlasName();
			}
			set
			{
				set_AtlasName(value);
			}
		}

		public string DNMENKJBHFE
		{
			get
			{
				return get_AttributeName();
			}
		}

		public float OBGGBMDABAD
		{
			get
			{
				return get_LeftLimit();
			}
		}

		public float NGPJDHKOEJC
		{
			get
			{
				return get_RightLimit();
			}
		}

		public ResolutionImage get_Icon()
		{
			return _icon;
		}

		public float get_MinWidth()
		{
			if (_layoutElement != null)
			{
				return _layoutElement.minWidth;
			}
			return 0f;
		}

		public void set_MinWidth(float value)
		{
			if (_layoutElement != null)
			{
				_layoutElement.minWidth = value;
			}
		}

		public float get_MinHeight()
		{
			if (_layoutElement != null)
			{
				return _layoutElement.minHeight;
			}
			return 0f;
		}

		public void set_MinHeight(float value)
		{
			if (_layoutElement != null)
			{
				_layoutElement.minHeight = value;
			}
		}

		public string get_AtlasName()
		{
			return BJNGHILDLDB;
		}

		public void set_AtlasName(string value)
		{
			BJNGHILDLDB = value;
		}

		public string get_AttributeName()
		{
			return DMCABMCOKGB;
		}

		public float get_LeftLimit()
		{
			if (NHFCOJNLMOI >= 0f && DANEDLPLBAG >= 0f)
			{
				return NHFCOJNLMOI;
			}
			return (float)ListSF.CCDKHLAMKKO().PINDEKDNCNL() * AMNKPEFDEEO + (float)HAOPIJJPNBD;
		}

		public float get_RightLimit()
		{
			if (NHFCOJNLMOI >= 0f && DANEDLPLBAG >= 0f)
			{
				return DANEDLPLBAG;
			}
			return (float)ListSF.CCDKHLAMKKO().PINDEKDNCNL() * AMNKPEFDEEO + (float)HAOPIJJPNBD;
		}

		public void Init(string name, string ADONPNOBBDE, int value, int OKEFHDDPMEC, bool EIAKNKDEEKA, string MMOBJGKHPNA = null)
		{
			InitVariables(name, EIAKNKDEEKA, MMOBJGKHPNA);
			if (_progressBar != null)
			{
				_progressBar.Init();
			}
			if (_icon != null)
			{
				_icon.set_SpriteName(BJNGHILDLDB + ADONPNOBBDE);
			}
			SetValue(value, OKEFHDDPMEC);
		}

		public void SetValue(int value, int OKEFHDDPMEC, float _Duration = 0f)
		{
			SetTextValue(value, OKEFHDDPMEC, _Duration);
			SetProgressBarValue(value, OKEFHDDPMEC, _Duration);
		}

		public void SetTextValue(int value, int OKEFHDDPMEC, float _Duration = 0f)
		{
			BKJGPPDJMBJ(value, _Duration);
			int bAINMLLIKOL = OKEFHDDPMEC - value;
			KKIEENKPEGC(bAINMLLIKOL, _Duration);
		}

		protected void BKJGPPDJMBJ(int value, float _Duration)
		{
			if (!(_value == null))
			{
				KillTween(ref FNAKFEMPAOL);
				FNAKFEMPAOL = DOTween.To(() => CCLCIIOEAGN, (int DHDMNHCIPEH) =>
				{
					CCLCIIOEAGN = DHDMNHCIPEH;
					_value.set_text(CCLCIIOEAGN.ToString());
				}, value, _Duration);
			}
		}

		protected void KKIEENKPEGC(int value, float _Duration = 0f)
		{
			if (_additionalValue == null)
			{
				return;
			}
			KillTween(ref ANMLMAEBNFA);
			if (_Duration == 0f)
			{
				HPBCEGEACFF(value);
				return;
			}
			ANMLMAEBNFA = DOTween.To(() => LOMGCFPCHGH, HPBCEGEACFF, value, _Duration);
		}

		protected void HPBCEGEACFF(int value)
		{
			LOMGCFPCHGH = value;
			_additionalValue.set_text(string.Format((LOMGCFPCHGH <= 0) ? "({0})" : "(+{0})", LOMGCFPCHGH));
			_additionalValue.color = ((LOMGCFPCHGH <= 0) ? Constants.GJKMPOAJDCF : Constants.NHHLHLAMFMO);
			bool active = LOMGCFPCHGH != 0;
			_additionalValue.gameObject.SetActive(active);
		}

		protected void KillTween(ref Tween tween)
		{
			if (tween != null)
			{
				tween.Kill();
				tween = null;
			}
		}

		public void SetProgressBarValue(int MCOIPKLENOC, int OKEFHDDPMEC, float _Duration = 0f)
		{
			if (OKEFHDDPMEC < MCOIPKLENOC)
			{
				MCPIOGALBMK(OKEFHDDPMEC, JDEIHDIBFOP.ORANGE, _Duration);
				MCPIOGALBMK(MCOIPKLENOC, JDEIHDIBFOP.RED, _Duration);
				MCPIOGALBMK(OKEFHDDPMEC, JDEIHDIBFOP.GREEN, _Duration);
			}
			else
			{
				MCPIOGALBMK(MCOIPKLENOC, JDEIHDIBFOP.ORANGE, _Duration);
				MCPIOGALBMK(OKEFHDDPMEC, JDEIHDIBFOP.GREEN, _Duration);
				MCPIOGALBMK(MCOIPKLENOC, JDEIHDIBFOP.RED, _Duration);
			}
		}

		public void ResetValue()
		{
			if (_value != null)
			{
				_value.set_text("0");
			}
			if (_additionalValue != null)
			{
				_additionalValue.set_text("0");
				_additionalValue.gameObject.SetActive(false);
			}
			if (_progressBar != null)
			{
				_progressBar.SetValue(0f);
			}
		}

		private void InitVariables(string CEELFMIPAII, bool EIAKNKDEEKA, string MMOBJGKHPNA = null)
		{
			DMCABMCOKGB = CEELFMIPAII;
			KJPICMNJFGA = MMOBJGKHPNA;
			if (string.IsNullOrEmpty(KJPICMNJFGA))
			{
				WarriorAttribute bCNOAOPGAEI = GameUtils.BGENALLCKII.NGNDIGFKKHE(DMCABMCOKGB);
				KJPICMNJFGA = ((bCNOAOPGAEI == null) ? DMCABMCOKGB : bCNOAOPGAEI.HCCKLLOEPJN);
			}
			if (string.IsNullOrEmpty(KJPICMNJFGA))
			{
				NHFCOJNLMOI = -1f;
				DANEDLPLBAG = -1f;
				AMNKPEFDEEO = 1f;
				HAOPIJJPNBD = 0;
				return;
			}
			BarScale bABKPEHINKF = GameUtils.NPHEOMBNOLK.HNECOCDPENN(KJPICMNJFGA);
			if (bABKPEHINKF == null)
			{
				LLLOJBFMONN.Error("Needed barScale not exist.");
				return;
			}
			Limit pEKGEPHFCMN = ((!EIAKNKDEEKA) ? bABKPEHINKF.GPBFMLDPOKH(ListSF.CCDKHLAMKKO().PINDEKDNCNL()) : bABKPEHINKF.EHKJEKAIDFF(ListSF.CCDKHLAMKKO().PINDEKDNCNL()));
			if (pEKGEPHFCMN == null)
			{
				pEKGEPHFCMN = ((!EIAKNKDEEKA) ? bABKPEHINKF.IKEBHGKBGHO() : bABKPEHINKF.NMMHOKHKFEE());
			}
			NHFCOJNLMOI = 0f;
			DANEDLPLBAG = 0f;
			if (pEKGEPHFCMN != null)
			{
				NHFCOJNLMOI = pEKGEPHFCMN.OBGGBMDABAD;
				DANEDLPLBAG = pEKGEPHFCMN.NGPJDHKOEJC;
				AMNKPEFDEEO = pEKGEPHFCMN.LevelMultiplier;
				HAOPIJJPNBD = pEKGEPHFCMN.Shift;
			}
			_power = ((!(bABKPEHINKF.MFGLDPKEDJB < 0f)) ? bABKPEHINKF.MFGLDPKEDJB : FMHFHOLIFEB);
			DPODKOHAPGP = ((!(bABKPEHINKF.DPGMCKCDMBC < 0f)) ? bABKPEHINKF.DPGMCKCDMBC : BDIIMJGEEJI);
			_type = ((!string.IsNullOrEmpty(bABKPEHINKF.Type)) ? bABKPEHINKF.Type : LIPDIOBCMBC);
		}

		protected virtual void MCPIOGALBMK(int value, JDEIHDIBFOP index = JDEIHDIBFOP.ORANGE, float _Duration = 0f)
		{
			float oKEFHDDPMEC = GetPercentFromValue(value);
			if (_progressBar != null)
			{
				_progressBar.SetValue(oKEFHDDPMEC, (int)index, _Duration);
			}
		}

		protected virtual float GetPercentFromValue(float value)
		{
			float rightLimit = get_RightLimit();
			float num;
			if (!string.IsNullOrEmpty(_type) && _type.Equals(LIPDIOBCMBC))
			{
				num = Mathf.Pow(value / rightLimit, _power);
			}
			else
			{
				float bGJPLNFFEOB = GameUtils.BGJPLNFFEOB;
				num = Mathf.Pow(2f, (value - rightLimit) * _power / bGJPLNFFEOB);
			}
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			return Mathf.Max(num, DPODKOHAPGP);
		}
	}
}
