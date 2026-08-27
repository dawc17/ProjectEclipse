using System.Diagnostics;
using System.Xml;

public class PerkActionProvoke : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string OOJBEPKKFEC;

	public string FEDHCBGNJIM
	{
		get
		{
			return FFLBCPJJKEJ();
		}
		protected set
		{
			set_Trigger(value);
		}
	}

	public PerkActionProvoke()
	{
	}

	public PerkActionProvoke(PerkActionProvoke NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Trigger(NOLFMPDGCOC.FFLBCPJJKEJ());
	}

	public string FFLBCPJJKEJ()
	{
		return OOJBEPKKFEC;
	}

	protected void set_Trigger(string value)
	{
		OOJBEPKKFEC = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_PROVOKE);
		set_Trigger(node.Attributes["Trigger"].CIPOICEEIBK(string.Empty));
	}
}
