using System;

internal sealed class ZlibCodec
{
	public byte[] PEFOCMDODLD;

	public int LMIPBGGILEJ;

	public int IAPJEIDMGNP;

	public long ALJBBHPGGPA;

	public byte[] DKCGBABIAEN;

	public int EIBFDELHKNM;

	public int NBNGINIIKNA;

	public long HCDKLJJLMOD;

	public string Message;

	internal DeflateManager FGOHAMANMMM;

	internal InflateManager LLBAFBNPCGH;

	internal uint _Adler32;

	public NKFKKGNBHDK EKAHOIBGJIH = NKFKKGNBHDK.Default;

	public int MHFEKEJCDAG = 15;

	public DDGGLIIKFPL JKDDDFMLLMI;

	public int IAJPFDALGJM
	{
		get
		{
			return NIJPNPDMMMH();
		}
	}

	public ZlibCodec()
	{
	}

	public ZlibCodec(KAOCBBMMFOG NMMPBADCFHK)
	{
		switch (NMMPBADCFHK)
		{
		case KAOCBBMMFOG.Compress:
			if (JCBLHDMMDAB() != 0)
			{
				throw new ZlibException("Cannot initialize for deflate.");
			}
			break;
		case KAOCBBMMFOG.Decompress:
			if (InitializeInflate() != 0)
			{
				throw new ZlibException("Cannot initialize for inflate.");
			}
			break;
		default:
			throw new ZlibException("Invalid ZlibStreamFlavor.");
		}
	}

	public int NIJPNPDMMMH()
	{
		return (int)_Adler32;
	}

	public int InitializeInflate()
	{
		return InitializeInflate(MHFEKEJCDAG);
	}

	public int InitializeInflate(bool EKEOIGPLABK)
	{
		return InitializeInflate(MHFEKEJCDAG, EKEOIGPLABK);
	}

	public int InitializeInflate(int KGFELFAKFIA)
	{
		MHFEKEJCDAG = KGFELFAKFIA;
		return InitializeInflate(KGFELFAKFIA, true);
	}

	public int InitializeInflate(int KGFELFAKFIA, bool EKEOIGPLABK)
	{
		MHFEKEJCDAG = KGFELFAKFIA;
		if (FGOHAMANMMM != null)
		{
			throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
		}
		LLBAFBNPCGH = new InflateManager(EKEOIGPLABK);
		return LLBAFBNPCGH.EHAJODIAFEG(this, KGFELFAKFIA);
	}

	public int Inflate(AFJHGKAEJPG NGBJDNFAPKC)
	{
		if (LLBAFBNPCGH == null)
		{
			throw new ZlibException("No Inflate State!");
		}
		return LLBAFBNPCGH.Inflate(NGBJDNFAPKC);
	}

	public int LGGKOHICFEE()
	{
		if (LLBAFBNPCGH == null)
		{
			throw new ZlibException("No Inflate State!");
		}
		int result = LLBAFBNPCGH.PLHPGFGAGKJ();
		LLBAFBNPCGH = null;
		return result;
	}

	public int IMBNNOOKGCJ()
	{
		if (LLBAFBNPCGH == null)
		{
			throw new ZlibException("No Inflate State!");
		}
		return LLBAFBNPCGH.JGCOKJJDLBC();
	}

	public int JCBLHDMMDAB()
	{
		return JDDDCFAAAHO(true);
	}

	public int JCBLHDMMDAB(NKFKKGNBHDK GNLOCMLBNHF)
	{
		EKAHOIBGJIH = GNLOCMLBNHF;
		return JDDDCFAAAHO(true);
	}

	public int JCBLHDMMDAB(NKFKKGNBHDK GNLOCMLBNHF, bool JIHPEOOBCBG)
	{
		EKAHOIBGJIH = GNLOCMLBNHF;
		return JDDDCFAAAHO(JIHPEOOBCBG);
	}

	public int JCBLHDMMDAB(NKFKKGNBHDK GNLOCMLBNHF, int HLFOKLCKNEE)
	{
		EKAHOIBGJIH = GNLOCMLBNHF;
		MHFEKEJCDAG = HLFOKLCKNEE;
		return JDDDCFAAAHO(true);
	}

	public int JCBLHDMMDAB(NKFKKGNBHDK GNLOCMLBNHF, int HLFOKLCKNEE, bool JIHPEOOBCBG)
	{
		EKAHOIBGJIH = GNLOCMLBNHF;
		MHFEKEJCDAG = HLFOKLCKNEE;
		return JDDDCFAAAHO(JIHPEOOBCBG);
	}

