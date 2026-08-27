using System.Collections.Generic;
using System.Xml;

public class ConditionCurrentAnimation : ConditionAnimation
{
	private string _Name;

	private bool HNBFHJHDCCC;

	private bool KMKJFLHJDAF;

	public ConditionCurrentAnimation(XmlNode node)
		: base(DGAGKLODADD.CURRENT_ANIMATION)
	{
		if (node.Attributes["Name"] != null)
		{
			_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
			HNBFHJHDCCC = _Name == "$NoAnimation$";
		}
		KMKJFLHJDAF = node.Attributes["Physics"].ParseBool();
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(_Name))
		{
			List<string> list = LKALMFBALCN(conditions);
			if (!("$Move" == _Name))
			{
				flag = ((!HNBFHJHDCCC) ? IsNames(_Name, list) : (list.Count == 0));
			}
			else if (0 < list.Count)
			{
				flag = IsNames(list[0], conditions.PDKPGKPBBIL);
			}
		}
		else
		{
			flag = FEMJOBECCKD(conditions);
			flag = flag == KMKJFLHJDAF;
		}
		return (!IsNot) ? flag : (!flag);
	}

	private static bool IsNames(List<string> NIKHAICFGNM, List<string> MGNOPLPBOHC)
	{
		foreach (string item in NIKHAICFGNM)
		{
			foreach (string item2 in MGNOPLPBOHC)
			{
				if (item == item2)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsNames(string name, List<string> MGNOPLPBOHC)
	{
		foreach (string item in MGNOPLPBOHC)
		{
			if (name == item)
			{
				return true;
			}
		}
		return false;
	}

	private List<string> LKALMFBALCN(ModelConditions conditions)
	{
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return conditions.NNPJJLPCOHD;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			return conditions.MGFNFEHILNF;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return conditions.DHHADKMMOHP;
		case ModelType.KEIDBIOIFGA.MODEL_CHILD:
			return conditions.NKPMIACBKDE;
		default:
			LLLOJBFMONN.Error("ConditionCurrentAnimation: getAnimationNames - wrong type: {0}", OOFFOILONLO);
			return conditions.NNPJJLPCOHD;
		}
	}

	private bool FEMJOBECCKD(ModelConditions conditions)
	{
		switch (OOFFOILONLO)
		{
		case ModelType.KEIDBIOIFGA.MODEL_THIS:
			return conditions.NCBPMBJCFBK;
		case ModelType.KEIDBIOIFGA.MODEL_OTHER:
			return conditions.EKFCILFBDPO;
		case ModelType.KEIDBIOIFGA.MODEL_PARENT:
			return conditions.LFLDHGKEDEH;
		default:
			LLLOJBFMONN.Error("ConditionCurrentAnimation: getAnimationNames - wrong type: %i", OOFFOILONLO);
			return conditions.NCBPMBJCFBK;
		}
	}
}
