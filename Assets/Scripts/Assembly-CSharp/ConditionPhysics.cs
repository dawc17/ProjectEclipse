using System.Xml;

public class ConditionPhysics : ConditionAnimation
{
	private float DPGMCKCDMBC;

	private float EBDBPJNBHGI;

	public ConditionPhysics(XmlNode node)
		: base(DGAGKLODADD.PHYSICS_FRAME)
	{
		DPGMCKCDMBC = node.Attributes["Min"].ParseFloat(-1f);
		EBDBPJNBHGI = node.Attributes["Max"].ParseFloat(-1f);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = false;
		if ((DPGMCKCDMBC == -1f || (float)conditions.KAKMANLHJOA >= DPGMCKCDMBC) && (EBDBPJNBHGI == -1f || (float)conditions.KAKMANLHJOA <= EBDBPJNBHGI))
		{
			flag = true;
		}
		return (!IsNot) ? flag : (!flag);
	}
}
