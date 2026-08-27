using System.Diagnostics;
using UnityEngine;

public class PlayCommand
{
	private float LNBIJCDHCIA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int IICMKGHFCHE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string CDCOKIBCAML;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool FPKOIEJFJOK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ECNEEOIOLLG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool GHKEHHMNBLO;

	private AudioSettings HPOJCCHMKOP;

	public int GJKIMBMKCGN
	{
		get
		{
			return OKFNIMIANKK();
		}
		private set
		{
			set_ChanelID(value);
		}
	}

	public string MGCIEBJAJAL
	{
		get
		{
			return JIKANFGDMJN();
		}
		private set
		{
			set_Sound(value);
		}
	}

	public bool OGBDAKHHGID
	{
		get
		{
			return ADCBILEEEEO();
		}
		private set
		{
			KFEBCEOJLEI(value);
		}
	}

	public bool BDOLJOBHMNJ
	{
		get
		{
			return FKGNNDDNJDN();
		}
		private set
		{
			KHKMBMCPLIC(value);
		}
	}

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

	public float FLJPEPPDICN
	{
		get
		{
			return AFKMLMCCJLI();
		}
		set
		{
			EGGPLDIFDBC(value);
		}
	}

	public PlayCommand(int ADNDLGKIJJK, string LGLFOBEIPKB, bool KKHJAJFEPPA, bool HBCDAPJLKOJ, float JIJAJFEJJHK)
	{
		EGGPLDIFDBC(JIJAJFEJJHK);
		KHKMBMCPLIC(HBCDAPJLKOJ);
		KFEBCEOJLEI(KKHJAJFEPPA);
		set_Sound(LGLFOBEIPKB);
		set_ChanelID(ADNDLGKIJJK);
	}

	public int OKFNIMIANKK()
	{
		return IICMKGHFCHE;
	}

	private void set_ChanelID(int value)
	{
		IICMKGHFCHE = value;
	}

	public string JIKANFGDMJN()
	{
		return CDCOKIBCAML;
	}

	private void set_Sound(string value)
	{
		CDCOKIBCAML = value;
	}

	public bool ADCBILEEEEO()
	{
		return FPKOIEJFJOK;
	}

	private void KFEBCEOJLEI(bool value)
	{
		FPKOIEJFJOK = value;
	}

	public bool FKGNNDDNJDN()
	{
		return ECNEEOIOLLG;
	}

	private void KHKMBMCPLIC(bool value)
	{
		ECNEEOIOLLG = value;
	}

	public bool AGEEHOABFFF()
	{
		return GHKEHHMNBLO;
	}

	private void set_IsMusic(bool value)
	{
		GHKEHHMNBLO = value;
	}

	public float AFKMLMCCJLI()
	{
		return LNBIJCDHCIA * ((!AGEEHOABFFF()) ? HPOJCCHMKOP.NBHPABEBLOP() : HPOJCCHMKOP.EAIGFAPKILL());
	}

	public void EGGPLDIFDBC(float value)
	{
		LNBIJCDHCIA = Mathf.Clamp01(value);
	}

	internal void KICCODIDHAP(bool MHAFPAHIFKP)
	{
		set_IsMusic(MHAFPAHIFKP);
	}

	internal void JJOFEEGNEDM(AudioSettings CCKFFGJGEJE)
	{
		HPOJCCHMKOP = CCKFFGJGEJE;
	}
}
