# Definitive Edition scope audit

Date: 2026-08-29

Eclipse is the reusable/open-source base project. Definitive Edition should be a
downstream mod/content package once the modding API exists.

This note records which current changes are general reconstruction/platform work
and which ones overlap with the original Definitive Edition feature set. It is a
scope guide, not a request to revert currently working behavior.

## Clearly base-project work

These are useful regardless of whether Definitive Edition exists and should stay
in Eclipse:

- AssetRipper project repair and Unity 2022.3 compatibility.
- Xbox/controller input support.
- high-refresh-rate fight/camera/render interpolation.
- Cocos/TexturePacker frame-offset correction.
- TableView `CanvasRenderer` reconstruction fix.
- motion-blur/rendering fixes and desktop render settings.
- content-loader schema/path compatibility needed to consume recovered or modded
  XML safely.
- historical item/model aliases where they only bridge versioned asset names.
- diagnostics, validation tools, build tooling, and deobfuscation workflow.
- generic support for recovered fight types, raid mechanics, HUD states, maps,
  bosses, and other content that already exists in recovered code/assets.

Supporting a recovered feature is not the same as forcing that feature to be
enabled in vanilla. The base should expose enough machinery for content packs to
use it.

## Currently DE-aligned behavior

These overlap directly with the original Definitive Edition goals and should not
be expanded further in the base until there is a mod/configuration boundary.

### Monetization/offline policy

- `Assets/Scripts/Eclipse/Runtime/Offline/OfflineServices.cs` blocks recovered
  external web/service behavior and only permits local filesystem content through
  legacy download paths.
- `Assets/Scripts/Eclipse/Runtime/Offline/OfflineStoreData.cs` provides store
  compatibility types whose products are never available to purchase.
- recovered networking/browser call sites have been adapted to those offline
  shims.

This strongly aligns with DE's "fully offline", "no ads", and "no real-money
purchases" direction. It is useful for a self-contained PC build today, but the
long-term base architecture should make service policy replaceable rather than
define "offline forever" as a core game rule.

### Energy and premium/shop UI removal

`Assets/Scripts/Eclipse/UI/TopBar/DesktopTopBarLayout.cs` currently:

- hides the energy panel;
- hides the ruby purchase button;
- hides the shop/add button;
- hides the ruby-sale presentation;
- compacts the remaining currency/XP layout.

That is directly aligned with DE's "no energy system" and monetization-removal
presentation. The layout helper itself is reusable, but the decision to hide
those systems should eventually be a mod/profile option rather than vanilla
Eclipse policy.

The recovered energy implementation still exists (`GameUtils`, `ListSF`, dialogs,
timers, etc.). The current code primarily suppresses its UI; it is not a clean
architectural removal of the energy mechanic.

### Dojo Disciple

`DojoScene.Init` currently routes the selected fight through
`GameUtils.HIPIGHPMBIJ(...)`, enabling the recovered Dojo Disciple behavior that
was explicitly restored during this project.

That lines up directly with DE's "Dojo Disciple (restored cut feature)" item.
Keep the reconstruction capability, but avoid adding more policy that makes cut
content unconditionally part of vanilla before the mod/content boundary exists.

### Restored Underworld/event content

Eclipse currently contains explicit integration work for offline raid zones:

- importing `raid_stages_default.xml` zones when absent;
- adapting offline raid rounds;
- map toggle/power-mode presentation;
- raid map sprite/icon compatibility;
- segmented raid-boss life bar styling/transitions;
- raid shield presentation and diagnostics.

Much of this is valuable generic support because the recovered game already
contains raid systems. The DE-aligned part is *automatically restoring/enabling*
event/raid content in a self-contained offline content set. Future design should
separate "engine supports raid content" from "this content pack enables every
raid/event permanently".

### XML/content-pack compatibility

Several current transforms exist because the active XML set mixes content from
different historical versions:

- missing battle merge/materialization;
- quest syntax/action compatibility;
- old move/template restoration;
- survival reward-row completion;
- legacy round-time clamping;
- internal-settings fallback/normalization;
- historical item aliases and unavailable-model hiding.

The mechanism belongs in Eclipse because mods will need compatibility tools. The
specific transforms should eventually be versioned/content-pack policy rather
than an ever-growing pile of implicit vanilla mutations.

## DE features not meaningfully implemented by Eclipse code yet

The following parts of the original DE description are still primarily data
content, recovered game behavior, or future work rather than dedicated Eclipse
engine features:

- instant upgrades/enchanting/forging and removal of all wait timers;
- a general "gems earned through gameplay" economy replacement;
- removal of every battle pass / premium offer / FOMO path as a configurable
  engine policy;
- "every weapon/armor/helm/ranged/magic" as a code feature;
- Titan's Desolator reward policy;
- Monk set / Ascension reward policy;
- Sentinel, Neo-wanderer and other set availability policy;
- all cut/unused music enablement;
- Sensei's Story content itself.

Those are largely expressed by XML/assets and should be ideal early consumers of
the future modding/content API rather than hard-coded into Eclipse.

## Direction until the modding API exists

Prefer work in this order:

1. vanilla/recovered behavior fidelity and correctness;
2. reusable engine hooks and content-loading boundaries;
3. mod discovery/manifest/versioning;
4. data override/merge API for XML-driven mods;
5. code hooks/events only where data is insufficient;
6. move current DE-aligned policy behind a `Definitive Edition` mod/profile;
7. continue adding DE features in that downstream mod.

Until steps 3-5 exist, do not expand base-project hard-coding for "no energy",
"everything unlocked", "all events permanent", "no timers", or economy changes.
Fix bugs in the current behavior when necessary, but treat new DE feature work as
deferred mod work.

## Naming rule

Project-owned base code uses the `Eclipse` namespace and lives under
`Assets/Scripts/Eclipse/`. `SF2DE`/Definitive Edition names should remain only
where they are historical/provenance identifiers for actual source material or
where a future downstream DE mod explicitly owns the code/content.
