using System;
using System.Diagnostics;

public sealed class Message
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DGDIGIMFGMI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string MJDIHAAHDIC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string JFKBADLJJBM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan BPBMHJKAIPG;

	public string Id
	{
		get
		{
			return IMMIJJCLPBO();
		}
		internal set
		{
			MKAMABIPHEN(value);
		}
	}

	public string MOFKKABEFEB
	{
		get
		{
			return EMCEPDNKAPK();
		}
		internal set
		{
			set_Event(value);
		}
	}

	public TimeSpan DOIGOILHAKM
	{
		get
		{
			return GOOCPGAOBBH();
		}
		internal set
		{
			set_Retry(value);
		}
	}

	public string IMMIJJCLPBO()
	{
		return DGDIGIMFGMI;
	}

	internal void MKAMABIPHEN(string value)
	{
		DGDIGIMFGMI = value;
	}

	public string EMCEPDNKAPK()
	{
		return MJDIHAAHDIC;
	}

	internal void set_Event(string value)
	{
		MJDIHAAHDIC = value;
	}

	public string CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	internal void set_Data(string value)
	{
		JFKBADLJJBM = value;
	}

	public TimeSpan GOOCPGAOBBH()
	{
		return BPBMHJKAIPG;
	}

	internal void set_Retry(TimeSpan value)
	{
		BPBMHJKAIPG = value;
	}

	public override string ToString()
	{
		return string.Format("\"{0}\": \"{1}\"", EMCEPDNKAPK(), CHIGLEKCFFN());
	}
}
