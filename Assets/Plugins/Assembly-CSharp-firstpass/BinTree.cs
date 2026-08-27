using System;
using System.IO;

public class BinTree : InWindow, IInWindowStream, IMatchFinder
{
	private uint COJKJDCNPKK;

	private uint KHADNAIFHBC;

	private uint CLPBKMCAKNP;

	private uint[] MMBJNCGMDHC;

	private uint[] PIBKPHKCHGC;

	private uint PNKNDHJACDC = 255u;

	private uint EMOLEFJIIAA;

	private uint OMMLGEAAMHD;

	private bool HASH_ARRAY = true;

	private const uint AMIPEBFMAGJ = 1024u;

	private const uint OMAGGPMCIDK = 65536u;

	private const uint FGDFBHMBGJI = 65536u;

	private const uint ILACMFGNEGJ = 1u;

	private const uint FDKODIBJKEM = 1024u;

	private const uint ADPNCIIEHIN = 0u;

	private const uint KFLBBGNLNDH = 2147483647u;

	private uint ODGKKDLJDOE;

	private uint HFJCODDCHPN = 4u;

	private uint LOMAJDKHOPJ = 66560u;

	public void SetType(int EOKCENIBPJD)
	{
		HASH_ARRAY = EOKCENIBPJD > 2;
		if (HASH_ARRAY)
		{
			ODGKKDLJDOE = 0u;
			HFJCODDCHPN = 4u;
			LOMAJDKHOPJ = 66560u;
		}
		else
		{
			ODGKKDLJDOE = 2u;
			HFJCODDCHPN = 3u;
			LOMAJDKHOPJ = 0u;
		}
	}

	public new void SetStream(Stream ABJIEFMMIEK)
	{
		base.SetStream(ABJIEFMMIEK);
	}

	public new void IAIFCIAAHOE()
	{
		base.IAIFCIAAHOE();
	}

	public new void Init()
	{
		base.Init();
		for (uint num = 0u; num < OMMLGEAAMHD; num++)
		{
			PIBKPHKCHGC[num] = 0u;
		}
		COJKJDCNPKK = 0u;
		ReduceOffsets(-1);
	}

	public new void MHEJFMDCOHI()
	{
		if (++COJKJDCNPKK >= KHADNAIFHBC)
		{
			COJKJDCNPKK = 0u;
		}
		base.MHEJFMDCOHI();
		if (_pos == int.MaxValue)
		{
			NBDMEIKNJBG();
		}
	}

	public new byte GetIndexByte(int index)
	{
		return base.GetIndexByte(index);
	}

	public new uint GetMatchLen(int index, uint OIOMNNFMDOO, uint LOHCIKNKDEI)
	{
		return base.GetMatchLen(index, OIOMNNFMDOO, LOHCIKNKDEI);
	}

	public new uint HBJMPBCHFJB()
	{
		return base.HBJMPBCHFJB();
	}

	public void Create(uint PGNMIJNBAAJ, uint JHKHNGLLCLK, uint CCKFKNACIIN, uint CDINDGLFPKA)
	{
		if (PGNMIJNBAAJ > 2147483391)
		{
			throw new Exception();
		}
		PNKNDHJACDC = 16 + (CCKFKNACIIN >> 1);
		uint iKHIOAIPBNL = (PGNMIJNBAAJ + JHKHNGLLCLK + CCKFKNACIIN + CDINDGLFPKA) / 2 + 256;
		Create(PGNMIJNBAAJ + JHKHNGLLCLK, CCKFKNACIIN + CDINDGLFPKA, iKHIOAIPBNL);
		CLPBKMCAKNP = CCKFKNACIIN;
		uint num = PGNMIJNBAAJ + 1;
		if (KHADNAIFHBC != num)
		{
			MMBJNCGMDHC = new uint[(KHADNAIFHBC = num) * 2];
		}
		uint num2 = 65536u;
		if (HASH_ARRAY)
		{
			num2 = PGNMIJNBAAJ - 1;
			num2 |= num2 >> 1;
			num2 |= num2 >> 2;
			num2 |= num2 >> 4;
			num2 |= num2 >> 8;
			num2 >>= 1;
			num2 |= 0xFFFF;
			if (num2 > 16777216)
			{
				num2 >>= 1;
			}
			EMOLEFJIIAA = num2;
			num2++;
			num2 += LOMAJDKHOPJ;
		}
		if (num2 != OMMLGEAAMHD)
		{
			PIBKPHKCHGC = new uint[OMMLGEAAMHD = num2];
		}
	}

