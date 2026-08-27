using System;
using System.Diagnostics;

public sealed class HTTPProxy
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri KLNINJOAAPG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Credentials ACGBCDDPEGA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool PBCPBGAKFKB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool PCBBBGCHHMJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ADBGDHAHGPI;

	public Uri IJMOLOMMEBG
	{
		get
		{
			return DNIJHGFINDG();
		}
		set
		{
			set_Address(value);
		}
	}

	public Credentials MADDPLIJOIP
	{
		get
		{
			return HPKPFEOBIOC();
		}
		set
		{
			PJELDABIDCA(value);
		}
	}

	public bool BKJOAAJJEAN
	{
		get
		{
			return JDBFAABAEIL();
		}
		set
		{
			KEPJCOKKHLA(value);
		}
	}

	public bool MEBCCPJFKEE
	{
		get
		{
			return EGNDGIEKOGA();
		}
		set
		{
			EHCHFMOJIAP(value);
		}
	}

	public bool BEJENMBNEPC
	{
		get
		{
			return OHCGKBPPMEN();
		}
		set
		{
			NFJNADEMMAM(value);
		}
	}

	public HTTPProxy()
		: this(null, null, false)
	{
	}

	public HTTPProxy(Uri IKHEAOEKLHL)
		: this(IKHEAOEKLHL, null, false)
	{
	}

	public HTTPProxy(Uri IKHEAOEKLHL, Credentials JKBAHGNLECO)
		: this(IKHEAOEKLHL, JKBAHGNLECO, false)
	{
	}

	public HTTPProxy(Uri IKHEAOEKLHL, Credentials JKBAHGNLECO, bool CBLAANPNDHE)
		: this(IKHEAOEKLHL, JKBAHGNLECO, CBLAANPNDHE, true)
	{
	}

	public HTTPProxy(Uri IKHEAOEKLHL, Credentials JKBAHGNLECO, bool CBLAANPNDHE, bool HAIHALMOEFD)
		: this(IKHEAOEKLHL, JKBAHGNLECO, CBLAANPNDHE, true, true)
	{
	}

	public HTTPProxy(Uri IKHEAOEKLHL, Credentials JKBAHGNLECO, bool CBLAANPNDHE, bool HAIHALMOEFD, bool AGCDFHONOEF)
	{
		set_Address(IKHEAOEKLHL);
		PJELDABIDCA(JKBAHGNLECO);
		KEPJCOKKHLA(CBLAANPNDHE);
		EHCHFMOJIAP(HAIHALMOEFD);
		NFJNADEMMAM(AGCDFHONOEF);
	}

	public Uri DNIJHGFINDG()
	{
		return KLNINJOAAPG;
	}

	public void set_Address(Uri value)
	{
		KLNINJOAAPG = value;
	}

	public Credentials HPKPFEOBIOC()
	{
		return ACGBCDDPEGA;
	}

	public void PJELDABIDCA(Credentials value)
	{
		ACGBCDDPEGA = value;
	}

	public bool JDBFAABAEIL()
	{
		return PBCPBGAKFKB;
	}

	public void KEPJCOKKHLA(bool value)
	{
		PBCPBGAKFKB = value;
	}

	public bool EGNDGIEKOGA()
	{
		return PCBBBGCHHMJ;
	}

	public void EHCHFMOJIAP(bool value)
	{
		PCBBBGCHHMJ = value;
	}

	public bool OHCGKBPPMEN()
	{
		return ADBGDHAHGPI;
	}

	public void NFJNADEMMAM(bool value)
	{
		ADBGDHAHGPI = value;
	}
}
