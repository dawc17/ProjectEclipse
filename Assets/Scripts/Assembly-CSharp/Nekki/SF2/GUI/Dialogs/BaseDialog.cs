using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Dialogs
{
	public class BaseDialog : SFMonoBehaviour<object>, BackKeyController
	{
		public enum IPJEOLNMLEH
		{
			OnPopupClose = 0,
			OnPopupCloseOK = 1,
			OnPopupCloseCascade = 2
		}

		public enum KBDHPMOMJLL
		{
			FOOTER_NONE = 0,
			FOOTER_OK = 1,
			FOOTER_CANCEL = 2,
			FOOTER_BOTH = 3
		}

		public const int BASE_TEXT_FONT_SIZE = 103;

		public const int BASE_LINE_HEIGHT = 180;

		private const int JNBAAMAKFJJ = 64;

		private const int KHCBFOBKIDF = 86;

		private const int KENGHLNFHBD = 100;

		private const int IBNKNFCOHMM = 64;

		private const int JJFGJMBLINP = 272;

		private const int PGFJCKPKNJN = 544;

		private const int HIIJFINAAJK = 60;

		private const int ADLDBOPPCIO = 152;

		protected List<Button> _btns = new List<Button>();

		protected List<TextTimer> ELALCENFCPJ = new List<TextTimer>();

		[SerializeField]
		protected ResolutionImage _topStripe;

		[SerializeField]
		protected ResolutionImage _bottomStripe;

		[SerializeField]
		protected GameObject _content;

		[SerializeField]
		protected LabelAlias _header;

		[SerializeField]
		protected LabelButton _btnCancel;

		[SerializeField]
		protected LabelButton _btnOK;

		protected string ODLPOMFLOCP = string.Empty;

		protected string BGJJDGOBPKA = string.Empty;

		protected string EBCJGLPLHAD = string.Empty;

		protected bool PJMJIOICBMN;

		protected static int KPMBLKKFMAI;

		protected bool PLBDNCCGLIA;

		protected float _fadeDuration = 1f;

		public bool IsIgnoreBack;

		public bool IsQuestDialog;

		protected KBDHPMOMJLL GBECKKCHAFI;

		public bool IsPausing = true;

		public bool TopMenuIsActive;

		public LabelAlias Title
		{
			get
			{
				return get_Header();
			}
		}

		public LabelButton OAKHBCBKCMN
		{
			get
			{
				return get_ButtonCancel();
			}
		}

		public LabelButton MBKMPHMFMOG
		{
			get
			{
				return get_ButtonOK();
			}
		}

		public LabelAlias get_Header()
		{
			return _header;
		}

		public LabelButton get_ButtonCancel()
		{
			return _btnCancel;
		}

		public LabelButton get_ButtonOK()
		{
			return _btnOK;
		}

		public virtual void Init(object data)
		{
			Init(string.Empty);
		}

		public virtual void Init(string DIKEFIIPNBE = "", string EHMEFCPIODJ = "OK", string EOCPGMKEEHK = "CANCEL", KBDHPMOMJLL HJNAHNICGMH = KBDHPMOMJLL.FOOTER_NONE)
		{
			ODLPOMFLOCP = DIKEFIIPNBE;
			BGJJDGOBPKA = EHMEFCPIODJ;
			EBCJGLPLHAD = EOCPGMKEEHK;
			GBECKKCHAFI = HJNAHNICGMH;
			BackKeyManager.get_Instance().AddBackKeyController(this);
		}

		protected virtual void Start()
		{
			HLJBLAPMDCB();
			CGKGDKAGFLI();
			FLOHKIBCOKG();
			SetupHeader(ODLPOMFLOCP);
			MAGOIKICKAH(GBECKKCHAFI);
			if (AssemblyController.KMEOEAGGPBI())
			{
				BHLHODFNHHO();
			}
		}

		private void OnDestroy()
		{
			foreach (TextTimer item in ELALCENFCPJ)
			{
				item.set_Label(null);
			}
		}

		public virtual void Close(object data)
		{
			IPJEOLNMLEH iPJEOLNMLEH = IPJEOLNMLEH.OnPopupCloseOK;
			OnClose(iPJEOLNMLEH);
		}

		public virtual void OnClose(object data)
		{
			base.gameObject.SetActive(false);
			// Release the global raycaster lock before purchase/upgrade callbacks
			// rebuild and refocus the shop UI.  If a callback opens another dialog,
			// that dialog will establish its own lock normally.
			DialogsManager.ELEBLBJKDBI().StopDialog(this);
			BackKeyManager.get_Instance().RemoveBackKeyController(this);
			try
			{
				CallEvent(0, data);
			}
			finally
			{
				LNJOJHJJPOM();
			}
		}

		public virtual void OnBackKeyClicked(object data)
		{
			if (!IsIgnoreBack)
			{
				int leftButtonId = GetLeftButtonId();
				OnClose(leftButtonId);
			}
		}

		public virtual int GetLeftButtonId()
		{
			IPJEOLNMLEH result = IPJEOLNMLEH.OnPopupCloseOK;
			if (_btnCancel != null)
			{
				result = (IPJEOLNMLEH)_btnCancel.ButtonId;
			}
			else if (_btnOK != null)
			{
				result = (IPJEOLNMLEH)_btnOK.ButtonId;
			}
			return (int)result;
		}

		protected virtual void HLJBLAPMDCB()
		{
		}

		protected virtual void FLOHKIBCOKG()
		{
			float num = FFIJLPAAJKB();
			if (_topStripe != null)
			{
				_topStripe.transform.BGNJGIACJBG(150f + num);
			}
			if (_bottomStripe != null)
			{
				_bottomStripe.transform.BGNJGIACJBG(0f - (160f + num));
			}
		}

		protected virtual void MAGOIKICKAH(KBDHPMOMJLL HJNAHNICGMH)
		{
			bool flag = HJNAHNICGMH == KBDHPMOMJLL.FOOTER_BOTH;
			_btns.Clear();
			if (flag || HJNAHNICGMH == KBDHPMOMJLL.FOOTER_OK)
			{
				PHKIJLEICHE(_btnOK, KBDHPMOMJLL.FOOTER_OK);
				_btnOK.transform.OKHPLHPBPKJ((!flag) ? 0f : (_btnOK.get_rect().width / 2f + 32f));
				_btns.Add(_btnOK);
			}
			else
			{
				_btnOK.gameObject.SetActive(false);
			}
			if (flag || HJNAHNICGMH == KBDHPMOMJLL.FOOTER_CANCEL)
			{
				PHKIJLEICHE(_btnCancel, KBDHPMOMJLL.FOOTER_CANCEL);
				_btnCancel.transform.OKHPLHPBPKJ((!flag) ? 0f : (0f - (_btnCancel.get_rect().width / 2f + 32f)));
				_btns.Add(_btnCancel);
			}
			else
			{
				_btnCancel.gameObject.SetActive(false);
			}
			NCDPGDINBPH();
		}

		protected virtual void NCDPGDINBPH()
		{
			float num = FFIJLPAAJKB() + 60f;
			foreach (LabelButton item in _btns)
			{
				item.transform.BGNJGIACJBG(0f - num);
			}
		}

		protected virtual void PHKIJLEICHE(LabelButton GAMILDJHFDB, KBDHPMOMJLL MOPOCBKIKBI)
		{
			GAMILDJHFDB.gameObject.SetActive(true);
			string alias = string.Empty;
			int buttonId = 0;
			LabelButton.FBMGEHJPPIK color = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;
			switch (MOPOCBKIKBI)
			{
			case KBDHPMOMJLL.FOOTER_CANCEL:
				alias = EBCJGLPLHAD;
				color = LabelButton.FBMGEHJPPIK.BUTTON_DARK;
				buttonId = 0;
				break;
			case KBDHPMOMJLL.FOOTER_OK:
				alias = BGJJDGOBPKA;
				color = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;
				buttonId = 1;
				break;
			}
			GAMILDJHFDB.SetColor(color);
			GAMILDJHFDB.SetAlias(alias);
			GAMILDJHFDB.ButtonId = buttonId;
			GAMILDJHFDB.RemoveEventListener(2, OnClose);
			GAMILDJHFDB.AddEventListener(2, OnClose);
			GAMILDJHFDB.transform.OKHPLHPBPKJ(0f);
		}

		protected virtual void CGKGDKAGFLI()
		{
			if (_content.transform.childCount == 0)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			foreach (Transform item in _content.transform)
			{
				float y = item.transform.localPosition.y;
				float y2 = item.transform.localScale.y;
				float num3 = item.GetComponent<RectTransform>().rect.height;
				if (item.GetComponent<Text>() != null)
				{
					num3 = item.GetComponent<Text>().preferredHeight;
				}
				if (y + num3 / 2f > num)
				{
					num = y + num3 / 2f * y2;
				}
				if (y - num3 / 2f < num2)
				{
					num2 = y - num3 / 2f * y2;
				}
			}
			Vector2 sizeDelta = new Vector2(_content.GetComponent<RectTransform>().rect.width, 2f * Mathf.Max((!(num < 0f)) ? num : (0f - num), (!(num2 < 0f)) ? num2 : (0f - num2)));
			_content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}

		protected virtual void LNJOJHJJPOM()
		{
			Object.Destroy(base.gameObject);
		}

		protected virtual void BHLHODFNHHO()
		{
		}

		protected virtual float FFIJLPAAJKB()
		{
			if (_content == null)
			{
				return 272f;
			}
			float value = _content.GetComponent<RectTransform>().rect.height / 2f + 60f;
			return Mathf.Clamp(value, 272f, 544f);
		}

		protected virtual void SetupHeader(string HCPNFPMHFCM)
		{
			_header.set_LabelFontSize(152);
			_header.color = Constants.KLLKHFKHCGK;
			_header.set_Alias(HCPNFPMHFCM);
			float x = _content.GetComponent<RectTransform>().rect.width - 120f;
			_header.rectTransform.sizeDelta = new Vector2(x, _header.rectTransform.rect.height);
			UpdateHeaderPosition();
		}

		public virtual void UpdateHeaderPosition()
		{
			float num = FFIJLPAAJKB();
			_header.transform.BGNJGIACJBG(num + 64f);
		}

		protected virtual void KJHPCLOFDJB()
		{
			FLOHKIBCOKG();
			UpdateHeaderPosition();
			NCDPGDINBPH();
		}

		protected virtual void FFALBJIJIIP(object data)
		{
			foreach (TextTimer item in ELALCENFCPJ)
			{
				item.JLPMOKPFECK();
			}
		}
	}
}
