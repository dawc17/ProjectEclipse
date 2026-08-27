internal interface IManager
{
	void Remove(Socket JLEACANCMJF);

	void Close(bool IEPJILJMNDN = true);

	void TryToReconnect();

	bool OnTransportConnected(ITransport CHMELBKHOPP);

	void OnTransportError(ITransport DLAOOGHJGBI, string KEPBNIIECPN);

	void SendPacket(Packet NPKADBPBKIG);

	void OnPacket(Packet NPKADBPBKIG);

	void EmitEvent(string DOPHKKGNAEF, params object[] LKIOKGCNKHE);

	void EmitEvent(ECDAJBEFCAH LFLGCDNKNJI, params object[] LKIOKGCNKHE);

	void EmitError(CCCOMMIFIMB GNKCGOGKAEK, string CKEHOEGLMBM);

	void EmitAll(string DOPHKKGNAEF, params object[] LKIOKGCNKHE);
}
