using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class Socket : ISocket
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SocketManager JNNOJIEMLEK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HCHALPNMNMK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KNBOFMPBFKN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool BABMNMGKNMB;

	private Dictionary<int, LKDPANKMCOG> CKMOJINNGDG;

	private EventTable FNFJJIMFOAD;

	private List<object> arguments = new List<object>();

	public SocketManager CPOHGNDIBJD
	{
		get
		{
			return HLBNHJADOMP();
		}
		private set
		{
			CMOJGLBBCKC(value);
		}
	}

	public bool PLCIGHLBOPP
	{
		get
		{
			return DJKKJPNLOAE();
		}
		private set
		{
			JDHIENHFJCE(value);
		}
	}

	public bool KCIILHEGDAG
	{
		get
		{
			return CAACHPIAHIJ();
		}
		set
		{
			FEDKJGINJID(value);
		}
	}

	internal Socket(string JBALIKEKHGL, SocketManager BJGMPDIKEJC)
	{
		set_Namespace(JBALIKEKHGL);
		CMOJGLBBCKC(BJGMPDIKEJC);
		JDHIENHFJCE(false);
		FEDKJGINJID(true);
		FNFJJIMFOAD = new EventTable(this);
	}

	public SocketManager HLBNHJADOMP()
	{
		return JNNOJIEMLEK;
	}

	private void CMOJGLBBCKC(SocketManager value)
	{
		JNNOJIEMLEK = value;
	}

	public string IONIEDIPEGB()
	{
		return HCHALPNMNMK;
	}

	private void set_Namespace(string value)
	{
		HCHALPNMNMK = value;
	}

	public bool DJKKJPNLOAE()
	{
		return KNBOFMPBFKN;
	}

	private void JDHIENHFJCE(bool value)
	{
		KNBOFMPBFKN = value;
	}

	public bool CAACHPIAHIJ()
	{
		return BABMNMGKNMB;
	}

	public void FEDKJGINJID(bool value)
	{
		BABMNMGKNMB = value;
	}

	void ISocket.Open()
	{
		if (HLBNHJADOMP().FLBBFDNHJAJ() == SocketManager.IFLBJIKPLOL.Open)
		{
			OLLDDGGCLGO(HLBNHJADOMP().PDJFKOBODHH(), null);
			return;
		}
		HLBNHJADOMP().PDJFKOBODHH().Off("connect", OLLDDGGCLGO);
		HLBNHJADOMP().PDJFKOBODHH().JPJAFMLNALO("connect", OLLDDGGCLGO);
		if (HLBNHJADOMP().HLHJJJGJEEL().JLCKLGDFADC() && HLBNHJADOMP().FLBBFDNHJAJ() == SocketManager.IFLBJIKPLOL.Initial)
		{
			HLBNHJADOMP().Open();
		}
	}

	public void Disconnect()
	{
		((ISocket)this).Disconnect(true);
	}

	void ISocket.Disconnect(bool GGONLJPAABO)
	{
		if (DJKKJPNLOAE())
		{
			Packet nPKADBPBKIG = new Packet(HJDLGPHLPNF.Message, ECDAJBEFCAH.Disconnect, IONIEDIPEGB(), string.Empty);
			((IManager)HLBNHJADOMP()).SendPacket(nPKADBPBKIG);
			JDHIENHFJCE(false);
			((ISocket)this).OnPacket(nPKADBPBKIG);
		}
		if (CKMOJINNGDG != null)
		{
			CKMOJINNGDG.Clear();
		}
		if (GGONLJPAABO)
		{
			FNFJJIMFOAD.Clear();
			((IManager)HLBNHJADOMP()).Remove(this);
		}
	}

	public Socket Emit(string DOPHKKGNAEF, params object[] LKIOKGCNKHE)
	{
		return Emit(DOPHKKGNAEF, null, LKIOKGCNKHE);
	}

	public Socket Emit(string DOPHKKGNAEF, LKDPANKMCOG callback, params object[] LKIOKGCNKHE)
	{
		if (EventNames.IsBlacklisted(DOPHKKGNAEF))
		{
			throw new ArgumentException("Blacklisted event: " + DOPHKKGNAEF);
		}
		arguments.Clear();
		arguments.Add(DOPHKKGNAEF);
		List<byte[]> list = null;
		if (LKIOKGCNKHE != null && LKIOKGCNKHE.Length > 0)
		{
			int num = 0;
			for (int i = 0; i < LKIOKGCNKHE.Length; i++)
			{
				byte[] array = LKIOKGCNKHE[i] as byte[];
				if (array != null)
				{
					if (list == null)
					{
						list = new List<byte[]>();
					}
					arguments.Add(string.Format("{{\"_placeholder\":true,\"num\":{0}}}", num++.ToString()));
					list.Add(array);
				}
				else
				{
					arguments.Add(LKIOKGCNKHE[i]);
				}
			}
		}
		string text = null;
		try
		{
			text = HLBNHJADOMP().KCMCCGKJGLE().Encode(arguments);
		}
		catch (Exception ex)
		{
			((ISocket)this).EmitError(CCCOMMIFIMB.Internal, "Error while encoding payload: " + ex.Message + " " + ex.StackTrace);
			return this;
		}
		arguments.Clear();
		if (text == null)
		{
			throw new ArgumentException("Encoding the arguments to JSON failed!");
		}
		int num2 = 0;
		if (callback != null)
		{
			num2 = HLBNHJADOMP().ICKFNGAOMFI();
			if (CKMOJINNGDG == null)
			{
				CKMOJINNGDG = new Dictionary<int, LKDPANKMCOG>();
			}
			CKMOJINNGDG[num2] = callback;
		}
		Packet cMPKPLIGKLC = new Packet(HJDLGPHLPNF.Message, (list != null) ? ECDAJBEFCAH.BinaryEvent : ECDAJBEFCAH.Event, IONIEDIPEGB(), text, 0, num2);
		if (list != null)
		{
			cMPKPLIGKLC.set_Attachments(list);
		}
		((IManager)HLBNHJADOMP()).SendPacket(cMPKPLIGKLC);
		return this;
	}

	public Socket DPILHMMKPHD(Packet FMAMCLDBKFM, params object[] LKIOKGCNKHE)
	{
		if (FMAMCLDBKFM == null)
		{
			throw new ArgumentNullException("originalPacket == null!");
		}
		if (FMAMCLDBKFM.CMEHGNCCCIN() != ECDAJBEFCAH.Event && FMAMCLDBKFM.CMEHGNCCCIN() != ECDAJBEFCAH.BinaryEvent)
		{
			throw new ArgumentException("Wrong packet - you can't send an Ack for a packet with id == 0 and SocketIOEvent != Event or SocketIOEvent != BinaryEvent!");
		}
		arguments.Clear();
		if (LKIOKGCNKHE != null && LKIOKGCNKHE.Length > 0)
		{
			arguments.AddRange(LKIOKGCNKHE);
		}
		string text = null;
		try
		{
			text = HLBNHJADOMP().KCMCCGKJGLE().Encode(arguments);
		}
		catch (Exception ex)
		{
			((ISocket)this).EmitError(CCCOMMIFIMB.Internal, "Error while encoding payload: " + ex.Message + " " + ex.StackTrace);
			return this;
		}
		if (text == null)
		{
			throw new ArgumentException("Encoding the arguments to JSON failed!");
		}
		Packet nPKADBPBKIG = new Packet(HJDLGPHLPNF.Message, (FMAMCLDBKFM.CMEHGNCCCIN() != ECDAJBEFCAH.Event) ? ECDAJBEFCAH.BinaryAck : ECDAJBEFCAH.Ack, IONIEDIPEGB(), text, 0, FMAMCLDBKFM.IMMIJJCLPBO());
		((IManager)HLBNHJADOMP()).SendPacket(nPKADBPBKIG);
		return this;
	}

	public void JPJAFMLNALO(string DOPHKKGNAEF, BLIMHGJLDLD callback)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(DOPHKKGNAEF, callback, false, CAACHPIAHIJ());
	}

	public void JPJAFMLNALO(ECDAJBEFCAH LFLGCDNKNJI, BLIMHGJLDLD callback)
	{
		string dOPHKKGNAEF = EventNames.ICAIODPBKBO(LFLGCDNKNJI);
		FNFJJIMFOAD.DNKHCGPPBAE(dOPHKKGNAEF, callback, false, CAACHPIAHIJ());
	}

	public void JPJAFMLNALO(string DOPHKKGNAEF, BLIMHGJLDLD callback, bool EJDLINOJJIF)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(DOPHKKGNAEF, callback, false, EJDLINOJJIF);
	}

	public void JPJAFMLNALO(ECDAJBEFCAH LFLGCDNKNJI, BLIMHGJLDLD callback, bool EJDLINOJJIF)
	{
		string dOPHKKGNAEF = EventNames.ICAIODPBKBO(LFLGCDNKNJI);
		FNFJJIMFOAD.DNKHCGPPBAE(dOPHKKGNAEF, callback, false, EJDLINOJJIF);
	}

	public void FKIKKBDNCNP(string DOPHKKGNAEF, BLIMHGJLDLD callback)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(DOPHKKGNAEF, callback, true, CAACHPIAHIJ());
	}

	public void FKIKKBDNCNP(ECDAJBEFCAH LFLGCDNKNJI, BLIMHGJLDLD callback)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(EventNames.ICAIODPBKBO(LFLGCDNKNJI), callback, true, CAACHPIAHIJ());
	}

	public void FKIKKBDNCNP(string DOPHKKGNAEF, BLIMHGJLDLD callback, bool EJDLINOJJIF)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(DOPHKKGNAEF, callback, true, EJDLINOJJIF);
	}

	public void FKIKKBDNCNP(ECDAJBEFCAH LFLGCDNKNJI, BLIMHGJLDLD callback, bool EJDLINOJJIF)
	{
		FNFJJIMFOAD.DNKHCGPPBAE(EventNames.ICAIODPBKBO(LFLGCDNKNJI), callback, true, EJDLINOJJIF);
	}

	public void Off()
	{
		FNFJJIMFOAD.Clear();
	}

	public void Off(string DOPHKKGNAEF)
	{
		FNFJJIMFOAD.Unregister(DOPHKKGNAEF);
	}

	public void Off(ECDAJBEFCAH LFLGCDNKNJI)
	{
		Off(EventNames.ICAIODPBKBO(LFLGCDNKNJI));
	}

	public void Off(string DOPHKKGNAEF, BLIMHGJLDLD callback)
	{
		FNFJJIMFOAD.Unregister(DOPHKKGNAEF, callback);
	}

	public void Off(ECDAJBEFCAH LFLGCDNKNJI, BLIMHGJLDLD callback)
	{
		FNFJJIMFOAD.Unregister(EventNames.ICAIODPBKBO(LFLGCDNKNJI), callback);
	}

	void ISocket.OnPacket(Packet NPKADBPBKIG)
	{
		switch (NPKADBPBKIG.CMEHGNCCCIN())
		{
		case ECDAJBEFCAH.Disconnect:
			if (DJKKJPNLOAE())
			{
				JDHIENHFJCE(false);
				Disconnect();
			}
			break;
		case ECDAJBEFCAH.Error:
		{
			bool IBFAPIMOMBA = false;
			Dictionary<string, object> dictionary = Json.Decode(NPKADBPBKIG.NLHGDFGNIHB(), ref IBFAPIMOMBA) as Dictionary<string, object>;
			if (IBFAPIMOMBA)
			{
				Error eOFKDCNBPHO = new Error((CCCOMMIFIMB)Convert.ToInt32(dictionary["code"]), dictionary["message"] as string);
				FNFJJIMFOAD.Call(EventNames.ICAIODPBKBO(ECDAJBEFCAH.Error), NPKADBPBKIG, eOFKDCNBPHO);
				return;
			}
			break;
		}
		}
		FNFJJIMFOAD.Call(NPKADBPBKIG);
		if ((NPKADBPBKIG.CMEHGNCCCIN() != ECDAJBEFCAH.Ack && NPKADBPBKIG.CMEHGNCCCIN() != ECDAJBEFCAH.BinaryAck) || CKMOJINNGDG == null)
		{
			return;
		}
		LKDPANKMCOG value = null;
		if (CKMOJINNGDG.TryGetValue(NPKADBPBKIG.IMMIJJCLPBO(), out value) && value != null)
		{
			try
			{
				value(this, NPKADBPBKIG, NPKADBPBKIG.Decode(HLBNHJADOMP().KCMCCGKJGLE()));
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("Socket", "ackCallback", mPFFFAOGBJE);
			}
		}
		CKMOJINNGDG.Remove(NPKADBPBKIG.IMMIJJCLPBO());
	}

	void ISocket.EmitEvent(ECDAJBEFCAH LFLGCDNKNJI, params object[] LKIOKGCNKHE)
	{
		((ISocket)this).EmitEvent(EventNames.ICAIODPBKBO(LFLGCDNKNJI), LKIOKGCNKHE);
	}

	void ISocket.EmitEvent(string DOPHKKGNAEF, params object[] LKIOKGCNKHE)
	{
		if (!string.IsNullOrEmpty(DOPHKKGNAEF))
		{
			FNFJJIMFOAD.Call(DOPHKKGNAEF, null, LKIOKGCNKHE);
		}
	}

	void ISocket.EmitError(CCCOMMIFIMB GNKCGOGKAEK, string CKEHOEGLMBM)
	{
		((ISocket)this).EmitEvent(ECDAJBEFCAH.Error, new object[1]
		{
			new Error(GNKCGOGKAEK, CKEHOEGLMBM)
		});
	}

	private void OLLDDGGCLGO(Socket JLEACANCMJF, Packet NPKADBPBKIG, params object[] LKIOKGCNKHE)
	{
		if (IONIEDIPEGB() != "/")
		{
			((IManager)HLBNHJADOMP()).SendPacket(new Packet(HJDLGPHLPNF.Message, ECDAJBEFCAH.Connect, IONIEDIPEGB(), string.Empty));
		}
		JDHIENHFJCE(true);
	}
}
