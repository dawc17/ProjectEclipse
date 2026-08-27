using System.Collections.Generic;
using Nekki.Utils;
using UnityEngine;

namespace Nekki.SF2.GUI.Dialogs
{
	public class StoryDialog : BaseDialog
	{
		private const float JJDELNLKLMI = 120f;

		private const string PRICE_TEXT_BACKGROUND = "ShopPieces.stripe";

		private const int KKPDDHGAECI = 100;

		private const int EGKAICLFKGD = 900;

		private const int BLOJHNNDAHN = -120;

		private const int DPJCJMNHJPA = 120;

		private const int HJJMGMKOMJP = 740;

		private const int LLLAOBBPFHK = 60;

		private const int EHMDBBMBFMJ = 40;

		protected const int EAGOJOBFHBC = 103;

		protected const int LKJMEIKLGPL = 1680;

		private const int PHOFEFIMAEM = 40;

		private const int BJEMKGGJEAP = 1234;

		private const int NFCHOPIJOHF = 20;

		protected float CJDGAIICNGM = GameUtils.FCMBGDFIBPK();

		protected const float OICADIFPCOC = 0.9f;

		protected const float FAPMANOMJDN = 0.8f;

		private const int JPHAHHBOHCA = -500;

		private const int BKNNIPNGDKL = 110;

		public const float FOOTER_TEXT_OFFSET_Y = 60f;

		public const float LINES_PADDING = 32.5f;

		public const float FOOTER_TEXT_DOWN_OFFSET_Y = 30f;

		protected TextTimer OIHKOMFCFME;

		[SerializeField]
		protected LabelAlias _text;

		[SerializeField]
		protected LabelAlias _timeLabel;

		[SerializeField]
		protected TimerLabel _timerLabel;

		[SerializeField]
		protected ResolutionImage _portrait;

		[SerializeField]
		protected GameObject _textsSprite;

		protected float HAPLFCEGEFI = 100f;

		protected int HPBNHCPCOEB;

		protected int LOKIJLFALJP;

		protected bool FCGALLIMMAF = true;

		protected List<StoryDialogContent> IHMEPGICLGF = new List<StoryDialogContent>();

		protected string AKNJEGGNNBJ = string.Empty;

		protected bool ANIJAKJOHED = true;

		protected UserItem NKBIOFJMONB;

		protected RecipeItemInfo DMDLCMBKEHA;

		protected string APBAOFGMAAA = string.Empty;

		protected string MALELNACHFP = string.Empty;

		protected LabelButton.FBMGEHJPPIK OKJBFFAIJPL;

		protected LabelButton.FBMGEHJPPIK JCAOLHHIFEC;

		protected int HCMDPDLJLOO = int.MaxValue;

		protected bool PAPNPKAGDNB;

		protected long _leftTime;

		protected List<StoryDialogContent> LGEKNIKOFMD = new List<StoryDialogContent>();

		[SerializeField]
		protected GameObject _footerTextsSprite;

		protected bool JODDCOBBHMN;

		protected bool GIGHCCNJNGA;

		public override void Init(object data)
		{
			string dIKEFIIPNBE = string.Empty;
			if (data != null)
			{
				StoryDialogInfo gPJMLFBLDEF = (StoryDialogInfo)data;
				dIKEFIIPNBE = gPJMLFBLDEF.Title;
				AKNJEGGNNBJ = gPJMLFBLDEF.GBMEDJJOFBF;
				ANIJAKJOHED = gPJMLFBLDEF.IPOAINACEOB;
				IHMEPGICLGF = gPJMLFBLDEF.CHJHCGODKJM;
				FCGALLIMMAF = gPJMLFBLDEF.MHPGECECDGO;
				MALELNACHFP = gPJMLFBLDEF.KCMBJJDAGHP;
				JCAOLHHIFEC = gPJMLFBLDEF.LEALGLNFFDI;
				EBCJGLPLHAD = gPJMLFBLDEF.PDBEAEIJCBO;
				OKJBFFAIJPL = gPJMLFBLDEF.DFAGEOKEMIE;
				GIGHCCNJNGA = gPJMLFBLDEF.MOOMLCGKFBA;
				if (gPJMLFBLDEF.Dlg != null)
				{
					AddEventListener(0, gPJMLFBLDEF.Dlg);
				}
			}
			GlobalTimer.get_Instance().addEventListener(0, ILFBDHDMHPD);
			base.Init(dIKEFIIPNBE, MALELNACHFP, EBCJGLPLHAD);
		}

