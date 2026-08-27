using System;
using System.Diagnostics;

public sealed class HTTPCacheMaintananceParams
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan DKEPNBDPBND;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong DJJOJLCMACI;

	public TimeSpan BEIDGPIMGBB
	{
		get
		{
			return DKAPKJDOAEJ();
		}
		private set
		{
			set_DeleteOlder(value);
		}
	}

	public ulong AJFGLJKCBLM
	{
		get
		{
			return GAJGIIJPPBF();
		}
		private set
		{
			set_MaxCacheSize(value);
		}
	}

	public HTTPCacheMaintananceParams(TimeSpan HJELDPKENPC, ulong EBFOBGKCGJP)
	{
		set_DeleteOlder(HJELDPKENPC);
		set_MaxCacheSize(EBFOBGKCGJP);
	}

	public TimeSpan DKAPKJDOAEJ()
	{
		return DKEPNBDPBND;
	}

	private void set_DeleteOlder(TimeSpan value)
	{
		DKEPNBDPBND = value;
	}

	public ulong GAJGIIJPPBF()
	{
		return DJJOJLCMACI;
	}

	private void set_MaxCacheSize(ulong value)
	{
		DJJOJLCMACI = value;
	}
}
