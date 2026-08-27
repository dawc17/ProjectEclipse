using System;

internal sealed class InflateCodes
{
	private const int HNOHMIKMEGG = 0;

	private const int DGFBGCNDNHC = 1;

	private const int HNCLAEAKAJD = 2;

	private const int JJKGECHIOGB = 3;

	private const int JDKAIFINFKM = 4;

	private const int EPNKALGIMJB = 5;

	private const int LEJDEIPDEMG = 6;

	private const int NMFDKKHFLAF = 7;

	private const int EIOHLLPEKKE = 8;

	private const int IDFCAHPKAMK = 9;

	internal int NMMPBADCFHK;

	internal int JCAJDBOMGOM;

	internal int[] EDBPBGAMMDO;

	internal int HFDPNDNCFCC;

	internal int MANJJMDPBFL;

	internal int DNOLBDAAAFD;

	internal int ACLJBJJAENG;

	internal int CGIBMHPALCO;

	internal byte MBIGEAHKFOH;

	internal byte GKNNODIJMEI;

	internal int[] JKLBBEFFMID;

	internal int GAJGGDLCHFF;

	internal int[] JDFLHKFAEOD;

	internal int MGGEIOOOFFO;

	internal InflateCodes()
	{
	}

	internal void Init(int GGEJHHHGPKN, int NBHIKILKMED, int[] AEFHBJIMPHM, int HLDNDJKELJE, int[] GICLKGGKJAG, int KMKIMJDIKHC)
	{
		NMMPBADCFHK = 0;
		MBIGEAHKFOH = (byte)GGEJHHHGPKN;
		GKNNODIJMEI = (byte)NBHIKILKMED;
		JKLBBEFFMID = AEFHBJIMPHM;
		GAJGGDLCHFF = HLDNDJKELJE;
		JDFLHKFAEOD = GICLKGGKJAG;
		MGGEIOOOFFO = KMKIMJDIKHC;
		EDBPBGAMMDO = null;
	}

