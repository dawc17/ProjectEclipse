# Owned custom UI implementation

Status: the initial `sf2.ui` contract is published in API 0.15. API 0.16 adds safe-area anchor placement. It advances E4
but does not satisfy its full creator-facing acceptance criteria. The public
reference in Docs/Modding documents the exact available surface and limits.

## Implemented ownership and state

`Assets/Scripts/Eclipse/Runtime/Modding/ModUiRuntime.cs` has no Unity component or
recovered game dependencies. `ModUiScope` owns up to eight surfaces for one mod;
disposing the scope closes all its surfaces. Duplicate open surface IDs are
rejected within a scope, and separate owners can reuse the same IDs. A closed
surface ID can be reopened with fresh state. Live references are never saved.

Each immutable layout tree contains up to 256 nodes, depth 16, with stable IDs.
Initial nodes support stack, row, column, scroll, text, button and progress.
Scroll has one content child. Rows/columns have spacing. Dimensions and progress
are finite/bounded, strings are bounded, and invalid trees are rejected before
being registered in a scope. Layout data contains no expressions or operations.

Live surfaces update text, progress, visibility and enabled state by ID. Reads
return immutable snapshots. Only changed values notify the renderer. Button
dispatch checks the widget and every ancestor for visibility/enabled state,
rejects reentrant dispatch, and ignores closed controls. A callback can update
or close its own surface. A throwing callback or rendering update closes only
that surface. Teardown observer failures cannot prevent other observers from
running; diagnostic failures cannot interrupt cleanup.

## Implemented Unity view

`Assets/Scripts/Eclipse/UI/Modding/ModUiView.cs` projects a surface into a supplied
RectTransform mount using Unity UI. It uses the same `ui/fonts/AGOpusBold` font
lookup and fallback as TitleScreen/ReturnToTitleButton. Text is plain, wrapped,
and not a raycast target. Progress updates move the fill boundary. Scroll content
uses ScrollRect and RectMask2D. Surface updates do not rebuild the tree.

The view has no global input polling. It exposes focus traversal and activation
to its future coordinator, disables Unity automatic navigation between unrelated
surfaces, and routes pointer buttons through the same guarded state model. Close
immediately deactivates the view before deferred native destruction; external
native destruction closes its model. Focus is restored when the closing view
still owns the selected object. Ancestor CanvasGroups mirror enabled state.

The caller must supply a valid canvas mount. This is deliberately not a hidden
claim that menu, modal and HUD mounting/input arbitration already exist.

## Implemented scene coordinator

`ModUiLayerStack` orders modal, menu and HUD surfaces, with the newest surface
winning within each mount priority. Only the foreground surface accepts input;
root visibility changes update priority, while ordinary widget updates do not
rebuild ordering. A surface cannot belong to two stacks. Scene disposal closes
mounted surfaces without disposing their script scopes. Native blocking suspends
all mod input; Back closes an exclusive foreground surface and leaves HUDs open.

`ModUiCoordinator` supplies scene-owned Unity canvases, safe-area anchors and
uniform fitting for oversized root layouts. Sorted canvas ranks follow the
layer stack and stay below the existing title-screen canvas. Menu/modal backdrops
consume pointer input outside their content. Background surfaces cannot receive
raycasts. Exclusive surfaces take and restore EventSystem navigation ownership;
the coordinator routes explicit traversal/activation/Back calls. HUD creation
does not automatically steal keyboard focus. Native blocking hides mod canvases
and yields navigation; it never changes native GraphicRaycaster enablement.

`ModUiGameBridge.Attach` now creates the scene coordinator on demand. The bridge
routes keyboard and controller navigation/activation/Back before ordinary game
updates, waits for neutral menu input on capture, repeats held navigation with
unscaled time, and reserves the closing frame so Back/Submit cannot also affect
the underlying game. Title/restart state suspends mod UI.

Native DialogCanvasController signals blocking before its raycaster changes;
BackKeyManager offers Back to the mod UI before the native screen stack.
GameController routes keyboard, gamepad and touch control events through
`ModUiControlGate`, releases active controls in press order when exclusive UI
captures input, and keeps polling release edges. Captured presses must release
before they can become new gameplay presses. Without capture, ordinary native
press/release delivery is preserved. These native hooks compile and have source
contracts; physical-device behavior in the complete game remains unverified.

Every MoonSharp script context owns a UI scope and disposes it before clearing
its script tables. API 0.15 binds creation, close, is_open and text/value/visible/
enabled setters through `sf2.ui`. An injected mount callback connects the game
host to the bridge and permits renderer-isolated Lua tests. Weak-key handle
storage avoids accumulating closed handles. Click callbacks use the existing
bounded Lua runner and carry a UI handle/widget ID, never a fighter capability.

## Remaining integration and expansion

1. Verify creation during startup, scene changes,
   registration rollback and script-session shutdown in the complete game.
2. Verify physical keyboard/controller/Back input, pointer click-through,
   overlapping mods and focus restoration in the complete game. Opening
   UI must not implicitly grant pause, combat actions or result authority.
3. Extend the existing bounded bindings with UI lifecycle hooks and additional
   gameplay intent where warranted. Never capture expired combat fighter handles
   in a later UI click. The Charged Strike example changes Lua state on click and
   consumes it using fresh authority in an outgoing-hit callback.
4. Extend explicit localized text refresh with automatic bindings if warranted;
   add validated custom sprites/fonts, intrinsic sizing,
   toggle/slider/list/grid behavior and virtualization. The current primitives
   are a foundation, not the promised full widget surface.
