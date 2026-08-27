using System.Xml;

public class ActionStopFollowEffect : ActionAnimation
{
	private string _Name;

	public ActionStopFollowEffect(XmlNode node)
		: base(FADAJCEEKIO.STOP_FOLLOW_EFFECT)
	{
		Parse(node);
	}

	public string get_Name()
	{
		return _Name;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}
}
