using System;
using System.IO;

public class MNPBDHNFEBB : ICoder, MGGJFKDFADC, IWriteCoderProperties
{
	private enum MFLEICEJPNC
	{
		BT2 = 0,
		BT4 = 1
	}

	private class HMMNGDPPEOO
	{
		public struct EKANDKFGMGL
		{
			private BitEncoder[] HMCMAMJBKND;

			public void Create()
			{
				HMCMAMJBKND = new BitEncoder[768];
			}

			public void Init()
			{
				for (int i = 0; i < 768; i++)
				{
					HMCMAMJBKND[i].Init();
				}
			}

			public void Encode(ABCAONADOMK JHAAEJNODIF, byte symbol)
			{
				uint num = 1u;
				for (int num2 = 7; num2 >= 0; num2--)
				{
					uint num3 = (uint)((symbol >> num2) & 1);
					HMCMAMJBKND[num].Encode(JHAAEJNODIF, num3);
					num = (num << 1) | num3;
				}
			}

			public void IMEHLOFGAHF(ABCAONADOMK JHAAEJNODIF, byte HGMKIONDDNO, byte symbol)
			{
				uint num = 1u;
				bool flag = true;
				for (int num2 = 7; num2 >= 0; num2--)
				{
					uint num3 = (uint)((symbol >> num2) & 1);
					uint num4 = num;
					if (flag)
					{
						uint num5 = (uint)((HGMKIONDDNO >> num2) & 1);
						num4 += 1 + num5 << 8;
						flag = num5 == num3;
					}
					HMCMAMJBKND[num4].Encode(JHAAEJNODIF, num3);
					num = (num << 1) | num3;
				}
			}

			public uint GetPrice(bool PGBFBFEDLBH, byte HGMKIONDDNO, byte symbol)
			{
				uint num = 0u;
				uint num2 = 1u;
				int num3 = 7;
				if (PGBFBFEDLBH)
				{
					while (num3 >= 0)
					{
						uint num4 = (uint)((HGMKIONDDNO >> num3) & 1);
						uint num5 = (uint)((symbol >> num3) & 1);
						num += HMCMAMJBKND[(1 + num4 << 8) + num2].GetPrice(num5);
						num2 = (num2 << 1) | num5;
						if (num4 != num5)
						{
							num3--;
							break;
						}
						num3--;
					}
				}
				while (num3 >= 0)
				{
					uint num6 = (uint)((symbol >> num3) & 1);
					num += HMCMAMJBKND[num2].GetPrice(num6);
					num2 = (num2 << 1) | num6;
					num3--;
				}
				return num;
			}
		}

		private EKANDKFGMGL[] NJGENNBJGNJ;

		private int BPMAHCNJIGH;

		private int AICNPFKGFLD;

		private uint m_PosMask;

		public void Create(int PGIOGOCKAPN, int NNNENAADHAE)
		{
			if (NJGENNBJGNJ == null || BPMAHCNJIGH != NNNENAADHAE || AICNPFKGFLD != PGIOGOCKAPN)
			{
				AICNPFKGFLD = PGIOGOCKAPN;
				m_PosMask = (uint)((1 << PGIOGOCKAPN) - 1);
				BPMAHCNJIGH = NNNENAADHAE;
				uint num = (uint)(1 << BPMAHCNJIGH + AICNPFKGFLD);
				NJGENNBJGNJ = new EKANDKFGMGL[num];
				for (uint num2 = 0u; num2 < num; num2++)
				{
					NJGENNBJGNJ[num2].Create();
				}
			}
		}

		public void Init()
		{
			uint num = (uint)(1 << BPMAHCNJIGH + AICNPFKGFLD);
			for (uint num2 = 0u; num2 < num; num2++)
			{
				NJGENNBJGNJ[num2].Init();
			}
		}

		public EKANDKFGMGL MCLCHDNOONB(uint LCCLEFMKLPB, byte PMEIMKDGNJP)
		{
			return NJGENNBJGNJ[(int)((LCCLEFMKLPB & m_PosMask) << BPMAHCNJIGH) + (PMEIMKDGNJP >> 8 - BPMAHCNJIGH)];
		}
	}

	private class NOINIDIMIPK
	{
		private BitEncoder DANFCJKJIGN = default(BitEncoder);

		private BitEncoder KJOECJJAFAA = default(BitEncoder);

		private BitTreeEncoder[] ALAECANPIPH = new BitTreeEncoder[16];

		private BitTreeEncoder[] ILMEHFDGAKL = new BitTreeEncoder[16];

		private BitTreeEncoder PHBNNNJEING = new BitTreeEncoder(8);

		public NOINIDIMIPK()
		{
			for (uint num = 0u; num < 16; num++)
			{
				ALAECANPIPH[num] = new BitTreeEncoder(3);
				ILMEHFDGAKL[num] = new BitTreeEncoder(3);
			}
		}

		public void Init(uint BHMGNFOKODN)
		{
			DANFCJKJIGN.Init();
			KJOECJJAFAA.Init();
			for (uint num = 0u; num < BHMGNFOKODN; num++)
			{
				ALAECANPIPH[num].Init();
				ILMEHFDGAKL[num].Init();
			}
			PHBNNNJEING.Init();
		}

		public void Encode(ABCAONADOMK JHAAEJNODIF, uint symbol, uint LFOAILOHHHD)
		{
			if (symbol < 8)
			{
				DANFCJKJIGN.Encode(JHAAEJNODIF, 0u);
				ALAECANPIPH[LFOAILOHHHD].Encode(JHAAEJNODIF, symbol);
				return;
			}
			symbol -= 8;
			DANFCJKJIGN.Encode(JHAAEJNODIF, 1u);
			if (symbol < 8)
			{
				KJOECJJAFAA.Encode(JHAAEJNODIF, 0u);
				ILMEHFDGAKL[LFOAILOHHHD].Encode(JHAAEJNODIF, symbol);
			}
			else
			{
				KJOECJJAFAA.Encode(JHAAEJNODIF, 1u);
				PHBNNNJEING.Encode(JHAAEJNODIF, symbol - 8);
			}
		}

		public void DHELHLENEDG(uint LFOAILOHHHD, uint DCFHENPGLID, uint[] LDDIFHOEMEI, uint DCOKHNMLPGJ)
		{
			uint num = DANFCJKJIGN.ONPEFCGICJL();
			uint num2 = DANFCJKJIGN.AJMPGCNGLHF();
			uint num3 = num2 + KJOECJJAFAA.ONPEFCGICJL();
			uint num4 = num2 + KJOECJJAFAA.AJMPGCNGLHF();
			uint num5 = 0u;
			for (num5 = 0u; num5 < 8; num5++)
			{
				if (num5 >= DCFHENPGLID)
				{
					return;
				}
				LDDIFHOEMEI[DCOKHNMLPGJ + num5] = num + ALAECANPIPH[LFOAILOHHHD].GetPrice(num5);
			}
			for (; num5 < 16; num5++)
			{
				if (num5 >= DCFHENPGLID)
				{
					return;
				}
				LDDIFHOEMEI[DCOKHNMLPGJ + num5] = num3 + ILMEHFDGAKL[LFOAILOHHHD].GetPrice(num5 - 8);
			}
			for (; num5 < DCFHENPGLID; num5++)
			{
				LDDIFHOEMEI[DCOKHNMLPGJ + num5] = num4 + PHBNNNJEING.GetPrice(num5 - 8 - 8);
			}
		}
	}

	private class FEIJFCPIENJ : NOINIDIMIPK
	{
		private uint[] NHDOHBCNLGB = new uint[4352];

		private uint _tableSize;

		private uint[] GGOFNBMGJAF = new uint[16];

		public void NJBLBNKENAI(uint ELDKFILGOIH)
		{
			_tableSize = ELDKFILGOIH;
		}

		public uint GetPrice(uint symbol, uint LFOAILOHHHD)
		{
			return NHDOHBCNLGB[LFOAILOHHHD * 272 + symbol];
		}

		private void LEEJBLFBEPF(uint LFOAILOHHHD)
		{
			DHELHLENEDG(LFOAILOHHHD, _tableSize, NHDOHBCNLGB, LFOAILOHHHD * 272);
			GGOFNBMGJAF[LFOAILOHHHD] = _tableSize;
		}

