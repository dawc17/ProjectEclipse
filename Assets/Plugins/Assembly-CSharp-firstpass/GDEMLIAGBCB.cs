using System;
using System.IO;

public class GDEMLIAGBCB : ICoder, ISetDecoderProperties
{
	private class DGDNGEBMJLB
	{
		private BitDecoder BGPNNOFFAJG = default(BitDecoder);

		private BitDecoder NKHJHGIACBM = default(BitDecoder);

		private BitTreeDecoder[] HMFLPPFGBGD = new BitTreeDecoder[16];

		private BitTreeDecoder[] MGHHELIBDCF = new BitTreeDecoder[16];

		private BitTreeDecoder MKOBOBCOAKI = new BitTreeDecoder(8);

		private uint m_NumPosStates;

		public void Create(uint BHMGNFOKODN)
		{
			for (uint num = m_NumPosStates; num < BHMGNFOKODN; num++)
			{
				HMFLPPFGBGD[num] = new BitTreeDecoder(3);
				MGHHELIBDCF[num] = new BitTreeDecoder(3);
			}
			m_NumPosStates = BHMGNFOKODN;
		}

		public void Init()
		{
			BGPNNOFFAJG.Init();
			for (uint num = 0u; num < m_NumPosStates; num++)
			{
				HMFLPPFGBGD[num].Init();
				MGHHELIBDCF[num].Init();
			}
			NKHJHGIACBM.Init();
			MKOBOBCOAKI.Init();
		}

		public uint Decode(CEILAGAKGKF HELKEOGALEA, uint LFOAILOHHHD)
		{
			if (BGPNNOFFAJG.Decode(HELKEOGALEA) == 0)
			{
				return HMFLPPFGBGD[LFOAILOHHHD].Decode(HELKEOGALEA);
			}
			uint num = 8u;
			if (NKHJHGIACBM.Decode(HELKEOGALEA) == 0)
			{
				return num + MGHHELIBDCF[LFOAILOHHHD].Decode(HELKEOGALEA);
			}
			num += 8;
			return num + MKOBOBCOAKI.Decode(HELKEOGALEA);
		}
	}

	private class FNLKJODGOBN
	{
		private struct OHICAENPGNO
		{
			private BitDecoder[] FOKFLMNPACI;

			public void Create()
			{
				FOKFLMNPACI = new BitDecoder[768];
			}

			public void Init()
			{
				for (int i = 0; i < 768; i++)
				{
					FOKFLMNPACI[i].Init();
				}
			}

			public byte FDOKAOIHCCI(CEILAGAKGKF HELKEOGALEA)
			{
				uint num = 1u;
				do
				{
					num = (num << 1) | FOKFLMNPACI[num].Decode(HELKEOGALEA);
				}
				while (num < 256);
				return (byte)num;
			}

			public byte NMGENOEIAHE(CEILAGAKGKF HELKEOGALEA, byte HGMKIONDDNO)
			{
				uint num = 1u;
				do
				{
					uint num2 = (uint)((HGMKIONDDNO >> 7) & 1);
					HGMKIONDDNO <<= 1;
					uint num3 = FOKFLMNPACI[(1 + num2 << 8) + num].Decode(HELKEOGALEA);
					num = (num << 1) | num3;
					if (num2 != num3)
					{
						while (num < 256)
						{
							num = (num << 1) | FOKFLMNPACI[num].Decode(HELKEOGALEA);
						}
						break;
					}
				}
				while (num < 256);
				return (byte)num;
			}
		}

		private OHICAENPGNO[] NJGENNBJGNJ;

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
				NJGENNBJGNJ = new OHICAENPGNO[num];
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

		private uint GetState(uint LCCLEFMKLPB, byte PMEIMKDGNJP)
		{
			return ((LCCLEFMKLPB & m_PosMask) << BPMAHCNJIGH) + (uint)(PMEIMKDGNJP >> 8 - BPMAHCNJIGH);
		}

		public byte FDOKAOIHCCI(CEILAGAKGKF HELKEOGALEA, uint LCCLEFMKLPB, byte PMEIMKDGNJP)
		{
			return NJGENNBJGNJ[GetState(LCCLEFMKLPB, PMEIMKDGNJP)].FDOKAOIHCCI(HELKEOGALEA);
		}

		public byte NMGENOEIAHE(CEILAGAKGKF HELKEOGALEA, uint LCCLEFMKLPB, byte PMEIMKDGNJP, byte HGMKIONDDNO)
		{
			return NJGENNBJGNJ[GetState(LCCLEFMKLPB, PMEIMKDGNJP)].NMGENOEIAHE(HELKEOGALEA, HGMKIONDDNO);
		}
	}

	private OutWindow ODBFGFPEPKK = new OutWindow();

	private CEILAGAKGKF IBNCMNNJKOJ = new CEILAGAKGKF();

	private BitDecoder[] CKJMEDOJIGI = new BitDecoder[192];

