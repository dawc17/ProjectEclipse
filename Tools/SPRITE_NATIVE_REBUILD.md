# Unity-native sprite recovery

The handwritten Sprite YAML exporter caused a Unity 2019 native crash in
`SpriteUtilityBindings::GetSpriteUVs` during thumbnail import. It declared
render-data version 3 while placing UV0 in legacy vertex channel 3 (modern
channel 3 is color; UV0 belongs in channel 4). Managed compilation and file
existence checks do not detect this failure.

**Do not restore SPRITE_YAML or patch serialized vertex layouts manually.**
Use the matching Unity version to create and serialize sprites, then test the
native UV and preview paths. Existing sprite GUIDs and original PNGs stay intact.

## New extraction workflow

`ExtractRaidArt.py` still recovers textures/atlas metadata, but new standalone
UI sprites are queued in `Temp/raid-native-sprite-queue.json` rather than emitted
as handwritten `.asset` files. Finish those queued assets with:

1. `python Tools/RebuildUnsafeRaidSprites.py prepare_pending`
2. Run the matching Unity editor in batch mode:

   ```powershell
   & 'F:\UnityInstalls\2019.4.41f2\Editor\Unity.exe' -batchmode -nographics `
     -projectPath 'F:\SF2DE\SF2DE\ExportedProject\Tools\SpriteRepairProject' `
     -executeMethod RebuildRecoveredSprites.Run `
     -repairManifest 'F:\SF2DE\SF2DE\ExportedProject\Temp\raid-sprite-rebuild.json' `
     -logFile 'F:\SF2DE\SF2DE\ExportedProject\Temp\raid-sprite-native-rebuild.log'
   ```

3. Require a fresh `[SpriteRebuild] PASS` and a completed `.rebuilt.json` report.
4. `python Tools/RebuildUnsafeRaidSprites.py install`
5. Run `SF2 > Validate Rebuilt Raid Sprites` in the main Unity project. A batch
   run of `DevSpriteCrashValidator.Run` can do the same. Omit `-nographics` to
   exercise actual thumbnail rendering, not just native UV access.

The isolated project never imports the broken source `.asset` files. It reads
descriptors and PNGs, creates real sprites, serializes/reimports them, and checks
that their UVs cover the intended atlas rectangle. Installation remaps texture
GUIDs to the original PNGs, restores original sprite names and preserves each
existing `.meta` GUID. Pre-repair records are kept outside Assets under
`Temp/unsafe-raid-sprites-before-native-rebuild` and must not be reimported.

The `prepare` mode specifically identifies the old 149-record crash regression;
it is not needed after those files have been repaired.
