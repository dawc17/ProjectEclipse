using System;
using System.IO;
using System.Text;

public sealed class WebSocketClose : WebSocketBinaryFrame
{
	public WebSocketClose()
		: base(null)
	{
	}

	public WebSocketClose(ushort KJPGKHJNOMC, string LIOGIBJBHAH)
		: base(GDENFGNLFKL(KJPGKHJNOMC, LIOGIBJBHAH))
	{
	}

	public override BECKAHJIEGE get_Type()
	{
		return BECKAHJIEGE.ConnectionClose;
	}

	private static byte[] GDENFGNLFKL(ushort KJPGKHJNOMC, string LIOGIBJBHAH)
	{
		int byteCount = Encoding.UTF8.GetByteCount(LIOGIBJBHAH);
		using (MemoryStream memoryStream = new MemoryStream(2 + byteCount))
		{
			byte[] bytes = BitConverter.GetBytes(KJPGKHJNOMC);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes, 0, bytes.Length);
			}
			memoryStream.Write(bytes, 0, bytes.Length);
			bytes = Encoding.UTF8.GetBytes(LIOGIBJBHAH);
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream.ToArray();
		}
	}
}
