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

Reserved local storage, ignored by Git and outside Unity imports:

`ResearchSources/DECorpus/17UA-TnslzXws0M6hZs3qEzpH3skn8Pxy/`

The failed response is stored there as `quota-response.html`, not as an archive.
Keep the original archive permanently with its SHA-256 and inventory after a
successful download. Extract into a separate directory under this storage root;
do not bulk-copy the corpus into Unity's `Assets` or overwrite existing GUIDs.

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
