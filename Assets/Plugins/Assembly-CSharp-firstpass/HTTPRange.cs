using System.Diagnostics;

public sealed class HTTPRange
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int NFMICCCCHPL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int BBINNPBELEA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int IHMKOHOINGK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CNJCHGHAAJC;

	public int MKLMNJJEEHO
	{
		get
		{
			return AHALHOCNCJK();
		}
		private set
		{
			ONABCINMKAH(value);
		}
	}

	public int KEGEPOAGBCM
	{
		get
		{
			return CCJEDKMCDHP();
		}
		private set
		{
			KJDKAHLIFEE(value);
		}
	}

	public int NHKGOHBPMJL
	{
		get
		{
			return HOKMAPAFJJA();
		}
		private set
		{
			OGCAJBCEANC(value);
		}
	}

	public bool GJHHGDAOHGK
	{
		get
		{
			return DINANCBOIMJ();
		}
		private set
		{
			set_IsValid(value);
		}
	}

	internal HTTPRange()
	{
		OGCAJBCEANC(-1);
		set_IsValid(false);
	}

	internal HTTPRange(int HDIIBKGCCNB)
	{
		OGCAJBCEANC(HDIIBKGCCNB);
		set_IsValid(false);
	}

	internal HTTPRange(int JNLLEFJLHIE, int PFOBJOCNOAP, int HDIIBKGCCNB)
	{
		ONABCINMKAH(JNLLEFJLHIE);
		KJDKAHLIFEE(PFOBJOCNOAP);
		OGCAJBCEANC(HDIIBKGCCNB);
		set_IsValid(AHALHOCNCJK() <= CCJEDKMCDHP() && HOKMAPAFJJA() > CCJEDKMCDHP());
	}

	public int AHALHOCNCJK()
	{
		return NFMICCCCHPL;
	}

	private void ONABCINMKAH(int value)
	{
		NFMICCCCHPL = value;
	}

	public int CCJEDKMCDHP()
	{
		return BBINNPBELEA;
	}

	private void KJDKAHLIFEE(int value)
	{
		BBINNPBELEA = value;
	}

	public int HOKMAPAFJJA()
	{
		return IHMKOHOINGK;
	}

	private void OGCAJBCEANC(int value)
	{
		IHMKOHOINGK = value;
	}

	public bool DINANCBOIMJ()
	{
		return CNJCHGHAAJC;
	}

	private void set_IsValid(bool value)
	{
		CNJCHGHAAJC = value;
	}

	public override string ToString()
	{
		return string.Format("{0}-{1}/{2} (valid: {3})", AHALHOCNCJK(), CCJEDKMCDHP(), HOKMAPAFJJA(), DINANCBOIMJ());
	}
}