	private BitDecoder[] FOBHCPJMHNN = new BitDecoder[12];

	private BitDecoder[] NFMKGHFGBAO = new BitDecoder[12];

	private BitDecoder[] EHBFDKMHONH = new BitDecoder[12];

	private BitDecoder[] BKHONELNCPD = new BitDecoder[12];

	private BitDecoder[] MDJCJKHKOJA = new BitDecoder[192];

	private BitTreeDecoder[] NLGDGDJKMIE = new BitTreeDecoder[4];

	private BitDecoder[] OJHIIOEGCHF = new BitDecoder[114];

	private BitTreeDecoder EFAGHHMNPAJ = new BitTreeDecoder(4);

	private DGDNGEBMJLB PHJCMEDFNIG = new DGDNGEBMJLB();

	private DGDNGEBMJLB PMHBPEAAJGM = new DGDNGEBMJLB();

	private FNLKJODGOBN DIKLPNFHHHF = new FNLKJODGOBN();

	private uint GHLDMOGIBBB;

	private uint KFGNNPEIOJC;

	private uint PAGIAOFKEMN;

	private bool _solid;

	public GDEMLIAGBCB()
	{
		GHLDMOGIBBB = uint.MaxValue;
		for (int i = 0; (long)i < 4L; i++)
		{
			NLGDGDJKMIE[i] = new BitTreeDecoder(6);
		}
	}

	private void SetDictionarySize(uint MMNLJFOCACJ)
	{
		if (GHLDMOGIBBB != MMNLJFOCACJ)
		{
			GHLDMOGIBBB = MMNLJFOCACJ;
			KFGNNPEIOJC = Math.Max(GHLDMOGIBBB, 1u);
			uint aKOEOKJFINO = Math.Max(KFGNNPEIOJC, 4096u);
			ODBFGFPEPKK.Create(aKOEOKJFINO);
		}
	}

	private void SetLiteralProperties(int MHNGHOGBEAE, int LMHBHHENKHG)
	{
		if (MHNGHOGBEAE > 8)
		{
			throw new KJJGIJBMJJG();
		}
		if (LMHBHHENKHG > 8)
		{
			throw new KJJGIJBMJJG();
		}
		DIKLPNFHHHF.Create(MHNGHOGBEAE, LMHBHHENKHG);
	}

	private void SetPosBitsProperties(int LMJOLGGBKNL)
	{
		if (LMJOLGGBKNL > 4)
		{
			throw new KJJGIJBMJJG();
		}
		uint num = (uint)(1 << LMJOLGGBKNL);
		PHJCMEDFNIG.Create(num);
		PMHBPEAAJGM.Create(num);
		PAGIAOFKEMN = num - 1;
	}

	private void Init(Stream BHHJJHBNEKD, Stream BBBGGJLOCPB)
	{
		IBNCMNNJKOJ.Init(BHHJJHBNEKD);
		ODBFGFPEPKK.Init(BBBGGJLOCPB, _solid);
		for (uint num = 0u; num < 12; num++)
		{
			for (uint num2 = 0u; num2 <= PAGIAOFKEMN; num2++)
			{
				uint num3 = (num << 4) + num2;
				CKJMEDOJIGI[num3].Init();
				MDJCJKHKOJA[num3].Init();
			}
			FOBHCPJMHNN[num].Init();
			NFMKGHFGBAO[num].Init();
			EHBFDKMHONH[num].Init();
			BKHONELNCPD[num].Init();
		}
		DIKLPNFHHHF.Init();
		for (uint num = 0u; num < 4; num++)
		{
			NLGDGDJKMIE[num].Init();
		}
		for (uint num = 0u; num < 114; num++)
		{
			OJHIIOEGCHF[num].Init();
		}
		PHJCMEDFNIG.Init();
		PMHBPEAAJGM.Init();
		EFAGHHMNPAJ.Init();
	}

