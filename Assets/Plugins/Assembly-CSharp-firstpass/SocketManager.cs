using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

[DefaultMember("Item")]
public sealed class SocketManager : IHeartbeat, IManager
{
	public enum IFLBJIKPLOL
	{
		Initial = 0,
		Closed = 1,
		Opening = 2,
		Open = 3,
		Reconnecting = 4
	}

	public static OOINGNLNJGM MLILENMLJAH = new LBMMAKIBFHD();

	public const int MinProtocolVersion = 4;

	private IFLBJIKPLOL state;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SocketOptions GLJHJKNBNDD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HandshakeData GCKLPLNOHCH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ITransport DCDGJNEKNKE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong GDDMABPJDPP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int CPDNJPDONOL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OOINGNLNJGM NBCJNBIGHCJ;

	private int MFMMDNPNOCE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IFLBJIKPLOL KKPFPDLOEFJ;

	private Dictionary<string, Socket> OABAACFMPFH = new Dictionary<string, Socket>();

	private List<Socket> FADLKIHOHLN = new List<Socket>();

	private List<Packet> LCAAGNNLNKO;

	private DateTime GHAGBCLOOFC = DateTime.MinValue;

	private DateTime ECDDELFMACL = DateTime.MinValue;

	private DateTime GDEMFJODPBO;

	private DateTime BAHEGCMFMJI;

	public IFLBJIKPLOL AFINHOBCHMC
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

	public SocketOptions IFGNHLCDDCO
	{
		get
		{
			return HLHJJJGJEEL();
		}
		private set
		{
			KAFLNEBPLIA(value);
		}
	}

	public Uri GJIGOCNEPME
	{
		get
		{
			return OJBDMGBGJMA();
		}
		private set
		{
			set_Uri(value);
		}
	}

	public HandshakeData CDOOLJJGOAA
	{
		get
		{
			return EIOHJJFBIAL();
		}
		private set
		{
			NLCGDODNKEF(value);
		}
	}

	public ITransport HEFNODJLIBE
	{
		get
		{
			return LODFOKFEAPC();
		}
		private set
		{
			AOJLKJODKMC(value);
		}
	}

	public ulong PHFIIKGKDCF
	{
		get
		{
			return EKBGNBPGFNG();
		}
		internal set
		{
			set_RequestCounter(value);
		}
	}

