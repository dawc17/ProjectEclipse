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
                WeaponDefinition weapon;
                if (!_content.TryGetWeapon(listing.Item, out weapon))
                    throw new InvalidOperationException("Committed shop listing has no weapon: " + listing.Item);
                if (_items.KCCDBEEKBCG(weapon.Id.ToString()) != null)
                    throw new InvalidOperationException("Legacy item already exists: " + weapon.Id);
            }

            try
            {
                foreach (ShopListingDefinition listing in _content.ShopListings)
                {
                    WeaponDefinition weapon;
                    if (!_content.TryGetWeapon(listing.Item, out weapon)) continue;
                    XmlElement node = BuildWeaponNode(weapon, listing);
                    ItemInfo item = _items.AddExternalWeapon(node);
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
            foreach (WeaponDefinition weapon in _content.Weapons)
            {
                LocalizationDefinition displayName;
                if (!_content.TryGetLocalization(weapon.DisplayName, out displayName)) continue;
                string value = displayName.GetOrEnglish(language);
                if (string.IsNullOrEmpty(value)) continue;
                string key = weapon.Id.ToString();
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
                    _items.RemoveExternalWeapon(_itemNames[i]);
            }
            _itemNames.Clear();
            _itemsApplied = false;
        }

        private static XmlElement BuildWeaponNode(WeaponDefinition weapon, ShopListingDefinition listing)
        {
            var document = new XmlDocument();
            XmlElement item = document.CreateElement("Item");
            document.AppendChild(item);
            Set(item, "Name", weapon.Id.ToString());
            Set(item, "Image", weapon.Icon.ToString());
            Set(item, "Model", weapon.Model.ToString());
            Set(item, "Type", "Weapon");
            Set(item, "SubType", weapon.SubType);
            Set(item, "Text", weapon.DisplayName.ToString());
            Set(item, "TextButton", weapon.DisplayName.ToString());
            Set(item, "Level", listing.Level.ToString(CultureInfo.InvariantCulture));
            Set(item, "UpgradeLevel", (listing.Level * 100).ToString(CultureInfo.InvariantCulture));
            Set(item, "WeaponDamage", weapon.Damage.ToString(CultureInfo.InvariantCulture));
            if (listing.Price.Currency == ModPriceCurrency.Coins)
                Set(item, "Price", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));
            else
                Set(item, "BonusPrice", listing.Price.Amount.ToString(CultureInfo.InvariantCulture));

            XmlElement upgrades = document.CreateElement("Upgrades");
            upgrades.SetAttribute("Template", "Weapon_Bonus");
            item.AppendChild(upgrades);
            return item;
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
