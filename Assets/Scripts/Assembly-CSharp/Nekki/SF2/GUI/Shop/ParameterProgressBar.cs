using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Shop
{
	public class ParameterProgressBar : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage Background;

		[SerializeField]
		private List<ResolutionImage> Stripes;

		public virtual void Init()
		{
			if (Stripes == null)
			{
				return;
			}
			foreach (ResolutionImage stripe in Stripes)
			{
				stripe.type = Image.Type.Filled;
				stripe.fillMethod = Image.FillMethod.Horizontal;
				stripe.fillOrigin = 0;
				stripe.fillAmount = 0f;
			}
		}

		public virtual void SetValue(float OKEFHDDPMEC, int HDOJFLBLPMO, float _Duration = 0f)
		{
			if (Stripes.Count > HDOJFLBLPMO)
			{
				ResolutionImage target = Stripes[HDOJFLBLPMO];
				target.DOKill();
				target.DOFillAmount(OKEFHDDPMEC, _Duration);
			}
		}

		public virtual void SetValue(float OKEFHDDPMEC, float _Duration = 0f)
		{
			foreach (ResolutionImage stripe in Stripes)
			{
				stripe.DOKill();
				stripe.DOFillAmount(OKEFHDDPMEC, _Duration);
			}
		}

		public virtual float GetValue(int HDOJFLBLPMO)
		{
			if (Stripes.Count > HDOJFLBLPMO)
			{
				return Stripes[HDOJFLBLPMO].fillAmount;
			}
			return 0f;
		}
	}
}
