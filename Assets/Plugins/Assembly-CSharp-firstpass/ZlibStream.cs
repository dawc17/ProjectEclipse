using System;
using System.IO;

internal class ZlibStream : Stream
{
	internal ZlibBaseStream LOPJAPMKLJO;

	private bool _disposed;

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

	public ZlibStream(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK)
		: this(ABJIEFMMIEK, NMMPBADCFHK, NKFKKGNBHDK.Default, false)
	{
	}

	public ZlibStream(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, NKFKKGNBHDK GNLOCMLBNHF)
		: this(ABJIEFMMIEK, NMMPBADCFHK, GNLOCMLBNHF, false)
	{
	}

	public ZlibStream(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, bool LOLBAGJKKPH)
		: this(ABJIEFMMIEK, NMMPBADCFHK, NKFKKGNBHDK.Default, LOLBAGJKKPH)
	{
	}

	public ZlibStream(Stream ABJIEFMMIEK, KAOCBBMMFOG NMMPBADCFHK, NKFKKGNBHDK GNLOCMLBNHF, bool LOLBAGJKKPH)
	{
		LOPJAPMKLJO = new ZlibBaseStream(ABJIEFMMIEK, NMMPBADCFHK, GNLOCMLBNHF, ZlibStreamFlavor.ZLIB, LOLBAGJKKPH);
	}

	public virtual AFJHGKAEJPG MCLKHFKLKKM()
	{
		return LOPJAPMKLJO.HOHDFAOLNFI;
	}

	public virtual void PPEEKPLBIED(AFJHGKAEJPG value)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZlibStream");
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
			throw new ObjectDisposedException("ZlibStream");
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
				throw new ObjectDisposedException("ZlibStream");
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
				throw new ObjectDisposedException("ZlibStream");
			}
			return LOPJAPMKLJO._stream.CanWrite;
		}
	}
	public override void Flush()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZlibStream");
		}
		LOPJAPMKLJO.Flush();
	}

	public override int Read(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZlibStream");
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
			throw new NotSupportedException();
		}
	}
	public override long Position
	{
		get
		{
			if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Writer)
			{
				return LOPJAPMKLJO.DGBPNHJILKM.HCDKLJJLMOD;
			}
			if (LOPJAPMKLJO.EEGOCNIMEOI == ZlibBaseStream.GKBLMACCNDB.Reader)
			{
				return LOPJAPMKLJO.DGBPNHJILKM.ALJBBHPGGPA;
			}
			return 0L;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public override long Seek(long IPCOBJBKNAO, SeekOrigin IKOOJMAOFOD)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZlibStream");
		}
		LOPJAPMKLJO.Write(buffer, IPCOBJBKNAO, count);
	}

	public static byte[] CompressString(string JDCCBCNFENK)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			Stream aBKOBELCOIK = new ZlibStream(memoryStream, KAOCBBMMFOG.Compress, NKFKKGNBHDK.BestCompression);
			ZlibBaseStream.CompressString(JDCCBCNFENK, aBKOBELCOIK);
			return memoryStream.ToArray();
		}
	}

	public static byte[] CompressBuffer(byte[] AAOIAEJJINO)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			Stream aBKOBELCOIK = new ZlibStream(memoryStream, KAOCBBMMFOG.Compress, NKFKKGNBHDK.BestCompression);
			ZlibBaseStream.CompressBuffer(AAOIAEJJINO, aBKOBELCOIK);
			return memoryStream.ToArray();
		}
	}

	public static string UncompressString(byte[] FCPABLANKDN)
	{
		using (MemoryStream aBJIEFMMIEK = new MemoryStream(FCPABLANKDN))
		{
			Stream iNIMCIOFFCJ = new ZlibStream(aBJIEFMMIEK, KAOCBBMMFOG.Decompress);
			return ZlibBaseStream.UncompressString(FCPABLANKDN, iNIMCIOFFCJ);
		}
	}

	public static byte[] UncompressBuffer(byte[] FCPABLANKDN)
	{
		using (MemoryStream aBJIEFMMIEK = new MemoryStream(FCPABLANKDN))
		{
			Stream iNIMCIOFFCJ = new ZlibStream(aBJIEFMMIEK, KAOCBBMMFOG.Decompress);
			return ZlibBaseStream.UncompressBuffer(FCPABLANKDN, iNIMCIOFFCJ);
		}
	}
}
