using System;
using System.Diagnostics;
using System.IO;

public class WebSocketBinaryFrame : IWebSocketFrameWriter
{
	private static readonly byte[] NoData = new byte[0];

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool OIGMDFDEPHD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] JFKBADLJJBM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong JEAHFBGJDII;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong DCGIHLANEKJ;

	public bool EKPJBHAKGED
	{
		get
		{
			return MOOCLIBIPBI();
		}
		protected set
		{
			set_IsFinal(value);
		}
	}

	protected ulong KICOKBAGIPH
	{
		get
		{
			return FLMOEFEIFCE();
		}
		set
		{
			ONEFHFDLHBB(value);
		}
	}

	protected ulong IHGONCCOKMK
	{
		get
		{
			return KLIOMCPELLF();
		}
		set
		{
			set_Length(value);
		}
	}

	public WebSocketBinaryFrame(byte[] data)
		: this(data, 0uL, (ulong)((data == null) ? 0 : data.Length), true)
	{
	}

	public WebSocketBinaryFrame(byte[] data, bool JDHJLBBIKLM)
		: this(data, 0uL, (ulong)((data == null) ? 0 : data.Length), JDHJLBBIKLM)
	{
	}

	public WebSocketBinaryFrame(byte[] data, ulong LCCLEFMKLPB, ulong BDBOAEGELMC, bool JDHJLBBIKLM)
	{
		set_Data(data);
		ONEFHFDLHBB(LCCLEFMKLPB);
		set_Length(BDBOAEGELMC);
		set_IsFinal(JDHJLBBIKLM);
	}

	public virtual BECKAHJIEGE get_Type()
	{
		return BECKAHJIEGE.Binary;
	}

	public bool MOOCLIBIPBI()
	{
		return OIGMDFDEPHD;
	}

	protected void set_IsFinal(bool value)
	{
		OIGMDFDEPHD = value;
	}

	protected byte[] CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	protected void set_Data(byte[] value)
	{
		JFKBADLJJBM = value;
	}

	protected ulong FLMOEFEIFCE()
	{
		return JEAHFBGJDII;
	}

	protected void ONEFHFDLHBB(ulong value)
	{
		JEAHFBGJDII = value;
	}

	protected ulong KLIOMCPELLF()
	{
		return DCGIHLANEKJ;
	}

	protected void set_Length(ulong value)
	{
		DCGIHLANEKJ = value;
	}

	public virtual byte[] Get()
	{
		if (CHIGLEKCFFN() == null)
		{
			set_Data(NoData);
		}
		using (MemoryStream memoryStream = new MemoryStream((int)KLIOMCPELLF() + 9))
		{
			byte b = (byte)(MOOCLIBIPBI() ? 128u : 0u);
			memoryStream.WriteByte((byte)((uint)b | (uint)get_Type()));
			if (KLIOMCPELLF() < 126)
			{
				memoryStream.WriteByte((byte)(0x80 | (byte)KLIOMCPELLF()));
			}
			else if (KLIOMCPELLF() < 65535)
			{
				memoryStream.WriteByte(254);
				byte[] bytes = BitConverter.GetBytes((ushort)KLIOMCPELLF());
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes, 0, bytes.Length);
				}
				memoryStream.Write(bytes, 0, bytes.Length);
			}
			else
			{
				memoryStream.WriteByte(byte.MaxValue);
				byte[] bytes2 = BitConverter.GetBytes(KLIOMCPELLF());
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes2, 0, bytes2.Length);
				}
				memoryStream.Write(bytes2, 0, bytes2.Length);
			}
			byte[] bytes3 = BitConverter.GetBytes(GetHashCode());
			memoryStream.Write(bytes3, 0, bytes3.Length);
			for (ulong num = FLMOEFEIFCE(); num < FLMOEFEIFCE() + KLIOMCPELLF(); num++)
			{
				memoryStream.WriteByte((byte)(CHIGLEKCFFN()[num] ^ bytes3[(num - FLMOEFEIFCE()) % 4]));
			}
			return memoryStream.ToArray();
		}
	}
}
