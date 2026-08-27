using System;
using System.IO;
using System.Security;
using System.Threading;

public class OIPMMMLGOAJ : Stream
{
	internal delegate void HCPMBHGJAAA(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, bool MHBDEKPKCPF);

	private enum LJKCGJKMJHC : byte
	{
		Managed = 0,
		Unknown = 1
	}

	internal const int DefaultBufferSize = 8192;

	private Stream _stream;

	private OAJFDGLHJMC CPGGHIBIOEB;

	private bool _leaveOpen;

	private Inflater MDBFAKODLBA;

	private IDeflater EPMPOGADCLD;

	private byte[] buffer;

	private int asyncOperations;

	private readonly AsyncCallback m_CallBack;

	private readonly HCPMBHGJAAA FJNCPNNAICA;

	private IFileFormatWriter CAKGMDHONCF;

	private bool MENIPFPBJJO;

	private bool LPNLFDJGKON;

	public Stream OPFHPIPINNP
	{
		get
		{
			return BIFBKCDGGAB();
		}
	}

	public OIPMMMLGOAJ(Stream ABJIEFMMIEK, OAJFDGLHJMC NMMPBADCFHK)
		: this(ABJIEFMMIEK, NMMPBADCFHK, false)
	{
	}

	public OIPMMMLGOAJ(Stream ABJIEFMMIEK, OAJFDGLHJMC NMMPBADCFHK, bool LOLBAGJKKPH)
	{
		if (ABJIEFMMIEK == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (NMMPBADCFHK != OAJFDGLHJMC.Compress && NMMPBADCFHK != OAJFDGLHJMC.Decompress)
		{
			throw new ArgumentException(SR.GetString("Argument out of range"), "mode");
		}
		_stream = ABJIEFMMIEK;
		CPGGHIBIOEB = NMMPBADCFHK;
		_leaveOpen = LOLBAGJKKPH;
		switch (CPGGHIBIOEB)
		{
		case OAJFDGLHJMC.Decompress:
			if (!_stream.CanRead)
			{
				throw new ArgumentException(SR.GetString("Not a readable stream"), "stream");
			}
			MDBFAKODLBA = new Inflater();
			m_CallBack = IDHMFIMHELK;
			break;
		case OAJFDGLHJMC.Compress:
			if (!_stream.CanWrite)
			{
				throw new ArgumentException(SR.GetString("Not a writeable stream"), "stream");
			}
			EPMPOGADCLD = AOJJGIABNCP();
			FJNCPNNAICA = NMFBLHDKJAG;
			m_CallBack = ECNBCGABOLE;
			break;
		}
		buffer = new byte[8192];
	}

	private static IDeflater AOJJGIABNCP()
	{
		if (KKJDCPPNIHL() == LJKCGJKMJHC.Managed)
		{
			return new DeflaterManaged();
		}
		throw new SystemException("Program entered an unexpected state.");
	}

	[SecuritySafeCritical]
	private static LJKCGJKMJHC KKJDCPPNIHL()
	{
		return LJKCGJKMJHC.Managed;
	}

	internal void LDBNNMLIKOC(IFileFormatReader reader)
	{
		if (reader != null)
		{
			MDBFAKODLBA.LDBNNMLIKOC(reader);
		}
	}

	internal void LMLNNELNJCD(IFileFormatWriter writer)
	{
		if (writer != null)
		{
			CAKGMDHONCF = writer;
		}
	}

	public Stream BIFBKCDGGAB()
	{
		return _stream;
	}

	public override bool CanRead
	{
		get
		{
			if (_stream == null)
			{
				return false;
			}
			return CPGGHIBIOEB == OAJFDGLHJMC.Decompress && _stream.CanRead;
		}
	}
	public override bool CanWrite
	{
		get
		{
			if (_stream == null)
			{
				return false;
			}
			return CPGGHIBIOEB == OAJFDGLHJMC.Compress && _stream.CanWrite;
		}
	}
	public override bool CanSeek
	{
		get
		{
			return false;
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
		if (_stream != null)
		{
			_stream.Flush();
		}
	}

	public override long Seek(long IPCOBJBKNAO, SeekOrigin IKOOJMAOFOD)
	{
		throw new NotSupportedException(SR.GetString("Not supported"));
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException(SR.GetString("Not supported"));
	}

	public override int Read(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		CMIFHCGOCLH();
		GHKOKLKDLCH(HFPDMGAEJJE, IPCOBJBKNAO, count);
		LIPOCDDPOMB();
		int num = IPCOBJBKNAO;
		int num2 = count;
		while (true)
		{
			int num3 = MDBFAKODLBA.Inflate(HFPDMGAEJJE, num, num2);
			num += num3;
			num2 -= num3;
			if (num2 == 0 || MDBFAKODLBA.ALDLIOBKDFF())
			{
				break;
			}
			int num4 = _stream.Read(buffer, 0, buffer.Length);
			if (num4 == 0)
			{
				break;
			}
			MDBFAKODLBA.SetInput(buffer, 0, num4);
		}
		return count - num2;
	}

	private void GHKOKLKDLCH(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		if (HFPDMGAEJJE == null)
		{
			throw new ArgumentNullException("array");
		}
		if (IPCOBJBKNAO < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (HFPDMGAEJJE.Length - IPCOBJBKNAO < count)
		{
			throw new ArgumentException(SR.GetString("Invalid argument offset count"));
		}
	}

	private void LIPOCDDPOMB()
	{
		if (_stream == null)
		{
			throw new ObjectDisposedException(null, SR.GetString("Object disposed"));
		}
	}

	private void CMIFHCGOCLH()
	{
		if (CPGGHIBIOEB != OAJFDGLHJMC.Decompress)
		{
			throw new InvalidOperationException(SR.GetString("Cannot read from deflate stream"));
		}
	}

	private void KBKEDAEJMOA()
	{
		if (CPGGHIBIOEB != OAJFDGLHJMC.Compress)
		{
			throw new InvalidOperationException(SR.GetString("Cannot write to deflate stream"));
		}
	}

	public override IAsyncResult BeginRead(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, AsyncCallback FCLGHDMMEBC, object LEGPNOBHGIE)
	{
		CMIFHCGOCLH();
		if (asyncOperations != 0)
		{
			throw new InvalidOperationException(SR.GetString("Invalid begin call"));
		}
		GHKOKLKDLCH(HFPDMGAEJJE, IPCOBJBKNAO, count);
		LIPOCDDPOMB();
		Interlocked.Increment(ref asyncOperations);
		try
		{
			DeflateStreamAsyncResult bOJEBIGFIKA = new DeflateStreamAsyncResult(this, LEGPNOBHGIE, FCLGHDMMEBC, HFPDMGAEJJE, IPCOBJBKNAO, count);
			bOJEBIGFIKA.NHHJKCGAGPP = false;
			int num = MDBFAKODLBA.Inflate(HFPDMGAEJJE, IPCOBJBKNAO, count);
			if (num != 0)
			{
				bOJEBIGFIKA.FHIHMGIFNAF(true, num);
				return bOJEBIGFIKA;
			}
			if (MDBFAKODLBA.ALDLIOBKDFF())
			{
				bOJEBIGFIKA.FHIHMGIFNAF(true, 0);
				return bOJEBIGFIKA;
			}
			_stream.BeginRead(buffer, 0, buffer.Length, m_CallBack, bOJEBIGFIKA);
			bOJEBIGFIKA.CHGOEKFDFME &= bOJEBIGFIKA.IsCompleted;
			return bOJEBIGFIKA;
		}
		catch
		{
			Interlocked.Decrement(ref asyncOperations);
			throw;
		}
	}

	private void IDHMFIMHELK(IAsyncResult KCLJLMAHPFI)
	{
		DeflateStreamAsyncResult bOJEBIGFIKA = (DeflateStreamAsyncResult)KCLJLMAHPFI.AsyncState;
		bOJEBIGFIKA.CHGOEKFDFME &= KCLJLMAHPFI.CompletedSynchronously;
		int num = 0;
		try
		{
			LIPOCDDPOMB();
			num = _stream.EndRead(KCLJLMAHPFI);
			if (num <= 0)
			{
				bOJEBIGFIKA.FHIHMGIFNAF(0);
				return;
			}
			MDBFAKODLBA.SetInput(buffer, 0, num);
			num = MDBFAKODLBA.Inflate(bOJEBIGFIKA.buffer, bOJEBIGFIKA.IPCOBJBKNAO, bOJEBIGFIKA.count);
			if (num == 0 && !MDBFAKODLBA.ALDLIOBKDFF())
			{
				_stream.BeginRead(buffer, 0, buffer.Length, m_CallBack, bOJEBIGFIKA);
			}
			else
			{
				bOJEBIGFIKA.FHIHMGIFNAF(num);
			}
		}
		catch (Exception dCJLKCFKCOM)
		{
			bOJEBIGFIKA.FHIHMGIFNAF(dCJLKCFKCOM);
		}
	}

	public override int EndRead(IAsyncResult BHNNOKGCDEG)
	{
		CMIFHCGOCLH();
		FFNNDNALIHB(BHNNOKGCDEG);
		DeflateStreamAsyncResult bOJEBIGFIKA = (DeflateStreamAsyncResult)BHNNOKGCDEG;
		GOLMGCCBLCL(bOJEBIGFIKA);
		Exception ex = bOJEBIGFIKA.JLDIDDAAFIL() as Exception;
		if (ex != null)
		{
			throw ex;
		}
		return (int)bOJEBIGFIKA.JLDIDDAAFIL();
	}

	public override void Write(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		KBKEDAEJMOA();
		GHKOKLKDLCH(HFPDMGAEJJE, IPCOBJBKNAO, count);
		LIPOCDDPOMB();
		NMFBLHDKJAG(HFPDMGAEJJE, IPCOBJBKNAO, count, false);
	}

	internal void NMFBLHDKJAG(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, bool MHBDEKPKCPF)
	{
		NMOENBFBIND(HFPDMGAEJJE, IPCOBJBKNAO, count);
		IMEONMDNMLA(MHBDEKPKCPF);
		EPMPOGADCLD.SetInput(HFPDMGAEJJE, IPCOBJBKNAO, count);
		IMEONMDNMLA(MHBDEKPKCPF);
	}

	private void IMEONMDNMLA(bool MHBDEKPKCPF)
	{
		while (!EPMPOGADCLD.NeedsInput())
		{
			int num = EPMPOGADCLD.GetDeflateOutput(buffer);
			if (num > 0)
			{
				KADEFEAOMDB(buffer, 0, num, MHBDEKPKCPF);
			}
		}
	}

	private void KADEFEAOMDB(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, bool MHBDEKPKCPF)
	{
		if (MHBDEKPKCPF)
		{
			IAsyncResult asyncResult = _stream.BeginWrite(HFPDMGAEJJE, IPCOBJBKNAO, count, null, null);
			_stream.EndWrite(asyncResult);
		}
		else
		{
			_stream.Write(HFPDMGAEJJE, IPCOBJBKNAO, count);
		}
	}

	private void NMOENBFBIND(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count)
	{
		if (count <= 0)
		{
			return;
		}
		LPNLFDJGKON = true;
		if (CAKGMDHONCF != null)
		{
			if (!MENIPFPBJJO)
			{
				byte[] array = CAKGMDHONCF.BOLKKEBKHEE();
				_stream.Write(array, 0, array.Length);
				MENIPFPBJJO = true;
			}
			CAKGMDHONCF.UpdateWithBytesRead(HFPDMGAEJJE, IPCOBJBKNAO, count);
		}
	}

	private void CDHOCGEOBLB(bool KLCPNDHEBGP)
	{
		if (!KLCPNDHEBGP || _stream == null)
		{
			return;
		}
		Flush();
		if (CPGGHIBIOEB != OAJFDGLHJMC.Compress)
		{
			return;
		}
		if (LPNLFDJGKON)
		{
			IMEONMDNMLA(false);
			bool flag;
			do
			{
				int GJBPPJIGAIG;
				flag = EPMPOGADCLD.Finish(buffer, out GJBPPJIGAIG);
				if (GJBPPJIGAIG > 0)
				{
					KADEFEAOMDB(buffer, 0, GJBPPJIGAIG, false);
				}
			}
			while (!flag);
		}
		if (CAKGMDHONCF != null && MENIPFPBJJO)
		{
			byte[] array = CAKGMDHONCF.AIJJKADBLMG();
			_stream.Write(array, 0, array.Length);
		}
	}

	protected override void Dispose(bool KLCPNDHEBGP)
	{
		try
		{
			CDHOCGEOBLB(KLCPNDHEBGP);
		}
		finally
		{
			try
			{
				if (KLCPNDHEBGP && !_leaveOpen && _stream != null)
				{
					_stream.Dispose();
				}
			}
			finally
			{
				_stream = null;
				try
				{
					if (EPMPOGADCLD != null)
					{
						EPMPOGADCLD.Dispose();
					}
				}
				finally
				{
					EPMPOGADCLD = null;
					base.Dispose(KLCPNDHEBGP);
				}
			}
		}
	}

	public override IAsyncResult BeginWrite(byte[] HFPDMGAEJJE, int IPCOBJBKNAO, int count, AsyncCallback FCLGHDMMEBC, object LEGPNOBHGIE)
	{
		KBKEDAEJMOA();
		if (asyncOperations != 0)
		{
			throw new InvalidOperationException(SR.GetString("Invalid begin call"));
		}
		GHKOKLKDLCH(HFPDMGAEJJE, IPCOBJBKNAO, count);
		LIPOCDDPOMB();
		Interlocked.Increment(ref asyncOperations);
		try
		{
			DeflateStreamAsyncResult bOJEBIGFIKA = new DeflateStreamAsyncResult(this, LEGPNOBHGIE, FCLGHDMMEBC, HFPDMGAEJJE, IPCOBJBKNAO, count);
			bOJEBIGFIKA.NHHJKCGAGPP = true;
			FJNCPNNAICA.BeginInvoke(HFPDMGAEJJE, IPCOBJBKNAO, count, true, m_CallBack, bOJEBIGFIKA);
			bOJEBIGFIKA.CHGOEKFDFME &= bOJEBIGFIKA.IsCompleted;
			return bOJEBIGFIKA;
		}
		catch
		{
			Interlocked.Decrement(ref asyncOperations);
			throw;
		}
	}

	private void ECNBCGABOLE(IAsyncResult BHNNOKGCDEG)
	{
		DeflateStreamAsyncResult bOJEBIGFIKA = (DeflateStreamAsyncResult)BHNNOKGCDEG.AsyncState;
		bOJEBIGFIKA.CHGOEKFDFME &= BHNNOKGCDEG.CompletedSynchronously;
		try
		{
			FJNCPNNAICA.EndInvoke(BHNNOKGCDEG);
		}
		catch (Exception dCJLKCFKCOM)
		{
			bOJEBIGFIKA.FHIHMGIFNAF(dCJLKCFKCOM);
			return;
		}
		bOJEBIGFIKA.FHIHMGIFNAF(null);
	}

	public override void EndWrite(IAsyncResult BHNNOKGCDEG)
	{
		KBKEDAEJMOA();
		FFNNDNALIHB(BHNNOKGCDEG);
		DeflateStreamAsyncResult bOJEBIGFIKA = (DeflateStreamAsyncResult)BHNNOKGCDEG;
		GOLMGCCBLCL(bOJEBIGFIKA);
		Exception ex = bOJEBIGFIKA.JLDIDDAAFIL() as Exception;
		if (ex != null)
		{
			throw ex;
		}
	}

	private void FFNNDNALIHB(IAsyncResult BHNNOKGCDEG)
	{
		if (asyncOperations != 1)
		{
			throw new InvalidOperationException(SR.GetString("Invalid end call"));
		}
		if (BHNNOKGCDEG == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		LIPOCDDPOMB();
		DeflateStreamAsyncResult bOJEBIGFIKA = BHNNOKGCDEG as DeflateStreamAsyncResult;
		if (bOJEBIGFIKA == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
	}

	private void GOLMGCCBLCL(DeflateStreamAsyncResult BHNNOKGCDEG)
	{
		try
		{
			if (!BHNNOKGCDEG.IsCompleted)
			{
				BHNNOKGCDEG.AsyncWaitHandle.WaitOne();
			}
		}
		finally
		{
			Interlocked.Decrement(ref asyncOperations);
			BHNNOKGCDEG.Close();
		}
	}
}
