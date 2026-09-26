# Phase 1 integrated showcase

`example.phase1` is an acceptance/showcase mod for the public API surface available before P2A.

**Accepted end to end, 2026-09-09:** user gameplay retests confirmed map discovery,
fighter/AI/opening step, arena/music/round settings, dialogue, forge and Opening
Focus's effect/description, selectable locale, and finally the token reward panel.
This closes the integrated showcase gate and makes P2A ready to begin. Set metadata
has no collection UI; acceptance does not imply coverage of every API combination.
It deliberately uses only `require("sf2")` and public Lua functions. There is no raw XML,
test-only bridge, recovered C# object, or DE-specific branch.

This showcase primarily proves typed content integration, transactional
registration, references, and lifecycle cleanup. Much of its declarative content
can map to recovered XML without losing meaning; it is not the demonstration of
the API's full programmable direction. Static content should remain declarative.
Custom procedures should use Lua handlers and safe typed capabilities rather than
generic action/condition instruction tables. Existing quest and tactic adapters
remain supported compatibility paths.

See [the API design rule](../DE_API_IMPLEMENTATION_PLAN.md#37-static-definitions-and-programmable-behavior)
and `example.enchantment` for the current reusable behavior foundation. P2A must
add a runnable example with meaningful runtime decisions, state, and gameplay
effects. Expanded combat hooks, quest callbacks, and programmable AI are not
provided by this Phase 1 showcase.

The connected path is:

`state + localization -> P1C item/perk/forge -> P1D location -> P1A zone/battle/fight/reward -> P1B quests`

P1D move/template/trigger/tactic registration is also exercised. The tactic inherits Standard's
complete decision weights, and the P1A warrior consumes the opaque tactic handle directly.
The custom opening move is activated by a round-stage event, not selected by the tactic.
`sf2.tactics.name(handle)` is available only when an explicit recovered runtime name is genuinely
needed; normal authoring does not need to manufacture qualified tactic IDs.

The forge fixture borrows `sf2.forge.profile("Simple")`; it does not define or mutate shared forge
prices, currencies, delivery timers, skip prices, upgrade tables, or any other core economy field.

Run `Tools/TestPhase1Showcase.ps1` for the focused static contract and
`Tools/TestPhase1ShowcaseRuntime.ps1` for real discovery, MoonSharp execution, transactional commit,
duplicate rollback, and teardown validation.

## Testing the current fixture

Stop Play Mode, allow compilation to finish, and start a new Play session. An
already failed session can contain duplicate base zones and a disabled mod host;
reopening the map cannot repair that in-memory state. Do not reset the save.

1. Open the story map. The session quest should reveal/focus an additional page
   titled **Phase 1 Showcase**, with one **Phase 1 Showcase** battle. It reuses
   the Hero Reborn map art (`Map1.1`); its title and single battle distinguish it.
2. Enter that battle to face the level-1 **Showcase Fighter** using the custom
   ordinary tactic and **Opening Focus** perk.
3. The fight has one round and a 99-second timer. Its custom arena has a packaged
   battlefield backdrop, a 200-unit showcase emblem, its own WAV music, and
   separate fighter spawn positions. A black arena is not intentional.
4. The magic recharge rule applies each round. A character without an equipped
   magic item cannot visibly demonstrate that rule.
5. At the start of the fight stage, the perk owner should perform the custom
   opening step; its animation-start trigger plays the WAV at 25% volume. There
   is no new walk/dash control. Confirm timing and animation in a playtest.
6. Ending this specific fight triggers the completion dialog. The condition
   checks fight identity, not victory; the dialog itself grants no item.
7. Winning grants **one Phase Token** through the fight reward path. Its icon
   should appear in the item reward panel before the coin/XP breakdown, including
   when a token is already owned. This fixture defines no coin or XP reward.
8. Phase Token is a reward consumable, not equipment or a purchasable shop item.
   It is hidden from shop listings; displaying it there previously left the
   last weapon model on screen because it has no equipment presentation.
9. **Phase One Relics** is registered set metadata for the token. The sample adds no
   equipment or progression-tree branch, so it does not promise a new equippable
   set or a learnable perk in Profile.
10. The **Phase 1 Showcase** forge family borrows the base Simple economy and
    offers Opening Focus for weapons. Test only once the normal forge is
    accessible on the save; costs/timers remain those of the base profile.
    Equip the enchanted weapon and a magic item, then enter an ordinary fight:
    **Opening Focus adds 25% magic charge once at fight start**, capped by the
    normal meter limit, and increments the mod's `fight_begins` counter.
    The showcase's full-magic rule can conceal this bonus. There is no damage
    bonus, and this callback currently applies to the player's saved equipment.
    The perk also enables the automatic opening step for its owner.
    Existing forged copies use the new default parameter without reforging.
11. Settings should offer **English (Phase 1 Showcase)**. Base UI text and fonts
    remain available. `pol.toml` supplies Polish mod translations but does not
    register Polish as a selectable base language.

The callback originally only counted fight starts. It now uses the typed
`combat.magic_charge` capability. The runtime dispatches learned player perks,
saved equipment enchantments, and behavior-backed perks installed by the forge;
merely putting Opening Focus on this opponent does not invoke a player-save callback.

## September 9 runtime blockers

The initial sample reused the base locale code `en`, causing a late parser
exception. Retrying the parser appended vanilla zones again and then disabled
mods on duplicate core item registration. The sample now uses `en-x-phase1`;
locale collisions are validated before stage/move application, and late locale
binding failures cannot restart the parser. Locale teardown removes owned entries.

The initial sample also omitted map art, a gameplay layer/spawn positions, and
move activation events. These are now supplied. `Tools/TestModLocaleRuntime.ps1`
reproduces the original collision and parser retry failure; the showcase runtime
test checks the actual Lua fixture against canonical locale metadata.

Shop/Profile returning to Map was observed in the failed session. The save has
pending map quest checkpoints despite its finished tutorial. Navigation, the
fight, rewards, forge presentation, and language selection required fresh gameplay
retesting; the later user acceptance above supersedes this initial blocker report.

The subsequent Editor run reached the fight and exposed an AI selection error:
the event-only opening step was being selected through the keyboard AI path.
That path now requires key conditions, including when considering overrides;
round-stage events still activate the opening step directly. Missing dodge tables
for event-only poses/steps are treated as unavailable rather than repeatedly logged.
Two startup compatibility errors are also repaired: full version operands with
`CompareType="Versions"`, and the shipped energy dialog's empty Close button
(mapped to the recovered middle Cancel button). Run
`Tools/TestShowcaseEditorRegressions.ps1` for these focused regression cases.

The next playtest confirmed the map, fighter visuals, dialogue, and forge recipe,
but exposed stale tooltip descriptions, stalled AI, and an invisible arena.
Explicitly clearing a localized label now clears its previous text. The sample
no longer replaces all Standard animation weights with its event-only step or
adds an unbounded `SelfUninterrupt` interval. Qualified arena sprites now account
for pixels per unit; the backdrop uses the verified packaged sprite address
`core:Textures/Locations/battlefield/battlefield_bg1.back_1`.

Validation: showcase discovery/real Lua callback, fight dispatch (including forged
perks, active filtering, and one-shot dispatch), editor regression fixtures, and
managed compilation pass. The isolated Unity 2022.3.62f3 packaged-art validator
loads the backdrop and resolves it through the public asset API. Later user
gameplay retests confirmed arena rendering, opponent movement/blocking, tooltip
behavior, and the visible magic meter.

The subsequent playtest confirmed the opening step and normal opponent behavior,
fighter, music, round/timer, dialogue, magic-charge effect, and selectable locale.
It found three remaining presentation failures. Percentage formatting now handles
literal/trailing percent signs and uses current offsets after alias expansion.
The location path/cache recognizes `core:` sprite references without prefixing
`Textures/`, and the artwork is centered separately from fighter coordinates.
Reward projection now sets the recovered `Drop` flag required by the results UI;
owned external consumables remain eligible for another reward, and the duplicate
quest grant is removed. Tests cover formatting, qualified path preservation,
the real Unity location cache, projected reward visibility, and repeated consumable
eligibility (`Tools/TestModConsumableRewards.ps1`). The subsequent user retests
confirmed these paths without resetting the save.

The next playtest confirmed the arena rendering, including its floating sword
sample sprite. The remaining missing reward was traced to `FightList::getReward`
requesting slot 1 from a one-entry list on victory. The sample now supplies an
empty zero-win reward followed by the token victory reward. Empty reward
definitions are supported by the API, and the showcase runtime test checks both
slots. The completion dialogue intentionally repeats on every fight end.
The final user screenshot confirmed the item reward panel after victory, closing
the last outstanding showcase check.
