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
    private DG.Tweening.Sequence _presentation;
    private bool _finished;
    private bool _musicCaptured;

	// best guess for name
	private float _originalMusicVolume;

	private float ALGOCMIFECI;

	public static EnterScreen Create()
	{
		EnterScreen original = Resources.Load<EnterScreen>("Prefabs/Map/EnterScreen");
		return UnityEngine.Object.Instantiate(original);
	}

	public void Init(List<KeyValuePair<string, int>> IGLEKOAILHD, Action ODDEOFKLIAG)
	{
        InitLines(IGLEKOAILHD, ODDEOFKLIAG, false);
    }

    public void InitResolved(List<KeyValuePair<string, int>> lines, Action completed)
    {
        InitLines(lines, completed, true);
    }

    private void InitLines(List<KeyValuePair<string, int>> IGLEKOAILHD, Action ODDEOFKLIAG, bool resolved)
	{
		_dlg = ODDEOFKLIAG;
		SetVisible(false);
		CCDGDBJLLNG();
		PDCFDHNFCIG(MIN_OPACITY);
		if (_label != null && IGLEKOAILHD.Count > 0)
		{
			SetLine(IGLEKOAILHD[0].Key, resolved);
		}
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(0f);
		foreach (KeyValuePair<string, int> MGPBPJOHMLH in IGLEKOAILHD)
		{
			sequence.AppendCallback(() =>
			{
				if (_label != null)
				{
					SetLine(MGPBPJOHMLH.Key, resolved);
				}
			});
			float interval = (float)MGPBPJOHMLH.Value / 60f;
			sequence.AppendInterval(interval);
		}
		DG.Tweening.Sequence t = MuteMusicSequence();
		DG.Tweening.Sequence s = DOTween.Sequence();
        _presentation = s;
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
        _presentation = s;
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
			Sound.OAFCOFNOIJK(_originalMusicVolume);
			Sound.FAJONFGJBPD();
		});
		return sequence;
	}

	public void End()
	{
        if (_finished) return;
        var completed = _dlg;
        FinishPresentation(false);
        // Destroy invokes OnDisable synchronously: report success before a
        // presentation owner's disable hook can interpret it as cancellation.
        try { completed?.Invoke(); }
        finally { UnityEngine.Object.Destroy(base.gameObject); }
    }

    public void Cancel()
    {
        if (_finished) return;
        FinishPresentation();
    }

    private void FinishPresentation(bool destroyObject = true)
    {
        _finished = true;
        _dlg = null;
        _presentation?.Kill();
        _presentation = null;
		if (_musicCaptured) Sound.OAFCOFNOIJK(_originalMusicVolume);
		if (_musicCaptured && !Sound.ELHMADOKHHE())
		{
			SoundController.IsBackgroundMusicIntro = false;
			SoundController.KHPHDKFDCLL();
		}
		if (destroyObject) UnityEngine.Object.Destroy(base.gameObject);
	}

    private void OnDestroy()
    {
        if (!_finished) FinishPresentation(false);
    }

    private void SetLine(string text, bool resolved)
    {
        if (!resolved) { _label.set_Alias(text); return; }
        _label.set_Alias(string.Empty);
        _label.supportRichText = false;
        _label.set_text(text);
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
			Sound.OAFCOFNOIJK(_originalMusicVolume);
			Sound.FAJONFGJBPD();
			SoundController.IsBackgroundMusicIntro = false;
			SoundController.KHPHDKFDCLL("act", false);
		}
	}

	private void CCDGDBJLLNG()
	{
		if (!Sound.ELHMADOKHHE())
		{
			_originalMusicVolume = Sound.EAIGFAPKILL();
            _musicCaptured = true;
			ALGOCMIFECI = _originalMusicVolume;
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
