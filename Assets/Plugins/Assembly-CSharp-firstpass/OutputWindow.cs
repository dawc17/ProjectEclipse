using System;

internal class OutputWindow
{
	private const int FGIEBNCIJLK = 32768;

	private const int CCLGMCFNNNN = 32767;

	private byte[] window = new byte[32768];

	private int PCLFFOBJJFO;

	private int HLNCODHLPID;

	public int BMDIGKGNDPO
	{
		get
		{
			return JBPBBAEEAFO();
		}
	}

	public int EDCELODAANL
	{
		get
		{
			return EJAHIMFDFJI();
		}
	}

	public void Write(byte AAOIAEJJINO)
	{
		window[PCLFFOBJJFO++] = AAOIAEJJINO;
		PCLFFOBJJFO &= 32767;
		HLNCODHLPID++;
	}

	public void WriteLengthDistance(int BDBOAEGELMC, int OIOMNNFMDOO)
	{
		HLNCODHLPID += BDBOAEGELMC;
		int num = (PCLFFOBJJFO - OIOMNNFMDOO) & 0x7FFF;
		int num2 = 32768 - BDBOAEGELMC;
		if (num <= num2 && PCLFFOBJJFO < num2)
		{
			if (BDBOAEGELMC <= OIOMNNFMDOO)
			{
				Array.Copy(window, num, window, PCLFFOBJJFO, BDBOAEGELMC);
				PCLFFOBJJFO += BDBOAEGELMC;
			}
			else
			{
				while (BDBOAEGELMC-- > 0)
				{
					window[PCLFFOBJJFO++] = window[num++];
				}
			}
		}
		else
		{
			while (BDBOAEGELMC-- > 0)
			{
				window[PCLFFOBJJFO++] = window[num++];
				PCLFFOBJJFO &= 32767;
				num &= 0x7FFF;
			}
		}
	}

	public int CopyFrom(InputBuffer NILNDHEKNLJ, int BDBOAEGELMC)
	{
		BDBOAEGELMC = Math.Min(Math.Min(BDBOAEGELMC, 32768 - HLNCODHLPID), NILNDHEKNLJ.EJAHIMFDFJI());
		int num = 32768 - PCLFFOBJJFO;
		int num2;
		if (BDBOAEGELMC > num)
		{
			num2 = NILNDHEKNLJ.CopyTo(window, PCLFFOBJJFO, num);
			if (num2 == num)
			{
				num2 += NILNDHEKNLJ.CopyTo(window, 0, BDBOAEGELMC - num);
			}
		}
		else
		{
			num2 = NILNDHEKNLJ.CopyTo(window, PCLFFOBJJFO, BDBOAEGELMC);
		}
		PCLFFOBJJFO = (PCLFFOBJJFO + num2) & 0x7FFF;
		HLNCODHLPID += num2;
		return num2;
	}

	public int JBPBBAEEAFO()
	{
		return 32768 - HLNCODHLPID;
	}

	public int EJAHIMFDFJI()
	{
		return HLNCODHLPID;
	}

	public int CopyTo(byte[] output, int IPCOBJBKNAO, int BDBOAEGELMC)
	{
		int num;
		if (BDBOAEGELMC > HLNCODHLPID)
		{
			num = PCLFFOBJJFO;
			BDBOAEGELMC = HLNCODHLPID;
		}
		else
		{
			num = (PCLFFOBJJFO - HLNCODHLPID + BDBOAEGELMC) & 0x7FFF;
		}
		int num2 = BDBOAEGELMC;
		int num3 = BDBOAEGELMC - num;
		if (num3 > 0)
		{
			Array.Copy(window, 32768 - num3, output, IPCOBJBKNAO, num3);
			IPCOBJBKNAO += num3;
			BDBOAEGELMC = num;
		}
		Array.Copy(window, num - BDBOAEGELMC, output, IPCOBJBKNAO, BDBOAEGELMC);
		HLNCODHLPID -= num2;
		return num2;
	}
}
