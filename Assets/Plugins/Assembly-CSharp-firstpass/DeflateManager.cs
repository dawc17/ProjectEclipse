using System;

internal sealed class DeflateManager
{
	internal delegate HHLELELECLA LJKNIDJPGBN(AFJHGKAEJPG NGBJDNFAPKC);

	internal class CLOGLEGLGGF
	{
		internal int MLJCJEAACIO;

		internal int PIHEHJPHNME;

		internal int NNLJOHLDEOE;

		internal int DEPMLENGCBF;

		internal DPEICFAJMJH BHMMEEECCKO;

		private static readonly CLOGLEGLGGF[] Table;

		private CLOGLEGLGGF(int IAOAPAPPECC, int DEHAFDGLEOP, int AFEAHPCPHHI, int ENOGJBAJMCJ, DPEICFAJMJH CENOEIJNIAG)
		{
			MLJCJEAACIO = IAOAPAPPECC;
			PIHEHJPHNME = DEHAFDGLEOP;
			NNLJOHLDEOE = AFEAHPCPHHI;
			DEPMLENGCBF = ENOGJBAJMCJ;
			BHMMEEECCKO = CENOEIJNIAG;
		}

		static CLOGLEGLGGF()
		{
			Table = new CLOGLEGLGGF[10]
			{
				new CLOGLEGLGGF(0, 0, 0, 0, DPEICFAJMJH.Store),
				new CLOGLEGLGGF(4, 4, 8, 4, DPEICFAJMJH.Fast),
				new CLOGLEGLGGF(4, 5, 16, 8, DPEICFAJMJH.Fast),
				new CLOGLEGLGGF(4, 6, 32, 32, DPEICFAJMJH.Fast),
				new CLOGLEGLGGF(4, 4, 16, 16, DPEICFAJMJH.Slow),
				new CLOGLEGLGGF(8, 16, 32, 32, DPEICFAJMJH.Slow),
				new CLOGLEGLGGF(8, 16, 128, 128, DPEICFAJMJH.Slow),
				new CLOGLEGLGGF(8, 32, 128, 256, DPEICFAJMJH.Slow),
				new CLOGLEGLGGF(32, 128, 258, 1024, DPEICFAJMJH.Slow),
				new CLOGLEGLGGF(32, 258, 258, 4096, DPEICFAJMJH.Slow)
			};
		}

		public static CLOGLEGLGGF CJKIADKJMIK(NKFKKGNBHDK GNLOCMLBNHF)
		{
			return Table[(int)GNLOCMLBNHF];
		}
	}

	private static readonly int JIHDOLCCHBO = 9;

	private static readonly int COEHLNBICJF = 8;

	private LJKNIDJPGBN LOCMPAAEKFN;

	private static readonly string[] _ErrorMessage = new string[10]
	{
		"need dictionary",
		"stream end",
		string.Empty,
		"file error",
		"stream error",
		"data error",
		"insufficient memory",
		"buffer error",
		"incompatible version",
		string.Empty
	};

	private static readonly int LEDINGHODAJ = 32;

	private static readonly int GAJKKNHFMCI = 42;

	private static readonly int HPOMCEOMBPG = 113;

	private static readonly int OJECJENOCNK = 666;

	private static readonly int GLGHJDNCDON = 8;

	private static readonly int DMBNHNOAIBN = 0;

	private static readonly int DDEEFLKAGEG = 1;

	private static readonly int IPKJNFEPLEG = 2;

	private static readonly int LKEOCFIJMME = 0;

	private static readonly int BCJEIPADOCH = 1;

	private static readonly int JAOKIMHCBCG = 2;

	private static readonly int Buf_size = 16;

	private static readonly int JNGNJJIBHFK = 3;

	private static readonly int BBCACPAGMOF = 258;

	private static readonly int BLNLIJEEHHP = BBCACPAGMOF + JNGNJJIBHFK + 1;

	private static readonly int HEAP_SIZE = 2 * InternalConstants.IHNFCKICBAG + 1;

	private static readonly int ALFDCNDGFEG = 256;

	internal ZlibCodec CJMKCEHHMCH;

	internal int status;

	internal byte[] BMPCIFFMMPP;

	internal int GBKNGFEBIOL;

	internal int CCDNPCJKGGK;

	internal sbyte data_type;

	internal int KNACOPCPMJK;

	internal int NNHBBKGGHJF;

	internal int EEKHLFIPIJG;

	internal int BHAHBAHNDHM;

	internal byte[] window;

	internal int ABOMKGJLCHA;

	internal short[] JIABHEAAKCH;

	internal short[] POLFAHOJJCN;

	internal int FDKJFJENANA;

	internal int ELHHCCOMLNM;

	internal int OJPGOFNBHFP;

	internal int MFIODCHIHLF;

	internal int FIPCMJOCFCD;

	internal int ALPONFEFJAK;

	private CLOGLEGLGGF IBBOLEEKAOM;

	internal int GONJNELNHCH;

	internal int HPLACHFGPFJ;

	internal int MIJFDHGBDAD;

	internal int EMFNCOCHGEA;

	internal int NDCEAFEGAFK;

	internal int BINANKICGPN;

	internal int DANPHDEJIFM;

	internal NKFKKGNBHDK CPOCBHJGICD;

	internal DDGGLIIKFPL IDOIMLPCFNP;

	internal short[] MJIMJGKCKHA;

	internal short[] ANHCBLNDMPO;

	internal short[] GBODMCOAPOF;

	internal ZTree DLONKBOGAOD = new ZTree();

	internal ZTree HLPIEBBMLLB = new ZTree();

	internal ZTree AGFCLLKFOBH = new ZTree();

	internal short[] OOJOJFEKPEL = new short[InternalConstants.LHOJMFFOHIM + 1];

	internal int[] heap = new int[2 * InternalConstants.IHNFCKICBAG + 1];

	internal int ICNCOCBABJG;

	internal int ENPMDMPEKOM;

	internal sbyte[] depth = new sbyte[2 * InternalConstants.IHNFCKICBAG + 1];

	internal int HMLFNGFEPHM;

	internal int BENKHMFEIID;

	internal int OPMAFAHGDNH;

	internal int KDFEEGCLJHL;

	internal int LJEPPNBNHPH;

	internal int KJFNFHFAFGI;

	internal int ICCBNNGJOCE;

	internal int JPGEMNFECEL;

	internal short bi_buf;

	internal int PKMPBCCLNEM;

