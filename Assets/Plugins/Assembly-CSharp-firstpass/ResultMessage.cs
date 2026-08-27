using System.Collections.Generic;
using System.Diagnostics;

public sealed class ResultMessage : IServerMessage, IHubMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong BIGMGMIOOMA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object EAIPEGFMFMB;

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

	public object IHFJLIONHJJ
	{
		get
		{
			return LBAIENGDLDJ();
		}
		private set
		{
			set_ReturnValue(value);
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
		return LENCKBHFKLD.Result;
	}

	public ulong HGFDDMNOPJA()
	{
		return BIGMGMIOOMA;
	}

	private void set_InvocationId(ulong value)
	{
		BIGMGMIOOMA = value;
	}

	public object LBAIENGDLDJ()
	{
		return EAIPEGFMFMB;
	}

	private void set_ReturnValue(object value)
	{
		EAIPEGFMFMB = value;
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
		if (dictionary.TryGetValue("R", out value))
		{
			set_ReturnValue(value);
		}
		if (dictionary.TryGetValue("S", out value))
		{
			set_State(value as IDictionary<string, object>);
		}
	}
}
