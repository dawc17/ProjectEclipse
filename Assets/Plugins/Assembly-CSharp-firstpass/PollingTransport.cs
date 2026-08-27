using System;

public sealed class PollingTransport : PostSendTransportBase, IHeartbeat
{
	private DateTime LastPoll;

	private TimeSpan EMJECEPDOJL;

	private TimeSpan NNAPNJNKFEG;

	private HTTPRequest KMAGLDDNDHP;

	public override bool ODFCAGMNOHK
	{
		get
		{
			return IBMJBEKAIAH();
		}
	}

	public PollingTransport(Connection MDGFGCDPGFI)
		: base("longPolling", MDGFGCDPGFI)
	{
		LastPoll = DateTime.MinValue;
		NNAPNJNKFEG = MDGFGCDPGFI.EOBPEOEMEDB().LFLAILLBGOF() + TimeSpan.FromSeconds(10.0);
	}

	public override bool IBMJBEKAIAH()
	{
		return false;
	}

	public override AHLJIMDEAJD get_Type()
	{
		return AHLJIMDEAJD.LongPoll;
	}

	public override void NDCILHIAPIK()
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Sending Open Request");
		if (FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting)
		{
			set_State(LJLKMCGDKJK.Connecting);
		}
		FHIEGKMHOCC lFLGCDNKNJI = ((FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting) ? FHIEGKMHOCC.Connect : FHIEGKMHOCC.Reconnect);
		HTTPRequest iPLGNIDJDCF = new HTTPRequest(BAFGHLCPPHM().BuildUri(lFLGCDNKNJI, this), LAAFHDKKJFL.Get, true, true, HJFCHFIBCPH);
		BAFGHLCPPHM().PrepareRequest(iPLGNIDJDCF, lFLGCDNKNJI);
		iPLGNIDJDCF.Send();
	}

	public override void Stop()
	{
		HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		if (KMAGLDDNDHP != null)
		{
			KMAGLDDNDHP.AKLEEMEHBIC();
			KMAGLDDNDHP = null;
		}
	}

	protected override void HHLGNIDNLNG()
	{
		LastPoll = DateTime.UtcNow;
		HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
	}

	protected override void NGGKNLJALML()
	{
		HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
	}

	private void HJFCHFIBCPH(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		string text = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Connect - Request Finished Successfully! " + BEIGFGCBICO.DPBLPGKOEJB());
				PIGDCLOPNKJ();
				IServerMessage bNGPAAAKBOP = TransportBase.Parse(BAFGHLCPPHM().IBNMFHGHIBI(), BEIGFGCBICO.DPBLPGKOEJB());
				if (bNGPAAAKBOP != null)
				{
					BAFGHLCPPHM().OnMessage(bNGPAAAKBOP);
					MultiMessage eIKBBLMECNO = bNGPAAAKBOP as MultiMessage;
					if (eIKBBLMECNO != null && eIKBBLMECNO.LNCCPGIEPOH().HasValue)
					{
						EMJECEPDOJL = eIKBBLMECNO.LNCCPGIEPOH().Value;
					}
				}
			}
			else
			{
				text = string.Format("Connect - Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			}
			break;
		case CFGBMHKCENK.Error:
			text = "Connect - Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			text = "Connect - Request Aborted!";
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = "Connect - Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			text = "Connect - Processing the request Timed Out!";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			BAFGHLCPPHM().Error(text);
		}
	}

	private void BMOLBAJPGDJ(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		if (CGOIOKHEGOE.FLBBFDNHJAJ() == CFGBMHKCENK.Aborted)
		{
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("Transport - " + get_Name(), "Poll - Request Aborted!");
			return;
		}
		KMAGLDDNDHP = null;
		string text = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Poll - Request Finished Successfully! " + BEIGFGCBICO.DPBLPGKOEJB());
				IServerMessage bNGPAAAKBOP = TransportBase.Parse(BAFGHLCPPHM().IBNMFHGHIBI(), BEIGFGCBICO.DPBLPGKOEJB());
				if (bNGPAAAKBOP != null)
				{
					BAFGHLCPPHM().OnMessage(bNGPAAAKBOP);
					MultiMessage eIKBBLMECNO = bNGPAAAKBOP as MultiMessage;
					if (eIKBBLMECNO != null && eIKBBLMECNO.LNCCPGIEPOH().HasValue)
					{
						EMJECEPDOJL = eIKBBLMECNO.LNCCPGIEPOH().Value;
					}
					LastPoll = DateTime.UtcNow;
				}
			}
			else
			{
				text = string.Format("Poll - Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			}
			break;
		case CFGBMHKCENK.Error:
			text = "Poll - Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = "Poll - Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			text = "Poll - Processing the request Timed Out!";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			BAFGHLCPPHM().Error(text);
		}
	}

	private void GNGIDEJLNCF()
	{
		KMAGLDDNDHP = new HTTPRequest(BAFGHLCPPHM().BuildUri(FHIEGKMHOCC.Poll, this), LAAFHDKKJFL.Get, true, true, BMOLBAJPGDJ);
		BAFGHLCPPHM().PrepareRequest(KMAGLDDNDHP, FHIEGKMHOCC.Poll);
		KMAGLDDNDHP.DKLGPGDJPGO(NNAPNJNKFEG);
		KMAGLDDNDHP.Send();
	}

	void IHeartbeat.OnHeartbeatUpdate(TimeSpan OJOKANCMPLG)
	{
		LJLKMCGDKJK lJLKMCGDKJK = FLBBFDNHJAJ();
		if (lJLKMCGDKJK == LJLKMCGDKJK.Started && KMAGLDDNDHP == null && DateTime.UtcNow >= LastPoll + EMJECEPDOJL + BAFGHLCPPHM().EOBPEOEMEDB().NCMIDNBFDID())
		{
			GNGIDEJLNCF();
		}
	}
}