	public uint GetMatches(uint[] PIPLHPNGIPF)
	{
		uint num;
		if (_pos + CLPBKMCAKNP <= OONMJHCFEHO)
		{
			num = CLPBKMCAKNP;
		}
		else
		{
			num = OONMJHCFEHO - _pos;
			if (num < HFJCODDCHPN)
			{
				MHEJFMDCOHI();
				return 0u;
			}
		}
		uint num2 = 0u;
		uint num3 = ((_pos > KHADNAIFHBC) ? (_pos - KHADNAIFHBC) : 0u);
		uint num4 = HGGJBAEEKJN + _pos;
		uint num5 = 1u;
		uint num6 = 0u;
		uint num7 = 0u;
		uint num9;
		if (HASH_ARRAY)
		{
			uint num8 = CRC.Table[_bufferBase[num4]] ^ _bufferBase[num4 + 1];
			num6 = num8 & 0x3FF;
			num8 ^= (uint)(_bufferBase[num4 + 2] << 8);
			num7 = num8 & 0xFFFF;
			num9 = (num8 ^ (CRC.Table[_bufferBase[num4 + 3]] << 5)) & EMOLEFJIIAA;
		}
		else
		{
			num9 = (uint)(_bufferBase[num4] ^ (_bufferBase[num4 + 1] << 8));
		}
		uint num10 = PIBKPHKCHGC[LOMAJDKHOPJ + num9];
		if (HASH_ARRAY)
		{
			uint num11 = PIBKPHKCHGC[num6];
			uint num12 = PIBKPHKCHGC[1024 + num7];
			PIBKPHKCHGC[num6] = _pos;
			PIBKPHKCHGC[1024 + num7] = _pos;
			if (num11 > num3 && _bufferBase[HGGJBAEEKJN + num11] == _bufferBase[num4])
			{
				num5 = (PIPLHPNGIPF[num2++] = 2u);
				PIPLHPNGIPF[num2++] = _pos - num11 - 1;
			}
			if (num12 > num3 && _bufferBase[HGGJBAEEKJN + num12] == _bufferBase[num4])
			{
				if (num12 == num11)
				{
					num2 -= 2;
				}
				num5 = (PIPLHPNGIPF[num2++] = 3u);
				PIPLHPNGIPF[num2++] = _pos - num12 - 1;
				num11 = num12;
			}
			if (num2 != 0 && num11 == num10)
			{
				num2 -= 2;
				num5 = 1u;
			}
		}
		PIBKPHKCHGC[LOMAJDKHOPJ + num9] = _pos;
		uint num13 = (COJKJDCNPKK << 1) + 1;
		uint num14 = COJKJDCNPKK << 1;
		uint val2;
		uint val = (val2 = ODGKKDLJDOE);
		if (ODGKKDLJDOE != 0 && num10 > num3 && _bufferBase[HGGJBAEEKJN + num10 + ODGKKDLJDOE] != _bufferBase[num4 + ODGKKDLJDOE])
		{
			num5 = (PIPLHPNGIPF[num2++] = ODGKKDLJDOE);
			PIPLHPNGIPF[num2++] = _pos - num10 - 1;
		}
		uint pNKNDHJACDC = PNKNDHJACDC;
		while (true)
		{
			if (num10 <= num3 || pNKNDHJACDC-- == 0)
			{
				MMBJNCGMDHC[num13] = (MMBJNCGMDHC[num14] = 0u);
				break;
			}
			uint num15 = _pos - num10;
			uint num16 = ((num15 > COJKJDCNPKK) ? (COJKJDCNPKK - num15 + KHADNAIFHBC) : (COJKJDCNPKK - num15)) << 1;
			uint num17 = HGGJBAEEKJN + num10;
			uint num18 = Math.Min(val, val2);
			if (_bufferBase[num17 + num18] == _bufferBase[num4 + num18])
			{
				while (++num18 != num && _bufferBase[num17 + num18] == _bufferBase[num4 + num18])
				{
				}
				if (num5 < num18)
				{
					num5 = (PIPLHPNGIPF[num2++] = num18);
					PIPLHPNGIPF[num2++] = num15 - 1;
					if (num18 == num)
					{
						MMBJNCGMDHC[num14] = MMBJNCGMDHC[num16];
						MMBJNCGMDHC[num13] = MMBJNCGMDHC[num16 + 1];
						break;
					}
				}
			}
			if (_bufferBase[num17 + num18] < _bufferBase[num4 + num18])
			{
				MMBJNCGMDHC[num14] = num10;
				num14 = num16 + 1;
				num10 = MMBJNCGMDHC[num14];
				val2 = num18;
			}
			else
			{
				MMBJNCGMDHC[num13] = num10;
				num13 = num16;
				num10 = MMBJNCGMDHC[num13];
				val = num18;
			}
		}
		MHEJFMDCOHI();
		return num2;
	}

