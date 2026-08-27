using System.IO;

internal class ABCAONADOMK
{
	public const uint kTopValue = 16777216u;

	private Stream Stream;

	public ulong Low;

	public uint Range;

	private uint LDIHHEJEIHL;

	private byte _cache;

	private long StartPosition;

	public void SetStream(Stream ABJIEFMMIEK)
	{
		Stream = ABJIEFMMIEK;
	}

	public void IAIFCIAAHOE()
	{
		Stream = null;
	}

	public void Init()
	{
		StartPosition = Stream.Position;
		Low = 0uL;
		Range = uint.MaxValue;
		LDIHHEJEIHL = 1u;
		_cache = 0;
	}

	public void DMHMONMENHH()
	{
		for (int i = 0; i < 5; i++)
		{
			GDJHIBPJMBN();
		}
	}

	public void PDFBMGAJEHM()
	{
		Stream.Flush();
	}

	public void GKKOGGMCJEC()
	{
		Stream.Close();
	}

	public void Encode(uint ILENLCMAMBH, uint PEEOEOMEBFG, uint ADLMOFDBBMG)
	{
		Low += ILENLCMAMBH * (Range /= ADLMOFDBBMG);
		Range *= PEEOEOMEBFG;
		while (Range < 16777216)
		{
			Range <<= 8;
			GDJHIBPJMBN();
		}
	}

	public void GDJHIBPJMBN()
	{
		if ((uint)Low < 4278190080u || (int)(Low >> 32) == 1)
		{
			byte b = _cache;
			do
			{
				Stream.WriteByte((byte)(b + (Low >> 32)));
				b = byte.MaxValue;
			}
			while (--LDIHHEJEIHL != 0);
			_cache = (byte)((uint)Low >> 24);
		}
		LDIHHEJEIHL++;
		Low = (uint)((int)Low << 8);
	}

	public void LJKOCPDCLLK(uint AFIEJABPAKA, int HEGEFMNECOF)
	{
		for (int num = HEGEFMNECOF - 1; num >= 0; num--)
		{
			Range >>= 1;
			if (((AFIEJABPAKA >> num) & 1) == 1)
			{
				Low += Range;
			}
			if (Range < 16777216)
			{
				Range <<= 8;
				GDJHIBPJMBN();
			}
		}
	}

	public void EncodeBit(uint DOONDFDPDFH, int HEGEFMNECOF, uint symbol)
	{
		uint num = (Range >> HEGEFMNECOF) * DOONDFDPDFH;
		if (symbol == 0)
		{
			Range = num;
		}
		else
		{
			Low += num;
			Range -= num;
		}
		while (Range < 16777216)
		{
			Range <<= 8;
			GDJHIBPJMBN();
		}
	}

	public long IAONGGCNFID()
	{
		return LDIHHEJEIHL + Stream.Position - StartPosition + 4;
	}
}
