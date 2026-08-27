using System;
using System.IO;
using System.Text;

internal class DMOMPOFCMJJ : Stream
{
	public DateTime? LastModified;

	private int CFCMEGCDNHO;

	internal ZlibBaseStream LOPJAPMKLJO;

	private bool _disposed;

	private bool PDJBHCALANL;

	private string KHHJNHKEHPM;

	private string HKNKJNOPIOL;

	private int OGMJMDGHNDD;

	internal static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	internal static readonly Encoding iso8859dash1 = Encoding.GetEncoding("iso-8859-1");

	public string OIGMHMDJOIC
	{
		get
		{
			return MHBNIAOHOMF();
		}
		set
		{
			FNOIGJDBMJH(value);
		}
	}

	public string FileName
	{
		get
		{
			return EPDMGFELIMC();
		}
		set
		{
			IMMLGNKJPKA(value);
		}
	}

	public int OJMLODFAGNP
	{
		get
		{
			return DNINGHEBKBB();
		}
	}

	public virtual AFJHGKAEJPG PLKLGGBMBDE
	{
		get
		{
			return MCLKHFKLKKM();
		}
		set
		{
			PPEEKPLBIED(value);
		}
	}

	public int AIFEABGOAHP
	{
		get
		{
			return IKGIOLADFKL();
		}
		set
		{
			set_BufferSize(value);
		}
	}

	public virtual long JODMICHLOAE
	{
		get
		{
			return PGOLMCHCFJJ();
		}
	}

	public virtual long LHPBEEEBPOP
	{
		get
		{
			return JEPKKIAJONF();
		}
	}

	public DMOMPOFCMJJ(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK)
		: this(ABJIEFMMIEK, NMMPBADCFHK, NKFKKGNBHDK.Default, false)
	{
	}

	public DMOMPOFCMJJ(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, NKFKKGNBHDK GNLOCMLBNHF)
		: this(ABJIEFMMIEK, NMMPBADCFHK, GNLOCMLBNHF, false)
	{
	}

	public DMOMPOFCMJJ(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, bool LOLBAGJKKPH)
		: this(ABJIEFMMIEK, NMMPBADCFHK, NKFKKGNBHDK.Default, LOLBAGJKKPH)
	{
	}

	public DMOMPOFCMJJ(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, NKFKKGNBHDK GNLOCMLBNHF, bool LOLBAGJKKPH)
	{
		LOPJAPMKLJO = new ZlibBaseStream(ABJIEFMMIEK, NMMPBADCFHK, GNLOCMLBNHF, ZlibStreamFlavor.GZIP, LOLBAGJKKPH);
	}

	public string MHBNIAOHOMF()
	{
		return HKNKJNOPIOL;
	}

