using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using CodeStage.AntiCheat.ObscuredTypes;
using Nekki.Utils;

public class Recipe
{
	private string _name = string.Empty;
	private string _alias = string.Empty;
	private bool _isFree;
	private readonly List<RecipeItem> _items = new List<RecipeItem>();
	private readonly List<RecipePrices> _prices = new List<RecipePrices>();
	private readonly List<Variation> _variations = new List<Variation>();

	public string Name => _name;
	public string Alias => _alias;
	public bool IsFree { get => _isFree; set => _isFree = value; }
	public IReadOnlyList<RecipeItem> Items => _items.AsReadOnly();
	public IReadOnlyList<RecipePrices> Prices => _prices.AsReadOnly();
	public IReadOnlyList<Variation> Variations => _variations.AsReadOnly();

	public Recipe()
	{
	}

	public Recipe(XmlNode node)
	{
		if (node == null) return;
		_name = Attr(node, "Name");
		_alias = Attr(node, "Alias");
		XmlNode items = node["Items"];
		if (items != null)
			foreach (XmlNode item in items.ChildNodes)
				if (item.NodeType == XmlNodeType.Element && item.Name == "Item") _items.Add(new RecipeItem(item));
		XmlNode prices = node["Prices"];
		if (prices != null)
			foreach (XmlNode block in prices.ChildNodes)
				if (block.NodeType == XmlNodeType.Element) _prices.Add(new RecipePrices(block));
		XmlNode variations = node["Variations"];
		if (variations != null)
			foreach (XmlNode variation in variations.ChildNodes)
				if (variation.NodeType == XmlNodeType.Element && variation.Name == "Variation") _variations.Add(new Variation(variation));
	}

	public string get_Name() => _name;

	public RecipePrice GetPriceByItem(UserItem userItem)
	{
		if (userItem == null) return null;
		ItemInfo info = CurrentInfo(userItem);
		return info == null ? null : GetPriceByItemLevel(userItem, info.MHGODOLNDLE);
	}

	public RecipePrice GetPriceByItemLevel(UserItem userItem, int itemLevel)
	{
		RecipeItem recipeItem = GetRecipeItemByItem(userItem);
		if (recipeItem == null) return null;
		RecipePrices prices = GetRecipePricesByName(recipeItem.PricesBlockName);
		return prices?.GetPriceByLevel(itemLevel);
	}

	public RecipeItem GetRecipeItemByItem(UserItem userItem)
	{
		ItemInfo info = CurrentInfo(userItem);
		if (info == null) return null;
		for (int i = 0; i < _items.Count; i++)
			if (string.Equals(_items[i].ItemType, info.Type, StringComparison.Ordinal)) return _items[i];
		return null;
	}

	private RecipePrices GetRecipePricesByName(string name)
	{
		for (int i = 0; i < _prices.Count; i++)
			if (string.Equals(_prices[i].Name, name, StringComparison.Ordinal)) return _prices[i];
		return null;
	}

	public bool IsRecipeAvailableForItem(UserItem userItem)
	{
		if (userItem == null || userItem.OFOPFCJNEBL() <= 0 || userItem.PHDBCIHJKON() != null) return false;
		ItemInfo info = CurrentInfo(userItem);
		return info != null && IsRecipeAvailableForItemType(info.Type) && GetPriceByItem(userItem) != null &&
			IsRecipeWillEnchantItem(userItem, info.MHGODOLNDLE);
	}

	public bool IsRecipeAvailableForItemType(string itemType)
	{
		for (int i = 0; i < _items.Count; i++)
			if (string.Equals(_items[i].ItemType, itemType, StringComparison.Ordinal)) return true;
		return false;
	}

	public bool IsRecipeWillEnchantItem(UserItem userItem, int itemLevel)
	{
		int required = GetRequiredEnchantmentsByItem(userItem);
		return required > 0 && GetPossibleEnchantments(userItem, itemLevel, true).Count >= required;
	}

	// Recovered callers use this method as the "Available" and materials gate.
	public bool IHHJGMBGHEB(UserItem userItem)
	{
		return IsRecipeAvailableForItem(userItem) && CheckMaterialsForItem(userItem);
	}

	public bool CheckMaterialsForItem(UserItem userItem)
	{
		if (_isFree) return true;
		RecipePrice price = GetPriceByItem(userItem);
		Roster roster = ListSF.CCDKHLAMKKO();
		if (price == null || roster == null) return false;
		foreach (CurrencyStruct material in price.Materials)
		{
			if (material?.BKDEAGGPNAO == null) return false;
			if (roster.GetCurrencyCount(material.BKDEAGGPNAO) < material.Count) return false;
		}
		return true;
	}

