using System;
using System.Collections.Generic;
using System.Text;

public class NekkiRandom
{
	private RandomGenerator KIDGAELKGEI;

	public NekkiRandom(uint OKGKLCLEDFN)
	{
		KIDGAELKGEI = new RandomGenerator(OKGKLCLEDFN);
	}

	public NekkiRandom()
	{
		KIDGAELKGEI = new RandomGenerator(0u);
		uint oKGKLCLEDFN = (uint)DateTime.UtcNow.Ticks;
		KIDGAELKGEI.setSeed(oKGKLCLEDFN);
	}

	public uint OHBDPMGHNFM()
	{
		return RandomGenerator.HHJPLLHOGCH();
	}

	public uint DADGADIAJHI()
	{
		return KIDGAELKGEI.DADGADIAJHI();
	}

	public void setSeed(uint OKGKLCLEDFN)
	{
		KIDGAELKGEI.setSeed(OKGKLCLEDFN);
	}

	public uint EBKAFDGPLOE()
	{
		return KIDGAELKGEI.EBKAFDGPLOE();
	}

	public float randomFloat()
	{
		return (float)DADGADIAJHI() / (float)OHBDPMGHNFM() + (float)DADGADIAJHI() / (float)OHBDPMGHNFM() / (float)OHBDPMGHNFM();
	}

	public float randomFloat(float KAEPJHHLLPK)
	{
		return randomFloat() * KAEPJHHLLPK;
	}

	public float randomFloat(float LHNCHOAEGEA, float KAEPJHHLLPK)
	{
		return LHNCHOAEGEA + randomFloat(KAEPJHHLLPK - LHNCHOAEGEA);
	}

	public uint randomInt(uint KAEPJHHLLPK)
	{
		return (uint)((float)KAEPJHHLLPK * randomFloat());
	}

	public uint randomInt(uint LHNCHOAEGEA, uint KAEPJHHLLPK)
	{
		return LHNCHOAEGEA + randomInt(KAEPJHHLLPK - LHNCHOAEGEA);
	}

	public bool randomChance(float AMBMJABLPFE, float BCCEJBCHNHC = 100f)
	{
		return randomFloat(BCCEJBCHNHC) < AMBMJABLPFE;
	}

	public string randomString(int AEPODKHKPDF, string GBLKDKHFKFL = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
	{
		uint length = (uint)GBLKDKHFKFL.Length;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < AEPODKHKPDF; i++)
		{
			stringBuilder.Append(GBLKDKHFKFL[(int)randomInt(length)]);
		}
		return stringBuilder.ToString();
	}

	public void ShuffleList<T>(List<T> OMKIGJOLJJE)
	{
		int num = OMKIGJOLJJE.Count;
		while (num > 1)
		{
			num--;
			int index = (int)randomInt(0u, (uint)num);
			T value = OMKIGJOLJJE[index];
			OMKIGJOLJJE[index] = OMKIGJOLJJE[num];
			OMKIGJOLJJE[num] = value;
		}
	}

	public void ShuffleArray<T>(T[] AALGCAPHOED)
	{
		int num = AALGCAPHOED.Length;
		while (num > 1)
		{
			num--;
			int num2 = (int)randomInt(0u, (uint)num);
			T val = AALGCAPHOED[num2];
			AALGCAPHOED[num2] = AALGCAPHOED[num];
			AALGCAPHOED[num] = val;
		}
	}
}
