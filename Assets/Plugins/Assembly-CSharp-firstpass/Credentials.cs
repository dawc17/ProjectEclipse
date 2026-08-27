using System.Diagnostics;

public sealed class Credentials
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BMBGFBGIAPL KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string MHIOHGELAGB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FDPDIGOJOOG;

	public string OEDGKJNJCEA
	{
		get
		{
			return BFFCEKDPNAM();
		}
		private set
		{
			IHIOOLDEDBN(value);
		}
	}

	public string LCIENEOINCL
	{
		get
		{
			return LDEFEGOBBGO();
		}
		private set
		{
			EOMDIHIOGDO(value);
		}
	}

	public Credentials(string IFCOOFDKDGL, string AODNGDGJCMD)
		: this(BMBGFBGIAPL.Unknown, IFCOOFDKDGL, AODNGDGJCMD)
	{
	}

	public Credentials(BMBGFBGIAPL LFLGCDNKNJI, string IFCOOFDKDGL, string AODNGDGJCMD)
	{
		set_Type(LFLGCDNKNJI);
		IHIOOLDEDBN(IFCOOFDKDGL);
		EOMDIHIOGDO(AODNGDGJCMD);
	}

	public BMBGFBGIAPL get_Type()
	{
		return KAHHEBMBCFA;
	}

	private void set_Type(BMBGFBGIAPL value)
	{
		KAHHEBMBCFA = value;
	}

	public string BFFCEKDPNAM()
	{
		return MHIOHGELAGB;
	}

	private void IHIOOLDEDBN(string value)
	{
		MHIOHGELAGB = value;
	}

	public string LDEFEGOBBGO()
	{
		return FDPDIGOJOOG;
	}

	private void EOMDIHIOGDO(string value)
	{
		FDPDIGOJOOG = value;
	}
}
