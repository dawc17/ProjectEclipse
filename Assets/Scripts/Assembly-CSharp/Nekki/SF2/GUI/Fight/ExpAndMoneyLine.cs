using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Fight
{
	public class ExpAndMoneyLine : MonoBehaviour, Line
	{
		[SerializeField]
		private IconAndText exp;

		[SerializeField]
		private IconAndText moneyCount;

		[SerializeField]
		private float moneyAddTime;

		private DG.Tweening.Sequence sequence;

		private UnityEvent endEvent = new UnityEvent();

		private long OPDBMDGCGFO;

		private long NOCNKGALGHF;

		private long PHGCCAFJFLL;

		private long CDGOOJOAOPL;

		private bool needShowExpAndMoney;

		public void Init(long exp, long GBGNFPNCGED)
		{
			needShowExpAndMoney = true;
			OPDBMDGCGFO = exp;
			NOCNKGALGHF = 0L;
			PHGCCAFJFLL = GBGNFPNCGED;
			CDGOOJOAOPL = 0L;
			if (moneyCount != null)
			{
				moneyCount.SetIcon(ListSF.CCDKHLAMKKO().OGJBDMNBMLJ());
			}
			VisibleExpAndMoney(false);
		}

		public void StartAnimation()
		{
			if (needShowExpAndMoney)
			{
				VisibleExpAndMoney(true);
				needShowExpAndMoney = false;
				endEvent.Invoke();
			}
			else
			{
				KDIBKONDDOO();
			}
		}

		public void VisibleExpAndMoney(bool KFIECNIMAOA)
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberGroupSeparator = " ";
			NumberFormatInfo numberFormatInfo2 = numberFormatInfo;
			if (exp != null)
			{
				exp.SetText(NOCNKGALGHF.ToString("N0", numberFormatInfo2));
				exp.gameObject.SetActive(KFIECNIMAOA);
			}
			if (moneyCount != null)
			{
				moneyCount.SetText(CDGOOJOAOPL.ToString("N0", numberFormatInfo2));
				moneyCount.gameObject.SetActive(KFIECNIMAOA);
			}
		}

		private void KDIBKONDDOO()
		{
			NumberFormatInfo f = new NumberFormatInfo
			{
				NumberGroupSeparator = " "
			};
			sequence = DOTween.Sequence();
			if (moneyCount != null && exp != null)
			{
				Tweener t = DOTween.To(() => CDGOOJOAOPL, (long DHDMNHCIPEH) =>
				{
					CDGOOJOAOPL = DHDMNHCIPEH;
					moneyCount.SetText(CDGOOJOAOPL.ToString("N0", f));
				}, PHGCCAFJFLL, moneyAddTime);
				sequence.Append(t);
				Tweener t2 = DOTween.To(() => NOCNKGALGHF, (long DHDMNHCIPEH) =>
				{
					NOCNKGALGHF = DHDMNHCIPEH;
					exp.SetText(NOCNKGALGHF.ToString("N0", f));
				}, OPDBMDGCGFO, moneyAddTime);
				sequence.Join(t2);
				sequence.AppendCallback(() =>
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
			if (sequence != null)
			{
				sequence.Kill();
				sequence = null;
			}
			VisibleExpAndMoney(true);
			if (exp != null)
			{
				NOCNKGALGHF = OPDBMDGCGFO;
				exp.SetText(NOCNKGALGHF.ToString());
			}
			if (moneyCount != null)
			{
				CDGOOJOAOPL = PHGCCAFJFLL;
				moneyCount.SetText(CDGOOJOAOPL.ToString());
			}
		}
	}
}
