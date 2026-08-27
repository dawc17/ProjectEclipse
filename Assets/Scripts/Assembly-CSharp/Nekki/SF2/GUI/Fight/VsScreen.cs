using DG.Tweening;
using UnityEngine;

namespace Nekki.SF2.GUI.Fight
{
	public class VsScreen : MonoBehaviour
	{
		[SerializeField]
		private Vector2 playerLeftFinishPos;

		[SerializeField]
		private Vector2 playerRightFinishPos;

		[SerializeField]
		private Vector2 vsImageFinishScale;

		[SerializeField]
		private float vsImageFinishAlpha;

		[SerializeField]
		private float vsStripeFinishFillAmount;

		[SerializeField]
		private float moveAvatarTime;

		[SerializeField]
		private float afterMoveAvatarPause;

		[SerializeField]
		private float vsImageScaleTime;

		[SerializeField]
		private float afterVsImageScalePause;

		[SerializeField]
		private float vsStripeFillTime;

		[SerializeField]
		private float afterVsStripeFillPause;

		[SerializeField]
		private float afterNameShowPause;

		[SerializeField]
		private ResolutionImageAvatar playerLeft;

		[SerializeField]
		private ResolutionImageAvatar playerRight;

		[SerializeField]
		private ResolutionImage vsImage;

		[SerializeField]
		private ResolutionImage leftStripe;

		[SerializeField]
		private ResolutionImage rightStripe;

		[SerializeField]
		private LabelAlias nameLeft;

		[SerializeField]
		private LabelAlias nameRight;

		private float GLAMMHFCJPN;

		private string texturePath = SF2Paths.BHCPOOOJAAK();

		public float ADMLKNCMFLG
		{
			get
			{
				return get_AnimationTime();
			}
		}

		public float get_AnimationTime()
		{
			return GLAMMHFCJPN;
		}

		public void Init(ModelParameters KEJDJHAGBMK, ModelParameters HFGPAELCNMF)
		{
			if (playerLeft != null)
			{
				playerLeft.set_TexturePath(texturePath);
				playerLeft.set_SpriteName(KEJDJHAGBMK.HNKFHGOOKEG);
				playerLeft.SetNativeSize();
			}
			if (playerRight != null)
			{
				playerRight.set_TexturePath(texturePath);
				playerRight.set_SpriteName(HFGPAELCNMF.HNKFHGOOKEG);
				playerRight.SetNativeSize();
			}
			if (nameLeft != null)
			{
				nameLeft.set_Alias(KEJDJHAGBMK.BMFLPBLAFLK);
			}
			if (nameRight != null)
			{
				nameRight.set_Alias(HFGPAELCNMF.BMFLPBLAFLK);
			}
			if (playerLeft != null && playerRight != null && nameLeft != null && nameRight != null && vsImage != null && leftStripe != null && rightStripe != null)
			{
				DG.Tweening.Sequence s = DOTween.Sequence();
				s.Append(playerLeft.transform.DOLocalMove(playerLeftFinishPos, moveAvatarTime));
				s.Join(playerRight.transform.DOLocalMove(playerRightFinishPos, moveAvatarTime));
				s.AppendInterval(afterMoveAvatarPause);
				s.Append(vsImage.transform.DOScale(vsImageFinishScale, vsImageScaleTime));
				s.Join(vsImage.DOFade(vsImageFinishAlpha, vsImageScaleTime));
				s.AppendInterval(afterVsImageScalePause);
				s.Append(leftStripe.DOFillAmount(vsStripeFinishFillAmount, vsStripeFillTime * 0.5f));
				s.Append(rightStripe.DOFillAmount(vsStripeFinishFillAmount, vsStripeFillTime * 0.5f));
				s.AppendInterval(afterVsStripeFillPause);
				s.AppendCallback(() =>
				{
					nameLeft.gameObject.SetActive(true);
					nameRight.gameObject.SetActive(true);
				});
				s.AppendInterval(afterNameShowPause);
				GLAMMHFCJPN = moveAvatarTime + afterMoveAvatarPause + vsImageScaleTime + afterVsImageScalePause + vsStripeFillTime + afterVsStripeFillPause + afterNameShowPause;
			}
		}
	}
}
