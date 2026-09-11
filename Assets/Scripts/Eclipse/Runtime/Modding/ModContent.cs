using System;
using System.Collections.Generic;
using System.Globalization;

namespace Eclipse.Modding
{
    public enum ModContentPatchOperation
    {
        Replace = 0,
        Append = 1,
        Remove = 2
    }

    public enum ModContentFieldPolicy
    {
        ReadOnly = 0,
        BaseOnly = 1,
        Replaceable = 2,
        Appendable = 3,
        Removable = 4,
        Mergeable = 5
    }

    public sealed class ModContentPatchRecord
    {
        public ModId Owner { get; }
        public DefinitionId Target { get; }
        public string Field { get; }
        public ModContentPatchOperation Operation { get; }

        internal ModContentPatchRecord(ModId owner, DefinitionId target, string field,
            ModContentPatchOperation operation)
        {
            Owner = owner;
            Target = target;
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Operation = operation;
        }
    }

    internal readonly struct ModContentPatchKey : IEquatable<ModContentPatchKey>
    {
        public DefinitionId Target { get; }
        public string Field { get; }

        public ModContentPatchKey(DefinitionId target, string field)
        {
            Target = target;
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public bool Equals(ModContentPatchKey other)
        {
            return Target == other.Target && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ModContentPatchKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Target.GetHashCode() * 397) ^ StringComparer.Ordinal.GetHashCode(Field);
            }
        }
    }

    internal sealed class LocalizationValuePatch
    {
        public ModContentPatchRecord Record { get; }
        public string Language { get; }
        public string Value { get; }

        public LocalizationValuePatch(ModContentPatchRecord record, string language, string value)
        {
            Record = record ?? throw new ArgumentNullException(nameof(record));
            Language = language ?? throw new ArgumentNullException(nameof(language));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    internal sealed class FightFieldPatch
    {
        public ModContentPatchRecord Record { get; }
        public string StringValue { get; }
        public int IntValue { get; }
        public DefinitionId[] Rules { get; }
        public bool AppendRules { get; }

        public FightFieldPatch(ModContentPatchRecord record, string stringValue, int intValue, DefinitionId[] rules = null, bool appendRules = false)
        {
            Record = record ?? throw new ArgumentNullException(nameof(record));
            StringValue = stringValue;
            IntValue = intValue;
            Rules = rules == null ? null : (DefinitionId[])rules.Clone();
            AppendRules = appendRules;
        }
    }

    // Semantic field policy is deliberately centralized. New public patch adapters must map
    // their typed fields here before they can mutate committed content. Unknown fields are
    // read-only by default, while all economy/* fields are permanently base-owned.
    internal static class ModContentPolicies
    {
        public const string EconomyItemPrice = "economy/item-price";
        public const string EconomyUpgradeCost = "economy/upgrade-cost";
        public const string EconomyForgeCost = "economy/forge-cost";
        public const string EconomyEnchantmentCost = "economy/enchantment-cost";
        public const string EconomySkipCost = "economy/skip-cost";
        public const string EconomyCurrencyValue = "economy/currency-value";
        public const string EconomyCurrencyFormula = "economy/currency-formula";
        public const string EconomyBalanceTable = "economy/balance-table";

        public static string LocalizationValue(string language)
        {
            if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
            return "values/" + language;
        }

        public const string FightRules = "fight/rules";
        public const string FightLocation = "fight/location";
        public const string FightMusic = "fight/music";
        public const string FightDescription = "fight/description";
        public const string FightRounds = "fight/rounds";
        public const string FightRoundTime = "fight/round-time";
        public const string ZoneBattleChildren = "children/battles/";

        public static ModContentFieldPolicy GetFieldPolicy(DefinitionId target, string field)
        {
            if (string.IsNullOrEmpty(field)) return ModContentFieldPolicy.ReadOnly;
            if (field.StartsWith("economy/", StringComparison.Ordinal)) return ModContentFieldPolicy.BaseOnly;
            if (target.Category == "localization" && field.StartsWith("values/", StringComparison.Ordinal))
                return ModContentFieldPolicy.Replaceable;
            if (target.Category == "fights" &&
                (field == FightDescription || field == FightRounds || field == FightRoundTime ||
                 field == FightRules || field == FightLocation || field == FightMusic))
                return ModContentFieldPolicy.Replaceable;
            if (target.Category == "zones" && field.StartsWith(ZoneBattleChildren, StringComparison.Ordinal))
                return ModContentFieldPolicy.Appendable;
            return ModContentFieldPolicy.ReadOnly;
        }

        public static void RequirePatchAllowed(DefinitionId target, string field, ModContentPatchOperation operation)
        {
            ModContentFieldPolicy policy = GetFieldPolicy(target, field);
            if (policy == ModContentFieldPolicy.BaseOnly)
                throw new ModContentException("Field '" + field + "' on '" + target +
                    "' is base-only. The shared Eclipse economy cannot be modified by mods.");
            if (policy == ModContentFieldPolicy.ReadOnly)
                throw new ModContentException("Field '" + field + "' on '" + target + "' is read-only.");

            bool allowed = operation == ModContentPatchOperation.Replace
                ? policy == ModContentFieldPolicy.Replaceable || policy == ModContentFieldPolicy.Mergeable
                : operation == ModContentPatchOperation.Append
                    ? policy == ModContentFieldPolicy.Appendable || policy == ModContentFieldPolicy.Mergeable
                    : operation == ModContentPatchOperation.Remove &&
                        (policy == ModContentFieldPolicy.Removable || policy == ModContentFieldPolicy.Mergeable);
            if (!allowed)
                throw new ModContentException("Patch operation " + operation + " is not allowed for field '" +
                    field + "' on '" + target + "' (policy " + policy + ").");
        }
    }

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
        // Exact recovered lookup key for projected core localization. Mod-owned localization
        // definitions have no legacy key. This is a read-only identity bridge, not raw XML.
        public string LegacyKey { get; }

        internal LocalizationDefinition(DefinitionId id, Dictionary<string, string> values, string legacyKey = null)
        {
            Id = id;
            _values = new Dictionary<string, string>(values ?? throw new ArgumentNullException(nameof(values)),
                StringComparer.Ordinal);
            LegacyKey = legacyKey;
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

        public static void ValidateName(string name)
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
        public ModParameterSchema StateSchema { get; }
        public string StateLifetime { get; }
        public int StateVersion { get; }

        internal ModBehaviorDefinition(DefinitionId id, ModParameterSchema parameters,
            ModParameterSchema state = null, string lifetime = "fight", int version = 1)
        {
            Id = id;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            if (lifetime != "fight" && lifetime != "round" && lifetime != "saved") throw new ModContentException("Invalid behavior state lifetime.");
            if (version < 1) throw new ModContentException("State version must be positive.");
            StateSchema = state; StateLifetime = lifetime; StateVersion = version;
        }
    }

    // API 0.3 treats behavior as an independent executable definition. Perks and enchantments
    // may both reference the same behavior without referencing each other. The template-backed
    // fields remain only for API 0.2 compatibility with the recovered PerkInfoItem backend.
    public sealed class PerkUpgradeDefinition
    {
        public int Level { get; }
        public DefinitionId Description { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }
        public IReadOnlyDictionary<string, ModParameterValue> TypedParameters { get; }
        public PerkUpgradeDefinition(int level, DefinitionId description,
            IReadOnlyDictionary<string, string> parameters = null,
            IReadOnlyDictionary<string, ModParameterValue> typedParameters = null)
        {
            if (level < 1 || level > 100) throw new ModContentException("Perk upgrade level must be 1..100.");
            Level = level; Description = description;
            var native = new Dictionary<string, string>(StringComparer.Ordinal);
            if (parameters != null) foreach (var pair in parameters) native.Add(pair.Key, pair.Value);
            var typed = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            if (typedParameters != null) foreach (var pair in typedParameters) typed.Add(pair.Key, pair.Value);
            Parameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(native);
            TypedParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(typed);
        }
    }

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
        public IReadOnlyList<PerkUpgradeDefinition> Upgrades { get; }
        public bool IsCore => Id.Namespace.Value == "core";

        internal PerkDefinition(DefinitionId id, DefinitionId template, bool hasTemplate,
            DefinitionId displayName, DefinitionId description, AssetId icon, ModPerkKind kind,
            IReadOnlyDictionary<string, string> parameters = null, string legacyName = null,
            string legacyPerkXml = null, DefinitionId behavior = default,
            IReadOnlyDictionary<string, ModParameterValue> initialParameters = null, PerkUpgradeDefinition[] upgrades = null)
        {
            Upgrades = Array.AsReadOnly(upgrades == null ? Array.Empty<PerkUpgradeDefinition>() : (PerkUpgradeDefinition[])upgrades.Clone());
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
        internal PerkDefinition WithUpgrades(PerkUpgradeDefinition[] upgrades) => new PerkDefinition(
            Id, Template, HasTemplate, DisplayName, Description, Icon, Kind, Parameters, LegacyName,
            LegacyPerkXml, Behavior, InitialParameters, upgrades);

        public Dictionary<string, ModParameterValue> ResolveSavedUpgradeParameters(System.Xml.XmlNode node,
            IReadOnlyDictionary<string, ModParameterValue> saved)
        {
            // Existing perks without an upgrade table keep their historical parameter contract.
            if (Upgrades.Count == 0) return ResolveUpgradeParameters(0, saved);
            int level = 0;
            string text = node?.Attributes?["UpgradeLevel"]?.Value;
            if (!string.IsNullOrEmpty(text) && (!int.TryParse(text, System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture, out level) || level < 0))
                throw new ModContentException("Invalid saved perk upgrade level. Saved data preserved.");
            return ResolveUpgradeParameters(level, saved);
        }

        public Dictionary<string, ModParameterValue> ResolveUpgradeParameters(int level,
            IReadOnlyDictionary<string, ModParameterValue> saved)
        {
            if (level < 0 || (Upgrades.Count > 0 && level > Upgrades.Count))
                throw new ModContentException("Unsupported saved perk upgrade level: " + level + ". Saved data preserved.");
            var result = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
            foreach (var pair in saved ?? InitialParameters) result.Add(pair.Key, pair.Value);
            if (Upgrades.Count > 0 && level > 0)
                foreach (var pair in Upgrades[level - 1].TypedParameters) result[pair.Key] = pair.Value;
            return result;
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

    public enum ModBattleKind
    {
        Dummy = 0,
        Tutorial = 1,
        Challenge = 2,
        Bosses = 3,
        Tournament = 4,
        Story = 5,
        Survival = 6,
        Friendly = 7,
        Auto = 8,
        Ai = 9,
        Hidden = 10,
        Fake = 11,
        Pvp = 12,
        Final = 13,
        FinalTitan = 14,
        Periodic = 15,
        Replayable = 16,
        BossesReplayable = 17,
        FinalReplayable = 18,
        BossesIntermission = 19,
        Ascension = 20,
        Raid = 21,
    }

    public enum ModFightRuleKind
    {
        RequireItem = 0,
        NoPerks = 1,
        EquipItem = 2,
        Avatar = 3,
        Name = 4,
        Perk = 5,
        RechargeMagicEachRound = 6,
        Attributes = 7,
        NoButton = 8,
        Behavior = 9,
    }

    public enum ModRuleTarget
    {
        Player = 0,
        Opponent = 1,
        All = 2,
    }

    public enum ModRuleMode
    {
        All = 0,
        Normal = 1,
        Eclipse = 2,
    }

    public sealed class ZoneDefinition
    {
        private readonly DefinitionId[] _battles;
        private readonly IReadOnlyList<DefinitionId> _readOnlyBattles;

        public DefinitionId Id { get; }
        public string LegacyName { get; }
        public string FileName { get; }
        public bool IsStart { get; }
        public IReadOnlyList<DefinitionId> Battles => _readOnlyBattles;
        public bool IsCore => Id.Namespace.Value == "core";

        internal ZoneDefinition(DefinitionId id, string legacyName, string fileName, bool isStart,
            DefinitionId[] battles)
        {
            Id = id;
            LegacyName = legacyName ?? string.Empty;
            FileName = fileName ?? string.Empty;
            IsStart = isStart;
            _battles = battles == null ? Array.Empty<DefinitionId>() : (DefinitionId[])battles.Clone();
            _readOnlyBattles = Array.AsReadOnly(_battles);
        }
    }

    public sealed class BattleDefinition
    {
        private readonly DefinitionId[] _fights;
        private readonly IReadOnlyList<DefinitionId> _readOnlyFights;

        public DefinitionId Id { get; }
        public DefinitionId Zone { get; }
        public string LegacyName { get; }
        public ModBattleKind Kind { get; }
        public int X { get; }
        public int Y { get; }
        public string Alias { get; }
        public string Title { get; }
        public string Icon { get; }
        public string IconAtlas { get; }
        public string EclipseToggleName { get; }
        public string Preview { get; }
        public string Description { get; }
        public string Location { get; }
        public string Music { get; }
        public string RewardImage { get; }
        public bool ShowResistance { get; }
        public IReadOnlyList<DefinitionId> Fights => _readOnlyFights;
        public bool IsCore => Id.Namespace.Value == "core";
        internal string LegacyXml { get; }

        internal BattleDefinition(DefinitionId id, DefinitionId zone, string legacyName, ModBattleKind kind,
            int x, int y, string alias, string title, string icon, string preview, string description,
            string location, string music, string rewardImage, bool showResistance, DefinitionId[] fights,
            string iconAtlas = null, string eclipseToggleName = null,
            string legacyXml = null)
        {
            Id = id;
            Zone = zone;
            LegacyName = legacyName ?? string.Empty;
            Kind = kind;
            X = x;
            Y = y;
            Alias = alias ?? string.Empty;
            Title = title ?? string.Empty;
            Icon = icon ?? string.Empty;
            IconAtlas = iconAtlas ?? string.Empty;
            EclipseToggleName = eclipseToggleName ?? string.Empty;
            Preview = preview ?? string.Empty;
            Description = description ?? string.Empty;
            Location = location ?? string.Empty;
            Music = music ?? string.Empty;
            RewardImage = rewardImage ?? string.Empty;
            ShowResistance = showResistance;
            _fights = fights == null ? Array.Empty<DefinitionId>() : (DefinitionId[])fights.Clone();
            _readOnlyFights = Array.AsReadOnly(_fights);
            LegacyXml = legacyXml;
        }
    }

    public sealed class FightDefinition
    {
        private readonly DefinitionId[] _warriors;
        private readonly DefinitionId[] _rules;
        private readonly DefinitionId[] _rewards;

        public DefinitionId Id { get; }
        public DefinitionId Battle { get; }
        public string LegacyName { get; }
        public int Replays { get; }
        public int ReplayInterval { get; }
        public int Power { get; }
        public int Rounds { get; }
        public int RoundTime { get; }
        public string Location { get; }
        public string Music { get; }
        public float EvaluatedRating { get; }
        public float HealthRecovery { get; }
        public string Description { get; }
        public bool Locked { get; }
        public string RewardImage { get; }
        public IReadOnlyList<DefinitionId> Warriors => Array.AsReadOnly(_warriors);
        public IReadOnlyList<DefinitionId> Rules => Array.AsReadOnly(_rules);
        public IReadOnlyList<DefinitionId> Rewards => Array.AsReadOnly(_rewards);
        public bool IsCore => Id.Namespace.Value == "core";
        internal string LegacyXml { get; }
        public bool ReplacesLegacyRules { get; }

        internal FightDefinition(DefinitionId id, DefinitionId battle, string legacyName, int replays,
            int replayInterval, int power, int rounds, int roundTime, string location, string music,
            float evaluatedRating, float healthRecovery, string description, bool locked, string rewardImage,
            DefinitionId[] warriors, DefinitionId[] rules, DefinitionId[] rewards, string legacyXml = null, bool replacesLegacyRules = false)
        {
            if (replays < 0) throw new ModContentException("Fight replays must not be negative.");
            if (replayInterval < 0) throw new ModContentException("Fight replay interval must not be negative.");
            if (power < 0) throw new ModContentException("Fight power must not be negative.");
            if (rounds < 1 || rounds > 100) throw new ModContentException("Fight rounds must be 1..100.");
            if (roundTime < 1 || roundTime > 86400) throw new ModContentException("Fight round time must be 1..86400 seconds.");
            if (float.IsNaN(evaluatedRating) || float.IsInfinity(evaluatedRating))
                throw new ModContentException("Fight evaluated rating must be finite.");
            if (float.IsNaN(healthRecovery) || float.IsInfinity(healthRecovery) || healthRecovery < 0f)
                throw new ModContentException("Fight health recovery must be finite and non-negative.");
            Id = id;
            Battle = battle;
            LegacyName = legacyName ?? string.Empty;
            Replays = replays;
            ReplayInterval = replayInterval;
            Power = power;
            Rounds = rounds;
            RoundTime = roundTime;
            Location = location ?? string.Empty;
            Music = music ?? string.Empty;
            EvaluatedRating = evaluatedRating;
            HealthRecovery = healthRecovery;
            Description = description ?? string.Empty;
            Locked = locked;
            RewardImage = rewardImage ?? string.Empty;
            _warriors = warriors == null ? Array.Empty<DefinitionId>() : (DefinitionId[])warriors.Clone();
            _rules = rules == null ? Array.Empty<DefinitionId>() : (DefinitionId[])rules.Clone();
            _rewards = rewards == null ? Array.Empty<DefinitionId>() : (DefinitionId[])rewards.Clone();
            LegacyXml = legacyXml;
            ReplacesLegacyRules = replacesLegacyRules;
        }

        internal FightDefinition WithDescription(string description)
        {
            return new FightDefinition(Id, Battle, LegacyName, Replays, ReplayInterval, Power, Rounds, RoundTime,
                Location, Music, EvaluatedRating, HealthRecovery, description, Locked, RewardImage,
                _warriors, _rules, _rewards, LegacyXml, ReplacesLegacyRules);
        }

        internal FightDefinition WithRounds(int rounds)
        {
            return new FightDefinition(Id, Battle, LegacyName, Replays, ReplayInterval, Power, rounds, RoundTime,
                Location, Music, EvaluatedRating, HealthRecovery, Description, Locked, RewardImage,
                _warriors, _rules, _rewards, LegacyXml, ReplacesLegacyRules);
        }

        internal FightDefinition WithRoundTime(int roundTime)
        {
            return new FightDefinition(Id, Battle, LegacyName, Replays, ReplayInterval, Power, Rounds, roundTime,
                Location, Music, EvaluatedRating, HealthRecovery, Description, Locked, RewardImage,
                _warriors, _rules, _rewards, LegacyXml, ReplacesLegacyRules);
        }
        internal FightDefinition WithPresentation(string location, string music)
        {
            return new FightDefinition(Id, Battle, LegacyName, Replays, ReplayInterval, Power, Rounds, RoundTime,
                location, music, EvaluatedRating, HealthRecovery, Description, Locked, RewardImage,
                _warriors, _rules, _rewards, LegacyXml, ReplacesLegacyRules);
        }

        internal FightDefinition WithRules(DefinitionId[] rules, bool append)
        {
            var combined = append ? new List<DefinitionId>(_rules) : new List<DefinitionId>();
            foreach (var rule in rules)
            {
                if (combined.Contains(rule)) throw new ModContentException("Fight rule is already attached: '" + rule + "'.");
                combined.Add(rule);
            }
            if (combined.Count > 100) throw new ModContentException("A patched fight supports at most 100 rule handles.");
            return new FightDefinition(Id, Battle, LegacyName, Replays, ReplayInterval, Power, Rounds, RoundTime,
                Location, Music, EvaluatedRating, HealthRecovery, Description, Locked, RewardImage,
                _warriors, combined.ToArray(), _rewards, LegacyXml, ReplacesLegacyRules || !append);
        }
    }

    // Shared with the recovered adapter so projection can be verified without loading Unity.
    public static class ModFightPatchProjection
    {
        public static void Apply(System.Xml.XmlElement node, FightDefinition fight, string field,
            ModContentCatalog content, Func<FightRuleDefinition, System.Xml.XmlElement> buildRule)
        {
            if (node == null || fight == null || content == null) throw new ArgumentNullException(nameof(node));
            if (field == ModContentPolicies.FightDescription) node.SetAttribute("Description", fight.Description);
            else if (field == ModContentPolicies.FightRounds) node.SetAttribute("Rounds", fight.Rounds.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (field == ModContentPolicies.FightRoundTime) node.SetAttribute("RoundTime", fight.RoundTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else if (field == ModContentPolicies.FightLocation) node.SetAttribute("Location", fight.Location);
            else if (field == ModContentPolicies.FightMusic) node.SetAttribute("Music", fight.Music);
            else if (field == ModContentPolicies.FightRules)
            {
                if (fight.ReplacesLegacyRules)
                {
                    var oldRules = node.SelectNodes("Rules");
                    foreach (System.Xml.XmlNode old in oldRules) node.RemoveChild(old);
                }
                var rules = node.SelectSingleNode("Rules") as System.Xml.XmlElement;
                if (rules == null)
                {
                    rules = node.OwnerDocument.CreateElement("Rules");
                    node.AppendChild(rules);
                }
                foreach (var id in fight.Rules)
                {
                    if (!content.TryGetFightRule(id, out var rule)) throw new ModContentException("Missing fight rule '" + id + "'.");
                    if (rule.Kind != ModFightRuleKind.Behavior) rules.AppendChild(buildRule(rule));
                }
            }
            else throw new ModContentException("Unsupported committed fight patch field '" + field + "'.");
        }
    }

    public sealed class WarriorDefinition
    {
        private readonly DefinitionId[] _items;
        private readonly DefinitionId[] _perks;
        private readonly Dictionary<string, float> _attributes;
        private readonly WarriorAttributeAlignmentDefinition[] _attributeAlignments;
        public DefinitionId Id { get; }
        public DefinitionId Template { get; }
        public bool HasTemplate { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Avatar { get; }
        public string Voice { get; }
        public int Level { get; }
        public string Tactic { get; }
        public int HealthBars { get; }
        public string Group { get; }
        public int Random { get; }
        public IReadOnlyDictionary<string, float> Attributes => _attributes;
        public IReadOnlyList<WarriorAttributeAlignmentDefinition> AttributeAlignments =>
            Array.AsReadOnly(_attributeAlignments);
        public IReadOnlyList<DefinitionId> Items => Array.AsReadOnly(_items);
        public IReadOnlyList<DefinitionId> Perks => Array.AsReadOnly(_perks);

        internal WarriorDefinition(DefinitionId id, string firstName, string lastName, string avatar, string voice,
            int level, string tactic, DefinitionId[] items, DefinitionId[] perks,
            DefinitionId template = default(DefinitionId), bool hasTemplate = false, string group = null,
            int random = 0, IReadOnlyDictionary<string, float> attributes = null,
            WarriorAttributeAlignmentDefinition[] attributeAlignments = null, int healthBars = 0)
        {
            if (level < 0 || level > 10000) throw new ModContentException("Warrior level must be 0..10000.");
            Id = id;
            Template = template;
            HasTemplate = hasTemplate;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Avatar = avatar ?? string.Empty;
            Voice = voice ?? string.Empty;
            Level = level;
            Tactic = tactic ?? string.Empty;
            if (healthBars < 0 || healthBars > 10000) throw new ModContentException("Warrior health_bars must be 0..10000 (zero inherits the template).");
            HealthBars = healthBars;
            Group = group ?? string.Empty;
            Random = random;
            _attributes = new Dictionary<string, float>(StringComparer.Ordinal);
            if (attributes != null)
                foreach (KeyValuePair<string, float> pair in attributes)
                {
                    if (string.IsNullOrWhiteSpace(pair.Key) || float.IsNaN(pair.Value) || float.IsInfinity(pair.Value))
                        throw new ModContentException("Warrior attributes require non-empty names and finite values.");
                    _attributes.Add(pair.Key, pair.Value);
                }
            _attributeAlignments = attributeAlignments == null ? Array.Empty<WarriorAttributeAlignmentDefinition>() :
                (WarriorAttributeAlignmentDefinition[])attributeAlignments.Clone();
            _items = items == null ? Array.Empty<DefinitionId>() : (DefinitionId[])items.Clone();
            _perks = perks == null ? Array.Empty<DefinitionId>() : (DefinitionId[])perks.Clone();
        }
    }

    public sealed class FightRuleDefinition
    {
        public DefinitionId Behavior { get; }
        public IReadOnlyDictionary<string, ModParameterValue> InitialParameters { get; }

        private readonly int[] _rounds;
        public DefinitionId Id { get; }
        public ModFightRuleKind Kind { get; }
        public ModRuleTarget Target { get; }
        public ModRuleMode Mode { get; }
        public IReadOnlyList<int> Rounds => Array.AsReadOnly(_rounds);
        public string Name { get; }
        public DefinitionId Item { get; }
        public bool HasItem { get; }
        public int MinimumLevel { get; }
        public DefinitionId Perk { get; }
        public bool HasPerk { get; }
        private readonly Dictionary<string, float> _attributes;
        public IReadOnlyDictionary<string, float> Attributes => _attributes;

        internal FightRuleDefinition(DefinitionId id, ModFightRuleKind kind, ModRuleTarget target, ModRuleMode mode,
            int[] rounds, string name, DefinitionId item, bool hasItem, int minimumLevel,
            DefinitionId perk = default(DefinitionId), bool hasPerk = false,
            IReadOnlyDictionary<string, float> attributes = null, DefinitionId behavior = default,
            IReadOnlyDictionary<string, ModParameterValue> initialParameters = null)
        {
            Behavior = behavior;
            var parameterCopy = new Dictionary<string, ModParameterValue>();
            if (initialParameters != null)
                foreach (var pair in initialParameters) parameterCopy.Add(pair.Key, pair.Value);
            InitialParameters = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ModParameterValue>(
                parameterCopy);
            if (kind == ModFightRuleKind.Behavior && behavior.Category != "behaviors")
                throw new ModContentException("Behavior rule requires a behavior definition.");
            Id = id;
            Kind = kind;
            Target = target;
            Mode = mode;
            _rounds = rounds == null ? Array.Empty<int>() : (int[])rounds.Clone();
            for (int i = 0; i < _rounds.Length; i++)
                if (_rounds[i] < 1 || _rounds[i] > 100)
                    throw new ModContentException("Rule round must be 1..100.");
            Name = name ?? string.Empty;
            Item = item;
            HasItem = hasItem;
            MinimumLevel = minimumLevel;
            Perk = perk;
            HasPerk = hasPerk;
            _attributes = new Dictionary<string, float>(StringComparer.Ordinal);
            if (attributes != null)
            {
                foreach (KeyValuePair<string, float> pair in attributes)
                {
                    if (string.IsNullOrWhiteSpace(pair.Key) || float.IsNaN(pair.Value) || float.IsInfinity(pair.Value))
                        throw new ModContentException("Rule attribute names and values must be valid and finite.");
                    _attributes.Add(pair.Key, pair.Value);
                }
            }
            if (kind == ModFightRuleKind.RequireItem || kind == ModFightRuleKind.EquipItem)
            {
                if (!hasItem || item.Category != "items")
                    throw new ModContentException(kind + " rule requires an item definition.");
                if (minimumLevel < 0 || minimumLevel > 10000)
                    throw new ModContentException(kind + " minimum level must be 0..10000.");
            }
            else if (hasItem)
            {
                throw new ModContentException("Only item rules accept an item reference.");
            }
            if (kind == ModFightRuleKind.Perk && (!hasPerk || perk.Category != "perks"))
                throw new ModContentException("Perk rule requires a perk definition.");
            if (kind != ModFightRuleKind.Perk && hasPerk)
                throw new ModContentException("Only Perk rules accept a perk reference.");
            if (kind == ModFightRuleKind.Attributes && _attributes.Count == 0)
                throw new ModContentException("Attributes rule requires at least one override.");
            if (kind != ModFightRuleKind.Attributes && _attributes.Count != 0)
                throw new ModContentException("Only Attributes rules accept attribute overrides.");
        }
    }

    public sealed class RewardItemGrant
    {
        public DefinitionId Item { get; }
        public uint UpgradeNumber { get; }
        public RewardItemGrant(DefinitionId item, uint upgradeNumber = 0)
        {
            if (item.Category != "items") throw new ModContentException("Reward item must reference an item definition.");
            Item = item;
            UpgradeNumber = upgradeNumber;
        }
    }

    public sealed class RewardChoiceItem
    {
        public RewardItemGrant Grant { get; }
        public float Weight { get; }
        public RewardChoiceItem(RewardItemGrant grant, float weight = 1f)
        {
            Grant = grant ?? throw new ArgumentNullException(nameof(grant));
            if (float.IsNaN(weight) || float.IsInfinity(weight) || weight <= 0f)
                throw new ModContentException("Reward choice weight must be finite and positive.");
            Weight = weight;
        }
    }

    public sealed class RewardChoiceDefinition
    {
        private readonly RewardChoiceItem[] _items;
        public IReadOnlyList<RewardChoiceItem> Items => Array.AsReadOnly(_items);
        public RewardChoiceDefinition(RewardChoiceItem[] items)
        {
            if (items == null || items.Length == 0)
                throw new ModContentException("Reward choice must contain at least one item.");
            _items = (RewardChoiceItem[])items.Clone();
        }
    }

    public sealed class RewardDefinition
    {
        private readonly RewardItemGrant[] _items;
        private readonly RewardChoiceDefinition[] _choices;
        public DefinitionId Id { get; }
        public int Gems { get; }
        public IReadOnlyList<RewardItemGrant> Items => Array.AsReadOnly(_items);
        public IReadOnlyList<RewardChoiceDefinition> Choices => Array.AsReadOnly(_choices);

        internal RewardDefinition(DefinitionId id, RewardItemGrant[] items, RewardChoiceDefinition[] choices, int gems = 0)
        {
            Id = id;
            _items = items == null ? Array.Empty<RewardItemGrant>() : (RewardItemGrant[])items.Clone();
            if (gems < 0 || gems > 1000000) throw new ModContentException("Reward gems must be 0..1000000.");
            Gems = gems;
            _choices = choices == null ? Array.Empty<RewardChoiceDefinition>() : (RewardChoiceDefinition[])choices.Clone();
            // Empty slots are meaningful: recovered fights index rewards by wins,
            // including a zero-win slot that commonly grants nothing.
        }
    }

    public sealed partial class ModContentCatalog
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
        private readonly DefinitionRegistry<ZoneDefinition> _zones =
            new DefinitionRegistry<ZoneDefinition>(value => value.Id);
        private readonly DefinitionRegistry<BattleDefinition> _battles =
            new DefinitionRegistry<BattleDefinition>(value => value.Id);
        private readonly DefinitionRegistry<FightDefinition> _fights =
            new DefinitionRegistry<FightDefinition>(value => value.Id);
        private readonly DefinitionRegistry<WarriorDefinition> _warriors =
            new DefinitionRegistry<WarriorDefinition>(value => value.Id);
        private readonly DefinitionRegistry<FightRuleDefinition> _fightRules =
            new DefinitionRegistry<FightRuleDefinition>(value => value.Id);
        private readonly DefinitionRegistry<RewardDefinition> _rewards =
            new DefinitionRegistry<RewardDefinition>(value => value.Id);
        private readonly List<ModContentPatchRecord> _patches = new List<ModContentPatchRecord>();
        private readonly IReadOnlyList<ModContentPatchRecord> _readOnlyPatches;
        private readonly Dictionary<ModContentPatchKey, ModContentPatchRecord> _patchByKey =
            new Dictionary<ModContentPatchKey, ModContentPatchRecord>();

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
        public IReadOnlyList<ZoneDefinition> Zones => _zones.Values;
        public IReadOnlyList<BattleDefinition> Battles => _battles.Values;
        public IReadOnlyList<FightDefinition> Fights => _fights.Values;
        public IReadOnlyList<WarriorDefinition> Warriors => _warriors.Values;
        public IReadOnlyList<FightRuleDefinition> FightRules => _fightRules.Values;
        public IReadOnlyList<RewardDefinition> Rewards => _rewards.Values;
        public IReadOnlyList<ModContentPatchRecord> Patches => _readOnlyPatches;

        public ModContentCatalog()
        {
            _readOnlyPatches = _patches.AsReadOnly();
        }

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
            if (TryGetP1CItem(id, out value)) return true;
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

        public bool TryGetZone(DefinitionId id, out ZoneDefinition value)
        {
            value = null;
            return id.Category == "zones" && _zones.TryGet(id, out value);
        }

        public bool TryGetBattle(DefinitionId id, out BattleDefinition value)
        {
            value = null;
            return id.Category == "battles" && _battles.TryGet(id, out value);
        }

        public bool TryGetFight(DefinitionId id, out FightDefinition value)
        {
            value = null;
            return id.Category == "fights" && _fights.TryGet(id, out value);
        }

        public bool TryGetWarrior(DefinitionId id, out WarriorDefinition value)
        {
            value = null;
            return id.Category == "warriors" && _warriors.TryGet(id, out value);
        }

        public bool TryGetFightRule(DefinitionId id, out FightRuleDefinition value)
        {
            value = null;
            return id.Category == "rules" && _fightRules.TryGet(id, out value);
        }

        public bool TryGetReward(DefinitionId id, out RewardDefinition value)
        {
            value = null;
            return id.Category == "rewards" && _rewards.TryGet(id, out value);
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        internal void Commit(ModRegistrationTransaction transaction,
            LocalizationDefinition[] localizations, WeaponDefinition[] weapons,
            ArmorDefinition[] armors, HelmDefinition[] helms, RangedDefinition[] ranged,
            MagicDefinition[] magic, NonEquipmentItemDefinition[] nonEquipmentItems,
            ItemRedirectDefinition[] itemRedirects,
            ShopListingDefinition[] shopListings, PerkDefinition[] perks,
            EnchantmentDefinition[] enchantments, ModBehaviorDefinition[] behaviors,
            ZoneDefinition[] zones, BattleDefinition[] battles, FightDefinition[] fights,
            WarriorDefinition[] warriors, FightRuleDefinition[] fightRules, RewardDefinition[] rewards,
            LocalizationValuePatch[] localizationPatches, FightFieldPatch[] fightPatches,
            ModContentPatchRecord[] collectionPatches)
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
            _zones.ValidateCanAdd(zones);
            _battles.ValidateCanAdd(battles);
            _fights.ValidateCanAdd(fights);
            _warriors.ValidateCanAdd(warriors);
            _fightRules.ValidateCanAdd(fightRules);
            _rewards.ValidateCanAdd(rewards);

            Dictionary<DefinitionId, LocalizationDefinition> localizationReplacements =
                PrepareLocalizationPatches(localizationPatches);
            Dictionary<DefinitionId, FightDefinition> fightReplacements = PrepareFightPatches(fightPatches);
            ValidateCollectionPatches(collectionPatches);

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
                foreach (var upgrade in perk.Upgrades)
                    if (!ContainsLocalization(localizations, upgrade.Description) &&
                        !_localizations.TryGet(upgrade.Description, out LocalizationDefinition ignoredUpgradeDescription))
                        throw new ModContentException("Perk upgrade references missing description localization '" + upgrade.Description + "'.");
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

            ValidateStageGraph(zones, battles, fights, warriors, fightRules, rewards,
                weapons, armors, helms, ranged, magic, nonEquipmentItems, perks);

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
            _zones.AddRange(zones);
            _battles.AddRange(battles);
            _fights.AddRange(fights);
            _warriors.AddRange(warriors);
            _fightRules.AddRange(fightRules);
            _rewards.AddRange(rewards);
            foreach (KeyValuePair<DefinitionId, LocalizationDefinition> replacement in localizationReplacements)
                _localizations.Replace(replacement.Key, replacement.Value);
            foreach (KeyValuePair<DefinitionId, FightDefinition> replacement in fightReplacements)
                _fights.Replace(replacement.Key, replacement.Value);
            for (int i = 0; i < localizationPatches.Length; i++)
            {
                ModContentPatchRecord record = localizationPatches[i].Record;
                var key = new ModContentPatchKey(record.Target, record.Field);
                _patchByKey.Add(key, record);
                _patches.Add(record);
            }
            for (int i = 0; i < fightPatches.Length; i++)
            {
                ModContentPatchRecord record = fightPatches[i].Record;
                var key = new ModContentPatchKey(record.Target, record.Field);
                _patchByKey.Add(key, record);
                _patches.Add(record);
            }
            for (int i = 0; i < collectionPatches.Length; i++)
            {
                ModContentPatchRecord record = collectionPatches[i];
                var key = new ModContentPatchKey(record.Target, record.Field);
                _patchByKey.Add(key, record);
                _patches.Add(record);
            }
        }

        private void ValidateCollectionPatches(ModContentPatchRecord[] patches)
        {
            var pending = new HashSet<ModContentPatchKey>();
            for (int i = 0; i < patches.Length; i++)
            {
                ModContentPatchRecord record = patches[i];
                ModContentPolicies.RequirePatchAllowed(record.Target, record.Field, record.Operation);
                var key = new ModContentPatchKey(record.Target, record.Field);
                ModContentPatchRecord existing;
                if (_patchByKey.TryGetValue(key, out existing)) throw PatchConflict(existing, record);
                if (!pending.Add(key))
                    throw new ModContentException("Collection child patch is staged more than once for '" +
                        record.Target + "' field '" + record.Field + "'.");
            }
        }

        private Dictionary<DefinitionId, FightDefinition> PrepareFightPatches(FightFieldPatch[] fightPatches)
        {
            var replacements = new Dictionary<DefinitionId, FightDefinition>();
            var pendingKeys = new HashSet<ModContentPatchKey>();
            for (int i = 0; i < fightPatches.Length; i++)
            {
                FightFieldPatch patch = fightPatches[i];
                ModContentPatchRecord record = patch.Record;
                ModContentPolicies.RequirePatchAllowed(record.Target, record.Field, record.Operation);
                var patchKey = new ModContentPatchKey(record.Target, record.Field);
                ModContentPatchRecord existingPatch;
                if (_patchByKey.TryGetValue(patchKey, out existingPatch)) throw PatchConflict(existingPatch, record);
                if (!pendingKeys.Add(patchKey))
                    throw new ModContentException("Fight patch is staged more than once for '" + record.Target +
                        "' field '" + record.Field + "'.");
                FightDefinition current;
                if (!replacements.TryGetValue(record.Target, out current) && !_fights.TryGet(record.Target, out current))
                    throw new ModContentException("Fight patch target is not registered: '" + record.Target + "'.");
                if (record.Field == ModContentPolicies.FightDescription) current = current.WithDescription(patch.StringValue);
                else if (record.Field == ModContentPolicies.FightRounds) current = current.WithRounds(patch.IntValue);
                else if (record.Field == ModContentPolicies.FightRoundTime) current = current.WithRoundTime(patch.IntValue);
                else if (record.Field == ModContentPolicies.FightLocation) current = current.WithPresentation(patch.StringValue, current.Music);
                else if (record.Field == ModContentPolicies.FightMusic) current = current.WithPresentation(current.Location, patch.StringValue);
                else if (record.Field == ModContentPolicies.FightRules) current = current.WithRules(patch.Rules, patch.AppendRules);
                else throw new ModContentException("Unsupported fight patch field '" + record.Field + "'.");
                replacements[record.Target] = current;
            }
            return replacements;
        }

        private void ValidateStageGraph(ZoneDefinition[] zones, BattleDefinition[] battles, FightDefinition[] fights,
            WarriorDefinition[] warriors, FightRuleDefinition[] fightRules, RewardDefinition[] rewards,
            WeaponDefinition[] weapons, ArmorDefinition[] armors, HelmDefinition[] helms,
            RangedDefinition[] ranged, MagicDefinition[] magic, NonEquipmentItemDefinition[] nonEquipmentItems,
            PerkDefinition[] perks)
        {
            var zoneIds = new HashSet<DefinitionId>();
            for (int i = 0; i < zones.Length; i++) zoneIds.Add(zones[i].Id);
            var battleIds = new HashSet<DefinitionId>();
            for (int i = 0; i < battles.Length; i++) battleIds.Add(battles[i].Id);
            var fightIds = new HashSet<DefinitionId>();
            for (int i = 0; i < fights.Length; i++) fightIds.Add(fights[i].Id);
            var warriorIds = new HashSet<DefinitionId>();
            for (int i = 0; i < warriors.Length; i++) warriorIds.Add(warriors[i].Id);
            var ruleIds = new HashSet<DefinitionId>();
            for (int i = 0; i < fightRules.Length; i++) ruleIds.Add(fightRules[i].Id);
            var rewardIds = new HashSet<DefinitionId>();
            for (int i = 0; i < rewards.Length; i++) rewardIds.Add(rewards[i].Id);

            for (int i = 0; i < zones.Length; i++)
            {
                ZoneDefinition zone = zones[i];
                for (int j = 0; j < zone.Battles.Count; j++)
                {
                    DefinitionId battleId = zone.Battles[j];
                    if (!battleIds.Contains(battleId))
                        throw new ModContentException("Zone '" + zone.Id + "' references missing battle '" + battleId + "'.");
                }
            }

            for (int i = 0; i < battles.Length; i++)
            {
                BattleDefinition battle = battles[i];
                if (!zoneIds.Contains(battle.Zone) && !_zones.TryGet(battle.Zone, out ZoneDefinition ignoredZone))
                    throw new ModContentException("Battle '" + battle.Id + "' references missing zone '" + battle.Zone + "'.");
                for (int j = 0; j < battle.Fights.Count; j++)
                    if (!fightIds.Contains(battle.Fights[j]))
                        throw new ModContentException("Battle '" + battle.Id + "' references missing fight '" + battle.Fights[j] + "'.");
            }

            for (int i = 0; i < fights.Length; i++)
            {
                FightDefinition fight = fights[i];
                if (!battleIds.Contains(fight.Battle))
                    throw new ModContentException("Fight '" + fight.Id + "' references missing battle '" + fight.Battle + "'.");
                for (int j = 0; j < fight.Warriors.Count; j++)
                    if (!warriorIds.Contains(fight.Warriors[j]) && !_warriors.TryGet(fight.Warriors[j], out WarriorDefinition ignoredWarrior))
                        throw new ModContentException("Fight '" + fight.Id + "' references missing warrior '" + fight.Warriors[j] + "'.");
                for (int j = 0; j < fight.Rules.Count; j++)
                    if (!ruleIds.Contains(fight.Rules[j]) && !_fightRules.TryGet(fight.Rules[j], out FightRuleDefinition ignoredRule))
                        throw new ModContentException("Fight '" + fight.Id + "' references missing rule '" + fight.Rules[j] + "'.");
                for (int j = 0; j < fight.Rewards.Count; j++)
                    if (!rewardIds.Contains(fight.Rewards[j]) && !_rewards.TryGet(fight.Rewards[j], out RewardDefinition ignoredReward))
                        throw new ModContentException("Fight '" + fight.Id + "' references missing reward '" + fight.Rewards[j] + "'.");
            }

            for (int i = 0; i < warriors.Length; i++)
            {
                WarriorDefinition warrior = warriors[i];
                for (int j = 0; j < warrior.Items.Count; j++)
                {
                    ItemDefinition item;
                    if (!TryGetPendingItem(warrior.Items[j], weapons, armors, helms, ranged, magic,
                            nonEquipmentItems, out item) &&
                        !TryResolveItem(warrior.Items[j], out item))
                        throw new ModContentException("Warrior '" + warrior.Id + "' references missing item '" + warrior.Items[j] + "'.");
                }
                for (int j = 0; j < warrior.Perks.Count; j++)
                {
                    PerkDefinition perk;
                    if (!TryGetPendingPerk(warrior.Perks[j], perks, out perk) && !_perks.TryGet(warrior.Perks[j], out perk))
                        throw new ModContentException("Warrior '" + warrior.Id + "' references missing perk '" + warrior.Perks[j] + "'.");
                }
            }

            for (int i = 0; i < fightRules.Length; i++)
            {
                FightRuleDefinition rule = fightRules[i];
                if (!rule.HasItem) continue;
                ItemDefinition item;
                if (!TryGetPendingItem(rule.Item, weapons, armors, helms, ranged, magic,
                        nonEquipmentItems, out item) &&
                    !TryResolveItem(rule.Item, out item))
                    throw new ModContentException("Fight rule '" + rule.Id + "' references missing item '" + rule.Item + "'.");
            }

            for (int i = 0; i < rewards.Length; i++)
            {
                RewardDefinition reward = rewards[i];
                for (int j = 0; j < reward.Items.Count; j++)
                    ValidateCommittedRewardItem(reward.Id, reward.Items[j], weapons, armors, helms, ranged, magic,
                        nonEquipmentItems);
                for (int j = 0; j < reward.Choices.Count; j++)
                    for (int k = 0; k < reward.Choices[j].Items.Count; k++)
                        ValidateCommittedRewardItem(reward.Id, reward.Choices[j].Items[k].Grant,
                            weapons, armors, helms, ranged, magic, nonEquipmentItems);
            }
        }

        private void ValidateCommittedRewardItem(DefinitionId rewardId, RewardItemGrant grant,
            WeaponDefinition[] weapons, ArmorDefinition[] armors, HelmDefinition[] helms,
            RangedDefinition[] ranged, MagicDefinition[] magic, NonEquipmentItemDefinition[] nonEquipmentItems)
        {
            ItemDefinition item;
            if (!TryGetPendingItem(grant.Item, weapons, armors, helms, ranged, magic, nonEquipmentItems, out item) &&
                !TryResolveItem(grant.Item, out item))
                throw new ModContentException("Reward '" + rewardId + "' references missing item '" + grant.Item + "'.");
        }

        private Dictionary<DefinitionId, LocalizationDefinition> PrepareLocalizationPatches(
            LocalizationValuePatch[] localizationPatches)
        {
            var replacements = new Dictionary<DefinitionId, LocalizationDefinition>();
            var pendingKeys = new HashSet<ModContentPatchKey>();
            for (int i = 0; i < localizationPatches.Length; i++)
            {
                LocalizationValuePatch patch = localizationPatches[i];
                ModContentPatchRecord record = patch.Record;
                if (record.Target.Category != "localization")
                    throw new ModContentException("Localization patch target must use the localization category: '" +
                        record.Target + "'.");
                ModContentPolicies.RequirePatchAllowed(record.Target, record.Field, record.Operation);

                var patchKey = new ModContentPatchKey(record.Target, record.Field);
                ModContentPatchRecord existingPatch;
                if (_patchByKey.TryGetValue(patchKey, out existingPatch))
                    throw PatchConflict(existingPatch, record);
                if (!pendingKeys.Add(patchKey))
                    throw new ModContentException("Patch is staged more than once for '" + record.Target +
                        "' field '" + record.Field + "' by mod '" + record.Owner + "'.");

                LocalizationDefinition current;
                if (!replacements.TryGetValue(record.Target, out current) &&
                    !_localizations.TryGet(record.Target, out current))
                    throw new ModContentException("Patch target is not registered: '" + record.Target + "'.");

                var values = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (KeyValuePair<string, string> pair in current.Values) values.Add(pair.Key, pair.Value);
                values[patch.Language] = patch.Value;
                replacements[record.Target] = new LocalizationDefinition(current.Id, values, current.LegacyKey);
            }
            return replacements;
        }

        private static ModContentException PatchConflict(ModContentPatchRecord existing,
            ModContentPatchRecord incoming)
        {
            return new ModContentException("Patch conflict on '" + incoming.Target + "' field '" + incoming.Field +
                "': mod '" + existing.Owner + "' already owns this semantic field; mod '" + incoming.Owner +
                "' cannot replace it.");
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
            return TryGetPendingItem(id, weapons, armors, helms, ranged, magic,
                new NonEquipmentItemDefinition[0], out value);
        }

        private static bool TryGetPendingItem(DefinitionId id, WeaponDefinition[] weapons,
            ArmorDefinition[] armors, HelmDefinition[] helms, RangedDefinition[] ranged,
            MagicDefinition[] magic, NonEquipmentItemDefinition[] nonEquipmentItems, out ItemDefinition value)
        {
            value = null;
            for (int i = 0; i < weapons.Length; i++) if (weapons[i].Id == id) { value = weapons[i]; return true; }
            for (int i = 0; i < armors.Length; i++) if (armors[i].Id == id) { value = armors[i]; return true; }
            for (int i = 0; i < helms.Length; i++) if (helms[i].Id == id) { value = helms[i]; return true; }
            for (int i = 0; i < ranged.Length; i++) if (ranged[i].Id == id) { value = ranged[i]; return true; }
            for (int i = 0; i < magic.Length; i++) if (magic[i].Id == id) { value = magic[i]; return true; }
            for (int i = 0; i < nonEquipmentItems.Length; i++)
                if (nonEquipmentItems[i].Id == id) { value = nonEquipmentItems[i]; return true; }
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

        internal void ImportCoreStages(ZoneDefinition[] zones, BattleDefinition[] battles,
            FightDefinition[] fights)
        {
            if (IsFrozen) throw new InvalidOperationException("Definition registries are frozen.");
            if (zones == null) throw new ArgumentNullException(nameof(zones));
            if (battles == null) throw new ArgumentNullException(nameof(battles));
            if (fights == null) throw new ArgumentNullException(nameof(fights));
            _zones.ValidateCanAdd(zones);
            _battles.ValidateCanAdd(battles);
            _fights.ValidateCanAdd(fights);
            for (int i = 0; i < zones.Length; i++)
                if (!zones[i].IsCore || string.IsNullOrEmpty(zones[i].LegacyName))
                    throw new ModContentException("Invalid core zone import: " + zones[i].Id);
            for (int i = 0; i < battles.Length; i++)
                if (!battles[i].IsCore || battles[i].Zone.Namespace.Value != "core" ||
                    string.IsNullOrEmpty(battles[i].LegacyName) || string.IsNullOrEmpty(battles[i].LegacyXml))
                    throw new ModContentException("Invalid core battle import: " + battles[i].Id);
            for (int i = 0; i < fights.Length; i++)
                if (!fights[i].IsCore || fights[i].Battle.Namespace.Value != "core" ||
                    string.IsNullOrEmpty(fights[i].LegacyName) || string.IsNullOrEmpty(fights[i].LegacyXml))
                    throw new ModContentException("Invalid core fight import: " + fights[i].Id);
            _zones.AddRange(zones);
            _battles.AddRange(battles);
            _fights.AddRange(fights);
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

            public void Replace(DefinitionId id, T value)
            {
                T existing;
                if (!_byId.TryGetValue(id, out existing))
                    throw new ModContentException("Definition does not exist for replacement: '" + id + "'.");
                if (_getId(value) != id)
                    throw new ModContentException("Replacement definition identity changed: '" + id + "'.");
                int index = _values.IndexOf(existing);
                if (index < 0) throw new InvalidOperationException("Definition registry index is inconsistent: '" + id + "'.");
                _byId[id] = value;
                _values[index] = value;
            }
        }
    }

    public sealed partial class ModRegistrationTransaction : IDisposable
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
        private readonly Dictionary<DefinitionId, ZoneDefinition> _zones =
            new Dictionary<DefinitionId, ZoneDefinition>();
        private readonly Dictionary<DefinitionId, BattleDefinition> _battles =
            new Dictionary<DefinitionId, BattleDefinition>();
        private readonly Dictionary<DefinitionId, FightDefinition> _fights =
            new Dictionary<DefinitionId, FightDefinition>();
        private readonly List<DefinitionId> _battleOrder = new List<DefinitionId>();
        private readonly List<DefinitionId> _fightOrder = new List<DefinitionId>();
        private readonly Dictionary<DefinitionId, WarriorDefinition> _warriors =
            new Dictionary<DefinitionId, WarriorDefinition>();
        private readonly Dictionary<DefinitionId, FightRuleDefinition> _fightRules =
            new Dictionary<DefinitionId, FightRuleDefinition>();
        private readonly Dictionary<DefinitionId, RewardDefinition> _rewards =
            new Dictionary<DefinitionId, RewardDefinition>();
        private readonly List<LocalizationValuePatch> _localizationPatches = new List<LocalizationValuePatch>();
        private readonly List<FightFieldPatch> _fightPatches = new List<FightFieldPatch>();
        private readonly List<ModContentPatchRecord> _collectionPatches = new List<ModContentPatchRecord>();
        private readonly HashSet<ModContentPatchKey> _patchKeys = new HashSet<ModContentPatchKey>();
        private readonly HashSet<DefinitionId> _listedItems = new HashSet<DefinitionId>();
        private bool _completed;

        public ModDescriptor Mod { get; }
        public int RegistrationCount => _localizations.Count + _weapons.Count + _armors.Count + _helms.Count +
            _ranged.Count + _magic.Count + _itemRedirects.Count + _shopListings.Count + _perks.Count +
            _enchantments.Count + _behaviors.Count + _zones.Count + _battles.Count + _fights.Count +
            _warriors.Count + _fightRules.Count + _rewards.Count + _localizationPatches.Count + _fightPatches.Count +
            _collectionPatches.Count + P1CRegistrationCount + P1BRegistrationCount + P1DRegistrationCount + _modes.Count + _timers.Count + _disabledFeatures.Count + _counters.Count + _achievements.Count + _replacements.Count;

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

        public DefinitionId PatchLocalization(string reference, string language, string value)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference))
                throw new ModContentException("Localization patch target must not be empty.");
            DefinitionId id;
            try
            {
                id = reference.IndexOf(':') >= 0 ? DefinitionId.Parse(reference) : Qualify("localization", reference);
            }
            catch (FormatException exception)
            {
                throw new ModContentException(exception.Message, exception);
            }
            if (id.Category != "localization")
                throw new ModContentException("Localization patch target must use the 'localization' definition category: '" +
                    id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot patch undeclared namespace '" +
                    id.Namespace + "'. Declare it as a dependency first.");
            LocalizationDefinition existing;
            if (!_catalog.TryGetLocalization(id, out existing))
                throw new ModContentException("Localization patch target is not registered: '" + id + "'.");

            string normalizedLanguage = NormalizeLanguage(language);
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                throw new ModContentException("Localization patch value for '" + id + "' must not be empty.");
            string field = ModContentPolicies.LocalizationValue(normalizedLanguage);
            ModContentPolicies.RequirePatchAllowed(id, field, ModContentPatchOperation.Replace);
            var key = new ModContentPatchKey(id, field);
            EnsureCapacityForNewRegistration();
            if (!_patchKeys.Add(key))
                throw new ModContentException("Duplicate localization patch for '" + id + "' language '" +
                    normalizedLanguage + "'.");

            var record = new ModContentPatchRecord(Mod.Id, id, field, ModContentPatchOperation.Replace);
            _localizationPatches.Add(new LocalizationValuePatch(record, normalizedLanguage, value));
            return id;
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

        public ItemDefinition GetItem(string reference)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference)) throw new ModContentException("Item reference must not be empty.");
            DefinitionId id;
            try { id = DefinitionId.Parse(reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "items")
                throw new ModContentException("Item reference must use the 'items' definition category: '" + id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot reference undeclared item namespace '" +
                    id.Namespace + "'.");
            ItemDefinition value;
            if (TryGetPendingItem(id, out value) || _catalog.TryResolveItem(id, out value)) return value;
            throw new ModContentException("Item is not registered: '" + id + "'.");
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

        public ModBehaviorDefinition RegisterBehavior(string localId, ModParameterSchema parameters,
            ModParameterSchema state = null, string lifetime = "fight", int version = 1)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("behaviors", localId);
            if (_behaviors.ContainsKey(id))
                throw new ModContentException("Duplicate behavior definition: '" + id + "'.");
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            EnsureCapacityForNewRegistration();
            var definition = new ModBehaviorDefinition(id, parameters, state, lifetime, version);
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

        public PerkDefinition SetPerkUpgrades(DefinitionId id, PerkUpgradeDefinition[] upgrades)
        {
            ThrowIfCompleted();
            if (id.Namespace != Mod.Id || !_perks.TryGetValue(id, out var perk))
                throw new ModContentException("Upgrades require a perk registered in this transaction.");
            if (upgrades == null || upgrades.Length == 0 || upgrades.Length > 100)
                throw new ModContentException("Perk upgrades require 1..100 entries.");
            for (int i = 0; i < upgrades.Length; i++)
            {
                var upgrade = upgrades[i];
                if (upgrade == null || upgrade.Level != i + 1) throw new ModContentException("Perk upgrades must be ordered and contiguous from level 1.");
                ValidateScriptedPresentation(perk.DisplayName, upgrade.Description, perk.Icon, "Perk upgrade");
                if (perk.HasBehavior)
                {
                    if (upgrade.Parameters.Count != 0) throw new ModContentException("Scripted upgrades require typed parameters.");
                    var combined = new Dictionary<string, ModParameterValue>();
                    foreach (var pair in perk.InitialParameters) combined.Add(pair.Key, pair.Value);
                    foreach (var pair in upgrade.TypedParameters) combined[pair.Key] = pair.Value;
                    RequirePendingBehavior(perk.Behavior, "Perk upgrade").Parameters.ResolveValues(combined);
                }
                else
                {
                    if (upgrade.TypedParameters.Count != 0) throw new ModContentException("Native upgrades require native parameters.");
                    if (upgrade.Parameters.Count > 64) throw new ModContentException("Perk upgrade parameter limit exceeded (64).");
                    foreach (var pair in upgrade.Parameters)
                    {
                        ValidatePerkParameterName(pair.Key);
                        if (pair.Value == null || pair.Value.Length == 0 || pair.Value.Length > 2048) throw new ModContentException("Invalid perk upgrade parameter value.");
                    }
                }
            }
            var definition = perk.WithUpgrades(upgrades);
            _perks[id] = definition;
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

        public ZoneDefinition RegisterZone(string localId, string fileName = null, bool isStart = false)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("zones", localId);
            if (_zones.ContainsKey(id)) throw new ModContentException("Duplicate zone definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new ZoneDefinition(id, id.ToString(), fileName, isStart, Array.Empty<DefinitionId>());
            _zones.Add(id, definition);
            return definition;
        }

        public ZoneDefinition GetZone(string reference)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference)) throw new ModContentException("Zone reference must not be empty.");
            DefinitionId id;
            try { id = reference.IndexOf(':') >= 0 ? DefinitionId.Parse(reference) : Qualify("zones", reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "zones")
                throw new ModContentException("Zone reference must use the 'zones' definition category: '" + id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot reference undeclared zone namespace '" +
                    id.Namespace + "'.");
            ZoneDefinition value;
            if (_zones.TryGetValue(id, out value) || _catalog.TryGetZone(id, out value)) return value;
            throw new ModContentException("Zone is not registered: '" + id + "'.");
        }

        public BattleDefinition RegisterBattle(string localId, DefinitionId zone, ModBattleKind kind,
            int x = 0, int y = 0, string alias = null, string title = null, string icon = null,
            string preview = null, string description = null, string location = null, string music = null,
            string rewardImage = null, bool showResistance = false, string iconAtlas = null,
            string eclipseToggleName = null)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("battles", localId);
            if (_battles.ContainsKey(id)) throw new ModContentException("Duplicate battle definition: '" + id + "'.");
            if (zone.Category != "zones" || !CanReferenceNamespace(zone.Namespace))
                throw new ModContentException("External battle references an invalid or undeclared zone namespace: '" + zone + "'.");
            ZoneDefinition zoneDefinition;
            bool pendingZone = _zones.TryGetValue(zone, out zoneDefinition);
            if (!pendingZone && !_catalog.TryGetZone(zone, out zoneDefinition))
                throw new ModContentException("External battle references missing zone '" + zone + "'.");
            if (!Enum.IsDefined(typeof(ModBattleKind), kind))
                throw new ModContentException("Unsupported battle kind: " + kind + ".");
            if (kind == ModBattleKind.Periodic || kind == ModBattleKind.Replayable ||
                kind == ModBattleKind.BossesReplayable || kind == ModBattleKind.FinalReplayable ||
                kind == ModBattleKind.Ascension)
                throw new ModContentException("Battle kind '" + kind +
                    "' needs its dedicated roadmap mode adapter and is not available through the ordinary P1A battle API.");
            EnsureCapacityForNewRegistration();
            var definition = new BattleDefinition(id, zone, id.ToString(), kind, x, y, alias, title,
                string.IsNullOrEmpty(icon) ? "training" : icon, preview, description, location, music,
                rewardImage, showResistance, Array.Empty<DefinitionId>(), iconAtlas, eclipseToggleName);
            _battles.Add(id, definition);
            _battleOrder.Add(id);
            if (!pendingZone)
            {
                string field = ModContentPolicies.ZoneBattleChildren + id.ToString();
                ModContentPolicies.RequirePatchAllowed(zone, field, ModContentPatchOperation.Append);
                var key = new ModContentPatchKey(zone, field);
                if (!_patchKeys.Add(key))
                    throw new ModContentException("Duplicate battle child append for zone '" + zone +
                        "' and battle '" + id + "'.");
                _collectionPatches.Add(new ModContentPatchRecord(Mod.Id, zone, field,
                    ModContentPatchOperation.Append));
            }
            return definition;
        }

        public WarriorDefinition RegisterWarrior(string localId, string firstName, string lastName, string avatar,
            string voice, int level, string tactic, DefinitionId[] items, DefinitionId[] perks,
            DefinitionId template = default(DefinitionId), bool hasTemplate = false, string group = null,
            int random = 0, IReadOnlyDictionary<string, float> attributes = null,
            WarriorAttributeAlignmentDefinition[] attributeAlignments = null, int healthBars = 0)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("warriors", localId);
            if (_warriors.ContainsKey(id)) throw new ModContentException("Duplicate warrior definition: '" + id + "'.");
            if (hasTemplate)
            {
                WarriorTemplateDefinition templateDefinition;
                if (template.Category != "warrior-templates" || !CanReferenceNamespace(template.Namespace) ||
                    !_catalog.TryGetWarriorTemplate(template, out templateDefinition))
                    throw new ModContentException("Warrior references unavailable template '" + template + "'.");
            }
            if (random < 0) throw new ModContentException("Warrior random group selector must not be negative.");
            items = items ?? Array.Empty<DefinitionId>();
            perks = perks ?? Array.Empty<DefinitionId>();
            var seenItems = new HashSet<DefinitionId>();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Category != "items" || !CanReferenceNamespace(items[i].Namespace))
                    throw new ModContentException("Warrior item belongs to an undeclared or invalid namespace: '" + items[i] + "'.");
                ItemDefinition item;
                if (!TryGetPendingItem(items[i], out item) && !_catalog.TryResolveItem(items[i], out item))
                    throw new ModContentException("Warrior references missing item '" + items[i] + "'.");
                if (!seenItems.Add(items[i])) throw new ModContentException("Duplicate warrior item '" + items[i] + "'.");
            }
            var seenPerks = new HashSet<DefinitionId>();
            for (int i = 0; i < perks.Length; i++)
            {
                if (perks[i].Category != "perks" || !CanReferenceNamespace(perks[i].Namespace))
                    throw new ModContentException("Warrior perk belongs to an undeclared or invalid namespace: '" + perks[i] + "'.");
                PerkDefinition perk;
                if (!_perks.TryGetValue(perks[i], out perk) && !_catalog.TryGetPerk(perks[i], out perk))
                    throw new ModContentException("Warrior references missing perk '" + perks[i] + "'.");
                if (!seenPerks.Add(perks[i])) throw new ModContentException("Duplicate warrior perk '" + perks[i] + "'.");
            }
            EnsureCapacityForNewRegistration();
            var definition = new WarriorDefinition(id, firstName, lastName, avatar, voice, level, tactic, items, perks,
                template, hasTemplate, group, random, attributes, attributeAlignments, healthBars);
            _warriors.Add(id, definition);
            return definition;
        }

        public FightRuleDefinition RegisterNoPerksRule(string localId, ModRuleTarget target, ModRuleMode mode,
            int[] rounds, string name = null)
        {
            ThrowIfCompleted();
            ValidateRuleEnums(target, mode);
            DefinitionId id = Qualify("rules", localId);
            if (_fightRules.ContainsKey(id)) throw new ModContentException("Duplicate fight rule definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new FightRuleDefinition(id, ModFightRuleKind.NoPerks, target, mode,
                rounds, name, default, false, 0);
            _fightRules.Add(id, definition);
            return definition;
        }

        public FightRuleDefinition RegisterRequireItemRule(string localId, DefinitionId item, int minimumLevel,
            ModRuleMode mode, int[] rounds)
        {
            ThrowIfCompleted();
            ValidateRuleEnums(ModRuleTarget.Player, mode);
            if (!CanReferenceNamespace(item.Namespace))
                throw new ModContentException("RequireItem rule references undeclared namespace '" + item.Namespace + "'.");
            ItemDefinition itemDefinition;
            if (!TryGetPendingItem(item, out itemDefinition) && !_catalog.TryResolveItem(item, out itemDefinition))
                throw new ModContentException("RequireItem rule references missing item '" + item + "'.");
            DefinitionId id = Qualify("rules", localId);
            if (_fightRules.ContainsKey(id)) throw new ModContentException("Duplicate fight rule definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new FightRuleDefinition(id, ModFightRuleKind.RequireItem, ModRuleTarget.Player, mode,
                rounds, string.Empty, item, true, minimumLevel);
            _fightRules.Add(id, definition);
            return definition;
        }

        public FightRuleDefinition RegisterEquipItemRule(string localId, DefinitionId item, int minimumLevel,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            ThrowIfCompleted();
            ValidateRuleEnums(target, mode);
            if (!CanReferenceNamespace(item.Namespace)) throw new ModContentException("EquipItem references undeclared namespace.");
            ItemDefinition itemDefinition;
            if (!TryGetPendingItem(item, out itemDefinition) && !_catalog.TryResolveItem(item, out itemDefinition))
                throw new ModContentException("EquipItem references missing item '" + item + "'.");
            return RegisterExtendedRule(localId, ModFightRuleKind.EquipItem, target, mode, rounds, string.Empty,
                item, true, minimumLevel, default(DefinitionId), false, null);
        }

        public FightRuleDefinition RegisterNamedRule(string localId, ModFightRuleKind kind, string name,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            if (kind != ModFightRuleKind.Avatar && kind != ModFightRuleKind.Name && kind != ModFightRuleKind.NoButton)
                throw new ModContentException("Unsupported named rule kind: " + kind + ".");
            if (string.IsNullOrWhiteSpace(name)) throw new ModContentException(kind + " rule requires a name.");
            return RegisterExtendedRule(localId, kind, target, mode, rounds, name, default(DefinitionId), false, 0,
                default(DefinitionId), false, null);
        }

        public FightRuleDefinition RegisterPerkRule(string localId, DefinitionId perk, ModRuleTarget target,
            ModRuleMode mode, int[] rounds)
        {
            ThrowIfCompleted();
            PerkDefinition perkDefinition;
            if (perk.Category != "perks" || !CanReferenceNamespace(perk.Namespace) ||
                (!_perks.TryGetValue(perk, out perkDefinition) && !_catalog.TryGetPerk(perk, out perkDefinition)))
                throw new ModContentException("Perk rule references an unavailable perk '" + perk + "'.");
            return RegisterExtendedRule(localId, ModFightRuleKind.Perk, target, mode, rounds, string.Empty,
                default(DefinitionId), false, 0, perk, true, null);
        }

        public FightRuleDefinition RegisterRechargeMagicRule(string localId, ModRuleTarget target, ModRuleMode mode,
            int[] rounds)
        {
            return RegisterExtendedRule(localId, ModFightRuleKind.RechargeMagicEachRound, target, mode, rounds,
                string.Empty, default(DefinitionId), false, 0, default(DefinitionId), false, null);
        }

        public FightRuleDefinition RegisterAttributesRule(string localId, ModRuleTarget target, ModRuleMode mode,
            int[] rounds, IReadOnlyDictionary<string, float> attributes)
        {
            return RegisterExtendedRule(localId, ModFightRuleKind.Attributes, target, mode, rounds, string.Empty,
                default(DefinitionId), false, 0, default(DefinitionId), false, attributes);
        }

        private FightRuleDefinition RegisterExtendedRule(string localId, ModFightRuleKind kind, ModRuleTarget target,
            ModRuleMode mode, int[] rounds, string name, DefinitionId item, bool hasItem, int minimumLevel,
            DefinitionId perk, bool hasPerk, IReadOnlyDictionary<string, float> attributes)
        {
            ThrowIfCompleted();
            ValidateRuleEnums(target, mode);
            DefinitionId id = Qualify("rules", localId);
            if (_fightRules.ContainsKey(id)) throw new ModContentException("Duplicate fight rule definition: '" + id + "'.");
            EnsureCapacityForNewRegistration();
            var definition = new FightRuleDefinition(id, kind, target, mode, rounds, name, item, hasItem,
                minimumLevel, perk, hasPerk, attributes);
            _fightRules.Add(id, definition);
            return definition;
        }

        public FightRuleDefinition RegisterBehaviorRule(string localId, DefinitionId behavior, ModRuleTarget target,
            ModRuleMode mode, int[] rounds, IReadOnlyDictionary<string, ModParameterValue> parameters)
        {
            ThrowIfCompleted();
            ValidateRuleEnums(target, mode);
            DefinitionId id = Qualify("rules", localId);
            if (_fightRules.ContainsKey(id)) throw new ModContentException("Duplicate fight rule definition: '" + id + "'.");
            if (!CanReferenceNamespace(behavior.Namespace))
                throw new ModContentException("Rule behavior belongs to an undeclared dependency: '" + behavior + "'.");
            if (!_behaviors.TryGetValue(behavior, out var definition) && !_catalog.TryGetBehavior(behavior, out definition))
                throw new ModContentException("Rule behavior is not registered: '" + behavior + "'.");
            if (definition.StateLifetime == "saved")
                throw new ModContentException("Battle rules support fight or round state; use mod-owned state for persistent progression.");
            var values = definition.Parameters.ResolveValues(parameters);
            var rule = new FightRuleDefinition(id, ModFightRuleKind.Behavior, target, mode, rounds,
                string.Empty, default, false, 0, behavior: behavior, initialParameters: values);
            EnsureCapacityForNewRegistration();
            _fightRules.Add(id, rule);
            return rule;
        }

        public RewardDefinition RegisterReward(string localId, RewardItemGrant[] items, RewardChoiceDefinition[] choices, int gems = 0)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("rewards", localId);
            if (_rewards.ContainsKey(id)) throw new ModContentException("Duplicate reward definition: '" + id + "'.");
            var definition = new RewardDefinition(id, items, choices, gems);
            ValidateRewardReferences(definition);
            EnsureCapacityForNewRegistration();
            _rewards.Add(id, definition);
            return definition;
        }

        public FightDefinition RegisterFight(string localId, DefinitionId battle, int replays, int replayInterval,
            int power, int rounds, int roundTime, string location, string music, float evaluatedRating,
            float healthRecovery, string description, bool locked, string rewardImage,
            DefinitionId[] warriors, DefinitionId[] rules, DefinitionId[] rewards)
        {
            ThrowIfCompleted();
            DefinitionId id = Qualify("fights", localId);
            if (_fights.ContainsKey(id)) throw new ModContentException("Duplicate fight definition: '" + id + "'.");
            if (battle.Namespace != Mod.Id || battle.Category != "battles" || !_battles.ContainsKey(battle))
                throw new ModContentException("External fight must reference a battle registered by the same mod transaction.");
            warriors = warriors ?? Array.Empty<DefinitionId>();
            rules = rules ?? Array.Empty<DefinitionId>();
            rewards = rewards ?? Array.Empty<DefinitionId>();
            if (warriors.Length == 0) throw new ModContentException("External fight must reference at least one warrior.");
            ValidateDefinitionReferences(warriors, "warriors", id, "warrior");
            ValidateDefinitionReferences(rules, "rules", id, "rule");
            ValidateDefinitionReferences(rewards, "rewards", id, "reward");
            EnsureCapacityForNewRegistration();
            var definition = new FightDefinition(id, battle, id.ToString(), replays, replayInterval, power, rounds,
                roundTime, location, music, evaluatedRating, healthRecovery, description, locked, rewardImage,
                warriors, rules, rewards);
            _fights.Add(id, definition);
            _fightOrder.Add(id);
            return definition;
        }

        public DefinitionId PatchFightDescription(string reference, string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return StageFightPatch(reference, ModContentPolicies.FightDescription, value, 0);
        }

        public DefinitionId PatchFightRounds(string reference, int value)
        {
            if (value < 1 || value > 100) throw new ModContentException("Fight rounds patch must be 1..100.");
            return StageFightPatch(reference, ModContentPolicies.FightRounds, null, value);
        }

        public DefinitionId PatchFightRoundTime(string reference, int value)
        {
            if (value < 1 || value > 86400) throw new ModContentException("Fight round-time patch must be 1..86400 seconds.");
            return StageFightPatch(reference, ModContentPolicies.FightRoundTime, null, value);
        }

        public DefinitionId PatchFightLocation(string reference, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ModContentException("Fight location must not be empty.");
            return StageFightPatch(reference, ModContentPolicies.FightLocation, value, 0);
        }

        public DefinitionId PatchFightMusic(string reference, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ModContentException("Fight music must not be empty.");
            return StageFightPatch(reference, ModContentPolicies.FightMusic, value, 0);
        }

        public DefinitionId PatchFightRules(string reference, DefinitionId[] rules, bool append)
        {
            if (rules == null || rules.Length > 100) throw new ModContentException("Fight rules must contain at most 100 handles.");
            if (append && rules.Length == 0) throw new ModContentException("append_rules must not be empty.");
            ValidateDefinitionReferences(rules, "rules", default, "rule");
            return StageFightPatch(reference, ModContentPolicies.FightRules, null, 0, rules, append);
        }

        private DefinitionId StageFightPatch(string reference, string field, string stringValue, int intValue, DefinitionId[] rules = null, bool appendRules = false)
        {
            ThrowIfCompleted();
            if (string.IsNullOrWhiteSpace(reference)) throw new ModContentException("Fight patch target must not be empty.");
            DefinitionId id;
            try { id = reference.IndexOf(':') >= 0 ? DefinitionId.Parse(reference) : Qualify("fights", reference); }
            catch (FormatException exception) { throw new ModContentException(exception.Message, exception); }
            if (id.Category != "fights") throw new ModContentException("Fight patch target must use the fights category: '" + id + "'.");
            if (!CanReferenceNamespace(id.Namespace))
                throw new ModContentException("Mod '" + Mod.Id + "' cannot patch undeclared namespace '" + id.Namespace + "'.");
            FightDefinition existing;
            if (!_catalog.TryGetFight(id, out existing)) throw new ModContentException("Fight patch target is not registered: '" + id + "'.");
            ModContentPolicies.RequirePatchAllowed(id, field, ModContentPatchOperation.Replace);
            var key = new ModContentPatchKey(id, field);
            EnsureCapacityForNewRegistration();
            if (!_patchKeys.Add(key)) throw new ModContentException("Duplicate fight patch for '" + id + "' field '" + field + "'.");
            var record = new ModContentPatchRecord(Mod.Id, id, field, ModContentPatchOperation.Replace);
            _fightPatches.Add(new FightFieldPatch(record, stringValue, intValue, rules, appendRules));
            return id;
        }

        public void Commit()
        {
            ThrowIfCompleted();
            ValidateP1CCommit();
            ValidateP1BCommit();
            ValidateP1DCommit();
            ValidateP2Commit();
            ValidateP3Commit();

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
            var nonEquipmentItems = new NonEquipmentItemDefinition[_p1cItems.Count];
            _p1cItems.Values.CopyTo(nonEquipmentItems, 0);
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
            ZoneDefinition[] zones = BuildCommittedZones();
            BattleDefinition[] battles = BuildCommittedBattles();
            FightDefinition[] fights = SortedValues(_fights);
            WarriorDefinition[] warriors = SortedValues(_warriors);
            FightRuleDefinition[] fightRules = SortedValues(_fightRules);
            RewardDefinition[] rewards = SortedValues(_rewards);
            LocalizationValuePatch[] localizationPatches = _localizationPatches.ToArray();
            FightFieldPatch[] fightPatches = _fightPatches.ToArray();
            ModContentPatchRecord[] collectionPatches = _collectionPatches.ToArray();

            _catalog.Commit(this, localizations, weapons, armors, helms, ranged, magic, nonEquipmentItems,
                itemRedirects, listings,
                perks, enchantments, behaviors, zones, battles, fights, warriors, fightRules, rewards,
                localizationPatches, fightPatches, collectionPatches);
            ApplyP1CCommit();
            ApplyP1BCommit();
            ApplyP1DCommit();
            ApplyP2Commit();
            ApplyP3Commit();
            _completed = true;
            ClearPending();
        }

        private ZoneDefinition[] BuildCommittedZones()
        {
            var result = new List<ZoneDefinition>();
            foreach (ZoneDefinition zone in _zones.Values)
            {
                var children = new List<DefinitionId>();
                for (int i = 0; i < _battleOrder.Count; i++)
                {
                    BattleDefinition battle = _battles[_battleOrder[i]];
                    if (battle.Zone == zone.Id) children.Add(battle.Id);
                }
                result.Add(new ZoneDefinition(zone.Id, zone.LegacyName, zone.FileName, zone.IsStart, children.ToArray()));
            }
            result.Sort((left, right) => string.CompareOrdinal(left.Id.ToString(), right.Id.ToString()));
            return result.ToArray();
        }

        private BattleDefinition[] BuildCommittedBattles()
        {
            var result = new List<BattleDefinition>();
            foreach (BattleDefinition battle in _battles.Values)
            {
                var children = new List<DefinitionId>();
                for (int i = 0; i < _fightOrder.Count; i++)
                {
                    FightDefinition fight = _fights[_fightOrder[i]];
                    if (fight.Battle == battle.Id) children.Add(fight.Id);
                }
                result.Add(new BattleDefinition(battle.Id, battle.Zone, battle.LegacyName, battle.Kind,
                    battle.X, battle.Y, battle.Alias, battle.Title, battle.Icon, battle.Preview,
                    battle.Description, battle.Location, battle.Music, battle.RewardImage,
                    battle.ShowResistance, children.ToArray(), battle.IconAtlas, battle.EclipseToggleName,
                    battle.LegacyXml));
            }
            result.Sort((left, right) => string.CompareOrdinal(left.Id.ToString(), right.Id.ToString()));
            return result.ToArray();
        }

        private static T[] SortedValues<T>(Dictionary<DefinitionId, T> source) where T : class
        {
            var pairs = new List<KeyValuePair<DefinitionId, T>>(source);
            pairs.Sort((left, right) => string.CompareOrdinal(left.Key.ToString(), right.Key.ToString()));
            var result = new T[pairs.Count];
            for (int i = 0; i < pairs.Count; i++) result[i] = pairs[i].Value;
            return result;
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

        private static void ValidateRuleEnums(ModRuleTarget target, ModRuleMode mode)
        {
            if (!Enum.IsDefined(typeof(ModRuleTarget), target))
                throw new ModContentException("Unsupported rule target: " + target + ".");
            if (!Enum.IsDefined(typeof(ModRuleMode), mode))
                throw new ModContentException("Unsupported rule mode: " + mode + ".");
        }

        private void ValidateRewardReferences(RewardDefinition reward)
        {
            for (int i = 0; i < reward.Items.Count; i++) ValidateRewardItem(reward.Items[i]);
            for (int i = 0; i < reward.Choices.Count; i++)
            {
                RewardChoiceDefinition choice = reward.Choices[i];
                for (int j = 0; j < choice.Items.Count; j++) ValidateRewardItem(choice.Items[j].Grant);
            }
        }

        private void ValidateRewardItem(RewardItemGrant grant)
        {
            if (!CanReferenceNamespace(grant.Item.Namespace))
                throw new ModContentException("Reward item belongs to undeclared namespace '" + grant.Item.Namespace + "'.");
            ItemDefinition item;
            if (!TryGetPendingItem(grant.Item, out item) && !_catalog.TryResolveItem(grant.Item, out item))
                throw new ModContentException("Reward references missing item '" + grant.Item + "'.");
        }

        private void ValidateDefinitionReferences(DefinitionId[] references, string category, DefinitionId owner,
            string kind)
        {
            var seen = new HashSet<DefinitionId>();
            for (int i = 0; i < references.Length; i++)
            {
                DefinitionId reference = references[i];
                if (reference.Category != category || !CanReferenceNamespace(reference.Namespace))
                    throw new ModContentException("Fight '" + owner + "' references invalid or undeclared " + kind +
                        " '" + reference + "'.");
                bool exists;
                if (category == "warriors") exists = _warriors.ContainsKey(reference) || _catalog.TryGetWarrior(reference, out WarriorDefinition ignoredWarrior);
                else if (category == "rules") exists = _fightRules.ContainsKey(reference) || _catalog.TryGetFightRule(reference, out FightRuleDefinition ignoredRule);
                else if (category == "rewards") exists = _rewards.ContainsKey(reference) || _catalog.TryGetReward(reference, out RewardDefinition ignoredReward);
                else exists = false;
                if (!exists) throw new ModContentException("Fight '" + owner + "' references missing " + kind + " '" + reference + "'.");
                if (!seen.Add(reference)) throw new ModContentException("Fight '" + owner + "' repeats " + kind + " '" + reference + "'.");
            }
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
            if (TryGetPendingP1CItem(id, out value)) return true;
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
            _zones.Clear();
            _battles.Clear();
            _fights.Clear();
            _battleOrder.Clear();
            _fightOrder.Clear();
            _warriors.Clear();
            _fightRules.Clear();
            _rewards.Clear();
            _localizationPatches.Clear();
            _fightPatches.Clear();
            _collectionPatches.Clear();
            _patchKeys.Clear();
            _listedItems.Clear();
            ClearP1CPending();
            ClearP1BPending();
            ClearP1DPending();
            ClearP2Pending();
            ClearP3Pending();
        }
    }

    public sealed class ModContentException : Exception
    {
        public ModContentException(string message) : base(message) { }
        public ModContentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
