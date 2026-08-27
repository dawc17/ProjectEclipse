public sealed class WebSocketContinuationFrame : WebSocketBinaryFrame
{
	public WebSocketContinuationFrame(byte[] data, bool JDHJLBBIKLM)
		: base(data, 0uL, (ulong)data.Length, JDHJLBBIKLM)
	{
	}

	public WebSocketContinuationFrame(byte[] data, ulong LCCLEFMKLPB, ulong BDBOAEGELMC, bool JDHJLBBIKLM)
		: base(data, LCCLEFMKLPB, BDBOAEGELMC, JDHJLBBIKLM)
	{
	}

	public override BECKAHJIEGE get_Type()
	{
		return BECKAHJIEGE.Continuation;
	}
}
