using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public class EventSource : IHeartbeat
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	private EDMIJLJOPPF MAFFNGPOMJD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan EJIPHPJCPHO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string NFONJOFLBCD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPRequest HMKDGNFLBMB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private BHJHIPILHJB OnOpen;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private IPIGAJKKJLN onMessageField;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnErrorDelegate onErrorField;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnRetryDelegate OnRetry;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BHJHIPILHJB onClosedField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private GAHJEMHNLNB OnStateChanged;

	private Dictionary<string, JEEIPOLICHA> IANMNLLLEHH;

	private byte RetryCount;

	private DateTime RetryCalled;

	public Uri GJIGOCNEPME
	{
		get
		{
			return OJBDMGBGJMA();
		}
		private set
		{
			set_Uri(value);
		}
	}

	public EDMIJLJOPPF AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		private set
		{
			set_State(value);
		}
	}

	public TimeSpan AMHFGABBPEH
	{
		get
		{
			return NDLJCMFOAJG();
		}
		set
		{
			set_ReconnectionTime(value);
		}
	}

	public string OMJPABGFOHH
	{
		get
		{
			return HLDPNIFFCDG();
		}
		private set
		{
			set_LastEventId(value);
		}
	}

	public HTTPRequest OOHLFJNPGGA
	{
		get
		{
			return KGBEGJJPCKC();
		}
		private set
		{
			HMPIGPEAMPM(value);
		}
	}

	public event BHJHIPILHJB HKBKFMIBCED
	{
		add
		{
			LMOIENENDCP(value);
		}
		remove
		{
			IIGDNCOBGDB(value);
		}
	}

	public event IPIGAJKKJLN OnMessage
	{
		add
		{
			LIMLEFJPHPP(value);
		}
		remove
		{
			FEJIPPJIAHH(value);
		}
	}

	public event OnErrorDelegate OnError
	{
		add
		{
			BJDMHEHILEO(value);
		}
		remove
		{
			LEIDAIFMPCE(value);
		}
	}

	public event OnRetryDelegate HNPIDLGCLDL
	{
		add
		{
			CKMLLHFIEJG(value);
		}
		remove
		{
			HCNCCLGKAND(value);
		}
	}

	public event BHJHIPILHJB OnClosed
	{
		add
		{
			IDCIMGLDBJG(value);
		}
		remove
		{
			OIBOHOKKFKE(value);
		}
	}

	public event GAHJEMHNLNB KPGNPBCPCJK
	{
		add
		{
			FADMHEJNPJO(value);
		}
		remove
		{
			NEFHCNPDIHG(value);
		}
	}

	public EventSource(Uri KJHNCLAJMLO)
	{
		set_Uri(KJHNCLAJMLO);
		set_ReconnectionTime(TimeSpan.FromMilliseconds(2000.0));
		HMPIGPEAMPM(new HTTPRequest(OJBDMGBGJMA(), LAAFHDKKJFL.Get, false, true, GCGGFEIEJBN));
		KGBEGJJPCKC().MMPFBNNMGED("Accept", "text/event-stream");
		KGBEGJJPCKC().MMPFBNNMGED("Cache-Control", "no-cache");
		KGBEGJJPCKC().MMPFBNNMGED("Accept-Encoding", "identity");
		KGBEGJJPCKC().MBLIFPIOOON(OBBKIBFJEMI.ServerSentEvents);
		KGBEGJJPCKC().GFFABFBMJAO = GFFABFBMJAO;
		KGBEGJJPCKC().LADBBAMKLPJ(true);
	}

	public Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	private void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	public EDMIJLJOPPF FLBBFDNHJAJ()
	{
		return MAFFNGPOMJD;
	}

	private void set_State(EDMIJLJOPPF value)
	{
		EDMIJLJOPPF mAFFNGPOMJD = MAFFNGPOMJD;
		MAFFNGPOMJD = value;
		if (OnStateChanged != null)
		{
			try
			{
				OnStateChanged(this, mAFFNGPOMJD, MAFFNGPOMJD);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", "OnStateChanged", mPFFFAOGBJE);
			}
		}
	}

	public TimeSpan NDLJCMFOAJG()
	{
		return EJIPHPJCPHO;
	}

	public void set_ReconnectionTime(TimeSpan value)
	{
		EJIPHPJCPHO = value;
	}

	public string HLDPNIFFCDG()
	{
		return NFONJOFLBCD;
	}

	private void set_LastEventId(string value)
	{
		NFONJOFLBCD = value;
	}

	public HTTPRequest KGBEGJJPCKC()
	{
		return HMKDGNFLBMB;
	}

	private void HMPIGPEAMPM(HTTPRequest value)
	{
		HMKDGNFLBMB = value;
	}

	public void LMOIENENDCP(BHJHIPILHJB value)
	{
		BHJHIPILHJB bHJHIPILHJB = OnOpen;
		BHJHIPILHJB bHJHIPILHJB2;
		do
		{
			bHJHIPILHJB2 = bHJHIPILHJB;
			bHJHIPILHJB = Interlocked.CompareExchange(ref OnOpen, (BHJHIPILHJB)Delegate.Combine(bHJHIPILHJB2, value), bHJHIPILHJB);
		}
		while ((object)bHJHIPILHJB != bHJHIPILHJB2);
	}

	public void IIGDNCOBGDB(BHJHIPILHJB value)
	{
		BHJHIPILHJB bHJHIPILHJB = OnOpen;
		BHJHIPILHJB bHJHIPILHJB2;
		do
		{
			bHJHIPILHJB2 = bHJHIPILHJB;
			bHJHIPILHJB = Interlocked.CompareExchange(ref OnOpen, (BHJHIPILHJB)Delegate.Remove(bHJHIPILHJB2, value), bHJHIPILHJB);
		}
		while ((object)bHJHIPILHJB != bHJHIPILHJB2);
	}

	public void LIMLEFJPHPP(IPIGAJKKJLN value)
	{
		IPIGAJKKJLN iPIGAJKKJLN = onMessageField;
		IPIGAJKKJLN iPIGAJKKJLN2;
		do
		{
			iPIGAJKKJLN2 = iPIGAJKKJLN;
			iPIGAJKKJLN = Interlocked.CompareExchange(ref onMessageField, (IPIGAJKKJLN)Delegate.Combine(iPIGAJKKJLN2, value), iPIGAJKKJLN);
		}
		while ((object)iPIGAJKKJLN != iPIGAJKKJLN2);
	}

	public void FEJIPPJIAHH(IPIGAJKKJLN value)
	{
		IPIGAJKKJLN iPIGAJKKJLN = onMessageField;
		IPIGAJKKJLN iPIGAJKKJLN2;
		do
		{
			iPIGAJKKJLN2 = iPIGAJKKJLN;
			iPIGAJKKJLN = Interlocked.CompareExchange(ref onMessageField, (IPIGAJKKJLN)Delegate.Remove(iPIGAJKKJLN2, value), iPIGAJKKJLN);
		}
		while ((object)iPIGAJKKJLN != iPIGAJKKJLN2);
	}

	public void BJDMHEHILEO(OnErrorDelegate value)
	{
		OnErrorDelegate eGECAPOLBHF = onErrorField;
		OnErrorDelegate eGECAPOLBHF2;
		do
		{
			eGECAPOLBHF2 = eGECAPOLBHF;
			eGECAPOLBHF = Interlocked.CompareExchange(ref onErrorField, (OnErrorDelegate)Delegate.Combine(eGECAPOLBHF2, value), eGECAPOLBHF);
		}
		while ((object)eGECAPOLBHF != eGECAPOLBHF2);
	}

	public void LEIDAIFMPCE(OnErrorDelegate value)
	{
		OnErrorDelegate eGECAPOLBHF = onErrorField;
		OnErrorDelegate eGECAPOLBHF2;
		do
		{
			eGECAPOLBHF2 = eGECAPOLBHF;
			eGECAPOLBHF = Interlocked.CompareExchange(ref onErrorField, (OnErrorDelegate)Delegate.Remove(eGECAPOLBHF2, value), eGECAPOLBHF);
		}
		while ((object)eGECAPOLBHF != eGECAPOLBHF2);
	}

	public void CKMLLHFIEJG(OnRetryDelegate value)
	{
		OnRetryDelegate cPMLAEEAKNP = OnRetry;
		OnRetryDelegate cPMLAEEAKNP2;
		do
		{
			cPMLAEEAKNP2 = cPMLAEEAKNP;
			cPMLAEEAKNP = Interlocked.CompareExchange(ref OnRetry, (OnRetryDelegate)Delegate.Combine(cPMLAEEAKNP2, value), cPMLAEEAKNP);
		}
		while ((object)cPMLAEEAKNP != cPMLAEEAKNP2);
	}

	public void HCNCCLGKAND(OnRetryDelegate value)
	{
		OnRetryDelegate cPMLAEEAKNP = OnRetry;
		OnRetryDelegate cPMLAEEAKNP2;
		do
		{
			cPMLAEEAKNP2 = cPMLAEEAKNP;
			cPMLAEEAKNP = Interlocked.CompareExchange(ref OnRetry, (OnRetryDelegate)Delegate.Remove(cPMLAEEAKNP2, value), cPMLAEEAKNP);
		}
		while ((object)cPMLAEEAKNP != cPMLAEEAKNP2);
	}

	public void IDCIMGLDBJG(BHJHIPILHJB value)
	{
		BHJHIPILHJB bHJHIPILHJB = onClosedField;
		BHJHIPILHJB bHJHIPILHJB2;
		do
		{
			bHJHIPILHJB2 = bHJHIPILHJB;
			bHJHIPILHJB = Interlocked.CompareExchange(ref onClosedField, (BHJHIPILHJB)Delegate.Combine(bHJHIPILHJB2, value), bHJHIPILHJB);
		}
		while ((object)bHJHIPILHJB != bHJHIPILHJB2);
	}

	public void OIBOHOKKFKE(BHJHIPILHJB value)
	{
		BHJHIPILHJB bHJHIPILHJB = onClosedField;
		BHJHIPILHJB bHJHIPILHJB2;
		do
		{
			bHJHIPILHJB2 = bHJHIPILHJB;
			bHJHIPILHJB = Interlocked.CompareExchange(ref onClosedField, (BHJHIPILHJB)Delegate.Remove(bHJHIPILHJB2, value), bHJHIPILHJB);
		}
		while ((object)bHJHIPILHJB != bHJHIPILHJB2);
	}

	public void FADMHEJNPJO(GAHJEMHNLNB value)
	{
		GAHJEMHNLNB gAHJEMHNLNB = OnStateChanged;
		GAHJEMHNLNB gAHJEMHNLNB2;
		do
		{
			gAHJEMHNLNB2 = gAHJEMHNLNB;
			gAHJEMHNLNB = Interlocked.CompareExchange(ref OnStateChanged, (GAHJEMHNLNB)Delegate.Combine(gAHJEMHNLNB2, value), gAHJEMHNLNB);
		}
		while ((object)gAHJEMHNLNB != gAHJEMHNLNB2);
	}

	public void NEFHCNPDIHG(GAHJEMHNLNB value)
	{
		GAHJEMHNLNB gAHJEMHNLNB = OnStateChanged;
		GAHJEMHNLNB gAHJEMHNLNB2;
		do
		{
			gAHJEMHNLNB2 = gAHJEMHNLNB;
			gAHJEMHNLNB = Interlocked.CompareExchange(ref OnStateChanged, (GAHJEMHNLNB)Delegate.Remove(gAHJEMHNLNB2, value), gAHJEMHNLNB);
		}
		while ((object)gAHJEMHNLNB != gAHJEMHNLNB2);
	}

	public void LAJCMNNNIIM()
	{
		if (FLBBFDNHJAJ() == EDMIJLJOPPF.Initial || FLBBFDNHJAJ() == EDMIJLJOPPF.Retrying || FLBBFDNHJAJ() == EDMIJLJOPPF.Closed)
		{
			set_State(EDMIJLJOPPF.Connecting);
			if (!string.IsNullOrEmpty(HLDPNIFFCDG()))
			{
				KGBEGJJPCKC().MMPFBNNMGED("Last-Event-ID", HLDPNIFFCDG());
			}
			KGBEGJJPCKC().Send();
		}
	}

	public void Close()
	{
		if (FLBBFDNHJAJ() != EDMIJLJOPPF.Closing && FLBBFDNHJAJ() != EDMIJLJOPPF.Closed)
		{
			set_State(EDMIJLJOPPF.Closing);
			if (KGBEGJJPCKC() != null)
			{
				KGBEGJJPCKC().AKLEEMEHBIC();
			}
			else
			{
				set_State(EDMIJLJOPPF.Closed);
			}
		}
	}

	public void JPJAFMLNALO(string DOPHKKGNAEF, JEEIPOLICHA IBODMPMJELJ)
	{
		if (IANMNLLLEHH == null)
		{
			IANMNLLLEHH = new Dictionary<string, JEEIPOLICHA>();
		}
		IANMNLLLEHH[DOPHKKGNAEF] = IBODMPMJELJ;
	}

	public void Off(string DOPHKKGNAEF)
	{
		if (DOPHKKGNAEF != null)
		{
			IANMNLLLEHH.Remove(DOPHKKGNAEF);
		}
	}

	private void CallOnError(string JDONBAPIJCG, string CKEHOEGLMBM)
	{
		if (onErrorField != null)
		{
			try
			{
				onErrorField(this, JDONBAPIJCG);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", CKEHOEGLMBM + " - OnError", mPFFFAOGBJE);
			}
		}
	}

	private bool DKDJHEHMILP()
	{
		if (OnRetry != null)
		{
			try
			{
				return OnRetry(this);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", "CallOnRetry", mPFFFAOGBJE);
			}
		}
		return true;
	}

	private void FAJMMHIMBOB(string CKEHOEGLMBM)
	{
		set_State(EDMIJLJOPPF.Closed);
		if (onClosedField != null)
		{
			try
			{
				onClosedField(this);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", CKEHOEGLMBM + " - OnClosed", mPFFFAOGBJE);
			}
		}
	}

	private void DOIGOILHAKM()
	{
		if (RetryCount > 0 || !DKDJHEHMILP())
		{
			FAJMMHIMBOB("Retry");
			return;
		}
		RetryCount++;
		RetryCalled = DateTime.UtcNow;
		HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
		set_State(EDMIJLJOPPF.Retrying);
	}

	private void GFFABFBMJAO(HTTPRequest BPMCLBNFEDK, HTTPResponse GIHDDAKBMHE)
	{
		EventSourceResponse eNJKHKLBBLI = GIHDDAKBMHE as EventSourceResponse;
		if (eNJKHKLBBLI == null)
		{
			CallOnError("Not an EventSourceResponse!", "OnUpgraded");
			return;
		}
		if (OnOpen != null)
		{
			try
			{
				OnOpen(this);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", "OnOpen", mPFFFAOGBJE);
			}
		}
		eNJKHKLBBLI.OnMessage = (Action<EventSourceResponse, Message>)Delegate.Combine(eNJKHKLBBLI.OnMessage, new Action<EventSourceResponse, Message>(GKPFJAIFHMC));
		eNJKHKLBBLI.PBAFKNHCJHD();
		RetryCount = 0;
		set_State(EDMIJLJOPPF.Open);
	}

	private void GCGGFEIEJBN(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		if (FLBBFDNHJAJ() == EDMIJLJOPPF.Closed)
		{
			return;
		}
		if (FLBBFDNHJAJ() == EDMIJLJOPPF.Closing)
		{
			FAJMMHIMBOB("OnRequestFinished");
			return;
		}
		string text = string.Empty;
		bool flag = true;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Processing:
			flag = !BEIGFGCBICO.HasHeader("content-length");
			break;
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.KNMDPGBPNED() == 200 && !BEIGFGCBICO.HasHeaderWithValue("content-type", "text/event-stream"))
			{
				text = "No Content-Type header with value 'text/event-stream' present.";
				flag = false;
			}
			if (flag && BEIGFGCBICO.KNMDPGBPNED() != 500 && BEIGFGCBICO.KNMDPGBPNED() != 502 && BEIGFGCBICO.KNMDPGBPNED() != 503 && BEIGFGCBICO.KNMDPGBPNED() != 504)
			{
				flag = false;
				text = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			}
			break;
		case CFGBMHKCENK.Error:
			text = "Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			text = "OnRequestFinished - Aborted without request. EventSource's State: " + FLBBFDNHJAJ();
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = "Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			text = "Processing the request Timed Out!";
			break;
		}
		if (FLBBFDNHJAJ() < EDMIJLJOPPF.Closing)
		{
			if (!string.IsNullOrEmpty(text))
			{
				CallOnError(text, "OnRequestFinished");
			}
			if (flag)
			{
				DOIGOILHAKM();
			}
			else
			{
				FAJMMHIMBOB("OnRequestFinished");
			}
		}
		else
		{
			FAJMMHIMBOB("OnRequestFinished");
		}
	}

	private void GKPFJAIFHMC(EventSourceResponse BEIGFGCBICO, Message LIOGIBJBHAH)
	{
		if (FLBBFDNHJAJ() >= EDMIJLJOPPF.Closing)
		{
			return;
		}
		if (LIOGIBJBHAH.IMMIJJCLPBO() != null)
		{
			set_LastEventId(LIOGIBJBHAH.IMMIJJCLPBO());
		}
		if (LIOGIBJBHAH.GOOCPGAOBBH().TotalMilliseconds > 0.0)
		{
			set_ReconnectionTime(LIOGIBJBHAH.GOOCPGAOBBH());
		}
		if (string.IsNullOrEmpty(LIOGIBJBHAH.CHIGLEKCFFN()))
		{
			return;
		}
		if (onMessageField != null)
		{
			try
			{
				onMessageField(this, LIOGIBJBHAH);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", "OnMessageReceived - OnMessage", mPFFFAOGBJE);
			}
		}
		JEEIPOLICHA value;
		if (string.IsNullOrEmpty(LIOGIBJBHAH.EMCEPDNKAPK()) || !IANMNLLLEHH.TryGetValue(LIOGIBJBHAH.EMCEPDNKAPK(), out value) || value == null)
		{
			return;
		}
		try
		{
			value(this, LIOGIBJBHAH);
		}
		catch (Exception mPFFFAOGBJE2)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("EventSource", "OnMessageReceived - action", mPFFFAOGBJE2);
		}
	}

	void IHeartbeat.OnHeartbeatUpdate(TimeSpan OJOKANCMPLG)
	{
		if (FLBBFDNHJAJ() != EDMIJLJOPPF.Retrying)
		{
			HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		}
		else if (DateTime.UtcNow - RetryCalled >= NDLJCMFOAJG())
		{
			LAJCMNNNIIM();
			if (FLBBFDNHJAJ() != EDMIJLJOPPF.Connecting)
			{
				FAJMMHIMBOB("OnHeartbeatUpdate");
			}
			HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		}
	}
}
