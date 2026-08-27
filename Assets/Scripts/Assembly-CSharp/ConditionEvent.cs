using System.Xml;

public class ConditionEvent : ConditionAnimation
{
	private EventAnimation MHPOELBJAIJ;

	public ConditionEvent(XmlNode node)
		: base(DGAGKLODADD.EVENT)
	{
		MHPOELBJAIJ = EventParser.Create(node);
		MHPOELBJAIJ.Init(node);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = MHPOELBJAIJ.IsEqual(conditions.HFCIDBJJINB);
		return (!IsNot) ? flag : (!flag);
	}
}
