using System.Xml;

public class PerkActionFlag : PerkActionModificator
{
	public PerkActionFlag()
	{
	}

	public PerkActionFlag(PerkActionFlag NOLFMPDGCOC)
		: base(NOLFMPDGCOC)
	{
	}

	public override void Parse(XmlNode node)
	{
		base.Parse(node);
		set_Type(ActionType.ACTION_FLAG);
	}
}
