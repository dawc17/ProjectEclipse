using System;
using System.IO;

internal class CRC32
{
	private uint PHOKLKLMLFA;

	private long _TotalBytesRead;

	private bool reverseBits;

	private uint[] crc32Table;

	private const int BUFFER_SIZE = 8192;

	private uint CIACMAKFIAP = uint.MaxValue;

	public long DDBILOJCBFN
	{
		get
		{
			return BFADCOPLBPM();
		}
	}

	public int FAJNNCGFPPF
	{
		get
		{
			return MMBAMEEDDFA();
		}
	}

	public CRC32()
		: this(false)
	{
	}

	public CRC32(bool reverseBits)
		: this(-306674912, reverseBits)
	{
	}

	public CRC32(int OEFFIELGAEI, bool reverseBits)
	{
		this.reverseBits = reverseBits;
		PHOKLKLMLFA = (uint)OEFFIELGAEI;
		ILBPHHDOGAI();
	}

	public long BFADCOPLBPM()
	{
		return _TotalBytesRead;
	}

	public int MMBAMEEDDFA()
	{
		return (int)(~CIACMAKFIAP);
	}

	public int GetCrc32(Stream NILNDHEKNLJ)
	{
		return GetCrc32AndCopy(NILNDHEKNLJ, null);
	}

	public int GetCrc32AndCopy(Stream NILNDHEKNLJ, Stream output)
	{
		if (NILNDHEKNLJ == null)
		{
			throw new Exception("The input stream must not be null.");
		}
		byte[] array = new byte[8192];
		int count = 8192;
		_TotalBytesRead = 0L;
		int num = NILNDHEKNLJ.Read(array, 0, count);
		if (output != null)
		{
			output.Write(array, 0, num);
		}
		_TotalBytesRead += num;
		while (num > 0)
		{
			LOAACENMBJJ(array, 0, num);
			num = NILNDHEKNLJ.Read(array, 0, count);
			if (output != null)
			{
				output.Write(array, 0, num);
			}
			_TotalBytesRead += num;
		}
		return (int)(~CIACMAKFIAP);
	}

	public int ComputeCrc32(int BLFBMIOIPOI, byte LDKCOIHONPG)
	{
		return _InternalComputeCrc32((uint)BLFBMIOIPOI, LDKCOIHONPG);
	}

	internal int _InternalComputeCrc32(uint BLFBMIOIPOI, byte LDKCOIHONPG)
	{
		return (int)(crc32Table[(BLFBMIOIPOI ^ LDKCOIHONPG) & 0xFF] ^ (BLFBMIOIPOI >> 8));
	}

	public void LOAACENMBJJ(byte[] JILGHDDEMPE, int IPCOBJBKNAO, int count)
	{
		if (JILGHDDEMPE == null)
		{
			throw new Exception("The data buffer must not be null.");
		}
		for (int i = 0; i < count; i++)
		{
			int num = IPCOBJBKNAO + i;
			byte b = JILGHDDEMPE[num];
			if (reverseBits)
			{
				uint num2 = (CIACMAKFIAP >> 24) ^ b;
				CIACMAKFIAP = (CIACMAKFIAP << 8) ^ crc32Table[num2];
			}
			else
			{
				uint num3 = (CIACMAKFIAP & 0xFF) ^ b;
				CIACMAKFIAP = (CIACMAKFIAP >> 8) ^ crc32Table[num3];
			}
		}
		_TotalBytesRead += count;
	}

	public void UpdateCRC(byte AAOIAEJJINO)
	{
		if (reverseBits)
		{
			uint num = (CIACMAKFIAP >> 24) ^ AAOIAEJJINO;
			CIACMAKFIAP = (CIACMAKFIAP << 8) ^ crc32Table[num];
		}
		else
		{
			uint num2 = (CIACMAKFIAP & 0xFF) ^ AAOIAEJJINO;
			CIACMAKFIAP = (CIACMAKFIAP >> 8) ^ crc32Table[num2];
		}
	}

