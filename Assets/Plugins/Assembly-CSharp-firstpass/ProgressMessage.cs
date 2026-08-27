using System.Collections.Generic;
using System.Diagnostics;

public sealed class ProgressMessage : IServerMessage, IHubMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong BIGMGMIOOMA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double ELHMKIJGGNL;

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

	public double OIOANIMIIIA
	{
		get
		{
			return ALDEPEHMGNK();
		}
		private set
		{
			set_Progress(value);
		}
	}

	public LENCKBHFKLD get_Type()
	{
		return LENCKBHFKLD.Progress;
	}

	public ulong HGFDDMNOPJA()
	{
		return BIGMGMIOOMA;
	}

	private void set_InvocationId(ulong value)
	{
		BIGMGMIOOMA = value;
	}

	public double ALDEPEHMGNK()
	{
		return ELHMKIJGGNL;
	}

	private void set_Progress(double value)
	{
		ELHMKIJGGNL = value;
	}

	void IServerMessage.Parse(object data)
	{
		IDictionary<string, object> dictionary = data as IDictionary<string, object>;
		IDictionary<string, object> dictionary2 = dictionary["P"] as IDictionary<string, object>;
		set_InvocationId(ulong.Parse(dictionary2["I"].ToString()));
		set_Progress(double.Parse(dictionary2["D"].ToString()));
	}
}
