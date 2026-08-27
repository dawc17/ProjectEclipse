using System;

internal sealed class InflateBlocks
{
	private enum NHGOBMFNMLD
	{
		TYPE = 0,
		LENS = 1,
		STORED = 2,
		TABLE = 3,
		BTREE = 4,
		DTREE = 5,
		CODES = 6,
		DRY = 7,
		DONE = 8,
		BAD = 9
	}

	private const int MANY = 1440;

	internal static readonly int[] LCKCNAGJMGG = new int[19]
	{
		16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
		11, 4, 12, 3, 13, 2, 14, 1, 15
	};

	private NHGOBMFNMLD NMMPBADCFHK;

	internal int MKICABFAHFA;

	internal int BFGHBIMJHAK;

	internal int index;

	internal int[] DKFIBIMAJLL;

	internal int[] KKFMKNCBLDC = new int[1];

	internal int[] ILPHPGNPGAE = new int[1];

	internal InflateCodes ELBFMJHINKF = new InflateCodes();

	internal int IBMGAPMHMOB;

	internal ZlibCodec CJMKCEHHMCH;

	internal int DBFGKGGCEAI;

	internal int FPGCIJMGFLH;

	internal int[] GAMNAIHAIDP;

	internal byte[] window;

	internal int PCLFFOBJJFO;

	internal int IONENIAEDKJ;

	internal int HBFBCHDJEBM;

	internal object checkfn;

	internal uint check;

	internal InfTree BPLHOPHCKPC = new InfTree();

	internal InflateBlocks(ZlibCodec HNJFOALABOA, object checkfn, int OKPHBCHECPI)
	{
		CJMKCEHHMCH = HNJFOALABOA;
		GAMNAIHAIDP = new int[4320];
		window = new byte[OKPHBCHECPI];
		PCLFFOBJJFO = OKPHBCHECPI;
		this.checkfn = checkfn;
		NMMPBADCFHK = NHGOBMFNMLD.TYPE;
		Reset();
	}

	internal uint Reset()
	{
		uint iADLPBPGLKO = check;
		NMMPBADCFHK = NHGOBMFNMLD.TYPE;
		DBFGKGGCEAI = 0;
		FPGCIJMGFLH = 0;
		IONENIAEDKJ = (HBFBCHDJEBM = 0);
		if (checkfn != null)
		{
			CJMKCEHHMCH._Adler32 = (check = Adler.IAJPFDALGJM(0u, null, 0, 0));
		}
		return iADLPBPGLKO;
	}

