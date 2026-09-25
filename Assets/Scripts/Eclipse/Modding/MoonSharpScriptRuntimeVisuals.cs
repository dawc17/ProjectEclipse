using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly Dictionary<Table, ModSettingToggle> _settingHandles = new Dictionary<Table, ModSettingToggle>();

            // sf2.settings: installation-wide toggles shown under Options > Mod settings.
            // sf2.visuals: typed configuration for the engine's optional fight visuals.
            private void AddVisualsModule(Table root)
            {
                var settings = new Table(_script);
                settings.Set("toggle", DynValue.NewCallback((ctx, args) => ApiCall("sf2.settings.toggle", () =>
                {
                    const string function = "sf2.settings.toggle";
                    Table table = args.AsType(0, function, DataType.Table, false).Table;
                    ValidateFields(table, function, "id", "label", "description", "default");
                    ModSettingToggle toggle = _api.RegisterSettingToggle(RequiredString(table, "id", function),
                        RequiredString(table, "label", function), OptionalString(table, "description", null, function),
                        OptionalBool(table, "default", false, function));
                    return NewHandle(_settingHandles, toggle);
                })));
                settings.Set("get", DynValue.NewCallback((ctx, args) => ApiCall("sf2.settings.get", () =>
                {
                    Table handle = args.AsType(0, "sf2.settings.get", DataType.Table, false).Table;
                    if (!_settingHandles.TryGetValue(handle, out var toggle))
                        throw new ModContentException("sf2.settings.get requires a setting handle created by this script context.");
                    return DynValue.NewBoolean(ModSettingValues.Read(toggle));
                })));
                root.Set("settings", DynValue.NewTable(settings));

                var visuals = new Table(_script);
                visuals.Set("background_depth", DynValue.NewCallback(Visual("background_depth", ModVisualEffect.BackgroundDepth)));
                visuals.Set("weapon_trails", DynValue.NewCallback(Visual("weapon_trails", ModVisualEffect.WeaponTrails, "color")));
                visuals.Set("depth_haze", DynValue.NewCallback(Visual("depth_haze", ModVisualEffect.DepthHaze)));
                visuals.Set("rim_light", DynValue.NewCallback(Visual("rim_light", ModVisualEffect.RimLight)));
                visuals.Set("bloom", DynValue.NewCallback(Visual("bloom", ModVisualEffect.Bloom)));
                visuals.Set("ambient_particles", DynValue.NewCallback(Visual("ambient_particles", ModVisualEffect.AmbientParticles, "default_style", "locations")));
                visuals.Set("impact", DynValue.NewCallback(Visual("impact", ModVisualEffect.Impact)));
                root.Set("visuals", DynValue.NewTable(visuals));
            }

            private Func<ScriptExecutionContext, CallbackArguments, DynValue> Visual(string name, ModVisualEffect effect, params string[] extra)
            {
                string function = "sf2.visuals." + name;
                return (ctx, args) => ApiCall(function, () =>
                {
                    Table table = args.AsType(0, function, DataType.Table, true).Table ?? new Table(_script);
                    var allowed = new List<string> { "setting" };
                    foreach (var parameter in ModVisualParameters.For(effect)) allowed.Add(parameter.Name);
                    allowed.AddRange(extra);
                    ValidateFields(table, function, allowed.ToArray());

                    var numbers = new Dictionary<string, float>(StringComparer.Ordinal);
                    foreach (var parameter in ModVisualParameters.For(effect))
                        if (!table.Get(parameter.Name).IsNil())
                            numbers[parameter.Name] = OptionalFloat(table, parameter.Name, parameter.Default, function);

                    string setting = null;
                    DynValue settingValue = table.Get("setting");
                    if (!settingValue.IsNil())
                    {
                        if (settingValue.Type != DataType.Table || !_settingHandles.TryGetValue(settingValue.Table, out var toggle))
                            throw new ModContentException(function + ".setting must be a handle from sf2.settings.toggle.");
                        setting = toggle.Name;
                    }

                    ModUiColor color = null;
                    if (!table.Get("color").IsNil())
                    {
                        try { color = new ModUiColor(RequiredString(table, "color", function)); }
                        catch (Exception error) { throw new ModContentException(function + ".color: " + error.Message); }
                    }

                    ModParticleStyle defaultStyle = ModParticleStyle.Dust;
                    if (!table.Get("default_style").IsNil())
                        defaultStyle = ReadParticleStyle(RequiredString(table, "default_style", function), function + ".default_style");

                    var rules = new List<ModParticleRule>();
                    DynValue locations = table.Get("locations");
                    if (!locations.IsNil())
                    {
                        if (locations.Type != DataType.Table) throw new ModContentException(function + ".locations must be an array.");
                        int index = 0;
                        foreach (DynValue entry in locations.Table.Values)
                        {
                            string where = function + ".locations[" + (++index) + "]";
                            if (entry.Type != DataType.Table) throw new ModContentException(where + " must be a table.");
                            ValidateFields(entry.Table, where, "match", "style");
                            DynValue match = entry.Table.Get("match");
                            if (match.Type != DataType.Table) throw new ModContentException(where + ".match must be an array of words.");
                            var words = new List<string>();
                            foreach (DynValue word in match.Table.Values)
                            {
                                if (word.Type != DataType.String) throw new ModContentException(where + ".match must contain strings.");
                                words.Add(word.String);
                            }
                            rules.Add(new ModParticleRule(words.ToArray(),
                                ReadParticleStyle(RequiredString(entry.Table, "style", where), where + ".style")));
                        }
                    }

                    _api.RegisterVisual(effect, numbers, setting, color, defaultStyle, rules);
                    return DynValue.Nil;
                });
            }

            private static ModParticleStyle ReadParticleStyle(string value, string where)
            {
                ModParticleStyle? style = ModVisualParameters.ParseStyle(value);
                if (!style.HasValue) throw new ModContentException(where + " must be none, dust, snow, embers or petals.");
                return style.Value;
            }
        }
    }
}
