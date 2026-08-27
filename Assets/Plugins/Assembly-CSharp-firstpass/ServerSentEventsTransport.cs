using System;

public sealed class ServerSentEventsTransport : PostSendTransportBase
{
	private EventSource BFKJAGPIDJO;

	public override bool ODFCAGMNOHK
	{
		get
		{
			return IBMJBEKAIAH();
		}
	}

	public ServerSentEventsTransport(Connection EPDOEDFFPFD)
		: base("serverSentEvents", EPDOEDFFPFD)
	{
	}

	public override bool IBMJBEKAIAH()
	{
		return true;
	}

	public override AHLJIMDEAJD get_Type()
	{
		return AHLJIMDEAJD.ServerSentEvents;
	}

	public override void NDCILHIAPIK()
	{
		if (BFKJAGPIDJO != null)
		{
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("ServerSentEventsTransport", "Start - EventSource already created!");
			return;
		}
		if (FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting)
		{
			set_State(LJLKMCGDKJK.Connecting);
		}
		FHIEGKMHOCC lFLGCDNKNJI = ((FLBBFDNHJAJ() != LJLKMCGDKJK.Reconnecting) ? FHIEGKMHOCC.Connect : FHIEGKMHOCC.Reconnect);
		Uri kJHNCLAJMLO = BAFGHLCPPHM().BuildUri(lFLGCDNKNJI, this);
		BFKJAGPIDJO = new EventSource(kJHNCLAJMLO);
		BFKJAGPIDJO.LMOIENENDCP(AHBKKGHHAJH);
		BFKJAGPIDJO.LIMLEFJPHPP(NDPKMPBOIGA);
		BFKJAGPIDJO.BJDMHEHILEO(NFOPKNNGAON);
		BFKJAGPIDJO.IDCIMGLDBJG(GAHKIGNMDKI);
		BFKJAGPIDJO.CKMLLHFIEJG((EventSource LDKKPKBGFOK) => false);
		BFKJAGPIDJO.LAJCMNNNIIM();
	}

	public override void Stop()
	{
		BFKJAGPIDJO.IIGDNCOBGDB(AHBKKGHHAJH);
		BFKJAGPIDJO.FEJIPPJIAHH(NDPKMPBOIGA);
		BFKJAGPIDJO.LEIDAIFMPCE(NFOPKNNGAON);
		BFKJAGPIDJO.OIBOHOKKFKE(GAHKIGNMDKI);
		BFKJAGPIDJO.Close();
		BFKJAGPIDJO = null;
	}

	protected override void HHLGNIDNLNG()
	{
	}

	public override void AKLEEMEHBIC()
	{
		base.AKLEEMEHBIC();
		BFKJAGPIDJO.Close();
	}

	protected override void NGGKNLJALML()
	{
		if (FLBBFDNHJAJ() == LJLKMCGDKJK.Closing)
		{
			set_State(LJLKMCGDKJK.Closed);
		}
	}

	private void AHBKKGHHAJH(EventSource GLFHBCIPCBD)
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "OnEventSourceOpen");
	}

	private void NDPKMPBOIGA(EventSource GLFHBCIPCBD, Message LIOGIBJBHAH)
	{
		if (LIOGIBJBHAH.CHIGLEKCFFN().Equals("initialized"))
		{
			PIGDCLOPNKJ();
			return;
		}
		IServerMessage bNGPAAAKBOP = TransportBase.Parse(BAFGHLCPPHM().IBNMFHGHIBI(), LIOGIBJBHAH.CHIGLEKCFFN());
		if (bNGPAAAKBOP != null)
		{
			BAFGHLCPPHM().OnMessage(bNGPAAAKBOP);
		}
	}

	private void NFOPKNNGAON(EventSource GLFHBCIPCBD, string JDONBAPIJCG)
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "OnEventSourceError");
		if (FLBBFDNHJAJ() == LJLKMCGDKJK.Reconnecting)
		{
			NDCILHIAPIK();
		}
		else if (FLBBFDNHJAJ() != LJLKMCGDKJK.Closed)
		{
			if (FLBBFDNHJAJ() == LJLKMCGDKJK.Closing)
			{
				set_State(LJLKMCGDKJK.Closed);
			}
			else
			{
				BAFGHLCPPHM().Error(JDONBAPIJCG);
			}
		}
	}

	private void GAHKIGNMDKI(EventSource GLFHBCIPCBD)
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "OnEventSourceClosed");
		NFOPKNNGAON(GLFHBCIPCBD, "EventSource Closed!");
	}
}
