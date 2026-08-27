public enum WebSocketStausCodes : uint
{
	NormalClosure = 1000u,
	GoingAway = 1001u,
	ProtocolError = 1002u,
	WrongDataType = 1003u,
	Reserved = 1004u,
	NoStatusCode = 1005u,
	ClosedAbnormally = 1006u,
	DataError = 1007u,
	PolicyError = 1008u,
	TooBigMessage = 1009u,
	ExtensionExpected = 1010u,
	WrongRequest = 1011u,
	TLSHandshakeError = 1015u
}
