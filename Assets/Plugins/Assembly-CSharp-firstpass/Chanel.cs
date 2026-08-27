using System.Collections.Generic;
using System.Diagnostics;
using Nekki.Audio;
using UnityEngine;

internal class Chanel
{
	private readonly Dictionary<string, AudioUnit> _active = new Dictionary<string, AudioUnit>();

	private Dictionary<string, AudioClip> _clips;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool GHKEHHMNBLO;

	public bool JNCMBHENOIM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int CLPCIBMBDDP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float FGBOBKBLFBP;

	public bool MKAGGEPMEHF
	{
		get
		{
			return AGEEHOABFFF();
		}
		private set
		{
			set_IsMusic(value);
		}
	}

	public bool EELNODBOPAB
	{
		get
		{
			return MJAONMOCKNO();
		}
	}

	public int GJCOGFOJAEB
	{
		get
		{
			return ANAECCFDHMI();
		}
		private set
		{
			set_ID(value);
		}
	}

	public bool BDJPLHOKIPF
	{
		get
		{
			return EGCDMGAFFEE();
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

	internal Chanel(int OKNNNLIPODI, bool MHAFPAHIFKP, Dictionary<string, AudioClip> OCEMOHJPDLK)
	{
		set_IsMusic(MHAFPAHIFKP);
		set_ID(OKNNNLIPODI);
		_clips = OCEMOHJPDLK;
		JNCMBHENOIM = false;
		set_MasterVolume(1f);
	}

	public bool AGEEHOABFFF()
	{
		return GHKEHHMNBLO;
	}

	private void set_IsMusic(bool value)
	{
		GHKEHHMNBLO = value;
	}

	public bool MJAONMOCKNO()
	{
		return !AGEEHOABFFF();
	}

	public int ANAECCFDHMI()
	{
		return CLPCIBMBDDP;
	}

	private void set_ID(int value)
	{
		CLPCIBMBDDP = value;
	}

	public bool EGCDMGAFFEE()
	{
		return _active.Count != 0;
	}

	public float LFDFKPHKEGJ()
	{
		return FGBOBKBLFBP;
	}

	public void set_MasterVolume(float value)
	{
		FGBOBKBLFBP = value;
	}

	internal void EACCANOGCFL(PlayCommand LEKEGLMDAHA)
	{
		AudioClip audioClip = ((!_clips.ContainsKey(LEKEGLMDAHA.JIKANFGDMJN())) ? null : _clips[LEKEGLMDAHA.JIKANFGDMJN()]);
		if (!audioClip)
		{
			return;
		}
		if (!LEKEGLMDAHA.FKGNNDDNJDN())
		{
			IEHPNJOOPCG();
		}
		if (_active.ContainsKey(LEKEGLMDAHA.JIKANFGDMJN()))
		{
			_active[LEKEGLMDAHA.JIKANFGDMJN()].Init(this, LEKEGLMDAHA, audioClip);
			_active[LEKEGLMDAHA.JIKANFGDMJN()].set_IsMute(JNCMBHENOIM);
			return;
		}
		AudioUnit audioUnit = OverallUnitPool.NGDGDCCFONE();
		if ((bool)audioUnit)
		{
			audioUnit.Init(this, LEKEGLMDAHA, audioClip);
			audioUnit.set_IsMute(JNCMBHENOIM);
			_active.Add(LEKEGLMDAHA.JIKANFGDMJN(), audioUnit);
		}
	}

	internal void Pause(bool KCANPMPILKI)
	{
		foreach (AudioUnit value in _active.Values)
		{
			if (KCANPMPILKI)
			{
				value.Pause();
			}
			else
			{
				value.UnPause();
			}
		}
	}

	internal void Pause(bool KCANPMPILKI, string LGLFOBEIPKB)
	{
		if (_active.ContainsKey(LGLFOBEIPKB))
		{
			if (KCANPMPILKI)
			{
				_active[LGLFOBEIPKB].Pause();
			}
			else
			{
				_active[LGLFOBEIPKB].UnPause();
			}
		}
	}

	public void FreeUnit(AudioUnit PNJCPKNCLCP)
	{
		foreach (KeyValuePair<string, AudioUnit> item in _active)
		{
			if (item.Value == PNJCPKNCLCP)
			{
				_active.Remove(item.Key);
				break;
			}
		}
	}

	public void IEHPNJOOPCG(bool BJIOMMPCLEA = false)
	{
		foreach (AudioUnit value in _active.Values)
		{
			value.Stop(BJIOMMPCLEA);
		}
		_active.Clear();
	}

	public void LKLAFKJFNIP()
	{
		JNCMBHENOIM = true;
		foreach (AudioUnit value in _active.Values)
		{
			value.set_IsMute(true);
		}
	}

	public void PNNNNJBKONA()
	{
		JNCMBHENOIM = false;
		foreach (AudioUnit value in _active.Values)
		{
			value.set_IsMute(false);
		}
	}
}
