using System.Text;

public sealed class HKALCPMGELL : WebSocketBinaryFrame
{
	public HKALCPMGELL(string CKEHOEGLMBM)
		: base(Encoding.UTF8.GetBytes(CKEHOEGLMBM))
	{
	}

	public override BECKAHJIEGE get_Type()
	{
		return BECKAHJIEGE.Ping;
	}
}
