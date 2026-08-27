using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Map
{
	public class BattlePrize : SFMonoBehaviour<object>
	{
		public const float BATTLE_GOLD_SIZE = 40f;

		private List<BattlePrizeElement> DEKNGFABMJA = new List<BattlePrizeElement>();

		private float GEFOLNHPJMI = -1f;

		private float BKNHLFAFGHO = -1f;

		private float FKGFPGOOJIB;

		private float FJGELPELEMN;

		private bool NDDFNGNAPIC;

		[SerializeField]
		private GameObject _prizeElemPrefab;

		[SerializeField]
		private HorizontalLayoutGroup _layoutGroup;

		[SerializeField]
		private ResolutionImage _itemIcon;

		private float KNMFEAMFMFK;

		private float _spacing = 20f;

		private static int GNAONAPDDLD = 360;

		public void Init(long GBGNFPNCGED, long PAGGOKFIEOP, RewardPrize DPIIJICBGGA, float HOJOKAOLMGN, float AHKNBOHOOOK, int CFMPJLLNCFF)
		{
			float jPDGMJHNKPK = 1f;
			float jPDGMJHNKPK2 = 1f;
			foreach (BattlePrizeElement item in DEKNGFABMJA)
			{
				Object.Destroy(item.gameObject);
			}
			DEKNGFABMJA.Clear();
			_itemIcon.gameObject.SetActive(false);
			GEFOLNHPJMI = HOJOKAOLMGN;
			BKNHLFAFGHO = AHKNBOHOOOK;
			string aDONPNOBBDE = "MiscSprites.ruby";
			string aDONPNOBBDE2 = ListSF.CCDKHLAMKKO().OGJBDMNBMLJ();
			KNMFEAMFMFK = 0f;
			if (GBGNFPNCGED > 0)
			{
				BattlePrizeElement component = Object.Instantiate(_prizeElemPrefab).GetComponent<BattlePrizeElement>();
				component.Init(aDONPNOBBDE2, GBGNFPNCGED, CFMPJLLNCFF, jPDGMJHNKPK);
				GLJMJOACEIP(component);
			}
			if (PAGGOKFIEOP > 0)
			{
				BattlePrizeElement component2 = Object.Instantiate(_prizeElemPrefab).GetComponent<BattlePrizeElement>();
				component2.Init(aDONPNOBBDE, PAGGOKFIEOP, CFMPJLLNCFF, jPDGMJHNKPK2);
				GLJMJOACEIP(component2);
			}
			foreach (RewardCurrency item2 in DPIIJICBGGA.KIMJGOHCCPO)
			{
				if (item2.GOOBKHECJIF)
				{
					GameCurrency cJJOFMHLFFM = GameUtils.AJDKHINLIDI.ICFINJLNCPM(item2.Name);
					if (cJJOFMHLFFM != null)
					{
						string mJBPMLCLMFN = cJJOFMHLFFM.MJBPMLCLMFN;
						long bAINMLLIKOL = item2.MFPJMGJLKMH();
						BattlePrizeElement component3 = Object.Instantiate(_prizeElemPrefab).GetComponent<BattlePrizeElement>();
						component3.Init(mJBPMLCLMFN, bAINMLLIKOL, CFMPJLLNCFF);
						GLJMJOACEIP(component3);
					}
				}
			}
			foreach (RewardResistance item3 in DPIIJICBGGA.KBMDJACLAOH)
			{
				if (item3.GOOBKHECJIF)
				{
					GameResistance oOJJEOFENBJ = GameUtils.JNIMKHKGPHE.NDMEGBEFBPJ(item3.Name);
					if (oOJJEOFENBJ != null)
					{
						string aDONPNOBBDE3 = oOJJEOFENBJ.CIOKDNDHFBE();
						long bAINMLLIKOL2 = item3.Value;
						BattlePrizeElement component4 = Object.Instantiate(_prizeElemPrefab).GetComponent<BattlePrizeElement>();
						component4.Init(aDONPNOBBDE3, bAINMLLIKOL2, CFMPJLLNCFF);
						GLJMJOACEIP(component4);
					}
				}
			}
			_layoutGroup.spacing = _spacing;
			_layoutGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(KNMFEAMFMFK, _layoutGroup.GetComponent<RectTransform>().sizeDelta.y);
			JPDODBLOPFJ();
			foreach (RewardItem item4 in DPIIJICBGGA.HELFDCAIJNE)
			{
				if (item4.GOOBKHECJIF)
				{
					ItemInfo dJKEECEOCJB = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(item4.Name);
					UserItem dKCHDHMLKHN = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(dJKEECEOCJB);
					if (dKCHDHMLKHN == null)
					{
						AddItem(dJKEECEOCJB);
						break;
					}
				}
			}
		}

		public void AddItem(ItemInfo PJDAGCBPLJE)
		{
			if (PJDAGCBPLJE.Type == "Seal")
			{
				_itemIcon.set_TexturePath(SF2Paths.BHCPOOOJAAK());
			}
			else
			{
				_itemIcon.set_TexturePath(SF2Paths.LFIIMPEAMFG());
			}
			_itemIcon.set_SpriteName(PJDAGCBPLJE.FileName);
			_itemIcon.gameObject.SetActive(true);
			_itemIcon.preserveAspect = true;
			foreach (BattlePrizeElement item in DEKNGFABMJA)
			{
				item.gameObject.SetActive(false);
			}
			JPDODBLOPFJ();
		}

		private void Update()
		{
			if (NDDFNGNAPIC)
			{
				OMAPINCCAFA();
			}
		}

		private void GLJMJOACEIP(BattlePrizeElement DGNDGHPMPJD)
		{
			if (DGNDGHPMPJD != null)
			{
				KNMFEAMFMFK += DGNDGHPMPJD.GetComponent<LayoutElement>().preferredWidth;
				KNMFEAMFMFK += _spacing;
				DGNDGHPMPJD.transform.SetParent(_layoutGroup.transform, false);
				DEKNGFABMJA.Add(DGNDGHPMPJD);
			}
		}

		private void JPDODBLOPFJ()
		{
			NDDFNGNAPIC = _layoutGroup.GetComponent<RectTransform>().rect.width > GetComponent<RectTransform>().rect.width;
			if (null != _itemIcon)
			{
				_itemIcon.rectTransform.sizeDelta = new Vector2(500f, BKNHLFAFGHO);
			}
		}

		private void OMAPINCCAFA()
		{
			GNAONAPDDLD++;
			float mNADIKCPPIG = MapGUI.ELPKAJAKAEL.MNADIKCPPIG;
			float mIFFMBOIAGC = MapGUI.ELPKAJAKAEL.MIFFMBOIAGC;
			float num = _layoutGroup.GetComponent<RectTransform>().rect.width - GetComponent<RectTransform>().rect.width;
			float num2 = num / (2f * mNADIKCPPIG);
			float bAINMLLIKOL = num2 * mNADIKCPPIG * Mathf.Cos(mIFFMBOIAGC * (float)GNAONAPDDLD);
			_layoutGroup.transform.OKHPLHPBPKJ(bAINMLLIKOL);
		}
	}
}