	private bool GIIABEPDGLD;

	private bool LOICEAGHLIO = true;

	internal bool EANNKDFMMIJ
	{
		get
		{
			return GFOKPNKCOOP();
		}
		set
		{
			NGEBPALKODO(value);
		}
	}

	internal DeflateManager()
	{
		MJIMJGKCKHA = new short[HEAP_SIZE * 2];
		ANHCBLNDMPO = new short[(2 * InternalConstants.JBINAIJBEPN + 1) * 2];
		GBODMCOAPOF = new short[(2 * InternalConstants.NLIKGOGMFCH + 1) * 2];
	}

	private void FPLKPJJFJLL()
	{
		ABOMKGJLCHA = 2 * NNHBBKGGHJF;
		Array.Clear(POLFAHOJJCN, 0, ELHHCCOMLNM);
		IBBOLEEKAOM = CLOGLEGLGGF.CJKIADKJMIK(CPOCBHJGICD);
		JADFPOKKPEP();
		EMFNCOCHGEA = 0;
		ALPONFEFJAK = 0;
		BINANKICGPN = 0;
		GONJNELNHCH = (DANPHDEJIFM = JNGNJJIBHFK - 1);
		MIJFDHGBDAD = 0;
		FDKJFJENANA = 0;
	}

	private void AMOLIEBKCDB()
	{
		DLONKBOGAOD.dyn_tree = MJIMJGKCKHA;
		DLONKBOGAOD.ENBONDDMEKE = StaticTree.NIBOLIDCANA;
		HLPIEBBMLLB.dyn_tree = ANHCBLNDMPO;
		HLPIEBBMLLB.ENBONDDMEKE = StaticTree.Distances;
		AGFCLLKFOBH.dyn_tree = GBODMCOAPOF;
		AGFCLLKFOBH.ENBONDDMEKE = StaticTree.APFLNEFDCBN;
		bi_buf = 0;
		PKMPBCCLNEM = 0;
		JPGEMNFECEL = 8;
		NMHIEGAGEEP();
	}

	internal void NMHIEGAGEEP()
	{
		for (int i = 0; i < InternalConstants.IHNFCKICBAG; i++)
		{
			MJIMJGKCKHA[i * 2] = 0;
		}
		for (int j = 0; j < InternalConstants.JBINAIJBEPN; j++)
		{
			ANHCBLNDMPO[j * 2] = 0;
		}
		for (int k = 0; k < InternalConstants.NLIKGOGMFCH; k++)
		{
			GBODMCOAPOF[k * 2] = 0;
		}
		MJIMJGKCKHA[ALFDCNDGFEG * 2] = 1;
		LJEPPNBNHPH = (KJFNFHFAFGI = 0);
		OPMAFAHGDNH = (ICCBNNGJOCE = 0);
	}

	internal void GAHEAKPOCIJ(short[] EDBPBGAMMDO, int KJBMNAEJIHG)
	{
		int num = heap[KJBMNAEJIHG];
		for (int num2 = KJBMNAEJIHG << 1; num2 <= ICNCOCBABJG; num2 <<= 1)
		{
			if (num2 < ICNCOCBABJG && ELMMNHMHIEC(EDBPBGAMMDO, heap[num2 + 1], heap[num2], depth))
			{
				num2++;
			}
			if (ELMMNHMHIEC(EDBPBGAMMDO, num, heap[num2], depth))
			{
				break;
			}
			heap[KJBMNAEJIHG] = heap[num2];
			KJBMNAEJIHG = num2;
		}
		heap[KJBMNAEJIHG] = num;
	}

	internal static bool ELMMNHMHIEC(short[] EDBPBGAMMDO, int HDKKKCDKFEE, int OFBGCEPCNOL, sbyte[] depth)
	{
		short num = EDBPBGAMMDO[HDKKKCDKFEE * 2];
		short num2 = EDBPBGAMMDO[OFBGCEPCNOL * 2];
		return num < num2 || (num == num2 && depth[HDKKKCDKFEE] <= depth[OFBGCEPCNOL]);
	}

	internal void NKBPOCPNKDG(short[] EDBPBGAMMDO, int max_code)
	{
		int num = -1;
		int num2 = EDBPBGAMMDO[1];
		int num3 = 0;
		int num4 = 7;
		int num5 = 4;
		if (num2 == 0)
		{
			num4 = 138;
			num5 = 3;
		}
		EDBPBGAMMDO[(max_code + 1) * 2 + 1] = short.MaxValue;
		for (int i = 0; i <= max_code; i++)
		{
			int num6 = num2;
			num2 = EDBPBGAMMDO[(i + 1) * 2 + 1];
			if (++num3 < num4 && num6 == num2)
			{
				continue;
			}
			if (num3 < num5)
			{
				GBODMCOAPOF[num6 * 2] = (short)(GBODMCOAPOF[num6 * 2] + num3);
			}
			else if (num6 != 0)
			{
				if (num6 != num)
				{
					GBODMCOAPOF[num6 * 2]++;
				}
				GBODMCOAPOF[InternalConstants.OGAGDEGEKHL * 2]++;
			}
			else if (num3 <= 10)
			{
				GBODMCOAPOF[InternalConstants.MHAPHGGOFDL * 2]++;
			}
			else
			{
				GBODMCOAPOF[InternalConstants.FECIOJMMLFI * 2]++;
			}
			num3 = 0;
			num = num6;
			if (num2 == 0)
			{
				num4 = 138;
				num5 = 3;
			}
			else if (num6 == num2)
			{
				num4 = 6;
				num5 = 3;
			}
			else
			{
				num4 = 7;
				num5 = 4;
			}
		}
	}

	internal int ONFIHECFIFN()
	{
		NKBPOCPNKDG(MJIMJGKCKHA, DLONKBOGAOD.max_code);
		NKBPOCPNKDG(ANHCBLNDMPO, HLPIEBBMLLB.max_code);
		AGFCLLKFOBH.MKOOHAEKKNO(this);
		int num = InternalConstants.NLIKGOGMFCH - 1;
		while (num >= 3 && GBODMCOAPOF[ZTree.DKMPFGEFBJJ[num] * 2 + 1] == 0)
		{
			num--;
		}
		LJEPPNBNHPH += 3 * (num + 1) + 5 + 5 + 4;
		return num;
	}

