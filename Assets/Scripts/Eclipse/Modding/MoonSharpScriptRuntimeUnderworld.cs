using MoonSharp.Interpreter;

namespace Eclipse.Modding
{
    public sealed partial class MoonSharpScriptRuntime
    {
        private sealed partial class MoonSharpScriptContext
        {
            // sf2.underworld: presentation of the Underworld (raid) map for owned pages.
            private void AddUnderworldModule(Table root)
            {
                var underworld = new Table(_script);
                underworld.Set("set_toggle_visible", DynValue.NewCallback((ctx, args) => ApiCall("sf2.underworld.set_toggle_visible", () =>
                {
                    _api.RequireCapability("story.progression");
                    if (_uiCloseDepth != 0) throw new ModContentException("Underworld presentation is unavailable during UI cleanup.");
                    if (args[0].Type != DataType.Boolean)
                        throw new ModContentException("sf2.underworld.set_toggle_visible requires a boolean.");
                    if (ModUnderworldAccess.SetToggleVisible == null) throw new ModContentException("Underworld presentation is unavailable in this host.");
                    return DynValue.NewBoolean(ModUnderworldAccess.SetToggleVisible(args[0].Boolean));
                })));
                underworld.Set("set_focus", DynValue.NewCallback((ctx, args) => ApiCall("sf2.underworld.set_focus", () =>
                {
                    // The host refuses battles outside this mod's Underworld pages.
                    var battle = ReadProgressionBattle(args, "sf2.underworld.set_focus");
                    if (ModUnderworldAccess.SetFocus == null) throw new ModContentException("Underworld presentation is unavailable in this host.");
                    return DynValue.NewBoolean(ModUnderworldAccess.SetFocus(battle));
                })));
                root.Set("underworld", DynValue.NewTable(underworld));
            }
        }
    }
}
