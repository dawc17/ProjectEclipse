# Raid navigation layout regression

The raid buttons were created as direct children of the map canvas during
`MapScene.Init`. `Scene.Awake` subsequently calls `WideScreenController.Run`.
For screens wider than 16:9, it applies left/right inset offsets to every direct
canvas child, assuming each child stretches across the canvas. On a fixed-size,
right-anchored button this overwrites its position and gives it negative width.
The object survives in the hierarchy but its image disappears at the right edge.
Repairing its sprite alone does not fix this layout bug.

`RaidMapControlsLayout` supplies one stretched `RaidControlsLayer`. Widescreen
insets apply to that layer; the entry/return button, tier arrow, and power-mode
control remain children with their authored sizes and offsets. Visibility rules,
click handlers, save progression, and the global widescreen controller are unchanged.

## Regression test

The isolated sprite repair project also runs a layout test using copies of the
actual production helper and widescreen controller. Refresh the copies first:

```powershell
Copy-Item Assets/Scripts/Assembly-CSharp/WideScreenController.cs Tools/SpriteRepairProject/Assets/NavigationTestRuntime/WideScreenController.cs
Copy-Item Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Map/RaidMapControlsLayout.cs Tools/SpriteRepairProject/Assets/NavigationTestRuntime/RaidMapControlsLayout.cs
```

Run Unity 2019.4.41f2 in batch mode on `Tools/SpriteRepairProject`, executing
`ValidateRaidNavigationLayout.Run`. It expects the three navigation sprites
generated from `Temp/raid-navigation-sprite-rebuild.json` (entry=0, return=1,
tier arrow=2). The test invokes the real widescreen inset method, reproduces
the old negative-width failure, and checks the new controls at 4:3, 16:9, 21:9,
and 32:9, including a repeated layout pass. It does not launch the game or load
a profile. Results: `Temp/raid-navigation-layout-validation.log`.
