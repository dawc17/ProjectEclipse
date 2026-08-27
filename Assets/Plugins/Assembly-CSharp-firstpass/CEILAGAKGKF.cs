using System.IO;

internal class CEILAGAKGKF
{
	public const uint kTopValue = 16777216u;

	public uint Range;

	public uint EDEEELJMHLG;

	public Stream Stream;

	public void Init(Stream ABJIEFMMIEK)
	{
		Stream = ABJIEFMMIEK;
		EDEEELJMHLG = 0u;
		Range = uint.MaxValue;
		for (int i = 0; i < 5; i++)
		{
			EDEEELJMHLG = (EDEEELJMHLG << 8) | (byte)Stream.ReadByte();
		}
	}

	public void IAIFCIAAHOE()
	{
		Stream = null;
	}

	public void GKKOGGMCJEC()
	{
		Stream.Close();
	}

	public void NBDMEIKNJBG()
	{
		while (Range < 16777216)
		{
			EDEEELJMHLG = (EDEEELJMHLG << 8) | (byte)Stream.ReadByte();
			Range <<= 8;
		}
	}

	public void OPODFGOCMPC()
	{
		if (Range < 16777216)
		{
			EDEEELJMHLG = (EDEEELJMHLG << 8) | (byte)Stream.ReadByte();
			Range <<= 8;
		}
	}

	public uint GetThreshold(uint ADLMOFDBBMG)
	{
		return EDEEELJMHLG / (Range /= ADLMOFDBBMG);
	}

	public void Decode(uint ILENLCMAMBH, uint PEEOEOMEBFG, uint ADLMOFDBBMG)
	{
		EDEEELJMHLG -= ILENLCMAMBH * Range;
		Range *= PEEOEOMEBFG;
		NBDMEIKNJBG();
	}

	public uint DecodeDirectBits(int HEGEFMNECOF)
	{
		uint num = Range;
		uint num2 = EDEEELJMHLG;
		uint num3 = 0u;
		for (int num4 = HEGEFMNECOF; num4 > 0; num4--)
		{
			num >>= 1;
			uint num5 = num2 - num >> 31;
			num2 -= num & (num5 - 1);
			num3 = (num3 << 1) | (1 - num5);
			if (num < 16777216)
			{
				num2 = (num2 << 8) | (byte)Stream.ReadByte();
				num <<= 8;
			}
		}
		Range = num;
		EDEEELJMHLG = num2;
		return num3;
	}

	public uint AFDBLPPNGBC(uint DOONDFDPDFH, int HEGEFMNECOF)
	{
		uint num = (Range >> HEGEFMNECOF) * DOONDFDPDFH;
		uint result;
		if (EDEEELJMHLG < num)
		{
			result = 0u;
			Range = num;
		}
		else
		{
			result = 1u;
			EDEEELJMHLG -= num;
			Range -= num;
		}
		NBDMEIKNJBG();
		return result;
	}
}
