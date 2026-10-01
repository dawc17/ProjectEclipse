# Modding engineering records

These records preserve design decisions, source evidence and verification limits.
The [public modding wiki](../../Modding/README.md) documents the implemented
contract; historical plans and acceptance records do not establish current support.
Installable content belongs under [Mods](../../../Mods/README.md).

| Topic | Records |
| --- | --- |
| Direction and scope | [DE parity target](DE_PARITY_TARGET.md), [implementation plan](DE_API_IMPLEMENTATION_PLAN.md), [roadmap status](ROADMAP_STATUS.md) |
| Runtime extension design | [Engine extensibility](MOD_ENGINE_EXTENSIBILITY.md), [short-form migration](SHORT_FORM_MIGRATION.md) |
| Earlier API contracts | [P1C](P1C_API.md), [P2](P2_API.md), [P3](P3_API.md) |
| Historical guide | [Former Mods README](LEGACY_MOD_GUIDE.md), preserving superseded notes and examples |
| Draft selection ideas | [Equipment notes](EQUIPMENT_SELECTION_NOTES.md); mode/mod scope needs confirmation before implementation |
| Acceptance evidence | [Work log](PRE_DE_WORK_LOG.md), [manual checklist](PRE_DE_TEST_CHECKLIST.md) |
| Runtime investigations | [Character forms](CHARACTER_FORM_RUNTIME_AUDIT.md), [item acquisition](ITEM_ACQUISITION_AUDIT.md), [perk upgrades](PERK_UPGRADE_IMPLEMENTATION.md), [UI](UI_RUNTIME_IMPLEMENTATION.md) |
| Archived XML comparison | [Gap audit](DE_XML_API_GAP_AUDIT.md), [file coverage](DE_XML_FILE_COVERAGE.md), [inventory](DE_XML_FILE_INVENTORY.json), [feature index](DE_XML_FEATURE_INDEX.json), [compressed delta ledger](DE_XML_DELTA_LEDGER.json.gz) |
| XML evidence refresh | [Source drift, portable hashes and the original baseline](XML_AUDIT_REFRESH.md) |
| Configuration comparison | [Classified configuration ledger](PHASE3_CONFIGURATION_AUDIT.json) |

Check the XML and configuration evidence from the repository root:

```sh
python Tools/Audits/AuditDEXmlApi.py
python Tools/Audits/AuditPhase3Configuration.py
```

Both audits check for source drift by default. Their `--write` option regenerates
the ledgers here after review. Archived DE data remains comparison evidence;
canonical base data lives in `Assets/vanillaXml/`.
