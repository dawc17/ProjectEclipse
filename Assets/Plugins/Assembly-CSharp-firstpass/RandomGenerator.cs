public class RandomGenerator
{
	private const uint M = 2147483647u;

	private uint CALKMDJCFDK;

	private uint _seed;

	public RandomGenerator(uint OKGKLCLEDFN)
	{
		setSeed(OKGKLCLEDFN);
	}

	public uint DADGADIAJHI()
	{
		CALKMDJCFDK = randLCG(CALKMDJCFDK);
		return CALKMDJCFDK;
	}

	public uint EBKAFDGPLOE()
	{
		return _seed;
	}

	public void setSeed(uint OKGKLCLEDFN)
	{
		_seed = OKGKLCLEDFN;
		CALKMDJCFDK = OKGKLCLEDFN;
	}

	public uint randLCG(uint DGNDGHPMPJD)
	{
		DGNDGHPMPJD = (DGNDGHPMPJD * 1103515245 + 12345) & 0x7FFFFFFF;
		return DGNDGHPMPJD;
	}

	public static uint HHJPLLHOGCH()
	{
		return 2147483648u;
	}
}
