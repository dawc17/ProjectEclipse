# Underworld integration review — 2026-08-27

## Follow-up: editor startup crash

The initial 149-record UI text repair was unsafe: its declared sprite format
and vertex-channel layout disagreed. Unity crashed natively while building a
RaidMisc thumbnail. Those records have now been rebuilt with Unity 2019's own
Sprite.Create/serializer in an isolated project, preserving sprite GUIDs and
source PNGs. The handwritten exporter has been removed; see
`SPRITE_NATIVE_REBUILD.md`. Earlier managed/audit passes did not cover this
native import/preview failure.

## Confirmed defects repaired

- Volcano's crash came from the recovery script emitting `sourceSize={{w,h}}`.
  Corrected recovered metadata and made the parser tolerate the old form using
  invariant-culture numeric parsing. Invalid/nonfinite values still fail.
- Raid locations now use the custom layout matching recovered combined layers.
  Vortex previously loaded the embedded layout for obsolete separate tiles.
  Story locations retain their existing layout selection.
- Corrected layer/effect path joining, standalone picture effects, flip flags,
  and sprite-mask support needed by the DE event locations.
- The previous shield change was mostly a HUD overlay. Template inheritance
  could erase ShieldTotal and incoming damage still consumed the whole pool.
  ShieldTotal now controls total one-bar pools, including carry-over damage,
  healing, final death, reset, explicit-zero overrides, and cloning.
- The draining bar shows the current segment and a compact remaining-bar count.
  The count counter-rotates against the mirrored enemy-bar transform.
- Removed the previous guessed player-relative boss-stat rewrite. Authored
  WarriorPower, equipment, and the existing AttributesAlign rules remain in
  control. Fight creation logs their resolved values for balance investigation.
- Mode selection rejects saved hard-mode focus while Power Mode is disabled.
  The checkbox no longer occupies the label rectangle; newly added story
  buttons remain hidden in Underworld. Optional map lookups no longer report
  nonexistent Titan/intermission entries as errors.
- Raid timer labels allow three digits instead of clipping 999-second rounds.

## Asset recovery

`RepairUnderworldAssets.py --write` uses exact XML dependencies, local bundles,
and the reference DE APK/export. It does not download anything or touch saves.

- Repaired/added 76 atlas metadata files, including missing shared sequences.
- Recovered 110 loose/reference image and sprite files plus three shared floor
  tiles. The three floor tiles explicitly reuse legacy dojo foreground tiles.
- Recovered Volcano's spark_fire2 prefab and its material/texture dependencies.
- Repaired 149 UI sprite records whose UVs sampled the entire sheet and whose
  serialized render data omitted texture rectangles (including the shield).
- Removed external bundle tags from 62 recovered raid-preview metadata files.
- Preserved existing texture files and GUIDs when repairing metadata/sprites.

## Verification

- `msbuild Assembly-CSharp.csproj /nologo /v:quiet /clp:ErrorsOnly`: passed.
- `msbuild Assembly-CSharp-Editor.csproj /nologo /v:quiet /clp:ErrorsOnly`: passed.
- `Tools/TestUnderworldRuntime.ps1`: 269 assertions against the compiled runtime
  passed. These exercise real ModelParameters and Cocos frame-parser methods.
- `Tools/AuditUnderworld.py`: 76 battle definitions / 39 distinct locations,
  zero missing image, mask, scenery-sequence or particle-prefab references;
  checked atlas rectangles fit their PNGs and source-size metadata is valid.
- The static asset audit is not a claim that every battle has been completed.
  Unity desktop access was denied, so no visual/combat playtest was performed.
  The Unity-side importer validator was compiled but has not been run here.

## Playtest gate

1. Exit Play mode and let Unity finish importing/recompiling.
2. Run **SF2 > Validate Underworld Integration**. This checks actual imported
   sprites, textures, masks, scenery sequences, particles, and key raid UI art
   without touching a save. If sub-sprites are not imported yet, first run
   **SF2 > Import Recovered Texture Atlases** and retry validation.
3. Test normal Volcano and Vortex, then their Power Mode versions: confirm
   backgrounds, layered scenery, movement, attacks, timer, and shield counter.
4. Drain multiple bars, including a hit crossing a bar boundary. Kill the boss
   and verify rewards/return-to-map, then repeat a fight and verify full reset.
5. Visit all eight pages and event fights, especially the masked Halloween dojo,
   ritual locations and shared-dojo locations. Test power toggle after returning
   from a hard fight and after restarting the map.

Authored boss balance and complete boss-specific perk/reward/quest parity still
need end-to-end playtesting. This pass removes known integration failures; it
does not replace missing behavior with an assertion of complete DE parity.