		private new void Start()
		{
			base.Start();
			KJHPCLOFDJB();
		}

		private void OnDestroy()
		{
			GlobalTimer.get_Instance().removeEventListener(0, ILFBDHDMHPD);
		}

		public override void Close(object data)
		{
			IPJEOLNMLEH iPJEOLNMLEH = IPJEOLNMLEH.OnPopupClose;
			OnClose(iPJEOLNMLEH);
		}

		protected override void HLJBLAPMDCB()
		{
			HPGFNENGBFI(AKNJEGGNNBJ);
			if (GIGHCCNJNGA)
			{
				GDMKEKGBDGG();
			}
			else
			{
				AJNMAKEIDMH();
			}
			_content.transform.BGNJGIACJBG(20f);
			Vector2 sizeDelta = new Vector2(_content.GetComponent<RectTransform>().rect.width, CJDGAIICNGM * 0.9f * 0.8f);
			_content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}

		protected override void CGKGDKAGFLI()
		{
			float num = 0f;
			if (_text.get_text() != string.Empty)
			{
				num = _text.preferredHeight;
			}
			else if (_textsSprite != null)
			{
				num += _textsSprite.GetComponent<RectTransform>().rect.height;
			}
			float b = CJDGAIICNGM * 0.9f * 0.8f;
			float num2 = Mathf.Max(num, b);
			if (_footerTextsSprite.gameObject.activeSelf)
			{
				num2 += _footerTextsSprite.GetComponent<RectTransform>().rect.height + 32.5f;
			}
			Vector2 sizeDelta = new Vector2(_content.GetComponent<RectTransform>().rect.width, num2);
			_content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}

		protected override void MAGOIKICKAH(KBDHPMOMJLL HJNAHNICGMH)
		{
			if (HPBNHCPCOEB + 1 < IHMEPGICLGF.Count)
			{
				SetButton(IHMEPGICLGF[HPBNHCPCOEB].AJELOOEBCPO);
			}
			if (HPBNHCPCOEB + 1 == IHMEPGICLGF.Count)
			{
				BGJJDGOBPKA = ((!(MALELNACHFP == string.Empty)) ? MALELNACHFP : IHMEPGICLGF[HPBNHCPCOEB].AJELOOEBCPO);
				FALMBFKEGIE();
			}
			else if (IHMEPGICLGF.Count == 0)
			{
				BGJJDGOBPKA = ((!(MALELNACHFP == string.Empty)) ? MALELNACHFP : BGJJDGOBPKA);
				FALMBFKEGIE();
			}
		}

		protected virtual void FALMBFKEGIE()
		{
			KBDHPMOMJLL kBDHPMOMJLL = KBDHPMOMJLL.FOOTER_NONE;
			kBDHPMOMJLL = ((!FCGALLIMMAF) ? KBDHPMOMJLL.FOOTER_OK : KBDHPMOMJLL.FOOTER_BOTH);
			base.MAGOIKICKAH(kBDHPMOMJLL);
			_btnOK.RemoveEventListener(2, OnClose);
			_btnOK.RemoveEventListener(2, GPEKKGLDKDF);
			_btnOK.AddEventListener(2, GPEKKGLDKDF);
			_btnCancel.gameObject.SetActive(FCGALLIMMAF);
			if (FCGALLIMMAF)
			{
				_btnCancel.RemoveEventListener(2, OnClose);
				_btnCancel.AddEventListener(2, MDCHOBEPGCO);
			}
			CHFENJOILAB();
			BHLHODFNHHO();
		}

		protected virtual void HPGFNENGBFI(string LBBHPDDLLOK)
		{
			string[] array = LBBHPDDLLOK.Split('|');
			string[] array2 = LBBHPDDLLOK.Split('/');
			string[] array3 = array2[array2.Length - 1].Split('.');
			_portrait.set_TexturePath(SF2Paths.BHCPOOOJAAK());
			_portrait.set_SpriteName(array3[0]);
			int num = ((array.Length <= 1) ? 1 : (-1));
			_portrait.transform.BGNJGIACJBG(0f);
			_portrait.transform.OKHPLHPBPKJ(-500f);
			_portrait.SetNativeSize();
			_portrait.transform.localScale = new Vector2(1.8f * (float)num, 1.8f);
			_portrait.gameObject.SetActive(ANIJAKJOHED);
		}

