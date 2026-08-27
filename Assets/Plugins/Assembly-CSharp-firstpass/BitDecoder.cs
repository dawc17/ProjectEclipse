internal struct BitDecoder
{
	public const int GMKPCANHECM = 11;

	public const uint kBitModelTotal = 2048u;

	private const int PEDEJBJFKOF = 5;

	private uint Prob;

	public void UpdateModel(int EGMENJEPBNH, uint symbol)
	{
		if (symbol == 0)
		{
			Prob += 2048 - Prob >> EGMENJEPBNH;
		}
		else
		{
			Prob -= Prob >> EGMENJEPBNH;
		}
	}

	public void Init()
	{
		Prob = 1024u;
	}

	public uint Decode(CEILAGAKGKF HELKEOGALEA)
	{
		uint num = (HELKEOGALEA.Range >> 11) * Prob;
		if (HELKEOGALEA.EDEEELJMHLG < num)
		{
			HELKEOGALEA.Range = num;
			Prob += 2048 - Prob >> 5;
			if (HELKEOGALEA.Range < 16777216)
			{
				HELKEOGALEA.EDEEELJMHLG = (HELKEOGALEA.EDEEELJMHLG << 8) | (byte)HELKEOGALEA.Stream.ReadByte();
				HELKEOGALEA.Range <<= 8;
			}
			return 0u;
		}
		HELKEOGALEA.Range -= num;
		HELKEOGALEA.EDEEELJMHLG -= num;
		Prob -= Prob >> 5;
		if (HELKEOGALEA.Range < 16777216)
		{
			HELKEOGALEA.EDEEELJMHLG = (HELKEOGALEA.EDEEELJMHLG << 8) | (byte)HELKEOGALEA.Stream.ReadByte();
			HELKEOGALEA.Range <<= 8;
		}
		return 1u;
	}
}
