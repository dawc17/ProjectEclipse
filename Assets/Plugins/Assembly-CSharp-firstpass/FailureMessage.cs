using System.Collections.Generic;
using System.Diagnostics;

public sealed class FailureMessage : IServerMessage, IHubMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong BIGMGMIOOMA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KNLKFJBNGOC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OPFGDBPMJLC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IDictionary<string, object> DAEBIGKGOGL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string AKBMKGOFAON;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IDictionary<string, object> MKHEFCIEOCA;

	public ulong EBFDNDACIMG
	{
		get
		{
			return HGFDDMNOPJA();
		}
		private set
		{
			set_InvocationId(value);
		}
	}

	public bool DIDEDJPCHNE
	{
		get
		{
			return MCCOIIEILFP();
		}
		private set
		{
			set_IsHubError(value);
		}
	}

	public string FHIJGLJLLGL
	{
		get
		{
			return LCHHLEOPONE();
		}
		private set
		{
			JBOLKCMBOLO(value);
		}
	}

	public IDictionary<string, object> AEONJODMKMG
	{
		get
		{
			return GNIMEFNENEK();
		}
		private set
		{
			JBIBAFGIOCB(value);
		}
	}

	public string IHJKCGACBBD
	{
		get
		{
			return DLHBLMLNKJF();
		}
		private set
		{
			MGOGKLCPLFL(value);
		}
	}

	public IDictionary<string, object> AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		private set
		{
			set_State(value);
		}
	}

	public LENCKBHFKLD get_Type()
	{
		return LENCKBHFKLD.Failure;
	}

	public ulong HGFDDMNOPJA()
	{
		return BIGMGMIOOMA;
	}

	private void set_InvocationId(ulong value)
	{
		BIGMGMIOOMA = value;
	}

	public bool MCCOIIEILFP()
	{
		return KNLKFJBNGOC;
	}

	private void set_IsHubError(bool value)
	{
		KNLKFJBNGOC = value;
	}

	public string LCHHLEOPONE()
	{
		return OPFGDBPMJLC;
	}

	private void JBOLKCMBOLO(string value)
	{
		OPFGDBPMJLC = value;
	}

	public IDictionary<string, object> GNIMEFNENEK()
	{
		return DAEBIGKGOGL;
	}

	private void JBIBAFGIOCB(IDictionary<string, object> value)
	{
		DAEBIGKGOGL = value;
	}

	public string DLHBLMLNKJF()
	{
		return AKBMKGOFAON;
	}

	private void MGOGKLCPLFL(string value)
	{
		AKBMKGOFAON = value;
	}

	public IDictionary<string, object> FLBBFDNHJAJ()
	{
		return MKHEFCIEOCA;
	}

	private void set_State(IDictionary<string, object> value)
	{
		MKHEFCIEOCA = value;
	}

	void IServerMessage.Parse(object data)
	{
		IDictionary<string, object> dictionary = data as IDictionary<string, object>;
		set_InvocationId(ulong.Parse(dictionary["I"].ToString()));
		object value;
		if (dictionary.TryGetValue("E", out value))
		{
			JBOLKCMBOLO(value.ToString());
		}
		if (dictionary.TryGetValue("H", out value))
		{
			set_IsHubError(int.Parse(value.ToString()) == 1);
		}
		if (dictionary.TryGetValue("D", out value))
		{
			JBIBAFGIOCB(value as IDictionary<string, object>);
		}
		if (dictionary.TryGetValue("T", out value))
		{
			MGOGKLCPLFL(value.ToString());
		}
		if (dictionary.TryGetValue("S", out value))
		{
			set_State(value as IDictionary<string, object>);
		}
	}
}
