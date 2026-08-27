using System;
using System.Diagnostics;

internal class FastEncoderWindow
{
	private byte[] window;

	private int IFFCIPOGHIH;

	private int OJKIELNPBFG;

	private const int OOKHBDFDFHK = 4;

	private const int EDHEMHHBHAA = 2048;

	private const int FJKIHECMFKN = 2047;

	private const int OGJIOCAFEIA = 8192;

	private const int DFHLCKGICBF = 8191;

	private const int HOFNAEHKFBE = 16384;

	internal const int CGBGDMNJAIK = 258;

	internal const int AOEFLGEAPFP = 3;

	private const int NJOICKKBADL = 32;

	private const int MLJCJEAACIO = 4;

	private const int NNLJOHLDEOE = 32;

	private const int AKMEKFDBEFF = 6;

	private ushort[] JIABHEAAKCH;

	private ushort[] CPOIHHKONLA;

	public int EOBMLHGNPFL
	{
		get
		{
			return LIPBPKCMELJ();
		}
	}

	public DeflateInput NFFBHLDODLH
	{
		get
		{
			return EGHDOBABAFB();
		}
	}

	public int OFPAAKEHIML
	{
		get
		{
			return EHGMBKDHEGD();
		}
	}

	public FastEncoderWindow()
	{
		HLDNGOMPMAH();
	}

	public int LIPBPKCMELJ()
	{
		return OJKIELNPBFG - IFFCIPOGHIH;
	}

	public DeflateInput EGHDOBABAFB()
	{
		DeflateInput pGEGNLJIJFE = new DeflateInput();
		pGEGNLJIJFE.set_Buffer(window);
		pGEGNLJIJFE.MOFAGMEDPNM(IFFCIPOGHIH);
		pGEGNLJIJFE.CHILOKHFALD(OJKIELNPBFG - IFFCIPOGHIH);
		return pGEGNLJIJFE;
	}

	public void KDIBKEKLFEL()
	{
		HLDNGOMPMAH();
	}

	private void HLDNGOMPMAH()
	{
		window = new byte[16646];
		JIABHEAAKCH = new ushort[8450];
		CPOIHHKONLA = new ushort[2048];
		IFFCIPOGHIH = 8192;
		OJKIELNPBFG = IFFCIPOGHIH;
	}

	public int EHGMBKDHEGD()
	{
		return 16384 - OJKIELNPBFG;
	}

	public void JGEOAPANNLP(byte[] MMFIPPNMIKJ, int CAILGDNIKJD, int count)
	{
		Array.Copy(MMFIPPNMIKJ, CAILGDNIKJD, window, OJKIELNPBFG, count);
		OJKIELNPBFG += count;
	}

	public void PEKGDBIPCOA()
	{
		Array.Copy(window, IFFCIPOGHIH - 8192, window, 0, 8192);
		for (int i = 0; i < 2048; i++)
		{
			int num = CPOIHHKONLA[i] - 8192;
			if (num <= 0)
			{
				CPOIHHKONLA[i] = 0;
			}
			else
			{
				CPOIHHKONLA[i] = (ushort)num;
			}
		}
		for (int i = 0; i < 8192; i++)
		{
			long num2 = (long)(int)JIABHEAAKCH[i] - 8192L;
			if (num2 <= 0)
			{
				JIABHEAAKCH[i] = 0;
			}
			else
			{
				JIABHEAAKCH[i] = (ushort)num2;
			}
		}
		IFFCIPOGHIH = 8192;
		OJKIELNPBFG = IFFCIPOGHIH;
	}

	private uint HashValue(uint HDPBNCNCMOH, byte AAOIAEJJINO)
	{
		return (HDPBNCNCMOH << 4) ^ AAOIAEJJINO;
	}

	private uint InsertString(ref uint HDPBNCNCMOH)
	{
		HDPBNCNCMOH = HashValue(HDPBNCNCMOH, window[IFFCIPOGHIH + 2]);
		uint num = CPOIHHKONLA[HDPBNCNCMOH & 0x7FF];
		CPOIHHKONLA[HDPBNCNCMOH & 0x7FF] = (ushort)IFFCIPOGHIH;
		JIABHEAAKCH[IFFCIPOGHIH & 0x1FFF] = (ushort)num;
		return num;
	}

	private void InsertStrings(ref uint HDPBNCNCMOH, int EEPFDKNNGJB)
	{
		if (OJKIELNPBFG - IFFCIPOGHIH <= EEPFDKNNGJB)
		{
			IFFCIPOGHIH += EEPFDKNNGJB - 1;
			return;
		}
		while (--EEPFDKNNGJB > 0)
		{
			InsertString(ref HDPBNCNCMOH);
			IFFCIPOGHIH++;
		}
	}

