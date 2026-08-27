using System;
using System.Diagnostics;
using UnityEngine;

public class DefaultLogger : ILogger
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BFNKPHDJNII HGNLPOMKHHK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BIJJOEGAEBA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KFEBBIEFCEK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BOFOACNAHFL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string ANALMINIJND;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string EDENEHELCIH;

	public BFNKPHDJNII Level
	{
		get
		{
			return PINDEKDNCNL();
		}
		set
		{
			DLDMOHEGENM(value);
		}
	}

	public string LEGHIOADBJC
	{
		get
		{
			return IOPJBEOJMLD();
		}
		set
		{
			FDKNEECBIBB(value);
		}
	}

	public string LNIJFMFOIDN
	{
		get
		{
			return AHDNMMFOFLB();
		}
		set
		{
			MIKLLLHHECB(value);
		}
	}

	public string LHGJKHCPGBM
	{
		get
		{
			return KBKEJHEPJEN();
		}
		set
		{
			LLDEKAOFIGL(value);
		}
	}

	public string BJDEHEABJGA
	{
		get
		{
			return DJACEMPJGGF();
		}
		set
		{
			IMHLPLJCJGK(value);
		}
	}

	public string FOGIBNCFGJJ
	{
		get
		{
			return ACMJHFAHMIM();
		}
		set
		{
			HHNFNCJGEKP(value);
		}
	}

	public DefaultLogger()
	{
		FDKNEECBIBB("I [{0}]: {1}");
		MIKLLLHHECB("I [{0}]: {1}");
		LLDEKAOFIGL("W [{0}]: {1}");
		IMHLPLJCJGK("Err [{0}]: {1}");
		HHNFNCJGEKP("Ex [{0}]: {1} - Message: {2}  StackTrace: {3}");
		DLDMOHEGENM((!UnityEngine.Debug.isDebugBuild) ? BFNKPHDJNII.Error : BFNKPHDJNII.Warning);
	}

	public BFNKPHDJNII PINDEKDNCNL()
	{
		return HGNLPOMKHHK;
	}

	public void DLDMOHEGENM(BFNKPHDJNII value)
	{
		HGNLPOMKHHK = value;
	}

	public string IOPJBEOJMLD()
	{
		return BIJJOEGAEBA;
	}

	public void FDKNEECBIBB(string value)
	{
		BIJJOEGAEBA = value;
	}

	public string AHDNMMFOFLB()
	{
		return KFEBBIEFCEK;
	}

	public void MIKLLLHHECB(string value)
	{
		KFEBBIEFCEK = value;
	}

	public string KBKEJHEPJEN()
	{
		return BOFOACNAHFL;
	}

	public void LLDEKAOFIGL(string value)
	{
		BOFOACNAHFL = value;
	}

	public string DJACEMPJGGF()
	{
		return ANALMINIJND;
	}

	public void IMHLPLJCJGK(string value)
	{
		ANALMINIJND = value;
	}

	public string ACMJHFAHMIM()
	{
		return EDENEHELCIH;
	}

	public void HHNFNCJGEKP(string value)
	{
		EDENEHELCIH = value;
	}

	public void JMHHKELODIO(string HMHPCGBCNGI, string POOAFNBCFHM)
	{
		if (PINDEKDNCNL() <= BFNKPHDJNII.All)
		{
			try
			{
				AdvLog.Log(string.Format(IOPJBEOJMLD(), HMHPCGBCNGI, POOAFNBCFHM));
			}
			catch
			{
			}
		}
	}

	public void KDAFBLAKBMI(string HMHPCGBCNGI, string EMBBNNBFODN)
	{
		if (PINDEKDNCNL() <= BFNKPHDJNII.Information)
		{
			try
			{
				AdvLog.Log(string.Format(AHDNMMFOFLB(), HMHPCGBCNGI, EMBBNNBFODN));
			}
			catch
			{
			}
		}
	}

	public void GLCKHLCAPIN(string HMHPCGBCNGI, string EPMNBLHHAHF)
	{
		if (PINDEKDNCNL() <= BFNKPHDJNII.Warning)
		{
			try
			{
				AdvLog.LOPHFKMOPAA(string.Format(KBKEJHEPJEN(), HMHPCGBCNGI, EPMNBLHHAHF));
			}
			catch
			{
			}
		}
	}

	public void Error(string HMHPCGBCNGI, string KEPBNIIECPN)
	{
		if (PINDEKDNCNL() <= BFNKPHDJNII.Error)
		{
			try
			{
				AdvLog.CCOFFJPPAKC(string.Format(DJACEMPJGGF(), HMHPCGBCNGI, KEPBNIIECPN));
			}
			catch
			{
			}
		}
	}

	public void COHEDILAHFD(string HMHPCGBCNGI, string CKEHOEGLMBM, Exception MPFFFAOGBJE)
	{
		if (PINDEKDNCNL() <= BFNKPHDJNII.Exception)
		{
			try
			{
				AdvLog.CCOFFJPPAKC(string.Format(ACMJHFAHMIM(), HMHPCGBCNGI, CKEHOEGLMBM, (MPFFFAOGBJE == null) ? "null" : MPFFFAOGBJE.Message, (MPFFFAOGBJE == null) ? "null" : MPFFFAOGBJE.StackTrace));
			}
			catch
			{
			}
		}
	}
}
