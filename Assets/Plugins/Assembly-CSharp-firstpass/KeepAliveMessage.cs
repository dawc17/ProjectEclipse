public sealed class KeepAliveMessage : IServerMessage
{
	public LENCKBHFKLD get_Type()
	{
		return LENCKBHFKLD.KeepAlive;
	}

	void IServerMessage.Parse(object data)
	{
	}
}
