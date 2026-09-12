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
	private sealed class ExternalCandidate
	{
		public PerkStruct Perk;
		public int MinLevel;
		public int MaxLevel;
		public bool Contains(int level) => level >= MinLevel && level <= MaxLevel;
	}

	private readonly Dictionary<string, List<ExternalCandidate>> _externalEnchantments =
		new Dictionary<string, List<ExternalCandidate>>(StringComparer.Ordinal);
	private readonly Dictionary<string, HashSet<string>> _excludedNativeCandidates =
		new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
	private readonly Dictionary<string, DeviationOverride> _deviationOverrides =
		new Dictionary<string, DeviationOverride>(StringComparer.Ordinal);

	private sealed class DeviationOverride : IDisposable
	{
		private Recipe _recipe;
		public readonly RecipeItem Item;
		public DeviationOverride(Recipe recipe, RecipeItem item) { _recipe = recipe; Item = item; }
		public void Dispose()
		{
			Recipe recipe = _recipe;
			if (recipe == null) return;
			_recipe = null;
			if (recipe._deviationOverrides.TryGetValue(Item.ItemType, out var current) && ReferenceEquals(current, this))
				recipe._deviationOverrides.Remove(Item.ItemType);
		}
	}

	internal bool TryOverrideDeviation(string itemType, int minimum, int maximum, out IDisposable lifetime)
	{
		lifetime = null;
		if (string.IsNullOrEmpty(itemType) || minimum < -10000 || maximum > 10000 || minimum > maximum ||
			_deviationOverrides.ContainsKey(itemType)) return false;
		RecipeItem original = GetRecipeItemByType(itemType);
		if (original == null || !original.RandomAspect) return false;
		var item = new RecipeItem(original.ItemType, original.PricesBlockName, original.EnchantmentsNumber,
			original.BarScale, minimum, maximum, true);
		var replacement = new DeviationOverride(this, item);
		_deviationOverrides.Add(itemType, replacement);
		lifetime = replacement;
		return true;
	}

	private PerkStruct CopyCandidateForItem(PerkStruct source, string itemType)
	{
		var copy = new PerkStruct(source);
		if (itemType == null || !_deviationOverrides.TryGetValue(itemType, out var replacement)) return copy;
		for (int i = 0; i < copy.Pairs.Count; i++)
		{
			var pair = copy.Pairs[i];
			// Only replace a complete native random-aspect call. Fixed values and compound expressions retain their meaning.
			if (pair.Key != "Aspect" || pair.Value == null || pair.Value.Length > 128 ||
				!System.Text.RegularExpressions.Regex.IsMatch(pair.Value, @"\A\?RandomAspect\[-?\d+,-?\d+\]\z")) continue;
			copy.Pairs[i] = new KeyValuePair<string, string>(pair.Key, "?RandomAspect[" +
				replacement.Item.MinDeviation.ToString(CultureInfo.InvariantCulture) + "," +
				replacement.Item.MaxDeviation.ToString(CultureInfo.InvariantCulture) + "]");
		}
		return copy;
	}

	private sealed class NativeCandidateExclusion : IDisposable
	{
		private Recipe _recipe;
		private readonly string _itemType, _perkName;
		public NativeCandidateExclusion(Recipe recipe, string itemType, string perkName)
		{ _recipe = recipe; _itemType = itemType; _perkName = perkName; }
		public void Dispose()
		{
			Recipe recipe = _recipe;
			if (recipe == null) return;
			_recipe = null;
			if (!recipe._excludedNativeCandidates.TryGetValue(_itemType, out var excluded)) return;
			excluded.Remove(_perkName);
			if (excluded.Count == 0) recipe._excludedNativeCandidates.Remove(_itemType);
		}
	}

	internal bool TryExcludeNativeCandidate(string itemType, string perkName, out IDisposable lifetime)
	{
		lifetime = null;
		if (string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(perkName) || GetRecipeItemByType(itemType) == null)
			return false;
		bool exists = false;
		foreach (var variation in _variations)
			foreach (var candidate in variation.Enchantments)
				if (candidate != null && string.Equals(candidate.get_Name(), perkName, StringComparison.Ordinal)) exists = true;
		if (!exists) return false;
		if (!_excludedNativeCandidates.TryGetValue(itemType, out var excluded))
		{
			excluded = new HashSet<string>(StringComparer.Ordinal);
			_excludedNativeCandidates.Add(itemType, excluded);
		}
		if (!excluded.Add(perkName)) return false;
		lifetime = new NativeCandidateExclusion(this, itemType, perkName);
		return true;
	}

	private bool IsNativeCandidateExcluded(string itemType, string perkName)
	{
		return itemType != null && _excludedNativeCandidates.TryGetValue(itemType, out var excluded) && excluded.Contains(perkName);
	}

	public string Name => _name;
	public string Alias => _alias;
	public bool IsFree { get => _isFree; set => _isFree = value; }
	public IReadOnlyList<RecipeItem> Items
	{
		get
		{
			if (_deviationOverrides.Count == 0) return _items.AsReadOnly();
			var items = new List<RecipeItem>(_items.Count);
			foreach (var item in _items)
				items.Add(_deviationOverrides.TryGetValue(item.ItemType, out var replacement) ? replacement.Item : item);
			return items.AsReadOnly();
		}
	}
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
		return GetRecipeItemByType(info.Type);
	}

	internal Recipe(string name, string alias, Recipe economicProfile, IReadOnlyList<ExternalRecipeItemSpec> items)
	{
		if (string.IsNullOrEmpty(name)) throw new ArgumentException("Recipe name must not be empty.", "name");
		if (economicProfile == null) throw new ArgumentNullException("economicProfile");
		if (items == null || items.Count == 0) throw new ArgumentException("Recipe requires at least one item type.", "items");
		_name = name;
		_alias = alias ?? string.Empty;
		for (int i = 0; i < items.Count; i++)
		{
			ExternalRecipeItemSpec spec = items[i];
			RecipeItem economicItem = economicProfile.GetRecipeItemByType(spec.ItemType);
			if (economicItem == null)
				throw new InvalidOperationException("Economic profile '" + economicProfile.Name +
					"' has no price binding for item type '" + spec.ItemType + "'.");
			_items.Add(new RecipeItem(spec.ItemType, economicItem.PricesBlockName, spec.EnchantmentsNumber,
				spec.BarScale, spec.MinDeviation, spec.MaxDeviation, spec.RandomAspect));
		}
		// These objects are intentionally shared with the immutable host profile. External recipe
		// families can select a host-owned economic profile, but can never author or mutate its values.
		_prices.AddRange(economicProfile._prices);
	}

	public bool AddExternalEnchantmentCandidate(string itemType, string perkName, string enchantmentId, string perkKind,
		IReadOnlyDictionary<string, string> eclipseParameters = null)
	{
		return AddExternalCandidate(itemType, perkName, enchantmentId, perkKind, int.MinValue, int.MaxValue,
			eclipseParameters);
	}

	public bool AddExternalPerkCandidate(string itemType, string perkName, string perkKind,
		int minLevel = int.MinValue, int maxLevel = int.MaxValue)
	{
		return AddExternalCandidate(itemType, perkName, null, perkKind, minLevel, maxLevel, null);
	}

	private bool AddExternalCandidate(string itemType, string perkName, string enchantmentId, string perkKind,
		int minLevel, int maxLevel, IReadOnlyDictionary<string, string> eclipseParameters)
	{
		if (string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(perkName) || minLevel > maxLevel)
			return false;
		if (!string.Equals(perkKind, "Single", StringComparison.Ordinal) &&
			!string.Equals(perkKind, "Combo", StringComparison.Ordinal)) return false;
		RecipeItem recipeItem = GetRecipeItemByType(itemType);
		if (recipeItem == null) return false;

		List<ExternalCandidate> candidates;
		if (!_externalEnchantments.TryGetValue(itemType, out candidates))
		{
			candidates = new List<ExternalCandidate>();
			_externalEnchantments.Add(itemType, candidates);
		}
		for (int i = 0; i < candidates.Count; i++)
			if (string.Equals(candidates[i].Perk.get_Name(), perkName, StringComparison.Ordinal)) return false;

		var document = new XmlDocument();
		XmlElement perk = document.CreateElement("Perk");
		perk.SetAttribute("Name", perkName);
		perk.SetAttribute("ItemType", itemType);
		if (!string.IsNullOrEmpty(enchantmentId))
			perk.SetAttribute(PerkStruct.EclipseEnchantmentAttribute, enchantmentId);
		perk.SetAttribute(PerkStruct.EclipseKindAttribute, perkKind);
		if (eclipseParameters != null && eclipseParameters.Count > 0)
		{
			XmlElement parameters = document.CreateElement(Eclipse.Modding.ModEffectSaveData.NodeName);
			parameters.SetAttribute("Format", Eclipse.Modding.ModEffectSaveData.Format);
			foreach (KeyValuePair<string, string> pair in eclipseParameters)
			{
				XmlElement parameter = document.CreateElement(Eclipse.Modding.ModEffectSaveData.ParameterNodeName);
				parameter.SetAttribute("Name", pair.Key);
				parameter.SetAttribute("Value", pair.Value ?? string.Empty);
				parameters.AppendChild(parameter);
			}
			perk.AppendChild(parameters);
		}
			if (UsesRandomAspectForExternalCandidates(itemType))
		{
			XmlElement set = document.CreateElement("Set");
			set.SetAttribute("Aspect", "?RandomAspect[" +
				recipeItem.MinDeviation.ToString(CultureInfo.InvariantCulture) + "," +
				recipeItem.MaxDeviation.ToString(CultureInfo.InvariantCulture) + "]");
			perk.AppendChild(set);
		}
		document.AppendChild(perk);
		candidates.Add(new ExternalCandidate { Perk = new PerkStruct(perk), MinLevel = minLevel, MaxLevel = maxLevel });
		return true;
	}

	public bool RemoveExternalEnchantmentCandidate(string itemType, string perkName)
	{
		if (string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(perkName)) return false;
		List<ExternalCandidate> candidates;
		if (!_externalEnchantments.TryGetValue(itemType, out candidates)) return false;
		for (int i = 0; i < candidates.Count; i++)
		{
			if (!string.Equals(candidates[i].Perk.get_Name(), perkName, StringComparison.Ordinal)) continue;
			candidates.RemoveAt(i);
			if (candidates.Count == 0) _externalEnchantments.Remove(itemType);
			return true;
		}
		return false;
	}

	private RecipeItem GetRecipeItemByType(string itemType)
	{
		if (itemType != null && _deviationOverrides.TryGetValue(itemType, out var replacement)) return replacement.Item;
		for (int i = 0; i < _items.Count; i++)
			if (string.Equals(_items[i].ItemType, itemType, StringComparison.Ordinal)) return _items[i];
		return null;
	}

	private bool UsesRandomAspectForExternalCandidates(string itemType)
	{
		RecipeItem item = GetRecipeItemByType(itemType);
		return item != null && item.RandomAspect;
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
		ItemInfo info = CurrentInfo(userItem);
			for (int i = 0; i < _variations.Count; i++)
			{
				Variation variation = _variations[i];
				if (!variation.CheckConditions(userItem, itemLevel)) continue;
				foreach (PerkStruct enchantment in variation.Enchantments)
				{
					if (enchantment != null && IsNativeCandidateExcluded(info?.Type, enchantment.get_Name())) continue;
					if (enchantment == null || !IsPerkReadyToEnchant(enchantment)) continue;
					if (checkRequired && IsEnchantmentAlreadyExists(enchantment, userItem.JAJNJAIJOPA)) continue;
					result.Add(CopyCandidateForItem(enchantment, info?.Type));
				}
			}

				List<ExternalCandidate> external;
		if (info != null && _externalEnchantments.TryGetValue(info.Type, out external))
		{
			for (int i = 0; i < external.Count; i++)
			{
					ExternalCandidate candidate = external[i];
					if (!candidate.Contains(itemLevel)) continue;
					PerkStruct enchantment = candidate.Perk;
				if (enchantment == null || !IsPerkReadyToEnchant(enchantment)) continue;
				if (checkRequired && IsEnchantmentAlreadyExists(enchantment, userItem.JAJNJAIJOPA)) continue;
				result.Add(CopyCandidateForItem(enchantment, info.Type));
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
	public bool RandomAspect { get; }

	public RecipeItem(XmlNode node)
	{
		ItemType = Recipe.Attr(node, "Type");
		PricesBlockName = Recipe.Attr(node, "Prices");
		EnchantmentsNumber = Recipe.IntAttr(node, "Enchantments", 1);
		BarScale = Recipe.Attr(node, "BarScale");
		MinDeviation = Recipe.IntAttr(node, "MinDeviation");
		MaxDeviation = Recipe.IntAttr(node, "MaxDeviation");
		RandomAspect = node?.Attributes?["MinDeviation"] != null || node?.Attributes?["MaxDeviation"] != null;
	}

	internal RecipeItem(string itemType, string pricesBlockName, int enchantmentsNumber, string barScale,
		int minDeviation, int maxDeviation, bool randomAspect)
	{
		ItemType = itemType ?? string.Empty;
		PricesBlockName = pricesBlockName ?? string.Empty;
		EnchantmentsNumber = enchantmentsNumber;
		BarScale = barScale ?? string.Empty;
		MinDeviation = minDeviation;
		MaxDeviation = maxDeviation;
		RandomAspect = randomAspect;
	}
}

public sealed class ExternalRecipeItemSpec
{
	public string ItemType { get; }
	public int EnchantmentsNumber { get; }
	public string BarScale { get; }
	public int MinDeviation { get; }
	public int MaxDeviation { get; }
	public bool RandomAspect { get; }

	public ExternalRecipeItemSpec(string itemType, int enchantmentsNumber, string barScale,
		int minDeviation, int maxDeviation, bool randomAspect)
	{
		ItemType = itemType ?? string.Empty;
		EnchantmentsNumber = enchantmentsNumber;
		BarScale = barScale ?? string.Empty;
		MinDeviation = minDeviation;
		MaxDeviation = maxDeviation;
		RandomAspect = randomAspect;
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
