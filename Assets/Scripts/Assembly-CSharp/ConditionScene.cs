using System.Xml;

public class ConditionScene : ConditionAnimation
{
	private SceneTypes KCIIELDOBOM;

	private string _Name;

	public ConditionScene(XmlNode node)
		: base(DGAGKLODADD.SCREEN)
	{
		_Name = node.Attributes["Name"].CIPOICEEIBK(string.Empty);
		KCIIELDOBOM = get_Type();
	}

	public SceneTypes get_Type()
	{
		if (_Name == "ShopArmor")
		{
			return SceneTypes.SceneShopArmor;
		}
		if (_Name == "ShopWeapon")
		{
			return SceneTypes.SceneShopWeapon;
		}
		if (_Name == "ShopHelm")
		{
			return SceneTypes.SceneShopHelm;
		}
		if (_Name == "ShopMissile")
		{
			return SceneTypes.SceneShopMissile;
		}
		if (_Name == "ShopMagic")
		{
			return SceneTypes.SceneShopMagic;
		}
		if (_Name == "ShopRuby")
		{
			return SceneTypes.SceneShopRuby;
		}
		if (_Name == "ShopFree")
		{
			return SceneTypes.SceneShopFree;
		}
		if (_Name == "ShopRaidItemPack")
		{
			return SceneTypes.SceneShopRaidItemPack;
		}
		if (_Name == "Profile")
		{
			return SceneTypes.SceneProfile;
		}
		if (_Name == "Fight")
		{
			return SceneTypes.SceneFight;
		}
		return SceneTypes.SceneNone;
	}

	public string get_Name()
	{
		return _Name;
	}

	public override bool IsEqual(ModelConditions conditions)
	{
		bool flag = conditions.IBBALIJOJMC == KCIIELDOBOM;
		return (!IsNot) ? flag : (!flag);
	}
}
