"""Check cross-platform XML evidence hashes and detection of real source changes."""
from pathlib import Path
import sys
import tempfile

sys.path.insert(0, str(Path(__file__).resolve().parents[2] / "Audits"))
import AuditDEXmlApi as audit


def main():
    with tempfile.TemporaryDirectory(prefix="EclipseXmlAudit-") as directory:
        audit.ROOT = Path(directory)
        paths = [audit.ROOT / "Assets" / tree / "stages.xml"
                 for tree in ("vanillaXml", "DExml")]
        for path in paths:
            path.parent.mkdir(parents=True)
            path.write_bytes(b'<Root>\n<Item Name="A" Value="1"/>\n</Root>\n')
        baseline = audit.inventory()
        for path in paths:
            path.write_bytes(path.read_bytes().replace(b"\n", b"\r\n"))
        assert audit.inventory() == baseline, "Line endings changed the audit evidence"
        paths[1].write_bytes(paths[1].read_bytes().replace(b'Value="1"', b'Value="2"'))
        changed = audit.inventory()
        assert changed != baseline, "Real source drift was missed"
        entry = changed["entries"][0]
        assert entry["status"] == "changed"
        assert entry["vanilla_sha256"] != entry["de_sha256"]
        assert entry["changes"] == [dict(path='/Root/Item[@Name="A"][1]/@Value',
                                         operation="attribute", before="1", after="2")]
    print("PASS: LF/CRLF checkouts agree; changed XML hashes and attribute deltas are detected.")


if __name__ == "__main__":
    main()
