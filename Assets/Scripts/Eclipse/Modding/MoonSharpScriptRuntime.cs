using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime : IModScriptRuntime
    {
        public const int MaxSourceBytes = 1024 * 1024;
        public const int MaxModules = 128;
        public const long InstructionSlice = 50000;
        public const int MaxInstructionSlices = 100;
        public const long MaxEntrypointInstructions = InstructionSlice * MaxInstructionSlices;
        public const int MaxBehaviorInstructionSlices = 4;
        public const long MaxBehaviorInstructions = InstructionSlice * MaxBehaviorInstructionSlices;
        public const int MaxStateMigrationInstructionSlices = 20;
        public const long MaxStateMigrationInstructions = InstructionSlice * MaxStateMigrationInstructionSlices;

        public string Name => "MoonSharp " + Script.VERSION;

        public IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api)
        {
            return new MoonSharpScriptContext(mod, api);
        }

        private sealed partial class MoonSharpScriptContext : IModScriptContext, IModBehaviorScriptContext,
            IModInteractiveBehaviorScriptContext, IModStateMigrationScriptContext
        {
            private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

            private readonly ModApiFacade _api;
            private readonly Script _script;
            private readonly Dictionary<string, DynValue> _modules =
                new Dictionary<string, DynValue>(StringComparer.Ordinal);
            private readonly HashSet<string> _loading = new HashSet<string>(StringComparer.Ordinal);
            private readonly Dictionary<Table, DefinitionId> _localizationHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, AssetId> _spriteHandles =
                new Dictionary<Table, AssetId>();
            private readonly Dictionary<Table, AssetId> _modelHandles =
                new Dictionary<Table, AssetId>();
            private readonly Dictionary<Table, DefinitionId> _itemHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _itemSetHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _forgeProfileHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _forgeRecipeHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, ModPrice> _priceHandles =
                new Dictionary<Table, ModPrice>();
            private readonly Dictionary<Table, DefinitionId> _perkHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _enchantmentHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, ModBehaviorDefinition> _behaviorHandles =
                new Dictionary<Table, ModBehaviorDefinition>();
            private readonly Dictionary<Table, DefinitionId> _zoneHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _battleHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _warriorHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _warriorTemplateHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _ruleHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _rewardHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _fightHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _questHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<(DefinitionId, ModEffectEvent), DynValue> _behaviorHandlers =
                new Dictionary<(DefinitionId, ModEffectEvent), DynValue>();
            private readonly Dictionary<int, DynValue> _stateMigrations = new Dictionary<int, DynValue>();
            private ModStateDefinition _stateDefinition;
            private bool _disposed;

            public ModDescriptor Mod { get; }

            public MoonSharpScriptContext(ModDescriptor mod, ModApiFacade api)
            {
                Mod = mod ?? throw new ArgumentNullException(nameof(mod));
                _api = api ?? throw new ArgumentNullException(nameof(api));
                if (api.Mod.Id != mod.Id)
                    throw new ArgumentException("Script API facade belongs to another mod.", nameof(api));

                _script = new Script(CoreModules.Preset_HardSandbox);
                _script.Options.DebugPrint = message => _api.Log(ModLogLevel.Info, message);
                _script.Options.DebugInput = prompt => throw new ScriptRuntimeException("Interactive input is disabled.");
                _script.Globals.Set("require", DynValue.NewCallback(Require));
            }

            public void ExecuteEntrypoint()
            {
                ThrowIfDisposed();
                string sourceName = Mod.Manifest.Entrypoint;
                try
                {
                    DynValue function = LoadChunk(EntrypointId(), sourceName);
                    RunBounded(function, sourceName);
                }
                catch (InterpreterException exception)
                {
                    throw Wrap(sourceName, exception);
                }
                catch (Exception exception) when (exception is IOException || exception is InvalidDataException ||
                    exception is FormatException || exception is UnauthorizedAccessException)
                {
                    throw new ModScriptException(Mod.Id, sourceName,
                        "Failed to execute mod entrypoint '" + sourceName + "': " + exception.Message, exception);
                }
            }

            public bool HasBehaviorHandler(DefinitionId behaviorId, ModEffectEvent effectEvent)
            {
                ThrowIfDisposed();
                return _behaviorHandlers.ContainsKey((behaviorId, effectEvent));
            }

            public bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
                IReadOnlyDictionary<string, ModParameterValue> parameters,
                IReadOnlyDictionary<string, string> context, out string error)
            {
                return TryInvokeBehavior(behaviorId, effectEvent, parameters, context, null, out error);
            }

            public bool TryInvokeBehavior(DefinitionId behaviorId, ModEffectEvent effectEvent,
                IReadOnlyDictionary<string, ModParameterValue> parameters,
                IReadOnlyDictionary<string, string> context, IModFighterOperations fighter, out string error)
            {
                ThrowIfDisposed();
                error = string.Empty;
                if (effectEvent != ModEffectEvent.FightBegin && effectEvent != ModEffectEvent.DamageReceived)
                {
                    error = "Unsupported behavior event: " + effectEvent + ".";
                    return false;
                }
                DynValue handler;
                if (!_behaviorHandlers.TryGetValue((behaviorId, effectEvent), out handler))
                {
                    error = "Behavior handler is not registered: '" + behaviorId + "'.";
                    return false;
                }

                bool invocationActive = true;
                try
                {
                    var parameterTable = new Table(_script);
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, ModParameterValue> pair in parameters)
                            parameterTable.Set(pair.Key, ToDynValue(pair.Value));
                    }
                    var fighterTable = new Table(_script);
                    if (context != null)
                    {
                        foreach (KeyValuePair<string, string> pair in context)
                            fighterTable.Set(pair.Key, DynValue.NewString(pair.Value ?? string.Empty));
                    }
                    if (fighter != null)
                    {
                        fighterTable.Set("change_health", DynValue.NewCallback((ctx, args) =>
                            invocationActive ? FighterOperation("fighter:change_health", "combat.change_life", args, fighter.TryChangeHealth) :
                                throw new ScriptRuntimeException("Fighter operations have expired.")));
                        fighterTable.Set("add_magic_charge", DynValue.NewCallback((ctx, args) =>
                            invocationActive ? FighterOperation("fighter:add_magic_charge", "combat.magic_charge", args, fighter.TryAddMagicCharge) :
                                throw new ScriptRuntimeException("Fighter operations have expired.")));
                    }
                    var eventTable = new Table(_script);
                    ModDamageEvent damage = (fighter as IModDamageEventSource)?.DamageEvent;
                    if (effectEvent == ModEffectEvent.DamageReceived)
                    {
                        if (damage == null) throw new ModContentException("Damage event snapshot is missing.");
                        eventTable.Set("round", DynValue.NewNumber(damage.Round));
                        eventTable.Set("health_before", DynValue.NewNumber(damage.HealthBefore));
                        eventTable.Set("health_after", DynValue.NewNumber(damage.HealthAfter));
                        eventTable.Set("damage", DynValue.NewNumber(damage.Damage));
                        eventTable.Set("blocked", DynValue.NewBoolean(damage.Blocked));
                        eventTable.Set("critical", DynValue.NewBoolean(damage.Critical));
                    }
                    RunBounded(handler, behaviorId + ":" + effectEvent, MaxBehaviorInstructionSlices,
                        new[] { DynValue.NewTable(parameterTable), DynValue.NewTable(fighterTable), DynValue.NewTable(eventTable) });
                    return true;
                }
                catch (InterpreterException exception)
                {
                    error = exception.DecoratedMessage ?? exception.Message;
                    return false;
                }
                catch (Exception exception)
                {
                    error = exception.Message;
                    return false;
                }
                finally { invocationActive = false; }
            }

            public bool TryMigrateState(int fromVersion, IReadOnlyDictionary<string, ModParameterValue> values,
                out IReadOnlyDictionary<string, ModParameterValue> migrated, out string error)
            {
                ThrowIfDisposed();
                migrated = null;
                error = string.Empty;
                DynValue handler;
                if (!_stateMigrations.TryGetValue(fromVersion, out handler))
                {
                    error = "No migration handler is registered for state schema " + fromVersion + " -> " +
                        (fromVersion + 1) + ".";
                    return false;
                }

                try
                {
                    var table = new Table(_script);
                    if (values != null)
                    {
                        foreach (KeyValuePair<string, ModParameterValue> pair in values)
                            table.Set(pair.Key, ToDynValue(pair.Value));
                    }
                    DynValue result = RunBounded(handler, Mod.Id + ":state-migration-" + fromVersion,
                        MaxStateMigrationInstructionSlices, new[] { DynValue.NewTable(table) });
                    Table output;
                    if (result == null || result.IsNil()) output = table;
                    else if (result.Type == DataType.Table) output = result.Table;
                    else
                    {
                        error = "State migration must return a table or nil after mutating its input table.";
                        return false;
                    }
                    migrated = ReadMigratedState(output, values);
                    return true;
                }
                catch (InterpreterException exception)
                {
                    error = exception.DecoratedMessage ?? exception.Message;
                    return false;
                }
                catch (Exception exception)
                {
                    error = exception.Message;
                    return false;
                }
            }

            private IReadOnlyDictionary<string, ModParameterValue> ReadMigratedState(Table table,
                IReadOnlyDictionary<string, ModParameterValue> previous)
            {
                var result = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
                foreach (TablePair pair in table.Pairs)
                {
                    if (result.Count >= ModStateRuntime.MaxSavedValues)
                        throw new ModContentException("Migrated state exceeds the saved value limit (" +
                            ModStateRuntime.MaxSavedValues + ").");
                    if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String))
                        throw new ModContentException("State migration produced a non-string field name.");
                    string name = pair.Key.String;
                    ModParameterDefinition.ValidateName(name);
                    if (result.ContainsKey(name))
                        throw new ModContentException("State migration produced duplicate field '" + name + "'.");

                    ModParameterType type;
                    ModParameterDefinition current;
                    ModParameterValue oldValue;
                    if (_stateDefinition != null && _stateDefinition.Fields.TryGet(name, out current))
                        type = current.Type;
                    else if (previous != null && previous.TryGetValue(name, out oldValue))
                        type = oldValue.Type;
                    else
                        type = InferMigrationType(pair.Value, name);
                    result.Add(name, ParameterValue(type, pair.Value, "State migration field '" + name + "'"));
                }
                return result;
            }

            private static ModParameterType InferMigrationType(DynValue value, string name)
            {
                switch (value.Type)
                {
                    case DataType.Number: return ModParameterType.Number;
                    case DataType.Boolean: return ModParameterType.Boolean;
                    case DataType.String: return ModParameterType.String;
                    default: throw new ModContentException("State migration field '" + name +
                        "' must be a number, boolean, or string.");
                }
            }

            private delegate bool FighterOperationDelegate(double amount, out string error);

            private DynValue FighterOperation(string function, string capability, CallbackArguments args,
                FighterOperationDelegate operation)
            {
                try
                {
                    _api.RequireCapability(capability);
                }
                catch (ModContentException exception)
                {
                    throw new ScriptRuntimeException(exception.Message);
                }
                int valueIndex = args.Count > 1 && args[0].Type == DataType.Table ? 1 : 0;
                double amount = args.AsType(valueIndex, function, DataType.Number, false).Number;
                if (double.IsNaN(amount) || double.IsInfinity(amount) || amount < -float.MaxValue || amount > float.MaxValue)
                    throw new ScriptRuntimeException(function + " amount must be a finite single-precision number.");
                string error;
                if (!operation(amount, out error))
                    throw new ScriptRuntimeException(string.IsNullOrEmpty(error) ? function + " failed." : error);
                return DynValue.Nil;
            }

            private static DynValue ToDynValue(ModParameterValue value)
            {
                switch (value.Type)
                {
                    case ModParameterType.Number: return DynValue.NewNumber(value.Number);
                    case ModParameterType.Integer: return DynValue.NewNumber(value.Integer);
                    case ModParameterType.Boolean: return DynValue.NewBoolean(value.Boolean);
                    case ModParameterType.String: return DynValue.NewString(value.String);
                    default: throw new InvalidOperationException("Unsupported parameter type: " + value.Type);
                }
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _modules.Clear();
                _loading.Clear();
                _localizationHandles.Clear();
                _spriteHandles.Clear();
                _modelHandles.Clear();
                _itemHandles.Clear();
                _itemSetHandles.Clear();
                _forgeProfileHandles.Clear();
                _forgeRecipeHandles.Clear();
                _priceHandles.Clear();
                _perkHandles.Clear();
                _enchantmentHandles.Clear();
                _behaviorHandles.Clear();
                _zoneHandles.Clear();
                _battleHandles.Clear();
                _warriorHandles.Clear();
                _warriorTemplateHandles.Clear();
                _ruleHandles.Clear();
                _rewardHandles.Clear();
                _fightHandles.Clear();
                _questHandles.Clear();
                ClearP1DHandles();
                _behaviorHandlers.Clear();
            }

            private DynValue Require(ScriptExecutionContext context, CallbackArguments args)
            {
                string module = args.AsType(0, "require", DataType.String, false).String;
                if (string.Equals(module, "sf2", StringComparison.Ordinal))
                    return GetSf2Module();

                string canonical = CanonicalModuleName(module);
                DynValue cached;
                if (_modules.TryGetValue(canonical, out cached)) return cached;
                if (_modules.Count >= MaxModules)
                    throw new ScriptRuntimeException("Module limit exceeded (" + MaxModules + ").");
                if (!_loading.Add(canonical))
                    throw new ScriptRuntimeException("Circular require detected for module '" + canonical + "'.");

                try
                {
                    AssetId id = AssetId.Parse(Mod.Id.Value + ":scripts/" + canonical.Replace('.', '/'));
                    string sourceName = "scripts/" + canonical.Replace('.', '/') + ".lua";
                    DynValue function = LoadChunk(id, sourceName);
                    return DynValue.NewTailCallReq(new TailCallData
                    {
                        Function = function,
                        Args = new[] { DynValue.NewString(canonical) },
                        Continuation = new CallbackFunction((ctx, returned) =>
                        {
                            DynValue value = returned.Count == 0 ? DynValue.True : returned[0].ToScalar();
                            if (value == null || value.IsNil()) value = DynValue.True;
                            _modules[canonical] = value;
                            _loading.Remove(canonical);
                            return value;
                        }, "require:" + canonical)
                    });
                }
                catch
                {
                    _loading.Remove(canonical);
                    throw;
                }
            }

            private DynValue RunBounded(DynValue function, string sourceName)
            {
                return RunBounded(function, sourceName, MaxInstructionSlices, Array.Empty<DynValue>());
            }

            private DynValue RunBounded(DynValue function, string sourceName, int maxSlices, DynValue[] args)
            {
                DynValue coroutineValue = _script.CreateCoroutine(function);
                Coroutine coroutine = coroutineValue.Coroutine;
                coroutine.AutoYieldCounter = InstructionSlice;

                int forcedYields = 0;
                bool firstResume = true;
                while (true)
                {
                    DynValue result = firstResume ? coroutine.Resume(args) : coroutine.Resume();
                    firstResume = false;
                    if (coroutine.State == CoroutineState.Dead) return result;
                    if (coroutine.State == CoroutineState.ForceSuspended)
                    {
                        forcedYields++;
                        if (forcedYields >= maxSlices)
                            throw new ScriptRuntimeException("Execution instruction budget exceeded in '" +
                                sourceName + "' (limit " + (InstructionSlice * maxSlices) + ").");
                        continue;
                    }

                    throw new ScriptRuntimeException("Unexpected Lua yield in '" + sourceName + "'.");
                }
            }

            private DynValue GetSf2Module()
            {
                const string moduleName = "sf2";
                DynValue cached;
                if (_modules.TryGetValue(moduleName, out cached)) return cached;

                var root = new Table(_script);
                var mod = new Table(_script);
                mod.Set("id", DynValue.NewString(Mod.Id.Value));
                mod.Set("name", DynValue.NewString(Mod.Manifest.Name));
                mod.Set("version", DynValue.NewString(Mod.Version.ToString()));
                var log = new Table(_script);
                log.Set("debug", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Debug, "sf2.log.debug", args)));
                log.Set("info", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Info, "sf2.log.info", args)));
                log.Set("warn", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Warning, "sf2.log.warn", args)));
                log.Set("error", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Error, "sf2.log.error", args)));
                root.Set("log", DynValue.NewTable(log));
                // Compatibility aliases for early mods; new scripts should use sf2.log.
                mod.Set("log", log.Get("info"));
                mod.Set("warn", log.Get("warn"));
                mod.Set("error", log.Get("error"));
                root.Set("mod", DynValue.NewTable(mod));

                var assets = new Table(_script);
                assets.Set("qualify", DynValue.NewCallback(AssetQualify));
                assets.Set("exists", DynValue.NewCallback(AssetExists));
                assets.Set("sprite", DynValue.NewCallback(AssetSprite));
                assets.Set("model", DynValue.NewCallback(AssetModel));
                root.Set("assets", DynValue.NewTable(assets));

                var localization = new Table(_script);
                localization.Set("key", DynValue.NewCallback(LocalizationKey));
                localization.Set("patch", DynValue.NewCallback(LocalizationPatch));
                root.Set("localization", DynValue.NewTable(localization));

                var state = new Table(_script);
                state.Set("NUMBER", DynValue.NewString("number"));
                state.Set("INTEGER", DynValue.NewString("integer"));
                state.Set("BOOLEAN", DynValue.NewString("boolean"));
                state.Set("STRING", DynValue.NewString("string"));
                state.Set("register", DynValue.NewCallback(StateRegister));
                state.Set("get", DynValue.NewCallback(StateGet));
                state.Set("set", DynValue.NewCallback(StateSet));
                state.Set("unset", DynValue.NewCallback(StateUnset));
                root.Set("state", DynValue.NewTable(state));

                var items = new Table(_script);
                items.Set("register_weapon", DynValue.NewCallback(RegisterWeapon));
                items.Set("register_armor", DynValue.NewCallback(RegisterArmor));
                items.Set("register_helm", DynValue.NewCallback(RegisterHelm));
                items.Set("register_ranged", DynValue.NewCallback(RegisterRanged));
                items.Set("register_magic", DynValue.NewCallback(RegisterMagic));
                items.Set("register_consumable", DynValue.NewCallback((ctx, args) =>
                    RegisterNonEquipmentItem(ModNonEquipmentItemKind.Consumable, "sf2.items.register_consumable", args)));
                items.Set("register_free", DynValue.NewCallback((ctx, args) =>
                    RegisterNonEquipmentItem(ModNonEquipmentItemKind.Free, "sf2.items.register_free", args)));
                items.Set("register_seal", DynValue.NewCallback((ctx, args) =>
                    RegisterNonEquipmentItem(ModNonEquipmentItemKind.Seal, "sf2.items.register_seal", args)));
                items.Set("get", DynValue.NewCallback(GetItem));
                items.Set("alias", DynValue.NewCallback(RegisterItemAlias));
                items.Set("tombstone", DynValue.NewCallback(RegisterItemTombstone));
                root.Set("items", DynValue.NewTable(items));

                var perks = new Table(_script);
                perks.Set("SINGLE", DynValue.NewString("single"));
                perks.Set("COMBO", DynValue.NewString("combo"));
                perks.Set("get", DynValue.NewCallback(GetPerk));
                perks.Set("register", DynValue.NewCallback(RegisterPerk));
                root.Set("perks", DynValue.NewTable(perks));

                var itemSets = new Table(_script);
                itemSets.Set("register", DynValue.NewCallback(RegisterItemSet));
                root.Set("itemsets", DynValue.NewTable(itemSets));

                var progression = new Table(_script);
                progression.Set("UNLOCK", DynValue.NewString("unlock"));
                progression.Set("UPGRADE", DynValue.NewString("upgrade"));
                progression.Set("replace_perk_branch", DynValue.NewCallback(ReplaceProgressionBranch));
                root.Set("progression", DynValue.NewTable(progression));

                var forge = new Table(_script);
                forge.Set("WEAPON", DynValue.NewString("weapon"));
                forge.Set("ARMOR", DynValue.NewString("armor"));
                forge.Set("HELM", DynValue.NewString("helm"));
                forge.Set("RANGED", DynValue.NewString("ranged"));
                forge.Set("MAGIC", DynValue.NewString("magic"));
                forge.Set("profile", DynValue.NewCallback(GetForgeEconomicProfile));
                forge.Set("register_recipe", DynValue.NewCallback(RegisterForgeRecipeFamily));
                root.Set("forge", DynValue.NewTable(forge));

                var behaviors = new Table(_script);
                behaviors.Set("NUMBER", DynValue.NewString("number"));
                behaviors.Set("INTEGER", DynValue.NewString("integer"));
                behaviors.Set("BOOLEAN", DynValue.NewString("boolean"));
                behaviors.Set("STRING", DynValue.NewString("string"));
                behaviors.Set("register", DynValue.NewCallback(RegisterBehavior));
                root.Set("behaviors", DynValue.NewTable(behaviors));

                var enchantments = new Table(_script);
                enchantments.Set("SIMPLE", DynValue.NewString("simple"));
                enchantments.Set("MEDIUM", DynValue.NewString("medium"));
                enchantments.Set("COMPLEX", DynValue.NewString("complex"));
                enchantments.Set("WEAPON", DynValue.NewString("weapon"));
                enchantments.Set("ARMOR", DynValue.NewString("armor"));
                enchantments.Set("HELM", DynValue.NewString("helm"));
                enchantments.Set("RANGED", DynValue.NewString("ranged"));
                enchantments.Set("MAGIC", DynValue.NewString("magic"));
                enchantments.Set("register", DynValue.NewCallback(RegisterEnchantment));
                root.Set("enchantments", DynValue.NewTable(enchantments));

                var zones = new Table(_script);
                zones.Set("get", DynValue.NewCallback(GetZone));
                zones.Set("register", DynValue.NewCallback(RegisterZone));
                root.Set("zones", DynValue.NewTable(zones));

                var battles = new Table(_script);
                battles.Set("DUMMY", DynValue.NewString("dummy"));
                battles.Set("TUTORIAL", DynValue.NewString("tutorial"));
                battles.Set("CHALLENGE", DynValue.NewString("challenge"));
                battles.Set("BOSSES", DynValue.NewString("bosses"));
                battles.Set("TOURNAMENT", DynValue.NewString("tournament"));
                battles.Set("STORY", DynValue.NewString("story"));
                battles.Set("SURVIVAL", DynValue.NewString("survival"));
                battles.Set("FRIENDLY", DynValue.NewString("friendly"));
                battles.Set("AUTO", DynValue.NewString("auto"));
                battles.Set("AI", DynValue.NewString("ai"));
                battles.Set("HIDDEN", DynValue.NewString("hidden"));
                battles.Set("FAKE", DynValue.NewString("fake"));
                battles.Set("PVP", DynValue.NewString("pvp"));
                battles.Set("FINAL", DynValue.NewString("final"));
                battles.Set("FINAL_TITAN", DynValue.NewString("final_titan"));
                battles.Set("register", DynValue.NewCallback(RegisterBattle));
                root.Set("battles", DynValue.NewTable(battles));

                var warriors = new Table(_script);
                warriors.Set("get_template", DynValue.NewCallback(GetWarriorTemplate));
                warriors.Set("register", DynValue.NewCallback(RegisterWarrior));
                root.Set("warriors", DynValue.NewTable(warriors));

                var rules = new Table(_script);
                rules.Set("PLAYER", DynValue.NewString("player"));
                rules.Set("OPPONENT", DynValue.NewString("opponent"));
                rules.Set("ALL", DynValue.NewString("all"));
                rules.Set("NORMAL", DynValue.NewString("normal"));
                rules.Set("ECLIPSE", DynValue.NewString("eclipse"));
                rules.Set("BOTH", DynValue.NewString("all"));
                rules.Set("no_perks", DynValue.NewCallback(RegisterNoPerksRule));
                rules.Set("require_item", DynValue.NewCallback(RegisterRequireItemRule));
                rules.Set("equip_item", DynValue.NewCallback(RegisterEquipItemRule));
                rules.Set("avatar", DynValue.NewCallback(RegisterAvatarRule));
                rules.Set("name", DynValue.NewCallback(RegisterNameRule));
                rules.Set("perk", DynValue.NewCallback(RegisterPerkRule));
                rules.Set("recharge_magic_each_round", DynValue.NewCallback(RegisterRechargeMagicRule));
                rules.Set("attributes", DynValue.NewCallback(RegisterAttributesRule));
                rules.Set("no_button", DynValue.NewCallback(RegisterNoButtonRule));
                root.Set("rules", DynValue.NewTable(rules));

                var rewards = new Table(_script);
                rewards.Set("register", DynValue.NewCallback(RegisterReward));
                root.Set("rewards", DynValue.NewTable(rewards));

                var fights = new Table(_script);
                fights.Set("register", DynValue.NewCallback(RegisterFight));
                fights.Set("patch", DynValue.NewCallback(PatchFight));
                root.Set("fights", DynValue.NewTable(fights));

                var quests = new Table(_script);
                quests.Set("register", DynValue.NewCallback(RegisterQuest));
                root.Set("quests", DynValue.NewTable(quests));

                var price = new Table(_script);
                price.Set("coins", DynValue.NewCallback((ctx, args) => Price(ModPriceCurrency.Coins,
                    "sf2.price.coins", args)));
                price.Set("gems", DynValue.NewCallback((ctx, args) => Price(ModPriceCurrency.Gems,
                    "sf2.price.gems", args)));
                root.Set("price", DynValue.NewTable(price));

                var shop = new Table(_script);
                shop.Set("WEAPONS", DynValue.NewString("weapons"));
                shop.Set("ARMOR", DynValue.NewString("armor"));
                shop.Set("HELMETS", DynValue.NewString("helmets"));
                shop.Set("RANGED", DynValue.NewString("ranged"));
                shop.Set("MAGIC", DynValue.NewString("magic"));
                shop.Set("INHERIT", DynValue.NewString("inherit"));
                shop.Set("FORCE_VISIBLE", DynValue.NewString("force_visible"));
                shop.Set("FORCE_HIDDEN", DynValue.NewString("force_hidden"));
                shop.Set("set_availability", DynValue.NewCallback(SetItemAvailability));
                shop.Set("addItem", DynValue.NewCallback(ShopAddItem));
                shop.Set("add", shop.Get("addItem"));
                root.Set("shop", DynValue.NewTable(shop));

                AddP1DModules(root);

                DynValue value = DynValue.NewTable(root);
                _modules.Add(moduleName, value);
                return value;
            }

            private DynValue LogCallback(ModLogLevel level, string function, CallbackArguments args)
            {
                string message = args.AsType(0, function, DataType.String, false).String;
                _api.Log(level, message);
                return DynValue.Nil;
            }

            private DynValue AssetQualify(ScriptExecutionContext context, CallbackArguments args)
            {
                string reference = args.AsType(0, "sf2.assets.qualify", DataType.String, false).String;
                try { return DynValue.NewString(_api.QualifyAsset(reference).ToString()); }
                catch (Exception exception) when (exception is FormatException || exception is InvalidOperationException)
                {
                    throw new ScriptRuntimeException(exception.Message);
                }
            }

            private DynValue AssetExists(ScriptExecutionContext context, CallbackArguments args)
            {
                string reference = args.AsType(0, "sf2.assets.exists", DataType.String, false).String;
                try { return DynValue.NewBoolean(_api.AssetExists(reference)); }
                catch (Exception exception) when (exception is FormatException || exception is InvalidOperationException)
                {
                    throw new ScriptRuntimeException(exception.Message);
                }
            }

            private DynValue AssetSprite(ScriptExecutionContext context, CallbackArguments args)
            {
                string reference = args.AsType(0, "sf2.assets.sprite", DataType.String, false).String;
                return ApiCall("sf2.assets.sprite", () =>
                    NewHandle(_spriteHandles, _api.RequireAsset(reference, AssetKind.Sprite)));
            }

            private DynValue AssetModel(ScriptExecutionContext context, CallbackArguments args)
            {
                string reference = args.AsType(0, "sf2.assets.model", DataType.String, false).String;
                return ApiCall("sf2.assets.model", () =>
                    NewHandle(_modelHandles, _api.RequireAsset(reference, AssetKind.Model)));
            }

            private DynValue LocalizationKey(ScriptExecutionContext context, CallbackArguments args)
            {
                string key = args.AsType(0, "sf2.localization.key", DataType.String, false).String;
                return ApiCall("sf2.localization.key", () =>
                    NewHandle(_localizationHandles, _api.GetLocalization(key)));
            }

            private DynValue LocalizationPatch(ScriptExecutionContext context, CallbackArguments args)
            {
                Table table = args.AsType(0, "sf2.localization.patch", DataType.Table, false).Table;
                return ApiCall("sf2.localization.patch", () =>
                {
                    ValidateFields(table, "sf2.localization.patch", "target", "language", "value");
                    string target = RequiredString(table, "target", "sf2.localization.patch");
                    string language = RequiredString(table, "language", "sf2.localization.patch");
                    string value = RequiredString(table, "value", "sf2.localization.patch");
                    _api.PatchLocalization(target, language, value);
                    return DynValue.Nil;
                });
            }

            private DynValue StateRegister(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.state.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "version", "fields", "aliases", "tombstones", "migrations");
                    int version = RequiredInt(table, "version", function);
                    ModParameterSchema fields = OptionalParameterSchema(table, "fields", function);
                    Dictionary<string, string> aliases = OptionalStringMap(table, "aliases", function);
                    string[] tombstones = OptionalStringArray(table, "tombstones", function);
                    Dictionary<int, DynValue> migrations = OptionalMigrationMap(table, "migrations", version, function);
                    ModStateDefinition definition = _api.RegisterState(version, fields, aliases, tombstones);
                    _stateDefinition = definition;
                    _stateMigrations.Clear();
                    foreach (KeyValuePair<int, DynValue> pair in migrations) _stateMigrations.Add(pair.Key, pair.Value);
                    return DynValue.Nil;
                });
            }

            private DynValue StateGet(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.state.get";
                string name = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () =>
                {
                    ModParameterValue value;
                    return _api.TryGetState(name, out value) ? ToDynValue(value) : DynValue.Nil;
                });
            }

            private DynValue StateSet(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.state.set";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    if (_stateDefinition == null)
                        throw new ModContentException("Mod must call sf2.state.register before writing state.");
                    var values = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
                    foreach (TablePair pair in table.Pairs)
                    {
                        if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String))
                            throw new ModContentException(function + " contains a non-string field name.");
                        ModParameterDefinition field;
                        if (!_stateDefinition.Fields.TryGet(pair.Key.String, out field))
                            throw new ModContentException(function + " contains unknown state field '" + pair.Key.String + "'.");
                        values.Add(pair.Key.String, ParameterValue(field.Type, pair.Value,
                            function + " field '" + pair.Key.String + "'"));
                    }
                    _api.SetState(values);
                    return DynValue.Nil;
                });
            }

            private DynValue StateUnset(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.state.unset";
                string name = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () =>
                {
                    _api.UnsetState(name);
                    return DynValue.Nil;
                });
            }

            private DynValue RegisterWeapon(ScriptExecutionContext context, CallbackArguments args)
            {
                Table table = args.AsType(0, "sf2.items.register_weapon", DataType.Table, false).Table;
                return ApiCall("sf2.items.register_weapon", () =>
                {
                    ValidateFields(table, "sf2.items.register_weapon", "id", "display_name", "icon", "model",
                        "subtype");
                    string id = RequiredString(table, "id", "sf2.items.register_weapon");
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", "sf2.items.register_weapon");
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite",
                        "sf2.items.register_weapon");
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model",
                        "sf2.items.register_weapon");
                    string subType = OptionalString(table, "subtype", "Katana", "sf2.items.register_weapon");
                    WeaponDefinition definition = _api.RegisterWeapon(id, displayName, icon, model, subType);
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterArmor(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.register_armor";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "display_name", "icon", "model");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model", function);
                    ArmorDefinition definition = _api.RegisterArmor(id, displayName, icon, model);
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterHelm(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.register_helm";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "display_name", "icon", "model");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model", function);
                    HelmDefinition definition = _api.RegisterHelm(id, displayName, icon, model);
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterRanged(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.register_ranged";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "display_name", "icon", "model", "subtype");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model", function);
                    string subType = RequiredString(table, "subtype", function);
                    RangedDefinition definition = _api.RegisterRanged(id, displayName, icon, model, subType);
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterMagic(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.register_magic";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "display_name", "icon", "model", "subtype");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model", function);
                    string subType = RequiredString(table, "subtype", function);
                    MagicDefinition definition = _api.RegisterMagic(id, displayName, icon, model, subType);
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterNonEquipmentItem(ModNonEquipmentItemKind kind, string function,
                CallbackArguments args)
            {
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "display_name", "icon", "model", "subtype",
                        "pack_label", "silent_receive", "spend_after_use");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    AssetId icon = OptionalHandle(table, "icon", _spriteHandles, "sprite", function,
                        default(AssetId));
                    AssetId model = OptionalHandle(table, "model", _modelHandles, "model", function,
                        default(AssetId));
                    NonEquipmentItemDefinition definition = _api.RegisterNonEquipmentItem(id, kind, displayName,
                        icon, model, OptionalStringAllowEmpty(table, "subtype", string.Empty, function),
                        OptionalStringAllowEmpty(table, "pack_label", string.Empty, function),
                        OptionalBool(table, "silent_receive", false, function),
                        OptionalBool(table, "spend_after_use", false, function));
                    return NewHandle(_itemHandles, definition.Id);
                });
            }

            private DynValue RegisterItemSet(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.itemsets.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "title", "text", "brief", "members");
                    DefinitionId title = RequiredHandle(table, "title", _localizationHandles, "localization", function);
                    DefinitionId text = RequiredHandle(table, "text", _localizationHandles, "localization", function);
                    DefinitionId brief = RequiredHandle(table, "brief", _localizationHandles, "localization", function);
                    ModItemSetMember[] members = ReadItemSetMembers(table.Get("members"), function + ".members");
                    ItemSetDefinition definition = _api.RegisterItemSet(RequiredString(table, "id", function),
                        title, text, brief, members);
                    return NewHandle(_itemSetHandles, definition.Id);
                });
            }

            private DynValue ReplaceProgressionBranch(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.progression.replace_perk_branch";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "level", "entries");
                    ModProgressionPerkEntry[] entries = ReadProgressionEntries(table.Get("entries"),
                        function + ".entries");
                    _api.ReplaceProgressionBranch(RequiredInt(table, "level", function), entries);
                    return DynValue.Nil;
                });
            }

            private DynValue GetForgeEconomicProfile(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.forge.profile";
                string reference = args.AsType(0, function, DataType.String, false).String;
                if (reference.IndexOf(':') < 0) reference = "core:forge-profiles/" + reference;
                string resolved = reference;
                return ApiCall(function, () => NewHandle(_forgeProfileHandles,
                    _api.GetForgeEconomicProfile(resolved).Id));
            }

            private DynValue RegisterForgeRecipeFamily(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.forge.register_recipe";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "alias", "economic_profile", "items", "candidates");
                    DefinitionId profile = RequiredHandle(table, "economic_profile", _forgeProfileHandles,
                        "forge economic profile", function);
                    ModForgeRecipeItem[] items = ReadForgeRecipeItems(table.Get("items"), function + ".items");
                    ModForgeRecipeCandidate[] candidates = ReadForgeRecipeCandidates(table.Get("candidates"),
                        function + ".candidates");
                    ForgeRecipeFamilyDefinition definition = _api.RegisterForgeRecipeFamily(
                        RequiredString(table, "id", function),
                        OptionalStringAllowEmpty(table, "alias", string.Empty, function), profile, items, candidates);
                    return NewHandle(_forgeRecipeHandles, definition.Id);
                });
            }

            private DynValue SetItemAvailability(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.shop.set_availability";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "item", "visibility", "required_group");
                    DefinitionId item = RequiredHandle(table, "item", _itemHandles, "item", function);
                    string visibilityValue = OptionalString(table, "visibility", "inherit", function);
                    ModItemVisibility visibility;
                    switch (visibilityValue)
                    {
                        case "inherit": visibility = ModItemVisibility.Inherit; break;
                        case "force_visible": visibility = ModItemVisibility.ForceVisible; break;
                        case "force_hidden": visibility = ModItemVisibility.ForceHidden; break;
                        default: throw new ModContentException(function + " field 'visibility' is not supported.");
                    }
                    _api.SetItemAvailability(item, visibility,
                        OptionalStringAllowEmpty(table, "required_group", string.Empty, function));
                    return DynValue.Nil;
                });
            }

            private DynValue RegisterItemAlias(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.alias";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "from", "to");
                    string from = RequiredString(table, "from", function);
                    DefinitionId target = RequiredHandle(table, "to", _itemHandles, "item", function);
                    _api.RegisterItemAlias(from, target);
                    return DynValue.Nil;
                });
            }

            private DynValue GetItem(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.get";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_itemHandles, _api.GetItem(reference).Id));
            }

            private DynValue RegisterItemTombstone(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.items.tombstone";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id");
                    _api.RegisterItemTombstone(RequiredString(table, "id", function));
                    return DynValue.Nil;
                });
            }

            private DynValue GetPerk(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.perks.get";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_perkHandles, _api.GetPerk(reference).Id));
            }

            private DynValue RegisterBehavior(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.behaviors.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "parameters", "on_fight_begin", "on_damage_received");
                    string id = RequiredString(table, "id", function);
                    ModParameterSchema parameters = OptionalParameterSchema(table, "parameters", function);
                    DynValue handler = table.Get("on_fight_begin");
                    DynValue damageHandler = table.Get("on_damage_received");
                    if (!handler.IsNil() && handler.Type != DataType.Function)
                        throw new ModContentException(function + " field 'on_fight_begin' must be a Lua function.");
                    if (!damageHandler.IsNil() && damageHandler.Type != DataType.Function)
                        throw new ModContentException(function + " field 'on_damage_received' must be a Lua function.");
                    if (handler.IsNil() && damageHandler.IsNil())
                        throw new ModContentException(function + " requires at least one behavior handler.");
                    ModBehaviorDefinition definition = _api.RegisterBehavior(id, parameters);
                    if (!handler.IsNil()) _behaviorHandlers.Add((definition.Id, ModEffectEvent.FightBegin), handler);
                    if (!damageHandler.IsNil()) _behaviorHandlers.Add((definition.Id, ModEffectEvent.DamageReceived), damageHandler);
                    return NewHandle(_behaviorHandles, definition);
                });
            }

            private DynValue RegisterPerk(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.perks.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "template", "behavior", "display_name", "description",
                        "icon", "parameters", "kind");
                    string id = RequiredString(table, "id", function);
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", function);
                    DefinitionId description = RequiredHandle(table, "description", _localizationHandles,
                        "localization", function);
                    AssetId icon = default(AssetId);
                    DynValue iconValue = table.Get("icon");
                    if (iconValue.Type != DataType.Nil && iconValue.Type != DataType.Void)
                        icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                    bool hasTemplate = !table.Get("template").IsNil();
                    bool hasBehavior = !table.Get("behavior").IsNil();
                    if (hasTemplate == hasBehavior)
                        throw new ModContentException(function + " requires exactly one of 'template' or 'behavior'.");

                    PerkDefinition definition;
                    if (hasTemplate)
                    {
                        if (!table.Get("kind").IsNil())
                            throw new ModContentException(function + " legacy template form must not set 'kind'.");
                        DefinitionId template = RequiredHandle(table, "template", _perkHandles, "perk", function);
                        Dictionary<string, string> parameters = OptionalScalarMap(table, "parameters", function);
                        definition = _api.RegisterPerk(id, template, displayName, description, icon, parameters);
                    }
                    else
                    {
                        ModBehaviorDefinition behavior = RequiredHandle(table, "behavior", _behaviorHandles,
                            "behavior", function);
                        string kindText = RequiredString(table, "kind", function);
                        ModPerkKind kind;
                        switch (kindText)
                        {
                            case "single": kind = ModPerkKind.Single; break;
                            case "combo": kind = ModPerkKind.Combo; break;
                            default: throw new ModContentException(function + " field 'kind' is not supported.");
                        }
                        Dictionary<string, ModParameterValue> parameters = OptionalTypedParameterMap(table,
                            "parameters", behavior.Parameters, function);
                        definition = _api.RegisterScriptedPerk(id, displayName, description, icon, kind,
                            behavior.Id, parameters);
                    }
                    return NewHandle(_perkHandles, definition.Id);
                });
            }

            private DynValue RegisterEnchantment(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.enchantments.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "perk", "behavior", "display_name", "description",
                        "icon", "recipe", "item_types", "parameters");
                    string id = RequiredString(table, "id", function);
                    string recipeText = RequiredString(table, "recipe", function);
                    ModEnchantmentRecipe recipe;
                    switch (recipeText)
                    {
                        case "simple": recipe = ModEnchantmentRecipe.Simple; break;
                        case "medium": recipe = ModEnchantmentRecipe.Medium; break;
                        case "complex": recipe = ModEnchantmentRecipe.Complex; break;
                        default: throw new ModContentException(function + " field 'recipe' is not supported.");
                    }
                    ModEquipmentKind[] itemTypes = RequiredEquipmentKinds(table, "item_types", function);
                    bool hasPerk = !table.Get("perk").IsNil();
                    bool hasBehavior = !table.Get("behavior").IsNil();
                    if (hasPerk == hasBehavior)
                        throw new ModContentException(function + " requires exactly one of 'perk' or 'behavior'.");

                    EnchantmentDefinition definition;
                    if (hasPerk)
                    {
                        if (!table.Get("display_name").IsNil() || !table.Get("description").IsNil() ||
                            !table.Get("icon").IsNil() || !table.Get("parameters").IsNil())
                            throw new ModContentException(function +
                                " legacy perk form must not set direct behavior presentation/parameters.");
                        DefinitionId perk = RequiredHandle(table, "perk", _perkHandles, "perk", function);
                        definition = _api.RegisterEnchantment(id, perk, recipe, itemTypes);
                    }
                    else
                    {
                        ModBehaviorDefinition behavior = RequiredHandle(table, "behavior", _behaviorHandles,
                            "behavior", function);
                        DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                            "localization", function);
                        DefinitionId description = RequiredHandle(table, "description", _localizationHandles,
                            "localization", function);
                        AssetId icon = default;
                        DynValue iconValue = table.Get("icon");
                        if (iconValue.Type != DataType.Nil && iconValue.Type != DataType.Void)
                            icon = RequiredHandle(table, "icon", _spriteHandles, "sprite", function);
                        Dictionary<string, ModParameterValue> parameters = OptionalTypedParameterMap(table,
                            "parameters", behavior.Parameters, function);
                        definition = _api.RegisterScriptedEnchantment(id, displayName, description, icon, recipe,
                            itemTypes, behavior.Id, parameters);
                    }
                    return NewHandle(_enchantmentHandles, definition.Id);
                });
            }

            private DynValue RegisterZone(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.zones.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "file", "start");
                    string id = RequiredString(table, "id", function);
                    string file = OptionalStringAllowEmpty(table, "file", string.Empty, function);
                    bool isStart = OptionalBool(table, "start", false, function);
                    return NewHandle(_zoneHandles, _api.RegisterZone(id, file, isStart).Id);
                });
            }

            private DynValue GetZone(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.zones.get";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_zoneHandles, _api.GetZone(reference).Id));
            }

            private DynValue RegisterBattle(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.battles.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "zone", "type", "x", "y", "alias", "title", "icon",
                        "icon_atlas", "eclipse_toggle_name", "preview", "description", "location", "music",
                        "reward_image", "show_resistance");
                    string id = RequiredString(table, "id", function);
                    DefinitionId zone = RequiredHandle(table, "zone", _zoneHandles, "zone", function);
                    ModBattleKind kind = ParseBattleKind(RequiredString(table, "type", function), function);
                    BattleDefinition definition = _api.RegisterBattle(id, zone, kind,
                        OptionalInt(table, "x", 0, function), OptionalInt(table, "y", 0, function),
                        OptionalStringAllowEmpty(table, "alias", string.Empty, function),
                        OptionalStringAllowEmpty(table, "title", string.Empty, function),
                        OptionalStringAllowEmpty(table, "icon", string.Empty, function),
                        OptionalStringAllowEmpty(table, "preview", string.Empty, function),
                        OptionalStringAllowEmpty(table, "description", string.Empty, function),
                        OptionalStringAllowEmpty(table, "location", string.Empty, function),
                        OptionalStringAllowEmpty(table, "music", string.Empty, function),
                        OptionalStringAllowEmpty(table, "reward_image", string.Empty, function),
                        OptionalBool(table, "show_resistance", false, function),
                        OptionalStringAllowEmpty(table, "icon_atlas", string.Empty, function),
                        OptionalStringAllowEmpty(table, "eclipse_toggle_name", string.Empty, function));
                    return NewHandle(_battleHandles, definition.Id);
                });
            }

            private DynValue RegisterWarrior(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.warriors.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "template", "first_name", "last_name", "avatar", "voice", "level",
                        "tactic", "group", "random", "attributes", "attribute_alignments", "items", "perks");
                    string id = RequiredString(table, "id", function);
                    DefinitionId[] items = OptionalHandleArray(table, "items", _itemHandles, "item", function);
                    DefinitionId[] perks = OptionalHandleArray(table, "perks", _perkHandles, "perk", function);
                    DynValue templateValue = table.Get("template");
                    DefinitionId template = default(DefinitionId);
                    bool hasTemplate = !templateValue.IsNil();
                    if (hasTemplate)
                    {
                        if (templateValue.Type != DataType.Table || !_warriorTemplateHandles.TryGetValue(templateValue.Table, out template))
                            throw new ModContentException(function + " field 'template' must be a warrior template handle.");
                    }
                    var attributes = new Dictionary<string, float>(StringComparer.Ordinal);
                    DynValue attributesValue = table.Get("attributes");
                    if (!attributesValue.IsNil())
                    {
                        if (attributesValue.Type != DataType.Table) throw new ModContentException(function + " field 'attributes' must be a table.");
                        foreach (TablePair pair in attributesValue.Table.Pairs)
                        {
                            if (pair.Key.Type != DataType.String || string.IsNullOrWhiteSpace(pair.Key.String) ||
                                pair.Value.Type != DataType.Number || double.IsNaN(pair.Value.Number) || double.IsInfinity(pair.Value.Number) ||
                                pair.Value.Number < -float.MaxValue || pair.Value.Number > float.MaxValue)
                                throw new ModContentException(function + " attributes must map non-empty names to finite numbers.");
                            attributes.Add(pair.Key.String, (float)pair.Value.Number);
                        }
                    }
                    WarriorAttributeAlignmentDefinition[] alignments = ReadWarriorAlignments(table.Get("attribute_alignments"),
                        function + ".attribute_alignments");
                    DynValue tacticValue = table.Get("tactic");
                    string tactic = string.Empty;
                    if (!tacticValue.IsNil())
                    {
                        if (tacticValue.Type == DataType.String)
                            tactic = tacticValue.String;
                        else if (tacticValue.Type == DataType.Table && _tacticHandles.TryGetValue(tacticValue.Table, out DefinitionId tacticId))
                            tactic = tacticId.ToString();
                        else
                            throw new ModContentException(function + " field 'tactic' must be a tactic handle or string.");
                    }
                    WarriorDefinition definition = _api.RegisterWarrior(id,
                        OptionalStringAllowEmpty(table, "first_name", string.Empty, function),
                        OptionalStringAllowEmpty(table, "last_name", string.Empty, function),
                        OptionalStringAllowEmpty(table, "avatar", string.Empty, function),
                        OptionalStringAllowEmpty(table, "voice", string.Empty, function),
                        OptionalInt(table, "level", 0, function), tactic, items, perks,
                        template, hasTemplate, OptionalStringAllowEmpty(table, "group", string.Empty, function),
                        OptionalInt(table, "random", 0, function), attributes, alignments);
                    return NewHandle(_warriorHandles, definition.Id);
                });
            }

            private DynValue GetWarriorTemplate(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.warriors.get_template";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_warriorTemplateHandles,
                    _api.GetWarriorTemplate(reference).Id));
            }

            private DynValue RegisterNoPerksRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.no_perks";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "target", "mode", "rounds", "name");
                    FightRuleDefinition definition = _api.RegisterNoPerksRule(
                        RequiredString(table, "id", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function),
                        OptionalStringAllowEmpty(table, "name", string.Empty, function));
                    return NewHandle(_ruleHandles, definition.Id);
                });
            }

            private DynValue RegisterRequireItemRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.require_item";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "item", "minimum_level", "mode", "rounds");
                    FightRuleDefinition definition = _api.RegisterRequireItemRule(
                        RequiredString(table, "id", function),
                        RequiredHandle(table, "item", _itemHandles, "item", function),
                        OptionalInt(table, "minimum_level", 0, function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function));
                    return NewHandle(_ruleHandles, definition.Id);
                });
            }

            private DynValue RegisterEquipItemRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.equip_item";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "item", "minimum_level", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterEquipItemRule(
                        RequiredString(table, "id", function),
                        RequiredHandle(table, "item", _itemHandles, "item", function),
                        OptionalInt(table, "minimum_level", 0, function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterAvatarRule(ScriptExecutionContext context, CallbackArguments args) =>
                RegisterNamedRule(args, "sf2.rules.avatar", ModFightRuleKind.Avatar);

            private DynValue RegisterNameRule(ScriptExecutionContext context, CallbackArguments args) =>
                RegisterNamedRule(args, "sf2.rules.name", ModFightRuleKind.Name);

            private DynValue RegisterNoButtonRule(ScriptExecutionContext context, CallbackArguments args) =>
                RegisterNamedRule(args, "sf2.rules.no_button", ModFightRuleKind.NoButton);

            private DynValue RegisterNamedRule(CallbackArguments args, string function, ModFightRuleKind kind)
            {
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "name", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterNamedRule(
                        RequiredString(table, "id", function), kind, RequiredString(table, "name", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterPerkRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.perk";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "perk", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterPerkRule(
                        RequiredString(table, "id", function),
                        RequiredHandle(table, "perk", _perkHandles, "perk", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterRechargeMagicRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.recharge_magic_each_round";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "target", "mode", "rounds");
                    return NewHandle(_ruleHandles, _api.RegisterRechargeMagicRule(
                        RequiredString(table, "id", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function)).Id);
                });
            }

            private DynValue RegisterAttributesRule(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rules.attributes";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "target", "mode", "rounds", "values");
                    DynValue values = table.Get("values");
                    if (values.Type != DataType.Table) throw new ModContentException(function + " field 'values' must be a table.");
                    var attributes = new Dictionary<string, float>(StringComparer.Ordinal);
                    foreach (TablePair pair in values.Table.Pairs)
                    {
                        if (pair.Key.Type != DataType.String || string.IsNullOrWhiteSpace(pair.Key.String) ||
                            pair.Value.Type != DataType.Number || double.IsNaN(pair.Value.Number) || double.IsInfinity(pair.Value.Number) ||
                            pair.Value.Number < -float.MaxValue || pair.Value.Number > float.MaxValue)
                            throw new ModContentException(function + " attributes must map non-empty names to finite numbers.");
                        attributes.Add(pair.Key.String, (float)pair.Value.Number);
                    }
                    return NewHandle(_ruleHandles, _api.RegisterAttributesRule(
                        RequiredString(table, "id", function),
                        ParseRuleTarget(OptionalString(table, "target", "all", function), function),
                        ParseRuleMode(OptionalString(table, "mode", "all", function), function),
                        OptionalIntArray(table, "rounds", function), attributes).Id);
                });
            }

            private DynValue RegisterReward(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.rewards.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "items", "choices");
                    RewardItemGrant[] items = ReadRewardItems(table.Get("items"), function + ".items", false);
                    RewardChoiceDefinition[] choices = ReadRewardChoices(table.Get("choices"), function + ".choices");
                    RewardDefinition definition = _api.RegisterReward(RequiredString(table, "id", function), items, choices);
                    return NewHandle(_rewardHandles, definition.Id);
                });
            }

            private DynValue RegisterFight(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.fights.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "battle", "replays", "replay_interval", "power", "rounds",
                        "round_time", "location", "music", "evaluated_rating", "health_recovery", "description",
                        "locked", "reward_image", "warriors", "rules", "rewards");
                    FightDefinition definition = _api.RegisterFight(
                        RequiredString(table, "id", function),
                        RequiredHandle(table, "battle", _battleHandles, "battle", function),
                        OptionalInt(table, "replays", 0, function),
                        OptionalInt(table, "replay_interval", 0, function),
                        OptionalInt(table, "power", 0, function),
                        OptionalInt(table, "rounds", 3, function),
                        OptionalInt(table, "round_time", 99, function),
                        OptionalStringAllowEmpty(table, "location", string.Empty, function),
                        OptionalStringAllowEmpty(table, "music", string.Empty, function),
                        OptionalFloat(table, "evaluated_rating", -1f, function),
                        OptionalFloat(table, "health_recovery", 1f, function),
                        OptionalStringAllowEmpty(table, "description", string.Empty, function),
                        OptionalBool(table, "locked", false, function),
                        OptionalStringAllowEmpty(table, "reward_image", string.Empty, function),
                        OptionalHandleArray(table, "warriors", _warriorHandles, "warrior", function),
                        OptionalHandleArray(table, "rules", _ruleHandles, "rule", function),
                        OptionalHandleArray(table, "rewards", _rewardHandles, "reward", function));
                    return NewHandle(_fightHandles, definition.Id);
                });
            }

            private DynValue PatchFight(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.fights.patch";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "target", "description", "rounds", "round_time");
                    string target = RequiredString(table, "target", function);
                    bool changed = false;
                    DynValue description = table.Get("description");
                    if (!description.IsNil())
                    {
                        if (description.Type != DataType.String) throw new ModContentException(function + " field 'description' must be a string.");
                        _api.PatchFightDescription(target, description.String);
                        changed = true;
                    }
                    DynValue rounds = table.Get("rounds");
                    if (!rounds.IsNil()) { _api.PatchFightRounds(target, RequiredInt(table, "rounds", function)); changed = true; }
                    DynValue roundTime = table.Get("round_time");
                    if (!roundTime.IsNil()) { _api.PatchFightRoundTime(target, RequiredInt(table, "round_time", function)); changed = true; }
                    if (!changed) throw new ModContentException(function + " must patch at least one supported field.");
                    return DynValue.Nil;
                });
            }

            private DynValue RegisterQuest(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.quests.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "priority", "unresumable", "allow_doubles", "place",
                        "groups", "marks", "events", "conditions", "actions");
                    QuestDefinition definition = _api.RegisterQuest(RequiredString(table, "id", function),
                        OptionalInt(table, "priority", 0, function), OptionalBool(table, "unresumable", false, function),
                        OptionalBool(table, "allow_doubles", false, function),
                        ParseQuestPlace(OptionalString(table, "place", "map", function), function),
                        OptionalQuestStringArray(table, "groups", function), OptionalQuestStringArray(table, "marks", function),
                        ReadQuestEvents(table.Get("events"), function + ".events"),
                        ReadQuestConditions(table.Get("conditions"), function + ".conditions"),
                        ReadQuestActions(table.Get("actions"), function + ".actions"));
                    return NewHandle(_questHandles, definition.Id);
                });
            }

            private DynValue Price(ModPriceCurrency currency, string function, CallbackArguments args)
            {
                return ApiCall(function, () =>
                {
                    int amount = RequiredInt(args, 0, function);
                    if (amount < 0) throw new ModContentException(function + " amount must not be negative.");
                    return NewHandle(_priceHandles, new ModPrice(currency, amount));
                });
            }

            private DynValue ShopAddItem(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.shop.addItem";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "section", "item", "level", "price");
                    string sectionText = RequiredString(table, "section", function);
                    ModShopSection section;
                    switch (sectionText)
                    {
                        case "weapons": section = ModShopSection.Weapons; break;
                        case "armor": section = ModShopSection.Armor; break;
                        case "helmets": section = ModShopSection.Helmets; break;
                        case "ranged": section = ModShopSection.Ranged; break;
                        case "magic": section = ModShopSection.Magic; break;
                        default: throw new ModContentException(function + " field 'section' is not a supported shop section.");
                    }
                    DefinitionId item = RequiredHandle(table, "item", _itemHandles, "item", function);
                    int level = RequiredInt(table, "level", function);
                    ModPrice price = RequiredHandle(table, "price", _priceHandles, "price", function);
                    ShopListingDefinition listing = _api.RegisterShopListing(item, section, level, price);
                    return DynValue.NewString(listing.Id.ToString());
                });
            }

            private DynValue NewHandle<T>(Dictionary<Table, T> handles, T value)
            {
                var table = new Table(_script);
                handles.Add(table, value);
                return DynValue.NewTable(table);
            }

            private static T RequiredHandle<T>(Table table, string field, Dictionary<Table, T> handles,
                string kind, string function)
            {
                DynValue value = table.Get(field);
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a " + kind + " handle.");
                T result;
                if (!handles.TryGetValue(value.Table, out result))
                    throw new ModContentException(function + " field '" + field + "' is not a " + kind +
                        " handle created by this mod context.");
                return result;
            }

            private static string RequiredString(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.Type != DataType.String || string.IsNullOrEmpty(value.String))
                    throw new ModContentException(function + " field '" + field + "' must be a non-empty string.");
                return value.String;
            }

            private static string OptionalString(Table table, string field, string fallback, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.String || string.IsNullOrEmpty(value.String))
                    throw new ModContentException(function + " field '" + field + "' must be a non-empty string.");
                return value.String;
            }

            private static string OptionalStringAllowEmpty(Table table, string field, string fallback, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.String)
                    throw new ModContentException(function + " field '" + field + "' must be a string.");
                return value.String ?? string.Empty;
            }

            private static bool OptionalBool(Table table, string field, bool fallback, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.Boolean)
                    throw new ModContentException(function + " field '" + field + "' must be boolean.");
                return value.Boolean;
            }

            private static int OptionalInt(Table table, string field, int fallback, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.Number)
                    throw new ModContentException(function + " field '" + field + "' must be an integer.");
                return ToInt(value.Number, function + " field '" + field + "'");
            }

            private static float OptionalFloat(Table table, string field, float fallback, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.Number || double.IsNaN(value.Number) || double.IsInfinity(value.Number) ||
                    value.Number < -float.MaxValue || value.Number > float.MaxValue)
                    throw new ModContentException(function + " field '" + field + "' must be a finite single-precision number.");
                return (float)value.Number;
            }

            private static T[] OptionalHandleArray<T>(Table table, string field, Dictionary<Table, T> handles,
                string kind, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return Array.Empty<T>();
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be an array table.");
                var result = new List<T>();
                for (int i = 1; ; i++)
                {
                    DynValue item = value.Table.Get(i);
                    if (item.IsNil()) break;
                    if (item.Type != DataType.Table)
                        throw new ModContentException(function + " field '" + field + "' entries must be " + kind + " handles.");
                    T handle;
                    if (!handles.TryGetValue(item.Table, out handle))
                        throw new ModContentException(function + " field '" + field + "' contains a " + kind +
                            " handle not created by this mod context.");
                    result.Add(handle);
                }
                int entries = 0;
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.Number) throw new ModContentException(function + " field '" + field +
                        "' must be a dense array table.");
                    entries++;
                }
                if (entries != result.Count)
                    throw new ModContentException(function + " field '" + field + "' must be a dense array table.");
                return result.ToArray();
            }

            private static int[] OptionalIntArray(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return Array.Empty<int>();
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be an array table.");
                var result = new List<int>();
                for (int i = 1; ; i++)
                {
                    DynValue item = value.Table.Get(i);
                    if (item.IsNil()) break;
                    if (item.Type != DataType.Number)
                        throw new ModContentException(function + " field '" + field + "' entries must be integers.");
                    result.Add(ToInt(item.Number, function + " field '" + field + "' entry " + i));
                }
                int entries = 0;
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.Number) throw new ModContentException(function + " field '" + field +
                        "' must be a dense array table.");
                    entries++;
                }
                if (entries != result.Count)
                    throw new ModContentException(function + " field '" + field + "' must be a dense array table.");
                return result.ToArray();
            }

            private static WarriorAttributeAlignmentDefinition[] ReadWarriorAlignments(DynValue value,
                string function)
            {
                if (value.IsNil()) return Array.Empty<WarriorAttributeAlignmentDefinition>();
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " must be an array table.");
                var result = new List<WarriorAttributeAlignmentDefinition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table)
                        throw new ModContentException(function + " entries must be tables.");
                    Table row = entry.Table;
                    string where = function + "[" + i + "]";
                    ValidateFields(row, where, "factor", "shift", "priority", "mode");
                    DynValue factor = row.Get("factor");
                    DynValue shift = row.Get("shift");
                    if (factor.Type != DataType.Number || shift.Type != DataType.Number)
                        throw new ModContentException(where + " requires numeric factor and shift.");
                    if (double.IsNaN(factor.Number) || double.IsInfinity(factor.Number) ||
                        double.IsNaN(shift.Number) || double.IsInfinity(shift.Number) ||
                        factor.Number < -float.MaxValue || factor.Number > float.MaxValue ||
                        shift.Number < -float.MaxValue || shift.Number > float.MaxValue)
                        throw new ModContentException(where + " factor and shift must be finite single-precision numbers.");
                    result.Add(new WarriorAttributeAlignmentDefinition((float)factor.Number, (float)shift.Number,
                        OptionalInt(row, "priority", 0, where),
                        ParseRuleMode(OptionalString(row, "mode", "all", where), where)));
                }
                return result.ToArray();
            }

            private RewardItemGrant[] ReadRewardItems(DynValue value, string function, bool weighted)
            {
                if (value.IsNil()) return Array.Empty<RewardItemGrant>();
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<RewardItemGrant>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table item = entry.Table;
                    ValidateFields(item, function + "[" + i + "]", weighted ? new[] { "item", "upgrade", "weight" } : new[] { "item", "upgrade" });
                    DefinitionId id = RequiredHandle(item, "item", _itemHandles, "item", function + "[" + i + "]");
                    int upgrade = OptionalInt(item, "upgrade", 0, function + "[" + i + "]");
                    if (upgrade < 0) throw new ModContentException(function + " upgrade must not be negative.");
                    result.Add(new RewardItemGrant(id, (uint)upgrade));
                }
                return result.ToArray();
            }

            private RewardChoiceDefinition[] ReadRewardChoices(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<RewardChoiceDefinition>();
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<RewardChoiceDefinition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table choice = entry.Table;
                    ValidateFields(choice, function + "[" + i + "]", "items");
                    DynValue itemsValue = choice.Get("items");
                    if (itemsValue.Type != DataType.Table)
                        throw new ModContentException(function + "[" + i + "].items must be an array table.");
                    var items = new List<RewardChoiceItem>();
                    for (int j = 1; ; j++)
                    {
                        DynValue itemValue = itemsValue.Table.Get(j);
                        if (itemValue.IsNil()) break;
                        if (itemValue.Type != DataType.Table)
                            throw new ModContentException(function + "[" + i + "].items entries must be tables.");
                        Table item = itemValue.Table;
                        string itemFunction = function + "[" + i + "].items[" + j + "]";
                        ValidateFields(item, itemFunction, "item", "upgrade", "weight");
                        DefinitionId id = RequiredHandle(item, "item", _itemHandles, "item", itemFunction);
                        int upgrade = OptionalInt(item, "upgrade", 0, itemFunction);
                        if (upgrade < 0) throw new ModContentException(itemFunction + " upgrade must not be negative.");
                        float weight = OptionalFloat(item, "weight", 1f, itemFunction);
                        items.Add(new RewardChoiceItem(new RewardItemGrant(id, (uint)upgrade), weight));
                    }
                    result.Add(new RewardChoiceDefinition(items.ToArray()));
                }
                return result.ToArray();
            }

            private static ModBattleKind ParseBattleKind(string value, string function)
            {
                switch (value)
                {
                    case "dummy": return ModBattleKind.Dummy;
                    case "tutorial": return ModBattleKind.Tutorial;
                    case "challenge": return ModBattleKind.Challenge;
                    case "bosses": return ModBattleKind.Bosses;
                    case "tournament": return ModBattleKind.Tournament;
                    case "story": return ModBattleKind.Story;
                    case "survival": return ModBattleKind.Survival;
                    case "friendly": return ModBattleKind.Friendly;
                    case "auto": return ModBattleKind.Auto;
                    case "ai": return ModBattleKind.Ai;
                    case "hidden": return ModBattleKind.Hidden;
                    case "fake": return ModBattleKind.Fake;
                    case "pvp": return ModBattleKind.Pvp;
                    case "final": return ModBattleKind.Final;
                    case "final_titan": return ModBattleKind.FinalTitan;
                    default: throw new ModContentException(function + " field 'type' is not supported.");
                }
            }

            private static ModRuleTarget ParseRuleTarget(string value, string function)
            {
                switch (value)
                {
                    case "player": return ModRuleTarget.Player;
                    case "opponent": return ModRuleTarget.Opponent;
                    case "all": return ModRuleTarget.All;
                    default: throw new ModContentException(function + " field 'target' is not supported.");
                }
            }

            private static ModRuleMode ParseRuleMode(string value, string function)
            {
                switch (value)
                {
                    case "all": return ModRuleMode.All;
                    case "normal": return ModRuleMode.Normal;
                    case "eclipse": return ModRuleMode.Eclipse;
                    default: throw new ModContentException(function + " field 'mode' is not supported.");
                }
            }

            private static string[] OptionalQuestStringArray(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return Array.Empty<string>();
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be an array table.");
                var result = new List<string>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.String || string.IsNullOrWhiteSpace(entry.String))
                        throw new ModContentException(function + " field '" + field + "' entries must be non-empty strings.");
                    result.Add(entry.String);
                }
                return result.ToArray();
            }

            private static ModQuestActionPlace ParseQuestPlace(string value, string function)
            {
                switch (value)
                {
                    case "map": return ModQuestActionPlace.Map;
                    case "fight": return ModQuestActionPlace.Fight;
                    case "dojo": return ModQuestActionPlace.Dojo;
                    default: throw new ModContentException(function + " field 'place' is not supported.");
                }
            }

            private static ModQuestEventKind[] ReadQuestEvents(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModQuestEventKind>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i); if (entry.IsNil()) break;
                    if (entry.Type != DataType.String) throw new ModContentException(function + " entries must be strings.");
                    switch (entry.String)
                    {
                        case "fight_enter": result.Add(ModQuestEventKind.FightEnter); break;
                        case "fight_end": result.Add(ModQuestEventKind.FightEnd); break;
                        case "level_up": result.Add(ModQuestEventKind.LevelUp); break;
                        case "got_item": result.Add(ModQuestEventKind.GotItem); break;
                        case "dialog": result.Add(ModQuestEventKind.Dialog); break;
                        case "session": result.Add(ModQuestEventKind.Session); break;
                        case "activate": result.Add(ModQuestEventKind.Activate); break;
                        case "purchase": result.Add(ModQuestEventKind.Purchase); break;
                        case "delivery": result.Add(ModQuestEventKind.Delivery); break;
                        case "timer_end": result.Add(ModQuestEventKind.TimerEnd); break;
                        case "map_button": result.Add(ModQuestEventKind.MapButtonPress); break;
                        case "enchantment": result.Add(ModQuestEventKind.Enchantment); break;
                        case "activate_perk": result.Add(ModQuestEventKind.ActivatePerk); break;
                        case "deactivate_perk": result.Add(ModQuestEventKind.DeactivatePerk); break;
                        case "set_item_acquired": result.Add(ModQuestEventKind.SetItemAcquired); break;
                        case "scene_loaded": result.Add(ModQuestEventKind.SceneLoaded); break;
                        case "shop_enter": result.Add(ModQuestEventKind.ShopEnter); break;
                        default: throw new ModContentException(function + " contains unsupported event '" + entry.String + "'.");
                    }
                }
                return result.ToArray();
            }

            private ModQuestCondition[] ReadQuestConditions(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModQuestCondition>();
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModQuestCondition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i); if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    result.Add(ReadQuestCondition(entry.Table, function + "[" + i + "]"));
                }
                return result.ToArray();
            }

            private ModQuestCondition ReadQuestCondition(Table table, string function)
            {
                string op = RequiredString(table, "op", function);
                bool not = OptionalBool(table, "not", false, function);
                if (op == "all" || op == "any")
                {
                    ValidateFields(table, function, "op", "not", "conditions");
                    ModQuestCondition[] children = ReadQuestConditions(table.Get("conditions"), function + ".conditions");
                    return new ModQuestCondition(op == "all" ? ModQuestConditionKind.All : ModQuestConditionKind.Any, children, not);
                }
                ValidateFields(table, function, "op", "not", "left", "right");
                ModQuestCompareOperator compare;
                switch (op)
                {
                    case "eq": compare = ModQuestCompareOperator.Equal; break;
                    case "gt": compare = ModQuestCompareOperator.Greater; break;
                    case "gte": compare = ModQuestCompareOperator.GreaterEqual; break;
                    case "lt": compare = ModQuestCompareOperator.Less; break;
                    case "lte": compare = ModQuestCompareOperator.LessEqual; break;
                    default: throw new ModContentException(function + " has unsupported condition op '" + op + "'.");
                }
                return new ModQuestCondition(compare, ReadQuestOperand(table.Get("left"), function + ".left"),
                    ReadQuestOperand(table.Get("right"), function + ".right"), not);
            }

            private ModQuestOperand ReadQuestOperand(DynValue value, string function)
            {
                if (value.Type == DataType.String) return new ModQuestOperand(ModQuestOperandKind.Literal, value.String);
                if (value.Type == DataType.Number) return new ModQuestOperand(ModQuestOperandKind.Literal,
                    value.Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                if (value.Type == DataType.Boolean) return new ModQuestOperand(ModQuestOperandKind.Literal, value.Boolean ? "1" : "0");
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be a literal or operand table.");
                Table table = value.Table; string kind = RequiredString(table, "kind", function);
                switch (kind)
                {
                    case "variable": ValidateFields(table, function, "kind", "name"); return new ModQuestOperand(ModQuestOperandKind.UserVariable, RequiredString(table, "name", function));
                    case "event_fight": ValidateFields(table, function, "kind"); return new ModQuestOperand(ModQuestOperandKind.EventFight);
                    case "fight_result": ValidateFields(table, function, "kind"); return new ModQuestOperand(ModQuestOperandKind.EventFightResult);
                    case "current_battle": ValidateFields(table, function, "kind"); return new ModQuestOperand(ModQuestOperandKind.CurrentFightBattle);
                    case "fight_wins":
                        ValidateFields(table, function, "kind", "fight");
                        return new ModQuestOperand(ModQuestOperandKind.FightWinCount,
                            RequiredHandle(table, "fight", _fightHandles, "fight", function));
                    case "fight_id":
                        ValidateFields(table, function, "kind", "fight");
                        return new ModQuestOperand(ModQuestOperandKind.FightId,
                            RequiredHandle(table, "fight", _fightHandles, "fight", function));
                    default: throw new ModContentException(function + " has unsupported operand kind '" + kind + "'.");
                }
            }

            private ModQuestAction[] ReadQuestActions(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModQuestAction>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i); if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table action = entry.Table; string type = RequiredString(action, "type", function + "[" + i + "]");
                    string where = function + "[" + i + "]";
                    switch (type)
                    {
                        case "dialog":
                            ValidateFields(action, where, "type", "title", "image", "lines", "button");
                            ModQuestDialogLine[] dialogLines = ReadQuestLines(action.Get("lines"), where + ".lines");
                            DynValue buttonValue = action.Get("button");
                            if (buttonValue.IsNil())
                            {
                                result.Add(ModQuestAction.Dialog(OptionalStringAllowEmpty(action, "title", string.Empty, where),
                                    OptionalStringAllowEmpty(action, "image", string.Empty, where), dialogLines));
                            }
                            else
                            {
                                if (buttonValue.Type != DataType.Table)
                                    throw new ModContentException(where + " field 'button' must be a table.");
                                Table button = buttonValue.Table;
                                ValidateFields(button, where + ".button", "text", "color", "actions");
                                result.Add(ModQuestAction.Dialog(
                                    OptionalStringAllowEmpty(action, "title", string.Empty, where),
                                    OptionalStringAllowEmpty(action, "image", string.Empty, where), dialogLines,
                                    new ModQuestDialogButton(
                                        RequiredString(button, "text", where + ".button"),
                                        ReadQuestActions(button.Get("actions"), where + ".button.actions"),
                                        OptionalString(button, "color", "Beige", where + ".button"))));
                            }
                            break;
                        case "story": ValidateFields(action, where, "type", "lines"); result.Add(ModQuestAction.StoryScreen(ReadQuestLines(action.Get("lines"), where + ".lines"))); break;
                        case "set_variable": ValidateFields(action, where, "type", "name", "value"); result.Add(ModQuestAction.SetUserVariable(RequiredString(action, "name", where), RequiredString(action, "value", where))); break;
                        case "show_battle": ValidateFields(action, where, "type", "battle", "locked"); result.Add(ModQuestAction.ShowBattle(RequiredHandle(action, "battle", _battleHandles, "battle", where), OptionalBool(action, "locked", false, where))); break;
                        case "toggle_battle": ValidateFields(action, where, "type", "battle", "visible"); result.Add(ModQuestAction.ToggleBattle(RequiredHandle(action, "battle", _battleHandles, "battle", where), OptionalBool(action, "visible", true, where))); break;
                        case "map_focus": ValidateFields(action, where, "type", "battle"); result.Add(ModQuestAction.SetMapFocus(RequiredHandle(action, "battle", _battleHandles, "battle", where))); break;
                        case "fight": ValidateFields(action, where, "type", "fight"); result.Add(ModQuestAction.StartFight(RequiredHandle(action, "fight", _fightHandles, "fight", where))); break;
                        case "current_fight": ValidateFields(action, where, "type"); result.Add(ModQuestAction.StartCurrentFight()); break;
                        case "eclipse": ValidateFields(action, where, "type", "enabled"); result.Add(ModQuestAction.ToggleEclipseMode(OptionalBool(action, "enabled", true, where))); break;
                        case "update_eclipse_battles": ValidateFields(action, where, "type"); result.Add(ModQuestAction.UpdateEclipseBattles()); break;
                        case "give_item": ValidateFields(action, where, "type", "item"); result.Add(ModQuestAction.GiveItem(RequiredHandle(action, "item", _itemHandles, "item", where))); break;
                        default: throw new ModContentException(where + " has unsupported action type '" + type + "'.");
                    }
                }
                return result.ToArray();
            }

            private static ModQuestDialogLine[] ReadQuestLines(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModQuestDialogLine>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i); if (entry.IsNil()) break;
                    if (entry.Type == DataType.String) { result.Add(new ModQuestDialogLine(entry.String)); continue; }
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be strings or tables.");
                    Table line = entry.Table; ValidateFields(line, function + "[" + i + "]", "text", "button", "frames");
                    result.Add(new ModQuestDialogLine(RequiredString(line, "text", function + "[" + i + "]"),
                        OptionalStringAllowEmpty(line, "button", string.Empty, function + "[" + i + "]"),
                        OptionalInt(line, "frames", 0, function + "[" + i + "]")));
                }
                return result.ToArray();
            }

            private ModItemSetMember[] ReadItemSetMembers(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModItemSetMember>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table item = entry.Table;
                    string itemFunction = function + "[" + i + "]";
                    ValidateFields(item, itemFunction, "item", "scale", "rotate", "x", "y", "icons_y");
                    result.Add(new ModItemSetMember(
                        RequiredHandle(item, "item", _itemHandles, "item", itemFunction),
                        OptionalFloat(item, "scale", 1f, itemFunction),
                        OptionalFloat(item, "rotate", 0f, itemFunction),
                        OptionalFloat(item, "x", 0f, itemFunction),
                        OptionalFloat(item, "y", 0f, itemFunction),
                        OptionalFloat(item, "icons_y", 0f, itemFunction)));
                }
                EnsureDenseArray(value.Table, result.Count, function);
                return result.ToArray();
            }

            private ModProgressionPerkEntry[] ReadProgressionEntries(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModProgressionPerkEntry>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table item = entry.Table;
                    string itemFunction = function + "[" + i + "]";
                    ValidateFields(item, itemFunction, "perk", "action");
                    DefinitionId perk = RequiredHandle(item, "perk", _perkHandles, "perk", itemFunction);
                    string actionValue = OptionalString(item, "action", "unlock", itemFunction);
                    ModProgressionPerkAction action;
                    if (actionValue == "unlock") action = ModProgressionPerkAction.Unlock;
                    else if (actionValue == "upgrade") action = ModProgressionPerkAction.Upgrade;
                    else throw new ModContentException(itemFunction + " field 'action' is not supported.");
                    result.Add(new ModProgressionPerkEntry(perk, action));
                }
                EnsureDenseArray(value.Table, result.Count, function);
                return result.ToArray();
            }

            private ModForgeRecipeItem[] ReadForgeRecipeItems(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModForgeRecipeItem>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table item = entry.Table;
                    string itemFunction = function + "[" + i + "]";
                    ValidateFields(item, itemFunction, "equipment", "enchantments", "bar_scale", "min_deviation",
                        "max_deviation", "random_aspect");
                    result.Add(new ModForgeRecipeItem(
                        ParseEquipmentKind(RequiredString(item, "equipment", itemFunction), itemFunction),
                        OptionalInt(item, "enchantments", 1, itemFunction),
                        OptionalStringAllowEmpty(item, "bar_scale", string.Empty, itemFunction),
                        OptionalInt(item, "min_deviation", 0, itemFunction),
                        OptionalInt(item, "max_deviation", 0, itemFunction),
                        OptionalBool(item, "random_aspect", false, itemFunction)));
                }
                EnsureDenseArray(value.Table, result.Count, function);
                return result.ToArray();
            }

            private ModForgeRecipeCandidate[] ReadForgeRecipeCandidates(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                var result = new List<ModForgeRecipeCandidate>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = value.Table.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    Table item = entry.Table;
                    string itemFunction = function + "[" + i + "]";
                    ValidateFields(item, itemFunction, "perk", "equipment", "min_level", "max_level");
                    result.Add(new ModForgeRecipeCandidate(
                        RequiredHandle(item, "perk", _perkHandles, "perk", itemFunction),
                        ParseEquipmentKind(RequiredString(item, "equipment", itemFunction), itemFunction),
                        OptionalInt(item, "min_level", int.MinValue, itemFunction),
                        OptionalInt(item, "max_level", int.MaxValue, itemFunction)));
                }
                EnsureDenseArray(value.Table, result.Count, function);
                return result.ToArray();
            }

            private static ModEquipmentKind ParseEquipmentKind(string value, string function)
            {
                switch (value)
                {
                    case "weapon": return ModEquipmentKind.Weapon;
                    case "armor": return ModEquipmentKind.Armor;
                    case "helm": return ModEquipmentKind.Helm;
                    case "ranged": return ModEquipmentKind.Ranged;
                    case "magic": return ModEquipmentKind.Magic;
                    default: throw new ModContentException(function + " field 'equipment' is not supported.");
                }
            }

            private static void EnsureDenseArray(Table table, int count, string function)
            {
                int entries = 0;
                foreach (TablePair pair in table.Pairs)
                {
                    if (pair.Key.Type != DataType.Number)
                        throw new ModContentException(function + " must be a dense array table.");
                    entries++;
                }
                if (entries != count) throw new ModContentException(function + " must be a dense array table.");
            }

            private static Dictionary<string, string> OptionalScalarMap(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                if (value.IsNil()) return result;
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a table.");
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String))
                        throw new ModContentException(function + " field '" + field + "' contains a non-string key.");
                    string scalar;
                    switch (pair.Value.Type)
                    {
                        case DataType.String:
                            scalar = pair.Value.String;
                            break;
                        case DataType.Number:
                            if (double.IsNaN(pair.Value.Number) || double.IsInfinity(pair.Value.Number))
                                throw new ModContentException(function + " field '" + field + "' contains a non-finite number.");
                            scalar = pair.Value.Number.ToString("R", CultureInfo.InvariantCulture);
                            break;
                        case DataType.Boolean:
                            scalar = pair.Value.Boolean ? "1" : "0";
                            break;
                        default:
                            throw new ModContentException(function + " field '" + field +
                                "' values must be strings, numbers, or booleans.");
                    }
                    if (string.IsNullOrEmpty(scalar))
                        throw new ModContentException(function + " field '" + field + "' contains an empty value.");
                    if (result.ContainsKey(pair.Key.String))
                        throw new ModContentException(function + " field '" + field + "' contains a duplicate key '" +
                            pair.Key.String + "'.");
                    result.Add(pair.Key.String, scalar);
                }
                return result;
            }

            private static T OptionalHandle<T>(Table table, string field, Dictionary<Table, T> handles,
                string kind, string function, T fallback)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return fallback;
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a " + kind + " handle.");
                T result;
                if (!handles.TryGetValue(value.Table, out result))
                    throw new ModContentException(function + " field '" + field + "' is not a " + kind +
                        " handle created by this mod context.");
                return result;
            }

            private static Dictionary<string, string> OptionalStringMap(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                if (value.IsNil()) return result;
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a table.");
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String) ||
                        pair.Value.Type != DataType.String || string.IsNullOrEmpty(pair.Value.String))
                        throw new ModContentException(function + " field '" + field +
                            "' must map non-empty string keys to non-empty string values.");
                    if (result.ContainsKey(pair.Key.String))
                        throw new ModContentException(function + " field '" + field + "' contains duplicate key '" +
                            pair.Key.String + "'.");
                    result.Add(pair.Key.String, pair.Value.String);
                }
                return result;
            }

            private static string[] OptionalStringArray(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return Array.Empty<string>();
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be an array table.");
                var result = new List<string>();
                for (int i = 1; ; i++)
                {
                    DynValue item = value.Table.Get(i);
                    if (item.IsNil()) break;
                    if (item.Type != DataType.String || string.IsNullOrEmpty(item.String))
                        throw new ModContentException(function + " field '" + field +
                            "' entries must be non-empty strings.");
                    result.Add(item.String);
                }
                int entries = 0;
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.Number) throw new ModContentException(function + " field '" + field +
                        "' must be a dense array table.");
                    entries++;
                }
                if (entries != result.Count)
                    throw new ModContentException(function + " field '" + field + "' must be a dense array table.");
                return result.ToArray();
            }

            private static Dictionary<int, DynValue> OptionalMigrationMap(Table table, string field, int targetVersion,
                string function)
            {
                DynValue value = table.Get(field);
                var result = new Dictionary<int, DynValue>();
                if (value.IsNil()) return result;
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a table.");
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.Number || Math.Truncate(pair.Key.Number) != pair.Key.Number ||
                        pair.Key.Number < 1 || pair.Key.Number >= targetVersion)
                        throw new ModContentException(function + " field '" + field +
                            "' keys must be integer source schema versions in 1..version-1.");
                    int fromVersion = (int)pair.Key.Number;
                    if (pair.Value.Type != DataType.Function && pair.Value.Type != DataType.ClrFunction)
                        throw new ModContentException(function + " migration " + fromVersion +
                            " must be a function.");
                    if (result.ContainsKey(fromVersion))
                        throw new ModContentException(function + " contains duplicate migration for schema " +
                            fromVersion + ".");
                    result.Add(fromVersion, pair.Value);
                }
                return result;
            }

            private static ModParameterSchema OptionalParameterSchema(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return new ModParameterSchema(Array.Empty<ModParameterDefinition>());
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a table.");
                var result = new List<ModParameterDefinition>();
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String))
                        throw new ModContentException(function + " field '" + field + "' contains a non-string key.");
                    string name = pair.Key.String;
                    ModParameterType type;
                    bool required = true;
                    bool hasDefault = false;
                    ModParameterValue defaultValue = default;

                    if (pair.Value.Type == DataType.String)
                    {
                        type = ParseParameterType(pair.Value.String, function, name);
                    }
                    else if (pair.Value.Type == DataType.Table)
                    {
                        Table definition = pair.Value.Table;
                        ValidateFields(definition, function + ".parameters." + name, "type", "required", "default");
                        type = ParseParameterType(RequiredString(definition, "type", function + ".parameters." + name),
                            function, name);
                        DynValue requiredValue = definition.Get("required");
                        if (!requiredValue.IsNil())
                        {
                            if (requiredValue.Type != DataType.Boolean)
                                throw new ModContentException(function + " parameter '" + name +
                                    "' field 'required' must be boolean.");
                            required = requiredValue.Boolean;
                        }
                        DynValue defaultDyn = definition.Get("default");
                        if (!defaultDyn.IsNil())
                        {
                            defaultValue = ParameterValue(type, defaultDyn, function + " parameter '" + name + "' default");
                            hasDefault = true;
                        }
                    }
                    else
                    {
                        throw new ModContentException(function + " parameter '" + name +
                            "' must be a type token or schema table.");
                    }

                    result.Add(hasDefault
                        ? new ModParameterDefinition(name, type, required, defaultValue)
                        : new ModParameterDefinition(name, type, required));
                }
                return new ModParameterSchema(result);
            }

            private static Dictionary<string, ModParameterValue> OptionalTypedParameterMap(Table table, string field,
                ModParameterSchema schema, string function)
            {
                DynValue value = table.Get(field);
                var result = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
                if (value.IsNil()) return result;
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be a table.");
                foreach (TablePair pair in value.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.String || string.IsNullOrEmpty(pair.Key.String))
                        throw new ModContentException(function + " field '" + field + "' contains a non-string key.");
                    ModParameterDefinition definition;
                    if (!schema.TryGet(pair.Key.String, out definition))
                        throw new ModContentException(function + " field '" + field + "' contains unknown parameter '" +
                            pair.Key.String + "'.");
                    result.Add(pair.Key.String, ParameterValue(definition.Type, pair.Value,
                        function + " parameter '" + pair.Key.String + "'"));
                }
                return result;
            }

            private static ModParameterType ParseParameterType(string value, string function, string name)
            {
                switch (value)
                {
                    case "number": return ModParameterType.Number;
                    case "integer": return ModParameterType.Integer;
                    case "boolean": return ModParameterType.Boolean;
                    case "string": return ModParameterType.String;
                    default: throw new ModContentException(function + " parameter '" + name +
                        "' has unsupported type '" + value + "'.");
                }
            }

            private static ModParameterValue ParameterValue(ModParameterType type, DynValue value, string name)
            {
                switch (type)
                {
                    case ModParameterType.Number:
                        if (value.Type != DataType.Number || double.IsNaN(value.Number) || double.IsInfinity(value.Number))
                            throw new ModContentException(name + " must be a finite number.");
                        return ModParameterValue.FromNumber(value.Number);
                    case ModParameterType.Integer:
                        if (value.Type != DataType.Number || double.IsNaN(value.Number) || double.IsInfinity(value.Number) ||
                            Math.Truncate(value.Number) != value.Number ||
                            value.Number < ModParameterValue.MinSafeInteger || value.Number > ModParameterValue.MaxSafeInteger)
                            throw new ModContentException(name + " must be an exact Lua integer.");
                        return ModParameterValue.FromInteger((long)value.Number);
                    case ModParameterType.Boolean:
                        if (value.Type != DataType.Boolean) throw new ModContentException(name + " must be boolean.");
                        return ModParameterValue.FromBoolean(value.Boolean);
                    case ModParameterType.String:
                        if (value.Type != DataType.String) throw new ModContentException(name + " must be a string.");
                        return ModParameterValue.FromString(value.String);
                    default:
                        throw new ModContentException(name + " has unsupported type " + type + ".");
                }
            }

            private static ModEquipmentKind[] RequiredEquipmentKinds(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.Type != DataType.Table)
                    throw new ModContentException(function + " field '" + field + "' must be an array table.");
                var result = new List<ModEquipmentKind>();
                for (int i = 1; ; i++)
                {
                    DynValue item = value.Table.Get(i);
                    if (item.IsNil()) break;
                    if (item.Type != DataType.String)
                        throw new ModContentException(function + " field '" + field + "' entries must be strings.");
                    ModEquipmentKind kind;
                    switch (item.String)
                    {
                        case "weapon": kind = ModEquipmentKind.Weapon; break;
                        case "armor": kind = ModEquipmentKind.Armor; break;
                        case "helm": kind = ModEquipmentKind.Helm; break;
                        case "ranged": kind = ModEquipmentKind.Ranged; break;
                        case "magic": kind = ModEquipmentKind.Magic; break;
                        default: throw new ModContentException(function + " field '" + field +
                            "' contains unsupported equipment type '" + item.String + "'.");
                    }
                    result.Add(kind);
                }
                if (result.Count == 0)
                    throw new ModContentException(function + " field '" + field + "' must not be empty.");
                int arrayEntries = 0;
                foreach (TablePair pair in value.Table.Pairs)
                    if (pair.Key.Type == DataType.Number) arrayEntries++;
                    else throw new ModContentException(function + " field '" + field + "' must be an array table.");
                if (arrayEntries != result.Count)
                    throw new ModContentException(function + " field '" + field + "' must be a dense array table.");
                return result.ToArray();
            }

            private static int RequiredInt(Table table, string field, string function)
            {
                DynValue value = table.Get(field);
                if (value.Type != DataType.Number)
                    throw new ModContentException(function + " field '" + field + "' must be an integer.");
                return ToInt(value.Number, function + " field '" + field + "'");
            }

            private static int RequiredInt(CallbackArguments args, int index, string function)
            {
                DynValue value = args[index];
                if (value.Type != DataType.Number)
                    throw new ModContentException(function + " argument " + (index + 1) + " must be an integer.");
                return ToInt(value.Number, function + " argument " + (index + 1));
            }

            private static int ToInt(double value, string name)
            {
                if (double.IsNaN(value) || double.IsInfinity(value) || value < int.MinValue || value > int.MaxValue ||
                    Math.Truncate(value) != value)
                    throw new ModContentException(name + " must be a 32-bit integer.");
                return (int)value;
            }

            private static void ValidateFields(Table table, string function, params string[] fields)
            {
                var allowed = new HashSet<string>(fields, StringComparer.Ordinal);
                foreach (TablePair pair in table.Pairs)
                {
                    if (pair.Key.Type != DataType.String)
                        throw new ModContentException(function + " input table contains a non-string field.");
                    if (!allowed.Contains(pair.Key.String))
                        throw new ModContentException(function + " input table contains unknown field '" +
                            pair.Key.String + "'.");
                }
            }

            private static DynValue ApiCall(string function, Func<DynValue> action)
            {
                try { return action(); }
                catch (Exception exception) when (exception is ModContentException || exception is FormatException ||
                    exception is InvalidOperationException || exception is ArgumentException)
                {
                    throw new ScriptRuntimeException(function + ": " + exception.Message);
                }
            }

            private DynValue LoadChunk(AssetId id, string sourceName)
            {
                AssetBytes bytes;
                if (!_api.Assets.TryRead(id, out bytes))
                    throw new FileNotFoundException("Lua source was not found in the mod virtual filesystem: " + sourceName);
                if (bytes.Metadata.Kind != AssetKind.Text || bytes.Metadata.Format != ".lua")
                    throw new InvalidDataException("Lua source is not a .lua text asset: " + id);
                if (bytes.Data.Length > MaxSourceBytes)
                    throw new InvalidDataException("Lua source exceeds " + MaxSourceBytes + " bytes: " + sourceName);

                string source;
                try { source = StrictUtf8.GetString(bytes.Data); }
                catch (DecoderFallbackException exception)
                {
                    throw new InvalidDataException("Lua source is not valid UTF-8: " + sourceName, exception);
                }
                return _script.LoadString(source, null, sourceName);
            }

            private AssetId EntrypointId()
            {
                string path = Mod.Manifest.Entrypoint;
                if (!path.EndsWith(".lua", StringComparison.Ordinal))
                    throw new InvalidDataException("Lua entrypoint must end in .lua: " + path);
                return AssetId.Parse(Mod.Id.Value + ":" + path.Substring(0, path.Length - 4));
            }

            private static string CanonicalModuleName(string module)
            {
                if (string.IsNullOrWhiteSpace(module))
                    throw new ScriptRuntimeException("Module name must not be empty.");
                string trimmed = module.Trim();
                if (!string.Equals(trimmed, module, StringComparison.Ordinal) || trimmed.IndexOf('/') >= 0 ||
                    trimmed.IndexOf('\\') >= 0 || trimmed.IndexOf(':') >= 0 || trimmed.StartsWith(".", StringComparison.Ordinal) ||
                    trimmed.EndsWith(".", StringComparison.Ordinal) || trimmed.Contains(".."))
                    throw new ScriptRuntimeException("Unsafe module name '" + module + "'.");

                for (int i = 0; i < trimmed.Length; i++)
                {
                    char c = trimmed[i];
                    if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                        c == '_' || c == '-' || c == '.'))
                        throw new ScriptRuntimeException("Unsafe module name '" + module + "'.");
                }
                return trimmed.ToLowerInvariant();
            }

            private ModScriptException Wrap(string sourceName, InterpreterException exception)
            {
                string message = string.IsNullOrEmpty(exception.DecoratedMessage) ? exception.Message : exception.DecoratedMessage;
                return new ModScriptException(Mod.Id, sourceName, message, exception);
            }

            private void ThrowIfDisposed()
            {
                if (_disposed) throw new ObjectDisposedException(nameof(MoonSharpScriptContext));
            }
        }
    }
}
