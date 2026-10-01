# Regression tests and validators

Run these commands from the repository root. Runners and their C# fixtures stay
together here; generated sources, binaries, logs and isolated projects go under
ignored `Temp/`. Tool-specific suites in `Tools/Animation/`, `Tools/ModdingEditor/`,
`Tools/NetplayTests/` and `Tools/ModZipInstallerTests/` keep their existing locations.

| Directory | Coverage |
| --- | --- |
| [CharacterForms](CharacterForms/) | Model replacement, form transitions, animation bindings and transferred state |
| [Combat](Combat/) | Moves, projectiles, perks, rules and local versus |
| [Progression](Progression/) | Profiles, purchases, rewards, quests, story, tutorials and raids |
| [Modding](Modding/) | API contracts, content registration, Lua, UI and mod lifecycle |
| [Presentation](Presentation/) | Loading, title previews, locations, artwork, layouts and performance |
| [Runtime](Runtime/) | Assembly cleanup, archive audits, offline runtime and shared runtime checks |
| [DE128](DE128/) | Downstream DE128 content acceptance; may require local downstream inputs |
| [Shared](Shared/) | Common managed loader and stubs used across suites |

## Python entry points

```sh
python Tools/Tests/Runtime/TestAuditDECorpus.py
python Tools/Tests/Runtime/TestDEXmlAudit.py
python Tools/Tests/CharacterForms/TestCharacterForms.py
python Tools/Tests/CharacterForms/TestCharacterForms.py --case TestFormModifierTransfer
```

The corpus suite uses temporary controlled inputs. Character-form checks reuse
the extracted-C# assertions from the PowerShell runners and require the matching
installed Unity editor's bundled compiler plus a .NET 10 runtime. They do not
launch Unity by default. Use `--help` for compiler paths and explicit native runs.

The four fixtures repaired during the October 1 cleanup cover transition barriers
and interpolation call order, speculative body disposal, the current node enum,
and own-animation observation in versus/title AI readiness. These use controlled
multiplayer/rendering dependencies; passing them is not rollback or Unity acceptance.

The other `Test*Native.py` scripts prepare or run isolated editor fixtures; read
their help and docstrings for required inputs and execution behavior.

## PowerShell entry points

Use PowerShell 7 (`pwsh`) for managed-runtime checks. Individual scripts may also
require MSBuild, a .NET SDK, Visual Studio's compiler or a specified Unity editor.
For example, when MSBuild and the matching managed references are available:

```powershell
pwsh -NoProfile -File Tools/Tests/Progression/TestUnderworldRuntime.ps1
python Tools/Audits/AuditUnderworld.py
```

Select the runner for the subsystem you changed rather than invoking every
script. `Shared/LoadUnityManagedAssemblies.ps1` is the shared loader, not a test runner.
Some historical DE checks require downstream content or local research inputs.

## Native validators

`Validate*.cs` and `Verify*.cs` are fixtures invoked by their paired runners or
through an explicit Unity editor command; they are not imported into the main
project automatically. Follow the runner's instructions for fixture setup.
Managed checks and static audits do not prove native sprite import, rendering
or gameplay. Report native validation and playtests separately.