	internal int HDEHLIKBKJG(InflateBlocks CGKHDGJKOMG, int BOPODEAIEBJ)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		ZlibCodec cJMKCEHHMCH = CGKHDGJKOMG.CJMKCEHHMCH;
		num3 = cJMKCEHHMCH.LMIPBGGILEJ;
		int num4 = cJMKCEHHMCH.IAPJEIDMGNP;
		num = CGKHDGJKOMG.FPGCIJMGFLH;
		num2 = CGKHDGJKOMG.DBFGKGGCEAI;
		int num5 = CGKHDGJKOMG.HBFBCHDJEBM;
		int num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
		while (true)
		{
			switch (NMMPBADCFHK)
			{
			case 0:
				if (num6 >= 258 && num4 >= 10)
				{
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					BOPODEAIEBJ = HPHGIOKICCK(MBIGEAHKFOH, GKNNODIJMEI, JKLBBEFFMID, GAJGGDLCHFF, JDFLHKFAEOD, MGGEIOOOFFO, CGKHDGJKOMG, cJMKCEHHMCH);
					num3 = cJMKCEHHMCH.LMIPBGGILEJ;
					num4 = cJMKCEHHMCH.IAPJEIDMGNP;
					num = CGKHDGJKOMG.FPGCIJMGFLH;
					num2 = CGKHDGJKOMG.DBFGKGGCEAI;
					num5 = CGKHDGJKOMG.HBFBCHDJEBM;
					num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
					if (BOPODEAIEBJ != 0)
					{
						NMMPBADCFHK = ((BOPODEAIEBJ != 1) ? 9 : 7);
						break;
					}
				}
				MANJJMDPBFL = MBIGEAHKFOH;
				EDBPBGAMMDO = JKLBBEFFMID;
				HFDPNDNCFCC = GAJGGDLCHFF;
				NMMPBADCFHK = 1;
				goto case 1;
			case 1:
			{
				int aCLJBJJAENG;
				for (aCLJBJJAENG = MANJJMDPBFL; num2 < aCLJBJJAENG; num2 += 8)
				{
					if (num4 != 0)
					{
						BOPODEAIEBJ = 0;
						num4--;
						num |= (cJMKCEHHMCH.PEFOCMDODLD[num3++] & 0xFF) << num2;
						continue;
					}
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				}
				int num7 = (HFDPNDNCFCC + (num & InternalInflateConstants.PEKJPCOGGBP[aCLJBJJAENG])) * 3;
				num >>= EDBPBGAMMDO[num7 + 1];
				num2 -= EDBPBGAMMDO[num7 + 1];
				int num8 = EDBPBGAMMDO[num7];
				if (num8 == 0)
				{
					DNOLBDAAAFD = EDBPBGAMMDO[num7 + 2];
					NMMPBADCFHK = 6;
					break;
				}
				if ((num8 & 0x10) != 0)
				{
					ACLJBJJAENG = num8 & 0xF;
					JCAJDBOMGOM = EDBPBGAMMDO[num7 + 2];
					NMMPBADCFHK = 2;
					break;
				}
				if ((num8 & 0x40) == 0)
				{
					MANJJMDPBFL = num8;
					HFDPNDNCFCC = num7 / 3 + EDBPBGAMMDO[num7 + 2];
					break;
				}
				if ((num8 & 0x20) != 0)
				{
					NMMPBADCFHK = 7;
					break;
				}
				NMMPBADCFHK = 9;
				cJMKCEHHMCH.Message = "invalid literal/length code";
				BOPODEAIEBJ = -3;
				CGKHDGJKOMG.FPGCIJMGFLH = num;
				CGKHDGJKOMG.DBFGKGGCEAI = num2;
				cJMKCEHHMCH.IAPJEIDMGNP = num4;
				cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
				cJMKCEHHMCH.LMIPBGGILEJ = num3;
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
			}
			case 2:
			{
				int aCLJBJJAENG;
				for (aCLJBJJAENG = ACLJBJJAENG; num2 < aCLJBJJAENG; num2 += 8)
				{
					if (num4 != 0)
					{
						BOPODEAIEBJ = 0;
						num4--;
						num |= (cJMKCEHHMCH.PEFOCMDODLD[num3++] & 0xFF) << num2;
						continue;
					}
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				}
				JCAJDBOMGOM += num & InternalInflateConstants.PEKJPCOGGBP[aCLJBJJAENG];
				num >>= aCLJBJJAENG;
				num2 -= aCLJBJJAENG;
				MANJJMDPBFL = GKNNODIJMEI;
				EDBPBGAMMDO = JDFLHKFAEOD;
				HFDPNDNCFCC = MGGEIOOOFFO;
				NMMPBADCFHK = 3;
				goto case 3;
			}
			case 3:
			{
				int aCLJBJJAENG;
				for (aCLJBJJAENG = MANJJMDPBFL; num2 < aCLJBJJAENG; num2 += 8)
				{
					if (num4 != 0)
					{
						BOPODEAIEBJ = 0;
						num4--;
						num |= (cJMKCEHHMCH.PEFOCMDODLD[num3++] & 0xFF) << num2;
						continue;
					}
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				}
				int num7 = (HFDPNDNCFCC + (num & InternalInflateConstants.PEKJPCOGGBP[aCLJBJJAENG])) * 3;
				num >>= EDBPBGAMMDO[num7 + 1];
				num2 -= EDBPBGAMMDO[num7 + 1];
				int num8 = EDBPBGAMMDO[num7];
				if ((num8 & 0x10) != 0)
				{
					ACLJBJJAENG = num8 & 0xF;
					CGIBMHPALCO = EDBPBGAMMDO[num7 + 2];
					NMMPBADCFHK = 4;
					break;
				}
				if ((num8 & 0x40) == 0)
				{
					MANJJMDPBFL = num8;
					HFDPNDNCFCC = num7 / 3 + EDBPBGAMMDO[num7 + 2];
					break;
				}
				NMMPBADCFHK = 9;
				cJMKCEHHMCH.Message = "invalid distance code";
				BOPODEAIEBJ = -3;
				CGKHDGJKOMG.FPGCIJMGFLH = num;
				CGKHDGJKOMG.DBFGKGGCEAI = num2;
				cJMKCEHHMCH.IAPJEIDMGNP = num4;
				cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
				cJMKCEHHMCH.LMIPBGGILEJ = num3;
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
			}
			case 4:
			{
				int aCLJBJJAENG;
				for (aCLJBJJAENG = ACLJBJJAENG; num2 < aCLJBJJAENG; num2 += 8)
				{
					if (num4 != 0)
					{
						BOPODEAIEBJ = 0;
						num4--;
						num |= (cJMKCEHHMCH.PEFOCMDODLD[num3++] & 0xFF) << num2;
						continue;
					}
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				}
				CGIBMHPALCO += num & InternalInflateConstants.PEKJPCOGGBP[aCLJBJJAENG];
				num >>= aCLJBJJAENG;
				num2 -= aCLJBJJAENG;
				NMMPBADCFHK = 5;
				goto case 5;
			}
			case 5:
			{
				int i;
				for (i = num5 - CGIBMHPALCO; i < 0; i += CGKHDGJKOMG.PCLFFOBJJFO)
				{
				}
				while (JCAJDBOMGOM != 0)
				{
					if (num6 == 0)
					{
						if (num5 == CGKHDGJKOMG.PCLFFOBJJFO && CGKHDGJKOMG.IONENIAEDKJ != 0)
						{
							num5 = 0;
							num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
						}
						if (num6 == 0)
						{
							CGKHDGJKOMG.HBFBCHDJEBM = num5;
							BOPODEAIEBJ = CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
							num5 = CGKHDGJKOMG.HBFBCHDJEBM;
							num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
							if (num5 == CGKHDGJKOMG.PCLFFOBJJFO && CGKHDGJKOMG.IONENIAEDKJ != 0)
							{
								num5 = 0;
								num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
							}
							if (num6 == 0)
							{
								CGKHDGJKOMG.FPGCIJMGFLH = num;
								CGKHDGJKOMG.DBFGKGGCEAI = num2;
								cJMKCEHHMCH.IAPJEIDMGNP = num4;
								cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
								cJMKCEHHMCH.LMIPBGGILEJ = num3;
								CGKHDGJKOMG.HBFBCHDJEBM = num5;
								return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
							}
						}
					}
					CGKHDGJKOMG.window[num5++] = CGKHDGJKOMG.window[i++];
					num6--;
					if (i == CGKHDGJKOMG.PCLFFOBJJFO)
					{
						i = 0;
					}
					JCAJDBOMGOM--;
				}
				NMMPBADCFHK = 0;
				break;
			}
			case 6:
				if (num6 == 0)
				{
					if (num5 == CGKHDGJKOMG.PCLFFOBJJFO && CGKHDGJKOMG.IONENIAEDKJ != 0)
					{
						num5 = 0;
						num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
					}
					if (num6 == 0)
					{
						CGKHDGJKOMG.HBFBCHDJEBM = num5;
						BOPODEAIEBJ = CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
						num5 = CGKHDGJKOMG.HBFBCHDJEBM;
						num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
						if (num5 == CGKHDGJKOMG.PCLFFOBJJFO && CGKHDGJKOMG.IONENIAEDKJ != 0)
						{
							num5 = 0;
							num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
						}
						if (num6 == 0)
						{
							CGKHDGJKOMG.FPGCIJMGFLH = num;
							CGKHDGJKOMG.DBFGKGGCEAI = num2;
							cJMKCEHHMCH.IAPJEIDMGNP = num4;
							cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
							cJMKCEHHMCH.LMIPBGGILEJ = num3;
							CGKHDGJKOMG.HBFBCHDJEBM = num5;
							return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
						}
					}
				}
				BOPODEAIEBJ = 0;
				CGKHDGJKOMG.window[num5++] = (byte)DNOLBDAAAFD;
				num6--;
				NMMPBADCFHK = 0;
				break;
			case 7:
				if (num2 > 7)
				{
					num2 -= 8;
					num4++;
					num3--;
				}
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				BOPODEAIEBJ = CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				num5 = CGKHDGJKOMG.HBFBCHDJEBM;
				num6 = ((num5 >= CGKHDGJKOMG.IONENIAEDKJ) ? (CGKHDGJKOMG.PCLFFOBJJFO - num5) : (CGKHDGJKOMG.IONENIAEDKJ - num5 - 1));
				if (CGKHDGJKOMG.IONENIAEDKJ != CGKHDGJKOMG.HBFBCHDJEBM)
				{
					CGKHDGJKOMG.FPGCIJMGFLH = num;
					CGKHDGJKOMG.DBFGKGGCEAI = num2;
					cJMKCEHHMCH.IAPJEIDMGNP = num4;
					cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
					cJMKCEHHMCH.LMIPBGGILEJ = num3;
					CGKHDGJKOMG.HBFBCHDJEBM = num5;
					return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
				}
				NMMPBADCFHK = 8;
				goto case 8;
			case 8:
				BOPODEAIEBJ = 1;
				CGKHDGJKOMG.FPGCIJMGFLH = num;
				CGKHDGJKOMG.DBFGKGGCEAI = num2;
				cJMKCEHHMCH.IAPJEIDMGNP = num4;
				cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
				cJMKCEHHMCH.LMIPBGGILEJ = num3;
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
			case 9:
				BOPODEAIEBJ = -3;
				CGKHDGJKOMG.FPGCIJMGFLH = num;
				CGKHDGJKOMG.DBFGKGGCEAI = num2;
				cJMKCEHHMCH.IAPJEIDMGNP = num4;
				cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
				cJMKCEHHMCH.LMIPBGGILEJ = num3;
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
			default:
				BOPODEAIEBJ = -2;
				CGKHDGJKOMG.FPGCIJMGFLH = num;
				CGKHDGJKOMG.DBFGKGGCEAI = num2;
				cJMKCEHHMCH.IAPJEIDMGNP = num4;
				cJMKCEHHMCH.ALJBBHPGGPA += num3 - cJMKCEHHMCH.LMIPBGGILEJ;
				cJMKCEHHMCH.LMIPBGGILEJ = num3;
				CGKHDGJKOMG.HBFBCHDJEBM = num5;
				return CGKHDGJKOMG.MKPBJGMJPMI(BOPODEAIEBJ);
			}
		}
	}

	internal int HPHGIOKICCK(int GGEJHHHGPKN, int NBHIKILKMED, int[] AEFHBJIMPHM, int HLDNDJKELJE, int[] GICLKGGKJAG, int KMKIMJDIKHC, InflateBlocks JDCCBCNFENK, ZlibCodec LKPCKJOLJDO)
	{
		int lMIPBGGILEJ = LKPCKJOLJDO.LMIPBGGILEJ;
		int num = LKPCKJOLJDO.IAPJEIDMGNP;
		int num2 = JDCCBCNFENK.FPGCIJMGFLH;
		int num3 = JDCCBCNFENK.DBFGKGGCEAI;
		int num4 = JDCCBCNFENK.HBFBCHDJEBM;
		int num5 = ((num4 >= JDCCBCNFENK.IONENIAEDKJ) ? (JDCCBCNFENK.PCLFFOBJJFO - num4) : (JDCCBCNFENK.IONENIAEDKJ - num4 - 1));
		int num6 = InternalInflateConstants.PEKJPCOGGBP[GGEJHHHGPKN];
		int num7 = InternalInflateConstants.PEKJPCOGGBP[NBHIKILKMED];
		int num12;
		while (true)
		{
			if (num3 < 20)
			{
				num--;
				num2 |= (LKPCKJOLJDO.PEFOCMDODLD[lMIPBGGILEJ++] & 0xFF) << num3;
				num3 += 8;
				continue;
			}
			int num8 = num2 & num6;
			int[] array = AEFHBJIMPHM;
			int num9 = HLDNDJKELJE;
			int num10 = (num9 + num8) * 3;
			int num11;
			if ((num11 = array[num10]) == 0)
			{
				num2 >>= array[num10 + 1];
				num3 -= array[num10 + 1];
				JDCCBCNFENK.window[num4++] = (byte)array[num10 + 2];
				num5--;
			}
			else
			{
				while (true)
				{
					num2 >>= array[num10 + 1];
					num3 -= array[num10 + 1];
					if ((num11 & 0x10) != 0)
					{
						num11 &= 0xF;
						num12 = array[num10 + 2] + (num2 & InternalInflateConstants.PEKJPCOGGBP[num11]);
						num2 >>= num11;
						for (num3 -= num11; num3 < 15; num3 += 8)
						{
							num--;
							num2 |= (LKPCKJOLJDO.PEFOCMDODLD[lMIPBGGILEJ++] & 0xFF) << num3;
						}
						num8 = num2 & num7;
						array = GICLKGGKJAG;
						num9 = KMKIMJDIKHC;
						num10 = (num9 + num8) * 3;
						num11 = array[num10];
						while (true)
						{
							num2 >>= array[num10 + 1];
							num3 -= array[num10 + 1];
							if ((num11 & 0x10) != 0)
							{
								break;
							}
							if ((num11 & 0x40) == 0)
							{
								num8 += array[num10 + 2];
								num8 += num2 & InternalInflateConstants.PEKJPCOGGBP[num11];
								num10 = (num9 + num8) * 3;
								num11 = array[num10];
								continue;
							}
							LKPCKJOLJDO.Message = "invalid distance code";
							num12 = LKPCKJOLJDO.IAPJEIDMGNP - num;
							num12 = ((num3 >> 3 >= num12) ? num12 : (num3 >> 3));
							num += num12;
							lMIPBGGILEJ -= num12;
							num3 -= num12 << 3;
							JDCCBCNFENK.FPGCIJMGFLH = num2;
							JDCCBCNFENK.DBFGKGGCEAI = num3;
							LKPCKJOLJDO.IAPJEIDMGNP = num;
							LKPCKJOLJDO.ALJBBHPGGPA += lMIPBGGILEJ - LKPCKJOLJDO.LMIPBGGILEJ;
							LKPCKJOLJDO.LMIPBGGILEJ = lMIPBGGILEJ;
							JDCCBCNFENK.HBFBCHDJEBM = num4;
							return -3;
						}
						for (num11 &= 0xF; num3 < num11; num3 += 8)
						{
							num--;
							num2 |= (LKPCKJOLJDO.PEFOCMDODLD[lMIPBGGILEJ++] & 0xFF) << num3;
						}
						int num13 = array[num10 + 2] + (num2 & InternalInflateConstants.PEKJPCOGGBP[num11]);
						num2 >>= num11;
						num3 -= num11;
						num5 -= num12;
						int num14;
						if (num4 >= num13)
						{
							num14 = num4 - num13;
							if (num4 - num14 > 0 && 2 > num4 - num14)
							{
								JDCCBCNFENK.window[num4++] = JDCCBCNFENK.window[num14++];
								JDCCBCNFENK.window[num4++] = JDCCBCNFENK.window[num14++];
								num12 -= 2;
							}
							else
							{
								Array.Copy(JDCCBCNFENK.window, num14, JDCCBCNFENK.window, num4, 2);
								num4 += 2;
								num14 += 2;
								num12 -= 2;
							}
						}
						else
						{
							num14 = num4 - num13;
							do
							{
								num14 += JDCCBCNFENK.PCLFFOBJJFO;
							}
							while (num14 < 0);
							num11 = JDCCBCNFENK.PCLFFOBJJFO - num14;
							if (num12 > num11)
							{
								num12 -= num11;
								if (num4 - num14 > 0 && num11 > num4 - num14)
								{
									do
									{
										JDCCBCNFENK.window[num4++] = JDCCBCNFENK.window[num14++];
									}
									while (--num11 != 0);
								}
								else
								{
									Array.Copy(JDCCBCNFENK.window, num14, JDCCBCNFENK.window, num4, num11);
									num4 += num11;
									num14 += num11;
									num11 = 0;
								}
								num14 = 0;
							}
						}
						if (num4 - num14 > 0 && num12 > num4 - num14)
						{
							do
							{
								JDCCBCNFENK.window[num4++] = JDCCBCNFENK.window[num14++];
							}
							while (--num12 != 0);
							break;
						}
						Array.Copy(JDCCBCNFENK.window, num14, JDCCBCNFENK.window, num4, num12);
						num4 += num12;
						num14 += num12;
						num12 = 0;
						break;
					}
					if ((num11 & 0x40) == 0)
					{
						num8 += array[num10 + 2];
						num8 += num2 & InternalInflateConstants.PEKJPCOGGBP[num11];
						num10 = (num9 + num8) * 3;
						if ((num11 = array[num10]) == 0)
						{
							num2 >>= array[num10 + 1];
							num3 -= array[num10 + 1];
							JDCCBCNFENK.window[num4++] = (byte)array[num10 + 2];
							num5--;
							break;
						}
						continue;
					}
					if ((num11 & 0x20) != 0)
					{
						num12 = LKPCKJOLJDO.IAPJEIDMGNP - num;
						num12 = ((num3 >> 3 >= num12) ? num12 : (num3 >> 3));
						num += num12;
						lMIPBGGILEJ -= num12;
						num3 -= num12 << 3;
						JDCCBCNFENK.FPGCIJMGFLH = num2;
						JDCCBCNFENK.DBFGKGGCEAI = num3;
						LKPCKJOLJDO.IAPJEIDMGNP = num;
						LKPCKJOLJDO.ALJBBHPGGPA += lMIPBGGILEJ - LKPCKJOLJDO.LMIPBGGILEJ;
						LKPCKJOLJDO.LMIPBGGILEJ = lMIPBGGILEJ;
						JDCCBCNFENK.HBFBCHDJEBM = num4;
						return 1;
					}
					LKPCKJOLJDO.Message = "invalid literal/length code";
					num12 = LKPCKJOLJDO.IAPJEIDMGNP - num;
					num12 = ((num3 >> 3 >= num12) ? num12 : (num3 >> 3));
					num += num12;
					lMIPBGGILEJ -= num12;
					num3 -= num12 << 3;
					JDCCBCNFENK.FPGCIJMGFLH = num2;
					JDCCBCNFENK.DBFGKGGCEAI = num3;
					LKPCKJOLJDO.IAPJEIDMGNP = num;
					LKPCKJOLJDO.ALJBBHPGGPA += lMIPBGGILEJ - LKPCKJOLJDO.LMIPBGGILEJ;
					LKPCKJOLJDO.LMIPBGGILEJ = lMIPBGGILEJ;
					JDCCBCNFENK.HBFBCHDJEBM = num4;
					return -3;
				}
			}
			if (num5 < 258 || num < 10)
			{
				break;
			}
		}
		num12 = LKPCKJOLJDO.IAPJEIDMGNP - num;
		num12 = ((num3 >> 3 >= num12) ? num12 : (num3 >> 3));
		num += num12;
		lMIPBGGILEJ -= num12;
		num3 -= num12 << 3;
		JDCCBCNFENK.FPGCIJMGFLH = num2;
		JDCCBCNFENK.DBFGKGGCEAI = num3;
		LKPCKJOLJDO.IAPJEIDMGNP = num;
		LKPCKJOLJDO.ALJBBHPGGPA += lMIPBGGILEJ - LKPCKJOLJDO.LMIPBGGILEJ;
		LKPCKJOLJDO.LMIPBGGILEJ = lMIPBGGILEJ;
		JDCCBCNFENK.HBFBCHDJEBM = num4;
		return 0;
	}
}