		public void OPPPKCDDGDA(uint BHMGNFOKODN)
		{
			for (uint num = 0u; num < BHMGNFOKODN; num++)
			{
				LEEJBLFBEPF(num);
			}
		}

		public new void Encode(ABCAONADOMK JHAAEJNODIF, uint symbol, uint LFOAILOHHHD)
		{
			base.Encode(JHAAEJNODIF, symbol, LFOAILOHHHD);
			if (--GGOFNBMGJAF[LFOAILOHHHD] == 0)
			{
				LEEJBLFBEPF(LFOAILOHHHD);
			}
		}
	}

	private class JDOABBOPFIP
	{
		public Base.IPAFOKKOCPF AFINHOBCHMC;

		public bool JFLGHPCMGPJ;

		public bool DFFBJFJFNPO;

		public uint PDDPBPPEEIL;

		public uint JFFHGEODLOA;

		public uint MDAAJFBENON;

		public uint DFLODJBKHDN;

		public uint EKNEPEFHCIL;

		public uint NEILFPEKJIH;

		public uint JJLNNBJLAFJ;

		public uint EBCPIDBOAEA;

		public uint BOIAJCDOPJH;

		public void KILCJHGPMLF()
		{
			EKNEPEFHCIL = uint.MaxValue;
			JFLGHPCMGPJ = false;
		}

		public void NCAABLGLDFG()
		{
			EKNEPEFHCIL = 0u;
			JFLGHPCMGPJ = false;
		}

		public bool BEOHCMCNOFO()
		{
			return EKNEPEFHCIL == 0;
		}
	}

	private const uint ALLLEHAPNJO = 268435455u;

	private static byte[] JGDAIMGAKIJ;

	private Base.IPAFOKKOCPF MAFFNGPOMJD = default(Base.IPAFOKKOCPF);

	private byte _previousByte;

	private uint[] CFFEKCKAGIN = new uint[4];

	private const int kDefaultDictionaryLogSize = 22;

	private const uint CDIAJCBFFEM = 32u;

	private const uint BNJFONJBECI = 16u;

	private const uint KBOCBLEAFKJ = 4096u;

	private JDOABBOPFIP[] OHIALPKODDM = new JDOABBOPFIP[4096];

	private IMatchFinder BECCNBNGBMH;

	private ABCAONADOMK GAHJGFOBDEK = new ABCAONADOMK();

	private BitEncoder[] OANEKNIFFCJ = new BitEncoder[192];

	private BitEncoder[] EGJKEMKOAGI = new BitEncoder[12];

	private BitEncoder[] FMMIBCGBJFF = new BitEncoder[12];

	private BitEncoder[] FEBICAJPIEN = new BitEncoder[12];

	private BitEncoder[] MEOAPKPCCDJ = new BitEncoder[12];

	private BitEncoder[] DIPPHLGCMGI = new BitEncoder[192];

	private BitTreeEncoder[] DEBLJELNEIA = new BitTreeEncoder[4];

	private BitEncoder[] ENCNCJDEKJP = new BitEncoder[114];

	private BitTreeEncoder IKLHGFMBHPA = new BitTreeEncoder(4);

	private FEIJFCPIENJ OCKNLPLBGIH = new FEIJFCPIENJ();

	private FEIJFCPIENJ LFNCDIHBEBP = new FEIJFCPIENJ();

	private HMMNGDPPEOO HFBHLKGNLEJ = new HMMNGDPPEOO();

	private uint[] CKBOOFEPIBC = new uint[548];

	private uint IAOEIFNEDDA = 32u;

	private uint LMHPLGLKPDM;

	private uint GBKOANCOMKF;

	private uint GEGHMBCKOOK;

	private uint NGCOGHKHDJF;

	private uint OJAMMLHOBGB;

	private bool FIMGEKNDHNG;

	private uint[] JENEFGHKCFI = new uint[256];

	private uint[] ACLJBILGHAA = new uint[512];

	private uint[] CAGKEHLNPGC = new uint[16];

	private uint ENDOKKMJEBN;

	private uint FCNOKIOBIEI = 44u;

	private int FPIFNNAMBMD = 2;

	private uint OODFDPMGBAB = 3u;

	private int IFMDMMDINPJ;

	private int BNGMCPLAHJG = 3;

	private uint MIGBGBGMDCJ = 4194304u;

	private uint LENLBJPOICN = uint.MaxValue;

	private uint NPGJKAFGDGP = uint.MaxValue;

	private long nowPos64;

	private bool EJDPCKCHJJL;

	private Stream _inStream;

	private MFLEICEJPNC BFDNPAJNFFK = MFLEICEJPNC.BT4;

	private bool PAILLFPPLBF;

	private bool NDEEGIDGFGK;

	private uint[] JDJPNHNCNEC = new uint[4];

	private uint[] DBBGKDIBMAH = new uint[4];

	private const int kPropSize = 5;

	private byte[] properties = new byte[5];

	private uint[] GIHLHAANGHC = new uint[128];

	private uint DCLKLHGBGIA;

	private static string[] kMatchFinderIDs;

	private uint FFBNKMHPPMM;

	static MNPBDHNFEBB()
	{
		JGDAIMGAKIJ = new byte[2048];
		kMatchFinderIDs = new string[2] { "BT2", "BT4" };
		int num = 2;
		JGDAIMGAKIJ[0] = 0;
		JGDAIMGAKIJ[1] = 1;
		for (byte b = 2; b < 22; b++)
		{
			uint num2 = (uint)(1 << (b >> 1) - 1);
			uint num3 = 0u;
			while (num3 < num2)
			{
				JGDAIMGAKIJ[num] = b;
				num3++;
				num++;
			}
		}
	}

	public MNPBDHNFEBB()
	{
		for (int i = 0; (long)i < 4096L; i++)
		{
			OHIALPKODDM[i] = new JDOABBOPFIP();
		}
		for (int j = 0; (long)j < 4L; j++)
		{
			DEBLJELNEIA[j] = new BitTreeEncoder(6);
		}
	}

	private static uint BINLJKCMANH(uint LCCLEFMKLPB)
	{
		if (LCCLEFMKLPB < 2048)
		{
			return JGDAIMGAKIJ[LCCLEFMKLPB];
		}
		if (LCCLEFMKLPB < 2097152)
		{
			return (uint)(JGDAIMGAKIJ[LCCLEFMKLPB >> 10] + 20);
		}
		return (uint)(JGDAIMGAKIJ[LCCLEFMKLPB >> 20] + 40);
	}

	private static uint OLBLOPEHCCI(uint LCCLEFMKLPB)
	{
		if (LCCLEFMKLPB < 131072)
		{
			return (uint)(JGDAIMGAKIJ[LCCLEFMKLPB >> 6] + 12);
		}
		if (LCCLEFMKLPB < 134217728)
		{
			return (uint)(JGDAIMGAKIJ[LCCLEFMKLPB >> 16] + 32);
		}
		return (uint)(JGDAIMGAKIJ[LCCLEFMKLPB >> 26] + 52);
	}

	private void AIDKEKHBNOJ()
	{
		MAFFNGPOMJD.Init();
		_previousByte = 0;
		for (uint num = 0u; num < 4; num++)
		{
			CFFEKCKAGIN[num] = 0u;
		}
	}

	private void Create()
	{
		if (BECCNBNGBMH == null)
		{
			BinTree mEEBALKDNBG = new BinTree();
			int eOKCENIBPJD = 4;
			if (BFDNPAJNFFK == MFLEICEJPNC.BT2)
			{
				eOKCENIBPJD = 2;
			}
			mEEBALKDNBG.SetType(eOKCENIBPJD);
			BECCNBNGBMH = mEEBALKDNBG;
		}
		HFBHLKGNLEJ.Create(IFMDMMDINPJ, BNGMCPLAHJG);
		if (MIGBGBGMDCJ != LENLBJPOICN || NPGJKAFGDGP != IAOEIFNEDDA)
		{
			BECCNBNGBMH.Create(MIGBGBGMDCJ, 4096u, IAOEIFNEDDA, 274u);
			LENLBJPOICN = MIGBGBGMDCJ;
			NPGJKAFGDGP = IAOEIFNEDDA;
		}
	}

