using System;
using System.Collections.Generic;
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

        public string Name => "MoonSharp " + Script.VERSION;

        public IModScriptContext CreateContext(ModDescriptor mod, ModApiFacade api)
        {
            return new MoonSharpScriptContext(mod, api);
        }

        private sealed class MoonSharpScriptContext : IModScriptContext
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
                DynValue coroutineValue = _script.CreateCoroutine(function);
                Coroutine coroutine = coroutineValue.Coroutine;
                coroutine.AutoYieldCounter = InstructionSlice;

                int forcedYields = 0;
                while (true)
                {
                    DynValue result = coroutine.Resume();
                    if (coroutine.State == CoroutineState.Dead) return result;
                    if (coroutine.State == CoroutineState.ForceSuspended)
                    {
                        forcedYields++;
                        if (forcedYields >= MaxInstructionSlices)
                            throw new ScriptRuntimeException("Execution instruction budget exceeded in '" +
                                sourceName + "' (limit " + MaxEntrypointInstructions + ").");
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
                mod.Set("log", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Info, "sf2.mod.log", args)));
                mod.Set("warn", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Warning, "sf2.mod.warn", args)));
                mod.Set("error", DynValue.NewCallback((ctx, args) => LogCallback(ModLogLevel.Error, "sf2.mod.error", args)));
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
                root.Set("items", DynValue.NewTable(items));

                var price = new Table(_script);
                price.Set("coins", DynValue.NewCallback((ctx, args) => Price(ModPriceCurrency.Coins,
                    "sf2.price.coins", args)));
                price.Set("gems", DynValue.NewCallback((ctx, args) => Price(ModPriceCurrency.Gems,
                    "sf2.price.gems", args)));
                root.Set("price", DynValue.NewTable(price));

                var shop = new Table(_script);
                shop.Set("WEAPONS", DynValue.NewString("weapons"));
                shop.Set("add", DynValue.NewCallback(ShopAdd));
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
                        "subtype", "damage");
                    string id = RequiredString(table, "id", "sf2.items.register_weapon");
                    DefinitionId displayName = RequiredHandle(table, "display_name", _localizationHandles,
                        "localization", "sf2.items.register_weapon");
                    AssetId icon = RequiredHandle(table, "icon", _spriteHandles, "sprite",
                        "sf2.items.register_weapon");
                    AssetId model = RequiredHandle(table, "model", _modelHandles, "model",
                        "sf2.items.register_weapon");
                    string subType = OptionalString(table, "subtype", "Katana", "sf2.items.register_weapon");
                    int damage = RequiredInt(table, "damage", "sf2.items.register_weapon");
                    WeaponDefinition definition = _api.RegisterWeapon(id, displayName, icon, model, subType, damage);
                    return NewHandle(_itemHandles, definition.Id);
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

            private DynValue ShopAdd(ScriptExecutionContext context, CallbackArguments args)
            {
                Table table = args.AsType(0, "sf2.shop.add", DataType.Table, false).Table;
                return ApiCall("sf2.shop.add", () =>
                {
                    ValidateFields(table, "sf2.shop.add", "section", "item", "level", "price");
                    string sectionText = RequiredString(table, "section", "sf2.shop.add");
                    if (!string.Equals(sectionText, "weapons", StringComparison.Ordinal))
                        throw new ModContentException("sf2.shop.add field 'section' only supports sf2.shop.WEAPONS.");
                    DefinitionId item = RequiredHandle(table, "item", _itemHandles, "item", "sf2.shop.add");
                    int level = RequiredInt(table, "level", "sf2.shop.add");
                    ModPrice price = RequiredHandle(table, "price", _priceHandles, "price", "sf2.shop.add");
                    ShopListingDefinition listing = _api.RegisterShopListing(item, ModShopSection.Weapons, level, price);
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
