# Local Versus Recovery Notes

This document records evidence relevant to restoring standalone local versus in Eclipse. It separates Switch IL2CPP layout/signature evidence, behavior visible in current Eclipse source, and new design proposed for the local implementation.

Native recovery was verified on September 16, 2026. The extracted Switch executable
contains the actual ARM64 method bodies, and IDA Professional 9.1 with Hex-Rays
successfully decompiled 14 targeted PvP routines. The original local implementation
was written before this native analysis. Runtime behavior must still be validated
independently; a successful decompilation is not a tested port.

See [Switch PvP native recovery with IDA](../../Tools/SwitchPvpRecovery/README.md) for the
reproducible tools, exact function list, reviewed findings, and interpretation
limits. The final native exports and IDA database are in
`Temp/SwitchPvpIDA-20260916-final/`.

## Evidence limits

The initial inspection used IL2CPP Dumper metadata in `ResearchSources/reversingsf2/decompilation/generated/switch_il2cpp/il2cppdumper/dump.cs` and matching signatures in `script.json`. Those text dumps provide type layouts, fields, method signatures, and RVAs, but no implementation bodies. The later native analysis used `reference_builds/switch_v1.1.0/program/exefs/main`, verified all three decompressed segment hashes, and produced actual disassembly and pseudocode. Absence of bodies in a metadata dump does not imply they cannot be recovered from the executable.

`ResearchSources/reversingsf2/decompilation/generated/switch_il2cpp/cpp2il/` contains reconstructed assemblies, including `Assembly-CSharp.dll`. The retained Cpp2IL log says no processing layers were requested for that compatibility-DLL run. It was not evidence of an unsuccessful exhaustive native recovery attempt.

## Switch PvP data model

`PvpCharacter` is declared at `dump.cs:322193-322232`, TypeDefIndex 6757. Its layout contains `name`, `template`, `imageName`, `bigImageName`, `defaultWeapon`, `lockedByBattleIDS`, `lockedName`, `bossAbilityDescription`, `bossAbilityHint`, and `_isOpened`. Its `PvpCharacter(XmlNode node)` constructor is at RVA `0xFC7DF0`. The same range exposes `Name`, `isBoss`, `isHaveBossAbilityHint`, and `isOpened` accessors.

`PvpLocation` is declared at `dump.cs:322235-322246`, TypeDefIndex 6758. Its layout contains `name`, `template`, and `imageName`. Its `PvpLocation(XmlNode node)` constructor is at RVA `0xFC82E0`.

`PvpSettings` is declared at `dump.cs:322249-322313`, TypeDefIndex 6759. It contains `Node`, character/location/weapon arrays, and horizontal/vertical/default-position values for character and location selection. Relevant signatures are:

- constructor, `dump.cs:322273-322274`, RVA `0xFC8430`
- `GetCharacterModuleSizes`, `dump.cs:322293-322294`, RVA `0xFC9490`
- `GetLocationModuleSizes`, `dump.cs:322296-322297`, RVA `0xFC94B0`
- `UpdateCharactersLock`, `dump.cs:322299-322300`, RVA `0xFC9230`
- `ParseCharacterNode`, `dump.cs:322302-322303`, RVA `0xFC8550`
- `ParseLocationNode`, `dump.cs:322305-322306`, RVA `0xFC8930`
- `ParseWeapons`, `dump.cs:322308-322309`, RVA `0xFC8D10`
- `GetCharacterByAvatar`, `dump.cs:322311-322312`, RVA `0xFC94D0`

`ListSF.ParsePvpSettings()` appears at `dump.cs:321590-321596`, RVA `0xD10920`. This is signature evidence that PvP settings were part of the Switch data-loading surface. Its body is not recovered here.

## Switch fight construction and rules

The Switch `GameUtils` surface at `dump.cs:364468-364490` includes:

- `PrepareFight(FightList, out ModelParameters, out List<ModelParameters>)`, RVA `0xD449B0`
- `CreateFight(object, PreFight)`, RVA `0xD450F0`
- `CreateFight(FightList, ModelParameters, List<ModelParameters>, PreFight)`, RVA `0xD45230`
- `CreateFightPVP(FightList, List<ModelParameters>, ref ModelParameters)`, `dump.cs:364486-364487`, RVA `0xD44F00`

The native `CreateFightPVP` body has now been recovered. It prepares the first player, registers both input assignments with `InputSelector`, and enables user control while disabling AI for the first enemy when at least two gamepads are present. That branch also sets the enemy's `IsPlayer` flag to true. The reviewed instruction addresses and implications are recorded in the IDA recovery guide.