	private void SetWriteEndMarkerMode(bool KJDHMDADICC)
	{
		PAILLFPPLBF = KJDHMDADICC;
	}

	private void Init()
	{
		AIDKEKHBNOJ();
		GAHJGFOBDEK.Init();
		for (uint num = 0u; num < 12; num++)
		{
			for (uint num2 = 0u; num2 <= OODFDPMGBAB; num2++)
			{
				uint num3 = (num << 4) + num2;
				OANEKNIFFCJ[num3].Init();
				DIPPHLGCMGI[num3].Init();
			}
			EGJKEMKOAGI[num].Init();
			FMMIBCGBJFF[num].Init();
			FEBICAJPIEN[num].Init();
			MEOAPKPCCDJ[num].Init();
		}
		HFBHLKGNLEJ.Init();
		for (uint num = 0u; num < 4; num++)
		{
			DEBLJELNEIA[num].Init();
		}
		for (uint num = 0u; num < 114; num++)
		{
			ENCNCJDEKJP[num].Init();
		}
		OCKNLPLBGIH.Init((uint)(1 << FPIFNNAMBMD));
		LFNCDIHBEBP.Init((uint)(1 << FPIFNNAMBMD));
		IKLHGFMBHPA.Init();
		FIMGEKNDHNG = false;
		NGCOGHKHDJF = 0u;
		OJAMMLHOBGB = 0u;
		GEGHMBCKOOK = 0u;
	}

	private void ReadMatchDistances(out uint CEAHDKFDIOK, out uint IBGEENFNMHL)
	{
		CEAHDKFDIOK = 0u;
		IBGEENFNMHL = BECCNBNGBMH.GetMatches(CKBOOFEPIBC);
		if (IBGEENFNMHL != 0)
		{
			CEAHDKFDIOK = CKBOOFEPIBC[IBGEENFNMHL - 2];
			if (CEAHDKFDIOK == IAOEIFNEDDA)
			{
				CEAHDKFDIOK += BECCNBNGBMH.GetMatchLen((int)(CEAHDKFDIOK - 1), CKBOOFEPIBC[IBGEENFNMHL - 1], 273 - CEAHDKFDIOK);
			}
		}
		GEGHMBCKOOK++;
	}

	private void MHEJFMDCOHI(uint OMEDGJMNGKE)
	{
		if (OMEDGJMNGKE != 0)
		{
			BECCNBNGBMH.Skip(OMEDGJMNGKE);
			GEGHMBCKOOK += OMEDGJMNGKE;
		}
	}

	private uint KNBJIEKIPOP(Base.IPAFOKKOCPF state, uint LFOAILOHHHD)
	{
		return FMMIBCGBJFF[state.Index].ONPEFCGICJL() + DIPPHLGCMGI[(state.Index << 4) + LFOAILOHHHD].ONPEFCGICJL();
	}

	private uint LFONOEMCMDD(uint CBEPCAHEEMI, Base.IPAFOKKOCPF state, uint LFOAILOHHHD)
	{
		uint num;
		if (CBEPCAHEEMI == 0)
		{
			num = FMMIBCGBJFF[state.Index].ONPEFCGICJL();
			return num + DIPPHLGCMGI[(state.Index << 4) + LFOAILOHHHD].AJMPGCNGLHF();
		}
		num = FMMIBCGBJFF[state.Index].AJMPGCNGLHF();
		if (CBEPCAHEEMI == 1)
		{
			return num + FEBICAJPIEN[state.Index].ONPEFCGICJL();
		}
		num += FEBICAJPIEN[state.Index].AJMPGCNGLHF();
		return num + MEOAPKPCCDJ[state.Index].GetPrice(CBEPCAHEEMI - 2);
	}

	private uint LMFLOILOLDJ(uint CBEPCAHEEMI, uint JCAJDBOMGOM, Base.IPAFOKKOCPF state, uint LFOAILOHHHD)
	{
		uint num = LFNCDIHBEBP.GetPrice(JCAJDBOMGOM - 2, LFOAILOHHHD);
		return num + LFONOEMCMDD(CBEPCAHEEMI, state, LFOAILOHHHD);
	}

	private uint GetPosLenPrice(uint LCCLEFMKLPB, uint JCAJDBOMGOM, uint LFOAILOHHHD)
	{
		uint num = Base.BBAEOHBBCHI(JCAJDBOMGOM);
		uint num2 = ((LCCLEFMKLPB >= 128) ? (JENEFGHKCFI[(num << 6) + OLBLOPEHCCI(LCCLEFMKLPB)] + CAGKEHLNPGC[LCCLEFMKLPB & 0xF]) : ACLJBILGHAA[num * 128 + LCCLEFMKLPB]);
		return num2 + OCKNLPLBGIH.GetPrice(JCAJDBOMGOM - 2, LFOAILOHHHD);
	}

	private uint Backward(out uint PHJEECBMCFO, uint MGPKJFBKOOO)
	{
		NGCOGHKHDJF = MGPKJFBKOOO;
		uint dFLODJBKHDN = OHIALPKODDM[MGPKJFBKOOO].DFLODJBKHDN;
		uint eKNEPEFHCIL = OHIALPKODDM[MGPKJFBKOOO].EKNEPEFHCIL;
		do
		{
			if (OHIALPKODDM[MGPKJFBKOOO].JFLGHPCMGPJ)
			{
				OHIALPKODDM[dFLODJBKHDN].KILCJHGPMLF();
				OHIALPKODDM[dFLODJBKHDN].DFLODJBKHDN = dFLODJBKHDN - 1;
				if (OHIALPKODDM[MGPKJFBKOOO].DFFBJFJFNPO)
				{
					OHIALPKODDM[dFLODJBKHDN - 1].JFLGHPCMGPJ = false;
					OHIALPKODDM[dFLODJBKHDN - 1].DFLODJBKHDN = OHIALPKODDM[MGPKJFBKOOO].PDDPBPPEEIL;
					OHIALPKODDM[dFLODJBKHDN - 1].EKNEPEFHCIL = OHIALPKODDM[MGPKJFBKOOO].JFFHGEODLOA;
				}
			}
			uint num = dFLODJBKHDN;
			uint eKNEPEFHCIL2 = eKNEPEFHCIL;
			eKNEPEFHCIL = OHIALPKODDM[num].EKNEPEFHCIL;
			dFLODJBKHDN = OHIALPKODDM[num].DFLODJBKHDN;
			OHIALPKODDM[num].EKNEPEFHCIL = eKNEPEFHCIL2;
			OHIALPKODDM[num].DFLODJBKHDN = MGPKJFBKOOO;
			MGPKJFBKOOO = num;
		}
		while (MGPKJFBKOOO != 0);
		PHJEECBMCFO = OHIALPKODDM[0].EKNEPEFHCIL;
		OJAMMLHOBGB = OHIALPKODDM[0].DFLODJBKHDN;
		return OJAMMLHOBGB;
	}

