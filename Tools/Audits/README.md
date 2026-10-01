# Content and recovery audits

Run from the repository root. Checks verify recorded contracts or dependencies;
they do not prove Unity rendering, gameplay acceptance or complete DE parity.
Archived DE XML is evidence, not the authority for base-game behavior.

## Core and packaged content

| Tool | Inputs and result | Writes and requirements |
| --- | --- | --- |
| [AuditAssemblyCleanup.py](AuditAssemblyCleanup.py) | Reviewed assembly-removal manifest, archived source/metas and current serialized references | Read-only verification; does not infer additional dead code |
| [AuditNativeContent.py](AuditNativeContent.py) | Runtime art catalog and installed payloads; validates addresses, hashes and sizes | Read-only by default; `--refresh` rewrites catalog hashes after deliberate edits; `--deep` also verifies TAR/LZ4 through AssetPacker; `--root` selects the catalog directory |
| [AuditUnderworld.py](AuditUnderworld.py) | Installed raid XML, location params and real packaged sprite names | Does not edit assets; extracts/cache-writes under `Library/UnderworldAudit`; requires AssetPacker and actual installed bundle bytes |
| [AuditRecoveredTextureCompression.py](AuditRecoveredTextureCompression.py) | Source bundle texture formats plus recovered PNG import settings | Read-only report; `--bundle-root` defaults to local `ResearchSources/CDNBundles/downloads`, `--project-root` defaults to this directory; requires UnityPy |

## Archived XML and downstream evidence

| Tool | Inputs and result | Writes and requirements |
| --- | --- | --- |
| [AuditDEXmlApi.py](AuditDEXmlApi.py) | Current `Assets/vanillaXml` versus `Assets/DExml`; checks the full delta ledger and companions | Read-only by default; `--write` regenerates evidence under `Docs/Engineering/Modding`; schema 2 hashes normalize CRLF to LF; see the [refresh record](../../Docs/Engineering/Modding/XML_AUDIT_REFRESH.md) |
| [AuditPhase3Configuration.py](AuditPhase3Configuration.py) | Selected base/archive configuration and credits differences | Read-only by default; `--write` regenerates the classified engineering ledger; new internal/build sections require classification |
| [AuditDECorpus.py](AuditDECorpus.py) | Required `--corpus`, archived XML reference and bundled mod payloads; reconciles hashes, semantic XML and ambiguous paths | Required `--output` writes a new JSON report outside all input trees and refuses overwrite; `--xml-root`, `--reference`, `--bundled` select inputs; bundled default requires local `Mods/de128/assets` |
| [AuditDE128DojoLocations.py](AuditDE128DojoLocations.py) | Archived dojo choices, installed params and packaged sprite bundles | Read-only dependency check with verified temporary extraction; `--require` demands selected choices; requires AssetPacker |
| [AuditDE128EquipmentMoves.py](AuditDE128EquipmentMoves.py) | Archived item move consumers and canonical native moves | Prints evidence; no asset/mod writes and no gameplay equivalence claim |
| [AuditDE128SpellGraphs.py](AuditDE128SpellGraphs.py) | Archived spell/move dependencies and core graph vocabulary | Read-only by default; optional `--output` writes JSON evidence (prefer ignored `Temp/`) |
| [AuditDE128MissingEquipmentAssets.cs](AuditDE128MissingEquipmentAssets.cs) | Base/archive equipment and the running Editor's core asset provider | Read-only Unity editor evaluation; armor/helm/ranged/magic availability, not a standalone compiled program |
| [AuditDE128MissingWeaponAssets.cs](AuditDE128MissingWeaponAssets.cs) | Base/archive weapons and the running Editor's core asset provider | Read-only Unity editor evaluation; example invocation is recorded in the source |

## Historical migration research

[AnalyzeXmlMigration.py](AnalyzeXmlMigration.py) compares the older `Assets/xml`
layout with recovered C# and local APK enum evidence. It writes
`ResearchSources/XMLMigration/report.json` and `report.md` by default; there is
no read-only CLI mode. Its source/version assumptions describe the earlier
migration and are not the current base XML contract.

## Common checks

```sh
python Tools/Audits/AuditAssemblyCleanup.py
python Tools/Audits/AuditDEXmlApi.py
python Tools/Audits/AuditPhase3Configuration.py
python Tools/Tests/Runtime/TestAuditDECorpus.py
python Tools/Tests/Runtime/TestDEXmlAudit.py
```

Check installed bundles only when their LFS payloads and required tools are available:

```sh
python Tools/Audits/AuditNativeContent.py
python Tools/Audits/AuditUnderworld.py
```

Keep report refreshes separate from source repairs. Missing research files or
editor dependencies are verification limits, not a reason to invent recovered data.
Find import and repair workflows in the [recovery index](../Recovery/README.md).