	private int JDDDCFAAAHO(bool JIHPEOOBCBG)
	{
		if (LLBAFBNPCGH != null)
		{
			throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
		}
		FGOHAMANMMM = new DeflateManager();
		FGOHAMANMMM.NGEBPALKODO(JIHPEOOBCBG);
		return FGOHAMANMMM.EHAJODIAFEG(this, EKAHOIBGJIH, MHFEKEJCDAG, JKDDDFMLLMI);
	}

	public int GAMMFNJHCFO(AFJHGKAEJPG NGBJDNFAPKC)
	{
		if (FGOHAMANMMM == null)
		{
			throw new ZlibException("No Deflate State!");
		}
		return FGOHAMANMMM.GAMMFNJHCFO(NGBJDNFAPKC);
	}

	public int GPBPBEHKNEO()
	{
		if (FGOHAMANMMM == null)
		{
			throw new ZlibException("No Deflate State!");
		}
		FGOHAMANMMM = null;
		return 0;
	}

	public void CCBAMHBALNO()
	{
		if (FGOHAMANMMM == null)
		{
			throw new ZlibException("No Deflate State!");
		}
		FGOHAMANMMM.Reset();
	}

	public int HGDGKLGCFGL(NKFKKGNBHDK GNLOCMLBNHF, DDGGLIIKFPL FNLGJNHJCPL)
	{
		if (FGOHAMANMMM == null)
		{
			throw new ZlibException("No Deflate State!");
		}
		return FGOHAMANMMM.HBFLMIBKBBF(GNLOCMLBNHF, FNLGJNHJCPL);
	}

	public int SetDictionary(byte[] dictionary)
	{
		if (LLBAFBNPCGH != null)
		{
			return LLBAFBNPCGH.SetDictionary(dictionary);
		}
		if (FGOHAMANMMM != null)
		{
			return FGOHAMANMMM.SetDictionary(dictionary);
		}
		throw new ZlibException("No Inflate or Deflate state!");
	}

	internal void CHAPNKCGONG()
	{
		int num = FGOHAMANMMM.CCDNPCJKGGK;
		if (num > NBNGINIIKNA)
		{
			num = NBNGINIIKNA;
		}
		if (num != 0)
		{
			if (FGOHAMANMMM.BMPCIFFMMPP.Length <= FGOHAMANMMM.GBKNGFEBIOL || DKCGBABIAEN.Length <= EIBFDELHKNM || FGOHAMANMMM.BMPCIFFMMPP.Length < FGOHAMANMMM.GBKNGFEBIOL + num || DKCGBABIAEN.Length < EIBFDELHKNM + num)
			{
				throw new ZlibException(string.Format("Invalid State. (pending.Length={0}, pendingCount={1})", FGOHAMANMMM.BMPCIFFMMPP.Length, FGOHAMANMMM.CCDNPCJKGGK));
			}
			Array.Copy(FGOHAMANMMM.BMPCIFFMMPP, FGOHAMANMMM.GBKNGFEBIOL, DKCGBABIAEN, EIBFDELHKNM, num);
			EIBFDELHKNM += num;
			FGOHAMANMMM.GBKNGFEBIOL += num;
			HCDKLJJLMOD += num;
			NBNGINIIKNA -= num;
			FGOHAMANMMM.CCDNPCJKGGK -= num;
			if (FGOHAMANMMM.CCDNPCJKGGK == 0)
			{
				FGOHAMANMMM.GBKNGFEBIOL = 0;
			}
		}
	}

	internal int read_buf(byte[] HLDLIFPJMOA, int ILENLCMAMBH, int PEEOEOMEBFG)
	{
		int num = IAPJEIDMGNP;
		if (num > PEEOEOMEBFG)
		{
			num = PEEOEOMEBFG;
		}
		if (num == 0)
		{
			return 0;
		}
		IAPJEIDMGNP -= num;
		if (FGOHAMANMMM.GFOKPNKCOOP())
		{
			_Adler32 = Adler.IAJPFDALGJM(_Adler32, PEFOCMDODLD, LMIPBGGILEJ, num);
		}
		Array.Copy(PEFOCMDODLD, LMIPBGGILEJ, HLDLIFPJMOA, ILENLCMAMBH, num);
		LMIPBGGILEJ += num;
		ALJBBHPGGPA += num;
		return num;
	}
}