	public Socket KNPLDJGCAKJ
	{
		get
		{
			return PDJFKOBODHH();
		}
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public Socket get_DLKPBAJDHBO(string JBALIKEKHGL)
	{
		return get_Item(JBALIKEKHGL);
	}

	public int LFJJLGFIOKE
	{
		get
		{
			return IKFMEIGJFCA();
		}
		private set
		{
			set_ReconnectAttempts(value);
		}
	}

	public OOINGNLNJGM GHLNIIFAKCF
	{
		get
		{
			return KCMCCGKJGLE();
		}
		set
		{
			MLMBDFBLPAC(value);
		}
	}

	internal uint KBEDPONBFDK
	{
		get
		{
			return GPEEDKOHFIG();
		}
	}

	internal int LEELHFGHGFL
	{
		get
		{
			return ICKFNGAOMFI();
		}
	}

	internal IFLBJIKPLOL OKNBCJILDGP
	{
		get
		{
			return GBDAFIEGGPA();
		}
		private set
		{
			MDMJNFNIHGE(value);
		}
	}

	public SocketManager(Uri KJHNCLAJMLO)
		: this(KJHNCLAJMLO, new SocketOptions())
	{
	}

	public SocketManager(Uri KJHNCLAJMLO, SocketOptions LHONCAIFCAF)
	{
		set_Uri(KJHNCLAJMLO);
		KAFLNEBPLIA(LHONCAIFCAF);
		set_State(IFLBJIKPLOL.Initial);
		MDMJNFNIHGE(IFLBJIKPLOL.Initial);
		MLMBDFBLPAC(MLILENMLJAH);
	}

	public IFLBJIKPLOL FLBBFDNHJAJ()
	{
		return state;
	}

	private void set_State(IFLBJIKPLOL value)
	{
		MDMJNFNIHGE(state);
		state = value;
	}

	public SocketOptions HLHJJJGJEEL()
	{
		return GLJHJKNBNDD;
	}

	private void KAFLNEBPLIA(SocketOptions value)
	{
		GLJHJKNBNDD = value;
	}

	public Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	private void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	public HandshakeData EIOHJJFBIAL()
	{
		return GCKLPLNOHCH;
	}

	private void NLCGDODNKEF(HandshakeData value)
	{
		GCKLPLNOHCH = value;
	}

	public ITransport LODFOKFEAPC()
	{
		return DCDGJNEKNKE;
	}

	private void AOJLKJODKMC(ITransport value)
	{
		DCDGJNEKNKE = value;
	}

	public ulong EKBGNBPGFNG()
	{
		return GDDMABPJDPP;
	}

	internal void set_RequestCounter(ulong value)
	{
		GDDMABPJDPP = value;
	}

	public Socket PDJFKOBODHH()
	{
		return ELGJFOCAJPE();
	}

	public Socket get_Item(string JBALIKEKHGL)
	{
		return ELGJFOCAJPE(JBALIKEKHGL);
	}

	public int IKFMEIGJFCA()
	{
		return CPDNJPDONOL;
	}

	private void set_ReconnectAttempts(int value)
	{
		CPDNJPDONOL = value;
	}

	public OOINGNLNJGM KCMCCGKJGLE()
	{
		return NBCJNBIGHCJ;
	}

	public void MLMBDFBLPAC(OOINGNLNJGM value)
	{
		NBCJNBIGHCJ = value;
	}

	internal uint GPEEDKOHFIG()
	{
		return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
	}

	internal int ICKFNGAOMFI()
	{
		return Interlocked.Increment(ref MFMMDNPNOCE);
	}

	internal IFLBJIKPLOL GBDAFIEGGPA()
	{
		return KKPFPDLOEFJ;
	}

	private void MDMJNFNIHGE(IFLBJIKPLOL value)
	{
		KKPFPDLOEFJ = value;
	}

	public Socket ELGJFOCAJPE()
	{
		return ELGJFOCAJPE("/");
	}

	public Socket ELGJFOCAJPE(string JBALIKEKHGL)
	{
		if (string.IsNullOrEmpty(JBALIKEKHGL))
		{
			throw new ArgumentNullException("Namespace parameter is null or empty!");
		}
		Socket value = null;
		if (!OABAACFMPFH.TryGetValue(JBALIKEKHGL, out value))
		{
			value = new Socket(JBALIKEKHGL, this);
			OABAACFMPFH.Add(JBALIKEKHGL, value);
			FADLKIHOHLN.Add(value);
			((ISocket)value).Open();
		}
		return value;
	}

	void IManager.Remove(Socket JLEACANCMJF)
	{
		OABAACFMPFH.Remove(JLEACANCMJF.IONIEDIPEGB());
		FADLKIHOHLN.Remove(JLEACANCMJF);
	}

	public void Open()
	{
		if (FLBBFDNHJAJ() == IFLBJIKPLOL.Initial || FLBBFDNHJAJ() == IFLBJIKPLOL.Closed || FLBBFDNHJAJ() == IFLBJIKPLOL.Reconnecting)
		{
			HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SocketManager", "Opening");
			GDEMFJODPBO = DateTime.MinValue;
			NLCGDODNKEF(new HandshakeData(this));
			EIOHJJFBIAL().OnReceived = (HandshakeData LLAAFNOMDHA) =>
			{
				JDJOMBGCABG();
			};
			EIOHJJFBIAL().OnError = (HandshakeData LLAAFNOMDHA, string KEPBNIIECPN) =>
			{
				((IManager)this).EmitError(CCCOMMIFIMB.Internal, KEPBNIIECPN);
				((IManager)this).TryToReconnect();
			};
			EIOHJJFBIAL().Start();
			((IManager)this).EmitEvent("connecting", new object[0]);
			set_State(IFLBJIKPLOL.Opening);
			BAHEGCMFMJI = DateTime.UtcNow;
			HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
			ELGJFOCAJPE("/");
		}
	}

	public void Close()
	{
		((IManager)this).Close(true);
	}

	void IManager.Close(bool IEPJILJMNDN)
	{
		if (FLBBFDNHJAJ() == IFLBJIKPLOL.Closed)
		{
			return;
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SocketManager", "Closing");
		HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		if (IEPJILJMNDN)
		{
			while (FADLKIHOHLN.Count > 0)
			{
				((ISocket)FADLKIHOHLN[FADLKIHOHLN.Count - 1]).Disconnect(IEPJILJMNDN);
			}
		}
		else
		{
			for (int i = 0; i < FADLKIHOHLN.Count; i++)
			{
				((ISocket)FADLKIHOHLN[i]).Disconnect(IEPJILJMNDN);
			}
		}
		set_State(IFLBJIKPLOL.Closed);
		GHAGBCLOOFC = DateTime.MinValue;
		if (LCAAGNNLNKO != null)
		{
			LCAAGNNLNKO.Clear();
		}
		if (IEPJILJMNDN)
		{
			OABAACFMPFH.Clear();
		}
		if (EIOHJJFBIAL() != null)
		{
			EIOHJJFBIAL().AKLEEMEHBIC();
		}
		NLCGDODNKEF(null);
		if (LODFOKFEAPC() != null)
		{
			LODFOKFEAPC().Close();
		}
		AOJLKJODKMC(null);
	}

	void IManager.TryToReconnect()
	{
		if (FLBBFDNHJAJ() == IFLBJIKPLOL.Reconnecting || FLBBFDNHJAJ() == IFLBJIKPLOL.Closed)
		{
			return;
		}
		if (!HLHJJJGJEEL().AMHAEEBHDFE())
		{
			Close();
			return;
		}
		int num;
		set_ReconnectAttempts(num = IKFMEIGJFCA() + 1);
		if (num >= HLHJJJGJEEL().GIBLAAJPHLP())
		{
			((IManager)this).EmitEvent("reconnect_failed", new object[0]);
			Close();
			return;
		}
		Random random = new Random();
		int num2 = (int)HLHJJJGJEEL().CHAGLLGOKKE().TotalMilliseconds * IKFMEIGJFCA();
		GDEMFJODPBO = DateTime.UtcNow + TimeSpan.FromMilliseconds(Math.Min(random.Next((int)((float)num2 - (float)num2 * HLHJJJGJEEL().JNJBBOELNIG()), (int)((float)num2 + (float)num2 * HLHJJJGJEEL().JNJBBOELNIG())), (int)HLHJJJGJEEL().ODAHMCJKEIL().TotalMilliseconds));
		((IManager)this).Close(false);
		set_State(IFLBJIKPLOL.Reconnecting);
		for (int i = 0; i < FADLKIHOHLN.Count; i++)
		{
			((ISocket)FADLKIHOHLN[i]).Open();
		}
		HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SocketManager", "Reconnecting");
	}

	private void JDJOMBGCABG()
	{
		if (EIOHJJFBIAL().BLCLIKIBIPE().Contains("websocket"))
		{
			AOJLKJODKMC(new KMLMEACFJGA(this));
		}
		else
		{
			AOJLKJODKMC(new HLNFFAMCLEH(this));
		}
		LODFOKFEAPC().LAJCMNNNIIM();
	}

	bool IManager.OnTransportConnected(ITransport DLAOOGHJGBI)
	{
		if (FLBBFDNHJAJ() != IFLBJIKPLOL.Opening)
		{
			return false;
		}
		if (GBDAFIEGGPA() == IFLBJIKPLOL.Reconnecting)
		{
			((IManager)this).EmitEvent("reconnect", new object[0]);
		}
		set_State(IFLBJIKPLOL.Open);
		ECDDELFMACL = DateTime.UtcNow;
		set_ReconnectAttempts(0);
		GNNBAGPHJKA();
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SocketManager", "Open");
		return true;
	}

	void IManager.OnTransportError(ITransport DLAOOGHJGBI, string KEPBNIIECPN)
	{
		((IManager)this).EmitError(CCCOMMIFIMB.Internal, KEPBNIIECPN);
		if (DLAOOGHJGBI.FLBBFDNHJAJ() == FGMEPPMFFKG.Connecting || DLAOOGHJGBI.FLBBFDNHJAJ() == FGMEPPMFFKG.Opening)
		{
			if (DLAOOGHJGBI is KMLMEACFJGA)
			{
				DLAOOGHJGBI.Close();
				AOJLKJODKMC(new HLNFFAMCLEH(this));
				LODFOKFEAPC().LAJCMNNNIIM();
			}
			else
			{
				((IManager)this).TryToReconnect();
			}
		}
		else
		{
			DLAOOGHJGBI.Close();
			((IManager)this).TryToReconnect();
		}
	}

	private ITransport DHHBFHPFGFJ()
	{
		if (FLBBFDNHJAJ() != IFLBJIKPLOL.Open)
		{
			return null;
		}
		return (!LODFOKFEAPC().LILBDKKEHCE()) ? LODFOKFEAPC() : null;
	}

	private void GNNBAGPHJKA()
	{
		ITransport bNPCOHLEHNM = DHHBFHPFGFJ();
		if (LCAAGNNLNKO != null && LCAAGNNLNKO.Count > 0 && bNPCOHLEHNM != null)
		{
			bNPCOHLEHNM.Send(LCAAGNNLNKO);
			LCAAGNNLNKO.Clear();
		}
	}

	void IManager.SendPacket(Packet NPKADBPBKIG)
	{
		ITransport bNPCOHLEHNM = DHHBFHPFGFJ();
		if (bNPCOHLEHNM != null)
		{
			try
			{
				bNPCOHLEHNM.Send(NPKADBPBKIG);
				return;
			}
			catch (Exception ex)
			{
				((IManager)this).EmitError(CCCOMMIFIMB.Internal, ex.Message + " " + ex.StackTrace);
				return;
			}
		}
		if (LCAAGNNLNKO == null)
		{
			LCAAGNNLNKO = new List<Packet>();
		}
		LCAAGNNLNKO.Add(NPKADBPBKIG.Clone());
	}

	void IManager.OnPacket(Packet NPKADBPBKIG)
	{
		if (FLBBFDNHJAJ() != IFLBJIKPLOL.Closed)
		{
			switch (NPKADBPBKIG.FFJBNPEOAHI())
			{
			case HJDLGPHLPNF.Ping:
				((IManager)this).SendPacket(new Packet(HJDLGPHLPNF.Pong, ECDAJBEFCAH.Unknown, "/", string.Empty));
				break;
			case HJDLGPHLPNF.Pong:
				ECDDELFMACL = DateTime.UtcNow;
				break;
			}
			Socket value = null;
			if (OABAACFMPFH.TryGetValue(NPKADBPBKIG.IONIEDIPEGB(), out value))
			{
				((ISocket)value).OnPacket(NPKADBPBKIG);
			}
			else
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SocketManager", "Namespace \"" + NPKADBPBKIG.IONIEDIPEGB() + "\" not found!");
			}
		}
	}

