# Open work

This backlog tracks pending verification and investigations. Completed presentation
work and its original checks are preserved in the
[September 28 engineering record](Docs/Engineering/UI_CONTINUATION_2026-09-28.md).
No visual acceptance is implied by a managed test passing.

## Presentation acceptance

- [ ] Shop: check forge icon, open/close/preview/apply, requirement visibility,
      hint arrow, upgrade/buy inline icons, wheel scrolling and perk auto-scroll.
- [ ] Fight: check entering pause without activating a pause button, charge/joystick
      easing, critical sound, banners/results, VS timing and raid bar counter.
- [ ] Map: check challenge/difficulty crossfade, wheel scroll, press bounce,
      Eclipse tint, normal/Power Mode tint and returning without tint leakage.
- [ ] Title/settings: check door transition, hover reset, mod-settings footer,
      globe, intro toggle/disclaimer/credit, upscaled scenes, control packs and quit.
- [ ] Downstream DE128: verify its 150-second rounds and distinct fight/menu
      loading artwork in that mod's acceptance workflow.

## Runtime and recovery verification

- [ ] Complete a full fight/result sequence and apply an enchantment; check all
      dojo choices and transition timing. Earlier captures covered limited scenes.
- [ ] Reproduce the UGUI Selectable index errors, LightingChainStart preview error
      and BackKeyManager teardown warnings recorded in the historical UI check;
      investigate any that remain in the current build.
- [ ] Resolve the recorded missing `fungus_raid/layer_0_2` Underworld resource using
      recovery evidence, then rerun `python Tools/Audits/AuditUnderworld.py`.
- [ ] Run native sprite/thumbnail validation with each fixture's matching editor,
      following [the native recovery procedure](Docs/Engineering/SPRITE_NATIVE_REBUILD.md).
- [ ] Restore access to `cross_build_map/authoritative_members.tsv` before running
      the reviewed-map dry run; do not infer confirmed recovery mappings.

## Planning

- [ ] Establish the intended mode/mod scope of the
      [equipment selection notes](Docs/Engineering/Modding/EQUIPMENT_SELECTION_NOTES.md)
      before implementing their proposed stats or enchantment policy.
