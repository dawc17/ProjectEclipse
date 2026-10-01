# XML audit refresh: October 1, 2026

The audit's failure preceded the repository reorganization. Running the original
script against its original ledger reproduced the same drift. Two causes were
confirmed:

- The September 10 ledger recorded Windows CRLF source-byte hashes. Converting
  the current LF bytes to CRLF matched 292 previously differing hashes.
- Semantic differences changed in 69 location XML paths. The later scenery
  recovery changed coordinates, atlas/layer definitions and base location
  coverage. All semantic drift was in the locations domain; the named item,
  perk, move, achievement and quest feature index is unchanged.

The location changes are recorded in commit `a57454dc` (September 24,
"Upscaled locations and more") and subsequent location recovery work. They
belong to the existing checkout, not this cleanup. No source XML was edited to
make the audit pass.

## Evidence and hash contract

The [original September 10 ledger](https://github.com/DAWC17/ProjectEclipse/blob/8f07a3660438c7bdcf1ee281163c6ca3a9d12fc4/Mods/DE_XML_DELTA_LEDGER.json.gz)
and its companion files remain in Git history. The
[original gap review](DE_XML_API_GAP_AUDIT.md) retains its historical counts and
capability assessments. The current generated files describe the present
checkout; refreshing them does not verify new API capabilities or DE intent.

Ledger schema 2 records SHA-256 source hashes after replacing CRLF with LF.
Other bytes, including BOMs, remain exact. This makes the evidence independent
of Git checkout line endings while preserving detection of other byte changes.
XML delta comparison still ignores indentation, comments and attribute order,
preserves child order, and keeps numeric strings exact.

| Inventory | September 10 baseline | Refreshed checkout |
| --- | ---: | ---: |
| Base XML files | 164 | 171 |
| Archived DE XML files | 212 | 212 |
| Union of paths | 219 | 219 |
| Shared files with differences | 83 | 121 |
| Shared files equal | 74 | 43 |
| DE-only files | 55 | 48 |
| Base-only files | 7 | 7 |
| Structural delta records | 44,955 | 46,960 |

More deltas do not mean more DE features. Later base scenery recovery can
introduce differences from the older archive or make previously DE-only paths
shared. Determine intent from source history and runtime evidence.

## Repeatable checks

```sh
python Tools/Tests/Runtime/TestDEXmlAudit.py
python Tools/Audits/AuditDEXmlApi.py
python Tools/Audits/AuditPhase3Configuration.py
```

The controlled hash check verifies LF/CRLF equivalence and detection of a real
attribute change. Audit `--write` regenerates the current ledger, inventory,
feature index and file coverage after reviewing drift. Domain coverage prose
retains the earlier review and must not be used as a current API inventory.
