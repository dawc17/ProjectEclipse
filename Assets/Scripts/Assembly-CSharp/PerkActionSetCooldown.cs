using System.Diagnostics;
using System.Xml;

public class PerkActionSetCooldown : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int PJKLJNAGNCJ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string PLKHBNBNCDD;

	public new int OCFKLCDIEBF
	{
		get
		{
			return BFJEFNHKPJI();
		}
		protected set
		{
			set_Frames(value);
		}
	}

	public string NOIAMHIBHDL
	{
		get
		{
			return GHHAKGGLBCN();
		}
		protected set
		{
			set_ButtonName(value);
		}
	}

	public PerkActionSetCooldown()
	{
	}

	public PerkActionSetCooldown(PerkActionSetCooldown NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_Frames(NOLFMPDGCOC.BFJEFNHKPJI());
		set_ButtonName(NOLFMPDGCOC.GHHAKGGLBCN());
	}

	public new int BFJEFNHKPJI()
	{
		return PJKLJNAGNCJ;
	}

	protected void set_Frames(int value)
	{
		PJKLJNAGNCJ = value;
	}

	public string GHHAKGGLBCN()
	{
		return PLKHBNBNCDD;
	}

	protected void set_ButtonName(string value)
	{
		PLKHBNBNCDD = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_SET_COOLDOWN);
		set_Frames(node.Attributes["Frames"].ParseInt());
		set_ButtonName(node.Attributes["Button"].CIPOICEEIBK(string.Empty));
	}
}
