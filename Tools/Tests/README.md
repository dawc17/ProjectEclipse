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
python Tools/Tests/Presentation/TestVersusTransition.py
```

The versus-transition suite checks native loading artwork ownership and the
unchanged VS completion timer for local, online, replay, training and campaign
paths with controlled Unity dependencies. It does not render the transition or
run a game playtest.

`pwsh -NoProfile -File Tools/Tests/Presentation/TestVersusPreviewNative.ps1`
uses the matching Unity editor in an isolated project with graphics enabled. It
checks the production fighter preview's animation ticks, changing pixels and
transparency across scene unloading, UI visibility and world/texture cleanup.
Fighter meshes are controlled animated quads; this is native rendering validation,
not a multiplayer game playtest.

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

`Modding/TestModExtensions.ps1` runs the production Lua runtime and script-session
lifecycle against controlled asset/combat/UI sources. It covers typed framework
services, capability/dependency/version checks, isolated data, startup rollback,
execution limits, migration restrictions and the shipped Focus framework/add-on
with saved-state reload. It does not render a native fight or perform a game playtest.

`Modding/TestModExtensionsUnity.ps1` uses the matching Unity editor with graphics
enabled in isolated Play Mode. It executes the shipped Focus pair through the
production script session, renders its production HUD view with the recovered
font, checks changing pixels and teardown, and preserves serialized state through
mod removal/reinstallation. Contacts, the asset host and unused artwork/scroll
paths are controlled. Its PNGs/XML/logs stay in `Temp/`; full-game combat,
native menu/profile integration and disk-crash acceptance remain separate.

PvP balance checks are `Combat/TestPvpBalance.ps1` (strict JSON, immutable rules,
hashing, inheritance, nonlethal chip and recovery with controlled native
dependencies) and `Combat/TestPvpHealthRuntime.ps1` (compiled recovered health
setters, pool cloning/reset, production rollback policy and state hash).
The compiled health runner and Underworld runner accept `-AssemblyDirectory`
and `-UnityManagedDirectory` overrides for the matching compiled game/editor
references. `Presentation/TestPvpHealthBarNative.ps1` renders the production
recovery component and native skew mesh in an isolated Unity project, with
controlled health and atlas-base dependencies. None is a full online playtest.

## Native validators

`Runtime/TestUnity6Workflows.ps1` uses the matching Unity 6.6 editor in an isolated
project to test input devices across Play Mode frames, modern UI modules and
native Build Profile/Multiplayer Play Mode scenario serialization. It does not
perform a full game build or multiplayer fight; see
[the workflow guide](../../Docs/Engineering/UNITY_6_WORKFLOWS.md).

`Validate*.cs` and `Verify*.cs` are fixtures invoked by their paired runners or
through an explicit Unity editor command; they are not imported into the main
project automatically. Follow the runner's instructions for fixture setup.
Managed checks and static audits do not prove native sprite import, rendering
or gameplay. Report native validation and playtests separately.
