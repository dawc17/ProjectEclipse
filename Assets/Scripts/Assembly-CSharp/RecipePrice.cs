using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;

public class RecipePrice
{
	private int _minLevel = 1;
	private int _maxLevel = int.MaxValue;
	private readonly List<CurrencyStruct> _materials = new List<CurrencyStruct>();

	// Recovered field names kept for existing callers.
	public int EHKNIKHPGDN;
	public ObscuredLong KLHOKKPALOK;

	public int MinLevel => _minLevel;
	public int MaxLevel => _maxLevel;
	public int DeliveryTime => EHKNIKHPGDN;
	public ObscuredLong BonusDeliveryPrice => KLHOKKPALOK;
	public List<CurrencyStruct> Materials => _materials;

	public RecipePrice()
	{
	}

	public RecipePrice(XmlNode node)
	{
		if (node == null) return;

		int exactLevel;
		if (TryInt(node, "Level", out exactLevel))
		{
			_minLevel = exactLevel;
			_maxLevel = exactLevel;
		}
		else
		{
			int value;
			if (TryInt(node, "MinLevel", out value)) _minLevel = value;
			if (TryInt(node, "MaxLevel", out value)) _maxLevel = value;
		}

		int delivery;
		EHKNIKHPGDN = TryInt(node, "DeliveryTime", out delivery) ? delivery : 0;
		long bonus;
		KLHOKKPALOK = (ObscuredLong)(TryLong(node, "BonusDeliveryPrice", out bonus) ? bonus : 0L);

		if (node.Attributes == null) return;
		foreach (XmlAttribute attribute in node.Attributes)
		{
			if (!attribute.Name.StartsWith("ForgeMaterial", System.StringComparison.Ordinal)) continue;
			int count;
			if (!int.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out count) || count <= 0)
				continue;
			GameCurrency currency = GameUtils.AJDKHINLIDI == null
				? null
				: GameUtils.AJDKHINLIDI.ICFINJLNCPM(attribute.Name);
			if (currency == null)
				currency = new GameCurrency(attribute.Name, string.Empty, GameCurrency.DEFOMBPHMBP.CURRENCY_GROUP_FORGE);
			_materials.Add(new CurrencyStruct(currency, count));
		}
	}

	public bool IsAvailableForItem(UserItem userItem)
	{
		if (userItem == null) return false;
		ItemInfo info = userItem.DBLCMCEGJGI(false);
		if (info == null) info = userItem.BHKHOJPANHE();
		return info != null && IsAvailableForLevel(info.MHGODOLNDLE);
	}

	public bool IsAvailableForLevel(int level)
	{
		return level >= _minLevel && level <= _maxLevel;
	}

	private static bool TryInt(XmlNode node, string name, out int value)
	{
		value = 0;
		XmlAttribute attribute = node.Attributes?[name];
		return attribute != null && int.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
	}

	private static bool TryLong(XmlNode node, string name, out long value)
	{
		value = 0L;
		XmlAttribute attribute = node.Attributes?[name];
		return attribute != null && long.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
	}
}
