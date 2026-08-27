using System.Diagnostics;
using System.Xml;

public class PerkActionClearAction : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string BODLHPJCLIG;

	public string AOOJOKOHAHA
	{
		get
		{
			return DDBPICENEJE();
		}
		protected set
		{
			set_NameAction(value);
		}
	}

	public PerkActionClearAction()
	{
	}

	public PerkActionClearAction(PerkActionClearAction NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_NameAction(NOLFMPDGCOC.DDBPICENEJE());
	}

	public string DDBPICENEJE()
	{
		return BODLHPJCLIG;
	}

	protected void set_NameAction(string value)
	{
		BODLHPJCLIG = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_CLEAR_ACTION);
		set_NameAction(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
	}
}