	private uint GetOptimum(uint MGMMDGFPBLP, out uint PHJEECBMCFO)
	{
		if (NGCOGHKHDJF != OJAMMLHOBGB)
		{
			uint result = OHIALPKODDM[OJAMMLHOBGB].DFLODJBKHDN - OJAMMLHOBGB;
			PHJEECBMCFO = OHIALPKODDM[OJAMMLHOBGB].EKNEPEFHCIL;
			OJAMMLHOBGB = OHIALPKODDM[OJAMMLHOBGB].DFLODJBKHDN;
			return result;
		}
		OJAMMLHOBGB = (NGCOGHKHDJF = 0u);
		uint CEAHDKFDIOK;
		uint IBGEENFNMHL;
		if (!FIMGEKNDHNG)
		{
			ReadMatchDistances(out CEAHDKFDIOK, out IBGEENFNMHL);
		}
		else
		{
			CEAHDKFDIOK = LMHPLGLKPDM;
			IBGEENFNMHL = GBKOANCOMKF;
			FIMGEKNDHNG = false;
		}
		uint num = BECCNBNGBMH.HBJMPBCHFJB() + 1;
		if (num < 2)
		{
			PHJEECBMCFO = uint.MaxValue;
			return 1u;
		}
		if (num > 273)
		{
			num = 273u;
		}
		uint num2 = 0u;
		for (uint num3 = 0u; num3 < 4; num3++)
		{
			JDJPNHNCNEC[num3] = CFFEKCKAGIN[num3];
			DBBGKDIBMAH[num3] = BECCNBNGBMH.GetMatchLen(-1, JDJPNHNCNEC[num3], 273u);
			if (DBBGKDIBMAH[num3] > DBBGKDIBMAH[num2])
			{
				num2 = num3;
			}
		}
		if (DBBGKDIBMAH[num2] >= IAOEIFNEDDA)
		{
			PHJEECBMCFO = num2;
			uint num4 = DBBGKDIBMAH[num2];
			MHEJFMDCOHI(num4 - 1);
			return num4;
		}
		if (CEAHDKFDIOK >= IAOEIFNEDDA)
		{
			PHJEECBMCFO = CKBOOFEPIBC[IBGEENFNMHL - 1] + 4;
			MHEJFMDCOHI(CEAHDKFDIOK - 1);
			return CEAHDKFDIOK;
		}
		byte b = BECCNBNGBMH.GetIndexByte(-1);
		byte b2 = BECCNBNGBMH.GetIndexByte((int)(0 - CFFEKCKAGIN[0] - 1 - 1));
		if (CEAHDKFDIOK < 2 && b != b2 && DBBGKDIBMAH[num2] < 2)
		{
			PHJEECBMCFO = uint.MaxValue;
			return 1u;
		}
		OHIALPKODDM[0].AFINHOBCHMC = MAFFNGPOMJD;
		uint num5 = MGMMDGFPBLP & OODFDPMGBAB;
		OHIALPKODDM[1].MDAAJFBENON = OANEKNIFFCJ[(MAFFNGPOMJD.Index << 4) + num5].ONPEFCGICJL() + HFBHLKGNLEJ.MCLCHDNOONB(MGMMDGFPBLP, _previousByte).GetPrice(!MAFFNGPOMJD.ALIFLOIMDFO(), b2, b);
		OHIALPKODDM[1].KILCJHGPMLF();
		uint num6 = OANEKNIFFCJ[(MAFFNGPOMJD.Index << 4) + num5].AJMPGCNGLHF();
		uint num7 = num6 + EGJKEMKOAGI[MAFFNGPOMJD.Index].AJMPGCNGLHF();
		if (b2 == b)
		{
			uint num8 = num7 + KNBJIEKIPOP(MAFFNGPOMJD, num5);
			if (num8 < OHIALPKODDM[1].MDAAJFBENON)
			{
				OHIALPKODDM[1].MDAAJFBENON = num8;
				OHIALPKODDM[1].NCAABLGLDFG();
			}
		}
		uint num9 = ((CEAHDKFDIOK < DBBGKDIBMAH[num2]) ? DBBGKDIBMAH[num2] : CEAHDKFDIOK);
		if (num9 < 2)
		{
			PHJEECBMCFO = OHIALPKODDM[1].EKNEPEFHCIL;
			return 1u;
		}
		OHIALPKODDM[1].DFLODJBKHDN = 0u;
		OHIALPKODDM[0].NEILFPEKJIH = JDJPNHNCNEC[0];
		OHIALPKODDM[0].JJLNNBJLAFJ = JDJPNHNCNEC[1];
		OHIALPKODDM[0].EBCPIDBOAEA = JDJPNHNCNEC[2];
		OHIALPKODDM[0].BOIAJCDOPJH = JDJPNHNCNEC[3];
		uint num10 = num9;
		do
		{
			OHIALPKODDM[num10--].MDAAJFBENON = 268435455u;
		}
		while (num10 >= 2);
		for (uint num3 = 0u; num3 < 4; num3++)
		{
			uint num11 = DBBGKDIBMAH[num3];
			if (num11 < 2)
			{
				continue;
			}
			uint num12 = num7 + LFONOEMCMDD(num3, MAFFNGPOMJD, num5);
			do
			{
				uint num13 = num12 + LFNCDIHBEBP.GetPrice(num11 - 2, num5);
				JDOABBOPFIP jDOABBOPFIP = OHIALPKODDM[num11];
				if (num13 < jDOABBOPFIP.MDAAJFBENON)
				{
					jDOABBOPFIP.MDAAJFBENON = num13;
					jDOABBOPFIP.DFLODJBKHDN = 0u;
					jDOABBOPFIP.EKNEPEFHCIL = num3;
					jDOABBOPFIP.JFLGHPCMGPJ = false;
				}
			}
			while (--num11 >= 2);
		}
		uint num14 = num6 + EGJKEMKOAGI[MAFFNGPOMJD.Index].ONPEFCGICJL();
		num10 = ((DBBGKDIBMAH[0] < 2) ? 2u : (DBBGKDIBMAH[0] + 1));
		if (num10 <= CEAHDKFDIOK)
		{
			uint num15;
			for (num15 = 0u; num10 > CKBOOFEPIBC[num15]; num15 += 2)
			{
			}
			while (true)
			{
				uint num16 = CKBOOFEPIBC[num15 + 1];
				uint num17 = num14 + GetPosLenPrice(num16, num10, num5);
				JDOABBOPFIP jDOABBOPFIP2 = OHIALPKODDM[num10];
				if (num17 < jDOABBOPFIP2.MDAAJFBENON)
				{
					jDOABBOPFIP2.MDAAJFBENON = num17;
					jDOABBOPFIP2.DFLODJBKHDN = 0u;
					jDOABBOPFIP2.EKNEPEFHCIL = num16 + 4;
					jDOABBOPFIP2.JFLGHPCMGPJ = false;
				}
				if (num10 == CKBOOFEPIBC[num15])
				{
					num15 += 2;
					if (num15 == IBGEENFNMHL)
					{
						break;
					}
				}
				num10++;
			}
		}
		uint num18 = 0u;
		uint CEAHDKFDIOK2;
		while (true)
		{
			num18++;
			if (num18 == num9)
			{
				return Backward(out PHJEECBMCFO, num18);
			}
			ReadMatchDistances(out CEAHDKFDIOK2, out IBGEENFNMHL);
			if (CEAHDKFDIOK2 >= IAOEIFNEDDA)
			{
				break;
			}
			MGMMDGFPBLP++;
			uint num19 = OHIALPKODDM[num18].DFLODJBKHDN;
			Base.IPAFOKKOCPF aFINHOBCHMC;
			if (OHIALPKODDM[num18].JFLGHPCMGPJ)
			{
				num19--;
				if (OHIALPKODDM[num18].DFFBJFJFNPO)
				{
					aFINHOBCHMC = OHIALPKODDM[OHIALPKODDM[num18].PDDPBPPEEIL].AFINHOBCHMC;
					if (OHIALPKODDM[num18].JFFHGEODLOA < 4)
					{
						aFINHOBCHMC.EJIFGEACABJ();
					}
					else
					{
						aFINHOBCHMC.HCMNGMEPJGM();
					}
				}
				else
				{
					aFINHOBCHMC = OHIALPKODDM[num19].AFINHOBCHMC;
				}
				aFINHOBCHMC.BPGEKNIINGF();
			}
			else
			{
				aFINHOBCHMC = OHIALPKODDM[num19].AFINHOBCHMC;
			}
			if (num19 == num18 - 1)
			{
				if (OHIALPKODDM[num18].BEOHCMCNOFO())
				{
					aFINHOBCHMC.GGAGNGPBMIH();
				}
				else
				{
					aFINHOBCHMC.BPGEKNIINGF();
				}
			}
			else
			{
				uint num20;
				if (OHIALPKODDM[num18].JFLGHPCMGPJ && OHIALPKODDM[num18].DFFBJFJFNPO)
				{
					num19 = OHIALPKODDM[num18].PDDPBPPEEIL;
					num20 = OHIALPKODDM[num18].JFFHGEODLOA;
					aFINHOBCHMC.EJIFGEACABJ();
				}
				else
				{
					num20 = OHIALPKODDM[num18].EKNEPEFHCIL;
					if (num20 < 4)
					{
						aFINHOBCHMC.EJIFGEACABJ();
					}
					else
					{
						aFINHOBCHMC.HCMNGMEPJGM();
					}
				}
				JDOABBOPFIP jDOABBOPFIP3 = OHIALPKODDM[num19];
				switch (num20)
				{
				case 0u:
					JDJPNHNCNEC[0] = jDOABBOPFIP3.NEILFPEKJIH;
					JDJPNHNCNEC[1] = jDOABBOPFIP3.JJLNNBJLAFJ;
					JDJPNHNCNEC[2] = jDOABBOPFIP3.EBCPIDBOAEA;
					JDJPNHNCNEC[3] = jDOABBOPFIP3.BOIAJCDOPJH;
					break;
				case 1u:
					JDJPNHNCNEC[0] = jDOABBOPFIP3.JJLNNBJLAFJ;
					JDJPNHNCNEC[1] = jDOABBOPFIP3.NEILFPEKJIH;
					JDJPNHNCNEC[2] = jDOABBOPFIP3.EBCPIDBOAEA;
					JDJPNHNCNEC[3] = jDOABBOPFIP3.BOIAJCDOPJH;
					break;
				case 2u:
					JDJPNHNCNEC[0] = jDOABBOPFIP3.EBCPIDBOAEA;
					JDJPNHNCNEC[1] = jDOABBOPFIP3.NEILFPEKJIH;
					JDJPNHNCNEC[2] = jDOABBOPFIP3.JJLNNBJLAFJ;
					JDJPNHNCNEC[3] = jDOABBOPFIP3.BOIAJCDOPJH;
					break;
				case 3u:
					JDJPNHNCNEC[0] = jDOABBOPFIP3.BOIAJCDOPJH;
					JDJPNHNCNEC[1] = jDOABBOPFIP3.NEILFPEKJIH;
					JDJPNHNCNEC[2] = jDOABBOPFIP3.JJLNNBJLAFJ;
					JDJPNHNCNEC[3] = jDOABBOPFIP3.EBCPIDBOAEA;
					break;
				default:
					JDJPNHNCNEC[0] = num20 - 4;
					JDJPNHNCNEC[1] = jDOABBOPFIP3.NEILFPEKJIH;
					JDJPNHNCNEC[2] = jDOABBOPFIP3.JJLNNBJLAFJ;
					JDJPNHNCNEC[3] = jDOABBOPFIP3.EBCPIDBOAEA;
					break;
				}
			}
			OHIALPKODDM[num18].AFINHOBCHMC = aFINHOBCHMC;
			OHIALPKODDM[num18].NEILFPEKJIH = JDJPNHNCNEC[0];
			OHIALPKODDM[num18].JJLNNBJLAFJ = JDJPNHNCNEC[1];
			OHIALPKODDM[num18].EBCPIDBOAEA = JDJPNHNCNEC[2];
			OHIALPKODDM[num18].BOIAJCDOPJH = JDJPNHNCNEC[3];
			uint mDAAJFBENON = OHIALPKODDM[num18].MDAAJFBENON;
			b = BECCNBNGBMH.GetIndexByte(-1);
			b2 = BECCNBNGBMH.GetIndexByte((int)(0 - JDJPNHNCNEC[0] - 1 - 1));
			num5 = MGMMDGFPBLP & OODFDPMGBAB;
			uint num21 = mDAAJFBENON + OANEKNIFFCJ[(aFINHOBCHMC.Index << 4) + num5].ONPEFCGICJL() + HFBHLKGNLEJ.MCLCHDNOONB(MGMMDGFPBLP, BECCNBNGBMH.GetIndexByte(-2)).GetPrice(!aFINHOBCHMC.ALIFLOIMDFO(), b2, b);
			JDOABBOPFIP jDOABBOPFIP4 = OHIALPKODDM[num18 + 1];
			bool flag = false;
			if (num21 < jDOABBOPFIP4.MDAAJFBENON)
			{
				jDOABBOPFIP4.MDAAJFBENON = num21;
				jDOABBOPFIP4.DFLODJBKHDN = num18;
				jDOABBOPFIP4.KILCJHGPMLF();
				flag = true;
			}
			num6 = mDAAJFBENON + OANEKNIFFCJ[(aFINHOBCHMC.Index << 4) + num5].AJMPGCNGLHF();
			num7 = num6 + EGJKEMKOAGI[aFINHOBCHMC.Index].AJMPGCNGLHF();
			if (b2 == b && (jDOABBOPFIP4.DFLODJBKHDN >= num18 || jDOABBOPFIP4.EKNEPEFHCIL != 0))
			{
				uint num22 = num7 + KNBJIEKIPOP(aFINHOBCHMC, num5);
				if (num22 <= jDOABBOPFIP4.MDAAJFBENON)
				{
					jDOABBOPFIP4.MDAAJFBENON = num22;
					jDOABBOPFIP4.DFLODJBKHDN = num18;
					jDOABBOPFIP4.NCAABLGLDFG();
					flag = true;
				}
			}
			uint val = BECCNBNGBMH.HBJMPBCHFJB() + 1;
			val = Math.Min(4095 - num18, val);
			num = val;
			if (num < 2)
			{
				continue;
			}
			if (num > IAOEIFNEDDA)
			{
				num = IAOEIFNEDDA;
			}
			if (!flag && b2 != b)
			{
				uint lOHCIKNKDEI = Math.Min(val - 1, IAOEIFNEDDA);
				uint num23 = BECCNBNGBMH.GetMatchLen(0, JDJPNHNCNEC[0], lOHCIKNKDEI);
				if (num23 >= 2)
				{
					Base.IPAFOKKOCPF pIFKPLHIOFJ = aFINHOBCHMC;
					pIFKPLHIOFJ.BPGEKNIINGF();
					uint num24 = (MGMMDGFPBLP + 1) & OODFDPMGBAB;
					uint num25 = num21 + OANEKNIFFCJ[(pIFKPLHIOFJ.Index << 4) + num24].AJMPGCNGLHF() + EGJKEMKOAGI[pIFKPLHIOFJ.Index].AJMPGCNGLHF();
					uint num26 = num18 + 1 + num23;
					while (num9 < num26)
					{
						OHIALPKODDM[++num9].MDAAJFBENON = 268435455u;
					}
					uint num27 = num25 + LMFLOILOLDJ(0u, num23, pIFKPLHIOFJ, num24);
					JDOABBOPFIP jDOABBOPFIP5 = OHIALPKODDM[num26];
					if (num27 < jDOABBOPFIP5.MDAAJFBENON)
					{
						jDOABBOPFIP5.MDAAJFBENON = num27;
						jDOABBOPFIP5.DFLODJBKHDN = num18 + 1;
						jDOABBOPFIP5.EKNEPEFHCIL = 0u;
						jDOABBOPFIP5.JFLGHPCMGPJ = true;
						jDOABBOPFIP5.DFFBJFJFNPO = false;
					}
				}
			}
			uint num28 = 2u;
			for (uint num29 = 0u; num29 < 4; num29++)
			{
				uint num30 = BECCNBNGBMH.GetMatchLen(-1, JDJPNHNCNEC[num29], num);
				if (num30 < 2)
				{
					continue;
				}
				uint num31 = num30;
				while (true)
				{
					if (num9 < num18 + num30)
					{
						OHIALPKODDM[++num9].MDAAJFBENON = 268435455u;
						continue;
					}
					uint num32 = num7 + LMFLOILOLDJ(num29, num30, aFINHOBCHMC, num5);
					JDOABBOPFIP jDOABBOPFIP6 = OHIALPKODDM[num18 + num30];
					if (num32 < jDOABBOPFIP6.MDAAJFBENON)
					{
						jDOABBOPFIP6.MDAAJFBENON = num32;
						jDOABBOPFIP6.DFLODJBKHDN = num18;
						jDOABBOPFIP6.EKNEPEFHCIL = num29;
						jDOABBOPFIP6.JFLGHPCMGPJ = false;
					}
					if (--num30 < 2)
					{
						break;
					}
				}
				num30 = num31;
				if (num29 == 0)
				{
					num28 = num30 + 1;
				}
				if (num30 >= val)
				{
					continue;
				}
				uint lOHCIKNKDEI2 = Math.Min(val - 1 - num30, IAOEIFNEDDA);
				uint num33 = BECCNBNGBMH.GetMatchLen((int)num30, JDJPNHNCNEC[num29], lOHCIKNKDEI2);
				if (num33 >= 2)
				{
					Base.IPAFOKKOCPF pIFKPLHIOFJ2 = aFINHOBCHMC;
					pIFKPLHIOFJ2.EJIFGEACABJ();
					uint num34 = (MGMMDGFPBLP + num30) & OODFDPMGBAB;
					uint num35 = num7 + LMFLOILOLDJ(num29, num30, aFINHOBCHMC, num5) + OANEKNIFFCJ[(pIFKPLHIOFJ2.Index << 4) + num34].ONPEFCGICJL() + HFBHLKGNLEJ.MCLCHDNOONB(MGMMDGFPBLP + num30, BECCNBNGBMH.GetIndexByte((int)(num30 - 1 - 1))).GetPrice(true, BECCNBNGBMH.GetIndexByte((int)(num30 - 1 - (JDJPNHNCNEC[num29] + 1))), BECCNBNGBMH.GetIndexByte((int)(num30 - 1)));
					pIFKPLHIOFJ2.BPGEKNIINGF();
					num34 = (MGMMDGFPBLP + num30 + 1) & OODFDPMGBAB;
					uint num36 = num35 + OANEKNIFFCJ[(pIFKPLHIOFJ2.Index << 4) + num34].AJMPGCNGLHF();
					uint num37 = num36 + EGJKEMKOAGI[pIFKPLHIOFJ2.Index].AJMPGCNGLHF();
					uint num38 = num30 + 1 + num33;
					while (num9 < num18 + num38)
					{
						OHIALPKODDM[++num9].MDAAJFBENON = 268435455u;
					}
					uint num39 = num37 + LMFLOILOLDJ(0u, num33, pIFKPLHIOFJ2, num34);
					JDOABBOPFIP jDOABBOPFIP7 = OHIALPKODDM[num18 + num38];
					if (num39 < jDOABBOPFIP7.MDAAJFBENON)
					{
						jDOABBOPFIP7.MDAAJFBENON = num39;
						jDOABBOPFIP7.DFLODJBKHDN = num18 + num30 + 1;
						jDOABBOPFIP7.EKNEPEFHCIL = 0u;
						jDOABBOPFIP7.JFLGHPCMGPJ = true;
						jDOABBOPFIP7.DFFBJFJFNPO = true;
						jDOABBOPFIP7.PDDPBPPEEIL = num18;
						jDOABBOPFIP7.JFFHGEODLOA = num29;
					}
				}
			}
			if (CEAHDKFDIOK2 > num)
			{
				CEAHDKFDIOK2 = num;
				for (IBGEENFNMHL = 0u; CEAHDKFDIOK2 > CKBOOFEPIBC[IBGEENFNMHL]; IBGEENFNMHL += 2)
				{
				}
				CKBOOFEPIBC[IBGEENFNMHL] = CEAHDKFDIOK2;
				IBGEENFNMHL += 2;
			}
			if (CEAHDKFDIOK2 < num28)
			{
				continue;
			}
			num14 = num6 + EGJKEMKOAGI[aFINHOBCHMC.Index].ONPEFCGICJL();
			while (num9 < num18 + CEAHDKFDIOK2)
			{
				OHIALPKODDM[++num9].MDAAJFBENON = 268435455u;
			}
			uint num40;
			for (num40 = 0u; num28 > CKBOOFEPIBC[num40]; num40 += 2)
			{
			}
			uint num41 = num28;
			while (true)
			{
				uint num42 = CKBOOFEPIBC[num40 + 1];
				uint num43 = num14 + GetPosLenPrice(num42, num41, num5);
				JDOABBOPFIP jDOABBOPFIP8 = OHIALPKODDM[num18 + num41];
				if (num43 < jDOABBOPFIP8.MDAAJFBENON)
				{
					jDOABBOPFIP8.MDAAJFBENON = num43;
					jDOABBOPFIP8.DFLODJBKHDN = num18;
					jDOABBOPFIP8.EKNEPEFHCIL = num42 + 4;
					jDOABBOPFIP8.JFLGHPCMGPJ = false;
				}
				if (num41 == CKBOOFEPIBC[num40])
				{
					if (num41 < val)
					{
						uint lOHCIKNKDEI3 = Math.Min(val - 1 - num41, IAOEIFNEDDA);
						uint num44 = BECCNBNGBMH.GetMatchLen((int)num41, num42, lOHCIKNKDEI3);
						if (num44 >= 2)
						{
							Base.IPAFOKKOCPF pIFKPLHIOFJ3 = aFINHOBCHMC;
							pIFKPLHIOFJ3.HCMNGMEPJGM();
							uint num45 = (MGMMDGFPBLP + num41) & OODFDPMGBAB;
							uint num46 = num43 + OANEKNIFFCJ[(pIFKPLHIOFJ3.Index << 4) + num45].ONPEFCGICJL() + HFBHLKGNLEJ.MCLCHDNOONB(MGMMDGFPBLP + num41, BECCNBNGBMH.GetIndexByte((int)(num41 - 1 - 1))).GetPrice(true, BECCNBNGBMH.GetIndexByte((int)(num41 - (num42 + 1) - 1)), BECCNBNGBMH.GetIndexByte((int)(num41 - 1)));
							pIFKPLHIOFJ3.BPGEKNIINGF();
							num45 = (MGMMDGFPBLP + num41 + 1) & OODFDPMGBAB;
							uint num47 = num46 + OANEKNIFFCJ[(pIFKPLHIOFJ3.Index << 4) + num45].AJMPGCNGLHF();
							uint num48 = num47 + EGJKEMKOAGI[pIFKPLHIOFJ3.Index].AJMPGCNGLHF();
							uint num49 = num41 + 1 + num44;
							while (num9 < num18 + num49)
							{
								OHIALPKODDM[++num9].MDAAJFBENON = 268435455u;
							}
							num43 = num48 + LMFLOILOLDJ(0u, num44, pIFKPLHIOFJ3, num45);
							jDOABBOPFIP8 = OHIALPKODDM[num18 + num49];
							if (num43 < jDOABBOPFIP8.MDAAJFBENON)
							{
								jDOABBOPFIP8.MDAAJFBENON = num43;
								jDOABBOPFIP8.DFLODJBKHDN = num18 + num41 + 1;
								jDOABBOPFIP8.EKNEPEFHCIL = 0u;
								jDOABBOPFIP8.JFLGHPCMGPJ = true;
								jDOABBOPFIP8.DFFBJFJFNPO = true;
								jDOABBOPFIP8.PDDPBPPEEIL = num18;
								jDOABBOPFIP8.JFFHGEODLOA = num42 + 4;
							}
						}
					}
					num40 += 2;
					if (num40 == IBGEENFNMHL)
					{
						break;
					}
				}
				num41++;
			}
		}
		GBKOANCOMKF = IBGEENFNMHL;
		LMHPLGLKPDM = CEAHDKFDIOK2;
		FIMGEKNDHNG = true;
		return Backward(out PHJEECBMCFO, num18);
	}

