using System.Diagnostics;
using System.Xml;

public class PerkActionSetModFrames : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OPAFELFOFFB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension DEDEBCBLNBJ;

	public string POLPHCDNLEL
	{
		get
		{
			return CMKKGFDBBJF();
		}
		protected set
		{
			set_ModName(value);
		}
	}

	public FunctionExtension KIEAMMONFOJ
	{
		get
		{
			return NFPODDJPNEL();
		}
		protected set
		{
			CKIALPMBKGN(value);
		}
	}

	public PerkActionSetModFrames()
	{
	}

	public PerkActionSetModFrames(PerkActionSetModFrames NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_ModName(NOLFMPDGCOC.CMKKGFDBBJF());
		CKIALPMBKGN(NOLFMPDGCOC.NFPODDJPNEL());
	}

	public string CMKKGFDBBJF()
	{
		return OPAFELFOFFB;
	}

	protected void set_ModName(string value)
	{
		OPAFELFOFFB = value;
	}

	public FunctionExtension NFPODDJPNEL()
	{
		return DEDEBCBLNBJ;
	}

	protected void CKIALPMBKGN(FunctionExtension value)
	{
		DEDEBCBLNBJ = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_MOD_FRAMES);
		set_ModName(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
		CKIALPMBKGN(null);
		string text = node.Attributes["Frames"].CIPOICEEIBK(string.Empty);
		if (text != null && text != string.Empty)
		{
			CKIALPMBKGN(new FunctionExtension());
			NFPODDJPNEL().Parse(text);
			NFPODDJPNEL().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			NFPODDJPNEL().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			NFPODDJPNEL().set_Target(this);
		}
	}
}
