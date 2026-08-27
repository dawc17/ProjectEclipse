using System;
using System.IO;

internal class CrcCalculatorStream : Stream, IDisposable
{
	private static readonly long UnsetLengthLimit = -99L;

	internal Stream _innerStream;

	private CRC32 OGMJMDGHNDD;

	private long _lengthLimit = -99L;

	private bool _leaveOpen;

	public long IDLBIAJDPOP
	{
		get
		{
			return AMFBINPAOBC();
		}
	}

	public int LNINNEFODFM
	{
		get
		{
			return GNENPHADPHE();
		}
	}

	public bool PNDPBAKKGEE
	{
		get
		{
			return HCBKKKBGLDO();
		}
		set
		{
			set_LeaveOpen(value);
		}
	}

	public CrcCalculatorStream(Stream ABJIEFMMIEK)
		: this(true, UnsetLengthLimit, ABJIEFMMIEK, null)
	{
	}

	public CrcCalculatorStream(Stream ABJIEFMMIEK, bool LOLBAGJKKPH)
		: this(LOLBAGJKKPH, UnsetLengthLimit, ABJIEFMMIEK, null)
	{
	}

	public CrcCalculatorStream(Stream ABJIEFMMIEK, long BDBOAEGELMC)
		: this(true, BDBOAEGELMC, ABJIEFMMIEK, null)
	{
		if (BDBOAEGELMC < 0)
		{
			throw new ArgumentException("length");
		}
	}

	public CrcCalculatorStream(Stream ABJIEFMMIEK, long BDBOAEGELMC, bool LOLBAGJKKPH)
		: this(LOLBAGJKKPH, BDBOAEGELMC, ABJIEFMMIEK, null)
	{
		if (BDBOAEGELMC < 0)
		{
			throw new ArgumentException("length");
		}
	}

	public CrcCalculatorStream(Stream ABJIEFMMIEK, long BDBOAEGELMC, bool LOLBAGJKKPH, CRC32 CJGBICDHGGL)
		: this(LOLBAGJKKPH, BDBOAEGELMC, ABJIEFMMIEK, CJGBICDHGGL)
	{
		if (BDBOAEGELMC < 0)
		{
			throw new ArgumentException("length");
		}
	}

	private CrcCalculatorStream(bool LOLBAGJKKPH, long BDBOAEGELMC, Stream ABJIEFMMIEK, CRC32 CJGBICDHGGL)
	{
		_innerStream = ABJIEFMMIEK;
		OGMJMDGHNDD = CJGBICDHGGL ?? new CRC32();
		_lengthLimit = BDBOAEGELMC;
		_leaveOpen = LOLBAGJKKPH;
	}

	public long AMFBINPAOBC()
	{
		return OGMJMDGHNDD.BFADCOPLBPM();
	}

	public int GNENPHADPHE()
	{
		return OGMJMDGHNDD.MMBAMEEDDFA();
	}

	public bool HCBKKKBGLDO()
	{
		return _leaveOpen;
	}

	public void set_LeaveOpen(bool value)
	{
		_leaveOpen = value;
	}

	public override int Read(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (_lengthLimit != UnsetLengthLimit)
		{
			if (OGMJMDGHNDD.BFADCOPLBPM() >= _lengthLimit)
			{
				return 0;
			}
			long num = _lengthLimit - OGMJMDGHNDD.BFADCOPLBPM();
			if (num < count)
			{
				count = (int)num;
			}
		}
		int num2 = _innerStream.Read(buffer, IPCOBJBKNAO, count);
		if (num2 > 0)
		{
			OGMJMDGHNDD.LOAACENMBJJ(buffer, IPCOBJBKNAO, num2);
		}
		return num2;
	}

	public override void Write(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (count > 0)
		{
			OGMJMDGHNDD.LOAACENMBJJ(buffer, IPCOBJBKNAO, count);
		}
		_innerStream.Write(buffer, IPCOBJBKNAO, count);
	}

	public override bool CanRead
	{
		get
		{
			return _innerStream.CanRead;
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
			return _innerStream.CanWrite;
		}
	}
	public override void Flush()
	{
		_innerStream.Flush();
	}

	public override long Length
	{
		get
		{
			if (_lengthLimit == UnsetLengthLimit)
			{
				return _innerStream.Length;
			}
			return _lengthLimit;
		}
	}
	public override long Position
	{
		get
		{
			return OGMJMDGHNDD.BFADCOPLBPM();
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

	void IDisposable.Dispose()
	{
		Close();
	}

	public override void Close()
	{
		Dispose();
		if (!_leaveOpen)
		{
			_innerStream.Dispose();
		}
	}
}
