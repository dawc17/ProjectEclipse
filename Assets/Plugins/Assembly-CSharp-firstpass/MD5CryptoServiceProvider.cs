using System;
using System.Runtime.InteropServices;

[ComVisible(true)]
public sealed class MD5CryptoServiceProvider : MD5
{
	private const int BLOCK_SIZE_BYTES = 64;

	private uint[] DHJDOAPANCD;

	private uint[] MOLMIGAIEPN;

	private ulong count;

	private byte[] _ProcessingBuffer;

	private int _ProcessingBufferCount;

	private static readonly uint[] K = new uint[64]
	{
		3614090360u, 3905402710u, 606105819u, 3250441966u, 4118548399u, 1200080426u, 2821735955u, 4249261313u, 1770035416u, 2336552879u,
		4294925233u, 2304563134u, 1804603682u, 4254626195u, 2792965006u, 1236535329u, 4129170786u, 3225465664u, 643717713u, 3921069994u,
		3593408605u, 38016083u, 3634488961u, 3889429448u, 568446438u, 3275163606u, 4107603335u, 1163531501u, 2850285829u, 4243563512u,
		1735328473u, 2368359562u, 4294588738u, 2272392833u, 1839030562u, 4259657740u, 2763975236u, 1272893353u, 4139469664u, 3200236656u,
		681279174u, 3936430074u, 3572445317u, 76029189u, 3654602809u, 3873151461u, 530742520u, 3299628645u, 4096336452u, 1126891415u,
		2878612391u, 4237533241u, 1700485571u, 2399980690u, 4293915773u, 2240044497u, 1873313359u, 4264355552u, 2734768916u, 1309151649u,
		4149444226u, 3174756917u, 718787259u, 3951481745u
	};

	public MD5CryptoServiceProvider()
	{
		DHJDOAPANCD = new uint[4];
		MOLMIGAIEPN = new uint[16];
		_ProcessingBuffer = new byte[64];
		Initialize();
	}

	~MD5CryptoServiceProvider()
	{
		Dispose(false);
	}

	protected override void Dispose(bool KLCPNDHEBGP)
	{
		if (_ProcessingBuffer != null)
		{
			Array.Clear(_ProcessingBuffer, 0, _ProcessingBuffer.Length);
			_ProcessingBuffer = null;
		}
		if (DHJDOAPANCD != null)
		{
			Array.Clear(DHJDOAPANCD, 0, DHJDOAPANCD.Length);
			DHJDOAPANCD = null;
		}
		if (MOLMIGAIEPN != null)
		{
			Array.Clear(MOLMIGAIEPN, 0, MOLMIGAIEPN.Length);
			MOLMIGAIEPN = null;
		}
	}

	protected override void HashCore(byte[] JIBHIBGDEKD, int CHMHMMEBBHN, int OABLBDAIJDK)
	{
		State = 1;
		if (_ProcessingBufferCount != 0)
		{
			if (OABLBDAIJDK < 64 - _ProcessingBufferCount)
			{
				Buffer.BlockCopy(JIBHIBGDEKD, CHMHMMEBBHN, _ProcessingBuffer, _ProcessingBufferCount, OABLBDAIJDK);
				_ProcessingBufferCount += OABLBDAIJDK;
				return;
			}
			int num = 64 - _ProcessingBufferCount;
			Buffer.BlockCopy(JIBHIBGDEKD, CHMHMMEBBHN, _ProcessingBuffer, _ProcessingBufferCount, num);
			IMBPBKBAMMH(_ProcessingBuffer, 0);
			_ProcessingBufferCount = 0;
			CHMHMMEBBHN += num;
			OABLBDAIJDK -= num;
		}
		for (int num = 0; num < OABLBDAIJDK - OABLBDAIJDK % 64; num += 64)
		{
			IMBPBKBAMMH(JIBHIBGDEKD, CHMHMMEBBHN + num);
		}
		if (OABLBDAIJDK % 64 != 0)
		{
			Buffer.BlockCopy(JIBHIBGDEKD, OABLBDAIJDK - OABLBDAIJDK % 64 + CHMHMMEBBHN, _ProcessingBuffer, 0, OABLBDAIJDK % 64);
			_ProcessingBufferCount = OABLBDAIJDK % 64;
		}
	}

