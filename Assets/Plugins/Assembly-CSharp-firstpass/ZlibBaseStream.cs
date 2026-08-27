using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class ZlibBaseStream : Stream
{
	internal enum GKBLMACCNDB
	{
		Writer = 0,
		Reader = 1,
		Undefined = 2
	}

	protected internal ZlibCodec DGBPNHJILKM;

	protected internal GKBLMACCNDB EEGOCNIMEOI = GKBLMACCNDB.Undefined;

	protected internal AFJHGKAEJPG HOHDFAOLNFI;

	protected internal ZlibStreamFlavor MGHKJOOIJGI;

	protected internal KAOCBBMMFOG ANNHDHMNDFJ;

	protected internal NKFKKGNBHDK _level;

	protected internal bool _leaveOpen;

	protected internal byte[] PIKMAFBLGOF;

	protected internal int CBOPONBPHPE = 16384;

	protected internal byte[] ADDHDMAGBFB = new byte[1];

	protected internal Stream _stream;

	protected internal DDGGLIIKFPL JKDDDFMLLMI;

	private CRC32 GAICMJOFOJD;

	protected internal string PKGNEOJAJGD;

	protected internal string OHOHAPPHLDI;

	protected internal DateTime _GzipMtime;

	protected internal int LONHHBHGGHF;

	private bool LFIGEPGACDP;

	internal int OJMLODFAGNP
	{
		get
		{
			return DNINGHEBKBB();
		}
	}

	protected internal bool JNGGOGDAKOE
	{
		get
		{
			return EHKLJHJOKPC();
		}
	}

	private ZlibCodec LKPCKJOLJDO
	{
		get
		{
			return BMFJFPAFEIM();
		}
	}

	private byte[] PDPDMGPNEBH
	{
		get
		{
			return HELANNNBHDN();
		}
	}

	public ZlibBaseStream(Stream ABJIEFMMIEK, KAOCBBMMFOG HCCDFEPLGBA, NKFKKGNBHDK GNLOCMLBNHF, ZlibStreamFlavor CENOEIJNIAG, bool LOLBAGJKKPH)
	{
		HOHDFAOLNFI = AFJHGKAEJPG.None;
		_stream = ABJIEFMMIEK;
		_leaveOpen = LOLBAGJKKPH;
		ANNHDHMNDFJ = HCCDFEPLGBA;
		MGHKJOOIJGI = CENOEIJNIAG;
		_level = GNLOCMLBNHF;
		if (CENOEIJNIAG == ZlibStreamFlavor.GZIP)
		{
			GAICMJOFOJD = new CRC32();
		}
	}

	internal int DNINGHEBKBB()
	{
		if (GAICMJOFOJD == null)
		{
			return 0;
		}
		return GAICMJOFOJD.MMBAMEEDDFA();
	}

	protected internal bool EHKLJHJOKPC()
	{
		return ANNHDHMNDFJ == KAOCBBMMFOG.Compress;
	}

	private ZlibCodec BMFJFPAFEIM()
	{
		if (DGBPNHJILKM == null)
		{
			bool flag = MGHKJOOIJGI == ZlibStreamFlavor.ZLIB;
			DGBPNHJILKM = new ZlibCodec();
			if (ANNHDHMNDFJ == KAOCBBMMFOG.Decompress)
			{
				DGBPNHJILKM.InitializeInflate(flag);
			}
			else
			{
				DGBPNHJILKM.JKDDDFMLLMI = JKDDDFMLLMI;
				DGBPNHJILKM.JCBLHDMMDAB(_level, flag);
			}
		}
		return DGBPNHJILKM;
	}

	private byte[] HELANNNBHDN()
	{
		if (PIKMAFBLGOF == null)
		{
			PIKMAFBLGOF = new byte[CBOPONBPHPE];
		}
		return PIKMAFBLGOF;
	}

	public override void Write(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (GAICMJOFOJD != null)
		{
			GAICMJOFOJD.LOAACENMBJJ(buffer, IPCOBJBKNAO, count);
		}
		if (EEGOCNIMEOI == GKBLMACCNDB.Undefined)
		{
			EEGOCNIMEOI = GKBLMACCNDB.Writer;
		}
		else if (EEGOCNIMEOI != GKBLMACCNDB.Writer)
		{
			throw new ZlibException("Cannot Write after Reading.");
		}
		if (count == 0)
		{
			return;
		}
		BMFJFPAFEIM().PEFOCMDODLD = buffer;
		DGBPNHJILKM.LMIPBGGILEJ = IPCOBJBKNAO;
		DGBPNHJILKM.IAPJEIDMGNP = count;
		bool flag = false;
		do
		{
			DGBPNHJILKM.DKCGBABIAEN = HELANNNBHDN();
			DGBPNHJILKM.EIBFDELHKNM = 0;
			DGBPNHJILKM.NBNGINIIKNA = PIKMAFBLGOF.Length;
			int num = ((!EHKLJHJOKPC()) ? DGBPNHJILKM.Inflate(HOHDFAOLNFI) : DGBPNHJILKM.GAMMFNJHCFO(HOHDFAOLNFI));
			if (num != 0 && num != 1)
			{
				throw new ZlibException(((!EHKLJHJOKPC()) ? "in" : "de") + "flating: " + DGBPNHJILKM.Message);
			}
			_stream.Write(PIKMAFBLGOF, 0, PIKMAFBLGOF.Length - DGBPNHJILKM.NBNGINIIKNA);
			flag = DGBPNHJILKM.IAPJEIDMGNP == 0 && DGBPNHJILKM.NBNGINIIKNA != 0;
			if (MGHKJOOIJGI == ZlibStreamFlavor.GZIP && !EHKLJHJOKPC())
			{
				flag = DGBPNHJILKM.IAPJEIDMGNP == 8 && DGBPNHJILKM.NBNGINIIKNA != 0;
			}
		}
		while (!flag);
	}

	private void BFDAHEHCAGK()
	{
		if (DGBPNHJILKM == null)
		{
			return;
		}
		if (EEGOCNIMEOI == GKBLMACCNDB.Writer)
		{
			bool flag = false;
			do
			{
				DGBPNHJILKM.DKCGBABIAEN = HELANNNBHDN();
				DGBPNHJILKM.EIBFDELHKNM = 0;
				DGBPNHJILKM.NBNGINIIKNA = PIKMAFBLGOF.Length;
				int num = ((!EHKLJHJOKPC()) ? DGBPNHJILKM.Inflate(AFJHGKAEJPG.Finish) : DGBPNHJILKM.GAMMFNJHCFO(AFJHGKAEJPG.Finish));
				if (num != 1 && num != 0)
				{
					string text = ((!EHKLJHJOKPC()) ? "in" : "de") + "flating";
					if (DGBPNHJILKM.Message == null)
					{
						throw new ZlibException(string.Format("{0}: (rc = {1})", text, num));
					}
					throw new ZlibException(text + ": " + DGBPNHJILKM.Message);
				}
				if (PIKMAFBLGOF.Length - DGBPNHJILKM.NBNGINIIKNA > 0)
				{
					_stream.Write(PIKMAFBLGOF, 0, PIKMAFBLGOF.Length - DGBPNHJILKM.NBNGINIIKNA);
				}
				flag = DGBPNHJILKM.IAPJEIDMGNP == 0 && DGBPNHJILKM.NBNGINIIKNA != 0;
				if (MGHKJOOIJGI == ZlibStreamFlavor.GZIP && !EHKLJHJOKPC())
				{
					flag = DGBPNHJILKM.IAPJEIDMGNP == 8 && DGBPNHJILKM.NBNGINIIKNA != 0;
				}
			}
			while (!flag);
			Flush();
			if (MGHKJOOIJGI == ZlibStreamFlavor.GZIP)
			{
				if (!EHKLJHJOKPC())
				{
					throw new ZlibException("Writing with decompression is not supported.");
				}
				int value = GAICMJOFOJD.MMBAMEEDDFA();
				_stream.Write(BitConverter.GetBytes(value), 0, 4);
				int value2 = (int)(GAICMJOFOJD.BFADCOPLBPM() & 0xFFFFFFFFu);
				_stream.Write(BitConverter.GetBytes(value2), 0, 4);
			}
		}
		else
		{
			if (EEGOCNIMEOI != GKBLMACCNDB.Reader || MGHKJOOIJGI != ZlibStreamFlavor.GZIP)
			{
				return;
			}
			if (EHKLJHJOKPC())
			{
				throw new ZlibException("Reading with compression is not supported.");
			}
			if (DGBPNHJILKM.HCDKLJJLMOD == 0)
			{
				return;
			}
			byte[] array = new byte[8];
			if (DGBPNHJILKM.IAPJEIDMGNP < 8)
			{
				Array.Copy(DGBPNHJILKM.PEFOCMDODLD, DGBPNHJILKM.LMIPBGGILEJ, array, 0, DGBPNHJILKM.IAPJEIDMGNP);
				int num2 = 8 - DGBPNHJILKM.IAPJEIDMGNP;
				int num3 = _stream.Read(array, DGBPNHJILKM.IAPJEIDMGNP, num2);
				if (num2 != num3)
				{
					throw new ZlibException(string.Format("Missing or incomplete GZIP trailer. Expected 8 bytes, got {0}.", DGBPNHJILKM.IAPJEIDMGNP + num3));
				}
			}
			else
			{
				Array.Copy(DGBPNHJILKM.PEFOCMDODLD, DGBPNHJILKM.LMIPBGGILEJ, array, 0, array.Length);
			}
			int num4 = BitConverter.ToInt32(array, 0);
			int num5 = GAICMJOFOJD.MMBAMEEDDFA();
			int num6 = BitConverter.ToInt32(array, 4);
			int num7 = (int)(DGBPNHJILKM.HCDKLJJLMOD & 0xFFFFFFFFu);
			if (num5 != num4)
			{
				throw new ZlibException(string.Format("Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))", num5, num4));
			}
			if (num7 != num6)
			{
				throw new ZlibException(string.Format("Bad size in GZIP trailer. (actual({0})!=expected({1}))", num7, num6));
			}
		}
	}

	private void PCLFFOBJJFO()
	{
		if (BMFJFPAFEIM() != null)
		{
			if (EHKLJHJOKPC())
			{
				DGBPNHJILKM.GPBPBEHKNEO();
			}
			else
			{
				DGBPNHJILKM.LGGKOHICFEE();
			}
			DGBPNHJILKM = null;
		}
	}

	public override void Close()
	{
		if (_stream == null)
		{
			return;
		}
		try
		{
			BFDAHEHCAGK();
		}
		finally
		{
			PCLFFOBJJFO();
			if (!_leaveOpen)
			{
				_stream.Dispose();
			}
			_stream = null;
		}
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override long Position
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public override long Seek(long IPCOBJBKNAO, SeekOrigin IKOOJMAOFOD)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		_stream.SetLength(value);
	}

	private string LLJFJHPPFHD()
	{
		List<byte> list = new List<byte>();
		bool flag = false;
		do
		{
			int num = _stream.Read(ADDHDMAGBFB, 0, 1);
			if (num != 1)
			{
				throw new ZlibException("Unexpected EOF reading GZIP header.");
			}
			if (ADDHDMAGBFB[0] == 0)
			{
				flag = true;
			}
			else
			{
				list.Add(ADDHDMAGBFB[0]);
			}
		}
		while (!flag);
		byte[] array = list.ToArray();
		return DMOMPOFCMJJ.iso8859dash1.GetString(array, 0, array.Length);
	}

	private int CJKBCCEICJL()
	{
		int num = 0;
		byte[] array = new byte[10];
		int num2 = _stream.Read(array, 0, array.Length);
		switch (num2)
		{
		case 0:
			return 0;
		default:
			throw new ZlibException("Not a valid GZIP stream.");
		case 10:
		{
			if (array[0] != 31 || array[1] != 139 || array[2] != 8)
			{
				throw new ZlibException("Bad GZIP header.");
			}
			int num3 = BitConverter.ToInt32(array, 4);
			_GzipMtime = DMOMPOFCMJJ._unixEpoch.AddSeconds(num3);
			num += num2;
			if ((array[3] & 4) == 4)
			{
				num2 = _stream.Read(array, 0, 2);
				num += num2;
				short num4 = (short)(array[0] + array[1] * 256);
				byte[] array2 = new byte[num4];
				num2 = _stream.Read(array2, 0, array2.Length);
				if (num2 != num4)
				{
					throw new ZlibException("Unexpected end-of-file reading GZIP header.");
				}
				num += num2;
			}
			if ((array[3] & 8) == 8)
			{
				PKGNEOJAJGD = LLJFJHPPFHD();
			}
			if ((array[3] & 0x10) == 16)
			{
				OHOHAPPHLDI = LLJFJHPPFHD();
			}
			if ((array[3] & 2) == 2)
			{
				Read(ADDHDMAGBFB, 0, 1);
			}
			return num;
		}
		}
	}

	public override int Read(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (EEGOCNIMEOI == GKBLMACCNDB.Undefined)
		{
			if (!_stream.CanRead)
			{
				throw new ZlibException("The stream is not readable.");
			}
			EEGOCNIMEOI = GKBLMACCNDB.Reader;
			BMFJFPAFEIM().IAPJEIDMGNP = 0;
			if (MGHKJOOIJGI == ZlibStreamFlavor.GZIP)
			{
				LONHHBHGGHF = CJKBCCEICJL();
				if (LONHHBHGGHF == 0)
				{
					return 0;
				}
			}
		}
		if (EEGOCNIMEOI != GKBLMACCNDB.Reader)
		{
			throw new ZlibException("Cannot Read after Writing.");
		}
		if (count == 0)
		{
			return 0;
		}
		if (LFIGEPGACDP && EHKLJHJOKPC())
		{
			return 0;
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (IPCOBJBKNAO < buffer.GetLowerBound(0))
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (IPCOBJBKNAO + count > buffer.GetLength(0))
		{
			throw new ArgumentOutOfRangeException("count");
		}
		int num = 0;
		DGBPNHJILKM.DKCGBABIAEN = buffer;
		DGBPNHJILKM.EIBFDELHKNM = IPCOBJBKNAO;
		DGBPNHJILKM.NBNGINIIKNA = count;
		DGBPNHJILKM.PEFOCMDODLD = HELANNNBHDN();
		do
		{
			if (DGBPNHJILKM.IAPJEIDMGNP == 0 && !LFIGEPGACDP)
			{
				DGBPNHJILKM.LMIPBGGILEJ = 0;
				DGBPNHJILKM.IAPJEIDMGNP = _stream.Read(PIKMAFBLGOF, 0, PIKMAFBLGOF.Length);
				if (DGBPNHJILKM.IAPJEIDMGNP == 0)
				{
					LFIGEPGACDP = true;
				}
			}
			num = ((!EHKLJHJOKPC()) ? DGBPNHJILKM.Inflate(HOHDFAOLNFI) : DGBPNHJILKM.GAMMFNJHCFO(HOHDFAOLNFI));
			if (LFIGEPGACDP && num == -5)
			{
				return 0;
			}
			if (num != 0 && num != 1)
			{
				throw new ZlibException(string.Format("{0}flating:  rc={1}  msg={2}", (!EHKLJHJOKPC()) ? "in" : "de", num, DGBPNHJILKM.Message));
			}
		}
		while (((!LFIGEPGACDP && num != 1) || DGBPNHJILKM.NBNGINIIKNA != count) && DGBPNHJILKM.NBNGINIIKNA > 0 && !LFIGEPGACDP && num == 0);
		if (DGBPNHJILKM.NBNGINIIKNA > 0)
		{
			if (num != 0 || DGBPNHJILKM.IAPJEIDMGNP == 0)
			{
			}
			if (LFIGEPGACDP && EHKLJHJOKPC())
			{
				num = DGBPNHJILKM.GAMMFNJHCFO(AFJHGKAEJPG.Finish);
				if (num != 0 && num != 1)
				{
					throw new ZlibException(string.Format("Deflating:  rc={0}  msg={1}", num, DGBPNHJILKM.Message));
				}
			}
		}
		num = count - DGBPNHJILKM.NBNGINIIKNA;
		if (GAICMJOFOJD != null)
		{
			GAICMJOFOJD.LOAACENMBJJ(buffer, IPCOBJBKNAO, num);
		}
		return num;
	}

	public override bool CanRead
	{
		get
		{
			return _stream.CanRead;
		}
	}
	public override bool CanSeek
	{
		get
		{
			return _stream.CanSeek;
		}
	}
	public override bool CanWrite
	{
		get
		{
			return _stream.CanWrite;
		}
	}
	public override long Length
	{
		get
		{
			return _stream.Length;
		}
	}
	public static void CompressString(string JDCCBCNFENK, Stream ABKOBELCOIK)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(JDCCBCNFENK);
		using (ABKOBELCOIK)
		{
			ABKOBELCOIK.Write(bytes, 0, bytes.Length);
		}
	}

	public static void CompressBuffer(byte[] AAOIAEJJINO, Stream ABKOBELCOIK)
	{
		using (ABKOBELCOIK)
		{
			ABKOBELCOIK.Write(AAOIAEJJINO, 0, AAOIAEJJINO.Length);
		}
	}

	public static string UncompressString(byte[] FCPABLANKDN, Stream INIMCIOFFCJ)
	{
		byte[] array = new byte[1024];
		Encoding uTF = Encoding.UTF8;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (INIMCIOFFCJ)
			{
				int count;
				while ((count = INIMCIOFFCJ.Read(array, 0, array.Length)) != 0)
				{
					memoryStream.Write(array, 0, count);
				}
			}
			memoryStream.Seek(0L, SeekOrigin.Begin);
			StreamReader streamReader = new StreamReader(memoryStream, uTF);
			return streamReader.ReadToEnd();
		}
	}

	public static byte[] UncompressBuffer(byte[] FCPABLANKDN, Stream INIMCIOFFCJ)
	{
		byte[] array = new byte[1024];
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (INIMCIOFFCJ)
			{
				int count;
				while ((count = INIMCIOFFCJ.Read(array, 0, array.Length)) != 0)
				{
					memoryStream.Write(array, 0, count);
				}
			}
			return memoryStream.ToArray();
		}
	}
}
