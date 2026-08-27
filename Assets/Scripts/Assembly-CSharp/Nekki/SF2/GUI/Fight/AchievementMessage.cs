using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Nekki.SF2.GUI.Fight
{
	public class AchievementMessage : MonoBehaviour
	{
		[SerializeField]
		private ResolutionImage icon;

		[SerializeField]
		private LabelAlias text;

		[SerializeField]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private float showTime = 0.5f;

		[SerializeField]
		private float hideTime = 1f;

		[SerializeField]
		private float waitTime = 1.5f;

		[SerializeField]
		private float maxAlpha = 1f;

		[SerializeField]
		private float minAlpha;

		private Achievement JJGCLBIGIPL;

		public UnityEvent OnHide = new UnityEvent();

		public void Init(Achievement NCCHENOEPNF)
		{
			JJGCLBIGIPL = NCCHENOEPNF;
			if (text != null && NCCHENOEPNF != null)
			{
				text.SetAlias(NCCHENOEPNF.Name);
			}
			if (icon != null && NCCHENOEPNF != null)
			{
				icon.set_TexturePath("UI/Achievements/");
				icon.set_SpriteName(NCCHENOEPNF.MJBPMLCLMFN);
			}
			if (canvasGroup != null)
			{
				canvasGroup.alpha = minAlpha;
			}
		}

		public void StartAnimation()
		{
			DG.Tweening.Sequence s = DOTween.Sequence();
			if (canvasGroup != null)
			{
				s.Append(canvasGroup.DOFade(maxAlpha, showTime));
			}
			s.AppendInterval(waitTime);
			if (canvasGroup != null)
			{
				s.Append(canvasGroup.DOFade(minAlpha, hideTime));
			}
			s.AppendCallback(() =>
			{
				OnHide.Invoke();
			});
		}
	}
}
