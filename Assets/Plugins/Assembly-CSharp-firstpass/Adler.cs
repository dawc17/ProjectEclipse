public sealed class Adler
{
	private static readonly uint BASE = 65521u;

	private static readonly int NMAX = 5552;

	public static uint IAJPFDALGJM(uint HMGBNHKFPAG, byte[] HLDLIFPJMOA, int index, int JCAJDBOMGOM)
	{
		if (HLDLIFPJMOA == null)
		{
			return 1u;
		}
		uint num = HMGBNHKFPAG & 0xFFFF;
		uint num2 = (HMGBNHKFPAG >> 16) & 0xFFFF;
		while (JCAJDBOMGOM > 0)
		{
			int num3 = ((JCAJDBOMGOM >= NMAX) ? NMAX : JCAJDBOMGOM);
			JCAJDBOMGOM -= num3;
			while (num3 >= 16)
			{
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num += HLDLIFPJMOA[index++];
				num2 += num;
				num3 -= 16;
			}
			if (num3 != 0)
			{
				do
				{
					num += HLDLIFPJMOA[index++];
					num2 += num;
				}
				while (--num3 != 0);
			}
			num %= BASE;
			num2 %= BASE;
		}
		return (num2 << 16) | num;
	}
}
