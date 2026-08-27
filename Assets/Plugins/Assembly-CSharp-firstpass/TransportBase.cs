using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public abstract class TransportBase
{
	private const int MaxRetryCount = 5;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IConnection HNPEACEPJIB;

	public LJLKMCGDKJK MAFFNGPOMJD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private LCGIFKDMOMP OnStateChanged;

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		protected set
		{
			set_Name(value);
		}
	}

	public abstract bool ODFCAGMNOHK { get; }

	public IConnection PEBFDIFIMBO
	{
		get
		{
			return BAFGHLCPPHM();
		}
		protected set
		{
			GNLCPJFBAJE(value);
		}
	}

	public LJLKMCGDKJK AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		protected set
		{
			set_State(value);
		}
	}

	public event LCGIFKDMOMP KPGNPBCPCJK
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

	public TransportBase(string name, Connection MDGFGCDPGFI)
	{
		set_Name(name);
		GNLCPJFBAJE(MDGFGCDPGFI);
		set_State(LJLKMCGDKJK.Initial);
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	protected void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public abstract bool IBMJBEKAIAH();

	public abstract AHLJIMDEAJD get_Type();

	public IConnection BAFGHLCPPHM()
	{
		return HNPEACEPJIB;
	}

	protected void GNLCPJFBAJE(IConnection value)
	{
		HNPEACEPJIB = value;
	}

	public LJLKMCGDKJK FLBBFDNHJAJ()
	{
		return MAFFNGPOMJD;
	}

	protected void set_State(LJLKMCGDKJK value)
	{
		LJLKMCGDKJK mAFFNGPOMJD = MAFFNGPOMJD;
		MAFFNGPOMJD = value;
		if (OnStateChanged != null)
		{
			OnStateChanged(this, mAFFNGPOMJD, MAFFNGPOMJD);
		}
	}

	public void FADMHEJNPJO(LCGIFKDMOMP value)
	{
		LCGIFKDMOMP lCGIFKDMOMP = OnStateChanged;
		LCGIFKDMOMP lCGIFKDMOMP2;
		do
		{
			lCGIFKDMOMP2 = lCGIFKDMOMP;
			lCGIFKDMOMP = Interlocked.CompareExchange(ref OnStateChanged, (LCGIFKDMOMP)Delegate.Combine(lCGIFKDMOMP2, value), lCGIFKDMOMP);
		}
		while ((object)lCGIFKDMOMP != lCGIFKDMOMP2);
	}

	public void NEFHCNPDIHG(LCGIFKDMOMP value)
	{
		LCGIFKDMOMP lCGIFKDMOMP = OnStateChanged;
		LCGIFKDMOMP lCGIFKDMOMP2;
		do
		{
			lCGIFKDMOMP2 = lCGIFKDMOMP;
			lCGIFKDMOMP = Interlocked.CompareExchange(ref OnStateChanged, (LCGIFKDMOMP)Delegate.Remove(lCGIFKDMOMP2, value), lCGIFKDMOMP);
		}
		while ((object)lCGIFKDMOMP != lCGIFKDMOMP2);
	}

	public abstract void NDCILHIAPIK();

	public abstract void Stop();

	protected abstract void SendImpl(string EMDHMHOKGFP);

	protected abstract void HHLGNIDNLNG();

	protected abstract void NGGKNLJALML();

	protected void PIGDCLOPNKJ()
	{
		if (FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting)
		{
			Start();
			return;
		}
		BAFGHLCPPHM().TransportReconnected();
		HHLGNIDNLNG();
		set_State(LJLKMCGDKJK.Started);
	}

	protected void Start()
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Sending Start Request");
		set_State(LJLKMCGDKJK.Starting);
		HTTPRequest iPLGNIDJDCF = new HTTPRequest(BAFGHLCPPHM().BuildUri(FHIEGKMHOCC.Start, this), LAAFHDKKJFL.Get, true, true, FFOABDGLCOE);
		iPLGNIDJDCF.set_Tag(0);
		iPLGNIDJDCF.LADBBAMKLPJ(true);
		iPLGNIDJDCF.DKLGPGDJPGO(BAFGHLCPPHM().EOBPEOEMEDB().LFLAILLBGOF() + TimeSpan.FromSeconds(10.0));
		BAFGHLCPPHM().PrepareRequest(iPLGNIDJDCF, FHIEGKMHOCC.Start);
		iPLGNIDJDCF.Send();
	}

	private void FFOABDGLCOE(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		CFGBMHKCENK cFGBMHKCENK = CGOIOKHEGOE.FLBBFDNHJAJ();
		if (cFGBMHKCENK == CFGBMHKCENK.Finished)
		{
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Start - Returned: " + BEIGFGCBICO.DPBLPGKOEJB());
				string text = BAFGHLCPPHM().ParseResponse(BEIGFGCBICO.DPBLPGKOEJB());
				if (text != "started")
				{
					BAFGHLCPPHM().Error(string.Format("Expected 'started' response, but '{0}' found!", text));
					return;
				}
				set_State(LJLKMCGDKJK.Started);
				HHLGNIDNLNG();
				BAFGHLCPPHM().TransportStarted();
				return;
			}
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("Transport - " + get_Name(), string.Format("Start - request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE()));
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Start request state: " + CGOIOKHEGOE.FLBBFDNHJAJ());
		int num = (int)CGOIOKHEGOE.LOIGCKFONHJ();
		if (num++ < 5)
		{
			CGOIOKHEGOE.set_Tag(num);
			CGOIOKHEGOE.Send();
		}
		else
		{
			BAFGHLCPPHM().Error("Failed to send Start request.");
		}
	}

	public virtual void AKLEEMEHBIC()
	{
		if (FLBBFDNHJAJ() == LJLKMCGDKJK.Started)
		{
			set_State(LJLKMCGDKJK.Closing);
			HTTPRequest iPLGNIDJDCF = new HTTPRequest(BAFGHLCPPHM().BuildUri(FHIEGKMHOCC.Abort, this), LAAFHDKKJFL.Get, true, true, DOJCPAAEELL);
			iPLGNIDJDCF.set_Tag(0);
			iPLGNIDJDCF.LADBBAMKLPJ(true);
			BAFGHLCPPHM().PrepareRequest(iPLGNIDJDCF, FHIEGKMHOCC.Abort);
			iPLGNIDJDCF.Send();
		}
	}

	protected void JGGILBCPENL()
	{
		set_State(LJLKMCGDKJK.Closed);
		BAFGHLCPPHM().TransportAborted();
		NGGKNLJALML();
	}

	private void DOJCPAAEELL(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		CFGBMHKCENK cFGBMHKCENK = CGOIOKHEGOE.FLBBFDNHJAJ();
		if (cFGBMHKCENK == CFGBMHKCENK.Finished)
		{
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Abort - Returned: " + BEIGFGCBICO.DPBLPGKOEJB());
				if (FLBBFDNHJAJ() == LJLKMCGDKJK.Closing)
				{
					JGGILBCPENL();
				}
				return;
			}
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("Transport - " + get_Name(), string.Format("Abort - Handshake request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB(), CGOIOKHEGOE.DKAECMGPGOE()));
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Abort request state: " + CGOIOKHEGOE.FLBBFDNHJAJ());
		int num = (int)CGOIOKHEGOE.LOIGCKFONHJ();
		if (num++ < 5)
		{
			CGOIOKHEGOE.set_Tag(num);
			CGOIOKHEGOE.Send();
		}
		else
		{
			BAFGHLCPPHM().Error("Failed to send Abort request!");
		}
	}

	public void Send(string DGNLDMDLKDA)
	{
		try
		{
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Sending: " + DGNLDMDLKDA);
			SendImpl(DGNLDMDLKDA);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("Transport - " + get_Name(), "Send", mPFFFAOGBJE);
		}
	}

	public void IGFIEFDGBDJ()
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Reconnecting");
		Stop();
		set_State(LJLKMCGDKJK.Reconnecting);
		NDCILHIAPIK();
	}

	public static IServerMessage Parse(AJAIAKCIJIJ GLOJHMAIFOK, string EMDHMHOKGFP)
	{
		if (string.IsNullOrEmpty(EMDHMHOKGFP))
		{
			HTTPManager.MBBMPNDDPIH().Error("MessageFactory", "Parse - called with empty or null string!");
			return null;
		}
		if (EMDHMHOKGFP.Length == 2 && EMDHMHOKGFP == "{}")
		{
			return new KeepAliveMessage();
		}
		IDictionary<string, object> dictionary = null;
		try
		{
			dictionary = GLOJHMAIFOK.DecodeMessage(EMDHMHOKGFP);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("MessageFactory", "Parse - encoder.DecodeMessage", mPFFFAOGBJE);
			return null;
		}
		if (dictionary == null)
		{
			HTTPManager.MBBMPNDDPIH().Error("MessageFactory", "Parse - Json Decode failed for json string: \"" + EMDHMHOKGFP + "\"");
			return null;
		}
		IServerMessage bNGPAAAKBOP = null;
		bNGPAAAKBOP = (dictionary.ContainsKey("C") ? new MultiMessage() : (dictionary.ContainsKey("E") ? ((IServerMessage)new FailureMessage()) : ((IServerMessage)new ResultMessage())));
		bNGPAAAKBOP.Parse(dictionary);
		return bNGPAAAKBOP;
	}
}
