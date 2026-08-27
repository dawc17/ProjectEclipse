using UnityEngine;

namespace Nekki.SF2.GUI.Profile
{
	public class RightInfoSprite : SFMonoBehaviour<object>
	{
		private const int MCOMHPPHDAP = 83;

		private const int MAHAMJNNEKI = 104;

		private const int NJMAJKNEBLG = 8;

		private const float ANLKFGPDBIM = 150f;

		[SerializeField]
		private LabelAlias _header;

		[SerializeField]
		private LabelAlias _noContentMessage;

		[SerializeField]
		private PerkContent _perkContent;

		[SerializeField]
		private TrickContent _trickContent;

		[SerializeField]
		private AchievementContent _achievContent;

		private int HJBOBDMDHNC;

		public void Init()
		{
			IJAAMJCPNBI();
		}

		public void SetPerkInfo(KAHIFHMHDAF BPANICNCIAO)
		{
			_perkContent.gameObject.SetActive(true);
			_trickContent.gameObject.SetActive(false);
			_achievContent.gameObject.SetActive(false);
			_perkContent.Init(BPANICNCIAO.EMDJGBHIAIA, BPANICNCIAO.state, BPANICNCIAO.ODDEOFKLIAG, BPANICNCIAO.HCBDNEOKGNK, BPANICNCIAO.JMLAKAKDBBL);
			if (_perkContent.HeaderFontSize > 0)
			{
				HJBOBDMDHNC = _perkContent.HeaderFontSize;
			}
			else
			{
				HJBOBDMDHNC = 104;
			}
			_noContentMessage.gameObject.SetActive(false);
			SetLabel(BPANICNCIAO.name);
			float upBorder = _header.transform.localPosition.y - _header.rectTransform.rect.height / 2f;
			_perkContent.SetUpBorder(upBorder);
		}

		public void SetTrickInfo(TrickInfo ACNOAOIBCBM)
		{
			_perkContent.gameObject.SetActive(false);
			_trickContent.gameObject.SetActive(true);
			_achievContent.gameObject.SetActive(false);
			_trickContent.Init(ACNOAOIBCBM.EMBBNNBFODN, ACNOAOIBCBM.CKKFKEIELCP, ACNOAOIBCBM.ODDEOFKLIAG, ACNOAOIBCBM.EMDJGBHIAIA);
			if (_perkContent.HeaderFontSize > 0)
			{
				HJBOBDMDHNC = _perkContent.HeaderFontSize;
			}
			else
			{
				HJBOBDMDHNC = 104;
			}
			_noContentMessage.gameObject.SetActive(false);
			SetLabel(ACNOAOIBCBM.HHAAFADDOJB);
			float upBorder = _header.transform.localPosition.y - _header.rectTransform.rect.height / 2f;
			_trickContent.SetUpBorder(upBorder);
		}

		public void SetAchievementInfo(AchievementInfo BBOFGPLPEPB)
		{
			_perkContent.gameObject.SetActive(false);
			_trickContent.gameObject.SetActive(false);
			_achievContent.gameObject.SetActive(true);
			_achievContent.Init(BBOFGPLPEPB.MJBLCNPNOBC, BBOFGPLPEPB.GBGNFPNCGED, BBOFGPLPEPB.PNDAIFALIKF, BBOFGPLPEPB.ODDEOFKLIAG, BBOFGPLPEPB.DJGOCCEOAKD, BBOFGPLPEPB.NNEHNDILGDP);
			if (_achievContent.HeaderFontSize > 0)
			{
				HJBOBDMDHNC = _achievContent.HeaderFontSize;
			}
			else
			{
				HJBOBDMDHNC = 104;
			}
			_noContentMessage.gameObject.SetActive(false);
			SetLabel(BBOFGPLPEPB.HHAAFADDOJB);
			float upBorder = _header.transform.localPosition.y - _header.rectTransform.rect.height / 2f;
			_trickContent.SetUpBorder(upBorder);
		}

		public void SetItemInfo(ItemInfo PJDAGCBPLJE)
		{
			_perkContent.gameObject.SetActive(false);
			_trickContent.gameObject.SetActive(false);
			_achievContent.gameObject.SetActive(true);
			_achievContent.Init(PJDAGCBPLJE.GGDJIPKMKFC);
			if (_achievContent.HeaderFontSize > 0)
			{
				HJBOBDMDHNC = _achievContent.HeaderFontSize;
			}
			else
			{
				HJBOBDMDHNC = 104;
			}
			_noContentMessage.gameObject.SetActive(false);
			SetLabel(PJDAGCBPLJE.Name);
			float upBorder = _header.transform.localPosition.y - _header.rectTransform.rect.height / 2f;
			_trickContent.SetUpBorder(upBorder);
		}

		public void SetLabel(string HHAAFADDOJB)
		{
			_header.set_LabelFontSize(HJBOBDMDHNC);
			_header.set_Alias(HHAAFADDOJB);
		}

		public void SetNoContentMessage(string LIOGIBJBHAH)
		{
			_noContentMessage.set_Alias(LIOGIBJBHAH);
		}

		public void Clear()
		{
			_header.set_text(string.Empty);
			_noContentMessage.gameObject.SetActive(true);
			_perkContent.gameObject.SetActive(false);
			_trickContent.gameObject.SetActive(false);
			_achievContent.gameObject.SetActive(false);
		}

		public float GetLabelWidth()
		{
			return GetComponent<RectTransform>().rect.width - 120f;
		}

		public LabelButton GetBtnPerkImprove()
		{
			if (_perkContent.gameObject.activeSelf)
			{
				return _perkContent.GetBtnImprove();
			}
			return null;
		}

		public LabelButton GetBtnStrikeShow()
		{
			if (_trickContent.gameObject.activeSelf)
			{
				return _trickContent.GetBtnShow();
			}
			return null;
		}

		private void IJAAMJCPNBI()
		{
			_header.set_text(string.Empty);
			_header.set_LabelFontSize(HJBOBDMDHNC);
			_header.transform.OKHPLHPBPKJ(8f);
			_header.transform.BGNJGIACJBG(GetComponent<RectTransform>().rect.height / 2f - 150f);
			_header.rectTransform.sizeDelta = new Vector2(GetLabelWidth(), _header.rectTransform.rect.height);
			_header.color = Constants.PJJIMHMJPAL;
			_noContentMessage.set_text(string.Empty);
			_noContentMessage.rectTransform.sizeDelta = new Vector2(GetLabelWidth(), _noContentMessage.rectTransform.rect.height);
			_noContentMessage.set_LabelFontSize(83);
			_noContentMessage.transform.OKHPLHPBPKJ(8f);
			_noContentMessage.transform.BGNJGIACJBG(0f);
			_noContentMessage.color = Constants.PJJIMHMJPAL;
		}
	}
}