	private bool ChangePair(uint NBNEODKIPFO, uint FKEJHBLHOBL)
	{
		return NBNEODKIPFO < 33554432 && FKEJHBLHOBL >= NBNEODKIPFO << 7;
	}

	private void CIBIBOLHHIB(uint LFOAILOHHHD)
	{
		if (PAILLFPPLBF)
		{
			OANEKNIFFCJ[(MAFFNGPOMJD.Index << 4) + LFOAILOHHHD].Encode(GAHJGFOBDEK, 1u);
			EGJKEMKOAGI[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 0u);
			MAFFNGPOMJD.HCMNGMEPJGM();
			uint num = 2u;
			OCKNLPLBGIH.Encode(GAHJGFOBDEK, num - 2, LFOAILOHHHD);
			uint iIFFPBLOKKC = 63u;
			uint num2 = Base.BBAEOHBBCHI(num);
			DEBLJELNEIA[num2].Encode(GAHJGFOBDEK, iIFFPBLOKKC);
			int num3 = 30;
			uint num4 = (uint)((1 << num3) - 1);
			GAHJGFOBDEK.LJKOCPDCLLK(num4 >> 4, num3 - 4);
			IKLHGFMBHPA.INFLKOLKKHG(GAHJGFOBDEK, num4 & 0xF);
		}
	}

