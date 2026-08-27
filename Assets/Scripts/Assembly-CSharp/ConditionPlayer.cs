using System.Xml;

public class ConditionPlayer : ConditionAnimation
{
	private bool APFFAGBOCAP;

	public ConditionPlayer(XmlNode node)
		: base(DGAGKLODADD.PLAYER)
	{
		APFFAGBOCAP = node.Attributes["Number"].ParseInt(1) == 1;
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = APFFAGBOCAP == conditions.IsPlayer;
		return (!IsNot) ? flag : (!flag);
	}
}
