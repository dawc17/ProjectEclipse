using System.Xml;

public class ActionStopEffect : ActionAnimation
{
	private string _Name;

	public ActionStopEffect(XmlNode node)
		: base(FADAJCEEKIO.STOP_EFFECT)
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
