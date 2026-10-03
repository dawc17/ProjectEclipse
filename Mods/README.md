# Mods

This directory holds loose mod content. The [public modding wiki](../Docs/Modding/README.md)
documents the implemented Lua API, asset formats, capabilities and examples.
Start with [your first weapon](../Docs/Modding/src/content/docs/guides/first-weapon.mdx)
if you are creating a mod.

## Installing a mod

Place each mod in its own folder with `mod.toml` at the folder root:

```text
Mods/
  my.mod/
    mod.toml
    scripts/
    assets/
```

Use this repository's root `Mods/` directory when running from Unity. Standalone
desktop installations normally use `Mods/` beside the game executable;
launcher-managed installations may use a configured shared directory.

On Windows and Android, open **Mods > Install ZIP** on the title screen to
install an archive with `mod.toml` at its root or inside one enclosing folder.
Review the preview and choose **Install mod**, or **Replace mod** for an update.

## Enabling and disabling mods

Open **Mods** from the title screen, toggle your selection, then choose
**Apply & Restart**. Enter Campaign after the restart. **Back / Cancel** discards
unapplied changes. During gameplay, choose **Menu > Return to Title** to reach
the mod list.

Enabling a mod enables its dependencies. Disabling a dependency disables mods
that require it. Core remains enabled. Resolve requirements shown under
**Details** before applying. New mods default to enabled; selections persist
across launches. Disabling a mod retains its saved progress.

## Content and reference

- [Repulse Trial](example.repulse/README.md) demonstrates a Lua movement ability using queued fighter displacement.
- [Active Strike Trial](example.active-strike/README.md) starts an authored move through a Lua HUD ability and observes its playback receipt.
- [Audio Lab](example.audio-lab/README.md) plays an original beacon through owned sound instances, with volume, stop and game/real pause clocks.
- [Chiaroscuro](chiaroscuro/README.md) is the cinematic visuals mod kept here.
- [Archived examples](../ArchivedMods/) preserve earlier showcases outside the active mod directory.
- [Engineering records](../Docs/Engineering/Modding/README.md) contain plans, audits and verification history.
- [Historical mod guide](../Docs/Engineering/Modding/LEGACY_MOD_GUIDE.md) preserves the former long README, including superseded API notes.

Definitive Edition is a downstream content target. Its
[parity requirements](../Docs/Engineering/Modding/DE_PARITY_TARGET.md) describe
required coverage, not a declaration of current API support.
