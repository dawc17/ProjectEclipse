using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Fight
{
	public class EndFightContent : MonoBehaviour
	{
		[SerializeField]
		private string goldPrizeAlias = "goldPrize";

		[SerializeField]
		private string goldPerfectAlias = "goldPerfect";

		[SerializeField]
		private string goldFirstStrikeAlias = "goldFirstStrike";

		[SerializeField]
		private string goldComboAlias = "goldCombo";

		[SerializeField]
		private string goldShockAlias = "goldShock";

		[SerializeField]
		private GameObject _rewardItemPrefab;

		[SerializeField]
		private GameObject _textAndMoneyLinePrefab;

		[SerializeField]
		private GameObject _expAndMoneyLinePrefab;

		[SerializeField]
		private LabelButton _buttonOk;

		[SerializeField]
		private Transform _buttonPanel;

		private VerticalLayoutGroup KCAMOOCBMBE;

		private VerticalLayoutGroup JJPGPNCNFII;

		private FightResult MPFLHOFEOGI;

		private ItemRewardHardmode _itemReward;

		private List<Line> GOJABLKPFJM = new List<Line>();

		private List<ItemInfo> _items = new List<ItemInfo>();

		private int _currentLine;

		private const float ICLIJILAGMB = 300f;

		private float LGEDDHALPDI;

		private float AAGLGCPCECE;

		public UnityEvent AnimationEndEvent = new UnityEvent();

		public UnityEvent CloseEvent = new UnityEvent();

		private Button _animationFinishButton;

		public void Init(FightResult HEIADONEACH, VerticalLayoutGroup KPAICOOKACB, Button OBMBALDIBEB)
		{
			_animationFinishButton = OBMBALDIBEB;
			MPFLHOFEOGI = HEIADONEACH;
			_currentLine = 0;
			_items = MPFLHOFEOGI.PMIHPJFAJIO.PJNJIJIODHE(true);
			bool flag = _items.Count > 0;
			JJPGPNCNFII = KPAICOOKACB;
			KCAMOOCBMBE = GetComponent<VerticalLayoutGroup>();
			if (!flag)
			{
				HIODPBFDLIM();
			}
			else
			{
				JHGJAJAIONL();
			}
		}

		private void JHGJAJAIONL()
		{
			if (_animationFinishButton != null)
			{
				_animationFinishButton.gameObject.SetActive(false);
			}
			if ((bool)JJPGPNCNFII)
			{
				LGEDDHALPDI = JJPGPNCNFII.spacing;
				JJPGPNCNFII.spacing = 300f;
			}
			if ((bool)KCAMOOCBMBE)
			{
				AAGLGCPCECE = KCAMOOCBMBE.spacing;
				KCAMOOCBMBE.spacing = 300f;
			}
			if (_items.Count != 0)
			{
				_itemReward = Object.Instantiate(_rewardItemPrefab).GetComponent<ItemRewardHardmode>();
				_itemReward.gameObject.SetActive(true);
				_itemReward.transform.SetParent(base.transform, false);
				_itemReward.setIcon(_items[0]);
			}
			if (_buttonOk != null)
			{
				_buttonOk.onClick.AddListener(ILBGMNNMIEN);
				_buttonOk.gameObject.SetActive(true);
			}
			if (_buttonPanel != null)
			{
				_buttonPanel.SetAsLastSibling();
			}
		}

		private void HIODPBFDLIM()
		{
			if (_animationFinishButton != null)
			{
				_animationFinishButton.gameObject.SetActive(true);
			}
			if (_textAndMoneyLinePrefab != null)
			{
				GOJABLKPFJM.Add(CreateLine(goldPrizeAlias, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.PDJPOBHLIHA, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.JNCDLOAEMCG, string.Empty));
				GOJABLKPFJM.Add(CreateLine(goldPerfectAlias, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.MKNGIDKGOLE, 0L, MPFLHOFEOGI.AIOMDIAFHGB.JDKFHFOJKPI.ToString()));
				GOJABLKPFJM.Add(CreateLine(goldFirstStrikeAlias, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.LOONMILKCFK, 0L, MPFLHOFEOGI.AIOMDIAFHGB.MOLDOOIJELI.ToString()));
				GOJABLKPFJM.Add(CreateLine(goldComboAlias, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.GKAEJDCDMHC, 0L, MPFLHOFEOGI.AIOMDIAFHGB.KKJHBKBMPGN.ToString()));
				GOJABLKPFJM.Add(CreateLine(goldShockAlias, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.APCAKCCOMLO, 0L, MPFLHOFEOGI.AIOMDIAFHGB.OGMOILIMCOM.ToString()));
				GOJABLKPFJM.Add(CreateLine(MPFLHOFEOGI.AIOMDIAFHGB.StatisticCrazyStyleToString, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.AIJNPAIMPHG, 0L, string.Empty));
			}
			if (_expAndMoneyLinePrefab != null)
			{
				GOJABLKPFJM.Add(CreateLine((long)MPFLHOFEOGI.NJNKGLJNNDH, MPFLHOFEOGI.AIOMDIAFHGB.ECOOCLMNFJM.POPNFGNAOJD));
			}
			if (_buttonOk != null)
			{
				_buttonOk.onClick.AddListener(() =>
				{
					CloseEvent.Invoke();
				});
				_buttonOk.gameObject.SetActive(false);
			}
			if (_buttonPanel != null)
			{
				_buttonPanel.SetAsLastSibling();
			}
			BKOEBBIFCDE();
		}

		private void ILBGMNNMIEN()
		{
			if (_buttonOk != null)
			{
				_buttonOk.onClick.RemoveListener(ILBGMNNMIEN);
			}
			if ((bool)JJPGPNCNFII)
			{
				JJPGPNCNFII.spacing = LGEDDHALPDI;
			}
			if ((bool)KCAMOOCBMBE)
			{
				KCAMOOCBMBE.spacing = AAGLGCPCECE;
			}
			Object.DestroyObject(_itemReward.gameObject);
			HIODPBFDLIM();
		}

		private void BKOEBBIFCDE()
		{
			if (_currentLine < GOJABLKPFJM.Count)
			{
				Line fCMOHBLGJFP = GOJABLKPFJM[_currentLine];
				fCMOHBLGJFP.AddListener(JACPGFBNLDI);
				fCMOHBLGJFP.AddListener(fCMOHBLGJFP.StartAnimation);
				fCMOHBLGJFP.StartAnimation();
				return;
			}
			Line fCMOHBLGJFP2 = ((GOJABLKPFJM.Count <= 0) ? null : GOJABLKPFJM[GOJABLKPFJM.Count - 1]);
			if (fCMOHBLGJFP2 != null)
			{
				fCMOHBLGJFP2.AddListener(OCHPJGEBHAE);
			}
			foreach (Line item in GOJABLKPFJM)
			{
				item.RemoveListener(JACPGFBNLDI);
				item.RemoveListener(item.StartAnimation);
			}
		}

		private void JACPGFBNLDI()
		{
			StartCoroutine(LFPPJMCFOND());
		}

		private void OCHPJGEBHAE()
		{
			AnimationEndEvent.Invoke();
			if (_buttonOk != null)
			{
				_buttonOk.gameObject.SetActive(true);
			}
		}

		private IEnumerator LFPPJMCFOND()
		{
			yield return new WaitForEndOfFrame();
			_currentLine++;
			BKOEBBIFCDE();
		}

		public Line CreateLine(string HCPNFPMHFCM, long GBGNFPNCGED, long PAGGOKFIEOP = 0L, string BBLOBPOCGNM = "")
		{
			TextAndMoneyLine component = Object.Instantiate(_textAndMoneyLinePrefab).GetComponent<TextAndMoneyLine>();
			component.gameObject.SetActive(true);
			component.transform.SetParent(base.transform, false);
			component.Init(HCPNFPMHFCM, GBGNFPNCGED, PAGGOKFIEOP, BBLOBPOCGNM);
			return component;
		}

		public Line CreateLine(long exp, long GBGNFPNCGED)
		{
			ExpAndMoneyLine component = Object.Instantiate(_expAndMoneyLinePrefab).GetComponent<ExpAndMoneyLine>();
			component.gameObject.SetActive(true);
			component.transform.SetParent(base.transform, false);
			component.Init(exp, GBGNFPNCGED);
			return component;
		}

		public void FinishAnimation()
		{
			foreach (Line item in GOJABLKPFJM)
			{
				item.FinishAnimation();
			}
			OCHPJGEBHAE();
		}
	}
}
