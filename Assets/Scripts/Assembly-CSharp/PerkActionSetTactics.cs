using System.Diagnostics;
using System.Xml;

public class PerkActionSetTactics : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string PLGMJAFEELF;

	public string DFJMOIDKKOB
	{
		get
		{
			return NLCLHLIPFFH();
		}
		protected set
		{
			set_Tactics(value);
		}
	}

	public PerkActionSetTactics()
	{
	}

	public PerkActionSetTactics(PerkActionSetTactics NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Tactics(NOLFMPDGCOC.NLCLHLIPFFH());
	}

	public string NLCLHLIPFFH()
	{
		return PLGMJAFEELF;
	}

	protected void set_Tactics(string value)
	{
		PLGMJAFEELF = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_TACTICS);
		set_Tactics(node.Attributes["Name"].CIPOICEEIBK(string.Empty));
	}
}
