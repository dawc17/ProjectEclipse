using System;

internal class OutputBuffer
{
	internal struct LHFANIPMGPA
	{
		internal int LCCLEFMKLPB;

		internal uint bitBuf;

		internal int EGEFBPOCGGN;
	}

	private byte[] byteBuffer;

	private int LCCLEFMKLPB;

	private uint bitBuf;

	private int EGEFBPOCGGN;

	internal int DPJJAFPABEE
	{
		get
		{
			return GEBLFKFACKO();
		}
	}

	internal int BMDIGKGNDPO
	{
		get
		{
			return JBPBBAEEAFO();
		}
	}

	internal int OHONOONEGAG
	{
		get
		{
			return DBBLKJPGAOO();
		}
	}

	internal void UpdateBuffer(byte[] output)
	{
		byteBuffer = output;
		LCCLEFMKLPB = 0;
	}

	internal int GEBLFKFACKO()
	{
		return LCCLEFMKLPB;
	}

	internal int JBPBBAEEAFO()
	{
		return byteBuffer.Length - LCCLEFMKLPB;
	}

	internal void WriteUInt16(ushort value)
	{
		byteBuffer[LCCLEFMKLPB++] = (byte)value;
		byteBuffer[LCCLEFMKLPB++] = (byte)(value >> 8);
	}

	internal void EHFDJAJPOAO(int HDKKKCDKFEE, uint HLFOKLCKNEE)
	{
		bitBuf |= HLFOKLCKNEE << EGEFBPOCGGN;
		EGEFBPOCGGN += HDKKKCDKFEE;
		if (EGEFBPOCGGN >= 16)
		{
			byteBuffer[LCCLEFMKLPB++] = (byte)bitBuf;
			byteBuffer[LCCLEFMKLPB++] = (byte)(bitBuf >> 8);
			EGEFBPOCGGN -= 16;
			bitBuf >>= 16;
		}
	}

	internal void NOOJGJGNLBL()
	{
		while (EGEFBPOCGGN >= 8)
		{
			byteBuffer[LCCLEFMKLPB++] = (byte)bitBuf;
			EGEFBPOCGGN -= 8;
			bitBuf >>= 8;
		}
		if (EGEFBPOCGGN > 0)
		{
			byteBuffer[LCCLEFMKLPB++] = (byte)bitBuf;
			bitBuf = 0u;
			EGEFBPOCGGN = 0;
		}
	}

	internal void FJPANBOJJDI(byte[] HFADMOEOHFA, int IPCOBJBKNAO, int count)
	{
		if (EGEFBPOCGGN == 0)
		{
			Array.Copy(HFADMOEOHFA, IPCOBJBKNAO, byteBuffer, LCCLEFMKLPB, count);
			LCCLEFMKLPB += count;
		}
		else
		{
			LELLIEHNDLB(HFADMOEOHFA, IPCOBJBKNAO, count);
		}
	}

	private void LELLIEHNDLB(byte[] HFADMOEOHFA, int IPCOBJBKNAO, int count)
	{
		for (int i = 0; i < count; i++)
		{
			byte aAOIAEJJINO = HFADMOEOHFA[IPCOBJBKNAO + i];
			WriteByteUnaligned(aAOIAEJJINO);
		}
	}

	private void WriteByteUnaligned(byte AAOIAEJJINO)
	{
		EHFDJAJPOAO(8, AAOIAEJJINO);
	}

	internal int DBBLKJPGAOO()
	{
		return EGEFBPOCGGN / 8 + 1;
	}

	internal LHFANIPMGPA ENBODKKOALL()
	{
		LHFANIPMGPA result = default(LHFANIPMGPA);
		result.LCCLEFMKLPB = LCCLEFMKLPB;
		result.bitBuf = bitBuf;
		result.EGEFBPOCGGN = EGEFBPOCGGN;
		return result;
	}

	internal void BIDLPPIPACF(LHFANIPMGPA state)
	{
		LCCLEFMKLPB = state.LCCLEFMKLPB;
		bitBuf = state.bitBuf;
		EGEFBPOCGGN = state.EGEFBPOCGGN;
	}
}
