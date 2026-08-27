using System.Collections.Generic;
using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class TrickSubItem : SubItem
	{
		public enum KHPHEEBBCPI
		{
			onTrickShow = 12
		}

		private const int BIOPJCPBOCK = 110;

		private const int GMIGELNEIBI = 100;

		private const int IMDHKABNOKK = 110;

		protected Trick DGDDKFKEDNO;

		private InfoAnimation BJONHDGCNFE;

		private TrickInfo NFJICFOAOPH;

		private bool MIIEANDEPHO;

		[SerializeField]
		private LabelAlias _keysDescription;

		[SerializeField]
		private GameObject _keys;

		public void Init(string KHPKDMGDMAB, Trick JECLDALKMKA, int OKNNNLIPODI)
		{
			Init(OKNNNLIPODI);
			_keys.gameObject.SetActive(false);
			_keysDescription.gameObject.SetActive(false);
			DGDDKFKEDNO = JECLDALKMKA;
			GJPJJHACOJJ = KHPKDMGDMAB;
			BHKAAODJMJF = ProfileGUI.OJEAKFALOGE.EBDBPJNBHGI / 255f;
			CDNOKAKOLMP = ProfileGUI.OJEAKFALOGE.DPGMCKCDMBC / 255f;
			MIIEANDEPHO = DGDDKFKEDNO != null && DGDDKFKEDNO.IsNew;
			BJONHDGCNFE = JECLDALKMKA.KJHMOGGECBN;
			NFJICFOAOPH = new TrickInfo(DGDDKFKEDNO.Name, JECLDALKMKA.KJHMOGGECBN, KNLJDIPLOIA(), NFNAOFAKEJK, DGDDKFKEDNO.HIAMFGEIGDP);
			Data = NFJICFOAOPH;
			UpdateIcon();
			SetActive(true);
			DECAGHCLJJI();
			HDBAJGJBBKA();
			UpdatePositions();
		}

		public Trick GetTrick()
		{
			return DGDDKFKEDNO;
		}

		public void ShowTrick()
		{
			if (BJONHDGCNFE != null && BJONHDGCNFE.NHNEJKIBPJG)
			{
				CallEvent(12, this);
			}
		}

		public void UpdatePositions()
		{
			if (_keys.activeSelf)
			{
				float num = 100f;
				if (_icon != null)
				{
					num += _icon.transform.localPosition.x + _icon.rectTransform.rect.width / 2f;
				}
				_keys.transform.OKHPLHPBPKJ(num);
			}
			if (_keysDescription.gameObject.activeSelf)
			{
				float num2 = GetComponent<RectTransform>().rect.width / 2f;
				if (_icon != null)
				{
					num2 += _icon.transform.localPosition.x + _icon.rectTransform.rect.width / 2f;
				}
				num2 += 100f - _keysDescription.rectTransform.rect.width / 2f;
				_keysDescription.transform.OKHPLHPBPKJ(num2);
			}
			if (_keys.gameObject.activeSelf && _keysDescription.gameObject.activeSelf)
			{
				_keys.transform.BGNJGIACJBG(55f);
				_keysDescription.transform.BGNJGIACJBG(-55f);
			}
			else
			{
				_keys.transform.BGNJGIACJBG(0f);
				_keysDescription.transform.BGNJGIACJBG(0f);
			}
		}

		public override void UpdateState()
		{
			MIIEANDEPHO = DGDDKFKEDNO != null && DGDDKFKEDNO.IsNew;
		}

		protected void DECAGHCLJJI()
		{
			foreach (Transform item in _keys.transform)
			{
				Object.Destroy(item.gameObject);
			}
			ConditionKeys bHDEBDIHDFM = DGDDKFKEDNO.KJHMOGGECBN.ILBCHANCOBP();
			if (bHDEBDIHDFM == null)
			{
				return;
			}
			_keys.gameObject.SetActive(true);
			float num = 0f;
			KeyData fONEJOKEIEN = bHDEBDIHDFM.FONEJOKEIEN;
			for (int i = 0; i < fONEJOKEIEN.CEPODJDDLBF.Count; i++)
			{
				ResolutionImage keyIcon = PerkContent.GetKeyIcon(fONEJOKEIEN.CEPODJDDLBF[i]);
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
				resolutionImage.raycastTarget = false;
				resolutionImage.transform.SetParent(_keys.transform, false);
				resolutionImage.transform.OKHPLHPBPKJ(num);
				num += 110f;
			}
			for (int j = 0; j < fONEJOKEIEN.IGEEOAGOMEM.Count; j++)
			{
				ResolutionImage keyIcon2 = PerkContent.GetKeyIcon(fONEJOKEIEN.IGEEOAGOMEM[j]);
				if (keyIcon2 != null)
				{
					keyIcon2.transform.SetParent(_keys.transform, false);
					keyIcon2.transform.OKHPLHPBPKJ(num);
					num += 110f;
				}
			}
		}

		protected void HDBAJGJBBKA()
		{
			if (DGDDKFKEDNO != null && !(DGDDKFKEDNO.COJPEGLPGDF == string.Empty))
			{
				_keysDescription.gameObject.SetActive(true);
				_keysDescription.set_LabelFontSize(104);
				_keysDescription.color = Constants.PJJIMHMJPAL;
				_keysDescription.set_Alias(DGDDKFKEDNO.COJPEGLPGDF);
			}
		}

		protected override void FGICHADOEHF()
		{
			base.FGICHADOEHF();
			if (MIIEANDEPHO)
			{
				AJGODMIMDDP();
			}
		}

		private List<float> KNLJDIPLOIA()
		{
			List<float> list = new List<float>();
			if (BJONHDGCNFE != null)
			{
				List<IntervalAnimation> cAANBJEPGAA = BJONHDGCNFE.ODACDCDONJE.Intervals;
				for (int i = 0; i < cAANBJEPGAA.Count; i++)
				{
					if (cAANBJEPGAA[i].Type == IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK)
					{
						list.Add(((IntervalAttack)cAANBJEPGAA[i]).GHGGNMBCMNM());
					}
				}
			}
			return list;
		}

		private void NFNAOFAKEJK(object data)
		{
			ShowTrick();
		}
	}
}