`PvpRule` is declared at `dump.cs:339088-339100`, TypeDefIndex 7116. It derives from `InFightRule` and exposes `PvpRule(XmlNode, RuleAppliance)` at RVA `0xEB6EB0`, `CompareSingle(object)` at RVA `0xEB6EF0`, and `Copy()` at RVA `0xEB6F00`. Native analysis confirms that `CompareSingle` unconditionally returns false (`MOV W0, WZR; RET`) in this build.

Current Eclipse still defines `FightPVP = 18` in `Assets/Scripts/Assembly-CSharp/BattleType.cs:1-26`. Vanilla move data in `Assets/vanillaXml/animations/moves.xml` contains `FightPVP` conditions and PvP-specific moves. A restored local mode should retain `FightPVP` so those existing conditions see the intended battle type.

## Switch PvP scene and input layout

`PvpScene` is declared at `dump.cs:402445-402552`, TypeDefIndex 8667. Its layout contains `battleName = "PvpBattle|pvp|1"`, character/location/random selection modules, selected player-one/player-two templates, selected location, a `PvPHintController`, and static `_PlayerOneInput`/`_PlayerTwoInput` fields. Recovered properties include `HaveSecondPlayer`, `PlayerOneInput`, and `PlayerTwoInput`.

Relevant signatures are:

- `Init(object)`, `dump.cs:402492-402493`, RVA `0xD95000`
- `LoadItems()`, `dump.cs:402508-402509`, RVA `0xD953B0`
- `OpenCharacterChooseModule()`, `dump.cs:402511-402512`, RVA `0xD95950`
- `OnCharsConfirm(string, string)`, `dump.cs:402514-402515`, RVA `0xD95B10`
- `OnLocationConfirm(string, string)`, `dump.cs:402520-402521`, RVA `0xD95E90`
- `StartFight()`, `dump.cs:402529-402530`, RVA `0xD961F0`
- `GetModelParameters(string, PlayerId)`, `dump.cs:402532-402533`, RVA `0xD96FB0`
- `GetLocationNode()`, `dump.cs:402535-402536`, RVA `0xD96C50`
- `SwitchPlayerOneInput()`, `dump.cs:402538-402539`, RVA `0xD971B0`
- `SwitchPlayerTwoInput()`, `dump.cs:402541-402542`, RVA `0xD97380`
- `SwitchToNextNotOwnedDevice(InputData, InputData)`, `dump.cs:402544-402545`, RVA `0xD97260`
- `SwitchInputToNextDevice(InputData)`, `dump.cs:402547-402548`, RVA `0xD97430`

These signatures establish two distinct `InputData` slots, explicit device ownership/cycling APIs, two character selections, and location selection before fight start. The later native analysis additionally recovered cycling and model routing: ownership equality compares input type and control index, while `InputSelector` resolves the assigned team and corresponding fighter. The cycle includes two keyboard indices before gamepads.

The English localization in `Assets/vanillaXml/localizations/eng.xml` describes sparring and asks the player to prepare Joy-Con controllers for two players, supporting the local two-player intent.

## Switch result screen

`EndPvpFightScreen` is declared at `dump.cs:393295-393365`, TypeDefIndex 8465. It contains a `FightResult`, player label/avatar, PvP result content, and presentation state. Relevant signatures are `Init(FightResult)` at `dump.cs:393338-393339`, RVA `0xE51220`; `ReplayHandler()` at `dump.cs:393341-393342`, RVA `0xE51A20`; `CloseHandler()` at `dump.cs:393344-393345`, RVA `0xE51760`; and `OnAnimationFinishButton()` at `dump.cs:393350-393351`, RVA `0xE51AD0`.

Both replay and close handlers have native pseudocode exports. The reviewed replay handler calls `Fight.RestartFight` and destroys the result-screen object. The close handler also contains conditional resistance/lottery presentation paths, so it must be interpreted in context before reusing its behavior.

## Current Eclipse side, input, and AI seams

`ModelParameters` stores `IsPlayer`, `EEGMBGBLLIF`, and `ABAPAIEBNGK` at `Assets/Scripts/Assembly-CSharp/ModelParameters.cs:50-64`. Its constructor initializes `IsPlayer = false`, `EEGMBGBLLIF = true`, and `ABAPAIEBNGK = false` at `ModelParameters.cs:399-424`.

`ModelAi.get_IsEnabled()` at `Assets/Scripts/Assembly-CSharp/ModelAi.cs:1437-1440` returns `get_AiOn() && (NHDAJBADMND.EEGMBGBLLIF || AiData.get_BothBotEnabled())`. `Fight` globally enables `ModelAi` at `Assets/Scripts/Assembly-CSharp/Fight.cs:992-999`. A locally controlled second fighter therefore needs its per-model AI flag disabled unless the input architecture changes this gate.

