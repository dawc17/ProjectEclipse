using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public sealed class WebSocketFrameReader
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool OIGMDFDEPHD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BECKAHJIEGE KAHHEBMBCFA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ACMIAFGAMGN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong DCGIHLANEKJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] ONMKFKCCJGC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] JFKBADLJJBM;

	public bool EKPJBHAKGED
	{
		get
		{
			return MOOCLIBIPBI();
		}
		private set
		{
			set_IsFinal(value);
		}
	}

	public bool IILMMCDAABG
	{
		get
		{
			return FIDNGEELBPG();
		}
		private set
		{
			PCADCFCIJDF(value);
		}
	}

	public ulong IHGONCCOKMK
	{
		get
		{
			return KLIOMCPELLF();
		}
		private set
		{
			set_Length(value);
		}
	}

	public byte[] NMHABDNGDLJ
	{
		get
		{
			return JIIDHHHNCGL();
		}
		private set
		{
			PGOEMEOOJOH(value);
		}
	}

	public bool MOOCLIBIPBI()
	{
		return OIGMDFDEPHD;
	}

	private void set_IsFinal(bool value)
	{
		OIGMDFDEPHD = value;
	}

	public BECKAHJIEGE get_Type()
	{
		return KAHHEBMBCFA;
	}

	private void set_Type(BECKAHJIEGE value)
	{
		KAHHEBMBCFA = value;
	}

	public bool FIDNGEELBPG()
	{
		return ACMIAFGAMGN;
	}

	private void PCADCFCIJDF(bool value)
	{
		ACMIAFGAMGN = value;
	}

	public ulong KLIOMCPELLF()
	{
		return DCGIHLANEKJ;
	}

	private void set_Length(ulong value)
	{
		DCGIHLANEKJ = value;
	}

	public byte[] JIIDHHHNCGL()
	{
		return ONMKFKCCJGC;
	}

	private void PGOEMEOOJOH(byte[] value)
	{
		ONMKFKCCJGC = value;
	}

	public byte[] CHIGLEKCFFN()
	{
		return JFKBADLJJBM;
	}

	private void set_Data(byte[] value)
	{
		JFKBADLJJBM = value;
	}

	internal void Read(Stream ABJIEFMMIEK)
	{
		byte b = (byte)ABJIEFMMIEK.ReadByte();
		set_IsFinal((b & 0x80) != 0);
		set_Type((BECKAHJIEGE)(b & 0xF));
		b = (byte)ABJIEFMMIEK.ReadByte();
		PCADCFCIJDF((b & 0x80) != 0);
		set_Length((ulong)(b & 0x7F));
		if (KLIOMCPELLF() == 126)
		{
			byte[] array = new byte[2];
			ABJIEFMMIEK.ReadBuffer(array);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array, 0, array.Length);
			}
			set_Length(BitConverter.ToUInt16(array, 0));
		}
		else if (KLIOMCPELLF() == 127)
		{
			byte[] array2 = new byte[8];
			ABJIEFMMIEK.ReadBuffer(array2);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array2, 0, array2.Length);
			}
			set_Length(BitConverter.ToUInt64(array2, 0));
		}
		if (FIDNGEELBPG())
		{
			PGOEMEOOJOH(new byte[4]);
			ABJIEFMMIEK.Read(JIIDHHHNCGL(), 0, 4);
		}
		set_Data(new byte[KLIOMCPELLF()]);
		if (KLIOMCPELLF() == 0)
		{
			return;
		}
		int num = 0;
		do
		{
			num += ABJIEFMMIEK.Read(CHIGLEKCFFN(), num, CHIGLEKCFFN().Length - num);
		}
		while (num < CHIGLEKCFFN().Length);
		if (FIDNGEELBPG())
		{
			for (int i = 0; i < CHIGLEKCFFN().Length; i++)
			{
				CHIGLEKCFFN()[i] = (byte)(CHIGLEKCFFN()[i] ^ JIIDHHHNCGL()[i % 4]);
			}
		}
	}

	internal void Assemble(List<WebSocketFrameReader> DAGGODDBKDD)
	{
		DAGGODDBKDD.Add(this);
		ulong num = 0uL;
		for (int i = 0; i < DAGGODDBKDD.Count; i++)
		{
			num += DAGGODDBKDD[i].KLIOMCPELLF();
		}
		byte[] array = new byte[num];
		ulong num2 = 0uL;
		for (int j = 0; j < DAGGODDBKDD.Count; j++)
		{
			Array.Copy(DAGGODDBKDD[j].CHIGLEKCFFN(), 0, array, (int)num2, (int)DAGGODDBKDD[j].KLIOMCPELLF());
			num2 += DAGGODDBKDD[j].KLIOMCPELLF();
		}
		set_Type(DAGGODDBKDD[0].get_Type());
		set_Length(num);
		set_Data(array);
	}
}
