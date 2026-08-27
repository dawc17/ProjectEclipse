using System.Diagnostics;
using System.Xml;

public class PerkActionVariable : PerkActionModificator
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FunctionExtension IELPCLONGKP;

	public PerkActionVariable()
	{
	}

	public PerkActionVariable(PerkActionVariable NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Value(NOLFMPDGCOC.OEAKCOHMIHH());
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
		set_Type(ActionType.ACTION_VARIABLE);
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
