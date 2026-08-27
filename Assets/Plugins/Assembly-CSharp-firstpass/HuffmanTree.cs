using Unity.IO.Compression;

internal class HuffmanTree
{
	internal const int ODNHPDMFBCN = 288;

	internal const int LMHLPPPFFAM = 32;

	internal const int JNAMHNNNOFD = 256;

	internal const int IPJMAIGFOFF = 19;

	private int BDCGEABNBNP;

	private short[] BFGHBIMJHAK;

	private short[] MKICABFAHFA;

	private short[] JMLKHIPBCLI;

	private byte[] codeLengthArray;

	private int NOHHGLPGCGE;

	private static HuffmanTree BBGLCFHEAFJ;

	private static HuffmanTree IHGOLENKGBO;

	public static HuffmanTree CDHAJEBPIGP
	{
		get
		{
			return CMJKCGMHABI();
		}
	}

	public static HuffmanTree KCGLLGFCHEO
	{
		get
		{
			return CECHGKLBAAN();
		}
	}

	static HuffmanTree()
	{
		BBGLCFHEAFJ = new HuffmanTree(EONIMMMPOLE());
		IHGOLENKGBO = new HuffmanTree(CPOEOOCGFFJ());
	}

	public HuffmanTree(byte[] BOKCDKCDIOA)
	{
		codeLengthArray = BOKCDKCDIOA;
		if (codeLengthArray.Length == 288)
		{
			BDCGEABNBNP = 9;
		}
		else
		{
			BDCGEABNBNP = 7;
		}
		NOHHGLPGCGE = (1 << BDCGEABNBNP) - 1;
		JDFKMJMOEOF();
	}

	public static HuffmanTree CMJKCGMHABI()
	{
		return BBGLCFHEAFJ;
	}

	public static HuffmanTree CECHGKLBAAN()
	{
		return IHGOLENKGBO;
	}

	private static byte[] EONIMMMPOLE()
	{
		byte[] array = new byte[288];
		for (int i = 0; i <= 143; i++)
		{
			array[i] = 8;
		}
		for (int j = 144; j <= 255; j++)
		{
			array[j] = 9;
		}
		for (int k = 256; k <= 279; k++)
		{
			array[k] = 7;
		}
		for (int l = 280; l <= 287; l++)
		{
			array[l] = 8;
		}
		return array;
	}

	private static byte[] CPOEOOCGFFJ()
	{
		byte[] array = new byte[32];
		for (int i = 0; i < 32; i++)
		{
			array[i] = 5;
		}
		return array;
	}

	private uint[] FLDOBAHJHKK()
	{
		uint[] array = new uint[17];
		byte[] bIODNNBNOFC = codeLengthArray;
		foreach (int num in bIODNNBNOFC)
		{
			array[num]++;
		}
		array[0] = 0u;
		uint[] array2 = new uint[17];
		uint num2 = 0u;
		for (int j = 1; j <= 16; j++)
		{
			num2 = (array2[j] = num2 + array[j - 1] << 1);
		}
		uint[] array3 = new uint[288];
		for (int k = 0; k < codeLengthArray.Length; k++)
		{
			int num3 = codeLengthArray[k];
			if (num3 > 0)
			{
				array3[k] = FastEncoderStatics.MEFBBOOOOII(array2[num3], num3);
				array2[num3]++;
			}
		}
		return array3;
	}

	private void JDFKMJMOEOF()
	{
		uint[] array = FLDOBAHJHKK();
		BFGHBIMJHAK = new short[1 << BDCGEABNBNP];
		MKICABFAHFA = new short[2 * codeLengthArray.Length];
		JMLKHIPBCLI = new short[2 * codeLengthArray.Length];
		short num = (short)codeLengthArray.Length;
		for (int i = 0; i < codeLengthArray.Length; i++)
		{
			int num2 = codeLengthArray[i];
			if (num2 <= 0)
			{
				continue;
			}
			int num3 = (int)array[i];
			if (num2 <= BDCGEABNBNP)
			{
				int num4 = 1 << num2;
				if (num3 >= num4)
				{
					throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
				}
				int num5 = 1 << BDCGEABNBNP - num2;
				for (int j = 0; j < num5; j++)
				{
					BFGHBIMJHAK[num3] = (short)i;
					num3 += num4;
				}
				continue;
			}
			int num6 = num2 - BDCGEABNBNP;
			int num7 = 1 << BDCGEABNBNP;
			int num8 = num3 & ((1 << BDCGEABNBNP) - 1);
			short[] array2 = BFGHBIMJHAK;
			do
			{
				short num9 = array2[num8];
				if (num9 == 0)
				{
					array2[num8] = (short)(-num);
					num9 = (short)(-num);
					num++;
				}
				if (num9 > 0)
				{
					throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
				}
				array2 = (((num3 & num7) != 0) ? JMLKHIPBCLI : MKICABFAHFA);
				num8 = -num9;
				num7 <<= 1;
				num6--;
			}
			while (num6 != 0);
			array2[num8] = (short)i;
		}
	}

	public int NBKGIKBOJGM(InputBuffer NILNDHEKNLJ)
	{
		uint num = NILNDHEKNLJ.DDGBLEAPMLA();
		if (NILNDHEKNLJ.PEKEJGLMKPH() == 0)
		{
			return -1;
		}
		int num2 = BFGHBIMJHAK[num & NOHHGLPGCGE];
		if (num2 < 0)
		{
			uint num3 = (uint)(1 << BDCGEABNBNP);
			do
			{
				num2 = -num2;
				num2 = (((num & num3) != 0) ? JMLKHIPBCLI[num2] : MKICABFAHFA[num2]);
				num3 <<= 1;
			}
			while (num2 < 0);
		}
		int num4 = codeLengthArray[num2];
		if (num4 <= 0)
		{
			throw new InvalidDataException(SR.GetString("Invalid Huffman data"));
		}
		if (num4 > NILNDHEKNLJ.PEKEJGLMKPH())
		{
			return -1;
		}
		NILNDHEKNLJ.SkipBits(num4);
		return num2;
	}
}
