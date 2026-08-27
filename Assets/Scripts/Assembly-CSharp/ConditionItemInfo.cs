using System.Xml;

public class ConditionItemInfo : ConditionAnimation
{
	private string KCIIELDOBOM;

	private string LOKOGOFENFO;

	private string _Name;

	public string MDPPNGIEJGD
	{
		get
		{
			return EAIMKPPOODM();
		}
	}

	public ConditionItemInfo(XmlNode node)
		: base(DGAGKLODADD.ITEM)
	{
		KCIIELDOBOM = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		LOKOGOFENFO = node.Attributes["SubType"].CIPOICEEIBK(string.Empty);
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
	}

	public string get_Type()
	{
		return KCIIELDOBOM;
	}

	public string EAIMKPPOODM()
	{
		return LOKOGOFENFO;
	}

	public string get_Name()
	{
		return _Name;
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		if (conditions == null || conditions.OJIAKDDCGLB == null)
			return IsNot;
		foreach (ItemInfo item in conditions.OJIAKDDCGLB)
		{
			if ((string.IsNullOrEmpty(KCIIELDOBOM) || KCIIELDOBOM == item.Type) && (string.IsNullOrEmpty(LOKOGOFENFO) || LOKOGOFENFO == item.MDPPNGIEJGD) && (string.IsNullOrEmpty(_Name) || _Name == item.Name))
			{
				return !IsNot;
			}
		}
		return IsNot;
	}
}
