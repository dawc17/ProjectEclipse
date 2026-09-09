# Mod menu and session restart

The title screen has its own Mods entry. The campaign's expanded main menu has
a Return to Title button. Apply & Restart saves the selection and reloads to the
title; Campaign then starts the selected content through normal initialization.

Selections are versioned XML under `Application.persistentDataPath/ModSelections`,
keyed by the mod root. Only disabled IDs are stored, including temporarily absent
mods. Core cannot be disabled. Dependency toggles cascade, and dependency
resolution validates the selected set before application. Draft edits never
change the active host. File replacement preserves the previous selection if
writing the temporary file fails.

The restart path saves the current native profile before saving preferences or
changing scenes, then explicitly stops persistent campaign music and effects.
It uses the existing scene loader. On preloader re-entry,
mod adapters are disposed before native list/module reset, then the title gate
opens. This keeps old scene objects from accessing a prematurely cleared roster
and rebuilds registrations for the next campaign session.
Native profile reset removes its global timer subscription before clearing the
roster. The Return to Title overlay owns a root canvas so the recovered menu's
scale cannot shrink or reposition it; it appears alongside the expanded menu,
below the currency header.

## Automated checks

Run from the repository root:

```powershell
.\Tools\TestModSelection.ps1
.\Tools\TestGameSessionRestart.ps1
.\Tools\TestModMenuUI.ps1
```

Selection tests cover persistence, transitive dependency changes, missing mods,
invalid settings, and the core guard. Restart tests execute production sequencing
against boundary stubs, including save failure and duplicate-click handling.
They also execute the production profile reset across repeated timer subscriptions.
The isolated Unity UI fixture executes the production menu components, validates
draft/apply behavior and main-menu visibility, and renders PNGs into `Temp`.
Its native save and scene transition are stubs; it is not a full campaign test.

User testing confirmed that disabling all mods and re-enabling them both load
successfully. Returning to title exposed stale profile timer callbacks, persistent
dojo music, and a nested-canvas placement bug; those fixes still need a full-game
retest from the dojo.

## Full-game playtest

1. Open Mods from the title. Disable a showcase, cancel, reopen: it remains enabled.
2. Disable it and Apply & Restart. Enter Campaign: its zone/content is absent.
3. Open Menu, Return to Title, re-enable the showcase, and apply. Its content returns
   exactly once, with its saved mod progress intact.
4. Repeat the cycle with several mods and check that zones do not accumulate.
5. Exit and relaunch: the selection remains saved.
6. Disable a required mod and check that its dependents switch off. Re-enable a
   dependent and check that its requirements switch on.

The return entry is on the campaign main menu; it is not a combat pause action.
