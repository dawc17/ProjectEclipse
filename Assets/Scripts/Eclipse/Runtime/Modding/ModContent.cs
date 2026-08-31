using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModShopSection
    {
        Weapons = 0
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

    public sealed class WeaponDefinition
    {
        public DefinitionId Id { get; }
        public DefinitionId DisplayName { get; }
        public AssetId Icon { get; }
        public AssetId Model { get; }
        public string SubType { get; }
        public int Damage { get; }

        internal WeaponDefinition(DefinitionId id, DefinitionId displayName, AssetId icon, AssetId model,
            string subType, int damage)
        {
            Id = id;
            DisplayName = displayName;
            Icon = icon;
            Model = model;
            SubType = subType;
            Damage = damage;
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
        private readonly DefinitionRegistry<ShopListingDefinition> _shopListings =
            new DefinitionRegistry<ShopListingDefinition>(value => value.Id);

        public bool IsFrozen { get; private set; }
        public IReadOnlyList<LocalizationDefinition> Localizations => _localizations.Values;
        public IReadOnlyList<WeaponDefinition> Weapons => _weapons.Values;
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
            ShopListingDefinition[] shopListings)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");

            _localizations.ValidateCanAdd(localizations);
            _weapons.ValidateCanAdd(weapons);
            _shopListings.ValidateCanAdd(shopListings);

            for (int i = 0; i < weapons.Length; i++)
            {
                LocalizationDefinition ignored;
                if (!ContainsLocalization(localizations, weapons[i].DisplayName) &&
                    !_localizations.TryGet(weapons[i].DisplayName, out ignored))
                    throw new ModContentException("Weapon '" + weapons[i].Id +
                        "' references missing localization '" + weapons[i].DisplayName + "'.");
            }

            for (int i = 0; i < shopListings.Length; i++)
            {
                WeaponDefinition ignored;
                if (!ContainsWeapon(weapons, shopListings[i].Item) &&
                    !_weapons.TryGet(shopListings[i].Item, out ignored))
                    throw new ModContentException("Shop listing '" + shopListings[i].Id +
                        "' references missing weapon '" + shopListings[i].Item + "'.");
            }

            _localizations.AddRange(localizations);
            _weapons.AddRange(weapons);
            _shopListings.AddRange(shopListings);
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

        private readonly ModContentCatalog _catalog;
        private readonly Dictionary<DefinitionId, Dictionary<string, string>> _localizations =
            new Dictionary<DefinitionId, Dictionary<string, string>>();
        private readonly Dictionary<DefinitionId, WeaponDefinition> _weapons =
            new Dictionary<DefinitionId, WeaponDefinition>();
        private readonly Dictionary<DefinitionId, ShopListingDefinition> _shopListings =
            new Dictionary<DefinitionId, ShopListingDefinition>();
        private readonly HashSet<DefinitionId> _listedItems = new HashSet<DefinitionId>();
        private bool _completed;

        public ModDescriptor Mod { get; }
        public int RegistrationCount => _localizations.Count + _weapons.Count + _shopListings.Count;

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
            AssetId model, string subType, int damage)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("items", "weapon/" + localId);
            if (displayName.Namespace != Mod.Id)
                throw new ModContentException("Weapon display_name must belong to mod namespace '" + Mod.Id + "'.");
            if (displayName.Category != "localization")
                throw new ModContentException("Weapon display_name must be a localization handle.");
            if (string.IsNullOrWhiteSpace(subType))
                throw new ModContentException("Weapon subtype must not be empty.");
            if (damage <= 0 || damage > 1000000)
                throw new ModContentException("Weapon damage must be within 1..1000000.");
            if (_weapons.ContainsKey(id))
                throw new ModContentException("Duplicate weapon definition: '" + id + "'.");

            EnsureCapacityForNewRegistration();
            var definition = new WeaponDefinition(id, displayName, icon, model, subType.Trim(), damage);
            _weapons.Add(id, definition);
            return definition;
        }

        public ShopListingDefinition RegisterShopListing(DefinitionId item, ModShopSection section,
            int level, ModPrice price)
        {
            ThrowIfCompleted();
            if (item.Category != "items" || !item.LocalId.StartsWith("weapon/", StringComparison.Ordinal))
                throw new ModContentException("Shop item must be a weapon definition.");
            if (item.Namespace != Mod.Id)
                throw new ModContentException("Mod API v1 shop listings may only expose items owned by the registering mod.");
            if (section != ModShopSection.Weapons)
                throw new ModContentException("Only the weapon shop section is supported by the first content slice.");
            if (level < 1 || level > 10000)
                throw new ModContentException("Shop level must be within 1..10000.");
            if (!_listedItems.Add(item))
                throw new ModContentException("Weapon already has a shop listing: '" + item + "'.");

            string local = item.LocalId.Substring("weapon/".Length);
            DefinitionId id = Qualify("shop", "weapons/" + local);
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
            var listings = new ShopListingDefinition[_shopListings.Count];
            _shopListings.Values.CopyTo(listings, 0);

            _catalog.Commit(this, localizations, weapons, listings);
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
