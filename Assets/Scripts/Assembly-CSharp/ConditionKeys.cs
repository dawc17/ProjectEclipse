using System.Xml;

public class ConditionKeys : ConditionAnimation
{
	// best guess for name
	public KeyData RequiredKeys = new KeyData();

	public KeyData GNNEIPGALBE;

	public ConditionKeys(XmlNode node)
		: base(ConditionType.KEYS)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string bAINMLLIKOL = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
			FightCID item = (FightCID)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.KEY_TYPE, bAINMLLIKOL);
			switch (childNode.Attributes["PressType"].CIPOICEEIBK(string.Empty))
			{
			case "Hold":
				RequiredKeys.CEPODJDDLBF.Add((int)item);
				break;
			case "Tap":
				RequiredKeys.IGEEOAGOMEM.Add((int)item);
				break;
			case "Release":
				RequiredKeys.HPEOJLAMIHC.Add((int)item);
				break;
			}
		}
		RequiredKeys.ResetPressType();
		GNNEIPGALBE = new KeyData(RequiredKeys);
		GNNEIPGALBE.Reverse(-1);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		KeyData oHGJEGDLEJK = ((!conditions.BJACLIMKPAE.IsInverted && conditions.PCAOCHAIBJC <= 0) ? GNNEIPGALBE : RequiredKeys);
		bool flag = !conditions.IDCHHGHAENM || oHGJEGDLEJK.IsVariable(conditions.BJACLIMKPAE);
		return (!IsNot) ? flag : (!flag);
	}

	public bool IsEqual(KeyData KDKEJHHKCDB, bool ANCFHGGJOJB)
	{
		bool flag = !ANCFHGGJOJB || RequiredKeys.IsVariable(KDKEJHHKCDB);
		return (!IsNot) ? flag : (!flag);
	}

	public bool HasSameKeyRequirementAs(ConditionKeys other)
	{
		return other != null && IsNot == other.IsNot && TargetModelType == other.TargetModelType &&
			RequiredKeys.IsInverted == other.RequiredKeys.IsInverted &&
			RequiredKeys.IsVariable(other.RequiredKeys) && other.RequiredKeys.IsVariable(RequiredKeys);
	}
}
