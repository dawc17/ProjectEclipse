using System;
using System.Diagnostics;
using System.IO;

public class Compressor
{
	public enum JJCLELFLHMH
	{
		LZMA = 0
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static JJCLELFLHMH NMOKAEMLKDF;

	private static int dictionary;

	private static bool eos;

	private static LNHBEIOHMGB[] JPIKKLMCDNM;

	private static object[] properties;

	public static JJCLELFLHMH JNKKGLAJEKI
	{
		get
		{
			return DGHOGAHDOKI();
		}
		set
		{
			EECNEIELFIG(value);
		}
	}

	static Compressor()
	{
		dictionary = 8388608;
		eos = false;
		JPIKKLMCDNM = new LNHBEIOHMGB[8]
		{
			LNHBEIOHMGB.DictionarySize,
			LNHBEIOHMGB.PosStateBits,
			LNHBEIOHMGB.LitContextBits,
			LNHBEIOHMGB.LitPosBits,
			LNHBEIOHMGB.Algorithm,
			LNHBEIOHMGB.NumFastBytes,
			LNHBEIOHMGB.MatchFinder,
			LNHBEIOHMGB.EndMarker
		};
		properties = new object[8] { dictionary, 2, 3, 0, 2, 128, "bt4", eos };
		EECNEIELFIG(JJCLELFLHMH.LZMA);
	}

	public static JJCLELFLHMH DGHOGAHDOKI()
	{
		return NMOKAEMLKDF;
	}

	public static void EECNEIELFIG(JJCLELFLHMH value)
	{
		NMOKAEMLKDF = value;
	}

	public static void DDIDIMMDPDN(string OBAMLJHHPPE, string POBFCEMGIGO, JJCLELFLHMH ABKOBELCOIK = JJCLELFLHMH.LZMA)
	{
		File.WriteAllBytes(OBAMLJHHPPE, Compress(File.ReadAllBytes(POBFCEMGIGO), ABKOBELCOIK));
	}

	public static void OGANCHANAMK(string OBAMLJHHPPE, string OOPMIPCMFPC, JJCLELFLHMH ABKOBELCOIK = JJCLELFLHMH.LZMA)
	{
		File.WriteAllBytes(OOPMIPCMFPC, EFJJNIMIBEO(File.ReadAllBytes(OBAMLJHHPPE), ABKOBELCOIK));
	}

	public static byte[] Compress(byte[] APACFLKJCKF, JJCLELFLHMH ABKOBELCOIK = JJCLELFLHMH.LZMA)
	{
		if (ABKOBELCOIK == JJCLELFLHMH.LZMA)
		{
			MemoryStream memoryStream = new MemoryStream(APACFLKJCKF);
			MemoryStream memoryStream2 = new MemoryStream();
			MNPBDHNFEBB mNPBDHNFEBB = new MNPBDHNFEBB();
			mNPBDHNFEBB.KOKOGBHPOFA(JPIKKLMCDNM, properties);
			mNPBDHNFEBB.FGKHFOOJIGA(memoryStream2);
			long length = memoryStream.Length;
			for (int i = 0; i < 8; i++)
			{
				memoryStream2.WriteByte((byte)(length >> 8 * i));
			}
			mNPBDHNFEBB.EDEEELJMHLG(memoryStream, memoryStream2, -1L, -1L, null);
			return memoryStream2.ToArray();
		}
		return new byte[0];
	}

	public static byte[] EFJJNIMIBEO(byte[] APACFLKJCKF, JJCLELFLHMH ABKOBELCOIK = JJCLELFLHMH.LZMA)
	{
		if (ABKOBELCOIK == JJCLELFLHMH.LZMA)
		{
			using (MemoryStream memoryStream = new MemoryStream(APACFLKJCKF))
			{
				GDEMLIAGBCB gDEMLIAGBCB = new GDEMLIAGBCB();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					byte[] array = new byte[5];
					if (memoryStream.Read(array, 0, 5) != 5)
					{
						throw new Exception("input .lzma is too short");
					}
					long num = 0L;
					for (int i = 0; i < 8; i++)
					{
						int num2 = memoryStream.ReadByte();
						if (num2 < 0)
						{
							throw new Exception("Can't Read 1");
						}
						num |= (long)(int)(byte)num2 << 8 * i;
					}
					gDEMLIAGBCB.SetDecoderProperties(array);
					long nCKELGLBGJN = memoryStream.Length - memoryStream.Position;
					gDEMLIAGBCB.EDEEELJMHLG(memoryStream, memoryStream2, nCKELGLBGJN, num, null);
					return memoryStream2.ToArray();
				}
			}
		}
		return new byte[0];
	}

	public static void DecodeStream(FileStream FBGBPGIDKHM, MemoryStream BBBGGJLOCPB)
	{
		GDEMLIAGBCB gDEMLIAGBCB = new GDEMLIAGBCB();
		FBGBPGIDKHM.Seek(0L, SeekOrigin.Begin);
		byte[] array = new byte[5];
		if (FBGBPGIDKHM.Read(array, 0, 5) != 5)
		{
			throw new Exception("input .lzma is too short");
		}
		long num = 0L;
		for (int i = 0; i < 8; i++)
		{
			int num2 = FBGBPGIDKHM.ReadByte();
			if (num2 < 0)
			{
				throw new Exception("Can't Read 1");
			}
			num |= (long)(int)(byte)num2 << 8 * i;
		}
		gDEMLIAGBCB.SetDecoderProperties(array);
		long nCKELGLBGJN = FBGBPGIDKHM.Length - FBGBPGIDKHM.Position;
		gDEMLIAGBCB.EDEEELJMHLG(FBGBPGIDKHM, BBBGGJLOCPB, nCKELGLBGJN, num, null);
	}
}
