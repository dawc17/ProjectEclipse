using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class AudioSettings
{
	public delegate void NKLBOBJEEDA(float JIJAJFEJJHK);

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private NKLBOBJEEDA SoundsVolumeChanged;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private NKLBOBJEEDA MusicVolumeChanged;

	private float FFFFAMKMCLM;

	private float BNPMJIOBENC;

	private float IOJKBCBFHKJ;

	private bool _muted;

	public bool KHHDJOHAKFO
	{
		get
		{
			return NJOFCALNKMF();
		}
		set
		{
			set_Muted(value);
		}
	}

	public float MABNKFFKBKN
	{
		get
		{
			return LFDFKPHKEGJ();
		}
		set
		{
			set_MasterVolume(value);
		}
	}

	public float ECCOGGCFLPF
	{
		get
		{
			return NBHPABEBLOP();
		}
		set
		{
			JOFLPDCONNC(value);
		}
	}

	public float JBNOHFLLGPL
	{
		get
		{
			return EAIGFAPKILL();
		}
		set
		{
			OAFCOFNOIJK(value);
		}
	}

	public event NKLBOBJEEDA KEAGHEHDMEH
	{
		add
		{
			PGIIMEJAHBH(value);
		}
		remove
		{
			FMDGLOPCGEP(value);
		}
	}

	public event NKLBOBJEEDA CHMLIHIAEHM
	{
		add
		{
			IKJIMIMPNJP(value);
		}
		remove
		{
			PPJOIHOPHFF(value);
		}
	}

	internal AudioSettings()
	{
		FFFFAMKMCLM = PlayerPrefs.GetFloat("_masterVolume", 1f);
		IOJKBCBFHKJ = PlayerPrefs.GetFloat("_musicVolume", 1f);
		BNPMJIOBENC = PlayerPrefs.GetFloat("_soundsVolume", 1f);
		_muted = PlayerPrefs.GetInt("_muted", 0) == 1;
	}

	public void PGIIMEJAHBH(NKLBOBJEEDA value)
	{
		NKLBOBJEEDA nKLBOBJEEDA = SoundsVolumeChanged;
		NKLBOBJEEDA nKLBOBJEEDA2;
		do
		{
			nKLBOBJEEDA2 = nKLBOBJEEDA;
			nKLBOBJEEDA = Interlocked.CompareExchange(ref SoundsVolumeChanged, (NKLBOBJEEDA)Delegate.Combine(nKLBOBJEEDA2, value), nKLBOBJEEDA);
		}
		while ((object)nKLBOBJEEDA != nKLBOBJEEDA2);
	}

	public void FMDGLOPCGEP(NKLBOBJEEDA value)
	{
		NKLBOBJEEDA nKLBOBJEEDA = SoundsVolumeChanged;
		NKLBOBJEEDA nKLBOBJEEDA2;
		do
		{
			nKLBOBJEEDA2 = nKLBOBJEEDA;
			nKLBOBJEEDA = Interlocked.CompareExchange(ref SoundsVolumeChanged, (NKLBOBJEEDA)Delegate.Remove(nKLBOBJEEDA2, value), nKLBOBJEEDA);
		}
		while ((object)nKLBOBJEEDA != nKLBOBJEEDA2);
	}

	public void IKJIMIMPNJP(NKLBOBJEEDA value)
	{
		NKLBOBJEEDA nKLBOBJEEDA = MusicVolumeChanged;
		NKLBOBJEEDA nKLBOBJEEDA2;
		do
		{
			nKLBOBJEEDA2 = nKLBOBJEEDA;
			nKLBOBJEEDA = Interlocked.CompareExchange(ref MusicVolumeChanged, (NKLBOBJEEDA)Delegate.Combine(nKLBOBJEEDA2, value), nKLBOBJEEDA);
		}
		while ((object)nKLBOBJEEDA != nKLBOBJEEDA2);
	}

	public void PPJOIHOPHFF(NKLBOBJEEDA value)
	{
		NKLBOBJEEDA nKLBOBJEEDA = MusicVolumeChanged;
		NKLBOBJEEDA nKLBOBJEEDA2;
		do
		{
			nKLBOBJEEDA2 = nKLBOBJEEDA;
			nKLBOBJEEDA = Interlocked.CompareExchange(ref MusicVolumeChanged, (NKLBOBJEEDA)Delegate.Remove(nKLBOBJEEDA2, value), nKLBOBJEEDA);
		}
		while ((object)nKLBOBJEEDA != nKLBOBJEEDA2);
	}

	public bool NJOFCALNKMF()
	{
		return _muted;
	}

	public void set_Muted(bool value)
	{
		if (value != _muted)
		{
			_muted = value;
			BKNHDEMCBFA(NBHPABEBLOP());
			BIDFNHMGFKM(NBHPABEBLOP());
			PlayerPrefs.SetInt("_muted", _muted ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public float LFDFKPHKEGJ()
	{
		return (!NJOFCALNKMF()) ? FFFFAMKMCLM : 0f;
	}

	public void set_MasterVolume(float value)
	{
		value = Mathf.Clamp01(value);
		if (!(Math.Abs(value - FFFFAMKMCLM) < 0.01f))
		{
			FFFFAMKMCLM = value;
			BKNHDEMCBFA(NBHPABEBLOP());
			BIDFNHMGFKM(NBHPABEBLOP());
			PlayerPrefs.SetFloat("_masterVolume", FFFFAMKMCLM);
			PlayerPrefs.Save();
		}
	}

	public float NBHPABEBLOP()
	{
		return BNPMJIOBENC * LFDFKPHKEGJ();
	}

	public void JOFLPDCONNC(float value)
	{
		value = Mathf.Clamp01(value);
		if (!(Math.Abs(value - BNPMJIOBENC) < 0.01f))
		{
			BNPMJIOBENC = Mathf.Clamp01(value);
			BKNHDEMCBFA(NBHPABEBLOP());
			PlayerPrefs.SetFloat("_soundsVolume", BNPMJIOBENC);
			PlayerPrefs.Save();
		}
	}

	public float EAIGFAPKILL()
	{
		return IOJKBCBFHKJ * LFDFKPHKEGJ();
	}

	public void OAFCOFNOIJK(float value)
	{
		value = Mathf.Clamp01(value);
		if (!(Math.Abs(value - IOJKBCBFHKJ) < 0.01f))
		{
			IOJKBCBFHKJ = Mathf.Clamp01(value);
			BIDFNHMGFKM(NBHPABEBLOP());
			PlayerPrefs.SetFloat("_musicVolume", IOJKBCBFHKJ);
			PlayerPrefs.Save();
		}
	}

	protected virtual void BKNHDEMCBFA(float JIJAJFEJJHK)
	{
		if (SoundsVolumeChanged != null)
		{
			SoundsVolumeChanged(JIJAJFEJJHK);
		}
	}

	protected virtual void BIDFNHMGFKM(float JIJAJFEJJHK)
	{
		if (MusicVolumeChanged != null)
		{
			MusicVolumeChanged(JIJAJFEJJHK);
		}
	}
}
