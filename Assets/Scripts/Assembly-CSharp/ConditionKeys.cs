using System.Xml;

public class ConditionKeys : ConditionAnimation
{
	public KeyData FONEJOKEIEN = new KeyData();

	public KeyData GNNEIPGALBE;

	public ConditionKeys(XmlNode node)
		: base(DGAGKLODADD.KEYS)
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			string bAINMLLIKOL = childNode.Attributes["Type"].CIPOICEEIBK(string.Empty);
			FightCID item = (FightCID)MovesMaps.HHBMBMNLJIE(MovesMaps.NHKAHBBOIHG.KEY_TYPE, bAINMLLIKOL);
			switch (childNode.Attributes["PressType"].CIPOICEEIBK(string.Empty))
			{
			case "Hold":
				FONEJOKEIEN.CEPODJDDLBF.Add((int)item);
				break;
			case "Tap":
				FONEJOKEIEN.IGEEOAGOMEM.Add((int)item);
				break;
			case "Release":
				FONEJOKEIEN.HPEOJLAMIHC.Add((int)item);
				break;
			}
		}
		FONEJOKEIEN.ResetPressType();
		GNNEIPGALBE = new KeyData(FONEJOKEIEN);
		GNNEIPGALBE.Reverse(-1);
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		KeyData oHGJEGDLEJK = ((!conditions.BJACLIMKPAE.IsInverted && conditions.PCAOCHAIBJC <= 0) ? GNNEIPGALBE : FONEJOKEIEN);
		bool flag = !conditions.IDCHHGHAENM || oHGJEGDLEJK.IsVariable(conditions.BJACLIMKPAE);
		return (!IsNot) ? flag : (!flag);
	}

	public bool IsEqual(KeyData KDKEJHHKCDB, bool ANCFHGGJOJB)
	{
		bool flag = !ANCFHGGJOJB || FONEJOKEIEN.IsVariable(KDKEJHHKCDB);
		return (!IsNot) ? flag : (!flag);
	}
}
