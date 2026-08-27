using System.Collections.Generic;

public abstract class PostSendTransportBase : TransportBase
{
	protected List<HTTPRequest> BLKGCEOGHGB = new List<HTTPRequest>();

	public PostSendTransportBase(string name, Connection EPDOEDFFPFD)
		: base(name, EPDOEDFFPFD)
	{
	}

	protected override void SendImpl(string EMDHMHOKGFP)
	{
		HTTPRequest iPLGNIDJDCF = new HTTPRequest(BAFGHLCPPHM().BuildUri(FHIEGKMHOCC.Send, this), LAAFHDKKJFL.Post, true, true, HPKOMAEBJGP);
		iPLGNIDJDCF.OJCFIIONEKJ(AIEMPPBDGNH.UrlEncoded);
		iPLGNIDJDCF.AddField("data", EMDHMHOKGFP);
		BAFGHLCPPHM().PrepareRequest(iPLGNIDJDCF, FHIEGKMHOCC.Send);
		iPLGNIDJDCF.INEEHPCAICE(-1);
		iPLGNIDJDCF.Send();
		BLKGCEOGHGB.Add(iPLGNIDJDCF);
	}

	private void HPKOMAEBJGP(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		BLKGCEOGHGB.Remove(CGOIOKHEGOE);
		string text = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Transport - " + get_Name(), "Send - Request Finished Successfully! " + BEIGFGCBICO.DPBLPGKOEJB());
				if (!string.IsNullOrEmpty(BEIGFGCBICO.DPBLPGKOEJB()))
				{
					IServerMessage bNGPAAAKBOP = TransportBase.Parse(BAFGHLCPPHM().IBNMFHGHIBI(), BEIGFGCBICO.DPBLPGKOEJB());
					if (bNGPAAAKBOP != null)
					{
						BAFGHLCPPHM().OnMessage(bNGPAAAKBOP);
					}
				}
			}
			else
			{
				text = string.Format("Send - Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			}
			break;
		case CFGBMHKCENK.Error:
			text = "Send - Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.Aborted:
			text = "Send - Request Aborted!";
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = "Send - Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			text = "Send - Processing the request Timed Out!";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			BAFGHLCPPHM().Error(text);
		}
	}
}
