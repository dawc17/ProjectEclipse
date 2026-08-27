using System;
using Unity.IO.Compression;

internal class Inflater
{
	private static readonly byte[] DEODDHFPBFE = new byte[29]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
		1, 1, 2, 2, 2, 2, 3, 3, 3, 3,
		4, 4, 4, 4, 5, 5, 5, 5, 0
	};

	private static readonly int[] NMNHMEJNLKB = new int[29]
	{
		3, 4, 5, 6, 7, 8, 9, 10, 11, 13,
		15, 17, 19, 23, 27, 31, 35, 43, 51, 59,
		67, 83, 99, 115, 131, 163, 195, 227, 258
	};

	private static readonly int[] KCGFDBBLOLL = new int[32]
	{
		1, 2, 3, 4, 5, 7, 9, 13, 17, 25,
		33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
		1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577,
		0, 0
	};

	private static readonly byte[] KCDJIAOANON = new byte[19]
	{
		16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
		11, 4, 12, 3, 13, 2, 14, 1, 15
	};

	private static readonly byte[] MIHBNJNICPA = new byte[32]
	{
		0, 16, 8, 24, 4, 20, 12, 28, 2, 18,
		10, 26, 6, 22, 14, 30, 1, 17, 9, 25,
		5, 21, 13, 29, 3, 19, 11, 27, 7, 23,
		15, 31
	};

	private OutputWindow output;

	private InputBuffer NILNDHEKNLJ;

	private HuffmanTree BADNHOBGEBO;

	private HuffmanTree IOBEEOPEKJJ;

	private BGMMMMHMIJF state;

	private bool hasFormatReader;

	private int PCLGPAHIBGH;

	private BPKEFFDGEIC AGJCDPOFIFE;

	private byte[] JEGPOHGJPIK = new byte[4];

	private int IHIGJLEBJBI;

	private int BDBOAEGELMC;

	private int GHCFEMCPBFA;

	private int extraBits;

	private int DOLKGPHFKPJ;

	private int GOIJLBOAAJO;

	private int NHDAMIACBBI;

	private int DIOGELJGGFC;

	private int AGBMNMICLFA;

	private int PCPIKCIKHLO;

	private byte[] OBEOJJGLMFG;

	private byte[] KKIAJONPLKI;

	private HuffmanTree PFCJKHONLFF;

	private IFileFormatReader IECPCDGIJGM;

	public int GBEPDJCMOPF
	{
		get
		{
			return MLAADGBFCOP();
		}
	}

	public Inflater()
	{
		output = new OutputWindow();
		NILNDHEKNLJ = new InputBuffer();
		OBEOJJGLMFG = new byte[320];
		KKIAJONPLKI = new byte[19];
		Reset();
	}

	internal void LDBNNMLIKOC(IFileFormatReader reader)
	{
		IECPCDGIJGM = reader;
		hasFormatReader = true;
		Reset();
	}

	private void Reset()
	{
		if (hasFormatReader)
		{
			state = BGMMMMHMIJF.ReadingHeader;
		}
		else
		{
			state = BGMMMMHMIJF.ReadingBFinal;
		}
	}

	public void SetInput(byte[] APACFLKJCKF, int IPCOBJBKNAO, int BDBOAEGELMC)
	{
		NILNDHEKNLJ.SetInput(APACFLKJCKF, IPCOBJBKNAO, BDBOAEGELMC);
	}

	public bool ALDLIOBKDFF()
	{
		return state == BGMMMMHMIJF.Done || state == BGMMMMHMIJF.VerifyingFooter;
	}

	public int MLAADGBFCOP()
	{
		return output.EJAHIMFDFJI();
	}

	public bool NeedsInput()
	{
		return NILNDHEKNLJ.NeedsInput();
	}

	public int Inflate(byte[] KPAMPCLHCEN, int IPCOBJBKNAO, int BDBOAEGELMC)
	{
		int num = 0;
		do
		{
			int num2 = output.CopyTo(KPAMPCLHCEN, IPCOBJBKNAO, BDBOAEGELMC);
			if (num2 > 0)
			{
				if (hasFormatReader)
				{
					IECPCDGIJGM.UpdateWithBytesRead(KPAMPCLHCEN, IPCOBJBKNAO, num2);
				}
				IPCOBJBKNAO += num2;
				num += num2;
				BDBOAEGELMC -= num2;
			}
		}
		while (BDBOAEGELMC != 0 && !ALDLIOBKDFF() && Decode());
		if (state == BGMMMMHMIJF.VerifyingFooter && output.EJAHIMFDFJI() == 0)
		{
			IECPCDGIJGM.FGCBJJKKILH();
		}
		return num;
	}

	private bool Decode()
	{
		bool DNCHJPDPNJK = false;
		bool flag = false;
		if (ALDLIOBKDFF())
		{
			return true;
		}
		if (hasFormatReader)
		{
			if (state == BGMMMMHMIJF.ReadingHeader)
			{
				if (!IECPCDGIJGM.DJJBPAJHJFI(NILNDHEKNLJ))
				{
					return false;
				}
				state = BGMMMMHMIJF.ReadingBFinal;
			}
			else if (state == BGMMMMHMIJF.StartReadingFooter || state == BGMMMMHMIJF.ReadingFooter)
			{
				if (!IECPCDGIJGM.BEPMEBNFAEL(NILNDHEKNLJ))
				{
					return false;
				}
				state = BGMMMMHMIJF.VerifyingFooter;
				return true;
			}
		}
		if (state == BGMMMMHMIJF.ReadingBFinal)
		{
			if (!NILNDHEKNLJ.EnsureBitsAvailable(1))
			{
				return false;
			}
			PCLGPAHIBGH = NILNDHEKNLJ.GetBits(1);
			state = BGMMMMHMIJF.ReadingBType;
		}
		if (state == BGMMMMHMIJF.ReadingBType)
		{
			if (!NILNDHEKNLJ.EnsureBitsAvailable(2))
			{
				state = BGMMMMHMIJF.ReadingBType;
				return false;
			}
			AGJCDPOFIFE = (BPKEFFDGEIC)NILNDHEKNLJ.GetBits(2);
			if (AGJCDPOFIFE == BPKEFFDGEIC.Dynamic)
			{
				state = BGMMMMHMIJF.ReadingNumLitCodes;
			}
			else if (AGJCDPOFIFE == BPKEFFDGEIC.Static)
			{
				BADNHOBGEBO = HuffmanTree.CMJKCGMHABI();
				IOBEEOPEKJJ = HuffmanTree.CECHGKLBAAN();
				state = BGMMMMHMIJF.DecodeTop;
			}
			else
			{
				if (AGJCDPOFIFE != BPKEFFDGEIC.Uncompressed)
				{
					throw new InvalidDataException(SR.GetString("Unknown block type"));
				}
				state = BGMMMMHMIJF.UncompressedAligning;
			}
		}
		if (AGJCDPOFIFE == BPKEFFDGEIC.Dynamic)
		{
			flag = ((state >= BGMMMMHMIJF.DecodeTop) ? ILMILAAOOIJ(out DNCHJPDPNJK) : JOMLEBOKPNI());
		}
		else if (AGJCDPOFIFE == BPKEFFDGEIC.Static)
		{
			flag = ILMILAAOOIJ(out DNCHJPDPNJK);
		}
		else
		{
			if (AGJCDPOFIFE != BPKEFFDGEIC.Uncompressed)
			{
				throw new InvalidDataException(SR.GetString("Unknown block type"));
			}
			flag = PPHOKJAMIOE(out DNCHJPDPNJK);
		}
		if (DNCHJPDPNJK && PCLGPAHIBGH != 0)
		{
			if (hasFormatReader)
			{
				state = BGMMMMHMIJF.StartReadingFooter;
			}
			else
			{
				state = BGMMMMHMIJF.Done;
			}
		}
		return flag;
	}

	private bool PPHOKJAMIOE(out bool DNCHJPDPNJK)
	{
		DNCHJPDPNJK = false;
		while (true)
		{
			switch (state)
			{
			case BGMMMMHMIJF.UncompressedAligning:
				NILNDHEKNLJ.KHMFPEJHFHC();
				state = BGMMMMHMIJF.UncompressedByte1;
				goto case BGMMMMHMIJF.UncompressedByte1;
			case BGMMMMHMIJF.UncompressedByte1:
			case BGMMMMHMIJF.UncompressedByte2:
			case BGMMMMHMIJF.UncompressedByte3:
			case BGMMMMHMIJF.UncompressedByte4:
			{
				int num2 = NILNDHEKNLJ.GetBits(8);
				if (num2 < 0)
				{
					return false;
				}
				JEGPOHGJPIK[(int)(state - 16)] = (byte)num2;
				if (state == BGMMMMHMIJF.UncompressedByte4)
				{
					IHIGJLEBJBI = JEGPOHGJPIK[0] + JEGPOHGJPIK[1] * 256;
					int num3 = JEGPOHGJPIK[2] + JEGPOHGJPIK[3] * 256;
					if ((ushort)IHIGJLEBJBI != (ushort)(~num3))
					{
						throw new InvalidDataException(SR.GetString("Invalid block length"));
					}
				}
				break;
			}
			case BGMMMMHMIJF.DecodingUncompressed:
			{
				int num = output.CopyFrom(NILNDHEKNLJ, IHIGJLEBJBI);
				IHIGJLEBJBI -= num;
				if (IHIGJLEBJBI == 0)
				{
					state = BGMMMMHMIJF.ReadingBFinal;
					DNCHJPDPNJK = true;
					return true;
				}
				if (output.JBPBBAEEAFO() == 0)
				{
					return true;
				}
				return false;
			}
			default:
				throw new InvalidDataException(SR.GetString("Unknown state"));
			}
			state++;
		}
	}

	private bool ILMILAAOOIJ(out bool COIHANBPBME)
	{
		COIHANBPBME = false;
		int num = output.JBPBBAEEAFO();
		while (num > 258)
		{
			switch (state)
			{
			case BGMMMMHMIJF.DecodeTop:
			{
				int num2 = BADNHOBGEBO.NBKGIKBOJGM(NILNDHEKNLJ);
				if (num2 < 0)
				{
					return false;
				}
				if (num2 < 256)
				{
					output.Write((byte)num2);
					num--;
					break;
				}
				if (num2 == 256)
				{
					COIHANBPBME = true;
					state = BGMMMMHMIJF.ReadingBFinal;
					return true;
				}
				num2 -= 257;
				if (num2 < 8)
				{
					num2 += 3;
					extraBits = 0;
				}
				else if (num2 == 28)
				{
					num2 = 258;
					extraBits = 0;
				}
				else
				{
					if (num2 < 0 || num2 >= DEODDHFPBFE.Length)
					{
						throw new InvalidDataException(SR.GetString("Invalid data"));
					}
					extraBits = DEODDHFPBFE[num2];
				}
				BDBOAEGELMC = num2;
				goto case BGMMMMHMIJF.HaveInitialLength;
			}
			case BGMMMMHMIJF.HaveInitialLength:
				if (extraBits > 0)
				{
					state = BGMMMMHMIJF.HaveInitialLength;
					int num4 = NILNDHEKNLJ.GetBits(extraBits);
					if (num4 < 0)
					{
						return false;
					}
					if (BDBOAEGELMC < 0 || BDBOAEGELMC >= NMNHMEJNLKB.Length)
					{
						throw new InvalidDataException(SR.GetString("Invalid data"));
					}
					BDBOAEGELMC = NMNHMEJNLKB[BDBOAEGELMC] + num4;
				}
				state = BGMMMMHMIJF.HaveFullLength;
				goto case BGMMMMHMIJF.HaveFullLength;
			case BGMMMMHMIJF.HaveFullLength:
				if (AGJCDPOFIFE == BPKEFFDGEIC.Dynamic)
				{
					GHCFEMCPBFA = IOBEEOPEKJJ.NBKGIKBOJGM(NILNDHEKNLJ);
				}
				else
				{
					GHCFEMCPBFA = NILNDHEKNLJ.GetBits(5);
					if (GHCFEMCPBFA >= 0)
					{
						GHCFEMCPBFA = MIHBNJNICPA[GHCFEMCPBFA];
					}
				}
				if (GHCFEMCPBFA < 0)
				{
					return false;
				}
				state = BGMMMMHMIJF.HaveDistCode;
				goto case BGMMMMHMIJF.HaveDistCode;
			case BGMMMMHMIJF.HaveDistCode:
			{
				int oIOMNNFMDOO;
				if (GHCFEMCPBFA > 3)
				{
					extraBits = GHCFEMCPBFA - 2 >> 1;
					int num3 = NILNDHEKNLJ.GetBits(extraBits);
					if (num3 < 0)
					{
						return false;
					}
					oIOMNNFMDOO = KCGFDBBLOLL[GHCFEMCPBFA] + num3;
				}
				else
				{
					oIOMNNFMDOO = GHCFEMCPBFA + 1;
				}
				output.WriteLengthDistance(BDBOAEGELMC, oIOMNNFMDOO);
				num -= BDBOAEGELMC;
				state = BGMMMMHMIJF.DecodeTop;
				break;
			}
			default:
				throw new InvalidDataException(SR.GetString("Unknown state"));
			}
		}
		return true;
	}

	private bool JOMLEBOKPNI()
	{
		switch (state)
		{
		case BGMMMMHMIJF.ReadingNumLitCodes:
			GOIJLBOAAJO = NILNDHEKNLJ.GetBits(5);
			if (GOIJLBOAAJO < 0)
			{
				return false;
			}
			GOIJLBOAAJO += 257;
			state = BGMMMMHMIJF.ReadingNumDistCodes;
			goto case BGMMMMHMIJF.ReadingNumDistCodes;
		case BGMMMMHMIJF.ReadingNumDistCodes:
			NHDAMIACBBI = NILNDHEKNLJ.GetBits(5);
			if (NHDAMIACBBI < 0)
			{
				return false;
			}
			NHDAMIACBBI++;
			state = BGMMMMHMIJF.ReadingNumCodeLengthCodes;
			goto case BGMMMMHMIJF.ReadingNumCodeLengthCodes;
		case BGMMMMHMIJF.ReadingNumCodeLengthCodes:
			DIOGELJGGFC = NILNDHEKNLJ.GetBits(4);
			if (DIOGELJGGFC < 0)
			{
				return false;
			}
			DIOGELJGGFC += 4;
			DOLKGPHFKPJ = 0;
			state = BGMMMMHMIJF.ReadingCodeLengthCodes;
			goto case BGMMMMHMIJF.ReadingCodeLengthCodes;
		case BGMMMMHMIJF.ReadingCodeLengthCodes:
		{
			while (DOLKGPHFKPJ < DIOGELJGGFC)
			{
				int num2 = NILNDHEKNLJ.GetBits(3);
				if (num2 < 0)
				{
					return false;
				}
				KKIAJONPLKI[KCDJIAOANON[DOLKGPHFKPJ]] = (byte)num2;
				DOLKGPHFKPJ++;
			}
			for (int l = DIOGELJGGFC; l < KCDJIAOANON.Length; l++)
			{
				KKIAJONPLKI[KCDJIAOANON[l]] = 0;
			}
			PFCJKHONLFF = new HuffmanTree(KKIAJONPLKI);
			AGBMNMICLFA = GOIJLBOAAJO + NHDAMIACBBI;
			DOLKGPHFKPJ = 0;
			state = BGMMMMHMIJF.ReadingTreeCodesBefore;
			goto case BGMMMMHMIJF.ReadingTreeCodesBefore;
		}
		case BGMMMMHMIJF.ReadingTreeCodesBefore:
		case BGMMMMHMIJF.ReadingTreeCodesAfter:
		{
			while (DOLKGPHFKPJ < AGBMNMICLFA)
			{
				if (state == BGMMMMHMIJF.ReadingTreeCodesBefore && (PCPIKCIKHLO = PFCJKHONLFF.NBKGIKBOJGM(NILNDHEKNLJ)) < 0)
				{
					return false;
				}
				if (PCPIKCIKHLO <= 15)
				{
					OBEOJJGLMFG[DOLKGPHFKPJ++] = (byte)PCPIKCIKHLO;
				}
				else
				{
					if (!NILNDHEKNLJ.EnsureBitsAvailable(7))
					{
						state = BGMMMMHMIJF.ReadingTreeCodesAfter;
						return false;
					}
					if (PCPIKCIKHLO == 16)
					{
						if (DOLKGPHFKPJ == 0)
						{
							throw new InvalidDataException();
						}
						byte b = OBEOJJGLMFG[DOLKGPHFKPJ - 1];
						int num = NILNDHEKNLJ.GetBits(2) + 3;
						if (DOLKGPHFKPJ + num > AGBMNMICLFA)
						{
							throw new InvalidDataException();
						}
						for (int i = 0; i < num; i++)
						{
							OBEOJJGLMFG[DOLKGPHFKPJ++] = b;
						}
					}
					else if (PCPIKCIKHLO == 17)
					{
						int num = NILNDHEKNLJ.GetBits(3) + 3;
						if (DOLKGPHFKPJ + num > AGBMNMICLFA)
						{
							throw new InvalidDataException();
						}
						for (int j = 0; j < num; j++)
						{
							OBEOJJGLMFG[DOLKGPHFKPJ++] = 0;
						}
					}
					else
					{
						int num = NILNDHEKNLJ.GetBits(7) + 11;
						if (DOLKGPHFKPJ + num > AGBMNMICLFA)
						{
							throw new InvalidDataException();
						}
						for (int k = 0; k < num; k++)
						{
							OBEOJJGLMFG[DOLKGPHFKPJ++] = 0;
						}
					}
				}
				state = BGMMMMHMIJF.ReadingTreeCodesBefore;
			}
			byte[] array = new byte[288];
			byte[] array2 = new byte[32];
			Array.Copy(OBEOJJGLMFG, array, GOIJLBOAAJO);
			Array.Copy(OBEOJJGLMFG, GOIJLBOAAJO, array2, 0, NHDAMIACBBI);
			if (array[256] == 0)
			{
				throw new InvalidDataException();
			}
			BADNHOBGEBO = new HuffmanTree(array);
			IOBEEOPEKJJ = new HuffmanTree(array2);
			state = BGMMMMHMIJF.DecodeTop;
			return true;
		}
		default:
			throw new InvalidDataException(SR.GetString("Unknown state"));
		}
	}
}
