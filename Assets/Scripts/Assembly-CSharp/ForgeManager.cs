using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Content;
using UnityEngine;

public class ForgeManager : global::EventDispatcher<object>
{
	private sealed class ForgeAspect
	{
		public int MinLevel = int.MinValue;
		public int MaxLevel = int.MaxValue;
		public int Value;

		public ForgeAspect(XmlNode node)
		{
			int exact = Recipe.IntAttr(node, "Level", int.MinValue);
			if (exact != int.MinValue)
			{
				MinLevel = exact;
				MaxLevel = exact;
			}
			else
			{
				MinLevel = Recipe.IntAttr(node, "MinLevel", int.MinValue);
				MaxLevel = Recipe.IntAttr(node, "MaxLevel", int.MaxValue);
			}
			Value = Recipe.IntAttr(node, "Value");
		}

		public bool Contains(int level) => level >= MinLevel && level <= MaxLevel;
	}

	private static ForgeManager _instance;
	private static Action<UserItem> _onItemEnchanted;
	private readonly List<ForgeAspect> _aspects = new List<ForgeAspect>();
	private readonly List<Recipe> _recipes = new List<Recipe>();
	private bool _parsed;
	private int _aspectLevelOverride = -1;

	public static event Action<UserItem> OnItemEnchanted
	{
		add { _onItemEnchanted += value; }
		remove { _onItemEnchanted -= value; }
	}

	public IReadOnlyList<Recipe> Recipes
	{
		get
		{
			EnsureParsed();
			return _recipes.AsReadOnly();
		}
	}

	public static ForgeManager BPCBBHAKFDM => ELEBLBJKDBI();

	public static ForgeManager ELEBLBJKDBI()
	{
		if (_instance == null) _instance = new ForgeManager();
		return _instance;
	}

	public static void Reset()
	{
		_instance = null;
		_onItemEnchanted = null;
	}

	public void Parse()
	{
		string path = Path.Combine(GameplayContentArchive.GetXmlRoot(), "forge.xml");
		var document = new XmlDocument();
		document.Load(path);
		XmlElement root = document["Forge"];
		if (root == null) throw new InvalidDataException("forge.xml is missing the Forge root node: " + path);

		_aspects.Clear();
		_recipes.Clear();
		XmlNode aspectScale = root["AspectScale"];
		if (aspectScale != null)
			foreach (XmlNode node in aspectScale.ChildNodes)
				if (node.NodeType == XmlNodeType.Element && node.Name == "Aspect") _aspects.Add(new ForgeAspect(node));
		XmlNode recipes = root["Recipes"];
		if (recipes != null)
			foreach (XmlNode node in recipes.ChildNodes)
				if (node.NodeType == XmlNodeType.Element && node.Name == "Recipe") _recipes.Add(new Recipe(node));

		if (_aspects.Count == 0 || _recipes.Count == 0)
			throw new InvalidDataException("forge.xml does not contain aspect scale and recipe data: " + path);
		_parsed = true;
		Debug.Log("[Forge] Loaded " + _recipes.Count + " recipes and " + _aspects.Count + " aspect ranges from " + path);
	}

	public RecipePrice FIGKJLNILIN(string itemName, string recipeName)
	{
		Recipe recipe = GetRecipeByName(recipeName);
		Roster roster = ListSF.CCDKHLAMKKO();
		UserItem userItem = roster?.KHCNHPCPFII()?.CMGOCLGHNLH(itemName);
		return recipe?.GetPriceByItem(userItem);
	}

	public Recipe GetRecipeByName(string name)
	{
		EnsureParsed();
		for (int i = 0; i < _recipes.Count; i++)
			if (string.Equals(_recipes[i].Name, name, StringComparison.OrdinalIgnoreCase)) return _recipes[i];
		return null;
	}

	public List<Recipe> GetAvailableRecipesForItem(UserItem userItem)
	{
		EnsureParsed();
		var result = new List<Recipe>();
		for (int i = 0; i < _recipes.Count; i++)
			if (_recipes[i].IsRecipeAvailableForItem(userItem)) result.Add(_recipes[i]);
		return result;
	}

	public bool IsAvailableRecipesForItemType(string itemType)
	{
		EnsureParsed();
		for (int i = 0; i < _recipes.Count; i++) if (_recipes[i].IsRecipeAvailableForItemType(itemType)) return true;
		return false;
	}

	public RecipeItemInfo GetRecipeItemInfo(string name)
	{
		if (string.IsNullOrEmpty(name)) return null;
		int separator = name.LastIndexOf('|');
		if (separator <= 0 || separator >= name.Length - 1) return null;
		string itemName = name.Substring(0, separator);
		string recipeName = name.Substring(separator + 1);
		Recipe recipe = GetRecipeByName(recipeName);
		UserItem userItem = ListSF.CCDKHLAMKKO()?.KHCNHPCPFII()?.CMGOCLGHNLH(itemName);
		RecipePrice price = recipe?.GetPriceByItem(userItem);
		return recipe == null || userItem == null || price == null ? null : new RecipeItemInfo(recipe, userItem, price);
	}

