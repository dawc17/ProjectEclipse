using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public sealed class SocketOptions
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool MODPJMLMIAO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int OKJFNLAHGJL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan HCEBEOKEPAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan BCGIAIMMFLA;

	private float randomizationFactor;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan OCOBNPGODHJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ACPBHKMPPKK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, string> LGJADADCGHL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool IJJGBHCLDHH;

	private string BuiltQueryParams;

	public bool LIAJIPFMBJA
	{
		get
		{
			return AMHAEEBHDFE();
		}
		set
		{
			MKNMNMLHOGA(value);
		}
	}

	public int LBAHDILDNGE
	{
		get
		{
			return GIBLAAJPHLP();
		}
		set
		{
			set_ReconnectionAttempts(value);
		}
	}

	public TimeSpan EJHPNIHPKLH
	{
		get
		{
			return CHAGLLGOKKE();
		}
		set
		{
			PFIINOHHLPF(value);
		}
	}

	public TimeSpan CAKPOBKOFJE
	{
		get
		{
			return ODAHMCJKEIL();
		}
		set
		{
			OBLKKCBCMEL(value);
		}
	}

	public float IKKNDCBGMNC
	{
		get
		{
			return JNJBBOELNIG();
		}
		set
		{
			set_RandomizationFactor(value);
		}
	}

	public TimeSpan BEOBDJHNHIO
	{
		get
		{
			return FJKGKLJGIJI();
		}
		set
		{
			DKLGPGDJPGO(value);
		}
	}

	public bool AIKNLBNELKF
	{
		get
		{
			return JLCKLGDFADC();
		}
		set
		{
			AHGIJFEGONK(value);
		}
	}

	public Dictionary<string, string> CNGINADLODB
	{
		get
		{
			return MONGJAOIELO();
		}
		set
		{
			set_AdditionalQueryParams(value);
		}
	}

	public bool KOCIJKDENMF
	{
		get
		{
			return DKJAFHAOKDB();
		}
		set
		{
			set_QueryParamsOnlyForHandshake(value);
		}
	}

	public SocketOptions()
	{
		MKNMNMLHOGA(true);
		set_ReconnectionAttempts(int.MaxValue);
		PFIINOHHLPF(TimeSpan.FromMilliseconds(1000.0));
		OBLKKCBCMEL(TimeSpan.FromMilliseconds(5000.0));
		set_RandomizationFactor(0.5f);
		DKLGPGDJPGO(TimeSpan.FromMilliseconds(20000.0));
		AHGIJFEGONK(true);
		set_QueryParamsOnlyForHandshake(true);
	}

	public bool AMHAEEBHDFE()
	{
		return MODPJMLMIAO;
	}

	public void MKNMNMLHOGA(bool value)
	{
		MODPJMLMIAO = value;
	}

	public int GIBLAAJPHLP()
	{
		return OKJFNLAHGJL;
	}

	public void set_ReconnectionAttempts(int value)
	{
		OKJFNLAHGJL = value;
	}

	public TimeSpan CHAGLLGOKKE()
	{
		return HCEBEOKEPAB;
	}

	public void PFIINOHHLPF(TimeSpan value)
	{
		HCEBEOKEPAB = value;
	}

	public TimeSpan ODAHMCJKEIL()
	{
		return BCGIAIMMFLA;
	}

	public void OBLKKCBCMEL(TimeSpan value)
	{
		BCGIAIMMFLA = value;
	}

	public float JNJBBOELNIG()
	{
		return randomizationFactor;
	}

	public void set_RandomizationFactor(float value)
	{
		randomizationFactor = Math.Min(1f, Math.Max(0f, value));
	}

	public TimeSpan FJKGKLJGIJI()
	{
		return OCOBNPGODHJ;
	}

	public void DKLGPGDJPGO(TimeSpan value)
	{
		OCOBNPGODHJ = value;
	}

	public bool JLCKLGDFADC()
	{
		return ACPBHKMPPKK;
	}

	public void AHGIJFEGONK(bool value)
	{
		ACPBHKMPPKK = value;
	}

	public Dictionary<string, string> MONGJAOIELO()
	{
		return LGJADADCGHL;
	}

	public void set_AdditionalQueryParams(Dictionary<string, string> value)
	{
		LGJADADCGHL = value;
	}

	public bool DKJAFHAOKDB()
	{
		return IJJGBHCLDHH;
	}

	public void set_QueryParamsOnlyForHandshake(bool value)
	{
		IJJGBHCLDHH = value;
	}

	internal string LEKAOBKGMPF()
	{
		if (MONGJAOIELO() == null || MONGJAOIELO().Count == 0)
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(BuiltQueryParams))
		{
			return BuiltQueryParams;
		}
		StringBuilder stringBuilder = new StringBuilder(MONGJAOIELO().Count * 4);
		foreach (KeyValuePair<string, string> item in MONGJAOIELO())
		{
			stringBuilder.Append("&");
			stringBuilder.Append(item.Key);
			if (!string.IsNullOrEmpty(item.Value))
			{
				stringBuilder.Append("=");
				stringBuilder.Append(item.Value);
			}
		}
		return BuiltQueryParams = stringBuilder.ToString();
	}
}
