using System;

internal class InputBuffer
{
	private byte[] buffer;

	private int ILENLCMAMBH;

	private int PCLFFOBJJFO;

	private uint bitBuffer;

	private int FJPIAFJLLBI;

	public int GMAEJHLBDPL
	{
		get
		{
			return PEKEJGLMKPH();
		}
	}

	public int EDCELODAANL
	{
		get
		{
			return EJAHIMFDFJI();
		}
	}

	public int PEKEJGLMKPH()
	{
		return FJPIAFJLLBI;
	}

	public int EJAHIMFDFJI()
	{
		return PCLFFOBJJFO - ILENLCMAMBH + FJPIAFJLLBI / 8;
	}

	public bool EnsureBitsAvailable(int count)
	{
		if (FJPIAFJLLBI < count)
		{
			if (NeedsInput())
			{
				return false;
			}
			bitBuffer |= (uint)(buffer[ILENLCMAMBH++] << FJPIAFJLLBI);
			FJPIAFJLLBI += 8;
			if (FJPIAFJLLBI < count)
			{
				if (NeedsInput())
				{
					return false;
				}
				bitBuffer |= (uint)(buffer[ILENLCMAMBH++] << FJPIAFJLLBI);
				FJPIAFJLLBI += 8;
			}
		}
		return true;
	}

	public uint DDGBLEAPMLA()
	{
		if (FJPIAFJLLBI < 8)
		{
			if (ILENLCMAMBH < PCLFFOBJJFO)
			{
				bitBuffer |= (uint)(buffer[ILENLCMAMBH++] << FJPIAFJLLBI);
				FJPIAFJLLBI += 8;
			}
			if (ILENLCMAMBH < PCLFFOBJJFO)
			{
				bitBuffer |= (uint)(buffer[ILENLCMAMBH++] << FJPIAFJLLBI);
				FJPIAFJLLBI += 8;
			}
		}
		else if (FJPIAFJLLBI < 16 && ILENLCMAMBH < PCLFFOBJJFO)
		{
			bitBuffer |= (uint)(buffer[ILENLCMAMBH++] << FJPIAFJLLBI);
			FJPIAFJLLBI += 8;
		}
		return bitBuffer;
	}

	private uint GetBitMask(int count)
	{
		return (uint)((1 << count) - 1);
	}

	public int GetBits(int count)
	{
		if (!EnsureBitsAvailable(count))
		{
			return -1;
		}
		int result = (int)(bitBuffer & GetBitMask(count));
		bitBuffer >>= count;
		FJPIAFJLLBI -= count;
		return result;
	}

	public int CopyTo(byte[] output, int IPCOBJBKNAO, int BDBOAEGELMC)
	{
		int num = 0;
		while (FJPIAFJLLBI > 0 && BDBOAEGELMC > 0)
		{
			output[IPCOBJBKNAO++] = (byte)bitBuffer;
			bitBuffer >>= 8;
			FJPIAFJLLBI -= 8;
			BDBOAEGELMC--;
			num++;
		}
		if (BDBOAEGELMC == 0)
		{
			return num;
		}
		int num2 = PCLFFOBJJFO - ILENLCMAMBH;
		if (BDBOAEGELMC > num2)
		{
			BDBOAEGELMC = num2;
		}
		Array.Copy(buffer, ILENLCMAMBH, output, IPCOBJBKNAO, BDBOAEGELMC);
		ILENLCMAMBH += BDBOAEGELMC;
		return num + BDBOAEGELMC;
	}

	public bool NeedsInput()
	{
		return ILENLCMAMBH == PCLFFOBJJFO;
	}

	public void SetInput(byte[] buffer, int IPCOBJBKNAO, int BDBOAEGELMC)
	{
		this.buffer = buffer;
		ILENLCMAMBH = IPCOBJBKNAO;
		PCLFFOBJJFO = IPCOBJBKNAO + BDBOAEGELMC;
	}

	public void SkipBits(int HDKKKCDKFEE)
	{
		bitBuffer >>= HDKKKCDKFEE;
		FJPIAFJLLBI -= HDKKKCDKFEE;
	}

	public void KHMFPEJHFHC()
	{
		bitBuffer >>= FJPIAFJLLBI % 8;
		FJPIAFJLLBI -= FJPIAFJLLBI % 8;
	}
}