	private void MKPBJGMJPMI(uint KGBFLBENJJG)
	{
		PLBGNBLOMGA();
		CIBIBOLHHIB(KGBFLBENJJG & OODFDPMGBAB);
		GAHJGFOBDEK.DMHMONMENHH();
		GAHJGFOBDEK.PDFBMGAJEHM();
	}

	public void CodeOneBlock(out long NCKELGLBGJN, out long JNILCBKONPG, out bool IAAOKDKLNGH)
	{
		NCKELGLBGJN = 0L;
		JNILCBKONPG = 0L;
		IAAOKDKLNGH = true;
		if (_inStream != null)
		{
			BECCNBNGBMH.SetStream(_inStream);
			BECCNBNGBMH.Init();
			NDEEGIDGFGK = true;
			_inStream = null;
			if (FFBNKMHPPMM != 0)
			{
				BECCNBNGBMH.Skip(FFBNKMHPPMM);
			}
		}
		if (EJDPCKCHJJL)
		{
			return;
		}
		EJDPCKCHJJL = true;
		long hMCMFCDHGIG = nowPos64;
		if (nowPos64 == 0)
		{
			if (BECCNBNGBMH.HBJMPBCHFJB() == 0)
			{
				MKPBJGMJPMI((uint)nowPos64);
				return;
			}
			uint CEAHDKFDIOK;
			uint IBGEENFNMHL;
			ReadMatchDistances(out CEAHDKFDIOK, out IBGEENFNMHL);
			uint num = (uint)(int)nowPos64 & OODFDPMGBAB;
			OANEKNIFFCJ[(MAFFNGPOMJD.Index << 4) + num].Encode(GAHJGFOBDEK, 0u);
			MAFFNGPOMJD.BPGEKNIINGF();
			byte b = BECCNBNGBMH.GetIndexByte((int)(0 - GEGHMBCKOOK));
			HFBHLKGNLEJ.MCLCHDNOONB((uint)nowPos64, _previousByte).Encode(GAHJGFOBDEK, b);
			_previousByte = b;
			GEGHMBCKOOK--;
			nowPos64++;
		}
		if (BECCNBNGBMH.HBJMPBCHFJB() == 0)
		{
			MKPBJGMJPMI((uint)nowPos64);
			return;
		}
		while (true)
		{
			uint PHJEECBMCFO;
			uint num2 = GetOptimum((uint)nowPos64, out PHJEECBMCFO);
			uint num3 = (uint)(int)nowPos64 & OODFDPMGBAB;
			uint num4 = (MAFFNGPOMJD.Index << 4) + num3;
			if (num2 == 1 && PHJEECBMCFO == uint.MaxValue)
			{
				OANEKNIFFCJ[num4].Encode(GAHJGFOBDEK, 0u);
				byte b2 = BECCNBNGBMH.GetIndexByte((int)(0 - GEGHMBCKOOK));
				HMMNGDPPEOO.EKANDKFGMGL eKANDKFGMGL = HFBHLKGNLEJ.MCLCHDNOONB((uint)nowPos64, _previousByte);
				if (!MAFFNGPOMJD.ALIFLOIMDFO())
				{
					byte hGMKIONDDNO = BECCNBNGBMH.GetIndexByte((int)(0 - CFFEKCKAGIN[0] - 1 - GEGHMBCKOOK));
					eKANDKFGMGL.IMEHLOFGAHF(GAHJGFOBDEK, hGMKIONDDNO, b2);
				}
				else
				{
					eKANDKFGMGL.Encode(GAHJGFOBDEK, b2);
				}
				_previousByte = b2;
				MAFFNGPOMJD.BPGEKNIINGF();
			}
			else
			{
				OANEKNIFFCJ[num4].Encode(GAHJGFOBDEK, 1u);
				if (PHJEECBMCFO < 4)
				{
					EGJKEMKOAGI[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 1u);
					if (PHJEECBMCFO == 0)
					{
						FMMIBCGBJFF[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 0u);
						if (num2 == 1)
						{
							DIPPHLGCMGI[num4].Encode(GAHJGFOBDEK, 0u);
						}
						else
						{
							DIPPHLGCMGI[num4].Encode(GAHJGFOBDEK, 1u);
						}
					}
					else
					{
						FMMIBCGBJFF[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 1u);
						if (PHJEECBMCFO == 1)
						{
							FEBICAJPIEN[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 0u);
						}
						else
						{
							FEBICAJPIEN[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 1u);
							MEOAPKPCCDJ[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, PHJEECBMCFO - 2);
						}
					}
					if (num2 == 1)
					{
						MAFFNGPOMJD.GGAGNGPBMIH();
					}
					else
					{
						LFNCDIHBEBP.Encode(GAHJGFOBDEK, num2 - 2, num3);
						MAFFNGPOMJD.EJIFGEACABJ();
					}
					uint num5 = CFFEKCKAGIN[PHJEECBMCFO];
					if (PHJEECBMCFO != 0)
					{
						for (uint num6 = PHJEECBMCFO; num6 >= 1; num6--)
						{
							CFFEKCKAGIN[num6] = CFFEKCKAGIN[num6 - 1];
						}
						CFFEKCKAGIN[0] = num5;
					}
				}
				else
				{
					EGJKEMKOAGI[MAFFNGPOMJD.Index].Encode(GAHJGFOBDEK, 0u);
					MAFFNGPOMJD.HCMNGMEPJGM();
					OCKNLPLBGIH.Encode(GAHJGFOBDEK, num2 - 2, num3);
					PHJEECBMCFO -= 4;
					uint num7 = BINLJKCMANH(PHJEECBMCFO);
					uint num8 = Base.BBAEOHBBCHI(num2);
					DEBLJELNEIA[num8].Encode(GAHJGFOBDEK, num7);
					if (num7 >= 4)
					{
						int num9 = (int)((num7 >> 1) - 1);
						uint num10 = (2 | (num7 & 1)) << num9;
						uint num11 = PHJEECBMCFO - num10;
						if (num7 < 14)
						{
							BitTreeEncoder.INFLKOLKKHG(ENCNCJDEKJP, num10 - num7 - 1, GAHJGFOBDEK, num9, num11);
						}
						else
						{
							GAHJGFOBDEK.LJKOCPDCLLK(num11 >> 4, num9 - 4);
							IKLHGFMBHPA.INFLKOLKKHG(GAHJGFOBDEK, num11 & 0xF);
							ENDOKKMJEBN++;
						}
					}
					uint num12 = PHJEECBMCFO;
					for (uint num13 = 3u; num13 >= 1; num13--)
					{
						CFFEKCKAGIN[num13] = CFFEKCKAGIN[num13 - 1];
					}
					CFFEKCKAGIN[0] = num12;
					DCLKLHGBGIA++;
				}
				_previousByte = BECCNBNGBMH.GetIndexByte((int)(num2 - 1 - GEGHMBCKOOK));
			}
			GEGHMBCKOOK -= num2;
			nowPos64 += num2;
			if (GEGHMBCKOOK == 0)
			{
				if (DCLKLHGBGIA >= 128)
				{
					AKJMJJFPDDO();
				}
				if (ENDOKKMJEBN >= 16)
				{
					EDALDEDKIOL();
				}
				NCKELGLBGJN = nowPos64;
				JNILCBKONPG = GAHJGFOBDEK.IAONGGCNFID();
				if (BECCNBNGBMH.HBJMPBCHFJB() == 0)
				{
					MKPBJGMJPMI((uint)nowPos64);
					return;
				}
				if (nowPos64 - hMCMFCDHGIG >= 4096)
				{
					break;
				}
			}
		}
		EJDPCKCHJJL = false;
		IAAOKDKLNGH = false;
	}

