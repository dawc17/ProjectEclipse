using System.Diagnostics;

public sealed class KeyValuePair
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DPKIBLDMMKP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string IELPCLONGKP;

	public string ENFBNOGCCBH
	{
		get
		{
			return AENLBNDAEKB();
		}
		set
		{
			set_Key(value);
		}
	}

	public KeyValuePair(string KGBGENDIMBC)
	{
		set_Key(KGBGENDIMBC);
	}

	public string AENLBNDAEKB()
	{
		return DPKIBLDMMKP;
	}

	public void set_Key(string value)
	{
		DPKIBLDMMKP = value;
	}

	public string OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	public void set_Value(string value)
	{
		IELPCLONGKP = value;
	}

	public override string ToString()
	{
		if (!string.IsNullOrEmpty(OEAKCOHMIHH()))
		{
			return AENLBNDAEKB() + '=' + OEAKCOHMIHH();
		}
		return AENLBNDAEKB();
	}
}
