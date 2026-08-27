using System.IO;

public class HTTPProxyResponse : HTTPResponse
{
	internal HTTPProxyResponse(HTTPRequest ONOCIELLAPL, Stream ABJIEFMMIEK, bool IBIIADCLKCH, bool PEAJIKCANHP)
		: base(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP)
	{
	}

	internal override bool Receive(int JHFPNBPNHEH = -1, bool NDCKHEGBAGO = false)
	{
		return base.Receive(JHFPNBPNHEH, false);
	}
}
