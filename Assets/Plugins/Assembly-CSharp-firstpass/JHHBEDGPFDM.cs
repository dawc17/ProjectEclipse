using System;

public sealed class JHHBEDGPFDM : TransportBase
{
	private WebSocket GPDLJHEAEDF;

	public override bool ODFCAGMNOHK
	{
		get
		{
			return IBMJBEKAIAH();
		}
	}

	public JHHBEDGPFDM(Connection MDGFGCDPGFI)
		: base("webSockets", MDGFGCDPGFI)
	{
	}

	public override bool IBMJBEKAIAH()
	{
		return true;
	}

	public override AHLJIMDEAJD get_Type()
	{
		return AHLJIMDEAJD.WebSocket;
	}

	public override void NDCILHIAPIK()
	{
		if (GPDLJHEAEDF != null)
		{
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("WebSocketTransport", "Start - WebSocket already created!");
			return;
		}
		if (FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting)
		{
			set_State(LJLKMCGDKJK.Connecting);
		}
		FHIEGKMHOCC lFLGCDNKNJI = ((FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting) ? FHIEGKMHOCC.Connect : FHIEGKMHOCC.Reconnect);
		Uri kJHNCLAJMLO = BAFGHLCPPHM().BuildUri(lFLGCDNKNJI, this);
		GPDLJHEAEDF = new WebSocket(kJHNCLAJMLO);
		WebSocket gPDLJHEAEDF = GPDLJHEAEDF;
		gPDLJHEAEDF.HKBKFMIBCED = (BNIEFDKHAJN)Delegate.Combine(gPDLJHEAEDF.HKBKFMIBCED, new BNIEFDKHAJN(AOKIEKEJPMA));
		WebSocket gPDLJHEAEDF2 = GPDLJHEAEDF;
		gPDLJHEAEDF2.OnMessage = (KCEBOGOANEH)Delegate.Combine(gPDLJHEAEDF2.OnMessage, new KCEBOGOANEH(DHJPKAIAILI));
		WebSocket gPDLJHEAEDF3 = GPDLJHEAEDF;
		gPDLJHEAEDF3.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(gPDLJHEAEDF3.OnClosed, new OnWebSocketClosedDelegate(MCJELOMLJFG));
		WebSocket gPDLJHEAEDF4 = GPDLJHEAEDF;
		gPDLJHEAEDF4.NKNFDKGPPAJ = (JFCEKCIHELB)Delegate.Combine(gPDLJHEAEDF4.NKNFDKGPPAJ, new JFCEKCIHELB(PMAEHHKJPFH));
		BAFGHLCPPHM().PrepareRequest(GPDLJHEAEDF.KGBEGJJPCKC(), lFLGCDNKNJI);
		GPDLJHEAEDF.LAJCMNNNIIM();
	}

	protected override void SendImpl(string EMDHMHOKGFP)
	{
		if (GPDLJHEAEDF != null && GPDLJHEAEDF.DJKKJPNLOAE())
		{
			GPDLJHEAEDF.Send(EMDHMHOKGFP);
		}
	}

	public override void Stop()
	{
		if (GPDLJHEAEDF != null && GPDLJHEAEDF.DJKKJPNLOAE())
		{
			GPDLJHEAEDF.HKBKFMIBCED = null;
			GPDLJHEAEDF.OnMessage = null;
			GPDLJHEAEDF.OnClosed = null;
			GPDLJHEAEDF.NKNFDKGPPAJ = null;
			GPDLJHEAEDF.Close();
			GPDLJHEAEDF = null;
		}
	}

	protected override void HHLGNIDNLNG()
	{
	}

	protected override void NGGKNLJALML()
	{
		if (GPDLJHEAEDF != null && GPDLJHEAEDF.DJKKJPNLOAE())
		{
			GPDLJHEAEDF.Close();
			GPDLJHEAEDF = null;
		}
	}

	private void AOKIEKEJPMA(WebSocket ILNFPNFEOCL)
	{
		if (ILNFPNFEOCL == GPDLJHEAEDF)
		{
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocketTransport", "WSocket_OnOpen");
			PIGDCLOPNKJ();
		}
	}

	private void DHJPKAIAILI(WebSocket ILNFPNFEOCL, string LIOGIBJBHAH)
	{
		if (ILNFPNFEOCL == GPDLJHEAEDF)
		{
			IServerMessage bNGPAAAKBOP = TransportBase.Parse(BAFGHLCPPHM().IBNMFHGHIBI(), LIOGIBJBHAH);
			if (bNGPAAAKBOP != null)
			{
				BAFGHLCPPHM().OnMessage(bNGPAAAKBOP);
			}
		}
	}

	private void MCJELOMLJFG(WebSocket ILNFPNFEOCL, ushort KJPGKHJNOMC, string LIOGIBJBHAH)
	{
		if (ILNFPNFEOCL == GPDLJHEAEDF)
		{
			string text = KJPGKHJNOMC + " : " + LIOGIBJBHAH;
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocketTransport", "WSocket_OnClosed " + text);
			if (FLBBFDNHJAJ() == LJLKMCGDKJK.Closing)
			{
				set_State(LJLKMCGDKJK.Closed);
			}
			else
			{
				BAFGHLCPPHM().Error(text);
			}
		}
	}

	private void PMAEHHKJPFH(WebSocket ILNFPNFEOCL, string NEPOLDCKNJL)
	{
		if (ILNFPNFEOCL == GPDLJHEAEDF)
		{
			if (FLBBFDNHJAJ() == LJLKMCGDKJK.Closing || FLBBFDNHJAJ() == LJLKMCGDKJK.Closed)
			{
				JGGILBCPENL();
				return;
			}
			HTTPManager.MBBMPNDDPIH().Error("WebSocketTransport", "WSocket_OnError " + NEPOLDCKNJL);
			BAFGHLCPPHM().Error(NEPOLDCKNJL);
		}
	}
}
