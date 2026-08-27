using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Org.BouncyCastle.Crypto.Tls;
using UnityEngine;

public sealed class HTTPRequest : IEnumerator<HTTPRequest>, IDisposable, IEnumerator
{
	internal static readonly byte[] HGBANJPCEPF = new byte[2] { 13, 10 };

	internal static readonly string[] MethodNames = new string[6]
	{
		LAAFHDKKJFL.Get.ToString().ToUpper(),
		LAAFHDKKJFL.Head.ToString().ToUpper(),
		LAAFHDKKJFL.Post.ToString().ToUpper(),
		LAAFHDKKJFL.Put.ToString().ToUpper(),
		LAAFHDKKJFL.Delete.ToString().ToUpper(),
		LAAFHDKKJFL.Patch.ToString().ToUpper()
	};

	public static int INCPJJNNLAH = 1024;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri NHCOGAAPOAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private LAAFHDKKJFL GEHKCDKIFFI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private byte[] NCMBNCMCKEL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Stream PANPGKLJELD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KOAKOCBLDKM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CCNLKBOIHEJ;

	public OnUploadProgressDelegate EEPGPFILKFI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnRequestFinishedDelegate CEIILBMBHNC;

	public OnDownloadProgressDelegate OGLIKFCADME;

	public OnRequestFinishedDelegate GFFABFBMJAO;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool BGIFEDNADAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool GEHBFMDMCFM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Uri GHHMHJFFKAN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPResponse NMDDHIJHLEF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPResponse LLEGKHNLKPK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Exception DIJNDDDGJBF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private object LMKADGBMEDI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Credentials ACGBCDDPEGA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private HTTPProxy FGGPKCKKPNB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int BMGAKPGILNG;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool KEANPHNAPBI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool ENOAEBAADAD;

	private List<Cookie> POPFAPJLJDC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private AIEMPPBDGNH ENAKFEIKHIM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private CFGBMHKCENK MKHEFCIEOCA;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int GENKKLDJCMI;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Func<HTTPRequest, X509Certificate, X509Chain, bool> CustomCertificationValidator;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan FNHLIDGNHLF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TimeSpan OCOBNPGODHJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool MNLGBPBDEOI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int NPKBBOEHNGD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ICertificateVerifyer GHCGPGJOKHN;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OBBKIBFJEMI IAKAMBLPFIO;

	private OnBeforeRedirectionDelegate IGFALGIFOAH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int NGNIFINHADM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int AADKGFKCFAH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool DEGMNGAPLMM;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long ECCKDNEBLPF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private long OANOIHGPFCJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool PMPPMACHDFJ;

	private bool LLLAPINJJIJ;

	private bool JNCJAGIBJFL;

	private int BDIBIKNPMJF;

	private bool MAKCLIKPHFD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Dictionary<string, List<string>> BOEIOCLGPDI;

	private HTTPFormBase OFMIEIKGJJB;

	private HTTPFormBase DNDHNALKCFF;

