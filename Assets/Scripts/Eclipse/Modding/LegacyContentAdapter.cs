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
        private Items _items;
        private bool _itemsApplied;
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

        public void Dispose()
        {
            if (_disposed) return;
            if (_languageSubscribed)
            {
                LocalizationManager.OCLBJLPOKLB -= OnLanguageChanged;
                _languageSubscribed = false;
            }
            RemoveLocalization();
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
