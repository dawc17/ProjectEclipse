using System;

internal class FastEncoder
{
	private FastEncoderWindow FLDIPHNDIAP;

	private Match HPFOOBECGJN;

	private double lastCompressionRatio;

	internal int CLLDNIDJLPI
	{
		get
		{
			return EHHKMDHLKHJ();
		}
	}

	internal DeflateInput NFFBHLDODLH
	{
		get
		{
			return EGHDOBABAFB();
		}
	}

	internal double LPDJOBMKMBK
	{
		get
		{
			return LMLAIGGBPFL();
		}
	}

	public FastEncoder()
	{
		FLDIPHNDIAP = new FastEncoderWindow();
		HPFOOBECGJN = new Match();
	}

	internal int EHHKMDHLKHJ()
	{
		return FLDIPHNDIAP.LIPBPKCMELJ();
	}

	internal DeflateInput EGHDOBABAFB()
	{
		return FLDIPHNDIAP.EGHDOBABAFB();
	}

	internal void BMLBPABODCO()
	{
		FLDIPHNDIAP.KDIBKEKLFEL();
	}

	internal double LMLAIGGBPFL()
	{
		return lastCompressionRatio;
	}

	internal void GetBlock(DeflateInput NILNDHEKNLJ, OutputBuffer output, int OADBBDFBPOG)
	{
		JBFANIPBMGO(output);
		ADCHLOJOKMD(NILNDHEKNLJ, output, OADBBDFBPOG);
		DFBFDLOPMAB(output);
	}

	internal void IOKJILLFPDI(DeflateInput NILNDHEKNLJ, OutputBuffer output)
	{
		ADCHLOJOKMD(NILNDHEKNLJ, output, -1);
	}

	internal void ILPKALKNGIP(OutputBuffer output)
	{
		JBFANIPBMGO(output);
	}

	internal void ADMOOJIAFEI(OutputBuffer output)
	{
		DFBFDLOPMAB(output);
	}

	private void ADCHLOJOKMD(DeflateInput NILNDHEKNLJ, OutputBuffer output, int OADBBDFBPOG)
	{
		int num = output.GEBLFKFACKO();
		int num2 = 0;
		int num3 = EHHKMDHLKHJ() + NILNDHEKNLJ.OFOPFCJNEBL();
		do
		{
			int num4 = ((NILNDHEKNLJ.OFOPFCJNEBL() >= FLDIPHNDIAP.EHGMBKDHEGD()) ? FLDIPHNDIAP.EHGMBKDHEGD() : NILNDHEKNLJ.OFOPFCJNEBL());
			if (OADBBDFBPOG >= 1)
			{
				num4 = Math.Min(num4, OADBBDFBPOG - num2);
			}
			if (num4 > 0)
			{
				FLDIPHNDIAP.JGEOAPANNLP(NILNDHEKNLJ.FAJIIIFCCPD(), NILNDHEKNLJ.JHGJIJNGNBO(), num4);
				NILNDHEKNLJ.MBODOPCOFFE(num4);
				num2 += num4;
			}
			ADCHLOJOKMD(output);
		}
		while (AHEDMEEIAND(output) && LFFBLHANLLM(NILNDHEKNLJ) && (OADBBDFBPOG < 1 || num2 < OADBBDFBPOG));
		int num5 = output.GEBLFKFACKO();
		int num6 = num5 - num;
		int num7 = EHHKMDHLKHJ() + NILNDHEKNLJ.OFOPFCJNEBL();
		int num8 = num3 - num7;
		if (num6 != 0)
		{
			lastCompressionRatio = (double)num6 / (double)num8;
		}
	}

	private void ADCHLOJOKMD(OutputBuffer output)
	{
		while (FLDIPHNDIAP.LIPBPKCMELJ() > 0 && AHEDMEEIAND(output))
		{
			FLDIPHNDIAP.MJLPAFBLONC(HPFOOBECGJN);
			if (HPFOOBECGJN.FLBBFDNHJAJ() == CDKCDPDMGDK.HasSymbol)
			{
				DEDDLLDKGBO(HPFOOBECGJN.BCHAFDDNJHG(), output);
				continue;
			}
			if (HPFOOBECGJN.FLBBFDNHJAJ() == CDKCDPDMGDK.HasMatch)
			{
				IIPKHHPBOFA(HPFOOBECGJN.KLIOMCPELLF(), HPFOOBECGJN.ECJPLFFAMJO(), output);
				continue;
			}
			DEDDLLDKGBO(HPFOOBECGJN.BCHAFDDNJHG(), output);
			IIPKHHPBOFA(HPFOOBECGJN.KLIOMCPELLF(), HPFOOBECGJN.ECJPLFFAMJO(), output);
		}
	}

	private bool LFFBLHANLLM(DeflateInput NILNDHEKNLJ)
	{
		return NILNDHEKNLJ.OFOPFCJNEBL() > 0 || EHHKMDHLKHJ() > 0;
	}

	private bool AHEDMEEIAND(OutputBuffer output)
	{
		return output.JBPBBAEEAFO() > 16;
	}

	private void DFBFDLOPMAB(OutputBuffer output)
	{
		uint num = FastEncoderStatics.HEOFMEEEIKP[256];
		int hDKKKCDKFEE = (int)(num & 0x1F);
		output.EHFDJAJPOAO(hDKKKCDKFEE, num >> 5);
	}

	internal static void IIPKHHPBOFA(int EEPFDKNNGJB, int MIAOKJENHOF, OutputBuffer output)
	{
		uint num = FastEncoderStatics.HEOFMEEEIKP[254 + EEPFDKNNGJB];
		int num2 = (int)(num & 0x1F);
		if (num2 <= 16)
		{
			output.EHFDJAJPOAO(num2, num >> 5);
		}
		else
		{
			output.EHFDJAJPOAO(16, (num >> 5) & 0xFFFF);
			output.EHFDJAJPOAO(num2 - 16, num >> 21);
		}
		num = FastEncoderStatics.CIBAPLNOJJL[FastEncoderStatics.MFIEBGCGIDF(MIAOKJENHOF)];
		output.EHFDJAJPOAO((int)(num & 0xF), num >> 8);
		int num3 = (int)((num >> 4) & 0xF);
		if (num3 != 0)
		{
			output.EHFDJAJPOAO(num3, (uint)MIAOKJENHOF & FastEncoderStatics.EFKOOBOPIDF[num3]);
		}
	}

	internal static void DEDDLLDKGBO(byte AAOIAEJJINO, OutputBuffer output)
	{
		uint num = FastEncoderStatics.HEOFMEEEIKP[AAOIAEJJINO];
		output.EHFDJAJPOAO((int)(num & 0x1F), num >> 5);
	}

	internal static void JBFANIPBMGO(OutputBuffer output)
	{
		output.FJPANBOJJDI(FastEncoderStatics.KOEECIBJHJO, 0, FastEncoderStatics.KOEECIBJHJO.Length);
		output.EHFDJAJPOAO(9, 34u);
	}
}
