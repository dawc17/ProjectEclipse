using System.Xml;

public class ActionAddBullets : ActionAnimation
{
	private BulletType KCIIELDOBOM;

	private int _Value;

	public BulletType HJEILDDNNCJ
	{
		get
		{
			return AOLGKCANKLL();
		}
	}

	public int Value
	{
		get
		{
			return OEAKCOHMIHH();
		}
	}

	public ActionAddBullets(XmlNode node)
		: base(FADAJCEEKIO.ADD_BULLETS)
	{
		Parse(node);
	}

	public BulletType AOLGKCANKLL()
	{
		return KCIIELDOBOM;
	}

	public int OEAKCOHMIHH()
	{
		return _Value;
	}

	public override void Visit(Model ACENLMONNPA)
	{
		ACENLMONNPA.OPPIKLBKMPN(this);
	}

	protected override void Parse(XmlNode node)
	{
		base.Parse(node);
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
		_Value = node.Attributes["Value"].ParseInt();
		if ((KCIIELDOBOM == BulletType.MAGIC_BULLET || KCIIELDOBOM == BulletType.RAID_CHARGE_BULLET) && GameUtils.GLHMHHIADMK)
		{
			_Value = 0;
		}
	}
}
