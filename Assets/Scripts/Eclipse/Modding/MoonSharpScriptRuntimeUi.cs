using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            private int _uiCloseDepth;
            private System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModUiSurface> _uiHandles =
                new System.Runtime.CompilerServices.ConditionalWeakTable<Table, ModUiSurface>();

            private void AddUiModule(Table root)
            {
                var ui = new Table(_script);
                ui.Set("open", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.open", () => OpenUi(args))));
                ui.Set("close", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.close", () => {
                    UiHandle(args, "sf2.ui.close").Close(); return DynValue.Nil;
                })));
                ui.Set("is_open", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.is_open", () =>
                    DynValue.NewBoolean(!UiHandle(args, "sf2.ui.is_open").IsClosed))));
                ui.Set("set_text", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_text", () => {
                    UiHandle(args, "sf2.ui.set_text").SetText(UiString(args, 1, "sf2.ui.set_text"), UiString(args, 2, "sf2.ui.set_text"));
                    return DynValue.Nil;
                })));
                ui.Set("set_value", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_value", () => {
                    UiHandle(args, "sf2.ui.set_value").SetValue(UiString(args, 1, "sf2.ui.set_value"), UiArgument(args,2,DataType.Number,"sf2.ui.set_value").Number);
                    return DynValue.Nil;
                })));
                ui.Set("set_sprite", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_sprite", () => {
                    var surface = UiHandle(args, "sf2.ui.set_sprite");
                    var handle = UiArgument(args, 2, DataType.Table, "sf2.ui.set_sprite").Table;
                    if (!_spriteHandles.TryGetValue(handle, out var sprite))
                        throw new ModContentException("sf2.ui.set_sprite requires a sprite handle created by this script context.");
                    surface.SetSprite(UiString(args, 1, "sf2.ui.set_sprite"), sprite);
                    return DynValue.Nil;
                })));
                ui.Set("set_checked", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_checked", () => {
                    UiHandle(args, "sf2.ui.set_checked").SetChecked(UiString(args, 1, "sf2.ui.set_checked"), UiArgument(args,2,DataType.Boolean,"sf2.ui.set_checked").Boolean);
                    return DynValue.Nil;
                })));
                ui.Set("set_visible", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_visible", () => {
                    UiHandle(args, "sf2.ui.set_visible").SetVisible(UiString(args, 1, "sf2.ui.set_visible"), UiArgument(args,2,DataType.Boolean,"sf2.ui.set_visible").Boolean);
                    return DynValue.Nil;
                })));
                ui.Set("set_enabled", DynValue.NewCallback((ctx, args) => ApiCall("sf2.ui.set_enabled", () => {
                    UiHandle(args, "sf2.ui.set_enabled").SetEnabled(UiString(args, 1, "sf2.ui.set_enabled"), UiArgument(args,2,DataType.Boolean,"sf2.ui.set_enabled").Boolean);
                    return DynValue.Nil;
                })));
                root.Set("ui", DynValue.NewTable(ui));
            }

            private static string UiString(CallbackArguments args, int index, string function) =>
                UiArgument(args,index,DataType.String,function).String;

            private static DynValue UiArgument(CallbackArguments args, int index, DataType type, string function)
            {
                var value = args[index];
                if (value.Type != type) throw new ModContentException(function + " argument " + (index + 1) + " must be " + type + ".");
                return value;
            }

            private ModUiSurface UiHandle(CallbackArguments args, string function)
            {
                ThrowIfDisposed();
                var table = args.AsType(0, function, DataType.Table, false).Table;
                if (!_uiHandles.TryGetValue(table, out var surface))
                    throw new ModContentException(function + " requires a UI handle created by this script context.");
                return surface;
            }

            private DynValue OpenUi(CallbackArguments args)
            {
                ThrowIfDisposed(); _api.RequireCapability("ui.create");
                if (_uiCloseDepth != 0) throw new ModContentException("UI cannot be opened from on_close; finish cleanup before opening another surface.");
                if (_mountUi == null) throw new ModContentException("Custom UI rendering is unavailable in this host.");
                const string function = "sf2.ui.open";
                var table = args.AsType(0, function, DataType.Table, false).Table;
                ValidateFields(table, function, "id", "mount", "root", "on_click", "on_close", "on_change", "placement");
                string id = RequiredString(table, "id", function);
                ModUiMount mount;
                switch (RequiredString(table, "mount", function))
                {
                    case "menu": mount = ModUiMount.Menu; break;
                    case "modal": mount = ModUiMount.Modal; break;
                    case "hud": mount = ModUiMount.CombatHud; break;
                    default: throw new ModContentException("UI mount must be menu, modal or hud.");
                }
                var callback = table.Get("on_click");
                if (!callback.IsNil() && callback.Type != DataType.Function)
                    throw new ModContentException("UI on_click must be a Lua function.");
                var onClose = table.Get("on_close");
                if (!onClose.IsNil() && onClose.Type != DataType.Function)
                    throw new ModContentException("UI on_close must be a Lua function.");
                var onChange = table.Get("on_change");
                if (!onChange.IsNil() && onChange.Type != DataType.Function)
                    throw new ModContentException("UI on_change must be a Lua function.");
                int count = 0;
                var node = ReadUiNode(table.Get("root"), 1, ref count);
                ModUiPlacement placement = null;
                var placementValue = table.Get("placement");
                if (!placementValue.IsNil())
                {
                    if (placementValue.Type != DataType.Table) throw new ModContentException("UI placement must be a table.");
                    ValidateFields(placementValue.Table, "UI placement", "anchor", "x", "y");
                    placement = new ModUiPlacement(OptionalString(placementValue.Table,"anchor","center","UI placement"),
                        UiNumber(placementValue.Table,"x"),UiNumber(placementValue.Table,"y"));
                }
                var handle = DynValue.NewTable(new Table(_script));
                bool ready = false;
                var surface = UiScope.Open(id, mount, node, callback.IsNil() ? (Action<string>)null : widget => {
                    ThrowIfDisposed();
                    if (!ready) throw new ModContentException("UI input arrived before mounting completed.");
                    RunBounded(callback, Mod.Id + ":ui/" + id + ":on_click", MaxBehaviorInstructionSlices,
                        new[] { handle, DynValue.NewString(widget) });
                }, placement, reason => {
                    // A failed mount or dead script has no live Lua view to notify.
                    if (!ready || _disposed || reason == ModUiCloseReason.Shutdown || onClose.IsNil()) return;
                    _uiCloseDepth++;
                    try
                    {
                        RunBounded(onClose, Mod.Id + ":ui/" + id + ":on_close", MaxBehaviorInstructionSlices,
                            new[] { handle, DynValue.NewString(reason.ToString().ToLowerInvariant()) });
                    }
                    finally { _uiCloseDepth--; }
                }, onChange.IsNil() ? (Action<string, double>)null : (widget, number) => {
                    ThrowIfDisposed();
                    if (!ready) throw new ModContentException("UI input arrived before mounting completed.");
                    var definition = FindUiNode(node, widget);
                    RunBounded(onChange, Mod.Id + ":ui/" + id + ":on_change", MaxBehaviorInstructionSlices,
                        new[] { handle, DynValue.NewString(widget), definition.Kind == ModUiKind.Toggle ? DynValue.NewBoolean(number != 0) : DynValue.NewNumber(number) });
                });
                try
                {
                    _uiHandles.Add(handle.Table, surface);
                    _mountUi(surface);
                    if (surface.IsClosed) throw new ModContentException("UI closed while mounting.");
                    ready = true;
                    return handle;
                }
                catch { surface.Close(); throw; }
            }

            private ModUiNode ReadUiNode(DynValue value, int depth, ref int count)
            {
                if (depth > 16 || ++count > 256) throw new ModContentException("UI trees permit 256 nodes and depth 16.");
                if (value.Type != DataType.Table) throw new ModContentException("UI nodes must be tables.");
                const string function = "UI node";
                var node = value.Table;
                ValidateFields(node, function, "id", "kind", "width", "height", "gap", "text", "value", "checked", "visible", "enabled", "children", "style", "sprite", "columns", "cell_width", "cell_height");
                ModUiKind kind;
                switch (RequiredString(node, "kind", function))
                {
                    case "stack": kind = ModUiKind.Stack; break;
                    case "row": kind = ModUiKind.Row; break;
                    case "column": kind = ModUiKind.Column; break;
                    case "scroll": kind = ModUiKind.Scroll; break;
                    case "text": kind = ModUiKind.Text; break;
                    case "button": kind = ModUiKind.Button; break;
                    case "progress": kind = ModUiKind.Progress; break;
                    case "toggle": kind = ModUiKind.Toggle; break;
                    case "slider": kind = ModUiKind.Slider; break;
                    case "image": kind = ModUiKind.Image; break;
                    case "grid": kind = ModUiKind.Grid; break;
                    default: throw new ModContentException("Unsupported UI widget kind.");
                }
                if (kind != ModUiKind.Toggle && !node.Get("checked").IsNil()) throw new ModContentException("Only toggles accept checked.");
                if (kind == ModUiKind.Toggle && !node.Get("value").IsNil()) throw new ModContentException("Use checked for a toggle.");
                if (kind != ModUiKind.Image && !node.Get("sprite").IsNil()) throw new ModContentException("Only image widgets accept a sprite.");
                AssetId? sprite = kind == ModUiKind.Image ? RequiredHandle(node,"sprite",_spriteHandles,"sprite",function) : (AssetId?)null;
                if (kind != ModUiKind.Grid && (!node.Get("columns").IsNil() || !node.Get("cell_width").IsNil() || !node.Get("cell_height").IsNil()))
                    throw new ModContentException("Only grids accept columns and cell dimensions.");
                double columns = UiNumber(node,"columns");
                if (columns != Math.Truncate(columns) || columns < 0 || columns > 256)
                    throw new ModContentException("Grid columns must be an integer from 1 to 256.");
                var children = new List<ModUiNode>();
                var source = node.Get("children");
                if (!source.IsNil())
                {
                    if (source.Type != DataType.Table) throw new ModContentException("UI children must be a dense array.");
                    int length = source.Table.Length, pairs = 0;
                    if (length > 256) throw new ModContentException("UI children exceed the node limit.");
                    foreach (var pair in source.Table.Pairs)
                    {
                        pairs++;
                        if (pair.Key.Type != DataType.Number || pair.Key.Number < 1 || pair.Key.Number > length || pair.Key.Number != Math.Truncate(pair.Key.Number))
                            throw new ModContentException("UI children must be a dense array.");
                    }
                    if (pairs != length) throw new ModContentException("UI children must be a dense array.");
                    for (int i = 1; i <= length; i++) children.Add(ReadUiNode(source.Table.Get(i), depth + 1, ref count));
                }
                return new ModUiNode(RequiredString(node,"id",function), kind,
                    UiNumber(node,"width"), UiNumber(node,"height"), OptionalStringAllowEmpty(node,"text","",function),
                    kind == ModUiKind.Toggle ? (OptionalBool(node,"checked",false,function) ? 1 : 0) : UiNumber(node,"value"), OptionalBool(node,"visible",true,function), OptionalBool(node,"enabled",true,function),
                    UiNumber(node,"gap"), children, ReadUiStyle(node.Get("style")), sprite,
                    (int)columns, UiNumber(node,"cell_width"), UiNumber(node,"cell_height"));
            }

            private static ModUiNode FindUiNode(ModUiNode node, string id)
            {
                if (node.Id == id) return node;
                foreach (var child in node.Children) { var found = FindUiNode(child, id); if (found != null) return found; }
                return null;
            }

            private ModUiStyle ReadUiStyle(DynValue value)
            {
                if (value.IsNil()) return null;
                const string function = "UI style";
                if (value.Type != DataType.Table) throw new ModContentException("UI style must be a table.");
                var table = value.Table;
                ValidateFields(table, function, "text_color", "background_color", "fill_color", "font_size", "text_align");
                int? size = null;
                if (!table.Get("font_size").IsNil())
                {
                    double number = UiNumber(table,"font_size");
                    if (double.IsNaN(number) || double.IsInfinity(number) || number < 8 || number > 128 || number != Math.Truncate(number))
                        throw new ModContentException("UI font_size must be an integer from 8 to 128.");
                    size = (int)number;
                }
                return new ModUiStyle(OptionalString(table,"text_color",null,function),
                    OptionalString(table,"background_color",null,function), OptionalString(table,"fill_color",null,function),
                    size, OptionalString(table,"text_align",null,function));
            }

            private static double UiNumber(Table table, string name)
            {
                var value = table.Get(name);
                if (value.IsNil()) return 0;
                if (value.Type != DataType.Number) throw new ModContentException("UI " + name + " must be a number.");
                return value.Number;
            }
        }
    }
}