	HTTPRequest IEnumerator<HTTPRequest>.Current
	{
		get
		{
			return System_002ECollections_002EGeneric_002EIEnumerator_003CBestHTTP_002EHTTPRequest_003E_002Eget_Current();
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

	public LAAFHDKKJFL DMMBLEBFHEK
	{
		get
		{
			return JCHNIGKBBMI();
		}
		set
		{
			CEMMLGJJAAL(value);
		}
	}

	public byte[] CPGFEOPNAGN
	{
		get
		{
			return BEKEFBNFBFJ();
		}
		set
		{
			set_RawData(value);
		}
	}

	public bool CNGGLELNHEN
	{
		get
		{
			return PGBKODJJGGF();
		}
		set
		{
			ENFCNCGMJAK(value);
		}
	}

	public bool ENGDAEEEPIE
	{
		get
		{
			return NFDILFHDGAB();
		}
		set
		{
			JEIFGAOMDAP(value);
		}
	}

	public bool PLCLCHIAIKG
	{
		get
		{
			return DPCEODODILD();
		}
		set
		{
			PAIGIMAHIME(value);
		}
	}

	public bool BLPOIMACEPM
	{
		get
		{
			return DCOLJJKGFGD();
		}
		set
		{
			JJCLPAGJEBJ(value);
		}
	}

	public bool LGNPEHCBANB
	{
		get
		{
			return MDEPOKKKKCL();
		}
		set
		{
			DMHKNGKPHLJ(value);
		}
	}

	public int HOFDIMEBECJ
	{
		get
		{
			return CKFPMFMHPGI();
		}
		set
		{
			LPALILOEHPE(value);
		}
	}

	public OnRequestFinishedDelegate GKLKCHEHOGH
	{
		get
		{
			return FOIICPPDENH();
		}
		set
		{
			AFGFGHKDJJI(value);
		}
	}

	public bool JONCACCHFCP
	{
		get
		{
			return CKLEKLGMEAG();
		}
		set
		{
			LADBBAMKLPJ(value);
		}
	}

	public bool CLNIFKPHMBL
	{
		get
		{
			return BBDEHICPIFI();
		}
		internal set
		{
			MAKPGAOFDOD(value);
		}
	}

	public Uri DLMDHFFCILP
	{
		get
		{
			return GHIEHEIOANG();
		}
		internal set
		{
			NPFNLBPENAC(value);
		}
	}

	public Uri MKLGBJIGGDE
	{
		get
		{
			return DKAECMGPGOE();
		}
	}

	public HTTPResponse EJFMJFKAFDN
	{
		get
		{
			return POGDKNCHIBG();
		}
		internal set
		{
			AOMLIJAIJHE(value);
		}
	}

	public HTTPResponse ICGLLDJCGHB
	{
		get
		{
			return MJKNMBDFBID();
		}
		internal set
		{
			FFBIEJDBKIL(value);
		}
	}

	public Exception COHEDILAHFD
	{
		get
		{
			return IEFGFKFHNMD();
		}
		internal set
		{
			set_Exception(value);
		}
	}

	public object DDFDDHGJFBO
	{
		get
		{
			return LOIGCKFONHJ();
		}
		set
		{
			set_Tag(value);
		}
	}

	public Credentials MADDPLIJOIP
	{
		get
		{
			return HPKPFEOBIOC();
		}
		set
		{
			PJELDABIDCA(value);
		}
	}

	public bool JKNCBCEAILF
	{
		get
		{
			return AOPIGGFCGHC();
		}
	}

	public HTTPProxy DEFLNIGINCO
	{
		get
		{
			return FHGBKFBCGCO();
		}
		set
		{
			PNGMAECJHID(value);
		}
	}

	public int IOOLGJDFBJG
	{
		get
		{
			return MNBNOBNFOJH();
		}
		set
		{
			CFHKJBBLPND(value);
		}
	}

	public bool GPDGHBJHJGC
	{
		get
		{
			return KMOEMMLAJNC();
		}
		set
		{
			GFJNLIKIEMH(value);
		}
	}

	public bool PHLKGBCMHMO
	{
		get
		{
			return IJJCLBHKMDJ();
		}
		set
		{
			AGOGJCLDCGG(value);
		}
	}

	public List<Cookie> FPFLODAGEFD
	{
		get
		{
			return HNDADBHDOID();
		}
		set
		{
			PPLAPHMALFL(value);
		}
	}

	public AIEMPPBDGNH NKHMOEIOMKI
	{
		get
		{
			return CCJNBGLGAAM();
		}
		set
		{
			OJCFIIONEKJ(value);
		}
	}

	public CFGBMHKCENK AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
		internal set
		{
			set_State(value);
		}
	}

	public int FOABKLLOPPH
	{
		get
		{
			return FJNLLEMJKDC();
		}
		internal set
		{
			NDCFOHCFKHE(value);
		}
	}

	public TimeSpan PFODFFILGKE
	{
		get
		{
			return DGHOJLHDGPB();
		}
		set
		{
			CLDOBKEACOC(value);
		}
	}

	public TimeSpan BEOBDJHNHIO
	{
		get
		{
			return FJKGKLJGIJI();
		}
		set
		{
			DKLGPGDJPGO(value);
		}
	}

	public bool PHJOOGBCGJF
	{
		get
		{
			return IFPLGJHAANE();
		}
		set
		{
			AHAMAPFDLMH(value);
		}
	}

	public int Priority
	{
		get
		{
			return KCKAPPJABBL();
		}
		set
		{
			INEEHPCAICE(value);
		}
	}

	public ICertificateVerifyer EHONOPHLIKI
	{
		get
		{
			return KNFEJHLHPDO();
		}
		set
		{
			MJNIKOEJCFO(value);
		}
	}

	public OBBKIBFJEMI IMKFKJFEMAJ
	{
		get
		{
			return BEKFCACGBLL();
		}
		set
		{
			MBLIFPIOOON(value);
		}
	}

	internal int DHBCMOJLDFB
	{
		get
		{
			return IBILKGBKKOI();
		}
		set
		{
			BHOHEPLCIOI(value);
		}
	}

	internal int NAKFHGEGIHG
	{
		get
		{
			return ELADIMFGGEO();
		}
		set
		{
			HEEHALMDLPL(value);
		}
	}

	internal bool GBBBIAEKFIA
	{
		get
		{
			return BOABEDJEDDC();
		}
		set
		{
			HNPAEADANKK(value);
		}
	}

	internal long LABHLMBPEPA
	{
		get
		{
			return ABNBBLEAKAP();
		}
	}

	internal long MIIOHMHDIJF
	{
		get
		{
			return MJBCCNEIBDA();
		}
		private set
		{
			DPGCNHGFLBC(value);
		}
	}

	internal long CJELLNDJCDM
	{
		get
		{
			return LKHMFMMBAHL();
		}
		private set
		{
			HOOAAHPEACM(value);
		}
	}

	internal bool GFOCIMKCLGH
	{
		get
		{
			return LCECFOLDKHH();
		}
		set
		{
			MBNHNNCHJAG(value);
		}
	}

	private Dictionary<string, List<string>> CPNAPDCFCDL
	{
		get
		{
			return AJCCGKHBNML();
		}
		set
		{
			set_Headers(value);
		}
	}

	public object BLOOLFFMKFI
	{
		get
		{
			return this;
		}
	}

	public event Func<HTTPRequest, X509Certificate, X509Chain, bool> JAGKBEDPHNB
	{
		add
		{
			PIODJFHAHDL(value);
		}
		remove
		{
			HMCFCFPAMDG(value);
		}
	}

	public event OnBeforeRedirectionDelegate EPJFGMCDAFO
	{
		add
		{
			MHDBEHENOOO(value);
		}
		remove
		{
			KIEOPMBICDD(value);
		}
	}

	public HTTPRequest(Uri KJHNCLAJMLO)
		: this(KJHNCLAJMLO, LAAFHDKKJFL.Get, HTTPManager.HAIGHJHOEDH(), HTTPManager.NLGHFPFIMMH(), null)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, OnRequestFinishedDelegate callback)
		: this(KJHNCLAJMLO, LAAFHDKKJFL.Get, HTTPManager.HAIGHJHOEDH(), HTTPManager.NLGHFPFIMMH(), callback)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, bool LLLAPINJJIJ, OnRequestFinishedDelegate callback)
		: this(KJHNCLAJMLO, LAAFHDKKJFL.Get, LLLAPINJJIJ, HTTPManager.NLGHFPFIMMH(), callback)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, bool LLLAPINJJIJ, bool JNCJAGIBJFL, OnRequestFinishedDelegate callback)
		: this(KJHNCLAJMLO, LAAFHDKKJFL.Get, LLLAPINJJIJ, JNCJAGIBJFL, callback)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, LAAFHDKKJFL AMFJIGAEHLD)
		: this(KJHNCLAJMLO, AMFJIGAEHLD, HTTPManager.HAIGHJHOEDH(), HTTPManager.NLGHFPFIMMH() || AMFJIGAEHLD != LAAFHDKKJFL.Get, null)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, LAAFHDKKJFL AMFJIGAEHLD, OnRequestFinishedDelegate callback)
		: this(KJHNCLAJMLO, AMFJIGAEHLD, HTTPManager.HAIGHJHOEDH(), HTTPManager.NLGHFPFIMMH() || AMFJIGAEHLD != LAAFHDKKJFL.Get, callback)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, LAAFHDKKJFL AMFJIGAEHLD, bool LLLAPINJJIJ, OnRequestFinishedDelegate callback)
		: this(KJHNCLAJMLO, AMFJIGAEHLD, LLLAPINJJIJ, HTTPManager.NLGHFPFIMMH() || AMFJIGAEHLD != LAAFHDKKJFL.Get, callback)
	{
	}

	public HTTPRequest(Uri KJHNCLAJMLO, LAAFHDKKJFL AMFJIGAEHLD, bool LLLAPINJJIJ, bool JNCJAGIBJFL, OnRequestFinishedDelegate callback)
	{
		set_Uri(KJHNCLAJMLO);
		CEMMLGJJAAL(AMFJIGAEHLD);
		PAIGIMAHIME(LLLAPINJJIJ);
		JJCLPAGJEBJ(JNCJAGIBJFL);
		AFGFGHKDJJI(callback);
		LPALILOEHPE(4096);
		LADBBAMKLPJ(AMFJIGAEHLD == LAAFHDKKJFL.Post);
		CFHKJBBLPND(int.MaxValue);
		NDCFOHCFKHE(0);
		AGOGJCLDCGG(HTTPManager.IJJCLBHKMDJ());
		int bAINMLLIKOL = 0;
		HEEHALMDLPL(bAINMLLIKOL);
		BHOHEPLCIOI(bAINMLLIKOL);
		HNPAEADANKK(false);
		set_State(CFGBMHKCENK.Initial);
		CLDOBKEACOC(HTTPManager.DGHOJLHDGPB());
		DKLGPGDJPGO(HTTPManager.AFHJBDAKIPE());
		AHAMAPFDLMH(false);
		PNGMAECJHID(HTTPManager.FHGBKFBCGCO());
		JEIFGAOMDAP(true);
		ENFCNCGMJAK(true);
		MJNIKOEJCFO(HTTPManager.MBEAAMJILEI());
		GFJNLIKIEMH(HTTPManager.IHBIPNGCEFM());
	}

	public Uri OJBDMGBGJMA()
	{
		return NHCOGAAPOAB;
	}

	private void set_Uri(Uri value)
	{
		NHCOGAAPOAB = value;
	}

	public LAAFHDKKJFL JCHNIGKBBMI()
	{
		return GEHKCDKIFFI;
	}

	public void CEMMLGJJAAL(LAAFHDKKJFL value)
	{
		GEHKCDKIFFI = value;
	}

	public byte[] BEKEFBNFBFJ()
	{
		return NCMBNCMCKEL;
	}

	public void set_RawData(byte[] value)
	{
		NCMBNCMCKEL = value;
	}

	public Stream IHMCGKHBLKN()
	{
		return PANPGKLJELD;
	}

	public void set_UploadStream(Stream value)
	{
		PANPGKLJELD = value;
	}

	public bool PGBKODJJGGF()
	{
		return KOAKOCBLDKM;
	}

	public void ENFCNCGMJAK(bool value)
	{
		KOAKOCBLDKM = value;
	}

	public bool NFDILFHDGAB()
	{
		return CCNLKBOIHEJ;
	}

	public void JEIFGAOMDAP(bool value)
	{
		CCNLKBOIHEJ = value;
	}

	public bool DPCEODODILD()
	{
		return LLLAPINJJIJ;
	}

	public void PAIGIMAHIME(bool value)
	{
		if (FLBBFDNHJAJ() == CFGBMHKCENK.Processing)
		{
			throw new NotSupportedException("Changing the IsKeepAlive property while processing the request is not supported.");
		}
		LLLAPINJJIJ = value;
	}

	public bool DCOLJJKGFGD()
	{
		return JNCJAGIBJFL;
	}

	public void JJCLPAGJEBJ(bool value)
	{
		if (FLBBFDNHJAJ() == CFGBMHKCENK.Processing)
		{
			throw new NotSupportedException("Changing the DisableCache property while processing the request is not supported.");
		}
		JNCJAGIBJFL = value;
	}

	public bool MDEPOKKKKCL()
	{
		return MAKCLIKPHFD;
	}

	public void DMHKNGKPHLJ(bool value)
	{
		if (FLBBFDNHJAJ() == CFGBMHKCENK.Processing)
		{
			throw new NotSupportedException("Changing the UseStreaming property while processing the request is not supported.");
		}
		MAKCLIKPHFD = value;
	}

	public int CKFPMFMHPGI()
	{
		return BDIBIKNPMJF;
	}

	public void LPALILOEHPE(int value)
	{
		if (FLBBFDNHJAJ() == CFGBMHKCENK.Processing)
		{
			throw new NotSupportedException("Changing the StreamFragmentSize property while processing the request is not supported.");
		}
		if (value < 1)
		{
			throw new ArgumentException("StreamFragmentSize must be at least 1.");
		}
		BDIBIKNPMJF = value;
	}

	public OnRequestFinishedDelegate FOIICPPDENH()
	{
		return CEIILBMBHNC;
	}

	public void AFGFGHKDJJI(OnRequestFinishedDelegate value)
	{
		CEIILBMBHNC = value;
	}

	public bool CKLEKLGMEAG()
	{
		return BGIFEDNADAB;
	}

	public void LADBBAMKLPJ(bool value)
	{
		BGIFEDNADAB = value;
	}

	public bool BBDEHICPIFI()
	{
		return GEHBFMDMCFM;
	}

	internal void MAKPGAOFDOD(bool value)
	{
		GEHBFMDMCFM = value;
	}

	public Uri GHIEHEIOANG()
	{
		return GHHMHJFFKAN;
	}

	internal void NPFNLBPENAC(Uri value)
	{
		GHHMHJFFKAN = value;
	}

	public Uri DKAECMGPGOE()
	{
		return (!BBDEHICPIFI()) ? OJBDMGBGJMA() : GHIEHEIOANG();
	}

	public HTTPResponse POGDKNCHIBG()
	{
		return NMDDHIJHLEF;
	}

	internal void AOMLIJAIJHE(HTTPResponse value)
	{
		NMDDHIJHLEF = value;
	}

	public HTTPResponse MJKNMBDFBID()
	{
		return LLEGKHNLKPK;
	}

	internal void FFBIEJDBKIL(HTTPResponse value)
	{
		LLEGKHNLKPK = value;
	}

	public Exception IEFGFKFHNMD()
	{
		return DIJNDDDGJBF;
	}

	internal void set_Exception(Exception value)
	{
		DIJNDDDGJBF = value;
	}

	public object LOIGCKFONHJ()
	{
		return LMKADGBMEDI;
	}

	public void set_Tag(object value)
	{
		LMKADGBMEDI = value;
	}

	public Credentials HPKPFEOBIOC()
	{
		return ACGBCDDPEGA;
	}

	public void PJELDABIDCA(Credentials value)
	{
		ACGBCDDPEGA = value;
	}

	public bool AOPIGGFCGHC()
	{
		return FHGBKFBCGCO() != null;
	}

	public HTTPProxy FHGBKFBCGCO()
	{
		return FGGPKCKKPNB;
	}

	public void PNGMAECJHID(HTTPProxy value)
	{
		FGGPKCKKPNB = value;
	}

	public int MNBNOBNFOJH()
	{
		return BMGAKPGILNG;
	}

	public void CFHKJBBLPND(int value)
	{
		BMGAKPGILNG = value;
	}

	public bool KMOEMMLAJNC()
	{
		return KEANPHNAPBI;
	}

	public void GFJNLIKIEMH(bool value)
	{
		KEANPHNAPBI = value;
	}

	public bool IJJCLBHKMDJ()
	{
		return ENOAEBAADAD;
	}

	public void AGOGJCLDCGG(bool value)
	{
		ENOAEBAADAD = value;
	}

	public List<Cookie> HNDADBHDOID()
	{
		if (POPFAPJLJDC == null)
		{
			POPFAPJLJDC = new List<Cookie>();
		}
		return POPFAPJLJDC;
	}

	public void PPLAPHMALFL(List<Cookie> value)
	{
		POPFAPJLJDC = value;
	}

	public AIEMPPBDGNH CCJNBGLGAAM()
	{
		return ENAKFEIKHIM;
	}

	public void OJCFIIONEKJ(AIEMPPBDGNH value)
	{
		ENAKFEIKHIM = value;
	}

	public CFGBMHKCENK FLBBFDNHJAJ()
	{
		return MKHEFCIEOCA;
	}

	internal void set_State(CFGBMHKCENK value)
	{
		MKHEFCIEOCA = value;
	}

	public int FJNLLEMJKDC()
	{
		return GENKKLDJCMI;
	}

	internal void NDCFOHCFKHE(int value)
	{
		GENKKLDJCMI = value;
	}

	public void PIODJFHAHDL(Func<HTTPRequest, X509Certificate, X509Chain, bool> value)
	{
		Func<HTTPRequest, X509Certificate, X509Chain, bool> func = CustomCertificationValidator;
		Func<HTTPRequest, X509Certificate, X509Chain, bool> func2;
		do
		{
			func2 = func;
			func = Interlocked.CompareExchange(ref CustomCertificationValidator, (Func<HTTPRequest, X509Certificate, X509Chain, bool>)Delegate.Combine(func2, value), func);
		}
		while ((object)func != func2);
	}

	public void HMCFCFPAMDG(Func<HTTPRequest, X509Certificate, X509Chain, bool> value)
	{
		Func<HTTPRequest, X509Certificate, X509Chain, bool> func = CustomCertificationValidator;
		Func<HTTPRequest, X509Certificate, X509Chain, bool> func2;
		do
		{
			func2 = func;
			func = Interlocked.CompareExchange(ref CustomCertificationValidator, (Func<HTTPRequest, X509Certificate, X509Chain, bool>)Delegate.Remove(func2, value), func);
		}
		while ((object)func != func2);
	}

	public TimeSpan DGHOJLHDGPB()
	{
		return FNHLIDGNHLF;
	}

	public void CLDOBKEACOC(TimeSpan value)
	{
		FNHLIDGNHLF = value;
	}

	public TimeSpan FJKGKLJGIJI()
	{
		return OCOBNPGODHJ;
	}

	public void DKLGPGDJPGO(TimeSpan value)
	{
		OCOBNPGODHJ = value;
	}

	public bool IFPLGJHAANE()
	{
		return MNLGBPBDEOI;
	}

	public void AHAMAPFDLMH(bool value)
	{
		MNLGBPBDEOI = value;
	}

	public int KCKAPPJABBL()
	{
		return NPKBBOEHNGD;
	}

	public void INEEHPCAICE(int value)
	{
		NPKBBOEHNGD = value;
	}

	public ICertificateVerifyer KNFEJHLHPDO()
	{
		return GHCGPGJOKHN;
	}

	public void MJNIKOEJCFO(ICertificateVerifyer value)
	{
		GHCGPGJOKHN = value;
	}

	public OBBKIBFJEMI BEKFCACGBLL()
	{
		return IAKAMBLPFIO;
	}

	public void MBLIFPIOOON(OBBKIBFJEMI value)
	{
		IAKAMBLPFIO = value;
	}

	public void MHDBEHENOOO(OnBeforeRedirectionDelegate value)
	{
		IGFALGIFOAH = (OnBeforeRedirectionDelegate)Delegate.Combine(IGFALGIFOAH, value);
	}

	public void KIEOPMBICDD(OnBeforeRedirectionDelegate value)
	{
		IGFALGIFOAH = (OnBeforeRedirectionDelegate)Delegate.Remove(IGFALGIFOAH, value);
	}

	internal int IBILKGBKKOI()
	{
		return NGNIFINHADM;
	}

	internal void BHOHEPLCIOI(int value)
	{
		NGNIFINHADM = value;
	}

	internal int ELADIMFGGEO()
	{
		return AADKGFKCFAH;
	}

	internal void HEEHALMDLPL(int value)
	{
		AADKGFKCFAH = value;
	}

	internal bool BOABEDJEDDC()
	{
		return DEGMNGAPLMM;
	}

	internal void HNPAEADANKK(bool value)
	{
		DEGMNGAPLMM = value;
	}

	internal long ABNBBLEAKAP()
	{
		if (IHMCGKHBLKN() == null || !NFDILFHDGAB())
		{
			return -1L;
		}
		try
		{
			return IHMCGKHBLKN().Length;
		}
		catch
		{
			return -1L;
		}
	}

	internal long MJBCCNEIBDA()
	{
		return ECCKDNEBLPF;
	}

	private void DPGCNHGFLBC(long value)
	{
		ECCKDNEBLPF = value;
	}

	internal long LKHMFMMBAHL()
	{
		return OANOIHGPFCJ;
	}

	private void HOOAAHPEACM(long value)
	{
		OANOIHGPFCJ = value;
	}

	internal bool LCECFOLDKHH()
	{
		return PMPPMACHDFJ;
	}

	internal void MBNHNNCHJAG(bool value)
	{
		PMPPMACHDFJ = value;
	}

	private Dictionary<string, List<string>> AJCCGKHBNML()
	{
		return BOEIOCLGPDI;
	}

	private void set_Headers(Dictionary<string, List<string>> value)
	{
		BOEIOCLGPDI = value;
	}

	public void AddField(string LKABGPANBMH, string value)
	{
		AddField(LKABGPANBMH, value, Encoding.UTF8);
	}

	public void AddField(string LKABGPANBMH, string value, Encoding FOPOKALJIIJ)
	{
		if (OFMIEIKGJJB == null)
		{
			OFMIEIKGJJB = new HTTPFormBase();
		}
		OFMIEIKGJJB.AddField(LKABGPANBMH, value, FOPOKALJIIJ);
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME)
	{
		AddBinaryData(LKABGPANBMH, DMNBDBJNKME, null, null);
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME, string PMFEIPCHENB)
	{
		AddBinaryData(LKABGPANBMH, DMNBDBJNKME, PMFEIPCHENB, null);
	}

	public void AddBinaryData(string LKABGPANBMH, byte[] DMNBDBJNKME, string PMFEIPCHENB, string KIDMMGJIEHJ)
	{
		if (OFMIEIKGJJB == null)
		{
			OFMIEIKGJJB = new HTTPFormBase();
		}
		OFMIEIKGJJB.AddBinaryData(LKABGPANBMH, DMNBDBJNKME, PMFEIPCHENB, KIDMMGJIEHJ);
	}

	public void SetFields(WWWForm GHLEOIMGGMO)
	{
		OJCFIIONEKJ(AIEMPPBDGNH.Unity);
		DNDHNALKCFF = new UnityForm(GHLEOIMGGMO);
	}

	public void ONIJMDADJFC(HTTPFormBase HOELLMLEBAK)
	{
		DNDHNALKCFF = HOELLMLEBAK;
	}

	public void PMCMNAIFGJA()
	{
		DNDHNALKCFF = null;
		OFMIEIKGJJB = null;
	}

	private HTTPFormBase NGNCMKLECFI()
	{
		if (DNDHNALKCFF != null)
		{
			return DNDHNALKCFF;
		}
		if (OFMIEIKGJJB == null)
		{
			return null;
		}
		switch (CCJNBGLGAAM())
		{
		case AIEMPPBDGNH.Automatic:
			if (OFMIEIKGJJB.MNLGNEHBCJK() || OFMIEIKGJJB.FJPFFNEKKOL())
			{
				goto case AIEMPPBDGNH.Multipart;
			}
			goto case AIEMPPBDGNH.UrlEncoded;
		case AIEMPPBDGNH.UrlEncoded:
			DNDHNALKCFF = new HTTPUrlEncodedForm();
			break;
		case AIEMPPBDGNH.Multipart:
			DNDHNALKCFF = new HTTPMultiPartForm();
			break;
		case AIEMPPBDGNH.Unity:
			DNDHNALKCFF = new UnityForm();
			break;
		}
		DNDHNALKCFF.CopyFrom(OFMIEIKGJJB);
		return DNDHNALKCFF;
	}

	public void AddHeader(string name, string value)
	{
		if (AJCCGKHBNML() == null)
		{
			set_Headers(new Dictionary<string, List<string>>());
		}
		List<string> list;
		if (!AJCCGKHBNML().TryGetValue(name, out list))
		{
			AJCCGKHBNML().Add(name, list = new List<string>(1));
		}
		list.Add(value);
	}

	public void MMPFBNNMGED(string name, string value)
	{
		if (AJCCGKHBNML() == null)
		{
			set_Headers(new Dictionary<string, List<string>>());
		}
		List<string> list;
		if (!AJCCGKHBNML().TryGetValue(name, out list))
		{
			AJCCGKHBNML().Add(name, list = new List<string>(1));
		}
		list.Clear();
		list.Add(value);
	}

	public bool KKCENCBJJIJ(string name)
	{
		if (AJCCGKHBNML() == null)
		{
			return false;
		}
		return AJCCGKHBNML().Remove(name);
	}

	public bool HasHeader(string name)
	{
		return AJCCGKHBNML() != null && AJCCGKHBNML().ContainsKey(name);
	}

	public string GetFirstHeaderValue(string name)
	{
		if (AJCCGKHBNML() == null)
		{
			return null;
		}
		List<string> value = null;
		if (AJCCGKHBNML().TryGetValue(name, out value) && value.Count > 0)
		{
			return value[0];
		}
		return null;
	}

	public List<string> GetHeaderValues(string name)
	{
		if (AJCCGKHBNML() == null)
		{
			return null;
		}
		List<string> value = null;
		if (AJCCGKHBNML().TryGetValue(name, out value) && value.Count > 0)
		{
			return value;
		}
		return null;
	}

	public void FCOCENOLBEB()
	{
		if (AJCCGKHBNML() != null)
		{
			AJCCGKHBNML().Clear();
		}
	}

	public void SetRangeHeader(int DAENDDKBFGP)
	{
		MMPFBNNMGED("Range", string.Format("bytes={0}-", DAENDDKBFGP));
	}

	public void SetRangeHeader(int DAENDDKBFGP, int PBKEPECFHCK)
	{
		MMPFBNNMGED("Range", string.Format("bytes={0}-{1}", DAENDDKBFGP, PBKEPECFHCK));
	}

	private void SendHeaders(BinaryWriter ABJIEFMMIEK)
	{
		if (!HasHeader("Host"))
		{
			MMPFBNNMGED("Host", DKAECMGPGOE().Authority);
		}
		if (BBDEHICPIFI() && !HasHeader("Referer"))
		{
			AddHeader("Referer", OJBDMGBGJMA().ToString());
		}
		if (!HasHeader("Accept-Encoding"))
		{
			AddHeader("Accept-Encoding", "gzip, identity");
		}
		if (AOPIGGFCGHC() && !HasHeader("Proxy-Connection"))
		{
			AddHeader("Proxy-Connection", (!DPCEODODILD()) ? "Close" : "Keep-Alive");
		}
		if (!HasHeader("Connection"))
		{
			AddHeader("Connection", (!DPCEODODILD()) ? "Close, TE" : "Keep-Alive, TE");
		}
		if (!HasHeader("TE"))
		{
			AddHeader("TE", "identity");
		}
		if (!HasHeader("User-Agent"))
		{
			AddHeader("User-Agent", "BestHTTP");
		}
		long num = -1L;
		if (IHMCGKHBLKN() == null)
		{
			byte[] array = JLLCKEFOEBF();
			num = ((array != null) ? array.Length : 0);
			if (BEKEFBNFBFJ() == null && (DNDHNALKCFF != null || (OFMIEIKGJJB != null && !OFMIEIKGJJB.DAIAOBAEDCB())))
			{
				NGNCMKLECFI();
				if (DNDHNALKCFF != null)
				{
					DNDHNALKCFF.PrepareRequest(this);
				}
			}
		}
		else
		{
			num = ABNBBLEAKAP();
			if (num == -1)
			{
				MMPFBNNMGED("Transfer-Encoding", "Chunked");
			}
			if (!HasHeader("Content-Type"))
			{
				MMPFBNNMGED("Content-Type", "application/octet-stream");
			}
		}
		if (num != -1 && !HasHeader("Content-Length"))
		{
			MMPFBNNMGED("Content-Length", num.ToString());
		}
		if (AOPIGGFCGHC() && FHGBKFBCGCO().HPKPFEOBIOC() != null)
		{
			switch (FHGBKFBCGCO().HPKPFEOBIOC().get_Type())
			{
			case BMBGFBGIAPL.Basic:
				MMPFBNNMGED("Proxy-Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(FHGBKFBCGCO().HPKPFEOBIOC().BFFCEKDPNAM() + ":" + FHGBKFBCGCO().HPKPFEOBIOC().LDEFEGOBBGO())));
				break;
			case BMBGFBGIAPL.Unknown:
			case BMBGFBGIAPL.Digest:
			{
				Digest kHNAPCOOAEF = DigestStore.Get(FHGBKFBCGCO().DNIJHGFINDG());
				if (kHNAPCOOAEF != null)
				{
					string text = kHNAPCOOAEF.CIIGLAEHAOJ(this, FHGBKFBCGCO().HPKPFEOBIOC());
					if (!string.IsNullOrEmpty(text))
					{
						MMPFBNNMGED("Proxy-Authorization", text);
					}
				}
				break;
			}
			}
		}
		if (HPKPFEOBIOC() != null)
		{
			switch (HPKPFEOBIOC().get_Type())
			{
			case BMBGFBGIAPL.Basic:
				MMPFBNNMGED("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(HPKPFEOBIOC().BFFCEKDPNAM() + ":" + HPKPFEOBIOC().LDEFEGOBBGO())));
				break;
			case BMBGFBGIAPL.Unknown:
			case BMBGFBGIAPL.Digest:
			{
				Digest kHNAPCOOAEF2 = DigestStore.Get(DKAECMGPGOE());
				if (kHNAPCOOAEF2 != null)
				{
					string text2 = kHNAPCOOAEF2.CIIGLAEHAOJ(this, HPKPFEOBIOC());
					if (!string.IsNullOrEmpty(text2))
					{
						MMPFBNNMGED("Authorization", text2);
					}
				}
				break;
			}
			}
		}
		List<Cookie> list = ((!IJJCLBHKMDJ()) ? null : CookieJar.Get(DKAECMGPGOE()));
		if (list == null || list.Count == 0)
		{
			list = POPFAPJLJDC;
		}
		else if (POPFAPJLJDC != null)
		{
			for (int i = 0; i < POPFAPJLJDC.Count; i++)
			{
				Cookie NINHECJLKDH = POPFAPJLJDC[i];
				int num2 = list.FindIndex((Cookie ILHDJDNPFKH) => ILHDJDNPFKH.get_Name().Equals(NINHECJLKDH.get_Name()));
				if (num2 >= 0)
				{
					list[num2] = NINHECJLKDH;
				}
				else
				{
					list.Add(NINHECJLKDH);
				}
			}
		}
		if (list != null && list.Count > 0)
		{
			bool flag = true;
			string text3 = string.Empty;
			bool flag2 = HTTPProtocolFactory.IsSecureProtocol(DKAECMGPGOE());
			OBBKIBFJEMI oBBKIBFJEMI = HTTPProtocolFactory.AOMOKHPFJFA(DKAECMGPGOE());
			foreach (Cookie item in list)
			{
				if ((!item.KFPJIIHEAFJ() || (item.KFPJIIHEAFJ() && flag2)) && (!item.BJGFJBHHAFA() || (item.BJGFJBHHAFA() && oBBKIBFJEMI == OBBKIBFJEMI.HTTP)))
				{
					if (!flag)
					{
						text3 += "; ";
					}
					else
					{
						flag = false;
					}
					text3 += item.ToString();
					item.ABGLCGLPNKO(DateTime.UtcNow);
				}
			}
			MMPFBNNMGED("Cookie", text3);
		}
		foreach (KeyValuePair<string, List<string>> item2 in AJCCGKHBNML())
		{
			byte[] buffer = (item2.Key + ": ").GetASCIIBytes();
			for (int num3 = 0; num3 < item2.Value.Count; num3++)
			{
				ABJIEFMMIEK.Write(buffer);
				ABJIEFMMIEK.Write(item2.Value[num3].GetASCIIBytes());
				ABJIEFMMIEK.Write(HGBANJPCEPF);
			}
		}
	}

	public string GOMNEDINJIJ()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter aBJIEFMMIEK = new BinaryWriter(memoryStream))
			{
				SendHeaders(aBJIEFMMIEK);
				return memoryStream.ToArray().JBAOFMBHJND();
			}
		}
	}

	internal byte[] JLLCKEFOEBF()
	{
		if (BEKEFBNFBFJ() != null)
		{
			return BEKEFBNFBFJ();
		}
		if (DNDHNALKCFF != null || (OFMIEIKGJJB != null && !OFMIEIKGJJB.DAIAOBAEDCB()))
		{
			NGNCMKLECFI();
			if (DNDHNALKCFF != null)
			{
				return DNDHNALKCFF.GDENFGNLFKL();
			}
		}
		return null;
	}

	internal void SendOutTo(Stream ABJIEFMMIEK)
	{
		try
		{
			BinaryWriter binaryWriter = new BinaryWriter(ABJIEFMMIEK);
			string text = string.Format("{0} {1} HTTP/1.1", MethodNames[(uint)JCHNIGKBBMI()], (!AOPIGGFCGHC() || !FHGBKFBCGCO().EGNDGIEKOGA()) ? DKAECMGPGOE().PathAndQuery : DKAECMGPGOE().OriginalString);
			if (HTTPManager.MBBMPNDDPIH().PINDEKDNCNL() <= BFNKPHDJNII.Information)
			{
				HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("HTTPRequest", string.Format("Sending request: {0}", text));
			}
			binaryWriter.Write(text.GetASCIIBytes());
			binaryWriter.Write(HGBANJPCEPF);
			SendHeaders(binaryWriter);
			binaryWriter.Write(HGBANJPCEPF);
			binaryWriter.Flush();
			byte[] array = BEKEFBNFBFJ();
			if (array == null && DNDHNALKCFF != null)
			{
				array = DNDHNALKCFF.GDENFGNLFKL();
			}
			if (array == null && IHMCGKHBLKN() == null)
			{
				return;
			}
			Stream stream = IHMCGKHBLKN();
			if (stream == null)
			{
				stream = new MemoryStream(array, 0, array.Length);
				HOOAAHPEACM(array.Length);
			}
			else
			{
				HOOAAHPEACM((!NFDILFHDGAB()) ? (-1) : ABNBBLEAKAP());
			}
			DPGCNHGFLBC(0L);
			byte[] array2 = new byte[INCPJJNNLAH];
			int num = 0;
			while ((num = stream.Read(array2, 0, array2.Length)) > 0)
			{
				if (!NFDILFHDGAB())
				{
					binaryWriter.Write(num.ToString("X").GetASCIIBytes());
					binaryWriter.Write(HGBANJPCEPF);
				}
				binaryWriter.Write(array2, 0, num);
				if (!NFDILFHDGAB())
				{
					binaryWriter.Write(HGBANJPCEPF);
				}
				binaryWriter.Flush();
				DPGCNHGFLBC(MJBCCNEIBDA() + num);
				MBNHNNCHJAG(true);
			}
			if (!NFDILFHDGAB())
			{
				binaryWriter.Write("0".GetASCIIBytes());
				binaryWriter.Write(HGBANJPCEPF);
				binaryWriter.Write(HGBANJPCEPF);
			}
			binaryWriter.Flush();
			if (IHMCGKHBLKN() == null && stream != null)
			{
				stream.Dispose();
			}
		}
		catch (Exception ex)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPRequest", "SendOutTo", ex);
			throw ex;
		}
		finally
		{
			if (IHMCGKHBLKN() != null && PGBKODJJGGF())
			{
				IHMCGKHBLKN().Dispose();
			}
		}
	}

	internal void PPNNNGLBPFD()
	{
		if (POGDKNCHIBG() == null || !POGDKNCHIBG().ODOHODEENIB())
		{
			return;
		}
		try
		{
			if (GFFABFBMJAO != null)
			{
				GFFABFBMJAO(this, POGDKNCHIBG());
			}
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPRequest", "UpgradeCallback", mPFFFAOGBJE);
		}
	}

	internal void FLNDBIJDGMH()
	{
		try
		{
			if (FOIICPPDENH() != null)
			{
				FOIICPPDENH()(this, POGDKNCHIBG());
			}
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("HTTPRequest", "CallCallback", mPFFFAOGBJE);
		}
	}

	internal bool CallOnBeforeRedirection(Uri JJCEFGDNEEO)
	{
		if (IGFALGIFOAH != null)
		{
			return IGFALGIFOAH(this, POGDKNCHIBG(), JJCEFGDNEEO);
		}
		return true;
	}

	internal void NOEMFDALAGD()
	{
		if (POGDKNCHIBG() != null && MDEPOKKKKCL())
		{
			POGDKNCHIBG().NOEMFDALAGD();
		}
	}

	internal void NDNOFGKMHDG()
	{
		if (CCJNBGLGAAM() == AIEMPPBDGNH.Unity)
		{
			NGNCMKLECFI();
		}
	}

	internal bool IMMANGELKAN(X509Certificate DBCFDLIJOBD, X509Chain GCONPBMJDFL)
	{
		if (CustomCertificationValidator != null)
		{
			return CustomCertificationValidator(this, DBCFDLIJOBD, GCONPBMJDFL);
		}
		return true;
	}

	public HTTPRequest Send()
	{
		return HTTPManager.EMPGOCGHMBI(this);
	}

	public void AKLEEMEHBIC()
	{
		lock (HTTPManager.Locker)
		{
			if (FLBBFDNHJAJ() >= CFGBMHKCENK.Finished)
			{
				HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("HTTPRequest", string.Format("Abort - Already in a state({0}) where no Abort required!", FLBBFDNHJAJ().ToString()));
				return;
			}
			HTTPConnection hPNEPPBEKGG = HTTPManager.IBOHPADLFIM(this);
			if (hPNEPPBEKGG == null)
			{
				if (!HTTPManager.HHHKPIJIAPK(this))
				{
					HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("HTTPRequest", "Abort - No active connection found with this request! (The request may already finished?)");
				}
				set_State(CFGBMHKCENK.Aborted);
			}
			else
			{
				if (POGDKNCHIBG() != null && POGDKNCHIBG().HOKPOJABMPK())
				{
					POGDKNCHIBG().Dispose();
				}
				hPNEPPBEKGG.AKLEEMEHBIC(AHFEJIOPFGP.AbortRequested);
			}
		}
	}

	public void Clear()
	{
		PMCMNAIFGJA();
		FCOCENOLBEB();
	}

	public object Current
	{
		get
		{
			return this;
		}
	}

	public bool MoveNext()
	{
		return FLBBFDNHJAJ() < CFGBMHKCENK.Finished;
	}

	public void Reset()
	{
		throw new NotImplementedException();
	}

	private HTTPRequest System_002ECollections_002EGeneric_002EIEnumerator_003CBestHTTP_002EHTTPRequest_003E_002Eget_Current()
	{
		return this;
	}

	public void Dispose()
	{
	}
}
