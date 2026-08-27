using System.Xml;

public class ConditionBullets : ConditionAnimation
{
	private BulletType KCIIELDOBOM;

	private int KOOPPGNGIFM;

	private int BCMMPCOHJNF;

	public ConditionBullets(XmlNode node)
		: base(DGAGKLODADD.BULLETS)
	{
		string text = node.Attributes["Type"].CIPOICEEIBK(string.Empty);
		if (text == "MagicBullet")
		{
			KCIIELDOBOM = BulletType.MAGIC_BULLET;
		}
		else if (text == "RaidChargeBullet")
		{
			KCIIELDOBOM = BulletType.RAID_CHARGE_BULLET;
		}
		else
		{
			LLLOJBFMONN.Error("ERROR: Unknown bulletType");
		}
		KOOPPGNGIFM = node.Attributes["Min"].ParseInt();
		BCMMPCOHJNF = node.Attributes["Max"].ParseInt(int.MaxValue);
		if ((KCIIELDOBOM == BulletType.MAGIC_BULLET || KCIIELDOBOM == BulletType.RAID_CHARGE_BULLET) && GameUtils.GLHMHHIADMK)
		{
			KOOPPGNGIFM = 0;
		}
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		int num = 0;
		switch (KCIIELDOBOM)
		{
		case BulletType.MAGIC_BULLET:
			num = conditions.JJDNDOLCMMN;
			break;
		case BulletType.RAID_CHARGE_BULLET:
			num = conditions.KHDBLNPFDPE;
			break;
		default:
			LLLOJBFMONN.Error("Strange type condition bullet");
			break;
		}
		bool flag = KOOPPGNGIFM <= num && num <= BCMMPCOHJNF;
		return (!IsNot) ? flag : (!flag);
	}
}
