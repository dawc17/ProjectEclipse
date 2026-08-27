using System.Xml;

public class ActionSetCooldown : ActionAnimation
{
	private int _Duration;

	private string _ButtonName;

	public int KFNNOPBIOFK
	{
		get
		{
			return OMIEPGOPPBO();
		}
	}

	public string NOIAMHIBHDL
	{
		get
		{
			return GHHAKGGLBCN();
		}
	}

	public ActionSetCooldown(XmlNode node)
		: base(FADAJCEEKIO.SET_COOLDOWN)
	{
		Parse(node);
	}

	public int OMIEPGOPPBO()
	{
		return _Duration;
	}

	public string GHHAKGGLBCN()
	{
		return _ButtonName;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_Duration = node.Attributes["Duration"].ParseInt();
		_ButtonName = node.Attributes["Button"].CIPOICEEIBK(string.Empty);
	}
}
