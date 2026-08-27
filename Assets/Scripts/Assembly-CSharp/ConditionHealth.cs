using System.Xml;

public class ConditionHealth : ConditionAnimation
{
	public float HIKKOHGMFDO;

	public float IJEKNNPOBJD;

	public ConditionHealth(XmlNode node)
		: base(DGAGKLODADD.HEALTH)
	{
		HIKKOHGMFDO = node.Attributes["Min"].ParseFloat();
		IJEKNNPOBJD = node.Attributes["Max"].ParseFloat();
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		float num = conditions.BFLPOMAHPJD / conditions.KGCJIBCACBH;
		bool flag = HIKKOHGMFDO <= num && num <= IJEKNNPOBJD;
		return (!IsNot) ? flag : (!flag);
	}
}
