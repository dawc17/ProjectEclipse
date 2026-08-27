using System;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class PerkContent : Content
	{
		public enum NIFHEMGLNKI
		{
			zText = 0,
			zIcon = 1
		}

		private const int BFGKMIDOEFC = 88;

		private const int CKJIDMCICGK = 70;

		private const int KLHCBMNAGGC = 104;

		private const float OAKHGJGOGHE = -294f;

		private const float POHALNJGPLL = -294f;

		private const int BIOPJCPBOCK = 110;

		[SerializeField]
		private GameObject _keys;

		[SerializeField]
		private LabelAlias _textLabel;

		[SerializeField]
		private LabelButton _btnImprove;

		[SerializeField]
		private LabelAlias _lblImprove;

		private Action<object> FNOECGMEKGL;

		private InfoAnimation BJONHDGCNFE;

		private string _text;

		private string ADDKGJGCBMB;

		private string CBCBPBGPCCA;

		private ProfilePerk.KMHBPKKCNPP MAFFNGPOMJD;

		private bool OBGMJMBNMME;

		private float ECCMHGEEFLE;

		private float GEFOLNHPJMI;

		private void Start()
		{
			_btnImprove.onClick.AddListener(KNDBFAFPBPK);
		}

		public void Init(string HCPNFPMHFCM, ProfilePerk.KMHBPKKCNPP state, Action<object> ODDEOFKLIAG = null, InfoAnimation HCBDNEOKGNK = null, float JMLAKAKDBBL = -1f)
		{
			_text = HCPNFPMHFCM;
			FNOECGMEKGL = ODDEOFKLIAG;
			MAFFNGPOMJD = state;
			GEFOLNHPJMI = JMLAKAKDBBL;
			BJONHDGCNFE = HCBDNEOKGNK;
			ADDKGJGCBMB = "profile_BtnImprove";
			CBCBPBGPCCA = "profile_LblImprove";
			HeaderFontSize = 88;
			PHKIJLEICHE();
			DECAGHCLJJI();
			AJNMAKEIDMH();
			CPHNDCNNHOH();
		}

		public override void SetUpBorder(float BGEEALIPKCC)
		{
			ECCMHGEEFLE = BGEEALIPKCC;
			OBGMJMBNMME = true;
		}

		public LabelButton GetBtnImprove()
		{
			return _btnImprove;
		}

		private void AJNMAKEIDMH()
		{
			_textLabel.color = Constants.PJJIMHMJPAL;
			_textLabel.rectTransform.sizeDelta = new Vector2(GEFOLNHPJMI, _textLabel.rectTransform.rect.height);
			_textLabel.set_LabelFontSize(70);
			_textLabel.set_Alias(_text);
			_lblImprove.color = Constants.PJJIMHMJPAL;
			_lblImprove.rectTransform.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width - 120f, _lblImprove.rectTransform.rect.height);
			_lblImprove.set_LabelFontSize(104);
			_lblImprove.set_Alias(CBCBPBGPCCA);
			_lblImprove.transform.OKHPLHPBPKJ(0f);
			_lblImprove.transform.BGNJGIACJBG(-294f);
			if (MAFFNGPOMJD == ProfilePerk.KMHBPKKCNPP.PERK_SELECTED)
			{
				_lblImprove.gameObject.SetActive(true);
			}
			else
			{
				_lblImprove.gameObject.SetActive(false);
			}
		}

		private void CPHNDCNNHOH()
		{
			_textLabel.rectTransform.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width - 120f, _textLabel.rectTransform.rect.height);
			_lblImprove.rectTransform.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width - 120f, _lblImprove.rectTransform.rect.height);
			float bAINMLLIKOL = 0f;
			if (OBGMJMBNMME)
			{
				float eCCMHGEEFLE = ECCMHGEEFLE;
				float num = _btnImprove.transform.localPosition.y + _btnImprove.GetComponent<RectTransform>().rect.height / 2f;
				bAINMLLIKOL = eCCMHGEEFLE - (eCCMHGEEFLE - num) / 2f;
			}
			_lblImprove.transform.OKHPLHPBPKJ(0f);
			_lblImprove.transform.BGNJGIACJBG(-294f);
			if (_keys.gameObject.activeSelf)
			{
				_keys.transform.BGNJGIACJBG(bAINMLLIKOL);
				_textLabel.gameObject.SetActive(false);
			}
			else
			{
				_textLabel.gameObject.SetActive(true);
			}
		}

		private void PHKIJLEICHE()
		{
			_btnImprove.gameObject.SetActive(true);
			_btnImprove.SetAlias(ADDKGJGCBMB);
			if (MAFFNGPOMJD == ProfilePerk.KMHBPKKCNPP.PERK_LOCK || MAFFNGPOMJD == ProfilePerk.KMHBPKKCNPP.PERK_SELECTED || MAFFNGPOMJD == ProfilePerk.KMHBPKKCNPP.PERK_UNAVAILABLE)
			{
				_btnImprove.gameObject.SetActive(false);
			}
			_btnImprove.transform.OKHPLHPBPKJ(0f);
			_btnImprove.transform.BGNJGIACJBG(-294f);
		}

		private void DECAGHCLJJI()
		{
			foreach (Transform item in _keys.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
				_keys.gameObject.SetActive(false);
			}
			if (BJONHDGCNFE == null)
			{
				return;
			}
			_keys.gameObject.SetActive(true);
			float num = 0f;
			KeyData fONEJOKEIEN = BJONHDGCNFE.ILBCHANCOBP().FONEJOKEIEN;
			for (int i = 0; i < fONEJOKEIEN.CEPODJDDLBF.Count; i++)
			{
				ResolutionImage keyIcon = GetKeyIcon(fONEJOKEIEN.CEPODJDDLBF[i]);
				if (keyIcon != null)
				{
					keyIcon.transform.SetParent(_keys.transform, false);
					keyIcon.transform.OKHPLHPBPKJ(num);
					num += 110f;
				}
			}
			if (fONEJOKEIEN.CEPODJDDLBF.Count > 0)
			{
				GameObject gameObject = new GameObject("KeyIcon");
				ResolutionImage resolutionImage = gameObject.AddComponent<ResolutionImage>();
				resolutionImage.set_SpriteName("ComboButtons.icon_plus");
				resolutionImage.SetNativeSize();
				resolutionImage.transform.SetParent(_keys.transform, false);
				resolutionImage.transform.OKHPLHPBPKJ(num);
				num += 110f;
			}
			for (int j = 0; j < fONEJOKEIEN.IGEEOAGOMEM.Count; j++)
			{
				ResolutionImage keyIcon2 = GetKeyIcon(fONEJOKEIEN.IGEEOAGOMEM[j]);
				if (keyIcon2 != null)
				{
					keyIcon2.transform.SetParent(_keys.transform, false);
					keyIcon2.transform.OKHPLHPBPKJ(num);
					num += 110f;
				}
			}
			_keys.transform.OKHPLHPBPKJ((0f - (num - 110f)) / 2f);
		}

		public static ResolutionImage GetKeyIcon(int PONDIGKAALH)
		{
			ResolutionImage resolutionImage = null;
			string spriteName = string.Empty;
			float num = 0f;
			switch (PONDIGKAALH)
			{
			case 5:
				spriteName = "ComboButtons.icon_left";
				num = 270f;
				break;
			case 10:
				spriteName = "ComboButtons.icon_kick";
				break;
			case 7:
				spriteName = "ComboButtons.icon_left";
				break;
			case 9:
				spriteName = "ComboButtons.icon_punch";
				break;
			case 3:
				spriteName = "ComboButtons.icon_left";
				num = 180f;
				break;
			case 1:
				spriteName = "ComboButtons.icon_left";
				num = 90f;
				break;
			case 8:
				spriteName = "ComboButtons.icon_left";
				num = 45f;
				break;
			case 2:
				spriteName = "ComboButtons.icon_left";
				num = 135f;
				break;
			case 6:
				spriteName = "ComboButtons.icon_left";
				num = 315f;
				break;
			case 4:
				spriteName = "ComboButtons.icon_left";
				num = 225f;
				break;
			default:
				LLLOJBFMONN.Error("PerkContent::getKeyIcon - unknown type: %i", (FightCID)PONDIGKAALH);
				break;
			}
			GameObject gameObject = new GameObject("KeyIcon");
			resolutionImage = gameObject.AddComponent<ResolutionImage>();
			resolutionImage.set_SpriteName(spriteName);
			resolutionImage.SetNativeSize();
			resolutionImage.raycastTarget = false;
			if (resolutionImage != null)
			{
				resolutionImage.transform.localEulerAngles = new Vector3(0f, 0f, 0f - num);
			}
			return resolutionImage;
		}

		private void KNDBFAFPBPK()
		{
			if (FNOECGMEKGL != null)
			{
				FNOECGMEKGL(null);
			}
		}
	}
}
