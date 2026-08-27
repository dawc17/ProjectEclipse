using System.Diagnostics;
using System.Xml;

public class PerkActionDisableInterval : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string KHKLMPFJEJJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HIDMKDOBNDF;

	public string CCFKABDNCLA
	{
		get
		{
			return BIIIIDOCMEK();
		}
		protected set
		{
			NINADDOJPAA(value);
		}
	}

	public string MOLEHILDAGP
	{
		get
		{
			return KFDPPOKFMPI();
		}
		protected set
		{
			HPPDPOEEPEP(value);
		}
	}

	public PerkActionDisableInterval()
	{
	}

	public PerkActionDisableInterval(PerkActionDisableInterval NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		NINADDOJPAA(NOLFMPDGCOC.BIIIIDOCMEK());
		HPPDPOEEPEP(NOLFMPDGCOC.KFDPPOKFMPI());
	}

	public string BIIIIDOCMEK()
	{
		return KHKLMPFJEJJ;
	}

	protected void NINADDOJPAA(string value)
	{
		KHKLMPFJEJJ = value;
	}

	public string KFDPPOKFMPI()
	{
		return HIDMKDOBNDF;
	}

	protected void HPPDPOEEPEP(string value)
	{
		HIDMKDOBNDF = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_DISABLE_INTERVAL);
		NINADDOJPAA(node.Attributes["IntervalName"].CIPOICEEIBK(string.Empty));
		HPPDPOEEPEP(node.Attributes["IntervalType"].CIPOICEEIBK(string.Empty));
	}
}