`IsPlayer` currently represents more than human control. `Fight` and `Model` use it for side ownership, camera behavior, item/rule selection, winner/counter handling, and player-model assumptions. For example, `Fight.CheckCountersStopFight` branches on the winner's `IsPlayer` at `Fight.cs:3891-3942`, and `Model.EPCNJLEHJCB()` returns `ModelParameters.IsPlayer` at `Model.cs:2355-2358`. Player two therefore needs a separate local-control/input-slot concept unless all `IsPlayer` branches are audited for two-human semantics.

`Model` creates a `ModelAi` for every fighter at `Assets/Scripts/Assembly-CSharp/Model.cs:2495-2515`. Local control should gate AI decisions rather than assume that the AI object is absent.

## Current Eclipse campaign start seam

The ordinary `GameUtils.StartFight` flow in `Assets/Scripts/Assembly-CSharp/GameUtils.cs` is campaign-oriented: it resolves/creates roster fight state, can register/update it, and dispatches fight-entry/story/quest state. A standalone local match should have a transient construction path instead of using that campaign entry point.

`FightHolder` is narrower. `Assets/Scripts/Assembly-CSharp/FightHolder.cs:46-58` consumes a static `FightList` and calls `GameUtils.ABAIHGFPHMO(fightList, preFight)`. The Switch `CreateFightPVP` signature supports adding an explicit PvP preparation branch while retaining the native `Fight` loop.

The implemented local match does not clone the campaign fighter loadout. It creates
fresh warrior parameters for each side and attaches cloned catalog items for the
selected standard equipment. The local bootstrap separately clones the profile XML
document so parsing and mod-state migration work cannot mutate the campaign document.

## Current Eclipse counters and end seam

`Fight.FinishRound()` at `Assets/Scripts/Assembly-CSharp/Fight.cs:3298-3307` stops the round after Eclipse round-end combat events. `GameOver` at `Fight.cs:3309-3323` marks the game over and invokes `CheckCountersStopFight`.

`CheckCountersStopFight` at `Fight.cs:3891-3942` assumes `FightList.CNAOMDMIGLJ` is a valid `Battle`, reads that battle's fight list, and updates `CountersFight`. A transient versus fight must therefore have a structurally valid transient owner battle or explicitly bypass campaign counters.

`CheckCountersEndRound` at `Fight.cs:3944-3964` reads time/statistics from `preFight`. A local match using this native path needs a compatible `PreFight` or a versus-safe branch.

`CountersFight` is not purely presentational. `Assets/Scripts/Assembly-CSharp/CountersFight.cs:311-338` completes counters and writes `Counter.CompleteValue`; increments dispatch events at `CountersFight.cs:353-364`. Local versus should use no campaign counters, an empty counter set, or an explicit bypass.

The ordinary end path at `Assets/Scripts/Assembly-CSharp/Fight.cs:4401-4439` calls `GameUtils.EndFight(...)`; another path does so at `Fight.cs:4163-4183`. That API is the campaign completion surface. Local versus needs interception before it so match completion does not award campaign rewards or advance roster, quest, achievement, or save state.

`Fight.BCFBHJOLGNL(FightResult)` at `Fight.cs:4203-4210` opens the existing end-fight screen through `PreFight`. A local implementation can reuse `FightResult`, while the Switch `EndPvpFightScreen` evidence supports a dedicated replay/return result surface.

## Current Eclipse boot and scene seams

`GameLoaderScene.Init` opens the Eclipse title at `Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Scenes/GameLoaderScene.cs:68-84`. `GameLoaderScene.Update` returns while `TitleScreen.IsOpen` at `GameLoaderScene.cs:86-105`, so normal loading modules do not advance while the title is open.

The loader sequence at `GameLoaderScene.cs:137-149` places `ParseModule` before `LoginModule`. `Assets/Scripts/Assembly-CSharp/ParseModule.cs:3-21` initializes variables/settings, animation and AI data, `ListSF`, perk tree, sound, localization, locale overlays, and user/roster data. This is the useful initialization boundary for a local lobby that needs real fighters, items, locations, moves, and rendering data.

`Assets/Scripts/Assembly-CSharp/LoginModule.cs:13-31` subsequently calls `GameUtils.CGFHDKDJCPL()`, hooks login completion, and starts `ListSF.IAAELKAKHPN()`. A title-launched local-versus path should be able to finish required parsing without implicitly entering campaign login/start-application quest behavior.

