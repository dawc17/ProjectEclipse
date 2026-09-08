using System;
using System.Collections.Generic;
using System.Globalization;

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

    public enum ModPerkKind
    {
        Single = 0,
        Combo = 1
    }

    public enum ModEnchantmentRecipe
    {
        Simple = 0,
        Medium = 1,
        Complex = 2
    }

    public enum ModEquipmentKind
    {
        Weapon = 0,
        Armor = 1,
        Helm = 2,
        Ranged = 3,
        Magic = 4
    }

    public enum ModParameterType
    {
        Number = 0,
        Integer = 1,
        Boolean = 2,
        String = 3
    }

    public readonly struct ModParameterValue
    {
        public const long MaxSafeInteger = 9007199254740991L;
        public const long MinSafeInteger = -MaxSafeInteger;

        private readonly double _number;
        private readonly long _integer;
        private readonly bool _boolean;
        private readonly string _string;

        public ModParameterType Type { get; }
        public double Number => Type == ModParameterType.Number ? _number : throw TypeError(ModParameterType.Number);
        public long Integer => Type == ModParameterType.Integer ? _integer : throw TypeError(ModParameterType.Integer);
        public bool Boolean => Type == ModParameterType.Boolean ? _boolean : throw TypeError(ModParameterType.Boolean);
        public string String => Type == ModParameterType.String ? (_string ?? string.Empty) : throw TypeError(ModParameterType.String);

        private ModParameterValue(ModParameterType type, double number, long integer, bool boolean, string text)
        {
            Type = type;
            _number = number;
            _integer = integer;
            _boolean = boolean;
            _string = text;
        }

        public static ModParameterValue FromNumber(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Parameter number must be finite.");
            return new ModParameterValue(ModParameterType.Number, value, 0L, false, null);
        }

        public static ModParameterValue FromInteger(long value)
        {
            if (value < MinSafeInteger || value > MaxSafeInteger)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Parameter integer must stay within the exact Lua integer range.");
            return new ModParameterValue(ModParameterType.Integer, 0d, value, false, null);
        }

        public static ModParameterValue FromBoolean(bool value)
        {
            return new ModParameterValue(ModParameterType.Boolean, 0d, 0L, value, null);
        }

        public static ModParameterValue FromString(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length > ModParameterDefinition.MaxStringLength)
                throw new ArgumentOutOfRangeException(nameof(value), "Parameter string exceeds the maximum length.");
            return new ModParameterValue(ModParameterType.String, 0d, 0L, false, value);
        }

        public string ToWireString()
        {
            switch (Type)
            {
                case ModParameterType.Number: return _number.ToString("R", CultureInfo.InvariantCulture);
                case ModParameterType.Integer: return _integer.ToString(CultureInfo.InvariantCulture);
                case ModParameterType.Boolean: return _boolean ? "1" : "0";
                case ModParameterType.String: return _string ?? string.Empty;
                default: throw new InvalidOperationException("Unsupported parameter type: " + Type);
            }
        }

        public static bool TryParse(ModParameterType type, string text, out ModParameterValue value)
        {
            value = default;
            if (text == null) return false;
            switch (type)
            {
                case ModParameterType.Number:
                    double number;
                    if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number) ||
                        double.IsNaN(number) || double.IsInfinity(number)) return false;
                    value = FromNumber(number);
                    return true;
                case ModParameterType.Integer:
                    long integer;
                    if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out integer) ||
                        integer < MinSafeInteger || integer > MaxSafeInteger) return false;
                    value = FromInteger(integer);
                    return true;
                case ModParameterType.Boolean:
                    if (text == "1" || string.Equals(text, "true", StringComparison.OrdinalIgnoreCase))
                    { value = FromBoolean(true); return true; }
                    if (text == "0" || string.Equals(text, "false", StringComparison.OrdinalIgnoreCase))
                    { value = FromBoolean(false); return true; }
                    return false;
                case ModParameterType.String:
                    if (text.Length > ModParameterDefinition.MaxStringLength) return false;
                    value = FromString(text);
                    return true;
                default:
                    return false;
            }
        }

        private InvalidOperationException TypeError(ModParameterType expected)
        {
            return new InvalidOperationException("Parameter value is " + Type + ", expected " + expected + ".");
        }
    }

    public sealed class ModParameterDefinition
    {
        public const int MaxNameLength = 64;
        public const int MaxStringLength = 2048;

        public string Name { get; }
        public ModParameterType Type { get; }
        public bool Required { get; }
        public bool HasDefault { get; }
        public ModParameterValue DefaultValue { get; }

        public ModParameterDefinition(string name, ModParameterType type, bool required = false)
            : this(name, type, required, false, default)
        {
        }

        public ModParameterDefinition(string name, ModParameterType type, bool required, ModParameterValue defaultValue)
            : this(name, type, required, true, defaultValue)
        {
        }

        private ModParameterDefinition(string name, ModParameterType type, bool required, bool hasDefault,
            ModParameterValue defaultValue)
        {
            ValidateName(name);
            if (!Enum.IsDefined(typeof(ModParameterType), type))
                throw new ArgumentOutOfRangeException(nameof(type), "Unsupported parameter type.");
            if (hasDefault && defaultValue.Type != type)
                throw new ArgumentException("Parameter default type does not match schema type.", nameof(defaultValue));
            Name = name;
            Type = type;
            Required = required;
            HasDefault = hasDefault;
            DefaultValue = defaultValue;
        }

        internal static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > MaxNameLength)
                throw new ModContentException("Parameter name must be 1.." + MaxNameLength + " characters.");
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') || c == '_'))
                    throw new ModContentException("Unsafe parameter name '" + name + "'.");
            }
        }
    }

    public sealed class ModParameterSchema
    {
        public const int MaxParameters = 64;
        private readonly Dictionary<string, ModParameterDefinition> _byName;
        private readonly IReadOnlyList<ModParameterDefinition> _parameters;

        public IReadOnlyList<ModParameterDefinition> Parameters => _parameters;
        public int Count => _parameters.Count;

        public ModParameterSchema(IEnumerable<ModParameterDefinition> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            _byName = new Dictionary<string, ModParameterDefinition>(StringComparer.Ordinal);
            var values = new List<ModParameterDefinition>();
            foreach (ModParameterDefinition parameter in parameters)
            {
                if (parameter == null) throw new ArgumentException("Parameter schema contains null.", nameof(parameters));
                if (values.Count >= MaxParameters)
                    throw new ModContentException("Parameter schema limit exceeded (" + MaxParameters + ").");
                if (_byName.ContainsKey(parameter.Name))
                    throw new ModContentException("Duplicate parameter schema field '" + parameter.Name + "'.");
                _byName.Add(parameter.Name, parameter);
                values.Add(parameter);
            }
            _parameters = values.AsReadOnly();
        }

        public bool TryGet(string name, out ModParameterDefinition parameter)
        {
            return _byName.TryGetValue(name, out parameter);
        }

        public Dictionary<string, ModParameterValue> ResolveValues(
            IReadOnlyDictionary<string, ModParameterValue> supplied)
        {
            var result = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            if (supplied != null)
            {
                foreach (KeyValuePair<string, ModParameterValue> pair in supplied)
                {
                    ModParameterDefinition definition;
                    if (!TryGet(pair.Key, out definition))
                        throw new ModContentException("Unknown instance parameter '" + pair.Key + "'.");
                    if (pair.Value.Type != definition.Type)
                        throw new ModContentException("Instance parameter '" + pair.Key + "' has type " + pair.Value.Type +
                            ", expected " + definition.Type + ".");
                    result.Add(pair.Key, pair.Value);
                }
            }
            for (int i = 0; i < Parameters.Count; i++)
            {
                ModParameterDefinition parameter = Parameters[i];
                if (result.ContainsKey(parameter.Name)) continue;
                if (parameter.HasDefault) result.Add(parameter.Name, parameter.DefaultValue);
                else if (parameter.Required)
                    throw new ModContentException("Required instance parameter '" + parameter.Name + "' is missing.");
            }
            return result;
        }
    }

    public sealed class ModBehaviorDefinition
    {
        public DefinitionId Id { get; }
        public ModParameterSchema Parameters { get; }

        internal ModBehaviorDefinition(DefinitionId id, ModParameterSchema parameters)
        {
            Id = id;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }
    }

    // API 0.3 treats behavior as an independent executable definition. Perks and enchantments
    // may both reference the same behavior without referencing each other. The template-backed
    // fields remain only for API 0.2 compatibility with the recovered PerkInfoItem backend.
    public sealed class PerkDefinition
    {
        private readonly Dictionary<string, string> _parameters;
        private readonly IReadOnlyDictionary<string, ModParameterValue> _initialParameters;

        public DefinitionId Id { get; }
        public DefinitionId Template { get; }
        public bool HasTemplate { get; }
        public DefinitionId Behavior { get; }
        public bool HasBehavior { get; }
        public DefinitionId DisplayName { get; }
        public DefinitionId Description { get; }
        public AssetId Icon { get; }
        public bool HasIcon => !string.IsNullOrEmpty(Icon.Path);
        public ModPerkKind Kind { get; }
        public IReadOnlyDictionary<string, string> Parameters => _parameters;
        public IReadOnlyDictionary<string, ModParameterValue> InitialParameters => _initialParameters;
        public bool IsScripted => HasBehavior;
        public string LegacyName { get; }
        public string LegacyPerkXml { get; }
        public bool IsCore => Id.Namespace.Value == "core";

        internal PerkDefinition(DefinitionId id, DefinitionId template, bool hasTemplate,
            DefinitionId displayName, DefinitionId description, AssetId icon, ModPerkKind kind,
            IReadOnlyDictionary<string, string> parameters = null, string legacyName = null,
            string legacyPerkXml = null, DefinitionId behavior = default,
            IReadOnlyDictionary<string, ModParameterValue> initialParameters = null)
        {
            Id = id;
            Template = template;
            HasTemplate = hasTemplate;
            Behavior = behavior;
            HasBehavior = !string.IsNullOrEmpty(behavior.Category);
            DisplayName = displayName;
            Description = description;
            Icon = icon;
            Kind = kind;
            _parameters = new Dictionary<string, string>(StringComparer.Ordinal);
            if (parameters != null)
                foreach (KeyValuePair<string, string> pair in parameters) _parameters.Add(pair.Key, pair.Value);
            var copiedInitial = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            if (initialParameters != null)
                foreach (KeyValuePair<string, ModParameterValue> pair in initialParameters) copiedInitial.Add(pair.Key, pair.Value);
            _initialParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(copiedInitial);
            LegacyName = legacyName;
            LegacyPerkXml = legacyPerkXml;
        }
    }

    public sealed class EnchantmentDefinition
    {
        private readonly ModEquipmentKind[] _equipment;
        private readonly IReadOnlyList<ModEquipmentKind> _readOnlyEquipment;
        private readonly IReadOnlyDictionary<string, ModParameterValue> _initialParameters;

        public DefinitionId Id { get; }
        public DefinitionId Perk { get; }
        public bool HasPerk { get; }
        public DefinitionId Behavior { get; }
        public bool HasBehavior { get; }
        public DefinitionId DisplayName { get; }
        public DefinitionId Description { get; }
        public AssetId Icon { get; }
        public bool HasIcon => !string.IsNullOrEmpty(Icon.Path);
        public ModEnchantmentRecipe Recipe { get; }
        public IReadOnlyList<ModEquipmentKind> Equipment => _readOnlyEquipment;
        public IReadOnlyDictionary<string, ModParameterValue> InitialParameters => _initialParameters;
        public bool IsScripted => HasBehavior;
        public ModPerkKind Kind => Recipe == ModEnchantmentRecipe.Complex ? ModPerkKind.Combo : ModPerkKind.Single;

        internal EnchantmentDefinition(DefinitionId id, DefinitionId perk, ModEnchantmentRecipe recipe,
            ModEquipmentKind[] equipment)
        {
            Id = id;
            Perk = perk;
            HasPerk = true;
            Behavior = default;
            HasBehavior = false;
            DisplayName = default;
            Description = default;
            Icon = default;
            Recipe = recipe;
            _equipment = equipment == null ? Array.Empty<ModEquipmentKind>() : (ModEquipmentKind[])equipment.Clone();
            _readOnlyEquipment = Array.AsReadOnly(_equipment);
            _initialParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(
                new Dictionary<string, ModParameterValue>(StringComparer.Ordinal));
        }

        internal EnchantmentDefinition(DefinitionId id, DefinitionId displayName, DefinitionId description,
            AssetId icon, ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment,
            DefinitionId behavior, IReadOnlyDictionary<string, ModParameterValue> initialParameters)
        {
            Id = id;
            Perk = default;
            HasPerk = false;
            Behavior = behavior;
            HasBehavior = true;
            DisplayName = displayName;
            Description = description;
            Icon = icon;
            Recipe = recipe;
            _equipment = equipment == null ? Array.Empty<ModEquipmentKind>() : (ModEquipmentKind[])equipment.Clone();
            _readOnlyEquipment = Array.AsReadOnly(_equipment);
            var copiedInitial = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            if (initialParameters != null)
                foreach (KeyValuePair<string, ModParameterValue> pair in initialParameters) copiedInitial.Add(pair.Key, pair.Value);
            _initialParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(copiedInitial);
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
        private readonly DefinitionRegistry<PerkDefinition> _perks =
            new DefinitionRegistry<PerkDefinition>(value => value.Id);
        private readonly DefinitionRegistry<EnchantmentDefinition> _enchantments =
            new DefinitionRegistry<EnchantmentDefinition>(value => value.Id);
        private readonly DefinitionRegistry<ModBehaviorDefinition> _behaviors =
            new DefinitionRegistry<ModBehaviorDefinition>(value => value.Id);

        public bool IsFrozen { get; private set; }
        public IReadOnlyList<LocalizationDefinition> Localizations => _localizations.Values;
        public IReadOnlyList<WeaponDefinition> Weapons => _weapons.Values;
        public IReadOnlyList<ArmorDefinition> Armors => _armors.Values;
        public IReadOnlyList<HelmDefinition> Helms => _helms.Values;
        public IReadOnlyList<RangedDefinition> Ranged => _ranged.Values;
        public IReadOnlyList<MagicDefinition> Magic => _magic.Values;
        public IReadOnlyList<ItemRedirectDefinition> ItemRedirects => _itemRedirects.Values;
        public IReadOnlyList<ShopListingDefinition> ShopListings => _shopListings.Values;
        public IReadOnlyList<PerkDefinition> Perks => _perks.Values;
        public IReadOnlyList<EnchantmentDefinition> Enchantments => _enchantments.Values;
        public IReadOnlyList<ModBehaviorDefinition> Behaviors => _behaviors.Values;

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

        public bool TryGetPerk(DefinitionId id, out PerkDefinition value)
        {
            value = null;
            return id.Category == "perks" && _perks.TryGet(id, out value);
        }

        public bool TryGetEnchantment(DefinitionId id, out EnchantmentDefinition value)
        {
            value = null;
            return id.Category == "enchantments" && _enchantments.TryGet(id, out value);
        }

        public bool TryGetBehavior(DefinitionId id, out ModBehaviorDefinition value)
        {
            value = null;
            return id.Category == "behaviors" && _behaviors.TryGet(id, out value);
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        internal void Commit(ModRegistrationTransaction transaction,
            LocalizationDefinition[] localizations, WeaponDefinition[] weapons,
            ArmorDefinition[] armors, HelmDefinition[] helms, RangedDefinition[] ranged,
            MagicDefinition[] magic, ItemRedirectDefinition[] itemRedirects,
            ShopListingDefinition[] shopListings, PerkDefinition[] perks,
            EnchantmentDefinition[] enchantments, ModBehaviorDefinition[] behaviors)
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
            _perks.ValidateCanAdd(perks);
            _enchantments.ValidateCanAdd(enchantments);
            _behaviors.ValidateCanAdd(behaviors);

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

            for (int i = 0; i < perks.Length; i++)
            {
                PerkDefinition perk = perks[i];
                if (!ContainsLocalization(localizations, perk.DisplayName) &&
                    !_localizations.TryGet(perk.DisplayName, out LocalizationDefinition ignoredName))
                    throw new ModContentException("Perk '" + perk.Id +
                        "' references missing display localization '" + perk.DisplayName + "'.");
                if (!ContainsLocalization(localizations, perk.Description) &&
                    !_localizations.TryGet(perk.Description, out LocalizationDefinition ignoredDescription))
                    throw new ModContentException("Perk '" + perk.Id +
                        "' references missing description localization '" + perk.Description + "'.");
                if (perk.HasTemplate)
                {
                    PerkDefinition template;
                    if (!TryGetPendingPerk(perk.Template, perks, out template) && !_perks.TryGet(perk.Template, out template))
                        throw new ModContentException("Perk '" + perk.Id + "' references missing template '" +
                            perk.Template + "'.");
                    if (template.Id == perk.Id)
                        throw new ModContentException("Perk cannot derive from itself: '" + perk.Id + "'.");
                }
                else if (perk.HasBehavior)
                {
                    ModBehaviorDefinition behavior;
                    if (!TryGetPendingBehavior(perk.Behavior, behaviors, out behavior) &&
                        !_behaviors.TryGet(perk.Behavior, out behavior))
                        throw new ModContentException("Perk '" + perk.Id + "' references missing behavior '" +
                            perk.Behavior + "'.");
                }
                else if (!perk.IsCore)
                {
                    throw new ModContentException("External perk '" + perk.Id +
                        "' must either use the legacy template bridge or register Lua behavior.");
                }
            }

            for (int i = 0; i < enchantments.Length; i++)
            {
                EnchantmentDefinition enchantment = enchantments[i];
                if (enchantment.Equipment.Count == 0)
                    throw new ModContentException("Enchantment '" + enchantment.Id + "' has no equipment categories.");
                if (enchantment.HasPerk)
                {
                    PerkDefinition perk;
                    if (!TryGetPendingPerk(enchantment.Perk, perks, out perk) && !_perks.TryGet(enchantment.Perk, out perk))
                        throw new ModContentException("Enchantment '" + enchantment.Id + "' references missing perk '" +
                            enchantment.Perk + "'.");
                    if (enchantment.Recipe == ModEnchantmentRecipe.Complex && perk.Kind != ModPerkKind.Combo)
                        throw new ModContentException("Complex enchantment '" + enchantment.Id + "' requires a combo perk.");
                    if (enchantment.Recipe != ModEnchantmentRecipe.Complex && perk.Kind != ModPerkKind.Single)
                        throw new ModContentException("Simple/medium enchantment '" + enchantment.Id +
                            "' requires a single perk.");
                }
                else
                {
                    if (!enchantment.HasBehavior)
                        throw new ModContentException("Enchantment '" + enchantment.Id + "' has no behavior backend.");
                    ModBehaviorDefinition behavior;
                    if (!TryGetPendingBehavior(enchantment.Behavior, behaviors, out behavior) &&
                        !_behaviors.TryGet(enchantment.Behavior, out behavior))
                        throw new ModContentException("Enchantment '" + enchantment.Id +
                            "' references missing behavior '" + enchantment.Behavior + "'.");
                    if (!ContainsLocalization(localizations, enchantment.DisplayName) &&
                        !_localizations.TryGet(enchantment.DisplayName, out LocalizationDefinition ignoredEnchantName))
                        throw new ModContentException("Enchantment '" + enchantment.Id +
                            "' references missing display localization '" + enchantment.DisplayName + "'.");
                    if (!ContainsLocalization(localizations, enchantment.Description) &&
                        !_localizations.TryGet(enchantment.Description, out LocalizationDefinition ignoredEnchantDescription))
                        throw new ModContentException("Enchantment '" + enchantment.Id +
                            "' references missing description localization '" + enchantment.Description + "'.");
                }
            }

            _localizations.AddRange(localizations);
            _weapons.AddRange(weapons);
            _armors.AddRange(armors);
            _helms.AddRange(helms);
            _ranged.AddRange(ranged);
            _magic.AddRange(magic);
            _itemRedirects.AddRange(itemRedirects);
            _shopListings.AddRange(shopListings);
            _perks.AddRange(perks);
            _enchantments.AddRange(enchantments);
            _behaviors.AddRange(behaviors);
        }

        private static bool TryGetPendingPerk(DefinitionId id, PerkDefinition[] values, out PerkDefinition value)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i].Id == id) { value = values[i]; return true; }
            value = null;
            return false;
        }

        private static bool TryGetPendingBehavior(DefinitionId id, ModBehaviorDefinition[] values,
            out ModBehaviorDefinition value)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i].Id == id) { value = values[i]; return true; }
            value = null;
            return false;
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

        internal void ImportCorePerks(PerkDefinition[] perks)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            if (perks == null) throw new ArgumentNullException(nameof(perks));
            _perks.ValidateCanAdd(perks);
            for (int i = 0; i < perks.Length; i++)
                if (!perks[i].IsCore || perks[i].HasTemplate || string.IsNullOrEmpty(perks[i].LegacyName))
                    throw new ModContentException("Invalid core perk import: " + perks[i].Id);
            _perks.AddRange(perks);
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
        private readonly Dictionary<DefinitionId, PerkDefinition> _perks =
            new Dictionary<DefinitionId, PerkDefinition>();
        private readonly Dictionary<DefinitionId, EnchantmentDefinition> _enchantments =
            new Dictionary<DefinitionId, EnchantmentDefinition>();
        private readonly Dictionary<DefinitionId, ModBehaviorDefinition> _behaviors =
            new Dictionary<DefinitionId, ModBehaviorDefinition>();
        private readonly HashSet<DefinitionId> _listedItems = new HashSet<DefinitionId>();
        private bool _completed;

        public ModDescriptor Mod { get; }
        public int RegistrationCount => _localizations.Count + _weapons.Count + _armors.Count + _helms.Count +
            _ranged.Count + _magic.Count + _itemRedirects.Count + _shopListings.Count + _perks.Count +
            _enchantments.Count + _behaviors.Count;

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

        public PerkDefinition GetPerk(string reference)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference))
                throw new ModContentException("Perk reference must not be empty.");
            DefinitionId id;
            try
            {
                id = reference.IndexOf(':') >= 0 ? DefinitionId.Parse(reference) : Qualify("perks", reference);
            }
            catch (FormatException exception)
            {
                throw new ModContentException(exception.Message, exception);
            }
            if (id.Category != "perks")
                throw new ModContentException("Perk reference must use the 'perks' definition category: '" + id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot reference undeclared perk namespace '" +
                    id.Namespace + "'.");
            PerkDefinition value;
            if (_perks.TryGetValue(id, out value) || _catalog.TryGetPerk(id, out value)) return value;
            throw new ModContentException("Perk is not registered: '" + id + "'.");
        }

        public ModBehaviorDefinition RegisterBehavior(string localId, ModParameterSchema parameters)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("behaviors", localId);
            if (_behaviors.ContainsKey(id))
                throw new ModContentException("Duplicate behavior definition: '" + id + "'.");
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            EnsureCapacityForNewRegistration();
            var definition = new ModBehaviorDefinition(id, parameters);
            _behaviors.Add(id, definition);
            return definition;
        }

        public PerkDefinition RegisterPerk(string localId, DefinitionId template, DefinitionId displayName,
            DefinitionId description, AssetId icon, IReadOnlyDictionary<string, string> parameters)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("perks", localId);
            if (_perks.ContainsKey(id)) throw new ModContentException("Duplicate perk definition: '" + id + "'.");
            if (displayName.Namespace != Mod.Id || displayName.Category != "localization")
                throw new ModContentException("Perk display_name must be a localization owned by mod '" + Mod.Id + "'.");
            if (description.Namespace != Mod.Id || description.Category != "localization")
                throw new ModContentException("Perk description must be a localization owned by mod '" + Mod.Id + "'.");
            PerkDefinition templateDefinition;
            if (!_perks.TryGetValue(template, out templateDefinition) && !_catalog.TryGetPerk(template, out templateDefinition))
                throw new ModContentException("Perk template is not registered: '" + template + "'.");
            if (!CanReferenceNamespace(template.Namespace))
                throw new ModContentException("Perk template belongs to undeclared namespace '" + template.Namespace + "'.");
            if (!icon.Equals(default(AssetId)) && !CanReferenceNamespace(icon.Namespace))
                throw new ModContentException("Perk icon belongs to undeclared namespace '" + icon.Namespace + "'.");

            var copiedParameters = new Dictionary<string, string>(StringComparer.Ordinal);
            if (parameters != null)
            {
                if (parameters.Count > 64) throw new ModContentException("Perk parameter limit exceeded (64).");
                foreach (KeyValuePair<string, string> pair in parameters)
                {
                    ValidatePerkParameterName(pair.Key);
                    if (pair.Value == null || pair.Value.Length == 0 || pair.Value.Length > 2048)
                        throw new ModContentException("Perk parameter '" + pair.Key + "' must be 1..2048 characters.");
                    copiedParameters.Add(pair.Key, pair.Value);
                }
            }

            EnsureCapacityForNewRegistration();
            var definition = new PerkDefinition(id, templateDefinition.Id, true, displayName, description, icon,
                templateDefinition.Kind, copiedParameters);
            _perks.Add(id, definition);
            return definition;
        }

        public PerkDefinition RegisterScriptedPerk(string localId, DefinitionId displayName,
            DefinitionId description, AssetId icon, ModPerkKind kind, DefinitionId behavior,
            IReadOnlyDictionary<string, ModParameterValue> initialParameters)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("perks", localId);
            if (_perks.ContainsKey(id)) throw new ModContentException("Duplicate perk definition: '" + id + "'.");
            ValidateScriptedPresentation(displayName, description, icon, "Perk");
            if (!Enum.IsDefined(typeof(ModPerkKind), kind))
                throw new ModContentException("Unsupported perk kind: " + kind);
            ModBehaviorDefinition behaviorDefinition = RequirePendingBehavior(behavior, "Perk");
            Dictionary<string, ModParameterValue> initial = behaviorDefinition.Parameters.ResolveValues(initialParameters);

            EnsureCapacityForNewRegistration();
            var definition = new PerkDefinition(id, default, false, displayName, description, icon, kind,
                null, null, null, behaviorDefinition.Id, initial);
            _perks.Add(id, definition);
            return definition;
        }

        public EnchantmentDefinition RegisterEnchantment(string localId, DefinitionId perk,
            ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("enchantments", localId);
            if (_enchantments.ContainsKey(id))
                throw new ModContentException("Duplicate enchantment definition: '" + id + "'.");
            if (perk.Namespace != Mod.Id || perk.Category != "perks")
                throw new ModContentException("Mod API 0.2 enchantments may only expose perks owned by the registering mod.");
            PerkDefinition perkDefinition;
            if (!_perks.TryGetValue(perk, out perkDefinition))
                throw new ModContentException("Enchantment perk must be registered by the same transaction: '" + perk + "'.");
            if (!perkDefinition.HasTemplate)
                throw new ModContentException("Mod API 0.2 perk-backed enchantments require a template-backed perk. " +
                    "Behavior-backed enchantments must register their behavior directly.");
            if (!Enum.IsDefined(typeof(ModEnchantmentRecipe), recipe))
                throw new ModContentException("Unsupported enchantment recipe: " + recipe);
            if (equipment == null || equipment.Length == 0)
                throw new ModContentException("Enchantment must support at least one equipment category.");
            var seen = new HashSet<ModEquipmentKind>();
            var copied = new ModEquipmentKind[equipment.Length];
            for (int i = 0; i < equipment.Length; i++)
            {
                if (!Enum.IsDefined(typeof(ModEquipmentKind), equipment[i]))
                    throw new ModContentException("Unsupported enchantment equipment category: " + equipment[i]);
                if (!seen.Add(equipment[i]))
                    throw new ModContentException("Duplicate enchantment equipment category: " + equipment[i]);
                copied[i] = equipment[i];
            }
            if (recipe == ModEnchantmentRecipe.Complex && perkDefinition.Kind != ModPerkKind.Combo)
                throw new ModContentException("Complex enchantments require a combo perk.");
            if (recipe != ModEnchantmentRecipe.Complex && perkDefinition.Kind != ModPerkKind.Single)
                throw new ModContentException("Simple/medium enchantments require a single perk.");

            foreach (EnchantmentDefinition existing in _enchantments.Values)
            {
                if (existing.Perk != perkDefinition.Id || existing.Recipe != recipe) continue;
                for (int i = 0; i < copied.Length; i++)
                {
                    for (int j = 0; j < existing.Equipment.Count; j++)
                    {
                        if (copied[i] != existing.Equipment[j]) continue;
                        throw new ModContentException("Enchantment '" + id + "' duplicates perk candidate '" +
                            existing.Id + "' for recipe " + recipe + " and equipment " + copied[i] + ".");
                    }
                }
            }

            EnsureCapacityForNewRegistration();
            var definition = new EnchantmentDefinition(id, perkDefinition.Id, recipe, copied);
            _enchantments.Add(id, definition);
            return definition;
        }

        public EnchantmentDefinition RegisterScriptedEnchantment(string localId, DefinitionId displayName,
            DefinitionId description, AssetId icon, ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment,
            DefinitionId behavior, IReadOnlyDictionary<string, ModParameterValue> initialParameters)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("enchantments", localId);
            if (_enchantments.ContainsKey(id))
                throw new ModContentException("Duplicate enchantment definition: '" + id + "'.");
            ValidateScriptedPresentation(displayName, description, icon, "Enchantment");
            ValidateRecipeAndEquipment(recipe, equipment, out ModEquipmentKind[] copied);
            ModBehaviorDefinition behaviorDefinition = RequirePendingBehavior(behavior, "Enchantment");
            Dictionary<string, ModParameterValue> initial = behaviorDefinition.Parameters.ResolveValues(initialParameters);

            EnsureCapacityForNewRegistration();
            var definition = new EnchantmentDefinition(id, displayName, description, icon, recipe, copied,
                behaviorDefinition.Id, initial);
            _enchantments.Add(id, definition);
            return definition;
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
            var perks = new PerkDefinition[_perks.Count];
            _perks.Values.CopyTo(perks, 0);
            var enchantments = new EnchantmentDefinition[_enchantments.Count];
            _enchantments.Values.CopyTo(enchantments, 0);
            var behaviors = new ModBehaviorDefinition[_behaviors.Count];
            _behaviors.Values.CopyTo(behaviors, 0);

            _catalog.Commit(this, localizations, weapons, armors, helms, ranged, magic, itemRedirects, listings,
                perks, enchantments, behaviors);
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

        private bool CanReferenceNamespace(ModId namespaceId)
        {
            if (namespaceId == Mod.Id) return true;
            foreach (ModDependency dependency in Mod.Manifest.Dependencies)
                if (dependency.Id == namespaceId) return true;
            return false;
        }

        private static void ValidatePerkParameterName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 64)
                throw new ModContentException("Perk parameter name must be 1..64 characters.");
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_'))
                    throw new ModContentException("Unsafe perk parameter name '" + name + "'.");
            }
        }

        private void ValidateScriptedPresentation(DefinitionId displayName, DefinitionId description,
            AssetId icon, string type)
        {
            if (displayName.Namespace != Mod.Id || displayName.Category != "localization")
                throw new ModContentException(type + " display_name must be a localization owned by mod '" + Mod.Id + "'.");
            if (description.Namespace != Mod.Id || description.Category != "localization")
                throw new ModContentException(type + " description must be a localization owned by mod '" + Mod.Id + "'.");
            if (!icon.Equals(default(AssetId)) && !CanReferenceNamespace(icon.Namespace))
                throw new ModContentException(type + " icon belongs to undeclared namespace '" + icon.Namespace + "'.");
        }

        private ModBehaviorDefinition RequirePendingBehavior(DefinitionId behavior, string ownerType)
        {
            if (behavior.Namespace != Mod.Id || behavior.Category != "behaviors")
                throw new ModContentException(ownerType + " behavior must be owned by the registering mod.");
            ModBehaviorDefinition definition;
            if (!_behaviors.TryGetValue(behavior, out definition))
                throw new ModContentException(ownerType + " behavior must be registered by the same transaction: '" +
                    behavior + "'.");
            return definition;
        }

        private static void ValidateRecipeAndEquipment(ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment,
            out ModEquipmentKind[] copied)
        {
            if (!Enum.IsDefined(typeof(ModEnchantmentRecipe), recipe))
                throw new ModContentException("Unsupported enchantment recipe: " + recipe);
            if (equipment == null || equipment.Length == 0)
                throw new ModContentException("Enchantment must support at least one equipment category.");
            var seen = new HashSet<ModEquipmentKind>();
            copied = new ModEquipmentKind[equipment.Length];
            for (int i = 0; i < equipment.Length; i++)
            {
                if (!Enum.IsDefined(typeof(ModEquipmentKind), equipment[i]))
                    throw new ModContentException("Unsupported enchantment equipment category: " + equipment[i]);
                if (!seen.Add(equipment[i]))
                    throw new ModContentException("Duplicate enchantment equipment category: " + equipment[i]);
                copied[i] = equipment[i];
            }
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
            _perks.Clear();
            _enchantments.Clear();
            _behaviors.Clear();
            _listedItems.Clear();
        }
    }

    public sealed class ModContentException : Exception
    {
        public ModContentException(string message) : base(message) { }
        public ModContentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
