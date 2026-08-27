using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class MethodCallMessage : IServerMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JFIMJNHKJHA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FPNIGCIIIJK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object[] BADMNBMBKIB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IDictionary<string, object> MKHEFCIEOCA;

	public string LHDEDFFGBHI
	{
		get
		{
			return GDANEAJOFMP();
		}
		private set
		{
			FKEKBJKDNKN(value);
		}
	}

	public string Method
	{
		get
		{
			return OIPIMPLLDCP();
		}
		private set
		{
			GOLMHEHNMDE(value);
		}
	}

	public object[] AIANPCBJCKN
	{
		get
		{
			return FNKPHEHFKEI();
		}
		private set
		{
			set_Arguments(value);
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
		return LENCKBHFKLD.MethodCall;
	}

	public string GDANEAJOFMP()
	{
		return JFIMJNHKJHA;
	}

	private void FKEKBJKDNKN(string value)
	{
		JFIMJNHKJHA = value;
	}

	public string OIPIMPLLDCP()
	{
		return FPNIGCIIIJK;
	}

	private void GOLMHEHNMDE(string value)
	{
		FPNIGCIIIJK = value;
	}

	public object[] FNKPHEHFKEI()
	{
		return BADMNBMBKIB;
	}

	private void set_Arguments(object[] value)
	{
		BADMNBMBKIB = value;
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
		FKEKBJKDNKN(dictionary["H"].ToString());
		GOLMHEHNMDE(dictionary["M"].ToString());
		List<object> list = new List<object>();
		foreach (object item in dictionary["A"] as IEnumerable)
		{
			list.Add(item);
		}
		set_Arguments(list.ToArray());
		object value;
		if (dictionary.TryGetValue("S", out value))
		{
			set_State(value as IDictionary<string, object>);
		}
	}
}
