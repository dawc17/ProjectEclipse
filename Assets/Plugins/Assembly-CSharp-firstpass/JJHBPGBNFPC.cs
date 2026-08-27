using System;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public class JJHBPGBNFPC
{
	private static byte[] publicKey = new byte[148]
	{
		6, 2, 0, 0, 0, 164, 0, 0, 82, 83,
		65, 49, 0, 4, 0, 0, 17, 0, 0, 0,
		253, 204, 69, 228, 171, 92, 184, 7, 144, 118,
		20, 89, 210, 43, 78, 187, 232, 229, 181, 226,
		100, 254, 168, 224, 123, 134, 52, 18, 93, 221,
		149, 56, 114, 214, 120, 160, 73, 76, 234, 126,
		212, 247, 163, 84, 61, 99, 132, 37, 57, 23,
		114, 89, 43, 217, 60, 111, 236, 105, 200, 45,
		39, 134, 68, 243, 115, 79, 141, 79, 195, 14,
		165, 2, 22, 12, 246, 168, 144, 54, 190, 54,
		143, 190, 251, 131, 75, 198, 92, 99, 255, 107,
		0, 162, 65, 221, 236, 130, 73, 115, 151, 166,
		141, 223, 9, 228, 53, 219, 62, 55, 88, 136,
		234, 224, 47, 136, 45, 251, 45, 79, 88, 32,
		71, 200, 98, 140, 162, 179, 134, 170
	};

	private static int blockLengthField = 128;

	private static int exponentField = publicKey[16] | (publicKey[17] << 8) | (publicKey[18] << 16);

	private static BigInteger nField;

	static JJHBPGBNFPC()
	{
		byte[] array = new byte[blockLengthField];
		Buffer.BlockCopy(publicKey, 20, array, 0, blockLengthField);
		Array.Reverse(array);
		nField = new BigInteger(array);
	}

	private static string DKJLEBDLKLJ(byte[] AAOIAEJJINO)
	{
		int i;
		for (i = 0; i < AAOIAEJJINO.Length && AAOIAEJJINO[i] == 0; i++)
		{
		}
		if (i != AAOIAEJJINO.Length)
		{
			byte[] array = new byte[AAOIAEJJINO.Length - i];
			Buffer.BlockCopy(AAOIAEJJINO, i, array, 0, AAOIAEJJINO.Length - i);
			return Encoding.UTF8.GetString(array);
		}
		return string.Empty;
	}

	public static string DKDDEIHHMJP(byte[] KPAMPCLHCEN, bool KKJCGBFKBGD)
	{
		if (KPAMPCLHCEN.Length == blockLengthField)
		{
			BigInteger bigInteger = new BigInteger(KPAMPCLHCEN);
			byte[] bytes = bigInteger.ModPow(exponentField, nField).GetBytes();
			string text = DKJLEBDLKLJ(bytes);
			if (KKJCGBFKBGD)
			{
				return text.Substring(1, text.Length - 2);
			}
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < KPAMPCLHCEN.Length / blockLengthField; i++)
		{
			byte[] array = new byte[blockLengthField];
			Buffer.BlockCopy(KPAMPCLHCEN, i * blockLengthField, array, 0, blockLengthField);
			BigInteger bigInteger2 = new BigInteger(array);
			byte[] bytes2 = bigInteger2.ModPow(exponentField, nField).GetBytes();
			stringBuilder.Append(DKJLEBDLKLJ(bytes2));
		}
		if (KKJCGBFKBGD)
		{
			string text2 = stringBuilder.ToString();
			return text2.Substring(1, text2.Length - 2);
		}
		return stringBuilder.ToString();
	}
}

	public struct BigInteger
	{
		private uint[] d;

		public BigInteger(byte[] littleEndianBytes)
		{
			int len = (littleEndianBytes.Length + 3) / 4;
			uint[] t = new uint[len];
			for (int i = 0; i < littleEndianBytes.Length; i++)
			{
				t[i >> 2] |= (uint)(littleEndianBytes[i] << ((i & 3) * 8));
			}
			d = Normalize(t);
		}

		private static uint[] Normalize(uint[] a)
		{
			int l = a.Length;
			while (l > 0 && a[l - 1] == 0)
			{
				l--;
			}
			if (l == a.Length)
			{
				return a;
			}
			uint[] r = new uint[(l == 0) ? 1 : l];
			Array.Copy(a, 0, r, 0, r.Length);
			return r;
		}

		private static int BitLen(uint[] a)
		{
			for (int i = a.Length - 1; i >= 0; i--)
			{
				if (a[i] != 0)
				{
					int b = 31;
					while ((a[i] & (1 << b)) == 0)
					{
						b--;
					}
					return (i << 5) + b + 1;
				}
			}
			return 0;
		}

		private static int GetBit(uint[] a, int i)
		{
			int w = i >> 5;
			if (w >= a.Length)
			{
				return 0;
			}
			return (int)((a[w] >> (i & 31)) & 1u);
		}

		private static uint[] Add(uint[] a, uint[] b)
		{
			int n = Math.Max(a.Length, b.Length);
			uint[] r = new uint[n + 1];
			ulong c = 0uL;
			for (int i = 0; i < n; i++)
			{
				ulong x = c + (ulong)((i < a.Length) ? a[i] : 0u) + (ulong)((i < b.Length) ? b[i] : 0u);
				r[i] = (uint)x;
				c = x >> 32;
			}
			r[n] = (uint)c;
			return r;
		}

		private static int Cmp(uint[] a, uint[] b)
		{
			int la = BitLen(a);
			int lb = BitLen(b);
			if (la != lb)
			{
				return (la > lb) ? 1 : (-1);
			}
			for (int i = la - 1; i >= 0; i--)
			{
				int wa = GetBit(a, i);
				int wb = GetBit(b, i);
				if (wa != wb)
				{
					return (wa > wb) ? 1 : (-1);
				}
			}
			return 0;
		}

		private static uint[] Sub(uint[] a, uint[] b)
		{
			uint[] r = new uint[a.Length];
			long borrow = 0L;
			for (int i = 0; i < a.Length; i++)
			{
				long x = (long)a[i] - (long)((i < b.Length) ? b[i] : 0u) - borrow;
				if (x < 0)
				{
					x += 4294967296L;
					borrow = 1L;
				}
				else
				{
					borrow = 0L;
				}
				r[i] = (uint)x;
			}
			return r;
		}

		private static uint[] Rem(uint[] a, uint[] m)
		{
			if (Cmp(a, m) < 0)
			{
				return a;
			}
			int mb = BitLen(m);
			int ab = BitLen(a);
			uint[] r = new uint[a.Length];
			Array.Copy(a, r, a.Length);
			for (int i = ab - 1; i >= mb; i--)
			{
				if (GetBit(r, i) == 0)
				{
					continue;
				}
				int shift = i - mb;
				int words = (shift >> 5) + 1;
				uint[] t = new uint[m.Length + words];
				for (int j = 0; j < m.Length; j++)
				{
					ulong v = (ulong)m[j] << (shift & 31);
					t[j + (shift >> 5)] |= (uint)v;
					t[j + (shift >> 5) + 1] |= (uint)(v >> 32);
				}
				if (Cmp(r, t) >= 0)
				{
					r = Sub(r, t);
				}
			}
			return Normalize(r);
		}

		public BigInteger ModPow(int exponent, BigInteger modulus)
		{
			uint[] rd = new uint[1] { 1u };
			uint[] bas = d;
			int e = exponent;
			while (e != 0)
			{
				if ((e & 1) != 0)
				{
					rd = MulMod(rd, bas, modulus.d);
				}
				e >>= 1;
				if (e != 0)
				{
					bas = MulMod(bas, bas, modulus.d);
				}
			}
			BigInteger result = default(BigInteger);
			result.d = Normalize(rd);
			return result;
		}

		private static uint[] MulMod(uint[] a, uint[] b, uint[] m)
		{
			int bb = BitLen(b);
			uint[] acc = new uint[1];
			for (int i = bb - 1; i >= 0; i--)
			{
				acc = ShlMod(acc, m);
				if (GetBit(b, i) != 0)
				{
					acc = Rem(Add(acc, a), m);
				}
			}
			return acc;
		}

		private static uint[] ShlMod(uint[] a, uint[] m)
		{
			uint[] r = new uint[a.Length + 1];
			for (int i = 0; i < a.Length; i++)
			{
				ulong v = (ulong)a[i] << 1;
				r[i] |= (uint)v;
				r[i + 1] |= (uint)(v >> 32);
			}
			return Rem(Normalize(r), m);
		}

		public byte[] GetBytes()
		{
			int bits = BitLen(d);
			if (bits == 0)
			{
				return new byte[0];
			}
			byte[] tmp = new byte[(bits + 7) / 8];
			for (int i = 0; i < tmp.Length; i++)
			{
				tmp[i] = (byte)((d[i >> 2] >> ((i & 3) * 8)) & 0xFFu);
			}
			Array.Reverse(tmp);
			return tmp;
		}
	}
