using System.Diagnostics;
using System.Xml;

public class PerkActionLifesteal : PerkAction
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float GJMNMAJANLP;

	public float CMFGKFFEHCA
	{
		get
		{
			return NIBCOALEIDN();
		}
		protected set
		{
			set_DamagePart(value);
		}
	}

	public PerkActionLifesteal()
	{
	}

	public PerkActionLifesteal(PerkActionLifesteal NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
		set_DamagePart(NOLFMPDGCOC.NIBCOALEIDN());
	}

	public float NIBCOALEIDN()
	{
		return GJMNMAJANLP;
	}

	protected void set_DamagePart(float value)
	{
		GJMNMAJANLP = value;
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_LIFE_STEAL);
		set_DamagePart(node.Attributes["DamagePart"].ParseFloat());
	}
}
