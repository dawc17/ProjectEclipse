using DG.Tweening;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	[SerializeField]
	public ResolutionImage Background;

	[SerializeField]
	public ResolutionImage Stripe;

	private float JEECPIEHLLH;

	private float FPPIMENCNJP = 1f;

	private float LOMDDBJKNNO = 1f;

	private Tween _tween;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void Init()
	{
		Stripe.type = Image.Type.Filled;
		Stripe.fillMethod = Image.FillMethod.Horizontal;
	}

	public void SetValueBorders(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		JEECPIEHLLH = LHNCHOAEGEA;
		FPPIMENCNJP = KAEPJHHLLPK;
		IEHJMPBHAFJ();
	}

	public virtual void SetValue(float OKEFHDDPMEC, float _Duration = 0f)
	{
		OKEFHDDPMEC = Mathf.Clamp(OKEFHDDPMEC, JEECPIEHLLH, FPPIMENCNJP);
		if (LOMDDBJKNNO != OKEFHDDPMEC)
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}
			_tween = DOTween.To(() => LOMDDBJKNNO, (float DHDMNHCIPEH) =>
			{
				LOMDDBJKNNO = DHDMNHCIPEH;
				IEHJMPBHAFJ();
			}, OKEFHDDPMEC, _Duration);
			IEHJMPBHAFJ();
		}
	}

	public virtual float GetValue()
	{
		return LOMDDBJKNNO;
	}

	private void IEHJMPBHAFJ()
	{
		Stripe.fillAmount = (LOMDDBJKNNO - JEECPIEHLLH) / (FPPIMENCNJP - JEECPIEHLLH);
	}
}
