using System;
using System.Collections.Generic;
using Nekki.SF2.GUI.Map;
using Nekki.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Dialogs
{
	public class StrangerDialog : StoryDialog
	{
		private const int KHKOLFGFPCM = 2;

		private const int DLPMGPHJGAN = 0;

		private const int CDOMLIBFMGN = 1;

		private const int PELMMKODEME = 3;

		private const int EIHCJGIBION = 13;

		private const int LOMFJFMBMCN = 1;

		private const int IONANDLGODB = 100;

		private const int OHEMMPFCFLO = 90;

		private const int GHPHGGJADMI = -80;

		private const int LPKENGHDHIA = -60;

		private const int KMMBFLDHAKE = 280;

		private const int DPJCJMNHJPA = 130;

		private const int FKGKGIAJOMH = 87;

		private const int LLLAOBBPFHK = 40;

		private const int EHMDBBMBFMJ = 40;

		private const int EGKAICLFKGD = 900;

		private const int OHPAGLENIIM = 103;

		[SerializeField]
		private LabelButton _rejectButton;

		[SerializeField]
		private LabelButton _acceptButton;

		[SerializeField]
		private LabelButton _storeButton;

		[SerializeField]
		private Toggle _checkBox;

		[SerializeField]
		private LabelAlias _checkBoxLabel;

		[SerializeField]
		private ProgressBar _difficult;

		[SerializeField]
		private LabelAlias _difficultLabel;

		private string EEJJDEHIDEH;

		private string OPHCGFOHKAN;

		private string PHCMDJCMPDF;

		private LabelButton.FBMGEHJPPIK NBJEHGNACHH;

		private LabelButton.FBMGEHJPPIK PHLHOABOODK;

		private LabelButton.FBMGEHJPPIK CKFCBIOHFAF;

		private float _ratio;

		private bool HFCFEKNIEEA;

		private bool LOOHNJPAAHD;

		private bool CJJBDGPDOFF;

		private string IAHHOEJJJHP = string.Empty;

		private Action<object> _dlg;

		public override void Init(object data)
		{
			StrangerDialogInfo gCPCDFIBLGN = (StrangerDialogInfo)data;
			IHMEPGICLGF = gCPCDFIBLGN.CHJHCGODKJM;
			NBJEHGNACHH = gCPCDFIBLGN.IGHFDCLELCO;
			EEJJDEHIDEH = gCPCDFIBLGN.OMPNOCLIPEO;
			PHLHOABOODK = gCPCDFIBLGN.AEBLFMOIEKM;
			OPHCGFOHKAN = gCPCDFIBLGN.GPDFOBPMAAG;
			CKFCBIOHFAF = gCPCDFIBLGN.IOFGJDJFMOD;
			PHCMDJCMPDF = gCPCDFIBLGN.AFFLDJOMBNM;
			_ratio = gCPCDFIBLGN.Ratio;
			HFCFEKNIEEA = gCPCDFIBLGN.PLAFJPIFHHL;
			GIGHCCNJNGA = gCPCDFIBLGN.MOOMLCGKFBA;
			LOOHNJPAAHD = gCPCDFIBLGN.KIGGOAIKFCB;
			CJJBDGPDOFF = gCPCDFIBLGN.HLFPOONJFNM;
			IAHHOEJJJHP = gCPCDFIBLGN.CJKCAIJLFPN;
			if (gCPCDFIBLGN.Dlg != null)
			{
				_dlg = gCPCDFIBLGN.Dlg.Invoke;
				AddEventListener(0, gCPCDFIBLGN.Dlg);
			}
			GlobalTimer.get_Instance().addEventListener(0, ILFBDHDMHPD);
			AKNJEGGNNBJ = gCPCDFIBLGN.GBMEDJJOFBF;
			base.Init(gCPCDFIBLGN.Title, "dlgButtonWait", "dlgStoryBtnGoodbye");
		}

		private new void Start()
		{
			base.Start();
			GJKBCJBPHEE();
			CHFENJOILAB();
		}

		private void OnDestroy()
		{
			GlobalTimer.get_Instance().removeEventListener(0, ILFBDHDMHPD);
		}

		protected override void MAGOIKICKAH(KBDHPMOMJLL IOJJEMLBKOA)
		{
			OPFBDPMJCHH();
			if (LOOHNJPAAHD)
			{
				CGKGDKAGFLI();
				KJHPCLOFDJB();
			}
			if (GIGHCCNJNGA && NLOKJHGPOIF() < 3)
			{
				_btnOK = GetEdgeButton(true);
				LabelButton labelButton = GetEdgeButton(false);
				_btnCancel = ((!(labelButton != _btnOK)) ? null : labelButton);
				CHFENJOILAB();
			}
		}

		protected override void HLJBLAPMDCB()
		{
			base.HLJBLAPMDCB();
			PHGFOLEOFOD();
			if (LOOHNJPAAHD)
			{
				float bAINMLLIKOL = _content.transform.localPosition.y + 50f + 20f;
				_content.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}

		protected override void BHLHODFNHHO()
		{
			base.BHLHODFNHHO();
		}

		protected override void FLOHKIBCOKG()
		{
			base.FLOHKIBCOKG();
			if (LOOHNJPAAHD)
			{
				float bAINMLLIKOL = _topStripe.transform.localPosition.y + 12f;
				_topStripe.transform.BGNJGIACJBG(bAINMLLIKOL);
				bAINMLLIKOL = _bottomStripe.transform.localPosition.y - 12f;
				_bottomStripe.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}

		protected override void CHFENJOILAB()
		{
			base.CHFENJOILAB();
			if (NLOKJHGPOIF() == 3)
			{
				float y = _bottomStripe.transform.localPosition.y;
				float num = y + 130f;
				if (LOOHNJPAAHD)
				{
					num += 130f;
				}
				_storeButton.transform.BGNJGIACJBG(num);
				_acceptButton.transform.BGNJGIACJBG(num);
				_rejectButton.transform.BGNJGIACJBG(num);
			}
			if (LOOHNJPAAHD)
			{
				if (_btnOK.gameObject.activeSelf)
				{
					float bAINMLLIKOL = _btnOK.transform.localPosition.y + 100f + 30f;
					_btnOK.transform.BGNJGIACJBG(bAINMLLIKOL);
				}
				if (_btnCancel != null && _btnCancel.gameObject.activeSelf)
				{
					float bAINMLLIKOL2 = _btnCancel.transform.localPosition.y + 100f + 30f;
					_btnCancel.transform.BGNJGIACJBG(bAINMLLIKOL2);
				}
				if (_checkBox.gameObject.activeSelf)
				{
					float bAINMLLIKOL3 = _bottomStripe.transform.localPosition.y + 90f;
					_checkBox.transform.BGNJGIACJBG(bAINMLLIKOL3);
					_checkBoxLabel.transform.BGNJGIACJBG(bAINMLLIKOL3);
					float num2 = _checkBox.GetComponent<RectTransform>().rect.width / 2f + _checkBoxLabel.preferredWidth;
					_checkBox.transform.OKHPLHPBPKJ((0f - num2) / 2f);
					float num3 = _checkBox.transform.localPosition.x + _checkBox.GetComponent<RectTransform>().rect.width / 2f;
					_checkBoxLabel.transform.OKHPLHPBPKJ(num3 + _checkBoxLabel.preferredWidth / 2f);
				}
			}
		}

		protected override void CGKGDKAGFLI()
		{
			float num = 0f;
			if (_text.get_text() != string.Empty)
			{
				num = _text.preferredHeight;
			}
			else if (_textsSprite.gameObject.activeSelf)
			{
				num += _textsSprite.GetComponent<RectTransform>().rect.height;
			}
			if (_checkBox.gameObject.activeSelf)
			{
				num += _checkBox.GetComponent<RectTransform>().rect.height;
			}
			if (HFCFEKNIEEA)
			{
				num += 240f;
			}
			float b = CJDGAIICNGM * 0.9f * 0.8f;
			float num2 = Mathf.Max(num, b);
			if (_footerTextsSprite.gameObject.activeSelf)
			{
				float num3 = _footerTextsSprite.GetComponent<RectTransform>().rect.height + 60f + 30f;
				num2 += num3;
				_content.transform.BGNJGIACJBG(_content.transform.localPosition.y + num3 / 2f);
			}
			Vector2 sizeDelta = new Vector2(_content.GetComponent<RectTransform>().rect.width, num2);
			_content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}

		public override int GetLeftButtonId()
		{
			return GetEdgeButton(false).ButtonId;
		}

		private void OPFBDPMJCHH()
		{
			if (_btnCancel != null)
			{
				_btnCancel.gameObject.SetActive(false);
			}
			if (_btnOK != null)
			{
				_btnOK.gameObject.SetActive(false);
			}
			PHKIJLEICHE(_rejectButton, NBJEHGNACHH, EEJJDEHIDEH, 0);
			_rejectButton.RemoveEventListener(2, ButtonCallback);
			_rejectButton.AddEventListener(2, ButtonCallback);
			_rejectButton.gameObject.SetActive(EEJJDEHIDEH != string.Empty);
			PHKIJLEICHE(_acceptButton, PHLHOABOODK, OPHCGFOHKAN, 2);
			_acceptButton.RemoveEventListener(2, ButtonCallback);
			_acceptButton.AddEventListener(2, ButtonCallback);
			_acceptButton.gameObject.SetActive(OPHCGFOHKAN != string.Empty);
			PHKIJLEICHE(_storeButton, CKFCBIOHFAF, PHCMDJCMPDF, 1);
			_storeButton.RemoveEventListener(2, ButtonCallback);
			_storeButton.AddEventListener(2, ButtonCallback);
			_storeButton.gameObject.SetActive(PHCMDJCMPDF != string.Empty);
			float num = 0f;
			float bAINMLLIKOL = _bottomStripe.transform.localPosition.y + 130f;
			num = (0f - _acceptButton.GetComponent<RectTransform>().rect.width) / 2f - 40f - _rejectButton.GetComponent<RectTransform>().rect.width / 2f;
			_rejectButton.transform.OKHPLHPBPKJ(num);
			_rejectButton.transform.BGNJGIACJBG(bAINMLLIKOL);
			num = 0f;
			_acceptButton.transform.OKHPLHPBPKJ(num);
			_acceptButton.transform.BGNJGIACJBG(bAINMLLIKOL);
			num = _acceptButton.GetComponent<RectTransform>().rect.width / 2f + 40f + _storeButton.GetComponent<RectTransform>().rect.width / 2f;
			_storeButton.transform.OKHPLHPBPKJ(num);
			_storeButton.transform.BGNJGIACJBG(bAINMLLIKOL);
			if (LOOHNJPAAHD)
			{
				_checkBox.gameObject.SetActive(true);
				_checkBox.isOn = CJJBDGPDOFF;
				_checkBox.onValueChanged.AddListener((bool value) =>
				{
					ButtonCallback(3);
				});
				_checkBox.transform.BGNJGIACJBG(bAINMLLIKOL);
				_checkBoxLabel.gameObject.SetActive(true);
				_checkBoxLabel.set_LabelFontSize(103);
				_checkBoxLabel.color = Constants.PJJIMHMJPAL;
				_checkBoxLabel.set_Alias(IAHHOEJJJHP);
				_checkBoxLabel.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}

		protected override void FALMBFKEGIE()
		{
			BGJJDGOBPKA = "dlgButtonWait";
			base.MAGOIKICKAH(KBDHPMOMJLL.FOOTER_BOTH);
			_acceptButton.gameObject.SetActive(false);
			_rejectButton.gameObject.SetActive(false);
			_storeButton.gameObject.SetActive(false);
			float bAINMLLIKOL = 20f + _btnOK.GetComponent<RectTransform>().rect.width / 2f;
			float bAINMLLIKOL2 = _bottomStripe.transform.localPosition.y + 130f;
			_btnOK.transform.OKHPLHPBPKJ(bAINMLLIKOL);
			_btnOK.transform.BGNJGIACJBG(bAINMLLIKOL2);
			_btnOK.Label.set_LabelFontSize(87);
			_btnOK.ButtonId = 13;
			_btnOK.RemoveEventListener(2, OnClose);
			_btnOK.AddEventListener(2, ButtonCallback);
			bAINMLLIKOL = -20f - _btnCancel.GetComponent<RectTransform>().rect.width / 2f;
			_btnCancel.transform.OKHPLHPBPKJ(bAINMLLIKOL);
			_btnCancel.transform.BGNJGIACJBG(bAINMLLIKOL2);
			_btnCancel.Label.set_LabelFontSize(87);
			_btnCancel.ButtonId = 1;
			_btnCancel.RemoveEventListener(2, OnClose);
			_btnCancel.AddEventListener(2, ButtonCallback);
			CHFENJOILAB();
		}

		protected override void KCDJNNNDJCE()
		{
			_text.set_text(APBAOFGMAAA);
			_text.transform.OKHPLHPBPKJ(_portrait.transform.localPosition.x + 40f + CJDGAIICNGM * 0.9f / 2f + _text.rectTransform.rect.width / 2f);
			_text.transform.BGNJGIACJBG(0f);
			if (_difficult.gameObject.activeSelf)
			{
				_difficult.transform.OKHPLHPBPKJ(280f);
				_difficultLabel.transform.OKHPLHPBPKJ(280f);
				_text.transform.BGNJGIACJBG(70f + _difficultLabel.preferredHeight / 4f);
				_difficult.transform.BGNJGIACJBG(-80f + _text.transform.localPosition.y - _text.preferredHeight / 2f);
				_difficultLabel.transform.BGNJGIACJBG(-60f + _difficult.transform.localPosition.y - _difficultLabel.preferredHeight / 4f);
			}
			JJAMIENHFPJ();
			CGKGDKAGFLI();
			KJHPCLOFDJB();
			CHFENJOILAB();
		}

		private int NLOKJHGPOIF()
		{
			int num = 0;
			if (EEJJDEHIDEH != string.Empty)
			{
				num++;
			}
			if (PHCMDJCMPDF != string.Empty)
			{
				num++;
			}
			if (OPHCGFOHKAN != string.Empty)
			{
				num++;
			}
			return num;
		}

		private LabelButton GetEdgeButton(bool LKHICEPFOMG)
		{
			if (LKHICEPFOMG)
			{
				return _storeButton.gameObject.activeSelf ? _storeButton : (_acceptButton.gameObject.activeSelf ? _acceptButton : ((!_rejectButton.gameObject.activeSelf) ? null : _rejectButton));
			}
			return _rejectButton.gameObject.activeSelf ? _rejectButton : (_acceptButton.gameObject.activeSelf ? _acceptButton : ((!_storeButton.gameObject.activeSelf) ? null : _storeButton));
		}

		private void PHKIJLEICHE(LabelButton GAMILDJHFDB, LabelButton.FBMGEHJPPIK color, string LOKLDPLAPOL, int OKNNNLIPODI)
		{
			GAMILDJHFDB.gameObject.SetActive(true);
			GAMILDJHFDB.SetColor(color);
			GAMILDJHFDB.SetAlias(LOKLDPLAPOL);
			GAMILDJHFDB.ButtonId = OKNNNLIPODI;
		}

		private void PHGFOLEOFOD()
		{
			_difficult.SetValueBorders(0f, 100f);
			_difficult.gameObject.SetActive(HFCFEKNIEEA);
			_difficultLabel.set_Alias(string.Empty);
			_difficultLabel.set_text("???");
			_difficultLabel.set_LabelFontSize(103);
			_difficultLabel.color = Constants.PJJIMHMJPAL;
			_difficultLabel.gameObject.SetActive(HFCFEKNIEEA);
		}

		private void GJKBCJBPHEE()
		{
			if (_ratio < 0f)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			List<global::Pair<string, float>> difficultyEvaluation = DifficultyPanel.get_DifficultyEvaluation();
			global::Pair<string, float> cCKLNOPEKHO = difficultyEvaluation[0];
			foreach (global::Pair<string, float> item in difficultyEvaluation)
			{
				if (item.Second < _ratio && cCKLNOPEKHO.Second < item.Second)
				{
					cCKLNOPEKHO = item;
					num2 = num;
				}
				num++;
			}
			_difficult.Stripe.set_SpriteName(Constants.DNDKOMGCBLC[num2]);
			if (cCKLNOPEKHO != null)
			{
				_difficult.SetValue(100f);
				_difficultLabel.set_Alias(cCKLNOPEKHO.First);
			}
		}

		private void ButtonCallback(object data)
		{
			switch ((int)data)
			{
			case 2:
				OnClose(data);
				break;
			case 0:
				OnClose(data);
				break;
			case 1:
				OnClose(data);
				break;
			case 3:
				if (_dlg != null)
				{
					int num = ((!_checkBox.isOn) ? 4 : 3);
					_dlg(num);
				}
				break;
			}
		}
	}
}
