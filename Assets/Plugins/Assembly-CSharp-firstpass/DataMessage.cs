using System.Diagnostics;

public sealed class DataMessage : IServerMessage
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object JFKBADLJJBM;

	public LENCKBHFKLD get_Type()
	{
		return LENCKBHFKLD.Data;
	}

	public object CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	private void set_Data(object value)
	{
		JFKBADLJJBM = value;
	}

	void IServerMessage.Parse(object data)
	{
		set_Data(data);
	}
}