	public void UpdateCRC(byte AAOIAEJJINO, int HDKKKCDKFEE)
	{
		while (HDKKKCDKFEE-- > 0)
		{
			if (reverseBits)
			{
				uint num = (CIACMAKFIAP >> 24) ^ AAOIAEJJINO;
				CIACMAKFIAP = (CIACMAKFIAP << 8) ^ crc32Table[(num < 0) ? (num + 256) : num];
			}
			else
			{
				uint num2 = (CIACMAKFIAP & 0xFF) ^ AAOIAEJJINO;
				CIACMAKFIAP = (CIACMAKFIAP >> 8) ^ crc32Table[(num2 < 0) ? (num2 + 256) : num2];
			}
		}
	}

	private static uint ReverseBits(uint data)
	{
		uint num = data;
		num = ((num & 0x55555555) << 1) | ((num >> 1) & 0x55555555);
		num = ((num & 0x33333333) << 2) | ((num >> 2) & 0x33333333);
		num = ((num & 0xF0F0F0F) << 4) | ((num >> 4) & 0xF0F0F0F);
		return (num << 24) | ((num & 0xFF00) << 8) | ((num >> 8) & 0xFF00) | (num >> 24);
	}

	private static byte ReverseBits(byte data)
	{
		uint num = (uint)(data * 131586);
		uint num2 = 17055760u;
		uint num3 = num & num2;
		uint num4 = (num << 2) & (num2 << 1);
		return (byte)(16781313 * (num3 + num4) >> 24);
	}

	private void ILBPHHDOGAI()
	{
		crc32Table = new uint[256];
		byte b = 0;
		do
		{
			uint num = b;
			for (byte b2 = 8; b2 > 0; b2--)
			{
				num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ PHOKLKLMLFA));
			}
			if (reverseBits)
			{
				crc32Table[ReverseBits(b)] = ReverseBits(num);
			}
			else
			{
				crc32Table[b] = num;
			}
			b++;
		}
		while (b != 0);
	}

	private uint gf2_matrix_times(uint[] NHBBGODHBEF, uint HCMPBOCKJOP)
	{
		uint num = 0u;
		int num2 = 0;
		while (HCMPBOCKJOP != 0)
		{
			if ((HCMPBOCKJOP & 1) == 1)
			{
				num ^= NHBBGODHBEF[num2];
			}
			HCMPBOCKJOP >>= 1;
			num2++;
		}
		return num;
	}

	private void gf2_matrix_square(uint[] AACJHHFILGC, uint[] BLBBHHDOBEB)
	{
		for (int i = 0; i < 32; i++)
		{
			AACJHHFILGC[i] = gf2_matrix_times(BLBBHHDOBEB, BLBBHHDOBEB[i]);
		}
	}

	public void Combine(int GAICMJOFOJD, int BDBOAEGELMC)
	{
		uint[] array = new uint[32];
		uint[] array2 = new uint[32];
		if (BDBOAEGELMC == 0)
		{
			return;
		}
		uint num = ~CIACMAKFIAP;
		array2[0] = PHOKLKLMLFA;
		uint num2 = 1u;
		for (int i = 1; i < 32; i++)
		{
			array2[i] = num2;
			num2 <<= 1;
		}
		gf2_matrix_square(array, array2);
		gf2_matrix_square(array2, array);
		uint num3 = (uint)BDBOAEGELMC;
		do
		{
			gf2_matrix_square(array, array2);
			if ((num3 & 1) == 1)
			{
				num = gf2_matrix_times(array, num);
			}
			num3 >>= 1;
			if (num3 == 0)
			{
				break;
			}
			gf2_matrix_square(array2, array);
			if ((num3 & 1) == 1)
			{
				num = gf2_matrix_times(array2, num);
			}
			num3 >>= 1;
		}
		while (num3 != 0);
		num ^= (uint)GAICMJOFOJD;
		CIACMAKFIAP = ~num;
	}

	public void Reset()
	{
		CIACMAKFIAP = uint.MaxValue;
	}
}
