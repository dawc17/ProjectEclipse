using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Eclipse.Modding
{
    public sealed class LegacyContentAdapter : IDisposable
    {
        private readonly ModContentCatalog _content;
        private readonly List<string> _itemNames = new List<string>();
        private readonly List<string> _localizationKeys = new List<string>();
        private readonly List<string> _perkNames = new List<string>();
        private readonly List<ExternalEnchantmentBinding> _enchantmentBindings =
            new List<ExternalEnchantmentBinding>();
        private Items _items;
        private PerkItems _perks;
        private ForgeManager _forge;
        private bool _itemsApplied;
        private bool _perksApplied;
        private bool _languageSubscribed;
        private bool _disposed;

        public LegacyContentAdapter(ModContentCatalog content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            if (!content.IsFrozen)
                throw new InvalidOperationException("Legacy content may only adapt a frozen definition catalog.");
        }

        public void ApplyItems(Items items)
        {
            ThrowIfDisposed();
            if (_itemsApplied) throw new InvalidOperationException("Legacy items are already applied.");
            _items = items ?? throw new ArgumentNullException(nameof(items));

            foreach (ShopListingDefinition listing in _content.ShopListings)
            {
                ItemDefinition definition;
                if (!_content.TryGetItem(listing.Item, out definition))
                    throw new InvalidOperationException("Committed shop listing has no item: " + listing.Item);
                if (_items.KCCDBEEKBCG(definition.Id.ToString()) != null)
                    throw new InvalidOperationException("Legacy item already exists: " + definition.Id);
            }

            try
            {
                foreach (ShopListingDefinition listing in _content.ShopListings)
                {
                    ItemDefinition definition;
                    if (!_content.TryGetItem(listing.Item, out definition)) continue;
                    XmlElement node = BuildItemNode(definition, listing);
                    ItemInfo item = _items.AddExternalItem(node);
                    _itemNames.Add(item.Name);
                }
                _itemsApplied = true;
            }
            catch
            {
                RemoveItems();
                throw;
            }
        }

        public void ApplyLocalization()
        {
            ThrowIfDisposed();
            RemoveLocalization();

            string language = LocalizationManager.ILAJKOBCHFH == null
                ? LocalizationManager.POIPGLLCCKC
                : LocalizationManager.ILAJKOBCHFH.name;
            foreach (LocalizationDefinition localization in _content.Localizations)
            {
                string value = localization.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = localization.Id.ToString();
                LocalizationManager.SetExternalString(key, value);
                _localizationKeys.Add(key);
            }

            // Recovered shop/item UI usually localizes an ItemInfo by ItemInfo.Name rather than
            // by its optional Text/TextButton fields. Keep the canonical namespaced localization
            // definition available, but also publish the display string under the legacy item id.
            foreach (ShopListingDefinition listing in _content.ShopListings)
            {
                ItemDefinition item;
                if (!_content.TryGetItem(listing.Item, out item)) continue;
                LocalizationDefinition displayName;
                if (!_content.TryGetLocalization(item.DisplayName, out displayName)) continue;
                string value = displayName.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = item.Id.ToString();
                LocalizationManager.SetExternalString(key, value);
                _localizationKeys.Add(key);
            }

            if (!_languageSubscribed)
            {
                LocalizationManager.OCLBJLPOKLB += OnLanguageChanged;
                _languageSubscribed = true;
            }
        }

        public void ApplyPerksAndEnchantments(PerkItems perks, ForgeManager forge)
        {
            ThrowIfDisposed();
            if (_perksApplied) throw new InvalidOperationException("Legacy perks are already applied.");
            _perks = perks ?? throw new ArgumentNullException(nameof(perks));
            _forge = forge ?? throw new ArgumentNullException(nameof(forge));

            try
            {
                var visiting = new HashSet<DefinitionId>();
                foreach (PerkDefinition definition in _content.Perks)
                    if (!definition.IsCore) EnsurePerkApplied(definition, visiting);

                foreach (EnchantmentDefinition enchantment in _content.Enchantments)
                {
                    string perkName;
                    string perkKind;
                    IReadOnlyDictionary<string, ModParameterValue> initialParameters = null;
                    if (enchantment.HasPerk)
                    {
                        PerkDefinition perk;
                        if (!_content.TryGetPerk(enchantment.Perk, out perk))
                            throw new InvalidOperationException("Committed enchantment has no perk: " + enchantment.Id);
                        perkName = RuntimePerkName(perk);
                        perkKind = perk.Kind == ModPerkKind.Combo ? "Combo" : "Single";
                    }
                    else if (enchantment.HasBehavior)
                    {
                        EnsureScriptedEnchantmentApplied(enchantment);
                        perkName = enchantment.Id.ToString();
                        perkKind = enchantment.Kind == ModPerkKind.Combo ? "Combo" : "Single";
                        initialParameters = enchantment.InitialParameters;
                    }
                    else
                    {
                        throw new InvalidOperationException("Committed enchantment has no behavior backend: " + enchantment.Id);
                    }
                    string recipeName = RecipeName(enchantment.Recipe);
                    for (int i = 0; i < enchantment.Equipment.Count; i++)
                    {
                        string itemType = EquipmentType(enchantment.Equipment[i]);
                        if (!_forge.AddExternalEnchantmentCandidate(recipeName, itemType, perkName,
                                enchantment.Id.ToString(), perkKind, ToWireParameters(initialParameters)))
                            throw new InvalidOperationException("Could not add external enchantment '" + enchantment.Id +
                                "' to " + recipeName + "/" + itemType + ".");
                        _enchantmentBindings.Add(new ExternalEnchantmentBinding(recipeName, itemType, perkName));
                    }
                }
                _perksApplied = true;
            }
            catch
            {
                RemovePerksAndEnchantments();
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_languageSubscribed)
            {
                LocalizationManager.OCLBJLPOKLB -= OnLanguageChanged;
                _languageSubscribed = false;
            }
            RemoveLocalization();
            RemovePerksAndEnchantments();
            RemoveItems();
            _disposed = true;
        }

        private void OnLanguageChanged()
        {
            if (!_disposed) ApplyLocalization();
        }

        private void RemoveLocalization()
        {
            for (int i = 0; i < _localizationKeys.Count; i++)
                LocalizationManager.RemoveExternalString(_localizationKeys[i]);
            _localizationKeys.Clear();
        }

        private void RemoveItems()
        {
            if (_items != null)
            {
                for (int i = _itemNames.Count - 1; i >= 0; i--)
                    _items.RemoveExternalItem(_itemNames[i]);
            }
            _itemNames.Clear();
            _itemsApplied = false;
        }

        private PerkInfoItem EnsurePerkApplied(PerkDefinition definition, HashSet<DefinitionId> visiting)
        {
            if (definition.IsCore)
            {
                PerkInfoItem core = _perks.ABAGJKMKCBA(definition.LegacyName);
                if (core == null)
                    throw new InvalidOperationException("Core perk template is unavailable at runtime: " + definition.Id);
                return core;
            }

            string runtimeName = RuntimePerkName(definition);
            PerkInfoItem existing = _perks.ABAGJKMKCBA(runtimeName);
            if (existing != null)
            {
                if (_perkNames.Contains(runtimeName)) return existing;
                throw new InvalidOperationException("Legacy perk already exists: " + runtimeName);
            }
            if (definition.HasBehavior)
            {
                XmlElement scriptedNode = BuildScriptedPerkNode(definition.Id, definition.DisplayName,
                    definition.Description, definition.Icon, definition.HasIcon, definition.Kind);
                PerkInfoItem scripted = _perks.AddExternalBasePerk(scriptedNode);
                _perkNames.Add(scripted.Name);
                return scripted;
            }
            if (!definition.HasTemplate)
                throw new InvalidOperationException("External perk has no template: " + definition.Id);
            if (!visiting.Add(definition.Id))
                throw new InvalidOperationException("Perk template cycle detected at " + definition.Id);

            try
            {
                PerkDefinition templateDefinition;
                if (!_content.TryGetPerk(definition.Template, out templateDefinition))
                    throw new InvalidOperationException("Perk template is unavailable: " + definition.Template);
                PerkInfoItem template = EnsurePerkApplied(templateDefinition, visiting);
                XmlElement node = BuildPerkNode(definition, template);
                PerkInfoItem applied = _perks.AddExternalBasePerk(node);
                _perkNames.Add(applied.Name);
                return applied;
            }
            finally
            {
                visiting.Remove(definition.Id);
            }
        }

        private XmlElement BuildPerkNode(PerkDefinition definition, PerkInfoItem template)
        {
            if (template == null || template.HAAKMBKCMCO == null)
                throw new InvalidOperationException("Perk template has no canonical runtime XML: " + definition.Template);
            var document = new XmlDocument();
            XmlElement node = document.ImportNode(template.HAAKMBKCMCO, true) as XmlElement;
            if (node == null) throw new InvalidOperationException("Perk template is not a Perk element: " + definition.Template);
            document.AppendChild(node);

            string templateName = template.Name;
            string inheritedTemplates = node.GetAttribute("Template");
            node.SetAttribute("Name", definition.Id.ToString());
			node.SetAttribute("ID", _perks.CJJEPHDFOCJ().Count.ToString(CultureInfo.InvariantCulture));
			node.SetAttribute("Alias", definition.DisplayName.ToString());
			node.SetAttribute("Description", definition.Description.ToString());
			// Keep the template's logical Image by default. Vanilla intentionally resolves
			// that one value through different shop and fight sprite paths.
			if (definition.HasIcon) node.SetAttribute("Image", definition.Icon.ToString());
			node.SetAttribute("Template", string.IsNullOrEmpty(inheritedTemplates)
                ? templateName : inheritedTemplates + "|" + templateName);

            XmlElement set = node["Set"];
            if (set == null && definition.Parameters.Count > 0)
            {
                set = document.CreateElement("Set");
                XmlNode firstTrigger = node.SelectSingleNode("Trigger");
                if (firstTrigger == null) node.AppendChild(set);
                else node.InsertBefore(set, firstTrigger);
            }
            if (set != null)
                foreach (KeyValuePair<string, string> parameter in definition.Parameters)
                    set.SetAttribute(parameter.Key, parameter.Value);
            return node;
        }

        private PerkInfoItem EnsureScriptedEnchantmentApplied(EnchantmentDefinition definition)
        {
            string runtimeName = definition.Id.ToString();
            PerkInfoItem existing = _perks.ABAGJKMKCBA(runtimeName);
            if (existing != null)
            {
                if (_perkNames.Contains(runtimeName)) return existing;
                throw new InvalidOperationException("Legacy perk already exists for scripted enchantment: " + runtimeName);
            }
            XmlElement node = BuildScriptedPerkNode(definition.Id, definition.DisplayName,
                definition.Description, definition.Icon, definition.HasIcon, definition.Kind);
            PerkInfoItem applied = _perks.AddExternalBasePerk(node);
            _perkNames.Add(applied.Name);
            return applied;
        }

        private XmlElement BuildScriptedPerkNode(DefinitionId id, DefinitionId displayName,
            DefinitionId description, AssetId icon, bool hasIcon, ModPerkKind kind)
        {
            var document = new XmlDocument();
            XmlElement node = document.CreateElement("Perk");
            document.AppendChild(node);
            node.SetAttribute("Name", id.ToString());
            node.SetAttribute("ID", _perks.CJJEPHDFOCJ().Count.ToString(CultureInfo.InvariantCulture));
            node.SetAttribute("Alias", displayName.ToString());
            node.SetAttribute("Description", description.ToString());
            if (hasIcon) node.SetAttribute("Image", icon.ToString());
            if (kind == ModPerkKind.Combo) node.SetAttribute("PerkType", "Combo");
            return node;
        }

        private static IReadOnlyDictionary<string, string> ToWireParameters(
            IReadOnlyDictionary<string, ModParameterValue> parameters)
        {
            if (parameters == null || parameters.Count == 0) return null;
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, ModParameterValue> pair in parameters)
                result.Add(pair.Key, pair.Value.ToWireString());
            return result;
        }

        private void RemovePerksAndEnchantments()
        {
            if (_forge != null)
            {
                for (int i = _enchantmentBindings.Count - 1; i >= 0; i--)
                {
                    ExternalEnchantmentBinding binding = _enchantmentBindings[i];
                    _forge.RemoveExternalEnchantmentCandidate(binding.Recipe, binding.ItemType, binding.PerkName);
                }
            }
            _enchantmentBindings.Clear();
            if (_perks != null)
                for (int i = _perkNames.Count - 1; i >= 0; i--) _perks.RemoveExternalBasePerk(_perkNames[i]);
            _perkNames.Clear();
            _perksApplied = false;
            _perks = null;
            _forge = null;
        }

        private static string RuntimePerkName(PerkDefinition definition)
        {
            return definition.IsCore ? definition.LegacyName : definition.Id.ToString();
        }

        private static string RecipeName(ModEnchantmentRecipe recipe)
        {
            switch (recipe)
            {
                case ModEnchantmentRecipe.Simple: return "Simple";
                case ModEnchantmentRecipe.Medium: return "Medium";
                case ModEnchantmentRecipe.Complex: return "Complex";
                default: throw new InvalidOperationException("Unsupported enchantment recipe: " + recipe);
            }
        }

        private static string EquipmentType(ModEquipmentKind kind)
        {
            switch (kind)
            {
                case ModEquipmentKind.Weapon: return "Weapon";
                case ModEquipmentKind.Armor: return "Armor";
                case ModEquipmentKind.Helm: return "Helm";
                case ModEquipmentKind.Ranged: return "Ranged";
                case ModEquipmentKind.Magic: return "Magic";
                default: throw new InvalidOperationException("Unsupported equipment kind: " + kind);
            }
        }

        private sealed class ExternalEnchantmentBinding
        {
            public readonly string Recipe;
            public readonly string ItemType;
            public readonly string PerkName;

            public ExternalEnchantmentBinding(string recipe, string itemType, string perkName)
            {
                Recipe = recipe;
                ItemType = itemType;
                PerkName = perkName;
            }
        }

        private XmlElement BuildItemNode(ItemDefinition definition, ShopListingDefinition listing)
        {
            if (definition.Progression != ItemProgressionKind.Vanilla)
                throw new InvalidOperationException("External item does not use the vanilla progression profile: " +
                    definition.Id);

            var document = new XmlDocument();
            XmlElement item = document.CreateElement("Item");
            document.AppendChild(item);
            Set(item, "Name", definition.Id.ToString());
            Set(item, "Image", definition.Icon.ToString());
            Set(item, "Model", definition.Model.ToString());
            Set(item, "Text", definition.DisplayName.ToString());
            Set(item, "TextButton", definition.DisplayName.ToString());
            Set(item, "Level", listing.Level.ToString(CultureInfo.InvariantCulture));
            Set(item, "UpgradeLevel", (listing.Level * 100).ToString(CultureInfo.InvariantCulture));

            string upgradeTemplate;
            if (definition is WeaponDefinition weapon)
            {
                Set(item, "Type", "Weapon");
                Set(item, "SubType", weapon.SubType);
                upgradeTemplate = "Weapon_Bonus";
                Set(item, "WeaponDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "WeaponDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is ArmorDefinition)
            {
                Set(item, "Type", "Armor");
                upgradeTemplate = "Armor_Bonus";
                Set(item, "BodyDefense", ResolveVanillaStat(upgradeTemplate, listing.Level, "BodyDefense")
                    .ToString(CultureInfo.InvariantCulture));
                Set(item, "UnarmedDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "UnarmedDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is HelmDefinition)
            {
                Set(item, "Type", "Helm");
                upgradeTemplate = "Helm_Bonus";
                Set(item, "HeadDefense", ResolveVanillaStat(upgradeTemplate, listing.Level, "HeadDefense")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is RangedDefinition ranged)
            {
                Set(item, "Type", "Ranged");
                Set(item, "SubType", ranged.SubType);
                upgradeTemplate = "Ranged_Bonus";
                Set(item, "RangedDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "RangedDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else if (definition is MagicDefinition magic)
            {
                Set(item, "Type", "Magic");
                Set(item, "SubType", magic.SubType);
                upgradeTemplate = "Magic_Bonus";
                Set(item, "MagicDamage", ResolveVanillaStat(upgradeTemplate, listing.Level, "MagicDamage")
                    .ToString(CultureInfo.InvariantCulture));
            }
            else throw new InvalidOperationException("Unsupported external item definition: " + definition.Id);

            if (listing.Price.Currency == ModPriceCurrency.Coins)
                Set(item, "Price", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));
            else
                Set(item, "BonusPrice", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));

            XmlElement upgrades = document.CreateElement("Upgrades");
            upgrades.SetAttribute("Template", upgradeTemplate);
            item.AppendChild(upgrades);
            return item;
        }

        private int ResolveVanillaStat(string template, int level, string attribute)
        {
            // The shared vanilla tables begin at level 3 for melee/defense equipment and
            // level 6 for ranged/magic. Preserve the canonical early normal-item baselines
            // instead of inventing a formula for values that are stored directly on items.
            if (template == "Weapon_Bonus")
            {
                if (level == 1 && attribute == "WeaponDamage") return 5;
                if (level == 2 && attribute == "WeaponDamage") return 20;
            }
            else if (template == "Armor_Bonus" && level == 2)
            {
                if (attribute == "BodyDefense") return 22;
                if (attribute == "UnarmedDamage") return 8;
            }
            else if (template == "Helm_Bonus" && level == 2 && attribute == "HeadDefense")
            {
                return 18;
            }

            UpgradeDataContainer upgrades = _items.BKPOCLGODDM(template);
            if (upgrades == null)
                throw new InvalidOperationException("Vanilla upgrade template is unavailable: " + template);

            int upgradeLevel = checked(level * 100);
            for (int i = 0; i < upgrades.KPAPEBOAKIE.Count; i++)
            {
                UpgradeData upgrade = upgrades.KPAPEBOAKIE[i];
                if (upgrade.OGLHOJNMEBD.Level != level || upgrade.OGLHOJNMEBD.AKKLOMFOLNO != upgradeLevel)
                    continue;
                int value = 0;
                if (!upgrade.OGLHOJNMEBD.IBLHIAHECLK.Get(attribute, ref value, false))
                    throw new InvalidOperationException("Vanilla progression milestone " + template + " level " +
                        level + " does not define " + attribute + ".");
                return value;
            }

            throw new InvalidOperationException("Vanilla progression milestone is unavailable: " + template +
                " level " + level + ".");
        }

        private static void Set(XmlElement element, string name, string value)
        {
            element.SetAttribute(name, value ?? string.Empty);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(LegacyContentAdapter));
        }
    }
}