	public List<PerkStruct> GetPossibleEnchantments(UserItem userItem, int itemLevel, bool checkRequired = true)
	{
		var result = new List<PerkStruct>();
		if (userItem == null) return result;
		for (int i = 0; i < _variations.Count; i++)
		{
			Variation variation = _variations[i];
			if (!variation.CheckConditions(userItem, itemLevel)) continue;
			foreach (PerkStruct enchantment in variation.Enchantments)
			{
				if (enchantment == null || !IsPerkReadyToEnchant(enchantment)) continue;
				if (checkRequired && IsEnchantmentAlreadyExists(enchantment, userItem.JAJNJAIJOPA)) continue;
				result.Add(new PerkStruct(enchantment));
			}
		}
		return result;
	}

	public bool PossibleEnchantmentsHasComboPerk(UserItem userItem, int itemLevel)
	{
		foreach (PerkStruct perk in GetPossibleEnchantments(userItem, itemLevel, false))
		{
			PerkInfoItem info = GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(perk.get_Name());
			if (info != null && info.LELHEEDNMBP == PerkInfoItem.DNPGIEGCGKH.COMBO) return true;
		}
		return false;
	}

	public int GetReadyEnchantments(UserItem userItem, int itemLevel)
	{
		return GetPossibleEnchantments(userItem, itemLevel, true).Count;
	}

	public string GetPerksInfo(UserItem userItem, int itemLevel)
	{
		var names = new List<string>();
		foreach (PerkStruct perk in GetPossibleEnchantments(userItem, itemLevel, false)) names.Add(perk.get_Name());
		return string.Join(", ", names.ToArray());
	}

	public int GetRequiredEnchantmentsByItem(UserItem userItem)
	{
		RecipeItem item = GetRecipeItemByItem(userItem);
		return item == null ? 0 : item.EnchantmentsNumber;
	}

	public List<PerkStruct> GetEnchantmentsForItem(UserItem userItem, int itemLevel)
	{
		List<PerkStruct> possible = GetPossibleEnchantments(userItem, itemLevel, true);
		int required = GetRequiredEnchantmentsByItem(userItem);
		var result = new List<PerkStruct>();
		while (result.Count < required && possible.Count > 0)
		{
			int index = NekkiMath.randomInt(possible.Count);
			result.Add(possible[index]);
			possible.RemoveAt(index);
		}
		return result.Count == required ? result : new List<PerkStruct>();
	}

	private static bool IsPerkReadyToEnchant(PerkStruct enchantment)
	{
		return enchantment != null && GameUtils.FDEJIIDIPBI != null &&
			GameUtils.FDEJIIDIPBI.ABAGJKMKCBA(enchantment.get_Name()) != null;
	}

	private static bool IsEnchantmentAlreadyExists(PerkStruct enchantment, List<PerkInfoItem> enchantments)
	{
		if (enchantment == null || enchantments == null) return false;
		for (int i = 0; i < enchantments.Count; i++)
			if (enchantments[i] != null && string.Equals(enchantments[i].Name, enchantment.get_Name(), StringComparison.Ordinal))
				return true;
		return false;
	}

	private static ItemInfo CurrentInfo(UserItem userItem)
	{
		if (userItem == null) return null;
		ItemInfo info = userItem.DBLCMCEGJGI(false);
		return info ?? userItem.BHKHOJPANHE();
	}

	internal static string Attr(XmlNode node, string name, string fallback = "")
	{
		XmlAttribute attribute = node?.Attributes?[name];
		return attribute == null ? fallback : attribute.Value;
	}

