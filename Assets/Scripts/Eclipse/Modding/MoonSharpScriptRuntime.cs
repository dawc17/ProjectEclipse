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
                    _script.Call(function);
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
                    DynValue value = _script.Call(function, DynValue.NewString(canonical));
                    if (value == null || value.IsNil()) value = DynValue.True;
                    _modules.Add(canonical, value);
                    return value;
                }
                finally
                {
                    _loading.Remove(canonical);
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
                root.Set("assets", DynValue.NewTable(assets));

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
