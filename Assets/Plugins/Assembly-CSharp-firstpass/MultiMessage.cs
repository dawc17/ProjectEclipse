using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class MultiMessage : IServerMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NPEODIOJAMN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool MIEPCPDMMOI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string LNIDLMDCKLM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool GFLIGIBHBBD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan? CKFDPLJIPPO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private List<IServerMessage> JFKBADLJJBM;

	public string MCGBPBPLBJJ
	{
		get
		{
			return BJOOBDBFHGL();
		}
		private set
		{
			CLKCFGEAMLM(value);
		}
	}

	public bool KAPALNEODPA
	{
		get
		{
			return BPDICBIDIPO();
		}
		private set
		{
			ECPFEGDPIGL(value);
		}
	}

	public string LPEDAAHHCBD
	{
		get
		{
			return PCINJIIKLFH();
		}
		private set
		{
			JDJGDANMMHI(value);
		}
	}

	public bool MFBBCALPODO
	{
		get
		{
			return ACMAJHFOIDJ();
		}
		private set
		{
			IFKKOHGDAKA(value);
		}
	}

	public TimeSpan? EMJECEPDOJL
	{
		get
		{
			return LNCCPGIEPOH();
		}
		private set
		{
			set_PollDelay(value);
		}
	}

	public LENCKBHFKLD get_Type()
	{
		return LENCKBHFKLD.Multiple;
	}

	public string BJOOBDBFHGL()
	{
		return NPEODIOJAMN;
	}

	private void CLKCFGEAMLM(string value)
	{
		NPEODIOJAMN = value;
	}

	public bool BPDICBIDIPO()
	{
		return MIEPCPDMMOI;
	}

	private void ECPFEGDPIGL(bool value)
	{
		MIEPCPDMMOI = value;
	}

	public string PCINJIIKLFH()
	{
		return LNIDLMDCKLM;
	}

	private void JDJGDANMMHI(string value)
	{
		LNIDLMDCKLM = value;
	}

	public bool ACMAJHFOIDJ()
	{
		return GFLIGIBHBBD;
	}

	private void IFKKOHGDAKA(bool value)
	{
		GFLIGIBHBBD = value;
	}

	public TimeSpan? LNCCPGIEPOH()
	{
		return CKFDPLJIPPO;
	}

	private void set_PollDelay(TimeSpan? value)
	{
		CKFDPLJIPPO = value;
	}

	public List<IServerMessage> CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	private void set_Data(List<IServerMessage> value)
	{
		JFKBADLJJBM = value;
	}

	void IServerMessage.Parse(object data)
	{
		IDictionary<string, object> dictionary = data as IDictionary<string, object>;
		CLKCFGEAMLM(dictionary["C"].ToString());
		object value;
		if (dictionary.TryGetValue("S", out value))
		{
			ECPFEGDPIGL(int.Parse(value.ToString()) == 1);
		}
		else
		{
			ECPFEGDPIGL(false);
		}
		if (dictionary.TryGetValue("G", out value))
		{
			JDJGDANMMHI(value.ToString());
		}
		if (dictionary.TryGetValue("T", out value))
		{
			IFKKOHGDAKA(int.Parse(value.ToString()) == 1);
		}
		else
		{
			IFKKOHGDAKA(false);
		}
		if (dictionary.TryGetValue("L", out value))
		{
			set_PollDelay(TimeSpan.FromMilliseconds(double.Parse(value.ToString())));
		}
		IEnumerable enumerable = dictionary["M"] as IEnumerable;
		if (enumerable == null)
		{
			return;
		}
		set_Data(new List<IServerMessage>());
		foreach (object item in enumerable)
		{
			IDictionary<string, object> dictionary2 = item as IDictionary<string, object>;
			IServerMessage bNGPAAAKBOP = null;
			bNGPAAAKBOP = ((dictionary2 == null) ? new DataMessage() : ((!dictionary2.ContainsKey("H")) ? ((!dictionary2.ContainsKey("I")) ? ((IServerMessage)new DataMessage()) : ((IServerMessage)new ProgressMessage())) : new MethodCallMessage()));
			bNGPAAAKBOP.Parse(item);
			CHIGLEKCFFN().Add(bNGPAAAKBOP);
		}
	}
}
