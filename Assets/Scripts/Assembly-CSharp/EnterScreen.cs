using System;
using System.Collections.Generic;
using DG.Tweening;
using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

public class EnterScreen : MonoBehaviour
{
	[SerializeField]
	public float MIN_OPACITY;

	[SerializeField]
	public float MAX_OPACITY = 1f;

	[SerializeField]
	public float fadeTime = 1f;

	[SerializeField]
	public float expandTime = 1f;

	[SerializeField]
	public float hideTime = 1f;

	[SerializeField]
	public float endTime = 1f;

	[SerializeField]
	private LabelAlias _label;

	[SerializeField]
	private ResolutionImage _backgroundLeft;

	[SerializeField]
	private ResolutionImage _backgroundRight;

	[SerializeField]
	private Image _foreground;

	private Action _dlg;

	private float EFGJOKICKHO;

	private float ALGOCMIFECI;

	public static EnterScreen Create()
	{
		EnterScreen original = Resources.Load<EnterScreen>("Prefabs/Map/EnterScreen");
		return UnityEngine.Object.Instantiate(original);
	}

	public void Init(List<KeyValuePair<string, int>> IGLEKOAILHD, Action ODDEOFKLIAG)
	{
		_dlg = ODDEOFKLIAG;
		SetVisible(false);
		CCDGDBJLLNG();
		PDCFDHNFCIG(MIN_OPACITY);
		if (_label != null && IGLEKOAILHD.Count > 0)
		{
			_label.set_Alias(IGLEKOAILHD[0].Key);
		}
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0f);
		foreach (KeyValuePair<string, int> MGPBPJOHMLH in IGLEKOAILHD)
		{
			sequence.AppendCallback(() =>
			{
				if (_label != null)
				{
					_label.set_Alias(MGPBPJOHMLH.Key);
				}
			});
			float interval = (float)MGPBPJOHMLH.Value / 60f;
			sequence.AppendInterval(interval);
		}
		DG.Tweening.Sequence t = MuteMusicSequence();
		DG.Tweening.Sequence s = DOTween.Sequence();
		s.Append(t);
		s.Join(_foreground.DOFade(MAX_OPACITY, fadeTime));
		s.AppendCallback(() =>
		{
			PlayMusic();
			SetVisible(true);
		});
		s.Append(_foreground.DOFade(MIN_OPACITY, expandTime));
		s.Append(sequence);
		s.Append(_foreground.DOFade(MAX_OPACITY, endTime));
		s.AppendCallback(() =>
		{
			End();
		});
	}

	public void Init(string HCPNFPMHFCM, Action ODDEOFKLIAG)
	{
		_dlg = ODDEOFKLIAG;
		if (_label != null)
		{
			_label.set_Alias(HCPNFPMHFCM);
		}
		SetVisible(false);
		CCDGDBJLLNG();
		PDCFDHNFCIG(MIN_OPACITY);
		DG.Tweening.Sequence t = MuteMusicSequence();
		DG.Tweening.Sequence s = DOTween.Sequence();
		s.Append(t);
		s.Join(_foreground.DOFade(MAX_OPACITY, fadeTime));
		s.AppendCallback(() =>
		{
			PlayMusic();
			SetVisible(true);
		});
		s.Append(_foreground.DOFade(MIN_OPACITY, expandTime));
		s.AppendInterval(hideTime);
		s.Append(_foreground.DOFade(MAX_OPACITY, endTime));
		s.AppendCallback(() =>
		{
			End();
		});
	}

	public DG.Tweening.Sequence MuteMusicSequence()
	{
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(DOTween.To(() => ALGOCMIFECI, (float DHDMNHCIPEH) =>
		{
			ALGOCMIFECI = DHDMNHCIPEH;
			Sound.OAFCOFNOIJK(ALGOCMIFECI);
		}, 0f, fadeTime));
		sequence.AppendCallback(() =>
		{
			Sound.OAFCOFNOIJK(EFGJOKICKHO);
			Sound.FAJONFGJBPD();
		});
		return sequence;
	}

	public void End()
	{
		if (_dlg != null)
		{
			_dlg();
		}
		if (!Sound.ELHMADOKHHE())
		{
			SoundController.IsBackgroundMusicIntro = false;
			SoundController.KHPHDKFDCLL();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetVisible(bool value)
	{
		if (_backgroundLeft != null)
		{
			_backgroundLeft.gameObject.SetActive(value);
		}
		if (_backgroundRight != null)
		{
			_backgroundRight.gameObject.SetActive(value);
		}
		if (_label != null)
		{
			_label.gameObject.SetActive(value);
		}
	}

	private void PlayMusic()
	{
		if (!Sound.ELHMADOKHHE())
		{
			Sound.OAFCOFNOIJK(EFGJOKICKHO);
			Sound.FAJONFGJBPD();
			SoundController.IsBackgroundMusicIntro = false;
			SoundController.KHPHDKFDCLL("act", false);
		}
	}

	private void CCDGDBJLLNG()
	{
		if (!Sound.ELHMADOKHHE())
		{
			EFGJOKICKHO = Sound.EAIGFAPKILL();
			ALGOCMIFECI = EFGJOKICKHO;
		}
	}

	private void PDCFDHNFCIG(float KGJALFLDIBG)
	{
		if (_foreground != null)
		{
			Color color = _foreground.color;
			color.a = KGJALFLDIBG;
			_foreground.color = color;
		}
	}
}
