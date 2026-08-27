using System;
using System.IO;

public class NGDOLJPDPBI : Stream
{
	private OIPMMMLGOAJ BAEJEHBFIHM;

	public Stream OPFHPIPINNP
	{
		get
		{
			return BIFBKCDGGAB();
		}
	}

	public NGDOLJPDPBI(Stream ABJIEFMMIEK, OAJFDGLHJMC NMMPBADCFHK)
		: this(ABJIEFMMIEK, NMMPBADCFHK, false)
	{
	}

	public NGDOLJPDPBI(Stream ABJIEFMMIEK, OAJFDGLHJMC NMMPBADCFHK, bool LOLBAGJKKPH)
	{
		BAEJEHBFIHM = new OIPMMMLGOAJ(ABJIEFMMIEK, NMMPBADCFHK, LOLBAGJKKPH);
		FBAHCDDHKMD(NMMPBADCFHK);
	}

	private void FBAHCDDHKMD(OAJFDGLHJMC NMMPBADCFHK)
	{
		if (NMMPBADCFHK == OAJFDGLHJMC.Compress)
		{
			IFileFormatWriter aPMCMDOBFOI = new GZipFormatter();
			BAEJEHBFIHM.LMLNNELNJCD(aPMCMDOBFOI);
		}
		else
		{
			IFileFormatReader iJIMLLIHKGN = new GZipDecoder();
			BAEJEHBFIHM.LDBNNMLIKOC(iJIMLLIHKGN);
		}
	}

	public override bool CanRead
	{
		get
		{
			if (BAEJEHBFIHM == null)
			{
				return false;
			}
			return BAEJEHBFIHM.CanRead;
		}
	}
	public override bool CanWrite
	{
		get
		{
			if (BAEJEHBFIHM == null)
			{
				return false;
			}
			return BAEJEHBFIHM.CanWrite;
		}
	}
	public override bool CanSeek
	{
		get
		{
			if (BAEJEHBFIHM == null)
			{
				return false;
			}
			return BAEJEHBFIHM.CanSeek;
		}
	}
	public override long Length
	{
		get
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}
	}
	public override long Position
	{
		get
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}
		set
		{
			throw new NotSupportedException(SR.GetString("Not supported"));
		}
	}

	public override void Flush()
	{
		OPFHPIPINNP.Flush();
	}

	public override long Seek(long IPCOBJBKNAO, SeekOrigin IKOOJMAOFOD)
	{
		throw new NotSupportedException(SR.GetString("Not supported"));
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException(SR.GetString("Not supported"));
	}

	public override IAsyncResult BeginRead(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, AsyncCallback FCLGHDMMEBC, object LEGPNOBHGIE)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new InvalidOperationException(SR.GetString("Object disposed"));
		}
		return BAEJEHBFIHM.BeginRead(HFPDMGAEJJE, IPCOBJBKNAO, count, FCLGHDMMEBC, LEGPNOBHGIE);
	}

	public override int EndRead(IAsyncResult BHNNOKGCDEG)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new InvalidOperationException(SR.GetString("Object disposed"));
		}
		return BAEJEHBFIHM.EndRead(BHNNOKGCDEG);
	}

	public override IAsyncResult BeginWrite(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, AsyncCallback FCLGHDMMEBC, object LEGPNOBHGIE)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new InvalidOperationException(SR.GetString("Object disposed"));
		}
		return BAEJEHBFIHM.BeginWrite(HFPDMGAEJJE, IPCOBJBKNAO, count, FCLGHDMMEBC, LEGPNOBHGIE);
	}

	public override void EndWrite(IAsyncResult BHNNOKGCDEG)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new InvalidOperationException(SR.GetString("Object disposed"));
		}
		BAEJEHBFIHM.EndWrite(BHNNOKGCDEG);
	}

	public override int Read(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
		}
		return BAEJEHBFIHM.Read(HFPDMGAEJJE, IPCOBJBKNAO, count);
	}

	public override void Write(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		if (BAEJEHBFIHM == null)
		{
			throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
		}
		BAEJEHBFIHM.Write(HFPDMGAEJJE, IPCOBJBKNAO, count);
	}

	protected override void Dispose(bool KLCPNDHEBGP)
	{
		try
		{
			if (KLCPNDHEBGP && BAEJEHBFIHM != null)
			{
				BAEJEHBFIHM.Dispose();
			}
			BAEJEHBFIHM = null;
		}
		finally
		{
			base.Dispose(KLCPNDHEBGP);
		}
	}

	public Stream BIFBKCDGGAB()
	{
		if (BAEJEHBFIHM != null)
		{
			return BAEJEHBFIHM.BIFBKCDGGAB();
		}
		return null;
	}
}