	public void EmitAll(string DOPHKKGNAEF, params object[] LKIOKGCNKHE)
	{
		for (int i = 0; i < FADLKIHOHLN.Count; i++)
		{
			FADLKIHOHLN[i].Emit(DOPHKKGNAEF, LKIOKGCNKHE);
		}
	}

	void IManager.EmitEvent(string DOPHKKGNAEF, params object[] LKIOKGCNKHE)
	{
		Socket value = null;
		if (OABAACFMPFH.TryGetValue("/", out value))
		{
			((ISocket)value).EmitEvent(DOPHKKGNAEF, LKIOKGCNKHE);
		}
	}

	void IManager.EmitEvent(ECDAJBEFCAH LFLGCDNKNJI, params object[] LKIOKGCNKHE)
	{
		((IManager)this).EmitEvent(EventNames.ICAIODPBKBO(LFLGCDNKNJI), LKIOKGCNKHE);
	}

	void IManager.EmitError(CCCOMMIFIMB GNKCGOGKAEK, string CKEHOEGLMBM)
	{
		((IManager)this).EmitEvent(ECDAJBEFCAH.Error, new object[1]
		{
			new Error(GNKCGOGKAEK, CKEHOEGLMBM)
		});
	}

	void IManager.EmitAll(string DOPHKKGNAEF, params object[] LKIOKGCNKHE)
	{
		for (int i = 0; i < FADLKIHOHLN.Count; i++)
		{
			((ISocket)FADLKIHOHLN[i]).EmitEvent(DOPHKKGNAEF, LKIOKGCNKHE);
		}
	}

