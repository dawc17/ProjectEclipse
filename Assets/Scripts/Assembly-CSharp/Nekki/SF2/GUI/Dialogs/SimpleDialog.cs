using System;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Dialogs
{
	public class SimpleDialog : BaseDialog
	{
		private const int KHCBFOBKIDF = 100;

		private const int IBNKNFCOHMM = -20;

		private const float IGANLEMDENC = 1680f;

		private const float MONACNLJJPI = 1100f;

		private const float LODNLNEGOFL = 60f;

		private const float LMIOPOFAEMM = -20f;

		private const float IGDGCHMFJIF = 100f;

		private const int EAGOJOBFHBC = 103;

		[SerializeField]
		private LabelAlias _label;

		[SerializeField]
		private LabelAlias _checkBoxLabel;

		[SerializeField]
		private Toggle _checkBox;

		private Action<object> _dlg;

		private string KKHONCCHNMG = string.Empty;

		private bool LOOHNJPAAHD;

		private bool CJJBDGPDOFF;

		private string IAHHOEJJJHP = string.Empty;

		private float OCMLLEDKLFL = 100f;

		protected LabelButton.FBMGEHJPPIK CHEHEHHMDNF = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;

		protected LabelButton.FBMGEHJPPIK AHJODMEKEGG;

		public override void Init(object data)
		{
			string dIKEFIIPNBE = string.Empty;
			KBDHPMOMJLL hJNAHNICGMH = KBDHPMOMJLL.FOOTER_NONE;
			if (data != null)
			{
				SimpleDialogInfo jJMIOMABAKK = (SimpleDialogInfo)data;
				BGJJDGOBPKA = jJMIOMABAKK.BKANENCBCOA;
				EBCJGLPLHAD = jJMIOMABAKK.FFPLNDENING;
				CHEHEHHMDNF = jJMIOMABAKK.HBMMFJGFCPH;
				AHJODMEKEGG = jJMIOMABAKK.NLLFNHKKKBE;
				_dlg = jJMIOMABAKK.Dlg;
				KKHONCCHNMG = jJMIOMABAKK.GGDJIPKMKFC;
				LOOHNJPAAHD = jJMIOMABAKK.KIGGOAIKFCB;
				CJJBDGPDOFF = jJMIOMABAKK.HLFPOONJFNM;
				IAHHOEJJJHP = jJMIOMABAKK.CJKCAIJLFPN;
				dIKEFIIPNBE = jJMIOMABAKK.Title;
				hJNAHNICGMH = jJMIOMABAKK.DHKDOHFKOOJ;
			}
			base.Init(dIKEFIIPNBE, BGJJDGOBPKA, EBCJGLPLHAD, hJNAHNICGMH);
		}

		protected override void Start()
		{
			base.Start();
			_label.set_Alias(KKHONCCHNMG);
			_checkBox.gameObject.SetActive(LOOHNJPAAHD);
			_checkBoxLabel.gameObject.SetActive(LOOHNJPAAHD);
			if (LOOHNJPAAHD)
			{
				CreateCheckBox(CJJBDGPDOFF, IAHHOEJJJHP);
			}
			CGICCNNDLPC();
		}

		protected virtual void JNFDOIOKDJH(bool value)
		{
			if (_dlg != null)
			{
				int num = ((!value) ? 4 : 3);
				_dlg(num);
			}
		}

		private void LHLIIGLPOOP()
		{
			float jMLAKAKDBBL = Math.Min(Math.Max(INNODIOFCPO(), 1100f), 1680f);
			NJHHGCMGCGH(jMLAKAKDBBL);
			JOGBCHAOLOG();
			_content.GetComponent<RectTransform>().sizeDelta = new Vector2(_content.GetComponent<RectTransform>().rect.width, _label.preferredHeight);
		}

		private void NJHHGCMGCGH(float JMLAKAKDBBL)
		{
			OCMLLEDKLFL = JMLAKAKDBBL;
		}

		private void JOGBCHAOLOG()
		{
			if (GBECKKCHAFI == KBDHPMOMJLL.FOOTER_BOTH)
			{
				float width = _btnOK.GetComponent<RectTransform>().rect.width;
				float width2 = _btnCancel.GetComponent<RectTransform>().rect.width;
				float num = (OCMLLEDKLFL - width - width2) / 3f;
				float num2 = (0f - OCMLLEDKLFL) / 2f + num + width2 / 2f;
				_btnCancel.transform.OKHPLHPBPKJ(num2);
				num2 += width2 / 2f + num + width / 2f;
				_btnOK.transform.OKHPLHPBPKJ(num2);
			}
			else if (GBECKKCHAFI == KBDHPMOMJLL.FOOTER_OK)
			{
				_btnOK.transform.OKHPLHPBPKJ(0f);
			}
			else if (GBECKKCHAFI == KBDHPMOMJLL.FOOTER_CANCEL)
			{
				_btnCancel.transform.OKHPLHPBPKJ(0f);
			}
		}

		private float INNODIOFCPO()
		{
			int num = 1;
			float num2 = 0f;
			if (GBECKKCHAFI == KBDHPMOMJLL.FOOTER_BOTH || GBECKKCHAFI == KBDHPMOMJLL.FOOTER_OK)
			{
				num2 = _btnOK.GetComponent<RectTransform>().rect.width;
				num++;
			}
			float num3 = 0f;
			if (GBECKKCHAFI == KBDHPMOMJLL.FOOTER_BOTH || GBECKKCHAFI == KBDHPMOMJLL.FOOTER_CANCEL)
			{
				num3 = _btnCancel.GetComponent<RectTransform>().rect.width;
				num++;
			}
			return num2 + num3 + 60f * (float)num;
		}

		private void CGICCNNDLPC()
		{
			LHLIIGLPOOP();
			JHADDLNINDI();
		}

		private void CreateCheckBox(bool EPHHGNKDPEG, string DOEEIGAHKEN)
		{
			float num = _checkBox.GetComponent<RectTransform>().rect.height / 4f;
			float bAINMLLIKOL = (0f - _label.preferredHeight) / 2f - 2f * num;
			_checkBox.transform.BGNJGIACJBG(bAINMLLIKOL);
			_label.transform.BGNJGIACJBG(_label.transform.localPosition.y + num);
			_checkBox.isOn = EPHHGNKDPEG;
			_checkBox.onValueChanged.RemoveListener(JNFDOIOKDJH);
			_checkBox.onValueChanged.AddListener(JNFDOIOKDJH);
			_checkBoxLabel.set_LabelFontSize(103);
			_checkBoxLabel.color = Constants.PJJIMHMJPAL;
			_checkBoxLabel.set_Alias(DOEEIGAHKEN);
			_checkBoxLabel.transform.BGNJGIACJBG(bAINMLLIKOL);
			float num2 = _checkBox.GetComponent<RectTransform>().rect.width + _checkBoxLabel.preferredWidth;
			num2 = _checkBoxLabel.preferredWidth;
			_checkBox.transform.OKHPLHPBPKJ((0f - num2) / 2f);
			_checkBoxLabel.transform.OKHPLHPBPKJ(_checkBox.GetComponent<RectTransform>().rect.width / 2f);
		}

		protected virtual void JHADDLNINDI()
		{
			float num = _content.GetComponent<RectTransform>().rect.height / 2f + 160f;
			if (_topStripe != null && _bottomStripe != null)
			{
				_topStripe.transform.BGNJGIACJBG(80f + num);
				_bottomStripe.transform.BGNJGIACJBG(-120f - num);
			}
			if (_header != null)
			{
				_header.transform.BGNJGIACJBG(num + -20f);
			}
			if (_btnOK != null)
			{
				_btnOK.transform.BGNJGIACJBG(0f - num + -20f);
			}
			if (_btnCancel != null)
			{
				_btnCancel.transform.BGNJGIACJBG(0f - num + -20f);
			}
		}

		protected override void PHKIJLEICHE(LabelButton GAMILDJHFDB, KBDHPMOMJLL MOPOCBKIKBI)
		{
			string alias = string.Empty;
			int buttonId = 0;
			LabelButton.FBMGEHJPPIK color = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;
			switch (MOPOCBKIKBI)
			{
			case KBDHPMOMJLL.FOOTER_CANCEL:
				alias = EBCJGLPLHAD;
				color = AHJODMEKEGG;
				buttonId = 0;
				break;
			case KBDHPMOMJLL.FOOTER_OK:
				alias = BGJJDGOBPKA;
				color = CHEHEHHMDNF;
				buttonId = 1;
				break;
			}
			GAMILDJHFDB.SetColor(color);
			GAMILDJHFDB.SetAlias(alias);
			GAMILDJHFDB.ButtonId = buttonId;
			GAMILDJHFDB.RemoveEventListener(2, OnClose);
			GAMILDJHFDB.AddEventListener(2, OnClose);
		}
	}
}
