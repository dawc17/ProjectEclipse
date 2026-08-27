using System.Xml;

public class ActionTryOnEnd : ActionAnimation
{
	public ActionTryOnEnd(XmlNode node)
		: base(FADAJCEEKIO.DELETE)
	{
		Parse(node);
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
	}
}
