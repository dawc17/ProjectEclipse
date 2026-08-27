using System.Xml;

public class ConditionRound : ConditionAnimation
{
	private string KECJIFJANEO;

	public ConditionRound(XmlNode node)
		: base(DGAGKLODADD.ROUND)
	{
		KECJIFJANEO = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = false;
		if ((conditions.JMHJDHLBHLK == 1 && KECJIFJANEO == "StartStance") || (conditions.JMHJDHLBHLK == 2 && KECJIFJANEO == "Fight") || (conditions.JMHJDHLBHLK == 3 && KECJIFJANEO == "EndStance") || (conditions.JMHJDHLBHLK == 7 && KECJIFJANEO == "TryOn"))
		{
			flag = true;
		}
		return (!IsNot) ? flag : (!flag);
	}
}
