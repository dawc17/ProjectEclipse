using System.Xml;

public class ConditionName : ConditionAnimation
{
	private string _Name;

	public ConditionName(XmlNode node)
		: base(DGAGKLODADD.NAME)
	{
		_Name = node.Attributes["Value"].CIPOICEEIBK(string.Empty);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = conditions.ModelName == _Name;
		return (!IsNot) ? flag : (!flag);
	}
}