	internal bool MJLPAFBLONC(Match MLPEJKLNAKF)
	{
		uint hDPBNCNCMOH = HashValue(0u, window[IFFCIPOGHIH]);
		hDPBNCNCMOH = HashValue(hDPBNCNCMOH, window[IFFCIPOGHIH + 1]);
		int MIAOKJENHOF = 0;
		int num;
		if (OJKIELNPBFG - IFFCIPOGHIH <= 3)
		{
			num = 0;
		}
		else
		{
			int num2 = (int)InsertString(ref hDPBNCNCMOH);
			if (num2 != 0)
			{
				num = PMCAPOBMDLC(num2, out MIAOKJENHOF, 32, 32);
				if (IFFCIPOGHIH + num > OJKIELNPBFG)
				{
					num = OJKIELNPBFG - IFFCIPOGHIH;
				}
			}
			else
			{
				num = 0;
			}
		}
		if (num < 3)
		{
			MLPEJKLNAKF.set_State(CDKCDPDMGDK.HasSymbol);
			MLPEJKLNAKF.set_Symbol(window[IFFCIPOGHIH]);
			IFFCIPOGHIH++;
		}
		else
		{
			IFFCIPOGHIH++;
			if (num <= 6)
			{
				int MIAOKJENHOF2 = 0;
				int num3 = (int)InsertString(ref hDPBNCNCMOH);
				int num4;
				if (num3 != 0)
				{
					num4 = PMCAPOBMDLC(num3, out MIAOKJENHOF2, (num >= 4) ? 8 : 32, 32);
					if (IFFCIPOGHIH + num4 > OJKIELNPBFG)
					{
						num4 = OJKIELNPBFG - IFFCIPOGHIH;
					}
				}
				else
				{
					num4 = 0;
				}
				if (num4 > num)
				{
					MLPEJKLNAKF.set_State(CDKCDPDMGDK.HasSymbolAndMatch);
					MLPEJKLNAKF.set_Symbol(window[IFFCIPOGHIH - 1]);
					MLPEJKLNAKF.set_Position(MIAOKJENHOF2);
					MLPEJKLNAKF.set_Length(num4);
					IFFCIPOGHIH++;
					num = num4;
					InsertStrings(ref hDPBNCNCMOH, num);
				}
				else
				{
					MLPEJKLNAKF.set_State(CDKCDPDMGDK.HasMatch);
					MLPEJKLNAKF.set_Position(MIAOKJENHOF);
					MLPEJKLNAKF.set_Length(num);
					num--;
					IFFCIPOGHIH++;
					InsertStrings(ref hDPBNCNCMOH, num);
				}
			}
			else
			{
				MLPEJKLNAKF.set_State(CDKCDPDMGDK.HasMatch);
				MLPEJKLNAKF.set_Position(MIAOKJENHOF);
				MLPEJKLNAKF.set_Length(num);
				InsertStrings(ref hDPBNCNCMOH, num);
			}
		}
		if (IFFCIPOGHIH == 16384)
		{
			PEKGDBIPCOA();
		}
		return true;
	}

	private int PMCAPOBMDLC(int AFMNFDABMGF, out int MIAOKJENHOF, int KJPBPEOKMBG, int AFEAHPCPHHI)
	{
		int num = 0;
		int num2 = 0;
		int num3 = IFFCIPOGHIH - 8192;
		byte b = window[IFFCIPOGHIH];
		while (AFMNFDABMGF > num3)
		{
			if (window[AFMNFDABMGF + num] == b)
			{
				int i;
				for (i = 0; i < 258 && window[IFFCIPOGHIH + i] == window[AFMNFDABMGF + i]; i++)
				{
				}
				if (i > num)
				{
					num = i;
					num2 = AFMNFDABMGF;
					if (i > 32)
					{
						break;
					}
					b = window[IFFCIPOGHIH + i];
				}
			}
			if (--KJPBPEOKMBG == 0)
			{
				break;
			}
			AFMNFDABMGF = JIABHEAAKCH[AFMNFDABMGF & 0x1FFF];
		}
		MIAOKJENHOF = IFFCIPOGHIH - num2 - 1;
		if (num == 3 && MIAOKJENHOF >= 16384)
		{
			return 0;
		}
		return num;
	}

	[Conditional("DEBUG")]
	private void EGKOLNMOELH()
	{
		for (int i = 0; i < 2048; i++)
		{
			ushort num = CPOIHHKONLA[i];
			while (num != 0 && IFFCIPOGHIH - num < 8192)
			{
				ushort num2 = JIABHEAAKCH[num & 0x1FFF];
				if (IFFCIPOGHIH - num2 >= 8192)
				{
					break;
				}
				num = num2;
			}
		}
	}

	private uint RecalculateHash(int MGMMDGFPBLP)
	{
		return (uint)(((window[MGMMDGFPBLP] << 8) ^ (window[MGMMDGFPBLP + 1] << 4) ^ window[MGMMDGFPBLP + 2]) & 0x7FF);
	}
}
