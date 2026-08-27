using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Fight
{
	public class TextAndMoneyLine : MonoBehaviour, Line
	{
		[SerializeField]
		private LabelAlias textLabel;

		[SerializeField]
		private IconAndText moneyCount;

		[SerializeField]
		private IconAndText rubyCount;

		[SerializeField]
		private Vector2 basePos;

		[SerializeField]
		private float textMoveTime;

		[SerializeField]
		private float moneyAddTime;

		private Vector2 IINGLPEOPNN;

		private long PHGCCAFJFLL;

		private long CDGOOJOAOPL;

		private long EKHOFFABFOG;

		private long LJAEAKIDDOE;

		private bool needShowLabel;

		private DG.Tweening.Sequence BMJCFMAIDIE;

		private DG.Tweening.Sequence HEFFELHEAME;

		private UnityEvent endEvent = new UnityEvent();

		public void Init(string HCPNFPMHFCM, long GBGNFPNCGED, long PAGGOKFIEOP, string BBLOBPOCGNM = "")
		{
			needShowLabel = true;
			PHGCCAFJFLL = GBGNFPNCGED;
			CDGOOJOAOPL = 0L;
			EKHOFFABFOG = PAGGOKFIEOP;
			LJAEAKIDDOE = 0L;
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberGroupSeparator = " ";
			NumberFormatInfo numberFormatInfo2 = numberFormatInfo;
			if (textLabel != null && !string.IsNullOrEmpty(HCPNFPMHFCM))
			{
				if (BBLOBPOCGNM != null)
				{
					HCPNFPMHFCM = string.Format("{0}{1}", HCPNFPMHFCM, "{" + BBLOBPOCGNM + "}");
				}
				textLabel.SetAlias(HCPNFPMHFCM);
				IINGLPEOPNN = textLabel.transform.localPosition;
				textLabel.transform.localPosition = basePos;
			}
			if (moneyCount != null)
			{
				moneyCount.SetIcon(ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
				moneyCount.SetText(CDGOOJOAOPL.ToString("N0", numberFormatInfo2));
				moneyCount.gameObject.SetActive(false);
			}
			if (rubyCount != null)
			{
				rubyCount.SetText(LJAEAKIDDOE.ToString("N0", numberFormatInfo2));
				rubyCount.gameObject.SetActive(false);
			}
		}

		public void StartAnimation()
		{
			if (needShowLabel)
			{
				NEOAHCLHNHE();
				needShowLabel = false;
			}
			else
			{
				IAPEAPKIIHN();
			}
		}

		private void NEOAHCLHNHE()
		{
			BMJCFMAIDIE = DOTween.Sequence();
			if (textLabel != null)
			{
				BMJCFMAIDIE.Append(textLabel.transform.DOLocalMove(IINGLPEOPNN, textMoveTime));
			}
			BMJCFMAIDIE.AppendCallback(() =>
			{
				if ((PHGCCAFJFLL > 0 || EKHOFFABFOG < 1) && moneyCount != null)
				{
					moneyCount.gameObject.SetActive(true);
				}
				if (EKHOFFABFOG > 0 && rubyCount != null)
				{
					rubyCount.gameObject.SetActive(true);
				}
			});
			BMJCFMAIDIE.AppendCallback(() =>
			{
				endEvent.Invoke();
			});
		}

		private void IAPEAPKIIHN()
		{
			NumberFormatInfo f = new NumberFormatInfo
			{
				NumberGroupSeparator = " "
			};
			HEFFELHEAME = DOTween.Sequence();
			if (moneyCount != null && rubyCount != null)
			{
				Tweener t = DOTween.To(() => CDGOOJOAOPL, (long DHDMNHCIPEH) =>
				{
					CDGOOJOAOPL = DHDMNHCIPEH;
					moneyCount.SetText(CDGOOJOAOPL.ToString("N0", f));
				}, PHGCCAFJFLL, moneyAddTime);
				HEFFELHEAME.Append(t);
				if (EKHOFFABFOG > 0)
				{
					Tweener t2 = DOTween.To(() => LJAEAKIDDOE, (long DHDMNHCIPEH) =>
					{
						LJAEAKIDDOE = DHDMNHCIPEH;
						rubyCount.SetText(LJAEAKIDDOE.ToString("N0", f));
					}, EKHOFFABFOG, moneyAddTime);
					HEFFELHEAME.Join(t2);
				}
				HEFFELHEAME.AppendCallback(() =>
				{
					endEvent.Invoke();
				});
			}
			else
			{
				endEvent.Invoke();
			}
		}

		public void AddListener(UnityAction ODDEOFKLIAG)
		{
			endEvent.AddListener(ODDEOFKLIAG);
		}

		public void RemoveListener(UnityAction ODDEOFKLIAG)
		{
			endEvent.RemoveListener(ODDEOFKLIAG);
		}

		public void FinishAnimation()
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberGroupSeparator = " ";
			NumberFormatInfo numberFormatInfo2 = numberFormatInfo;
			if (BMJCFMAIDIE != null)
			{
				BMJCFMAIDIE.Kill();
				BMJCFMAIDIE = null;
			}
			if (HEFFELHEAME != null)
			{
				HEFFELHEAME.Kill();
				HEFFELHEAME = null;
			}
			if (textLabel != null)
			{
				textLabel.transform.localPosition = IINGLPEOPNN;
			}
			if ((PHGCCAFJFLL > 0 || EKHOFFABFOG < 1) && moneyCount != null)
			{
				moneyCount.gameObject.SetActive(true);
				CDGOOJOAOPL = PHGCCAFJFLL;
				moneyCount.SetText(CDGOOJOAOPL.ToString("N0", numberFormatInfo2));
			}
			if (EKHOFFABFOG > 0 && rubyCount != null)
			{
				rubyCount.gameObject.SetActive(true);
				LJAEAKIDDOE = EKHOFFABFOG;
				rubyCount.SetText(LJAEAKIDDOE.ToString("N0", numberFormatInfo2));
			}
		}
	}
}