	public void EDEEELJMHLG(Stream BHHJJHBNEKD, Stream BBBGGJLOCPB, long NCKELGLBGJN, long JNILCBKONPG, ICodeProgress progress)
	{
		Init(BHHJJHBNEKD, BBBGGJLOCPB);
		Base.IPAFOKKOCPF iPAFOKKOCPF = default(Base.IPAFOKKOCPF);
		iPAFOKKOCPF.Init();
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 0u;
		ulong num5 = 0uL;
		if (num5 < (ulong)JNILCBKONPG)
		{
			if (CKJMEDOJIGI[iPAFOKKOCPF.Index << 4].Decode(IBNCMNNJKOJ) != 0)
			{
				throw new CHEMCBIKGLL();
			}
			iPAFOKKOCPF.BPGEKNIINGF();
			byte aAOIAEJJINO = DIKLPNFHHHF.FDOKAOIHCCI(IBNCMNNJKOJ, 0u, 0);
			ODBFGFPEPKK.PutByte(aAOIAEJJINO);
			num5++;
		}
		while (num5 < (ulong)JNILCBKONPG)
		{
			uint num6 = (uint)(int)num5 & PAGIAOFKEMN;
			if (CKJMEDOJIGI[(iPAFOKKOCPF.Index << 4) + num6].Decode(IBNCMNNJKOJ) == 0)
			{
				byte pMEIMKDGNJP = ODBFGFPEPKK.GetByte(0u);
				byte aAOIAEJJINO2 = (iPAFOKKOCPF.ALIFLOIMDFO() ? DIKLPNFHHHF.FDOKAOIHCCI(IBNCMNNJKOJ, (uint)num5, pMEIMKDGNJP) : DIKLPNFHHHF.NMGENOEIAHE(IBNCMNNJKOJ, (uint)num5, pMEIMKDGNJP, ODBFGFPEPKK.GetByte(num)));
				ODBFGFPEPKK.PutByte(aAOIAEJJINO2);
				iPAFOKKOCPF.BPGEKNIINGF();
				num5++;
				continue;
			}
			uint num8;
			if (FOBHCPJMHNN[iPAFOKKOCPF.Index].Decode(IBNCMNNJKOJ) == 1)
			{
				if (NFMKGHFGBAO[iPAFOKKOCPF.Index].Decode(IBNCMNNJKOJ) == 0)
				{
					if (MDJCJKHKOJA[(iPAFOKKOCPF.Index << 4) + num6].Decode(IBNCMNNJKOJ) == 0)
					{
						iPAFOKKOCPF.GGAGNGPBMIH();
						ODBFGFPEPKK.PutByte(ODBFGFPEPKK.GetByte(num));
						num5++;
						continue;
					}
				}
				else
				{
					uint num7;
					if (EHBFDKMHONH[iPAFOKKOCPF.Index].Decode(IBNCMNNJKOJ) == 0)
					{
						num7 = num2;
					}
					else
					{
						if (BKHONELNCPD[iPAFOKKOCPF.Index].Decode(IBNCMNNJKOJ) == 0)
						{
							num7 = num3;
						}
						else
						{
							num7 = num4;
							num4 = num3;
						}
						num3 = num2;
					}
					num2 = num;
					num = num7;
				}
				num8 = PMHBPEAAJGM.Decode(IBNCMNNJKOJ, num6) + 2;
				iPAFOKKOCPF.EJIFGEACABJ();
			}
			else
			{
				num4 = num3;
				num3 = num2;
				num2 = num;
				num8 = 2 + PHJCMEDFNIG.Decode(IBNCMNNJKOJ, num6);
				iPAFOKKOCPF.HCMNGMEPJGM();
				uint num9 = NLGDGDJKMIE[Base.BBAEOHBBCHI(num8)].Decode(IBNCMNNJKOJ);
				if (num9 >= 4)
				{
					int num10 = (int)((num9 >> 1) - 1);
					num = (2 | (num9 & 1)) << num10;
					if (num9 < 14)
					{
						num += BitTreeDecoder.ACNFPHDBCPC(OJHIIOEGCHF, num - num9 - 1, IBNCMNNJKOJ, num10);
					}
					else
					{
						num += IBNCMNNJKOJ.DecodeDirectBits(num10 - 4) << 4;
						num += EFAGHHMNPAJ.ACNFPHDBCPC(IBNCMNNJKOJ);
					}
				}
				else
				{
					num = num9;
				}
			}
			if (num >= ODBFGFPEPKK.FAJMEIBMEDF + num5 || num >= KFGNNPEIOJC)
			{
				if (num == uint.MaxValue)
				{
					break;
				}
				throw new CHEMCBIKGLL();
			}
			ODBFGFPEPKK.CopyBlock(num, num8);
			num5 += num8;
		}
		ODBFGFPEPKK.MKPBJGMJPMI();
		ODBFGFPEPKK.IAIFCIAAHOE();
		IBNCMNNJKOJ.IAIFCIAAHOE();
	}

	public void SetDecoderProperties(byte[] properties)
	{
		if (properties.Length < 5)
		{
			throw new KJJGIJBMJJG();
		}
		int lMHBHHENKHG = properties[0] % 9;
		int num = properties[0] / 9;
		int mHNGHOGBEAE = num % 5;
		int num2 = num / 5;
		if (num2 > 4)
		{
			throw new KJJGIJBMJJG();
		}
		uint num3 = 0u;
		for (int i = 0; i < 4; i++)
		{
			num3 += (uint)(properties[1 + i] << i * 8);
		}
		SetDictionarySize(num3);
		SetLiteralProperties(mHNGHOGBEAE, lMHBHHENKHG);
		SetPosBitsProperties(num2);
	}

	public bool Train(Stream ABJIEFMMIEK)
	{
		_solid = true;
		return ODBFGFPEPKK.Train(ABJIEFMMIEK);
	}
}