	private void PLBGNBLOMGA()
	{
		if (BECCNBNGBMH != null && NDEEGIDGFGK)
		{
			BECCNBNGBMH.IAIFCIAAHOE();
			NDEEGIDGFGK = false;
		}
	}

	private void NLINMCJOAML(Stream BBBGGJLOCPB)
	{
		GAHJGFOBDEK.SetStream(BBBGGJLOCPB);
	}

	private void BIMKENAFGBL()
	{
		GAHJGFOBDEK.IAIFCIAAHOE();
	}

	private void HIPBLGHMMBD()
	{
		PLBGNBLOMGA();
		BIMKENAFGBL();
	}

	private void SetStreams(Stream BHHJJHBNEKD, Stream BBBGGJLOCPB, long NCKELGLBGJN, long JNILCBKONPG)
	{
		_inStream = BHHJJHBNEKD;
		EJDPCKCHJJL = false;
		Create();
		NLINMCJOAML(BBBGGJLOCPB);
		Init();
		AKJMJJFPDDO();
		EDALDEDKIOL();
		OCKNLPLBGIH.NJBLBNKENAI(IAOEIFNEDDA + 1 - 2);
		OCKNLPLBGIH.OPPPKCDDGDA((uint)(1 << FPIFNNAMBMD));
		LFNCDIHBEBP.NJBLBNKENAI(IAOEIFNEDDA + 1 - 2);
		LFNCDIHBEBP.OPPPKCDDGDA((uint)(1 << FPIFNNAMBMD));
		nowPos64 = 0L;
	}

