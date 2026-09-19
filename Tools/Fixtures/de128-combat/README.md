# DE128 native combat acceptance fixture

This engineering fixture is copied beside the real DE128 package in an independent
Unity project. It is not shipped in the DE128 mod. Its Lua declares a test arena,
Jian-equipped opponent and fight through the public API. The editor harness boots
the game, starts that fight and supplies a native double-tap/Forward `KeyData`
sequence through `Model.PlayAnimation(KeyData)`, which dispatches normal selection
events. Directions are mirrored for a left-facing fighter, and the synthetic
keys are released after selection. It does not choose the resulting move by name.

Run from the repository root with the matching editor:

```powershell
python Tools/TestDE128CombatNative.py --unity-editor F:/UnityInstalls/6000.6.0f1/Editor/Unity.exe
```

The launcher copies project inputs independently using the existing form-fixture
preparer. It sets a separate product name for save isolation and preserves logs
under the printed `Temp/FormNative-*` directory. It does not open a fight or
change saves in the working editor. Fresh Unity imports can take several minutes.

After a run has ended, retain the cache and refresh specific changed sources:

```powershell
python Tools/TestDE128CombatNative.py --unity-editor F:/UnityInstalls/6000.6.0f1/Editor/Unity.exe --reuse-native Temp/FormNative-EXISTING --sync-native-source Assets/Scripts/Assembly-CSharp/Model.cs
```

Reuse verifies the marked clone and rejects an active editor lock. It always
refreshes this fixture and its editor harness. Other source refreshes must be
explicit repository-relative files under `Assets` or `Mods/de128`. Prior inputs
are backed up with hashes, and rerun logs go into a new `DE128Runs/Run-*` directory.

Passing requires real package initialization, Jian's patched subtype, native move
selection, four attack intervals and timed sound actions, completion, an active
rig, all authored attack edges bound, and continued simulation without captured
exceptions after body readiness. Both fighters are made
immortal and enemy AI is disabled after readiness to isolate the input/animation
case. This does not prove physical keyboard/gamepad handling, AI choices, hit
contact, audible playback, profile rendering, shop completion or save continuity.

The harness also compares the ten restored weapons with the archive after boot:
native damage, subtype, level, upgrade level, price, pack membership, exact default
enchantment name/aspect, and real model-text/sprite loading. Moon Fans must preserve its absent initial damage and match the archived
item after applying its first native upgrade to isolated clones. Archive XML is read only by
this engineering harness; the shipped mod has no XML loader or definitions.

It also validates eight restored armor/helm/ranged/magic definitions against the
archive, including presence/absence of every initial stat, levels, prices,
upgrade templates, default enchantment aspects and loadable model/icon assets.
This does not exercise their combat actions or prove shared DE move parity.

The shared-move checks inspect all five native patches and evaluate the actual
MassBomb/LightningArrow conditions with Stun absent and present. Private native
move objects also exercise apply-before-initialization, apply-after-initialization,
and repeated rollback for interval ends, hit reactions and sound scheduling.
These checks do not mutate player saves or prove visual preview parity.

Inert fixture moves additionally exercise Lua-authored projectile actions. After
boot, the harness compares native CreatePlayer equipment-copy records, charge and
delete scheduling against Sphere1 archive entries and checks an owned starting-move
handle. These definitions are never selected in the Jian fight. This is actual
Unity parsing, not a live projectile lifetime/contact test.

The inert child move now also supplies actor-name and charge conditions, velocity,
acceleration, velocity preservation and no-recharge flags. The harness evaluates
matching/nonmatching conditions and checks parsed motion/flag values. It still does
not select or simulate this child move, so trajectory/contact acceptance is separate.

## Sphere1 integration checkpoint (in progress)

Version 0.14.0 adds Minor Charge of Darkness and nine Lua-authored Sphere1 combat
and shop moves, with three unchanged recovered animation binaries. No runtime XML
loader is used. Complete archive comparisons passed (1,004 combined move checks),
and 2,509 foundation checks passed.

The latest isolated Unity run, Run-cdsq9ulc, exited 1: native casting, inherited
projectile equipment, charge consumption and deletion were observed, but the
harness did not observe the middle-flight phase. This is an unresolved acceptance
failure, not a completed spell integration. Numerical damage, visible effects,
shop preview and purchase/save acceptance remain open. Four other missing spell
graphs also remain pending. This checkpoint is committed at the user's request.

## Sphere1 native lifecycle follow-up

The failed flight expectation was caused by the preceding Jian attack closing the
fighters' gap: the projectile hit during its damaging startup and executed its
Strike deletion action before reaching flight. The harness now restores long-range
spacing before casting and listens to native animation-selection events rather
than relying solely on editor-update polling. No production graph or engine
behavior was changed to force a pass.

Isolated Unity run `Run-kejilqi4` exited 0. It observed native Magic-input selection,
exactly one inherited-equipment child, Sphere1Start then Sphere1Middle, one consumed
charge and child deletion/removal. The prior Jian and restored-equipment checks also
passed. This resolves the preceding failed middle-flight check. Numerical damage,
wall-miss cleanup, audible/visual output, shop previews and purchase/save acceptance
remain open; this is not full spell parity. The mod continues to author its graph
in Lua and does not load archived XML at runtime.
