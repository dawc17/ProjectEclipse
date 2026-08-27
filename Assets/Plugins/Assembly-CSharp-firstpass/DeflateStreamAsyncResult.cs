using System;
using System.Threading;

internal class DeflateStreamAsyncResult : IAsyncResult
{
	public byte[] buffer;

	public int IPCOBJBKNAO;

	public int count;

	public bool NHHJKCGAGPP;

	private object IJMABMIINAB;

	private object EHNEPDNDBED;

	private AsyncCallback m_AsyncCallback;

	private object MGGFAMGCHCL;

	internal bool CHGOEKFDFME;

	private int ABBAMCKIJGB;

	private int OBPDLMDNOEM;

	private object KAMHMIPFCNB;

	public object ABIIHLJKHEI
	{
		get
		{
			return AsyncState;
		}
	}

	public WaitHandle MAHFIFOFJJI
	{
		get
		{
			return AsyncWaitHandle;
		}
	}

	public bool LIHCAGEHNMF
	{
		get
		{
			return CompletedSynchronously;
		}
	}

	public bool FDDOKOEBOAG
	{
		get
		{
			return IsCompleted;
		}
	}

	internal object CFFJPCJGMDA
	{
		get
		{
			return JLDIDDAAFIL();
		}
	}

	public DeflateStreamAsyncResult(object FKBFNLAMILO, object LEGPNOBHGIE, AsyncCallback FCLGHDMMEBC, byte[] buffer, int IPCOBJBKNAO, int count)
	{
		this.buffer = buffer;
		this.IPCOBJBKNAO = IPCOBJBKNAO;
		this.count = count;
		CHGOEKFDFME = true;
		IJMABMIINAB = FKBFNLAMILO;
		EHNEPDNDBED = LEGPNOBHGIE;
		m_AsyncCallback = FCLGHDMMEBC;
	}

	public object AsyncState
	{
		get
		{
			return EHNEPDNDBED;
		}
	}

	public WaitHandle AsyncWaitHandle
	{
		get
		{
		int oBPDLMDNOEM = OBPDLMDNOEM;
		if (KAMHMIPFCNB == null)
		{
			Interlocked.CompareExchange(ref KAMHMIPFCNB, new ManualResetEvent(oBPDLMDNOEM != 0), null);
		}
		ManualResetEvent manualResetEvent = (ManualResetEvent)KAMHMIPFCNB;
		if (oBPDLMDNOEM == 0 && OBPDLMDNOEM != 0)
		{
			manualResetEvent.Set();
		}
		return manualResetEvent;
		}
	}

	public bool CompletedSynchronously
	{
		get
		{
			return CHGOEKFDFME;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return OBPDLMDNOEM != 0;
		}
	}

	internal object JLDIDDAAFIL()
	{
		return MGGFAMGCHCL;
	}

	internal void Close()
	{
		if (KAMHMIPFCNB != null)
		{
			((ManualResetEvent)KAMHMIPFCNB).Close();
		}
	}

	internal void FHIHMGIFNAF(bool ALLIOBCJDGG, object DCJLKCFKCOM)
	{
		Complete(ALLIOBCJDGG, DCJLKCFKCOM);
	}

	internal void FHIHMGIFNAF(object DCJLKCFKCOM)
	{
		Complete(DCJLKCFKCOM);
	}

	private void Complete(bool ALLIOBCJDGG, object DCJLKCFKCOM)
	{
		CHGOEKFDFME = ALLIOBCJDGG;
		Complete(DCJLKCFKCOM);
	}

	private void Complete(object DCJLKCFKCOM)
	{
		MGGFAMGCHCL = DCJLKCFKCOM;
		Interlocked.Increment(ref OBPDLMDNOEM);
		if (KAMHMIPFCNB != null)
		{
			((ManualResetEvent)KAMHMIPFCNB).Set();
		}
		if (Interlocked.Increment(ref ABBAMCKIJGB) == 1 && m_AsyncCallback != null)
		{
			m_AsyncCallback(this);
		}
	}
}
