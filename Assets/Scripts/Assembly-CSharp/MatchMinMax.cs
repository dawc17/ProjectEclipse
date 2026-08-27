using System.Diagnostics;
using System.Xml;

public class MatchMinMax
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float ADKLBHJHNHH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float PEBIPGIDELJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CKMPECJMKFP;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool CHMPMCGENFK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension OPNEPGNBKPC;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension IKMOJIGKDNJ;

	public float DPGMCKCDMBC
	{
		get
		{
			return PPCEOKCAEBD();
		}
		set
		{
			KPPNJLNHGME(value);
		}
	}

	public float EBDBPJNBHGI
	{
		get
		{
			return EFDLCJBJNPE();
		}
		set
		{
			BIPMDHGOMBG(value);
		}
	}

	public bool ABMGNIIELAL
	{
		get
		{
			return KEMLMMPIPGJ();
		}
		set
		{
			DGGMKIGCGLI(value);
		}
	}

	public bool AFGKKJHBEKM
	{
		get
		{
			return HFGENILMBKK();
		}
		set
		{
			ENIOHPINGMP(value);
		}
	}

	public FunctionExtension HHHDNKDGMMI
	{
		get
		{
			return CIAFOALFPKM();
		}
		protected set
		{
			KHAKPIDDGDC(value);
		}
	}

	public FunctionExtension GMKLGOJMMHD
	{
		get
		{
			return GOEMPDFINCL();
		}
		protected set
		{
			COADNHJEPHE(value);
		}
	}

	public MatchMinMax()
	{
		KPPNJLNHGME(0f);
		BIPMDHGOMBG(0f);
		DGGMKIGCGLI(true);
		ENIOHPINGMP(true);
		KHAKPIDDGDC(new FunctionExtension());
		COADNHJEPHE(new FunctionExtension());
	}

	public float PPCEOKCAEBD()
	{
		return ADKLBHJHNHH;
	}

	public void KPPNJLNHGME(float value)
	{
		ADKLBHJHNHH = value;
	}

	public float EFDLCJBJNPE()
	{
		return PEBIPGIDELJ;
	}

	public void BIPMDHGOMBG(float value)
	{
		PEBIPGIDELJ = value;
	}

	public bool KEMLMMPIPGJ()
	{
		return CKMPECJMKFP;
	}

	public void DGGMKIGCGLI(bool value)
	{
		CKMPECJMKFP = value;
	}

	public bool HFGENILMBKK()
	{
		return CHMPMCGENFK;
	}

	public void ENIOHPINGMP(bool value)
	{
		CHMPMCGENFK = value;
	}

	public FunctionExtension CIAFOALFPKM()
	{
		return OPNEPGNBKPC;
	}

	protected void KHAKPIDDGDC(FunctionExtension value)
	{
		OPNEPGNBKPC = value;
	}

	public FunctionExtension GOEMPDFINCL()
	{
		return IKMOJIGKDNJ;
	}

	protected void COADNHJEPHE(FunctionExtension value)
	{
		IKMOJIGKDNJ = value;
	}

	public void Parse(XmlNode node, PerkCondition IOFGGOCEIAM, PerkInfoItem IOHONODPIIO)
	{
		XmlAttribute cJBEMNNNHDM = node.Attributes["Min"];
		if (!cJBEMNNNHDM.Empty())
		{
			string bLLCOEAOJGF = cJBEMNNNHDM.CIPOICEEIBK(string.Empty);
			CIAFOALFPKM().Parse(bLLCOEAOJGF);
			DGGMKIGCGLI(false);
		}
		XmlAttribute cJBEMNNNHDM2 = node.Attributes["Max"];
		if (!cJBEMNNNHDM2.Empty())
		{
			string bLLCOEAOJGF2 = cJBEMNNNHDM2.CIPOICEEIBK(string.Empty);
			GOEMPDFINCL().Parse(bLLCOEAOJGF2);
			ENIOHPINGMP(false);
		}
		CIAFOALFPKM().PBPBNENGLPA(IOHONODPIIO.HJFEFJIEINN);
		CIAFOALFPKM().DMPCFMACDJM(IOHONODPIIO.OKPFNCJFLDL);
		CIAFOALFPKM().set_Target(IOFGGOCEIAM);
		GOEMPDFINCL().PBPBNENGLPA(IOHONODPIIO.HJFEFJIEINN);
		GOEMPDFINCL().DMPCFMACDJM(IOHONODPIIO.OKPFNCJFLDL);
		GOEMPDFINCL().set_Target(IOFGGOCEIAM);
	}

	public void IBCPKBBAFNH()
	{
		FunctionResult dEIHAOLOPLC = CIAFOALFPKM().IBCPKBBAFNH();
		FunctionResult dEIHAOLOPLC2 = GOEMPDFINCL().IBCPKBBAFNH();
		KPPNJLNHGME(dEIHAOLOPLC.DCJLKCFKCOM.ToFloat());
		BIPMDHGOMBG(dEIHAOLOPLC2.DCJLKCFKCOM.ToFloat());
	}
}