	public void FNOIGJDBMJH(string value)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		HKNKJNOPIOL = value;
	}

	public string EPDMGFELIMC()
	{
		return KHHJNHKEHPM;
	}

	public void IMMLGNKJPKA(string value)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		KHHJNHKEHPM = value;
		if (KHHJNHKEHPM != null)
		{
			if (KHHJNHKEHPM.IndexOf("/") != -1)
			{
				KHHJNHKEHPM = KHHJNHKEHPM.Replace("/", "\\");
			}
			if (KHHJNHKEHPM.EndsWith("\\"))
			{
				throw new Exception("Illegal filename");
			}
			if (KHHJNHKEHPM.IndexOf("\\") != -1)
			{
				KHHJNHKEHPM = Path.GetFileName(KHHJNHKEHPM);
			}
		}
	}

	public int DNINGHEBKBB()
	{
		return OGMJMDGHNDD;
	}

	public virtual AFJHGKAEJPG MCLKHFKLKKM()
	{
		return LOPJAPMKLJO.HOHDFAOLNFI;
	}

	public virtual void PPEEKPLBIED(AFJHGKAEJPG value)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		LOPJAPMKLJO.HOHDFAOLNFI = value;
	}

	public int IKGIOLADFKL()
	{
		return LOPJAPMKLJO.CBOPONBPHPE;
	}

	public void set_BufferSize(int value)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		if (LOPJAPMKLJO.PIKMAFBLGOF != null)
		{
			throw new ZlibException("The working buffer is already set.");
		}
		if (value < 1024)
		{
			throw new ZlibException(string.Format("Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, 1024));
		}
		LOPJAPMKLJO.CBOPONBPHPE = value;
	}

	public virtual long PGOLMCHCFJJ()
	{
		return LOPJAPMKLJO.DGBPNHJILKM.ALJBBHPGGPA;
	}

	public virtual long JEPKKIAJONF()
	{
		return LOPJAPMKLJO.DGBPNHJILKM.HCDKLJJLMOD;
	}

	protected override void Dispose(bool KLCPNDHEBGP)
	{
		try
		{
			if (!_disposed)
			{
				if (KLCPNDHEBGP && LOPJAPMKLJO != null)
				{
					LOPJAPMKLJO.Close();
					OGMJMDGHNDD = LOPJAPMKLJO.DNINGHEBKBB();
				}
				_disposed = true;
			}
		}
		finally
		{
			base.Dispose(KLCPNDHEBGP);
		}
	}

	public override bool CanRead
	{
		get
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return LOPJAPMKLJO._stream.CanRead;
		}
	}
	public override bool CanSeek
	{
		get
		{
			return false;
		}
	}
	public override bool CanWrite
	{
		get
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return LOPJAPMKLJO._stream.CanWrite;
		}
	}
	public override void Flush()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		LOPJAPMKLJO.Flush();
	}
	public override int Read(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Writer)
		{
			throw new InvalidOperationException("Cannot Read after Writing.");
		}
		return LOPJAPMKLJO.Read(buffer, IPCOBJBKNAO, count);
	}


	public override long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}
	public override long Position
	{
		get
		{
			if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Writer)
			{
				return LOPJAPMKLJO.DGBPNHJILKM.HCDKLJJLMOD + CFCMEGCDNHO;
			}
			if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Reader)
			{
				return LOPJAPMKLJO.DGBPNHJILKM.ALJBBHPGGPA + LOPJAPMKLJO.LONHHBHGGHF;
			}
			return 0L;
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
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Undefined)
		{
			if (!LOPJAPMKLJO.EHKLJHJOKPC())
			{
				throw new InvalidOperationException();
			}
			CFCMEGCDNHO = PBAAGFINJHF();
		}
		LOPJAPMKLJO.Write(buffer, IPCOBJBKNAO, count);
	}

	private int PBAAGFINJHF()
	{
		byte[] array = ((MHBNIAOHOMF() != null) ? iso8859dash1.GetBytes(MHBNIAOHOMF()) : null);
		byte[] array2 = ((EPDMGFELIMC() != null) ? iso8859dash1.GetBytes(EPDMGFELIMC()) : null);
		int num = ((MHBNIAOHOMF() != null) ? (array.Length + 1) : 0);
		int num2 = ((EPDMGFELIMC() != null) ? (array2.Length + 1) : 0);
		int num3 = 10 + num + num2;
		byte[] array3 = new byte[num3];
		int num4 = 0;
		array3[num4++] = 31;
		array3[num4++] = 139;
		array3[num4++] = 8;
		byte b = 0;
		if (MHBNIAOHOMF() != null)
		{
			b ^= 0x10;
		}
		if (EPDMGFELIMC() != null)
		{
			b ^= 8;
		}
		array3[num4++] = b;
		if (!LastModified.HasValue)
		{
			LastModified = DateTime.Now;
		}
		int value = (int)(LastModified.Value - _unixEpoch).TotalSeconds;
		Array.Copy(BitConverter.GetBytes(value), 0, array3, num4, 4);
		num4 += 4;
		array3[num4++] = 0;
		array3[num4++] = byte.MaxValue;
		if (num2 != 0)
		{
			Array.Copy(array2, 0, array3, num4, num2 - 1);
			num4 += num2 - 1;
			array3[num4++] = 0;
		}
		if (num != 0)
		{
			Array.Copy(array, 0, array3, num4, num - 1);
			num4 += num - 1;
			array3[num4++] = 0;
		}
		LOPJAPMKLJO._stream.Write(array3, 0, array3.Length);
		return array3.Length;
	}

	public static byte[] CompressString(string JDCCBCNFENK)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			Stream aBKOBELCOIK = new DMOMPOFCMJJ(memoryStream, KAOCBBMMFOG.Compress, NKFKKGNBHDK.BestCompression);
			ZlibBaseStream.CompressString(JDCCBCNFENK, aBKOBELCOIK);
			return memoryStream.ToArray();
		}
	}

	public static byte[] CompressBuffer(byte[] AAOIAEJJINO)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			Stream aBKOBELCOIK = new DMOMPOFCMJJ(memoryStream, KAOCBBMMFOG.Compress, NKFKKGNBHDK.BestCompression);
			ZlibBaseStream.CompressBuffer(AAOIAEJJINO, aBKOBELCOIK);
			return memoryStream.ToArray();
		}
	}

	public static string UncompressString(byte[] FCPABLANKDN)
	{
		using (MemoryStream aBJIEFMMIEK = new MemoryStream(FCPABLANKDN))
		{
			Stream iNIMCIOFFCJ = new DMOMPOFCMJJ(aBJIEFMMIEK, KAOCBBMMFOG.Decompress);
			return ZlibBaseStream.UncompressString(FCPABLANKDN, iNIMCIOFFCJ);
		}
	}

	public static byte[] UncompressBuffer(byte[] FCPABLANKDN)
	{
		using (MemoryStream aBJIEFMMIEK = new MemoryStream(FCPABLANKDN))
		{
			Stream iNIMCIOFFCJ = new DMOMPOFCMJJ(aBJIEFMMIEK, KAOCBBMMFOG.Decompress);
			return ZlibBaseStream.UncompressBuffer(FCPABLANKDN, iNIMCIOFFCJ);
		}
	}
}
