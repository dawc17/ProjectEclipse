using System.Xml;

public class ConditionBattle : ConditionCounter
{
	private string _type;

	private BattleType JGEBALAKCIJ;

	public ConditionBattle(XmlNode node)
		: base(FELOFIAKFCO.BATTLE)
	{
		Parse(node);
	}

	public override bool IsEqual(CounterConditions conditions)
	{
		bool dCJLKCFKCOM = conditions.BattleType == JGEBALAKCIJ;
		return IsNotCompare(dCJLKCFKCOM);
	}

	public override void AEPHNNABOEK()
	{
		JGEBALAKCIJ = ListSF.ELEBLBJKDBI().HIDKFHHJBDH(_type);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
		_type = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
	}
}
