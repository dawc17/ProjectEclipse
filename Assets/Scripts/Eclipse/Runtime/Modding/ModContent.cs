using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModShopSection
    {
        Weapons = 0,
        Armor = 1,
        Helmets = 2,
        Ranged = 3,
        Magic = 4
    }

    public enum ModPriceCurrency
    {
        Coins = 0,
        Gems = 1
    }

    public readonly struct ModPrice : IEquatable<ModPrice>
    {
        public ModPriceCurrency Currency { get; }
        public long Amount { get; }

        public ModPrice(ModPriceCurrency currency, long amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Price amount must not be negative.");
            Currency = currency;
            Amount = amount;
        }

        public bool Equals(ModPrice other)
        {
            return Currency == other.Currency && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            return obj is ModPrice other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked { return ((int)Currency * 397) ^ Amount.GetHashCode(); }
        }

        public override string ToString()
        {
            return Currency.ToString().ToLowerInvariant() + ":" + Amount;
        }

        public static bool operator ==(ModPrice left, ModPrice right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModPrice left, ModPrice right)
        {
            return !left.Equals(right);
        }
    }

    public sealed class LocalizationDefinition
    {
        private readonly Dictionary<string, string> _values;

        public DefinitionId Id { get; }
        public IReadOnlyDictionary<string, string> Values => _values;

        internal LocalizationDefinition(DefinitionId id, Dictionary<string, string> values)
        {
            Id = id;
            _values = new Dictionary<string, string>(values ?? throw new ArgumentNullException(nameof(values)),
                StringComparer.Ordinal);
        }

        public bool TryGet(string language, out string value)
        {
            value = null;
            return language != null && _values.TryGetValue(language.ToLowerInvariant(), out value);
        }

        public string GetOrEnglish(string language)
        {
            string value;
            if (TryGet(language, out value)) return value;
            return _values.TryGetValue("eng", out value) ? value : string.Empty;
        }
    }

    public enum ItemProgressionKind
    {
        LegacySnapshot = 0,
        Vanilla = 1
    }

    public abstract class ItemDefinition
    {
        public DefinitionId Id { get; }
        public DefinitionId DisplayName { get; }
        public AssetId Icon { get; }
        public AssetId Model { get; }
        public bool HasIcon => !string.IsNullOrEmpty(Icon.Path);
        public bool HasModel => !string.IsNullOrEmpty(Model.Path);
        public string LegacyName { get; }
        public string LegacyItemXml { get; }
        public bool IsCore => Id.Namespace.Value == "core";
        public ItemProgressionKind Progression { get; }

        protected ItemDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model,
            string legacyName = null, string legacyItemXml = null,
            ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
        {
            Id = id;
            DisplayName = displayName;
            Icon = icon;
            Model = model;
            LegacyName = legacyName;
            LegacyItemXml = legacyItemXml;
            Progression = progression;
        }
    }

    public sealed class WeaponDefinition : ItemDefinition
    {
        public string SubType { get; }
        public int Damage { get; }

        internal WeaponDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model,
            string subType, int damage, string legacyName = null, string legacyItemXml = null,
            ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
            : base(id, displayName, icon, model, legacyName, legacyItemXml, progression)
        {
            SubType = subType;
            Damage = damage;
        }
    }

    public sealed class ArmorDefinition : ItemDefinition
    {
        public int BodyDefense { get; }
        public int HeadDefense { get; }
        public int UnarmedDamage { get; }

        internal ArmorDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model,
            int bodyDefense, int headDefense, int unarmedDamage, string legacyName = null,
            string legacyItemXml = null, ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
            : base(id, displayName, icon, model, legacyName, legacyItemXml, progression)
        {
            BodyDefense = bodyDefense;
            HeadDefense = headDefense;
            UnarmedDamage = unarmedDamage;
        }
    }

    public sealed class HelmDefinition : ItemDefinition
    {
        public int HeadDefense { get; }

        internal HelmDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model, int headDefense,
            string legacyName = null, string legacyItemXml = null,
            ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
            : base(id, displayName, icon, model, legacyName, legacyItemXml, progression)
        {
            HeadDefense = headDefense;
        }
    }

    public sealed class RangedDefinition : ItemDefinition
    {
        public string SubType { get; }
        public int RangedDamage { get; }
        public int WeaponDamage { get; }

        internal RangedDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model, string subType,
            int rangedDamage, int weaponDamage, string legacyName = null, string legacyItemXml = null,
            ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
            : base(id, displayName, icon, model, legacyName, legacyItemXml, progression)
        {
            SubType = subType;
            RangedDamage = rangedDamage;
            WeaponDamage = weaponDamage;
        }
    }

    public sealed class MagicDefinition : ItemDefinition
    {
        public string SubType { get; }
        public int MagicDamage { get; }

        internal MagicDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model, string subType,
            int magicDamage, string legacyName = null, string legacyItemXml = null,
            ItemProgressionKind progression = ItemProgressionKind.LegacySnapshot)
            : base(id, displayName, icon, model, legacyName, legacyItemXml, progression)
        {
            SubType = subType;
            MagicDamage = magicDamage;
        }
    }

    public sealed class ItemRedirectDefinition
    {
        public DefinitionId Id { get; }
        public DefinitionId Target { get; }
        public bool IsTombstone { get; }

        internal ItemRedirectDefinition(DefinitionId id, DefinitionId target, bool isTombstone)
        {
            Id = id;
            Target = target;
            IsTombstone = isTombstone;
        }
    }

    public sealed class ShopListingDefinition
    {
        public DefinitionId Id { get; }
        public DefinitionId Item { get; }
        public ModShopSection Section { get; }
        public int Level { get; }
        public ModPrice Price { get; }

        internal ShopListingDefinition(DefinitionId id, DefinitionId item, ModShopSection section,
            int level, ModPrice price)
        {
            Id = id;
            Item = item;
            Section = section;
            Level = level;
            Price = price;
        }
    }

    public sealed class ModContentCatalog
    {
        private readonly DefinitionRegistry<LocalizationDefinition> _localizations =
            new DefinitionRegistry<LocalizationDefinition>(value => value.Id);
        private readonly DefinitionRegistry<WeaponDefinition> _weapons =
            new DefinitionRegistry<WeaponDefinition>(value => value.Id);
        private readonly DefinitionRegistry<ArmorDefinition> _armors =
            new DefinitionRegistry<ArmorDefinition>(value => value.Id);
        private readonly DefinitionRegistry<HelmDefinition> _helms =
            new DefinitionRegistry<HelmDefinition>(value => value.Id);
        private readonly DefinitionRegistry<RangedDefinition> _ranged =
            new DefinitionRegistry<RangedDefinition>(value => value.Id);
        private readonly DefinitionRegistry<MagicDefinition> _magic =
            new DefinitionRegistry<MagicDefinition>(value => value.Id);
        private readonly DefinitionRegistry<ItemRedirectDefinition> _itemRedirects =
            new DefinitionRegistry<ItemRedirectDefinition>(value => value.Id);
        private readonly DefinitionRegistry<ShopListingDefinition> _shopListings =
            new DefinitionRegistry<ShopListingDefinition>(value => value.Id);

        public bool IsFrozen { get; private set; }
        public IReadOnlyList<LocalizationDefinition> Localizations => _localizations.Values;
        public IReadOnlyList<WeaponDefinition> Weapons => _weapons.Values;
        public IReadOnlyList<ArmorDefinition> Armors => _armors.Values;
        public IReadOnlyList<HelmDefinition> Helms => _helms.Values;
        public IReadOnlyList<RangedDefinition> Ranged => _ranged.Values;
        public IReadOnlyList<MagicDefinition> Magic => _magic.Values;
        public IReadOnlyList<ItemRedirectDefinition> ItemRedirects => _itemRedirects.Values;
        public IReadOnlyList<ShopListingDefinition> ShopListings => _shopListings.Values;

        public ModRegistrationTransaction BeginRegistration(ModDescriptor mod)
        {
            if (mod == null) throw new ArgumentNullException(nameof(mod));
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            return new ModRegistrationTransaction(this, mod);
        }

        public bool TryGetLocalization(DefinitionId id, out LocalizationDefinition value)
        {
            return _localizations.TryGet(id, out value);
        }

        public bool TryGetWeapon(DefinitionId id, out WeaponDefinition value)
        {
            return _weapons.TryGet(id, out value);
        }

        public bool TryGetArmor(DefinitionId id, out ArmorDefinition value)
        {
            return _armors.TryGet(id, out value);
        }

        public bool TryGetHelm(DefinitionId id, out HelmDefinition value)
        {
            return _helms.TryGet(id, out value);
        }

        public bool TryGetRanged(DefinitionId id, out RangedDefinition value)
        {
            return _ranged.TryGet(id, out value);
        }

        public bool TryGetMagic(DefinitionId id, out MagicDefinition value)
        {
            return _magic.TryGet(id, out value);
        }

        public bool TryGetItem(DefinitionId id, out ItemDefinition value)
        {
            value = null;
            if (id.Category != "items") return false;
            WeaponDefinition weapon;
            if (_weapons.TryGet(id, out weapon)) { value = weapon; return true; }
            ArmorDefinition armor;
            if (_armors.TryGet(id, out armor)) { value = armor; return true; }
            HelmDefinition helm;
            if (_helms.TryGet(id, out helm)) { value = helm; return true; }
            RangedDefinition ranged;
            if (_ranged.TryGet(id, out ranged)) { value = ranged; return true; }
            MagicDefinition magic;
            if (_magic.TryGet(id, out magic)) { value = magic; return true; }
            return false;
        }

        public bool TryGetItemRedirect(DefinitionId id, out ItemRedirectDefinition value)
        {
            return _itemRedirects.TryGet(id, out value);
        }

        public bool TryResolveItem(DefinitionId id, out ItemDefinition value)
        {
            value = null;
            if (id.Category != "items") return false;
            DefinitionId current = id;
            var visited = new HashSet<DefinitionId>();
            while (visited.Add(current))
            {
                if (TryGetItem(current, out value)) return true;
                ItemRedirectDefinition redirect;
                if (!_itemRedirects.TryGet(current, out redirect) || redirect.IsTombstone) return false;
                current = redirect.Target;
            }
            return false;
        }

        public bool TryGetShopListing(DefinitionId id, out ShopListingDefinition value)
        {
            return _shopListings.TryGet(id, out value);
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        internal void Commit(ModRegistrationTransaction transaction,
            LocalizationDefinition[] localizations, WeaponDefinition[] weapons,
            ArmorDefinition[] armors, HelmDefinition[] helms, RangedDefinition[] ranged,
            MagicDefinition[] magic, ItemRedirectDefinition[] itemRedirects,
            ShopListingDefinition[] shopListings)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");

            _localizations.ValidateCanAdd(localizations);
            _weapons.ValidateCanAdd(weapons);
            _armors.ValidateCanAdd(armors);
            _helms.ValidateCanAdd(helms);
            _ranged.ValidateCanAdd(ranged);
            _magic.ValidateCanAdd(magic);
            _itemRedirects.ValidateCanAdd(itemRedirects);
            _shopListings.ValidateCanAdd(shopListings);

            ValidateRegisteredItems(localizations, weapons, "Weapon");
            ValidateRegisteredItems(localizations, armors, "Armor");
            ValidateRegisteredItems(localizations, helms, "Helm");
            ValidateRegisteredItems(localizations, ranged, "Ranged item");
            ValidateRegisteredItems(localizations, magic, "Magic item");

            for (int i = 0; i < shopListings.Length; i++)
            {
                ItemDefinition item;
                if (!TryGetPendingItem(shopListings[i].Item, weapons, armors, helms, ranged, magic, out item) &&
                    !TryGetItem(shopListings[i].Item, out item))
                    throw new ModContentException("Shop listing '" + shopListings[i].Id +
                        "' references missing item '" + shopListings[i].Item + "'.");
                if (!SectionMatchesItem(shopListings[i].Section, item))
                    throw new ModContentException("Shop listing '" + shopListings[i].Id +
                        "' uses the wrong section for item '" + shopListings[i].Item + "'.");
            }

            _localizations.AddRange(localizations);
            _weapons.AddRange(weapons);
            _armors.AddRange(armors);
            _helms.AddRange(helms);
            _ranged.AddRange(ranged);
            _magic.AddRange(magic);
            _itemRedirects.AddRange(itemRedirects);
            _shopListings.AddRange(shopListings);
        }

        private void ValidateRegisteredItems<T>(LocalizationDefinition[] localizations, T[] items, string type)
            where T : ItemDefinition
        {
            for (int i = 0; i < items.Length; i++)
            {
                LocalizationDefinition ignored;
                if (!ContainsLocalization(localizations, items[i].DisplayName) &&
                    !_localizations.TryGet(items[i].DisplayName, out ignored))
                    throw new ModContentException(type + " '" + items[i].Id +
                        "' references missing localization '" + items[i].DisplayName + "'.");
            }
        }

        private static bool TryGetPendingItem(DefinitionId id, WeaponDefinition[] weapons,
            ArmorDefinition[] armors, HelmDefinition[] helms, RangedDefinition[] ranged,
            MagicDefinition[] magic, out ItemDefinition value)
        {
            value = null;
            for (int i = 0; i < weapons.Length; i++) if (weapons[i].Id == id) { value = weapons[i]; return true; }
            for (int i = 0; i < armors.Length; i++) if (armors[i].Id == id) { value = armors[i]; return true; }
            for (int i = 0; i < helms.Length; i++) if (helms[i].Id == id) { value = helms[i]; return true; }
            for (int i = 0; i < ranged.Length; i++) if (ranged[i].Id == id) { value = ranged[i]; return true; }
            for (int i = 0; i < magic.Length; i++) if (magic[i].Id == id) { value = magic[i]; return true; }
            return false;
        }

        internal static bool SectionMatchesItem(ModShopSection section, ItemDefinition item)
        {
            if (item is WeaponDefinition) return section == ModShopSection.Weapons;
            if (item is ArmorDefinition) return section == ModShopSection.Armor;
            if (item is HelmDefinition) return section == ModShopSection.Helmets;
            if (item is RangedDefinition) return section == ModShopSection.Ranged;
            if (item is MagicDefinition) return section == ModShopSection.Magic;
            return false;
        }

        internal void ImportCore(LocalizationDefinition[] localizations, WeaponDefinition[] weapons,
            ArmorDefinition[] armors = null, HelmDefinition[] helms = null,
            RangedDefinition[] ranged = null, MagicDefinition[] magic = null)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            armors = armors ?? Array.Empty<ArmorDefinition>();
            helms = helms ?? Array.Empty<HelmDefinition>();
            ranged = ranged ?? Array.Empty<RangedDefinition>();
            magic = magic ?? Array.Empty<MagicDefinition>();
            _localizations.ValidateCanAdd(localizations);
            _weapons.ValidateCanAdd(weapons);
            _armors.ValidateCanAdd(armors);
            _helms.ValidateCanAdd(helms);
            _ranged.ValidateCanAdd(ranged);
            _magic.ValidateCanAdd(magic);
            foreach (WeaponDefinition weapon in weapons)
                if (!weapon.IsCore || !HasCoreLocalization(localizations, weapon.DisplayName))
                    throw new ModContentException("Invalid core weapon import: " + weapon.Id);
            foreach (ArmorDefinition armor in armors)
                if (!armor.IsCore || !HasCoreLocalization(localizations, armor.DisplayName))
                    throw new ModContentException("Invalid core armor import: " + armor.Id);
            ValidateCoreItems(localizations, helms, "helm");
            ValidateCoreItems(localizations, ranged, "ranged");
            ValidateCoreItems(localizations, magic, "magic");
            _localizations.AddRange(localizations);
            _weapons.AddRange(weapons);
            _armors.AddRange(armors);
            _helms.AddRange(helms);
            _ranged.AddRange(ranged);
            _magic.AddRange(magic);
        }

        private void ValidateCoreItems<T>(LocalizationDefinition[] localizations, T[] items, string type)
            where T : ItemDefinition
        {
            foreach (T item in items)
                if (!item.IsCore || !HasCoreLocalization(localizations, item.DisplayName))
                    throw new ModContentException("Invalid core " + type + " import: " + item.Id);
        }

        private bool HasCoreLocalization(LocalizationDefinition[] pending, DefinitionId id)
        {
            LocalizationDefinition ignored;
            return ContainsLocalization(pending, id) || _localizations.TryGet(id, out ignored);
        }

        private static bool ContainsLocalization(LocalizationDefinition[] values, DefinitionId id)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i].Id == id) return true;
            return false;
        }

        private static bool ContainsWeapon(WeaponDefinition[] values, DefinitionId id)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i].Id == id) return true;
            return false;
        }

        private sealed class DefinitionRegistry<T>
        {
            private readonly Func<T, DefinitionId> _getId;
            private readonly Dictionary<DefinitionId, T> _byId = new Dictionary<DefinitionId, T>();
            private readonly List<T> _values = new List<T>();
            private readonly IReadOnlyList<T> _readOnlyValues;

            public IReadOnlyList<T> Values => _readOnlyValues;

            public DefinitionRegistry(Func<T, DefinitionId> getId)
            {
                _getId = getId ?? throw new ArgumentNullException(nameof(getId));
                _readOnlyValues = _values.AsReadOnly();
            }

            public bool TryGet(DefinitionId id, out T value)
            {
                return _byId.TryGetValue(id, out value);
            }

            public void ValidateCanAdd(T[] values)
            {
                var pending = new HashSet<DefinitionId>();
                for (int i = 0; i < values.Length; i++)
                {
                    DefinitionId id = _getId(values[i]);
                    if (_byId.ContainsKey(id) || !pending.Add(id))
                        throw new ModContentException("Definition already exists: '" + id + "'.");
                }
            }

            public void AddRange(T[] values)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    DefinitionId id = _getId(values[i]);
                    _byId.Add(id, values[i]);
                    _values.Add(values[i]);
                }
            }
        }
    }

    public sealed class ModRegistrationTransaction : IDisposable
    {
        public const int MaxRegistrations = 4096;
        public const int MaxEquipmentLevel = 52;

        private readonly ModContentCatalog _catalog;
        private readonly Dictionary<DefinitionId, Dictionary<string, string>> _localizations =
            new Dictionary<DefinitionId, Dictionary<string, string>>();
        private readonly Dictionary<DefinitionId, WeaponDefinition> _weapons =
            new Dictionary<DefinitionId, WeaponDefinition>();
        private readonly Dictionary<DefinitionId, ArmorDefinition> _armors =
            new Dictionary<DefinitionId, ArmorDefinition>();
        private readonly Dictionary<DefinitionId, HelmDefinition> _helms =
            new Dictionary<DefinitionId, HelmDefinition>();
        private readonly Dictionary<DefinitionId, RangedDefinition> _ranged =
            new Dictionary<DefinitionId, RangedDefinition>();
        private readonly Dictionary<DefinitionId, MagicDefinition> _magic =
            new Dictionary<DefinitionId, MagicDefinition>();
        private readonly Dictionary<DefinitionId, ItemRedirectDefinition> _itemRedirects =
            new Dictionary<DefinitionId, ItemRedirectDefinition>();
        private readonly Dictionary<DefinitionId, ShopListingDefinition> _shopListings =
            new Dictionary<DefinitionId, ShopListingDefinition>();
        private readonly HashSet<DefinitionId> _listedItems = new HashSet<DefinitionId>();
        private bool _completed;

        public ModDescriptor Mod { get; }
        public int RegistrationCount => _localizations.Count + _weapons.Count + _armors.Count + _helms.Count +
            _ranged.Count + _magic.Count + _itemRedirects.Count + _shopListings.Count;

        internal ModRegistrationTransaction(ModContentCatalog catalog, ModDescriptor mod)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
        }

        public DefinitionId AddLocalization(string key, string language, string value)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("localization", key);
            string normalizedLanguage = NormalizeLanguage(language);
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0) throw new ModContentException("Localization value for '" + id + "' must not be empty.");

            Dictionary<string, string> values;
            if (!_localizations.TryGetValue(id, out values))
            {
                EnsureCapacityForNewRegistration();
                values = new Dictionary<string, string>(StringComparer.Ordinal);
                _localizations.Add(id, values);
            }
            if (values.ContainsKey(normalizedLanguage))
                throw new ModContentException("Duplicate localization '" + id + "' for language '" + normalizedLanguage + "'.");
            values.Add(normalizedLanguage, value);
            return id;
        }

        public DefinitionId GetLocalization(string key)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("localization", key);
            if (_localizations.ContainsKey(id)) return id;
            LocalizationDefinition ignored;
            if (_catalog.TryGetLocalization(id, out ignored)) return id;
            throw new ModContentException("Localization key is not registered: '" + id + "'.");
        }

        public WeaponDefinition RegisterWeapon(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "weapon/" + localId);
            EnsureItemIdAvailable(id);
            if (displayName.Namespace != Mod.Id)
                throw new ModContentException("Weapon display_name must belong to mod namespace '" + Mod.Id + "'.");
            if (displayName.Category != "localization")
                throw new ModContentException("Weapon display_name must be a localization handle.");
            if (string.IsNullOrWhiteSpace(subType))
                throw new ModContentException("Weapon subtype must not be empty.");
            if (_weapons.ContainsKey(id))
                throw new ModContentException("Duplicate weapon definition: '" + id + "'.");

            EnsureCapacityForNewRegistration();
            var definition = new WeaponDefinition(id, displayName, icon, model, subType.Trim(), 0,
                progression: ItemProgressionKind.Vanilla);
            _weapons.Add(id, definition);
            return definition;
        }

        public ArmorDefinition RegisterArmor(string localId, DefinitionId displayName, AssetId icon,
            AssetId model)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "armor/" + localId);
            EnsureItemIdAvailable(id);
            ValidateExternalItem(id, displayName, "Armor");
            if (_armors.ContainsKey(id)) throw new ModContentException("Duplicate armor definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new ArmorDefinition(id, displayName, icon, model, 0, 0, 0,
                progression: ItemProgressionKind.Vanilla);
            _armors.Add(id, definition);
            return definition;
        }

        public HelmDefinition RegisterHelm(string localId, DefinitionId displayName, AssetId icon,
            AssetId model)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "helm/" + localId);
            EnsureItemIdAvailable(id);
            ValidateExternalItem(id, displayName, "Helm");
            if (_helms.ContainsKey(id)) throw new ModContentException("Duplicate helm definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new HelmDefinition(id, displayName, icon, model, 0,
                progression: ItemProgressionKind.Vanilla);
            _helms.Add(id, definition);
            return definition;
        }

        public RangedDefinition RegisterRanged(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "ranged/" + localId);
            EnsureItemIdAvailable(id);
            ValidateExternalItem(id, displayName, "Ranged item");
            if (string.IsNullOrWhiteSpace(subType)) throw new ModContentException("Ranged subtype must not be empty.");
            if (_ranged.ContainsKey(id)) throw new ModContentException("Duplicate ranged definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new RangedDefinition(id, displayName, icon, model, subType.Trim(), 0, 0,
                progression: ItemProgressionKind.Vanilla);
            _ranged.Add(id, definition);
            return definition;
        }

        public MagicDefinition RegisterMagic(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "magic/" + localId);
            EnsureItemIdAvailable(id);
            ValidateExternalItem(id, displayName, "Magic item");
            if (string.IsNullOrWhiteSpace(subType)) throw new ModContentException("Magic subtype must not be empty.");
            if (_magic.ContainsKey(id)) throw new ModContentException("Duplicate magic definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new MagicDefinition(id, displayName, icon, model, subType.Trim(), 0,
                progression: ItemProgressionKind.Vanilla);
            _magic.Add(id, definition);
            return definition;
        }

        public ItemRedirectDefinition RegisterItemAlias(string oldLocalPath, DefinitionId target)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", oldLocalPath);
            ValidateRedirectSource(id);
            if (target.Category != "items" || target.Namespace != Mod.Id)
                throw new ModContentException("Item alias target must be an item owned by mod namespace '" + Mod.Id + "'.");
            ItemDefinition targetDefinition;
            if (!TryGetPendingItem(target, out targetDefinition) && !_catalog.TryResolveItem(target, out targetDefinition))
                throw new ModContentException("Item alias target is not registered: '" + target + "'.");
            if (!SameItemKind(id, targetDefinition.Id))
                throw new ModContentException("Item alias must preserve its equipment category: '" + id + "' -> '" +
                    targetDefinition.Id + "'.");
            if (id == targetDefinition.Id)
                throw new ModContentException("Item alias cannot target itself: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var redirect = new ItemRedirectDefinition(id, targetDefinition.Id, false);
            _itemRedirects.Add(id, redirect);
            return redirect;
        }

        public ItemRedirectDefinition RegisterItemTombstone(string oldLocalPath)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", oldLocalPath);
            ValidateRedirectSource(id);
            EnsureCapacityForNewRegistration();
            var redirect = new ItemRedirectDefinition(id, default(DefinitionId), true);
            _itemRedirects.Add(id, redirect);
            return redirect;
        }

        public ShopListingDefinition RegisterShopListing(DefinitionId item, ModShopSection section,
            int level, ModPrice price)
        {
            ThrowIfCompleted();
            if (item.Category != "items") throw new ModContentException("Shop item must be an item definition.");
            if (item.Namespace != Mod.Id)
                throw new ModContentException("Mod API v1 shop listings may only expose items owned by the registering mod.");
            ItemDefinition definition;
            if (!TryGetPendingItem(item, out definition))
                throw new ModContentException("Shop item must be registered by the same transaction: '" + item + "'.");
            if (!ModContentCatalog.SectionMatchesItem(section, definition))
                throw new ModContentException("Shop section does not match item type: '" + item + "'.");
            int minimumLevel = MinimumVanillaProgressionLevel(definition);
            if (level < minimumLevel || level > MaxEquipmentLevel)
                throw new ModContentException("Shop level for '" + item + "' must be within " + minimumLevel +
                    ".." + MaxEquipmentLevel + " for the vanilla progression profile.");
            if (!_listedItems.Add(item))
                throw new ModContentException("Item already has a shop listing: '" + item + "'.");

            int slash = item.LocalId.IndexOf('/');
            string local = slash < 0 ? item.LocalId : item.LocalId.Substring(slash + 1);
            DefinitionId id = Qualify("shop", ShopSectionPath(section) + "/" + local);
            if (_shopListings.ContainsKey(id))
                throw new ModContentException("Duplicate shop listing: '" + id + "'.");

            EnsureCapacityForNewRegistration();
            var listing = new ShopListingDefinition(id, item, section, level, price);
            _shopListings.Add(id, listing);
            return listing;
        }

        public void Commit()
        {
            ThrowIfCompleted();

            var localizations = new LocalizationDefinition[_localizations.Count];
            int localizationIndex = 0;
            foreach (KeyValuePair<DefinitionId, Dictionary<string, string>> pair in _localizations)
            {
                if (!pair.Value.ContainsKey("eng"))
                    throw new ModContentException("Localization '" + pair.Key + "' must provide an 'eng' fallback.");
                localizations[localizationIndex++] = new LocalizationDefinition(pair.Key, pair.Value);
            }

            var weapons = new WeaponDefinition[_weapons.Count];
            _weapons.Values.CopyTo(weapons, 0);
            var armors = new ArmorDefinition[_armors.Count];
            _armors.Values.CopyTo(armors, 0);
            var helms = new HelmDefinition[_helms.Count];
            _helms.Values.CopyTo(helms, 0);
            var ranged = new RangedDefinition[_ranged.Count];
            _ranged.Values.CopyTo(ranged, 0);
            var magic = new MagicDefinition[_magic.Count];
            _magic.Values.CopyTo(magic, 0);
            var itemRedirects = new ItemRedirectDefinition[_itemRedirects.Count];
            _itemRedirects.Values.CopyTo(itemRedirects, 0);
            var listings = new ShopListingDefinition[_shopListings.Count];
            _shopListings.Values.CopyTo(listings, 0);

            _catalog.Commit(this, localizations, weapons, armors, helms, ranged, magic, itemRedirects, listings);
            _completed = true;
            ClearPending();
        }

        public void Dispose()
        {
            if (_completed) return;
            _completed = true;
            ClearPending();
        }

        private DefinitionId Qualify(string category, string localId)
        {
            if (string.IsNullOrWhiteSpace(localId))
                throw new ModContentException("Definition local ID must not be empty.");
            try { return DefinitionId.Parse(Mod.Id.Value + ":" + category + "/" + localId); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
        }

        private void EnsureCapacityForNewRegistration()
        {
            if (RegistrationCount >= MaxRegistrations)
                throw new ModContentException("Registration limit exceeded (" + MaxRegistrations + ").");
        }

        private void ValidateExternalItem(DefinitionId id, DefinitionId displayName, string type)
        {
            if (displayName.Namespace != Mod.Id)
                throw new ModContentException(type + " display_name must belong to mod namespace '" + Mod.Id + "'.");
            if (displayName.Category != "localization")
                throw new ModContentException(type + " display_name must be a localization handle.");
            if (id.Namespace != Mod.Id) throw new ModContentException(type + " definition namespace mismatch.");
        }

        private void EnsureItemIdAvailable(DefinitionId id)
        {
            if (_itemRedirects.ContainsKey(id))
                throw new ModContentException("Item definition collides with a pending alias/tombstone: '" + id + "'.");
            ItemRedirectDefinition existing;
            if (_catalog.TryGetItemRedirect(id, out existing))
                throw new ModContentException("Item definition collides with an existing alias/tombstone: '" + id + "'.");
        }

        private void ValidateRedirectSource(DefinitionId id)
        {
            if (_itemRedirects.ContainsKey(id))
                throw new ModContentException("Duplicate item alias/tombstone: '" + id + "'.");
            ItemDefinition pending;
            if (TryGetPendingItem(id, out pending))
                throw new ModContentException("Item alias/tombstone collides with an item definition: '" + id + "'.");
            ItemDefinition existingItem;
            if (_catalog.TryGetItem(id, out existingItem))
                throw new ModContentException("Item alias/tombstone collides with an existing item definition: '" + id + "'.");
            ItemRedirectDefinition existingRedirect;
            if (_catalog.TryGetItemRedirect(id, out existingRedirect))
                throw new ModContentException("Item alias/tombstone already exists: '" + id + "'.");
        }

        private static bool SameItemKind(DefinitionId left, DefinitionId right)
        {
            return ItemKind(left) == ItemKind(right);
        }

        private static string ItemKind(DefinitionId id)
        {
            int slash = id.LocalId.IndexOf('/');
            return slash < 0 ? id.LocalId : id.LocalId.Substring(0, slash);
        }

        private static int MinimumVanillaProgressionLevel(ItemDefinition item)
        {
            if (item is WeaponDefinition) return 1;
            if (item is ArmorDefinition || item is HelmDefinition) return 2;
            if (item is RangedDefinition || item is MagicDefinition) return 6;
            throw new ModContentException("Unsupported equipment progression for '" + item.Id + "'.");
        }

        private bool TryGetPendingItem(DefinitionId id, out ItemDefinition value)
        {
            value = null;
            WeaponDefinition weapon;
            if (_weapons.TryGetValue(id, out weapon)) { value = weapon; return true; }
            ArmorDefinition armor;
            if (_armors.TryGetValue(id, out armor)) { value = armor; return true; }
            HelmDefinition helm;
            if (_helms.TryGetValue(id, out helm)) { value = helm; return true; }
            RangedDefinition ranged;
            if (_ranged.TryGetValue(id, out ranged)) { value = ranged; return true; }
            MagicDefinition magic;
            if (_magic.TryGetValue(id, out magic)) { value = magic; return true; }
            return false;
        }

        private static string ShopSectionPath(ModShopSection section)
        {
            switch (section)
            {
                case ModShopSection.Weapons: return "weapons";
                case ModShopSection.Armor: return "armor";
                case ModShopSection.Helmets: return "helmets";
                case ModShopSection.Ranged: return "ranged";
                case ModShopSection.Magic: return "magic";
                default: throw new ModContentException("Unsupported shop section: " + section);
            }
        }

        private static string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ModContentException("Localization language must not be empty.");
            string value = language.Trim().ToLowerInvariant();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '-'))
                    throw new ModContentException("Unsafe localization language '" + language + "'.");
            }
            return value;
        }

        private void ThrowIfCompleted()
        {
            if (_completed) throw new InvalidOperationException("Registration transaction is already completed.");
        }

        private void ClearPending()
        {
            _localizations.Clear();
            _weapons.Clear();
            _armors.Clear();
            _helms.Clear();
            _ranged.Clear();
            _magic.Clear();
            _itemRedirects.Clear();
            _shopListings.Clear();
            _listedItems.Clear();
        }
    }

    public sealed class ModContentException : Exception
    {
        public ModContentException(string message) : base(message) { }
        public ModContentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
