using System.Collections.Generic;
using System.Xml;

public class ConditionInterval : ConditionAnimation
{
	private IntervalAnimation.NGAJJDIEDGF KCIIELDOBOM;

	private string _Name;

	public ConditionInterval(XmlNode node)
		: base(DGAGKLODADD.CURRENT_INTERVAL)
	{
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		if (node.Attributes["Type"] != null)
		{
			switch (node.Attributes["Type"].CIPOICEEIBK(string.Empty))
			{
			case "Attack":
				KCIIELDOBOM = IntervalAnimation.NGAJJDIEDGF.INTERVAL_ATTACK;
				break;
			case "Block":
				KCIIELDOBOM = IntervalAnimation.NGAJJDIEDGF.INTERVAL_BLOCK;
				break;
			case "Invulnerable":
				KCIIELDOBOM = IntervalAnimation.NGAJJDIEDGF.INTERVAL_INVULNERABLE;
				break;
			}
		}
		else
		{
			KCIIELDOBOM = IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE;
		}
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = false;
		List<IntervalAnimation> cAANBJEPGAA = conditions.Intervals;
		if (cAANBJEPGAA != null)
		{
			foreach (IntervalAnimation item in cAANBJEPGAA)
			{
				if ((KCIIELDOBOM == IntervalAnimation.NGAJJDIEDGF.INTERVAL_NONE || KCIIELDOBOM == item.Type) && (_Name == string.Empty || item.Name == _Name))
				{
					flag = true;
					break;
				}
			}
		}
		return (!IsNot) ? flag : (!flag);
	}

	private List<IntervalAnimation> GetIntervals(ModelConditions conditions)
	{
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return conditions.Intervals;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			return conditions.FJFOIEFFMEM;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return conditions.JLCFPNDDGCJ;
		default:
			LLLOJBFMONN.Error("ConditionCurrentAnimation: getAnimationNames - wrong type: {0}", OOFFOILONLO.ToString());
			return conditions.Intervals;
		}
	}
}
