using System;
using System.Collections.Generic;
using System.Diagnostics;

internal sealed class KMLMEACFJGA : ITransport
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FGMEPPMFFKG MKHEFCIEOCA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SocketManager JNNOJIEMLEK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KLEAIPJDFKO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private WebSocket HKKDEGEKIGG;

	private Packet IOPFFNBKCPH;

	private byte[] Buffer;

	public FGMEPPMFFKG AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		private set
		{
			set_State(value);
		}
	}

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

	public bool KNCJBAHIAGI
	{
		get
		{
			return LILBDKKEHCE();
		}
		private set
		{
			set_IsRequestInProgress(value);
		}
	}

	public WebSocket DBDBDNMJPDA
	{
		get
		{
			return GBFNHHJALAN();
		}
		private set
		{
			BCKIPGEEHKC(value);
		}
	}

	public KMLMEACFJGA(SocketManager BJGMPDIKEJC)
	{
		set_State(FGMEPPMFFKG.Closed);
		CMOJGLBBCKC(BJGMPDIKEJC);
	}

	public FGMEPPMFFKG FLBBFDNHJAJ()
	{
		return MKHEFCIEOCA;
	}

	private void set_State(FGMEPPMFFKG value)
	{
		MKHEFCIEOCA = value;
	}

	public SocketManager HLBNHJADOMP()
	{
		return JNNOJIEMLEK;
	}

	private void CMOJGLBBCKC(SocketManager value)
	{
		JNNOJIEMLEK = value;
	}

	public bool LILBDKKEHCE()
	{
		return KLEAIPJDFKO;
	}

	private void set_IsRequestInProgress(bool value)
	{
		KLEAIPJDFKO = value;
	}

	public WebSocket GBFNHHJALAN()
	{
		return HKKDEGEKIGG;
	}

	private void BCKIPGEEHKC(WebSocket value)
	{
		HKKDEGEKIGG = value;
	}

	public void LAJCMNNNIIM()
	{
		if (FLBBFDNHJAJ() == FGMEPPMFFKG.Closed)
		{
			Uri kJHNCLAJMLO = new Uri(string.Format("{0}?transport=websocket&sid={1}{2}", new UriBuilder("ws", HLBNHJADOMP().OJBDMGBGJMA().Host, HLBNHJADOMP().OJBDMGBGJMA().Port, HLBNHJADOMP().OJBDMGBGJMA().PathAndQuery).Uri.ToString(), HLBNHJADOMP().EIOHJJFBIAL().EDLOIOOBPAJ(), HLBNHJADOMP().HLHJJJGJEEL().DKJAFHAOKDB() ? string.Empty : HLBNHJADOMP().HLHJJJGJEEL().LEKAOBKGMPF()));
			BCKIPGEEHKC(new WebSocket(kJHNCLAJMLO));
			GBFNHHJALAN().HKBKFMIBCED = HKBKFMIBCED;
			GBFNHHJALAN().OnMessage = OnMessage;
			GBFNHHJALAN().OnBinary = OnBinary;
			GBFNHHJALAN().OnError = OnError;
			GBFNHHJALAN().OnClosed = OnClosed;
			GBFNHHJALAN().LAJCMNNNIIM();
			set_State(FGMEPPMFFKG.Connecting);
		}
	}

	public void Close()
	{
		if (FLBBFDNHJAJ() != FGMEPPMFFKG.Closed)
		{
			set_State(FGMEPPMFFKG.Closed);
			GBFNHHJALAN().Close();
			BCKIPGEEHKC(null);
		}
	}

	public void GNGIDEJLNCF()
	{
	}

	private void HKBKFMIBCED(WebSocket IIBIPJJLEGJ)
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocketTransport", "OnOpen");
		set_State(FGMEPPMFFKG.Opening);
		Send(new Packet(HJDLGPHLPNF.Ping, ECDAJBEFCAH.Unknown, "/", "probe"));
	}

	private void OnMessage(WebSocket IIBIPJJLEGJ, string LIOGIBJBHAH)
	{
		if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
		{
			HTTPManager.MBBMPNDDPIH().JMHHKELODIO("WebSocketTransport", "OnMessage: " + LIOGIBJBHAH);
		}
		try
		{
			Packet cMPKPLIGKLC = new Packet(LIOGIBJBHAH);
			if (cMPKPLIGKLC.AIHPGOGLBCE() == 0)
			{
				OnPacket(cMPKPLIGKLC);
			}
			else
			{
				IOPFFNBKCPH = cMPKPLIGKLC;
			}
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("WebSocketTransport", "OnMessage", mPFFFAOGBJE);
		}
	}

	private void OnBinary(WebSocket IIBIPJJLEGJ, byte[] data)
	{
		if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
		{
			HTTPManager.MBBMPNDDPIH().JMHHKELODIO("WebSocketTransport", "OnBinary");
		}
		if (IOPFFNBKCPH == null)
		{
			return;
		}
		IOPFFNBKCPH.AddAttachmentFromServer(data, false);
		if (!IOPFFNBKCPH.DGDMLLFEAME())
		{
			return;
		}
		try
		{
			OnPacket(IOPFFNBKCPH);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("WebSocketTransport", "OnBinary", mPFFFAOGBJE);
		}
		finally
		{
			IOPFFNBKCPH = null;
		}
	}

	private void OnError(WebSocket IIBIPJJLEGJ, Exception MPFFFAOGBJE)
	{
		string text = string.Empty;
		if (MPFFFAOGBJE != null)
		{
			text = MPFFFAOGBJE.Message + " " + MPFFFAOGBJE.StackTrace;
		}
		else
		{
			switch (IIBIPJJLEGJ.KGBEGJJPCKC().FLBBFDNHJAJ())
			{
			case CFGBMHKCENK.Finished:
				text = ((!IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().AICKPAMONBH() && IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().KNMDPGBPNED() != 101) ? string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().KNMDPGBPNED(), IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().DCKPMHKDLEJ(), IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().DPBLPGKOEJB()) : string.Format("Request finished. Status Code: {0} Message: {1}", IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().KNMDPGBPNED()
					.ToString(), IIBIPJJLEGJ.KGBEGJJPCKC().POGDKNCHIBG().DCKPMHKDLEJ()));
				break;
			case CFGBMHKCENK.Error:
				text = (("Request Finished with Error! : " + IIBIPJJLEGJ.KGBEGJJPCKC().IEFGFKFHNMD() == null) ? string.Empty : (IIBIPJJLEGJ.KGBEGJJPCKC().IEFGFKFHNMD().Message + " " + IIBIPJJLEGJ.KGBEGJJPCKC().IEFGFKFHNMD().StackTrace));
				break;
			case CFGBMHKCENK.Aborted:
				text = "Request Aborted!";
				break;
			case CFGBMHKCENK.ConnectionTimedOut:
				text = "Connection Timed Out!";
				break;
			case CFGBMHKCENK.TimedOut:
				text = "Processing the request Timed Out!";
				break;
			}
		}
		HTTPManager.MBBMPNDDPIH().Error("WebSocketTransport", "OnError: " + text);
		((IManager)HLBNHJADOMP()).OnTransportError((ITransport)this, text);
	}

	private void OnClosed(WebSocket IIBIPJJLEGJ, ushort KJPGKHJNOMC, string LIOGIBJBHAH)
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocketTransport", "OnClosed");
		Close();
		((IManager)HLBNHJADOMP()).TryToReconnect();
	}

	public void Send(Packet NPKADBPBKIG)
	{
		if (FLBBFDNHJAJ() == FGMEPPMFFKG.Closed || FLBBFDNHJAJ() == FGMEPPMFFKG.Paused)
		{
			return;
		}
		string text = NPKADBPBKIG.Encode();
		if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.All)
		{
			HTTPManager.MBBMPNDDPIH().JMHHKELODIO("WebSocketTransport", "Send: " + text);
		}
		if (NPKADBPBKIG.AIHPGOGLBCE() != 0 || (NPKADBPBKIG.BINAPGLGAGE() != null && NPKADBPBKIG.BINAPGLGAGE().Count != 0))
		{
			if (NPKADBPBKIG.BINAPGLGAGE() == null)
			{
				throw new ArgumentException("packet.Attachments are null!");
			}
			if (NPKADBPBKIG.AIHPGOGLBCE() != NPKADBPBKIG.BINAPGLGAGE().Count)
			{
				throw new ArgumentException("packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
			}
		}
		GBFNHHJALAN().Send(text);
		if (NPKADBPBKIG.AIHPGOGLBCE() == 0)
		{
			return;
		}
		int num = NPKADBPBKIG.BINAPGLGAGE()[0].Length + 1;
		for (int i = 1; i < NPKADBPBKIG.BINAPGLGAGE().Count; i++)
		{
			if (NPKADBPBKIG.BINAPGLGAGE()[i].Length + 1 > num)
			{
				num = NPKADBPBKIG.BINAPGLGAGE()[i].Length + 1;
			}
		}
		if (Buffer == null || Buffer.Length < num)
		{
			Array.Resize(ref Buffer, num);
		}
		for (int j = 0; j < NPKADBPBKIG.AIHPGOGLBCE(); j++)
		{
			Buffer[0] = 4;
			Array.Copy(NPKADBPBKIG.BINAPGLGAGE()[j], 0, Buffer, 1, NPKADBPBKIG.BINAPGLGAGE()[j].Length);
			GBFNHHJALAN().Send(Buffer, 0uL, (ulong)NPKADBPBKIG.BINAPGLGAGE()[j].Length + 1uL);
		}
	}

	public void Send(List<Packet> DPGGBKDLDJE)
	{
		for (int i = 0; i < DPGGBKDLDJE.Count; i++)
		{
			Send(DPGGBKDLDJE[i]);
		}
		DPGGBKDLDJE.Clear();
	}

	private void OnPacket(Packet NPKADBPBKIG)
	{
		switch (NPKADBPBKIG.FFJBNPEOAHI())
		{
		case HJDLGPHLPNF.Message:
			if (NPKADBPBKIG.CMEHGNCCCIN() == ECDAJBEFCAH.Connect && FLBBFDNHJAJ() == FGMEPPMFFKG.Opening)
			{
				set_State(FGMEPPMFFKG.Open);
				if (!((IManager)HLBNHJADOMP()).OnTransportConnected((ITransport)this))
				{
					return;
				}
			}
			break;
		case HJDLGPHLPNF.Pong:
			if (NPKADBPBKIG.NLHGDFGNIHB() == "probe")
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("WebSocketTransport", "\"probe\" packet received, sending Upgrade packet");
				Send(new Packet(HJDLGPHLPNF.Upgrade, ECDAJBEFCAH.Event, "/", string.Empty));
			}
			break;
		}
		((IManager)HLBNHJADOMP()).OnPacket(NPKADBPBKIG);
	}
}
