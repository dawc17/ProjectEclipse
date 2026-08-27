using System.Text;

public sealed class AEGCCCNBCML : WebSocketBinaryFrame
{
	public AEGCCCNBCML(string HCPNFPMHFCM)
		: base(Encoding.UTF8.GetBytes(HCPNFPMHFCM))
	{
	}

	public override BECKAHJIEGE get_Type()
	{
		return BECKAHJIEGE.Text;
	}
}