	internal int HDEHLIKBKJG(int BOPODEAIEBJ)
	{
		int num = CJMKCEHHMCH.LMIPBGGILEJ;
		int num2 = CJMKCEHHMCH.IAPJEIDMGNP;
		int num3 = FPGCIJMGFLH;
		int i = DBFGKGGCEAI;
		int num4 = HBFBCHDJEBM;
		int num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
		while (true)
		{
			switch (NMMPBADCFHK)
			{
			case NHGOBMFNMLD.TYPE:
			{
				for (; i < 3; i += 8)
				{
					if (num2 != 0)
					{
						BOPODEAIEBJ = 0;
						num2--;
						num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
						continue;
					}
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				int num6 = num3 & 7;
				IBMGAPMHMOB = num6 & 1;
				switch ((uint)num6 >> 1)
				{
				case 0u:
					num3 >>= 3;
					i -= 3;
					num6 = i & 7;
					num3 >>= num6;
					i -= num6;
					NMMPBADCFHK = NHGOBMFNMLD.LENS;
					break;
				case 1u:
				{
					int[] array = new int[1];
					int[] array2 = new int[1];
					int[][] array3 = new int[1][];
					int[][] array4 = new int[1][];
					InfTree.KFBEFCDGIDA(array, array2, array3, array4, CJMKCEHHMCH);
					ELBFMJHINKF.Init(array[0], array2[0], array3[0], 0, array4[0], 0);
					num3 >>= 3;
					i -= 3;
					NMMPBADCFHK = NHGOBMFNMLD.CODES;
					break;
				}
				case 2u:
					num3 >>= 3;
					i -= 3;
					NMMPBADCFHK = NHGOBMFNMLD.TABLE;
					break;
				case 3u:
					num3 >>= 3;
					i -= 3;
					NMMPBADCFHK = NHGOBMFNMLD.BAD;
					CJMKCEHHMCH.Message = "invalid block type";
					BOPODEAIEBJ = -3;
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				break;
			}
			case NHGOBMFNMLD.LENS:
				for (; i < 32; i += 8)
				{
					if (num2 != 0)
					{
						BOPODEAIEBJ = 0;
						num2--;
						num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
						continue;
					}
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				if (((~num3 >> 16) & 0xFFFF) != (num3 & 0xFFFF))
				{
					NMMPBADCFHK = NHGOBMFNMLD.BAD;
					CJMKCEHHMCH.Message = "invalid stored block lengths";
					BOPODEAIEBJ = -3;
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				MKICABFAHFA = num3 & 0xFFFF;
				num3 = (i = 0);
				NMMPBADCFHK = ((MKICABFAHFA != 0) ? NHGOBMFNMLD.STORED : ((IBMGAPMHMOB != 0) ? NHGOBMFNMLD.DRY : NHGOBMFNMLD.TYPE));
				break;
			case NHGOBMFNMLD.STORED:
			{
				if (num2 == 0)
				{
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				if (num5 == 0)
				{
					if (num4 == PCLFFOBJJFO && IONENIAEDKJ != 0)
					{
						num4 = 0;
						num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
					}
					if (num5 == 0)
					{
						HBFBCHDJEBM = num4;
						BOPODEAIEBJ = MKPBJGMJPMI(BOPODEAIEBJ);
						num4 = HBFBCHDJEBM;
						num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
						if (num4 == PCLFFOBJJFO && IONENIAEDKJ != 0)
						{
							num4 = 0;
							num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
						}
						if (num5 == 0)
						{
							FPGCIJMGFLH = num3;
							DBFGKGGCEAI = i;
							CJMKCEHHMCH.IAPJEIDMGNP = num2;
							CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
							CJMKCEHHMCH.LMIPBGGILEJ = num;
							HBFBCHDJEBM = num4;
							return MKPBJGMJPMI(BOPODEAIEBJ);
						}
					}
				}
				BOPODEAIEBJ = 0;
				int num6 = MKICABFAHFA;
				if (num6 > num2)
				{
					num6 = num2;
				}
				if (num6 > num5)
				{
					num6 = num5;
				}
				Array.Copy(CJMKCEHHMCH.PEFOCMDODLD, num, window, num4, num6);
				num += num6;
				num2 -= num6;
				num4 += num6;
				num5 -= num6;
				if ((MKICABFAHFA -= num6) == 0)
				{
					NMMPBADCFHK = ((IBMGAPMHMOB != 0) ? NHGOBMFNMLD.DRY : NHGOBMFNMLD.TYPE);
				}
				break;
			}
			case NHGOBMFNMLD.TABLE:
			{
				for (; i < 14; i += 8)
				{
					if (num2 != 0)
					{
						BOPODEAIEBJ = 0;
						num2--;
						num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
						continue;
					}
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				int num6 = (BFGHBIMJHAK = num3 & 0x3FFF);
				if ((num6 & 0x1F) > 29 || ((num6 >> 5) & 0x1F) > 29)
				{
					NMMPBADCFHK = NHGOBMFNMLD.BAD;
					CJMKCEHHMCH.Message = "too many length or distance symbols";
					BOPODEAIEBJ = -3;
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				num6 = 258 + (num6 & 0x1F) + ((num6 >> 5) & 0x1F);
				if (DKFIBIMAJLL == null || DKFIBIMAJLL.Length < num6)
				{
					DKFIBIMAJLL = new int[num6];
				}
				else
				{
					Array.Clear(DKFIBIMAJLL, 0, num6);
				}
				num3 >>= 14;
				i -= 14;
				index = 0;
				NMMPBADCFHK = NHGOBMFNMLD.BTREE;
				goto case NHGOBMFNMLD.BTREE;
			}
			case NHGOBMFNMLD.BTREE:
			{
				while (index < 4 + (BFGHBIMJHAK >> 10))
				{
					for (; i < 3; i += 8)
					{
						if (num2 != 0)
						{
							BOPODEAIEBJ = 0;
							num2--;
							num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
							continue;
						}
						FPGCIJMGFLH = num3;
						DBFGKGGCEAI = i;
						CJMKCEHHMCH.IAPJEIDMGNP = num2;
						CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
						CJMKCEHHMCH.LMIPBGGILEJ = num;
						HBFBCHDJEBM = num4;
						return MKPBJGMJPMI(BOPODEAIEBJ);
					}
					DKFIBIMAJLL[LCKCNAGJMGG[index++]] = num3 & 7;
					num3 >>= 3;
					i -= 3;
				}
				while (index < 19)
				{
					DKFIBIMAJLL[LCKCNAGJMGG[index++]] = 0;
				}
				KKFMKNCBLDC[0] = 7;
				int num6 = BPLHOPHCKPC.NLOHPGJGJJN(DKFIBIMAJLL, KKFMKNCBLDC, ILPHPGNPGAE, GAMNAIHAIDP, CJMKCEHHMCH);
				if (num6 != 0)
				{
					BOPODEAIEBJ = num6;
					if (BOPODEAIEBJ == -3)
					{
						DKFIBIMAJLL = null;
						NMMPBADCFHK = NHGOBMFNMLD.BAD;
					}
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				index = 0;
				NMMPBADCFHK = NHGOBMFNMLD.DTREE;
				goto case NHGOBMFNMLD.DTREE;
			}
			case NHGOBMFNMLD.DTREE:
			{
				int num6;
				while (true)
				{
					num6 = BFGHBIMJHAK;
					if (index >= 258 + (num6 & 0x1F) + ((num6 >> 5) & 0x1F))
					{
						break;
					}
					for (num6 = KKFMKNCBLDC[0]; i < num6; i += 8)
					{
						if (num2 != 0)
						{
							BOPODEAIEBJ = 0;
							num2--;
							num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
							continue;
						}
						FPGCIJMGFLH = num3;
						DBFGKGGCEAI = i;
						CJMKCEHHMCH.IAPJEIDMGNP = num2;
						CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
						CJMKCEHHMCH.LMIPBGGILEJ = num;
						HBFBCHDJEBM = num4;
						return MKPBJGMJPMI(BOPODEAIEBJ);
					}
					num6 = GAMNAIHAIDP[(ILPHPGNPGAE[0] + (num3 & InternalInflateConstants.PEKJPCOGGBP[num6])) * 3 + 1];
					int num7 = GAMNAIHAIDP[(ILPHPGNPGAE[0] + (num3 & InternalInflateConstants.PEKJPCOGGBP[num6])) * 3 + 2];
					if (num7 < 16)
					{
						num3 >>= num6;
						i -= num6;
						DKFIBIMAJLL[index++] = num7;
						continue;
					}
					int num8 = ((num7 != 18) ? (num7 - 14) : 7);
					int num9 = ((num7 != 18) ? 3 : 11);
					for (; i < num6 + num8; i += 8)
					{
						if (num2 != 0)
						{
							BOPODEAIEBJ = 0;
							num2--;
							num3 |= (CJMKCEHHMCH.PEFOCMDODLD[num++] & 0xFF) << i;
							continue;
						}
						FPGCIJMGFLH = num3;
						DBFGKGGCEAI = i;
						CJMKCEHHMCH.IAPJEIDMGNP = num2;
						CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
						CJMKCEHHMCH.LMIPBGGILEJ = num;
						HBFBCHDJEBM = num4;
						return MKPBJGMJPMI(BOPODEAIEBJ);
					}
					num3 >>= num6;
					i -= num6;
					num9 += num3 & InternalInflateConstants.PEKJPCOGGBP[num8];
					num3 >>= num8;
					i -= num8;
					num8 = index;
					num6 = BFGHBIMJHAK;
					if (num8 + num9 > 258 + (num6 & 0x1F) + ((num6 >> 5) & 0x1F) || (num7 == 16 && num8 < 1))
					{
						DKFIBIMAJLL = null;
						NMMPBADCFHK = NHGOBMFNMLD.BAD;
						CJMKCEHHMCH.Message = "invalid bit length repeat";
						BOPODEAIEBJ = -3;
						FPGCIJMGFLH = num3;
						DBFGKGGCEAI = i;
						CJMKCEHHMCH.IAPJEIDMGNP = num2;
						CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
						CJMKCEHHMCH.LMIPBGGILEJ = num;
						HBFBCHDJEBM = num4;
						return MKPBJGMJPMI(BOPODEAIEBJ);
					}
					num7 = ((num7 == 16) ? DKFIBIMAJLL[num8 - 1] : 0);
					do
					{
						DKFIBIMAJLL[num8++] = num7;
					}
					while (--num9 != 0);
					index = num8;
				}
				ILPHPGNPGAE[0] = -1;
				int[] array5 = new int[1] { 9 };
				int[] array6 = new int[1] { 6 };
				int[] array7 = new int[1];
				int[] array8 = new int[1];
				num6 = BFGHBIMJHAK;
				num6 = BPLHOPHCKPC.ENIFNPJMGIB(257 + (num6 & 0x1F), 1 + ((num6 >> 5) & 0x1F), DKFIBIMAJLL, array5, array6, array7, array8, GAMNAIHAIDP, CJMKCEHHMCH);
				if (num6 != 0)
				{
					if (num6 == -3)
					{
						DKFIBIMAJLL = null;
						NMMPBADCFHK = NHGOBMFNMLD.BAD;
					}
					BOPODEAIEBJ = num6;
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				ELBFMJHINKF.Init(array5[0], array6[0], GAMNAIHAIDP, array7[0], GAMNAIHAIDP, array8[0]);
				NMMPBADCFHK = NHGOBMFNMLD.CODES;
				goto case NHGOBMFNMLD.CODES;
			}
			case NHGOBMFNMLD.CODES:
				FPGCIJMGFLH = num3;
				DBFGKGGCEAI = i;
				CJMKCEHHMCH.IAPJEIDMGNP = num2;
				CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
				CJMKCEHHMCH.LMIPBGGILEJ = num;
				HBFBCHDJEBM = num4;
				BOPODEAIEBJ = ELBFMJHINKF.HDEHLIKBKJG(this, BOPODEAIEBJ);
				if (BOPODEAIEBJ != 1)
				{
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				BOPODEAIEBJ = 0;
				num = CJMKCEHHMCH.LMIPBGGILEJ;
				num2 = CJMKCEHHMCH.IAPJEIDMGNP;
				num3 = FPGCIJMGFLH;
				i = DBFGKGGCEAI;
				num4 = HBFBCHDJEBM;
				num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
				if (IBMGAPMHMOB == 0)
				{
					NMMPBADCFHK = NHGOBMFNMLD.TYPE;
					break;
				}
				NMMPBADCFHK = NHGOBMFNMLD.DRY;
				goto case NHGOBMFNMLD.DRY;
			case NHGOBMFNMLD.DRY:
				HBFBCHDJEBM = num4;
				BOPODEAIEBJ = MKPBJGMJPMI(BOPODEAIEBJ);
				num4 = HBFBCHDJEBM;
				num5 = ((num4 >= IONENIAEDKJ) ? (PCLFFOBJJFO - num4) : (IONENIAEDKJ - num4 - 1));
				if (IONENIAEDKJ != HBFBCHDJEBM)
				{
					FPGCIJMGFLH = num3;
					DBFGKGGCEAI = i;
					CJMKCEHHMCH.IAPJEIDMGNP = num2;
					CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
					CJMKCEHHMCH.LMIPBGGILEJ = num;
					HBFBCHDJEBM = num4;
					return MKPBJGMJPMI(BOPODEAIEBJ);
				}
				NMMPBADCFHK = NHGOBMFNMLD.DONE;
				goto case NHGOBMFNMLD.DONE;
			case NHGOBMFNMLD.DONE:
				BOPODEAIEBJ = 1;
				FPGCIJMGFLH = num3;
				DBFGKGGCEAI = i;
				CJMKCEHHMCH.IAPJEIDMGNP = num2;
				CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
				CJMKCEHHMCH.LMIPBGGILEJ = num;
				HBFBCHDJEBM = num4;
				return MKPBJGMJPMI(BOPODEAIEBJ);
			case NHGOBMFNMLD.BAD:
				BOPODEAIEBJ = -3;
				FPGCIJMGFLH = num3;
				DBFGKGGCEAI = i;
				CJMKCEHHMCH.IAPJEIDMGNP = num2;
				CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
				CJMKCEHHMCH.LMIPBGGILEJ = num;
				HBFBCHDJEBM = num4;
				return MKPBJGMJPMI(BOPODEAIEBJ);
			default:
				BOPODEAIEBJ = -2;
				FPGCIJMGFLH = num3;
				DBFGKGGCEAI = i;
				CJMKCEHHMCH.IAPJEIDMGNP = num2;
				CJMKCEHHMCH.ALJBBHPGGPA += num - CJMKCEHHMCH.LMIPBGGILEJ;
				CJMKCEHHMCH.LMIPBGGILEJ = num;
				HBFBCHDJEBM = num4;
				return MKPBJGMJPMI(BOPODEAIEBJ);
			}
		}
	}

	internal void PJNFHNFLNNO()
	{
		Reset();
		window = null;
		GAMNAIHAIDP = null;
	}

	internal void SetDictionary(byte[] d, int ILENLCMAMBH, int HDKKKCDKFEE)
	{
		Array.Copy(d, ILENLCMAMBH, window, 0, HDKKKCDKFEE);
		IONENIAEDKJ = (HBFBCHDJEBM = HDKKKCDKFEE);
	}

	internal int NGLFANAHOJJ()
	{
		return (NMMPBADCFHK == NHGOBMFNMLD.LENS) ? 1 : 0;
	}

	internal int MKPBJGMJPMI(int BOPODEAIEBJ)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = ((i != 0) ? (HBFBCHDJEBM - IONENIAEDKJ) : (((IONENIAEDKJ > HBFBCHDJEBM) ? PCLFFOBJJFO : HBFBCHDJEBM) - IONENIAEDKJ));
			if (num == 0)
			{
				if (BOPODEAIEBJ == -5)
				{
					BOPODEAIEBJ = 0;
				}
				return BOPODEAIEBJ;
			}
			if (num > CJMKCEHHMCH.NBNGINIIKNA)
			{
				num = CJMKCEHHMCH.NBNGINIIKNA;
			}
			if (num != 0 && BOPODEAIEBJ == -5)
			{
				BOPODEAIEBJ = 0;
			}
			CJMKCEHHMCH.NBNGINIIKNA -= num;
			CJMKCEHHMCH.HCDKLJJLMOD += num;
			if (checkfn != null)
			{
				CJMKCEHHMCH._Adler32 = (check = Adler.IAJPFDALGJM(check, window, IONENIAEDKJ, num));
			}
			Array.Copy(window, IONENIAEDKJ, CJMKCEHHMCH.DKCGBABIAEN, CJMKCEHHMCH.EIBFDELHKNM, num);
			CJMKCEHHMCH.EIBFDELHKNM += num;
			IONENIAEDKJ += num;
			if (IONENIAEDKJ == PCLFFOBJJFO && i == 0)
			{
				IONENIAEDKJ = 0;
				if (HBFBCHDJEBM == PCLFFOBJJFO)
				{
					HBFBCHDJEBM = 0;
				}
			}
			else
			{
				i++;
			}
		}
		return BOPODEAIEBJ;
	}
}