	public int GetAspectValueByLevel(int playerLevel)
	{
		EnsureParsed();
		for (int i = 0; i < _aspects.Count; i++) if (_aspects[i].Contains(playerLevel)) return _aspects[i].Value;
		return 0;
	}

	internal int ResolveAspectLevel(int currentPlayerLevel)
	{
		return _aspectLevelOverride >= 0 ? _aspectLevelOverride : currentPlayerLevel;
	}

	public bool CheckAvailableEnchantments(UserItem userItem, Recipe recipe, int itemLevel, int playerLevel)
	{
		return userItem != null && recipe != null && recipe.IsRecipeWillEnchantItem(userItem, itemLevel);
	}

	public bool EnchantItem(RecipeItemInfo recipeItem)
	{
		return recipeItem != null && EnchantItem(recipeItem.MFEAIEJFDAM(), recipeItem.OIMGNCLBPHD(),
			recipeItem.ItemLevel, recipeItem.PlayerLevel);
	}

	public bool EnchantItem(UserItem userItem, Recipe recipe, int itemLevel, int playerLevel)
	{
		if (userItem == null || recipe == null) return false;
		List<PerkStruct> enchantments = recipe.GetEnchantmentsForItem(userItem, itemLevel);
		if (enchantments.Count == 0) return false;

		int previousOverride = _aspectLevelOverride;
		_aspectLevelOverride = playerLevel;
		try
		{
			// This recovered method owns the original replacement rule and serializes
			// the resulting <Enchantments> subtree into the existing UserItem node.
			userItem.GDBFNNLHPOB(enchantments, itemLevel, playerLevel);
		}
		finally
		{
			_aspectLevelOverride = previousOverride;
		}
		NotifyItemEnchanted(userItem, recipe);
		return true;
	}

	public bool StartEnchant(UserItem userItem, Recipe recipe, out RecipeItemInfo recipeItem)
	{
		recipeItem = null;
		if (userItem == null || recipe == null || !recipe.IsRecipeAvailableForItem(userItem) ||
			!recipe.CheckMaterialsForItem(userItem)) return false;
		RecipePrice price = recipe.GetPriceByItem(userItem);
		if (price == null) return false;
		recipeItem = new RecipeItemInfo(recipe, userItem, price);

		Roster roster = ListSF.CCDKHLAMKKO();
		bool free = recipe.IsFree;
		if (!free && !DeductMaterials(roster, price))
		{
			recipeItem = null;
			return false;
		}

		bool success = false;
		try
		{
			if (price.DeliveryTime <= 0)
				success = EnchantItem(recipeItem);
			else
				success = userItem.SetRecipeDelivery(recipeItem);
			if (!success) throw new InvalidOperationException("Unable to start forge recipe '" + recipe.Name + "'.");
			if (free) recipe.IsFree = false;
			roster?.GGGEHAGCLGC(true);
			return true;
		}
		catch (Exception exception)
		{
			if (!free) RefundMaterials(roster, price);
			Debug.LogError("[Forge] Failed to start enchantment; materials were restored. " + exception);
			recipeItem = null;
			return false;
		}
	}

	public bool FinishEnchant(RecipeItemInfo recipeItem)
	{
		if (recipeItem == null || recipeItem.MFEAIEJFDAM() == null) return false;
		UserItem userItem = recipeItem.MFEAIEJFDAM();
		bool success = EnchantItem(recipeItem);
		if (success)
		{
			userItem.ClearRecipeDelivery();
			ListSF.CCDKHLAMKKO()?.GGGEHAGCLGC(true);
		}
		return success;
	}

	private static bool DeductMaterials(Roster roster, RecipePrice price)
	{
		if (roster == null || price == null) return false;
		foreach (CurrencyStruct material in price.Materials)
		{
			if (material?.BKDEAGGPNAO == null || roster.GetCurrencyCount(material.BKDEAGGPNAO) < material.Count)
				return false;
		}
		foreach (CurrencyStruct material in price.Materials)
			roster.AddCurrencyCount(material.BKDEAGGPNAO, -(int)material.Count);
		return true;
	}

	private static void RefundMaterials(Roster roster, RecipePrice price)
	{
		if (roster == null || price == null) return;
		foreach (CurrencyStruct material in price.Materials)
			if (material?.BKDEAGGPNAO != null) roster.AddCurrencyCount(material.BKDEAGGPNAO, (int)material.Count);
	}

	private static void NotifyItemEnchanted(UserItem userItem, Recipe recipe)
	{
		Action<UserItem> callback = _onItemEnchanted;
		if (callback != null) callback(userItem);
		ListSF list = ListSF.BPCBBHAKFDM;
		if (list == null) return;
		QuestParameters parameters = list.BNMLDPNCMLB();
		parameters.DPLEGFCHOCE.OHCGEEEKEJH = userItem.get_Name();
		parameters.DPLEGFCHOCE.FHELNNCGCGC = recipe.Name;
		parameters.DPLEGFCHOCE.BMNFPNBAMAF = 0L;
		parameters.DPLEGFCHOCE.MECEADEKGJB = string.Empty;
		if (list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT)) list.MHHNIPBJNAD();
	}

	private void EnsureParsed()
	{
		if (!_parsed) Parse();
	}
}