	protected override byte[] HashFinal()
	{
		byte[] array = new byte[16];
		BPJIKIKONGF(_ProcessingBuffer, 0, _ProcessingBufferCount);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				array[i * 4 + j] = (byte)(DHJDOAPANCD[i] >> j * 8);
			}
		}
		return array;
	}

	public override void Initialize()
	{
		count = 0uL;
		_ProcessingBufferCount = 0;
		DHJDOAPANCD[0] = 1732584193u;
		DHJDOAPANCD[1] = 4023233417u;
		DHJDOAPANCD[2] = 2562383102u;
		DHJDOAPANCD[3] = 271733878u;
	}

	private void IMBPBKBAMMH(byte[] MMFIPPNMIKJ, int FMKACHAGFKK)
	{
		count += 64uL;
		for (int i = 0; i < 16; i++)
		{
			MOLMIGAIEPN[i] = (uint)(MMFIPPNMIKJ[FMKACHAGFKK + 4 * i] | (MMFIPPNMIKJ[FMKACHAGFKK + 4 * i + 1] << 8) | (MMFIPPNMIKJ[FMKACHAGFKK + 4 * i + 2] << 16) | (MMFIPPNMIKJ[FMKACHAGFKK + 4 * i + 3] << 24));
		}
		uint num = DHJDOAPANCD[0];
		uint num2 = DHJDOAPANCD[1];
		uint num3 = DHJDOAPANCD[2];
		uint num4 = DHJDOAPANCD[3];
		num += (((num3 ^ num4) & num2) ^ num4) + K[0] + MOLMIGAIEPN[0];
		num = (num << 7) | (num >> 25);
		num += num2;
		num4 += (((num2 ^ num3) & num) ^ num3) + K[1] + MOLMIGAIEPN[1];
		num4 = (num4 << 12) | (num4 >> 20);
		num4 += num;
		num3 += (((num ^ num2) & num4) ^ num2) + K[2] + MOLMIGAIEPN[2];
		num3 = (num3 << 17) | (num3 >> 15);
		num3 += num4;
		num2 += (((num4 ^ num) & num3) ^ num) + K[3] + MOLMIGAIEPN[3];
		num2 = (num2 << 22) | (num2 >> 10);
		num2 += num3;
		num += (((num3 ^ num4) & num2) ^ num4) + K[4] + MOLMIGAIEPN[4];
		num = (num << 7) | (num >> 25);
		num += num2;
		num4 += (((num2 ^ num3) & num) ^ num3) + K[5] + MOLMIGAIEPN[5];
		num4 = (num4 << 12) | (num4 >> 20);
		num4 += num;
		num3 += (((num ^ num2) & num4) ^ num2) + K[6] + MOLMIGAIEPN[6];
		num3 = (num3 << 17) | (num3 >> 15);
		num3 += num4;
		num2 += (((num4 ^ num) & num3) ^ num) + K[7] + MOLMIGAIEPN[7];
		num2 = (num2 << 22) | (num2 >> 10);
		num2 += num3;
		num += (((num3 ^ num4) & num2) ^ num4) + K[8] + MOLMIGAIEPN[8];
		num = (num << 7) | (num >> 25);
		num += num2;
		num4 += (((num2 ^ num3) & num) ^ num3) + K[9] + MOLMIGAIEPN[9];
		num4 = (num4 << 12) | (num4 >> 20);
		num4 += num;
		num3 += (((num ^ num2) & num4) ^ num2) + K[10] + MOLMIGAIEPN[10];
		num3 = (num3 << 17) | (num3 >> 15);
		num3 += num4;
		num2 += (((num4 ^ num) & num3) ^ num) + K[11] + MOLMIGAIEPN[11];
		num2 = (num2 << 22) | (num2 >> 10);
		num2 += num3;
		num += (((num3 ^ num4) & num2) ^ num4) + K[12] + MOLMIGAIEPN[12];
		num = (num << 7) | (num >> 25);
		num += num2;
		num4 += (((num2 ^ num3) & num) ^ num3) + K[13] + MOLMIGAIEPN[13];
		num4 = (num4 << 12) | (num4 >> 20);
		num4 += num;
		num3 += (((num ^ num2) & num4) ^ num2) + K[14] + MOLMIGAIEPN[14];
		num3 = (num3 << 17) | (num3 >> 15);
		num3 += num4;
		num2 += (((num4 ^ num) & num3) ^ num) + K[15] + MOLMIGAIEPN[15];
		num2 = (num2 << 22) | (num2 >> 10);
		num2 += num3;
		num += (((num2 ^ num3) & num4) ^ num3) + K[16] + MOLMIGAIEPN[1];
		num = (num << 5) | (num >> 27);
		num += num2;
		num4 += (((num ^ num2) & num3) ^ num2) + K[17] + MOLMIGAIEPN[6];
		num4 = (num4 << 9) | (num4 >> 23);
		num4 += num;
		num3 += (((num4 ^ num) & num2) ^ num) + K[18] + MOLMIGAIEPN[11];
		num3 = (num3 << 14) | (num3 >> 18);
		num3 += num4;
		num2 += (((num3 ^ num4) & num) ^ num4) + K[19] + MOLMIGAIEPN[0];
		num2 = (num2 << 20) | (num2 >> 12);
		num2 += num3;
		num += (((num2 ^ num3) & num4) ^ num3) + K[20] + MOLMIGAIEPN[5];
		num = (num << 5) | (num >> 27);
		num += num2;
		num4 += (((num ^ num2) & num3) ^ num2) + K[21] + MOLMIGAIEPN[10];
		num4 = (num4 << 9) | (num4 >> 23);
		num4 += num;
		num3 += (((num4 ^ num) & num2) ^ num) + K[22] + MOLMIGAIEPN[15];
		num3 = (num3 << 14) | (num3 >> 18);
		num3 += num4;
		num2 += (((num3 ^ num4) & num) ^ num4) + K[23] + MOLMIGAIEPN[4];
		num2 = (num2 << 20) | (num2 >> 12);
		num2 += num3;
		num += (((num2 ^ num3) & num4) ^ num3) + K[24] + MOLMIGAIEPN[9];
		num = (num << 5) | (num >> 27);
		num += num2;
		num4 += (((num ^ num2) & num3) ^ num2) + K[25] + MOLMIGAIEPN[14];
		num4 = (num4 << 9) | (num4 >> 23);
		num4 += num;
		num3 += (((num4 ^ num) & num2) ^ num) + K[26] + MOLMIGAIEPN[3];
		num3 = (num3 << 14) | (num3 >> 18);
		num3 += num4;
		num2 += (((num3 ^ num4) & num) ^ num4) + K[27] + MOLMIGAIEPN[8];
		num2 = (num2 << 20) | (num2 >> 12);
		num2 += num3;
		num += (((num2 ^ num3) & num4) ^ num3) + K[28] + MOLMIGAIEPN[13];
		num = (num << 5) | (num >> 27);
		num += num2;
		num4 += (((num ^ num2) & num3) ^ num2) + K[29] + MOLMIGAIEPN[2];
		num4 = (num4 << 9) | (num4 >> 23);
		num4 += num;
		num3 += (((num4 ^ num) & num2) ^ num) + K[30] + MOLMIGAIEPN[7];
		num3 = (num3 << 14) | (num3 >> 18);
		num3 += num4;
		num2 += (((num3 ^ num4) & num) ^ num4) + K[31] + MOLMIGAIEPN[12];
		num2 = (num2 << 20) | (num2 >> 12);
		num2 += num3;
		num += (num2 ^ num3 ^ num4) + K[32] + MOLMIGAIEPN[5];
		num = (num << 4) | (num >> 28);
		num += num2;
		num4 += (num ^ num2 ^ num3) + K[33] + MOLMIGAIEPN[8];
		num4 = (num4 << 11) | (num4 >> 21);
		num4 += num;
		num3 += (num4 ^ num ^ num2) + K[34] + MOLMIGAIEPN[11];
		num3 = (num3 << 16) | (num3 >> 16);
		num3 += num4;
		num2 += (num3 ^ num4 ^ num) + K[35] + MOLMIGAIEPN[14];
		num2 = (num2 << 23) | (num2 >> 9);
		num2 += num3;
		num += (num2 ^ num3 ^ num4) + K[36] + MOLMIGAIEPN[1];
		num = (num << 4) | (num >> 28);
		num += num2;
		num4 += (num ^ num2 ^ num3) + K[37] + MOLMIGAIEPN[4];
		num4 = (num4 << 11) | (num4 >> 21);
		num4 += num;
		num3 += (num4 ^ num ^ num2) + K[38] + MOLMIGAIEPN[7];
		num3 = (num3 << 16) | (num3 >> 16);
		num3 += num4;
		num2 += (num3 ^ num4 ^ num) + K[39] + MOLMIGAIEPN[10];
		num2 = (num2 << 23) | (num2 >> 9);
		num2 += num3;
		num += (num2 ^ num3 ^ num4) + K[40] + MOLMIGAIEPN[13];
		num = (num << 4) | (num >> 28);
		num += num2;
		num4 += (num ^ num2 ^ num3) + K[41] + MOLMIGAIEPN[0];
		num4 = (num4 << 11) | (num4 >> 21);
		num4 += num;
		num3 += (num4 ^ num ^ num2) + K[42] + MOLMIGAIEPN[3];
		num3 = (num3 << 16) | (num3 >> 16);
		num3 += num4;
		num2 += (num3 ^ num4 ^ num) + K[43] + MOLMIGAIEPN[6];
		num2 = (num2 << 23) | (num2 >> 9);
		num2 += num3;
		num += (num2 ^ num3 ^ num4) + K[44] + MOLMIGAIEPN[9];
		num = (num << 4) | (num >> 28);
		num += num2;
		num4 += (num ^ num2 ^ num3) + K[45] + MOLMIGAIEPN[12];
		num4 = (num4 << 11) | (num4 >> 21);
		num4 += num;
		num3 += (num4 ^ num ^ num2) + K[46] + MOLMIGAIEPN[15];
		num3 = (num3 << 16) | (num3 >> 16);
		num3 += num4;
		num2 += (num3 ^ num4 ^ num) + K[47] + MOLMIGAIEPN[2];
		num2 = (num2 << 23) | (num2 >> 9);
		num2 += num3;
		num += ((~num4 | num2) ^ num3) + K[48] + MOLMIGAIEPN[0];
		num = (num << 6) | (num >> 26);
		num += num2;
		num4 += ((~num3 | num) ^ num2) + K[49] + MOLMIGAIEPN[7];
		num4 = (num4 << 10) | (num4 >> 22);
		num4 += num;
		num3 += ((~num2 | num4) ^ num) + K[50] + MOLMIGAIEPN[14];
		num3 = (num3 << 15) | (num3 >> 17);
		num3 += num4;
		num2 += ((~num | num3) ^ num4) + K[51] + MOLMIGAIEPN[5];
		num2 = (num2 << 21) | (num2 >> 11);
		num2 += num3;
		num += ((~num4 | num2) ^ num3) + K[52] + MOLMIGAIEPN[12];
		num = (num << 6) | (num >> 26);
		num += num2;
		num4 += ((~num3 | num) ^ num2) + K[53] + MOLMIGAIEPN[3];
		num4 = (num4 << 10) | (num4 >> 22);
		num4 += num;
		num3 += ((~num2 | num4) ^ num) + K[54] + MOLMIGAIEPN[10];
		num3 = (num3 << 15) | (num3 >> 17);
		num3 += num4;
		num2 += ((~num | num3) ^ num4) + K[55] + MOLMIGAIEPN[1];
		num2 = (num2 << 21) | (num2 >> 11);
		num2 += num3;
		num += ((~num4 | num2) ^ num3) + K[56] + MOLMIGAIEPN[8];
		num = (num << 6) | (num >> 26);
		num += num2;
		num4 += ((~num3 | num) ^ num2) + K[57] + MOLMIGAIEPN[15];
		num4 = (num4 << 10) | (num4 >> 22);
		num4 += num;
		num3 += ((~num2 | num4) ^ num) + K[58] + MOLMIGAIEPN[6];
		num3 = (num3 << 15) | (num3 >> 17);
		num3 += num4;
		num2 += ((~num | num3) ^ num4) + K[59] + MOLMIGAIEPN[13];
		num2 = (num2 << 21) | (num2 >> 11);
		num2 += num3;
		num += ((~num4 | num2) ^ num3) + K[60] + MOLMIGAIEPN[4];
		num = (num << 6) | (num >> 26);
		num += num2;
		num4 += ((~num3 | num) ^ num2) + K[61] + MOLMIGAIEPN[11];
		num4 = (num4 << 10) | (num4 >> 22);
		num4 += num;
		num3 += ((~num2 | num4) ^ num) + K[62] + MOLMIGAIEPN[2];
		num3 = (num3 << 15) | (num3 >> 17);
		num3 += num4;
		num2 += ((~num | num3) ^ num4) + K[63] + MOLMIGAIEPN[9];
		num2 = (num2 << 21) | (num2 >> 11);
		num2 += num3;
		DHJDOAPANCD[0] += num;
		DHJDOAPANCD[1] += num2;
		DHJDOAPANCD[2] += num3;
		DHJDOAPANCD[3] += num4;
	}

	private void BPJIKIKONGF(byte[] MMFIPPNMIKJ, int FMKACHAGFKK, int GMNFIOIAJJM)
	{
		ulong num = count + (ulong)GMNFIOIAJJM;
		int num2 = (int)(56 - num % 64);
		if (num2 < 1)
		{
			num2 += 64;
		}
		byte[] array = new byte[GMNFIOIAJJM + num2 + 8];
		for (int i = 0; i < GMNFIOIAJJM; i++)
		{
			array[i] = MMFIPPNMIKJ[i + FMKACHAGFKK];
		}
		array[GMNFIOIAJJM] = 128;
		for (int j = GMNFIOIAJJM + 1; j < GMNFIOIAJJM + num2; j++)
		{
			array[j] = 0;
		}
		ulong bDBOAEGELMC = num << 3;
		AddLength(bDBOAEGELMC, array, GMNFIOIAJJM + num2);
		IMBPBKBAMMH(array, 0);
		if (GMNFIOIAJJM + num2 + 8 == 128)
		{
			IMBPBKBAMMH(array, 64);
		}
	}

	internal void AddLength(ulong BDBOAEGELMC, byte[] buffer, int MGMMDGFPBLP)
	{
		buffer[MGMMDGFPBLP++] = (byte)BDBOAEGELMC;
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 8);
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 16);
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 24);
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 32);
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 40);
		buffer[MGMMDGFPBLP++] = (byte)(BDBOAEGELMC >> 48);
		buffer[MGMMDGFPBLP] = (byte)(BDBOAEGELMC >> 56);
	}
}
