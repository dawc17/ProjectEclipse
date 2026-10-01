# Native sprite recovery fixture

This isolated project targets **Unity 2019.4.41f2**, as recorded in
`ProjectSettings/ProjectVersion.txt`. The main Eclipse project targets Unity 6.6.
Follow the [native recovery procedure](../../Docs/Engineering/SPRITE_NATIVE_REBUILD.md)
for manifest preparation, rebuilding, installation and thumbnail validation.

`Assets/Generated/` contains retained Unity-native sprites and textures.
`ValidateRaidNavigationLayout.Run` loads `sprite_0.asset` and `sprite_2.asset`
from that directory. These are fixture inputs as well as recovery outputs;
preserve the assets and every `.meta` GUID.

`RebuildRecoveredSprites.Run` consumes an external `-repairManifest`, creates
native assets and checks their reimported UVs. Generated filenames depend on
manifest order. Do not delete retained outputs or replace serialized sprite
layouts by hand as a cleanup step.