	internal void send_all_trees(int EHFOCDNBFHI, int GNCJDCINAFE, int LPLODDOBOJK)
	{
		send_bits(EHFOCDNBFHI - 257, 5);
		send_bits(GNCJDCINAFE - 1, 5);
		send_bits(LPLODDOBOJK - 4, 4);
		for (int i = 0; i < LPLODDOBOJK; i++)
		{
			send_bits(GBODMCOAPOF[ZTree.DKMPFGEFBJJ[i] * 2 + 1], 3);
		}
		PIOOLDBCMKP(MJIMJGKCKHA, EHFOCDNBFHI - 1);
		PIOOLDBCMKP(ANHCBLNDMPO, GNCJDCINAFE - 1);
	}

	internal void PIOOLDBCMKP(short[] EDBPBGAMMDO, int max_code)
	{
		int num = -1;
		int num2 = EDBPBGAMMDO[1];
		int num3 = 0;
		int num4 = 7;
		int num5 = 4;
		if (num2 == 0)
		{
			num4 = 138;
			num5 = 3;
		}
		for (int i = 0; i <= max_code; i++)
		{
			int num6 = num2;
			num2 = EDBPBGAMMDO[(i + 1) * 2 + 1];
			if (++num3 < num4 && num6 == num2)
			{
				continue;
			}
			if (num3 < num5)
			{
				do
				{
					send_code(num6, GBODMCOAPOF);
				}
				while (--num3 != 0);
			}
			else if (num6 != 0)
			{
				if (num6 != num)
				{
					send_code(num6, GBODMCOAPOF);
					num3--;
				}
				send_code(InternalConstants.OGAGDEGEKHL, GBODMCOAPOF);
				send_bits(num3 - 3, 2);
			}
			else if (num3 <= 10)
			{
				send_code(InternalConstants.MHAPHGGOFDL, GBODMCOAPOF);
				send_bits(num3 - 3, 3);
			}
			else
			{
				send_code(InternalConstants.FECIOJMMLFI, GBODMCOAPOF);
				send_bits(num3 - 11, 7);
			}
			num3 = 0;
			num = num6;
			if (num2 == 0)
			{
				num4 = 138;
				num5 = 3;
			}
			else if (num6 == num2)
			{
				num4 = 6;
				num5 = 3;
			}
			else
			{
				num4 = 7;
				num5 = 4;
			}
		}
	}

	private void put_bytes(byte[] PIIEECCHMAC, int ILENLCMAMBH, int JCAJDBOMGOM)
	{
		Array.Copy(PIIEECCHMAC, ILENLCMAMBH, BMPCIFFMMPP, CCDNPCJKGGK, JCAJDBOMGOM);
		CCDNPCJKGGK += JCAJDBOMGOM;
	}

	internal void send_code(int ILHDJDNPFKH, short[] EDBPBGAMMDO)
	{
		int num = ILHDJDNPFKH * 2;
		send_bits(EDBPBGAMMDO[num] & 0xFFFF, EDBPBGAMMDO[num + 1] & 0xFFFF);
	}

