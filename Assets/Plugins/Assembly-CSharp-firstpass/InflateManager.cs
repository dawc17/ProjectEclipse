internal sealed class InflateManager
{
	private enum EMMGLMMPKDO
	{
		METHOD = 0,
		FLAG = 1,
		DICT4 = 2,
		DICT3 = 3,
		DICT2 = 4,
		DICT1 = 5,
		DICT0 = 6,
		BLOCKS = 7,
		CHECK4 = 8,
		CHECK3 = 9,
		CHECK2 = 10,
		CHECK1 = 11,
		DONE = 12,
		BAD = 13
	}

	private const int LEDINGHODAJ = 32;

	private const int GLGHJDNCDON = 8;

	private EMMGLMMPKDO NMMPBADCFHK;

	internal ZlibCodec CJMKCEHHMCH;

	internal int FJLOLCPJACB;

	internal uint MNAKBCPLIMJ;

	internal uint IFIAIEJEEOK;

	internal int OMHIFDHPIMD;

	private bool _handleRfc1950HeaderBytes = true;

	internal int BEJLEKCJHFM;

	internal InflateBlocks CGKHDGJKOMG;

	private static readonly byte[] mark = new byte[4] { 0, 0, 255, 255 };

	internal bool NGMPHIPIFLC
	{
		get
		{
			return CJPMKPAIMCF();
		}
		set
		{
			set_HandleRfc1950HeaderBytes(value);
		}
	}

	public InflateManager()
	{
	}

	public InflateManager(bool FOCCBLONFOF)
	{
		_handleRfc1950HeaderBytes = FOCCBLONFOF;
	}

	internal bool CJPMKPAIMCF()
	{
		return _handleRfc1950HeaderBytes;
	}

	internal void set_HandleRfc1950HeaderBytes(bool value)
	{
		_handleRfc1950HeaderBytes = value;
	}

	internal int Reset()
	{
		CJMKCEHHMCH.ALJBBHPGGPA = (CJMKCEHHMCH.HCDKLJJLMOD = 0L);
		CJMKCEHHMCH.Message = null;
		NMMPBADCFHK = ((!CJPMKPAIMCF()) ? EMMGLMMPKDO.BLOCKS : EMMGLMMPKDO.METHOD);
		CGKHDGJKOMG.Reset();
		return 0;
	}

	internal int PLHPGFGAGKJ()
	{
		if (CGKHDGJKOMG != null)
		{
			CGKHDGJKOMG.PJNFHNFLNNO();
		}
		CGKHDGJKOMG = null;
		return 0;
	}

	internal int EHAJODIAFEG(ZlibCodec HNJFOALABOA, int OKPHBCHECPI)
	{
		CJMKCEHHMCH = HNJFOALABOA;
		CJMKCEHHMCH.Message = null;
		CGKHDGJKOMG = null;
		if (OKPHBCHECPI < 8 || OKPHBCHECPI > 15)
		{
			PLHPGFGAGKJ();
			throw new ZlibException("Bad window size.");
		}
		BEJLEKCJHFM = OKPHBCHECPI;
		CGKHDGJKOMG = new InflateBlocks(HNJFOALABOA, (!CJPMKPAIMCF()) ? null : this, 1 << OKPHBCHECPI);
		Reset();
		return 0;
	}

	internal int Inflate(AFJHGKAEJPG NGBJDNFAPKC)
	{
		if (CJMKCEHHMCH.PEFOCMDODLD == null)
		{
			throw new ZlibException("InputBuffer is null. ");
		}
		int num = 0;
		int num2 = -5;
		while (true)
		{
			switch (NMMPBADCFHK)
			{
			case EMMGLMMPKDO.METHOD:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				if (((FJLOLCPJACB = CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++]) & 0xF) != 8)
				{
					NMMPBADCFHK = EMMGLMMPKDO.BAD;
					CJMKCEHHMCH.Message = string.Format("unknown compression method (0x{0:X2})", FJLOLCPJACB);
					OMHIFDHPIMD = 5;
				}
				else if ((FJLOLCPJACB >> 4) + 8 > BEJLEKCJHFM)
				{
					NMMPBADCFHK = EMMGLMMPKDO.BAD;
					CJMKCEHHMCH.Message = string.Format("invalid window size ({0})", (FJLOLCPJACB >> 4) + 8);
					OMHIFDHPIMD = 5;
				}
				else
				{
					NMMPBADCFHK = EMMGLMMPKDO.FLAG;
				}
				break;
			case EMMGLMMPKDO.FLAG:
			{
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				int num3 = CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] & 0xFF;
				if (((FJLOLCPJACB << 8) + num3) % 31 != 0)
				{
					NMMPBADCFHK = EMMGLMMPKDO.BAD;
					CJMKCEHHMCH.Message = "incorrect header check";
					OMHIFDHPIMD = 5;
				}
				else
				{
					NMMPBADCFHK = (((num3 & 0x20) != 0) ? EMMGLMMPKDO.DICT4 : EMMGLMMPKDO.BLOCKS);
				}
				break;
			}
			case EMMGLMMPKDO.DICT4:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK = (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 24) & 0xFF000000u);
				NMMPBADCFHK = EMMGLMMPKDO.DICT3;
				break;
			case EMMGLMMPKDO.DICT3:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 16) & 0xFF0000);
				NMMPBADCFHK = EMMGLMMPKDO.DICT2;
				break;
			case EMMGLMMPKDO.DICT2:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 8) & 0xFF00);
				NMMPBADCFHK = EMMGLMMPKDO.DICT1;
				break;
			case EMMGLMMPKDO.DICT1:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)(CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] & 0xFF);
				CJMKCEHHMCH._Adler32 = IFIAIEJEEOK;
				NMMPBADCFHK = EMMGLMMPKDO.DICT0;
				return 2;
			case EMMGLMMPKDO.DICT0:
				NMMPBADCFHK = EMMGLMMPKDO.BAD;
				CJMKCEHHMCH.Message = "need dictionary";
				OMHIFDHPIMD = 0;
				return -2;
			case EMMGLMMPKDO.BLOCKS:
				num2 = CGKHDGJKOMG.HDEHLIKBKJG(num2);
				switch (num2)
				{
				case -3:
					NMMPBADCFHK = EMMGLMMPKDO.BAD;
					OMHIFDHPIMD = 0;
					goto end_IL_0028;
				case 0:
					num2 = num;
					break;
				}
				if (num2 != 1)
				{
					return num2;
				}
				num2 = num;
				MNAKBCPLIMJ = CGKHDGJKOMG.Reset();
				if (!CJPMKPAIMCF())
				{
					NMMPBADCFHK = EMMGLMMPKDO.DONE;
					return 1;
				}
				NMMPBADCFHK = EMMGLMMPKDO.CHECK4;
				break;
			case EMMGLMMPKDO.CHECK4:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK = (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 24) & 0xFF000000u);
				NMMPBADCFHK = EMMGLMMPKDO.CHECK3;
				break;
			case EMMGLMMPKDO.CHECK3:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 16) & 0xFF0000);
				NMMPBADCFHK = EMMGLMMPKDO.CHECK2;
				break;
			case EMMGLMMPKDO.CHECK2:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)((CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] << 8) & 0xFF00);
				NMMPBADCFHK = EMMGLMMPKDO.CHECK1;
				break;
			case EMMGLMMPKDO.CHECK1:
				if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
				{
					return num2;
				}
				num2 = num;
				CJMKCEHHMCH.IAPJEIDMGNP--;
				CJMKCEHHMCH.ALJBBHPGGPA++;
				IFIAIEJEEOK += (uint)(CJMKCEHHMCH.PEFOCMDODLD[CJMKCEHHMCH.LMIPBGGILEJ++] & 0xFF);
				if (MNAKBCPLIMJ != IFIAIEJEEOK)
				{
					NMMPBADCFHK = EMMGLMMPKDO.BAD;
					CJMKCEHHMCH.Message = "incorrect data check";
					OMHIFDHPIMD = 5;
					break;
				}
				NMMPBADCFHK = EMMGLMMPKDO.DONE;
				return 1;
			case EMMGLMMPKDO.DONE:
				return 1;
			case EMMGLMMPKDO.BAD:
				throw new ZlibException(string.Format("Bad state ({0})", CJMKCEHHMCH.Message));
			default:
				{
					throw new ZlibException("Stream error.");
				}
				end_IL_0028:
				break;
			}
		}
	}

	internal int SetDictionary(byte[] dictionary)
	{
		int iLENLCMAMBH = 0;
		int num = dictionary.Length;
		if (NMMPBADCFHK != EMMGLMMPKDO.DICT0)
		{
			throw new ZlibException("Stream error.");
		}
		if (Adler.IAJPFDALGJM(1u, dictionary, 0, dictionary.Length) != CJMKCEHHMCH._Adler32)
		{
			return -3;
		}
		CJMKCEHHMCH._Adler32 = Adler.IAJPFDALGJM(0u, null, 0, 0);
		if (num >= 1 << BEJLEKCJHFM)
		{
			num = (1 << BEJLEKCJHFM) - 1;
			iLENLCMAMBH = dictionary.Length - num;
		}
		CGKHDGJKOMG.SetDictionary(dictionary, iLENLCMAMBH, num);
		NMMPBADCFHK = EMMGLMMPKDO.BLOCKS;
		return 0;
	}

	internal int JGCOKJJDLBC()
	{
		if (NMMPBADCFHK != EMMGLMMPKDO.BAD)
		{
			NMMPBADCFHK = EMMGLMMPKDO.BAD;
			OMHIFDHPIMD = 0;
		}
		int num;
		if ((num = CJMKCEHHMCH.IAPJEIDMGNP) == 0)
		{
			return -5;
		}
		int num2 = CJMKCEHHMCH.LMIPBGGILEJ;
		int num3 = OMHIFDHPIMD;
		while (num != 0 && num3 < 4)
		{
			num3 = ((CJMKCEHHMCH.PEFOCMDODLD[num2] != mark[num3]) ? ((CJMKCEHHMCH.PEFOCMDODLD[num2] == 0) ? (4 - num3) : 0) : (num3 + 1));
			num2++;
			num--;
		}
		CJMKCEHHMCH.ALJBBHPGGPA += num2 - CJMKCEHHMCH.LMIPBGGILEJ;
		CJMKCEHHMCH.LMIPBGGILEJ = num2;
		CJMKCEHHMCH.IAPJEIDMGNP = num;
		OMHIFDHPIMD = num3;
		if (num3 != 4)
		{
			return -3;
		}
		long aLJBBHPGGPA = CJMKCEHHMCH.ALJBBHPGGPA;
		long hCDKLJJLMOD = CJMKCEHHMCH.HCDKLJJLMOD;
		Reset();
		CJMKCEHHMCH.ALJBBHPGGPA = aLJBBHPGGPA;
		CJMKCEHHMCH.HCDKLJJLMOD = hCDKLJJLMOD;
		NMMPBADCFHK = EMMGLMMPKDO.BLOCKS;
		return 0;
	}

	internal int NGLFANAHOJJ(ZlibCodec LKPCKJOLJDO)
	{
		return CGKHDGJKOMG.NGLFANAHOJJ();
	}
}
