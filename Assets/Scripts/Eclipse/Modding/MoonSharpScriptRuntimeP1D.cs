using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private readonly Dictionary<Table, AssetId> _audioHandles = new Dictionary<Table, AssetId>();
            private readonly Dictionary<Table, AssetId> _binaryHandles = new Dictionary<Table, AssetId>();
            private readonly Dictionary<Table, DefinitionId> _locationHandles = new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _moveTemplateHandles = new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _moveHandles = new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _moveTriggerHandles = new Dictionary<Table, DefinitionId>();
            private readonly Dictionary<Table, DefinitionId> _tacticHandles = new Dictionary<Table, DefinitionId>();

            private void ClearP1DHandles()
            {
                _audioHandles.Clear();
                _binaryHandles.Clear();
                _locationHandles.Clear();
                _moveTemplateHandles.Clear();
                _moveHandles.Clear();
                _moveTriggerHandles.Clear();
                _tacticHandles.Clear();
            }

            private void AddP1DModules(Table root)
            {
                Table assets = root.Get("assets").Table;
                assets.Set("audio", DynValue.NewCallback(AssetAudio));
                assets.Set("binary", DynValue.NewCallback(AssetBinary));

                var locales = new Table(_script);
                locales.Set("register", DynValue.NewCallback(RegisterLocaleMetadata));
                root.Set("locales", DynValue.NewTable(locales));

                var locations = new Table(_script);
                locations.Set("register", DynValue.NewCallback(RegisterLocation));
                locations.Set("name", DynValue.NewCallback(LocationName));
                locations.Set("select_dojo", DynValue.NewCallback(SelectDojo));
                locations.Set("reset_dojo", DynValue.NewCallback(ResetDojo));
                locations.Set("selected_dojo", DynValue.NewCallback(SelectedDojo));
                root.Set("locations", DynValue.NewTable(locations));

                var moves = new Table(_script);
                moves.Set("ANIMATION_END", DynValue.NewString("animation_end"));
                moves.Set("ANIMATION_START", DynValue.NewString("animation_start"));
                moves.Set("INTERVAL_END", DynValue.NewString("interval_end"));
                moves.Set("INTERVAL_START", DynValue.NewString("interval_start"));
                moves.Set("HIT", DynValue.NewString("hit"));
                moves.Set("STRIKE", DynValue.NewString("strike"));
                moves.Set("EVERY_FRAME", DynValue.NewString("every_frame"));
                moves.Set("BIRTH", DynValue.NewString("birth"));
                moves.Set("ROUND_STAGE_START", DynValue.NewString("round_stage_start"));
                moves.Set("MOD_EXPIRES", DynValue.NewString("mod_expires"));
                moves.Set("CURRENT_ANIMATION", DynValue.NewString("current_animation"));
                moves.Set("CURRENT_INTERVAL", DynValue.NewString("current_interval"));
                moves.Set("ITEM", DynValue.NewString("item"));
                moves.Set("PERK", DynValue.NewString("perk"));
                moves.Set("ALL", DynValue.NewString("all"));
                moves.Set("ANY", DynValue.NewString("any"));
                moves.Set("SOUND", DynValue.NewString("sound"));
                moves.Set("HIT_EFFECT", DynValue.NewString("hit_effect"));
                moves.Set("extend_item_lock", DynValue.NewCallback((context, args) =>
                {
                    const string function = "sf2.moves.extend_item_lock";
                    Table table = args.AsType(0,function,DataType.Table,false).Table;
                    return ApiCall(function, () =>
                    {
                        ValidateFields(table,function,"move","item_type","source_subtype","subtype");
                        _api.ExtendMoveItemLock(RequiredString(table,"move",function),RequiredString(table,"item_type",function),
                            RequiredString(table,"source_subtype",function),RequiredString(table,"subtype",function));
                        return DynValue.Nil;
                    });
                }));
                moves.Set("patch", DynValue.NewCallback((context, args) => ApiCall("sf2.moves.patch", () =>
                {
                    const string function = "sf2.moves.patch";
                    Table table = args.AsType(0, function, DataType.Table, false).Table;
                    ValidateFields(table, function, "move", "conditions", "interval_end", "hit", "sound_frame", "disable");
                    ModMoveFramePatch Frame(string key)
                    {
                        DynValue value = table.Get(key); if (value.IsNil()) return null;
                        if (value.Type != DataType.Table) throw new ModContentException(function + "." + key + " must be a table.");
                        ValidateFields(value.Table, function + "." + key, "name", "expected", "value");
                        return new ModMoveFramePatch(RequiredString(value.Table,"name",function),
                            RequiredInt(value.Table,"expected",function),RequiredInt(value.Table,"value",function));
                    }
                    ModMoveHitPatch hit = null; DynValue rawHit = table.Get("hit");
                    if (!rawHit.IsNil())
                    {
                        if (rawHit.Type != DataType.Table) throw new ModContentException(function + ".hit must be a table.");
                        ValidateFields(rawHit.Table,function + ".hit","expected","value");
                        hit = new ModMoveHitPatch(RequiredString(rawHit.Table,"expected",function),RequiredString(rawHit.Table,"value",function));
                    }
                    _api.PatchMove(RequiredString(table,"move",function),ReadMoveConditions(table.Get("conditions"),function + ".conditions"),
                        Frame("interval_end"), hit, Frame("sound_frame"), OptionalBool(table,"disable",false,function));
                    return DynValue.Nil;
                })));
                moves.Set("register_template", DynValue.NewCallback(RegisterMoveTemplate));
                moves.Set("register", DynValue.NewCallback(RegisterMove));
                moves.Set("register_trigger", DynValue.NewCallback(RegisterMoveTrigger));
                moves.Set("remove_perk_lock", DynValue.NewCallback(RemoveMovePerkLock));
                root.Set("moves", DynValue.NewTable(moves));

                var tactics = new Table(_script);
                tactics.Set("RANDOM", DynValue.NewString("random"));
                tactics.Set("TABULAR", DynValue.NewString("tabular"));
                tactics.Set("LINEAR", DynValue.NewString("linear"));
                tactics.Set("EXPONENTIAL", DynValue.NewString("exponential"));
                tactics.Set("register", DynValue.NewCallback(RegisterTactic));
                tactics.Set("name", DynValue.NewCallback(TacticName));
                root.Set("tactics", DynValue.NewTable(tactics));
            }

            private DynValue AssetAudio(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.assets.audio";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_audioHandles, _api.RequireAsset(reference, AssetKind.Audio)));
            }

            private DynValue AssetBinary(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.assets.binary";
                string reference = args.AsType(0, function, DataType.String, false).String;
                return ApiCall(function, () => NewHandle(_binaryHandles, _api.RequireAsset(reference, AssetKind.Binary)));
            }

            private DynValue RegisterLocaleMetadata(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locales.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "name", "locale", "alias", "file_icon",
                        "file_icon_selected", "loader_image", "preloader_image", "is_asian", "fonts");
                    LocaleFontDefinition fonts = ReadLocaleFonts(table.Get("fonts"), function + ".fonts");
                    LocaleMetadataDefinition value = _api.RegisterLocaleMetadata(
                        RequiredString(table, "id", function), RequiredString(table, "name", function),
                        RequiredString(table, "locale", function),
                        OptionalStringAllowEmpty(table, "alias", string.Empty, function),
                        OptionalStringAllowEmpty(table, "file_icon", string.Empty, function),
                        OptionalStringAllowEmpty(table, "file_icon_selected", string.Empty, function),
                        OptionalStringAllowEmpty(table, "loader_image", string.Empty, function),
                        OptionalStringAllowEmpty(table, "preloader_image", string.Empty, function),
                        OptionalBool(table, "is_asian", false, function), fonts);
                    return DynValue.NewString(value.Id.ToString());
                });
            }

            private LocaleFontDefinition ReadLocaleFonts(DynValue value, string function)
            {
                if (value.IsNil()) return null;
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be a table.");
                Table table = value.Table;
                ValidateFields(table, function, "content", "title", "button", "size_scale", "line_spacing",
                    "custom_line_spacing_scale");
                return new LocaleFontDefinition(RequiredString(table, "content", function),
                    RequiredString(table, "title", function), RequiredString(table, "button", function),
                    OptionalFloat(table, "size_scale", 1f, function),
                    OptionalFloat(table, "line_spacing", 1f, function),
                    OptionalFloat(table, "custom_line_spacing_scale", 1f, function));
            }

            private DynValue RegisterLocation(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locations.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "color", "wall", "floor", "position_y", "width",
                        "height", "min_width", "friction_force", "grid_size", "music", "music_choices", "dojo", "layers");
                    float width = OptionalFloat(table, "width", 1936f, function);
                    LocationDefinition value = _api.RegisterLocation(RequiredString(table, "id", function),
                        OptionalString(table, "color", "0x000000", function),
                        OptionalFloat(table, "wall", 200f, function), OptionalFloat(table, "floor", 80f, function),
                        OptionalFloat(table, "position_y", 0f, function), width,
                        OptionalFloat(table, "height", 512f, function), OptionalFloat(table, "min_width", width, function),
                        OptionalFloat(table, "friction_force", 0f, function), OptionalInt(table, "grid_size", 0, function),
                        OptionalHandle(table, "music", _audioHandles, "audio", function, default(AssetId)),
                        ReadLocationLayers(table.Get("layers"), function + ".layers"),
                        OptionalHandleArray(table, "music_choices", _audioHandles, "audio", function),
                        OptionalBool(table, "dojo", false, function));
                    return NewHandle(_locationHandles, value.Id);
                });
            }

            private DynValue LocationName(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locations.name";
                Table handle = args.AsType(0, function, DataType.Table, false).Table;
                DefinitionId id;
                if (!_locationHandles.TryGetValue(handle, out id))
                    throw new ScriptRuntimeException(function + " expects a location handle created by this mod context.");
                return DynValue.NewString(id.ToString());
            }

            private ModDojoSelection RequireDojo(string function)
            {
                _api.RequireCapability("presentation.dojo");
                if (_dojoSelection == null || !_dojoSelection.IsBound)
                    throw new ModContentException(function + " requires an active game profile.");
                return _dojoSelection;
            }

            private DynValue SelectDojo(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locations.select_dojo";
                return ApiCall(function, () => {
                    var selection = RequireDojo(function);
                    var handle = args.AsType(0, function, DataType.Table, false).Table;
                    if (!_locationHandles.TryGetValue(handle, out var id) || id.Namespace != Mod.Id)
                        throw new ModContentException(function + " requires this mod's registered location handle.");
                    selection.Select(id);
                    return DynValue.Nil;
                });
            }

            private DynValue ResetDojo(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locations.reset_dojo";
                return ApiCall(function, () => {
                    var selection = RequireDojo(function);
                    DefinitionId saved;
                    if (DefinitionId.TryParse(selection.SavedLocation, out saved) && saved.Namespace != Mod.Id)
                        throw new ModContentException(function + " cannot reset another mod's selected dojo.");
                    selection.Reset();
                    return DynValue.Nil;
                });
            }

            private DynValue SelectedDojo(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.locations.selected_dojo";
                return ApiCall(function, () => {
                    string saved = RequireDojo(function).SavedLocation;
                    return string.IsNullOrEmpty(saved) ? DynValue.Nil : DynValue.NewString(saved);
                });
            }

            private LocationLayerDefinition[] ReadLocationLayers(DynValue value, string function)
            {
                Table array = RequireArray(value, function);
                var result = new List<LocationLayerDefinition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    string where = function + "[" + i + "]";
                    Table layer = entry.Table;
                    ValidateFields(layer, where, "type", "factor", "scaling", "images", "fighters");
                    LocationFighterPositions fighters = null;
                    DynValue fighterValue = layer.Get("fighters");
                    if (!fighterValue.IsNil())
                    {
                        if (fighterValue.Type != DataType.Table) throw new ModContentException(where + ".fighters must be a table.");
                        Table positions = fighterValue.Table;
                        ValidateFields(positions, where + ".fighters", "player_x", "player_y", "enemy_x", "enemy_y");
                        foreach (string field in new[] { "player_x", "player_y", "enemy_x", "enemy_y" })
                            if (positions.Get(field).IsNil()) throw new ModContentException(where + ".fighters requires " + field + ".");
                        fighters = new LocationFighterPositions(OptionalFloat(positions, "player_x", 0, where),
                            OptionalFloat(positions, "player_y", 0, where), OptionalFloat(positions, "enemy_x", 0, where),
                            OptionalFloat(positions, "enemy_y", 0, where));
                    }
                    result.Add(new LocationLayerDefinition(OptionalInt(layer, "type", 1, where),
                        OptionalFloat(layer, "factor", 1f, where), OptionalBool(layer, "scaling", false, where),
                        layer.Get("images").IsNil() ? Array.Empty<LocationImageDefinition>() :
                            ReadLocationImages(layer.Get("images"), where + ".images"), fighters));
                }
                EnsureDenseArray(array, result.Count, function);
                if (result.Count == 0) throw new ModContentException(function + " must not be empty.");
                return result.ToArray();
            }

            private LocationImageDefinition[] ReadLocationImages(DynValue value, string function)
            {
                Table array = RequireArray(value, function);
                var result = new List<LocationImageDefinition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    string where = function + "[" + i + "]";
                    Table image = entry.Table;
                    ValidateFields(image, where, "sprite", "x", "y", "width", "height", "opaque", "flip_x",
                        "flip_y", "mask", "motion_x", "motion_y", "rotation", "opacity");
                    result.Add(new LocationImageDefinition(RequiredHandle(image, "sprite", _spriteHandles, "sprite", where),
                        OptionalFloat(image, "x", 0f, where), OptionalFloat(image, "y", 0f, where),
                        OptionalFloat(image, "width", 1f, where), OptionalFloat(image, "height", 1f, where),
                        OptionalBool(image, "opaque", false, where), OptionalBool(image, "flip_x", false, where),
                        OptionalBool(image, "flip_y", false, where), OptionalBool(image, "mask", false, where),
                        ReadLocationCurve(image.Get("motion_x"), where + ".motion_x"),
                        ReadLocationCurve(image.Get("motion_y"), where + ".motion_y"),
                        ReadLocationCurve(image.Get("rotation"), where + ".rotation"),
                        ReadLocationCurve(image.Get("opacity"), where + ".opacity")));
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private LocationCurveDefinition ReadLocationCurve(DynValue value, string function)
            {
                if (value.IsNil()) return null;
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be a curve table.");
                Table curve = value.Table; ValidateFields(curve, function, "offset", "points");
                Table points = RequireArray(curve.Get("points"), function + ".points");
                var result = new List<LocationCurvePoint>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = points.Get(i); if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table || i > 64) throw new ModContentException(function + " requires 2..64 point tables.");
                    Table point = entry.Table; string where = function + ".points[" + i + "]";
                    ValidateFields(point, where, "period", "value", "ease");
                    if (point.Get("period").IsNil() || point.Get("value").IsNil())
                        throw new ModContentException(where + " requires period and value.");
                    result.Add(new LocationCurvePoint(OptionalFloat(point, "period", 0, where),
                        OptionalFloat(point, "value", 0, where), OptionalFloat(point, "ease", 0, where)));
                }
                EnsureDenseArray(points, result.Count, function + ".points");
                return new LocationCurveDefinition(OptionalFloat(curve, "offset", 0, function), result.ToArray());
            }

            private DynValue RegisterMoveTemplate(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.moves.register_template";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateMoveNodeFields(table, function, false);
                    MoveTemplateDefinition value = _api.RegisterMoveTemplate(RequiredString(table, "id", function),
                        OptionalHandleArray(table, "templates", _moveTemplateHandles, "move-template", function),
                        OptionalStringArray(table, "core_templates", function), ReadMoveEvents(table.Get("events"), function + ".events"),
                        ReadMoveConditions(table.Get("conditions"), function + ".conditions"),
                        ReadMoveIntervals(table.Get("intervals"), function + ".intervals"),
                        OptionalStringAllowEmpty(table, "type", string.Empty, function), OptionalInt(table, "priority", 0, function),
                        OptionalInt(table, "mid_frames", 0, function), OptionalInt(table, "first_frame", 0, function),
                        OptionalInt(table, "end_frame", 0, function), OptionalStringAllowEmpty(table, "mirror_node", string.Empty, function),
                        OptionalStringAllowEmpty(table, "tactic_equivalent", string.Empty, function),
                        OptionalStringAllowEmpty(table, "tactic_weapon", string.Empty, function),
                        OptionalBool(table, "looped", false, function), OptionalBool(table, "ends_stage", false, function), ReadMoveGraph(table,function));
                    return NewHandle(_moveTemplateHandles, value.Id);
                });
            }

            private DynValue RegisterMove(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.moves.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateMoveNodeFields(table, function, true);
                    MoveDefinition value = _api.RegisterMove(RequiredString(table, "id", function),
                        RequiredHandle(table, "animation", _binaryHandles, "binary", function),
                        OptionalHandleArray(table, "templates", _moveTemplateHandles, "move-template", function),
                        OptionalStringArray(table, "core_templates", function), ReadMoveEvents(table.Get("events"), function + ".events"),
                        ReadMoveConditions(table.Get("conditions"), function + ".conditions"),
                        ReadMoveIntervals(table.Get("intervals"), function + ".intervals"),
                        OptionalStringAllowEmpty(table, "type", string.Empty, function), OptionalInt(table, "priority", 0, function),
                        OptionalInt(table, "mid_frames", 0, function), OptionalInt(table, "first_frame", 0, function),
                        OptionalInt(table, "end_frame", 0, function), OptionalStringAllowEmpty(table, "mirror_node", string.Empty, function),
                        OptionalStringAllowEmpty(table, "tactic_equivalent", string.Empty, function),
                        OptionalStringAllowEmpty(table, "tactic_weapon", string.Empty, function),
                        OptionalBool(table, "looped", false, function), OptionalBool(table, "ends_stage", false, function), ReadMoveGraph(table,function));
                    return NewHandle(_moveHandles, value.Id);
                });
            }

            private static void ValidateMoveNodeFields(Table table, string function, bool animation)
            {
                var fields = new List<string> { "id", "templates", "core_templates", "events", "conditions", "intervals",
                    "type", "priority", "mid_frames", "first_frame", "end_frame", "mirror_node", "tactic_equivalent",
                    "tactic_weapon", "looped", "ends_stage", "locks", "align", "direction" };
                if (animation) { fields.AddRange(new[] { "animation", "transitions", "actions", "profile", "tactic_distance", "no_wall_repulsion", "no_interpolation_frames", "no_magic_recharge", "velocity" }); }
                ValidateFields(table, function, fields.ToArray());
            }

            private ModMovePoint ReadMovePoint(DynValue value,string function)
            {
                if(value.Type!=DataType.Table) throw new ModContentException(function+" requires a point table.");
                var table=value.Table;
                ValidateFields(table,function,"object","player","part","shift_x","shift_y");
                return new ModMovePoint(RequiredString(table,"object",function),
                    table.Get("player").IsNil()?null:RequiredString(table,"player",function),
                    table.Get("part").IsNil()?null:RequiredString(table,"part",function),UiNumber(table,"shift_x"),UiNumber(table,"shift_y"));
            }

            private ModMoveGraph ReadMoveGraph(Table table,string function)
            {
                var transitions=new List<ModMoveTransition>();
                if(!table.Get("transitions").IsNil())
                {
                    var array=RequireArray(table.Get("transitions"),function+".transitions");
                    if(array.Length>32) throw new ModContentException("At most 32 move transitions are supported.");
                    for(int i=1;i<=array.Length;i++)
                    {
                        var value=array.Get(i);if(value.Type!=DataType.Table) throw new ModContentException("Transitions require tables.");
                        var entry=value.Table;ValidateFields(entry,function+".transitions","conditions","frame_shift","first_frame");
                        transitions.Add(new ModMoveTransition(ReadMoveConditions(entry.Get("conditions"),function+".transitions.conditions"),
                            entry.Get("frame_shift").IsNil()?(int?)null:RequiredInt(entry,"frame_shift",function),
                            entry.Get("first_frame").IsNil()?(int?)null:RequiredInt(entry,"first_frame",function)));
                    }
                    EnsureDenseArray(array,transitions.Count,function+".transitions");
                }
                ModMoveAlignment align=null;
                if(!table.Get("align").IsNil())
                {
                    var value=table.Get("align");if(value.Type!=DataType.Table) throw new ModContentException("Align requires a table.");
                    var entry=value.Table;ValidateFields(entry,function+".align","axes","pivot","position");
                    align=new ModMoveAlignment(OptionalStringArray(entry,"axes",function),ReadMovePoint(entry.Get("pivot"),function+".align.pivot"),
                        ReadMovePoint(entry.Get("position"),function+".align.position"));
                }
                ModMoveDirection direction=null;
                if(!table.Get("direction").IsNil())
                {
                    var value=table.Get("direction");if(value.Type!=DataType.Table) throw new ModContentException("Direction requires a table.");
                    var entry=value.Table;
                    if (!entry.Get("impulse").IsNil())
                    {
                        ValidateFields(entry,function+".direction","impulse");
                        if (entry.Get("impulse").Type != DataType.Table) throw new ModContentException("Direction impulse requires a table.");
                        var impulse=entry.Get("impulse").Table;
                        ValidateFields(impulse,function+".direction.impulse","reverse");
                        direction=new ModMoveDirection(OptionalBool(impulse,"reverse",false,function));
                    }
                    else
                    {
                        ValidateFields(entry,function+".direction","from","to");
                        direction=new ModMoveDirection(ReadMovePoint(entry.Get("from"),function+".direction.from"),ReadMovePoint(entry.Get("to"),function+".direction.to"));
                    }
                }
                return new ModMoveGraph(ReadMoveConditions(table.Get("locks"),function+".locks"),transitions.ToArray(),align,direction,ReadMovePresentation(table,function));
            }

            private ModMovePresentation ReadMovePresentation(Table table, string function)
            {
                var actions = new List<ModMoveScheduledAction>();
                if (!table.Get("actions").IsNil())
                {
                    var array = RequireArray(table.Get("actions"), function + ".actions");
                    if (array.Length > 64) throw new ModContentException("At most 64 scheduled move actions are supported.");
                    for (int i = 1; i <= array.Length; i++)
                    {
                        if (array.Get(i).Type != DataType.Table) throw new ModContentException("Move actions require tables.");
                        var entry = array.Get(i).Table;
                        string kind = RequiredString(entry, "type", function);
                        if (kind == "random_sound") ValidateFields(entry, function, "type", "frame", "event", "core_sounds");
                        else if (kind == "effect") ValidateFields(entry, function, "type", "frame", "event", "effect");
                        else if (kind == "stop_effect" || kind == "stop_follow_effect") ValidateFields(entry, function, "type", "frame", "event", "effect_name");
                        else if (kind == "create_projectile") ValidateFields(entry, function, "type", "frame", "event", "projectile");
                        else if (kind == "add_bullets") ValidateFields(entry, function, "type", "frame", "event", "bullets");
                        else if (kind == "delete_actor") ValidateFields(entry, function, "type", "frame", "event", "player");
                        else if (kind == "sound") ValidateFields(entry, function, "type", "frame", "event", "sound");
                        else if (kind == "shake_screen") ValidateFields(entry, function, "type", "frame", "event", "shake");
                        else ValidateFields(entry, function, "type", "frame", "event");
                        ModMoveSound sound = null;
                        if (kind == "sound")
                        {
                            if (entry.Get("sound").Type != DataType.Table) throw new ModContentException("Sound action requires a sound table.");
                            var spec = entry.Get("sound").Table;
                            ValidateFields(spec, function + ".sound", "core_sound", "voice");
                            sound = new ModMoveSound(RequiredString(spec, "core_sound", function),
                                spec.Get("voice").IsNil() ? null : RequiredString(spec, "voice", function));
                        }
                        ModMoveShake shake = null;
                        if (kind == "shake_screen")
                        {
                            if (entry.Get("shake").Type != DataType.Table) throw new ModContentException("Shake action requires a shake table.");
                            var spec = entry.Get("shake").Table;
                            ValidateFields(spec, function + ".shake", "pause_time", "effect_time", "amplitude_x", "amplitude_y", "frequency_x", "frequency_y");
                            shake = new ModMoveShake(OptionalInt(spec, "pause_time", 0, function), OptionalInt(spec, "effect_time", 0, function),
                                UiNumber(spec, "amplitude_x"), UiNumber(spec, "amplitude_y"), UiNumber(spec, "frequency_x"), UiNumber(spec, "frequency_y"));
                        }
                        ModMoveProjectile projectile = null;
                        if (kind == "create_projectile")
                        {
                            if (entry.Get("projectile").Type != DataType.Table) throw new ModContentException("Projectile action requires a projectile table.");
                            var spec = entry.Get("projectile").Table;
                            ValidateFields(spec, function + ".projectile", "name", "core_skeleton", "copy_parent_type", "core_start_animation", "start_move");
                            projectile = new ModMoveProjectile(RequiredString(spec, "name", function), RequiredString(spec, "core_skeleton", function),
                                RequiredString(spec, "copy_parent_type", function), spec.Get("core_start_animation").IsNil() ? null : RequiredString(spec, "core_start_animation", function),
                                spec.Get("start_move").IsNil() ? (DefinitionId?)null : RequiredHandle(spec, "start_move", _moveHandles, "move", function));
                        }
                        ModMoveBulletChange bullets = null;
                        if (kind == "add_bullets")
                        {
                            if (entry.Get("bullets").Type != DataType.Table) throw new ModContentException("Bullet action requires a bullets table.");
                            var spec = entry.Get("bullets").Table;
                            ValidateFields(spec, function + ".bullets", "type", "value");
                            bullets = new ModMoveBulletChange(RequiredString(spec, "type", function), RequiredInt(spec, "value", function));
                        }
                        ModMoveEffect effect = null;
                        if (kind == "effect")
                        {
                            if (entry.Get("effect").Type != DataType.Table) throw new ModContentException("Effect action requires an effect table.");
                            var spec = entry.Get("effect").Table;
                            ValidateFields(spec, function + ".effect", "name", "core_sequence", "scale", "time_scale", "looped", "position", "follow");
                            effect = new ModMoveEffect(RequiredString(spec, "name", function), RequiredString(spec, "core_sequence", function),
                                OptionalFloat(spec, "scale", 1, function), OptionalFloat(spec, "time_scale", 1, function),
                                OptionalBool(spec, "looped", false, function), spec.Get("position").IsNil() ? null : ReadMovePoint(spec.Get("position"), function + ".effect.position"),
                                OptionalBool(spec, "follow", false, function));
                        }
                        actions.Add(new ModMoveScheduledAction(kind,
                            entry.Get("frame").IsNil() ? (int?)null : RequiredInt(entry, "frame", function),
                            entry.Get("event").IsNil() ? null : RequiredString(entry, "event", function),
                            OptionalStringArray(entry, "core_sounds", function), effect,
                            kind == "stop_effect" || kind == "stop_follow_effect" ? RequiredString(entry, "effect_name", function) : null, projectile, bullets,
                            kind == "delete_actor" ? RequiredString(entry, "player", function) : null, sound, shake));
                    }
                    EnsureDenseArray(array, actions.Count, function + ".actions");
                }
                ModMoveProfile profile = null;
                if (!table.Get("profile").IsNil())
                {
                    if (table.Get("profile").Type != DataType.Table) throw new ModContentException("Profile requires a table.");
                    var entry = table.Get("profile").Table;
                    ValidateFields(entry, function, "rank", "core_icon", "display_name");
                    profile = new ModMoveProfile(RequiredInt(entry, "rank", function), RequiredString(entry, "core_icon", function),
                        entry.Get("display_name").IsNil() ? (DefinitionId?)null : RequiredHandle(entry, "display_name", _localizationHandles, "localization", function));
                }
                ModMoveTacticDistance distance = null;
                if (!table.Get("tactic_distance").IsNil())
                {
                    if (table.Get("tactic_distance").Type != DataType.Table) throw new ModContentException("Tactic distance requires a table.");
                    var entry = table.Get("tactic_distance").Table;
                    ValidateFields(entry, function, "axis", "minimum", "maximum", "from", "to");
                    distance = new ModMoveTacticDistance(RequiredString(entry, "axis", function),
                        OptionalFloat(entry, "minimum", -1000000, function), OptionalFloat(entry, "maximum", 1000000, function),
                        ReadMovePoint(entry.Get("from"), function), ReadMovePoint(entry.Get("to"), function));
                }
                ModMoveVelocity velocity = null;
                if (!table.Get("velocity").IsNil())
                {
                    if (table.Get("velocity").Type != DataType.Table) throw new ModContentException("Velocity requires a table.");
                    var entry = table.Get("velocity").Table;
                    ValidateFields(entry, function + ".velocity", "x", "y", "z", "ax", "ay", "az", "save_velocity");
                    velocity = new ModMoveVelocity(OptionalFloat(entry, "x", 0, function), OptionalFloat(entry, "y", 0, function), OptionalFloat(entry, "z", 0, function),
                        OptionalFloat(entry, "ax", 0, function), OptionalFloat(entry, "ay", 0, function), OptionalFloat(entry, "az", 0, function),
                        OptionalBool(entry, "save_velocity", false, function));
                }
                return new ModMovePresentation(actions.ToArray(), profile, distance,
                    OptionalBool(table, "no_wall_repulsion", false, function), OptionalBool(table, "no_interpolation_frames", false, function),
                    OptionalBool(table, "no_magic_recharge", false, function), velocity);
            }

            private DynValue RegisterMoveTrigger(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.moves.register_trigger";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "events", "conditions", "actions");
                    MoveTriggerDefinition value = _api.RegisterMoveTrigger(RequiredString(table, "id", function),
                        ReadMoveEvents(table.Get("events"), function + ".events"),
                        ReadMoveConditions(table.Get("conditions"), function + ".conditions"),
                        ReadMoveActions(table.Get("actions"), function + ".actions"));
                    return NewHandle(_moveTriggerHandles, value.Id);
                });
            }

            private ModMoveEvent[] ReadMoveEvents(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModMoveEvent>();
                Table array = RequireArray(value, function);
                var result = new List<ModMoveEvent>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type == DataType.String)
                    {
                        result.Add(new ModMoveEvent(ParseMoveEventKind(entry.String, function + "[" + i + "]")));
                        continue;
                    }
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be strings or tables.");
                    string where = function + "[" + i + "]";
                    Table item = entry.Table;
                    ValidateFields(item, where, "type", "name", "player");
                    result.Add(new ModMoveEvent(ParseMoveEventKind(RequiredString(item, "type", where), where),
                        OptionalStringAllowEmpty(item, "name", string.Empty, where),
                        OptionalStringAllowEmpty(item, "player", string.Empty, where)));
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private ModMoveCondition[] ReadMoveConditions(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModMoveCondition>();
                Table array = RequireArray(value, function);
                var result = new List<ModMoveCondition>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    result.Add(ReadMoveCondition(entry.Table, function + "[" + i + "]"));
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private ModMoveCondition ReadMoveCondition(Table table, string function)
            {
                string type = RequiredString(table, "type", function);
                ModMoveConditionKind kind = ParseMoveConditionKind(type, function);
                if (kind == ModMoveConditionKind.Distance)
                {
                    ValidateFields(table, function, "type", "axis", "from", "to", "minimum", "maximum", "not");
                    return new ModMoveCondition(kind, not: OptionalBool(table, "not", false, function),
                        distance: new ModMoveDistance(RequiredString(table, "axis", function), ReadMovePoint(table.Get("from"), function + ".from"),
                            ReadMovePoint(table.Get("to"), function + ".to"), OptionalFloat(table, "minimum", -1000000, function), OptionalFloat(table, "maximum", 1000000, function)));
                }
                if (kind == ModMoveConditionKind.ActorName || kind == ModMoveConditionKind.Bullets)
                {
                    if (kind == ModMoveConditionKind.ActorName) ValidateFields(table, function, "type", "name", "player", "not");
                    else ValidateFields(table, function, "type", "bullet_type", "minimum", "maximum", "player", "not");
                    return new ModMoveCondition(kind, kind == ModMoveConditionKind.ActorName ? RequiredString(table, "name", function) : null,
                        OptionalStringAllowEmpty(table, "player", string.Empty, function), not: OptionalBool(table, "not", false, function),
                        bullets: kind == ModMoveConditionKind.Bullets ? new ModMoveBulletRange(RequiredString(table, "bullet_type", function),
                            table.Get("minimum").IsNil() ? 0 : RequiredInt(table, "minimum", function),
                            table.Get("maximum").IsNil() ? int.MaxValue : RequiredInt(table, "maximum", function)) : null);
                }
                if (kind == ModMoveConditionKind.Character)
                {
                    ValidateFields(table,function,"type","warrior","not");
                    return new ModMoveCondition(kind,RequiredHandle(table,"warrior",_warriorHandles,"warrior",function).ToString(),not:OptionalBool(table,"not",false,function));
                }
                if (kind == ModMoveConditionKind.RoundStage || kind == ModMoveConditionKind.ModExists || kind == ModMoveConditionKind.Screen)
                {
                    ValidateFields(table, function, "type", "name", "player", "not");
                    return new ModMoveCondition(kind, RequiredString(table, "name", function),
                        OptionalStringAllowEmpty(table, "player", string.Empty, function), not: OptionalBool(table, "not", false, function));
                }
                if (kind == ModMoveConditionKind.Keys)
                {
                    ValidateFields(table,function,"type","keys","not");
                    var array=RequireArray(table.Get("keys"),function+".keys");
                    var keys=new List<ModMoveKey>();
                    for(int i=1;i<=array.Length;i++)
                    {
                        var entry=array.Get(i); if(entry.Type!=DataType.Table) throw new ModContentException("Move keys require tables.");
                        ValidateFields(entry.Table,function+".keys","key","press");
                        keys.Add(new ModMoveKey(RequiredString(entry.Table,"key",function),OptionalString(entry.Table,"press","Tap",function)));
                    }
                    EnsureDenseArray(array,keys.Count,function);
                    return new ModMoveCondition(kind,not:OptionalBool(table,"not",false,function),keys:keys.ToArray());
                }
                if (kind == ModMoveConditionKind.Perk)
                {
                    ValidateFields(table, function, "type", "perk", "player", "not");
                    return new ModMoveCondition(kind, RequiredHandle(table, "perk", _perkHandles, "perk", function).ToString(),
                        OptionalStringAllowEmpty(table, "player", string.Empty, function),
                        not: OptionalBool(table, "not", false, function));
                }
                if (kind == ModMoveConditionKind.All || kind == ModMoveConditionKind.Any)
                {
                    ValidateFields(table, function, "type", "not", "conditions");
                    return new ModMoveCondition(kind, not: OptionalBool(table, "not", false, function),
                        children: ReadMoveConditions(table.Get("conditions"), function + ".conditions"));
                }
                ValidateFields(table, function, "type", "name", "player", "item_type", "item_subtype", "not");
                return new ModMoveCondition(kind, OptionalStringAllowEmpty(table, "name", string.Empty, function),
                    OptionalStringAllowEmpty(table, "player", string.Empty, function),
                    OptionalStringAllowEmpty(table, "item_type", string.Empty, function),
                    OptionalStringAllowEmpty(table, "item_subtype", string.Empty, function),
                    OptionalBool(table, "not", false, function));
            }

            private ModMoveInterval[] ReadMoveIntervals(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModMoveInterval>();
                Table array = RequireArray(value, function);
                var result = new List<ModMoveInterval>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    string where = function + "[" + i + "]";
                    ValidateFields(entry.Table, where, "type", "name", "start", "end", "attack");
                    result.Add(new ModMoveInterval(OptionalStringAllowEmpty(entry.Table, "type", string.Empty, where),
                        OptionalStringAllowEmpty(entry.Table, "name", string.Empty, where),
                        entry.Table.Get("start").IsNil() ? (int?)null : RequiredInt(entry.Table,"start",where),
                        entry.Table.Get("end").IsNil() ? (int?)null : RequiredInt(entry.Table,"end",where),
                        ReadMoveAttack(entry.Table.Get("attack"),where+".attack")));
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private ModMoveAttack ReadMoveAttack(DynValue value,string function)
            {
                if(value.IsNil()) return null;
                if(value.Type!=DataType.Table) throw new ModContentException(function+" must be a table.");
                var table=value.Table;
                ValidateFields(table,function,"direct","edges","damage","damage_type","damage_terms","hit","hit_move","id","impulse","options");
                ModMoveDamageTerm[] terms = null;
                if (!table.Get("damage_terms").IsNil())
                {
                    if (!table.Get("damage_type").IsNil()) throw new ModContentException("Use damage_type or damage_terms, not both.");
                    var array = RequireArray(table.Get("damage_terms"), function + ".damage_terms");
                    if (array.Length < 1 || array.Length > 4) throw new ModContentException("damage_terms requires 1..4 terms.");
                    terms = new ModMoveDamageTerm[array.Length];
                    for (int i = 1; i <= array.Length; i++)
                    {
                        var entry = array.Get(i);
                        if (entry.Type != DataType.Table) throw new ModContentException("Damage terms require tables.");
                        ValidateFields(entry.Table, function + ".damage_terms", "type", "shift");
                        terms[i-1] = new ModMoveDamageTerm(RequiredString(entry.Table, "type", function), UiNumber(entry.Table, "shift"));
                    }
                    EnsureDenseArray(array, terms.Length, function + ".damage_terms");
                }
                double x=0,y=0,z=0;
                var impulse=table.Get("impulse");
                if(!impulse.IsNil())
                {
                    if(impulse.Type!=DataType.Table) throw new ModContentException("Attack impulse must be a table.");
                    ValidateFields(impulse.Table,function+".impulse","x","y","z");
                    x=UiNumber(impulse.Table,"x"); y=UiNumber(impulse.Table,"y"); z=UiNumber(impulse.Table,"z");
                }
                ModMoveAttackOptions options = null;
                if (!table.Get("options").IsNil())
                {
                    if (table.Get("options").Type != DataType.Table) throw new ModContentException("Attack options require a table.");
                    var spec = table.Get("options").Table;
                    ValidateFields(spec, function + ".options", "no_effect", "no_critical", "ignores_block", "body_part", "defense_types", "ignores_invulnerable", "ignores_all_invulnerable");
                    options = new ModMoveAttackOptions(OptionalBool(spec, "no_effect", false, function), OptionalBool(spec, "no_critical", false, function),
                        OptionalBool(spec, "ignores_block", false, function), spec.Get("body_part").IsNil() ? null : RequiredString(spec, "body_part", function),
                        OptionalStringArray(spec, "defense_types", function), OptionalStringArray(spec, "ignores_invulnerable", function),
                        OptionalBool(spec, "ignores_all_invulnerable", false, function));
                }
                return new ModMoveAttack(OptionalStringArray(table,"edges",function),UiNumber(table,"damage"),
                    table.Get("damage_type").IsNil() ? null : RequiredString(table,"damage_type",function),
                    table.Get("hit").IsNil() ? null : RequiredString(table,"hit",function),OptionalInt(table,"id",0,function),x,y,z,terms,options,OptionalBool(table,"direct",false,function),
                    table.Get("hit_move").IsNil() ? (DefinitionId?)null : RequiredHandle(table,"hit_move",_moveHandles,"move",function));
            }

            private ModMoveAction[] ReadMoveActions(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModMoveAction>();
                Table array = RequireArray(value, function);
                var result = new List<ModMoveAction>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    string where = function + "[" + i + "]";
                    Table action = entry.Table;
                    string type = RequiredString(action, "type", where);
                    if (type == "sound")
                    {
                        ValidateFields(action, where, "type", "audio", "volume", "looped");
                        result.Add(ModMoveAction.Sound(RequiredHandle(action, "audio", _audioHandles, "audio", where),
                            OptionalFloat(action, "volume", 1f, where), OptionalBool(action, "looped", false, where)));
                    }
                    else if (type == "hit_effect")
                    {
                        ValidateFields(action, where, "type", "name");
                        result.Add(ModMoveAction.HitEffect(RequiredString(action, "name", where)));
                    }
                    else throw new ModContentException(where + " has unsupported action type '" + type + "'.");
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private DynValue RegisterTactic(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.tactics.register";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "id", "type", "template", "memory", "counter_attack", "dodge",
                        "block", "safe_attack", "table_attack", "cautious_movement", "dodge_missiles", "dodge_magic",
                        "animation_weights", "quick_attacks", "evades", "expected_wait", "on_decide");
                    var onDecide = table.Get("on_decide");
                    if (!onDecide.IsNil())
                    {
                        if (onDecide.Type != DataType.Function) throw new ModContentException("Tactic on_decide must be a Lua function.");
                        if (OptionalString(table,"type","tabular",function) != "tabular") throw new ModContentException("Programmable AI requires the tabular input controller.");
                    }
                    int memoryStrikes = 0;
                    float memoryRoundFactor = 0f;
                    DynValue memoryValue = table.Get("memory");
                    if (!memoryValue.IsNil())
                    {
                        if (memoryValue.Type != DataType.Table) throw new ModContentException(function + ".memory must be a table.");
                        ValidateFields(memoryValue.Table, function + ".memory", "strikes", "round_factor");
                        memoryStrikes = OptionalInt(memoryValue.Table, "strikes", 0, function + ".memory");
                        memoryRoundFactor = OptionalFloat(memoryValue.Table, "round_factor", 0f, function + ".memory");
                    }
                    TacticDefinition value = _api.RegisterTactic(RequiredString(table, "id", function),
                        ParseTacticKind(OptionalString(table, "type", "tabular", function), function),
                        OptionalStringAllowEmpty(table, "template", string.Empty, function), memoryStrikes, memoryRoundFactor,
                        ReadTacticValue(table.Get("counter_attack"), function + ".counter_attack"),
                        ReadTacticValue(table.Get("dodge"), function + ".dodge"),
                        ReadTacticValue(table.Get("block"), function + ".block"),
                        ReadTacticValue(table.Get("safe_attack"), function + ".safe_attack"),
                        ReadTacticValue(table.Get("table_attack"), function + ".table_attack"),
                        ReadTacticValue(table.Get("cautious_movement"), function + ".cautious_movement"),
                        ReadTacticValue(table.Get("dodge_missiles"), function + ".dodge_missiles"),
                        ReadTacticValue(table.Get("dodge_magic"), function + ".dodge_magic"),
                        ReadTacticAnimationValues(table.Get("animation_weights"), function + ".animation_weights"),
                        ReadTacticAnimationValues(table.Get("quick_attacks"), function + ".quick_attacks"),
                        ReadTacticAnimationValues(table.Get("evades"), function + ".evades"),
                        ReadTacticAnimationValues(table.Get("expected_wait"), function + ".expected_wait"));
                    if (!onDecide.IsNil()) _aiHandlers.Add(value.Id.ToString(), onDecide);
                    return NewHandle(_tacticHandles, value.Id);
                });
            }

            private sealed class AiMemory
            {
                public Table State;
                public bool Failed;
            }
            private readonly Dictionary<string, DynValue> _aiHandlers = new Dictionary<string, DynValue>(StringComparer.Ordinal);
            private System.Runtime.CompilerServices.ConditionalWeakTable<object, Dictionary<string, AiMemory>> _aiInstances =
                new System.Runtime.CompilerServices.ConditionalWeakTable<object, Dictionary<string, AiMemory>>();

            public bool HasAiHandler(string tactic) => !_disposed && tactic != null && _aiHandlers.ContainsKey(tactic);

            public bool TryDecideAi(string tactic, object instance, ModCombatSnapshot snapshot, IReadOnlyList<string> actions,
                out int? selection, out string error)
            {
                // Compatibility for hosts that supply only candidate names.
                if (actions == null || actions.Count > 1024)
                { selection = null; error = "Invalid AI decision snapshot."; return false; }
                var candidates = new ModAiActionSnapshot[actions.Count];
                for (int i = 0; i < actions.Count; i++)
                {
                    if (actions[i] == null)
                    { selection = null; error = "Invalid AI action name."; return false; }
                    candidates[i] = new ModAiActionSnapshot(actions[i]);
                }
                return TryDecideAi(tactic, instance, snapshot, candidates, out selection, out error);
            }

            private DynValue RemoveMovePerkLock(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.moves.remove_perk_lock";
                Table table = args.AsType(0, function, DataType.Table, false).Table;
                return ApiCall(function, () =>
                {
                    ValidateFields(table, function, "move", "perk");
                    DefinitionId perk = RequiredHandle(table, "perk", _perkHandles, "perk", function);
                    _api.RemoveMovePerkLock(RequiredString(table, "move", function), perk);
                    return DynValue.Nil;
                });
            }

            public bool TryDecideAi(string tactic, object instance, ModCombatSnapshot snapshot, IReadOnlyList<ModAiActionSnapshot> actions,
                out int? selection, out string error)
            {
                selection = null; error = null;
                AiMemory memory = null;
                try
                {
                    ThrowIfDisposed();
                    if (!HasAiHandler(tactic)) return true;
                    if (instance == null || snapshot == null || actions == null || actions.Count > 1024)
                        throw new ModContentException("Invalid AI decision snapshot.");
                    var instances = _aiInstances.GetOrCreateValue(instance);
                    if (!instances.TryGetValue(tactic, out memory)) instances.Add(tactic, memory = new AiMemory { State = new Table(_script) });
                    if (memory.Failed) return true;
                    var eventTable = new Table(_script);
                    eventTable.Set("self", FighterSnapshotTable(snapshot.Self));
                    eventTable.Set("opponent", FighterSnapshotTable(snapshot.Opponent));
                    eventTable.Set("frame", DynValue.NewNumber(snapshot.Frame));
                    eventTable.Set("seconds", DynValue.NewNumber(snapshot.Seconds));
                    var list = new Table(_script);
                    var choices = new Dictionary<Table, int>();
                    for (int i = 0; i < actions.Count; i++)
                    {
                        var candidate = actions[i] ?? throw new ModContentException("Invalid AI action snapshot.");
                        var action = new Table(_script);
                        action.Set("name", DynValue.NewString(candidate.Name));
                        action.Set("type", DynValue.NewString(candidate.Type));
                        action.Set("priority", DynValue.NewNumber(candidate.Priority));
                        if (candidate.Timing != null)
                        {
                            var timing = new Table(_script);
                            timing.Set("first_sample", DynValue.NewNumber(candidate.Timing.FirstSample));
                            timing.Set("last_sample", DynValue.NewNumber(candidate.Timing.LastSample));
                            timing.Set("mid_frames", DynValue.NewNumber(candidate.Timing.MidFrames));
                            timing.Set("nominal_frames", DynValue.NewNumber(candidate.Timing.NominalFrames));
                            timing.Set("nominal_seconds", DynValue.NewNumber(candidate.Timing.NominalSeconds));
                            timing.Set("looped", DynValue.NewBoolean(candidate.Timing.Looped));
                            action.Set("timing", DynValue.NewTable(timing));
                        }
                        var inputs = new Table(_script);
                        for (int inputIndex = 0; inputIndex < candidate.Inputs.Count; inputIndex++)
                        {
                            var input = new Table(_script);
                            input.Set("control", DynValue.NewString(candidate.Inputs[inputIndex].Control));
                            input.Set("press", DynValue.NewString(candidate.Inputs[inputIndex].Press));
                            inputs.Set(inputIndex + 1, DynValue.NewTable(input));
                        }
                        action.Set("inputs", DynValue.NewTable(inputs));
                        choices.Add(action, i); list.Set(i + 1, DynValue.NewTable(action));
                    }
                    eventTable.Set("actions", DynValue.NewTable(list));
                    var result = RunBounded(_aiHandlers[tactic], tactic + ":on_decide", MaxBehaviorInstructionSlices,
                        new[] { DynValue.NewTable(memory.State), DynValue.NewTable(eventTable) });
                    if (result.IsNil()) return true;
                    if (result.Type == DataType.String && result.String == "wait") { selection = -1; return true; }
                    if (result.Type == DataType.Table && choices.TryGetValue(result.Table, out int index)) { selection = index; return true; }
                    throw new ModContentException("AI on_decide must return nil, 'wait', or an action from this decision's snapshot.");
                }
                catch (Exception exception)
                {
                    if (memory != null) memory.Failed = true;
                    error = exception.Message; return false;
                }
            }

            private DynValue TacticName(ScriptExecutionContext context, CallbackArguments args)
            {
                const string function = "sf2.tactics.name";
                DynValue value = args[0];
                DefinitionId id;
                if (value.Type != DataType.Table || !_tacticHandles.TryGetValue(value.Table, out id))
                    throw new ModContentException(function + " requires a tactic handle.");
                return DynValue.NewString(id.ToString());
            }

            private ModTacticValue ReadTacticValue(DynValue value, string function)
            {
                if (value.IsNil()) return null;
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be a table.");
                Table table = value.Table;
                ValidateFields(table, function, "base", "counter_factor", "damage_factor", "health_factor",
                    "enemy_health_factor", "animation_frames_factor", "child_frames_factor", "magic_bullet_factor",
                    "missile_bullet_factor", "hit_factor", "distance_factor", "shift", "limit", "anti_limit", "factor_type");
                return new ModTacticValue(OptionalFloat(table, "base", 0f, function),
                    OptionalFloat(table, "counter_factor", 0f, function), OptionalFloat(table, "damage_factor", 0f, function),
                    OptionalFloat(table, "health_factor", 0f, function), OptionalFloat(table, "enemy_health_factor", 0f, function),
                    OptionalFloat(table, "animation_frames_factor", 0f, function), OptionalFloat(table, "child_frames_factor", 0f, function),
                    OptionalFloat(table, "magic_bullet_factor", 0f, function), OptionalFloat(table, "missile_bullet_factor", 0f, function),
                    OptionalFloat(table, "hit_factor", 0f, function), OptionalFloat(table, "distance_factor", 0f, function),
                    OptionalFloat(table, "shift", 0f, function), OptionalFloat(table, "limit", 0f, function),
                    OptionalFloat(table, "anti_limit", 0f, function),
                    ParseTacticFactorType(OptionalString(table, "factor_type", "linear", function), function));
            }

            private ModTacticAnimationValue[] ReadTacticAnimationValues(DynValue value, string function)
            {
                if (value.IsNil()) return Array.Empty<ModTacticAnimationValue>();
                Table array = RequireArray(value, function);
                var result = new List<ModTacticAnimationValue>();
                for (int i = 1; ; i++)
                {
                    DynValue entry = array.Get(i);
                    if (entry.IsNil()) break;
                    if (entry.Type != DataType.Table) throw new ModContentException(function + " entries must be tables.");
                    string where = function + "[" + i + "]";
                    Table item = entry.Table;
                    ValidateFields(item, where, "move", "animation", "value");
                    DefinitionId move = OptionalHandle(item, "move", _moveHandles, "move", where, default(DefinitionId));
                    string animation = OptionalStringAllowEmpty(item, "animation", string.Empty, where);
                    DynValue tacticValue = item.Get("value");
                    result.Add(new ModTacticAnimationValue(move, animation,
                        tacticValue.IsNil() ? new ModTacticValue() : ReadTacticValue(tacticValue, where + ".value")));
                }
                EnsureDenseArray(array, result.Count, function);
                return result.ToArray();
            }

            private static Table RequireArray(DynValue value, string function)
            {
                if (value.Type != DataType.Table) throw new ModContentException(function + " must be an array table.");
                return value.Table;
            }

            private static ModMoveEventKind ParseMoveEventKind(string value, string function)
            {
                switch (value)
                {
                    case "animation_end": return ModMoveEventKind.AnimationEnd;
                    case "animation_start": return ModMoveEventKind.AnimationStart;
                    case "interval_end": return ModMoveEventKind.IntervalEnd;
                    case "interval_start": return ModMoveEventKind.IntervalStart;
                    case "hit": return ModMoveEventKind.Hit;
                    case "strike": return ModMoveEventKind.Strike;
                    case "every_frame": return ModMoveEventKind.EveryFrame;
                    case "birth": return ModMoveEventKind.Birth;
                    case "round_stage_start": return ModMoveEventKind.RoundStageStart;
                    case "mod_expires": return ModMoveEventKind.ModExpires;
                    case "key_pressed": return ModMoveEventKind.KeyPressed;
                    default: throw new ModContentException(function + " has unsupported event type '" + value + "'.");
                }
            }

            private static ModMoveConditionKind ParseMoveConditionKind(string value, string function)
            {
                switch (value)
                {
                    case "perk": return ModMoveConditionKind.Perk;
                    case "keys": return ModMoveConditionKind.Keys;
                    case "round_stage": return ModMoveConditionKind.RoundStage;
                    case "mod_exists": return ModMoveConditionKind.ModExists;
                    case "screen": return ModMoveConditionKind.Screen;
                    case "character": return ModMoveConditionKind.Character;
                    case "distance": return ModMoveConditionKind.Distance;
                    case "actor_name": return ModMoveConditionKind.ActorName;
                    case "bullets": return ModMoveConditionKind.Bullets;
                    case "current_animation": return ModMoveConditionKind.CurrentAnimation;
                    case "current_interval": return ModMoveConditionKind.CurrentInterval;
                    case "item": return ModMoveConditionKind.Item;
                    case "all": return ModMoveConditionKind.All;
                    case "any": return ModMoveConditionKind.Any;
                    default: throw new ModContentException(function + " has unsupported condition type '" + value + "'.");
                }
            }

            private static ModTacticKind ParseTacticKind(string value, string function)
            {
                if (value == "random") return ModTacticKind.Random;
                if (value == "tabular") return ModTacticKind.Tabular;
                throw new ModContentException(function + " field 'type' must be 'random' or 'tabular'.");
            }

            private static ModTacticFactorType ParseTacticFactorType(string value, string function)
            {
                if (value == "linear") return ModTacticFactorType.Linear;
                if (value == "exponential") return ModTacticFactorType.Exponential;
                throw new ModContentException(function + " field 'factor_type' must be 'linear' or 'exponential'.");
            }
        }
    }
}