		protected virtual void CHFENJOILAB()
		{
			float y = _bottomStripe.transform.localPosition.y;
			float bAINMLLIKOL = y + 120f;
			if (_btnOK != null && _btnOK.gameObject.activeSelf)
			{
				float bAINMLLIKOL2 = 740f - _btnOK.GetComponent<RectTransform>().rect.width / 2f;
				_btnOK.transform.OKHPLHPBPKJ(bAINMLLIKOL2);
				_btnOK.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
			if (_btnCancel != null && _btnCancel.gameObject.activeSelf)
			{
				float num = 0f;
				TransformExtensions.OKHPLHPBPKJ(value: (!_btnOK.gameObject.activeSelf) ? ((1680f - _btnCancel.GetComponent<RectTransform>().rect.width) / 2f - 740f) : (_btnOK.transform.localPosition.x - 60f - (_btnOK.GetComponent<RectTransform>().rect.width + _btnCancel.GetComponent<RectTransform>().rect.width) / 2f), KGOIHPPNFGC: _btnCancel.transform);
				_btnCancel.transform.BGNJGIACJBG(bAINMLLIKOL);
			}
		}

		protected virtual void NBGHLFJPOGM(string LIOGIBJBHAH)
		{
			JODDCOBBHMN = true;
			APBAOFGMAAA = LocalizationManager.GetString(LIOGIBJBHAH);
			_timeLabel.gameObject.SetActive(false);
		}

		protected virtual void NBGHLFJPOGM(StoryDialogContent DMNBDBJNKME)
		{
			KAFHPCAIFNI(DMNBDBJNKME);
			NBGHLFJPOGM(DMNBDBJNKME.GGDJIPKMKFC);
			if (_timerLabel != null)
			{
				_timerLabel.gameObject.SetActive(false);
			}
			int num = -1;
			if (DMNBDBJNKME.MALKNOOGNBA != null)
			{
				num = ELALCENFCPJ.IndexOf(DMNBDBJNKME.MALKNOOGNBA);
				if (num == -1)
				{
					ELALCENFCPJ.Add(DMNBDBJNKME.MALKNOOGNBA);
					_timerLabel = HOPECLFOPLH(DMNBDBJNKME);
					DMNBDBJNKME.MALKNOOGNBA.set_Label(_timerLabel);
					DMNBDBJNKME.MALKNOOGNBA.JLPMOKPFECK();
					num = ELALCENFCPJ.Count - 1;
				}
				_timerLabel = DMNBDBJNKME.MALKNOOGNBA.EDAKEMEHFIC();
			}
			else
			{
				_timerLabel = null;
			}
			bool flag = false;
			foreach (Transform item in _content.transform)
			{
				if (item.gameObject.tag == (1234 + num).ToString())
				{
					flag = true;
					break;
				}
			}
			if ((bool)_timerLabel && !flag)
			{
				_timerLabel.set_LabelFontSize(103);
				_timerLabel.color = DMNBDBJNKME.MALKNOOGNBA.Color;
				_timerLabel.tag = (1234 + num).ToString();
				_timerLabel.transform.SetParent(_content.transform, false);
			}
			if (DMNBDBJNKME.CheckTimer)
			{
				NHAGNALDKCP(DMNBDBJNKME);
			}
		}

		protected virtual void NHAGNALDKCP(StoryDialogContent DMNBDBJNKME)
		{
			PAPNPKAGDNB = true;
			_leftTime = DMNBDBJNKME.Timer;
			HCMDPDLJLOO = DMNBDBJNKME.Id;
			NKBIOFJMONB = DMNBDBJNKME.FGBNJDPGOFN;
			DMDLCMBKEHA = DMNBDBJNKME.KMNGHHBCEGD;
			if (_leftTime <= 0)
			{
				OnClose(HCMDPDLJLOO);
			}
			JJAMIENHFPJ();
		}

		protected virtual void SetButton(string HCPNFPMHFCM)
		{
			BGJJDGOBPKA = HCPNFPMHFCM;
			PHKIJLEICHE(_btnOK, KBDHPMOMJLL.FOOTER_OK);
			_btnOK.RemoveEventListener(2, OnClose);
			_btnOK.RemoveEventListener(2, GPEKKGLDKDF);
			_btnOK.AddEventListener(2, GPEKKGLDKDF);
		}

		protected virtual void ILFBDHDMHPD(object data)
		{
			if (ELALCENFCPJ.Count > 0)
			{
				FFALBJIJIIP(0);
				FOCAHKBJKEK();
			}
			if (PAPNPKAGDNB)
			{
				if (_leftTime <= 0)
				{
					OnClose(HCMDPDLJLOO);
				}
				FLKJAKKMBIP();
				JJAMIENHFPJ();
			}
		}

		private void FLKJAKKMBIP()
		{
			if (NKBIOFJMONB == null)
			{
				if (DMDLCMBKEHA == null)
				{
					_leftTime--;
				}
				else
				{
					_leftTime = GameUtils.GetLeftTime(DMDLCMBKEHA.HGDELDFDFNH());
				}
			}
			else
			{
				_leftTime = GameUtils.GetLeftTime(NKBIOFJMONB.IJGAOHJNLAH());
			}
		}

		protected virtual void AJNMAKEIDMH()
		{
			_timeLabel.set_Alias(string.Empty);
			_timeLabel.set_text(string.Empty);
			_timeLabel.color = Constants.KLLKHFKHCGK;
			_timeLabel.set_LabelFontSize(103);
			_timeLabel.gameObject.SetActive(false);
			_timeLabel.transform.SetParent(_content.transform, false);
			int count = IHMEPGICLGF.Count;
			if (count > 0)
			{
				NBGHLFJPOGM(IHMEPGICLGF[0]);
			}
		}

		protected virtual void KAFHPCAIFNI(StoryDialogContent DMNBDBJNKME)
		{
			_text.gameObject.SetActive(true);
			_text.set_Alias(string.Empty);
			_text.set_text(string.Empty);
			_text.set_LabelFontSize(103);
			_text.color = DMNBDBJNKME.FontColor;
			_text.alignment = TextAnchor.MiddleLeft;
			_text.transform.BGNJGIACJBG(0f);
			float x = ((!ANIJAKJOHED) ? 1680 : 900);
			_text.rectTransform.sizeDelta = new Vector2(x, _text.rectTransform.rect.height);
		}

		protected virtual void GPEKKGLDKDF(object data)
		{
			HPBNHCPCOEB++;
			if (HPBNHCPCOEB == IHMEPGICLGF.Count - 1)
			{
				NBGHLFJPOGM(IHMEPGICLGF[HPBNHCPCOEB]);
				BGJJDGOBPKA = ((!(MALELNACHFP == string.Empty)) ? MALELNACHFP : IHMEPGICLGF[HPBNHCPCOEB].AJELOOEBCPO);
				FALMBFKEGIE();
			}
			else if (HPBNHCPCOEB >= IHMEPGICLGF.Count)
			{
				OnClose(data);
			}
			else
			{
				NBGHLFJPOGM(IHMEPGICLGF[HPBNHCPCOEB]);
				SetButton(IHMEPGICLGF[HPBNHCPCOEB].AJELOOEBCPO);
				CHFENJOILAB();
				BHLHODFNHHO();
			}
		}

		protected void MDCHOBEPGCO(object data)
		{
			base.OnClose(data);
		}

		private void Update()
		{
			if (JODDCOBBHMN)
			{
				JODDCOBBHMN = false;
				KCDJNNNDJCE();
			}
		}

		protected virtual void KCDJNNNDJCE()
		{
			string text = APBAOFGMAAA;
			if (_timerLabel != null)
			{
				text += _timerLabel.get_text();
			}
			_text.set_text(text);
			_text.transform.BGNJGIACJBG(0f);
			if (ANIJAKJOHED)
			{
				_text.transform.OKHPLHPBPKJ(_portrait.transform.localPosition.x + 40f + CJDGAIICNGM * 0.9f / 2f + _text.rectTransform.rect.width / 2f);
			}
			else
			{
				_text.transform.OKHPLHPBPKJ(0f);
			}
			if (_timerLabel != null)
			{
				_timerLabel.gameObject.SetActive(true);
			}
			JJAMIENHFPJ();
			CGKGDKAGFLI();
			KJHPCLOFDJB();
			CHFENJOILAB();
		}

		protected virtual void JJAMIENHFPJ()
		{
			if (!(_timeLabel == null) && PAPNPKAGDNB)
			{
				bool aNLFBBLJMJH = true;
				string timeString = TimerLabel.GetTimeString(_leftTime, true, true, true, aNLFBBLJMJH, ":", string.Empty, true, true, true, true, true, string.Empty, string.Empty, string.Empty);
				_timeLabel.set_text(timeString);
				_timeLabel.gameObject.SetActive(true);
				if (!_text)
				{
					_timeLabel.transform.OKHPLHPBPKJ(_portrait.transform.localPosition.x + 40f + CJDGAIICNGM * 0.9f / 2f + _timeLabel.preferredWidth / 2f);
				}
				else if (!(_text.preferredWidth + _timeLabel.preferredWidth < 900f))
				{
					_text.transform.BGNJGIACJBG(40f);
					_timeLabel.transform.OKHPLHPBPKJ(_portrait.transform.localPosition.x + 40f + CJDGAIICNGM * 0.9f / 2f + _timeLabel.preferredWidth / 2f);
					_timeLabel.transform.BGNJGIACJBG(_text.transform.localPosition.y - _text.preferredHeight / 2f - 40f);
				}
			}
		}

		protected virtual void MFEGIBHOLDI()
		{
			_footerTextsSprite.gameObject.SetActive(true);
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			foreach (StoryDialogContent item in LGEKNIKOFMD)
			{
				GameObject gameObject = new GameObject("LabelAlias");
				LabelAlias labelAlias = gameObject.AddComponent<LabelAlias>();
				labelAlias.set_LabelFontSize(103);
				labelAlias.UseLabelLineSpacing = true;
				labelAlias.set_LabelLineSpacing(0.7f);
				labelAlias.color = item.FontColor;
				labelAlias.alignment = TextAnchor.MiddleCenter;
				labelAlias.alignByGeometry = true;
				labelAlias.verticalOverflow = VerticalWrapMode.Overflow;
				labelAlias.rectTransform.sizeDelta = new Vector2(1680f, 10f);
				labelAlias.set_text(LocalizationManager.GetString(item.GGDJIPKMKFC));
				string text = LocalizationManager.GetString(item.GGDJIPKMKFC);
				bool flag = null != item.MALKNOOGNBA;
				TimerLabel timerLabel = null;
				if (flag)
				{
					ELALCENFCPJ.Add(item.MALKNOOGNBA);
					timerLabel = HOPECLFOPLH(item);
					item.MALKNOOGNBA.set_Label(timerLabel);
					item.MALKNOOGNBA.JLPMOKPFECK();
					timerLabel.transform.SetParent(_footerTextsSprite.transform, false);
					text = text + "<visible=0>" + timerLabel.get_text() + "</>";
				}
				labelAlias.set_text(text);
				if (num3 > 0)
				{
					num2 += 32.5f;
				}
				num -= labelAlias.preferredHeight / 2f;
				labelAlias.transform.SetParent(_footerTextsSprite.transform, false);
				labelAlias.transform.BGNJGIACJBG(num);
				labelAlias.transform.OKHPLHPBPKJ(0f);
				num -= labelAlias.preferredHeight / 2f + 32.5f;
				if (flag)
				{
					timerLabel.gameObject.SetActive(true);
				}
				GameObject gameObject2 = new GameObject("PriceBackground");
				ResolutionImage resolutionImage = gameObject2.AddComponent<ResolutionImage>();
				resolutionImage.set_SpriteName("ShopPieces.stripe");
				resolutionImage.transform.SetParent(_footerTextsSprite.transform, false);
				resolutionImage.SetNativeSize();
				resolutionImage.transform.OKHPLHPBPKJ(labelAlias.transform.localPosition.x);
				resolutionImage.transform.BGNJGIACJBG(labelAlias.transform.localPosition.y);
				float width = resolutionImage.rectTransform.rect.width;
				float width2 = labelAlias.rectTransform.rect.width;
				if (width < width2 + 240f)
				{
					float x = width2 + 240f;
					resolutionImage.rectTransform.sizeDelta = new Vector2(x, resolutionImage.rectTransform.rect.height);
				}
				num2 += resolutionImage.rectTransform.rect.height;
				num3++;
			}
			_footerTextsSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(1680f, num2);
			float a = _portrait.transform.localPosition.y - CJDGAIICNGM * 0.9f * 0.8f / 2f;
			float b = _textsSprite.transform.localPosition.y - _textsSprite.GetComponent<RectTransform>().rect.height;
			float num4 = Mathf.Min(a, b);
			_footerTextsSprite.transform.BGNJGIACJBG(num4 - 60f);
		}

		protected override void PHKIJLEICHE(LabelButton GAMILDJHFDB, KBDHPMOMJLL MOPOCBKIKBI)
		{
			GAMILDJHFDB.gameObject.SetActive(true);
			string alias = string.Empty;
			int buttonId = 0;
			LabelButton.FBMGEHJPPIK color = LabelButton.FBMGEHJPPIK.BUTTON_WHITE;
			switch (MOPOCBKIKBI)
			{
			case KBDHPMOMJLL.FOOTER_CANCEL:
				alias = EBCJGLPLHAD;
				color = OKJBFFAIJPL;
				buttonId = 0;
				break;
			case KBDHPMOMJLL.FOOTER_OK:
				alias = BGJJDGOBPKA;
				color = JCAOLHHIFEC;
				buttonId = 1;
				break;
			}
			GAMILDJHFDB.SetColor(color);
			GAMILDJHFDB.SetAlias(alias);
			GAMILDJHFDB.ButtonId = buttonId;
			GAMILDJHFDB.RemoveEventListener(2, OnClose);
			GAMILDJHFDB.AddEventListener(2, OnClose);
		}

		protected virtual void GDMKEKGBDGG()
		{
			float num = 0f;
			_textsSprite.gameObject.SetActive(true);
			float x = ((!ANIJAKJOHED) ? 1680 : 900);
			float num2 = 0f;
			int i = 0;
			for (int count = IHMEPGICLGF.Count; i < count; i++)
			{
				StoryDialogContent nJEPNCJLPPF = IHMEPGICLGF[i];
				if (!nJEPNCJLPPF.CheckTimer)
				{
					switch (nJEPNCJLPPF.NGEPEDCCMAI)
					{
					case StoryDialogContent.MFHMNFAPAOH.CONTENT_TYPE_REGULAR:
					{
						GameObject gameObject = new GameObject("LabelAlias");
						LabelAlias labelAlias = gameObject.AddComponent<LabelAlias>();
						labelAlias.set_LabelFontSize(103);
						labelAlias.UseLabelLineSpacing = true;
						labelAlias.set_LabelLineSpacing(0.7f);
						labelAlias.color = nJEPNCJLPPF.FontColor;
						labelAlias.alignment = TextAnchor.MiddleLeft;
						labelAlias.alignByGeometry = true;
						labelAlias.verticalOverflow = VerticalWrapMode.Overflow;
						labelAlias.rectTransform.sizeDelta = new Vector2(x, 10f);
						string text = LocalizationManager.GetString(nJEPNCJLPPF.GGDJIPKMKFC);
						bool flag = null != nJEPNCJLPPF.MALKNOOGNBA;
						TimerLabel timerLabel = null;
						if (flag)
						{
							ELALCENFCPJ.Add(nJEPNCJLPPF.MALKNOOGNBA);
							timerLabel = HOPECLFOPLH(nJEPNCJLPPF);
							nJEPNCJLPPF.MALKNOOGNBA.set_Label(timerLabel);
							nJEPNCJLPPF.MALKNOOGNBA.JLPMOKPFECK();
							timerLabel.transform.SetParent(_textsSprite.transform, false);
							text += "<visible=0>00:00:00</>";
						}
						labelAlias.set_text(text);
						labelAlias.transform.SetParent(_textsSprite.transform, false);
						num -= labelAlias.preferredHeight / 2f;
						labelAlias.transform.BGNJGIACJBG(num);
						labelAlias.transform.OKHPLHPBPKJ(_portrait.transform.localPosition.x + 40f + CJDGAIICNGM * 0.9f / 2f + labelAlias.rectTransform.rect.width / 2f);
						num -= labelAlias.preferredHeight / 2f + 32.5f;
						num2 += labelAlias.preferredHeight + 32.5f;
						if (flag)
						{
							timerLabel.gameObject.SetActive(true);
						}
						break;
					}
					case StoryDialogContent.MFHMNFAPAOH.CONTENT_TYPE_PRICELINE:
						LGEKNIKOFMD.Add(nJEPNCJLPPF);
						break;
					}
				}
				else
				{
					_timeLabel.set_Alias(string.Empty);
					_timeLabel.set_text(string.Empty);
					_timeLabel.color = Constants.KLLKHFKHCGK;
					_timeLabel.set_LabelFontSize(103);
					_timeLabel.gameObject.SetActive(false);
					_timeLabel.transform.SetParent(_textsSprite.transform, false);
					NHAGNALDKCP(nJEPNCJLPPF);
					num -= _timeLabel.preferredHeight / 4f;
					_timeLabel.transform.BGNJGIACJBG(num);
					num -= _timeLabel.preferredHeight / 4f + 32.5f;
					num2 += _timeLabel.preferredHeight;
				}
			}
			IHMEPGICLGF.Clear();
			_textsSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(x, num2);
			_textsSprite.transform.BGNJGIACJBG(-20f - num / 2f);
			if (LGEKNIKOFMD.Count > 0)
			{
				MFEGIBHOLDI();
			}
		}

		protected virtual TimerLabel HOPECLFOPLH(StoryDialogContent DMNBDBJNKME)
		{
			GameObject gameObject = new GameObject("TimerLabel");
			TimerLabel timerLabel = gameObject.AddComponent<TimerLabel>();
			timerLabel.IsSeconds = DMNBDBJNKME.MALKNOOGNBA.CBCBKMHGLEF;
			timerLabel.IsMinutes = DMNBDBJNKME.MALKNOOGNBA.PMNNBLHOCPH;
			timerLabel.IsHours = DMNBDBJNKME.MALKNOOGNBA.KFMCBOHHFNH;
			timerLabel.IsDays = DMNBDBJNKME.MALKNOOGNBA.DAPJNFEGFJL;
			timerLabel.Delimiter = DMNBDBJNKME.MALKNOOGNBA.HNECCLNDKJL;
			timerLabel.DaysString = DMNBDBJNKME.MALKNOOGNBA.DEJKIIKMGAO;
			timerLabel.UseDaysDelimiter = DMNBDBJNKME.MALKNOOGNBA.NPODIGENMMO;
			timerLabel.IsSecondsZero = DMNBDBJNKME.MALKNOOGNBA.LHOKGJNFELC;
			timerLabel.IsMinutesZero = DMNBDBJNKME.MALKNOOGNBA.CLFKEPDADAM;
			timerLabel.IsHoursZero = DMNBDBJNKME.MALKNOOGNBA.HAFGMOFEJGI;
			timerLabel.IsDaysZero = DMNBDBJNKME.MALKNOOGNBA.DNLOOJOACNE;
			timerLabel.set_LabelFontSize(103);
			timerLabel.color = DMNBDBJNKME.MALKNOOGNBA.Color;
			return timerLabel;
		}

		protected List<string> ParseString(string IGGFGLLIGCG)
		{
			int length = IGGFGLLIGCG.Length;
			int num = length;
			string empty = string.Empty;
			string empty2 = string.Empty;
			for (int i = 110; i < length; i++)
			{
				if (IGGFGLLIGCG[i] == ' ')
				{
					num = i + 1;
					break;
				}
			}
			empty.Insert(0, IGGFGLLIGCG.Substring(0, num));
			empty2.Insert(0, IGGFGLLIGCG.Substring(num));
			List<string> list = new List<string>();
			list.Add(empty);
			if (empty2 != string.Empty)
			{
				list.Add(empty2);
			}
			return list;
		}

		protected virtual void FOCAHKBJKEK()
		{
			foreach (TextTimer item in ELALCENFCPJ)
			{
				if (item.Time <= 0)
				{
					OnClose(0);
					break;
				}
			}
		}
	}
}
