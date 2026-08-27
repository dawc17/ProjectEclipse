using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

[DefaultMember("Item")]
public sealed class Connection : IHeartbeat, IConnection
{
	public static AJAIAKCIJIJ MLILENMLJAH = new NLHENJCBLHC();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	private OHLFKFFAOMF MAFFNGPOMJD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private NegotiationData FPOFMJPGPPK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Hub[] OPNKMIKOIPJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TransportBase DCDGJNEKNKE;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, string> LGJADADCGHL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool IJJGBHCLDHH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AJAIAKCIJIJ HDCJIOPHKHC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IAuthenticationProvider NFEAHINLEPH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private PILIPIHGBEG OnConnected;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private KMBJIOLJJCE onClosedField;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DHGLHLDFDAC onErrorField;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private PILIPIHGBEG OnReconnecting;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private PILIPIHGBEG OnReconnected;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private AIBCPDGLFPB OnStateChanged;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private OnNonHubMessageDelegate OnNonHubMessage;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private GAHNKDKDAGM ELGFIKKAIGK;

	internal object SyncRoot = new object();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ulong IHPMBFLOAPN;

	private readonly string NJNGCKKIKMG = "1.5";

	private ulong PHFIIKGKDCF;

	private MultiMessage HGJBHOLJFED;

	private string LPEDAAHHCBD;

	private List<IServerMessage> KIECCNGHGOG;

	private DateTime LAPLGEGKFGI;

	private DateTime? JNPJOFDOAAG;

	private DateTime OIHEGPAPFIO;

	private TimeSpan PingInterval;

	private HTTPRequest ACHGLFBEJCK;

	private DateTime? JEAPOHAGCLL;

	private StringBuilder queryBuilder = new StringBuilder();

	private string OBPNFNBIANJ;

	private string BuiltQueryParams;

	private OBBKIBFJEMI PJJBHIFNPGF;

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

	public OHLFKFFAOMF AFINHOBCHMC
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

	public NegotiationData IIPFKKBEANI
	{
		get
		{
			return EOBPEOEMEDB();
		}
		private set
		{
			DNPEFPIPCKJ(value);
		}
	}

	public Hub[] LLOKMJFOCED
	{
		get
		{
			return LINDGKFKGND();
		}
		private set
		{
			ABCJIPNMCFF(value);
		}
	}

	public TransportBase HEFNODJLIBE
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

	public Dictionary<string, string> CNGINADLODB
	{
		get
		{
			return MONGJAOIELO();
		}
		set
		{
			set_AdditionalQueryParams(value);
		}
	}

	public bool KOCIJKDENMF
	{
		get
		{
			return DKJAFHAOKDB();
		}
		set
		{
			set_QueryParamsOnlyForHandshake(value);
		}
	}

	public AJAIAKCIJIJ CBGKGGCMHLL
	{
		get
		{
			return IBNMFHGHIBI();
		}
		set
		{
			LPEPILDNMNE(value);
		}
	}

	public IAuthenticationProvider IHFLGNOCFDL
	{
		get
		{
			return DLKDCNNCKCL();
		}
		set
		{
			FBFLBJGPEGA(value);
		}
	}