	void IHeartbeat.OnHeartbeatUpdate(TimeSpan OJOKANCMPLG)
	{
		switch (FLBBFDNHJAJ())
		{
		case IFLBJIKPLOL.Opening:
			if (DateTime.UtcNow - BAHEGCMFMJI >= HLHJJJGJEEL().FJKGKLJGIJI())
			{
				((IManager)this).EmitEvent("connect_error", new object[0]);
				((IManager)this).EmitEvent("connect_timeout", new object[0]);
				((IManager)this).TryToReconnect();
			}
			break;
		case IFLBJIKPLOL.Reconnecting:
			if (GDEMFJODPBO != DateTime.MinValue && DateTime.UtcNow >= GDEMFJODPBO)
			{
				((IManager)this).EmitEvent("reconnect_attempt", new object[0]);
				((IManager)this).EmitEvent("reconnecting", new object[0]);
				Open();
			}
			break;
		case IFLBJIKPLOL.Open:
		{
			ITransport bNPCOHLEHNM = null;
			if (LODFOKFEAPC() != null && LODFOKFEAPC().FLBBFDNHJAJ() == FGMEPPMFFKG.Open)
			{
				bNPCOHLEHNM = LODFOKFEAPC();
			}
			if (bNPCOHLEHNM == null || bNPCOHLEHNM.FLBBFDNHJAJ() != FGMEPPMFFKG.Open)
			{
				break;
			}
			bNPCOHLEHNM.GNGIDEJLNCF();
			GNNBAGPHJKA();
			if (GHAGBCLOOFC == DateTime.MinValue)
			{
				GHAGBCLOOFC = DateTime.UtcNow;
				break;
			}
			if (DateTime.UtcNow - GHAGBCLOOFC > EIOHJJFBIAL().CMFJFNKMJIP())
			{
				((IManager)this).SendPacket(new Packet(HJDLGPHLPNF.Ping, ECDAJBEFCAH.Unknown, "/", string.Empty));
				GHAGBCLOOFC = DateTime.UtcNow;
			}
			if (DateTime.UtcNow - ECDDELFMACL > EIOHJJFBIAL().EPFLHIKEBFO())
			{
				((IManager)this).TryToReconnect();
			}
			break;
		}
		}
	}
}
