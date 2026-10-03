# Local Multiplayer

Eclipse's first multiplayer mode is **Local Versus**. From the title screen, open
**Multiplayer** to configure a match on one PC. In the Editor, start from
`Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity` to reach the title.

The current selection offers four weapon loadouts: **Unarmed**, **Knives**,
**Staff**, and **Katana**. Arenas are **Dojo**, **Autumn**, and **Bamboo grove**.
Both fighters use isolated standard loadouts so a local match does not alter the
campaign inventory or equipment.

Input can be configured as **keyboard + gamepad 1** or **gamepad 1 + gamepad 2**.
The lobby lets you choose first to **1, 2, or 3 wins**. Rounds currently use a
**99-second** timer. The local flow includes pause/resume, rematch, and return to
the title screen.

Keyboard play uses the saved campaign movement and attack bindings. Defaults are
**WASD** to move, **O** to punch, and **P** to kick. Controller bindings are also
shared with the existing controller settings; the defaults are **X / Square** to
punch and **A / Cross** to kick. Use the D-pad or selected movement stick to move.
Press **Escape** or **Start / Options** to pause or resume. A Start button assigned
to a combat action keeps that assignment; use Escape or the HUD pause button in
that case. Losing focus or disconnecting an assigned controller pauses the match.
Reconnect the required controllers and resume manually.

Equal-health timeouts and simultaneous knockouts replay the round without adding
to either score. Other timeouts award the round to the fighter with more health.

Local Versus is isolated from campaign progression. It does not award campaign
rewards or dispatch campaign story/combat mod callbacks. Mod content projection
remains loaded, so this does not mean every mod is globally disabled while a local
match is active.

The local bootstrap works from a temporary clone of the profile data. Profile
changes and mod-state migrations made during Local Versus are discarded instead
of being saved back to the campaign profile. The save guard also covers delayed
authentication/save callbacks after returning to the title screen.

Online versus is implemented with room discovery, direct connections, and relay
fallback. New rooms allow simultaneous fights, and members can spectate ongoing
matches. See [Online Versus](Engineering/ONLINE_VERSUS.md) for the current network,
loadout, replay, and verification details; the local-mode description above records
the initial recovery implementation.

## Adjustable balance

Choose a JSON balance preset with **BALANCE** in the lobby or training setup.
Open **SF2 > Multiplayer > PvP Balance** in Unity to edit, duplicate, import or
export presets, preview category/equipment/move inheritance and restart training
with saved changes. Defaults retain the 0.5 hit / 0.25 blocked multipliers, make
blocked damage recoverable and prevent blocked strikes from knocking out.
Landing hits or making the opponent block restores existing grey health at the
configured rates. Active matches and rematches retain their captured rules.
See [the profile guide](Modding/src/content/docs/guides/pvp-balance.md) for all
fields, defaults, paths and online/replay compatibility, and
[implementation and checks](Engineering/PVP_BALANCE.md) for verification limits.

## Verification

`Tools/Tests/Combat/TestLocalVersusRules.ps1` and `Tools/Tests/Combat/TestLocalVersusInput.ps1` compile the
production configuration/scoring and input classes in isolated harnesses. They
cover 38 configuration/scoring checks and 14 device-routing/state checks.

`Assets/Editor/ValidateLocalVersusNative.cs` exercises the real Unity title boot,
local lobby, all three native arenas, independent fighter input and movement,
player-two knockout wins, both timeout winners, drawn timeouts and double
knockouts, pause overlays, rematches, result UI and return to title. Its final run
passed **46 native checks** on Unity 6000.6.0f1, including disk-save checks at local
startup, forced save, match completion, and delayed callbacks after title return.
Both `users.xml` and `users_backup.xml` retained their contents and write times.
Evidence is written to `Temp/LocalVersusNative/`.

The native validator supplies control events and temporarily disables device
disconnect monitoring. It does not emulate physical controller drivers. Physical
gamepad input, hot unplugging/reconnection and controller pause shortcuts still
need a hands-on playtest. An Xbox controller was detected by the end of validation,
but the automated checks supplied control events instead of pressing its buttons.
