using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public sealed class UploadStream : Stream
{
	private MemoryStream ReadBuffer = new MemoryStream();

	private MemoryStream LCDHLKCLFLB = new MemoryStream();

	private bool noMoreData;

	private AutoResetEvent ARE = new AutoResetEvent(false);

	private object locker = new object();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		private set
		{
			set_Name(value);
		}
	}

	private bool ACIPIAGGOFA
	{
		get
		{
			return AJIIONEPLIB();
		}
	}






	public UploadStream(string name)
		: this()
	{
		set_Name(name);
	}

	public UploadStream()
	{
		ReadBuffer = new MemoryStream();
		LCDHLKCLFLB = new MemoryStream();
		set_Name(string.Empty);
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	private bool AJIIONEPLIB()
	{
		lock (locker)
		{
			return ReadBuffer.Position == ReadBuffer.Length;
		}
	}

	public override int Read(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (noMoreData)
		{
			if (ReadBuffer.Position != ReadBuffer.Length)
			{
				return ReadBuffer.Read(buffer, IPCOBJBKNAO, count);
			}
			if (LCDHLKCLFLB.Length <= 0)
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("UploadStream", string.Format("{0} - Read - End Of Stream", get_Name()));
				return -1;
			}
			KLNFJCHIEFH();
		}
		if (AJIIONEPLIB())
		{
			ARE.WaitOne();
			lock (locker)
			{
				if (AJIIONEPLIB() && LCDHLKCLFLB.Length > 0)
				{
					KLNFJCHIEFH();
				}
			}
		}
		int num = -1;
		lock (locker)
		{
			return ReadBuffer.Read(buffer, IPCOBJBKNAO, count);
		}
	}

	public override void Write(byte[] buffer, int IPCOBJBKNAO, int count)
	{
		if (noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		lock (locker)
		{
			LCDHLKCLFLB.Write(buffer, IPCOBJBKNAO, count);
			KLNFJCHIEFH();
		}
		ARE.Set();
	}

	public override void Flush()
	{
		GEJLNPIEDPF();
	}

	protected override void Dispose(bool KLCPNDHEBGP)
	{
		if (KLCPNDHEBGP)
		{
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("UploadStream", string.Format("{0} - Dispose", get_Name()));
			ReadBuffer.Dispose();
			ReadBuffer = null;
			LCDHLKCLFLB.Dispose();
			LCDHLKCLFLB = null;
			ARE.Close();
			ARE = null;
		}
		base.Dispose(KLCPNDHEBGP);
	}

	public void GEJLNPIEDPF()
	{
		if (noMoreData)
		{
			throw new ArgumentException("noMoreData already set!");
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("UploadStream", string.Format("{0} - Finish", get_Name()));
		noMoreData = true;
		ARE.Set();
	}

	private bool KLNFJCHIEFH()
	{
		lock (locker)
		{
			if (ReadBuffer.Position == ReadBuffer.Length)
			{
				LCDHLKCLFLB.Seek(0L, SeekOrigin.Begin);
				ReadBuffer.SetLength(0L);
				MemoryStream lCDHLKCLFLB = LCDHLKCLFLB;
				LCDHLKCLFLB = ReadBuffer;
				ReadBuffer = lCDHLKCLFLB;
				return true;
			}
		}
		return false;
	}

	public override bool CanRead
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanSeek
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override bool CanWrite
	{
		get
		{
			throw new NotImplementedException();
		}
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
		throw new NotImplementedException();
	}
}
