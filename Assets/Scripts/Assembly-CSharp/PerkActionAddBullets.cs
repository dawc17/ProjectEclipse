using System.Diagnostics;
using System.Xml;

public class PerkActionAddBullets : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string ABMCCMHGHAB;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension IELPCLONGKP;

	public PerkActionAddBullets()
	{
	}

	public PerkActionAddBullets(PerkActionAddBullets NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_BulletType(NOLFMPDGCOC.MPGDOMBCAAF());
		set_Value(NOLFMPDGCOC.OEAKCOHMIHH());
	}

	public string MPGDOMBCAAF()
	{
		return ABMCCMHGHAB;
	}

	protected void set_BulletType(string value)
	{
		ABMCCMHGHAB = value;
	}

	public FunctionExtension OEAKCOHMIHH()
	{
		return IELPCLONGKP;
	}

	protected void set_Value(FunctionExtension value)
	{
		IELPCLONGKP = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_ADD_BULLETS);
		set_BulletType(node.Attributes["BulletType"].CIPOICEEIBK(string.Empty));
		string text = node.Attributes["Value"].CIPOICEEIBK(string.Empty);
		if (text != null && text != string.Empty)
		{
			set_Value(new FunctionExtension());
			OEAKCOHMIHH().Parse(text);
			OEAKCOHMIHH().PBPBNENGLPA(JMDLAMHAJLN().HJFEFJIEINN);
			OEAKCOHMIHH().DMPCFMACDJM(JMDLAMHAJLN().OKPFNCJFLDL);
			OEAKCOHMIHH().set_Target(this);
		}
	}
}