	public void Skip(uint OMEDGJMNGKE)
	{
		do
		{
			uint num;
			if (_pos + CLPBKMCAKNP <= OONMJHCFEHO)
			{
				num = CLPBKMCAKNP;
			}
			else
			{
				num = OONMJHCFEHO - _pos;
				if (num < HFJCODDCHPN)
				{
					MHEJFMDCOHI();
					continue;
				}
			}
			uint num2 = ((_pos > KHADNAIFHBC) ? (_pos - KHADNAIFHBC) : 0u);
			uint num3 = HGGJBAEEKJN + _pos;
			uint num7;
			if (HASH_ARRAY)
			{
				uint num4 = CRC.Table[_bufferBase[num3]] ^ _bufferBase[num3 + 1];
				uint num5 = num4 & 0x3FF;
				PIBKPHKCHGC[num5] = _pos;
				num4 ^= (uint)(_bufferBase[num3 + 2] << 8);
				uint num6 = num4 & 0xFFFF;
				PIBKPHKCHGC[1024 + num6] = _pos;
				num7 = (num4 ^ (CRC.Table[_bufferBase[num3 + 3]] << 5)) & EMOLEFJIIAA;
			}
			else
			{
				num7 = (uint)(_bufferBase[num3] ^ (_bufferBase[num3 + 1] << 8));
			}
			uint num8 = PIBKPHKCHGC[LOMAJDKHOPJ + num7];
			PIBKPHKCHGC[LOMAJDKHOPJ + num7] = _pos;
			uint num9 = (COJKJDCNPKK << 1) + 1;
			uint num10 = COJKJDCNPKK << 1;
			uint val2;
			uint val = (val2 = ODGKKDLJDOE);
			uint pNKNDHJACDC = PNKNDHJACDC;
			while (true)
			{
				if (num8 <= num2 || pNKNDHJACDC-- == 0)
				{
					MMBJNCGMDHC[num9] = (MMBJNCGMDHC[num10] = 0u);
					break;
				}
				uint num11 = _pos - num8;
				uint num12 = ((num11 > COJKJDCNPKK) ? (COJKJDCNPKK - num11 + KHADNAIFHBC) : (COJKJDCNPKK - num11)) << 1;
				uint num13 = HGGJBAEEKJN + num8;
				uint num14 = Math.Min(val, val2);
				if (_bufferBase[num13 + num14] == _bufferBase[num3 + num14])
				{
					while (++num14 != num && _bufferBase[num13 + num14] == _bufferBase[num3 + num14])
					{
					}
					if (num14 == num)
					{
						MMBJNCGMDHC[num10] = MMBJNCGMDHC[num12];
						MMBJNCGMDHC[num9] = MMBJNCGMDHC[num12 + 1];
						break;
					}
				}
				if (_bufferBase[num13 + num14] < _bufferBase[num3 + num14])
				{
					MMBJNCGMDHC[num10] = num8;
					num10 = num12 + 1;
					num8 = MMBJNCGMDHC[num10];
					val2 = num14;
				}
				else
				{
					MMBJNCGMDHC[num9] = num8;
					num9 = num12;
					num8 = MMBJNCGMDHC[num9];
					val = num14;
				}
			}
			MHEJFMDCOHI();
		}
		while (--OMEDGJMNGKE != 0);
	}

	private void NormalizeLinks(uint[] HELFDCAIJNE, uint DDLKICOHOGG, uint BALBEBAOPMP)
	{
		for (uint num = 0u; num < DDLKICOHOGG; num++)
		{
			uint num2 = HELFDCAIJNE[num];
			num2 = ((num2 > BALBEBAOPMP) ? (num2 - BALBEBAOPMP) : 0u);
			HELFDCAIJNE[num] = num2;
		}
	}

	private void NBDMEIKNJBG()
	{
		uint bALBEBAOPMP = _pos - KHADNAIFHBC;
		NormalizeLinks(MMBJNCGMDHC, KHADNAIFHBC * 2, bALBEBAOPMP);
		NormalizeLinks(PIBKPHKCHGC, OMMLGEAAMHD, bALBEBAOPMP);
		ReduceOffsets((int)bALBEBAOPMP);
	}

	public void BFILFJNGNNP(uint PADNFMPEFDM)
	{
		PNKNDHJACDC = PADNFMPEFDM;
	}
}
