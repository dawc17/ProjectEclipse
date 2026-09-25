# DE asset corpus

The owner designated this archive as the authoritative source for **all DE mod
content**, including assets and gameplay XML, on 2026-09-19:

[Assets.7z on Google Drive](https://drive.usercontent.google.com/download?id=17UA-TnslzXws0M6hZs3qEzpH3skn8Pxy&export=download&authuser=0)

The owner also authorized implementing content previously blocked by missing
assets. Revisit those gaps after inventorying the archive. Do not assume the old
missing-asset limitations still apply.

## Acquisition status

**Not downloaded.** Google identifies the file as `Assets.7z` (1.6 GB), but its
confirmed download returned an HTML **Quota exceeded** response on 2026-09-19.
No archive checksum or content inventory can be claimed yet. A local file path or
alternate download location has been requested.

A later confirmed-download retry also returned quota HTML (2,009 bytes), not a
7z signature. Repeated retries are not evidence of a usable corpus.

Reserved local storage, ignored by Git and outside Unity imports:

`ResearchSources/DECorpus/17UA-TnslzXws0M6hZs3qEzpH3skn8Pxy/`

The failed response is stored there as `quota-response.html`, not as an archive.
Keep the original archive permanently with its SHA-256 and inventory after a
successful download. Extract into a separate directory under this storage root;
do not bulk-copy the corpus into Unity's `Assets` or overwrite existing GUIDs.

The owner subsequently requested continuing production from the available repository
data while deferring this download. Historical XML may be used for continued work,
but reconciliation against the designated corpus remains required when accessible.

A separate local owner asset drop at `ResearchSources/de128_assets/` contains
gameplay XML and exported art used by recent DE128 work. Its reviewed
`gamedata/raid_stages_default.xml` has SHA-256
`d012a1f47418def617d375743864e2256b00f4fd709f6785da45aa67a8c3fa7c`;
the Underworld generator pins that exact file. This drop is not verified to be
the designated 1.6 GB `Assets.7z`, so its local reconciliation does not close
the full-corpus audit below.

Blackness's Grasp was reviewed against this drop's
`gamedata/animations/moves.xml` (SHA-256
`7F4D181848DC3F430BA4AF1A8A024905D629FC77C499FF74832710C65D07B818`).
Its caster priority is 200 there; the older `Assets/DExml` move file says
1000. The two selected animation binaries match their packaged native copies
byte for byte; see Step 65 in `PRODUCTION.md` for their hashes and live checks.
This review does not establish equivalence with the designated `Assets.7z`.

## Reconciliation requirements

- Treat `Assets/DExml` and earlier production comparisons as historical evidence,
  not as authority over this corpus.
- Compare every previously used DE XML and bundled binary against the archive.
  Record changed, missing and additional files; compare XML semantically as well
  as by hash so formatting changes do not hide gameplay changes.
- Reflect changed definitions in Lua and their acceptance checks. Never make the
  shipped DE mod load XML. Keep reusable engine support in C#.
- Source DE art, animation and audio from this archive. Retain source-relative
  paths and hashes for imported files; use existing Unity import workflows.
- Reassess blocked assets, restored-content gaps and presentation acceptance from
  the actual inventory. Do not call corpus reconciliation complete until the
  existing implementation and remaining scope have both been checked.

## Read-only audit tool

After acquiring, validating and extracting the archive, run from the repository:

```powershell
python Tools/AuditDECorpus.py --corpus ResearchSources/DECorpus/17UA-TnslzXws0M6hZs3qEzpH3skn8Pxy/extracted --output ResearchSources/DECorpus/17UA-TnslzXws0M6hZs3qEzpH3skn8Pxy/audit-001.json
```

The tool inventories every file with its source-relative path, byte size and
SHA-256. It compares historical `Assets/DExml` by relative-path suffix and all
bundled `Mods/de128/assets` by filename, recording candidates explicitly.
Multiple candidates are **ambiguous**, even if one has identical bytes. Use
`--xml-root` to select an inspected XML subtree inside the extracted corpus.
Missing matches are evidence to investigate, not permission to substitute an old
file. Alternate archive layouts may require explicit matching work.

XML comparisons ignore indentation and attribute order, but preserve child order,
repeated elements, attribute values and meaningful text whitespace. Reports retain
every positional difference rather than truncating after a few examples. Numeric
spellings and domain-specific equivalences are not guessed. DTD/entity declarations
and malformed XML require manual review. Additional XML paths are listed separately.

The audit never edits source assets, Lua or Unity metadata. Input links/junctions
are rejected. Reports must be new files outside the input trees, so prior evidence
cannot be overwritten. A report is an inventory/comparison, **not** proof that Lua
has been reconciled or that the game renders the assets correctly.

`python Tools/TestAuditDECorpus.py` passes eight controlled-fixture tests. The
actual corpus audit remains pending acquisition.
