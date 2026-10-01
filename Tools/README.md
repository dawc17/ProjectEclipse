# Eclipse tools

Run commands from the repository root unless a tool's README says otherwise.
Tools stay outside Unity's `Assets/` tree. Generated checks and isolated Unity
fixtures normally go under ignored `Temp/`; each tool documents its own inputs.

| Directory | Purpose |
| --- | --- |
| [Tests](Tests/README.md) | Managed regression runners, C# fixtures and Unity validators |
| [Audits](Audits/README.md) | Content, archive and reference audits; report/refresh modes are documented |
| [Recovery](Recovery/README.md) | Extraction, import, repair, content generation and historical migrations |
| [Saves](Saves/) | Windows save-profile preparation, backups and switching |
| [UnityUpgrade](UnityUpgrade/) | Historical editor-upgrade preparation and finalization |
| [Animation](Animation/README.md) | Character authoring, animation conversion and its focused tests |
| [AssetPacker](AssetPacker/README.md) | TAR/LZ4 content archive commands |
| [ModdingEditor](ModdingEditor/README.md) | VS Code extension, Lua definitions and mod starter templates |
| [ModZipInstallerTests](ModZipInstallerTests/) | Standalone mod ZIP installer tests |
| [NetplayTests](NetplayTests/) | Standalone online-versus core and room-server tests |
| [Fixtures](Fixtures/) | Shared content fixtures |
| [SpriteRepairProject](SpriteRepairProject/README.md) | Isolated native sprite repair and rendering fixtures |
| [LegacyServices](LegacyServices/README.md) | Archived service source, metadata and reviewed removal manifest |
| [SwitchPvpRecovery](SwitchPvpRecovery/README.md) | Switch PvP reverse-engineering workflow |
| [DiscordResearch](DiscordResearch/README.md) | Discord export research utility |

## Common checks

```sh
python Tools/Tests/Runtime/TestAuditDECorpus.py
python Tools/Tests/CharacterForms/TestCharacterForms.py
python Tools/Audits/AuditAssemblyCleanup.py
```

Character-form tests use the matching installed Unity editor's bundled compiler
and a .NET 10 runtime without launching Unity. See the [test guide](Tests/README.md)
for the separate PowerShell and native-editor requirements.

Content repair/import scripts can rewrite repository assets. Read the relevant
script's help and [engineering guidance](../Docs/Engineering/README.md) first.
Native sprite rebuilding must follow the
[Unity-native recovery workflow](../Docs/Engineering/SPRITE_NATIVE_REBUILD.md);
retain its generated fixture assets and every existing `.meta` GUID.

`python-packages/`, dependency installs, caches and build outputs are local,
ignored directories. The shared `python-packages/` location remains here for
recovery scripts that use locally installed UnityPy or Pillow.

Player builds, launcher publishing and build patches live in
[BuildScripts](../BuildScripts/README.md). Public modding documentation lives in
[Docs/Modding](../Docs/Modding/README.md). Historical recovery tools and archives
remain available; their presence does not make them required build steps.
