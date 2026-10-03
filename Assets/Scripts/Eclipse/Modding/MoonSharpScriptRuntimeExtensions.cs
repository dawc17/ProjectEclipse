using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModExtensionDefinition> _extensionHandles =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModExtensionDefinition>();
            private bool _migratingState;

            private void AddExtensionsModule(Table root)
            {
                var extensions = new Table(_script);
                extensions.Set("register", DynValue.NewCallback((ctx, args) => ApiCall("sf2.extensions.register", () => RegisterExtension(args))));
                extensions.Set("get", DynValue.NewCallback((ctx, args) => ApiCall("sf2.extensions.get", () => GetExtension(args))));
                extensions.Set("call", DynValue.NewCallback((ctx, args) => ApiCall("sf2.extensions.call", () => CallExtension(args))));
                extensions.Set("try_call", DynValue.NewCallback((ctx, args) => ApiCall("sf2.extensions.try_call", () =>
                {
                    _api.RequireCapability("extensions.call");
                    try { return DynValue.NewTuple(CallExtension(args), DynValue.Nil); }
                    catch (Exception error) when (error is ModContentException || error is InterpreterException ||
                        error is ArgumentException || error is InvalidOperationException)
                    { return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(error.Message)); }
                })));
                root.Set("extensions", DynValue.NewTable(extensions));
            }

            private ModExtensionRegistry RequireExtensions()
            {
                ThrowIfDisposed();
                if (_api.Extensions == null) throw new ModContentException("This host has no mod extension registry.");
                if (_migratingState) throw new ModContentException("State migrations cannot call mod extensions.");
                return _api.Extensions;
            }

            private DynValue RegisterExtension(CallbackArguments args)
            {
                const string function = "sf2.extensions.register";
                _api.RequireCapability("extensions.provide");
                var registry = RequireExtensions();
                if (_api.Registration == null) throw new ModContentException("Extension registration is unavailable on this host.");
                var table = args.AsType(0, function, DataType.Table, false).Table;
                ValidateFields(table, function, "id", "version", "request", "response", "handler");
                var handler = table.Get("handler");
                if (handler.Type != DataType.Function) throw new ModContentException("Extension handler must be a Lua function.");
                var definition = _api.Registration.RegisterExtension(RequiredString(table, "id", function),
                    RequiredInt(table, "version", function),
                    OptionalParameterSchema(table, "request", function), OptionalParameterSchema(table, "response", function));
                registry.Stage(definition, (caller, input) =>
                {
                    var request = ExtensionValues(input);
                    var result = RunBounded(handler, definition.Id + ":handler", MaxBehaviorInstructionSlices,
                        new[] { DynValue.NewTable(request), DynValue.NewString(caller.Value) });
                    if (result.Type != DataType.Table) throw new ModContentException("Extension handler must return a response table.");
                    return ReadExtensionValues(result.Table, definition.Response, definition.Id + ".response");
                });
                return DynValue.NewString(definition.Id.ToString());
            }

            private DynValue GetExtension(CallbackArguments args)
            {
                const string function = "sf2.extensions.get";
                _api.RequireCapability("extensions.call");
                var reference = args.AsType(0, function, DataType.String, false).String;
                var version = args.AsType(1, function, DataType.Number, false).Number;
                if (double.IsNaN(version) || version < 1 || version > 1000000 || Math.Truncate(version) != version)
                    throw new ModContentException("Extension version must be an integer from 1..1000000.");
                var definition = RequireExtensions().Resolve(Mod, DefinitionId.Parse(reference), (int)version);
                var handle = new Table(_script);
                _extensionHandles.Add(handle, definition);
                return DynValue.NewTable(handle);
            }

            private DynValue CallExtension(CallbackArguments args)
            {
                const string function = "sf2.extensions.call";
                _api.RequireCapability("extensions.call");
                var registry = RequireExtensions();
                var handle = args.AsType(0, function, DataType.Table, false).Table;
                if (!_extensionHandles.TryGetValue(handle, out var definition))
                    throw new ModContentException("Expected an extension handle acquired by this script.");
                var request = args.AsType(1, function, DataType.Table, false).Table;
                var input = ReadExtensionValues(request, definition.Request, definition.Id + ".request");
                return DynValue.NewTable(ExtensionValues(registry.Call(Mod, definition.Id, definition.Version, input)));
            }

            private Table ExtensionValues(IReadOnlyDictionary<string, ModParameterValue> values)
            {
                var table = new Table(_script);
                foreach (var pair in values) table.Set(pair.Key, ToDynValue(pair.Value));
                return table;
            }

            private static IReadOnlyDictionary<string, ModParameterValue> ReadExtensionValues(Table table,
                ModParameterSchema schema, string where)
            {
                // Plain records only. Metatables/functions/handles remain local to their script.
                if (table.MetaTable != null) throw new ModContentException(where + " must not have a metatable.");
                var values = new Dictionary<string, ModParameterValue>(StringComparer.Ordinal);
                foreach (var pair in table.Pairs)
                {
                    if (pair.Key.Type != DataType.String || !schema.TryGet(pair.Key.String, out var field))
                        throw new ModContentException(where + " contains an unknown field.");
                    values.Add(pair.Key.String, ParameterValue(field.Type, pair.Value, where + "." + field.Name));
                }
                return schema.ResolveValues(values);
            }
        }
    }
}