	internal void send_bits(int value, int BDBOAEGELMC)
	{
		if (PKMPBCCLNEM > Buf_size - BDBOAEGELMC)
		{
			bi_buf |= (short)((value << PKMPBCCLNEM) & 0xFFFF);
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)bi_buf;
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(bi_buf >> 8);
			bi_buf = (short)((uint)value >> Buf_size - PKMPBCCLNEM);
			PKMPBCCLNEM += BDBOAEGELMC - Buf_size;
		}
		else
		{
			bi_buf |= (short)((value << PKMPBCCLNEM) & 0xFFFF);
			PKMPBCCLNEM += BDBOAEGELMC;
		}
	}

	internal void KMHCDPIKPGF()
	{
		send_bits(DDEEFLKAGEG << 1, 3);
		send_code(ALFDCNDGFEG, StaticTree.HMAFLCHFIGL);
		BHANKOKACDM();
		if (1 + JPGEMNFECEL + 10 - PKMPBCCLNEM < 9)
		{
			send_bits(DDEEFLKAGEG << 1, 3);
			send_code(ALFDCNDGFEG, StaticTree.HMAFLCHFIGL);
			BHANKOKACDM();
		}
		JPGEMNFECEL = 7;
	}

	internal bool _tr_tally(int CGIBMHPALCO, int LMHBHHENKHG)
	{
		BMPCIFFMMPP[KDFEEGCLJHL + OPMAFAHGDNH * 2] = (byte)((uint)CGIBMHPALCO >> 8);
		BMPCIFFMMPP[KDFEEGCLJHL + OPMAFAHGDNH * 2 + 1] = (byte)CGIBMHPALCO;
		BMPCIFFMMPP[HMLFNGFEPHM + OPMAFAHGDNH] = (byte)LMHBHHENKHG;
		OPMAFAHGDNH++;
		if (CGIBMHPALCO == 0)
		{
			MJIMJGKCKHA[LMHBHHENKHG * 2]++;
		}
		else
		{
			ICCBNNGJOCE++;
			CGIBMHPALCO--;
			MJIMJGKCKHA[(ZTree.LACJMDGEMAL[LMHBHHENKHG] + InternalConstants.ICKCLCDCBAH + 1) * 2]++;
			ANHCBLNDMPO[ZTree.JKKOHECEGAP(CGIBMHPALCO) * 2]++;
		}
		if ((OPMAFAHGDNH & 0x1FFF) == 0 && CPOCBHJGICD > NKFKKGNBHDK.Level2)
		{
			int num = OPMAFAHGDNH << 3;
			int num2 = EMFNCOCHGEA - ALPONFEFJAK;
			for (int i = 0; i < InternalConstants.JBINAIJBEPN; i++)
			{
				num = (int)(num + ANHCBLNDMPO[i * 2] * (5L + (long)ZTree.BHDPMJMOHMI[i]));
			}
			num >>= 3;
			if (ICCBNNGJOCE < OPMAFAHGDNH / 2 && num < num2 / 2)
			{
				return true;
			}
		}
		return OPMAFAHGDNH == BENKHMFEIID - 1 || OPMAFAHGDNH == BENKHMFEIID;
	}

	internal void send_compressed_block(short[] JKLBBEFFMID, short[] JDFLHKFAEOD)
	{
		int num = 0;
		if (OPMAFAHGDNH != 0)
		{
			do
			{
				int num2 = KDFEEGCLJHL + num * 2;
				int num3 = ((BMPCIFFMMPP[num2] << 8) & 0xFF00) | (BMPCIFFMMPP[num2 + 1] & 0xFF);
				int num4 = BMPCIFFMMPP[HMLFNGFEPHM + num] & 0xFF;
				num++;
				if (num3 == 0)
				{
					send_code(num4, JKLBBEFFMID);
					continue;
				}
				int num5 = ZTree.LACJMDGEMAL[num4];
				send_code(num5 + InternalConstants.ICKCLCDCBAH + 1, JKLBBEFFMID);
				int num6 = ZTree.ECCFNFEKKCC[num5];
				if (num6 != 0)
				{
					num4 -= ZTree.FCLEBOKLJIK[num5];
					send_bits(num4, num6);
				}
				num3--;
				num5 = ZTree.JKKOHECEGAP(num3);
				send_code(num5, JDFLHKFAEOD);
				num6 = ZTree.BHDPMJMOHMI[num5];
				if (num6 != 0)
				{
					num3 -= ZTree.CJHEICAPLNM[num5];
					send_bits(num3, num6);
				}
			}
			while (num < OPMAFAHGDNH);
		}
		send_code(ALFDCNDGFEG, JKLBBEFFMID);
		JPGEMNFECEL = JKLBBEFFMID[ALFDCNDGFEG * 2 + 1];
	}

	internal void DCBNPKMFGFH()
	{
		int i = 0;
		int num = 0;
		int num2 = 0;
		for (; i < 7; i++)
		{
			num2 += MJIMJGKCKHA[i * 2];
		}
		for (; i < 128; i++)
		{
			num += MJIMJGKCKHA[i * 2];
		}
		for (; i < InternalConstants.ICKCLCDCBAH; i++)
		{
			num2 += MJIMJGKCKHA[i * 2];
		}
		data_type = (sbyte)((num2 <= num >> 2) ? BCJEIPADOCH : LKEOCFIJMME);
	}

	internal void BHANKOKACDM()
	{
		if (PKMPBCCLNEM == 16)
		{
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)bi_buf;
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(bi_buf >> 8);
			bi_buf = 0;
			PKMPBCCLNEM = 0;
		}
		else if (PKMPBCCLNEM >= 8)
		{
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)bi_buf;
			bi_buf >>= 8;
			PKMPBCCLNEM -= 8;
		}
	}

	internal void HKGMDLCPKKB()
	{
		if (PKMPBCCLNEM > 8)
		{
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)bi_buf;
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(bi_buf >> 8);
		}
		else if (PKMPBCCLNEM > 0)
		{
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)bi_buf;
		}
		bi_buf = 0;
		PKMPBCCLNEM = 0;
	}

	internal void FOPHLLOHAFK(int HLDLIFPJMOA, int JCAJDBOMGOM, bool HHAAFADDOJB)
	{
		HKGMDLCPKKB();
		JPGEMNFECEL = 8;
		if (HHAAFADDOJB)
		{
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)JCAJDBOMGOM;
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(JCAJDBOMGOM >> 8);
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(~JCAJDBOMGOM);
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(~JCAJDBOMGOM >> 8);
		}
		put_bytes(window, HLDLIFPJMOA, JCAJDBOMGOM);
	}

	internal void BCNCGCMIELN(bool MMCDBIAEFHO)
	{
		ANMDGAIIHGA((ALPONFEFJAK < 0) ? (-1) : ALPONFEFJAK, EMFNCOCHGEA - ALPONFEFJAK, MMCDBIAEFHO);
		ALPONFEFJAK = EMFNCOCHGEA;
		CJMKCEHHMCH.CHAPNKCGONG();
	}

	internal HHLELELECLA LOOAHPKFKJE(AFJHGKAEJPG NGBJDNFAPKC)
	{
		int num = 65535;
		if (num > BMPCIFFMMPP.Length - 5)
		{
			num = BMPCIFFMMPP.Length - 5;
		}
		while (true)
		{
			if (BINANKICGPN <= 1)
			{
				KCKPMHBIPJM();
				if (BINANKICGPN == 0 && NGBJDNFAPKC == AFJHGKAEJPG.None)
				{
					return HHLELELECLA.NeedMore;
				}
				if (BINANKICGPN == 0)
				{
					break;
				}
			}
			EMFNCOCHGEA += BINANKICGPN;
			BINANKICGPN = 0;
			int num2 = ALPONFEFJAK + num;
			if (EMFNCOCHGEA == 0 || EMFNCOCHGEA >= num2)
			{
				BINANKICGPN = EMFNCOCHGEA - num2;
				EMFNCOCHGEA = num2;
				BCNCGCMIELN(false);
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					return HHLELELECLA.NeedMore;
				}
			}
			if (EMFNCOCHGEA - ALPONFEFJAK >= NNHBBKGGHJF - BLNLIJEEHHP)
			{
				BCNCGCMIELN(false);
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					return HHLELELECLA.NeedMore;
				}
			}
		}
		BCNCGCMIELN(NGBJDNFAPKC == AFJHGKAEJPG.Finish);
		if (CJMKCEHHMCH.NBNGINIIKNA == 0)
		{
			return (NGBJDNFAPKC == AFJHGKAEJPG.Finish) ? HHLELELECLA.FinishStarted : HHLELELECLA.NeedMore;
		}
		return (NGBJDNFAPKC != AFJHGKAEJPG.Finish) ? HHLELELECLA.BlockDone : HHLELELECLA.FinishDone;
	}

	internal void OPBEHCNBEPA(int HLDLIFPJMOA, int PNFGGDAMONJ, bool MMCDBIAEFHO)
	{
		send_bits((DMBNHNOAIBN << 1) + (MMCDBIAEFHO ? 1 : 0), 3);
		FOPHLLOHAFK(HLDLIFPJMOA, PNFGGDAMONJ, true);
	}

	internal void ANMDGAIIHGA(int HLDLIFPJMOA, int PNFGGDAMONJ, bool MMCDBIAEFHO)
	{
		int num = 0;
		int num2;
		int num3;
		if (CPOCBHJGICD > NKFKKGNBHDK.None)
		{
			if (data_type == JAOKIMHCBCG)
			{
				DCBNPKMFGFH();
			}
			DLONKBOGAOD.MKOOHAEKKNO(this);
			HLPIEBBMLLB.MKOOHAEKKNO(this);
			num = ONFIHECFIFN();
			num2 = LJEPPNBNHPH + 3 + 7 >> 3;
			num3 = KJFNFHFAFGI + 3 + 7 >> 3;
			if (num3 <= num2)
			{
				num2 = num3;
			}
		}
		else
		{
			num2 = (num3 = PNFGGDAMONJ + 5);
		}
		if (PNFGGDAMONJ + 4 <= num2 && HLDLIFPJMOA != -1)
		{
			OPBEHCNBEPA(HLDLIFPJMOA, PNFGGDAMONJ, MMCDBIAEFHO);
		}
		else if (num3 == num2)
		{
			send_bits((DDEEFLKAGEG << 1) + (MMCDBIAEFHO ? 1 : 0), 3);
			send_compressed_block(StaticTree.HMAFLCHFIGL, StaticTree.DEDBBACIMLA);
		}
		else
		{
			send_bits((IPKJNFEPLEG << 1) + (MMCDBIAEFHO ? 1 : 0), 3);
			send_all_trees(DLONKBOGAOD.max_code + 1, HLPIEBBMLLB.max_code + 1, num + 1);
			send_compressed_block(MJIMJGKCKHA, ANHCBLNDMPO);
		}
		NMHIEGAGEEP();
		if (MMCDBIAEFHO)
		{
			HKGMDLCPKKB();
		}
	}

	private void KCKPMHBIPJM()
	{
		do
		{
			int num = ABOMKGJLCHA - BINANKICGPN - EMFNCOCHGEA;
			int num2;
			if (num == 0 && EMFNCOCHGEA == 0 && BINANKICGPN == 0)
			{
				num = NNHBBKGGHJF;
			}
			else if (num == -1)
			{
				num--;
			}
			else if (EMFNCOCHGEA >= NNHBBKGGHJF + NNHBBKGGHJF - BLNLIJEEHHP)
			{
				Array.Copy(window, NNHBBKGGHJF, window, 0, NNHBBKGGHJF);
				NDCEAFEGAFK -= NNHBBKGGHJF;
				EMFNCOCHGEA -= NNHBBKGGHJF;
				ALPONFEFJAK -= NNHBBKGGHJF;
				num2 = ELHHCCOMLNM;
				int num3 = num2;
				do
				{
					int num4 = POLFAHOJJCN[--num3] & 0xFFFF;
					POLFAHOJJCN[num3] = (short)((num4 >= NNHBBKGGHJF) ? (num4 - NNHBBKGGHJF) : 0);
				}
				while (--num2 != 0);
				num2 = NNHBBKGGHJF;
				num3 = num2;
				do
				{
					int num4 = JIABHEAAKCH[--num3] & 0xFFFF;
					JIABHEAAKCH[num3] = (short)((num4 >= NNHBBKGGHJF) ? (num4 - NNHBBKGGHJF) : 0);
				}
				while (--num2 != 0);
				num += NNHBBKGGHJF;
			}
			if (CJMKCEHHMCH.IAPJEIDMGNP == 0)
			{
				break;
			}
			num2 = CJMKCEHHMCH.read_buf(window, EMFNCOCHGEA + BINANKICGPN, num);
			BINANKICGPN += num2;
			if (BINANKICGPN >= JNGNJJIBHFK)
			{
				FDKJFJENANA = window[EMFNCOCHGEA] & 0xFF;
				FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + 1] & 0xFF)) & MFIODCHIHLF;
			}
		}
		while (BINANKICGPN < BLNLIJEEHHP && CJMKCEHHMCH.IAPJEIDMGNP != 0);
	}

	internal HHLELELECLA CGOFCHPJIGC(AFJHGKAEJPG NGBJDNFAPKC)
	{
		int num = 0;
		while (true)
		{
			if (BINANKICGPN < BLNLIJEEHHP)
			{
				KCKPMHBIPJM();
				if (BINANKICGPN < BLNLIJEEHHP && NGBJDNFAPKC == AFJHGKAEJPG.None)
				{
					return HHLELELECLA.NeedMore;
				}
				if (BINANKICGPN == 0)
				{
					break;
				}
			}
			if (BINANKICGPN >= JNGNJJIBHFK)
			{
				FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + (JNGNJJIBHFK - 1)] & 0xFF)) & MFIODCHIHLF;
				num = POLFAHOJJCN[FDKJFJENANA] & 0xFFFF;
				JIABHEAAKCH[EMFNCOCHGEA & BHAHBAHNDHM] = POLFAHOJJCN[FDKJFJENANA];
				POLFAHOJJCN[FDKJFJENANA] = (short)EMFNCOCHGEA;
			}
			if ((long)num != 0 && ((EMFNCOCHGEA - num) & 0xFFFF) <= NNHBBKGGHJF - BLNLIJEEHHP && IDOIMLPCFNP != DDGGLIIKFPL.HuffmanOnly)
			{
				GONJNELNHCH = longest_match(num);
			}
			bool flag;
			if (GONJNELNHCH >= JNGNJJIBHFK)
			{
				flag = _tr_tally(EMFNCOCHGEA - NDCEAFEGAFK, GONJNELNHCH - JNGNJJIBHFK);
				BINANKICGPN -= GONJNELNHCH;
				if (GONJNELNHCH <= IBBOLEEKAOM.PIHEHJPHNME && BINANKICGPN >= JNGNJJIBHFK)
				{
					GONJNELNHCH--;
					do
					{
						EMFNCOCHGEA++;
						FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + (JNGNJJIBHFK - 1)] & 0xFF)) & MFIODCHIHLF;
						num = POLFAHOJJCN[FDKJFJENANA] & 0xFFFF;
						JIABHEAAKCH[EMFNCOCHGEA & BHAHBAHNDHM] = POLFAHOJJCN[FDKJFJENANA];
						POLFAHOJJCN[FDKJFJENANA] = (short)EMFNCOCHGEA;
					}
					while (--GONJNELNHCH != 0);
					EMFNCOCHGEA++;
				}
				else
				{
					EMFNCOCHGEA += GONJNELNHCH;
					GONJNELNHCH = 0;
					FDKJFJENANA = window[EMFNCOCHGEA] & 0xFF;
					FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + 1] & 0xFF)) & MFIODCHIHLF;
				}
			}
			else
			{
				flag = _tr_tally(0, window[EMFNCOCHGEA] & 0xFF);
				BINANKICGPN--;
				EMFNCOCHGEA++;
			}
			if (flag)
			{
				BCNCGCMIELN(false);
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					return HHLELELECLA.NeedMore;
				}
			}
		}
		BCNCGCMIELN(NGBJDNFAPKC == AFJHGKAEJPG.Finish);
		if (CJMKCEHHMCH.NBNGINIIKNA == 0)
		{
			if (NGBJDNFAPKC == AFJHGKAEJPG.Finish)
			{
				return HHLELELECLA.FinishStarted;
			}
			return HHLELELECLA.NeedMore;
		}
		return (NGBJDNFAPKC != AFJHGKAEJPG.Finish) ? HHLELELECLA.BlockDone : HHLELELECLA.FinishDone;
	}

	internal HHLELELECLA JAIJANNMDCF(AFJHGKAEJPG NGBJDNFAPKC)
	{
		int num = 0;
		while (true)
		{
			if (BINANKICGPN < BLNLIJEEHHP)
			{
				KCKPMHBIPJM();
				if (BINANKICGPN < BLNLIJEEHHP && NGBJDNFAPKC == AFJHGKAEJPG.None)
				{
					return HHLELELECLA.NeedMore;
				}
				if (BINANKICGPN == 0)
				{
					break;
				}
			}
			if (BINANKICGPN >= JNGNJJIBHFK)
			{
				FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + (JNGNJJIBHFK - 1)] & 0xFF)) & MFIODCHIHLF;
				num = POLFAHOJJCN[FDKJFJENANA] & 0xFFFF;
				JIABHEAAKCH[EMFNCOCHGEA & BHAHBAHNDHM] = POLFAHOJJCN[FDKJFJENANA];
				POLFAHOJJCN[FDKJFJENANA] = (short)EMFNCOCHGEA;
			}
			DANPHDEJIFM = GONJNELNHCH;
			HPLACHFGPFJ = NDCEAFEGAFK;
			GONJNELNHCH = JNGNJJIBHFK - 1;
			if (num != 0 && DANPHDEJIFM < IBBOLEEKAOM.PIHEHJPHNME && ((EMFNCOCHGEA - num) & 0xFFFF) <= NNHBBKGGHJF - BLNLIJEEHHP)
			{
				if (IDOIMLPCFNP != DDGGLIIKFPL.HuffmanOnly)
				{
					GONJNELNHCH = longest_match(num);
				}
				if (GONJNELNHCH <= 5 && (IDOIMLPCFNP == DDGGLIIKFPL.Filtered || (GONJNELNHCH == JNGNJJIBHFK && EMFNCOCHGEA - NDCEAFEGAFK > 4096)))
				{
					GONJNELNHCH = JNGNJJIBHFK - 1;
				}
			}
			if (DANPHDEJIFM >= JNGNJJIBHFK && GONJNELNHCH <= DANPHDEJIFM)
			{
				int num2 = EMFNCOCHGEA + BINANKICGPN - JNGNJJIBHFK;
				bool flag = _tr_tally(EMFNCOCHGEA - 1 - HPLACHFGPFJ, DANPHDEJIFM - JNGNJJIBHFK);
				BINANKICGPN -= DANPHDEJIFM - 1;
				DANPHDEJIFM -= 2;
				do
				{
					if (++EMFNCOCHGEA <= num2)
					{
						FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[EMFNCOCHGEA + (JNGNJJIBHFK - 1)] & 0xFF)) & MFIODCHIHLF;
						num = POLFAHOJJCN[FDKJFJENANA] & 0xFFFF;
						JIABHEAAKCH[EMFNCOCHGEA & BHAHBAHNDHM] = POLFAHOJJCN[FDKJFJENANA];
						POLFAHOJJCN[FDKJFJENANA] = (short)EMFNCOCHGEA;
					}
				}
				while (--DANPHDEJIFM != 0);
				MIJFDHGBDAD = 0;
				GONJNELNHCH = JNGNJJIBHFK - 1;
				EMFNCOCHGEA++;
				if (flag)
				{
					BCNCGCMIELN(false);
					if (CJMKCEHHMCH.NBNGINIIKNA == 0)
					{
						return HHLELELECLA.NeedMore;
					}
				}
			}
			else if (MIJFDHGBDAD != 0)
			{
				if (_tr_tally(0, window[EMFNCOCHGEA - 1] & 0xFF))
				{
					BCNCGCMIELN(false);
				}
				EMFNCOCHGEA++;
				BINANKICGPN--;
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					return HHLELELECLA.NeedMore;
				}
			}
			else
			{
				MIJFDHGBDAD = 1;
				EMFNCOCHGEA++;
				BINANKICGPN--;
			}
		}
		if (MIJFDHGBDAD != 0)
		{
			bool flag = _tr_tally(0, window[EMFNCOCHGEA - 1] & 0xFF);
			MIJFDHGBDAD = 0;
		}
		BCNCGCMIELN(NGBJDNFAPKC == AFJHGKAEJPG.Finish);
		if (CJMKCEHHMCH.NBNGINIIKNA == 0)
		{
			if (NGBJDNFAPKC == AFJHGKAEJPG.Finish)
			{
				return HHLELELECLA.FinishStarted;
			}
			return HHLELELECLA.NeedMore;
		}
		return (NGBJDNFAPKC != AFJHGKAEJPG.Finish) ? HHLELELECLA.BlockDone : HHLELELECLA.FinishDone;
	}

	internal int longest_match(int FGDAGFCDECP)
	{
		int num = IBBOLEEKAOM.DEPMLENGCBF;
		int num2 = EMFNCOCHGEA;
		int num3 = DANPHDEJIFM;
		int num4 = ((EMFNCOCHGEA > NNHBBKGGHJF - BLNLIJEEHHP) ? (EMFNCOCHGEA - (NNHBBKGGHJF - BLNLIJEEHHP)) : 0);
		int num5 = IBBOLEEKAOM.NNLJOHLDEOE;
		int bHAHBAHNDHM = BHAHBAHNDHM;
		int num6 = EMFNCOCHGEA + BBCACPAGMOF;
		byte b = window[num2 + num3 - 1];
		byte b2 = window[num2 + num3];
		if (DANPHDEJIFM >= IBBOLEEKAOM.MLJCJEAACIO)
		{
			num >>= 2;
		}
		if (num5 > BINANKICGPN)
		{
			num5 = BINANKICGPN;
		}
		do
		{
			int num7 = FGDAGFCDECP;
			if (window[num7 + num3] != b2 || window[num7 + num3 - 1] != b || window[num7] != window[num2] || window[++num7] != window[num2 + 1])
			{
				continue;
			}
			num2 += 2;
			num7++;
			while (window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && window[++num2] == window[++num7] && num2 < num6)
			{
			}
			int num8 = BBCACPAGMOF - (num6 - num2);
			num2 = num6 - BBCACPAGMOF;
			if (num8 > num3)
			{
				NDCEAFEGAFK = FGDAGFCDECP;
				num3 = num8;
				if (num8 >= num5)
				{
					break;
				}
				b = window[num2 + num3 - 1];
				b2 = window[num2 + num3];
			}
		}
		while ((FGDAGFCDECP = JIABHEAAKCH[FGDAGFCDECP & bHAHBAHNDHM] & 0xFFFF) > num4 && --num != 0);
		if (num3 <= BINANKICGPN)
		{
			return num3;
		}
		return BINANKICGPN;
	}

	internal bool GFOKPNKCOOP()
	{
		return LOICEAGHLIO;
	}

	internal void NGEBPALKODO(bool value)
	{
		LOICEAGHLIO = value;
	}

	internal int EHAJODIAFEG(ZlibCodec HNJFOALABOA, NKFKKGNBHDK GNLOCMLBNHF)
	{
		return EHAJODIAFEG(HNJFOALABOA, GNLOCMLBNHF, 15);
	}

	internal int EHAJODIAFEG(ZlibCodec HNJFOALABOA, NKFKKGNBHDK GNLOCMLBNHF, int HLFOKLCKNEE)
	{
		return EHAJODIAFEG(HNJFOALABOA, GNLOCMLBNHF, HLFOKLCKNEE, COEHLNBICJF, DDGGLIIKFPL.Default);
	}

	internal int EHAJODIAFEG(ZlibCodec HNJFOALABOA, NKFKKGNBHDK GNLOCMLBNHF, int HLFOKLCKNEE, DDGGLIIKFPL IDOIMLPCFNP)
	{
		return EHAJODIAFEG(HNJFOALABOA, GNLOCMLBNHF, HLFOKLCKNEE, COEHLNBICJF, IDOIMLPCFNP);
	}

	internal int EHAJODIAFEG(ZlibCodec HNJFOALABOA, NKFKKGNBHDK GNLOCMLBNHF, int KGFELFAKFIA, int GLEJJCGAOMO, DDGGLIIKFPL FNLGJNHJCPL)
	{
		CJMKCEHHMCH = HNJFOALABOA;
		CJMKCEHHMCH.Message = null;
		if (KGFELFAKFIA < 9 || KGFELFAKFIA > 15)
		{
			throw new ZlibException("windowBits must be in the range 9..15.");
		}
		if (GLEJJCGAOMO < 1 || GLEJJCGAOMO > JIHDOLCCHBO)
		{
			throw new ZlibException(string.Format("memLevel must be in the range 1.. {0}", JIHDOLCCHBO));
		}
		CJMKCEHHMCH.FGOHAMANMMM = this;
		EEKHLFIPIJG = KGFELFAKFIA;
		NNHBBKGGHJF = 1 << EEKHLFIPIJG;
		BHAHBAHNDHM = NNHBBKGGHJF - 1;
		OJPGOFNBHFP = GLEJJCGAOMO + 7;
		ELHHCCOMLNM = 1 << OJPGOFNBHFP;
		MFIODCHIHLF = ELHHCCOMLNM - 1;
		FIPCMJOCFCD = (OJPGOFNBHFP + JNGNJJIBHFK - 1) / JNGNJJIBHFK;
		window = new byte[NNHBBKGGHJF * 2];
		JIABHEAAKCH = new short[NNHBBKGGHJF];
		POLFAHOJJCN = new short[ELHHCCOMLNM];
		BENKHMFEIID = 1 << GLEJJCGAOMO + 6;
		BMPCIFFMMPP = new byte[BENKHMFEIID * 4];
		KDFEEGCLJHL = BENKHMFEIID;
		HMLFNGFEPHM = 3 * BENKHMFEIID;
		CPOCBHJGICD = GNLOCMLBNHF;
		IDOIMLPCFNP = FNLGJNHJCPL;
		Reset();
		return 0;
	}

	internal void Reset()
	{
		CJMKCEHHMCH.ALJBBHPGGPA = (CJMKCEHHMCH.HCDKLJJLMOD = 0L);
		CJMKCEHHMCH.Message = null;
		CCDNPCJKGGK = 0;
		GBKNGFEBIOL = 0;
		GIIABEPDGLD = false;
		status = ((!GFOKPNKCOOP()) ? HPOMCEOMBPG : GAJKKNHFMCI);
		CJMKCEHHMCH._Adler32 = Adler.IAJPFDALGJM(0u, null, 0, 0);
		KNACOPCPMJK = 0;
		AMOLIEBKCDB();
		FPLKPJJFJLL();
	}

	internal int PLHPGFGAGKJ()
	{
		if (status != GAJKKNHFMCI && status != HPOMCEOMBPG && status != OJECJENOCNK)
		{
			return -2;
		}
		BMPCIFFMMPP = null;
		POLFAHOJJCN = null;
		JIABHEAAKCH = null;
		window = null;
		return (status == HPOMCEOMBPG) ? (-3) : 0;
	}

	private void JADFPOKKPEP()
	{
		switch (IBBOLEEKAOM.BHMMEEECCKO)
		{
		case DPEICFAJMJH.Store:
			LOCMPAAEKFN = LOOAHPKFKJE;
			break;
		case DPEICFAJMJH.Fast:
			LOCMPAAEKFN = CGOFCHPJIGC;
			break;
		case DPEICFAJMJH.Slow:
			LOCMPAAEKFN = JAIJANNMDCF;
			break;
		}
	}

	internal int HBFLMIBKBBF(NKFKKGNBHDK GNLOCMLBNHF, DDGGLIIKFPL FNLGJNHJCPL)
	{
		int result = 0;
		if (CPOCBHJGICD != GNLOCMLBNHF)
		{
			CLOGLEGLGGF cLOGLEGLGGF = CLOGLEGLGGF.CJKIADKJMIK(GNLOCMLBNHF);
			if (cLOGLEGLGGF.BHMMEEECCKO != IBBOLEEKAOM.BHMMEEECCKO && CJMKCEHHMCH.ALJBBHPGGPA != 0)
			{
				result = CJMKCEHHMCH.GAMMFNJHCFO(AFJHGKAEJPG.Partial);
			}
			CPOCBHJGICD = GNLOCMLBNHF;
			IBBOLEEKAOM = cLOGLEGLGGF;
			JADFPOKKPEP();
		}
		IDOIMLPCFNP = FNLGJNHJCPL;
		return result;
	}

	internal int SetDictionary(byte[] dictionary)
	{
		int num = dictionary.Length;
		int sourceIndex = 0;
		if (dictionary == null || status != GAJKKNHFMCI)
		{
			throw new ZlibException("Stream error.");
		}
		CJMKCEHHMCH._Adler32 = Adler.IAJPFDALGJM(CJMKCEHHMCH._Adler32, dictionary, 0, dictionary.Length);
		if (num < JNGNJJIBHFK)
		{
			return 0;
		}
		if (num > NNHBBKGGHJF - BLNLIJEEHHP)
		{
			num = NNHBBKGGHJF - BLNLIJEEHHP;
			sourceIndex = dictionary.Length - num;
		}
		Array.Copy(dictionary, sourceIndex, window, 0, num);
		EMFNCOCHGEA = num;
		ALPONFEFJAK = num;
		FDKJFJENANA = window[0] & 0xFF;
		FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[1] & 0xFF)) & MFIODCHIHLF;
		for (int i = 0; i <= num - JNGNJJIBHFK; i++)
		{
			FDKJFJENANA = ((FDKJFJENANA << FIPCMJOCFCD) ^ (window[i + (JNGNJJIBHFK - 1)] & 0xFF)) & MFIODCHIHLF;
			JIABHEAAKCH[i & BHAHBAHNDHM] = POLFAHOJJCN[FDKJFJENANA];
			POLFAHOJJCN[FDKJFJENANA] = (short)i;
		}
		return 0;
	}

	internal int GAMMFNJHCFO(AFJHGKAEJPG NGBJDNFAPKC)
	{
		if (CJMKCEHHMCH.DKCGBABIAEN == null || (CJMKCEHHMCH.PEFOCMDODLD == null && CJMKCEHHMCH.IAPJEIDMGNP != 0) || (status == OJECJENOCNK && NGBJDNFAPKC != AFJHGKAEJPG.Finish))
		{
			CJMKCEHHMCH.Message = _ErrorMessage[4];
			throw new ZlibException(string.Format("Something is fishy. [{0}]", CJMKCEHHMCH.Message));
		}
		if (CJMKCEHHMCH.NBNGINIIKNA == 0)
		{
			CJMKCEHHMCH.Message = _ErrorMessage[7];
			throw new ZlibException("OutputBuffer is full (AvailableBytesOut == 0)");
		}
		int kNACOPCPMJK = KNACOPCPMJK;
		KNACOPCPMJK = (int)NGBJDNFAPKC;
		if (status == GAJKKNHFMCI)
		{
			int num = GLGHJDNCDON + (EEKHLFIPIJG - 8 << 4) << 8;
			int num2 = (int)((CPOCBHJGICD - 1) & (NKFKKGNBHDK)0xFF) >> 1;
			if (num2 > 3)
			{
				num2 = 3;
			}
			num |= num2 << 6;
			if (EMFNCOCHGEA != 0)
			{
				num |= LEDINGHODAJ;
			}
			num += 31 - num % 31;
			status = HPOMCEOMBPG;
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(num >> 8);
			BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)num;
			if (EMFNCOCHGEA != 0)
			{
				BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF000000u) >> 24);
				BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF0000) >> 16);
				BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF00) >> 8);
				BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(CJMKCEHHMCH._Adler32 & 0xFF);
			}
			CJMKCEHHMCH._Adler32 = Adler.IAJPFDALGJM(0u, null, 0, 0);
		}
		if (CCDNPCJKGGK != 0)
		{
			CJMKCEHHMCH.CHAPNKCGONG();
			if (CJMKCEHHMCH.NBNGINIIKNA == 0)
			{
				KNACOPCPMJK = -1;
				return 0;
			}
		}
		else if (CJMKCEHHMCH.IAPJEIDMGNP == 0 && (int)NGBJDNFAPKC <= kNACOPCPMJK && NGBJDNFAPKC != AFJHGKAEJPG.Finish)
		{
			return 0;
		}
		if (status == OJECJENOCNK && CJMKCEHHMCH.IAPJEIDMGNP != 0)
		{
			CJMKCEHHMCH.Message = _ErrorMessage[7];
			throw new ZlibException("status == FINISH_STATE && _codec.AvailableBytesIn != 0");
		}
		if (CJMKCEHHMCH.IAPJEIDMGNP != 0 || BINANKICGPN != 0 || (NGBJDNFAPKC != AFJHGKAEJPG.None && status != OJECJENOCNK))
		{
			HHLELELECLA hHLELELECLA = LOCMPAAEKFN(NGBJDNFAPKC);
			if (hHLELELECLA == HHLELELECLA.FinishStarted || hHLELELECLA == HHLELELECLA.FinishDone)
			{
				status = OJECJENOCNK;
			}
			switch (hHLELELECLA)
			{
			case HHLELELECLA.NeedMore:
			case HHLELELECLA.FinishStarted:
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					KNACOPCPMJK = -1;
				}
				return 0;
			case HHLELELECLA.BlockDone:
				if (NGBJDNFAPKC == AFJHGKAEJPG.Partial)
				{
					KMHCDPIKPGF();
				}
				else
				{
					OPBEHCNBEPA(0, 0, false);
					if (NGBJDNFAPKC == AFJHGKAEJPG.Full)
					{
						for (int i = 0; i < ELHHCCOMLNM; i++)
						{
							POLFAHOJJCN[i] = 0;
						}
					}
				}
				CJMKCEHHMCH.CHAPNKCGONG();
				if (CJMKCEHHMCH.NBNGINIIKNA == 0)
				{
					KNACOPCPMJK = -1;
					return 0;
				}
				break;
			}
		}
		if (NGBJDNFAPKC != AFJHGKAEJPG.Finish)
		{
			return 0;
		}
		if (!GFOKPNKCOOP() || GIIABEPDGLD)
		{
			return 1;
		}
		BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF000000u) >> 24);
		BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF0000) >> 16);
		BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)((CJMKCEHHMCH._Adler32 & 0xFF00) >> 8);
		BMPCIFFMMPP[CCDNPCJKGGK++] = (byte)(CJMKCEHHMCH._Adler32 & 0xFF);
		CJMKCEHHMCH.CHAPNKCGONG();
		GIIABEPDGLD = true;
		return (CCDNPCJKGGK == 0) ? 1 : 0;
	}
}
