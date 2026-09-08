using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed class MoonSharpScriptRuntime : IModScriptRuntime
    {
        public const int MaxSourceBytes = 1024 * 1024;
        public const int MaxModules = 128;
        public const long InstructionSlice = 50000;
        public const int MaxInstructionSlices = 100;
        public const long MaxEntrypointInstructions = InstructionSlice * MaxInstructionSlices;
        public const int MaxBehaviorInstructionSlices = 4;
        public const long MaxBehaviorInstructions = InstructionSlice * MaxBehaviorInstructionSlices;

        public string Name => "MoonSharp " + Script.VERSION;

        public IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api)
        {
            return new MoonSharpScriptContext(mod, api);
        }

        private sealed class MoonSharpScriptContext : IModScriptContext, IModBehaviorScriptContext,
            IModInteractiveBehaviorScriptContext
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
            private readonly Dictionary<Table, ModPrice> _priceHandles =
                new Dictionary<Table, ModPrice>();
            private readonly Dictionary<Table, DefinitionId> _perkHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _enchantmentHandles =
                new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, ModBehaviorDefinition> _behaviorHandles =
                new Dictionary<Table, ModBehaviorDefinition>();
            private readonly Dictionary<DefinitionId, DynValue> _behaviorHandlers =
                new Dictionary<DefinitionId, DynValue>();
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
                return effectEvent == ModEffectEvent.FightBegin && _behaviorHandlers.ContainsKey(behaviorId);
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
                if (effectEvent != ModEffectEvent.FightBegin)
                {
                    error = "Unsupported behavior event: " + effectEvent + ".";
                    return false;
                }
                DynValue handler;
                if (!_behaviorHandlers.TryGetValue(behaviorId, out handler))
                {
                    error = "Behavior handler is not registered: '" + behaviorId + "'.";
                    return false;
                }

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
                            FighterOperation("fighter:change_health", "combat.change_life", args, fighter.TryChangeHealth)));
                        fighterTable.Set("add_magic_charge", DynValue.NewCallback((ctx, args) =>
                            FighterOperation("fighter:add_magic_charge", "combat.magic_charge", args, fighter.TryAddMagicCharge)));
                    }
                    RunBounded(handler, behaviorId + ":" + effectEvent, MaxBehaviorInstructionSlices,
                        new[] { DynValue.NewTable(parameterTable), DynValue.NewTable(fighterTable) });
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
                _priceHandles.Clear();
                _perkHandles.Clear();
                _enchantmentHandles.Clear();
                _behaviorHandles.Clear();
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
                root.Set("localization", DynValue.NewTable(localization));

                var items = new Table(_script);
                items.Set("register_weapon", DynValue.NewCallback(RegisterWeapon));
                items.Set("register_armor", DynValue.NewCallback(RegisterArmor));
                items.Set("register_helm", DynValue.NewCallback(RegisterHelm));
                items.Set("register_ranged", DynValue.NewCallback(RegisterRanged));
                items.Set("register_magic", DynValue.NewCallback(RegisterMagic));
                items.Set("alias", DynValue.NewCallback(RegisterItemAlias));
                items.Set("tombstone", DynValue.NewCallback(RegisterItemTombstone));
                root.Set("items", DynValue.NewTable(items));

                var perks = new Table(_script);
                perks.Set("SINGLE", DynValue.NewString("single"));
                perks.Set("COMBO", DynValue.NewString("combo"));
                perks.Set("get", DynValue.NewCallback(GetPerk));
                perks.Set("register", DynValue.NewCallback(RegisterPerk));
                root.Set("perks", DynValue.NewTable(perks));

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
                shop.Set("addItem", DynValue.NewCallback(ShopAddItem));
                shop.Set("add", shop.Get("addItem"));
                root.Set("shop", DynValue.NewTable(shop));

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
                    ValidateFields(table, function, "id", "parameters", "on_fight_begin");
                    string id = RequiredString(table, "id", function);
                    ModParameterSchema parameters = OptionalParameterSchema(table, "parameters", function);
                    DynValue handler = table.Get("on_fight_begin");
                    if (handler.Type != DataType.Function)
                        throw new ModContentException(function + " field 'on_fight_begin' must be a Lua function.");
                    ModBehaviorDefinition definition = _api.RegisterBehavior(id, parameters);
                    _behaviorHandlers.Add(definition.Id, handler);
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
