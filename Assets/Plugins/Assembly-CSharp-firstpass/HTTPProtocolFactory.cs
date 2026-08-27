using System;
using System.IO;

internal static class HTTPProtocolFactory
{
	public static HTTPResponse Get(OBBKIBFJEMI ENLHAIGCCBO, HTTPRequest ONOCIELLAPL, Stream ABJIEFMMIEK, bool IBIIADCLKCH, bool PEAJIKCANHP)
	{
		switch (ENLHAIGCCBO)
		{
		case OBBKIBFJEMI.WebSocket:
			return new WebSocketResponse(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP);
		case OBBKIBFJEMI.ServerSentEvents:
			return new EventSourceResponse(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP);
		default:
			return new HTTPResponse(ONOCIELLAPL, ABJIEFMMIEK, IBIIADCLKCH, PEAJIKCANHP);
		}
	}

	public static OBBKIBFJEMI AOMOKHPFJFA(Uri KJHNCLAJMLO)
	{
		switch (KJHNCLAJMLO.Scheme.ToLowerInvariant())
		{
		case "ws":
		case "wss":
			return OBBKIBFJEMI.WebSocket;
		default:
			return OBBKIBFJEMI.HTTP;
		}
	}

	public static bool IsSecureProtocol(Uri KJHNCLAJMLO)
	{
		switch (KJHNCLAJMLO.Scheme.ToLowerInvariant())
		{
		case "https":
		case "wss":
			return true;
		default:
			return false;
		}
	}
}
