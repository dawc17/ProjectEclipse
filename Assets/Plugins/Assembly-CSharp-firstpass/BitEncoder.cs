internal struct BitEncoder
{
	public const int GMKPCANHECM = 11;

	public const uint kBitModelTotal = 2048u;

	private const int PEDEJBJFKOF = 5;

	private const int MAGKCKLGMDD = 2;

	public const int CNMMKAGLEHI = 6;

	private uint Prob;

	private static uint[] ProbPrices;

	static BitEncoder()
	{
		ProbPrices = new uint[512];
		for (int num = 8; num >= 0; num--)
		{
			uint num2 = (uint)(1 << 9 - num - 1);
			uint num3 = (uint)(1 << 9 - num);
			for (uint num4 = num2; num4 < num3; num4++)
			{
				ProbPrices[num4] = (uint)(num << 6) + (num3 - num4 << 6 >> 9 - num - 1);
			}
		}
	}

	public void Init()
	{
		Prob = 1024u;
	}

	public void UpdateModel(uint symbol)
	{
		if (symbol == 0)
		{
			Prob += 2048 - Prob >> 5;
		}
		else
		{
			Prob -= Prob >> 5;
		}
	}

	public void Encode(ABCAONADOMK GLOJHMAIFOK, uint symbol)
	{
		uint num = (GLOJHMAIFOK.Range >> 11) * Prob;
		if (symbol == 0)
		{
			GLOJHMAIFOK.Range = num;
			Prob += 2048 - Prob >> 5;
		}
		else
		{
			GLOJHMAIFOK.Low += num;
			GLOJHMAIFOK.Range -= num;
			Prob -= Prob >> 5;
		}
		if (GLOJHMAIFOK.Range < 16777216)
		{
			GLOJHMAIFOK.Range <<= 8;
			GLOJHMAIFOK.GDJHIBPJMBN();
		}
	}

	public uint GetPrice(uint symbol)
	{
		return ProbPrices[(((Prob - symbol) ^ (int)(0 - symbol)) & 0x7FF) >> 2];
	}

	public uint ONPEFCGICJL()
	{
		return ProbPrices[Prob >> 2];
	}

	public uint AJMPGCNGLHF()
	{
		return ProbPrices[2048 - Prob >> 2];
	}
}