`Module.DLOKJOHNDID` at `Assets/Scripts/Assembly-CSharp/Module.cs:73-109` updates `QuestParameters` while changing screens. `Module.OAAFAINKKMI` at `Module.cs:111-123` loads the scene and dispatches the scene-loaded quest event when registered. Local lobby/fight/result transitions therefore need a quest-suppressed transition seam or equivalent local-session transition.

## Eclipse implementation design

Everything below is **new Eclipse design** informed by the recovered Switch architecture and current runtime constraints. It is not recovered Switch method behavior. The implemented first local mode is described in `Docs/LOCAL_MULTIPLAYER.md`, including the native validation results and remaining physical-controller checks.

1. Add a local-versus session object owning player-one selection/loadout, player-two selection/loadout, location, round settings, and two local input slots.
2. Let title-launched versus complete the parsing/data initialization it needs, then branch before campaign login/start-application quest work.
3. Add a quest-suppressed local scene/module transition for lobby, fight, and result navigation.
4. Build independent `ModelParameters` for both fighters from fresh warrior data, then attach cloned catalog items for the standard local loadouts. Keep player two's AI flag disabled and represent controller ownership separately from `IsPlayer`.
5. Build a transient `FightList` with `BattleType.FightPVP` and a structurally complete transient owning `Battle` containing that fight. Do not register it in campaign battle/roster/save collections.
6. Add a PvP preparation/construction branch around the existing `Fight` runtime, analogous in responsibility to the Switch `CreateFightPVP` seam. Feed it both prepared fighters and their distinct local input sources.
7. Reuse native round timing, health, combat, camera, animation, and winner calculation where they are side-safe. Bypass campaign counters and campaign completion at explicit versus seams.
8. Finish into a local `FightResult`/result UI. Rematch should rebuild/reset transient match state from the session selections. Return should dispose transient state and navigate back without campaign progression side effects.
9. Keep this local architecture suitable for later online input transport: fighter side identity, input source, and campaign ownership should remain separate concepts so remote input can replace player-two local input without rewriting combat semantics.

The implemented save boundary is additional Eclipse code rather than recovered
Switch behavior. `ListSF` captures whether its loaded profile belongs to the local
session, parses local bootstrap state from a cloned profile document, and suppresses
authentication/save queue processing for that captured local profile. The captured
flag deliberately outlives the visible session so stale callbacks after title return
cannot write local state into the campaign save.

### Lobby-to-fight presentation

Local and online matches (including room matches and rematches) keep the Eclipse
VS introduction visible across native scene loading until the first round is
prepared. The loader scene still clears its atlas cache and loads the fight, but
disables both vanilla loading pictures while that introduction owns the screen.
The menu backdrop holds its rendered frame while the outgoing scenery unloads.
Before loading starts, both fighter previews preserve their live world roots
across scene unloading. Each root owns its model container, render layers,
fighter meshes and camera; the UI continues displaying that camera's texture.
Animation and camera framing keep updating during loading. Hiding the VS screen
deactivates its independent preview worlds, and destroying the preview components
releases those worlds and textures.

The native VS artwork is hidden for these matches. Its animation duration and
simulation stop event remain unchanged so peer synchronization and existing
recorded input timelines do not change. Replays, training and campaign fights
retain their existing native presentation. The introduction starts loading after
its normal online delay; local players can still start loading sooner with Enter,
Space or Escape. It disappears only when the owned fight reaches round one;
loading errors and early disconnects keep their replacement menus visible.

`python Tools/Tests/Presentation/TestVersusTransition.py` passes 48 controlled
production-method checks for loading artwork ownership and preserved native VS
timers. Unity's bundled compiler compiled all four game assemblies, and the
online-core suite passed 55,227 checks.
`pwsh -NoProfile -File Tools/Tests/Presentation/TestVersusPreviewNative.ps1`
also passes 30 native rendering checks using the complete production preview
component with controlled animated fighter meshes: models and cameras survive
scene unloading, animation ticks and rendered pixels continue changing,
transparency is preserved, hiding the UI stops the worlds, reopening resumes them,
and preview destruction releases the worlds and textures. This verifies
the live preview lifetime in Unity; the full multiplayer transition with recovered
fighter art still needs a game playtest.

The first implementation passed 46 native Unity checks recorded in
`Temp/LocalVersusNative/result.txt`, including title boot, fighter input and movement,
rounds, draws, rematches, results, teardown, and unchanged primary/backup save
contents and write times. Those checks establish the tested Eclipse behavior;
they do not exercise physical buttons. Switch native recovery is established
separately by the later IDA exports described at the top of this document.
