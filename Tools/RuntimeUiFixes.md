# Runtime UI and combat fixes — 2026-09-19

## Project status

The checkout started at `98d449fb` (Start gradual DE129 implementation), with existing generated changes to `Assembly-CSharp-firstpass.csproj` and `Assembly-CSharp-Editor.csproj`. Those changes were preserved. The actual project/editor version is Unity 6000.6.0f1, although the agent guide still names 2022.3.62f3. The editor was open on Shop outside Play mode.

All fixes below are engine C#. No DE128/DE129 Lua scripts, gameplay XML, existing sprite assets, or Unity GUIDs were changed. New helper scripts have new metadata. Changes remain uncommitted.

## Requested changes

| # | Implementation | Main source |
| --- | --- | --- |
| 1 | Select the active options tab after rebuilding the page; Display no longer retains keyboard selection. | `TitleScreen.cs` |
| 2 | Accepted keyboard/gamepad presses and releases update action-button and stick visuals; UI input capture releases visuals too. | `GameController.cs`, `ActionButtons.cs`, `Stick.cs`, `SFButton.cs` |
| 3 | Restore each loading-logo half's trimmed size and offset relative to its original canvas. | `SplitImageLayout.cs`, `ResolutionImage.cs` |
| 4 | Critical pause defaults to 80% of its original duration; persistent Accessibility slider allows 0–100%. | `AccessibilitySettings.cs`, `TitleScreen.cs`, `Camera.cs` |
| 5 | Apply the corresponding trimmed-canvas correction to both Pause halves. | `SplitImageLayout.cs` |
| 6 | Cache failed atlas lookups and use exact sprite fallback instead of loading the entire UI atlas directory; avoid repeated inline-icon name assignment. | `AtlasCache.cs`, `TextPic.cs` |
| 7 | Convert wheel notches to visible list movement and stop competing inertia/tweens. | `SFScrollRect.cs`, `TableViewScroll.cs`, `ItemsScroll.cs` |
| 8 | Compensate the recovered dojo background tiles' missing rendered width at runtime so the two halves meet. | `Location.cs` |
| 9 | Resolve complete texture paths supplied by formatted dialogue arguments; place multiple inline icons using rendered character indices and tolerate missing assets. Existing localization substitution is retained. | `ResolutionImage.cs`, `TextPic.cs`, `LabelAlias.cs` |
| 10 | Advanced settings opens the shared title-screen options and returns to the previous dialog. Volume sliders live in Audio; control size lives in Accessibility. | `SettingsDialog.cs`, `TitleScreen.cs` |
| 11 | Shared scroll handling applies to perks, achievements, seals, and moves as well as the shop. | `SFScrollRect.cs`, `TableViewScroll.cs`, `ItemsScroll.cs` |
| 12 | Correct horizontal/vertical size access, cancel competing scroll tweens, clamp wheel movement, and calculate drag-settle duration before resetting velocity. | `TableViewScroll.cs`, `TableView.cs`, `ItemsScroll.cs`, `SFScrollRect.cs` |
| 13 | Add draggable, automatically hidden desktop scrollbars to table views; internal scrollbar synchronization does not invoke user callbacks. | `DesktopScrollbars.cs`, `TableView.cs`, `SFScrollRect.cs` |
| 14 | Resolve the active challenge description before fight initialization and normalize its initial text alpha. | `FightList.cs`, `ContentTourChallBoss.cs` |
| 15 | Retain the yellow damage transition when the final health bar is lost, including a one-shot. | `UnderworldRaidLifeBarTransition.cs` |
| 16 | Critical shake defaults to 65% of original strength; persistent Accessibility slider applies to normal and interpolated rendering. | `AccessibilitySettings.cs`, `Camera.cs` |
| 17 | Commit a selected lethal hit reaction before round-end processing can replace the pending animation. | `SelectAnimation.cs` |
| 18 | Preserve the new-item flag after a first quest/boss equipment grant and refresh the live menu badge. | `QuestActionGiveItem.cs` |

Owned UI helpers are under `Assets/Scripts/Eclipse/UI/`; recovered classes remain in `Assets/Scripts/Assembly-CSharp/`. Existing sound-volume, control-size, and controller-layout methods received descriptive inferred names with the required best-guess comments; their callers were updated.

## Verification

- All four managed projects built successfully with `dotnet build ... -v:q /clp:ErrorsOnly`: Eclipse.Runtime, Assembly-CSharp-firstpass, Assembly-CSharp, and Assembly-CSharp-Editor. Final builds reported 0 errors; main/editor emitted 134/15 warnings. Plain MSBuild could not resolve the installed SDK, so the SDK-aware dotnet build was used.
- Unity 6000.6.0f1 imported and compiled the C# changes successfully.
- `Tools/TestUnderworldRuntime.ps1`: 1,282 assertions passed.
- `Tools/VerifyRuntimeUiFixes.cs`, executed with `unity command eval_file --file Tools/VerifyRuntimeUiFixes.cs --json`: 21 native Unity assertions passed. Covers final-bar transitions, actual recovered split sprites and idempotent layout, wheel movement, scrollbar interaction, qualified inline textures, multiple-icon placement, and removal of obsolete icons. The fixture creates temporary objects and destroys them afterward.
- `git diff --check -- Assets/Scripts Tools`: passed.
- `Tools/AuditUnderworld.py` ran but reports 40 missing loose raid-art frame references. All 40 parent atlases appear in the packaged-art catalog. The audit does not inspect their bundled frame contents; this is not a clean art-validation pass.

## Verification limits

No complete campaign/gamepad playtest was performed. Native fixture success is not end-to-end confirmation of all 18 reports. In particular, visually confirm the dojo seam at multiple resolutions, challenge previews, controller visuals, boss-reward badge, and lethal reactions in actual fights. Dialogue loading improvement follows removal of the broad atlas load; no before/after frame-time benchmark was recorded. No native sprite geometry was edited or reimported.

## Follow-up: logo alignment and startup options

The user's follow-up screenshot required a further four authored pixels of upward adjustment to `Logo.right`, scaled with its UI size. The project editor log identified null roster access in music-volume saving and control-size lookup before campaign initialization. Audio settings now persist volume and mute independently in PlayerPrefs, safely update the roster when available, and reapply saved preferences after roster audio initialization. Control size similarly persists independently of a loaded roster while retaining legacy profile fallback until explicitly changed.

Managed compilation and Unity compilation passed. `Tools/VerifyStartupOptions.cs` passed 10 native checks with no profile loaded, covering both volume sliders through mute/unmute, preference reapplication, and control-size toggling. It restores the original preferences and audio state afterward. The 21 existing native UI checks also passed. The final logo placement still needs visual confirmation in the loading screen.