	public GAHNKDKDAGM BEHBLACKLGN
	{
		get
		{
			return MAKFGPNOKNL();
		}
		set
		{
			EHLHIKGDAJE(value);
		}
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public Hub get_DLKPBAJDHBO(int OOPOEMNCCGH)
	{
		return get_Item(OOPOEMNCCGH);
	}

	// C# has no syntax for parameterized property 'DLKPBAJDHBO'.
	public Hub get_DLKPBAJDHBO(string KKNJICFENMD)
	{
		return get_Item(KKNJICFENMD);
	}

	internal ulong NNJHGACOHCF
	{
		get
		{
			return FOIDELLGGOL();
		}
		set
		{
			set_ClientMessageCounter(value);
		}
	}

	private uint KBEDPONBFDK
	{
		get
		{
			return GPEEDKOHFIG();
		}
	}

	private string NMLDBCMAEMO
	{
		get
		{
			return CPEMFGDHOCB();
		}
	}

	private string IEJEGAEGCGM
	{
		get
		{
			return FGDGEPEPCJL();
		}
	}

	public event PILIPIHGBEG PIGDCLOPNKJ
	{
		add
		{
			FJBEHFPIAHI(value);
		}
		remove
		{
			LCIOENIELOA(value);
		}
	}

	public event KMBJIOLJJCE OnClosed
	{
		add
		{
			IDCIMGLDBJG(value);
		}
		remove
		{
			OIBOHOKKFKE(value);
		}
	}

	public event DHGLHLDFDAC OnError
	{
		add
		{
			BJDMHEHILEO(value);
		}
		remove
		{
			LEIDAIFMPCE(value);
		}
	}

	public event PILIPIHGBEG ALOMMPHLNPF
	{
		add
		{
			GPEFJGKDKEC(value);
		}
		remove
		{
			LBEEKKOLILM(value);
		}
	}

	public event PILIPIHGBEG DLKLCKJDEGC
	{
		add
		{
			IKLLDGGCHKE(value);
		}
		remove
		{
			MEMBFLPEJJJ(value);
		}
	}

	public event AIBCPDGLFPB KPGNPBCPCJK
	{
		add
		{
			FADMHEJNPJO(value);
		}
		remove
		{
			NEFHCNPDIHG(value);
		}
	}

	public event OnNonHubMessageDelegate MFJCDIIEPKI
	{
		add
		{
			EHOAGKMPCJH(value);
		}
		remove
		{
			JFMFJFDMALO(value);
		}
	}

	public Connection(Uri KJHNCLAJMLO, params string[] KMDACKIILBI)
		: this(KJHNCLAJMLO)
	{
		if (KMDACKIILBI != null && KMDACKIILBI.Length > 0)
		{
			ABCJIPNMCFF(new Hub[KMDACKIILBI.Length]);
			for (int i = 0; i < KMDACKIILBI.Length; i++)
			{
				LINDGKFKGND()[i] = new Hub(KMDACKIILBI[i], this);
			}
		}
	}

	public Connection(Uri KJHNCLAJMLO, params Hub[] EOAEFLODECF)
		: this(KJHNCLAJMLO)
	{
		ABCJIPNMCFF(EOAEFLODECF);
		if (EOAEFLODECF != null)
		{
			for (int i = 0; i < EOAEFLODECF.Length; i++)
			{
				((IHub)EOAEFLODECF[i]).GNLCPJFBAJE(this);
			}
		}
	}

	public Connection(Uri KJHNCLAJMLO)
	{
		set_State(OHLFKFFAOMF.Initial);
		set_Uri(KJHNCLAJMLO);
		LPEPILDNMNE(MLILENMLJAH);
		PingInterval = TimeSpan.FromMinutes(5.0);
	}

	public Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	private void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	public OHLFKFFAOMF FLBBFDNHJAJ()
	{
		return MAFFNGPOMJD;
	}

	private void set_State(OHLFKFFAOMF value)
	{
		OHLFKFFAOMF mAFFNGPOMJD = MAFFNGPOMJD;
		MAFFNGPOMJD = value;
		if (OnStateChanged != null)
		{
			OnStateChanged(this, mAFFNGPOMJD, MAFFNGPOMJD);
		}
	}

	public NegotiationData EOBPEOEMEDB()
	{
		return FPOFMJPGPPK;
	}

	private void DNPEFPIPCKJ(NegotiationData value)
	{
		FPOFMJPGPPK = value;
	}

	public Hub[] LINDGKFKGND()
	{
		return OPNKMIKOIPJ;
	}

	private void ABCJIPNMCFF(Hub[] value)
	{
		OPNKMIKOIPJ = value;
	}

	public TransportBase LODFOKFEAPC()
	{
		return DCDGJNEKNKE;
	}

	private void AOJLKJODKMC(TransportBase value)
	{
		DCDGJNEKNKE = value;
	}

	public Dictionary<string, string> MONGJAOIELO()
	{
		return LGJADADCGHL;
	}

	public void set_AdditionalQueryParams(Dictionary<string, string> value)
	{
		LGJADADCGHL = value;
	}

	public bool DKJAFHAOKDB()
	{
		return IJJGBHCLDHH;
	}

	public void set_QueryParamsOnlyForHandshake(bool value)
	{
		IJJGBHCLDHH = value;
	}

	public AJAIAKCIJIJ IBNMFHGHIBI()
	{
		return HDCJIOPHKHC;
	}

	public void LPEPILDNMNE(AJAIAKCIJIJ value)
	{
		HDCJIOPHKHC = value;
	}

	public IAuthenticationProvider DLKDCNNCKCL()
	{
		return NFEAHINLEPH;
	}

	public void FBFLBJGPEGA(IAuthenticationProvider value)
	{
		NFEAHINLEPH = value;
	}

	public void FJBEHFPIAHI(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnConnected;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnConnected, (PILIPIHGBEG)Delegate.Combine(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void LCIOENIELOA(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnConnected;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnConnected, (PILIPIHGBEG)Delegate.Remove(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void IDCIMGLDBJG(KMBJIOLJJCE value)
	{
		KMBJIOLJJCE kMBJIOLJJCE = onClosedField;
		KMBJIOLJJCE kMBJIOLJJCE2;
		do
		{
			kMBJIOLJJCE2 = kMBJIOLJJCE;
			kMBJIOLJJCE = Interlocked.CompareExchange(ref onClosedField, (KMBJIOLJJCE)Delegate.Combine(kMBJIOLJJCE2, value), kMBJIOLJJCE);
		}
		while ((object)kMBJIOLJJCE != kMBJIOLJJCE2);
	}

	public void OIBOHOKKFKE(KMBJIOLJJCE value)
	{
		KMBJIOLJJCE kMBJIOLJJCE = onClosedField;
		KMBJIOLJJCE kMBJIOLJJCE2;
		do
		{
			kMBJIOLJJCE2 = kMBJIOLJJCE;
			kMBJIOLJJCE = Interlocked.CompareExchange(ref onClosedField, (KMBJIOLJJCE)Delegate.Remove(kMBJIOLJJCE2, value), kMBJIOLJJCE);
		}
		while ((object)kMBJIOLJJCE != kMBJIOLJJCE2);
	}

	public void BJDMHEHILEO(DHGLHLDFDAC value)
	{
		DHGLHLDFDAC dHGLHLDFDAC = onErrorField;
		DHGLHLDFDAC dHGLHLDFDAC2;
		do
		{
			dHGLHLDFDAC2 = dHGLHLDFDAC;
			dHGLHLDFDAC = Interlocked.CompareExchange(ref onErrorField, (DHGLHLDFDAC)Delegate.Combine(dHGLHLDFDAC2, value), dHGLHLDFDAC);
		}
		while ((object)dHGLHLDFDAC != dHGLHLDFDAC2);
	}

	public void LEIDAIFMPCE(DHGLHLDFDAC value)
	{
		DHGLHLDFDAC dHGLHLDFDAC = onErrorField;
		DHGLHLDFDAC dHGLHLDFDAC2;
		do
		{
			dHGLHLDFDAC2 = dHGLHLDFDAC;
			dHGLHLDFDAC = Interlocked.CompareExchange(ref onErrorField, (DHGLHLDFDAC)Delegate.Remove(dHGLHLDFDAC2, value), dHGLHLDFDAC);
		}
		while ((object)dHGLHLDFDAC != dHGLHLDFDAC2);
	}

	public void GPEFJGKDKEC(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnReconnecting;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnReconnecting, (PILIPIHGBEG)Delegate.Combine(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void LBEEKKOLILM(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnReconnecting;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnReconnecting, (PILIPIHGBEG)Delegate.Remove(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void IKLLDGGCHKE(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnReconnected;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnReconnected, (PILIPIHGBEG)Delegate.Combine(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void MEMBFLPEJJJ(PILIPIHGBEG value)
	{
		PILIPIHGBEG pILIPIHGBEG = OnReconnected;
		PILIPIHGBEG pILIPIHGBEG2;
		do
		{
			pILIPIHGBEG2 = pILIPIHGBEG;
			pILIPIHGBEG = Interlocked.CompareExchange(ref OnReconnected, (PILIPIHGBEG)Delegate.Remove(pILIPIHGBEG2, value), pILIPIHGBEG);
		}
		while ((object)pILIPIHGBEG != pILIPIHGBEG2);
	}

	public void FADMHEJNPJO(AIBCPDGLFPB value)
	{
		AIBCPDGLFPB aIBCPDGLFPB = OnStateChanged;
		AIBCPDGLFPB aIBCPDGLFPB2;
		do
		{
			aIBCPDGLFPB2 = aIBCPDGLFPB;
			aIBCPDGLFPB = Interlocked.CompareExchange(ref OnStateChanged, (AIBCPDGLFPB)Delegate.Combine(aIBCPDGLFPB2, value), aIBCPDGLFPB);
		}
		while ((object)aIBCPDGLFPB != aIBCPDGLFPB2);
	}

	public void NEFHCNPDIHG(AIBCPDGLFPB value)
	{
		AIBCPDGLFPB aIBCPDGLFPB = OnStateChanged;
		AIBCPDGLFPB aIBCPDGLFPB2;
		do
		{
			aIBCPDGLFPB2 = aIBCPDGLFPB;
			aIBCPDGLFPB = Interlocked.CompareExchange(ref OnStateChanged, (AIBCPDGLFPB)Delegate.Remove(aIBCPDGLFPB2, value), aIBCPDGLFPB);
		}
		while ((object)aIBCPDGLFPB != aIBCPDGLFPB2);
	}

	public void EHOAGKMPCJH(OnNonHubMessageDelegate value)
	{
		OnNonHubMessageDelegate gAGJEANDJEK = OnNonHubMessage;
		OnNonHubMessageDelegate gAGJEANDJEK2;
		do
		{
			gAGJEANDJEK2 = gAGJEANDJEK;
			gAGJEANDJEK = Interlocked.CompareExchange(ref OnNonHubMessage, (OnNonHubMessageDelegate)Delegate.Combine(gAGJEANDJEK2, value), gAGJEANDJEK);
		}
		while ((object)gAGJEANDJEK != gAGJEANDJEK2);
	}

	public void JFMFJFDMALO(OnNonHubMessageDelegate value)
	{
		OnNonHubMessageDelegate gAGJEANDJEK = OnNonHubMessage;
		OnNonHubMessageDelegate gAGJEANDJEK2;
		do
		{
			gAGJEANDJEK2 = gAGJEANDJEK;
			gAGJEANDJEK = Interlocked.CompareExchange(ref OnNonHubMessage, (OnNonHubMessageDelegate)Delegate.Remove(gAGJEANDJEK2, value), gAGJEANDJEK);
		}
		while ((object)gAGJEANDJEK != gAGJEANDJEK2);
	}

	public GAHNKDKDAGM MAKFGPNOKNL()
	{
		return ELGFIKKAIGK;
	}

	public void EHLHIKGDAJE(GAHNKDKDAGM value)
	{
		ELGFIKKAIGK = value;
	}

	public Hub get_Item(int OOPOEMNCCGH)
	{
		return LINDGKFKGND()[OOPOEMNCCGH];
	}

	public Hub get_Item(string KKNJICFENMD)
	{
		for (int i = 0; i < LINDGKFKGND().Length; i++)
		{
			Hub hGCBNOGDDPB = LINDGKFKGND()[i];
			if (hGCBNOGDDPB.get_Name().Equals(KKNJICFENMD, StringComparison.OrdinalIgnoreCase))
			{
				return hGCBNOGDDPB;
			}
		}
		return null;
	}

	internal ulong FOIDELLGGOL()
	{
		return IHPMBFLOAPN;
	}

	internal void set_ClientMessageCounter(ulong value)
	{
		IHPMBFLOAPN = value;
	}

	private uint GPEEDKOHFIG()
	{
		return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks;
	}

	private string CPEMFGDHOCB()
	{
		if (!string.IsNullOrEmpty(OBPNFNBIANJ))
		{
			return OBPNFNBIANJ;
		}
		StringBuilder stringBuilder = new StringBuilder("[", LINDGKFKGND().Length * 4);
		if (LINDGKFKGND() != null)
		{
			for (int i = 0; i < LINDGKFKGND().Length; i++)
			{
				stringBuilder.Append("{\"Name\":\"");
				stringBuilder.Append(LINDGKFKGND()[i].get_Name());
				stringBuilder.Append("\"}");
				if (i < LINDGKFKGND().Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
		}
		stringBuilder.Append("]");
		return OBPNFNBIANJ = Uri.EscapeUriString(stringBuilder.ToString());
	}

	private string FGDGEPEPCJL()
	{
		if (MONGJAOIELO() == null || MONGJAOIELO().Count == 0)
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(BuiltQueryParams))
		{
			return BuiltQueryParams;
		}
		StringBuilder stringBuilder = new StringBuilder(MONGJAOIELO().Count * 4);
		foreach (KeyValuePair<string, string> item in MONGJAOIELO())
		{
			stringBuilder.Append("&");
			stringBuilder.Append(item.Key);
			if (!string.IsNullOrEmpty(item.Value))
			{
				stringBuilder.Append("=");
				stringBuilder.Append(Uri.EscapeDataString(item.Value));
			}
		}
		return BuiltQueryParams = stringBuilder.ToString();
	}

	public void LAJCMNNNIIM()
	{
		if (FLBBFDNHJAJ() == OHLFKFFAOMF.Initial || FLBBFDNHJAJ() == OHLFKFFAOMF.Closed)
		{
			if (DLKDCNNCKCL() != null && DLKDCNNCKCL().MCHOHLKGMBI())
			{
				set_State(OHLFKFFAOMF.Authenticating);
				DLKDCNNCKCL().IJPBAJDFAED(EFCGDJPAJIG);
				DLKDCNNCKCL().NEAGLBOCLHI(HMIFKIFAFMK);
				DLKDCNNCKCL().MKODIGEMHFN();
			}
			else
			{
				LCHALDLMLML();
			}
		}
	}

	private void EFCGDJPAJIG(IAuthenticationProvider EEGMFLOPLLH)
	{
		EEGMFLOPLLH.KFGAHIPDDOF(EFCGDJPAJIG);
		LCHALDLMLML();
	}

	private void HMIFKIFAFMK(IAuthenticationProvider EEGMFLOPLLH, string NEPOLDCKNJL)
	{
		EEGMFLOPLLH.BFANLHDOICD(HMIFKIFAFMK);
		((IConnection)this).Error(NEPOLDCKNJL);
	}

	private void LCHALDLMLML()
	{
		set_State(OHLFKFFAOMF.Negotiating);
		DNPEFPIPCKJ(new NegotiationData(this));
		EOBPEOEMEDB().OnReceived = NADFEIHHBJL;
		EOBPEOEMEDB().OnError = PBFHCEMPCEN;
		EOBPEOEMEDB().Start();
	}

	private void NADFEIHHBJL(NegotiationData data)
	{
		if (data.AOKNIGBFMKG())
		{
			AOJLKJODKMC(new JHHBEDGPFDM(this));
			PJJBHIFNPGF = OBBKIBFJEMI.ServerSentEvents;
		}
		else
		{
			AOJLKJODKMC(new ServerSentEventsTransport(this));
			PJJBHIFNPGF = OBBKIBFJEMI.HTTP;
		}
		set_State(OHLFKFFAOMF.Connecting);
		JEAPOHAGCLL = DateTime.UtcNow;
		LODFOKFEAPC().NDCILHIAPIK();
	}

	private void PBFHCEMPCEN(NegotiationData data, string JDONBAPIJCG)
	{
		((IConnection)this).Error(JDONBAPIJCG);
	}

	public void Close()
	{
		if (FLBBFDNHJAJ() == OHLFKFFAOMF.Closed)
		{
			return;
		}
		set_State(OHLFKFFAOMF.Closed);
		JNPJOFDOAAG = null;
		JEAPOHAGCLL = null;
		if (LODFOKFEAPC() != null)
		{
			LODFOKFEAPC().AKLEEMEHBIC();
			AOJLKJODKMC(null);
		}
		DNPEFPIPCKJ(null);
		HTTPManager.MAMNLAJACOD().HKMBDKKHPCB(this);
		HGJBHOLJFED = null;
		if (LINDGKFKGND() != null)
		{
			for (int i = 0; i < LINDGKFKGND().Length; i++)
			{
				((IHub)LINDGKFKGND()[i]).Close();
			}
		}
		if (KIECCNGHGOG != null)
		{
			KIECCNGHGOG.Clear();
			KIECCNGHGOG = null;
		}
		if (onClosedField == null)
		{
			return;
		}
		try
		{
			onClosedField(this);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("SignalR Connection", "OnClosed", mPFFFAOGBJE);
		}
	}

	public void IGFIEFDGBDJ()
	{
		DateTime? jNPJOFDOAAG = JNPJOFDOAAG;
		if (jNPJOFDOAAG.HasValue)
		{
			return;
		}
		HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", "Reconnecting");
		set_State(OHLFKFFAOMF.Reconnecting);
		JNPJOFDOAAG = DateTime.UtcNow;
		LODFOKFEAPC().IGFIEFDGBDJ();
		if (ACHGLFBEJCK != null)
		{
			ACHGLFBEJCK.AKLEEMEHBIC();
		}
		if (OnReconnecting == null)
		{
			return;
		}
		try
		{
			OnReconnecting(this);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("SignalR Connection", "OnReconnecting", mPFFFAOGBJE);
		}
	}

	public void Send(object EHCLMBADLKH)
	{
		if (EHCLMBADLKH == null)
		{
			throw new ArgumentNullException("arg");
		}
		lock (SyncRoot)
		{
			if (FLBBFDNHJAJ() == OHLFKFFAOMF.Connected)
			{
				string dGNLDMDLKDA = IBNMFHGHIBI().Encode(EHCLMBADLKH);
				LODFOKFEAPC().Send(dGNLDMDLKDA);
			}
		}
	}

	public void CJDGGCJDHIE(string EMDHMHOKGFP)
	{
		if (EMDHMHOKGFP == null)
		{
			throw new ArgumentNullException("json");
		}
		lock (SyncRoot)
		{
			if (FLBBFDNHJAJ() == OHLFKFFAOMF.Connected)
			{
				LODFOKFEAPC().Send(EMDHMHOKGFP);
			}
		}
	}

	void IConnection.OnMessage(IServerMessage CKEHOEGLMBM)
	{
		if (FLBBFDNHJAJ() == OHLFKFFAOMF.Closed)
		{
			return;
		}
		if (FLBBFDNHJAJ() == OHLFKFFAOMF.Connecting)
		{
			if (KIECCNGHGOG == null)
			{
				KIECCNGHGOG = new List<IServerMessage>();
			}
			KIECCNGHGOG.Add(CKEHOEGLMBM);
			return;
		}
		LAPLGEGKFGI = DateTime.UtcNow;
		switch (CKEHOEGLMBM.get_Type())
		{
		case LENCKBHFKLD.Multiple:
			HGJBHOLJFED = CKEHOEGLMBM as MultiMessage;
			if (HGJBHOLJFED.BPDICBIDIPO())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SignalR Connection", "OnMessage - Init");
			}
			if (HGJBHOLJFED.PCINJIIKLFH() != null)
			{
				LPEDAAHHCBD = HGJBHOLJFED.PCINJIIKLFH();
			}
			if (HGJBHOLJFED.ACMAJHFOIDJ())
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SignalR Connection", "OnMessage - Should Reconnect");
				IGFIEFDGBDJ();
			}
			if (HGJBHOLJFED.CHIGLEKCFFN() != null)
			{
				for (int i = 0; i < HGJBHOLJFED.CHIGLEKCFFN().Count; i++)
				{
					((IConnection)this).OnMessage(HGJBHOLJFED.CHIGLEKCFFN()[i]);
				}
			}
			break;
		case LENCKBHFKLD.MethodCall:
		{
			MethodCallMessage iFKLAELFLJL = CKEHOEGLMBM as MethodCallMessage;
			Hub hGCBNOGDDPB = get_Item(iFKLAELFLJL.GDANEAJOFMP());
			if (hGCBNOGDDPB != null)
			{
				((IHub)hGCBNOGDDPB).OnMethod(iFKLAELFLJL);
			}
			else
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", string.Format("Hub \"{0}\" not found!", iFKLAELFLJL.GDANEAJOFMP()));
			}
			break;
		}
		case LENCKBHFKLD.Result:
		case LENCKBHFKLD.Failure:
		case LENCKBHFKLD.Progress:
		{
			ulong eJPBNFMDJBJ = (CKEHOEGLMBM as IHubMessage).HGFDDMNOPJA();
			Hub hGCBNOGDDPB = EDPONAFOEAN(eJPBNFMDJBJ);
			if (hGCBNOGDDPB != null)
			{
				((IHub)hGCBNOGDDPB).OnMessage(CKEHOEGLMBM);
			}
			else
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", string.Format("No Hub found for Progress message! Id: {0}", eJPBNFMDJBJ.ToString()));
			}
			break;
		}
		case LENCKBHFKLD.Data:
			if (OnNonHubMessage != null)
			{
				OnNonHubMessage(this, (CKEHOEGLMBM as DataMessage).CHIGLEKCFFN());
			}
			break;
		case LENCKBHFKLD.KeepAlive:
			break;
		default:
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", "Unknown message type received: " + CKEHOEGLMBM.get_Type());
			break;
		}
	}

	void IConnection.TransportStarted()
	{
		if (FLBBFDNHJAJ() != OHLFKFFAOMF.Connecting)
		{
			return;
		}
		MPHLGELKCFE();
		if (OnConnected != null)
		{
			try
			{
				OnConnected(this);
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("SignalR Connection", "OnOpened", mPFFFAOGBJE);
			}
		}
		if (KIECCNGHGOG != null)
		{
			for (int i = 0; i < KIECCNGHGOG.Count; i++)
			{
				((IConnection)this).OnMessage(KIECCNGHGOG[i]);
			}
			KIECCNGHGOG.Clear();
			KIECCNGHGOG = null;
		}
	}

	void IConnection.TransportReconnected()
	{
		if (FLBBFDNHJAJ() != OHLFKFFAOMF.Reconnecting)
		{
			return;
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SignalR Connection", "Transport Reconnected");
		MPHLGELKCFE();
		if (OnReconnected == null)
		{
			return;
		}
		try
		{
			OnReconnected(this);
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("SignalR Connection", "OnReconnected", mPFFFAOGBJE);
		}
	}

	void IConnection.TransportAborted()
	{
		Close();
	}

	void IConnection.Error(string NEPOLDCKNJL)
	{
		if (FLBBFDNHJAJ() != OHLFKFFAOMF.Closed)
		{
			HTTPManager.MBBMPNDDPIH().Error("SignalR Connection", NEPOLDCKNJL);
			if (onErrorField != null)
			{
				onErrorField(this, NEPOLDCKNJL);
			}
			if (FLBBFDNHJAJ() == OHLFKFFAOMF.Connected || FLBBFDNHJAJ() == OHLFKFFAOMF.Reconnecting)
			{
				IGFIEFDGBDJ();
			}
			else if (FLBBFDNHJAJ() != OHLFKFFAOMF.Connecting || !JLAECHHBCAK())
			{
				Close();
			}
		}
	}

	Uri IConnection.BuildUri(FHIEGKMHOCC LFLGCDNKNJI)
	{
		return ((IConnection)this).BuildUri(LFLGCDNKNJI, (TransportBase)null);
	}

	Uri IConnection.BuildUri(FHIEGKMHOCC LFLGCDNKNJI, TransportBase CHMELBKHOPP)
	{
		lock (SyncRoot)
		{
			queryBuilder.Length = 0;
			UriBuilder uriBuilder = new UriBuilder(OJBDMGBGJMA());
			if (!uriBuilder.Path.EndsWith("/"))
			{
				uriBuilder.Path += "/";
			}
			PHFIIKGKDCF %= ulong.MaxValue;
			switch (LFLGCDNKNJI)
			{
			case FHIEGKMHOCC.Negotiate:
				uriBuilder.Path += "negotiate";
				goto default;
			case FHIEGKMHOCC.Connect:
				if (CHMELBKHOPP != null && CHMELBKHOPP.get_Type() == AHLJIMDEAJD.WebSocket)
				{
					uriBuilder.Scheme = ((!HTTPProtocolFactory.IsSecureProtocol(OJBDMGBGJMA())) ? "ws" : "wss");
				}
				uriBuilder.Path += "connect";
				goto default;
			case FHIEGKMHOCC.Start:
				uriBuilder.Path += "start";
				goto default;
			case FHIEGKMHOCC.Poll:
				uriBuilder.Path += "poll";
				if (HGJBHOLJFED != null)
				{
					queryBuilder.Append("messageId=");
					queryBuilder.Append(HGJBHOLJFED.BJOOBDBFHGL());
				}
				goto default;
			case FHIEGKMHOCC.Send:
				uriBuilder.Path += "send";
				goto default;
			case FHIEGKMHOCC.Reconnect:
				if (CHMELBKHOPP != null && CHMELBKHOPP.get_Type() == AHLJIMDEAJD.WebSocket)
				{
					uriBuilder.Scheme = ((!HTTPProtocolFactory.IsSecureProtocol(OJBDMGBGJMA())) ? "ws" : "wss");
				}
				uriBuilder.Path += "reconnect";
				if (HGJBHOLJFED != null)
				{
					queryBuilder.Append("messageId=");
					queryBuilder.Append(HGJBHOLJFED.BJOOBDBFHGL());
				}
				if (!string.IsNullOrEmpty(LPEDAAHHCBD))
				{
					if (queryBuilder.Length > 0)
					{
						queryBuilder.Append("&");
					}
					queryBuilder.Append("groupsToken=");
					queryBuilder.Append(LPEDAAHHCBD);
				}
				goto default;
			case FHIEGKMHOCC.Abort:
				uriBuilder.Path += "abort";
				goto default;
			case FHIEGKMHOCC.Ping:
				uriBuilder.Path += "ping";
				queryBuilder.Append("&tid=");
				queryBuilder.Append(PHFIIKGKDCF++.ToString());
				queryBuilder.Append("&_=");
				queryBuilder.Append(GPEEDKOHFIG().ToString());
				break;
			default:
				if (queryBuilder.Length > 0)
				{
					queryBuilder.Append("&");
				}
				queryBuilder.Append("tid=");
				queryBuilder.Append(PHFIIKGKDCF++.ToString());
				queryBuilder.Append("&_=");
				queryBuilder.Append(GPEEDKOHFIG().ToString());
				if (CHMELBKHOPP != null)
				{
					queryBuilder.Append("&transport=");
					queryBuilder.Append(CHMELBKHOPP.get_Name());
				}
				queryBuilder.Append("&clientProtocol=");
				queryBuilder.Append(NJNGCKKIKMG);
				if (EOBPEOEMEDB() != null && !string.IsNullOrEmpty(EOBPEOEMEDB().HKBCNJMMOOP()))
				{
					queryBuilder.Append("&connectionToken=");
					queryBuilder.Append(EOBPEOEMEDB().HKBCNJMMOOP());
				}
				if (LINDGKFKGND() != null && LINDGKFKGND().Length > 0)
				{
					queryBuilder.Append("&connectionData=");
					queryBuilder.Append(CPEMFGDHOCB());
				}
				break;
			}
			if (MONGJAOIELO() != null && MONGJAOIELO().Count > 0)
			{
				queryBuilder.Append(FGDGEPEPCJL());
			}
			uriBuilder.Query = queryBuilder.ToString();
			queryBuilder.Length = 0;
			return uriBuilder.Uri;
		}
	}

	HTTPRequest IConnection.PrepareRequest(HTTPRequest CGOIOKHEGOE, FHIEGKMHOCC LFLGCDNKNJI)
	{
		if (CGOIOKHEGOE != null && DLKDCNNCKCL() != null)
		{
			DLKDCNNCKCL().PrepareRequest(CGOIOKHEGOE, LFLGCDNKNJI);
		}
		if (MAKFGPNOKNL() != null)
		{
			MAKFGPNOKNL()(this, CGOIOKHEGOE, LFLGCDNKNJI);
		}
		return CGOIOKHEGOE;
	}

	string IConnection.ParseResponse(string GHCCHADLAEK)
	{
		Dictionary<string, object> dictionary = Json.Decode(GHCCHADLAEK) as Dictionary<string, object>;
		if (dictionary == null)
		{
			((IConnection)this).Error("Failed to parse Start response: " + GHCCHADLAEK);
			return string.Empty;
		}
		object value;
		if (!dictionary.TryGetValue("Response", out value) || value == null)
		{
			((IConnection)this).Error("No 'Response' key found in response: " + GHCCHADLAEK);
			return string.Empty;
		}
		return value.ToString();
	}

	void IHeartbeat.OnHeartbeatUpdate(TimeSpan OJOKANCMPLG)
	{
		OHLFKFFAOMF oHLFKFFAOMF = FLBBFDNHJAJ();
		if (oHLFKFFAOMF == OHLFKFFAOMF.Connected)
		{
			if (LODFOKFEAPC().IBMJBEKAIAH() && EOBPEOEMEDB().FCIMGPJDODG().HasValue)
			{
				TimeSpan? timeSpan = EOBPEOEMEDB().FCIMGPJDODG();
				if (timeSpan.HasValue && DateTime.UtcNow - LAPLGEGKFGI >= timeSpan.GetValueOrDefault())
				{
					IGFIEFDGBDJ();
				}
			}
			if (ACHGLFBEJCK == null && DateTime.UtcNow - OIHEGPAPFIO >= PingInterval)
			{
				PFNOKDGKMBE();
			}
			return;
		}
		DateTime? jEAPOHAGCLL = JEAPOHAGCLL;
		if (jEAPOHAGCLL.HasValue)
		{
			DateTime? jEAPOHAGCLL2 = JEAPOHAGCLL;
			TimeSpan? timeSpan2 = ((!jEAPOHAGCLL2.HasValue) ? ((TimeSpan?)null) : new TimeSpan?(DateTime.UtcNow - jEAPOHAGCLL2.GetValueOrDefault()));
			if (timeSpan2.HasValue && timeSpan2.GetValueOrDefault() >= EOBPEOEMEDB().BICBAKIOCMM())
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", "OnHeartbeatUpdate - Transport failed to connect in the given time!");
				((IConnection)this).Error("Transport failed to connect in the given time!");
			}
		}
		DateTime? jNPJOFDOAAG = JNPJOFDOAAG;
		if (jNPJOFDOAAG.HasValue)
		{
			DateTime? jNPJOFDOAAG2 = JNPJOFDOAAG;
			TimeSpan? timeSpan3 = ((!jNPJOFDOAAG2.HasValue) ? ((TimeSpan?)null) : new TimeSpan?(DateTime.UtcNow - jNPJOFDOAAG2.GetValueOrDefault()));
			if (timeSpan3.HasValue && timeSpan3.GetValueOrDefault() >= EOBPEOEMEDB().BDMKAEDCGNL())
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("SignalR Connection", "OnHeartbeatUpdate - Failed to reconnect in the given time!");
				Close();
			}
		}
	}

	private void MPHLGELKCFE()
	{
		set_State(OHLFKFFAOMF.Connected);
		JNPJOFDOAAG = null;
		JEAPOHAGCLL = null;
		OIHEGPAPFIO = DateTime.UtcNow;
		LAPLGEGKFGI = DateTime.UtcNow;
		HTTPManager.MAMNLAJACOD().ELAHFBCGAGL(this);
	}

	private Hub EDPONAFOEAN(ulong EJPBNFMDJBJ)
	{
		if (LINDGKFKGND() != null)
		{
			for (int i = 0; i < LINDGKFKGND().Length; i++)
			{
				if (((IHub)LINDGKFKGND()[i]).HasSentMessageId(EJPBNFMDJBJ))
				{
					return LINDGKFKGND()[i];
				}
			}
		}
		return null;
	}

	private bool JLAECHHBCAK()
	{
		if (FLBBFDNHJAJ() == OHLFKFFAOMF.Connecting)
		{
			if (KIECCNGHGOG != null)
			{
				KIECCNGHGOG.Clear();
			}
			LODFOKFEAPC().Stop();
			AOJLKJODKMC(null);
			switch (PJJBHIFNPGF)
			{
			case OBBKIBFJEMI.ServerSentEvents:
				AOJLKJODKMC(new ServerSentEventsTransport(this));
				PJJBHIFNPGF = OBBKIBFJEMI.HTTP;
				break;
			case OBBKIBFJEMI.HTTP:
				AOJLKJODKMC(new PollingTransport(this));
				PJJBHIFNPGF = OBBKIBFJEMI.Unknown;
				break;
			case OBBKIBFJEMI.Unknown:
				return false;
			}
			JEAPOHAGCLL = DateTime.UtcNow;
			LODFOKFEAPC().NDCILHIAPIK();
			if (ACHGLFBEJCK != null)
			{
				ACHGLFBEJCK.AKLEEMEHBIC();
			}
			return true;
		}
		return false;
	}

	private void PFNOKDGKMBE()
	{
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SignalR Connection", "Sending Ping request.");
		ACHGLFBEJCK = new HTTPRequest(((IConnection)this).BuildUri(FHIEGKMHOCC.Ping), KCNMPCMGCGH);
		ACHGLFBEJCK.CLDOBKEACOC(PingInterval);
		ACHGLFBEJCK.Send();
		OIHEGPAPFIO = DateTime.UtcNow;
	}

	private void KCNMPCMGCGH(HTTPRequest CGOIOKHEGOE, HTTPResponse BEIGFGCBICO)
	{
		ACHGLFBEJCK = null;
		string text = string.Empty;
		switch (CGOIOKHEGOE.FLBBFDNHJAJ())
		{
		case CFGBMHKCENK.Finished:
			if (BEIGFGCBICO.AICKPAMONBH())
			{
				string text2 = ((IConnection)this).ParseResponse(BEIGFGCBICO.DPBLPGKOEJB());
				if (text2 != "pong")
				{
					text = "Wrong answer for ping request: " + text2;
				}
				else
				{
					HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("SignalR Connection", "Pong received.");
				}
			}
			else
			{
				text = string.Format("Ping - Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", BEIGFGCBICO.KNMDPGBPNED(), BEIGFGCBICO.DCKPMHKDLEJ(), BEIGFGCBICO.DPBLPGKOEJB());
			}
			break;
		case CFGBMHKCENK.Error:
			text = "Ping - Request Finished with Error! " + ((CGOIOKHEGOE.IEFGFKFHNMD() == null) ? "No Exception" : (CGOIOKHEGOE.IEFGFKFHNMD().Message + "\n" + CGOIOKHEGOE.IEFGFKFHNMD().StackTrace));
			break;
		case CFGBMHKCENK.ConnectionTimedOut:
			text = "Ping - Connection Timed Out!";
			break;
		case CFGBMHKCENK.TimedOut:
			text = "Ping - Processing the request Timed Out!";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			((IConnection)this).Error(text);
		}
	}
}
