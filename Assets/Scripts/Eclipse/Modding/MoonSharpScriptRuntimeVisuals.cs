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

                var fx = new Table(_script);
                fx.Set("particles", DynValue.NewCallback(Fx("particles", ModFxKind.Particles,
                    "placement", "node", "fighters", "blend", "sprite", "color", "end_color")));
                fx.Set("overlay", DynValue.NewCallback(Fx("overlay", ModFxKind.Overlay, "placement", "blend", "sprite", "color")));
                fx.Set("trail", DynValue.NewCallback(Fx("trail", ModFxKind.Trail, "weapon", "nodes", "fighters", "blend", "color")));
                fx.Set("screen", DynValue.NewCallback(Fx("screen", ModFxKind.Screen, "tint")));
                root.Set("fx", DynValue.NewTable(fx));
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

            // sf2.fx.*: composable effects with the mod's own art and numbers.
            private Func<ScriptExecutionContext, CallbackArguments, DynValue> Fx(string name, ModFxKind kind, params string[] extra)
            {
                string function = "sf2.fx." + name;
                return (ctx, args) => ApiCall(function, () =>
                {
                    Table table = args.AsType(0, function, DataType.Table, false).Table;
                    var allowed = new List<string> { "id", "setting", "match", "scenes" };
                    foreach (var parameter in ModFxParameters.For(kind)) allowed.Add(parameter.Name);
                    allowed.AddRange(extra);
                    ValidateFields(table, function, allowed.ToArray());
                    string id = RequiredString(table, "id", function);
                    var request = new ModFxRequest();
                    foreach (var parameter in ModFxParameters.For(kind))
                        if (!table.Get(parameter.Name).IsNil())
                            request.Numbers[parameter.Name] = OptionalFloat(table, parameter.Name, parameter.Default, function);

                    DynValue settingValue = table.Get("setting");
                    if (!settingValue.IsNil())
                    {
                        if (settingValue.Type != DataType.Table || !_settingHandles.TryGetValue(settingValue.Table, out var toggle))
                            throw new ModContentException(function + ".setting must be a handle from sf2.settings.toggle.");
                        request.Setting = toggle.Name;
                    }
                    request.Match = OptionalWords(table, "match", function);
                    request.Nodes = OptionalWords(table, "nodes", function, false);
                    if (!table.Get("node").IsNil()) request.Nodes = new[] { RequiredString(table, "node", function) };

                    string scenes = OptionalString(table, "scenes", "fights", function);
                    request.Scenes = scenes == "fights" ? ModFxScenes.Fights : scenes == "everywhere" ? ModFxScenes.Everywhere
                        : throw new ModContentException(function + ".scenes must be fights or everywhere.");
                    string defaultPlacement = kind == ModFxKind.Overlay ? "background" : "behind";
                    string placement = OptionalString(table, "placement", defaultPlacement, function);
                    request.Placement = placement == "background" ? ModFxPlacement.Background : placement == "behind" ? ModFxPlacement.Behind
                        : placement == "front" ? ModFxPlacement.Front : placement == "node" ? ModFxPlacement.Node
                        : throw new ModContentException(function + ".placement must be background, behind, front or node.");
                    if (kind == ModFxKind.Particles && request.Nodes != null && request.Nodes.Length != 0 && table.Get("placement").IsNil())
                        request.Placement = ModFxPlacement.Node;
                    string fighters = OptionalString(table, "fighters", "both", function);
                    request.Fighters = fighters == "both" ? ModFxFighters.Both : fighters == "player" ? ModFxFighters.Player
                        : fighters == "opponent" ? ModFxFighters.Opponent : throw new ModContentException(function + ".fighters must be both, player or opponent.");
                    string blend = OptionalString(table, "blend", "alpha", function);
                    request.Blend = blend == "alpha" ? ModFxBlend.Alpha : blend == "additive" ? ModFxBlend.Additive
                        : throw new ModContentException(function + ".blend must be alpha or additive.");
                    request.Weapon = OptionalBool(table, "weapon", false, function);
                    if (!table.Get("sprite").IsNil()) request.Sprite = RequiredHandle(table, "sprite", _spriteHandles, "sprite", function);
                    request.Color = OptionalColor(table, "color", function);
                    request.EndColor = OptionalColor(table, "end_color", function);
                    if (request.Color == null) request.Color = OptionalColor(table, "tint", function);

                    ModFxDefinition definition = _api.RegisterFx(kind, id, request);
                    return DynValue.NewString(definition.Name);
                });
            }

            private static string[] OptionalWords(Table table, string field, string function, bool lowercaseOnly = true)
            {
                DynValue value = table.Get(field);
                if (value.IsNil()) return null;
                if (value.Type != DataType.Table) throw new ModContentException(function + "." + field + " must be an array of strings.");
                var words = new List<string>();
                foreach (DynValue word in value.Table.Values)
                {
                    if (word.Type != DataType.String) throw new ModContentException(function + "." + field + " must contain strings.");
                    words.Add(word.String);
                }
                return words.ToArray();
            }

            private static ModUiColor OptionalColor(Table table, string field, string function)
            {
                if (table.Get(field).IsNil()) return null;
                try { return new ModUiColor(RequiredString(table, field, function)); }
                catch (ModContentException) { throw; }
                catch (Exception error) { throw new ModContentException(function + "." + field + ": " + error.Message); }
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
