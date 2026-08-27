# SF2 script-name recovery

This directory records the repeatable work used to recover names in the
AssetRipper export. The changes are applied directly to the C# sources under:

- `Assets/Scripts/Assembly-CSharp`
- `Assets/Plugins/Assembly-CSharp-firstpass`

This is not a cosmetic list of suggested names. Accepted identifiers are
replaced in the source contents, top-level script files are renamed, and an
existing Unity `.cs.meta` file is moved with its script so its GUID is retained.
AssetRipper did not emit a `.meta` file for most scripts; the mapper only moves
one when it exists and never fabricates a GUID.

## Current checkpoint

As of 2026-08-25:

- `Assembly-CSharp`: 239 initially obfuscated top-level filenames, 34 remain.
- `Assembly-CSharp-firstpass`: 493 initially obfuscated top-level filenames,
  142 remain.
- 556 top-level script filenames have therefore been recovered across the two
  assemblies.
- 2,317 unambiguous reviewed symbol mappings are currently loaded by the
  application script.
- More than 21,000 exact identifier occurrences have been changed in the C#
  contents over the completed passes.
- 9 conflicting member mappings are deliberately withheld.
- 24 firstpass type mappings are deliberately withheld because stripping their
  original namespaces made their clean leaf names collide.

The remaining all-capital occurrences are not equivalent to the number of
unknown types. Most are fields, methods, parameters, and local variables. Local
variable names are not present in the cross-build metadata and often cannot be
recovered authoritatively.

## Evidence used

The principal comparison data lives outside this exported project under
`/home/czapla/RE/decompilation/generated`. The recovery uses:

1. Android Mono metadata and original IL fingerprints: type kind, base type,
   field order and types, method signatures, referenced fields and calls,
   constants, and string literals.
2. The unobfuscated Switch IL2CPP metadata and dump: clean type/member names,
   namespaces, field layouts, method signatures, and native method RVAs.
3. The generated Android-to-Switch cross-build map in
   `cross_build_map/`, including accepted seed anchors and authoritative member
   mappings.
4. The earlier unpoisoned firstpass candidate snapshot in
   `.cross_build_map.pwIZKx/`. This matters because later experiments included
   some bad manual anchors that must not be allowed to propagate.
5. Direct behavioral evidence in Android method bodies. Examples include XML
   discriminator strings, quest/action factory cases, parser literals, enum
   value sets, diagnostic messages, singleton usage, and concrete call sites.
6. Version-adapted semantic recovery when a class exists in this Android build
   but was merged, split, or removed in the later Switch build. These names are
   accepted only when the role is explicit in call sites; the evidence is
   written beside every such entry in `manual_confirmed_types.tsv`.

No internet lookup or name guessing from alphabetical proximity was used.

## Acceptance rules

A proposed mapping is applied only when it resolves to one valid C# identifier.
Automatic type mappings use the high-confidence/anchor boundary from the
cross-build reports. Manual type mappings require recorded structural or
behavioral evidence. Member mappings use authoritative or regenerated
high-confidence rows.

The application step is intentionally conservative:

- It performs whole-identifier replacements, never substring replacements.
- It retains all competing targets for a token and skips the token if they
  disagree.
- It skips a file rename if the destination already exists.
- It skips a rename if more than one source file has the same obfuscated stem.
- It does not flatten two different namespaced clean types to the same global
  name. This is the reason for the held firstpass `Encoder`, `Decoder`,
  `GZipStream`, `DeflateStream`, delegate, and similar pairs.
- Compiler backing-field names such as `<Name>k__BackingField` are normalized to
  a legal source identifier such as `_Name` before application.
- Hand-reviewed corrections override an older bad mapping without weakening
  the general confidence threshold.

One useful example of the collision check is `JOPONFMDIKE`. Its
`uint min`, `uint max`, `long value`, XML constructor, and both owning
collections match `CharProgLevel` exactly. A first semantic label of
`MinMaxValue` collided with a different, genuine float-based `MinMaxValue`
already in the project. The metadata comparison identified the exact
`CharProgLevel` type, and only the affected constructor and collection uses
were corrected before the script was renamed.

## Files in this directory

- `manual_confirmed_types.tsv`: reviewed top-level type mappings and a concise
  evidence statement for each one. This is the main human audit trail.
- `manual_corrections.tsv`: corrections for known bad or superseded mappings.
- `derived_authoritative_members.tsv`: member mappings regenerated after adding
  reviewed type anchors.
- `reviewed_symbol_application.tsv`: the complete last application report,
  including applied symbols and skipped conflicts/collisions.
- `apply_reviewed_maps.py`: loads all reviewed sources, replaces identifiers in
  both assemblies, renames scripts, and moves existing `.meta` files.
- `generate_manual_member_map.py`: stages reviewed type anchors, invokes the
  cross-build member matcher, and exports only authoritative/high-confidence
  member rows into this project.
- `analyze_quest_factory.py`: extracts quest-action factory evidence.
- `analyze_remaining_types.py`: shows body literals and ranked clean-build
  candidates for each remaining obfuscated main-assembly type.

The subsystem reports consumed by the mapper live in
`/home/czapla/RE/decompilation/generated/system_maps`.

## Reproducing and auditing

From the project root, preview the reviewed mapping pass with:

```bash
python3 Deobfuscation/apply_reviewed_maps.py --dry-run
```

Apply newly reviewed rows with:

```bash
python3 Deobfuscation/apply_reviewed_maps.py
```

An immediate second dry run should report zero source changes, zero identifier
replacements, and zero script renames. That is the idempotency check.

To review the unresolved main-assembly types:

```bash
python3 Deobfuscation/analyze_remaining_types.py
```

To regenerate member mappings after adding reviewed type anchors:

```bash
python3 Deobfuscation/generate_manual_member_map.py
python3 Deobfuscation/apply_reviewed_maps.py --dry-run
```

The generator uses temporary staging under `/tmp`; it does not rewrite the
source map under `/home/czapla/RE`.

## Work still outstanding

- Resolve as many of the 34 main and 142 firstpass top-level names as the
  available evidence supports.
- Recover high-confidence members inside already renamed classes. Many locals
  and parameters will necessarily remain obfuscated.
- Resolve namespace-erasure collisions only by restoring the correct
  namespaces and qualified references; simple leaf-name renaming is unsafe.
- Audit textual Unity scenes, prefabs, and assets for serialized field names
  before treating the project as editor-ready.
- Run structural checks and the strongest practical compile check after the
  final mapping pass. An AssetRipper export may still have unrelated Unity
  version/decompiler build errors.

