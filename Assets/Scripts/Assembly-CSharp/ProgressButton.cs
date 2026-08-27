using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressButton : SFButton
{
	[SerializeField]
	private Image _picCircleProgressBar;

	[SerializeField]
	private Image _picComplete;

	private float ILDDBLFPPPG;

	public const float PercentageMin = 0f;

	public const float PercentageMax = 100f;

	private Tween _tween;

	public void Init()
	{
		if (_picComplete != null)
		{
			_picComplete.gameObject.SetActive(false);
		}
		ResetPercentage();
	}

	public float GetPercentage()
	{
		return ILDDBLFPPPG;
	}

	private void EHCCJJNBGAF(float EJHLCDFHNPA)
	{
		ILDDBLFPPPG = Mathf.Clamp(EJHLCDFHNPA, 0f, 100f);
		if (_picComplete != null)
		{
			if (ILDDBLFPPPG == 0f)
			{
				_picComplete.gameObject.SetActive(false);
			}
			if (ILDDBLFPPPG == 100f)
			{
				_picComplete.gameObject.SetActive(true);
			}
		}
		if (_picCircleProgressBar != null)
		{
			_picCircleProgressBar.fillAmount = ILDDBLFPPPG / 100f;
		}
	}

	public void SetPercentage(float EJHLCDFHNPA, float BFJBKLCLIHP)
	{
		float num = BFJBKLCLIHP / 60f;
		KillTween();
		EJHLCDFHNPA = Mathf.Clamp(EJHLCDFHNPA, 0f, 100f);
		if (ILDDBLFPPPG == EJHLCDFHNPA)
		{
			return;
		}
		if (num == 0f)
		{
			EHCCJJNBGAF(EJHLCDFHNPA);
			return;
		}
		_tween = DOTween.To(() => ILDDBLFPPPG, (float DHDMNHCIPEH) =>
		{
			EHCCJJNBGAF(DHDMNHCIPEH);
		}, EJHLCDFHNPA, num);
	}

	public void AddPercentage(float FOIPKLDNGDL)
	{
		KillTween();
		EHCCJJNBGAF(GetPercentage() + FOIPKLDNGDL);
	}

	public void ResetPercentage()
	{
		KillTween();
		EHCCJJNBGAF(0f);
	}

	private void KillTween()
	{
		if (_tween != null)
		{
			_tween.Kill();
			_tween = null;
		}
	}
}
