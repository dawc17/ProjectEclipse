using System;

public interface IConnection
{
	NegotiationData IIPFKKBEANI { get; }

	AJAIAKCIJIJ CBGKGGCMHLL { get; set; }

	NegotiationData EOBPEOEMEDB();

	AJAIAKCIJIJ IBNMFHGHIBI();

	void LPEPILDNMNE(AJAIAKCIJIJ value);

	void OnMessage(IServerMessage CKEHOEGLMBM);

	void TransportStarted();

	void TransportReconnected();

	void TransportAborted();

	void Error(string NEPOLDCKNJL);

	Uri BuildUri(FHIEGKMHOCC LFLGCDNKNJI);

	Uri BuildUri(FHIEGKMHOCC LFLGCDNKNJI, TransportBase CHMELBKHOPP);

	HTTPRequest PrepareRequest(HTTPRequest CGOIOKHEGOE, FHIEGKMHOCC LFLGCDNKNJI);

	string ParseResponse(string GHCCHADLAEK);
}