	internal static int IntAttr(XmlNode node, string name, int fallback = 0)
	{
		int value;
		return int.TryParse(Attr(node, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : fallback;
	}
}

public sealed class RecipeItem
{
	public string ItemType { get; }
	public string PricesBlockName { get; }
	public int EnchantmentsNumber { get; }
	public string BarScale { get; }
	public int MinDeviation { get; }
	public int MaxDeviation { get; }

	public RecipeItem(XmlNode node)
	{
		ItemType = Recipe.Attr(node, "Type");
		PricesBlockName = Recipe.Attr(node, "Prices");
		EnchantmentsNumber = Recipe.IntAttr(node, "Enchantments", 1);
		BarScale = Recipe.Attr(node, "BarScale");
		MinDeviation = Recipe.IntAttr(node, "MinDeviation");
		MaxDeviation = Recipe.IntAttr(node, "MaxDeviation");
	}
}

public sealed class RecipePrices
{
	private readonly List<RecipePrice> _prices = new List<RecipePrice>();
	public string Name { get; }
	public List<RecipePrice> Prices => _prices;

	public RecipePrices(XmlNode node)
	{
		Name = Recipe.Attr(node, "Name");
		if (node == null) return;
		foreach (XmlNode price in node.ChildNodes)
			if (price.NodeType == XmlNodeType.Element && price.Name == "Price") _prices.Add(new RecipePrice(price));
	}

	public RecipePrice GetPriceByLevel(UserItem userItem)
	{
		if (userItem == null) return null;
		ItemInfo info = userItem.DBLCMCEGJGI(false) ?? userItem.BHKHOJPANHE();
		return info == null ? null : GetPriceByLevel(info.MHGODOLNDLE);
	}

	public RecipePrice GetPriceByLevel(int level)
	{
		for (int i = 0; i < _prices.Count; i++) if (_prices[i].IsAvailableForLevel(level)) return _prices[i];
		return null;
	}
}

public sealed class Variation
{
	private readonly List<VariationCondition> _conditions = new List<VariationCondition>();
	private readonly List<PerkStruct> _enchantments = new List<PerkStruct>();
	public List<VariationCondition> Conditions => _conditions;
	public List<PerkStruct> Enchantments => _enchantments;

	public Variation(XmlNode node)
	{
		XmlNode conditions = node?["Conditions"];
		if (conditions != null)
			foreach (XmlNode conditionNode in conditions.ChildNodes)
			{
				if (conditionNode.NodeType != XmlNodeType.Element) continue;
				VariationCondition condition = VariationCondition.Create(conditionNode);
				if (condition != null) _conditions.Add(condition);
			}
		XmlNode enchantments = node?["Enchantments"];
		if (enchantments != null)
			foreach (XmlNode perk in enchantments.ChildNodes)
				if (perk.NodeType == XmlNodeType.Element && perk.Name == "Perk") _enchantments.Add(new PerkStruct(perk));
	}

	public bool CheckConditions(UserItem userItem, int itemLevel)
	{
		for (int i = 0; i < _conditions.Count; i++) if (!_conditions[i].Check(userItem, itemLevel)) return false;
		return true;
	}
}

public enum VariationConditionType
{
	None = 0,
	Item = 1,
	Level = 2,
	Operator = 3
}

public abstract class VariationCondition
{
	public VariationConditionType Type { get; }
	protected VariationCondition(VariationConditionType type) { Type = type; }
	public abstract bool Check(UserItem userItem, int itemLevel);

	public static VariationCondition Create(XmlNode node)
	{
		if (node == null) return null;
		switch (node.Name)
		{
			case "Item": return new VariationConditionItem(node);
			case "Level": return new VariationConditionLevel(node);
			case "Or": return new VariationConditionOperator(node, false);
			case "And": return new VariationConditionOperator(node, true);
			default: return null;
		}
	}
}

public sealed class VariationConditionItem : VariationCondition
{
	private readonly string _itemType;
	internal VariationConditionItem(XmlNode node) : base(VariationConditionType.Item)
	{
		_itemType = Recipe.Attr(node, "Type");
	}
	public override bool Check(UserItem userItem, int itemLevel)
	{
		ItemInfo info = userItem == null ? null : (userItem.DBLCMCEGJGI(false) ?? userItem.BHKHOJPANHE());
		return info != null && string.Equals(info.Type, _itemType, StringComparison.Ordinal);
	}
}

public sealed class VariationConditionLevel : VariationCondition
{
	private readonly int _minLevel;
	private readonly int _maxLevel;
	internal VariationConditionLevel(XmlNode node) : base(VariationConditionType.Level)
	{
		int exact = Recipe.IntAttr(node, "Level", int.MinValue);
		if (exact != int.MinValue) { _minLevel = exact; _maxLevel = exact; }
		else
		{
			_minLevel = Recipe.IntAttr(node, "MinLevel", int.MinValue);
			_maxLevel = Recipe.IntAttr(node, "MaxLevel", int.MaxValue);
		}
	}
	public override bool Check(UserItem userItem, int itemLevel) => itemLevel >= _minLevel && itemLevel <= _maxLevel;
}

public sealed class VariationConditionOperator : VariationCondition
{
	private readonly bool _and;
	private readonly List<VariationCondition> _conditions = new List<VariationCondition>();
	internal VariationConditionOperator(XmlNode node, bool and) : base(VariationConditionType.Operator)
	{
		_and = and;
		foreach (XmlNode child in node.ChildNodes)
		{
			if (child.NodeType != XmlNodeType.Element) continue;
			VariationCondition condition = Create(child);
			if (condition != null) _conditions.Add(condition);
		}
	}
	public override bool Check(UserItem userItem, int itemLevel)
	{
		if (_conditions.Count == 0) return true;
		if (_and)
		{
			for (int i = 0; i < _conditions.Count; i++) if (!_conditions[i].Check(userItem, itemLevel)) return false;
			return true;
		}
		for (int i = 0; i < _conditions.Count; i++) if (_conditions[i].Check(userItem, itemLevel)) return true;
		return false;
	}
}