	public void EDEEELJMHLG(Stream BHHJJHBNEKD, Stream BBBGGJLOCPB, long NCKELGLBGJN, long JNILCBKONPG, ICodeProgress progress)
	{
		NDEEGIDGFGK = false;
		try
		{
			SetStreams(BHHJJHBNEKD, BBBGGJLOCPB, NCKELGLBGJN, JNILCBKONPG);
			while (true)
			{
				long NCKELGLBGJN2;
				long JNILCBKONPG2;
				bool IAAOKDKLNGH;
				CodeOneBlock(out NCKELGLBGJN2, out JNILCBKONPG2, out IAAOKDKLNGH);
				if (IAAOKDKLNGH)
				{
					break;
				}
				if (progress != null)
				{
					progress.LPOMOAKHBBA(NCKELGLBGJN2, JNILCBKONPG2);
				}
			}
		}
		finally
		{
			HIPBLGHMMBD();
		}
	}

	public void FGKHFOOJIGA(Stream BBBGGJLOCPB)
	{
		properties[0] = (byte)((FPIFNNAMBMD * 5 + IFMDMMDINPJ) * 9 + BNGMCPLAHJG);
		for (int i = 0; i < 4; i++)
		{
			properties[1 + i] = (byte)((MIGBGBGMDCJ >> 8 * i) & 0xFF);
		}
		BBBGGJLOCPB.Write(properties, 0, 5);
	}

	private void AKJMJJFPDDO()
	{
		for (uint num = 4u; num < 128; num++)
		{
			uint num2 = BINLJKCMANH(num);
			int num3 = (int)((num2 >> 1) - 1);
			uint num4 = (2 | (num2 & 1)) << num3;
			GIHLHAANGHC[num] = BitTreeEncoder.NCEFHMCLCPM(ENCNCJDEKJP, num4 - num2 - 1, num3, num - num4);
		}
		for (uint num5 = 0u; num5 < 4; num5++)
		{
			BitTreeEncoder fLKFKPEKKPD = DEBLJELNEIA[num5];
			uint num6 = num5 << 6;
			for (uint num7 = 0u; num7 < FCNOKIOBIEI; num7++)
			{
				JENEFGHKCFI[num6 + num7] = fLKFKPEKKPD.GetPrice(num7);
			}
			for (uint num7 = 14u; num7 < FCNOKIOBIEI; num7++)
			{
				JENEFGHKCFI[num6 + num7] += (num7 >> 1) - 1 - 4 << 6;
			}
			uint num8 = num5 * 128;
			uint num9;
			for (num9 = 0u; num9 < 4; num9++)
			{
				ACLJBILGHAA[num8 + num9] = JENEFGHKCFI[num6 + num9];
			}
			for (; num9 < 128; num9++)
			{
				ACLJBILGHAA[num8 + num9] = JENEFGHKCFI[num6 + BINLJKCMANH(num9)] + GIHLHAANGHC[num9];
			}
		}
		DCLKLHGBGIA = 0u;
	}

	private void EDALDEDKIOL()
	{
		for (uint num = 0u; num < 16; num++)
		{
			CAGKEHLNPGC[num] = IKLHGFMBHPA.NCEFHMCLCPM(num);
		}
		ENDOKKMJEBN = 0u;
	}

	private static int FindMatchFinder(string JDCCBCNFENK)
	{
		for (int i = 0; i < kMatchFinderIDs.Length; i++)
		{
			if (JDCCBCNFENK == kMatchFinderIDs[i])
			{
				return i;
			}
		}
		return -1;
	}

	public void KOKOGBHPOFA(LNHBEIOHMGB[] JPIKKLMCDNM, object[] properties)
	{
		for (uint num = 0u; num < properties.Length; num++)
		{
			object obj = properties[num];
			switch (JPIKKLMCDNM[num])
			{
			case LNHBEIOHMGB.NumFastBytes:
			{
				if (!(obj is int))
				{
					throw new KJJGIJBMJJG();
				}
				int num2 = (int)obj;
				if (num2 < 5 || (long)num2 > 273L)
				{
					throw new KJJGIJBMJJG();
				}
				IAOEIFNEDDA = (uint)num2;
				break;
			}
			case LNHBEIOHMGB.MatchFinder:
			{
				if (!(obj is string))
				{
					throw new KJJGIJBMJJG();
				}
				MFLEICEJPNC bFDNPAJNFFK = BFDNPAJNFFK;
				int num6 = FindMatchFinder(((string)obj).ToUpper());
				if (num6 < 0)
				{
					throw new KJJGIJBMJJG();
				}
				BFDNPAJNFFK = (MFLEICEJPNC)num6;
				if (BECCNBNGBMH != null && bFDNPAJNFFK != BFDNPAJNFFK)
				{
					LENLBJPOICN = uint.MaxValue;
					BECCNBNGBMH = null;
				}
				break;
			}
			case LNHBEIOHMGB.DictionarySize:
			{
				if (!(obj is int))
				{
					throw new KJJGIJBMJJG();
				}
				int num7 = (int)obj;
				if ((long)num7 < 1L || (long)num7 > 1073741824L)
				{
					throw new KJJGIJBMJJG();
				}
				MIGBGBGMDCJ = (uint)num7;
				int i;
				for (i = 0; (long)i < 30L && num7 > (uint)(1 << i); i++)
				{
				}
				FCNOKIOBIEI = (uint)(i * 2);
				break;
			}
			case LNHBEIOHMGB.PosStateBits:
			{
				if (!(obj is int))
				{
					throw new KJJGIJBMJJG();
				}
				int num3 = (int)obj;
				if (num3 < 0 || (long)num3 > 4L)
				{
					throw new KJJGIJBMJJG();
				}
				FPIFNNAMBMD = num3;
				OODFDPMGBAB = (uint)((1 << FPIFNNAMBMD) - 1);
				break;
			}
			case LNHBEIOHMGB.LitPosBits:
			{
				if (!(obj is int))
				{
					throw new KJJGIJBMJJG();
				}
				int num5 = (int)obj;
				if (num5 < 0 || (long)num5 > 4L)
				{
					throw new KJJGIJBMJJG();
				}
				IFMDMMDINPJ = num5;
				break;
			}
			case LNHBEIOHMGB.LitContextBits:
			{
				if (!(obj is int))
				{
					throw new KJJGIJBMJJG();
				}
				int num4 = (int)obj;
				if (num4 < 0 || (long)num4 > 8L)
				{
					throw new KJJGIJBMJJG();
				}
				BNGMCPLAHJG = num4;
				break;
			}
			case LNHBEIOHMGB.EndMarker:
				if (!(obj is bool))
				{
					throw new KJJGIJBMJJG();
				}
				SetWriteEndMarkerMode((bool)obj);
				break;
			default:
				throw new KJJGIJBMJJG();
			case LNHBEIOHMGB.Algorithm:
				break;
			}
		}
	}

	public void PFEOCEBDELA(uint CIGKDGKAADK)
	{
		FFBNKMHPPMM = CIGKDGKAADK;
	}
}