5. Complete the loadout chooser and branching lobby workflows, and expand the
   shipped pointer-based Charged Strike meter, with
   wiki sections, editor schema/templates and Lua/editor tests in the same change
   that exposes those public APIs.

## Verification

`Tools/TestModUiRuntime.ps1` executes production ownership/state code in an
isolated .NET fixture. It covers isolation, invalid/oversized trees and updates,
duplicate/reopened IDs, input suppression/reentrancy, callback/render failures,
scope disposal, stale handles and teardown failures.

`Tools/TestModUiUnity.ps1` creates a separate temporary Unity 2022.3.62f3 project
and enters play mode. It checks production native hierarchy/layout components,
font fallback, targeted updates, guarded activation/focus and cleanup. It uses
the installed Unity UI package. Since API 0.18 it also copies the original
button/parchment/combat-bar assets and game font, and the production ResolutionImage
loader. Resource/bundle and atlas lookup backends are fixture substitutes. Optional
`-WithPreview` enables graphics and saves a native rendered PNG for inspection.
This does not verify actual device events, native game dialogs, combat pause or
full-game scene integration. It now includes multiple
simultaneous mod canvases, modal/menu/HUD priority, focus handoff, native-block
simulation, Back routing, duplicate mount rejection and coordinator destruction.

Current evidence: 84 managed UI/control checks and 66 isolated Unity play-mode
checks pass, plus 209 battle-rule checks including actual Lua-context UI scope
ownership/disposal, the existing combat/mode regression and all four managed
project builds. Bridge fixture shell/controller signals are substitutes; the
production bridge, coordinator and renderer execute in Unity play mode. No
physical-device/full-game acceptance is claimed. API 0.15 adds a separate
434-check real-Lua fixture for validation, capabilities, handles, callback
instruction bounds, mounting/entrypoint failure cleanup and the full Charged
Strike source across 300 ticks, arming, blocked hits, consumption and rounds.


## Anchored placement and Lua-to-Unity verification

API 0.16 adds an immutable optional `placement` with nine anchors and finite
reference-unit offsets. Center remains the default. Positive X moves right and
positive Y moves down; the renderer uniformly fits oversized roots and clamps
their full rectangle to the safe area, recomputing from requested offsets after
resize. The model and Lua parser reject unknown fields/anchors and invalid values.
The Charged Strike example and editor template now use the top-right anchor.

The isolated Unity fixture now copies the production MoonSharp bindings, neutral
runtime, installed MoonSharp assembly, canonical stages and actual example Lua.
Its real Unity button invokes the Lua click callback, 300 combat ticks update the
native label/progress bar, and fresh hit callbacks consume the armed bonus.
Round and script disposal close the native view. Anchor/pivot, offset, extreme
clamping and resize/restore checks execute the production renderer. Fighter and
physical input signals remain fixture substitutes; this is not full-game visual
or physical-device acceptance. Editor definitions, LuaLS completion and the wiki
cover the same placement contract.


## Dynamic translations

API 0.17 exposes `sf2.localization.text` using a registration-time localization
handle. Reads resolve the current catalog or the current transaction's pending
values/patches, with the native game language provided by ModRuntime on every
call. The shared assembly has no dependency on LocalizationManager. UI widgets
still store plain strings; Lua explicitly formats and refreshes them. Charged
Strike resolves its English/Polish text during each refresh. The Unity fixture
changes the language provider after combat has begun and verifies native status
and button text, including unknown-language English fallback. Font glyph and
native language-menu acceptance remain full-game work.


## Game-consistent styling

The user explicitly requires custom UI to remain consistent with the original
game. Shared defaults therefore use AGOpusBold, native parchment for menu/modal
roots, CommonButtons.BtnWhite for buttons, and FightUI health-bar strip sprites
for progress tracks/fills. Text and selectable-state colors follow the native
LabelButton/ExitDialog prefabs. HUD roots remain transparent. Buttons scale their
native sliced borders with authored height so the pointed ends retain proportion.
Resources go through the existing ResolutionImage compatibility loader; no source
texture, serialized sprite geometry, asset GUID or meta file was changed.

API 0.18 provides immutable explicit style fields for font size, horizontal text
alignment, text color, background tint and progress-fill tint. Validation enforces
integer font sizes 8..128, known alignment values and #RRGGBB/#RRGGBBAA syntax;
irrelevant widget/style combinations fail. Defaults do not require authors to
recreate the game theme. Input semantics and rich-text prohibition remain intact.

The isolated graphics fixture renders the real font and recovered skin assets.
Its preview was visually inspected for parchment, label appearance, button shape
and bar texture. Root visibility also controls the added parchment graphic.
Complete native screen composition and physical input still need full-game QA.


## Close notification and cancellation

API 0.21 adds on_close(view, reason) after successful mount and teardown. The
neutral surface records the first reason and clears widgets/click handlers,
removes ownership and runs native Closed listeners before calling Lua. Back,
scene-stack disposal, external native destruction and callback/render errors
have distinct reasons. Scope/script shutdown and failed mounts do not run Lua
notifications. Close errors are isolated and notifications have the existing
200,000-instruction callback budget. Opening UI from any nested close callback
is rejected; this prevents new surfaces escaping scene teardown.

Charged Strike clears its view reference and cancels charge/armed state on
close. The model fixture verifies reason/order/once-only delivery and observer
failures. Actual Lua tests cover nested close, invalid/stale handles, budgets,
reopen rejection, mount/shutdown suppression and scene-close cancellation.
The Unity fixture verifies external native destruction reports its reason and
cancels an actually armed Lua bonus; the original game skin is preserved.
Physical input, full scene transitions and native gameplay acceptance remain
manual checks. This provides a cancellation notification, not asynchronous
mode entry or a transactional run controller.
