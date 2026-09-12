using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModLogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Debug = 3
    }

    public readonly struct ModLogEntry
    {
        public ModId ModId { get; }
        public ModLogLevel Level { get; }
        public string Message { get; }

        public ModLogEntry(ModId modId, ModLogLevel level, string message)
        {
            ModId = modId;
            Level = level;
            Message = message ?? string.Empty;
        }

        public override string ToString()
        {
            return "[" + ModId + "] " + Level + ": " + Message;
        }
    }

    public sealed partial class ModApiFacade
    {
        private readonly Action<ModLogEntry> _logger;

        public ModDescriptor Mod { get; }
        public AssetResolver Assets { get; }
        public ModRegistrationTransaction Registration { get; }
        public ModStateRuntime State { get; }

        public ModApiFacade(ModDescriptor mod, AssetResolver assets, Action<ModLogEntry> logger)
            : this(mod, assets, null, new ModStateRuntime(), logger)
        {
        }

        public ModApiFacade(ModDescriptor mod, AssetResolver assets, ModRegistrationTransaction registration,
            Action<ModLogEntry> logger)
            : this(mod, assets, registration, new ModStateRuntime(), logger)
        {
        }

        public ModApiFacade(ModDescriptor mod, AssetResolver assets, ModRegistrationTransaction registration,
            ModStateRuntime state, Action<ModLogEntry> logger)
        {
            Mod = mod ?? throw new ArgumentNullException(nameof(mod));
            Assets = assets ?? throw new ArgumentNullException(nameof(assets));
            if (registration != null && registration.Mod.Id != mod.Id)
                throw new ArgumentException("Registration transaction belongs to another mod.", nameof(registration));
            Registration = registration;
            State = state ?? throw new ArgumentNullException(nameof(state));
            _logger = logger;
        }

        public AssetId QualifyAsset(string reference)
        {
            AssetId id = Assets.Qualify(Mod.Id, reference);
            if (id.Namespace == Mod.Id) return id;

            foreach (ModDependency dependency in Mod.Manifest.Dependencies)
            {
                if (dependency.Id == id.Namespace) return id;
            }

            throw new InvalidOperationException("Mod '" + Mod.Id +
                "' cannot reference undeclared dependency namespace '" + id.Namespace + "'.");
        }

        public DefinitionId ValidateProfileReference(string reference, string category)
        {
            RequireCapability("profile.read");
            if (category != "items" && category != "perks")
                throw new ModContentException("Unsupported profile reference category.");
            var id = DefinitionId.Parse(reference);
            if (id.Category != category)
                throw new ModContentException("Expected a qualified " + category + " definition ID.");
            if (id.Namespace == Mod.Id) return id;
            foreach (ModDependency dependency in Mod.Manifest.Dependencies)
                if (dependency.Id == id.Namespace) return id;
            throw new ModContentException("Profile query references undeclared dependency namespace '" + id.Namespace + "'.");
        }

        public bool AssetExists(string reference)
        {
            AssetMetadata metadata;
            return Assets.TryDescribe(QualifyAsset(reference), out metadata);
        }

        public AssetId RequireAsset(string reference, AssetKind kind)
        {
            AssetId id = QualifyAsset(reference);
            AssetMetadata metadata;
            if (!Assets.TryDescribe(id, out metadata))
                throw new ModContentException("Asset does not exist: '" + id + "'.");
            if (metadata.Kind != kind)
                throw new ModContentException("Asset '" + id + "' is " + metadata.Kind +
                    ", expected " + kind + ".");
            return id;
        }

        public DefinitionId GetLocalization(string key)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetLocalization(key);
        }

        public string ReadLocalization(DefinitionId id, string language) => RequireRegistration().ReadLocalization(id, language);

        public DefinitionId PatchLocalization(string target, string language, string value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchLocalization(target, language, value);
        }

        public ModStateDefinition RegisterState(int version, ModParameterSchema fields,
            System.Collections.Generic.IReadOnlyDictionary<string, string> aliases,
            System.Collections.Generic.IReadOnlyCollection<string> tombstones)
        {
            RequireCapability("state.write");
            return State.RegisterDefinition(Mod, version, fields, aliases, tombstones);
        }

        public bool TryGetState(string name, out ModParameterValue value)
        {
            RequireCapability("state.read");
            return State.TryGetValue(Mod.Id, name, out value);
        }

        public void SetState(System.Collections.Generic.IReadOnlyDictionary<string, ModParameterValue> values)
        {
            RequireCapability("state.write");
            State.SetValues(Mod.Id, values);
        }

        public void UnsetState(string name)
        {
            RequireCapability("state.write");
            State.UnsetValue(Mod.Id, name);
        }

        // Persist the stream in an ordinary owned integer field. Keep this sequence
        // stable: saved states and reproducible mod runs depend on these constants.
        private int ReadRandomState(string field)
        {
            RequireCapability("state.read");
            RequireCapability("state.write");
            if (!State.TryGetValue(Mod.Id, field, out var value) ||
                value.Type != ModParameterType.Integer || value.Integer < int.MinValue || value.Integer > int.MaxValue)
                throw new ModContentException("Random stream requires a declared integer state field with a signed 32-bit value: '" + field + "'.");
            return (int)value.Integer;
        }

        private static uint NextRandomWord(ref int state)
        {
            unchecked
            {
                uint word = (uint)state + 0x9e3779b9u;
                state = (int)word;
                word = (word ^ (word >> 16)) * 0x85ebca6bu;
                word = (word ^ (word >> 13)) * 0xc2b2ae35u;
                return word ^ (word >> 16);
            }
        }

        private void CommitRandomState(string field, int state)
        {
            State.SetValues(Mod.Id, new Dictionary<string, ModParameterValue>(StringComparer.Ordinal)
            {
                [field] = ModParameterValue.FromInteger(state)
            });
        }

        public double RandomNumber(string field)
        {
            int state = ReadRandomState(field);
            double result = NextRandomWord(ref state) / 4294967296.0;
            CommitRandomState(field, state);
            return result;
        }

        public int RandomInteger(string field, int minimum, int maximum)
        {
            if (minimum > maximum)
                throw new ModContentException("Random integer minimum must not exceed maximum.");
            int state = ReadRandomState(field);
            ulong range = (ulong)((long)maximum - minimum + 1);
            ulong limit = (4294967296UL / range) * range;
            // Rejection avoids modulo bias. Bound native work independently of the
            // Lua instruction budget; a rejected call leaves the saved stream intact.
            for (int attempt = 0; attempt < 128; attempt++)
            {
                uint word = NextRandomWord(ref state);
                if (word >= limit) continue;
                int result = (int)(minimum + (long)(word % range));
                CommitRandomState(field, state);
                return result;
            }
            throw new ModContentException("Random integer rejection limit reached; saved stream was not advanced.");
        }

        public WeaponDefinition RegisterWeapon(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterWeapon(localId, displayName, icon, model, subType);
        }

        public ArmorDefinition RegisterArmor(string localId, DefinitionId displayName, AssetId icon,
            AssetId model)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterArmor(localId, displayName, icon, model);
        }

        public HelmDefinition RegisterHelm(string localId, DefinitionId displayName, AssetId icon,
            AssetId model)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterHelm(localId, displayName, icon, model);
        }

        public RangedDefinition RegisterRanged(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRanged(localId, displayName, icon, model, subType);
        }

        public MagicDefinition RegisterMagic(string localId, DefinitionId displayName, AssetId icon,
            AssetId model, string subType)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterMagic(localId, displayName, icon, model, subType);
        }

        public ItemDefinition GetItem(string reference)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetItem(reference);
        }

        public ItemRedirectDefinition RegisterItemAlias(string oldLocalPath, DefinitionId target)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterItemAlias(oldLocalPath, target);
        }

        public ItemRedirectDefinition RegisterItemTombstone(string oldLocalPath)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterItemTombstone(oldLocalPath);
        }

        public ShopListingDefinition RegisterShopListing(DefinitionId item, ModShopSection section,
            int level, ModPrice price)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterShopListing(item, section, level, price);
        }

        public PerkDefinition GetPerk(string reference)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetPerk(reference);
        }

        public ModBehaviorDefinition RegisterBehavior(string localId, ModParameterSchema parameters,
            ModParameterSchema state = null, string lifetime = "fight", int version = 1)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterBehavior(localId, parameters, state, lifetime, version);
        }

        public PerkDefinition RegisterPerk(string localId, DefinitionId template, DefinitionId displayName,
            DefinitionId description, AssetId icon, System.Collections.Generic.IReadOnlyDictionary<string, string> parameters)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterPerk(localId, template, displayName, description, icon, parameters);
        }

        public PerkDefinition SetPerkUpgrades(DefinitionId id, PerkUpgradeDefinition[] upgrades)
        {
            RequireCapability("content.register");
            return RequireRegistration().SetPerkUpgrades(id, upgrades);
        }

        public PerkDefinition RegisterScriptedPerk(string localId, DefinitionId displayName,
            DefinitionId description, AssetId icon, ModPerkKind kind, DefinitionId behavior,
            System.Collections.Generic.IReadOnlyDictionary<string, ModParameterValue> initialParameters)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterScriptedPerk(localId, displayName, description, icon, kind,
                behavior, initialParameters);
        }

        public EnchantmentDefinition RegisterEnchantment(string localId, DefinitionId perk,
            ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterEnchantment(localId, perk, recipe, equipment);
        }

        public EnchantmentDefinition RegisterScriptedEnchantment(string localId, DefinitionId displayName,
            DefinitionId description, AssetId icon, ModEnchantmentRecipe recipe, ModEquipmentKind[] equipment,
            DefinitionId behavior,
            System.Collections.Generic.IReadOnlyDictionary<string, ModParameterValue> initialParameters)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterScriptedEnchantment(localId, displayName, description, icon,
                recipe, equipment, behavior, initialParameters);
        }

        public ZoneDefinition RegisterZone(string localId, string fileName, bool isStart)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterZone(localId, fileName, isStart);
        }

        public ZoneDefinition GetZone(string reference)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetZone(reference);
        }

        public BattleDefinition RegisterBattle(string localId, DefinitionId zone, ModBattleKind kind,
            int x, int y, string alias, string title, string icon, string preview, string description,
            string location, string music, string rewardImage, bool showResistance, string iconAtlas,
            string eclipseToggleName)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterBattle(localId, zone, kind, x, y, alias, title, icon,
                preview, description, location, music, rewardImage, showResistance, iconAtlas, eclipseToggleName);
        }

        public WarriorDefinition RegisterWarrior(string localId, string firstName, string lastName, string avatar,
            string voice, int level, string tactic, DefinitionId[] items, DefinitionId[] perks,
            DefinitionId template, bool hasTemplate, string group, int random,
            System.Collections.Generic.IReadOnlyDictionary<string, float> attributes,
            WarriorAttributeAlignmentDefinition[] attributeAlignments, int healthBars = 0,
            AssetId bodyModel = default, AssetId[] skinModels = null)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterWarrior(localId, firstName, lastName, avatar, voice, level,
                tactic, items, perks, template, hasTemplate, group, random, attributes, attributeAlignments, healthBars, bodyModel, skinModels);
        }

        public WarriorTemplateDefinition GetWarriorTemplate(string reference)
        {
            RequireCapability("content.register");
            return RequireRegistration().GetWarriorTemplate(reference);
        }

        public FightRuleDefinition RegisterBehaviorRule(string localId, DefinitionId behavior, ModRuleTarget target,
            ModRuleMode mode, int[] rounds, IReadOnlyDictionary<string, ModParameterValue> parameters)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterBehaviorRule(localId, behavior, target, mode, rounds, parameters);
        }

        public FightRuleDefinition RegisterNoPerksRule(string localId, ModRuleTarget target, ModRuleMode mode,
            int[] rounds, string name)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterNoPerksRule(localId, target, mode, rounds, name);
        }

        public FightRuleDefinition RegisterRequireItemRule(string localId, DefinitionId item, int minimumLevel,
            ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRequireItemRule(localId, item, minimumLevel, mode, rounds);
        }

        public FightRuleDefinition RegisterEquipItemRule(string localId, DefinitionId item, int minimumLevel,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterEquipItemRule(localId, item, minimumLevel, target, mode, rounds);
        }

        public FightRuleDefinition RegisterNamedRule(string localId, ModFightRuleKind kind, string name,
            ModRuleTarget target, ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterNamedRule(localId, kind, name, target, mode, rounds);
        }

        public FightRuleDefinition RegisterPerkRule(string localId, DefinitionId perk, ModRuleTarget target,
            ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterPerkRule(localId, perk, target, mode, rounds);
        }

        public FightRuleDefinition RegisterRechargeMagicRule(string localId, ModRuleTarget target,
            ModRuleMode mode, int[] rounds)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterRechargeMagicRule(localId, target, mode, rounds);
        }

        public FightRuleDefinition RegisterAttributesRule(string localId, ModRuleTarget target, ModRuleMode mode,
            int[] rounds, System.Collections.Generic.IReadOnlyDictionary<string, float> attributes)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterAttributesRule(localId, target, mode, rounds, attributes);
        }

        public RewardDefinition RegisterReward(string localId, RewardItemGrant[] items,
            RewardChoiceDefinition[] choices, int gems = 0)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterReward(localId, items, choices, gems);
        }

        public FightDefinition RegisterFight(string localId, DefinitionId battle, int replays, int replayInterval,
            int power, int rounds, int roundTime, string location, string music, float evaluatedRating,
            float healthRecovery, string description, bool locked, string rewardImage, DefinitionId[] warriors,
            DefinitionId[] rules, DefinitionId[] rewards)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterFight(localId, battle, replays, replayInterval, power, rounds,
                roundTime, location, music, evaluatedRating, healthRecovery, description, locked, rewardImage,
                warriors, rules, rewards);
        }

        public DefinitionId PatchFightLocation(string target, string value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightLocation(target, value);
        }
        public DefinitionId PatchFightMusic(string target, string value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightMusic(target, value);
        }
        public DefinitionId PatchFightRules(string target, DefinitionId[] rules, bool append)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightRules(target, rules, append);
        }

        public DefinitionId PatchFightDescription(string target, string value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightDescription(target, value);
        }

        // Host-only call boundary; no Lua function accepts this delegate.
        public void StageFightPatchCall(Action stage)
        {
            RequireCapability("content.patch");
            RequireRegistration().StageFightPatchCall(stage);
        }

        public DefinitionId PatchFightWarriors(string target, DefinitionId[] warriors)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightWarriors(target, warriors);
        }

        public DefinitionId PatchFightRewardDrops(string target, int wins, ModRuleMode mode,
            int? minimumLevel, int? maximumLevel, DefinitionId reward)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightRewardDrops(target, wins, mode, minimumLevel, maximumLevel, reward);
        }

        public DefinitionId PatchFightRounds(string target, int value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightRounds(target, value);
        }

        public DefinitionId PatchFightRoundTime(string target, int value)
        {
            RequireCapability("content.patch");
            return RequireRegistration().PatchFightRoundTime(target, value);
        }

        public DefinitionId SuppressQuest(string target)
        {
            RequireCapability("content.patch");
            return RequireRegistration().SuppressQuest(target);
        }

        public QuestDefinition RegisterQuest(string localId, int priority, bool unresumable, bool allowDoubles,
            ModQuestActionPlace place, string[] groups, string[] marks, ModQuestEventKind[] events,
            ModQuestCondition[] conditions, ModQuestAction[] actions)
        {
            RequireCapability("content.register");
            return RequireRegistration().RegisterQuest(localId, priority, unresumable, allowDoubles, place,
                groups, marks, events, conditions, actions);
        }

        public bool HasCapability(string capability)
        {
            if (string.IsNullOrEmpty(capability)) return false;
            foreach (string value in Mod.Manifest.Capabilities)
                if (string.Equals(value, capability, StringComparison.Ordinal)) return true;
            return false;
        }

        public void RequireCapability(string capability)
        {
            if (!HasCapability(capability))
                throw new ModContentException("Mod '" + Mod.Id + "' did not declare required capability '" +
                    capability + "'.");
        }

        public void Log(ModLogLevel level, string message)
        {
            _logger?.Invoke(new ModLogEntry(Mod.Id, level, message));
        }

        private ModRegistrationTransaction RequireRegistration()
        {
            if (Registration == null)
                throw new InvalidOperationException("This script context has no content registration transaction.");
            return Registration;
        }
    }

    public interface IModModeScriptContext
    {
        bool TryChooseModeNext(ModModeDefinition mode, bool won, int step, int completions, out int? selectedStep, out string error);
    }

    public interface IModAiScriptContext
    {
        bool HasAiHandler(string tactic);
        bool TryDecideAi(string tactic, object instance, ModCombatSnapshot snapshot, IReadOnlyList<string> actions,
            out int? selection, out string error);
    }

    public interface IModModePrepareScriptContext
    {
        bool TryPrepareMode(ModModeDefinition mode, int step, int completions, ModModeRequest request, out string error);
    }

    public interface IModScriptRuntime
    {
        string Name { get; }
        IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api);
    }

    public enum ModEffectEvent
    {
        FightBegin = 0,
        DamageReceived = 1,
        DamageDealt = 2,
        RoundBegin = 3,
        RoundEnd = 4,
        FightEnd = 5,
        Block = 6,
        Critical = 7,
        DamageResolving = 8,
        DamageDealing = 9,
        ComboChanged = 10,
        StyleChanged = 11,
        Tick = 12
    }

    public interface IModScriptContext : IDisposable
    {
        ModDescriptor Mod { get; }
        void ExecuteEntrypoint();
    }

    public interface IModBehaviorScriptContext
    {
        bool HasBehaviorHandler(DefinitionId behaviorId, ModEffectEvent effectEvent);
        bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
            System.Collections.Generic.IReadOnlyDictionary<string, ModParameterValue> parameters,
            System.Collections.Generic.IReadOnlyDictionary<string, string> context,
            out string error);
    }

    // Combat handlers receive only this narrow capability surface. Implementations live in the
    // recovered fight assembly and may delegate to authoritative engine operations, but scripts
    // never receive the backing Model/Fight objects themselves.
    public interface IModFighterOperations
    {
        bool TryChangeHealth(double amount, out string error);
        bool TryAddMagicCharge(double amount, out string error);
    }

    // Immutable observations of a resolved hit, never a live engine object.
    public sealed class ModDamageEvent
    {
        public int Round { get; }
        public double HealthBefore { get; }
        public double HealthAfter { get; }
        public double Damage => System.Math.Max(0, HealthBefore - HealthAfter);
        public bool Blocked { get; }
        public bool Critical { get; }
        public ModDamageEvent(int round, double before, double after, bool blocked, bool critical)
        {
            if (round < 1 || double.IsNaN(before) || double.IsInfinity(before) ||
                double.IsNaN(after) || double.IsInfinity(after) || before < 0 || after < 0)
                throw new System.ArgumentOutOfRangeException(nameof(before));
            Round = round; HealthBefore = before; HealthAfter = after; Blocked = blocked; Critical = critical;
        }
    }

    // Detached observations. These types deliberately contain no recovered engine references.
    public sealed class ModFighterSnapshot
    {
        public double Health { get; }
        public double MaxHealth { get; }
        public int HealthBars { get; }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public ModFighterSnapshot(double health, double maxHealth, int healthBars, double x, double y, double z)
        {
            foreach (double value in new[] { health, maxHealth, x, y, z })
                if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(health));
            if (health < 0 || maxHealth < 0 || healthBars < 1) throw new ArgumentOutOfRangeException(nameof(health));
            Health = health; MaxHealth = maxHealth; HealthBars = healthBars; X = x; Y = y; Z = z;
        }
    }

    public sealed class ModCombatSnapshot
    {
        public ModFighterSnapshot Self { get; }
        public ModFighterSnapshot Opponent { get; }
        public int Frame { get; }
        public double Seconds => Frame / 60d;
        public bool RoundActive { get; }
        public ModCombatSnapshot(ModFighterSnapshot self, ModFighterSnapshot opponent, int frame, bool roundActive)
        {
            Self = self ?? throw new ArgumentNullException(nameof(self));
            if (frame < 0) throw new ArgumentOutOfRangeException(nameof(frame));
            Opponent = opponent; Frame = frame; RoundActive = roundActive;
        }
    }

    public interface IModCombatSnapshotSource
    {
        ModCombatSnapshot CaptureCombatSnapshot();
    }

    public sealed class ModCombatActivityEvent
    {
        public ModEffectEvent Type { get; }
        public int Combo { get; }
        public int LastCombo { get; }
        public int StyleRank { get; }
        public string StyleName { get; }
        public double StyleGain { get; }
        public bool IsHit { get; }
        private ModCombatActivityEvent(ModEffectEvent type, int combo, int lastCombo, int rank, string name, double gain, bool isHit)
        { Type = type; Combo = combo; LastCombo = lastCombo; StyleRank = rank; StyleName = name; StyleGain = gain; IsHit = isHit; }
        public static ModCombatActivityEvent ComboChange(int combo, int lastCombo)
        {
            if (combo < 0 || lastCombo < 0) throw new ArgumentOutOfRangeException(nameof(combo));
            return new ModCombatActivityEvent(ModEffectEvent.ComboChanged, combo, lastCombo, 0, "", 0, false);
        }
        public static ModCombatActivityEvent StyleChange(int rank, string name, double gain, bool isHit)
        {
            if (rank < 0 || name == null || double.IsNaN(gain) || double.IsInfinity(gain)) throw new ArgumentOutOfRangeException(nameof(rank));
            return new ModCombatActivityEvent(ModEffectEvent.StyleChanged, 0, 0, rank, name, gain, isHit);
        }
    }
    public interface IModCombatActivitySource { ModCombatActivityEvent ActivityEvent { get; } }

    public interface IModDamageEventSource
    {
        ModDamageEvent DamageEvent { get; }
    }

    /// <summary>Combat-owned rule instances. Never attached to profile XML or equipment.</summary>
    public sealed class ModBattleRuleInstances
    {
        private readonly List<FightRuleDefinition> _rules = new List<FightRuleDefinition>();
        private readonly Dictionary<(DefinitionId, bool), System.Xml.XmlNode> _instances =
            new Dictionary<(DefinitionId, bool), System.Xml.XmlNode>();
        private bool _initialized;

        public IEnumerable<FightRuleDefinition> Applicable(ModContentCatalog content, string runtimeFightId,
            bool player, int round, bool eclipse)
        {
            if (!_initialized)
            {
                _initialized = true;
                foreach (var fight in content.Fights)
                {
                    if (content.RuntimeFightId(fight.Id) != runtimeFightId) continue;
                    var seen = new HashSet<DefinitionId>();
                    foreach (var id in fight.Rules)
                        if (seen.Add(id) && content.TryGetFightRule(id, out var rule) && rule.Kind == ModFightRuleKind.Behavior)
                            _rules.Add(rule);
                    break;
                }
            }
            foreach (var rule in _rules)
            {
                if (rule.Target == ModRuleTarget.Player && !player || rule.Target == ModRuleTarget.Opponent && player) continue;
                if (rule.Mode == ModRuleMode.Normal && eclipse || rule.Mode == ModRuleMode.Eclipse && !eclipse) continue;
                if (rule.Rounds.Count != 0)
                {
                    bool applies = false;
                    foreach (int number in rule.Rounds) if (number == round) applies = true;
                    if (!applies) continue;
                }
                yield return rule;
            }
        }

        public System.Xml.XmlNode Instance(DefinitionId rule, bool player)
        {
            if (!_instances.TryGetValue((rule, player), out var node))
            {
                var document = new System.Xml.XmlDocument();
                node = document.CreateElement("BattleRuleInstance"); document.AppendChild(node);
                _instances.Add((rule, player), node);
            }
            return node;
        }
    }

    public interface IModBehaviorInstanceSource
    {
        System.Xml.XmlNode SavedInstance { get; }
    }

    // Adds persistence provenance without exposing the saved XML to Lua.
    public interface IModFighterEffects
    {
        bool TrySetDamageShield(object key, double fraction, int frames, out string error);
        bool TryRemoveDamageShield(object key, out string error);
    }
    public interface IModIncomingHitSource { ModIncomingHit IncomingHit { get; } }
    public sealed class ModIncomingHit
    {
        private readonly Func<double> _read;
        private readonly Action<double> _write;
        public double Damage => _read();
        public bool Blocked { get; }
        public bool Critical { get; }
        public ModIncomingHit(Func<double> read, Action<double> write, bool blocked = false, bool critical = false)
        { _read = read; _write = write; Blocked = blocked; Critical = critical; }
        public bool TryScale(double scale, out string error)
        {
            error = "";
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale < 0 || scale > 1) { error = "Incoming damage scale must be 0..1."; return false; }
            _write(Damage * scale); return true;
        }
        public bool TryScaleOutgoing(double scale, out string error)
        {
            error = "";
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale < 0 || scale > 16)
            { error = "Outgoing damage scale must be 0..16."; return false; }
            double value = Damage * scale;
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > float.MaxValue)
            { error = "Outgoing damage must remain a finite nonnegative single-precision value."; return false; }
            _write(value); return true;
        }
    }
    public interface IModFighterTargets
    {
        double Health { get; }
        IModFighterOperations Opponent { get; }
    }

    public sealed class ModInstanceFighter : IModFighterOperations, IModDamageEventSource, IModBehaviorInstanceSource, IModFighterTargets, IModIncomingHitSource, IModFighterEffects, IModCombatSnapshotSource, IModCombatActivitySource
    {
        private readonly IModFighterOperations _inner;
        public ModCombatActivityEvent ActivityEvent => (_inner as IModCombatActivitySource)?.ActivityEvent;
        public ModCombatSnapshot CaptureCombatSnapshot() => (_inner as IModCombatSnapshotSource)?.CaptureCombatSnapshot();
        public System.Xml.XmlNode SavedInstance { get; }
        public ModDamageEvent DamageEvent => (_inner as IModDamageEventSource)?.DamageEvent;
        public ModIncomingHit IncomingHit => (_inner as IModIncomingHitSource)?.IncomingHit;
        public double Health => (_inner as IModFighterTargets)?.Health ?? 0;
        public IModFighterOperations Opponent => (_inner as IModFighterTargets)?.Opponent;
        public bool TrySetDamageShield(object key, double fraction, int frames, out string error)
        {
            if (SavedInstance != null && _inner is IModFighterEffects effects) return effects.TrySetDamageShield((SavedInstance, key), fraction, frames, out error);
            error = "Timed effects are unavailable."; return false;
        }
        public bool TryRemoveDamageShield(object key, out string error)
        {
            if (SavedInstance != null && _inner is IModFighterEffects effects) return effects.TryRemoveDamageShield((SavedInstance, key), out error);
            error = "Timed effects are unavailable."; return false;
        }
        public ModInstanceFighter(IModFighterOperations inner, System.Xml.XmlNode node) { _inner = inner; SavedInstance = node; }
        public bool TryChangeHealth(double amount, out string error)
        {
            error = "Fighter unavailable."; return _inner != null && _inner.TryChangeHealth(amount, out error);
        }
        public bool TryAddMagicCharge(double amount, out string error)
        {
            error = "Fighter unavailable."; return _inner != null && _inner.TryAddMagicCharge(amount, out error);
        }
    }

    public interface IModInteractiveBehaviorScriptContext
    {
        bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
            System.Collections.Generic.IReadOnlyDictionary<string, ModParameterValue> parameters,
            System.Collections.Generic.IReadOnlyDictionary<string, string> context,
            IModFighterOperations fighter, out string error);
    }

    public sealed class ModScriptException : Exception
    {
        public ModId ModId { get; }
        public string SourceName { get; }

        public ModScriptException(ModId modId, string sourceName, string message, Exception innerException = null)
            : base(message, innerException)
        {
            ModId = modId;
            SourceName = sourceName ?? string.Empty;
        }
    }
}
